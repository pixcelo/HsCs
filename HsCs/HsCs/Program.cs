using HsCs;
using HsCs.Models;
using Microsoft.Extensions.Configuration;

namespace hscs
{
    class Program
    {
        static async Task Main()
        {     
            // 設定ファイル読み込み
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var logger = new Logger("app.log", Path.Combine(Directory.GetCurrentDirectory(), "logs"));            

            // APIクライアント
            var bitFlyerClient = new BitFlyerClient(configuration, logger);

            // Tradeing Fee
            double fee = await bitFlyerClient.GetTradingCommissionAsync("FX_BTC_JPY");

            // 約定履歴のリスト
            var executions = new List<BitFlyerExecution>();

            var boffset = GenerateOffsetList(30, -10);
            var soffset = GenerateOffsetList(30, 10);
            const int SECONDS_TO_TRACK = 5;

            var markertMaker = new MarketMaker(SECONDS_TO_TRACK, boffset, soffset);

            // 価格情報を保持
            var priceTracker = new PriceTracker(SECONDS_TO_TRACK);            

            // webSocket接続
            var client = new BitFlyerWebSocketClient(new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));
            await client.StartAsync(async (execution) =>
            {                
                Console.WriteLine($"Executed {execution.Side} {execution.Size} BTC at {execution.Price.ToString("#,0")} JPY ({execution.ExecDate})");

                // 建玉の損切り
                double lossThreshold = 200; // 損切り閾値
                await bitFlyerClient.CutLossAsync("FX_BTC_JPY", lossThreshold);

                //　建玉の利食い
                double profitThreshold = 500;
                await bitFlyerClient.TakeProfitAsync("FX_BTC_JPY", profitThreshold);


                // 約定履歴のリストには、常に最新 N件を保持する
                UpdateExecutions(executions, execution);

                // 中央値を計算
                List<double> prices = priceTracker.GetPrices(execution.ExecDate, execution.Price);
                Console.WriteLine($"The prices count is {prices.Count}");

                int window = 5;

                if (prices.Count > window)
                {
                    // ヒストリカル・ボラティリティを計算
                    var hishistoricalVolatility = new HistoricalVolatility(prices, window);
                    var hv = hishistoricalVolatility.Calculate();

                    var median = new Median(prices);
                    markertMaker.AddMidPrice(median.GetValue());

                    // 約定確率を計算
                    var buyExcutions = executions.Where(x => x.Side == "BUY").ToList();
                    var buyProbabilities = markertMaker.UpdateBuyProbabilities(buyExcutions);
                    var sellExcutions = executions.Where(x => x.Side == "SELL").ToList();
                    var sellProbabilities = markertMaker.UpdateSellProbabilities(sellExcutions);

                    // 収益機会が最大となるBestOffset(B, S)を計算
                    var (bestBuyOffset, bestSellOffset) = markertMaker.CalculateBestOffset(buyProbabilities, sellProbabilities, hv);

                    Console.WriteLine($"bestBuyOffset: {bestBuyOffset.ToString()}, bestSellOffset: {bestSellOffset.ToString()}");
                    //Console.WriteLine($"bestBuyOffset: {(bestBuyOffset + median.GetValue()).ToString()}, bestSellOffset: {(bestSellOffset + median.GetValue()).ToString()}");

                    // 証拠金残高の取得
                    double collateral = await bitFlyerClient.GetCollateral();
                    Console.WriteLine($"証拠金残高: {collateral}");

                    // 注文条件のチェック
                    double orderSize = 0.01;
                    double buyOrderPrice = median.GetValue() + bestBuyOffset;
                    double sellOrderPrice = median.GetValue() + bestSellOffset;

                    Console.WriteLine($"The expected profit is {orderSize * (sellOrderPrice - buyOrderPrice) - fee}");

                    //int leverage = 1;
                    //double requiredMargin = CalculateRequiredMargin(orderSize, buyOrderPrice, leverage);

                    // 現在のアクティブな注文を取得
                    var openOrders = await bitFlyerClient.GetOpenOrders();

                    // 注文の有効期限 TODO:有効期限切れで約定せず、片側だけ建玉を保持した状態が発生する
                    const int MINUTE_TO_EXPIRE = 30;

                    // 未約定の指値注文が存在する場合
                    if (openOrders.Count > 0)
                    {
                        Console.WriteLine("未約定の指値注文が存在します");

                        foreach (var openOrder in openOrders)
                        {
                            // 指値をキャンセル
                            await bitFlyerClient.CancelOrderAsync("FX_BTC_JPY", openOrder.ChildOrderId);
                            Console.WriteLine($"cancel & reOrder : {openOrder.ChildOrderId} {openOrder.Side}");

                            // bestOffsetで指し直す
                            var orderPrice = openOrder.Side == "BUY" ? buyOrderPrice : sellOrderPrice;
                            var orderId = bitFlyerClient.SendOrderAsync("FX_BTC_JPY", openOrder.Side, orderPrice, MINUTE_TO_EXPIRE, orderSize);
                        }

                    }
                    else
                    {
                        // 指値注文                    
                        //var buyOrderId = bitFlyerClient.SendOrderAsync("BTC_JPY", "BUY", median.GetValue() + bestBuyOffset, 0.001);
                        var buyOrderId = await bitFlyerClient.SendOrderAsync("FX_BTC_JPY", "BUY", buyOrderPrice, MINUTE_TO_EXPIRE, orderSize);
                        var sellOrderId = await bitFlyerClient.SendOrderAsync("FX_BTC_JPY", "SELL", sellOrderPrice, MINUTE_TO_EXPIRE, orderSize);
                        Console.WriteLine(buyOrderId == "" ? "The buy order placement failed." : $"place order BUY : {buyOrderId}");
                        Console.WriteLine(sellOrderId == "" ? "The sell order placement failed." : $"place order SELL : {sellOrderId}");
                    }

                    // 建玉があって、反対売買の指値がない場合


                }

            });
        }

        /// <summary>
        /// 必要な証拠金を計算
        /// </summary>
        /// <param name="size"></param>
        /// <param name="price"></param>
        /// <param name="leverage"></param>
        /// <param name="positionMargin"></param>
        /// <param name="orderMargin"></param>
        /// <returns></returns>
        public static double CalculateRequiredMargin(
            double size,
            double price,
            int leverage,
            double positionMargin,
            double orderMargin)
        {
            // 証拠金率を計算
            double marginRate = 1.0 / leverage;

            // 建玉必要証拠金と注文必要証拠金を計算
            double newPositionMargin = size * price * marginRate;
            double newOrderMargin = newPositionMargin + orderMargin;

            // 必要な証拠金額を計算
            double requiredMargin = positionMargin + newOrderMargin;

            return requiredMargin;
        }

        /// <summary>
        /// 約定履歴のリストを更新
        /// </summary>
        /// <param name="executions"></param>
        /// <param name="execution"></param>
        private static void UpdateExecutions(List<BitFlyerExecution> executions, BitFlyerExecution execution)
        {
            const int MAX_COUNT = 10;

            executions.Add(execution);

            // MAXを超えた場合、最も古いアイテムを削除
            if (executions.Count > MAX_COUNT)
            {
                executions.RemoveAt(0);
            }
        }

        /// <summary>
        /// 任意の離散化したステップ価格リスト i…n を作成
        /// </summary>
        /// <returns></returns>
        private static List<double> GenerateOffsetList(int offsetNum, int step)
        {
            var list = new List<double>();

            for (int i = 1; i <= offsetNum; i++)
            {
                list.Add(i * step);
            }

            return list;
        }
    }
}

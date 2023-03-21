using HsCs;
using HsCs.Models;

namespace hscs
{
    class Program
    {
        static async Task Main()
        {
            // 約定履歴のリスト
            var executions = new List<BitFlyerExecution>();

            var boffset = GenerateOffsetList(5, -100);
            var soffset = GenerateOffsetList(5, 100);
            const int SECONDS_TO_TRACK = 10;

            var markertMaker = new MarketMaker(SECONDS_TO_TRACK, boffset, soffset);
            var priceTracker = new PriceTracker(SECONDS_TO_TRACK);            

            // webSocket接続
            var client = new BitFlyerWebSocketClient(new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));
            await client.StartAsync((execution) =>
            {                
                //Console.WriteLine($"Executed {execution.Side} {execution.Size} BTC at {execution.Price} JPY ({execution.ExecDate})");

                // 約定履歴のリストには、常に最新 N件を保持する
                UpdateExecutions(executions, execution);

                // 中央値を計算
                List<double> prices = priceTracker.GetPrices(execution.ExecDate, execution.Price);
                
                int window = 10;                

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
                }

            });
        }

        /// <summary>
        /// 約定履歴のリストを更新
        /// </summary>
        /// <param name="executions"></param>
        /// <param name="execution"></param>
        private static void UpdateExecutions(List<BitFlyerExecution> executions, BitFlyerExecution execution)
        {
            const int MAX_COUNT = 100;

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

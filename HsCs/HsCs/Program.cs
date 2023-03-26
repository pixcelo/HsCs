﻿using HsCs;
using HsCs.Models;
using Microsoft.Extensions.Configuration;

namespace hscs
{
    class Program
    {
        static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var bitFlyerClient = new BitFlyerClient(configuration);

            // 約定履歴のリスト
            var executions = new List<BitFlyerExecution>();

            var boffset = GenerateOffsetList(30, -10);
            var soffset = GenerateOffsetList(30, 10);
            const int SECONDS_TO_TRACK = 5;

            var markertMaker = new MarketMaker(SECONDS_TO_TRACK, boffset, soffset);
            var priceTracker = new PriceTracker(SECONDS_TO_TRACK);            

            // webSocket接続
            var client = new BitFlyerWebSocketClient(new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));
            await client.StartAsync(async (execution) =>
            {                
                Console.WriteLine($"Executed {execution.Side} {execution.Size} BTC at {execution.Price.ToString("#,0")} JPY ({execution.ExecDate})");

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
                    //Console.WriteLine($"bestBuyOffset: {(bestBuyOffset + median.GetValue()).ToString()}, bestSellOffset: {(bestSellOffset + median.GetValue()).ToString()}");

                    // 証拠金残高の取得
                    double collateral = await bitFlyerClient.GetCollateral();
                    Console.WriteLine($"証拠金残高: {collateral}");

                    // 現在のポジション情報を取得
                    var openOrders = await bitFlyerClient.GetOpenOrders();

                    // 未約定の指値注文が存在する場合、注文を出さない
                    if (openOrders.Count > 0)
                    {
                        Console.WriteLine("未約定の指値注文が存在します");
                        return;
                    }

                    // 指値注文
                    //var buyOrderId = bitFlyerClient.SendOrderAsync("BTC_JPY", "BUY", median.GetValue() + bestBuyOffset, 0.001);
                    //var buyOrderId = bitFlyerClient.SendOrderAsync("FX_BTC_JPY", "BUY", median.GetValue() + bestBuyOffset, 0.01);
                    //var sellOrderId = bitFlyerClient.SendOrderAsync("FX_BTC_JPY", "SELL", median.GetValue() + bestSellOffset, 0.01);

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

using HsCs.Models;

namespace HsCs
{
    public class MarketMaker
    {
        private int seconds;
        private double alpha = 1;
        private List<double> Boffset;
        private List<double> Soffset;
        private List<double> recentMidPrices;

        public MarketMaker(int seconds, List<double> boffset, List<double> soffset)
        {
            this.seconds = seconds;
            Boffset = boffset;
            Soffset = soffset;
            recentMidPrices = new List<double>();
        }

        /// <summary>
        /// 中央値を取得
        /// </summary>
        /// <param name="midPrice"></param>
        public void AddMidPrice(double midPrice)
        {
            this.recentMidPrices.Add(midPrice);
            if (recentMidPrices.Count > this.seconds)
            {
                recentMidPrices.RemoveAt(0);
            }
        }

        /// <summary>
        /// 両方の指値が約定する場合の期待収益
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="sp"></param>
        /// <param name="sellPrice"></param>
        /// <param name="buyPrice"></param>
        /// <returns></returns>
        private double CalculateEprofit(double bp, double sp, double sellPrice, double buyPrice)
        {
            return bp * sp * (sellPrice - buyPrice);
        }

        /// <summary>
        /// 直近N秒の中値(S)から収益期待値が最大となるBestOffset(B, S)を算出
        /// </summary>
        /// <param name="buyProbabilities"></param>
        /// <param name="sellProbabilities"></param>
        /// <param name="volatility"></param>
        /// <returns></returns>
        public (double BestBuyOffset, double BestSellOffset) CalculateBestOffset(List<double> buyProbabilities, List<double> sellProbabilities, double volatility)
        {
            double maxE = double.MinValue;
            double bestBuyOffset = 0;
            double bestSellOffset = 0;

            for (int i = 0; i < Boffset.Count; i++)
            {
                for (int j = 0; j < Soffset.Count; j++)
                {
                    double bp = buyProbabilities[i];
                    double sp = sellProbabilities[j];
                    double buyPrice = recentMidPrices.Last() - Boffset[i];
                    double sellPrice = recentMidPrices.Last() + Soffset[j];

                    // 期待値を算出
                    double eprofit = CalculateEprofit(bp, sp, sellPrice, buyPrice);
                    double eloss = (1 - (1 - bp) * (1 - sp) - bp * sp) * volatility * this.alpha;
                    double expected = eprofit - eloss;

                    if (expected > maxE)
                    {
                        maxE = expected;
                        bestBuyOffset = Boffset[i];
                        bestSellOffset = Soffset[j];
                    }
                }
            }

            return (BestBuyOffset: bestBuyOffset, BestSellOffset: bestSellOffset);
        }

        private double CalculateBetaMean(double alpha, double beta)
        {
            return alpha / (alpha + beta);
        }

        /// <summary>
        /// 約定確率を更新（ベイズ更新）
        /// </summary>
        /// <param name="BitFlyerExecution"></param>
        /// <param name="offsets"></param>
        /// <returns></returns>
        private List<double> UpdateProbabilities(List<BitFlyerExecution> executionData, List<double> offsets)
        {
            List<double> updatedProbabilities = new List<double>();

            foreach (double offset in offsets)
            {
                double alpha = 1; // 事前分布のαパラメータ
                double beta = 1;  // 事前分布のβパラメータ

                foreach (var data in executionData)
                {
                    double price = recentMidPrices.Last() + offset;

                    // 買い指値の場合、指値価格よりも安値で約定していたら約定できたと仮定する
                    if (data.Side == "BUY")
                    {
                        if (price >= data.Price)
                        {
                            alpha += 1;
                        }
                        else
                        {
                            beta += 1;
                        }
                    }

                    // 売り指値の場合、指値価格よりも高値で約定していたら約定できたと仮定する
                    if (data.Side == "SELL")
                    {
                        if (price <= data.Price)
                        {
                            alpha += 1;
                        }
                        else
                        {
                            beta += 1;
                        }
                    }

                }

                updatedProbabilities.Add(CalculateBetaMean(alpha, beta));
            }

            return updatedProbabilities;
        }

        public List<double> UpdateBuyProbabilities(List<BitFlyerExecution> buyExecutionData)
        {
            return UpdateProbabilities(buyExecutionData, Boffset);
        }

        public List<double> UpdateSellProbabilities(List<BitFlyerExecution> sellExecutionData)
        {
            return UpdateProbabilities(sellExecutionData, Soffset);
        }
    }
}


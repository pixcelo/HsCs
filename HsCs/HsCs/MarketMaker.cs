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
    }
}


namespace HsCs
{
    /// <summary>
    /// ヒストリカル・ボラティリティを計算するクラス
    /// </summary>
    public class HistoricalVolatility
    {
        private readonly List<double> prices;
        private readonly int window;

        public HistoricalVolatility(List<double> prices, int window)
        {
            this.prices = prices;
            this.window = window;
        }

        /// <summary>
        /// ヒストリカル・ボラティリティを計算
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double Calculate()
        {
            //if (window < 2 || window > prices.Count)
            //{
            //    throw new ArgumentException($"Invalid window size. Must be between 2 and {prices.Count}");
            //}

            List<double> returns = CalculateReturns();
            return CalculateStandardDeviation(returns);
        }

        /// <summary>
        /// 連続リターン（連続成長率）を計算
        /// 連続リターンは、資産の価格変動をパーセント表現から自然対数表現に変換する方法で、これにより計算が容易になる
        /// </summary>
        /// <returns></returns>
        private List<double> CalculateReturns()
        {
            var returns = new List<double>();

            for (int i = 1; i < prices.Count; i++)
            {
                double dailyReturn = Math.Log(prices[i] / prices[i - 1]);
                returns.Add(dailyReturn);
            }

            return returns;
        }

        /// <summary>
        /// 標準偏差を計算
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private double CalculateStandardDeviation(List<double> dailyReturns)
        {
            var squaredDeviations = new List<double>();
            double average = dailyReturns.Skip(dailyReturns.Count - this.window).Take(this.window).Average();

            for (int i = dailyReturns.Count - this.window; i < dailyReturns.Count; i++)
            {
                double deviation = dailyReturns[i] - average;
                squaredDeviations.Add(deviation * deviation);
            }

            double variance = squaredDeviations.Average();
            double standardDeviation = Math.Sqrt(variance);
            int annualConversionFactor = 365; // 年間換算の係数

            return standardDeviation * Math.Sqrt(annualConversionFactor); // Annualized Volatility
        }

    }
}

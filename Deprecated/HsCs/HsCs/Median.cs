namespace HsCs
{
    public class Median
    {
        private readonly List<double> prices;
        private double value;

        public Median(List<double> prices)
        {
            this.prices = prices;

            Calculate();
        }

        public double GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// 中央値を計算
        /// </summary>
        /// <returns></returns>
        private void Calculate()
        {
            int count = prices.Count;

            if (count == 0)
            {
                this.value = 0;
            }

            prices.Sort();

            if (count % 2 == 0)
            {
                // 偶数の場合、真ん中のデータ２つの平均
                this.value = (prices[count / 2] + prices[(count / 2) - 1]) / 2;
            }
            
            // 奇数の場合、真ん中のデータ
            this.value = prices[count / 2];
        }        
    }
}

namespace HsCs
{
    public class ExecutionProbability
    {
        private readonly double median;
        private readonly int step;
        private readonly int offsetNum;
        private readonly List<double> prices;
        private readonly List<double> offsetList;

        public ExecutionProbability(double median, int step, int offsetNum, List<double> prices)
        {
            this.median = median;
            this.step = step;
            this.offsetNum = offsetNum;
            this.prices = prices;
            
            offsetList = GenerateOffsetList();
        }

        /// <summary>
        /// 任意の離散化したステップ価格リスト i…n を作成
        /// </summary>
        /// <returns></returns>
        private List<double> GenerateOffsetList()
        {
            var list = new List<double>();

            for (int i = 1; i <= this.offsetNum; i++)
            {
                list.Add(this.median + i * this.step);
            }

            return list;
        }

        /// <summary>
        /// ステップ価格ごとの約定回数を取得
        /// </summary>
        /// <returns></returns>
        private List<int> CountExecutions(string side)
        {
            var executions = new List<int>(new int[this.offsetNum]);

            foreach (double price in this.prices)
            {
                for (int i = 0; i < this.offsetNum; i++)
                {
                    // 買い指値の場合、指値価格よりも安値で約定していたら約定できたと仮定する
                    if (side == "BUY"　&& this.offsetList[i] >= price)
                    {
                        executions[i]++;
                    }
                    
                    // 売り指値の場合、指値価格よりも高値で約定していたら約定できたと仮定する
                    if (side == "SELL" && this.offsetList[i] <= price)
                    {
                        executions[i]++;
                    }
                }
            }

            return executions;
        }

        /// <summary>
        /// 約定確率を計算する
        /// </summary>
        /// <returns></returns>
        public double CalculateExecutionProbability(string side)
        {
            int totalTrades = this.prices.Count;
            var executions = CountExecutions(side);

            if (executions.Count == 0) return 0;

            return (double)executions.Count / totalTrades;
        }
    }

}

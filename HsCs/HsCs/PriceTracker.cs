namespace HsCs
{
    /// <summary>
    /// 最新のN秒間の価格情報を保持するクラス
    /// </summary>

    public class PriceTracker
    {
        private readonly List<double> prices;
        private readonly int window;
        private readonly TimeSpan interval;
        private DateTime lastUpdateTime;

        public PriceTracker(int secondsToTrack)
        {
            prices = new List<double>();
            window = secondsToTrack;
            interval = TimeSpan.FromSeconds(secondsToTrack);
            lastUpdateTime = DateTime.MinValue;
        }

        public List<double> GetPrices(DateTime execDate, double price)
        {
            if (execDate - lastUpdateTime >= interval)
            {
                RemoveOldPrices(execDate);
                lastUpdateTime = execDate;
            }

            prices.Add(price);
#if DEBUG
            GetTimeRange();
#endif
            return prices.ToList();
        }

        private void RemoveOldPrices(DateTime execDate)
        {
            prices.RemoveAll(p => (execDate - lastUpdateTime) > interval);
        }

        private void GetTimeRange()
        {
            var endTime = DateTime.Now;
            var startTime = endTime.AddSeconds(window);
            Console.WriteLine($"PriceTracker is holding prices from {startTime.ToString()} to {endTime.ToString()}.");
            Console.WriteLine($"PriceTracker has {prices.Count} prices.");
        }
    }
}

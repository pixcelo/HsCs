using System.Text.Json.Serialization;

namespace HsCs.Models
{
    public class BitFlyerExecution
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("side")]
        public string Side { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("size")]
        public double Size { get; set; }

        [JsonPropertyName("exec_date")]
        public DateTime ExecDate { get; set; }

        [JsonPropertyName("buy_child_order_acceptance_id")]
        public string BuyChildOrderAcceptanceId { get; set; }

        [JsonPropertyName("sell_child_order_acceptance_id")]
        public string SellChildOrderAcceptanceId { get; set; }
    }
}

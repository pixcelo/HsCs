using System.Text.Json.Serialization;

namespace HsCs.Models
{
    public class BitFlyerPosition
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; set; }

        [JsonPropertyName("side")]
        public string Side { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("size")]
        public double Size { get; set; }

        [JsonPropertyName("commission")]
        public double Commission { get; set; }

        [JsonPropertyName("swap_point_accumulate")]
        public double SwapPointAccumulate { get; set; }

        [JsonPropertyName("require_collateral")]
        public double RequireCollateral { get; set; }

        [JsonPropertyName("open_date")]
        public DateTimeOffset OpenDate { get; set; }

        [JsonPropertyName("leverage")]
        public double Leverage { get; set; }

        [JsonPropertyName("pnl")]
        public double Pnl { get; set; }

        [JsonPropertyName("sfd")]
        public double Sfd { get; set; }
    }
}

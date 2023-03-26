using System.Text.Json.Serialization;

namespace HsCs.Models
{
    public class BitFlyerCollateralInfo
    {
        [JsonPropertyName("collateral")]
        public double Collateral { get; set; }

        [JsonPropertyName("open_position_pnl")]
        public double OpenPositionPnl { get; set; }

        [JsonPropertyName("require_collateral")]
        public double RequireCollateral { get; set; }

        [JsonPropertyName("keep_rate")]
        public double KeepRate { get; set; }

        [JsonPropertyName("margin_call_amount")]
        public double MarginCallAmount { get; set; }

        [JsonPropertyName("margin_call_due_date")]
        public string MarginCallDueDate { get; set; }
    }
}

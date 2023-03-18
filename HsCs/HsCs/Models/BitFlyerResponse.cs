using System.Text.Json.Serialization;

namespace HsCs.Models
{
    public class BitFlyerResponse
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("message")]
        public BitFlyerExecution[] Message { get; set; }
    }
}

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HsCs
{
    public class BitFlyerWebSocketClient
    {
        private readonly Uri _uri;
        private ClientWebSocket _webSocket;

        public BitFlyerWebSocketClient(Uri uri)
        {
            _uri = uri;
        }

        public async Task StartAsync(Action<BitFlyerExecution> onExecution)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(_uri, CancellationToken.None);

            var jsonRequest = Encoding.UTF8.GetBytes(@"{""jsonrpc"":""2.0"",""method"":""subscribe"",""params"":{""channel"":""lightning_executions_FX_BTC_JPY""},""id"":""1""}");
            await _webSocket.SendAsync(new ArraySegment<byte>(jsonRequest), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024 * 4];
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // Ignore any message that is not a JSON - RPC notification
                    if (!jsonString.StartsWith(@"{""jsonrpc"":""2.0"",""method"":""", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var jsonDocument = JsonDocument.Parse(jsonString);
                    var respnse = JsonSerializer.Deserialize<BitFlyerResponse>(jsonDocument.RootElement.GetProperty("params").GetRawText(), jsonOptions);

                    foreach (var execution in respnse.Message)
                    {
                        onExecution?.Invoke(execution);
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocketException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public async Task StopAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
    }    

    public class BitFlyerResponse
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("message")]
        public BitFlyerExecution[] Message { get; set; }
    }

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

using System.Net.WebSockets;
using System.Text.Json;

namespace hscs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var url = new Uri("wss://stream.bybit.com/realtime_public");
            var url = new Uri("wss://stream-testnet.bybit.com/v5/public/linear");
            using (var client = new ClientWebSocket())
            {
                await client.ConnectAsync(url, CancellationToken.None);
                Console.WriteLine("WebSocket connected");

                //var channel = "instrument_info.100ms.BTCUSDT";
                var channel = "kline.1.BTCUSDT";
                var request = new { op = "subscribe", args = new string[] { channel } };
                var json = JsonSerializer.Serialize(request);

                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                var buffer = new ArraySegment<byte>(bytes);

                await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                var responseBuffer = new byte[1024 * 4];
                while (client.State == WebSocketState.Open)
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        Console.WriteLine("WebSocket closed");
                    }
                    else
                    {
                        var message = System.Text.Encoding.UTF8.GetString(responseBuffer, 0, result.Count);
                        Console.WriteLine($"Received message: {message}");
                    }
                }
            }
        }
    }
}

using HsCs.Models;
using System;
using System.Net.Http;
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
        private readonly HttpClient _httpClient = new HttpClient();

        public BitFlyerWebSocketClient(Uri uri)
        {
            _uri = uri;
        }

        public async Task StartAsync(Action<BitFlyerExecution> onExecution)
        {
            while (await IsTradingSuspended())
            {
                Console.WriteLine("メンテナンス中です。再試行します。");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

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
                    var response = JsonSerializer.Deserialize<BitFlyerResponse>(jsonDocument.RootElement.GetProperty("params").GetRawText(), jsonOptions);

                    foreach (var execution in response.Message)
                    {
                        onExecution?.Invoke(execution);
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocketException: {ex.Message}");
                    await ReStartAsync(onExecution);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    await ReStartAsync(onExecution);
                }
            }

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        private async Task ReStartAsync(Action<BitFlyerExecution>  onExecution)
        {
            Console.WriteLine($"Restart WebSocket");
            await Task.Delay(5000);

            while (await IsTradingSuspended())
            {
                Console.WriteLine("メンテナンス中です。再試行します。");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            await StartAsync(onExecution);
        }

        public async Task StopAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }

        /// <summary>
        /// 取引所の状態を確認：毎日午前 4 時 00 分～午前 4 時 10 分、定期メンテナンス（時間帯は前後する）
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsTradingSuspended()
        {
            var response = await _httpClient.GetAsync("https://api.bitflyer.com/v1/gethealth");
            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseContent);
            var status = jsonDocument.RootElement.GetProperty("status").GetString();
            return status == "STOP";
        }
    }
}

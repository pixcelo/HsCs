﻿using System;
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
                    Console.WriteLine($"Restart WebSocket");
                    await Task.Delay(5000);
                    await StartAsync(onExecution);
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

        // TODO: 修正
        /// <summary>
        /// 取引所の状態を確認：毎日午前 4 時 00 分～午前 4 時 10 分、定期メンテナンス（時間帯は前後する）
        /// </summary>
        /// <returns></returns>
        //private async Task<bool> IsTradingSuspended()
        //{
        //    var response = await _httpClient.GetAsync("https://api.bitflyer.com/v1/gethealth");
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    var responseJson = JObject.Parse(responseContent);
        //    var status = responseJson.Value<string>("status");
        //    return status == "STOP";
        //}
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

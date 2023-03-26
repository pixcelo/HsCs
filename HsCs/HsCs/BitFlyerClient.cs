using HsCs.Models;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HsCs
{
    public class BitFlyerClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public BitFlyerClient(IConfiguration configuration)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://api.bitflyer.com") };
            _apiKey = configuration["BitFlyer:ApiKey"];
            _apiSecret = configuration["BitFlyer:ApiSecret"];
        }

        public async Task<string> SendOrderAsync(
            string productCode,
            string side,
            double price,
            int minute_to_expire,
            double size)
        {
            var path = "/v1/me/sendchildorder";
            var body = JsonSerializer.Serialize(new
            {
                product_code = productCode,
                child_order_type = "LIMIT",
                side,
                price,
                minute_to_expire, // 期限切れまでの時間を分で指定: 省略した場合の値は 43200 (30 日間)
                size
            });

            var response = await SendRequestAsync(HttpMethod.Post, path, body);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);

            return jsonDocument.RootElement.GetProperty("child_order_acceptance_id").GetString();
        }

        public async Task<string> SendMarketOrderAsync(string productCode, string side, double size)
        {
            var path = "/v1/me/sendchildorder";
            var body = JsonSerializer.Serialize(new
            {
                product_code = productCode,
                child_order_type = "MARKET",
                side,
                size
            });

            var response = await SendRequestAsync(HttpMethod.Post, path, body);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);

            return jsonDocument.RootElement.GetProperty("child_order_acceptance_id").GetString();
        }


        public async Task<bool> CancelOrderAsync(string productCode, string orderId)
        {
            var path = $"/v1/me/cancelchildorder";
            var body = JsonSerializer.Serialize(new
            {
                product_code = productCode,
                child_order_acceptance_id = orderId
            });

            var response = await SendRequestAsync(HttpMethod.Post, path, body);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string path, string body)
        {
            var request = new HttpRequestMessage(method, path);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var message = timestamp + method.ToString() + path + body;

            var signature = CreateSignature(_apiSecret, message);

            request.Headers.Add("ACCESS-KEY", _apiKey);
            request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
            request.Headers.Add("ACCESS-SIGN", signature);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(request);
        }

        /// <summary>
        /// 証拠金の状態を取得
        /// </summary>
        /// <returns></returns>
        public async Task<double> GetCollateral()
        {
            string path = "/v1/me/getcollateral";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string method = "GET";
            string body = "";

            var message = timestamp + method + path + body;

            var signature = CreateSignature(_apiSecret, message);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.bitflyer.com" + path);
            request.Headers.Add("ACCESS-KEY", _apiKey);
            request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
            request.Headers.Add("ACCESS-SIGN", signature);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var collateralInfo = JsonSerializer.Deserialize<BitFlyerCollateralInfo>(jsonResponse, jsonSerializerOptions);
            return collateralInfo.Collateral;
        }

        /// <summary>
        /// アクティブな注文一覧を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<BitFlyerOrder>> GetOpenOrders()
        {
            string path = "/v1/me/getchildorders?product_code=FX_BTC_JPY&child_order_state=ACTIVE";            
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string method = "GET";
            string body = "";

            var message = timestamp + method + path + body;

            var signature = CreateSignature(_apiSecret, message);
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.bitflyer.com" + path);
            request.Headers.Add("ACCESS-KEY", _apiKey);
            request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
            request.Headers.Add("ACCESS-SIGN", signature);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<BitFlyerOrder>>(jsonResponse, jsonSerializerOptions);
        }

        /// <summary>
        /// 建玉を閾値で損切する
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="lossThreshold"></param>
        /// <returns></returns>
        public async Task CutLossAsync(string productCode, double lossThreshold)
        {
            // 建玉一覧を取得
            var positions = await GetPositionsAsync(productCode);

            // pnl のマイナスが lossThreshold 以下の建玉をフィルタリング
            var positionsToCut = positions.Where(p => p.Pnl <= -lossThreshold).ToList();

            // 損切りするために、ポジションごとに成行注文を発注
            foreach (var position in positionsToCut)
            {
                string side = position.Side == "BUY" ? "SELL" : "BUY";
                await SendMarketOrderAsync(productCode, side, position.Size);
                Console.WriteLine($"Loss cut {position.Side} posistion");
            }
        }

        /// <summary>
        /// 建玉一覧を取得
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        private async Task<List<BitFlyerPosition>> GetPositionsAsync(string productCode)
        {
            string path = $"/v1/me/getpositions?product_code={productCode}";
           
            var response = await SendRequestAsync(HttpMethod.Get, path, string.Empty);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BitFlyerPosition>>(jsonResponse);
        }

        

        private string CreateSignature(string secret, string message)
        {
            using var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }
}

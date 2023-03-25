using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HsCs.Models;
using Microsoft.Extensions.Configuration;

namespace HsCs
{
    class BitFlyerClient
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

        public async Task<string> SendOrderAsync(string productCode, string side, double price, double size)
        {
            var path = "/v1/me/sendchildorder";
            var body = JsonSerializer.Serialize(new
            {
                product_code = productCode,
                child_order_type = "LIMIT",
                side,
                price,
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
        /// アクティブな注文一覧を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<BitFlyerOrder>> GetOpenOrders()
        {
            string method = "GET";
            string path = "/v1/me/getchildorders?product_code=FX_BTC_JPY&child_order_state=ACTIVE";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string sign = GenerateSignature(timestamp, method, path, _apiSecret);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ACCESS-KEY", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", timestamp);
            _httpClient.DefaultRequestHeaders.Add("ACCESS-SIGN", sign);
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            HttpResponseMessage response = await _httpClient.GetAsync("https://api.bitflyer.com" + path);
            string content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BitFlyerOrder>>(content);
        }

        private string CreateSignature(string secret, string message)
        {
            using var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private string GenerateSignature(string timestamp, string method, string path, string apiSecret, string requestBody = "")
        {
            string text = timestamp + method + path + requestBody;
            var encoding = new UTF8Encoding();
            byte[] key = encoding.GetBytes(apiSecret);
            byte[] source = encoding.GetBytes(text);
            using HMACSHA256 hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(source);
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }
    }
}

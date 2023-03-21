using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        private string CreateSignature(string secret, string message)
        {
            using var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}

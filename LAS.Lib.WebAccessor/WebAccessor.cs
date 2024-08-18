using System.Text;
using System.Text.Json;

namespace LAS.Lib.WebAccessor
{
    public class WebAccessor
    {
        private static HttpClient? httpClient;

        private static readonly string baseUrl = "https://localhost:7037/";

        /// <summary>
        /// HttpClientのインスタンスを取得する
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateClient()
        {
            if (httpClient is null)
            {
                httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(baseUrl)
                };
            }

            return httpClient;
        }

        /// <summary>
        /// WebAPIにリクエストを送信する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiPath"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<T?> SendRequestAsync<T>(
            string apiPath,
            HttpMethod method,
            object? content = null)
        {
            try
            {
                var client = CreateClient();
                var request = new HttpRequestMessage(method, client.BaseAddress + apiPath);

                if (content != null)
                {
                    var jsonContent = JsonSerializer.Serialize(content);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);                
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return default;
            }
        }
    }
}

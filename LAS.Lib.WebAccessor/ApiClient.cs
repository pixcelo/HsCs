using System.Text;
using System.Text.Json;

namespace LAS.Lib.WebAccessor
{
    public class ApiClient
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApiClient()
        {
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// GETメソッド
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>レスポンス文字列</returns>
        public async Task<string> GetAsync(string url)
        {
            try
            {
                var response = await this.httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// POSTメソッド
        /// </summary>
        /// <typeparam name="T">送信するデータの型</typeparam>
        /// <param name="url">URL</param>
        /// <param name="data">送信するデータ</param>
        /// <returns>レスポンス文字列</returns>
        public async Task<string> PostAsync<T>(string url, T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await this.httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }
    }
}

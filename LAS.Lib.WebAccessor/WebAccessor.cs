using LAS.Domain.Models;
using System.Text;
using System.Text.Json;

namespace LAS.Lib.WebAccessor
{
    public class WebAccessor
    {
        private static HttpClient? httpClient;

        private static readonly string baseUrl = "https://localhost:5001/";

        /// <summary>
        /// HttpClientのインスタンスを取得する
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateClient()
        {
            if (httpClient is null)
            {
                httpClient = new HttpClient();
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
        private static async Task<T?> SendRequestAsync<T>(
            string apiPath,
            HttpMethod method,
            object? content = null)
        {
            try
            {
                var client = CreateClient();
                var request = new HttpRequestMessage(method, baseUrl + apiPath);

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

        public static Task<IEnumerable<TodoItem>?> GetTodoItemsAsync()
        {
            return SendRequestAsync<IEnumerable<TodoItem>>("api/TodoItems", HttpMethod.Get);
        }

        public static Task<TodoItem?> CreateTodoItemAsync(TodoItem item)
        {
            return SendRequestAsync<TodoItem>("api/TodoItems", HttpMethod.Post, item);
        }

        public static Task<TodoItem?> UpdateTodoItemAsync(TodoItem item)
        {
            return SendRequestAsync<TodoItem>($"api/TodoItems/{item.Id}", HttpMethod.Put, item);
        }

        public static Task<bool> DeleteTodoItemAsync(int id)
        {
            return SendRequestAsync<bool>($"api/TodoItems/{id}", HttpMethod.Delete);
        }
    }
}

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

        public async Task<string> GetAsync(string url)
        {
            try
            {
                var response = await this.httpClient.GetAsync(url);
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

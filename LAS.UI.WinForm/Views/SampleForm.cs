using LAS.Domain.Models;
using LAS.Lib.WebAccessor;

namespace LAS.UI.WinForm.Views
{
    public partial class SampleForm : Form
    {
        private readonly ApiClient apiClient;

        public SampleForm(ApiClient apiClient)
        {
            InitializeComponent();

            this.apiClient = apiClient;
        }

        /// <summary>
        /// GETボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void getButton_Click(object sender, EventArgs e)
        {
            // API呼び出しのテスト
            string endpoint = "https://localhost:7037/api/sample/get";
            string result = await apiClient.GetAsync(endpoint);

            if (result != null)
            {
                MessageBox.Show(result, "API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to get response from API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// POSTボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void postButton_Click(object sender, EventArgs e)
        {            
            string endpoint = "https://localhost:7037/api/sample";
            var data = new SampleModel() { Id = 1, Name = "Sample", Value = "AAA" };

            try
            {
                var result = await apiClient.PostAsync(endpoint, data);

                MessageBox.Show(result,
                    "API Response",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to get response from API. Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

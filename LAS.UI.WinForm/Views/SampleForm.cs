using LAS.Lib.WebAccessor;

namespace LAS.UI.WinForm.Views
{
    public partial class SampleForm : Form
    {
        private readonly ApiClient apiClient;

        public SampleForm()
        {
            InitializeComponent();

            this.apiClient = new ApiClient();
        }

        /// <summary>
        /// ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void getbutton_Click(object sender, EventArgs e)
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
    }
}

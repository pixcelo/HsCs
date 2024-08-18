using LAS.Lib.WebAccessor;

namespace LAS.UI.WinForm.Views
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void getButton_Click(object sender, EventArgs e)
        {
            var todoItems = await TodoItemsAccessor.GetTodoItemsAsync();            
            this.todoItemDataGridView.DataSource = todoItems;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void postButton_Click(object sender, EventArgs e)
        {

        }
    }
}

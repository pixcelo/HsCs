namespace LAS.UI.WinForm.Views
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.geButton = new Button();
            this.postButton = new Button();
            this.todoItemDataGridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)this.todoItemDataGridView).BeginInit();
            this.SuspendLayout();
            // 
            // geButton
            // 
            this.geButton.Location = new Point(64, 97);
            this.geButton.Margin = new Padding(3, 4, 3, 4);
            this.geButton.Name = "geButton";
            this.geButton.Size = new Size(86, 31);
            this.geButton.TabIndex = 0;
            this.geButton.Text = "get";
            this.geButton.UseVisualStyleBackColor = true;
            this.geButton.Click += this.getButton_Click;
            // 
            // postButton
            // 
            this.postButton.Location = new Point(171, 97);
            this.postButton.Margin = new Padding(3, 4, 3, 4);
            this.postButton.Name = "postButton";
            this.postButton.Size = new Size(86, 31);
            this.postButton.TabIndex = 1;
            this.postButton.Text = "post";
            this.postButton.UseVisualStyleBackColor = true;
            this.postButton.Click += this.postButton_Click;
            // 
            // todoItemDataGridView
            // 
            this.todoItemDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.todoItemDataGridView.Dock = DockStyle.Bottom;
            this.todoItemDataGridView.Location = new Point(0, 173);
            this.todoItemDataGridView.Margin = new Padding(3, 4, 3, 4);
            this.todoItemDataGridView.Name = "todoItemDataGridView";
            this.todoItemDataGridView.RowHeadersWidth = 51;
            this.todoItemDataGridView.Size = new Size(450, 200);
            this.todoItemDataGridView.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(450, 373);
            this.Controls.Add(this.todoItemDataGridView);
            this.Controls.Add(this.postButton);
            this.Controls.Add(this.geButton);
            this.Margin = new Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)this.todoItemDataGridView).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Button geButton;
        private Button postButton;
        private DataGridView todoItemDataGridView;
    }
}

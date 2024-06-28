namespace LAS.UI.WinForm.Views
{
    partial class SampleForm
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
            getbutton = new Button();
            SuspendLayout();
            // 
            // getbutton
            // 
            getbutton.Location = new Point(56, 73);
            getbutton.Name = "getbutton";
            getbutton.Size = new Size(75, 23);
            getbutton.TabIndex = 0;
            getbutton.Text = "get";
            getbutton.UseVisualStyleBackColor = true;
            getbutton.Click += getbutton_Click;
            // 
            // SampleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(394, 450);
            Controls.Add(getbutton);
            Name = "SampleForm";
            Text = "SampleForm";
            ResumeLayout(false);
        }

        #endregion

        private Button getbutton;
    }
}

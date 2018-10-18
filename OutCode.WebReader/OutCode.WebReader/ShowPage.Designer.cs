namespace OutCode.WebReader
{
    partial class ShowPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // myWebBrowser
            // 
            this.myWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.myWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.myWebBrowser.Name = "myWebBrowser";
            this.myWebBrowser.Size = new System.Drawing.Size(908, 595);
            this.myWebBrowser.TabIndex = 0;
            // 
            // ShowPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 595);
            this.Controls.Add(this.myWebBrowser);
            this.Name = "ShowPage";
            this.Text = "ShowPage";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser myWebBrowser;
    }
}
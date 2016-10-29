namespace OutCode.Robot
{
    partial class SVC_Robot
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Log = new System.Diagnostics.EventLog();
            this.PdfWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.Log)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PdfWatcher)).BeginInit();
            // 
            // Log
            // 
            this.Log.Log = "Application";
            this.Log.Source = "OutCode.Robot";
            // 
            // PdfWatcher
            // 
            this.PdfWatcher.EnableRaisingEvents = true;
            this.PdfWatcher.Filter = "*.pdf";
            this.PdfWatcher.Path = Properties.Settings.Default.PDFDOCPATH;
            this.PdfWatcher.Created += new System.IO.FileSystemEventHandler(this.PdfWatcher_Created);
            // 
            // SVC_Robot
            // 
            this.ServiceName = "OutCode.Robot";
            ((System.ComponentModel.ISupportInitialize)(this.Log)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PdfWatcher)).EndInit();

        }

        #endregion

        internal System.Diagnostics.EventLog Log;
        private System.IO.FileSystemWatcher PdfWatcher;
    }
}

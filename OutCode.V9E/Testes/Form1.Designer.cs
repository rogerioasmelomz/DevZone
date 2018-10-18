namespace Testes
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.checkmail = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.picQR = new System.Windows.Forms.PictureBox();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkmail
            // 
            this.checkmail.Location = new System.Drawing.Point(52, 129);
            this.checkmail.Name = "checkmail";
            this.checkmail.Size = new System.Drawing.Size(76, 24);
            this.checkmail.TabIndex = 1;
            this.checkmail.Text = "Ckeck Mail";
            this.checkmail.UseVisualStyleBackColor = true;
            this.checkmail.Click += new System.EventHandler(this.checkmail_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(53, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(369, 38);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(327, 20);
            this.txtInfo.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(609, 70);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 19);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // picQR
            // 
            this.picQR.Location = new System.Drawing.Point(376, 125);
            this.picQR.Name = "picQR";
            this.picQR.Size = new System.Drawing.Size(318, 236);
            this.picQR.TabIndex = 5;
            this.picQR.TabStop = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(52, 196);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "FTP";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 393);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.picQR);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkmail);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button checkmail;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox picQR;
        private System.Windows.Forms.Button button4;
    }
}


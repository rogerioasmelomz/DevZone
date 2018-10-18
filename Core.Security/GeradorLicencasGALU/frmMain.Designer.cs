namespace GeradorLicencasGALU
{
    partial class frmMain
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
            this.chkLTecnica = new System.Windows.Forms.CheckBox();
            this.dtInicio = new System.Windows.Forms.DateTimePicker();
            this.dtFim = new System.Windows.Forms.DateTimePicker();
            this.txtNUIT = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtNumCliPrimavera = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVerERP = new System.Windows.Forms.TextBox();
            this.btGerarLic = new System.Windows.Forms.Button();
            this.btLimpar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkLTecnica
            // 
            this.chkLTecnica.AutoSize = true;
            this.chkLTecnica.Location = new System.Drawing.Point(124, 167);
            this.chkLTecnica.Name = "chkLTecnica";
            this.chkLTecnica.Size = new System.Drawing.Size(106, 17);
            this.chkLTecnica.TabIndex = 6;
            this.chkLTecnica.Text = "Licença Técnica";
            this.chkLTecnica.UseVisualStyleBackColor = true;
            // 
            // dtInicio
            // 
            this.dtInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtInicio.Location = new System.Drawing.Point(124, 96);
            this.dtInicio.Name = "dtInicio";
            this.dtInicio.Size = new System.Drawing.Size(158, 20);
            this.dtInicio.TabIndex = 4;
            // 
            // dtFim
            // 
            this.dtFim.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFim.Location = new System.Drawing.Point(124, 132);
            this.dtFim.Name = "dtFim";
            this.dtFim.Size = new System.Drawing.Size(158, 20);
            this.dtFim.TabIndex = 5;
            // 
            // txtNUIT
            // 
            this.txtNUIT.Location = new System.Drawing.Point(124, 28);
            this.txtNUIT.Name = "txtNUIT";
            this.txtNUIT.Size = new System.Drawing.Size(158, 20);
            this.txtNUIT.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "NUIT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nº Lic. Primavera";
            // 
            // TxtNumCliPrimavera
            // 
            this.TxtNumCliPrimavera.Location = new System.Drawing.Point(124, 62);
            this.TxtNumCliPrimavera.Name = "TxtNumCliPrimavera";
            this.TxtNumCliPrimavera.Size = new System.Drawing.Size(158, 20);
            this.TxtNumCliPrimavera.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Data Inicio Licença";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Data Fim Licença";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(313, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Versão ERP";
            // 
            // txtVerERP
            // 
            this.txtVerERP.Location = new System.Drawing.Point(395, 28);
            this.txtVerERP.Name = "txtVerERP";
            this.txtVerERP.Size = new System.Drawing.Size(158, 20);
            this.txtVerERP.TabIndex = 2;
            // 
            // btGerarLic
            // 
            this.btGerarLic.Location = new System.Drawing.Point(395, 144);
            this.btGerarLic.Name = "btGerarLic";
            this.btGerarLic.Size = new System.Drawing.Size(156, 40);
            this.btGerarLic.TabIndex = 7;
            this.btGerarLic.Text = "Gerar Licença";
            this.btGerarLic.UseVisualStyleBackColor = true;
            this.btGerarLic.Click += new System.EventHandler(this.btGerarLic_Click);
            // 
            // btLimpar
            // 
            this.btLimpar.Location = new System.Drawing.Point(395, 96);
            this.btLimpar.Name = "btLimpar";
            this.btLimpar.Size = new System.Drawing.Size(156, 40);
            this.btLimpar.TabIndex = 12;
            this.btLimpar.Text = "Limpar";
            this.btLimpar.UseVisualStyleBackColor = true;
            this.btLimpar.Click += new System.EventHandler(this.btLimpar_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(568, 202);
            this.Controls.Add(this.btLimpar);
            this.Controls.Add(this.btGerarLic);
            this.Controls.Add(this.txtVerERP);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtNumCliPrimavera);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNUIT);
            this.Controls.Add(this.dtFim);
            this.Controls.Add(this.dtInicio);
            this.Controls.Add(this.chkLTecnica);
            this.Name = "frmMain";
            this.Text = "Licenciamento Gestão de Alunos";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkLTecnica;
        private System.Windows.Forms.DateTimePicker dtInicio;
        private System.Windows.Forms.DateTimePicker dtFim;
        private System.Windows.Forms.TextBox txtNUIT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtNumCliPrimavera;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVerERP;
        private System.Windows.Forms.Button btGerarLic;
        private System.Windows.Forms.Button btLimpar;
    }
}


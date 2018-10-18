using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Security;
using Core.IO;

namespace GeradorLicencasGALU
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btLimpar_Click(object sender, EventArgs e)
        {

            txtNUIT.Text = "";
            TxtNumCliPrimavera.Text  = "";
            txtVerERP.Text = "";

            dtFim.Value = DateTime.Now.AddMonths(5)  ;
            dtInicio.Value = DateTime.Now;   

        }

        private void btGerarLic_Click(object sender, EventArgs e)
        {

            string strText;
            string strNuit;

            IniFile ini;

            try
            {

                if (string.IsNullOrWhiteSpace(txtNUIT.Text) || string.IsNullOrWhiteSpace(TxtNumCliPrimavera.Text) || string.IsNullOrWhiteSpace(TxtNumCliPrimavera.Text))
                {
                    MessageBox.Show("Existem campos não preenchidos!", "Atenção");

                }
                else
                {

                    strNuit = string.Format("{0}{1}", (chkLTecnica.Checked ? "LT" : ""), txtNUIT.Text);

                    strText = string.Format("{0}|{1}|{2}|{3}|{4}", strNuit, TxtNumCliPrimavera.Text, dtInicio.Value.ToShortDateString(), dtFim.Value.ToShortDateString(), txtVerERP.Text);

                    ini = new IniFile(String.Format("{0}.lic", strNuit));


                    string encryptedstring = StringCipher.Thing(strText, strNuit);
                    string encryptedKey = StringCipher.Thing(strNuit, "CSULICENSE.LIC");


                    ini.Write("L0", encryptedKey, "GALU");
                    ini.Write("L1", encryptedstring, "GALU");

                    MessageBox.Show("Licença gerada com sucesso!", "Licenciamento"); 
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {
                ini = null;
            }
            


        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            dtFim.Value = DateTime.Now.AddMonths(5);
            dtInicio.Value = DateTime.Now;
        }
    }
}

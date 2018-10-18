using OutCode.Cambios;
using OutCode.WebReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowTester
{
    public partial class Form1 : Form
    {
        Page p;
        OutCode.Cambios.MZ _cambios;

        public Form1()
        {
            InitializeComponent();

            _cambios = new OutCode.Cambios.MZ();

            ShowButtons (false);

            _cambios.EventoCambioLido += new OutCode.Cambios.MZ.EventoCambiosObtidos(CambioObtido);  

        }

        private void CambioObtido(string Banco,bool Enable)
        {
            switch(Banco){
                case "BU":
                    button1.Visible  = true;
                    button1.Enabled = Enable; 
                    break;
                case "MOZA":
                    button4.Visible = true;
                    button4.Enabled = Enable;
                    break;
                case "BIM":
                    button3.Visible = true;
                    button3.Enabled = Enable;
                    break;
                case "BCI":
                    button2.Visible = true;
                    button2.Enabled = Enable;
                    break;
                case "BMZ":
                    button5.Visible = true;
                    button5.Enabled = Enable;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowBanco(_cambios.Bancos[0]);
        }

        private void GetFromUnico()
        {
            tbInfo.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine  , p.GetInfo("EUR", 17), p.GetInfo("USD", 17), p.GetInfo("ZAR", 17))  ;
            Buttons(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            ShowBanco(_cambios.Bancos[1]);
        }

        private void GetFromBCI()
        {
            tbInfo.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, p.GetInfo("EUR", 19), p.GetInfo("USD", 19), p.GetInfo("ZAR", 19));
            Buttons(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowBanco(_cambios.Bancos[2]);
        }

        private void GetFromBIM()
        {
            tbInfo.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, p.GetInfo("EUR", 15), p.GetInfo("USD", 15), p.GetInfo("ZAR", 15));
            Buttons(true); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowBanco(_cambios.Bancos[4]);
        }

        private void GetFromMOZA()
        {
            p.CleanToToken("preço médio de compra diário");
            tbInfo.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, p.GetInfo("EUR", 22), p.GetInfo("USD", 22), p.GetInfo("ZAR", 22));
            Buttons(true);
        }

        private void Buttons(bool b) {

            button1.Enabled = b;
            button2.Enabled = b;
            button3.Enabled = b;
            button4.Enabled = b;
            button5.Enabled =b; 
        }

        private void ShowButtons(bool b)
        {

            button1.Visible  = b;
            button2.Visible = b;
            button3.Visible = b;
            button4.Visible = b;
            button5.Visible = b;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowBanco(_cambios.Bancos[3]);
        }

        private void GetFromBMZ()
        {
            tbInfo.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, p.GetInfo("EUR", 17), p.GetInfo("USD", 17), p.GetInfo("ZAR", 17));
            Buttons(true);
        }

        private void ShowBanco(Banco B) {
            tbInfo.Text = B.Nome;

            foreach (Moeda m in B.Moedas)
            {
                tbInfo.Text += string.Format("{0}{1}\tCompra: {2}\tVeda:{3}", Environment.NewLine, m.Nome, m.Compra, m.Venda);
            }
        }
    }
}

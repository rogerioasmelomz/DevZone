﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OutCode.Net;
using System.Reflection;

namespace Testes
{
    public partial class Form1 : Form //,IResult 
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void SetResult(bool Erro, int ErrorNumber, string ErrorMessage)
        {
            if (Erro) {
                MessageBox.Show("Ocorreu um erro."); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

           // PriMotor _motor = new PriMotor(Interop.StdBE900.EnumTipoPlataforma.tpEmpresarial , "DEMO", "rogerio", "ixp8du2011","DEFAULT",this);
           // //MessageBox.Show(String.Format("Motores Iniciados Motor:{0} GCP:{0}", _motor.GetVersion(), _motor.Logistica.GetVersion()));
           //// MessageBox.Show(String.Format("Motores Iniciados Motor:{0} GCP:{0}", ProgInfo.GetName(_motor), ProgInfo.GetName(_motor.Logistica. )));
           // _motor.Logistica.ImprimeDocumento(1,"w:\\1.pdf",this);  
        }

        private void checkmail_Click(object sender, EventArgs e)
        {
            //Email eEngine = new Email();

            //eEngine.ReadMail();

            POP3 mpop = new POP3("mail.outcode.biz", "afonso.sousa@outcode.biz", "+pop-2015");

            MessageBox.Show(String.Format("Email: {0}", mpop.GetStatus()));

        }

        private void button2_Click(object sender, EventArgs e)
        {

            //string data;
            //Assembly ass= this.GetType().Assembly;

            //foreach (string str in OutCode.Interop.Resources.GetResourcesList(ass, "sql")) {

            //    data = Resources.LoadTextResourceData(ass, str);
            //    MessageBox.Show(string.Format ("R:{0}{1}{2}",str,Environment.NewLine,data  )); 
            //}

        }

        private void button3_Click(object sender, EventArgs e)
        {

            //OutCode.Codes.QREngine myQR;

            //myQR = new OutCode.Codes.QREngine();

            //myQR.RenderQrCode(txtInfo.Text);

            //picQR.Image = myQR.GetImage();
            //picQR.SizeMode = PictureBoxSizeMode.CenterImage;

            //picQR.SizeMode = PictureBoxSizeMode.StretchImage;

            //myQR.Save("teste.jpg");   

        }

        private void button4_Click(object sender, EventArgs e)
        {

            FTP ftp;

            ftp = new FTP("ftp://ftp.w15.wh-2.com", "umbacmz0_pvera", "#umbprimavera2018#");

            string[] info = ftp.directoryListSimple("./","txt");

            ftp.download (info[1],@"c:\ftp\"+info[1]);
           
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Indy.Sockets.Mail;

namespace Temp {
    public partial class frmMain : Form {
        private SMTP mSMTPClient;

        public frmMain() {
            InitializeComponent();
        }

        private void LogToScreen(string msg) {
            textBox1.Text += msg + "\r\n";
        }

        private void button1_Click(object sender, EventArgs e) {
            mSMTPClient = new SMTP("myk9space.com", "lastpush.com", LogToScreen);
        }

        private void button2_Click(object sender, EventArgs e) {
            if (mSMTPClient.Quit()) {
                mSMTPClient.Dispose();
                mSMTPClient = null;
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (mSMTPClient.QuickSend("wisesifu@lastpush.com", "wisesifu@myk9space.com", "This is a test msg.\r\n  This is a test.")) {
                mSMTPClient.Quit();
                mSMTPClient.Dispose();
                mSMTPClient = null;
            }
        }
    }
}

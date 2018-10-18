using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Indy.Sockets.Core;
using Indy.Sockets.Mail;

namespace MailClient {
    public partial class Window1 : Window {
        protected string SettingsFile;
        protected string POP3Host;
        protected string POP3User;
        protected string POP3Password;

        public Window1() {
            InitializeComponent();
            SettingsFile = System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetEntryAssembly().Location, ".txt");

            if (new System.IO.FileInfo(SettingsFile).Exists) {
                Settings xDS = new Settings();
                xDS.ReadXml(SettingsFile, System.Data.XmlReadMode.IgnoreSchema);
                Settings.POP3Row xRow = xDS.POP3[0];
                POP3Host = xRow.Host;
                POP3User = xRow.User;
                POP3Password = xRow.Password;
            }
            // If no file, creates empty shell. If exists, adds new fields
            SaveSettings();

            lboxMessages.SelectionChanged += new SelectionChangedEventHandler(lboxMessages_SelectionChanged);
        }

        void lboxMessages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var xItem = (KeyValuePair<int, string>)lboxMessages.SelectedItem;
            ShowMessage(xItem.Key);
        }

        protected void SaveSettings() {
            Settings xDS = new Settings(); 
            Settings.POP3Row xRow = xDS.POP3.NewPOP3Row();
            xRow.Host = POP3Host;
            xRow.User = POP3User;
            xRow.Password = POP3Password;
            xDS.POP3.AddPOP3Row(xRow);
            xDS.WriteXml(SettingsFile, System.Data.XmlWriteMode.IgnoreSchema);
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            button1.IsEnabled = false;
            try {
                using (var xSMTP = new SMTP("localhost", false)) {
                    var xMsg = new MessageMemory();
                    xMsg.From.Email = "chad@indyproject.org";
                    xMsg.To.Add(new EmailAddress("chad@indyproject.org"));
                    xMsg.Subject = "Test";
                    xMsg.Body.Add("Hello");
                    xSMTP.Send(xMsg);
                }
            } finally {
                button1.IsEnabled = true;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            button2.IsEnabled = false;
            try {
                using (var xPOP3 = new POP3(POP3Host, POP3User, POP3Password)) {
                    int xVoid;
                    xPOP3.GetStatus(out xVoid);
                    xPOP3.NoOp();
                    xPOP3.Delete(2);
                    xPOP3.GetSizes();
                    xPOP3.GetSize(1);
                    xPOP3.GetUIDs();
                    xPOP3.GetUID(1);

                    var xMsg = new MessageMemory();
                    xPOP3.RetrieveHeaders(1);
                    xPOP3.Retrieve(1, xMsg);
                    xPOP3.Retrieve(1, xMsg, 1);
                    
                    xPOP3.Reset();
                }
            } finally {
                button2.IsEnabled = true;
            }
        }

        protected POP3 mPOP3;

        protected void ShowMessage(int aMsgNo) {
            lboxHeaders.Items.Clear();
            var xMsg = new MessageMemory();
            mPOP3.Retrieve(aMsgNo, xMsg);

            lablSubject.Content = xMsg.Subject;
            lboxHeaders.Items.Add("From: " + xMsg.From);
            lboxHeaders.Items.Add("To:");
            foreach (var x in xMsg.To) {
                lboxHeaders.Items.Add("  " + x.Text);
            }
            lboxHeaders.Items.Add("CC:" );
            foreach (var x in xMsg.CC) {
                lboxHeaders.Items.Add("  " + x.Text);
            }
            lboxHeaders.Items.Add("BCC:");
            foreach (var x in xMsg.BCC) {
                lboxHeaders.Items.Add("  " + x.Text);
            }

            var xBody = new StringBuilder();
            foreach (string s in xMsg.Body) {
                xBody.AppendLine(s);
            }
            textBody.Text = xBody.ToString();
        }

        private void butnCheckMail_Click(object sender, RoutedEventArgs e) {
            ((Button)sender).IsEnabled = false;
            try {
                mPOP3 = new POP3(POP3Host, POP3User, POP3Password);
                var xUIDs = mPOP3.GetUIDs();
                foreach (var xUID in xUIDs) {
                    lboxMessages.Items.Add(xUID);
                }
                ShowMessage(1);
            } finally {
                ((Button)sender).IsEnabled = true;
            }
        }

        protected void ServerRun(TCPConnection aConnection) {
            aConnection.TextWriter.WriteLine("Hello");
            aConnection.TextReader.ReadLine();
        }

        private void buttonTestTCPServer_Click(object sender, RoutedEventArgs e) {
            var xServer = new TCPServer(23, ServerRun);
        }
    }
}

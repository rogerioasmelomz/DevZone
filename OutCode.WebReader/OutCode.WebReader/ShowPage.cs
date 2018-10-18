using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutCode.WebReader
{
    public partial class ShowPage : Form
    {

        private string _pageText;

        IPageParser _parser;

        Timer _timeout;

        public ShowPage()
        {
            InitializeComponent();

            _timeout = new Timer();
        }

        public void LoadPage(string PageUrl, IPageParser Parser,int TimeOut) {

            _parser = Parser;

            _timeout.Interval = 1000* TimeOut;
            _timeout.Tick += _timeout_Tick; 

            myWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DisplayText);
            myWebBrowser.AllowNavigation = true;
            myWebBrowser.Navigate(PageUrl);
            //myWebBrowser.Url = new Uri(PageUrl); 

            _timeout.Enabled = true;
            _timeout.Start(); 

        }

        private void _timeout_Tick(object sender, EventArgs e)
        {
            DisplayText(null, null);
        }

        private void DisplayText(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                WebBrowser wb = (WebBrowser)sender;
                wb.Document.ExecCommand("SelectAll", false, null);
                wb.Document.ExecCommand("Copy", false, null);
                _pageText = Clipboard.GetText();
            }
            catch (Exception)
            {
            }


            _parser.PageLoaded(_pageText);

            _timeout.Stop();
            _timeout.Enabled = false; 
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutCode.WebReader
{
    
    public class Page : IPageParser 
    {
        ShowPage _page;

        public delegate void EventPageReaded();
        public event EventPageReaded PageReaded;

        private string _pageText;
        private string _pageURL;

        public Page(string PageUrl) { _pageURL = PageUrl; }

        public void ReadPage() {

            try
            {
                LoadPageText();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void LoadPageText()
        {

            try
            {

                _page = new ShowPage();

                _page.Show();
                _page.LoadPage(_pageURL,this,10);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public string GetPageText() { return _pageText; }

        public void CleanToToken(string Key) {
            int Ini = _pageText.IndexOf(Key);
            _pageText = _pageText.Substring(Ini);
        }

        public string GetInfo(string Key, int Size) {

            try
            {
                int Ini = _pageText.IndexOf(Key);

                return (_pageText.Substring(Ini, Size).Replace("\n", "\t"));
            }
            catch (Exception)
            {
                return (string.Empty );
            }


        }

        void IPageParser.PageLoaded(string PageText)
        {
            try
            {
                _pageText = PageText;
                try
                {
                    PageReaded.Invoke();
                }
                catch (Exception)
                {
                }
                
                _page.Close();
                _page.Dispose(); 
            }
            catch (Exception)
            {
            }


        }

    }
}

using iTextSharp.text.xml.xmp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Pdf
{
    /// <summary>
    /// Classe para reter a metainformação referente a um certificado
    /// </summary>
    public class MetaData
    {
        #region Attributes

        private IDictionary<string, string> info = new Dictionary<string, string>();

        #endregion

        #region Accessors

        public IDictionary<string, string> Info
        {
            get { return info; }
            set { info = value; }
        }

        public string Author
        {
            get { return (string)info["Author"]; }
            set { info.Add("Author", value); }
        }
        public string Title
        {
            get { return (string)info["Title"]; }
            set { info.Add("Title", value); }
        }
        public string Subject
        {
            get { return (string)info["Subject"]; }
            set { info.Add("Subject", value); }
        }
        public string Keywords
        {
            get { return (string)info["Keywords"]; }
            set { info.Add("Keywords", value); }
        }
        public string Producer
        {
            get { return (string)info["Producer"]; }
            set { info.Add("Producer", value); }
        }

        public string Creator
        {
            get { return (string)info["Creator"]; }
            set { info.Add("Creator", value); }
        }

        public IDictionary<string, string> getMetaData()
        {
            return this.info;
        }

        public byte[] getStreamedMetaData()
        {
            MemoryStream os = new System.IO.MemoryStream();
            XmpWriter xmp = new XmpWriter(os, this.info);
            xmp.Close();
            return os.ToArray();
        }

        #endregion


        /// <summary>
        /// Devolve a versão da DLL
        /// </summary>
        /// <returns>string com a versão</returns>
        public static string GetVersion()
        {
            return (Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}

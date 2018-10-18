using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutCode.Codes;
using System.Drawing;  

namespace WezoCode
{
    public class Engine
    {

        QREngine _engine;

        string _IdDoc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ID"></param>
        public Engine() { }

        public void RenderQRCode(string ID) {

            _IdDoc = ID;
            _engine = new QREngine();

            _engine.RenderQrCode(string.Format("http://wezocode.outcode.biz/?IDD={0}", _IdDoc));
        }

        public void Save(string FileName) { _engine.Save(FileName);  }

        public Bitmap GetImage() { return _engine.GetImage(); }
    }
}

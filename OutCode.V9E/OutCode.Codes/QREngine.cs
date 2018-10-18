using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace OutCode.Codes
{
    public class QREngine
    {


        Bitmap _qrCode;
        QRCodeGenerator.ECCLevel _eccLevel;

        public QREngine() {

            _eccLevel = QRCodeGenerator.ECCLevel.H;
        }


        public void RenderWezoCode(string Text) {

            RenderQrCode(string.Format ("http://wezocode.outcode.biz/?IDD={0}",Text ));

        }

        public void RenderQrCode(string Text)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(Text, _eccLevel))
                    {
                        using (QRCode qrCode = new QRCode(qrCodeData))
                        {
                            _qrCode = qrCode.GetGraphic(20, Color.Black , Color.White, true);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("Erro ao gerar QRCode",err);
            }
        }

        public Bitmap GetImage() {
            return _qrCode;
        }

        public void Save(string FileName) {
            _qrCode.Save(FileName, ImageFormat.Jpeg);
        }

    }
}

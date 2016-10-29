using Interop.StdClasses900;
using Interop.StdBE900;
using stdole;

namespace KBTForms900
{
    
    public class clsKBTImages : clsAplImages
    {
        public StdPicture get_Icon(ref EnumTipoPlataforma TipoPlataforma)
        {
            //var pic = (stdole.StdPicture)Microsoft.VisualBasic.Compatibility.VB6.Support.ImageToIPicture(Properties.Resources.operacoes.ToBitmap() );

            var pic = IconConverter.ImageToIPicture(Properties.Resources.operacoes.ToBitmap());
            return (stdole.StdPicture)pic;
        }
    }
    public class IconConverter : System.Windows.Forms.AxHost
    {
        private IconConverter() : base(string.Empty)
        {
        }

        public static stdole.IPictureDisp ImageToIPicture(System.Drawing.Image image)
        {

            return (stdole.IPictureDisp)IconConverter.GetIPictureDispFromPicture(image);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ErpBS900;
using OutCode.V9E.Interfaces;
using OutCode.Interop;

namespace OutCode.V9E.BRModules
{
    public class GCP 
    {

        #region Properties

        private ErpBS _motorv9;

        #endregion

        #region Constructors

        public GCP(ErpBS  motor){
            _motorv9 = motor;
        }

        #endregion


        public void ImprimeDocumento(int Numerodoc,string PdfPath,IResult R) {
            try
            {
                _motorv9.Comercial.Vendas.ImprimeDocumento("FA", "A", Numerodoc, "000",1,null,false,PdfPath);
                R.SetResult(false, 0, "");
            }
            catch (Exception ex)
            {
                R.SetResult(true, ex.HResult, ex.ToString());
            }
        }


    }
}

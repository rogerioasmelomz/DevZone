using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interop.ErpBS900;
using Interop.StdPlatBS900;

namespace DLLDEVII
{
    public class Engine
    {

        ErpBS _bso;
        StdBSInterfPub _pso;


        public Engine() { }

        public void SetErpContext( object Bso,object Pso) {

            try
            {
                _bso = (ErpBS)Bso;
                _pso = (StdBSInterfPub)Pso;

                _pso.Dialogos.MostraAviso("Motor iniciado com sucesso!"); 
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Show() {

            AboutBox1 frmAbout;

            frmAbout = new DLLDEVII.AboutBox1();

            frmAbout.ShowDialog();

            frmAbout.Dispose(); 

        }

        public void ImprimeVenda() {

            

        }

        public string  ShowClientes() {

            string strSql = "select cliente,nome from clientes";

            return( _pso.Listas.GetF4SQL("A minha lista de cliente", strSql, "cliente"));
        }

    }
}

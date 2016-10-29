using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.StdBE900;
using Interop.StdClasses900;

namespace KBTForms900
{
    public class clsKBTServicos : clsAplServicos
    {

        private string _utilizador { get; set; }
        private string _password { get; set; }

        public void AtribuiUtilizador(ref string strUtilizador, ref string strPassword)
        {
            _utilizador = strUtilizador;
            _password = strPassword;
        }

        public void Executa(ref short intIndex)
        {
  
        }

        public void Inicializa(ref EnumTipoPlataforma enuTipoPlataforma, ref string strEmpresa, ref string strInstalacao, ref clsLicenca objLic)
        {

        }

        public void Lista(ref clsParamAplServicos objParametros, ref Array strLista, ref global::VBA.Collection colLoc)
        {
            string[] servicos;
            servicos = new string[2];

            servicos[1] = "Assistente de criação de utilizadores";

            strLista = servicos;
        }
    }
}

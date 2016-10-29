using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.StdBE900;
using Interop.StdClasses900;
using Microsoft.VisualBasic;

namespace KBTForms900
{

    public class clsKBTParametrizacoes : clsAplParametrizacoes
    {
        
        private string _utilizador { get; set; }
        private string _password { get; set; }

        public void AtribuiUtilizador(ref string strUtilizador, ref string strPassword)
        {
            _utilizador = strUtilizador;
            _password = strPassword;
        }

        public void Inicializa(ref EnumTipoPlataforma enuTipoPlataforma, ref string strEmpresa, ref string strInstalacao, ref clsLicenca objLic)
        {

        }

        public void Lista(ref clsParamAplParams objParametros, ref Array strLista, ref global::VBA.Collection colLoc)
        {
            string[] Parametros;

            Parametros = new string[2];

            Parametros[1] = "Parametros Gerais";
            strLista = Parametros;
        }

        public bool ModuloDisponivelLocalizacao(ref EnumLocalizacaoSede enuLocSede)
        {
            return (true);
        }

        public void Mostra(ref short intIndex, ref short intModoOperacao, ref object objOwnerForm)
        {
            switch (intIndex)
            {
                case 1:
                    
                    break;
            }

            Interaction.CallByName(objOwnerForm, "ActivaInterface", CallType.Method, true);
        }

        public void TiposExercicioSuportados(ref EnumTipoPlataforma enuTipoPlataforma, ref Array enuTiposExercicio) { 

        }
    }
}

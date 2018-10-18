using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primavera.MotoresPrimavera.Parametros
{
    /// <summary>
    /// Classe Instancia para a gestão das versões do primavera e as suas respectivas assemblies
    /// </summary>
    public class Instancia
    {

        public int instancia = 1;
        public string empresa;
        public string usuario;
        public string password;
        public string instanciaSql;
        public string empresaSql;
        public string usuarioSql;
        public string passwordSql;
        public static string versaoErp = "V800";
        public const string pastaConfigV800 = "PRIMAVERA\\SG800";

        public const string pastaConfigV900 = "PRIMAVERA\\SG900";
        public object daConnectionString()
        {
            return "Data Source=" + instanciaSql + ";Initial Catalog= " + empresaSql + ";User Id=" + usuarioSql + ";Password=" + passwordSql;
        }

        public static string daPastaConfig()
        {
            if (versaoErp == "V800")
            {
                return pastaConfigV800;
            }
            else
            {
                return pastaConfigV900;
            }
        }

    }
}

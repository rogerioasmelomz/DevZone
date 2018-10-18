using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primavera.Data
{
    public class PrimaveraResultStructure
    {
        public string tipoProblema;

        public int codigo;

        public string codeLevel;

        public string subNivel;

        public string descricao;

        public string procedimento;

        public string to_String()
        {
            return String.Format("[{0}] PrimaveraResultStructure: {1} ; {2} ; {3} ; {4} ; {5} ; {6} ", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), tipoProblema,
                codigo,codeLevel,subNivel,descricao,procedimento);
        }
    }
}

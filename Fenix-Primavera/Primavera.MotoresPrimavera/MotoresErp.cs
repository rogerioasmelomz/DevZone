using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.MotoresPrimavera.Comercial;
using Primavera.MotoresPrimavera.Parametros;

namespace Primavera.MotoresPrimavera
{
    public class MotoresErp
    {
         public EmpresaErp _empresaErp {get;set;}
         public MotoresComercial _comercial {get;set;}

         public MotoresErp()
         {
             _empresaErp = new EmpresaErp();
             _comercial = new MotoresComercial(_empresaErp._erpBs);

             AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
         }
               
        /// <summary>
        /// Inicializa os Motores Erp Primavera em todos as classes que invocam os motores
        /// </summary>
         public void inicializaMotores_EmpresaErp()
         {
             _comercial = new MotoresComercial(_empresaErp._erpBs);
         }

         /// <summary>
         /// Método para resolução das assemblies.
         /// </summary>
         /// <param name="sender">Application</param>
         /// <param name="args">Resolving Assembly Name</param>
         /// <returns>Assembly</returns>
         static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
         {
             string assemblyFullName;
             System.Reflection.AssemblyName assemblyName;
             string PRIMAVERA_COMMON_FILES_FOLDER = Instancia.daPastaConfig();//pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
             assemblyName = new System.Reflection.AssemblyName(args.Name);
             assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll");
             if (System.IO.File.Exists(assemblyFullName))
                 return System.Reflection.Assembly.LoadFile(assemblyFullName);
             else
                 return null;
         }
    }
}

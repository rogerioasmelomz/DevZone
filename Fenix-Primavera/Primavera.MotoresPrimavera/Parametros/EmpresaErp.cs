using Interop.ErpBS800;
using Interop.StdBE800;

using Primavera.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Primavera.MotoresPrimavera.Parametros
{
    public class EmpresaErp
    {

        public ErpBS _erpBs;
        private int tipoPlataforma { get; set; }

        private string codUsuario { get; set; }

        private string codEmpresa { get; set; }

        private string password { get; set; }

        public EmpresaErp()
        {
            // Resolução das Assemblies
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
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
        
        /// <summary>
        /// Metodo para inicializar o motor do primavera
        /// </summary>
        /// <param name="tipoPlataforma"> 0 - Executiva, 1- Profissional</param>
        /// <param name="codEmpresa"></param>
        /// <param name="codUsuario"></param>
        /// <param name="password"></param>
        /// <remarks></remarks>
        public PrimaveraResultStructure AbreEmpresaPrimavera(int tipoPlataforma, string codEmpresa, string codUsuario, string password)
        {
            PrimaveraResultStructure result = new PrimaveraResultStructure();

            try
            {
                this.tipoPlataforma = tipoPlataforma;
                this.codUsuario = codUsuario;
                this.codEmpresa = codEmpresa;
                this.password = password;

                if (_erpBs == null)
                {
                    _erpBs = new ErpBS();
                }
                else
                {
                    _erpBs.FechaEmpresaTrabalho();
                }
                
                _erpBs.AbreEmpresaTrabalho( tipoPlataforma== 0? EnumTipoPlataforma.tpEmpresarial:EnumTipoPlataforma.tpProfissional, 
                    codEmpresa, codUsuario, password,null,"DEFAULT",true);

                result.codigo = 0;
                result.descricao = string.Format("Empresa {0} - {1} Aberta Com Sucesso", _erpBs.Contexto.CodEmp,_erpBs.Contexto.IDNome);
                Console.WriteLine(String.Format("[{0}] Empresa {1} - {2} Aberta Com Sucesso",DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), _erpBs.Contexto.CodEmp, _erpBs.Contexto.IDNome));
                
                return result;
            }
            catch (Exception ex)
            {
                result.codigo = 3;
                result.descricao = ex.Message;
                Console.WriteLine(String.Format("[{0}] Erro a abrir a Empresa {1} - {2} devido a: {3}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), _erpBs.Contexto.CodEmp, _erpBs.Contexto.IDNome, ex.Message));
                
                return result;
            }

        }

        /// <summary>
        /// Valida se empresa primavera se encontra aberta
        /// </summary>
        /// <returns></returns>
        public bool EmpresaPrimaveraAberta()
        {
            if (_erpBs == null)
            {
                return false;
            }
            else
            {
                return _erpBs.Contexto.EmpresaAberta;
            }
        }

        /// <summary>
        /// Inicialização das Transações no Erp
        /// </summary>
        public void IniciaTransacao()
        {
            _erpBs.IniciaTransaccao();
            Console.WriteLine(String.Format("[{0}] Inicia a Transação da Empresa {1} - {2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), _erpBs.Contexto.CodEmp, _erpBs.Contexto.IDNome));
                
        }

        /// <summary>
        /// Finialização das Transações no Erp
        /// </summary>
        public void TerminaTransacao()
        {
            _erpBs.TerminaTransaccao();
            Console.WriteLine(String.Format("[{0}] Termina a Transação da Empresa {1} - {2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), _erpBs.Contexto.CodEmp, _erpBs.Contexto.IDNome));
             
        }
        
        /// <summary>
        /// Roolback das transações
        /// </summary>
        public void DesfazTransacao()
        {
            try
            {
                _erpBs.DesfazTransaccao();
                
            }
            catch { }
           
            
        }
        
        /// <summary>
        /// Executa Query na empresa primavera
        /// </summary>
        /// <param name="str_query"></param>
        public string Executa_Query_Insert_Update(string str_query)
        {
            try
            {
                object a;
                _erpBs.DSO.BDAPL.Execute(str_query, out a, -1);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message ;
            }
        }
                
    }
}

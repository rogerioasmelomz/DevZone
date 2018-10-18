using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MIT.MotoresErp;


namespace MIT.WCF_Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MIT_MotoresErp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MIT_MotoresErp.svc or MIT_MotoresErp.svc.cs at the Solution Explorer and start debugging.
    public class MIT_MotoresErp : IMIT_MotoresErp
    {
        private MotoresErpV900 motorErp { get; set; }
        PrimaveraResultStructure result = new PrimaveraResultStructure();
        
        public PrimaveraResultStructure AbreEmpresaPrimavera(int tipoPlataforma, string codEmpresa, string codUsuario, string password)
        {           

            try
            {
                motorErp = new MotoresErpV900();

                var pResult = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password);
                
                result = new PrimaveraResultStructure()
                {
                    codigo = pResult.codigo,
                    descricao = pResult.descricao

                };
                            
                return result;

            }
            catch (Exception ex)
            {
                result.codigo = 3;
                result.descricao = ex.Message;
                
                return result;
            }           

        }

        public List<Moeda> ListaMoedas(int tipoPlataforma, string codEmpresa, string codUsuario, string password)
        {
            List<Moeda> lista = new List<Moeda>();

            try
            {
                motorErp = new MotoresErpV900();

                var pResult = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password);

                result = new PrimaveraResultStructure()
                {
                    codigo = pResult.codigo,
                    descricao = pResult.descricao

                };

                var _lista = motorErp._empresaErp.listaMoedas();

                foreach (var item in _lista)
                {
                    lista.Add(new Moeda() { moeda = item.moedaId, descricao = item.descricao });
                }
            }
            catch (Exception ex)
            {
                result.codigo = 3;
                result.descricao = ex.Message;

                
            }    
            return lista;
        }

        public List<Empresa> Lista_Empresas(int tipoPlataforma, string codUsuario, string password, ref string categoria)
        {
            List<Empresa> lista = new List<Empresa>();

            try
            {
                motorErp = new MotoresErpV900();

                //var pResult = motorErp.iniciliazarAdministrador(codUsuario, password, "DEFAULT", "Professional");


                string _tipoPlataforma="";
                switch (tipoPlataforma)
                {
                    case 0: _tipoPlataforma = "Executive"; break;
                    case 1: _tipoPlataforma = "Professional"; break;
                }
                motorErp.iniciliazarAdministrador(codUsuario, password, "DEFAULT",_tipoPlataforma);

                
                var _lista = motorErp._administrador.lista_empresas();

                foreach (var item in _lista)
                {
                    lista.Add(new Empresa() { codigo = item.codigo,  nome = item.nome });
                }
            }
            catch (Exception ex)
            {
                result.codigo = 3;
                result.descricao = ex.Message;


            }
            return lista;
        }

        public PrimaveraResultStructure Pendentes_Gravar_Diferencas_Cambio(int tipoPlataforma, string codEmpresa, string codUsuario, string password,string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data)
        {
            try
            {
                motorErp = new MotoresErpV900();
                motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password);

                if (motorErp._empresaErp.EmpresaPrimaveraAberta())
                {
                    motorErp.Gravar_Diferencas_Cambio(docDiferencaPositivos, docDiferencasNegativos, valor, entidade, tipoEntidade, moeda, data);
                    result.codigo = 0;
                    result.descricao = "Empresa se econtra aberta";
                }
                else
                {
                    result.codigo = 1;
                    result.descricao = "Empresa não se econtra aberta";

                }
                
            }
            catch (Exception ex)
            {
                result.codigo = 2;
                result.descricao = ex.Message;
            }
               
            return result;
        }
    }
}

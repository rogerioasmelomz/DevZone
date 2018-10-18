using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Primavera.Operacoes_Erp_Fenix;

namespace Primavera.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Primavera_Accsys" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Primavera_Accsys.svc or Primavera_Accsys.svc.cs at the Solution Explorer and start debugging.
    public class Primavera_Accsys : IPrimavera_Accsys
    {
        private short tipoPlataforma;

        private String codEmpresa, codUsuario, password;
        private GestaoAlunos gestaoAlunos = new GestaoAlunos();

        #region Web Services

        public PrimaveraResultStructure Inscricao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2,string nuit, bool bolsa, string emailPessoal, string emailAlternativo,
            string tipoIngresso, string observacao,string vendedor)
        {

            PrimaveraResultStructure result = new PrimaveraResultStructure() { codigo = -1 };


            try
            {
                result = abrirConfiguracoes();

                var resultado = gestaoAlunos.GerarInscricaoIsutc(tipoPlataforma, codEmpresa, codUsuario, password,
                    nrEstudante, nomeCompleto, morada, bairro, nrTelefone, nrTelefone2,nuit, bolsa,
                          emailPessoal, emailAlternativo, tipoIngresso, observacao,vendedor);

                result = new PrimaveraResultStructure()
                {
                    tipoProblema = resultado.tipoProblema,
                    codigo = resultado.codigo,
                    codeLevel = resultado.codeLevel,
                    descricao = resultado.descricao
                };


                return result;
            }
            catch (Exception ex)
            {
                result = new PrimaveraResultStructure()
                {
                    tipoProblema = "Erro de Logica no Conector Primavera",
                    codigo = 2,
                    codeLevel = "21",
                    descricao = "213 - Erro ao conectar com o DLL " + ex.Message
                };
                return result;
            }

        }

        public PrimaveraResultStructure Matricula(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2,string nuit, string bolsa, string curso, int ano, int semestre, string emailIsutc,
            string emailPessoal, string turma, string codigoArtigoMatricula, string codigoArtigoPropina, string vendedor)
        {

            PrimaveraResultStructure result = new PrimaveraResultStructure() { codigo = -1 };

            try
            {
                result = abrirConfiguracoes();

                var resultado = gestaoAlunos.GerarMatriculaIsutc(tipoPlataforma, codEmpresa, codUsuario, password,
                    nrEstudante, nomeCompleto, morada, bairro,
                    nrTelefone, nrTelefone2, bolsa, curso, ano, semestre, emailIsutc,
                    emailPessoal, turma, codigoArtigoMatricula, codigoArtigoPropina, nuit);

                result = new PrimaveraResultStructure()
                {
                    tipoProblema = resultado.tipoProblema,
                    codigo = resultado.codigo,
                    codeLevel = resultado.codeLevel,
                    descricao = resultado.descricao
                };


                return result;
            }
            catch (Exception ex)
            {
                result = new PrimaveraResultStructure()
                {
                    tipoProblema = "Erro de Logica no Conector Primavera",
                    codigo = 2,
                    codeLevel = "21",
                    descricao = "213 - Erro ao conectar com o DLL " + ex.Message
                };
                return result;
            }

        }

        public PrimaveraResultStructure Renovacao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string bolsa, string curso, int ano, int semestre, string emailIsutc,
            string emailPessoal, string turma, int numeroDisciplinas, string codigoArtigoRenovacao, string codigoArtigoPropina, 
            string nuit, string vendedor)
        {

            PrimaveraResultStructure result = new PrimaveraResultStructure() { codigo = -1 };


            try
            {
                result = abrirConfiguracoes();

                var resultado = gestaoAlunos.GerarRenovacaoIsutc(tipoPlataforma, codEmpresa, codUsuario, password,
                    nrEstudante, nomeCompleto, morada, bairro, nrTelefone, nrTelefone2, bolsa, curso, ano, semestre, emailIsutc,
                    emailPessoal, turma, numeroDisciplinas, codigoArtigoRenovacao, codigoArtigoPropina, nuit);

                result = new PrimaveraResultStructure()
                {
                    tipoProblema = resultado.tipoProblema,
                    codigo = resultado.codigo,
                    codeLevel = resultado.codeLevel,
                    descricao = resultado.descricao
                };


                return result;
            }
            catch (Exception ex)
            {
                result = new PrimaveraResultStructure()
                {
                    tipoProblema = "Erro de Logica no Conector Primavera",
                    codigo = 2,
                    codeLevel = "21",
                    descricao = "213 - Erro ao conectar com o DLL " + ex.Message
                };
                return result;
            }

        }
        #endregion

        #region Configuracao do Conector com Primavera


        private XmlTextReader reader;

        private PrimaveraResultStructure abrirConfiguracoes()
        {
            PrimaveraResultStructure result = new PrimaveraResultStructure() { codigo = -1 };

            reader = new XmlTextReader(System.AppDomain.CurrentDomain.BaseDirectory + "resources\\Configuracao.xml");
            try
            {

                if ((System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "resources\\Configuracao.xml")))
                {
                    XmlReader document = new XmlTextReader(System.AppDomain.CurrentDomain.BaseDirectory + "resources\\Configuracao.xml");


                    while ((document.Read()))
                    {
                        dynamic type = document.NodeType;



                        if ((type == XmlNodeType.Element))
                        {

                            switch (document.Name)
                            {
                                case "tipoPlataforma":
                                    tipoPlataforma = short.Parse(document.ReadInnerXml());
                                    break;
                                case "empresa":
                                    codEmpresa = document.ReadInnerXml();
                                    break;
                                case "usuario":
                                    codUsuario = document.ReadInnerXml();
                                    break;
                                case "password":
                                    password = document.ReadInnerXml();
                                    break;
                            }

                        }


                    }


                    result = new PrimaveraResultStructure();
                    result.tipoProblema = "Erro do Sistema no Primavera";
                    result.codigo = 0;
                    result.codeLevel = "00 - Sucesso Completo";
                    result.descricao = "000 - Sucesso ao conectar com xml";

                }
                else
                {
                    result = new PrimaveraResultStructure();
                    result.tipoProblema = "Erro do Sistema no Primavera";
                    result.codigo = 2;
                    result.codeLevel = "21 - Erros com ficheiros DLL";
                    result.descricao = "210 - Ficheiro de Configuracao.xml não existe";
                }
            }
            catch (Exception ex)
            {
                result = new PrimaveraResultStructure();
                result.tipoProblema = "Erro do Sistema no Primavera";
                result.codigo = 2;
                result.codeLevel = "21 - Erros com ficheiros DLL";
                result.descricao = "210 - Erro no metodo Abrir as Configuracoes \n" + ex.Message;
            }

            return result;
        }
        #endregion
    }
}

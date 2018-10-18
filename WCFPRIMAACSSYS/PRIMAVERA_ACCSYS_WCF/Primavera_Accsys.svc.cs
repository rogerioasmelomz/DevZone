using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using CSU_DLL;


namespace PRIMAVERA_ACCSYS_WCF
{
    public class Primavera_Accsys : IPrimavera_Accsys
    {

        private short tipoPlataforma;

        private String codEmpresa, codUsuario, password;
        private CSU_DLL.Aluno_Controller aluno_control = new CSU_DLL.Aluno_Controller();

        #region Web Services
        public PrimaveraResultStructure Inscricao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string bolsa,  string emailPessoal, string emailAlternativo,
            string tipoIngresso, string status, string observacao)
        {
            
            PrimaveraResultStructure result = new PrimaveraResultStructure() { codigo = -1 };


            try
            {
                result = abrirConfiguracoes();

                var resultado = aluno_control.GerarInscricaoIsutc(tipoPlataforma, codEmpresa, codUsuario, password,
                    nrEstudante, nomeCompleto, morada, bairro, nrTelefone, nrTelefone2, bolsa, 
                          emailPessoal, emailAlternativo, tipoIngresso, status, observacao);

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

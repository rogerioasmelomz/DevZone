using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Primavera.WebServices
{
    [ServiceContract]
    [ServiceKnownType(typeof(PrimaveraResultStructure))]
    //[ServiceContract (Name= "Fenix-Primavera")]
    public interface IPrimavera_Accsys
    {
        [OperationContract(Name = "Operacao_1_Inscricao")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "Inscricao/{nrEstudante},{nomeCompleto}, {morada},{bairro},{nrTelefone},{nrTelefone2},{nuit},{bolsa}"
                + ",{curso},{emailPessoal},{emailAlternativo},{turma},{tipoIngresso},{observacao}")]
        PrimaveraResultStructure Inscricao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2,string nuit, bool bolsa, string emailPessoal, string emailAlternativo,
            string tipoIngresso, string observacao, string vendedor);

        [OperationContract(Name = "Operacao_2_Matricula")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "Matricula/{nrEstudante},{nomeCompleto}, {morada},{bairro},{nrTelefone},{nrTelefone2},{nuit},{bolsa}"
                + ",{curso},{emailPessoal},{emailAlternativo},{turma},{tipoIngresso},{status},{observacao}")]
        PrimaveraResultStructure Matricula(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string nuit,string bolsa,string curso, int ano, int semestre,  string emailIsutc,
            string emailPessoal, string turma, string codigoArtigoMatricula, string codigoArtigoPropina, string vendedor);

        [OperationContract(Name = "Operacao_3_Renovacao")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "Renovacao/{nrEstudante},{nomeCompleto}, {morada},{bairro},{nrTelefone},{nrTelefone2},{bolsa}"
                + ",{curso},{emailPessoal},{emailAlternativo},{turma},{tipoIngresso},{status},{observacao}, {nuit}")]
        PrimaveraResultStructure Renovacao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string bolsa, string curso, int ano, int semestre, string emailIsutc,
            string emailPessoal, string turma, int numeroDisciplinas, string codigoArtigoRenovacao, string codigoArtigoPropina, string nuit, string vendedor);
               
    }

    [DataContract]
    public class PrimaveraResultStructure
    {
        [DataMember(Name = "tipoProblema")]
        public string tipoProblema { get; set; }

        [DataMember(Name = "codigo")]
        public int codigo { get; set; }

        [DataMember(Name = "codeLevel")]
        public string codeLevel { get; set; }

        [DataMember(Name = "subNivel")]
        public string subNivel { get; set; }

        [DataMember(Name = "descricao")]
        public string descricao { get; set; }

        [DataMember(Name = "procedimento")]
        public string procedimento { get; set; }

    }
}



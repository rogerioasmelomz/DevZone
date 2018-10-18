using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PRIMAVERA_ACCSYS_WCF
{
    [ServiceContract]
    [ServiceKnownType(typeof(PrimaveraResultStructure))]
    //[ServiceContract (Name= "Fenix-Primavera")]
    public interface IPrimavera_Accsys
    {
        [OperationContract(Name = "Operacao_1_Inscricao")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "Inscricao/{nrEstudante},{nomeCompleto}, {morada},{bairro},{nrTelefone},{nrTelefone2},{bolsa}"
                + ",{curso},{emailPessoal},{emailAlternativo},{turma},{tipoIngresso},{status},{observacao}")]
        PrimaveraResultStructure Inscricao(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string bolsa, string emailPessoal, string emailAlternativo,
            string tipoIngresso, string status, string observacao);
               
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

        [DataMember(Name = "ficheiro")]
        public Stream ficheiro { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Primavera.WebServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPrimavera_Service
    {
        [OperationContract(Name = "CriarAluno")]
        PrimaveraResultStructure CriarAluno(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string cliente,
            string nome, string nomeFiscal, float desconto, string facMorada, string facLocalidade, string facNrTelefone, string nuit, string vendedor, string observacao,
            bool cdu_bolsa, bool cdu_geraMulta, string cdu_tipoIngresso, string cdu_codLic, string cdu_turma, Contactos contacto);

        [OperationContract(Name = "ActualizarAluno")]
        PrimaveraResultStructure ActualizarAluno( int tipoPlataforma, string codEmpresa,string codUtilizador, string password, string cliente, string novoNrCliente,
            string nome, string nomeFiscal, float desconto, string facMorada, string facLocalidade, string facNrTelefone, string nuit, string vendedor, string observacao,
            bool cdu_bolsa,bool cdu_geraMulta, string cdu_tipoIngresso,string cdu_codLic,string cdu_turma, Contactos contacto);

        [OperationContract(Name = "CriarFactura")]
        PrimaveraResultStructure GravarFactura(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string nomeUtilizador,string cliente,
            string tipoDoc, Artigo[] listaArtigos);

        [OperationContract(Name = "CriaOuActualizarContrato")]
        PrimaveraResultStructure CriaOuActualizarContrato(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string nomeUtilizador, string cliente,
            string tipoDoc, string semestreId,Artigo[] listaArtigos);

        [OperationContract(Name = "ConsultaConta")]
        PrimaveraResultStructure ConsultaConta(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string cliente);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "Primavera.WebServices.Services.ContractType".
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

        [DataMember(Name = "pedentes")]
        public Pendente[] pendentes { get; set; }
    }

    [DataContract]
    public class Contactos
    {
        [DataMember(Name = "codigo")]
        public string codigo { get; set; }

        [DataMember(Name = "primeiroNome")]
        public string primeiroNome { get; set; }

        [DataMember(Name = "ultimoNome")]
        public string ultimoNome { get; set; }

        [DataMember(Name = "nomeCompleto")]
        public string nomeCompleto { get; set; }

        [DataMember(Name = "tipoContacto")]
        public string tipoContacto { get; set; }

        [DataMember(Name = "emailPrincipal")]
        public string emailPrincipal { get; set; }

        [DataMember(Name = "emailAlternativo")]
        public string emailAlternativo { get; set; }

        [DataMember(Name = "nrTelefone")]
        public string nrTelefone { get; set; }

        [DataMember(Name = "nrTelefoneAlternativo")]
        public string nrTelefoneAlternativo { get; set; }

        [DataMember(Name = "morada")]
        public string morada { get; set; }

        [DataMember(Name = "localidade")]
        public string localidade { get; set; }
    }

    [DataContract]
    public class Artigo
    {
        [DataMember(Name = "artigo")]
        public string artigo { get; set; }

        [DataMember(Name = "quantidade")]
        public double quantidade { get; set; }

        [DataMember(Name = "desconto")]
        public double desconto { get; set; }

    }

    [DataContract]
    public class Pendente
    {
        [DataMember(Name = "cliente")]
        public string cliente { get; set; }

        [DataMember(Name = "tipoDocumento")]
        public string tipoDoc { get; set; }

        [DataMember(Name = "SerieDocumento")]
        public string serie { get; set; }

        [DataMember(Name = "NumeroDocumento")]
        public long numDoc { get; set; }

        [DataMember(Name = "ValorTotal")]
        public double valorTotal { get; set; }

        [DataMember(Name = "valorPendente")]
        public double valorPendente { get; set; }

        [DataMember(Name = "Moeda")]
        public string moeda { get; set; }

        [DataMember(Name = "DataCriacao")]
        public DateTime dataCriacao { get; set; }

        [DataMember(Name = "DataVencimento")]
        public DateTime dataVencimento { get; set; }
        
    }
}

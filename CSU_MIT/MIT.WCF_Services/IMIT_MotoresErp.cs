using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MIT.WCF_Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMIT_MotoresErp" in both code and config file together.
    [ServiceContract]
    public interface IMIT_MotoresErp
    {
        
        [OperationContract(Name = "AbreEmpresaPrimavera")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "AbreEmpresaPrimavera/{tipoPlataforma},{codEmpresa}, {codUsuario},{password}")]
        PrimaveraResultStructure AbreEmpresaPrimavera( int tipoPlataforma, string codEmpresa, string codUsuario, string password);

        [OperationContract(Name = "Empresa_Moedas")]
        List<Moeda> ListaMoedas(int tipoPlataforma, string codEmpresa, string codUsuario, string password);

        [OperationContract(Name = "Empresa_Lista")]
        List<Empresa> Lista_Empresas(int tipoPlataforma, string codUsuario, string password, ref string categoria);

        [OperationContract(Name = "Pendentes_Gravar_Diferencas_Cambio")]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare,
        //        UriTemplate = "AbreEmpresaPrimavera/{tipoPlataforma},{codEmpresa}, {codUsuario},{password}")]
        PrimaveraResultStructure Pendentes_Gravar_Diferencas_Cambio(int tipoPlataforma, string codEmpresa, string codUsuario, string password,string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data);
        
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

    [DataContract]
    public class Moeda
    {
        [DataMember(Name = "moeda")]
        public string moeda { get; set; }

        [DataMember(Name = "descricao")]
        public string descricao { get; set; }
    }

    [DataContract]
    public class Empresa
    {
        [DataMember (Name="Codigo")]
        public string codigo { get; set; }
        
        [DataMember(Name="nome")]
        public string nome { get; set; }

        [DataMember(Name="nuit")]
        public string nuit { get; set; }

    }
}

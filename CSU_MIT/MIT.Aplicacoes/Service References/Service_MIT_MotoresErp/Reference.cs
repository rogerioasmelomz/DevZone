﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MIT.Aplicacoes.Service_MIT_MotoresErp {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Empresa", Namespace="http://schemas.datacontract.org/2004/07/MIT.WCF_Services")]
    [System.SerializableAttribute()]
    public partial class Empresa : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodigoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string nomeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string nuitField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Codigo {
            get {
                return this.CodigoField;
            }
            set {
                if ((object.ReferenceEquals(this.CodigoField, value) != true)) {
                    this.CodigoField = value;
                    this.RaisePropertyChanged("Codigo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string nome {
            get {
                return this.nomeField;
            }
            set {
                if ((object.ReferenceEquals(this.nomeField, value) != true)) {
                    this.nomeField = value;
                    this.RaisePropertyChanged("nome");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string nuit {
            get {
                return this.nuitField;
            }
            set {
                if ((object.ReferenceEquals(this.nuitField, value) != true)) {
                    this.nuitField = value;
                    this.RaisePropertyChanged("nuit");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Service_MIT_MotoresErp.IMIT_MotoresErp")]
    public interface IMIT_MotoresErp {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/AbreEmpresaPrimavera", ReplyAction="http://tempuri.org/IMIT_MotoresErp/AbreEmpresaPrimaveraResponse")]
        MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure AbreEmpresaPrimavera(int tipoPlataforma, string codEmpresa, string codUsuario, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/AbreEmpresaPrimavera", ReplyAction="http://tempuri.org/IMIT_MotoresErp/AbreEmpresaPrimaveraResponse")]
        System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure> AbreEmpresaPrimaveraAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Empresa_Moedas", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Empresa_MoedasResponse")]
        MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.Moeda[] Empresa_Moedas(int tipoPlataforma, string codEmpresa, string codUsuario, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Empresa_Moedas", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Empresa_MoedasResponse")]
        System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.Moeda[]> Empresa_MoedasAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Empresa_Lista", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Empresa_ListaResponse")]
        MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaResponse Empresa_Lista(MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Empresa_Lista", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Empresa_ListaResponse")]
        System.Threading.Tasks.Task<MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaResponse> Empresa_ListaAsync(MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Pendentes_Gravar_Diferencas_Cambio", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Pendentes_Gravar_Diferencas_CambioResponse")]
        MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure Pendentes_Gravar_Diferencas_Cambio(int tipoPlataforma, string codEmpresa, string codUsuario, string password, string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMIT_MotoresErp/Pendentes_Gravar_Diferencas_Cambio", ReplyAction="http://tempuri.org/IMIT_MotoresErp/Pendentes_Gravar_Diferencas_CambioResponse")]
        System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure> Pendentes_Gravar_Diferencas_CambioAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password, string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Empresa_Lista", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class Empresa_ListaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int tipoPlataforma;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string codUsuario;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string password;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=3)]
        public string categoria;
        
        public Empresa_ListaRequest() {
        }
        
        public Empresa_ListaRequest(int tipoPlataforma, string codUsuario, string password, string categoria) {
            this.tipoPlataforma = tipoPlataforma;
            this.codUsuario = codUsuario;
            this.password = password;
            this.categoria = categoria;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Empresa_ListaResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class Empresa_ListaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa[] Empresa_ListaResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string categoria;
        
        public Empresa_ListaResponse() {
        }
        
        public Empresa_ListaResponse(MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa[] Empresa_ListaResult, string categoria) {
            this.Empresa_ListaResult = Empresa_ListaResult;
            this.categoria = categoria;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMIT_MotoresErpChannel : MIT.Aplicacoes.Service_MIT_MotoresErp.IMIT_MotoresErp, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MIT_MotoresErpClient : System.ServiceModel.ClientBase<MIT.Aplicacoes.Service_MIT_MotoresErp.IMIT_MotoresErp>, MIT.Aplicacoes.Service_MIT_MotoresErp.IMIT_MotoresErp {
        
        public MIT_MotoresErpClient() {
        }
        
        public MIT_MotoresErpClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MIT_MotoresErpClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MIT_MotoresErpClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MIT_MotoresErpClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure AbreEmpresaPrimavera(int tipoPlataforma, string codEmpresa, string codUsuario, string password) {
            return base.Channel.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password);
        }
        
        public System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure> AbreEmpresaPrimaveraAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password) {
            return base.Channel.AbreEmpresaPrimaveraAsync(tipoPlataforma, codEmpresa, codUsuario, password);
        }
        
        public MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.Moeda[] Empresa_Moedas(int tipoPlataforma, string codEmpresa, string codUsuario, string password) {
            return base.Channel.Empresa_Moedas(tipoPlataforma, codEmpresa, codUsuario, password);
        }
        
        public System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.Moeda[]> Empresa_MoedasAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password) {
            return base.Channel.Empresa_MoedasAsync(tipoPlataforma, codEmpresa, codUsuario, password);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaResponse MIT.Aplicacoes.Service_MIT_MotoresErp.IMIT_MotoresErp.Empresa_Lista(MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest request) {
            return base.Channel.Empresa_Lista(request);
        }
        
        public MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa[] Empresa_Lista(int tipoPlataforma, string codUsuario, string password, ref string categoria) {
            MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest inValue = new MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest();
            inValue.tipoPlataforma = tipoPlataforma;
            inValue.codUsuario = codUsuario;
            inValue.password = password;
            inValue.categoria = categoria;
            MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaResponse retVal = ((MIT.Aplicacoes.Service_MIT_MotoresErp.IMIT_MotoresErp)(this)).Empresa_Lista(inValue);
            categoria = retVal.categoria;
            return retVal.Empresa_ListaResult;
        }
        
        public System.Threading.Tasks.Task<MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaResponse> Empresa_ListaAsync(MIT.Aplicacoes.Service_MIT_MotoresErp.Empresa_ListaRequest request) {
            return base.Channel.Empresa_ListaAsync(request);
        }
        
        public MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure Pendentes_Gravar_Diferencas_Cambio(int tipoPlataforma, string codEmpresa, string codUsuario, string password, string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data) {
            return base.Channel.Pendentes_Gravar_Diferencas_Cambio(tipoPlataforma, codEmpresa, codUsuario, password, docDiferencaPositivos, docDiferencasNegativos, valor, entidade, tipoEntidade, moeda, data);
        }
        
        public System.Threading.Tasks.Task<MIT.Aplicacoes.CCT_Processamento_Diferencas_Cambio.Service_MIT_MotoresErp.PrimaveraResultStructure> Pendentes_Gravar_Diferencas_CambioAsync(int tipoPlataforma, string codEmpresa, string codUsuario, string password, string docDiferencaPositivos, string docDiferencasNegativos, double valor, string entidade, string tipoEntidade, string moeda, System.DateTime data) {
            return base.Channel.Pendentes_Gravar_Diferencas_CambioAsync(tipoPlataforma, codEmpresa, codUsuario, password, docDiferencaPositivos, docDiferencasNegativos, valor, entidade, tipoEntidade, moeda, data);
        }
    }
}

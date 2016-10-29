using Interop.StdClasses900;

namespace KBTAudit900
{
    public class clsKBTOperacoesAplicacao : clsAplAudit
    {
        public clsArvoreOperacoes get_ArvOperacoes(ref clsParamOpsAplicacao objParametros)
        {
            clsArvoreOperacoes objOps = new clsArvoreOperacoes();
            clsOperacaoApl objOp;

            objOp = objOps.Add("mnuOperacao", "Operações", 0, "");
            objOp = objOps.Add("mnuOperacao1", "Operação 1", 0, "mnuOperacao");

            return objOps;
        }

        public clsPermissoesVar get_PermissoesDinamicas(ref clsParamOpsAplicacao objParametros)
        {
            clsPermissoesVar objVars = new clsPermissoesVar();
            clsPermissaoVar objVar;

            objVar = objVars.Add("Documento", "Documento", "FA", "Fatura da Sorte.", objParametros.get_Empresa());
            objVar.OperacoesPossiveis.Add("CRIAR", "Criar");
            objVar.OperacoesPossiveis.Add("MODIFICAR", "Modificar");
            objVar.OperacoesPossiveis.Add("ANULAR", "Anular");
            objVar.OperacoesPossiveis.Add("VISUALIZAR", "Visualizar");

            return objVars;
        }
    }
}

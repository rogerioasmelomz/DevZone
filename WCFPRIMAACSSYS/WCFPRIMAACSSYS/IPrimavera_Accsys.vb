Imports System.ServiceModel

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IPrimavera_Accsys" in both code and config file together.
<ServiceContract()>
Public Interface IPrimavera_Accsys

    ''' <summary>
    ''' Esta operação será responsável por inscrever um aluno através do método inscreverAluno(inputs listados); 
    '''  Será necessário o preenchimento dos campos obrigatórios;
    '''  Será necessário a validação dos campos, de forma a evitar que o utilizador insira dados errados;
    '''  Depois do preenchimento dos dados, deverá existir um botão Gravar;
    '''  A informação deverá se encontrar na base de dados do Fenix, a espera de serem válidados;
    '''  Só depois de serem válidados pela SA, esta informação será carregada no Primavera através de um botão Validar.
    ''' NOTA1: O campo “bolsa”, será do tipo boolean, essa informação depois será manipulada manualmente no Primavera;
    ''' NOTA2: O campo “status”, é que vai definir se ocorreu um erro ou não em relação a comunicação Fenix-Primavera. Onde só depois do Primavera confirmar que gravou o estudante, este campo irá retornar um “ok”, caso não permanecerá com “erro”.
    ''' </summary>
    ''' <param name="nrEstudante"></param>
    ''' <param name="nomeCompleto"></param>
    ''' <param name="nrTalao"></param>
    ''' <param name="morada"></param>
    ''' <param name="bairro"></param>
    ''' <param name="nrTelefone"></param>
    ''' <param name="nrTelefone2"></param>
    ''' <param name="bosla"></param>
    ''' <param name="curso"></param>
    ''' <param name="emailIsutc"></param>
    ''' <param name="emailAlternativo"></param>
    ''' <param name="turma"></param>
    ''' <param name="tipoIngresso"></param>
    ''' <param name="status"></param>
    ''' <param name="observacao"></param>
    ''' <param name="dataRegistro"></param>
    ''' <returns>PrimaveraResultStructure.statusCode</returns>
    ''' <remarks> 
    ''' </remarks>
    <OperationContract(Name = "Inscricao")>
        <WebInvoke(Method = "GET", )>
    Function Inscricao(nrEstudante As String, nomeCompleto As String, ByRef nrTalao As Long, morada As String, bairro As String, _
                       nrTelefone As String, ByRef nrTelefone2 As String, bosla As Boolean, curso As String, emailIsutc As String, _
                       ByRef emailAlternativo As String, turma As String, tipoIngresso As String, ByRef status As String, _
                       ByRef observacao As String, dataRegistro As Date) As String

End Interface

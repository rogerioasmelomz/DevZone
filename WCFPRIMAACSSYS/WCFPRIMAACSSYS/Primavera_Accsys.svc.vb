' NOTE: You can use the "Rename" command on the context menu to change the class name "Primavera_Accsys" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select Primavera_Accsys.svc or Primavera_Accsys.svc.vb at the Solution Explorer and start debugging.
Public Class Primavera_Accsys
    Implements IPrimavera_Accsys

    Public Function Inscricao(nrEstudante As String, nomeCompleto As String, ByRef nrTalao As Long, morada As String, bairro As String, _
                       nrTelefone As String, ByRef nrTelefone2 As String, bosla As Boolean, curso As String, emailIsutc As String, _
                       ByRef emailAlternativo As String, turma As String, tipoIngresso As String, ByRef status As String, _
                       ByRef observacao As String, dataRegistro As Date) As String Implements IPrimavera_Accsys.Inscricao
        Return ""
    End Function

End Class

Public Class Instancia
    Public instancia As Integer = 1

    Public empresa As String
    Public usuario As String
    Public password As String
    Public instanciaSql As String
    Public empresaSql As String
    Public usuarioSql As String
    Public passwordSql As String
    Public Shared versaoErp As String = "V800"
    Public Const pastaConfigV800 As String = "PRIMAVERA\\SG800"
    Public Const pastaConfigV900 As String = "PRIMAVERA\\SG900"

    Public Function daConnectionString()
        Return "Data Source=" + instanciaSql + ";Initial Catalog= " + empresaSql + ";User Id=" + usuarioSql + ";Password=" + passwordSql
    End Function

    Public Shared Function daPastaConfig()
        If versaoErp = "V800" Then
            Return pastaConfigV800
        Else
            Return pastaConfigV900
        End If
    End Function

End Class

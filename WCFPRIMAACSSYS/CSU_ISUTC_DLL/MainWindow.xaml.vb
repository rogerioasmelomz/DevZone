Class MainWindow 

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'Dim a As New Aluno_Controller
        'a.abrirConfiguracoes()

        Dim objDll As Object
        objDll = CreateObject("CSU_TRANSCOM.Inscricao_ISUTC_DLL")
        Dim resposta As String

        resposta = objDll.GerarInscricaoIsutc(1, "Transfenix", _
             "accsys", "accsys2011", "00011", "guimas teste", _
             "", "", "", "", "", _
             "", "", "", "", _
             "", "", "", "", _
             "")
        MsgBox(resposta)

        objDll = Nothing
    End Sub
End Class

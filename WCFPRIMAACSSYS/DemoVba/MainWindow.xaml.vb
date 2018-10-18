Imports System.Reflection

Class MainWindow

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'Dim a As New Aluno_Controller
        'a.abrirConfiguracoes()

        Try
            Dim objDll As Object

            'Dim type As Type = type.GetTypeFromProgID("CSU_TRANSCOM.Inscricao_ISUTC_DLL")
            'objDll = Activator.CreateInstance(type)


            'Dim resposta As String
            'Dim args() As [Object] = {1, "Transfenix", _
            '     "accsys", "accsys2011", "00011", "guimas teste", _
            '    "Maputo", "Bairro Central", "+258 84 95 35 156", "", _
            '     "False", "gmahota@accsys.co.mz", "", "ID", _
            '     "", ""}

            'resposta = CType(type.InvokeMember("GerarInscricaoIsutc", BindingFlags.InvokeMethod, Nothing, objDll, args), String)

            'Dim type As Type = type.GetTypeFromProgID("CSU_TRANSCOM.Inscricao_ISUTC_DLL")
            'objDll = Activator.CreateInstance(type)

            'Dim type As Type = type.GetTypeFromProgID("CSU_TRANSCOM.Inscricao_ISUTC_DLL")
            objDll = CreateObject("CSU_TRANSCOM.Inscricao_ISUTC_DLL") 'Activator.CreateInstance(type)
            Dim resposta As String

            Dim args() As [Object] = {1, "Transfenix", _
                 "accsys", "accsys2011", "00011", "guimas teste", _
                "Maputo", "Bairro Central", "+258 84 95 35 156", "", _
                 "False", "gmahota@accsys.co.mz", "", "ID", _
                 "", ""}
            If Not objDll Is Nothing Then
                resposta = objDll.GerarInscricaoIsutc(1, "Transfenix", _
                     "accsys", "accsys2011", "00011", "guimas teste", _
                    "Maputo", "Bairro Central", "+258 84 95 35 156", "", _
                     "False", "gmahota@accsys.co.mz", "", "ID", _
                     "", "")
            End If



            MsgBox(resposta)

            objDll = Nothing
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
End Class

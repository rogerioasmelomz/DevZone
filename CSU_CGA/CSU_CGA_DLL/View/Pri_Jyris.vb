Imports CSU_CGA

<ComClass(Pri_Jyris.ClassId, Pri_Jyris.InterfaceId, Pri_Jyris.EventsId)>
Public Class Pri_Jyris
    Public Const ClassId As String = "B664D118-F69B-4749-BBD0-C5948E59B272"
    Public Const InterfaceId As String = "A475DCB8-8FE6-4D6D-B1A6-C1DA983C9255"
    Public Const EventsId As String = "7355FB6C-6BB6-4815-8A34-FB70C0C67298"
    'Public janela As MainWindow

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String)
        Try
            Dim janela As ImportadorJyris
            janela = New ImportadorJyris()

            janela.jury_controller.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password)
        Catch ex As Exception
            MsgBox("erro ao abrir a janela" + ex.Message)
        End Try

    End Sub

    Public Sub New()

    End Sub
End Class

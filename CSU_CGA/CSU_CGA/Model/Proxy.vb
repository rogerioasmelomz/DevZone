<ComClass(Proxy.ClassId, Proxy.InterfaceId, Proxy.EventsId)>
Public Class Proxy
    Public Const ClassId As String = "0DAD61F3-B0B8-4642-A49D-3B37D04BD856"
    Public Const InterfaceId As String = "701B0744-9452-430E-A074-03499961AAD8"
    Public Const EventsId As String = "2FE80C14-6B58-4DAD-AB52-D04ED0646E15"

    Public janela As MainWindow

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String)
        janela = New MainWindow()
        janela.jury_controller.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password)

    End Sub
End Class

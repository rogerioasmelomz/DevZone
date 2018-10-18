Imports Interop.ErpBS800
Imports Interop.StdBE800

Public Class Jvris_Controller

    Public objmotor As ErpBS

    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    Dim result As PrimaveraResultStructure

    Public Sub InicializaMotor(motor As ErpBS)
        Me.objmotor = motor
    End Sub

    ''' <summary>
    ''' Metodo para inicializar o motor do primavera
    ''' </summary>
    ''' <param name="tipoPlataforma"> 0 - Executiva, 1- Profissional</param>
    ''' <param name="codEmpresa"></param>
    ''' <param name="codUsuario"></param>
    ''' <param name="password"></param>
    ''' <remarks></remarks>
    Public Function AbreEmpresaPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String) As String
        Try
            If objmotor Is Nothing Then
                objmotor = New ErpBS
            Else
                objmotor.FechaEmpresaTrabalho()
            End If

            Me.tipoPlataforma = tipoPlataforma
            Me.codUsuario = codUsuario
            Me.codEmpresa = codEmpresa
            Me.password = password

            objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)

            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function EmpresaPrimaveraAberta() As Boolean
        If objmotor Is Nothing Then
            Return False
        Else
            Return objmotor.Contexto.EmpresaAberta
        End If
    End Function

    ''' <summary>
    ''' Valida se existe o relatorio
    ''' </summary>
    ''' <param name="numExterno"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RelatorioExiste(numExterno As String)

        'Dim palavra As String
        Dim lista As StdBELista

        lista = objmotor.Consulta("select * from cabecdoc where Referencia ='" + numExterno + "'")

        If lista.Vazia Then Return True

        Return False
    End Function

    'Operacao 1
    Public Sub LeituraPrimavera()

        'Valida se o relatorio existe
        If (RelatorioExiste("")) Then

        Else

        End If

    End Sub

End Class

Imports System.Reflection

Public Class Aluno_Controller
    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    Public objmotor As Object

    Dim result As PrimaveraResultStructure
    Public aluno As Aluno


    Public Function GerarInscricaoIsutc(tipoPlataforma As Short, codEmpresa As String, _
        codUsuario As String, password As String, nrEstudante As String, nomeCompleto As String, _
        morada As String, bairro As String, nrTelefone As String, nrTelefone2 As String, _
        bolsa As String, emailIsutc As String, emailAlternativo As String, _
        tipoIngresso As String, status As String, observacao As String) As PrimaveraResultStructure

        Try

            Dim result = New PrimaveraResultStructure()

            Dim objDll As Object

            Dim type As Type = type.GetTypeFromProgID("CSU_TRANSCOM.Inscricao_ISUTC_DLL")
            objDll = Activator.CreateInstance(type)

            Dim resposta As String
            Dim args() As [Object] = {tipoPlataforma, codEmpresa, _
                codUsuario, password, nrEstudante, nomeCompleto, morada, bairro, nrTelefone, nrTelefone2, _
                bolsa, emailIsutc, emailAlternativo, tipoIngresso, status, observacao}
            'Dim args() As [Object] = {"OLA"}


            resposta = CType(type.InvokeMember("GerarInscricaoIsutc", BindingFlags.InvokeMethod, Nothing, objDll, args), String)
            

            'resposta = objDll.GerarInscricaoIsutc(tipoPlataforma, codEmpresa, _
            '    codUsuario, password, nrEstudante, nomeCompleto, morada, bairro, nrTelefone, nrTelefone2, _
            '    bolsa, emailIsutc, emailAlternativo, tipoIngresso, status, observacao)

            Dim palavras As String() = Split(resposta, ";")
            Dim i As Integer = 0

            For Each palavra In palavras

                Select Case i

                    Case 0 : result.codigo = palavra
                    Case 1 : result.tipoProblema = palavra
                    Case 2 : result.codeLevel = palavra
                    Case 3 : result.descricao = palavra
                    Case 4 : result.procedimento = palavra
                End Select
                i = i + 1
            Next

            'result.descricao = resposta

            objDll = Nothing

            Return result

        Catch ex As Exception
            'objmotor.DesfazTransaccao()

            result = New PrimaveraResultStructure()
            result.tipoProblema = "Erro no sistema primavera"
            result.codeLevel = "21 - Erros com ficheiros dll"
            result.codigo = 2
            result.descricao = "210- Erro ao gravar o aluno " + ex.Message
            Return result
        End Try

    End Function


End Class

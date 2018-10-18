
Imports System.Globalization

Public Class Aluno


    Public nrEstudante, nomeCompleto, empresa, morada, bairro, nrTelefone, nrTelefone2, curso, emailIsutc, emailAlternativo, turma, tipoIngresso,
        status, observacao As String

    Public nrTalao As Long?

    Public dataRegistro As Date

    Public bolsa As Boolean

    Public geraMulta As Boolean



    Public Sub New()

    End Sub

    Public Function InicializaAtributos(nrEstudante As String, nomeCompleto As String, nrTalao As String, morada As String, bairro As String, _
            nrTelefone As String, nrTelefone2 As String, bolsa As String, curso As String, emailIsutc As String, emailAlternativo As String, _
            turma As String, tipoIngresso As String, status As String, observacao As String, dataRegisto As String) As PrimaveraResultStructure

        Dim mensaguem As String = ""
        Dim result = New PrimaveraResultStructure()

        result.codigo = 0


        If (nrEstudante <> "" And nrEstudante.Length < 13) Then
            Me.nrEstudante = nrEstudante
        Else
            mensaguem = mensaguem + "O Nr. de Estudante não pode ter mais de 12 caractes e não pode estar vazia"
        End If

        If (nomeCompleto <> "" And nomeCompleto.Length < 51) Then
            Me.nomeCompleto = nomeCompleto
        Else
            mensaguem = mensaguem + "O Nome Completo não pode ter mais de 50 caractes e não pode estar vazia"
        End If

        Try
            Me.nrTalao = Long.Parse(nrTalao)
        Catch ex As Exception
            If (nrTalao <> "") Then
                mensaguem = mensaguem + "'O nr de talao de deposito é invalido' "
            End If
        End Try

        If (morada <> "" And morada.Length < 51) Then
            Me.morada = morada
        Else
            mensaguem = mensaguem + "A Morada não pode ter mais de 50 caractes e não pode estar vazia"
        End If

        If (bairro <> "" Or bairro.Length < 51) Then
            Me.bairro = bairro
        Else
            mensaguem = mensaguem + "O Bairro não pode ter mais de 12 caractes e nem pode estar vazia"
        End If

        If (nrTelefone <> "" Or nrTelefone.Length < 21) Then
            Me.nrTelefone = nrTelefone
        Else
            mensaguem = mensaguem + "O Nr. de Telefone não pode ter mais de 20 caractes e nem pode estar vazia"
        End If

        If (nrTelefone2.Length < 21) Then
            Me.nrTelefone2 = nrTelefone2
        Else
            mensaguem = mensaguem + "O Nr. de Telefone 2 não pode ter mais de 20 caractes"
        End If

        Try
            Me.bolsa = Convert.ToBoolean(bolsa)
        Catch ex As Exception
            Me.bolsa = False
        End Try


        If (curso <> "" Or curso.Length < 21) Then
            Me.curso = curso
        Else
            mensaguem = mensaguem + "O Curso não pode ter mais de 21 caractes e não pode estar vazia"
        End If

        If (emailIsutc.Length < 101) Then
            Me.emailIsutc = emailIsutc
        Else
            mensaguem = mensaguem + "O emailIsutc não pode ter mais de 100 caractes e nem pode estar vazia"
        End If

        If (emailAlternativo.Length < 101) Then
            Me.emailAlternativo = emailAlternativo
        Else
            mensaguem = mensaguem + "O email Alternativo não pode ter mais de 100 caractes e nem pode estar vazia"
        End If

        If (turma.Length < 21) Then
            Me.turma = turma
        Else
            mensaguem = mensaguem + "A turma não pode ter mais de 21 caractes"
        End If

        If (tipoIngresso <> "" Or tipoIngresso.Length < 101) Then
            Me.tipoIngresso = tipoIngresso
        Else
            mensaguem = mensaguem + "O Tipo de Ingresso não pode ter mais de 100 caractes e não pode estar vazia"
        End If

        If (status <> "" Or status.Length < 251) Then
            Me.status = status
        Else
            mensaguem = mensaguem + "O Status não pode ter mais de 100 caractes e não pode estar vazia"
        End If

        If (observacao.Length < 251) Then
            Me.observacao = observacao
        Else
            mensaguem = mensaguem + "A observação não pode ter mais de 250 caractes"
        End If

        Try
            Dim provider = New CultureInfo("pt-PT")
            Me.dataRegistro = Date.ParseExact(dataRegisto, "dd/MM/yyyy", CultureInfo.InstalledUICulture)
        Catch ex As Exception
            mensaguem = mensaguem + "'A Data de Registro ( " + dataRegisto + ") é invalida, deve obedecer o formato dd/MM/yyyy' "
        End Try

        If (mensaguem = "") Then
            result.codigo = 0
        Else
            result.codigo = 1
            result.tipoProblema = "Erro de Marshalling de dados"
            result.codeLevel = "10 - Validação de Campos"
            result.descricao = mensaguem
        End If

        Return result
    End Function

End Class

Public Class PrimaveraResultStructure

    Public tipoProblema As String

    Public codigo As Integer

    Public codeLevel As String

    Public subNivel As String

    Public descricao As String

    Public procedimento As String

End Class
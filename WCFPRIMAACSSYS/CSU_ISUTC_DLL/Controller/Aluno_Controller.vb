Imports Interop.StdBE800
Imports Interop.ErpBS800
Imports Interop.GcpBE800
Imports Interop.ICrmBS800
Imports Interop.CrmBE800
Imports System.Xml
Imports System.Reflection

Public Class Aluno_Controller
    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    Public objmotor As ErpBS
    Dim reader As XmlTextReader
    Dim result As PrimaveraResultStructure

    Public Const pastaConfig As String = "PRIMAVERA\\SG800"

    Public Sub New()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve

    End Sub

    ''' <summary>
    ''' Método para resolução das assemblies.
    ''' </summary>
    ''' <param name="sender">Application</param>
    ''' <param name="args">Resolving Assembly Name</param>
    ''' <returns>Assembly</returns>
    Public Function CurrentDomain_AssemblyResolve1(sender As Object, args As ResolveEventArgs) As System.Reflection.Assembly
        Dim assemblyFullName As String
        Dim assemblyName As System.Reflection.AssemblyName
        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig '"PRIMAVERA\\S800" ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
        assemblyName = New System.Reflection.AssemblyName(args.Name)
        assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll")
        If (System.IO.File.Exists(assemblyFullName)) Then
            Return System.Reflection.Assembly.LoadFile(assemblyFullName)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary> 
    '''     Módulo de exemplo de carregamento de Interops da pasta de ficheiros comuns. 
    ''' </summary> 
    ''' <remarks></remarks> 
    <Runtime.InteropServices.ComVisible(False)> _
    Public Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly

        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig  '"PRIMAVERA\\SG800"

        Dim outAssembly, objExeAssembly As Assembly
        Dim strTempAssemblyPath As String = ""
        Dim strArgToLoad As String

        objExeAssembly = Assembly.GetExecutingAssembly
        Dim arrRefAssemblyNames() As AssemblyName = objExeAssembly.GetReferencedAssemblies

        strArgToLoad = args.Name.Substring(0, args.Name.IndexOf(","))

        For Each strAName As AssemblyName In arrRefAssemblyNames

            If strAName.FullName.Substring(0, strAName.FullName.IndexOf(",")) = strArgToLoad Then

                strTempAssemblyPath = System.IO.Path.Combine(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER),
                    strArgToLoad + ".dll"
                )

                Exit For
            End If

        Next

        'Valida nome do assembly
        If String.IsNullOrEmpty(strTempAssemblyPath) Then
            outAssembly = Nothing
        Else
            outAssembly = Assembly.LoadFrom(strTempAssemblyPath)
        End If

        Return outAssembly

    End Function

    Public Sub abrirConfiguracoes()

        reader = New XmlTextReader("resources\Configuracao.xml")
        Try
            If (IO.File.Exists("resources\Configuracao.xml")) Then

                Dim document As XmlReader = New XmlTextReader("resources\Configuracao.xml")

                While (document.Read())

                    Dim type = document.NodeType


                    If (type = XmlNodeType.Element) Then


                        Select Case document.Name
                            Case "tipoPlataforma"
                                tipoPlataforma = Convert.ToInt32(document.ReadInnerXml.ToString())
                            Case "empresa"
                                codEmpresa = document.ReadInnerXml.ToString()
                            Case "usuario"
                                codUsuario = document.ReadInnerXml.ToString()
                            Case "password"
                                password = document.ReadInnerXml.ToString()
                        End Select

                    End If


                End While

            Else
                result = New PrimaveraResultStructure
                result.tipoProblema = "Erro do Sistema no Primavera"
                result.codigo = 2
                result.codeLevel = "21 - Erros com ficheiros DLL"
                result.descricao = "210 - Ficheiro de reources\Configuracao.xml não existe"
            End If
        Catch ex As Exception
            result = New PrimaveraResultStructure
            result.tipoProblema = "Erro do Sistema no Primavera"
            result.codigo = 2
            result.codeLevel = "21 - Erros com ficheiros DLL"
            result.descricao = "210 - Erro no metodo Abrir as Configuracoes /n" + ex.Message
        End Try

    End Sub

    Public Function EmpresaPrimaveraAberta() As Boolean
        If objmotor Is Nothing Then
            Return False
        Else
            Return objmotor.Contexto.EmpresaAberta
        End If
    End Function

    Public Function GravarAluno(aluno As Aluno) As PrimaveraResultStructure
        Try
            Dim result = New PrimaveraResultStructure()

            Dim objcliente As GcpBECliente

            'Inicia a transaccao
            objmotor.IniciaTransaccao()

            'Verifica se o Aluno Existe
            If (objmotor.Comercial.Clientes.Existe(aluno.nrEstudante) = True) Then

                'Actualiza os dados do aluno
                Try
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "nome", aluno.nomeCompleto)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "NomeFiscal", aluno.nomeCompleto)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "Moeda", "MT")
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "TipoTerceiro", daCodLicienciatura(aluno.curso))
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_CodLic", aluno.curso)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_Turma", aluno.turma)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_Bolsa", aluno.bolsa)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_TipoIngresso", aluno.tipoIngresso)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_GeraMulta", aluno.geraMulta)
                Catch ex As Exception
                    objmotor.DesfazTransaccao()

                    result.codigo = 3
                    result.tipoProblema = "Erro lógica no Primavera"
                    result.codeLevel = "30 - Erro ao actualizar os dados do aluno"
                    result.descricao = ex.Message
                    Return result
                End Try

            Else
                'Cria um novo aluno
                Try
                    objcliente = New GcpBECliente

                    objcliente.Cliente = aluno.nrEstudante
                    objcliente.Nome = aluno.nomeCompleto
                    objcliente.NomeFiscal = aluno.nomeCompleto
                    objcliente.Morada = aluno.morada

                    objcliente.Moeda = "MT"
                    objcliente.TipoTerceiro = daCodLicienciatura(aluno.curso)
                    objcliente.CamposUtil("CDU_CodLic").Valor = aluno.curso
                    objcliente.CamposUtil("CDU_Turma").Valor = aluno.turma
                    objcliente.CamposUtil("CDU_Bolsa").Valor = aluno.bolsa
                    objcliente.CamposUtil("CDU_TipoIngresso").Valor = aluno.tipoIngresso
                    objcliente.CamposUtil("CDU_GeraMulta").Valor = aluno.geraMulta

                    objmotor.Comercial.Clientes.Actualiza(objcliente)

                    result.codigo = 0
                    result.tipoProblema = "Codigos de Sucesso"
                    result.codeLevel = "00 - O Aluno Gravado Sucesso Completo"
                    result.descricao = "000 - Gravado com sucesso"

                Catch ex As Exception
                    objmotor.DesfazTransaccao()

                    result.codigo = 3
                    result.tipoProblema = "Erro lógica no Primavera"
                    result.codeLevel = "30 - Erro ao gravar um novo aluno"
                    result.descricao = ex.Message
                    Return result
                End Try

            End If

            objmotor.TerminaTransaccao()


            Return result


        Catch ex As Exception
            objmotor.DesfazTransaccao()
            result.codigo = 3
            result.descricao = Err.Description + " " + "- Erro ao gravar o aluno"
            Return result
        End Try
    End Function

    Public Function GerarInscricaoIsutc(aluno As Aluno) As PrimaveraResultStructure

        Try
            Dim result = New PrimaveraResultStructure()

            Dim objContacto As CrmBEContacto
            Dim objEntidateContacto As CrmBELinhaContactoEntidade
            Dim objcliente As GcpBECliente

            'Inicia a transaccao
            objmotor.IniciaTransaccao()

            Dim erro As String
            erro = ""

            'Verifica se o Aluno Existe
            If (objmotor.Comercial.Clientes.Existe(aluno.nrEstudante) = True) Then

                'Actualiza os dados do aluno
                Try
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "nome", aluno.nomeCompleto)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "NomeFiscal", aluno.nomeCompleto)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "Moeda", "MT")
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "TipoTerceiro", daCodLicienciatura(aluno.curso))
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_CodLic", aluno.curso)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_Turma", aluno.turma)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_Bolsa", aluno.bolsa)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_TipoIngresso", aluno.tipoIngresso)
                    objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno.nrEstudante, "CDU_GeraMulta", aluno.geraMulta)
                Catch ex As Exception
                    objmotor.DesfazTransaccao()

                    result.codigo = 3
                    result.tipoProblema = "Erro lógica no Primavera"
                    result.codeLevel = "30 - Erro ao actualizar os dados do aluno"
                    result.descricao = ex.Message
                    Return result
                End Try

            Else
                'Cria um novo aluno
                Try
                    objcliente = New GcpBECliente

                    objcliente.Cliente = aluno.nrEstudante
                    objcliente.Nome = aluno.nomeCompleto
                    objcliente.NomeFiscal = aluno.nomeCompleto
                    objcliente.Morada = aluno.morada

                    objcliente.Moeda = "MT"
                    objcliente.TipoTerceiro = daCodLicienciatura(aluno.curso)
                    objcliente.CamposUtil("CDU_CodLic").Valor = aluno.curso
                    objcliente.CamposUtil("CDU_Turma").Valor = aluno.turma
                    objcliente.CamposUtil("CDU_Bolsa").Valor = aluno.bolsa
                    objcliente.CamposUtil("CDU_TipoIngresso").Valor = aluno.tipoIngresso
                    objcliente.CamposUtil("CDU_GeraMulta").Valor = aluno.geraMulta

                    objmotor.Comercial.Clientes.Actualiza(objcliente)

                    result.codigo = 0
                    result.tipoProblema = "Codigos de Sucesso"
                    result.codeLevel = "00 - O Aluno Gravado Sucesso Completo"
                    result.descricao = "000 - Gravado com sucesso"

                Catch ex As Exception
                    objmotor.DesfazTransaccao()

                    result.codigo = 3
                    result.tipoProblema = "Erro lógica no Primavera"
                    result.codeLevel = "30 - Erro ao gravar um novo aluno"
                    result.descricao = ex.Message
                    Return result
                End Try

            End If

            Try

                'Valida se existe o contacto do aluno no sistema

                If (objmotor.CRM.Contactos.Existe(aluno.nrEstudante) = True) Then

                    'Actualiza os contactos do aluno

                    Try

                        objContacto = objmotor.CRM.Contactos.Edita(aluno.nrEstudante)

                        objContacto.EmModoEdicao = True
                        objContacto.Contacto = aluno.nrEstudante
                        objContacto.Nome = aluno.nomeCompleto
                        objContacto.Morada = aluno.morada
                        'objContacto.Morada2 = aluno morada2
                        objContacto.Telefone = aluno.nrTelefone
                        objContacto.Telefone2 = aluno.nrTelefone2
                        objContacto.Email = aluno.emailIsutc
                        objContacto.EmailResid = aluno.emailAlternativo

                        objmotor.CRM.Contactos.Actualiza(objContacto)

                        result.codigo = 0
                        result.tipoProblema = "Codigos de Sucesso"
                        result.codeLevel = "00 - Contacto Actualizado com Sucesso Completo"
                        result.descricao = "000 - Contacto actualizado com sucesso"

                        result.codigo = 0
                        result.tipoProblema = "Códigos de Sucesso"
                        result.codeLevel = "00 - Sucesso ao Actualizar os contactos"
                        result.descricao = "000 - Sucesso ao Actualizar os contactos"

                    Catch ex As Exception
                        result.codigo = 0
                        result.tipoProblema = "Códigos de Sucesso"
                        result.codeLevel = "02 - Sucesso com Aviso ao actualizar os contactos"
                        result.descricao = ex.Message

                        Return result
                    End Try




                Else

                    Try
                        objContacto = New CrmBEContacto


                        objContacto.ID = System.Guid.NewGuid().ToString()
                        objContacto.Contacto = aluno.nrEstudante
                        objContacto.Nome = aluno.nomeCompleto
                        objContacto.Morada = aluno.morada
                        'objContacto.Morada2 = aluno morada2
                        objContacto.Telefone = aluno.nrTelefone
                        objContacto.Telefone2 = aluno.nrTelefone2
                        objContacto.Email = aluno.emailIsutc
                        objContacto.EmailResid = aluno.emailAlternativo

                        'objmotor.CRM.Contactos.Actualiza(objContacto)

                        objEntidateContacto = New CrmBELinhaContactoEntidade

                        With objEntidateContacto
                            .IDContacto = objContacto.ID
                            .Entidade = aluno.nrEstudante
                            .TipoEntidade = "C"
                            .Email = aluno.emailIsutc
                            .Telefone = aluno.nrTelefone
                            .Telemovel = aluno.nrTelefone2
                            .TipoContacto = "ALUNO" ' Para j??fixo
                        End With



                        objContacto.LinhasEntidade.Insere(objEntidateContacto)

                        objmotor.CRM.Contactos.Actualiza(objContacto)

                        result.codigo = 0
                        result.tipoProblema = "Codigos de Sucesso"
                        result.codeLevel = "00 - Contacto Criado com Sucesso Completo"
                        result.descricao = "000 - Contacto Criado com sucesso"

                    Catch ex As Exception
                        result.codigo = 0
                        result.tipoProblema = "Códigos de Sucesso"
                        result.codeLevel = "02 - Sucesso com Aviso ao gravar nos contactos"
                        result.descricao = ex.Message

                        Return result
                    End Try




                End If
            Catch ex As Exception
                result.codigo = 0
                result.tipoProblema = "Códigos de Sucesso"
                result.codeLevel = "02 - Sucesso com Aviso ao gravar nos contactos"
                result.descricao = ex.Message

                Return result
            End Try


            objmotor.TerminaTransaccao()


            Return result


        Catch ex As Exception
            objmotor.DesfazTransaccao()
            result.codigo = 3
            result.descricao = Err.Description + " " + "- Erro ao gravar o aluno"
            Return result
        End Try




    End Function

    ''' <summary>
    ''' Metodo para inicializar o motor do primavera
    ''' </summary>
    ''' <param name="tipoPlataforma"> 0 - Executiva, 1- Profissional</param>
    ''' <param name="codEmpresa"></param>
    ''' <param name="codUsuario"></param>
    ''' <param name="password"></param>
    ''' <remarks></remarks>

    Public Function AbreEmpresaPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String) As PrimaveraResultStructure
        On Error GoTo trataerro

        result = New PrimaveraResultStructure


        If objmotor Is Nothing Then
            objmotor = New ErpBS
            Me.tipoPlataforma = tipoPlataforma
            Me.codUsuario = codUsuario
            Me.codEmpresa = codEmpresa
            Me.password = password

            objmotor.AbreEmpresaTrabalho(Me.tipoPlataforma, Me.codEmpresa, Me.codUsuario, Me.password)

        Else
            objmotor.FechaEmpresaTrabalho()
        End If




        result.codigo = 0
        result.tipoProblema = "Codigos de Sucesso;"
        result.subNivel = "00 - Empresa aberta, Sucesso Completo;"
        result.descricao = "000 - Gravado com sucesso"

        Return result

        Exit Function
trataerro:
        result.codigo = 2
        result.tipoProblema = "Erro de sistema no Primavera"
        result.subNivel = "20 - Erros com a DB"
        result.descricao = "200 - Primavera não consegue conectar a Base de Dados motivo : - " + Err.Description
        result.procedimento = "Consultar os técnicos do projecto"

        Return result

    End Function

    Public Function EmpresaPrimaveraAberta() As Boolean
        If objmotor Is Nothing Then
            Return False
        Else
            Return objmotor.Contexto.EmpresaAberta
        End If
    End Function

    Public Function GravarAlunoIsutc(nrEstudante As String, nomeCompleto As String, morada As String, _
            bolsa As String, tipoIngresso As String, status As String, observacao As String, nuit As String) As PrimaveraResultStructure

        On Error GoTo trataerro

        result = New PrimaveraResultStructure


        'Verifica se o Aluno Existe
        If (objmotor.Comercial.Clientes.Existe(nrEstudante) = True) Then

            'Actualiza os dados do aluno
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "nome", nomeCompleto)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "NomeFiscal", nomeCompleto)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "Moeda", "MT")
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "Morada", morada)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "NumContribuinte", nuit)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "CDU_Bolsa", bolsa)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "CDU_TipoIngresso", tipoIngresso)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "CondPag", "10")
            objmotor.Comercial.Clientes.ActualizaValorAtributo(nrEstudante, "Observacoes", observacao)

            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso;"
            result.subNivel = "00 - O Aluno Gravado Sucesso Completo"
            result.descricao = "000 - Gravado com sucesso"

            Return result

        Else

            'Cria um novo aluno
            Dim objcliente As GcpBECliente

            objcliente = New GcpBECliente

            objcliente.Cliente = nrEstudante
            objcliente.Nome = nomeCompleto
            objcliente.NomeFiscal = nomeCompleto
            objcliente.Morada = morada
            objcliente.Moeda = "MT"
            objcliente.CondPag = "10"
            objcliente.NumContribuinte = nuit
            objcliente.CamposUtil("CDU_Bolsa").Valor = bolsa
            objcliente.CamposUtil("CDU_TipoIngresso").Valor = tipoIngresso
            objcliente.Observacoes = observacao

            objmotor.Comercial.Clientes.Actualiza(objcliente)

            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso;"
            result.subNivel = "00 - O Aluno Gravado Sucesso Completo"
            result.descricao = "000 - Gravado com sucesso"

            Return result

        End If

        Exit Function
trataerro:

        result.codigo = 3
        result.tipoProblema = "Erro Logica no Primavera;"
        result.subNivel = "30 - O Erro ao gravar o aluno"
        result.descricao = "Erro ao gravar o aluno - " + Err.Description
        result.procedimento = "Consultar os técnicos do projecto;"

        Return result

    End Function


    Public Function GravarContactoAluno(nrEstudante As String, nomeCompleto As String, morada As String, bairro As String, nrTelefone As String, nrTelefone2 As String, _
            emailIsutc As String, emailAlternativo As String) As PrimaveraResultStructure

        On Error GoTo trataerro

        result = New PrimaveraResultStructure

        Dim objContacto As CrmBEContacto
        Dim objEntidateContacto As CrmBELinhaContactoEntidade

        'Valida se existe o contacto do aluno no sistema

        If (objmotor.CRM.Contactos.Existe(nrEstudante) = True) Then

            'Actualiza os contactos do aluno

            objContacto = objmotor.CRM.Contactos.Edita(nrEstudante)

            objContacto.EmModoEdicao = True
            objContacto.Contacto = nrEstudante
            objContacto.Nome = nomeCompleto
            objContacto.Morada = morada
            'objContacto.Morada2 = aluno morada2
            objContacto.Localidade = bairro
            objContacto.Telefone = nrTelefone
            objContacto.Telefone2 = nrTelefone2
            objContacto.Email = emailIsutc
            objContacto.EmailResid = emailAlternativo

            objmotor.CRM.Contactos.Actualiza(objContacto)

            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso;"
            result.subNivel = "00 - Sucesso ao Actualizar os contactos"
            result.descricao = "000 - Gravado com sucesso"

            Return result

        Else


            objContacto = New CrmBEContacto

            objContacto.ID = Guid.NewGuid().ToString
            objContacto.Contacto = nrEstudante
            objContacto.Nome = nomeCompleto
            objContacto.Morada = morada
            'objContacto.Morada2 = aluno morada2
            objContacto.Localidade = bairro
            objContacto.Telefone = nrTelefone
            objContacto.Telefone2 = nrTelefone2
            objContacto.Email = emailIsutc
            objContacto.EmailResid = emailAlternativo

            objmotor.CRM.Contactos.Actualiza(objContacto)

            objEntidateContacto = New CrmBELinhaContactoEntidade

            With objEntidateContacto
                .IDContacto = objContacto.ID
                .Entidade = nrEstudante
                .TipoEntidade = "C"
                .Email = emailIsutc
                .Telefone = nrTelefone
                .Telemovel = nrTelefone2
                .TipoContacto = "ALUNO" ' Para j??fixo
            End With

            objContacto.LinhasEntidade.Insere(objEntidateContacto)

            objmotor.CRM.Contactos.Actualiza(objContacto)



            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso;"
            result.subNivel = "00 - Sucesso ao Actualizar os contactos"
            result.descricao = "000 - Gravado com sucesso"

            Return result

        End If

        Exit Function
trataerro:

        result.codigo = 3
        result.tipoProblema = "Erro Logica no Primavera;"
        result.subNivel = "30 - Erro ao gravar os contactos;"
        result.descricao = "Erro ao gravar o aluno - " + Err.Description
        result.procedimento = "Consultar os técnicos do projecto;"

        Return result

    End Function

    Private Function EmitirFactura(aluno As String, tipoIngresso As String) As PrimaveraResultStructure

        On Error GoTo trataerro

        Dim strSQL As String
        Dim objLista As Object

        Dim DocV As GcpBEDocumentoVenda

        DocV = New GcpBEDocumentoVenda

        result = New PrimaveraResultStructure

        DocV.Tipodoc = "FA"
        DocV.Serie = objmotor.Comercial.Series.DaSerieDefeito("V", DocV.Tipodoc)

        DocV.TipoEntidade = "C"
        DocV.Entidade = aluno
        DocV.DataDoc = Now
        DocV.DataGravacao = Now

        objmotor.Comercial.Vendas.PreencheDadosRelacionados(DocV)

        objmotor.Comercial.Vendas.AdicionaLinha(DocV, tipoIngresso)

        objmotor.Comercial.Vendas.Actualiza(DocV)

        result.codigo = 0
        result.tipoProblema = "Codigos de Sucesso"
        result.subNivel = "00 - Sucesso ao Actualizar os contactos"
        result.descricao = "000 - Gravado com sucesso, Foi gerado a factura " + DocV.Tipodoc

        Return result

        Exit Function
trataerro:

        result.codigo = 3
        result.tipoProblema = "Erro Logica no Primavera;"
        result.subNivel = "30 - O Erro ao gravar a factura"
        result.descricao = "Erro ao gravar a facturao - " + Err.Description
        result.procedimento = "Consultar os técnicos do projecto;"

        Return result

    End Function


    Public Function GerarInscricaoIsutc(tipoPlataforma As Integer, codEmpresa As String, _
            codUsuario As String, password As String, nrEstudante As String, nomeCompleto As String, _
            morada As String, bairro As String, nrTelefone As String, nrTelefone2 As String, _
            bolsa As String, emailIsutc As String, emailAlternativo As String, _
            tipoIngresso As String, status As String, observacao As String, nuit As String) As PrimaveraResultStructure

        On Error GoTo trataerro

        result = New PrimaveraResultStructure

        result = AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password)
        'Se não ocorrer erros na criação grava/actualiza o contacto do aluno
        If (result.codigo = 0) Then
            result = GravarAlunoIsutc(nrEstudante, nomeCompleto, morada, bolsa, tipoIngresso, status, observacao, nuit)
            'objmotor.TerminaTransaccao()

        End If



        'Se não ocorrer erros na criação grava/actualiza o contacto do aluno
        If (result.codigo = 0) Then
            result = GravarContactoAluno(nrEstudante, nomeCompleto, morada, bairro, nrTelefone, _
        nrTelefone2, emailIsutc, emailAlternativo)
            'objmotor.TerminaTransaccao()

        End If


        'Emitir a factura da inscrição
        If (result.codigo = 0) Then
            'objmotor.IniciaTransaccao()
            result = EmitirFactura(nrEstudante, tipoIngresso)
        End If

        'If (result.codigo = 3) Then
        '    objmotor.DesfazTransaccao()
        'Else
        '    objmotor.TerminaTransaccao()
        'End If

        GerarInscricaoIsutc = result
        Exit Function
trataerro:

        result.codigo = 3
        result.tipoProblema = "Erro Logica no Primavera;"
        result.subNivel = "21 - Erros com ficheiros dll"
        result.descricao = "210- Primavera não consegue carregar um DLL" + Err.Description
        result.procedimento = "Consultar os técnicos do projecto;"

        Return result

    End Function

#Region "Importador antigo"
    ''' <summary>
    ''' Procedure : ImportaAlunosPlatao
    ''' Autor     : Guimarães Mahota, gmahota@accsys.co.mz
    ''' Data      : 20-08-2013
    ''' Descrição :
    ''' importação de motores primavera em tools -> references ->
    ''' primavera erpbs800 ,
    ''' primavera gcpbe800,
    ''' primavera crmbe
    '''Campos a serem chamados ao gravar / editar o aluno
    ''' </summary>
    ''' <param name="aluno"></param>
    ''' <param name="nome"></param>
    ''' <param name="nomeAbreviado"></param>
    ''' <param name="Morada"></param>
    ''' <param name="morada2"></param>
    ''' <param name="telefone"></param>
    ''' <param name="telefone2"></param>
    ''' <param name="celular"></param>
    ''' <param name="emailIsutc"></param>
    ''' <param name="emailPessoal"></param>
    ''' <param name="codlic"></param>
    ''' <param name="turma"></param>
    ''' <param name="geraMulta"></param>
    ''' <param name="bolsa"></param>
    ''' <remarks></remarks>

    Public Function GerarInscricaoIsutc(aluno As String, nome As String, ByRef nomeAbreviado As String, Morada As String, ByRef morada2 As String _
    , ByRef telefone As String, ByRef telefone2 As String, ByRef celular As String, ByRef emailIsutc As String, ByRef emailPessoal As String, _
         ByRef codlic As String, ByRef turma As String, ByRef geraMulta As Boolean, bolsa As Boolean) As String

        On Error GoTo trataerro

        Dim objContacto As CrmBEContacto
        Dim objEntidateContacto As CrmBELinhaContactoEntidade
        Dim objContactos As CrmBEContactos
        Dim objcliente As GcpBECliente



        Dim erro As String
        erro = ""
        If (objmotor.Comercial.Clientes.Existe(aluno) = True) Then
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "nome", nome)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "NomeFiscal", nomeAbreviado)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "Moeda", "MT")
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "TipoTerceiro", daCodLicienciatura(codlic))
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_CodLic", codlic)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_Turma", turma)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_Bolsa", bolsa)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_GeraMulta", geraMulta)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_Bolsa", bolsa)
            objmotor.Comercial.Clientes.ActualizaValorAtributo(aluno, "CDU_Novo", True)
        Else
            objcliente = New GcpBECliente

            objcliente.Cliente = aluno
            objcliente.Nome = nome
            objcliente.NomeFiscal = nomeAbreviado
            objcliente.Moeda = "MT"
            objcliente.TipoTerceiro = daCodLicienciatura(codlic)
            objcliente.CamposUtil("CDU_CodLic").Valor = codlic
            objcliente.CamposUtil("CDU_Turma").Valor = turma
            objcliente.CamposUtil("CDU_Bolsa").Valor = turma
            objcliente.CamposUtil("CDU_GeraMulta").Valor = geraMulta
            objcliente.CamposUtil("CDU_Bolsa").Valor = bolsa
            objcliente.CamposUtil("CDU_Novo").Valor = True

            objmotor.Comercial.Clientes.Actualiza(objcliente)
        End If

        If (objmotor.CRM.Contactos.Existe(aluno) = True) Then
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "nome", nome)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "Morada", Morada)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "Morada2", morada2)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "telefone", telefone)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "telefone2", telefone2)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "email", emailIsutc)
            objmotor.CRM.Contactos.ActualizaValorAtributo(aluno, "emailResid", emailPessoal)

            Dim i As Integer
            Dim j As Long

            objContactos = objmotor.CRM.Contactos.ListaContactosDaEntidade("C", aluno)
            For j = 1 To objContactos.NumItens

                objContacto = objContactos.Conteudo(j)

                For i = 1 To objContacto.LinhasEntidade.NumItens

                    If objContacto.LinhasEntidade(i).TipoContacto = "ALUNO" Then

                        objContacto.LinhasEntidade(i).EmModoEdicao = True
                        objContacto.LinhasEntidade(i).Email = emailIsutc
                        objContacto.LinhasEntidade(i).Telefone = telefone
                        objContacto.LinhasEntidade(i).Telemovel = telefone2

                    End If

                Next i
            Next j

        Else
            objContacto = New CrmBEContacto

            objContacto.Nome = nome
            objContacto.Morada = Morada
            objContacto.Morada2 = morada2
            objContacto.Telefone = telefone
            objContacto.Telefone2 = telefone2
            objContacto.Email = emailIsutc
            objContacto.EmailResid = emailPessoal

            objmotor.CRM.Contactos.Actualiza(objContacto)

            objEntidateContacto = New CrmBELinhaContactoEntidade

            With objEntidateContacto
                .IDContacto = objContacto.ID
                .Entidade = aluno
                .TipoEntidade = "C"
                .Email = emailIsutc
                .Telefone = telefone
                .Telemovel = telefone2
                .TipoContacto = "ALUNO" ' Para j??fixo
            End With

            objContacto.LinhasEntidade.Insere(objEntidateContacto)

            objmotor.CRM.Contactos.Actualiza(objContacto)
        End If
        Return erro
        Exit Function
trataerro:
        Return Err.Description + " " + "- Erro ao gravar o aluno"

    End Function

    ''' <summary>
    ''' Gerar o documento contrato, matricula e inscrição do novo ingresso do ITC    ''' 
    ''' </summary>
    ''' <param name="aluno"></param>
    ''' <param name="dataInscricao"></param>
    ''' <remarks>Criado por Guimarães Mahota - gmahota@accsys.co.mz</remarks>
    Private Sub gerarIngresso(aluno As String, dataInscricao As Date)

        On Error GoTo trataerro

        Dim strSQL As String
        Dim objLista As StdBELista

        Dim artigo As String
        Dim DocV As New GcpBEDocumentoVenda
        Dim sqlString As String

        Dim resultado As String

        resultado = ""


        DocV.Tipodoc = "IC"
        DocV.Serie = objmotor.Comercial.Series.DaSerieDefeito("V", DocV.Tipodoc)

        DocV.TipoEntidade = "C"
        DocV.Entidade = aluno
        DocV.DataDoc = dataInscricao
        DocV.DataGravacao = Today

        objmotor.Comercial.Vendas.PreencheDadosRelacionados(DocV)

        objmotor.Comercial.Vendas.AdicionaLinha(DocV, "TI")
        objmotor.Comercial.Vendas.AdicionaLinha(DocV, "MA")

        objmotor.Comercial.Vendas.Actualiza(DocV)

        'DocV.Serie
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    ''' <summary>
    ''' Apartir do codigo da licenciatura devolve o codigo do tipo terceiro
    ''' </summary>
    ''' <param name="codlic">Codigo da Licenciatura</param>
    ''' <returns>Codigo do tipo terceiro</returns>
    ''' <remarks></remarks>
    Public Function daCodLicienciatura(codlic As String) As String
        On Error GoTo trataerro

        Dim sqlString As String
        Dim objLista As StdBELista

        Dim resultado As String

        resultado = ""

        sqlString = vbNullString
        sqlString = "Select * from tipoterceiros where descricao='" & codlic & "'"
        objLista = objmotor.Consulta(sqlString)

        If objLista.Vazia = False Then
            resultado = objLista.Valor("Tipoterceiro")
        End If
        Return resultado
        Exit Function
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Function
#End Region
End Class

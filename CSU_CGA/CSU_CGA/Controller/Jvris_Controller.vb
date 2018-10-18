Imports Interop.ErpBS800
Imports Interop.StdBE800
Imports Interop.GcpBE800
Imports System.IO
Imports System.Globalization
Imports System.Threading
Imports System.Data
Imports System.Reflection

Public Class Jvris_Controller

#Region "Funções e varieveis do classe"
    Public objmotor As ErpBS

    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    Dim result As PrimaveraResultStructure
    Dim file As FileInfo
    
    Dim type As Type
    Dim objVba_CSU_Helper As Object
    Dim xmlHelper As XmlHelper = New XmlHelper
    Dim resposta As String
    Dim args() As [Object]
    Dim log As String

    Public Const pastaConfigV800 As String = "PRIMAVERA\\SG800"
    Public Const pastaConfigV900 As String = "PRIMAVERA\\SG900"

    Dim listaCabecDoc As List(Of CabecDoc) = New List(Of CabecDoc)

    Public Sub New()
        Try
            Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-PT")
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("pt-PT")

            FrameworkElement.LanguageProperty.OverrideMetadata( _
                GetType(FrameworkElement), _
                New FrameworkPropertyMetadata( _
                    System.Windows.Markup.XmlLanguage.GetLanguage( _
                    CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            )

            xmlHelper = New XmlHelper
            xmlHelper.loadFolder()

            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve

        Catch ex As Exception
            'MessageBox.Show(Err.Description, "Erro ao inicializar o controlador")
        End Try

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

        Dim PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.daPastaConfig() ' pastaConfig '"PRIMAVERA\\S800" ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.

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

        Dim PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.daPastaConfig()  '"PRIMAVERA\\SG800"

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

    Public Sub InicializaMotor(motor As Object)
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

            Me.tipoPlataforma = tipoPlataforma
            Me.codUsuario = codUsuario
            Me.codEmpresa = codEmpresa
            Me.password = password

            'VBA 6 Helper
            'type = type.GetTypeFromProgID("CSU_CGA_VBA6.Clientes_Helper")
            'objVba_CSU_Helper = Activator.CreateInstance(type)

            If objmotor Is Nothing Then
                objmotor = New ErpBS
            Else
                'objmotor.FechaEmpresaTrabalho()

            End If

            objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)

            Return ""
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Erro ao abrir a empresa Primavera ")
            Return "Erro ao abrir a empresa Primavera " + ex.Message
        End Try

    End Function
    Public Function EmpresaPrimaveraAberta() As Boolean
        If objmotor Is Nothing Then
            Return False
        Else
            Return objmotor.Contexto.EmpresaAberta
        End If
    End Function

    Public Function Integracao_Primavera(nomeficheiro As String, tipocaminho As String) As PrimaveraResultStructure
        Try

            log = ""

            Dim caminho As String = xmlHelper.daPasta(tipocaminho)
            IIf(caminho <> "", caminho, "C:\jvris_primavera\" + tipocaminho)

            file = New FileInfo(caminho + "\" + nomeficheiro) 'New FileInfo()

            'fileReader = My.Computer.FileSystem.OpenTextFileReader(caminho + "\" + nomeficheiro)

            LeituraPrimavera(file)

            If EmpresaPrimaveraAberta() = False Then
                xmlHelper.loadFolder()

                AbreEmpresaPrimavera(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa,
                                     xmlHelper.instancia.usuario, xmlHelper.instancia.password)
            End If

            For Each cabec In listaCabecDoc
                validaDadosClientes(cabec)
                validaDadosArtigos(cabec)

                'Operacao 2 - Relatorio Existe?
                If (RelatorioExiste(cabec)) Then

                    If (RelatorioTransformado(cabec)) Then
                        'Operacao 8 - Mensaguem de Alerta e Gera Nota de Credito
                        gerarNotaCredito(cabec)

                    Else
                        ' Operacao 4 - Relatorio ainda nao foi transformado
                        editarRelatorioNaoTransformado(cabec)
                    End If

                Else
                    'Operacao 3 - Relatorio Nao exite
                    gerarRelatorioNovo(cabec)
                End If

            Next

            TerminaOperacao()
            'gravaLog()

            Return result
        Catch ex As Exception
            EscreveLog(Err.Description + " Erro ao Ler o Metodo de Integração Primavera para o ficheiro: " + nomeficheiro, "Erro de Importação")
            TerminaOperacao()
            'gravaLog()
            Return Nothing
        End Try

    End Function

    Private Sub EscreveLog(mensaguem As String, pasta As String)
        log = log + Date.Now.ToString("[dd/MM/yyyy hh:mm:ss] ") + mensaguem '+ vbNewLine
        gravaLog(pasta)
    End Sub

    Private Sub gravaLog(Nomepasta As String)
        Dim pasta = xmlHelper.daPasta(Nomepasta)
        Dim objWriter As New System.IO.StreamWriter(pasta + "\" + file.Name.Substring(0, file.Name.Length - 3) + "log.txt", True)

        objWriter.WriteLine(log)
        log = ""
        objWriter.Close()
    End Sub

    Public Function GetDouble(ByVal doublestring As String) As Double
        Try
            If doublestring = "" Then
                Return 0
            Else
                Dim retval As Double
                Dim sep As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

                Double.TryParse(Replace(Replace(doublestring, ".", sep), ",", sep), retval)
                Return retval
            End If

        Catch ex As Exception
            'EscreveLog(Err.Description + " Erro ao fazer o cast para double")
            Return 0
            'Return Nothing
        End Try

    End Function

#End Region

#Region "Operação 1 - Leitura Pelo Primavera do Ficheiro JVRIS"
    'Operacao 1
    Public Function LeituraPrimavera(ficheiro As FileInfo) As PrimaveraResultStructure
        Try
            Dim stringReader As String
            Dim fileReader As System.IO.StreamReader
            Dim cabecDoc As CabecDoc = New CabecDoc
            Dim linhasdoc As LinhasDoc

            fileReader = My.Computer.FileSystem.OpenTextFileReader(ficheiro.FullName, Text.Encoding.GetEncoding("ISO-8859-1"))

            stringReader = fileReader.ReadLine()
            listaCabecDoc = New List(Of CabecDoc)


            Dim palavras As String()

            Dim i As Integer = 1

            While stringReader <> ""
                palavras = Split(stringReader, ";")
                Try
                    i = 1
                    If palavras(0) = "H" Then
                        cabecDoc = New CabecDoc
                        listaCabecDoc.Add(cabecDoc)

                        For Each palavra In palavras

                            Select Case i

                                Case 1 : cabecDoc.tipoRegisto = palavra
                                Case 2 : cabecDoc.tipoDoc = palavra

                                    'Dados cliente
                                Case 3 : cabecDoc.codigoEntidade = palavra
                                Case 4 : cabecDoc.nomeEntidade = palavra
                                Case 5 : cabecDoc.moradaEntidade = palavra
                                Case 6 : cabecDoc.localidadeEntidade = palavra
                                Case 7 : cabecDoc.codigoPostaEntidade = palavra
                                Case 8 : cabecDoc.localidadePostalEntidade = palavra
                                Case 9 : cabecDoc.nifEntidade = palavra
                                Case 10 : cabecDoc.paisEntidade = palavra

                                    'Dados comercias
                                Case 11 : cabecDoc.condicoesPag = palavra
                                Case 12 : cabecDoc.modPag = palavra
                                Case 13 : cabecDoc.codigoMoeda = palavra
                                Case 14 : cabecDoc.tipoMercadoErp = palavra
                                Case 15 : cabecDoc.segmentoErp = palavra
                                Case 16 : cabecDoc.serie = palavra
                                Case 17

                                    Try
                                        Dim provider = New CultureInfo("pt-PT")
                                        cabecDoc.data_Emissao = Date.ParseExact(palavra, "dd/MM/yyyy", CultureInfo.InstalledUICulture)
                                    Catch ex As Exception

                                    End Try

                                Case 18
                                    Try
                                        Dim provider = New CultureInfo("pt-PT")
                                        cabecDoc.dataVencimento = Date.ParseExact(palavra, "dd/MM/yyyy", CultureInfo.InstalledUICulture)
                                    Catch ex As Exception

                                    End Try

                                Case 19 : cabecDoc.numeroDocumentoRef = palavra
                                Case 20 : cabecDoc.valorCambio = GetDouble(palavra)
                                Case 21 : cabecDoc.percentagemDescontoEntidade = GetDouble(palavra)
                                Case 22 : cabecDoc.percentagemDescontoFinanceiro = GetDouble(palavra)

                            End Select
                            i = i + 1
                        Next
                    End If
                Catch ex As Exception
                    EscreveLog(Err.Description + ", Erro ao Ler no Header na coluna " + Str(i), "Erro de Importação")
                End Try

                Try
                    If palavras(0) = "L" Then
                        linhasdoc = New LinhasDoc

                        For Each palavra In palavras



                            Select Case i

                                Case 1 : linhasdoc.tipoRegisto = palavra
                                Case 2 : linhasdoc.artigo = palavra
                                Case 3 : linhasdoc.descricao = palavra
                                Case 4 : linhasdoc.tipoArtigo = palavra
                                Case 5
                                    Try
                                        Dim provider = New CultureInfo("pt-PT")
                                        linhasdoc.taxaIva = GetDouble(palavra)
                                    Catch ex As Exception

                                    End Try

                                Case 6
                                    Try
                                        linhasdoc.movStock = palavra
                                    Catch ex As Exception

                                    End Try

                                Case 7
                                    Try
                                        linhasdoc.sujeitoDevolucao = palavra
                                    Catch ex As Exception

                                    End Try

                                Case 8 : linhasdoc.codArmazem = palavra
                                Case 9 : linhasdoc.codLocalizacao = palavra
                                Case 10 : linhasdoc.precoUnitario = GetDouble(palavra)
                                Case 11 : linhasdoc.codUnidades = palavra
                                Case 12 : linhasdoc.quantidade = GetDouble(palavra)
                                Case 13 : linhasdoc.descontoLinha = GetDouble(palavra)

                            End Select
                            i = i + 1
                        Next

                        cabecDoc.linhasdoc.Add(linhasdoc)
                    End If
                Catch ex As Exception

                    EscreveLog(Err.Description + " Erro ao Ler as Linhas Doc na coluna " + Str(i) + " " + palavras(i) + " do relatorio " + cabecDoc.numeroDocumentoRef, "Erro de Importação")
                End Try

                stringReader = fileReader.ReadLine()
            End While

            fileReader.Close()
            result = New PrimaveraResultStructure

            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso"
            result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
            result.descricao = "000 - Gravado com sucesso"

            Return result
        Catch ex As Exception


            result.codigo = 1
            result.tipoProblema = Err.Description
            result.codeLevel = "01 - Erro Marcherling"
            result.descricao = "001 - Erro no Ficheiro de Importação"

            EscreveLog(result.getResultToString(), "Erro de Importação")

            Return Nothing
        End Try


    End Function

    ''' <summary>
    ''' Cria cliente no caso do cliente não existir no primavera
    ''' </summary>
    ''' <param name="cabecDoc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function validaDadosClientes(cabecDoc As CabecDoc) As PrimaveraResultStructure

        Dim result = New PrimaveraResultStructure()
        'Dim objlista As StdBELista
        Dim objCliente As New GcpBECliente  'Object
        Dim objLista As StdBELista 'Object 

        Dim resposta As String
        
        If objmotor.Comercial.Clientes.Existe(cabecDoc.codigoEntidade) Then
            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso"
            result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
            result.descricao = "000 - Gravado com sucesso"
        Else

            'Cria um novo cabecDoc
            Try
                objCliente = New GcpBECliente 'New CreateObject("GCPBE800.GcpBECliente")

                objCliente.Cliente = cabecDoc.codigoEntidade
                objCliente.Nome = cabecDoc.nomeEntidade
                objCliente.NomeFiscal = cabecDoc.nomeEntidade
                objCliente.Morada = cabecDoc.moradaEntidade
                objCliente.Localidade = cabecDoc.localidadeEntidade
                objCliente.CodigoPostal = cabecDoc.codigoPostaEntidade
                objCliente.LocalidadeCodigoPostal = cabecDoc.localidadePostalEntidade
                objCliente.NumContribuinte = cabecDoc.nifEntidade ' Validar com o cliente
                objCliente.Pais = cabecDoc.paisEntidade

                'Dados Comerciais
                objCliente.ModoPag = "CHQ"
                objCliente.Moeda = "MT"
                objCliente.CondPag = "2"

                objCliente.Pais = cabecDoc.paisEntidade
                objCliente.TipoMercado = cabecDoc.tipoMercado.ToString()

                'Entidades Associadas
                'objLista = objmotor.Consulta("select * from clientes where numcontrib ='" + objCliente.NumContribuinte + "'")
                resposta = ActualizaCliente(objCliente)

                'If resposta = "Gravado com sucesso" Then
                '    Dim clientDs As New ClienteDS
                '    Dim dsNewRow As DataRow

                '    If (Not objLista.Vazia) Then
                '        'While Not (objLista.Vazia Or objLista.NoFim)
                '        '    dsNewRow = clientDs.Tables("Clientes").NewRow()
                '        '    dsNewRow.Item("Cliente") = objLista.Valor("Cliente")
                '        '    dsNewRow.Item("Nome") = objLista.Valor("Nome")
                '        '    dsNewRow.Item("Numcontrib") = objLista.Valor("Numcontrib")

                '        '    clientDs.Tables("Clientes").Rows.Add(dsNewRow)
                '        '    objLista.Seguinte()
                '        'End While
                '        'Dim ClienteWindow = New ClienteWindow
                '        'ClienteWindow.inicilizarComponentes(clientDs, objmotor, cabecDoc.codigoEntidade, cabecDoc.nomeEntidade)
                '        'ClienteWindow.ShowDialog()
                '    Else
                '    End If
                result.codigo = 0
                result.tipoProblema = "Codigos de Sucesso"
                result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
                result.descricao = "000 - Gravado com sucesso"
                'Else
                'MessageBox.Show("Erro ao gravar o cliente " + resposta)
                'End If

            Catch ex As Exception
                'objmotor.DesfazTransaccao()

                result.codigo = 3
                result.tipoProblema = "Erro lógica no Primavera"
                result.codeLevel = "30 - Erro ao gravar um novo cabecDoc"
                result.descricao = ex.Message
                EscreveLog(result.getResultToString, "Erro de Importação")
                Return result
            End Try

        End If


        Return result
    End Function

    Public Function ActualizaCliente(objCliente As GcpBECliente) As String
        'AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUsuario, password)
        objmotor.Comercial.Clientes.Actualiza(objCliente)
        ActualizaCliente = "Gravado com sucesso"

        'objmotor.FechaEmpresaTrabalho()
    End Function

    ''' <summary>
    ''' Cria artigos no caso do artigo não existir no primavera
    ''' </summary>
    ''' <param name="linhadoc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function validaDadosArtigos(linhadoc As LinhasDoc) As PrimaveraResultStructure

        Dim result = New PrimaveraResultStructure()
        'Dim objlista As StdBELista
        Dim objArtigo As Object 'GcpBEArtigo

        'Dim resposta As String
        'Dim args() As [Object] = {tipoPlataforma, codEmpresa, codUsuario, password, linhadoc.artigo}

        'Verifica se o cabecDoc Existe
        If (objmotor.Comercial.Artigos.Existe(linhadoc.artigo) = True) Then
            'If (CType(type.InvokeMember("existeArtigo", BindingFlags.InvokeMethod, Nothing, objVba_CSU_Helper, args), Boolean) = True) Then

            result.codigo = 0
            result.tipoProblema = "Codigos de Sucesso"
            result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
            result.descricao = "000 - Gravado com sucesso"
        Else
            'Cria um novo cabecDoc
            Try
                objArtigo = New GcpBEArtigo 'CreateObject("GcpBe800.GcpBEArtigo") 
                objArtigo.Artigo = linhadoc.artigo
                objArtigo.Descricao = linhadoc.descricao
                objArtigo.TipoArtigo = "0"
                objArtigo.IVA = "17"
                objArtigo.MovStock = "N"
                objArtigo.SujeitoDevolucao = True
                objArtigo.UnidadeBase = "UN"
                objArtigo.UnidadeCompra = "UN"
                objArtigo.UnidadeEntrada = "UN"
                objArtigo.UnidadeVenda = "UN"
                objArtigo.UnidadeSaida = "UN"

                'objArtigo = objmotor.Comercial.Artigos.Edita("A1")

                'args = {tipoPlataforma, codEmpresa, codUsuario, password, objArtigo}

                'resposta = CType(type.InvokeMember("ActualizaArtigo", BindingFlags.InvokeMethod, Nothing, objVba_CSU_Helper, args), String)

                objmotor.Comercial.Artigos.Actualiza(objArtigo)

                result.codigo = 0
                result.tipoProblema = "Codigos de Sucesso"
                result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
                result.descricao = "000 - Gravado com sucesso"

            Catch ex As Exception


                result.codigo = 3
                result.tipoProblema = "Erro lógica no Primavera"
                result.codeLevel = "30 - Erro ao gravar um novo cabecDoc"
                result.descricao = ex.Message
                EscreveLog(result.getResultToString(), "Erro de Importação")
                Return result
            End Try

        End If


        Return result
    End Function
    Public Function validaDadosArtigos(cabec As CabecDoc) As PrimaveraResultStructure

        Dim result = New PrimaveraResultStructure()
        'Dim objlista As StdBELista
        Dim objArtigo As Object 'GcpBEArtigo

        'Dim resposta As String
        'Dim args() As [Object] = {tipoPlataforma, codEmpresa, codUsuario, password, linhadoc.artigo}

        For Each linhadoc In cabec.linhasdoc
            'Verifica se o artigo existe
            If (objmotor.Comercial.Artigos.Existe(linhadoc.artigo) = True) Then
                'If (CType(type.InvokeMember("existeArtigo", BindingFlags.InvokeMethod, Nothing, objVba_CSU_Helper, args), Boolean) = True) Then

                result.codigo = 0
                result.tipoProblema = "Codigos de Sucesso"
                result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
                result.descricao = "000 - Gravado com sucesso"
            Else
                'Cria um novo cabecDoc
                Try
                    objArtigo = New GcpBEArtigo 'CreateObject("GcpBe800.GcpBEArtigo") 
                    objArtigo.Artigo = linhadoc.artigo
                    objArtigo.Descricao = linhadoc.descricao
                    objArtigo.TipoArtigo = "0"
                    objArtigo.IVA = "17"
                    objArtigo.MovStock = "N"
                    objArtigo.SujeitoDevolucao = True
                    objArtigo.UnidadeBase = "UN"
                    objArtigo.UnidadeCompra = "UN"
                    objArtigo.UnidadeEntrada = "UN"
                    objArtigo.UnidadeVenda = "UN"
                    objArtigo.UnidadeSaida = "UN"

                    'args = {tipoPlataforma, codEmpresa, codUsuario, password, objArtigo}
                    'resposta = CType(type.InvokeMember("ActualizaArtigo", BindingFlags.InvokeMethod, Nothing, objVba_CSU_Helper, args), String)

                    objmotor.Comercial.Artigos.Actualiza(objArtigo)

                    result.codigo = 0
                    result.tipoProblema = "Codigos de Sucesso"
                    result.codeLevel = "00 - O cabecDoc Gravado Sucesso Completo"
                    result.descricao = "000 - Gravado com sucesso"

                Catch ex As Exception


                    result.codigo = 3
                    result.tipoProblema = "Erro lógica no Primavera"
                    result.codeLevel = "30 - Erro ao gravar um novo cabecDoc"
                    result.descricao = ex.Message
                    EscreveLog(result.getResultToString, "Erro de Importação")
                    Return result
                End Try

            End If
        Next

        Return result
    End Function
#End Region

#Region "Operação 2 - Relatório Existe?"
    'Operacao 2
    ''' <summary>
    ''' Valida se existe o relatorio
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RelatorioExiste(cabecDoc As CabecDoc) As Boolean
        Try
            'Dim palavra As String
            Dim objLista As Object 'StdBELista

            Dim query As String = "select cabecdoc.* from cabecdoc inner join CABECDOCSTATUS on Idcabecdoc = id where " & _
                                         "  anulado= 0 and fechado = 0 and Requisicao = '" &
                                         cabecDoc.numeroDocumentoRef & "'" & _
                                         " and tipodoc = '" & cabecDoc.tipoDoc & "'"
            'estado <> 'T' and

            objLista = objmotor.Consulta(query)
            ' and estado <> 'F'

            If Not objLista.Vazia Then

                cabecDoc.documentoGerado = objLista.Valor("TipoDoc") + " " + Str(objLista.Valor("Numdoc")) + "/" + objLista.Valor("Serie")

                Return True

            End If

            Return False
        Catch ex As Exception
            EscreveLog(Err.Description + " Erro ao validar se o relatorio existe", "Erro de Importação")
            Return Nothing
        End Try

    End Function
#End Region

#Region "Operação 3- Relatorio Existe = Não"

    Public Function gerarRelatorioNovo(cabecDoc As CabecDoc) As PrimaveraResultStructure
        Try
            Dim DocV As GcpBEDocumentoVenda
            DocV = New GcpBEDocumentoVenda

            DocV.Tipodoc = cabecDoc.tipoDoc


            args = {tipoPlataforma, codEmpresa, codUsuario, password, "V", DocV.Tipodoc}

            'Verifica se o cabecDoc Existe

            objmotor.Comercial.Series.DaSerieDefeito("V", DocV.Tipodoc)

            DocV.DataDoc = cabecDoc.dataEmissao
            DocV.DataGravacao = Today

            DocV.TipoEntidade = "C"
            DocV.Entidade = cabecDoc.codigoEntidade
            DocV.Requisicao = cabecDoc.numeroDocumentoRef

            objmotor.Comercial.Vendas.PreencheDadosRelacionados(DocV)

            For Each linha As LinhasDoc In cabecDoc.linhasdoc
                objmotor.Comercial.Vendas.AdicionaLinha(DocV, linha.artigo, linha.quantidade, , , linha.precoUnitario)
            Next

            objmotor.Comercial.Vendas.Actualiza(DocV)

            cabecDoc.numDoc = DocV.NumDoc
            cabecDoc.serie = DocV.Serie
            
            EscreveLog("Relatorio Importado, foi gerado o documento: " + cabecDoc.paraString(), "Importado")

            Dim pasta = xmlHelper.daPasta("Importado")
            
        Catch ex As Exception
            EscreveLog(ex.Message + " Erro ao Gerar Um novo Relatorio a partir do relatorio: " + cabecDoc.numeroDocumentoRef, "Erro de Importação")

        End Try

        Return result
    End Function

#End Region

#Region "Operação 4- Relatorio Existe = Sim"
    Private Function RelatorioTransformado(cabecDoc As CabecDoc) As Boolean
        'Dim palavra As String
        Dim objLista As StdBELista
        Dim palavra As String = "select cabecdoc.* from cabecdoc inner join CABECDOCSTATUS on Idcabecdoc = id where " & _
                                     " Requisicao = '" & cabecDoc.numeroDocumentoRef & "'" & _
                                     " and tipodoc = 'FA'"

        objLista = objmotor.Consulta(palavra)
        ' and estado <> 'F'

        If Not objLista.Vazia Then
            cabecDoc.documentoGerado = "FA" + " " + Str(objLista.Valor("Numdoc")) + "/ " + objLista.Valor("Serie")
            Return True

        End If


        Return False

    End Function
#End Region

#Region "Operacao 5 - Relatorio Existe = Sim e Relatorio Transformado = Nao"

    Public Function editarRelatorioNaoTransformado(cabecDoc As CabecDoc) As PrimaveraResultStructure
        Try
            Dim DocV As GcpBEDocumentoVenda 'Object
            DocV = New GcpBEDocumentoVenda 'CreateObject("GcpBe800.GcpBEDocumentoVenda")

            Dim objLista As StdBELista 'Object 


            objLista = objmotor.Consulta("select cabecdoc.* from cabecdoc inner join CABECDOCSTATUS on Idcabecdoc = id where " & _
                                     "estado <> 'T' and anulado= 0 and fechado = 0 and Requisicao = '" &
                                     cabecDoc.numeroDocumentoRef & "'" & _
                                     " and tipodoc = '" & cabecDoc.tipoDoc & "'")
            If Not objLista.Vazia Then

                DocV = objmotor.Comercial.Vendas.EditaID(objLista.Valor("Id"))

                DocV.Linhas.RemoveTodos()

                For Each linha As LinhasDoc In cabecDoc.linhasdoc
                    objmotor.Comercial.Vendas.AdicionaLinha(DocV, linha.artigo, linha.quantidade, , , linha.precoUnitario)
                Next

                objmotor.Comercial.Vendas.Actualiza(DocV)

                cabecDoc.numDoc = DocV.NumDoc
                cabecDoc.serie = DocV.Serie

                Dim pasta = xmlHelper.daPasta("Importado")

                EscreveLog("Foi Editado o documento " + cabecDoc.paraString(), "Importado")
            End If

        Catch ex As Exception
            EscreveLog(ex.Message + " Erro ao gravar o Relatorio : " + cabecDoc.numeroDocumentoRef, "Erro de Importação")
        End Try

        Return result
    End Function
#End Region

#Region "Operacao 8 - Gerar Nota de Credito"

    Public Function gerarNotaCredito(cabecdoc As CabecDoc) As PrimaveraResultStructure
        Dim opc = MessageBox.Show("O Honorario " + cabecdoc.numeroDocumentoRef + " já foi transformado na factura " + cabecdoc.documentoGerado + ", deseja criar uma nota de credito?", "Validação", MessageBoxButton.YesNo)

        If opc = MessageBoxResult.Yes Then

            If (FacturaLiquidada()) Then

                EscreveLog("Cancelado Pelo Utilizador - O Honorario já havia sido transformado em Factura : " + cabecdoc.documentoGerado, "Cancelados")

                'TerminaOperacao()
            Else
                GeraNotaCredito(cabecdoc)
            End If
        Else
            EscreveLog("Cancelado Pelo Utilizador - O Honorario já havia sido transformado em Factura : " + cabecdoc.documentoGerado, "Cancelados")

            'Dim pasta = xmlHelper.daPasta("Cancelados")
            'If pasta <> "" Then
            '    file.MoveTo(pasta + "\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'Else
            '    file.MoveTo("C:\jvris_primavera\Cancelados\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'End If

        End If

        Return New PrimaveraResultStructure
    End Function

    Private Function FacturaLiquidada() As Boolean

        'Throw New NotImplementedException
        Return False
    End Function

    Private Sub TerminaOperacao()
        Dim pasta = xmlHelper.daPasta("Ficheiros Exportados")
        file.MoveTo(pasta + "\" + file.Name)
       
    End Sub

    Private Sub GeraNotaCredito(cabecDoc As CabecDoc)
        Try
            Dim DocV As New GcpBEDocumentoVenda '  Object
            Dim docE As New GcpBEDocumentoVenda '  Object
            Dim objLista, objLista2 As StdBELista  'Object

            objLista = objmotor.Consulta("select cabecdoc.* from cabecdoc inner join CABECDOCSTATUS on Idcabecdoc = id where " & _
                                     " Requisicao = '" & cabecDoc.numeroDocumentoRef & "'" & _
                                     " and tipodoc = 'FA'")
            If Not objLista.Vazia Then

                objLista2 = objmotor.Consulta("select cb.* from LinhasLiq lq inner join CabecDoc cb on cb.TipoDoc = lq.TipoDocOrig and" &
                                              " cb.NumDoc = lq.NumDocOrig and cb.Serie = lq.SerieOrig " & _
                                     " where cb.Referencia = '" & cabecDoc.numeroDocumentoRef & "'" & _
                                     " and cb.TipoDoc = 'FA'")

                If objLista.Vazia Then

                    DocV = objmotor.Comercial.Vendas.EditaID(objLista.Valor("Id"))

                    docE = New GcpBEDocumentoVenda 'CreateObject("GcpBe800.GcpBEDocumentoVenda")
                    docE.Tipodoc = "NC"
                    docE.Serie = "2015"
                    docE.DataDoc = Today
                    docE.DataGravacao = Today
                    docE.TipoEntidade = "C"
                    docE.Entidade = cabecDoc.codigoEntidade
                    docE.Requisicao = cabecDoc.numeroDocumentoRef

                    objmotor.Comercial.Vendas.PreencheDadosRelacionados(docE)

                    For Each linha As GcpBELinhaDocumentoVenda In DocV.Linhas
                        objmotor.Comercial.Vendas.AdicionaLinha(docE, linha.Artigo, linha.Quantidade, , , linha.PrecUnit)

                    Next

                    objmotor.Comercial.Vendas.EstornaDocumentoVenda(objLista.Valor("Id"), "01", "Alteração", Today, Today, docE)

                    gerarRelatorioDeCorrecao(cabecDoc, "Foi Gerada a Nota de Credito " + Str(docE.NumDoc) + "/" + docE.Serie)

                Else

                    'MessageBox.Show("Ja foi gerada um documento de liquidação da factura : " + Str(objLista2.Valor("NumDoc")) + "/" + objLista2.Valor("Serie"))

                    EscreveLog(("Ja foi gerada um documento de liquidação da factura : " + Str(objLista2.Valor("NumDoc")) + "/" + objLista2.Valor("Serie")), "Cancelados")

                    Dim pasta = xmlHelper.daPasta("Erro de Importação")
                   
                End If
            End If

        Catch ex As Exception
            'MsgBox(ex.Message)
            EscreveLog(("Erro na importação contacte do administrador do primavera, " + ex.Message), "Erro de Importação")

            Dim pasta = xmlHelper.daPasta("Erro de Importação")
            'If pasta <> "" Then
            '    file.MoveTo(pasta + "\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'Else
            '    file.MoveTo("C:\jvris_primavera\Erro de Importação\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'End If

        End Try
    End Sub

    Public Function gerarRelatorioDeCorrecao(cabecDoc As CabecDoc, mensagem As String) As PrimaveraResultStructure
        Try
            Dim DocV As New GcpBEDocumentoVenda

            DocV.Serie = objmotor.Comercial.Series.DaSerieDefeito("V", DocV.Tipodoc)
            DocV.DataDoc = cabecDoc.dataEmissao
            DocV.DataGravacao = Today

            DocV.TipoEntidade = "C"
            DocV.Entidade = cabecDoc.codigoEntidade
            DocV.Requisicao = cabecDoc.numeroDocumentoRef

            objmotor.Comercial.Vendas.PreencheDadosRelacionados(DocV)

            For Each linha As LinhasDoc In cabecDoc.linhasdoc
                objmotor.Comercial.Vendas.AdicionaLinha(DocV, linha.artigo, linha.quantidade, , , linha.precoUnitario)
            Next

            objmotor.Comercial.Vendas.Actualiza(DocV)

            cabecDoc.numDoc = DocV.NumDoc
            cabecDoc.serie = DocV.Serie

            EscreveLog(mensagem + " e criado o honorario de correção: " + cabecDoc.paraString(), "Importado")

            Dim pasta = xmlHelper.daPasta("Importado")
           
        Catch ex As Exception
            'MsgBox(ex.Message)
            EscreveLog("Erro ao Gerar o relatorio de Correção do relatorio " + cabecDoc.numeroDocumentoRef, "Erro de Importação")
            Dim pasta = xmlHelper.daPasta("Erro de Importação")
            'If pasta <> "" Then
            '    file.MoveTo(pasta + "\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'Else
            '    file.MoveTo("C:\jvris_primavera\Erro de Importação\" + file.Name + "_( " + Today.ToString("dd_MM_yyyy") + ")")
            'End If

        End Try

        Return result
    End Function
#End Region

End Class

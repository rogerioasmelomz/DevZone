Imports System.Reflection
Imports Interop.ErpBS900
Imports Interop.StdBE900
Imports Interop.GcpBE900
Imports MIT.Data.Model
Imports Interop.AdmBS900
Imports Interop.AdmBE900

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Xml
Imports Microsoft.Win32
Imports System.Globalization

Public Class EmpresaErp

    Public objmotor As ErpBS

    Private Property tipoPlataforma As Integer

    Private Property codUsuario As String

    Private Property codEmpresa As String

    Private Property password As String

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
    Public Function AbreEmpresaPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String) As PrimaveraResultStructure
        Dim result = New PrimaveraResultStructure
        Try

            Me.tipoPlataforma = tipoPlataforma
            Me.codUsuario = codUsuario
            Me.codEmpresa = codEmpresa
            Me.password = password

            If objmotor Is Nothing Then
                objmotor = New ErpBS
            Else
                objmotor.FechaEmpresaTrabalho()
            End If

            objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)

            result.codigo = 0
            result.descricao = String.Format("Empresa {0} - {1} Aberta Com Sucesso", objmotor.Contexto.CodEmp, objmotor.Contexto.IDNome)

            Return result
        Catch ex As Exception
            result.codigo = 3
            result.descricao = ex.Message

            Return result
        End Try

    End Function

    Public Function EmpresaPrimaveraAberta() As Boolean
        If objmotor Is Nothing Then
            Return False
        Else
            Return objmotor.Contexto.EmpresaAberta
        End If
    End Function

    Public Function listaMoedas() As ICollection(Of Moeda)
        Dim lista As List(Of Moeda)
        Dim moedaTemp As Moeda
        Try
            Dim _listaMoedas = objmotor.Consulta(String.Format("Select * from moedas"))
            While Not (_listaMoedas.NoFim) And Not (_listaMoedas.NoInicio)
                moedaTemp = New Moeda

                With moedaTemp
                    .moedaId = _listaMoedas.Valor("moeda")
                    .descricao = _listaMoedas.Valor("Descricao")

                End With
                _listaMoedas.Seguinte()

            End While
        Catch ex As Exception

        End Try


        Return lista


    End Function

    Public Sub New()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
    End Sub
End Class

Public Class AdministradorErp1
    Public _administradorErp As AdmBS
    Protected objtrans As StdBETransaccao

    Public Sub inicializarAdministrador(tipoPlataforma As Integer, codUsuario As String, password As String, Optional instancia As String = "DEFAULT")
        _administradorErp = New AdmBS()
        objtrans = New StdBETransaccao()
        _administradorErp.AbrePRIEMPRE(tipoPlataforma, codUsuario, password, objtrans, instancia)

    End Sub

End Class

Public Class AdministradorErp
    Public Property User() As String
        Get
            Return m_User
        End Get
        Set(value As String)
            m_User = value
        End Set
    End Property
    Private m_User As String
    Public Property Password() As String
        Get
            Return m_Password
        End Get
        Set(value As String)
            m_Password = value
        End Set
    End Property
    Private m_Password As String
    Public Property Instance() As String
        Get
            Return m_Instance
        End Get
        Set(value As String)
            m_Instance = value
        End Set
    End Property
    Private m_Instance As String
    Public Property Type() As String
        Get
            Return m_Type
        End Get
        Set(value As String)
            m_Type = value
        End Set
    End Property
    Private m_Type As String
    Public Property Backupsdir() As String
        Get
            Return m_Backupsdir
        End Get
        Set(value As String)
            m_Backupsdir = value
        End Set
    End Property
    Private m_Backupsdir As String

    Private Property adm() As AdmBS
        Get
            Return m_adm
        End Get
        Set(value As AdmBS)
            m_adm = value
        End Set
    End Property
    Private m_adm As AdmBS

    Public Sub New(user__1 As String, password__2 As String, Optional instance__3 As String = "DEFAULT", Optional type__4 As String = "Executive", Optional backupsdir__5 As String = Nothing)
        User = user__1
        Password = password__2
        Instance = instance__3
        Type = type__4
        If backupsdir__5 IsNot Nothing Then
            Backupsdir = backupsdir__5
        End If

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
    End Sub

    Public Sub init()
        If adm Is Nothing Then
            adm = New AdmBS()
            Dim objtrans As New StdBETransaccao()

            Dim tp As EnumTipoPlataforma = EnumTipoPlataforma.tpProfissional
            If Type.Equals("Executive") Then
                tp = EnumTipoPlataforma.tpEmpresarial
            End If

            adm.AbrePRIEMPRE(tp, User, Password, objtrans, Instance)
        End If
    End Sub
    Public Sub [end]()
        adm.FechaPRIEMPRE()
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


    Public Sub cria_copia_seguranca(database As String)
        init()

        Dim bkpname As String = database & Convert.ToString(" backup")
        Dim bkpdescription As String = Convert.ToString("Full backup for ") & database
        Dim dir As String = ""
        If Backupsdir IsNot Nothing Then
            dir = Backupsdir
        End If

        Dim now As DateTime = DateTime.Now
        Dim nowString As String = now.ToString("yyyyMMddhhmmss")

        Dim f As String = Convert.ToString((Convert.ToString("Full_Backup_") & database) + "_") & nowString
        ' Common.WriteToConsoleOut(Convert.ToString("Backup file: ") & f)

        adm.BasesDados.CopiaSeguranca(database, bkpname, bkpdescription, dir, f)
        Return
    End Sub
    Public Sub reposicao_copia_seguranca(database As String, file As String)
        init()
        'Common.WriteToConsoleOut(Convert.ToString((Convert.ToString("Reposicao de copia seguranca na base dados: ") & database) + "; com ficheiro: ") & file)

        adm.BasesDados.ReposicaoCompletaCopiaSeguranca(database, file)
        Return
    End Sub
    Public Sub lista_backups()
        init()
        Dim backupsdir__1 As String = Backupsdir
        If backupsdir__1 Is Nothing Then
            backupsdir__1 = adm.SQLServer.DirectoriaBackup()
        End If

        'Common.WriteToConsoleOut( "backupsdir: " + backupsdir );

        Dim dirInfo As New DirectoryInfo(backupsdir__1)
        Dim filenames As FileInfo() = dirInfo.GetFiles("*.*")

        ' sort file names
        Array.Sort(filenames, Function(a, b) DateTime.Compare(b.LastWriteTime, a.LastWriteTime))
        For Each fi As FileInfo In filenames
            ' TODO read backup file for get more info
            'read_backupfile(fi);
            'Common.WriteToConsoleOut("{0};{1};{2};{3};{4}", fi.Name, fi.CreationTime, fi.LastWriteTime, fi.Length, fi.FullName)
        Next
        Return
    End Sub
    Public Sub lista_basesdados()
        init()
        Dim abds As AdmBEBasesDados = adm.BasesDados.ListaBasesDados()
        For Each bd As AdmBEBaseDados In abds
            'Common.WriteToConsoleOut("name: " + bd.get_Nome())
        Next
        Return
    End Sub
    Public Sub config_backups()
        init()
        Dim bkpdir As String = adm.SQLServer.DirectoriaBackup()

        'Common.WriteToConsoleOut(Convert.ToString("DirectoriaBackup: ") & bkpdir)
        Return
    End Sub
    Public Sub lista_planos_copiaseguranca()
        init()
        Dim lista As AdmBEPlanosCopiasSeg = adm.PlanosCopiasSeguranca.ListaPlanos()
        For Each pl As AdmBEPlanoCopiasSeg In lista
            Dim id As String = pl.Id()
            'Common.WriteToConsoleOut(Convert.ToString("PlanoCopiasSeg_id: ") & id)

            Dim xmlPlano As String = pl.Plano()
            'Common.WriteToConsoleOut(" xml: " + xmlPlano);

            Dim xmlreader__1 As XmlReader = XmlReader.Create(New StringReader(xmlPlano))

            'xmlreader.Read();
            xmlreader__1.ReadToFollowing("backupPlan")
            'Common.WriteToConsoleOut(" id: " + xmlreader__1.GetAttribute("id"))
            'Common.WriteToConsoleOut(" name: " + xmlreader__1.GetAttribute("name"))
            'Common.WriteToConsoleOut(" verify: " + xmlreader__1.GetAttribute("verify"))
            'Common.WriteToConsoleOut(" incremental: " + xmlreader__1.GetAttribute("incremental"))
            'Common.WriteToConsoleOut(" overwrite: " + xmlreader__1.GetAttribute("overwrite"))
            'Common.WriteToConsoleOut(" destination: " + xmlreader__1.GetAttribute("destination"))
            ''Common.WriteToConsoleOut(" schedule: " + xmlreader.GetAttribute("schedule"));
            'Common.WriteToConsoleOut(" date: " + xmlreader__1.GetAttribute("date"))
            'Common.WriteToConsoleOut(" lastExecution: " + xmlreader__1.GetAttribute("lastExecution"))
            'Common.WriteToConsoleOut(" nextExecution: " + xmlreader__1.GetAttribute("nextExecution"))

            'Dim schedule_id As String = xmlreader__1.GetAttribute("schedule")
            'Common.WriteToConsoleOut(Convert.ToString(" schedule id: ") & schedule_id)
            'Dim pcal As AdmBECalendario = adm.Calendario.Edita(schedule_id)
            ''Common.WriteToConsoleOut(" schedule_id: " + pcal.Id );
            'Common.WriteToConsoleOut("  schedule_periodo: " + pcal.get_Periodo().ToString())

            'xmlreader__1.ReadToFollowing("companies")
            'While xmlreader__1.ReadToFollowing("company")
            '    xmlreader__1.ReadToFollowing("properties")
            '    Common.WriteToConsoleOut(" company_key: " + xmlreader__1.GetAttribute("key"))
            '    Common.WriteToConsoleOut(" company_name: " + xmlreader__1.GetAttribute("name"))
            'End While
        Next
    End Sub
    Public Sub insere_plano_copiaseguranca(name As String, verify As String, incremental As String, overwrite As String, companiesByComma As String, periodo As String)
        init()
        Dim newid As String = System.Guid.NewGuid().ToString()

        Dim newPC As New AdmBEPlanoCopiasSeg()
        Dim objCal As New AdmBECalendario()

        'newPC.Id(newid)
        'objCal.Id = newid

        'If periodo.Equals("mensal") Then
        '    objCal.Periodo(EnumPeriodoExecucao.prMensal)
        'ElseIf periodo.Equals("semanal") Then
        '    objCal.set_Periodo(EnumPeriodoExecucao.prSemanal)
        'Else
        '    objCal.set_Periodo(EnumPeriodoExecucao.prDiario)
        'End If

        '' Exec 23h (TODO change this by arg)
        'objCal.set_FreqUnicaHora(New DateTime(1900, 1, 1, 23, 0, 0))

        adm.Calendario.Actualiza(objCal)

        Dim stringwriter As New StringWriter()

        Dim xmlsettings As New XmlWriterSettings()
        xmlsettings.OmitXmlDeclaration = True
        xmlsettings.Indent = False
        Dim xmlwriter__1 As XmlWriter = XmlWriter.Create(stringwriter, xmlsettings)

        xmlwriter__1.WriteStartElement("backupPlan")
        xmlwriter__1.WriteAttributeString("id", "{" + newPC.Id() + "}")

        xmlwriter__1.WriteAttributeString("name", name)
        xmlwriter__1.WriteAttributeString("verify", verify)
        xmlwriter__1.WriteAttributeString("incremental", incremental)
        xmlwriter__1.WriteAttributeString("overwrite", overwrite)

        Dim backupsdir__2 As String = Backupsdir
        If backupsdir__2 Is Nothing Then
            backupsdir__2 = adm.SQLServer.DirectoriaBackup()
        End If

        xmlwriter__1.WriteAttributeString("destination", backupsdir__2)
        xmlwriter__1.WriteAttributeString("schedule", "{" + objCal.Id + "}")

        'Common.WriteToConsoleOut(" date: " + xmlreader.GetAttribute("date"));
        Dim datenow As DateTime = DateTime.Now
        xmlwriter__1.WriteAttributeString("date", datenow.ToString("dd-MM-yyyy HH:mm:ss"))

        'Common.WriteToConsoleOut(" lastExecution: " + xmlreader.GetAttribute("lastExecution"));
        Dim lastdate As DateTime = objCal.UltimaOcorrencia
        xmlwriter__1.WriteAttributeString("lastExecution", lastdate.ToString("dd-MM-yyyy HH:mm:ss"))

        'Common.WriteToConsoleOut(" nextExecution: " + xmlreader.GetAttribute("nextExecution"));
        'DateTime nextdate = new DateTime(datenow.Year,datenow.Month,datenow.Day);
        Dim nextdate As DateTime = objCal.ProximaOcorrencia
        xmlwriter__1.WriteAttributeString("nextExecution", nextdate.ToString("dd-MM-yyyy HH:mm:ss"))

        ' companies
        xmlwriter__1.WriteStartElement("companies")

        'string companiesByComma = "DEMO,PRIDEMO;DEMOX,PRIDEMOX";
        Dim companies As String() = companiesByComma.Split(New Char() {";"c})

        For Each company As String In companies
            Dim cfields As String() = company.Split(New Char() {","c})
            If cfields.Length = 2 Then
                xmlwriter__1.WriteStartElement("company")

                xmlwriter__1.WriteStartElement("properties")
                xmlwriter__1.WriteAttributeString("key", cfields(0))
                xmlwriter__1.WriteAttributeString("name", cfields(1))
                xmlwriter__1.WriteEndElement()
                ' properties
                ' company
                xmlwriter__1.WriteEndElement()
            End If
        Next

        xmlwriter__1.WriteEndElement()
        ' companies
        xmlwriter__1.WriteEndElement()
        ' backupPlan
        xmlwriter__1.Flush()

        'Common.WriteToConsoleOut("xml string: " + stringwriter.ToString());

        'string strBackupPlan = "<backupPlan id=\"" + newpc_id + "\" name=\"teste all\" verify=\"False\" incremental=\"False\" overwrite=\"False\" destination=\"C:\\PROGRAM FILES\\MICROSOFT SQL SERVER\\MSSQL10.PRIMAVERA\\MSSQL\\BACKUP\\\" schedule=\"" + newpc_id + "\" date=\"" + DateTime.Now.ToString() + "\" lastExecution=\"undefined\" nextExecution=\"" + DateTime.Now.ToString("dd-MM-yyyy") + " 23:00:00\"><companies><company><properties key=\"OBIADM\" name=\"BIADM\"/></company><company><properties key=\"EDEMO\" name=\"PRIDEMO\"/></company><company><properties key=\"EDEMOX\" name=\"PRIDEMOX\"/></company><company><properties key=\"OPRIEMPRE\" name=\"PRIEMPRE\"/></company></companies></backupPlan>";
        'newPC.set_Plano(stringwriter.ToString())

        'adm.PlanosCopiasSeguranca.Actualiza(newPC)
        'adm.PlanosCopiasSeguranca.ListaPlanos().Insere(newPC)

        'Common.WriteToConsoleOut(" Plano de Copia Seguranca inserido com id: " + newPC.get_Id())
    End Sub
    Public Sub remove_plano_copiaseguranca(id As String)
        init()
        adm.Calendario.Remove(id)
        adm.PlanosCopiasSeguranca.Remove(id)

        'Common.WriteToConsoleOut((Convert.ToString(" Plano de Copia Seguranca com id: ") & id) + " removido.")
    End Sub

    Public Function lista_empresas(Optional categoria As String = "") As List(Of Empresa)
        init()
        Dim lista As List(Of Empresa) = New List(Of Empresa)

        Dim empresas As AdmBEEmpresas = adm.Empresas.ListaEmpresas(True)
        For Each e As AdmBEEmpresa In empresas
            Dim empresa = New Empresa
            With (empresa)
                .codigo = e.Identificador
                .nome = e.IDNome
                .nuit = e.IFNIF
            End With
            lista.Add(empresa)
            'Common.WriteToConsoleOut("name: " + e.get_Identificador() + " description: " + e.get_IDNome())
        Next
        Return lista
    End Function
    Public Sub lista_utilizadores()
        init()
        Dim uList As StdBELista = adm.Consulta("SELECT * FROM utilizadores")

        uList.Inicio()
        While Not uList.NoFim()

            Dim idioma As CultureInfo = CultureInfo.GetCultureInfo(uList.Valor("Idioma"))

            'Common.WriteToConsoleOut("Utilizador: " + uList.Valor("Codigo"))
            'Common.WriteToConsoleOut(" Codigo: " + uList.Valor("Codigo"))
            'Common.WriteToConsoleOut(" Nome: " + uList.Valor("Nome"))
            'Common.WriteToConsoleOut(" Email: " + uList.Valor("Email"))
            'Common.WriteToConsoleOut(" Activo: " + uList.Valor("Activo"))
            'Common.WriteToConsoleOut(" Administrador: " + uList.Valor("Administrador"))
            'Common.WriteToConsoleOut(" PerfilSugerido: " + uList.Valor("PerfilSugerido"))
            'Common.WriteToConsoleOut(" NaoPodeAlterarPwd: " + uList.Valor("NaoPodeAlterarPwd"))
            'Common.WriteToConsoleOut(" Idioma: " + idioma)
            'Common.WriteToConsoleOut(" LoginWindows: " + uList.Valor("LoginWindows"))
            'Common.WriteToConsoleOut(" Telemovel: " + uList.Valor("Telemovel"))
            'Common.WriteToConsoleOut(" Bloqueado: " + uList.Valor("Bloqueado"))
            'Common.WriteToConsoleOut(" TentativasFalhadas: " + uList.Valor("TentativasFalhadas"))
            'Common.WriteToConsoleOut(" AutenticacaoPersonalizada: " + uList.Valor("AutenticacaoPersonalizada"))
            'Common.WriteToConsoleOut(" SuperAdministrador: " + uList.Valor("SuperAdministrador"))
            'Common.WriteToConsoleOut(" Tecnico: " + uList.Valor("Tecnico"))

            uList.Seguinte()
        End While
        Return
    End Sub
    Public Sub lista_perfis()
        init()
        Dim pList As StdBELista = adm.Consulta("SELECT * FROM perfis")

        pList.Inicio()
        While Not pList.NoFim()
            'Common.WriteToConsoleOut("Perfil: " + pList.Valor("Codigo"))
            'Common.WriteToConsoleOut(" Codigo: " + pList.Valor("Codigo"))
            'Common.WriteToConsoleOut(" Nome: " + pList.Valor("Nome"))

            pList.Seguinte()
        End While
        Return
    End Sub
    Public Sub lista_aplicacoes()
        init()

        Dim rk_LM As RegistryKey = Registry.LocalMachine

        Dim s_basepath As String = "SOFTWARE\PRIMAVERA\SGE900"
        If Type.Equals("Executive") Then
            s_basepath = "SOFTWARE\PRIMAVERA\SGE900"
        Else
            s_basepath = "SOFTWARE\PRIMAVERA\SGP900"
        End If
        Dim rk_PrimaveraDefault As RegistryKey = rk_LM.OpenSubKey(s_basepath & Convert.ToString("\DEFAULT"))
        Dim subkeys As String() = rk_PrimaveraDefault.GetSubKeyNames()
        For Each key As String In subkeys
            Dim rk_App As RegistryKey = rk_PrimaveraDefault.OpenSubKey(key)
            If rk_App IsNot Nothing Then
                Dim nome As String = DirectCast(rk_App.GetValue("NOME"), String)
                If (nome IsNot Nothing) AndAlso (key.Length = 3) Then
                    Dim versao As String = DirectCast(rk_App.GetValue("VERSAO"), String)
                    'Common.WriteToConsoleOut(Convert.ToString("Aplicacao: ") & key)
                    'Common.WriteToConsoleOut(Convert.ToString(" Codigo: ") & key)
                    'Common.WriteToConsoleOut(Convert.ToString(" Nome: ") & nome)
                    'Common.WriteToConsoleOut(Convert.ToString(" Versao: ") & versao)
                End If
            End If
        Next
        Return
    End Sub
    Public Sub lista_utilizador_aplicacoes(user As String)
        init()
        Dim uaList As StdBELista = adm.Consulta((Convert.ToString("SELECT * FROM UtilizadoresAplicacoes WHERE Utilizador='") & user) + "'")

        uaList.Inicio()
        While Not uaList.NoFim()
            'Common.WriteToConsoleOut("Aplicacao: " + uaList.Valor("Apl"))
            uaList.Seguinte()
        End While
        Return
    End Sub
    Public Sub insere_utilizador_aplicacao(user As String, apl As String)
        init()
        Dim sqlInsereUtilizadorAplicacao As String = (Convert.ToString((Convert.ToString("INSERT [UtilizadoresAplicacoes] ([Utilizador], [Apl]) VALUES (N'") & user) + "',N'") & apl) + "')"
        adm.SQLServer.ExecutaComando(sqlInsereUtilizadorAplicacao, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString((Convert.ToString("Insert utilizador '") & user) + "' applicacao '") & apl) + "' ok.")
        Return
    End Sub
    Public Sub remove_utilizador_aplicacao(user As String, apl As String)
        init()
        Dim sqlRemoveUtilizadorAplicacao As String = (Convert.ToString((Convert.ToString("DELETE [UtilizadoresAplicacoes] WHERE [Utilizador] = '") & user) + "' AND [Apl] = '") & apl) + "'"
        adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorAplicacao, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString((Convert.ToString("Delete utilizador '") & user) + "' applicacao '") & apl) + "' ok.")
        Return
    End Sub
    Public Function actualiza_utilizador_aplicacoes(user As String, aplicacoes As String()) As Boolean
        init()

        adm.IniciaTransaccao()
        Try
            Dim sqlRemoveUtilizadorAplicacoes As String = (Convert.ToString("DELETE [UtilizadoresAplicacoes] WHERE [Utilizador] = '") & user) + "'"
            adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorAplicacoes, "PRIEMPRE", False)

            For Each apl As String In aplicacoes
                Dim sqlInsereUtilizadorAplicacao As String = (Convert.ToString((Convert.ToString("INSERT [UtilizadoresAplicacoes] ([Utilizador], [Apl]) VALUES (N'") & user) + "',N'") & apl) + "')"
                adm.SQLServer.ExecutaComando(sqlInsereUtilizadorAplicacao, "PRIEMPRE", False)
            Next
        Catch e As Exception
            adm.DesfazTransaccao()
            'Common.WriteToConsoleOut((Convert.ToString("Actualiza applicacoes do utilizador '") & user) + "' falhou: {0} Exception caught.", e)
            Return False
        End Try
        adm.TerminaTransaccao()

        'Common.WriteToConsoleOut((Convert.ToString("Actualiza applicacoes do utilizador '") & user) + "' ok.")
        Return True
    End Function
    Public Sub lista_utilizador_permissoes(user As String)
        init()
        Dim upList As StdBELista = adm.Consulta((Convert.ToString("SELECT * FROM Permissoes WHERE Utilizador='") & user) + "'")

        upList.Inicio()
        While Not upList.NoFim()
            'Common.WriteToConsoleOut("Permissao: ")
            'Common.WriteToConsoleOut(" Perfil: " + upList.Valor("Perfil"))
            'Common.WriteToConsoleOut(" Empresa: " + upList.Valor("Empresa"))
            upList.Seguinte()
        End While
        Return
    End Sub
    Public Sub insere_utilizador_permissao(user As String, perfil As String, empresa As String)
        init()
        Dim sqlInsereUtilizadorPermissao As String = (Convert.ToString((Convert.ToString((Convert.ToString("INSERT [Permissoes] ([Utilizador], [Perfil], [Empresa]) VALUES (N'") & user) + "',N'") & perfil) + "',N'") & empresa) + "')"
        adm.SQLServer.ExecutaComando(sqlInsereUtilizadorPermissao, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString((Convert.ToString((Convert.ToString("Insert utilizador '") & user) + "' permissao do perfil '") & perfil) + "' empresa '") & empresa) + "' ok.")
        Return
    End Sub
    Public Sub remove_utilizador_permissao(user As String, perfil As String, empresa As String)
        init()
        Dim sqlRemoveUtilizadorPermissao As String = (Convert.ToString((Convert.ToString((Convert.ToString("DELETE [Permissoes] WHERE [Utilizador] = '") & user) + "' AND [Perfil] = '") & perfil) + "' AND [Empresa] = '") & empresa) + "'"
        adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorPermissao, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString((Convert.ToString((Convert.ToString("Delete utilizador '") & user) + "' permissao do perfil '") & perfil) + "' empresa '") & empresa) + "' ok.")
        Return
    End Sub
    Public Function actualiza_utilizador_permissoes(user As String, permissoes As String()()) As Boolean
        init()

        adm.IniciaTransaccao()
        Try
            Dim sqlRemoveUtilizadorPermissoes As String = (Convert.ToString("DELETE [Permissoes] WHERE [Utilizador] = '") & user) + "'"
            adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorPermissoes, "PRIEMPRE", False)


            For i As Integer = 0 To permissoes.Length - 1
                If permissoes(i).Length = 2 Then
                    Dim perfil As String = permissoes(i)(0)
                    Dim empresa As String = permissoes(i)(1)
                    Dim sqlInsereUtilizadorPermissao As String = (Convert.ToString((Convert.ToString((Convert.ToString("INSERT [Permissoes] ([Utilizador], [Perfil], [Empresa]) VALUES (N'") & user) + "',N'") & perfil) + "',N'") & empresa) + "')"
                    adm.SQLServer.ExecutaComando(sqlInsereUtilizadorPermissao, "PRIEMPRE", False)
                End If
            Next
        Catch e As Exception
            adm.DesfazTransaccao()
            'Common.WriteToConsoleOut((Convert.ToString("Actualiza permissoes do utilizador '") & user) + "' falhou: {0} Exception caught.", e)
            Return False
        End Try
        adm.TerminaTransaccao()

        'Common.WriteToConsoleOut((Convert.ToString("Actualiza permissoes do utilizador '") & user) + "' ok.")
        Return True
    End Function
    Public Sub insere_utilizador(codigo As String, nome As String, email As String, password As String, activo As String, administrador As String, _
        perfilSugerido As String, naoPodeAlterarPwd As String, idioma As String, loginWindows As String, telemovel As String, bloqueado As String, _
        tentativasFalhadas As String, autenticacaoPersonalizada As String, superAdministrador As String, tecnico As String)
        init()
        Dim sqlInsereUtilizador As String = (Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString("INSERT [Utilizadores] ([Codigo], [Nome], [Email], [Password], [Activo], [Administrador], [PerfilSugerido], [NaoPodeAlterarPwd], [Idioma], [LoginWindows], [Telemovel], [Bloqueado], [TentativasFalhadas], [AutenticacaoPersonalizada], [SuperAdministrador], [Tecnico]) VALUES (N'") & codigo) + "',N'") & nome) + "',N'") & email) + "',N'") & password) + "',") & activo) + ",") & administrador) + ",N'") & perfilSugerido) + "',") & naoPodeAlterarPwd) + ",") & idioma) + ",N'") & loginWindows) + "',N'") & telemovel) + "',") & bloqueado) + ",") & tentativasFalhadas) + ",") & autenticacaoPersonalizada) + ",") & superAdministrador) + ",") & tecnico) + ")"
        adm.SQLServer.ExecutaComando(sqlInsereUtilizador, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString("Insert user '") & codigo) + "' ok.")
    End Sub
    Public Sub actualiza_utilizador(codigo As String, nome As String, email As String, password As String, activo As String, administrador As String, _
        perfilSugerido As String, naoPodeAlterarPwd As String, idioma As String, loginWindows As String, telemovel As String, bloqueado As String, _
        tentativasFalhadas As String, autenticacaoPersonalizada As String, superAdministrador As String, tecnico As String)
        init()
        Dim sqlActualizaUtilizador As String = Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString("UPDATE [Utilizadores] SET [Nome] = '") & nome) + "', [Email] = '") & email) + "', [Activo] = ") & activo) + ", [Administrador] = ") & administrador) + ", [PerfilSugerido] = '") & perfilSugerido) + "', [NaoPodeAlterarPwd] = ") & naoPodeAlterarPwd) + ", [Idioma] = ") & idioma) + ", [LoginWindows] = '") & loginWindows) + "', [Telemovel] = '") & telemovel) + "', [Bloqueado] = ") & bloqueado) + ", [TentativasFalhadas] = ") & tentativasFalhadas) + ", [AutenticacaoPersonalizada] = ") & autenticacaoPersonalizada) + ", [SuperAdministrador] = ") & superAdministrador) + ", [Tecnico] = ") & tecnico
        If password.Length > 0 Then
            sqlActualizaUtilizador = (Convert.ToString(sqlActualizaUtilizador & Convert.ToString(", [Password] = '")) & password) + "'"
        End If
        sqlActualizaUtilizador = (Convert.ToString(sqlActualizaUtilizador & Convert.ToString(" WHERE [Codigo] = '")) & codigo) + "'"

        'Common.WriteToConsoleOut(sqlActualizaUtilizador);

        adm.SQLServer.ExecutaComando(sqlActualizaUtilizador, "PRIEMPRE", False)
        'Common.WriteToConsoleOut((Convert.ToString("Update user '") & codigo) + "' ok.")
    End Sub
    Public Function remove_utilizador(codigo As String) As Boolean
        init()

        adm.IniciaTransaccao()
        Try
            Dim sqlRemoveUtilizadorAplicacoes As String = (Convert.ToString("DELETE [UtilizadoresAplicacoes] WHERE [Utilizador] = '") & codigo) + "'"
            adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorAplicacoes, "PRIEMPRE", False)

            Dim sqlRemoveUtilizadorPermissao As String = (Convert.ToString("DELETE [Permissoes] WHERE [Utilizador] = '") & codigo) + "'"
            adm.SQLServer.ExecutaComando(sqlRemoveUtilizadorPermissao, "PRIEMPRE", False)

            Dim sqlRemoveUtilizador As String = (Convert.ToString("DELETE [Utilizadores] WHERE [Codigo] = '") & codigo) + "'"
            adm.SQLServer.ExecutaComando(sqlRemoveUtilizador, "PRIEMPRE", False)
        Catch e As Exception
            adm.DesfazTransaccao()
            'Common.WriteToConsoleOut((Convert.ToString("Actualiza applicacoes do utilizador '") & codigo) + "' falhou: {0} Exception caught.", e)
            Return False
        End Try
        adm.TerminaTransaccao()
        'Common.WriteToConsoleOut((Convert.ToString("Delete user '") & codigo) + "' ok.")

        Return True
    End Function
    Public Sub info()
        init()
        Dim motor As New ErpBS()

        Dim _false As Boolean = False

        'Common.WriteToConsoleOut("License: " + Not motor.Licenca.VersaoDemo)
        'Common.WriteToConsoleOut("Language: " + adm.Params.get_Idioma())
        'Common.WriteToConsoleOut("Seguranca Activa: " + adm.Params.get_SegurancaActiva())
        'Common.WriteToConsoleOut("Seguranca Pro Emp Activa: " + adm.Params.get_SegurancaPorEmpActiva())
        'Common.WriteToConsoleOut("Modo Seguranca: " + adm.Params.get_SegurancaActiva())
        'Common.WriteToConsoleOut("N Postos: " + adm.Postos.ListaPostos(_false).NumItens)

        Dim backupsdir__1 As String = Backupsdir
        If backupsdir__1 Is Nothing Then
            backupsdir__1 = adm.SQLServer.DirectoriaBackup()
        End If

        'Common.WriteToConsoleOut(Convert.ToString("DirectoriaBackup: ") & backupsdir__1)

        Dim uList As StdBELista = adm.Consulta("SELECT * FROM utilizadores")
        'Common.WriteToConsoleOut("N Utilizadores: " + uList.NumLinhas())

        uList.Inicio()
        While Not uList.NoFim()
            'Common.WriteToConsoleOut(" Utilizador: " + uList.Valor("Codigo") + ", " + uList.Valor("Nome"))
            uList.Seguinte()
        End While

        Dim eList As StdBELista = adm.Consulta("SELECT * FROM empresas")
        'Common.WriteToConsoleOut("N Empresas: " + eList.NumLinhas())

        eList.Inicio()
        While Not eList.NoFim()
            'Common.WriteToConsoleOut(" Empresa: " + eList.Valor("Codigo") + ", " + eList.Valor("IDNome"))
            eList.Seguinte()
        End While
        Return
    End Sub
End Class

Public Class Instancia
    Public instancia As Integer = 1

    Public empresa As String
    Public usuario As String
    Public password As String
    Public instanciaSql As String
    Public empresaSql As String
    Public usuarioSql As String
    Public passwordSql As String
    Public Shared versaoErp As String = "V900"
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

Public Class PrimaveraResultStructure

    ''' <summary>
    ''' 0 - Sucesso Completo
    ''' 1 - Sucesso Com Avisos
    ''' 2 - Erro
    ''' </summary>
    ''' <remarks></remarks>
    Public codigo As Integer

    Public tipoProblema As String
    Public codeLevel As String
    Public subNivel As String
    Public descricao As String
    Public pasta

    Public Function getResultToString()
        Return " , " + codigo + " , " + codeLevel + " , " + tipoProblema + "," + descricao
    End Function

End Class

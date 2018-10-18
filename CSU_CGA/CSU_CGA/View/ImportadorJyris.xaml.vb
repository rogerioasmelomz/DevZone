Imports System.IO
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Threading
Imports MahApps.Metro.Controls
Imports System.Reflection
Imports System.Text
Imports System.Windows.Threading

Public Class ImportadorJyris : Inherits MetroWindow
    Public listFiles As ObservableCollection(Of Ficheiro)
    Public jury_controller As Jvris_Controller
    Dim xmlHelper As XmlHelper

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        Me.Show()

        inicializar()
        updateTreeView()

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

        assemblyName = New System.Reflection.AssemblyName(args.Name)

        If Instancia.versaoErp = "V900" Then
            Const PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.pastaConfigV900 ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
            assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll")
        Else
            Const PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.pastaConfigV800
            assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll")
        End If

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



        Dim outAssembly, objExeAssembly As Assembly
        Dim strTempAssemblyPath As String = ""
        Dim strArgToLoad As String

        objExeAssembly = Assembly.GetExecutingAssembly
        Dim arrRefAssemblyNames() As AssemblyName = objExeAssembly.GetReferencedAssemblies

        strArgToLoad = args.Name.Substring(0, args.Name.IndexOf(","))

        For Each strAName As AssemblyName In arrRefAssemblyNames

            If strAName.FullName.Substring(0, strAName.FullName.IndexOf(",")) = strArgToLoad Then

                If Instancia.versaoErp = "V900" Then
                    Const PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.pastaConfigV900 ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
                    strTempAssemblyPath = System.IO.Path.Combine(
                   System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER),
                   strArgToLoad + ".dll")
                Else
                    Const PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.pastaConfigV800
                    strTempAssemblyPath = System.IO.Path.Combine(
                   System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER),
                   strArgToLoad + ".dll")
                End If



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

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)




    End Sub

    Public Sub inicializar()

        Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-PT")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("pt-PT")

        dtFim.SelectedDate = Today
        dtInicio.SelectedDate = New Date(Today.Year, Today.Month, 1)


        listFiles = New ObservableCollection(Of Ficheiro)()
        ' Add any initialization after the InitializeComponent() call.
        jury_controller = New Jvris_Controller()
        'jury_controller.AbreEmpresaPrimavera(1, "cga", "accsys", "accsys2011")

        xmlHelper = New XmlHelper
    End Sub

    Private Sub updateTreeView()
        Try
            fillTreewView("Por Importar")
            fillTreewView("Importado")
            fillTreewView("Cancelados")
            fillTreewView("Erro de Importação")

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub fillTreewView(tipocaminho As String)

        Dim caminho As String = ""

        Select Case tipocaminho
            Case "Por Importar"
                caminho = xmlHelper.daPasta("Por Importar")
                IIf(caminho <> "", caminho, "C:\jvris_primavera\Por Importar")

            Case "Importado"
                caminho = xmlHelper.daPasta("Importado")
                IIf(caminho <> "", caminho, "C:\jvris_primavera\Importado")

            Case "Cancelados"
                caminho = xmlHelper.daPasta("Cancelados")
                IIf(caminho <> "", caminho, "C:\jvris_primavera\Cancelados")

            Case "Erro de Importação"
                caminho = xmlHelper.daPasta("Erro de Importação")
                IIf(caminho <> "", caminho, "C:\jvris_primavera\Erro de Importação")

        End Select

        Dim FileNm As DirectoryInfo = New DirectoryInfo(caminho)
        Dim filename = FileNm.GetFiles()

        Dim lista = CType(Me.Resources("ListFicheiros"), Ficheiro_Controller)

        Dim items = treeviewListaFicheiros.Items

        For Each item As TreeViewItem In items
            If (item.Header = tipocaminho) Then
                item.Items.Clear()
                Dim itemNovo As TreeViewItem

                For Each f As FileInfo In filename
                    itemNovo = New TreeViewItem()
                    itemNovo.Header = f.ToString()

                    item.Items.Add(itemNovo)
                    'lista.Add(
                    '    New Ficheiro(True, f.ToString(), f)
                    ')

                Next
            End If
        Next




    End Sub

    Private Sub treeViewItem_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Try
            Dim tvi As TreeViewItem = sender
            Dim items = tvi.Items

            For Each item As TreeViewItem In items
                If (item.IsSelected = True) Then
                    FicheiroSelecionado(tvi.Header, item.Header)
                    Exit For
                End If
            Next
            e.Handled = True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub FicheiroSelecionado(tipocaminho As String, nomeficheiro As String)
        Try
            'Dim lista = CType(Me.Resources("ListFicheiros"), Ficheiro_Controller)
            'Dim objficheiro = lista(listViewFolder.SelectedIndex)

            Dim caminho As String = xmlHelper.daPasta(tipocaminho)
            IIf(caminho <> "", caminho, "C:\jvris_primavera\Importado")

            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(caminho + "\" + nomeficheiro, Text.Encoding.GetEncoding("ISO-8859-1"))

            richtxtTexto.Document.Blocks.Clear()
            richtxtTexto.Document.Blocks.Add(New Paragraph(New Run(fileReader.ReadToEnd())))

            'txtTexto.Text = fileReader.ReadToEnd()
            Dim stringReader As String

            stringReader = fileReader.ReadLine()

            Dim palavras As String()

            Dim i As Integer = 1

            While stringReader <> ""
                palavras = Split(stringReader, ";")

                stringReader = fileReader.ReadLine()
            End While

            fileReader.Close()
        Catch ex As Exception

        End Try

    End Sub

    Delegate Sub Load1()
    Sub Load2()
        'System.Threading.Thread.Sleep(New TimeSpan(0, 0, 3))

    End Sub
    Dim Load3 As Load1 = AddressOf Load2

    Private Sub btSave_Click(sender As Object, e As RoutedEventArgs)
        IsProgressBarVisible = True

        Cursor = Cursors.Wait
        'TextBlockStatus.Text = "Loading..."
        'TextBlockStatus.UpdateLayout(); //include the update before calling dispatcher
        Dispatcher.Invoke(DispatcherPriority.Background, Load3)
        'TextBlockStatus.Text = String.Empty


        Try

            Dim items = treeviewListaFicheiros.Items

            For Each item As TreeViewItem In items
                If (item.Header = "Por Importar") Then
                    If item.IsSelected = True Then
                        For Each ficheiroSelecionado As TreeViewItem In item.Items
                            jury_controller.Integracao_Primavera(ficheiroSelecionado.Header, "Por Importar")

                        Next
                    Else
                        For Each ficheiroSelecionado As TreeViewItem In item.Items
                            If (ficheiroSelecionado.IsSelected = True) Then
                                jury_controller.Integracao_Primavera(ficheiroSelecionado.Header, "Por Importar")
                            End If
                        Next
                    End If
                End If

            Next
            e.Handled = True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Cursor = Cursors.Arrow
    End Sub



    Public IsProgressBarVisible As Boolean = False
    Public ProgressValue As Double

    Private Sub btupdate_Click(sender As Object, e As RoutedEventArgs)
        updateTreeView()
    End Sub

    Private Sub btRemove_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class

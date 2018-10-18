Imports System.IO
Imports System.Collections.ObjectModel
Imports CSU_CGA
Imports System.Globalization
Imports System.Threading

Class MainWindow
    Public listFiles As ObservableCollection(Of Ficheiro)
    Public jury_controller As Jvris_Controller
    Dim xmlHelper As XmlHelper = New XmlHelper

    Public Sub New()
        
        
        ' This call is required by the designer.
        InitializeComponent()
        Dim imp As ImportadorJyris
        imp = New ImportadorJyris
        imp.Show()

        xmlHelper.loadFolder()
        imp.jury_controller.AbreEmpresaPrimavera(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa,
                                                 xmlHelper.instancia.usuario, xmlHelper.instancia.password)
        imp.jury_controller = New Jvris_Controller
        Hide()

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

        Dim a As XmlHelper
        a = New XmlHelper
        a.loadFolder()

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        'Dim lista = CType(Me.Resources("ListFicheiros"), Ficheiro_Controller)

        'If lista.Count > 0 Then
        '    For Each fich As Ficheiro In lista

        '        If fich.Sel = "True" Then
        '            jury_controller.Integracao_Primavera(fich.FicheiroSel)
        '        End If


        '    Next
        '    actualizar()
        'Else
        '    MessageBox.Show("Não foi selecionado nenhum ficheiro")

        'End If


    End Sub

    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        actualizar()
    End Sub

    Private Sub actualizar()
        Try
            txtTexto.Text = ""
            Dim caminho As String = ""


            Dim sel As ListBoxItem = cbFicheiro.SelectedValue

            Select Case sel.Content
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

            lista.Clear()

            For Each f As FileInfo In filename
                lista.Add(
                    New Ficheiro(True, f.ToString(), f)
                )

            Next



        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub listViewFolder_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles listViewFolder.SelectionChanged
        Try
            Dim lista = CType(Me.Resources("ListFicheiros"), Ficheiro_Controller)
            Dim objficheiro = lista(listViewFolder.SelectedIndex)

            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(objficheiro.ficheiro.FullName)
            txtTexto.Text = fileReader.ReadToEnd()
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
End Class

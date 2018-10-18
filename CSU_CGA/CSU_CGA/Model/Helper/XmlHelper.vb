Imports System.IO

Imports System.Xml.Serialization
Imports System.Xml

Public Class XmlHelper
    Public filename As String
    Public instancia As Instancia
    Public passwordFicheiro As String

    Public Function EncriptaPalavra(palavra As String, chave As String) As String
        Return ""
    End Function


    Public Sub loadFolder()
        Try
            Dim doc As XmlDocument = New XmlDocument
            doc.Load(filename)

            Dim pastas As XmlNode = doc.SelectSingleNode("/Configuracao/geral/Pastas")

            For Each node As XmlNode In pastas
                If (Not System.IO.Directory.Exists(node.Attributes.GetNamedItem("folder").InnerText)) Then
                    System.IO.Directory.CreateDirectory(node.Attributes.GetNamedItem("folder").InnerText)
                End If
            Next

            'pastas = doc.SelectSingleNode("/Configuracao/geral/ficheiro ")
            'passwordFicheiro = pastas.Attributes.GetNamedItem("folder").InnerText()
            

            pastas = doc.SelectSingleNode("/Configuracao/instancias")
            instancia = New Instancia
            For Each node As XmlNode In pastas
                Try
                    instancia.instancia = Conversion.Int(node.Attributes.GetNamedItem("instancia").InnerText)

                Catch ex As Exception

                End Try
                instancia.empresa = node.Attributes.GetNamedItem("empresa").InnerText
                instancia.empresaSql = node.Attributes.GetNamedItem("empresaSql").InnerText
                instancia.instanciaSql = node.Attributes.GetNamedItem("instanciaSql").InnerText
                instancia.password = node.Attributes.GetNamedItem("password").InnerText
                instancia.passwordSql = node.Attributes.GetNamedItem("passwordSql").InnerText
                instancia.usuario = node.Attributes.GetNamedItem("usuario").InnerText
                instancia.usuarioSql = node.Attributes.GetNamedItem("usuarioSql").InnerText
                instancia.versaoErp = node.Attributes.GetNamedItem("versaoErp").InnerText
            Next

        Catch ex As Exception

        End Try

        

    End Sub

    Public Function daPasta(tipo As String) As String
        Try
            Dim doc As XmlDocument = New XmlDocument
            doc.Load(filename)

            Dim pastas As XmlNode = doc.SelectSingleNode("/Configuracao/geral/Pastas")

            For Each node As XmlNode In pastas
                If (tipo = node.Attributes.GetNamedItem("tipo").InnerText) Then
                    Return node.Attributes.GetNamedItem("folder").InnerText
                End If
                
            Next
        Catch ex As Exception
            MessageBox.Show("Pasta de configuração não encontrada: " + filename)
        End Try
        Return ""
    End Function


    Public Sub New()
        filename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "\Resources\Config.xml"
        ' My.Application.Info.DirectoryPath + "\Resources\Config.xml"
    End Sub
End Class

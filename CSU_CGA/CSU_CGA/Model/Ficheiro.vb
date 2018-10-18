Imports System.IO

Public Class Ficheiro
    Public selecionado As Boolean
    Public caminho As String
    Public ficheiro As FileInfo

    Public Property FicheiroSel() As FileInfo
        Get
            Return Me.ficheiro
        End Get
        Set(ByVal value As FileInfo)
            Me.ficheiro = value
        End Set
    End Property

    Public Sub New(selecionado As Boolean, caminho As String)
        Me.selecionado = selecionado
        Me.caminho = caminho
    End Sub

    Public Sub New(selecionado As Boolean, caminho As String, ByRef ficheiro As FileInfo)
        Me.ficheiro = ficheiro
        Me.selecionado = selecionado
        Me.caminho = caminho
    End Sub

    ' Properties 
    Public Property Sel() As Boolean
        Get
            Return Me.selecionado
        End Get
        Set(ByVal value As Boolean)
            Me.selecionado = value
        End Set
    End Property

    Public Property cam() As String
        Get
            Return Me.caminho
        End Get
        Set(ByVal value As String)
            Me.caminho = value
        End Set
    End Property
End Class
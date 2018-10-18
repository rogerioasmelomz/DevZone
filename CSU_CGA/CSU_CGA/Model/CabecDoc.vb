Imports System.Globalization

Public Class CabecDoc

    Public numDoc As String

    'Prencido com H
    Public tipoRegisto As String

    ' TipoDoc = "HON","AVE"
    Public tipoDoc As String
    Public codigoEntidade As String
    Public nomeEntidade As String
    Public moradaEntidade As String
    Public localidadeEntidade As String
    Public codigoPostaEntidade As String
    Public localidadePostalEntidade As String
    Public nifEntidade As String
    Public paisEntidade As String
    Public condicaoPag As String = "1"

    Public modPag As String = "1"

    Public codMoedaErp As String = "MT"
    Public tipoMercado As Integer = 0
    Public segmentoErp As String
    Public serie As String
    Public dataEmissao As Date
    Public dataVencimento As Date
    Public numeroDocumentoRef As String
    Public valorCambio As Decimal
    Public percentagemDescontoEntidade As Decimal
    Public percentagemDescontoFinanceiro As Decimal

    Public linhasdoc As List(Of LinhasDoc) = New List(Of LinhasDoc)

    Public documentoGerado As String

    Public Property condicoesPag() As String
        Get
            Return Me.condicaoPag
        End Get
        Set(ByVal value As String)

            If value = "" Then
                Me.condicaoPag = "1"

            Else
                Me.condicaoPag = value
            End If

        End Set
    End Property

    Public Property modoPagamento() As String
        Get
            Return Me.modPag
        End Get
        Set(ByVal value As String)

            If value = "" Then
                Me.modPag = "1"

            Else
                Me.modPag = value
            End If

        End Set
    End Property

    Public Property codigoMoeda() As String
        Get
            Return Me.codMoedaErp
        End Get
        Set(ByVal value As String)

            If value = "" Then
                Me.codMoedaErp = "MT"

            Else
                Me.modPag = value
            End If

        End Set
    End Property

    Public Property tipoMercadoErp() As String
        Get
            Return Me.tipoMercado
        End Get
        Set(ByVal value As String)

            If value = "" Then
                Me.tipoMercado = "0"

            Else
                Me.tipoMercado = value
            End If

        End Set
    End Property

    Public Property data_Emissao() As Date
        Get
            Return Me.dataEmissao
        End Get
        Set(ByVal value As Date)


        End Set
    End Property

    Public Property valor_Cambio() As Double
        Get
            Return Me.valorCambio
        End Get
        Set(ByVal value As Double)

            If value = "" Then
                Me.valorCambio = 0

            Else
                Me.valorCambio = value
            End If

        End Set
    End Property

    Public Function paraString() As String
        Return "O Relatorio " + numeroDocumentoRef + " gerou o " + tipoDoc + " - " + numDoc + "/" + serie
    End Function

End Class

Public Class LinhasDoc

    'Prenchido com L
    Public tipoRegisto As String

    Public artigo As String
    Public descricao As String

    'Tipo Servico
    Public tipoArtigo As String
    Public taxaIva As String
    Public movStock As Boolean = False
    Public sujeitoDevolucao As Boolean = False
    Public codUnidades As String = "UN"
    Public precoUnitario As Double
    Public quantidade As Double = 1
    Public descontoLinha As Decimal = 0
    Public codArmazem As String
    Public codLocalizacao As String


    Public Property taxa_Iva As Double
        Get
            Return Me.taxaIva
        End Get
        Set(ByVal value As Double)

            If value = "" Then
                Me.taxaIva = 0

            Else
                Me.taxaIva = value
            End If

        End Set
    End Property

    
End Class

Public Class PrimaveraResultStructure

    Public tipoProblema As String

    Public codigo As Integer

    Public codeLevel As String

    Public subNivel As String

    Public descricao As String

    Public Function getResultToString()
        Return " , " + codigo + " , " + codeLevel + " , " + tipoProblema + "," + descricao
    End Function

End Class


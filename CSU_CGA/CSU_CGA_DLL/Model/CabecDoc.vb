Public Class CabecDoc
    'Prencido com H e A
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
    Public modPag As String = "MT"
    Public tipoMercado As Integer = 0
    Public segmentoErp As String
    Public serie As String
    Public dataEmissao As Date
    Public dataVencimento As Date
    Public numeroDocumentoRef As String
    Public valorCambio As Decimal
    Public percentagemDescontoEntidade As Decimal
    Public percentagemDescontoFinanceiro As Decimal

    Public linhasdoc As List(Of LinhasDoc)

End Class

Public Class LinhasDoc

    'Prenchido com L
    Public tipoRegistro As String

    Public artigo As String
    Public descricao As String

    'Tipo Servico
    Public tipoArtigo As String
    Public taxaIva As String
    Public movStock As Boolean = False
    Public sujeitoDevolucao As Boolean = False
    Public codUnidades As Boolean = "UN"
    Public precoUnitario As Decimal
    Public quantidade As Double = 1
    Public descontoLinha As Decimal = 0


End Class

Public Class PrimaveraResultStructure

    Public tipoProblema As String

    Public codigo As Integer

    Public codeLevel As String

    Public subNivel As String

    Public descricao As String

End Class


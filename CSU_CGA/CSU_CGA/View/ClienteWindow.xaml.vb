Imports Interop.ErpBS800
Imports System.Data
Imports Interop.GcpBE800

Public Class ClienteWindow
    Dim objmotor As ErpBS
    Dim cliente As String
    Public Sub inicilizarComponentes(ds As ClienteDS, objmotor As ErpBS, cliente As String, nome As String)
        Me.objmotor = objmotor
        Me.cliente = cliente

        lblCliente.Content = cliente + " - " + nome

        dgClientes.ItemsSource = ds.Tables("Clientes").DefaultView
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim dv As DataView
        Dim i, j As Integer
        Dim objEntidadeAssociada As GcpBEEntidadeAssociada

        dv = dgClientes.ItemsSource

        Try
            For i = 0 To dgClientes.Items.Count - 1
                If dv.Item(i).Row("Selecionado") = "True" Then

                    If Not (objmotor.Comercial.EntidadesAssociadas.Existe("C", dv.Item(i).Row("Cliente"), "C",
                                                                          dv.Item(j).Row("Cliente"))) Then
                        objEntidadeAssociada = New GcpBEEntidadeAssociada
                        objEntidadeAssociada.TipoEntidade = "C"
                        objEntidadeAssociada.TipoEntidadeAssociada = "C"
                        objEntidadeAssociada.Entidade = dv.Item(i).Row("Cliente")
                        objEntidadeAssociada.EntidadeAssociada = cliente

                        objmotor.Comercial.EntidadesAssociadas.Actualiza(objEntidadeAssociada)
                    End If

                    For j = i + 1 To dgClientes.Items.Count - 1
                        If dv.Item(j).Row("Selecionado") = "True" Then
                            If Not (objmotor.Comercial.EntidadesAssociadas.Existe("C", dv.Item(i).Row("Cliente"), "C",
                                                                          dv.Item(j).Row("Cliente"))) Then

                                objEntidadeAssociada = New GcpBEEntidadeAssociada
                                objEntidadeAssociada.TipoEntidade = "C"
                                objEntidadeAssociada.TipoEntidadeAssociada = "C"
                                objEntidadeAssociada.Entidade = dv.Item(i).Row("Cliente")
                                objEntidadeAssociada.EntidadeAssociada = dv.Item(j).Row("Cliente")

                                objmotor.Comercial.EntidadesAssociadas.Actualiza(objEntidadeAssociada)
                            End If
                        End If
                    Next j
                End If


            Next i

        Catch
            MessageBox.Show("Erro durante a criação das entidades Associadas")
        End Try

        MessageBox.Show("Entidades Associadas Criadas com Sucesso")
    End Sub
End Class

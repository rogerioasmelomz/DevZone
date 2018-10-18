using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interop.ErpBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using Interop.RhpBE900;
using Interop.StdPlatBS900;

namespace SDEVII_TESTES
{
    class Program
    {
        static void Main(string[] args)
        {

            ErpBS myBSO=null;

            StdPlatBS myPSO;

            GcpBECliente obCliente;
            //string strNomeCliente;

            //GcpBEDocumentoVenda obVenda;
            //GcpBELinhaDocumentoVenda obLinhaVenda;

            //List<GcpBELinhaDocumentoVenda> myLinhas;

            RhpBEFuncionario obFuncionario;

            try
            {


                myBSO = new ErpBS();

                myBSO.AbreEmpresaTrabalho(EnumTipoPlataforma.tpEmpresarial, "demodevi", "demo", "chave");


                #region "Abre plataforma"

                StdBSConfApl Apl = new StdBSConfApl();
                StdBETransaccao trs = new StdBETransaccao();

                Apl.AbvtApl = "GCP";
                Apl.Instancia = "DEFAULT";
                Apl.Utilizador = "demo";
                Apl.PwdUtilizador = "chave";
                Apl.LicVersaoMinima = "9.00";


                // Inicializa a plataforma
                myPSO = new StdPlatBS();
                myPSO.AbrePlataformaEmpresa("demodevi", ref trs, ref Apl, EnumTipoPlataforma.tpEmpresarial, null);

                #endregion


                //myPSO.Dialogos.MostraAviso("Abriu a plataforma com sucesso!" );   

                //string strSql;
                //StdBELista lstFuncionarios;
                //string strCodCliente;
                //string strNovoNome;

                //strSql = "select cliente,nome from clientes";

                //strCodCliente = myPSO.Listas.GetF4SQL("A minha lista de cliente", strSql, "cliente");

                //obCliente = myBSO.Comercial.Clientes.Edita(strCodCliente );

                //double dblPlafondDiponivel;

                //dblPlafondDiponivel = obCliente.get_ValorSaldoCob();

                //myPSO.Dialogos.MostraAviso("O cliente " + strCodCliente +" tem " + dblPlafondDiponivel.ToString()+" de saldo disponivel"  );
           


                //lstFuncionarios = myBSO.Consulta(strSql);

                //while (!lstFuncionarios.NoFim()) {

                //    strCodFuncionario = lstFuncionarios.Valor("codigo"); 

                //if (myBSO.RecursosHumanos.Funcionarios.Existe(strCodCliente ))
                //{

                //    obFuncionario = myBSO.RecursosHumanos.Funcionarios.Edita(strCodCliente);
                //    strNovoNome = string.Format(" O menu novo nome {0}",strCodCliente );
                //    obFuncionario.set_Nome(strNovoNome );

                //    myBSO.RecursosHumanos.Funcionarios.Actualiza(obFuncionario);
                //}

                //    lstFuncionarios.Seguinte(); 

                //}






                //myLinhas = new List<GcpBELinhaDocumentoVenda>();

                //myLinhas.Insert(1,new GcpBELinhaDocumentoVenda());
                //myLinhas.Insert(2, new GcpBELinhaDocumentoVenda());



                //obVenda = new GcpBEDocumentoVenda();

                //obVenda.set_Tipodoc("FA");
                //obVenda.set_Serie(myBSO.Comercial.Series.DaSerieDefeito ("V","FA",DateTime.Now     )) ;
                //obVenda.set_TipoEntidade("C"); 
                //obVenda.set_DataDoc(DateTime.Now );

                //obVenda.set_Entidade("silva2");

                //obLinhaVenda = new GcpBELinhaDocumentoVenda();

                //obLinhaVenda.set_Artigo("A0001");
                //obLinhaVenda.set_Quantidade(1);
                //obLinhaVenda.set_Armazem("A1");

                //obVenda.get_Linhas().Insere(obLinhaVenda);

                //myBSO.Comercial.Vendas.AdicionaLinha(obVenda, "A0001", 1, "A1","A1", 1000, 0);



                //obVenda.get_Linhas()[obVenda.get_Linhas().NumItens ].set_Descricao("Artigo A001");

                //myBSO.Comercial.Vendas.AdicionaLinha(obVenda, "A0002", 1, "A1", "A1", 500, 0);

                //myBSO.Comercial.Vendas.PreencheDadosRelacionados(obVenda);   

                //myBSO.Comercial.Vendas.Actualiza(obVenda);

                //Console.WriteLine("Gerou o documento FA N.º {0}", obVenda.get_NumDoc());

                //strNomeCliente = myBSO.Comercial.Clientes.DaNome("silva");

                //obCliente= myBSO.Comercial.Clientes.Edita("silva");

                //obCliente.set_Nome("Maria José Silva");

                //obCliente.set_Cliente("SILVA2");

                //obCliente.set_EmModoEdicao(false); 

                //myBSO.Comercial.Clientes.Actualiza(obCliente);   

                //Console.WriteLine("Olá {0}!", strNomeCliente);





            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
            finally {

                myBSO.FechaEmpresaTrabalho(); 

                myBSO = null;
            }


        }
    }
}

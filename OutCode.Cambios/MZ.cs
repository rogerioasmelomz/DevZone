using OutCode.WebReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OutCode.Cambios
{
    public class MZ
    {
        Moeda _eur;
        Moeda _usd;
        Moeda _zar;

        Page _unico;
        Page _moza;
        Page _bim;
        Page _bci;
        Page _bmz;



        
        private List<Banco> _bancos;

        public List<Banco> Bancos
        {
            get { return _bancos; }
        }


        public delegate void EventoCambiosObtidos(string Banco,bool Result);

        public event EventoCambiosObtidos EventoCambioLido;

        public MZ() {

            _bancos = new List<Banco>();

            _bancos.Add(new Banco("BU"));
            _bancos.Add(new Banco("BCI"));
            _bancos.Add(new Banco("BIM"));
            _bancos.Add(new Banco("BMZ"));
            _bancos.Add(new Banco("MOZA"));

            _eur = new Moeda("EUR");
            _usd = new Moeda("USD");
            _zar = new Moeda("ZAR");

            _unico  = new Page("http://www.bancounico.co.mz/");
            _bci = new Page("https://www.bci.co.mz/Canais/Cotacoes.asp?Id=774");
            _bim = new Page("http://ind.millenniumbim.co.mz/pt/Paginas/Homepage.aspx");
            _moza = new Page("https://mozabanco.co.mz/pt/empresas/");
            _bmz = new Page("http://www.bancomoc.mz/");

            _unico.PageReaded += new Page.EventPageReaded(ParseBU);
            _bci.PageReaded += new Page.EventPageReaded(ParseBCI);
            _bim.PageReaded += new Page.EventPageReaded(ParseBIM);
            _moza.PageReaded += new Page.EventPageReaded(ParseMOZA);
            _bmz.PageReaded += new Page.EventPageReaded(ParseBMZ);

            _unico.ReadPage();
            _bci.ReadPage();
            _bim.ReadPage();
            _moza.ReadPage();
            _bmz.ReadPage();

        }

        private void ParseBMZ()
        {

            try
            {
                string[] eur = _bmz.GetInfo("EUR", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');
                string[] usd = _bmz.GetInfo("USD", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');
                string[] zar = _bmz.GetInfo("ZAR", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');

                _bancos[3].Add(new Moeda("EUR") { Compra = double.Parse(eur[2].Replace(".", ",")), Venda = double.Parse(eur[3].Replace(".", ",")) });
                _bancos[3].Add(new Moeda("USD") { Compra = double.Parse(usd[2].Replace(".", ",")), Venda = double.Parse(usd[3].Replace(".", ",")) });
                _bancos[3].Add(new Moeda("ZAR") { Compra = double.Parse(zar[2].Replace(".", ",")), Venda = double.Parse(zar[3].Replace(".", ",")) });

                EventoCambioLido.Invoke("BMZ", true);
            }
            catch (Exception)
            {

                EventoCambioLido.Invoke("BMZ", false);
            }


        }

        private void ParseMOZA()
        {

            try
            {
                _moza.CleanToToken("preço médio de compra diário");

                string[] eur = _moza.GetInfo("EUR", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');
                string[] usd = _moza.GetInfo("USD", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');
                string[] zar = _moza.GetInfo("ZAR", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');

                _bancos[4].Add(new Moeda("EUR") { Compra = double.Parse(eur[2].Replace(".", ",")), Venda = double.Parse(eur[3].Replace(".", ",")) });
                _bancos[4].Add(new Moeda("USD") { Compra = double.Parse(usd[2].Replace(".", ",")), Venda = double.Parse(usd[3].Replace(".", ",")) });
                _bancos[4].Add(new Moeda("ZAR") { Compra = double.Parse(zar[2].Replace(".", ",")), Venda = double.Parse(zar[3].Replace(".", ",")) });

                EventoCambioLido.Invoke("MOZA", true);
            }
            catch (Exception)
            {

                EventoCambioLido.Invoke("MOZA", false);
            }


        }

        private void ParseBIM()
        {
            string[] eur = _bim.GetInfo("EUR", 17).Replace("\r"," ").Replace("\t"," ").Split(' ');
            string[] usd = _bim.GetInfo("USD", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');
            string[] zar = _bim.GetInfo("ZAR", 17).Replace("\r", " ").Replace("\t", " ").Split(' ');

            _bancos[2].Add(new Moeda("EUR") { Compra = double.Parse(eur[2].Replace(".", ",")), Venda = double.Parse(eur[3].Replace(".", ",")) });
            _bancos[2].Add(new Moeda("USD") { Compra = double.Parse(usd[2].Replace(".", ",")), Venda = double.Parse(usd[3].Replace(".", ",")) });
            _bancos[2].Add(new Moeda("ZAR") { Compra = double.Parse(zar[2].Replace(".", ",")), Venda = double.Parse(zar[3].Replace(".", ",")) });

            EventoCambioLido.Invoke("BIM",true);
        }

        private void ParseBCI()
        {
            string[] eur = _bci.GetInfo("EUR", 19).Split(' ');
            string[] usd = _bci.GetInfo("USD", 19).Split(' ');
            string[] zar = _bci.GetInfo("ZAR", 19).Split(' ');

            _bancos[1].Add(new Moeda("EUR") { Compra = double.Parse(eur[3].Replace(".", ",")), Venda = double.Parse(eur[6].Replace(".", ",")) });
            _bancos[1].Add(new Moeda("USD") { Compra = double.Parse(usd[3].Replace(".", ",")), Venda = double.Parse(usd[6].Replace(".", ",")) });
            _bancos[1].Add(new Moeda("ZAR") { Compra = double.Parse(zar[3].Replace(".", ",")), Venda = double.Parse(zar[6].Replace(".", ",")) });

            EventoCambioLido.Invoke("BCI",true);
        }

        private void ParseBU()
        {

            string[] eur = _unico.GetInfo("EUR", 17).Split(' ') ;
            string[] usd = _unico.GetInfo("USD", 17).Split(' ');
            string[] zar = _unico.GetInfo("ZAR", 17).Split(' ');

            _bancos[0].Add(new Moeda("EUR") { Compra = double.Parse (eur[1].Replace(".",",") ), Venda = double.Parse(eur[2].Replace(".", ",")) });
            _bancos[0].Add(new Moeda("USD") { Compra = double.Parse(usd[1].Replace(".", ",")), Venda = double.Parse(usd[2].Replace(".", ",")) });
            _bancos[0].Add(new Moeda("ZAR") { Compra = double.Parse(zar[1].Replace(".", ",")), Venda = double.Parse(zar[2].Replace(".", ",")) });

            EventoCambioLido.Invoke("BU",true);
        }
    }
}

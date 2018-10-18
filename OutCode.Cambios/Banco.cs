using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Cambios
{
    public class Banco
    {
        private string  _nome;

        public string  Nome
        {
            get { return _nome; }
        }

        private List<Moeda> _moedas;

        public List<Moeda> Moedas { get { return (_moedas); }}

        public Banco(string Nome) { _nome = Nome; _moedas = new List<Moeda>(); }

        public void Add(Moeda moeda) { _moedas.Add(moeda); }

        public int Count() { return(_moedas.Count); }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Cambios
{
    public class Moeda
    {
        private string _nome;

        public string Nome
        {
            get { return _nome; }
        }

        private double _compra;

        public double Compra
        {
            get { return _compra; }
            set { _compra = value; }
        }


        private double _venda;

        public double Venda
        {
            get { return _venda; }
            set { _venda = value; }
        }

        internal Moeda(string ISO) { _nome = ISO; }
    }
}

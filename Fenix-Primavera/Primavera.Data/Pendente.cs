using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Primavera.Data
{
    public class Pendente
    {
        public string cliente { get; set; }

        public string tipoDoc { get; set; }

        public long numDoc { get; set; }

        public string serie { get; set; }

        public double valorTotal { get; set; }

        public double valorPendente { get; set; }

        public string moeda { get; set; }

        public DateTime dataCriacao { get; set; }
        
        public DateTime dataVencimento { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ErpBS900;
using Interop.StdBE900;
using OutCode.V9E.Interfaces;
using OutCode.V9E.BRModules;
using OutCode.Interop;


namespace OutCode.V9E.Core
{
    public class PriMotor {

        #region Porperties

        private ErpBS _motor;
        private GCP _vendas;

        public ErpBS MotorV9 { get { return (_motor); } }
        public GCP Logistica { get { return (_vendas); } }

        public bool Ligado {
            get {
                try{
                    return (_motor.Contexto.EmpresaAberta);
                }
                catch {
                    return (false);
                }
            }
        }

        #endregion

        #region Constructors

        public PriMotor(ErpBS Motor){
            _motor = Motor;
            _vendas  = new GCP(_motor);
        }

        public PriMotor(EnumTipoPlataforma Plataforma,string Empresa,string Utilizador,string Chave,string Instancia,IResult R) {
            try
            {
                _motor = new ErpBS();
                _motor.AbreEmpresaTrabalho(Plataforma, Empresa, Utilizador, Chave, null, Instancia,false);
                _motor.set_CacheActiva(false);
                _vendas = new GCP(_motor);
                R.SetResult(false,0,""); 
            }
            catch (Exception ex)
            {
                R.SetResult(true, ex.HResult, ex.ToString()); 
            }
        }

        #endregion

        /// <summary>
        /// Class destroyer
        /// </summary>
        ~PriMotor() {
            if (this.Ligado) _motor.FechaEmpresaTrabalho();
            _vendas = null; 
        }


    }
}

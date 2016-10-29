using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using OutCode.V9E.Core;
using OutCode.V9E.Interfaces;
using OutCode.V9E.BRModules;
using System.Configuration.Install;
using System.Reflection;
using System.Timers;

namespace OutCode.Robot
{
    public partial class SVC_Robot : ServiceBase,IResult
    {
        private int i;
        private PriMotor _motor;
        Timer RobotTimer;

        private bool _red;

        public SVC_Robot()
        {
            InitializeComponent();

        }

        /// <summary>
        /// IResult Implemetation
        /// </summary>
        /// <param name="Erro">True: Erro, False: Ok</param>
        /// <param name="ErrorNumber">Error number</param>
        /// <param name="ErrorMessage">Error Message</param>
        public void SetResult(bool Erro, int ErrorNumber, string ErrorMessage)
        {
            if (Erro) { 
                WLog(ErrorMessage, EventLogEntryType.Error ); 
            }
        }

        /// <summary>
        /// Service Start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {

            IniciaMotores();
            i = 1;
            InitializeTimer();

        }

        protected override void OnPause()
        {
            RobotTimer.Stop();
            FechaMotores();
            base.OnPause();
        }

        protected override void OnContinue()
        {
            IniciaMotores();
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            FechaMotores();
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            //_motor.
            WLog(String.Format("Serviço Parado..."), EventLogEntryType.Information);
            RobotTimer.Dispose();
        }


        private void RobotTimer_Tick(object sender, EventArgs e)
        {
            if (!_red)
            {
                _red = true;
                WLog(String.Format("Imprimir documento:w:\\{0}.pdf", i), EventLogEntryType.Information);

                _motor.Logistica.ImprimeDocumento(i, String.Format("w:\\{0}.pdf", i), this);

                _red = false;
                WLog(String.Format("Documento Impresso:w:\\{0}.pdf", i++), EventLogEntryType.Information);
            }
            else {
                WLog(String.Format("Imprimir documento em curso:w:\\{0}.pdf", i), EventLogEntryType.Information);
            }
        }

        #region Helpers

        /// <summary>
        /// Escreve Informação para o AppLog do windows
        /// </summary>
        /// <param name="Information">Informação a escrever</param>
        /// <param name="T">Tipo de mensagem</param>
        private void WLog(string Information, EventLogEntryType T)
        {
            if (T == EventLogEntryType.Error || Properties.Settings.Default.DEBUG)
            {
                Log.WriteEntry(Information, T);
            }
        }

        private void IniciaMotores() {
            try
            {
                //WLog(String.Format("A Iniciar motores..."), EventLogEntryType.Information);
                _motor = new PriMotor(global::Interop.StdBE900.EnumTipoPlataforma.tpEmpresarial, "DEMO", "rogerio", "ixp8du2011", "DEFAULT", this);
                //WLog(String.Format("Motores Iniciados Motor:{0} GCP:{0}",_motor.GetVersion(),_motor.Logistica.GetVersion()), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WLog(ex.ToString() , EventLogEntryType.Error);
            }        
                
        }

        private void FechaMotores() {
            _motor = null;
            WLog(String.Format("Motores Fechados..."), EventLogEntryType.Information);
        }

        /// <summary>
        /// Inicializa os TIMERS
        /// </summary>
        private void InitializeTimer()
        {
            RobotTimer = new Timer();
            RobotTimer.Interval = 1000* (double)Properties.Settings.Default.TimerInterval;
            RobotTimer.Enabled = true;
            RobotTimer.Elapsed += new ElapsedEventHandler(RobotTimer_Tick);
            RobotTimer.Start();
        }

        #endregion

        private void PdfWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {

        }
    }
}

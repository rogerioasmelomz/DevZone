using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Robot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //static void Main()
        //{
        //    ServiceBase[] ServicesToRun;
        //    ServicesToRun = new ServiceBase[]
        //    {
        //        new Service1()
        //    };
        //    ServiceBase.Run(ServicesToRun);
        //}

        static void Main(string[] args)
        {
            bool debugMode = false;
            if (args.Length > 0)
            {
                for (int ii = 0; ii < args.Length; ii++)
                {
                    switch (args[ii].ToUpper())
                    {
                        case "/I":
                            InstallService();
                            return;
                        case "/U":
                            UninstallService();
                            return;
                        case "/D":
                            debugMode = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SVC_Robot()
            };
            ServiceBase.Run(ServicesToRun);

            //if (debugMode)
            //{
            //    Program service = new Program();
            //    service.OnStart(null);
            //    Console.WriteLine("Service Started...");
            //    Console.WriteLine("<press any key to exit...>");
            //    Console.Read();
            //}
            //else
            //{
            //    System.ServiceProcess.ServiceBase.Run(new Program());
            //}
        }


        private static void InstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
            }
            catch { }
        }

        private static void UninstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[]
            { "/u", Assembly.GetExecutingAssembly().Location });
        }
    }
}

using Primavera.MotoresPrimavera;
using Primavera.WPF_Application_Test.ServicePrimavera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Primavera.WPF_Application_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /// <summary>
        /// Método para resolução das assemblies.
        /// </summary>
        /// <param name="sender">Application</param>
        /// <param name="args">Resolving Assembly Name</param>
        /// <returns>Assembly</returns>
        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyFullName;
            System.Reflection.AssemblyName assemblyName;
            const string PRIMAVERA_COMMON_FILES_FOLDER = "PRIMAVERA\\SG800"; //pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
            assemblyName = new System.Reflection.AssemblyName(args.Name);
            assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll");
            if (System.IO.File.Exists(assemblyFullName))
                return System.Reflection.Assembly.LoadFile(assemblyFullName);
            else
                return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contacto = new ServicePrimavera.Contactos()
                    {
                        codigo = (txtNovoCodigo.Text == "" ) ? txtCodigo.Text : txtNovoCodigo.Text,
                        nomeCompleto = txtNome.Text,
                        morada = txtMorada.Text,
                        localidade = txtBairro.Text,
                        emailPrincipal = txtEmail.Text
                    };
                
                
                ServicePrimavera.Primavera_ServiceClient client = new ServicePrimavera.Primavera_ServiceClient();
                client.Open();
                var a = client.ActualizarAluno(1, "Transcom", "fenix-user", "123456", txtCodigo.Text, txtNovoCodigo.Text, txtNome.Text, "", 0, txtMorada.Text, txtBairro.Text, txtTelefone1.Text, txtNuit.Text, "ISU", "", false,true, txtIngresso.Text,txtCurso.Text,txtTurma.Text, contacto);
                
                //, lista.ToArray());

                MessageBox.Show(a.tipoProblema + "," + a.codigo + "," + a.codeLevel + "," + a.codeLevel + "," +
                    a.codeLevel + "," + a.descricao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var listaArtigos = new List<Artigo>();
                listaArtigos.Add(new Artigo() { artigo = "ID", quantidade = 1 });
                ServicePrimavera.Primavera_ServiceClient client = new ServicePrimavera.Primavera_ServiceClient();
                client.Open();
                var a = client.CriarFactura(1, "Transcom", "fenix-user", "123456","Guimarães Mahota", txtCodigo.Text,"FA", listaArtigos.ToArray());

                //, lista.ToArray());

                MessageBox.Show(a.tipoProblema + "," + a.codigo + "," + a.codeLevel + "," + a.codeLevel + "," +
                    a.codeLevel + "," + a.descricao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var listaArtigos = new List<Artigo>();
                listaArtigos.Add(new Artigo() { artigo = "DAT", quantidade = 2 });
                listaArtigos.Add(new Artigo() { artigo = "PROPINA_M1", quantidade = 1 });
                ServicePrimavera.Primavera_ServiceClient client = new ServicePrimavera.Primavera_ServiceClient();
                client.Open();
                var a = client.CriaOuActualizarContrato(1, "Transcom", "fenix-user", "123456", "Guimarães Mahota", txtCodigo.Text, "CTR",txtSemestre.Text, listaArtigos.ToArray());

                //, lista.ToArray());

                MessageBox.Show(a.tipoProblema + "," + a.codigo + "," + a.codeLevel + "," + a.codeLevel + "," +
                    a.codeLevel + "," + a.descricao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var contacto = new ServicePrimavera.Contactos()
                {
                    codigo = txtCodigo.Text,
                    nomeCompleto = txtNome.Text,
                    morada = txtMorada.Text,
                    localidade = txtBairro.Text,
                    emailPrincipal = txtEmail.Text
                };

                ServicePrimavera.Primavera_ServiceClient client = new ServicePrimavera.Primavera_ServiceClient();
                client.Open();
                var a = client.CriarAluno(1, "Transcom", "fenix-user", "123456", txtCodigo.Text, txtNome.Text, "", 0, txtMorada.Text, txtBairro.Text, txtTelefone1.Text, txtNuit.Text, "ISU", "", false,true, txtIngresso.Text, "","", contacto);
                
                MessageBox.Show(a.tipoProblema + "," + a.codigo + "," + a.codeLevel + "," + a.codeLevel + "," +
                    a.codeLevel + "," + a.descricao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {

                ServicePrimavera.Primavera_ServiceClient client = new ServicePrimavera.Primavera_ServiceClient();
                client.Open();
                var a = client.ConsultaConta(1, "Transcom", "fenix-user", "123456",txtCodigo.Text);
                
                MessageBox.Show(a.pedentes.Sum(x=> x.valorPendente ).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MotoresErp m = new MotoresErp();
            
            var result = m._empresaErp.AbreEmpresaPrimavera(0,"Transcom", "Accsys", "accsys2011");
            m.inicializaMotores_EmpresaErp();


            m._comercial.RemoveContactoCliente(txtCodigo.Text);
            //MessageBox.Show(m._comercial.RemoveContactoCliente(txtCodigo.Text).to_String());
        }
    }
}

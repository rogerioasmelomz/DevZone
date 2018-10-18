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
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using MIT.Aplicacoes.Service_MIT_MotoresErp;

namespace MIT.Aplicacoes.View.Geral
{
    /// <summary>
    /// Interaction logic for ParametrosGerais.xaml
    /// </summary>
    public partial class ParametrosGerais :  Flyout
    {
        private ObservableCollection<Empresa> lista = new ObservableCollection<Empresa>();

        public ParametrosGerais()
        {
            InitializeComponent();            
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var service = new Service_MIT_MotoresErp.MIT_MotoresErpClient();
            string o = "";
            var listaEmpresas = service.Empresa_Lista(cbInstancia.SelectedIndex, txtUser.Text,txtPassword.Password, ref o);

            cbEmpresaPri.Items.Clear();

            foreach (var emp in listaEmpresas)
            {
                cbEmpresaPri.Items.Add(emp);
                cbEmpresaPri.DisplayMemberPath = "Codigo";
            }

        }
    }
}

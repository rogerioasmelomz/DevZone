namespace Plugin2
{
    using System.Windows.Forms;

    using PluginBase;

    public class Plugin2 : IPlugin
    {
        public void Activate()
        {
            MessageBox.Show("Activating Plugin 2");
        }

        public void Execute()
        {
            MessageBox.Show("Peforming Action Plugin 2");
        }

        public void Deactivate()
        {
            MessageBox.Show("Deactivating Plugin 2");
        }
    }
}

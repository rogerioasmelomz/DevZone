namespace Plugin1
{
    using System.Windows.Forms;

    using PluginBase;

    public class Plugin1 : IPlugin
    {
        public void Activate()
        {
            MessageBox.Show("Activating Plugin 1");
        }

        public void Execute()
        {
            MessageBox.Show("Peforming Action Plugin 1");
        }

        public void Deactivate()
        {
            MessageBox.Show("Deactivating Plugin 1");
        }
    }
}

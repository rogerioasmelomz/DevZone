namespace PluginBase
{
    /// <summary>
    /// The plug-in interface.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Activate the plug-in.
        /// </summary>
        void Activate();

        /// <summary>
        /// Execute the plug-in.
        /// </summary>
        void Execute();

        /// <summary>
        /// Deactivate the plug-in.
        /// </summary>
        void Deactivate();
    }
}
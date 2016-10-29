namespace Application
{
    using System;

    /// <summary>
    /// Contains the context of the current plug-in.
    /// </summary>
    [Serializable]
    public class PluginContext
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete plugin.
        /// </summary>
        public bool CanDeletePlugin { get; set; }
    }
}
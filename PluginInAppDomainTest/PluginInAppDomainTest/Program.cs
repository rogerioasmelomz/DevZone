namespace Application
{
    using System;
    using System.IO;
    using System.Reflection;

    using PluginBase;

    using PluginInAppDomainTest;

    /// <summary>
    /// https://stackoverflow.com/questions/425077/how-to-delete-the-pluginassembly-after-appdomain-unloaddomain/2475177#2475177
    /// </summary>
    public class Program
    {
        static void Main()
        {
            try
            {
                // Iterate through all plug-ins.
                foreach (var filePath in Directory.GetFiles(Constants.PluginPath, Constants.PluginSearchPattern))
                {
                    // Create the plug-in AppDomain setup.
                    var pluginAppDomainSetup = new AppDomainSetup();
                    pluginAppDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

                    // Create the plug-in AppDomain with the setup.
                    var plugInAppDomain = AppDomain.CreateDomain(filePath, null, pluginAppDomainSetup);

                    // Pass the plug-in file path to the appdomain
                    var pluginContext = new PluginContext { FilePath = filePath };
                    plugInAppDomain.SetData(Constants.PluginContextKey, pluginContext);

                    // Execute the loader in the plug-in AppDomain's context.
                    // This will also execute the plug-in.
                    plugInAppDomain.DoCallBack(PluginCallback);

                    // Retrieve the flag if the plug-in has executed and can be deleted.
                    pluginContext = plugInAppDomain.GetData(Constants.PluginContextKey) as PluginContext;

                    // Unload the plug-in AppDomain.
                    AppDomain.Unload(plugInAppDomain);

                    // Delete the plug-in if applicable.
                    if (pluginContext != null && pluginContext.CanDeletePlugin)
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// The callback routine that is executed in the plug-in AppDomain context.
        /// </summary>
        private static void PluginCallback()
        {
            try
            {
                // Retrieve the filePath from the plug-in AppDomain
                var pluginContext = AppDomain.CurrentDomain.GetData(Constants.PluginContextKey) as PluginContext;
                if (pluginContext != null)
                {
                    // Load the plug-in.
                    var pluginAssembly = Assembly.LoadFrom(pluginContext.FilePath);

                    // Iterate through types of the plug-in assembly to find the plug-in class.
                    foreach (var type in pluginAssembly.GetTypes())
                    {
                        if (type.IsClass && typeof(IPlugin).IsAssignableFrom(type))
                        {
                            // Create the instance of the plug-in and call the interface methods.
                            var plugin = Activator.CreateInstance(type) as IPlugin;
                            if (plugin != null)
                            {
                                plugin.Activate();
                                plugin.Execute();
                                plugin.Deactivate();

                                // Set the delete flag to true, to signal that the plug-in can be deleted.
                                pluginContext.CanDeletePlugin = true;
                                AppDomain.CurrentDomain.SetData(Constants.DeletePluginKey, pluginContext);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}

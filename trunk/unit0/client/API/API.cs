using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Client.API
{
    public interface IClientPlugin
    {
        void InitClientPlugin();
        void UnloadClientPlugin();
        string ClientPluinName();
    }

    public partial class ClientAPI
    {
        public static List<IClientPlugin> Plugins = new List<IClientPlugin>();
        protected static List<Assembly> LoadedAssemblies = new List<Assembly>();

        public static void LoadPlugin(Assembly assembly)
        {
            bool loadMe = false;
            foreach (Type t in assembly.GetTypes())
            {
                Type i = t.GetInterface(typeof(IClientPlugin).Name);
                if (i != null)
                {
                    loadMe = true;
                    IClientPlugin plugin = (IClientPlugin)Activator.CreateInstance(t);
                    Plugins.Add(plugin);
                    plugin.InitClientPlugin();
                }
            }
            if (loadMe)
                LoadedAssemblies.Add(assembly);
        }

        public static void UnloadPlugins()
        {
            foreach (IClientPlugin plugin in Plugins)
                plugin.UnloadClientPlugin();

            Plugins.Clear();
        }
    }
}

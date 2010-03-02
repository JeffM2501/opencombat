using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Project2501Server;

using ServerConfigurator;

namespace CLIServer
{
    class Program
    {
        static ServerConfig config = null;

        static void Main(string[] args)
        {
            string configFile = "config.xml";

            if (args.Length > 0)
                configFile = args[0];

            config = new ServerConfig(configFile);

            if (config.ConfigItems.Count == 0)
                LoadDefaultConfig(config);

            int sleepTime = 10;

            Server server = new Server(config.GetInt("port"));
            server.PublicListInfo = new PublicListCallback(PublicListCallback);

            server.Init();
            while (true)
            {
                server.Service();
                Thread.Sleep(sleepTime);
            }
        }

        static void LoadDefaultConfig(ServerConfig config)
        {
            config.SetItem("port", "2501");
            config.SetItem("host", "localhost:2501");
            config.SetItem("description", "Default Test Server Run by CLIServer");
            config.SetItem("name", "TestServer");

            config.Save();
        }

        static bool PublicListCallback(ref string host, ref string name, ref string description, ref string key, ref string type)
        {
            host = config.GetItem("host");
            name = config.GetItem("name");
            description = config.GetItem("description");
            key = config.GetItem("key");
            type = config.GetItem("type");

            return true;
        }
    }
}

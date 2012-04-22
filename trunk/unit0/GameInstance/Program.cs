using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

using InstanceConfig;

namespace GameInstance
{
    class Program
    {
        public static Configuration Config = Configuration.Empty;

        public static GameServer Server = null;

        public static bool Die = false;

        static void Main(string[] args)
        {
            string confPath = "temp.xml";

            if (args.Length != 0)
                confPath = args[0];

            if (File.Exists(confPath))
                Config = Configuration.Read(confPath);
            else
            {
                Config = new Configuration();
                Config.Write(confPath);
            }

            Server = new GameServer();

            while (!Server.Die())
            {
                Server.Update();
                Thread.Sleep(10);
            }

            Server.Kill();
        }
    }
}

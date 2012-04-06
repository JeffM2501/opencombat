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
            if (args.Length == 0)
                Config = Configuration.Read(args[0]);

            if (Config == Configuration.Empty)
            {
                if (args.Length > 0 && args[0] != string.Empty)
                {
                    Config = new Configuration();
                    Config.Write(args[0]);
                }
                else
                    return;
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

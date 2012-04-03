using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using InstanceConfig;

namespace GameInstance
{
    class Program
    {
        public static Configuration Config = Configuration.Empty;
    
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            Config = Configuration.Read(args[0]);
            if (Config == Configuration.Empty)
                return;

            WritePID();
            
        }

        static void WritePID()
        {
            Int32 pid = Process.GetCurrentProcess().Id;
            if (Directory.Exists(Path.GetDirectoryName(Config.PIDPath)))
            {
                FileInfo file = new FileInfo(Config.PIDPath);
                FileStream fs = file.OpenWrite();
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(pid.ToString());
                sw.Close();
                fs.Close();
            }
        }
    }
}

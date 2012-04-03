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
     
        }
    }
}

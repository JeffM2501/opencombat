using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace InstanceManager
{
    class Program
    {
        static List<WebInbound> Requests = new List<WebInbound>();

        static void Main(string[] args)
        {
            InstanceManagerConfig config = GetConfig(args);

            HttpListener listener = new HttpListener();

            foreach (string prefix in config.ListenPorts)
                listener.Prefixes.Add(prefix);

            listener.Start();
            bool killMe = false;

            Manager.TheManager.Init(config);

            Thread deadThread = new Thread(new ThreadStart(CheckForDead));

            while (!killMe)
            {
                HttpListenerContext context = listener.GetContext();

                if (!deadThread.IsAlive)
                    deadThread.Start();

                WebInbound r = new WebInbound(context);
                lock(Requests)
                    Requests.Add(r);

                Thread.Sleep(100);
            }
            
            listener.Stop();
            Manager.TheManager.Kill();

            if(deadThread.IsAlive)
                deadThread.Abort();

            lock(Requests)
            {
                foreach (WebInbound request in Requests)
                    request.Kill();
                Requests.Clear();
            }
        }

        static void CheckForDead()
        {
            bool done = false;
            while (!done)
            {
                Thread.Sleep(200);
                lock (Requests)
                {
                    List<WebInbound> dead = new List<WebInbound>();
                    foreach (WebInbound request in Requests)
                    {
                        if (request.Done)
                            dead.Add(request);
                    }

                    foreach (WebInbound request in dead)
                        Requests.Remove(request);

                    if (Requests.Count == 0)
                        done = true;
                }
            }
        }

        static string GetConfigPath(string[] args)
        {
            int index = Array.FindIndex(args, IsConfig);
            if (index == -1 || index == args.Length-1)
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "instance_manager_config.xml");

            return new FileInfo(args[index + 1]).FullName;
        }

        static InstanceManagerConfig GetConfig(string[] args)
        {
            string path = GetConfigPath(args);
            InstanceManagerConfig cfg = InstanceManagerConfig.Read(path);

            // create it, or update it to the current
            cfg.Write(path);

            return cfg;
        }

        public static bool IsConfig(string s)
        {
            return s == "-config";
        }
    }

    internal class InstanceManagerConfig
    {
        static List<string> DefualtPrefixes()
        {
            List<string> l = new List<string>();
            l.Add("http://localhost:2502/");
            l.Add("http://127.0.0.1:2502/");
            return l;
        }

        public List<string> ListenPorts = new List<string>();
        public string HostKey = string.Empty;

        public string CoordinatorURL = "http://192.168.153.131/unit0/InstanceCoordinator/";

        public int MaxInstances = 0;
        public int MaxPlayers = 0;
        public double HeartbeatTimeout = 60.0;

        public int InstanceManagePort = 2503;

        public string InstanceConfigPath = "./";
        public string PathToInstanceExe = "./GameInstance.exe";
        public string CLICommandLine = string.Empty;

        public string PIDFolder = string.Empty;

        public static InstanceManagerConfig Read(string path)
        {
            XmlSerializer xml = new XmlSerializer(typeof(InstanceManagerConfig));
            FileInfo file = new FileInfo(path);
            InstanceManagerConfig cfg = new InstanceManagerConfig();

            if (file.Exists)
            {
                FileStream fs = file.OpenRead();
                cfg = (InstanceManagerConfig)xml.Deserialize(fs);
                fs.Close();
            }
            else
                cfg.ListenPorts = DefualtPrefixes();

            return cfg;
        }

        public void Write(string path)
        {
            XmlSerializer xml = new XmlSerializer(typeof(InstanceManagerConfig));
            FileInfo file = new FileInfo(path);
            if (file.Exists)
                file.Delete();

            FileStream fs = file.OpenWrite();
            xml.Serialize(fs,this);
            fs.Close();
        }
    }
}

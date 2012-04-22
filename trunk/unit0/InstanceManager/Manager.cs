using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace InstanceManager
{
    internal class Manager
    {
        public static Manager TheManager = new Manager();
        public InstanceManagerConfig Config = null;

        int _hostID = 0;
        public int HostID { get { lock (locker) return _hostID; } }
        Thread worker = null;

        object locker = new object();

        internal InstanceConnection InstanceLink;

        protected bool Die = false;

        public Manager()
        {

        }

        public void Init(InstanceManagerConfig cfg)
        {
            Config = cfg;

            InstanceLink = new InstanceConnection(cfg);
            InstanceLink.InstanceConnected += new InstanceConnection.InstanceEvent(InstanceLink_InstanceConnected);
            InstanceLink.InstanceDisconnected += new InstanceConnection.InstanceEvent(InstanceLink_InstanceDisconnected);
            InstanceLink.InstanceMessage += new InstanceConnection.InstanceEvent(InstanceLink_InstanceMessage);

            // start the main logic thread
            worker = new Thread(new ThreadStart(ProcessConnections));
            worker.Start();
        }

        protected Dictionary<UInt64, Instance> Instances = new Dictionary<UInt64, Instance>();

        protected Instance GetInstance(UInt64 id)
        {
            if (Instances.ContainsKey(id))
                return Instances[id];
            return null;
        }

        protected bool StartInstance(string paramters, UInt64 ID)
        {
            lock (Instances)
            {
                if (Instances.ContainsKey(ID))
                    return false;
            }

            Instance inst = new Instance(paramters, ID);

            lock (Config)
            {
                string configFile = new FileInfo(Path.Combine(Config.InstanceConfigPath, ID.ToString() + ".xml")).FullName;
                inst.Config.Write(configFile);

                string command = string.Empty;
                if (Config.CLICommandLine != string.Empty)
                    command = Config.CLICommandLine;
                else if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                    command += "mono ";

                string args = configFile;
                if (command == string.Empty)
                    command = Config.PathToInstanceExe;
                else
                    args = Config.PathToInstanceExe + " " + configFile;

                inst.Proc = Process.Start(command, args);
                if (inst.Proc == null)
                    return false;

                lock (Instances)
                    Instances.Add(ID, inst);
            }

            return true;
        }

        protected void KillInstance(UInt64 ID)
        {
            InstanceLink.SendMessage(ID, "ABORT");
            Thread.Sleep(200);

            Process process = null;

            lock (Instances)
            {
                if (Instances.ContainsKey(ID))
                    process = Instances[ID].Proc;
            }

            if (process != null && !process.HasExited)
                process.Kill();

            lock (Instances)
            {
                if (Instances.ContainsKey(ID))
                    Instances.Remove(ID);
            }
        }

        void InstanceLink_InstanceMessage(UInt64 id)
        {
            Instance instance = GetInstance(id);
            if (instance == null)
                return;
        }

        void InstanceLink_InstanceDisconnected(UInt64 id)
        {
            Instance instance = GetInstance(id);
            if (instance == null)
                return;

            instance.ConnectionStatus = Instance.Status.Disconnected;

            // kill them or do a timeout?
            KillInstance(id);
        }

        void InstanceLink_InstanceConnected(UInt64 id)
        {
            Instance instance = GetInstance(id);
            if (instance == null)
                return;
            InstanceLink.SendMessage(id, "accept;OK");
            instance.ConnectionStatus = Instance.Status.Connected;
        }

        public void Kill()
        {
            if (worker.IsAlive)
                worker.Abort();

            InstanceLink.Kill();
        }

        Stopwatch timer = new Stopwatch();

        public double Now
        {
            get
            {
                lock(timer)
                {
                    if (!timer.IsRunning)
                        timer.Start();

                    return timer.ElapsedMilliseconds * 0.001;
                }
            }
        }

        double LastHeartbeat = 0;
        
        protected void ProcessConnections()
        {
           /* gotHostID = false;*/

            SendInitalConnection();
            while (true)
            {
                ProcessInbound();
                double now = Now;

                if (_hostID != -1)
                {
                    double HearbeatTimeout = -1;
                    lock (Config)
                        HearbeatTimeout = Config.HeartbeatTimeout;

                    if (now - LastHeartbeat > HearbeatTimeout)
                    {
                        SendHeartbeat();
                        LastHeartbeat = now;
                    }
                }

                List<UInt64> dead = new List<UInt64>();
                lock (Instances)
                {

                    foreach (Instance inst in Instances.Values)
                    {
                        if (inst.Proc.HasExited)
                            dead.Add(inst.Config.InstanceID);
    
                    }
                }

                foreach (UInt64 id in dead)
                    KillInstance(id);

                Thread.Sleep(100);
            }
        }

        protected void GotHostID()
        {
            // wait to be told what to do?
        }

        protected void SendHeartbeat()
        {
            string URL = string.Empty;

            lock (Config)
            {
                URL = "?action=heartbeat&ID=" + _hostID.ToString();
                URL += "&max_instances" + Config.MaxInstances.ToString();
                URL += "&max_players" + Config.MaxPlayers.ToString();
            }

            WebOutbound.Send(URL);
        }

        protected void SendInitalConnection()
        {
            string URL = string.Empty;

            lock (Config)
            {
                URL = "?action=addhost&key=" + System.Web.HttpUtility.UrlEncode(Config.HostKey);
                URL += "&max_instances" + Config.MaxInstances.ToString();
                URL += "&max_players" + Config.MaxPlayers.ToString();
            }

            WebOutbound.Send(URL);
        }

        protected void ProcessInbound()
        {
            string msg = PopWebMessage();
            while (msg != string.Empty)
            {
                string[] parts = msg.Split(";".ToCharArray());
                string code = parts[0];
                if (code == "addhost" && parts.Length > 1)
                {
                    if (parts[1] != "OK")
                    {
                        Console.WriteLine("Connection error to coordinator: " + msg);
                        Die = true;
                    }
                    else
                        int.TryParse(parts[2].Split("=".ToCharArray())[1], out _hostID);
                }
                msg = PopWebMessage();
            }

            HttpListenerRequest query = PopWebQuery();
            while (query != null)
            {
                string action = string.Empty;
                if (query.QueryString.GetValues("action").Length > 0)
                    action = query.QueryString.GetValues("action")[0];

                query = PopWebQuery();
            }
        }

        protected List<string> InboundWebMessages = new List<string>();

        protected string PopWebMessage()
        {
            lock (InboundWebMessages)
            {
                if (InboundWebMessages.Count == 0)
                    return string.Empty;

                string ret = InboundWebMessages[0];
                InboundWebMessages.RemoveAt(0);
                return ret;
            }
        }

        public void NewWebMessage(string msg)
        {
            lock (InboundWebMessages)
                InboundWebMessages.Add(msg);
        }

        protected List<HttpListenerRequest> InboundWebQueries = new List<HttpListenerRequest>();

        protected HttpListenerRequest PopWebQuery()
        {
            lock (InboundWebQueries)
            {
                if (InboundWebQueries.Count == 0)
                    return null;

                HttpListenerRequest ret = InboundWebQueries[0];
                InboundWebQueries.RemoveAt(0);
                return ret;
            }
        }

        public void NewWebQuery(HttpListenerRequest query)
        {
            lock (InboundWebQueries)
                InboundWebQueries.Add(query);
        }
    }
}

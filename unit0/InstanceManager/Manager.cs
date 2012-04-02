using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Web;
using System.Diagnostics;

namespace InstanceManager
{
    public class Manager
    {
        public static Manager Instance = new Manager();
        InstanceManagerConfig Config = null;

        int _hostID = -1;
        int HostID { get { lock (locker)return _hostID; } }
        Thread worker = null;

        object locker = new object();

        public Manager()
        {

        }

        public void Init(InstanceManagerConfig cfg)
        {
            Config = cfg;

            // start the main logic thread
            worker = new Thread(new ThreadStart(Process));
            worker.Start();

            // start the game proto thread that listens for instances
        }

        public void Kill()
        {
            if (worker.IsAlive)
                worker.Abort();
        }

        bool gotHostID = false;

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
        

        protected void Process()
        {
            gotHostID = false;

            SendInitalConnection();
            while (true)
            {
                double now = Now;

                // check for stuff.
                if (!gotHostID)
                {
                    if (HostID != -1)
                    {
                        gotHostID = true;
                        if (HostID == -2)
                        {  // error
                        }
                        else
                        {
                            // real host ID
                        }
                    }
                }
                else
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

                Thread.Sleep(100);
            }
        }

        protected void SendHeartbeat()
        {

        }

        protected void SendInitalConnection()
        {
            string URL = string.Empty;

            lock (Config)
            {
                URL = (string)Config.CoordinatorURL.Clone();
                URL += "?action=addhost&key=" + System.Web.HttpUtility.UrlEncode(Config.HostKey);
                URL += "&max_instances" + Config.MaxInstances.ToString();
                URL += "&max_players" + Config.MaxPlayers.ToString();
            }
            
            WebClient client = new  WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(URL));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result.Contains("Error:"))
            {

            }
            else if (e.Result.Contains("OK: "))
            {
                string[] nugs = e.Result.Split(" ".ToCharArray());

                lock (locker)
                {
                    int.TryParse(nugs[1], out _hostID);
                    if (_hostID == -1)
                        _hostID = -2;
                }
            }
        }
    }
}

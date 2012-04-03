using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace InstanceManager
{
    internal class WebOutbound
    {
        protected static List<WebClient> ActiveClients = new List<WebClient>();

        public static void Send(string URL)
        {
            lock (Manager.TheManager.Config)
                URL = Manager.TheManager.Config.CoordinatorURL + URL;

            WebClient client = new  WebClient();
            lock (ActiveClients)
                ActiveClients.Add(client);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(URL));
        }

        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Manager.TheManager.NewWebMessage(e.Result);

            WebClient client = sender as WebClient;
            if (client != null)
            {
                lock(ActiveClients)
                    ActiveClients.Remove(client);
            }
        }
    }

    internal class WebInbound
    {
        protected HttpListenerContext Context = null;

        Thread worker = null;

        object locker = new object();

        protected bool done = false;

        public bool Done { get { lock (locker)return done; } }

        public WebInbound(HttpListenerContext context)
        {
            Context = context;
            worker = new Thread(new ThreadStart(Process));
            worker.Start();
        }

        public void Kill()
        {
            if (worker.IsAlive)
                worker.Abort();
        }

        protected void Process()
        {
            HttpListenerRequest request = Context.Request;

            string ConnectedHost = string.Empty;

            lock(Manager.TheManager.Config)
                ConnectedHost = (string)Manager.TheManager.Config.CoordinatorURL.Clone();

            if (!ConnectedHost.Contains(request.UserHostName) && !ConnectedHost.Contains(request.UserHostAddress))
                return;

            Manager.TheManager.NewWebQuery(request);

            Context.Response.StatusCode = 200;
            StreamWriter sw = new StreamWriter(Context.Response.OutputStream);
            sw.WriteLine("OK;Accept");

            lock(locker)
                done = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Web;

using Lidgren.Network;

using Game;
using Game.Messages;

namespace GameInstance
{
    public class ResourceProcessor
    {
        protected static List<ResourceResponceMessage.Resource> Resources = new List<ResourceResponceMessage.Resource>();
        protected static List<ResourceResponceMessage.Resource> ResourcesPendingHash = new List<ResourceResponceMessage.Resource>();

        protected static Dictionary<string, int> ResourceNames = new Dictionary<string, int>();

        public static void AddResource(string name, string url)
        {
            lock (Resources)
            {
                if (ResourceNames.ContainsKey(name))
                    return;
            }
            ResourceResponceMessage.Resource res = new ResourceResponceMessage.Resource();
            res.Name = name;
            res.URL = url;

            if (Program.Config.ResourceHost != string.Empty)
            {
                lock (ResourcesPendingHash)
                    ResourcesPendingHash.Add(res);

                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(Program.Config.ResourceHost + "?action=hash&path=" + HttpUtility.UrlEncode(url)), res);
            
            }
            else
            {
                lock (Resources)
                {
                    Resources.Add(res);
                    ResourceNames.Add(res.Name, Resources.Count - 1);
                }
            }
        }

        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ResourceResponceMessage.Resource res = e.UserState as ResourceResponceMessage.Resource;
            if (res == null)
                return;
            lock (ResourcesPendingHash)
                ResourcesPendingHash.Remove(res);

            lock (Resources)
            {
                if (ResourceNames.ContainsKey(res.Name))
                    return;
            }

            if (!e.Result.Contains("Error"))
                res.Hash = e.Result;
            lock (Resources)
            {
                Resources.Add(res);
                ResourceNames.Add(res.Name, Resources.Count - 1);
            }
        }

        public static void AddResource (string name, byte[] data)
        {
            lock (Resources)
            {
                if (ResourceNames.ContainsKey(name))
                    return;
            }

            ResourceResponceMessage.Resource res = new ResourceResponceMessage.Resource();
            res.Name = name;
            res.data = data;
            if (data != null && data.Length > 0)
                res.Hash = Utilities.GetMD5Hash(data);
            lock (Resources)
            {
                Resources.Add(res);
                ResourceNames.Add(res.Name, Resources.Count - 1);
            }
        }

        public static void AddReqest(ResourceRequestMessage msg, Player player)
        {
            if (player == null)
                return;

            StartThread();

            lock (Requests)
                Requests.Add(new Request(msg, player));
        }

        protected class Request
        {
            public ResourceRequestMessage message = null;
            public Player player = null;

            public Request(ResourceRequestMessage msg, Player p)
            {
                message = msg;
                player = p;
            }
        }

        protected static List<Request> Requests = new List<Request>();

        protected static Request NextRequest()
        {
            lock (Requests)
            {
                if (Requests.Count != 0)
                {
                    Request r = Requests[0];
                    Requests.RemoveAt(0);
                    return r;
                }
            }

            return null;
        }

        public static void Kill()
        {
            if (worker == null || !worker.IsAlive)
                return;

            worker.Abort();
            worker = null;
        }

        protected static void StartThread()
        {
            if (worker != null && worker.IsAlive)
                return;

            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        protected static Thread worker = null;

        protected static void Run()
        {
            while (true)
            {
                Request request = NextRequest();
                while (request != null)
                {
                    if (request.message.ResourceNames.Count == 0)
                    {
                        // they want all the resources
                        ResourceResponceMessage msg = new ResourceResponceMessage();
                        lock (Resources)
                        {
                            foreach (ResourceResponceMessage.Resource res in Resources)
                            {
                                ResourceResponceMessage.Resource resInfo = new ResourceResponceMessage.Resource();
                                resInfo.Name = res.Name;
                                resInfo.URL = res.URL;
                                resInfo.Hash = res.Hash;

                                msg.Resources.Add(resInfo);
                            }
                        }
                        request.player.SendReliable(msg);
                    }
                    else
                    {
                        ResourceResponceMessage msg = new ResourceResponceMessage();
                        lock (Resources)
                        {
                            foreach (string res in request.message.ResourceNames)
                            {         
                                if (ResourceNames.ContainsKey(res))
                                    msg.Resources.Add(Resources[ResourceNames[res]]);
                            }

                            if (msg.Resources.Count > 0)
                                request.player.SendReliable(msg);
                        }
                    }

                    request = NextRequest();
                }

                Thread.Sleep(10);
            }
        }
    }
}

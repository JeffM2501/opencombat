using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Threading;

using Game;
using Game.Messages;

using GridWorld;

namespace Client
{
    internal class ResourceProcessor
    {
        public static ServerConnection Connection = null;
        public static ClientGame Client = null;

        protected static List<ResourceResponceMessage.Resource> ResourcesToWebGet = new List<ResourceResponceMessage.Resource>();
        protected static List<string> ResourcesToProtoGet = new List<string>();

        protected static List<ResourceResponceMessage.Resource> ResourcesToProcess = new List<ResourceResponceMessage.Resource>();

        public static string ResourceHost = string.Empty;

        public static event EventHandler<EventArgs> ResourcesComplete;

        public static void NewResponce(ResourceResponceMessage responce)
        {
            if (responce != null)
            {
                lock (ResourcesToProcess)
                {
                    foreach (ResourceResponceMessage.Resource res in responce.Resources)
                        ResourcesToProcess.Add(res);
                }
            }

            StartThread();
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

        protected static ResourceResponceMessage.Resource NextToProcess()
        {
            lock (ResourcesToProcess)
            {
                if (ResourcesToProcess.Count == 0)
                    return null;

                ResourceResponceMessage.Resource res = ResourcesToProcess[0];
                ResourcesToProcess.RemoveAt(0);
                return res;
            }
        }

        static void client_DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            ResourceResponceMessage.Resource res = e.UserState as ResourceResponceMessage.Resource;
            if (res == null)
                return;

            if (res.Name == ResourceRequestMessage.MapResourceName)
            {
                Client.CacheWorld(World.WorldDefData.Deserialize(e.Result), res.Hash);
            }
            else
            {
                // do stuff?
            }


            lock (ResourcesToWebGet)
                ResourcesToWebGet.Remove(res);

            CheckDone();
        }

        protected static void CheckDone()
        {
            int count = 0;
            lock (ResourcesToProcess)
                count += ResourcesToProcess.Count;

            lock (ResourcesToWebGet)
                count += ResourcesToWebGet.Count;

            lock (ResourcesToProtoGet)
                count += ResourcesToProtoGet.Count;

            if (count == 0 && ResourcesComplete != null)
                ResourcesComplete(Client, EventArgs.Empty);
        }

        protected static void Run()
        {
            while (true)
            {
                ResourceResponceMessage.Resource res = NextToProcess();
                while (res != null)
                {
                    if (res.data == null || res.data.Length == 0)
                    {
                        if (res.URL != string.Empty)
                        {
                            lock (ResourcesToWebGet)
                            {
                                ResourcesToWebGet.Add(res);
                                WebClient client = new WebClient();
                                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadCompleted);
                                client.DownloadDataAsync(new Uri(ResourceHost + "?action=get&path=" + HttpUtility.UrlEncode(res.URL)), res);
                            }
                        }
                        else
                        {
                            if (res.Name == ResourceRequestMessage.MapResourceName)
                            {
                                if (Client.HaveWorld(res.Hash))
                                    continue;
                            }
                            
                            ResourceRequestMessage msg = new ResourceRequestMessage();
                            msg.ResourceNames.Add(res.Name);
                            Connection.SendReliable(msg);
                            ResourcesToProtoGet.Add(res.Name);
                        }
                    }
                    else
                    {
                        lock (ResourcesToProtoGet)
                            ResourcesToProtoGet.Remove(res.Name);

                        if (res.Name == ResourceRequestMessage.MapResourceName)
                        {
                            Client.CacheWorld(World.WorldDefData.Deserialize(res.data), res.Hash);
                        }
                        else
                        {
                            // stuff
                        }
                    }

                    res = NextToProcess();
                }
                CheckDone();
                Thread.Sleep(10);
            }
        }
    }
}

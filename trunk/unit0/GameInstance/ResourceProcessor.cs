using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Lidgren.Network;

using Game;
using Game.Messages;

namespace GameInstance
{
    public class ResourceProcessor
    {
        public static void AddReqest(ResourceRequestMessage msg, Player player)
        {
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
                    }

                    request = NextRequest();
                }

                Thread.Sleep(10);
            }
        }
    }
}

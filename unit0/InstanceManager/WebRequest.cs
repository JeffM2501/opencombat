using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace InstanceManager
{
    class WebRequest
    {
        protected HttpListenerContext Context = null;

        Thread worker = null;

        object locker = new object();

        protected bool done = false;

        public bool Done { get { lock (locker)return done; } }

        public WebRequest(HttpListenerContext context)
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
            HttpListenerResponse response = Context.Response;

            lock(locker)
                done = true;
        }
    }
}

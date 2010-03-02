using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;

namespace Project2501Server
{
    public class ServerLister
    {
        public class Job
        {
            public enum Action
            {
                Add,
                Update,
                Remove,
            }

            public Action action = Action.Add;
            public string host = string.Empty;
            public string name = string.Empty;
            public string desc = string.Empty;
            public string key = string.Empty;
            public string serverType = string.Empty;
        }

        protected List<Job> PendingJobs = new List<Job>();

        protected Thread worker = null;

        public ServerLister()
        {
            worker = new Thread(new ThreadStart(Worker));
            worker.Start();
        }

        public void Dispose()
        {
            Kill();
        }

        public void Kill()
        {
            if (worker != null)
            {
                worker.Abort();
                worker = null;
            }
        }

        protected void Worker ()
        {
            bool done = false;

            WebClient webClient = new WebClient();

            while (!done)
            {
                Job job = null;
                int jobCount = 0;
                lock(PendingJobs)
                {
                    if (PendingJobs.Count > 0)
                    {
                        job = PendingJobs[0];
                        PendingJobs.Remove(job);
                    }

                    jobCount = PendingJobs.Count;
                }

                if (job != null)
                {
                    // do job

                    string url = "http://www.opencombat.net/services/list.php?action=";
                    switch (job.action)
                    {
                        default:
                            url += "addhost";
                            break;
                        case Job.Action.Update:
                            url += "updatehost";
                            break;
                        case Job.Action.Remove:
                            url += "removehost";
                            break;
                    }
                    if (job.host != string.Empty)
                        url += "&host=" + HttpUtility.UrlEncode(job.host);
                    else
                        url += "&host=127.0.0.1";

                    if (job.name != string.Empty)
                        url += "&name=" + HttpUtility.UrlEncode(job.name);
                    if (job.key != string.Empty)
                        url += "&key=" + HttpUtility.UrlEncode(job.key);
                    if (job.desc != string.Empty)
                        url += "&desc=" + HttpUtility.UrlEncode(job.desc);
                    if (job.serverType != string.Empty)
                        url += "&type=" + HttpUtility.UrlEncode(job.serverType);
                    else
                        url += "&type=unverified";

                    Stream stream = webClient.OpenRead(url);
                    StreamReader reader = new StreamReader(stream);

                    string code = reader.ReadLine();
                    
                    reader.Close();
                    stream.Close();
                }
                if (jobCount == 0)
                    Thread.Sleep(1000);
            }
        }

        public void AddHost (Job job )
        {
            lock(PendingJobs)
            {
                PendingJobs.Add(job);
            }
        }
    }
}

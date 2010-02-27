using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Project2501Server
{
    public class TokenChecker : IDisposable
    {
        public class Job
        {
            public UInt64 UID = 0;
            public UInt64 CID = 0;
            public UInt64 Token = 0;
            public String IP = string.Empty;

            public bool Checked = false;
            public bool Verified = false;

            public string Callsign = string.Empty;

            public object Tag = null;
        }

        protected List<Job> PendingJobs = new List<Job>();
        protected List<Job> FinishedJobs = new List<Job>();

        protected Thread worker = null;

        public TokenChecker()
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
                    job.Checked = false;
                    job.Verified = false;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.awesomelaser.com/p2501/Auth/authceck.php?uid=" + job.UID.ToString() + "&token=" + job.Token.ToString() + "&cid=" + job.CID.ToString() + "&ip=" + HttpUtility.UrlEncode(job.IP));
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    string code = reader.ReadLine();
                    if (code == "ok")
                    {
                        if (reader.ReadLine() == "verified")
                            job.Verified = true;

                        job.Callsign = reader.ReadLine();
                    }

                    reader.Close();
                    stream.Close();

                    if (job.Callsign == string.Empty)
                        job.Checked = false;

                    lock (FinishedJobs)
                    {
                        FinishedJobs.Add(job);
                    }
                }
                if (jobCount == 0)
                    Thread.Sleep(1000);
            }
        }

        public void AddJob ( UInt64 UID, UInt64 Token, UInt64 CID, string IP, object tag )
        {
            Job job = new Job();
            job.UID = UID;
            job.CID = CID;
            job.Token = Token;
            job.IP = IP;
            job.Tag = tag;

            lock(PendingJobs)
            {
                PendingJobs.Add(job);
            }
        }

        public Job GetFinishedJob ()
        {
            Job job = null;
            lock (FinishedJobs)
            {
                if (FinishedJobs.Count > 0)
                { 
                   job = FinishedJobs[0];
                   FinishedJobs.Remove(job);
                }
            }
            return job;
        }
    }
}

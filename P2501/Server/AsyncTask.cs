using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Project2501Server
{
    public class AsyncTask : IDisposable
    {
        public class Job
        {
            public bool PostToFinished = false;
            public object Tag = null;
        }

        protected List<Job> PendingJobs = new List<Job>();
        protected List<Job> FinishedJobs = new List<Job>();

        protected List<Thread> workers = new List<Thread>();

        public AsyncTask()
        {
        }

        public void Dispose()
        {
            Kill();
        }

        public void Kill()
        {
            lock(workers)
            {
                foreach(Thread t in workers)
                    t.Abort();

                workers.Clear();
            }
        }

        protected virtual void Process ( Job job )
        {
        }

        protected void Worker ()
        {
            bool done = false;


            while (!done)
            {
                Job job = null;
                lock(PendingJobs)
                {
                    if (PendingJobs.Count > 0)
                    {
                        job = PendingJobs[0];
                        PendingJobs.Remove(job);
                    }
                }

                if (job != null)
                {
                    Process(job);
                    if (job.PostToFinished)
                    {
                        lock (FinishedJobs)
                        {
                            FinishedJobs.Add(job);
                        }
                    }
                }
                    

                lock (PendingJobs)
                {
                    if (PendingJobs.Count == 0)
                        done = true;
                }
            }
        }

        public void AddJob ( Job job )
        {
            lock(PendingJobs)
            {
                PendingJobs.Add(job);
                lock(workers)
                {
                    if (workers.Count == 0)
                    {
                        Thread t = new Thread(new ThreadStart(Worker));
                        t.Start();
                        workers.Add(t);
                    }
                }
            }
        }

        public Job GetFinishedJob()
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

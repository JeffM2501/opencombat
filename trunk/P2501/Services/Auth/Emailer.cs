﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace AuthServer
{
    public class Emailer : IDisposable
    {
        public class Job
        {
            public string To = string.Empty;
            public string From = string.Empty;
            public string Subject = string.Empty;
            public string Body = string.Empty;

            public object Tag = null;
        }

        protected List<Job> PendingJobs = new List<Job>();

        protected Thread worker = null;

        protected string smtpServer = string.Empty;

        SmtpClient client = null;
        public Emailer()
        {
            client = new SmtpClient("localhost");
        }

        public Emailer(string server)
        {
            client = new SmtpClient(server);
        }

        protected void Start()
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

        protected void Worker()
        {
            bool done = false;

            while (!done)
            {
                Job job = null;
                int jobCount = 0;
                lock (PendingJobs)
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
                    MailAddress from = new MailAddress(job.From);
                    MailAddress to = new MailAddress(job.To);

                    MailMessage mail = new MailMessage(from,to);
   
                    mail.Subject = job.Subject;
                    mail.Body = job.Body;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;

                    try
                    {
                        client.Send(mail);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                if (jobCount == 0)
                    Thread.Sleep(1000);
            }
        }

        public void AddJob(string from, string to, string subject, string body, object tag)
        {
            Job job = new Job();
            job.From = from;
            job.To = to;
            job.Subject = subject;
            job.Body = body;
            job.Tag = tag;

            lock (PendingJobs)
            {
                PendingJobs.Add(job);
            }
        }
    }
}
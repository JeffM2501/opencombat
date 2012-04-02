/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;

namespace Project2501Server
{
    public class ServerLister : AsyncTask
    {
        public class ServerListJob : AsyncTask.Job
        {
            public ServerListJob() : base()
            {
                PostToFinished = false;
            }

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

        protected override void  Process(Job j)
        {
            WebClient webClient = new WebClient();

            ServerListJob job = j as ServerListJob;

            if (job != null)
            {
                // do job

                string url = "http://www.opencombat.net/services/list.php?action=";
                switch (job.action)
                {
                    default:
                        url += "addhost";
                        break;
                    case ServerListJob.Action.Update:
                        url += "updatehost";
                        break;
                    case ServerListJob.Action.Remove:
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
        }
    }
}

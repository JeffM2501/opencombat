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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Project2501Server
{
    public class TokenChecker : AsyncTask
    {
        public class TokenCheckerJob : AsyncTask.Job
        {
            public UInt64 UID = 0;
            public UInt64 CID = 0;
            public UInt64 Token = 0;
            public String IP = string.Empty;

            public bool Checked = false;
            public bool Verified = false;

            public string Callsign = string.Empty;

            public TokenCheckerJob(UInt64 uid, UInt64 token, UInt64 cid, string ip, object tag)
            {
                UID = uid;
                CID = cid;
                Token = token;
                IP = ip;
                Tag = tag;
            }
        }

        protected override void Process(Job j)
        {
            WebClient client = new WebClient();
            TokenCheckerJob job = j as TokenCheckerJob;

           if (job != null)
            {
                // do job
                job.Checked = false;
                job.Verified = false;

                string url = "http://www.opencombat.net/services/authceck.php?uid=" + job.UID.ToString() + "&token=" + job.Token.ToString() + "&cid=" + job.CID.ToString() + "&ip=" + HttpUtility.UrlEncode(job.IP);

                Stream stream = client.OpenRead(url);
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
            }
        }
    }
}

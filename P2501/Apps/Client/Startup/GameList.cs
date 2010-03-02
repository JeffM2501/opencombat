using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace P2501Client
{
    public class GameList : IDisposable
    {
        public class ListedServer
        {
            public string Name = string.Empty;
            public string Host = string.Empty;
            public string Description = string.Empty;
            public string Group = string.Empty;
        }

        UInt64 UID = 0;

        List<ListedServer> GameServers = new List<ListedServer>();
        bool dirty = false;

        Thread worker = null;

        public bool Dirty
        {
            get
            {
                lock (GameServers)
                {
                    return dirty; 
                }
            }
        }

        public void Dispose()
        {
            Kill();
        }

        public void Kill ()
        {
            if (worker == null)
                return;

            lock (GameServers)
            {
                worker.Abort();
                worker = null;
            }
        }

        public GameList ( UInt64 id )
        {
            UID = id;

            Update();
            worker = new Thread(new ThreadStart(Poll));
            worker.Start();
        }

        void Poll ()
        {
            Thread.Sleep(120 * 1000);
            Update();
        }

        protected void Update ()
        {
            lock (GameServers)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.opencombat.net/services/list.php?action=viewlist&uid=" + UID.ToString());
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                GameServers.Clear();

                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                int count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    ListedServer server = new ListedServer();
                    string[] nugs = reader.ReadLine().Split('\t');

                    if (nugs.Length == 0)
                        break;
                    server.Host = nugs[0];
                    server.Name = nugs[1];
                    server.Description = nugs[2];
                    server.Group = nugs[3];

                    GameServers.Add(server);
                }

                reader.Close();
                stream.Close();
                dirty = true;
            }
        }

        public List<ListedServer> GetGameServers()
        {
            List<ListedServer> newList = new List<ListedServer>();
            lock(GameServers)
            {
                foreach (ListedServer g in GameServers)
                    newList.Add(g);
               
                dirty = false;
            }

            return newList;
        }
    }
}

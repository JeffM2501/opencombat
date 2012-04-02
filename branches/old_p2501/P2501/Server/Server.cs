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
using System.Linq;
using System.Text;

using Hosts;
using Simulation;
using Lidgren.Network;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

using Messages;

namespace Project2501Server
{
    public class Client : System.Object
    {
        public NetConnection Connection;
        public Simulation.Player Player = null;

        public UInt64 UID = 0;
        public UInt64 CID = 0;
        public UInt64 Token = 0;

        public ServerInstance Instance = null;

        public bool Checked = false;
        public bool Verified = false;

        public Client ( NetConnection connection )
        {
            Connection = connection;
        }
    }

    public delegate void MessageHandler ( Client client, MessageClass message );
    public delegate void InstanceMessageHandler(Client client, MessageClass message, ServerInstance instance);
    public delegate bool PublicListCallback(ref string host, ref string name, ref string description, ref string key, ref string type);
    public delegate void DefaultInstanceSetupCallback(ref ServerInstanceSettings settings );
    
    public class InstanceEventArgs : EventArgs
    {
        public int InstanceID = 0;

        public InstanceEventArgs ( int i ) : base()
        {
            InstanceID = i;
        }
    }

    public delegate void InstanceNotifiactionEventHandler(object sender, InstanceEventArgs args);

    public partial class Server
    {
        Host host;

        protected TokenChecker tokenChecker = null;
        protected ServerLister serverLister = null;
        public bool SaveMessages = true;

        public PublicListCallback PublicListInfo = null;
        public DefaultInstanceSetupCallback DefaultInstanceSetup = null;

        public event InstanceNotifiactionEventHandler InstanceStarted;
        public event InstanceNotifiactionEventHandler InstanceStoped;

        MessageMapper messageMapper = new MessageMapper();

        public Dictionary<NetConnection, Client> Clients = new Dictionary<NetConnection, Client>();

        public List<String> PendingHostMessages = new List<String>();

        double lastUpdateTime = -1;

        Stopwatch timer;

        Thread ServerThread = null;

        ServerInstanceSettings defaultInstanceSettings = new ServerInstanceSettings();

        public int ServerSleepTime = 10;
        protected int port = 2501;

        public Server ( int p )
        {
            port = p;
        }

        public bool Init ()
        {
            defaultInstanceSettings.Description = "Root";
            // setup the default settings
            if (DefaultInstanceSetup != null)
                DefaultInstanceSetup(ref defaultInstanceSettings);

            if (defaultInstanceSettings.MapFile == null || !defaultInstanceSettings.MapFile.Exists)
                return false;

            // add the root instance
            int id = ServerInstanceManger.AddInstnace(this, defaultInstanceSettings);

            if (InstanceStarted != null)
                InstanceStarted(this, new InstanceEventArgs(id));

            timer = new Stopwatch();
            timer.Start();
            host = new Host(port);

            if (!NoTokenCheck)
            {
                tokenChecker = new TokenChecker();
                serverLister = new ServerLister();
            }

            host.Connect += new MonitoringEvent(host_Message);
            host.Disconnect += new MonitoringEvent(host_Message);
            host.DebugMessage += new MonitoringEvent(host_Message);

            InitMessageHandlers();

            listServer();
            return true;
        }

        protected void listServer()
        {
            if (NoTokenCheck)
                return;

            if (serverLister == null)
                return;

            if (PublicListInfo == null)
            {
                serverLister.Kill();
                serverLister = null;
                return;
            }

            ServerLister.ServerListJob job = new ServerLister.ServerListJob();
            if (!PublicListInfo(ref job.host, ref job.name, ref job.desc, ref job.key, ref job.serverType))
            {
                serverLister.Kill();
                serverLister = null;
                return;
            }

            serverLister.AddJob(job);
        }

        public bool Run ()
        {
            if (host == null)
            {
                if (!Init())
                    return false;
            }
            ServerThread = new Thread(new ThreadStart(PrivateRun));
            ServerThread.Start();

            return true;
        }

        protected void PrivateRun()
        {
            while(true)
            {
                Update();
                Thread.Sleep(ServerSleepTime);
            }
        }

        public void Kill()
        {
            ServerInstanceManger.StopAll();

            if (ServerThread != null)
                ServerThread.Abort();
            ServerThread = null;

            if (host != null)
                host.Kill();
            host = null;

            if (tokenChecker != null)
                tokenChecker.Kill();
            tokenChecker = null;

            if(serverLister != null)
                serverLister.Kill();
            serverLister = null;
        }

        protected void DisconnectPlayer ( NetConnection player )
        {
            if (Clients.ContainsKey(player))
            {
                Client client = Clients[player];
                Clients.Remove(player);

                if (client.Instance != null)
                    client.Instance.RemoveClient(client);
            }
        }     
   
        protected double Now ()
        {
            return timer.ElapsedMilliseconds * 0.001;
        }

        public void Service ()
        {
            if (ServerThread != null)
                return;

            Update();
        }

        protected void Update ()
        {
            lastUpdateTime = Now();

            NetConnection newConnect = host.GetPentConnection();
            while (newConnect != null)
            {
                Client client = new Client(newConnect);
                Clients.Add(newConnect,client );
                Send(client, MessageClass.Hail);
                newConnect = host.GetPentConnection();
            }

            NetConnection newDisconnect = host.GetPentDisconnection();
            while (newDisconnect != null)
            {
                DisconnectPlayer(newDisconnect);
                newDisconnect = host.GetPentDisconnection();
            }

            Message msg = host.GetPentMessage();
            while (msg != null)
            {
                ProcessMessage(msg);
                msg = host.GetPentMessage();
            }

            if (tokenChecker != null)
            {
                TokenChecker.TokenCheckerJob job = tokenChecker.GetFinishedJob() as TokenChecker.TokenCheckerJob;
                while (job != null)
                {
                    ProcessTokenJob(job);
                    job = tokenChecker.GetFinishedJob() as TokenChecker.TokenCheckerJob;
                }
            }

            int deadInstance = ServerInstanceManger.GetMournedInstance();
            while (deadInstance > 0)
            {
                if (InstanceStoped != null)
                    InstanceStoped(this, new InstanceEventArgs(deadInstance));
                deadInstance = ServerInstanceManger.GetMournedInstance();
            }
        }

        protected void ProcessTokenJob ( TokenChecker.TokenCheckerJob job )
        {
            Client client = job.Tag as Client;
            if (client == null)
                return;

            client.Checked = job.Checked;
            client.Verified = job.Verified;
            client.Player.Callsign = job.Callsign;
            if (!client.Checked)
                DisconnectPlayer(client.Connection);
            else
                FinishLogin(client);
        }

        public String PopHostMessage ( )
        {
            String msg = string.Empty;
            lock(PendingHostMessages)
            {
                if (PendingHostMessages.Count > 0)
                {
                    msg = PendingHostMessages[0];
                    PendingHostMessages.Remove(msg);
                }
            }
            return msg;
        }

        void host_Message(object sender, MonitoringEventArgs args)
        {
            if (!SaveMessages)
                return;

            lock (PendingHostMessages)
                PendingHostMessages.Add(args.Message);
        }
    }
}

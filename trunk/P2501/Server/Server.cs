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

        public bool Checked = false;
        public bool Verified = false;

        public Client ( NetConnection connection )
        {
            Connection = connection;
        }

        public void Hail ( Host host )
        {
            Messages.Hail hail = new Messages.Hail();
            host.SendMessage(Connection, hail.Pack(), hail.Channel());
        }
    }

    public delegate void MessageHandler ( Client client, MessageClass message );
    public delegate bool PublicListCallback(ref string host, ref string name, ref string description, ref string key, ref string type);

    public partial class Server
    {
        Sim sim = new Sim();
        Host host;

        protected TokenChecker tokenChecker = null;
        protected ServerLister serverLister = null;
        public bool SaveMessages = true;

        public PublicListCallback PublicListInfo = null;

        MessageMapper messageMapper = new MessageMapper();

        public Dictionary<NetConnection, Client> Clients = new Dictionary<NetConnection, Client>();

        public List<String> PendingHostMessages = new List<String>();

        double lastUpdateTime = -1;

        Stopwatch timer;

        Thread ServerThread = null;

        public int ServerSleepTime = 10;
        protected int port = 2501;

        public Server ( int p )
        {
            port = p;
        }

        public void Init ()
        {
            timer = new Stopwatch();
            timer.Start();
            host = new Host(port);
            sim.Init();

            tokenChecker = new TokenChecker();
            serverLister = new ServerLister();

            host.Connect += new MonitoringEvent(host_Message);
            host.Disconnect += new MonitoringEvent(host_Message);
            host.DebugMessage += new MonitoringEvent(host_Message);

            sim.PlayerJoined += new PlayerJoinedHandler(sim_PlayerJoined);
            sim.PlayerRemoved += new PlayerRemovedHandler(sim_PlayerRemoved);
            sim.PlayerStatusChanged += new PlayerStatusChangeHandler(sim_PlayerStatusChanged);

            InitMessageHandlers();

            listServer();
        }

        protected void listServer()
        {
            if (serverLister == null)
                return;

            if (PublicListInfo == null)
            {
                serverLister.Kill();
                serverLister = null;
                return;
            }

            ServerLister.Job job = new ServerLister.Job();
            if (!PublicListInfo(ref job.host, ref job.name, ref job.desc, ref job.key, ref job.serverType))
            {
                serverLister.Kill();
                serverLister = null;
                return;
            }

            serverLister.AddHost(job);
        }

        public void Run ()
        {
            ServerThread = new Thread(new ThreadStart(PrivateRun));
            ServerThread.Start();
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

        void sim_PlayerStatusChanged(object sender, PlayerEventArgs args)
        {
            if (args.player.Status == PlayerStatus.Alive)
            {
                PlayerSpawn spawn = new PlayerSpawn(args.player);
                host.Broadcast(spawn.Pack(), spawn.Channel());
            }
        }

        protected void sim_PlayerJoined ( object sender, PlayerEventArgs args )
        {           
            PlayerInfo info = new PlayerInfo(args.player);
            host.Broadcast(info.Pack(), info.Channel());
        }

        protected void sim_PlayerRemoved(object sender, PlayerEventArgs args)
        {
            Client client = args.player.Tag as Client;
            if (client != null)
            {
                Messages.Disconnect disconnect = new Messages.Disconnect();
                disconnect.ID = client.Player.ID;
                host.Broadcast(disconnect.Pack(), disconnect.Channel());
            }
        }

        protected void DisconnectPlayer ( NetConnection player )
        {
            if (Clients.ContainsKey(player))
            {
                Client client = Clients[player];
                Clients.Remove(player);

                if (client.Player != null)
                    sim.RemovePlayer(client.Player);
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
                client.Hail(host);
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

            TokenChecker.Job job = tokenChecker.GetFinishedJob();
            while (job != null)
            {
                ProcessTokenJob(job);
                job = tokenChecker.GetFinishedJob();
            }

            sim.Update(lastUpdateTime);
        }

        protected void ProcessTokenJob ( TokenChecker.Job job )
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

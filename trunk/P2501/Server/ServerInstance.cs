using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;

using Messages;
using Simulation;

namespace Project2501Server
{
    public class ServerInstanceManger
    {
        public static List<ServerInstance> Instances = new List<ServerInstance>();

        public static ServerInstance GetInstance ( int ID )
        {
            foreach(ServerInstance inst in Instances)
            {
                if (inst.ID == ID)
                    return inst;
            }
            return null;
        }

        public static void AddInstnace ( Server server, string desc, bool dieWhenEmpty )
        {
            CheckDeads();

            ServerInstance inst = new ServerInstance(server);
            Instances.Add(inst);
            inst.Description = desc;
            inst.destroyOnEmpty = dieWhenEmpty;
            inst.Start();
        }

        protected static void CheckDeads()
        {
            List<ServerInstance> corpses = new List<ServerInstance>();

            foreach(ServerInstance inst in Instances)
            {
                if (inst.Dead)
                    corpses.Add(inst);
            }

            foreach (ServerInstance inst in corpses)
                Instances.Remove(inst);
        }

        public static KeyValuePair<int,string>[] GetInstanceList ()
        {
            CheckDeads();

            List<KeyValuePair<int, string>> l = new List<KeyValuePair<int, string>>();
            foreach (ServerInstance inst in Instances)
                l.Add(new KeyValuePair<int, string>(inst.ID, inst.Description));

            return l.ToArray();
        }

        public static void StopAll()
        {
            CheckDeads();
            foreach (ServerInstance inst in Instances)
                inst.Kill();

            Instances.Clear();
        }
    }

    public class ServerInstance : IDisposable
    {
        public int ID = -1;
        public string Description = string.Empty;

        List<KeyValuePair<Client, MessageClass>> PendingMessages = new List<KeyValuePair<Client, MessageClass>>();

        List<Client> DeadCients = new List<Client>();
        List<Client> NewbornCients = new List<Client>();

        List<Client> PlayingClients = new List<Client>();

        protected Server server = null;
        protected Thread worker = null;

        Sim sim = new Sim();

        public bool destroyOnEmpty = false;
        bool hadAJoin = false;

        Stopwatch stopwatch = new Stopwatch();

        bool die = false;

        double lastUpdateTime = -1;

        public bool Dead
        {
            get { return worker == null; }
        }

        public ServerInstance ( Server s )
        {
            server = s;
            stopwatch.Start();

            sim.PlayerJoined += new PlayerJoinedHandler(sim_PlayerJoined);
            sim.PlayerRemoved += new PlayerRemovedHandler(sim_PlayerRemoved);
            sim.PlayerStatusChanged += new PlayerStatusChangeHandler(sim_PlayerStatusChanged);

            sim.Init();
        }

        public void AddMessage ( Client client, MessageClass message )
        {
            lock(PendingMessages)
            {
                PendingMessages.Add(new KeyValuePair<Client, MessageClass>(client, message));
            }
        }

        public void AddClient ( Client client )
        {
            lock(NewbornCients)
            {
                NewbornCients.Add(client);
                hadAJoin = true;
            }
        } 

        public Player AddPlayer ( Client client )
        {
            client.Player = sim.NewPlayer();
            client.Player.ID = client.UID;
            client.Player.Tag = client;
            sim.AddPlayer(client.Player);
            return client.Player;
        }

        public void RemoveClient ( Client client )
        {
            lock(DeadCients)
            {
                DeadCients.Add(client);
            }
        }

        public void Start ()
        {
            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        protected void Run()
        {
            while (!die)
            {
                ProcessMessages();
                Thread.Sleep(100);

                if (destroyOnEmpty && PlayingClients.Count == 0 && hadAJoin)
                    die = true;
            }

            worker = null;
        }

        public void Dispose()
        {
            Kill();
        }

        public void Kill ()
        {
            if (worker != null)
            {
                worker.Abort();
                worker = null;
            }
        }

        protected virtual void Think()
        {
            sim.Update(lastUpdateTime);
        }

        public void ProcessMessages ()
        {
            lastUpdateTime = stopwatch.ElapsedMilliseconds / 1000.0;

            // process new clients
            lock(NewbornCients)
            {
                foreach(Client newClient in NewbornCients)
                    PlayingClients.Add(newClient);
                NewbornCients.Clear();
            }

            // process dead clients
            lock(DeadCients)
            {
                foreach(Client deadClient in DeadCients)
                {
                    if (PlayingClients.Contains(deadClient))
                    {
                        if (deadClient.Player != null)
                            sim.RemovePlayer(deadClient.Player);
                        PlayingClients.Remove(deadClient);
                    }
                }
                DeadCients.Clear();
            }

            List<KeyValuePair<Client, MessageClass>> processingMessages = new List<KeyValuePair<Client, MessageClass>>();

            lock (PendingMessages)
            {
                foreach (KeyValuePair<Client, MessageClass> item in PendingMessages)
                    processingMessages.Add(item);
                PendingMessages.Clear();
            }

            foreach(KeyValuePair<Client, MessageClass> msg in processingMessages)
            {
                if (PlayingClients.Contains(msg.Key))
                    server.ProcessInstanceMessage(msg.Key, msg.Value, this);
            }

            Think();
        }

        void sim_PlayerStatusChanged(object sender, PlayerEventArgs args)
        {
            if (args.player.Status == PlayerStatus.Alive)
            {
                PlayerSpawn spawn = new PlayerSpawn(args.player);
                Broadcast(spawn);
            }
        }

        protected void sim_PlayerJoined ( object sender, PlayerEventArgs args )
        {           
            PlayerInfo info = new PlayerInfo(args.player);
            Broadcast(info);
        }

        protected void sim_PlayerRemoved(object sender, PlayerEventArgs args)
        {
            Client client = args.player.Tag as Client;
            if (client != null)
            {
                Messages.Disconnect disconnect = new Messages.Disconnect();
                disconnect.ID = client.Player.ID;
                Broadcast(disconnect);
            }
        }

        public void Broadcast ( MessageClass message )
        {
            foreach (Client client in PlayingClients)
                server.Send(client, message);
        }

        public void Spawn ( Player player )
        {
            sim.SpawnPlayer(player, lastUpdateTime);
        }
    }
}

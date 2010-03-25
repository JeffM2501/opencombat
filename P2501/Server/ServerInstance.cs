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
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

using Messages;
using Simulation;

namespace Project2501Server
{
    public class ServerInstanceSettings
    {
        public FileInfo MapFile;
        public bool DieWhenEmpty= false;
        public Simulation.SimSettings Settings = new Simulation.SimSettings();
        public String Description = string.Empty;
    }

    public class ServerInstanceManger
    {
        public static List<ServerInstance> Instances = new List<ServerInstance>();

        static List<int> MournedInstances = new List<int>();

        static int lastInstID = 0;

        public static int GetMournedInstance()
        {
            CheckDeads();
            if (MournedInstances.Count > 0)
            {
                int id = MournedInstances[0];
                MournedInstances.Remove(id);
                return id;
            }

            return -1;
        }

        public static ServerInstance GetInstance ( int ID )
        {
            foreach(ServerInstance inst in Instances)
            {
                if (inst.ID == ID)
                    return inst;
            }
            return null;
        }

        public static int Count
        {
            get
            {
                return Instances.Count;
            }
        }

        public static int AddInstnace ( Server server, ServerInstanceSettings settings )
        {
            CheckDeads();

            ServerInstance inst = new ServerInstance(server, settings);
            lastInstID++;
            inst.ID = lastInstID;
            Instances.Add(inst);
            inst.Description = settings.Description;
            inst.Start();
            return inst.ID;
        }

        protected static void CheckDeads()
        {
            List<ServerInstance> corpses = new List<ServerInstance>();

                foreach (ServerInstance inst in Instances)
                {
                    if (inst.Dead)
                        corpses.Add(inst);
                }

                foreach (ServerInstance inst in corpses)
                {
                    MournedInstances.Add(inst.ID);
                    Instances.Remove(inst);
                }
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

        ServerInstanceSettings settings = null;
        public ServerInstanceSettings Settings
        {
            get { return settings; }
        }

        List<KeyValuePair<Client, MessageClass>> PendingMessages = new List<KeyValuePair<Client, MessageClass>>();

        List<Client> DeadCients = new List<Client>();
        List<Client> NewbornCients = new List<Client>();

        List<Client> PlayingClients = new List<Client>();

        int WorldCacheFile = -1;

        protected Server server = null;
        protected Thread worker = null;

        Sim sim = null;

        public bool Valid 
        {
            get { return sim != null; }
        }

        bool hadAJoin = false;

        Stopwatch stopwatch = new Stopwatch();

        bool die = false;

        double lastUpdateTime = -1;

        public bool Dead
        {
            get { return worker == null; }
        }

        protected string[] GetTeamNames ( SimSettings settings )
        {
            if (settings.GameMode == GameType.DeathMatch || settings.GameMode == GameType.KillTheChicken)
                return null;

            if (settings.GameMode == GameType.TeamDeathMatch)
                return new string[4]{"Red","Blue","Yellow","Purple"};

            return new string[2]{"Red", "Blue" };
        }

        public ServerInstance ( Server s, ServerInstanceSettings serverSettings )
        {
            settings = serverSettings;
            server = s;
            stopwatch.Start();

            if (!settings.MapFile.Exists)
                return;

            sim = new Sim();
            sim.PlayerJoined += new PlayerJoinedHandler(sim_PlayerJoined);
            sim.PlayerRemoved += new PlayerRemovedHandler(sim_PlayerRemoved);
            sim.PlayerStatusChanged += new PlayerStatusChangeHandler(sim_PlayerStatusChanged);

            string[] teams = GetTeamNames(serverSettings.Settings);
            if (teams != null)
                sim.TeamNames = teams;

            sim.Init();
            sim.SetWorld(settings.MapFile);

            using (MemoryStream stream = new MemoryStream())
            {
                sim.World.Write(stream,true);
                WorldCacheFile = FileDownloadManager.ChacheFile(new MemoryStream(stream.ToArray()));
                stream.Close();
            }
            // save some ram
            sim.World.FlushLightmaps();
       }

        public bool ClientIsPlayer( Client client )
        {
            return PlayingClients.Contains(client);
        }

        public void SendMap ( Client client, int id )
        {
            foreach(FileTransfter file in FileDownloadManager.GetMessages(WorldCacheFile, id))
                server.Send(client, file);
        }

        public void SendSettings ( Client client )
        {
            InstanceSettings settings = new InstanceSettings();
            settings.ID = ID;
            settings.Settings = Settings.Settings;
            settings.MapChecksum = FileDownloadManager.GetFileChecksum(WorldCacheFile);
            settings.TeamNames = sim.TeamNames;
            server.Send(client, settings);

            // send player list

            foreach (Player player in sim.Players)
                server.Send(new PlayerInfo(player));

            server.Send(client,MessageClass.PlayerListDone);
            server.Send(client, MessageClass.AllowSpawn);
        }

        public void SetPlayerTeamPref ( Client client, int team )
        {
            Player player = client.Player;
            if (player == null || player.SpawnedOnce)
                return;
            player.TeamPreference = team;
        }

        public void SpawnPlayer ( Client client )
        {
            sim.SpawnPlayer(client.Player, -1);
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
            client.Player.ID = client.UID;
            client.Player.Tag = client;
            client.Player.TeamPreference = -1;
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

                if (ServerInstanceManger.Count > 1 && settings.DieWhenEmpty && PlayingClients.Count == 0 && hadAJoin)
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
                foreach (Client newClient in NewbornCients)
                {
                    PlayingClients.Add(newClient);
                    SendSettings(newClient);
                }
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
            Broadcast(new PlayerInfo(args.player));
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

        public void Spawn ( Client client )
        {
            if (client.Player == null || !sim.Players.Contains(client.Player))
                AddPlayer(client);

            sim.SpawnPlayer(client.Player, lastUpdateTime);
        }
    }
}

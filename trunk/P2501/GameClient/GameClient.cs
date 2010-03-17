using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Simulation;
using Clients;
using Messages;
using Lidgren.Network;

namespace P2501GameClient
{
    public delegate void MessageHandler(MessageClass message);

    public delegate void AuthenticationCallback(ref UInt64 UID, ref UInt64 CID, ref UInt64 Token);

    public class ServerVersionEventArgs : EventArgs
    {
        public int Major;
        public int Minor;
        public int Rev;

        public ServerVersionEventArgs (int major, int minor, int rev)
        {
            Major = major;
            Minor = minor;
            Rev = rev;
        }
    }

    public class PlayerEventArgs : EventArgs
    {
        public Player ThePlayer;

        public PlayerEventArgs(Player player)
        {
            ThePlayer = player;
        }
    }

    public class ChatEventArgs : EventArgs
    {
        public string Channel = string.Empty;
        public string From = string.Empty;
        public string Text = string.Empty;

        public ChatEventArgs(string channel, string from, string message)
        {
            Channel = channel;
            From = from;
            Text = message;
        }
    }

    public class HostConnectionEventArgs : EventArgs
    {
        public string Message = string.Empty;

        public HostConnectionEventArgs(string e)
        {
            Message = e;
        }
    }

    public delegate void ServerVersionHandler ( object sender, ServerVersionEventArgs args );
    public delegate void PlayerEventHandler(object sender, PlayerEventArgs args);
    public delegate void ChatEventHandler ( object sender, ChatEventArgs args );
    public delegate void HostConnectionHandler(object sender, HostConnectionEventArgs args);
    public delegate void GeneralEventHandler(object sender, EventArgs args);

    public partial class GameClient
    {
        public Sim sim = new Sim();
        public Player ThisPlayer = null;

        public event ServerVersionHandler ServerVersionEvent;
        public event PlayerEventHandler AllowSpawnEvent;
        public event ChatEventHandler ChatSentEvent;
        public event ChatEventHandler ChatReceivedEvent;

        public event HostConnectionHandler HostConnectionEvent;
        public event HostConnectionHandler HostDisconnectionEvent;

        public event GeneralEventHandler LoginAcceptEvent;

        public event GeneralEventHandler InstanceListEvent;
        public event GeneralEventHandler InstanceJoinedEvent;
        public event GeneralEventHandler InstanceJoinFailedEvent;

        public class InstanceDefinition
        {
            public int ID = -1;
            public string Description = string.Empty;
        }

        protected List<InstanceDefinition> AvailableInstances = new List<InstanceDefinition>();

        public InstanceDefinition[] ServerInstances
        {
            get { return AvailableInstances.ToArray(); }
        }

        public double Time
        {
            get { return lastUpdateTime; }
        }

        Client client;
        bool connected = false;

        bool requestedSpawn = false;

        public bool ConnectedToHost
        {
            get { return connected; }
        }

        public AuthenticationCallback GetAuthentication;

        MessageMapper messageMapper = new MessageMapper();

        double serverTimeOffset = 0;
        double lastUpdateTime = -1;
        Stopwatch stopwatch = new Stopwatch();

        bool haveSyncedTime = false;
        bool gotAllowSpawn = false;

        UInt64 lastPing = 0;
        double lastPingTime = -1;
        Dictionary<UInt64, double> OutStandingPings = new Dictionary<UInt64, double>();

        List<double> latencyList = new List<double>();

        double averageLatency = -1;
        double jitter = 0;
        double packetloss = 0;
        public static int LatencySamples = 5;
        public static double PingTime = 60;

        protected int ConnectedInstance = -1;
        public int Instance
        {
            get { return ConnectedInstance; }
        }

        public double AverageLatency
        {
            get { return averageLatency; }
        }

        public double LastLatency
        {
            get { if (latencyList.Count == 0) return 0; else return latencyList[latencyList.Count-1]; }
        }

        public double Jitter
        {
            get {return jitter;}
        }

        public double Packetloss
        {
            get { return packetloss; }
        }

        public GameClient ( string address, int port )
        {
            gotAllowSpawn = false;
            stopwatch.Start();
            InitMessageHandlers();
            lastPingTime = RawTime() + PingTime * 100;
            client = new Client(address, port);
        }

        public void Kill ()
        {
            client.Kill();
        }

        public bool Update ()
        {
            lastUpdateTime = Now();

            if (!connected && client.IsConnected)
            {
                if (HostConnectionEvent != null)
                    HostConnectionEvent(this, new HostConnectionEventArgs("Connected"));
            }

            if (connected)
            {
                if (!client.IsConnected)
                {
                    if (HostDisconnectionEvent != null)
                        HostDisconnectionEvent(this, new HostConnectionEventArgs("Disconnected"));
                    return false;
                }

                NetBuffer buffer = client.GetPentMessage();
                while (buffer != null)
                {
                    if (buffer.LengthBytes >= sizeof(Int32))
                    {
                        int name = buffer.ReadInt32();
                        MessageClass msg = messageMapper.MessageFromID(name);
                        msg.Unpack(ref buffer);

                        if (messageHandlers.ContainsKey(msg.GetType()))
                            messageHandlers[msg.GetType()](msg);
                    }
                    buffer = client.GetPentMessage();
                }

                sim.Update(lastUpdateTime);

                if (lastPingTime + PingTime < RawTime())
                    SendPing();
            }
            else
                connected = client.IsConnected;
            return true;
        }

        public double RawTime ()
        {
            return stopwatch.ElapsedMilliseconds * 0.001;
        }

        public double Now()
        {
            return RawTime() + serverTimeOffset;
        }

        public void RequestSpawn ()
        {
            if (requestedSpawn || ThisPlayer == null || ThisPlayer.Status != PlayerStatus.Despawned)
                return;

            requestedSpawn = true;
            RequestSpawn msg = new RequestSpawn();
            client.SendMessage(msg.Pack(), msg.Channel());
        }

        public void SendClockUpdate ( )
        {
            WhatTimeIsIt wti = new WhatTimeIsIt();
            lastPing++;
            wti.ID = lastPing;
            OutStandingPings.Add(lastPing,RawTime());
            client.SendMessage(wti.Pack(), wti.Channel());
            lastPingTime = RawTime();
        }

        public void SendPing()
        {
            Ping p = new Ping();
            lastPing++;
            p.ID = lastPing;
            OutStandingPings.Add(lastPing, RawTime());
            client.SendMessage(p.Pack(), p.Channel());
            lastPingTime = RawTime();
        }

        public void SendChat ( string channel, string message )
        {
            if (message == string.Empty)
                return;

            if (ChatSentEvent != null)
                ChatSentEvent(this, new ChatEventArgs(channel, ThisPlayer.Callsign, message));

            ChatMessage msg = new ChatMessage();
            msg.ChatChannel = channel;
            msg.Message = message;
            client.SendMessage(msg.Pack(), msg.Channel());
        }
    }
}

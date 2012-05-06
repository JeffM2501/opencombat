using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Game;
using Game.Messages;

using Lidgren.Network;

namespace Client
{
    public class PlayerInfo
    {
        public UInt64 UID = UInt64.MaxValue;
        public UInt64 PID = UInt64.MaxValue;

        public string Name = string.Empty;
        public int TeamID = -1;

        public ConnectInfo.OptionInfo[] Options = new ConnectInfo.OptionInfo[0];
        public int[] OptionChoices = new int[0];
    }

    public class ServerConnection
    {
        NetClient Client = null;
        GameState State = null;

        protected object locker = new object();

        public EventHandler<EventArgs> Connected;
        public EventHandler<EventArgs> Failed;
        public EventHandler<EventArgs> Disconnected;

        public EventHandler<EventArgs> StatusChanged;

        public class GameInfoEventArgs : EventArgs
        {
            public ConnectInfo Info = null;
            public GameInfoEventArgs(ConnectInfo info) : base()
            {
                Info = info;
            }
        }

        public EventHandler<GameInfoEventArgs> GameInfoLoaded;

        public ChatProcessor Chat = null;

        Thread worker = null;

        public class ScriptInfo
        {
            public string GameStyle = string.Empty;
            public string ScriptSet = string.Empty;
            public string ScriptHash = string.Empty;
        }

        public ScriptInfo ScriptingInfo = new ScriptInfo();

        PlayerInfo player = new PlayerInfo();
        public PlayerInfo Player
        {
            get
            {
                lock (player)
                    return player;
            }
        }

        public enum ConnectionStatus
        {
            New,
            Connecting,
            Loading,
            WaitOptions,
            Playing,
            Limboed,
            Disconnected,
        }
        ConnectionStatus conStatus;

        public ConnectionStatus Status { get { lock (locker)return conStatus; } protected set { lock (locker)conStatus = value; if (StatusChanged != null) StatusChanged(this, EventArgs.Empty); } }

        public Dictionary<int, TimeSyncMessage> OutstandingPings = new Dictionary<int, TimeSyncMessage>();
        public int LastPingID = 0;
        public double LastPing = -1;
        public double RepingTime = 30;

        public ServerConnection(string address, int port, GameState state)
        {
            State = state;
            RegisterMessageHandlers();

            NetPeerConfiguration config = new NetPeerConfiguration(GameMessage.ConnectionName);
            config.AutoFlushSendQueue = true;
            Client = new NetClient(config);

            HailMessage hail = new HailMessage();
            hail.RequestedName = "SlartyBartFast";

            Client.Start();
            Client.Connect(address, port, hail.Pack(Client.CreateMessage()));

            worker = new Thread(new ThreadStart(Run));
            worker.Start();

            ResourceProcessor.Connection = this;
            ResourceProcessor.ResourcesComplete += new EventHandler<EventArgs>(ResourceProcessor_ResourcesComplete);

            Chat = new ChatProcessor(this);
        }

        void ResourceProcessor_ResourcesComplete(object sender, EventArgs e)
        {
            Status = ConnectionStatus.Playing;
        }

        public void Kill()
        {
            Chat.Kill();
            ResourceProcessor.Kill();

            if (worker != null)
                worker.Abort();

            worker = null;

            if (Client != null)
                Client.Disconnect("Client Close");

            Client = null;
        }

        public void SendReliable(GameMessage message)
        {
            SendReliable(message, 1);
        }

        public void SendReliable(GameMessage message, int channel)
        {
            if (Client == null)
                return;

            lock (Client)
            {
                Client.SendMessage(message.Pack(Client.CreateMessage()), NetDeliveryMethod.ReliableOrdered, channel);
            }
        }

        public void SendUnreliable(GameMessage message)
        {
            SendUnreliable(message, 1);
        }

        public void SendUnreliable(GameMessage message, int channel)
        {
            if (Client == null)
                return;

            lock (Client)
            {
                Client.SendMessage(message.Pack(Client.CreateMessage()), NetDeliveryMethod.Unreliable, channel);
            }
        }

        protected void RegisterMessageHandlers()
        {
            GameMessage.AddMessageCallback(GameMessage.MessageCode.AnyUnhandled,AnyUnhandled);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ConnectInfo, ConnectionInfo);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ResourceResponce, ResourceResponce);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ChatText, ChatPacket);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ChatUserInfo, ChatPacket);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.TimeSync, TimeSync);
        }

        void AnyUnhandled(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {

        }

        void ChatPacket(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            Chat.ProcessChatMessage(messageData);
        }

        void ResourceResponce(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            if (Status != ConnectionStatus.Loading)
                Status = ConnectionStatus.Loading;

            ResourceProcessor.NewResponce(messageData as ResourceResponceMessage);
        }

        void TimeSync(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            double now = State.RawNow;

            TimeSyncMessage msg = messageData as TimeSyncMessage;
            if (msg == null)
                return;

            TimeSyncMessage ping = msg;
            if (OutstandingPings.ContainsKey(ping.Token))
            {
                ping = OutstandingPings[ping.Token];
                OutstandingPings.Remove(ping.Token);
            }

            double delta = now - ping.StartTime;
            double toServerTime = delta / 2;

            double myTimeAtServer = ping.StartTime + toServerTime;
            State.SetClockOffset(msg.ReplyTime - myTimeAtServer);
        }

        protected void SendClockUpdate()
        {
            TimeSyncMessage msg = new TimeSyncMessage();
            msg.Token = LastPingID++;
            msg.StartTime = State.RawNow;
            OutstandingPings.Add(msg.Token, msg);
            SendReliable(msg);
            LastPing = State.RawNow;
        }

        void ConnectionInfo(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            SendClockUpdate();

            ConnectInfo info = messageData as ConnectInfo;
            if (info == null)
            {
                if (Failed != null)
                    Failed(this, EventArgs.Empty);

                Die = true;
                return;
            }

            Player.UID = info.UID;
            Player.PID = info.PID;

            Player.Name = info.Name;

            Player.Options = info.Options.ToArray();
            List<int> temp = new List<int>();
            foreach (ConnectInfo.OptionInfo option in info.Options)
                temp.Add(option.Default);

            Player.OptionChoices = temp.ToArray();

            ScriptingInfo.GameStyle = info.GameStyle;
            ScriptingInfo.ScriptSet = info.ScriptPack;
            ScriptingInfo.ScriptHash = info.ScriptPackHash;

            Status = ConnectionStatus.Connecting;

            if (Connected != null)
                Connected(this,EventArgs.Empty);

            if (GameInfoLoaded != null)
                GameInfoLoaded(this, new GameInfoEventArgs(info));

             // ask for a list of resources with an empty request
            SendReliable(new ResourceRequestMessage());
        }

        public bool Die = false;

        public void Run()
        {
            Die = false;
            while (!Die)
            {
                NetIncomingMessage im = null;

                lock (Client)
                    im = Client.ReadMessage();

                while (im != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            if (im.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                if (Player.UID == UInt64.MaxValue)
                                {
                                    if (Failed != null)
                                        Failed(this, EventArgs.Empty);
                                }
                                else if (Disconnected != null)
                                    Disconnected(this, EventArgs.Empty);

                                Die = true;
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            GameMessage.Process(im);
                            break;
                    }
                    lock (Client)
                        im = Client.ReadMessage();
                }

                if (State.RawNow - LastPing > RepingTime)
                    SendClockUpdate();
                Thread.Sleep(10);
            }

            Client.Disconnect("Client Error");
            worker = null;
        }
    }
}

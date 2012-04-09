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

        public List<ConnectInfo.OptionInfo> Options = new List<ConnectInfo.OptionInfo>();
        public List<int> OptionChoices = new List<int>();
    }

    public class ServerConnection
    {
        NetClient Client = null;

        public EventHandler<EventArgs> Connected;
        public EventHandler<EventArgs> Failed;
        public EventHandler<EventArgs> Disconnected;

        Thread worker = null;

        PlayerInfo player = new PlayerInfo();

        public PlayerInfo Player
        {
            get
            {
                lock (player)
                    return player;
            }
        }

        public ServerConnection(string address, int port)
        {
            RegisterMessageHandlers();

            NetPeerConfiguration config = new NetPeerConfiguration(GameMessage.ConnectionName);
            config.AutoFlushSendQueue = true;
            Client = new NetClient(config);

            HailMessage hail = new HailMessage();
            hail.RequestedName = "SlartyBartFast";

            Client.Connect(address, port, hail.Pack(Client.CreateMessage()));

            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        public void Kill()
        {
            if (worker != null)
                worker.Abort();

            worker = null;

            if (Client != null)
                Client.Disconnect("Client Close");

            Client = null;
        }

        public void SendReliable(GameMessage message)
        {
            if (Client == null)
                return;

            lock (Client)
            {
                Client.SendMessage(message.Pack(Client.CreateMessage()), NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        public void SendUnreliable(GameMessage message)
        {
            if (Client == null)
                return;

            lock (Client)
            {
                Client.SendMessage(message.Pack(Client.CreateMessage()), NetDeliveryMethod.Unreliable, 1);
            }
        }

        protected void RegisterMessageHandlers()
        {
            GameMessage.AddMessageCallback(GameMessage.MessageCode.AnyUnhandled,AnyUnhandled);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ConnectInfo, ConnectionInfo);
        }

        void AnyUnhandled(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {

        }

        void ConnectionInfo(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
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

            Player.Options = info.Options;
            Player.OptionChoices = new List<int>;
            foreach (ConnectInfo.OptionInfo option in info.Options)
                Player.OptionChoices.Add(option.Default);

            if (Connected != null)
                Connected(this,EventArgs.Empty);
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
                Thread.Sleep(10);
            }

            Client.Disconnect("Client Error");
            worker = null;
        }
    }
}

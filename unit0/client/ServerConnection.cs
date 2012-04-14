﻿using System;
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

        protected object locker = new object();

        public EventHandler<EventArgs> Connected;
        public EventHandler<EventArgs> Failed;
        public EventHandler<EventArgs> Disconnected;

        public EventHandler<EventArgs> StatusChanged;

        Thread worker = null;

        public class ScriptInfo
        {
            public string GameStyle = string.Empty;
            public string ScriptSet = string.Empty;
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

        public ServerConnection(string address, int port)
        {
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

            ClientScripting.Script.Init(info.ScriptPack);

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

            Status = ConnectionStatus.Connecting;

            if (Connected != null)
                Connected(this,EventArgs.Empty);

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
                Thread.Sleep(10);
            }

            Client.Disconnect("Client Error");
            worker = null;
        }
    }
}

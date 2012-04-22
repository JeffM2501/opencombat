using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace GameInstance
{
    internal class ManagerConnection
    {
        NetClient Client = null;

        protected string Address = string.Empty;
        protected int Port = 0;

        protected bool Die = false;
        public void KillMe()
        {
            lock (Client)
            {
                Die = true;
                Client.Disconnect("killed");
            }
        }

        public ManagerConnection()
        {
            NetPeerConfiguration netConfig = new NetPeerConfiguration("ManagerConnection");

            netConfig.AutoFlushSendQueue = true;
            Client = new NetClient(netConfig);

            Port = 2503;
            string[] address = Program.Config.ManagerAddress.Split(":".ToCharArray());
            if (address.Length > 1)
                int.TryParse(address[1], out Port);

            Client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(GotMessage));

            Address = address[0];
            Client.Connect(Address, Port, Hail());
        }

        protected NetOutgoingMessage Hail()
        {
            NetOutgoingMessage hail = Client.CreateMessage();
            hail.Write("ManagerConnection: " + Program.Config.InstanceID.ToString());
            return hail;
        }

        public void Send(string text)
        {
            lock (Client)
                Client.SendMessage(Client.CreateMessage(text), NetDeliveryMethod.ReliableOrdered, 1);
        }

        protected List<string> InboundMessages = new List<string>();

        public string PopMessage()
        {
            lock (InboundMessages)
            {
                if (InboundMessages.Count > 0)
                {
                    string s = InboundMessages[0];
                    InboundMessages.RemoveAt(0);
                    return s;
                }
            }
            return string.Empty;
        }

        public void GotMessage(object peer)
        {
            NetIncomingMessage im = null;

            lock (Client)
                im = Client.ReadMessage();

            while (im != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        lock (InboundMessages)
                            InboundMessages.Add(im.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        if (im.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            lock (Client)
                            {
                                if (!Die)
                                {
                                    Client.Disconnect("reconnect");
                                    Client.Connect(Address, Port, Hail());
                                }
                            }
                        }
                        break;
                }
                lock (Client)
                    im = Client.ReadMessage();
            }
        }
    }
}

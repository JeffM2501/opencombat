using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Lidgren.Network;

namespace InstanceManager
{
    internal class InstanceConnection : IDisposable
    {
        NetServer Server = null;
        Thread worker = null;

        public delegate void InstanceEvent(UInt64 instance);

        public event InstanceEvent InstanceConnected;
        public event InstanceEvent InstanceDisconnected;
        public event InstanceEvent InstanceMessage;

        public InstanceConnection( InstanceManagerConfig config )
        {
            NetPeerConfiguration netConfig = new NetPeerConfiguration("ManagerConnection");
            netConfig.Port = config.InstanceManagePort;
            netConfig.MaximumConnections = config.MaxInstances + 2;

            Server = new NetServer(netConfig);

            worker = new Thread(new ThreadStart(Process));
            Server.Start();
        }

        public void Dispose()
        {
            Kill();
        }

        public void Kill()
        {
            if (worker != null)
                worker.Abort();

            worker = null;

            if (Server != null)
                Server.Shutdown("Kill;Manager_Kill");

            Server = null;
            InboundMessages.Clear();
            OutboundMessages.Clear();
        }

        protected bool Done = false;

        protected Dictionary<UInt64, List<string>> InboundMessages = new Dictionary<UInt64, List<string>>();

        public string[] PopMessages(UInt64 instance)
        {
            string[] list = new string[0];
            lock (InboundMessages)
            {
                if (InboundMessages.ContainsKey(instance))
                {
                    if (InboundMessages[instance].Count > 0)
                    {
                        list = InboundMessages[instance].ToArray();
                        InboundMessages[instance].Clear();
                    }
                }
            }
            return list;
        }

        protected void PushMessage(UInt64 instance, string message)
        {
            lock (InboundMessages)
            {
                if (!InboundMessages.ContainsKey(instance))
                    InboundMessages.Add(instance, new List<string>());

                InboundMessages[instance].Add(message);
            }
        }

        protected Dictionary<UInt64, List<string>> OutboundMessages = new Dictionary<UInt64, List<string>>();

        protected string[] GetOoutbound(UInt64 instance)
        {
            string[] list = new string[0];
            lock (OutboundMessages)
            {
                if (OutboundMessages.ContainsKey(instance))
                {
                    if (OutboundMessages[instance].Count > 0)
                    {
                        list = OutboundMessages[instance].ToArray();
                        OutboundMessages[instance].Clear();
                    }
                }
            }
            return list;
        }

        public void SendMessage(UInt64 instance, string message)
        {
            lock (OutboundMessages)
            {
                if (!OutboundMessages.ContainsKey(instance))
                    OutboundMessages.Add(instance, new List<string>());

                OutboundMessages[instance].Add(message);
            }
        }

        protected Dictionary<UInt64, NetConnection> Peers = new Dictionary<UInt64, NetConnection>();

        protected class PeerIDContainer
        {
            public UInt64 ID = 0;

            public PeerIDContainer(UInt64 i)
            {
                ID = i;
            }
        }
        
        protected void Process()
        {
            string thisIP = "127.0.0.1";

            while (!Done)
            {
                // check inbound

                NetIncomingMessage im = null;

                lock (Server)
                    im = Server.ReadMessage();

                while (im != null)
                {
                    UInt64 peerID = UInt64.MaxValue;

                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            if (im.SenderConnection.RemoteEndpoint.Address.ToString() != thisIP)
                            {
                                im.SenderConnection.Deny();
                                break;
                            }
                            lock (Peers)
                            {
                                string data = im.SenderConnection.RemoteHailMessage.ReadString();
                                if (data.Contains("ManagerConnection:"))
                                {
                                    string[] parts = data.Split(" ".ToCharArray());
                                    if (parts.Length > 1)
                                    {
                                        UInt64 peer = UInt64.MaxValue;
                                        UInt64.TryParse(parts[1], out peer);
                                        im.SenderConnection.Tag = new PeerIDContainer(peer);
                                        if (peer != UInt64.MaxValue && !Peers.ContainsKey(peer))
                                        {
                                            Peers.Add(peer, im.SenderConnection);

                                            if (InstanceConnected != null)
                                                InstanceConnected(peer);
                                            break;
                                        }
                                    }
                                }
                                im.SenderConnection.Deny("Invalid Hail");
                            }
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
							string reason = im.ReadString();
                            if (status == NetConnectionStatus.Disconnecting)
                            {
                                lock (Peers)
                                {
                                    peerID = (im.SenderConnection.Tag as PeerIDContainer).ID;
                                    if (!Peers.ContainsKey(peerID))
                                        break;

                                    Peers.Remove(peerID);

                                    if (InstanceDisconnected != null)
                                        InstanceDisconnected(peerID);
                                }
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            peerID = (im.SenderConnection.Tag as PeerIDContainer).ID;
                            lock (Peers)
                            {
                                if (!Peers.ContainsKey(peerID))
                                    break;
                            }

                            PushMessage(peerID, im.ReadString());
                            if (InstanceMessage != null)
                                InstanceMessage(peerID);
                            break;
                    }
                    im = Server.ReadMessage();
                }

                lock (Peers)
                {
                    // check outbound
                    foreach (KeyValuePair<UInt64, NetConnection> peer in Peers)
                    {
                        string[] messages = GetOoutbound(peer.Key);
                        foreach (string msg in messages)
                        {
                            NetOutgoingMessage message = null;
                            lock (Server)
                                message = Server.CreateMessage(msg);
                            lock(peer.Value)
                                peer.Value.SendMessage(message,NetDeliveryMethod.ReliableOrdered,1);
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}

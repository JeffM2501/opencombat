using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace GameInstance
{
    class ManagerConnection
    {
        NetClient Client = null;

        ManagerConnection()
        {
            NetPeerConfiguration netConfig = new NetPeerConfiguration("ManagerConnection");

            netConfig.AutoFlushSendQueue = true;
            Client = new NetClient(netConfig);

            int ManagerPort = 2503;
            string[] address = Program.Config.ManagerAddress.Split(":".ToCharArray());
            if (address.Length > 1)
                int.TryParse(address[1], out ManagerPort);

            Client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(GotMessage));

            NetOutgoingMessage hail = Client.CreateMessage();
            hail.Write("ManagerConnection: " + Program.Config.InstanceID.ToString());

            Client.Connect(address[0], ManagerPort);
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
                }
                lock (Client)
                    im = Client.ReadMessage();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Game;
using Game.Messages;
using Lidgren.Network;

namespace GameInstance
{
    class GameMessageProcessor
    {
        public GameState State;
        protected Thread worker;

        protected NetServer Server = null;

        public GameMessageProcessor(GameState state)
        {
            State = state;
            

            NetPeerConfiguration netConfig = new NetPeerConfiguration(GameMessage.ConnectionName);
            netConfig.Port = Program.Config.Port;
            netConfig.MaximumConnections = Program.Config.MaxPlayers + 2;

            Server = new NetServer(netConfig);
            Server.Start();

            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        public void Kill()
        {
            if (worker != null && worker.IsAlive)
                worker.Abort();

            worker = null;

            if (Server != null)
                Server.Shutdown("Host Shutdown");

            Server = null;
        }

        public void Run()
        {
            while (true)
            {
                NetIncomingMessage im = null;
                lock (Server)
                    im = Server.ReadMessage();
                while (im != null)
                {

                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:

                            HailMessage hail = GameMessage.Unpack(im.SenderConnection.RemoteHailMessage) as HailMessage;
                            if (hail == null || hail.Code != GameMessage.MessageCode.Hail)
                            {
                                im.SenderConnection.Deny();
                                break;
                            }
                            else
                            {
                                // add them
                                Player player = 
                            }
                            break;
                    }

                    lock (Server)
                        im = Server.ReadMessage();
                }

                Thread.Sleep(10);
            }
        }
    }
}

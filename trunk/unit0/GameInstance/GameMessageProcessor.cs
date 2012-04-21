using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

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
        protected ChatProcessor Chat = null;

        public GameMessageProcessor(GameState state)
        {
            State = state;
            RegisterMessageHandlers();

            NetPeerConfiguration netConfig = new NetPeerConfiguration(GameMessage.ConnectionName);
            netConfig.Port = Program.Config.Port;
            netConfig.LocalAddress = IPAddress.Any;
            netConfig.MaximumConnections = Program.Config.MaxPlayers + 2;

            Chat = new ChatProcessor();

            Server = new NetServer(netConfig);
            Server.Start();

            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        public void Kill()
        {
            ResourceProcessor.Kill();

            if (worker != null && worker.IsAlive)
                worker.Abort();

            worker = null;

            if (Server != null)
                Server.Shutdown("Host Shutdown");

            Server = null;
        }

        public NetOutgoingMessage NewMessage()
        {
            lock (Server)
                return Server.CreateMessage();
        }

        protected void RegisterMessageHandlers()
        {
            GameMessage.AddMessageCallback(GameMessage.MessageCode.AnyUnhandled, AnyUnhandled);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.OptionSelect, OptionSelect);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ChatUserInfo, ChatMessage);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ChatText, ChatMessage); 
            GameMessage.AddMessageCallback(GameMessage.MessageCode.StateChange, StateChange);
            GameMessage.AddMessageCallback(GameMessage.MessageCode.ResourceRequest, ResourceRequest);
        }

        void AnyUnhandled(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {

        }

        void OptionSelect(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            OptionSelectMessage msg = messageData as OptionSelectMessage;
            Player player = sender.Tag as Player;
            if (msg == null || player == null)
                return;

            OptionSelectMessage outMsg = new OptionSelectMessage();

            foreach (OptionSelectMessage.Selection option in msg.Selections)
            {
                if (Player.SetOption(player, option.ID, option.Pick))
                    outMsg.Selections.Add(option);
            }

            player.SendReliable(outMsg.Pack(NewMessage()));
        }

        void ChatMessage(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            Chat.AddMessage(messageData, sender.Tag as Player);
        }

        void StateChange(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {

        }

        void ResourceRequest(GameMessage.MessageCode code, GameMessage messageData, NetConnection sender)
        {
            ResourceRequestMessage req = messageData as ResourceRequestMessage;
            if (req == null)
                return;

            ResourceProcessor.AddReqest(req, sender.Tag as Player);
        }

        public void Run()
        {
            Console.WriteLine("Listening");
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
                            {
                                HailMessage hail = GameMessage.UnpackUnknown(im.SenderConnection.RemoteHailMessage) as HailMessage;
                                if (hail == null || hail.Code != GameMessage.MessageCode.Hail || hail.Magic != HailMessage.MagicMessage)
                                {
                                    im.SenderConnection.Deny();
                                    break;
                                }
                                else
                                    NewPlayerConnection(im.SenderConnection);
                            }
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            if (im.SenderConnection.Status == NetConnectionStatus.Disconnected)
                                PlayerDisconnect(im.SenderConnection.Tag as Player);
                            else if (im.SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                HailMessage hail = GameMessage.UnpackUnknown(im.SenderConnection.RemoteHailMessage) as HailMessage;
                                if (hail == null || hail.Code != GameMessage.MessageCode.Hail || hail.Magic != HailMessage.MagicMessage)
                                {
                                    im.SenderConnection.Deny();
                                    break;
                                }
                                else
                                    NewPlayerConnection(im.SenderConnection);
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            GameMessage.Process(im);
                            break;
                    }

                    lock (Server)
                        im = Server.ReadMessage();
                }

                Thread.Sleep(10);
            }
        }

        void NewPlayerConnection( NetConnection con)
        {
            // add them
            Player player = new Player(con,Server);
            player.PID = Player.NewPlayerID();

            con.Tag = player;

            // check token against one sent from master hail.Token = 
            // this should have come from the master, but fake it for now
            player.UID = player.PID;
            player.Name = "Player_" + player.UID.ToString();
            player.Options = new int[GameInfo.Info.UserOptions.Count];
            for (int i = 0; i < player.Options.Length; i++)
                player.Options[i] = GameInfo.Info.UserOptions[i].Default;

            Player.AddPlayer(player);

            ConnectInfo info = new ConnectInfo();
            info.GameStyle = GameInfo.Info.GameStyle;
            info.Options = GameInfo.Info.UserOptions;
            info.PlayerModels = GameInfo.Info.PlayerModels;
            info.PlayerAvatars = GameInfo.Info.PlayerAvatars;
            info.PID = player.PID;
            info.UID = player.UID;
            info.Name = player.Name;
            info.AvatarID = player.AvatarID;
            info.TeamID = -1;
            info.TeamName = string.Empty;
            info.ScriptPack = ServerScripting.Script.ScriptPackName;

            player.SendReliable(info.Pack(NewMessage()));

            Chat.SendNewPlayerChatInfo(info.PID);
            Chat.SendUserList(info.PID);
        }

        public void PlayerDisconnect(Player player)
        {
            if (player == null)
                return;
            lock (player)
            {
                player.Status = Player.PlayerStatus.Disconnecting;
                ServerScripting.Script.PlayerParted(player);
            }

            Player.RemovePlayer(player);
        }
    }
}

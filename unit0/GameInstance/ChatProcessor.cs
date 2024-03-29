﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Game;
using Game.Messages;

using Lidgren.Network;

namespace GameInstance
{
    public class ChatProcessor
    {
        Thread worker = null;

        public ChatProcessor()
        {
            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        public void Kill()
        {
            if (worker != null && worker.IsAlive)
                worker.Abort();

            worker = null;
        }

        protected class Message
        {
            public GameMessage message = null;
            public Player player = null;

            public Message(GameMessage m, Player p)
            {
                message = m;
                player = p;
            }
        }

        protected List<Message> InboundMessages = new List<Message>();

        public void AddMessage(GameMessage msg, Player sender)
        {
            if (sender == null)
                return;

            lock (InboundMessages)
                InboundMessages.Add(new Message(msg, sender));
        }

        protected Message PopMessage()
        {
            lock (InboundMessages)
            {
                if (InboundMessages.Count == 0)
                    return null;

                Message msg = InboundMessages[0];
                InboundMessages.RemoveAt(0);
                return msg;
            }
        }

        public void SendNewPlayerChatInfo( UInt64 playerID )
        {
            Player.SendToAllReliable(AddPlayerToInfo(new ChatUserInfoMessage(), playerID), 3);
        }

        public void ServerMessageToInstance( string text )
        {
            ChatTextMessage message = new ChatTextMessage();
            message.From = UInt64.MaxValue;
            message.To = UInt64.MaxValue;
            message.ChatType = ChatTextMessage.MessageType.Instance;
            message.Text = text;
            ComposeChatMessage(message);
        }

        public void ServerMessageToInstance(UInt64 from, string text)
        {
            ChatTextMessage message = new ChatTextMessage();
            message.From = from;
            message.To = UInt64.MaxValue;
            message.ChatType = ChatTextMessage.MessageType.Instance;
            message.Text = text;
            ComposeChatMessage(message);
        }


        public void ServerMessageToPlayer(string text, UInt64 UID)
        {
            ChatTextMessage message = new ChatTextMessage();
            message.From = UInt64.MaxValue;
            message.To = UID;
            message.ChatType = ChatTextMessage.MessageType.Personal;
            message.Text = text;
            ComposeChatMessage(message);
        }

        protected void ComposeChatMessage( ChatTextMessage msg)
        {
            ComposeChatMessage(msg, msg.From);
        }

        public class ChatMessageArgs : EventArgs
        {
            public ChatTextMessage message = null;
            public ChatMessageArgs(ChatTextMessage msg)
                : base()
            {
                message = msg;
            }
        }

        public event EventHandler<ChatMessageArgs> FilterChatMessage;

        protected void ComposeChatMessage( ChatTextMessage msg, UInt64 sender)
        {
            if (msg == null)
                return;
            msg.From = sender;

            if (FilterChatMessage != null)
                FilterChatMessage(this, new ChatMessageArgs(msg));

            List<UInt64> WhoGetsIt = new List<UInt64>();

            WhoGetsIt.Add(sender);

            if (msg.ChatType == ChatTextMessage.MessageType.Personal)
                WhoGetsIt.Add(msg.To);
            else if (msg.ChatType == ChatTextMessage.MessageType.Team)
            {
                foreach (UInt64 member in TeamInfo.GetTeam((int)msg.To).Members)
                    WhoGetsIt.Add(member);
            }
            else
            {
                foreach (UInt64 player in Player.UserIDList())
                {
                    if (player != sender)
                        WhoGetsIt.Add(player);
                }
            }

            foreach (UInt64 UID in WhoGetsIt)
            {
                // are they one of us
                Player recipient = Player.PlayerByUID(UID);
                if (recipient != null)
                    recipient.SendReliable(msg, 3);
                else
                {
                    // send that shit up to the manager
                }
            }
        }

        protected ChatUserInfoMessage AddPlayerToInfo(ChatUserInfoMessage message, UInt64 playerID)
        {
            ChatUserInfoMessage.Info user = new ChatUserInfoMessage.Info();

            Player player = Player.PlayerByPID(playerID);
            lock (player)
            {
                user.UID = player.UID;
                user.TeamName = player.Team.Name;
                user.AvatarID = player.AvatarID;
                user.Name = player.Name;
            }
            message.Users.Add(user);
            return message;
        }

        public void SendUserList(UInt64 playerID)
        {
            ChatUserInfoMessage message = new ChatUserInfoMessage();

            foreach (UInt64 pid in Player.PlayerIDList())
                AddPlayerToInfo(message, pid);

            Player player = Player.PlayerByPID(playerID);
            lock(player)
                player.SendReliable(message,3);
        }

        protected void Run()
        {
            while (true)
            {
                Message msg = PopMessage();
                while (msg != null)
                {
                    if (msg.message.Code == GameMessage.MessageCode.ChatUserInfo)
                        SendUserList(msg.player.UID);
                    else if (msg.message.Code == GameMessage.MessageCode.ChatText)
                    {
                        ChatTextMessage message = msg.message as ChatTextMessage;
                        if (message != null)
                            ComposeChatMessage(message, msg.player.UID);
                    }
                    msg = PopMessage();
                }
                Thread.Sleep(100);
            }
        }
    }
}

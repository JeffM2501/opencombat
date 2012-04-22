using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Game;
using Game.Messages;

namespace Client
{
    public class ChatProcessor
    {
        public class ChatUser
        {
            public string Name = string.Empty;
            public UInt64 UID = UInt64.MaxValue;
            public int AvatarID = -1;

            public static ChatUser Empty = new ChatUser();
        }

        protected Dictionary<UInt64, ChatUser> ChatUsers = new Dictionary<UInt64, ChatUser>();
        protected Dictionary<string, UInt64> ChatUserNameLookup = new Dictionary<string, UInt64>();

        public UInt64[] ChatUserList()
        {
            lock (ChatUsers)
                return ChatUsers.Keys.ToArray();
        }

        public ChatUser GetUserInfo(UInt64 uid)
        {
            lock (ChatUsers)
            {
                if (ChatUsers.ContainsKey(uid))
                    return ChatUsers[uid];
            }
            return ChatUser.Empty;
        }

        public UInt64 FindChatUser(string name)
        {
            lock (ChatUsers)
            {
                if (ChatUserNameLookup.ContainsKey(name))
                    return ChatUserNameLookup[name];
            }
            return UInt64.MaxValue;
        }

        public class ChatUserArgs : EventArgs
        {
            public ChatUser User = null;
            public ChatUserArgs( ChatUser user) : base ()
            {
                User = user;
            }
        }

        public class ChatMessageArgs : EventArgs
        {
            public UInt64 From = UInt64.MaxValue;
            public UInt64 To = UInt64.MaxValue;

            public ChatTextMessage.MessageType MessageType = ChatTextMessage.MessageType.Instance;
            public string Text = string.Empty;

            public ChatMessageArgs(ChatTextMessage msg)
                : base()
            {
                From = msg.From;
                To = msg.To;
                MessageType = msg.ChatType;
                Text = msg.Text;
            }
        }

        public event EventHandler<ChatUserArgs> NewChatUser;
        public event EventHandler<ChatMessageArgs> FilterChatMessage;
        public event EventHandler<ChatMessageArgs> RecivedChatMessage;

        public event EventHandler<EventArgs> OutboundChatChanged;

        protected List<GameMessage> PendingMessages = new List<GameMessage>();

        protected string OutgoingChatLine = string.Empty;
        public string OutboundChatLine { get { lock (OutgoingChatLine)return (string)OutgoingChatLine.Clone(); } }

        public void AddOutboundChat(string text)
        {
            if (text == string.Empty)
                return;

            lock (OutgoingChatLine)
                OutgoingChatLine += text;

            if (OutboundChatChanged != null)
                OutboundChatChanged(this, EventArgs.Empty);
        }

        public void ClearOutboundChat()
        {
            lock (OutgoingChatLine)
                OutgoingChatLine = "";

            if (OutboundChatChanged != null)
                OutboundChatChanged(this, EventArgs.Empty);
        }

        protected ServerConnection Connection = null;

        public ChatProcessor(ServerConnection connection)
        {
            Connection = connection;
        }

        public void Kill()
        {
            if (worker != null && worker.IsAlive)
                worker.Abort();

            worker = null;
        }

        public void SendChatMessage(UInt64 UID, ChatTextMessage.MessageType messageType, string text)
        {
            ChatTextMessage msg = new ChatTextMessage();
            msg.To = UID;
            msg.ChatType = messageType;
            msg.Text = text;

            Connection.SendReliable(msg, 3);
        }

        public void SendChatMessage(string to, ChatTextMessage.MessageType messageType, string text)
        {
            lock (ChatUsers)
            {
                if (ChatUserNameLookup.ContainsKey(to))
                    SendChatMessage(ChatUserNameLookup[to], messageType, text);
            }
        }

        public void ProcessChatMessage(GameMessage message)
        {
            lock (PendingMessages)
                PendingMessages.Add(message);

            CheckThread();
        }

        protected GameMessage PopMessage()
        {
            lock (PendingMessages)
            {
                if (PendingMessages.Count == 0)
                    return null;

                GameMessage msg = PendingMessages[0];
                PendingMessages.RemoveAt(0);
                return msg;
            }
        }

        protected Thread worker = null;

        protected void CheckThread()
        {
            if (worker != null && worker.IsAlive)
                return;

            worker = new Thread(new ThreadStart(Run));
            worker.Start();
        }

        protected void Run()
        {
            GameMessage msg = PopMessage();
            while (msg != null)
            {
                if (msg.Code == GameMessage.MessageCode.ChatUserInfo)
                {
                    ChatUserInfoMessage newUsers = msg as ChatUserInfoMessage;

                    foreach (ChatUserInfoMessage.Info info in newUsers.Users)
                    {
                        lock (ChatUsers)
                        {
                            if (ChatUsers.ContainsKey(info.UID))
                            {
                                ChatUserNameLookup.Remove(ChatUsers[info.UID].Name);
                                ChatUsers[info.UID].Name = info.Name;
                                ChatUsers[info.UID].AvatarID = info.AvatarID;
                            }
                            else
                            {
                                ChatUser user = new ChatUser();
                                user.AvatarID = info.AvatarID;
                                user.Name = info.Name;
                                user.UID = info.UID;

                                ChatUsers.Add(user.UID, user);
                            }

                            ChatUserNameLookup.Add(info.Name,info.UID);
                        }
                        if (NewChatUser != null)
                            NewChatUser(this, new ChatUserArgs(GetUserInfo(info.UID)));
                    }
                }
                else if (msg.Code == GameMessage.MessageCode.ChatText)
                {
                    ChatTextMessage chatMessage = msg as ChatTextMessage;

                    ChatMessageArgs args = new ChatMessageArgs(chatMessage);
                    if (FilterChatMessage != null)
                        FilterChatMessage(this, args);

                    if (RecivedChatMessage != null)
                        RecivedChatMessage(this, args);
                }
                msg = PopMessage();
            }
        }
    }
}

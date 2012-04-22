using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace Game.Messages
{
    public class ChatUserInfoMessage : GameMessage
    {
        public class Info
        {
            public string Name = string.Empty;
            public string TeamName = string.Empty;
            public int AvatarID = -1;

            public UInt64 UID = UInt64.MaxValue;

            public Info()
            {
            }

            public Info(NetIncomingMessage msg)
            {
                Unpack(msg);
            }

            public void Pack(NetOutgoingMessage msg)
            {
                msg.Write(Name);
                msg.Write(TeamName);
                msg.Write(UID);
                msg.Write(AvatarID);
            }

            public void Unpack(NetIncomingMessage msg)
            {
                Name = msg.ReadString();
                TeamName = msg.ReadString();
                UID = msg.ReadUInt64();
                AvatarID = msg.ReadInt32();
            }
        }

        public List<Info> Users = new List<Info>();

        public ChatUserInfoMessage()
        {
            Code = GameMessage.MessageCode.ChatUserInfo;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
            outMsg.Write(Users.Count);
            foreach (Info info in Users)
                info.Pack(outMsg);
            return outMsg;
        }

        public override void Unpack(Lidgren.Network.NetIncomingMessage msg)
        {
            base.Unpack(msg);
            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                Users.Add(new Info(msg));
        }
    }

    public class ChatTextMessage : GameMessage
    {
        public UInt64 ID = 0;

        public UInt64 From = UInt64.MaxValue;
        public UInt64 To = UInt64.MaxValue;
 
        public enum MessageType
        {
            Personal,
            Team,
            Instance,
            Network,
        }

        public MessageType ChatType = MessageType.Personal;

        public string Text = string.Empty;

        public ChatTextMessage()
        {
            Code = GameMessage.MessageCode.ChatText;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
            outMsg.Write(ID);
            outMsg.Write(From);
            outMsg.Write(To);
            outMsg.Write((Int32)ChatType);
            outMsg.Write(Text);
            return outMsg;
        }

        public override void Unpack(Lidgren.Network.NetIncomingMessage msg)
        {
            base.Unpack(msg);
            ID = msg.ReadUInt64(); 
            From = msg.ReadUInt64();
            To = msg.ReadUInt64();
            ChatType = (MessageType)Enum.ToObject(typeof(MessageType),msg.ReadInt32());
            Text = msg.ReadString();
        }
    }
}

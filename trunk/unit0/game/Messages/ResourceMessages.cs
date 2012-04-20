using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace Game.Messages
{
    public class ResourceRequestMessage : GameMessage
    {
        public static string MapResourceName = "WorldMap";

        public List<string> ResourceNames = new List<string>();

        public ResourceRequestMessage()
        {
            Code = GameMessage.MessageCode.ResourceRequest;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);

            outMsg.Write(ResourceNames.Count);
            foreach (string name in ResourceNames)
                outMsg.Write(name);
            return outMsg;
        }

        public override void Unpack(NetIncomingMessage msg)
        {
            base.Unpack(msg);

            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                ResourceNames.Add(msg.ReadString());
        }
    }

    public class ResourceResponceMessage : GameMessage
    {
        public class Resource
        {
            public enum ResourceType
            {
                Unknown,
                Map,
                Script
            }
            public ResourceType ResType = ResourceType.Unknown;

            public string Name = string.Empty;
            public string Hash = string.Empty;
            public string URL = string.Empty;
            public byte[] data = new byte[0];

            public Resource()
            {
            }

            public Resource(NetIncomingMessage msg)
            {
                Unpack(msg);
            }

            public void Pack(NetOutgoingMessage msg)
            {
                msg.Write(Name);
                msg.Write(Hash);
                msg.Write(URL);
                msg.Write((byte)ResType);

                msg.Write(data.Length);
                if (data.Length > 0)
                    msg.Write(data);
            }

            public void Unpack(NetIncomingMessage msg)
            {
                Name = msg.ReadString();
                Hash = msg.ReadString();
                URL = msg.ReadString();
                ResType = (ResourceType)Enum.ToObject(typeof(ResourceType), msg.ReadByte());

                int size = msg.ReadInt32();
                if (size > 0)
                    data = msg.ReadBytes(size);
            }
        }

        public List<Resource> Resources = new List<Resource>();

        public ResourceResponceMessage()
        {
            Code = GameMessage.MessageCode.ResourceResponce;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);

            outMsg.Write(Resources.Count);
            foreach (Resource rez in Resources)
                rez.Pack(outMsg);
            return outMsg;
        }

        public override void Unpack(NetIncomingMessage msg)
        {
            base.Unpack(msg);

            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                Resources.Add(new Resource(msg));
        }
    }
}

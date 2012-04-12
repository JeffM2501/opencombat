using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Messages
{
    public class ResourceRequestMessage : GameMessage
    {
        public List<string> ResourceNames = new List<string>();

        public ResourceRequestMessage()
        {
            Code = GameMessage.MessageCode.ResourceRequest;
        }
    }

    public class ResourceResponceMessage : GameMessage
    {
        public class Resource
        {
            public string Name = string.Empty;
            public string Hash = string.Empty;
            public string URL = string.Empty;
            public byte[] data = new byte[0];
        }

        public List<Resource> Resources = new List<Resource>();

        public ResourceResponceMessage()
        {
            Code = GameMessage.MessageCode.ResourceResponce;
        }
    }
}

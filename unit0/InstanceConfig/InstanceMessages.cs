using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Lidgren.Network;

namespace InstanceConfig
{
    public class InstanceMessage
    {
        public enum MessageCode
        {
            Null = 0,
            Connect,
        }

        public MessageCode Code = MessageCode.Null;

        public static Dictionary<MessageCode, Type> MessageClasses = new Dictionary<MessageCode, Type>();

        public delegate void MessageReceived ( MessageCode code, InstanceMessage messageData );

        public static Dictionary<Type, List<MessageReceived>> ReceivedCallbacks = new Dictionary<Type, List<MessageReceived>>();

        public static void Process(NetIncomingMessage msg)
        {
            InstanceMessage messageData = Unpack(msg);
            if (messageData != null && ReceivedCallbacks.ContainsKey(messageData.GetType()))
            {
                foreach (MessageReceived cb in ReceivedCallbacks[messageData.GetType()])
                    cb(messageData.Code, messageData);
            }
        }

        public static void Register()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(InstanceMessage)))
                {
                    MessageCode code = ((InstanceMessage)Activator.CreateInstance(t)).Code;
                    if (!MessageClasses.ContainsKey(code))
                        MessageClasses.Add(code, t);
                }
            }
        }

        public NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            if (MessageClasses.Count ==- 0)
                Register();

            msg.Write((byte)Code);
            msg.WriteAllFields(this);
            return msg;
        }

        public static InstanceMessage Unpack(NetIncomingMessage msg)
        {
            byte c = msg.ReadByte();
            MessageCode code = (MessageCode)Enum.ToObject(typeof(MessageCode),c);

            if (MessageClasses.ContainsKey(code))
            {
                InstanceMessage dataClass = (InstanceMessage)Activator.CreateInstance(MessageClasses[code]);
                msg.ReadAllFields(dataClass);
                return dataClass;
            }
            return null;
        }
    }

    public class InstanceConnect
    {
    }
}

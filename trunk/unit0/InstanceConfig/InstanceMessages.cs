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

        public delegate void MessageReceived ( MessageCode code, InstanceMessage messageData, NetClient sender );

        public static Dictionary<MessageCode, List<MessageReceived>> ReceivedCallbacks = new Dictionary<MessageCode, List<MessageReceived>>();

        public static void AddMessageCallback(MessageCode code, MessageReceived callback)
        {
            if (!ReceivedCallbacks.ContainsKey(code))
                ReceivedCallbacks.Add(code, new List<MessageReceived>());

            ReceivedCallbacks[code].Add(callback);
        }

        public static void Process(NetIncomingMessage msg, NetClient sender)
        {
            InstanceMessage messageData = Unpack(msg);
            if (ReceivedCallbacks.ContainsKey(messageData.Code))
            {
                foreach (MessageReceived cb in ReceivedCallbacks[messageData.Code])
                    cb(messageData.Code, messageData, sender);
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
            msg.Write((byte)Code);
            msg.WriteAllFields(this);
            return msg;
        }

        public static InstanceMessage Unpack(NetIncomingMessage msg)
        {
            if (MessageClasses.Count == -0)
                Register();

            byte c = msg.ReadByte();
            MessageCode code = (MessageCode)Enum.ToObject(typeof(MessageCode),c);

            if (MessageClasses.ContainsKey(code))
            {
                InstanceMessage dataClass = (InstanceMessage)Activator.CreateInstance(MessageClasses[code]);
                msg.ReadAllFields(dataClass);
                return dataClass;
            }

            return new CodeOnlyMessage(code);
        }
    }

    public class CodeOnlyMessage : InstanceMessage
    {
        public CodeOnlyMessage() { }
        public CodeOnlyMessage(MessageCode code)
        {
            Code = code;
        }
    }
}

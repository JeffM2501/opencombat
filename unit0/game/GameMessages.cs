﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using Lidgren.Network;

namespace Game
{
    public class GameMessage
    {
        public static string ConnectionName = "Unit0GameConnection";
        public static string ConnectionMagic = ConnectionName + ":1";

        public enum MessageCode
        {
            Null = 0,
            Hail,
            ConnectInfo,
            OptionSelect,

            TimeSync,

            ResourceRequest,
            ResourceResponce,

            StateChange,

            ChatUserInfo,
            ChatText,

            ScoreInfo,

            ActorUpdate,
            ActorInput,

            Quit,

            AnyUnhandled,
        }

        public MessageCode Code = MessageCode.Null;

        public static Dictionary<MessageCode, Type> MessageClasses = new Dictionary<MessageCode, Type>();

        public delegate void MessageReceived(MessageCode code, GameMessage messageData, NetConnection sender);

        public static Dictionary<MessageCode, List<MessageReceived>> ReceivedCallbacks = new Dictionary<MessageCode, List<MessageReceived>>();

        public static void AddMessageCallback(MessageCode code, MessageReceived callback)
        {
            if (!ReceivedCallbacks.ContainsKey(code))
                ReceivedCallbacks.Add(code, new List<MessageReceived>());

            ReceivedCallbacks[code].Add(callback);
        }

        protected static void CallHandlers(GameMessage messageData, NetConnection sender)
        {
            
        }
        public static void Process(NetIncomingMessage msg)
        {
            GameMessage messageData = UnpackUnknown(msg);
            if (ReceivedCallbacks.ContainsKey(messageData.Code))
            {
                foreach (MessageReceived cb in ReceivedCallbacks[messageData.Code])
                    cb(messageData.Code, messageData, msg.SenderConnection);
            }
            else
            {
                if (ReceivedCallbacks.ContainsKey(GameMessage.MessageCode.AnyUnhandled))
                {
                    foreach (MessageReceived cb in ReceivedCallbacks[GameMessage.MessageCode.AnyUnhandled])
                        cb(messageData.Code, messageData, msg.SenderConnection);
                }
            }
        }

        public static void RegisterMessageClasses()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(GameMessage)))
                {
                    MessageCode code = ((GameMessage)Activator.CreateInstance(t)).Code;
                    if (!MessageClasses.ContainsKey(code))
                        MessageClasses.Add(code, t);
                }
            }
        }

        public virtual NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            msg.Write((byte)Code);
            return msg;
        }

        public static GameMessage UnpackUnknown(NetIncomingMessage msg)
        {
            if (MessageClasses.Count == 0)
                RegisterMessageClasses();

            byte c = msg.ReadByte();
            MessageCode code = (MessageCode)Enum.ToObject(typeof(MessageCode),c);

            if (MessageClasses.ContainsKey(code))
            {
                GameMessage dataClass = (GameMessage)Activator.CreateInstance(MessageClasses[code]);
                dataClass.Unpack(msg);
                return dataClass;
            }

            return new CodeOnlyMessage(code);
        }

        public virtual void Unpack(NetIncomingMessage msg)
        {

        }
    }

    public class CodeOnlyMessage : GameMessage
    {
        public CodeOnlyMessage() { }
        public CodeOnlyMessage(MessageCode code)
        {
            Code = code;
        }
    }
}

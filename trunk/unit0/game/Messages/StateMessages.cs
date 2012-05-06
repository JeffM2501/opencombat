using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using OpenTK;

namespace Game.Messages
{
    public class StateChangeMessage : GameMessage
    {
        public enum PlayerState
        {
            Loading,
            Active,
            Dead,
            Limbo,
        }

        public PlayerState State = PlayerState.Loading;
        public UInt64 ID = UInt64.MaxValue;

        public ActorUpdateMesage Update = null;

        public StateChangeMessage()
        {
            Code = GameMessage.MessageCode.StateChange;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
            outMsg.Write(ID);
            outMsg.Write((Int32)State);
            if (Update == null)
                outMsg.Write(false);
            else
                outMsg = Update.Pack(outMsg);
            return outMsg;
        }

        public override void Unpack(Lidgren.Network.NetIncomingMessage msg)
        {
            base.Unpack(msg);
            ID = msg.ReadUInt64(); 
            State = (PlayerState)Enum.ToObject(typeof(PlayerState), msg.ReadInt32());
            bool hasUpdate = msg.ReadBoolean();
            if (hasUpdate)
            {
                Update = new ActorUpdateMesage();
                Update.Unpack(msg);
            }
            else
                Update = null;
        }
    }

    public class ActorUpdateMesage : GameMessage
    {
        public Vector3 Postion = Vector3.Zero;
        public Vector3 Motion = Vector3.Zero;
        public Vector3 Orientation = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;

        public ActorUpdateMesage()
        {
            Code = GameMessage.MessageCode.ActorUpdate;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
           
            return outMsg;
        }

        public override void Unpack(Lidgren.Network.NetIncomingMessage msg)
        {
            base.Unpack(msg);
        }
    }
}

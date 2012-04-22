using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace Game.Messages
{
    public class HailMessage : GameMessage
    {
        public static string MagicMessage = "Unit0Proto1";

        public string Magic = MagicMessage;
        public string RequestedName = string.Empty;
        public string Token = string.Empty;

        public HailMessage()
        {
            Code = GameMessage.MessageCode.Hail;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
            outMsg.Write(Magic);
            outMsg.Write(RequestedName);
            outMsg.Write(Token);
            return outMsg;
        }

        public override void Unpack(Lidgren.Network.NetIncomingMessage msg)
        {
            base.Unpack(msg);
            Magic = msg.ReadString();
            RequestedName = msg.ReadString();
            Token = msg.ReadString();
        }
    }

    public class ConnectInfo : GameMessage
    {
        public UInt64 UID = 0;
        public UInt64 PID = 0;
        public string Name = string.Empty;

        public int TeamID = -1;
        public string TeamName = string.Empty;

        public string GameStyle = string.Empty;
        public string ScriptPack = string.Empty;
        public string ScriptPackHash = string.Empty;

        public int AvatarID = -1;

        public class OptionInfo
        {
            public string Name = string.Empty;
            public int Default = -1;
            public List<string> Options = new List<string>();

            public OptionInfo()
            {
            }

            public OptionInfo(NetIncomingMessage msg)
            {
                Unpack(msg);
            }

            public void Pack(NetOutgoingMessage msg)
            {
                msg.Write(Name);
                msg.Write(Default);
                msg.Write(Options.Count);
                foreach (string option in Options)
                    msg.Write(option);
            }

            public void Unpack(NetIncomingMessage msg)
            {
                Name = msg.ReadString();
                Default = msg.ReadInt32();
                int count = msg.ReadInt32();
                for (int i = 0; i < count; i++)
                    Options.Add(msg.ReadString());
            }
        }

        public List<OptionInfo> Options = new List<OptionInfo>();

        public List<string> PlayerModels = new List<string>();
        public List<string> PlayerAvatars = new List<string>();

        public ConnectInfo()
        {
            Code = GameMessage.MessageCode.ConnectInfo;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);
            outMsg.Write(UID);
            outMsg.Write(PID);
            outMsg.Write(Name);
            outMsg.Write(AvatarID);

            outMsg.Write(TeamID);
            outMsg.Write(TeamName);

            outMsg.Write(GameStyle);
            outMsg.Write(ScriptPack);
            outMsg.Write(ScriptPackHash);

            outMsg.Write(Options.Count);
            foreach (OptionInfo option in Options)
                option.Pack(outMsg);

            outMsg.Write(PlayerModels.Count);
            foreach (string a in PlayerModels)
                outMsg.Write(a);

            outMsg.Write(PlayerAvatars.Count);
            foreach (string a in PlayerAvatars)
                outMsg.Write(a);

            return outMsg;
        }

        public override void Unpack(NetIncomingMessage msg)
        {
            base.Unpack(msg);

            UID = msg.ReadUInt64();
            PID = msg.ReadUInt64();
            Name = msg.ReadString();
            AvatarID = msg.ReadInt32();

            TeamID = msg.ReadInt32();
            TeamName = msg.ReadString();;

            GameStyle = msg.ReadString();
            ScriptPack = msg.ReadString();
            ScriptPackHash = msg.ReadString();

            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                Options.Add(new OptionInfo(msg));

            count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                PlayerModels.Add(msg.ReadString());

            count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                PlayerAvatars.Add(msg.ReadString());
        }
    }

    public class OptionSelectMessage : GameMessage
    {
        public class Selection
        {
            public int ID = -1;
            public int Pick = -1;

            public Selection()
            {
            }

            public Selection(NetIncomingMessage msg)
            {
                Unpack(msg);
            }

            public void Pack(NetOutgoingMessage msg)
            {
                msg.Write(ID);
                msg.Write(Pick);
            }

            public void Unpack(NetIncomingMessage msg)
            {
                ID = msg.ReadInt32();
                Pick = msg.ReadInt32();
            }
        }

        public List<Selection> Selections = new List<Selection>();
        
        public OptionSelectMessage()
        {
            Code = GameMessage.MessageCode.OptionSelect;
        }

        public override NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            NetOutgoingMessage outMsg = base.Pack(msg);


            outMsg.Write(Selections.Count);
            foreach (Selection sel in Selections)
                sel.Pack(outMsg);
            return outMsg;
        }

        public override void Unpack(NetIncomingMessage msg)
        {
            base.Unpack(msg);

            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
                Selections.Add(new Selection(msg));
        }
    }
}

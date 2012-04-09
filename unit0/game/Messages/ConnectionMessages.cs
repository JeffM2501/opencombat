﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public class OptionInfo
        {
            public string Name = string.Empty;
            public int Default = -1;
            public List<string> Options = new List<string>();
        }

        public List<OptionInfo> Options = new List<OptionInfo>();

        public List<string> Avatars = new List<string>();

        public ConnectInfo()
        {
            Code = GameMessage.MessageCode.ConnectInfo;
        }
    }

    public class OptionSelectMessage : GameMessage
    {
        public class Selection
        {
            public int ID = -1;
            public int Pick = -1;
        }

        public List<Selection> Selections = new List<Selection>();
        
        public OptionSelectMessage()
        {
            Code = GameMessage.MessageCode.OptionSelect;
        }
    }
}

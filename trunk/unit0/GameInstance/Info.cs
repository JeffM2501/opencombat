using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Messages;

namespace GameInstance
{
    public class GameInfo
    {
        public static GameInfo Info = new GameInfo(); 
        
        public List<ConnectInfo.OptionInfo> UserOptions = new List<ConnectInfo.OptionInfo>();
        public string GameStyle = string.Empty;

        public ConnectInfo.OptionInfo NewOption(string name, int defaultValue)
        {
            ConnectInfo.OptionInfo opt = new ConnectInfo.OptionInfo();
            opt.Name = name;
            opt.Default = defaultValue;
            UserOptions.Add(opt);
            return opt;
        }
    }
}

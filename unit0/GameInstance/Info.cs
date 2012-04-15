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

        public List<string> PlayerModels = new List<string>();
        public List<string> PlayerAvatars = new List<string>();

        public double DefaultLinSpeed = 10;
        public double DefaultRotSpeed = 90;
        public double DefaultGravity = -10;

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

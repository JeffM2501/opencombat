using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class TeamInfo
    {
        public static TeamInfo Empty = new TeamInfo();

        public int ID = -1;
        public string Name = string.Empty;

        public List<UInt64> Members = new List<UInt64>();

        protected static List<TeamInfo> Teams = new List<TeamInfo>();

        public static TeamInfo GetTeam(int id)
        {
            lock (Teams)
            {
                if (id > 0 && id < Teams.Count())
                    return Teams[id];
            }
            return TeamInfo.Empty;
        }
    }
}

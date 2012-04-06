using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Game;

namespace GameInstance
{
    class Player
    {
        public delegate void PlayerEvent(Player player);
        public static event PlayerEvent NewPlayer;
        public static event PlayerEvent RemovePlayer;

        public UInt64 UID = UInt64.MaxValue;
        public UInt64 PID = UInt64.MaxValue;

        public string Name = string.Empty;

        public NetClient Connection = null;
        public TeamInfo Team = TeamInfo.Empty;
        public RemotePlayer SimPlayer = null;

        public static Player Empty = new Player(null);

        public Player(NetClient con)
        {
            Connection = con;
        }

        protected static Dictionary<UInt64, Player> Players = new Dictionary<UInt64, Player>();
        protected static Dictionary<UInt64, Player> PlayersUID = new Dictionary<UInt64, Player>();

        protected static int LastPlayerID = 0;
        public static int NewPlayerID()
        {
            lock (Players)
            {
                LastPlayerID++;
                return LastPlayerID;
            }
        }

        public static Player PlayerByPID(UInt64 id)
        {
            lock (Players)
            {
                if (Players.ContainsKey(id))
                    return Players[id];
            }
            return Player.Empty;
        }

        public static Player PlayerByUID(UInt64 id)
        {
            lock (Players)
            {
                if (PlayersUID.ContainsKey(id))
                    return PlayersUID[id];
            }

            return Player.Empty;
        }

        public static Player PlayerByConnection(NetClient con)
        {
            lock (Players)
            {
                foreach (Player p in Players.Values)
                {
                    if (p.Connection == con)
                        return p;
                }
            }

            return Player.Empty;
        }

        public static bool AddPlayer(Player player)
        {
            lock (Players)
            {
                if (Players.ContainsKey(player.PID) || PlayersUID.ContainsKey(player.UID))
                    return false;

                Players.Add(player.PID, player);
                PlayersUID.Add(player.UID, player);

                if (NewPlayer != null)
                    NewPlayer(player);
            }

            return true;
        }
    }
}

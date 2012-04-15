using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;
using Game;

namespace GameInstance
{
    public class Player
    {
        public delegate void PlayerEvent(Player player);
        public delegate void PlayerOptionEvent(Player player, int option);

        public static event PlayerEvent NewPlayer;
        public static event PlayerEvent DeletedPlayer;

        public static event PlayerOptionEvent OptionChanged;

        public UInt64 UID = UInt64.MaxValue;
        public UInt64 PID = UInt64.MaxValue;

        public string Name = string.Empty;

        public NetConnection Connection = null;
        public TeamInfo Team = TeamInfo.Empty;
        public RemotePlayer SimPlayer = null;

        public UInt64 ModelID = UInt64.MaxValue;
        public int AvatarID = -1;

        public int[] Options = null;

        public static Player Empty = new Player(null,null);

        public bool Valid = true;

        public enum PlayerStatus
        {
            New,
            Identified,
            Loaded,
            Joined, // player has an actor
            Limbo, // player has an actor but that actor is inactive.
            Disconnecting,
            Parted,
        }

        public PlayerStatus Status = PlayerStatus.New;

        protected NetServer Server = null;

        public Player(NetConnection con, NetServer server)
        {
            Server = server;
            Connection = con;
        }

        public void SendReliable(NetOutgoingMessage msg)
        {
            if (!Valid || Connection.Status == NetConnectionStatus.Disconnecting || Connection.Status == NetConnectionStatus.Disconnected)
                return;

            Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendReliable(GameMessage msg)
        {
            SendReliable(msg, 2);
        }

        public void SendReliable(GameMessage msg, int channel)
        {
            if (!Valid || Server.Status != NetPeerStatus.Running || Connection.Status == NetConnectionStatus.Disconnecting || Connection.Status == NetConnectionStatus.Disconnected)
                return;

            lock (Server)
                Connection.SendMessage(msg.Pack(Server.CreateMessage()), NetDeliveryMethod.ReliableOrdered, channel);
        }


        protected static Dictionary<UInt64, Player> Players = new Dictionary<UInt64, Player>();
        protected static Dictionary<UInt64, Player> PlayersUID = new Dictionary<UInt64, Player>();

        protected static UInt64 LastPlayerID = 0;
        public static UInt64 NewPlayerID()
        {
            lock (Players)
            {
                LastPlayerID++;
                return LastPlayerID;
            }
        }

        public static UInt64[] PlayerIDList()
        {
            lock (Players)
                return Players.Keys.ToArray();
        }

        public static UInt64[] UserIDList()
        {
            lock (Players)
                return PlayersUID.Keys.ToArray();
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

        public static Player PlayerByConnection(NetConnection con)
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

        public static bool SetOption(Player player, int optionIndex, int value)
        {
            lock (player)
            {
                if (optionIndex >= 0 && optionIndex < player.Options.Length)
                {
                    player.Options[optionIndex] = value;
                    if (OptionChanged != null)
                        OptionChanged(player, value);
                    return true;
                }
            }
            return false;
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

        public static void RemovePlayer(Player player)
        {
            lock (Players)
            {
                lock (player)
                    player.Valid = false;

                if (!Players.ContainsKey(player.PID))
                  Players.Remove(player.PID);

                if (PlayersUID.ContainsKey(player.UID))
                    PlayersUID.Remove(player.UID);

                if (DeletedPlayer != null)
                    DeletedPlayer(player);
            }
        }
    }
}

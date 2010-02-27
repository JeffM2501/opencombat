using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class ShotEventArgs : EventArgs
    {
        public Shot shot;
        public double time;

        public ShotEventArgs ( Shot s, double t)
        {
            shot = s;
            time = t;
        }
    }

    public class PlayerEventArgs : EventArgs
    {
        public Player player;
        public double time;

        public PlayerEventArgs(Player p, double t)
        {
            player = p;
            time = t;
        }
    }

    public class PlayerUpdateEventArgs : PlayerEventArgs
    {
        ObjectState state;

        public PlayerUpdateEventArgs(Player p, ObjectState s, double t) : base(p,t)
        {
            state = s;
        }
    }

    public class SimSettings
    {
        public float BaseSpeed = 70.0f;
        public float BaseTurnSpeed = 180.0f;
        public float BaseAcceleration = 25f;
        public float BaseTurnAcceleration = 10f;
    }

    public delegate void ShotEndedHandler(object sender, ShotEventArgs args );
    public delegate void ShotStartedHandler(object sender, ShotEventArgs args);

    public delegate void PlayerJoinedHandler(object sender, PlayerEventArgs args);
    public delegate void PlayerRemovedHandler(object sender, PlayerEventArgs args);
    public delegate void PlayerUpdateHandler(object sender, PlayerUpdateEventArgs args);
    public delegate void PlayerStatusChangeHandler(object sender, PlayerEventArgs args);


    public class Sim
    {
        public MapDef Map = new MapDef();

        public List<Player> Players = new List<Player>();
        public List<Shot> Shots = new List<Shot>();

        public event ShotStartedHandler ShotStarted;
        public event ShotEndedHandler ShotEnded;
        
        public event PlayerJoinedHandler PlayerJoined;
        public event PlayerRemovedHandler PlayerRemoved;
        public event PlayerUpdateHandler PlayerUpdated;

        public event PlayerStatusChangeHandler PlayerStatusChanged;

        public SimSettings Settings = new SimSettings();

        double lastUpdateTime = -1;

        public void Init ()
        {
        }

        public bool PlayerNameValid ( string name )
        {
            foreach (Player player in Players)
            {
                if (player.Callsign == name)
                    return false;
            }

            return true;
        }

        public Player FindPlayer ( UInt64 GUID )
        {
            foreach (Player player in Players)
            {
                if (player.ID == GUID)
                    return player;
            }
            return null;
        }

        public Player NewPlayer()
        {
            return new Player(this);
        }

        public void AddPlayer ( Player player )
        {
            Player existing = FindPlayer(player.ID);
            if (existing != null)
                existing.CopyFrom(player);
            else
            {
                Players.Add(player);
                existing = player;
            }
            if (PlayerJoined != null)
                PlayerJoined(this, new PlayerEventArgs(existing, lastUpdateTime));
        }

        public Shot NewShot()
        {
            return new Shot(this);
        }

        public void AddShot(Shot shot)
        {
            Shots.Add(shot);
            if (ShotStarted != null)
                ShotStarted(this, new ShotEventArgs(shot, lastUpdateTime));
        }

        public void RemovePlayer ( Player player )
        {
            Players.Remove(player);
            if (PlayerRemoved != null)
                PlayerRemoved(this, new PlayerEventArgs(player, lastUpdateTime));
        }

        protected void RemoveShot ( Shot shot )
        {
            Shots.Remove(shot);
            if (ShotEnded != null)
                ShotEnded(this, new ShotEventArgs(shot, lastUpdateTime));
        }

        public void UpdatePlayer ( Player player, ObjectState state, double time )
        {
            player.LastUpdateState = state;
            player.LastUpdateTime = time;
            if (PlayerUpdated != null)
                PlayerUpdated(this, new PlayerUpdateEventArgs(player, state, time));
        }

        public void SpawnPlayer ( Player player, double time )
        {
            if (SpawnGenerator.SpawnPlayer(player, this))
            {
                player.Status = PlayerStatus.Alive;
                player.LastUpdateTime = time;

                if (PlayerStatusChanged != null)
                    PlayerStatusChanged(this, new PlayerEventArgs(player, time));
            }
        }

        public void SetPlayerStatus ( Player player, PlayerStatus status, double time)
        {
            player.Status = status;
            if (PlayerStatusChanged != null)
                PlayerStatusChanged(this, new PlayerEventArgs(player, time));
        }

        public void Update ( double time )
        {
            lastUpdateTime = time;

            foreach (Player player in Players)
                player.Update(time);

            List<Shot> deadShots = new List<Shot>();
            foreach (Shot shot in Shots)
            {
                shot.Update(time);
                if (shot.Expired(time))
                    deadShots.Add(shot);
            }

            foreach (Shot shot in deadShots)
                RemoveShot(shot);
        }
    }
}

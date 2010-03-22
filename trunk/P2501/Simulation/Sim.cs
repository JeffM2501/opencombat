/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using World;

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

    public enum GameType
    {
        DeathMatch,
        TeamDeathMatch,
        CaptureTheFlag,
        CaptureTheBase,
        KillTheChicken,
        Domination,
        Assult,
    }

    [Serializable]
    public class SimSettings
    {
        public GameType GameMode = GameType.TeamDeathMatch;
        public float BaseSpeed = 70.0f;
        public float BaseTurnSpeed = 180.0f;
        public float BaseAcceleration = 25f;
        public float BaseTurnAcceleration = 10f;
    }
    public delegate void SimEventHandler(object sender, EventArgs args);

    public delegate void ShotEndedHandler(object sender, ShotEventArgs args );
    public delegate void ShotStartedHandler(object sender, ShotEventArgs args);

    public delegate void PlayerJoinedHandler(object sender, PlayerEventArgs args);
    public delegate void PlayerRemovedHandler(object sender, PlayerEventArgs args);
    public delegate void PlayerUpdateHandler(object sender, PlayerUpdateEventArgs args);
    public delegate void PlayerStatusChangeHandler(object sender, PlayerEventArgs args);

    public class Sim
    {
        public MapDef MapInfo = new MapDef();
        public PortalWorld World = null;

        public List<Player> Players = new List<Player>();
        public List<Shot> Shots = new List<Shot>();

        public event SimEventHandler WorldChanged;

        public event ShotStartedHandler ShotStarted;
        public event ShotEndedHandler ShotEnded;
        
        public event PlayerJoinedHandler PlayerJoined;
        public event PlayerRemovedHandler PlayerRemoved;
        public event PlayerUpdateHandler PlayerUpdated;

        public event PlayerStatusChangeHandler PlayerStatusChanged;

        public SimSettings Settings = new SimSettings();

        public string[] TeamNames = null;

        double lastUpdateTime = -1;

        public void Init ()
        {
            TeamNames = new string[1];
            TeamNames[0] = "None";
        }

        public void SetWorld ( FileInfo file )
        {
            SetWorld(PortalWorld.Read(file));
        } 

        public void SetWorld ( PortalWorld w )
        {
            World = w;
            if (WorldChanged != null)
                WorldChanged(this, EventArgs.Empty);
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

        public static Player NewPlayer()
        {
            return new Player();
        }

        public void AddPlayer ( Player player )
        {
            player.SetSim(this);
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

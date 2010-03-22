﻿/*
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

using Messages;
using Simulation;
using Lidgren.Network;

using World;

namespace P2501GameClient
{
    public partial class GameClient
    {
        Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();
        Dictionary<int, MessageHandler> messageCodeHandlers = new Dictionary<int, MessageHandler>();

        int MapFileID = -1;

        protected void InitMessageHandlers()
        {
            messageHandlers.Add(typeof(Ping), new MessageHandler(PingHandler));
            messageHandlers.Add(typeof(Pong), new MessageHandler(PongHandler));
            messageCodeHandlers.Add(MessageClass.Hail, new MessageHandler(HailHandler));
            messageHandlers.Add(typeof(LoginAccept), new MessageHandler(LoginAcceptHandler));
            messageHandlers.Add(typeof(InstanceList), new MessageHandler(InstanceListHandler));
            messageCodeHandlers.Add(MessageClass.InstanceSelectFailed, new MessageHandler(InstanceSelectFailedHandler));
            messageHandlers.Add(typeof(ServerVersInfo), new MessageHandler(ServerVersHandler));
            messageHandlers.Add(typeof(PlayerInfo), new MessageHandler(PlayerInfoHandler));
            messageCodeHandlers.Add(MessageClass.PlayerListDone, new MessageHandler(PlayerListDoneHandler));
            messageHandlers.Add(typeof(FileTransfter), new MessageHandler(FileTransfterHandler));
            messageCodeHandlers.Add(MessageClass.PlayerJoinAccept, new MessageHandler(PlayerJoinAcceptHandler));
            messageHandlers.Add(typeof(ChatMessage), new MessageHandler(ChatMessageHandler));
            messageCodeHandlers.Add(MessageClass.AllowSpawn, new MessageHandler(AllowSpawnHandler));
            messageHandlers.Add(typeof(PlayerSpawn), new MessageHandler(PlayerSpawnHandler));
            messageHandlers.Add(typeof(TheTimeIsNow), new MessageHandler(TheTimeIsNowHandler));
            messageHandlers.Add(typeof(InstanceSettings), new MessageHandler(InstanceSettingsHandler));
        }

        protected void Send ( MessageClass message )
        {
            client.SendMessage(message.Pack(), message.Channel());
        }

        protected void Send ( Int32 code )
        {
            NetBuffer buffer = new NetBuffer();
            buffer.Write(code);
            client.SendMessage(buffer, NetChannel.ReliableInOrder9);
        }

        protected void PingHandler(MessageClass message)
        {
            Ping msg = message as Ping;
            if (msg == null)
                return;

            Send(new Pong(msg.ID));
        }

        protected void AddLatencyUpdate ( double latency )
        {
            if (latency < 0)
                return; // can't go back in time

            latencyList.Add(latency);
            if (latencyList.Count > LatencySamples)
                latencyList.RemoveRange(0, latencyList.Count - LatencySamples);

            double min = latency;
            double max = latency;
            double sum = 0;

            foreach(double d in latencyList)
            {
                sum += d;
                if (d > max)
                    max = d;
                if (d < min)
                    min = d;
            }

            averageLatency = sum / latencyList.Count;
            jitter = (max - min)/2;

            double t = RawTime() - (averageLatency * 4);

            int lostPings = 0;

            foreach(KeyValuePair<UInt64,double> ping in OutStandingPings)
            {
                if (ping.Value < t)
                    lostPings++;
            }

            packetloss = (double)lostPings / (double)lastPing;
            packetloss *= 100;
        }

        protected void PongHandler(MessageClass message)
        {
            Pong msg = message as Pong;
            if (msg == null)
                return;

            if (!OutStandingPings.ContainsKey(msg.ID))
                return;

            double delta = RawTime() - OutStandingPings[msg.ID];
            OutStandingPings.Remove(msg.ID);
            AddLatencyUpdate(delta);
        }

        protected void HailHandler(MessageClass message)
        {
            SendClockUpdate();

            Login login = new Login();
            login.UID = 0;
            login.CID = 0;
            login.Token = 0;
            login.Major = ClientVersion.Major;
            login.Minor = ClientVersion.Minor;
            login.Revision = ClientVersion.Revision;
            login.Bin = ClientVersion.Bin;

            if (GetAuthentication != null)
                GetAuthentication(ref login.UID, ref login.CID, ref login.Token);

            Send(login);
        }

        protected void LoginAcceptHandler(MessageClass message)
        {
            LoginAccept accept = message as LoginAccept;
            if (accept == null)
                return;

            ThisPlayer = Sim.NewPlayer();
            ThisPlayer.ID = accept.PlayerID;
            ThisPlayer.Callsign = accept.Callsign;

            RequestInstanceList();

            if (LoginAccepted != null)
                LoginAccepted(this, EventArgs.Empty);
        }

        public void RequestInstanceList ()
        {
            Send(MessageClass.RequestInstanceList);
        }

        protected void ServerVersHandler(MessageClass message)
        {
            ServerVersInfo info = message as ServerVersInfo;
            if (info == null)
                return;

            if (info.Protocoll != MessageProtcoll.Version)
            {
                client.Kill();
                return;
            }

            if (ServerVersionReceived != null)
                ServerVersionReceived(this, new ServerVersionEventArgs(info.Major, info.Minor, info.Rev));
        }

        protected void InstanceListHandler(MessageClass message)
        {
            InstanceList info = message as InstanceList;
            if (info == null)
                return;

            AvailableInstances.Clear();
            foreach(InstanceList.InstanceDescription d in info.Instances)
            {
                InstanceDefinition def = new InstanceDefinition();
                def.ID = d.ID;
                def.Description = d.Description;
                AvailableInstances.Add(def);
            }

            if (InstanceList != null)
                InstanceList(this, EventArgs.Empty);
        }

        protected bool InstanceExists ( int id )
        {
            foreach (InstanceDefinition def in AvailableInstances)
            {
                if (def.ID == id)
                    return true;
            }
            return false;
        }

        public bool SelectInstance ( int instance )
        {
            if (!InstanceExists(instance))
                return false;

            InstanceSelect sel = new InstanceSelect();
            sel.ID = instance;
            Send(sel);
            return true;
        }

        protected void InstanceSelectFailedHandler(MessageClass message)
        {
            ConnectedInstance = -1;

            if (InstanceJoinFailed != null)
                InstanceJoinFailed(this, EventArgs.Empty);
        }

        protected void InstanceSettingsHandler(MessageClass message)
        {
            InstanceSettings info = message as InstanceSettings;
            if (info == null)
                return;

            if (ConnectedInstance != info.ID)
            {
                ConnectedInstance = info.ID;
                if (InstanceJoined != null)
                    InstanceJoined(this, EventArgs.Empty);
            }

            if (InstanceSettingsReceived != null)
                InstanceSettingsReceived(this, EventArgs.Empty);

            sim.Settings = info.Settings;

            if (FileDownloadManager.FileExist(info.MapChecksum)) // we already have the map
                LoadMap(FileDownloadManager.GetFile(info.MapChecksum));
            else
            {
                RequestMapInfo mapInfo = new RequestMapInfo();
                MapFileID = mapInfo.ID = FileDownloadManager.GetDownloadID(new FileEventHandler(MapComplete));
                Send(mapInfo);
                if (StartMapTransfer != null)
                    StartMapTransfer(this, EventArgs.Empty);
            }
        }

        protected void LoadMap ( Stream stream )
        {
            sim.SetWorld(PortalWorld.Read(stream, true));

            if (sim.World == null)
            {
                if (MapLoadFailed != null)
                    MapLoadFailed(this, EventArgs.Empty);
            }
            else
            {
                if (MapLoaded != null)
                    MapLoaded(this, EventArgs.Empty);
            }
        }

        protected void MapComplete(object sender, FileDownloadEventArgs args)
        {
            if (EndMapTransfer != null)
                EndMapTransfer(this, EventArgs.Empty);

            LoadMap(FileDownloadManager.GetFile(args.ID));
        }

        protected void PlayerInfoHandler(MessageClass message)
        {
            PlayerInfo info = message as PlayerInfo;
            if (info == null)
                return;

            Player player = Sim.NewPlayer();
            player.ID = info.PlayerID;
            player.Callsign = info.Callsign;
            player.Score = info.Score;
            player.Status = info.Status;

            sim.AddPlayer(player);
        }

        protected void FileTransfterHandler(MessageClass message)
        {
            FileTransfter info = message as FileTransfter;
            if (info == null)
                return;

            if (FileTransferProgress != null)
            {
                FileTransferProgressEventArgs.FileTransferType t = FileTransferProgressEventArgs.FileTransferType.Other;
                if (info.ID == MapFileID)
                    t = FileTransferProgressEventArgs.FileTransferType.Map;
                FileTransferProgress(this, new FileTransferProgressEventArgs(info.Chunk, info.Total,t));
            }

            FileDownloadManager.AddFileData(info);
        }

        protected void PlayerListDoneHandler ( MessageClass message )
        {
            Send(MessageClass.PlayerJoin);
            SendPing();
        }

        protected void PlayerJoinAcceptHandler ( MessageClass message )
        {
            // just fire off a callback or something
            sim.AddPlayer(ThisPlayer);
            SendPing();
        }

        protected void ChatMessageHandler ( MessageClass message )
        {
            ChatMessage msg = message as ChatMessage;
            if (msg == null)
                return;

            if (ChatReceived != null)
                ChatReceived(this, new ChatEventArgs(msg.ChatChannel, msg.From, msg.Message));
        }

        protected void CallAllowSpawn ()
        {
            requestedSpawn = false;
            ThisPlayer.Status = PlayerStatus.Despawned;
            if (AllowSpawn != null)
                AllowSpawn(this, new  PlayerEventArgs(ThisPlayer));
        }

        protected void AllowSpawnHandler(MessageClass message)
        {
            gotAllowSpawn = true;

            if (haveSyncedTime)
                CallAllowSpawn();
        }    
    
        protected void PlayerSpawnHandler ( MessageClass message )
        {
            PlayerSpawn msg = message as PlayerSpawn;
            if (msg == null)
                return;

            Player player = sim.FindPlayer(msg.PlayerID);

            player.LastUpdateTime = msg.Time;
            player.LastUpdateState = msg.PlayerState;
            player.Status = PlayerStatus.Alive;
            player.Update(player.LastUpdateTime);
            sim.SetPlayerStatus(player, PlayerStatus.Alive,lastUpdateTime);
            SendPing();
        }

        protected void TheTimeIsNowHandler(MessageClass message)
        {
            TheTimeIsNow msg = message as TheTimeIsNow;
            if (msg == null)
                return;

            if (!OutStandingPings.ContainsKey(msg.ID))
            {
                SendClockUpdate();
                return; // something bad happened, fire off another one just to be safe
            }

            double timeSent = OutStandingPings[msg.ID];
            double delta = RawTime()-timeSent;
            OutStandingPings.Remove(msg.ID);

            double serverTimeNow = msg.Time + delta*0.5;

            serverTimeOffset = serverTimeNow - RawTime();

            AddLatencyUpdate(delta);

            haveSyncedTime = true;

            if (ThisPlayer != null && ThisPlayer.Status == PlayerStatus.Connecting && gotAllowSpawn)
                CallAllowSpawn(); // if we haven't synced clock yet, then do it
        }    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Messages;
using Simulation;

namespace P2501GameClient
{
    public partial class GameClient
    {
        Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();

        protected void InitMessageHandlers()
        {
            messageHandlers.Add(typeof(Ping), new MessageHandler(PingHandler));
            messageHandlers.Add(typeof(Pong), new MessageHandler(PongHandler));
            messageHandlers.Add(typeof(Hail), new MessageHandler(HailHandler));
            messageHandlers.Add(typeof(ServerVersInfo), new MessageHandler(ServerVersHandler));
            messageHandlers.Add(typeof(PlayerInfo), new MessageHandler(PlayerInfoHandler));
            messageHandlers.Add(typeof(PlayerListDone), new MessageHandler(PlayerListDoneHandler));
            messageHandlers.Add(typeof(MapInfo), new MessageHandler(MapInfoHandler));
            messageHandlers.Add(typeof(PlayerJoinAccept), new MessageHandler(PlayerJoinAcceptHandler));
            messageHandlers.Add(typeof(ChatMessage), new MessageHandler(ChatMessageHandler));
            messageHandlers.Add(typeof(AllowSpawn), new MessageHandler(AllowSpawnHandler));
            messageHandlers.Add(typeof(PlayerSpawn), new MessageHandler(PlayerSpawnHandler));
            messageHandlers.Add(typeof(TheTimeIsNow), new MessageHandler(TheTimeIsNowHandler));
        }

        protected void PingHandler(MessageClass message)
        {
            Ping msg = message as Ping;
            if (msg == null)
                return;

            Pong pong = new Pong();
            pong.ID = msg.ID;
            client.SendMessage(pong.Pack(), pong.Channel());
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
            Hail hail = message as Hail;
            if (hail == null)
                return;

            SendClockUpdate();

            Login login = new Login();
            login.UID = 0;
            login.CID = 0;
            login.Token = 0;
            if (GetAuthentication != null)
                GetAuthentication(ref login.UID, ref login.CID, ref login.Token);

            client.SendMessage(login.Pack(), login.Channel());
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

            if (ServerVersionEvent != null)
                ServerVersionEvent(this, info.Major, info.Minor, info.Rev);
        }

        protected void PlayerInfoHandler(MessageClass message)
        {
            PlayerInfo info = message as PlayerInfo;
            if (info == null)
                return;

            Player player = sim.NewPlayer();
            player.ID = info.PlayerID;
            player.Callsign = info.Callsign;
            player.Score = info.Score;
            player.Pilot = info.Pilot;
            player.Status = info.Status;

            sim.AddPlayer(player);
        }

        protected void MapInfoHandler ( MessageClass message )
        {
            MapInfo info = message as MapInfo;
            if (info == null)
                return;

            sim.Map = info.Map;
        }

        protected void PlayerListDoneHandler ( MessageClass message )
        {
            PlayerJoin join = new PlayerJoin();
            join.Callsign = "Player";
            join.Pilot = "Pilot0u";

            if (GetJoinInfo != null)
                GetJoinInfo(ref join.Callsign, ref join.Pilot);

            client.SendMessage(join.Pack(), join.Channel());
            SendPing();
        }

        protected void PlayerJoinAcceptHandler ( MessageClass message )
        {
            PlayerJoinAccept msg = message as PlayerJoinAccept;
            if (msg == null)
                return;

            Player player = sim.FindPlayer(msg.PlayerID);
            if (player == null)
            {
                player = sim.NewPlayer();
                player.ID = msg.PlayerID;
                sim.AddPlayer(player);
            }

            player.Callsign = msg.Callsign;
            ThisPlayer = player;
            SendPing();
        }

        protected void ChatMessageHandler ( MessageClass message )
        {
            ChatMessage msg = message as ChatMessage;
            if (msg == null)
                return;

            if (ChatReceivedEvent != null)
                ChatReceivedEvent(this, msg.ChatChannel, msg.From, msg.Message);
        }

        protected void CallAllowSpawn ()
        {
            requestedSpawn = false;
            ThisPlayer.Status = PlayerStatus.Despawned;
            if (AllowSpawnEvent != null)
                AllowSpawnEvent(this, ThisPlayer);
        }

        protected void AllowSpawnHandler(MessageClass message)
        {
            AllowSpawn msg = message as AllowSpawn;
            if (msg == null)
                return;

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

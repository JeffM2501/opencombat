using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using Hosts;
using Simulation;
using Messages;

using Lidgren.Network;

namespace Project2501Server
{
    public partial class Server
    {
        Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();
        Dictionary<int, MessageHandler> messageCodeHandlers = new Dictionary<int, MessageHandler>();

        protected void InitMessageHandlers()
        {
            messageHandlers.Add(typeof(Login), new MessageHandler(LoginHandler));
            messageHandlers.Add(typeof(PlayerJoin), new MessageHandler(PlayerJoinHandler));
            messageHandlers.Add(typeof(ChatMessage), new MessageHandler(ChatMessageHandler));
            messageHandlers.Add(typeof(RequestSpawn), new MessageHandler(RequestSpawnHandler));
            messageHandlers.Add(typeof(Ping), new MessageHandler(PingHandler));
            messageHandlers.Add(typeof(WhatTimeIsIt), new MessageHandler(WhatTimeIsItHandler));
        }

        protected void ProcessMessage(Message msg)
        {
            if (!Clients.ContainsKey(msg.Sender))
                return;

            Client client = Clients[msg.Sender];

            if (msg.Name != MessageClass.Login)
            {
                if (!client.Checked || client.UID == 0)
                    return;
            }

            MessageClass message = messageMapper.MessageFromID(msg.Name);
            if (message != null)
            {
                message.Unpack(ref msg.Data);

                if (messageHandlers.ContainsKey(message.GetType()))
                    messageHandlers[message.GetType()](client, message);
                else if (messageCodeHandlers.ContainsKey(msg.Name))
                    messageCodeHandlers[msg.Name](client, message);
            }
            else
            {
                if (messageCodeHandlers.ContainsKey(msg.Name))
                    messageCodeHandlers[msg.Name](client, MessageClass.NoDataMessage(msg.Name));
            }
        }

        protected void Send (Client client, MessageClass message)
        {
            host.SendMessage(client.Connection, message.Pack(), message.Channel());
        }

        protected void PingHandler(Client client, MessageClass message)
        {
            Ping msg = message as Ping;
            if (msg == null)
                return;

            Pong pong = new Pong();
            pong.ID = msg.ID;
            Send(client, pong);
        }

        protected void LoginHandler(Client client, MessageClass message)
        {
            Login login = message as Login;
            if (login == null)
                return;

            client.UID = login.UID;
            client.CID = login.CID;
            client.Token = login.Token;

            tokenChecker.AddJob(login.UID, login.Token, login.CID,client.Connection.RemoteEndpoint.Address.ToString(), client);
        }

        protected void FinishLogin ( Client client )
        {
            LoginAccept accept = new LoginAccept();
            accept.Callsign = client.Player.Callsign;
            accept.PlayerID = client.Player.ID;

            Send(client, accept);
        }

        protected void PlayerJoinHandler(Client client, MessageClass message)
        {
            PlayerJoin join = message as PlayerJoin;
            if (join == null)
                return;

            client.Player = sim.NewPlayer();
            client.Player.Tag = client;

            client.Player.ID = GUIDManager.NewGUID();
            client.Player.Pilot = join.Pilot;

            while (!sim.PlayerNameValid(join.Callsign))
                join.Callsign += "X";

            client.Player.Callsign = join.Callsign;

            sim.AddPlayer(client.Player);

            PlayerJoinAccept accept = new PlayerJoinAccept();
            accept.Callsign = client.Player.Callsign;
            accept.PlayerID = client.Player.ID;

            Send(client, accept);

            AllowSpawn spawn = new AllowSpawn();
            Send(client, spawn);
        }

        protected void ChatMessageHandler(Client client, MessageClass message)
        {
            ChatMessage msg = message as ChatMessage;
            if (msg == null || client.Player == null)
                return;

            msg.From = client.Player.Callsign;
            host.Broadcast(msg.Pack(), msg.Channel());
        }
        
        protected void RequestSpawnHandler(Client client, MessageClass message)
        {
            RequestSpawn msg = message as RequestSpawn;
            if (msg == null || client.Player == null)
                return;

            sim.SpawnPlayer(client.Player, lastUpdateTime);
        }

        protected void WhatTimeIsItHandler(Client client, MessageClass message)
        {
            WhatTimeIsIt msg = message as WhatTimeIsIt;
            if (msg == null)
                return;

            TheTimeIsNow time = new TheTimeIsNow();
            time.ID = msg.ID;
            time.Time = Now();
            Send(client, time);
        }
    }
}

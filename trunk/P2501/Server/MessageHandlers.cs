﻿using System;
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
        public bool NoTokenCheck = false;

        Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();
        Dictionary<int, MessageHandler> messageCodeHandlers = new Dictionary<int, MessageHandler>();

        Dictionary<Type, InstanceMessageHandler> instanceMessageHandlers = new Dictionary<Type, InstanceMessageHandler>();
        Dictionary<int, InstanceMessageHandler> instanceMessageCodeHandlers = new Dictionary<int, InstanceMessageHandler>();

        protected void InitMessageHandlers()
        {
            messageHandlers.Add(typeof(Login), new MessageHandler(LoginHandler));
            messageHandlers.Add(typeof(Ping), new MessageHandler(PingHandler));
            messageHandlers.Add(typeof(WhatTimeIsIt), new MessageHandler(WhatTimeIsItHandler));
            messageCodeHandlers.Add(MessageClass.RequestInstanceList, new MessageHandler(RequestInstanceListHandler));
            messageHandlers.Add(typeof(InstanceSelect), new MessageHandler(InstanceSelectHandler));

            instanceMessageCodeHandlers.Add(MessageClass.RequestMapInfo, new InstanceMessageHandler(RequestMapInfoHandler));
            instanceMessageCodeHandlers.Add(MessageClass.PlayerJoin, new InstanceMessageHandler(PlayerJoinHandler));
            instanceMessageCodeHandlers.Add(MessageClass.RequestSpawn, new InstanceMessageHandler(RequestSpawnHandler));
            instanceMessageHandlers.Add(typeof(ChatMessage), new InstanceMessageHandler(ChatMessageHandler));
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

            MessageClass message = null;

            if (client.Instance != null) // see if an instance wants it
            {
                lock(instanceMessageHandlers)
                {
                    message = messageMapper.MessageFromID(msg.Name);
                    if (message != null)
                    {
                        message.Unpack(ref msg.Data);

                        if (instanceMessageHandlers.ContainsKey(message.GetType()) || instanceMessageCodeHandlers.ContainsKey(msg.Name))
                        {
                            client.Instance.AddMessage(client, message);
                            return;
                        }
                    }
                    else
                    {
                        if (instanceMessageCodeHandlers.ContainsKey(msg.Name))
                        {
                            client.Instance.AddMessage(client, MessageClass.NoDataMessage(msg.Name));
                            return;
                        }
                    }
                }
            }

            lock(client)
            {
                message = messageMapper.MessageFromID(msg.Name);
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
        }

        public void ProcessInstanceMessage ( Client client, MessageClass message, ServerInstance instance )
        {
            lock (instanceMessageHandlers)
            {
                lock (client)
                {
                    if (instanceMessageHandlers.ContainsKey(message.GetType()))
                        instanceMessageHandlers[message.GetType()](client, message, instance);
                    else if (instanceMessageCodeHandlers.ContainsKey(message.Name))
                        instanceMessageCodeHandlers[message.Name](client, message, instance);
                }
            }
        }

        public void Send(Client client, Int32 message)
        {
            Send(client, MessageClass.NoDataMessage(message));
        }

        public void Send (Client client, MessageClass message)
        {
            host.SendMessage(client.Connection, message.Pack(), message.Channel());
        }

        public void Send(MessageClass message)
        {
            host.Broadcast(message.Pack(), message.Channel());
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

            if (login.Version != MessageProtcoll.Version)
            {
                host.DisconnectUser(client.Connection);
                return;
            }

            client.UID = login.UID;
            client.CID = login.CID;
            client.Token = login.Token;
            client.Player = Sim.NewPlayer();

            if (NoTokenCheck)
            {
                client.Checked = true;
                client.Verified = true;
                client.Player.Callsign = "TestUser";
                FinishLogin(client);
            }
            else
            {
                tokenChecker.AddJob(new TokenChecker.TokenCheckerJob(login.UID, login.Token, login.CID, client.Connection.RemoteEndpoint.Address.ToString(), client));
            }
        }

        protected void FinishLogin ( Client client )
        {
            LoginAccept accept = new LoginAccept();
            accept.Callsign = client.Player.Callsign;
            accept.PlayerID = client.Player.ID;

            Send(client, accept);
        }

        protected void RequestInstanceListHandler(Client client, MessageClass message)
        {
            InstanceList instances = new InstanceList();
            foreach (KeyValuePair<int,string> item in ServerInstanceManger.GetInstanceList())
                instances.Add(item.Key, item.Value);

            Send(client, instances);
        }

        protected void InstanceSelectHandler(Client client, MessageClass message)
        {
            InstanceSelect select = message as InstanceSelect;
            if (select == null)
                return;

            ServerInstance instance = ServerInstanceManger.GetInstance(select.ID);
            if (instance == null)
            {
                Send(client, MessageClass.InstanceSelectFailed);
                return;
            }

            lock (client)
            {
                client.Instance = instance;
                instance.AddClient(client);
            }

            InstanceJoined joined = new InstanceJoined();

            joined.ID = instance.ID;

            Send(client, joined);

            InstanceSettings settings = new InstanceSettings();
            settings.Settings = instance.Settings.Settings;
            Send(client, settings);
        }

        protected void RequestMapInfoHandler(Client client, MessageClass message, ServerInstance instance)
        {
            instance.SendMap(client);
        }

        protected void PlayerJoinHandler(Client client, MessageClass message, ServerInstance instance)
        {
            instance.AddPlayer(client);
            Send(client, MessageClass.PlayerJoinAccept);
            Send(client, MessageClass.AllowSpawn);
        }

        protected void ChatMessageHandler(Client client, MessageClass message, ServerInstance instance)
        {
            ChatMessage msg = message as ChatMessage;
            if (msg == null || client.Player == null)
                return;

            msg.From = client.Player.Callsign;
            instance.Broadcast(msg);
        }
        
        protected void RequestSpawnHandler(Client client, MessageClass message, ServerInstance instance)
        {
            instance.Spawn(client.Player);
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

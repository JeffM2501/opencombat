﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ClientGame
	{
        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<EventArgs> ResourcesComplete;
        public event EventHandler<EventArgs> ScriptsLoaded;

        public class SystemMessageEventArgs : EventArgs
        {
            public string Text = string.Empty;
            public SystemMessageEventArgs(string txt) : base()
            {
                Text = txt;
            }
        }

        public event EventHandler<SystemMessageEventArgs> SystemMessage;

        public ServerConnection.ConnectionStatus Status = ServerConnection.ConnectionStatus.New;

        protected List<EventHandler<EventArgs>> PendingArgEvents = new List<EventHandler<EventArgs>>();

        protected void CallPendingEvents()
        {
            int count = 0;
            lock(PendingArgEvents)
                count = PendingArgEvents.Count;

            for(int i = 0; i < count; i++)
            {
                EventHandler<EventArgs> evt = null;
                lock(PendingArgEvents)
                    evt = PendingArgEvents[i];
                evt(this,EventArgs.Empty);
            }
            
            lock(PendingArgEvents)
                PendingArgEvents.RemoveRange(0,count);
        }

        protected void AddPendingEvents(EventHandler<EventArgs> evt)
        {
            if (evt == null)
                return;

            lock (PendingArgEvents)
                PendingArgEvents.Add(evt);
        }

        void ServerConnectionStatusChanged(object sender, EventArgs args)
        {
            Status = Connection.Status;
            AddPendingEvents(StatusChanged);
        }

        void ResourceLoadComplete(object sender, EventArgs args)
        {
            Status = Connection.Status;
            AddPendingEvents(ResourcesComplete);
        }

        public void SendSystemMessage(string message)
        {
            if (SystemMessage != null)
                SystemMessage(this, new SystemMessageEventArgs(message));
        }
	}
}
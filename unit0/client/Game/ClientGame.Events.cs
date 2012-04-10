using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ClientGame
	{
        public event EventHandler<EventArgs> StatusChanged;

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
                PendingArgEvents.RemoveRange(0,count-1);
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
	}
}

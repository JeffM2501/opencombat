using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;
using GridWorld;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Client
{
    public partial class ClientGame
    {
        public EventHandler<EventArgs> ToggleDrawing;

        public delegate void DebugValueCallback (string name, string value);
        public event DebugValueCallback AddDebugLogItem;

        protected void CallDebugLogItem(string name, string value)
        {
            if (AddDebugLogItem != null)
                AddDebugLogItem(name, value);
        }

        public GameState State = null;
        protected InputSystem InputTracker = null;

        ServerConnection Connection = null;

        protected object locker = new object();

        protected bool Done = false;
        protected string LastError = string.Empty;

        public bool IsDone()
        {
            lock (locker)
                return Done;
        }

        public string GetLastError()
        {
            lock (locker)
                return LastError;
        }

        public ClientGame(InputSystem input)
        {
            InputTracker = input;
            State = new GameState();

            ClientScripting.Script.SetState(this);

            State.GetWorld = GetSimpleWorld;
            State.MapLoaded += new EventHandler<EventArgs>(State_MapLoaded);

            InputTracker.RegisterControls += new EventHandler<EventArgs>(InputTracker_RegisterControls);
            InputTracker.LoadDefaultBindings += new EventHandler<EventArgs>(InputTracker_LoadDefaultBindings);
        }

        public void Connect(string host, int port)
        {
            Connection = new ServerConnection(host, port);

            Connection.StatusChanged += new EventHandler<EventArgs>(ServerConnectionStatusChanged);
            Connection.Connected += new EventHandler<EventArgs>(ConnectionComplete);
            Connection.Failed += new EventHandler<EventArgs>(ConnectionError);
            Connection.Disconnected += new EventHandler<EventArgs>(ConnectionEnded);
        }

        void ConnectionComplete (object sender, EventArgs args)
        {
            ClientScripting.Script.Init(Connection.ScriptingInfo.ScriptSet);
            ClientScripting.Script.InitGameScript(Connection.ScriptingInfo.GameStyle);
        }

        void ConnectionError(object sender, EventArgs args)
        {
            lock (locker)
            {
                Done = true;
                LastError = "Connection Failure";
            }
        }

        void ConnectionEnded(object sender, EventArgs args)
        {
            lock (locker)
            {
                Done = true;
                LastError = "Connection Terminated";
            }
        }
       
        void State_MapLoaded(object sender, EventArgs e)
        {
            Vector3 pos = SetCameraZ(new Vector3(3, 3, 1));

            PlayerActor = State.AddActor(StandardActors.LocalPlayer) as LocalPlayer;

            PlayerActor.LastUpdatePostion = new Vector3(pos);
            PlayerActor.LastUpdateRotation = new Vector3(0, 0, 0);
            PlayerActor.LastUpdateTime = State.Now;
        }

        World GetSimpleWorld()
        {
            return WorldBuilder.NewWorld(string.Empty, null);
        }

        public void Load()
        {
            State.Load();
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Game;
using GridWorld;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using FileLocations;

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

        protected World MapWorld = null;
        protected object MapLocker = new object();

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

            State.GetWorld = GetWorld;
            State.MapLoaded += new EventHandler<EventArgs>(State_MapLoaded);

            InputTracker.RegisterControls += new EventHandler<EventArgs>(InputTracker_RegisterControls);
            InputTracker.LoadDefaultBindings += new EventHandler<EventArgs>(InputTracker_LoadDefaultBindings);

            ResourceProcessor.Client = this;
            ResourceProcessor.ResourcesComplete += new EventHandler<EventArgs>(ResourceLoadComplete);

            this.ResourcesComplete += new EventHandler<EventArgs>(LoadGame);
        }

        public void LoadGame(object sender, EventArgs e)
        {
            State.Load();
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

                Status = ServerConnection.ConnectionStatus.Disconnected;
                AddPendingEvents(StatusChanged);
            }
        }

        void ConnectionEnded(object sender, EventArgs args)
        {
            lock (locker)
            {
                Done = true;
                LastError = "Connection Terminated";

                Status = ServerConnection.ConnectionStatus.Disconnected;
                AddPendingEvents(StatusChanged);
            }
        }
       
        void State_MapLoaded(object sender, EventArgs e)
        {
            Vector3 pos = SetCameraZ(new Vector3(3, 3, 1));
// 
//             PlayerActor = State.AddActor(StandardActors.LocalPlayer) as LocalPlayer;
//  
//             PlayerActor.LastUpdatePostion = new Vector3(pos);
//             PlayerActor.LastUpdateRotation = new Vector3(0, 0, 0);
//             PlayerActor.LastUpdateTime = State.Now;
        }

        World GetWorld()
        {
            lock (MapLocker)
            {
                if (MapWorld != null)
                    return MapWorld;
            }
            return null;// WorldBuilder.NewWorld(string.Empty, null);
        }

        public bool HaveWorld(string hash)
        {
            string dir = Path.Combine(Locations.GetChacheFolder(),"maps");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string mapCache = Path.Combine(dir, hash);
            if (!Directory.Exists(mapCache))
                return false;

            lock (MapLocker)
            {
                MapWorld = World.ReadWorldWithGeometry(new FileInfo(Path.Combine(mapCache, "world.world")));

                if (MapWorld == null || MapWorld == World.Empty)
                    return false;
            }

            return true;   
        }

        public bool HaveScript(string name, string hash)
        {
            string dir = Path.Combine(Locations.GetChacheFolder(), "scripts");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string scriptCacheDir = Path.Combine(dir, Connection.ScriptingInfo.ScriptHash);
            if (!Directory.Exists(scriptCacheDir))
                return false;

            string scriptPath = Path.Combine(scriptCacheDir,hash);
            
            // check script file here!

            return true;
        }

        public void CacheWorld(World.WorldDefData buffer, string hash)
        {
            string dir = Path.Combine(Locations.GetChacheFolder(), "maps");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string mapCache = Path.Combine(dir, hash);
            if (!Directory.Exists(mapCache))
                Directory.CreateDirectory(mapCache);

            lock (MapLocker)
            {
                MapWorld = World.ReadWorldWithGeometry(buffer);
                MapWorld.SaveWorldWithGeometry(new FileInfo(Path.Combine(mapCache, "world.world")));
            }
        }

        public void Kill()
        {
            if (Connection != null)
                Connection.Kill();
        }
    }
}

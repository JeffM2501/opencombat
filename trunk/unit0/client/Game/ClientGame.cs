using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Game;
using Game.Messages;
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

        public ConnectInfo GameInfo = null;

        protected void CallDebugLogItem(string name, string value)
        {
            if (AddDebugLogItem != null)
                AddDebugLogItem(name, value);
        }

        public GameState State = null;
        protected InputSystem InputTracker = null;

        public ServerConnection Connection = null;

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
            string dir = Path.Combine(Locations.GetChacheFolder(), "scripts");
            if (Directory.Exists(dir))
            {
                string scriptCacheDir = Path.Combine(dir, Connection.ScriptingInfo.ScriptHash);
                ClientScripting.Script.Init(scriptCacheDir);

                AddPendingEvents(ScriptsLoaded);
            }

            State.Load();
        }

        public void Connect(string host, int port)
        {
            Connection = new ServerConnection(host, port);

            Connection.StatusChanged += new EventHandler<EventArgs>(ServerConnectionStatusChanged);
            Connection.Connected += new EventHandler<EventArgs>(ConnectionComplete);
            Connection.GameInfoLoaded += new EventHandler<ServerConnection.GameInfoEventArgs>(ConnectionGameInfoLoaded);
            Connection.Failed += new EventHandler<EventArgs>(ConnectionError);
            Connection.Disconnected += new EventHandler<EventArgs>(ConnectionEnded);
        }

        public void ConnectionGameInfoLoaded(object sender, ServerConnection.GameInfoEventArgs args)
        {
            ClientScripting.Script.InitGameScript(Connection.ScriptingInfo.GameStyle);
            GameInfo = args.Info;
        }

        void ConnectionComplete (object sender, EventArgs args)
        {
          //  ClientScripting.Script.Init(Connection.ScriptingInfo.ScriptSet);
            SendSystemMessage("Connection Complete");
        }

        void ConnectionError(object sender, EventArgs args)
        {
            lock (locker)
            {
                Done = true;
                LastError = "Connection Failure";
                SendSystemMessage(LastError);

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
                SendSystemMessage(LastError);
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

        public bool HaveResource(ResourceResponceMessage.Resource res)
        {
            if (res.ResType == ResourceResponceMessage.Resource.ResourceType.Map)
                return HaveWorld(res.Hash);
            else if (res.ResType == ResourceResponceMessage.Resource.ResourceType.Script)
                return HaveScript(res.Name, res.Hash);

            return false;
        }

        protected bool HaveWorld(string hash)
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

        protected bool HaveScript(string name, string hash)
        {
            string dir = Path.Combine(Locations.GetChacheFolder(), "scripts");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string scriptCacheDir = Path.Combine(dir, Connection.ScriptingInfo.ScriptHash);
            if (!Directory.Exists(scriptCacheDir))
                return false;

            string scriptPath = Path.Combine(scriptCacheDir,hash);

            if (!Directory.Exists(scriptPath))
                return false;

            string scriptFile = Path.Combine(scriptPath, name);
            if (!File.Exists(scriptFile))
                return false;

            string scriptHashFile = scriptFile + ".md5";

            return Utilities.GetMD5Hash(scriptFile, scriptHashFile) == hash;
        }

        public void CacheResource(ResourceResponceMessage.Resource res, byte[] buffer)
        {
            if (res.ResType == ResourceResponceMessage.Resource.ResourceType.Map)
                CacheWorld(buffer,res.Hash);
            else if (res.ResType == ResourceResponceMessage.Resource.ResourceType.Script)
                CacheScript(res,buffer);
        }

        protected void CacheScript(ResourceResponceMessage.Resource res, byte[] buffer)
        {
            string dir = Path.Combine(Locations.GetChacheFolder(), "scripts");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string scriptCacheDir = Path.Combine(dir, Connection.ScriptingInfo.ScriptHash);
            if (!Directory.Exists(scriptCacheDir))
                Directory.CreateDirectory(scriptCacheDir);

            string scriptPath = Path.Combine(scriptCacheDir, res.Hash);

            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);

            string scriptFile = Path.Combine(scriptPath, res.Name);
            if (!File.Exists(scriptFile))
                File.Delete(scriptFile);

            FileInfo file = new FileInfo(scriptFile);
            FileStream fs = file.OpenWrite();
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();

            string scriptHashFile = scriptFile + ".md5";
            if (!File.Exists(scriptHashFile))
                File.Delete(scriptHashFile);

            if (Utilities.GetMD5Hash(scriptFile, scriptHashFile) != res.Hash)
                throw (new Exception("Hash on save does not match for " + scriptFile));
        }

        protected void CacheWorld(byte[] data, string hash)
        {
            World.WorldDefData buffer = World.WorldDefData.Deserialize(data);

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

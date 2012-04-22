using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using InstanceConfig;
using Lidgren.Network;

using Game;
using Game.Messages;
using GridWorld;

namespace GameInstance
{
    class GameServer
    {
        public ManagerConnection Manager = null;

        public GameState State = null;

        protected GameMessageProcessor Processor = null;

        public GameServer()
        {
            if (Program.Config.ManagerAddress != string.Empty)
                Manager = new ManagerConnection();

            State = new GameState();
            State.GetWorld = LoadMap;
            State.Ready += new EventHandler<EventArgs>(State_Ready);

            State.Load();

            Player.NewPlayer += new Player.PlayerEvent(Player_NewPlayer);
            Player.DeletedPlayer += new Player.PlayerEvent(Player_DeletedPlayer);

            // TODO, start the net message thread here
            // that will update sim, process chat, and handle resource requests
        }

        void Player_DeletedPlayer(Player player)
        {
            // this is only to make combo debugging easier, we would NOT do this in production
            // we would tell the manager that we lost a player
            _Die = true;
        }

        void Player_NewPlayer(Player player)
        {
            ServerScripting.Script.NewPlayer(player);
            if (player.AvatarID < 0 && GameInfo.Info.PlayerAvatars.Count > 0) // give em a random one
                player.AvatarID = new Random().Next(GameInfo.Info.PlayerAvatars.Count-1);
        }

        void State_Ready(object sender, EventArgs e)
        {
            Processor = new GameMessageProcessor(State);
        }

        public void Kill()
        {
            if (Manager != null)
            {
                Manager.Send("System;Kill");
                Manager.KillMe();
            }

            if (Processor != null)
                Processor.Kill();

            Processor = null;
        }

        World LoadMap()
        {
            World Map = null;
            if (Program.Config.MapFilePath != string.Empty)
                Map = World.ReadWorldAndClusters(new System.IO.FileInfo(Program.Config.MapFilePath));
            else
                Map = WorldBuilder.NewWorld(string.Empty, null);

            Map.Finailize();

            if (Map == null || Map.Clusters.Count == 0)
            {
                if (Manager != null)
                    Manager.Send("Error;Map;NoLoad");
                
                _Die = true;
            }
            if (!_Die)
                LoadSettings();

            ResourceProcessor.AddResource(ResourceRequestMessage.MapResourceName, Map.SaveWorldWithGeometry().Serialize(),ResourceResponceMessage.Resource.ResourceType.Map); ;
            Map.FlushGeometry();

            return Map;
        }

        void LoadSettings()
        {
            ServerScripting.Script.Init(Program.Config.ScriptPath, State);

            // compute 
            GameInfo.Info.ClientScriptPack = Path.GetDirectoryName(Program.Config.ClientScripts);
            DirectoryInfo clientScriptDir = new DirectoryInfo(Program.Config.ClientScripts);

            GameInfo.Info.ClientScriptsHash = Utilities.GetMD5Hash(clientScriptDir);

            foreach (FileInfo file in clientScriptDir.GetFiles())
            {
                FileStream fs = file.OpenRead();
                byte[] buffer = new byte[file.Length];
                fs.Read(buffer, 0, (int)file.Length);
                fs.Close();

                ResourceProcessor.AddResource(file.Name, buffer, true, ResourceResponceMessage.Resource.ResourceType.Script);
            }

            ServerScripting.Script.GetGameInfo(GameInfo.Info);
        }

        protected bool _Die = false;

        public bool Die()
        {
            return _Die;
        }

        public void Update()
        {
            if (Manager != null)
            {
                string managerString = Manager.PopMessage();
                while (managerString != string.Empty)
                {
                    HandleManagerMeessages(managerString);
                    managerString = Manager.PopMessage();
                }
            }

            State.UpdateActors();
        }

        protected void HandleManagerMeessages(string msg)
        {
            string[] parts = msg.Split(";".ToCharArray());
            string code = parts[0];

            if (code == "Kill")
                _Die = true;
            else
            {

            }
        }
    }
}

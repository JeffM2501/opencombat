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

            // TODO, start the net message thread here
            // that will update sim, process chat, and handle resource requests
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

            ResourceProcessor.AddResource(ResourceRequestMessage.MapResourceName, Map.SaveWorldWithGeometry().Serialize()); ;
            Map.FlushGeometry();

            return Map;
        }

        void LoadSettings()
        {
            ServerScripting.Script.Init(Program.Config.ScriptPath, State);
            ServerScripting.Script.GetGameInfo(GameInfo.Info);
        }

        protected bool _Die = false;

        public bool Die()
        {
            return false;
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

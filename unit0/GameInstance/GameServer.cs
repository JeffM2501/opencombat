using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using InstanceConfig;
using Lidgren.Network;

using GridWorld;

namespace GameInstance
{
    class GameServer
    {
        public ManagerConnection Manager = null;

        public World Map = null;

        public GameServer()
        {
            if (Program.Config.ManagerAddress != string.Empty)
                Manager = new ManagerConnection();
            if (LoadMap())
            {

            }

            // TODO, start the net message thread here
            // that will update sim, process chat, and handle resource requests
        }

        public void Kill()
        {
            if (Manager != null)
            {
                Manager.Send("System;Kill");
                Manager.KillMe();
            }
        }

        public bool LoadMap()
        {
            Map = World.ReadWorldAndClusters(new System.IO.FileInfo(Program.Config.MapFilePath));

            if (Map == null || Map.Clusters.Count == 0)
            {
                if (Manager != null)
                    Manager.Send("Error;Map;NoLoad");
                
                _Die = true;
            }
            return true;
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

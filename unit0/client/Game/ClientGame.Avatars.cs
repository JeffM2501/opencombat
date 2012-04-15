using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ClientGame
    {
        public class PlayerModelDescriptor
        {
            public string Model = string.Empty;
            public List<string> TeamSkin = new List<string>();

            public PlayerModelDescriptor(string model)
            {
                Model = model; 
            }

            public void AddTeamSkin(string skin)
            {
                TeamSkin.Add(skin);
            }
        }

        public List<PlayerModelDescriptor> PlayerModels = new List<PlayerModelDescriptor>();

        public PlayerModelDescriptor AddModel(string model)
        {
            lock (PlayerModels)
            {
                PlayerModelDescriptor a = new PlayerModelDescriptor(model);
                PlayerModels.Add(a);
                return a;
            }
        }
    }
}

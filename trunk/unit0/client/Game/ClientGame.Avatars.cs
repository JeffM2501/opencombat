using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ClientGame
    {
        public class AvatarDescriptor
        {
            public string Model = string.Empty;
            public List<string> TeamSkin = new List<string>();

            public AvatarDescriptor(string model)
            {
                Model = model; 
            }

            public void AddTeamSkin(string skin)
            {
                TeamSkin.Add(skin);
            }
        }

        public List<AvatarDescriptor> Avatars = new List<AvatarDescriptor>();

        public AvatarDescriptor AddAvatar(string model)
        {
            lock (Avatars)
            {
                AvatarDescriptor a = new AvatarDescriptor(model);
                Avatars.Add(a);
                return a;
            }
        }
    }
}

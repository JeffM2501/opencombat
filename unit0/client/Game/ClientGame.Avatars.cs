using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Textures;
using FileLocations;

using WorldDrawing;

namespace Client
{
    public partial class ClientGame
    {
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

        protected int GetAvatarID(int id)
        {
            if (id < 0 || id >= GameInfo.PlayerAvatars.Count)
                return new Random().Next(GameInfo.PlayerAvatars.Count - 1);

            return id;
        }

        public string GetPlayerAvatar(UInt64 uid)
        {
            if (GameInfo.PlayerAvatars.Count == 0)
                return string.Empty;

            int id = -1;

            if (uid == MyPlayerID)
            {
               if(GameInfo.AvatarID >= 0)
                   id = GetAvatarID(GameInfo.AvatarID);
            }

            if (id < 0)
            {
                ChatProcessor.ChatUser chatUser = Connection.Chat.GetUserInfo(uid);
                id = GetAvatarID(chatUser.AvatarID);
                if (chatUser != ChatProcessor.ChatUser.Empty)
                    chatUser.AvatarID = id; // in case it was random
            }

            return GameInfo.PlayerAvatars[id];
        }
    }
}

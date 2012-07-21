using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using System.Drawing;

namespace Client
{
    public class ClientConfig
    {
        public static ClientConfig Config = new ClientConfig();
        public static string ConfigPath = string.Empty;

        public Size WindowSize = new Size(1280, 700);
        public bool FullScreen = false;

        // TODO when actual release happens, change this to NoPatch, and default to false
        public bool DEVELNOPatch = true;

        // TODO when actual release happens, change this to NoAuth, and default to false
        public bool DEVELNOAuth = true;

        public bool SaveCredentials = false;
        public string SavedUsername = string.Empty;
        public string SavedPassword = string.Empty;

        public bool AutoConnect = true;
        public string AutoConnectHost = "localhost";

        public string AuthenticationURL = "http://opencombat.invertedpolarity.com/users/auth.php";

        public static void Load(string path)
        {
            ConfigPath = path;

            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                Save();
                return;
            }

            FileStream fs = file.OpenRead();
            XmlSerializer xml = new XmlSerializer(typeof(ClientConfig));
            ClientConfig cfg = (ClientConfig)xml.Deserialize(fs);
            fs.Close();
            Config = cfg;
        }

        public static void Save()
        {
            FileInfo file = new FileInfo(ConfigPath);
            if (file.Exists)
                file.Delete();

            FileStream fs = file.OpenWrite();
            XmlSerializer xml = new XmlSerializer(typeof(ClientConfig));
            xml.Serialize(fs,Config);
            fs.Close();
        }
    }
}

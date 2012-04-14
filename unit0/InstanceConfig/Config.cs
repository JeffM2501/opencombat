using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace InstanceConfig
{
    public class Configuration
    {
        public static Configuration Empty = new Configuration();

        public string MapFilePath = string.Empty;
        public string MapHash = string.Empty;
        public string MapURL = string.Empty;

        public string GameMode = string.Empty;

        public int Port = 2501;
        public string ManagerAddress = string.Empty;

        public string Name = string.Empty;
        public UInt64 InstanceID = UInt64.MaxValue;

        public int MaxPlayers = 100;

        public string ScriptPath = "../../../server_data/test/server/";

        public string ResourceHost = string.Empty;

        public static Configuration Read(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
                return Empty;

            FileStream fs = file.OpenRead();
            XmlSerializer xml = new XmlSerializer(typeof(Configuration));
            Configuration c = (Configuration)xml.Deserialize(fs);
            fs.Close();
            return c;
        }

        public bool Write(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
                file.Delete();

            try
            {
                FileStream fs = file.OpenWrite();
                XmlSerializer xml = new XmlSerializer(typeof(Configuration));
                xml.Serialize(fs,this);
                fs.Close();
            }
            catch (System.Exception /*ex*/)
            {
                return false;
            }
            return true;
        }
    }
}

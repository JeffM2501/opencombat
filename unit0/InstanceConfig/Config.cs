using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace InstanceConfig
{
    public class Config
    {
        public static Config Empty = new Config();

        public string MapFilePath = string.Empty;
        public string GameMode = string.Empty;

        public int Port = -1;
        public string ManagerAddress = string.Empty;

        public string Name = string.Empty;

        public static Config Read(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
                return Empty;

            FileStream fs = file.OpenRead();
            XmlSerializer xml = new XmlSerializer(typeof(Config));
            Config c = (Config)xml.Deserialize(fs);
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
                XmlSerializer xml = new XmlSerializer(typeof(Config));
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

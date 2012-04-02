/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace P2501Client
{
    public class ClientPrefs
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public FileInfo PrefsFile = null;

        public List<string> Accounts = new List<string>();

        public class GraphicsSettings
        {
            public Size Screen = new Size(1024, 640);
            public bool Fullscreen = false;
        }

        public GraphicsSettings Graphics = new GraphicsSettings();

        public static FileInfo GetDefaultPrefsFile ()
        {
            DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
            if (!AppSettingsDir.Exists)
                AppSettingsDir.Create();

            DirectoryInfo appDir = AppSettingsDir.CreateSubdirectory("Client");

            return new FileInfo(Path.Combine(appDir.FullName, "prefs.xml"));
        }

        public static ClientPrefs Read(FileInfo file)
        {
            ClientPrefs prefs;
            if (!file.Exists)
            {
                prefs = new ClientPrefs();
                prefs.PrefsFile = file;
                return prefs;
            }

            XmlSerializer XML = new XmlSerializer(typeof(ClientPrefs));
            FileStream stream = file.OpenRead();
            try
            {
                prefs = (ClientPrefs)XML.Deserialize(stream);
                stream.Close();
            }
            catch (System.Exception /*ex*/)
            {
                stream.Close();
                file.Delete();
                prefs = new ClientPrefs();
            }
            prefs.PrefsFile = file;
            return prefs;
        }

        public bool Write()
        {
            XmlSerializer XML = new XmlSerializer(typeof(ClientPrefs));

            PrefsFile.Delete();
            FileStream stream = PrefsFile.OpenWrite();
            XML.Serialize(stream, this);
            stream.Close();
            return PrefsFile.Exists;
        }
    }
}

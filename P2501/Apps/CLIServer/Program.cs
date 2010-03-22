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
using System.Linq;
using System.Text;
using System.Threading;

using Project2501Server;

using ServerConfigurator;

namespace CLIServer
{
    class Program
    {
        static ServerConfig config = null;

        static void Main(string[] args)
        {
            string configFile = "config.xml";

            if (args.Length > 0)
                configFile = args[0];

            config = new ServerConfig(configFile);

            if (config.ConfigItems.Count == 0)
                LoadDefaultConfig(config);

            int sleepTime = 10;

            Server server = new Server(config.GetInt("port"));
            server.PublicListInfo = new PublicListCallback(PublicListCallback);

            server.Init();
            while (true)
            {
                server.Service();
                Thread.Sleep(sleepTime);
            }
        }

        static void LoadDefaultConfig(ServerConfig config)
        {
            config.SetItem("port", "2501");
            config.SetItem("host", "localhost:2501");
            config.SetItem("description", "Default Test Server Run by CLIServer");
            config.SetItem("name", "TestServer");

            config.Save();
        }

        static bool PublicListCallback(ref string host, ref string name, ref string description, ref string key, ref string type)
        {
            host = config.GetItem("host");
            name = config.GetItem("name");
            description = config.GetItem("description");
            key = config.GetItem("key");
            type = config.GetItem("type");

            return true;
        }
    }
}

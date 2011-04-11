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
using System.Windows.Forms;
using System.IO;

using Project2501Server;
using ServerConfigurator;

namespace P2501Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool done = false;
            while (!done)
            {
                StartupForm startupForm = new StartupForm();
                Application.Run(startupForm);

                if (startupForm.OkToPlay)
                {
                    Server server = null;
                    

                    if (startupForm.StartPublicServer)
                    {
                        ServerConfig config = new ServerConfig();
                        LoadDefaultConfig(config);

                        server = new Server(config.GetInt("port"));
                        server.DefaultInstanceSetup = new DefaultInstanceSetupCallback(SetupInstance);
                        server.Run();
                    }
                    new Game(startupForm.ConnectHost, startupForm.UID, startupForm.Token, startupForm.CharacterID).Run();

                    if (server != null)
                    {
                        server.Kill();
                        server = null;
                    }
                }
                else
                    done = true;
            }
        }

        static void SetupInstance(ref ServerInstanceSettings settings)
        {
            settings.MapFile = WorldBuilder.BuildDefaultWorld(Path.GetTempFileName());
        }

        static void LoadDefaultConfig(ServerConfig config)
        {
            config.SetItem("port", "2501");
            config.SetItem("host", "localhost:2501");
            config.SetItem("description", "Default Test Server Run by client");
            config.SetItem("name", "TestServer");
        }
    }
}

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
using System.Text;
using System.Threading;
using System.Diagnostics;

using Project2501Server;
using P2501GameClient;
using Simulation;

namespace ClientServerTestHarnes
{
    class Program
    {
        static FileInfo map;

        static object locker = new object();
        static bool serverUp = false;

        static void Main(string[] args)
        {
            Console.WriteLine("System Startup");

            map = new FileInfo("./map.PortalMap");
            if (!map.Exists)
            {
                map = new FileInfo("../map.PortalMap");
                if (!map.Exists)
                {
                    map = new FileInfo("../../map.PortalMap");
                    if (!map.Exists)
                    {
                        map = new FileInfo("../../../map.PortalMap");
                        if (!map.Exists)
                        {
                            Console.WriteLine("Map not found");
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("Starting Server");
            Server server = new Server(2501);
            server.DefaultInstanceSetup = new DefaultInstanceSetupCallback(defaultInstCB);
            server.NoTokenCheck = true;
            server.InstanceStarted +=new InstanceNotifiactionEventHandler(server_InstanceStarted);
            if (!server.Run())
            {
                Console.WriteLine("Server failed to start");
                return;
            }

            while (true)
            {
                lock(locker)
                {
                    if (serverUp)
                        break;
                }
                Thread.Sleep(100);
            }
            Console.WriteLine("Server Started");

            GameClient client = new GameClient("localhost", 2501);
            client.CacheFileDir = new DirectoryInfo("./");

            client.GetAuthentication = new AuthenticationCallback(authCB);
            client.InstanceList += new GeneralEventHandler(client_InstanceListEvent);
            client.LoginAccepted += new GeneralEventHandler(client_LoginAcceptEvent);
            client.HostConnected += new HostConnectionHandler(client_HostConnectionEvent);
            client.InstanceSettingsReceived += new GeneralEventHandler(client_InstanceSettingsReceived);

            client.StartMapTransfer += new GeneralEventHandler(client_StartMapTransfer);
            client.EndMapTransfer += new GeneralEventHandler(client_EndMapTransfer);
            client.FileTransferProgress += new FileTransferProgressEventHandler(client_FileTransferProgress);

            client.MapLoaded += new GeneralEventHandler(client_MapLoaded);
            client.MapLoadFailed += new GeneralEventHandler(client_MapLoadFailed);
            
            while ( true )
            {
                if (!client.Update())
                    break;
            }

            client.Kill();
            server.Kill();
        }

        static void client_MapLoadFailed(object sender, EventArgs args)
        {
            Console.WriteLine("**ERROR Map load failed**");
        }

        static void client_InstanceSettingsReceived(object sender, EventArgs args)
        {
            GameClient client = sender as GameClient;
            if (client != null)
                client.SetPreferedTeam(-1);

            Console.WriteLine("Settings Received");
        }

        static void client_MapLoaded(object sender, EventArgs args)
        {
            Console.WriteLine("Map loaded");
        }

        static void client_FileTransferProgress(object sender, FileTransferProgressEventArgs args)
        {
            Console.Write("=");
        }

        static void client_EndMapTransfer(object sender, EventArgs args)
        {
            Console.WriteLine(">");
        }

        static void client_StartMapTransfer(object sender, EventArgs args)
        {
            Console.Write("Map Transfer <");
        }

        static void server_InstanceStarted(object sender, InstanceEventArgs args)
        {
            lock(locker)
            {
                serverUp = true;
            }
        }

        static void client_HostConnectionEvent(object sender, HostConnectionEventArgs args)
        {
            Console.WriteLine("Client connected " + args.Message);
        }

        static void client_LoginAcceptEvent(object sender, EventArgs args)
        {
            Console.WriteLine("Login Accepted");
        }

        static void client_InstanceListEvent(object sender, EventArgs args)
        {
            GameClient client = sender as GameClient;
            if (client == null || client.ServerInstances.Length == 0)
                return;

            Console.WriteLine("Joining instance " + client.ServerInstances[0].ID.ToString());

            client.SelectInstance(client.ServerInstances[0].ID);
        }

        static void authCB(ref UInt64 UID, ref UInt64 CID, ref UInt64 Token)
        {
            Console.WriteLine("Sending Auth");
            UID = 1;
            CID = 1;
            Token = 1;
        }

        static void defaultInstCB(ref ServerInstanceSettings settings)
        {
            settings.MapFile = map;
            settings.Settings.GameMode = GameType.TeamDeathMatch;
        }
    }
}

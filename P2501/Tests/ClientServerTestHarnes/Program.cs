using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

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

            client.GetAuthentication = new AuthenticationCallback(authCB);
            client.InstanceListEvent += new GeneralEventHandler(client_InstanceListEvent);
            client.LoginAcceptEvent += new GeneralEventHandler(client_LoginAcceptEvent);
            client.HostConnectionEvent += new HostConnectionHandler(client_HostConnectionEvent);
            
            while ( true )
            {
                if (!client.Update())
                    break;
            }

            client.Kill();
            server.Kill();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Project2501Server;
using P2501GameClient;

namespace ClientServerTestHarnes
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(2501);
            server.NoTokenCheck = true;
            server.Run();

            GameClient client = new GameClient("localhost", 2501);

            client.GetAuthentication = new AuthenticationCallback(authCB);
            client.InstanceListEvent += new GeneralEventHandler(client_InstanceListEvent);
            client.LoginAcceptEvent += new GeneralEventHandler(client_LoginAcceptEvent);
            client.HostConnectionEvent += new HostConnectionHandler(client_HostConnectionEvent);
            while ( true )
            {
                client.Update();
            }

            client.Kill();
            server.Kill();
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
    }
}

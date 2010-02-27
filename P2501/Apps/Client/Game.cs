using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using P2501GameClient;

namespace P2501Client
{
    class Game
    {
        ClientPrefs Prefs = null;

        Visual visual = null;
        GameClient Client;

        public Game(string host, UInt64 uid, UInt64 token, UInt64 cid)
        {
            Prefs = ClientPrefs.Read(ClientPrefs.GetDefaultPrefsFile());
        }

        public void Init ()
        {
            GameWindowFlags flags = GameWindowFlags.Default;
            if (Prefs.Graphics.Fullscreen)
                flags = GameWindowFlags.Fullscreen;

            visual = new Visual(Prefs.Graphics.Screen.Width, Prefs.Graphics.Screen.Height, GraphicsMode.Default, flags);

            visual.Update += new Visual.UpdateEventHandler(Update);

            Client = new GameClient("localhost",2501);
        }

        public void Cleanup()
        {
            Client.Kill();
        }

        void Update (Visual sender, double time)
        {

        }

        public void Run ()
        {
            Init();

            visual.Run();

            Cleanup();
        }
    }
}

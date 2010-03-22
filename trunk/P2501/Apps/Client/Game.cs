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

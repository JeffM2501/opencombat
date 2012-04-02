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
using System.Threading;
using System.Diagnostics;

namespace Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args)
        {
            DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
            if (!AppSettingsDir.Exists)
                AppSettingsDir.Create();

            DirectoryInfo appDir = AppSettingsDir.CreateSubdirectory("Client");
            DirectoryInfo updateDir = AppSettingsDir.CreateSubdirectory("Updates");

            FileInfo newGuy = new FileInfo(Path.Combine(updateDir.FullName, "Launcher.exe"));

            string theAppDir = Path.GetDirectoryName(Application.ExecutablePath);
            if (args.Length > 0)
                theAppDir = args[0];

            if (newGuy.Exists && Application.ExecutablePath != newGuy.FullName)
            {
                StartApp(newGuy.FullName,theAppDir);
                return;
            }
            FileInfo patcher = new FileInfo(Path.Combine(updateDir.FullName,"Patcher.exe"));
            if (!patcher.Exists)
                patcher = new FileInfo(Path.Combine(theAppDir,"Patcher.exe"));

            FileInfo patcherVer = new FileInfo(Path.Combine(Path.GetDirectoryName(patcher.FullName),"Patcher_vers.txt"));
                  
            PatchUpdater updater = new PatchUpdater();
            if (patcherVer.Exists)
            {
                Stream v = patcherVer.OpenRead();
                StreamReader reader = new StreamReader(v);
                updater.existingVers = reader.ReadLine();
                reader.Close();
                v.Close();
            }

            if (updater.ShowDialog() == DialogResult.Cancel)
                return;

            patcher = new FileInfo(Path.Combine(updateDir.FullName,"Patcher.exe"));
            if (patcher.Exists)
            {  
                StartApp(patcher.FullName,theAppDir);
                return;
            }

            patcher = new FileInfo(Path.Combine(theAppDir, "Patcher.exe"));
            if (patcher.Exists)
            {
                StartApp(patcher.FullName,theAppDir);
                return;
            }

            MessageBox.Show("Unable to start patcher");
        }

        public static void StartApp ( string path, string args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32S)
              Process.Start(path, args);
          else
              Process.Start("mono", path + " " + args);
        }
    }
}

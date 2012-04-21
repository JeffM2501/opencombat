﻿/*
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
using System.Windows.Forms;
using System.Diagnostics;

namespace Patcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Patcher must be called from launcher");
                return;
            }

            string theAppDir = args[0];

            DirectoryInfo AppSettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projekt2501"));
            if (!AppSettingsDir.Exists)
                AppSettingsDir.Create();

            DirectoryInfo prefsDir = AppSettingsDir.CreateSubdirectory("Client");
            DirectoryInfo updateDir = AppSettingsDir.CreateSubdirectory("Updates");

            FileInfo file = new FileInfo(Path.Combine(updateDir.FullName, "client_version.txt"));
            if (!file.Exists)
                file = new FileInfo(Path.Combine(theAppDir, "client_version.txt"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PatchForm form = new PatchForm(file);
            if (form.ShowDialog() == DialogResult.Cancel)
                return;

            FileInfo client = new FileInfo(Path.Combine(updateDir.FullName, "P2501Client.exe"));
            if (client.Exists)
            {
                StartApp(client.FullName, theAppDir);
                return;
            }

            client = new FileInfo(Path.Combine(theAppDir, "P2501Client.exe"));
            if (client.Exists)
            {
                StartApp(client.FullName, theAppDir);
                return;
            }

            MessageBox.Show("Unable to start client");
        }

        public static void StartApp(string path, string args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32S)
                Process.Start(path, args);
            else
                Process.Start("mono", path + " " + args);
        }
    }
}
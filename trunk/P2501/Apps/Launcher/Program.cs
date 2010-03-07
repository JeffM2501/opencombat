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

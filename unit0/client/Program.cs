﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using FileLocations;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (CheckStartup(args))
                return; // we bail here if we got redirected to a newer exe

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InputSystem.GetBindingFilePath += GetBindingFile;
            ClientConfig.Load(GetConfigFIle());

            bool done = false;

            //bool runOnce = false;
            while (!done)
            {
                Launcher launcher = new Launcher();
                launcher.CheckForUpdates = false; //!runOnce;

                if (launcher.Start())
                {
                   // runOnce = true;
                    using (MainWindow win = new MainWindow(launcher))
                    {
                        win.Run();
                        done = win.QuitOnExit;
                    }
                }
                else
                    done = true;
            }
        }

        static string GetConfigFIle()
        {
            return Path.Combine(Application.UserAppDataPath, "config.xml");
        }

        static string GetBindingFile()
        {
            return Path.Combine(Application.UserAppDataPath, "input.bindings");
        }

        static bool CheckStartup( string[] args )
        {
            string commonPath = string.Empty;
            try
            {
                // linux may not have an accesable common folder, so we don't set it if we have to
                commonPath = Application.CommonAppDataPath;
            }
            catch (System.Exception /*ex*/)
            {
            	
            }

            Locations.DataDirName = "base_data";

            Locations.Startup(Application.StartupPath,commonPath,Application.UserAppDataPath,Path.GetFileName(Application.ExecutablePath),args);
            return Locations.LaunchHigherExe();

        }
    }
}

using System;
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

            bool done = false;

            //bool runOnce = false;
            while (!done)
            {
                PreGame game = new PreGame();
                game.CheckForUpdates = false; //!runOnce;

                if (game.ShowDialog() == DialogResult.OK)
                {
                   // runOnce = true;
                    using (MainWindow win = new MainWindow())
                    {
                        win.Run();
                        done = win.QuitOnExit || game.AutoPlay;
                    }
                }
                else
                    done = true;
            }
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

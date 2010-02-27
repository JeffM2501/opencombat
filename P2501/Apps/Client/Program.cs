using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace P2501Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool done = false;
            while (!done)
            {
                StartupForm startupForm = new StartupForm();
                Application.Run(startupForm);

                if (startupForm.Token != 0)
                    new Game(startupForm.ConnectHost, startupForm.UID, startupForm.Token, startupForm.CharacterID).Run();
                else
                    done = true;
            }
        }
    }
}

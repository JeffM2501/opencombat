using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Web;

using Game;

using System.Windows.Forms;

using FileLocations;

namespace Client
{
    public class LauncherInterface
    {
        public virtual void Init(Launcher launcher)
        {
        }

        public virtual bool Start()
        {
            return false;
        }
    }

    public class DefaultLauncherInterface : LauncherInterface
    {
        Launcher TheLauncher = null;

        DefaultLauncher LauncherDlog = null;

        public override void Init(Launcher launcher)
        {
            TheLauncher = launcher;
            FailoverBrowser.UseBrowser = Utilities.GetRealPlatform() == Utilities.PlatformType.Windows;

            LauncherDlog = new DefaultLauncher(TheLauncher);

            TheLauncher.StartPatch += new EventHandler<EventArgs>(TheLauncher_StartPatch);
            TheLauncher.EndPatch += new EventHandler<EventArgs>(TheLauncher_EndPatch);
            TheLauncher.PatchRequresRestart += new EventHandler<EventArgs>(TheLauncher_PatchRequresRestart);
            TheLauncher.StartPatchDownload += new EventHandler<EventArgs>(TheLauncher_StartPatchDownload);
        }

        void TheLauncher_StartPatchDownload(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void TheLauncher_PatchRequresRestart(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void TheLauncher_EndPatch(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void TheLauncher_StartPatch(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override bool Start()
        {
            return LauncherDlog.ShowDialog() == DialogResult.OK;
        }
    }

    public class Launcher
    {
        public string NewsURL = string.Empty;

        public string Username = string.Empty;
        public string Password = string.Empty;

        public bool AutoConnect = false;

        public bool SaveCredentials = false;

        public string Host = string.Empty;
        public List<string> SavedHosts = new List<string>();

        public List<string> Callsigns = new List<string>();
        public int CallsignIndex = -1;

        public UInt64 UID = UInt64.MaxValue;
        public string Token = string.Empty;

        public event EventHandler<EventArgs> StartPatch;
        public event EventHandler<EventArgs> EndPatch;
        public event EventHandler<EventArgs> StartPatchDownload;

        public event EventHandler<EventArgs> PatchRequresRestart;

        public string RegisterURL = "http://opencombat.invertedpolarity.com/users/register.php";
        public string AuthenticateURL = "http://opencombat.invertedpolarity.com/users/authenticate.php";

        protected Thread PatchThread = null;

        public bool CheckForUpdates = true;
        public bool ShowAuthentication = false;

        public bool Start()
        {
            NewsURL = "http://opencombat.invertedpolarity.com/wiki/wikka.php?wakka=Unit0News";
            if (ClientConfig.Config.SaveCredentials)
            {
                SaveCredentials = true;
                Username = ClientConfig.Config.SavedUsername;
                Password = ClientConfig.Config.SavedPassword; // TODO crypto this
            }
            AutoConnect = ClientConfig.Config.AutoConnect;
            if (AutoConnect)
                Host = ClientConfig.Config.AutoConnectHost;
            else
                Host = "localhost";

            ShowAuthentication = !ClientConfig.Config.DEVELNOAuth;
            CheckForUpdates = !ClientConfig.Config.DEVELNOPatch;

            // see if we can find an OS specific DLL
            string OS = Utilities.GetRealPlatform().ToString();
            string dll = Path.Combine(Locations.ThisExePath, "LauncherInterface." + OS + ".dll");

            LauncherInterface launcher = GetLauncher(dll);
            if (launcher == null)
                launcher = new DefaultLauncherInterface();

            launcher.Init(this);
            // start the patcher

            if (CheckForUpdates)
            {
                PatchThread = new Thread(new ThreadStart(Patch));
                PatchThread.Start();
            }   

            bool result = launcher.Start(); // this thread will block here while the dialog is up
            if (PatchThread != null)
            {
                PatchThread.Abort();
                PatchThread = null;

                if (patcher != null)
                    patcher.KillUpdate();

                patcher = null;
            }

            ClientConfig.Config.SaveCredentials = SaveCredentials;
            if (SaveCredentials)
            {
                ClientConfig.Config.SavedUsername = Username;
                ClientConfig.Config.SavedPassword = Password; // TODO crypto this
            }
            else
            {
                ClientConfig.Config.SavedUsername = string.Empty;
                ClientConfig.Config.SavedPassword = string.Empty;
            }

            ClientConfig.Config.AutoConnect = AutoConnect;
            ClientConfig.Config.AutoConnectHost = Host;

            ClientConfig.Save();

            return result;
        }

        protected LauncherInterface GetLauncher(string filename)
        {
            if (!File.Exists(filename))
                return null;

            Assembly module = null;
            
            try
            {
                 module = Assembly.LoadFile(filename);
            if (module == null)
                return null;
            }
            catch (System.Exception /*ex*/)
            {
            	return null;
            }

            foreach (Type t in module.GetTypes())
            {
                if (t.IsSubclassOf(typeof(LauncherInterface)))
                    return (LauncherInterface)Activator.CreateInstance(t);
            }

            return null;
        }

        public bool Authenticate()
        {
            if (ClientConfig.Config.DEVELNOAuth)
            {
                Callsigns.Add("Player");
                UID = 1;
                Token = "NOPE";
                return true;
            }

            if (AuthenticateURL == string.Empty)
            {
                WebClient client = new WebClient();
                string responce = client.DownloadString(AuthenticateURL + "?username=" + HttpUtility.UrlEncode(Username) + "&password=" + HttpUtility.UrlEncode(Password));
                if (!responce.Contains("ERROR"))
                {
                    string[] parts = responce.Split(" ".ToCharArray());
                    if (parts.Length < 3)
                        return false;

                    UInt64.TryParse(parts[0], out UID);
                    Token = parts[1];

                    Callsigns.Clear();

                    for (int i = 2; i < parts.Length; i++ )
                        Callsigns.Add(parts[i]);
                }
            }
            return false;
        }

        public void Register()
        {
            Process.Start(RegisterURL);
        }

        Patcher patcher = null;

        public void Patch()
        {
            patcher = new Patcher();
            if (StartPatch != null)
                StartPatch(this, EventArgs.Empty);

            patcher.BuildHashTables();
            patcher.StartUpdate();

            bool startedData = false;

            while (!patcher.UpdateDone())
            {
                switch (patcher.GetStatus())
                {
                    case Patcher.Status.BuldingHashes:
                        break;

                    case Patcher.Status.GettingDataList:
                        break;

                    case Patcher.Status.GettingDataFiles:
                        if (!startedData)
                        {
                            if (StartPatchDownload != null)
                                StartPatchDownload(this, EventArgs.Empty);
                            startedData = true;
                        }
                        break;
                }
                Thread.Sleep(100);
            }

            //check for bins here and restart

            PatchThread = null;
            patcher = null;

            if (EndPatch != null)
                EndPatch(this, EventArgs.Empty);
        }
    }
}

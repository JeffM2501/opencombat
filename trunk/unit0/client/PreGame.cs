using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using FileLocations;

namespace Client
{
    public partial class PreGame : Form
    {
        public interface BrowserManagement
        {
            void Init(System.Windows.Forms.Control parent, string url);
            void Resize(int x, int y);
        }

        protected BrowserManagement BrowserManagerInterface = null;

        public bool CheckForUpdates = false;
        protected int DrawCount = 0;
        protected bool CheckedForUpdates = false;

        public bool AutoPlay = true;

        object WebBrowser = new object();

        public PreGame()
        {
            InitializeComponent();
        }

        private void Play_BN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        protected void SetupWinBrowser( string url)
        {
            System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
            WebBrowser = browser;
            WebFrame.Controls.Add(browser);
            browser.Size = WebFrame.Size;
            browser.Navigate(url);
        }

        protected void ResizeWinBrowser()
        {
            System.Windows.Forms.WebBrowser browser = WebBrowser as System.Windows.Forms.WebBrowser;
            if (browser == null)
                return;
            browser.Size = WebFrame.Size;
        }

        protected void SetupMonoBrowser(string url)
        {
            string browserManagerFile = Path.Combine(Locations.ThisExePath,"BrowserManager.dll");

            try
            {
                Assembly manager = Assembly.LoadFile(browserManagerFile);
                if (manager == null)
                    return;

                Type managerType = null;
                foreach (Type t in manager.GetTypes())
                {
                    Type interfaceType = t.GetInterface(typeof(BrowserManagement).Name);
                    if (interfaceType != null)
                    {
                        managerType = t;
                        break;
                    }
                }

                if (managerType == null)
                    return;

                BrowserManagerInterface = (BrowserManagement)Activator.CreateInstance(managerType);

                BrowserManagerInterface.Init(WebFrame,url);
            }
            catch (System.Exception /*ex*/)
            {
            	
            }
        }

        protected void ResizeMonoBrowser()
        {
            if (BrowserManagerInterface == null)
                return;

            BrowserManagerInterface.Resize(WebFrame.Size.Width, WebFrame.Size.Height);
        }

        private void SetupBrowser( string url)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix && System.Environment.OSVersion.Platform != PlatformID.MacOSX)
                SetupWinBrowser(url);
            else
                SetupMonoBrowser(url);
        }

        private void PreGame_Load(object sender, EventArgs e)
        {
            Play_BN.Enabled = false;
            SetupBrowser("http://www.opencombat.net/wiki/wikka.php?wakka=Unit0News");
            if (!CheckForUpdates && AutoPlay)
                Play_BN_Click(this, EventArgs.Empty);

//             Update();
//             if (CheckForUpdates)
//             {
//                 UpdateProgress updater = new UpdateProgress();
// 
//                 if (updater.ShowDialog(this) == DialogResult.Cancel)
//                 {
//                     DialogResult = DialogResult.Cancel;
//                     this.Close();
//                 }
//                 // updates done
//                 int i = 0;
//             }
        }

        private void PreGame_Paint(object sender, PaintEventArgs e)
        {
            if (CheckForUpdates && !CheckedForUpdates)
            {
                DrawCount++;
                if (DrawCount > 1)
                {
                    if (CheckForUpdates)
                    {
                        UpdateProgress updater = new UpdateProgress();

                        if (updater.ShowDialog(this) == DialogResult.Cancel)
                        {
                            DialogResult = DialogResult.Cancel;
                            this.Close();
                        }
                        // updates done
                        CheckedForUpdates = true;
                        Play_BN.Enabled = true;

                        if (AutoPlay)
                            Play_BN_Click(this, EventArgs.Empty);
                    }
                }
                this.Invalidate();
            }
        }

        private void WebFrame_Resize(object sender, EventArgs e)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix && System.Environment.OSVersion.Platform != PlatformID.MacOSX)
                ResizeWinBrowser();
            else
                ResizeMonoBrowser();
        }
    }
}

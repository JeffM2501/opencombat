using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class DefaultLauncher : Form
    {
        Launcher TheLauncher = null;

        public bool Authenticated = false;
        public bool UpdatesDone = false;

        public DefaultLauncher( Launcher launcher )
        {
            InitializeComponent();

            TheLauncher = launcher;

            TheLauncher.StartPatch += new EventHandler<EventArgs>(TheLauncher_StartPatch);
            TheLauncher.EndPatch += new EventHandler<EventArgs>(TheLauncher_EndPatch);
            TheLauncher.PatchRequresRestart += new EventHandler<EventArgs>(TheLauncher_PatchRequresRestart);
            TheLauncher.StartPatchDownload += new EventHandler<EventArgs>(TheLauncher_StartPatchDownload);
            PatchStatusLabel.Text = "Patch Status: None";

            ServerPulldown.Text = TheLauncher.Host;
        }

        void TheLauncher_StartPatchDownload(object sender, EventArgs e)
        {
            PatchStatusLabel.Text = "Patch Status: Download starting";
        }

        void TheLauncher_PatchRequresRestart(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        void TheLauncher_EndPatch(object sender, EventArgs e)
        {
            UpdatesDone = true;
            Play_BN.Enabled = Authenticated;

            if (Play_BN.Enabled && TheLauncher.AutoConnect)
                Play_BN_Click(this, EventArgs.Empty);
        }

        void TheLauncher_StartPatch(object sender, EventArgs e)
        {
            Play_BN.Enabled = false;
        }

        private void DefaultLauncher_Load(object sender, EventArgs e)
        {
            NewsBrowser.Navigate(TheLauncher.NewsURL);

            Play_BN.Enabled = TheLauncher.CheckForUpdates;
            LoginButton.Enabled = false;
            CallsignList.Enabled = false;
            UpdatesDone = !TheLauncher.CheckForUpdates;
            AutoPlay.Checked = TheLauncher.AutoConnect;

            if (TheLauncher.SaveCredentials)
            {
                Email.Text = TheLauncher.Username;
                Password.Text = TheLauncher.Password;

                if (LoginButton.Enabled)
                    LoginButton_Click(this, EventArgs.Empty);
            }

            if (!TheLauncher.ShowAuthentication)
            {
                AuthGroup.Enabled = false;
                LoginButton_Click(this,EventArgs.Empty);
            }
        }

        private void RegisterLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TheLauncher.Register();
        }

        private void Play_BN_Click(object sender, EventArgs e)
        {
            TheLauncher.Host = ServerPulldown.Text;
            TheLauncher.CallsignIndex = CallsignList.SelectedIndex;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Authenticated = TheLauncher.Authenticate();
            CallsignList.Enabled = Authenticated;

            if (CallsignList.Enabled)
            {
                foreach (string callsign in TheLauncher.Callsigns)
                    CallsignList.Items.Add(callsign);

                int index = 0;
                if (TheLauncher.CallsignIndex >= 0)
                    index = TheLauncher.CallsignIndex;

                CallsignList.SelectedIndex = index;

                if (Play_BN.Enabled && TheLauncher.AutoConnect)
                    Play_BN_Click(this, EventArgs.Empty);
            }
        }

        protected void CheckLoginButton()
        {
            LoginButton.Enabled = (TheLauncher.Username != string.Empty && TheLauncher.Password != string.Empty);
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            TheLauncher.Username = Email.Text;
            CheckLoginButton();
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            TheLauncher.Password = Password.Text;
            CheckLoginButton();
        }

        private void SaveAuth_CheckedChanged(object sender, EventArgs e)
        {
            TheLauncher.SaveCredentials = SaveAuth.Checked;
        }

        private void CallsignList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Play_BN.Enabled = UpdatesDone;
        }

        private void AutoPlay_CheckedChanged(object sender, EventArgs e)
        {
            TheLauncher.AutoConnect = AutoPlay.Checked;
        }
    }
}

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace P2501Client
{
    public partial class StartupForm : Form
    {
        public UInt64 UID = 0;
        public UInt64 Token = 0;
        public UInt64 CharacterID = 0;
        public string ConnectHost = string.Empty;

        public bool OkToPlay = false;

        GameList gameList = null;

        Timer timer;

        ClientPrefs prefs;

        public StartupForm()
        {
            InitializeComponent();

            prefs = ClientPrefs.Read(ClientPrefs.GetDefaultPrefsFile());
        }

        private void StartupForm_Load(object sender, EventArgs e)
        {
            NewsBrowser.Navigate("http://www.opencombat.net/?page_id=17");

            UpdateUIStates();

            foreach (string name in prefs.Accounts)
                Username.Items.Add(name);

            Username.SelectedIndex = 0;

            timer = new Timer();
            timer.Interval = 15000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (gameList == null || !gameList.Dirty)
                return;

            List<GameList.ListedServer> list = gameList.GetGameServers();

            PreferedList.Items.Clear();

            foreach(GameList.ListedServer item in list)
            {
                int image = 0;
                if (item.Group == "NULL")
                    image = 1;

                AddServerToList(item, PreferedList, image);
            }
        }

        public void AddServerToList ( GameList.ListedServer server, ListView view, int image )
        {
            string groupName = server.Group;
            if (groupName == "NULL")
                groupName = "Open";

            ListViewGroup group = null;
            foreach ( ListViewGroup g in view.Groups )
            {
                if (g.Name == groupName)
                    group = g;
            }
            if (group == null)
            {
                group = new ListViewGroup(groupName, groupName);
                view.Groups.Add(group);
            }

            string[] labels = new string[] {server.Name,server.Description,"0",server.Host};

            ListViewItem item = new ListViewItem(labels, image, group);
            item.ToolTipText = "connection: unknown";
            item.Tag = server;

            view.Items.Add(item);
        }

        protected void UpdateUIStates ()
        {
            CallsignsGroup.Enabled = Token != 0;
            GamesGroup.Enabled = Token != 0;

            if (Token == 0)
                Password.Enabled = Username.Text != string.Empty;
            else
                Password.Enabled = false;

            if (Token == 0)
                LoginButton.Enabled = Username.Text != string.Empty && Password.Text != string.Empty && Password.Enabled;
            else
                LoginButton.Enabled = true;

            Username.Enabled = Token == 0;
            RegisterButton.Enabled = Token == 0;

            Play.Enabled = false;

            if (Token != 0 && CallsignList.SelectedItem != null)
            {
                ListView serverList = null;

                if (ServerTabs.SelectedTab == PreferedTab)
                    serverList = PreferedList;
                else if (ServerTabs.SelectedTab == CommunityTab)
                    serverList = CommunityList;

                if (serverList != null)
                    Play.Enabled = serverList.SelectedItems.Count > 0;
            }
        }

        private void Username_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIStates();
        }

        private void Username_TextUpdate(object sender, EventArgs e)
        {
            UpdateUIStates();
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            UpdateUIStates();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegistrationForm form = new RegistrationForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Username.Text = form.AccountName;
            }
            UpdateUIStates();
        }

        protected class CharaterListItem
        {
            public string Name = string.Empty;
            public UInt64 CID = 0;

            public CharaterListItem ( string n, UInt64 id )
            {
                Name = n;
                CID = id;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (UID != 0)
            {
                CallsignList.Items.Clear();
                UID = 0;
                Token = 0;
                LoginButton.Text = "Login";
                gameList.Kill();
                gameList = null;
                UpdateUIStates();
                return;
            }
            Login();
            
            if (Token != 0)
            {
                if (!prefs.Accounts.Contains(Username.Text))
                {
                    prefs.Accounts.Insert(0,Username.Text);
                    Username.Items.Insert(0,Username.Text);
                    prefs.Write();
                }
            }
        }

        protected void Login ()
        {
            Login login = new Login();
            UID = 0;
            Token = 0;
            if (login.Connect(Username.Text, Password.Text))
            {
                UID = login.UID;
                Token = login.Token;

                LoginButton.Text = "Logout";

                // get characters
                Dictionary<UInt64, string> list = login.GetCharacterList();
                if (list != null)
                {
                    CallsignList.Items.Clear();

                    foreach (KeyValuePair<UInt64, string> character in list)
                        CallsignList.Items.Add(new CharaterListItem(character.Value, character.Key));
                }
                WaitBox box = new WaitBox("Servers");
                box.Show();
                box.Update("Fetching servers");

                if (gameList != null)
                    gameList.Kill();
                gameList = new GameList(UID);
                timer_Tick(this, EventArgs.Empty);
                box.Close();
            }
            else
                MessageBox.Show("Login failure");

            UpdateUIStates();
        }

        private void StartupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameList != null)
                gameList.Kill();
            if (timer != null)
                timer.Stop();

            gameList = null;
            timer = null;
        }

        private void NewCharacter_Click(object sender, EventArgs e)
        {
            if (new CharacterInfoForm(string.Empty,Username.Text, Password.Text).ShowDialog() == DialogResult.OK)
                Login();
        }

        private void Play_Click(object sender, EventArgs e)
        {
            prefs.Write();
            
            ListView serverList = null;

            if (ServerTabs.SelectedTab == PreferedTab)
                serverList = PreferedList;
            else if (ServerTabs.SelectedTab == CommunityTab)
                serverList = CommunityList;

            if (serverList == null || serverList.SelectedItems.Count == 0)
                return;

            GameList.ListedServer host = serverList.SelectedItems[0].Tag as GameList.ListedServer;
            if (host == null || CallsignList.SelectedItem == null)
                return;

            CharacterID = ((CharaterListItem)CallsignList.SelectedItem).CID;
            ConnectHost = host.Host;

            OkToPlay = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CommunityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIStates();
        }

        private void PreferedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIStates();
        }
    }
}

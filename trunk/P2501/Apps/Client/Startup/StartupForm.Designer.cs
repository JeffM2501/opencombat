namespace P2501Client
{
    partial class StartupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader PreferedServer;
            System.Windows.Forms.ColumnHeader PreferedDescription;
            System.Windows.Forms.ColumnHeader PreferedPlayers;
            System.Windows.Forms.ColumnHeader PreferedHost;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader3;
            System.Windows.Forms.ColumnHeader columnHeader4;
            System.Windows.Forms.ColumnHeader columnHeader5;
            System.Windows.Forms.ColumnHeader columnHeader6;
            System.Windows.Forms.ColumnHeader columnHeader7;
            System.Windows.Forms.ColumnHeader columnHeader8;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupForm));
            this.LoginGroup = new System.Windows.Forms.GroupBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.LoginButton = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Username = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NewsBrowser = new System.Windows.Forms.WebBrowser();
            this.CallsignsGroup = new System.Windows.Forms.GroupBox();
            this.NewCharacter = new System.Windows.Forms.Button();
            this.CallsignList = new System.Windows.Forms.ListBox();
            this.GamesGroup = new System.Windows.Forms.GroupBox();
            this.ServerTabs = new System.Windows.Forms.TabControl();
            this.PreferedTab = new System.Windows.Forms.TabPage();
            this.PreferedList = new System.Windows.Forms.ListView();
            this.ServerListIcons = new System.Windows.Forms.ImageList(this.components);
            this.CommunityTab = new System.Windows.Forms.TabPage();
            this.CommunityList = new System.Windows.Forms.ListView();
            this.LeagueTab = new System.Windows.Forms.TabPage();
            this.Options = new System.Windows.Forms.Button();
            this.Play = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            PreferedServer = new System.Windows.Forms.ColumnHeader();
            PreferedDescription = new System.Windows.Forms.ColumnHeader();
            PreferedPlayers = new System.Windows.Forms.ColumnHeader();
            PreferedHost = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            columnHeader6 = new System.Windows.Forms.ColumnHeader();
            columnHeader7 = new System.Windows.Forms.ColumnHeader();
            columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.LoginGroup.SuspendLayout();
            this.CallsignsGroup.SuspendLayout();
            this.GamesGroup.SuspendLayout();
            this.ServerTabs.SuspendLayout();
            this.PreferedTab.SuspendLayout();
            this.CommunityTab.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PreferedServer
            // 
            PreferedServer.Text = "Name";
            PreferedServer.Width = 83;
            // 
            // PreferedDescription
            // 
            PreferedDescription.Text = "Description";
            PreferedDescription.Width = 140;
            // 
            // PreferedPlayers
            // 
            PreferedPlayers.Text = "Players";
            PreferedPlayers.Width = 49;
            // 
            // PreferedHost
            // 
            PreferedHost.Text = "Host";
            PreferedHost.Width = 48;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 83;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Description";
            columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Players";
            columnHeader3.Width = 49;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Host";
            columnHeader4.Width = 48;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Name";
            columnHeader5.Width = 83;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Description";
            columnHeader6.Width = 140;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Players";
            columnHeader7.Width = 49;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "Host";
            columnHeader8.Width = 48;
            // 
            // LoginGroup
            // 
            this.LoginGroup.Controls.Add(this.RegisterButton);
            this.LoginGroup.Controls.Add(this.LoginButton);
            this.LoginGroup.Controls.Add(this.Password);
            this.LoginGroup.Controls.Add(this.label2);
            this.LoginGroup.Controls.Add(this.Username);
            this.LoginGroup.Controls.Add(this.label1);
            this.LoginGroup.Location = new System.Drawing.Point(12, 12);
            this.LoginGroup.Name = "LoginGroup";
            this.LoginGroup.Size = new System.Drawing.Size(200, 140);
            this.LoginGroup.TabIndex = 0;
            this.LoginGroup.TabStop = false;
            this.LoginGroup.Text = "Login";
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(91, 106);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(75, 23);
            this.RegisterButton.TabIndex = 5;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(10, 106);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 4;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(10, 80);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(184, 20);
            this.Password.TabIndex = 3;
            this.Password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // Username
            // 
            this.Username.FormattingEnabled = true;
            this.Username.Location = new System.Drawing.Point(10, 36);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(184, 21);
            this.Username.TabIndex = 1;
            this.Username.SelectedIndexChanged += new System.EventHandler(this.Username_SelectedIndexChanged);
            this.Username.TextUpdate += new System.EventHandler(this.Username_TextUpdate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // NewsBrowser
            // 
            this.NewsBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NewsBrowser.Location = new System.Drawing.Point(3, 3);
            this.NewsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.NewsBrowser.Name = "NewsBrowser";
            this.NewsBrowser.Size = new System.Drawing.Size(402, 189);
            this.NewsBrowser.TabIndex = 1;
            // 
            // CallsignsGroup
            // 
            this.CallsignsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.CallsignsGroup.Controls.Add(this.NewCharacter);
            this.CallsignsGroup.Controls.Add(this.CallsignList);
            this.CallsignsGroup.Location = new System.Drawing.Point(12, 167);
            this.CallsignsGroup.Name = "CallsignsGroup";
            this.CallsignsGroup.Size = new System.Drawing.Size(200, 349);
            this.CallsignsGroup.TabIndex = 2;
            this.CallsignsGroup.TabStop = false;
            this.CallsignsGroup.Text = "Callsigns";
            // 
            // NewCharacter
            // 
            this.NewCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NewCharacter.Location = new System.Drawing.Point(10, 311);
            this.NewCharacter.Name = "NewCharacter";
            this.NewCharacter.Size = new System.Drawing.Size(75, 23);
            this.NewCharacter.TabIndex = 1;
            this.NewCharacter.Text = "New...";
            this.NewCharacter.UseVisualStyleBackColor = true;
            this.NewCharacter.Click += new System.EventHandler(this.NewCharacter_Click);
            // 
            // CallsignList
            // 
            this.CallsignList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.CallsignList.FormattingEnabled = true;
            this.CallsignList.Location = new System.Drawing.Point(10, 18);
            this.CallsignList.Name = "CallsignList";
            this.CallsignList.Size = new System.Drawing.Size(175, 277);
            this.CallsignList.TabIndex = 0;
            // 
            // GamesGroup
            // 
            this.GamesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GamesGroup.Controls.Add(this.ServerTabs);
            this.GamesGroup.Controls.Add(this.Options);
            this.GamesGroup.Controls.Add(this.Play);
            this.GamesGroup.Location = new System.Drawing.Point(3, 3);
            this.GamesGroup.Name = "GamesGroup";
            this.GamesGroup.Size = new System.Drawing.Size(402, 290);
            this.GamesGroup.TabIndex = 3;
            this.GamesGroup.TabStop = false;
            this.GamesGroup.Text = "Servers";
            // 
            // ServerTabs
            // 
            this.ServerTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerTabs.Controls.Add(this.PreferedTab);
            this.ServerTabs.Controls.Add(this.CommunityTab);
            this.ServerTabs.Controls.Add(this.LeagueTab);
            this.ServerTabs.ImageList = this.ServerListIcons;
            this.ServerTabs.Location = new System.Drawing.Point(6, 19);
            this.ServerTabs.Name = "ServerTabs";
            this.ServerTabs.SelectedIndex = 0;
            this.ServerTabs.Size = new System.Drawing.Size(390, 228);
            this.ServerTabs.TabIndex = 2;
            // 
            // PreferedTab
            // 
            this.PreferedTab.Controls.Add(this.PreferedList);
            this.PreferedTab.ImageIndex = 0;
            this.PreferedTab.Location = new System.Drawing.Point(4, 31);
            this.PreferedTab.Name = "PreferedTab";
            this.PreferedTab.Padding = new System.Windows.Forms.Padding(3);
            this.PreferedTab.Size = new System.Drawing.Size(382, 193);
            this.PreferedTab.TabIndex = 0;
            this.PreferedTab.Text = "Prefered";
            this.PreferedTab.UseVisualStyleBackColor = true;
            // 
            // PreferedList
            // 
            this.PreferedList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PreferedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            PreferedServer,
            PreferedDescription,
            PreferedPlayers,
            PreferedHost});
            this.PreferedList.FullRowSelect = true;
            this.PreferedList.GridLines = true;
            this.PreferedList.HideSelection = false;
            this.PreferedList.LargeImageList = this.ServerListIcons;
            this.PreferedList.Location = new System.Drawing.Point(0, 0);
            this.PreferedList.Name = "PreferedList";
            this.PreferedList.Size = new System.Drawing.Size(379, 188);
            this.PreferedList.SmallImageList = this.ServerListIcons;
            this.PreferedList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.PreferedList.TabIndex = 0;
            this.PreferedList.UseCompatibleStateImageBehavior = false;
            this.PreferedList.View = System.Windows.Forms.View.Details;
            this.PreferedList.SelectedIndexChanged += new System.EventHandler(this.PreferedList_SelectedIndexChanged);
            // 
            // ServerListIcons
            // 
            this.ServerListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ServerListIcons.ImageStream")));
            this.ServerListIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ServerListIcons.Images.SetKeyName(0, "agt_action_success.png");
            this.ServerListIcons.Images.SetKeyName(1, "add_group.png");
            this.ServerListIcons.Images.SetKeyName(2, "agt_forum.png");
            this.ServerListIcons.Images.SetKeyName(3, "bookmark_add.png");
            this.ServerListIcons.Images.SetKeyName(4, "xclock.png");
            // 
            // CommunityTab
            // 
            this.CommunityTab.Controls.Add(this.CommunityList);
            this.CommunityTab.ImageIndex = 2;
            this.CommunityTab.Location = new System.Drawing.Point(4, 31);
            this.CommunityTab.Name = "CommunityTab";
            this.CommunityTab.Padding = new System.Windows.Forms.Padding(3);
            this.CommunityTab.Size = new System.Drawing.Size(382, 193);
            this.CommunityTab.TabIndex = 1;
            this.CommunityTab.Text = "Community";
            this.CommunityTab.UseVisualStyleBackColor = true;
            // 
            // CommunityList
            // 
            this.CommunityList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CommunityList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader5,
            columnHeader6,
            columnHeader7,
            columnHeader8});
            this.CommunityList.FullRowSelect = true;
            this.CommunityList.GridLines = true;
            this.CommunityList.HideSelection = false;
            this.CommunityList.LargeImageList = this.ServerListIcons;
            this.CommunityList.Location = new System.Drawing.Point(0, 0);
            this.CommunityList.Name = "CommunityList";
            this.CommunityList.Size = new System.Drawing.Size(379, 188);
            this.CommunityList.SmallImageList = this.ServerListIcons;
            this.CommunityList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.CommunityList.TabIndex = 1;
            this.CommunityList.UseCompatibleStateImageBehavior = false;
            this.CommunityList.View = System.Windows.Forms.View.Details;
            this.CommunityList.SelectedIndexChanged += new System.EventHandler(this.CommunityList_SelectedIndexChanged);
            // 
            // LeagueTab
            // 
            this.LeagueTab.ImageIndex = 4;
            this.LeagueTab.Location = new System.Drawing.Point(4, 31);
            this.LeagueTab.Name = "LeagueTab";
            this.LeagueTab.Size = new System.Drawing.Size(382, 193);
            this.LeagueTab.TabIndex = 2;
            this.LeagueTab.Text = "League";
            this.LeagueTab.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.Options.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Options.Location = new System.Drawing.Point(6, 253);
            this.Options.Name = "Options";
            this.Options.Size = new System.Drawing.Size(75, 23);
            this.Options.TabIndex = 1;
            this.Options.Text = "Options...";
            this.Options.UseVisualStyleBackColor = true;
            // 
            // Play
            // 
            this.Play.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Play.Location = new System.Drawing.Point(321, 253);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(75, 23);
            this.Play.TabIndex = 0;
            this.Play.Text = "Play";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(218, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.NewsBrowser);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.GamesGroup);
            this.splitContainer1.Size = new System.Drawing.Size(408, 504);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 4;
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 528);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.CallsignsGroup);
            this.Controls.Add(this.LoginGroup);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 350);
            this.Name = "StartupForm";
            this.Text = "Projekt 2501";
            this.Load += new System.EventHandler(this.StartupForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartupForm_FormClosing);
            this.LoginGroup.ResumeLayout(false);
            this.LoginGroup.PerformLayout();
            this.CallsignsGroup.ResumeLayout(false);
            this.GamesGroup.ResumeLayout(false);
            this.ServerTabs.ResumeLayout(false);
            this.PreferedTab.ResumeLayout(false);
            this.CommunityTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox LoginGroup;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Username;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.WebBrowser NewsBrowser;
        private System.Windows.Forms.GroupBox CallsignsGroup;
        private System.Windows.Forms.Button NewCharacter;
        private System.Windows.Forms.ListBox CallsignList;
        private System.Windows.Forms.GroupBox GamesGroup;
        private System.Windows.Forms.Button Options;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl ServerTabs;
        private System.Windows.Forms.TabPage PreferedTab;
        private System.Windows.Forms.ListView PreferedList;
        private System.Windows.Forms.TabPage CommunityTab;
        private System.Windows.Forms.TabPage LeagueTab;
        private System.Windows.Forms.ImageList ServerListIcons;
        private System.Windows.Forms.ListView CommunityList;
    }
}


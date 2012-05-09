namespace Client
{
    partial class DefaultLauncher
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
            this.MainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CallsignList = new System.Windows.Forms.ComboBox();
            this.ServerPulldown = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Play_BN = new System.Windows.Forms.Button();
            this.AuthGroup = new System.Windows.Forms.GroupBox();
            this.SaveAuth = new System.Windows.Forms.CheckBox();
            this.RegisterLabel = new System.Windows.Forms.LinkLabel();
            this.LoginButton = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
			if (!NoBrowser)
				this.NewsBrowser = new System.Windows.Forms.WebBrowser();
            this.ProgressPanel = new System.Windows.Forms.Panel();
            this.PatchStatusLabel = new System.Windows.Forms.Label();
            this.AutoPlay = new System.Windows.Forms.CheckBox();
            this.MainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.AuthGroup.SuspendLayout();
            this.ProgressPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Controls.Add(this.groupBox2);
            this.MainPanel.Controls.Add(this.AuthGroup);
            this.MainPanel.Controls.Add(this.NewsBrowser);
            this.MainPanel.Location = new System.Drawing.Point(2, 1);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(785, 377);
            this.MainPanel.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.AutoPlay);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.CallsignList);
            this.groupBox2.Controls.Add(this.ServerPulldown);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.Play_BN);
            this.groupBox2.Location = new System.Drawing.Point(391, 315);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(391, 58);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(177, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Callsign";
            // 
            // CallsignList
            // 
            this.CallsignList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CallsignList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CallsignList.FormattingEnabled = true;
            this.CallsignList.Location = new System.Drawing.Point(180, 31);
            this.CallsignList.Name = "CallsignList";
            this.CallsignList.Size = new System.Drawing.Size(123, 21);
            this.CallsignList.TabIndex = 3;
            this.CallsignList.SelectedIndexChanged += new System.EventHandler(this.CallsignList_SelectedIndexChanged);
            // 
            // ServerPulldown
            // 
            this.ServerPulldown.FormattingEnabled = true;
            this.ServerPulldown.Location = new System.Drawing.Point(6, 32);
            this.ServerPulldown.Name = "ServerPulldown";
            this.ServerPulldown.Size = new System.Drawing.Size(168, 21);
            this.ServerPulldown.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Server";
            // 
            // Play_BN
            // 
            this.Play_BN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Play_BN.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Play_BN.Location = new System.Drawing.Point(310, 29);
            this.Play_BN.Name = "Play_BN";
            this.Play_BN.Size = new System.Drawing.Size(75, 23);
            this.Play_BN.TabIndex = 0;
            this.Play_BN.Text = "Play";
            this.Play_BN.UseVisualStyleBackColor = true;
            this.Play_BN.Click += new System.EventHandler(this.Play_BN_Click);
            // 
            // AuthGroup
            // 
            this.AuthGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AuthGroup.Controls.Add(this.SaveAuth);
            this.AuthGroup.Controls.Add(this.RegisterLabel);
            this.AuthGroup.Controls.Add(this.LoginButton);
            this.AuthGroup.Controls.Add(this.Password);
            this.AuthGroup.Controls.Add(this.label2);
            this.AuthGroup.Controls.Add(this.Email);
            this.AuthGroup.Controls.Add(this.label1);
            this.AuthGroup.Location = new System.Drawing.Point(3, 315);
            this.AuthGroup.Name = "AuthGroup";
            this.AuthGroup.Size = new System.Drawing.Size(382, 59);
            this.AuthGroup.TabIndex = 3;
            this.AuthGroup.TabStop = false;
            this.AuthGroup.Text = "Authentication";
            // 
            // SaveAuth
            // 
            this.SaveAuth.AutoSize = true;
            this.SaveAuth.Location = new System.Drawing.Point(242, 34);
            this.SaveAuth.Name = "SaveAuth";
            this.SaveAuth.Size = new System.Drawing.Size(51, 17);
            this.SaveAuth.TabIndex = 6;
            this.SaveAuth.Text = "Save";
            this.SaveAuth.UseVisualStyleBackColor = true;
            this.SaveAuth.CheckedChanged += new System.EventHandler(this.SaveAuth_CheckedChanged);
            // 
            // RegisterLabel
            // 
            this.RegisterLabel.AutoSize = true;
            this.RegisterLabel.Location = new System.Drawing.Point(330, 13);
            this.RegisterLabel.Name = "RegisterLabel";
            this.RegisterLabel.Size = new System.Drawing.Size(46, 13);
            this.RegisterLabel.TabIndex = 5;
            this.RegisterLabel.TabStop = true;
            this.RegisterLabel.Text = "Register";
            this.RegisterLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RegisterLabel_LinkClicked);
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(301, 30);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 4;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(135, 32);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(98, 20);
            this.Password.TabIndex = 3;
            this.Password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(6, 32);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(122, 20);
            this.Email.TabIndex = 1;
            this.Email.TextChanged += new System.EventHandler(this.Email_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Email";
            // 
            // NewsBrowser
            // 
			if (!NoBrowser)
			{
				this.NewsBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
				this.NewsBrowser.Location = new System.Drawing.Point(3, 3);
				this.NewsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
				this.NewsBrowser.Name = "NewsBrowser";
				this.NewsBrowser.Size = new System.Drawing.Size(779, 306);
				this.NewsBrowser.TabIndex = 0;
			}
            // 
            // ProgressPanel
            // 
            this.ProgressPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressPanel.Controls.Add(this.PatchStatusLabel);
            this.ProgressPanel.Location = new System.Drawing.Point(2, 384);
            this.ProgressPanel.Name = "ProgressPanel";
            this.ProgressPanel.Size = new System.Drawing.Size(785, 45);
            this.ProgressPanel.TabIndex = 1;
            // 
            // PatchStatusLabel
            // 
            this.PatchStatusLabel.AutoSize = true;
            this.PatchStatusLabel.Location = new System.Drawing.Point(6, 25);
            this.PatchStatusLabel.Name = "PatchStatusLabel";
            this.PatchStatusLabel.Size = new System.Drawing.Size(35, 13);
            this.PatchStatusLabel.TabIndex = 0;
            this.PatchStatusLabel.Text = "label5";
            // 
            // AutoPlay
            // 
            this.AutoPlay.AutoSize = true;
            this.AutoPlay.Location = new System.Drawing.Point(310, 9);
            this.AutoPlay.Name = "AutoPlay";
            this.AutoPlay.Size = new System.Drawing.Size(71, 17);
            this.AutoPlay.TabIndex = 5;
            this.AutoPlay.Text = "Auto Play";
            this.AutoPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AutoPlay.UseVisualStyleBackColor = true;
            this.AutoPlay.CheckedChanged += new System.EventHandler(this.AutoPlay_CheckedChanged);
            // 
            // DefaultLauncher
            // 
            this.AcceptButton = this.Play_BN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 431);
            this.Controls.Add(this.ProgressPanel);
            this.Controls.Add(this.MainPanel);
            this.Name = "DefaultLauncher";
            this.Text = "DefaultLauncher";
            this.Load += new System.EventHandler(this.DefaultLauncher_Load);
            this.MainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.AuthGroup.ResumeLayout(false);
            this.AuthGroup.PerformLayout();
            this.ProgressPanel.ResumeLayout(false);
            this.ProgressPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.WebBrowser NewsBrowser;
        private System.Windows.Forms.Panel ProgressPanel;
        private System.Windows.Forms.GroupBox AuthGroup;
        private System.Windows.Forms.CheckBox SaveAuth;
        private System.Windows.Forms.LinkLabel RegisterLabel;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CallsignList;
        private System.Windows.Forms.ComboBox ServerPulldown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Play_BN;
        private System.Windows.Forms.Label PatchStatusLabel;
        private System.Windows.Forms.CheckBox AutoPlay;

    }
}
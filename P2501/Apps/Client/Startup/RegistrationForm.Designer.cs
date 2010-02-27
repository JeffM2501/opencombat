namespace P2501Client
{
    partial class RegistrationForm
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
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.TextBox();
            this.lable2 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.PassVerify = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PassError = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Callsign = new System.Windows.Forms.TextBox();
            this.Check = new System.Windows.Forms.Button();
            this.Terms = new System.Windows.Forms.LinkLabel();
            this.Agree = new System.Windows.Forms.CheckBox();
            this.TermsBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(288, 164);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "Create";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(207, 164);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Email Address";
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(94, 6);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(269, 20);
            this.Email.TabIndex = 3;
            this.Email.TextChanged += new System.EventHandler(this.Email_TextChanged);
            // 
            // lable2
            // 
            this.lable2.AutoSize = true;
            this.lable2.Location = new System.Drawing.Point(12, 35);
            this.lable2.Name = "lable2";
            this.lable2.Size = new System.Drawing.Size(53, 13);
            this.lable2.TabIndex = 5;
            this.lable2.Text = "Password";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(94, 32);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(147, 20);
            this.Password.TabIndex = 6;
            this.Password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // PassVerify
            // 
            this.PassVerify.Location = new System.Drawing.Point(94, 58);
            this.PassVerify.Name = "PassVerify";
            this.PassVerify.PasswordChar = '*';
            this.PassVerify.Size = new System.Drawing.Size(147, 20);
            this.PassVerify.TabIndex = 8;
            this.PassVerify.TextChanged += new System.EventHandler(this.PassVerify_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Password Verify";
            // 
            // PassError
            // 
            this.PassError.AutoSize = true;
            this.PassError.ForeColor = System.Drawing.Color.Maroon;
            this.PassError.Location = new System.Drawing.Point(247, 64);
            this.PassError.Name = "PassError";
            this.PassError.Size = new System.Drawing.Size(123, 13);
            this.PassError.TabIndex = 9;
            this.PassError.Text = "Passwords do not match";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Desired Callsign";
            // 
            // Callsign
            // 
            this.Callsign.Location = new System.Drawing.Point(94, 83);
            this.Callsign.Name = "Callsign";
            this.Callsign.Size = new System.Drawing.Size(147, 20);
            this.Callsign.TabIndex = 11;
            this.Callsign.TextChanged += new System.EventHandler(this.Callsign_TextChanged);
            // 
            // Check
            // 
            this.Check.Location = new System.Drawing.Point(250, 80);
            this.Check.Name = "Check";
            this.Check.Size = new System.Drawing.Size(104, 23);
            this.Check.TabIndex = 12;
            this.Check.Text = "Check Availabity";
            this.Check.UseVisualStyleBackColor = true;
            this.Check.Click += new System.EventHandler(this.Check_Click);
            // 
            // Terms
            // 
            this.Terms.AutoSize = true;
            this.Terms.Location = new System.Drawing.Point(110, 115);
            this.Terms.Name = "Terms";
            this.Terms.Size = new System.Drawing.Size(109, 13);
            this.Terms.TabIndex = 13;
            this.Terms.TabStop = true;
            this.Terms.Text = "Terms and Conditions";
            this.Terms.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Terms_LinkClicked);
            // 
            // Agree
            // 
            this.Agree.AutoSize = true;
            this.Agree.Location = new System.Drawing.Point(15, 114);
            this.Agree.Name = "Agree";
            this.Agree.Size = new System.Drawing.Size(89, 17);
            this.Agree.TabIndex = 14;
            this.Agree.Text = "I agree to the";
            this.Agree.UseVisualStyleBackColor = true;
            this.Agree.CheckedChanged += new System.EventHandler(this.Agree_CheckedChanged);
            // 
            // TermsBrowser
            // 
            this.TermsBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TermsBrowser.Location = new System.Drawing.Point(15, 137);
            this.TermsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.TermsBrowser.Name = "TermsBrowser";
            this.TermsBrowser.Size = new System.Drawing.Size(339, 21);
            this.TermsBrowser.TabIndex = 15;
            // 
            // RegistrationForm
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(375, 199);
            this.Controls.Add(this.TermsBrowser);
            this.Controls.Add(this.Agree);
            this.Controls.Add(this.Terms);
            this.Controls.Add(this.Check);
            this.Controls.Add(this.Callsign);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PassError);
            this.Controls.Add(this.PassVerify);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.lable2);
            this.Controls.Add(this.Email);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationForm";
            this.Text = "New User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.Label lable2;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.TextBox PassVerify;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label PassError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Callsign;
        private System.Windows.Forms.Button Check;
        private System.Windows.Forms.LinkLabel Terms;
        private System.Windows.Forms.CheckBox Agree;
        private System.Windows.Forms.WebBrowser TermsBrowser;
    }
}
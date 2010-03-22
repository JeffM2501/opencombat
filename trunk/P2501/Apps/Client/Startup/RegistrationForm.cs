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
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using Clients;
using Lidgren.Network;

using Auth;

namespace P2501Client
{
    public partial class RegistrationForm : Form
    {
        public string AccountName = string.Empty;

        public RegistrationForm()
        {
            InitializeComponent();
            TermsBrowser.Visible = false;
            Check.Enabled = false;
            OK.Enabled = false;
        }

        private void Terms_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TermsBrowser.Visible)
                return;

            TermsBrowser.Visible = true;
            TermsBrowser.Navigate("http://www.awesomelaser.com/p2501/terms.html");
            this.Height += 250;
        }

        private void CheckOK ()
        {
            OK.Enabled = false;

            if (Email.Text == string.Empty || !Email.Text.Contains("@"))
                return;

            if (Password.Text == string.Empty || PassVerify.Text == string.Empty || Password.Text != PassVerify.Text)
                return;

            if (Callsign.Text == string.Empty)
                return;

            if (!Agree.Checked)
                return;

            OK.Enabled = true;
        }

        private void CheckPasswords()
        {
            PassError.Visible = Password.Text != PassVerify.Text;
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            CheckOK();
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            CheckOK();
            CheckPasswords();
        }

        private void PassVerify_TextChanged(object sender, EventArgs e)
        {
            CheckOK();
            CheckPasswords();
        }

        private void Callsign_TextChanged(object sender, EventArgs e)
        {
            CheckOK();
            Check.Enabled = Callsign.Text != string.Empty;
        }

        private void Agree_CheckedChanged(object sender, EventArgs e)
        {
            CheckOK();
        }

        private void Check_Click(object sender, EventArgs e)
        {
          if (CheckName())
              MessageBox.Show("The name " + Callsign.Text + " is currently available");
        }

        private bool CheckName ()
        {
            WaitBox box = new WaitBox("Checking Availability");
            box.Show(this);
            box.Update("Contacting server");

            bool avail = Login.CheckName(Callsign.Text);

            box.Close();

            if (!avail)
            {
                MessageBox.Show("The name " + Callsign.Text + " is not available");
                Callsign.Text = string.Empty;
                Callsign.Select();
            }

            return avail;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (!CheckName())
            {
                DialogResult = DialogResult.None;
                return;
            }

            // do the registration
            WaitBox box = new WaitBox("Registering Account");
            box.Show();
            box.Update("Connecting to secure host");

            switch(Login.Register(Email.Text,Password.Text,Callsign.Text))
            {
                case Login.RegisterCode.Error:
                    MessageBox.Show("The registration server could not be contacted");
                    DialogResult = DialogResult.None;
                    break;

                case Login.RegisterCode.BadCallsign:
                    MessageBox.Show("The name " + Callsign.Text + " was not available");
                    Callsign.Text = string.Empty;
                    Callsign.Select();
                    DialogResult = DialogResult.None;
                    break; 

                case Login.RegisterCode.BadEmail:
                    MessageBox.Show("The email " + Email.Text + " is already registered");
                    Email.Text = string.Empty;
                    Email.Select();
                    DialogResult = DialogResult.None;
                    break;

            }
            box.Close();
            AccountName = Email.Text;
        }
    }
}

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
            box.Update(10, "Contacting secure host");

           // CryptoClient client = new CryptoClient("www.awesomelaser.com", 4111);
            CryptoClient client = new CryptoClient("localhost", 4111);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            int timeout = 120;

            bool done = false;
            bool connected = false;
            bool worked = false;

            while (!done)
            {
                if (!connected && client.IsConnected)
                    box.Update(20, "Connection established");
                if (connected && !client.IsConnected)
                {
                    client.Kill();
                    box.Close();
                    MessageBox.Show("The registration server could not be contacted");
                    DialogResult = DialogResult.None;
                    return;
                }

                NetBuffer buffer = client.GetPentMessage();
                while (buffer != null)
                {
                    int name = buffer.ReadInt32();

                    if (name == AuthMessage.Hail)
                    {
                        RequestAdd msg = new RequestAdd();
                        msg.email = Email.Text;
                        msg.password = Password.Text;
                        msg.callsign = Callsign.Text;

                        box.Update(75, "Registering account");
                        client.SendMessage(msg.Pack(), msg.Channel());
                    }
                    else if (name == AuthMessage.AddOK)
                    {
                        box.Update(100, "Registration complete");
                        client.Kill();
                        done = true;
                        worked = true;
                    }
                    else if (name == AuthMessage.AddBadCallsign)
                    {
                        client.Kill();
                        box.Close();
                        MessageBox.Show("The name " + Callsign.Text + " was not available");
                        Callsign.Text = string.Empty;
                        Callsign.Select();
                        DialogResult = DialogResult.None;
                        return;
                    }
                    else if (name == AuthMessage.AddBadEmail)
                    {
                        client.Kill();
                        box.Close();
                        MessageBox.Show("The email " + Email.Text + " is already registered");
                        Email.Text = string.Empty;
                        Email.Select();
                        DialogResult = DialogResult.None;
                        return;
                    }
                    else
                    {
                        done = true;
                        connected = false;
                    }

                    if (!done)
                        buffer = client.GetPentMessage();
                    else
                        buffer = null;
                }

                if (timer.ElapsedMilliseconds / 1000 > timeout)
                {
                    done = true;
                    connected = false;
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            client.Kill();
            box.Close();

            if (!worked)
            {
                box.Close();
                MessageBox.Show("The registration server could not be contacted");
                DialogResult = DialogResult.None;
                return;
            }

            AccountName = Email.Text;
        }
    }
}

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
    public partial class CharacterInfoForm : Form
    {
        string Username = string.Empty;
        string Password = string.Empty;

        public CharacterInfoForm( string callsign, string username, string password )
        {
            InitializeComponent();
            textBox1.Text = callsign;
            OK.Enabled = Check.Enabled = textBox1.Text != string.Empty;

            Username = username;
            Password = password;
        }

        private void Check_Click(object sender, EventArgs e)
        {
            if (!Login.CheckName(textBox1.Text))
            {
                MessageBox.Show("The name " + textBox1.Text + " is not available");
                textBox1.Text = string.Empty;
                textBox1.Select();
            }
            else
                MessageBox.Show("The name " + textBox1.Text + " is currently available");
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            WaitBox box = new WaitBox("Adding Character");
            box.Update("Contacting server");

            if (!login.Connect(Username, Password))
            {
                box.Close();
                MessageBox.Show("Character server is unavailable");
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (!login.AddCharacter(textBox1.Text))
            {
                box.Close();
                MessageBox.Show("Callsign unavailable");
                DialogResult = DialogResult.Cancel;
                return;
            }

            box.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OK.Enabled = Check.Enabled = textBox1.Text != string.Empty;
        }
    }
}

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
    public partial class WaitBox : Form
    {
        public WaitBox( string title)
        {
            InitializeComponent();
            Text = title;
        }

        public void Update ( int val, string status )
        {
            progressBar1.Visible = true;
            progressBar1.Value = val;
            StatusLine.Text = status;
            Update();
        }

        public void Update(string status)
        {
            progressBar1.Visible = false;
            StatusLine.Text = status;
            Update();
        }

        private void WaitBox_Load(object sender, EventArgs e)
        {

        }
    }
}

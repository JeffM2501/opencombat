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

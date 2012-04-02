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

namespace GUIElementConstructor
{
    public partial class OptionEditor : Form
    {
        public String OptionName = string.Empty;
        public String OptionValue = string.Empty;

        public OptionEditor()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            OptionName = NameBox.Text;
            OptionValue = ValueBox.Text;
        }

        private void OptionEditor_Load(object sender, EventArgs e)
        {
            NameBox.Text = OptionName;
            ValueBox.Text = OptionValue;
        }
    }
}

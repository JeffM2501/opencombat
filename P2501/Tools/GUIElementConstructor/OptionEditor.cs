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

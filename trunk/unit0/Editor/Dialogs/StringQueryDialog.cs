using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor.Dialogs
{
    public partial class StringQueryDialog : Form
    {

        public string Title = string.Empty;
        public string Query = string.Empty;
        public string Return = string.Empty;

        public bool MultiLine = false;

        public StringQueryDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Return = TextEntry.Text;
        }

        private void StringQueryDialog_Load(object sender, EventArgs e)
        {
            this.Text = Title;
            QueryLabel.Text = Query;
            TextEntry.Text = Return;
            if (MultiLine)
            {
                TextEntry.Multiline = true;
                this.Size = new System.Drawing.Size(this.Size.Width,this.Size.Height + 50);
                Invalidate();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class FailoverBrowser : UserControl
    {
        public static bool UseBrowser = false;

        public FailoverBrowser()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public void Navigate(string URL)
        {
            if (UseBrowser)
                Browser.Navigate(URL);
        }
    }
}

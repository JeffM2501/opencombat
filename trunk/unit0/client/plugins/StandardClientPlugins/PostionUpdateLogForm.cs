using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Client.API;

namespace StandardClientPlugins
{
    public partial class PostionUpdateLogForm : Form
    {
        public ProjectionErrorData Data = null;

        public PostionUpdateLogForm()
        {
            InitializeComponent();
        }

        string BuildListToString(List<string> input)
        {
            string output = "";

            foreach (string i in input)
                output += i + "\r\n";

            return output;
        }

        private void PostionUpdateLogForm_Load(object sender, EventArgs e)
        {
            BeforeEndTE.Text = BuildListToString(Data.FinalPositionLogBeforeTruncation);
            BeforeTrimTE.Text = BuildListToString(Data.TruncationPositionLogBeforeRemoval);

            AfterEndTE.Text = BuildListToString(Data.FinalPositionLogAfterTruncation);
            AfterTrimTE.Text = BuildListToString(Data.TruncationPositionLogAfterRemoval);

            Now.Text = Data.Now.ToString();
            Trim.Text = Data.TruncationTime.ToString();
        }
    }
}

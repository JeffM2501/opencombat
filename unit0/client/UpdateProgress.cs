using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class UpdateProgress : Form
    {
        Patcher patcher = null;
        public UpdateProgress()
        {
            InitializeComponent();
        }

        private void UpdateProgress_Load(object sender, EventArgs e)
        {
            patcher = new Patcher();
            UpdateStatusLabel.Text = "Checking Hashtables";
            this.Update();

            patcher.BuildHashTables();

            UpdateStatusLabel.Text = "Contacting update servers";
            this.Update();

            patcher.StartUpdate();

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (patcher.UpdateDone())
            {
                if (patcher.UpdateError() || patcher.NeedToExit())
                {
                    if (patcher.UpdateError())
                    {
                        MessageBox.Show(this, "There was an error during updating");
                    }
                    Cancel_Click(this, EventArgs.Empty);
                    return;
                }

                timer1.Stop();
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            else
            {
                 // get some status stuff here
                int total = 0;
                int current = 0;
                patcher.GetProgress(out total,out current);
                switch (patcher.GetStatus())
                {
                    case Patcher.Status.BuldingHashes:
                        UpdateStatusLabel.Text = "Checking Local Files";
                        break;

                    case Patcher.Status.GettingDataList:
                        UpdateStatusLabel.Text = "Getting Remote File List";
                        break;

                    case Patcher.Status.GettingDataFiles:
                        UpdateStatusLabel.Text = "Checking Local Files";
                        UpdateStatusBar.Value = current/total;
                        break;
                }
            }

            this.Update();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (patcher != null)
                patcher.KillUpdate();

            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

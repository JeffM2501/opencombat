using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using GridWorld;

namespace PlanEdit
{
    public partial class EditView : Form
    {
		ClusterEditor Editor = null;
		PlanRenderer PlanView = null;

        public EditView()
        {
            InitializeComponent();
        }

		public void SavePrefs(Prefs pref)
		{
			string prefsFile = Path.Combine(Application.CommonAppDataPath, "Prefs.xml");
			if (File.Exists(prefsFile))
				File.Delete(prefsFile);

			XmlSerializer xml = new XmlSerializer(typeof(Prefs));
			FileStream fs = new FileStream(prefsFile, FileMode.OpenOrCreate, FileAccess.Write);
			xml.Serialize(fs, pref);
			fs.Close();
		}

		private void EditView_Load(object sender, EventArgs e)
		{
			Prefs prefs = new Prefs();

			string prefsFile = Path.Combine(Application.CommonAppDataPath, "Prefs.xml");
			if (File.Exists(prefsFile))
			{
				XmlSerializer xml = new XmlSerializer(typeof(Prefs));
				FileStream fs = new FileStream(prefsFile, FileMode.Open, FileAccess.Read);
				prefs = (Prefs)xml.Deserialize(fs);
				fs.Close();
			}
			else
			{
				FolderBrowserDialog fbd = new FolderBrowserDialog();
				fbd.Description = "Select folder with texture data";
				if (fbd.ShowDialog(this) != DialogResult.OK)
				{
					this.Close();
					return;
				}
				else
				{
					prefs.DataPath = fbd.SelectedPath;
					SavePrefs(prefs);
				}
			}
			Editor = new ClusterEditor(prefs);

			PlanView = new PlanRenderer(MapOverviewBox, Editor.TheWorld, Editor.ThePrefs);
		}

        private void ZBar_Scroll(object sender, EventArgs e)
        {
            PlanView.ZLevel = ZBar.Value;
            MapOverviewBox.Invalidate();
        }

        private void MapOverviewBox_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void MapOverviewBox_MouseMove(object sender, MouseEventArgs e)
        {
            int dX = e.X - mousePos.X;
            int dY = e.Y - mousePos.Y;

            if (e.Button == MouseButtons.Right)
            {
                PlanView.DrawOffset.X += dX;
                PlanView.DrawOffset.Y += dY;
                MapOverviewBox.Invalidate();
            }

            mousePos = new Point(e.X, e.Y);
        }

        Point mousePos = new Point(0, 0);

        private void MapOverviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos = new Point(e.X, e.Y);
        }

        private void MapOverviewBox_MouseUp(object sender, MouseEventArgs e)
        {
            mousePos = new Point(e.X, e.Y);
        }
    }
}

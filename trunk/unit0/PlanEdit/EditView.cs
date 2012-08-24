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

        private void MapOverviewBox_Paint(object sender, PaintEventArgs e)
        {

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
    }
}

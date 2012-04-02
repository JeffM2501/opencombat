using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using GridWorld;
using FileLocations;

namespace Editor.Dialogs
{
    public partial class TextureManagement : Form
    {
        protected World TheWorld = null;
        protected bool Loaded = false;


        Dictionary<string, Bitmap> BitmapCache = new Dictionary<string, Bitmap>();

        public event EventHandler<EventArgs> DataChanged = null;
        public TextureManagement()
        {
            InitializeComponent();
        }

        public void Update(World newWorld)
        {
            BitmapCache.Clear();
            TheWorld = newWorld;

            if (Loaded)
                BuildGUI();
        }

        public void BuildGUI()
        {
            TextureList.Items.Clear();
            foreach (World.TextureInfo info in TheWorld.Info.Textures)
                TextureList.Items.Add(info);

            TextureList.SelectedIndex = 0;
        }

        private void TextureManagement_Load(object sender, EventArgs e)
        {
            Loaded = true;
            BuildGUI();
        }

        bool changingSelections = false;
        private void TextureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            changingSelections = true;

            if (TextureList.SelectedIndex < 0)
            {
                PreviewBox.Image = null;
                InfoBox.Enabled = false;
                BlockX.Value = 0;
                BlockY.Value = 0;
            }
            else
            {
                World.TextureInfo info = TextureList.SelectedItem as World.TextureInfo;
                if (!BitmapCache.ContainsKey(info.FileName))
                    BitmapCache.Add(info.FileName, new Bitmap(Locations.FindDataFile(info.FileName)));

                PreviewBox.Image = BitmapCache[info.FileName];
                InfoBox.Enabled = true;
                BlockX.Value = info.XCount;
                BlockY.Value = info.YCount;

                StartIDTE.Text = info.Start.ToString();
                EndIDTE.Text = info.End.ToString();
            }
            changingSelections = false;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files (*.png)|*.png|JPEG files (*.jpg)|*.jpg;";
            ofd.InitialDirectory = Path.GetDirectoryName(Locations.FindDataFile(TextureList.Items[TextureList.Items.Count - 1].ToString()));
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                string pathPart = ofd.FileName.Remove(Locations.DataPathOveride.Length);
                if (pathPart != Locations.DataPathOveride)
                {
                    MessageBox.Show(this, "Path for image is not in the data directory. Please pick a file that is in the data path.");
                    return;
                }
                string localPath = ofd.FileName.Remove(0,Locations.DataPathOveride.Length+1).Replace('\\','/');

                World.TextureInfo info = new World.TextureInfo(localPath,1,1);
                TheWorld.Info.Textures.Add(info);

                if (DataChanged != null)
                    DataChanged(this,EventArgs.Empty);

                BuildGUI();
                TextureList.SelectedIndex = TextureList.Items.Count - 1;
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (TextureList.Items.Count <= 1)
            {
                MessageBox.Show(this, "There must always be at least one texture");
                return;
            }

            TheWorld.Info.Textures.Remove(TheWorld.Info.Textures[TextureList.SelectedIndex]);

            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);

            BuildGUI();
        }

        private void Block_ValueChanged(object sender, EventArgs e)
        {
            if (changingSelections)
                return;

            int selectedIndex = TextureList.SelectedIndex;
            World.TextureInfo info = TextureList.SelectedItem as World.TextureInfo;
            if (info == null)
                return;

            if (sender == BlockX)
                info.XCount = (int)BlockX.Value;
            else
                info.YCount = (int)BlockY.Value;

            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);

            BuildGUI();

            TextureList.SelectedIndex = selectedIndex;
        }
    }
}

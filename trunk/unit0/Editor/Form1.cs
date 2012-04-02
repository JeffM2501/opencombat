using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

using OpenTK;

using WorldDrawing;
using GridWorld;

namespace Editor
{
    public partial class Form1 : Form
    {
        Edit TheEditor = null;
        EditorConfig Config = new EditorConfig();
        Viewer TheViewer = null;

        Timer timer = null;

        FileInfo FilePath = null;
        string LastMapPath = string.Empty;

        protected Dictionary<int, Bitmap> BlockBitmapCache = new Dictionary<int, Bitmap>();

        public Form1()
        {
            InitializeComponent();
        }

        void SetMenuChecks()
        {
            showGeometryGridToolStripMenuItem.Checked = GridWorldRenderer.DrawDebugLines;
            TheEditor.RegenerateAllGeometry += TheViewer.RegenerateGeometry;
            TheEditor.WorldObjectChanged += TheViewer.WorldObjectChange;

            TheEditor.BlockDefsChanged += new EventHandler<EventArgs>(TheEditor_BlockDefsChanged);
            TheEditor.SelectedBlockChanged += new EventHandler<EventArgs>(TheEditor_SelectedBlockChanged);

            TheEditor.WorldInfoChanged += new EventHandler<EventArgs>(TheEditor_WorldInfoChanged);
            TheEditor.WorldObjectChanged += TheEditor_WorldObjectChanged;
        }

        protected int lastBlockSlection = -1;
        void TheEditor_SelectedBlockChanged(object sender, EventArgs e)
        {
            if (lastBlockSlection != TheEditor.SelectedBlockDef)
            {
                BlockDefList.SelectedIndex = TheEditor.SelectedBlockDef;

                Image bitmap = null;
                if (BlockBitmapCache.ContainsKey(BlockDefList.SelectedIndex))
                    bitmap = BlockBitmapCache[BlockDefList.SelectedIndex];
                else
                {
                    bitmap = MapDrawingUtils.BuildBlockDefImage(BlockDefList.SelectedIndex, TheEditor.TheWorld);
                    BlockBitmapCache.Add(BlockDefList.SelectedIndex,bitmap as Bitmap);
                }
                BlockPreview.Image = bitmap;

                lastBlockSlection = BlockDefList.SelectedIndex;
            }
        }

        void InitGUI()
        {
            InitBlockDefList();
        }

        void InitBlockDefList()
        {
            BlockBitmapCache.Clear();

            BlockDefList.Items.Clear();
            foreach (GridWorld.World.BlockDef def in TheEditor.TheWorld.BlockDefs)
                BlockDefList.Items.Add(def);
        }

        void TheEditor_BlockDefsChanged(object sender, EventArgs e)
        {
            InitBlockDefList();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            SetupEditor();

            TheViewer = new Viewer(TheEditor.TheWorld, glControl1);
            TheViewer.Resize(glControl1);

            Viewer.GetSelections = TheEditor.GetSelections;
            SetMenuChecks();
            InitGUI();

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            Redraw();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Redraw();
        }

        public void Redraw()
        {
            if (TheViewer != null)
                TheViewer.Draw();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (TheViewer != null)
                TheViewer.Draw();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (TheViewer != null)
                TheViewer.Resize(glControl1);
        }

        protected string GetConfigPath()
        {
            return Path.Combine(Application.UserAppDataPath, "edit_config.xml");
        }

        protected void LoadConfig()
        {
            string file = GetConfigPath();
            if (File.Exists(file))
            {
                XmlSerializer xml = new XmlSerializer(typeof(EditorConfig));
                FileInfo configFile = new FileInfo(file);
                FileStream fs = configFile.OpenRead();
                Config = (EditorConfig)xml.Deserialize(fs);
                fs.Close();
            }
        }

        protected void SaveConfig()
        {
            string file = GetConfigPath();
            if (File.Exists(file))
                File.Delete(file);
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(EditorConfig));
                FileInfo configFile = new FileInfo(file);
                FileStream fs = configFile.OpenWrite();
                xml.Serialize(fs, Config);
                fs.Close();
            }
            finally { }
        }

        protected void SetupEditor()
        {
            LoadConfig();

            if (Config.ResourcesPath == string.Empty)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Please select resource path";
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    Config.ResourcesPath = fbd.SelectedPath;
                }
            }

            TheEditor = new Edit(Config);
            SaveConfig();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            if (TextureDialog != null)
            {
                TextureDialog.Close();
                TextureDialog = null;
            }
        }

        Point LastMouseLocation = Point.Empty;
        bool rightDown = false;
        bool leftDown = false;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                rightDown = true;
            if (e.Button == MouseButtons.Left)
            {
                TheEditor.ProjectionClick(TheViewer.PickSelect(e.X, e.Y), ((ModifierKeys & Keys.Shift) != 0));
                leftDown = true;
                Redraw();
            }
            LastMouseLocation = new Point(e.X, e.Y);
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                rightDown = false;
            if (e.Button == MouseButtons.Left)
                leftDown = false;
            LastMouseLocation = new Point(e.X, e.Y);
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            int deltaX = e.X - LastMouseLocation.X;
            int deltaY = e.Y - LastMouseLocation.Y;

            if (rightDown)
            {
                if ((ModifierKeys & Keys.Control) != 0)
                {
                    if ((ModifierKeys & Keys.Alt) != 0)
                        TheViewer.ViewMovment(new Vector3(deltaX * 0.125f, 0, deltaY * 0.125f), 0, 0, false);
                    else
                        TheViewer.ViewMovment(new Vector3(deltaX * 0.125f, deltaY * 0.125f, 0), 0, 0, false);
                }
                else
                    TheViewer.ViewMovment(Vector3.Zero, deltaX * -0.5f, deltaY * -0.5f, false);
            }


            if (leftDown && ((ModifierKeys & Keys.Shift) != 0))
            {
                TheEditor.ProjectionClick(TheViewer.PickSelect(e.X, e.Y), true);
            }

            Redraw();
            LastMouseLocation = new Point(e.X, e.Y);
        }

        private void showGeometryGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridWorldRenderer.DrawDebugLines = !GridWorldRenderer.DrawDebugLines;
            showGeometryGridToolStripMenuItem.Checked = GridWorldRenderer.DrawDebugLines;
            Redraw();
        }

        private void clearMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Really clear the entire map?", "Confirm Map Clear", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                TheEditor.ClearMap();

            Redraw();
        }

        private void BlockDefList_SelectedIndexChanged(object sender, EventArgs e)
        {
            TheEditor.SelectBlockDef(BlockDefList.SelectedIndex);
        }

        private void fillZRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.FillZRange dlog = new Dialogs.FillZRange();

            foreach (GridWorld.World.BlockDef def in TheEditor.TheWorld.BlockDefs)
                dlog.BlockNames.Add(def.Name);

            if (dlog.ShowDialog(this) == DialogResult.OK)
            {
                Vector3 min = new Vector3(dlog.LowerX, dlog.LowerY, dlog.LowerZ);
                Vector3 max = new Vector3(dlog.UpperX, dlog.UpperY, dlog.UpperZ);

                TheEditor.FillZRange(dlog.SelectedBlock, dlog.SelectedGeometry, min, max, dlog.FillEntireMap);
                Redraw();
            }
        }

        protected Dialogs.TextureManagement TextureDialog = null;

        private void manageTextureMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TextureDialog != null)
            {
                TextureDialog.Visible = true;
                return;
            }

            TextureDialog = new Dialogs.TextureManagement();
            TextureDialog.FormClosing += new FormClosingEventHandler(TextureDialog_FormClosing);
            TextureDialog.DataChanged += new EventHandler<EventArgs>(TextureDialog_DataChanged);
            TextureDialog.Update(TheEditor.TheWorld);
            TextureDialog.Show(this);
        }

        bool settingWorldInfo = false;
        void TextureDialog_DataChanged(object sender, EventArgs e)
        {
            settingWorldInfo = true;
            TheEditor.WorldInfoUpdated();
            settingWorldInfo = false;
        }

        void TextureDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            TextureDialog = null;
        }

        void TheEditor_WorldInfoChanged(object sender, EventArgs e)
        {
            if (settingWorldInfo || TextureDialog == null)
                return;

            TextureDialog.Update(TheEditor.TheWorld);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "World files (*." + World.WorldFileExtension + ")|*." + World.WorldFileExtension + "|All files (*.*)|*.*;";

            if (LastMapPath != string.Empty)
                ofd.InitialDirectory = LastMapPath;

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                FilePath = new FileInfo(ofd.FileName);
                LastMapPath = FilePath.DirectoryName;

                TheEditor.OpenWorldFile(ofd.FileName);
                Redraw();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilePath == null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "World files (*." + World.WorldFileExtension + ")|*." + World.WorldFileExtension + ";";

                if (LastMapPath != string.Empty)
                    sfd.InitialDirectory = LastMapPath;

                if (sfd.ShowDialog(this) == DialogResult.OK)
                    FilePath = new FileInfo(sfd.FileName);
                else
                    return;
            }
            LastMapPath = FilePath.DirectoryName;

            TheEditor.SaveWorldFile(FilePath.FullName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilePath = null;
            saveToolStripMenuItem_Click(sender, e);
        }

        void TheEditor_WorldObjectChanged(GridWorld.World world)
        {
            TheEditor_WorldInfoChanged(this, EventArgs.Empty);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void showWorldGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TheViewer.DrawWorldGrid = !TheViewer.DrawWorldGrid;
            showWorldGridToolStripMenuItem.Checked = TheViewer.DrawWorldGrid;
            Redraw();
        }

        private void blockEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.BlockEditor blockEditor = new Dialogs.BlockEditor(TheEditor.TheWorld);
            blockEditor.ShowDialog(this);

            if (blockEditor.NeedGeoRebuild)
                TheEditor.RebuildStaticGeometry();
            Redraw();
        }
    }
}

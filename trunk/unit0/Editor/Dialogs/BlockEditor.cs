using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Renderer;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using GridWorld;
using Textures;
using FileLocations;
using WorldDrawing;

namespace Editor.Dialogs
{
    public partial class BlockEditor : Form
    {
        public World TheWorld = null;

        protected int TextureContext = -1;

        public bool NeedGeoRebuild = false;

        protected SimpleCamera Camera = null;
        public BlockEditor(World world)
        {
            TheWorld = world;
            InitializeComponent();

            BuildGeoList();
            BuildBlockList();

            GeometryList.SelectedIndex = 1;
            BlockList.SelectedIndex = 0;

            int maxTexture = TheWorld.Info.Textures[TheWorld.Info.Textures.Count - 1].End;
            TopID.Maximum = maxTexture;
            BottomID.Maximum = maxTexture;
            NorthID.Maximum = maxTexture;
            SouthID.Maximum = maxTexture;
            EastID.Maximum = maxTexture;
            WestID.Maximum = maxTexture;
        }

        public void BuildGeoList()
        {
            GeometryList.Items.Clear();
            foreach (Cluster.Block.Geometry geo in Enum.GetValues(typeof(Cluster.Block.Geometry)))
                GeometryList.Items.Add(geo);
        }

        public void BuildBlockList()
        {
            BlockList.Items.Clear();
            foreach (World.BlockDef def in TheWorld.BlockDefs)
                BlockList.Items.Add(def);
        }

        protected List<Texture> TextureCache = new List<Texture>();

        protected void CacheTextures()
        {
            TextureCache.Clear();

            foreach (World.TextureInfo info in TheWorld.Info.Textures)
                TextureCache.Add(Texture.Get(Locations.FindDataFile(info.FileName),false,true));
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            TextureContext = Texture.NewContext();
            Texture.SetContext(TextureContext);

            GL.ClearColor(Color.SkyBlue);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);

            // setup light 0
            Vector4 lightInfo = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightInfo);

            lightInfo = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightInfo);
            GL.Light(LightName.Light0, LightParameter.Specular, lightInfo);

            Camera = new SimpleCamera();
            Camera.Pullback = 2.25;
            Camera.Spin = 45;
            Camera.Tilt = 15;

            Camera.ViewPosition = new Vector3(0.5f, 0.5f, 0.5f);

            CacheTextures();

            glControl1_Resize(this, EventArgs.Empty);
            Redraw();
        }

        protected void DrawBox()
        {
            Cluster.Block tempBlock = new Cluster.Block();
            tempBlock.DefID = BlockList.SelectedIndex;
            tempBlock.Geom = (Cluster.Block.Geometry)GeometryList.SelectedItem;

            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;

            int texture = def.Top;

            int textureID = TheWorld.BlockTextureToTextureID(texture);
            int textureOffset = TheWorld.BlockTextureToTextureOffset(texture);

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildAboveGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));

            if (def.Sides != null && def.Sides[0] != World.BlockDef.EmptyID)
            {
                texture = def.Sides[0];

                textureID = TheWorld.BlockTextureToTextureID(texture);
                textureOffset = TheWorld.BlockTextureToTextureOffset(texture);
            }

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildNorthGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));

            if (def.Sides != null && def.Sides.Length > 1 && def.Sides[1] != World.BlockDef.EmptyID)
            {
                texture = def.Sides[1];

                textureID = TheWorld.BlockTextureToTextureID(texture);
                textureOffset = TheWorld.BlockTextureToTextureOffset(texture);
            }

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildSouthGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));

            if (def.Sides != null && def.Sides.Length > 2 && def.Sides[2] != World.BlockDef.EmptyID)
            {
                texture = def.Sides[2];

                textureID = TheWorld.BlockTextureToTextureID(texture);
                textureOffset = TheWorld.BlockTextureToTextureOffset(texture);
            }

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildEastGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));

            if (def.Sides != null && def.Sides.Length > 3 && def.Sides[3] != World.BlockDef.EmptyID)
            {
                texture = def.Sides[3];

                textureID = TheWorld.BlockTextureToTextureID(texture);
                textureOffset = TheWorld.BlockTextureToTextureOffset(texture);
            }

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildWestGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));

            if (def.Bottom != World.BlockDef.EmptyID)
                texture = def.Bottom;
            else
                texture = def.Top;

            textureID = TheWorld.BlockTextureToTextureID(texture);
            textureOffset = TheWorld.BlockTextureToTextureOffset(texture);

            TextureCache[textureID].Bind();
            GridWorldRenderer.DrawFace(ClusterGeometry.GeometryBuilder.BuildBelowGeometry(textureOffset, textureID, 0, 0, 0, tempBlock, TheWorld));
        }

        protected void Redraw()
        {
            if (SettingIndexes || Camera == null)
                return;

            glControl1.MakeCurrent();
            Texture.SetContext(TextureContext);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushMatrix();
            Camera.SetPerspective();

            Camera.Execute();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);

            GL.PushMatrix();
            GL.Rotate(90, Vector3.UnitX);
            DebugDrawing.DrawAxisMarkerLines(0.015f);

            GL.PopMatrix();

            // lighting
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            Vector4 lightInfo = new Vector4(100, -150, 200, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightInfo);
            GL.Enable(EnableCap.Texture2D);

            DrawBox();

            GL.PopMatrix();
            glControl1.SwapBuffers();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Redraw(); 
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            if (Camera != null)
                Camera.Resize(glControl1.Width, glControl1.Height);
        }

        bool SettingIndexes = false;
        private void BlockList_SelectedIndexChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null)
                return;
           
            if (def.Sides == null)
                def.Sides = new int[4] { World.BlockDef.EmptyID, World.BlockDef.EmptyID, World.BlockDef.EmptyID, World.BlockDef.EmptyID };
            else
            {
                int defSize = def.Sides.Length;
                if (def.Sides.Length < 4)
                    Array.Resize(ref def.Sides,4);
                for (int i = defSize; i < 4; i++)
                    def.Sides[i] = World.BlockDef.EmptyID;
            }

            SettingIndexes = true;

            TopID.Value = (decimal)def.Top;

            NorthEnabled.Checked = def.Sides != null && def.Sides[0] != World.BlockDef.EmptyID;
            if (NorthEnabled.Checked)
                NorthID.Value = (decimal)def.Sides[0];
            else
                NorthID.Value = -1;

            SouthEnabled.Checked = def.Sides != null && def.Sides.Length > 1 && def.Sides[1] != World.BlockDef.EmptyID;
            if (SouthEnabled.Checked)
                SouthID.Value = (decimal)def.Sides[1];
            else
                SouthID.Value = -1;

            EastEnabled.Checked = def.Sides != null && def.Sides.Length > 2 && def.Sides[2] != World.BlockDef.EmptyID;
            if (EastEnabled.Checked)
                EastID.Value = (decimal)def.Sides[2];
            else
                EastID.Value = -1;

            WestEnabled.Checked = def.Sides != null && def.Sides.Length > 3 && def.Sides[3] != World.BlockDef.EmptyID;
            if (WestEnabled.Checked)
                WestID.Value = (decimal)def.Sides[3];
            else
                WestID.Value = -1;

            BottomEnabled.Checked = def.Sides != null && def.Bottom != World.BlockDef.EmptyID;
            if (BottomEnabled.Checked)
                BottomID.Value = (decimal)def.Bottom;
            else
                BottomID.Value = -1;


            Trans.Checked = def.Transperant;

            SettingIndexes = false;
            Redraw();
        }

        private void BlockEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TextureContext > 0)
                Texture.RemoveContext(TextureContext);
        }

        private void NorthEnabled_CheckedChanged(object sender, EventArgs e)
        {
            NorthID.Enabled = NorthSet.Enabled = NorthEnabled.Checked;

            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (SettingIndexes)
                return;

            if (!NorthEnabled.Checked)
                NorthID.Value = World.BlockDef.EmptyID;
            else
                NorthID.Value = TopID.Value;
            Redraw();
        }

        private void SouthEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SouthID.Enabled = SouthSet.Enabled = SouthEnabled.Checked;
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (SettingIndexes)
                return;

            if (!SouthEnabled.Checked)
                SouthID.Value = World.BlockDef.EmptyID;
            else
                SouthID.Value = def.Top;

            Redraw();
        }

        private void EastEnabled_CheckedChanged(object sender, EventArgs e)
        {
            EastID.Enabled = EastSet.Enabled = EastEnabled.Checked;
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (SettingIndexes)
                return;

            if (!EastEnabled.Checked)
                EastID.Value = World.BlockDef.EmptyID;
            else
                EastID.Value = def.Top;

            Redraw();
        }

        private void WestEnabled_CheckedChanged(object sender, EventArgs e)
        {
            WestID.Enabled = WestSet.Enabled = WestEnabled.Checked;
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (SettingIndexes)
                return;

            if (!WestEnabled.Checked)
                WestID.Value = World.BlockDef.EmptyID;
            else
                WestID.Value = def.Top;

            Redraw();
        }

        private void BottomEnabled_CheckedChanged(object sender, EventArgs e)
        {
            BottomID.Enabled = BottomSet.Enabled = BottomEnabled.Checked;
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            if (!BottomEnabled.Checked)
                def.Bottom = World.BlockDef.EmptyID;
            else
                def.Bottom = def.Top;

            Redraw();
        }

        private void GeometryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        bool LeftDown = false;
        bool RightDown = false;
        Point lastMouse = Point.Empty;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                LeftDown = true;
            if (e.Button == MouseButtons.Right)
                RightDown = true;
            lastMouse = new Point(e.X, e.Y);
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                LeftDown = false;
            if (e.Button == MouseButtons.Right)
                RightDown = false;
            lastMouse = new Point(e.X, e.Y);
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            int dx = e.X - lastMouse.X;
            int dy = e.Y - lastMouse.Y;

            if (RightDown)
            {
                Camera.Spin += dx * 0.25f;
                Camera.Tilt -= dy * 0.25f;
                if (Camera.Tilt > 90)
                    Camera.Tilt = 90;
                if (Camera.Tilt < -90)
                    Camera.Tilt = -90;
            }

            Redraw();
            lastMouse = new Point(e.X, e.Y);
        }

        protected bool GetNewTextureID ( ref int offset )
        {
            Dialogs.TexturePicker tp = new Dialogs.TexturePicker(TheWorld);
            tp.TextureID = TheWorld.BlockTextureToTextureID(offset);
            tp.TextureOffset = offset;

            tp.ShowDialog(this);

            offset = tp.TextureOffset;
                 
            return true;
        }

        private void TopSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Top;
            if (GetNewTextureID(ref id))
            {
                TopID.Value = id;
                Redraw();
            }
        }

        private void NorthSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Sides[0];
            if (GetNewTextureID(ref id))
            {
                NorthID.Value = id;
                Redraw();
            }
        }

        private void SouthSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Sides[1];
            if (GetNewTextureID(ref id))
            {
                SouthID.Value = id;
                Redraw();
            }
        }

        private void EastSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Sides[2];
            if (GetNewTextureID(ref id))
            {
                EastID.Value = id;
                Redraw();
            }
        }

        private void WestSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Sides[3];
            if (GetNewTextureID(ref id))
            {
                WestID.Value = id;
                Redraw();
            }
        }

        private void BottomSet_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            int id = def.Bottom;
            if (GetNewTextureID(ref id))
            {
                BottomID.Value = id;
                Redraw();
            }
        }

        private void TopID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Top = (int)TopID.Value;
            Redraw();
        }

        private void NorthID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            def.Sides[0] = (int)NorthID.Value;
            Redraw();
        }

        private void SouthID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Sides[1] = (int)SouthID.Value;
            Redraw();
        }

        private void EastID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Sides[2] = (int)EastID.Value;
            Redraw();
        }

        private void WestID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Sides[3] = (int)WestID.Value;
            Redraw();
        }

        private void BottomID_ValueChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Bottom = (int)BottomID.Value;
            Redraw();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Dialogs.StringQueryDialog dlog = new Dialogs.StringQueryDialog();
            dlog.Title = "New Block";
            dlog.Query = "Name for new block?";
            dlog.Return = "New Block " + TheWorld.BlockDefs.Count.ToString();

            if (dlog.ShowDialog(this) == DialogResult.OK)
            {
                World.BlockDef def = new World.BlockDef();
                def.Name = dlog.Return;
                if (def.Name == string.Empty)
                    def.Name = "New Block " + TheWorld.BlockDefs.Count.ToString();

                def.Top = 0;
                def.Transperant = false;

                TheWorld.BlockDefs.Add(def);

                BuildBlockList();
                BlockList.SelectedIndex = BlockList.Items.Count - 1;
                Redraw();
            }
        }

        private void Trans_CheckedChanged(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;
            NeedGeoRebuild = true;
            def.Transperant = Trans.Checked;
        }

        private void Rename_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            Dialogs.StringQueryDialog dlog = new Dialogs.StringQueryDialog();
            dlog.Title = "Rename Block";
            dlog.Query = "Name for new block?";
            dlog.Return = def.Name;

            if (dlog.ShowDialog(this) == DialogResult.OK)
            {
                def.Name = dlog.Return;

                int selectedBlock = BlockList.SelectedIndex;
                BuildBlockList();
                BlockList.SelectedIndex = selectedBlock;
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            World.BlockDef def = BlockList.SelectedItem as World.BlockDef;
            if (def == null || SettingIndexes)
                return;

            if (TheWorld.BlockDefs.Count == 1)
            {
                MessageBox.Show("This", "Cannot remove block 0, there must always be one block");
                return;
            }

            if (MessageBox.Show(this, "Really remove block " + def.Name, "Confirm Block Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                TheWorld.BlockDefs.Remove(def);
                NeedGeoRebuild = true;

                BuildBlockList();
                BlockList.SelectedIndex = BlockList.Items.Count - 1;
                Redraw();
            }
        }
    }
}

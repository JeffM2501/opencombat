using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Drawables.Cameras;
using Drawables.Textures;
using Utilities.Paths;

using GUIObjects;

namespace GUIElementConstructor
{
    public partial class Editor : Form
    {
        Camera camera;

        Stopwatch stopwatch = new Stopwatch();

        Texture cautionTexture;
        Texture grass;

        bool LoadingInterface = false;

        public Editor()
        {
            InitializeComponent();
        }

        private void GLView_Load(object sender, EventArgs e)
        {
            GL.ClearColor(System.Drawing.Color.LightSkyBlue);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.LineSmooth);
            GL.LightModel(LightModelParameter.LightModelColorControl, 1);

            // setup light 0
            Vector4 lightInfo = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightInfo);

            lightInfo = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightInfo);
            GL.Light(LightName.Light0, LightParameter.Specular, lightInfo);

            camera = new Camera();
            camera.set(new Vector3(0, -8, 5), -25, 90);

            ResourceManager.AddPath("../../../../../p2501_data");
            ResourceManager.AddPath("../../../../p2501_data");
            ResourceManager.AddPath("../../../p2501_data");
            ResourceManager.AddPath("../../p2501_data");
            ResourceManager.AddPath("../p2501_data");
            ResourceManager.AddPath("./p2501_data");

            GUIObjectManager.AddDefaultComponents();

            cautionTexture = TextureSystem.system.GetTexture(ResourceManager.FindFile("Textures/BZ/caution.png"));
            grass = TextureSystem.system.GetTexture(ResourceManager.FindFile("Textures/BZ/grass.png"));

            stopwatch.Start();
            UpdateTimer.Start();

            GLView_Resize(this,EventArgs.Empty);

            foreach (KeyValuePair<string,Type> component in GUIObjectManager.Components)
                ComponentList.Items.Add(component.Key);

            ObjectDataPannel.Enabled = false;
            Add.Enabled = false;
        }

        protected void XZQuad( float width, float height)
        {
            GL.Begin(BeginMode.Quads);

            GL.Normal3(0, 1, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 0, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(width, 0, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(width, 0, height);

            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, height);

            GL.End();
        }

        protected void XZQuadBackwards(float width, float height)
        {
            GL.Begin(BeginMode.Quads);

            GL.Normal3(0, -1, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, height);
           
            GL.TexCoord2(1, 0);
            GL.Vertex3(width, 0, height);

            GL.TexCoord2(1, 1);
            GL.Vertex3(width, 0, 0);
            
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 0, 0);

            GL.End();
        }

        private void GLView_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            camera.SetPersective();
            camera.Execute();

            GL.PushMatrix();
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0, -15, 10, 1.0f));

            float angle = stopwatch.ElapsedMilliseconds / 1000f * 45f;
            if (cautionTexture != null)
                cautionTexture.Bind();

            GL.Rotate(angle, 0, 0, 1);
            GL.Color4(Color.White);
            for (int i = 0; i < 4; i++)
            {
                GL.Rotate(90, 0, 0, 1);
                XZQuad(2, 2);
                XZQuadBackwards(2, 2);
            }
            GL.PopMatrix();

            GL.PushMatrix();

            GL.Translate(-15, -15, 0);
            if (grass != null)
                grass.Draw(30, 30, 128);

            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            camera.SetOrthographic();

            GL.Translate(0, 0, -1);

            DrawUI();

            GLView.SwapBuffers();
        }

        private void DrawUI ()
        {
            foreach(KeyValuePair<string,GUIObject> element in GUIObjectManager.Elements)
                element.Value.Draw(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private void GLView_Resize(object sender, EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, GLView.Size.Width, GLView.Size.Height);
            GUIObjectManager.Resize(GLView.Size.Width, GLView.Size.Height);
            if (camera != null)
                camera.Resize(GLView.Size.Width, GLView.Size.Height);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            GLView.Invalidate(true);
        }

        private TreeNode AddControlsToNode ( TreeNode node, GUIObject obj )
        {
            foreach (GUIObject child in obj.Children)
            {
                TreeNode childNode = new TreeNode(child.Name, 1, 1);
                childNode.Tag = child;
                node.Nodes.Add(childNode);

                AddControlsToNode(childNode, child);
            }
            return node;
        }

        private void UpdateUIList()
        {
            ElementTree.Nodes.Clear();

            foreach (KeyValuePair<string, GUIObject> element in GUIObjectManager.Elements)
            {
                TreeNode node = new TreeNode(element.Key, 0, 0);
                node.Tag = element.Value;
                ElementTree.Nodes.Add(node);

                AddControlsToNode(node, element.Value); ;
            }

            ElementTree.ExpandAll();
            UpdateComponentInfo(null);
        }

        protected void UpdateComponentInfo ( GUIObject obj )
        {
            if (obj == null)
            {
                ObjectDataPannel.Enabled = false;
                return;
            }
            ObjectDataPannel.Enabled = true;
            SuspendLayout();
            LoadingInterface = true;
            GUIObject.ElementDefinition def = obj.GetDefinition(false);

            XPos.Value = (decimal)obj.Poisition.X;
            YPos.Value = (decimal)obj.Poisition.Y;

            XSize.Value = (decimal)obj.Size.Width;
            YSize.Value = (decimal)obj.Size.Height;

            BGColorPanel.BackColor = Color.FromArgb(255,obj.BackgroundColor);
            BGAlpha.Value = (decimal)(obj.BackgroundColor.A / 255f * 100);
            FGColorPanel.BackColor = Color.FromArgb(255,obj.ForegroundColor);
            FGAlpha.Value = (decimal)(obj.ForegroundColor.A / 255f * 100);

            ValueName.Text = obj.ValueName;

            Options.Items.Clear();
            foreach (GUIObject.ElementDefinition.OptionValue value in def.Options)
            {
                string[] temp = new string[2];
                temp[0] = value.Name;
                temp[1] = value.Value;

                Options.Items.Add(new ListViewItem(temp));
            }
            LoadingInterface = false;
            ResumeLayout();
        }

        private void NewElement_Click(object sender, EventArgs e)
        {
            GUIObject.ElementDefinition def = new GUIObject.ElementDefinition();
            def.Name = "New Element";
            GUIObjectManager.AddElement(def);
            UpdateUIList();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject parent = ElementTree.SelectedNode.Tag as GUIObject;
            string component = ComponentList.SelectedItem as string;
            if (parent == null || component == null)
                return;

            parent.Children.Add(GUIObjectManager.CreateComponent(component));
            parent.Bind();

            UpdateUIList();
        }

        private void ElementTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (LoadingInterface)
                return;

            GUIObject obj = e.Node.Tag as GUIObject;
            if (obj == null)
                return;

            ElementTree.SuspendLayout();
            if (e.Node.ImageIndex != 0)
                e.Node.Text = string.Copy(obj.Name);
            else
                obj.Name = string.Copy(e.Label);

            ElementTree.ResumeLayout();
        }

        private void ElementTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            UpdateComponentInfo(e.Node.Tag as GUIObject);
        }

        private void Options_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void BGColorPanel_Click(object sender, EventArgs e)
        {
           if (ElementTree.SelectedNode == null)
            return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            ColorDialog picker = new ColorDialog();
            picker.AllowFullOpen = true;
            picker.AnyColor = true;
            picker.Color = obj.BackgroundColor;
            if (picker.ShowDialog(this) == DialogResult.OK)
            {
                obj.BackgroundColor = Color.FromArgb(GetBGAlpha(),picker.Color);
                BGColorPanel.BackColor = picker.Color;
            }
        }

        private void FGColorPanel_Click(object sender, EventArgs e)
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            ColorDialog picker = new ColorDialog();
            picker.AllowFullOpen = true;
            picker.AnyColor = true;
            picker.Color = obj.ForegroundColor;
            if (picker.ShowDialog(this) == DialogResult.OK)
            {
                obj.ForegroundColor = Color.FromArgb(GetFGAlpha(),picker.Color);
                FGColorPanel.BackColor = picker.Color;
            }
        }

        private void Pos_ValueChanged(object sender, EventArgs e)
        {
            if (LoadingInterface)
                return;
            
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            obj.Poisition = new Point((int)XPos.Value, (int)YPos.Value);
            GLView.Invalidate(true);
        }

        private void Size_ValueChanged(object sender, EventArgs e)
        {
            if (LoadingInterface)
                return;
            
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            obj.Size = new Size((int)XSize.Value, (int)YSize.Value);
            GLView.Invalidate(true);
        }

        int GetBGAlpha ()
        {
            return (int)(BGAlpha.Value/100*255);
        }

        int GetFGAlpha()
        {
            return (int)(FGAlpha.Value / 100 * 255);
        }

        private void BGAlpha_ValueChanged(object sender, EventArgs e)
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            Color newColor = Color.FromArgb(GetBGAlpha(), obj.BackgroundColor.R, obj.BackgroundColor.G, obj.BackgroundColor.B);
            obj.BackgroundColor = newColor;
            BGColorPanel.BackColor = Color.FromArgb(255,newColor);
        }

        private void FGAlpha_ValueChanged(object sender, EventArgs e)
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            Color newColor = Color.FromArgb(GetFGAlpha(), obj.ForegroundColor.R, obj.ForegroundColor.G, obj.ForegroundColor.B);
            obj.ForegroundColor = newColor;
            FGColorPanel.BackColor = Color.FromArgb(255, newColor);
        }

        private void RebuildOptions ()
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            GUIObject.ElementDefinition def = obj.GetDefinition(false);

            def.Options.Clear();
            foreach(ListViewItem item in Options.Items)
            {
                GUIObject.ElementDefinition.OptionValue val = new GUIObject.ElementDefinition.OptionValue();
                val.Name = item.Text;
                val.Value = item.SubItems[1].Text;
                def.Options.Add(val);
            }

            obj.CreateFromDefinition(def, false);
        }

        private void Options_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            RebuildOptions();
        }

        private void AddOption_Click(object sender, EventArgs e)
        {
            string[] temp = new string[2];
            temp[0] = "New Option";
            temp[1] = "VALUE";

            Options.Items.Add(new ListViewItem(temp));
        }

        private void RemoveOption_Click(object sender, EventArgs e)
        {
            ListViewItem item = GetSelectedOption();
            if (item == null)
                return;

            Options.Items.Remove(item);
            RebuildOptions();
        }

        private void Options_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = GetSelectedOption();
            if (item == null)
                return;

            OptionEditor dlg = new OptionEditor();

            dlg.OptionName = item.Text;
            dlg.OptionValue = item.SubItems[1].Text;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                item.Text = dlg.OptionName;
                item.SubItems[1].Text = dlg.OptionValue;
            }
            RebuildOptions();
        }

        ListViewItem GetSelectedOption()
        {
            if (Options.SelectedItems.Count == 0)
                return null;

            return Options.SelectedItems[0];
        }

        private void ComponentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Add.Enabled = ComponentList.SelectedItem != null;
        }

        private void ValueName_TextChanged(object sender, EventArgs e)
        {
            if (ElementTree.SelectedNode == null)
                return;

            GUIObject obj = ElementTree.SelectedNode.Tag as GUIObject;
            if (obj == null)
                return;

            GUIObject.ElementDefinition def = obj.GetDefinition(false);

            def.ValueName = ValueName.Text;
            obj.CreateFromDefinition(def, false);
            obj.Bind();
        }
    }
}

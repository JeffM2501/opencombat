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
            GUIObject.ElementDefinition def = obj.GetDefinition(false);

            XPos.Value = (decimal)obj.Poisition.X;
            YPos.Value = (decimal)obj.Poisition.Y;

            XSize.Value = (decimal)obj.Size.Width;
            YSize.Value = (decimal)obj.Size.Height;

            BGColorPanel.BackColor = obj.BackgroundColor;
            FGColorPanel.BackColor = obj.ForegroundColor;
            ValueName.Text = obj.ValueName;

            Options.Items.Clear();
            foreach (GUIObject.ElementDefinition.OptionValue value in def.Options)
            {
                string[] temp = new string[2];
                temp[0] = value.Name;
                temp[1] = value.Value;

                Options.Items.Add(new ListViewItem(temp));
            }

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
            GUIObject parent = ElementTree.SelectedNode.Tag as GUIObject;
            string component = ComponentList.SelectedItem as string;
            if (parent == null || component == null)
                return;

            parent.Children.Add(GUIObjectManager.CreateComponent(component));

            UpdateUIList();
        }

        private void ElementTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
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

        }

        private void Options_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Options_DoubleClick(object sender, EventArgs e)
        {

        }

        private void BGColorPanel_Click(object sender, EventArgs e)
        {

        }

        private void FGColorPanel_Click(object sender, EventArgs e)
        {

        }
    }
}

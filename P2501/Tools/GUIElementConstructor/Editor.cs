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
            foreach(GUIObject element in GUIObjectManager.Elements)
                element.Draw(stopwatch.ElapsedMilliseconds / 1000.0);
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
    }
}

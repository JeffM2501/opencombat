using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Drawables.Cameras;
using Utilities.Paths;

using GUIObjects;

namespace GUITest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Visual().Run();
        }
    }

    public class Visual : GameWindow
    {
        Camera camera;

        public Visual() : base(1024,800)
        {
        }

        GUIObject sampleElement;

        protected void LoadTestData ()
        {
            GUIObject.ElementDefinition def = new GUIObject.ElementDefinition();
            def.Name = "SampleFrame";
            def.Position = new Point(100,200);
            
            GUIObject.ElementDefinition def2 = new GUIObject.ElementDefinition();
            def2.Name = "Frame";
            def2.Position = new Point(0, 0);
            def2.Size = new Size(300, 284);
            def2.SetOptionValue("BackgroundTexture", "class1_300.png");

            def.Children.Add(def2);
            GUIObjectManager.AddElement(def);

            sampleElement = GUIObjectManager.GetElement("SampleFrame");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(System.Drawing.Color.SkyBlue);
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

            camera = new Camera();

            ResourceManager.AddPath("../../data");
            ResourceManager.AddPath("../data");
            ResourceManager.AddPath("./data");

            GUIObjectManager.AddDefaultComponents();

            OnResize(EventArgs.Empty);
            LoadTestData();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            if (camera != null)
                camera.Resize(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);

            camera.SetOrthographic();

            GL.Translate(0, 0, -1);

            sampleElement.Draw(0);

            SwapBuffers();
        }
    }
}

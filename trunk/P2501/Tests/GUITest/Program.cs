using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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

        GlobalValue changingVal = new GlobalValue();

        public Visual() : base(1024,800)
        {
        }

        GUIObject sampleElement;

        protected void LoadTestData ()
        {
            changingVal.Name = "DynamicVal";
            changingVal.Value = "10";

            GlobalValueCache.Values.Add(changingVal.Name,changingVal);

            GUIObject.ElementDefinition def = new GUIObject.ElementDefinition();
            def.Name = "SampleFrame";
            def.Position = new Point(100,200);
            
            GUIObject.ElementDefinition def2 = new GUIObject.ElementDefinition();
            def2.Name = "Frame";
            def2.Position = new Point(0, 0);
            def2.Size = new Size(320, 284);
            def2.SetOptionValue("BackgroundTexture", "brushed.png");
            def2.SetOptionValue("RepeatTexture", "On");

            GUIObject.ElementDefinition def3 = new GUIObject.ElementDefinition();
            def3.Name = "GroupBox";
            def3.Position = new Point(10, 10);
            def3.Size = new Size(300, 150);
            def3.ValueName = "Group";
            def2.Children.Add(def3);

            GUIObject.ElementDefinition def4 = new GUIObject.ElementDefinition();
            def4.Name = "Label";
            def4.Position = new Point(5, 5);
            def4.ValueName = "Frame Time";
            def3.Children.Add(def4);

            def4 = new GUIObject.ElementDefinition();
            def4.Name = "ValueLabel";
            def4.Position = new Point(100, 5);
            def4.ValueName = "DynamicVal";
            def3.Children.Add(def4);

            def4 = new GUIObject.ElementDefinition();
            def4.Name = "Picture";
            def4.Position = new Point(5, 25);
            def4.Size = new Size(128, 128);
            def4.SetOptionValue("Image", "kspaceduel.png");
            //def4.SetOptionValue("ClampImage", "True");
            def3.Children.Add(def4);


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

            DirectoryInfo dir = new DirectoryInfo("./");
         //   GUIObjectManager.SaveAllElements(dir);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            GUIObjectManager.Resize(Width, Height);
            if (camera != null)
                camera.Resize(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            changingVal.Value = e.Time.ToString();

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

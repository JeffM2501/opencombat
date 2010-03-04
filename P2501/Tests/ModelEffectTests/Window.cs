using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Drawables.Cameras;
using Drawables.DisplayLists;

namespace ModelEffectTests
{
    class Window : GameWindow
    {

        public Camera camera = new Camera();

        public Window ( ) : base(1024,640)
        {
            VSync = VSyncMode.Off;
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

            // setup light 0
            Vector4 lightInfo = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightInfo);

            lightInfo = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightInfo);
            GL.Light(LightName.Light0, LightParameter.Specular, lightInfo);

            camera.set(new Vector3(-10, 0, 2), 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            camera.Resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            camera.Execute();

            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(10, 15, 10, 1.0f));

            GridList_Generate(null, null);
            DrawGridAxisMarker();

            SwapBuffers();
        }

        protected void DrawGridAxisMarker()
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.LineSmooth);

            GL.LineWidth(2);
            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.Blue);
            GL.Vertex3(-1, 0, 0);
            GL.Vertex3(2, 0, 0);

            GL.Color3(Color.Red);
            GL.Vertex3(0, -1, 0);
            GL.Vertex3(0, 2, 0);

            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, -1);
            GL.Vertex3(0, 0, 2);
            GL.End();

            GL.Enable(EnableCap.LineSmooth);
            GL.PopMatrix();
            GL.Enable(EnableCap.Lighting);
            GL.DepthMask(true);
        }

        float GridSubDivisions = 1.0f;
        float GridSize = 50;
        float GridStep = 2;

        void GridList_Generate(object sender, DisplayList list)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.LineSmooth);
            GL.DepthMask(false);

            float gridSize = GridSize;

            GL.PushMatrix();
            GL.Translate(0, 0, 0);

            GL.Color4(Color.FromArgb(128, Color.LightSlateGray));
            GL.LineWidth(1);

            if (GridSubDivisions > 0)
            {
                GL.Begin(BeginMode.Lines);

                for (float i = 0; i < gridSize; i += GridSubDivisions)
                {
                    //    if (i - (int)i < GridSubDivisions)
                    //       continue;

                    GL.Vertex2(gridSize, i);
                    GL.Vertex2(-gridSize, i);

                    GL.Vertex2(gridSize, -i);
                    GL.Vertex2(-gridSize, -i);

                    GL.Vertex2(i, gridSize);
                    GL.Vertex2(i, -gridSize);

                    GL.Vertex2(-i, gridSize);
                    GL.Vertex2(-i, -gridSize);
                }
                GL.End();
            }
            GL.LineWidth(2);
            GL.Begin(BeginMode.Lines);
            GL.Color4(Color.FromArgb(128, Color.LightGray));

            for (float i = 0; i < gridSize; i += GridStep)
            {
                GL.Vertex2(gridSize, i);
                GL.Vertex2(-gridSize, i);

                GL.Vertex2(gridSize, -i);
                GL.Vertex2(-gridSize, -i);

                GL.Vertex2(i, gridSize);
                GL.Vertex2(i, -gridSize);

                GL.Vertex2(-i, gridSize);
                GL.Vertex2(-i, -gridSize);
            }
            GL.End();

            GL.LineWidth(3);
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.Snow);
            GL.Vertex3(-gridSize, 0, 0);
            GL.Vertex3(gridSize, 0, 0);
            GL.Vertex3(0, -gridSize, 0);
            GL.Vertex3(0, gridSize, 0);
            GL.End();

            GL.Enable(EnableCap.LineSmooth);
            GL.PopMatrix();
            GL.Enable(EnableCap.Lighting);
            GL.DepthMask(true);
        }
    }
}

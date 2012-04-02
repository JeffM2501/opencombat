using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using GridWorld;
using WorldDrawing;
using Renderer;
using Textures;

using Math3D;

namespace Editor
{
    public partial class Viewer
    {
        GridWorldRenderer WorldRenderer = null;

        public FrustumCamera Camera = null;

        protected GLControl CTL = null;

        protected DebugDrawing debugDrawer = null;

        public delegate List<Viewer.Selection> GetSelectionsCB();
        public static GetSelectionsCB GetSelections = null;

        public bool DrawWorldGrid = false;

        public Viewer(World world, OpenTK.GLControl control)
        {
            CTL = control;

            InitGL();
           // GridWorldRenderer.UseVBO = false;
            WorldRenderer = new GridWorldRenderer(world);
            GridWorldRenderer.DrawDebugLines = false;
            WorldRenderer.StaticInit();
        }

        protected void ChangeWorld( World newWorld )
        {
            WorldRenderer.ChangeWorld(newWorld);
        }

        public void RegenerateGeometry(object sender, EventArgs args)
        {
            StaticVertexBufferObject.KillAll();
            DisplayList.KillAll();
            WorldRenderer.ChangeWorld(WorldRenderer.GameWorld);
        }

        public void WorldObjectChange(GridWorld.World world)
        {
            StaticVertexBufferObject.KillAll();
            DisplayList.KillAll();
            WorldRenderer.ChangeWorld(world);
        }

        protected void InitGL()
        {
            if (CTL != null)
                CTL.MakeCurrent();

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

            Camera = new FrustumCamera();

            Camera.NearPlane = 0.001f;

            Camera.FarPlane = 250.0f;
            Camera.setPos(0, 0, 9);
            Camera.set(new Vector3(0, 0, 9), 0, 0);

            debugDrawer = new DebugDrawing();
        }

        public void ViewMovment(Vector3 linear, float spin, float tilt, bool abs)
        {
            if (abs)
            {
                Camera.set(linear, spin, tilt);
            }
            else
            {
                Camera.turn(tilt, spin);

                float currentZ = Camera.EyePoint.Z;
                Camera.MoveRelitive(linear);

                Camera.set(new Vector3(Camera.EyePoint.X, Camera.EyePoint.Y, currentZ + linear.Z));
                
            }
        }

        public void Resize(OpenTK.GLControl control)
        {
            CTL = control;
            control.MakeCurrent();
            GL.Viewport(0, 0, control.Width, control.Height);
            if (Camera != null)
                Camera.Resize(control.Width, control.Height);

            if (debugDrawer != null)
                debugDrawer.Resize(control.Width, control.Height);
        }

        protected void DrawSelections()
        {
            if (GetSelections == null)
                return;

            List<Selection> selections = GetSelections();

            if (selections.Count == 0)
                return;

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            GL.LineWidth(2);
            GL.Color4(Color.Red);
            foreach (Selection selection in selections)
            {
                GL.PushMatrix();
                GL.Translate(selection.GlobalBlock);
                
                GridWorldRenderer.DrawConerLines();
                GL.PopMatrix();
            }

            GL.LineWidth(1);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
        }

        public void Draw()
        {
            CTL.MakeCurrent();
            Texture.SetContext(0);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushMatrix();
            Camera.SetPersective();

            Camera.Execute();

            GL.Enable(EnableCap.Texture2D);

            if (WorldRenderer != null)
                WorldRenderer.DrawSky(Camera);

            DrawUnderGrid();

            // lighting
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            Vector4 lightInfo = new Vector4(WorldRenderer.GameWorld.Info.SunPosition.X, WorldRenderer.GameWorld.Info.SunPosition.Y, WorldRenderer.GameWorld.Info.SunPosition.Z, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightInfo);

            WorldRenderer.DrawVisible(Camera.ViewFrustum);
            WorldRenderer.DrawAll();

            GL.Clear(ClearBufferMask.DepthBufferBit);

            DrawSelections();

            debugDrawer.DrawAxisMarkers(Camera);

            Camera.SetOrthographic();

            GL.PopMatrix();
            CTL.SwapBuffers();
        }

        public void DrawUnderGrid()
        {
            if (!DrawWorldGrid)
                return;

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);

            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            // axis markers
            GL.PushMatrix();
            GL.Rotate(90, Vector3.UnitX);
            DebugDrawing.DrawAxisMarkerLines(0.25f);

            GL.PopMatrix();

            float alpha = 0.125f;

            GL.LineWidth(3.0f);
            GL.Begin(BeginMode.Lines);
            int size = Cluster.XYSize * 10;
            for (int z = 0; z <= Cluster.ZSize; z += Cluster.ZSize)
            {
                for (int i = -size; i <= size; i++)
                {
                    if (i % Cluster.XYSize != 0)
                        GL.Color4(1f, 1f, 1f, alpha);
                    else
                        GL.Color4(0.5f, 0.5f, 1f, alpha*2);

                    GL.Vertex3(-size, i, z);
                    GL.Vertex3(size, i, z);

                    GL.Vertex3(i, -size, z);
                    GL.Vertex3(i, size, z);
                }
            }

            GL.Color4(0.5f, 0.5f, 1f, alpha * 2);
            for (int x = -size; x <= size; x += Cluster.XYSize)
            {
                for (int y = -size; y <= size; y += Cluster.XYSize)
                {
                    GL.Vertex3(x, y, 0);
                    GL.Vertex3(x, y, Cluster.ZSize);
                }
            }

            GL.End();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            GL.LineWidth(1);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

        }
    }
}

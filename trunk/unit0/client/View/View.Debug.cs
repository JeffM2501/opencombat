using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;

using GridWorld;
using Game;
using Renderer;

using FileLocations;
using Textures;

namespace Client
{
    partial class View
    {
        public Vector3 debugRayStart = new Vector3(22, 21, 7);

        protected Texture Grid = null;

        protected StaticVertexBufferObject vbo1 = null;
        protected List<StaticVertexBufferObject> vbos = new List<StaticVertexBufferObject>();

        Font chatFont = new Font(FontFamily.GenericSerif, 12);
        public TextPrinter printer = new TextPrinter(TextQuality.High);


        public void DrawDebugPerspectiveGUI()
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            DrawAxisMarkers();

            GL.LineWidth(1);
        }

        public void DrawDebugOrthoGUI()
        {
        //    DrawFPS();
            DrawDebugLogLines();
        }

        protected void DrawDebugCrap()
        {
            if (Grid == null)
                Grid = Texture.Get(Locations.FindDataFile("world/grid.png"));

           // DrawDebugObjects();

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.DepthMask(false);

//          DrawDebugRay();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.LineWidth(1);
            
        }

        void DrawFPS()
        {
            printer.Begin();
            printer.Print("FPS:" + (1.0f / Window.RenderTime).ToString(), chatFont, Color.White, new RectangleF(0, 0, 100, 25), TextPrinterOptions.Default);
            printer.End();
        }

        public void AddDebugLogItem(string key, string value)
        {
            if (DebugLogLines.ContainsKey(key))
                DebugLogLines[key] = value;
            else
                DebugLogLines.Add(key, value);
        }

        void DrawDebugLogLines()
        {
            int lineHeight = 25;
            int height = Window.Height - (DebugLogLines.Count * lineHeight);

            printer.Begin();
            foreach (KeyValuePair<string, string> line in DebugLogLines)
            {
                printer.Print(line.Key + ":"+ line.Value, chatFont, Color.White, new RectangleF(0, height, 250, lineHeight), TextPrinterOptions.Default);
                height += lineHeight;
            }

            printer.End();
        }

        void DrawDebugObjects()
        {
            if (vbo1 == null)
            {
                Random rand = new Random();
                vbo1 = new StaticVertexBufferObject(buildVBO);

                for (int i = 0; i < 2500; i++)
                {
                    StaticVertexBufferObject o = new StaticVertexBufferObject(buildVBO);
                    Vector3 offset = new Vector3((float)(rand.NextDouble() * 100), (float)(rand.NextDouble() * 100), (float)(rand.NextDouble() * 25));
                    o.Tag = offset;
                    vbos.Add(o);
                }
            }

            Grid.Bind();
            vbo1.Draw();
            foreach (StaticVertexBufferObject o in vbos)
                o.Draw();
        }

        StaticVertexBufferObject.BufferData buildVBO(StaticVertexBufferObject obj)
        {
            StaticVertexBufferObject.ShortIndexFullVertBuffer buffer = new StaticVertexBufferObject.ShortIndexFullVertBuffer();

            List<StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData> verts = new List<StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData>();
            List<short> indexes = new List<short>();

            Vector3 offset = Vector3.Zero;
            if (obj.Tag != null && obj.Tag.GetType() == typeof(Vector3))
                offset = (Vector3)obj.Tag;

            if (obj == vbo1)
            {
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(Vector2.Zero, Color.White, Vector3.UnitZ, new Vector3(10, 10, 10) + offset));
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(new Vector2(1, 0), Color.White, Vector3.UnitZ, new Vector3(20, 10, 10) + offset));
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(new Vector2(1, 1), Color.White, Vector3.UnitZ, new Vector3(20, 20, 10) + offset));

                indexes.Add(0);
                indexes.Add(1);
                indexes.Add(2);
            }
            else
            {
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(Vector2.Zero, Color.White, Vector3.UnitZ, new Vector3(20, 20, 10) + offset));
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(new Vector2(1, 0), Color.White, Vector3.UnitZ, new Vector3(30, 20, 10) + offset));
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(new Vector2(1, 1), Color.White, Vector3.UnitZ, new Vector3(30, 30, 10) + offset));
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(new Vector2(0, 1), Color.White, Vector3.UnitZ, new Vector3(20, 30, 10) + offset));

                indexes.Add(0);
                indexes.Add(1);
                indexes.Add(2);

                indexes.Add(2);
                indexes.Add(3);
                indexes.Add(0);
            }

            buffer.Elements = indexes.ToArray();
            buffer.Data = verts.ToArray();

            return buffer.Pack();
        }

        void DrawDebugRay()
        {
            World.CollisionInfo info = State.GameWorld.LineCollidesWithWorld(debugRayStart, State.GameWorld.Info.SunPosition);

            GL.LineWidth(3);
            GL.Begin(BeginMode.Lines);

            if (!info.Collided)
                GL.Color3(Color.Green);
            else
                GL.Color3(Color.Pink);

            GL.Vertex3(debugRayStart);
            GL.Vertex3(State.GameWorld.Info.SunPosition);
            GL.End();
            
            if (info.Collided)
            {
                GL.Begin(BeginMode.Lines);

                GL.Color3(Color.Red);
                GL.Vertex3(debugRayStart);
                Vector3 delta = State.GameWorld.Info.SunPosition - debugRayStart;
                delta.Normalize();
                GL.Vertex3(debugRayStart + (delta * info.Lenght));

                GL.Color3(Color.DarkMagenta);
                GL.Vertex3(debugRayStart);
                GL.Vertex3(debugRayStart + (delta * info.StartLen));

                GL.End();
                GL.Color3(Color.Tomato);
                GL.PushMatrix();
                GL.Translate(info.CollidedBlockPosition);
                DrawConerLines();
                GL.PopMatrix();
            }
            else
            {
                GL.Begin(BeginMode.Lines);

                Vector3 delta = State.GameWorld.Info.SunPosition - debugRayStart;
                delta.Normalize();

                GL.Color3(Color.DarkMagenta);
                GL.Vertex3(debugRayStart);
                GL.Vertex3(debugRayStart + (delta * info.StartLen));

                GL.End();
            }
   
            GL.Color4(Color.White);
        }

        void DrawAxisMarkers()
        {
            GL.PushMatrix();
            GL.LoadIdentity();

            Vector2 windowVec = new Vector2(Window.Width * -0.5f, Window.Height * -0.5f);
            windowVec.Normalize();

            float distance = 120;

            GL.Translate(windowVec.X * distance, windowVec.Y * distance, -250);

            // GL.Translate(50, 50, -50);

            // GL.Rotate(-90, 1.0f, 0.0f, 0.0f);
            GL.Rotate(camera.Tilt, 1.0f, 0.0f, 0.0f);
            GL.Rotate(-camera.Spin + 90.0, 0.0f, 1.0f, 0.0f);

            GL.LineWidth(2);

            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(30, 0, 0);
            GL.Vertex3(30, 0, 0);
            GL.Vertex3(25, 0, -5);

            GL.Vertex3(15, 10, 0);
            GL.Vertex3(20, 5, 0);

            GL.Vertex3(20, 10, 0);
            GL.Vertex3(15, 5, 0);

            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 30, 0);
            GL.Vertex3(0, 30, 0);
            GL.Vertex3(0, 25, -5);

            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -30);
            GL.Vertex3(0, 0, -30);
            GL.Vertex3(0, 5, -25);

            GL.Vertex3(0, 10, -15);
            GL.Vertex3(0, 7.5, -17.5);

            GL.Vertex3(0, 10, -20);
            GL.Vertex3(0, 7.5, -17.5);

            GL.Vertex3(0, 7.5, -17.5);
            GL.Vertex3(0, 5, -17.5);

            GL.End();


            GL.PopMatrix();
        }

        public static void DrawConerLines()
        {
            float size = 0.125f;

            GL.Begin(BeginMode.Lines);

            // lower
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(size, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, size, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, size);

            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1 - size, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, size, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, size);

            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1 - size, 1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1 - size, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1, size);

            GL.Vertex3(0, 1, 0);
            GL.Vertex3(size, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1 - size, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1, size);

            // upper
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(size, 0, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, size, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, 0, 1 - size);

            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1 - size, 0, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, size, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, 0, 1 - size);

            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1 - size, 1, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 1 - size, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 1, 1 - size);

            GL.Vertex3(0, 1, 1);
            GL.Vertex3(size, 1, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1 - size, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1, 1 - size);

            GL.End();
        }
    }
}

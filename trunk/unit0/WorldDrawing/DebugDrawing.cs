using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Renderer;

namespace WorldDrawing
{
    public class DebugDrawing
    {
        Size WindowSize = Size.Empty;

        public void Resize(int x, int y)
        {
            WindowSize = new Size(x, y);
        }

        public static void DrawAxisMarkerLines( float scale )
        {
            GL.LineWidth(2);

            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(30 * scale, 0, 0);
            GL.Vertex3(30 * scale, 0, 0);
            GL.Vertex3(25 * scale, 0, -5 * scale);

            GL.Vertex3(15 * scale, 10 * scale, 0);
            GL.Vertex3(20 * scale, 5 * scale, 0);

            GL.Vertex3(20 * scale, 10 * scale, 0);
            GL.Vertex3(15 * scale, 5 * scale, 0);

            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 30 * scale, 0);
            GL.Vertex3(0, 30 * scale, 0);
            GL.Vertex3(0, 25 * scale, -5 * scale);

            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -30 * scale);
            GL.Vertex3(0, 0, -30 * scale);
            GL.Vertex3(0, 5 * scale, -25 * scale);

            GL.Vertex3(0, 10 * scale, -15 * scale);
            GL.Vertex3(0, 7.5f * scale, -17.5f * scale);

            GL.Vertex3(0, 10 * scale, -20 * scale);
            GL.Vertex3(0, 7.5f * scale, -17.5f * scale);

            GL.Vertex3(0, 7.5f * scale, -17.5f * scale);
            GL.Vertex3(0, 5f * scale, -17.5f * scale);

            GL.End();
        }

        public void DrawAxisMarkers( Camera cam)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            GL.PushMatrix();
            GL.LoadIdentity();

            Vector2 windowVec = new Vector2(WindowSize.Width * -0.5f, WindowSize.Height * -0.5f);
            windowVec.Normalize();

            float distance = 120;

            GL.Translate(windowVec.X * distance, windowVec.Y * distance, -250);

            // GL.Translate(50, 50, -50);

            // GL.Rotate(-90, 1.0f, 0.0f, 0.0f);
            GL.Rotate(-cam.GetTilt(), 1.0f, 0.0f, 0.0f);
            GL.Rotate(-cam.GetSpin() + 90.0, 0.0f, 1.0f, 0.0f);

            DrawAxisMarkerLines(1f);

            GL.PopMatrix();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            GL.LineWidth(1);
        }
    }
}

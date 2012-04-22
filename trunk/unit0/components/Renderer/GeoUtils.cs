using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class GeoUtils
    {
        public static void Quad(Vector2 center, Vector2 bounds, float z, Color color)
        {
            float w = bounds.X / 2;
            float h = bounds.Y / 2;

            GL.Color4(color);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(Vector3.UnitZ);
            GL.TexCoord2(0, 0);
            GL.Vertex3(center.X - w, center.Y - h, z);
            GL.TexCoord2(1, 0);
            GL.Vertex3(center.X + w, center.Y - h, z);
            GL.TexCoord2(1, 1);
            GL.Vertex3(center.X + w, center.Y + h, z);
            GL.TexCoord2(0, 1);
            GL.Vertex3(center.X - w, center.Y + h, z);
            GL.End();
        }

        public static void Quad(Point center, Size bounds, float z, Color color)
        {
            Quad(new Vector2(center.X,center.Y),new Vector2(bounds.Width,bounds.Height), z, color);
        }

        public static void Quad(Point center, Size bounds, Color color)
        {
            GeoUtils.Quad(center, bounds,0, color);
        }

        public static void Quad(Point center, Size bounds)
        {
            GeoUtils.Quad(center, bounds, Color.White);
        }

        public static void Quad(Point min, Point max, Color color)
        {

        }

        public static void QuadXZ (Size bounds )
        {
            int w = bounds.Width / 2;
            int h = bounds.Height / 2;

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(Vector3.UnitZ);
            GL.TexCoord2(0, 0);
            GL.Vertex3( - w, 0 ,  - h);
            GL.TexCoord2(1, 0);
            GL.Vertex3(w, 0, - h);
            GL.TexCoord2(1, 1);
            GL.Vertex3(w, 0, h);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-w, 0,  h);
            GL.End();
        }

    }
}

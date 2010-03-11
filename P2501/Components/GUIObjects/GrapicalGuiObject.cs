using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GUIObjects
{
    public class GrapicalGuiObject : GUIObject
    {
        public static float ChildOffset = 0.01f;
        public override void Draw(double time)
        {
            Render(time);
            GL.PushMatrix();
            GL.Translate(0, 0, ChildOffset);

            foreach (GUIObject child in Children)
                child.Draw(time);
            GL.PopMatrix();
        }

        protected virtual void Render ( double time )
        {
        }
    }
}

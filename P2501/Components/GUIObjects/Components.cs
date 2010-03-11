using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Drawables.Textures;
using Utilities.Paths;

namespace GUIObjects
{
    public class Frame: GUIObject
    {
        string backgroundTexture = string.Empty;
        Texture BGTexture = null;

        public Frame()
        {
            Name = "Frame";
        }

        protected override void ReadExtraDefInfo(ElementDefinition def)
        {
            base.ReadExtraDefInfo(def);

            backgroundTexture = def.GetOptionValue("BackgroundTexture");
            if (backgroundTexture != string.Empty)
                BGTexture = TextureSystem.system.GetTexture(ResourceManager.FindFile(backgroundTexture));
        }

        protected override void WriteExtraDefInfo(ElementDefinition def)
        {
            base.WriteExtraDefInfo(def);
            def.SetOptionValue("BackgroundTexture", backgroundTexture);
        }

        protected override void Render(double time)
        {
            GL.Color4(BackgroundColor);
            if (BGTexture != null)
                BGTexture.Draw(size.Width, size.Height);
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Begin(BeginMode.Quads);

                GL.Vertex2(0, 0);
                GL.Vertex2(size.Width, 0);
                GL.Vertex2(size.Width, size.Height);
                GL.Vertex2(0, size.Height);

                GL.End();
            }
        }
    }

    public class GroupBox : GUIObject
    {
        public GroupBox()
        {
            Name = "GroupBox";
        }

        protected override void Render(double time)
        {
            base.Render(time);

            GL.Color4(ForegroundColor);

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(BeginMode.LineLoop);

            GL.Vertex2(0, 0);
            GL.Vertex2(size.Width, 0);
            GL.Vertex2(size.Width, size.Height);
            GL.Vertex2(0, size.Height);

            GL.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Textures;
using Renderer;
using FileLocations;

using OpenTK;
using OpenTK.Graphics;

namespace Client.Hud
{
    public class FrameRenderer : PannelRenderer
    {
        public string TextureName = string.Empty;
        protected Texture texture = null;

        protected Size gridSize = Size.Empty;

        protected const float aThird = 1.0f / 3.0f;

        public FrameRenderer(string name, string tex)
        {
            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);

            TextureName = tex;
            texture = Texture.Get(Locations.FindDataFile(tex), Texture.SmoothType.Nearest, true);
            if (texture == null)
                gridSize = Size.Empty;
            else
                gridSize = new Size(texture.Width / 3, texture.Height / 3);
        }

        public FrameRenderer(string tex)
        {
            if (!Pannels.ContainsKey(tex))
                Pannels.Add(tex, this);

            TextureName = tex;
            texture = Texture.Get(Locations.FindDataFile(tex), Texture.SmoothType.Nearest, true);
            if (texture == null)
                gridSize = Size.Empty;
            else
                gridSize = new Size(texture.Width / 3, texture.Height / 3);
        }

        protected Dictionary<Size, DisplayList> CachedSizes = new Dictionary<Size, DisplayList>();

        void MakeList(DisplayList list)
        {
            Size size = Size.Empty;
            try
            {
                if (list.Tag != null)
                    size = (Size)list.Tag;
            }
            catch (System.Exception /*ex*/)
            {
                return;
            }

            if (size == Size.Empty)
                return;

            GL.Normal3(0, 0, 1);
            GL.Begin(BeginMode.Quads);

            // upper left
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, size.Height);

            GL.TexCoord2(0, aThird);
            GL.Vertex2(0, size.Height - gridSize.Height);

            GL.TexCoord2(aThird, aThird);
            GL.Vertex2(gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(aThird, 0);
            GL.Vertex2(gridSize.Width, size.Height);


            // middle left
            GL.TexCoord2(0, aThird);
            GL.Vertex2(0, size.Height - gridSize.Height);

            GL.TexCoord2(0, aThird * 2);
            GL.Vertex2(0, gridSize.Height);

            GL.TexCoord2(aThird, aThird * 2);
            GL.Vertex2(gridSize.Width, gridSize.Height);

            GL.TexCoord2(aThird, aThird);
            GL.Vertex2(gridSize.Width, size.Height - gridSize.Height);

            // lower left
            GL.TexCoord2(0, aThird * 2);
            GL.Vertex2(0, gridSize.Height);

            GL.TexCoord2(0, 1);
            GL.Vertex2(0, 0);

            GL.TexCoord2(aThird, 1);
            GL.Vertex2(gridSize.Width, 0);

            GL.TexCoord2(aThird, aThird * 2);
            GL.Vertex2(gridSize.Width, gridSize.Height);


            // upper middle
            GL.TexCoord2(aThird, 0);
            GL.Vertex2(gridSize.Width, size.Height);

            GL.TexCoord2(aThird, aThird);
            GL.Vertex2(gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(aThird * 2, aThird);
            GL.Vertex2(size.Width - gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(aThird * 2, 0);
            GL.Vertex2(size.Width - gridSize.Width, size.Height);

            // middle
            GL.TexCoord2(aThird, aThird);
            GL.Vertex2(gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(aThird, aThird * 2);
            GL.Vertex2(gridSize.Width, gridSize.Height);

            GL.TexCoord2(aThird * 2, aThird * 2);
            GL.Vertex2(size.Width - gridSize.Width, gridSize.Height);

            GL.TexCoord2(aThird * 2, aThird);
            GL.Vertex2(size.Width - gridSize.Width, size.Height - gridSize.Height);

            // lower middle
            GL.TexCoord2(aThird, aThird * 2);
            GL.Vertex2(gridSize.Width, gridSize.Height);

            GL.TexCoord2(aThird, 1);
            GL.Vertex2(gridSize.Width, 0);

            GL.TexCoord2(aThird * 2, 1);
            GL.Vertex2(size.Width - gridSize.Width, 0);

            GL.TexCoord2(aThird * 2, aThird * 2);
            GL.Vertex2(size.Width - gridSize.Width, gridSize.Height);

            // upper right
            GL.TexCoord2(aThird * 2, 0);
            GL.Vertex2(size.Width - gridSize.Width, size.Height);

            GL.TexCoord2(aThird * 2, aThird);
            GL.Vertex2(size.Width - gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(1, aThird);
            GL.Vertex2(size.Width, size.Height - gridSize.Height);

            GL.TexCoord2(1, 0);
            GL.Vertex2(size.Width, size.Height);

            // middle right
            GL.TexCoord2(aThird * 2, aThird);
            GL.Vertex2(size.Width - gridSize.Width, size.Height - gridSize.Height);

            GL.TexCoord2(aThird * 2, aThird * 2);
            GL.Vertex2(size.Width - gridSize.Width, gridSize.Height);

            GL.TexCoord2(1, aThird * 2);
            GL.Vertex2(size.Width, gridSize.Height);

            GL.TexCoord2(1, aThird);
            GL.Vertex2(size.Width, size.Height - gridSize.Height);

            // lower right
            GL.TexCoord2(aThird * 2, aThird * 2);
            GL.Vertex2(size.Width - gridSize.Width, gridSize.Height);

            GL.TexCoord2(aThird * 2, 1);
            GL.Vertex2(size.Width - gridSize.Width, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex2(size.Width, 0);

            GL.TexCoord2(1, aThird * 2);
            GL.Vertex2(size.Width, gridSize.Height);

            GL.End();
        }

        public override void DrawElement(PannelElement element, double now, double frameTime)
        {
            if (texture == null)
                return;

            Vector2 o = element.GetDrawOrigin();

            GL.Translate(o.X, o.Y, 0);
            GL.Color4(element.color.red, element.color.green, element.color.blue, element.alpha);

            texture.Bind();
            if (CachedSizes.ContainsKey(element.size))
                CachedSizes[element.size].Call();
            else
            {
                DisplayList list = new DisplayList(MakeList);
                list.Tag = element.size;
                CachedSizes.Add(GetElementSize(element), list);
                list.Call();
            }

            base.DrawElement(element, now, frameTime);
        }
    }

    public class ImagePannelRenderer : PannelRenderer
    {
        public ImagePannelRenderer(string name)
        {
            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public override Size GetElementSize(PannelElement element)
        {
            if (element.text == string.Empty)
                return element.size;

            Texture texture = element.RenderTag as Texture;
            if (texture == null)
            {
                texture = Texture.Get(Locations.FindDataFile(element.text));
                element.RenderTag = texture;
            }

            if (texture == null)
                return element.size;

            int w = texture.Width;
            int h = texture.Height;
            if (element.size != Size.Empty)
            {
                w = element.size.Width;
                h = element.size.Height;
            }

            return new Size(w, h);
        }

        public override void DrawElement(PannelElement element, double now, double frameTime)
        {
            if (element.text == string.Empty)
                return;

            Texture texture = element.RenderTag as Texture;
            if (texture == null)
            {
                texture = Texture.Get(Locations.FindDataFile(element.text));
                element.RenderTag = texture;
            }

            if (texture == null)
                return;

            int w = texture.Width;
            int h = texture.Height;
            if (element.size != Size.Empty)
            {
                w = element.size.Width;
                h = element.size.Height;
            }

            Vector2 o = element.GetDrawOrigin();

            GL.Translate(o.X, o.Y, 0);
            GL.Color4(element.color.red, element.color.green, element.color.blue, element.alpha);

            texture.Bind();
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(0, h);

            GL.TexCoord2(0, 1);
            GL.Vertex2(0, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex2(w, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex2(w, h);

            GL.End();

            base.DrawElement(element, now, frameTime);
        }
    }

    public class SizeableChatBoxFrame : PannelRenderer
    {
        Texture Background = null;

        public SizeableChatBoxFrame(string name, string textureName)
        {
            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);

            Background = Texture.Get(Locations.FindDataFile(textureName), Texture.SmoothType.Nearest, true);
        }

        public SizeableChatBoxFrame(string name)
        {
            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);

            Background = Texture.Get(Locations.FindDataFile(name), Texture.SmoothType.Nearest, true);
        }

        public override void DrawElement(PannelElement element, double now, double frameTime)
        {
            float thirdHeight = element.size.Height / 3.0f;
            float actualWidht = Background.Width;
            if (element.size.Width < actualWidht)
                actualWidht = element.size.Width;

            float imageQuaterHeight = Background.Height / 4.0f;

            float textureU = actualWidht/Background.Width;

            Vector2 o = element.GetDrawOrigin();

            GL.Translate(o.X, o.Y, 0);
            GL.Color4(element.color.red, element.color.green, element.color.blue, element.alpha);

            Background.Bind();

            GL.Begin(BeginMode.Quads);

            float bottomHeight = imageQuaterHeight;
            float topHeight = imageQuaterHeight;

            float middleHeight = 0;
            if (bottomHeight + topHeight < element.size.Height)
                middleHeight = element.size.Height - (bottomHeight + topHeight);
            else if (bottomHeight + topHeight != element.size.Height)
                topHeight = bottomHeight = element.size.Height / 2;

            // do the bottom
            float bottomV = 1.0f - (bottomHeight/Background.Height);

            GL.TexCoord2(0, bottomV);
            GL.Vertex2(0, bottomHeight);

            GL.TexCoord2(0, 1);
            GL.Vertex2(0, 0);

            GL.TexCoord2(textureU, 1);
            GL.Vertex2(actualWidht, 0);

            GL.TexCoord2(textureU, bottomV);
            GL.Vertex2(actualWidht, bottomHeight);

            if(middleHeight > 0)
            {
                GL.TexCoord2(0, 0.5f);
                GL.Vertex2(0, bottomHeight + middleHeight);

                GL.TexCoord2(0, 0.75f);
                GL.Vertex2(0, bottomHeight);

                GL.TexCoord2(textureU, 0.75f);
                GL.Vertex2(actualWidht, bottomHeight);

                GL.TexCoord2(textureU, 0.5f);
                GL.Vertex2(actualWidht, bottomHeight + middleHeight);
            }

            float TopV = 0.25f + (topHeight/Background.Height);

            // else
            GL.TexCoord2(0, 0.25f);
            GL.Vertex2(0, bottomHeight + middleHeight + topHeight);

            GL.TexCoord2(0, TopV);
            GL.Vertex2(0, bottomHeight + middleHeight);

            GL.TexCoord2(textureU, TopV);
            GL.Vertex2(actualWidht, bottomHeight + middleHeight);

            GL.TexCoord2(textureU, 0.25f);
            GL.Vertex2(actualWidht, bottomHeight + middleHeight + topHeight);

            GL.End();

            base.DrawElement(element, now, frameTime);
        }
    }
}

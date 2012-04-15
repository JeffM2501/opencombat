using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using Renderer;
using FileLocations;
using Textures;

namespace Client.Hud
{
    public class TextLabelRenderer : PannelRenderer
    {
        Font font = null;

        public TextLabelRenderer(Font _font, string name)
        {
            font = _font;

            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public override void ElementAdded(PannelElement element)
        {
            HudRenderer.AddTextItem(new HudRenderer.TextItem(this, element, false));
        }

        public override void ElementRemoved(PannelElement element)
        {
            HudRenderer.RemoveTextItem(element);
        }

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            if (!element.focus && element.text == string.Empty)
                return;

            RectangleF rect = printer.Measure(element.text, font).BoundingBox;
            float yOffset = rect.Height;

            Size s = element.size;
            if (element.size == Size.Empty)
                s = new Size((int)rect.Width, (int)rect.Height);

            Vector2 pos = element.GetWorldPos(s);

            TextPrinterOptions options = TextPrinterOptions.Default;
            TextAlignment alignment = TextAlignment.Near;
            if (element.origin == PannelElement.Alignmnet.Centered)
                alignment = TextAlignment.Center;
            if (element.origin == PannelElement.Alignmnet.RightBottom || element.origin == PannelElement.Alignmnet.RightCenter || element.origin == PannelElement.Alignmnet.RightTop)
                alignment = TextAlignment.Far;

            if (element.text != string.Empty)
                printer.Print(element.text, font, element.Color, new RectangleF(pos.X, view.Bounds.Height - pos.Y - yOffset, s.Width, s.Height), options, alignment);
        }
    }

    public class TextEditRenderer : PannelRenderer
    {
        Font font = null;

        public static double CursorBlinkRate = 0.5;

        public class CursorData
        {
            public double lastBlinkTime = -1;
            public bool lastBlinkState = true;
        }

        public TextEditRenderer(Font _font, string name)
        {
            font = _font;

            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public override void ElementAdded(PannelElement element)
        {
            HudRenderer.AddTextItem(new HudRenderer.TextItem(this, element, false));
        }

        public override void ElementRemoved(PannelElement element)
        {
            HudRenderer.RemoveTextItem(element);
        }

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            if (!element.focus && element.text == string.Empty)
                return;

            RectangleF rect = printer.Measure(element.text, font).BoundingBox;
            float yOffset = rect.Height;

            Size s = element.size;
            if (element.size == Size.Empty)
                s = new Size((int)rect.Width, (int)rect.Height);

            Vector2 pos = element.GetWorldPos(s);

            TextPrinterOptions options = TextPrinterOptions.Default;
            TextAlignment alignment = TextAlignment.Near;
            if (element.origin == PannelElement.Alignmnet.Centered)
                alignment = TextAlignment.Center;
            if (element.origin == PannelElement.Alignmnet.RightBottom || element.origin == PannelElement.Alignmnet.RightCenter || element.origin == PannelElement.Alignmnet.RightTop)
                alignment = TextAlignment.Far;

            if (element.text != string.Empty)
                printer.Print(element.text, font, element.Color, new RectangleF(pos.X, view.Bounds.Height - pos.Y - yOffset, s.Width, s.Height), options, alignment);

            // check and see if we have a cursor to draw

            if (element.focus && element.textCursorOffset >= 0)
            {
                CursorData data = element.RenderTag as CursorData;
                if (data == null)
                    data = new CursorData();

                if (data.lastBlinkTime < 0)
                    data.lastBlinkTime = now;
                else if (data.lastBlinkTime + CursorBlinkRate < now)
                {
                    data.lastBlinkState = !data.lastBlinkState;
                    data.lastBlinkTime = now;
                }

                if (data.lastBlinkState)
                {
                    float offset = 0;
                    if (element.textCursorOffset > 0)
                    {
                        string tmp = element.text.Substring(0, element.textCursorOffset);

                        offset = printer.Measure(element.text, font).BoundingBox.Width;

                        GL.DepthFunc(DepthFunction.Always);
                        GL.LineWidth(2);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Begin(BeginMode.Lines);
                        GL.Color4(element.Color);
                        GL.Vertex3(pos.X + offset, view.Bounds.Height - pos.Y, 0.5f);
                        GL.Vertex3(pos.X + offset, view.Bounds.Height - pos.Y - yOffset, 0.5f);
                        GL.End();
                        GL.Enable(EnableCap.Texture2D);
                        GL.LineWidth(1);
                        GL.DepthFunc(DepthFunction.Lequal);
                    }
                }
            }
        }
    }

    public class ChatPannelRenderer : PannelRenderer
    {
        Font font = null;
        Color fromColor = Color.Blue;
        Color serverColor = Color.Red;

        public class ChatLines
        {
            public ChatMessage[] messages = null;
        }

        public ChatPannelRenderer(Font _font, Color _from, Color _server, string name)
        {
            font = _font;
            fromColor = _from;
            serverColor = _server;

            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public override void ElementAdded(PannelElement element)
        {
            HudRenderer.AddTextItem(new HudRenderer.TextItem(this, element, false));
        }

        public override void ElementRemoved(PannelElement element)
        {
            HudRenderer.RemoveTextItem(element);
        }

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            ChatLines lines = element.RenderTag as ChatLines;
            if (lines == null)
                return;

            int lineHeight = (int)(printer.Measure("X", font).BoundingBox.Height + 6);

            Size size = element.GetElementSize();
            int lineCount = size.Height / lineHeight;

            if (lineCount > lines.messages.Length)
                lineCount = lines.messages.Length;

            Vector2 pos = element.GetWorldPos();

            for (int i = 0; i < lineCount; i++)
            {
                ChatMessage msg = lines.messages[lines.messages.Length - i - 1];

                if (msg.chat != string.Empty)
                {
                    int yoffset = i * lineHeight;
                    if (msg.from != string.Empty)
                    {
                        int xoffset = (int)(printer.Measure(msg.from, font).BoundingBox.Width + 0.5);

                        printer.Print(msg.from, font, fromColor, new RectangleF(pos.X, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, xoffset, lineHeight));
                        printer.Print(msg.chat, font, element.Color, new RectangleF(pos.X + xoffset, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, size.Width - xoffset, lineHeight));
                    }
                    else
                        printer.Print(msg.chat, font, serverColor, new RectangleF(pos.X, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, size.Width, lineHeight));
                }
            }
        }
    }

    public class PlayerListPannelRenderer : PannelRenderer
    {
        Font font = null;
        Color fromColor = Color.Blue;
        Color serverColor = Color.Red;

        public class PlayerList
        {
            public class Item
            {
                public string name = string.Empty;

                public Item(string n)
                {
                    name = n;
                }
            }
            public List<Item> items = new List<Item>();
        }

        public PlayerListPannelRenderer(Font _font, Color _from, Color _server, string name)
        {
            font = _font;
            fromColor = _from;
            serverColor = _server;

            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public override void ElementAdded(PannelElement element)
        {
            HudRenderer.AddTextItem(new HudRenderer.TextItem(this, element, false));
        }

        public override void ElementRemoved(PannelElement element)
        {
            HudRenderer.RemoveTextItem(element);
        }

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            PlayerList list = element.RenderTag as PlayerList;
            if (list == null)
                return;

            lock (list)
            {
                int lineHeight = (int)(printer.Measure("X", font).BoundingBox.Height + 6);

                Size size = element.GetElementSize();
                int lineCount = size.Height / lineHeight;

                if (lineCount > list.items.Count)
                    lineCount = list.items.Count;

                Vector2 pos = element.GetWorldPos();

                for (int i = 0; i < lineCount; i++)
                {
                    PlayerList.Item item = list.items[i];

                    if (item.name != string.Empty)
                    {
                        int yoffset = i * lineHeight;

                        printer.Print(item.name, font, element.Color, new RectangleF(pos.X, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, 0, lineHeight));
                    }
                }
            }
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
        }
    }

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
        }
    }
}

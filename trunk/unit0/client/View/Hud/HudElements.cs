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

  
}

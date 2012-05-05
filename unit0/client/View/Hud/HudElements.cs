using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
// use compatibility for TextPrinter
using OpenTK.Graphics;
#pragma warning disable 618 , 612

using Renderer;
using FileLocations;
using Textures;

namespace Client.Hud
{
    public class ChatPannelRenderer : PannelRenderer
    {
        Font font = null;
        Color fromColor = Color.Aqua;
        Color serverColor = Color.Maroon;
        Color ErrorColor = Color.Red;

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

        protected float IconWidth = 24;
        protected Size IconSize = new Size(24, 24);

        public override void DrawElement(PannelElement element, double now, double frameTime)
        {
            ChatInfo info = element.RenderTag as ChatInfo;
            if (info == null || LineHeight < 1)
                return;

            Size size = element.GetElementSize();
            int lineCount = size.Height / LineHeight;

            Vector2 pos = element.GetDrawOrigin();
            int yoffset = 0;

            for (int i = 0; i < lineCount; i++)
            {
                ChatInfo.ChatMessage msg = info.GetRecentMessage(i);

                if (msg == null || yoffset >= (element.size.Height + (IconWidth/2.0)))
                    break;

                if (msg.Text != string.Empty)
                {
                    ChatInfo.ChatUser user = info.GetUser(msg.From);

                    Texture icon = info.ServerAvatar;
                    if (user != ChatInfo.ChatUser.Empty && user.Icon != null)
                        icon = user.Icon;

                    GL.PushMatrix();
                   // GL.Translate(pos.X, pos.Y, 0.01f);
                    GL.Translate(pos.X + 1 + (IconWidth / 2.0f), pos.Y + yoffset + (IconWidth / 1.75f), 0.5f);
                    GL.Color4(1.0, 1.0, 1.0, 1.0);

                    icon.Draw(IconSize);
                    yoffset = i * LineHeight;
                    GL.PopMatrix();
                }
            }


            base.DrawElement(element, now, frameTime);
        }

        int LineHeight = -1;

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            ChatInfo info = element.RenderTag as ChatInfo;
            if (info == null)
                return;

            if (LineHeight == -1)
                LineHeight = (int)(printer.Measure("X", font).BoundingBox.Height + 6);

            Size size = element.GetElementSize();
            int lineCount = size.Height / LineHeight;

            Vector2 pos = element.GetWorldPos();
            int yoffset = 0;

            pos.X += IconWidth + 2;

            for (int i = 0; i < lineCount; i++)
            {
                ChatInfo.ChatMessage msg = info.GetRecentMessage(i);

                if (msg == null || yoffset >= element.size.Height)
                    break;

                if (msg.Text != string.Empty)
                {
                    string fromName = info.GetUser(msg.From).Name;
                   
                    Color nameColor = fromColor;
                    if (msg.From == ChatInfo.ErrorChatUID)
                        nameColor = ErrorColor;
                    else if (msg.From == ChatInfo.GameChatUID || fromName == string.Empty)
                        nameColor = serverColor;

                    if (fromName == string.Empty)
                        fromName = "Server";

                    int xoffset = (int)(printer.Measure(fromName, font).BoundingBox.Width + 2.5);
                    yoffset = i * LineHeight;

                    RectangleF fromRect = new RectangleF(pos.X, view.Bounds.Height - (pos.Y + yoffset) - LineHeight, xoffset, LineHeight);
                    printer.Print(fromName, font, nameColor, fromRect,TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);

                    RectangleF msgRect = new RectangleF(pos.X + xoffset, view.Bounds.Height - (pos.Y + yoffset) - LineHeight, size.Width - xoffset, LineHeight);
                    printer.Print(msg.Text, font, element.Color, msgRect);
                 }
            }
        }
    }

    public class PlayerListPannelRenderer : PannelRenderer
    {
        Font font = null;
       // Color fromColor = Color.Blue;
       // Color serverColor = Color.Red;

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
            //fromColor = _from;
           // serverColor = _server;

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

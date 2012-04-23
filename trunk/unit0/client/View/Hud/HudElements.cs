﻿using System;
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

        public override void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
            ChatInfo info = element.RenderTag as ChatInfo;
            if (info == null)
                return;

            int lineHeight = (int)(printer.Measure("X", font).BoundingBox.Height + 6);

            Size size = element.GetElementSize();
            int lineCount = size.Height / lineHeight;

            Vector2 pos = element.GetWorldPos();
            int yoffset = 0;

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
                    yoffset = i * lineHeight;
    
                    RectangleF fromRect = new RectangleF(pos.X, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, xoffset, lineHeight);
                    printer.Print(fromName, font, nameColor, fromRect,TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);

                    RectangleF msgRect = new RectangleF(pos.X + xoffset, view.Bounds.Height - (pos.Y + yoffset) - lineHeight, size.Width - xoffset, lineHeight);
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

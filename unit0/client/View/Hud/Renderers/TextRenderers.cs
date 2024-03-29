﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Textures;
using Renderer;
using FileLocations;

using OpenTK;
// use compatibility for TextRenderer
using OpenTK.Graphics;
#pragma warning disable 618 , 612

namespace Client.Hud
{
    public class TextLabelRenderer : PannelRenderer
    {
        Font font = null;

        public override void Register(ElementRenderDef def, FontsCache cache)
        {
            if (def.Paramaters.Count > 0)
                font = cache.Get(def.Paramaters[0]);

            if (!Pannels.ContainsKey(def.Name))
                Pannels.Add(def.Name, this);
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
            if (element.text == string.Empty)
                return;

            // we add a little buffer to the width just to ensure the last character prints.
            RectangleF rect = printer.Measure(element.text + "X", font).BoundingBox;
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

        public override void Register(ElementRenderDef def, FontsCache cache)
        {
           if (def.Paramaters.Count > 0)
                font = cache.Get(def.Paramaters[0]);

            if (!Pannels.ContainsKey(def.Name))
                Pannels.Add(def.Name, this);
        }

        public override void ElementAdded(PannelElement element)
        {
            HudRenderer.AddTextItem(new HudRenderer.TextItem(this, element, false));
        }

        public override void ElementRemoved(PannelElement element)
        {
            HudRenderer.RemoveTextItem(element);
        }

        public override void DrawElement(PannelElement element, double now, double frameTime)
        {
            if (element.focus)
            {
                GL.Disable(EnableCap.Texture2D);

                GL.PushMatrix();
                Vector2 o = element.GetDrawOrigin();

                GL.Translate(o.X, o.Y, 0);

                GL.Color4(element.Color);
                GL.Begin(BeginMode.LineLoop);

                GL.Vertex2(0, 0);
                GL.Vertex2(element.size.Width, 0);
                GL.Vertex2(element.size.Width, element.size.Height);
                GL.Vertex2(0, element.size.Height);

                GL.End();
                GL.PopMatrix();
                GL.Enable(EnableCap.Texture2D);
            }
            base.DrawElement(element, now, frameTime);
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
            pos.X += 2; // just a buffer to make it look better

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
                      //  string tmp = element.text.Substring(0, element.textCursorOffset);

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

}

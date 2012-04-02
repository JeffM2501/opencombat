/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;

using Drawables.Textures;
using Utilities.Paths;

namespace GUIObjects
{
    public class Frame: GUIObject
    {
        string backgroundTexture = string.Empty;
        Texture BGTexture = null;

        bool repeats = false;

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

            string rep = def.GetOptionValue("RepeatTexture");
            if (rep != string.Empty && (rep != "0" || rep != "False"))
                repeats = true;
        }

        protected override void WriteExtraDefInfo(ElementDefinition def)
        {
            base.WriteExtraDefInfo(def);
            def.SetOptionValue("BackgroundTexture", backgroundTexture);
            def.SetOptionValue("RepeatTexture", repeats.ToString());
        }

        protected override void Render(double time)
        {
            GL.Color4(BackgroundColor);
            if (BGTexture != null)
            {
                if (repeats)
                    BGTexture.Draw(size.Width, size.Height,1);
                else
                    BGTexture.Draw(size.Width, size.Height);
            }
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
        public float TextSize = -1;
        public float TextHeight = -1;

        public GroupBox()
        {
            Name = "GroupBox";
        }

        protected override void ReadExtraDefInfo(GUIObject.ElementDefinition def)
        {
            base.ReadExtraDefInfo(def);

            if (ValueName != string.Empty)
            {
                TextExtents extents = GUIObjectManager.Printer.Measure(ValueName, GUIObject.GUIFont);
                TextSize = (float)extents.BoundingBox.Width;
                TextHeight = (float)extents.BoundingBox.Height;
            }
        }

        protected override void Render(double time)
        {
            base.Render(time);

            GL.Color4(ForegroundColor);

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(BeginMode.Lines);

            GL.Vertex2(0, 0);
            GL.Vertex2(size.Width, 0);
           
            GL.Vertex2(size.Width, 0);
            GL.Vertex2(size.Width, size.Height);
           
            GL.Vertex2(0, 0);
            GL.Vertex2(0, size.Height);

            if (ValueName != string.Empty)
            {
                if (TextSize < size.Width-4)
                {
                    GL.Vertex2(size.Width, size.Height);
                    GL.Vertex2(TextSize+4, size.Height);
                }
            }
            else
            {
                GL.Vertex2(size.Width, size.Height);
                GL.Vertex2(0, size.Height);
            }

            GL.End();

            GUIObjectManager.Printer.Begin();
            GUIObjectManager.Printer.Print(ValueName, GUIObject.GUIFont, ForegroundColor, GetTextRect(2, size.Height + TextHeight / 2, TextSize + 2));
            GUIObjectManager.Printer.End();
        }
    }

    public class Label : GUIObject
    {
        public float TextSize = -1;
        public float TextHeight = -1;

        public Label()
        {
            Name = "Label";
        }

        protected override void ReadExtraDefInfo(GUIObject.ElementDefinition def)
        {
            base.ReadExtraDefInfo(def);

            if (ValueName != string.Empty)
            {
                TextExtents extents = GUIObjectManager.Printer.Measure(ValueName, GUIObject.GUIFont);
                TextSize = (float)extents.BoundingBox.Width;
                TextHeight = (float)extents.BoundingBox.Height;
            }
        }
      
        protected override void Render(double time)
        {
            base.Render(time);

            GUIObjectManager.Printer.Begin();
            GUIObjectManager.Printer.Print(ValueName, GUIObject.GUIFont, ForegroundColor, GetTextRect(0, TextHeight, TextSize+2));
            GUIObjectManager.Printer.End();
        }
    }

    public class ValueLabel : GUIObject
    {
        public float TextSize = -1;
        public float TextHeight = -1;

        public ValueLabel()
        {
            Name = "ValueLabel";
        }

        protected override void ReadExtraDefInfo(GUIObject.ElementDefinition def)
        {
            base.ReadExtraDefInfo(def);
        }

        protected override void Render(double time)
        {
            base.Render(time);
            if (Value != null && Value.Value != string.Empty)
            {
                TextExtents extents = GUIObjectManager.Printer.Measure(Value.Value, GUIObject.GUIFont);
                TextSize = (float)extents.BoundingBox.Width;
                TextHeight = (float)extents.BoundingBox.Height;
               
                GUIObjectManager.Printer.Begin();
                GUIObjectManager.Printer.Print(Value.Value, GUIObject.GUIFont, ForegroundColor, GetTextRect(0, TextHeight, TextSize + 2));
                GUIObjectManager.Printer.End();
            }
        }
    }

    public class Picture : GUIObject
    {
        string image = string.Empty;
        Texture imageTexture = null;

        bool clamp = false;

        public Picture()
        {
            Name = "Picture";
        }

        protected override void ReadExtraDefInfo(ElementDefinition def)
        {
            base.ReadExtraDefInfo(def);

            image = def.GetOptionValue("Image");
            if (image != string.Empty)
                imageTexture = TextureSystem.system.GetTexture(ResourceManager.FindFile(image));

            string c = def.GetOptionValue("ClampImage");
            if (c != string.Empty && (c != "0" && c != "False"))
                clamp = true;

            if (imageTexture != null && clamp)
            {
                if (imageTexture.Width < size.Width)
                    size.Width = imageTexture.Width;
                if (imageTexture.Height < size.Height)
                    size.Height = imageTexture.Height;
            }
        }

        protected override void WriteExtraDefInfo(ElementDefinition def)
        {
            base.WriteExtraDefInfo(def);
            def.SetOptionValue("Image", image);
            def.SetOptionValue("ClampImage", clamp.ToString());
        }

        protected override void Render(double time)
        {
            GL.Color4(BackgroundColor);
            if (imageTexture == null)
                return;

            if (clamp)
                imageTexture.Draw(size.Width, size.Height, 1);
            else
                imageTexture.Draw(size.Width, size.Height);
        }
    }
}

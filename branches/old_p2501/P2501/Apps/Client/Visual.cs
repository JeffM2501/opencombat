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
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Simulation;
using Drawables.Cameras;

namespace P2501Client
{
    class Visual : GameWindow
    {
        public delegate void UpdateEventHandler (Visual sender, double time);

        public UpdateEventHandler Update;

        protected Sim simSate = null;

        public Sim GameState = null;

        protected Player ThisPlayer = null;

        public Player ThePlayer
        {
            get { return ThisPlayer; }
            set
            {
                ThisPlayer = value;
            }
        }

        protected Camera camera;

        public HUDRenderer HUD; 

        public Visual (int width, int height, GraphicsMode mode, GameWindowFlags options) : base(width,height,mode,"Projekt2501",options)
        {
            HUD = new HUDRenderer(this);
            camera = new Camera();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            if (camera != null)
                camera.Resize(Width, Height);
        }

        public void SetPlayer (Player player)
        {
            ThisPlayer = player;
            HUD.ThePlayer = player;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Update != null)
                Update(this, e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);

            if (GameState != null)
            { 

                // set ortho
                camera.SetOrthographic();
                HUD.Update(); 
            }

            SwapBuffers();
        }
    }
}

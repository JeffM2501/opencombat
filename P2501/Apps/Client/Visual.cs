using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;

namespace P2501Client
{
    class Visual : GameWindow
    {
        public delegate void UpdateEventHandler (Visual sender, double time);

        public UpdateEventHandler Update;

        public Visual (int width, int height, GraphicsMode mode, GameWindowFlags options) : base(width,height,mode,"Projekt2501",options)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Update != null)
                Update(this, e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }
    }
}

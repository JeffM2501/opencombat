using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using GridWorld;
using Game;
using Renderer;

using WorldDrawing;

namespace Client
{
    public class HUD
    {
        public View TheView = null;


        public HUD(View view)
        {
            TheView = view;
        }

        public void Resize()
        {

        }

        public void Draw( FrustumCamera camera )
        {

        }

        public void StatusChange ( View.ViewStatus status)
        {
            if (status == View.ViewStatus.Playing)
                GL.ClearColor(Color.LightSkyBlue);
            else
                GL.ClearColor(Color.Black);
        }
    }
}
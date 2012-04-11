using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;

using GridWorld;
using Game;
using Renderer;
using Textures;
using FileLocations;
using Textures;

namespace Client
{
    public class HUD
    {
        public View TheView = null;

        Font WaitFont = new Font(FontFamily.GenericSansSerif, 72);
        public TextPrinter TextPrinter = new TextPrinter(TextQuality.High);

        protected Stopwatch Clock = new Stopwatch();

        Texture Spinner = null;

        public HUD(View view)
        {
            TheView = view;
            
        }

        public void Init()
        {
            Spinner = Texture.Get(Locations.FindDataFile("ui/wait_ring.png"));
            Clock.Start();
        }

        public void Resize()
        {

        }

        protected bool ShowWait = false;

        protected double Now = 0;
        protected double Delta = 0;
        public void Draw( FrustumCamera camera )
        {
            Delta = Now;
            Now = Clock.ElapsedMilliseconds * 0.001;
            Delta = Now - Delta;

            camera.SetOrthographic();
            GL.Disable(EnableCap.Lighting);
            GL.Color4(1, 1, 1, 1);

            if (ShowWait)
                DrawWaitScreen();
        }

        double lastDotTime = -1;
        int dots = 0;

        float lastWaitAngle = 0;

        protected void DrawWaitScreen()
        {
            GL.Disable(EnableCap.Texture2D);

            double DotTime = 1;

            if (Now - lastDotTime > DotTime)
            {
                dots++;
                if (dots > 4)
                    dots = 0;

                lastDotTime = Now;
            }

            string d = "";
            for (int i = 0; i < dots; i++)
                d += ".";

            TextPrinter.Begin();
            SizeF s = TextPrinter.Measure("Connecting", WaitFont).BoundingBox.Size;

            float buffer = TheView.Window.Width - s.Width;
            RectangleF r = new RectangleF(buffer / 2, (TheView.Window.Height / 2) - (s.Height / 2), TheView.Window.Width - (buffer / 2), s.Height);

            TextPrinter.Print("Connecting" + d, WaitFont, Color.White,r, TextPrinterOptions.Default,TextAlignment.Near);
            TextPrinter.End();

            GL.Enable(EnableCap.Texture2D);

            float spinspeed = 120;
            lastWaitAngle += spinspeed * (float)Delta;

            Spinner.Bind();
            GL.PushMatrix();
            GL.Translate(buffer / 4, (TheView.Window.Height / 2), -1);
            GL.Rotate(lastWaitAngle, Vector3.UnitZ);

            GeoUtils.Quad(Vector2.Zero, new Vector2(Spinner.Width, Spinner.Height), 0, Color.White);
            GL.PopMatrix();
        }

        public void StatusChange ( View.ViewStatus status)
        {
            ShowWait = status != View.ViewStatus.Playing;

            if (status == View.ViewStatus.Playing)
                GL.ClearColor(Color.LightSkyBlue);
            else
                GL.ClearColor(Color.Black);
        }
    }
}
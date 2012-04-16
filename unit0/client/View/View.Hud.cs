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

using Client.Hud;

namespace Client
{
    public class HUD
    {
        public View TheView = null;

        Font WaitFont = new Font(FontFamily.GenericSansSerif, 72);
        Font WaitInfoFont = new Font(FontFamily.GenericSansSerif, 14);
        public TextPrinter TextPrinter = new TextPrinter(TextQuality.High);

        protected HudRenderer GUIRenderer;

        protected Stopwatch Clock = new Stopwatch();

        Texture[] Spinners = null;

        public string StateMessage = string.Empty;

        protected string WaitHeader = "Processing";

        public HUD(View view)
        {
            TheView = view;
        }

        public void Init()
        {
            Spinners = new Texture[2];
            Spinners[0] = Texture.Get(Locations.FindDataFile("ui/outer_spinner.png"), Texture.SmoothType.SmoothMip, false);
            Spinners[1] = Texture.Get(Locations.FindDataFile("ui/inner_spinner.png"), Texture.SmoothType.SmoothMip, false);

            GUIRenderer = new HudRenderer(new ViewBounds(TheView.Window.Size));

            Clock.Start();
        }

        public void Resize()
        {
            GUIRenderer.WindowBounds.Bounds = TheView.Window.Size;
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
            else
                GUIRenderer.Render(Now, Delta);
        }

        double lastDotTime = -1;
        int dots = 0;

        float lastWaitAngle = 0;

        protected class Swoosh
        {
            public Vector2 position = Vector2.Zero;
            public float speed = 1;
            public Vector2 size = Vector2.Zero;
            public float alpha = 1;

            public static int BaseSwooshDensity = 1;
            public static int SwooshDensityVariant = 5;
            public static int SwooshDensity = 0;

            public static int SwooshBaseHeight = 10;
            public static int SwooshHeightVariant = 50;

            public static int SwooshBaseLenght = 200;
            public static int SwooshLenghtVariant = 1000;

            public static float SwooshBaseAlpha = 0.001f;
            public static float SwooshAlphaVariant = 0.15f;

            public static float SwooshBaseSpeed = 200.0f;
            public static float SwooshSpeedVariant = 2050.0f;

            public static int SwooshCenterSpread = 200;

            public static int SwooshAngleOffset = 50;

            public Swoosh()
            {
                size = NewSize();
                alpha = NewAlpha();
                speed = NewSpeed();
            }

            public void Draw( float z)
            {
                GL.PushMatrix();
                GL.Color4(1,1,1,alpha);
                GL.Translate(position.X,position.Y,z);
                GL.Normal3(Vector3.UnitZ);

                GL.Begin(BeginMode.Quads);
                GL.Vertex2(SwooshAngleOffset,0);
                GL.Vertex2(size.X,0);
                GL.Vertex2(size.X-SwooshAngleOffset,size.Y);
                GL.Vertex2(0,size.Y);
                GL.End();
                GL.PopMatrix();
            }

            public static Vector2 NewSize()
            {
                Random rand = new Random();

                return new Vector2(SwooshBaseLenght + rand.Next(SwooshLenghtVariant), SwooshBaseHeight + rand.Next(SwooshHeightVariant));
            }

            public static float NewAlpha()
            {
                Random rand = new Random();

                return (float)(SwooshBaseAlpha + (rand.NextDouble()*SwooshAlphaVariant));
            }

            public static float NewSpeed()
            {
                Random rand = new Random();

                return (float)(SwooshBaseSpeed + (rand.NextDouble()*SwooshSpeedVariant));
            }

            public static void NewDensity()
            {
                Swoosh.SwooshDensity = Swoosh.BaseSwooshDensity + new Random().Next(Swoosh.SwooshDensityVariant);
            }
        }

        protected List<Swoosh>[] Swooshes = new List<Swoosh>[2]{new List<Swoosh>(),new List<Swoosh>()};

        public float NewSwooshY()
        {
            Random rand = new Random();

            int offset = rand.Next(Swoosh.SwooshCenterSpread * 2) - Swoosh.SwooshCenterSpread;

            return (TheView.Window.Height/3.5f) + offset;
        }
       
        protected void InitSwoshes()
        {
            if (Swooshes[0].Count + Swooshes[1].Count > 0)
                return;

            Random rand = new Random();

            Swoosh.NewDensity();
            for (int i = 0; i < Swoosh.SwooshDensity; i++)
            {
                Swoosh swoosh = new Swoosh();
                swoosh.position = new Vector2((float)rand.NextDouble() * TheView.Window.Width,NewSwooshY());
                Swooshes[rand.Next(1)].Add(swoosh);
            }
        }

        SizeF WaitHeaderSize = SizeF.Empty;
        string WaitHeaderPlaceholder = "XXXXXXXXXX";

        protected void DrawWaitScreen()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Disable(EnableCap.Texture2D);
            foreach (Swoosh swoosh in Swooshes[0])
                swoosh.Draw(-2);

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

            if (WaitHeaderSize == SizeF.Empty)
                WaitHeaderSize = TextPrinter.Measure(WaitHeaderPlaceholder, WaitFont).BoundingBox.Size;

            float buffer = TheView.Window.Width - WaitHeaderSize.Width;
            RectangleF r = new RectangleF(buffer / 2, (TheView.Window.Height / 2) - (WaitHeaderSize.Height / 2), TheView.Window.Width - (buffer / 2), WaitHeaderSize.Height);

            TextPrinter.Print(WaitHeader + d, WaitFont, Color.White, r, TextPrinterOptions.Default, TextAlignment.Near);

            r = new RectangleF(buffer / 2 + 10, (TheView.Window.Height / 2) + (WaitHeaderSize.Height / 2) - 10, TheView.Window.Width - (buffer / 2), 40);
            TextPrinter.Print(StateMessage, WaitInfoFont, Color.White, r, TextPrinterOptions.Default, TextAlignment.Near);
            TextPrinter.End();

            GL.Enable(EnableCap.Texture2D);

            float spinspeed = 120;
            lastWaitAngle += spinspeed * (float)Delta;

            GL.PushMatrix();
            GL.Translate(buffer / 4, (TheView.Window.Height / 2), -1);

            GL.PushMatrix();
            GL.Rotate(lastWaitAngle, Vector3.UnitZ);

            float scale = 0.25f;
            Spinners[0].Draw(scale);
            GL.PopMatrix();

            GL.Translate(0,0, 0.01f);
            GL.Rotate(-lastWaitAngle * 1.034f, Vector3.UnitZ);
            Spinners[1].Draw(scale);
            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            foreach (Swoosh swoosh in Swooshes[1])
                swoosh.Draw(-0.5f);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            // update all the swooshes
            foreach (List<Swoosh> swooshes in Swooshes)
            {
                List<Swoosh> toKill = new List<Swoosh>();
                foreach (Swoosh swoosh in swooshes)
                {
                    swoosh.position.X -= swoosh.speed * (float)Delta;
                    if ( swoosh.position.X + swoosh.size.X < 0)
                        toKill.Add(swoosh);
                }

                 foreach (Swoosh swoosh in toKill)
                     swooshes.Remove(swoosh);
            }

            Swoosh.NewDensity();
            if (Swooshes[0].Count + Swooshes[1].Count < Swoosh.SwooshDensity)
            {
                Random rand = new Random();
                int newSwooshes = Swoosh.SwooshDensity - (Swooshes[0].Count + Swooshes[1].Count);
                for (int i = 0; i < newSwooshes; i++)
                {
                    Swoosh swoosh = new Swoosh();
                    swoosh.position = new Vector2(TheView.Window.Width + 2,NewSwooshY());
                    Swooshes[rand.Next(1)].Add(swoosh);
                }
            }
        }

        public void StatusChange ( View.ViewStatus status)
        {
            ShowWait = status != View.ViewStatus.Playing;

            if (status == View.ViewStatus.Playing)
                GL.ClearColor(Color.LightSkyBlue);
            else
            {
                GL.ClearColor(Color.Black);
                InitSwoshes();

                if (status == View.ViewStatus.Connecting)
                    WaitHeader = "Connecting";
                else if (status == View.ViewStatus.Errored)
                    WaitHeader = "Error";
                else
                    WaitHeader = "Loading";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using Game;
using Renderer;
using Textures;
using FileLocations;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WorldDrawing
{
    public class PlayerRenderer : ActorRenderer
    {
        public PlayerActor Player = null;

        public static WavefrontOBJ Tank = null;
        public static List<Texture> TeamTextures = new List<Texture>();

        public static OBJRenderer Renderer = null;

        public static Texture Shadow = null;

        public static string FileCallback(string file)
        {
            return Locations.FindDataFile(Path.Combine("models/icotank/"+file));
        }

        public static string FindTexture(string name)
        {
            return Locations.FindDataFile(name);
        }

        public PlayerRenderer(PlayerActor actor) : base(actor)
        {
            Player = actor;
            if (Tank == null)
            {
                if (WavefrontOBJ.FindFile == null)
                    WavefrontOBJ.FindFile = FileCallback;

                if(OBJRenderer.FindTexture == null)
                    OBJRenderer.FindTexture = FindTexture;

                Tank = new WavefrontOBJ();
                Tank.Read(new FileInfo(Locations.FindDataFile("models/icotank/icotank.obj")));

                TeamTextures.Add(Texture.Get(Locations.FindDataFile("models/icotank/blue.png")));
                TeamTextures.Add(Texture.Get(Locations.FindDataFile("models/icotank/red.png")));
                TeamTextures.Add(Texture.Get(Locations.FindDataFile("models/icotank/yellow.png")));
                TeamTextures.Add(Texture.Get(Locations.FindDataFile("models/icotank/purple.png")));

                Shadow = Texture.Get(Locations.FindDataFile("models/icotank/shadow.png"));

                Renderer = new OBJRenderer(Tank);
            }
        }

        public override void Draw()
        {
            GameState.BoundableActor.Location loc = Player.GetLocation();
            GL.PushMatrix();

           // GL.Translate(0, 0, -0.5f);
            GL.PushMatrix();

                GL.Translate(loc.Position);
                GL.Rotate(loc.Rotation.Z - 90, 0, 0, 1);

                GL.Rotate(90, 1, 0, 0);
                GL.Scale(0.25, 0.25, 0.25);
                Renderer.Draw();
            GL.PopMatrix();
 
            Shadow.Bind();

            // compute vector from center of tank to sun
            Vector3 delta = Player.State.GameWorld.Info.SunPosition - new Vector3(loc.Position + (Vector3.UnitZ * 0.5f));
            delta.Normalize();

            float param = 0.025f/delta.Z;
            Vector3 pos = delta * param;

            GL.Translate(loc.Position.X - pos.X, loc.Position.Y - pos.Y, loc.Position.Z);
            GL.Rotate(Player.GetLocation().Rotation.Z+90, 0, 0, 1);

            GeoUtils.Quad(Vector2.Zero, new Vector2(1.25f, 1.25f),0.01f,Color.White);

            GL.PopMatrix();
        }
    }
}

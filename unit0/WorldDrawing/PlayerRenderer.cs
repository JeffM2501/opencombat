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

        public WavefrontOBJ Tank = null;
        public Texture Skin = null;

        public string Folder = string.Empty;

        int textureOffset = 0;

        public OBJRenderer Renderer = null;

        public Texture Shadow = null;

        public string FileCallback(string file)
        {
            return Locations.FindDataFile(Path.Combine(Folder,file));
        }

        public string FindTexture(string name)
        {
            return Locations.FindDataFile(name);
        }

        public delegate string GraphicInfoCB(UInt64 GUID);
        public static GraphicInfoCB GetPlayerTexture;

        public delegate string GetPlayerModelCB(UInt64 GUID);
        public static GraphicInfoCB GetPlayerModel;

        public PlayerRenderer(PlayerActor actor) : base(actor)
        {
            Player = actor;

            Tank = new WavefrontOBJ();
            Tank.FindFile = FileCallback;

            if(OBJRenderer.FindTexture == null)
                OBJRenderer.FindTexture = FindTexture;

            string file = "models/icotank/icotank.obj";
            if (GetPlayerModel != null)
                file = GetPlayerModel(actor.GUID);

            Folder = Path.GetDirectoryName(file);

            Tank.Read(new FileInfo(Locations.FindDataFile(file)));

            string texture = Path.Combine(Folder, "blue.png");
            if (GetPlayerTexture != null)
                texture = GetPlayerTexture(actor.GUID);

            Skin = Texture.Get(Locations.FindDataFile(texture));

            string shadow = Locations.FindDataFile(Path.Combine(Folder, "shadow.png"));

            if (shadow != string.Empty && File.Exists(shadow))
                Shadow = Texture.Get(shadow);

            Renderer = new OBJRenderer(Tank);
            Renderer.BindTexture = BindTexture;
        }

        void BindTexture(OBJRenderer.Material mat)
        {
            mat.SetColor();
            Skin.Bind();
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

            if (Shadow != null)
            {    
                 Shadow.Bind();

                // compute vector from center of tank to sun
                Vector3 delta = Player.State.GameWorld.Info.SunPosition - new Vector3(loc.Position + (Vector3.UnitZ * 0.5f));
                delta.Normalize();

                float param = 0.025f/delta.Z;
                Vector3 pos = delta * param;

                GL.Translate(loc.Position.X - pos.X, loc.Position.Y - pos.Y, loc.Position.Z);
                GL.Rotate(Player.GetLocation().Rotation.Z+90, 0, 0, 1);

                GeoUtils.Quad(Vector2.Zero, new Vector2(1.25f, 1.25f),0.01f,Color.White);
            }

            GL.PopMatrix();
        }
    }
}

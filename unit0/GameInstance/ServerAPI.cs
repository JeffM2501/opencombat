using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;
using GridWorld;
using OpenTK;
using Math3D;

namespace GameInstance
{
    public class ServerAPI
    {
        public static GameState State = null;

        public static Vector4 GetStandardSpawn()
        {
            BoundingBox box = State.GameWorld.GetBounds();

            Random rand = new Random();

            Vector2 vec2 = new Vector2(box.Min.X + ((float)rand.NextDouble() * (box.Max.X-box.Min.X)),box.Min.Y + ((float)rand.NextDouble() * (box.Max.Y-box.Min.Y)));
            return new Vector4(vec2.X,vec2.Y,State.GameWorld.DropDepth(vec2),(float)rand.NextDouble() * 360);
        }
    }
}

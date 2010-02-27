using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using Math3D;

namespace Simulation
{
    public class SpawnGenerator
    {
        public static bool SpawnPlayer ( Player player, Sim sim )
        {
            player.LastUpdateState.Position = new Vector3(FloatHelper.Random(-sim.Map.Bounds.X, sim.Map.Bounds.X), FloatHelper.Random(-sim.Map.Bounds.Y, sim.Map.Bounds.Y), 0);
            player.LastUpdateState.Movement = Vector3.Zero;
            player.LastUpdateState.Rotation = FloatHelper.Random(-180f, 180f);
            player.LastUpdateState.Spin = 0;

            // todo, figure out if this is a good place or not

            return true;
        }
    }
}

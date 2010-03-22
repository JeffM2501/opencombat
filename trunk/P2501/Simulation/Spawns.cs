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
using Math3D;

namespace Simulation
{
    public class SpawnGenerator
    {
        public static bool SpawnPlayer ( Player player, Sim sim )
        {
            player.LastUpdateState.Position = new Vector3(FloatHelper.Random(-sim.MapInfo.Bounds.X, sim.MapInfo.Bounds.X), FloatHelper.Random(-sim.MapInfo.Bounds.Y, sim.MapInfo.Bounds.Y), 0);
            player.LastUpdateState.Movement = Vector3.Zero;
            player.LastUpdateState.Rotation = FloatHelper.Random(-180f, 180f);
            player.LastUpdateState.Spin = 0;

            // todo, figure out if this is a good place or not

            return true;
        }
    }
}

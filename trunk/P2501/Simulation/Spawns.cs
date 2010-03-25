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
using World;

namespace Simulation
{
    public class SpawnGenerator
    {
        public static bool SpawnPlayer ( Player player, Sim sim )
        {
            // see if there are any spawn objects for this player team in the map
            List<ObjectInstance> spawns = sim.World.FindObjects("Spawn");
            if (spawns.Count > 0)
            {
                List<ObjectInstance> teamspawns = new List<ObjectInstance>();
                if (player.Team < 0)
                    teamspawns = spawns;
                else
                {
                    foreach (ObjectInstance spawn in spawns)
                    {
                        if (spawn.ObjectAttributes.FindAttributeWithValue("Team", sim.TeamNames[player.Team]))
                            teamspawns.Add(spawn);
                    }
                }

                ObjectInstance spawnobject = teamspawns[new Random().Next(teamspawns.Count - 1)];

                player.LastUpdateState.Position = new Vector3(spawnobject.Postion);
                player.LastUpdateState.Movement = Vector3.Zero;
                player.LastUpdateState.Rotation = spawnobject.Rotation.Z;
                player.LastUpdateState.Spin = 0;
                return true;
            }

            // find a cell with the right attribute

            List<Cell> teamspawnCells = new List<Cell>();

            foreach (CellGroup group in sim.World.CellGroups)
            {
                foreach(Cell cell in group.Cells)
                {
                    if (player.Team < 0)
                    {
                        if (cell.CellAttributes.Find("Spwan").Length > 0)
                            teamspawnCells.Add(cell);
                    }
                    else
                    {
                        if (cell.CellAttributes.FindAttributeWithValue("Spawn", sim.TeamNames[player.Team]))
                            teamspawnCells.Add(cell);
                    }
                }
            }

            if (teamspawnCells.Count == 0)               
                teamspawnCells.Add(sim.World.CellGroups[0].Cells[0]); // fuckit, first cell

            Cell spawnCell = teamspawnCells[0];
            if (teamspawnCells.Count > 1)
                spawnCell = teamspawnCells[new Random().Next(teamspawnCells.Count - 1)];

            BoundingBox box = spawnCell.GetBoundingBox();

            player.LastUpdateState.Position = new Vector3(box.Min+(box.Max-box.Min));
            player.LastUpdateState.Movement = Vector3.Zero;
            player.LastUpdateState.Rotation = (float)(new Random().NextDouble())*360f;
            player.LastUpdateState.Spin = 0;
            return true;
        }
    }
}

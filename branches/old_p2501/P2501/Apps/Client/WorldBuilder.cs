using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using World;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Math3D;

namespace P2501Client
{
    class WorldBuilder
    {

        public static Plane GetEdgePlane(CellEdge edge, Cell cell)
        {
            if (edge.EdgePlane == Plane.Empty)
                edge.EdgePlane = new Plane(new Vector3(edge.Normal), Vector3.Dot(new Vector3(edge.Normal), cell.Verts[edge.Start].Bottom));
            return edge.EdgePlane;
        }

        public static void FixEdges ( Cell cell )
        {
            foreach (CellEdge edge in cell.Edges)
            {
                edge.Normal = new Vector2(cell.Verts[edge.Start].Bottom.Y - cell.Verts[edge.End].Bottom.Y, -1f * (cell.Verts[edge.Start].Bottom.X - cell.Verts[edge.End].Bottom.X));
                edge.Normal.Normalize();
                edge.Slope = new Vector2(cell.Verts[edge.Start].Bottom.X - cell.Verts[edge.End].Bottom.X, cell.Verts[edge.Start].Bottom.Y - cell.Verts[edge.End].Bottom.Y);
                edge.Slope.Normalize();
                edge.EdgePlane = GetEdgePlane(edge, cell);
            }
        }

        public static FileInfo BuildDefaultWorld(string path)
        {
            PortalWorld world = new PortalWorld();

            CellGroup group = new CellGroup();
            group.Name = world.NewGroupName();

            Cell cell = new Cell();
            cell.ID = new CellID(group.NewCellName(),group.Name);

            cell.Verts.Add(new CellVert(-400,-400,0,10));
            cell.Verts.Add(new CellVert(-400, 400, 0, 10));
            cell.Verts.Add(new CellVert(400, 400, 0, 10));
            cell.Verts.Add(new CellVert(400, -400, 0, 10));

            CellWallGeometry wallGeo = new CellWallGeometry(cell.ID, cell.ID, new CellMaterialInfo("Textures/BZ/brikwall.png"));
          
            cell.Edges.Add(new CellEdge(0, 1,wallGeo));
            cell.Edges.Add(new CellEdge(1, 2, wallGeo));
            cell.Edges.Add(new CellEdge(2, 3, wallGeo));
            cell.Edges.Add(new CellEdge(3, 0, wallGeo));

            cell.FloorMaterial = new CellMaterialInfo("Textures/BZ/grass.png");
            cell.RoofVizable = false;

            FixEdges(cell);
            group.Cells.Add(cell);
            world.CellGroups.Add(group);

            ObjectInstance obj = new ObjectInstance();
            obj.cells.Add(cell.ID);
            obj.ObjectType = "Spawn";
            obj.Name = "AutocreateSpawn.1";
            obj.Postion = new Vector3(0,0,0.1f);
            obj.Rotation = Vector3.Zero;

            world.MapObjects.Add(obj);

            FileInfo file = new FileInfo(path);

            world.Write(file);
            return file;
        }
    }
}

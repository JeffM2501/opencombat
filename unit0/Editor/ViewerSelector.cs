using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GridWorld;
using WorldDrawing;

using OpenTK;
using OpenTK.Graphics;

namespace Editor
{
    public partial class Viewer
    {
        public class Selection
        {
            public bool Valid = false;

            public Cluster.ClusterPos ClusteID = Cluster.ClusterPos.Zero;
            public Vector3 LocalBlock = Vector3.Zero;
            public Vector3 GlobalBlock = Vector3.Zero;

            public enum SelectedFace
            {
                Unknown,
                North,
                South,
                East,
                West,
                Above,
                Below,
            }

            public SelectedFace PickedFace = SelectedFace.Unknown;
        }

        public static int SelectionOffset = 5;

        protected void DrawFaceForSelection(ClusterGeometry.Face face, int name)
        {
            GL.LoadName(name);
            GridWorldRenderer.DrawFace(face);
        }

        protected class SelectionInfo
        {
            public Cluster cluster = null;
            public Vector3 block = Vector3.Zero;
            public Selection.SelectedFace face = Selection.SelectedFace.Unknown;
        }

        protected Vector3 FindMin(ClusterGeometry.Face face)
        {
            Vector3 min = new Vector3(face.Verts[0]);

            for (int i = 1; i < face.Verts.Length; i++)
            {
                if (face.Verts[i].X < min.X)
                    min.X = face.Verts[i].X;

                if (face.Verts[i].Y < min.Y)
                    min.Y= face.Verts[i].Y;

                if (face.Verts[i].Z < min.Z)
                    min.Z = face.Verts[i].Z;
            }

            return min;
        }

        protected Vector3 FindMax(ClusterGeometry.Face face)
        {
            Vector3 max = new Vector3(face.Verts[0]);

            for (int i = 1; i < face.Verts.Length; i++)
            {
                if (face.Verts[i].X > max.X)
                    max.X = face.Verts[i].X;

                if (face.Verts[i].Y > max.Y)
                    max.Y = face.Verts[i].Y;

                if (face.Verts[i].Z > max.Z)
                    max.Z = face.Verts[i].Z;
            }

            return max;
        }

        protected SelectionInfo InfoFromFace(Cluster cluster, ClusterGeometry.Face face)
        {
            SelectionInfo info = new SelectionInfo();
            info.cluster = cluster;

            Vector3 position = Vector3.Zero;

            Vector2 sampleRamp = new Vector2(1,1);
            sampleRamp.Normalize();

            if (face.Normal.Z > 0.95f)
            {
                info.face = Selection.SelectedFace.Above;
                position = FindMin(face) + new Vector3(0.5f, 0.5f, -0.1f);
            }
            else if (face.Normal.Z < -0.95f)
            {
                info.face = Selection.SelectedFace.Below;
                position = FindMin(face) + new Vector3(0.5f, 0.5f, +0.1f);
            }
            else if (face.Normal.X > 0.95f)
            {
                info.face = Selection.SelectedFace.East;
                position = FindMin(face) + new Vector3(-0.1f, 0.1f, +0.1f);
            }
            else if (face.Normal.X < -0.95f)
            {
                info.face = Selection.SelectedFace.West;
                position = FindMin(face) + new Vector3(0.1f, 0.1f, +0.1f);
            }
            else if (face.Normal.Y > 0.95f)
            {
                info.face = Selection.SelectedFace.North;
                position = FindMin(face) + new Vector3(0.1f, -0.1f, +0.1f);
            }
            else if (face.Normal.Y < -0.95f)
            {
                info.face = Selection.SelectedFace.South;
                position = FindMin(face) + new Vector3(0.1f, 0.1f, +0.1f);
            }
            else if (face.Normal.Z >= sampleRamp.X)
            {
                // it's a ramp
                info.face = Selection.SelectedFace.Above;
                position = FindMin(face) + new Vector3(0.5f, 0.5f, -0.1f);
            }

            info.block = World.PositionToBlock(position);

            return info;
        }

        static int selBufferSize = 20000;
        int[] selectBuffer = new int[selBufferSize];

        static UInt32 InvalidSelection = 0xffffffff;

        protected void SetupSelection(int x, int y)
        {
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport,viewport);

            GL.SelectBuffer(selBufferSize, selectBuffer);

            GL.RenderMode(RenderingMode.Select);
            GL.InitNames();
            GL.PushName(InvalidSelection);

            GL.MatrixMode(MatrixMode.Projection);

            GL.PushMatrix();
            GL.LoadIdentity();
            Glu.PickMatrix((double)x, (double)(viewport[3] - y), SelectionOffset, SelectionOffset, viewport);

            Camera.ApplyPrespectiveMatrix();

            GL.MatrixMode(MatrixMode.Modelview);

            GL.PushMatrix();
            GL.LoadIdentity();
            Camera.Execute();
        }

        protected int FinishSelection ()
        {
             // all the names are in and we are rendered
            GL.PopMatrix(); // pop off the camera

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix(); // pop off the select matrix

            GL.Flush();
            int hits =  GL.RenderMode(RenderingMode.Render);

            int selectedId = -1;
            uint closest = uint.MaxValue;

            for (int i = 0; i < hits; i++)
            {
                uint distance = (uint)selectBuffer[i * 4 + 1];

                if (closest >= distance)
                {
                    closest = distance;
                    selectedId = (int)selectBuffer[i * 4 + 3];
                }
            }

            return selectedId;
        }

        public Selection PickSelect(int x, int y)
        {
            Selection selection = new Selection();

            List<SelectionInfo> selectionCache = new List<SelectionInfo>();

            List<Cluster> vis = WorldRenderer.GameWorld.ClustersInFrustum(Camera.ViewFrustum, true);

            SetupSelection(x,y);

            foreach (Cluster cluster in vis)
            {
                foreach (ClusterGeometry.MeshGroup mesh in cluster.Geometry.Meshes.Values)
                {
                    foreach (ClusterGeometry.Face face in mesh.Faces)
                    {
                        selectionCache.Add(InfoFromFace(cluster, face));
                        DrawFaceForSelection(face,selectionCache.Count);
                    }
                }

                foreach (ClusterGeometry.MeshGroup mesh in cluster.Geometry.TranspereantMeshes.Values)
                {
                    foreach (ClusterGeometry.Face face in mesh.Faces)
                    {
                        selectionCache.Add(InfoFromFace(cluster, face));
                        DrawFaceForSelection(face,selectionCache.Count);
                    }
                }
            }

            int selectedID = FinishSelection();
            if (selectedID > 0 && selectedID < selectionCache.Count)
            {
                selection.Valid = true;
                SelectionInfo info = selectionCache[selectedID+1];
                selection.ClusteID = info.cluster.Origin;
                selection.GlobalBlock = info.block;
                selection.LocalBlock = new Vector3(info.cluster.Origin.X-info.block.X,info.cluster.Origin.Y-info.block.Y,info.block.Z);
                selection.PickedFace = info.face;
            }

            return selection;
        }
    }
}

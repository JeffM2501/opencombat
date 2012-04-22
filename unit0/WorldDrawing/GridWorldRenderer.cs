using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Math3D;
using GridWorld;
using Renderer;
using Textures;

using System.Diagnostics;

using FileLocations;

namespace WorldDrawing
{
    public class GridWorldRenderer
    {
        public World GameWorld = null;

        public static bool DrawDebugLines = false;

        public GridWorldRenderer(World w)
        {
            GameWorld = w;
        }

        protected Dictionary<Cluster.ClusterPos, Dictionary<int, DisplayList>> DisplayLists = new Dictionary<Cluster.ClusterPos, Dictionary<int, DisplayList>>();
        protected Dictionary<Cluster.ClusterPos, Dictionary<int, DisplayList>> TransperantDisplayLists = new Dictionary<Cluster.ClusterPos, Dictionary<int, DisplayList>>();

        protected Dictionary<Cluster.ClusterPos, Dictionary<int, StaticVertexBufferObject>> DisplayObjects = new Dictionary<Cluster.ClusterPos, Dictionary<int, StaticVertexBufferObject>>();
        protected Dictionary<Cluster.ClusterPos, Dictionary<int, StaticVertexBufferObject>> TransperantDisplayObjects = new Dictionary<Cluster.ClusterPos, Dictionary<int, StaticVertexBufferObject>>(); 

        protected List<Texture> TextureLookup = new List<Texture>();

        protected Dictionary<Cluster.ClusterPos, DisplayList> DebugLists = new Dictionary<Cluster.ClusterPos, DisplayList>();

        protected Dictionary<Cluster.ClusterPos, DisplayList> DebugLineLists = new Dictionary<Cluster.ClusterPos, DisplayList>();

        protected Texture Sun = null;
        protected Texture Skybox = null;

        protected DisplayList SkyList = null;
        protected DisplayList SunList = null;

        public static bool UseVBO = true;

        private void FlushTextures()
        {
            foreach (Texture t in TextureLookup)
                t.Invalidate();
        }

        private void RemoveTextures()
        {
            FlushTextures();
            TextureLookup.Clear();
        }

        public void BuildTextures()
        {
            TextureLookup.Clear();
            foreach (World.TextureInfo texture in GameWorld.Info.Textures)
                TextureLookup.Add(Texture.Get(Locations.FindDataFile(texture.FileName), Texture.SmoothType.Nearest, true));

            Sun = Texture.Get(Locations.FindDataFile("world/daystar.png"));
            Skybox = Texture.Get(Locations.FindDataFile("world/big_skybox.png"), Texture.SmoothType.Nearest, true);

            SkyList = new DisplayList(GenSkyBox);
        }

        public void ChangeWorld(World world)
        {
            RemoveTextures();
            // walk all the lists and kill them

            foreach (Dictionary<int, DisplayList> DL in DisplayLists.Values)
            {
                foreach (DisplayList D in DL.Values)
                    D.Dispose();

                DL.Clear();
            }
            DisplayLists.Clear();

            foreach (Dictionary<int, DisplayList> DL in TransperantDisplayLists.Values)
            {
                foreach (DisplayList D in DL.Values)
                    D.Dispose();

                DL.Clear();
            }
            TransperantDisplayLists.Clear();

            foreach (Dictionary<int, StaticVertexBufferObject> SL in DisplayObjects.Values)
            {
                foreach (StaticVertexBufferObject S in SL.Values)
                    S.Dispose();

                SL.Clear();
            }
            DisplayObjects.Clear();

            foreach (Dictionary<int, StaticVertexBufferObject> SL in TransperantDisplayObjects.Values)
            {
                foreach (StaticVertexBufferObject S in SL.Values)
                    S.Dispose();

                SL.Clear();
            }
            TransperantDisplayObjects.Clear();

            foreach (DisplayList D in DebugLists.Values)
                D.Dispose();
            DebugLists.Clear();

            foreach (DisplayList D in DebugLineLists.Values)
                D.Dispose();
            DebugLineLists.Clear();

            GameWorld = world;
            StaticInit();
        }

        public void StaticInit()
        {
            DisplayLists.Clear();
            TransperantDisplayLists.Clear();
            DisplayObjects.Clear();
            TransperantDisplayObjects.Clear();
            
            BuildTextures();

            foreach (Cluster cluster in GameWorld.Clusters.Values)
            {
                if(cluster.Geometry == null)
                    continue;

                if (UseVBO)
                {
                    DisplayObjects.Add(cluster.Origin, new Dictionary<int, StaticVertexBufferObject>());

                    foreach (KeyValuePair<int, ClusterGeometry.MeshGroup> meshgroup in cluster.Geometry.Meshes)
                    {
                        StaticVertexBufferObject dList = new StaticVertexBufferObject(GenMeshVBO);
                        dList.Tag = meshgroup.Value;
                        DisplayObjects[cluster.Origin].Add(meshgroup.Key, dList);
                    }

                    if (cluster.Geometry.TranspereantMeshes.Count > 0)
                    {
                        TransperantDisplayObjects.Add(cluster.Origin, new Dictionary<int, StaticVertexBufferObject>());

                        foreach (KeyValuePair<int, ClusterGeometry.MeshGroup> meshgroup in cluster.Geometry.TranspereantMeshes)
                        {
                            StaticVertexBufferObject dList = new StaticVertexBufferObject(GenMeshVBO);
                            dList.Tag = meshgroup.Value;
                            TransperantDisplayObjects[cluster.Origin].Add(meshgroup.Key, dList);
                        }
                    }
                }
                else
                {
                    DisplayLists.Add(cluster.Origin, new Dictionary<int, DisplayList>());

                    foreach (KeyValuePair<int, ClusterGeometry.MeshGroup> meshgroup in cluster.Geometry.Meshes)
                    {
                        DisplayList dList = new DisplayList(GenMesh);
                        dList.Tag = meshgroup.Value;
                        DisplayLists[cluster.Origin].Add(meshgroup.Key, dList);
                    }

                    TransperantDisplayLists.Add(cluster.Origin, new Dictionary<int, DisplayList>());

                    foreach (KeyValuePair<int, ClusterGeometry.MeshGroup> meshgroup in cluster.Geometry.TranspereantMeshes)
                    {
                        DisplayList dList = new DisplayList(GenMesh);
                        dList.Tag = meshgroup.Value;
                        TransperantDisplayLists[cluster.Origin].Add(meshgroup.Key, dList);
                    }
                }
                
                DisplayList debugList = new DisplayList(GetDebug);
                debugList.Tag = cluster;
                DebugLists.Add(cluster.Origin, debugList);

                debugList = new DisplayList(GetDebugLines);
                debugList.Tag = cluster;
                DebugLineLists.Add(cluster.Origin, debugList);
            }
        }

        public void FlushAll()
        {
            foreach (Cluster cluster in GameWorld.Clusters.Values)
                FlushGraphicsForCluster(cluster.Origin);

            FlushTextures();
        }

        public void FlushGraphicsForCluster(Cluster.ClusterPos pos)
        {
            if (DisplayLists.ContainsKey(pos))
            {
                Dictionary<int, DisplayList> lists = DisplayLists[pos];

                foreach (DisplayList list in lists.Values)
                    list.Invalidate();
            }

            if (TransperantDisplayLists.ContainsKey(pos))
            {
                Dictionary<int, DisplayList> lists = TransperantDisplayLists[pos];

                foreach (DisplayList list in lists.Values)
                    list.Invalidate();
            }

            if (DisplayObjects.ContainsKey(pos))
            {
                Dictionary<int, StaticVertexBufferObject> lists = DisplayObjects[pos];

                foreach (StaticVertexBufferObject o in lists.Values)
                    o.Invalidate();
            }

            if (TransperantDisplayObjects.ContainsKey(pos))
            {
                Dictionary<int, StaticVertexBufferObject> lists = TransperantDisplayObjects[pos];

                foreach (StaticVertexBufferObject o in lists.Values)
                    o.Invalidate();

                TransperantDisplayObjects.Remove(pos);
            }

            if (DebugLists.ContainsKey(pos))
                DebugLists[pos].Invalidate();

            if (DebugLineLists.ContainsKey(pos))
                DebugLineLists[pos].Invalidate();
        }

        public void RemoveGraphicsForCLuster(Cluster.ClusterPos pos)
        {
            if (DisplayLists.ContainsKey(pos))
            {
                Dictionary<int, DisplayList> lists = DisplayLists[pos];

                foreach (DisplayList list in lists.Values)
                    list.Dispose();

                DisplayLists.Remove(pos);
            }

            if (TransperantDisplayLists.ContainsKey(pos))
            {
                Dictionary<int, DisplayList> lists = TransperantDisplayLists[pos];

                foreach (DisplayList list in lists.Values)
                    list.Dispose();

                TransperantDisplayLists.Remove(pos);
            }

            if (DisplayObjects.ContainsKey(pos))
            {
                Dictionary<int, StaticVertexBufferObject> lists = DisplayObjects[pos];

                foreach (StaticVertexBufferObject o in lists.Values)
                    o.Dispose();

                DisplayObjects.Remove(pos);
            }

            if (TransperantDisplayObjects.ContainsKey(pos))
            {
                Dictionary<int, StaticVertexBufferObject> lists = TransperantDisplayObjects[pos];

                foreach (StaticVertexBufferObject o in lists.Values)
                    o.Dispose();

                TransperantDisplayObjects.Remove(pos);
            }

            if (DebugLists.ContainsKey(pos))
            {
                DebugLists[pos].Dispose();
                DebugLists.Remove(pos);
            }

            if (DebugLineLists.ContainsKey(pos))
            {
                DebugLineLists[pos].Dispose();
                DebugLineLists.Remove(pos);
            }
        }

        protected void GenSkyBox(DisplayList list)
        {
            float vDelta = 1f / 3f;
            float uDelta = 0.25f;

            float size = 1;

            GL.Color4(Color.White);
            GL.Begin(BeginMode.Quads);

            // north
            GL.TexCoord2(uDelta, vDelta * 2);
            GL.Vertex3(-size, size, -size);
            GL.TexCoord2(uDelta * 2, vDelta * 2);
            GL.Vertex3(size, size, -size);
            GL.TexCoord2(uDelta*2, vDelta);
            GL.Vertex3(size, size, size);
            GL.TexCoord2(uDelta, vDelta);
            GL.Vertex3(-size, size, size);

            // above
            GL.TexCoord2(uDelta, vDelta);
            GL.Vertex3(-size, size, size);
            GL.TexCoord2(uDelta*2, vDelta);
            GL.Vertex3(size, size, size);
            GL.TexCoord2(uDelta * 2, 0);
            GL.Vertex3(size, -size, size);
            GL.TexCoord2(uDelta, 0);
            GL.Vertex3(-size, -size, size);

            // right
            GL.TexCoord2(uDelta*2, vDelta*2);
            GL.Vertex3(size, size, -size);
            GL.TexCoord2(uDelta * 3, vDelta * 2);
            GL.Vertex3(size, -size, -size);
            GL.TexCoord2(uDelta * 3, vDelta);
            GL.Vertex3(size, -size, size);
            GL.TexCoord2(uDelta * 2, vDelta);
            GL.Vertex3(size, size, size);

            // left
            GL.TexCoord2(uDelta, vDelta * 2);
            GL.Vertex3(-size, size, -size);
            GL.TexCoord2(uDelta, vDelta);
            GL.Vertex3(-size, size, size);
            GL.TexCoord2(0, vDelta );
            GL.Vertex3(-size, -size, size);
            GL.TexCoord2(0, vDelta * 2);
            GL.Vertex3(-size, -size, -size);

            // back
            GL.TexCoord2(uDelta*3, vDelta * 2);
            GL.Vertex3(size, -size, -size);
            GL.TexCoord2(1, vDelta * 2);
            GL.Vertex3(-size, -size, -size);
            GL.TexCoord2(1, vDelta);
            GL.Vertex3(-size, -size, size);
            GL.TexCoord2(uDelta * 3, vDelta);
            GL.Vertex3(size, -size, size);

            GL.End();
        }

        Stopwatch SkyAnimationTimer = new Stopwatch();

        double Now()
        {
            return SkyAnimationTimer.ElapsedMilliseconds * 0.001;
        }

        public double SunSpinSpeed = 1;

        public void DrawSky( Camera cam )
        {
            if (!SkyAnimationTimer.IsRunning)
                SkyAnimationTimer.Start();

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Rotate(-cam.GetTilt(), 1.0f, 0.0f, 0.0f);					    // pops us to the tilt
            GL.Rotate(-cam.GetSpin() + 90.0, 0.0f, 1.0f, 0.0f);    // gets us on our rot
            GL.Rotate(-90, 1.0f, 0.0f, 0.0f);				            // gets us into XY

            Skybox.Bind();
            SkyList.Call();   

            GL.PopMatrix();

            GL.PushMatrix();
            
            GL.Translate(GameWorld.Info.SunPosition);
            GL.Translate(cam.GetPostion());
            cam.DoBillboard();

            if (SunSpinSpeed > 0)
                GL.Rotate(SunSpinSpeed * Now(), Vector3d.UnitY);

            GL.Color4(Color.White);
            Sun.Bind();
            GeoUtils.QuadXZ(new Size(50, 50));
            GL.PopMatrix();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }

        public void DrawAllDynamic()
        {
            Dictionary<int, List<ClusterGeometry.MeshGroup>> texturedList = new Dictionary<int, List<ClusterGeometry.MeshGroup>>();

            foreach (Cluster cluster in GameWorld.Clusters.Values)
            {
                if (cluster.Geometry == null)
                    continue;
                foreach (ClusterGeometry.MeshGroup mesh in cluster.Geometry.Meshes.Values)
                {
                    if (!texturedList.ContainsKey(mesh.TextureID))
                        texturedList.Add(mesh.TextureID, new List<ClusterGeometry.MeshGroup>());
                        
                    texturedList[mesh.TextureID].Add(mesh);
                }
            }

            foreach (KeyValuePair<int, List<ClusterGeometry.MeshGroup>> meshes in texturedList)
            {
                TextureLookup[meshes.Key].Bind();
                foreach (ClusterGeometry.MeshGroup mesh in meshes.Value)
                    DrawMesh(mesh);
            }

            GL.DepthMask(false);
            texturedList.Clear();
            foreach (Cluster cluster in GameWorld.Clusters.Values)
            {
                if (cluster.Geometry == null)
                    continue;
                foreach (ClusterGeometry.MeshGroup mesh in cluster.Geometry.TranspereantMeshes.Values)
                {
                    if (!texturedList.ContainsKey(mesh.TextureID))
                        texturedList.Add(mesh.TextureID, new List<ClusterGeometry.MeshGroup>());

                    texturedList[mesh.TextureID].Add(mesh);
                }
            }

            foreach (KeyValuePair<int, List<ClusterGeometry.MeshGroup>> meshes in texturedList)
            {
                TextureLookup[meshes.Key].Bind();
                foreach (ClusterGeometry.MeshGroup mesh in meshes.Value)
                    DrawMesh(mesh);
            }

            if (DrawDebugLines)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.Texture2D);
                foreach (Cluster cluster in GameWorld.Clusters.Values)
                {
                    if (DebugLineLists.ContainsKey(cluster.Origin))
                        DebugLineLists[cluster.Origin].Call();
                }

                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Texture2D);
            }

            GL.DepthMask(true);
        }

        public void DrawAll() { DrawAll(false); }

        public void DrawAll( bool debug)
        {
            Draw(GameWorld.Clusters.Values);

            if (debug)
                DrawDebug(GameWorld.Clusters.Values);
        }

        public void DrawVisible(BoundingFrustum frustum) { DrawVisible(frustum, false); }

        public void DrawVisible(BoundingFrustum frustum, bool debug)
        {
            List<Cluster> vis = GameWorld.ClustersInFrustum(frustum, true);
            Draw(vis);
            if (debug)
                DrawDebug(vis);
        }

        protected void Draw(IEnumerable<Cluster> clusters)
        {
            Dictionary<int, List<DisplayList>> texturedList = new Dictionary<int, List<DisplayList>>();
            Dictionary<int, List<StaticVertexBufferObject>> texturedObjects = new Dictionary<int, List<StaticVertexBufferObject>>();

            foreach (Cluster cluster in clusters)
            {
                if (UseVBO)
                {
                    if (DisplayObjects.ContainsKey(cluster.Origin))
                    {
                        foreach (KeyValuePair<int, StaticVertexBufferObject> dLists in DisplayObjects[cluster.Origin])
                        {
                            if (!texturedObjects.ContainsKey(dLists.Key))
                                texturedObjects.Add(dLists.Key, new List<StaticVertexBufferObject>());

                            texturedObjects[dLists.Key].Add(dLists.Value);
                        }
                    }
                }
                else
                {
                    if (DisplayLists.ContainsKey(cluster.Origin))
                    {
                        foreach (KeyValuePair<int, DisplayList> dLists in DisplayLists[cluster.Origin])
                        {
                            if (!texturedList.ContainsKey(dLists.Key))
                                texturedList.Add(dLists.Key, new List<DisplayList>());

                            texturedList[dLists.Key].Add(dLists.Value);
                        }
                    }
                }
            }

            if (UseVBO)
            {
                foreach (KeyValuePair<int, List<StaticVertexBufferObject>> i in texturedObjects)
                {
                    TextureLookup[i.Key].Bind();
                    foreach (StaticVertexBufferObject d in i.Value)
                        d.Draw();
                }
            }
            else
            {
                foreach (KeyValuePair<int, List<DisplayList>> i in texturedList)
                {
                    TextureLookup[i.Key].Bind();
                    foreach (DisplayList d in i.Value)
                        d.Call();
                }
            }
           

            // do the transperants;
            GL.DepthMask(false);
            texturedList.Clear();
            texturedObjects.Clear();

            foreach (Cluster cluster in clusters)
            {
                if (UseVBO)
                {
                    if (TransperantDisplayObjects.ContainsKey(cluster.Origin))
                    {
                        foreach (KeyValuePair<int, StaticVertexBufferObject> dLists in TransperantDisplayObjects[cluster.Origin])
                        {
                            if (!texturedObjects.ContainsKey(dLists.Key))
                                texturedObjects.Add(dLists.Key, new List<StaticVertexBufferObject>());

                            texturedObjects[dLists.Key].Add(dLists.Value);
                        }
                    }
                }
                else
                {
                    if (TransperantDisplayLists.ContainsKey(cluster.Origin))
                    {
                        foreach (KeyValuePair<int, DisplayList> dLists in TransperantDisplayLists[cluster.Origin])
                        {
                            if (!texturedList.ContainsKey(dLists.Key))
                                texturedList.Add(dLists.Key, new List<DisplayList>());

                            texturedList[dLists.Key].Add(dLists.Value);
                        }
                    }
                }
                
            }

            if (UseVBO)
            {
                foreach (KeyValuePair<int, List<StaticVertexBufferObject>> i in texturedObjects)
                {
                    TextureLookup[i.Key].Bind();
                    foreach (StaticVertexBufferObject d in i.Value)
                        d.Draw();
                }
            }
            else
            {
                foreach (KeyValuePair<int, List<DisplayList>> i in texturedList)
                {
                    TextureLookup[i.Key].Bind();
                    foreach (DisplayList d in i.Value)
                        d.Call();
                }
            }


            if (DrawDebugLines)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.Texture2D);
                foreach (Cluster cluster in clusters)
                {
                    if (DebugLineLists.ContainsKey(cluster.Origin))
                        DebugLineLists[cluster.Origin].Call();
                }

                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Texture2D);
            }

            GL.DepthMask(true);

        }

        protected void DrawDebug(IEnumerable<Cluster> clusters)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            foreach (Cluster cluster in clusters)
            {
                if (DebugLists.ContainsKey(cluster.Origin))
                    DebugLists[cluster.Origin].Call();
            }

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }

        public void DrawDebugLineGeo(ClusterGeometry.MeshGroup mesh, Color color, float alpha)
        {
            float offset = 0.05f;

            GL.Disable(EnableCap.Texture2D);
            GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
            foreach (ClusterGeometry.Face face in mesh.Faces)
            {
                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(face.Verts[0] + (face.Normal * offset));
                GL.Vertex3(face.Verts[1] + (face.Normal * offset));
                GL.Vertex3(face.Verts[2] + (face.Normal * offset));

                if (face.Verts[2] != face.Verts[3])
                    GL.Vertex3(face.Verts[3] + (face.Normal * offset));

                GL.End();
            }
            GL.Disable(EnableCap.PolygonOffsetLine);
            GL.Enable(EnableCap.Texture2D);
        }

        public static void DrawFace(ClusterGeometry.Face face)
        {
            if (face == null || face == ClusterGeometry.Face.Empty || face.Verts == null || face.Verts.Length == 0)
                return;

            GL.Normal3(face.Normal);
            GL.Begin(BeginMode.Polygon);

            GL.Color3(face.Luminance[0], face.Luminance[0], face.Luminance[0]);
            GL.TexCoord2(face.UVs[0]);
            GL.Vertex3(face.Verts[0]);

            GL.Color3(face.Luminance[1], face.Luminance[1], face.Luminance[1]);
            GL.TexCoord2(face.UVs[1]);
            GL.Vertex3(face.Verts[1]);

            GL.Color3(face.Luminance[2], face.Luminance[2], face.Luminance[2]);
            GL.TexCoord2(face.UVs[2]);
            GL.Vertex3(face.Verts[2]);

            if (face.Verts[2] != face.Verts[3])
            {
                GL.Color3(face.Luminance[3], face.Luminance[3], face.Luminance[3]);
                GL.TexCoord2(face.UVs[3]);
                GL.Vertex3(face.Verts[3]);
            }

            GL.End();
        }

        public void DrawMesh(ClusterGeometry.MeshGroup mesh)
        {
            GL.Color4(Color.White);
            foreach (ClusterGeometry.Face face in mesh.Faces)
                DrawFace(face);
        }

        public void GenMesh(DisplayList list)
        {
            ClusterGeometry.MeshGroup mesh = list.Tag as ClusterGeometry.MeshGroup;
            if (mesh == null)
                return;

            DrawMesh(mesh);
        }

        public static Color LuminanceFromColor ( Color baseColor, float lums)
        {
            return Color.FromArgb((int)(baseColor.A), (int)(baseColor.R * lums), (int)(baseColor.G * lums), (int)(baseColor.B * lums));
        }

        public StaticVertexBufferObject.BufferData GenMeshVBO(StaticVertexBufferObject vbo)
        {
            ClusterGeometry.MeshGroup mesh = vbo.Tag as ClusterGeometry.MeshGroup;
            if (mesh == null)
                return StaticVertexBufferObject.BufferData.Empty;

            StaticVertexBufferObject.ShortIndexFullVertBuffer buffer = new StaticVertexBufferObject.ShortIndexFullVertBuffer();

            List<StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData> verts = buffer.Verts;
            List<short> indexes = buffer.Indexes;

            foreach (ClusterGeometry.Face face in mesh.Faces)
            {
                short start = (short)verts.Count;

                indexes.Add((short)verts.Count); 
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(face.UVs[0], LuminanceFromColor(Color.White, face.Luminance[0]), face.Normal, face.Verts[0]));

                indexes.Add((short)verts.Count);
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(face.UVs[1], LuminanceFromColor(Color.White, face.Luminance[1]), face.Normal, face.Verts[1]));

                indexes.Add((short)verts.Count);
                verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(face.UVs[2], LuminanceFromColor(Color.White, face.Luminance[2]), face.Normal, face.Verts[2]));

                if (face.Verts.Length > 3 && face.Verts[3] != face.Verts[2])
                {
//                     indexes.Add((short)(verts.Count - 1));
//                     indexes.Add((short)(verts.Count - 2));
//                     indexes.Add((short)verts.Count);

                    indexes.Add((short)(start + 2));
                    indexes.Add((short)(start + 3));
                    indexes.Add((short)(start + 0));
                    verts.Add(new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData(face.UVs[3], LuminanceFromColor(Color.White, face.Luminance[3]), face.Normal, face.Verts[3]));
                }
            }

            buffer.Elements = indexes.ToArray();
            buffer.Data = verts.ToArray();
            return buffer.Pack();
        }

        public static void DrawConerLines()
        {
            float size = 0.125f;

            GL.Begin(BeginMode.Lines);

            // lower
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(size, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, size, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, size);

            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1-size, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, size, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, size);

            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1 - size, 1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1 - size, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1, size);

            GL.Vertex3(0, 1, 0);
            GL.Vertex3(size, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1 - size, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1, size);

            // upper
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(size, 0, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, size, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, 0, 1-size);

            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1 - size, 0, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, size, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, 0, 1-size);

            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1 - size, 1, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 1 - size, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 1, 1-size);

            GL.Vertex3(0, 1, 1);
            GL.Vertex3(size, 1, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1 - size, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1, 1-size);

            GL.End();
        }

        public static void DrawSoldBox()
        {
            GL.Enable(EnableCap.Lighting);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(0, 0, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(0, 1, 1);

            GL.Normal3(0, 0, -1);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 0, 0);

            GL.Normal3(1, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 0, 1);

            GL.Normal3(-1, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1, 0);

            GL.Normal3(0, 1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(1, 1, 1);

            GL.Normal3(0, 11, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, 0, 0);

            GL.End();
            GL.Disable(EnableCap.Lighting);
        }

        public void GetDebugLines(DisplayList list)
        {
            Cluster cluster = list.Tag as Cluster;
            if (cluster == null)
                return;

            foreach (ClusterGeometry.MeshGroup group in cluster.Geometry.Meshes.Values)
                DrawDebugLineGeo(group, Color.MediumSpringGreen, 0.25f);

            foreach (ClusterGeometry.MeshGroup group in cluster.Geometry.TranspereantMeshes.Values)
                DrawDebugLineGeo(group, Color.LightSeaGreen, 0.125f);
        }

        public void GetDebug(DisplayList list)
        {
            Cluster cluster = list.Tag as Cluster;
            if (cluster == null)
                return;
            GL.PushMatrix();

            GL.Translate(cluster.Origin.X, cluster.Origin.Y, 0f);
            GL.Color4(1f, 1f, 1f, 0.5f);
            for (int z = 0; z < Cluster.ZSize; z++)
            {
                for (int x = 0; x < Cluster.XYSize; x++)
                {
                    for (int y = 0; y < Cluster.XYSize; y++)
                    {
                        GL.PushMatrix();
                        GL.Translate(x,y,z);
                        Cluster.Block block = cluster.GetBlockRelative(x, y, z);

                        if (block.Geom == Cluster.Block.Geometry.Empty)
                        {
                            GL.Enable(EnableCap.DepthTest);
                            GL.Color4(1f,1f,1f,0.0625f);
                            DrawConerLines();
                            GL.Disable(EnableCap.DepthTest);
                        }
                        else
                        {      
                            GL.Color4(0,1f,0,0.5f);
                            if (block.Geom == Cluster.Block.Geometry.Solid)
                                GL.Color4(0.5f, 0.5f, 0.25f, 0.5f);
                            DrawSoldBox();
                        }

                        GL.PopMatrix();
                    }
                }
            }
            GL.Color4(1f,1f,1f,1f);
            GL.PopMatrix();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Textures;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class DrawableObjFiles
    {
        protected Dictionary<OBJRenderer.Material, List<StaticVertexBufferObject>> Objects = new Dictionary<OBJRenderer.Material, List<StaticVertexBufferObject>>();

        public void Draw(OBJRenderer renderer)
        {
            foreach (KeyValuePair<OBJRenderer.Material, StaticVertexBufferObject> vbos in renderer.VBOs)
            {
                if (!Objects.ContainsKey(vbos.Key))
                    Objects.Add(vbos.Key, new List<StaticVertexBufferObject>());

                Objects[vbos.Key].Add(vbos.Value);
            }
        }

        public void Process()
        {
            foreach (KeyValuePair<OBJRenderer.Material, List<StaticVertexBufferObject>> obs in Objects)
            {
                obs.Key.Bind();
                foreach (StaticVertexBufferObject o in obs.Value)
                    o.Draw();
            }
        }
    }

    public class OBJRenderer
    {
        WavefrontOBJ model;

        public static bool UseVBO = false;

        public delegate string FindTextureCB(string name);
        public static FindTextureCB FindTexture = null;

        public class Material : IEquatable<Material>
        {
            public Texture texture;
            public Color color;

            public Material(WavefrontOBJ.Material mat)
            {
                string file = string.Empty;

                if (FindTexture != null)
                    file = FindTexture(mat.DiffuseTexture);

                if (file != string.Empty && File.Exists(file))
                    texture = Texture.Get(file);
                
                color = mat.Diffuse;
            }

            public Material ()
            {
                texture = null;
                color = Color.White;
            }

            public void SetColor()
            {
                GL.Color4(color);
            }
            public void Bind ()
            {
                SetColor();
                if (texture != null)
                    texture.Bind();
                else
                    GL.Disable(EnableCap.Texture2D);
            }

            public static Material Empty = new Material();

            public override int GetHashCode()
            {
                if (texture == null)
                    return color.GetHashCode();

                return texture.GetHashCode() ^ color.GetHashCode();
            }

            public bool Equals(Material obj)
            {
                if (obj == null || GetHashCode() != obj.GetHashCode())
                    return false;

                return texture.Name == obj.texture.Name && color == obj.color;
            }

            public override bool Equals(object obj)
            {
                return obj.GetType() == typeof(Material) && Equals(obj as Material);
            }
        }

        protected Dictionary<WavefrontOBJ.FaceSet,Material> MaterialCache = new Dictionary<WavefrontOBJ.FaceSet,Material>();

        public Dictionary<Material, StaticVertexBufferObject> VBOs = new Dictionary<Material, StaticVertexBufferObject>();

        public void Flush()
        {
            MaterialCache.Clear();
        }

        protected Material GetMaterial ( WavefrontOBJ.FaceSet faceset )
        {
            if (MaterialCache.ContainsKey(faceset))
                return MaterialCache[faceset];

             Material mat;
            if (model.Materials.ContainsKey(faceset.Material))
                mat = new Material(model.Materials[faceset.Material]);
            else
                mat = Material.Empty;

            MaterialCache[faceset] = mat;
            return mat;
        }

        public OBJRenderer(WavefrontOBJ obj)
        {
            model = obj;

            // walk that shit and build us a lit

            foreach (KeyValuePair<string, List<WavefrontOBJ.Group>> groups in model.Objects)
            {
                foreach (WavefrontOBJ.Group group in groups.Value)
                {
                    foreach (KeyValuePair<string, WavefrontOBJ.FaceSet> faceset in group.FaceSets)
                    {
                        Material mat = GetMaterial(faceset.Value);
                        if (!VBOs.ContainsKey(mat))
                        {
                            StaticVertexBufferObject vob = new StaticVertexBufferObject(buildVBO);
                            vob.Tag = mat;
                            VBOs.Add(mat, vob);
                        }
                    }
                }
            }
        }

        public StaticVertexBufferObject.BufferData buildVBO(StaticVertexBufferObject obj)
        {
            StaticVertexBufferObject.ShortIndexFullVertBuffer buffer = new StaticVertexBufferObject.ShortIndexFullVertBuffer();

            Material thisMat = obj.Tag as Material;
            if (thisMat != null)
            {
                foreach (KeyValuePair<string, List<WavefrontOBJ.Group>> groups in model.Objects)
                {
                    foreach (WavefrontOBJ.Group group in groups.Value)
                    {
                        foreach (KeyValuePair<string, WavefrontOBJ.FaceSet> faceset in group.FaceSets)
                        {
                            if (GetMaterial(faceset.Value) == thisMat)
                            {
                                foreach (WavefrontOBJ.Face face in faceset.Value.Faces)
                                {
                                    if (face.Verts.Count < 3)
                                        continue;

                                    short startVert = (short)buffer.Verts.Count;
                                    for (short i = 0; i < face.Verts.Count; i++)
                                    {
                                        StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData data = new StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData();
                                        data.Color = StaticVertexBufferObject.ShortIndexFullVertBuffer.VertexData.ToRgba(thisMat.color);
                                        data.Vertex = model.Verts[face.Verts[i]];
                                        data.Normal = model.Norms[face.Norms[i]];
                                        data.UV = model.UVs[face.UVs[i]];
                                        buffer.Verts.Add(data);
                                        if (i >= 3)
                                        {
                                            buffer.Indexes.Add((short)(startVert + i -1));
                                            buffer.Indexes.Add((short)(startVert + i));
                                            buffer.Indexes.Add((short)(startVert));
                                        }
                                        else
                                            buffer.Indexes.Add((short)(startVert + i));

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return buffer.Pack();
        }

        public delegate void BindTextureCB(OBJRenderer.Material mat);

        public BindTextureCB BindTexture;

        protected void Bind(OBJRenderer.Material mat)
        {
            if (BindTexture != null)
                BindTexture(mat);
            else
                mat.Bind();
        }

        public void Draw()
        {
            if (UseVBO)
            {
                foreach (KeyValuePair<OBJRenderer.Material, StaticVertexBufferObject> vbos in VBOs)
                {
                    Bind(vbos.Key);
                    vbos.Value.Draw();
                }
            }
            else
            {
                foreach (KeyValuePair<string, List<WavefrontOBJ.Group>> groups in model.Objects)
                {
                    foreach (WavefrontOBJ.Group group in groups.Value)
                    {
                        foreach (KeyValuePair<string, WavefrontOBJ.FaceSet> faceset in group.FaceSets)
                        {
                            Bind(GetMaterial(faceset.Value));

                            foreach (WavefrontOBJ.Face face in faceset.Value.Faces)
                            {
                                GL.Begin(BeginMode.Polygon);
                                for (int i = 0; i < face.Verts.Count; i++)
                                {
                                    if (face.Norms[i] >= 0 && face.Norms[i] < model.Norms.Count)
                                        GL.Normal3(model.Norms[face.Norms[i]]);

                                    if (face.UVs[i] >= 0 && face.UVs[i] < model.UVs.Count)
                                        GL.TexCoord2(model.UVs[face.UVs[i]].X, 1.0 - model.UVs[face.UVs[i]].Y);

                                    if (face.Verts[i] >= 0 && face.Verts[i] < model.Verts.Count)
                                        GL.Vertex3(model.Verts[face.Verts[i]]);
                                }
                                GL.End();
                            }
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

using OpenTK;

namespace Renderer
{
    public class WavefrontOBJ
    {
        public List<Vector3> Verts = new List<Vector3>();
        public List<Vector3> Norms = new List<Vector3>();
        public List<Vector2> UVs = new List<Vector2>();

        public delegate string FileCallback(string file);

        public static FileCallback FindFile = null;

        public class Material
        {
            public string Name = string.Empty;
            public Color Ambient = Color.Gray;
            public Color Diffuse = Color.White;
            public Color Specular = Color.Empty;
            public Color Emssive = Color.Empty;
            public string AmbientTexture = string.Empty;
            public string DiffuseTexture = string.Empty;
            public string SpecularTexture = string.Empty;
            public string BumpMap = string.Empty;
            public string AlphaMap = string.Empty;
        }

        public Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public class Face
        {
            public List<int> Verts = new List<int>();
            public List<int> Norms = new List<int>();
            public List<int> UVs = new List<int>();
        }

        public class FaceSet
        {
            public string Material = string.Empty;
            public List<Face> Faces = new List<Face>();
        }

        public class Group
        {
            public string Name = string.Empty;
            public Dictionary<string,FaceSet> FaceSets = new Dictionary<string,FaceSet>();
        }
        public Dictionary<string, List<Group>> Objects = new Dictionary<string, List<Group> >();

        public void Clear()
        {
            Objects.Clear();
            Verts.Clear();
            Norms.Clear();
            UVs.Clear();
            Materials.Clear();
        }

        protected string CleanupString(string input)
        {
            if (input.IndexOf('#') != -1)
                input.Remove(input.IndexOf('#'));

            input = input.TrimStart(new char[] { ' ', '\t' });
            input = input.TrimEnd(new char[] { ' ', '\t' });

            if (input.Length > 0)
                return input;
            return string.Empty;
        }

        protected void ReadFaceVertex ( string input, out int[] indexes )
        {
            indexes = new int[]{-1,-1,-1};

            int offset = input.IndexOf('/');
            if (offset == -1)
            {
                indexes[0] = int.Parse(input)-1;
                return;
            }

            indexes[0] = int.Parse(input.Substring(0, offset)) -1;

            int secondSlash = input.IndexOf('/',offset+1);
            if (secondSlash != offset+1)
                indexes[1] = int.Parse(input.Substring(offset + 1, secondSlash-offset-1)) - 1;
            if (secondSlash != input.Length)
                indexes[2] = int.Parse(input.Substring(secondSlash+1,input.Length-secondSlash-1)) - 1;
        }

        protected void ReadMaterials(string _file)
        {
            if (FindFile != null)
                _file = FindFile(_file);

            FileInfo file = new FileInfo(_file);

            if (!file.Exists)
                return;

            StreamReader reader = file.OpenText();

            Material mat = new Material();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.TrimStart(new char[] { ' ', '\t' });
                if (line.Length == 0 || line[0] == '#')
                    continue;

                string[] spaceDelim = line.Split(" ".ToCharArray());

                switch (line[0])
                {
                    case 'n':
                        if (mat.Name != string.Empty)
                        {
                            Materials.Add(mat.Name, mat);
                            mat = new Material();
                        }

                        mat.Name = CleanupString(line.Remove(0, 7));
                        break;
                    case 'K':
                        if (spaceDelim.Length > 3)
                        {
                            float[] p = new float[]{1,1,1,1};
                            int j = 0;
                            for (int i = 1; i < spaceDelim.Length; i++)
                            {
                                if (spaceDelim[i].Length > 0)
                                {
                                    if (spaceDelim[i][0] == '#')
                                        i = spaceDelim.Length;
                                    else
                                        p[j++] = (float.Parse(spaceDelim[i]));
                                }
                            }

                            Color c = Color.FromArgb((byte)(p[3] * byte.MaxValue), (byte)(p[0] * byte.MaxValue), (byte)(p[1] * byte.MaxValue), (byte)(p[2] * byte.MaxValue));
                            if (spaceDelim[0] == "Ka")
                                mat.Ambient = c;
                            else if (spaceDelim[0] == "Kd")
                                mat.Diffuse = c;
                            else if (spaceDelim[0] == "Ks")
                                mat.Specular = c;
                        }
                        break;

                    case 'm':

                        if (spaceDelim.Length > 1)
                        {
                            string map = CleanupString(line.Remove(0, spaceDelim[0].Length+1));

                            if (spaceDelim[0] == "map_Kd")
                                mat.DiffuseTexture = map;
                            else if (spaceDelim[0] == "map_Ka" || mat.DiffuseTexture != string.Empty)
                                mat.DiffuseTexture = map;
                            else if (spaceDelim[0] == "map_Ks")
                                mat.SpecularTexture = map;
                            else if (spaceDelim[0] == "map_d")
                                mat.AlphaMap = map;
                        }
                        break;
                }
            }
            if (mat.Name != string.Empty)
            {
                Materials.Add(mat.Name, mat);
                mat = new Material();
            }

            reader.Close();
        }

        public void Read( FileInfo file )
        {
            if (!file.Exists)
                return;

            FileStream stream = null;
            while (stream== null)
            {
                try
                {
                    stream = file.OpenRead();
                }
                catch
                {
                    stream = null;
                }
            }
            StreamReader reader = new StreamReader(stream); 

            Group group = null;
            FaceSet faceset = null;

            string groupName = string.Empty;
            string objectName = string.Empty;
            string matName = string.Empty;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.TrimStart(new char[] { ' ', '\t' });
                if (line.Length == 0 || line[0] == '#')
                    continue;

                string[] spaceDelim = line.Split(" ".ToCharArray());

                switch (line[0])
                {
                    case 'v':
                        if (spaceDelim.Length > 2)
                        {
                            List<float> vals = new List<float>();
                            int i = 1;
                            while (i < spaceDelim.Length)
                            {
                                if (spaceDelim[i].Length != 0 && spaceDelim[i][0] != '#')
                                    vals.Add(float.Parse(spaceDelim[i]));
                                else if (spaceDelim[i].Length != 0)
                                    i = spaceDelim.Length;

                                i++;
                            }
                            if (spaceDelim[0] == "v")
                                Verts.Add(new Vector3(vals[0],vals[1],vals[2]));
                            else if (spaceDelim[0] == "vn")
                                Norms.Add(new Vector3(vals[0],vals[1],vals[2]));
                            else if (spaceDelim[0] == "vt")
                                UVs.Add(new Vector2(vals[0],vals[1]));
                        }
                        break;

                    case 'o':
                        if (faceset != null && faceset.Faces.Count > 0)
                        {
                            if (group == null)
                                group = new Group();

                            if (group.FaceSets.ContainsKey(faceset.Material))
                                group.FaceSets[faceset.Material].Faces = group.FaceSets[faceset.Material].Faces.Concat(faceset.Faces).ToList();
                            else
                                group.FaceSets.Add(faceset.Material, faceset);
                        }

                        if (group != null && group.FaceSets.Count > 0)
                        {
                            if (!Objects.ContainsKey(objectName))
                                Objects.Add(objectName, new List<Group>());
                            Objects[objectName].Add(group);
                        }

                        objectName = CleanupString(line.Remove(0,2));
                        faceset = new FaceSet();
                        faceset.Material = matName;
                        group = new Group();
                        break;
                    case 'g':
                        if (faceset != null && faceset.Faces.Count > 0)
                        {
                            if (group == null)
                                group = new Group();

                            if (group.FaceSets.ContainsKey(faceset.Material))
                                group.FaceSets[faceset.Material].Faces = group.FaceSets[faceset.Material].Faces.Concat(faceset.Faces).ToList();
                            else
                                group.FaceSets.Add(faceset.Material, faceset); 
                        }

                        if (group != null && group.FaceSets.Count > 0)
                        {
                            if (!Objects.ContainsKey(objectName))
                                Objects.Add(objectName, new List<Group>());
                            Objects[objectName].Add(group);
                        }

                        group = new Group();
                        group.Name = CleanupString(line.Remove(0, 2));
                        faceset = new FaceSet();
                        faceset.Material = matName;
                        break;

                    case 'u': //usemtl
                        if (faceset != null && faceset.Faces.Count > 0)
                        {
                            if (group == null)
                                group = new Group();

                            if (group.FaceSets.ContainsKey(faceset.Material))
                                group.FaceSets[faceset.Material].Faces = group.FaceSets[faceset.Material].Faces.Concat(faceset.Faces).ToList();
                            else
                                group.FaceSets.Add(faceset.Material, faceset); 
                        }
                        matName = CleanupString(line.Remove(0, 7));
                        faceset = new FaceSet();
                        faceset.Material = matName;
                        break;

                    case 'f':
                        Face face = new Face();
                        if (faceset == null)
                            faceset = new FaceSet();
                        for (int i = 1; i < spaceDelim.Length; i++)
                        {
                            string component = spaceDelim[i];
                            if (component.Length > 0)
                            {
                                if (component[0] == '#')
                                    i = spaceDelim.Length;
                                else
                                {
                                    int[] indexes;
                                    ReadFaceVertex(component,out indexes);
                                    if (indexes[0] >= 0)
                                    {
                                        face.Verts.Add(indexes[0]);
                                        face.Norms.Add(indexes[2]);
                                        face.UVs.Add(indexes[1]);
                                    }
                                }
                            }
                        }
                        if (face.Verts.Count > 0)
                            faceset.Faces.Add(face);
                        break;

                    case 'm':
                        ReadMaterials(CleanupString(line.Substring(7, line.Length - 7)));
                        break;

                    case 's':
                        break;
                }

            }

            if (faceset != null && faceset.Faces.Count > 0)
            {
                if (group == null)
                    group = new Group();

                if (group.FaceSets.ContainsKey(faceset.Material))
                    group.FaceSets[faceset.Material].Faces = group.FaceSets[faceset.Material].Faces.Concat(faceset.Faces).ToList();
                else
                    group.FaceSets.Add(faceset.Material, faceset);
            }

            if (group != null && group.FaceSets.Count > 0)
            {
                if (!Objects.ContainsKey(objectName))
                    Objects.Add(objectName, new List<Group>());
                Objects[objectName].Add(group);
            }

            reader.Close();
            stream.Close();
        }

    }
}

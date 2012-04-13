﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

using Math3D;
using OpenTK;

namespace GridWorld
{
    public class World
    {
        public static string WorldFileExtension = "world";
        public static string ClusterFileExtension = "cluster";
        public static string GeometryFileExtension = "Geom";

        protected static bool _CompressFileIO = true;
        public static bool CompressFileIO {get {return _CompressFileIO;}}

        public static World Empty = new World();

        public class TextureInfo
        {
            public override string ToString()
            {
                return FileName;
            }

            public string FileName = string.Empty;
            public int XCount = 16;
            public int YCount = 16;

            public TextureInfo() { }

            public TextureInfo(string name)
            {
                FileName = name;
            }

            public TextureInfo(string name, int x, int y)
            {
                FileName = name;
                XCount = x;
                YCount = y;
            }

            [XmlIgnore]
            public int Start = -1;

            [XmlIgnore]
            public int End = -1;
        }

        public class WorldInfo
        {
            public List<TextureInfo> Textures = new List<TextureInfo>();
            public string Name = string.Empty;
            public string Author = string.Empty;
            public string Site = string.Empty;

            public Vector3 SunPosition = new Vector3(200, 100, 150);
            public float Ambient = 0.5f;
            public float SunLuminance = 1.0f;
        }
        public WorldInfo Info = new WorldInfo();

        [XmlIgnore]
        protected Dictionary<Cluster.Block.Geometry, Plane> CollisionPlanes = new Dictionary<Cluster.Block.Geometry, Plane>();

        public void SetupTextureInfos()
        {
            int count = 0;
            foreach (TextureInfo info in Info.Textures)
            {
                info.Start = count;
                count += info.XCount * info.YCount;
                info.End = count - 1;
            }
        }

        protected void CheckTextureInfos()
        {
            if (Info.Textures.Count > 0 && Info.Textures[0].End == -1)
                SetupTextureInfos();
        }

        public int BlockTextureToTextureID(int blockTexture)
        {
            CheckTextureInfos();

            for (int i = 0; i < Info.Textures.Count; i++)
            {
                if (blockTexture >= Info.Textures[i].Start && blockTexture <= Info.Textures[i].End)
                    return i;
            }

            return -1;
        }

        public int BlockTextureToTextureOffset(int blockTexture)
        {
            int texture = BlockTextureToTextureID(blockTexture);
            if (texture < 0)
                return -1;

            return blockTexture - Info.Textures[texture].Start;
        }

        public class BlockDef
        {
            public string Name = string.Empty;

            public override string ToString()
            {
                return Name;
            }

            // defines the textures used
            public int Top = -1;
            public int[] Sides = null;
            public int Bottom = -1;

            public bool Transperant = false;

            public BlockDef() { }

            public BlockDef(string name, int top)
            {
                Name = name; 
                Top = top;
            }

            public BlockDef(string name, int top, int sides)
            {
                Name = name;
                Top = top;
                Sides = new int[1];
                Sides[0] = sides;
            }

            public BlockDef(string name, int top, int sides, int bottom)
            {
                Name = name; 
                Top = top;
                Sides = new int[1];
                Sides[0] = sides;
                Bottom = bottom;
            }

            public BlockDef(string name, int top, int north, int south, int east, int west, int bottom)
            {
                Name = name;
                Top = top;
                Sides = new int[4];
                Sides[0] = north;
                Sides[1] = south;
                Sides[2] = east;
                Sides[3] = west;

                Bottom = bottom;
            }

            public static int EmptyID = -1;
        }

        public List<BlockDef> BlockDefs = new List<BlockDef>();

        public int AddBlockDef(BlockDef def)
        {
            BlockDefs.Add(def);
            return BlockDefs.Count - 1;
        }

        [XmlIgnore]
        protected Octree octree;

        [XmlIgnore]
        public Dictionary<Cluster.ClusterPos, Cluster> Clusters = new Dictionary<Cluster.ClusterPos, Cluster>();

        public BoundingBox GetBounds()
        {
            return octree.bounds;
        }

        protected void Write(Stream fs)
        {
            XmlSerializer XML = new XmlSerializer(typeof(World));
            if (CompressFileIO)
            {
                GZipStream gz = new GZipStream(fs, CompressionMode.Compress);
                XML.Serialize(gz, this);
                gz.Close();
            }
            else
                XML.Serialize(fs, this);
        }

        public void Serialize(FileInfo location)
        {
            if (location.Exists)
                location.Delete();
            
            FileStream fs = location.OpenWrite();
            Write(fs);
            fs.Close();
        }

        public byte[] Serialize()
        {
            MemoryStream fs = new MemoryStream();
            Write(fs);
            fs.Close();
            return fs.GetBuffer();
        }

        protected static World Read(Stream fs)
        {
            XmlSerializer XML = new XmlSerializer(typeof(World));
            World world = null;
            if (CompressFileIO)
            {
                GZipStream gz = new GZipStream(fs, CompressionMode.Decompress);
                world = (World)XML.Deserialize(gz);
                gz.Close();
            }
            else
                world = (World)XML.Deserialize(fs);
            return world;
        }

        public static World Deserialize(FileInfo location)
        {
            if (!location.Exists)
                return World.Empty;

            try
            {
                FileStream fs = location.OpenRead();

                World world = Read(fs);
                fs.Close();
                return world;
            }
            catch (System.Exception /*ex*/)
            {
            }

            return World.Empty;
        }

        public static World Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return World.Empty;

            try
            {
                MemoryStream fs = new MemoryStream(buffer);
                World world = Read(fs);
                fs.Close();
                return world;
            }
            catch (System.Exception /*ex*/)
            {
            }

            return World.Empty;
        }

        public class WorldDefData
        {
            public byte[] WorldData = null;
            public List<byte[]> Clusters = new List<byte[]>();
            public List<byte[]> Geometries = new List<byte[]>();

            public byte[] Serialize()
            {
                MemoryStream fs = new MemoryStream();

                XmlSerializer XML = new XmlSerializer(typeof(WorldDefData));
                if (CompressFileIO)
                {
                    GZipStream gz = new GZipStream(fs, CompressionMode.Compress);
                    XML.Serialize(gz, this);
                    gz.Close();
                }
                else
                    XML.Serialize(fs, this);
                fs.Close();
                return fs.GetBuffer();
            }

            public static WorldDefData Deserialize(byte[] buffer)
            {
                MemoryStream fs = new MemoryStream(buffer);

                XmlSerializer XML = new XmlSerializer(typeof(WorldDefData));
                WorldDefData data = null;
                if (CompressFileIO)
                {
                    GZipStream gz = new GZipStream(fs, CompressionMode.Decompress);
                    data = (WorldDefData)XML.Deserialize(gz);
                    gz.Close();
                }
                else
                    data = (WorldDefData)XML.Deserialize(fs);
                return data;
            }
        }

        public static World ReadWorldAndClusters(FileInfo location)
        {
            World world = Deserialize(location);
            if (world == World.Empty)
                return World.Empty;

            DirectoryInfo dir = new DirectoryInfo(location.DirectoryName);
            foreach (FileInfo file in dir.GetFiles("*." + ClusterFileExtension))
            {
                try
                {
                    Cluster cluster = Cluster.Deserialize(file);
                    world.Clusters.Add(cluster.Origin, cluster);
                }
                catch (System.Exception /*ex*/)
                {

                }
            }
            world.Finailize();
            return world;
        }

        public static World ReadWorldAndClusters(WorldDefData data)
        {
            World world = Deserialize(data.WorldData);
            if (world == World.Empty)
                return World.Empty;

            foreach (byte[] buffer in data.Clusters)
            {
                try
                {
                    Cluster cluster = Cluster.Deserialize(buffer);
                    world.Clusters.Add(cluster.Origin, cluster);
                }
                catch (System.Exception /*ex*/)
                {

                }
            }
            world.Finailize();
            return world;
        }

        public static World ReadWorldWithGeometry(FileInfo location)
        {
            World world = ReadWorldAndClusters(location);
            if (world == World.Empty)
                return World.Empty;

            DirectoryInfo dir = new DirectoryInfo(location.DirectoryName);
            foreach (FileInfo file in dir.GetFiles("*." + GeometryFileExtension))
            {
                try
                {
                    ClusterGeometry geometry = ClusterGeometry.Deserialize(file);
                    if (world.Clusters.ContainsKey(geometry.ClusterOrigin))
                        world.Clusters[geometry.ClusterOrigin].Geometry = geometry;
                }
                catch (System.Exception /*ex*/)
                {

                }
            }
            return world;
        }

        public static World ReadWorldWithGeometry(WorldDefData data)
        {
            World world = ReadWorldAndClusters(data);
            if (world == World.Empty)
                return World.Empty;

            foreach (byte[] buffer in data.Geometries)
            {
                try
                {
                    ClusterGeometry geometry = ClusterGeometry.Deserialize(buffer);
                    if (world.Clusters.ContainsKey(geometry.ClusterOrigin))
                        world.Clusters[geometry.ClusterOrigin].Geometry = geometry;
                }
                catch (System.Exception /*ex*/)
                {

                }
            }
            return world;
        }

        public void SaveWorldAndClusters(FileInfo location)
        {
            Serialize(location);

            // kill all clusters in that folder
            foreach (FileInfo clusterFile in location.Directory.GetFiles("*." + ClusterFileExtension))
                clusterFile.Delete();

            foreach (Cluster c in Clusters.Values)
            {
                FileInfo file = new FileInfo(Path.Combine(location.DirectoryName,c.Origin.ToString() + "." + ClusterFileExtension));
                c.Serialize(file);
            }
        }

        public WorldDefData SaveWorldAndClusters()
        {
            WorldDefData data = new WorldDefData();

            data.WorldData = Serialize();

            foreach (Cluster c in Clusters.Values)
                data.Clusters.Add(c.Serialize());

            return data;
        }

        public void SaveWorldWithGeometry(FileInfo location)
        {
            SaveWorldAndClusters(location);

            // kill all clusters in that folder
            foreach (FileInfo clusterFile in location.Directory.GetFiles("*." + GeometryFileExtension))
                clusterFile.Delete();

            foreach (Cluster c in Clusters.Values)
                c.Geometry.Serialize(new FileInfo(Path.Combine(location.DirectoryName, c.Origin.ToString() + "." + GeometryFileExtension)));
        }

        public WorldDefData SaveWorldWithGeometry()
        {
            WorldDefData data = SaveWorldAndClusters();

            foreach (Cluster c in Clusters.Values)
                data.Geometries.Add(c.Geometry.Serialize());

            return data;
        }

        public List<Cluster> ClustersInFrustum(BoundingFrustum frustum, bool useOctree)
        {
            // super cheap
            List<Cluster> vis = new List<Cluster>();

            if (useOctree && octree != null)
                return InFrustum<Cluster>(frustum);
            else
            {
                foreach (KeyValuePair<Cluster.ClusterPos, Cluster> item in Clusters)
                {
                    if (frustum.Contains(item.Value.Bounds) != ContainmentType.Disjoint)
                        vis.Add(item.Value);
                }
            }
            return vis;
        }

        public void Finailize()
        {
            octree = new Octree();
            octree.Add(Clusters.Values);
        }

        public void FlushGeometry()
        {
            foreach (Cluster c in Clusters.Values)
                c.Geometry = null;
        }

        public void AddObject(IOctreeObject obj)
        {
            if (octree != null)
                octree.Add(obj);
        }

        public void RemoveObject(IOctreeObject obj)
        {
            if (octree != null)
                octree.FastRemove(obj);
        }

        public List<T> InFrustum<T>(BoundingFrustum frustum) where T : IOctreeObject
        {
            List<T> objects = new List<T>();

            if (octree != null)
            {
                List<object> v = new List<object>();
                octree.ObjectsInFrustum(v, frustum);
                foreach (object c in v)
                {
                    if (c.GetType() == typeof(T) || c.GetType().IsSubclassOf(typeof(T)))
                        objects.Add((T)c);
                }
            }
            return objects;
        }

        public List<T> InBoundingBox<T>(BoundingBox box) where T : IOctreeObject
        {
            List<T> objects = new List<T>();

            if (octree != null)
            {
                List<object> v = new List<object>();
                octree.ObjectsInBoundingBox(v, box);
                foreach (T c in v)
                    objects.Add(c);
            }
            return objects;
        }

        public List<T> InBoundingSphere<T>(BoundingSphere sphere) where T : IOctreeObject
        {
            List<T> objects = new List<T>();

            if (octree != null)
            {
                List<object> v = new List<object>();
                octree.ObjectsInBoundingSphere(v, sphere);
                foreach (T c in v)
                    objects.Add(c);
            }
            return objects;
        }

        protected int AxisToGrid(int value)
        {
            if (value >= 0)
                return (value / Cluster.XYSize) * Cluster.XYSize;

            return ((value - Cluster.XYSize) / Cluster.XYSize) * Cluster.XYSize;
        }

        public static Vector3 PositionToBlock(Vector3 pos)
        {
            return new Vector3((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y), (float)Math.Floor(pos.Z));
        }

        public Cluster.Block BlockFromPosition(int x, int y, int z)
        {
            if (z >= Cluster.ZSize || z < 0)
                return Cluster.Block.Invalid;

            Cluster.ClusterPos pos = new Cluster.ClusterPos(AxisToGrid(x), AxisToGrid(y));

            if (!Clusters.ContainsKey(pos))
                return Cluster.Block.Invalid;

            return Clusters[pos].GetBlockAbs(x, y, z);
        }

        public Cluster.Block BlockFromPosition(float x, float y, float z)
        {
            return BlockFromPosition((int)x, (int)y, (int)z);
        }

        public Cluster.Block BlockFromPosition(Vector3 pos)
        {
            return BlockFromPosition((int)pos.X, (int)pos.Y, (int)pos.Z);
        }

        public Cluster ClusterFromPosition(int x, int y, int z)
        {
            if (z >= Cluster.ZSize || z < 0)
                return null;
            return ClusterFromPosition(new Cluster.ClusterPos(AxisToGrid(x), AxisToGrid(y)));
        }

        public Cluster ClusterFromPosition(Cluster.ClusterPos pos)
        {
            if (!Clusters.ContainsKey(pos))
                return null;

            return Clusters[pos];
        }

        public Cluster ClusterFromPosition(float x, float y, float z)
        {
            return ClusterFromPosition((int)x, (int)y, (int)z);
        }

        public Cluster ClusterFromPosition(Vector3 pos)
        {
            return ClusterFromPosition((int)pos.X, (int)pos.Y, (int)pos.Z);
        }

        public bool PositionIsOffMap(float x, float y, float z)
        {
            return PositionIsOffMap((int)x, (int)y, (int)z);
        }

        public bool PositionIsOffMap(Vector3 pos)
        {
            return PositionIsOffMap((int)pos.X, (int)pos.Y, (int)pos.Z);
        }

        public bool PositionIsOffMap(int x, int y, int z)
        {
            if (z >= Cluster.ZSize || z < 0)
                return true;

            Cluster.ClusterPos pos = new Cluster.ClusterPos(AxisToGrid(x), AxisToGrid(y));

            if (!Clusters.ContainsKey(pos))
                return true;

            return false;
        }

        public float DropDepth(Vector2 position)
        {
            return DropDepth(position.X, position.Y);
        }

        public float DropDepth(float positionX, float positionY)
        {
            Cluster.ClusterPos pos = new Cluster.ClusterPos(AxisToGrid((int)positionX), AxisToGrid((int)positionY));
            if (!Clusters.ContainsKey(pos))
                return float.MinValue;

            Cluster c = Clusters[pos];
            int x = (int)positionX - pos.X;
            int y = (int)positionY - pos.Y;

            float blockX = positionX - x;
            float blockY = positionY - y;

            for (int z = Cluster.ZSize - 1; z >= 0; z--)
            {
                float value = c.GetBlockRelative(x, y, z).GetZForLocalPosition(blockX, blockY);
                if (value != float.MinValue)
                    return z + value;
            }

            return float.MinValue;
        }

        public bool BlockPositionIsPassable(Vector3 pos)
        {
            return BlockPositionIsPassable(pos, null);
        }

        protected void CheckPlanes()
        {
            if (CollisionPlanes.Count != 0)
                return;

            Vector3 vec = new Vector3(0,-1,1);
            vec.Normalize();

            CollisionPlanes.Add(Cluster.Block.Geometry.NorthFullRamp, new Plane(vec, 0));

            vec = new Vector3(0, 1, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.SouthFullRamp, new Plane(vec, 1));

            vec = new Vector3(-1, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.EastFullRamp, new Plane(vec, 0));

            vec = new Vector3(1, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.WestFullRamp, new Plane(vec, 1));


            vec = new Vector3(0, -0.5f, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.NorthHalfLowerRamp, new Plane(vec, 0));

            vec = new Vector3(0, 0.5f, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.SouthHalfLowerRamp, new Plane(vec, 0.5f));

            vec = new Vector3(-0.5f, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.EastHalfLowerRamp, new Plane(vec, 0));

            vec = new Vector3(0.5f, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.WestHalfLowerRamp, new Plane(vec, 0.5f));


            vec = new Vector3(0, -0.5f, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.NorthHalfUpperRamp, new Plane(vec, 0.5f));

            vec = new Vector3(0, 0.5f, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.SouthHalfUpperRamp, new Plane(vec, 1));

            vec = new Vector3(-0.5f, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.EastHalfUpperRamp, new Plane(vec, 0.5f));

            vec = new Vector3(0.5f, 0, 1);
            vec.Normalize();
            CollisionPlanes.Add(Cluster.Block.Geometry.WestHalfUpperRamp, new Plane(vec, 1));
        }

        public bool BlockPositionIsPassable(Vector3 pos, Cluster.Block block, Vector3 blockPos, CollisionInfo info)
        {
            CheckPlanes();

            if (block == Cluster.Block.Empty || block == Cluster.Block.Invalid || block.Geom == Cluster.Block.Geometry.Empty)
                return true;

            if (block.Geom == Cluster.Block.Geometry.Solid)
                return false;

            if (block.Geom == Cluster.Block.Geometry.Fluid)
                return true;

            int X = (int)blockPos.X;
            int Y = (int)blockPos.Y;
            int Z = (int)blockPos.Z;

            Vector3 relPos = pos - blockPos;
            
            switch (block.Geom)
            {
                case Cluster.Block.Geometry.HalfUpper:
                    return relPos.Z < 0.5f;

                case Cluster.Block.Geometry.HalfLower:
                    return relPos.Z >= 0.5f;
            }

            Plane plane = CollisionPlanes[block.Geom];

            if (info != null)
                info.ClipPlane = plane;

            if (plane.IntersectsPoint(relPos) == PlaneIntersectionType.Front)
                return true;

            return false;
        }

        public bool BlockPositionIsPassable(Vector3 pos, CollisionInfo info)
        {
            Cluster.Block block = BlockFromPosition(pos);
            Vector3 blockPos = PositionToBlock(pos);

            return BlockPositionIsPassable(pos, block, blockPos, info);
        }

        public class CollisionInfo
        {
            public Cluster.Block CollidedBlock = Cluster.Block.Empty;
            public float Lenght = 0f;
            public Vector3 CollidedBlockPosition = Vector3.Zero;
            public Vector3 CollisionLocation = Vector3.Zero;

            public Plane ClipPlane = Plane.Empty;
            public float StartLen = 0;

            public bool Collided = true;

            public CollisionInfo NoCollide() { Collided = false; return this; }
        }

        public CollisionInfo LineCollidesWithWorld(Vector3 start, Vector3 end)
        {
            CollisionInfo info = new CollisionInfo();

            // figure out the axis with the longest delta
            Vector3 delta = end - start;
            float lenght = delta.Length;
            delta.Normalize();
            int axis = 0;
            float max = Math.Abs(delta.X);
            if (Math.Abs(delta.Y) > max)
            {
                axis = 1;
                max = Math.Abs(delta.Y);
            }
            if (Math.Abs(delta.Z) > max)
            {
                axis = 2;
                max = Math.Abs(delta.Z);
            }

            info.CollidedBlock = BlockFromPosition(start);
            info.CollidedBlockPosition = PositionToBlock(start);
            info.CollisionLocation = start;

            if (max < 0.001f) // the ray is too small to test in any axis
            {
                if (BlockPositionIsPassable(start))
                    return info.NoCollide();
                else
                    return info;
            }

            if (!BlockPositionIsPassable(start))
                return info;
            
            info.CollidedBlock = BlockFromPosition(end);
            info.Lenght = lenght;
            info.CollidedBlockPosition = PositionToBlock(end);
            info.CollisionLocation = end; // TODO, run the vector back untill an axis hits a valid number so we get the first edge hit

            if(!BlockPositionIsPassable(end))
                return info;

            Vector3 NewStart = Vector3.Zero;
            Vector3 newDelta = Vector3.Zero;

            float maxSegments = 0;

            float newStartParam = 0;
            // find the start point;
            if (axis == 0)
            {
                newStartParam = (((int)start.X + 1) - start.X) / delta.X;
                NewStart = start + delta * newStartParam;

                float param = 1f / delta.X;
                newDelta = delta * param;

                maxSegments = Math.Abs(end.X - start.X);
            }
            else if (axis == 1)
            {
                newStartParam = (((int)start.Y + 1) - start.Y) / delta.Y;
                NewStart = start + delta * newStartParam;

                float param = 1f / delta.Y;
                newDelta = delta * param;

                maxSegments = Math.Abs(end.Y - start.Y);
            }
            else 
            {
                newStartParam = (((int)start.Z + 1) - start.Z) / delta.Z;
                NewStart = start + delta * newStartParam;

                float param = 1f / delta.Z;
                newDelta = delta * param;

                maxSegments = Math.Abs(end.Z - start.Z);
            }

            info.StartLen = newStartParam;
            info.CollidedBlockPosition = PositionToBlock(NewStart);
            info.CollisionLocation = NewStart; // TODO run back
            info.CollidedBlock = BlockFromPosition(NewStart);
            info.Lenght = newStartParam;

            if (!BlockPositionIsPassable(NewStart))
                return info;

            float deltaLen = newDelta.Length;

            Vector3 lastPos = NewStart;

            for (float f = 1; f < maxSegments; f += 1f)
            {
                NewStart += newDelta;
                Vector3 blockPos = PositionToBlock(NewStart);

                if (PositionIsOffMap(blockPos)) // we are done as soon as we go off the map
                    return info.NoCollide();

                Cluster.Block block = BlockFromPosition(blockPos);

                info.CollidedBlock = block;
                info.CollidedBlockPosition = blockPos;
                info.CollisionLocation = lastPos; // TODO run back

                if (!BlockPositionIsPassable(lastPos, block, blockPos, info))
                    return info;

                info.Lenght += deltaLen;
                info.CollisionLocation = NewStart;
                if (!BlockPositionIsPassable(NewStart, block, blockPos, info))
                    return info;

                lastPos = NewStart;
            }

            return info.NoCollide();
        }
    }
}

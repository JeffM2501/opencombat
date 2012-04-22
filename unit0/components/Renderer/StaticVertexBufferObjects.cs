using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class StaticVertexBufferObject : IDisposable
    {
        public class BufferData
        {
            public InterleavedArrayFormat Format = InterleavedArrayFormat.V3f;
            public byte ElementSize = 2;
            public byte VertexSize = sizeof(float)*3;

            public int VertexCount = 0;
            public int ElementCount = 0;

            public object ElementList = null;
            public object VertexList = null;

            public DrawElementsType ElementType = DrawElementsType.UnsignedShort;
            public BeginMode GeometryType = BeginMode.Triangles;

            public virtual BufferData Pack() { return this; }

            public static BufferData Empty = new BufferData();
        }

        public class ShortIndexFullVertBuffer : BufferData
        {
            public ShortIndexFullVertBuffer()
                : base()
            {
                Format = InterleavedArrayFormat.T2fC4fN3fV3f;
                VertexSize = sizeof(float) * (2 + 4 + 3 + 3);
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct VertexData
            {
                public Vector2 UV;
                public Vector4 Color;
                public Vector3 Normal;
                public Vector3 Vertex;

                public VertexData(Vector2 uv, System.Drawing.Color color, Vector3 normal, Vector3 vertex)
                {
                    UV = uv;
                    Color = ToRgba(color);
                    Normal = normal;
                    Vertex = vertex;
                }

                public static Vector4 ToRgba(System.Drawing.Color color)
                {
                    return new Vector4(color.R/255f,color.G/255f,color.B/255f,color.A/255f);
                }
            }

            public List<VertexData> Verts = new List<VertexData>();
            public List<short> Indexes = new List<short>();

            public VertexData[] Data = null;
            public short[] Elements = null;

            public override BufferData Pack()
            {
                Data = Verts.ToArray();
                Elements = Indexes.ToArray();

                VertexCount = Verts.Count;
                ElementCount = Indexes.Count;

                VertexList = Verts.ToArray();
                ElementList = Indexes.ToArray();
                return this;
            }
        }

        public delegate BufferData VertexBufferEvent(StaticVertexBufferObject obj);
        public VertexBufferEvent GetData;

        protected InterleavedArrayFormat Format;
        protected int ElementSize = 0;
        protected int DataSize = 0;
        protected BeginMode GeometryMode = BeginMode.Triangles;
        protected DrawElementsType ElementType = DrawElementsType.UnsignedShort;
        protected int ElementCount = 0;
        protected int VertexSize = 0;

        protected bool Invalid = false;

        public object Tag = null;

        protected static int CurrentContext = 0;

        protected static Dictionary<int,List<StaticVertexBufferObject>> InitalCache()
        {
            Dictionary<int, List<StaticVertexBufferObject>> d = new Dictionary<int, List<StaticVertexBufferObject>>();
            d.Add(0, new List<StaticVertexBufferObject>());
            return d;
        }

        protected static Dictionary<int, List<StaticVertexBufferObject>> Caches = InitalCache();
        protected static List<StaticVertexBufferObject> Cache()
        {
            lock (Caches)
            {
                return Caches[CurrentContext];
            }
        }

        public static void FlushGL()
        {
            List<StaticVertexBufferObject> cache = Cache();
            lock (cache)
            {
                foreach (StaticVertexBufferObject o in cache)
                    o.Invalidate();
            }
        }

        public static void KillAll()
        {
            FlushGL();

            List<StaticVertexBufferObject> cache = Cache();
            lock (cache)
            {
                cache.Clear();
            }
        }

        protected static int lastContextID = 0;

        public static int NewContext()
        {
            lastContextID++;
            AddContext(lastContextID);
            return lastContextID;
        }

        protected static void AddContext(int id)
        {
            lock (Caches)
            {
                Caches.Add(lastContextID, new List<StaticVertexBufferObject>());
                CurrentContext = id;
            }
        }

        public static void SetContext(int context)
        {
            CurrentContext = context;
        }

        public static void RemoveContext(int context)
        {
            if (context == 0)
                return;

            CurrentContext = context;
            FlushGL();

            CurrentContext = 0;

            lock (Caches)
                Caches.Remove(context);
        }

        public static void ContextCreated(int contextID)
        {
            AddContext(contextID);
        }

        public static void ContextDestroyed(int contextID)
        {
            RemoveContext(contextID);
        }

        public static void ContextChanged(int contextID)
        {
            SetContext(contextID);
        }

        public static void ContextUnload(int contextID)
        {
            FlushGL();
        }

        public static void ContextReload(int contextID)
        {

        }

        public void Invalidate()
        {
            if (Buffers != null)
                GL.DeleteBuffers(2, Buffers);

            Invalid = false;
            Buffers = null;
        }

        public void Dispose()
        {
            Invalidate();
            List<StaticVertexBufferObject> cache = Cache();
            lock (cache)
            {
                cache.Remove(this);
            }
        }

        protected uint[] Buffers = null;

        public StaticVertexBufferObject(VertexBufferEvent callback)
        {
            GetData = callback;
            List<StaticVertexBufferObject> cache = Cache();

            lock (cache)
            {
                cache.Add(this);
            }
        }

        public void Draw()
        {
            if (Invalid)
                return;

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            if (Buffers == null)
            {
                if (GetData == null)
                    return;

                BufferData data = GetData(this);

                if (data == BufferData.Empty)
                    Invalid = true;

                if (data as ShortIndexFullVertBuffer != null)
                {
                    ShortIndexFullVertBuffer b = data as ShortIndexFullVertBuffer;

                    ElementType = data.ElementType;
                    ElementCount = data.ElementCount;
                    GeometryMode = b.GeometryType;
                    Format = b.Format;
                    ElementSize = data.ElementSize;
                    DataSize = data.VertexCount;
                    VertexSize = data.VertexSize;

                    Buffers = new uint[2];
                    GL.GenBuffers(2, Buffers);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffers[0]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(ElementCount * ElementSize), b.Elements, BufferUsageHint.StaticDraw);

                    int size;
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
                    if (data.ElementCount * sizeof(short) != size)
                        throw new ApplicationException("Element data not uploaded correctly");

                    GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers[1]);

                    GL.InterleavedArrays(data.Format, 0, IntPtr.Zero);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(DataSize * VertexSize), b.Data, BufferUsageHint.StaticDraw);

                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                    if (DataSize * VertexSize != size)
                        throw new ApplicationException("ArrayBuffer data not uploaded correctly");
                }
                else
                    return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffers[1]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffers[0]);

            GL.InterleavedArrays(Format, 0, IntPtr.Zero);

            GL.DrawElements(BeginMode.Triangles, ElementCount, ElementType, IntPtr.Zero);
        }
    }
}

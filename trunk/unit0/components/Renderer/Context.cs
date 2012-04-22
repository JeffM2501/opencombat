using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Textures;

namespace Renderer
{
    public class Context
    {
        protected static List<int> Contexts = new List<int>();
        protected static int LastContext = 0;

        protected static int Current = 0;
        public static int CurrentContext { get { return Current; } }

        public delegate void ContextEvent(int contextID);

        public static event ContextEvent ContextCreated = null;
        public static event ContextEvent ContextDestroyed = null;
        public static event ContextEvent ContextChanged = null;

        public static event ContextEvent ContextUnload = null;
        public static event ContextEvent ContextReload = null;

        public static void LinkSubSystems()
        {
            ContextCreated += Texture.ContextCreated;
            ContextDestroyed += Texture.ContextDestroyed;
            ContextChanged += Texture.ContextChanged;
            ContextUnload += Texture.ContextUnload;
            ContextReload += Texture.ContextReload;

            ContextCreated += DisplayList.ContextCreated;
            ContextDestroyed += DisplayList.ContextDestroyed;
            ContextChanged += DisplayList.ContextChanged;
            ContextUnload += DisplayList.ContextUnload;
            ContextReload += DisplayList.ContextReload;

            ContextCreated += StaticVertexBufferObject.ContextCreated;
            ContextDestroyed += StaticVertexBufferObject.ContextDestroyed;
            ContextChanged += StaticVertexBufferObject.ContextChanged;
            ContextUnload += StaticVertexBufferObject.ContextUnload;
            ContextReload += StaticVertexBufferObject.ContextReload;
        }

        public static int NewContext()
        {
            LastContext++;
            Contexts.Add(LastContext);

            if (ContextCreated != null)
                ContextCreated(Current);

            return LastContext;
        }

        public static void RemoveContext()
        {
            if (Current == 0)
                return;

            if (ContextCreated != null)
                ContextDestroyed(Current);

            SetContext(0);
        }

        public static void SetContext( int context)
        {
            if (!Contexts.Contains(context) || context == Current)
                return;

            Current = context;

            if (ContextChanged != null)
                ContextChanged(Current);
        }

        public static void UnloadContext()
        {
            if (ContextUnload != null)
                ContextUnload(Current);
        }

        public static void LoadContext()
        {
            if (ContextReload != null)
                ContextReload(Current);
        }
    }
}

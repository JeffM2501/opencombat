using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class DisplayList : IDisposable
    {
        public delegate void GenerateListEvent(DisplayList list);

        public object Tag = null;

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

        public static void FlushGL()
        {
            List<DisplayList> lists = ListCache();

            lock (lists)
            {
                foreach (DisplayList list in lists)
                    list.Invalidate();
            }
        }

        public static void KillAll()
        {
            FlushGL();

            List<DisplayList> lists = ListCache();

            lock (lists)
            {
                lists.Clear();
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
            lock (ListCaches)
            {
                ListCaches.Add(lastContextID, new List<DisplayList>());
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

            lock (ListCaches)
                ListCaches.Remove(context);
        }

        protected static Dictionary<int, List<DisplayList>> InitalList()
        {
            Dictionary<int, List<DisplayList>> l = new Dictionary<int, List<DisplayList>>();
            l.Add(0, new List<DisplayList>());
            return l;
        }

        protected static List<DisplayList> ListCache()
        {
            lock (ListCaches)
                return ListCaches[CurrentContext];
        }

        protected static int CurrentContext = 0;
        protected static Dictionary<int, List<DisplayList>> ListCaches = InitalList();

        protected event GenerateListEvent Generate;

        protected static int InvalidList = -1;
        protected int List = InvalidList;

        public DisplayList ( GenerateListEvent e )
        {
            Generate = e;
            List<DisplayList> lists = ListCache();

            lock (lists)
            {
                lists.Add(this);
            }
        }

        public void Dispose()
        {
            Invalidate();
            List<DisplayList> lists = ListCache();
            lock (lists)
            {
                lists.Remove(this);
            }
        }

        public void Invalidate ()
        {
            if (List != InvalidList)
                GL.DeleteLists(List, 1);

            List = InvalidList;
        }

        public void Call ()
        {
            if (Generate == null)
                return;

            if (List == InvalidList)
            {
                List = GL.GenLists(1);
                GL.NewList(List, ListMode.CompileAndExecute);

                Generate(this);

                GL.EndList();
            }
            else
                GL.CallList(List);
        }
    }
}

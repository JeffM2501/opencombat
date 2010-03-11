using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

namespace GUIObjects
{
    public class GUIObject
    {
        public GUIObject Parrent;
        public string ValueName = string.Empty;

        public GlobalValue Value = GlobalValue.Empty;

        public String Name = string.Empty;

        public List<GUIObject> Children = new List<GUIObject>();

        public RectangleF Bounds = RectangleF.Empty;
        
        protected Point position = Point.Empty;
        public Point Poisition
        {
            get {return position;}
            set {position = value;Moved();}
        }

        protected Size size = Size.Empty;
        public Size Size
        {
            get {return size;}
            set {size = value;Resized();}
        }

        protected bool hasFocus = false;

        public bool Focus
        {
            get { return hasFocus; }
            set { hasFocus = value; FocusChange();}
        }

        public Color BackgroundColor = Color.Transparent;
        public Color ForegroundColor = Color.Black;

        public virtual void Think ( double time )
        {
            foreach (GUIObject child in Children)
                child.Think(time);
        }

        public virtual void Draw ( double time )
        {
            foreach (GUIObject child in Children)
                child.Draw(time);
        }

        public virtual void Activate ( bool show )
        {
            foreach (GUIObject child in Children)
                child.Activate(show);
        }

        protected virtual void FocusChange()
        {
            foreach (GUIObject child in Children)
                child.FocusChange();
        }

        protected virtual void Moved()
        {
            foreach (GUIObject child in Children)
                child.Moved();
        }

        protected virtual void Resized()
        {
            foreach (GUIObject child in Children)
                child.Resized();
        }

        public virtual void Bind ()
        {
            Value = GlobalValueCache.FindValue(ValueName);
            foreach (GUIObject child in Children)
            {
                child.Parrent = this;
                child.Bind();
            }
        }

        public class ElementDefinition
        {
            public string Name = string.Empty;
            public string ValueName = string.Empty;
            public Point Position = Point.Empty;
            public Size Size = Size.Empty;

            public string BackgroundColor = string.Empty;
            public string ForegroundColor = string.Empty;

            public List<ElementDefinition> Children;

            public List<KeyValuePair<string, string>> Options = new List<KeyValuePair<string, string>>();
        }

        protected Color ParseColor (string val)
        {
            try
            {
                return Color.FromName(val);
            }
            catch (System.Exception /*ex*/)
            {
                int a = 0;
                int r = 0;
                int g = 0;
                int b = 0;

                string[] split = val.Split(" ".ToCharArray());
                if (split.Length > 0)
                    int.TryParse(split[0], out r);
                if (split.Length > 1)
                    int.TryParse(split[1], out g);
                if (split.Length > 2)
                    int.TryParse(split[2], out b);
                if (split.Length > 3)
                    int.TryParse(split[3], out a);

                return Color.FromArgb(a, r, g, b);
            }
        }

        protected string GetColorName ( Color color )
        {
            if (color.Name != string.Empty)
                return color.Name;

            return color.R.ToString() + " " + color.G.ToString() + " " + color.B.ToString() + " " + color.A.ToString();
        }

        protected virtual void WriteExtraDefInfo(ElementDefinition def)
        {

        }

        public virtual ElementDefinition GetDefinition()
        {
            ElementDefinition element = new ElementDefinition();
            element.Name = Name;
            if (Value != GlobalValue.Empty)
                element.ValueName = Value.Name;
            element.Position =Poisition;
            element.Size = Size;
            element.BackgroundColor = GetColorName(BackgroundColor);
            element.ForegroundColor = GetColorName(ForegroundColor);

            WriteExtraDefInfo(element);

            foreach (GUIObject child in Children)
                element.Children.Add(child.GetDefinition());

            return element;
        }

        protected virtual void ReadExtraDefInfo (ElementDefinition def)
        {

        }

        public virtual void CreateFromDefinition(ElementDefinition def)
        {
            ValueName = def.ValueName;
            Poisition = def.Position;
            Size = def.Size;
            BackgroundColor = ParseColor(def.BackgroundColor);
            ForegroundColor = ParseColor(def.ForegroundColor);
            ReadExtraDefInfo(def);

            foreach (ElementDefinition childDef in def.Children)
            {
                GUIObject childObject = new GUIObject();
                if (GUIObjectManager.Components.ContainsKey(childDef.Name))
                    childObject = (GUIObject)Activator.CreateInstance(GUIObjectManager.Components[childDef.Name]);

                childObject.CreateFromDefinition(childDef);
                Children.Add(childObject);
            }
        }
    }

   

    public class GUIObjectManager
    {
        public static Dictionary<string, Type> Components = new Dictionary<string, Type>();
        public static Dictionary<string, GUIObject> Elements = new Dictionary<string, GUIObject>();

        public static void AddDefaultElements()
        {

        }

        public static void SaveAllElements ( DirectoryInfo dir )
        {
            if (!dir.Exists)
                return;

            foreach (KeyValuePair<string,GUIObject> element in Elements)
            {
                FileInfo file = new FileInfo(Path.Combine(dir.FullName, element.Key + ".xml"));
                SaveElementToDisk(file, element.Value);
            }
        }

        public static void SaveElementToDisk ( FileInfo file, GUIObject obj )
        {
            Stream stream = file.OpenWrite();
            XmlSerializer XML = new XmlSerializer(typeof(GUIObject.ElementDefinition));
            XML.Serialize(stream, obj.GetDefinition());
            stream.Close();
        }

        public static void AddElementsFromDirectory( DirectoryInfo dir )
        {
            foreach (FileInfo file in dir.GetFiles("*.xml"))
                ReadElementFromDisk(file);
        }

        public static void ReadElementFromDisk(FileInfo file)
        {
            if (!file.Exists)
                return;

            Stream stream = file.OpenRead();
            XmlSerializer XML = new XmlSerializer(typeof(GUIObject.ElementDefinition));
            AddElement((GUIObject.ElementDefinition)XML.Deserialize(stream));
            stream.Close();
        }

        public static void AddDefaultComponents()
        {
            AddComonentsFromAssembly(Assembly.GetCallingAssembly());
        }

        public static void AddComonentsFromAssembly ( Assembly assembly )
        {
            foreach (Type t in assembly.GetTypes())
                AddComponent(t);
        }

        public static void AddElement(GUIObject.ElementDefinition element)
        {
            GUIObject parrent = ElementToObject(element);
            if (Elements.ContainsKey(parrent.Name))
                Elements[parrent.Name] = parrent;
            else
                Elements.Add(parrent.Name, parrent);

            parrent.Bind();
        }

        protected static GUIObject ElementToObject ( GUIObject.ElementDefinition def )
        {
            GUIObject element = new GUIObject();

            if(Components.ContainsKey(def.Name))
                element = (GUIObject)Activator.CreateInstance(Components[def.Name]);

            element.CreateFromDefinition(def);

            return element;
        }

        public static void AddComponent ( Type component )
        {
            if (!component.IsSubclassOf(typeof(GUIObject)))
                return;

            GUIObject tmp = (GUIObject)Activator.CreateInstance(component);

            if (tmp.Name == string.Empty)
                return;

            if (Components.ContainsKey(tmp.Name))
                Components[tmp.Name] = component;
            else
                Components.Add(tmp.Name, component);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using System.Xml;
using System.Xml.Serialization;

using OpenTK;
using OpenTK.Graphics;

using Textures;
using Renderer;
using FileLocations;

using Game;

namespace Client.Hud
{
    public class ViewBounds
    {
        protected Size _Bounds = new Size();

        public Size Bounds
        {
            get { return _Bounds; }
            set { _Bounds = value; if (BoundsChanged != null) BoundsChanged(this, EventArgs.Empty); }
        }

        public event EventHandler<EventArgs> BoundsChanged;

        public ViewBounds(Size s)
        {
            _Bounds = s;
        }
    }

    public class ChatInfo
    {
        public class ChatMessage
        {
            public UInt64 From = UInt64.MaxValue;
            public string Text = string.Empty;

            public ChatMessage(UInt64 from, string text)
            {
                From = from;
                Text = text;
            }
        }

        public void AddMessage(UInt64 from, string text)
        {
            lock (ChatMessages)
                ChatMessages.Add(new ChatMessage(from, text));
        }

        public void TrimChat(int maxValue)
        {
            lock (ChatMessages)
            {
                if (ChatMessages.Count > maxValue)
                    ChatMessages.RemoveRange(0, maxValue);
            }
        }

        public ChatMessage GetRecentMessage(int index)
        {
            lock (ChatMessages)
            {
                int realIndex = ChatMessages.Count - index - 1;
                if (realIndex < 0 || realIndex >= ChatMessages.Count)
                    return null;

                return ChatMessages[realIndex];
            }
        }

        protected List<ChatMessage> ChatMessages = new List<ChatMessage>();

        public class ChatUser
        {
            public string Name = "Unknown";
            public Texture Icon = null;
            public UInt64 UID = UInt64.MaxValue;

            public static ChatUser Empty = new ChatUser();
        }

        protected Dictionary<UInt64, ChatUser> ChatUsers = new Dictionary<UInt64, ChatUser>();

        public void AddUser(ChatUser user)
        {
            lock (ChatUsers)
            {
                if (ChatUsers.ContainsKey(user.UID))
                    ChatUsers[user.UID] = user;
                else
                    ChatUsers.Add(user.UID, user);
            }
        }

        public ChatUser GetUser(UInt64 uid)
        {
            lock (ChatUsers)
            {
                if (ChatUsers.ContainsKey(uid))
                    return ChatUsers[uid];
                
            }
            return ChatUser.Empty;
        }

        public static UInt64 GameChatUID = UInt64.MaxValue - 2;
        public static UInt64 ErrorChatUID = UInt64.MaxValue - 3;

        protected string _OutboundLine = string.Empty;

        public string OutboundLine
        {
            get
            {
                lock (_OutboundLine)
                    return (string)_OutboundLine.Clone();
            }

            set
            {
                lock (_OutboundLine)
                    _OutboundLine = value;
            }
        }
    }
    

    public class HudRenderer
    {
        Font anouncementFont = new Font(FontFamily.GenericSansSerif, 24);
        Font chatFont = new Font(FontFamily.GenericSerif, 12);
        public TextPrinter printer = new TextPrinter(TextQuality.High);

        PlayerListPannelRenderer.PlayerList playerUIList = new PlayerListPannelRenderer.PlayerList();

        public ChatInfo Chat = new ChatInfo();

        public class TextItem
        {
            public PannelRenderer render = null;
            public PannelElement element = null;
            public bool dynamic = true;

            public TextItem(PannelRenderer r, PannelElement e)
            {
                render = r;
                element = e;
            }
            public TextItem(PannelRenderer r, PannelElement e, bool d)
            {
                render = r;
                element = e;
                dynamic = d;
            }
        }

        protected static List<TextItem> textItems = new List<TextItem>();
        public static TextItem AddTextItem(TextItem item)
        {
            if (!textItems.Contains(item))
                textItems.Add(item);

            return item;
        }

        public static bool RemoveTextItem(PannelElement e)
        {
            TextItem killer = null;

            foreach (TextItem t in textItems)
            {
                if (e == t.element)
                    killer = t;
            }
            if (killer != null)
                textItems.Remove(killer);

            return killer != null;
        }
        public List<PannelElement> elements = new List<PannelElement>();

        public ViewBounds WindowBounds = null;

        public double RenderFrequency = 1;
        public double TargetRenderFrequency = 1;
        public double UpdateFrequency = 1;

        public HudRenderer(ViewBounds bounds)
        {
            WindowBounds = bounds;
        }

        public void Init()
        {
            LoadPannelDefs();

            LoadUI();
            HookUpUILinks();
        }

        public void ShowDialog(string name, string paramaters)
        {
            lock (elements)
            {
                foreach (PannelElement element in elements)
                {
                    if (element.name == name)
                    {
                        element.enabled = true;
                        element.dialogParamaters = paramaters;
                        element.GetRenderer().ShowDialog(element, paramaters);
                    }
                }
            }
        }

        void PlayerManager_PlayerAdded(string name, string team)
        {
            lock (playerUIList)
            {
                playerUIList.items.Add(new PlayerListPannelRenderer.PlayerList.Item(name));
            }
        }

        void PlayerManager_PlayerRemoved(string name, string team)
        {
            lock (playerUIList)
            {
                PlayerListPannelRenderer.PlayerList.Item toKill = null;
                foreach (PlayerListPannelRenderer.PlayerList.Item item in playerUIList.items)
                {
                    if (item.name == name)
                    {
                        toKill = item;
                        break;
                    }
                }
                playerUIList.items.Remove(toKill);
            }
        }

        public PannelElement FindByName(string name)
        {
            foreach (PannelElement e in elements)
            {
                PannelElement r = HasName(e, name);
                if (r != null)
                    return r;
            }

            return null;
        }

        protected PannelElement HasName(PannelElement e, string name)
        {
            if (e.name == name)
                return e;

            foreach (PannelElement child in e.children)
            {
                PannelElement r = HasName(child, name);
                if (r != null)
                    return r;
            }

            return null;
        }

        public void HideAll()
        {
            foreach (PannelElement e in elements)
                e.enabled = false;
        }

        public void ShowAll()
        {
            foreach (PannelElement e in elements)
                e.enabled = true;
        }

        public void Show(string name)
        {
            PannelElement e = FindByName(name);
            if (e != null)
                e.enabled = true;
        }

        public void Hide(string name)
        {
            PannelElement e = FindByName(name);
            if (e != null)
                e.enabled = false;
        }

        public event EventHandler<EventArgs> LoadElmentRenderers;

        protected void LoadPannelDefs()
        {
            if (LoadElmentRenderers != null)
                LoadElmentRenderers(this, EventArgs.Empty);

//             new FrameRenderer("window", "ui/xp_32.png");
//             new FrameRenderer("button", "ui/xp_32_button.png");
//             new FrameRenderer("frame", "ui/16_white_rounded.png");
            new TextLabelRenderer(chatFont, "chatFontLabel");
            new TextLabelRenderer(anouncementFont, "anouncementFontLabel");
            new TextEditRenderer(chatFont, "chatFontTextEdit");
            new ChatPannelRenderer(chatFont, Color.Blue, Color.Red, "chatWindow");
            new ImagePannelRenderer("image");
            new SizeableChatBoxFrame("chatbox","ui/chatbox.png");
        }

        public event EventHandler<EventArgs> LoadGUIElements;

        protected void LoadUI()
        {
            if (LoadGUIElements != null)
                LoadGUIElements(this, EventArgs.Empty);
        }

        public PannelElement NewElement(string name, string type)
        {
            PannelElement element = new PannelElement();
            element.name = name;
            element.pannelType = type;

            return element;
        }

        public PannelElement LoadElement(PannelElement element)
        {
            if (element != null)
            {
                element.view = WindowBounds;
                element.Adopt();
                elements.Add(element);
            }
            return element;
        }

        public PannelElement LoadElement(string path)
        {
            string item = Locations.FindDataFile(path);
            if (item == string.Empty)
                return null;

            FileStream fs = null;
            PannelElement e = null;
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(PannelElement));

                fs = new FileStream(item, FileMode.Open, FileAccess.Read);
                e =  LoadElement((PannelElement)xml.Deserialize(fs));
            }
            catch (System.Exception /*ex*/)
            {

            }

            if (fs != null)
                fs.Close();
            return e;
        }

        protected void HookUpUILinks()
        {
            foreach (PannelElement element in elements)
                LinkUpdates(element);
        }

        protected void LinkUpdates(PannelElement element)
        {
            if (element.updateFunction != string.Empty)
            {
                if (element.updateFunction == "FPS")
                    element.Update = FPSUpdater;
                else if (element.updateFunction == "UPDATES")
                    element.Update = UpdateUpdater;
                else if (element.updateFunction == "WAIT_MESSAGE")
                    element.Update = WaitMessage;
                else if (element.updateFunction == "CHAT_LOG")
                    element.Update = ChatLogUpdater;
                else if (element.updateFunction == "PLAYER_LIST")
                    element.Update = PlayerListUpdater;
            }
            foreach (PannelElement child in element.children)
                LinkUpdates(child);
        }

        protected void PlayerListUpdater(PannelElement element)
        {
            PlayerListPannelRenderer.PlayerList list = element.RenderTag as PlayerListPannelRenderer.PlayerList;
            if (list == null)
                element.RenderTag = playerUIList;
        }

        protected void FPSUpdater(PannelElement element)
        {
            double v = TargetRenderFrequency;
            if (v < 1)
                v = RenderFrequency;

            int f = (int)(v + 0.5f);

            element.text = f.ToString();
        }

        protected void UpdateUpdater(PannelElement element)
        {
            element.text = UpdateFrequency.ToString();
        }

        protected void WaitMessage(PannelElement element)
        {
           // element.text = view.waitMessage;
        }

        protected void ChatLogUpdater(PannelElement element)
        {
            element.RenderTag = Chat;
        }

        public void Render(double now, double frameTime)
        {
            lock (elements)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Normal3(0, 0, 1);

                GL.PushMatrix();
                GL.Translate(0, 0, -1);

                foreach (PannelElement element in elements)
                    element.Draw(now, frameTime);
                GL.PopMatrix();

                printer.Begin();

                List<TextItem> toKill = new List<TextItem>();

                foreach (TextItem item in textItems)
                {
                    if (item.element.enabled)
                    {
                        item.render.DrawElementText(printer, WindowBounds, item.element, now, frameTime);
                        if (item.dynamic)
                            toKill.Add(item);
                    }
                }
                printer.End();

                foreach (TextItem item in toKill)
                    textItems.Remove(item);

                GL.Enable(EnableCap.Lighting);
            }
        }

        // shittyness to make python easer
        public PannelElement.Alignmnet AlignRelative = PannelElement.Alignmnet.Relative;
        public PannelElement.Alignmnet AlignLeftBottom = PannelElement.Alignmnet.LeftBottom;
        public PannelElement.Alignmnet AlignLeftTop = PannelElement.Alignmnet.LeftTop;
        public PannelElement.Alignmnet AlignRightBottom = PannelElement.Alignmnet.RightBottom;
        public PannelElement.Alignmnet AlignRightTop = PannelElement.Alignmnet.RightTop;
        public PannelElement.Alignmnet AlignCentered = PannelElement.Alignmnet.Centered;
        public PannelElement.Alignmnet AlignTopCenter = PannelElement.Alignmnet.TopCenter;
        public PannelElement.Alignmnet AlignBottomCenter = PannelElement.Alignmnet.BottomCenter;
        public PannelElement.Alignmnet AlignLeftCenter = PannelElement.Alignmnet.LeftCenter;
        public PannelElement.Alignmnet AlignRightCenter = PannelElement.Alignmnet.RightCenter;
    }

    public class PannelElement
    {
        public Vector2 pos = Vector2.Zero;
        public Size size = Size.Empty;

        public PannelElement  NewChild( string name, string type )
        {
            PannelElement element = new PannelElement();
            element.name = name;
            element.pannelType = type;

            children.Add(element);

            return element;
        }

        public void SetPosition(float x, float y)
        {
            pos = new Vector2(x, y);
        }

        public void SetSize(int x, int y)
        {
            size = new Size(x, y);
        }

        public enum Alignmnet
        {
            Relative,
            LeftBottom,
            LeftTop,
            RightBottom,
            RightTop,
            Centered,
            TopCenter,
            BottomCenter,
            LeftCenter,
            RightCenter,
        }

        public Alignmnet alignment = Alignmnet.Relative;
        public Alignmnet origin = Alignmnet.LeftBottom;

        public class ElementColor
        {
            public float red = 1.0f;
            public float green = 1.0f;
            public float blue = 1.0f;

            public ElementColor() { }
            public ElementColor(float r, float g, float b) { red = r; green = g; blue = b; }

            public static ElementColor White = new ElementColor();
        }

        public ElementColor color = ElementColor.White;

        public void SetColor(float red, float green, float blue, float a)
        {
            color = new ElementColor(red, green, blue);
            alpha = a;
        }

        public void SetColor(float red, float green, float blue)
        {
            color = new ElementColor(red, green, blue);
        }

        public float alpha = 1;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public Color Color
        {
            set
            {
                color.red = value.R / (float)byte.MaxValue;
                color.green = value.G / (float)byte.MaxValue;
                color.blue = value.B / (float)byte.MaxValue;
            }
            get
            {
                return System.Drawing.Color.FromArgb((byte)(byte.MaxValue * alpha), (byte)(byte.MaxValue * color.red), (byte)(byte.MaxValue * color.green), (byte)(byte.MaxValue * color.blue));
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public object Tag = null;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public object RenderTag = null;

        public string text = string.Empty;

        public string name = string.Empty;

        public string pannelType = string.Empty;
        protected PannelRenderer pannel = null;

        public string updateFunction = string.Empty;

        public delegate void UpdateFunction(PannelElement element);

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public UpdateFunction Update = null;

        public bool enabled = true;

        public List<PannelElement> children = new List<PannelElement>();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        PannelElement parrent = null;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public ViewBounds view = null;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool focus = false;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int textCursorOffset = -1;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string dialogParamaters = string.Empty;

        public class Option
        {
            public string name = string.Empty;
            public string value = string.Empty;
            public Option() { }
            public Option(string _name, string _value)
            {
                name = _name;
                value = _value;
            }
        }
        public List<Option> options = new List<Option>();

        public void AddOption(string name, string value)
        {
            options.Add(new Option(name, value));
        }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public Dictionary<string, string> optionMap = new Dictionary<string, string>();

        public void Adopt()
        {
            foreach (PannelElement child in children)
            {
                child.parrent = this;
                child.Adopt();
            }

            optionMap.Clear();
            foreach (Option o in options)
            {
                if (!optionMap.ContainsKey(o.name))
                    optionMap.Add(o.name, o.value);
            }
        }

        public void Adopt(PannelElement child)
        {
            child.parrent = this;
            children.Add(child);
        }

        public static float ChildIncrement = 0.01f;

        protected Vector2 GetOrigin()
        {
            return GetOrigin(GetElementSize());
        }

        protected Vector2 GetOrigin(Size _size)
        {
            if (origin == Alignmnet.Relative || origin == Alignmnet.LeftBottom)
                return pos;

            Size bounds = _size;

            switch (alignment)
            {
                case Alignmnet.LeftBottom:
                    return pos;
                case Alignmnet.LeftTop:
                    return new Vector2(pos.X, bounds.Height + pos.Y);

                case Alignmnet.RightBottom:
                    return new Vector2(pos.X - bounds.Width, pos.Y);
                case Alignmnet.RightTop:
                    return new Vector2(pos.X - bounds.Width, pos.Y - bounds.Height);

                case Alignmnet.Centered:
                    return new Vector2(pos.X - bounds.Width / 2, pos.Y - bounds.Height / 2);
                case Alignmnet.TopCenter:
                    return new Vector2(pos.X - bounds.Width / 2, pos.Y - bounds.Height);
                case Alignmnet.BottomCenter:
                    return new Vector2(pos.X - bounds.Width / 2, pos.Y);

                case Alignmnet.LeftCenter:
                    return new Vector2(pos.X, pos.Y - bounds.Height / 2);
                case Alignmnet.RightCenter:
                    return new Vector2(bounds.Width - pos.X, bounds.Height / 2 - pos.Y);
            }

            return pos;
        }

        public Vector2 GetDrawOrigin()
        {
            return GetDrawOrigin(GetElementSize());
        }

        public Vector2 GetDrawOrigin(Size _size)
        {
            if (alignment == Alignmnet.Relative)
                return pos;

            Size bounds = Size.Empty;

            if (parrent == null)
            {
                if (view == null)
                    return pos;

                bounds = new Size(view.Bounds.Width, view.Bounds.Height);
            }
            else
                bounds = parrent.size;

            Vector2 p = GetOrigin(_size);

            switch (alignment)
            {
                case Alignmnet.LeftBottom:
                    return p;
                case Alignmnet.LeftTop:
                    return new Vector2(p.X, bounds.Height + p.Y);

                case Alignmnet.RightBottom:
                    return new Vector2(bounds.Width + p.X, p.Y);
                case Alignmnet.RightTop:
                    return new Vector2(bounds.Width + p.X, bounds.Height + p.Y);

                case Alignmnet.Centered:
                    return new Vector2(bounds.Width / 2 + p.X, bounds.Height / 2 + p.Y);
                case Alignmnet.TopCenter:
                    return new Vector2(bounds.Width / 2 + p.X, bounds.Height + p.Y);
                case Alignmnet.BottomCenter:
                    return new Vector2(bounds.Width / 2 + p.X, p.Y);

                case Alignmnet.LeftCenter:
                    return new Vector2(p.X, bounds.Height / 2 - p.Y);
                case Alignmnet.RightCenter:
                    return new Vector2(bounds.Width + p.X, bounds.Height / 2 - p.Y);
            }

            return pos;
        }

        public Size GetElementSize()
        {
            return GetRenderer().GetElementSize(this);
        }

        public Vector2 GetWorldPos()
        {
            return GetWorldPos(GetElementSize());
        }

        public Vector2 GetWorldPos(Size _size)
        {
            if (parrent != null)
                return parrent.GetWorldPos(_size) + GetDrawOrigin(_size);
            return GetDrawOrigin(_size);
        }

        public PannelRenderer GetRenderer()
        {
            if (pannel == null)
            {
                pannel = PannelRenderer.Get(pannelType);
                pannel.ElementAdded(this);
            }
            return pannel;
        }

        public void Remove()
        {
            if (pannel != null)
                pannel.ElementRemoved(this);
        }

        public static bool outLinePannels = false;
        public void Draw(double now, double frameTime)
        {
            if (!enabled || pannelType == string.Empty)
                return;

            GL.PushMatrix();

            if (Update != null)
                Update(this);

            GetRenderer().DrawElement(this, now, frameTime);

            foreach (PannelElement element in children)
            {
                GL.Translate(0, 0, ChildIncrement);
                GL.PushMatrix();
                element.Draw(now, frameTime);
                GL.PopMatrix();
            }

            GL.PopMatrix();
        }

        public void Save(string path)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(PannelElement));
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                xml.Serialize(fs, this);
                fs.Close();
            }
            catch (System.Exception ex)
            {
                string l = ex.Message;
            }
        }
    }

    public class PannelRenderer
    {
        public static Dictionary<string, PannelRenderer> Pannels = new Dictionary<string, PannelRenderer>();
        public static PannelRenderer Get(string name)
        {
            if (Pannels.ContainsKey(name))
                return Pannels[name];

            PannelRenderer p = new PannelRenderer(name);
            Pannels.Add(name, p);
            return p;
        }

        public PannelRenderer()
        {
        }

        public PannelRenderer(string name)
        {
            if (!Pannels.ContainsKey(name))
                Pannels.Add(name, this);
        }

        public virtual Size GetElementSize(PannelElement element)
        {
            return element.size;
        }

        public virtual void ElementAdded(PannelElement element)
        {
        }

        public virtual void ElementRemoved(PannelElement element)
        {
        }

        public virtual void DrawElementText(TextPrinter printer, ViewBounds view, PannelElement element, double now, double frameTime)
        {
        }

        public virtual void DrawElement(PannelElement element, double now, double frameTime)
        {
            if (PannelElement.outLinePannels)
            {
                GL.PushMatrix();
                GL.Translate(0, 0, 0.01f);

                GL.Disable(EnableCap.Texture2D);
                GL.Color4(Color.White);
                int w = element.size.Width;
                int h = element.size.Height;
               
                Vector2 o = element.GetDrawOrigin();

                GL.Translate(o.X, o.Y, 0);

                GL.Begin(BeginMode.LineLoop);

              
                GL.Vertex2(0, h);

                GL.Vertex2(0, 0);

                GL.Vertex2(w, 0);

                GL.Vertex2(w, h);

                GL.End();
                GL.Enable(EnableCap.Texture2D);
                GL.PopMatrix();
            }
        }

        public virtual void ShowDialog(PannelElement element, string paramaters)
        {
        }
    }
}

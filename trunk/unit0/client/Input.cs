using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;

using OpenTK;
using OpenTK.Input;

namespace Client
{
    public class InputSystem
    {
        public static int LeftMouseButton = 0;
        public static int RightMouseButton = 2;
        public static int MiddleMouseButton = 1;
        
        public class Axis
        {
            public bool DoubleSided = true;
            public bool Cumulative = false;
            public float Value = 0;
            public float Delta = 0;

            public string Name = string.Empty;

            public int useCount = 0;

            public int ID = -1;

            public EventHandler<EventArgs> Changed;

            public static double DeadZone = 0.001;

            public bool HasMove()
            {
                return Math.Abs(Value) > DeadZone;
            }
        }

        public class Button
        {
            public bool Down = false;
            public EventHandler<EventArgs> Changed;
            public string Name = string.Empty;

            public int useCount = 0;
            public int ID = -1;

            public Key LastKeyUsed = Key.Unknown;
        }

        public class BindingElement
        {
            public string ControlName = string.Empty;
        }

        public class AxisBinding : BindingElement
        {
            public Axis ControlAxis = null;

            public float Position = 0;
        }

        public class ButtonBinding : BindingElement
        {
            public Button ControlButton = null;
        }

        public class TwoButtonAxis : AxisBinding
        {
            public float Speed = 1.0f;

            public Key MaxKey = Key.Unknown;
            public Key MinKey = Key.Unknown;

            public bool MaxDown = false;
            public bool MinDown = false;
        }

        public class ThreeButtonAxis : TwoButtonAxis
        {
            public Key FactorKey = Key.Unknown;
            public float Factor = 0.5f;

            public bool FactorDown = false;
        }

        public class MouseAxis : AxisBinding
        {
            public bool IsXAxis = false;
            public int LimitButton = -1;
            public float Factor = 1.0f;
        }

        public class KeyButton : ButtonBinding
        {
            public Key ButtonKey = Key.Unknown;

            public KeyButton()
                : base()
            {

            }

            public KeyButton(string control, Key key)
                : base()
            {
                ControlName = control;
                ButtonKey = key;
            }
        }

        public class MouseButton : ButtonBinding
        {
            public int ButtonIndex = 0;
        }

        protected string GetBindingName(Type type)
        {
            if (type == typeof(MouseAxis))
                return "MouseAxis";
            else if (type == typeof(TwoButtonAxis))
                return "TwoButtonAxis";
            else if (type == typeof(ThreeButtonAxis))
                return "ThreeButtonAxis";
            else if (type == typeof(KeyButton))
                return "KeyButton";
            else if (type == typeof(MouseButton))
                return "MouseButton"; 
            
            return "Unknown";
        }

        protected Type BindingFromName(string name)
        {
            if (name == "MouseAxis")
                return typeof(MouseAxis);
            else if (name == "TwoButtonAxis")
                return typeof(TwoButtonAxis);
            else if (name =="ThreeButtonAxis")
                return typeof(ThreeButtonAxis);
            else if (name == "KeyButton")
                return typeof(KeyButton);
            else if (name == "MouseButton")
                return typeof(MouseButton);

            return null;
        }

        protected string WriteBinding(BindingElement element)
        {
            string output = string.Empty;

            output += element.ControlName + " ";

            Type type = element.GetType();
            output += GetBindingName(type) + " ";

            if (type == typeof(MouseAxis))
            {
                output += "Axis:";
                MouseAxis b = element as MouseAxis;
                if (b.IsXAxis)
                    output += "X";
                else
                    output += "Y";

                output += " Factor:" + b.Factor.ToString();
                if (b.LimitButton >= 0)
                    output += " LimitButton:" + b.LimitButton.ToString();
            }
            else if (type == typeof(TwoButtonAxis))
            {
                TwoButtonAxis b = element as TwoButtonAxis;
                output += "Max:" + b.MaxKey.ToString() + " Min:" + b.MinKey.ToString() + " Speed:" + b.Speed.ToString();
            }
            else if (type == typeof(ThreeButtonAxis))
            {
                ThreeButtonAxis b = element as ThreeButtonAxis;
                output += "Max:" + b.MaxKey.ToString() + " Min:" + b.MinKey.ToString() + " Speed:" + b.Speed.ToString() + " FactorKey:" + b.FactorKey.ToString() + " Factor:" + b.Factor.ToString();
            }
            else if (type == typeof(KeyButton))
            {
                KeyButton b = element as KeyButton;
                output += "Key:" + b.ButtonKey.ToString();
            }
            else if (type == typeof(MouseButton))
            {
                MouseButton b = element as MouseButton;
                output += "Button:" + b.ButtonIndex.ToString();
            }

            return output;
        }

        protected BindingElement ReadBinding(string line)
        {
            string[] parts = line.Split(" ".ToCharArray());
            if (parts.Length < 2)
                return null;

            Type t = BindingFromName(parts[1]);

            if (t == null)
                return null;

            BindingElement element = (BindingElement)Activator.CreateInstance(t);
            element.ControlName = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string[] bits = parts[i].Split(":".ToCharArray(), 2);
                if (bits.Length < 2)
                    continue;

                string code = bits[0];
                string value = bits[1];
                float fValue = 0;
                float.TryParse(value, out fValue);

                int iValue = -1;
                int.TryParse(value, out iValue);

                Key kValue = Key.Unknown;
                Enum.TryParse<Key>(value,out kValue);

                if (code == "Axis" && value == "X" && t == typeof(MouseAxis))
                    (element as MouseAxis).IsXAxis = true;
                else if (code == "Factor")
                {
                    if (t == typeof(MouseAxis))
                        (element as MouseAxis).Factor = fValue;
                    else if (t == typeof(ThreeButtonAxis))
                        (element as ThreeButtonAxis).Factor = fValue;
                }
                else if (code == "LimitButton" && t == typeof(MouseAxis))
                    (element as MouseAxis).LimitButton = iValue;
                else if (code == "Max" && (t == typeof(TwoButtonAxis) || t == typeof(ThreeButtonAxis)))
                    (element as TwoButtonAxis).MaxKey = kValue;
                else if (code == "Min" && (t == typeof(TwoButtonAxis) || t == typeof(ThreeButtonAxis)))
                    (element as TwoButtonAxis).MinKey = kValue;
                else if (code == "Speed" && (t == typeof(TwoButtonAxis) || t == typeof(ThreeButtonAxis)))
                    (element as TwoButtonAxis).Speed = fValue;
                else if (code == "FactorKey" && t == typeof(ThreeButtonAxis))
                    (element as ThreeButtonAxis).FactorKey = kValue;
                else if (code == "Key" && t == typeof(KeyButton))
                    (element as KeyButton).ButtonKey = kValue;
                else if (code == "Button" && t == typeof(MouseButton))
                    (element as MouseButton).ButtonIndex = iValue;
            }

            return element;
        }

        protected Dictionary<string, Axis> Axes = new Dictionary<string, Axis>();
        protected Dictionary<string, Button> Buttons = new Dictionary<string, Button>();

        public Button FindButton(string name)
        {
            if (Buttons.ContainsKey(name))
                return Buttons[name];

            return null;
        }

        public Axis FindAxis(string name)
        {
            if (Axes.ContainsKey(name))
                return Axes[name];

            return null;
        }

        protected List<BindingElement> Bindings = new List<BindingElement>();

        protected List<MouseAxis> MouseAxisBindings = new List<MouseAxis>();
        protected List<MouseButton> MouseButtonBindings = new List<MouseButton>();

        protected List<AxisBinding> KeyboardAxisBindings = new List<AxisBinding>();
        protected List<KeyButton> KeyboardButtonBindings = new List<KeyButton>();

        public EventHandler<EventArgs> RegisterControls;
        public EventHandler<EventArgs> LoadDefaultBindings;

        public delegate string GetStringCallback();
        public static GetStringCallback GetBindingFilePath = null;

        protected Dictionary<int, List<AxisBinding>> BindingsPerAxis = new Dictionary<int, List<AxisBinding>>();

        protected bool[] MouseButtonStates = null;
        protected Point LastMousePosition = new Point(0, 0);
        protected Point MouseDelta = new Point(0,0);

        protected int Width = 0;
        protected int Height = 0;

        protected bool TextMode = false;

        public InputSystem(GameWindow window)
        {
            window.Load += new EventHandler<EventArgs>(window_Load);
            window.Resize += new EventHandler<EventArgs>(window_Resize);
            window.Keyboard.KeyDown += new EventHandler<KeyboardKeyEventArgs>(Keyboard_KeyDown);
            window.Keyboard.KeyUp += new EventHandler<KeyboardKeyEventArgs>(Keyboard_KeyUp);
            window.KeyPress += new EventHandler<KeyPressEventArgs>(window_KeyPress);

            window.Mouse.ButtonUp += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonUp);
            window.Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonDown);
            window.Mouse.Move += new EventHandler<MouseMoveEventArgs>(Mouse_Move);
            window.Mouse.WheelChanged += new EventHandler<MouseWheelEventArgs>(Mouse_WheelChanged);
			
			if (window.Mouse.NumberOfButtons == 0)
				MouseButtonStates = new bool[3]; // stupid macintosh!
			else
            	MouseButtonStates = new bool[window.Mouse.NumberOfButtons];

            TextMode = false;

            window_Resize(window, EventArgs.Empty);
        }

        void window_Resize(object sender, EventArgs e)
        {
            GameWindow win = sender as GameWindow;
            if (win != null)
            {
                Width = win.Width;
                Height = win.Height;
            }
        }

        public bool BindingExists(string name)
        {
            foreach (BindingElement element in Bindings)
            {
                if (element.ControlName == name)
                    return true;
            }
            return false;
        }

        void window_Load(object sender, EventArgs e)
        {
            string path = string.Empty;
            if (GetBindingFilePath != null)
                path = GetBindingFilePath();

            if (RegisterControls != null)
                RegisterControls(this,EventArgs.Empty);

            // find the bindings, if not load the defaults?
            if (LoadDefaultBindings != null)
                LoadDefaultBindings(this, EventArgs.Empty);

            List<BindingElement> defaults = Bindings;
            Bindings = new List<BindingElement>();

            LoadBindings(path);

            List<BindingElement> toAdd = new List<BindingElement>();

            // find any defaults that are not bound and bind them.
            // how to handle features that should not be bound? make a null binding?
            foreach (BindingElement element in defaults)
            {
                if (!BindingExists(element.ControlName))
                    toAdd.Add(element);
            }

            foreach (BindingElement element in toAdd)
                AddBinding(element);

            SortBindings();

            SaveBindings(path);
        }

        protected void LoadBindings(string path)
        {
            if (path == string.Empty || !File.Exists(path))
                return;

            FileInfo file = new FileInfo(path);
            StreamReader reader = file.OpenText();

            while (!reader.EndOfStream)
                AddBinding(ReadBinding(reader.ReadLine()));

            reader.Close();
        }

        public void SaveBindings(string path)
        {
            if (path == string.Empty)
                return;
            try
            {
                FileInfo file = new FileInfo(path);
                FileStream fs = file.OpenWrite();
                StreamWriter writer = new StreamWriter(fs);
                foreach (BindingElement element in Bindings)
                    writer.WriteLine(WriteBinding(element));

                writer.Close();
                fs.Close();
            }
            catch (System.Exception /*ex*/)
            {
            	
            }
        }

        protected void SortBindings()
        {
            foreach (BindingElement element in Bindings)
            {
                Type elementType = element.GetType();

                if (elementType.IsSubclassOf(typeof(ButtonBinding)))
                {
                    if (elementType == typeof(MouseButton) || elementType.IsSubclassOf(typeof(MouseButton)))
                        MouseButtonBindings.Add(element as MouseButton);
                    else
                        KeyboardButtonBindings.Add(element as KeyButton);

                    ButtonBinding button = element as ButtonBinding;
                    button.ControlButton = FindButton(button.ControlName);
                }
                else if (element.GetType().IsSubclassOf(typeof(AxisBinding)))
                {
                    if (elementType == typeof(MouseAxis) || elementType.IsSubclassOf(typeof(MouseAxis)))
                        MouseAxisBindings.Add(element as MouseAxis);
                    else
                        KeyboardAxisBindings.Add(element as AxisBinding);

                    AxisBinding axis = element as AxisBinding;
                    axis.ControlAxis = FindAxis(axis.ControlName);

                    if (!BindingsPerAxis.ContainsKey(axis.ControlAxis.ID))
                        BindingsPerAxis[axis.ControlAxis.ID] = new List<AxisBinding>();

                    BindingsPerAxis[axis.ControlAxis.ID].Add(axis);
                }
            }
        }

        void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
        {
           // throw new NotImplementedException();
        }

        void Mouse_Move(object sender, MouseMoveEventArgs e)
        {
            MouseDelta.X += e.XDelta;
            MouseDelta.Y += e.YDelta;

            LastMousePosition = new Point(e.X, e.Y);
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (MouseButton binding in MouseButtonBindings)
            {
                if (binding.ButtonIndex == (int)e.Button)
                {
                    int oldCount = binding.ControlButton.useCount;
                    binding.ControlButton.useCount++;
                    binding.ControlButton.Down = true;
                    if (oldCount == 0 && binding.ControlButton.Changed != null)
                        binding.ControlButton.Changed(binding.ControlButton, EventArgs.Empty);
                }
            }

            MouseButtonStates[(int)e.Button] = true;
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (MouseButton binding in MouseButtonBindings)
            {
                binding.ControlButton.useCount--;
                if (binding.ControlButton.useCount <= 0)
                {
                    binding.ControlButton.Down = false;

                    if (binding.ControlButton.Changed != null)
                        binding.ControlButton.Changed(binding.ControlButton, EventArgs.Empty);
                }
            }

            MouseButtonStates[(int)e.Button] = false;
        }

        void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.CapsLock)
                CapsLock = false;
            if (e.Key == Key.RShift || e.Key == Key.LShift || e.Key == Key.ShiftLeft || e.Key == Key.ShiftRight)
                ShiftCount--;

            if (TextMode && e.Key != IngnoredUp)
                ProcessTextModeKey(e.Key, false);
            else
            {
                // check the buttons for events
                foreach (KeyButton binding in KeyboardButtonBindings)
                {
                    if (binding.ButtonKey == e.Key)
                    {
                        int oldCount = binding.ControlButton.useCount;
                        if (oldCount != 0) // we will get an extra keyup for the key that is used to end text mode so just ignore it
                        {
                            binding.ControlButton.useCount--;
                            if (binding.ControlButton.useCount <= 0)
                            {
                                binding.ControlButton.Down = false;
                                binding.ControlButton.LastKeyUsed = e.Key;

                                if (e.Key != IngnoredUp && binding.ControlButton.Changed != null) // the client can ignore the next keyup for the chat init key if it cares about keyups
                                    binding.ControlButton.Changed(binding.ControlButton, EventArgs.Empty);
                            }
                        }
                    }
                }

                foreach (AxisBinding binding in KeyboardAxisBindings)
                    DoAxisKeyEvent(binding, e.Key, false);
            }
        }

        protected void DoAxisKeyEvent(AxisBinding binding, Key key, bool down)
        {
            // see what kinda axis binding it is
            TwoButtonAxis twoButton = binding as TwoButtonAxis;
            if (binding.GetType() == typeof(TwoButtonAxis))
            {
                twoButton = binding as TwoButtonAxis;
                if (key != twoButton.MaxKey && key != twoButton.MinKey)
                    return;

                if (key == twoButton.MaxKey)
                    twoButton.MaxDown = down;
                else
                    twoButton.MinDown = down;

                if (twoButton.MaxDown == twoButton.MinDown)
                    twoButton.Position = 0;
                else
                {
                    if (twoButton.MaxDown)
                        twoButton.Position = 1;
                    else
                    {
                        if (twoButton.ControlAxis.DoubleSided)
                            twoButton.Position = -1;
                        else
                            twoButton.Position = 0;
                    }
                }
            }
            else
            {
                ThreeButtonAxis threeButton = binding as ThreeButtonAxis;
                if (threeButton == null)
                    return;

                if (key != threeButton.MaxKey && key != threeButton.MinKey && key != threeButton.FactorKey)
                    return;

                if (key == twoButton.MaxKey)
                    threeButton.MaxDown = down;
                else if (key == twoButton.MinKey)
                    threeButton.MinDown = down;
                else if (key == threeButton.FactorKey)
                    threeButton.FactorDown = down;

                if (threeButton.MaxDown == threeButton.MinDown)
                    threeButton.Position = 0;
                else
                {
                    float val = 1.0f;
                    if (threeButton.FactorDown)
                        val = threeButton.Factor;

                    if (threeButton.MaxDown)
                        threeButton.Position = val;
                    else
                    {
                        if (threeButton.ControlAxis.DoubleSided)
                            threeButton.Position = -val;
                        else
                            threeButton.Position = 0;
                    }
                }
            }
        }

        Key IngnoredUp = Key.Unknown;

        public void IgnoreNextUp(Key key)
        {
            IngnoredUp = key;
        }

        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.CapsLock)
                CapsLock = true;
            if (e.Key == Key.RShift || e.Key == Key.LShift || e.Key == Key.ShiftLeft || e.Key == Key.ShiftRight)
                ShiftCount++;

            if (TextMode)
                ProcessTextModeKey(e.Key, true);
            else
            {
                // check all the buttons
                foreach (KeyButton binding in KeyboardButtonBindings)
                {
                    if (binding.ButtonKey == e.Key)
                    {
                        int oldCount = binding.ControlButton.useCount;
                        binding.ControlButton.useCount++;
                        binding.ControlButton.Down = true;
                        binding.ControlButton.LastKeyUsed = e.Key;
                        if (oldCount == 0 && binding.ControlButton.Changed != null)
                            binding.ControlButton.Changed(binding.ControlButton, EventArgs.Empty);
                    }
                }

                foreach (AxisBinding binding in KeyboardAxisBindings)
                    DoAxisKeyEvent(binding, e.Key, true);
            }
        }

        public Axis AddAxis(string name)
        {
            return AddAxis(name, true,false);
        }

        public Axis AddAxis(string name, bool doubleSided)
        {
            return AddAxis(name, doubleSided,false);
        }

        public Axis AddAxis(string name, bool doubleSided, bool cumulative)
        {
            if (Axes.ContainsKey(name))
                return Axes[name];

            Axis axis = new Axis();
            axis.DoubleSided = doubleSided;
            axis.Cumulative = cumulative;
            axis.Name = name;
            axis.ID = Axes.Count;
            Axes.Add(name, axis);
            return axis;
        }

        public Button AddButton(string name)
        {
            return AddButton(name, null);
        }

        public Button AddButton(string name, EventHandler<EventArgs> changed)
        {
            name = name.Replace(' ', '_');

            if (Buttons.ContainsKey(name))
                return Buttons[name];

            Button button = new Button();
            button.Name = name;
            if (changed != null)
                button.Changed = changed;
            Buttons.Add(name, button);
            return button;
        }
        public void AddBinding(BindingElement element)
        {
            if (element == null)
                return;

            element.ControlName = element.ControlName.Replace(' ', '_');

            Bindings.Add(element);
        }

        public event EventHandler<EventArgs> TextModeEnded;
        public event EventHandler<EventArgs> TextChanged;
        protected string TextModeString = string.Empty;

        Button TextModeEndButton;

        public bool InTextMode()
        {
            return TextMode;
        }

        public string GetTextModeString()
        {
            return TextModeString;
        }

        public void EnterTextMode(Button EndButton)
        {
            if (TextMode)
                return;

            TextMode = true;
            TextModeString = "";
            TextModeEndButton = EndButton;
        }

        public static double TextKeyRepeatTime = 1;

        Dictionary<Key, double> LastCharacterTime = new Dictionary<Key, double>();

        int ShiftCount = 0;
        bool CapsLock = false;

        protected bool CapsChar()
        {
            return ShiftCount > 0 || CapsLock;
        }

        protected void ProcessTextModeKey(Key key, bool down)
        {
            if (down)
            {
                TextModeKeyPress(key);
                if (LastCharacterTime.ContainsKey(key))
                    LastCharacterTime[key] = Now();
                else
                    LastCharacterTime.Add(key, Now());
            }
            else
            {
                if (LastCharacterTime.ContainsKey(key))
                    LastCharacterTime.Remove(key);
            }
        }

        bool CharIsEmpty(char c)
        {
            if (c == ' ' || c == '\t')
                return false;

            return c.ToString().Trim() == string.Empty;
        }

        void window_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!TextMode || CharIsEmpty(e.KeyChar))
                return;

            if (CapsChar())
                TextModeString += e.KeyChar.ToString().ToUpper();
            else
                TextModeString += e.KeyChar.ToString().ToLower();

            if (TextChanged != null)
                TextChanged(this, EventArgs.Empty);
        }

        protected void TextModeKeyPress(Key key)
        {
            // is this the end key?
            foreach (KeyButton binding in KeyboardButtonBindings)
            {
                if (binding.ControlButton == TextModeEndButton && binding.ButtonKey == key)
                {
                    LastCharacterTime.Clear();
                    if (TextModeEnded != null)
                        TextModeEnded(this, EventArgs.Empty);
                    TextMode = false;

                    return;
                }
            }

            // was it a delete character?
            if (key == Key.Delete || key == Key.BackSpace)
            {
                if (TextModeString.Length > 0)
                {
                    TextModeString = TextModeString.Remove(TextModeString.Length - 1);
                    if (TextChanged != null)
                        TextChanged(this, EventArgs.Empty);
                }
            }
        }

        protected void CheckForKeyRepeats()
        {
            if (!InTextMode())
                return;

           // double now = Now();
        }

        Stopwatch InputTimer = new Stopwatch();
        double LastAxistime = 0;

        protected double Now()
        {
             if (!InputTimer.IsRunning)
                InputTimer.Start();
             return InputTimer.ElapsedMilliseconds / 1000.0;
        }

        public void Update()
        {
            UpdateAxes();
            CheckForKeyRepeats();
        }

        public void UpdateAxes()
        {
            double now = Now();
            float delta = (float)(now - LastAxistime);
            LastAxistime = now;

            foreach (Axis axis in Axes.Values)
            {
                if (!BindingsPerAxis.ContainsKey(axis.ID))
                    continue;

                float lastValue = axis.Value;

                if (axis.Cumulative)
                {
                    float maxIncrement = 0;
                    foreach (AxisBinding linkedBinding in BindingsPerAxis[axis.ID])
                    {
                        float thisIncrement = 0;
                        TwoButtonAxis keyAxis = linkedBinding as TwoButtonAxis;
                        if (keyAxis != null)
                            thisIncrement = keyAxis.Position * keyAxis.Speed * delta;
                        else
                        {
                             MouseAxis mouseAxis = linkedBinding as MouseAxis;
                             if (mouseAxis != null)
                             {
								bool button = false;
								if (mouseAxis.LimitButton >= 0 && mouseAxis.LimitButton < MouseButtonStates.Length)
									button = MouseButtonStates[mouseAxis.LimitButton];
									                                    
                                 if (mouseAxis.LimitButton == -1 || button)
                                 {
                                     if (mouseAxis.IsXAxis)
                                         thisIncrement = MouseDelta.X * mouseAxis.Factor;
                                     else
                                         thisIncrement = MouseDelta.Y * mouseAxis.Factor;
                                 }
                             }
                        }

                        if (Math.Abs(thisIncrement) > Math.Abs(maxIncrement))
                            maxIncrement = thisIncrement;
                    }

                    axis.Value += maxIncrement;
                    axis.Delta = maxIncrement;
                }
                else
                {
                    float maxValue = 0;
                    // check all the axis tied to this thing and pick a value
                    foreach (AxisBinding linkedBinding in BindingsPerAxis[axis.ID])
                    {
                        float pos = 0;
                        TwoButtonAxis keyAxis = linkedBinding as TwoButtonAxis;
                        if (keyAxis != null)
                            pos = linkedBinding.Position;
                        else
                        {
                            MouseAxis mouseAxis = linkedBinding as MouseAxis;
                            if (mouseAxis != null && MouseButtonStates != null)
                            {
                                bool button = false;
                                if (mouseAxis.LimitButton >= 0 && mouseAxis.LimitButton < MouseButtonStates.Length)
                                    button = false;

                                if (mouseAxis.LimitButton == -1 || button)
                                {
                                    
                                    if (mouseAxis.IsXAxis)
                                        pos = (float)LastMousePosition.X / (float)Width * 2 - 1;
                                    else
                                        pos = (float)LastMousePosition.Y / (float)Height * 2 - 1;

                                    if (Math.Abs(pos) > 1)
                                        pos = (float)Math.Abs(pos) * (pos / Math.Abs(pos));
                                }
                            }
                        }
                        if (Math.Abs(pos) > Math.Abs(maxValue))
                            maxValue = pos;
                    }
                    axis.Value = maxValue;
                }

                if (axis.Value != lastValue && axis.Changed != null)
                    axis.Changed(axis, EventArgs.Empty);
            }

            MouseDelta.X = 0;
            MouseDelta.Y = 0;
        }
    }
}

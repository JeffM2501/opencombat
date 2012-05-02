using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Client
{
    public partial class ClientGame
    {
        public InputSystem.Axis SpinAxis = null;
        public InputSystem.Axis TiltAxis = null;
        public InputSystem.Axis LinearAxis = null;
        public InputSystem.Axis SidestepAxis = null;
        public InputSystem.Axis ZAxis = null;
        public InputSystem.Button ResetZ = null;

        public InputSystem.Button ResetSpin = null;

        // debug buttons
        public InputSystem.Button ToggleDebugDrawing = null;

        // tank movement
        protected InputSystem.Axis TankLinearAxis = null;
        protected InputSystem.Axis TankRotaryAxis = null;

        // chat text
        protected InputSystem.Button ChatButton = null;

        void InputTracker_RegisterControls(object sender, EventArgs args)
        {
            SpinAxis = InputTracker.AddAxis("Spin", true, true);
            TiltAxis = InputTracker.AddAxis("Tilt", true, true);
            LinearAxis = InputTracker.AddAxis("Linear", true, false);
            SidestepAxis = InputTracker.AddAxis("Sidestep", true, false);
            ZAxis = InputTracker.AddAxis("ZAxis", true, false);
            ResetZ = InputTracker.AddButton("ResetZ");
            ResetZ.Changed = new EventHandler<EventArgs>(ResetZ_Changed);
            ResetSpin = InputTracker.AddButton("ResetSpin");
            ResetSpin.Changed = new EventHandler<EventArgs>(ResetSpin_Changed);

            ChatButton = InputTracker.AddButton("Chat",new EventHandler<EventArgs>(InputTracker_ChatButton_Changed));

            ToggleDebugDrawing = InputTracker.AddButton("ToggleDebugDraw", new EventHandler<EventArgs>(ToggleDebugDrawing_Changed));

            TankLinearAxis = InputTracker.AddAxis("TankLinear", true, false);
            TankRotaryAxis = InputTracker.AddAxis("TankRotary", true, false);

            InputTracker.TextChanged += new EventHandler<EventArgs>(InputTracker_TextChanged);
            InputTracker.TextModeEnded += new EventHandler<EventArgs>(InputTracker_TextModeEnded);
        }

        void InputTracker_TextModeEnded(object sender, EventArgs e)
        {
            if (ExitTextMode != null)
                ExitTextMode(this, EventArgs.Empty);
        }

        void InputTracker_TextChanged(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        void InputTracker_ChatButton_Changed(object sender, EventArgs args)
        {
            if (ChatButton.Down || !InputTracker.InTextMode())
            {
                InputTracker.IgnoreNextUp(ChatButton.LastKeyUsed);
                InputTracker.EnterTextMode(ChatButton);
                if (EnterTextMode != null)
                    EnterTextMode(this, EventArgs.Empty);
            }
        }

        void InputTracker_LoadDefaultBindings(object sender, EventArgs args)
        {
            InputSystem.MouseAxis spinBinding = new InputSystem.MouseAxis();
            spinBinding.ControlName = "Spin";
            spinBinding.Factor = 0.5f;
            spinBinding.IsXAxis = true;
            spinBinding.LimitButton = InputSystem.RightMouseButton;
            InputTracker.AddBinding(spinBinding);

            InputSystem.MouseAxis tiltBinding = new InputSystem.MouseAxis();
            tiltBinding.ControlName = "Tilt";
            tiltBinding.Factor = 0.5f;
            tiltBinding.IsXAxis = false;
            tiltBinding.LimitButton = InputSystem.RightMouseButton;
            InputTracker.AddBinding(tiltBinding);

            InputSystem.TwoButtonAxis linearBinding = new InputSystem.TwoButtonAxis();
            linearBinding.ControlName = "TankLinear";
            linearBinding.MaxKey = Key.Up;
            linearBinding.MinKey = Key.Down;
            InputTracker.AddBinding(linearBinding);

            linearBinding = new InputSystem.TwoButtonAxis();
            linearBinding.ControlName = "Linear";
            linearBinding.MaxKey = Key.W;
            linearBinding.MinKey = Key.S;
            InputTracker.AddBinding(linearBinding);

            linearBinding = new InputSystem.TwoButtonAxis();
            linearBinding.ControlName = "TankRotary";
            linearBinding.MaxKey = Key.Right;
            linearBinding.MinKey = Key.Left;
            InputTracker.AddBinding(linearBinding);

            linearBinding = new InputSystem.TwoButtonAxis();
            linearBinding.ControlName = "Sidestep";
            linearBinding.MaxKey = Key.D;
            linearBinding.MinKey = Key.A;
            InputTracker.AddBinding(linearBinding);

            linearBinding = new InputSystem.TwoButtonAxis();
            linearBinding.ControlName = "ZAxis";
            linearBinding.MaxKey = Key.PageUp;
            linearBinding.MinKey = Key.PageDown;
            InputTracker.AddBinding(linearBinding);

            InputTracker.AddBinding(new InputSystem.KeyButton("ResetZ", Key.Home));

            InputTracker.AddBinding(new InputSystem.KeyButton("ToggleDebugDraw", Key.F1));

            InputTracker.AddBinding(new InputSystem.KeyButton("ResetSpin", Key.Home));

            InputTracker.AddBinding(new InputSystem.KeyButton("Chat", Key.Enter));
        }
    }
}

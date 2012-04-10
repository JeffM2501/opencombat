using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;
using GridWorld;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Client
{
    public class ClientGame
    {
        public EventHandler<EventArgs> ToggleDrawing;

        public delegate void DebugValueCallback (string name, string value);
        public event DebugValueCallback AddDebugLogItem;

        protected void CallDebugLogItem(string name, string value)
        {
            if (AddDebugLogItem != null)
                AddDebugLogItem(name, value);
        }

        public GameState State = null;
        protected InputSystem InputTracker = null;

        ServerConnection Connection = null;

        public ClientGame(InputSystem input)
        {
            InputTracker = input;
            State = new GameState();

            ClientScripting.Script.SetState(State);

            State.GetWorld = GetSimpleWorld;
            State.MapLoaded += new EventHandler<EventArgs>(State_MapLoaded);

            InputTracker.RegisterControls += new EventHandler<EventArgs>(InputTracker_RegisterControls);
            InputTracker.LoadDefaultBindings += new EventHandler<EventArgs>(InputTracker_LoadDefaultBindings);
        }

        public void Connect(string host, int port)
        {
            Connection = new ServerConnection(host, port);
            Connection.Connected += new EventHandler<EventArgs>(ConnectionComplete);
            Connection.Failed += new EventHandler<EventArgs>(ConnectionError);
            Connection.Disconnected += new EventHandler<EventArgs>(ConnectionEnded);
        }

        void ConnectionComplete (object sender, EventArgs args)
        {
            ClientScripting.Script.Init(Connection.ScriptingInfo.ScriptSet);
            ClientScripting.Script.InitGameScript(Connection.ScriptingInfo.GameStyle);
        }

        void ConnectionError(object sender, EventArgs args)
        {

        }

        void ConnectionEnded(object sender, EventArgs args)
        {

        }

        public LocalPlayer PlayerActor = null;

        void State_MapLoaded(object sender, EventArgs e)
        {
            Vector3 pos = SetCameraZ(new Vector3(3, 3, 1));

            PlayerActor = State.AddActor(StandardActors.LocalPlayer) as LocalPlayer;

            PlayerActor.LastUpdatePostion = new Vector3(pos);
            PlayerActor.LastUpdateRotation = new Vector3(0, 0, 0);
            PlayerActor.LastUpdateTime = State.Now;
        }

        World GetSimpleWorld()
        {
            return WorldBuilder.NewWorld(string.Empty, null);
        }

        public void Load()
        {
            State.Load();
        }

        protected Vector3 SetCameraZ(Vector3 pos)
        {
            float depth = State.GameWorld.DropDepth(pos.X, pos.Y);
            if (depth != float.MinValue)
                pos.Z = depth;

            return pos;
        }


        public InputSystem.Axis SpinAxis = null;
        public InputSystem.Axis TiltAxis = null;
        public InputSystem.Axis LinearAxis = null;
        public InputSystem.Axis SidestepAxis = null;
        public InputSystem.Axis ZAxis = null;
        public InputSystem.Button ResetZ = null;

        // debug buttons
        public InputSystem.Button ToggleDebugDrawing = null;
        public InputSystem.Button MoveDebugRayXPos = null;
        public InputSystem.Button MoveDebugRayYPos = null;
        public InputSystem.Button MoveDebugRayXNeg = null;
        public InputSystem.Button MoveDebugRayYNeg = null;
        public InputSystem.Button MoveDebugRayZPos = null;
        public InputSystem.Button MoveDebugRayZNeg = null;

        // tank movement
        protected InputSystem.Axis TankLinearAxis = null;
        protected InputSystem.Axis TankRotaryAxis = null;

        double lastPushTime = -1;
        double lastFlushTime = -1;

        public void UpdatePlayerInput(double now)
        {
            double updateDelta = 1 / 30.0;
            if (now - lastPushTime > updateDelta)
            {
                PlayerActor.AddVectorInput(TankLinearAxis.Value, -TankRotaryAxis.Value, TiltAxis.Value);
                lastPushTime = now;
            }
        }

        public void Update()
        {
            double now = State.Now;
            UpdatePlayerInput(now);

            double delta = now - lastFlushTime;

            double timeout = 1;

            if (delta > timeout)
            {
                double time = now - 0.25f;

                GameState.BoundableActor.Location locNowBefore = PlayerActor.GetLocationAtTime(now);

                GameState.BoundableActor.Location loc = PlayerActor.GetLocationAtTime(time);

                PlayerActor.SetKnownState(time, loc.Position, loc.Rotation);

                CallDebugLogItem("Good Pos", loc.Position.ToString());
                CallDebugLogItem("Good Rot", loc.Rotation.ToString());
                CallDebugLogItem("Good Time", time.ToString());

                if (PlayerActor.ClearHistoryBeforeTime(time)) // if there were enough updates to clear any
                {
                    GameState.BoundableActor.Location locAfter = PlayerActor.GetLocationAtTime(time);

                    GameState.BoundableActor.Location locNowAfter = PlayerActor.GetLocationAtTime(now);

                    Vector3 d = locNowBefore.Position - locNowAfter.Position;
                    //  if (d.LengthFast > 0.0001)
                    // An error happend, do some debugin
                }
                lastFlushTime = now;
            }

            CallDebugLogItem("History Size", PlayerActor.InputHistory.Count.ToString());

            State.UpdateActors();

            GameState.BoundableActor.Location currentLoc = PlayerActor.GetLocation();

            CallDebugLogItem("Current Pos", currentLoc.Position.ToString());
            CallDebugLogItem("Current Rot", currentLoc.Rotation.ToString());
            CallDebugLogItem("Current Time", now.ToString());
        }

        public float CameraOffetZ = 1;

        void ResetZ_Changed(object sender, EventArgs args)
        {
            CameraOffetZ = 1;
        }

        void InputTracker_RegisterControls(object sender, EventArgs args)
        {
            SpinAxis = InputTracker.AddAxis("Spin", true, true);
            TiltAxis = InputTracker.AddAxis("Tilt", true, true);
            LinearAxis = InputTracker.AddAxis("Linear", true, false);
            SidestepAxis = InputTracker.AddAxis("Sidestep", true, false);
            ZAxis = InputTracker.AddAxis("ZAxis", true, false);
            ResetZ = InputTracker.AddButton("ResetZ");
            ResetZ.Changed = new EventHandler<EventArgs>(ResetZ_Changed);

            ToggleDebugDrawing = InputTracker.AddButton("ToggleDebugDraw", new EventHandler<EventArgs>(ToggleDebugDrawing_Changed));

//             MoveDebugRayXPos = InputTracker.AddButton("DX+", new EventHandler<EventArgs>(MoveDebugRay));
//             MoveDebugRayYPos = InputTracker.AddButton("DY+", new EventHandler<EventArgs>(MoveDebugRay));
//             MoveDebugRayXNeg = InputTracker.AddButton("DX-", new EventHandler<EventArgs>(MoveDebugRay));
//             MoveDebugRayYNeg = InputTracker.AddButton("DY-", new EventHandler<EventArgs>(MoveDebugRay));
//             MoveDebugRayZPos = InputTracker.AddButton("DZ+", new EventHandler<EventArgs>(MoveDebugRay));
//             MoveDebugRayZNeg = InputTracker.AddButton("DZ-", new EventHandler<EventArgs>(MoveDebugRay));

            TankLinearAxis = InputTracker.AddAxis("TankLinear", true, false);
            TankRotaryAxis = InputTracker.AddAxis("TankRotary", true, false);
        }

        void ToggleDebugDrawing_Changed(object sender, EventArgs args)
        {
            if (ToggleDrawing != null)
                ToggleDrawing(ToggleDebugDrawing, EventArgs.Empty);
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

            InputTracker.AddBinding(new InputSystem.KeyButton("DX+", Key.Keypad6));
            InputTracker.AddBinding(new InputSystem.KeyButton("DY+", Key.Keypad8));
            InputTracker.AddBinding(new InputSystem.KeyButton("DX-", Key.Keypad4));
            InputTracker.AddBinding(new InputSystem.KeyButton("DY-", Key.Keypad2));
            InputTracker.AddBinding(new InputSystem.KeyButton("DZ+", Key.Keypad9));
            InputTracker.AddBinding(new InputSystem.KeyButton("DZ-", Key.Keypad3));
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Game;
using GridWorld;
using WorldDrawing;
using Math3D;

namespace Client
{
    class MainWindow : IDisposable
    {
        public bool QuitOnExit = false;

        protected GameWindow Window = null;
        protected View GameView = null;
        protected GameState State = null;
        protected InputSystem InputTracker = null;

        public MainWindow()
        {
            Window = new GameWindow(1024, 800, GraphicsMode.Default, "Unit 0", GameWindowFlags.Default);
            Window.VSync = VSyncMode.Adaptive;

            InputTracker = new InputSystem(Window);

            Window.UpdateFrame += new EventHandler<FrameEventArgs>(Window_UpdateFrame);
            Window.Closed += new EventHandler<EventArgs>(Window_Closed);

            State = new GameState();
            State.GetWorld = GetSimpleWorld;
            State.MapLoaded += new EventHandler<EventArgs>(State_MapLoaded);
            GameView = new View(Window, State);
            GameView.ModifyCamera += new View.ModifyCameraCB(GameView_ModifyCamera);

            // so it will hopefully bet called after the view has had it's time to load the window
            Window.Load += new EventHandler<EventArgs>(Window_Load);

            InputTracker.RegisterControls += new EventHandler<EventArgs>(InputTracker_RegisterControls);
            InputTracker.LoadDefaultBindings += new EventHandler<EventArgs>(InputTracker_LoadDefaultBindings);
        }

        protected LocalPlayer playerActor = null;

        void State_MapLoaded(object sender, EventArgs e)
        {
            Vector3 pos = SetCameraZ(new Vector3(3, 3, 1));

            playerActor = State.AddActor(StandardActors.LocalPlayer) as LocalPlayer;

            playerActor.LastUpdatePostion =  new Vector3(pos);
            playerActor.LastUpdateRotation = new Vector3(0, 0, 0);
            playerActor.LastUpdateTime = State.Now;
        }

        protected InputSystem.Axis SpinAxis = null;
        protected InputSystem.Axis TiltAxis = null;
        protected InputSystem.Axis LinearAxis = null;
        protected InputSystem.Axis SidestepAxis = null;
        protected InputSystem.Axis ZAxis = null;
        protected InputSystem.Button ResetZ = null;

        // debug buttons
        protected InputSystem.Button ToggleDebugDrawing = null;
        protected InputSystem.Button MoveDebugRayXPos = null;
        protected InputSystem.Button MoveDebugRayYPos = null;
        protected InputSystem.Button MoveDebugRayXNeg = null;
        protected InputSystem.Button MoveDebugRayYNeg = null;
        protected InputSystem.Button MoveDebugRayZPos = null;
        protected InputSystem.Button MoveDebugRayZNeg = null;

        // tank movement
        protected InputSystem.Axis TankLinearAxis = null;
        protected InputSystem.Axis TankRotaryAxis = null;

        protected float cameraOffetZ = 1;

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

            MoveDebugRayXPos = InputTracker.AddButton("DX+", new EventHandler<EventArgs>(MoveDebugRay));
            MoveDebugRayYPos = InputTracker.AddButton("DY+", new EventHandler<EventArgs>(MoveDebugRay));
            MoveDebugRayXNeg = InputTracker.AddButton("DX-", new EventHandler<EventArgs>(MoveDebugRay));
            MoveDebugRayYNeg = InputTracker.AddButton("DY-", new EventHandler<EventArgs>(MoveDebugRay));
            MoveDebugRayZPos = InputTracker.AddButton("DZ+", new EventHandler<EventArgs>(MoveDebugRay));
            MoveDebugRayZNeg = InputTracker.AddButton("DZ-", new EventHandler<EventArgs>(MoveDebugRay));

            TankLinearAxis = InputTracker.AddAxis("TankLinear", true, false);
            TankRotaryAxis = InputTracker.AddAxis("TankRotary", true, false);
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

        void MoveDebugRay(object sender, EventArgs args)
        {
            InputSystem.Button button = sender as InputSystem.Button;
            if (button == null || !button.Down)
                return;

            if (button == MoveDebugRayXPos)
                GameView.debugRayStart.X += 1;
            else if (button == MoveDebugRayYPos)
                GameView.debugRayStart.Y += 1;
            else if (button == MoveDebugRayXNeg)
                GameView.debugRayStart.X -= 1;
            else if (button == MoveDebugRayYNeg)
                GameView.debugRayStart.Y -= 1;
            else if (button == MoveDebugRayZPos)
                GameView.debugRayStart.Z += 1;
            else if (button == MoveDebugRayZNeg)
                GameView.debugRayStart.Z -= 1;
        }

        void ResetZ_Changed(object sender, EventArgs args)
        {
            cameraOffetZ = 1;
        }

        void ToggleDebugDrawing_Changed(object sender, EventArgs args)
        {
            if (ToggleDebugDrawing.Down)
            {
                GridWorldRenderer.DrawDebugLines = !GridWorldRenderer.DrawDebugLines;
                Renderer.DisplayList.FlushGL();
            }
        }

        void GameView_ModifyCamera(Renderer.SimpleCamera cam)
        {
            GameState.BoundableActor.Location loc = playerActor.GetLocation();

            cam.Spin = loc.Rotation.Z;
            cam.Tilt = TiltAxis.Value;

            if (cam.Tilt > 85)
                cam.Tilt = 85;
            if (cam.Tilt < -85)
                cam.Tilt = -85;

            cam.ViewPosition = loc.Position;// camPos;// SetCameraZ(camPos);
            cam.ViewPosition.Z += 1;

            float offset = 2;
            Vector2 rot = VectorHelper2.FromAngle(cam.Spin);
            cam.ViewPosition.X -= rot.X * offset;
            cam.ViewPosition.Y -= rot.Y * offset;

        }

        void Window_Load(object sender, EventArgs e)
        {
            State.Load();
        }

        World GetSimpleWorld()
        {
            return WorldBuilder.NewWorld(string.Empty, null);
        }

        void Window_Closed(object sender, EventArgs e)
        {
            QuitOnExit = true;
        }

        Vector3 camPos = new Vector3(1, 1, 9);

        double lastFlushTime = -1;
        double lastPushTime = -1;

        void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (InputTracker != null)
                InputTracker.UpdateAxes();

            if (LinearAxis.HasMove() || SidestepAxis.HasMove() || ZAxis.HasMove())
            {
                float speed = 10;
                Vector2 dir = VectorHelper2.FromAngle(SpinAxis.Value * -1) * speed * LinearAxis.Value * (float)e.Time;
                dir += VectorHelper2.FromAngle((SpinAxis.Value + 90) * -1) * speed * SidestepAxis.Value * (float)e.Time;

                float maxDist = speed * (float)e.Time;
                if (dir.LengthSquared > (maxDist * maxDist))
                    dir *= maxDist / dir.Length;

                camPos.X += dir.X;
                camPos.Y += dir.Y;

                float zSpeed = speed / 4;

                if (ZAxis.HasMove())
                    camPos.Z += ZAxis.Value * (float)e.Time * zSpeed;

                // SetCameraZ();
            }

            // check the input and shit
            //  if (TankLinearAxis.HasMove() || TankRotaryAxis.HasMove())
            double now = State.Now;

            double updateDelta = 1 / 30.0;
            if (now - lastPushTime > updateDelta)
            {
                playerActor.AddVectorInput(TankLinearAxis.Value, -TankRotaryAxis.Value, TiltAxis.Value);
                lastPushTime = now;
            }
           
            if(lastFlushTime < 0)
                lastFlushTime = now;

            double delta = now - lastFlushTime;

            double timeout = 1;

            if (delta > timeout)
            {
                double time = now - 0.25f;

                GameState.BoundableActor.Location locNowBefore = playerActor.GetLocationAtTime(now);
               
                GameState.BoundableActor.Location loc = playerActor.GetLocationAtTime(time);

                playerActor.SetKnownState(time, loc.Position, loc.Rotation);

                GameView.AddDebugLogItem("Good Pos", loc.Position.ToString());
                GameView.AddDebugLogItem("Good Rot", loc.Rotation.ToString());
                GameView.AddDebugLogItem("Good Time", time.ToString());

                if (playerActor.ClearHistoryBeforeTime(time)) // if there were enough updates to clear any
                {
                    GameState.BoundableActor.Location locAfter = playerActor.GetLocationAtTime(time);

                    GameState.BoundableActor.Location locNowAfter = playerActor.GetLocationAtTime(now);

                    Vector3 d = locNowBefore.Position - locNowAfter.Position;
                  //  if (d.LengthFast > 0.0001)
                    // An error happend, do some debugin
                }

                playerActor.LogUpdates = false;

                lastFlushTime = now;
            }

            GameView.AddDebugLogItem("History Size",playerActor.InputHistory.Count.ToString());

            State.UpdateActors();

            GameState.BoundableActor.Location currentLoc = playerActor.GetLocation();

            GameView.AddDebugLogItem("Current Pos", currentLoc.Position.ToString());
            GameView.AddDebugLogItem("Current Rot", currentLoc.Rotation.ToString());
            GameView.AddDebugLogItem("Current Time", now.ToString());

            // see if it's time to remove any old player history
        }

        protected Vector3 SetCameraZ( Vector3 pos)
        {
            float depth = State.GameWorld.DropDepth(pos.X, pos.Y);
            if (depth != float.MinValue)
                pos.Z = depth;

            return pos;
        }

        public void Run()
        {
            if (Window != null)
                Window.Run();
        }

        public void Dispose()
        {
            if (Window != null)
                Window.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;
using Game.Messages;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Client
{
    public partial class ClientGame
    {
        public LocalPlayer PlayerActor = null;

        protected Vector3 SetCameraZ(Vector3 pos)
        {
            float depth = State.GameWorld.DropDepth(pos.X, pos.Y);
            if (depth != float.MinValue)
                pos.Z = depth;

            return pos;
        }

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
            CallPendingEvents();

            double now = State.Now;

            if (PlayerActor == null)
                return;

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

        void ToggleDebugDrawing_Changed(object sender, EventArgs args)
        {
            if (ToggleDrawing != null)
                ToggleDrawing(ToggleDebugDrawing, EventArgs.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using Math3D;

namespace Game
{
    public class StandardActors
    {
        public static void Register(GameState state)
        {
            state.RegisterFactory(new PlayerFactory());
            state.RegisterFactory(new LocalPlayerrFactory());
            state.RegisterFactory(new RemotePlayerFactory());
        }

        public static string LocalPlayer = "LocalPlayer";
        public static string RemotePlayer = "RemotePlayer";
    }
    public class PlayerActor : ControledActor
    {
        public PlayerActor(GameState state)
            : base(state)
        {
            Type = GameState.ActorType.Avatar;
        }
    }

    public class PlayerFactory : GameState.ActorFactory
    {
        public override string ActorClassName()
        {
            return "Player";
        }

        public override GameState.Actor NewActor(GameState state)
        {
            return new PlayerActor(state);
        }
    }

    public class RemotePlayer : PlayerActor
    {
        public RemotePlayer(GameState state)
            : base(state)
        {

        }
    }

    public class RemotePlayerFactory : PlayerFactory
    {
        public override string ActorClassName()
        {
            return StandardActors.RemotePlayer;
        }

        public override GameState.Actor NewActor(GameState state)
        {
            return new RemotePlayer(state);
        }
    }

    public class LocalPlayer : PlayerActor
    {
        public double LinearSpeed = 0;
        public double RotarySpeed = 0;

        public LocalPlayer(GameState state)
            : base(state)
        {
            LinearSpeed = state.Info.DefaultPlayerLinearSpeed;
            RotarySpeed = state.Info.DefaultPlayerTurnSpeed;
        }

        public delegate BoundingBox GetBoundsCB();
        public GetBoundsCB ComputeBoundingBox;

        public override BoundingBox GetBounds()
        {
            if (ComputeBoundingBox == null)
            {
                Location loc = GetLocation();

                return new BoundingBox(new Vector3(loc.Position.X - 1, loc.Position.Y - 1, loc.Position.Z), new Vector3(loc.Position.X + 1, loc.Position.Y + 1, loc.Position.Z+1));
            }
            else
                return ComputeBoundingBox();
        }

        public void AddVectorInput(float linear, float rotation, float tilt)
        {
            double Now = State.Now;

            double time = LastUpdateTime;
            if (InputHistory.Count > 0)
                time = InputHistory[InputHistory.Count - 1].InputTime;

            Location lastLoc = GetLocationAtTime(time);

            float rotaryDelta = (float)(rotation * RotarySpeed * (Now - time));

            Vector2 angleVec = VectorHelper2.FromAngle(lastLoc.Rotation.Z + rotaryDelta);

            HistoryItem item = new HistoryItem();
            item.InputTime = Now;
            item.Rotation = new Vector3(0, 0, (float)(rotation * RotarySpeed));
            item.Direction = new Vector3(angleVec.X * linear * (float)LinearSpeed, angleVec.Y * linear * (float)LinearSpeed, 0);

            AddHistoryItem(item);
        }

        public void AddInput(float linear, float desiredSpin, float desiredTilt)
        {
            //float movement = linear * (float)LinearSpeed;

            double Now = State.Now;
            // see where we are now
            double time = LastUpdateTime;
            if (InputHistory.Count > 0)
                time = InputHistory[InputHistory.Count-1].InputTime;

            Location lastLoc = GetLocationAtTime(time);

            double delta = Now - time;
            Vector3 EPRotation  = lastLoc.Rotation;

            double distToMove = Math.Abs(desiredSpin - EPRotation.Z);

            double rotDir = 1;
            if (distToMove != 0)
                rotDir = (desiredSpin - EPRotation.Z) / distToMove;

            double newRot = 0;
            double timeStamp = Now;
            double timeToMove = distToMove / RotarySpeed;

            float rotParam = 1;

            if (timeToMove >= delta) // not going to make it
                newRot = EPRotation.Z + (delta * RotarySpeed * rotDir);
            else
            {
                if (timeToMove == 0)
                    rotParam = 0; // we are there
                else
                {
                    timeStamp = time + timeToMove;
                    newRot = desiredSpin;
                }
            }

            Vector2 angleVec = VectorHelper2.FromAngle(newRot);

            HistoryItem item = new HistoryItem();
            item.InputTime = timeStamp;
            item.Rotation = new Vector3(0, 0, (float)(RotarySpeed * rotDir * rotParam));
            item.Direction = new Vector3(angleVec.X * linear, angleVec.Y * linear, 0);

            AddHistoryItem(item);

            if (item.InputTime != Now)
            {
                item = new HistoryItem();
                item.InputTime = timeStamp;
                item.Rotation = new Vector3(0, 0, 0);
                item.Direction = new Vector3(angleVec.X, angleVec.Y, 0);
                AddHistoryItem(item);
            }
        }
    }

    public class LocalPlayerrFactory : PlayerFactory
    {
        public override string ActorClassName()
        {
            return StandardActors.LocalPlayer;
        }

        public override GameState.Actor NewActor(GameState state)
        {
            return new LocalPlayer(state);
        }
    }
}

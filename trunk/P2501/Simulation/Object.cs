using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Math3D;
using OpenTK;

namespace Simulation
{
    public class ObjectState
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Heading = Vector3.Zero;
        public float Rotation = 0;
       
        public Vector3 Movement = Vector3.Zero;
        public float Spin = 0;
    }

    public class GUIDManager
    {
        static UInt64 lastID = 1;

        public static UInt64 NewGUID()
        {
            lastID++;
            return lastID;
        }
    }

    public class SimObject : Object
    {
        public Object Tag = null;
        
        public UInt64 ID;

        public float Radius = 0f;

        public ObjectState CurrentState = new ObjectState();

        public ObjectState LastUpdateState = new ObjectState();
        public double LastUpdateTime = -1;

        protected Sim sim;

        internal SimObject ( Sim s )
        {
            sim = s;
        }

        internal SimObject()
        {
            sim = null;
        }

        internal void SetSim(Sim s)
        {
            sim = s;
        }

        public virtual void CopyFrom ( SimObject obj)
        {
            Tag = obj.Tag;
            ID = obj.ID;
            Radius = obj.Radius;
            CurrentState = obj.CurrentState;
            LastUpdateState = obj.LastUpdateState;
            LastUpdateTime = obj.LastUpdateTime;
        }

        protected Vector3 PredictPosition(float delta)
        {
            return LastUpdateState.Position + LastUpdateState.Movement * delta;
        }

        protected float PredictRotation(float delta)
        {
            return LastUpdateState.Rotation + LastUpdateState.Spin * delta;
        }

        public virtual void Update(double time)
        {
            float delta = (float)(time - LastUpdateTime);

            CurrentState.Position = PredictPosition(delta);
            CurrentState.Rotation = PredictRotation(delta);

            CurrentState.Heading = VectorHelper3.FromAngle(CurrentState.Rotation);
        }
    }
}

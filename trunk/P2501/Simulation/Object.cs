/*
    Open Combat/Projekt 2501
    Copyright (C) 2010  Jeffery Allen Myers

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
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

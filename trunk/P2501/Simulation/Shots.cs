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

using OpenTK;
using Math3D;

namespace Simulation
{
    public class Shot : SimObject
    {
        public Player Owner;
        public float Damage = 0;
        public double Lifetime = 10.0;

        ObjectState lastBounce = new ObjectState();
       // double lastBounceTime = 0;

        internal Shot(Sim s)
            : base(s)
        {

        }

        public override void Update(double time)
        {
            float delta = (float)(time - LastUpdateTime);
            Vector3 pos = PredictPosition(delta);
            float rot = PredictRotation(delta);

            // do bounce collisions

            CurrentState.Position = pos;
            CurrentState.Rotation = rot;
            CurrentState.Heading = VectorHelper3.FromAngle(rot);
        }

        public bool Expired ( double time )
        {
            return (time - LastUpdateTime) > Lifetime;
        }
    }
}

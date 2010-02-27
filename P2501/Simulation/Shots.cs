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

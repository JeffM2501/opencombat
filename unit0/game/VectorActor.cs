using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Math3D;
using GridWorld;
using OpenTK;

namespace Game
{

    public class VectorActor : GameState.BoundableActor
    {
        public VectorActor(GameState state)
            : base(state)
        {

        }

        public double Birthday = 0;
        public Vector3 StartPosition = Vector3.Zero;
        public Vector3 Vector = Vector3.Zero;

        public double Lifespan = 0;

        public float SphereSize = 0.125f;

        protected BoundingBox bbox = BoundingBox.Empty;

        public override BoundingBox GetBounds()
        {
            Vector3 position = GetLocation().Position;

            if (bbox == BoundingBox.Empty)
                bbox = new BoundingBox(position, position);

            bbox.Max.X = position.X + SphereSize;
            bbox.Max.Y = position.Y + SphereSize;
            bbox.Max.Z = position.Z + SphereSize;

            bbox.Min.X = position.X - SphereSize;
            bbox.Min.Y = position.Y - SphereSize;
            bbox.Min.Z = position.Z - SphereSize;

            return bbox;
        }

        public override Location GetLocationAtTime(double time)
        {
            return new Location(StartPosition + (Vector * (float)(time - Birthday)));
        }

        public override bool Update()
        {
            base.Update();
            CurrentLocation = GetLocationAtTime(State.Now);

            if (State.ThisUpdateTime >= Birthday + Lifespan || State.GameWorld.PositionIsOffMap(CurrentLocation.Position) || !State.GameWorld.BlockPositionIsPassable(CurrentLocation.Position))
                DeleteMe = true;

            Moved = true;
            return true;
        }
    }
}
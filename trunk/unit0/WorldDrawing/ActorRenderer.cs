using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;

namespace WorldDrawing
{
    public class ActorRenderer : IDisposable
    {
        public GameState.BoundableActor Actor = null;

        public ActorRenderer(GameState.BoundableActor actor)
        {
            Actor = actor;
        }

        public virtual void Draw()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}

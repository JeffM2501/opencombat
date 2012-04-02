using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Math3D;
using GridWorld;
using OpenTK;
using System.Diagnostics;

namespace Game
{
    public class GameState
    {
        public delegate World GetWorldCB();
        public delegate void ActorEvent(GameState sender, Actor actor);

        public GetWorldCB GetWorld = null;

        public event EventHandler<EventArgs> MapLoaded;
        public event EventHandler<EventArgs> RegisterFactories;
        public event ActorEvent ActorCreated;
        public event ActorEvent ActorDeleted;
        public event ActorEvent ActorUpdated;

        public World GameWorld = null;

        public class GameInfo
        {
            public double DefaultPlayerLinearSpeed = 10.0;
            public double DefaultPlayerTurnSpeed = 90.0;
        }

        public GameInfo Info = new GameInfo();

        public enum ActorType
        {
            Unknown,
            Avatar,
            Pickup,
            Scenery,
            Effect,
            Munition,
        }

        public class Actor
        {
            public UInt64 GUID = UInt64.MaxValue;
            public ActorType Type = ActorType.Unknown;
            public string ClassName = string.Empty;
            public string Name = string.Empty;
            public bool HasBounds = false;

            public static UInt64 LastGUID = UInt64.MinValue;

            public GameState State {get; protected set;}

            public Actor( GameState state )
            {
                State = state;
                GUID = LastGUID;
                LastGUID++;
            }

            public bool DeleteMe { get; protected set; }

            public virtual bool Update() { DeleteMe = false; return false; }

            public object SimTag = null;
            public object RenderTag = null;
        }

        public class BoundableActor : Actor , IOctreeObject
        {
            public BoundingBox GetOctreeBounds(){return GetBounds();}

            public virtual BoundingBox GetBounds() {return BoundingBox.Empty;}

            public class Location
            {
                public Vector3 Position = Vector3.Zero;
                public Vector3 Rotation = Vector3.Zero;

                public static Location Zero = new Location();

                public Location() { }
                public Location(Vector3 pos)
                {
                    Position = pos;
                }

                public Location(Vector3 pos, Vector3 rot)
                {
                    Position = pos;
                    Rotation = rot;
                }
            }

            protected Location CurrentLocation = new Location();

            public virtual Location GetLocation() { return CurrentLocation; }
            public virtual Location GetLocationAtTime( double time) { return Location.Zero; }

            public BoundableActor(GameState state)
                : base(state)
            {
                HasBounds = true;
            }

            public bool Moved = false;
            public override bool Update() { Moved = false; return base.Update(); }
        }

        public class ActorFactory
        {
            public virtual string ActorClassName() { return string.Empty; }
            public virtual Actor NewActor(GameState state) { return new Actor(state); }
        }

        protected Dictionary<string, ActorFactory> Factories = new Dictionary<string, ActorFactory>();

        public void RegisterFactory(ActorFactory factory)
        {
            if (!Factories.ContainsKey(factory.ActorClassName()))
                Factories.Add(factory.ActorClassName(), factory);
        }

        protected Dictionary<ActorType, List<Actor>> ActorsByType = new Dictionary<ActorType, List<Actor>>();
        protected Dictionary<UInt64, Actor> Actors = new Dictionary<UInt64, Actor>();

        public GameState()
        {
        }

        public void Load()
        {
            StandardActors.Register(this);
            if (RegisterFactories != null)
                RegisterFactories(this, EventArgs.Empty);

            if (GetWorld != null)
                GameWorld = GetWorld();
            else
                GameWorld = new World();

            GameWorld.Finailize();

            if (Actors.Count > 0)
            {
                foreach (Actor actor in Actors.Values)
                {
                    if (actor.HasBounds)
                        GameWorld.AddObject(actor as BoundableActor);
                }
            }

            if (MapLoaded != null)
                MapLoaded(this,EventArgs.Empty);
        }

        public Actor AddActor(string className)
        {
            return AddActor(className, UInt64.MaxValue);
        }

        public Actor AddActor(string className, UInt64 GUID)
        {
            if (!UpdateTimer.IsRunning)
                UpdateTimer.Start();

            Actor actor = null;

            if (!Factories.ContainsKey(className))
                return actor;

            ActorFactory factory = Factories[className];
                actor = factory.NewActor(this);
            if (GUID != UInt64.MaxValue)
                actor.GUID = GUID;

            lock (Actors)
            {
                if (!ActorsByType.ContainsKey(actor.Type))
                    ActorsByType.Add(actor.Type, new List<Actor>());

                ActorsByType[actor.Type].Add(actor);
            
                if (Actors.ContainsKey(actor.GUID))
                    RemoveActor(Actors[actor.GUID]);
               Actors.Add(actor.GUID, actor);
            }

            if (actor.HasBounds && GameWorld != null)
                GameWorld.AddObject(actor as BoundableActor);

            lock (actor)
            {
                if (ActorCreated != null)
                    ActorCreated(this, actor);
            }
            return actor;
        }

        public void RemoveActor(Actor actor)
        {
            lock (Actors)
            {
                if (actor.HasBounds)
                    GameWorld.RemoveObject(actor as BoundableActor);

                if (ActorsByType.ContainsKey(actor.Type))
                    ActorsByType[actor.Type].Remove(actor);

                if (Actors.ContainsKey(actor.GUID))
                    Actors.Remove(actor.GUID);
            }

            lock (actor)
            {
                if (ActorDeleted != null)
                    ActorDeleted(this, actor);
            }
        }

        public Actor FindActor(UInt64 GUID)
        {
            if (!Actors.ContainsKey(GUID))
                return null;

            return Actors[GUID];
        }

        public Actor[] GetActors(ActorType _type)
        {
            lock (Actors)
            {
                if (!ActorsByType.ContainsKey(_type))
                    return new Actor[0];

                return ActorsByType[_type].ToArray();
            }
        }

        public BoundableActor[] ActorsInFrustum(BoundingFrustum frustum)
        {
            if (GameWorld == null)
                return new BoundableActor[0];

            lock(Actors)
                return GameWorld.InFrustum<BoundableActor>(frustum).ToArray();
        }

        public BoundableActor[] ActorsInSphere(BoundingSphere sphere)
        {
            if (GameWorld == null)
                return new BoundableActor[0];

            lock (Actors)
                return GameWorld.InBoundingSphere<BoundableActor>(sphere).ToArray();
        }

        public double LastUpdateTime = 0;
        public double ThisUpdateTime = 0;
        public double UpdateDelta = 0;
        protected Stopwatch UpdateTimer = new Stopwatch();

        public static bool UseFixedTime = false;
        protected double LastFixedTime = 0;
        protected double ThisFixedTime = 0;
        protected double FixedTimeIncrement = 0.01;

        public double Now { get { if (UseFixedTime) return ThisFixedTime; else return UpdateTimer.ElapsedMilliseconds / 1000f; } }

        protected void UpdateActorPosition(Actor actor)
        {
            if (!actor.HasBounds)
                return;

            GameWorld.RemoveObject(actor as BoundableActor);
            GameWorld.AddObject(actor as BoundableActor);
        }

        public void UpdateActors()
        {
            if (!UpdateTimer.IsRunning)
                UpdateTimer.Start();

            if (UseFixedTime)
            {
                LastFixedTime = ThisFixedTime;
                ThisFixedTime += FixedTimeIncrement;
            }

            LastUpdateTime = ThisUpdateTime;
            ThisUpdateTime = Now;
            UpdateDelta = ThisUpdateTime - LastUpdateTime;

            List<Actor> toRemove = new List<Actor>();
            foreach (Actor actor in Actors.Values)
            {
                bool updated = actor.Update();
                if (updated)
                {
                    if (actor.HasBounds)
                    {
                        if ((actor as BoundableActor).Moved)
                            UpdateActorPosition(actor);
                    }

                    if (ActorUpdated != null)
                        ActorUpdated(this, actor);
                }

                if (actor.DeleteMe)
                    toRemove.Add(actor);
            }

            foreach (Actor actor in toRemove)
                RemoveActor(actor);
        }
    }
}

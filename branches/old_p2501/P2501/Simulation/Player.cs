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
    public enum PlayerStatus
    {
        Connecting = 0,
        Despawned = 1,
        Alive = 2,
        Dead = 3,
    }

    public class Player : SimObject
    {
        public string Callsign = string.Empty;
        public int Score = -1;
        public string Pilot = string.Empty;
        public string Avatar = string.Empty;
        public int Team = -1;

        protected bool spawned = false;
        public bool SpawnedOnce
        {
            get { return spawned; }
        }

        public int TeamPreference = -1;

        public float MovementSpeed
        {
            get { return forAftSpeed; }
        }

        object InputLocker = new object();
        float intendedAngle = 0;
        Vector2 intendedSpeed = new Vector2(0,0);
        float forAftSpeed = 0;
       // float sideSpeed = 0;

        public PlayerStatus Status = PlayerStatus.Connecting;

        bool hasInput = false;

        public virtual float TurningSpeed
        {
            get { return sim.Settings.BaseTurnSpeed; }
        }

        public virtual float TurningAcceleration
        {
            get { return sim.Settings.BaseTurnAcceleration; }
        }

        public virtual float ForwardSpeed
        {
            get { return sim.Settings.BaseSpeed; }
        }

        public virtual float BackwardSpeed
        {
            get { return sim.Settings.BaseSpeed * 0.5f; }
        }

        public virtual float SidewaysSpeed
        {
            get { return sim.Settings.BaseSpeed * 0.25f; }
        }

        public virtual float ForeAftAccelration
        {
            get { return sim.Settings.BaseAcceleration; }
        }

        public virtual float SidewaysAccelration
        {
            get { return sim.Settings.BaseAcceleration * 0.5f; }
        }


        internal Player ( Sim s ) :base(s)
        {

        }

        internal Player()
            : base()
        {

        }

        public virtual void CopyFrom ( Player player )
        {
            base.CopyFrom(player);
            Callsign = player.Callsign;
            Score = player.Score;
            Pilot = player.Pilot;
            Status = player.Status;
        }

        protected virtual void ProcessInput(double time)
        {
            lock (InputLocker)
            {
                if (!hasInput)
                    return;

                double timeDelta = time - LastUpdateTime;

                // rotation
                // check the intended angle and see if we are close to being there (we will be this update)
                // if so just snap to it.
                float delta = intendedAngle - LastUpdateState.Rotation;
                if (Math.Abs(delta) > 180)
                {
                    float sign = delta / (float)Math.Abs(delta);

                    delta = (float)Math.Abs(delta) - 180;
                    delta = 180 - delta;
                    delta *= -sign;
                }

                if (Math.Abs(delta) <= Math.Abs(LastUpdateState.Spin * timeDelta) || delta == 0)
                {
                    CurrentState.Rotation = LastUpdateState.Rotation = intendedAngle;
                    CurrentState.Spin = LastUpdateState.Spin = 0;
                }
                else
                {
                    LastUpdateState.Spin = (delta / Math.Abs(delta)) * TurningSpeed;

                    LastUpdateState.Rotation += LastUpdateState.Spin * (float)timeDelta;

                    if (LastUpdateState.Rotation > 180)
                        LastUpdateState.Rotation = -360 + LastUpdateState.Rotation;
                    if (LastUpdateState.Rotation < -180)
                        LastUpdateState.Rotation = 360 - LastUpdateState.Rotation;

                    CurrentState.Rotation = LastUpdateState.Rotation;
                }

                //thrust

                // see what way we want to go
                float speedDelta = intendedSpeed.X - forAftSpeed;

                if (intendedSpeed.X < 0 && forAftSpeed <= 0) // going backwards and want to go backwards
                {
                    if (Math.Abs(speedDelta) < ForeAftAccelration * timeDelta)
                        forAftSpeed = intendedSpeed.X;
                    else
                        forAftSpeed -= ForeAftAccelration * (float)timeDelta;

                    if (forAftSpeed < -BackwardSpeed)
                        forAftSpeed = -BackwardSpeed;
                }
                else if (intendedSpeed.X > 0 && forAftSpeed >= 0) // going forward and want to go forwards
                {
                    if (Math.Abs(speedDelta) < ForeAftAccelration * timeDelta)
                        forAftSpeed = intendedSpeed.X;
                    else
                        forAftSpeed += ForeAftAccelration * (float)timeDelta;

                    if (forAftSpeed > ForwardSpeed)
                        forAftSpeed = ForwardSpeed;
                }
                else if (intendedSpeed.X != forAftSpeed) // if we are going as fast as we want we are done
                {
                    if (intendedSpeed.X > forAftSpeed)
                        forAftSpeed += ForeAftAccelration * (float)timeDelta;
                    else
                        forAftSpeed -= ForeAftAccelration * (float)timeDelta;
                }

                LastUpdateState.Movement = VectorHelper3.FromAngle(LastUpdateState.Rotation) * forAftSpeed;

                CurrentState.Movement = LastUpdateState.Movement;
                LastUpdateState.Position += LastUpdateState.Movement * (float)timeDelta;
                LastUpdateTime = time;
            }
        }

        public override void Update(double time)
        {
            if (Status == PlayerStatus.Alive)
            {
                ProcessInput(time);

                float delta = (float)(time - LastUpdateTime);
                Vector3 pos = PredictPosition(delta);
                float rot= PredictRotation(delta);

                // do bounce collisions

                CurrentState.Spin = LastUpdateState.Spin;
                CurrentState.Position = pos;
                CurrentState.Rotation = rot;
                CurrentState.Heading = VectorHelper3.FromAngle(rot);
            }
        }

        public virtual void Turn ( float param )
        {
            lock(InputLocker)
            {
                hasInput = false;
                LastUpdateState.Spin = TurningSpeed * param;
            }
        }

        public virtual void TurnTo ( float angle )
        {
            lock (InputLocker)
            {
                hasInput = true;
                intendedAngle = angle;
            }
        }

        public virtual void Thrust ( float param )
        {
            lock (InputLocker)
            {
                hasInput = true;
                if (param > 0)
                    intendedSpeed.X = ForwardSpeed * param;
                else
                    intendedSpeed.X = BackwardSpeed * param;
            }
        }

        public virtual void ThrustOff( double delta )
        {
            lock (InputLocker)
            {
                if (intendedSpeed.X > 0)
                    intendedSpeed.X -= 25f * (float)delta;
                else if (intendedSpeed.X < 0)
                    intendedSpeed.X += 25f * (float)delta;

                if (Math.Abs(intendedSpeed.X) < 0.05f)
                    intendedSpeed.X = 0;
            }
        }

        public virtual void Sideswipe ( float param )
        {
            lock (InputLocker)
            {
                hasInput = true;
                intendedSpeed.Y = SidewaysSpeed * param;
            }
        }
    }
}

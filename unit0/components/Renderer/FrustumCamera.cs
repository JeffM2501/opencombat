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
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Math3D;

namespace Renderer
{
    public class FrustumCamera : Camera
    {
        Vector3 position = new Vector3();
        public Vector3 EyePoint
        {
            get { return position; }
        }

        float tilt = 0, spin = 0;

        public float Tilt
        {
            get { return tilt; }
        }

        public float Spin
        {
            get { return spin; }
        }

        float aspect = 1;
        float fov = 45f;

        int width = 1;
        int height = 1;

        public float FOV
        {
            get { return fov; }
            set { fov = FOV; SetPersective(); }
        }

        public float FOVX
        {
            get { return fov / aspect; }
        }

        float hither = 0.01f;
        public float NearPlane
        {
            get { return hither; }
            set { hither = NearPlane; SetPersective(); }
        }

        float yon = 1000.0f;
        public float FarPlane
        {
            get { return yon; }
            set { yon = FarPlane; SetPersective(); }
        }

        VisibleFrustum frustum = new VisibleFrustum();
        public VisibleFrustum ViewFrustum
        {
            get { return frustum; }
        }

        public void move (Vector3 pos)
        {
            position += pos;
        }

        public void move(float x, float y, float z)
        {
            position.X += x;
            position.Y += y;
            position.Z += z;
        }

        public void MoveRelitive(Vector3 vec )
        {
            MoveRelitive(vec.Y, vec.X, vec.Z);
        }

        public void MoveRelitive(float forward, float sideways, float up)
        {
            Vector3 forwardVec = new Vector3(Forward());
            Vector3 leftwardVec = Vector3.Cross(new Vector3(0,0,1),forwardVec);
            Vector3 upwardVec = Vector3.Cross(forwardVec, leftwardVec);

            Vector3 incremnt = new Vector3();
            incremnt += forwardVec * forward;
            incremnt += leftwardVec * sideways;
            incremnt += upwardVec * up;

            position += incremnt;
        }

        public void turn( float _tilt, float _spin )
        {
            tilt += _tilt;
            spin += _spin;
        }

        public void set(Vector3 pos, float _tilt, float _spin)
        {
            position = pos;
            tilt = _tilt;
            spin = _spin;
        }

        public void set(Vector3 pos)
        {
            position = pos;
        }

        public void setPos(float x, float y, float z)
        {
            position.X = x;
            position.Y = y;
            position.Z = z;
        }

        public void set(float _tilt, float _spin)
        {
            tilt = _tilt;
            spin = _spin;
        }


        public float HeadingAngle ()
        {
            return spin;
        }

        public Vector2 Heading ()
        {
            return new Vector2((float)Math.Cos(Trig.DegreeToRadian(spin)), (float)Math.Sin(Trig.DegreeToRadian(spin)));
        }

        public Vector3 Forward()
        {
            Vector3 forward = new Vector3(Heading());
            forward.Z = (float)Math.Tan(Trig.DegreeToRadian(tilt));
            forward.Normalize();
            return forward;
        }

        public void ApplyPrespectiveMatrix()
        {
            frustum.SetProjection(fov, aspect, hither, yon, width, height);
            GL.MultTransposeMatrix(ref frustum.projection);
        }

        public void SetPersective ()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            ApplyPrespectiveMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void SetOrthographic()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, 0, height, 0, yon);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public void Resize(Int32 _width, Int32 _height)
        {
            width = _width;
            height = _height;

            aspect = width/(float)height;
            SetPersective();
        }

        public void Execute()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            frustum.LookAt(position, position + Forward());
            GL.MultTransposeMatrix(ref frustum.view);
        }

        public VisibleFrustum SnapshotFrusum ( )
        {
            VisibleFrustum f = new VisibleFrustum();

            f.SetProjection(fov, aspect, hither, yon, width, height);
            f.LookAt(position, position + Forward());

            return f;
        }

        public void SimpleBillboard()
        {
            GL.Rotate(spin - 90, 0, 0, 1); 
            GL.Rotate(tilt, 1, 0, 0);
        }

        public override void DoBillboard() { SimpleBillboard(); }
        public override Vector3 GetPostion() { return EyePoint; }
        public override double GetSpin() { return HeadingAngle(); }
        public override double GetTilt() { return Tilt; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
// use compatiblity for GLU
using OpenTK.Graphics;

#pragma warning disable 618

namespace Renderer
{
    public class Camera
    {
        public virtual void DoBillboard() { }
        public virtual Vector3 GetPostion() { return Vector3.Zero;}
        public virtual double GetSpin() { return 0; }
        public virtual double GetTilt() { return 0; }
    }

    public class SimpleCamera : Camera
    {
        public bool Ortho = true;

        public int FOV = 45;
        public float PerspectiveNear = 1f;
        public float PerspectiveFar = 1000f;

        public float OrthoNear = 0;
        public float OrthoFar = 10f;

        public Vector3 ViewPosition = Vector3.Zero;
        public double Spin = 0;
        public double Tilt = 0;
        public double Pullback = 0;

        public int Width = 0;
        public int Height = 0;

        public override void DoBillboard() { ExecuteBillboard(); }
        public override Vector3 GetPostion() { return ViewPosition; }
        public override double GetSpin() { return Spin; }
        public override double GetTilt() { return Tilt; }

        public void Resize ( int x, int y)
        {
            Width = x;
            Height = y;
       /*     if (Ortho)
                SetOrthographic();
            else
                SetPerspective(); */
        }
      
        public void SetPerspective()
        {
            Ortho = false;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float aspect = (float)Width / (float)Height;

            Glu.Perspective(FOV, aspect, PerspectiveNear, PerspectiveFar);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void SetOrthographic()
        {
            Ortho = true;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, 0, Height, OrthoNear, OrthoFar);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Execute()
        {
            if (!Ortho)
            {
                SetPerspective();
                GL.LoadIdentity();

                GL.Translate(0, 0, -Pullback);						// pull back on along the zoom vector
                GL.Rotate(Tilt, 1.0f, 0.0f, 0.0f);					// pops us to the tilt
                GL.Rotate(-Spin+90.0, 0.0f, 1.0f, 0.0f);					// gets us on our rot
                GL.Translate(-ViewPosition.X, -ViewPosition.Z, ViewPosition.Y);	                        // take us to the pos
                GL.Rotate(-90, 1.0f, 0.0f, 0.0f);				    // gets us into XY
            }
            else
            {
                SetOrthographic();
                GL.LoadIdentity();
            }
        }

        public void ExecuteBillboard()
        {
            GL.Rotate(Spin, 0, 0, 1);
            GL.Rotate(-Tilt, 1, 0, 0);
        }
    }
}

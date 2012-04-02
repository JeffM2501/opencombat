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

using OpenTK;

namespace Math3D
{
    class Polygon
    {
        List<Vector3> points;
        Plane plane;

        public bool Valid ()
        {
            return points.Count > 2;
        }

        public bool Clip ( Plane clipPlane)
        {
            List<Vector3> newPoints = new List<Vector3>();

            foreach (Vector3 p in points)
            {
                if (clipPlane.Intersects(p) != PlaneIntersectionType.Back)
                    newPoints.Add(p);
            }
            points = newPoints;
            return Valid();
        }

        public static Polygon Clip ( Polygon input, Plane clipPlane )
        {
            List<Vector3> newPoints = new List<Vector3>();

            foreach(Vector3 p in input.points)
            {
                if (clipPlane.Intersects(p) != PlaneIntersectionType.Back)
                    newPoints.Add(p);
            }

            Polygon outPoly = new Polygon();
            outPoly.plane = new Plane(input.plane.Normal, input.plane.D);
            outPoly.points = newPoints;

            return outPoly;
        }
    }
}

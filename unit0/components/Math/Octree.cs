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
using System.Xml;
using System.Xml.Serialization;

using OpenTK;

namespace Math3D
{
//     public class OctreeObject
//     {
//         public BoundingBox bounds = BoundingBox.Empty;
//     }

    public interface IOctreeObject
    {
        BoundingBox GetOctreeBounds();
    }

    public class OctreeLeaf
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        const int maxDepth = 40;
       
        [System.Xml.Serialization.XmlIgnoreAttribute]
        const bool doFastOut = true;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        int maxObjects = 8;
        
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<object> containedObjects = new List<object>();
       
        public List<OctreeLeaf> children = null;
        public BoundingBox bounds;

        public OctreeLeaf(BoundingBox containerBox)
        {
            bounds = containerBox;
        }

        public List<object> ContainedObjects
        {
            get { return containedObjects; }
            set { containedObjects = value; }
        }

        public List<OctreeLeaf> ChildLeaves
        {
            get { return children; }
        }

        public BoundingBox ContainerBox
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public void FastRemove(object item)
        {
            if (ContainedObjects.Contains(item))
            {
                List<OctreeLeaf> toRemove = new List<OctreeLeaf>();

                ContainedObjects.Remove(item);
                foreach (OctreeLeaf leaf in ChildLeaves)
                {
                    leaf.FastRemove(item);
                    if (leaf.children == null || leaf.children.Count == 0)
                        toRemove.Add(leaf);
                }

                foreach (OctreeLeaf leaf in toRemove)
                    ChildLeaves.Remove(leaf);
            }
        }

        protected void Split()
        {
            if (children != null)
                return;

            Vector3 half = ContainerBox.Max - ContainerBox.Min;
            half *= 0.5f;
            Vector3 halfx = new Vector3(half.X, 0, 0);
            Vector3 halfy = new Vector3(0, half.Y, 0);
            Vector3 halfz = new Vector3(0, 0, half.Z);

            children = new List<OctreeLeaf>();

            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min, ContainerBox.Min + half)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfx, ContainerBox.Max - half + halfx)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfz, ContainerBox.Min + half + halfz)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfx + halfz, ContainerBox.Max - halfy)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfy, ContainerBox.Max - halfx - halfz)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfy + halfx, ContainerBox.Max - halfz)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + halfy + halfz, ContainerBox.Max - halfx)));
            ChildLeaves.Add(new OctreeLeaf(new BoundingBox(ContainerBox.Min + half, ContainerBox.Max)));
        }

        public void Distribute(int depth)
        {
            if (containedObjects.Count > maxObjects && depth <= maxDepth)
            {
                Split();
                for (int i = containedObjects.Count - 1; i >= 0; i--)// (OctreeObject item in containedObjects)
                {
                    object item = containedObjects[i];
                    foreach (OctreeLeaf leaf in ChildLeaves)
                    {
                        IOctreeObject o = item as IOctreeObject;
                        BoundingBox bounds = o.GetOctreeBounds();
                        if (leaf.ContainerBox.Contains(bounds) == ContainmentType.Contains)
                        {
                            leaf.ContainedObjects.Add(item);
                            containedObjects.Remove(item);
                            break;
                        }
                    }
                }

                depth++;
                foreach (OctreeLeaf leaf in ChildLeaves)
                    leaf.Distribute(depth);
                depth--;
            }
        }

        protected void FastAddChildren(List<object> objects)
        {
            foreach (object item in containedObjects)
                objects.Add(item);

            if (ChildLeaves != null)
            {
                foreach (OctreeLeaf leaf in ChildLeaves)
                    leaf.FastAddChildren(objects);
            }
        }

        public virtual void ObjectsInFrustum(List<object> objects, BoundingFrustum boundingFrustum)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (doFastOut && boundingFrustum.Contains(ContainerBox) == ContainmentType.Contains)
                FastAddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (object item in containedObjects) // add our straglers
                    objects.Add(item);

                if (ChildLeaves != null)
                {
                    foreach (OctreeLeaf leaf in ChildLeaves)
                    {
                        // if the child is totally in the volume then add it and it's kids
                        if (doFastOut && boundingFrustum.Contains(leaf.ContainerBox) == ContainmentType.Contains)
                            leaf.FastAddChildren(objects);
                        else
                        {
                            if (boundingFrustum.Intersects(leaf.ContainerBox))
                                leaf.ObjectsInFrustum(objects, boundingFrustum);
                        }

                    }
                }
            }
        }

        public virtual void ObjectsInBoundingBox(List<object> objects, BoundingBox boundingBox)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (boundingBox.Contains(ContainerBox) == ContainmentType.Contains)
                FastAddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (object item in containedObjects) // add our straglers
                    objects.Add(item);

                if (ChildLeaves != null)
                {
                    foreach (OctreeLeaf leaf in ChildLeaves)
                    {
                        // see if any of the sub boxes intesect our frustum
                        if (leaf.ContainerBox.Intersects(boundingBox))
                            leaf.ObjectsInBoundingBox(objects, boundingBox);
                    }
                }
            }
        }

        public virtual void ObjectsInBoundingSphere(List<object> objects, BoundingSphere boundingSphere)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (boundingSphere.Contains(ContainerBox) == ContainmentType.Contains)
                FastAddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (object item in containedObjects) // add our straglers
                    objects.Add(item);

                if (ChildLeaves != null)
                {
                    foreach (OctreeLeaf leaf in ChildLeaves)
                    {
                        // see if any of the sub boxes intesect our frustum
                        if (leaf.ContainerBox.Intersects(boundingSphere))
                            leaf.ObjectsInBoundingSphere(objects, boundingSphere);
                    }
                }
            }
        }

        protected ContainmentType testBoxInFrustum(BoundingBox extents, BoundingFrustum frustum)
        {
            // TODO - use a sphere vs. cone test first?

            Vector3 inside;  // inside point  (assuming partial)
            Vector3  outside; // outside point (assuming partial)
            float len = 0;
            ContainmentType result = ContainmentType.Contains;

            foreach (Plane plane in FrustumHelper.GetPlanes(frustum))
            {
                // setup the inside/outside corners
                // this can be determined easily based
                // on the normal vector for the plane
                if (plane.Normal.X > 0.0f)
                {
                    inside.X = extents.Max.X;
                    outside.X = extents.Min.X;
                }
                else
                {
                    inside.X = extents.Min.X;
                    outside.X = extents.Max.X;
                }

                if (plane.Normal.Y > 0.0f)
                {
                    inside.Y = extents.Max.Y;
                    outside.Y = extents.Min.Y;
                }
                else
                {
                    inside.Y = extents.Min.Y;
                    outside.Y = extents.Max.Y;
                }

                if (plane.Normal.Z > 0.0f)
                {
                    inside.Z = extents.Max.Z;
                    outside.Z = extents.Min.Z;
                }
                else
                {
                    inside.Z = extents.Min.Z;
                    outside.Z = extents.Max.Z;
                }
              
                // check the inside length
                len = plane.Distance(inside);
                if (len < -1.0f)
                    return ContainmentType.Disjoint; // box is fully outside the frustum

                // check the outside length
                len = plane.Distance(outside);
                if (len < -1.0f)
                    result = ContainmentType.Intersects; // partial containment at best
            }

            return result;
        }
    }

    public class Octree : OctreeLeaf
    {
        public Octree()
            : base(new BoundingBox())
        {
        }

        public void Bounds()
        {
            foreach (IOctreeObject item in ContainedObjects)
            {
                ContainerBox = BoundingBox.CreateMerged(ContainerBox, item.GetOctreeBounds());
            }
        }

        public virtual void Add(IEnumerable<object> items)
        {
            foreach (object item in items)
                ContainedObjects.Add(item);

            Bounds();
            base.Distribute(0);
        }

        public virtual void Add(object item)
        {
            ContainedObjects.Add(item);

            Bounds();
            base.Distribute(0);
        }

        public override void ObjectsInFrustum(List<object> objects, BoundingFrustum boundingFrustum)
        {
            bool useTree = true;
            if (useTree)
                base.ObjectsInFrustum(objects, boundingFrustum);
            else // brute force to see if our box in frustum works
                AddInFrustum(objects, boundingFrustum,this);
        }

        protected void AddInFrustum(List<object> objects, BoundingFrustum boundingFrustum, OctreeLeaf leaf)
        {
            foreach (IOctreeObject item in leaf.containedObjects)
            {
                if (boundingFrustum.Intersects(item.GetOctreeBounds()))
                    objects.Add(item);
            }

            if (leaf.ChildLeaves != null)
            {
                foreach (OctreeLeaf child in leaf.ChildLeaves)
                    AddInFrustum(objects, boundingFrustum, child);
            }
        }

        public override void ObjectsInBoundingBox(List<object> objects, BoundingBox box)
        {
            base.ObjectsInBoundingBox(objects, box);
        }

        public override void ObjectsInBoundingSphere(List<object> objects, BoundingSphere sphere)
        {
            base.ObjectsInBoundingSphere(objects, sphere);
        }
    }
}

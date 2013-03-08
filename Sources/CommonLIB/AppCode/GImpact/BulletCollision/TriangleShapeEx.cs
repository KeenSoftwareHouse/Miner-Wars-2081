/*
 * 
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
This source file is part of GIMPACT Library.

For the latest info, see http://gimpact.sourceforge.net/

Copyright (c) 2007 Francisco Leon Najera. C.C. 80087371.
email: projectileman@yahoo.com


This software is provided 'as-is', without any express or implied warranty.
In no event will the authors be held liable for any damages arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it freely,
subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using BulletXNA.LinearMath;
using MinerWarsMath;

namespace BulletXNA.BulletCollision
{
    //! Structure for collision
    public class GIM_TRIANGLE_CONTACT
    {
        public const int MAX_TRI_CLIPPING = 16;

        public float m_penetration_depth;
        public int m_point_count;
        public Vector4 m_separating_normal;
        public IndexedVector3[] m_points = new IndexedVector3[MAX_TRI_CLIPPING];

        public void CopyFrom(GIM_TRIANGLE_CONTACT other)
        {
            m_penetration_depth = other.m_penetration_depth;
            m_separating_normal = other.m_separating_normal;
            m_point_count = other.m_point_count;
            int i = m_point_count;
            while (i-- != 0)
            {
                m_points[i] = other.m_points[i];
            }
        }

        public GIM_TRIANGLE_CONTACT()
        {
        }

        public GIM_TRIANGLE_CONTACT(GIM_TRIANGLE_CONTACT other)
        {
            CopyFrom(other);
        }

        //! classify points that are closer
        public void MergePoints(ref Vector4 plane, float margin, ObjectArray<IndexedVector3> points, int point_count)
        {
            m_point_count = 0;
            m_penetration_depth = -1000.0f;

            int[] point_indices = new int[MAX_TRI_CLIPPING];

            int _k;

            for (_k = 0; _k < point_count; _k++)
            {
                float _dist = -ClipPolygon.DistancePointPlane(ref plane, ref points.GetRawArray()[_k]) + margin;

                if (_dist >= 0.0f)
                {
                    if (_dist > m_penetration_depth)
                    {
                        m_penetration_depth = _dist;
                        point_indices[0] = _k;
                        m_point_count = 1;
                    }
                    else if ((_dist + MathUtil.SIMD_EPSILON) >= m_penetration_depth)
                    {
                        point_indices[m_point_count] = _k;
                        m_point_count++;
                    }
                }
            }

            for (_k = 0; _k < m_point_count; _k++)
            {
                m_points[_k] = points[point_indices[_k]];
            }
        }

    }

    public class PrimitiveTriangle
    {
        public IndexedVector3[] m_vertices = new IndexedVector3[3];
        public Vector4 m_plane;
        public float m_margin;
        //float m_dummy;
        public PrimitiveTriangle()
        {
            m_margin = 0.01f;
        }


        public void BuildTriPlane()
        {
            IndexedVector3 normal = IndexedVector3.Cross(m_vertices[1] - m_vertices[0], m_vertices[2] - m_vertices[0]);
            normal.Normalize();
            m_plane = new Vector4(normal.ToVector3(), IndexedVector3.Dot(m_vertices[0], normal));
        }

        //! Test if triangles could collide
        public bool OverlapTestConservative(PrimitiveTriangle other)
        {
            float total_margin = m_margin + other.m_margin;
            // classify points on other triangle
            float dis0 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[0]) - total_margin;

            float dis1 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[1]) - total_margin;

            float dis2 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[2]) - total_margin;

            if (dis0 > 0.0f && dis1 > 0.0f && dis2 > 0.0f) return false;

            // classify points on this triangle
            dis0 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[0]) - total_margin;

            dis1 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[1]) - total_margin;

            dis2 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[2]) - total_margin;

            if (dis0 > 0.0f && dis1 > 0.0f && dis2 > 0.0f) return false;

            return true;

        }

        //! Calcs the plane which is paralele to the edge and perpendicular to the triangle plane
        /*!
        \pre this triangle must have its plane calculated.
        */
        public void GetEdgePlane(int edge_index, out Vector4 plane)
        {
            IndexedVector3 e0 = m_vertices[edge_index];
            IndexedVector3 e1 = m_vertices[(edge_index + 1) % 3];
            IndexedVector3 planeNormal = new IndexedVector3(m_plane.X, m_plane.Y, m_plane.Z);
            GeometeryOperations.bt_edge_plane(ref e0, ref e1, ref planeNormal, out plane);
        }

        public void ApplyTransform(ref IndexedMatrix t)
        {
            IndexedVector3.Transform(m_vertices, ref t, m_vertices);
        }

        //! Clips the triangle against this
        /*!
        \pre clipped_points must have MAX_TRI_CLIPPING size, and this triangle must have its plane calculated.
        \return the number of clipped points
        */
        public int ClipTriangle(PrimitiveTriangle other, ObjectArray<IndexedVector3> clipped_points)
        {
            // edge 0

            ObjectArray<IndexedVector3> temp_points = new ObjectArray<IndexedVector3>(GIM_TRIANGLE_CONTACT.MAX_TRI_CLIPPING);

            Vector4 edgeplane;

            GetEdgePlane(0, out edgeplane);


            int clipped_count = ClipPolygon.PlaneClipTriangle(ref edgeplane, ref other.m_vertices[0], ref other.m_vertices[1], ref other.m_vertices[2], temp_points);

            if (clipped_count == 0)
            {
                return 0;
            }

            ObjectArray<IndexedVector3> temp_points1 = new ObjectArray<IndexedVector3>(GIM_TRIANGLE_CONTACT.MAX_TRI_CLIPPING);


            // edge 1
            GetEdgePlane(1, out edgeplane);


            clipped_count = ClipPolygon.PlaneClipPolygon(ref edgeplane, temp_points, clipped_count, temp_points1);

            if (clipped_count == 0)
            {
                return 0;
            }

            // edge 2
            GetEdgePlane(2, out edgeplane);

            clipped_count = ClipPolygon.PlaneClipPolygon(ref edgeplane, temp_points1, clipped_count, clipped_points);

            return clipped_count;

        }

        //! Find collision using the clipping method
        /*!
        \pre this triangle and other must have their triangles calculated
        */
        public bool FindTriangleCollisionClipMethod(PrimitiveTriangle other, GIM_TRIANGLE_CONTACT contacts)
        {
            float margin = m_margin + other.m_margin;

            ObjectArray<IndexedVector3> clipped_points = new ObjectArray<IndexedVector3>(GIM_TRIANGLE_CONTACT.MAX_TRI_CLIPPING);

            int clipped_count;
            //create planes
            // plane v vs U points

            GIM_TRIANGLE_CONTACT contacts1 = new GIM_TRIANGLE_CONTACT();

            contacts1.m_separating_normal = m_plane;

            clipped_count = ClipTriangle(other, clipped_points);

            if (clipped_count == 0)
            {
                return false;//Reject
            }

            //find most deep interval face1
            contacts1.MergePoints(ref contacts1.m_separating_normal, margin, clipped_points, clipped_count);
            if (contacts1.m_point_count == 0)
            {
                return false; // too far
            }
            //Normal pointing to this triangle
            contacts1.m_separating_normal *= -1.0f;


            //Clip tri1 by tri2 edges
            GIM_TRIANGLE_CONTACT contacts2 = new GIM_TRIANGLE_CONTACT();
            contacts2.m_separating_normal = other.m_plane;

            clipped_count = other.ClipTriangle(this, clipped_points);

            if (clipped_count == 0)
            {
                return false;//Reject
            }

            //find most deep interval face1
            contacts2.MergePoints(ref contacts2.m_separating_normal, margin, clipped_points, clipped_count);
            if (contacts2.m_point_count == 0)
            {
                return false; // too far
            }

            ////check most dir for contacts
            if (contacts2.m_penetration_depth < contacts1.m_penetration_depth)
            {
                contacts.CopyFrom(contacts2);
            }
            else
            {
                contacts.CopyFrom(contacts1);
            }
            return true;

        }
    }



    //! Helper class for colliding Bullet Triangle Shapes
    /*!
    This class implements a better getAabb method than the previous btTriangleShape class
    */
    //public class TriangleShapeEx : TriangleShape
    //{
    //    public TriangleShapeEx()
    //        : base(IndexedVector3.Zero, IndexedVector3.Zero, IndexedVector3.Zero)
    //    {
    //    }

    //    public TriangleShapeEx(ref IndexedVector3 p0, ref IndexedVector3 p1, ref IndexedVector3 p2)
    //        : base(ref p0, ref p1, ref p2)
    //    {
    //    }

    //    public TriangleShapeEx(TriangleShapeEx other)
    //        : base(ref other.m_vertices1[0], ref other.m_vertices1[1], ref other.m_vertices1[2])
    //    {
    //    }

    //    public virtual void GetAabb(ref IndexedMatrix t, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
    //    {
    //        IndexedVector3 tv0 = t * m_vertices1[0];
    //        IndexedVector3 tv1 = t * m_vertices1[1];
    //        IndexedVector3 tv2 = t * m_vertices1[2];

    //        AABB trianglebox = new AABB(ref tv0, ref tv1, ref tv2, m_collisionMargin);
    //        aabbMin = trianglebox.m_min;
    //        aabbMax = trianglebox.m_max;
    //    }

    //    public void ApplyTransform(ref IndexedMatrix t)
    //    {
    //        IndexedVector3.Transform(m_vertices1, ref t, m_vertices1);
    //    }

    //    public void BuildTriPlane(out Vector4 plane)
    //    {
    //        IndexedVector3 normal = IndexedVector3.Cross(m_vertices1[1] - m_vertices1[0], m_vertices1[2] - m_vertices1[0]);
    //        normal.Normalize();
    //        plane = new Vector4(normal.ToVector3(), IndexedVector3.Dot(m_vertices1[0], normal));
    //    }

    //    public bool OverlapTestConservative(TriangleShapeEx other)
    //    {
    //        float total_margin = GetMargin() + other.GetMargin();

    //        Vector4 plane0;
    //        BuildTriPlane(out plane0);
    //        Vector4 plane1;
    //        other.BuildTriPlane(out plane1);

    //        // classify points on other triangle
    //        float dis0 = ClipPolygon.DistancePointPlane(ref plane0, ref other.m_vertices1[0]) - total_margin;

    //        float dis1 = ClipPolygon.DistancePointPlane(ref plane0, ref other.m_vertices1[1]) - total_margin;

    //        float dis2 = ClipPolygon.DistancePointPlane(ref plane0, ref other.m_vertices1[2]) - total_margin;

    //        if (dis0 > 0.0f && dis1 > 0.0f && dis2 > 0.0f) return false;

    //        // classify points on this triangle
    //        dis0 = ClipPolygon.DistancePointPlane(ref plane1, ref m_vertices1[0]) - total_margin;

    //        dis1 = ClipPolygon.DistancePointPlane(ref plane1, ref m_vertices1[1]) - total_margin;

    //        dis2 = ClipPolygon.DistancePointPlane(ref plane1, ref m_vertices1[2]) - total_margin;

    //        if (dis0 > 0.0f && dis1 > 0.0f && dis2 > 0.0f) return false;

    //        return true;

    //    }
    //}

}

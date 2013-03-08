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
    public class ClipPolygon
    {

        public static float DistancePointPlane(ref Vector4 plane, ref IndexedVector3 point)
        {
            return point.Dot(new IndexedVector3(plane.X, plane.Y, plane.Z)) - plane.W;
        }

        /*! Vector blending
        Takes two vectors a, b, blends them together*/
        public static void VecBlend(ref IndexedVector3 vr, ref IndexedVector3 va, ref IndexedVector3 vb, float blend_factor)
        {
            vr = (1 - blend_factor) * va + blend_factor * vb;
        }

        //! This function calcs the distance from a 3D plane
        public static void PlaneClipPolygonCollect(
                               ref IndexedVector3 point0,
                               ref IndexedVector3 point1,
                               float dist0,
                               float dist1,
                               ObjectArray<IndexedVector3> clipped,
                               ref int clipped_count)
        {
            bool _prevclassif = (dist0 > MathUtil.SIMD_EPSILON);
            bool _classif = (dist1 > MathUtil.SIMD_EPSILON);
            if (_classif != _prevclassif)
            {
                float blendfactor = -dist0 / (dist1 - dist0);
                VecBlend(ref clipped.GetRawArray()[clipped_count], ref point0, ref point1, blendfactor);
                clipped_count++;
            }
            if (!_classif)
            {
                clipped[clipped_count] = point1;
                clipped_count++;
            }
        }


        //! Clips a polygon by a plane
        /*!
        *\return The count of the clipped counts
        */
        public static int PlaneClipPolygon(
                               ref Vector4 plane,
                               ObjectArray<IndexedVector3> polygon_points,
                               int polygon_point_count,
                               ObjectArray<IndexedVector3> clipped)
        {
            int clipped_count = 0;

            IndexedVector3[] rawPoints = polygon_points.GetRawArray();

            //clip first point
            float firstdist = DistancePointPlane(ref plane, ref rawPoints[0]); ;
            if (!(firstdist > MathUtil.SIMD_EPSILON))
            {
                clipped[clipped_count] = polygon_points[0];
                clipped_count++;
            }

            float olddist = firstdist;
            for (int i = 1; i < polygon_point_count; i++)
            {
                float dist = DistancePointPlane(ref plane, ref rawPoints[i]);

                PlaneClipPolygonCollect(
                                ref rawPoints[i - 1], ref rawPoints[i],
                                olddist,
                                dist,
                                clipped,
                                ref clipped_count);


                olddist = dist;
            }

            //RETURN TO FIRST  point

            PlaneClipPolygonCollect(
                            ref rawPoints[polygon_point_count - 1], ref rawPoints[0],
                            olddist,
                            firstdist,
                            clipped,
                            ref clipped_count);

            return clipped_count;
        }

        //! Clips a polygon by a plane
        /*!
        *\param clipped must be an array of 16 points.
        *\return The count of the clipped counts
        */
        public static int PlaneClipTriangle(
                               ref Vector4 plane,
                               ref IndexedVector3 point0,
                               ref IndexedVector3 point1,
                               ref IndexedVector3 point2,
                               ObjectArray<IndexedVector3> clipped // an allocated array of 16 points at least
                               )
        {
            int clipped_count = 0;

            //clip first point0
            float firstdist = DistancePointPlane(ref plane, ref point0);
            if (!(firstdist > MathUtil.SIMD_EPSILON))
            {
                clipped[clipped_count] = point0;
                clipped_count++;
            }

            // point 1
            float olddist = firstdist;
            float dist = DistancePointPlane(ref plane, ref point1);

            PlaneClipPolygonCollect(
                            ref point0, ref point1,
                            olddist,
                            dist,
                            clipped,
                            ref clipped_count);

            olddist = dist;


            // point 2
            dist = DistancePointPlane(ref plane, ref point2);

            PlaneClipPolygonCollect(
                            ref point1, ref point2,
                            olddist,
                            dist,
                            clipped,
                            ref clipped_count);
            olddist = dist;



            //RETURN TO FIRST  point0
            PlaneClipPolygonCollect(
                            ref point2, ref point0,
                            olddist,
                            firstdist,
                            clipped,
                            ref clipped_count);

            return clipped_count;
        }
    }
}





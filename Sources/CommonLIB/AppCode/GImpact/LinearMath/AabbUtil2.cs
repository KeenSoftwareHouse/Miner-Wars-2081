/*
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
 * Bullet Continuous Collision Detection and Physics Library
 * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose, 
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Diagnostics;
using BulletXNA.LinearMath;

namespace BulletXNA
{
    public static class AabbUtil2
    {
        public static void AabbExpand(ref IndexedVector3 aabbMin,
                                   ref IndexedVector3 aabbMax,
                                   ref IndexedVector3 expansionMin,
                                   ref IndexedVector3 expansionMax)
        {
            aabbMin = aabbMin + expansionMin;
            aabbMax = aabbMax + expansionMax;
        }

        /// conservative test for overlap between two aabbs
        public static bool TestPointAgainstAabb2(ref IndexedVector3 aabbMin1, ref IndexedVector3 aabbMax1, ref IndexedVector3 point)
        {
            bool overlap = true;
            overlap = (aabbMin1.X > point.X || aabbMax1.X < point.X) ? false : overlap;
            overlap = (aabbMin1.Z > point.Z || aabbMax1.Z < point.Z) ? false : overlap;
            overlap = (aabbMin1.Y > point.Y || aabbMax1.Y < point.Y) ? false : overlap;
            return overlap;
        }


        /// conservative test for overlap between two aabbs
        public static bool TestAabbAgainstAabb2(ref IndexedVector3 aabbMin1, ref IndexedVector3 aabbMax1,
                                        ref IndexedVector3 aabbMin2, ref IndexedVector3 aabbMax2)
        {
            bool overlap = true;
            overlap = (aabbMin1.X > aabbMax2.X || aabbMax1.X < aabbMin2.X) ? false : overlap;
            overlap = (aabbMin1.Z > aabbMax2.Z || aabbMax1.Z < aabbMin2.Z) ? false : overlap;
            overlap = (aabbMin1.Y > aabbMax2.Y || aabbMax1.Y < aabbMin2.Y) ? false : overlap;
            return overlap;
        }

        /// conservative test for overlap between triangle and aabb
        public static bool TestTriangleAgainstAabb2(ref IndexedVector3 vertices0, ref IndexedVector3 vertices1, ref IndexedVector3 vertices2,
                                            ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
        {
            if (Math.Min(Math.Min(vertices0.X, vertices1.X), vertices2.X) > aabbMax.X) return false;
            if (Math.Max(Math.Max(vertices0.X, vertices1.X), vertices2.X) < aabbMin.X) return false;

            if (Math.Min(Math.Min(vertices0.Z, vertices1.Z), vertices2.Z) > aabbMax.Z) return false;
            if (Math.Max(Math.Max(vertices0.Z, vertices1.Z), vertices2.Z) < aabbMin.Z) return false;

            if (Math.Min(Math.Min(vertices0.Y, vertices1.Y), vertices2.Y) > aabbMax.Y) return false;
            if (Math.Max(Math.Max(vertices0.Y, vertices1.Y), vertices2.Y) < aabbMin.Y) return false;
            return true;
        }


        //public static bool TestTriangleAgainstAabb2(IList<IndexedVector3> vertices,
        //                            ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
        //{
        //    if (Math.Min(Math.Min(vertices[0].X, vertices[1].X), vertices[2].X) > aabbMax.X) return false;
        //    if (Math.Max(Math.Max(vertices[0].X, vertices[1].X), vertices[2].X) < aabbMin.X) return false;

        //    if (Math.Min(Math.Min(vertices[0].Z, vertices[1].Z), vertices[2].Z) > aabbMax.Z) return false;
        //    if (Math.Max(Math.Max(vertices[0].Z, vertices[1].Z), vertices[2].Z) < aabbMin.Z) return false;

        //    if (Math.Min(Math.Min(vertices[0].Y, vertices[1].Y), vertices[2].Y) > aabbMax.Y) return false;
        //    if (Math.Max(Math.Max(vertices[0].Y, vertices[1].Y), vertices[2].Y) < aabbMin.Y) return false;
        //    return true;
        //}




        public static int Outcode(ref IndexedVector3 p, ref IndexedVector3 halfExtent)
        {
            return (p.X < -halfExtent.X ? 0x01 : 0x0) |
                   (p.X > halfExtent.X ? 0x08 : 0x0) |
                   (p.Y < -halfExtent.Y ? 0x02 : 0x0) |
                   (p.Y > halfExtent.Y ? 0x10 : 0x0) |
                   (p.Z < -halfExtent.Z ? 0x4 : 0x0) |
                   (p.Z > halfExtent.Z ? 0x20 : 0x0);
        }



        public static bool RayAabb2(ref IndexedVector3 rayFrom,
                                  ref IndexedVector3 rayInvDirection,
                                  bool[] raySign,
                                  IndexedVector3[] bounds,
                                  out float tmin,
                                  float lambda_min,
                                  float lambda_max)
        {
            float tmax, tymin, tymax, tzmin, tzmax;
            tmin = (bounds[raySign[0] ? 1 : 0].X - rayFrom.X) * rayInvDirection.X;
            tmax = (bounds[1 - (raySign[0] ? 1 : 0)].X - rayFrom.X) * rayInvDirection.X;
            tymin = (bounds[raySign[1] ? 1 : 0].Y - rayFrom.Y) * rayInvDirection.Y;
            tymax = (bounds[1 - (raySign[1] ? 1 : 0)].Y - rayFrom.Y) * rayInvDirection.Y;

            if ((tmin > tymax) || (tymin > tmax))
                return false;

            if (tymin > tmin)
                tmin = tymin;

            if (tymax < tmax)
                tmax = tymax;

            tzmin = (bounds[(raySign[2] ? 1 : 0)].Z - rayFrom.Z) * rayInvDirection.Z;
            tzmax = (bounds[1 - (raySign[2] ? 1 : 0)].Z - rayFrom.Z) * rayInvDirection.Z;

            if ((tmin > tzmax) || (tzmin > tmax))
                return false;
            if (tzmin > tmin)
                tmin = tzmin;
            if (tzmax < tmax)
                tmax = tzmax;
            return ((tmin < lambda_max) && (tmax > lambda_min));
        }


        public static bool RayAabb(IndexedVector3 rayFrom,
                                    IndexedVector3 rayTo,
                                    ref IndexedVector3 aabbMin,
                                    ref IndexedVector3 aabbMax,
                                    ref float param, out IndexedVector3 normal)
        {
            return RayAabb(ref rayFrom, ref rayTo, ref aabbMin, ref aabbMax, ref param, out normal);
        }


        public static bool RayAabb(ref IndexedVector3 rayFrom,
                                    ref IndexedVector3 rayTo,
                                    ref IndexedVector3 aabbMin,
                                    ref IndexedVector3 aabbMax,
                                    ref float param, out IndexedVector3 normal)
        {
            IndexedVector3 aabbHalfExtent = (aabbMax - aabbMin) * 0.5f;
            IndexedVector3 aabbCenter = (aabbMax + aabbMin) * 0.5f;
            IndexedVector3 source = rayFrom - aabbCenter;
            IndexedVector3 target = rayTo - aabbCenter;
            int sourceOutcode = Outcode(ref source, ref aabbHalfExtent);
            int targetOutcode = Outcode(ref target, ref aabbHalfExtent);
            if ((sourceOutcode & targetOutcode) == 0x0)
            {
                float lambda_enter = 0f;
                float lambda_exit = param;
                IndexedVector3 r = target - source;
                int i;
                float normSign = 1;
                IndexedVector3 hitNormal = IndexedVector3.Zero;
                int bit = 1;

                for (int j = 0; j < 2; j++)
                {
                    for (i = 0; i != 3; ++i)
                    {
                        if ((sourceOutcode & bit) != 0)
                        {
                            float lambda = (-source[i] - aabbHalfExtent[i] * normSign) / r[i];

                            if (lambda_enter <= lambda)
                            {
                                lambda_enter = lambda;
                                hitNormal = IndexedVector3.Zero;
                                hitNormal[i] = normSign;
                            }
                        }
                        else if ((targetOutcode & bit) != 0)
                        {
                            float lambda = (-source[i] - aabbHalfExtent[i] * normSign) / r[i];
                            lambda_exit = Math.Min(lambda_exit, lambda);
                        }
                        bit <<= 1;
                    }
                    normSign = -1f;
                }
                if (lambda_enter <= lambda_exit)
                {
                    param = lambda_enter;
                    normal = hitNormal;
                    return true;
                }
            }
            param = 0f;
            normal = IndexedVector3.Zero;
            return false;
        }

        //This block replaces the block below and uses no branches, and replaces the 8 bit return with a 32 bit return for improved performance (~3x on XBox 360)
        public static bool TestQuantizedAabbAgainstQuantizedAabb(ref UShortVector3 aabbMin1, ref UShortVector3 aabbMax1, ref UShortVector3 aabbMin2, ref UShortVector3 aabbMax2)
        {
            //return ((aabbMin1[0] <= aabbMax2[0]) && (aabbMax1[0] >= aabbMin2[0])
            //    & (aabbMin1[2] <= aabbMax2[2]) && (aabbMax1[2] >= aabbMin2[2])
            //    & (aabbMin1[1] <= aabbMax2[1]) && (aabbMax1[1] >= aabbMin2[1]));
            //return (MathUtil.select(val,1, 0));

            // MAN - Not sure why this version isn't just replaced by anding all of the above, it's still not conditional as theres a quick ref.
            bool overlap = true;
            overlap = (aabbMin1.X > aabbMax2.X || aabbMax1.X < aabbMin2.X) ? false : overlap;
            overlap = (aabbMin1.Z > aabbMax2.Z || aabbMax1.Z < aabbMin2.Z) ? false : overlap;
            overlap = (aabbMin1.Y > aabbMax2.Y || aabbMax1.Y < aabbMin2.Y) ? false : overlap;
            return overlap;
        }

        public static void TransformAabb(IndexedVector3 halfExtents, float margin, ref IndexedMatrix t, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
        {
            TransformAabb(ref halfExtents, margin, ref t, out aabbMinOut, out aabbMaxOut);
        }

        public static void TransformAabb(ref IndexedVector3 halfExtents, float margin, ref IndexedMatrix t, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
        {
            IndexedVector3 halfExtentsWithMargin = halfExtents + new IndexedVector3(margin);
            IndexedBasisMatrix  abs_b = t._basis.Absolute();
            IndexedVector3 center = t._origin;

            IndexedVector3 extent = new IndexedVector3(abs_b[0].Dot(ref halfExtentsWithMargin),
                                                        abs_b[1].Dot(ref halfExtentsWithMargin),
                                                        abs_b[2].Dot(ref halfExtentsWithMargin));

            aabbMinOut = center - extent;
            aabbMaxOut = center + extent;
        }

        public static void TransformAabb(IndexedVector3 localAabbMin, IndexedVector3 localAabbMax, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
        {
            Debug.Assert(localAabbMin.X <= localAabbMax.X);
            Debug.Assert(localAabbMin.Y <= localAabbMax.Y);
            Debug.Assert(localAabbMin.Z <= localAabbMax.Z);
            IndexedVector3 localHalfExtents = -.5f * (localAabbMax - localAabbMin);
            localHalfExtents += new IndexedVector3(margin);

            IndexedVector3 localCenter = 0.5f * (localAabbMax + localAabbMin);
            IndexedBasisMatrix abs_b = trans._basis.Absolute();
            IndexedVector3 center = trans * localCenter;

            IndexedVector3 extent = new IndexedVector3(abs_b[0].Dot(ref localHalfExtents),
                                                        abs_b[1].Dot(ref localHalfExtents),
                                                        abs_b[2].Dot(ref localHalfExtents));

            aabbMinOut = center - extent;
            aabbMaxOut = center + extent;
        }

        public static void TransformAabb(ref IndexedVector3 localAabbMin, ref IndexedVector3 localAabbMax, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
        {
            Debug.Assert(localAabbMin.X <= localAabbMax.X);
            Debug.Assert(localAabbMin.Y <= localAabbMax.Y);
            Debug.Assert(localAabbMin.Z <= localAabbMax.Z);
            IndexedVector3 localHalfExtents = 0.5f * (localAabbMax - localAabbMin);
            localHalfExtents += new IndexedVector3(margin);

            IndexedVector3 localCenter = 0.5f * (localAabbMax + localAabbMin);
            IndexedBasisMatrix abs_b = trans._basis.Absolute();
            IndexedVector3 center = trans * localCenter;

            IndexedVector3 extent = new IndexedVector3(abs_b[0].Dot(ref localHalfExtents),
                                                        abs_b[1].Dot(ref localHalfExtents),
                                                        abs_b[2].Dot(ref localHalfExtents));

            aabbMinOut = center - extent;
            aabbMaxOut = center + extent;
        }
    }
}
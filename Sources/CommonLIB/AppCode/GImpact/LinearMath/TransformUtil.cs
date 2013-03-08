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
using System.Collections.Generic;

using MinerWarsMath;
using BulletXNA.LinearMath;

namespace BulletXNA
{
    public static class TransformUtil
    {
        public static IList<float> FloatToList(float f)
        {
            IList<float> list = new List<float>();
            list.Add(f);
            return list;
        }

        public static IList<float> VectorToList(IndexedVector3 vector)
        {
            return VectorToList(ref vector);
        }

        public static IList<float> VectorToList(ref IndexedVector3 vector)
        {
            IList<float> list = new List<float>();
            list.Add(vector.X);
            list.Add(vector.Y);
            list.Add(vector.Z);
            return list;
        }

        public static IList<IndexedVector3> VectorsFromList(IList<float> list)
        {
            IList<IndexedVector3> vecList = new List<IndexedVector3>();
            int numVectors = list.Count / 3;
            for(int i=0;i<numVectors;++i)
            {
                IndexedVector3 vec = new IndexedVector3(list[3*i],list[(3*i)+1],list[(3*i)+2]);
                vecList.Add(vec);
            }
            return vecList;
        }

        //public static IndexedVector3 InverseTransform(ref IndexedVector3 v, ref IndexedMatrix m)
        //{
        //    return IndexedVector3.Transform(v, IndexedMatrix.Invert(m));
        //}
        
        public static void PlaneSpace1(ref IndexedVector3 n, out IndexedVector3 p, out IndexedVector3 q)
        {
            if (Math.Abs(n.Z) > MathUtil.SIMDSQRT12)
            {
                // choose p in y-z plane
                float a = n.Y * n.Y + n.Z * n.Z;
                float k = MathUtil.RecipSqrt(a);
                p = new IndexedVector3(0, -n.Z * k, n.Y * k);
                // set q = n x p
                q = new IndexedVector3(a * k, -n.X * p.Z, n.X * p.Y);
            }
            else
            {
                // choose p in x-y plane
                float a = n.X * n.X + n.Y * n.Y;
                float k = MathUtil.RecipSqrt(a);
                p = new IndexedVector3(-n.Y * k, n.X * k, 0);
                // set q = n x p
                q = new IndexedVector3(-n.Z * p.Y, n.Z * p.X, a * k);
            }
        }


        public static IndexedVector3 AabbSupport(ref IndexedVector3 halfExtents,ref IndexedVector3 supportDir)
        {
	        return new IndexedVector3(supportDir.X < 0f ? -halfExtents.X : halfExtents.X,
              supportDir.Y < 0f ? -halfExtents.Y : halfExtents.Y,
              supportDir.Z < 0f ? -halfExtents.Z : halfExtents.Z); 
        }

        public static void IntegrateTransform(IndexedMatrix curTrans, IndexedVector3 linvel, IndexedVector3 angvel, float timeStep, out IndexedMatrix predictedTransform)
        {
            IntegrateTransform(ref curTrans, ref linvel, ref angvel, timeStep, out predictedTransform);
        }

	    public static void IntegrateTransform(ref IndexedMatrix curTrans,ref IndexedVector3 linvel,ref IndexedVector3 angvel,float timeStep,out IndexedMatrix predictedTransform)
	    {
            predictedTransform = IndexedMatrix.CreateTranslation(curTrans._origin + linvel * timeStep);
    //	#define QUATERNION_DERIVATIVE
	    #if QUATERNION_DERIVATIVE
            IndexedVector3 pos;
            Quaternion predictedOrn;
            IndexedVector3 scale;

            curTrans.Decompose(ref scale, ref predictedOrn, ref pos);


		    predictedOrn += (angvel * predictedOrn) * (timeStep * .5f));
		    predictedOrn.Normalize();
        #else
            //Exponential map
		    //google for "Practical Parameterization of Rotations Using the Exponential Map", F. Sebastian Grassia

		    IndexedVector3 axis;
		    float	fAngle = angvel.Length(); 
		    //limit the angular motion
		    if (fAngle*timeStep > ANGULAR_MOTION_THRESHOLD)
		    {
			    fAngle = ANGULAR_MOTION_THRESHOLD / timeStep;
		    }

		    if ( fAngle < 0.001f )
		    {
			    // use Taylor's expansions of sync function
			    axis   = angvel*( 0.5f*timeStep-(timeStep*timeStep*timeStep)*(0.020833333333f)*fAngle*fAngle );
		    }
		    else
		    {
			    // sync(fAngle) = sin(c*fAngle)/t
			    axis   = angvel*( (float)Math.Sin(0.5f*fAngle*timeStep)/fAngle );
		    }
		    Quaternion dorn = new Quaternion(axis.X,axis.Y,axis.Z,(float)Math.Cos( fAngle*timeStep*.5f) );

            Quaternion orn0 = curTrans.GetRotation();

		    Quaternion predictedOrn = dorn * orn0;
		    predictedOrn.Normalize();
	    #endif

            IndexedMatrix newMatrix = IndexedMatrix.CreateFromQuaternion(predictedOrn);
            predictedTransform._basis = newMatrix._basis;
	    }

        public static void CalculateVelocityQuaternion(ref IndexedVector3 pos0, ref IndexedVector3 pos1, ref Quaternion orn0, ref Quaternion orn1, float timeStep, out IndexedVector3 linVel, out IndexedVector3 angVel)
        {
            linVel = (pos1 - pos0) / timeStep;
            if (orn0 != orn1)
            {
                IndexedVector3 axis;
                float angle;
                CalculateDiffAxisAngleQuaternion(ref orn0, ref orn1, out axis, out angle);
                angVel = axis * (angle / timeStep);
            }
            else
            {
                angVel = IndexedVector3.Zero;
            }
        }

        public static void CalculateDiffAxisAngleQuaternion(ref Quaternion orn0, ref Quaternion orn1a, out IndexedVector3 axis, out float angle)
        {
            Quaternion orn1 = MathUtil.QuatFurthest(ref orn0, ref orn1a);
            Quaternion dorn = orn1 * MathUtil.QuaternionInverse(ref orn0);

            ///floating point inaccuracy can lead to w component > 1..., which breaks 
            dorn.Normalize();
            angle = MathUtil.QuatAngle(ref dorn);
            axis = new IndexedVector3(dorn.X, dorn.Y, dorn.Z);

            //check for axis length
            float len = axis.LengthSquared();
            if (len < MathUtil.SIMD_EPSILON * MathUtil.SIMD_EPSILON)
            {
                axis = new IndexedVector3(1f, 0, 0);
            }
            else
            {
                axis.Normalize();
            }
        }

        public static void CalculateVelocity(ref IndexedMatrix transform0, ref IndexedMatrix transform1, float timeStep, out IndexedVector3 linVel, out IndexedVector3 angVel)
        {
            linVel = (transform1._origin - transform0._origin) / timeStep;
            MathUtil.SanityCheckVector(ref linVel);
            IndexedVector3 axis;
            float angle;
            CalculateDiffAxisAngle(ref transform0, ref transform1, out axis, out angle);
            angVel = axis * (angle / timeStep);
            MathUtil.SanityCheckVector(ref angVel);
        }

        public static void CalculateDiffAxisAngle(ref IndexedMatrix transform0, ref IndexedMatrix transform1, out IndexedVector3 axis, out float angle)
        {
            //IndexedMatrix dmat = GetRotateMatrix(ref transform1) * IndexedMatrix.Invert(GetRotateMatrix(ref transform0));
            IndexedBasisMatrix dmat = transform1._basis * transform0._basis.Inverse();
            Quaternion dorn = Quaternion.Identity;
            GetRotation(ref dmat, out dorn);

            ///floating point inaccuracy can lead to w component > 1..., which breaks 
            dorn.Normalize();

            angle = MathUtil.QuatAngle(ref dorn);

            axis = new IndexedVector3(dorn.X, dorn.Y, dorn.Z);
            //axis[3] = float(0.);
            //check for axis length
            float len = axis.LengthSquared();
            if (len < MathUtil.SIMD_EPSILON * MathUtil.SIMD_EPSILON)
            {
                axis = new IndexedVector3(1,0,0);
            }
            else
            {
                axis.Normalize();
            }
        }



        public static void GetRotation(ref IndexedBasisMatrix a, out Quaternion rot)
        {
            rot = a.GetRotation();
        }

        public static Quaternion GetRotation(ref IndexedBasisMatrix a)
        {
            return a.GetRotation();
        }

        public static float ANGULAR_MOTION_THRESHOLD = .5f * MathUtil.SIMD_HALF_PI;

    }


    ///The btConvexSeparatingDistanceUtil can help speed up convex collision detection 
    ///by conservatively updating a cached separating distance/vector instead of re-calculating the closest distance
    public class ConvexSeparatingDistanceUtil
    {
        private Quaternion m_ornA;
        private Quaternion m_ornB;
        private IndexedVector3 m_posA;
        private IndexedVector3 m_posB;

        private IndexedVector3 m_separatingNormal;

        private float m_boundingRadiusA;
        private float m_boundingRadiusB;
        private float m_separatingDistance;

        public ConvexSeparatingDistanceUtil(float boundingRadiusA, float boundingRadiusB)
        {
            m_boundingRadiusA = boundingRadiusA;
            m_boundingRadiusB = boundingRadiusB;
            m_separatingDistance = 0f;
        }

        public float GetConservativeSeparatingDistance()
        {
            return m_separatingDistance;
        }

        public void UpdateSeparatingDistance(ref IndexedMatrix transA, ref IndexedMatrix transB)
        {
            IndexedVector3 toPosA = transA._origin;
            IndexedVector3 toPosB = transB._origin;
            Quaternion toOrnA = transA.GetRotation();
            Quaternion toOrnB = transB.GetRotation();

            if (m_separatingDistance > 0.0f)
            {
                IndexedVector3 linVelA;
                IndexedVector3 angVelA;
                IndexedVector3 linVelB;
                IndexedVector3 angVelB;

                TransformUtil.CalculateVelocityQuaternion(ref m_posA, ref toPosA, ref m_ornA, ref toOrnA, 1f, out linVelA, out angVelA);
                TransformUtil.CalculateVelocityQuaternion(ref m_posB, ref toPosB, ref m_ornB, ref toOrnB, 1f, out linVelB, out angVelB);
                float maxAngularProjectedVelocity = angVelA.Length() * m_boundingRadiusA + angVelB.Length() * m_boundingRadiusB;
                IndexedVector3 relLinVel = (linVelB - linVelA);
                float relLinVelocLength = IndexedVector3.Dot((linVelB - linVelA), m_separatingNormal);
                if (relLinVelocLength < 0f)
                {
                    relLinVelocLength = 0f;
                }

                float projectedMotion = maxAngularProjectedVelocity + relLinVelocLength;
                m_separatingDistance -= projectedMotion;
            }

            m_posA = toPosA;
            m_posB = toPosB;
            m_ornA = toOrnA;
            m_ornB = toOrnB;
        }

        void InitSeparatingDistance(ref IndexedVector3 separatingVector, float separatingDistance, ref IndexedMatrix transA, ref IndexedMatrix transB)
	    {
		    m_separatingNormal = separatingVector;
		    m_separatingDistance = separatingDistance;
    		
		    IndexedVector3 toPosA = transA._origin;
		    IndexedVector3 toPosB = transB._origin;
            Quaternion toOrnA = transA.GetRotation();
            Quaternion toOrnB = transB.GetRotation();
		    m_posA = toPosA;
		    m_posB = toPosB;
		    m_ornA = toOrnA;
		    m_ornB = toOrnB;
	    }
    }
}

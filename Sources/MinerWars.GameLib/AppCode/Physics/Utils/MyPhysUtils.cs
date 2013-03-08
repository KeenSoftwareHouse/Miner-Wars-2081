#region Using Statements

using System;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Utilities used in physics
    /// </summary>

    class MyPhysicsUtils
    {
        static MyPhysicsUtils()
        {
            m_zeroInertia = Matrix.Identity;
            m_zeroInertia.M11 = 0.0f;
            m_zeroInertia.M22 = 0.0f;
            m_zeroInertia.M33 = 0.0f;
            m_zeroInertia.M44 = 0.0f;
        }

        private static Matrix m_zeroInertia;

        public static Matrix ZeroInertiaTensor
        {
            get { return m_zeroInertia; }
        }

        static public void BoundingBoxAddPoint(ref BoundingBox aabb, Vector3 point)
        {
            if (aabb.Max.X < point.X)
                aabb.Max.X = point.X;

            if (aabb.Max.Y < point.Y)
                aabb.Max.Y = point.Y;

            if (aabb.Max.Z < point.Z)
                aabb.Max.Z = point.Z;

            if (aabb.Min.X > point.X)
                aabb.Min.X = point.X;

            if (aabb.Min.Y > point.Y)
                aabb.Min.Y = point.Y;

            if (aabb.Min.Z > point.Z)
                aabb.Min.Z = point.Z;
        }

        public static void NormalizeSafe(ref Vector3 vec)
        {
            float num0 = vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z;

            if (num0 != 0.0f)
            {
                float num1 = 1.0f / (float)System.Math.Sqrt(num0);
                vec.X *= num1; vec.Y *= num1; vec.Z *= num1;
            }
        }

        public static float PointPointDistance(Vector3 pt1, Vector3 pt2)
        {
            float num3 = pt1.X - pt2.X;
            float num2 = pt1.Y - pt2.Y;
            float num0 = pt1.Z - pt2.Z;
            float num4 = ((num3 * num3) + (num2 * num2)) + (num0 * num0);
            return (float)System.Math.Sqrt((double)num4);
        }

        public static float Max(float a, float b, float c)
        {
            float abMax = a > b ? a : b;

            return abMax > c ? abMax : c;
        }

        public static float Min(float a, float b, float c)
        {
            float abMin = a < b ? a : b;

            return abMin < c ? abMin : c;
        }

        public static void Orthonormalise(ref Matrix matrix)
        {
            float u11 = matrix.M11; float u12 = matrix.M12; float u13 = matrix.M13;
            float u21 = matrix.M21; float u22 = matrix.M22; float u23 = matrix.M23;
            float u31 = matrix.M31; float u32 = matrix.M32; float u33 = matrix.M33;

            float dot0, dot1;

            // u1
            float lengthSq0 = u11 * u11 + u12 * u12 + u13 * u13;
            float length0 = (float)System.Math.Sqrt(lengthSq0);
            u11 = u11 / length0;
            u12 = u12 / length0;
            u13 = u13 / length0;

            // u2
            dot0 = u11 * u21 + u12 * u22 + u13 * u23;
            u21 = u21 - dot0 * u11 / lengthSq0;
            u22 = u22 - dot0 * u12 / lengthSq0;
            u23 = u23 - dot0 * u13 / lengthSq0;

            float lengthSq1 = u21 * u21 + u22 * u22 + u23 * u23;
            float length1 = (float)System.Math.Sqrt(lengthSq1);
            u21 = u21 / length1;
            u22 = u22 / length1;
            u23 = u23 / length1;

            // u3
            dot0 = u11 * u31 + u12 * u32 + u13 * u33;
            dot1 = u21 * u31 + u22 * u32 + u23 * u33;
            u31 = u31 - dot0 * u11 / lengthSq0 - dot1 * u21 / lengthSq1;
            u32 = u32 - dot0 * u12 / lengthSq0 - dot1 * u22 / lengthSq1;
            u33 = u33 - dot0 * u13 / lengthSq0 - dot1 * u23 / lengthSq1;

            lengthSq0 = u31 * u31 + u32 * u32 + u33 * u33;
            length0 = (float)System.Math.Sqrt(lengthSq0);
            u31 = u31 / length0;
            u32 = u32 / length0;
            u33 = u33 / length0;

            matrix.M11 = u11; matrix.M12 = u12; matrix.M13 = u13;
            matrix.M21 = u21; matrix.M22 = u22; matrix.M23 = u23;
            matrix.M31 = u31; matrix.M32 = u32; matrix.M33 = u33;
        }

        public static float FLT_MAX = float.MaxValue;
        public static float FLT_MIN = float.MinValue;
        
        /// <summary>
        /// Computes the inertia tensor of a rigid body using box inertia definition
        /// </summary>
        public static void ComputeIntertiaTensor(MyRigidBody rbo)
        {
            MyCommonDebugUtils.AssertDebug(rbo != null);
            MyCommonDebugUtils.AssertDebug(rbo.GetRBElementList().Count > 0);
            MyCommonDebugUtils.AssertDebug(rbo.GetMass() > 0);

            float mass = rbo.GetMass();

            BoundingBox box;
            box.Min = new Vector3(FLT_MAX);
            box.Max = new Vector3(FLT_MIN);
            BoundingBox aabb;

            Matrix infTensor = new Matrix();
            infTensor.M11 = FLT_MAX;
            infTensor.M22 = FLT_MAX;
            infTensor.M33 = FLT_MAX;
            infTensor.M44 = 1.0f;
            if(rbo.IsStatic())
            {
                rbo.InertiaTensor = infTensor;
                return;
            }

            if (rbo.GetRBElementList().Count > 1)
            {
                for (int e = 0; e < rbo.GetRBElementList().Count; e++)
                {
                    MyRBElement el = rbo.GetRBElementList()[e];
                    switch (el.GetElementType())
                    {
                        case MyRBElementType.ET_TRIANGLEMESH:
                            {
                                rbo.InertiaTensor = infTensor;
                                return;
                            }
                            break;
                        case MyRBElementType.ET_VOXEL:
                            {
                                rbo.InertiaTensor = infTensor;
                                return;
                            }
                            break;
                        default:
                            {
                                aabb = el.GetWorldSpaceAABB();
                                box = BoundingBox.CreateMerged(box, aabb);
                            }
                            break;
                    }
                }

                Vector3 size = box.Max - box.Min;

                infTensor.M11 = mass * (size.Y * size.Y + size.Z * size.Z) / 12.0f;
                infTensor.M22 = mass * (size.X * size.X + size.Z * size.Z) / 12.0f;
                infTensor.M33 = mass * (size.X * size.X + size.Y * size.Y) / 12.0f;
                infTensor.M44 = 1.0f;

                rbo.InertiaTensor = infTensor;
                rbo.InvertInertiaTensor = Matrix.Invert(infTensor);
                return;
            }

            MyRBElement elem = rbo.GetRBElementList()[0];
            switch (elem.GetElementType())
            {
                case MyRBElementType.ET_TRIANGLEMESH:
                    {
                        rbo.InertiaTensor = infTensor;
                        infTensor.M11 = 0.0f;
                        infTensor.M22 = 0.0f;
                        infTensor.M33 = 0.0f;
                        infTensor.M44 = 0.0f;
                        rbo.InvertInertiaTensor = infTensor;
                        return;
                    }
                    break;
                case MyRBElementType.ET_VOXEL:
                    {
                        rbo.InertiaTensor = infTensor;
                        infTensor.M11 = 0.0f;
                        infTensor.M22 = 0.0f;
                        infTensor.M33 = 0.0f;
                        infTensor.M44 = 0.0f;
                        rbo.InvertInertiaTensor = infTensor;
                        return;
                    }
                case MyRBElementType.ET_SPHERE:
                    {
                        float radius = ((MyRBSphereElement)elem).Radius;

                        infTensor.M11 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M22 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M33 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M44 = 1.0f;

                        rbo.InertiaTensor = infTensor;
                        //rbo.InvertInertiaTensor = Matrix.Invert(infTensor);
                        return;
                    }
                    break;
                case MyRBElementType.ET_BOX:
                    {
                        //Vector3 size = ((MyRBBoxElement)elem).Size;

                        //infTensor.M11 = mass * (size.Y * size.Y + size.Z * size.Z) / 12.0f;
                        //infTensor.M22 = mass * (size.X * size.X + size.Z * size.Z) / 12.0f;
                        //infTensor.M33 = mass * (size.X * size.X + size.Y * size.Y) / 12.0f;
                        //infTensor.M44 = 1.0f;

                        //rbo.InertiaTensor = infTensor;
                        //rbo.InvertInertiaTensor = Matrix.Invert(infTensor);

                        // HACK: After speaking with PetrM, computing changed like box is sphere
                        float radius = ((MyRBBoxElement)elem).Size.Length() / 2;

                        infTensor.M11 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M22 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M33 = 2.0f / 5.0f * mass * radius * radius;
                        infTensor.M44 = 1.0f;

                        rbo.InertiaTensor = infTensor;
                        //rbo.InvertInertiaTensor = Matrix.Invert(infTensor);
                        return;
                    }
                    break;
                default:
                    MyCommonDebugUtils.AssertDebug(false);
                    break;
            }
        }

        public static Matrix GetSphereInertiaTensor(float radius, float mass)
        {
            Matrix inertiaTensor = new Matrix();

            inertiaTensor.M11 = 2.0f / 5.0f * mass * radius * radius;
            inertiaTensor.M22 = 2.0f / 5.0f * mass * radius * radius;
            inertiaTensor.M33 = 2.0f / 5.0f * mass * radius * radius;
            inertiaTensor.M44 = 1.0f;

            return inertiaTensor;
        }

        /// <summary>
        /// Unsafe class to access the vector and matrix index part
        /// </summary>
        public sealed class MyPhysicsUnsafe
        {
            public static unsafe float Get(ref Vector3 vec, int index)
            {
                fixed (Vector3* adr = &vec)
                {
                    return ((float*)adr)[index];
                }
            }

            public static unsafe float Get(Vector3 vec, int index)
            {
                Vector3* adr = &vec;
                return ((float*)adr)[index];
            }

            public static unsafe Vector3 Get(Matrix mat, int index)
            {
                float* adr = &mat.M11;
                adr += index;
                return ((Vector3*)adr)[index];
            }

            public static unsafe Vector3 Get(ref Matrix mat, int index)
            {
                fixed (float* adr = &mat.M11)
                {
                    return ((Vector3*)(adr + index))[index];
                }
            }

            public static unsafe void Get(ref Matrix mat, int index, out Vector3 vec)
            {
                fixed (float* adr = &mat.M11)
                {
                    vec = ((Vector3*)(adr + index))[index];
                }
            }
        }
    }

    //  This structure was created for avoiding an allocation of an array but with same indexing functionality (GC reasons)
    public struct MyFloatArray3
    {
        private float m_float0;
        private float m_float1;
        private float m_float2;

        public float this[int index]
        {
            get
            {
                AssertIndex(index);
                return Get(index);
            }
            set
            {
                AssertIndex(index);
                Set(index, value);
            }
        }

        unsafe float Get(int index)
        {
            fixed (float* adr = &this.m_float0)
            {
                return ((float*)adr)[index];
            }
        }

        unsafe void Set(int index, float value)
        {
            fixed (float* adr = &this.m_float0)
            {
                ((float*)adr)[index] = value;
            }
        }

        static void AssertIndex(int index)
        {
            MyCommonDebugUtils.AssertDebug((index >= 0) && (index <= 2));
        }
    }

    //  This structure was created for avoiding an allocation of an array but with same indexing functionality (GC reasons)
    public struct MyFloatArray13
    {
        private float m_float0;
        private float m_float1;
        private float m_float2;
        private float m_float3;
        private float m_float4;
        private float m_float5;
        private float m_float6;
        private float m_float7;
        private float m_float8;
        private float m_float9;
        private float m_float10;
        private float m_float11;
        private float m_float12;

        public float this[int index]
        {
            get
            {
                AssertIndex(index);
                return Get(index);
            }
            set
            {
                AssertIndex(index);
                Set(index, value);
            }
        }

        unsafe float Get(int index)
        {
            fixed (float* adr = &this.m_float0)
            {
                return ((float*)adr)[index];
            }
        }

        unsafe void Set(int index, float value)
        {
            fixed (float* adr = &this.m_float0)
            {
                ((float*)adr)[index] = value;
            }
        }

        static void AssertIndex(int index)
        {
            MyCommonDebugUtils.AssertDebug((index >= 0) && (index <= 12));
        }
    }

    //  This structure was created for avoiding an allocation of an array but with same indexing functionality (GC reasons)
    public struct MyVector3Array13
    {
        private Vector3 m_vec0;
        private Vector3 m_vec1;
        private Vector3 m_vec2;
        private Vector3 m_vec3;
        private Vector3 m_vec4;
        private Vector3 m_vec5;
        private Vector3 m_vec6;
        private Vector3 m_vec7;
        private Vector3 m_vec8;
        private Vector3 m_vec9;
        private Vector3 m_vec10;
        private Vector3 m_vec11;
        private Vector3 m_vec12;

        public Vector3 this[int index]
        {
            get
            {
                AssertIndex(index);
                return Get(index);
            }
            set
            {
                AssertIndex(index);
                Set(index, value);
            }
        }

        unsafe Vector3 Get(int index)
        {
            fixed (Vector3* adr = &this.m_vec0)
            {
                return ((Vector3*)adr)[index];
            }
        }

        unsafe void Set(int index, Vector3 value)
        {
            fixed (Vector3* adr = &this.m_vec0)
            {
                ((Vector3*)adr)[index] = value;
            }
        }

        static void AssertIndex(int index)
        {
            MyCommonDebugUtils.AssertDebug((index >= 0) && (index <= 12));
        }
    }

    //  This structure was created for avoiding an allocation of an array but with same indexing functionality (GC reasons)
    public struct MyVector3Array3
    {
        private Vector3 m_vec0;
        private Vector3 m_vec1;
        private Vector3 m_vec2;

        public Vector3 this[int index]
        {
            get
            {
                AssertIndex(index);
                return Get(index);
            }
            set
            {
                AssertIndex(index);
                Set(index, value);
            }
        }

        unsafe Vector3 Get(int index)
        {
            fixed (Vector3* adr = &this.m_vec0)
            {
                return ((Vector3*)adr)[index];
            }
        }

        unsafe void Set(int index, Vector3 value)
        {
            fixed (Vector3* adr = &this.m_vec0)
            {
                ((Vector3*)adr)[index] = value;
            }
        }

        static void AssertIndex(int index)
        {
            MyCommonDebugUtils.AssertDebug((index >= 0) && (index <= 2));
        }
    }
}
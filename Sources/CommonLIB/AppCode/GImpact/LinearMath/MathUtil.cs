///*
// * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
// *
// * Bullet Continuous Collision Detection and Physics Library
// * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
// *
// * This software is provided 'as-is', without any express or implied warranty.
// * In no event will the authors be held liable for any damages arising from
// * the use of this software.
// * 
// * Permission is granted to anyone to use this software for any purpose, 
// * including commercial applications, and to alter it and redistribute it
// * freely, subject to the following restrictions:
// * 
// * 1. The origin of this software must not be misrepresented; you must not
// *    claim that you wrote the original software. If you use this software
// *    in a product, an acknowledgment in the product documentation would be
// *    appreciated but is not required.
// * 2. Altered source versions must be plainly marked as such, and must not be
// *    misrepresented as being the original software.
// * 3. This notice may not be removed or altered from any source distribution.
// */

using System;
using System.Diagnostics;
using System.IO;
//using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;
using MinerWarsMath;
using System.Runtime.InteropServices;

namespace BulletXNA
{
    public static class MathUtil
    {
        public static float[,] BasisMatrixToFloatArray(ref IndexedBasisMatrix m)
        {
            float[,] result = new float[3, 3];
            result[0, 0] = m._Row0.X;
            result[0, 1] = m._Row0.Y;
            result[0, 2] = m._Row0.Z;
            result[1, 0] = m._Row1.X;
            result[1, 1] = m._Row1.Y;
            result[1, 2] = m._Row1.Z;
            result[2, 0] = m._Row2.X;
            result[2, 1] = m._Row2.Y;
            result[2, 2] = m._Row2.Z;
            return result;
        }

        public static void FloatArrayToBasisMatrix(float[,] f, ref IndexedBasisMatrix m)
        {
            m._Row0 = new IndexedVector3(f[0, 0], f[0, 1], f[0, 2]);
            m._Row1 = new IndexedVector3(f[1, 0], f[1, 1], f[1, 2]);
            m._Row2 = new IndexedVector3(f[2, 0], f[2, 1], f[2, 2]);
        }



        public static void InverseTransform(ref IndexedMatrix m, ref IndexedVector3 v, out IndexedVector3 o)
        {
            IndexedVector3 v1 = v - m._origin;
            o = m._basis.Transpose() * v1;
        }

        public static IndexedVector3 InverseTransform(ref IndexedMatrix m, ref IndexedVector3 v)
        {
            IndexedVector3 v1 = v - m._origin;
            return m._basis.Transpose() * v1;
        }

//        //public static IndexedMatrix TransposeTimesBasis(ref IndexedMatrix a, ref IndexedMatrix b)
//        public static IndexedMatrix TransposeTimesBasis(ref IndexedMatrix mA, ref IndexedMatrix mB)

//        {
//            IndexedMatrix ba = MathUtil.BasisMatrix(ref mA);
//            ba = IndexedMatrix.Transpose(ba);
//            IndexedMatrix bb = MathUtil.BasisMatrix(ref mB);
//            return BulletMatrixMultiply(ref ba, ref bb);
//        }

//        public static IndexedMatrix InverseTimes(IndexedMatrix a, IndexedMatrix b)
//        {
//            return InverseTimes(ref a, ref b);
//        }

//        public static IndexedMatrix InverseTimes(ref IndexedMatrix a, ref IndexedMatrix b)
//        {
//            IndexedMatrix m = IndexedMatrix.Invert(a);
//            return BulletMatrixMultiply(ref m, ref b);
//        }

//        public static IndexedMatrix TransposeBasis(IndexedMatrix m)
//        {
//            return TransposeBasis(ref m);
//        }

//        public static IndexedMatrix TransposeBasis(ref IndexedMatrix m)
//        {
//            return IndexedMatrix.Transpose(BasisMatrix(ref m));
//        }

//        public static IndexedMatrix InverseBasis(IndexedMatrix m)
//        {
//            return InverseBasis(ref m);
//        }

//        public static IndexedMatrix InverseBasis(ref IndexedMatrix m)
//        {
//            IndexedMatrix b = BasisMatrix(ref m);
//            b = IndexedMatrix.Invert(b);
//            return b;
//        }

//        public static float Cofac(ref IndexedMatrix m,int r1, int c1, int r2, int c2)
//        {
//            float a = MatrixComponent(ref m, r1, c1);
//            float b = MatrixComponent(ref m, r2, c2);
//            float c = MatrixComponent(ref m, r1, c2);
//            float d = MatrixComponent(ref m, r2, c1);

//            return a * b - c * d;
//        }


        public static float FSel(float a, float b, float c)
        {
            // dodgy but seems necessary for rounding issues.
            //return a >= -0.00001 ? b : c;
            return a >= 0 ? b : c;
        }

        public static int MaxAxis(ref IndexedVector3 a)
        {
            return a.X < a.Y ? (a.Y < a.Z ? 2 : 1) : (a.X < a.Z ? 2 : 0);
        }

        public static int MaxAxis(Vector4 a)
        {
            return MaxAxis(ref a);
        }

        public static int MaxAxis(ref Vector4 a)
        {
            int maxIndex = -1;
            float maxVal = -BT_LARGE_FLOAT;
            if (a.X > maxVal)
            {
                maxIndex = 0;
                maxVal = a.X;
            }
            if (a.Y > maxVal)
            {
                maxIndex = 1;
                maxVal = a.Y;
            }
            if (a.Z > maxVal)
            {
                maxIndex = 2;
                maxVal = a.Z;
            }
            if (a.W > maxVal)
            {
                maxIndex = 3;
                //maxVal = a.W;
            }
            return maxIndex;
        }

        public static int ClosestAxis(ref Vector4 a)
        {
            return MaxAxis(AbsoluteVector4(ref a));
        }


        public static Vector4 AbsoluteVector4(ref Vector4 vec)
        {
            return new Vector4(Math.Abs(vec.X), Math.Abs(vec.Y), Math.Abs(vec.Z), Math.Abs(vec.W));
        }

//        public static float VectorComponent(IndexedVector3 v, int i)
//        {
//            return VectorComponent(ref v, i);
//        }

//        public static float VectorComponent(ref IndexedVector3 v, int i)
//        {
//            switch (i)
//            {
//                case 0:
//                    return v.X;
//                case 1:
//                    return v.Y;
//                case 2:
//                    return v.Z;
//                default:
//                    Debug.Assert(false);
//                    return 0.0f;
//            }
//        }

//        public static void VectorComponent(ref IndexedVector3 v, int i, float f)
//        {
//            switch (i)
//            {
//                case 0:
//                    v.X = f;
//                    return;
//                case 1:
//                    v.Y = f;
//                    return;
//                case 2:
//                    v.Z = f;
//                    return;
//            }
//            Debug.Assert(false);
//        }

//        public static void VectorComponentAddAssign(ref IndexedVector3 v, int i, float f)
//        {
//            switch (i)
//            {
//                case 0:
//                    v.X += f;
//                    return;
//                case 1:
//                    v.Y += f;
//                    return;
//                case 2:
//                    v.Z += f;
//                    return;
//            }
//            Debug.Assert(false);
//        }

//        public static void VectorComponentMinusAssign(ref IndexedVector3 v, int i, float f)
//        {
//            switch (i)
//            {
//                case 0:
//                    v.X -= f;
//                    return;
//                case 1:
//                    v.Y -= f;
//                    return;
//                case 2:
//                    v.Z -= f;
//                    return;
//            }
//            Debug.Assert(false);
//        }

//        public static void VectorComponentMultiplyAssign(ref IndexedVector3 v, int i, float f)
//        {
//            switch (i)
//            {
//                case 0:
//                    v.X *= f;
//                    return;
//                case 1:
//                    v.Y *= f;
//                    return;
//                case 2:
//                    v.Z *= f;
//                    return;
//            }
//            Debug.Assert(false);
//        }

//        public static void VectorComponentDivideAssign(ref IndexedVector3 v, int i, float f)
//        {
//            switch (i)
//            {
//                case 0:
//                    v.X /= f;
//                    return;
//                case 1:
//                    v.Y /= f;
//                    return;
//                case 2:
//                    v.Z /= f;
//                    return;
//            }
//            Debug.Assert(false);
//        }


        public static float VectorComponent(Vector4 v, int i)
        {
            return VectorComponent(ref v, i);
        }

        public static float VectorComponent(ref Vector4 v, int i)
        {
            switch (i)
            {
                case 0:
                    return v.X;
                case 1:
                    return v.Y;
                case 2:
                    return v.Z;
                case 3:
                    return v.W;
                default:
                    Debug.Assert(false);
                    return 0.0f;
            }
        }



        public static void VectorComponent(ref Vector4 v, int i, float f)
        {
            switch (i)
            {
                case 0:
                    v.X = f;
                    return;
                case 1:
                    v.Y = f;
                    return;
                case 2:
                    v.Z = f;
                    return;
                case 3:
                    v.W = f;
                    return;
            }
            Debug.Assert(false);
        }

//        public static IndexedMatrix AbsoluteMatrix(IndexedMatrix input)
//        {
//            return AbsoluteMatrix(ref input);
//        }

//        public static IndexedMatrix AbsoluteMatrix(ref IndexedMatrix input)
//        {
//            IndexedMatrix output;
//            AbsoluteMatrix(ref input, out output);
//            return output;
//        }

//        public static void AbsoluteMatrix(ref IndexedMatrix input, out IndexedMatrix output)
//        {
//            output = new IndexedMatrix(
//                Math.Abs(input.M11),
//                Math.Abs(input.M12),
//                Math.Abs(input.M13),
//                Math.Abs(input.M14),
//                Math.Abs(input.M21),
//                Math.Abs(input.M22),
//                Math.Abs(input.M23),
//                Math.Abs(input.M24),
//                Math.Abs(input.M31),
//                Math.Abs(input.M32),
//                Math.Abs(input.M33),
//                Math.Abs(input.M34),
//                Math.Abs(input.M41),
//                Math.Abs(input.M42),
//                Math.Abs(input.M43),
//                Math.Abs(input.M44));
//        }

//        public static IndexedMatrix AbsoluteBasisMatrix(ref IndexedMatrix input)
//        {
//            IndexedMatrix output;
//            AbsoluteBasisMatrix(ref input, out output);
//            return output;
//        }

//        public static void AbsoluteBasisMatrix(ref IndexedMatrix input, out IndexedMatrix output)
//        {
//            output = new IndexedMatrix(
//                Math.Abs(input.M11), Math.Abs(input.M12), Math.Abs(input.M13), 0.0f,
//                Math.Abs(input.M21), Math.Abs(input.M22), Math.Abs(input.M23), 0.0f,
//                Math.Abs(input.M31), Math.Abs(input.M32), Math.Abs(input.M33), 0.0f,
//                0.0f, 0.0f, 0.0f, 1.0f);
//        }

//        public static void AbsoluteVector(ref IndexedVector3 input, out IndexedVector3 output)
//        {
//            output = new IndexedVector3(
//                Math.Abs(input.X),
//                Math.Abs(input.Y),
//                Math.Abs(input.Z));
//        }

//        public static void RotateVector(ref IndexedVector3 vec, ref IndexedMatrix m, out IndexedVector3 output)
//        {
//            Quaternion rotation;
//            IndexedVector3 component;
//            m.Decompose(out component, out rotation, out component);
//            output = IndexedVector3.Transform(vec, rotation);
//        }

//        public static void TransformAabb(IndexedVector3 halfExtents, float margin, IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
//        {
//            //TransformAabb(ref halfExtents,margin,ref trans,out aabbMinOut,out aabbMaxOut);
//            IndexedVector3 halfExtentsWithMargin = halfExtents + new IndexedVector3(margin);
//            IndexedVector3 center, extent;
//            AbsoluteExtents(ref trans, ref halfExtentsWithMargin, out center, out extent);
//            aabbMinOut = center - extent;
//            aabbMaxOut = center + extent;
//        }

//        public static void TransformAabb(ref IndexedVector3 halfExtents, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
//        {
//            IndexedVector3 halfExtentsWithMargin = halfExtents + new IndexedVector3(margin);
//            IndexedVector3 center, extent;
//            AbsoluteExtents(ref trans, ref halfExtentsWithMargin, out center, out extent);
//            aabbMinOut = center - extent;
//            aabbMaxOut = center + extent;
//        }

//        public static void TransformAabb(IndexedVector3 localAabbMin, IndexedVector3 localAabbMax, float margin, IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
//        {
//            TransformAabb(ref localAabbMin, ref localAabbMax, margin, ref trans, out aabbMinOut, out aabbMaxOut);
//        }

//        public static void TransformAabb(ref IndexedVector3 localAabbMin, ref IndexedVector3 localAabbMax, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
//        {
//            Debug.Assert(localAabbMin.X <= localAabbMax.X);
//            Debug.Assert(localAabbMin.Y <= localAabbMax.Y);
//            Debug.Assert(localAabbMin.Z <= localAabbMax.Z);
//            IndexedVector3 localHalfExtents = 0.5f * (localAabbMax - localAabbMin);
//            localHalfExtents += new IndexedVector3(margin);

//            IndexedVector3 localCenter = 0.5f * (localAabbMax + localAabbMin);
//            IndexedMatrix abs_b = MathUtil.AbsoluteBasisMatrix(ref trans);

//            IndexedVector3 center = IndexedVector3.Transform(localCenter, trans);

//            IndexedVector3 extent = new IndexedVector3(IndexedVector3.Dot(abs_b.Right, localHalfExtents),
//                                            IndexedVector3.Dot(abs_b.Up, localHalfExtents),
//                                            IndexedVector3.Dot(abs_b.Backward, localHalfExtents));

//            aabbMinOut = center - extent;
//            aabbMaxOut = center + extent;
//        }

//        public static void AbsoluteExtents(ref IndexedMatrix trans, ref IndexedVector3 vec, out IndexedVector3 center, out IndexedVector3 extent)
//        {
//            IndexedMatrix abs_b;
//            AbsoluteMatrix(ref trans, out abs_b);

//            center = trans._origin;
//            extent = new IndexedVector3(IndexedVector3.Dot(abs_b.Right, vec),
//                                            IndexedVector3.Dot(abs_b.Up, vec),
//                                            IndexedVector3.Dot(abs_b.Backward, vec));
//        }

//        public static void SetMatrixVector(ref IndexedMatrix matrix, int row, IndexedVector3 vector)
//        {
//            SetMatrixVector(ref matrix, row, ref vector);
//        }

//        public static void SetMatrixVector(ref IndexedMatrix matrix, int row, ref IndexedVector3 vector)
//        {
//            switch (row)
//            {
//                case 0:
//                    matrix.M11 = vector.X;
//                    matrix.M12 = vector.Y;
//                    matrix.M13 = vector.Z;
//                    return;
//                case 1:
//                    matrix.M21 = vector.X;
//                    matrix.M22 = vector.Y;
//                    matrix.M23 = vector.Z;
//                    return;
//                case 2:
//                    matrix.M31 = vector.X;
//                    matrix.M32 = vector.Y;
//                    matrix.M33 = vector.Z;
//                    return;
//                case 3:
//                    matrix.M41 = vector.X;
//                    matrix.M42 = vector.Y;
//                    matrix.M43 = vector.Z;
//                    return;
//            }
//            Debug.Assert(false);
//        }

//        public static void AddMatrixVector(ref IndexedMatrix matrix, int row, ref IndexedVector3 vector)
//        {
//            switch (row)
//            {
//                case 0:
//                    matrix.M11 += vector.X;
//                    matrix.M12 += vector.Y;
//                    matrix.M13 += vector.Z;
//                    return;
//                case 1:
//                    matrix.M21 += vector.X;
//                    matrix.M22 += vector.Y;
//                    matrix.M23 += vector.Z;
//                    return;
//                case 2:
//                    matrix.M31 += vector.X;
//                    matrix.M32 += vector.Y;
//                    matrix.M33 += vector.Z;
//                    return;
//                case 3:
//                    matrix.M41 += vector.X;
//                    matrix.M42 += vector.Y;
//                    matrix.M43 += vector.Z;
//                    return;
//            }
//            Debug.Assert(false);
//        }

        public static float Vector3Triple(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c)
        {
            return a.X * (b.Y * c.Z - b.Z * c.Y) +
                a.Y * (b.Z * c.X - b.X * c.Z) +
                a.Z * (b.X * c.Y - b.Y * c.X);
        }

//        // FIXME - MAN - make sure this is being called how we'd expect , may need to
//        // swap i,j for row/column differences
        
//        public static float MatrixComponent(ref IndexedMatrix m, int index)
//        {
//            //int i = index % 4;
//            //int j = index / 4;

//            int j = index % 4;
//            int i = index / 4;
            
//            return MatrixComponent(ref m,i,j);
//        }
        
//        public static float MatrixComponent(ref IndexedMatrix m, int row, int column)
//        {
//            switch (row)
//            {
//                case 0:
//                    if (column == 0) return m.M11;
//                    if (column == 1) return m.M12;
//                    if (column == 2) return m.M13;
//                    if (column == 3) return m.M14;
//                    break;
//                case 1:
//                    if (column == 0) return m.M21;
//                    if (column == 1) return m.M22;
//                    if (column == 2) return m.M23;
//                    if (column == 3) return m.M24;
//                    break;
//                case 2:
//                    if (column == 0) return m.M31;
//                    if (column == 1) return m.M32;
//                    if (column == 2) return m.M33;
//                    if (column == 3) return m.M34;
//                    break;
//                case 3:
//                    if (column == 0) return m.M41;
//                    if (column == 1) return m.M42;
//                    if (column == 2) return m.M43;
//                    if (column == 3) return m.M44;
//                    break;
//            }
//            return 0;
//        }

//        public static void MatrixComponent(ref IndexedMatrix m, int row, int column, float val)
//        {
//            switch (row)
//            {
//                case 0:
//                    if (column == 0) m.M11 = val;
//                    if (column == 1) m.M12 = val;
//                    if (column == 2) m.M13 = val;
//                    if (column == 3) m.M14 = val;
//                    break;
//                case 1:
//                    if (column == 0) m.M21 = val;
//                    if (column == 1) m.M22 = val;
//                    if (column == 2) m.M23 = val;
//                    if (column == 3) m.M24 = val;
//                    break;
//                case 2:
//                    if (column == 0) m.M31 = val;
//                    if (column == 1) m.M32 = val;
//                    if (column == 2) m.M33 = val;
//                    if (column == 3) m.M34 = val;
//                    break;
//                case 3:
//                    if (column == 0) m.M41 = val;
//                    if (column == 1) m.M42 = val;
//                    if (column == 2) m.M43 = val;
//                    if (column == 3) m.M44 = val;
//                    break;
//            }
//        }

//        public static IndexedVector3 MatrixColumn(IndexedMatrix matrix, int row)
//        {
//            return MatrixColumn(ref matrix, row);
//        }

//        public static IndexedVector3 MatrixColumn(ref IndexedMatrix matrix, int row)
//        {
//            IndexedVector3 vectorRow;
//            MatrixColumn(ref matrix, row, out vectorRow);
//            return vectorRow;
//        }

//        public static void MatrixColumn(IndexedMatrix matrix, int row, out IndexedVector3 vectorRow)
//        {
//            MatrixColumn(ref matrix,row, out vectorRow);
//        }

//        public static void MatrixColumn(ref IndexedMatrix matrix, int row, out IndexedVector3 vectorRow)
//        {
//            switch (row)
//            {
//                case 0:
//                    vectorRow = new IndexedVector3(matrix.M11, matrix.M12, matrix.M13);
//                    break;
//                case 1:
//                    vectorRow = new IndexedVector3(matrix.M21, matrix.M22, matrix.M23);
//                    break;
//                case 2:
//                    vectorRow = new IndexedVector3(matrix.M31, matrix.M32, matrix.M33);
//                    break;
//                case 3:
//                    vectorRow = new IndexedVector3(matrix.M41, matrix.M42, matrix.M43);
//                    break;
//                default:
//                    vectorRow = IndexedVector3.Zero;
//                    break;
//            }
//        }

//        public static IndexedVector3 MatrixRow(IndexedMatrix matrix, int row)
//        {
//            switch (row)
//            {
//                case 0:
//                    return new IndexedVector3(matrix.M11, matrix.M21, matrix.M31);
//                case 1:
//                    return new IndexedVector3(matrix.M12, matrix.M22, matrix.M32);
//                case 2:
//                    return new IndexedVector3(matrix.M13, matrix.M23, matrix.M33);
//                case 3:
//                    return new IndexedVector3(matrix.M14, matrix.M24, matrix.M34);
//                default:
//                    return IndexedVector3.Zero;
//            }
//        }

//        public static IndexedVector3 MatrixRow(ref IndexedMatrix matrix, int row)
//        {
//            switch (row)
//            {
//                case 0:
//                    return new IndexedVector3(matrix.M11, matrix.M21, matrix.M31);
//                case 1:
//                    return new IndexedVector3(matrix.M12, matrix.M22, matrix.M32);
//                case 2:
//                    return new IndexedVector3(matrix.M13, matrix.M23, matrix.M33);
//                case 3:
//                    return new IndexedVector3(matrix.M14, matrix.M24, matrix.M34);
//                default:
//                    return IndexedVector3.Zero;
//            }
//        }

//        public static void MatrixRow(ref IndexedMatrix matrix, int row, out IndexedVector3 vectorRow)
//        {
//            switch (row)
//            {
//                case 0:
//                    vectorRow = new IndexedVector3(matrix.M11, matrix.M21, matrix.M31);
//                    break;
//                case 1:
//                    vectorRow = new IndexedVector3(matrix.M12, matrix.M22, matrix.M32);
//                    break;
//                case 2:
//                    vectorRow = new IndexedVector3(matrix.M13, matrix.M23, matrix.M33);
//                    break;
//                case 3:
//                    vectorRow = new IndexedVector3(matrix.M14, matrix.M24, matrix.M34);
//                    break;
//                default:
//                    vectorRow = IndexedVector3.Zero;
//                    break;
//            }
//        }



        public static int GetQuantized(float x)
        {
            if (x < 0.0)
            {
                return (int)(x - 0.5);
            }
            return (int)(x + 0.5);
        }

        public static void VectorClampMax(ref IndexedVector3 input, ref IndexedVector3 bounds)
        {
            input.X = Math.Min(input.X, bounds.X);
            input.Y = Math.Min(input.Y, bounds.Y);
            input.Z = Math.Min(input.Z, bounds.Z);
        }


        public static void VectorClampMin(ref IndexedVector3 input, ref IndexedVector3 bounds)
        {
            input.X = Math.Max(input.X, bounds.X);
            input.Y = Math.Max(input.Y, bounds.Y);
            input.Z = Math.Max(input.Z, bounds.Z);
        }

        public static void VectorMin(IndexedVector3 input, ref IndexedVector3 output)
        {
            VectorMin(ref input, ref output);
        }

        public static void VectorMin(ref IndexedVector3 input, ref IndexedVector3 output)
        {
            output.X = Math.Min(input.X, output.X);
            output.Y = Math.Min(input.Y, output.Y);
            output.Z = Math.Min(input.Z, output.Z);
        }

        public static void VectorMin(ref IndexedVector3 input1, ref IndexedVector3 input2, out IndexedVector3 output)
        {
            output = new IndexedVector3(
                Math.Min(input1.X, input2.X),
                Math.Min(input1.Y, input2.Y),
                Math.Min(input1.Z, input2.Z));
        }

        public static void VectorMax(IndexedVector3 input, ref IndexedVector3 output)
        {
            VectorMax(ref input, ref output);
        }

        public static void VectorMax(ref IndexedVector3 input, ref IndexedVector3 output)
        {
            output.X = Math.Max(input.X, output.X);
            output.Y = Math.Max(input.Y, output.Y);
            output.Z = Math.Max(input.Z, output.Z);
        }

        public static void VectorMax(ref IndexedVector3 input1, ref IndexedVector3 input2, out IndexedVector3 output)
        {
            output = new IndexedVector3(
                Math.Max(input1.X, input2.X),
                Math.Max(input1.Y, input2.Y),
                Math.Max(input1.Z, input2.Z));
        }

        public static float RecipSqrt(float a)
        {
            return (float)(1 / Math.Sqrt(a));
        }

        public static bool CompareFloat(float val1, float val2)
        {
            return Math.Abs(val1 - val2) <= SIMD_EPSILON;
        }

        public static bool FuzzyZero(float val)
        {
            return Math.Abs(val) <= SIMD_EPSILON;
        }

        public static uint Select(uint condition, uint valueIfConditionNonZero, uint valueIfConditionZero)
        {
            // Set testNz to 0xFFFFFFFF if condition is nonzero, 0x00000000 if condition is zero
            // Rely on positive value or'ed with its negative having sign bit on
            // and zero value or'ed with its negative (which is still zero) having sign bit off 
            // Use arithmetic shift right, shifting the sign bit through all 32 bits
            uint testNz = (uint)(((int)condition | -(int)condition) >> 31);
            uint testEqz = ~testNz;
            return ((valueIfConditionNonZero & testNz) | (valueIfConditionZero & testEqz));
        }

//        public static void BasisMatrix(IndexedMatrix matrixIn, out IndexedMatrix matrixOut)
//        {
//            BasisMatrix(ref matrixIn, out matrixOut);
//        }
//        public static void BasisMatrix(ref IndexedMatrix matrixIn, out IndexedMatrix matrixOut)
//        {
//            matrixOut = matrixIn;
//            matrixOut.M41 = 0.0f;
//            matrixOut.M42 = 0.0f;
//            matrixOut.M43 = 0.0f;
//            matrixOut.M44 = 1.0f;
//        }

//        public static IndexedMatrix BasisMatrix(IndexedMatrix matrixIn)
//        {
//            return BasisMatrix(ref matrixIn);
//        }
//        public static IndexedMatrix BasisMatrix(ref IndexedMatrix matrixIn)
//        {
//            IndexedMatrix matrixOut = matrixIn;
//            matrixOut.M41 = 0.0f;
//            matrixOut.M42 = 0.0f;
//            matrixOut.M43 = 0.0f;
//            matrixOut.M44 = 1.0f;
//            return matrixOut;
//        }

        public static Quaternion ShortestArcQuat(IndexedVector3 axisInA, IndexedVector3 axisInB)
        {
            return ShortestArcQuat(ref axisInA, ref axisInB);
        }
        public static Quaternion ShortestArcQuat(ref IndexedVector3 axisInA, ref IndexedVector3 axisInB)
        {
            IndexedVector3 c = IndexedVector3.Cross(axisInA, axisInB);
            float d = IndexedVector3.Dot(axisInA, axisInB);

            if (d < -1.0 + SIMD_EPSILON)
            {
                return new Quaternion(0.0f, 1.0f, 0.0f, 0.0f); // just pick any vector
            }

            float s = (float)Math.Sqrt((1.0f + d) * 2.0f);
            float rs = 1.0f / s;

            return new Quaternion(c.X * rs, c.Y * rs, c.Z * rs, s * 0.5f);

        }

        public static float QuatAngle(ref Quaternion quat)
        {
            return 2f * (float)Math.Acos(quat.W);
        }

        public static Quaternion QuatFurthest(ref Quaternion input1, ref Quaternion input2)
        {
            Quaternion diff, sum;
            diff = input1 - input2;
            sum = input1 + input2;
            if (Quaternion.Dot(diff, diff) > Quaternion.Dot(sum, sum))
            {
                return input2;
            }
            return (-input2);
        }

        public static IndexedVector3 QuatRotate(Quaternion rotation, IndexedVector3 v)
        {
            return QuatRotate(ref rotation, ref v);
        }

        public static IndexedVector3 QuatRotate(ref Quaternion rotation, ref IndexedVector3 v)
        {
            Quaternion q = QuatVectorMultiply(ref rotation, ref v);
            q *= QuaternionInverse(ref rotation);
            return new IndexedVector3(q.X, q.Y, q.Z);
        }


        public static Quaternion QuatVectorMultiply(ref Quaternion q, ref IndexedVector3 w)
        {
            return new Quaternion(q.W * w.X + q.Y * w.Z - q.Z * w.Y,
                                    q.W * w.Y + q.Z * w.X - q.X * w.Z,
                                    q.W * w.Z + q.X * w.Y - q.Y * w.X,
                                    -q.X * w.X - q.Y * w.Y - q.Z * w.Z);
        }

//      /**@brief diagonalizes this matrix by the Jacobi method.
//       * @param rot stores the rotation from the coordinate system in which the matrix is diagonal to the original
//       * coordinate system, i.e., old_this = rot * new_this * rot^T. 
//       * @param threshold See iteration
//       * @param iteration The iteration stops when all off-diagonal elements are less than the threshold multiplied 
//       * by the sum of the absolute values of the diagonal, or when maxSteps have been executed. 
//       * 
//       * Note that this matrix is assumed to be symmetric. 
//       */
//        public static void Diagonalize(ref IndexedMatrix inMatrix,ref IndexedMatrix rot, float threshold, int maxSteps)
//        {
//            Debug.Assert(false);
//            rot = IndexedMatrix.Identity;
//            for (int step = maxSteps; step > 0; step--)
//            {
//                // find off-diagonal element [p][q] with largest magnitude
//                int p = 0;
//                int q = 1;
//                int r = 2;
//                float max = Math.Abs(inMatrix.M12);
//                float v = Math.Abs(inMatrix.M13);
//                if (v > max)
//                {
//                   q = 2;
//                   r = 1;
//                   max = v;
//                }
//                v = Math.Abs(inMatrix.M23);
//                if (v > max)
//                {
//                   p = 1;
//                   q = 2;
//                   r = 0;
//                   max = v;
//                }

//                float t = threshold * (Math.Abs(inMatrix.M11) + Math.Abs(inMatrix.M22) + Math.Abs(inMatrix.M33));
//                if (max <= t)
//                {
//                   if (max <= SIMD_EPSILON * t)
//                   {
//                      return;
//                   }
//                   step = 1;
//                }

//                // compute Jacobi rotation J which leads to a zero for element [p][q] 
//                float mpq = MathUtil.MatrixComponent(ref inMatrix,p,q);
//                float theta = (MathUtil.MatrixComponent(ref inMatrix,q,q)-MathUtil.MatrixComponent(ref inMatrix,p,p)) / (2 * mpq);
//                float theta2 = theta * theta;
//                float cos;
//                float sin;
//                if (theta2 * theta2 < 10f / SIMD_EPSILON)
//                {
//                   t = (theta >= 0f) ? (float)(1f / (theta + Math.Sqrt(1 + theta2)))
//                                            : (float)(1f / (theta - Math.Sqrt(1 + theta2)));
//                   cos = (float)(1f / Math.Sqrt(1 + t * t));
//                   sin = cos * t;
//                }
//                else
//                {
//                   // approximation for large theta-value, i.e., a nearly diagonal matrix
//                   t = 1 / (theta * (2 + 0.5f / theta2));
//                   cos = 1 - 0.5f * t * t;
//                   sin = cos * t;
//                }

//                // apply rotation to matrix (this = J^T * this * J)
//                MathUtil.MatrixComponent(ref inMatrix,p,q,0f);
//                MathUtil.MatrixComponent(ref inMatrix,q,p,0f);
//                MathUtil.MatrixComponent(ref inMatrix,p,p,MathUtil.MatrixComponent(ref inMatrix,p,p)-t*mpq);
//                MathUtil.MatrixComponent(ref inMatrix,q,q,MathUtil.MatrixComponent(ref inMatrix,q,q)+t*mpq);

//                float  mrp = MathUtil.MatrixComponent(ref inMatrix,r,p);
//                float  mrq = MathUtil.MatrixComponent(ref inMatrix,r,q);

//                MathUtil.MatrixComponent(ref inMatrix,r,p,cos * mrp - sin * mrq);
//                MathUtil.MatrixComponent(ref inMatrix,p,r,cos * mrp - sin * mrq);

//                MathUtil.MatrixComponent(ref inMatrix,r,q,cos * mrp + sin * mrq);
//                MathUtil.MatrixComponent(ref inMatrix,q,r,cos * mrp + sin * mrq);

//                // apply rotation to rot (rot = rot * J)
//                for (int i = 0; i < 3; i++)
//                {
//                    float  mrp2 = MathUtil.MatrixComponent(ref rot,i,p);
//                    float  mrq2 = MathUtil.MatrixComponent(ref rot,i,q);
//                    MathUtil.MatrixComponent(ref rot, i, p, cos * mrp - sin * mrq);
//                    MathUtil.MatrixComponent(ref rot, i, q, cos * mrp + sin * mrq);
//                }
//            }
//        }



        public static void GetSkewSymmetricMatrix(ref IndexedVector3 vecin, out IndexedVector3 v0, out IndexedVector3 v1, out IndexedVector3 v2)
        {
            v0 = new IndexedVector3(0f, -vecin.Z, vecin.Y);
            v1 = new IndexedVector3(vecin.Z, 0f, -vecin.X);
            v2 = new IndexedVector3(-vecin.Y, vecin.X, 0f);
        }
        public static void SanityCheckVector(IndexedVector3 v)
        {
#if DEBUG
            SanityCheckVector(ref v);
#endif
        }
        public static void ZeroCheckVector(IndexedVector3 v)
        {
#if DEBUG

            ZeroCheckVector(ref v);
#endif
        }

        public static void ZeroCheckVector(ref IndexedVector3 v)
        {
#if DEBUG

            if (FuzzyZero(v.LengthSquared()))
            {
                //Debug.Assert(false);
            }
#endif
        }
        public static void SanityCheckVector(ref IndexedVector3 v)
        {
#if DEBUG

            if (float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z))
            {
                Debug.Assert(false);
            }
#endif
        }

        public static void SanityCheckFloat(float f)
        {
#if DEBUG

            Debug.Assert(!float.IsInfinity(f) && !float.IsNaN(f));
#endif

        }

//        public static void Vector3FromFloat(out IndexedVector3 v, float[] fa)
//        {
//            v = new IndexedVector3(fa[0], fa[1], fa[2]);
//        }

//        //public static void FloatFromVector3(IndexedVector3 v, float[] fa)
//        //{
//        //    FloatFromVector3(ref v, fa);
//        //}

//        //public static void FloatFromVector3(ref IndexedVector3 v, float[] fa)
//        //{
//        //    fa[0] = v.X;
//        //    fa[1] = v.Y;
//        //    fa[2] = v.Z;
//        //}

//        //public static float[] FloatFromVector3(IndexedVector3 v)
//        //{
//        //    return FloatFromVector3(ref v);
//        //}

//        //public static float[] FloatFromVector3(ref IndexedVector3 v)
//        //{
//        //    return new float[] { v.X, v.Y, v.Z };
//        //}

        public static float GetMatrixElem(IndexedBasisMatrix mat, int index)
        {
	        int i = index%3;
	        int j = index/3;
	        return mat[i,j];
        }

        public static float GetMatrixElem(ref IndexedBasisMatrix mat, int index)
        {
            int i = index % 3;
            int j = index / 3;
            return mat[i, j];
        }

        public static bool MatrixToEulerXYZ(ref IndexedBasisMatrix mat, out IndexedVector3 xyz)
        {
            //	// rot =  cy*cz          -cy*sz           sy
            //	//        cz*sx*sy+cx*sz  cx*cz-sx*sy*sz -cy*sx
            //	//       -cx*cz*sy+sx*sz  cz*sx+cx*sy*sz  cx*cy
            //

            float matElem0 = MathUtil.GetMatrixElem(ref mat, 0);
            float matElem1 = MathUtil.GetMatrixElem(ref mat, 1);
            float matElem2 = MathUtil.GetMatrixElem(ref mat, 2);
            float matElem3 = MathUtil.GetMatrixElem(ref mat, 3);
            float matElem4 = MathUtil.GetMatrixElem(ref mat, 4);
            float matElem5 = MathUtil.GetMatrixElem(ref mat, 5);
            float matElem6 = MathUtil.GetMatrixElem(ref mat, 6);
            float matElem7 = MathUtil.GetMatrixElem(ref mat, 7);
            float matElem8 = MathUtil.GetMatrixElem(ref mat, 8);

            float fi = matElem2;
            if (fi < 1.0f)
            {
                if (fi > -1.0f)
                {
                    xyz = new IndexedVector3(
                        (float)Math.Atan2(-matElem5, matElem8),
                        (float)Math.Asin(matElem2),
                        (float)Math.Atan2(-matElem1, matElem0));
                    return true;
                }
                else
                {
                    // WARNING.  Not unique.  XA - ZA = -atan2(r10,r11)
                    xyz = new IndexedVector3(
                        (float)-Math.Atan2(matElem3, matElem4),
                        -SIMD_HALF_PI,
                        0f);
                    return false;
                }
            }
            else
            {
                // WARNING.  Not unique.  XAngle + ZAngle = atan2(r10,r11)
                xyz = new IndexedVector3(
                    (float)Math.Atan2(matElem3, matElem4),
                    SIMD_HALF_PI,
                    0.0f);
            }
            return false;
        }



//        public static IndexedVector3 MatrixToEuler(ref IndexedMatrix m)
//        {
//            IndexedVector3 translate;
//            IndexedVector3 scale;
//            Quaternion rotate;
//            m.Decompose(out scale, out rotate, out translate);
//            return quaternionToEuler(ref rotate);
//        }

//        // Taken from Fabian Vikings post at : http://forums.xna.com/forums/p/4574/23763.aspx  
//        public static IndexedVector3 quaternionToEuler(ref Quaternion q)
//        {
//            IndexedVector3 v = IndexedVector3.Zero;

//            v.X = (float)Math.Atan2
//            (
//                2 * q.Y * q.W - 2 * q.X * q.Z,
//                   1 - 2 * Math.Pow(q.Y, 2) - 2 * Math.Pow(q.Z, 2)
//            );

//            v.Z = (float)Math.Asin
//            (
//                2 * q.X * q.Y + 2 * q.Z * q.W
//            );

//            v.Y = (float)Math.Atan2
//            (
//                2 * q.X * q.W - 2 * q.Y * q.Z,
//                1 - 2 * Math.Pow(q.X, 2) - 2 * Math.Pow(q.Z, 2)
//            );

//            if (q.X * q.Y + q.Z * q.W == 0.5)
//            {
//                v.X = (float)(2 * Math.Atan2(q.X, q.W));
//                v.Y = 0;
//            }

//            else if (q.X * q.Y + q.Z * q.W == -0.5)
//            {
//                v.X = (float)(-2 * Math.Atan2(q.X, q.W));
//                v.Y = 0;
//            }

//            return v;
//        }

        public static Quaternion QuaternionInverse(Quaternion q)
        {
            return QuaternionInverse(ref q);
        }

        public static Quaternion QuaternionInverse(ref Quaternion q)
        {
            return new Quaternion(-q.X, -q.Y, -q.Z, q.W);
        }

        public static Quaternion QuaternionMultiply(Quaternion a, Quaternion b)
        {
            return QuaternionMultiply(ref a, ref b);
        }

        public static Quaternion QuaternionMultiply(ref Quaternion a, ref Quaternion b)
        {
            return a * b;
            //return b * a;
        }

        //public static IndexedMatrix BulletMatrixMultiply(IndexedMatrix m1, IndexedMatrix m2)
        //{
        //    return m1 * m2;
        //}

        //public static IndexedMatrix BulletMatrixMultiply(ref IndexedMatrix m1, ref IndexedMatrix m2)
        //{
        //    return m1 * m2;
        //}

//        public static IndexedMatrix BulletMatrixMultiplyBasis(IndexedMatrix m1, IndexedMatrix m2)
//        {
//            return BulletMatrixMultiplyBasis(ref m1, ref m2);
//        }

//        public static IndexedMatrix BulletMatrixMultiplyBasis(ref IndexedMatrix m1, ref IndexedMatrix m2)
//        {
//            IndexedMatrix mb1;
//            BasisMatrix(ref m1, out mb1);
//            IndexedMatrix mb2;
//            BasisMatrix(ref m2, out mb2);
//            return BulletMatrixMultiply(ref mb1, ref mb2);
//        }



        public static float NormalizeAngle(float angleInRadians)
        {
            // Need to check this mod operator works with floats...
            angleInRadians = angleInRadians % SIMD_2_PI;
            if (angleInRadians < -SIMD_PI)
            {
                return angleInRadians + SIMD_2_PI;
            }
            else if (angleInRadians > SIMD_PI)
            {
                return angleInRadians - SIMD_2_PI;
            }
            else
            {
                return angleInRadians;
            }
        }

        public static float DegToRadians(float degrees)
        {
            return (degrees / 360.0f) * SIMD_2_PI;
        }


        public static IndexedVector3 Interpolate3(IndexedVector3 v0, IndexedVector3 v1, float rt)
        {
            float s = 1.0f - rt;
            return new IndexedVector3(
                s * v0.X + rt * v1.X,
                s * v0.Y + rt * v1.Y,
                s * v0.Z + rt * v1.Z);
        }


        public static IndexedVector3 Interpolate3(ref IndexedVector3 v0, ref IndexedVector3 v1, float rt)
        {
            float s = 1.0f - rt;
            return new IndexedVector3(
                s * v0.X + rt * v1.X,
                s * v0.Y + rt * v1.Y,
                s * v0.Z + rt * v1.Z);
        }

        public static IndexedMatrix SetEulerZYX(float eulerX, float eulerY, float eulerZ)
        {
            //return IndexedMatrix.CreateFromYawPitchRoll(y, x,z);
            // This version tested and compared to c++ version. don't break it.
            // note that the row/column settings are switched from c++
            IndexedMatrix m = IndexedMatrix.Identity;
            m._basis.SetEulerZYX(eulerX, eulerY, eulerZ);
            return m;

        }

//        public static IndexedVector3 MatrixToVector(IndexedMatrix m, IndexedVector3 v)
//        {
//            return new IndexedVector3(
//                IndexedVector3.Dot(new IndexedVector3(m.M11, m.M12, m.M13), v) + m._origin.X,
//                IndexedVector3.Dot(new IndexedVector3(m.M21, m.M22, m.M23), v) + m._origin.Y,
//                IndexedVector3.Dot(new IndexedVector3(m.M31, m.M32, m.M33), v) + m._origin.Z
//                );
//        }


        public static IndexedVector3 Vector4ToVector3(Vector4 v4)
        {
            return new IndexedVector3(v4.X, v4.Y, v4.Z);
        }

        public static IndexedVector3 Vector4ToVector3(ref Vector4 v4)
        {
            return new IndexedVector3(v4.X, v4.Y, v4.Z);
        }


//        public static IndexedVector3 TransposeTransformNormal(IndexedVector3 v,IndexedMatrix m)
//        {
//            return TransposeTransformNormal(ref v, ref m);
//        }

//        public static IndexedVector3 TransposeTransformNormal(ref IndexedVector3 v,ref IndexedMatrix m)
//        {
//            IndexedMatrix mt = TransposeBasis(ref m);
//            return IndexedVector3.TransformNormal(v, mt);
//        }

//        //public static IndexedVector3 TransposeTransformNormal(ref IndexedVector3 v, ref IndexedMatrix m)
//        //{
//        //    IndexedMatrix mt = TransposeBasis(ref m);
//        //    return IndexedVector3.TransformNormal(ref v, ref mt);
//        //}



        public static void PrintQuaternion(TextWriter writer, Quaternion q)
        {
            writer.Write(String.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000} W:{3:0.00000000}}}", q.X, q.Y, q.Z, q.W));
        }

        public static void PrintVector3(TextWriter writer, IndexedVector3 v)
        {
            writer.WriteLine(String.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000}}}", v.X, v.Y, v.Z));
        }

        public static void PrintVector3(TextWriter writer, String name, IndexedVector3 v)
        {
            writer.WriteLine(String.Format("[{0}] {{X:{1:0.00000000} Y:{2:0.00000000} Z:{3:0.00000000}}}", name, v.X, v.Y, v.Z));
        }

        public static void PrintVector4(TextWriter writer, Vector4 v)
        {
            writer.WriteLine(String.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000} W:{3:0.00000000}}}", v.X, v.Y, v.Z, v.W));
        }

        public static void PrintVector4(TextWriter writer, String name, Vector4 v)
        {
            writer.WriteLine(String.Format("[{0}] {{X:{1:0.00000000} Y:{2:0.00000000} Z:{3:0.00000000} W:{4:0.00000000}}}", name, v.X, v.Y, v.Z, v.W));
        }


        public static void PrintMatrix(TextWriter writer, IndexedMatrix m)
        {
            PrintMatrix(writer, null, m);
        }

        public static void PrintMatrix(TextWriter writer, String name, IndexedMatrix m)
        {
            if (writer != null)
            {
                if (name != null)
                {
                    writer.WriteLine(name);
                }
                PrintVector3(writer, "Right       ", m._basis.GetColumn(0));
                PrintVector3(writer, "Up          ", m._basis.GetColumn(1));
                PrintVector3(writer, "Backward    ", m._basis.GetColumn(2));
                PrintVector3(writer, "Translation ", m._origin);
            }
        }

        public static void PrintMatrix(TextWriter writer, IndexedBasisMatrix m)
        {
            PrintMatrix(writer, null, m);
        }

        public static void PrintMatrix(TextWriter writer, String name, IndexedBasisMatrix m)
        {
            if (writer != null)
            {
                if (name != null)
                {
                    writer.WriteLine(name);
                }
                PrintVector3(writer, "Right       ", m.GetColumn(0));
                PrintVector3(writer, "Up          ", m.GetColumn(1));
                PrintVector3(writer, "Backward    ", m.GetColumn(2));
                PrintVector3(writer, "Translation ", IndexedVector3.Zero);
            }
        }
                
        //public static void PrintContactPoint(StreamWriter streamWriter, ManifoldPoint mp)
        //{
        //    if (streamWriter != null)
        //    {
        //        streamWriter.WriteLine("ContactPoint");
        //        PrintVector3(streamWriter, "localPointA", mp.m_localPointA);
        //        PrintVector3(streamWriter, "localPointB", mp.m_localPointB);
        //        PrintVector3(streamWriter, "posWorldA", mp.m_positionWorldOnA);
        //        PrintVector3(streamWriter, "posWorldB", mp.m_positionWorldOnB);
        //        PrintVector3(streamWriter, "normalWorldB", mp.m_normalWorldOnB);
        //    }
        //}

        public static bool IsAlmostZero(Vector3 v)
        {
            if (Math.Abs(v.X) > 1e-6 || Math.Abs(v.Y) > 1e-6 || Math.Abs(v.Z) > 1e-6) return false;
            return true;

        }

        public static bool IsAlmostZero(ref Vector3 v)
        {
            if (Math.Abs(v.X) > 1e-6 || Math.Abs(v.Y) > 1e-6 || Math.Abs(v.Z) > 1e-6) return false;
            return true;
        }

        public static bool IsAlmostZero(IndexedVector3 v)
        {
            if (Math.Abs(v.X) > 1e-6 || Math.Abs(v.Y) > 1e-6 || Math.Abs(v.Z) > 1e-6) return false;
            return true;

        }

        public static bool IsAlmostZero(ref IndexedVector3 v)
        {
            if (Math.Abs(v.X) > 1e-6 || Math.Abs(v.Y) > 1e-6 || Math.Abs(v.Z) > 1e-6) return false;
            return true;
        }


        public static IndexedVector3 Vector3Lerp(ref IndexedVector3 a, ref IndexedVector3 b, float t)
        {
            return new IndexedVector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t);
        }


        public static float Vector3Distance2XZ(IndexedVector3 x, IndexedVector3 y)
        {
            IndexedVector3 xa = new IndexedVector3(x.X, 0, x.Z);
            IndexedVector3 ya = new IndexedVector3(y.X, 0, y.Z);
            return (xa - ya).LengthSquared();
        }

        public static void PrintMatrix(TextWriter writer, Matrix m)
        {
            PrintMatrix(writer, null, m);
        }

        public static void PrintMatrix(TextWriter writer, String name, Matrix m)
        {
            if (writer != null)
            {
                if (name != null)
                {
                    writer.WriteLine(name);
                }
                PrintVector3(writer, "Right       ", m.Right);
                PrintVector3(writer, "Up          ", m.Up);
                PrintVector3(writer, "Backward    ", m.Backward);
                PrintVector3(writer, "Translation ", m.Translation);
            }
        }

        public static void PrintVector3(TextWriter writer, Vector3 v)
        {
            writer.WriteLine(String.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000}}}", v.X, v.Y, v.Z));
        }

        public static void PrintVector3(TextWriter writer, String name, Vector3 v)
        {
            writer.WriteLine(String.Format("[{0}] {{X:{1:0.00000000} Y:{2:0.00000000} Z:{3:0.00000000}}}", name, v.X, v.Y, v.Z));
        }


        //public const float SIMD_EPSILON = 0.0000001f;
        public const float SIMD_EPSILON = 1.192092896e-07f;
        public const float SIMDSQRT12 = 0.7071067811865475244008443621048490f;

        public const float BT_LARGE_FLOAT = 1e18f;
        public static IndexedVector3 MAX_VECTOR = new IndexedVector3(BT_LARGE_FLOAT);
        public static IndexedVector3 MIN_VECTOR = new IndexedVector3(-BT_LARGE_FLOAT);
        public const float SIMD_2_PI = 6.283185307179586232f;
        public const float SIMD_PI = SIMD_2_PI * 0.5f;
        public const float SIMD_HALF_PI = SIMD_PI * 0.5f;
        public const float SIMD_QUARTER_PI = SIMD_PI * 0.25f;

        public const float SIMD_INFINITY = float.MaxValue;
        public const float SIMD_RADS_PER_DEG = (SIMD_2_PI / 360.0f);
        public const float SIMD_DEGS_PER_RAD = (360.0f / SIMD_2_PI);

        
        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public int i;
            [FieldOffset(0)]
            public float f;
        }

        //  Returns the next floating-point number after x in the direction of y.
        public static float NextAfter(float x, float y)
        {

            if (float.IsNaN(x) || float.IsNaN(y)) return x + y;
            if (x == y) return y;  // nextafter (0.0, -0.0) should return -0.0

            FloatIntUnion u;
            u.i = 0; u.f = x; // shut up the compiler

            if (x == 0)
            {
                u.i = 1;
                return y > 0 ? u.f : -u.f;
            }

            if ((x > 0) == (y > x))
                u.i++;
            else
                u.i--;
            return u.f;
        }
    }
}

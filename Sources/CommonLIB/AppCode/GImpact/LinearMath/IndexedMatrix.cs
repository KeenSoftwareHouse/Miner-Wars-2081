using System;
using MinerWarsMath;

namespace BulletXNA.LinearMath
{
    public struct IndexedMatrix : IEquatable<IndexedMatrix>
    {
        private static IndexedMatrix _identity = new IndexedMatrix(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f,  0.0f, 0.0f, 1f,  0.0f, 0.0f, 0.0f);
        public static IndexedMatrix Identity
        {
            get
            {
                return IndexedMatrix._identity;
            }
        }

        public Matrix ToMatrix()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Right = _basis.GetColumn(0).ToVector3();
            matrix.Up = _basis.GetColumn(1).ToVector3();
            matrix.Backward = _basis.GetColumn(2).ToVector3();
            //matrix.Right = _basis.GetRow(0).ToVector3();
            //matrix.Up = _basis.GetRow(1).ToVector3();
            //matrix.Backward = _basis.GetRow(2).ToVector3();

            matrix.Translation = _origin.ToVector3();
            return matrix;
        }

        // User-defined conversion from IndexedVector3 to Vector3
        public static implicit operator Matrix(IndexedMatrix im)
        {
            Matrix matrix = Matrix.Identity;
            matrix.Right = im._basis.GetColumn(0).ToVector3();
            matrix.Up = im._basis.GetColumn(1).ToVector3();
            matrix.Backward = im._basis.GetColumn(2).ToVector3();
            matrix.Translation = im._origin.ToVector3();
            return matrix;
        }

        // User-defined conversion from IndexedVector3 to Vector3
        public static implicit operator IndexedMatrix(Matrix m)
        {
            IndexedMatrix im = new IndexedMatrix();
            im._origin = new IndexedVector3(m.Translation);
            //_basis = new IndexedBasisMatrix(new IndexedVector3(m.Right), new IndexedVector3(m.Up), new IndexedVector3(m.Backward)).Transpose();
            im._basis = new IndexedBasisMatrix(new IndexedVector3(m.Right), new IndexedVector3(m.Up), new IndexedVector3(m.Backward));
            return im;
        }



        public Matrix ToMatrixProjection()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Right = _basis.GetColumn(0).ToVector3();
            matrix.Up = _basis.GetColumn(1).ToVector3();
            matrix.Backward = _basis.GetColumn(2).ToVector3();
            //matrix.Right = _basis.GetRow(0).ToVector3();
            //matrix.Up = _basis.GetRow(1).ToVector3();
            //matrix.Backward = _basis.GetRow(2).ToVector3();

            matrix.Translation = _origin.ToVector3();
            matrix.M34 = -1;
            matrix.M44 = 0;
            return matrix;
        }


        static IndexedMatrix()
        {
        }

        public IndexedMatrix(float m11, float m12, float m13, float m21, float m22, float m23,  float m31, float m32, float m33,  float m41, float m42, float m43)
        {
            _basis = new IndexedBasisMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33);
            _origin = new IndexedVector3(m41, m42, m43);

        }

        public IndexedMatrix(IndexedBasisMatrix basis, IndexedVector3 origin)
        {
            _basis = basis;
            _origin = origin;
        }

        public IndexedMatrix(Matrix m)
        {
            _origin = new IndexedVector3(m.Translation);
            //_basis = new IndexedBasisMatrix(new IndexedVector3(m.Right), new IndexedVector3(m.Up), new IndexedVector3(m.Backward)).Transpose();
            _basis = new IndexedBasisMatrix(new IndexedVector3(m.Right), new IndexedVector3(m.Up), new IndexedVector3(m.Backward));
        }

        public static IndexedMatrix CreateLookAt(IndexedVector3 cameraPosition, IndexedVector3 cameraTarget, IndexedVector3 cameraUpVector)
        {
            IndexedVector3 vector3_1 = IndexedVector3.Normalize(cameraPosition - cameraTarget);
            IndexedVector3 vector3_2 = IndexedVector3.Normalize(IndexedVector3.Cross(cameraUpVector, vector3_1));
            IndexedVector3 vector1 = IndexedVector3.Cross(vector3_1, vector3_2);
            IndexedMatrix matrix = IndexedMatrix.Identity;
            //matrix._basis = new IndexedBasisMatrix(vector3_2.X, vector1.X, vector3_1.X, vector3_2.Y, vector1.Y, vector3_1.Y, vector3_2.Z, vector1.Z, vector3_1.Z).Transpose();
            matrix._basis = new IndexedBasisMatrix(ref vector3_2, ref vector1, ref vector3_1);

            matrix._origin = new IndexedVector3(-IndexedVector3.Dot(vector3_2, cameraPosition),
            -IndexedVector3.Dot(vector1, cameraPosition),
            -IndexedVector3.Dot(vector3_1, cameraPosition));
            return matrix;
        }

        public static IndexedMatrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
        //    if ((double)fieldOfView <= 0.0 || (double)fieldOfView >= 3.14159274101257)
        //        throw new ArgumentOutOfRangeException("fieldOfView", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.OutRangeFieldOfView, new object[1]
        //{
        //  (object) "fieldOfView"
        //}));
        //    else if ((double)nearPlaneDistance <= 0.0)
        //        throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[1]
        //{
        //  (object) "nearPlaneDistance"
        //}));
        //    else if ((double)farPlaneDistance <= 0.0)
        //    {
        //        throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[1]
        //{
        //  (object) "farPlaneDistance"
        //}));
        //    }
        //    else
            {
                //if ((double)nearPlaneDistance >= (double)farPlaneDistance)
                //    throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
                float num1 = 1f / (float)Math.Tan((double)fieldOfView * 0.5);
                float num2 = num1 / aspectRatio;
                IndexedMatrix matrix = IndexedMatrix.Identity ;
                matrix._basis = new IndexedBasisMatrix(num2, 0, 0, 0, num1, 0, 0, 0, farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
                matrix._origin = new IndexedVector3(0,0,(float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance)));
                
                return matrix;
            }
        }


        public static bool operator ==(IndexedMatrix matrix1, IndexedMatrix matrix2)
        {
            return matrix1._basis == matrix2._basis &&
                    matrix1._origin == matrix2._origin; 
        }

        public static bool operator !=(IndexedMatrix matrix1, IndexedMatrix matrix2)
        {
            return matrix1._basis != matrix2._basis ||
                    matrix1._origin != matrix2._origin;
        }

	    public static IndexedVector3 operator *(IndexedMatrix matrix1,IndexedVector3 v)
	    {
            //return new IndexedVector3(matrix1._basis[0].Dot(ref v) + matrix1._origin.X, 
            //                           matrix1._basis[1].Dot(ref v) + matrix1._origin.Y,
            //                            matrix1._basis[2].Dot(ref v) + matrix1._origin.Z);
            return new IndexedVector3(matrix1._basis._Row0.Dot(ref v) + matrix1._origin.X,
                                                   matrix1._basis._Row1.Dot(ref v) + matrix1._origin.Y,
                                                    matrix1._basis._Row2.Dot(ref v) + matrix1._origin.Z);
        }

        //public static IndexedVector3 operator *(IndexedVector3 v,IndexedMatrix matrix1)
        //{
        //    return new IndexedVector3(matrix1._basis[0].Dot(ref v) + matrix1._origin.X,
        //                               matrix1._basis[1].Dot(ref v) + matrix1._origin.Y,
        //                                matrix1._basis[2].Dot(ref v) + matrix1._origin.Z);
        //}

        public static IndexedMatrix operator *(IndexedMatrix matrix1, IndexedMatrix matrix2)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis = matrix1._basis * matrix2._basis;
            IndexedMatrix._origin = matrix1 * matrix2._origin;

            return IndexedMatrix;
        }

        //public static IndexedMatrix operator *(IndexedMatrix matrix1, float scaleFactor)
        //{
        //    float num = scaleFactor;
        //    IndexedMatrix IndexedMatrix;
        //    IndexedMatrix._basis._Row0 = matrix1._Row0  * scaleFactor;
        //    IndexedMatrix._basis._Row1 = matrix1._Row1 * scaleFactor;
        //    IndexedMatrix._basis._Row3 = matrix1._Row3 * scaleFactor;
        //    IndexedMatrix._basis.Row3 = matrix1.Row3 * scaleFactor;
        //    return IndexedMatrix;
        //}

        //public static IndexedMatrix operator *(float scaleFactor, IndexedMatrix matrix1)
        //{
        //    IndexedMatrix IndexedMatrix;
        //    IndexedMatrix._basis._Row0 = matrix1._basis._Row0 * scaleFactor;
        //    IndexedMatrix._basis._Row1 = matrix1._basis._Row1 * scaleFactor;
        //    IndexedMatrix._basis._Row2 = matrix1._basis._Row2 * scaleFactor;
        //    IndexedMatrix._origin = matrix1._origin* scaleFactor;
        //    return IndexedMatrix;
        //}

        public static IndexedMatrix operator /(IndexedMatrix matrix1, IndexedMatrix matrix2)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = matrix1._basis._Row0 / matrix2._basis._Row0;
            IndexedMatrix._basis._Row1 = matrix1._basis._Row1 / matrix2._basis._Row1;
            IndexedMatrix._basis._Row2 = matrix1._basis._Row2 / matrix2._basis._Row2;
            IndexedMatrix._origin = matrix1._origin / matrix2._origin;
            return IndexedMatrix;
        }

        public static IndexedMatrix operator /(IndexedMatrix matrix1, float divider)
        {
            float num = 1f / divider;
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = matrix1._basis._Row0 * num;
            IndexedMatrix._basis._Row1 = matrix1._basis._Row1 * num;
            IndexedMatrix._basis._Row2 = matrix1._basis._Row2 * num;
            IndexedMatrix._origin = matrix1._origin * num;
            return IndexedMatrix;
        }


        public static IndexedMatrix CreateTranslation(IndexedVector3 position)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(1, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, 1, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, 0, 1);
            IndexedMatrix._origin = position;            
            return IndexedMatrix;
        }

        public static void CreateTranslation(ref IndexedVector3 position, out IndexedMatrix result)
        {
            result._basis._Row0 = new IndexedVector3(1, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, 1, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, 1);
            result._origin =  position;
        }

        public static IndexedMatrix CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(1, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, 1, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, 0, 1);
            IndexedMatrix._origin = new IndexedVector3(xPosition, yPosition, zPosition);
            return IndexedMatrix;
        }

        public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out IndexedMatrix result)
        {
            result._basis._Row0 = new IndexedVector3(1, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, 1, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, 1);
            result._origin = new IndexedVector3(xPosition, yPosition, zPosition);
        }

        public static IndexedMatrix CreateScale(float xScale, float yScale, float zScale)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(xScale, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, yScale, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, 0, zScale);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            return IndexedMatrix;
        }

        public static void CreateScale(float xScale, float yScale, float zScale, out IndexedMatrix result)
        {
            result._basis._Row0 = new IndexedVector3(xScale, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, yScale, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, zScale);
            result._origin = new IndexedVector3(0,0,0);
        }

        public static IndexedMatrix CreateScale(IndexedVector3 scales)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(scales.X, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, scales.Y, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, 0, scales.Z);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            return IndexedMatrix;
        }

        public static void CreateScale(ref IndexedVector3 scales, out IndexedMatrix result)
        {
            result._basis._Row0 = new IndexedVector3(scales.X, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, scales.Y, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, scales.Z);
            result._origin = new IndexedVector3(0, 0, 0);
        }

        public static IndexedMatrix CreateScale(float scale)
        {
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(scale, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, scale, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, 0, scale);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            return IndexedMatrix;
        }

        public static void CreateScale(float scale, out IndexedMatrix result)
        {
            result._basis._Row0 = new IndexedVector3(scale, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, scale, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, scale);
            result._origin = new IndexedVector3(0, 0, 0);
        }

        public static IndexedMatrix CreateRotationX(float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(1, 0, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, num1, num2);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0, -num2, num1);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            
            return IndexedMatrix;
        }

        public static void CreateRotationX(float radians, out IndexedMatrix result)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result._basis._Row0 = new IndexedVector3(1, 0, 0);
            result._basis._Row1 = new IndexedVector3(0, num1, num2);
            result._basis._Row2 = new IndexedVector3(0, -num2, num1);
            result._origin = new IndexedVector3(0, 0, 0);
        }

        public static IndexedMatrix CreateRotationY(float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(num1, 0, -num2);
            IndexedMatrix._basis._Row1 = new IndexedVector3(0, 1, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(num2, -0, num1);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            return IndexedMatrix;
        }

        public static void CreateRotationY(float radians, out IndexedMatrix result)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result._basis._Row0 = new IndexedVector3(num1, 0, -num2);
            result._basis._Row1 = new IndexedVector3(0, 1, 0);
            result._basis._Row2 = new IndexedVector3(num2, -0, num1);
            result._origin = new IndexedVector3(0, 0, 0);
        }

        public static IndexedMatrix CreateRotationZ(float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            IndexedMatrix IndexedMatrix;
            IndexedMatrix._basis._Row0 = new IndexedVector3(num1, num2, 0);
            IndexedMatrix._basis._Row1 = new IndexedVector3(-num2, num1, 0);
            IndexedMatrix._basis._Row2 = new IndexedVector3(0,0,1);
            IndexedMatrix._origin = new IndexedVector3(0, 0, 0);
            return IndexedMatrix;
        }

        public static void CreateRotationZ(float radians, out IndexedMatrix result)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result._basis._Row0 = new IndexedVector3(num1, num2, 0);
            result._basis._Row1 = new IndexedVector3(-num2, num1, 0);
            result._basis._Row2 = new IndexedVector3(0, 0, 1);
            result._origin = new IndexedVector3(0, 0, 0);
        }

        public bool Equals(IndexedMatrix other)
        {
            return _basis.Equals(other._basis) && _origin.Equals(other._origin);
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is IndexedMatrix)
                flag = this.Equals((IndexedMatrix)obj);
            return flag;
        }

        public override int GetHashCode()
        {
            return this._basis.GetHashCode() + this._origin.GetHashCode();
        }

        public IndexedMatrix Inverse()
	    { 
		    IndexedBasisMatrix inv = _basis.Transpose();
            return new IndexedMatrix(inv, inv * -_origin);
	    }

        public IndexedVector3 InvXform(IndexedVector3 inVec)
        {
            IndexedVector3 v = inVec - _origin;
            return (_basis.Transpose() * v);
        }

        public IndexedVector3 InvXform(ref IndexedVector3 inVec)
        {
	        IndexedVector3 v = inVec - _origin;
	        return (_basis.Transpose() * v);
        }

        public IndexedMatrix InverseTimes(ref IndexedMatrix t)
        {
            IndexedVector3 v = t._origin - _origin;
            return new IndexedMatrix(_basis.TransposeTimes(t._basis),
			        v * _basis);
        }

	    public Quaternion GetRotation() 
        { 
		    return _basis.GetRotation();
	    }

        public static IndexedMatrix CreateFromQuaternion(Quaternion q)
        {
            IndexedMatrix i = new IndexedMatrix();
            i._basis.SetRotation(ref q);
            return i;
        }

        public static IndexedMatrix CreateFromQuaternion(ref Quaternion q)
        {
            IndexedMatrix i = new IndexedMatrix();
            i._basis.SetRotation(ref q);
            return i;
        }



        public IndexedBasisMatrix _basis;
        public IndexedVector3 _origin;
    
    }
}

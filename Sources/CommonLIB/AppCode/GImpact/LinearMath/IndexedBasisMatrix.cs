
using System.Diagnostics;
using System;
using MinerWarsMath;
namespace BulletXNA.LinearMath
{
    public struct IndexedBasisMatrix
    {
        private static IndexedBasisMatrix _identity = new IndexedBasisMatrix(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f);
        public static IndexedBasisMatrix Identity
        {
            get
            {
                return IndexedBasisMatrix._identity;
            }
        }

        public IndexedBasisMatrix Scaled(IndexedVector3 s)
        {

            return new IndexedBasisMatrix(_Row0.X * s.X, _Row0.Y * s.Y, _Row0.Z * s.Z,
                                        _Row1.X * s.X, _Row1.Y * s.Y, _Row1.Z * s.Z,
                                        _Row2.X * s.X, _Row2.Y * s.Y, _Row2.Z * s.Z);

        }

        public IndexedBasisMatrix Scaled(ref IndexedVector3 s)
        {

            return new IndexedBasisMatrix(_Row0.X * s.X, _Row0.Y * s.Y, _Row0.Z * s.Z,
                                        _Row1.X * s.X, _Row1.Y * s.Y, _Row1.Z * s.Z,
                                        _Row2.X * s.X, _Row2.Y * s.Y, _Row2.Z * s.Z);

        }


        public IndexedBasisMatrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            _Row0 = new IndexedVector3(m11, m12, m13);
            _Row1 = new IndexedVector3(m21, m22, m23);
            _Row2 = new IndexedVector3(m31, m32, m33);
        }

        public IndexedBasisMatrix(IndexedVector3 row0, IndexedVector3 row1, IndexedVector3 row2)
        {
            _Row0 = row0;
            _Row1 = row1;
            _Row2 = row2;
        }

        public IndexedBasisMatrix(ref IndexedVector3 row0, ref IndexedVector3 row1, ref IndexedVector3 row2)
        {
            _Row0 = row0;
            _Row1 = row1;
            _Row2 = row2;
        }

        public IndexedBasisMatrix(Quaternion q)
        {
            float d = q.LengthSquared();
            Debug.Assert(d != 0.0f);
            float s = 2.0f / d;
            float xs = q.X * s, ys = q.Y * s, zs = q.Z * s;
            float wx = q.W * xs, wy = q.W * ys, wz = q.W * zs;
            float xx = q.X * xs, xy = q.X * ys, xz = q.X * zs;
            float yy = q.Y * ys, yz = q.Y * zs, zz = q.Z * zs;
            _Row0 = new IndexedVector3(1.0f - (yy + zz), xy - wz, xz + wy);
            _Row1 = new IndexedVector3(xy + wz, 1.0f - (xx + zz), yz - wx);
            _Row2 = new IndexedVector3(xz - wy, yz + wx, 1.0f - (xx + yy));
        }


        public IndexedBasisMatrix(ref Quaternion q)
        {
            float d = q.LengthSquared();
            Debug.Assert(d != 0.0f);
            float s = 2.0f / d;
            float xs = q.X * s, ys = q.Y * s, zs = q.Z * s;
            float wx = q.W * xs, wy = q.W * ys, wz = q.W * zs;
            float xx = q.X * xs, xy = q.X * ys, xz = q.X * zs;
            float yy = q.Y * ys, yz = q.Y * zs, zz = q.Z * zs;
            _Row0 = new IndexedVector3(1.0f - (yy + zz), xy - wz, xz + wy);
            _Row1 = new IndexedVector3(xy + wz, 1.0f - (xx + zz), yz - wx);
            _Row2 = new IndexedVector3(xz - wy, yz + wx, 1.0f - (xx + yy));
        }



        public void SetValue(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            this[0] = new IndexedVector3(m11, m12, m13);
            this[1] = new IndexedVector3(m21, m22, m23);
            this[2] = new IndexedVector3(m31, m32, m33);
        }


        public IndexedVector3 GetColumn(int i)
        {
            Debug.Assert(i >= 0 && i < 3);
            return new IndexedVector3(_Row0[i], _Row1[i], _Row2[i]);
        }

        public IndexedVector3 GetRow(int i)
        {
            Debug.Assert(i >= 0 && i < 3);
            switch (i)
            {
                case (0):
                    return _Row0;
                case (1):
                    return _Row1;
                case (2):
                    return _Row2;
            }
            Debug.Assert(false);
            return IndexedVector3.Zero;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is IndexedBasisMatrix))
            {
                return false;
            }

            var right = (IndexedBasisMatrix) obj;
            return _Row0.Equals(right._Row0) && _Row1.Equals(right._Row1) && _Row2.Equals(right._Row2);
        }

        public float this[int i, int j]
        {
            get
            {
                switch (i)
                {
                    case (0):
                        {
                            switch (j)
                            {
                                case (0):
                                    return _Row0.X;
                                case (1):
                                    return _Row0.Y;
                                case (2):
                                    return _Row0.Z;
                                default:
                                    break;
                            }
                            break;
                        }
                    case (1):
                        {
                            switch (j)
                            {
                                case (0):
                                    return _Row1.X;
                                case (1):
                                    return _Row1.Y;
                                case (2):
                                    return _Row1.Z;
                                default:
                                    break;
                            }
                            break;
                        }
                    case (2):
                        {
                            switch (j)
                            {
                                case (0):
                                    return _Row2.X;
                                case (1):
                                    return _Row2.Y;
                                case (2):
                                    return _Row2.Z;
                                default:
                                    break;
                            }
                            break;
                        }
                }
                Debug.Assert(false);
                return 0.0f;
            }
            set
            {
                switch (i)
                {
                    case (0):
                        {
                            switch (j)
                            {
                                case (0):
                                    _Row0.X = value;
                                    break;
                                case (1):
                                    _Row0.Y = value;
                                    break;
                                case (2):
                                    _Row0.Z = value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    case (1):
                        {
                            switch (j)
                            {
                                case (0):
                                    _Row1.X = value;
                                    break;
                                case (1):
                                    _Row1.Y = value;
                                    break;
                                case (2):
                                    _Row1.Z = value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    case (2):
                        {
                            switch (j)
                            {
                                case (0):
                                    _Row2.X = value;
                                    break;
                                case (1):
                                    _Row2.Y = value;
                                    break;
                                case (2):
                                    _Row2.Z = value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                }
            }

        }


        public IndexedVector3 this[int i]
        {
            get
            {
                Debug.Assert(i >= 0 && i < 3);
                switch (i)
                {
                    case (0):
                        return _Row0;
                    case (1):
                        return _Row1;
                    case (2):
                        return _Row2;
                }
                Debug.Assert(false);
                return IndexedVector3.Zero;
            }
            set
            {
                Debug.Assert(i >= 0 && i < 3);

                switch (i)
                {
                    case (0):
                        _Row0 = value;
                        break;
                    case (1):
                        _Row1 = value;
                        break;
                    case (2):
                        _Row2 = value;
                        break;
                }
            }
        }



        public static IndexedBasisMatrix Transpose(IndexedBasisMatrix IndexedMatrix)
        {
            return new IndexedBasisMatrix(IndexedMatrix._Row0.X, IndexedMatrix._Row1.X, IndexedMatrix._Row2.X,
                IndexedMatrix._Row0.Y, IndexedMatrix._Row1.Y, IndexedMatrix._Row2.Y,
                IndexedMatrix._Row0.Z, IndexedMatrix._Row1.Z, IndexedMatrix._Row2.Z);

        }

        public static void Transpose(ref IndexedBasisMatrix IndexedMatrix, out IndexedBasisMatrix result)
        {
            result = new IndexedBasisMatrix(IndexedMatrix._Row0.X, IndexedMatrix._Row1.X, IndexedMatrix._Row2.X,
                IndexedMatrix._Row0.Y, IndexedMatrix._Row1.Y, IndexedMatrix._Row2.Y,
                IndexedMatrix._Row0.Z, IndexedMatrix._Row1.Z, IndexedMatrix._Row2.Z);
        }

        public static bool operator ==(IndexedBasisMatrix matrix1, IndexedBasisMatrix matrix2)
        {
            return matrix1._Row0 == matrix2._Row0 &&
                matrix1._Row1 == matrix2._Row1 &&
                matrix1._Row2 == matrix2._Row2;
        }

        public static bool operator !=(IndexedBasisMatrix matrix1, IndexedBasisMatrix matrix2)
        {
            return matrix1._Row0 != matrix2._Row0 ||
                matrix1._Row1 != matrix2._Row1 ||
                matrix1._Row2 != matrix2._Row2;
        }

        public static IndexedVector3 operator *(IndexedBasisMatrix m, IndexedVector3 v)
        {
            return new IndexedVector3(m._Row0.Dot(ref v), m._Row1.Dot(ref v), m._Row2.Dot(ref v));
        }

        public static IndexedVector3 operator *(IndexedVector3 v, IndexedBasisMatrix m)
        {
            return new IndexedVector3(m.TDotX(ref v), m.TDotY(ref v), m.TDotZ(ref v));
        }

        public static IndexedBasisMatrix operator *(IndexedBasisMatrix m1, IndexedBasisMatrix m2)
        {
            return new IndexedBasisMatrix(
                m2.TDotX(ref m1._Row0), m2.TDotY(ref m1._Row0), m2.TDotZ(ref m1._Row0),
                m2.TDotX(ref m1._Row1), m2.TDotY(ref m1._Row1), m2.TDotZ(ref m1._Row1),
                m2.TDotX(ref m1._Row2), m2.TDotY(ref m1._Row2), m2.TDotZ(ref m1._Row2));
        }


        public void SetEulerZYX(float eulerX, float eulerY, float eulerZ)
        {
            float ci = (float)Math.Cos(eulerX);
            float cj = (float)Math.Cos(eulerY);
            float ch = (float)Math.Cos(eulerZ);
            float si = (float)Math.Sin(eulerX);
            float sj = (float)Math.Sin(eulerY);
            float sh = (float)Math.Sin(eulerZ);
            float cc = ci * ch;
            float cs = ci * sh;
            float sc = si * ch;
            float ss = si * sh;

            SetValue(cj * ch, sj * sc - cs, sj * cc + ss, cj * sh, sj * ss + cc, sj * cs - sc, -sj, cj * si, cj * ci);

        }

        public float TDotX(ref IndexedVector3 v)
        {
            return _Row0.X * v.X + _Row1.X * v.Y + _Row2.X * v.Z;
        }
        public float TDotY(ref IndexedVector3 v)
        {
            return _Row0.Y * v.X + _Row1.Y * v.Y + _Row2.Y * v.Z;
        }
        public float TDotZ(ref IndexedVector3 v)
        {
            return _Row0.Z * v.X + _Row1.Z * v.Y + _Row2.Z * v.Z;
        }

        public IndexedBasisMatrix Inverse()
        {
            IndexedVector3 co = new IndexedVector3(Cofac(1, 1, 2, 2), Cofac(1, 2, 2, 0), Cofac(1, 0, 2, 1));
            float det = this[0].Dot(co);
            Debug.Assert(det != 0.0f);
            float s = 1.0f / det;
            return new IndexedBasisMatrix(co.X * s, Cofac(0, 2, 2, 1) * s, Cofac(0, 1, 1, 2) * s,
                co.Y * s, Cofac(0, 0, 2, 2) * s, Cofac(0, 2, 1, 0) * s,
                co.Z * s, Cofac(0, 1, 2, 0) * s, Cofac(0, 0, 1, 1) * s);

        }

        public float Cofac(int r1, int c1, int r2, int c2)
        {
            // slow?
            return this[r1][c1] * this[r2][c2] - this[r1][c2] * this[r2][c1];
        }

        public IndexedBasisMatrix TransposeTimes(IndexedBasisMatrix m)
        {
            return new IndexedBasisMatrix(
        _Row0.X * m._Row0.X + _Row1.X * m._Row1.X + _Row2.X * m._Row2.X,
        _Row0.X * m._Row0.Y + _Row1.X * m._Row1.Y + _Row2.X * m._Row2.Y,
        _Row0.X * m._Row0.Z + _Row1.X * m._Row1.Z + _Row2.X * m._Row2.Z,
        _Row0.Y * m._Row0.X + _Row1.Y * m._Row1.X + _Row2.Y * m._Row2.X,
        _Row0.Y * m._Row0.Y + _Row1.Y * m._Row1.Y + _Row2.Y * m._Row2.Y,
        _Row0.Y * m._Row0.Z + _Row1.Y * m._Row1.Z + _Row2.Y * m._Row2.Z,
        _Row0.Z * m._Row0.X + _Row1.Z * m._Row1.X + _Row2.Z * m._Row2.X,
        _Row0.Z * m._Row0.Y + _Row1.Z * m._Row1.Y + _Row2.Z * m._Row2.Y,
        _Row0.Z * m._Row0.Z + _Row1.Z * m._Row1.Z + _Row2.Z * m._Row2.Z);

        }

        public IndexedBasisMatrix TransposeTimes(ref IndexedBasisMatrix m)
        {
            return new IndexedBasisMatrix(
        _Row0.X * m._Row0.X + _Row1.X * m._Row1.X + _Row2.X * m._Row2.X,
        _Row0.X * m._Row0.Y + _Row1.X * m._Row1.Y + _Row2.X * m._Row2.Y,
        _Row0.X * m._Row0.Z + _Row1.X * m._Row1.Z + _Row2.X * m._Row2.Z,
        _Row0.Y * m._Row0.X + _Row1.Y * m._Row1.X + _Row2.Y * m._Row2.X,
        _Row0.Y * m._Row0.Y + _Row1.Y * m._Row1.Y + _Row2.Y * m._Row2.Y,
        _Row0.Y * m._Row0.Z + _Row1.Y * m._Row1.Z + _Row2.Y * m._Row2.Z,
        _Row0.Z * m._Row0.X + _Row1.Z * m._Row1.X + _Row2.Z * m._Row2.X,
        _Row0.Z * m._Row0.Y + _Row1.Z * m._Row1.Y + _Row2.Z * m._Row2.Y,
        _Row0.Z * m._Row0.Z + _Row1.Z * m._Row1.Z + _Row2.Z * m._Row2.Z);

        }

        public IndexedBasisMatrix TimesTranspose(IndexedBasisMatrix m)
        {
            return new IndexedBasisMatrix(
                _Row0.Dot(m._Row0), _Row0.Dot(m._Row1), _Row0.Dot(m._Row2),
                _Row1.Dot(m._Row0), _Row1.Dot(m._Row1), _Row1.Dot(m._Row2),
                _Row2.Dot(m._Row0), _Row2.Dot(m._Row1), _Row2.Dot(m._Row2));
        }


        public IndexedBasisMatrix Transpose()
        {
            return new IndexedBasisMatrix(_Row0.X, _Row1.X, _Row2.X,
                _Row0.Y, _Row1.Y, _Row2.Y,
                _Row0.Z, _Row1.Z, _Row2.Z);
        }



        public IndexedBasisMatrix Absolute()
        {
            return new IndexedBasisMatrix(_Row0.Abs(), _Row1.Abs(), _Row2.Abs());
        }

        public Quaternion GetRotation()
        {
            float trace = _Row0.X + _Row1.Y + _Row2.Z;
            IndexedVector3 temp = new IndexedVector3();
            float temp2 = 0f;
            if (trace > 0.0f)
            {
                float s = (float)Math.Sqrt(trace + 1.0f);
                temp2 = (s * 0.5f);
                s = 0.5f / s;

                temp[0] = ((_Row2.Y - _Row1.Z) * s);
                temp[1] = ((_Row0.Z - _Row2.X) * s);
                temp[2] = ((_Row1.X - _Row0.Y) * s);
            }
            else
            {
                int i = _Row0.X < _Row1.Y ?
                    (_Row1.Y < _Row2.Z ? 2 : 1) :
                    (_Row0.X < _Row2.Z ? 2 : 0);
                int j = (i + 1) % 3;
                int k = (i + 2) % 3;

                float s = (float)Math.Sqrt(this[i][i] - this[j][j] - this[k][k] + 1.0f);
                temp[i] = s * 0.5f;
                s = 0.5f / s;

                temp2 = (this[k][j] - this[j][k]) * s;
                temp[j] = (this[j][i] + this[i][j]) * s;
                temp[k] = (this[k][i] + this[i][k]) * s;
            }
            return new Quaternion(temp[0], temp[1], temp[2], temp2);

        }



        public void SetRotation(Quaternion q)
        {
            float d = q.LengthSquared();
            Debug.Assert(d != 0.0f);
            float s = 2.0f / d;
            float xs = q.X * s, ys = q.Y * s, zs = q.Z * s;
            float wx = q.W * xs, wy = q.W * ys, wz = q.W * zs;
            float xx = q.X * xs, xy = q.X * ys, xz = q.X * zs;
            float yy = q.Y * ys, yz = q.Y * zs, zz = q.Z * zs;
            SetValue(1.0f - (yy + zz), xy - wz, xz + wy,
                xy + wz, 1.0f - (xx + zz), yz - wx,
                xz - wy, yz + wx, 1.0f - (xx + yy));
        }

        public void SetRotation(ref Quaternion q)
        {
            float d = q.LengthSquared();
            Debug.Assert(d != 0.0f);
            float s = 2.0f / d;
            float xs = q.X * s, ys = q.Y * s, zs = q.Z * s;
            float wx = q.W * xs, wy = q.W * ys, wz = q.W * zs;
            float xx = q.X * xs, xy = q.X * ys, xz = q.X * zs;
            float yy = q.Y * ys, yz = q.Y * zs, zz = q.Z * zs;
            SetValue(1.0f - (yy + zz), xy - wz, xz + wy,
                xy + wz, 1.0f - (xx + zz), yz - wx,
                xz - wy, yz + wx, 1.0f - (xx + yy));
        }



        /**@brief diagonalizes this matrix by the Jacobi method.
       * @param rot stores the rotation from the coordinate system in which the matrix is diagonal to the original
       * coordinate system, i.e., old_this = rot * new_this * rot^T. 
       * @param threshold See iteration
       * @param iteration The iteration stops when all off-diagonal elements are less than the threshold multiplied 
       * by the sum of the absolute values of the diagonal, or when maxSteps have been executed. 
       * 
       * Note that this matrix is assumed to be symmetric. 
       */
        public void Diagonalize(out IndexedMatrix rot, float threshold, int maxSteps)
        {

            rot = IndexedMatrix.Identity;
            for (int step = maxSteps; step > 0; step--)
            {
                // find off-diagonal element [p][q] with largest magnitude
                int p = 0;
                int q = 1;
                int r = 2;
                float max = Math.Abs(this[0][1]);
                float v = Math.Abs(this[0][2]);
                if (v > max)
                {
                    q = 2;
                    r = 1;
                    max = v;
                }
                v = Math.Abs(this[1][2]);
                if (v > max)
                {
                    p = 1;
                    q = 2;
                    r = 0;
                    max = v;
                }

                float t = threshold * (Math.Abs(this[0][0]) + Math.Abs(this[1][1]) + Math.Abs(this[2][2]));
                if (max <= t)
                {
                    if (max <= MathUtil.SIMD_EPSILON * t)
                    {
                        return;
                    }
                    step = 1;
                }

                // compute Jacobi rotation J which leads to a zero for element [p][q] 
                float mpq = this[p][q];
                float theta = (this[q][q] - this[p][p]) / (2 * mpq);
                float theta2 = theta * theta;
                float cos;
                float sin;
                if (theta2 * theta2 < (10.0f / MathUtil.SIMD_EPSILON))
                {
                    t = (theta >= 0.0f) ? (1.0f / (float)(theta + (float)Math.Sqrt(1 + theta2)))
                        : (1 / (theta - (float)Math.Sqrt(1 + theta2)));
                    cos = 1.0f / (float)Math.Sqrt(1 + t * t);
                    sin = cos * t;
                }
                else
                {
                    // approximation for large theta-value, i.e., a nearly diagonal matrix
                    t = 1 / (theta * (2 + 0.5f / theta2));
                    cos = 1 - 0.5f * t * t;
                    sin = cos * t;
                }

                // apply rotation to matrix (this = J^T * this * J)
                this[p, q] = 0;
                this[q, p] = 0;
                this[p, p] -= t * mpq;
                this[q, q] += t * mpq;
                float mrp = this[r][p];
                float mrq = this[r][q];
                this[r, p] = this[p, r] = cos * mrp - sin * mrq;
                this[r, q] = this[q, r] = cos * mrq + sin * mrp;

                // apply rotation to rot (rot = rot * J)
                for (int i = 0; i < 3; i++)
                {
                    mrp = this[i, p];
                    mrq = this[i, q];
                    this[i, p] = cos * mrp - sin * mrq;
                    this[i, q] = cos * mrq + sin * mrp;
                }
            }
        }



        public IndexedVector3 _Row0;
        public IndexedVector3 _Row1;
        public IndexedVector3 _Row2;

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _Row0.GetHashCode();
                result = (result * 397) ^ _Row1.GetHashCode();
                result = (result * 397) ^ _Row2.GetHashCode();
                return result;
            }
        }

    }
}

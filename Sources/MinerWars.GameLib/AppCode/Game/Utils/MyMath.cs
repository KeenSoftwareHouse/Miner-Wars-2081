
using System;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyMath
    {
        private static Vector3[] m_corners = new Vector3[8];
        //Number of steps dividing whole circle
        private const int ANGLE_GRANULARITY = 2*314;
        private static readonly float OneOverRoot3 = (float)Math.Pow(3, -0.5f);

        static float[] m_precomputedValues = new float[ANGLE_GRANULARITY];

        public static Vector3 Vector3One = Vector3.One;

        public static void Init()
        {
            for (int i = 0; i < 2*314; i++)
            {
                m_precomputedValues[i] = (float)Math.Sin(i / 100.0f);
            }
        }

        public static float FastSin(float angle)
        {
            //Reduce angle to interval 0-2PI
            int angleInt = (int)(angle * 100.0f);
            angleInt = angleInt % ANGLE_GRANULARITY;
            if (angleInt < 0)
                angleInt += ANGLE_GRANULARITY;

            return m_precomputedValues[angleInt];
        }

        public static float FastCos(float angle)
        {
            return FastSin(angle + MathHelper.PiOver2);
        }

        //Creates rotation matrix from direction (dir must be normalized)
        public static Matrix MatrixFromDir(Vector3 dir)
        {
            Vector3 right = new Vector3(0.0f,0.0f,1.0f);
            Vector3 up;
            float d = dir.Z;

            if (d > -0.99999 && d < 0.99999)
            { // to avoid problems with normalize in special cases		
                right = right - dir * d;
                right = MyMwcUtils.Normalize(right);
                up = Vector3.Cross(dir, right);
            }
            else
            { //dir lies with z axis
                right = new Vector3(dir.Z, 0, -dir.X);
                up = new Vector3(0, 1, 0);
            };

            Matrix m = Matrix.Identity;
            m.Right = right;
            m.Up = up;
            m.Forward = dir;

            return m;
        }

        public static Matrix NormalizeMatrix(Matrix matrix)
        {
            Matrix m = matrix;
            m.Right = MyMwcUtils.Normalize(m.Right);
            m.Up = MyMwcUtils.Normalize(m.Up);
            m.Forward = MyMwcUtils.Normalize(m.Forward);
            return m;
        }

        public static float NormalizeAngle(float angle, float center = 0.0f)
        {
            return angle - MathHelper.TwoPi * (float)Math.Floor((double)((angle + MathHelper.Pi - center) / MathHelper.TwoPi));
        }

        /// <summary>
        /// ArcTanAngle
        /// </summary>
        /// <returns>ArcTan angle between x and y</returns>
        public static float ArcTanAngle(float x, float y)
        {
            if (x == 0.0f)
            {
                if (y == 1.0f)
                {
                    return (float)MathHelper.PiOver2;
                }
                else
                {
                    return (float)-MathHelper.PiOver2;
                }
            }
            else if (x > 0.0f)
                return (float)Math.Atan(y / x);
            else if (x < 0.0f)
            {
                if (y > 0.0f)
                    return (float)Math.Atan(y / x) + MathHelper.Pi;
                else
                    return (float)Math.Atan(y / x) - MathHelper.Pi;
            }
            else
                return 0.0f;
        }

        public static Vector3 Abs(ref Vector3 vector)
        {
            return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }

        /// <summary>
        /// Return vector with each component max
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 MaxComponents(ref Vector3 a, ref Vector3 b)
        {
            return new Vector3(MathHelper.Max(a.X, b.X), MathHelper.Max(a.Y, b.Y), MathHelper.Max(a.Z, b.Z));
        }

        /// <summary>
        /// AngleTo 
        /// </summary>
        /// <returns>Angle between the vector lines</returns>
        public static Vector3 AngleTo(Vector3 From, Vector3 Location)
        {
            Vector3 angle = Vector3.Zero;
            Vector3 v = Vector3.Normalize(Location - From);
            angle.X = (float)Math.Asin(v.Y);
            angle.Y = ArcTanAngle(-v.Z, -v.X);
            return angle;
        }

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            var dotProd = Vector3.Dot(a, b);
            var lenProd = a.Length() * b.Length();
            var divOperation = dotProd / lenProd;
            if (MyMwcUtils.IsZero(1.0f - divOperation))
                return 0;
            else
                return (float)(Math.Acos(divOperation));
        }

        /// <summary>
        /// QuaternionToEuler 
        /// </summary>
        /// <returns>Converted quaternion to the euler pitch, rot, yaw</returns>
        public static Vector3 QuaternionToEuler(Quaternion Rotation)
        {
            Vector3 forward = Vector3.Transform(Vector3.Forward, Rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, Rotation);
            Vector3 rotationAxes = AngleTo(new Vector3(), forward);
            if (rotationAxes.X == MathHelper.PiOver2)
            {
                rotationAxes.Y = ArcTanAngle(up.Z, up.X);
                rotationAxes.Z = 0.0f;
            }
            else if (rotationAxes.X == -MathHelper.PiOver2)
            {
                rotationAxes.Y = ArcTanAngle(-up.Y, -up.X);
                rotationAxes.Z = 0.0f;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationAxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationAxes.X));
                rotationAxes.Z = ArcTanAngle(up.Y, -up.X);
            }
            return rotationAxes;
        }


        /// <summary>
        /// CreateInvalidAABB
        /// </summary>
        /// <returns></returns>
        public static BoundingBox CreateInvalidAABB()
        {
            BoundingBox aabb = new BoundingBox();
            aabb = aabb.CreateInvalid();
            return aabb;
        }


        /// <summary>
        /// retrun center of bb
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Vector3 GetCenter(this BoundingBox bbox)
        {
            return (bbox.Max + bbox.Min) / 2.0f;
        }

        /// <summary>
        /// Intersects
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Intersects(this BoundingBox bbox, ref Vector3 point)
        {
            return ((point.X >= bbox.Min.X && point.X <= bbox.Max.X) && (point.Y >= bbox.Min.Y && point.Y <= bbox.Max.Y) && (point.Z >= bbox.Min.Z && point.Z <= bbox.Max.Z));
        }


        /// <summary>
        /// This projection results to initial velocity of non-engine objects, which parents move in some velocity
        /// We want to add only forward speed of the parent to the forward direction of the object, and if parent
        /// is going backward, no speed is added.
        /// </summary>
        /// <param name="forwardVector"></param>
        /// <param name="projectedVector"></param>
        /// <returns></returns>
        public static Vector3 ForwardVectorProjection(Vector3 forwardVector, Vector3 projectedVector)
        {
            Vector3 forwardVelocity = forwardVector;

            if (Vector3.Dot(projectedVector, forwardVector) > 0)
            {  //going forward
                forwardVelocity = forwardVector.Project(projectedVector + forwardVector);
                return forwardVelocity;
            }

            return Vector3.Zero;
        }

        /// <summary>
        /// Returns nearest bigger power of two
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int GetNearestBiggerPowerOfTwo(float f)
        {
            int x = 1;  
            while(x < f) 
            {    
                x <<= 1;  
            }

            return x;
        }


        public static BoundingBox CreateFromInsideRadius(float radius)
        {
            float halfSize = OneOverRoot3 * radius;
            return new BoundingBox(-new Vector3(halfSize), new Vector3(halfSize));
        }

        /// <summary>
        /// Calculates color from vector
        /// </summary>
        public static Vector3 VectorFromColor(byte red, byte green, byte blue)
        {
            return new Vector3(red / 255.0f, green / 255.0f, blue / 255.0f);
        }
        public static Vector4 VectorFromColor(byte red, byte green, byte blue, byte alpha)
        {
            return new Vector4(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
        }

        
        /// <summary>
        /// Return minimum distance between line segment v-w and point p.
        /// </summary>
        public static float DistanceSquaredFromLineSegment(Vector3 v, Vector3 w, Vector3 p)
        {
            Vector3 d = w - v;
            float l = d.LengthSquared();
            if (l == 0) return Vector3.DistanceSquared(p, v);   // v == w case

            float t = Vector3.Dot(p - v, d);
            if (t <= 0) return Vector3.DistanceSquared(p, v);       // Beyond the 'v' end of the segment
            else if (t >= l) return Vector3.DistanceSquared(p, w);  // Beyond the 'w' end of the segment
            else return Vector3.DistanceSquared(p, v + (t/l)*d);        // On the segment
        }
    }

    /// <summary>
    /// Usefull Vector3 extensions
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Calculates projection vector
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="length">The length.</param>
        public static Vector3 Project(this Vector3 projectedOntoVector, Vector3 projectedVector)
        {
            float dotProduct = 0.0f;
            dotProduct = Vector3.Dot(projectedVector, projectedOntoVector);

            Vector3 projectedOutputVector = (dotProduct / projectedOntoVector.LengthSquared()) * projectedOntoVector;
            return projectedOutputVector;
        }
    }

    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        private static Vector3[] temporaryCorners = new Vector3[8];

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="worldMatrix"></param>
        /// <returns></returns>
        public static BoundingBox Translate(this BoundingBox bbox, Matrix worldMatrix)
        {
            bbox.Min += worldMatrix.Translation;
            bbox.Max += worldMatrix.Translation;
            return bbox;
        }


        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="vctTranlsation"></param>
        /// <returns></returns>
        public static BoundingBox Translate(this BoundingBox bbox, Vector3 vctTranlsation)
        {
            bbox.Min += vctTranlsation;
            bbox.Max += vctTranlsation;
            return bbox;
        }

        /// <summary>
        /// Size
        /// </summary>
        /// <returns></returns>
        public static Vector3 Size(this BoundingBox bbox)
        {
            return bbox.Max - bbox.Min;
        }


        public static BoundingBox Transform(this BoundingBox bbox, Matrix worldMatrix)
        {
            BoundingBox oobb = MyMath.CreateInvalidAABB();
            
            lock (temporaryCorners)
            {
                bbox.GetCorners(temporaryCorners);

                foreach (Vector3 vct in temporaryCorners)
                {
                    Vector3 vctTransformed = Vector3.Transform(vct, worldMatrix);
                    oobb = oobb.Include(ref vctTransformed);
                }
            }

            return oobb;
        }


        /// <summary>
        /// return expanded aabb (abb include point)
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static BoundingBox Include(this BoundingBox bbox, ref Vector3 point)
        {
            if (point.X < bbox.Min.X)
                bbox.Min.X = point.X;

            if (point.Y < bbox.Min.Y)
                bbox.Min.Y = point.Y;

            if (point.Z < bbox.Min.Z)
                bbox.Min.Z = point.Z;


            if (point.X > bbox.Max.X)
                bbox.Max.X = point.X;

            if (point.Y > bbox.Max.Y)
                bbox.Max.Y = point.Y;

            if (point.Z > bbox.Max.Z)
                bbox.Max.Z = point.Z;

            return bbox;
        }

        /// <summary>
        /// return expanded aabb (abb include point)
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static BoundingBox Include(this BoundingBox bbox, ref BoundingBox box)
        {
            bbox.Min = Vector3.Min(bbox.Min, box.Min);
            bbox.Max = Vector3.Max(bbox.Max, box.Max);
            return bbox;
        }

        public static BoundingBox CreateInvalid(this BoundingBox bbox)
        {
            Vector3 vctMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vctMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            bbox.Min = vctMin;
            bbox.Max = vctMax;

            return bbox;
        }

        /// <summary>
        /// return perimeter of edges
        /// </summary>
        /// <returns></returns>
        public static float Perimeter(this BoundingBox bbox)
        {
            float wx = bbox.Max.X - bbox.Min.X;
            float wy = bbox.Max.Y - bbox.Min.Y;
            float wz = bbox.Max.Z - bbox.Min.Z;

            return 4.0f * (wx + wy + wz);
        }

        public static void Inflate(this BoundingBox bbox, float size)
        {
            bbox.Max += new Vector3(size);
            bbox.Min -= new Vector3(size);
        }
    }

    public static class BoundingFrustumExtensions
    {
        /// <summary>
        /// Creates bounding sphere from bounding frustum.
        /// Implementation taken from XNA source, replace IEnumerable with array
        /// </summary>
        /// <param name="frustum">The bounding frustum.</param>
        /// <param name="corners">Temporary memory to save corner when getting from frustum.</param>
        /// <returns>BoundingSphere</returns>
        public static BoundingSphere ToBoundingSphere(this BoundingFrustum frustum, Vector3[] corners)
        {
            float num;
            float num2;
            Vector3 vector2;
            float num4;
            float num5;
            BoundingSphere sphere;
            Vector3 vector5;
            Vector3 vector6;
            Vector3 vector7;
            Vector3 vector8;
            Vector3 vector9;

            if (corners.Length < 8)
            {
                throw new ArgumentException("Corners length must be at least 8");
            }

            frustum.GetCorners(corners);

            Vector3 vector4 = vector5 = vector6 = vector7 = vector8 = vector9 = corners[0];

            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 vector = corners[i];

                if (vector.X < vector4.X)
                {
                    vector4 = vector;
                }
                if (vector.X > vector5.X)
                {
                    vector5 = vector;
                }
                if (vector.Y < vector6.Y)
                {
                    vector6 = vector;
                }
                if (vector.Y > vector7.Y)
                {
                    vector7 = vector;
                }
                if (vector.Z < vector8.Z)
                {
                    vector8 = vector;
                }
                if (vector.Z > vector9.Z)
                {
                    vector9 = vector;
                }
            }
            Vector3.Distance(ref vector5, ref vector4, out num5);
            Vector3.Distance(ref vector7, ref vector6, out num4);
            Vector3.Distance(ref vector9, ref vector8, out num2);
            if (num5 > num4)
            {
                if (num5 > num2)
                {
                    Vector3.Lerp(ref vector5, ref vector4, 0.5f, out vector2);
                    num = num5 * 0.5f;
                }
                else
                {
                    Vector3.Lerp(ref vector9, ref vector8, 0.5f, out vector2);
                    num = num2 * 0.5f;
                }
            }
            else if (num4 > num2)
            {
                Vector3.Lerp(ref vector7, ref vector6, 0.5f, out vector2);
                num = num4 * 0.5f;
            }
            else
            {
                Vector3.Lerp(ref vector9, ref vector8, 0.5f, out vector2);
                num = num2 * 0.5f;
            }
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 vector10 = corners[i];

                Vector3 vector3;
                vector3.X = vector10.X - vector2.X;
                vector3.Y = vector10.Y - vector2.Y;
                vector3.Z = vector10.Z - vector2.Z;
                float num3 = vector3.Length();
                if (num3 > num)
                {
                    num = (num + num3) * 0.5f;
                    vector2 += (Vector3)((1f - (num / num3)) * vector3);
                }
            }
            sphere.Center = vector2;
            sphere.Radius = num;
            return sphere;
        }
    }
}

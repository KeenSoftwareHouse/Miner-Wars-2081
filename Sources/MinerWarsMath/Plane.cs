using System;
using System.ComponentModel;
using System.Globalization;

namespace MinerWarsMath
{
    /// <summary>
    /// Defines a plane.
    /// </summary>
    [Serializable]
    public struct Plane : IEquatable<Plane>
    {
        /// <summary>
        /// The normal vector of the Plane.
        /// </summary>
        public Vector3 Normal;
        /// <summary>
        /// The distance of the Plane along its normal from the origin.
        /// </summary>
        public float D;

        /// <summary>
        /// Creates a new instance of Plane.
        /// </summary>
        /// <param name="a">X component of the normal defining the Plane.</param><param name="b">Y component of the normal defining the Plane.</param><param name="c">Z component of the normal defining the Plane.</param><param name="d">Distance of the Plane along its normal from the origin.</param>
        public Plane(float a, float b, float c, float d)
        {
            this.Normal.X = a;
            this.Normal.Y = b;
            this.Normal.Z = c;
            this.D = d;
        }

        /// <summary>
        /// Creates a new instance of Plane.
        /// </summary>
        /// <param name="normal">The normal vector to the Plane.</param><param name="d">The Plane's distance along its normal from the origin.</param>
        public Plane(Vector3 normal, float d)
        {
            this.Normal = normal;
            this.D = d;
        }

        /// <summary>
        /// Creates a new instance of Plane.
        /// </summary>
        /// <param name="value">Vector4 with X, Y, and Z components defining the normal of the Plane. The W component defines the distance of the Plane along the normal from the origin.</param>
        public Plane(Vector4 value)
        {
            this.Normal.X = value.X;
            this.Normal.Y = value.Y;
            this.Normal.Z = value.Z;
            this.D = value.W;
        }

        /// <summary>
        /// Creates a new instance of Plane.
        /// </summary>
        /// <param name="point1">One point of a triangle defining the Plane.</param><param name="point2">One point of a triangle defining the Plane.</param><param name="point3">One point of a triangle defining the Plane.</param>
        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float num1 = point2.X - point1.X;
            float num2 = point2.Y - point1.Y;
            float num3 = point2.Z - point1.Z;
            float num4 = point3.X - point1.X;
            float num5 = point3.Y - point1.Y;
            float num6 = point3.Z - point1.Z;
            float num7 = (float)((double)num2 * (double)num6 - (double)num3 * (double)num5);
            float num8 = (float)((double)num3 * (double)num4 - (double)num1 * (double)num6);
            float num9 = (float)((double)num1 * (double)num5 - (double)num2 * (double)num4);
            float num10 = 1f / (float)Math.Sqrt((double)num7 * (double)num7 + (double)num8 * (double)num8 + (double)num9 * (double)num9);
            this.Normal.X = num7 * num10;
            this.Normal.Y = num8 * num10;
            this.Normal.Z = num9 * num10;
            this.D = (float)-((double)this.Normal.X * (double)point1.X + (double)this.Normal.Y * (double)point1.Y + (double)this.Normal.Z * (double)point1.Z);
        }

        /// <summary>
        /// Determines whether two instances of Plane are equal.
        /// </summary>
        /// <param name="lhs">The object to the left of the equality operator.</param><param name="rhs">The object to the right of the equality operator.</param>
        public static bool operator ==(Plane lhs, Plane rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two instances of Plane are not equal.
        /// </summary>
        /// <param name="lhs">The object to the left of the inequality operator.</param><param name="rhs">The object to the right of the inequality operator.</param>
        public static bool operator !=(Plane lhs, Plane rhs)
        {
            if ((double)lhs.Normal.X == (double)rhs.Normal.X && (double)lhs.Normal.Y == (double)rhs.Normal.Y && (double)lhs.Normal.Z == (double)rhs.Normal.Z)
                return (double)lhs.D != (double)rhs.D;
            else
                return true;
        }

        /// <summary>
        /// Determines whether the specified Plane is equal to the Plane.
        /// </summary>
        /// <param name="other">The Plane to compare with the current Plane.</param>
        public bool Equals(Plane other)
        {
            if ((double)this.Normal.X == (double)other.Normal.X && (double)this.Normal.Y == (double)other.Normal.Y && (double)this.Normal.Z == (double)other.Normal.Z)
                return (double)this.D == (double)other.D;
            else
                return false;
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the Plane.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Plane.</param>
        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Plane)
                flag = this.Equals((Plane)obj);
            return flag;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Normal.GetHashCode() + this.D.GetHashCode();
        }

        /// <summary>
        /// Returns a String that represents the current Plane.
        /// </summary>
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format((IFormatProvider)currentCulture, "{{Normal:{0} D:{1}}}", new object[2]
      {
        (object) this.Normal.ToString(),
        (object) this.D.ToString((IFormatProvider) currentCulture)
      });
        }

        /// <summary>
        /// Changes the coefficients of the Normal vector of this Plane to make it of unit length.
        /// </summary>
        public void Normalize()
        {
            float num1 = (float)((double)this.Normal.X * (double)this.Normal.X + (double)this.Normal.Y * (double)this.Normal.Y + (double)this.Normal.Z * (double)this.Normal.Z);
            if ((double)Math.Abs(num1 - 1f) < 1.19209289550781E-07)
                return;
            float num2 = 1f / (float)Math.Sqrt((double)num1);
            this.Normal.X *= num2;
            this.Normal.Y *= num2;
            this.Normal.Z *= num2;
            this.D *= num2;
        }

        /// <summary>
        /// Changes the coefficients of the Normal vector of a Plane to make it of unit length.
        /// </summary>
        /// <param name="value">The Plane to normalize.</param>
        public static Plane Normalize(Plane value)
        {
            float num1 = (float)((double)value.Normal.X * (double)value.Normal.X + (double)value.Normal.Y * (double)value.Normal.Y + (double)value.Normal.Z * (double)value.Normal.Z);
            if ((double)Math.Abs(num1 - 1f) < 1.19209289550781E-07)
            {
                Plane plane;
                plane.Normal = value.Normal;
                plane.D = value.D;
                return plane;
            }
            else
            {
                float num2 = 1f / (float)Math.Sqrt((double)num1);
                Plane plane;
                plane.Normal.X = value.Normal.X * num2;
                plane.Normal.Y = value.Normal.Y * num2;
                plane.Normal.Z = value.Normal.Z * num2;
                plane.D = value.D * num2;
                return plane;
            }
        }

        /// <summary>
        /// Changes the coefficients of the Normal vector of a Plane to make it of unit length.
        /// </summary>
        /// <param name="value">The Plane to normalize.</param><param name="result">[OutAttribute] An existing plane Plane filled in with a normalized version of the specified plane.</param>
        public static void Normalize(ref Plane value, out Plane result)
        {
            float num1 = (float)((double)value.Normal.X * (double)value.Normal.X + (double)value.Normal.Y * (double)value.Normal.Y + (double)value.Normal.Z * (double)value.Normal.Z);
            if ((double)Math.Abs(num1 - 1f) < 1.19209289550781E-07)
            {
                result.Normal = value.Normal;
                result.D = value.D;
            }
            else
            {
                float num2 = 1f / (float)Math.Sqrt((double)num1);
                result.Normal.X = value.Normal.X * num2;
                result.Normal.Y = value.Normal.Y * num2;
                result.Normal.Z = value.Normal.Z * num2;
                result.D = value.D * num2;
            }
        }

        /// <summary>
        /// Transforms a normalized Plane by a Matrix.
        /// </summary>
        /// <param name="plane">The normalized Plane to transform. This Plane must already be normalized, so that its Normal vector is of unit length, before this method is called.</param><param name="matrix">The transform Matrix to apply to the Plane.</param>
        public static Plane Transform(Plane plane, Matrix matrix)
        {
            Matrix result;
            Matrix.Invert(ref matrix, out result);
            float num1 = plane.Normal.X;
            float num2 = plane.Normal.Y;
            float num3 = plane.Normal.Z;
            float num4 = plane.D;
            Plane plane1;
            plane1.Normal.X = (float)((double)num1 * (double)result.M11 + (double)num2 * (double)result.M12 + (double)num3 * (double)result.M13 + (double)num4 * (double)result.M14);
            plane1.Normal.Y = (float)((double)num1 * (double)result.M21 + (double)num2 * (double)result.M22 + (double)num3 * (double)result.M23 + (double)num4 * (double)result.M24);
            plane1.Normal.Z = (float)((double)num1 * (double)result.M31 + (double)num2 * (double)result.M32 + (double)num3 * (double)result.M33 + (double)num4 * (double)result.M34);
            plane1.D = (float)((double)num1 * (double)result.M41 + (double)num2 * (double)result.M42 + (double)num3 * (double)result.M43 + (double)num4 * (double)result.M44);
            return plane1;
        }

        /// <summary>
        /// Transforms a normalized Plane by a Matrix.
        /// </summary>
        /// <param name="plane">The normalized Plane to transform. This Plane must already be normalized, so that its Normal vector is of unit length, before this method is called.</param><param name="matrix">The transform Matrix to apply to the Plane.</param><param name="result">[OutAttribute] An existing Plane filled in with the results of applying the transform.</param>
        public static void Transform(ref Plane plane, ref Matrix matrix, out Plane result)
        {
            Matrix result1;
            Matrix.Invert(ref matrix, out result1);
            float num1 = plane.Normal.X;
            float num2 = plane.Normal.Y;
            float num3 = plane.Normal.Z;
            float num4 = plane.D;
            result.Normal.X = (float)((double)num1 * (double)result1.M11 + (double)num2 * (double)result1.M12 + (double)num3 * (double)result1.M13 + (double)num4 * (double)result1.M14);
            result.Normal.Y = (float)((double)num1 * (double)result1.M21 + (double)num2 * (double)result1.M22 + (double)num3 * (double)result1.M23 + (double)num4 * (double)result1.M24);
            result.Normal.Z = (float)((double)num1 * (double)result1.M31 + (double)num2 * (double)result1.M32 + (double)num3 * (double)result1.M33 + (double)num4 * (double)result1.M34);
            result.D = (float)((double)num1 * (double)result1.M41 + (double)num2 * (double)result1.M42 + (double)num3 * (double)result1.M43 + (double)num4 * (double)result1.M44);
        }

        /// <summary>
        /// Transforms a normalized Plane by a Quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized Plane to transform. This Plane must already be normalized, so that its Normal vector is of unit length, before this method is called.</param><param name="rotation">The Quaternion rotation to apply to the Plane.</param>
        public static Plane Transform(Plane plane, Quaternion rotation)
        {
            float num1 = rotation.X + rotation.X;
            float num2 = rotation.Y + rotation.Y;
            float num3 = rotation.Z + rotation.Z;
            float num4 = rotation.W * num1;
            float num5 = rotation.W * num2;
            float num6 = rotation.W * num3;
            float num7 = rotation.X * num1;
            float num8 = rotation.X * num2;
            float num9 = rotation.X * num3;
            float num10 = rotation.Y * num2;
            float num11 = rotation.Y * num3;
            float num12 = rotation.Z * num3;
            float num13 = 1f - num10 - num12;
            float num14 = num8 - num6;
            float num15 = num9 + num5;
            float num16 = num8 + num6;
            float num17 = 1f - num7 - num12;
            float num18 = num11 - num4;
            float num19 = num9 - num5;
            float num20 = num11 + num4;
            float num21 = 1f - num7 - num10;
            float num22 = plane.Normal.X;
            float num23 = plane.Normal.Y;
            float num24 = plane.Normal.Z;
            Plane plane1;
            plane1.Normal.X = (float)((double)num22 * (double)num13 + (double)num23 * (double)num14 + (double)num24 * (double)num15);
            plane1.Normal.Y = (float)((double)num22 * (double)num16 + (double)num23 * (double)num17 + (double)num24 * (double)num18);
            plane1.Normal.Z = (float)((double)num22 * (double)num19 + (double)num23 * (double)num20 + (double)num24 * (double)num21);
            plane1.D = plane.D;
            return plane1;
        }

        /// <summary>
        /// Transforms a normalized Plane by a Quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized Plane to transform. This Plane must already be normalized, so that its Normal vector is of unit length, before this method is called.</param><param name="rotation">The Quaternion rotation to apply to the Plane.</param><param name="result">[OutAttribute] An existing Plane filled in with the results of applying the rotation.</param>
        public static void Transform(ref Plane plane, ref Quaternion rotation, out Plane result)
        {
            float num1 = rotation.X + rotation.X;
            float num2 = rotation.Y + rotation.Y;
            float num3 = rotation.Z + rotation.Z;
            float num4 = rotation.W * num1;
            float num5 = rotation.W * num2;
            float num6 = rotation.W * num3;
            float num7 = rotation.X * num1;
            float num8 = rotation.X * num2;
            float num9 = rotation.X * num3;
            float num10 = rotation.Y * num2;
            float num11 = rotation.Y * num3;
            float num12 = rotation.Z * num3;
            float num13 = 1f - num10 - num12;
            float num14 = num8 - num6;
            float num15 = num9 + num5;
            float num16 = num8 + num6;
            float num17 = 1f - num7 - num12;
            float num18 = num11 - num4;
            float num19 = num9 - num5;
            float num20 = num11 + num4;
            float num21 = 1f - num7 - num10;
            float num22 = plane.Normal.X;
            float num23 = plane.Normal.Y;
            float num24 = plane.Normal.Z;
            result.Normal.X = (float)((double)num22 * (double)num13 + (double)num23 * (double)num14 + (double)num24 * (double)num15);
            result.Normal.Y = (float)((double)num22 * (double)num16 + (double)num23 * (double)num17 + (double)num24 * (double)num18);
            result.Normal.Z = (float)((double)num22 * (double)num19 + (double)num23 * (double)num20 + (double)num24 * (double)num21);
            result.D = plane.D;
        }

        /// <summary>
        /// Calculates the dot product of a specified Vector4 and this Plane.
        /// </summary>
        /// <param name="value">The Vector4 to multiply this Plane by.</param>
        public float Dot(Vector4 value)
        {
            return (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z + (double)this.D * (double)value.W);
        }

        /// <summary>
        /// Calculates the dot product of a specified Vector4 and this Plane.
        /// </summary>
        /// <param name="value">The Vector4 to multiply this Plane by.</param><param name="result">[OutAttribute] The dot product of the specified Vector4 and this Plane.</param>
        public void Dot(ref Vector4 value, out float result)
        {
            result = (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z + (double)this.D * (double)value.W);
        }

        /// <summary>
        /// Returns the dot product of a specified Vector3 and the Normal vector of this Plane plus the distance (D) value of the Plane.
        /// </summary>
        /// <param name="value">The Vector3 to multiply by.</param>
        public float DotCoordinate(Vector3 value)
        {
            return (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z) + this.D;
        }

        /// <summary>
        /// Returns the dot product of a specified Vector3 and the Normal vector of this Plane plus the distance (D) value of the Plane.
        /// </summary>
        /// <param name="value">The Vector3 to multiply by.</param><param name="result">[OutAttribute] The resulting value.</param>
        public void DotCoordinate(ref Vector3 value, out float result)
        {
            result = (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z) + this.D;
        }

        /// <summary>
        /// Returns the dot product of a specified Vector3 and the Normal vector of this Plane.
        /// </summary>
        /// <param name="value">The Vector3 to multiply by.</param>
        public float DotNormal(Vector3 value)
        {
            return (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z);
        }

        /// <summary>
        /// Returns the dot product of a specified Vector3 and the Normal vector of this Plane.
        /// </summary>
        /// <param name="value">The Vector3 to multiply by.</param><param name="result">[OutAttribute] The resulting dot product.</param>
        public void DotNormal(ref Vector3 value, out float result)
        {
            result = (float)((double)this.Normal.X * (double)value.X + (double)this.Normal.Y * (double)value.Y + (double)this.Normal.Z * (double)value.Z);
        }

        /// <summary>
        /// Checks whether the current Plane intersects a specified BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to test for intersection with.</param>
        public PlaneIntersectionType Intersects(BoundingBox box)
        {
            Vector3 vector3_1;
            vector3_1.X = (double)this.Normal.X >= 0.0 ? box.Min.X : box.Max.X;
            vector3_1.Y = (double)this.Normal.Y >= 0.0 ? box.Min.Y : box.Max.Y;
            vector3_1.Z = (double)this.Normal.Z >= 0.0 ? box.Min.Z : box.Max.Z;
            Vector3 vector3_2;
            vector3_2.X = (double)this.Normal.X >= 0.0 ? box.Max.X : box.Min.X;
            vector3_2.Y = (double)this.Normal.Y >= 0.0 ? box.Max.Y : box.Min.Y;
            vector3_2.Z = (double)this.Normal.Z >= 0.0 ? box.Max.Z : box.Min.Z;
            if ((double)this.Normal.X * (double)vector3_1.X + (double)this.Normal.Y * (double)vector3_1.Y + (double)this.Normal.Z * (double)vector3_1.Z + (double)this.D > 0.0)
                return PlaneIntersectionType.Front;
            return (double)this.Normal.X * (double)vector3_2.X + (double)this.Normal.Y * (double)vector3_2.Y + (double)this.Normal.Z * (double)vector3_2.Z + (double)this.D < 0.0 ? PlaneIntersectionType.Back : PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the current Plane intersects a BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to check for intersection with.</param><param name="result">[OutAttribute] An enumeration indicating whether the Plane intersects the BoundingBox.</param>
        public void Intersects(ref BoundingBox box, out PlaneIntersectionType result)
        {
            Vector3 vector3_1;
            vector3_1.X = (double)this.Normal.X >= 0.0 ? box.Min.X : box.Max.X;
            vector3_1.Y = (double)this.Normal.Y >= 0.0 ? box.Min.Y : box.Max.Y;
            vector3_1.Z = (double)this.Normal.Z >= 0.0 ? box.Min.Z : box.Max.Z;
            Vector3 vector3_2;
            vector3_2.X = (double)this.Normal.X >= 0.0 ? box.Max.X : box.Min.X;
            vector3_2.Y = (double)this.Normal.Y >= 0.0 ? box.Max.Y : box.Min.Y;
            vector3_2.Z = (double)this.Normal.Z >= 0.0 ? box.Max.Z : box.Min.Z;
            if ((double)this.Normal.X * (double)vector3_1.X + (double)this.Normal.Y * (double)vector3_1.Y + (double)this.Normal.Z * (double)vector3_1.Z + (double)this.D > 0.0)
                result = PlaneIntersectionType.Front;
            else if ((double)this.Normal.X * (double)vector3_2.X + (double)this.Normal.Y * (double)vector3_2.Y + (double)this.Normal.Z * (double)vector3_2.Z + (double)this.D < 0.0)
                result = PlaneIntersectionType.Back;
            else
                result = PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the current Plane intersects a specified BoundingFrustum.
        /// </summary>
        /// <param name="frustum">The BoundingFrustum to check for intersection with.</param>
        public PlaneIntersectionType Intersects(BoundingFrustum frustum)
        {
            return frustum.Intersects(this);
        }

        /// <summary>
        /// Checks whether the current Plane intersects a specified BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to check for intersection with.</param>
        public PlaneIntersectionType Intersects(BoundingSphere sphere)
        {
            float num = (float)((double)sphere.Center.X * (double)this.Normal.X + (double)sphere.Center.Y * (double)this.Normal.Y + (double)sphere.Center.Z * (double)this.Normal.Z) + this.D;
            if ((double)num > (double)sphere.Radius)
                return PlaneIntersectionType.Front;
            return (double)num < -(double)sphere.Radius ? PlaneIntersectionType.Back : PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the current Plane intersects a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to check for intersection with.</param><param name="result">[OutAttribute] An enumeration indicating whether the Plane intersects the BoundingSphere.</param>
        public void Intersects(ref BoundingSphere sphere, out PlaneIntersectionType result)
        {
            float num = (float)((double)sphere.Center.X * (double)this.Normal.X + (double)sphere.Center.Y * (double)this.Normal.Y + (double)sphere.Center.Z * (double)this.Normal.Z) + this.D;
            if ((double)num > (double)sphere.Radius)
                result = PlaneIntersectionType.Front;
            else if ((double)num < -(double)sphere.Radius)
                result = PlaneIntersectionType.Back;
            else
                result = PlaneIntersectionType.Intersecting;
        }
    }
}

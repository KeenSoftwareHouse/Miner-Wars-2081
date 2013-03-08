#define NATIVE_SUPPORT

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace MinerWarsMath
{
    /// <summary>
    /// Defines a vector with three components.
    /// </summary>
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        private static Vector3 _zero = new Vector3();
        private static Vector3 _one = new Vector3(1f, 1f, 1f);
        private static Vector3 _unitX = new Vector3(1f, 0.0f, 0.0f);
        private static Vector3 _unitY = new Vector3(0.0f, 1f, 0.0f);
        private static Vector3 _unitZ = new Vector3(0.0f, 0.0f, 1f);
        private static Vector3 _up = new Vector3(0.0f, 1f, 0.0f);
        private static Vector3 _down = new Vector3(0.0f, -1f, 0.0f);
        private static Vector3 _right = new Vector3(1f, 0.0f, 0.0f);
        private static Vector3 _left = new Vector3(-1f, 0.0f, 0.0f);
        private static Vector3 _forward = new Vector3(0.0f, 0.0f, -1f);
        private static Vector3 _backward = new Vector3(0.0f, 0.0f, 1f);
        /// <summary>
        /// Gets or sets the x-component of the vector.
        /// </summary>
        public float X;
        /// <summary>
        /// Gets or sets the y-component of the vector.
        /// </summary>
        public float Y;
        /// <summary>
        /// Gets or sets the z-component of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Returns a Vector3 with all of its components set to zero.
        /// </summary>
        public static Vector3 Zero
        {
            get
            {
                return Vector3._zero;
            }
        }

        /// <summary>
        /// Returns a Vector3 with ones in all of its components.
        /// </summary>
        public static Vector3 One
        {
            get
            {
                return Vector3._one;
            }
        }

        /// <summary>
        /// Returns the x unit Vector3 (1, 0, 0).
        /// </summary>
        public static Vector3 UnitX
        {
            get
            {
                return Vector3._unitX;
            }
        }

        /// <summary>
        /// Returns the y unit Vector3 (0, 1, 0).
        /// </summary>
        public static Vector3 UnitY
        {
            get
            {
                return Vector3._unitY;
            }
        }

        /// <summary>
        /// Returns the z unit Vector3 (0, 0, 1).
        /// </summary>
        public static Vector3 UnitZ
        {
            get
            {
                return Vector3._unitZ;
            }
        }

        /// <summary>
        /// Returns a unit vector designating up (0, 1, 0).
        /// </summary>
        public static Vector3 Up
        {
            get
            {
                return Vector3._up;
            }
        }

        /// <summary>
        /// Returns a unit Vector3 designating down (0, −1, 0).
        /// </summary>
        public static Vector3 Down
        {
            get
            {
                return Vector3._down;
            }
        }

        /// <summary>
        /// Returns a unit Vector3 pointing to the right (1, 0, 0).
        /// </summary>
        public static Vector3 Right
        {
            get
            {
                return Vector3._right;
            }
        }

        /// <summary>
        /// Returns a unit Vector3 designating left (−1, 0, 0).
        /// </summary>
        public static Vector3 Left
        {
            get
            {
                return Vector3._left;
            }
        }

        /// <summary>
        /// Returns a unit Vector3 designating forward in a right-handed coordinate system(0, 0, −1).
        /// </summary>
        public static Vector3 Forward
        {
            get
            {
                return Vector3._forward;
            }
        }

        /// <summary>
        /// Returns a unit Vector3 designating backward in a right-handed coordinate system (0, 0, 1).
        /// </summary>
        public static Vector3 Backward
        {
            get
            {
                return Vector3._backward;
            }
        }

        static Vector3()
        {
        }

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="x">Initial value for the x-component of the vector.</param><param name="y">Initial value for the y-component of the vector.</param><param name="z">Initial value for the z-component of the vector.</param>
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Creates a new instance of Vector3.
        /// </summary>
        /// <param name="value">Value to initialize each component to.</param>
        public Vector3(float value)
        {
            this.X = this.Y = this.Z = value;
        }

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="value">A vector containing the values to initialize x and y components with.</param><param name="z">Initial value for the z-component of the vector.</param>
        public Vector3(Vector2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param>
        public static Vector3 operator -(Vector3 value)
        {
            Vector3 vector3;
            vector3.X = -value.X;
            vector3.Y = -value.Y;
            vector3.Z = -value.Z;
            return vector3;
        }

        /// <summary>
        /// Tests vectors for equality.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            if ((double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y)
                return (double)value1.Z == (double)value2.Z;
            else
                return false;
        }

        /// <summary>
        /// Tests vectors for inequality.
        /// </summary>
        /// <param name="value1">Vector to compare.</param><param name="value2">Vector to compare.</param>
        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            if ((double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y)
                return (double)value1.Z != (double)value2.Z;
            else
                return true;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X + value2.X;
            vector3.Y = value1.Y + value2.Y;
            vector3.Z = value1.Z + value2.Z;
            return vector3;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X - value2.X;
            vector3.Y = value1.Y - value2.Y;
            vector3.Z = value1.Z - value2.Z;
            return vector3;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X * value2.X;
            vector3.Y = value1.Y * value2.Y;
            vector3.Z = value1.Z * value2.Z;
            return vector3;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="scaleFactor">Scalar value.</param>
        public static Vector3 operator *(Vector3 value, float scaleFactor)
        {
            Vector3 vector3;
            vector3.X = value.X * scaleFactor;
            vector3.Y = value.Y * scaleFactor;
            vector3.Z = value.Z * scaleFactor;
            return vector3;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="scaleFactor">Scalar value.</param><param name="value">Source vector.</param>
        public static Vector3 operator *(float scaleFactor, Vector3 value)
        {
            Vector3 vector3;
            vector3.X = value.X * scaleFactor;
            vector3.Y = value.Y * scaleFactor;
            vector3.Z = value.Z * scaleFactor;
            return vector3;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Divisor vector.</param>
        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X / value2.X;
            vector3.Y = value1.Y / value2.Y;
            vector3.Z = value1.Z / value2.Z;
            return vector3;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="divider">The divisor.</param>
        public static Vector3 operator /(Vector3 value, float divider)
        {
            float num = 1f / divider;
            Vector3 vector3;
            vector3.X = value.X * num;
            vector3.Y = value.Y * num;
            vector3.Z = value.Z * num;
            return vector3;
        }

        /// <summary>
        /// Retrieves a string representation of the current object.
        /// </summary>
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format((IFormatProvider)currentCulture, "{{X:{0} Y:{1} Z:{2}}}", (object)this.X.ToString((IFormatProvider)currentCulture), (object)this.Y.ToString((IFormatProvider)currentCulture), (object)this.Z.ToString((IFormatProvider)currentCulture));
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the Vector3.
        /// </summary>
        /// <param name="other">The Vector3 to compare with the current Vector3.</param>
        public bool Equals(Vector3 other)
        {
            if ((double)this.X == (double)other.X && (double)this.Y == (double)other.Y)
                return (double)this.Z == (double)other.Z;
            else
                return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">Object to make the comparison with.</param>
        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Vector3)
                flag = this.Equals((Vector3)obj);
            return flag;
        }

        /// <summary>
        /// Gets the hash code of the vector object.
        /// </summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode();
        }

        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        public float Length()
        {
            return (float)Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z);
        }

        /// <summary>
        /// Calculates the length of the vector squared.
        /// </summary>
        public float LengthSquared()
        {
            return (float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z);
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static float Distance(Vector3 value1, Vector3 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The distance between the vectors.</param>
        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = (float)((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
            result = (float)Math.Sqrt((double)num4);
        }

        /// <summary>
        /// Calculates the distance between two vectors squared.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            return (float)((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        }

        /// <summary>
        /// Calculates the distance between two vectors squared.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The distance between the two vectors squared.</param>
        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            result = (float)((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        }

        /// <summary>
        /// Calculates the dot product of two vectors. If the two vectors are unit vectors, the dot product returns a floating point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="vector2">Source vector.</param>
        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return (float)((double)vector1.X * (double)vector2.X + (double)vector1.Y * (double)vector2.Y + (double)vector1.Z * (double)vector2.Z);
        }

        /// <summary>
        /// Calculates the dot product of two vectors and writes the result to a user-specified variable. If the two vectors are unit vectors, the dot product returns a floating point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="vector2">Source vector.</param><param name="result">[OutAttribute] The dot product of the two vectors.</param>
        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
        {
            result = (float)((double)vector1.X * (double)vector2.X + (double)vector1.Y * (double)vector2.Y + (double)vector1.Z * (double)vector2.Z);
        }

        /// <summary>
        /// Turns the current vector into a unit vector. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        public void Normalize()
        {
            float num = 1f / (float)Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z);
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
        }

        /// <summary>
        /// Creates a unit vector from the specified vector. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        /// <param name="value">The source Vector3.</param>
        public static Vector3 Normalize(Vector3 value)
        {
            float num = 1f / (float)Math.Sqrt((double)value.X * (double)value.X + (double)value.Y * (double)value.Y + (double)value.Z * (double)value.Z);
            Vector3 vector3;
            vector3.X = value.X * num;
            vector3.Y = value.Y * num;
            vector3.Z = value.Z * num;
            return vector3;
        }

        /// <summary>
        /// Creates a unit vector from the specified vector, writing the result to a user-specified variable. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="result">[OutAttribute] The normalized vector.</param>
        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float num = 1f / (float)Math.Sqrt((double)value.X * (double)value.X + (double)value.Y * (double)value.Y + (double)value.Z * (double)value.Z);
            result.X = value.X * num;
            result.Y = value.Y * num;
            result.Z = value.Z * num;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="vector2">Source vector.</param>
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3 vector3;
            vector3.X = (float)((double)vector1.Y * (double)vector2.Z - (double)vector1.Z * (double)vector2.Y);
            vector3.Y = (float)((double)vector1.Z * (double)vector2.X - (double)vector1.X * (double)vector2.Z);
            vector3.Z = (float)((double)vector1.X * (double)vector2.Y - (double)vector1.Y * (double)vector2.X);
            return vector3;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="vector2">Source vector.</param><param name="result">[OutAttribute] The cross product of the vectors.</param>
        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            float num1 = (float)((double)vector1.Y * (double)vector2.Z - (double)vector1.Z * (double)vector2.Y);
            float num2 = (float)((double)vector1.Z * (double)vector2.X - (double)vector1.X * (double)vector2.Z);
            float num3 = (float)((double)vector1.X * (double)vector2.Y - (double)vector1.Y * (double)vector2.X);
            result.X = num1;
            result.Y = num2;
            result.Z = num3;
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.  Reference page contains code sample.
        /// </summary>
        /// <param name="vector">Source vector.</param><param name="normal">Normal of the surface.</param>
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            float num = (float)((double)vector.X * (double)normal.X + (double)vector.Y * (double)normal.Y + (double)vector.Z * (double)normal.Z);
            Vector3 vector3;
            vector3.X = vector.X - 2f * num * normal.X;
            vector3.Y = vector.Y - 2f * num * normal.Y;
            vector3.Z = vector.Z - 2f * num * normal.Z;
            return vector3;
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.  Reference page contains code sample.
        /// </summary>
        /// <param name="vector">Source vector.</param><param name="normal">Normal of the surface.</param><param name="result">[OutAttribute] The reflected vector.</param>
        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            float num = (float)((double)vector.X * (double)normal.X + (double)vector.Y * (double)normal.Y + (double)vector.Z * (double)normal.Z);
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
        }

        /// <summary>
        /// Returns a vector that contains the lowest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = (double)value1.X < (double)value2.X ? value1.X : value2.X;
            vector3.Y = (double)value1.Y < (double)value2.Y ? value1.Y : value2.Y;
            vector3.Z = (double)value1.Z < (double)value2.Z ? value1.Z : value2.Z;
            return vector3;
        }

        /// <summary>
        /// Returns a vector that contains the lowest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The minimized vector.</param>
        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = (double)value1.X < (double)value2.X ? value1.X : value2.X;
            result.Y = (double)value1.Y < (double)value2.Y ? value1.Y : value2.Y;
            result.Z = (double)value1.Z < (double)value2.Z ? value1.Z : value2.Z;
        }

        /// <summary>
        /// Returns a vector that contains the highest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = (double)value1.X > (double)value2.X ? value1.X : value2.X;
            vector3.Y = (double)value1.Y > (double)value2.Y ? value1.Y : value2.Y;
            vector3.Z = (double)value1.Z > (double)value2.Z ? value1.Z : value2.Z;
            return vector3;
        }

        /// <summary>
        /// Returns a vector that contains the highest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The maximized vector.</param>
        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = (double)value1.X > (double)value2.X ? value1.X : value2.X;
            result.Y = (double)value1.Y > (double)value2.Y ? value1.Y : value2.Y;
            result.Z = (double)value1.Z > (double)value2.Z ? value1.Z : value2.Z;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value1">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param>
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
        {
            float num1 = value1.X;
            float num2 = (double)num1 > (double)max.X ? max.X : num1;
            float num3 = (double)num2 < (double)min.X ? min.X : num2;
            float num4 = value1.Y;
            float num5 = (double)num4 > (double)max.Y ? max.Y : num4;
            float num6 = (double)num5 < (double)min.Y ? min.Y : num5;
            float num7 = value1.Z;
            float num8 = (double)num7 > (double)max.Z ? max.Z : num7;
            float num9 = (double)num8 < (double)min.Z ? min.Z : num8;
            Vector3 vector3;
            vector3.X = num3;
            vector3.Y = num6;
            vector3.Z = num9;
            return vector3;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value1">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param><param name="result">[OutAttribute] The clamped value.</param>
        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            float num1 = value1.X;
            float num2 = (double)num1 > (double)max.X ? max.X : num1;
            float num3 = (double)num2 < (double)min.X ? min.X : num2;
            float num4 = value1.Y;
            float num5 = (double)num4 > (double)max.Y ? max.Y : num4;
            float num6 = (double)num5 < (double)min.Y ? min.Y : num5;
            float num7 = value1.Z;
            float num8 = (double)num7 > (double)max.Z ? max.Z : num7;
            float num9 = (double)num8 < (double)min.Z ? min.Z : num8;
            result.X = num3;
            result.Y = num6;
            result.Z = num9;
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            Vector3 vector3;
            vector3.X = value1.X + (value2.X - value1.X) * amount;
            vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
            vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return vector3;
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param><param name="result">[OutAttribute] The result of the interpolation.</param>
        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        /// <summary>
        /// Returns a Vector3 containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A Vector3 containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param><param name="value2">A Vector3 containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param><param name="value3">A Vector3 containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param><param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param><param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param>
        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
        {
            Vector3 vector3;
            vector3.X = (float)((double)value1.X + (double)amount1 * ((double)value2.X - (double)value1.X) + (double)amount2 * ((double)value3.X - (double)value1.X));
            vector3.Y = (float)((double)value1.Y + (double)amount1 * ((double)value2.Y - (double)value1.Y) + (double)amount2 * ((double)value3.Y - (double)value1.Y));
            vector3.Z = (float)((double)value1.Z + (double)amount1 * ((double)value2.Z - (double)value1.Z) + (double)amount2 * ((double)value3.Z - (double)value1.Z));
            return vector3;
        }

        /// <summary>
        /// Returns a Vector3 containing the 3D Cartesian coordinates of a point specified in barycentric (areal) coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A Vector3 containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param><param name="value2">A Vector3 containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param><param name="value3">A Vector3 containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param><param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param><param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param><param name="result">[OutAttribute] The 3D Cartesian coordinates of the specified point are placed in this Vector3 on exit.</param>
        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
        {
            result.X = (float)((double)value1.X + (double)amount1 * ((double)value2.X - (double)value1.X) + (double)amount2 * ((double)value3.X - (double)value1.X));
            result.Y = (float)((double)value1.Y + (double)amount1 * ((double)value2.Y - (double)value1.Y) + (double)amount2 * ((double)value3.Y - (double)value1.Y));
            result.Z = (float)((double)value1.Z + (double)amount1 * ((double)value2.Z - (double)value1.Z) + (double)amount2 * ((double)value3.Z - (double)value1.Z));
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Weighting value.</param>
        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
        {
            amount = (double)amount > 1.0 ? 1f : ((double)amount < 0.0 ? 0.0f : amount);
            amount = (float)((double)amount * (double)amount * (3.0 - 2.0 * (double)amount));
            Vector3 vector3;
            vector3.X = value1.X + (value2.X - value1.X) * amount;
            vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
            vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return vector3;
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Weighting value.</param><param name="result">[OutAttribute] The interpolated value.</param>
        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            amount = (double)amount > 1.0 ? 1f : ((double)amount < 0.0 ? 0.0f : amount);
            amount = (float)((double)amount * (double)amount * (3.0 - 2.0 * (double)amount));
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param>
        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            Vector3 vector3;
            vector3.X = (float)(0.5 * (2.0 * (double)value2.X + (-(double)value1.X + (double)value3.X) * (double)amount + (2.0 * (double)value1.X - 5.0 * (double)value2.X + 4.0 * (double)value3.X - (double)value4.X) * (double)num1 + (-(double)value1.X + 3.0 * (double)value2.X - 3.0 * (double)value3.X + (double)value4.X) * (double)num2));
            vector3.Y = (float)(0.5 * (2.0 * (double)value2.Y + (-(double)value1.Y + (double)value3.Y) * (double)amount + (2.0 * (double)value1.Y - 5.0 * (double)value2.Y + 4.0 * (double)value3.Y - (double)value4.Y) * (double)num1 + (-(double)value1.Y + 3.0 * (double)value2.Y - 3.0 * (double)value3.Y + (double)value4.Y) * (double)num2));
            vector3.Z = (float)(0.5 * (2.0 * (double)value2.Z + (-(double)value1.Z + (double)value3.Z) * (double)amount + (2.0 * (double)value1.Z - 5.0 * (double)value2.Z + 4.0 * (double)value3.Z - (double)value4.Z) * (double)num1 + (-(double)value1.Z + 3.0 * (double)value2.Z - 3.0 * (double)value3.Z + (double)value4.Z) * (double)num2));
            return vector3;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param><param name="result">[OutAttribute] A vector that is the result of the Catmull-Rom interpolation.</param>
        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            result.X = (float)(0.5 * (2.0 * (double)value2.X + (-(double)value1.X + (double)value3.X) * (double)amount + (2.0 * (double)value1.X - 5.0 * (double)value2.X + 4.0 * (double)value3.X - (double)value4.X) * (double)num1 + (-(double)value1.X + 3.0 * (double)value2.X - 3.0 * (double)value3.X + (double)value4.X) * (double)num2));
            result.Y = (float)(0.5 * (2.0 * (double)value2.Y + (-(double)value1.Y + (double)value3.Y) * (double)amount + (2.0 * (double)value1.Y - 5.0 * (double)value2.Y + 4.0 * (double)value3.Y - (double)value4.Y) * (double)num1 + (-(double)value1.Y + 3.0 * (double)value2.Y - 3.0 * (double)value3.Y + (double)value4.Y) * (double)num2));
            result.Z = (float)(0.5 * (2.0 * (double)value2.Z + (-(double)value1.Z + (double)value3.Z) * (double)amount + (2.0 * (double)value1.Z - 5.0 * (double)value2.Z + 4.0 * (double)value3.Z - (double)value4.Z) * (double)num1 + (-(double)value1.Z + 3.0 * (double)value2.Z - 3.0 * (double)value3.Z + (double)value4.Z) * (double)num2));
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position vector.</param><param name="tangent1">Source tangent vector.</param><param name="value2">Source position vector.</param><param name="tangent2">Source tangent vector.</param><param name="amount">Weighting factor.</param>
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = (float)(2.0 * (double)num2 - 3.0 * (double)num1 + 1.0);
            float num4 = (float)(-2.0 * (double)num2 + 3.0 * (double)num1);
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            Vector3 vector3;
            vector3.X = (float)((double)value1.X * (double)num3 + (double)value2.X * (double)num4 + (double)tangent1.X * (double)num5 + (double)tangent2.X * (double)num6);
            vector3.Y = (float)((double)value1.Y * (double)num3 + (double)value2.Y * (double)num4 + (double)tangent1.Y * (double)num5 + (double)tangent2.Y * (double)num6);
            vector3.Z = (float)((double)value1.Z * (double)num3 + (double)value2.Z * (double)num4 + (double)tangent1.Z * (double)num5 + (double)tangent2.Z * (double)num6);
            return vector3;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position vector.</param><param name="tangent1">Source tangent vector.</param><param name="value2">Source position vector.</param><param name="tangent2">Source tangent vector.</param><param name="amount">Weighting factor.</param><param name="result">[OutAttribute] The result of the Hermite spline interpolation.</param>
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = (float)(2.0 * (double)num2 - 3.0 * (double)num1 + 1.0);
            float num4 = (float)(-2.0 * (double)num2 + 3.0 * (double)num1);
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            result.X = (float)((double)value1.X * (double)num3 + (double)value2.X * (double)num4 + (double)tangent1.X * (double)num5 + (double)tangent2.X * (double)num6);
            result.Y = (float)((double)value1.Y * (double)num3 + (double)value2.Y * (double)num4 + (double)tangent1.Y * (double)num5 + (double)tangent2.Y * (double)num6);
            result.Z = (float)((double)value1.Z * (double)num3 + (double)value2.Z * (double)num4 + (double)tangent1.Z * (double)num5 + (double)tangent2.Z * (double)num6);
        }

        /// <summary>
        /// Transforms a 3D vector by the given matrix.
        /// </summary>
        /// <param name="position">The source vector.</param><param name="matrix">The transformation matrix.</param>
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
#if NATIVE_SUPPORT
            Vector3 vector3;
            Transform_Native(ref position, ref matrix, out vector3);
            return vector3;
#else
            float num1 = (float)((double)position.X * (double)matrix.M11 + (double)position.Y * (double)matrix.M21 + (double)position.Z * (double)matrix.M31) + matrix.M41;
            float num2 = (float)((double)position.X * (double)matrix.M12 + (double)position.Y * (double)matrix.M22 + (double)position.Z * (double)matrix.M32) + matrix.M42;
            float num3 = (float)((double)position.X * (double)matrix.M13 + (double)position.Y * (double)matrix.M23 + (double)position.Z * (double)matrix.M33) + matrix.M43;
            Vector3 vector3;
            vector3.X = num1;
            vector3.Y = num2;
            vector3.Z = num3;
            return vector3;
#endif
        }

        /// <summary>
        /// Transforms a Vector3 by the given Matrix.
        /// </summary>
        /// <param name="position">The source Vector3.</param><param name="matrix">The transformation Matrix.</param><param name="result">[OutAttribute] The transformed vector.</param>
        public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result)
        {
#if NATIVE_SUPPORT
            Transform_Native(ref position, ref matrix, out result);
#else
            float num1 = (float)((double)position.X * (double)matrix.M11 + (double)position.Y * (double)matrix.M21 + (double)position.Z * (double)matrix.M31) + matrix.M41;
            float num2 = (float)((double)position.X * (double)matrix.M12 + (double)position.Y * (double)matrix.M22 + (double)position.Z * (double)matrix.M32) + matrix.M42;
            float num3 = (float)((double)position.X * (double)matrix.M13 + (double)position.Y * (double)matrix.M23 + (double)position.Z * (double)matrix.M33) + matrix.M43;
            result.X = num1;
            result.Y = num2;
            result.Z = num3;
#endif
        }




        // Transform (x, y, z, 1) by matrix, project result back into w=1.
        //D3DXVECTOR3* WINAPI D3DXVec3TransformCoord
        //  ( D3DXVECTOR3 *pOut, CONST D3DXVECTOR3 *pV, CONST D3DXMATRIX *pM );

        /// <summary>Native Interop Function</summary>
        [DllImport("d3dx9_43.dll", EntryPoint = "D3DXVec3TransformCoord", CallingConvention = CallingConvention.StdCall, PreserveSig = true), SuppressUnmanagedCodeSecurityAttribute]
        private unsafe extern static Vector3* D3DXVec3TransformCoord_([Out] Vector3* pOut, [In] Vector3* pV,[In] Matrix* pM);

        public static void Transform_Native(ref Vector3 position, ref Matrix matrix, out Vector3 result)
        {
            unsafe
            {
                fixed (Vector3* resultRef_ = &result)
                fixed (Vector3* posRef_ = &position)
                fixed (Matrix* matRef_ = &matrix)

                    D3DXVec3TransformCoord_(resultRef_, posRef_, matRef_);
            }
        }




        /// <summary>
        /// Transforms a 3D vector normal by a matrix.
        /// </summary>
        /// <param name="normal">The source vector.</param><param name="matrix">The transformation matrix.</param>
        public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
        {
            float num1 = (float)((double)normal.X * (double)matrix.M11 + (double)normal.Y * (double)matrix.M21 + (double)normal.Z * (double)matrix.M31);
            float num2 = (float)((double)normal.X * (double)matrix.M12 + (double)normal.Y * (double)matrix.M22 + (double)normal.Z * (double)matrix.M32);
            float num3 = (float)((double)normal.X * (double)matrix.M13 + (double)normal.Y * (double)matrix.M23 + (double)normal.Z * (double)matrix.M33);
            Vector3 vector3;
            vector3.X = num1;
            vector3.Y = num2;
            vector3.Z = num3;
            return vector3;
        }

        /// <summary>
        /// Transforms a vector normal by a matrix.
        /// </summary>
        /// <param name="normal">The source vector.</param><param name="matrix">The transformation Matrix.</param><param name="result">[OutAttribute] The Vector3 resulting from the transformation.</param>
        public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result)
        {
            float num1 = (float)((double)normal.X * (double)matrix.M11 + (double)normal.Y * (double)matrix.M21 + (double)normal.Z * (double)matrix.M31);
            float num2 = (float)((double)normal.X * (double)matrix.M12 + (double)normal.Y * (double)matrix.M22 + (double)normal.Z * (double)matrix.M32);
            float num3 = (float)((double)normal.X * (double)matrix.M13 + (double)normal.Y * (double)matrix.M23 + (double)normal.Z * (double)matrix.M33);
            result.X = num1;
            result.Y = num2;
            result.Z = num3;
        }

        /// <summary>
        /// Transforms a Vector3 by a specified Quaternion rotation.
        /// </summary>
        /// <param name="value">The Vector3 to rotate.</param><param name="rotation">The Quaternion rotation to apply.</param>
        public static Vector3 Transform(Vector3 value, Quaternion rotation)
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
            float num13 = (float)((double)value.X * (1.0 - (double)num10 - (double)num12) + (double)value.Y * ((double)num8 - (double)num6) + (double)value.Z * ((double)num9 + (double)num5));
            float num14 = (float)((double)value.X * ((double)num8 + (double)num6) + (double)value.Y * (1.0 - (double)num7 - (double)num12) + (double)value.Z * ((double)num11 - (double)num4));
            float num15 = (float)((double)value.X * ((double)num9 - (double)num5) + (double)value.Y * ((double)num11 + (double)num4) + (double)value.Z * (1.0 - (double)num7 - (double)num10));
            Vector3 vector3;
            vector3.X = num13;
            vector3.Y = num14;
            vector3.Z = num15;
            return vector3;
        }

        /// <summary>
        /// Transforms a Vector3 by a specified Quaternion rotation.
        /// </summary>
        /// <param name="value">The Vector3 to rotate.</param><param name="rotation">The Quaternion rotation to apply.</param><param name="result">[OutAttribute] An existing Vector3 filled in with the results of the rotation.</param>
        public static void Transform(ref Vector3 value, ref Quaternion rotation, out Vector3 result)
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
            float num13 = (float)((double)value.X * (1.0 - (double)num10 - (double)num12) + (double)value.Y * ((double)num8 - (double)num6) + (double)value.Z * ((double)num9 + (double)num5));
            float num14 = (float)((double)value.X * ((double)num8 + (double)num6) + (double)value.Y * (1.0 - (double)num7 - (double)num12) + (double)value.Z * ((double)num11 - (double)num4));
            float num15 = (float)((double)value.X * ((double)num9 - (double)num5) + (double)value.Y * ((double)num11 + (double)num4) + (double)value.Z * (1.0 - (double)num7 - (double)num10));
            result.X = num13;
            result.Y = num14;
            result.Z = num15;
        }

        /// <summary>
        /// Transforms a source array of Vector3s by a specified Matrix and writes the results to an existing destination array.
        /// </summary>
        /// <param name="sourceArray">The source array.</param><param name="matrix">The transform Matrix to apply.</param><param name="destinationArray">An existing destination array into which the transformed Vector3s are written.</param>
        public static void Transform(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
        {
            for (int index = 0; index < sourceArray.Length; ++index)
            {
                float num1 = sourceArray[index].X;
                float num2 = sourceArray[index].Y;
                float num3 = sourceArray[index].Z;
                destinationArray[index].X = (float)((double)num1 * (double)matrix.M11 + (double)num2 * (double)matrix.M21 + (double)num3 * (double)matrix.M31) + matrix.M41;
                destinationArray[index].Y = (float)((double)num1 * (double)matrix.M12 + (double)num2 * (double)matrix.M22 + (double)num3 * (double)matrix.M32) + matrix.M42;
                destinationArray[index].Z = (float)((double)num1 * (double)matrix.M13 + (double)num2 * (double)matrix.M23 + (double)num3 * (double)matrix.M33) + matrix.M43;
            }
        }

        /// <summary>
        /// Applies a specified transform Matrix to a specified range of an array of Vector3s and writes the results into a specified range of a destination array.
        /// </summary>
        /// <param name="sourceArray">The source array.</param><param name="sourceIndex">The index in the source array at which to start.</param><param name="matrix">The transform Matrix to apply.</param><param name="destinationArray">The existing destination array.</param><param name="destinationIndex">The index in the destination array at which to start.</param><param name="length">The number of Vector3s to transform.</param>
        public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3[] destinationArray, int destinationIndex, int length)
        {
            for (; length > 0; --length)
            {
                float num1 = sourceArray[sourceIndex].X;
                float num2 = sourceArray[sourceIndex].Y;
                float num3 = sourceArray[sourceIndex].Z;
                destinationArray[destinationIndex].X = (float)((double)num1 * (double)matrix.M11 + (double)num2 * (double)matrix.M21 + (double)num3 * (double)matrix.M31) + matrix.M41;
                destinationArray[destinationIndex].Y = (float)((double)num1 * (double)matrix.M12 + (double)num2 * (double)matrix.M22 + (double)num3 * (double)matrix.M32) + matrix.M42;
                destinationArray[destinationIndex].Z = (float)((double)num1 * (double)matrix.M13 + (double)num2 * (double)matrix.M23 + (double)num3 * (double)matrix.M33) + matrix.M43;
                ++sourceIndex;
                ++destinationIndex;
            }
        }

        /// <summary>
        /// Transforms an array of 3D vector normals by a specified Matrix.
        /// </summary>
        /// <param name="sourceArray">The array of Vector3 normals to transform.</param><param name="matrix">The transform matrix to apply.</param><param name="destinationArray">An existing Vector3 array into which the results of the transforms are written.</param>
        public static void TransformNormal(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
        {
            for (int index = 0; index < sourceArray.Length; ++index)
            {
                float num1 = sourceArray[index].X;
                float num2 = sourceArray[index].Y;
                float num3 = sourceArray[index].Z;
                destinationArray[index].X = (float)((double)num1 * (double)matrix.M11 + (double)num2 * (double)matrix.M21 + (double)num3 * (double)matrix.M31);
                destinationArray[index].Y = (float)((double)num1 * (double)matrix.M12 + (double)num2 * (double)matrix.M22 + (double)num3 * (double)matrix.M32);
                destinationArray[index].Z = (float)((double)num1 * (double)matrix.M13 + (double)num2 * (double)matrix.M23 + (double)num3 * (double)matrix.M33);
            }
        }

        /// <summary>
        /// Transforms a specified range in an array of 3D vector normals by a specified Matrix and writes the results to a specified range in a destination array.
        /// </summary>
        /// <param name="sourceArray">The source array of Vector3 normals.</param><param name="sourceIndex">The starting index in the source array.</param><param name="matrix">The transform Matrix to apply.</param><param name="destinationArray">The destination Vector3 array.</param><param name="destinationIndex">The starting index in the destination array.</param><param name="length">The number of vectors to transform.</param>
        public static void TransformNormal(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3[] destinationArray, int destinationIndex, int length)
        {
            for (; length > 0; --length)
            {
                float num1 = sourceArray[sourceIndex].X;
                float num2 = sourceArray[sourceIndex].Y;
                float num3 = sourceArray[sourceIndex].Z;
                destinationArray[destinationIndex].X = (float)((double)num1 * (double)matrix.M11 + (double)num2 * (double)matrix.M21 + (double)num3 * (double)matrix.M31);
                destinationArray[destinationIndex].Y = (float)((double)num1 * (double)matrix.M12 + (double)num2 * (double)matrix.M22 + (double)num3 * (double)matrix.M32);
                destinationArray[destinationIndex].Z = (float)((double)num1 * (double)matrix.M13 + (double)num2 * (double)matrix.M23 + (double)num3 * (double)matrix.M33);
                ++sourceIndex;
                ++destinationIndex;
            }
        }

        /// <summary>
        /// Transforms a source array of Vector3s by a specified Quaternion rotation and writes the results to an existing destination array.
        /// </summary>
        /// <param name="sourceArray">The source array.</param><param name="rotation">The Quaternion rotation to apply.</param><param name="destinationArray">An existing destination array into which the transformed Vector3s are written.</param>
        public static void Transform(Vector3[] sourceArray, ref Quaternion rotation, Vector3[] destinationArray)
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
            for (int index = 0; index < sourceArray.Length; ++index)
            {
                float num22 = sourceArray[index].X;
                float num23 = sourceArray[index].Y;
                float num24 = sourceArray[index].Z;
                destinationArray[index].X = (float)((double)num22 * (double)num13 + (double)num23 * (double)num14 + (double)num24 * (double)num15);
                destinationArray[index].Y = (float)((double)num22 * (double)num16 + (double)num23 * (double)num17 + (double)num24 * (double)num18);
                destinationArray[index].Z = (float)((double)num22 * (double)num19 + (double)num23 * (double)num20 + (double)num24 * (double)num21);
            }
        }

        /// <summary>
        /// Applies a specified Quaternion rotation to a specified range of an array of Vector3s and writes the results into a specified range of a destination array.
        /// </summary>
        /// <param name="sourceArray">The source array.</param><param name="sourceIndex">The index in the source array at which to start.</param><param name="rotation">The Quaternion rotation to apply.</param><param name="destinationArray">The existing destination array.</param><param name="destinationIndex">The index in the destination array at which to start.</param><param name="length">The number of Vector3s to transform.</param>
        public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Quaternion rotation, Vector3[] destinationArray, int destinationIndex, int length)
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
            for (; length > 0; --length)
            {
                float num22 = sourceArray[sourceIndex].X;
                float num23 = sourceArray[sourceIndex].Y;
                float num24 = sourceArray[sourceIndex].Z;
                destinationArray[destinationIndex].X = (float)((double)num22 * (double)num13 + (double)num23 * (double)num14 + (double)num24 * (double)num15);
                destinationArray[destinationIndex].Y = (float)((double)num22 * (double)num16 + (double)num23 * (double)num17 + (double)num24 * (double)num18);
                destinationArray[destinationIndex].Z = (float)((double)num22 * (double)num19 + (double)num23 * (double)num20 + (double)num24 * (double)num21);
                ++sourceIndex;
                ++destinationIndex;
            }
        }

        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param>
        public static Vector3 Negate(Vector3 value)
        {
            Vector3 vector3;
            vector3.X = -value.X;
            vector3.Y = -value.Y;
            vector3.Z = -value.Z;
            return vector3;
        }

        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="result">[OutAttribute] Vector pointing in the opposite direction.</param>
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X + value2.X;
            vector3.Y = value1.Y + value2.Y;
            vector3.Z = value1.Z + value2.Z;
            return vector3;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] Sum of the source vectors.</param>
        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 Subtract(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X - value2.X;
            vector3.Y = value1.Y - value2.Y;
            vector3.Z = value1.Z - value2.Z;
            return vector3;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The result of the subtraction.</param>
        public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3 Multiply(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X * value2.X;
            vector3.Y = value1.Y * value2.Y;
            vector3.Z = value1.Z * value2.Z;
            return vector3;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="scaleFactor">Scalar value.</param>
        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            Vector3 vector3;
            vector3.X = value1.X * scaleFactor;
            vector3.Y = value1.Y * scaleFactor;
            vector3.Z = value1.Z * scaleFactor;
            return vector3;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="scaleFactor">Scalar value.</param><param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Divisor vector.</param>
        public static Vector3 Divide(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.X = value1.X / value2.X;
            vector3.Y = value1.Y / value2.Y;
            vector3.Z = value1.Z / value2.Z;
            return vector3;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param><param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param>
        public static Vector3 Divide(Vector3 value1, float value2)
        {
            float num = 1f / value2;
            Vector3 vector3;
            vector3.X = value1.X * num;
            vector3.Y = value1.Y * num;
            vector3.Z = value1.Z * num;
            return vector3;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param><param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector3 value1, float value2, out Vector3 result)
        {
            float num = 1f / value2;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace MinerWarsMath
{
    /// <summary>
    /// Defines an axis-aligned box-shaped 3D volume.
    /// </summary>
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        /// <summary>
        /// Specifies the total number of corners (8) in the BoundingBox.
        /// </summary>
        public const int CornerCount = 8;
        /// <summary>
        /// The minimum point the BoundingBox contains.
        /// </summary>
        public Vector3 Min;
        /// <summary>
        /// The maximum point the BoundingBox contains.
        /// </summary>
        public Vector3 Max;

        /// <summary>
        /// Creates an instance of BoundingBox.
        /// </summary>
        /// <param name="min">The minimum point the BoundingBox includes.</param><param name="max">The maximum point the BoundingBox includes.</param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Determines whether two instances of BoundingBox are equal.
        /// </summary>
        /// <param name="a">BoundingBox to compare.</param><param name="b">BoundingBox to compare.</param>
        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two instances of BoundingBox are not equal.
        /// </summary>
        /// <param name="a">The object to the left of the inequality operator.</param><param name="b">The object to the right of the inequality operator.</param>
        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            if (!(a.Min != b.Min))
                return a.Max != b.Max;
            else
                return true;
        }

        /// <summary>
        /// Gets an array of points that make up the corners of the BoundingBox.
        /// </summary>
        public Vector3[] GetCorners()
        {
            return new Vector3[8]
      {
        new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
        new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
        new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
        new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
        new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
        new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
        new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
        new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
      };
        }

        /// <summary>
        /// Gets the array of points that make up the corners of the BoundingBox.
        /// </summary>
        /// <param name="corners">An existing array of at least 8 Vector3 points where the corners of the BoundingBox are written.</param>
        public void GetCorners(Vector3[] corners)
        {
            corners[0].X = this.Min.X;
            corners[0].Y = this.Max.Y;
            corners[0].Z = this.Max.Z;
            corners[1].X = this.Max.X;
            corners[1].Y = this.Max.Y;
            corners[1].Z = this.Max.Z;
            corners[2].X = this.Max.X;
            corners[2].Y = this.Min.Y;
            corners[2].Z = this.Max.Z;
            corners[3].X = this.Min.X;
            corners[3].Y = this.Min.Y;
            corners[3].Z = this.Max.Z;
            corners[4].X = this.Min.X;
            corners[4].Y = this.Max.Y;
            corners[4].Z = this.Min.Z;
            corners[5].X = this.Max.X;
            corners[5].Y = this.Max.Y;
            corners[5].Z = this.Min.Z;
            corners[6].X = this.Max.X;
            corners[6].Y = this.Min.Y;
            corners[6].Z = this.Min.Z;
            corners[7].X = this.Min.X;
            corners[7].Y = this.Min.Y;
            corners[7].Z = this.Min.Z;
        }

        /// <summary>
        /// Determines whether two instances of BoundingBox are equal.
        /// </summary>
        /// <param name="other">The BoundingBox to compare with the current BoundingBox.</param>
        public bool Equals(BoundingBox other)
        {
            if (this.Min == other.Min)
                return this.Max == other.Max;
            else
                return false;
        }

        /// <summary>
        /// Determines whether two instances of BoundingBox are equal.
        /// </summary>
        /// <param name="obj">The Object to compare with the current BoundingBox.</param>
        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is BoundingBox)
                flag = this.Equals((BoundingBox)obj);
            return flag;
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

        /// <summary>
        /// Returns a String that represents the current BoundingBox.
        /// </summary>
        public override string ToString()
        {
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{{Min:{0} Max:{1}}}", new object[2]
      {
        (object) this.Min.ToString(),
        (object) this.Max.ToString()
      });
        }

        /// <summary>
        /// Creates the smallest BoundingBox that contains the two specified BoundingBox instances.
        /// </summary>
        /// <param name="original">One of the BoundingBoxs to contain.</param><param name="additional">One of the BoundingBoxs to contain.</param>
        public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
        {
            BoundingBox boundingBox;
            Vector3.Min(ref original.Min, ref additional.Min, out boundingBox.Min);
            Vector3.Max(ref original.Max, ref additional.Max, out boundingBox.Max);
            return boundingBox;
        }

        /// <summary>
        /// Creates the smallest BoundingBox that contains the two specified BoundingBox instances.
        /// </summary>
        /// <param name="original">One of the BoundingBox instances to contain.</param><param name="additional">One of the BoundingBox instances to contain.</param><param name="result">[OutAttribute] The created BoundingBox.</param>
        public static void CreateMerged(ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
        {
            Vector3 result1;
            Vector3.Min(ref original.Min, ref additional.Min, out result1);
            Vector3 result2;
            Vector3.Max(ref original.Max, ref additional.Max, out result2);
            result.Min = result1;
            result.Max = result2;
        }

        /// <summary>
        /// Creates the smallest BoundingBox that will contain the specified BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to contain.</param>
        public static BoundingBox CreateFromSphere(BoundingSphere sphere)
        {
            BoundingBox boundingBox;
            boundingBox.Min.X = sphere.Center.X - sphere.Radius;
            boundingBox.Min.Y = sphere.Center.Y - sphere.Radius;
            boundingBox.Min.Z = sphere.Center.Z - sphere.Radius;
            boundingBox.Max.X = sphere.Center.X + sphere.Radius;
            boundingBox.Max.Y = sphere.Center.Y + sphere.Radius;
            boundingBox.Max.Z = sphere.Center.Z + sphere.Radius;
            return boundingBox;
        }

        /// <summary>
        /// Creates the smallest BoundingBox that will contain the specified BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to contain.</param><param name="result">[OutAttribute] The created BoundingBox.</param>
        public static void CreateFromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Min.X = sphere.Center.X - sphere.Radius;
            result.Min.Y = sphere.Center.Y - sphere.Radius;
            result.Min.Z = sphere.Center.Z - sphere.Radius;
            result.Max.X = sphere.Center.X + sphere.Radius;
            result.Max.Y = sphere.Center.Y + sphere.Radius;
            result.Max.Z = sphere.Center.Z + sphere.Radius;
        }

        /// <summary>
        /// Creates the smallest BoundingBox that will contain a group of points.
        /// </summary>
        /// <param name="points">A list of points the BoundingBox should contain.</param>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException();
            bool flag = false;
            Vector3 result1 = new Vector3(float.MaxValue);
            Vector3 result2 = new Vector3(float.MinValue);
            foreach (Vector3 vector3 in points)
            {
                Vector3 vec3 = vector3;
                Vector3.Min(ref result1, ref vec3, out result1);
                Vector3.Max(ref result2, ref vec3, out result2);
                flag = true;
            }
            if (!flag)
                throw new ArgumentException();
            else
                return new BoundingBox(result1, result2);
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects another BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to check for intersection with.</param>
        public bool Intersects(BoundingBox box)
        {
            return (double)this.Max.X >= (double)box.Min.X && (double)this.Min.X <= (double)box.Max.X && ((double)this.Max.Y >= (double)box.Min.Y && (double)this.Min.Y <= (double)box.Max.Y) && ((double)this.Max.Z >= (double)box.Min.Z && (double)this.Min.Z <= (double)box.Max.Z);
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects another BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to check for intersection with.</param><param name="result">[OutAttribute] true if the BoundingBox instances intersect; false otherwise.</param>
        public void Intersects(ref BoundingBox box, out bool result)
        {
            result = false;
            if ((double)this.Max.X < (double)box.Min.X || (double)this.Min.X > (double)box.Max.X || ((double)this.Max.Y < (double)box.Min.Y || (double)this.Min.Y > (double)box.Max.Y) || ((double)this.Max.Z < (double)box.Min.Z || (double)this.Min.Z > (double)box.Max.Z))
                return;
            result = true;
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a BoundingFrustum.
        /// </summary>
        /// <param name="frustum">The BoundingFrustum to check for intersection with.</param>
        public bool Intersects(BoundingFrustum frustum)
        {
            if ((BoundingFrustum)null == frustum)
                throw new ArgumentNullException("frustum");
            else
                return frustum.Intersects(this);
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a Plane.
        /// </summary>
        /// <param name="plane">The Plane to check for intersection with.</param>
        public PlaneIntersectionType Intersects(Plane plane)
        {
            Vector3 vector3_1;
            vector3_1.X = (double)plane.Normal.X >= 0.0 ? this.Min.X : this.Max.X;
            vector3_1.Y = (double)plane.Normal.Y >= 0.0 ? this.Min.Y : this.Max.Y;
            vector3_1.Z = (double)plane.Normal.Z >= 0.0 ? this.Min.Z : this.Max.Z;
            Vector3 vector3_2;
            vector3_2.X = (double)plane.Normal.X >= 0.0 ? this.Max.X : this.Min.X;
            vector3_2.Y = (double)plane.Normal.Y >= 0.0 ? this.Max.Y : this.Min.Y;
            vector3_2.Z = (double)plane.Normal.Z >= 0.0 ? this.Max.Z : this.Min.Z;
            if ((double)plane.Normal.X * (double)vector3_1.X + (double)plane.Normal.Y * (double)vector3_1.Y + (double)plane.Normal.Z * (double)vector3_1.Z + (double)plane.D > 0.0)
                return PlaneIntersectionType.Front;
            return (double)plane.Normal.X * (double)vector3_2.X + (double)plane.Normal.Y * (double)vector3_2.Y + (double)plane.Normal.Z * (double)vector3_2.Z + (double)plane.D < 0.0 ? PlaneIntersectionType.Back : PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a Plane.
        /// </summary>
        /// <param name="plane">The Plane to check for intersection with.</param><param name="result">[OutAttribute] An enumeration indicating whether the BoundingBox intersects the Plane.</param>
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            Vector3 vector3_1;
            vector3_1.X = (double)plane.Normal.X >= 0.0 ? this.Min.X : this.Max.X;
            vector3_1.Y = (double)plane.Normal.Y >= 0.0 ? this.Min.Y : this.Max.Y;
            vector3_1.Z = (double)plane.Normal.Z >= 0.0 ? this.Min.Z : this.Max.Z;
            Vector3 vector3_2;
            vector3_2.X = (double)plane.Normal.X >= 0.0 ? this.Max.X : this.Min.X;
            vector3_2.Y = (double)plane.Normal.Y >= 0.0 ? this.Max.Y : this.Min.Y;
            vector3_2.Z = (double)plane.Normal.Z >= 0.0 ? this.Max.Z : this.Min.Z;
            if ((double)plane.Normal.X * (double)vector3_1.X + (double)plane.Normal.Y * (double)vector3_1.Y + (double)plane.Normal.Z * (double)vector3_1.Z + (double)plane.D > 0.0)
                result = PlaneIntersectionType.Front;
            else if ((double)plane.Normal.X * (double)vector3_2.X + (double)plane.Normal.Y * (double)vector3_2.Y + (double)plane.Normal.Z * (double)vector3_2.Z + (double)plane.D < 0.0)
                result = PlaneIntersectionType.Back;
            else
                result = PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a Ray.
        /// </summary>
        /// <param name="ray">The Ray to check for intersection with.</param>
        public float? Intersects(Ray ray)
        {
            float num1 = 0.0f;
            float num2 = float.MaxValue;
            if ((double)Math.Abs(ray.Direction.X) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.X < (double)this.Min.X || (double)ray.Position.X > (double)this.Max.X)
                    return new float?();
            }
            else
            {
                float num3 = 1f / ray.Direction.X;
                float num4 = (this.Min.X - ray.Position.X) * num3;
                float num5 = (this.Max.X - ray.Position.X) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return new float?();
            }
            if ((double)Math.Abs(ray.Direction.Y) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Y < (double)this.Min.Y || (double)ray.Position.Y > (double)this.Max.Y)
                    return new float?();
            }
            else
            {
                float num3 = 1f / ray.Direction.Y;
                float num4 = (this.Min.Y - ray.Position.Y) * num3;
                float num5 = (this.Max.Y - ray.Position.Y) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return new float?();
            }
            if ((double)Math.Abs(ray.Direction.Z) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Z < (double)this.Min.Z || (double)ray.Position.Z > (double)this.Max.Z)
                    return new float?();
            }
            else
            {
                float num3 = 1f / ray.Direction.Z;
                float num4 = (this.Min.Z - ray.Position.Z) * num3;
                float num5 = (this.Max.Z - ray.Position.Z) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                float num7 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num7)
                    return new float?();
            }
            return new float?(num1);
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a Ray.
        /// </summary>
        /// <param name="ray">The Ray to check for intersection with.</param><param name="result">[OutAttribute] Distance at which the ray intersects the BoundingBox, or null if there is no intersection.</param>
        public void Intersects(ref Ray ray, out float? result)
        {
            result = new float?();
            float num1 = 0.0f;
            float num2 = float.MaxValue;
            if ((double)Math.Abs(ray.Direction.X) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.X < (double)this.Min.X || (double)ray.Position.X > (double)this.Max.X)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.X;
                float num4 = (this.Min.X - ray.Position.X) * num3;
                float num5 = (this.Max.X - ray.Position.X) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return;
            }
            if ((double)Math.Abs(ray.Direction.Y) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Y < (double)this.Min.Y || (double)ray.Position.Y > (double)this.Max.Y)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.Y;
                float num4 = (this.Min.Y - ray.Position.Y) * num3;
                float num5 = (this.Max.Y - ray.Position.Y) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return;
            }
            if ((double)Math.Abs(ray.Direction.Z) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Z < (double)this.Min.Z || (double)ray.Position.Z > (double)this.Max.Z)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.Z;
                float num4 = (this.Min.Z - ray.Position.Z) * num3;
                float num5 = (this.Max.Z - ray.Position.Z) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                float num7 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num7)
                    return;
            }
            result = new float?(num1);
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to check for intersection with.</param>
        public bool Intersects(BoundingSphere sphere)
        {
            Vector3 result1;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out result1);
            float result2;
            Vector3.DistanceSquared(ref sphere.Center, ref result1, out result2);
            return (double)result2 <= (double)sphere.Radius * (double)sphere.Radius;
        }

        /// <summary>
        /// Checks whether the current BoundingBox intersects a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to check for intersection with.</param><param name="result">[OutAttribute] true if the BoundingBox and BoundingSphere intersect; false otherwise.</param>
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            Vector3 result1;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out result1);
            float result2;
            Vector3.DistanceSquared(ref sphere.Center, ref result1, out result2);
            result = (double)result2 <= (double)sphere.Radius * (double)sphere.Radius;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains another BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to test for overlap.</param>
        public ContainmentType Contains(BoundingBox box)
        {
            if ((double)this.Max.X < (double)box.Min.X || (double)this.Min.X > (double)box.Max.X || ((double)this.Max.Y < (double)box.Min.Y || (double)this.Min.Y > (double)box.Max.Y) || ((double)this.Max.Z < (double)box.Min.Z || (double)this.Min.Z > (double)box.Max.Z))
                return ContainmentType.Disjoint;
            return (double)this.Min.X > (double)box.Min.X || (double)box.Max.X > (double)this.Max.X || ((double)this.Min.Y > (double)box.Min.Y || (double)box.Max.Y > (double)this.Max.Y) || ((double)this.Min.Z > (double)box.Min.Z || (double)box.Max.Z > (double)this.Max.Z) ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a BoundingBox.
        /// </summary>
        /// <param name="box">The BoundingBox to test for overlap.</param><param name="result">[OutAttribute] Enumeration indicating the extent of overlap.</param>
        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            result = ContainmentType.Disjoint;
            if ((double)this.Max.X < (double)box.Min.X || (double)this.Min.X > (double)box.Max.X || ((double)this.Max.Y < (double)box.Min.Y || (double)this.Min.Y > (double)box.Max.Y) || ((double)this.Max.Z < (double)box.Min.Z || (double)this.Min.Z > (double)box.Max.Z))
                return;
            result = (double)this.Min.X > (double)box.Min.X || (double)box.Max.X > (double)this.Max.X || ((double)this.Min.Y > (double)box.Min.Y || (double)box.Max.Y > (double)this.Max.Y) || ((double)this.Min.Z > (double)box.Min.Z || (double)box.Max.Z > (double)this.Max.Z) ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a BoundingFrustum.
        /// </summary>
        /// <param name="frustum">The BoundingFrustum to test for overlap.</param>
        public ContainmentType Contains(BoundingFrustum frustum)
        {
            if (!frustum.Intersects(this))
                return ContainmentType.Disjoint;
            foreach (Vector3 point in frustum.cornerArray)
            {
                if (this.Contains(point) == ContainmentType.Disjoint)
                    return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a point.
        /// </summary>
        /// <param name="point">The point to test for overlap.</param>
        public ContainmentType Contains(Vector3 point)
        {
            return (double)this.Min.X > (double)point.X || (double)point.X > (double)this.Max.X || ((double)this.Min.Y > (double)point.Y || (double)point.Y > (double)this.Max.Y) || ((double)this.Min.Z > (double)point.Z || (double)point.Z > (double)this.Max.Z) ? ContainmentType.Disjoint : ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a point.
        /// </summary>
        /// <param name="point">The point to test for overlap.</param><param name="result">[OutAttribute] Enumeration indicating the extent of overlap.</param>
        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            result = (double)this.Min.X > (double)point.X || (double)point.X > (double)this.Max.X || ((double)this.Min.Y > (double)point.Y || (double)point.Y > (double)this.Max.Y) || ((double)this.Min.Z > (double)point.Z || (double)point.Z > (double)this.Max.Z) ? ContainmentType.Disjoint : ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to test for overlap.</param>
        public ContainmentType Contains(BoundingSphere sphere)
        {
            Vector3 result1;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out result1);
            float result2;
            Vector3.DistanceSquared(ref sphere.Center, ref result1, out result2);
            float num = sphere.Radius;
            if ((double)result2 > (double)num * (double)num)
                return ContainmentType.Disjoint;
            return (double)this.Min.X + (double)num > (double)sphere.Center.X || (double)sphere.Center.X > (double)this.Max.X - (double)num || ((double)this.Max.X - (double)this.Min.X <= (double)num || (double)this.Min.Y + (double)num > (double)sphere.Center.Y) || ((double)sphere.Center.Y > (double)this.Max.Y - (double)num || (double)this.Max.Y - (double)this.Min.Y <= (double)num || ((double)this.Min.Z + (double)num > (double)sphere.Center.Z || (double)sphere.Center.Z > (double)this.Max.Z - (double)num)) || (double)this.Max.X - (double)this.Min.X <= (double)num ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        /// <summary>
        /// Tests whether the BoundingBox contains a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The BoundingSphere to test for overlap.</param><param name="result">[OutAttribute] Enumeration indicating the extent of overlap.</param>
        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            Vector3 result1;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out result1);
            float result2;
            Vector3.DistanceSquared(ref sphere.Center, ref result1, out result2);
            float num = sphere.Radius;
            if ((double)result2 > (double)num * (double)num)
                result = ContainmentType.Disjoint;
            else
                result = (double)this.Min.X + (double)num > (double)sphere.Center.X || (double)sphere.Center.X > (double)this.Max.X - (double)num || ((double)this.Max.X - (double)this.Min.X <= (double)num || (double)this.Min.Y + (double)num > (double)sphere.Center.Y) || ((double)sphere.Center.Y > (double)this.Max.Y - (double)num || (double)this.Max.Y - (double)this.Min.Y <= (double)num || ((double)this.Min.Z + (double)num > (double)sphere.Center.Z || (double)sphere.Center.Z > (double)this.Max.Z - (double)num)) || (double)this.Max.X - (double)this.Min.X <= (double)num ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            result.X = (double)v.X >= 0.0 ? this.Max.X : this.Min.X;
            result.Y = (double)v.Y >= 0.0 ? this.Max.Y : this.Min.Y;
            result.Z = (double)v.Z >= 0.0 ? this.Max.Z : this.Min.Z;
        }
    }
}

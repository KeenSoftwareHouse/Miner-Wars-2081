using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using SysUtils;
    using System.Runtime.Serialization;

    //  Integer version of Vector2, not yet fully implemented
    public struct MyMwcVector2Int
    {
        public int X;
        public int Y;

        public MyMwcVector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        public static implicit operator Vector2(MyMwcVector2Int intVector)
        {
            return new Vector2(intVector.X, intVector.Y);
        }
    }

    //  Integer version of Vector3, not yet fully implemented
    // This is used in WCF methods
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    [DataContract]
    public struct MyMwcVector3Int : IEquatable<MyMwcVector3Int>
    {
        [DataMember(Name = "X")]
        public int X;

        [DataMember(Name = "Y")]
        public int Y;

        [DataMember(Name = "Z")]
        public int Z;

        public MyMwcVector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public bool Equals(MyMwcVector3Int other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(MyMwcVector3Int)) return false;
            return Equals((MyMwcVector3Int)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X;
                result = (result * 397) ^ Y;
                result = (result * 397) ^ Z;
                return result;
            }
        }

        /// <summary>
        /// Calculates rectangular distance.
        /// It's how many sectors you have to travel to get to other sector from current sector.
        /// </summary>
        public int RectangularDistance(MyMwcVector3Int otherVector)
        {
            return Math.Abs(X - otherVector.X) + Math.Abs(Y - otherVector.Y) + Math.Abs(Z - otherVector.Z);
        }

        public static MyMwcVector3Int operator *(MyMwcVector3Int a, MyMwcVector3Int b)
        {
            return new MyMwcVector3Int(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static bool operator ==(MyMwcVector3Int a, MyMwcVector3Int b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(MyMwcVector3Int a, MyMwcVector3Int b)
        {
            return !(a == b);
        }

        public static Vector3 operator *(MyMwcVector3Int a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator *(Vector3 a, MyMwcVector3Int b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator *(float num, MyMwcVector3Int b)
        {
            return new Vector3(num * b.X, num * b.Y, num * b.Z);
        }

        public static Vector3 operator *(MyMwcVector3Int a, float num)
        {
            return new Vector3(num * a.X, num * a.Y, num * a.Z);
        }

        public static MyMwcVector3Int Min(MyMwcVector3Int value1, MyMwcVector3Int value2)
        {
            MyMwcVector3Int vector3;
            vector3.X = value1.X < value2.X ? value1.X : value2.X;
            vector3.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
            vector3.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
            return vector3;
        }

        public static MyMwcVector3Int Max(MyMwcVector3Int value1, MyMwcVector3Int value2)
        {
            MyMwcVector3Int vector3;
            vector3.X = value1.X > value2.X ? value1.X : value2.X;
            vector3.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
            vector3.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
            return vector3;
        }
    }

    //  Integer version of Vector4, not yet fully implemented
    public struct MyMwcVector4Int
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public MyMwcVector4Int(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z + ", " + W;
        }
    }

    //  Short version of Vector3, not yet fully implemented
    [Serializable]
    public struct MyMwcVector3Short
    {
        public short X;
        public short Y;
        public short Z;

        public MyMwcVector3Short(short x, short y, short z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public static MinerWarsMath.Vector3 operator *(MinerWarsMath.Vector3 vector, MyMwcVector3Short shortVector)
        {
            return shortVector * vector;
        }

        public static MinerWarsMath.Vector3 operator *(MyMwcVector3Short shortVector, MinerWarsMath.Vector3 vector)
        {
            return new MinerWarsMath.Vector3(shortVector.X * vector.X, shortVector.Y * vector.Y, shortVector.Z * vector.Z);
        }
    }

    //  Sbyte version of Vector3, not yet fully implemented
    public struct MyMwcVector3Sbyte
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;

        public MyMwcVector3Sbyte(sbyte x, sbyte y, sbyte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public static MinerWarsMath.Vector3 operator *(MinerWarsMath.Vector3 vector, MyMwcVector3Sbyte shortVector)
        {
            return shortVector * vector;
        }

        public static MinerWarsMath.Vector3 operator *(MyMwcVector3Sbyte shortVector, MinerWarsMath.Vector3 vector)
        {
            return new MinerWarsMath.Vector3(shortVector.X * vector.X, shortVector.Y * vector.Y, shortVector.Z * vector.Z);
        }
    }
}

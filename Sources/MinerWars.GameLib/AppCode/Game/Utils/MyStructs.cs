using System;
using MinerWarsMath;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath.Graphics.PackedVector;
using System.Runtime.InteropServices;
using MinerWars.CommonLIB.AppCode.Import;

//  Place for declaration of various structs, classes and enums

namespace MinerWars.AppCode.Game.Utils
{
    //  2D rectangle defined by floats (that is his difference to XNA's Rectangle who uses ints)
    struct MyRectangle2D
    {
        public Vector2 LeftTop;     //  Coordinate of left/top point
        public Vector2 Size;        //  Width and height

        public MyRectangle2D(Vector2 leftTop, Vector2 size)
        {
            LeftTop = leftTop;
            Size = size;
        }
    }


    class MyAtlasTextureCoordinate
    {
        public Vector2 Offset;
        public Vector2 Size;

        public MyAtlasTextureCoordinate(Vector2 offset, Vector2 size)
        {
            Offset = offset;
            Size = size;
        }
    }

    enum MySpherePlaneIntersectionEnum : byte
    {
        BEHIND,
        FRONT,
        INTERSECTS
    }

    struct MyBox
    {
        public Vector3 Center;
        public Vector3 Size;

        public MyBox(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
    }

    //  Line defined by two vertexes 'from' and 'to', so it has start and end.
    public struct MyLine
    {
        public Vector3 From;
        public Vector3 To;
        public Vector3 Direction;
        public float Length;

        //  IMPORTANT: This bounding box is calculated in constructor, but only if needed. So check if you line was made with "calculateBoundingBox = true".  
        //  Do it with true if you want to use this line on MyGuiScreenGameBase.Static intersection testing.
        public BoundingBox BoundingBox;


        //  IMPORTANT: This struct must be initialized using this constructor, or by filling all four fields. It's because
        //  some code may need length or distance, and if they aren't calculated, we can have problems.
        public MyLine(Vector3 from, Vector3 to, bool calculateBoundingBox = true)
        {
            From = from;
            To = to;
            Direction = MyMwcUtils.Normalize(to - from);
            Vector3.Distance(ref to, ref from, out Length);

            //  Calculate line's bounding box, but only if we know we will need it
            BoundingBox = BoundingBoxHelper.InitialBox;
            if (calculateBoundingBox == true)
            {
                BoundingBoxHelper.AddLine(ref this, ref BoundingBox);
            }
        }
    }

    public struct MyTriangle_Vertexes
    {
        public Vector3 Vertex0;
        public Vector3 Vertex1;
        public Vector3 Vertex2;
    }

    struct MyTriangle_Normals
    {
        public Vector3 Normal0;
        public Vector3 Normal1;
        public Vector3 Normal2;
    }

    struct MyTriangle_Vertex_Normal
    {
        public MyTriangle_Vertexes Vertexes;
        public Vector3 Normal;
    }

    struct MyTriangle_Vertex_Normals
    {
        public MyTriangle_Vertexes Vertexes;
        public MyTriangle_Normals Normals;
        public MyTriangle_Normals Binormals;
        public MyTriangle_Normals Tangents;
    }

    public struct MyPlane
    {
        public Vector3 Point;           //  Point on a plane
        public Vector3 Normal;          //  Normal vector of a plane

        public MyPlane(Vector3 point, Vector3 normal)
        {
            Point = point;
            Normal = normal;
        }

        public MyPlane(ref Vector3 point, ref Vector3 normal)
        {
            Point = point;
            Normal = normal;
        }

        public MyPlane(ref MyTriangle_Vertexes triangle)
        {
            Point = triangle.Vertex0;
            Normal = MyMwcUtils.Normalize(Vector3.Cross((triangle.Vertex1 - triangle.Vertex0), (triangle.Vertex2 - triangle.Vertex0)));
        }

        //	This returns the distance the plane is from the origin (0, 0, 0)
        //	It takes the normal to the plane, along with ANY point that lies on the plane (any corner)
        public float GetPlaneDistance()
        {
            //	Use the plane equation to find the distance (Ax + By + Cz + D = 0)  We want to find D.
            //	So, we come up with D = -(Ax + By + Cz)

            //	Basically, the negated dot product of the normal of the plane and the point. (More about the dot product in another tutorial)
            return -((Normal.X * Point.X) + (Normal.Y * Point.Y) + (Normal.Z * Point.Z));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MyVoxelTriangle
    {
        public short VertexIndex0;
        public short VertexIndex1;
        public short VertexIndex2;

        public short this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return VertexIndex0;
                    case 1:
                        return VertexIndex1;
                    case 2:
                        return VertexIndex2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }

            set
            {
                switch (i)
                {
                    case 0:
                        VertexIndex0 = value;
                        break;
                    case 1:
                        VertexIndex1 = value;
                        break;
                    case 2:
                        VertexIndex2 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MyVoxelVertex : IEquatable<MyVoxelVertex>
	{
#if !PACKED_VERTEX_FORMAT

        public Vector3 Position;
        public Vector3 Normal;
		public float Ambient;
		public MyMwcVoxelMaterialsEnum Material;

#else //PACKED_VERTEX_FORMAT

        //12 + 4 + 4 + 1 = 21
        /*
		public Vector3 Position;
		Byte4 _Normal;
		public float Ambient;
		public MyMwcVoxelMaterialsEnum Material;

		public Byte4 PackedNormal
		{
			get { return _Normal; }
			set { _Normal = value; }
		}

		public Vector3 Normal
		{
            get { return MyVertexFormats.VF_Packer.UnpackNormal(ref _Normal); }
            set { _Normal.PackedValue = MyVertexFormats.VF_Packer.PackNormal(ref value); }
		}
          */

        //8 + 4 = 12
        public MyShort4 m_positionAndAmbient;
        public Byte4 m_normal;

        public MyMwcVoxelMaterialsEnum Material;
        //public bool OnRenderCellEdge;


        public Vector3 Position
        {
            get { return MyVertexCompression.INV_VOXEL_MULTIPLIER * new Vector3(m_positionAndAmbient.X + MyVertexCompression.VOXEL_OFFSET, m_positionAndAmbient.Y + MyVertexCompression.VOXEL_OFFSET, m_positionAndAmbient.Z + MyVertexCompression.VOXEL_OFFSET); }
            set
            {
                m_positionAndAmbient.X = (short)(MyVertexCompression.VOXEL_MULTIPLIER * value.X - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);
                m_positionAndAmbient.Y = (short)(MyVertexCompression.VOXEL_MULTIPLIER * value.Y - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);
                m_positionAndAmbient.Z = (short)(MyVertexCompression.VOXEL_MULTIPLIER * value.Z - MyVertexCompression.VOXEL_OFFSET + MyVertexCompression.VOXEL_COORD_EPSILON);
            }
        }

        public float Ambient
        {
            get { return m_positionAndAmbient.W * MyVertexCompression.INV_AMBIENT_MULTIPLIER; }
            set { m_positionAndAmbient.W = (short)(value * MyVertexCompression.AMBIENT_MULTIPLIER); }
        }

        public MyShort4 PackedPositionAndAmbient
        {
            get { return m_positionAndAmbient; }
            set { m_positionAndAmbient = value; }
        }

        public Byte4 PackedNormal
        {
            get { return m_normal; }
            set { m_normal = value; }
        }

        public Vector3 Normal
        {
            get { return VF_Packer.UnpackNormal(ref m_normal); }
            set { m_normal.PackedValue = VF_Packer.PackNormal(ref value); }
        }
#endif //PACKED_VERTEX_FORMAT

        public bool Equals(MyVoxelVertex other)
        {
            return Vector3.DistanceSquared(Position, other.Position) < MyMwcMathConstants.EPSILON_SQUARED;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
	}

    //  Quad made from 4 points
    public struct MyQuad
    {
        public Vector3 Point0;
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;
    }

    struct MyPolyLine
    {
        public Vector3 LineDirectionNormalized;     //  Vector from point 0 to 1, calculated as point1 - point0, than normalized
        public Vector3 Point0;
        public Vector3 Point1;
        public float Thickness;
    }

    //  Structure for holding voxel triangleVertexes used in JLX's collision-detection
    // size is 100 B
    struct MyColDetVoxelTriangle
    {
        public Vector3 Vertex0;
        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public MinerWarsMath.Plane Plane;
        Vector3 m_origin;
        Vector3 m_edge0;
        Vector3 m_edge1;
        Vector3 m_normal;

        //  Points specified so that pt1-pt0 is edge0 and p2-pt0 is edge1
        public void Update(ref Vector3 vertex0, ref Vector3 vertex1, ref Vector3 vertex2)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;

            Plane = new MinerWarsMath.Plane(Vertex0, Vertex1, Vertex2);

            m_origin = vertex0;
            m_edge0 = vertex1 - vertex0;
            m_edge1 = vertex2 - vertex0;

            m_normal = Vector3.Cross(m_edge0, m_edge1);
            MyPhysicsUtils.NormalizeSafe(ref m_normal);
        }

        //  Same numbering as in the constructor
        public Vector3 GetPoint(int i)
        {
            if (i == 1)
                return Vertex1;

            if (i == 2)
                return Vertex2;

            return Vertex0;
        }

        //  Same numbering as in the constructor
        public void GetPoint(int i, out Vector3 point)
        {
            if (i == 1)
            {
                point = Vertex1;
                return;
            }

            if (i == 2)
            {
                point = Vertex2;
                return;
            }

            point = Vertex0;
        }

        //  Returns the point parameterised by t0 and t1
        public Vector3 GetPoint(float t0, float t1)
        {
            return m_origin + t0 * m_edge0 + t1 * m_edge1;
        }

        //  Gets the minimum and maximum extents of the triangleVertexes along the axis
        public void GetSpan(out float min, out float max, Vector3 axis)
        {
            float d0 = Vector3.Dot(GetPoint(0), axis);
            float d1 = Vector3.Dot(GetPoint(1), axis);
            float d2 = Vector3.Dot(GetPoint(2), axis);

            min = MyPhysicsUtils.Min(d0, d1, d2);
            max = MyPhysicsUtils.Max(d0, d1, d2);
        }

        public Vector3 Centre
        {
            get { return m_origin + 0.333333333333f * (m_edge0 + m_edge1); }
        }

        public Vector3 Origin
        {
            get { return m_origin; }
            set { m_origin = value; }
        }

        public Vector3 Edge0
        {
            get { return m_edge0; }
            set { m_edge0 = value; }
        }

        public Vector3 Edge1
        {
            get { return m_edge1; }
            set { m_edge1 = value; }
        }

        //  Edge2 goes from pt1 to pt2
        public Vector3 Edge2
        {
            get { return m_edge1 - m_edge0; }
        }

        //  Gets the triangleVertexes normal. If degenerate it will be normalised, but
        //  the direction may be wrong!
        public Vector3 Normal
        {
            get { return m_normal; }
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MyShort4
    {
        [FieldOffset(0)]
        public ulong packed_value;

        [FieldOffset(0)]
        public short X;
        [FieldOffset(2)]
        public short Y;
        [FieldOffset(4)]
        public short Z;
        [FieldOffset(6)]
        public short W;

        public MyShort4(short x, short y, short z, short w)
        {
            packed_value = 0;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}

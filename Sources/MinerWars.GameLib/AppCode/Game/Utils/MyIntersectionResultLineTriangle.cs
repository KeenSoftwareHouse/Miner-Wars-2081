using System;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Utils
{
    //  Result of intersection between a ray and a triangle. This structure can be used only if intersection was found!
    //  If returned intersection is with voxel, all coordinates are in absolute/world space
    //  If returned intersection is with model instance, all coordinates are in model's local space (so for drawing we need to trasform them using world matrix)
    struct MyIntersectionResultLineTriangle
    {
        //  IMPORTANT: Use these members only for readonly acces. Change them only inside the constructor.
        //  We can't mark them 'readonly' because sometimes they are sent to different methods through "ref"

        //  Distance to the intersection point (calculated as distance from 'line.From' to 'intersection point')
        public float Distance;
        
        //  World coordinates of intersected triangle. It is also used as input parameter for col/det functions.
        public MyTriangle_Vertexes InputTriangle;

        //  Normals of vertexes of intersected triangle
        public Vector3 InputTriangleNormal;

        public MyIntersectionResultLineTriangle(ref MyTriangle_Vertexes triangle, ref Vector3 triangleNormal, float distance)
        {
            InputTriangle = triangle;
            InputTriangleNormal = triangleNormal;
            Distance = distance;
        }
        
        //  Find and return closer intersection of these two. If intersection is null then it's not really an intersection.
        public static MyIntersectionResultLineTriangle? GetCloserIntersection(ref MyIntersectionResultLineTriangle? a, ref MyIntersectionResultLineTriangle? b)
        {
            if (((a == null) && (b != null)) ||
                ((a != null) && (b != null) && (b.Value.Distance < a.Value.Distance)))
            {
                //  If only "b" contains valid intersection, or when it's closer than "a"
                return b;
            }
            else
            {
                //  This will be returned also when ((a == null) && (b == null))
                return a;
            }
        }
    }

    //  More detailed version of MyIntersectionResultLineTriangle, contains some calculated data, etc. This is usually 
    //  used as a result of triangle intersection searches
    struct MyIntersectionResultLineTriangleEx
    {
        //  IMPORTANT: Use these members only for readonly acces. Change them only inside the constructor.
        //  We can't mark them 'readonly' because sometimes they are sent to different methods through "ref"

        public MyIntersectionResultLineTriangle Triangle;

        //  Point of intersection, always in object space. Use only if intersection with object.
        public Vector3 IntersectionPointInObjectSpace;

        //  Point of intersection - always in world space
        public Vector3 IntersectionPointInWorldSpace;

        //  If intersection occured with phys object, here will be it
        public MyEntity Entity;

        //  Normal vector of intersection triangle - always in world space. Can be calculaed from input positions.
        public Vector3 NormalInWorldSpace;

        //  Normal vector of intersection triangle, always in object space. Use only if intersection with object.
        public Vector3 NormalInObjectSpace;

        //  Line used to get intersection, transformed to object space. For voxels it is also in world space, but for objects, use GetLineInWorldSpace()
        public MyLine InputLineInObjectSpace;

        public MyIntersectionResultLineTriangleEx(MyIntersectionResultLineTriangle triangle, MyEntity physObject, ref MyLine line)
        {
            Triangle = triangle;
            Entity = physObject;
            InputLineInObjectSpace = line;

            NormalInObjectSpace = MyUtils.GetNormalVectorFromTriangle(ref Triangle.InputTriangle);
            IntersectionPointInObjectSpace = line.From + line.Direction * Triangle.Distance;

            if (Entity is MyVoxelMap)
            {
                IntersectionPointInWorldSpace = IntersectionPointInObjectSpace;
                NormalInWorldSpace = NormalInObjectSpace;

                //  This will move intersection point from world space into voxel map's object space
                IntersectionPointInObjectSpace = IntersectionPointInObjectSpace - ((MyVoxelMap)Entity).PositionLeftBottomCorner;
            }
            else
            {
                Matrix worldMatrix = Entity.WorldMatrix;
                NormalInWorldSpace = MyUtils.GetTransformNormalNormalized(NormalInObjectSpace, ref worldMatrix);
                IntersectionPointInWorldSpace = MyUtils.GetTransform(IntersectionPointInObjectSpace, ref worldMatrix);
            }
        }

        /*public MyLine GetInputLineInWorldSpace()
        {
            if (IntersectionType == MyIntersectionResultLineTriangleType.WITH_VOXEL)
            {
                return InputLineInObjectSpace;
            }
            else
            {
                return new MyLine(
                    MyUtils.GetTransform(InputLineInObjectSpace.From, ref PhysObject.WorldMatrix),
                    MyUtils.GetTransform(InputLineInObjectSpace.To, ref PhysObject.WorldMatrix), true);
            }
        }*/

        //  Find and return closer intersection of these two. If intersection is null then it's not really an intersection.
        public static MyIntersectionResultLineTriangleEx? GetCloserIntersection(ref MyIntersectionResultLineTriangleEx? a, ref MyIntersectionResultLineTriangleEx? b)
        {
            if (((a == null) && (b != null)) ||
                ((a != null) && (b != null) && (b.Value.Triangle.Distance < a.Value.Triangle.Distance)))
            {
                //  If only "b" contains valid intersection, or when it's closer than "a"
                return b;
            }
            else
            {
                //  This will be returned also when ((a == null) && (b == null))
                return a;
            }
        }

        //  Find if distance between two intersections is less than "tolerance distance".
        public static bool IsDistanceLessThanTolerance(ref MyIntersectionResultLineTriangleEx? a, ref MyIntersectionResultLineTriangleEx? b,
            float distanceTolerance)
        {
            if (((a == null) && (b != null)) ||
                ((a != null) && (b != null) && (Math.Abs(b.Value.Triangle.Distance - a.Value.Triangle.Distance) <= distanceTolerance)))
            {
                return true;
            }
            else
            {
                //  This will be returned also when ((a == null) && (b == null))
                return false;
            }
        }
    }
}

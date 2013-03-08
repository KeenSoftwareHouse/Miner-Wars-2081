using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Physics.Collisions;

namespace MinerWars.AppCode.Game.Models
{
    interface IMyTriangePruningStructure
    {
        MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(MyEntity physObject, ref MyLine line, IntersectionFlags flags = IntersectionFlags.DIRECT_TRIANGLES);
        MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(MyEntity physObject, ref MyLine line, ref Matrix customInvMatrix, IntersectionFlags flags = IntersectionFlags.DIRECT_TRIANGLES);

        //  Return list of triangles intersecting specified sphere. Angle between every triangleVertexes normal vector and 'referenceNormalVector'
        //  is calculated, and if more than 'maxAngle', we ignore such triangleVertexes.
        //  Triangles are returned in 'retTriangles', and this list must be preallocated!
        //  IMPORTANT: Sphere must be in model space, so don't transform it!
        void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, Vector3? referenceNormalVector, float? maxAngle, List<MyTriangle_Vertex_Normals> retTriangles, int maxNeighbourTriangles);

        //  Return true if object intersects specified sphere.
        //  This method doesn't return exact point of intersection or any additional data.
        //  We don't look for closest intersection - so we stop on first intersection found.
        bool GetIntersectionWithSphere(MyEntity physObject, ref BoundingSphere sphere);

        //  Return list of triangles intersecting specified sphere. Angle between every triangleVertexes normal vector and 'referenceNormalVector'
        //  is calculated, and if more than 'maxAngle', we ignore such triangleVertexes.
        //  Triangles are returned in 'retTriangles', and this list must be preallocated!
        //  IMPORTANT: Sphere must be in model space, so don't transform it!
        void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, List<MyTriangle_Vertex_Normal> retTriangles, int maxNeighbourTriangles);


        void GetTrianglesIntersectingAABB(ref BoundingBox sphere, List<MyTriangle_Vertex_Normal> retTriangles, int maxNeighbourTriangles);

        void Close();

        int Size { get; }
    }
}

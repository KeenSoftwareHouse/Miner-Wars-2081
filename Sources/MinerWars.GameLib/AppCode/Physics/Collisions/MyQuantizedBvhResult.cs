#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using BulletXNA.BulletCollision;

#endregion

namespace MinerWars.AppCode.Physics.Collisions
{
    public enum IntersectionFlags
    {
        DIRECT_TRIANGLES = 0x01,
        FLIPPED_TRIANGLES = 0x02,

        ALL_TRIANGLES = DIRECT_TRIANGLES | FLIPPED_TRIANGLES
    }


    class MyQuantizedBvhResult
    {
        MyModel m_model;
        MyIntersectionResultLineTriangle? result;
        MyLine m_line;
        IntersectionFlags m_flags;

        public readonly ProcessCollisionHandler ProcessTriangleHandler;

        public MyQuantizedBvhResult()
        {
            ProcessTriangleHandler = new ProcessCollisionHandler(ProcessTriangle);
        }

        public MyIntersectionResultLineTriangle? Result
        {
            get
            {
                return result;
            }
        }        

        public void Start(MyModel model, MyLine line, IntersectionFlags flags = IntersectionFlags.DIRECT_TRIANGLES)
        {
            result = null;
            m_model = model;
            m_line = line;
            m_flags = flags;
        }

        private float? ProcessTriangle(int triangleIndex)
        {
            System.Diagnostics.Debug.Assert((int)m_flags != 0);

            MyTriangle_Vertexes triangle;
            MyTriangleVertexIndices triangleIndices = m_model.Triangles[triangleIndex];

            m_model.GetVertex(triangleIndices.I0, triangleIndices.I2, triangleIndices.I1, out triangle.Vertex0, out triangle.Vertex1, out triangle.Vertex2);

            Vector3 calculatedTriangleNormal = MyUtils.GetNormalVectorFromTriangle(ref triangle);
               
            //We dont want backside intersections
            if (((int)(m_flags & IntersectionFlags.FLIPPED_TRIANGLES) == 0) &&
                Vector3.Dot(m_line.Direction, calculatedTriangleNormal) > 0)
                return null;
                 
            float? distance = MyUtils.GetLineTriangleIntersection(ref m_line, ref triangle);

            //  If intersection occured and if distance to intersection is closer to origin than any previous intersection
            if ((distance != null) && ((result == null) || (distance.Value < result.Value.Distance)))
            {
                //  We need to remember original triangleVertexes coordinates (not transformed by world matrix)
                result = new MyIntersectionResultLineTriangle(ref triangle, ref calculatedTriangleNormal, distance.Value);
                return distance.Value;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;
using BulletXNA;

namespace MinerWars.AppCode.Physics.Collisions
{
    class MyQuantizedBvhAdapter : IMyTriangePruningStructure
    {
        GImpactQuantizedBvh m_bvh;
        MyModel m_model;

        [ThreadStatic]
        static ObjectArray<int> m_overlappedTrianglesThreadStatic;
        static List<ObjectArray<int>> m_overlappedTrianglesThreadStaticCollection = new List<ObjectArray<int>>();

        [ThreadStatic]
        static MyQuantizedBvhResult m_resultThreadStatic;

        private static MyQuantizedBvhResult m_result
        {
            get
            {
                if (m_resultThreadStatic == null)
                {
                    m_resultThreadStatic = new MyQuantizedBvhResult();
                }
                return m_resultThreadStatic;
            }
        }

        private static ObjectArray<int> m_overlappedTriangles
        {
            get
            {
                if (m_overlappedTrianglesThreadStatic == null)
                {
                    m_overlappedTrianglesThreadStatic = new ObjectArray<int>(1024);
                    lock (m_overlappedTrianglesThreadStaticCollection)
                    {
                        m_overlappedTrianglesThreadStaticCollection.Add(m_overlappedTrianglesThreadStatic);
                    }
                }
                return m_overlappedTrianglesThreadStatic;
            }
        }

        public MyQuantizedBvhAdapter(GImpactQuantizedBvh bvh, MyModel model)
        {
            m_bvh = bvh;
            m_model = model;
        }

        public MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(MyEntity physObject, ref MyLine line, IntersectionFlags flags)
        {
            BoundingSphere vol = physObject.WorldVolume;
            //  Check if line intersects phys object's current bounding sphere, and if not, return 'no intersection'
            if (MyUtils.IsLineIntersectingBoundingSphere(ref line, ref vol) == false) return null;

            //  Transform line into 'model instance' local/object space. Bounding box of a line is needed!!
            Matrix worldInv = physObject.GetWorldMatrixInverted();

            return GetIntersectionWithLine(physObject, ref line, ref worldInv, flags);
        }

        public MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(MyEntity physObject, ref MyLine line, ref Matrix customInvMatrix, IntersectionFlags flags)
        {
            MyLine lineInModelSpace = new MyLine(MyUtils.GetTransform(line.From, ref customInvMatrix), MyUtils.GetTransform(line.To, ref customInvMatrix), true);

            //MyIntersectionResultLineTriangle? result = null;

            m_result.Start(m_model, lineInModelSpace, flags);

            var dir = new IndexedVector3(lineInModelSpace.Direction);
            var from = new IndexedVector3(lineInModelSpace.From);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_bvh.RayQueryClosest()");
            m_bvh.RayQueryClosest(ref dir, ref from, m_result.ProcessTriangleHandler);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (m_result.Result.HasValue)
            {
                return new MyIntersectionResultLineTriangleEx(m_result.Result.Value, physObject, ref lineInModelSpace);
            }
            else
            {
                return null;
            }
        }

        public void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, Vector3? referenceNormalVector, float? maxAngle, List<MyTriangle_Vertex_Normals> retTriangles, int maxNeighbourTriangles)
        {
            var aabb = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphere, ref aabb);
            AABB gi_aabb = new AABB(ref aabb.Min, ref aabb.Max);
            m_overlappedTriangles.Clear();
            if (m_bvh.BoxQuery(ref gi_aabb, m_overlappedTriangles))
            {
                // temporary variable for storing tirngle boundingbox info
                BoundingBox triangleBoundingBox = new BoundingBox();

                for (int i = 0; i < m_overlappedTriangles.Count; i++)
                {
                    var triangleIndex = m_overlappedTriangles[i];

                    //  If we reached end of the buffer of neighbour triangles, we stop adding new ones. This is better behavior than throwing exception because of array overflow.
                    if (retTriangles.Count == maxNeighbourTriangles) return;

                    m_model.GetTriangleBoundingBox(triangleIndex, ref triangleBoundingBox);

                    //  First test intersection of triangleVertexes's bounding box with bounding sphere. And only if they overlap or intersect, do further intersection tests.
                    if (MyUtils.IsBoxIntersectingSphere(triangleBoundingBox, ref sphere) == true)
                    {
                        //if (m_triangleIndices[value] != ignoreTriangleWithIndex)
                        {
                            //  See that we swaped vertex indices!!
                            MyTriangle_Vertexes triangle;
                            MyTriangle_Normals triangleNormals;
                            MyTriangle_Normals triangleBinormals;
                            MyTriangle_Normals triangleTangents;

                            MyTriangleVertexIndices triangleIndices = m_model.Triangles[triangleIndex];
                            m_model.GetVertex(triangleIndices.I0, triangleIndices.I2, triangleIndices.I1, out triangle.Vertex0, out triangle.Vertex1, out triangle.Vertex2);

                            /*
                            triangle.Vertex0 = m_model.GetVertex(triangleIndices.I0);
                            triangle.Vertex1 = m_model.GetVertex(triangleIndices.I2);
                            triangle.Vertex2 = m_model.GetVertex(triangleIndices.I1);
                              */

                            triangleNormals.Normal0 = m_model.GetVertexNormal(triangleIndices.I0);
                            triangleNormals.Normal1 = m_model.GetVertexNormal(triangleIndices.I2);
                            triangleNormals.Normal2 = m_model.GetVertexNormal(triangleIndices.I1);

                            if (MinerWars.AppCode.Game.Render.MyRenderConstants.RenderQualityProfile.ForwardRender)
                            {
                                triangleBinormals.Normal0 = triangleNormals.Normal0;
                                triangleBinormals.Normal1 = triangleNormals.Normal1;
                                triangleBinormals.Normal2 = triangleNormals.Normal2;

                                triangleTangents.Normal0 = triangleNormals.Normal0;
                                triangleTangents.Normal1 = triangleNormals.Normal1;
                                triangleTangents.Normal2 = triangleNormals.Normal2;
                            }
                            else
                            {
                                triangleBinormals.Normal0 = m_model.GetVertexBinormal(triangleIndices.I0);
                                triangleBinormals.Normal1 = m_model.GetVertexBinormal(triangleIndices.I2);
                                triangleBinormals.Normal2 = m_model.GetVertexBinormal(triangleIndices.I1);

                                triangleTangents.Normal0 = m_model.GetVertexTangent(triangleIndices.I0);
                                triangleTangents.Normal1 = m_model.GetVertexTangent(triangleIndices.I2);
                                triangleTangents.Normal2 = m_model.GetVertexTangent(triangleIndices.I1);
                            }

                            MyPlane trianglePlane = new MyPlane(ref triangle);

                            if (MyUtils.GetSphereTriangleIntersection(ref sphere, ref trianglePlane, ref triangle) != null)
                            {
                                Vector3 triangleNormal = MyUtils.GetNormalVectorFromTriangle(ref triangle);

                                if ((referenceNormalVector.HasValue == false) || (maxAngle.HasValue == false) ||
                                    ((MyUtils.GetAngleBetweenVectors(referenceNormalVector.Value, triangleNormal) <= maxAngle)))
                                {
                                    MyTriangle_Vertex_Normals retTriangle;
                                    retTriangle.Vertexes = triangle;
                                    retTriangle.Normals = triangleNormals;
                                    retTriangle.Binormals = triangleBinormals;
                                    retTriangle.Tangents = triangleTangents;

                                    retTriangles.Add(retTriangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool GetIntersectionWithSphere(MyEntity physObject, ref BoundingSphere sphere)
        {
            //  Transform sphere from world space to object space
            Matrix worldInv = physObject.GetWorldMatrixInverted();
            Vector3 positionInObjectSpace = MyUtils.GetTransform(sphere.Center, ref worldInv);
            BoundingSphere sphereInObjectSpace = new BoundingSphere(positionInObjectSpace, sphere.Radius);

            var aabb = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphereInObjectSpace, ref aabb);
            AABB gi_aabb = new AABB(ref aabb.Min, ref aabb.Max);
            m_overlappedTriangles.Clear();
            if (m_bvh.BoxQuery(ref gi_aabb, m_overlappedTriangles))
            {
                // temporary variable for storing tirngle boundingbox info
                BoundingBox triangleBoundingBox = new BoundingBox();

                //  Triangles that are directly in this node
                for (int i = 0; i < m_overlappedTriangles.Count; i++)
                {
                    var triangleIndex = m_overlappedTriangles[i];

                    m_model.GetTriangleBoundingBox(triangleIndex, ref triangleBoundingBox);

                    //  First test intersection of triangleVertexes's bounding box with bounding sphere. And only if they overlap or intersect, do further intersection tests.
                    if (MyUtils.IsBoxIntersectingSphere(triangleBoundingBox, ref sphereInObjectSpace) == true)
                    {
                        //  See that we swaped vertex indices!!
                        MyTriangle_Vertexes triangle;
                        MyTriangleVertexIndices triangleIndices = m_model.Triangles[triangleIndex];
                        triangle.Vertex0 = m_model.GetVertex(triangleIndices.I0);
                        triangle.Vertex1 = m_model.GetVertex(triangleIndices.I2);
                        triangle.Vertex2 = m_model.GetVertex(triangleIndices.I1);

                        MyPlane trianglePlane = new MyPlane(ref triangle);

                        if (MyUtils.GetSphereTriangleIntersection(ref sphereInObjectSpace, ref trianglePlane, ref triangle) != null)
                        {
                            //  If we found intersection we can stop and dont need to look further
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //slow
        [Obsolete("Slow,use aabb")]
        public void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, List<MyTriangle_Vertex_Normal> retTriangles, int maxNeighbourTriangles)
        {
            Vector3? referenceNormalVector = null;
            float? maxAngle = null;

            var aabb = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphere, ref aabb);
            AABB gi_aabb = new AABB(ref aabb.Min, ref aabb.Max);
            m_overlappedTriangles.Clear();

           // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_bvh.BoxQuery"); // This code is called recursively and cause profiler to lag
            bool res = m_bvh.BoxQuery(ref gi_aabb, m_overlappedTriangles);
           // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (res)
            {
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_overlappedTriangles");  // This code is called recursively and cause profiler to lag

                // temporary variable for storing tirngle boundingbox info
                BoundingBox triangleBoundingBox = new BoundingBox();

                for (int i = 0; i < m_overlappedTriangles.Count; i++)
                {
                    var triangleIndex = m_overlappedTriangles[i];

                    //  If we reached end of the buffer of neighbour triangles, we stop adding new ones. This is better behavior than throwing exception because of array overflow.
                    if (retTriangles.Count == maxNeighbourTriangles) return;

                    m_model.GetTriangleBoundingBox(triangleIndex, ref triangleBoundingBox);
                    
                    //gi_aabb.CollideTriangleExact

                    //  First test intersection of triangleVertexes's bounding box with bounding sphere. And only if they overlap or intersect, do further intersection tests.
                    if (MyUtils.IsBoxIntersectingSphere(triangleBoundingBox, ref sphere) == true)
                    {
                        //if (m_triangleIndices[value] != ignoreTriangleWithIndex)
                        {
                            //  See that we swaped vertex indices!!
                            MyTriangle_Vertexes triangle;

                            MyTriangleVertexIndices triangleIndices = m_model.Triangles[triangleIndex];
                            triangle.Vertex0 = m_model.GetVertex(triangleIndices.I0);
                            triangle.Vertex1 = m_model.GetVertex(triangleIndices.I2);
                            triangle.Vertex2 = m_model.GetVertex(triangleIndices.I1);
                            Vector3 calculatedTriangleNormal = MyUtils.GetNormalVectorFromTriangle(ref triangle);

                            MyPlane trianglePlane = new MyPlane(ref triangle);

                            if (MyUtils.GetSphereTriangleIntersection(ref sphere, ref trianglePlane, ref triangle) != null)
                            {
                                Vector3 triangleNormal = MyUtils.GetNormalVectorFromTriangle(ref triangle);

                                if ((referenceNormalVector.HasValue == false) || (maxAngle.HasValue == false) ||
                                    ((MyUtils.GetAngleBetweenVectors(referenceNormalVector.Value, triangleNormal) <= maxAngle)))
                                {
                                    MyTriangle_Vertex_Normal retTriangle;
                                    retTriangle.Vertexes = triangle;
                                    retTriangle.Normal = calculatedTriangleNormal;

                                    retTriangles.Add(retTriangle);
                                }
                            }
                        }
                    }
                }

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public void GetTrianglesIntersectingAABB(ref BoundingBox aabb, List<MyTriangle_Vertex_Normal> retTriangles, int maxNeighbourTriangles)  
        {
            IndexedVector3 min = new IndexedVector3(ref aabb.Min);
            IndexedVector3 max = new IndexedVector3(ref aabb.Max);
            AABB gi_aabb = new AABB(ref min, ref max);

            m_overlappedTriangles.Clear();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_bvh.BoxQuery");
            bool res = m_bvh.BoxQuery(ref gi_aabb, m_overlappedTriangles);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (res)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_overlappedTriangles");

                //foreach (var triangleIndex in m_overlappedTriangles)
                for (int i = 0; i < m_overlappedTriangles.Count; i++)
                {
                    var triangleIndex = m_overlappedTriangles[i];

                    //  If we reached end of the buffer of neighbour triangles, we stop adding new ones. This is better behavior than throwing exception because of array overflow.
                    if (retTriangles.Count == maxNeighbourTriangles)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        return;
                    }


                    MyTriangleVertexIndices triangle = m_model.Triangles[triangleIndex];
                    MyTriangle_Vertexes triangleVertices = new MyTriangle_Vertexes();
                    m_model.GetVertex(triangle.I0, triangle.I1, triangle.I2, out triangleVertices.Vertex0, out triangleVertices.Vertex1, out triangleVertices.Vertex2);

                    IndexedVector3 iv0 = new IndexedVector3(ref triangleVertices.Vertex0);
                    IndexedVector3 iv1 = new IndexedVector3(ref triangleVertices.Vertex1);
                    IndexedVector3 iv2 = new IndexedVector3(ref triangleVertices.Vertex2);

                    MyTriangle_Vertex_Normal retTriangle;
                    retTriangle.Vertexes = triangleVertices;
                    retTriangle.Normal = Vector3.Forward;

                    retTriangles.Add(retTriangle);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public void Close()
        {
            lock (m_overlappedTrianglesThreadStaticCollection)
            {
                foreach (var item in m_overlappedTrianglesThreadStaticCollection)
                {
                    item.Clear();
                }
            }
        }

        public int Size
        {
            get
            {
                return m_bvh.Size;
            }
        }
    }
}

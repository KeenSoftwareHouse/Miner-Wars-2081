#region Using Statements


#endregion

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box vs trianglemesh interaction
    /// </summary>

    class ColPointComparer : IComparer<MyCollisionPointStruct>
    {
        public int Compare(MyCollisionPointStruct x, MyCollisionPointStruct y)
        {
            return x.OldDepth.CompareTo(y.OldDepth);
        }
    }

    struct MyCollisionPointStruct
    {
        public MyCollisionPointStruct(float oldDepth, MySmallCollPointInfo collPointInfo)
        {
            OldDepth = oldDepth;
            CollPointInfo = collPointInfo;
        }

        readonly public float OldDepth;
        readonly public MySmallCollPointInfo CollPointInfo;
    }

    class MyRBBoxElementTriangleMeshElementInteraction : MyRBElementInteraction
    {
        private MyBox m_tempBox1 = new MyBox(Matrix.Identity, Vector3.Zero);
        private MyBox m_tempBox2 = new MyBox(Matrix.Identity, Vector3.Zero);
        private List<MyCollisionPointStruct> m_collPoints = new List<MyCollisionPointStruct>(32);
        List<Vector3> m_pts = new List<Vector3>(32);

        public static int TestsCount = 0;
        public static int TrianglesTested = 0;

        static ColPointComparer m_colPointComparer = new ColPointComparer();

        protected override bool Interact(bool staticCollision)
        {
            try
            {
                if (!staticCollision)
                {
                    TestsCount++;
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BoxTriangleIntersection");
                }

                if (RBElement1.GetElementType() != MyRBElementType.ET_BOX)
                    SwapElements();

                var boxElement = (MyRBBoxElement)RBElement1;
                var triangleMeshElem = (MyRBTriangleMeshElement)RBElement2;

                MyModel model = ((boxElement.Flags & MyElementFlag.EF_MODEL_PREFER_LOD0) > 0 ? triangleMeshElem.ModelLOD0 : triangleMeshElem.Model);

                Matrix boxMatrix = boxElement.GetGlobalTransformation();
                Matrix triangleMeshMatrix = triangleMeshElem.GetGlobalTransformation();

                Matrix newMatrix = boxMatrix;

                if (!staticCollision)
                {
                    // MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep
                    newMatrix.Translation = newMatrix.Translation + boxElement.GetRigidBody().LinearVelocity * MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
                }

                MyBox oldBox = m_tempBox1;
                MyBox newBox = m_tempBox2;

                oldBox.Transform.Orientation = boxMatrix;
                oldBox.Transform.Orientation.Translation = Vector3.Zero;
                oldBox.Transform.Position = boxMatrix.Translation - Vector3.TransformNormal(boxElement.Size * 0.5f, boxMatrix);

                newBox.Transform.Orientation = newMatrix;
                newBox.Transform.Orientation.Translation = Vector3.Zero;
                newBox.Transform.Position = newMatrix.Translation - Vector3.TransformNormal(boxElement.Size * 0.5f, newMatrix);

                oldBox.SideLengths = boxElement.Size;
                newBox.SideLengths = boxElement.Size;

                float boxRadius = newBox.GetBoundingRadiusAroundCentre();

                #region REFERENCE: Vector3 boxCentre = newBox.GetCentre();
                Vector3 boxCentre;
                newBox.GetCentre(out boxCentre);
                // Deano need to trasnform the box center into mesh space
                Matrix invTransformMatrix = Matrix.Invert(triangleMeshMatrix);

                Vector3.Transform(ref boxCentre, ref invTransformMatrix, out boxCentre);
                #endregion

                BoundingBox bb = boxElement.GetWorldSpaceAABB();

                if (staticCollision)
                {
                    Vector3 bbMin = Vector3.Transform(bb.Min, invTransformMatrix);
                    Vector3 bbMax = Vector3.Transform(bb.Max, invTransformMatrix);

                    BoundingSphere bs = new BoundingSphere((bbMax + bbMin) / 2, Vector3.Distance(bbMin, bbMax));
                    List<MyTriangle_Vertex_Normal> triangles = MyPhysics.physicsSystem.GetContactConstraintModule().GetTriangleCache().GetFreeTriangleList(this);
                    model.GetTrianglePruningStructure().GetTrianglesIntersectingSphere(ref bs, triangles, triangles.Capacity);

                    for (int iTriangle = 0; iTriangle < triangles.Count; iTriangle++)
                    {
                        MyTriangle_Vertex_Normal triangle = triangles[iTriangle];

                        MyPlane plane = new MyPlane(ref triangle.Vertexes);

                        // quick early test is done in mesh space
                        float dist = MyUtils.GetDistanceFromPointToPlane(ref boxCentre, ref plane);

                        if (dist > boxRadius || dist < -boxRadius)
                            continue;

                        Vector3 oldPos = boxMatrix.Translation;
                        Vector3 newPos = newMatrix.Translation;
                        float collisionEpsilon = 0;//pz to test not sure about value

                        if (DoOverlapBoxTriangleStaticTest(
                                oldBox, newBox,
                                triangle,
                                plane,
                                collisionEpsilon,
                                ref triangleMeshMatrix,
                                ref oldPos,
                                ref newPos))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    bb.Min += boxElement.GetRigidBody().LinearVelocity * MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
                    bb.Max += boxElement.GetRigidBody().LinearVelocity * MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

                    var boxCenter = bb.GetCenter();
                    // aabox is done in mesh space and handles the mesh transform correctly
                    //int numTriangles = mesh.GetTrianglesIntersectingtAABox(potentialTriangles, MaxLocalStackTris, ref bb);

                    //boxElement.GetRigidBody().Position = Vector3.Zero;
                    //triangleMeshElem.GetRigidBody().Position = Vector3.Zero;
                    //BoundingSphere bs = new BoundingSphere((bbMax + bbMin) / 2, Vector3.Distance(bbMin, bbMax));

                    var halfSize = bb.Size() / 2;
                    BoundingBox bb2 = new BoundingBox(boxCentre - halfSize, boxCentre + halfSize);

                    List<MyTriangle_Vertex_Normal> triangles = MyPhysics.physicsSystem.GetContactConstraintModule().GetTriangleCache().GetFreeTriangleList(this);

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PruningStructure");

                    model.GetTrianglePruningStructure().GetTrianglesIntersectingAABB(ref bb2, triangles, triangles.Capacity);
                    //model.GetTrianglePruningStructure().GetTrianglesIntersectingSphere(ref bs, triangles, triangles.Capacity);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Tests count ", TestsCount);

                    MySmallCollPointInfo[] collPtArray = MyContactInfoCache.SCPIStackAlloc();
                    int refPointer = 0;

                    m_collPoints.Clear();

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Triangles");

                    for (int iTriangle = 0; iTriangle < triangles.Count; iTriangle++)
                    {
                        MyTriangle_Vertex_Normal triangle = triangles[iTriangle];
                        //IndexedTriangle meshTriangle = mesh.GetTriangle(potentialTriangles[iTriangle]);

                        MyPlane plane = new MyPlane(ref triangle.Vertexes);

                        // quick early test is done in mesh space
                        //float dist = meshTriangle.Plane.DotCoordinate(boxCentre);
                        float dist = MyUtils.GetDistanceFromPointToPlane(ref boxCentre, ref plane);

                        if (dist > boxRadius || dist < -boxRadius)
                            continue;

                        Vector3 oldPos = boxMatrix.Translation;
                        Vector3 newPos = newMatrix.Translation;

                        DoOverlapBoxTriangleTest(
                                oldBox, newBox,
                                triangle,
                                plane,
                                MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon,
                                ref triangleMeshMatrix,
                                ref oldPos,
                                ref newPos,
                                m_collPoints);
                    }

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    TrianglesTested += triangles.Count;

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Triangles tested ", TrianglesTested);

                    m_collPoints.Sort(m_colPointComparer);

                    refPointer = 0;
                    foreach (MyCollisionPointStruct collPoint in m_collPoints)
                    {
                        collPtArray[refPointer] = collPoint.CollPointInfo;
                        refPointer++;
                        if (refPointer >= MyPhysicsConfig.MaxContactPoints)
                        {
                            break;
                        }
                    }

                    if (refPointer > 0)
                        MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collPtArray, refPointer);


                    MyContactInfoCache.FreeStackAlloc(collPtArray);
                    MyPhysics.physicsSystem.GetContactConstraintModule().GetTriangleCache().PushBackTriangleList(triangles);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!staticCollision) MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            return false;
        }

        private bool DoOverlapBoxTriangleStaticTest(MyBox oldBox, MyBox newBox,
            MyTriangle_Vertex_Normal triangle, MyPlane plane, float collTolerance,
            ref Matrix transformMatrix, ref Vector3 oldBoxPos, ref Vector3 newBoxPos)
        {

            Matrix dirs0 = newBox.Orientation;
            dirs0.Translation = Vector3.Zero;

            #region REFERENCE: Triangle tri = new Triangle(mesh.GetVertex(triangleVertexes.GetVertexIndex(0)),mesh.GetVertex(triangleVertexes.GetVertexIndex(1)),mesh.GetVertex(triangleVertexes.GetVertexIndex(2)));
            Vector3 triVec0 = triangle.Vertexes.Vertex0;
            Vector3 triVec1 = triangle.Vertexes.Vertex1;
            Vector3 triVec2 = triangle.Vertexes.Vertex2;

            // Deano move tri into world space
            //Matrix transformMatrix = mesh.TransformMatrix;
            Vector3.Transform(ref triVec0, ref transformMatrix, out triVec0);
            Vector3.Transform(ref triVec1, ref transformMatrix, out triVec1);
            Vector3.Transform(ref triVec2, ref transformMatrix, out triVec2);

            MyTriangle tri = new MyTriangle(ref triVec0, ref triVec1, ref triVec2);
            #endregion

            #region REFERENCE Vector3 triEdge0 = (tri.GetPoint(1) - tri.GetPoint(0));
            Vector3 pt0;
            Vector3 pt1;
            tri.GetPoint(0, out pt0);
            tri.GetPoint(1, out pt1);

            Vector3 triEdge0;
            Vector3.Subtract(ref pt1, ref pt0, out triEdge0);
            #endregion

            #region REFERENCE Vector3 triEdge1 = (tri.GetPoint(2) - tri.GetPoint(1));
            Vector3 pt2;
            tri.GetPoint(2, out pt2);

            Vector3 triEdge1;
            Vector3.Subtract(ref pt2, ref pt1, out triEdge1);
            #endregion

            #region REFERENCE Vector3 triEdge2 = (tri.GetPoint(0) - tri.GetPoint(2));
            Vector3 triEdge2;
            Vector3.Subtract(ref pt0, ref pt2, out triEdge2);
            #endregion

            if (triEdge0.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;
            if (triEdge1.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;
            if (triEdge2.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;

            triEdge0.Normalize();
            triEdge1.Normalize();
            triEdge2.Normalize();

            //Vector3 triNormal = triangle.Plane.Normal;
            Vector3 triNormal = plane.Normal;
            Vector3.TransformNormal(ref triNormal, ref transformMatrix, out triNormal);

            // the 15 potential separating axes
            const int numAxes = 13;
            Vector3[] axes = new Vector3[numAxes];

            axes[0] = triNormal;
            axes[1] = dirs0.Right;
            axes[2] = dirs0.Up;
            axes[3] = dirs0.Backward;
            Vector3.Cross(ref axes[1], ref triEdge0, out axes[4]);
            Vector3.Cross(ref axes[1], ref triEdge1, out axes[5]);
            Vector3.Cross(ref axes[1], ref triEdge2, out axes[6]);
            Vector3.Cross(ref axes[2], ref triEdge0, out axes[7]);
            Vector3.Cross(ref axes[2], ref triEdge1, out axes[8]);
            Vector3.Cross(ref axes[2], ref triEdge2, out axes[9]);
            Vector3.Cross(ref axes[3], ref triEdge0, out axes[10]);
            Vector3.Cross(ref axes[3], ref triEdge1, out axes[11]);
            Vector3.Cross(ref axes[3], ref triEdge2, out axes[12]);

            // the overlap depths along each axis
            float[] overlapDepths = new float[numAxes];

            // see if the boxes are separate along any axis, and if not keep a 
            // record of the depths along each axis
            int i;
            for (i = 0; i < numAxes; ++i)
            {
                overlapDepths[i] = 1.0f;
                if (Disjoint(out overlapDepths[i], axes[i], newBox, tri, collTolerance))
                    return false;
            }

            // The box overlap, find the separation depth closest to 0.
            float minDepth = float.MaxValue;
            int minAxis = -1;

            for (i = 0; i < numAxes; ++i)
            {
                // If we can't normalise the axis, skip it
                float l2 = axes[i].LengthSquared();
                if (l2 < MyPhysicsConfig.Epsilon)
                    continue;

                // Normalise the separation axis and the depth
                float invl = 1.0f / (float)System.Math.Sqrt(l2);
                axes[i] *= invl;
                overlapDepths[i] *= invl;

                // If this axis is the minimum, select it
                if (overlapDepths[i] < minDepth)
                {
                    minDepth = overlapDepths[i];
                    minAxis = i;
                }
            }

            if (minAxis == -1)
                return false;

            // Make sure the axis is facing towards the 0th box.
            // if not, invert it
            Vector3 D = newBox.GetCentre() - tri.Centre;
            Vector3 N = axes[minAxis];
            float depth = overlapDepths[minAxis];

            if (Vector3.Dot(D, N) > 0.0f)
                N *= -1;

            Vector3 boxOldPos = oldBoxPos; //(info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
            Vector3 boxNewPos = newBoxPos; // (info.Skin0.Owner != null) ? info.Skin0.Owner.Position : Vector3.Zero;
            Vector3 meshPos = transformMatrix.Translation; // (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;

            List<Vector3> pts = new List<Vector3>();
            //pts.Clear();

            const float combinationDist = 0.05f;
            GetBoxTriangleIntersectionPoints(pts, newBox, tri, depth + combinationDist);

            // adjust the depth 
            #region delta
            Vector3 delta;
            Vector3.Subtract(ref boxNewPos, ref boxOldPos, out delta);
            #endregion

            #region oldDepth
            float oldDepth;
            Vector3.Dot(ref delta, ref N, out oldDepth);
            oldDepth += depth;
            #endregion

            // report collisions
            int numPts = pts.Count;
            
            if (numPts > 0)
                return true;
            else
                return false;
        }

        private bool DoOverlapBoxTriangleTest(MyBox oldBox, MyBox newBox, MyTriangle_Vertex_Normal triangle, MyPlane plane, float collTolerance,
            ref Matrix transformMatrix, ref Vector3 oldBoxPos, ref Vector3 newBoxPos, List<MyCollisionPointStruct> collPoints)
        {
            Matrix dirs0 = newBox.Orientation;
            dirs0.Translation = Vector3.Zero;

            #region Triangle
            Vector3 triVec0 = triangle.Vertexes.Vertex0;
            Vector3 triVec1 = triangle.Vertexes.Vertex1;
            Vector3 triVec2 = triangle.Vertexes.Vertex2;
            //mesh.GetVertex(triangle.GetVertexIndex(0), out triVec0);
            //mesh.GetVertex(triangle.GetVertexIndex(1), out triVec1);
            //mesh.GetVertex(triangle.GetVertexIndex(2), out triVec2);

            // Deano move tri into world space
            //Matrix transformMatrix = mesh.TransformMatrix;
            Vector3.Transform(ref triVec0, ref transformMatrix, out triVec0);
            Vector3.Transform(ref triVec1, ref transformMatrix, out triVec1);
            Vector3.Transform(ref triVec2, ref transformMatrix, out triVec2);

            MyTriangle tri = new MyTriangle(ref triVec0, ref triVec1, ref triVec2);
            #endregion


            #region triEdge0
            Vector3 pt0;
            Vector3 pt1;
            tri.GetPoint(0, out pt0);
            tri.GetPoint(1, out pt1);

            Vector3 triEdge0;
            Vector3.Subtract(ref pt1, ref pt0, out triEdge0);
            #endregion

            #region triEdge1
            Vector3 pt2;
            tri.GetPoint(2, out pt2);

            Vector3 triEdge1;
            Vector3.Subtract(ref pt2, ref pt1, out triEdge1);
            #endregion

            #region triEdge2
            Vector3 triEdge2;
            Vector3.Subtract(ref pt0, ref pt2, out triEdge2);
            #endregion


            if (triEdge0.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;
            if (triEdge1.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;
            if (triEdge2.LengthSquared() < MyMwcMathConstants.EPSILON)
                return false;


            /*
            triEdge0 = MyMwcUtils.Normalize(triEdge0);
            triEdge1 = MyMwcUtils.Normalize(triEdge1);
            triEdge2 = MyMwcUtils.Normalize(triEdge2);
              */
            triEdge0.Normalize();
            triEdge1.Normalize();
            triEdge2.Normalize();

            //Vector3 triNormal = triangle.Plane.Normal;
            Vector3 triNormal = plane.Normal;
            Vector3.TransformNormal(ref triNormal, ref transformMatrix, out triNormal);


            

            // the 15 potential separating axes (comment by Marek Rosa: note says 15 but code uses 13... I don't know why, mistake in the note??)
            const int NUM_AXES = 13;
            MyVector3Array13 axes = new MyVector3Array13();

            axes[0] = triNormal;
            axes[1] = dirs0.Right;
            axes[2] = dirs0.Up;
            axes[3] = dirs0.Backward;
            axes[4] = Vector3.Cross(axes[1], triEdge0);
            axes[5] = Vector3.Cross(axes[1], triEdge1);
            axes[6] = Vector3.Cross(axes[1], triEdge2);
            axes[7] = Vector3.Cross(axes[2], triEdge0);
            axes[8] = Vector3.Cross(axes[2], triEdge1);
            axes[9] = Vector3.Cross(axes[2], triEdge2);
            axes[10] = Vector3.Cross(axes[3], triEdge0);
            axes[11] = Vector3.Cross(axes[3], triEdge1);
            axes[12] = Vector3.Cross(axes[3], triEdge2);

            // the overlap depths along each axis
            MyFloatArray13 overlapDepths = new MyFloatArray13();

            // see if the boxes are separate along any axis, and if not keep a 
            // record of the depths along each axis
            int i;
            for (i = 0; i < NUM_AXES; ++i)
            {
                overlapDepths[i] = 1.0f;
                
                bool b;
                overlapDepths[i] = Disjoint(out b, axes[i], newBox, tri, collTolerance);
                if (b) return false;
            }

            // The box overlap, find the separation depth closest to 0.
            float minDepth = float.MaxValue;
            int minAxis = -1;

            for (i = 0; i < NUM_AXES; ++i)
            {
                // If we can't normalise the axis, skip it
                float l2 = axes[i].LengthSquared();
                if (l2 < MyPhysicsConfig.Epsilon)
                    continue;

                // Normalise the separation axis and the depth
                float invl = 1.0f / (float)System.Math.Sqrt(l2);
                axes[i] *= invl;
                overlapDepths[i] *= invl;

                // If this axis is the minimum, select it
                if (overlapDepths[i] < minDepth)
                {
                    minDepth = overlapDepths[i];
                    minAxis = i;
                }
            }

            if (minAxis == -1)
                return false;


            // Make sure the axis is facing towards the 0th box.
            // if not, invert it
            Vector3 D = newBox.GetCentre() - tri.Centre;
            Vector3 N = axes[minAxis];
            float depth = overlapDepths[minAxis];

            if (Vector3.Dot(D, N) > 0.0f)
                N *= -1;

            Vector3 boxOldPos = oldBoxPos; //(info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
            Vector3 boxNewPos = newBoxPos; // (info.Skin0.Owner != null) ? info.Skin0.Owner.Position : Vector3.Zero;
            Vector3 meshPos = transformMatrix.Translation; // (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;

            m_pts.Clear();

            const float combinationDist = 0.05f;
            GetBoxTriangleIntersectionPoints(m_pts, newBox, tri, depth + combinationDist);

            // adjust the depth 
            #region delta
            Vector3 delta;
            Vector3.Subtract(ref boxNewPos, ref boxOldPos, out delta);
            #endregion

            #region oldDepth
            float oldDepth;
            Vector3.Dot(ref delta, ref N, out oldDepth);
            oldDepth += depth;
            #endregion

            
            // report collisions
            
            int numPts = m_pts.Count;
            if (numPts > 0)
            {
                for (i = 0; i < numPts; ++i)
                {
                    collPoints.Add(new MyCollisionPointStruct(-oldDepth, new MySmallCollPointInfo(m_pts[i] - boxNewPos, m_pts[i] - meshPos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, N, -oldDepth, m_pts[i])));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SegmentTriangleIntersection(out float tS, out float tT0, out float tT1, MySegment seg, MyTriangle triangle)
        {
            /// the parameters - if hit then they get copied into the args
            float u, v, t;

            tS = 0;
            tT0 = 0;
            tT1 = 0;

            Vector3 e1 = triangle.Edge0;
            Vector3 e2 = triangle.Edge1;
            Vector3 p = Vector3.Cross(seg.Delta, e2);
            float a = Vector3.Dot(e1, p);
            if (a > -MyPhysicsConfig.Epsilon && a < MyPhysicsConfig.Epsilon)
                return false;
            float f = 1.0f / a;
            Vector3 s = seg.Origin - triangle.Origin;
            u = f * Vector3.Dot(s, p);
            if (u < 0.0f || u > 1.0f)
                return false;
            Vector3 q = Vector3.Cross(s, e1);
            v = f * Vector3.Dot(seg.Delta, q);
            if (v < 0.0f || (u + v) > 1.0f)
                return false;
            t = f * Vector3.Dot(e2, q);
            if (t < 0.0f || t > 1.0f)
                return false;

            tS = t;
            tT0 = u;
            tT1 = v;
            //if (tS != 0) tS = t;
            //if (tT0 != 0) tT0 = u;
            //if (tT1 != 0) tT1 = v;
            return true;
        }

        private int GetBoxTriangleIntersectionPoints(List<Vector3> pts, MyBox box, MyTriangle triangle, float combinationDistance)
        {
            // first intersect each edge of the box with the triangleVertexes
            MyBox.Edge[] edges;
            box.GetEdges(out edges);
            Vector3[] boxPts;
            box.GetCornerPoints(out boxPts);

            float tS;
            float tv1, tv2;

            int iEdge;
            for (iEdge = 0; iEdge < 12; ++iEdge)
            {
                MyBox.Edge edge = edges[iEdge];
                MySegment seg = new MySegment(boxPts[(int)edge.Ind0], boxPts[(int)edge.Ind1] - boxPts[(int)edge.Ind0]);
                if (SegmentTriangleIntersection(out tS, out tv1, out tv2, seg, triangle))
                {
                    AddPoint(pts, seg.GetPoint(tS), combinationDistance * combinationDistance);
                }
            }

            Vector3 pos, n;
            // now each edge of the triangleVertexes with the box
            for (iEdge = 0; iEdge < 3; ++iEdge)
            {
                Vector3 pt0 = triangle.GetPoint(iEdge);
                Vector3 pt1 = triangle.GetPoint((iEdge + 1) % 3);
                MySegment s1 = new MySegment(pt0, pt1 - pt0);
                MySegment s2 = new MySegment(pt1, pt0 - pt1);
                if (box.SegmentIntersect(out tS, out pos, out n, s1))
                    AddPoint(pts, pos, combinationDistance * combinationDistance);
                if (box.SegmentIntersect(out tS, out pos, out n, s2))
                    AddPoint(pts, pos, combinationDistance * combinationDistance);
            }

            return pts.Count;
        }

        public float PointPointDistanceSq(Vector3 pt1, Vector3 pt2)
        {
            float num3 = pt1.X - pt2.X;
            float num2 = pt1.Y - pt2.Y;
            float num0 = pt1.Z - pt2.Z;
            return ((num3 * num3) + (num2 * num2)) + (num0 * num0);
        }

        private bool AddPoint(List<Vector3> pts, Vector3 pt, float combinationDistanceSq)
        {
            for (int i = pts.Count; i-- != 0; )
            {
                if (PointPointDistanceSq(pts[i], pt) < combinationDistanceSq)
                {
                    pts[i] = 0.5f * (pts[i] + pt);
                    return false;
                }
            }
            pts.Add(pt);
            return true;
        }

        private float Disjoint(out bool b, Vector3 axis, MyBox box, MyTriangle triangle, float collTolerance)
        {
            float ret;
            b = Disjoint(out ret, axis, box, triangle, collTolerance);
            return ret;
        }

        private bool Disjoint(out float d, Vector3 axis, MyBox box, MyTriangle triangle, float collTolerance)
        {
            float min0, max0, min1, max1;

            box.GetSpan(out min0, out max0, axis);
            triangle.GetSpan(out min1, out max1, axis);

            if (min0 > (max1 + collTolerance + MyPhysicsConfig.Epsilon) ||
                min1 > (max0 + collTolerance + MyPhysicsConfig.Epsilon))
            {
                d = 0.0f;
                return true;
            }

            if ((max0 > max1) && (min1 > min0))
            {
                // triangleVertexes is inside - choose the min dist to move it out
                d = System.Math.Min(max0 - min1, max1 - min0);
            }
            else if ((max1 > max0) && (min0 > min1))
            {
                // box is inside - choose the min dist to move it out
                d = System.Math.Min(max1 - min0, max0 - min1);
            }
            else
            {
                // objects overlap
                d = (max0 < max1) ? max0 : max1;
                d -= (min0 > min1) ? min0 : min1;
            }

            return false;
        }

        public override MyRBElementInteraction CreateNewInstance() { return new MyRBBoxElementTriangleMeshElementInteraction(); }
    }

}
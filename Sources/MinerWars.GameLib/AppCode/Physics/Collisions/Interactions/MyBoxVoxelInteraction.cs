#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box vs voxel interaction
    /// </summary>

    class MyRBBoxElementVoxelElementInteraction : MyRBElementInteraction
    {
        private MyBox tempBox = new MyBox(Matrix.Identity, Vector3.Zero);

        private const int numAxes = 13;
        private Vector3[] m_axes = new Vector3[numAxes];
        private float[] m_overlapDepths = new float[numAxes];

        struct MyCP
        {
            public Vector3 m_Position;
            public Vector3 m_Normal;
            public float m_Depth;
        }

        private List<MyCP> m_CPList = new List<MyCP>(16);

        public override MyRBElementInteraction CreateNewInstance() { return new MyRBBoxElementVoxelElementInteraction(); }

        protected override bool Interact(bool staticCollision)
        {
            if (RBElement1.GetElementType() != MyRBElementType.ET_BOX)
                SwapElements();

            if (!staticCollision) MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BoxVoxelInteraction");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Transformations");

            MyRBBoxElement rbbox0 = (MyRBBoxElement)RBElement1;

            Matrix matrix0 = RBElement1.GetGlobalTransformation();

            MyBox box = tempBox;

            box.Transform.Orientation = matrix0;
            box.Transform.Orientation.Translation = Vector3.Zero;
            box.Transform.Position = matrix0.Translation - Vector3.TransformNormal(rbbox0.Size * 0.5f, matrix0);

            box.SideLengths = rbbox0.Size;

            float boxRadius = box.GetBoundingRadiusAroundCentre();

            #region boxCentre
            Vector3 boxCentre;
            box.GetCentre(out boxCentre);
            // Deano need to trasnform the box center into mesh space
            //Matrix invTransformMatrix = mesh.InverseTransformMatrix;
            //Vector3.Transform(ref boxCentre, ref invTransformMatrix, out boxCentre);
            #endregion

            BoundingBox bb = RBElement1.GetWorldSpaceAABB();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetPotentialTrianglesForColDet");
            // extent bb for the movement            
            int numTriangles;
            MyVoxelMaps.GetPotentialTrianglesForColDet(out numTriangles, ref bb);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (staticCollision)
            {
                for (int iTriangle = 0; iTriangle < numTriangles; ++iTriangle)
                {
                    MyColDetVoxelTriangle triangle = MyVoxelMaps.PotentialColDetTriangles[iTriangle];

                    // quick early test is done in mesh space
                    float dist = triangle.Plane.DotCoordinate(boxCentre);

                    if (dist > boxRadius) continue;

                    // skip too narrow triangles causing destability
                    if ((triangle.Vertex0 - triangle.Vertex1).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                        || (triangle.Vertex1 - triangle.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                        || (triangle.Vertex0 - triangle.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                       )
                        continue;

                    if (DoOverlapBoxTriangleStaticTest(box, ref triangle))
                        return true;
                }
            }
            else
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("for DoOverlapBoxTriangleTest");

                for (int iTriangle = 0; iTriangle < numTriangles; ++iTriangle)
                {
                    MyColDetVoxelTriangle triangle = MyVoxelMaps.PotentialColDetTriangles[iTriangle];

                    // skip too narrow triangles causing destability
                    if ((triangle.Vertex0 - triangle.Vertex1).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                        || (triangle.Vertex1 - triangle.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                        || (triangle.Vertex0 - triangle.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon
                       )
                        continue;

                    DoOverlapBoxTriangleTest(box, ref triangle);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if (!staticCollision) MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return false;
        }


        private bool DoOverlapBoxTriangleStaticTest(MyBox box, ref MyColDetVoxelTriangle triangle)
        {

            Matrix dirs0 = box.Orientation;

            #region triEdge0
            Vector3 pt0;
            Vector3 pt1;
            triangle.GetPoint(0, out pt0);
            triangle.GetPoint(1, out pt1);

            Vector3 triEdge0;
            Vector3.Subtract(ref pt1, ref pt0, out triEdge0);

            if (triEdge0.LengthSquared() < MyPhysicsConfig.Epsilon)
                return false;

            #endregion

            #region triEdge1
            Vector3 pt2;
            triangle.GetPoint(2, out pt2);

            Vector3 triEdge1;
            Vector3.Subtract(ref pt2, ref pt1, out triEdge1);

            if (triEdge1.LengthSquared() < MyPhysicsConfig.Epsilon)
                return false;

            #endregion

            #region triEdge2
            Vector3 triEdge2;
            Vector3.Subtract(ref pt0, ref pt2, out triEdge2);

            if (triEdge2.LengthSquared() < MyPhysicsConfig.Epsilon)
                return false;

            #endregion

            triEdge0.Normalize();
            triEdge1.Normalize();
            triEdge2.Normalize();

            Vector3 triNormal = triangle.Plane.Normal;             

            m_axes[0] = triNormal;
            m_axes[1] = dirs0.Right;
            m_axes[2] = dirs0.Up;
            m_axes[3] = dirs0.Backward;
            Vector3.Cross(ref m_axes[1], ref triEdge0, out m_axes[4]);
            Vector3.Cross(ref m_axes[1], ref triEdge1, out m_axes[5]);
            Vector3.Cross(ref m_axes[1], ref triEdge2, out m_axes[6]);
            Vector3.Cross(ref m_axes[2], ref triEdge0, out m_axes[7]);
            Vector3.Cross(ref m_axes[2], ref triEdge1, out m_axes[8]);
            Vector3.Cross(ref m_axes[2], ref triEdge2, out m_axes[9]);
            Vector3.Cross(ref m_axes[3], ref triEdge0, out m_axes[10]);
            Vector3.Cross(ref m_axes[3], ref triEdge1, out m_axes[11]);
            Vector3.Cross(ref m_axes[3], ref triEdge2, out m_axes[12]);

            // the overlap depths along each axis
            

            // see if the boxes are separate along any axis, and if not keep a 
            // record of the depths along each axis
            int i;
            for (i = 0; i < numAxes; ++i)
            {
                m_overlapDepths[i] = 1.0f;
                if (Disjoint(out m_overlapDepths[i], m_axes[i], box, triangle, MyPhysicsConfig.CollisionEpsilon))
                    return false;
            }

            // The box overlap, find the separation depth closest to 0.
            float minDepth = float.MaxValue;
            int minAxis = -1;

            for (i = 0; i < numAxes; ++i)
            {
                // If we can't normalise the axis, skip it
                float l2 = m_axes[i].LengthSquared();
                if (l2 < MyPhysicsConfig.Epsilon)
                    continue;

                // Normalise the separation axis and the depth
                float invl = 1.0f / (float)System.Math.Sqrt(l2);
                m_axes[i] *= invl;
                m_overlapDepths[i] *= invl;

                // If this axis is the minimum, select it
                if (m_overlapDepths[i] < minDepth)
                {
                    minDepth = m_overlapDepths[i];
                    minAxis = i;
                }
            }

            if (minAxis == -1)
                return false;

            // Make sure the axis is facing towards the 0th box.
            // if not, invert it
            Vector3 D = box.GetCentre() - triangle.Centre;
            Vector3 N = m_axes[minAxis];
            float depth = m_overlapDepths[minAxis];

            if (Vector3.Dot(D, N) < 0.0f)
                N *= -1;

            MyRigidBody rbo0 = GetRigidBody1();
            MyRigidBody rbo1 = GetRigidBody2();
            float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

            Vector3 boxOldPos = rbo0.Position;
            Vector3 boxNewPos = rbo0.Position + rbo0.LinearVelocity * dt;
            Vector3 meshPos = rbo1.Position;

            m_CPList.Clear();
            GetBoxTriangleIntersectionPoints(m_CPList, box, triangle, MyPhysicsConfig.CollisionEpsilon);

            // adjust the depth 
            #region delta
            Vector3 delta;
            Vector3.Subtract(ref boxNewPos, ref boxOldPos, out delta);
            #endregion

            int numPts = m_CPList.Count;
            if (numPts > 0)
                return true;
            else
                return false;
        }

        private bool DoOverlapBoxTriangleTest(MyBox box, ref MyColDetVoxelTriangle triangle)
        {
            Matrix dirs0 = box.Orientation;

            Vector3 triEdge0;
            Vector3 triEdge1;
            Vector3 triEdge2;

            triEdge0 = MyMwcUtils.Normalize(triangle.Edge0);
            triEdge1 = MyMwcUtils.Normalize(triangle.Edge1);
            triEdge2 = MyMwcUtils.Normalize(triangle.Edge2);

            Vector3 triNormal = triangle.Plane.Normal;

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
                overlapDepths[i] = Disjoint(out b, axes[i], box, triangle, MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon);
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
            Vector3 D = box.GetCentre() - triangle.Centre;
            Vector3 N = axes[minAxis];
            float depth = overlapDepths[minAxis];

            if (Vector3.Dot(D, N) < 0.0f)
                N *= -1;

            MyRigidBody rbo0 = GetRigidBody1();
            MyRigidBody rbo1 = GetRigidBody2();
            float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

            Vector3 boxOldPos = rbo0.Position;
            Vector3 boxNewPos = rbo0.Position + rbo0.LinearVelocity * dt;
            Vector3 meshPos = rbo1.Position;

            m_CPList.Clear();

            GetBoxTriangleIntersectionPoints(m_CPList, box, triangle, MyPhysicsConfig.CollisionEpsilon);

            // adjust the depth 
            #region delta
            Vector3 delta;
            Vector3.Subtract(ref boxNewPos, ref boxOldPos, out delta);
            #endregion

            // report collisions
            int numPts = m_CPList.Count;
            MySmallCollPointInfo[] collPtArray = MyContactInfoCache.SCPIStackAlloc();
            {
                if (numPts > 0)
                {
                    if (numPts >= MyPhysicsConfig.MaxContactPoints)
                    {
                        numPts = MyPhysicsConfig.MaxContactPoints - 1;
                    }

                    // adjust positions
                    for (i = 0; i < numPts; ++i)
                    {
                        collPtArray[i] = new MySmallCollPointInfo(m_CPList[i].m_Position - boxOldPos, m_CPList[i].m_Position - meshPos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, m_CPList[i].m_Normal, m_CPList[i].m_Depth, m_CPList[i].m_Position);
                    }

                    MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collPtArray, numPts);
                    MyContactInfoCache.FreeStackAlloc(collPtArray);
                    return true;
                }
                else
                {
                    MyContactInfoCache.FreeStackAlloc(collPtArray);
                    return false;
                }
            }
          
        }

        private float Disjoint(out bool b, Vector3 axis, MyBox box, MyColDetVoxelTriangle triangle, float collTolerance)
        {
            float ret;
            b = Disjoint(out ret, axis, box, triangle, collTolerance);
            return ret;
        }        

        //  Disjoint Returns true if disjoint.  Returns false if intersecting,
        //  and sets the overlap depth, d scaled by the axis length
        private static bool Disjoint(out float d, Vector3 axis, MyBox box, MyColDetVoxelTriangle triangle, float collTolerance)
        {
            float min0, max0, min1, max1;

            box.GetSpan(out min0, out max0, axis);
            triangle.GetSpan(out min1, out max1, axis);

            if (min0 > (max1 + collTolerance ) ||
                min1 > (max0 + collTolerance ))
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

        //  GetBoxTriangleIntersectionPoints
        //  Pushes intersection points onto the back of pts. Returns the
        //  number of points found.
        //  Points that are close together (compared to 
        //  combinationDistance) get combined
        private int GetBoxTriangleIntersectionPoints(List<MyCP> pts, MyBox box, MyColDetVoxelTriangle triangle, float combinationDistance)
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
                if (this.SegmentTriangleIntersection(out tS, out tv1, out tv2, seg, triangle))
                {
                    float depthA = Vector3.Dot(boxPts[(int)edge.Ind0] - seg.GetPoint(tS), triangle.Normal);
                    float depthB = Vector3.Dot(boxPts[(int)edge.Ind1] - seg.GetPoint(tS), triangle.Normal);
                    AddPoint(pts, seg.GetPoint(tS),triangle.Normal, depthA < depthB ? depthA : depthB , combinationDistance * combinationDistance);
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
                {
                    float depthA = Vector3.Dot(pt0 - pos, triangle.Normal);
                    float depthB = Vector3.Dot(pt1 - pos, triangle.Normal);
                    AddPoint(pts, pos, triangle.Normal, depthA < depthB ? depthA : depthB, combinationDistance * combinationDistance);
                }
                if (box.SegmentIntersect(out tS, out pos, out n, s2))
                {
                    float depthA = Vector3.Dot(pt0 - pos, triangle.Normal);
                    float depthB = Vector3.Dot(pt1 - pos, triangle.Normal);
                    AddPoint(pts, pos, triangle.Normal, depthA < depthB ? depthA : depthB, combinationDistance * combinationDistance);
                }
            }

            return pts.Count;
        }

        public bool SegmentTriangleIntersection(out float tS, out float tT0, out float tT1,
                                               MySegment seg, MyColDetVoxelTriangle triangle)
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

        public static float PointPointDistanceSq(Vector3 pt1, Vector3 pt2)
        {
            float num3 = pt1.X - pt2.X;
            float num2 = pt1.Y - pt2.Y;
            float num0 = pt1.Z - pt2.Z;
            return ((num3 * num3) + (num2 * num2)) + (num0 * num0);
        }


        //  AddPoint
        //  if pt is less than Sqrt(combinationDistanceSq) from one of the
        //  others the original is replaced with the mean of it
        //  and pt, and false is returned. true means that pt was
        //  added to pts
        private bool AddPoint(List<MyCP> pts, Vector3 pt, Vector3 normal, float depth , float combinationDistanceSq)
        {
            for (int i = pts.Count; i-- != 0; )
            {
                if (PointPointDistanceSq(pts[i].m_Position, pt) < combinationDistanceSq)
                {
                    //pts[i] = 0.5f * (pts[i] + pt);
                    return false;
                }
            }
            MyCP cp = new MyCP();
            cp.m_Normal = -normal;
            cp.m_Normal = MyMwcUtils.Normalize(cp.m_Normal);
            cp.m_Position = pt;
            cp.m_Depth = depth*0.1f - 0.5f*MyPhysicsConfig.CollisionEpsilon;
            if (cp.m_Depth < -0.5f)
            {
                cp.m_Depth *= 0.5f;
            }
            pts.Add(cp);
            return true;
        }
    }

}
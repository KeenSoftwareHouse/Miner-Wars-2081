#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using System.Diagnostics;
using System;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// DetectFunctor for BoxBox Collisions.
    /// </summary>
    class MyRBBoxElementBoxElementInteraction: MyRBElementInteraction
    {
        public override MyRBElementInteraction CreateNewInstance() { return new MyRBBoxElementBoxElementInteraction(); }

        #region private struct ContactPoint
        private struct ContactPoint
        {
            public Vector3 Pos;
            public int Count;

            public ContactPoint(ref Vector3 pos) { this.Pos = pos; this.Count = 1; }
        }
        #endregion

        /// <summary>
        /// Disjoint Returns true if disjoint. Returns false if intersecting,
        /// and sets the overlap depth, d scaled by the axis length.
        /// </summary>
        private static bool Disjoint(out float d, ref Vector3 axis, MyBox box0, MyBox box1, float collTolerance)
        {
            float min0, max0, min1, max1;

            box0.GetSpan(out min0, out max0, axis);
            box1.GetSpan(out min1, out max1, axis);

            if (min0 > (max1 + collTolerance + MyPhysicsConfig.CollisionEpsilon) ||
                min1 > (max0 + collTolerance + MyPhysicsConfig.CollisionEpsilon))
            {
                d = 0.0f;
                return true;
            }

            if ((max0 > max1) && (min1 > min0))
            {
                // box1 is inside - choose the min dist to move it out
                d = MathHelper.Min(max0 - min1, max1 - min0);
            }
            else if ((max1 > max0) && (min0 > min1))
            {
                // box0 is inside - choose the min dist to move it out
                d = MathHelper.Min(max1 - min0, max0 - min1);
            }
            else
            {
                // boxes overlap
                d = (max0 < max1) ? max0 : max1;
                d -= (min0 > min1) ? min0 : min1;
            }

            return false;
        }

        private static void GetSupportPoint(out Vector3 p, MyBox box, Vector3 axis)
        {
            #region INLINE: Vector3 orient0 = box.Orientation.Right;
            Vector3 orient0 = new Vector3(
                box.Transform.Orientation.M11,
                box.Transform.Orientation.M12,
                box.Transform.Orientation.M13);
            #endregion

            #region INLINE: Vector3 orient1 = box.Orientation.Up;
            Vector3 orient1 = new Vector3(
                box.Transform.Orientation.M21,
                box.Transform.Orientation.M22,
                box.Transform.Orientation.M23);
            #endregion

            #region INLINE: Vector3 orient2 = box.Orientation.Backward;
            Vector3 orient2 = new Vector3(
                box.Transform.Orientation.M31,
                box.Transform.Orientation.M32,
                box.Transform.Orientation.M33);
            #endregion

            #region INLINE: float ass = Vector3.Dot(axis,orient0);
            float ass = axis.X * orient0.X + axis.Y * orient0.Y + axis.Z * orient0.Z;
            #endregion

            #region INLINE: float au = Vector3.Dot(axis,orient1);
            float au = axis.X * orient1.X + axis.Y * orient1.Y + axis.Z * orient1.Z;
            #endregion

            #region INLINE: float ad = Vector3.Dot(axis,orient2);
            float ad = axis.X * orient2.X + axis.Y * orient2.Y + axis.Z * orient2.Z;
            #endregion

            float threshold = MyPhysicsConfig.CollisionEpsilon;

            box.GetCentre(out p);

            if (ass < -threshold)
            {
                #region INLINE: p += orient0 * (0.5 * box.SideLength.X);
                p.X += orient0.X * (0.5f * box.SideLengths.X);
                p.Y += orient0.Y * (0.5f * box.SideLengths.X);
                p.Z += orient0.Z * (0.5f * box.SideLengths.X);
                #endregion
            }
            else if (ass >= threshold)
            {
                #region INLINE: p -=  orient0 * (0.5 * box.SideLength.X);
                p.X -= orient0.X * (0.5f * box.SideLengths.X);
                p.Y -= orient0.Y * (0.5f * box.SideLengths.X);
                p.Z -= orient0.Z * (0.5f * box.SideLengths.X);
                #endregion
            }

            if (au < -threshold)
            {
                #region INLINE: p += orient1 * (0.5 * box.SideLength.Y);
                p.X += orient1.X * (0.5f * box.SideLengths.Y);
                p.Y += orient1.Y * (0.5f * box.SideLengths.Y);
                p.Z += orient1.Z * (0.5f * box.SideLengths.Y);
                #endregion
            }
            else if (au >= threshold)
            {
                #region INLINE: p -= orient1 * (0.5 * box.SideLength.Y);
                p.X -= orient1.X * (0.5f * box.SideLengths.Y);
                p.Y -= orient1.Y * (0.5f * box.SideLengths.Y);
                p.Z -= orient1.Z * (0.5f * box.SideLengths.Y);
                #endregion
            }

            if (ad < -threshold)
            {
                #region INLINE: p += orient2 * (0.5 * box.SideLength.Z);
                p.X += orient2.X * (0.5f * box.SideLengths.Z);
                p.Y += orient2.Y * (0.5f * box.SideLengths.Z);
                p.Z += orient2.Z * (0.5f * box.SideLengths.Z);
                #endregion
            }
            else if (ad >= threshold)
            {
                #region INLINE: p -= orient2 * (0.5 * box.SideLength.Z);
                p.X -= orient2.X * (0.5f * box.SideLengths.Z);
                p.Y -= orient2.Y * (0.5f * box.SideLengths.Z);
                p.Z -= orient2.Z * (0.5f * box.SideLengths.Z);
                #endregion
            }
        }

        /// <summary>
        /// AddPoint
        /// if pt is less than Sqrt(combinationDistanceSq) from one of the
        /// others the original is replaced with the mean of it
        /// and pt, and false is returned. true means that pt was
        /// added to pts
        /// </summary>
        private static bool AddPoint(List<ContactPoint> pts, ref Vector3 pt, float combinationDistanceSq)
        {
            for (int i = pts.Count; i-- != 0; )
            {
                ContactPoint cpt = pts[i];

                #region INLINE: float len = (cpt.Pos-pt).LengthSquared();
                float xd = cpt.Pos.X - pt.X;
                float yd = cpt.Pos.Y - pt.Y;
                float zd = cpt.Pos.Z - pt.Z;

                float len = (xd * xd) + (yd * yd) + (zd * zd);
                #endregion

                if (len < combinationDistanceSq)
                {
                    cpt.Pos = (cpt.Count * cpt.Pos + pt) / (cpt.Count + 1);
                    cpt.Count += 1;
                    return false;
                }
            }
            pts.Add(new ContactPoint(ref pt));
            return true;
        }

        /// <summary>
        /// The AABox has a corner at the origin and size sides.
        /// </summary>
        private static int GetAABox2EdgeIntersectionPoints(List<ContactPoint> pts,
            ref Vector3 sides, MyBox box, ref Vector3 edgePt0, ref Vector3 edgePt1,
            ref Matrix origBoxOrient, ref Vector3 origBoxPos,
            float combinationDistanceSq)
        {
            // The AABox faces are aligned with the world directions. Loop 
            // over the 3 directions and do the two tests. We know that the
            // AABox has a corner at the origin
            #region REFERENCE: Vector3 edgeDir = JiggleMath.NormalizeSafe(edgePt1 - edgePt0);
            Vector3 edgeDir;
            Vector3.Subtract(ref edgePt1, ref edgePt0, out edgeDir);
            MyPhysicsUtils.NormalizeSafe(ref edgeDir);
            #endregion

            int num = 0;

            for (int idir = 3; idir-- != 0; )
            {
                // skip edge/face tests if nearly parallel
                if (System.Math.Abs(MyPhysicsUtils.MyPhysicsUnsafe.Get(ref edgeDir, idir)) < 0.1f) continue;

                int jdir = (idir + 1) % 3;
                int kdir = (idir + 2) % 3;
                for (int iface = 2; iface-- != 0; )
                {
                    float offset = 0.0f;
                    if (iface == 1)
                    {
                        offset = MyPhysicsUtils.MyPhysicsUnsafe.Get(ref sides, idir);
                    }

                    float dist0 = MyPhysicsUtils.MyPhysicsUnsafe.Get(ref edgePt0, idir) - offset;
                    float dist1 = MyPhysicsUtils.MyPhysicsUnsafe.Get(ref edgePt1, idir) - offset;

                    float frac = -1.0f;

                    if (dist0 * dist1 < -MyPhysicsConfig.CollisionEpsilon)
                        frac = -dist0 / (dist1 - dist0);
                    else if (System.Math.Abs(dist0) < MyPhysicsConfig.CollisionEpsilon)
                        frac = 0.0f;
                    else if (System.Math.Abs(dist1) < MyPhysicsConfig.CollisionEpsilon)
                        frac = 1.0f;

                    if (frac >= 0.0f)
                    {
                        #region REFERENCE: Vector3 pt = (1.0f - frac) * edgePt0 + frac * edgePt1
                        Vector3 tmp; Vector3 pt;
                        Vector3.Multiply(ref edgePt1, frac, out tmp);
                        Vector3.Multiply(ref edgePt0, 1.0f - frac, out pt);
                        Vector3.Add(ref pt, ref tmp, out pt);
                        #endregion

                        // check the point is within the face rectangle
                        float ptJdir = MyPhysicsUtils.MyPhysicsUnsafe.Get(ref pt, jdir);
                        float ptKdir = MyPhysicsUtils.MyPhysicsUnsafe.Get(ref pt, kdir);

                        if ((ptJdir > -MyPhysicsConfig.CollisionEpsilon) &&
                            (ptJdir < MyPhysicsUtils.MyPhysicsUnsafe.Get(ref sides, jdir) + MyPhysicsConfig.CollisionEpsilon) &&
                            (ptKdir > -MyPhysicsConfig.CollisionEpsilon) &&
                            (ptKdir < MyPhysicsUtils.MyPhysicsUnsafe.Get(ref sides, kdir) + MyPhysicsConfig.CollisionEpsilon))
                        {
                            // woohoo got a point
                            #region REFERENCE: Vector3 pos = origBoxPos + Vector3.Transform(pt, origBoxOrient);
                            Vector3 pos;
                            Vector3.Transform(ref pt, ref origBoxOrient, out pos);
                            Vector3.Add(ref origBoxPos, ref pos, out pos);
                            #endregion

                            AddPoint(pts, ref pos, combinationDistanceSq);

                            if (++num == 2)
                                return num;
                        }
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// Pushes intersection points (in world space) onto the back of pts.
        /// Intersection is between an AABox faces and an orientated box's
        /// edges. orient and pos are used to transform the points from the
        /// AABox frame back into the original frame.
        /// </summary>
        private static int GetAABox2BoxEdgesIntersectionPoints(List<ContactPoint> pts, ref Vector3 sides,
            MyBox box, ref Matrix origBoxOrient, ref Vector3 origBoxPos, float combinationDistanceSq)
        {
            int num = 0;
            Vector3[] boxPts;
            box.GetCornerPoints(out boxPts);
            MyBox.Edge[] edges;
            box.GetEdges(out edges);

            for (int iedge = 0; iedge < 12; ++iedge)
            {
                Vector3 edgePt0 = boxPts[(int)edges[iedge].Ind0];
                Vector3 edgePt1 = boxPts[(int)edges[iedge].Ind1];

                num += GetAABox2EdgeIntersectionPoints(pts,
                    ref sides, box, ref edgePt0, ref edgePt1,
                    ref origBoxOrient, ref origBoxPos, combinationDistanceSq);

                // Don't think we can get more than 8... and anyway if we get too many 
                // then the penetration must be so bad who cares about the details?
                if (num >= 8) return num;
            }
            return num;
        }

        private static MyBox tempBox = new MyBox(Matrix.Identity, Vector3.Zero);

        /// <summary>
        /// Pushes intersection points onto the back of pts. Returns the
        /// number of points found.
        /// Points that are close together (compared to 
        /// combinationDistance) get combined
        /// dirToBody0 is the collision normal towards box0
        /// </summary>
        private static int GetBoxBoxIntersectionPoints(List<ContactPoint> pts,
            MyBox box0, MyBox box1, float combinationDistance,
            float collTolerance)
        {
            // first transform box1 into box0 space - there box0 has a corner
            // at the origin and faces parallel to the world planes. Then intersect
            // each of box1's edges with box0 faces, transforming each point back into
            // world space. Finally combine points
            float tolVal = 0.5f * collTolerance;

            Vector3 tol = new Vector3(tolVal);

            combinationDistance += collTolerance * 2.0f * (float)System.Math.Sqrt(3.0d);

            for (int ibox = 0; ibox < 2; ++ibox)
            {
                MyBox boxA = (ibox != 0) ? box1 : box0;
                MyBox boxB = (ibox != 0) ? box0 : box1;

                #region REFERENCE: Matrix boxAInvOrient = Matrix.Transpose(boxA.Orientation);
                Matrix boxAInvOrient;
                Matrix.Transpose(ref boxA.Transform.Orientation, out boxAInvOrient);
                #endregion

                #region REFERENCE: Vector3 pos = Vector3.Transform(boxB.m_initialSunWindPosition - boxA.m_initialSunWindPosition,boxAInvOrient)
                Vector3 pos;
                Vector3.Subtract(ref boxB.Transform.Position, ref boxA.Transform.Position, out pos);
                Vector3.TransformNormal(ref pos, ref boxAInvOrient, out pos);
                #endregion

                #region REFERENCE: Matrix boxOrient = boxB.Orientation * boxAInvOrient;
                Matrix boxOrient;
                Matrix.Multiply(ref boxB.Transform.Orientation, ref boxAInvOrient, out boxOrient);
                #endregion

                MyBox box = tempBox;
                box.Position = pos;
                box.Orientation = boxOrient;
                box.SideLengths = boxB.SideLengths;


                // if we get more than a certain number of points back from this call,
                // and iBox == 0, could probably skip the other test...
                Vector3 sL = boxA.SideLengths;
                GetAABox2BoxEdgesIntersectionPoints(pts, ref sL,
                    box, ref boxA.Transform.Orientation, ref boxA.Transform.Position, combinationDistance * combinationDistance);
            }

            return pts.Count;
        }

        // the 15 potential separating axes
        Vector3[] seperatingAxes = new Vector3[15];
        // the overlap depths along each axis
        float[] overlapDepth = new float[15];
        List<ContactPoint> contactPts = new List<ContactPoint>(64);

        MyBox m_TempBox1 = new MyBox(Matrix.Identity, Vector3.Zero);
        MyBox m_TempBox2 = new MyBox(Matrix.Identity, Vector3.Zero);

        protected override bool Interact(bool staticCollision)
        {
            if (!staticCollision) MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BoxBoxInteraction");

            try
            {
                MyRBBoxElement rbbox0 = (MyRBBoxElement)RBElement1;
                MyRBBoxElement rbbox1 = (MyRBBoxElement)RBElement2;

                MyBox box0 = m_TempBox1;
                MyBox box1 = m_TempBox2;

                Matrix matrix0 = rbbox0.GetGlobalTransformation();
                Matrix matrix1 = rbbox1.GetGlobalTransformation();

                box0.Transform.Orientation = matrix0;
                box0.Transform.Orientation.Translation = Vector3.Zero;
                box0.Transform.Position = matrix0.Translation - Vector3.TransformNormal(rbbox0.Size * 0.5f, matrix0);

                box1.Transform.Orientation = matrix1;
                box1.Transform.Orientation.Translation = Vector3.Zero;
                box1.Transform.Position = matrix1.Translation - Vector3.TransformNormal(rbbox1.Size * 0.5f, matrix1);

                box0.SideLengths = rbbox0.Size;
                box1.SideLengths = rbbox1.Size;

                // see if the boxes are separate along any axis, and if not keep a 
                // record of the depths along each axis
                for (int i = 0; i < 15; ++i)
                {
                    switch (i)
                    {
                        case 0: seperatingAxes[0] = box0.Orientation.Right; break;
                        case 1: seperatingAxes[1] = box0.Orientation.Up; break;
                        case 2: seperatingAxes[2] = box0.Orientation.Backward; break;
                        case 3: seperatingAxes[3] = box1.Orientation.Right; break;
                        case 4: seperatingAxes[4] = box1.Orientation.Up; break;
                        case 5: seperatingAxes[5] = box1.Orientation.Backward; break;
                        case 6: Vector3.Cross(ref seperatingAxes[0], ref seperatingAxes[3], out seperatingAxes[6]); break;
                        case 7: Vector3.Cross(ref seperatingAxes[0], ref seperatingAxes[4], out seperatingAxes[7]); break;
                        case 8: Vector3.Cross(ref seperatingAxes[0], ref seperatingAxes[5], out seperatingAxes[8]); break;
                        case 9: Vector3.Cross(ref seperatingAxes[1], ref seperatingAxes[3], out seperatingAxes[9]); break;
                        case 10: Vector3.Cross(ref seperatingAxes[1], ref seperatingAxes[4], out seperatingAxes[10]); break;
                        case 11: Vector3.Cross(ref seperatingAxes[1], ref seperatingAxes[5], out seperatingAxes[11]); break;
                        case 12: Vector3.Cross(ref seperatingAxes[2], ref seperatingAxes[3], out seperatingAxes[12]); break;
                        case 13: Vector3.Cross(ref seperatingAxes[2], ref seperatingAxes[4], out seperatingAxes[13]); break;
                        case 14: Vector3.Cross(ref seperatingAxes[2], ref seperatingAxes[5], out seperatingAxes[14]); break;
                    }

                    // If we can't normalise the axis, skip it
                    if (seperatingAxes[i].LengthSquared() < MyPhysicsConfig.CollisionEpsilon) continue;

                    overlapDepth[i] = float.MaxValue;

                    if (Disjoint(out overlapDepth[i], ref seperatingAxes[i], box0, box1, MyPhysicsConfig.CollisionEpsilon))
                        return false;
                }

                if (staticCollision)
                {
                    return true;  // Static collision: we're done.
                }

                // Dynamic collision.
                // The boxes overlap, find the seperation depth closest to 0.
                float minDepth = float.MaxValue;
                int minAxis = -1;

                for (int i = 0; i < 15; ++i)
                {
                    // If we can't normalise the axis, skip it
                    float l2 = seperatingAxes[i].LengthSquared();
                    if (l2 < MyPhysicsConfig.CollisionEpsilon) continue;

                    // Normalise the separation axis and depth
                    float invl = 1.0f / (float)System.Math.Sqrt(l2);
                    seperatingAxes[i] *= invl;
                    overlapDepth[i] *= invl;

                    // If this axis is the minmum, select it
                    if (overlapDepth[i] < minDepth)
                    {
                        minDepth = overlapDepth[i];
                        minAxis = i;
                    }
                }

                if (minAxis == -1)
                    return false;

                // Make sure the axis is facing towards the 0th box.
                // if not, invert it
                Vector3 D = box1.GetCentre() - box0.GetCentre();
                Vector3 N = seperatingAxes[minAxis];
                float depth = overlapDepth[minAxis];

                if (Vector3.Dot(D, N) < 0.0f)
                    N *= -1.0f;

                float minA = MathHelper.Min(box0.SideLengths.X, MathHelper.Min(box0.SideLengths.Y, box0.SideLengths.Z));
                float minB = MathHelper.Min(box1.SideLengths.X, MathHelper.Min(box1.SideLengths.Y, box1.SideLengths.Z));

                float combinationDist = 0.05f * MathHelper.Min(minA, minB);

                // the contact points            
                contactPts.Clear();

                int numPts = contactPts.Count;
                GetBoxBoxIntersectionPoints(contactPts, box0, box1, combinationDist, MyPhysicsConfig.CollisionEpsilon);
                numPts = contactPts.Count;

                MyRigidBody rbo0 = GetRigidBody1();
                MyRigidBody rbo1 = GetRigidBody2();
                float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

                Vector3 body0OldPos = rbo0.Position;
                Vector3 body1OldPos = rbo1.Position;
                Vector3 body0NewPos = (rbo0.Position + rbo0.LinearVelocity * dt);
                Vector3 body1NewPos = (rbo1.Position + rbo1.LinearVelocity * dt);

                #region REFERENCE: Vector3 bodyDelta = body0NewPos - body0OldPos - body1NewPos + body1OldPos;
                Vector3 bodyDelta;
                Vector3.Subtract(ref body0NewPos, ref body0OldPos, out bodyDelta);
                Vector3.Subtract(ref bodyDelta, ref body1NewPos, out bodyDelta);
                Vector3.Add(ref bodyDelta, ref body1OldPos, out bodyDelta);
                #endregion

                #region REFERENCE: float bodyDeltaLen = Vector3.Dot(bodyDelta,N);
                float bodyDeltaLen;
                Vector3.Dot(ref bodyDelta, ref N, out bodyDeltaLen);
                #endregion

                float oldDepth = depth + bodyDeltaLen;

                MySmallCollPointInfo[] collPtArray = MyContactInfoCache.SCPIStackAlloc();
                {
                    int numCollPts = 0;

                    Vector3 SATPoint;

                    switch (minAxis)
                    {
                        // Box0 face, Box1 corner collision
                        case 0:
                        case 1:
                        case 2:
                            {
                                // Get the lowest point on the box1 along box1 normal
                                GetSupportPoint(out SATPoint, box1, -N);
                                break;
                            }
                        // We have a Box2 corner/Box1 face collision
                        case 3:
                        case 4:
                        case 5:
                            {
                                // Find with vertex on the triangleVertexes collided
                                GetSupportPoint(out SATPoint, box0, N);
                                break;
                            }
                        // We have an edge/edge collision
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                            {
                                {
                                    // Retrieve which edges collided.
                                    int i = minAxis - 6;
                                    int ia = i / 3;
                                    int ib = i - ia * 3;
                                    // find two P0, P1 point on both edges. 
                                    Vector3 P0, P1;
                                    GetSupportPoint(out P0, box0, N);
                                    GetSupportPoint(out P1, box1, -N);
                                    // Find the edge intersection. 
                                    // plane along N and F, and passing through PB
                                    Vector3 box0Orient, box1Orient;
                                    MyPhysicsUtils.MyPhysicsUnsafe.Get(ref box0.Transform.Orientation, ia, out box0Orient);
                                    MyPhysicsUtils.MyPhysicsUnsafe.Get(ref box1.Transform.Orientation, ib, out box1Orient);

                                    #region REFERENCE: Vector3 planeNormal = Vector3.Cross(N, box1Orient[ib]);
                                    Vector3 planeNormal;
                                    Vector3.Cross(ref N, ref box1Orient, out planeNormal);
                                    #endregion

                                    #region REFERENCE: float planeD = Vector3.Dot(planeNormal, P1);
                                    float planeD;
                                    Vector3.Dot(ref planeNormal, ref P1, out planeD);
                                    #endregion

                                    // find the intersection t, where Pintersection = P0 + t*box edge dir
                                    #region REFERENCE: float div = Vector3.Dot(box0Orient, planeNormal);
                                    float div;
                                    Vector3.Dot(ref box0Orient, ref planeNormal, out div);
                                    #endregion

                                    // plane and ray colinear, skip the intersection.
                                    if (System.Math.Abs(div) < MyPhysicsConfig.CollisionEpsilon)
                                        return false;

                                    float t = (planeD - Vector3.Dot(P0, planeNormal)) / div;

                                    // point on edge of box0
                                    #region REFERENCE: P0 += box0Orient * t;
                                    P0 = Vector3.Add(Vector3.Multiply(box0Orient, t), P0);
                                    #endregion

                                    #region REFERENCE: SATPoint = (P0 + (0.5f * depth) * N);
                                    Vector3.Multiply(ref N, 0.5f * depth, out SATPoint);
                                    Vector3.Add(ref SATPoint, ref P0, out SATPoint);
                                    #endregion

                                }
                                break;
                            }
                        default:
                            {
                                SATPoint = Vector3.Zero;
                                Debug.Assert(false);
                                break;
                            }


                    }

                    // distribute the depth according to the distance to the SAT point
                    if (numPts > 0)
                    {
                        float minDist = float.MaxValue;
                        float maxDist = float.MinValue;
                        for (int i = 0; i < numPts; ++i)
                        {
                            float dist = MyPhysicsUtils.PointPointDistance(contactPts[i].Pos, SATPoint);
                            if (dist < minDist)
                                minDist = dist;
                            if (dist > maxDist)
                                maxDist = dist;
                        }

                        // got some intersection points
                        for (int i = 0; i < numPts; ++i)
                        {
                            float minDepthScale = 0.0f;
                            float dist = MyPhysicsUtils.PointPointDistance(contactPts[i].Pos, SATPoint);

                            float safeDivisionDist = (maxDist - minDist);
                            if ((maxDist - minDist) == 0.0f) safeDivisionDist = MyPhysicsConfig.CollisionEpsilon;
                            float depthScale = (dist - minDist) / safeDivisionDist;

                            depth = (1.0f - depthScale) * oldDepth + minDepthScale * depthScale * oldDepth;

                            if (numCollPts < MyPhysicsConfig.MaxContactPoints)
                            {
                                collPtArray[numCollPts++] = new MySmallCollPointInfo(contactPts[i].Pos - body0OldPos, contactPts[i].Pos - body1OldPos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, N, depth, contactPts[i].Pos);
                            }
                        }
                    }
                    else
                    {
                        #region REFERENCE: collPts.Add(new CollPointInfo(SATPoint - body0NewPos, SATPoint - body1NewPos, oldDepth));
                        //collPts.Add(new CollPointInfo(SATPoint - body0NewPos, SATPoint - body1NewPos, oldDepth));
                        Vector3 cp0;
                        Vector3.Subtract(ref SATPoint, ref body0NewPos, out cp0);

                        Vector3 cp1;
                        Vector3.Subtract(ref SATPoint, ref body1NewPos, out cp1);

                        if (numCollPts < MyPhysicsConfig.MaxContactPoints)
                        {
                            collPtArray[numCollPts++] = new MySmallCollPointInfo(cp0, cp1, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, N, oldDepth, SATPoint);
                        }
                        #endregion
                    }

                    // report Collisions
                    MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collPtArray, numCollPts);
                }
                MyContactInfoCache.FreeStackAlloc(collPtArray);
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
    }
}


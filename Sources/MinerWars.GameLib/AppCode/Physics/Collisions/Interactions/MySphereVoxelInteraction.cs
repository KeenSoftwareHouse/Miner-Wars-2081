#region Using Statements
using System.Collections.Generic;
using SysUtils.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Memory;
using SysUtils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere vs voxel interaction
    /// </summary>
    class MyRBSphereElementVoxelElementInteraction : MyRBElementInteraction
    {
        public override MyRBElementInteraction CreateNewInstance() { return new MyRBSphereElementVoxelElementInteraction(); }

        protected override bool Interact(bool staticCollision)
        {
            if (staticCollision)
            {
                //MyCommonDebugUtils.AssertDebug(false, "Sphere-voxel static interaction called! And that's wrong.");
            }
            else
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SphereVoxelInteraction");

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Transformations");

                if (RBElement1.GetElementType() != MyRBElementType.ET_SPHERE)
                    SwapElements();

                Matrix matrix0 = RBElement1.GetGlobalTransformation();
                Matrix matrix1 = RBElement2.GetGlobalTransformation();

                float sphereRadius = ((MyRBSphereElement)RBElement1).Radius;

                Vector3 body0Pos = matrix0.Translation; // sphere pos
                Vector3 body1Pos = matrix1.Translation;

                float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
                float epsylon = MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon;

                Vector3 newBody0Pos = matrix0.Translation + GetRigidBody1().LinearVelocity * dt;

                float sphereTolR = epsylon + sphereRadius;
                float sphereTolR2 = sphereTolR * sphereTolR;

                MySmallCollPointInfo[] collPtArray = MyContactInfoCache.SCPIStackAlloc();
                int numCollPts = 0;

                Vector3 collNormal = Vector3.Zero;

                //var colDetThroughVoxels = MyConstants.SPHERE_VOXELMAP_COLDET_THROUGH_VOXELS;
                var colDetThroughVoxels = !GetRigidBody1().ReadFlag(RigidBodyFlag.RBF_COLDET_THROUGH_VOXEL_TRIANGLES);

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                if (colDetThroughVoxels)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("colDetThroughVoxels");

                    BoundingSphere newSphere;
                    newSphere.Center = newBody0Pos;
                    newSphere.Radius = sphereRadius;

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PoolList.Get");
                    using (var voxelMapsFounded = PoolList<MyVoxelMap>.Get())
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("GetListOfVoxelMapsWhoseBoundingSphereIntersectsSphere");

                        MyVoxelMaps.GetListOfVoxelMapsWhoseBoundingSphereIntersectsSphere(ref newSphere, voxelMapsFounded, null);

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("foreach (MyVoxelMap voxelMap in voxelMapsFounded)");
                        foreach (MyVoxelMap voxelMap in voxelMapsFounded)
                        {
                            if (voxelMap != null)
                            {
                                //  We will iterate only voxels contained in the bounding box of new sphere, so here we get min/max corned in voxel units
                                MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                                    newSphere.Center.X - newSphere.Radius,
                                    newSphere.Center.Y - newSphere.Radius,
                                    newSphere.Center.Z - newSphere.Radius));
                                MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                                    newSphere.Center.X + newSphere.Radius,
                                    newSphere.Center.Y + newSphere.Radius,
                                    newSphere.Center.Z + newSphere.Radius));
                                voxelMap.FixVoxelCoord(ref minCorner);
                                voxelMap.FixVoxelCoord(ref maxCorner);

                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("for loop");

                                MyMwcVector3Int tempVoxelCoord;
                                for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
                                {
                                    for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                                    {
                                        for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                                        {
                                            byte voxelContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                                            //  Ignore voxels bellow the ISO value (empty, partialy empty...)
                                            if (voxelContent < MyVoxelConstants.VOXEL_ISO_LEVEL) continue;

                                            Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);

                                            //float voxelSize = MyVoxelMaps.GetVoxelContentAsFloat(voxelContent) * MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF;
                                            float voxelSize = MyVoxelMaps.GetVoxelContentAsFloat(voxelContent) * MyVoxelConstants.VOXEL_RADIUS;

                                            //  If distance to voxel border is less than sphere radius, we have a collision
                                            //  So now we calculate normal vector and penetration depth but on OLD sphere
                                            float newDistanceToVoxel = Vector3.Distance(voxelPosition, newSphere.Center) - voxelSize;
                                            if (newDistanceToVoxel < (epsylon + newSphere.Radius))
                                            {
                                                Vector3 collisionN = MyMwcUtils.Normalize(voxelPosition - body0Pos);

                                                if (numCollPts < MyPhysicsConfig.MaxContactPoints)
                                                {
                                                    //  Calculate penetration depth, but from old sphere (not new)
                                                    float oldDistanceToVoxel = Vector3.Distance(voxelPosition, newSphere.Center) - voxelSize;
                                                    float oldPenetrationDepth = oldDistanceToVoxel - sphereRadius;

                                                    // Vector3 pt = body0Pos + sphereRadius * collisionN;
                                                    Vector3 pt = voxelPosition - collisionN * (voxelSize - epsylon);

                                                    collPtArray[numCollPts++] = new MySmallCollPointInfo(pt - body0Pos, pt - body1Pos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, collisionN, oldPenetrationDepth, pt);
                                                }

                                                collNormal -= collisionN;
                                            }
                                        }
                                    }
                                }

                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                            }
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                else //if (colDetThroughVoxels)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ColDet triangles");

                    int optimalIterationCount = (int)(GetRigidBody1().LinearVelocity.Length() * dt / sphereRadius);
                    int maxIndex = (int)MathHelper.Min(MathHelper.Max(optimalIterationCount, 1), 16);

                    for (int i = 0; i < maxIndex; i++)
                    {

                        float velocityAdd = GetRigidBody1().LinearVelocity.Length() * dt / (float)maxIndex;

                        Vector3 interpolatedPosition = body0Pos + GetRigidBody1().LinearVelocity * dt * i / (float)maxIndex;

                        BoundingSphere newSphere;
                        newSphere.Center = interpolatedPosition;
                        newSphere.Radius = sphereRadius;

                        int numTriangles;

                        BoundingBox bb = BoundingBox.CreateFromSphere(newSphere);
                        MyVoxelMaps.GetPotentialTrianglesForColDet(out numTriangles, ref bb);

                        for (int iTriangle = 0; iTriangle < numTriangles; ++iTriangle)
                        {
                            MyColDetVoxelTriangle meshTriangle = MyVoxelMaps.PotentialColDetTriangles[iTriangle]; // mesh.GetTriangle(potentialTriangles[iTriangle]);


                            MyTriangle_Vertex_Normal triangle = new MyTriangle_Vertex_Normal();
                            triangle.Vertexes.Vertex0 = meshTriangle.Vertex0;
                            triangle.Vertexes.Vertex1 = meshTriangle.Vertex1;
                            triangle.Vertexes.Vertex2 = meshTriangle.Vertex2;


                            // skip too narrow triangles causing instability
                            if ((triangle.Vertexes.Vertex0 - triangle.Vertexes.Vertex1).LengthSquared() < MyPhysicsConfig.TriangleEpsilon)
                            {
                                continue;
                            }

                            if ((triangle.Vertexes.Vertex1 - triangle.Vertexes.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon)
                            {
                                continue;
                            }

                            if ((triangle.Vertexes.Vertex0 - triangle.Vertexes.Vertex2).LengthSquared() < MyPhysicsConfig.TriangleEpsilon)
                            {
                                continue;
                            }

                            MyPlane plane = new MyPlane(ref triangle.Vertexes);

                            Vector3? pt = MyUtils.GetSphereTriangleIntersection(ref newSphere, ref plane, ref triangle.Vertexes);
                            if (pt == null)
                                continue;


                            Vector3 collisionN = plane.Normal;

                            // skip triangle in case the normal is in wrong dir (narrow walls)  
                            Vector3 tempV = (newBody0Pos - pt.Value);
                            if (Vector3.Dot(collisionN, tempV) >= 0.8f * tempV.Length())  // equivalent to dot(collisionN, normalize(tempV)) > 0.8f, but works for zero vectors
                            {
                                continue;
                            }

                            float depth = Vector3.Distance(pt.Value, body0Pos) - sphereRadius;

                            if (numCollPts < MyPhysicsConfig.MaxContactPoints)
                            {
                                // since impulse get applied at the old position
                                Vector3 p2 = pt.Value;

                                collPtArray[numCollPts++] = new MySmallCollPointInfo(p2 - body0Pos, p2 - body1Pos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, collisionN, depth, p2);
                            }

                            collNormal += collisionN;
                        }
                    }

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                if (numCollPts > 0)
                {
                    MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collPtArray, numCollPts);
                }
                MyContactInfoCache.FreeStackAlloc(collPtArray);

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            return false;
        }
    }
}
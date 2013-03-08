#region Using Statements

#endregion

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere vs trianglemesh interaction
    /// </summary>
    class MyRBSphereElementTriangleMeshElementInteraction : MyRBElementInteraction
    {
        public override MyRBElementInteraction CreateNewInstance() { return new MyRBSphereElementTriangleMeshElementInteraction(); }

        const int MAX_AVAILABLE_ITERATION = 16;

        protected override bool Interact(bool staticCollision)
        {
            if (RBElement1.GetElementType() != MyRBElementType.ET_SPHERE)
                SwapElements();

            Matrix matrix0 = RBElement1.GetGlobalTransformation();
            Matrix matrix1 = RBElement2.GetGlobalTransformation();

            float sphereRadius = ((MyRBSphereElement)RBElement1).Radius;

            Vector3 body0Pos = matrix0.Translation; // sphere pos
            Vector3 body1Pos = matrix1.Translation;

            Matrix tempMat1 = matrix1;

            Matrix inverseMatrix1 = Matrix.Invert(tempMat1);

            MyModel model = ((RBElement1.Flags & MyElementFlag.EF_MODEL_PREFER_LOD0) > 0 ? ((MyRBTriangleMeshElement)RBElement2).ModelLOD0 : ((MyRBTriangleMeshElement)RBElement2).Model);

            if (staticCollision)
            {
                BoundingSphere bsphere = new BoundingSphere(body0Pos, sphereRadius);
                return model.GetTrianglePruningStructure().GetIntersectionWithSphere(((MinerWars.AppCode.Game.Physics.MyPhysicsBody)RBElement2.GetRigidBody().m_UserData).Entity, ref bsphere);
            }
            else
            {
                float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
                float epsilon = MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon;

                MySmallCollPointInfo[] collPtArray = MyContactInfoCache.SCPIStackAlloc();
                int numCollPts = 0;

                Vector3 collNormal = Vector3.Zero;

                int optimalIterationCount = (int)(GetRigidBody1().LinearVelocity.Length() * dt / (sphereRadius * 2)) + 1;
                //PZ: after consultation with petrM 1-4 iteration will be just ok
                int maxIndex = (int)MathHelper.Min(optimalIterationCount, MAX_AVAILABLE_ITERATION);
                //float speed = GetRigidBody1().LinearVelocity.Length();

                Vector3 velocityAdd = GetRigidBody1().LinearVelocity * dt / (float)maxIndex;
                float velocityAddLength = velocityAdd.Length();

                List<MyTriangle_Vertex_Normal> triangles = MyPhysics.physicsSystem.GetContactConstraintModule().GetTriangleCache().GetFreeTriangleList(this);

                //PZ: we will try to interpolate sphere position during this tick
                //we have to have at least one iteration
                for (int index = 0; index < maxIndex; index++)
                {
                    //PZ: from starting point 
                    Vector3 interpolatedPosition = body0Pos + velocityAdd * index;

                    // Deano : get the spheres centers in triangleVertexes mesh space
                    Vector3 newSphereCen = Vector3.Transform(interpolatedPosition, inverseMatrix1);

                    //  Transform sphere from world space to object space                        
                    BoundingSphere newSphereInObjectSpace = new BoundingSphere(newSphereCen, sphereRadius + velocityAddLength + epsilon);
                    BoundingBox newAABBInObjectSpace = BoundingBox.CreateFromSphere(newSphereInObjectSpace);

                    model.GetTrianglePruningStructure().GetTrianglesIntersectingAABB(ref newAABBInObjectSpace, triangles, triangles.Capacity);

                    for (int i = 0; i < triangles.Count; i++)
                    {
                        MyTriangle_Vertex_Normal triangle = triangles[i];

                        // skip too narrow triangles causing instability
                        /*    This must be done in preprocessor!
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
                        }        */

                        MyPlane plane = new MyPlane(ref triangle.Vertexes);

                        Vector3? pt = MyUtils.GetSphereTriangleIntersection(ref newSphereInObjectSpace, ref plane, ref triangle.Vertexes);
                        if (pt == null)
                            continue;

                        pt = Vector3.Transform(pt.Value, matrix1);

                        Vector3 collisionN = -plane.Normal;
                        collisionN = Vector3.TransformNormal(collisionN, matrix1);

                        // skip triangle in case the normal is in wrong dir (narrow walls)  
                        Vector3 tempV = (interpolatedPosition - pt.Value);
                        if (Vector3.Dot(collisionN, tempV) >= 0.8f * tempV.Length())  // equivalent to if (Vector3.Dot(collisionN, Vector3.Normalize(tempV)) > 0.8f)
                        {
                            continue;
                        }

                        float depth = Vector3.Distance(pt.Value, interpolatedPosition) - sphereRadius;

                        if (numCollPts < MyPhysicsConfig.MaxContactPoints)
                        {
                            // since impulse get applied at the old position
                            Vector3 p2 = pt.Value; // body0Pos - sphereRadius * 1.1f * collisionN;                                    

                            collPtArray[numCollPts++] = new MySmallCollPointInfo(p2 - interpolatedPosition, p2 - body1Pos, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, collisionN, depth, p2);

                            /*
                            MyDebugDraw.AddDrawTriangle(
                                Vector3.Transform(triangle.Vertexes.Vertex0, matrix1),
                                Vector3.Transform(triangle.Vertexes.Vertex1, matrix1),
                                Vector3.Transform(triangle.Vertexes.Vertex2, matrix1),
                                Color.Red);
                            */
                        }

                        collNormal += collisionN;
                    }

                    if (numCollPts > 0) // break if we catch any triangles in this iteration
                        break;

                }
                if (numCollPts > 0)
                {
                    MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collPtArray, numCollPts);
                }
                MyPhysics.physicsSystem.GetContactConstraintModule().GetTriangleCache().PushBackTriangleList(triangles);
                MyContactInfoCache.FreeStackAlloc(collPtArray);
            }
            return false;
        }
    }
}
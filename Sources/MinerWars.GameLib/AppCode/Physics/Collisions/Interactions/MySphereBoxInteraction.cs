#region Using Statements

using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere vs box interaction
    /// </summary>

    class MyRBSphereElementBoxElementInteraction : MyRBElementInteraction
    {
        public override MyRBElementInteraction CreateNewInstance() { return new MyRBSphereElementBoxElementInteraction(); }

        protected override bool Interact(bool staticCollision)
        {
            if (!staticCollision && GetRigidBody1().IsStatic() && GetRigidBody2().IsStatic()) return false;

            MyRBBoxElement box = null;
            MyRBSphereElement sphere = null;
            if (RBElement1.GetElementType() == MyRBElementType.ET_BOX)
                SwapElements();

            box = (MyRBBoxElement)RBElement2;
            sphere = (MyRBSphereElement)RBElement1;

            Matrix boxMatrix = box.GetGlobalTransformation();
            Vector3 sphereCenter = sphere.GetGlobalTransformation().Translation;

            Matrix invBoxMatrix = Matrix.Invert(boxMatrix);

            Vector3 boxLocalsphereCenter = Vector3.Transform(sphereCenter, invBoxMatrix);

            bool penetration = false;
            Vector3 normal = new Vector3();
            Vector3 closestPos = new Vector3();
            uint customData = 0;

            box.GetClosestPoint(boxLocalsphereCenter, ref closestPos, ref normal, ref penetration, ref customData);

            closestPos = Vector3.Transform(closestPos, boxMatrix);

            normal = -Vector3.TransformNormal(normal, boxMatrix);
            normal = MyMwcUtils.Normalize(normal);

            float vLength = (sphereCenter - closestPos).Length();

            if (staticCollision)
            {
                return vLength > 0 && vLength < sphere.Radius;
            }
            else
            {
                float eps = MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon;
                float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

                Vector3 pointVelocity1 = new Vector3();
                Vector3 pointVelocity2 = new Vector3();

                GetRigidBody1().GetGlobalPointVelocity(ref closestPos, out pointVelocity1);
                GetRigidBody2().GetGlobalPointVelocity(ref closestPos, out pointVelocity2);

                float dynEps = 0;
                if (vLength >= eps)
                {
                    float dot = Vector3.Dot(pointVelocity1 - pointVelocity2, normal) * dt;
                    if (dot >= 0)
                    {
                        dynEps = dot;
                    }
                }

                float radius = sphere.Radius;

                //Second part of condition commented due to 5968: Bug B - rocket passing through prefab
                //Does not seem to have any reason to be there
                if (vLength > 0 /*&& vLength < (radius + eps + dynEps)*/)
                {
                    float error = vLength - (radius + 0.5f * eps);
                    //error = System.Math.Min(error, eps);

                    MySmallCollPointInfo[] collInfo = MyContactInfoCache.SCPIStackAlloc();

                    collInfo[0] = new MySmallCollPointInfo(closestPos - sphereCenter, closestPos - boxMatrix.Translation, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, normal, error, closestPos);

                    MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collInfo, 1);

                    MyContactInfoCache.FreeStackAlloc(collInfo);
                }
            }
            return false;
        }
    }
}
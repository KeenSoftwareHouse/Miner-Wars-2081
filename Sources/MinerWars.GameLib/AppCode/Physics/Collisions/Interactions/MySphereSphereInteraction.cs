#region Using Statements

using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere vs sphere interaction
    /// </summary>

    class MyRBSphereElementSphereElementInteraction : MyRBElementInteraction
    {
        public override MyRBElementInteraction CreateNewInstance() { return new MyRBSphereElementSphereElementInteraction(); }

        protected override bool Interact(bool staticCollision)
        {
            if (!staticCollision && GetRigidBody1().IsStatic() && GetRigidBody2().IsStatic()) return false;

            MyRBSphereElement sphere1 = (MyRBSphereElement)RBElement1;
            MyRBSphereElement sphere2 = (MyRBSphereElement)RBElement2;

            Matrix matrix1 = sphere1.GetGlobalTransformation();
            Matrix matrix2 = sphere2.GetGlobalTransformation();

            Vector3 p1 = matrix1.Translation;
            Vector3 p2 = matrix2.Translation;
            Vector3 d = p2 - p1;
            float length = d.Length();

            float contactRadius = sphere1.Radius + sphere2.Radius;

            float eps = MyPhysics.physicsSystem.GetRigidBodyModule().CollisionEpsilon;

            if (staticCollision)
            {
                return length < contactRadius;
            }
            
            // from now on we handle dynamic collision
            float dynEps = 0;
            if (!staticCollision && length > eps)
            {
                dynEps = Vector3.Dot(GetRigidBody1().LinearVelocity - GetRigidBody2().LinearVelocity, d) / length * MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
                if (dynEps < 0)
                    dynEps = 0;
            }

            if (length > MyMwcMathConstants.EPSILON && length < contactRadius + eps + dynEps)
            {
                Vector3 n = MyMwcUtils.Normalize(d);
                Vector3 p = p1 + n * (sphere1.Radius + (length - contactRadius) * 0.5f);
                float error = length - (contactRadius + 0.5f * eps);

                MySmallCollPointInfo[] collInfo = MyContactInfoCache.SCPIStackAlloc();
                collInfo[0] = new MySmallCollPointInfo(p - matrix1.Translation, p - matrix2.Translation, GetRigidBody1().LinearVelocity, GetRigidBody2().LinearVelocity, n, error, p);

                MyPhysics.physicsSystem.GetContactConstraintModule().AddContactConstraint(this, collInfo, 1);

                MyContactInfoCache.FreeStackAlloc(collInfo);

                return true;
            }
            return false;
        }
    }
}
#region Using Statements
using MinerWarsMath;
#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Solver body is the support class for rigid body definition in the solver stepping
    /// </summary>
    class MyRBSolverBody
    {
        public enum SolverBodyState
        {
            SBS_Dynamic,
            SBS_Static,
            SBS_Kinematic,
        }

        public MyRBSolverBody()
        {
            Clear();
        }

        public void Clear()
        {
            m_RigidBody = null;
            m_Matrix = Matrix.Identity;

            m_LinearVelocity = Vector3.Zero;
            m_AngularVelocity = Vector3.Zero;

            m_LinearAcceleration = Vector3.Zero;
            m_AngularAcceleration = Vector3.Zero;

            m_OneOverMass = 1.0f;

            m_InertiaTensor = Matrix.Identity;
            m_InvertedInertiaTensor = Matrix.Identity;

            m_MaxAngularVelocity = MyPhysicsUtils.FLT_MAX;
            m_MaxLinearVelocity = MyPhysicsUtils.FLT_MAX;

            m_State = SolverBodyState.SBS_Dynamic;

            m_NormalHit = false;            

            m_Restitution = 1.0f;
        }

        public MyRigidBody m_RigidBody;

        public Matrix m_Matrix;

        public Vector3 m_LinearVelocity;
        public Vector3 m_AngularVelocity;

        public float m_MaxLinearVelocity;
        public float m_MaxAngularVelocity;

        public Vector3 m_LinearAcceleration;
        public Vector3 m_AngularAcceleration;

        public float m_OneOverMass;
        public Matrix m_InertiaTensor;
        public Matrix m_InvertedInertiaTensor;

        public bool m_NormalHit;
        public float m_Restitution;        

        public SolverBodyState m_State;
    }
}

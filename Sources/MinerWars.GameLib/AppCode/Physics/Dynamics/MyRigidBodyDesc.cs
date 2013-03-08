#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Rigid body descriptor
    /// </summary>
    class MyRigidBodyDesc
    {
        #region members

        public Matrix               m_Matrix;

        public float                m_Mass;

        public Vector3              m_CenterOfMass;

        public RigidBodyFlag        m_Flags;

        public ushort               m_Type;

        public uint                 m_IterationCount;

        public float                m_MaxAngularVelocity;
        public float                m_MaxLinearVelocity;
        public float                m_SleepEnergyThreshold;
        public float                m_LinearDamping;
        public float                m_AngularDamping;

        #endregion

        public MyRigidBodyDesc()
        {
            m_Type = 0;

            m_Matrix = Matrix.Identity;

            m_Mass = 1.0f;

            m_CenterOfMass = Vector3.Zero;

            m_Flags = 0;

            m_IterationCount = MyPhysicsConfig.DefaultIterationCount;

            m_MaxAngularVelocity = MyPhysicsConfig.DefaultMaxAngularVelocity;
            m_MaxLinearVelocity = MyPhysicsConfig.DefaultMaxLinearVelocity;
            m_SleepEnergyThreshold = MyPhysicsConfig.DefaultEnergySleepThreshold;
            m_LinearDamping = MyPhysicsConfig.DefaultLinearDamping;
            m_AngularDamping = MyPhysicsConfig.DefaultAngularDamping;
        }

        /// <summary>
        /// default rigid body settings
        /// </summary>
        public void SetToDefault()
        {
            m_Matrix = Matrix.Identity;

            m_Mass = 1.0f;

            m_CenterOfMass = Vector3.Zero;

            m_Flags = 0;

            m_IterationCount = MyPhysicsConfig.DefaultIterationCount;

            m_MaxAngularVelocity = MyPhysicsConfig.DefaultMaxAngularVelocity;
            m_MaxLinearVelocity = MyPhysicsConfig.DefaultMaxLinearVelocity;
            m_SleepEnergyThreshold = MyPhysicsConfig.DefaultEnergySleepThreshold;
            m_LinearDamping = MyPhysicsConfig.DefaultLinearDamping;
            m_AngularDamping = MyPhysicsConfig.DefaultAngularDamping;
        }


        /// <summary>
        /// validity check
        /// </summary>
        public bool IsValid()
        {
            if(m_Type > 31)
                return false;

            if(((m_Flags & RigidBodyFlag.RBF_RBO_STATIC) == 0) && m_Mass <= 0.0f)
                return false;

            if(m_IterationCount == 0)
                return false;

            return true;
        }
    }
}
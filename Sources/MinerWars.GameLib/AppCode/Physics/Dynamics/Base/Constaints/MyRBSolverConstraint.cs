#region Using Statements

using System;
using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Internal solver constraint data
    /// </summary>
    class MyRBSolverConstraint
    {
        public MyRBSolverConstraint()
        {
        }

        public void Clear()
        {
            m_SolverBody1 = null;
            m_SolverBody2 = null;

            m_RBConstraint = null;

            m_Magnitude = 0.0f;
            m_MinMagnitude = 0;
            m_MaxMagnitude = MyPhysicsUtils.FLT_MAX;

            m_Target = 0.0f;

            m_Normal = Vector3.Zero;
            m_ContactPoint = Vector3.Zero;

            m_Body1LocalPoint = Vector3.Zero;
            m_Body2LocalPoint = Vector3.Zero;

            m_Body1LocalPointCrossedNormal = Vector3.Zero;
            m_Body2LocalPointCrossedNormal = Vector3.Zero;

            m_StaticFriction = 0.0f;
            m_DynamicFriction = 0.0f;

            m_Affection = 0.0f;
        }

        public MyRBSolverBody m_SolverBody1;
        public MyRBSolverBody m_SolverBody2;

        public MyRBConstraint m_RBConstraint;

        public float m_Magnitude;
        public float m_MinMagnitude;
        public float m_MaxMagnitude;

        public float m_Target;

        public Vector3 m_Normal;        
        public Vector3 m_ContactPoint;

        public Vector3 m_Body1LocalPoint;
        public Vector3 m_Body2LocalPoint;

        public Vector3 m_Body1LocalPointCrossedNormal;
        public Vector3 m_Body2LocalPointCrossedNormal;

        public float m_StaticFriction;
        public float m_DynamicFriction;
        public float m_Restitution;

        public float m_Affection;

    };

}
#region Using Statements

using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Contact constraint between 2 rbelements
    /// </summary>
    class MyRBContactConstraint: MyRBConstraint
    {
        public const int MaxCollisionPoints = 10;
        
        private bool m_Satisfied;

        private uint m_Guid;

        private MyRBElementInteraction m_Interaction;

        public MyRBElementInteraction GetRBElementInteraction()
        {
            return m_Interaction;
        }

        public MyRBContactConstraint() { }

        #region Properties

        public float Magnitude
        {
            get { return m_Magnitude;  }
            set { m_Magnitude = value; }
        }

        public bool Satisfied
        {
            get { return m_Satisfied; }
            set { m_Satisfied = value; }
        }

        public uint GUID
        {
            get { return m_Guid; }
        }

        public MyCollPointInfo[] m_PointInfo = new MyCollPointInfo[MaxCollisionPoints];
        public int m_NumCollPts = 0;

        #endregion

        /// <summary>
        /// Initializes the constraint and creates the guid for contact reporting
        /// </summary>
        public void Init(MyRBElementInteraction interaction, MySmallCollPointInfo[] pointInfos, int numPointInfos)
        {
            this.m_Interaction = interaction;

            m_Magnitude = 0.0f;

            numPointInfos = (numPointInfos > MaxCollisionPoints) ? MaxCollisionPoints : numPointInfos;

            m_NumCollPts = 0;
            MyContactConstraintModule mod = MyPhysics.physicsSystem.GetContactConstraintModule();
            for (int i = 0; i < numPointInfos; ++i)
            {
                this.m_PointInfo[m_NumCollPts] = mod.PopCollPointInfo();
                this.m_PointInfo[m_NumCollPts++].Init(ref pointInfos[i]);
            }

            uint guid1 = (uint) interaction.RBElement1.GUID;
            uint guid2 = (uint) interaction.RBElement2.GUID;

            if (guid1 > guid2)
            {
                uint tm = guid2;
                guid2 = guid1;
                guid1 = tm;
            }

            m_Guid = guid1 + (guid2 << 16);
        }

        // public List<CollPointInfo>

        public void Destroy()
        {
            MyContactConstraintModule mod = MyPhysics.physicsSystem.GetContactConstraintModule();
            for (int i = 0; i < m_NumCollPts; ++i)
            {
                mod.PushCollPointInfo(this.m_PointInfo[i]);                
            }
        }

    }



}
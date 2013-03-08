#region Using Statements

using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////

    enum MyContactEventType
    {
        CET_START = 1,        
        CET_END = 2,
        CET_START_AND_END = 3,
        CET_TOUCH = 4,
        CET_START_AND_TOUCH = 5,
        CET_START_AND_TOUCH_AND_END = 6,
        CET_END_AND_TOUCH = 7,
    };

    //////////////////////////////////////////////////////////////////////////

	class MyContactEventInfo
	{

        public void Fill(MyRBContactConstraint cc)
        {
            m_RigidBody1 = cc.GetRBElementInteraction().GetRigidBody1();
            m_RigidBody2 = cc.GetRBElementInteraction().GetRigidBody2();

            m_Element1 = cc.GetRBElementInteraction().RBElement1;
            m_Element2 = cc.GetRBElementInteraction().RBElement2;

            m_ContactNormal = cc.m_PointInfo[0].m_Info.m_Normal;
            m_ContactPoint = cc.m_PointInfo[0].m_Info.m_WorldPosition;

            m_Velocity1 = cc.m_PointInfo[0].m_Info.m_V0;
            m_Velocity2 = cc.m_PointInfo[0].m_Info.m_V1;

            m_Guid = cc.GUID;
        }

		public MyRigidBody m_RigidBody1;
        public MyRigidBody m_RigidBody2;
        public MyRBElement m_Element1;
        public MyRBElement m_Element2;
        public Vector3 m_ContactPoint;
        public Vector3 m_ContactNormal;
        public Vector3 m_Velocity1;
        public Vector3 m_Velocity2;
        public uint m_Guid; //guid of the constraint
	};

    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Provides notifications about contacts from physic subsystem.
    /// </summary>
    interface IMyNotifyContact
    {
        /// <summary>
        /// Called when [contact start].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        void OnContactStart(MyContactEventInfo contactInfo);

        /// <summary>
        /// Called when [contact end].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        void OnContactEnd(MyContactEventInfo contactInfo);

        /// <summary>
        /// Called when [contact touch].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        void OnContactTouch(MyContactEventInfo contactInfo);
    }
}
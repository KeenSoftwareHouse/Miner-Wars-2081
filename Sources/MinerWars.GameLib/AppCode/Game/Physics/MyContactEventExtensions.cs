using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Physics
{
    public static class MyContactEventExtensions
    {
        /// <summary>
        /// Gets the other entity from contact.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="sourceEntity">Entity that we knows and for wich we want opposite entity.</param>
        /// <returns></returns>
        internal static MyEntity GetOtherEntity(this MyContactEventInfo eventInfo, MyEntity sourceEntity)
        {
            MyPhysicsBody ps1 = (MyPhysicsBody)eventInfo.m_RigidBody1.m_UserData;
            MyPhysicsBody ps2 = (MyPhysicsBody)eventInfo.m_RigidBody2.m_UserData;

            if (ps1.Entity == sourceEntity)
            {
                return ps2.Entity;
            }

            return ps1.Entity;
        }

        /// <summary>
        /// Gets the other entity from contact.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="sourceEntity">Entity that we knows and for wich we want opposite entity.</param>
        /// <returns></returns>
        internal static MyEntity GetOtherEntity(this MyRBSolverConstraint eventInfo, MyEntity sourceEntity)
        {
            MyPhysicsBody ps1 = (MyPhysicsBody)eventInfo.m_SolverBody1.m_RigidBody.m_UserData;
            MyPhysicsBody ps2 = (MyPhysicsBody)eventInfo.m_SolverBody2.m_RigidBody.m_UserData;

            if (ps1.Entity == sourceEntity)
            {
                return ps2.Entity;
            }

            return ps1.Entity;
        }
    }
}
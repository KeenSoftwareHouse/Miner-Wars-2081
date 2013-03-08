#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Internal filtering using the layer and mask test.
    /// </summary>
    static class MyFiltering
    {
        static public bool AcceptCollision(MyRBElement el0,MyRBElement el1)
        {
            if (!MyPhysics.physicsSystem.GetRigidBodyModule().IsEnabledCollisionInLayers(el0.CollisionLayer,el1.CollisionLayer))
            {
                return false;
            }

            MyGroupMask mask0 = el0.GroupMask;
            MyGroupMask mask1 = el1.GroupMask;

            if ((mask0.m_Mask0 & mask1.m_Mask0) > 0)
            {
                return false;
            }

            if ((mask0.m_Mask1 & mask1.m_Mask1) > 0)
            {
                return false;
            }

            if ((mask0.m_Mask2 & mask1.m_Mask2) > 0)
            {
                return false;
            }

            if ((mask0.m_Mask3 & mask1.m_Mask3) > 0)
            {
                return false;
            }

            return true;
        }
    }
}
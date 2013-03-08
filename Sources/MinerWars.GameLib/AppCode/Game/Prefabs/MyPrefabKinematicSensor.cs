using MinerWars.AppCode.Physics;
using System.Diagnostics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    class MyPrefabKinematicSensor : IMySensorEventHandler
    {
        int m_counter = 0;
        MyPrefabKinematic m_owner;

        /// <summary>
        /// Called when rigid body enters sensor.
        /// </summary>
        /// <param name="rbo">Rigid body that entered.</param>
        public void OnEnter(MySensor sensor, MyRigidBody rbo, MyRBElement rbElement)
        {
            //smallship
            var userData = rbo.m_UserData;
            var physicsBody = userData as MyPhysicsBody;
            if (physicsBody != null && physicsBody.Entity is MySmallShip)
            {
                m_counter++;
                if (m_counter > 0)
                    m_owner.OrderToOpen();
            }
        }

        /// <summary>
        /// Called when rigid body leaves sensor.
        /// </summary>
        /// <param name="rbo">Rigid body that left.</param>
        public void OnLeave(MySensor sensor, MyRigidBody rbo, MyRBElement rbElement)
        {
            ////TODO: Temporary solution - there must not be rbo==null, fix the error and change to assert
            //if (rbo == null)
            //    return;
            Debug.Assert(rbo != null);
            //smallship
            var userData = rbo.m_UserData;
            var physicsBody = userData as MyPhysicsBody;
            if (physicsBody != null && physicsBody.Entity is MySmallShip)
            {
                m_counter--;
                if (m_counter <= 0)
                    m_owner.OrderToClose();
            }
        }

        public MyPrefabKinematicSensor(MyPrefabKinematic owner)
        {
            m_owner = owner;
            m_counter = 0;
        }

        public int GetDetectedEntitiesCount()
        {
            return m_counter;
        }
    }
}

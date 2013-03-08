#region Using Statements

using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Provides notifications about sensor events from physic subsystem.
    /// </summary>
    interface IMySensorEventHandler     
    {
        /// <summary>
        /// Called when rigid body enters sensor.
        /// </summary>
        /// <param name="rbo">Rigid body that entered.</param>
        void OnEnter(MySensor sensor,MyRigidBody rbo, MyRBElement rbElement);

        /// <summary>
        /// Called when rigid body leaves sensor.
        /// </summary>
        /// <param name="rbo">Rigid body that left.</param>
        void OnLeave(MySensor sensor,MyRigidBody rbo, MyRBElement rbElement);
    }
}

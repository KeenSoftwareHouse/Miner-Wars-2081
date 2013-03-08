#region Using Statements

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////

    //abstract  class  MyRigidBodyEvent
    //{
    //   public virtual void OnActivated() {}
    //   public virtual void OnDeactivated() {}

    //   public MyRigidBody GetRigidBody() { return m_EventRbo; }

       
    //   private MyRigidBodyEvent(MyRigidBody rbo) { m_EventRbo = rbo; }
    
    //   private MyRigidBody m_EventRbo;
    //};

    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Provides notification on motion change by physics sustem.
    /// </summary>
    interface IMyNotifyMotion
    {
        /// <summary>
        /// Called when [motion].
        /// </summary>
        /// <param name="rbo">The rbo.</param>
        /// <param name="step">The step.</param>
        void OnMotion(MyRigidBody rbo, float step);
    };


    delegate void MyRigidBodyEvent();
}
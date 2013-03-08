#region Using Statements

using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Base element interaction
    /// </summary>
    abstract class MyRBElementInteraction: ParallelTasks.IWork
    {
        // These are to be overriden for each element-element interaction type.
        protected virtual bool Interact(bool staticCollision) { return false; }
        public virtual MyRBElementInteraction CreateNewInstance() { return null; }

        public MyRBElement RBElement1 { get { return this.m_Element1; } set { this.m_Element1 = value; } }
        public MyRBElement RBElement2 { get { return this.m_Element2; } set { this.m_Element2 = value; } }

        public MyRigidBody GetRigidBody1() { return this.m_Element1.GetRigidBody(); }
        public MyRigidBody GetRigidBody2() { return this.m_Element2.GetRigidBody(); }

        public void SwapElements()
        {
            MyRBElement tempEl = m_Element2;
            m_Element2 = m_Element1;
            m_Element1 = tempEl;
        }

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        private MyRBElement m_Element1;
        private MyRBElement m_Element2;

        public void DoWork() { Interact(false); }
        public bool DoStaticInitialTest() { return Interact(true); }
    }
}
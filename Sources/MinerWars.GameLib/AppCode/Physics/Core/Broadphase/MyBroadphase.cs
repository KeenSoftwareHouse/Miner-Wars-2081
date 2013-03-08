#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////

    enum MyBrodphaseType
    {
        BT_BRUTE_FORCE,
        BT_DAABB,
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// abstract class for base broadphase
    /// </summary>
    abstract class MyBroadphase: ParallelTasks.IWork
    {
        public virtual void CreateVolume(MyElement element) { }
        public virtual void DestroyVolume(MyElement element) { }
        public virtual void MoveVolume(MyElement element) { }

        public virtual void MoveVolumeFast(MyElement element) { }        

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        public virtual void DoWork() { }

        public virtual List<MyRBElementInteraction> GetRBElementInteractionList() { return null; }

        public virtual void Close() { }
    }
}
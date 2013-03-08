#region Using Statements
using System.Collections.Generic;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Checks the rigid body island if its possible to put the island sleep. Only the whole island can go sleep. If some rigid is still active the island cannot be put to sleep.
    /// </summary>
    class MyRigidBodyIslandSleepState : ParallelTasks.IWork
    {
        public MyRigidBodyIslandSleepState()
        {
        }

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        public MyBroadphase m_Broadphase;

        public void DoWork()
        {
            UpdateSleepState();
        }

        /// <summary>
        /// Checks the sleep state of rigids and decides if its possible to sleep the whole island
        /// </summary>
        private void UpdateSleepState()
        {
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIslands");

            List<MyRigidBodyIsland> islands = MyPhysics.physicsSystem.GetRigidBodyModule().GetRigidBodyIslandGeneration().GetIslands();

            float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Loop Islands");

            int count = 0;

            for (int i = 0; i < islands.Count; i++)
            {
                MyRigidBodyIsland island = islands[i];
                bool canDeactivate = true;

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Loop Rigids");

                for (int j = 0; j < island.GetRigids().Count; j++)
                {
                    count++;

                    MyRigidBody rbo = island.GetRigids()[j];
                    if (rbo.IsStatic())
                    {
                        rbo.PutToSleep();
                        MyPhysics.physicsSystem.GetRigidBodyModule().RemoveActiveRigid(rbo);
                    }
                    else
                    {
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MoveVolumeFast");

                        foreach(var el in rbo.GetRBElementList())
                        {
                            m_Broadphase.MoveVolumeFast(el);
                        }

                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("NotifyMotionHandler");

                        if (rbo.NotifyMotionHandler != null)
                        {
                            rbo.NotifyMotionHandler.OnMotion(rbo, dt);
                        }

                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }

                    // dynamic rigids use different approach
                    if(!rbo.CanDeactivate())
                    {
                        canDeactivate = false;                        
                    }
                }

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("canDeactivate");
                // check if all are asleep
                if(canDeactivate)
                {
                    foreach (var rbo in island.GetRigids())
                    {
                        rbo.PutToSleep();
                        MyPhysics.physicsSystem.GetRigidBodyModule().RemoveActiveRigid(rbo);
                    }
                }
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("UpdateSleepState", count);
        }        
    }

}
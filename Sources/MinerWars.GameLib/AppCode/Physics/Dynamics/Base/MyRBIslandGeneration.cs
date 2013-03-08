#region Using Statements
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Generates islands from active rigids and constraits list. This island is solved on its own solver later. 
    /// </summary>
    class MyRigidBodyIslandGeneration: ParallelTasks.IWork
    {
        public MyRigidBodyIslandGeneration()
        {
            m_islands = new List<MyRigidBodyIsland>(128);
            m_islandsPool = new MyObjectsPool<MyRigidBodyIsland>(256);
            m_proccesedList = new HashSet<MyRigidBody>();
            m_activeRigids = new List<MyRigidBody>(128);
        }

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        public void DoWork()
        {
            GenerateIslands();
        }

        /// <summary>
        /// Parsing all rigids
        /// </summary>
        private void GenerateIslands()
        {            
            m_islands.Clear();         
            m_islandsPool.DeallocateAll();
            m_proccesedList.Clear();
            m_activeRigids.Clear();

            HashSet <MyRigidBody> activeRbos = MyPhysics.physicsSystem.GetRigidBodyModule().GetActiveRigids();

            foreach (MyRigidBody rbo in activeRbos)
            {
                m_activeRigids.Add(rbo);
            }

            foreach (MyRigidBody rbo in m_activeRigids)
            {
                AddRigidBody(rbo, null, null);
            }

            m_activeRigids.Clear();
        }

        /// <summary>
        /// Adding rigid body recursively to check for constraint connections and make sure that its only in 1 island
        /// </summary>
        private void AddRigidBody(MyRigidBody rbo, MyRigidBody secondRigidBody,MyRigidBodyIsland addIsland)
        {
            if(rbo.IsStatic())
            {
                rbo.PutToSleep();
                return;
            }

            if (!m_proccesedList.Add(rbo))
                return;

            // add rigid bodies to island recursively
            int numInteractions = 0;

            for (int j = 0; j < rbo.GetRBElementList().Count; j++)
            {
                MyRBElement el = rbo.GetRBElementList()[j];
                numInteractions += el.GetRBElementInteractions().Count;
                
                for (int k = 0; k < el.GetRBElementInteractions().Count; k++)
                {
                    if (addIsland == null && !rbo.IsStatic())
                    {
                        addIsland = m_islandsPool.Allocate();
                        addIsland.Clear();
                        addIsland.IterationCount = 0;
                        addIsland.AddRigidBody(rbo);

                        m_islands.Add(addIsland);                        
                    }
                    else
                    {
                        if(!rbo.IsStatic())
                        {
                            addIsland.AddRigidBody(rbo);
                        }
                    }

                    MyRBElementInteraction intr = el.GetRBElementInteractions()[k];

                    if(intr.GetRigidBody1() != rbo && intr.GetRigidBody2() != secondRigidBody)
                    {
                        AddRigidBody(intr.GetRigidBody1(),rbo,addIsland);
                    }

                    if (intr.GetRigidBody2() != rbo && intr.GetRigidBody1() != secondRigidBody)
                    {
                        AddRigidBody(intr.GetRigidBody2(), rbo, addIsland);
                    }
                }
            }

            // isolated rbo
            if (numInteractions == 0 && !rbo.IsStatic())
            {
                MyRigidBodyIsland island = m_islandsPool.Allocate();
                island.Clear();
                island.IterationCount = 0;
                island.AddRigidBody(rbo);

                m_islands.Add(island);
            }
        }

        public List<MyRigidBodyIsland> GetIslands()
        {
            return m_islands;
        }

        /// <summary>
        /// Consistancy check
        /// </summary>
        public bool CheckIslands()
        {
            for (int j = 0; j < m_islands.Count; j++)
            {
                for (int i = 0; i < m_islands[j].GetRigids().Count; i++)
                {
                    MyRigidBody rbo = m_islands[j].GetRigids()[i];

                    // check the if rigid is in only 1 island
                    for (int k = 0;k  < m_islands.Count; k++)
                    {
                        if(k != j)
                        {
                            for (int l = 0; l < m_islands[k].GetRigids().Count; l++)
                            {
                                MyRigidBody testRbo = m_islands[k].GetRigids()[l];
                                if(testRbo == rbo)
                                {
                                    MyCommonDebugUtils.AssertDebug(false);
                                    return false;
                                }
                            }
                            
                        }                        
                    }
                }
            }
            return true;
        }

        private MyObjectsPool<MyRigidBodyIsland> m_islandsPool;
        private List<MyRigidBodyIsland>         m_islands;
        private HashSet<MyRigidBody>            m_proccesedList;
        private List<MyRigidBody>               m_activeRigids;
    }

}
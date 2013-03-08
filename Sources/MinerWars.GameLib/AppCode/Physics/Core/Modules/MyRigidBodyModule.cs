#region Using Statements

using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
using ParallelTasks;

using System.Diagnostics;
using SysUtils;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Utils;
using System.Linq;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    using System;

    //////////////////////////////////////////////////////////////////////////

    class MyRigidBodyModule
    {
        static MyRigidBodyModule()
        {
            m_ActiveRigids = new HashSet<MyRigidBody>();
            //m_Rigids = new List<MyRigidBody>(MyPhysicsConfig.MaxCollidingElements);
            m_SolverPool = new MyObjectsPool<MyRBIslandNSolver>(2 * MyPhysicsConfig.MaxCollidingElements);
            m_SolverList = new List<MyRBIslandNSolver>(2 * MyPhysicsConfig.MaxCollidingElements);
        }

        /// <summary>
        /// rigid body module stores all simulated rigids. If you want to simulate a rigid you have to insert it in this module.
        /// </summary>
        public MyRigidBodyModule()
        {
            m_ActiveRigids.Clear();
            //m_Rigids.Clear();

            m_BroadPhase = new MyDynamicAABBTreeBroadphase();

            m_PruningStructure = ((MyDynamicAABBTreeBroadphase) m_BroadPhase).GetDAABBTree();

           // m_DAABBTreeUpdater = new MyDAABBTreeUpdater(((MyDynamicAABBTreeBroadphase)m_BroadPhase).GetDAABBTree());

            m_RboIslandGeneration = new MyRigidBodyIslandGeneration();

            m_CollisionEpsilon = MyPhysicsConfig.CollisionEpsilon;
            m_CurrentTimeStep = 0.02f;

            m_GroupMaskManager = new MyGroupMaskManager();

            m_SolverPool.DeallocateAll();
            m_SolverList.Clear();

            m_DeactivationSolver = new MyRigidBodyIslandSleepState();

            m_DeactivationSolver.m_Broadphase = m_BroadPhase;


            m_SolverAction = SolverAction;// idx => m_SolverList[idx].DoWork();

            m_InteractionAction = InteractionAction; // idx => m_InteractionsList[idx].DoWork();
        }

        private void SolverAction(int idx)
        {
            m_SolverList[idx].DoWork();
        }

        private void InteractionAction(int idx)
        {
            m_InteractionsList[idx].DoWork();
        }

        public float CollisionEpsilon { get { return m_CollisionEpsilon; } set { m_CollisionEpsilon = value; } }
        public float CurrentTimeStep { get { return m_CurrentTimeStep; } set { m_CurrentTimeStep = value; } }

        public bool Insert(MyRigidBody rbo)
        {
            if(rbo.ReadFlag(RigidBodyFlag.RBF_INSERTED))
            {
                return true;
            }

#if PHYSICS_CHECK
            for (int i = 0; i < m_Rigids.Count; i++)
            {
                MyRigidBody r = m_Rigids[i];
                if (r == rbo)
                {
                    // rbo already inserted!
                    MyCommonDebugUtils.AssertDebug(false);
                    return false;
                }
            }            
#endif
            MyCommonDebugUtils.AssertDebug(rbo != null);

           // m_Rigids.Add(rbo);
            
            // insert to sort into bp
            for (int i = 0; i < rbo.GetRBElementList().Count; i++)
            {
                MyRBElement elem = rbo.GetRBElementList()[i];
                elem.GroupMask = MyGroupMask.Empty;
                elem.UpdateAABB();
                m_BroadPhase.CreateVolume(elem);
            }

            if (!rbo.ReadFlag(RigidBodyFlag.RBF_RBO_STATIC))
            {
                AddActiveRigid(rbo);
                rbo.ActivateNotification();
            }

            rbo.RaiseFlag(RigidBodyFlag.RBF_INSERTED);
            return true;
        }

        public void Remove(MyRigidBody rbo)
        {
            //Debug.Assert(!MyPhysics.physicsSystem.GetSensorInteractionModule().IsCheckInteractionsActive(), "You can't deactivate rigid body when check sensor's interactions is active!");

            if (rbo == null)
            {
                return;
            }

            for (int i = 0; i < rbo.GetRBElementList().Count; i++)
            {
                MyRBElement elem = rbo.GetRBElementList()[i];
                m_BroadPhase.DestroyVolume(elem);
            }

            RemoveActiveRigid(rbo);            
            rbo.DeactivateNotification();

           // m_Rigids.Remove(rbo);

            rbo.ClearFlag(RigidBodyFlag.RBF_INSERTED);
        }

        /// <summary>
        /// main internal simulation functions calling all physics substeps
        /// </summary>
        public void Simulate(float timeStep)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Flush");

            if (timeStep != 0)
            {
                CurrentTimeStep = timeStep;
            }

            Flush();

            MyPhysics.physicsSystem.GetSensorInteractionModule().Flush();

            // We must move PreparSensorInteraction from here after BroadPhaseUpdate, because we must know if sensor is active or not
            //MyPhysics.physicsSystem.GetSensorModule().PrepareSensorInteractions();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("HandleSensorChanges");
            MyPhysics.physicsSystem.GetSensorModule().HandleSensorChanges();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BroadPhaseUpdate");

            BroadPhaseUpdate(timeStep);
            
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();            

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PrepareSensorInteractions");
            MyPhysics.physicsSystem.GetSensorModule().PrepareSensorInteractions();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            if (timeStep != 0)
            {

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("NearPhaseUpdate");

                NearPhaseUpdate(timeStep);
                
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateInteractions");

                MyPhysics.physicsSystem.GetSensorInteractionModule().UpdateInteractions(timeStep);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("RboIslandGeneration");

                RboIslandGeneration();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Solve");
                Solve(timeStep);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SolveDeactivation");
                SolveDeactivation(timeStep);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CheckInteractions");
                MyPhysics.physicsSystem.GetSensorInteractionModule().CheckInteractions(timeStep);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ParseSensorInteractions");
                MyPhysics.physicsSystem.GetSensorModule().ParseSensorInteractions();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            }
            else
            {
                //force put to sleep
                List<MyRigidBody> activeRigids = m_ActiveRigids.ToList();
                foreach (MyRigidBody rbo in activeRigids)
                {
                    rbo.PutToSleep();
                    MyPhysics.physicsSystem.GetRigidBodyModule().RemoveActiveRigid(rbo);
                }

                MyPhysics.physicsSystem.GetSensorInteractionModule().PutCurrentInteractionsBack();
            }
                
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FireContactCallbacks");
            FireContactCallbacks(timeStep);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// clears data from last computations
        /// </summary>
        private void Flush()
        {            
            m_SolverPool.DeallocateAll();
        }

        /// <summary>
        /// Fires contact callbacks
        /// </summary>
        private void FireContactCallbacks(float dt)
        {
            MyPhysics.physicsSystem.GetContactConstraintModule().Flush();
        }

        /// <summary>
        /// Solves the deactivation of rigid body islands
        /// </summary>
        private void SolveDeactivation(float dt)
        {
            m_DeactivationSolver.DoWork();
            //Task dtask = Parallel.Start(m_DeactivationSolver);
            //dtask.Wait();
        }

        /// <summary>
        /// runs multithreaded solvers for each rigid body island
        /// </summary>
        private void Solve(float dt)
        {
            List<MyRigidBodyIsland> islands = m_RboIslandGeneration.GetIslands();

            foreach(var island in islands)
            {
                MyRBIslandNSolver solver = m_SolverPool.Allocate();
                solver.SetRBIsland(island);
                m_SolverList.Add(solver);
            }

#if RENDER_PROFILING
            
            for (int i = 0; i < m_SolverList.Count; i++)
            {
                m_SolverList[i].DoWork();
                //m_SolverAction[i](m_SolverList[i]);
            } 
            //Parallel.For(0, m_SolverList.Count, m_SolverAction, 1);
#else
			//PM: I am suspicous about peaks here in Parallel
            Parallel.For(0, m_SolverList.Count, m_SolverAction, 1);
/*
            for (int i = 0; i < m_SolverList.Count; i++)
            {
                m_SolverList[i].DoWork();
                //m_SolverAction[i](m_SolverList[i]);
            } */
#endif

            //System.Threading.Tasks.Parallel.For(0, m_SolverList.Count, m_SolverAction);
          
            m_SolverList.Clear();
        }
              /*
        /// <summary>
        /// start the rebalancing of the dynamic aabb tree
        /// </summary>
        private  void ReBalanceTreeStart()
        {            
            m_DAABBTreeTask = Parallel.StartBackground(m_DAABBTreeUpdater);
        }
            */
               /*
        /// <summary>
        /// stops the rebalancing of the dynamic aabb tree
        /// </summary>
        private void ReBalanceTreeStop()
        {
            m_DAABBTreeTask.Wait();
        }
                 */
        /// <summary>
        /// runs the rbo island generation - single threaded
        /// </summary>
        protected  void RboIslandGeneration()
        {
            m_RboIslandGeneration.DoWork();
            //Task rboTask = Parallel.Start(m_RboIslandGeneration);
            //rboTask.Wait();

            // A.B. consistency check
            //m_RboIslandGeneration.CheckIslands();
        }

        /// <summary>
        ///  updates the broadphase (AABB vs AABB detection) - single threaded
        /// </summary>
        protected void BroadPhaseUpdate(float timeStep)
        {
            m_BroadPhase.DoWork();
            //Task bpTask = Parallel.Start(m_BroadPhase);
            //bpTask.Wait();
        }

        string ParseLastElement(string text)
        {
            string result = text.ToString();
            int dotIndex = result.LastIndexOf('.');
            if (result.Length - dotIndex - 1 > 0)
                result = result.Substring(dotIndex + 1, result.Length - dotIndex - 1);

            return result;
        }

        /// <summary>
        /// precise collision detection based on data from broadphase. Collision detection runs multithreaded.
        /// </summary>
        protected void NearPhaseUpdate(float timeStep)
        {
            m_InteractionsList = m_BroadPhase.GetRBElementInteractionList();
            
            if (MyFakes.DEBUG_DRAW_COLLIDING_ENTITIES)
            {
                bool logToConsole = false;
                bool debugDraw = true;
                bool drawText = true;
                float textSize = 0.7f;

                Vector2 screenPos = new Vector2(100, 100);

                if (logToConsole) Console.WriteLine("-----------------------------");

                Vector4 color = new Vector4(1, 0, 0, 1);

                foreach (MyRBElementInteraction i in m_InteractionsList)
                {
                    object obj = i.GetRigidBody1().m_UserData;
                    if (obj is MyPhysicsBody)
                    {
                        MyPhysicsBody rb = (MyPhysicsBody)obj;
                        string name = rb.Entity.ToString() + " " + i.GetRigidBody1().GUID;

                        if (debugDraw) MyDebugDrawCachedLines.AddAABB(rb.Entity.WorldAABB, Color.Red);

                        if (drawText)
                        {
                            System.Text.StringBuilder sb0 = new System.Text.StringBuilder(100);
                            sb0.Append(ParseLastElement(i.ToString()));
                            MyDebugDraw.TextBatch.AddText(screenPos, sb0, Color.Red, textSize);
                            screenPos.Y += 20 * textSize;

                            System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
                            sb.Append(ParseLastElement(name));
                            MyDebugDraw.TextBatch.AddText(screenPos, sb, Color.YellowGreen, textSize);
                            screenPos.Y += 20 * textSize;
                        }

                        if (logToConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(name);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }

                    object obj2 = i.GetRigidBody2().m_UserData;
                    if (obj2 is MyPhysicsBody)
                    {
                        MyPhysicsBody rb2 = (MyPhysicsBody)obj2;
                        string name2 = rb2.Entity.ToString() + " " + i.GetRigidBody2().GUID;

                        if (debugDraw) MyDebugDrawCachedLines.AddAABB(rb2.Entity.WorldAABB, Color.Red);

                        if (drawText)
                        {
                            System.Text.StringBuilder sb2 = new System.Text.StringBuilder(100);
                            sb2.Append(ParseLastElement(name2));

                            MyDebugDraw.TextBatch.AddText(screenPos, sb2, Color.LightBlue, textSize);
                            screenPos.Y += 20 * textSize;
                        }

                        if (logToConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(name2);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }

                    if (drawText) screenPos.Y += 10 * textSize;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Processing interactions");

#if RENDER_PROFILING
            
            foreach (MyRBElementInteraction i in m_InteractionsList)
            {
                i.DoWork();
            } 
            //Parallel.For(0, m_InteractionsList.Count, m_InteractionAction, 1);
#else
            /*foreach (MyRBElementInteraction i in m_InteractionsList)
            {
                i.DoWork();
            } */
            //Peaks
            //System.Threading.Tasks.Parallel.For(0, m_InteractionsList.Count, m_InteractionAction);
            Parallel.For(0, m_InteractionsList.Count, m_InteractionAction, 1);
#endif
              
                  /*
            Dictionary<MinerWars.AppCode.Game.Entities.MyEntity, HashSet<MinerWars.AppCode.Game.Entities.MyEntity>> test = new Dictionary<Game.Managers.EntityManager.Entities.MyEntity, HashSet<Game.Managers.EntityManager.Entities.MyEntity>>();

            foreach (MyRBElementInteraction i in m_InteractionsList)
            {
                MinerWars.AppCode.Game.Entities.MyEntity e1 = ((MyPhysicsBody)i.GetRigidBody1().m_UserData).Entity;
                MinerWars.AppCode.Game.Entities.MyEntity e2 = ((MyPhysicsBody)i.GetRigidBody2().m_UserData).Entity;

                if (test.ContainsKey(e1))
                {
                    if (!test[e1].Add(e2))
                    {
                    }
                }
                else
                {
                    HashSet<Game.Managers.EntityManager.Entities.MyEntity> hs = new HashSet<Game.Managers.EntityManager.Entities.MyEntity>();
                    hs.Add(e2);
                    test.Add(e1, hs);
                }
                if (test.ContainsKey(e2))
                {
                    if (!test[e2].Add(e1))
                    {
                    }
                }
                else
                {
                    HashSet<Game.Managers.EntityManager.Entities.MyEntity> hs = new HashSet<Game.Managers.EntityManager.Entities.MyEntity>();
                    hs.Add(e1);
                    test.Add(e2, hs);

                }
            }   */


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            m_InteractionsList.Clear();
        }

        public void Initialize()
        {
            m_EventTypes = new MyContactEventType[32,32];

            MyContactEventType defaultContactTypeMask = MyContactEventType.CET_START_AND_END;

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    m_EventTypes[i, j] = defaultContactTypeMask;
                    m_EventTypes[j, i] = defaultContactTypeMask;
                }
            }

            m_CollisionLayers = new bool[32,32];
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if(i == j)
                        m_CollisionLayers[i, j] = true;
                    else
                        m_CollisionLayers[i, j] = false;
                }
            }

        }

        public void Destroy()
        {
            // release references to entities
            m_ActiveRigids.Clear();
            //m_Rigids.Clear();

            foreach (var item in m_SolverPool.GetPreallocatedItemsArray())
            {
                item.Value.Clear();
            }

            m_SolverPool.DeallocateAll();
            m_SolverList.Clear();

            m_BroadPhase.Close();

            System.Diagnostics.Debug.Assert(((MyDynamicAABBTreeBroadphase)m_BroadPhase).GetDAABBTree().GetHeight() == 0, "Something has left in AABB, AABB should be clear");
              /*
            MyDynamicAABBTree tree = ((MyDynamicAABBTreeBroadphase)m_BroadPhase).GetDAABBTree();
            List<MyElement> lis = new List<MyElement>();
            tree.GetAll(lis,true);
                */
                   /*
            // TODO: These clears should NOT be necessary! (ticket 5658)
            {
                m_BroadPhase = new MyDynamicAABBTreeBroadphase();
                m_PruningStructure = ((MyDynamicAABBTreeBroadphase)m_BroadPhase).GetDAABBTree();
                m_DeactivationSolver = new MyRigidBodyIslandSleepState();
                m_DeactivationSolver.m_Broadphase = m_BroadPhase;
            }        */
        }

        /// <summary>
        /// sets a mask for contact events for 2 rigid body types 
        /// </summary>
        public void SetRigidBodyContactEventTypeMask(MyContactEventType ev, ushort type1, ushort type2)
        {
            m_EventTypes[type1, type2] = ev;
            m_EventTypes[type2, type1] = ev;
        }

        public MyContactEventType GetRigidBodyContactEventTypeMask(ushort type1, ushort type2)
        {
            return m_EventTypes[type1,type2];
        }

 

        public HashSet<MyRigidBody> GetActiveRigids() { return m_ActiveRigids; }

        /// <summary>
        /// internal add of to active rigid list
        /// </summary>
        public void AddActiveRigid(MyRigidBody rbo)
        {
            if(rbo == null)
                return;

            if(rbo.ReadFlag(RigidBodyFlag.RBF_ACTIVE))
                return;

            //if (rbo.RigidBodyEventHandler != null)
            //{
            //    rbo.RigidBodyEventHandler.OnActivated();
            //}            

            rbo.RaiseFlag(RigidBodyFlag.RBF_ACTIVE);

            m_ActiveRigids.Add(rbo);
        }

        public void RemoveActiveRigid(MyRigidBody rbo)
        {
            if (rbo == null)
                return;

            if (!rbo.ReadFlag(RigidBodyFlag.RBF_ACTIVE))
                return;

            //if (rbo.RigidBodyEventHandler != null)
            //{
            //    rbo.RigidBodyEventHandler.OnDeactivated();
            //}            

            rbo.ClearFlag(RigidBodyFlag.RBF_ACTIVE);

            if (rbo.ReadFlag(RigidBodyFlag.RBF_KINEMATIC))
            {
                List<MyRigidBodyIsland> islands = MyPhysics.physicsSystem.GetRigidBodyModule().GetRigidBodyIslandGeneration().GetIslands();

                for (int i = 0; i < islands.Count; i++)
                {
                    MyRigidBodyIsland island = islands[i];

                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Loop Rigids");

                    for (int j = 0; j < island.GetRigids().Count; j++)
                    {
                        MyRigidBody rboi = island.GetRigids()[j];
                        if (rbo == rboi)
                        {
                            island.RemoveRigidBody(rbo);
                            break;
                        }
                    }
                }
            }

            bool found = m_ActiveRigids.Remove(rbo);
            
            //RBO wasnt in the list
            Debug.Assert(found);
        }

        public MyBroadphase GetBroadphase() { return m_BroadPhase; }

        /// <summary>
        /// defines if 2 layes can or cannot collide
        /// </summary>
        public void EnableCollisionInLayers(System.UInt16 layer0, System.UInt16 layer1, bool enable)
        {
            m_CollisionLayers[layer0, layer1] = enable;
            m_CollisionLayers[layer1, layer0] = enable;
        }

        public void EnableCollisionInLayers(System.UInt16 layer, bool enable)
        {
            for (int i = 0; i < 32; i++)
            {
                m_CollisionLayers[i, layer] = enable;
                m_CollisionLayers[layer, i] = enable;
            }
        }

        public bool IsEnabledCollisionInLayers(System.UInt16 layer0, System.UInt16 layer1)
        {
            return m_CollisionLayers[layer0,layer1];
        }

        public MyRigidBodyIslandGeneration GetRigidBodyIslandGeneration()
        {
            return m_RboIslandGeneration;
        }

        /// <summary>
        /// returns group mask manager to get group masks
        /// </summary>
        public MyGroupMaskManager GetGroupMaskManager()
        {
            return m_GroupMaskManager;
        }

        public MyDynamicAABBTree GetPruningStructure() { return m_PruningStructure; }

        #region members
        private MyContactEventType[,]   m_EventTypes;
        private bool[,] m_CollisionLayers;

        private static HashSet<MyRigidBody> m_ActiveRigids;
        private static List<MyRigidBody> m_Rigids;
        private MyBroadphase m_BroadPhase;
        private float m_CollisionEpsilon;
        private float m_CurrentTimeStep = 0.02f;

        private MyGroupMaskManager m_GroupMaskManager;

        private MyRigidBodyIslandGeneration m_RboIslandGeneration;

        private MyDynamicAABBTree m_PruningStructure;

        // in case of DAABB
        //private MyDAABBTreeUpdater m_DAABBTreeUpdater;
        private Task m_DAABBTreeTask;

        //private MyObjectsPool<MyRBIslandSolver> m_SolverPool;
        private static MyObjectsPool<MyRBIslandNSolver> m_SolverPool;

        private static List<MyRBIslandNSolver> m_SolverList;
        private Action<int> m_SolverAction;

        private List<MyRBElementInteraction> m_InteractionsList;
        private Action<int> m_InteractionAction;

        private MyRigidBodyIslandSleepState m_DeactivationSolver;
        #endregion
    }

}
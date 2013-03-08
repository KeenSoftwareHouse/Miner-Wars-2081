#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Dynamic aabb tree updater. Updates the DAABB tree to have it balanced again.
    /// </summary>
    /*
    class MyDAABBTreeUpdater: ParallelTasks.IWork
    {
        public MyDAABBTreeUpdater(MyDynamicAABBTree tree)
        {
            m_Tree = tree;
        }

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        public void DoWork()
        {
            m_Tree.Rebalance(NUM_BALANCE_STEPS);            
        }

        private int NUM_BALANCE_STEPS = 5;
        private MyDynamicAABBTree m_Tree;
    }
      */

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Broadphase (AABB vs AABB tests) implementation using the DAABB tree. This is slower then SAP for update but faster for insert/remove/AABB space tests
    /// </summary>
    class MyDynamicAABBTreeBroadphase : MyBroadphase
    {
        public MyDynamicAABBTreeBroadphase()
        {
            m_DAABBTree = new MyDynamicAABBTree(new Vector3(MyPhysicsConfig.AABBExtension));

            m_InteractionList = new List<MyRBElementInteraction>(256);
            m_overlapElementList = new List<MyElement>(256);
        }

        public MyDynamicAABBTree GetDAABBTree()
        {
            return m_DAABBTree;
        }

        public override void CreateVolume(MyElement element)
        {
            MyCommonDebugUtils.AssertDebug(element.ProxyData == MyElement.PROXY_UNASSIGNED);
            BoundingBox aabb = element.GetWorldSpaceAABB();
            element.ProxyData = m_DAABBTree.AddProxy(ref aabb, element, (uint)element.Flags);
        }
        public override void DestroyVolume(MyElement element)
        {
            if (element.ProxyData == MyElement.PROXY_UNASSIGNED)
            {
                return;
            }
            m_DAABBTree.RemoveProxy(element.ProxyData);
            element.ProxyData = MyElement.PROXY_UNASSIGNED;

            if ((element.Flags & MyElementFlag.EF_SENSOR_ELEMENT) > 0)
            {
                MySensorElement se = (MySensorElement)element;
            }

            if ((element.Flags & MyElementFlag.EF_RB_ELEMENT) > 0)
            {
                MyRBElement elm = (MyRBElement)element;
                
                
                //clear all iterations from me and from objects i iterate with
                while(elm.GetRBElementInteractions().Count > 0){
                    MyRBElementInteraction intr = elm.GetRBElementInteractions()[0];
                    MyPhysics.physicsSystem.GetRBInteractionModule().RemoveRBElementInteraction(intr.RBElement1, intr.RBElement2);
                }
                
                
                elm.GetRBElementInteractions().Clear();
            }
        }

        /// <summary>
        /// when a volume moves we have to update the AABB and then update the tree
        /// </summary>
        public override void MoveVolume(MyElement element)
        {
            if (element.ProxyData == MyElement.PROXY_UNASSIGNED)
            {
                return;
            }

            element.UpdateAABB();
            BoundingBox aabb = element.GetWorldSpaceAABB();
            m_DAABBTree.MoveProxy(element.ProxyData,ref aabb, Vector3.Zero);
        }

        /// <summary>
        /// we have the aabb updated and just update the tree using the info from velocity as a hint
        /// </summary>
        public override void MoveVolumeFast(MyElement element)
        {
            if (element.ProxyData == MyElement.PROXY_UNASSIGNED)
            {
                return;
            }

            if ((element.Flags & MyElementFlag.EF_RB_ELEMENT) > 0)
            {
                MyRBElement rel = (MyRBElement) element;

                float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

                Vector3 movement = rel.GetRigidBody().LinearVelocity * dt;
                BoundingBox aabb = element.GetWorldSpaceAABB();
                m_DAABBTree.MoveProxy(element.ProxyData, ref aabb, movement);
            }
            else
            {
                BoundingBox aabb = element.GetWorldSpaceAABB();
                m_DAABBTree.MoveProxy(element.ProxyData, ref aabb, Vector3.Zero);
            }
        }

        private void ClearInteractions()
        {
            MyRBInteractionModule module = MyPhysics.physicsSystem.GetRBInteractionModule();

            foreach (var intr in m_InteractionList)
            {
                module.RemoveRBElementInteraction(intr.RBElement1,intr.RBElement2);
            }

            m_InteractionList.Clear();
        }

        /// <summary>
        /// parses all active rigids, updates the aabbs and checks for possible collisions using the DAABB
        /// </summary>
        public override void DoWork()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ClearInteractions");
            ClearInteractions();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
    
            MyRBInteractionModule module = MyPhysics.physicsSystem.GetRBInteractionModule();
            HashSet<MyRigidBody> activeRigids = MyPhysics.physicsSystem.GetRigidBodyModule().GetActiveRigids();
            float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;
            BoundingBox aabb;

            //Dictionary<string, int> typeStats = new Dictionary<string, int>();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MoveProxy");
            
            // A.B. this might be expensive, maybe update separate or move somewhere else like in the solve -> update positions !!
            foreach (MyRigidBody rbo in activeRigids)
            {
                   /*
                string ts = ((MinerWars.AppCode.Game.Physics.MyPhysicsBody)rbo.m_UserData).Entity.GetType().Name.ToString();
                if (!typeStats.ContainsKey(ts))
                    typeStats.Add(ts, 0);
                typeStats[ts]++;
                     */

                for (int j = 0; j < rbo.GetRBElementList().Count; j++)
                {
                    MyRBElement el = rbo.GetRBElementList()[j];
                    el.UpdateAABB();
                    aabb = el.GetWorldSpaceAABB();
                    m_DAABBTree.MoveProxy(el.ProxyData, ref aabb, el.GetRigidBody().LinearVelocity*dt);
                }
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("make the AABB test");
            // make the AABB test
            MyRBElementInteraction interaction = null;

#if RENDER_PROFILING && !MEMORY_PROFILING
            int[] heights = new int[activeRigids.Count];
            int[] tests = new int[activeRigids.Count];
#endif

            int i = 0;
            foreach (MyRigidBody rbo in activeRigids)
            {
                for (int j = 0; j < rbo.GetRBElementList().Count; j++)
                {
                    MyRBElement el = rbo.GetRBElementList()[j];
                    Vector3 globalPosition = Vector3.Transform(el.LocalPosition, rbo.Matrix);
                    Vector3 deltaVelocity = rbo.LinearVelocity * dt;
                    aabb = el.GetWorldSpaceAABB();

                    if (rbo.ReadFlag(RigidBodyFlag.RBF_COLDET_THROUGH_VOXEL_TRIANGLES) || el is MyRBSphereElement) //because sphere is interpolated for whole path
                    {
                        Vector3 v = globalPosition + rbo.LinearVelocity * dt;
                        //Vector3 v = aabb.GetCenter()+rbo.LinearVelocity * dt;
                        aabb = aabb.Include(ref v);
                    }
                    else
                    {
                        aabb.Max += deltaVelocity;
                        aabb.Min += deltaVelocity;
                    }

                    //if (el is MyRBSphereElement)
                    //{
                        //MyDebugDraw.AddDrawSphereWireframe(new BoundingSphere(aabb.GetCenter(), (aabb.GetCorners()[0] - aabb.GetCenter()).Length()));
                       // MyDebugDraw.AddDrawSphereWireframe(new BoundingSphere(aabb.GetCenter()+rbo.LinearVelocity * dt, (aabb.GetCorners()[0] - aabb.GetCenter()).Length()));
                    //}

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_DAABBTree.OverlapAllBoundingBox");
#if RENDER_PROFILING && !MEMORY_PROFILING
                    m_DAABBTree.OverlapAllBoundingBox(ref aabb, m_overlapElementList, 0);
#else
                    m_DAABBTree.OverlapAllBoundingBox(ref aabb, m_overlapElementList, 0);
#endif

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Interactions");

                    foreach (var lEl in m_overlapElementList)
                    {
                        if (el == lEl)//optimization?
                            continue;
                        if ((lEl.Flags & MyElementFlag.EF_SENSOR_ELEMENT) > 0)
                        {
                            MySensorElement sensorElement = lEl as MySensorElement;
                            MyRBElement rbElement = el as MyRBElement;
                            MyPhysics.physicsSystem.GetSensorInteractionModule().AddSensorInteraction(sensorElement, rbElement);
                            continue;
                        }

                        if ((lEl.Flags & MyElementFlag.EF_RB_ELEMENT) > 0)
                        {
                            MyRBElement testEl = (MyRBElement)lEl;

                            if (el.GetRigidBody().IsStatic() && testEl.GetRigidBody().IsStatic())
                                continue;

                            if (el.GetRigidBody().IsKinematic() && testEl.GetRigidBody().IsKinematic())
                                continue;

                            if (el.GetRigidBody().IsKinematic() && testEl.GetRigidBody().IsStatic())
                                continue;

                            if (el.GetRigidBody().IsStatic() && testEl.GetRigidBody().IsKinematic())
                                continue;

                            if (el.GetRigidBody() == testEl.GetRigidBody())
                                continue;

                            if(!MyFiltering.AcceptCollision(el,testEl))
                                continue;

                            interaction = module.FindRBElementInteraction(el, testEl);
                            if (interaction == null)
                            {
                                interaction = module.AddRBElementInteraction(el, testEl);
                            }

                            if (interaction != null)
                            {
                                bool iinserted = false;
                                for (int t = 0; t < m_InteractionList.Count; t++)
                                {
                                    if (m_InteractionList[t] == interaction)
                                    {
                                        iinserted = true;
                                        break;
                                    }
                                }
                                if (!iinserted)
                                    m_InteractionList.Add(interaction);
                            }
                        }
                    }

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();



                }

                i++;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Active rigids", activeRigids.Count);

#if RENDER_PROFILING && !MEMORY_PROFILING
            float averageHeight = 0;
            float averageTest = 0;
            int maxHeight = 0;
            int maxTest = 0;
            for (int j = 0; j < activeRigids.Count; j++)
            {
                averageHeight += heights[j];
                averageTest += tests[j];
                if (maxHeight < heights[j])
                    maxHeight = heights[j];
                if (maxTest < tests[j])
                    maxTest = tests[j];
            }

            averageHeight /= activeRigids.Count;
            averageTest /= activeRigids.Count;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Average height", averageHeight);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Average test", averageTest);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Max height", maxHeight);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Max test", maxTest);
#endif

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("handle active sensors");
            List<MySensor> activeSensors = MyPhysics.physicsSystem.GetSensorModule().ActiveSensors;
            if (activeSensors.Count > 0)
            {
                if (m_activeSensorIndex >= activeSensors.Count)
                {
                    m_activeSensorIndex = 0;
                }

                MySensor activeSensor = activeSensors[m_activeSensorIndex];
                activeSensor.PrepareSensorInteractions();
                MySensorElement sensorElement = activeSensor.GetElement();
                BoundingBox sensorElAABB = sensorElement.GetWorldSpaceAABB();
                m_sensorInteractonList.Clear();
                m_DAABBTree.OverlapAllBoundingBox(ref sensorElAABB, m_sensorInteractonList, (uint)MyElementFlag.EF_RB_ELEMENT);
                foreach (MyRBElement rbElement in m_sensorInteractonList)
                {
                    MyPhysics.physicsSystem.GetSensorInteractionModule().AddSensorInteraction(sensorElement, rbElement);
                }
                activeSensor.Active = false;                
                m_activeSensorIndex++;
            }
            //List<MySensor> activeSensors = MyPhysics.physicsSystem.GetSensorModule().ActiveSensors;
            //for (int i = activeSensors.Count - 1; i >= 0; i--)
            //{
            //    MySensorElement sensorElement = activeSensors[i].GetElement();
            //    BoundingBox sensorElAABB = sensorElement.GetWorldSpaceAABB();
            //    m_sensorInteractonList.Clear();
            //    m_DAABBTree.OverlapRBAllBoundingBox(ref sensorElAABB, m_sensorInteractonList);
            //    foreach (MyRBElement rbElement in m_sensorInteractonList)
            //    {
            //        MyPhysics.physicsSystem.GetSensorInteractionModule().AddSensorInteraction(sensorElement, rbElement);
            //    }
            //    activeSensors[i].IsActive = false;
            //    activeSensors.RemoveAt(i);
            //}

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }        

        public override List<MyRBElementInteraction> GetRBElementInteractionList()
        {
            return m_InteractionList;
        }

        public override void Close()
        {
            m_DAABBTree.Clear();
            base.Close();
        }

        private MyDynamicAABBTree m_DAABBTree;
        private List<MyRBElementInteraction> m_InteractionList;
        private List<MyElement> m_overlapElementList;
        private List<MyRBElement> m_sensorInteractonList = new List<MyRBElement>();
        private int m_activeSensorIndex;
    }
}
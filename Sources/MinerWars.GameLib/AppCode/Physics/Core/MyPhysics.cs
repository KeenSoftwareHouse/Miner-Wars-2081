#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Threading;
using SysUtils.Utils;
using ParallelTasks;
using System;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// base physics class holding modules and physics classes. This is the world that you simulate.
    /// </summary>
    class MyPhysics
    {
        public static bool PhysicsSimulation = true;
        public static MyPhysics physicsSystem;

        static MyPhysics()
        {            
        }

        public MyPhysics()
        {
            physicsSystem = this;

            m_RigidBodyModule = new MyRigidBodyModule();
            m_SensorModule = new MySensorModule();
            m_ContactConstraintModule = new MyContactConstraintModule();

            m_SensorInteractionModule = new MySensorInteractionModule();            

            m_RBInteractionModule = new MyRBInteractionModule();
            m_Utils = new MyPhysicsUtils();

            m_PhysicsObjects = new MyPhysicsObjects();

            m_SimulationHandlers = new List<MyPhysSimulationHandler>(16);
        }

        public static void DebugDrawPhysicsPrunning()
        {
            if (MyPhysics.physicsSystem == null) 
            {
                return;
            }

            BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            List<Physics.MyElement> list = new List<Physics.MyElement>();
            //((MinerWars.AppCode.Physics.MyDynamicAABBTree)MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure()).Rebalance(256);

            MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure().OverlapAllBoundingBox(ref aabb, list, 0, false);

            foreach (Physics.MyElement element in list)
            {
                BoundingBox elementAABB = element.GetWorldSpaceAABB();

                if (Vector3.Distance(elementAABB.Min, MinerWars.AppCode.Game.Utils.MyCamera.Position) < 1000)
                {
                    Vector4 color = Vector4.One;
                    MinerWars.AppCode.Game.Utils.MyDebugDraw.DrawAABBLowRes(ref elementAABB, ref color, 1.0f);
                }
            }
        }

        public bool InitializePhysics()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyPhysics.InitializePhysics");
            m_RigidBodyModule.Initialize();
            m_RBInteractionModule.Init();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return true;
        }

        public void DestroyPhysics()
        {
            m_SensorModule.Destroy();
            m_SensorModule = null;

            m_RigidBodyModule.Destroy();
            m_RigidBodyModule = null;

            m_RBInteractionModule.Destroy();
            m_RBInteractionModule = null;

            m_ContactConstraintModule.Destroy();
            m_ContactConstraintModule = null;
            m_SimulationHandlers.Clear();
            m_SimulationHandlers = null;

            physicsSystem = null;            
            
            
            m_SensorInteractionModule = null;
            
            m_Utils = null;
            m_PhysicsObjects = null;
        }

        /// <summary>
        /// Simulation handlers can be registered here in order to receive simulation notifications
        /// </summary>
        public void RegisterSimulationHandler(MyPhysSimulationHandler simHandler)
        {
#if PHYSICS_CHECK
            for (int i = 0; i < m_SimulationHandlers.Count; i++ )
            {
                MyPhysSimulationHandler simH = m_SimulationHandlers[i];
                if(simH == simHandler)
                {            
                    // cannot add already existing item!
                    MyCommonDebugUtils.AssertDebug(false);
                }
            }
#endif
            m_SimulationHandlers.Add(simHandler);
        }

        public void UnregisterSimulationHandler(MyPhysSimulationHandler simHandler)
        {
#if PHYSICS_CHECK
            bool found = false;
            for (int i = 0; i < m_SimulationHandlers.Count; i++)
            {
                MyPhysSimulationHandler simH = m_SimulationHandlers[i];
                if (simH == simHandler)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                // cannot remove item!
                MyCommonDebugUtils.AssertDebug(false);
            }
#endif
            m_SimulationHandlers.Remove(simHandler);
        }

        /// <summary>
        /// main simulation function
        /// </summary>
        public void Simulate(float timeStep)
        {
            if (!PhysicsSimulation)
            {
                return;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("fire before simulation callbacks");

            MyRBBoxElementTriangleMeshElementInteraction.TestsCount = 0;
            MyRBBoxElementTriangleMeshElementInteraction.TrianglesTested = 0;

            // fire before simulation callbacks
            for (int i = 0; i < m_SimulationHandlers.Count; i++)
            {
                MyPhysSimulationHandler simH = m_SimulationHandlers[i];
                simH.BeforeSimulation(timeStep);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("simulate rigid bodies");

            // simulate rigid bodies
            m_RigidBodyModule.Simulate(timeStep);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("fire after simulation callbacks");

            // fire after simulation callbacks
            for (int i = 0; i < m_SimulationHandlers.Count; i++)
            {
                MyPhysSimulationHandler simH = m_SimulationHandlers[i];
                simH.AfterSimulation(timeStep);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public Vector3 Gravitation
        {
            get { return m_Gravitation; }
            set { m_Gravitation = value; }
        }

        public List<Tuple<BoundingSphere, float>> GravitationPoints
        {
            get { return m_GravitationPoints; }
        }

        public MyRigidBodyModule GetRigidBodyModule() { return m_RigidBodyModule; }
        public MyRBInteractionModule GetRBInteractionModule() { return m_RBInteractionModule; }
        public MyPhysicsUtils GetUtils() { return m_Utils; }
        public MyPhysicsObjects GetPhysicsObjects() { return m_PhysicsObjects; }
        public MyContactConstraintModule GetContactConstraintModule() { return m_ContactConstraintModule; }
        public MySensorModule GetSensorModule() { return m_SensorModule; }
        public MySensorInteractionModule GetSensorInteractionModule() { return m_SensorInteractionModule; }

#region members
        private Vector3 m_Gravitation = Vector3.Zero;
        private List<Tuple<BoundingSphere, float>> m_GravitationPoints = new List<Tuple<BoundingSphere, float>>();
        private MyPhysicsObjects            m_PhysicsObjects;
        private List<MyPhysSimulationHandler>   m_SimulationHandlers;
        private MyRigidBodyModule           m_RigidBodyModule;
        private MySensorModule              m_SensorModule;
        private MyContactConstraintModule   m_ContactConstraintModule;
        private MyRBInteractionModule       m_RBInteractionModule;
        private MyPhysicsUtils              m_Utils;        
        private MySensorInteractionModule   m_SensorInteractionModule;        

#endregion
        
    }

}
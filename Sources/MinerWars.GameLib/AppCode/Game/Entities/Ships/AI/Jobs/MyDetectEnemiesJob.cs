using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWarsMath;
using ParallelTasks;
using MinerWars.AppCode.Physics;
using KeenSoftwareHouse.Library.Memory;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo.UniversalLauncher;

namespace MinerWars.AppCode.Game.Entities.Ships.AI.Jobs
{
    class MyDetectEnemiesJob : IWork
    {
        MySmallShipBot m_bot;
        MyMwcObjectBuilder_FactionEnum m_botFaction;
        float m_seeDistance;
        Matrix m_botWorldMatrix;
        Vector3 m_position;

        private MyEntity m_closestEnemy;
        public MyEntity ClosestEnemy
        {
            get { return m_closestEnemy; }
            set
            {
                if (m_closestEnemy != null)
                {
                    m_closestEnemy.OnClose -= m_closestEnemy_OnClose;
                }
                m_closestEnemy = value;
                if (m_closestEnemy != null)
                {
                    m_closestEnemy.OnClose += m_closestEnemy_OnClose;
                }
            }
        }

        private MyEntity m_closestVisual;
        public MyEntity ClosestVisual
        {
            get { return m_closestVisual; }
            set
            {
                if (m_closestVisual != null)
                {
                    m_closestVisual.OnClose -= m_closestVisual_OnClose;
                }
                m_closestVisual = value;
                if (m_closestVisual != null)
                {
                    m_closestVisual.OnClose += m_closestVisual_OnClose;
                }
            }
        }

        public void Start(MySmallShipBot bot)
        {
            m_bot = bot;
            m_botFaction = bot.Faction;
            m_seeDistance = bot.SeeDistance;
            m_botWorldMatrix = bot.WorldMatrix;
            m_position = bot.GetPosition();
            m_closestEnemy_OnClose = closestEnemy_OnClose;
            m_closestVisual_OnClose = closestVisual_OnClose;
            m_bot.OnClose += m_bot_OnClose;
        }

        void m_bot_OnClose(MyEntity obj)
        {
            m_bot = null;
            Finish();
        }

        Action<MyEntity> m_closestEnemy_OnClose;
        Action<MyEntity> m_closestVisual_OnClose;

        void closestEnemy_OnClose(MyEntity obj)
        {
            m_closestEnemy = null;
        }

        void closestVisual_OnClose(MyEntity obj)
        {
            m_closestVisual = null;
        }

        public void DoWork()
        {
            //  Search for target to attack
            ClosestEnemy = null;
            ClosestVisual = null;

            float distanceSqr = m_seeDistance * m_seeDistance;
            float closestEnemyDistanceSqr = float.PositiveInfinity;
            float closestVisualDistanceSqr = float.PositiveInfinity;

            using (var rbFounded = PoolList<MyRBElement>.Get())
            {
                try
                {
                    MyEntities.EntityCloseLock.AcquireShared();

                    MyDynamicAABBTree prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();

                    BoundingBox rbInputElementGetWorldSpaceAABB = new BoundingBox(
                        m_botWorldMatrix.Translation - new Vector3(m_seeDistance),
                        m_botWorldMatrix.Translation + new Vector3(m_seeDistance));
                    prunningStructure.OverlapAllBoundingBox(ref rbInputElementGetWorldSpaceAABB, rbFounded, (uint)MyElementFlag.EF_RB_ELEMENT);

                    //now try find spot
                    foreach (MyRBElement rb in rbFounded)
                    {

                        if (m_bot == null)
                            return;

                        var rigidBody = rb.GetRigidBody();
                        if (rigidBody == null)
                            continue;

                        MyEntity entity = ((MyPhysicsBody)rigidBody.m_UserData).Entity;
                        if (entity == m_bot || entity == null || entity.AIPriority == -1) 
                            continue;


                        entity = entity.GetBaseEntity();    // Large weapons

                        // Ignore spoiled holograms
                        if (m_bot.IsSpoiledHologram(entity))
                        {
                            continue;
                        }

                        // Don't attack disabled weapons
                        MyPrefabLargeWeapon largeWeapon = entity as MyPrefabLargeWeapon;
                        MySmallShip smallShip = entity as MySmallShip;
                        MyPrefabLargeShip largeShip = entity as MyPrefabLargeShip;

                        if (largeWeapon != null && !largeWeapon.IsWorking())
                        {
                            continue;
                        }

                        // Test smallships and largeweapons
                        if (smallShip != null || largeWeapon != null || largeShip != null)
                        {
                            // Is enemy?
                            if (MyFactions.GetFactionsRelation(m_bot, entity) == MyFactionRelationEnum.Enemy && CanSeeTarget(m_bot, entity))
                            {
                                var entityDistanceSqr = Vector3.DistanceSquared(entity.GetPosition(), m_position);

                                if (entityDistanceSqr < distanceSqr &&
                                    (ClosestEnemy == null || entity.AIPriority >= ClosestEnemy.AIPriority) &&
                                    (entityDistanceSqr < closestEnemyDistanceSqr || entity.AIPriority > ClosestEnemy.AIPriority))
                                {
                                    MyLine line = new MyLine(m_position, entity.GetPosition(), true);
                                    var result = MyEntities.GetIntersectionWithLine(ref line, m_bot, entity, true, ignoreChilds: true);
                                    if (!result.HasValue)
                                    {
                                        // Visual Detection - ignore visualy detected targets if they are further than any normaly detected target
                                        if (IsVisualyDetected(smallShip))
                                        {
                                            if (entityDistanceSqr < closestVisualDistanceSqr)
                                            {
                                                ClosestVisual = entity;
                                                closestVisualDistanceSqr = entityDistanceSqr;
                                            }
                                        }
                                        else
                                        {
                                            closestEnemyDistanceSqr = entityDistanceSqr;
                                            ClosestEnemy = entity;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    MyEntities.EntityCloseLock.ReleaseShared();
                }
            }
        }

        public void Finish()
        {
            if (m_bot != null)
            {
                m_bot.OnClose -= new Action<MyEntity>(m_bot_OnClose);
            }
            if (m_closestEnemy != null)
            {
                m_closestEnemy.OnClose -= m_closestEnemy_OnClose;
            }
            if (m_closestVisual != null)
            {
                m_closestVisual.OnClose -= m_closestVisual_OnClose;
            }
        }

        /// <summary>
        /// Returns if bot can see target. If bot's fov is enabled and target is smallship which has radar jammer, then bot can see only targets in front of him
        /// </summary>
        /// <param name="target">Target</param>
        private bool CanSeeTarget(MySmallShipBot me, MyEntity target) 
        {
            bool canSeeTarget = true;
            if (MyFakes.ENABLE_BOTS_FOV_WHEN_RADAR_JAMMER)
            {
                MySmallShip smallShipTarget = target as MySmallShip;
                if (smallShipTarget != null && !smallShipTarget.IsHologram && smallShipTarget.HasRadarJammerActive()) 
                {
                    float distanceSqr = (me.GetPosition() - target.GetPosition()).LengthSquared();
                    float rangeOfViewSqr = smallShipTarget.IsHiddenFromBots() ? MyAIConstants.BOT_FOV_RANGE_HIDDEN : MyAIConstants.BOT_FOV_RANGE;
                    float targetRange = Vector3.Dot(m_botWorldMatrix.Forward, target.GetPosition() - me.GetPosition());

                    canSeeTarget =
                        targetRange <= rangeOfViewSqr &&
                        Vector3.Dot(m_botWorldMatrix.Forward, Vector3.Normalize(target.GetPosition() - m_position)) >= MyAIConstants.BOT_FOV_COS;
                }                
            }
            return canSeeTarget;
        }

        private bool IsVisualyDetected(MySmallShip smallShip)
        {
            return MyFakes.ENABLE_BOTS_FOV_WHEN_RADAR_JAMMER && smallShip != null && smallShip.HasRadarJammerActive();
        }

        public WorkOptions Options
        {
            get { return new WorkOptions() { MaximumThreads = 1 }; }
        }
    }
}

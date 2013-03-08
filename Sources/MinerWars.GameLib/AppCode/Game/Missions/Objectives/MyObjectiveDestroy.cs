using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Missions
{
    // A submission to destroy a list of entities.
    class MyObjectiveDestroy : MyMultipleObjectives
    {
        private List<uint> m_toKill;
        private List<uint> m_toKillSpawnpoints;
        private bool m_showMarks;

        private int m_destroyTotalCount;
        private int m_destroyCount;

        public int ToDestroyCount = 0;
        public string DestroyCaption;

        [Obsolete]
        public MyObjectiveDestroy(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, bool displayObjectivesCount = true, MyDialogueEnum? successDialogID = null, MyDialogueEnum? startDialogID = null)
            : this(name, id, description, icon, parentMission, requiredMissions, toKill, new List<uint>(), true, displayObjectivesCount, successDialogID, startDialogID)

        {
        }

        [Obsolete]
        public MyObjectiveDestroy(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, List<uint> toKillSpawnpoints, bool showMarks = true, bool displayObjectivesCount = true, MyDialogueEnum? successDialogID = null, MyDialogueEnum? startDialogID = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, displayObjectivesCount: displayObjectivesCount, successDialogId: successDialogID, startDialogId: startDialogID)
        {
            m_toKill = toKill;
            m_toKillSpawnpoints = toKillSpawnpoints;
            m_showMarks = showMarks;
            FailIfEntityDestroyed = false;
            MakeEntityIndestructible = false;
        }


        public MyObjectiveDestroy(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, bool displayObjectivesCount = true, MyDialogueEnum? successDialogID = null, MyDialogueEnum? startDialogID = null)
            : this(name, id, description, icon, parentMission, requiredMissions, toKill, new List<uint>(), true, displayObjectivesCount, successDialogID, startDialogID)
        {
        }

        public MyObjectiveDestroy(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, List<uint> toKillSpawnpoints, bool showMarks = true, bool displayObjectivesCount = true, MyDialogueEnum? successDialogID = null, MyDialogueEnum? startDialogID = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, displayObjectivesCount: displayObjectivesCount, successDialogId: successDialogID, startDialogId: startDialogID)
        {
            m_toKill = toKill;
            m_toKillSpawnpoints = toKillSpawnpoints;
            m_showMarks = showMarks;
            FailIfEntityDestroyed = false;
            MakeEntityIndestructible = false;
        }

        public void AddToKill(uint toKill)
        {
            if (!m_toKill.Contains(toKill))
            {
                m_toKill.Add(toKill);
                m_destroyTotalCount++;
                
                MyEntity entity = MyScriptWrapper.GetEntity(toKill);
                System.Diagnostics.Debug.Assert(!(entity is MySpawnPoint));
                MarkAsDestroy(entity);
            }
        }

        protected override int GetObjectivesCompletedCount()
        {
            return m_destroyCount;
        }

        protected override int GetObjectivesTotalCount()
        {
            return m_destroyTotalCount;
        }

        public override void Load()
        {
            Init();
            base.Load();

            MyScriptWrapper.EntityDeath += OnEntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
        }



        void OnSpawnpointBotSpawned(MyEntity entity1, MyEntity entity2)
        {
            if (m_showMarks && m_toKillSpawnpoints != null)
            {
                foreach (var spawnPointId in m_toKillSpawnpoints)
                {
                    foreach (var spawnPointBot in MyScriptWrapper.GetSpawnPointBots(spawnPointId))
                    {
                        if (spawnPointBot.Ship == entity2)
                        {
                            MarkAsDestroy(entity2);
                            break;
                        }
                    }
                }
            }
        }

        private void MarkAsDestroy(MyEntity entity)
        {
            string destroyCaption = DestroyCaption ?? (HudName!=null ? HudNameTemp.ToString() : null) ?? MyTextsWrapper.Get(MyTextsWrapperEnum.Destroy).ToString();
            if (entity is MySmallShipBot)
                destroyCaption = "";

            MyScriptWrapper.MarkEntity(entity, destroyCaption, guiTargetMode: MyGuitargetMode.Enemy, flags: MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_TEXT);
        }

        /// <summary>
        /// Inits destroy counts and hud markers
        /// </summary>
        private void Init()
        {
            // show asserts for ill defined spawnpoints
            if (m_toKillSpawnpoints != null)
            {
                foreach (var spawnpointId in m_toKillSpawnpoints)
                {
                    MySpawnPoint spawnpointEntity;
                    if (MyEntities.TryGetEntityById(new MyEntityIdentifier(spawnpointId), out spawnpointEntity))
                    {
                        Debug.Assert(spawnpointEntity.Activated, "DestroyObjective: spawnpointEntity.Activated == false");
                    }
                    else
                    {
                        Debug.Assert(spawnpointEntity != null, "DestroyObjective - bad spawnpoint");
                    }
                }
            }

            m_destroyTotalCount = 0;
            m_destroyCount = 0;
            if (m_toKill != null)
            {
                foreach (var entityId in m_toKill)
                {
                    if (!MyScriptWrapper.IsEntityDead(entityId))
                    {
                        MyEntity entity = MyScriptWrapper.GetEntity(entityId);
                        if (m_showMarks)
                        {
                            MarkAsDestroy(entity);
                        }
                    }
                    else
                    {
                        m_destroyCount++;
                    }
                    m_destroyTotalCount++;
                }
            }
            if (m_toKillSpawnpoints != null)
            {
                foreach (var entityId in m_toKillSpawnpoints)
                {
                    foreach (var bot in MyScriptWrapper.GetSpawnPointBots(entityId))
                    {
                        if (bot.Ship != null && !bot.Ship.IsDead())
                        {
                            if (m_showMarks)
                            {
                                MarkAsDestroy(bot.Ship);
                            }
                        }
                    }
                    MySpawnPoint spawnPoint = MyScriptWrapper.GetEntity(entityId) as MySpawnPoint;
                    //int totalForThisSpawnpoint = (spawnPoint.MaxSpawnCount <= 0) ? spawnPoint.GetBots().Count : spawnPoint.MaxSpawnCount;
                    int totalForThisSpawnpoint = spawnPoint.MaxSpawnCount;
                    m_destroyTotalCount += totalForThisSpawnpoint;
                    if (spawnPoint.IsActive())
                    {
                        int botsToKillCount = spawnPoint.LeftToSpawn + spawnPoint.GetShipCount();
                        m_destroyCount += totalForThisSpawnpoint - botsToKillCount;
                    }
                }
            }
        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;

        }

        public override bool IsSuccess()
        {
            if (ToDestroyCount > 0) return m_destroyCount >= ToDestroyCount;
            return m_destroyCount >= m_destroyTotalCount;
        }

        public override void Success()
        {
            base.Success();

            if (m_toKill != null)
            {
                foreach (var entityId in m_toKill)
                {
                    if (!MyScriptWrapper.IsEntityDead(entityId))
                    {
                        MyEntity entity = MyScriptWrapper.GetEntity(entityId);
                        MyScriptWrapper.RemoveEntityMark(entity);
                    }
                }
            }
        }

        public override bool IsMissionEntity(MyEntity target)
        {

            if (base.IsMissionEntity(target)) return true;
            if (!target.EntityId.HasValue) return false;

            bool isSpawnPointBot = false;
            if (m_toKillSpawnpoints != null && target is MySmallShipBot)
            {
                foreach (var entityId in m_toKillSpawnpoints)
                {
                    foreach (var spawnPointBot in MyScriptWrapper.GetSpawnPointBots(entityId))
                    {
                        if (spawnPointBot.Ship == target as MySmallShipBot)
                        {
                            isSpawnPointBot = true;
                            break;
                        }
                    }
                }
            }
            return /*m_showMarks &&*/ (m_toKill != null && m_toKill.Contains(target.EntityId.Value.NumericValue) || isSpawnPointBot);
        }

        void OnEntityDeath(MyEntity deadEntity, MyEntity killedBy)
        {
            EntityDisapeared(deadEntity);
        }



        private void EntityDisapeared(MyEntity entity)
        {
            if (entity.EntityId.HasValue)
            {
                if (m_toKill != null && m_toKill.Contains(entity.EntityId.Value.NumericValue))
                {
                    m_destroyCount++;
                    MyScriptWrapper.RemoveEntityMark(entity);
                }
                else if (m_toKillSpawnpoints != null)
                {
                    foreach (var spawnPointId in m_toKillSpawnpoints)
                    {
                        foreach (var spawnPointBot in MyScriptWrapper.GetSpawnPointBots(spawnPointId))
                        {
                            if (spawnPointBot.Ship == entity)
                            {
                                m_destroyCount++;
                                MyScriptWrapper.RemoveEntityMark(entity);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveDisablePrefabs : MyMultipleObjectives
    {
        private List<uint> m_toDisable;
        private int m_disabledPrefabsCount;
        private bool m_markObjectsToDisable;

        public MyObjectiveDisablePrefabs(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, 
                MyMissionID[] requiredMissions, List<uint> toDisable, List<uint> disableBy, bool displayObjectivesCount = true, 
                bool markObjectsToDisable = true, MyDialogueEnum? successDialogId = null, MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, disableBy, successDialogId, startDialogId,  displayObjectivesCount: displayObjectivesCount)
        {
            m_toDisable = toDisable;
            m_markObjectsToDisable = markObjectsToDisable;
            RequiredEntityIDs.AddRange(toDisable);
        }

        protected override int GetObjectivesTotalCount()
        {
            return m_toDisable.Count;
        }

        protected override int GetObjectivesCompletedCount()
        {
            return m_disabledPrefabsCount;
        }

        public override void Load()
        {
            RecalculatedDisabledPrefabsCount();
            base.Load();
            if (m_markObjectsToDisable){
                foreach (var entityId in m_toDisable)
                {
                    var prefab = MyScriptWrapper.GetPrefab(entityId);
                    if (prefab != null && prefab.Enabled)
                        MyScriptWrapper.MarkEntity(prefab, MyTextsWrapper.Get(MyTextsWrapperEnum.Disable).ToString());
                }
            }
            MyScriptWrapper.EntityClosing += OnEntityClosing;
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.EntityClosing -= OnEntityClosing;
        }

        private void RecalculatedDisabledPrefabsCount()
        {
            m_disabledPrefabsCount = 0;
            foreach (var entityId in m_toDisable)
            {
                var prefab = MyScriptWrapper.TryGetPrefab(entityId);
                if (prefab == null || !prefab.Enabled)
                {
                    m_disabledPrefabsCount++;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            RecalculatedDisabledPrefabsCount();
        }

        public override bool IsSuccess()
        {
            //foreach (var entityId in m_toDisable)
            //{
            //    var prefab = MyScriptWrapper.GetPrefab(entityId);
            //    if (prefab != null && prefab.Enabled)
            //        return false;
            //}
            //return true;
            return m_disabledPrefabsCount >= m_toDisable.Count;
        }

        public override void Success()
        {
            base.Success();

            foreach (var entityId in m_toDisable)
            {
                var prefab = MyScriptWrapper.GetPrefab(entityId);
                if (prefab != null)
                    MyScriptWrapper.RemoveEntityMark(prefab);
            }
        }
        /*
        public override bool IsMissionEntity(MyEntity target)
        {
            if (base.IsMissionEntity(target)) return true;
            if (!target.EntityId.HasValue) return false;
            return false;
            //return m_toDisable.Contains(target.EntityId.Value.NumericValue);
        }
        */
        void OnEntityClosing(MyEntity entity)
        {
            if (entity is MyPrefabBase && entity.EntityId.HasValue && m_toDisable.Contains(entity.EntityId.Value.NumericValue))
            {
                MyScriptWrapper.RemoveEntityMark(entity);
            }
        }
    }
}

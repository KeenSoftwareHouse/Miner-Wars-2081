using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions.Utils;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveGetItems : MyMultipleObjectives
    {
        private List<MyItemToGetDefinition> m_itemsToGet;
        private int m_totalCount;

        public MyObjectiveGetItems(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<MyItemToGetDefinition> itemsToGet, List<uint> getFrom)
            : base(name, id, description, icon, parentMission, requiredMissions, null, getFrom)
        {
            m_itemsToGet = itemsToGet;
        }

        public override void Load()
        {
            m_totalCount = m_itemsToGet.Count;
            RecalculateItemsCount(MyScriptWrapper.GetPlayerInventory());
            RecalculateItemsCount(MyScriptWrapper.GetCentralInventory());
            
            base.Load();
            MyScriptWrapper.EntityInventoryContentChanged += OnInventoryContentChanged;
            if (MySession.Static != null && MySession.Static.Inventory != null)
                MySession.Static.Inventory.OnInventoryContentChange += OnInventoryContentChangedEntity;
        }

        private void OnInventoryContentChangedEntity(MyInventory sender)
        {
            OnInventoryContentChanged(null, sender);
        }

        void OnInventoryContentChanged(MyEntity entity, Inventory.MyInventory inventory)
        {
            if (inventory == MyScriptWrapper.GetPlayerInventory() )
            {
                RecalculateItemsCount(inventory);
            }

            if (inventory == MyScriptWrapper.GetCentralInventory())
            {
                RecalculateItemsCount(inventory);
            }
        }

        private void RecalculateItemsCount(MyInventory inventory)
        {
            for (int index = 0; index < m_itemsToGet.Count; index++)
            {
                var itemToGet = m_itemsToGet[index];
                int itemCount = MyScriptWrapper.GetInventoryItemCount(inventory, itemToGet.ItemType, itemToGet.ItemId);
                if (itemCount >= itemToGet.Count)
                {
                    m_itemsToGet.Remove(itemToGet);
                    index--;

                }
            }
        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.EntityInventoryContentChanged -= OnInventoryContentChanged;
        }

        protected override int GetObjectivesTotalCount()
        {
            return m_totalCount;
        }

        protected override int GetObjectivesCompletedCount()
        {
            return m_totalCount - m_itemsToGet.Count;
        }

        public override bool IsSuccess()
        {
            //foreach (var itemToGet in m_itemsToGet)
            //{
            //    int itemCount = MyScriptWrapper.GetInventoryItemCount(MyScriptWrapper.GetPlayerInventory(), itemToGet.ItemType, itemToGet.ItemId);
            //    if (itemCount < itemToGet.Count)
            //    {
            //        return false;
            //    }
            //}
            //return true;
            return m_itemsToGet.Count == 0;
        }
    }
}

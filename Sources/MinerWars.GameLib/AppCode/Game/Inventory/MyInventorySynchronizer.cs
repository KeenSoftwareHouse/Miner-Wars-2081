using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.GUI.Helpers;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

using SharpDX.Toolkit;

namespace MinerWars.AppCode.Game.Inventory
{
    delegate bool MyMustBeInventorySynchronizedDelegate(MyInventory inventory);

    static class MyPlayerShipInventorySynchronizerHelper
    {
        public static bool MustBePlayerShipInventorySynchronized(MyInventory inventory) 
        {
            return MySession.PlayerShip != null &&
                   MySession.PlayerShip.Inventory == inventory &&
                   MyGuiScreenInventoryManagerForGame.IsInventoryOpen();
        }
    }

    struct MyInventoryItemAmountDefinition
    {
        public MyMwcObjectBuilderTypeEnum ObjectBuilderType;
        public int? ObjectBuilderId;
        public float OriginalAmount;
        public float AmountChange;
        
        public MyInventoryItemAmountDefinition(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float originalAmount, float amountChange)
        {
            ObjectBuilderType = objectBuilderType;
            ObjectBuilderId = objectBuilderId;
            OriginalAmount = originalAmount;
            AmountChange = amountChange;
        }
    }

    class MyInventorySynchronizer
    {
        private List<MyInventoryItem> m_inventoryItemsHelper;
        private List<MyInventoryItem> m_inventoryItemsToAdd;
        private List<MyInventoryItemAmountDefinition> m_inventoryItemsAmountChanges;
        private MyMustBeInventorySynchronizedDelegate m_mustBeInventorySynchronizedDelegate;
        private MyInventory m_inventory;

        public MyInventorySynchronizer(MyInventory inventory, MyMustBeInventorySynchronizedDelegate mustBeInventorySynchronizedDelegate) 
        {
            m_inventoryItemsHelper = new List<MyInventoryItem>();
            m_inventoryItemsToAdd = new List<MyInventoryItem>();
            m_inventoryItemsAmountChanges = new List<MyInventoryItemAmountDefinition>();
            m_mustBeInventorySynchronizedDelegate = mustBeInventorySynchronizedDelegate;
            m_inventory = inventory;

            //MyMinerGame.OnGameUpdate += MyMinerGame_OnGameUpdate;
        }

        public void Close() 
        {
            foreach (var item in m_inventoryItemsToAdd) 
            {
                MyInventory.CloseInventoryItem(item);
            }
            m_inventoryItemsToAdd.Clear();
            m_inventoryItemsToAdd = null;

            m_inventoryItemsAmountChanges.Clear();
            m_inventoryItemsAmountChanges = null;

            m_inventoryItemsHelper.Clear();
            m_inventoryItemsHelper = null;

            m_mustBeInventorySynchronizedDelegate = null;

            m_inventory = null;

            //MyMinerGame.OnGameUpdate -= MyMinerGame_OnGameUpdate;
        }

        public bool MustBeSynchronized() 
        {
            return m_mustBeInventorySynchronizedDelegate(m_inventory);
        }

        public void AddInventoryItemForSynchronization(MyInventoryItem item) 
        {
            m_inventoryItemsToAdd.Add(item);
        }

        public void AddInventoryItemAmountChangeForSynchronization(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float originalAmount, float amountChange)
        {
            m_inventoryItemsAmountChanges.Add(new MyInventoryItemAmountDefinition(objectBuilderType, objectBuilderId, originalAmount, amountChange));
        }

        void MyMinerGame_OnGameUpdate(GameTime gt)
        {
            if (!MustBeSynchronized()) 
            {
                if (m_inventoryItemsToAdd.Count > 0) 
                {
                    if (!m_inventory.UnlimitedCapacity)
                    {
                        m_inventoryItemsToAdd = m_inventoryItemsToAdd.GetRange(0, Math.Min(m_inventory.GetFreeSlotsCount(), m_inventoryItemsToAdd.Count));
                    }
                    m_inventory.AddInventoryItems(m_inventoryItemsToAdd);
                    m_inventoryItemsToAdd.Clear();
                }                

                if(m_inventoryItemsAmountChanges.Count > 0)
                {
                    foreach(MyInventoryItemAmountDefinition itemAmountDef in m_inventoryItemsAmountChanges)
                    {
                        m_inventoryItemsHelper.Clear();
                        m_inventory.GetInventoryItems(ref m_inventoryItemsHelper, itemAmountDef.ObjectBuilderType, itemAmountDef.ObjectBuilderId);
                        foreach(MyInventoryItem item in m_inventoryItemsHelper)
                        {
                            if(item.Amount == itemAmountDef.OriginalAmount)
                            {
                                Debug.Assert(item.Amount + itemAmountDef.AmountChange <= item.MaxAmount);
                                item.Amount += itemAmountDef.AmountChange;
                                break;
                            }
                        }
                    }
                    m_inventoryItemsAmountChanges.Clear();
                }
            }
        }
    }
}

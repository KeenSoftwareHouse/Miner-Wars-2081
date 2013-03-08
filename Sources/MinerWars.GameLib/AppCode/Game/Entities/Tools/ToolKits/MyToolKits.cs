using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Entities.Tools.ToolKits
{
    class MyToolKits
    {        
        private MySmallShip m_smallShipOwner;
        private MyPlayer m_playerOwner;
        private List<MyToolKit> m_toolKits;
        private List<MyInventoryItem> m_inventoryItemsHelper;
        private OnInventoryContentChange OnInventoryContentChangedHandler;

        public MyToolKits(MySmallShip smallShipOwner, MyPlayer playerOwner) 
        {
            OnInventoryContentChangedHandler = new OnInventoryContentChange(SmallShipInventory_OnInventoryContentChange);
            m_smallShipOwner = smallShipOwner;
            m_playerOwner = playerOwner;
            m_inventoryItemsHelper = new List<MyInventoryItem>();
            m_toolKits = new List<MyToolKit>();
            m_smallShipOwner.Inventory.OnInventoryContentChange += OnInventoryContentChangedHandler;
            RefreshToolKits();
        }        

        private void SmallShipInventory_OnInventoryContentChange(MyInventory sender)
        {
            RefreshToolKits();
        }

        public void Update() 
        {
            for (int i = m_toolKits.Count - 1; i >= 0; i--) 
            {
                MyToolKit toolKit = m_toolKits[i];
                toolKit.Update();
                if (toolKit.IsEmpty()) 
                {
                    m_smallShipOwner.Inventory.OnInventoryContentChange -= OnInventoryContentChangedHandler;
                    m_smallShipOwner.Inventory.RemoveInventoryItem(toolKit.GetToolInventoryItem(), true);
                    m_smallShipOwner.Inventory.OnInventoryContentChange += OnInventoryContentChangedHandler;
                    m_toolKits.RemoveAt(i);
                }
            }
        }

        public void Close() 
        {
            m_smallShipOwner.Inventory.OnInventoryContentChange -= OnInventoryContentChangedHandler;
            OnInventoryContentChangedHandler = null;
            m_toolKits.Clear();
            m_toolKits = null;
            m_inventoryItemsHelper.Clear();
            m_inventoryItemsHelper = null;
            m_smallShipOwner = null;
            m_playerOwner = null;
        }

        private void RefreshToolKits() 
        {
            m_toolKits.Clear();
            m_inventoryItemsHelper.Clear();
            m_smallShipOwner.Inventory.GetInventoryItems(ref m_inventoryItemsHelper, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, null);
            foreach (MyInventoryItem inventoryItem in m_inventoryItemsHelper) 
            {
                if (MyToolKit.IsSupportedToolKitItem(inventoryItem)) 
                {
                    m_toolKits.Add(MyToolKit.CreateInstance(m_smallShipOwner, m_playerOwner, inventoryItem));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Game.Inventory
{
    class MyInventoryItemsRepository
    {
        private Dictionary<int, MyInventoryItem> m_itemsWithKeys;
        private int m_lastUsedKey;

        public MyInventoryItemsRepository()
        {
            m_itemsWithKeys = new Dictionary<int, MyInventoryItem>();
            m_lastUsedKey = 0;
        }

        public int AddItem(MyInventoryItem item)
        {
            int newKey = m_lastUsedKey++;
            m_itemsWithKeys.Add(newKey, item);
            return newKey;
        }

        public MyInventoryItem GetItem(int key)
        {
            return m_itemsWithKeys[key];
        }

        public bool Contains(int key)
        {
            return m_itemsWithKeys.ContainsKey(key);
        }

        public void RemoveItem(int key, bool closeItem = true)
        {
            if (closeItem)
            {
                MyInventory.CloseInventoryItem(GetItem(key));
            }
            m_itemsWithKeys.Remove(key);
        }

        public void Clear() 
        {
            m_itemsWithKeys.Clear();
        }

        public void Close()
        {
            foreach (KeyValuePair<int, MyInventoryItem> inventoryItemKVP in m_itemsWithKeys)
            {
                MyInventory.CloseInventoryItem(inventoryItemKVP.Value);
            }
            m_itemsWithKeys.Clear();
        }
    }
}

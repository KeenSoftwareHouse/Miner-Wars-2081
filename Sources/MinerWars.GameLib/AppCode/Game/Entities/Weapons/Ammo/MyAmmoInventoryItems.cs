using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons.Ammo
{
    /// <summary>
    /// This class is only helper class, to easy ammo usage, because ammo is now as inventory item in inventory and we can't find ammo in whole inventory
    /// </summary>
    class MyAmmoInventoryItems
    {
        // ammo inventory items collection
        private List<MyInventoryItem> m_ammoInventoryItems;
        private List<MyInventoryItem>[] m_ammoInventoryItemsByAmmoType;

        public MyAmmoInventoryItems() 
        {
            m_ammoInventoryItems = new List<MyInventoryItem>();
            m_ammoInventoryItemsByAmmoType = new List<MyInventoryItem>[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum>() + 1];
            for (int i = 0; i < m_ammoInventoryItemsByAmmoType.Length; i++)
            {
                m_ammoInventoryItemsByAmmoType[i] = new List<MyInventoryItem>();
            }
        }

        /// <summary>
        /// Initialize ammo inventory items by inventory items
        /// </summary>
        /// <param name="ammoInventoryItems"></param>
        public void Init(List<MyInventoryItem> ammoInventoryItems) 
        {
            m_ammoInventoryItems.Clear();
            m_ammoInventoryItems.AddRange(ammoInventoryItems);

            foreach (List<MyInventoryItem> inventoryItems in m_ammoInventoryItemsByAmmoType)
            {
                inventoryItems.Clear();
            }

            foreach (MyInventoryItem ammoInventoryItem in m_ammoInventoryItems)
            {
                if (ammoInventoryItem.ObjectBuilderId != null)
                {
                    m_ammoInventoryItemsByAmmoType[ammoInventoryItem.ObjectBuilderId.Value].Add(ammoInventoryItem);
                }
            }
        }

        /// <summary>
        /// Returns ammo objectuilders
        /// </summary>
        /// <returns></returns>
        public List<MyMwcObjectBuilder_SmallShip_Ammo> GetAmmoObjectBuilders(bool getExactCopy) 
        {
            List<MyMwcObjectBuilder_SmallShip_Ammo> ammoObjectBuilders = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
            foreach (MyInventoryItem ammoInventoryItem in m_ammoInventoryItems) 
            {
                ammoObjectBuilders.Add((MyMwcObjectBuilder_SmallShip_Ammo)ammoInventoryItem.GetInventoryItemObjectBuilder(getExactCopy));
            }
            return ammoObjectBuilders;
        }

        /// <summary>
        /// Returns ammo inventory items
        /// </summary>
        /// <returns></returns>
        public List<MyInventoryItem> GetAmmoInventoryItems() 
        {
            return m_ammoInventoryItems;
        }        

        /// <summary>
        /// Returns ammo inventory items by ammo type
        /// </summary>
        /// <param name="ammoType">Ammo type</param>
        /// <returns></returns>
        public List<MyInventoryItem> GetAmmoInventoryItems(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            return m_ammoInventoryItemsByAmmoType[(int) ammoType];            
        }

        /// <summary>
        /// Returns amoo inventoy items count
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return m_ammoInventoryItems.Count;
        }

        public void Close() 
        {
            m_ammoInventoryItems.Clear();
            m_ammoInventoryItems = null;

            for (int i = 0; i < m_ammoInventoryItemsByAmmoType.Length; i++) 
            {
                m_ammoInventoryItemsByAmmoType[i].Clear();
                m_ammoInventoryItemsByAmmoType[i] = null;
            }
            m_ammoInventoryItemsByAmmoType = null;
        }
    }
}

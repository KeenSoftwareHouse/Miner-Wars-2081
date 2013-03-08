#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Reflection;
using KeenSoftwareHouse.Library.Extensions;
using System.Runtime.Serialization;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using System.IO;

#endregion

namespace MinerWars.AppCode.Game.Inventory
{
    /// <summary>
    /// Defines inventory-able object.
    /// </summary>
    interface IMyInventory
    {
        /// <summary>
        /// Gets the inventory.
        /// </summary>
        MyInventory Inventory { get; }
    }

    delegate void OnInventoryContentChange(MyInventory sender);
    delegate void OnInventoryItemAmountChange(MyInventory sender, MyInventoryItem inventoryItem, float amountChanged);

    class MyInventory
    {
        #region Contants
        public const int DEFAULT_MAX_ITEMS = 200;
        public const float DEFAULT_PRICE_COEFICIENT = 1f;
        #endregion

        #region Fields
        private List<MyInventoryItem> m_inventoryItems = new List<MyInventoryItem>();
        private List<MyInventoryItem> m_helperInventoryItems = new List<MyInventoryItem>();
        private List<MyInventoryItem> m_helperInventoryItemsForAddAndRemove = new List<MyInventoryItem>();        

        private int m_maxItems;
        private OnAmountChange m_onItemAmountChangeHandler;
        private float m_priceCoeficient;
        private bool m_unlimitedCapacity;

        public bool IsDummy = false;
        #endregion

        #region Properties
        /// <summary>
        /// Maximum items in inventory
        /// </summary>
        public int MaxItems
        {
            get
            {
                return m_maxItems;
            }
            set
            {
                m_maxItems = value;
                FixItemsOverMaxLimit();
            }
        }

        /// <summary>
        /// Sets or gets unlimited inventory capacity indicator
        /// </summary>
        public bool UnlimitedCapacity 
        {
            get 
            {
                return m_unlimitedCapacity;
            }
            set 
            {
                m_unlimitedCapacity = value;
                if (!m_unlimitedCapacity) 
                {
                    FixItemsOverMaxLimit();
                }
            }
        }

        /// <summary>
        /// Indicates if inventory is full or not
        /// </summary>
        public bool IsFull 
        {
            get 
            {
                return m_inventoryItems.Count >= MaxItems && !UnlimitedCapacity;
            }
        }

        /// <summary>
        /// Intentory template to refill
        /// </summary>
        public MyMwcInventoryTemplateTypeEnum? TemplateType { get; set; }

        /// <summary>
        /// Price coeficient for trading
        /// </summary>
        public float PriceCoeficient 
        {
            get 
            {                
                return m_priceCoeficient;
            }
            set 
            {
                Debug.Assert(value >= 1f);
                m_priceCoeficient = value;
            }
        }

        /// <summary>
        /// Inventory synchronizer
        /// </summary>
        public MyInventorySynchronizer InventorySynchronizer { get; set; }        
        #endregion

        #region Static
        static MyObjectsPool<MyInventoryItem> InventoryItemsPool = new MyObjectsPool<MyInventoryItem>(MyConstants.INVENTORY_ITEM_POOL);

        public static MyInventoryItem CreateInventoryItemFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder)
        {
            return MyInventory.CreateInventoryItemFromObjectBuilder(objectBuilder, 1f);
        }

        public static MyInventoryItem CreateInventoryItemFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder, float amount)
        {
            float maxAmount = MyGameplayConstants.GetGameplayProperties(objectBuilder, MyMwcObjectBuilder_FactionEnum.Euroamerican).MaxAmount;
            float amountToAdd = Math.Min(maxAmount, amount);

            int objectBuilderId = objectBuilder.GetObjectBuilderId().HasValue ? objectBuilder.GetObjectBuilderId().Value : 0;

            MyGuiHelperBase guiHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(objectBuilder.GetObjectBuilderType(), objectBuilderId);
            MyCommonDebugUtils.AssertDebug(guiHelper != null);

            //warning: use default faction for get gameplay properties for inventory item
            MyGameplayProperties inventoryItemProperties = MyGameplayConstants.GetGameplayProperties(objectBuilder.GetObjectBuilderType(), objectBuilderId, MyMwcObjectBuilder_FactionEnum.Euroamerican);
            MyCommonDebugUtils.AssertDebug(inventoryItemProperties != null);

            
           
            MyInventoryItem item = MyInventory.InventoryItemsPool.Allocate();
            item.Start(guiHelper, inventoryItemProperties, objectBuilder, amountToAdd);
            return item;
        }        

        public static MyInventoryItem CreateInventoryItemFromInventoryItemObjectBuilder(MyMwcObjectBuilder_InventoryItem inventoryItemBuilder)
        {
            MyInventoryItem item = CreateInventoryItemFromObjectBuilder(inventoryItemBuilder.ItemObjectBuilder, inventoryItemBuilder.Amount);
            item.TemporaryFlags = inventoryItemBuilder.TemporaryFlags;
            return item;
        }

        public static void CloseInventoryItem(MyInventoryItem item)
        {
            item.Owner = null;
            MyInventory.InventoryItemsPool.Deallocate(item);
        }
        
        #endregion

        #region Ctors

        public MyInventory()
            : this(DEFAULT_MAX_ITEMS)
        {            
        }

        public MyInventory(int maxItems)
        {            
            MaxItems = maxItems;
            PriceCoeficient = DEFAULT_PRICE_COEFICIENT;
            m_onItemAmountChangeHandler = new OnAmountChange(OnAmountChange);
        }

        #endregion

        #region Events
        /// <summary>
        /// Called when content changed
        /// </summary>
        public event OnInventoryContentChange OnInventoryContentChange;

        /// <summary>
        /// Called when amount of any item in inventory changed
        /// </summary>
        public event OnInventoryItemAmountChange OnInventoryItemAmountChange;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize inventory from objectbuilder
        /// </summary>
        /// <param name="inventoryObjectBuilder">Inventory objectbuilder</param>        
        public void Init(MyMwcObjectBuilder_Inventory inventoryObjectBuilder, float defaultPriceCoeficient = DEFAULT_PRICE_COEFICIENT)
        {
            Debug.Assert(inventoryObjectBuilder != null);

            if (inventoryObjectBuilder.MaxItems == 0)
            {
                MaxItems = MyInventory.DEFAULT_MAX_ITEMS;
            }
            else
            {
                MaxItems = inventoryObjectBuilder.MaxItems;
            }
            
            PriceCoeficient = inventoryObjectBuilder.PriceCoeficient != null ? inventoryObjectBuilder.PriceCoeficient.Value : defaultPriceCoeficient;
            TemplateType = inventoryObjectBuilder.TemplateType;
            UnlimitedCapacity = inventoryObjectBuilder.UnlimitedCapacity;

            if (inventoryObjectBuilder.InventoryItems != null && inventoryObjectBuilder.InventoryItems.Count > 0)
            {
                RemoveAllInventoryItemsPrivate(true);
                List<MyInventoryItem> inventoryItemsToAdd = new List<MyInventoryItem>();
                foreach (MyMwcObjectBuilder_InventoryItem inventoryItemObjectBuilder in inventoryObjectBuilder.InventoryItems)
                {
                    if (inventoryItemsToAdd.Count >= MaxItems && !UnlimitedCapacity) 
                    {
                        break;
                    }
                    // if old foundation factory is in inventory, we replace it with new prefab foundation factory
                    if (inventoryItemObjectBuilder.ItemObjectBuilder is MyMwcObjectBuilder_FoundationFactory)
                    {
                        inventoryItemObjectBuilder.ItemObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT);
                    }

                    // hack: we need set default prefab health and max health to prefab object builder
                    if (inventoryItemObjectBuilder.ItemObjectBuilder is MyMwcObjectBuilder_PrefabBase)
                    {
                        MyMwcObjectBuilder_PrefabBase prefabBuilder = inventoryItemObjectBuilder.ItemObjectBuilder as MyMwcObjectBuilder_PrefabBase;
                        if (prefabBuilder.PrefabHealthRatio == 0f)
                        {
                            prefabBuilder.PrefabHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
                        }
                        if (prefabBuilder.PrefabMaxHealth == 0f)
                        {
                            prefabBuilder.PrefabMaxHealth = MyGameplayConstants.MAXHEALTH_PREFAB;
                        }
                    }

                    MyInventoryItem inventoryItem = CreateInventoryItemFromObjectBuilder(inventoryItemObjectBuilder.ItemObjectBuilder, inventoryItemObjectBuilder.Amount);
                    inventoryItem.TemporaryFlags = inventoryItemObjectBuilder.TemporaryFlags;
                    inventoryItemsToAdd.Add(inventoryItem);
                }                
                AddInventoryItems(inventoryItemsToAdd);
            }
            else 
            {
                ClearInventoryItems(true);
            }
        }

        /// <summary>
        /// Returns objectbuilder of inventory
        /// </summary>
        /// <returns></returns>
        public MyMwcObjectBuilder_Inventory GetObjectBuilder(bool getExactCopy)
        {
            List<MyMwcObjectBuilder_InventoryItem> inventoryItemsObjectBuilders = new List<MyMwcObjectBuilder_InventoryItem>();
            foreach (MyInventoryItem item in m_inventoryItems)
            {
                //MyMwcObjectBuilder_InventoryItem inventoryItemBuilder = new MyMwcObjectBuilder_InventoryItem(item.GetInventoryItemObjectBuilder(getExactCopy), item.Amount);
                MyMwcObjectBuilder_InventoryItem inventoryItemBuilder = item.GetObjectBuilder();
                if(inventoryItemBuilder.ItemObjectBuilder is MyMwcObjectBuilder_PrefabBase)
                {
                    MyMwcObjectBuilder_PrefabBase prefabBuilder = inventoryItemBuilder.ItemObjectBuilder as MyMwcObjectBuilder_PrefabBase;
                    if (prefabBuilder.PrefabHealthRatio == 0f)
                    {
                        prefabBuilder.PrefabHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
                    }
                    if (prefabBuilder.PrefabMaxHealth == 0f)
                    {
                        prefabBuilder.PrefabMaxHealth = MyGameplayConstants.MAXHEALTH_PREFAB;
                    }
                }
                inventoryItemsObjectBuilders.Add(inventoryItemBuilder);
            }
            MyMwcObjectBuilder_Inventory inventoryBuilder = new MyMwcObjectBuilder_Inventory(inventoryItemsObjectBuilders, MaxItems, TemplateType, PriceCoeficient);
            inventoryBuilder.UnlimitedCapacity = UnlimitedCapacity;
            return inventoryBuilder;
        }

        /// <summary>
        /// Deallocate inventory items
        /// </summary>
        public void Close()
        {
            CloseInventoryItemsPrivate();
            m_inventoryItems.Clear();
            OnInventoryContentChange = null;
            OnInventoryItemAmountChange = null;
            if (InventorySynchronizer != null) 
            {
                InventorySynchronizer.Close();
                InventorySynchronizer = null;
            }
            //m_inventoryItems = null;            
        }

        /// <summary>
        /// Returns all inventory items
        /// </summary>
        /// <returns></returns>
        public List<MyInventoryItem> GetInventoryItems()
        {
            return m_inventoryItems;
        }

        /// <summary>
        /// Called when content of inventory changed
        /// </summary>
        private void CallContentChange()
        {
            if (OnInventoryContentChange != null)
                OnInventoryContentChange(this);

        }

        /// <summary>
        /// Called where amount of inventory item changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="amountChanged"></param>
        private void OnAmountChange(MyInventoryItem sender, float amountChanged)
        {
            if (OnInventoryItemAmountChange != null)
            {
                OnInventoryItemAmountChange(this, sender, amountChanged);
            }
        }

        /// <summary>
        /// Adds inventory item to inventory
        /// </summary>
        /// <param name="item">Inventory item</param>
        public void AddInventoryItem(MyInventoryItem item)
        {
            AddItemToInventoryPrivate(item);
            CallContentChange();
        }

        /// <summary>
        /// Adds inventory items to inventory
        /// </summary>
        /// <param name="items">Inventory items</param>
        public void AddInventoryItems(IEnumerable<MyInventoryItem> items)
        {
            bool added = false;
            foreach (MyInventoryItem item in items)
            {
                AddItemToInventoryPrivate(item);
                added = true;
            }
            if (added)
            {
                CallContentChange();
            }
        }

        /// <summary>
        /// Removes inventory item from inventory
        /// </summary>
        /// <param name="item">Inventory item</param>
        /// <param name="closeInventoryItem">If true, then close inventory item's instance in pool</param>
        public void RemoveInventoryItem(MyInventoryItem item, bool closeInventoryItem = false)
        {
            if (RemoveItemFromInventoryPrivate(item, closeInventoryItem))
            {
                CallContentChange();
            }
        }

        /// <summary>
        /// Removes inventory items from inventory
        /// </summary>
        /// <param name="items">Inventory items</param>
        /// <param name="closeInventoryItems">If true, then close inventory item's instance in pool</param>
        public void RemoveInventoryItems(IEnumerable<MyInventoryItem> items, bool closeInventoryItems = false) 
        {
            bool removed = false;
            foreach (MyInventoryItem item in items)
            {
                if (RemoveItemFromInventoryPrivate(item, closeInventoryItems)) 
                {
                    removed = true;
                }
            }

            if (removed)
            {
                CallContentChange();
            }
        }

        //public void RemoveInventoryItemsByType(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId = null)
        //{
        //    for (int i = 0; i < m_inventoryItems.Count; i++)
        //    {
        //        MyInventoryItem inventoryItem = m_inventoryItems[i];

        //        if (inventoryItem.ObjectBuilderType == objectBuilderType && (objectBuilderId == null || inventoryItem.ObjectBuilderId == objectBuilderId))
        //        {
        //            RemoveInventoryItem(inventoryItem);
        //        }
        //    }
        //}


        /// <summary>
        /// Removes inventory items from inventory
        /// </summary>
        /// <param name="objectBuilder">Inventory item's object builder</param>
        /// <param name="closeInventoryItems">If true, then close inventory item's instance in pool</param>
        public void RemoveInventoryItems(MyMwcObjectBuilder_Base objectBuilder, bool closeInventoryItems = false)
        {
            RemoveInventoryItems(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId(), closeInventoryItems);
        }

        /// <summary>
        /// Removes inventory items from inventory
        /// </summary>
        /// <param name="objectBuilderType">Inventory item's object builder type</param>
        /// <param name="objectBuilderId">Inventory item's object builder id</param>
        /// <param name="closeInventoryItems">If true, then close inventory item's instance in pool</param>
        public void RemoveInventoryItems(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, bool closeInventoryItems = false)
        {
            m_helperInventoryItems.Clear();
            GetInventoryItems(ref m_helperInventoryItems, objectBuilderType, objectBuilderId);
            RemoveInventoryItems(m_helperInventoryItems, closeInventoryItems);
        }

        /// <summary>
        /// Clears inventory
        /// </summary>
        /// <param name="closeInventoryItem">If true, then close inventory item's instance in pool</param>
        public void ClearInventoryItems(bool closeInventoryItem = false) 
        {
            MyMwcLog.WriteLine("MyInventory::ClearInventoryItems - START");
            MyMwcLog.IncreaseIndent();

            bool removed = RemoveAllInventoryItemsPrivate(closeInventoryItem);            
            
            if (removed)
            {
                CallContentChange();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyInventory::ClearInventoryItems - END");
        }

        /// <summary>
        /// Fills inventory with all items
        /// </summary>        
        /// <param name="maxItems">Max items limit</param>
        public void FillInventoryWithAllItems(int? maxItems = null, int? increaseMaxItemsAbout = null, bool loadSmallShips = false)
        {
            if (maxItems != null) 
            {
                MaxItems = maxItems.Value;
            }
            MyInventoryItem inventoryItem;
            List<MyInventoryItem> inventoryItemsToAdd = new List<MyInventoryItem>();
            // ammo
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Ammo_TypesEnumValues)
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Ammo(item));
                inventoryItem.Amount = inventoryItem.MaxAmount;
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // engines
            foreach (MyMwcObjectBuilder_SmallShip_Engine_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Engine_TypesEnumValues)
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Engine(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // weapons
            foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Weapon_TypesEnumValues)
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Weapon(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // tools
            foreach (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_ToolEnumValues)
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Tool(item));
                inventoryItem.Amount = inventoryItem.MaxAmount;
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // armors
            foreach (MyMwcObjectBuilder_SmallShip_Armor_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Armor(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // radars
            foreach (MyMwcObjectBuilder_SmallShip_Radar_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Radar(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // blueprints
            foreach (MyMwcObjectBuilder_Blueprint_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_Blueprint_TypesEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_Blueprint(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            //ore             
            foreach (MyMwcObjectBuilder_Ore_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_Ore_TypesEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_Ore(item));
                inventoryItem.Amount = 1;
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // fakeId
            foreach (MyMwcObjectBuilder_FactionEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_FalseId(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // hacking tool
            foreach (MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum)))
            {
                inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_HackingTool(item));
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // drone
            foreach (MyMwcObjectBuilder_Drone_TypesEnum droneType in Enum.GetValues(typeof(MyMwcObjectBuilder_Drone_TypesEnum)))
            {
                var droneBuilder = new MyMwcObjectBuilder_Drone(droneType);
                inventoryItem = CreateInventoryItemFromObjectBuilder(droneBuilder);
                inventoryItem.Amount = inventoryItem.MaxAmount;
                inventoryItemsToAdd.Add(inventoryItem);
            }
            // playerships
            if (loadSmallShips)
            {
                foreach (MyMwcObjectBuilder_SmallShip_TypesEnum shipType in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_TypesEnum)))
                {
                    inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(MyMwcObjectBuilder_SmallShip_Player.CreateDefaultShip(shipType, MySession.PlayerShip.Faction, MyShipTypeConstants.GetShipTypeProperties(shipType).GamePlay.CargoCapacity));
                    inventoryItemsToAdd.Add(inventoryItem);
                }
            }

            //// prefabs
            //foreach(MyMwcObjectBuilder_Prefab_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_Prefab_TypesEnum))) {
            //    inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_Prefab(item, new MyMwcVector3Short(0, 0, 0), new Vector3(0, 0, 0), MyGameplayConstants.MAXHEALTH_PREFAB));
            //    inventoryItemsToAdd.Add(inventoryItem);
            //}
            //// small debris
            //foreach (MyMwcObjectBuilder_SmallDebris_TypesEnum item in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallDebris_TypesEnum)))
            //{
            //    inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallDebris(item, true));
            //    inventoryItemsToAdd.Add(inventoryItem);
            //}
            // foundation factory
            inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT));
            inventoryItemsToAdd.Add(inventoryItem);

            if (maxItems != null && !UnlimitedCapacity)
            {
                while (inventoryItemsToAdd.Count > maxItems.Value) 
                {
                    int lastIndex = inventoryItemsToAdd.Count - 1;
                    inventoryItem = inventoryItemsToAdd[lastIndex];
                    inventoryItemsToAdd.RemoveAt(lastIndex);      
                    MyInventory.CloseInventoryItem(inventoryItem);                                  
                }                
            }
            else 
            {
                if (inventoryItemsToAdd.Count > MaxItems) 
                {
                    MaxItems = inventoryItemsToAdd.Count;
                }
            }
            if (increaseMaxItemsAbout != null) 
            {
                MaxItems += increaseMaxItemsAbout.Value;
            }
            AddInventoryItems(inventoryItemsToAdd);
        }

        /// <summary>
        /// Find inventory items in inventory
        /// </summary>
        /// <param name="inventoryItems">Collection to fill founded inventory items</param>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builder Id</param>
        /// <returns></returns>
        public void GetInventoryItems(ref List<MyInventoryItem> inventoryItems, MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId)
        {            
            foreach (MyInventoryItem inventoryItem in m_inventoryItems)
            {
                if (inventoryItem.ObjectBuilderType == objectBuilderType && (objectBuilderId == null || inventoryItem.ObjectBuilderId == objectBuilderId))
                {
                    inventoryItems.Add(inventoryItem);
                }
            }
        }

        /// <summary>
        /// Find inventory items from inventory
        /// </summary>
        /// <param name="inventoryItems">Collection to fill founded inventory items</param>
        /// <param name="objectBuilder">Item's object builder</param>
        /// <returns></returns>
        public void GetInventoryItems(ref List<MyInventoryItem> inventoryItems, MyMwcObjectBuilder_Base objectBuilder)
        {
            GetInventoryItems(ref inventoryItems, objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId());
        }

        /// <summary>
        /// Returns first inventory item from inventory
        /// </summary>
        /// <param name="objectBuilder">Item's object builder</param>
        /// <returns></returns>
        public MyInventoryItem GetInventoryItem(MyMwcObjectBuilder_Base objectBuilder) 
        {
            return GetInventoryItem(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId());
        }

        /// <summary>
        /// Returns first inventory item from inventory
        /// </summary>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builder id</param>
        /// <returns></returns>
        public MyInventoryItem GetInventoryItem(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId) 
        {
            m_helperInventoryItems.Clear();
            GetInventoryItems(ref m_helperInventoryItems, objectBuilderType, objectBuilderId);
            if (m_helperInventoryItems.Count > 0)
            {
                return m_helperInventoryItems[0];
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Removes amout of inventory items from inventory.
        /// </summary>
        /// <param name="inventoryItem">Inventory item</param>
        /// <param name="amount">Amount to remove</param>
        public void RemoveInventoryItemAmount(ref MyInventoryItem inventoryItem, float amount)
        {
            //inventoryItem.Amount -= amount;
            //if (inventoryItem.Amount <= 0f)
            //{
            //    RemoveInventoryItem(inventoryItem, true);                
            //}
            if (RemoveInventoryItemAmountPrivate(inventoryItem, amount))             
            {
                CallContentChange();
            }
        }

        /// <summary>
        /// Removes amout of inventory items from inventory.
        /// </summary>
        /// <param name="objectBuilder">Item's object builder</param>
        /// <param name="amount">Amount to remove</param>
        public void RemoveInventoryItemAmount(MyMwcObjectBuilder_Base objectBuilder, float amount)
        {
            RemoveInventoryItemAmount(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId(), amount);
        }

        /// <summary>
        /// Returns inventory items from inventory 
        /// </summary>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builder id</param>
        /// <param name="amount">Amount to remove</param>
        public bool RemoveInventoryItemAmount(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float amount)
        {
            bool removed = false;
            float inventoryItemsAmount = GetTotalAmountOfInventoryItems(objectBuilderType, objectBuilderId);

            if (inventoryItemsAmount < amount)
            {
                return false;
            }

            float amountToRemoveLeft = amount;
            m_helperInventoryItemsForAddAndRemove.Clear();
            GetInventoryItems(ref m_helperInventoryItemsForAddAndRemove, objectBuilderType, objectBuilderId);
            m_helperInventoryItemsForAddAndRemove.Sort((x, y) => x.Amount.CompareTo(y.Amount));
            foreach (MyInventoryItem inventoryItem in m_helperInventoryItemsForAddAndRemove)
            {
                float amountToRemove = Math.Min(inventoryItem.Amount, amountToRemoveLeft);

                MyInventoryItem inventoryItemToRemoveAmount = inventoryItem;
                if (RemoveInventoryItemAmountPrivate(inventoryItemToRemoveAmount, amountToRemove)) 
                {
                    removed = true;
                }

                amountToRemoveLeft -= amountToRemove;

                if (amountToRemoveLeft <= 0f)
                {
                    break;
                }
            }

            if (removed) 
            {
                CallContentChange();
            }

            return true;
        }

        /// <summary>
        /// Adds new inventory item from object builder type and id
        /// </summary>
        /// <param name="objectBuilderType">Objectbuilder type</param>
        /// <param name="objectBuilderId">Objectbuilder id</param>
        /// <param name="amount">Amount of inventory item</param>
        /// <param name="allAmmountAddAsNewInventoryItems">If true, then all amount adds as new inventory items, if false, then try find old inventory items of same type, and add amount to them as first</param>        
        public float AddInventoryItem(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float amount, bool allAmmountAddAsNewInventoryItems, bool increaseCapacityIfIsFull = false)
        {
            MyMwcObjectBuilder_Base objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(objectBuilderType, objectBuilderId);
            return AddInventoryItem(objectBuilder, amount, allAmmountAddAsNewInventoryItems, increaseCapacityIfIsFull);
        }

        /// <summary>
        /// Adds new inventory item from object builder
        /// </summary>
        /// <param name="objectBuilder">Item object builder</param>
        /// <param name="amount">Amount of inventory item</param>
        /// <param name="allAmountAddAsNewInventoryItems">If true, then all amount adds as new inventory items, if false, then try find old inventory items of same type, and add amount to them as first</param>        
        public float AddInventoryItem(MyMwcObjectBuilder_Base objectBuilder, float amount, bool allAmountAddAsNewInventoryItems, bool increaseCapacityIfIsFull = false)
        {
            bool added = false;
            MyMwcObjectBuilderTypeEnum objectBuilderType = objectBuilder.GetObjectBuilderType();
            int? objectBuilderId = objectBuilder.GetObjectBuilderId();
            m_helperInventoryItemsForAddAndRemove.Clear();
            GetInventoryItems(ref m_helperInventoryItemsForAddAndRemove, objectBuilderType, objectBuilderId);

            float amountToAddLeft = amount;
            if (!allAmountAddAsNewInventoryItems)
            {
                foreach (MyInventoryItem inventoryItem in m_helperInventoryItemsForAddAndRemove)
                {
                    if (amountToAddLeft <= 0f)
                        break;
                    float amountToAdd = Math.Min(inventoryItem.MaxAmount - inventoryItem.Amount, amountToAddLeft);
                    if (InventorySynchronizer != null && InventorySynchronizer.MustBeSynchronized())
                    {
                        InventorySynchronizer.AddInventoryItemAmountChangeForSynchronization(objectBuilderType, objectBuilderId, inventoryItem.Amount, amountToAdd);
                    }
                    else
                    {
                        inventoryItem.Amount += amountToAdd;
                    }
                    amountToAddLeft -= amountToAdd;
                }
            }

            while (amountToAddLeft > 0f)
            {
                if (IsFull)
                {
                    if (increaseCapacityIfIsFull)
                    {
                        MaxItems++;
                    }
                    else 
                    {
                        break;
                    }
                }
                MyInventoryItem newInventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(objectBuilder);
                float amountToAdd = Math.Min(newInventoryItem.MaxAmount, amountToAddLeft);
                //AddInventoryItem(newInventoryItem);
                AddItemToInventoryPrivate(newInventoryItem);
                newInventoryItem.Amount = amountToAdd; //After AddInventoryItem otherwise Amount event is not called
                amountToAddLeft -= amountToAdd;
                added = true;
            }

            if (added)
            {
                CallContentChange();
            }            

            return amountToAddLeft;
        }                        

        /// <summary>
        /// Returns total amount of inventory items in inventory
        /// </summary>
        /// <param name="objectBuilder">Item's object builder</param>
        /// <returns></returns>
        public float GetTotalAmountOfInventoryItems(MyMwcObjectBuilder_Base objectBuilder)
        {
            return GetTotalAmountOfInventoryItems(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId());
        }

        /// <summary>
        /// Returns total amount of inventory items in inventory
        /// </summary>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builder id</param>
        /// <returns></returns>
        public float GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId)
        {
            m_helperInventoryItems.Clear();
            GetInventoryItems(ref m_helperInventoryItems, objectBuilderType, objectBuilderId);

            float inventoryItemsAmount = 0;
            foreach (MyInventoryItem inventoryItem in m_helperInventoryItems)
            {
                inventoryItemsAmount += inventoryItem.Amount;
            }

            return inventoryItemsAmount;
        }

        /// <summary>
        /// Returns inventory item's count
        /// </summary>
        /// <param name="objectBuilder">Item's object builder</param>
        /// <returns></returns>
        public int GetInventoryItemsCount(MyMwcObjectBuilder_Base objectBuilder) 
        {
            return GetInventoryItemsCount(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId());
        }

        /// <summary>
        /// Returns inventory item's count
        /// </summary>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builde id</param>
        /// <returns></returns>
        public int GetInventoryItemsCount(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId)
        {
            m_helperInventoryItems.Clear();
            GetInventoryItems(ref m_helperInventoryItems, objectBuilderType, objectBuilderId);            
            return m_helperInventoryItems.Count;
        }

        /// <summary>
        /// Returns true if inventory contains item
        /// </summary>
        /// <param name="objectBuilder">Item's objectbuilder</param>
        /// <returns></returns>
        public bool Contains(MyMwcObjectBuilder_Base objectBuilder)
        {
            return Contains(objectBuilder.GetObjectBuilderType(), objectBuilder.GetObjectBuilderId());
        }

        /// <summary>
        /// Returns true if inventory contains item
        /// </summary>
        /// <param name="objectBuilderType">Item's object builder type</param>
        /// <param name="objectBuilderId">Item's object builder id</param>
        /// <returns></returns>
        public bool Contains(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId)
        {
            m_helperInventoryItems.Clear();
            GetInventoryItems(ref m_helperInventoryItems, objectBuilderType, objectBuilderId);
            return m_helperInventoryItems.Count > 0;
        }        

        private void AddItemToInventoryPrivate(MyInventoryItem item)
        {
            if (IsDisabled(item))
            {
                MyInventory.CloseInventoryItem(item);
            }
            else
            {
                if (InventorySynchronizer != null && InventorySynchronizer.MustBeSynchronized())
                {
                    InventorySynchronizer.AddInventoryItemForSynchronization(item);
                }
                else
                {
                    if (!UnlimitedCapacity)
                    {
                        if (m_inventoryItems.Count >= m_maxItems)
                            throw new Exception("Inventory has full capacity");
                    }

                    item.OnAmountChange += m_onItemAmountChangeHandler;
                    item.Owner = this;
                    m_inventoryItems.Add(item);
                    OnAmountChange(item, item.Amount);

                    if (UnlimitedCapacity)
                    {
                        MaxItems = Math.Max(m_inventoryItems.Count, MaxItems);
                    }
                }
            }
        }        

        public bool IsInventoryEmpty()
        {
            return m_inventoryItems.Count == 0;
        }

        public int GetFreeSlotsCount() 
        {
            return MaxItems - m_inventoryItems.Count;
        }

        public static void GenerateDebugInventoryItemsInfo() 
        {
            string format = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(format, "Type", "ID", "Max amount", "Price per unit", "Description", "Name"));
            MyInventory degubInventory = new MyInventory(1500);
            degubInventory.FillInventoryWithAllItems(null, null, true);
            foreach (var item in degubInventory.GetInventoryItems()) 
            {
                sb.AppendLine(string.Format(format, item.ObjectBuilderType, item.ObjectBuilderId, item.MaxAmount, item.PricePerUnit, item.Description, item.GuiHelper.DescriptionEnum.ToString()));
            }
            string directory = @"C:\Temp";
            string fileName = directory + @"\InventoryItemsInfo.txt";

            if (File.Exists(fileName)) 
            {
                File.Delete(fileName);
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var sw = File.CreateText(fileName)) 
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
        }

        private static bool IsDisabled(MyInventoryItem item) 
        {
            return MyMwcObjectBuilder_InventoryItem.IsDisabled(item.ObjectBuilderType, item.ObjectBuilderId);
        }

        private bool RemoveItemFromInventoryPrivate(MyInventoryItem item, bool closeInventoryItem) 
        {            
            bool result = m_inventoryItems.Remove(item);
            item.OnAmountChange -= m_onItemAmountChangeHandler;
            if (item.Amount > 0f) 
            {
                OnAmountChange(item, -item.Amount);
            }
            if (closeInventoryItem)
            {
                MyInventory.CloseInventoryItem(item);
            }
            else 
            {
                item.Owner = null;
            }
            return result;
        }

        private void CloseInventoryItemsPrivate()
        {
            foreach (MyInventoryItem item in m_inventoryItems)
            {
                item.OnAmountChange -= m_onItemAmountChangeHandler;
                MyInventory.CloseInventoryItem(item);
            }
        }

        private bool RemoveAllInventoryItemsPrivate(bool closeInventoryItem) 
        {
            bool removed = false;
            for (int i = m_inventoryItems.Count - 1; i >= 0; i--)
            {
                if (RemoveItemFromInventoryPrivate(m_inventoryItems[i], closeInventoryItem))
                {
                    removed = true;
                }
            }
            return removed;
        }

        private bool RemoveInventoryItemAmountPrivate(MyInventoryItem inventoryItem, float amount) 
        {
            bool removed = false;
            inventoryItem.Amount -= amount;
            if (inventoryItem.Amount <= 0f)
            {
                removed = RemoveItemFromInventoryPrivate(inventoryItem, true);
            }
            return removed;
        }

        private void FixItemsOverMaxLimit() 
        {
            bool removed = false;
            while (m_inventoryItems.Count > MaxItems)
            {
                MyInventoryItem itemToRemove = m_inventoryItems[m_inventoryItems.Count - 1];
                RemoveInventoryItem(itemToRemove, true);
                removed = true;
            }
            if (removed)
            {
                CallContentChange();
            }
        }

        #endregion
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MinerWars.AppCode.Game.Editor;
//using MinerWars.AppCode.Game.GUI.Core;
//using Microsoft.Xna.Framework;
//using MinerWars.AppCode.Game.Utils;
//using MinerWars.AppCode.Game.Localization;
//using MinerWars.AppCode.Game.Managers.Graphics.Textures;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
//using MinerWars.AppCode.Game.GUI.Helpers;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
//using MinerWars.CommonLIB.AppCode.Utils;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
//using SysUtils.Utils;
//using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders;
//using MinerWars.AppCode.Game.Inventory;
//using MinerWars.AppCode.Game.World.Global;
//using MinerWars.AppCode.Game.World;
//using MinerWars.AppCode.Game.Managers.Session;

//namespace MinerWars.AppCode.Game.GUI
//{
//    internal enum MyInventoryScreenType
//    {
//        CustomizationEditor,
//        Customization,
//        TradeForFree,
//        TradeForMoney,
//        Steal
//    }

//    internal static class MyGuiScreenInventoryFactory
//    {
//        public static MyGuiScreenInventory CreateInstance(bool isEditor)
//        {
//            MyInventory otherSideInventory = null;
//            StringBuilder otherSideInventoryName = null;
//            MyInventoryScreenType inventoryScreenType;
//            MyDetectedEntity detectedEntity = null;
//            if (isEditor)
//            {
//                if (MyGuiScreenGamePlay.Static.IsIngameEditorActive())
//                {
//                    otherSideInventory = MyEditor.Static.FoundationFactory.Inventory;
//                    otherSideInventoryName = new StringBuilder();
//                    otherSideInventoryName.Append(MyEditor.Static.FoundationFactory.Name);
//                    otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSideInventory));
//                    inventoryScreenType = MyInventoryScreenType.TradeForFree;
//                }
//                else
//                {
//                    otherSideInventory = new MyInventory();
//                    otherSideInventory.MaxItems = 1000;
//                    otherSideInventory.FillInventoryWithAllItems();
//                    otherSideInventoryName = MyTextsWrapper.Get(MyTextsWrapperEnum.AllItemsInventory);
//                    inventoryScreenType = MyInventoryScreenType.CustomizationEditor;
//                }
//            }
//            else
//            {
//                MyShipDetector detector = MySession.PlayerShip.ShipDetector;
//                if (detector != null)
//                {
//                    detectedEntity = detector.GetNearestDetectedEntity();
//                }

//                if (detectedEntity != null)
//                {
//                    IMyInventory detectedEntityInventory = detectedEntity.Entity as IMyInventory;

//                    otherSideInventoryName = new StringBuilder();
//                    otherSideInventoryName.Append(detectedEntity.Entity.Name);
//                    otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSideInventory));

//                    if (detectedEntity.DetectedAction == MyDetectedEntityAction.TradeForFree)
//                    {
//                        inventoryScreenType = MyInventoryScreenType.TradeForFree;
//                    }
//                    else if (detectedEntity.DetectedAction == MyDetectedEntityAction.Steal)
//                    {
//                        inventoryScreenType = MyInventoryScreenType.Steal;
//                    }
//                    else if (detectedEntity.DetectedAction == MyDetectedEntityAction.TradeForMoney)
//                    {
//                        inventoryScreenType = MyInventoryScreenType.TradeForMoney;
//                    }
//                    else if (detectedEntity.DetectedAction == MyDetectedEntityAction.Build)
//                    {
//                        inventoryScreenType = MyInventoryScreenType.TradeForFree;
//                    }
//                    else
//                    {
//                        throw new MyMwcExceptionApplicationShouldNotGetHere();
//                    }
//                    otherSideInventory = detectedEntityInventory.Inventory;
//                }
//                else
//                {
//                    inventoryScreenType = MyInventoryScreenType.Customization;
//                }
//            }
//            MyGuiScreenInventory inventory = new MyGuiScreenInventory(MySession.Static.Player, otherSideInventory, otherSideInventoryName, inventoryScreenType);
//            if (detectedEntity != null)
//            {
//                detectedEntity.OnLost += inventory.CancelTransfer;
//            }
//            return inventory;
//        }
//    }

//    internal class MyGuiScreenInventory : MyGuiScreenBase
//    {
//        private class MyItemsRepository
//        {
//            private Dictionary<int, MyInventoryItem> m_itemsWithKeys;

//            public MyItemsRepository()
//            {
//                m_itemsWithKeys = new Dictionary<int, MyInventoryItem>();
//            }

//            public int AddItem(MyInventoryItem item)
//            {
//                int newKey = m_itemsWithKeys.Count + 1;
//                m_itemsWithKeys.Add(newKey, item);
//                return newKey;
//            }

//            public MyInventoryItem GetItem(int key)
//            {
//                return m_itemsWithKeys[key];
//            }
//        }

//        private class MySmallShipInventoryItems
//        {
//            public List<MyInventoryItem> Inventory { get; set; }
//            public List<MyInventoryItem> LeftWeapons { get; set; }
//            public List<MyInventoryItem> RightWeapons { get; set; }
//            public MyInventoryItem Radar { get; set; }
//            public MyInventoryItem Engine { get; set; }
//            public MyInventoryItem Armor { get; set; }
//            public MyInventoryItem Harvester { get; set; }
//            public MyInventoryItem Drill { get; set; }
//            public MyInventoryItem FrontLauncher { get; set; }
//            public MyInventoryItem BackLauncher { get; set; }

//            public MySmallShipInventoryItems()
//            {
//                Inventory = new List<MyInventoryItem>();
//                LeftWeapons = new List<MyInventoryItem>();
//                RightWeapons = new List<MyInventoryItem>();                
//            }

//            public void SaveToSmallShipObjectBuilder(MyMwcObjectBuilder_SmallShip smallShipObjectBuilder)
//            {
//                // inventory and inventory items
//                if (smallShipObjectBuilder.Inventory == null)
//                {
//                    smallShipObjectBuilder.Inventory = new MyMwcObjectBuilder_Inventory();
//                    smallShipObjectBuilder.Inventory.MaxItems = 1000;
//                }
//                else
//                {
//                    if (smallShipObjectBuilder.Inventory.InventoryItems == null)
//                    {
//                        smallShipObjectBuilder.Inventory.InventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();
//                    }
//                    else
//                    {
//                        smallShipObjectBuilder.Inventory.InventoryItems.Clear();
//                    }
//                }
//                foreach (MyInventoryItem inventoryItem in Inventory)
//                {
//                    smallShipObjectBuilder.Inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(inventoryItem.GetObjectBuilder(true), inventoryItem.Amount));
//                }

//                // weapons
//                if (smallShipObjectBuilder.Weapons == null)
//                {
//                    smallShipObjectBuilder.Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
//                }
//                foreach (MyInventoryItem weapon in LeftWeapons)
//                {
//                    smallShipObjectBuilder.Weapons.Add(weapon.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }
//                foreach (MyInventoryItem weapon in RightWeapons)
//                {
//                    smallShipObjectBuilder.Weapons.Add(weapon.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }
//                if (Harvester != null)
//                {
//                    smallShipObjectBuilder.Weapons.Add(Harvester.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }
//                if (Drill != null)
//                {
//                    smallShipObjectBuilder.Weapons.Add(Drill.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }
//                if (FrontLauncher != null)
//                {
//                    smallShipObjectBuilder.Weapons.Add(FrontLauncher.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }
//                if (BackLauncher != null)
//                {
//                    smallShipObjectBuilder.Weapons.Add(BackLauncher.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Weapon);
//                }

//                // armor
//                if (Armor != null)
//                {
//                    smallShipObjectBuilder.Armor = Armor.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Armor;
//                }

//                // radar
//                if (Radar != null)
//                {
//                    smallShipObjectBuilder.Radar = Radar.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Tool;
//                }

//                // engine
//                if (Engine != null)
//                {
//                    smallShipObjectBuilder.Engine = Engine.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Engine;
//                }
//            }

//            public void LoadFromSmallShipObjectBuilder(MyMwcObjectBuilder_SmallShip smallShipObjectBuilder)
//            {
//                // inventory
//                if (smallShipObjectBuilder.Inventory != null)
//                {
//                    foreach (MyMwcObjectBuilder_InventoryItem inventoryItem in smallShipObjectBuilder.Inventory.InventoryItems)
//                    {
//                        Inventory.Add(MyInventory.CreateInventoryItemFromInventoryItemObjectBuilder(inventoryItem));
//                    }
//                }

//                // weapons
//                if (smallShipObjectBuilder.Weapons != null)
//                {
//                    foreach (MyMwcObjectBuilder_SmallShip_Weapon weapon in smallShipObjectBuilder.Weapons)
//                    {
//                        if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher ||
//                            weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser ||
//                            weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear ||
//                            weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure ||
//                            weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw ||
//                            weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal)
//                        {
//                            Drill = MyInventory.CreateInventoryItemFromObjectBuilder(weapon);
//                        }
//                        else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device)
//                        {
//                            Harvester = MyInventory.CreateInventoryItemFromObjectBuilder(weapon);
//                        }
//                        else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front)
//                        {
//                            FrontLauncher = MyInventory.CreateInventoryItemFromObjectBuilder(weapon);
//                        }
//                        else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back)
//                        {
//                            BackLauncher = MyInventory.CreateInventoryItemFromObjectBuilder(weapon);
//                        }
//                        else
//                        {
//                            if (LeftWeapons.Count < INVENTORY_WEAPONS_COLUMNS)
//                            {
//                                LeftWeapons.Add(MyInventory.CreateInventoryItemFromObjectBuilder(weapon));
//                            }
//                            else if (RightWeapons.Count < INVENTORY_WEAPONS_COLUMNS)
//                            {
//                                RightWeapons.Add(MyInventory.CreateInventoryItemFromObjectBuilder(weapon));
//                            }
//                            else
//                            {
//                                Inventory.Add(MyInventory.CreateInventoryItemFromObjectBuilder(weapon));
//                            }
//                        }
//                    }
//                }

//                // radar
//                if (smallShipObjectBuilder.Radar != null)
//                {
//                    Radar = MyInventory.CreateInventoryItemFromObjectBuilder(smallShipObjectBuilder.Radar);
//                }

//                // armor
//                if (smallShipObjectBuilder.Armor != null)
//                {
//                    Armor = MyInventory.CreateInventoryItemFromObjectBuilder(smallShipObjectBuilder.Armor);
//                }

//                // engine
//                if (smallShipObjectBuilder.Engine != null)
//                {
//                    Engine = MyInventory.CreateInventoryItemFromObjectBuilder(smallShipObjectBuilder.Engine);
//                }
//            }

//            public void CloseInventoryItems()
//            {
//                foreach (MyInventoryItem inventoryItem in Inventory)
//                {
//                    MyInventory.CloseInventoryItem(inventoryItem);
//                }
//                foreach (MyInventoryItem leftWeapon in LeftWeapons)
//                {
                    
//                }
//            }
//        }

//        private MyGuiControlListbox m_shipInventoryListBox;
//        private MyGuiControlListbox m_otherSideInventoryListBox;
//        private MyGuiControlListbox m_leftWeaponsInventoryListBox;
//        private MyGuiControlListbox m_rightWeaponsInventoryListBox;
//        private MyGuiControlListbox m_radarInventoryListBox;
//        private MyGuiControlListbox m_armorInventoryListBox;
//        private MyGuiControlListbox m_engineInventoryListBox;
//        private MyGuiControlListbox m_frontUniversalLauncherInventoryListBox;
//        private MyGuiControlListbox m_backUniversalLauncherInventoryListBox;
//        private MyGuiControlListbox m_drillInventoryListBox;
//        private MyGuiControlListbox m_harvestingToolInventoryListBox;
//        private MyGuiControlListboxDragAndDrop m_dragAndDrop;
//        private MyGuiControlLabel m_moneyTradeBalanceLabel;
//        private MyGuiControlLabel m_playersMoneyLabel;

//        private Dictionary<MyGuiControlListbox, Predicate<MyInventoryItem>> m_listboxDropConditions;
//        private MyItemsRepository m_itemsRepository;
//        private MyInventory m_otherSideInventory;
//        private MyInventory m_shipInventory;
//        private List<MyInventoryItem> m_removedInventoryItems;
//        private float m_moneyTradeBalance;
//        private MyPlayer m_player;
//        private MySmallShip m_playerShip;
//        private MyInventoryScreenType m_inventoryScreenType;


//        private const int INVENTORY_COLUMNS = 2;
//        private const int INVENTORY_DISPLAY_ROWS = 6;
//        private const int INVENTORY_WEAPONS_COLUMNS = 5;

//        public MyGuiScreenInventory(MyPlayer player, MyInventory otherSideInventory,
//                                    StringBuilder otherSideInventoryName, MyInventoryScreenType inventoryScreenType)
//            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(1.0f, 1.0f))
//        {
//            m_player = player;
//            m_playerShip = m_player.Ship as MySmallShip;
//            if (m_playerShip == null)
//            {
//                throw new Exception("Player's ship is not a MySmallShip");
//            }
//            m_shipInventory = m_playerShip.Inventory;
//            m_otherSideInventory = otherSideInventory;
//            m_removedInventoryItems = new List<MyInventoryItem>();
//            m_inventoryScreenType = inventoryScreenType;
//            m_moneyTradeBalance = 0f;

//            m_itemsRepository = new MyItemsRepository();

//            InitControls(otherSideInventoryName);
//            LoadInventoryContent();
//        }

//        private void InitControls(StringBuilder otherSideInventoryName)
//        {
//            if (m_inventoryScreenType == MyInventoryScreenType.CustomizationEditor)
//            {
//                AddCaption(MyTextsWrapperEnum.ShipCustomizationCaption);
//            }
//            else
//            {
//                AddCaption(MyTextsWrapperEnum.ShipInventoryCaption);
//            }
//            m_enableBackgroundFade = true;
//            m_drawBackgroundInterference = true;

//            Vector2 topLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.1f);
//            Vector2 topRight = new Vector2(m_size.Value.X / 2.0f - 0.05f, -m_size.Value.Y / 2.0f + 0.1f);

//            List<MyGuiControlListbox> listboxToDrop = new List<MyGuiControlListbox>();
//            m_listboxDropConditions = new Dictionary<MyGuiControlListbox, Predicate<MyInventoryItem>>();

//            #region my ship's inventory and customization

//            // Ship's inventory label
//            Controls.Add(new MyGuiControlLabel(this,
//                                               topRight + 0 * MyGuiConstants.CONTROLS_DELTA -
//                                               new Vector2(
//                                                   MyGuiConstants.LISTBOX_SMALL_SIZE.X * (float)INVENTORY_COLUMNS +
//                                                   MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH, 0), null,
//                                               MyTextsWrapperEnum.ShipInventory, MyGuiConstants.LABEL_TEXT_COLOR,
//                                               MyGuiConstants.LABEL_TEXT_SCALE,
//                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

//            // Ship inventory listbox
//            m_shipInventoryListBox = new MyGuiControlListbox(this,
//                                                             topRight + 1 * MyGuiConstants.CONTROLS_DELTA -
//                                                             new Vector2(
//                                                                 MyGuiConstants.LISTBOX_SMALL_SIZE.X *
//                                                                 (float)INVENTORY_COLUMNS +
//                                                                 MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH, 0),
//                                                             MyGuiControlPreDefinedSize.SMALL,
//                                                             MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                             MyTextsWrapper.Get(MyTextsWrapperEnum.ShipInventory),
//                                                             MyGuiConstants.LABEL_TEXT_SCALE,
//                                                             INVENTORY_COLUMNS, INVENTORY_DISPLAY_ROWS,
//                                                             INVENTORY_COLUMNS, true, true, false);
//            InitializeListboxDragAndAddToControls(m_shipInventoryListBox);
//            listboxToDrop.Add(m_shipInventoryListBox);
//            m_listboxDropConditions.Add(m_shipInventoryListBox, ii => true);

//            // Left weapons inventory listbox
//            m_leftWeaponsInventoryListBox = new MyGuiControlListbox(this,
//                                                                    new Vector2(
//                                                                        topLeft.X +
//                                                                        MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f, 0.2f),
//                                                                    MyGuiControlPreDefinedSize.SMALL,
//                                                                    MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                    MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
//                                                                    MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                    INVENTORY_WEAPONS_COLUMNS, 1,
//                                                                    INVENTORY_WEAPONS_COLUMNS, true, false, false);
//            InitializeListboxDragAndAddToControls(m_leftWeaponsInventoryListBox);
//            listboxToDrop.Add(m_leftWeaponsInventoryListBox);
//            m_listboxDropConditions.Add(m_leftWeaponsInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
//                                        false);

//            // Right weapons inventory listbox
//            m_rightWeaponsInventoryListBox = new MyGuiControlListbox(this,
//                                                                     new Vector2(
//                                                                         topRight.X -
//                                                                         ((float)INVENTORY_WEAPONS_COLUMNS - 0.5f) *
//                                                                         MyGuiConstants.LISTBOX_SMALL_SIZE.X, 0.2f),
//                                                                     MyGuiControlPreDefinedSize.SMALL,
//                                                                     MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RightWeapons),
//                                                                     MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                     INVENTORY_WEAPONS_COLUMNS, 1,
//                                                                     INVENTORY_WEAPONS_COLUMNS, true, false, false);
//            InitializeListboxDragAndAddToControls(m_rightWeaponsInventoryListBox);
//            listboxToDrop.Add(m_rightWeaponsInventoryListBox);
//            m_listboxDropConditions.Add(m_rightWeaponsInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
//                                        false);

//            // Engine inventory listbox
//            m_engineInventoryListBox = new MyGuiControlListbox(this,
//                                                               new Vector2(
//                                                                   -0.025f - MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f,
//                                                                   0.3f), MyGuiControlPreDefinedSize.SMALL,
//                                                               MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                               MyTextsWrapper.Get(MyTextsWrapperEnum.Engine),
//                                                               MyGuiConstants.LABEL_TEXT_SCALE,
//                                                               1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_engineInventoryListBox);
//            listboxToDrop.Add(m_engineInventoryListBox);
//            m_listboxDropConditions.Add(m_engineInventoryListBox,
//                                        ii => ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Engine);

//            // Back universal launcher inventory listbox
//            m_backUniversalLauncherInventoryListBox = new MyGuiControlListbox(this,
//                                                                              new Vector2(
//                                                                                  0.025f +
//                                                                                  MyGuiConstants.LISTBOX_SMALL_SIZE.X /
//                                                                                  2.0f, 0.3f),
//                                                                              MyGuiControlPreDefinedSize.SMALL,
//                                                                              MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                              MyTextsWrapper.Get(
//                                                                                  MyTextsWrapperEnum.
//                                                                                      WeaponUniversalLauncherBack),
//                                                                              MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                              1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_backUniversalLauncherInventoryListBox);
//            listboxToDrop.Add(m_backUniversalLauncherInventoryListBox);
//            m_listboxDropConditions.Add(m_backUniversalLauncherInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        ii.ObjectBuilderId.Value == 9);

//            // Radar inventory listbox
//            m_radarInventoryListBox = new MyGuiControlListbox(this, new Vector2(0.0f, 0.2f),
//                                                              MyGuiControlPreDefinedSize.SMALL,
//                                                              MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.Radar),
//                                                              MyGuiConstants.LABEL_TEXT_SCALE,
//                                                              1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_radarInventoryListBox);
//            listboxToDrop.Add(m_radarInventoryListBox);
//            m_listboxDropConditions.Add(m_radarInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Tool &&
//                                        ii.ObjectBuilderId == 23);

//            // Armor inventory listbox
//            m_armorInventoryListBox = new MyGuiControlListbox(this,
//                                                              new Vector2(
//                                                                  -0.025f - MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f,
//                                                                  0.1f), MyGuiControlPreDefinedSize.SMALL,
//                                                              MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.Armor),
//                                                              MyGuiConstants.LABEL_TEXT_SCALE,
//                                                              1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_armorInventoryListBox);
//            listboxToDrop.Add(m_armorInventoryListBox);
//            m_listboxDropConditions.Add(m_armorInventoryListBox,
//                                        ii => ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Armor);

//            // Front universal launcher inventory listbox
//            m_frontUniversalLauncherInventoryListBox = new MyGuiControlListbox(this,
//                                                                               new Vector2(
//                                                                                   0.025f +
//                                                                                   MyGuiConstants.LISTBOX_SMALL_SIZE.X /
//                                                                                   2.0f, 0.1f),
//                                                                               MyGuiControlPreDefinedSize.SMALL,
//                                                                               MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                               MyTextsWrapper.Get(
//                                                                                   MyTextsWrapperEnum.
//                                                                                       WeaponUniversalLauncherFront),
//                                                                               MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                               1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_frontUniversalLauncherInventoryListBox);
//            listboxToDrop.Add(m_frontUniversalLauncherInventoryListBox);
//            m_listboxDropConditions.Add(m_frontUniversalLauncherInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        ii.ObjectBuilderId.Value == 10);

//            // Drill inventory listbox
//            m_drillInventoryListBox = new MyGuiControlListbox(this,
//                                                              new Vector2(
//                                                                  -0.025f - MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f,
//                                                                  0.0f), MyGuiControlPreDefinedSize.SMALL,
//                                                              MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.Drill),
//                                                              MyGuiConstants.LABEL_TEXT_SCALE,
//                                                              1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_drillInventoryListBox);
//            listboxToDrop.Add(m_drillInventoryListBox);
//            m_listboxDropConditions.Add(m_drillInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        new int[] { 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value));

//            // Harvesting tool inventory listbox
//            m_harvestingToolInventoryListBox = new MyGuiControlListbox(this,
//                                                                       new Vector2(
//                                                                           0.025f +
//                                                                           MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f,
//                                                                           0.0f), MyGuiControlPreDefinedSize.SMALL,
//                                                                       MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                       MyTextsWrapper.Get(
//                                                                           MyTextsWrapperEnum.WeaponHarvestingDevice),
//                                                                       MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                       1, 1, 1, true, false, false);
//            InitializeListboxDragAndAddToControls(m_harvestingToolInventoryListBox);
//            listboxToDrop.Add(m_harvestingToolInventoryListBox);
//            m_listboxDropConditions.Add(m_harvestingToolInventoryListBox,
//                                        ii =>
//                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
//                                        ii.ObjectBuilderId.Value == 11);

//            #endregion

//            // money information
//            m_playersMoneyLabel = new MyGuiControlLabel(this, new Vector2(0.2f, 0.3f), null,
//                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.Cash),
//                                                        MyGuiConstants.LABEL_TEXT_COLOR,
//                                                        MyGuiConstants.LABEL_TEXT_SCALE,
//                                                        MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
//            m_playersMoneyLabel.UpdateParams(MySession.Static.Player.Money);
//            Controls.Add(m_playersMoneyLabel);
//            if (m_inventoryScreenType == MyInventoryScreenType.TradeForMoney)
//            {
//                m_moneyTradeBalanceLabel = new MyGuiControlLabel(this, new Vector2(-0.3f, 0.3f), null,
//                                                                 MyTextsWrapper.Get(MyTextsWrapperEnum.TradeBalance),
//                                                                 MyGuiConstants.LABEL_TEXT_COLOR,
//                                                                 MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                 MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
//                m_moneyTradeBalanceLabel.UpdateParams(m_moneyTradeBalance);
//                Controls.Add(m_moneyTradeBalanceLabel);
//            }

//            //  Buttons OK and BACK
//            Vector2 buttonDelta = new Vector2(0.05f,
//                                              m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y -
//                                              MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
//            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y),
//                                                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
//                                                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                                                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR,
//                                                MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick,
//                                                MyGuiControlButtonTextAlignment.CENTERED, true,
//                                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y),
//                                                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
//                                                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                                                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR,
//                                                MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick,
//                                                MyGuiControlButtonTextAlignment.CENTERED, true,
//                                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

//            #region other side's inventory

//            if (m_otherSideInventory != null)
//            {
//                // Other side's inventory label
//                Controls.Add(new MyGuiControlLabel(this,
//                                                   topLeft + 0 * MyGuiConstants.CONTROLS_DELTA +
//                                                   new Vector2(MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f, 0), null,
//                                                   otherSideInventoryName, MyGuiConstants.LABEL_TEXT_COLOR,
//                                                   MyGuiConstants.LABEL_TEXT_SCALE,
//                                                   MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

//                // Other side inventory listbox
//                m_otherSideInventoryListBox = new MyGuiControlListbox(this,
//                                                                      topLeft + 1 * MyGuiConstants.CONTROLS_DELTA +
//                                                                      new Vector2(
//                                                                          MyGuiConstants.LISTBOX_SMALL_SIZE.X / 2.0f, 0),
//                                                                      MyGuiControlPreDefinedSize.SMALL,
//                                                                      MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
//                                                                      otherSideInventoryName,
//                                                                      MyGuiConstants.LABEL_TEXT_SCALE,
//                                                                      INVENTORY_COLUMNS, INVENTORY_DISPLAY_ROWS,
//                                                                      INVENTORY_COLUMNS, true, true, false);
//                InitializeListboxDragAndAddToControls(m_otherSideInventoryListBox);
//                listboxToDrop.Add(m_otherSideInventoryListBox);
//                m_listboxDropConditions.Add(m_otherSideInventoryListBox, ii => true);
//            }

//            #endregion

//            // initialize drag and drop
//            m_dragAndDrop = new MyGuiControlListboxDragAndDrop(this, listboxToDrop, MyGuiControlPreDefinedSize.SMALL,
//                                                               MyGuiConstants.DRAG_AND_DROP_BACKGROUND_COLOR,
//                                                               MyGuiConstants.DRAG_AND_DROP_TEXT_COLOR,
//                                                               MyGuiConstants.LABEL_TEXT_SCALE,
//                                                               MyGuiConstants.DRAG_AND_DROP_TEXT_OFFSET, true);
//            m_dragAndDrop.ListboxItemDropped += OnDrop;
//            Controls.Add(m_dragAndDrop);

//            OnEnterCallback += OnOkClick;
//        }

//        private void FillListBoxWithInventoryContent(MyGuiControlListbox listbox, List<MyInventoryItem> inventoryItems)
//        {
//            foreach (MyInventoryItem item in inventoryItems)
//            {
//                AddInventoryItemToListBox(listbox, item);
//            }
//        }

//        private void AddInventoryItemToListBox(MyGuiControlListbox listbox, MyInventoryItem item)
//        {
//            int itemKey = m_itemsRepository.AddItem(item);
//            MyGuiControlListboxItem listboxItem = listbox.AddItem(itemKey, item.Description, item.Icon);
//            listboxItem.IconTexts = new MyIconTexts();

//            // add amount icon's text
//            if (item.Amount != 1f || item.Amount != item.MaxAmount)
//            {
//                StringBuilder amount = new StringBuilder();
//                amount.Append(item.Amount.ToString());
//                listboxItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM] = new MyColoredText(amount);
//            }
//            // add price icon's text
//            if (m_inventoryScreenType == MyInventoryScreenType.TradeForMoney)
//            {
//                StringBuilder price = new StringBuilder();
//                // if price of item not integer value, then we must show decimal places
//                if (item.Price - (int)item.Price > 0f)
//                {
//                    price.Append(item.Price.ToString("C"));
//                }
//                else
//                {
//                    price.Append(item.Price.ToString("C0"));
//                }
//                listboxItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP] = new MyColoredText(price);
//            }
//        }

//        private List<MyInventoryItem> GetInventoryItemsFromListbox(MyGuiControlListbox listbox)
//        {
//            List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
//            foreach (int itemKey in listbox.GetItemsKeys())
//            {
//                inventoryItems.Add(m_itemsRepository.GetItem(itemKey));
//            }
//            return inventoryItems;
//        }

//        private void InitializeListboxDragAndAddToControls(MyGuiControlListbox listbox)
//        {
//            Controls.Add(listbox);
//            listbox.ItemDrag += OnDrag;
//            listbox.ItemSelect += OnItemClick;
//            listbox.ItemDoubleClick += OnItemDoubleClick;
//        }

//        private void CreateInventoryItemAddToRepositoryAddToListbox(MyMwcObjectBuilder_Base objectBuilder, MyGuiControlListbox listbox)
//        {
//            MyInventoryItem item = MyInventory.CreateInventoryItemFromObjectBuilder(objectBuilder);
//            AddInventoryItemToListBox(listbox, item);
//        }

//        private void LoadInventoryContent()
//        {
//            // add player's ship's inventory to inventory screen
//            m_shipInventoryListBox.AddRows(m_shipInventory.MaxItems / INVENTORY_COLUMNS);
//            FillListBoxWithInventoryContent(m_shipInventoryListBox, m_shipInventory.GetInventoryItems());

//            m_leftWeaponsInventoryListBox.AddRow();
//            m_rightWeaponsInventoryListBox.AddRow();
//            m_drillInventoryListBox.AddRow();
//            m_harvestingToolInventoryListBox.AddRow();
//            m_frontUniversalLauncherInventoryListBox.AddRow();
//            m_backUniversalLauncherInventoryListBox.AddRow();
//            m_radarInventoryListBox.AddRow();
//            m_armorInventoryListBox.AddRow();
//            m_engineInventoryListBox.AddRow();

//            MyMwcObjectBuilder_SmallShip smallShipObjectBuilder =
//                (MyMwcObjectBuilder_SmallShip)m_playerShip.GetObjectBuilder(false);

//            // add weapons to inventory screen
//            foreach (MyMwcObjectBuilder_SmallShip_Weapon weapon in m_playerShip.Weapons.GetWeaponsObjectBuilders(true))
//            {
//                if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher ||
//                    weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser ||
//                    weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear ||
//                    weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure ||
//                    weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw ||
//                    weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal)
//                {
//                    CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_drillInventoryListBox);
//                }
//                else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device)
//                {
//                    CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_harvestingToolInventoryListBox);
//                }
//                else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front)
//                {
//                    CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_frontUniversalLauncherInventoryListBox);
//                }
//                else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back)
//                {
//                    CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_backUniversalLauncherInventoryListBox);
//                }
//                else
//                {
//                    if (m_leftWeaponsInventoryListBox.GetItemsCount() < INVENTORY_WEAPONS_COLUMNS)
//                    {
//                        CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_leftWeaponsInventoryListBox);
//                    }
//                    else if (m_rightWeaponsInventoryListBox.GetItemsCount() < INVENTORY_WEAPONS_COLUMNS)
//                    {
//                        CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_rightWeaponsInventoryListBox);
//                    }
//                    else
//                    {
//                        CreateInventoryItemAddToRepositoryAddToListbox(weapon, m_shipInventoryListBox);
//                    }
//                }
//            }

//            // add engine to inventory screen
//            if (MySession.PlayerShip.Engine != null)
//            {
//                CreateInventoryItemAddToRepositoryAddToListbox(m_playerShip.Engine, m_engineInventoryListBox);
//            }

//            // add armor to inventory screen
//            if (MySession.PlayerShip.Armor != null)
//            {
//                AddInventoryItemToListBox(m_armorInventoryListBox,
//                                          MyInventory.CreateInventoryItemFromObjectBuilder(m_playerShip.Armor));
//            }

//            // add radar to inventory screen
//            if (MySession.PlayerShip.Radar != null)
//            {
//                AddInventoryItemToListBox(m_radarInventoryListBox,
//                                          MyInventory.CreateInventoryItemFromObjectBuilder(m_playerShip.Radar));
//            }

//            // add other's side inventory to inventory screen
//            if (m_otherSideInventory != null)
//            {
//                m_otherSideInventoryListBox.AddRows(m_otherSideInventory.MaxItems / INVENTORY_COLUMNS);
//                FillListBoxWithInventoryContent(m_otherSideInventoryListBox, m_otherSideInventory.GetInventoryItems());
//            }
//        }

//        private void OnCancelClick()
//        {
//            CloseScreen();
//        }

//        private void FillInventoryContentFromListbox(MyInventory inventory, MyGuiControlListbox listBox)
//        {
//            inventory.ClearInventoryItems();
//            List<MyInventoryItem> inventoryItemsToAdd = new List<MyInventoryItem>();
//            foreach (MyInventoryItem item in GetInventoryItemsFromListbox(listBox))
//            {
//                inventoryItemsToAdd.Add(item);
//            }
//            inventory.AddInventoryItems(inventoryItemsToAdd);
//        }

//        private void SaveChangesFromInventoryScreenToPlayer()
//        {
//            // save inventory            
//            FillInventoryContentFromListbox(m_shipInventory, m_shipInventoryListBox);

//            // save engine
//            List<int> engineKeys = m_engineInventoryListBox.GetItemsKeys();
//            if (engineKeys.Count == 1)
//            {
//                m_playerShip.Engine = (MyMwcObjectBuilder_SmallShip_Engine)m_itemsRepository.GetItem(engineKeys[0]).GetObjectBuilder(true);
//            }
//            else
//            {
//                m_playerShip.Engine = null;
//            }

//            // save weapons
//            m_playerShip.Weapons.RemoveAllWeapons();
//            List<int> weaponsKeys = new List<int>();
//            weaponsKeys.AddRange(m_leftWeaponsInventoryListBox.GetItemsKeys());
//            weaponsKeys.AddRange(m_rightWeaponsInventoryListBox.GetItemsKeys());
//            weaponsKeys.AddRange(m_drillInventoryListBox.GetItemsKeys());
//            weaponsKeys.AddRange(m_harvestingToolInventoryListBox.GetItemsKeys());
//            weaponsKeys.AddRange(m_frontUniversalLauncherInventoryListBox.GetItemsKeys());
//            weaponsKeys.AddRange(m_backUniversalLauncherInventoryListBox.GetItemsKeys());
//            foreach (int weaponKey in weaponsKeys)
//            {
//                m_playerShip.Weapons.AddWeapon((MyMwcObjectBuilder_SmallShip_Weapon)m_itemsRepository.GetItem(weaponKey).GetObjectBuilder(true));
//            }

//            // save armor
//            List<int> armorKeys = m_armorInventoryListBox.GetItemsKeys();
//            if (armorKeys.Count == 1)
//            {
//                m_playerShip.Armor = m_itemsRepository.GetItem(armorKeys[0]).GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Armor;
//            }
//            else
//            {
//                m_playerShip.Armor = null;
//            }

//            // save radar
//            List<int> radarKeys = m_radarInventoryListBox.GetItemsKeys();
//            if (radarKeys.Count == 1)
//            {
//                m_playerShip.Radar = m_itemsRepository.GetItem(radarKeys[0]).GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Tool;
//            }
//            else
//            {
//                m_playerShip.Radar = null;
//            }

//            // save money
//            m_player.Money += m_moneyTradeBalance;
//        }

//        // save changes
//        private void OnOkClick()
//        {
//            if (m_player.Money + m_moneyTradeBalance >= 0f)
//            {
//                SaveChangesFromInventoryScreenToPlayer();

//                if (m_otherSideInventory != null && m_inventoryScreenType != MyInventoryScreenType.CustomizationEditor)
//                {
//                    FillInventoryContentFromListbox(m_otherSideInventory, m_otherSideInventoryListBox);
//                }
//                foreach (MyInventoryItem removedInventoryItem in m_removedInventoryItems)
//                {
//                    MyInventory.CloseInventoryItem(removedInventoryItem);
//                }
//                CloseScreen();
//            }
//            else
//            {
//                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyTextsWrapperEnum.NotificationYouDontHaveEnoughMoney,
//                                                                 MyTextsWrapperEnum.TradeResultTitle,
//                                                                 MyTextsWrapperEnum.Ok, null));
//            }
//        }

//        private void StartDragging(MyDropHandleType dropHandlingType, MyGuiControlListbox listbox, int rowIndex, int itemIndex)
//        {
//            MyDragAndDropInfo dragAndDropInfo = new MyDragAndDropInfo();
//            dragAndDropInfo.Listbox = listbox;
//            dragAndDropInfo.RowIndex = rowIndex;
//            dragAndDropInfo.ItemIndex = itemIndex;
//            MyGuiControlListboxItem draggingItem = dragAndDropInfo.Listbox.GetItem(dragAndDropInfo.RowIndex,
//                                                                                   dragAndDropInfo.ItemIndex);
//            dragAndDropInfo.Listbox.RemoveItem(dragAndDropInfo.RowIndex, dragAndDropInfo.ItemIndex);

//            m_dragAndDrop.StartDragging(dropHandlingType, draggingItem, dragAndDropInfo);
//        }

//        private void OnItemClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
//        {
//            //StartDraging(MyDragAndDropType.LeftMouseClick, sender, eventArgs);
//        }

//        private void OnItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
//        {
//            StartDragging(MyDropHandleType.LeftMouseClick, (MyGuiControlListbox)sender, eventArgs.RowIndex,
//                               eventArgs.ItemIndex);
//        }

//        private void OnDrag(object sender, MyGuiControlListboxItemEventArgs eventArgs)
//        {
//            StartDragging(MyDropHandleType.LeftMousePressed, (MyGuiControlListbox)sender, eventArgs.RowIndex,
//                               eventArgs.ItemIndex);
//        }

//        private void OnDrop(object sender, MyDragAndDropEventArgs eventArgs)
//        {
//            if (eventArgs.DropTo != null)
//            {
//                MyInventoryItem inventoryItem = m_itemsRepository.GetItem(eventArgs.ListboxItem.Key);

//                // test drop condition
//                Predicate<MyInventoryItem> dropCondition = m_listboxDropConditions[eventArgs.DropTo.Listbox];
//                // drop condition false (check if we able to drop this item to this listbox)
//                if (!dropCondition(inventoryItem))
//                {
//                    DropItem(eventArgs.DragFrom.Listbox, eventArgs.ListboxItem, eventArgs.DragFrom.RowIndex,
//                             eventArgs.DragFrom.ItemIndex);
//                    m_dragAndDrop.Stop();
//                }
//                // drop condition true
//                else
//                {
//                    if (m_inventoryScreenType == MyInventoryScreenType.TradeForMoney)
//                    {
//                        // item move from other side to player's ship
//                        if (eventArgs.DragFrom.Listbox == m_otherSideInventoryListBox &&
//                            eventArgs.DropTo.Listbox != m_otherSideInventoryListBox)
//                        {
//                            m_moneyTradeBalance -= inventoryItem.Price;
//                        }
//                        // item move from player's ship to other side
//                        else if (eventArgs.DragFrom.Listbox != m_otherSideInventoryListBox &&
//                                 eventArgs.DropTo.Listbox == m_otherSideInventoryListBox)
//                        {
//                            m_moneyTradeBalance += inventoryItem.Price;
//                        }
//                        m_moneyTradeBalanceLabel.UpdateParams(m_moneyTradeBalance);
//                    }

//                    MyGuiControlListboxItem itemAtDroppingPosition =
//                        eventArgs.DropTo.Listbox.GetItem(eventArgs.DropTo.RowIndex, eventArgs.DropTo.ItemIndex);
//                    // if there is any item at dropping position, then start draging this item
//                    if (itemAtDroppingPosition != null)
//                    {
//                        StartDragging(MyDropHandleType.LeftMouseClick, eventArgs.DropTo.Listbox,
//                                           eventArgs.DropTo.RowIndex, eventArgs.DropTo.ItemIndex);
//                    }
//                    else
//                    {
//                        m_dragAndDrop.Stop();
//                    }

//                    DropItem(eventArgs.DropTo.Listbox, eventArgs.ListboxItem, eventArgs.DropTo.RowIndex,
//                                  eventArgs.DropTo.ItemIndex);
//                }
//            }
//            else
//            {
//                // if item was dropped over inventory screen, then put them to original slot
//                if (IsMouseOver())
//                {
//                    DropItem(eventArgs.DragFrom.Listbox, eventArgs.ListboxItem, eventArgs.DragFrom.RowIndex,
//                             eventArgs.DragFrom.ItemIndex);
//                }
//                // if item was dropped out of inventory screen, then remove from game
//                else
//                {
//                    m_removedInventoryItems.Add(m_itemsRepository.GetItem(eventArgs.ListboxItem.Key));
//                }
//                m_dragAndDrop.Stop();
//            }
//        }

//        private void DropItem(MyGuiControlListbox listbox, MyGuiControlListboxItem item, int rowIndex, int itemIndex)
//        {
//            int key = item.Key;
//            StringBuilder value = item.Value;
//            MyTexture2D icon = item.Icon;
//            if (listbox.GetItem(rowIndex, itemIndex) != null)
//            {
//                listbox.AddItem(key, value, icon);
//            }
//            else
//            {
//                listbox.AddItem(key, value, icon, rowIndex, itemIndex);
//            }
//        }

//        public override void LoadContent()
//        {
//            base.LoadContent();
//        }

//        public override string GetFriendlyName()
//        {
//            return "MyGuiScreenInventory";
//        }

//        public override void UnloadContent()
//        {
//            base.UnloadContent();
//        }

//        public void CancelTransfer(MyDetectedEntity anotherSideEntity)
//        {
//            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyTextsWrapperEnum.TradeWasCanceled,
//                                                             MyTextsWrapperEnum.TradeResultTitle, MyTextsWrapperEnum.Ok,
//                                                             OnTransferCanceled));
//        }

//        public override bool Update(bool hasFocus)
//        {
//            if (base.Update(hasFocus) == false)
//            {
//                return false;
//            }

//            return true;
//        }

//        private void OnTransferCanceled(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
//        {
//            CloseScreen();
//        }

//        private bool IsMouseOver()
//        {
//            Vector2 topLeft = m_position - (m_size.Value / 2.0f);
//            Vector2 bottomRight = m_position + (m_size.Value / 2.0f);
//            bool isMouseOver = MyGuiManager.MouseCursorPosition.X >= topLeft.X &&
//                               MyGuiManager.MouseCursorPosition.Y >= topLeft.Y &&
//                               MyGuiManager.MouseCursorPosition.X <= bottomRight.X &&
//                               MyGuiManager.MouseCursorPosition.Y <= bottomRight.Y;

//            return isMouseOver;
//        }

//        private bool IsDraggingOut()
//        {
//            return m_dragAndDrop.IsActive() && !IsMouseOver();
//        }

//        public override bool Draw(float backgroundFadeAlpha)
//        {
//            if (!base.Draw(backgroundFadeAlpha)) return false;

//            if (IsDraggingOut())
//            {
//                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetLockedButtonTexture(), MyGuiManager.MouseCursorPosition,
//                                             m_dragAndDrop.GetSize().Value, new Color(new Vector4(1f, 0f, 0f, 0.5f)),
//                                             MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
//            }

//            return true;
//        }
//    }
//}

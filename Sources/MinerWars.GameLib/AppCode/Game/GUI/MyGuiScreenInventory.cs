using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Models;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.GUI
{
    enum MyGuiScreenInventoryType
    {
        Game,                   // in game, when any enitity to trade or loot, then show other side inventory
        InGameEditor,           // trading for free with foundation factory                
        GodEditor,              // in god editor, in other side inventory show all items in game
    }

    class MyGuiScreenInventorySaveResult
    {
        public List<MySmallShipBuilderWithName> SmallShipsObjectBuilders { get; set; }
        public int CurrentIndex { get; set; }
        public MyMwcObjectBuilder_Inventory OtherSideInventoryObjectBuilder { get; set; }
        public float Money { get; set; }
        public bool WasAnythingTransfered { get; set; }

        public MyGuiScreenInventorySaveResult(List<MySmallShipBuilderWithName> shipsBuilders, int currentIndex, MyMwcObjectBuilder_Inventory otherSideInventoryBuilder, float money, bool wasAnythingTransfered)
        {
            SmallShipsObjectBuilders = shipsBuilders;
            CurrentIndex = currentIndex;
            OtherSideInventoryObjectBuilder = otherSideInventoryBuilder;
            Money = money;
            WasAnythingTransfered = wasAnythingTransfered;
        }
    }

    class MySmallShipBuilderWithName
    {
        public StringBuilder Name { get; set; }
        public MyMwcObjectBuilder_SmallShip Builder { get; set; }
        public object UserData { get; set; }

        public MySmallShipBuilderWithName(StringBuilder name, MyMwcObjectBuilder_SmallShip builder)
            : this(name, builder, null)
        {
        }

        public MySmallShipBuilderWithName(StringBuilder name, MyMwcObjectBuilder_SmallShip builder, object userData)
        {
            Name = name;
            Builder = builder;
            UserData = userData;
        }

        public MySmallShipBuilderWithName(MyMwcObjectBuilder_SmallShip builder)
            : this(builder, null)
        {
        }

        public MySmallShipBuilderWithName(MyMwcObjectBuilder_SmallShip builder, object userData)
            : this(((MyGuiSmallShipHelperSmallShip)MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)builder.ShipType)).Name, builder, userData)
        {
        }
    }

    delegate void OnGuiScreenInventorySave(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveResult);

    class MyGuiScreenInventory : MyGuiScreenBase
    {
        private class MySmallShipInventoryItemIDs
        {
            public List<int> Inventory { get; set; }
            public List<int> LeftWeapons { get; set; }
            public List<int> RightWeapons { get; set; }
            public int? Radar { get; set; }
            public int? Engine { get; set; }
            public int? Armor { get; set; }
            public int? Harvester { get; set; }
            public int? Drill { get; set; }
            public int? FrontLauncher { get; set; }
            public int? BackLauncher { get; set; }

            public MySmallShipInventoryItemIDs()
            {
                Inventory = new List<int>();
                LeftWeapons = new List<int>();
                RightWeapons = new List<int>();
            }
        }

        private MyGuiControlListbox m_shipInventoryListBox;
        private MyGuiControlListbox m_otherSideInventoryListBox;        
        private List<MyGuiControlListbox> m_leftWeaponListboxes;        
        private List<MyGuiControlListbox> m_rightWeaponListboxes;
        //private MyGuiControlListbox m_radarInventoryListBox;
        private MyGuiControlListbox m_armorInventoryListBox;
        private MyGuiControlListbox m_engineInventoryListBox;
        private MyGuiControlListbox m_frontUniversalLauncherInventoryListBox;
        private MyGuiControlListbox m_backUniversalLauncherInventoryListBox;
        private MyGuiControlListbox m_drillInventoryListBox;
        private MyGuiControlListbox m_harvestingToolInventoryListBox;
        private MyGuiControlListboxDragAndDrop m_dragAndDrop;        
        private MyGuiControlLabel m_playersMoneyLabel;
        private MyGuiControlCombobox m_shipsCombobox;        
        private MyGuiControlButton m_okButton;
        private MyGuiControlButton m_cancelButton;
        private MyGuiControlButton m_ButtonTakeAll;
        private MyGuiControlButton m_removeAll;
        private MyGuiControlRotatingWheel m_transferingProgress;
        private MyGuiControlButton m_nextShipButton;
        private MyGuiControlButton m_previousShipButton;
        private MyGuiControlLabel m_shipInventoryCapacityLabel;
        private MyGuiControlLabel m_otherSideInventoryCapacityLabel;
        private MyGuiControlLabel m_shipNameLabel;


        //filter buttons
        //all items
        private MyGuiControlCheckbox m_sortAll;
        private MyGuiControlCheckbox m_sortConsumables;
        private MyGuiControlCheckbox m_sortEquipment;
        private MyGuiControlCheckbox m_sortGods;
        private MyGuiControlCheckbox m_sortOres;
        private MyGuiControlCheckbox m_sortWeapons;

        //my inventory
        private MyGuiControlCheckbox m_sortAllMy;
        private MyGuiControlCheckbox m_sortConsumablesMy;
        private MyGuiControlCheckbox m_sortEquipmentMy;
        private MyGuiControlCheckbox m_sortGodsMy;
        private MyGuiControlCheckbox m_sortOresMy;
        private MyGuiControlCheckbox m_sortWeaponsMy;

        private MyGuiControlPanel m_shipPanel;

        private MySoundCue? m_transferingProgressCue;
        private bool m_isTransferingInProgress;
        private int m_timeFromStartTransfering;
        private bool m_wasAnythingTrasfered;
        private bool m_resetWasAnythingTrasferedAfterComboboxChanged;
        private Dictionary<MyGuiControlListbox, Predicate<MyInventoryItem>> m_listboxDropConditions;
        private MyInventoryItemsRepository m_itemsRepository;

        private List<MySmallShipBuilderWithName> m_smallShipsBuilders;
        private int m_currentShipBuilderIndex;
        private MyMwcObjectBuilder_Inventory m_otherSideInventoryBuilder;
        private List<int> m_removedInventoryItemIDs;
        private float m_money;        
        private bool m_tradeForMoney;
        private MyGuiScreenInventoryType m_inventoryScreenType;
        private bool m_isInventoryLocked;

        private List<MySmallShipInventoryItemIDs> m_smallShipsInventoryItemIDs;
        private List<int> m_otherSideInventoryItemIDs;
        private List<int> m_shipInventoryHideList;
        private List<int> m_otherSideInventoryHideList;

        private const int INVENTORY_COLUMNS = 3;
        private const int INVENTORY_DISPLAY_ROWS = 4;
        private const int INVENTORY_WEAPONS_COLUMNS = 5;
        private const int INVENTORY_EMPTY_ROWS = 10;
        private const int COMPLETE_TRANSFER_TIME = 5000;    // in miliseconds

        private MyTexture2D m_inventoryItemTexture;
        private MyTexture2D m_inventoryScrollBarTexture;        

        List<ItemCategory> m_tempFilteredCategoriesMyInventory = new List<ItemCategory>(Enum.GetValues(typeof(ItemCategory)).Length);
        List<ItemCategory> m_tempFilteredCategoriesTheirsInventory = new List<ItemCategory>(Enum.GetValues(typeof(ItemCategory)).Length);

        public TimeSpan ScreenTimeout { get; set; }

        private MyTexture2D m_shipTexture;
        /// <summary>
        /// This constructor use only for bot's spawnpoints
        /// </summary>
        /// <param name="smallShipBuilder">Small ship's builder</param>
        /// <param name="otherSidesInventoryBuilder">Other side's inventory builder</param>
        /// <param name="otherSideInventoryName">Other side's inventory name</param>y
        public MyGuiScreenInventory(List<MySmallShipBuilderWithName> smallShipBuilders, int currentShipBuilderIndex, MyMwcObjectBuilder_Inventory otherSidesInventoryBuilder, StringBuilder otherSideInventoryName, bool resetWasAnythingTrasferedAfterComboboxChanged = false)
            : this(smallShipBuilders, currentShipBuilderIndex, 0f, false, otherSidesInventoryBuilder, otherSideInventoryName, MyGuiScreenInventoryType.GodEditor, resetWasAnythingTrasferedAfterComboboxChanged)
        {

        }

        public MyGuiScreenInventory(List<MySmallShipBuilderWithName> smallShipsBuilders, int currentShipBuilderIndex, float money, bool tradeForMoney,
            MyMwcObjectBuilder_Inventory otherSidesInventoryBuilder, StringBuilder otherSideInventoryName, MyGuiScreenInventoryType inventoryScreenType, bool resetWasAnythingTrasferedAfterComboboxChanged = false)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(1.0f, 1.0f), true, MyGuiManager.GetInventoryScreenBackgroundTexture())
        {
            ScreenTimeout = TimeSpan.MaxValue;
            m_isInventoryLocked = true;
            m_resetWasAnythingTrasferedAfterComboboxChanged = resetWasAnythingTrasferedAfterComboboxChanged;
            m_isTransferingInProgress = false;
            m_wasAnythingTrasfered = false;

            m_smallShipsBuilders = smallShipsBuilders;
            m_smallShipsInventoryItemIDs = new List<MySmallShipInventoryItemIDs>(smallShipsBuilders.Count);
            m_currentShipBuilderIndex = currentShipBuilderIndex;
            m_otherSideInventoryBuilder = otherSidesInventoryBuilder;
            if (m_otherSideInventoryBuilder != null)
            {
                m_otherSideInventoryItemIDs = new List<int>();
            }
            m_shipInventoryHideList = new List<int>();
            m_otherSideInventoryHideList = new List<int>();
            m_tradeForMoney = tradeForMoney;
            m_inventoryScreenType = inventoryScreenType;

            m_itemsRepository = new MyInventoryItemsRepository();
            m_removedInventoryItemIDs = new List<int>();

            LoadInventoryItemsFromObjectBuilders();

            if (m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor)
            {
                AddCaption(MyTextsWrapperEnum.ShipCustomizationCaption, new Vector2(0, 0.005f));
            }
            else
            {
                AddCaption(MyTextsWrapperEnum.ShipInventoryCaption, new Vector2(0, 0.005f));
            }
            m_enableBackgroundFade = false;
            m_backgroundFadeColor = Vector4.Zero;

            OnEnterCallback += OnOkClickDelegate;

            m_shipTexture = GetShipTexture();
            m_shipPanel = new MyGuiControlPanel(this, new Vector2(0.005f, -0.2347f), new Vector2(480 / 1600f, 367 / 1200f), MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                    m_shipTexture, null, null, null,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            Controls.Add(m_shipPanel);

            InitControls(otherSideInventoryName);

            FillOtherSideInventoryListbox();
            FillShipCustomizationListboxes(m_currentShipBuilderIndex);
            FillShipsCombobox();
            SwitchToShip(m_currentShipBuilderIndex);

            SetMoney(money);
            if (m_tradeForMoney) 
            {
                UpdateOtherSideInventoryListboxForTrading();
            }
            MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlOpen);
        }

        public event OnGuiScreenInventorySave OnSave;

        private void InitControls(StringBuilder otherSideInventoryName)
        {
            Vector2 topLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.1f);
            Vector2 topRight = new Vector2(m_size.Value.X / 2.0f - 0.05f, -m_size.Value.Y / 2.0f + 0.1f);

            List<MyGuiControlListbox> listboxToDrop = new List<MyGuiControlListbox>();
            m_listboxDropConditions = new Dictionary<MyGuiControlListbox, Predicate<MyInventoryItem>>();

            m_inventoryItemTexture = null;
            m_inventoryScrollBarTexture = MyGuiManager.GetInventoryScreenListboxScrollBarTexture();

            #region my ship's inventory and customization

            // Ship's inventory label
            Controls.Add(new MyGuiControlLabel(this,
                                               new Vector2(0.1959f, -0.357f), null,
                                               MyTextsWrapperEnum.ShipInventory, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                                               MyGuiManager.GetFontMinerWarsBlue()));

            // Ship's inventory capacity label
            m_shipInventoryCapacityLabel = new MyGuiControlLabel(this,
                                                                 new Vector2(0.4f, -0.357f), null, MyTextsWrapperEnum.InventoryCapacityLabel,
                                                                 MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                 MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER);
            Controls.Add(m_shipInventoryCapacityLabel);

            // Ship inventory listbox
            m_shipInventoryListBox = new MyGuiControlListbox(this,
                                                             new Vector2(0.3211f, -0.1425f),
                                                             MyGuiConstants.LISTBOX_SMALL_SIZE + new Vector2(0,0.003f),
                                                             new Vector4(0f, 0f, 0f, 0f),
                                                             null,
                                                             MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                             INVENTORY_COLUMNS, INVENTORY_DISPLAY_ROWS,
                                                             INVENTORY_COLUMNS, true, true, false,
                                                             null,
                                                             null,
                                                             m_inventoryScrollBarTexture,
                                                             null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0.03f, 0, 0, 0, -0.005f, -0.01f, -0.016f, -0.017f);
            InitializeListboxDragAndAddToControls(m_shipInventoryListBox);
            listboxToDrop.Add(m_shipInventoryListBox);
            m_listboxDropConditions.Add(m_shipInventoryListBox, ii => true);

            // Left weapons inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(-0.2637f, 0.2903f),
                                               null, MyTextsWrapperEnum.LeftWeapons, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            
            m_leftWeaponListboxes = new List<MyGuiControlListbox>();
            var leftWeapon1InventoryListBox = new MyGuiControlListbox(this,
                                                        new Vector2(-0.3075f, 0.2203f),
                                                        MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                        new Vector4(1f, 1f, 1f, 0f),
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                        1, 1,
                                                        1, true, false, false,
                                                        null, null,
                                                        null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0.02f, 0f, 0f, 0f, 0, 0, 0);


            leftWeapon1InventoryListBox.DisplayHighlight = false;
            m_leftWeaponListboxes.Add(leftWeapon1InventoryListBox);
            leftWeapon1InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(leftWeapon1InventoryListBox);
            listboxToDrop.Add(leftWeapon1InventoryListBox);
            m_listboxDropConditions.Add(leftWeapon1InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
                                        false);

            var leftWeapon2InventoryListBox = new MyGuiControlListbox(this,
                                                        new Vector2(-0.2363f, 0.2303f),
                                                        MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                        new Vector4(1f, 1f, 1f, 0f),
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                        1, 1,
                                                        1, true, false, false,
                                                        null, null,
                                                        null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            leftWeapon2InventoryListBox.DisplayHighlight = false;
            m_leftWeaponListboxes.Add(leftWeapon2InventoryListBox);
            leftWeapon2InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(leftWeapon2InventoryListBox);
            listboxToDrop.Add(leftWeapon2InventoryListBox);
            m_listboxDropConditions.Add(leftWeapon2InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
                                        false);

            var leftWeapon3InventoryListBox = new MyGuiControlListbox(this,
                                                        new Vector2(-0.1668f, 0.2303f),
                                                        MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                        new Vector4(1f, 1f, 1f, 0f),
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                        1, 1,
                                                        1, true, false, false,
                                                        null, null,
                                                        null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            leftWeapon3InventoryListBox.DisplayHighlight = false;
            m_leftWeaponListboxes.Add(leftWeapon3InventoryListBox);
            leftWeapon3InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(leftWeapon3InventoryListBox);
            listboxToDrop.Add(leftWeapon3InventoryListBox);
            m_listboxDropConditions.Add(leftWeapon3InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
                                        false);

            var leftWeapon4InventoryListBox = new MyGuiControlListbox(this,
                                                        new Vector2(-0.1659f, 0.1374f),
                                                        MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                        new Vector4(1f, 1f, 1f, 0f),
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                        1, 1,
                                                        1, true, false, false,
                                                        null, null,
                                                        null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            leftWeapon4InventoryListBox.DisplayHighlight = false;
            m_leftWeaponListboxes.Add(leftWeapon4InventoryListBox);
            leftWeapon4InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(leftWeapon4InventoryListBox);
            listboxToDrop.Add(leftWeapon4InventoryListBox);
            m_listboxDropConditions.Add(leftWeapon4InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
                                        false);

            var leftWeapon5InventoryListBox = new MyGuiControlListbox(this,
                                                        new Vector2(-0.0946f, 0.1374f),
                                                        MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                        new Vector4(1f, 1f, 1f, 0f),
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.LeftWeapons),
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                        1, 1,
                                                        1, true, false, false,
                                                        null, null,
                                                        null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            leftWeapon5InventoryListBox.DisplayHighlight = false;
            m_leftWeaponListboxes.Add(leftWeapon5InventoryListBox);
            leftWeapon5InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(leftWeapon5InventoryListBox);
            listboxToDrop.Add(leftWeapon5InventoryListBox);
            m_listboxDropConditions.Add(leftWeapon5InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) ==
                                        false);

            // Right weapons inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.2021f, 0.2909f),
                                               null, MyTextsWrapperEnum.RighWeapons, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            
            m_rightWeaponListboxes = new List<MyGuiControlListbox>();
            var rightWeapon1InventoryListBox = new MyGuiControlListbox(this,
                                                                     new Vector2(0.0877f, 0.1362f),
                                                                     MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                     new Vector4(1f, 1f, 1f, 0f),
                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RighWeapons),
                                                                     MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     1, 1,
                                                                     1, true, false, false,
                                                                     null, m_inventoryItemTexture,
                                                                     null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            rightWeapon1InventoryListBox.DisplayHighlight = false;
            m_rightWeaponListboxes.Insert(0, rightWeapon1InventoryListBox);
            rightWeapon1InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(rightWeapon1InventoryListBox);
            listboxToDrop.Add(rightWeapon1InventoryListBox);
            m_listboxDropConditions.Add(rightWeapon1InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) == false);

            var rightWeapon2InventoryListBox = new MyGuiControlListbox(this,
                                                                     new Vector2(0.1599f, 0.1362f),
                                                                     MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                     new Vector4(1f, 1f, 1f, 0f),
                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RighWeapons),
                                                                     MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     1, 1,
                                                                     1, true, false, false,
                                                                     null, m_inventoryItemTexture,
                                                                     null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            rightWeapon2InventoryListBox.DisplayHighlight = false;
            m_rightWeaponListboxes.Insert(0, rightWeapon2InventoryListBox);
            rightWeapon2InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(rightWeapon2InventoryListBox);
            listboxToDrop.Add(rightWeapon2InventoryListBox);
            m_listboxDropConditions.Add(rightWeapon2InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) == false);

            var rightWeapon3InventoryListBox = new MyGuiControlListbox(this,
                                                                     new Vector2(0.1581f, 0.2303f),
                                                                     MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                     new Vector4(1f, 1f, 1f, 0f),
                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RighWeapons),
                                                                     MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     1, 1,
                                                                     1, true, false, false,
                                                                     null, m_inventoryItemTexture,
                                                                     null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            rightWeapon3InventoryListBox.DisplayHighlight = false;
            m_rightWeaponListboxes.Insert(0, rightWeapon3InventoryListBox);
            rightWeapon3InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(rightWeapon3InventoryListBox);
            listboxToDrop.Add(rightWeapon3InventoryListBox);
            m_listboxDropConditions.Add(rightWeapon3InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) == false);

            var rightWeapon4InventoryListBox = new MyGuiControlListbox(this,
                                                                     new Vector2(0.2293f, 0.2303f),
                                                                     MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                     new Vector4(1f, 1f, 1f, 0f),
                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RighWeapons),
                                                                     MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     1, 1,
                                                                     1, true, false, false,
                                                                     null, m_inventoryItemTexture,
                                                                     null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            rightWeapon4InventoryListBox.DisplayHighlight = false;
            m_rightWeaponListboxes.Insert(0, rightWeapon4InventoryListBox);
            rightWeapon4InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(rightWeapon4InventoryListBox);
            listboxToDrop.Add(rightWeapon4InventoryListBox);
            m_listboxDropConditions.Add(rightWeapon4InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) == false);

            var rightWeapon5InventoryListBox = new MyGuiControlListbox(this,
                                                                     new Vector2(0.2997f, 0.2303f),
                                                                     MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                     new Vector4(1f, 1f, 1f, 0f),
                                                                     MyTextsWrapper.Get(MyTextsWrapperEnum.RighWeapons),
                                                                     MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     1, 1,
                                                                     1, true, false, false,
                                                                     null, m_inventoryItemTexture,
                                                                     null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            rightWeapon5InventoryListBox.DisplayHighlight = false;
            m_rightWeaponListboxes.Insert(0, rightWeapon5InventoryListBox);
            rightWeapon5InventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(rightWeapon5InventoryListBox);
            listboxToDrop.Add(rightWeapon5InventoryListBox);
            m_listboxDropConditions.Add(rightWeapon5InventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value) == false);

            // Engine inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0f, 0.1846f),
                                               null, MyTextsWrapperEnum.Engine, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            // Engine inventory listbox
            m_engineInventoryListBox = new MyGuiControlListbox(this,
                                                               new Vector2(-0.0042f, 0.1256f),
                                                               MyGuiConstants.LISTBOX_SMALL_SIZE, new Vector4(1f, 1f, 1f, 0f),
                                                               MyTextsWrapper.Get(MyTextsWrapperEnum.Engine),
                                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                               1, 1, 1, true, false, false,
                                                               null, m_inventoryItemTexture,
                                                               null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_engineInventoryListBox.DisplayHighlight = false;
            m_engineInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_engineInventoryListBox);
            listboxToDrop.Add(m_engineInventoryListBox);
            m_listboxDropConditions.Add(m_engineInventoryListBox,
                                        ii => ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Engine);

            // Back universal launcher inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(-0.0626f, 0.3374f),
                                               null, MyTextsWrapperEnum.WeaponUniversalLauncherBackTitle, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            // Back universal launcher inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(-0.0626f, 0.3531f),
                                               null, MyTextsWrapperEnum.WeaponUniversalLauncher, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));


            // Back universal launcher inventory listbox
            m_backUniversalLauncherInventoryListBox = new MyGuiControlListbox(this,
                                                                              new Vector2(-0.0605f, 0.2755f),
                                                                              MyGuiConstants.LISTBOX_SMALL_SIZE,
                                                                              new Vector4(1f, 1f, 1f, 0f),
                                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.WeaponUniversalLauncherBack),
                                                                              MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                              1, 1, 1, true, false, false,
                                                                              null, m_inventoryItemTexture,
                                                                              null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_backUniversalLauncherInventoryListBox.DisplayHighlight = false;
            m_backUniversalLauncherInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_backUniversalLauncherInventoryListBox);
            listboxToDrop.Add(m_backUniversalLauncherInventoryListBox);
            m_listboxDropConditions.Add(m_backUniversalLauncherInventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        ii.ObjectBuilderId.Value == 9);

            // Armor inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0573f, 0.3374f),
                                               null, MyTextsWrapperEnum.Armor, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            // Armor inventory listbox
            m_armorInventoryListBox = new MyGuiControlListbox(this,
                                                              new Vector2(0.0555f, 0.2755f),
                                                              MyGuiConstants.LISTBOX_SMALL_SIZE, new Vector4(1f, 1f, 1f, 0f),
                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.Armor),
                                                              MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                              1, 1, 1, true, false, false,
                                                              null, m_inventoryItemTexture,
                                                              null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_armorInventoryListBox.DisplayHighlight = false;
            m_armorInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_armorInventoryListBox);
            listboxToDrop.Add(m_armorInventoryListBox);
            m_listboxDropConditions.Add(m_armorInventoryListBox,
                                        ii => ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Armor);

            // Front universal launcher inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0f, 0.0323f),
                                               null, MyTextsWrapperEnum.WeaponUniversalLauncherFrontTitle, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            // Front universal launcher inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0f, 0.0462f),
                                               null, MyTextsWrapperEnum.WeaponUniversalLauncher, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            // Front universal launcher inventory listbox
            m_frontUniversalLauncherInventoryListBox = new MyGuiControlListbox(this,
                                                                               new Vector2(-0.0039f, -0.0287f),
                                                                               MyGuiConstants.LISTBOX_SMALL_SIZE, new Vector4(1f, 1f, 1f, 0f),
                                                                               MyTextsWrapper.Get(MyTextsWrapperEnum.WeaponUniversalLauncherFront),
                                                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                               1, 1, 1, true, false, false,
                                                                               null, m_inventoryItemTexture,
                                                                               null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_frontUniversalLauncherInventoryListBox.DisplayHighlight = false;
            m_frontUniversalLauncherInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_frontUniversalLauncherInventoryListBox);
            listboxToDrop.Add(m_frontUniversalLauncherInventoryListBox);
            m_listboxDropConditions.Add(m_frontUniversalLauncherInventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        ii.ObjectBuilderId.Value == 10);

            // Drill inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(-0.0920f, 0.0547f),
                                               null, MyTextsWrapperEnum.Drill, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));

            Controls.Add(new MyGuiControlLabel(this, new Vector2(-0.0920f, 0.0715f),
                                   null, MyTextsWrapperEnum.device, MyGuiConstants.LABEL_TEXT_COLOR,
                                   MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            // Drill inventory listbox
            m_drillInventoryListBox = new MyGuiControlListbox(this,
                                                              new Vector2(-0.0944f, -0.0044f),
                                                              MyGuiConstants.LISTBOX_SMALL_SIZE, new Vector4(1f, 1f, 1f, 0f),
                                                              MyTextsWrapper.Get(MyTextsWrapperEnum.Drill),
                                                              MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                              1, 1, 1, true, false, false,
                                                              null, m_inventoryItemTexture,
                                                              null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_drillInventoryListBox.DisplayHighlight = false;
            m_drillInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_drillInventoryListBox);
            listboxToDrop.Add(m_drillInventoryListBox);
            m_listboxDropConditions.Add(m_drillInventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        new int[] { 12, 13, 14, 15, 16, 18 }.Contains(ii.ObjectBuilderId.Value));

            // Harvesting tool inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0861f, 0.0559f),
                                               null, MyTextsWrapperEnum.Harvester, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            // Harvesting tool inventory label
            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0861f, 0.0703f),
                                               null, MyTextsWrapperEnum.device, MyGuiConstants.LABEL_TEXT_COLOR,
                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            // Harvesting tool inventory listbox
            m_harvestingToolInventoryListBox = new MyGuiControlListbox(this,
                                                                       new Vector2(0.0843f, -0.0044f),
                                                                       MyGuiConstants.LISTBOX_SMALL_SIZE, new Vector4(1f, 1f, 1f, 0f),
                                                                       MyTextsWrapper.Get(MyTextsWrapperEnum.WeaponHarvestingDevice),
                                                                       MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                       1, 1, 1, true, false, false,
                                                                       null, m_inventoryItemTexture,
                                                                       null, null, 0, 0, new Vector4(1f, 1f, 1f, 0f), 0f, 0f, 0f, 0f, 0, 0, 0);
            m_harvestingToolInventoryListBox.DisplayHighlight = false;
            m_harvestingToolInventoryListBox.AddRow();
            InitializeListboxDragAndAddToControls(m_harvestingToolInventoryListBox);
            listboxToDrop.Add(m_harvestingToolInventoryListBox);
            m_listboxDropConditions.Add(m_harvestingToolInventoryListBox,
                                        ii =>
                                        ii.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon &&
                                        ii.ObjectBuilderId.Value == 11);

            #endregion

            // money information
            m_playersMoneyLabel = new MyGuiControlLabel(this, new Vector2(0.355f, 0.1450f), null,
                                                        MyTextsWrapper.Get(MyTextsWrapperEnum.Cash),
                                                        MyGuiConstants.LABEL_TEXT_COLOR,
                                                        MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE * 1.2f,
                                                        MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER,
                                                        MyGuiManager.GetFontMinerWarsBlue());
            Controls.Add(m_playersMoneyLabel);            


            m_okButton = new MyGuiControlButton(this, new Vector2(0f, 0.4086f), MyGuiConstants.OK_BUTTON_SIZE,
                                   MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                                   MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Close,
                                   MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                                   true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_okButton);




            if (!m_tradeForMoney)
            {
                //Take all button
                m_ButtonTakeAll = new MyGuiControlButton(this, new Vector2(-0.3095f, 0.1455f), new Vector2(0.1532f, 0.063f),
                           MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                           MyGuiManager.GetInventoryScreenButtonTextureTakeAll(), null, null, MyTextsWrapperEnum.TakeAll,
                           MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER, MyGuiControlButtonTextAlignment.CENTERED, OnTakeAllClick,
                           true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(m_ButtonTakeAll);
            }

            if (m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor) 
            {
                m_removeAll = new MyGuiControlButton(this, new Vector2(0.29f, -0.4f), new Vector2(0.1532f, 0.063f),
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTextureTakeAll(), null, null, MyTextsWrapperEnum.RemoveAll,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE_SMALLER, MyGuiControlButtonTextAlignment.CENTERED, OnRemoveAllClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(m_removeAll);
            }

            #region other side's inventory

            if (m_otherSideInventoryBuilder != null)
            {
                // Other side's inventory label
                Controls.Add(new MyGuiControlLabel(this,
                                                   new Vector2(-0.4126f, -0.357f), null,
                                                   otherSideInventoryName, MyGuiConstants.LABEL_TEXT_COLOR,
                                                   MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                   MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                                                   MyGuiManager.GetFontMinerWarsBlue()));

                // Ship's inventory capacity label
                m_otherSideInventoryCapacityLabel = new MyGuiControlLabel(this,
                                                                     new Vector2(-0.215f, -0.357f), null, MyTextsWrapperEnum.InventoryCapacityLabel,
                                                                     MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                     MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER);
                Controls.Add(m_otherSideInventoryCapacityLabel);
                
                // Other side inventory listbox
                m_otherSideInventoryListBox = new MyGuiControlListbox(this,
                                                                      new Vector2(-0.2925f, -0.1425f),
                                                                      MyGuiConstants.LISTBOX_SMALL_SIZE + new Vector2(0, 0.003f),
                                                                      new Vector4(1f, 1f, 1f, 0f),
                                                                      null,
                                                                      MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                      INVENTORY_COLUMNS, INVENTORY_DISPLAY_ROWS,
                                                                      INVENTORY_COLUMNS, true, true, false,
                                                                      null,
                                                                      null,
                                                                      m_inventoryScrollBarTexture,
                                                                      null, 0, 0, new Vector4(1f, 1f, 1f, 0.0f), 0.03f, 0, 0, 0, -0.005f, -0.01f, -0.0155f, -0.017f);
                InitializeListboxDragAndAddToControls(m_otherSideInventoryListBox);
                listboxToDrop.Add(m_otherSideInventoryListBox);
                m_listboxDropConditions.Add(m_otherSideInventoryListBox, ii => true);                

                m_shipInventoryListBox.ItemDoubleClick += OnShipItemDoubleClick;
                m_otherSideInventoryListBox.ItemDoubleClick += OnOtherSideItemDoubleClick;
            }
            else
            {
                m_otherSideInventoryListBox = new MyGuiControlListbox(this,
                                                                      new Vector2(-0.2925f, -0.1425f),
                                                                      MyGuiConstants.LISTBOX_SMALL_SIZE + new Vector2(0, 0.003f),
                                                                      new Vector4(1f, 1f, 1f, 0f),
                                                                      otherSideInventoryName,
                                                                      MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                                      INVENTORY_COLUMNS, INVENTORY_DISPLAY_ROWS,
                                                                      INVENTORY_COLUMNS, true, true, false,
                                                                      null,
                                                                      null,
                                                                      m_inventoryScrollBarTexture,
                                                                      null, 0, 0, new Vector4(1f, 1f, 1f, 0.0f), 0.03f, 0, 0, 0, -0.005f, -0.01f, -0.0155f, -0.017f);
            }

            //Filters All Items

            const float delta = 0.028f;
            var position = new Vector2(-0.2402f, 0.0684f);
            m_sortAll = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortAllOff(),
                                             MyGuiManager.GetInventoryFilterSortAllOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowAll), true, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);            
            Controls.Add(m_sortAll);
            position.X -= delta;

            m_sortGods = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortGodsOff(),
                                             MyGuiManager.GetInventoryFilterSortGodsOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowGods), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);            
            Controls.Add(m_sortGods);
            position.X -= delta;

            m_sortOres = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortOresOff(),
                                             MyGuiManager.GetInventoryFilterSortOresOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowOres), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);            
            Controls.Add(m_sortOres);
            position.X -= delta;

            m_sortConsumables = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortConsumablesOff(),
                                             MyGuiManager.GetInventoryFilterSortConsumablesOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowConsumables), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            Controls.Add(m_sortConsumables);            
            position.X -= delta;

            m_sortEquipment = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortEquipmentOff(),
                                             MyGuiManager.GetInventoryFilterSortEquipmentOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowEquipment), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            Controls.Add(m_sortEquipment);            
            position.X -= delta;

            m_sortWeapons = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortWeaponsOff(),
                                             MyGuiManager.GetInventoryFilterSortWeaponsOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowWeapons), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);            
            Controls.Add(m_sortWeapons);

            if (m_otherSideInventoryBuilder != null)
            {
                m_sortAll.OnCheck += OnFilterInventorySortClick;
                m_sortGods.OnCheck += OnFilterInventorySortClick;
                m_sortOres.OnCheck += OnFilterInventorySortClick;
                m_sortConsumables.OnCheck += OnFilterInventorySortClick;
                m_sortEquipment.OnCheck += OnFilterInventorySortClick;
                m_sortWeapons.OnCheck += OnFilterInventorySortClick;
            }
            else 
            {
                m_sortAll.Checked = false;
                m_sortAll.Enabled = false;
                m_sortGods.Enabled = false;
                m_sortOres.Enabled = false;
                m_sortConsumables.Enabled = false;
                m_sortEquipment.Enabled = false;
                m_sortWeapons.Enabled = false;
            }

            //Filters My Inventory
            position = new Vector2(0.373f, 0.0684f);

            m_sortAllMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortAllOff(),
                                             MyGuiManager.GetInventoryFilterSortAllOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowAll), true, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            Controls.Add(m_sortAllMy);
            m_sortAllMy.OnCheck += OnFilterInventoryMySortClick;
            position.X -= delta;

            m_sortGodsMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortGodsOff(),
                                             MyGuiManager.GetInventoryFilterSortGodsOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowGods), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            m_sortGodsMy.OnCheck += OnFilterInventoryMySortClick;
            Controls.Add(m_sortGodsMy);
            position.X -= delta;

            m_sortOresMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortOresOff(),
                                             MyGuiManager.GetInventoryFilterSortOresOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowOres), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            Controls.Add(m_sortOresMy);
            m_sortOresMy.OnCheck += OnFilterInventoryMySortClick;
            position.X -= delta;

            m_sortConsumablesMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortConsumablesOff(),
                                             MyGuiManager.GetInventoryFilterSortConsumablesOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowConsumables), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            m_sortConsumablesMy.OnCheck += OnFilterInventoryMySortClick;
            Controls.Add(m_sortConsumablesMy);
            position.X -= delta;

            m_sortEquipmentMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortEquipmentOff(),
                                             MyGuiManager.GetInventoryFilterSortEquipmentOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowEquipment), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            m_sortEquipmentMy.OnCheck += OnFilterInventoryMySortClick;
            Controls.Add(m_sortEquipmentMy);
            position.X -= delta;

            m_sortWeaponsMy = new MyGuiControlCheckbox(this, position, MyGuiConstants.INVENTORY_FILTER_BUTTON_SIZE,
                                             MyGuiManager.GetInventoryFilterSortWeaponsOff(),
                                             MyGuiManager.GetInventoryFilterSortWeaponsOn(),
                                             MyTextsWrapper.Get(MyTextsWrapperEnum.InventoryShowWeapons), false, MyGuiConstants.BACK_BUTTON_TEXT_COLOR, true, null, MyGuiConstants.INVENTORY_FILTER_BUTTON_INNER_SIZE);
            m_sortWeaponsMy.OnCheck += OnFilterInventoryMySortClick;
            Controls.Add(m_sortWeaponsMy);



            #endregion

            #region combobox for select ship to customization
            if (m_smallShipsBuilders.Count > 1 && m_inventoryScreenType != MyGuiScreenInventoryType.Game)
            {
                m_shipsCombobox = new MyGuiControlCombobox(this, new Vector2(0f, topLeft.Y), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                        MyGuiConstants.COMBOBOX_TEXT_SCALE);
                m_shipsCombobox.OnSelect += OnComboboxSelectedItemChanged;
                Controls.Add(m_shipsCombobox);                
            }

            Vector2 shipButtonSize = new Vector2(128 / 1600f, 512 / 1200f) * 0.5f;
            m_previousShipButton = new MyGuiControlButton(this, new Vector2(-0.144f, -0.256f), shipButtonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                               MyGuiManager.GetInventoryPreviousShip(), null, null, MyTextsWrapperEnum.EmptyDescription,
                               Vector4.Zero, 0f, MyGuiControlButtonTextAlignment.CENTERED, OnPreviousShipButtonClick,
                               false, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_previousShipButton);

            m_nextShipButton = new MyGuiControlButton(this, new Vector2(0.144f, -0.256f), shipButtonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                               MyGuiManager.GetInventoryNextShip(), null, null, MyTextsWrapperEnum.EmptyDescription,
                               Vector4.Zero, 0f, MyGuiControlButtonTextAlignment.CENTERED, OnNextShipButtonClick,
                               false, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_nextShipButton);
            #endregion

            // ship's name
            m_shipNameLabel = new MyGuiControlLabel(this, 
                                                    new Vector2(0f, -0.1156f), null, new StringBuilder(), MyGuiConstants.LABEL_TEXT_COLOR, 
                                                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            Controls.Add(m_shipNameLabel);

            // initialize drag and drop
            m_dragAndDrop = new MyGuiControlListboxDragAndDrop(this, listboxToDrop, MyGuiControlPreDefinedSize.SMALL,
                                                               MyGuiConstants.DRAG_AND_DROP_BACKGROUND_COLOR,
                                                               MyGuiConstants.DRAG_AND_DROP_TEXT_COLOR,
                                                               MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE,
                                                               MyGuiConstants.DRAG_AND_DROP_TEXT_OFFSET, true);
            m_dragAndDrop.ListboxItemDropped += OnDrop;
            m_dragAndDrop.DrawBackgroundTexture = false;
            Controls.Add(m_dragAndDrop);
        }

        private void OnComboboxSelectedItemChanged()
        {            
            SwitchToShip(m_shipsCombobox.GetSelectedIndex());
        }

        private void OnPreviousShipButtonClick(MyGuiControlButton sender) 
        {
            int index = m_currentShipBuilderIndex - 1;
            if (index < 0) 
            {
                index = m_smallShipsBuilders.Count - 1;
            }
            SwitchToShip(index);
        }

        private void OnNextShipButtonClick(MyGuiControlButton sender)
        {
            int index = m_currentShipBuilderIndex + 1;
            if (index > m_smallShipsBuilders.Count - 1)
            {
                index = 0;
            }
            SwitchToShip(index);
        }

        private void SwitchToShip(int index) 
        {
            if (!m_isTransferingInProgress)
            {
                // save changes to current ship's ids from listboxes
                SaveIDsFromShipCustomizationListboxes(m_currentShipBuilderIndex);
                LoadShipInventory(index);
            }
        }

        private void LoadShipInventory(int index) 
        {
            m_currentShipBuilderIndex = index;

            if (m_shipsCombobox != null)
            {
                m_shipsCombobox.SelectItemByIndex(m_currentShipBuilderIndex);
            }

            // load listboxes for new selected ship's ids
            FillShipCustomizationListboxes(m_currentShipBuilderIndex);

            if (m_resetWasAnythingTrasferedAfterComboboxChanged)
            {
                m_wasAnythingTrasfered = false;
            }

            m_shipTexture = GetShipTexture();
            m_shipPanel.SetBackgroundTexture(m_shipTexture);
            m_shipNameLabel.UpdateText(m_smallShipsBuilders[m_currentShipBuilderIndex].Name.ToString());
        }

        private MyTexture2D GetShipTexture() 
        {
            MyGuiSmallShipHelperSmallShip smallShipHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)m_smallShipsBuilders[m_currentShipBuilderIndex].Builder.ShipType) as MyGuiSmallShipHelperSmallShip;
            return smallShipHelper.Preview;
        }

        protected override void Canceling()
        {
            //base.Canceling();

            OnOkClick(null);
        }

        private int CreateInventoryItemAddToRepository(MyMwcObjectBuilder_InventoryItem inventoryItem, MyMwcObjectBuilder_Inventory owner) 
        {
            MyInventoryItem item = MyInventory.CreateInventoryItemFromInventoryItemObjectBuilder(inventoryItem);
            item.Owner = owner;
            return m_itemsRepository.AddItem(item);
        }

        private int CreateInventoryItemAddToRepository(MyMwcObjectBuilder_Base builder, float amount, MyMwcObjectBuilder_Inventory owner)
        {
            MyInventoryItem item = MyInventory.CreateInventoryItemFromObjectBuilder(builder, amount);
            item.Owner = owner;
            return m_itemsRepository.AddItem(item);
        }

        private int CreateInventoryItemAddToRepository(MyMwcObjectBuilder_Base builder, MyMwcObjectBuilder_Inventory owner)
        {
            return CreateInventoryItemAddToRepository(builder, 1f, owner);
        }

        private MyMwcObjectBuilder_InventoryItem GetInventoryItemObjectBuilderFromRepository(int key)
        {
            MyInventoryItem inventoryItem = m_itemsRepository.GetItem(key);
            return inventoryItem.GetObjectBuilder();
        }

        private MyMwcObjectBuilder_Base GetObjectBuilderFromRepository(int key)
        {
            return m_itemsRepository.GetItem(key).GetInventoryItemObjectBuilder(false);
        }

        private MySmallShipInventoryItemIDs GetInventoryItemIDsFromObjectBuilder(MyMwcObjectBuilder_SmallShip smallShipBuilder) 
        {
            MySmallShipInventoryItemIDs shipInventoryItemsIDs = new MySmallShipInventoryItemIDs();

            // inventory items
            if (smallShipBuilder.Inventory != null && smallShipBuilder.Inventory.InventoryItems != null)
            {
                foreach (MyMwcObjectBuilder_InventoryItem inventoryItem in smallShipBuilder.Inventory.InventoryItems)
                {
                    shipInventoryItemsIDs.Inventory.Add(CreateInventoryItemAddToRepository(inventoryItem, smallShipBuilder.Inventory));
                }
            }

            Debug.Assert(smallShipBuilder.Inventory != null, "smallShipBuilder.Inventory != null");

            // weapons
            if (smallShipBuilder.Weapons != null)
            {
                foreach (MyMwcObjectBuilder_SmallShip_Weapon weapon in smallShipBuilder.Weapons)
                {
                    if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher ||
                        weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser ||
                        weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear ||
                        weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure ||
                        weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw ||
                        weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal)
                    {
                        shipInventoryItemsIDs.Drill = CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory);
                    }
                    else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device)
                    {
                        shipInventoryItemsIDs.Harvester = CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory);
                    }
                    else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front)
                    {
                        shipInventoryItemsIDs.FrontLauncher = CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory);
                    }
                    else if (weapon.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back)
                    {
                        shipInventoryItemsIDs.BackLauncher = CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory);
                    }
                    else
                    {
                        int maxLeftWeapons = MyShipTypeConstants.GetShipTypeProperties(smallShipBuilder.ShipType).GamePlay.MaxLeftWeapons;
                        int maxRightWeapons = MyShipTypeConstants.GetShipTypeProperties(smallShipBuilder.ShipType).GamePlay.MaxRightWeapons;

                        // first, try to fit left-mounted weapon in left slots
                        if (weapon.AutoMountLeft && shipInventoryItemsIDs.LeftWeapons.Count < maxLeftWeapons)
                        {
                            shipInventoryItemsIDs.LeftWeapons.Add(CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory));
                            continue;
                        }

                        // or right-mounted weapon in right
                        if (weapon.AutoMountRight && shipInventoryItemsIDs.RightWeapons.Count < maxRightWeapons)
                        {
                            shipInventoryItemsIDs.RightWeapons.Add(CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory));
                            continue;
                        }

                        // then, auto-mounted weapon in left slots
                        if (weapon.AutoMount && shipInventoryItemsIDs.LeftWeapons.Count < maxLeftWeapons)
                        {
                            shipInventoryItemsIDs.LeftWeapons.Add(CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory));
                            continue;
                        }

                        // then, auto-mounted weapon in right slots
                        if (weapon.AutoMount && shipInventoryItemsIDs.RightWeapons.Count < maxRightWeapons)
                        {
                            shipInventoryItemsIDs.RightWeapons.Add(CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory));
                            continue;
                        }

                        // put it in inventory if it doesn't fit anywhere else
                        if (shipInventoryItemsIDs.Inventory.Count < smallShipBuilder.Inventory.MaxItems)
                        {
                            shipInventoryItemsIDs.Inventory.Add(CreateInventoryItemAddToRepository(weapon, smallShipBuilder.Inventory));
                            continue;
                        }
                    }
                }
            }

            // armor
            if (smallShipBuilder.Armor != null)
            {
                shipInventoryItemsIDs.Armor = CreateInventoryItemAddToRepository(smallShipBuilder.Armor, smallShipBuilder.Inventory);
            }

            // engine
            if (smallShipBuilder.Engine != null)
            {
                shipInventoryItemsIDs.Engine = CreateInventoryItemAddToRepository(smallShipBuilder.Engine, smallShipBuilder.Inventory);
            }

            // radar
            if (smallShipBuilder.Radar != null)
            {
                shipInventoryItemsIDs.Radar = CreateInventoryItemAddToRepository(smallShipBuilder.Radar, smallShipBuilder.Inventory);
            }

            return shipInventoryItemsIDs;
        }

        private void LoadInventoryItemsFromObjectBuilders()
        {
            // load all ship's inventory items from objectbuilders
            for (int i = 0; i < m_smallShipsBuilders.Count; i++)
            {
                MyMwcObjectBuilder_SmallShip smallShipBuilder = m_smallShipsBuilders[i].Builder;

                m_smallShipsInventoryItemIDs.Add(GetInventoryItemIDsFromObjectBuilder(smallShipBuilder));
            }

            // load other side inventory items from objectbuilders
            if (m_otherSideInventoryItemIDs != null)
            {
                foreach (MyMwcObjectBuilder_InventoryItem inventoryItem in m_otherSideInventoryBuilder.InventoryItems)
                {
                    m_otherSideInventoryItemIDs.Add(CreateInventoryItemAddToRepository(inventoryItem, m_otherSideInventoryBuilder));
                }
            }
        }

        private void SaveIntentoryItemsToObjectBuilder(int index) 
        {
            MySmallShipInventoryItemIDs shipItemIDs = m_smallShipsInventoryItemIDs[index];
            MyMwcObjectBuilder_SmallShip shipBuilder = m_smallShipsBuilders[index].Builder.Clone() as MyMwcObjectBuilder_SmallShip;

            // inventory
            if (shipBuilder.Inventory == null)
            {
                shipBuilder.Inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), Math.Max(MyInventory.DEFAULT_MAX_ITEMS, shipItemIDs.Inventory.Count));
            }
            if (shipBuilder.Inventory.InventoryItems == null)
            {
                shipBuilder.Inventory.InventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();
            }
            else if (shipBuilder.Inventory.InventoryItems.Count > 0)
            {
                shipBuilder.Inventory.InventoryItems.Clear();
            }
            foreach (int id in shipItemIDs.Inventory)
            {
                shipBuilder.Inventory.InventoryItems.Add(GetInventoryItemObjectBuilderFromRepository(id));
            }

            // weapons
            if (shipBuilder.Weapons == null)
            {
                shipBuilder.Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            }
            else if (shipBuilder.Weapons.Count > 0)
            {
                shipBuilder.Weapons.Clear();
            }
            foreach (int id in shipItemIDs.LeftWeapons)
            {
                var weaponBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(id);
                weaponBuilder.SetAutoMountLeft();
                shipBuilder.Weapons.Add(weaponBuilder);
            }
            foreach (int id in shipItemIDs.RightWeapons)
            {
                var weaponBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(id);
                weaponBuilder.SetAutoMountRight();
                shipBuilder.Weapons.Add(weaponBuilder);
            }
            if (shipItemIDs.Drill != null)
            {
                var drillBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(shipItemIDs.Drill.Value);
                drillBuilder.SetAutoMount();
                shipBuilder.Weapons.Add(drillBuilder);
            }
            if (shipItemIDs.Harvester != null)
            {
                var harvesterBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(shipItemIDs.Harvester.Value);
                harvesterBuilder.SetAutoMount();
                shipBuilder.Weapons.Add(harvesterBuilder);
            }
            if (shipItemIDs.BackLauncher != null)
            {
                var backLauncherBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(shipItemIDs.BackLauncher.Value);
                backLauncherBuilder.SetAutoMount();
                shipBuilder.Weapons.Add(backLauncherBuilder);
            }
            if (shipItemIDs.FrontLauncher != null)
            {
                var frontLauncherBuilder = (MyMwcObjectBuilder_SmallShip_Weapon)GetObjectBuilderFromRepository(shipItemIDs.FrontLauncher.Value);
                frontLauncherBuilder.SetAutoMount();
                shipBuilder.Weapons.Add(frontLauncherBuilder);
            }

            // armor
            if (shipItemIDs.Armor != null)
            {
                shipBuilder.Armor = (MyMwcObjectBuilder_SmallShip_Armor)GetObjectBuilderFromRepository(shipItemIDs.Armor.Value);
                shipBuilder.Armor.SetAutoMount();
            }
            else
            {
                shipBuilder.Armor = null;
            }

            // engine
            if (shipItemIDs.Engine != null)
            {
                shipBuilder.Engine = (MyMwcObjectBuilder_SmallShip_Engine)GetObjectBuilderFromRepository(shipItemIDs.Engine.Value);
                shipBuilder.Engine.SetAutoMount();
            }
            else
            {
                shipBuilder.Engine = null;
            }

            // radar
            if (shipItemIDs.Radar != null)
            {
                shipBuilder.Radar = (MyMwcObjectBuilder_SmallShip_Radar)GetObjectBuilderFromRepository(shipItemIDs.Radar.Value);
                shipBuilder.Radar.SetAutoMount();
            }
            else
            {
                shipBuilder.Radar = null;
            }
            m_smallShipsBuilders[index].Builder = shipBuilder;
        }

        private void SaveInventoryItemsToObjectBuilders(ref List<MySmallShipBuilderWithName> shipBuilders, ref MyMwcObjectBuilder_Inventory otherSideInventoryBuilder)
        {
            for (int i = 0; i < m_smallShipsBuilders.Count; i++)
            {
                SaveIntentoryItemsToObjectBuilder(i);
                //shipBuilders.Add(shipBuilder);
            }

            if (otherSideInventoryBuilder != null)
            {
                otherSideInventoryBuilder.InventoryItems.Clear();
                foreach (int id in m_otherSideInventoryItemIDs)
                {
                    otherSideInventoryBuilder.InventoryItems.Add(GetInventoryItemObjectBuilderFromRepository(id));
                }
            }
        }

        private void FillListBoxFromIDs(MyGuiControlListbox listbox, List<int> ids, List<ItemCategory> categories, ref List<int> hideList)
        {
            hideList.Clear();
            foreach (int id in ids)
            {
                MyInventoryItem inventoryItem = m_itemsRepository.GetItem(id);
                /*
                Debug.WriteLine(inventoryItem.Description);
                Debug.WriteLine(inventoryItem.ItemCategory);
                Debug.WriteLine(inventoryItem.GetObjectBuilder(true).GetObjectBuilderType());
                Debug.WriteLine("==================================================");
                 * */
                if (categories.Count == 0 || categories.Contains(inventoryItem.ItemProperties.ItemCategory))
                {
                    FillListBoxFromID(listbox, id);
                }
                else 
                {
                    hideList.Add(id);
                }
            }
        }

        private void FillListBoxFromIDs(MyGuiControlListbox listbox, List<int> ids)
        {
            foreach (int id in ids)
            {
                FillListBoxFromID(listbox, id);
            }
        }

        private MyToolTips GetListboxItemTooltip(MyGuiControlListbox listbox, int id)
        {
            MyInventoryItem item = m_itemsRepository.GetItem(id);
            MyToolTips toolTips = new MyToolTips();
            toolTips.AddToolTip(item.MultiLineDescription, Color.White, 0.7f);
            // add price to tooltip
            if (m_tradeForMoney)
            {
                if (item.CanBeTraded)
                {
                    StringBuilder sb = new StringBuilder();
                    if (listbox == m_shipInventoryListBox)
                    {
                        sb.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SellFor));
                    }
                    else if (listbox == m_otherSideInventoryListBox)
                    {
                        sb.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.BuyFor));
                    }
                    sb.Append(GetPriceSB((decimal)GetTradeItemPrice(item)));
                    toolTips.AddToolTip(sb, Color.Gold);
                }
                else
                {
                    toolTips.AddToolTip(MyTextsWrapper.Get(MyTextsWrapperEnum.NotificationNonTradeableItem), Color.Red);
                }
            }
            return toolTips;
        }

        private void FillListBoxFromID(MyGuiControlListbox listbox, int id, bool canBeMoved = true)
        {
            MyInventoryItem inventoryItem = m_itemsRepository.GetItem(id);
            StringBuilder description = null;
            if (inventoryItem.Icon == null)
            {
                description = inventoryItem.MultiLineDescription;
            }

            MyGuiControlListboxItem listboxItem = new MyGuiControlListboxItem(id, description, inventoryItem.Icon, GetListboxItemTooltip(listbox, id), MyGuiConstants.LABEL_TEXT_SCALE);
            listboxItem.IconTexts = new MyIconTexts();

            // add amount icon's text
            if (inventoryItem.Amount != 0f || inventoryItem.Amount != inventoryItem.MaxAmount)            
            {
                StringBuilder amount = new StringBuilder();
                if (inventoryItem.Amount > (int)inventoryItem.Amount)
                {
                    float fixedAmount = Math.Max(0.01f, inventoryItem.Amount);  // prevent items with zero amount, for visualization 0.01 is the minimum value
                    amount.AppendDecimal(fixedAmount, 2);
                }
                else 
                {
                    amount.AppendInt32((int)inventoryItem.Amount);                    
                }
                //amount.Append(inventoryItem.Amount.ToString());
                MyGuiDrawAlignEnum align;
                Vector2 offset;

                //All amounts alignet to right bottom position from now
                /*
                if (inventoryItem.AmountTextAlign == MyInventoryAmountTextAlign.MiddleRight)
                {
                    align = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER;
                    offset = new Vector2(-0.004f, 0.0038f);
                }
                 
                else if (inventoryItem.AmountTextAlign == MyInventoryAmountTextAlign.BottomRight)
                {
                 * */
                align = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM;
                offset = new Vector2(-0.013f, -0.01f);

                /*}
                    else
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
                 * */

                listboxItem.IconTexts[align] = new MyColoredText(amount, Color.White, Color.White, MyGuiManager.GetFontMinerWarsBlue(), 0.6f, offset);
            }
            bool enabled = inventoryItem.CanBeMoved && canBeMoved;
            if (m_tradeForMoney)
            {
                enabled &= inventoryItem.CanBeTraded;                
            }
            listboxItem.Enabled = enabled;
            //if (!inventoryItem.CanBeMoved) 
            //{
            //    listboxItem.Enabled = false;
            //}
            listbox.AddItem(listboxItem);
        }

        private StringBuilder GetPriceSB(decimal price)
        {
            StringBuilder priceSB = new StringBuilder();
            priceSB.Append(MyMwcUtils.GetFormatedPriceForGame(price));
            return priceSB;
        }

        private void InitializeListboxDragAndAddToControls(MyGuiControlListbox listbox)
        {
            Controls.Add(listbox);
            listbox.ItemDrag += OnDrag;
            //listbox.ItemSelect += OnItemClick;
            //listbox.ItemDoubleClick += OnItemDoubleClick;
        }

        private void FillShipsCombobox()
        {
            if (m_shipsCombobox != null)
            {
                for (int i = 0; i < m_smallShipsBuilders.Count; i++)
                {
                    m_shipsCombobox.AddItem(i, m_smallShipsBuilders[i].Name);
                }
                m_shipsCombobox.SelectItemByIndex(m_currentShipBuilderIndex);
            }
        }

        private void AddFreeRowsIfCan(MyGuiControlListbox listbox, int rowCount)
        {
            int maxRows = (int)Math.Ceiling((double)GetMaxListboxItems(listbox) / INVENTORY_COLUMNS);
            if (listbox.GetRowsCount() + rowCount <= maxRows)
            {
                listbox.AddRows(rowCount);
            }
        }

        private int GetMaxListboxItems(MyGuiControlListbox listbox)
        {
            if (listbox == m_shipInventoryListBox)
            {
                return GetMaxInventoryItems(m_smallShipsBuilders[m_currentShipBuilderIndex].Builder.Inventory);
            }
            else if (listbox == m_otherSideInventoryListBox)
            {
                return GetMaxInventoryItems(m_otherSideInventoryBuilder);
            }
            else
            {
                return listbox.GetRowsCount() * listbox.GetColumsCount();
            }
        }

        private int GetListboxItemsCount(MyGuiControlListbox listbox) 
        {
            if (listbox == m_shipInventoryListBox) 
            {
                return m_shipInventoryListBox.GetItemsCount() + m_shipInventoryHideList.Count;
            }
            else if (listbox == m_otherSideInventoryListBox)
            {
                return m_otherSideInventoryListBox.GetItemsCount() + m_otherSideInventoryHideList.Count;
            }
            else 
            {
                return listbox.GetItemsCount();
            }
        }

        private int GetMaxInventoryItems(MyMwcObjectBuilder_Inventory inventoryBuilder)
        {
            int maxItems;
            if (inventoryBuilder != null)
            {
                maxItems = inventoryBuilder.MaxItems;
            }
            else
            {
                maxItems = MyInventory.DEFAULT_MAX_ITEMS;
            }
            return maxItems;
        }

        private void UpdateOtherSideInventoryListboxForTrading() 
        {
            foreach (var listboxItem in m_otherSideInventoryListBox.GetItems()) 
            {
                MyInventoryItem inventoryItem = m_itemsRepository.GetItem(listboxItem.Key);
                float price = GetTradeItemPrice(inventoryItem);
                inventoryItem.NotEnoughMoney = price > m_money;                
                if (inventoryItem.CanBeTraded)
                {
                    Color color = inventoryItem.NotEnoughMoney ? Color.Red : Color.Gold;
                    listboxItem.ToolTip.GetToolTips()[1].NormalColor = color;
                    listboxItem.Enabled = !inventoryItem.NotEnoughMoney;
                }                
            }
        }

        private void FillOtherSideInventoryListbox()
        {
            if (m_otherSideInventoryBuilder != null)
            {
                //m_otherSideInventoryHideList.Clear();
                //m_otherSideInventoryListBox.RemoveAllItems(false);
                //FillListBoxFromIDs(m_otherSideInventoryListBox, m_otherSideInventoryItemIDs);
                //AddFreeRowsIfCan(m_otherSideInventoryListBox, 1);
                FilterInventory(m_otherSideInventoryListBox, m_otherSideInventoryItemIDs, m_tempFilteredCategoriesTheirsInventory, m_otherSideInventoryHideList,
                m_sortAll, m_sortWeapons, m_sortOres, m_sortConsumables, m_sortEquipment, m_sortGods);
            }
        }

        private void FillWeapons(List<MyGuiControlListbox> weaponListboxes, List<int> weaponKeys)
        {
            foreach (MyGuiControlListbox weaponListbox in weaponListboxes)
            {
                weaponListbox.RemoveAllItems(false);
            }
            for (int i = 0; i < weaponKeys.Count; i++)
            {
                FillListBoxFromID(weaponListboxes[i], weaponKeys[i]);
            }
        }

        private void FillShipCustomizationListboxes(int shipIndex)
        {
            //m_shipInventoryHideList.Clear();
            MySmallShipInventoryItemIDs currenShipItemIDs = m_smallShipsInventoryItemIDs[shipIndex];

            // inventory
            //m_shipInventoryListBox.RemoveAllItems();
            //FillListBoxFromIDs(m_shipInventoryListBox, currenShipItemIDs.Inventory);
            //AddFreeRowsIfCan(m_shipInventoryListBox, 1);
            FilterInventory(m_shipInventoryListBox, currenShipItemIDs.Inventory, m_tempFilteredCategoriesMyInventory, m_shipInventoryHideList,
                m_sortAllMy, m_sortWeaponsMy, m_sortOresMy, m_sortConsumablesMy, m_sortEquipmentMy, m_sortGodsMy);

            // left weapons
            //m_leftWeaponsInventoryListBox.RemoveAllItems();
            //FillListBoxFromIDs(m_leftWeaponsInventoryListBox, currenShipItemIDs.LeftWeapons);
            FillWeapons(m_leftWeaponListboxes, currenShipItemIDs.LeftWeapons);

            // right weapons
            //m_rightWeaponsInventoryListBox.RemoveAllItems();
            //FillListBoxFromIDs(m_rightWeaponsInventoryListBox, currenShipItemIDs.RightWeapons);
            FillWeapons(m_rightWeaponListboxes, currenShipItemIDs.RightWeapons);

            // drill
            m_drillInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.Drill != null)
            {
                FillListBoxFromID(m_drillInventoryListBox, currenShipItemIDs.Drill.Value, m_inventoryScreenType != MyGuiScreenInventoryType.Game);
            }

            // harvester
            m_harvestingToolInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.Harvester != null)
            {
                FillListBoxFromID(m_harvestingToolInventoryListBox, currenShipItemIDs.Harvester.Value, m_inventoryScreenType != MyGuiScreenInventoryType.Game);
            }

            // front launcher
            m_frontUniversalLauncherInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.FrontLauncher != null)
            {
                FillListBoxFromID(m_frontUniversalLauncherInventoryListBox, currenShipItemIDs.FrontLauncher.Value);
            }

            // back launcher
            m_backUniversalLauncherInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.BackLauncher != null)
            {
                FillListBoxFromID(m_backUniversalLauncherInventoryListBox, currenShipItemIDs.BackLauncher.Value);
            }

            // armor
            m_armorInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.Armor != null)
            {
                FillListBoxFromID(m_armorInventoryListBox, currenShipItemIDs.Armor.Value);
            }

            //// radar
            //m_radarInventoryListBox.RemoveAllItems(false);
            //if (currenShipItemIDs.Radar != null)
            //{
            //    FillListBoxFromID(m_radarInventoryListBox, currenShipItemIDs.Radar.Value);
            //}

            // engine
            m_engineInventoryListBox.RemoveAllItems(false);
            if (currenShipItemIDs.Engine != null)
            {
                FillListBoxFromID(m_engineInventoryListBox, currenShipItemIDs.Engine.Value);
            }

            UpdateEnabledStateOfWeaponsListboxes();
            
        }

        private int? GetKeyFromOneItemListbox(MyGuiControlListbox listbox)
        {
            List<int> keys = listbox.GetItemsKeys();
            if (keys.Count == 0)
            {
                return null;
            }
            else
            {
                return keys[0];
            }
        }

        private void SaveWeapons(List<MyGuiControlListbox> weaponListboxes, List<int> weaponKeys)
        {
            weaponKeys.Clear();
            int? weaponKey;
            foreach (MyGuiControlListbox weaponListbox in weaponListboxes)
            {
                weaponKey = GetKeyFromOneItemListbox(weaponListbox);
                if (weaponKey != null)
                {
                    weaponKeys.Add(weaponKey.Value);
                }
            }
        }

        private void SaveIDsFromShipCustomizationListboxes(int shipIndex)
        {
            MySmallShipInventoryItemIDs currenShipItemIDs = m_smallShipsInventoryItemIDs[shipIndex];

            // inventory
            currenShipItemIDs.Inventory.Clear();
            currenShipItemIDs.Inventory.AddRange(m_shipInventoryListBox.GetItemsKeys());
            currenShipItemIDs.Inventory.AddRange(m_shipInventoryHideList);

            // left weapons
            //currenShipItemIDs.LeftWeapons.Clear();
            //currenShipItemIDs.LeftWeapons.AddRange(m_leftWeaponsInventoryListBox.GetItemsKeys());
            SaveWeapons(m_leftWeaponListboxes, currenShipItemIDs.LeftWeapons);

            // right weapons
            //currenShipItemIDs.RightWeapons.Clear();
            //currenShipItemIDs.RightWeapons.AddRange(m_rightWeaponsInventoryListBox.GetItemsKeys());
            SaveWeapons(m_rightWeaponListboxes, currenShipItemIDs.RightWeapons);

            // drill
            currenShipItemIDs.Drill = GetKeyFromOneItemListbox(m_drillInventoryListBox);

            // harvester
            currenShipItemIDs.Harvester = GetKeyFromOneItemListbox(m_harvestingToolInventoryListBox);

            // front launcher
            currenShipItemIDs.FrontLauncher = GetKeyFromOneItemListbox(m_frontUniversalLauncherInventoryListBox);

            // back launcher
            currenShipItemIDs.BackLauncher = GetKeyFromOneItemListbox(m_backUniversalLauncherInventoryListBox);

            // armor
            currenShipItemIDs.Armor = GetKeyFromOneItemListbox(m_armorInventoryListBox);

            // engine
            currenShipItemIDs.Engine = GetKeyFromOneItemListbox(m_engineInventoryListBox);

            //// radar
            //currenShipItemIDs.Radar = GetKeyFromOneItemListbox(m_radarInventoryListBox);
        }

        private void SaveIDsFromOtherSideInventoryListbox()
        {
            if (m_otherSideInventoryBuilder != null)
            {
                m_otherSideInventoryItemIDs.Clear();
                m_otherSideInventoryItemIDs.AddRange(m_otherSideInventoryListBox.GetItemsKeys());
                m_otherSideInventoryItemIDs.AddRange(m_otherSideInventoryHideList);
            }
        }

        private void OnMoveFromLeftToRightInventoryClick(MyGuiControlButton sender)
        {
            MoveItemBetweenListboxes(m_otherSideInventoryListBox, m_shipInventoryListBox, m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor, false);
        }

        private void OnMoveFromRightToLeftInventoryClick(MyGuiControlButton sender)
        {
            MoveItemBetweenListboxes(m_shipInventoryListBox, m_otherSideInventoryListBox, false, m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor);
        }

        private float GetTradeItemPrice(MyInventoryItem inventoryItem) 
        {
            Debug.Assert(m_otherSideInventoryBuilder.PriceCoeficient != null);
            Debug.Assert(m_smallShipsBuilders[m_currentShipBuilderIndex].Builder.Inventory.PriceCoeficient != null);
            float otherSidePriceCoeficient = m_otherSideInventoryBuilder.PriceCoeficient.Value;
            float shipPriceCoeficient = m_smallShipsBuilders[m_currentShipBuilderIndex].Builder.Inventory.PriceCoeficient.Value;

            float priceCoeficient = 1f;

            if (inventoryItem.Owner == m_otherSideInventoryBuilder)
            {
                priceCoeficient = otherSidePriceCoeficient / shipPriceCoeficient;
            }
            else
            {
                priceCoeficient = shipPriceCoeficient / otherSidePriceCoeficient;
            }

            return inventoryItem.Price * priceCoeficient;
        }
        
        private bool HandleTradeForMoney(MyGuiControlListbox listboxFrom, MyGuiControlListbox listboxTo, MyInventoryItem inventoryItem)
        {            
            float moneyToAdd = 0f;
            float itemPrice = GetTradeItemPrice(inventoryItem);
            
            // item move from other side to player's ship)
            if (listboxFrom == m_otherSideInventoryListBox && listboxTo != m_otherSideInventoryListBox)
            {
                moneyToAdd = -itemPrice;
            }
            // item move from player's ship to other side
            else if (listboxFrom != m_otherSideInventoryListBox && listboxTo == m_otherSideInventoryListBox)
            {
                moneyToAdd = itemPrice;
            }            

            if (m_money + moneyToAdd < 0f) 
            {
                // this can't happen
                throw new MyMwcExceptionApplicationShouldNotGetHere();
                //MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
                //                                                     MyTextsWrapperEnum.NotificationYouDontHaveEnoughMoney,
                //                                                     MyTextsWrapperEnum.TradeResultTitle,
                //                                                     MyTextsWrapperEnum.Ok, YouDontHaveEnoughMoneyMessageBoxCallBack));
                return false;
            }
            SetMoney(m_money + moneyToAdd);
            return true;
        }

        private void SetMoney(float money) 
        {
            m_money = money;
            decimal moneyD = Math.Round((decimal)m_money, 0);
            m_playersMoneyLabel.UpdateParams(GetPriceSB(moneyD));
            float textScaleMultiplicator = 1f;
            if (m_money >= 1000000000)
            {
                textScaleMultiplicator = 0.8f;
            }
            else 
            {
                textScaleMultiplicator = 1f;
            }
            m_playersMoneyLabel.SetTextScale(MyGuiConstants.INVENTORY_LABEL_TEXT_SCALE * textScaleMultiplicator);
        }

        private void MoveItemBetweenListboxes(MyGuiControlListbox listboxFrom, MyGuiControlListbox listboxTo, bool copyItem, bool deleteItem)
        {
            bool moved = false;
            MyGuiControlListboxItem selectedItem = listboxFrom.GetSelectedItem();
            if (selectedItem != null)
            {
                System.Diagnostics.Debug.Assert(m_itemsRepository.Contains(selectedItem.Key));

                if (!IsListboxFull(listboxTo) && m_itemsRepository.Contains(selectedItem.Key))
                {
                    MyInventoryItem item = m_itemsRepository.GetItem(selectedItem.Key);
                    if (!CanDropItem(item, listboxFrom, listboxTo)) 
                    {
                        return;
                    }
                    
                    if (m_tradeForMoney)
                    {
                        selectedItem.ToolTip = GetListboxItemTooltip(listboxTo, selectedItem.Key);
                        bool tradeForMoneyResult = HandleTradeForMoney(listboxFrom, listboxTo, item);
                        if (!tradeForMoneyResult) 
                        {
                            return;
                        }
                    }

                    if (NeedHandleSmallShipDrop(item))
                    {
                        HandleSmallShipDrop(listboxFrom, listboxTo, item);                        
                    }

                    // we make copy of moved item
                    if (copyItem)
                    {
                        listboxTo.AddItem(CreateCopyOfListboxItem(selectedItem));
                    }
                    else
                    {
                        listboxFrom.RemoveItem(selectedItem.Key, false);
                        // if we don't want delete moved item
                        if (!deleteItem)
                        {
                            listboxTo.AddItem(selectedItem);
                        }
                    }
                    moved = true;
                    m_wasAnythingTrasfered = true;

                    if (m_tradeForMoney) 
                    {
                        UpdateOtherSideInventoryListboxForTrading();
                    }
                }
                else
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryFullWarning);
                }
            }

            if (moved && listboxTo.GetEmptyRowsCount() == 0)
            {
                AddFreeRowsIfCan(listboxTo, 1);                
            }
        }

        private void UpdateInventoryCapacityLabels() 
        {
            UpdateInventoryCapacityLabel(m_shipInventoryCapacityLabel, m_shipInventoryListBox);
            if (m_otherSideInventoryBuilder != null) 
            {
                UpdateInventoryCapacityLabel(m_otherSideInventoryCapacityLabel, m_otherSideInventoryListBox);
            }
        }

        private void UpdateNextAndPreviousShipButtonsVisibility() 
        {
            m_previousShipButton.Visible = m_smallShipsBuilders.Count > 1;
            m_nextShipButton.Visible = m_smallShipsBuilders.Count > 1;
        }

        private void UpdateInventoryCapacityLabel(MyGuiControlLabel label, MyGuiControlListbox listbox) 
        {
            int currentItems = GetListboxItemsCount(listbox);
            int maxItems = GetMaxListboxItems(listbox);
            maxItems = Math.Max(currentItems, maxItems);
            label.UpdateParams(new object[] { currentItems, maxItems });
            if (!IsListboxFull(listbox))
            {
                label.Font = MyGuiManager.GetFontMinerWarsBlue();
            }
            else 
            {
                label.Font = MyGuiManager.GetFontMinerWarsRed();
            }
        }

        private void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
            MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlClose);
        }

        private void OnOkClickDelegate()
        {
            OnOkClick(null);
        }

        private void YouDontHaveEnoughMoneyMessageBoxCallBack(MyGuiScreenMessageBoxCallbackEnum callbackReturn) 
        {
            this.UnhideScreen();
        }

        // save changes
        private void OnOkClick(MyGuiControlButton sender)
        {
            //if (m_inventoryScreenType != MyGuiScreenInventoryType.GodEditor) 
            //{
            //    if (m_tradeForMoney && m_money + m_moneyTradeBalance < 0f)
            //    {
            //        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
            //                                                         MyTextsWrapperEnum.NotificationYouDontHaveEnoughMoney,
            //                                                         MyTextsWrapperEnum.TradeResultTitle,
            //                                                         MyTextsWrapperEnum.Ok, YouDontHaveEnoughMoneyMessageBoxCallBack));
            //        return;
            //    }                
            //}

            if (MyFakes.ENABLE_LOADING_AFTER_TRADING && m_wasAnythingTrasfered && m_inventoryScreenType != MyGuiScreenInventoryType.GodEditor)
            {
                StartTransfering();                                
            }
            else
            {                
                Save();
                CloseScreen();
            }
            MyAudio.AddCue2D(MySoundCuesEnum.GuiWheelControlClose);
        }

        private bool IsListboxFull(MyGuiControlListbox listbox) 
        {
            bool isFull = GetListboxItemsCount(listbox) >= GetMaxListboxItems(listbox);
            //if (listbox == m_shipInventoryListBox) 
            //{
            //    isFull = isFull && !MyGameplayCheats.IsCheatEnabled(MyGameplayCheatsEnum.UNLIMITED_CARGO_CAPACITY);
            //}
            return isFull;
        }

        // save changes
        private void OnTakeAllClick(MyGuiControlButton sender)
        {
            foreach (var otherKey in m_otherSideInventoryListBox.GetItemsKeys())
            {
                if (!IsListboxFull(m_shipInventoryListBox))
                {
                    MyInventoryItem inventoryItem = m_itemsRepository.GetItem(otherKey);
                    if (inventoryItem.CanBeMoved && CanDropItem(inventoryItem, m_otherSideInventoryListBox, m_shipInventoryListBox))
                    {
                        if (NeedHandleSmallShipDrop(inventoryItem))
                        {
                            HandleSmallShipDrop(m_otherSideInventoryListBox, m_shipInventoryListBox, inventoryItem);
                        }

                        MyGuiControlListboxItem item = m_otherSideInventoryListBox.GetItem(otherKey);
                        item.ToolTip = GetListboxItemTooltip(m_shipInventoryListBox, otherKey);
                        m_shipInventoryListBox.AddItem(item);
                        m_otherSideInventoryListBox.RemoveItem(otherKey);                        
                    }
                }
                else
                {
                    break;
                }
            }
            m_otherSideInventoryListBox.RemoveEmptyItems();
            // if there are no empty row, we try add new row
            if (m_shipInventoryListBox.GetEmptyRowsCount() == 0)
            {
                AddFreeRowsIfCan(m_shipInventoryListBox, 1);
            }
        }

        private void RemoveItemsFromListbox(MyGuiControlListbox listbox, bool removeItems = false) 
        {
            foreach (int key in listbox.GetItemsKeys())
            {
                m_removedInventoryItemIDs.Add(key);
            }
            listbox.RemoveAllItems(removeItems);
            if (removeItems)
            {                
                AddFreeRowsIfCan(listbox, 1);
            }            
        }

        private void OnRemoveAllClick(MyGuiControlButton sender)
        {
            foreach (int key in m_shipInventoryHideList)
            {
                m_removedInventoryItemIDs.Add(key);
            }
            m_shipInventoryHideList.Clear();
            RemoveItemsFromListbox(m_shipInventoryListBox, true);            
            RemoveItemsFromListbox(m_harvestingToolInventoryListBox);            
            RemoveItemsFromListbox(m_drillInventoryListBox);            
            RemoveItemsFromListbox(m_armorInventoryListBox);            
            RemoveItemsFromListbox(m_engineInventoryListBox);            
            RemoveItemsFromListbox(m_frontUniversalLauncherInventoryListBox);            
            RemoveItemsFromListbox(m_backUniversalLauncherInventoryListBox);           
            //RemoveItemsFromListbox(m_radarInventoryListBox);            
                                    
            foreach (var leftWeaponListbox in m_leftWeaponListboxes) 
            {
                RemoveItemsFromListbox(leftWeaponListbox);                
            }
            foreach (var rightWeaponListbox in m_rightWeaponListboxes)
            {
                RemoveItemsFromListbox(rightWeaponListbox);                
            }
        }

        private void UncheckAllInventoryCheckboxes()
        {
            m_sortWeapons.UnCheck();
            m_sortGods.UnCheck();
            m_sortOres.UnCheck();
            m_sortConsumables.UnCheck();
            m_sortEquipment.UnCheck();
        }


        // save changes
        private void OnFilterInventorySortClick(MyGuiControlCheckbox sender)
        {
            SaveIDsFromOtherSideInventoryListbox();
            SaveIDsFromShipCustomizationListboxes(m_currentShipBuilderIndex);
            if (sender.Checked)
            {
                if (sender == m_sortAll)
                {
                    UncheckAllInventoryCheckboxes();
                }
                else
                {
                    m_sortAll.UnCheck();
                }
            }
            else if (sender == m_sortAll)
            {
                m_sortAll.Check();
            }

            FilterInventory(m_otherSideInventoryListBox, m_otherSideInventoryItemIDs, m_tempFilteredCategoriesTheirsInventory, m_otherSideInventoryHideList,
                m_sortAll, m_sortWeapons, m_sortOres, m_sortConsumables, m_sortEquipment, m_sortGods);
            if (m_tradeForMoney)
            {
                UpdateOtherSideInventoryListboxForTrading();
            }
        }

        private void UncheckAllMyInventoryCheckboxes()
        {
            m_sortWeaponsMy.UnCheck();
            m_sortGodsMy.UnCheck();
            m_sortOresMy.UnCheck();
            m_sortConsumablesMy.UnCheck();
            m_sortEquipmentMy.UnCheck();
        }

        private void FilterInventory(MyGuiControlListbox listbox, List<int> itemsIDs, List<ItemCategory> filter, List<int> hideList,
            MyGuiControlCheckbox checkboxAll, MyGuiControlCheckbox checkboxWeapons, MyGuiControlCheckbox checkboxOres,
            MyGuiControlCheckbox checkboxConsumables, MyGuiControlCheckbox checkboxEquipment, MyGuiControlCheckbox checkboxGods)
        {
            if (listbox != null)
            {                
                listbox.RemoveAllItems();
                listbox.ResetScrollbarPosition();
                filter.Clear();
                if (!checkboxAll.Checked) 
                {
                    if (checkboxWeapons.Checked) filter.AddRange(MyItemFilterConstants.CategoryAmmo);
                    if (checkboxOres.Checked) filter.AddRange(MyItemFilterConstants.Ores);
                    if (checkboxConsumables.Checked) filter.AddRange(MyItemFilterConstants.ConsumAndMedical);
                    if (checkboxEquipment.Checked) filter.AddRange(MyItemFilterConstants.Devices);
                    if (checkboxGods.Checked) filter.AddRange(MyItemFilterConstants.GoodsAndIllegal);
                }
                FillListBoxFromIDs(listbox, itemsIDs, filter, ref hideList);
                AddFreeRowsIfCan(listbox, 1);
            }
        }

        // save changes
        private void OnFilterInventoryMySortClick(MyGuiControlCheckbox sender)
        {
            SaveIDsFromOtherSideInventoryListbox();
            SaveIDsFromShipCustomizationListboxes(m_currentShipBuilderIndex);
            if (sender.Checked) 
            {
                if (sender == m_sortAllMy)
                {
                    UncheckAllMyInventoryCheckboxes();
                }
                else 
                {
                    m_sortAllMy.UnCheck();
                }
            }
            else if (sender == m_sortAllMy)
            {
                m_sortAllMy.Check();
            }

            FilterInventory(m_shipInventoryListBox, m_smallShipsInventoryItemIDs[m_currentShipBuilderIndex].Inventory, m_tempFilteredCategoriesMyInventory, m_shipInventoryHideList,
                m_sortAllMy, m_sortWeaponsMy, m_sortOresMy, m_sortConsumablesMy, m_sortEquipmentMy, m_sortGodsMy);            
        }



        #region Drag and drop handling
        private MyGuiControlListboxItem CreateCopyOfListboxItem(MyGuiControlListboxItem templateItem)
        {
            MyInventoryItem templateInventoryItem = m_itemsRepository.GetItem(templateItem.Key);
            int newItemKey = CreateInventoryItemAddToRepository(templateInventoryItem.GetInventoryItemObjectBuilder(true), templateInventoryItem.Amount, (MyMwcObjectBuilder_Inventory)templateInventoryItem.Owner);
            MyGuiControlListboxItem itemsCopy = new MyGuiControlListboxItem(newItemKey, templateItem.ColoredText, templateItem.Icon, templateItem.ToolTip);
            itemsCopy.IconTexts = templateItem.IconTexts;
            return itemsCopy;
        }

        private void StartDragging(MyDropHandleType dropHandlingType, MyGuiControlListbox listbox, int rowIndex, int itemIndex)
        {
            MyDragAndDropInfo dragAndDropInfo = new MyDragAndDropInfo();
            dragAndDropInfo.Listbox = listbox;
            dragAndDropInfo.RowIndex = rowIndex;
            dragAndDropInfo.ItemIndex = itemIndex;

            MyGuiControlListboxItem draggingItem = dragAndDropInfo.Listbox.GetItem(dragAndDropInfo.RowIndex, dragAndDropInfo.ItemIndex);            

            // if inventory screen is in god editor mode, then we want use other side inventory items as templates
            if (m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor && listbox == m_otherSideInventoryListBox)
            {
                draggingItem = CreateCopyOfListboxItem(draggingItem);
            }
            else
            {
                dragAndDropInfo.Listbox.RemoveItem(dragAndDropInfo.RowIndex, dragAndDropInfo.ItemIndex, false);
            }

            m_dragAndDrop.StartDragging(dropHandlingType, draggingItem, dragAndDropInfo);
            listbox.HideToolTip();
            DisabledInvalidListboxesForDrop(m_itemsRepository.GetItem(draggingItem.Key));

            if (listbox != m_shipInventoryListBox && IsListboxFull(m_shipInventoryListBox)) 
            {
                m_shipInventoryListBox.Enabled = false;
            }
            else if (listbox != m_otherSideInventoryListBox && IsListboxFull(m_otherSideInventoryListBox)) 
            {
                m_otherSideInventoryListBox.Enabled = false;
            }
        }

        private void StopDragging()
        {
            m_dragAndDrop.Stop();
            EnableAllListboxes();
        }

        private void DisabledInvalidListboxesForDrop(MyInventoryItem draggedInventoryItem)
        {
            foreach (KeyValuePair<MyGuiControlListbox, Predicate<MyInventoryItem>> keyValuePair in m_listboxDropConditions)
            {
                if (!keyValuePair.Value(draggedInventoryItem))
                {
                    keyValuePair.Key.Enabled = false;
                }
            }            
        }        

        private void EnableAllListboxes()
        {
            foreach (KeyValuePair<MyGuiControlListbox, Predicate<MyInventoryItem>> keyValuePair in m_listboxDropConditions)
            {
                keyValuePair.Key.Enabled = true;
            }
            UpdateEnabledStateOfWeaponsListboxes();
        }

        private void UpdateEnabledStateOfWeaponsListboxes()
        {
            var shipTypeGameplayProperties = MyShipTypeConstants.GetShipTypeProperties(m_smallShipsBuilders[m_currentShipBuilderIndex].Builder.ShipType).GamePlay;
            int rightWeaponsSlots = shipTypeGameplayProperties.MaxRightWeapons;
            int leftWeaponsSlots = shipTypeGameplayProperties.MaxLeftWeapons;

            for (int i = 0; i < INVENTORY_WEAPONS_COLUMNS; i++)
            {
                m_leftWeaponListboxes[i].Enabled = i < leftWeaponsSlots;
                m_rightWeaponListboxes[i].Enabled = i < rightWeaponsSlots;
            }
        }

        private void OnItemClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            //StartDraging(MyDragAndDropType.LeftMouseClick, sender, eventArgs);
        }

        private void OnOtherSideItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            MoveItemBetweenListboxes(m_otherSideInventoryListBox, m_shipInventoryListBox, m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor, false);
        }

        private void OnShipItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            MoveItemBetweenListboxes(m_shipInventoryListBox, m_otherSideInventoryListBox, false, m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor);
        }

        private void OnItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            if (!m_isTransferingInProgress)
            {
                //StartDragging(MyDropHandleType.LeftMouseClick, (MyGuiControlListbox)sender, eventArgs.RowIndex, eventArgs.ItemIndex);
            }
        }

        private void OnDrag(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            if (!m_isTransferingInProgress)
            {
                StartDragging(MyDropHandleType.LeftMousePressed, (MyGuiControlListbox)sender, eventArgs.RowIndex, eventArgs.ItemIndex);
            }
        }

        private void OnDrop(object sender, MyDragAndDropEventArgs eventArgs)
        {
            if (eventArgs.DropTo != null)
            {
                MyInventoryItem inventoryItem = m_itemsRepository.GetItem(eventArgs.ListboxItem.Key);
                MyGuiControlListboxItem itemAtDroppingPosition = eventArgs.DropTo.Listbox.GetItem(eventArgs.DropTo.RowIndex, eventArgs.DropTo.ItemIndex);

                // test drop condition
                Predicate<MyInventoryItem> dropCondition = m_listboxDropConditions[eventArgs.DropTo.Listbox];
                // drop condition false (check if we able to drop this item to this listbox)
                if (!dropCondition(inventoryItem) || !CanDropItem(inventoryItem, eventArgs.DragFrom.Listbox, eventArgs.DropTo.Listbox))
                {
                    DropBack(eventArgs);
                }
                // check if listbox is full and we try drop item, if true drop item back
                else if (IsListboxFull(eventArgs.DropTo.Listbox))
                {
                    // if we try drop item at ship or other side inventory, then play warning
                    if (eventArgs.DropTo.Listbox == m_shipInventoryListBox || eventArgs.DropTo.Listbox == m_otherSideInventoryListBox)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryFullWarning);
                    }
                    DropBack(eventArgs);
                }
                // drop condition true
                else
                {
                    if (m_tradeForMoney)
                    {
                        bool tradeForMoneyResult = HandleTradeForMoney(eventArgs.DragFrom.Listbox, eventArgs.DropTo.Listbox, inventoryItem);
                        if (!tradeForMoneyResult) 
                        {
                            DropBack(eventArgs);
                            return;
                        }
                        eventArgs.ListboxItem.ToolTip = GetListboxItemTooltip(eventArgs.DropTo.Listbox, eventArgs.ListboxItem.Key);
                    }

                    if (NeedHandleSmallShipDrop(inventoryItem))
                    {
                        HandleSmallShipDrop(eventArgs.DragFrom.Listbox, eventArgs.DropTo.Listbox, inventoryItem);
                    }
                    DropItem(eventArgs.DropTo.Listbox, eventArgs.ListboxItem, eventArgs.DropTo.RowIndex, eventArgs.DropTo.ItemIndex);
                    m_wasAnythingTrasfered = true;

                    if (m_tradeForMoney) 
                    {
                        UpdateOtherSideInventoryListboxForTrading();
                    }
                    //StopDragging();                    
                }
            }
            else
            {
                // if item was dropped over inventory screen, then put them to original slot
                if (IsMouseOver() || eventArgs.DragFrom.Listbox == m_otherSideInventoryListBox)
                {
                    DropBack(eventArgs);
                }
                // if item was dropped out of inventory screen, then remove from game
                else
                {
                    m_removedInventoryItemIDs.Add(eventArgs.ListboxItem.Key);
                    m_wasAnythingTrasfered = true;
                    StopDragging();
                }                
            }
        }

        private bool NeedHandleSmallShipDrop(MyInventoryItem item) 
        {
            return item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Player &&
                m_inventoryScreenType == MyGuiScreenInventoryType.Game && !m_tradeForMoney;
        }

        private bool CanDropItem(MyInventoryItem item, MyGuiControlListbox dropFrom, MyGuiControlListbox dropTo) 
        {
            if (dropTo != dropFrom && NeedHandleSmallShipDrop(item) && dropTo == m_shipInventoryListBox)
            {
                MySmallShipBuilderWithName builderWithName = m_smallShipsBuilders.Find(x => x.Builder == item.GetInventoryItemObjectBuilder(false));
                Debug.Assert(builderWithName != null);
                int index = m_smallShipsBuilders.IndexOf(builderWithName);
                Debug.Assert(index > -1);
                // we can't drop same ship to current ship's inventory
                return index != m_currentShipBuilderIndex;
            }
            else 
            {
                return true;
            }
        }

        private void HandleSmallShipDrop(MyGuiControlListbox listboxFrom, MyGuiControlListbox listboxTo, MyInventoryItem item) 
        {
            if (listboxFrom != listboxTo) 
            {
                if (listboxTo == m_otherSideInventoryListBox)
                {
                    MySmallShipBuilderWithName builderWithName = new MySmallShipBuilderWithName(item.GetInventoryItemObjectBuilder(false) as MyMwcObjectBuilder_SmallShip_Player);
                    m_smallShipsBuilders.Add(builderWithName);
                    m_smallShipsInventoryItemIDs.Add(GetInventoryItemIDsFromObjectBuilder(builderWithName.Builder));
                    if (m_shipsCombobox != null)
                    {
                        m_shipsCombobox.AddItem(m_shipsCombobox.GetItemsCount(), builderWithName.Name);
                    }
                    item.IsTemporaryItem = true;
                }
                else if (listboxTo == m_shipInventoryListBox)
                {
                    MySmallShipBuilderWithName builderWithName = m_smallShipsBuilders.Find(x => x.Builder == item.GetInventoryItemObjectBuilder(false));
                    Debug.Assert(builderWithName != null);
                    int index = m_smallShipsBuilders.IndexOf(builderWithName);
                    Debug.Assert(index > -1);                                        
                    SaveIntentoryItemsToObjectBuilder(index);                    
                    item.ObjectBuilder = m_smallShipsBuilders[index].Builder;
                    DealocateInventoryItemsFromInventoryIDs(index);
                    m_smallShipsInventoryItemIDs.RemoveAt(index);
                    m_smallShipsBuilders.RemoveAt(index);
                    if (m_shipsCombobox != null)
                    {
                        m_shipsCombobox.RemoveItemByIndex(index);
                    }
                    if (m_currentShipBuilderIndex > index) 
                    {
                        m_currentShipBuilderIndex--;
                    }
                    Debug.Assert(m_currentShipBuilderIndex >= 0 && m_currentShipBuilderIndex <= m_smallShipsBuilders.Count - 1);
                    item.IsTemporaryItem = false;
                    //bool reloadShipInventory = m_currentShipBuilderIndex > index;
                    //m_currentShipBuilderIndex = Math.Min(m_currentShipBuilderIndex, m_smallShipsBuilders.Count - 1);
                    //if (reloadShipInventory)
                    //{
                    //    LoadShipInventory(m_currentShipBuilderIndex);
                    //}
                }
                else 
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
            }            
        }

        private void DealocateInventoryItemsFromInventoryIDs(int index) 
        {
            MySmallShipInventoryItemIDs inventoryIDs = m_smallShipsInventoryItemIDs[index];
            DealocateInventoryItems(inventoryIDs.Inventory);
            DealocateInventoryItems(inventoryIDs.LeftWeapons);
            DealocateInventoryItems(inventoryIDs.RightWeapons);
            DealocateInventoryItem(inventoryIDs.Armor);
            DealocateInventoryItem(inventoryIDs.BackLauncher);
            DealocateInventoryItem(inventoryIDs.Drill);
            DealocateInventoryItem(inventoryIDs.Engine);
            DealocateInventoryItem(inventoryIDs.FrontLauncher);
            DealocateInventoryItem(inventoryIDs.Harvester);
            DealocateInventoryItem(inventoryIDs.Radar);            
        }

        private void DealocateInventoryItems(List<int> ids) 
        {
            foreach (int id in ids)
            {
                DealocateInventoryItem(id);
            }
        }

        private void DealocateInventoryItem(int? id) 
        {
            if (id != null) 
            {
                m_itemsRepository.RemoveItem(id.Value, true);
            }
        }

        private void DropBack(MyDragAndDropEventArgs eventArgs) 
        {
            DropItem(eventArgs.DragFrom.Listbox, eventArgs.ListboxItem, eventArgs.DragFrom.RowIndex, eventArgs.DragFrom.ItemIndex);
            //StopDragging();
        }

        private void DropItem(MyGuiControlListbox listbox, MyGuiControlListboxItem item, int rowIndex, int itemIndex)
        {
            // in god editor and other side listbox, we don't want put this item to this listbox, so we remove it
            if (m_inventoryScreenType == MyGuiScreenInventoryType.GodEditor && listbox == m_otherSideInventoryListBox)
            {
                m_itemsRepository.RemoveItem(item.Key);
            }
            else
            {
                if (listbox.GetItem(rowIndex, itemIndex) != null || listbox.GetRowsCount() <= rowIndex)
                {
                    listbox.AddItem(item);
                }
                else
                {
                    listbox.AddItem(item, rowIndex, itemIndex);
                }

                // if there are no empty row, we try add new row
                if (listbox.GetEmptyRowsCount() == 0)
                {
                    AddFreeRowsIfCan(listbox, 1);
                }                
            }
            StopDragging();
        }
        #endregion

        public override string GetFriendlyName()
        {
            return "MyGuiScreenInventory";
        }

        public override void UnloadContent()
        {
            m_itemsRepository.Close();
            if (m_inventoryItemTexture != null)
            {
                MyTextureManager.UnloadTexture(m_inventoryItemTexture);
                m_inventoryItemTexture = null;
            }
            if (m_inventoryScrollBarTexture != null && m_inventoryScrollBarTexture.LoadState == Managers.LoadState.Loaded)
            {
                MyTextureManager.UnloadTexture(m_inventoryScrollBarTexture);
                m_inventoryScrollBarTexture = null;
            }
            base.UnloadContent();
        }

        public override void UnloadData()
        {
            base.UnloadData();
        }

        public override bool CloseScreen()
        {
            return base.CloseScreen();
        }

        public override void CloseScreenNow()
        {
            base.CloseScreenNow();
        }

        public void CancelTransfering()
        //public void CancelTransfering(MyDetectedEntity sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.TradeWasCanceled, MyTextsWrapperEnum.TradeResultTitle, MyTextsWrapperEnum.Ok, null));
            CloseScreen();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
 	         base.HandleInput(input, receivedFocusInThisUpdate);
             if (input.IsNewGameControlPressed(MyGameControlEnums.INVENTORY))
             {
                 Canceling();
             }
        }

        private void StartTransfering()
        {
            m_isTransferingInProgress = true;
            m_okButton.Enabled = false;
            m_okButton.Visible = false;
            //m_cancelButton.Enabled = false;
            //m_cancelButton.Visible = false;            

            m_timeFromStartTransfering = 0;
            m_transferingProgress = new MyGuiControlRotatingWheel(this, new Vector2(0f, 0f), MyGuiConstants.DEFAULT_CONTROL_FOREGROUND_COLOR.ToVector4(), 0.5f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiManager.GetLoadingTexture());
            OnEnterCallback -= OnOkClickDelegate;
            DrawMouseCursor = false;
            m_transferingProgressCue = MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryTransfer);
        }

        private void CompleteTransfering()
        {
            m_isTransferingInProgress = false;
            Save();
            MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryComplete);
            CloseScreenNow();
        }

        private void Save()
        {
            m_dragAndDrop.Drop();

            //if (m_wasAnythingTrasfered)
            //{
            //    SaveIDsFromShipCustomizationListboxes(m_currentShipBuilderIndex);
            //    SaveIDsFromOtherSideInventoryListbox();

            //    SaveInventoryItemsToObjectBuilders();
            //    if (m_tradeForMoney)
            //    {
            //        m_money += m_moneyTradeBalance;
            //    }
            //}

            SaveIDsFromShipCustomizationListboxes(m_currentShipBuilderIndex);
            SaveIDsFromOtherSideInventoryListbox();

            //List<MyMwcObjectBuilder_SmallShip> shipBuilders = new List<MyMwcObjectBuilder_SmallShip>();
            SaveInventoryItemsToObjectBuilders(ref m_smallShipsBuilders, ref m_otherSideInventoryBuilder);            

            if (m_inventoryScreenType == MyGuiScreenInventoryType.Game && m_removedInventoryItemIDs.Count > 0)
            {
                CreateCargoBoxWithDroppedItems(m_removedInventoryItemIDs, MySession.PlayerShip);
                m_removedInventoryItemIDs.Clear();
            }
            m_isInventoryLocked = false;

            if (OnSave != null)
            {
                MyGuiScreenInventorySaveResult saveResult = new MyGuiScreenInventorySaveResult(m_smallShipsBuilders, m_currentShipBuilderIndex, m_otherSideInventoryBuilder, m_money, m_wasAnythingTrasfered);
                OnSave(this, saveResult);
            }
        }

        public bool IsInventoryLocked() 
        {
            return m_isInventoryLocked;
        }

        void CreateCargoBoxWithDroppedItems(IEnumerable<int> droppedItemIDs, MySmallShip dropper)
        {
            var worldMatrix = Matrix.CreateWorld(
                dropper.WorldVolume.Center, 
                dropper.GetForward(), 
                dropper.GetUp());

            var items = new List<MyMwcObjectBuilder_InventoryItem>();
            foreach (var droppedItemID in droppedItemIDs)
            {
                var inventoryItem = m_itemsRepository.GetItem(droppedItemID);
                items.Add(new MyMwcObjectBuilder_InventoryItem(inventoryItem.GetInventoryItemObjectBuilder(false), inventoryItem.Amount));
            }
            var inventory = new MyMwcObjectBuilder_Inventory(items, MyInventory.DEFAULT_MAX_ITEMS);

            var cargoBoxBuilder = new MyMwcObjectBuilder_CargoBox(inventory)
                {
                    CargoBoxType = MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems
                };
            cargoBoxBuilder.PersistentFlags |= MyPersistentEntityFlags.ActivatedOnAllDifficulties;

            var cargoBox = MyEntities.CreateFromObjectBuilderAndAdd(null, cargoBoxBuilder, worldMatrix);

            var matrix = cargoBox.WorldMatrix;
            matrix.Translation += 0.8f * dropper.WorldVolume.Radius * dropper.GetForward();
            matrix.Translation -= 0.5f * dropper.WorldVolume.Radius * dropper.GetUp();
            cargoBox.WorldMatrix = matrix;

            cargoBoxBuilder.EntityId = cargoBox.EntityId.ToNullableUInt();
            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.NewEntity(cargoBoxBuilder, cargoBox.WorldMatrix);
                MyMultiplayerGameplay.Static.HookEntity(cargoBox);
            }
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false)
            {
                return false;
            }

            ScreenTimeout -= TimeSpan.FromMilliseconds(MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);

            if (m_isTransferingInProgress)
            {
                m_transferingProgress.Update();
                if (m_transferingProgressCue == null || !m_transferingProgressCue.Value.IsPlaying)
                {
                    m_timeFromStartTransfering += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                    if (m_timeFromStartTransfering >= COMPLETE_TRANSFER_TIME)
                    {
                        CompleteTransfering();
                    }
                }
                HideTooltips();
            }
            else if (ScreenTimeout <= TimeSpan.Zero)
            {
                CloseScreen();
            }

            UpdateInventoryCapacityLabels();
            UpdateNextAndPreviousShipButtonsVisibility();

            return true;
        }

        private bool IsDraggingOut()
        {
            return m_dragAndDrop.IsActive() && !IsMouseOver();
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (m_isTransferingInProgress)
            {
                m_transferingProgress.Draw();
            }
            else
            {
                if (!base.Draw(backgroundFadeAlpha))
                    return false;
                if (IsDraggingOut())
                {
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetLockedInventoryItem(), MyGuiManager.MouseCursorPosition,
                                                 m_dragAndDrop.GetSize().Value, new Color(new Vector4(1f, 0f, 0f, 0.5f)),
                                                 MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
            }            

            //if (m_isTransferingInProgress)
            //{
            //    float tempTransitionAlhpa = m_transitionAlpha;
            //    m_transitionAlpha = 0.3f;
            //    if (!base.Draw(backgroundFadeAlpha)) return false;
            //    m_transitionAlpha = tempTransitionAlhpa;                

            //    m_transferingProgress.Draw();
            //}
            //else
            //{
            //    if (!base.Draw(backgroundFadeAlpha)) return false;

            //    if (IsDraggingOut())
            //    {
            //        MyGuiManager.DrawSpriteBatch(MyGuiManager.GetLockedButtonTexture(), MyGuiManager.MouseCursorPosition,
            //                                     m_dragAndDrop.GetSize().Value, new Color(new Vector4(1f, 0f, 0f, 0.5f)),
            //                                     MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            //    }
            //}

            return true;
        }
    }
}

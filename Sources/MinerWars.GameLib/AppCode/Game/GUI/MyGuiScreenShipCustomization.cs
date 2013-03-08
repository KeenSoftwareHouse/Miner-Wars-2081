////#define AdvancedControls

//using System;
//using System.Collections.Generic;
//using System.Text;
//using MinerWarsMath;
//using MinerWars.AppCode.Game.GUI.Core;
//using MinerWars.AppCode.Game.GUI.Helpers;
//using MinerWars.AppCode.Game.Localization;
//using MinerWars.AppCode.Game.Entities;
//using MinerWars.AppCode.Game.Utils;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
//using MinerWars.CommonLIB.AppCode.Utils;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
//using SysUtils.Utils;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders;
//using MinerWars.AppCode.Game.Managers.Session;
//using MinerWars.AppCode.Game.Gameplay;

//namespace MinerWars.AppCode.Game.GUI
//{
//    class MyGuiScreenShipCustomization : MyGuiScreenBase
//    {
//        private class MyInventory
//        {
//            public List<MyMwcObjectBuilder_SmallShip_TypesEnum> SmallShips { get; set; }
//            public List<MyMwcObjectBuilder_SmallShip_Ammo> Ammo { get; set; }
//            public List<MyMwcObjectBuilder_SmallShip_Engine> Engines { get; set; }
//            public List<MyMwcObjectBuilder_SmallShip_Weapon> Weapons { get; set; }
//            public List<MyMwcObjectBuilder_SmallShip_Tool> Tools { get; set; }

//            public MyInventory()
//            {
//                SmallShips = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>();
//                Ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
//                Engines = new List<MyMwcObjectBuilder_SmallShip_Engine>();
//                Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
//                Tools = new List<MyMwcObjectBuilder_SmallShip_Tool>();
//            }

//            //This temporary method fill inventory with everything. Inventory will be load from somewhere later.
//            public void FillInventory()
//            {
//                foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Ammo_TypesEnumValues)
//                {
//                    //Ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(1000, item));
//                    Ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(item));
//                }
//                foreach (MyMwcObjectBuilder_SmallShip_Engine_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Engine_TypesEnumValues)
//                {
//                    Engines.Add(new MyMwcObjectBuilder_SmallShip_Engine(item));
//                }
//                foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Weapon_TypesEnumValues)
//                {
//                    Weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(item));
//                }
//                foreach (MyMwcObjectBuilder_SmallShip_TypesEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues)
//                {
//                    SmallShips.Add(item);
//                }
//                foreach (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum toolEnum in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_ToolEnumValues)
//                {
//                    Tools.Add(new MyMwcObjectBuilder_SmallShip_Tool(toolEnum));
//                }
//            }
//        }

//        private enum MyInventoryItemTypesEnum
//        {
//            Ammo = 1,
//            Engine = 2,
//            Weapons = 3,
//            Ships = 4,
//            Tools = 5
//        }

//        //MyGuiControlShipPreview m_shipPreview;
//        MyMwcObjectBuilder_SmallShip m_smallShipObjectBuilder;

//        public delegate void ShipCustomizationBoxCallback(MyMwcObjectBuilder_SmallShip callbackReturn);
//        ShipCustomizationBoxCallback m_callback=null;

//        MyInventory m_inventory;

//        MyGuiControlCombobox m_comboTypes;
//        MyGuiControlCombobox m_comboItems;


//#if AdvancedControls
//        MyGuiControlButton m_buttonShip;
//        MyGuiControlButton m_buttonEngine;
//        MyGuiControlCombobox m_comboAddedTools;
//        MyGuiControlCombobox m_comboAddedAmmo;
//#else
//        MyGuiControlCombobox m_comboAdded;
//        Dictionary<int,Object> m_comboAddedObjects;
//#endif

//        private bool IsComboItemsVisible
//        {
//            get
//            {
//                return Controls.Contains(m_comboItems);
//            }
//            set
//            {
//                if (Controls.Contains(m_comboItems))
//                {
//                    if (value == false) Controls.Remove(m_comboItems);
//                }
//                else
//                {
//                    if (value == true) Controls.Add(m_comboItems);
//                }
//            }
//        }

//        //  Temorary combobox clearing. (Becouse it cannot be empty.)
//        private void ClearCombobox(MyGuiControlCombobox combobox)
//        {
//            combobox.ClearItems();
//            combobox.AddItem(0, new StringBuilder("Empty"));
//            combobox.SelectItemByIndex(0);
//        }

//        public MyGuiScreenShipCustomization(MyMwcObjectBuilder_SmallShip obj, ShipCustomizationBoxCallback callback = null, bool allowNewObjectBuilder = false)
//            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.7f, 0.9f))
//        {

//            AddCaption(MyTextsWrapperEnum.ShipCustomizationCaption);
//            m_enableBackgroundFade = true;
//            m_drawBackgroundInterference = false;

//            //  Copy current small ship object builder (with copied lists) for new customized ship
//            m_smallShipObjectBuilder = /*(MyMwcObjectBuilder_SmallShip)MySession.PlayerShip.GetObjectBuilder()*/ obj;
//            if (m_smallShipObjectBuilder == null && !allowNewObjectBuilder)
//            {
//                return;
//            }

//            m_callback = callback;
//            //m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip(
//            //    m_smallShipObjectBuilder.ShipType,
//            //    m_smallShipObjectBuilder.Weapons == null ? null : new List<MyMwcObjectBuilder_SmallShip_Weapon>(m_smallShipObjectBuilder.Weapons),
//            //    m_smallShipObjectBuilder.Engine,
//            //    m_smallShipObjectBuilder.Ammo == null ? null : new List<MyMwcObjectBuilder_SmallShip_Ammo>(m_smallShipObjectBuilder.Ammo),
//            //    m_smallShipObjectBuilder.AssignmentOfAmmo == null ? null : new List<MyMwcObjectBuilder_AssignmentOfAmmo>(m_smallShipObjectBuilder.AssignmentOfAmmo),
//            //    m_smallShipObjectBuilder.Tools == null ? null : new List<MyMwcObjectBuilder_SmallShip_Tool>(m_smallShipObjectBuilder.Tools)
//            //    );
//            m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip(
//                m_smallShipObjectBuilder.ShipType,
//                m_smallShipObjectBuilder.Inventory,
//                m_smallShipObjectBuilder.Weapons == null ? null : new List<MyMwcObjectBuilder_SmallShip_Weapon>(m_smallShipObjectBuilder.Weapons),
//                m_smallShipObjectBuilder.Engine,                
//                m_smallShipObjectBuilder.AssignmentOfAmmo == null ? null : new List<MyMwcObjectBuilder_AssignmentOfAmmo>(m_smallShipObjectBuilder.AssignmentOfAmmo),
//                m_smallShipObjectBuilder.Armor,
//                m_smallShipObjectBuilder.Radar,
//                m_smallShipObjectBuilder.ShipHealth,
//                m_smallShipObjectBuilder.ArmorHealth,
//                m_smallShipObjectBuilder.Electricity,
//                m_smallShipObjectBuilder.Oxygen,
//                m_smallShipObjectBuilder.Fuel
//                );

//            if (m_smallShipObjectBuilder != null)
//            {
//                m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip(
//                    m_smallShipObjectBuilder.ShipType,
//                    m_smallShipObjectBuilder.Inventory,
//                    m_smallShipObjectBuilder.Weapons == null ? null : new List<MyMwcObjectBuilder_SmallShip_Weapon>(m_smallShipObjectBuilder.Weapons),
//                    m_smallShipObjectBuilder.Engine,
//                    m_smallShipObjectBuilder.AssignmentOfAmmo == null ? null : new List<MyMwcObjectBuilder_AssignmentOfAmmo>(m_smallShipObjectBuilder.AssignmentOfAmmo),
//                    m_smallShipObjectBuilder.Armor,
//                    m_smallShipObjectBuilder.Radar,
//                    m_smallShipObjectBuilder.ShipHealth,
//                    m_smallShipObjectBuilder.ArmorHealth,
//                    m_smallShipObjectBuilder.Electricity,
//                    m_smallShipObjectBuilder.Oxygen,
//                    m_smallShipObjectBuilder.Fuel
//                    );
//            }
//            else
//            {
//                m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip();
//            }

//            m_inventory = new MyInventory();
//            m_inventory.FillInventory();

//            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.025f, -m_size.Value.Y / 2.0f + 0.145f);
//            Vector2 controlsOriginRight = new Vector2(0.05f, -m_size.Value.Y / 2.0f + 0.145f);

//            #region Inventory controls

//            //  Type label
//            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.ShipCustomizationTypeLabel, MyGuiConstants.LABEL_TEXT_COLOR,
//                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

//            //  Type combobox
//            m_comboTypes = new MyGuiControlCombobox(this, controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 6);

//            m_comboTypes.AddItem((int)MyInventoryItemTypesEnum.Ships, MyTextsWrapperEnum.Ship);
//            m_comboTypes.AddItem((int)MyInventoryItemTypesEnum.Engine, MyTextsWrapperEnum.Engine);
//            m_comboTypes.AddItem((int)MyInventoryItemTypesEnum.Weapons, MyTextsWrapperEnum.Weapon);
//            m_comboTypes.AddItem((int)MyInventoryItemTypesEnum.Ammo, MyTextsWrapperEnum.Ammo);
//            m_comboTypes.AddItem((int)MyInventoryItemTypesEnum.Tools, MyTextsWrapperEnum.Tool);
//            m_comboTypes.SelectItemByIndex(0);
//            Controls.Add(m_comboTypes);
//            m_comboTypes.OnSelect = OnTypeComboboxSelect;

//            //  Subtype label
//            Controls.Add(new MyGuiControlLabel(this, controlsOriginRight + 0 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.ShipCustomizationSubtypeLabel, MyGuiConstants.LABEL_TEXT_COLOR, 
//                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

//            //  Subtype combobox
//            //m_comboItems = new MyGuiControlCombobox(this, position + new Vector2(0.125f, 0f), 0.5f, MyGuiConstants.LABEL_TEXT_COLOR,
//            //    MyGuiConstants.LABEL_TEXT_SCALE);
//            m_comboItems = new MyGuiControlCombobox(this, controlsOriginRight + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 6);
//            Controls.Add(m_comboItems);

//            //  Add button
//            Controls.Add(new MyGuiControlButton(this, new Vector2(0.0f, controlsOriginLeft.Y + 3 * MyGuiConstants.CONTROLS_DELTA.Y), MyGuiConstants.BACK_BUTTON_SIZE,
//                MyGuiConstants.BACK_BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Add, MyGuiConstants.BACK_BUTTON_TEXT_COLOR,
//                MyGuiConstants.BACK_BUTTON_TEXT_SCALE, OnAddNewItem, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

//            #endregion

//            #region Current ship controls

//#if AdvancedControls
//            m_buttonShip = new MyGuiControlButton(this, new Vector2(-0.27f, 0f), new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                null, null, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
//            Controls.Add(m_buttonShip);

//            m_comboAddedTools = new MyGuiControlCombobox(this, new Vector2(0.05f, 0f), 0.53f, MyGuiConstants.COMBOBOX_HEIGHT,
//                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 3, false, false, true);
//            Controls.Add(m_comboAddedTools);
//            m_comboAddedTools.OnSelect = OnRemoveToolFromShip;

//            m_buttonsWeapon.Add(MyWeaponPossiblePositionsOnSmallShip.MoreLeft, new MyGuiControlButton(this, new Vector2(-0.27f, 0.14f),
//                new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, null,
//                delegate() { OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip.MoreLeft); },
//                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            m_buttonsWeapon.Add(MyWeaponPossiblePositionsOnSmallShip.Left, new MyGuiControlButton(this, new Vector2(-0.16f, 0.14f),
//                new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, null,
//                delegate() { OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip.Left); },
//                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            m_buttonsWeapon.Add(MyWeaponPossiblePositionsOnSmallShip.Middle, new MyGuiControlButton(this, new Vector2(-0.05f, 0.14f),
//                new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, null,
//                delegate() { OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip.Middle); },
//                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            m_buttonsWeapon.Add(MyWeaponPossiblePositionsOnSmallShip.Right, new MyGuiControlButton(this, new Vector2(0.06f, 0.14f),
//                new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, null,
//                delegate() { OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip.Right); },
//                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            m_buttonsWeapon.Add(MyWeaponPossiblePositionsOnSmallShip.MoreRight, new MyGuiControlButton(this, new Vector2(0.17f, 0.14f),
//                new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR, null,
//                delegate() { OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip.MoreRight); },
//                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            foreach (var item in m_buttonsWeapon) Controls.Add(item.Value);

//            m_buttonEngine = new MyGuiControlButton(this, new Vector2(-0.27f, 0.28f), new Vector2(0.1f, 0.1f), MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                null, OnRemoveEngineFromShip, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
//            Controls.Add(m_buttonEngine);

//            m_comboAddedAmmo = new MyGuiControlCombobox(this, new Vector2(0.05f, 0.28f), 0.53f, MyGuiConstants.COMBOBOX_HEIGHT,
//                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 3, false, false, true);
//            Controls.Add(m_comboAddedAmmo);
//            m_comboAddedAmmo.OnSelect = OnRemoveAmmoFromShip;
//#else
//            // Your ship configuration label
//            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0f, controlsOriginRight.Y + 5 * MyGuiConstants.CONTROLS_DELTA.Y), null, new StringBuilder("Your ship configuration"), MyGuiConstants.LABEL_TEXT_COLOR,
//                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

//            m_comboAdded = new MyGuiControlCombobox(this, controlsOriginLeft + 6 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LARGE_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.LARGE,
//                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 8, false, false, true);

//            Controls.Add(m_comboAdded);
//            m_comboAdded.OnSelect = OnRemoveFromShip;
//            m_comboAddedObjects = new Dictionary<int, object>();

//            //  Temporary label!
//            Controls.Add(new MyGuiControlLabel(this, new Vector2(0.0f, 0.34f), null, new StringBuilder("Click on an item to remove it from your ship"),
//                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
//#endif

//            #endregion

//            UpdateControls();

//            //  Buttons OK and BACK
//            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
//            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
//                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
//        }

//        void UpdateControls()
//        {
//#if AdvancedControls
//            foreach (var item in m_buttonsWeapon)
//            {
//                MyMwcObjectBuilder_SmallShip_Weapon weapon = MyGuiSmallShipHelpers.GetWeaponOnPosition(m_smallShipObjectBuilder.Weapons, item.Key);
//                item.Value.SetImage(weapon == null ? null : MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperWeapon(weapon.WeaponType).Icon);
//            }

//            m_buttonShip.SetImage(MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperSmallShip(m_smallShipObjectBuilder.ShipType).Icon);

//            m_buttonEngine.SetImage(m_smallShipObjectBuilder.Engine == null ? null : MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperEngine(m_smallShipObjectBuilder.Engine.EngineType).Icon);

//            bool noTools = true;
//            m_comboAddedTools.ClearItems();
//            if (m_smallShipObjectBuilder.Tools != null)
//            {
//                for (int i = 0; i < m_smallShipObjectBuilder.Tools.Count; i++)
//                {
//                    m_comboAddedTools.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperTool(m_smallShipObjectBuilder.Tools[i].GetObjectBuilderType()).Description);
//                    noTools = false;
//                }
//            }
//            if (noTools) ClearCombobox(m_comboAddedTools);
//            m_comboAddedTools.SelectItemByIndex(0);

//            bool noAmmo = true;
//            m_comboAddedAmmo.ClearItems();
//            if (m_smallShipObjectBuilder.Ammo != null)
//            {
//                for (int i = 0; i < m_smallShipObjectBuilder.Ammo.Count; i++)
//                {
//                    m_comboAddedAmmo.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_smallShipObjectBuilder.Ammo[i].AmmoType).Description);
//                    noAmmo = false;
//                }
//            }
//            if (noAmmo) ClearCombobox(m_comboAddedAmmo);
//            m_comboAddedAmmo.SelectItemByIndex(0);
//#else
//            int i = 0;
//            m_comboAddedObjects.Clear();
//            m_comboAdded.ClearItems();
//            //  Ship type
//            i++;
//            m_comboAddedObjects.Add(i, m_smallShipObjectBuilder.ShipType);
//            if (m_smallShipObjectBuilder.ShipType != 0)
//                //m_comboAdded.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperSmallShip(m_smallShipObjectBuilder.ShipType).Description);
//                m_comboAdded.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)m_smallShipObjectBuilder.ShipType).Description);
//            //  Engine type
//            if (m_smallShipObjectBuilder.Engine != null)
//            {
//                i++;
//                m_comboAddedObjects.Add(i, m_smallShipObjectBuilder.Engine);
//                //m_comboAdded.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperEngine(m_smallShipObjectBuilder.Engine.EngineType).Description);
//                m_comboAdded.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)m_smallShipObjectBuilder.Engine.EngineType).Description);
                
//            }
//            //  Weapons
//            if (m_smallShipObjectBuilder.Weapons != null)
//            {
//                foreach (var item in m_smallShipObjectBuilder.Weapons)
//                {
//                    i++;
//                    m_comboAddedObjects.Add(i, item);
//                    //m_comboAdded.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperWeapon(item.WeaponType).Description);
//                    m_comboAdded.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)item.WeaponType).Description);
//                }

//            }
//            //  Ammo
//            if (m_smallShipObjectBuilder.Ammo != null)
//            {
//                foreach (var item in m_smallShipObjectBuilder.Ammo)
//                {
//                    i++;
//                    m_comboAddedObjects.Add(i, item);
//                    //StringBuilder description = new StringBuilder(MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(item.AmmoType).Description.ToString());
//                    StringBuilder description = new StringBuilder(MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)item.AmmoType).Description.ToString());
//                    //description.Append(" (");
//                    //MyMwcUtils.AppendInt32ToStringBuilder(description, item.Amount);
//                    //description.Append(")");
//                    m_comboAdded.AddItem(i, description);
//                }

//            }
//            //  Tools
//            if (m_smallShipObjectBuilder.Tools != null)
//            {
//                foreach (var item in m_smallShipObjectBuilder.Tools)
//                {
//                    i++;
//                    m_comboAddedObjects.Add(i, item);
//                    //m_comboAdded.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperTool(
//                    //    (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum)item.GetObjectBuilderId()).Description);
//                    m_comboAdded.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, item.GetObjectBuilderId().Value).Description);
//                }

//            }
//            if (m_comboAdded.GetItemsCount() > 0)
//            {
//                m_comboAdded.SelectItemByIndex(0);
//            }
//#endif

//            OnTypeComboboxSelect();
//        }

//        void OnTypeComboboxSelect()
//        {
//            m_comboItems.ClearItems();
//            switch ((MyInventoryItemTypesEnum)m_comboTypes.GetSelectedKey())
//            {
//                case MyInventoryItemTypesEnum.Ammo:
//                    IsComboItemsVisible = (m_inventory.Ammo.Count > 0);
//                    for (int i = 0; i < m_inventory.Ammo.Count; i++)
//                    {
//                        //MyGuiSmallShipHelperAmmo helper = MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_inventory.Ammo[i].AmmoType);
//                        //if (helper != null)
//                        //{
//                        //    m_comboItems.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_inventory.Ammo[i].AmmoType).Description);
//                        //}
//                        MyGuiHelperBase helper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)m_inventory.Ammo[i].AmmoType);
//                        if (helper != null)
//                        {
//                            m_comboItems.AddItem(i, helper.Description);
//                        }
//                    }
//                    break;
//                case MyInventoryItemTypesEnum.Engine:
//                    IsComboItemsVisible = (m_inventory.Engines.Count > 0);
//                    for (int i = 0; i < m_inventory.Engines.Count; i++)
//                    {
//                        //m_comboItems.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperEngine(m_inventory.Engines[i].EngineType).Description);
//                        m_comboItems.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(
//                            MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)m_inventory.Engines[i].EngineType).Description);
//                    }
//                    break;
//                case MyInventoryItemTypesEnum.Weapons:
//                    IsComboItemsVisible = (m_inventory.Weapons.Count > 0);
//                    for (int i = 0; i < m_inventory.Weapons.Count; i++)
//                    {
//                        //m_comboItems.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperWeapon(m_inventory.Weapons[i].WeaponType).Description);
//                        m_comboItems.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(
//                            MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)m_inventory.Weapons[i].WeaponType).Description);
//                    }
//                    break;
//                case MyInventoryItemTypesEnum.Ships:
//                    IsComboItemsVisible = (m_inventory.SmallShips.Count > 0);
//                    for (int i = 0; i < m_inventory.SmallShips.Count; i++)
//                    {
//                        //m_comboItems.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperSmallShip(m_inventory.SmallShips[i]).Description);
//                        m_comboItems.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(
//                            MyMwcObjectBuilderTypeEnum.SmallShip, (int)m_inventory.SmallShips[i]).Description);
//                    }
//                    break;
//                case MyInventoryItemTypesEnum.Tools:
//                    IsComboItemsVisible = (m_inventory.Tools.Count > 0);
//                    for (int i = 0; i < m_inventory.Tools.Count; i++)
//                    {
//                        //m_comboItems.AddItem(i, MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperTool(
//                        //    (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum)m_inventory.Tools[i].GetObjectBuilderId()).Description);
//                        m_comboItems.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(
//                            MyMwcObjectBuilderTypeEnum.SmallShip_Tool, m_inventory.Tools[i].GetObjectBuilderId().Value).Description);
//                    }
//                    break;
//                default:
//                    throw new MyMwcExceptionApplicationShouldNotGetHere();
//                    break;
//            }
//            if (IsComboItemsVisible && m_comboItems.GetItemsCount() > 0) m_comboItems.SelectItemByIndex(0);
//        }

//#if AdvancedControls 
//        void OnRemoveWeaponFromShip(MyWeaponPossiblePositionsOnSmallShip position)
//        {
//            MyMwcObjectBuilder_SmallShip_Weapon weapon = MyGuiSmallShipHelpers.GetWeaponOnPosition(m_smallShipObjectBuilder.Weapons, position);
//            if (weapon == null) return;
//            m_smallShipObjectBuilder.Weapons.Remove(weapon);
//            m_inventory.Weapons.Add(weapon);
//            UpdateControls();
//        }

//        void OnRemoveEngineFromShip()
//        {
//            MyMwcObjectBuilder_SmallShip_Engine engine = m_smallShipObjectBuilder.Engine;
//            if (engine == null) return;
//            m_smallShipObjectBuilder.Engine = null;
//            m_inventory.Engines.Add(engine);
//            UpdateControls();
//        }

//        void OnRemoveToolFromShip()
//        {
//            if (m_smallShipObjectBuilder.Tools == null || m_smallShipObjectBuilder.Tools.Count == 0) return;
//            MyMwcObjectBuilder_SmallShip_ToolBase tool = m_smallShipObjectBuilder.Tools[m_comboAddedTools.GetSelectedKey()];
//            if (tool == null) return;
//            m_smallShipObjectBuilder.Tools.Remove(tool);
//            m_inventory.Tools.Add(tool);
//            UpdateControls();
//        }

//        void OnRemoveAmmoFromShip()
//        {
//            if (m_smallShipObjectBuilder.Ammo == null || m_smallShipObjectBuilder.Ammo.Count == 0) return;
//            MyMwcObjectBuilder_SmallShip_Ammo ammo = m_smallShipObjectBuilder.Ammo[m_comboAddedAmmo.GetSelectedKey()];
//            if (ammo == null) return;
//            m_smallShipObjectBuilder.Ammo.Remove(ammo);
//            m_inventory.Ammo.Add(ammo);
//            UpdateControls();
//        }
//#else
//        void OnRemoveFromShip()
//        {
//            Object selected = m_comboAddedObjects[m_comboAdded.GetSelectedKey()];

//            //  Ship
//            if (selected is MyMwcObjectBuilder_SmallShip_TypesEnum)
//            {
//                //This is temporary message box! TODO: Create localizable text!
//                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(new StringBuilder("Just add new ship from combobox above."), new StringBuilder("CANNOT REMOVE SHIP"), MyTextsWrapperEnum.Ok, null));
//                return;
//            }
//            //  Engine
//            MyMwcObjectBuilder_SmallShip_Engine engine = selected as MyMwcObjectBuilder_SmallShip_Engine;
//            if (engine != null)
//            {
//                m_smallShipObjectBuilder.Engine = null;
//                m_inventory.Engines.Add(engine);
//                UpdateControls();
//                return;
//            }
//            //  Weapon
//            MyMwcObjectBuilder_SmallShip_Weapon weapon = selected as MyMwcObjectBuilder_SmallShip_Weapon;
//            if (weapon != null)
//            {
//                m_smallShipObjectBuilder.Weapons.Remove(weapon);
//                m_inventory.Weapons.Add(weapon);
//                UpdateControls();
//                return;
//            }
//            //  Ammo
//            MyMwcObjectBuilder_SmallShip_Ammo ammo = selected as MyMwcObjectBuilder_SmallShip_Ammo;
//            if (ammo != null)
//            {
//                m_smallShipObjectBuilder.Ammo.Remove(ammo);
//                m_inventory.Ammo.Add(ammo);
//                UpdateControls();
//                return;
//            }
//            //  Tool
//            MyMwcObjectBuilder_SmallShip_Tool tool = selected as MyMwcObjectBuilder_SmallShip_Tool;
//            if (tool != null)
//            {
//                m_smallShipObjectBuilder.Tools.Remove(tool);
//                m_inventory.Tools.Add(tool);
//                UpdateControls();
//                return;
//            }
//        }
//#endif

//        void OnAddNewItem()
//        {
//            if (!IsComboItemsVisible) return;

//            switch ((MyInventoryItemTypesEnum)m_comboTypes.GetSelectedKey())
//            {
//                case MyInventoryItemTypesEnum.Engine:
//                    if (m_smallShipObjectBuilder.Engine != null) m_inventory.Engines.Add(m_smallShipObjectBuilder.Engine);
//                    MyMwcObjectBuilder_SmallShip_Engine engine = m_inventory.Engines[m_comboItems.GetSelectedKey()];
//                    m_smallShipObjectBuilder.Engine = engine;
//                    m_inventory.Engines.Remove(engine);
//                    break;
//                case MyInventoryItemTypesEnum.Weapons:
//                    MyMwcObjectBuilder_SmallShip_Weapon weapon = m_inventory.Weapons[m_comboItems.GetSelectedKey()];
//                    bool isFreePosition = false;
//                    /* TODO: To be rewritten in new ship customization dialog
//                    foreach (var position in MyGuiSmallShipHelpers.GetPosiblePositions(weapon.WeaponType))
//                    {
//                        if (MyGuiSmallShipHelpers.IsWeaponPositionFree(m_smallShipObjectBuilder.Weapons, position))
//                        {
//                            weapon.PositionOnShip = MyGuiSmallShipHelpers.GetWeaponVector(position);
//                            if (m_smallShipObjectBuilder.Weapons == null) m_smallShipObjectBuilder.Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
//                            m_smallShipObjectBuilder.Weapons.Add(weapon);
//                            m_inventory.Weapons.Remove(weapon);
//                            isFreePosition = true;
//                            break;
//                        }
//                    } */
//                    if (m_smallShipObjectBuilder.Weapons == null) 
//                        m_smallShipObjectBuilder.Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
//                    m_smallShipObjectBuilder.Weapons.Add(weapon);
//                    m_inventory.Weapons.Remove(weapon);
//                    isFreePosition = true;


//                    //This is temporary messagebox!
//                    if (!isFreePosition) MyGuiManager.AddScreen(new MyGuiScreenMessageBox(new StringBuilder("No free position on the ship for this weapon."), new StringBuilder("NO FREE POSITION"), MyTextsWrapperEnum.Ok, null));
//                    break;
//                case MyInventoryItemTypesEnum.Ships:
//                    if (m_smallShipObjectBuilder.Engine != null) m_inventory.Engines.Add(m_smallShipObjectBuilder.Engine);
//                    if (m_smallShipObjectBuilder.Ammo != null)
//                    {
//                        foreach (var item in m_smallShipObjectBuilder.Ammo)
//                        {
//                            m_inventory.Ammo.Add(item);
//                        }
//                    }
//                    if (m_smallShipObjectBuilder.Weapons != null)
//                    {
//                        foreach (var item in m_smallShipObjectBuilder.Weapons)
//                        {
//                            m_inventory.Weapons.Add(item);
//                        }
//                    }
//                    if (m_smallShipObjectBuilder.Tools != null)
//                    {
//                        foreach (var item in m_smallShipObjectBuilder.Tools)
//                        {
//                            m_inventory.Tools.Add(item);
//                        }
//                    }
//                    m_inventory.SmallShips.Add(m_smallShipObjectBuilder.ShipType);
//                    MyMwcObjectBuilder_SmallShip_TypesEnum smallShip = m_inventory.SmallShips[m_comboItems.GetSelectedKey()];
//                    //m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip(
//                    //    smallShip, 
//                    //    new List<MyMwcObjectBuilder_SmallShip_Weapon>(), 
//                    //    null, 
//                    //    new List<MyMwcObjectBuilder_SmallShip_Ammo>(), 
//                    //    new List<MyMwcObjectBuilder_AssignmentOfAmmo>(), 
//                    //    new List<MyMwcObjectBuilder_SmallShip_Tool>());
//                    m_smallShipObjectBuilder = new MyMwcObjectBuilder_SmallShip(
//                        smallShip,
//                        null,
//                        new List<MyMwcObjectBuilder_SmallShip_Weapon>(),
//                        null,
//                        new List<MyMwcObjectBuilder_AssignmentOfAmmo>(),
//                        null,
//                        null,
//                        MyGameplayConstants.MAXHEALTH_SMALLSHIP,
//                        100f,
//                        float.MaxValue,
//                        float.MaxValue,
//                        float.MaxValue);
//                    m_inventory.SmallShips.Remove(smallShip);
//                    break;
//                case MyInventoryItemTypesEnum.Ammo:
//                    MyMwcObjectBuilder_SmallShip_Ammo ammo = m_inventory.Ammo[m_comboItems.GetSelectedKey()];
//                    if (m_smallShipObjectBuilder.Ammo == null) m_smallShipObjectBuilder.Ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
//                    m_smallShipObjectBuilder.Ammo.Add(ammo);
//                    m_inventory.Ammo.Remove(ammo);
//                    break;
//                case MyInventoryItemTypesEnum.Tools:
//                    MyMwcObjectBuilder_SmallShip_Tool tool = m_inventory.Tools[m_comboItems.GetSelectedKey()];
//                    if (m_smallShipObjectBuilder.Tools == null) m_smallShipObjectBuilder.Tools = new List<MyMwcObjectBuilder_SmallShip_Tool>();
//                    m_smallShipObjectBuilder.Tools.Add(tool);
//                    m_inventory.Tools.Remove(tool);
//                    break;
//                default:
//                    break;
//            }
//            OnTypeComboboxSelect();
//            UpdateControls();
//        }

//        void OnCancelClick()
//        {
//            CloseScreen();
//        }

//        void OnOkClick()
//        {
//            if (m_callback!=null)
//            {
//                m_callback(new MyMwcObjectBuilder_SmallShip_Player(
//                        m_smallShipObjectBuilder.ShipType,
//                        m_smallShipObjectBuilder.Inventory,
//                        m_smallShipObjectBuilder.Weapons,
//                        m_smallShipObjectBuilder.Engine,
//                        m_smallShipObjectBuilder.AssignmentOfAmmo,
//                        m_smallShipObjectBuilder.Armor,
//                        m_smallShipObjectBuilder.Radar,
//                        m_smallShipObjectBuilder.ShipHealth,
//                        m_smallShipObjectBuilder.ArmorHealth,
//                        m_smallShipObjectBuilder.Electricity, 
//                        m_smallShipObjectBuilder.Oxygen,
//                        m_smallShipObjectBuilder.Fuel
//                    ));
//            }
//            else
//            {
//                var matrix = MySession.PlayerShip.WorldMatrix;
//                MySession.PlayerShip.Close();
//                //Copy small ship object builder for new player ship
//                MyEntities.CreateFromObjectBuilderAndAdd(null, 
//                    new MyMwcObjectBuilder_SmallShip_Player(
//                        m_smallShipObjectBuilder.ShipType,
//                        m_smallShipObjectBuilder.Inventory,
//                        m_smallShipObjectBuilder.Weapons,
//                        m_smallShipObjectBuilder.Engine,
//                        m_smallShipObjectBuilder.AssignmentOfAmmo,
//                        m_smallShipObjectBuilder.Armor,
//                        m_smallShipObjectBuilder.Radar,
//                        m_smallShipObjectBuilder.ShipHealth,
//                        m_smallShipObjectBuilder.ArmorHealth,
//                        m_smallShipObjectBuilder.Electricity, 
//                        m_smallShipObjectBuilder.Oxygen,
//                        m_smallShipObjectBuilder.Fuel),
//                    matrix);
//            }
//            CloseScreen();
//        }

//        public override void LoadContent()
//        {
//            base.LoadContent();
//            //m_shipPreview = new MyGuiControlShipPreview(this, m_smallShipObjectBuilder);
//            //Controls.Add(m_shipPreview);
//        }

//        public override string GetFriendlyName()
//        {
//            return "MyGuiScreenShipCustomization";
//        }

//        public override void UnloadContent()
//        {
//            base.UnloadContent();
//            //if (m_shipPreview != null)
//            //{
//            //    Controls.Remove(m_shipPreview);
//            //    m_shipPreview.UnloadContent();
//            //    m_shipPreview = null;
//            //}
//        }
//    }
//}
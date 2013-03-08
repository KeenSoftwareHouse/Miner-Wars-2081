using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Core.TreeView;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiIngameEditorControls : MyGuiEditorControlsBase
    {
        [Flags]
        private enum MyTreeViewItemUpdateStateFlag
        {
            BuildingQueue = 1,
            BuildCountAndDragAndDrop = 2,
            CanBuildAndRequirements = 4,
            BuildingProgress = 8,
            All = BuildingQueue | BuildCountAndDragAndDrop | CanBuildAndRequirements | BuildingProgress,
        }

        private static Vector2 ORE_INFORMATION_RIGHT_OFFSET = new Vector2(0.15f, 0f);
        private const int UPDATE_TREEVIEW_ITEMS_STATE = 200;
        private static readonly StringBuilder ORE_TEXT_SEPARATOR = new StringBuilder(": ");
        private static readonly StringBuilder PERCENTAGE = new StringBuilder("%");
        private static readonly Vector2 CHECK_BOX_SIZE = new Vector2(0.05f, 0.05f);
        private int m_lastUpdateTime;        
        private StringBuilder m_oreCount;
        private StringBuilder m_oreName;
        private List<MyInventoryItem> m_helperInventoryItems = new List<MyInventoryItem>();

        private MyGuiControlCheckbox m_checkboxOnlyItemsOnWhichHasBlueprints;
        private MyGuiControlCheckbox m_checkboxOnlyItemsOnWhichHasOre;
        private MyGuiControlCheckbox m_checkboxOnlyItemsWhichHasBuild;

        private bool m_activeItemsOnWhichHasBlueprintsFilter;
        private bool m_activeItemsOnWhichHasOreFilter;
        private bool m_activeItemsWhichHasBuildFilter;

        private static readonly EditorToolBarButtonType[] m_enabledButtons =
            new EditorToolBarButtonType[]
                {
                    EditorToolBarButtonType.AddObjectButton,
                    EditorToolBarButtonType.EditObjectsButton,
                    EditorToolBarButtonType.ResetRotationButton,
                    EditorToolBarButtonType.CopySelectedButton,
                    EditorToolBarButtonType.SwitchGizmoSpaceButton,
                    EditorToolBarButtonType.SwitchGizmoModeButton,
                    EditorToolBarButtonType.LinkSnapPoints,
                    EditorToolBarButtonType.ToggleSnapPoints,
                    EditorToolBarButtonType.SelectAllObjectsButton,
                    EditorToolBarButtonType.ExitSelectedButton,
                    EditorToolBarButtonType.EnterPrefabContainerButton,
                    EditorToolBarButtonType.AttachToCameraButton,
                    EditorToolBarButtonType.EnterVoxelHandButton,
                    EditorToolBarButtonType.ClearWholeSectorButton,
                    EditorToolBarButtonType.SaveSectorButton,
                    EditorToolBarButtonType.SunSettingsButton,
                    EditorToolBarButtonType.GroupsButton,
                    EditorToolBarButtonType.OptionsButton,
                    EditorToolBarButtonType.SelectDeactivatedEntityButton,
                    EditorToolBarButtonType.FindEntityButton,
                };

        public MyGuiIngameEditorControls(MyGuiScreenBase parentScreen) : base(parentScreen)
        {
            m_oreName = new StringBuilder();
            m_oreCount = new StringBuilder();

            m_lastUpdateTime = 0;
        }        

        protected override EditorToolBarButtonType[] GetEnabledButtons()
        {
            return MyGuiIngameEditorControls.m_enabledButtons;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            // resize and reposition treeview, because there are icons between filter textbox and treeview
            Vector2 oldTreeViewPosition = m_addObjectTreeView.GetPosition();
            Vector2 oldTreeViewSize = m_addObjectTreeView.GetSize().Value;
            
            Vector2 filterCheckboxPosition = m_filterTextbox.GetPosition() + 
                                             new Vector2(-m_filterTextbox.GetSize().Value.X / 2f, m_filterTextbox.GetSize().Value.Y) +
                                             new Vector2(CHECK_BOX_SIZE.X / 2f, MyGuiConstants.TOOLBAR_PADDING.Y);
            Vector4 checkboxColor = new Vector4(0.75f, 0.75f, 0.75f, 0.9f);

            m_addObjectTreeView.SetSize(oldTreeViewSize - new Vector2(0f, CHECK_BOX_SIZE.Y));
            m_addObjectTreeView.SetPosition(oldTreeViewPosition + new Vector2(0f, CHECK_BOX_SIZE.Y / 2f + MyGuiConstants.TOOLBAR_PADDING.Y));


            m_checkboxOnlyItemsOnWhichHasBlueprints = new MyGuiControlCheckbox(m_parentScreen, filterCheckboxPosition, CHECK_BOX_SIZE,
                MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), MyTextsWrapper.Get(MyTextsWrapperEnum.FilterOnlyItemsOnWhichHasBlueprintsToolTip), false, checkboxColor, true);
            m_editorControls.Add(m_checkboxOnlyItemsOnWhichHasBlueprints);

            m_checkboxOnlyItemsOnWhichHasOre = new MyGuiControlCheckbox(m_parentScreen, filterCheckboxPosition + new Vector2(0.005f + CHECK_BOX_SIZE.X, 0f), CHECK_BOX_SIZE,
                MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), MyTextsWrapper.Get(MyTextsWrapperEnum.FilterOnlyItemsOnWhichHasOreToolTip), false, checkboxColor, true);
            m_editorControls.Add(m_checkboxOnlyItemsOnWhichHasOre);

            m_checkboxOnlyItemsWhichHasBuild = new MyGuiControlCheckbox(m_parentScreen, filterCheckboxPosition + new Vector2(0.005f + CHECK_BOX_SIZE.X, 0f) * 2, CHECK_BOX_SIZE,
                MyGuiManager.GetCheckboxOffTexture(), MyGuiManager.GetCheckboxOnTexture(), MyTextsWrapper.Get(MyTextsWrapperEnum.FilterOnlyItemsWhichHasBuildToolTip), false, checkboxColor, true);
            m_editorControls.Add(m_checkboxOnlyItemsWhichHasBuild);

            foreach (MyGuiControlBase control in m_editorControls)
            {
                control.DrawWhilePaused = false;
            }
        }

        protected override void SetAddObjectVisibility(bool visible)
        {
            base.SetAddObjectVisibility(visible);
            m_checkboxOnlyItemsOnWhichHasBlueprints.Visible = visible;
            m_checkboxOnlyItemsOnWhichHasOre.Visible = visible;
            m_checkboxOnlyItemsWhichHasBuild.Visible = visible;
        }

        protected override void FillTreeView()
        {
            MyTexture2D icon = null;
            MyTexture2D expand = MyGuiManager.GetCollapseTexture();
            MyTexture2D collapse = MyGuiManager.GetExpandTexture();

            Vector2 smallIconSize = Vector2.Zero;
            Vector2 expandIconSize = MyGuiConstants.CHECKBOX_SIZE;

            CategoryTypesEnum[] beamsCategories = { CategoryTypesEnum.LARGE, CategoryTypesEnum.MEDIUM, CategoryTypesEnum.SMALL, CategoryTypesEnum.FRAME, CategoryTypesEnum.SHELVES };
            CategoryTypesEnum[] shellsCategories = { CategoryTypesEnum.PANELS, CategoryTypesEnum.CHAMBERS, CategoryTypesEnum.ARMORS };
            CategoryTypesEnum[] modulesCategories = { CategoryTypesEnum.FLIGHT, CategoryTypesEnum.SUPPLY, CategoryTypesEnum.LIFE_SUPPORT, CategoryTypesEnum.INDUSTRY, CategoryTypesEnum.WEAPONRY, CategoryTypesEnum.COMMUNICATIONS, CategoryTypesEnum.MANNED_OBJECTS};
            CategoryTypesEnum[] connectionsCategories = { CategoryTypesEnum.TUNNELS, CategoryTypesEnum.PIPES, CategoryTypesEnum.PASSAGES, CategoryTypesEnum.CABLES };
            CategoryTypesEnum[] detailssCategories = { CategoryTypesEnum.LIGHTS, CategoryTypesEnum.BILLBOARDS, CategoryTypesEnum.SIGNS, CategoryTypesEnum.TRAFFIC_SIGNS, CategoryTypesEnum.BARRELS, CategoryTypesEnum.SOUNDS, CategoryTypesEnum.OTHER };

            foreach (MyMwcObjectBuilder_Prefab_AppearanceEnum factionTexture in MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues)
            {
                var factionName = MyGuiPrefabHelpers.GetFactionName(factionTexture);
                var factionItem = m_addObjectTreeView.AddItem(factionName, icon, smallIconSize, expand, collapse, expandIconSize);
                factionItem.ToolTip = new MyToolTips(factionName);

                // BuildTypesEnum.BEAMS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeBeam, BuildTypesEnum.BEAMS, beamsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                // BuildTypesEnum.SHELLS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypePanel, BuildTypesEnum.SHELLS, shellsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                // BuildTypesEnum.MODULES
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeModule, BuildTypesEnum.MODULES, modulesCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                //BuildTypesEnum.CONNECTIONS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeConnection, BuildTypesEnum.CONNECTIONS, connectionsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                //BuildTypesEnum.DETAILS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeDetail, BuildTypesEnum.DETAILS, detailssCategories, icon, smallIconSize, expand, collapse, expandIconSize);
            }

            // SmallShipAmmo
            var smallShipAmmoItem = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.Ammo), icon, smallIconSize, expand, collapse, expandIconSize);
            AddSmallShipAmmoItems(smallShipAmmoItem);

            // SmallShipWeapons
            var smallShipWeaponsItem = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.Weapons), icon, smallIconSize, expand, collapse, expandIconSize);
            AddSmallShipWeaponsItems(smallShipWeaponsItem);                                    
        }

        private void AddSmallShipAmmoItems(MyTreeViewItem parentItem)
        {
            Vector2 itemSize = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(64, 64));
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Ammo_TypesEnumValues)
            {
                MyMwcObjectBuilder_SmallShip_Ammo ammoObjectBuilder = new MyMwcObjectBuilder_SmallShip_Ammo(enumValue);
                MyBuildingSpecification buildingSpecification = MyBuildingSpecifications.GetBuildingSpecification(ammoObjectBuilder);
                if (buildingSpecification != null)
                {                    
                    MyGuiSmallShipHelperAmmo smallShipAmmoHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int) enumValue) as MyGuiSmallShipHelperAmmo;
                    AddTreeViewItem(parentItem, smallShipAmmoHelper.Description, smallShipAmmoHelper.Icon, itemSize, MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(), MyGuiConstants.CHECKBOX_SIZE, ammoObjectBuilder, buildingSpecification);
                }
            }
        }

        private void AddSmallShipWeaponsItems(MyTreeViewItem parentItem)
        {
            Vector2 itemSize = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(64, 64));
            foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_Weapon_TypesEnumValues)
            {
                MyMwcObjectBuilder_SmallShip_Weapon weaponObjectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(enumValue);
                MyBuildingSpecification buildingSpecification = MyBuildingSpecifications.GetBuildingSpecification(weaponObjectBuilder);                
                MyGuiSmallShipHelperWeapon smallShipWeaponHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int) enumValue) as MyGuiSmallShipHelperWeapon;
                AddTreeViewItem(parentItem, smallShipWeaponHelper.Description, smallShipWeaponHelper.Icon, itemSize, MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(), MyGuiConstants.CHECKBOX_SIZE, weaponObjectBuilder, buildingSpecification);
            }
        }

        private void AddTreeViewItem(MyTreeViewItem parentItem, StringBuilder text, MyTexture2D icon, Vector2 iconSize, MyTexture2D expandIcon, MyTexture2D collapseIcon, Vector2 expandIconSize, MyMwcObjectBuilder_Base objectBuilder, MyBuildingSpecification buildingSpecification)
        {
            var item = parentItem.AddItem(new StringBuilder(), icon, iconSize, expandIcon, collapseIcon, expandIconSize);
            item.Tag = new MyObjectToBuild(objectBuilder, buildingSpecification, MyGameplayConstants.GetGameplayProperties(objectBuilder, MyEditor.Static.FoundationFactory.PrefabContainer.Faction).MaxAmount);
            item.ToolTip = new MyToolTips();
            item.ToolTip.AddToolTip(text, Color.White, 0.7f);
            item.ToolTip.AddToolTip(MyTextsWrapper.Get(MyTextsWrapperEnum.BuildingRequirements));
            item.ToolTip.AddToolTip(new StringBuilder(), Color.Green);
            item.ToolTip.AddToolTip(new StringBuilder(), Color.Red);
            item.IconTexts = new MyIconTexts();
            item.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM] = new MyColoredText(new StringBuilder(), MyGuiConstants.COLORED_TEXT_DEFAULT_COLOR, 0.8f);            
            item.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM] = new MyColoredText(new StringBuilder());
            item.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER] = new MyColoredText(new StringBuilder());
            item.Action = OnItemAction;
            item.RightClick = OnItemRighClick;            
            UpdateTreeViewItemState(item, MyTreeViewItemUpdateStateFlag.All);
        }

        protected override void AddPrefabItems(MyTreeViewItem parentItem, MyMwcObjectBuilder_Prefab_AppearanceEnum appearanceTextureEnum, BuildTypesEnum buildType, CategoryTypesEnum categoryType)
        {
            MyMwcLog.WriteLine("GAME AddPrefabItems - START");

            Vector2 itemSize = MyGuiConstants.CHECKBOX_SIZE * 3;
            foreach (MyMwcObjectBuilderTypeEnum enumValue in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(enumValue))
                {
                    MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(enumValue, prefabId);
                    MyGuiPrefabHelper prefabModuleHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(enumValue, prefabId) as MyGuiPrefabHelper;

                    if (config == null)
                        continue;

                    if (config.FactionSpecific.HasValue && config.FactionSpecific.Value != appearanceTextureEnum)
                        continue;

                    if (config.BuildType == buildType && config.CategoryType == categoryType && config.EnabledInEditor)
                    {
                        MyMwcObjectBuilder_PrefabBase prefabObjectBuilder = MyPrefabFactory.GetInstance().CreatePrefabObjectBuilder(enumValue, prefabId, appearanceTextureEnum);                        
                        MyBuildingSpecification buildingSpecification = MyBuildingSpecifications.GetBuildingSpecification(prefabObjectBuilder);
                        MyTexture2D previewTexture = MyGuiManager.GetPrefabPreviewTexture(enumValue, prefabId, appearanceTextureEnum);
                        AddTreeViewItem(parentItem, prefabModuleHelper.Description, previewTexture, itemSize, MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(), MyGuiConstants.CHECKBOX_SIZE, prefabObjectBuilder, buildingSpecification);
                    }
                }                
            }

            MyMwcLog.WriteLine("GAME AddPrefabItems - END");
        }        

        private void UpdateTreeViewItemsState(MyTreeViewItemUpdateStateFlag updateFlags)
        {            
            int itemsCount = m_addObjectTreeView.GetItemCount();
            for (int i = 0; i < itemsCount; i++)
            {
                UpdateTreeViewItemsState(m_addObjectTreeView.GetItem(i), updateFlags);
            }            
        }

        private void UpdateTreeViewItemsState(MyTreeViewItem treeViewItem, MyTreeViewItemUpdateStateFlag updateFlags)
        {
            int itemsCount = treeViewItem.GetItemCount();
            for (int i = 0; i < itemsCount; i++)
            {
                UpdateTreeViewItemsState(treeViewItem.GetItem(i), updateFlags);
            }

            if (treeViewItem.Tag != null && treeViewItem.Tag is MyObjectToBuild)
            {
                UpdateTreeViewItemState(treeViewItem, updateFlags);
            }
        }
        
        private void UpdateTreeViewItemState(MyTreeViewItem treeViewItem, MyTreeViewItemUpdateStateFlag updateFlags)
        {            
            if ((updateFlags & MyTreeViewItemUpdateStateFlag.BuildCountAndDragAndDrop) != 0)
            {
                UpdateTreeViewItemStateBuildCount(treeViewItem);
            }
            if ((updateFlags & MyTreeViewItemUpdateStateFlag.BuildingQueue) != 0)
            {
                UpdateTreeViewItemStateBuildingQueue(treeViewItem);
            }
            if ((updateFlags & MyTreeViewItemUpdateStateFlag.BuildingProgress) != 0)
            {
                UpdateTreeViewItemStateBuildingProgress(treeViewItem);
            }
            if ((updateFlags & MyTreeViewItemUpdateStateFlag.CanBuildAndRequirements) != 0)
            {
                UpdateTreeViewItemStateCanBuild(treeViewItem);
            }                
        }

        private MyTreeViewItem GetTreeViewItem(MyObjectToBuild objectToBuild)
        {
            MyTreeViewItem result = null;
            int itemsCount = m_addObjectTreeView.GetItemCount();
            for (int i = 0; i < itemsCount; i++)
            {
                result = GetTreeViewItem(m_addObjectTreeView.GetItem(i), objectToBuild);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        private MyTreeViewItem GetTreeViewItem(MyTreeViewItem treeViewItem, MyObjectToBuild objectToBuild)
        {
            MyTreeViewItem result = null;

            if (treeViewItem.Tag is MyObjectToBuild && ((MyObjectToBuild)treeViewItem.Tag).IsSame(objectToBuild))
            {
                result = treeViewItem;
            }
            else
            {
                int itemsCount = treeViewItem.GetItemCount();
                for (int i = 0; i < itemsCount; i++)
                {
                    result = GetTreeViewItem(treeViewItem.GetItem(i), objectToBuild);
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Updates build count and enable/disable drag and drop on item
        /// </summary>
        /// <param name="treeViewItem">Treeview item</param>
        private void UpdateTreeViewItemStateBuildCount(MyTreeViewItem treeViewItem)
        {
            MyObjectToBuild objectToBuild = treeViewItem.Tag as MyObjectToBuild;
            int buildObjectsCount = GetBuildObjectsCount(objectToBuild);
            
            treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM].Text.Clear();
            treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM].Text.Append(buildObjectsCount);

            // we can drag and drop only prefabs
            if (IsDragAndDropObject(objectToBuild))
            {
                if (buildObjectsCount > 0 && treeViewItem.DragDrop == null)
                {
                    treeViewItem.DragDrop = m_addObjectTreeViewdragDrop;
                }
                else if (buildObjectsCount == 0 && treeViewItem.DragDrop != null)
                {
                    treeViewItem.DragDrop = null;
                }
            }
        }

        /// <summary>
        /// Updates building queue count
        /// </summary>
        /// <param name="treeViewItem">Treeview item</param>
        private void UpdateTreeViewItemStateBuildingQueue(MyTreeViewItem treeViewItem)
        {
            MyObjectToBuild objectToBuild = treeViewItem.Tag as MyObjectToBuild;

            int objectsInBuildingQueue = GetBuildingObjectInBuildinQueueCount(objectToBuild);
            treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM].Text.Clear();
            if (objectsInBuildingQueue > 0)
            {
                treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM].Text.Append(objectsInBuildingQueue);
            }
        }

        /// <summary>
        /// Updates can build state and refresh building requirements
        /// </summary>
        /// <param name="treeViewItem">Treeview item</param>
        private void UpdateTreeViewItemStateCanBuild(MyTreeViewItem treeViewItem)
        {
            MyObjectToBuild objectToBuild = treeViewItem.Tag as MyObjectToBuild;
            treeViewItem.Enabled = GetBuildingObjectCanBuildState(objectToBuild);

            //treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Clear();
            ////if (MyObjectToBuild.HasSameObjectBuilders(MyEditor.Static.FoundationFactory.BuildingObject, objectToBuild))
            //if (MyEditor.Static.FoundationFactory.BuildingObject.IsSame(objectToBuild))
            //{
            //    treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Append(GetBuildingObjectState(objectToBuild));
            //    treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Append(PERCENTAGE);
            //}

            if (objectToBuild.BuildingSpecification.BuildingRequirements.Count > 0)
            {
                //int toolTipsCount = treeViewItem.ToolTip.GetToolTips().Count;
                //if (toolTipsCount > 1)
                //{
                //    treeViewItem.ToolTip.RemoveRange(1, toolTipsCount - 1);
                //}
                GetBuildingRequirementsDescription(treeViewItem.ToolTip, objectToBuild);
            }            
        }

        /// <summary>
        /// Updates building progress
        /// </summary>
        /// <param name="treeViewItem">Treeview item</param>
        private void UpdateTreeViewItemStateBuildingProgress(MyTreeViewItem treeViewItem)
        {
            MyObjectToBuild objectToBuild = treeViewItem.Tag as MyObjectToBuild;
            
            treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Clear();
            //if (MyObjectToBuild.HasSameObjectBuilders(MyEditor.Static.FoundationFactory.BuildingObject, objectToBuild))
            if (MyEditor.Static.FoundationFactory.BuildingObject != null && MyEditor.Static.FoundationFactory.BuildingObject.IsSame(objectToBuild))
            {
                treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Append(GetBuildingObjectState(objectToBuild));
                treeViewItem.IconTexts[MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER].Text.Append(PERCENTAGE);
            }            
        }

        private int GetBuildObjectsCount(MyObjectToBuild objectToBuild)
        {            
            //return MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems(objectToBuild.ObjectBuilder).Count;
            return GetInventoryItemsCount(objectToBuild.ObjectBuilder);
        }

        private int GetInventoryItemsCount(MyMwcObjectBuilder_Base objectBuilder)
        {
            return GetInventoryItems(objectBuilder).Count;
        }

        private List<MyInventoryItem> GetInventoryItems(MyMwcObjectBuilder_Base objectBuilder)
        {
            m_helperInventoryItems.Clear();
            if (!(objectBuilder is MyMwcObjectBuilder_PrefabBase))
            {
                MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems(ref m_helperInventoryItems, objectBuilder);
                //return MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems(objectBuilder);
                //return m_helperInventoryItems;
            }
            else
            {
                MyMwcObjectBuilder_PrefabBase prefabBuilder = objectBuilder as MyMwcObjectBuilder_PrefabBase;
                //List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
                foreach (MyInventoryItem inventoryItem in MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems())
                {
                    MyMwcObjectBuilder_Base inventoryItemBuilder = inventoryItem.GetInventoryItemObjectBuilder(false);
                    if (inventoryItemBuilder is MyMwcObjectBuilder_PrefabBase)
                    {
                        MyMwcObjectBuilder_PrefabBase inventoryPrefabBuilder = inventoryItemBuilder as MyMwcObjectBuilder_PrefabBase;
                        if (inventoryPrefabBuilder.GetObjectBuilderType() == prefabBuilder.GetObjectBuilderType() &&
                           inventoryPrefabBuilder.GetObjectBuilderId() == prefabBuilder.GetObjectBuilderId() &&
                           inventoryPrefabBuilder.FactionAppearance == prefabBuilder.FactionAppearance)
                        {
                            //inventoryItems.Add(inventoryItem);
                            m_helperInventoryItems.Add(inventoryItem);
                        }
                    }
                }
                //return inventoryItems;
            }
            return m_helperInventoryItems;
        }

        private int? GetBuildingObjectState(MyObjectToBuild objectToBuild)
        {
            if (MyEditor.Static.FoundationFactory.IsBuilding && 
                //MyObjectToBuild.HasSameObjectBuilders(MyEditor.Static.FoundationFactory.BuildingObject, objectToBuild))
                MyEditor.Static.FoundationFactory.BuildingObject.IsSame(objectToBuild))
            {
                return (int?)(MyEditor.Static.FoundationFactory.BuildingPercentageComplete * 100f);
            }
            else
            {
                return null;
            }
        }

        private int GetBuildingObjectInBuildinQueueCount(MyObjectToBuild objectToBuild)
        {
            //return MyEditor.Static.FoundationFactory.BuildingQueue.Count(x => MyObjectToBuild.HasSameObjectBuilders(x, objectToBuild));
            return MyEditor.Static.FoundationFactory.BuildingQueue.Count(x => x.IsSame(objectToBuild));
        }

        private void GetBuildingRequirementsDescription(MyToolTips requirementsDescription, MyObjectToBuild objectToBuild)
        {            
            StringBuilder meetRequirements = requirementsDescription.GetToolTips()[2].Text;
            StringBuilder notMeetRequirements = requirementsDescription.GetToolTips()[3].Text;

            meetRequirements.Clear();
            notMeetRequirements.Clear();            

            foreach (IMyBuildingRequirement buildingRequirement in objectToBuild.BuildingSpecification.BuildingRequirements)
            {
                if (buildingRequirement.Check(MyEditor.Static.FoundationFactory))
                {
                    MyMwcUtils.AppendStringBuilder(meetRequirements, buildingRequirement.GetDescription());
                    meetRequirements.AppendLine();
                }
                else
                {                    
                    MyMwcUtils.AppendStringBuilder(notMeetRequirements, buildingRequirement.GetDescription());
                    notMeetRequirements.AppendLine();
                }
            }       
            
            requirementsDescription.RecalculateSize();
        }

        private bool GetBuildingObjectCanBuildState(MyObjectToBuild objectToBuild)
        {
            return objectToBuild.BuildingSpecification.CanBuild(MyEditor.Static.FoundationFactory);
        }

        private void OnItemAction(object sender, EventArgs args)
        {
            MyTreeViewItem item = sender as MyTreeViewItem;
            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;
            
            if (objectToBuild != null)
            {
                if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.INSTANT_BUILDING))
                {
                    MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.AddInventoryItem(objectToBuild.ObjectBuilder, objectToBuild.Amount, true);
                    UpdateTreeViewItemState(item, MyTreeViewItemUpdateStateFlag.BuildCountAndDragAndDrop);
                }
                else if (objectToBuild.BuildingSpecification.CanBuild(MyEditor.Static.FoundationFactory))
                {
                    MyEditor.Static.FoundationFactory.AddToBuildingQueue(objectToBuild);
                    OnBuildingQueueChanged(objectToBuild, true);
                }
            }
        }

        private void OnItemRighClick(object sender, EventArgs args)
        {
            MyTreeViewItem item = sender as MyTreeViewItem;
            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;

            if (objectToBuild != null)
            {
                //if (MyEditor.Static.FoundationFactory.IsBuilding && MyObjectToBuild.HasSameObjectBuilders(MyEditor.Static.FoundationFactory.BuildingObject, objectToBuild))
                if (MyEditor.Static.FoundationFactory.IsBuilding && MyEditor.Static.FoundationFactory.BuildingObject.IsSame(objectToBuild))
                {
                    MyEditor.Static.FoundationFactory.CancelBuilding();
                }
                else
                {
                    //MyObjectToBuild objectToRemove = MyEditor.Static.FoundationFactory.BuildingQueue.FindLast(x => MyObjectToBuild.HasSameObjectBuilders(x, objectToBuild));
                    MyObjectToBuild objectToRemove = MyEditor.Static.FoundationFactory.BuildingQueue.FindLast(x => x.IsSame(objectToBuild));
                    if (objectToRemove != null)
                    {
                        MyEditor.Static.FoundationFactory.RemoveFromBuildingQueue(objectToRemove);
                    }
                }                
                OnBuildingQueueChanged(objectToBuild, false);                
            }
        }        

        private void AddInventoryItem(MyMwcObjectBuilder_Base objectBuilder, float amount)
        {
            MyEditor.Static.FoundationFactory.Player.Ship.Inventory.AddInventoryItem(objectBuilder, amount, false);
        }

        private void AddPrefab(MyMwcObjectBuilder_PrefabBase prefabBuilder)
        {
            MyEditor.Static.CreateFromObjectBuilder(prefabBuilder, Matrix.Identity, MyGuiManager.MouseCursorPosition);
        }

        private bool IsDragAndDropObject(MyObjectToBuild objectToBuild)
        {
            return objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.Prefab ||
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabLight ||
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabParticles ||
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabSound ||
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabKinematic ||
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabLargeShip || 
                   objectToBuild.ObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon;            
        }

        protected override void OnDragDrop(object sender, EventArgs ea)
        {
            // Return if drop is done on some control
            foreach (var control in m_editorControls)
            {
                if (control == m_addObjectTreeViewdragDrop)
                    continue;
                if (control.Visible && control.ContainsMouse())
                    return;
            }
            
            MyObjectToBuild objectToBuild = m_addObjectTreeViewdragDrop.DraggedItem.Tag as MyObjectToBuild;

            if (objectToBuild != null && IsDragAndDropObject(objectToBuild))
            {
                List<MyInventoryItem> prefabInventoryItems = GetInventoryItems(objectToBuild.ObjectBuilder);
                if(prefabInventoryItems.Count > 0)
                {
                    AddPrefab(objectToBuild.ObjectBuilder as MyMwcObjectBuilder_PrefabBase);
                    MyInventoryItem prefabInventoryItem = prefabInventoryItems[0];
                    MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.RemoveInventoryItem(prefabInventoryItem, true);                    
                }
            }            
        }

        private bool IsFilterChanged(MyPrefabSnapPoint myPrefabSnapPoint)
        {
            return myPrefabSnapPoint != m_activeSnapPointFilter ||
                   m_filterTextbox.Text != m_activeTextFilter ||
                   m_checkboxOnlyItemsOnWhichHasBlueprints.Checked != m_activeItemsOnWhichHasBlueprintsFilter ||
                   m_checkboxOnlyItemsOnWhichHasOre.Checked != m_activeItemsOnWhichHasOreFilter ||
                   m_checkboxOnlyItemsWhichHasBuild.Checked != m_activeItemsWhichHasBuildFilter;
        }

        private bool FilterResultByText(MyTreeViewItem item)
        {
            // Filter by text
            if (!string.IsNullOrEmpty(m_activeTextFilter))
            {
                if (item.ToolTip == null)
                {
                    return false;
                }

                var toolTips = item.ToolTip.GetToolTips();
                if (toolTips.Count == 0 || toolTips[0].Text.ToString().IndexOf(m_activeTextFilter, StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        private bool FilterResultByItemOnWhichHasBlueprints(MyTreeViewItem item)
        {
            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;
            if (m_activeItemsOnWhichHasBlueprintsFilter)
            {
                foreach (IMyBuildingRequirement buildingRequirement in objectToBuild.BuildingSpecification.BuildingRequirements)
                {
                    if(buildingRequirement is MyBuildingRequirementBlueprint)
                    {
                        MyBuildingRequirementBlueprint blueprintRequirement = buildingRequirement as MyBuildingRequirementBlueprint;
                        if (!blueprintRequirement.Check(MyEditor.Static.FoundationFactory))
                        {
                            return false;
                        }
                    }
                }
                return true;                
            }
            return true;
        }

        private bool FilterResultByItemOnWhichHasOre(MyTreeViewItem item)
        {
            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;
            if (m_activeItemsOnWhichHasOreFilter)
            {
                foreach (IMyBuildingRequirement buildingRequirement in objectToBuild.BuildingSpecification.BuildingRequirements)
                {
                    if (buildingRequirement is MyBuildingRequirementInventoryItem)
                    {
                        MyBuildingRequirementInventoryItem inventoryItemRequirement = buildingRequirement as MyBuildingRequirementInventoryItem;
                        if (inventoryItemRequirement.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.Ore &&
                            !inventoryItemRequirement.Check(MyEditor.Static.FoundationFactory))
                        {
                            return false;
                        }
                    }
                }
                return true;                                
            }
            return true;
        }

        private bool FilterResultByBuildItem(MyTreeViewItem item)
        {
            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;
            if (m_activeItemsWhichHasBuildFilter)
            {                
                //if (MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems(objectToBuild.ObjectBuilder).Count > 0)
                if (GetInventoryItemsCount(objectToBuild.ObjectBuilder) > 0)                
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool FilterResultByItemAndPrefabSnapPoint(MyTreeViewItem item)
        {
            if (!MyEditor.Static.SnapPointFilter || m_activeSnapPointFilter == null)
            {
                // Always pass if snap point filter is disabled or no snap point selected
                return true;
            }

            MyObjectToBuild objectToBuild = item.Tag as MyObjectToBuild;
            if (objectToBuild.ObjectBuilder is MyMwcObjectBuilder_PrefabBase)
            {
                var snapPoints = MyEditor.GetPrefabSnapPoints(objectToBuild.ObjectBuilder.GetObjectBuilderType(), objectToBuild.ObjectBuilder.GetObjectBuilderId().Value);
                return snapPoints.Exists(a => m_activeSnapPointFilter.CanAttachTo(a));                
            }

            return false;
        }

        protected override void FilterAddObjectTree(MyPrefabSnapPoint myPrefabSnapPoint)
        {                        
            if (IsFilterChanged(myPrefabSnapPoint))
            {
                FilterAddObjectTreePrivate(myPrefabSnapPoint);
            }
        }

        private void FilterAddObjectTreePrivate(MyPrefabSnapPoint myPrefabSnapPoint)
        {
            m_activeSnapPointFilter = myPrefabSnapPoint;
            m_activeTextFilter = m_filterTextbox.Text;
            m_activeItemsOnWhichHasBlueprintsFilter = m_checkboxOnlyItemsOnWhichHasBlueprints.Checked;
            m_activeItemsOnWhichHasOreFilter = m_checkboxOnlyItemsOnWhichHasOre.Checked;
            m_activeItemsWhichHasBuildFilter = m_checkboxOnlyItemsWhichHasBuild.Checked;

            m_addObjectTreeView.FilterTree(item =>
            {
                return FilterResultByText(item) &&
                       FilterResultByItemAndPrefabSnapPoint(item) &&
                       FilterResultByItemOnWhichHasBlueprints(item) &&
                       FilterResultByItemOnWhichHasOre(item) &&
                       FilterResultByBuildItem(item);
            });
        }

        public override void Update()
        {
            base.Update();
            if (MyEditor.Static.FoundationFactory != null && MyEditor.Static.FoundationFactory.IsBuilding)
            {
                m_lastUpdateTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_lastUpdateTime >= UPDATE_TREEVIEW_ITEMS_STATE)
                {
                    m_lastUpdateTime = 0;                    
                    MyTreeViewItem treeViewItem = GetTreeViewItem(MyEditor.Static.FoundationFactory.BuildingObject);
                    UpdateTreeViewItemState(treeViewItem, MyTreeViewItemUpdateStateFlag.BuildingProgress);                    
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            DrawTexts();
        }

        private void DrawTexts()
        {
            m_oreName.Clear();
            m_oreCount.Clear();
            foreach (MyMwcObjectBuilder_Ore_TypesEnum key in Enum.GetValues(typeof(MyMwcObjectBuilder_Ore_TypesEnum)))
            {
                int val = (int)MySession.PlayerShip.Inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.Ore, (int)key);
                if (val > 0)
                {
                    m_oreName.Append(MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)key).Description);
                    m_oreName.Append(ORE_TEXT_SEPARATOR);
                    m_oreName.AppendLine();

                    m_oreCount.Append(val);
                    m_oreCount.AppendLine();
                }
            }

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_oreName,
                            MyGuiManager.GetScreenTextRightTopPosition() - ORE_INFORMATION_RIGHT_OFFSET, MyGuiConstants.LABEL_TEXT_SCALE,
                            MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_oreCount,
                            MyGuiManager.GetScreenTextRightTopPosition(), MyGuiConstants.LABEL_TEXT_SCALE,
                            MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
        }

        public override void UpdateScreenSize()
        {
            base.UpdateScreenSize();

            Vector2 oldTreeViewPosition = m_addObjectTreeView.GetPosition();
            Vector2 oldTreeViewSize = m_addObjectTreeView.GetSize().Value;
            
            Vector2 filterCheckboxPosition = m_filterTextbox.GetPosition() +
                                             new Vector2(-m_filterTextbox.GetSize().Value.X / 2f, m_filterTextbox.GetSize().Value.Y) +
                                             new Vector2(CHECK_BOX_SIZE.X / 2f, MyGuiConstants.TOOLBAR_PADDING.Y);

            m_addObjectTreeView.SetSize(oldTreeViewSize - new Vector2(0f, CHECK_BOX_SIZE.Y));
            m_addObjectTreeView.SetPosition(oldTreeViewPosition + new Vector2(0f, CHECK_BOX_SIZE.Y / 2f + MyGuiConstants.TOOLBAR_PADDING.Y));

            m_checkboxOnlyItemsOnWhichHasBlueprints.SetPosition(filterCheckboxPosition);
            m_checkboxOnlyItemsOnWhichHasBlueprints.SetSize(CHECK_BOX_SIZE);
            m_checkboxOnlyItemsOnWhichHasOre.SetPosition(filterCheckboxPosition + new Vector2(0.005f + CHECK_BOX_SIZE.X, 0f));
            m_checkboxOnlyItemsOnWhichHasOre.SetSize(CHECK_BOX_SIZE);
            m_checkboxOnlyItemsWhichHasBuild.SetPosition(filterCheckboxPosition + new Vector2(0.005f + CHECK_BOX_SIZE.X, 0f) * 2);
            m_checkboxOnlyItemsWhichHasBuild.SetSize(CHECK_BOX_SIZE);
            
        }

        private void OnBuildingComplete(MyPrefabFoundationFactory sender, MyObjectToBuild buildObject)
        {
            MyTreeViewItemUpdateStateFlag updateFlags = MyTreeViewItemUpdateStateFlag.BuildCountAndDragAndDrop | MyTreeViewItemUpdateStateFlag.BuildingProgress;

            MyTreeViewItem treeViewItem = GetTreeViewItem(buildObject);
            UpdateTreeViewItemState(treeViewItem, updateFlags);

            if(sender.BuildingObject != null)
            {
                UpdateTreeViewItemState(GetTreeViewItem(sender.BuildingObject), MyTreeViewItemUpdateStateFlag.BuildingQueue);
            }
            FilterAddObjectTreePrivate(MyEditorGizmo.SelectedSnapPoint);
        }

        private void OnBuildingQueueChanged(MyObjectToBuild objectToBuild, bool added)
        {            
            MyTreeViewItemUpdateStateFlag updateFlags = MyTreeViewItemUpdateStateFlag.BuildingQueue;
            if(!added)
            {
                updateFlags = updateFlags | MyTreeViewItemUpdateStateFlag.BuildingProgress;
            }

            MyTreeViewItem treeViewItem = GetTreeViewItem(objectToBuild);
            UpdateTreeViewItemState(treeViewItem, updateFlags);

            UpdateTreeViewItemsState(MyTreeViewItemUpdateStateFlag.CanBuildAndRequirements);
        }

        private void OnFoundationFactoryInventoryContentChanged(MyInventory sender)
        {
            MyTreeViewItemUpdateStateFlag updateFlags = MyTreeViewItemUpdateStateFlag.BuildCountAndDragAndDrop | MyTreeViewItemUpdateStateFlag.CanBuildAndRequirements;
            
            UpdateTreeViewItemsState(updateFlags);
            FilterAddObjectTreePrivate(MyEditorGizmo.SelectedSnapPoint);
        }

        public override void LoadEditorData()
        {
            // add basic construction kit to checkpoint's inventory if there is no one
            if (!MySession.Static.Inventory.Contains(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit))
            {
                MySession.Static.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit, 1f, true, true);
            }

            base.LoadEditorData();
            UpdateTreeViewItemsState(MyTreeViewItemUpdateStateFlag.All);
            FilterAddObjectTreePrivate(MyEditorGizmo.SelectedSnapPoint);
            MyEditor.Static.FoundationFactory.BuildingComplete += OnBuildingComplete;            
            MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.OnInventoryContentChange += OnFoundationFactoryInventoryContentChanged;
            MySession.Static.Inventory.OnInventoryContentChange += OnFoundationFactoryInventoryContentChanged;
        }

        public override void UnloadEditorData()
        {
            base.UnloadEditorData();
            MyEditor.Static.FoundationFactory.BuildingComplete -= OnBuildingComplete;            
            MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.OnInventoryContentChange -= OnFoundationFactoryInventoryContentChanged;
            MySession.Static.Inventory.OnInventoryContentChange -= OnFoundationFactoryInventoryContentChanged;
        }
    }
}

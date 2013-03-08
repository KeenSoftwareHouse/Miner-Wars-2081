using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core.TreeView;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Models;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiGodEditorControls : MyGuiEditorControlsBase
    {
        // Helper class for TreeView GUI, stores data for prefab insertion
        class PrefabTag
        {
            public MyMwcObjectBuilderTypeEnum PrefabModule;
            public MyMwcObjectBuilder_Prefab_AppearanceEnum FactionAppearance;
            public CategoryTypesEnum CategoryType;
            public int PrefabId;

            public PrefabTag(MyMwcObjectBuilderTypeEnum prefabModule, int prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum factionAppearance, CategoryTypesEnum categoryTypes)
            {
                PrefabModule = prefabModule;
                PrefabId = prefabId;
                FactionAppearance = factionAppearance;
                CategoryType = categoryTypes;
            }
        }

        enum MyEditorTopTextInfoPropertiesEnum
        {
            PLAYER_START_POSITION,
            SECTOR_IDENTIFIER,
            WORLD_COLLIDING_OBJECTS,
            SELECTED_OBJECT_TYPE,
            SELECTED_OBJECT_HUD_LABEL_TEXT,
            SELECTED_OBJECT_POSITION,
            SELECTED_OBJECT_SIZE_IN_METERS,
            SELECTED_OBJECT_ORIENTATION_FORWARD,
            SELECTED_OBJECT_ORIENTATION_UP,
            SELECTED_VOXEL_MAP_REMAINING_EXPLOSIONS_FOR_SAVE,
            NUMBER_OF_ASTEROIDS_IN_SECTOR,
            NUMBER_OF_OBJECTS_IN_SECTOR,
            SESSION_TYPE,
            OBJECT_AXIS_ROTATION_AMOUNT,
            SELECTED_OBJECT_COLLISION_OBJECTS,
            SELECTED_VOXEL_HAND_SHAPES_INFO,
            SELECTED_CONTAINER_OBJECT_PREFABS_LIMIT,
            NUMBER_OF_EDITALE_OBJECTS,
            NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR,
            VOXEL_POSITION_INT,
            VOXEL_MATERIAL,
            VOXEL_FILE,
            ENTITY_ID,
            TRANSFORM_LOCKED,
        }

        enum MyEditorBottomTextInfoPropertiesEnum
        {
            SPECTATOR_POSITION,
            SPECTATOR_SPEED_INDICATOR,
            GRID_SCALE_INDICATOR,
            GIZMO_SPACE,
            GRID_SNAP,
            ROTATE_SNAP
        }

        enum MyEditorAddObjectType
        {
            ASTEROID,
            SMALLSHIP,
            SPAWNPOINT,
            WAYPOINT,
            DUMMYPOINT,
            FOUNDATION_FACTORY,
            CARGO_BOX,
            MYSTERIOUS_CUBE,
            MYSTERIOUS_CUBE2,
            MYSTERIOUS_CUBE3,
            INFLUENCE_SPHERE
        }

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
                    EditorToolBarButtonType.LoadSectorButton,
                    EditorToolBarButtonType.SaveSectorButton,
                    EditorToolBarButtonType.SunSettingsButton,
                    EditorToolBarButtonType.GroupsButton,
                    EditorToolBarButtonType.CopyToolButton,
                    EditorToolBarButtonType.OptionsButton,
                    EditorToolBarButtonType.EditProperties,
                    EditorToolBarButtonType.SelectAndHideButton,
                    EditorToolBarButtonType.ListWayPoints,
                    EditorToolBarButtonType.CorrectSnappedPrefabsButton,
                    EditorToolBarButtonType.SmallShipTemplates,
                    EditorToolBarButtonType.SelectDeactivatedEntityButton,
                    EditorToolBarButtonType.FindEntityButton,
                };

        private StringBuilder[] m_rightTopCorner_sb; //for writing information text in right top corner
        private StringBuilder[] m_rightBottomCorner_sb; //for writing information text in right bottom corner
        private StringBuilder m_prefabContainerEditingActiveText;
        private StringBuilder m_editingWayPointConnectionsText;
        private StringBuilder m_editingWayPointConnectionsPathSelectedText;
        private bool m_savedSnapPointFilter;

        public MyGuiGodEditorControls(MyGuiScreenBase parentScreen) : base(parentScreen)
        {
            m_rightTopCorner_sb = new StringBuilder[Enum.GetValues(typeof(MyEditorTopTextInfoPropertiesEnum)).Length];
            m_rightBottomCorner_sb = new StringBuilder[Enum.GetValues(typeof(MyEditorBottomTextInfoPropertiesEnum)).Length];

            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.PLAYER_START_POSITION] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SECTOR_IDENTIFIER] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_TYPE] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_HUD_LABEL_TEXT] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_POSITION] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_SIZE_IN_METERS] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_ORIENTATION_FORWARD] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_ORIENTATION_UP] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_VOXEL_MAP_REMAINING_EXPLOSIONS_FOR_SAVE] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ASTEROIDS_IN_SECTOR] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_CONTAINER_OBJECT_PREFABS_LIMIT] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SESSION_TYPE] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.OBJECT_AXIS_ROTATION_AMOUNT] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_COLLISION_OBJECTS] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_VOXEL_HAND_SHAPES_INFO] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.WORLD_COLLIDING_OBJECTS] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.ENTITY_ID] = new StringBuilder();
            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.TRANSFORM_LOCKED] = new StringBuilder();

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.SPECTATOR_POSITION] = new StringBuilder();
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.SPECTATOR_SPEED_INDICATOR] = new StringBuilder();
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SCALE_INDICATOR] = new StringBuilder();
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GIZMO_SPACE] = new StringBuilder();
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SNAP] = new StringBuilder();
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.ROTATE_SNAP] = new StringBuilder();

            m_prefabContainerEditingActiveText = MyTextsWrapper.Get(MyTextsWrapperEnum.EditingActiveContainer);
            m_editingWayPointConnectionsText = MyTextsWrapper.Get(MyTextsWrapperEnum.EditingWayPointConnections);
            m_editingWayPointConnectionsPathSelectedText = MyTextsWrapper.Get(MyTextsWrapperEnum.EditingWayPointConnectionsPathSelected);
        }

        protected override EditorToolBarButtonType[] GetEnabledButtons()
        {
            return MyGuiGodEditorControls.m_enabledButtons;
        }

        protected override void FillTreeView()
        {
            m_addObjectTreeView.ClearItems();

            MyTexture2D icon = null; // MyGuiManager.GetCheckboxTexture();
            MyTexture2D expand = MyGuiManager.GetCollapseTexture();
            MyTexture2D collapse = MyGuiManager.GetExpandTexture();

            Vector2 smallIconSize = Vector2.Zero; // MyGuiConstants.CHECKBOX_SIZE;
            Vector2 bigIconSize = MyGuiConstants.CHECKBOX_SIZE * 4;
            Vector2 expandIconSize = MyGuiConstants.CHECKBOX_SIZE;

            CategoryTypesEnum[] largeShipCategories = { CategoryTypesEnum.LARGE_SHIPS };
            CategoryTypesEnum[] beamsCategories = { CategoryTypesEnum.LARGE, CategoryTypesEnum.MEDIUM, CategoryTypesEnum.SMALL, CategoryTypesEnum.FRAME, CategoryTypesEnum.SHELVES };
            CategoryTypesEnum[] shellsCategories = { CategoryTypesEnum.PANELS, CategoryTypesEnum.CHAMBERS, CategoryTypesEnum.ARMORS };
            CategoryTypesEnum[] modulesCategories = { CategoryTypesEnum.FLIGHT, CategoryTypesEnum.SUPPLY, CategoryTypesEnum.LIFE_SUPPORT, CategoryTypesEnum.INDUSTRY, CategoryTypesEnum.WEAPONRY, CategoryTypesEnum.COMMUNICATIONS, /*CategoryTypesEnum.HANGARS,*/ CategoryTypesEnum.MANNED_OBJECTS };
            //CategoryTypesEnum[] connectionsCategories = { CategoryTypesEnum.TUNNELS, CategoryTypesEnum.PIPES, CategoryTypesEnum.PASSAGES, CategoryTypesEnum.DOORS, CategoryTypesEnum.DOOR_CASES, CategoryTypesEnum.CABLES };
            CategoryTypesEnum[] connectionsCategories = { CategoryTypesEnum.TUNNELS, CategoryTypesEnum.PIPES, CategoryTypesEnum.PASSAGES, CategoryTypesEnum.DOOR_CASES, CategoryTypesEnum.CABLES };
            CategoryTypesEnum[] detailsCategories = { CategoryTypesEnum.LIGHTS, CategoryTypesEnum.BILLBOARDS, CategoryTypesEnum.SIGNS, CategoryTypesEnum.TRAFFIC_SIGNS, CategoryTypesEnum.BARRELS, CategoryTypesEnum.SOUNDS, CategoryTypesEnum.OTHER };
            CategoryTypesEnum[] customCategories = { CategoryTypesEnum.OTHER, CategoryTypesEnum.FOUNDATION_FACTORY, CategoryTypesEnum.SECURITY_CONTROL_HUB, CategoryTypesEnum.BANK_NODE, CategoryTypesEnum.GENERATOR, CategoryTypesEnum.SCANNER, CategoryTypesEnum.ALARM, CategoryTypesEnum.CAMERA, CategoryTypesEnum.FLIGHT };
            
            // prefab container
            var prefabItem = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.PrefabContainer), icon, smallIconSize, expand, collapse, expandIconSize);
            prefabItem.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.AddPrefabTooltip));

            var gamePlayItem = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.GamePlay), icon, smallIconSize, expand, collapse, expandIconSize);
            gamePlayItem.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.AddGamePlayTooltip));

            // prefab container -> large ships
            var largeShipItem = prefabItem.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.Largeship), icon, smallIconSize, expand, collapse, expandIconSize);
            largeShipItem.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.Largeship));

            // large ships -> completes
            var largeShipComlpetes = largeShipItem.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.LargeShipComplets), icon, smallIconSize, expand, collapse, expandIconSize);
            largeShipComlpetes.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.LargeShipComplets));

            AddPrefabItems(largeShipComlpetes, MyMwcObjectBuilder_Prefab_AppearanceEnum.None, BuildTypesEnum.LARGE_SHIPS, CategoryTypesEnum.LARGE_SHIPS);

            //Add fourth reich largeships
            AddPrefabItems(largeShipComlpetes, MyMwcObjectBuilder_Prefab_AppearanceEnum.FourthReich, BuildTypesEnum.LARGE_SHIPS, CategoryTypesEnum.LARGE_SHIPS);

            //Add russian largeships
            AddPrefabItems(largeShipComlpetes, MyMwcObjectBuilder_Prefab_AppearanceEnum.Russian, BuildTypesEnum.LARGE_SHIPS, CategoryTypesEnum.LARGE_SHIPS);

            // large ships -> eac Mship parts
            var mShipItem = largeShipItem.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.Mship), icon, smallIconSize, expand, collapse, expandIconSize);
            mShipItem.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.Mship));
            AddPrefabItems(mShipItem, MyMwcObjectBuilder_Prefab_AppearanceEnum.Euroamerican, BuildTypesEnum.LARGE_SHIPS, CategoryTypesEnum.MSHIP);            

            // prefab container -> factions
            foreach (MyMwcObjectBuilder_Prefab_AppearanceEnum factionTexture in MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues)
            {
                var factionName = MyGuiPrefabHelpers.GetFactionName(factionTexture);
                var factionItem = prefabItem.AddItem(factionName, icon, smallIconSize, expand, collapse, expandIconSize);
                factionItem.ToolTip = new MyToolTips(factionName);

                // BuildTypesEnum.BEAMS)
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeBeam, BuildTypesEnum.BEAMS, beamsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                // BuildTypesEnum.SHELLS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypePanel, BuildTypesEnum.SHELLS, shellsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                // BuildTypesEnum.MODULES
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeModule, BuildTypesEnum.MODULES, modulesCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                //BuildTypesEnum.CONNECTIONS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeConnection, BuildTypesEnum.CONNECTIONS, connectionsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                //BuildTypesEnum.DETAILS
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeDetail, BuildTypesEnum.DETAILS, detailsCategories, icon, smallIconSize, expand, collapse, expandIconSize);
                //BuildTypesEnum.CUSTOM ... Gameplay
                AddPrefabType(factionItem, factionTexture, MyTextsWrapperEnum.buildTypeCustom, BuildTypesEnum.CUSTOM, customCategories, icon, smallIconSize, expand, collapse, expandIconSize);
            }            

            MyTreeViewItem asteroidItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.Asteroid, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.ASTEROID);

            MyTreeViewItem smallShipItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.SmallShip, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.SMALLSHIP);

            MyTreeViewItem spawnPointItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.SpawnPoint, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.SPAWNPOINT);

            var influenceSphereItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.InfluenceSphereContextMenuCaption, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.INFLUENCE_SPHERE);
            influenceSphereItem.Action = OnInfluenceSphere;

            MyTreeViewItem wayPointItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.WayPoint, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.WAYPOINT);
            MyTreeViewItem dummyPointItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.DummyPoint, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.DUMMYPOINT);
            MyTreeViewItem cargoBoxItem = AddBaseDragTreeItem(gamePlayItem, MyTextsWrapperEnum.CargoBox, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.CARGO_BOX);
            var mysteriousCubesItem = gamePlayItem.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.MysteriousCubes), icon, smallIconSize, expand, collapse, expandIconSize);
            mysteriousCubesItem.ToolTip = new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.AddMysteriousCubesTooltip));
            MyTreeViewItem mysteriousBoxItem = AddBaseDragTreeItem(mysteriousCubesItem, MyTextsWrapperEnum.MysteriousCube_01, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.MYSTERIOUS_CUBE);
            MyTreeViewItem mysteriousBoxItem2 = AddBaseDragTreeItem(mysteriousCubesItem, MyTextsWrapperEnum.MysteriousCube_02, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.MYSTERIOUS_CUBE2);
            MyTreeViewItem mysteriousBoxItem3 = AddBaseDragTreeItem(mysteriousCubesItem, MyTextsWrapperEnum.MysteriousCube_03, icon, expand, collapse, smallIconSize, expandIconSize, MyEditorAddObjectType.MYSTERIOUS_CUBE3);
            

            //var debrisItem = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(MyTextsWrapperEnum.Debris), icon, smallIconSize, expand, collapse, expandIconSize);
            //AddDebrisItems(debrisItem);

            asteroidItem.Action = OnAsteorid;
            smallShipItem.Action = OnSmallShip;
            spawnPointItem.Action = OnSpawnPoint;
            wayPointItem.Action = OnWayPoint;
            dummyPointItem.Action = OnDummyPoint;
            cargoBoxItem.Action = OnAddCargoBox;
            mysteriousBoxItem.Action = OnAddMysteriousCube;
            mysteriousBoxItem2.Action = OnAddMysteriousCube2;
            mysteriousBoxItem3.Action = OnAddMysteriousCube3;
            
            StringBuilder[] nodePath = 
            {
                MyTextsWrapper.Get(MyTextsWrapperEnum.PrefabContainer), 
                MyTextsWrapper.Get(MyTextsWrapperEnum.buildTypePanel),
                MyGuiPrefabHelpers.GetMyGuiCategoryTypeHelper(CategoryTypesEnum.CHAMBERS).Description
            };
            TryExpand(nodePath);
        }

        private void AddLargeShipItems(MyTreeViewItem parentItem)
        {
            Vector2 itemSize = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(64, 64));
            foreach (MyMwcObjectBuilder_LargeShip_TypesEnum enumValue in MyGuiLargeShipHelpers.MyMwcLargeShipTypesEnumValues)
            {
                if (enumValue != MyMwcObjectBuilder_LargeShip_TypesEnum.JEROMIE_INTERIOR_STATION && enumValue != MyMwcObjectBuilder_LargeShip_TypesEnum.CRUISER_SHIP)
                {
                    MyGuiLargeShipHelper largeShipHelper = MyGuiLargeShipHelpers.GetMyGuiLargeShipHelper(enumValue);
                    var item = parentItem.AddItem(new StringBuilder(), largeShipHelper.Icon, itemSize, MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(), MyGuiConstants.CHECKBOX_SIZE);
                    item.ToolTip = new MyToolTips(largeShipHelper.Description);
                    item.Tag = enumValue;
                    item.Action = OnAddLargeShip;
                    item.DragDrop = m_addObjectTreeViewdragDrop;
                }
            }
        }

        private void AddDebrisItems(MyTreeViewItem parentItem)
        {
            Vector2 itemSize = MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(64, 64));
            foreach (MyMwcObjectBuilder_SmallDebris_TypesEnum enumValue in MyGuiSmallDebrisHelpers.MyMwcSmallDebrisEnumValues)
            {
                MyGuiSmallDebrisHelper smallDebrisHelper = MyGuiSmallDebrisHelpers.GetMyGuiSmallDebrisHelper(enumValue);
                var item = parentItem.AddItem(new StringBuilder(), smallDebrisHelper.Icon, itemSize, MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(), MyGuiConstants.CHECKBOX_SIZE);
                item.ToolTip = new MyToolTips(smallDebrisHelper.Description);
                item.Tag = enumValue;
                item.Action = OnAddDebris;
                item.DragDrop = m_addObjectTreeViewdragDrop;
            }
        }

        private MyTreeViewItem AddBaseDragTreeItem(MyTextsWrapperEnum text, MyTexture2D icon, MyTexture2D expand, MyTexture2D collapse, Vector2 smallIconSize, Vector2 expandIconSize, object tag)
        {
            var item = m_addObjectTreeView.AddItem(MyTextsWrapper.Get(text), icon, smallIconSize, expand, collapse, expandIconSize);
            item.DragDrop = m_addObjectTreeViewdragDrop;
            item.Tag = tag;
            return item;            
        }

        private MyTreeViewItem AddBaseDragTreeItem(MyTreeViewItem parentItem, MyTextsWrapperEnum text, MyTexture2D icon, MyTexture2D expand, MyTexture2D collapse, Vector2 smallIconSize, Vector2 expandIconSize, object tag) 
        {
            var item = parentItem.AddItem(MyTextsWrapper.Get(text), icon, smallIconSize, expand, collapse, expandIconSize);
            item.DragDrop = m_addObjectTreeViewdragDrop;
            item.Tag = tag;
            return item;
        }

        protected override void AddPrefabItems(MyTreeViewItem parentItem, MyMwcObjectBuilder_Prefab_AppearanceEnum appearanceTextureEnum, BuildTypesEnum buildType, CategoryTypesEnum categoryType)
        {
            MyMwcLog.WriteLine("GOD AddPrefabItems - START");

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
                        MyTexture2D previewTexture = MyGuiManager.GetPrefabPreviewTexture(enumValue, prefabId, appearanceTextureEnum);
                        var item = parentItem.AddItem(new StringBuilder(), previewTexture, itemSize,
                                                      MyGuiManager.GetBlankTexture(), MyGuiManager.GetBlankTexture(),
                                                      MyGuiConstants.CHECKBOX_SIZE);
                        item.ToolTip = GetPrefabToolTip(prefabModuleHelper.Description, config);
                        //item.ToolTip = new MyToolTips(prefabModuleHelper.Description);
                        item.Tag = new PrefabTag(enumValue, prefabId, appearanceTextureEnum, categoryType);
                        item.Action = OnAddPrefab;
                        item.DragDrop = m_addObjectTreeViewdragDrop;
                    }                    
                }
            }            
            MyMwcLog.WriteLine("GOD AddPrefabItems - END");
        }

        private MyToolTips GetPrefabToolTip(StringBuilder prefabDescription, MyPrefabConfiguration prefabConfiguration)
        {
            StringBuilder retval = new StringBuilder(prefabDescription.ToString() + Environment.NewLine);
            /*  //this is loading time killer!!
            MyModel model;
            string BBsize = "";
            model = MyModels.GetModel(prefabConfiguration.ModelLod0Enum);
            if (model == null || !model.LoadedData)
            {
                model = MyModels.GetModelOnlyModelInfo(prefabConfiguration.ModelLod0Enum);
            }
            if (model != null)
            {
                retval.AppendLine("LOD0 asset: " + model.AssetName);
                retval.AppendLine("LOD0 number of triangles: " + model.ModelInfo.TrianglesCount.ToString());
                retval.AppendLine("LOD0 number of vertexes: " + model.ModelInfo.VerticesCount.ToString());
                BBsize = "Bounding box size: (" + model.ModelInfo.BoundingBoxSize.X + ", " + model.ModelInfo.BoundingBoxSize.Y + ", " + model.ModelInfo.BoundingBoxSize.Z + ")";
            }
            if (prefabConfiguration.ModelLod1Enum.HasValue)
            {
                model = MyModels.GetModel(prefabConfiguration.ModelLod1Enum.Value);
                if (model == null || !model.LoadedData)
                {
                    model = MyModels.GetModelOnlyModelInfo(prefabConfiguration.ModelLod1Enum.Value);
                }
                if (model != null)
                {
                    retval.AppendLine("LOD1 asset: " + model.AssetName);
                    retval.AppendLine("LOD1 number of triangles: " + model.ModelInfo.TrianglesCount.ToString());
                    retval.AppendLine("LOD1 number of vertexes: " + model.ModelInfo.VerticesCount.ToString());
                }
            }
            if (prefabConfiguration.ModelCollisionEnum.HasValue)
            {
                model = MyModels.GetModel(prefabConfiguration.ModelCollisionEnum.Value);
                if (model == null || !model.LoadedData)
                {
                    model = MyModels.GetModelOnlyModelInfo(prefabConfiguration.ModelCollisionEnum.Value);
                }
                if (model != null)
                {
                    retval.AppendLine("COL asset: " + model.AssetName);
                    retval.AppendLine("COL number of triangles: " + model.ModelInfo.TrianglesCount.ToString());
                    retval.AppendLine("COL number of vertexes: " + model.ModelInfo.VerticesCount.ToString());
                }
            }
            retval.Append(BBsize);
                  */

            return new MyToolTips(retval);
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

            var tag = m_addObjectTreeViewdragDrop.DraggedItem.Tag;
            if (tag is PrefabTag)
            {
                AddPrefab(tag as PrefabTag, MyGuiManager.MouseCursorPosition);
            }
            else if (tag is MyMwcObjectBuilder_LargeShip_TypesEnum?)
            {
                AddLargeShip(tag as MyMwcObjectBuilder_LargeShip_TypesEnum?, MyGuiManager.MouseCursorPosition);
            }
            else if (tag is MyMwcObjectBuilder_SmallDebris_TypesEnum?)
            {
                AddDebris(tag as MyMwcObjectBuilder_SmallDebris_TypesEnum?, MyGuiManager.MouseCursorPosition);
            }
            else if (tag is MyEditorAddObjectType)
            {
                switch ((MyEditorAddObjectType)tag)
                {
                    case MyEditorAddObjectType.ASTEROID:
                        AddAsteroid(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.SMALLSHIP:
                        AddSmallShip(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.SPAWNPOINT:
                        AddSpawnPoint(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.INFLUENCE_SPHERE:
                        AddInfluenceSphere(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.WAYPOINT:
                        AddWayPoint(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.DUMMYPOINT:
                        AddDummyPoint(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.CARGO_BOX:
                        AddCargoBox(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.MYSTERIOUS_CUBE:
                        AddMysteriousCube(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.MYSTERIOUS_CUBE2:
                        AddMysteriousCube2(MyGuiManager.MouseCursorPosition);
                        break;
                    case MyEditorAddObjectType.MYSTERIOUS_CUBE3:
                        AddMysteriousCube3(MyGuiManager.MouseCursorPosition);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #region Add Object Handlers

       private void OnAsteorid(object sender, EventArgs ea)
        {
            AddAsteroid(null);
        }

        private void OnSmallShip(object sender, EventArgs ea)
        {
            AddSmallShip(null);
        }

        private void OnWayPoint(object sender, EventArgs ea)
        {
            AddWayPoint(null);
        }

        private void OnSpawnPoint(object sender, EventArgs ea)
        {
            AddSpawnPoint(null);
        }

        private void OnInfluenceSphere(object sender, EventArgs ea)
        {
            AddInfluenceSphere(null);
        }

        private void OnAddPrefab(object sender, EventArgs ea)
        {
            AddPrefab((sender as MyTreeViewItem).Tag as PrefabTag);
        }

        private void OnAddLargeShip(object sender, EventArgs ea)
        {
            AddLargeShip((sender as MyTreeViewItem).Tag as MyMwcObjectBuilder_LargeShip_TypesEnum?);
        }

        private void OnAddDebris(object sender, EventArgs ea)
        {
            AddDebris((sender as MyTreeViewItem).Tag as MyMwcObjectBuilder_SmallDebris_TypesEnum?);
        }

        private void OnDummyPoint(object sender, EventArgs ea)
        {
            AddDummyPoint(null);
        }

        private void OnAddCargoBox(object sender, EventArgs ea) 
        {
            AddCargoBox(null);
        }

        private void OnAddMysteriousCube(object sender, EventArgs ea)
        {
            AddMysteriousCube(null);
        }

        private void OnAddMysteriousCube2(object sender, EventArgs ea)
        {
            AddMysteriousCube2(null);
        }

        private void OnAddMysteriousCube3(object sender, EventArgs ea)
        {
            AddMysteriousCube3(null);
        }

        #endregion

        private void AddPrefab(PrefabTag tag, Vector2? screenPosition = null)
        {
            if (tag == null)
            {
                return;
            }

            MyMwcObjectBuilder_PrefabBase prefabBuilder = null;
            MyPrefabContainer container = MyEditor.Static.GetEditedPrefabContainer();

            if (MyFakes.ENABLE_OBJECT_COUNTS_LIMITS)
			{
            //here test if we can even add entity
        	    if (MyEntities.GetObjectsCount() >= MyEditorConstants.MAX_EDITOR_ENTITIES_LIMIT)
    	        {
	                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxLargeShipsCountReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
            	    return;
          		}
            }
            prefabBuilder = MyPrefabFactory.GetInstance().CreatePrefabObjectBuilder(tag.PrefabModule, tag.PrefabId, tag.FactionAppearance);            

            if(container != null && prefabBuilder is MyMwcObjectBuilder_PrefabFoundationFactory && container.ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory))
            {
                // prefab container can contains only one foundation factory
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PrefabContainerAlreadyContainsFoundationFactory,
                        MyTextsWrapperEnum.YouCantAddFoundationFactory, MyTextsWrapperEnum.Ok, null));
                    return;                
            }

            if (container == null)
            {
                List<MyMwcObjectBuilder_PrefabBase> prefabBuilders = new List<MyMwcObjectBuilder_PrefabBase>();
                prefabBuilders.Add(prefabBuilder);
                MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = new MyMwcObjectBuilder_PrefabContainer(
                    null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabBuilders, MyClientServer.LoggedPlayer.GetUserId(),
                    MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000));
                MyEditor.Static.CreateFromObjectBuilder(prefabContainerBuilder, Matrix.Identity, screenPosition);
            }
            else
            {
                MyEditor.Static.CreateFromObjectBuilder(prefabBuilder, Matrix.Identity, screenPosition);
            }
        }

        private void AddLargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum? tag, Vector2? screenPosition = null)
        {
            if (!tag.HasValue)
            {
                return;
            }

            MyMwcObjectBuilder_LargeShip largeShipBuilder = new MyMwcObjectBuilder_LargeShip(tag.Value);
            MyEditor.Static.CreateFromObjectBuilder(largeShipBuilder, Matrix.Identity, screenPosition);
        }

        private void AddDebris(MyMwcObjectBuilder_SmallDebris_TypesEnum? tag, Vector2? screenPosition = null)
        {
            if (!tag.HasValue)
            {
                return;
            }

            MyMwcObjectBuilder_SmallDebris smallDebrisBuilder = new MyMwcObjectBuilder_SmallDebris(tag.Value, false, 10000);
            MyEditor.Static.CreateFromObjectBuilder(smallDebrisBuilder, Matrix.Identity, screenPosition);
        }

        private void AddDummyPoint(Vector2? screenPosition)
        {
            var dummyPointObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null) as MyMwcObjectBuilder_DummyPoint;
            Debug.Assert(dummyPointObjectBuilder != null);

            // set default size
            dummyPointObjectBuilder.Size = new Vector3(MyDummyPointConstants.DEFAULT_DUMMYPOINT_SIZE, MyDummyPointConstants.DEFAULT_DUMMYPOINT_SIZE, MyDummyPointConstants.DEFAULT_DUMMYPOINT_SIZE);

            MyEditor.Static.CreateFromObjectBuilder(dummyPointObjectBuilder, Matrix.Identity, screenPosition);
        }

        private void AddCargoBox(Vector2? screenPosition) 
        {
            MyMwcObjectBuilder_CargoBox cargoBoxObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type1) as MyMwcObjectBuilder_CargoBox;
            MyEditor.Static.CreateFromObjectBuilder(cargoBoxObjectBuilder, Matrix.Identity, screenPosition);
        }

        private void AddMysteriousCube(Vector2? screenPosition)
        {
            MyMwcObjectBuilder_MysteriousCube mysteriousCubeObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type1) as MyMwcObjectBuilder_MysteriousCube;
            MyEditor.Static.CreateFromObjectBuilder(mysteriousCubeObjectBuilder, Matrix.Identity, screenPosition);
        }

        private void AddMysteriousCube2(Vector2? screenPosition)
        {
            MyMwcObjectBuilder_MysteriousCube mysteriousCubeObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type2) as MyMwcObjectBuilder_MysteriousCube;
            MyEditor.Static.CreateFromObjectBuilder(mysteriousCubeObjectBuilder, Matrix.Identity, screenPosition);
        }

        private void AddMysteriousCube3(Vector2? screenPosition)
        {
            MyMwcObjectBuilder_MysteriousCube mysteriousCubeObjectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type3) as MyMwcObjectBuilder_MysteriousCube;
            MyEditor.Static.CreateFromObjectBuilder(mysteriousCubeObjectBuilder, Matrix.Identity, screenPosition);
        }

        //private void AddFoundationFactory(Vector2? screenPosition)
        //{            
        //    // we adding foundation factory, to existing and editing prefab container
        //    if (MyEditor.Static.GetEditedPrefabContainer() != null)
        //    {              
        //        // prefab container can contains only one foundation factory
        //        if(MyEditor.Static.GetEditedPrefabContainer().ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory))
        //        {
        //            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyTextsWrapperEnum.PrefabContainerAlreadyContainsFoundationFactory,
        //                MyTextsWrapperEnum.YouCantAddFoundationFactory, MyTextsWrapperEnum.Ok, null));
        //            return;
        //        }
                
        //        MyPrefabConfiguration ffConfiguration = MyPrefabConstants.GetPrefabConfiguration(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, 0);
        //        MyMwcObjectBuilder_Base builder = MyPrefabFactory.GetInstance().CreatePrefabObjectBuilder()
        //        MyPrefabFoundationFactory foundationFactory = MyPrefabFactory.GetInstance().CreatePrefab(null, MyEditor.Static.GetEditedPrefabContainer(), )

        //        foundationFactory.Init(null, foundationFactoryObjectBuilder,);
        //        MyEntities.Add(foundationFactory);
        //    }
        //    // we adding foundation factory as new entity a we want create new prefab container for it
        //    else
        //    {
        //        MyMwcObjectBuilder_PrefabContainer prefabContainerObjectBuilder = 
        //            new MyMwcObjectBuilder_PrefabContainer(null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, new List<MyMwcObjectBuilder_PrefabBase>(),
        //                MyClientServer.LoggedPlayer.GetUserId(), MyMwcObjectBuilder_FactionEnum.Euroamerican, 
        //                new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), MyInventory.DEFAULT_MAX_ITEMS));
        //        foundationFactoryObjectBuilder.PrefabContainer = prefabContainerObjectBuilder;
        //        foundationFactory.Init(null, foundationFactoryObjectBuilder, Matrix.Identity);
        //        MyEntities.Add(foundationFactory);

        //        float distanceFromCamera = foundationFactory.WorldVolume.Radius * 2;                
        //        Vector3 newPosition = MySpectator.Position + distanceFromCamera * MySpectator.Orientation;
        //        foundationFactory.MoveAndRotate(newPosition, foundationFactory.GetWorldRotation());                
        //    }
        //}

        private void AddAsteroid(Vector2? screenPosition)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorAsteroid(screenPosition));
        }

        private void AddSmallShip(Vector2? screenPosition)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorSmallShip(screenPosition));
        }

        private void AddWayPoint(Vector2? screenPosition)
        {
            // display waypoints if they were hidden
            // adding an entity will make its type selectable
            if (!MyEntities.IsTypeSelectable(typeof(MyWayPoint)))
            {
                MyEntities.SetTypeHidden(typeof(MyWayPoint), false);
                MyEntities.SetTypeSelectable(typeof(MyWayPoint), true);
            }

            var waypoint = MyWayPointGraph.CreateWaypoint(Vector3.Zero, null);
            MyEditorGizmo.ClearSelection();
            MyEditorGizmo.AddEntityToSelection(waypoint);
            float distanceFromCamera = waypoint.WorldVolume.Radius * 2;
            waypoint.MoveAndRotate(MySpectator.Position + distanceFromCamera * MySpectator.Orientation, waypoint.GetWorldRotation());
        }

        private void AddSpawnPoint(Vector2? screenPosition)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorSpawnPoint());
        }

        private void AddInfluenceSphere(Vector2? screenPosition)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorInfluenceSphere(screenPosition));
        }


        protected override void FilterAddObjectTree(MyPrefabSnapPoint myPrefabSnapPoint)
        {
            if (myPrefabSnapPoint != m_activeSnapPointFilter || m_filterTextbox.Text != m_activeTextFilter || MyEditor.Static.SnapPointFilter != m_savedSnapPointFilter/* || MyEditor.GetCurrentState() ==  MyEditorStateEnum.SELECTED_WAYPOINT*/)
            {
                m_activeSnapPointFilter = myPrefabSnapPoint;
                m_activeTextFilter = m_filterTextbox.Text;
                m_savedSnapPointFilter = MyEditor.Static.SnapPointFilter;

                m_addObjectTreeView.FilterTree(item =>
                {
                    /*
                    // filter by waypoint
                    if(MyEditor.GetCurrentState() ==  MyEditorStateEnum.SELECTED_WAYPOINT){
                        if( string.Compare(item.Text.ToString(), "WayPoint") == 0)
                            return true;
                        else
                            return false;
                    }
                    */
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

                    // Filter by snap point
                    if (!MyEditor.Static.SnapPointFilter || myPrefabSnapPoint == null)
                    {
                        // Always pass if snap point filter is disabled or no snap point selected
                        return true;
                    }

                    var tag = item.Tag as PrefabTag;
                    if (tag != null)
                    {
                        var snapPoints = MyEditor.GetPrefabSnapPoints(tag.PrefabModule, tag.PrefabId);
                        return snapPoints.Exists(a => myPrefabSnapPoint.CanAttachTo(a));
                    }

                    return false;
                });
            }
        }

        public override void Draw()
        {
            if (MyEditor.EnableTextsDrawing)
            {
                DrawTexts();
            }
            base.Draw();
        }

        private void ClearTexts()
        {
            foreach (StringBuilder sb in m_rightTopCorner_sb)
            {
                MyMwcUtils.ClearStringBuilder(sb);
            }

            foreach (StringBuilder sb in m_rightBottomCorner_sb)
            {
                MyMwcUtils.ClearStringBuilder(sb);
            }
        }

        private void DrawTexts()
        {
            ClearTexts();

            #region General info

            //this can help players to always see, if editing story or mmo or sandbox sector
            // add text here about max entities 
            // add text here about max prefab child count
            if (MyGuiScreenGamePlay.Static.GetSessionType().HasValue)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SESSION_TYPE].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SessionType)).
                    Append(MyGuiScreenGamePlay.Static.GetSessionTypeFriendlyName());
            }

            if (MySession.PlayerShip != null)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.PLAYER_START_POSITION].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.PlayerStartPosition)).
                    Append(MyUtils.GetFormatedVector3(MySession.PlayerShip.WorldMatrix.Translation, 3));
            }

            m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SECTOR_IDENTIFIER].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SectorIdentifier)).
                Append(MyGuiScreenGamePlay.Static.GetSectorIdentifier().ToString());

            if (MyVoxelMaps.GetVoxelMapsCount() > 0)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ASTEROIDS_IN_SECTOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.NumberOfAsteroidsInSector));
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ASTEROIDS_IN_SECTOR].AppendInt32(MyVoxelMaps.GetVoxelMapsCount());                
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.NumberOfVoxelShapesInSector));
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR].Append(" : ");                
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR].AppendInt32(MyVoxelMaps.GetVoxelShapesCount());
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR].Append(" / ");
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_ALL_VOXEL_SHAPES_IN_SECTOR].AppendInt32(MyVoxelConstants.MAX_VOXEL_HAND_SHAPES_COUNT);                
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_VOXEL_MAP_REMAINING_EXPLOSIONS_FOR_SAVE].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedVoxelMapRemainingSaveExplosions)).
                                        Append(MyVoxelMaps.GetRemainingVoxelHandShapes());
                //TODO number of voxel shapes
            }

            if (MyEntities.GetObjectsCount() > 0)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.NumberOfObjectsInSector));
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].AppendInt32(MyEntities.GetEditorObjectsCount());
                //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].Append(" : ");
                //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].Append(MyEntities.GetObjectsCount().ToString());
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].Append(" / ");
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR].Append(MyEditorConstants.MAX_EDITOR_ENTITIES_LIMIT.ToString());
            }


            //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_CONTAINER_OBJECT_PREFABS_LIMIT] = new StringBuilder();
            MyPrefabContainer container = MyEditor.Static.GetEditedPrefabContainer();
            if (container != null)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.ObjectsInContainer));
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS].Append(" : ");
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS].Append(container.GetWorkingPrefabsCount().ToString());
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS].Append(" / ");
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS].Append(MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER.ToString());
            }

            if (MyEditor.TransformLocked)
            {
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.TRANSFORM_LOCKED].Append("Transform locked");
            }

            // show significant info at the bottom right corner
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.SPECTATOR_POSITION].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SpectatorPosition)).
                Append(MyUtils.GetFormatedVector3(MySpectator.Position, 3));

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.SPECTATOR_SPEED_INDICATOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SpectatorSpeedIndicator)).
                Append(MySpectator.SpeedMode);

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SCALE_INDICATOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.GridScaleIndicator));
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SCALE_INDICATOR].Append(MyEditorGrid.GetGridStepInMeters());
            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SCALE_INDICATOR].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.GridInMeters));

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GIZMO_SPACE].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.GizmoSpaceEditorText)).
                Append(MyEditorGizmo.ActiveSpace == TransformSpace.WORLD ? MyTextsWrapper.Get(MyTextsWrapperEnum.GizmoSpaceWorld) : MyTextsWrapper.Get(MyTextsWrapperEnum.GizmoSpaceLocal));

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.GRID_SNAP].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.GridSnapEditorText)).
                Append(MyEditorGizmo.SnapEnabled ? MyTextsWrapper.Get(MyTextsWrapperEnum.On) : MyTextsWrapper.Get(MyTextsWrapperEnum.Off));


            StringBuilder rotateSnapping = null;
            switch (MyEditorGizmo.ActiveRotateSnapping)
            {
                case RotateSnapping.NONE:
                    rotateSnapping = MyTextsWrapper.Get(MyTextsWrapperEnum.SnapOff);
                    break;
                case RotateSnapping.FIVE_DEGREES:
                    rotateSnapping = MyTextsWrapper.Get(MyTextsWrapperEnum.Snap5Deg);
                    break;
                case RotateSnapping.FIFTEEN_DEGREES:
                    rotateSnapping = MyTextsWrapper.Get(MyTextsWrapperEnum.Snap15Deg);
                    break;
                case RotateSnapping.FORTYFIVE_DEGREES:
                    rotateSnapping = MyTextsWrapper.Get(MyTextsWrapperEnum.Snap45Degs);
                    break;
                case RotateSnapping.NINETY_DEGREES:
                    rotateSnapping = MyTextsWrapper.Get(MyTextsWrapperEnum.Snap90Deg);
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            m_rightBottomCorner_sb[(int)MyEditorBottomTextInfoPropertiesEnum.ROTATE_SNAP].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.RotateSnapping)).
                Append(rotateSnapping);
           
            if (MyEditor.Static.CollidingElements.Count > 0)
            {

                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.WORLD_COLLIDING_OBJECTS].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.CollidingObjects));
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.WORLD_COLLIDING_OBJECTS].Append(" : ");
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.WORLD_COLLIDING_OBJECTS].Append(MyEditor.Static.CollidingElements.Count.ToString());

            }


            #endregion

            // Display for selected object(one)
            if (MyEditorGizmo.IsOnlyOneEntitySelected())
            {
                #region One phys object text info
                MyEntity entity = MyEditorGizmo.GetFirstSelected();

                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_TYPE].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectType)).
                    Append(entity.GetFriendlyName());

                if (!string.IsNullOrEmpty(entity.DisplayName))
                {
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_HUD_LABEL_TEXT].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectHudLabelText)).
                        Append(entity.DisplayName);
                }


                if (MyEditor.Static.CheckForEntityChildsCollisions(entity))
                {
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_COLLISION_OBJECTS].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.ObjectInCollisionWith));
                }
                if (entity is MyVoxelMap)
                {
                    var v = entity as MyVoxelMap;
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Voxel));
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append(" : ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append(MyGuiAsteroidHelpers.GetMyGuiVoxelFileHelper(v.GetVoxelFile()).Description);
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append(" ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append("(id: ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append((int)v.GetVoxelFile());
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Append(")");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_FILE].Replace(Environment.NewLine, " ");

                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.AsteroidMaterial));
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append(" : ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append(MyGuiAsteroidHelpers.GetMyGuiVoxelMaterialHelper(v.BaseMaterial).Description);
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append(" ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append("(id: ");
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append((int)v.BaseMaterial);
                    //m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_MATERIAL].Append(")");

                    MyMwcVector3Short intPosition = MyVoxelMaps.GetVoxelCenterCoordinateFromMeters(v.PositionLeftBottomCorner);
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Voxel));
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT].Append(" ");
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Position));
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT].Append(" : ");
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.VOXEL_POSITION_INT].AppendFormat("{{X: {0} Y: {1} Z: {2}}}", intPosition.X, intPosition.Y, intPosition.Z);
                }

                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_POSITION].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectPosition)).
                    Append(MyUtils.GetFormatedVector3(entity.GetPosition(), 3));

                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_ORIENTATION_FORWARD].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectOrientationForward)).
                   Append(MyUtils.GetFormatedVector3(entity.GetWorldRotation().Forward, 3));

                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_ORIENTATION_UP].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectOrientationUp)).
                   Append(MyUtils.GetFormatedVector3(entity.GetWorldRotation().Up, 3));

                if (entity is MyVoxelMap)
                {
                    MyVoxelMap voxelMap = (MyVoxelMap)entity;

                    //size of the voxel map in meters
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_SIZE_IN_METERS].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectSizeInMeters)).
                        Append(MyUtils.GetFormatedVector3(voxelMap.SizeInMetres, 3));                    
                }
                else
                {
                    //size of the other objects in meters
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_SIZE_IN_METERS].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectSizeInMeters)).
                        Append(MyUtils.GetFormatedVector3(entity.LocalAABB.Size(), 3));
                }

                // entity Id
                if (entity.EntityId != null) 
                {
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.ENTITY_ID].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.EntityId)).
                        Append(entity.EntityId.Value.NumericValue);
                }
                #endregion
            }
            else if (MyEditorGizmo.IsMoreThanOneEntitySelected())
            {
                // In case of multiple objects selected, display center position of gizmo
                m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_POSITION].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SelectedObjectPosition)).
                                    Append(MyUtils.GetFormatedVector3(MyEditorGizmo.Position, 3));
            }

            if (MyEditorGizmo.IsAnyEntitySelected())
            {
                if (MyEditorGizmo.TransformationActive && MyEditorGizmo.ActiveMode == GizmoMode.ROTATE)
                {
                    m_rightTopCorner_sb[(int)MyEditorTopTextInfoPropertiesEnum.OBJECT_AXIS_ROTATION_AMOUNT].Append(MyTextsWrapper.Get(MyTextsWrapperEnum.EditorObjectAxisRotationAmount)).
                   Append(MyEditorGizmo.RotationAmountInDegrees);
                }
            }

            #region Draw Strings

            float TEXT_OFFSET_Y = 0.018f;

            int topAlignementCounter = 0;

            Vector2 rightTop = MyGuiManager.GetScreenTextRightTopPosition() + new Vector2(0, MyGuiConstants.TOOLBAR_BUTTON_SIZE.Y + MyGuiConstants.TOOLBAR_PADDING.Y);

            for (int i = 0; i < m_rightTopCorner_sb.Length; i++)
            {
                if (m_rightTopCorner_sb[i].Length > 0)
                {
                    //test for red flashing
                    if (
                        i == (int)MyEditorTopTextInfoPropertiesEnum.WORLD_COLLIDING_OBJECTS ||
                        i == (int)MyEditorTopTextInfoPropertiesEnum.SELECTED_OBJECT_COLLISION_OBJECTS ||                        
                        (i == (int)MyEditorTopTextInfoPropertiesEnum.SELECTED_VOXEL_HAND_SHAPES_INFO && MyEditorGizmo.GetFirstSelected() != null && MyVoxelMaps.GetRemainingVoxelHandShapes() == 0) ||
                        (i == (int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_OBJECTS_IN_SECTOR && MyEntities.GetEditorObjectsCount() >= MyEditorConstants.MAX_EDITOR_ENTITIES_LIMIT) ||
                        (i == (int)MyEditorTopTextInfoPropertiesEnum.NUMBER_OF_EDITALE_OBJECTS && container != null && container.GetWorkingPrefabsCount() >= MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER)
                        )
                    {
                        if (MyMinerGame.TotalTimeInMilliseconds % 250 > 125)
                        {
                            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_rightTopCorner_sb[i],
                                rightTop + new Vector2(0, topAlignementCounter * TEXT_OFFSET_Y), MyGuiConstants.EDITOR_LABEL_TEXT_SCALE * 0.7f,
                                Color.Red,
                                MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
                        }
                    }
                    else
                    {
                        MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_rightTopCorner_sb[i],
                            rightTop + new Vector2(0, topAlignementCounter * TEXT_OFFSET_Y), MyGuiConstants.EDITOR_LABEL_TEXT_SCALE * 0.7f,
                            MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
                    }
                    topAlignementCounter++;
                }
            }

            int bottomAlignementCounter = 0;
            for (int i = 0; i < m_rightBottomCorner_sb.Length; i++)
            {
                if (m_rightBottomCorner_sb[i].Length > 0)
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_rightBottomCorner_sb[i],
                        MyGuiManager.GetScreenTextRightBottomPosition() + new Vector2(0, -(bottomAlignementCounter * TEXT_OFFSET_Y)), MyGuiConstants.EDITOR_LABEL_TEXT_SCALE * 0.65f,
                        MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
                    bottomAlignementCounter++;
                }
            }

            if (MyEditor.Static.IsEditingPrefabContainer())
            {
                Vector4 color = MyGuiConstants.LABEL_TEXT_COLOR;
                float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % 480;
                if (timeBlic > 250) timeBlic = 480 - timeBlic;
                timeBlic = MathHelper.Clamp(1 - timeBlic / 250, 0, 1);
                color.W = MathHelper.Lerp(0.3f, 1f, timeBlic);

                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_prefabContainerEditingActiveText,
                        (MyGuiManager.GetScreenTextLeftBottomPosition() + MyGuiManager.GetScreenTextRightBottomPosition()) / 2, MyGuiConstants.EDITOR_LABEL_TEXT_SCALE, new Color(color), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            }

            // waypoint help string
            if (MyEditorGizmo.SelectedEntities.Find(a => a is MyWayPoint) != null)
            {
                StringBuilder waypointHelp = new StringBuilder("");
                if (MyWayPointGraph.SelectedPath == null)
                {
                    waypointHelp.Append(m_editingWayPointConnectionsText);
                }
                else
                {
                    waypointHelp.AppendFormat(m_editingWayPointConnectionsPathSelectedText.ToString(), MyWayPointGraph.SelectedPath.Name);
                }
                    
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), waypointHelp,
                    ((MyGuiManager.GetScreenTextLeftBottomPosition() + MyGuiManager.GetScreenTextRightBottomPosition()) / 2) - new Vector2(0.0f, 0.075f), MyGuiConstants.EDITOR_LABEL_TEXT_SCALE,
                    new Color(MyGuiConstants.LABEL_TEXT_COLOR), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP
                );
            }

            #endregion
        }

        public override void Update()
        {
            base.Update();
        }        
    }
}

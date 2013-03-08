using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Editor;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    // This is enumeration of values, that should be used to decide, what is the action mapped to chosen context menu item
    public enum MyGuiContextMenuItemActionType
    {
        SELECT_ALL_OBJECTS,
        ADJUST_GRID,
        EXIT_TO_MAIN_MENU,
        ADD_OBJECT,
        CLEAR_WHOLE_SECTOR,
        SAVE_SECTOR,
        LOAD_SECTOR,
        ATTACH_TO_CAMERA,
        COPY_SELECTED,
        EDIT_SELECTED,
        RESET_ROTATION,
        SWITCH_GIZMO_SPACE,
        SWITCH_GIZMO_MODE,
        SAVE_ASTEROID_TO_FILE,
        RESET_ASTEROID,
        CLEAR_ASTEROID_MATERIALS,
        ENTER_PREFAB_CONTAINER,
        DETACH_FROM_CAMERA,
        SUN_SETTINGS,
        FOG_SETTINGS,
        SAVE_ASTEROIDS,
        ENTER_VOXEL_HAND,
        EXIT_VOXEL_HAND,
        EDIT_VOXEL_HAND,
        OPTIONS,
        EXIT_SELECTED,
        EXIT_EDITING_MODE,
        TOGGLE_SNAP_POINTS,
        EDIT_PROPERTIES,
        CONNECT_WAYPOINTS,
        DISCONNECT_WAYPOINTS,
        CORRECT_SNAPPED_PREFABS,
    }

    static class MyGuiContextMenuHelpers
    {

        static Dictionary<MyEditorStateEnum, MyGuiContextMenuHelper> m_editorContextMenuHelpers = new Dictionary<MyEditorStateEnum, MyGuiContextMenuHelper>();

        public static MyGuiContextMenuHelper GetEditorContextMenuHelper(MyEditorStateEnum stateEnum)
        {
            MyGuiContextMenuHelper ret;
            if (m_editorContextMenuHelpers.TryGetValue(stateEnum, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_editorContextMenuHelpers.Clear();
        }

        public static void LoadDefaultEditorContent()
        {
            MyMwcLog.WriteLine("LoadDefaultEditorContent()");

            m_editorContextMenuHelpers.Clear();

            #region Editor Context Menu Items


            // General context menu items
            List<MyGuiContextMenuItemHelper> generalMenuItems = new List<MyGuiContextMenuItemHelper>();
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SelectAllObjects, MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS, true, MyTextsWrapperEnum.SelectAllObjectsTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ClearWholeSector, MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR, true, MyTextsWrapperEnum.ClearWholeSectorTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AdjustGrid, MyGuiContextMenuItemActionType.ADJUST_GRID, false, MyTextsWrapperEnum.AdjustGridTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, false, MyTextsWrapperEnum.LoadSectorTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SunSettings, MyGuiContextMenuItemActionType.SUN_SETTINGS, false, MyTextsWrapperEnum.SunSettingsTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.FogSettings, MyGuiContextMenuItemActionType.FOG_SETTINGS, false, MyTextsWrapperEnum.FogSettingsTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveAsteroids, MyGuiContextMenuItemActionType.SAVE_ASTEROIDS, false, MyTextsWrapperEnum.SaveAsteroidsTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.Options, MyGuiContextMenuItemActionType.OPTIONS, true, MyTextsWrapperEnum.Options));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitToMainMenu, MyGuiContextMenuItemActionType.EXIT_TO_MAIN_MENU, true, MyTextsWrapperEnum.ExitToMainMenu));

            // Items that will be included for all types of selected objects
            List<MyGuiContextMenuItemHelper> allTypesMenuItems = new List<MyGuiContextMenuItemHelper>();
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AttachToCamera, MyGuiContextMenuItemActionType.ATTACH_TO_CAMERA, true, MyTextsWrapperEnum.AttachToCameraTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditSelected, MyGuiContextMenuItemActionType.EDIT_SELECTED, true, MyTextsWrapperEnum.EditSelectedTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SwitchGizmoSpace, MyGuiContextMenuItemActionType.SWITCH_GIZMO_SPACE, true, MyTextsWrapperEnum.SwitchGizmoSpaceTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SwitchGizmoMode, MyGuiContextMenuItemActionType.SWITCH_GIZMO_MODE, true, MyTextsWrapperEnum.SwitchGizmoModeTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.CopySelected, MyGuiContextMenuItemActionType.COPY_SELECTED, false, MyTextsWrapperEnum.CopySelectedTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.EXIT_SELECTED, true, null));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.LoadSectorTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ConnectWaypoints, MyGuiContextMenuItemActionType.CONNECT_WAYPOINTS, true, MyTextsWrapperEnum.ConnectWaypointsTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.DisconnectWaypoints, MyGuiContextMenuItemActionType.DISCONNECT_WAYPOINTS, true, MyTextsWrapperEnum.DisconnectWaypointsTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditProperties, MyGuiContextMenuItemActionType.EDIT_PROPERTIES, true, MyTextsWrapperEnum.EditPropertiesTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.CorrectSnappedPrefabs, MyGuiContextMenuItemActionType.CORRECT_SNAPPED_PREFABS, true, MyTextsWrapperEnum.CorrectSnappedPrefabsTooltip));

            // Asteroid context menu items
            List<MyGuiContextMenuItemHelper> asteroidMenuItems = new List<MyGuiContextMenuItemHelper>();
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveAsteroidToFile, MyGuiContextMenuItemActionType.SAVE_ASTEROID_TO_FILE, false, MyTextsWrapperEnum.SaveAsteroidToFileTooltip));
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetAsteroid, MyGuiContextMenuItemActionType.RESET_ASTEROID, false, MyTextsWrapperEnum.ResetAsteroidTooltip));
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ClearAsteroidMaterials, MyGuiContextMenuItemActionType.CLEAR_ASTEROID_MATERIALS, false, MyTextsWrapperEnum.ClearAsteroidMaterialsTooltip));
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditSelected, MyGuiContextMenuItemActionType.EDIT_SELECTED, true, MyTextsWrapperEnum.EditSelectedTooltip));
            asteroidMenuItems.AddRange(allTypesMenuItems);

            // Voxel hand menu items
            List<MyGuiContextMenuItemHelper> voxelHandMenuItems = new List<MyGuiContextMenuItemHelper>();
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitVoxelHand, MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND, true, MyTextsWrapperEnum.ExitVoxelHandTooltip));
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditVoxelHand, MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND, true, MyTextsWrapperEnum.EditVoxelHandTooltip));
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.LoadSectorTooltip));
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));

            // Container context menu items
            List<MyGuiContextMenuItemHelper> containerMenuItems = new List<MyGuiContextMenuItemHelper>();
            containerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterPrefabContainer, MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER, true, MyTextsWrapperEnum.EnterPrefabContainerTooltip));
            containerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            containerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditProperties, MyGuiContextMenuItemActionType.EDIT_PROPERTIES, true, MyTextsWrapperEnum.EditPropertiesTooltip));
            containerMenuItems.AddRange(allTypesMenuItems);

            List<MyGuiContextMenuItemHelper> activeContainerMenuItems = new List<MyGuiContextMenuItemHelper>();
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SelectAllObjects, MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS, true, MyTextsWrapperEnum.SelectAllObjectsTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitEditingMode, MyGuiContextMenuItemActionType.EXIT_EDITING_MODE, true, MyTextsWrapperEnum.ExitEditingModeTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.LoadSectorTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));

            List<MyGuiContextMenuItemHelper> wayPointMenuItems = new List<MyGuiContextMenuItemHelper>();
            wayPointMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            wayPointMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditSelected, MyGuiContextMenuItemActionType.EDIT_SELECTED, true, MyTextsWrapperEnum.EditSelectedTooltip));
            wayPointMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ConnectWaypoints, MyGuiContextMenuItemActionType.CONNECT_WAYPOINTS, true, MyTextsWrapperEnum.ConnectWaypointsTooltip));
            wayPointMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.DisconnectWaypoints, MyGuiContextMenuItemActionType.DISCONNECT_WAYPOINTS, true, MyTextsWrapperEnum.DisconnectWaypointsTooltip));
            wayPointMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));

            // Prefab context menu items
            List<MyGuiContextMenuItemHelper> prefabMenuItems = new List<MyGuiContextMenuItemHelper>();
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitEditingMode, MyGuiContextMenuItemActionType.EXIT_EDITING_MODE, true, MyTextsWrapperEnum.ExitEditingModeTooltip));
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ToggleSnapPoints, MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS, true, null));
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditProperties, MyGuiContextMenuItemActionType.EDIT_PROPERTIES, true, MyTextsWrapperEnum.EditPropertiesTooltip));
            prefabMenuItems.AddRange(allTypesMenuItems);

            // Prefabs context menu items
            List<MyGuiContextMenuItemHelper> prefabsMenuItems = new List<MyGuiContextMenuItemHelper>();
            prefabsMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitEditingMode, MyGuiContextMenuItemActionType.EXIT_EDITING_MODE, true, MyTextsWrapperEnum.ExitEditingModeTooltip));
            prefabsMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ToggleSnapPoints, MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS, true, null));
            prefabsMenuItems.AddRange(allTypesMenuItems);

            // Large ship context menu items
            List<MyGuiContextMenuItemHelper> largeShipMenuItems = new List<MyGuiContextMenuItemHelper>();
            largeShipMenuItems.AddRange(allTypesMenuItems);
            largeShipMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Small ship context menu items
            List<MyGuiContextMenuItemHelper> smallShipMenuItems = new List<MyGuiContextMenuItemHelper>();
            smallShipMenuItems.AddRange(allTypesMenuItems);
            smallShipMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            smallShipMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditProperties, MyGuiContextMenuItemActionType.EDIT_PROPERTIES, true, MyTextsWrapperEnum.EditPropertiesTooltip));

            // Debris context menu items
            List<MyGuiContextMenuItemHelper> debrisMenuItems = new List<MyGuiContextMenuItemHelper>();
            debrisMenuItems.AddRange(allTypesMenuItems);
            debrisMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Influence sphere menu items
            List<MyGuiContextMenuItemHelper> influenceSphereMenuItems = new List<MyGuiContextMenuItemHelper>();
            influenceSphereMenuItems.AddRange(allTypesMenuItems);
            influenceSphereMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Light menu items
            List<MyGuiContextMenuItemHelper> lightMenuItems = new List<MyGuiContextMenuItemHelper>();
            lightMenuItems.AddRange(allTypesMenuItems);
            lightMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // SpawnPoint items
            List<MyGuiContextMenuItemHelper> spawnPointMenuItems = new List<MyGuiContextMenuItemHelper>();
            spawnPointMenuItems.AddRange(allTypesMenuItems);

            // Static asteroid menu items
            List<MyGuiContextMenuItemHelper> staticAsteroidMenuItems = new List<MyGuiContextMenuItemHelper>();
            staticAsteroidMenuItems.AddRange(allTypesMenuItems);

            // Snap Point Selected menu items
            List<MyGuiContextMenuItemHelper> snapPointSelectedMenuItems = new List<MyGuiContextMenuItemHelper>();
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.EXIT_SELECTED, true, null));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.LoadSectorTooltip));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));            
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));

            // Attached context menu items
            List<MyGuiContextMenuItemHelper> attachedMenuItems = new List<MyGuiContextMenuItemHelper>();
            attachedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.DETACH_FROM_CAMERA, true, MyTextsWrapperEnum.DetachFromCameraTooltip));
            attachedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, true, MyTextsWrapperEnum.LoadSectorTooltip));
            attachedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));            
            attachedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));

            List<MyGuiContextMenuItemHelper> dummyPointMenuItems = new List<MyGuiContextMenuItemHelper>();
            dummyPointMenuItems.AddRange(allTypesMenuItems);

            // Security control HUB menu items
            List<MyGuiContextMenuItemHelper> securityControlHubMenuItems = new List<MyGuiContextMenuItemHelper>();
            securityControlHubMenuItems.AddRange(prefabMenuItems);

            // Scanner menu items
            List<MyGuiContextMenuItemHelper> scannerMenuItems = new List<MyGuiContextMenuItemHelper>();
            scannerMenuItems.AddRange(prefabMenuItems);

            // Cargo box menu items
            List<MyGuiContextMenuItemHelper> cargoBoxMenuItems = new List<MyGuiContextMenuItemHelper>();
            cargoBoxMenuItems.AddRange(allTypesMenuItems);
            cargoBoxMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditProperties, MyGuiContextMenuItemActionType.EDIT_PROPERTIES, true, MyTextsWrapperEnum.EditPropertiesTooltip));

            // Create menu helpers
            MyGuiContextMenuHelper generalContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.GeneralContextMenuCaption, generalMenuItems);
            MyGuiContextMenuHelper allTypesMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.MixedContextMenuCaption, allTypesMenuItems);
            MyGuiContextMenuHelper asteroidContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.AsteroidContextMenuCaption, asteroidMenuItems);
            MyGuiContextMenuHelper modifyAsteroidContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.ModifyAsteroidContextMenuCaption, voxelHandMenuItems);
            MyGuiContextMenuHelper containerContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContainerContextMenuCaption, containerMenuItems);
            MyGuiContextMenuHelper prefabContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContextMenuCaption, prefabMenuItems);
            MyGuiContextMenuHelper prefabsContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContextMenuCaption, prefabsMenuItems);
            MyGuiContextMenuHelper largeShipMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.LargeShipContextMenuCaption, largeShipMenuItems);
            MyGuiContextMenuHelper smallShipMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.SmallShipContextMenuCaption, smallShipMenuItems);
            MyGuiContextMenuHelper debrisMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.DebrisContextMenuCaption, debrisMenuItems);
            MyGuiContextMenuHelper influenceSphereItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.InfluenceSphereContextMenuCaption, influenceSphereMenuItems);
            MyGuiContextMenuHelper lightItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.CategoryTypeLights, lightMenuItems);
            MyGuiContextMenuHelper spawnPointItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.CategoryTypeLights, spawnPointMenuItems);
            MyGuiContextMenuHelper wayPointItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.CategoryTypeLights, wayPointMenuItems);
            MyGuiContextMenuHelper staticAsteroidItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.StaticAsteroidContextMenuCaption, staticAsteroidMenuItems);
            MyGuiContextMenuHelper snapPointSelectedItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.StaticAsteroidContextMenuCaption, snapPointSelectedMenuItems);
            MyGuiContextMenuHelper attachedContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.AttachedContextMenuCaption, attachedMenuItems);
            MyGuiContextMenuHelper editingContainerContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContainerContextMenuCaption, activeContainerMenuItems);
            MyGuiContextMenuHelper dummyPointItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.DummyPoint, dummyPointMenuItems);
            MyGuiContextMenuHelper securityControlHubHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.SecurityControlHUB, securityControlHubMenuItems);
            MyGuiContextMenuHelper scannerHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.Scanner, scannerMenuItems);            
            MyGuiContextMenuHelper cargoBoxHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.CargoBox, cargoBoxMenuItems);

            #endregion

            m_editorContextMenuHelpers.Add(MyEditorStateEnum.NOTHING_SELECTED, generalContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_ASTEROID, asteroidContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.VOXEL_HAND_ENABLED, modifyAsteroidContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFAB_CONTAINER, containerContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFAB, prefabContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFABS, prefabsContextMenuHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_LARGE_SHIP, largeShipMenuItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SMALL_SHIP, smallShipMenuItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_DEBRIS, debrisMenuItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_INFLUENCE_SPHERE, influenceSphereItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_LIGHT, lightItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_WAYPOINT, wayPointItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SPAWNPOINT, spawnPointItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_STATIC_ASTEROID, staticAsteroidItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SNAPPOINT, snapPointSelectedItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_MIXED, allTypesMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.ATTACHED, attachedContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.EDITING_PREFAB_CONTAINER, editingContainerContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_DUMMYPOINT, dummyPointItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SECURITY_CONTROL_HUB, securityControlHubHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SCANNER, scannerHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_CARGO_BOX, cargoBoxHelper);            
        }

        public static void LoadInGameEditorContent()
        {
            MyMwcLog.WriteLine("LoadInGameEditorContent()");

            m_editorContextMenuHelpers.Clear();

            #region Editor Context Menu Items

            // General context menu items
            List<MyGuiContextMenuItemHelper> generalMenuItems = new List<MyGuiContextMenuItemHelper>();
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SelectAllObjects, MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS, true, MyTextsWrapperEnum.SelectAllObjectsTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.RandomSector, MyGuiContextMenuItemActionType.RANDOM_SECTOR, true, MyTextsWrapperEnum.RandomSectorTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ClearWholeSector, MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR, true, MyTextsWrapperEnum.ClearWholeSectorTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveSector, MyGuiContextMenuItemActionType.SAVE_SECTOR, true, MyTextsWrapperEnum.SaveSectorTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AdjustGrid, MyGuiContextMenuItemActionType.ADJUST_GRID, false, MyTextsWrapperEnum.AdjustGridTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.LoadSector, MyGuiContextMenuItemActionType.LOAD_SECTOR, false, MyTextsWrapperEnum.LoadSectorTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SunSettings, MyGuiContextMenuItemActionType.SUN_SETTINGS, false, MyTextsWrapperEnum.SunSettingsTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.FogSettings, MyGuiContextMenuItemActionType.FOG_SETTINGS, false, MyTextsWrapperEnum.FogSettingsTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveAsteroids, MyGuiContextMenuItemActionType.SAVE_ASTEROIDS, false, MyTextsWrapperEnum.SaveAsteroidsTooltip));
            //generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterVoxelHand, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND, true, MyTextsWrapperEnum.EnterVoxelHandTooltip));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.Options, MyGuiContextMenuItemActionType.OPTIONS, true, MyTextsWrapperEnum.Options));
            generalMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitToMainMenu, MyGuiContextMenuItemActionType.EXIT_TO_MAIN_MENU, true, MyTextsWrapperEnum.ExitToMainMenu));

            // Items that will be included for all types of selected objects
            List<MyGuiContextMenuItemHelper> allTypesMenuItems = new List<MyGuiContextMenuItemHelper>();
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AttachToCamera, MyGuiContextMenuItemActionType.ATTACH_TO_CAMERA, true, MyTextsWrapperEnum.AttachToCameraTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditSelected, MyGuiContextMenuItemActionType.EDIT_SELECTED, true, MyTextsWrapperEnum.EditSelectedTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SwitchGizmoSpace, MyGuiContextMenuItemActionType.SWITCH_GIZMO_SPACE, true, MyTextsWrapperEnum.SwitchGizmoSpaceTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SwitchGizmoMode, MyGuiContextMenuItemActionType.SWITCH_GIZMO_MODE, true, MyTextsWrapperEnum.SwitchGizmoModeTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.CopySelected, MyGuiContextMenuItemActionType.COPY_SELECTED, false, MyTextsWrapperEnum.CopySelectedTooltip));
            allTypesMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.EXIT_SELECTED, true, null));

            /*
            // Asteroid context menu items
            List<MyGuiContextMenuItemHelper> asteroidMenuItems = new List<MyGuiContextMenuItemHelper>();
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SaveAsteroidToFile, MyGuiContextMenuItemActionType.SAVE_ASTEROID_TO_FILE, false, MyTextsWrapperEnum.SaveAsteroidToFileTooltip));
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetAsteroid, MyGuiContextMenuItemActionType.RESET_ASTEROID, false, MyTextsWrapperEnum.ResetAsteroidTooltip));
            asteroidMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ClearAsteroidMaterials, MyGuiContextMenuItemActionType.CLEAR_ASTEROID_MATERIALS, false, MyTextsWrapperEnum.ClearAsteroidMaterialsTooltip));
            asteroidMenuItems.AddRange(allTypesMenuItems);
            */
            /*
            // Voxel hand menu items
            List<MyGuiContextMenuItemHelper> voxelHandMenuItems = new List<MyGuiContextMenuItemHelper>();
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitVoxelHand, MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND, true, MyTextsWrapperEnum.ExitVoxelHandTooltip));
            voxelHandMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EditVoxelHand, MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND, true, MyTextsWrapperEnum.EditVoxelHandTooltip));
            */
            // Container context menu items
            List<MyGuiContextMenuItemHelper> containerMenuItems = new List<MyGuiContextMenuItemHelper>();
            containerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.EnterPrefabContainer, MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER, true, MyTextsWrapperEnum.EnterPrefabContainerTooltip));
            containerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            containerMenuItems.AddRange(allTypesMenuItems);

            List<MyGuiContextMenuItemHelper> activeContainerMenuItems = new List<MyGuiContextMenuItemHelper>();
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.AddObject, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.AddObjectTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.SelectAllObjects, MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS, true, MyTextsWrapperEnum.SelectAllObjectsTooltip));
            activeContainerMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));


            // Prefab context menu items
            List<MyGuiContextMenuItemHelper> prefabMenuItems = new List<MyGuiContextMenuItemHelper>();
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ToggleSnapPoints, MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS, true, null));
            prefabMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));
            prefabMenuItems.AddRange(allTypesMenuItems);

            // Prefabs context menu items
            List<MyGuiContextMenuItemHelper> prefabsMenuItems = new List<MyGuiContextMenuItemHelper>();
            prefabsMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ToggleSnapPoints, MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS, true, null));
            prefabsMenuItems.AddRange(allTypesMenuItems);

            // Large ship context menu items
            List<MyGuiContextMenuItemHelper> largeShipMenuItems = new List<MyGuiContextMenuItemHelper>();
            largeShipMenuItems.AddRange(allTypesMenuItems);
            largeShipMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Small ship context menu items
            List<MyGuiContextMenuItemHelper> smallShipMenuItems = new List<MyGuiContextMenuItemHelper>();
            smallShipMenuItems.AddRange(allTypesMenuItems);
            smallShipMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Debris context menu items
            List<MyGuiContextMenuItemHelper> debrisMenuItems = new List<MyGuiContextMenuItemHelper>();
            debrisMenuItems.AddRange(allTypesMenuItems);
            debrisMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Influence sphere menu items
            List<MyGuiContextMenuItemHelper> influenceSphereMenuItems = new List<MyGuiContextMenuItemHelper>();
            influenceSphereMenuItems.AddRange(allTypesMenuItems);
            influenceSphereMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Light menu items
            List<MyGuiContextMenuItemHelper> lightMenuItems = new List<MyGuiContextMenuItemHelper>();
            lightMenuItems.AddRange(allTypesMenuItems);
            lightMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Static asteroid menu items
            List<MyGuiContextMenuItemHelper> staticAsteroidMenuItems = new List<MyGuiContextMenuItemHelper>();
            staticAsteroidMenuItems.AddRange(allTypesMenuItems);

            // Snap Point Selected menu items
            List<MyGuiContextMenuItemHelper> snapPointSelectedMenuItems = new List<MyGuiContextMenuItemHelper>();
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.DeleteSelected, MyGuiContextMenuItemActionType.ADD_OBJECT, true, MyTextsWrapperEnum.DeleteSelectedTooltip));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.EXIT_SELECTED, true, null));
            snapPointSelectedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ResetRotation, MyGuiContextMenuItemActionType.RESET_ROTATION, true, MyTextsWrapperEnum.ResetRotationTooltip));

            // Attached context menu items
            List<MyGuiContextMenuItemHelper> attachedMenuItems = new List<MyGuiContextMenuItemHelper>();
            attachedMenuItems.Add(new MyGuiContextMenuItemHelper(MyTextsWrapperEnum.ExitSelected, MyGuiContextMenuItemActionType.DETACH_FROM_CAMERA, true, MyTextsWrapperEnum.DetachFromCameraTooltip));

            // Create menu helpers
            MyGuiContextMenuHelper generalContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.GeneralContextMenuCaption, generalMenuItems);
            MyGuiContextMenuHelper allTypesMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.MixedContextMenuCaption, allTypesMenuItems);
            //MyGuiContextMenuHelper asteroidContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.AsteroidContextMenuCaption, asteroidMenuItems);
            //MyGuiContextMenuHelper modifyAsteroidContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.ModifyAsteroidContextMenuCaption, voxelHandMenuItems);
            MyGuiContextMenuHelper containerContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContainerContextMenuCaption, containerMenuItems);
            MyGuiContextMenuHelper prefabContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContextMenuCaption, prefabMenuItems);
            MyGuiContextMenuHelper prefabsContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContextMenuCaption, prefabsMenuItems);
            MyGuiContextMenuHelper largeShipMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.LargeShipContextMenuCaption, largeShipMenuItems);
            MyGuiContextMenuHelper smallShipMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.SmallShipContextMenuCaption, smallShipMenuItems);
            MyGuiContextMenuHelper debrisMenuItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.DebrisContextMenuCaption, debrisMenuItems);
            MyGuiContextMenuHelper influenceSphereItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.InfluenceSphereContextMenuCaption, influenceSphereMenuItems);
            MyGuiContextMenuHelper lightItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.CategoryTypeLights, lightMenuItems);
            MyGuiContextMenuHelper staticAsteroidItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.StaticAsteroidContextMenuCaption, staticAsteroidMenuItems);
            MyGuiContextMenuHelper snapPointSelectedItemsHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.StaticAsteroidContextMenuCaption, snapPointSelectedMenuItems);
            MyGuiContextMenuHelper attachedContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.AttachedContextMenuCaption, attachedMenuItems);
            MyGuiContextMenuHelper editingContainerContextMenuHelper = new MyGuiContextMenuHelper(MyTextsWrapperEnum.PrefabContainerContextMenuCaption, activeContainerMenuItems);

            #endregion

            m_editorContextMenuHelpers.Add(MyEditorStateEnum.NOTHING_SELECTED, generalContextMenuHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_ASTEROID, asteroidContextMenuHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.VOXEL_HAND_ENABLED, modifyAsteroidContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFAB_CONTAINER, containerContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFAB, prefabContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_PREFABS, prefabsContextMenuHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_LARGE_SHIP, largeShipMenuItemsHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SMALL_SHIP, smallShipMenuItemsHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_DEBRIS, debrisMenuItemsHelper);
            //m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_INFLUENCE_SPHERE, influenceSphereItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_LIGHT, lightItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_STATIC_ASTEROID, staticAsteroidItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_SNAPPOINT, snapPointSelectedItemsHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.SELECTED_MIXED, allTypesMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.ATTACHED, attachedContextMenuHelper);
            m_editorContextMenuHelpers.Add(MyEditorStateEnum.EDITING_PREFAB_CONTAINER, editingContainerContextMenuHelper);
        }

    }
}

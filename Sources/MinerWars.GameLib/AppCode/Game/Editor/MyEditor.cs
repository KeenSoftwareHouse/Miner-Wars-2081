
using System;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using System.Threading;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Physics;
using System.Linq;

namespace MinerWars.AppCode.Game.Editor
{
    using SysUtils;
    using KeenSoftwareHouse.Library.Trace;
    using MinerWars.AppCode.Game.World;
    using MinerWars.AppCode.Game.TransparentGeometry;
    using System.Diagnostics;
    using System.Text;
    using MinerWars.AppCode.Game.Models;
    using MinerWars.AppCode.Game.Prefabs;
    using MinerWars.AppCode.Game.Entities.WayPoints;
    using MinerWars.AppCode.Game.Inventory;
    using MinerWars.AppCode.Game.Gameplay;
    using SysUtils.Utils;
    using MinerWars.AppCode.Game.Entities.CargoBox;
    using System.IO;
    using SharpDX.Toolkit;

    public enum MyEditorStateEnum
    {
        NOTHING_SELECTED,                      // represents state, when nothing is selected in editor
        SELECTED_ASTEROID,                     // represents state, when asteroid is selected
        VOXEL_HAND_ENABLED,                    // represents state, when using voxel hand for carving
        EDITING_PREFAB_CONTAINER,     // represents state, when editing container but nothing is selected inside
        SELECTED_PREFAB_CONTAINER,
        SELECTED_PREFAB,
        SELECTED_PREFABS,
        //SELECTED_LARGE_SHIP,
        SELECTED_SMALL_SHIP,
        SELECTED_DEBRIS,
        SELECTED_INFLUENCE_SPHERE,
        SELECTED_LIGHT,
        SELECTED_STATIC_ASTEROID,
        SELECTED_MIXED,
        SELECTED_SNAPPOINT,
        ATTACHED,                               // represents state, when something is attached to spectator            
        SELECTED_WAYPOINT,
        SELECTED_SPAWNPOINT,
        SELECTED_DUMMYPOINT,
        SELECTED_SECURITY_CONTROL_HUB,
        SELECTED_CARGO_BOX,
        SELECTED_SCANNER,
    }

    // This class represents game editor. There is no separate screen for editor, in case something needs to be disabled/enabled only for editor, we can do switch
    // from gameplay screen, based on current gameplay mode.
    partial class MyEditor
    {
        #region Fields

        /// <summary>
        /// Flag if user is in editable mode (in game/editor)
        /// </summary>
        private bool m_active = false;

        /// <summary>
        /// Property contains reference to currently edited container
        /// </summary>
        private MyPrefabContainer m_activePrefabContainer;

        /// <summary>
        /// Is used when changing speed of camera with scrollwheel
        /// </summary>
        private int m_lastScrollWheelValue;

        /// <summary>
        /// When adding new asteroid, whole operation is executed in separate thread, so when thread finishes, we show message to player that he cannot add new asteroid to scene due to size limitations
        /// </summary>
        private bool m_newAsteroidAllowed;

        /// <summary>
        /// Helps to control attachment of created objects in case, creation is performed in separate background thread
        /// </summary>
        private bool m_waitingForAtachment;

        /// <summary>
        /// Speed of camera is influenced by this property
        /// </summary>
        private double m_scrollWheelIndicatorHelper = 1;

        /// <summary>
        /// This can be either entity, that is being added(we are looking for its location) or that are being moved(catched)
        /// </summary>
        private Dictionary<MyEntity, Vector3> m_attachedEntities;

        /// <summary>
        /// When attaching multiple objects, sphere is created for all of them and is used to move objects in appropriate distance from camera when attached
        /// </summary>
        private BoundingSphere m_attachedEntitiesBoundingSphere;

        /// <summary>
        /// Selection rectangle created with mouse LMB
        /// </summary>
        private MinerWarsMath.Rectangle m_selectionRectangle;

        /// <summary>
        /// When objects attached to camera, this is used to change distance of objects from camera as required
        /// </summary>
        private int m_scrollWheelDistance = 1;

        ///// <summary>
        ///// Holds bookmarked camera positions
        ///// </summary>
        //private Dictionary<int, Vector3> m_cameraBookmarkPositions;

        ///// <summary>
        ///// TODO - unify to one property with above - Holds bookmarked camera targets
        ///// </summary>
        //private Dictionary<int, Vector3> m_cameraBookmarkTargets;

        /// <summary>
        /// Holds bookmarked camera
        /// </summary>
        private Dictionary<int, Matrix> m_cameraBookmarks;

        /// <summary>
        /// True when selection rectangle is being drawn
        /// </summary>
        private bool m_drawingSelectionRectangle;

        /// <summary>
        /// Start recording crosshair ray position for the rectangle selection
        /// </summary>
        private bool m_startCaptureCrosshairPosition = false;

        private MyLine m_startLineCrosshairSelection;
        private MyLine m_endLineCrosshairSelection;

        /// <summary>
        /// Computed in update how many objects are in collision possition
        /// </summary>
        private List<MyRBElement> m_collidingElements;

        /// <summary>
        /// Input tool which is used for various mouse move/mouse over situations
        /// </summary>
        private MyMouseInputTool m_mouseMoveInputTool = new MyMouseInputTool();

        /// <summary>
        /// Flag set from gui, to gain abbility set EditContainer mopde - after onserting entity into editor
        /// </summary>
        private static bool m_bInsertFromGuiFlag = false;

        /// <summary>
        /// Enables/disables lighting in editor mode
        /// </summary>
        public static bool EnableLightsInEditor = true;

        /// <summary>
        /// Enables/disables Bounding Container Cube
        /// </summary>
        public static bool DisplayPrefabContainerBounding = true;

        /// <summary>
        /// Enables/disables Prefab container's axis
        /// </summary>
        public static bool DisplayPrefabContainerAxis = true;

        /// <summary>
        /// Enables/disables Voxel bounding box
        /// </summary>
        public static bool DisplayVoxelBounding = true;

        /// <summary>
        /// Enables/disables player ship saving
        /// </summary>
        public static bool SavePlayerShip = false;

        /// <summary>
        /// List Of Named Prefab Object Groups
        /// </summary>
        public List<MyObjectGroup> ObjectGroups = new List<MyObjectGroup>();

        /// <summary>
        /// Show prefab snap points
        /// </summary>
        public bool ShowSnapPoints { get; set; }

        /// <summary>
        /// Show generators range
        /// </summary>
        public bool ShowGeneratorsRange { get; set; }

        /// <summary>
        /// Show large weapons range
        /// </summary>
        public bool ShowLargeWeaponsRange { get; set; }

        /// <summary>
        /// Show deactivated entities
        /// </summary>
        public bool ShowDeactivatedEntities { get; set; }

        /// <summary>
        /// Show helper texts
        /// </summary>
        public static bool EnableTextsDrawing { get; set; }        

        /// <summary>
        /// Enables rotating against center of object based on bounding box instead of center of model as defined in 3rd party editor
        /// </summary>
        public static bool EnableObjectPivot { get; set; }

        /// <summary>
        /// Locks selection for transformation
        /// </summary>
        public static bool TransformLocked { get; set; }

        /// <summary>
        /// Enable snap point filiter (add object tree view)
        /// </summary>\
        private bool snapPointFilter;
        public bool SnapPointFilter {
            get
            {
                return this.snapPointFilter;
            }

            set
            {
                if (this.snapPointFilter != value)
                {
                    //Activate();
                }
                this.snapPointFilter = value;
            }
        }

        /// <summary>
        /// If true, snap point size is fixed to screen size (no perspective degradation)
        /// </summary>
        public bool FixedSizeSnapPoints { get; set; }

        /// <summary>
        /// Snap Point list for all prefab types
        /// </summary>
        //public static Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, List<MyPrefabSnapPoint>> PrefabSnapPoints { get; set; }
        private static List<MyPrefabSnapPoint>[][] m_prefabSnapPoints;

        /// <summary>
        /// Collection of linked snap points
        /// </summary>
        public List<List<MySnapPointLink>> LinkedSnapPoints { get; set; }
        
        /// <summary>
        /// Used as temporary storage between checkpoint load and linkentities
        /// </summary>
        public List<MyMwcObjectBuilder_SnapPointLink> LinkedSnapPointsBuilders { get; set; }


        private int m_issueCheckAllCollidingObjectsCounter = -1;

        
        private static Matrix[] m_storedMatrices = new Matrix[4];

        #endregion

        #region Properties
        /// <summary>
        /// Singleton
        /// </summary>
        public static MyEditor Static = null;

        /// <summary>
        /// Reference to actually edited prefab container
        /// </summary>
        /// <returns></returns>
        public MyPrefabContainer GetEditedPrefabContainer() { return m_activePrefabContainer; }

        /// <summary>
        /// Background thread is used where the editor actions might consume more time and resources, that can stuck game.
        /// </summary>
        public Thread BackgroundWorkThread;

        /// <summary>
        /// Property that signals when to reload models in draw, for example when creating random sector
        /// </summary>
        public bool ReloadModelsInDrawRequired;

        /// <summary>
        /// Based on the type of selected objects, there can be some controls added/removed
        /// </summary>
        public bool ReloadEditorControls;

        /// <summary>
        /// When it is necessary to add/remove entities in loop but cannot modify collection during loop
        /// </summary>
        public List<MyEntity> m_safeIterationHelper;

        /// <summary>
        /// Entities that are in collision with other objects
        /// </summary>
        public List<MyRBElement> CollidingElements { get { return m_collidingElements; } }

        /// <summary>
        /// Foundation factory which is using by editor
        /// </summary>
        public MyPrefabFoundationFactory FoundationFactory { get; private set; }

        // Can store any data for copy, paste operations
        private static object Clipboard;

        #endregion

        #region Methods

        /// <summary>
        /// Static initialization
        /// </summary>
        static MyEditor()
        {
            Static = new MyEditor();
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        public MyEditor()
        {
            InitEditorInfo();
            MyInfluenceSphere.Init();
            m_lastScrollWheelValue = MyEditorConstants.DEFAULT_CAMERA_SPEED;
            m_attachedEntities = new Dictionary<MyEntity, Vector3>(100);
            m_safeIterationHelper = new List<MyEntity>(100);
            m_attachedEntitiesBoundingSphere = new BoundingSphere();
            //m_cameraBookmarkPositions = new Dictionary<int, Vector3>(10);
            //m_cameraBookmarkTargets = new Dictionary<int, Vector3>(10);
            m_cameraBookmarks = new Dictionary<int, Matrix>(9);
            m_collidingElements = new List<MyRBElement>();
            LinkedSnapPoints = new List<List<MySnapPointLink>>();
            ShowSnapPoints = true;
            SnapPointFilter = true;
            FixedSizeSnapPoints = false;
        }

        public static void SetClipboard(object data)
        {
            Clipboard = data;
        }

        public static bool GetClipboard<T>(out T value)
        {
            if (Clipboard is T)
            {
                value = (T)Clipboard;
                return true;
            }
            else if (typeof(T) == typeof(Color))
            {
                if (Clipboard is Vector4)
                {
                    value = (T)(object)new Color((Vector4)Clipboard);
                    return true;
                }
                if (Clipboard is Vector3)
                {
                    value = (T)(object)new Color((Vector3)Clipboard);
                    return true;
                }
            }

            value = default(T);

            return false;
        }

        /// <summary>
        /// EditorDebugDraw
        /// </summary>
        public void EditorDebugDraw()
        {
            if (!MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())
                return;

            DrawSelectionRectangle();
            DrawSelectedBounding();
            DrawDebugPrefabRanges();
            DrawDebugDeactivatedEntities();

            MyEditor.Static.DrawVisualControls();

            //@ JK just for debugging selection
            //DrawDebugSelection();
        }

        static void DrawDebugDeactivatedEntities() 
        {
            if (MyGuiScreenGamePlay.Static.IsEditorActive() && MyEditor.Static.ShowDeactivatedEntities) 
            {
                foreach (MyEntity entity in MyEntities.GetEntities()) 
                {
                    entity.DebugDrawDeactivated();
                }
            }
        }

        static void DrawDebugPrefabRanges()
        {
            if (MyGuiScreenGamePlay.Static.IsEditorActive() && (MyEditor.Static.ShowLargeWeaponsRange || MyEditor.Static.ShowGeneratorsRange))
            {
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    if (entity is MyPrefabContainer)
                    {
                        if (MyEditor.Static.ShowGeneratorsRange)
                        {
                            foreach (var prefab in ((MyPrefabContainer)entity).GetPrefabs(CategoryTypesEnum.GENERATOR))
                            {
                                MyPrefabGenerator prefabGenerator = prefab as MyPrefabGenerator;
                                if (prefabGenerator != null)
                                {
                                    prefabGenerator.DebugDrawRange();
                                }
                            }
                        }
                        if (MyEditor.Static.ShowLargeWeaponsRange)
                        {
                            foreach (var prefab in ((MyPrefabContainer)entity).GetPrefabs(CategoryTypesEnum.WEAPONRY))
                            {
                                MyPrefabLargeWeapon prefabLargeWeapon = prefab as MyPrefabLargeWeapon;
                                if (prefabLargeWeapon != null)
                                {
                                    prefabLargeWeapon.DebugDrawRange();
                                }
                            }
                        }
                    }
                }
            }
        }

        //Initializes stringbuilders used for displaying text information in editor
        private void InitEditorInfo()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.Editor, "Editor", EditorDebugDraw, MyRenderStage.DebugDraw);
        }

        /// <summary>
        /// Deinitialize
        /// </summary>
        public void Deinitialize()
        {
            //MyRender.UnregisterRenderModule("Editor DebugDraw", EditorDebugDraw, MyRenderStage.DebugDraw);
        }

        /// <summary>
        /// Set editor active
        /// </summary>
        /// <param name="bEnable"></param>
        public void SetActive(bool bEnable)
        {
            m_active = bEnable;
            if (m_active == false)
            {
                MyConfig.EditorEnableLightsInEditor = MyEditor.EnableLightsInEditor;
                MyConfig.Save();
                SwitchToGameplay();
            }

            InitializeFromConfig();
        }

        /// <summary>
        /// Checks if editor is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return m_active;
        }

        public static void InitializeFromConfig()
        {
            if (Static.m_active)
            {
                MyEntities.SetTypeHidden(typeof(MyWayPoint), MyConfig.EditorHiddenWayPoint);
                MyEntities.SetTypeSelectable(typeof(MyWayPoint), MyConfig.EditorSelectableWayPoint);

                MyEntities.SetTypeHidden(typeof(MyVoxelMap), MyConfig.EditorHiddenVoxelMap);
                MyEntities.SetTypeSelectable(typeof(MyVoxelMap), MyConfig.EditorSelectableVoxelMap);

                MyEntities.SetTypeHidden(typeof(MyDummyPoint), MyConfig.EditorHiddenDummyPoint);
                MyEntities.SetTypeSelectable(typeof(MyDummyPoint), MyConfig.EditorSelectableDummyPoint);

                MyEntities.SetTypeHidden(typeof(MyPrefabBase), MyConfig.EditorHiddenPrefabBase);
                MyEntities.SetTypeSelectable(typeof(MyPrefabBase), MyConfig.EditorSelectablePrefabBase);

                MyEntities.SetTypeHidden(typeof(MySpawnPoint), MyConfig.EditorHiddenSpawnPoint);
                MyEntities.SetTypeSelectable(typeof(MySpawnPoint), MyConfig.EditorSelectableSpawnPoint);

                MyEntities.SetTypeHidden(typeof(MyInfluenceSphere), MyConfig.EditorHiddenInfluenceSphere);
                MyEntities.SetTypeSelectable(typeof(MyInfluenceSphere), MyConfig.EditorSelectableInfluenceSphere);

                MyWayPointGraph.WaypointsIgnoreDepth = MyConfig.EditorWaypointsIgnoreDepth;

                MyEntities.SafeAreasHidden = !MyConfig.EditorShowSafeAreas;
                MyEntities.SafeAreasSelectable = MyConfig.EditorSelectableSafeAreas;

                MyEntities.DetectorsHidden = !MyConfig.EditorShowDetectors;
                MyEntities.DetectorsSelectable = MyConfig.EditorSelectableDetectors;
                
                MyEntities.ParticleEffectsHidden = !MyConfig.EditorShowParticleEffects;
                MyEntities.ParticleEffectsSelectable = MyConfig.EditorSelectableParticleEffects;
            }

            MyEditor.EnableLightsInEditor = MyConfig.EditorEnableLightsInEditor;
            MyEditor.DisplayPrefabContainerBounding = MyConfig.EditorDisplayPrefabContainerBounding;
            MyEditor.DisplayPrefabContainerAxis = MyConfig.EditorDisplayPrefabContainerAxis;
            MyEditor.Static.ShowSnapPoints = MyConfig.EditorShowSnapPoints;
            MyEditor.Static.SnapPointFilter = MyConfig.EditorSnapPointFilter;
            MyEditor.Static.FixedSizeSnapPoints = MyConfig.EditorFixedSizeSnapPoints;
            MyEditor.EnableObjectPivot = MyConfig.EditorEnableObjectPivot;
            MyEditor.DisplayVoxelBounding = MyConfig.EditorDisplayVoxelBounding;
            MyEditor.Static.ShowGeneratorsRange = MyConfig.EditorShowGeneratorsRange;
            MyEditor.Static.ShowLargeWeaponsRange = MyConfig.EditorShowLargeWeaponsRange;
            MyEditor.Static.ShowDeactivatedEntities = MyConfig.EditorShowDeactivatedEntities;
            MyEditor.EnableTextsDrawing = MyConfig.EditorEnableTextsDrawing;

            MyEditor.Static.RefreshSelectionSettings();
        }

        public void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor.LoadData");

            m_newAsteroidAllowed = true;

            MyEditorActions.LoadData();
            MyEditorGizmo.LoadData();
            MyEditorVoxelHand.LoadData();
            MyEditorGrid.LoadData();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiContextMenuHelpers.LoadDefaultEditorContent");
            MyGuiContextMenuHelpers.LoadDefaultEditorContent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyEditorGrid.IsGridActive = false;
            MyEditorShortcutManager.GetInstance().Initialize();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor.GetPrefabSnapPoints");
            GetPrefabSnapPoints();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Load content
        /// </summary>
        public void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor::LoadContent");

            MyEditorGizmo.LoadContent();
            MyEditorVoxelHand.LoadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Unload content
        /// </summary>
        public void UnloadContent()
        {

            MyEditorVoxelHand.UnloadContent();

        }

        public void UnloadData()
        {
            //  This is just for case that whole application is quiting after Alt+F4 or something
            //  Don't try to unload content in that thread or something - we don't know in what state it is. Just abort it.
            if (IsBackgroundWorkThreadAlive())
            {
                StopBackgroundWorkThread();
            }


            MyEditorActions.UnloadData();
            MyEditorGizmo.UnloadData();
            MyEditorVoxelHand.UnloadData();


            MyEditorShortcutManager.GetInstance().Clear();
            if (m_collidingElements != null) m_collidingElements.Clear();
            SetActive(false);
        }


        /// <summary>
        /// Prepare list of entities, so then we can do remove while iterating it
        /// </summary>
        void PrepareSafeAttachedObjectsIterationHelper()
        {
            m_safeIterationHelper.Clear();
            foreach (KeyValuePair<MyEntity, Vector3> kvp in m_attachedEntities)
            {
                m_safeIterationHelper.Add(kvp.Key);
            }
        }

        /// <summary>
        /// Enable mouse move input tool handling
        /// </summary>
        /// <param name="bEnable"></param>
        public void EnableMouseMoveInputHandling(bool bEnable)
        {
            m_mouseMoveInputTool.EnableInputHandling(bEnable);
        }

        /// <summary>
        /// Enable AABB under mouse cursor
        /// </summary>
        /// <param name="bEnable"></param>
        public void EnablePhysAABBUnderMouse(bool bEnable)
        {
            m_mouseMoveInputTool.EnableAABBUnderMouse(bEnable);
        }

        /// <summary>
        /// This method receives action type(each context menu item has this associated enum) and based on it, performs required action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool PerformContextMenuAction(MyGuiContextMenuItemActionType action)
        {
            // set close value correctly - when action opens new screen, set it to false,
            // so that context menu screen is not closed and we can return to it from new screen
            bool close = true;
            switch (action)
            {
                case MyGuiContextMenuItemActionType.ADD_OBJECT:
                    // Done by AddObject TreeView
                    break;
                case MyGuiContextMenuItemActionType.ADJUST_GRID:
                    //TODO - show screen with grid adjustment options
                    break;
                case MyGuiContextMenuItemActionType.ATTACH_TO_CAMERA:
                    AttachSelectedObjectsToCamera();
                    break;
                case MyGuiContextMenuItemActionType.CLEAR_ASTEROID_MATERIALS:
                    //TODO
                    break;
                case MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR:
                    ClearWholeSector();
                    break;
                case MyGuiContextMenuItemActionType.COPY_SELECTED:
                    CopySelected(true);
                    break;
                case MyGuiContextMenuItemActionType.EDIT_SELECTED:
                    EditSelected();
                    break;
                case MyGuiContextMenuItemActionType.RESET_ROTATION:
                    MyEditorGizmo.ResetSelectedRotation();
                    break;
                case MyGuiContextMenuItemActionType.EXIT_EDITING_MODE:
                    ExitActivePrefabContainer();
                    break;
                case MyGuiContextMenuItemActionType.EXIT_SELECTED:
                    MyEditorGizmo.ClearSelection();
                    MyEditorGizmo.SelectedSnapPoint = null;
                    break;
                case MyGuiContextMenuItemActionType.DETACH_FROM_CAMERA:
                    DetachAndConfirm();
                    break;
                case MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER:
                    MyPrefabContainer container = (MyPrefabContainer)MyEditorGizmo.GetFirstSelected();
                    EditPrefabContainer(container);
                    break;
                case MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS:
                    ToggleSnapPoints();
                    break;
                case MyGuiContextMenuItemActionType.EXIT_TO_MAIN_MENU:
                    MyGuiScreenMainMenu.OnExitToMainMenuClick(null);
                    break;
                case MyGuiContextMenuItemActionType.LOAD_SECTOR:
                    LoadSector();
                    break;
                case MyGuiContextMenuItemActionType.RESET_ASTEROID:
                    //TODO
                    break;
                case MyGuiContextMenuItemActionType.SAVE_ASTEROID_TO_FILE:
                    //TODO
                    break;
                case MyGuiContextMenuItemActionType.SAVE_SECTOR:
                    SaveSector();
                    break;
                case MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS:
                    SelectAllObjects();
                    break;
                case MyGuiContextMenuItemActionType.SWITCH_GIZMO_MODE:
                    MyEditorGizmo.SwitchGizmoMode();
                    break;
                case MyGuiContextMenuItemActionType.SWITCH_GIZMO_SPACE:
                    MyEditorGizmo.ActiveSpace = MyUtils.GetNextOrPreviousEnumValue(MyEditorGizmo.ActiveSpace, true);
                    break;
                case MyGuiContextMenuItemActionType.SUN_SETTINGS:
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSettingsSun());
                    close = false;
                    break;
                case MyGuiContextMenuItemActionType.FOG_SETTINGS:
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSettingsFog());
                    close = false;
                    break;
                case MyGuiContextMenuItemActionType.SAVE_ASTEROIDS:
                    //TODO
                    break;
                case MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND:
                    MyEditorVoxelHand.SetEnabled(true);
                    break;
                case MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND:
                    MyEditorVoxelHand.SetEnabled(false);
                    break;
                case MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND:
                    //MyGuiManager.AddScreen(new MyGuiScreenEditorEditVoxelHand());
                    //close = false;
                    break;
                case MyGuiContextMenuItemActionType.OPTIONS:
                    //Allow changing video options from game in DX version
                    MyGuiManager.AddScreen(new MyGuiScreenOptions(true));
                    //MyGuiManager.AddScreen(new MyGuiScreenOptions(false));
                    close = false;
                    break;
                case MyGuiContextMenuItemActionType.CONNECT_WAYPOINTS:
                    MyWayPoint.ConnectAll(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities));
                    break;
                case MyGuiContextMenuItemActionType.DISCONNECT_WAYPOINTS:
                    MyWayPoint.DisconnectAll(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities));
                    break;
                case MyGuiContextMenuItemActionType.CORRECT_SNAPPED_PREFABS:
                    MyEditor.Static.CorrectSnappedPrefabs();
                    break;
            }
            return close;
        }

        /// <summary>
        /// Handle input
        /// </summary>
        public void HandleInput(MyGuiInput input, bool hasMouse)
        {
            if (IsBackgroundWorkThreadAlive())
                return;

            // this must be here, because Update is before HandleInput and we need actual spec posititon and orientation
            if (MyEditorVoxelHand.IsEnabled())
            {
                MyEditorVoxelHand.UpdateShapePosition();
            }

            if (!hasMouse)
            {
                m_drawingSelectionRectangle = false;

                // Needed for disabling highlighting
                m_mouseMoveInputTool.EnableInputHandling(false);

                // We must save last scroll value, because when we use scrollwheel when editor doesn't handle input (any control has focus), 
                // then wrong value is here in next input handling
                m_lastScrollWheelValue = input.MouseScrollWheelValue();
                return;
            }

            if (input.IsLeftMousePressed())
            {
                MyGuiScreenGamePlay.Static.EditorControls.Focused(false);
            }

            MySpectator.EnableRotation();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Switch between normal and crosshair camera modes
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsNewMiddleMousePressed())
            {
                MyConfig.EditorUseCameraCrosshair = !MyConfig.EditorUseCameraCrosshair;
                MyConfig.Save();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Switch free and locked prefab rotations
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.R))
            {
                MyConfig.EditorLockedPrefab90DegreesRotation = !MyConfig.EditorLockedPrefab90DegreesRotation;
                MyConfig.Save();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Enable/Disable prefab container maximum area displaying
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.C))
            {
                DisplayPrefabContainerBounding = !DisplayPrefabContainerBounding;
                MyConfig.EditorDisplayPrefabContainerBounding = DisplayPrefabContainerBounding;
                MyConfig.Save();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Enable/Disable showing unselected objects bounding
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.U))
            {
                MyConfig.EditorDisplayUnselectedBounding = !MyConfig.EditorDisplayUnselectedBounding;
                MyConfig.Save();
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Enable/Disable transform lock
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.L))
            {
                MyEditor.TransformLocked = !MyEditor.TransformLocked;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Extract selected prefabs to new container
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (MyFakes.ENABLE_EXTRACT_PREFABS && input.IsAnyShiftKeyPressed() && input.IsAnyCtrlKeyPressed() && input.IsNewKeyPress(Keys.E))
            {
                if (m_extractPrefabsBuilder == null)
                {
                    ExtractPrefabsToNewContainer();
                }
                else
	            {
                    var activeContainer = GetEditedPrefabContainer();
                    ExitActivePrefabContainer();

                    MyEntities.CreateFromObjectBuilderAndAdd(null, m_extractPrefabsBuilder, activeContainer.WorldMatrix);
                    
                    m_extractPrefabsBuilder = null;
	            }
            }

            if (input.IsAnyShiftKeyPressed() && input.IsAnyCtrlKeyPressed() && input.IsNewKeyPress(Keys.X))
            {
                var container = MyEditor.Static.GetEditedPrefabContainer();
                if (container != null)
                {
                    MyEditorGizmo.SelectedEntities.Clear();
                    foreach (var prefab in container.GetPrefabs())
                    {
                        var light = prefab as MyPrefabLight;

                        if (light != null && light.Effect == CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs.MyLightEffectTypeEnum.DISTANT_GLARE)
                        {
                            light.Effect = CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs.MyLightEffectTypeEnum.NORMAL;
                            MyEditorGizmo.SelectedEntities.Add(light);
                        }
                        
                        if (light != null && light.Effect == CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs.MyLightEffectTypeEnum.DISTANT_GLARE_FLASHING)
                        {
                            light.Effect = CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs.MyLightEffectTypeEnum.CONSTANT_FLASHING;
                            MyEditorGizmo.SelectedEntities.Add(light);
                        }
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Connect selected waypoints in a path
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.J) && MyWayPoint.ContainsWayPoint(MyEditorGizmo.SelectedEntities))
            {
                MyWayPoint last = null;
                foreach (var v in MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities))
                {
                    if (last != null)
                        MyWayPoint.Connect(v as MyWayPoint, last);
                    last = v;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Connect/Disconnect all selected waypoints
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.K) && MyWayPoint.ContainsWayPoint(MyEditorGizmo.SelectedEntities))
            {
                //int total, save, notsave, generated;
                //MyWayPointGraph.CountWaypointTypes(out total, out notsave, out save, out generated);

                if (MyWayPoint.AnyEdgesBetween(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities)))
                {
                    MyWayPoint.DisconnectAll(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities));
                }
                else
                {
                    if (MyEditorGizmo.SelectedEntities.Count >= 10)
                        
                    {            
                        int edgeCount = MyEditorGizmo.SelectedEntities.Count * (MyEditorGizmo.SelectedEntities.Count - 1) / 2;
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, 
                            new StringBuilder().AppendFormat("Do you really want to create all \uEA61{0}\uEA60 edges?", edgeCount),
                            new StringBuilder("Stop and think!"), MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, m_connectAllSelectedWaypointsCallback
                        ));    
                    }
                    else
                    {
                        MyWayPoint.ConnectAll(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities));
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Add named waypoint selection (WayPointPath) or rename it
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.N) && MyWayPoint.ContainsWayPoint(MyEditorGizmo.SelectedEntities))
            {
                if (MyWayPointGraph.SelectedPath != null)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorWayPointPath(MyWayPointGraph.SelectedPath, false));
                }
                else
                {
                    // find unique name
                    int number = 1;
                    string name;
                    do
                    {
                        name = new StringBuilder("Group").Append(number).ToString();
                        number++;
                    } while (MyWayPointGraph.GetPath(name) != null);

                    var path = new MyWayPointPath(name);
                    foreach (var selectedWaypoint in MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities))
                        path.WayPoints.Add(selectedWaypoint);
                    MyWayPointGraph.SelectedPath = path;
                    MyGuiManager.AddScreen(new MyGuiScreenEditorWayPointPath(MyWayPointGraph.SelectedPath, true));
                }
            } 

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Correct snapped prefabs
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.N) && !MyWayPoint.ContainsWayPoint(MyEditorGizmo.SelectedEntities))
            {
                CorrectSnappedPrefabs();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Reset all prefab waypoints
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.H))
            {
                int modifiedPathCount = MyWayPointGraph.CountPathsWithWaypointsWithParents();
                var message = new StringBuilder("");
                message.AppendFormat("This will delete all {0} waypoints from all prefabs and create new ones.", MyWayPointGraph.CountWaypointsWithParents());
                if (modifiedPathCount > 0)
                {
                    message.AppendFormat("\n\rThese waypoints will also be removed from {0} paths.", modifiedPathCount);
                }
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, 
                    message.Append("\n\r\n\rAre you sure?"),
                    new StringBuilder("Reset all waypoints in prefabs"), MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, m_resetAllWaypointsInPrefabsCallback
                ));
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Cut out voxels to make space for selected entities
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.M))
            {
                foreach (var v in MyVoxelMaps.GetVoxelMaps())
                {
                    foreach (var e in MyEditorGizmo.SelectedEntities)
                    {
                        if (v.WorldAABB.Contains(e.WorldAABB) == ContainmentType.Disjoint) continue;

                        var voxelHandSphereBuilder = new MyMwcObjectBuilder_VoxelHand_Sphere(
                            new MyMwcPositionAndOrientation(e.WorldAABB.GetCenter(), Vector3.Forward, Vector3.Up),
                            Vector3.Distance(e.WorldAABB.GetCenter(), e.WorldAABB.GetCorners()[0]),
                            MyMwcVoxelHandModeTypeEnum.SUBTRACT
                        );
                        var voxelHandSphere = new MyVoxelHandSphere();
                        voxelHandSphere.Init(voxelHandSphereBuilder, v as MyVoxelMap);
                        MyEntities.Remove(voxelHandSphere);
                        (v as MyVoxelMap).AddVoxelHandShape(voxelHandSphere, false);
                    }
                }
                RecheckAllColidingEntitesAndClearNonColiding();
            }

            /////Camera+selection utils
            int pressedNumericKeyIndex = -1;
            for (int key = (int)Keys.D1; key <= (int)Keys.D4; key++)
            {
                if (input.IsNewKeyPress((Keys)key))
                {
                    pressedNumericKeyIndex = key - (int)Keys.D1;
                    break;
                }
            }
            if (pressedNumericKeyIndex != -1)
            {
                if (input.IsAnyCtrlKeyPressed())
                {
                    if (MyEditorGizmo.SelectedEntities.Count > 0)
                        m_storedMatrices[pressedNumericKeyIndex] = MyEditorGizmo.SelectedEntities[0].WorldMatrix;
                    else
                        m_storedMatrices[pressedNumericKeyIndex] = Matrix.CreateWorld(MyCamera.Position, MyCamera.ForwardVector, MyCamera.UpVector);
                }
                else
                {
                    if (m_storedMatrices[pressedNumericKeyIndex] != default(Matrix))
                    {
                        if (MyEditorGizmo.SelectedEntities.Count > 0)
                            MyEditorGizmo.SelectedEntities[0].WorldMatrix = m_storedMatrices[pressedNumericKeyIndex];
                        else
                            MySpectator.SetViewMatrix(Matrix.Invert(m_storedMatrices[pressedNumericKeyIndex]));
                    }
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Show add new object Tree View
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsNewKeyPress(Keys.Insert))
            {
                MyGuiScreenGamePlay.Static.EditorControls.OnAddObject(null);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  When using carving tool, you can modify asteroid only, you have to exit from this mode to edit rest of the world.
            //  Carving tool is providing possibility to remove or add content from asteroid
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (MyEditorVoxelHand.IsEnabled() && MyEditorVoxelHand.DetachedVoxelHand == null)
            {
                MyEditorVoxelHand.HandleInput(input);
            }
            else
            {
                if (MyEditorVoxelHand.IsEnabled() && MyEditorVoxelHand.DetachedVoxelHand != null)
                {
                    MyEditorVoxelHand.HandleInput(input);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Repair To The Max
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (input.IsNewKeyPress(Keys.O) && MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())
                {
                    foreach (MyEntity e in MyEditorGizmo.SelectedEntities)
                        e.RepairToMax();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Voxel Hand - if disabled, enable here
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((input.IsEditorControlNewPressed(MyEditorControlEnums.VOXEL_HAND)) &&
                    MyGuiContextMenuHelpers.GetEditorContextMenuHelper(GetCurrentState()).IsActionTypeAvailable(MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND))
                {
                    MyEditorVoxelHand.SwitchEnabled();
                    m_mouseMoveInputTool.EnableInputHandling(!MyEditorVoxelHand.IsEnabled());
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // First handle Rectangle selection on primary action key
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsEditorControlNewPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY) && input.IsAnyShiftKeyPressed() == false)
                {
                    m_drawingSelectionRectangle = true;
                    m_startCaptureCrosshairPosition = true;
                }

                if (input.IsEditorControlPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY) && input.IsAnyShiftKeyPressed() == false && m_drawingSelectionRectangle)
                {
                    CreateSelectionRectangle(input);
                    m_mouseMoveInputTool.EnableInputHandling(false);
                }
                else if (input.WasEditorControlReleased(MyEditorControlEnums.PRIMARY_ACTION_KEY))
                {
                    // Mouse was released a frame ago
                    m_drawingSelectionRectangle = false;
                    m_selectionRectangle = MinerWarsMath.Rectangle.Empty;
                    m_mouseMoveInputTool.EnableInputHandling(!MyEditorVoxelHand.IsEnabled());
                }




                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Second handle objects selection and other interaction with primary action key
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (input.IsEditorControlNewReleased(MyEditorControlEnums.PRIMARY_ACTION_KEY))
                {
                    m_mouseMoveInputTool.EnableInputHandling(false);
                    //In case selection rectangle is drawn with mouse, select objects in it, otherwise select single objects
                    if (IsSelectingByRectangle() && IsAnyEntityAttachedToCamera() == false)
                    {
                        MyEditorGizmo.SelectedSnapPoint = null;
                        SelectAllEntitiesInRectangle(input);
                        m_drawingSelectionRectangle = false;
                    }
                    else
                    {
                        // When left mouse pressed and no objects are attached to camera, try to select object under mouse cursor
                        if (IsAnyEntityAttachedToCamera())
                        {
                            // When objects attached, confirm their new location
                            DetachAndConfirm();
                        }
                    }
                }

                // Handle input for editor transformation gizmo
                MyEditorGizmo.HandleInput(input);

                if (!MyEditor.TransformLocked)
                {
                    if (input.IsEditorControlNewPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY))
                    {
                        if (!(IsSelectingByRectangle() && IsAnyEntityAttachedToCamera() == false))
                        {
                            // When left mouse pressed and no objects are attached to camera, try to select object under mouse cursor
                            if (IsAnyEntityAttachedToCamera() == false)
                            {
                                MyEditorGizmo.SelectByIntersectionLine(input);

                                if (MyConfig.EditorUseCameraCrosshair)
                                {
                                    AttachSelectedObjectsToCamera();
                                }
                            }
                        }
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Attach object to camera, attach object to snap point
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.WasEditorControlPressed(MyEditorControlEnums.SECONDARY_ACTION_KEY) == false && input.IsEditorControlPressed(MyEditorControlEnums.SECONDARY_ACTION_KEY))
                {
                    //m_mouseMoveInputTool.EnableInputHandling(false);
                    //In case RMB button is pressed while holding left shift, make copy of selected items, but only if nothing is attached right now
                    if (input.IsKeyPress(Keys.LeftShift) && IsAnyEntityAttachedToCamera() == false)
                    {
                        CopySelected(true);
                    }
                    else
                    {
                        if ((MyConfig.EditorUseCameraCrosshair || input.IsAnyControlPress()) && MyEditorGizmo.SelectedSnapPoint == null)
                        {
                            //It is not possible to attach any objects while there are some attached now
                            if (IsAnyEntityAttachedToCamera() == false)
                            {
                                AttachSelectedObjectsToCamera();

                                //when pressing RMB in container, leave container edit mode or show message, in case container does not contain any prefab and disallow to exit edit mode
                                if (!MyGuiScreenGamePlay.Static.IsIngameEditorActive() &&
                                    !IsAnyEntityAttachedToCamera() &&
                                    m_activePrefabContainer != null &&
                                    !MyEditorGizmo.IsEntitySelected(MyEntities.GetEntityUnderMouseCursor()))
                                {
                                    ExitActivePrefabContainer();
                                }
                            }
                        }
                        else if (input.IsAnyAltPress() && MyEditorGizmo.SelectedSnapPoint == null)
                        {
                            if (MyEditorGizmo.IsAnyEntitySelected())
                            {
                                BoundingSphere sphere;
                                GetSelectedEntitiesBoundingSphere(out sphere);

                                Vector3 direction = MyCamera.ForwardVector * m_scrollWheelDistance * sphere.Radius;
                                MySpectator.Position = sphere.Center - direction;
                            }
                        }
                        else if (MyEditorGizmo.SelectedSnapPoint != null)
                        {
                            var secondSnapPoint = MyEditorGizmo.GetSnapPoint(false);
                            if (secondSnapPoint != null)
                            {
                                if (!CanLinkSnapPoints(MyEditorGizmo.SelectedSnapPoint, secondSnapPoint))
                                {
                                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SnapPointLinkError, MyTextsWrapperEnum.LinkSnapPoints, MyTextsWrapperEnum.Ok, null));
                                }
                                else
                                {
                                    AttachSnapPoints(MyEditorGizmo.SelectedSnapPoint, secondSnapPoint, true);
                                    LinkSnapPoints();
                                }
                            }
                        }
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Select all objects
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsAnyControlPress() && input.IsNewKeyPress(Keys.A) && IsAnyEntityAttachedToCamera() == false)
                {
                    SelectAllObjects();
                }
                else if (input.IsAnyControlPress() && input.IsNewKeyPress(Keys.D) && IsAnyEntityAttachedToCamera() == false)
                {
                    MyEditorGizmo.ClearSelection();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Change distance of attached objects from camera
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsAnyControlPress() && IsAnyEntityAttachedToCamera())
                {
                    m_mouseMoveInputTool.EnableInputHandling(false);
                    if (m_lastScrollWheelValue > input.MouseScrollWheelValue() && m_scrollWheelDistance > 1)
                    {
                        m_scrollWheelDistance -= 1;

                    }
                    else if (m_lastScrollWheelValue < input.MouseScrollWheelValue() && m_scrollWheelDistance < 50)
                    {
                        m_scrollWheelDistance += 1;
                    }
                    m_lastScrollWheelValue = input.MouseScrollWheelValue();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Switch orientation of the grid on selected prefab
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //if (input.IsEditorControlNewPressed(MyEditorControlEnums.SWITCH_GIZMO_MODE))
                //{
                //    MyEditorGrid.SwitchGridOrientation();
                //}

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Commit prefab container
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsNewKeyPress(Keys.Enter))
                {
                    CommitPrefabContainerChanges();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Remove selected object
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsNewKeyPress(Keys.Delete))
                {
                    RemoveEntities(MyEditorGizmo.SelectedEntities);
                    ClearAttachedEntities();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Toggle snap points
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (input.IsNewKeyPress(Keys.P))
                {
                    if (ShowSnapPoints && MyEditorGizmo.SelectedSnapPoint != null)
                    {
                        MyEditorGizmo.SelectedSnapPoint = null;
                    }
                    ShowSnapPoints = !ShowSnapPoints;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Disable/Enable lighting in the editor
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //if (input.IsNewKeyPress(Keys.F3))
            if (input.IsNewKeyPress(Keys.Y) && input.IsAnyControlPress())
            {
                MyEditor.EnableLightsInEditor = !MyEditor.EnableLightsInEditor;
            }

            /*
            if (input.IsNewKeyPress(Keys.N) && input.IsAnyControlPress())
            {
                MyMwcObjectBuilder_VoxelMap voxelMapObjectBuilder = new MyMwcObjectBuilder_VoxelMap(new Vector3(0, 0, 0), MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128, MyMwcVoxelMaterialsEnum.Stone_01);

                for (int i = 0; i < 4800; i++)
                {
                    voxelMapObjectBuilder.VoxelHandShapes.Add(new MyMwcObjectBuilder_VoxelHand_Sphere(new MyMwcPositionAndOrientation(Vector3.Zero, Vector3.Forward, Vector3.Up), 50, MyMwcVoxelHandModeTypeEnum.ADD));
                }
                MyEntities.CreateFromObjectBuilderAndAdd(null, voxelMapObjectBuilder, Matrix.Identity);
            }
            */
        

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Changes editor spectator movement speed
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (m_lastScrollWheelValue != input.MouseScrollWheelValue() &&
                !(input.IsAnyControlPress() && IsAnyEntityAttachedToCamera()) &&
                !((input.IsAnyControlPress() || input.IsAnyShiftPress()) && MyEditorVoxelHand.IsEnabled()))
            {
                if (m_lastScrollWheelValue < input.MouseScrollWheelValue() && MySpectator.SpeedMode < MyEditorConstants.MAX_EDITOR_CAMERA_MOVE_MULTIPLIER)
                {
                    m_scrollWheelIndicatorHelper += 0.5;
                }
                else if (m_lastScrollWheelValue > input.MouseScrollWheelValue() && MySpectator.SpeedMode > MyEditorConstants.MIN_EDITOR_CAMERA_MOVE_MULTIPLIER)
                {
                    m_scrollWheelIndicatorHelper -= 0.5;
                }
                MySpectator.SpeedMode = (float)Math.Round(Math.Pow(2, m_scrollWheelIndicatorHelper), 2);

                m_lastScrollWheelValue = input.MouseScrollWheelValue();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Changes GRID step size
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (input.IsEditorControlNewPressed(MyEditorControlEnums.INCREASE_GRID_SCALE))
            {
                MyEditorGrid.IncreaseGridStep();
            }

            if (input.IsEditorControlNewPressed(MyEditorControlEnums.DECREASE_GRID_SCALE))
            {
                MyEditorGrid.DecreaseGridStep();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Show Editor help
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (input.IsNewKeyPress(Keys.F1) && !input.IsAnyCtrlKeyPressed())
            {
                MyGuiManager.AddScreen(new MyGuiScreenEditorHelp());
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Temporary for undo action
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (!MyFakes.EDITOR_DISABLE_UNDO_REDO && input.IsAnyControlPress() && input.IsNewKeyPress(Keys.Z))
            {
                MyEditorActions.Undo();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Temporary for redo action
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (!MyFakes.EDITOR_DISABLE_UNDO_REDO && input.IsAnyControlPress() && input.IsNewKeyPress(Keys.R))
            {
                MyEditorActions.Redo();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Edit entity inventory
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (input.IsNewKeyPress(Keys.F5) && MyEditorGizmo.IsOnlyOneEntitySelected() && MyEditorGizmo.SelectedEntities[0] is IMyInventory)
            {
                MyGuiManager.AddScreen(new MyGuiScreenEntityWithInventory(MyEditorGizmo.SelectedEntities[0]));
            }

            if (!(MyGuiScreenGamePlay.Static.IsIngameEditorActive() && GetEditedPrefabContainer() == null) && IsBackgroundWorkThreadNotAlive())
                m_mouseMoveInputTool.HandleInput(input);
        }

        
        private static MyGuiScreenMessageBox.MessageBoxCallback m_connectAllSelectedWaypointsCallback = new MyGuiScreenMessageBox.MessageBoxCallback(ConnectAllSelectedWaypointsCallback);
        private static void ConnectAllSelectedWaypointsCallback(MyGuiScreenMessageBoxCallbackEnum r)
        {
            if (r == MyGuiScreenMessageBoxCallbackEnum.YES)
                MyWayPoint.ConnectAll(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities));
        }

        private static MyGuiScreenMessageBox.MessageBoxCallback m_resetAllWaypointsInPrefabsCallback = new MyGuiScreenMessageBox.MessageBoxCallback(ResetAllWaypointsInPrefabsCallback);
        private static void ResetAllWaypointsInPrefabsCallback(MyGuiScreenMessageBoxCallbackEnum r)
        {
            if (r == MyGuiScreenMessageBoxCallbackEnum.YES)
                MyWayPointGraph.ReloadAllWaypointsWithParents();
        }

        public void CorrectSnappedPrefabs()
        {
            var alreadyCorrected = new HashSet<MyEntity>();
            foreach (var link in LinkedSnapPoints.ToList())
            {
                var entity = link.First().SnapPoint.Prefab;
                if (!alreadyCorrected.Contains(entity))
                {
                    var result = new List<MyEntity>();
                    FixLinkedEntities(entity, result, null);
                    foreach (var e in result)
                        alreadyCorrected.Add(e);
                }
            }
        }

        public void EnterInGameEditMode(MyPrefabFoundationFactory prefabFoundationFactory)
        {
            //MyPrefabContainer container = (MyPrefabContainer) GetCurrentlyLoggedUsersPrefabContainer();            
            MyGuiContextMenuHelpers.LoadInGameEditorContent();
            //m_activePrefabContainer = foundationFactory.PrefabContainer;
            //MyGuiScreenGamePlay.Static.EditorControls.SetEditorMode(true);                        
            //MyGuiScreenGamePlay.Static.FoundationFactoryControls.Init(foundationFactory);
            FoundationFactory = prefabFoundationFactory;            
            MyGuiScreenGamePlay.Static.EditorControls.LoadEditorData();            
            /*
            if (container == null)
            {
                List<MyMwcObjectBuilder_Prefab> prefabBuilders = new List<MyMwcObjectBuilder_Prefab>();
                //prefabBuilders.Add(prefabBuilder);
                MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = new MyMwcObjectBuilder_PrefabContainer(
                    new MyMwcPositionAndOrientation(MySpectator.Target + 100 * MySpectator.Orientation, Vector3.Forward, Vector3.Up),
                    null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabBuilders, MyClientServer.LoggedPlayer.GetUserId());
                MyEditor.Static.CreateFromObjectBuilder(prefabContainerBuilder);
                container = (MyPrefabContainer)GetCurrentlyLoggedUsersPrefabContainer();
            }
            */

            //EditPrefabContainer(container);
            EditPrefabContainer(prefabFoundationFactory.PrefabContainer);

        }

        public void ExitInGameEditMode()
        {
            //ExitActivePrefabContainer();
            MyGuiContextMenuHelpers.LoadDefaultEditorContent();                        
            MyGuiScreenGamePlay.Static.EditorControls.UnloadEditorData();
            FoundationFactory = null;
            //MyGuiScreenGamePlay.Static.EditorControls.SetEditorMode(false);            
        }


        /// <summary>
        /// Method switches editor into container edit mode
        /// </summary>
        /// <param name="container"></param>
        public void EditPrefabContainer(MyPrefabContainer container)
        {
            if (container != null)
            {
                if (container.IsEditingActive() == false)
                {
                    MyEditorGizmo.ClearSelection();
                    container.SwitchEditMode();
                    m_activePrefabContainer = container;
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorPrefabEnter);
                    m_activePrefabContainer.SetEditState(true);
                }
            }
        }

        /// <summary>
        /// Method exits from container editing mode
        /// </summary>
        public void ExitActivePrefabContainer()
        {
            if (m_activePrefabContainer != null)
            {

                MyEditorGizmo.ClearSelection();
                m_activePrefabContainer.SwitchEditMode();
                MyPrefabContainer setSelectedContainer = m_activePrefabContainer;
                ResetActivePrefabContainer();
                MyEditorGizmo.AddEntityToSelection(setSelectedContainer);
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorPrefabExit);
            }
        }

        /// <summary>
        /// When commiting changes to container, check if there are no collisions of prefabs with other non-container objects
        /// </summary>
        private void CommitPrefabContainerChanges()
        {
            if (IsEditingPrefabContainer())
            {
                MyEditorGizmo.ClearSelection();
                //m_activePrefabContainer.CommitAndReload();
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorPrefabCommit);
            }
        }

        /// <summary>
        /// Select all objects in sector
        /// </summary>
        private void SelectAllObjects()
        {
            if (MyGuiScreenGamePlay.Static.IsIngameEditorActive() && GetEditedPrefabContainer() == null)
                return;

            MyEditorGizmo.ClearSelection();

            if (IsEditingPrefabContainer() == false)
            {
                List<MyEntity> allEntities = MyEntities.GetEntities().ToList();
                MyEditorGizmo.AddEntitiesToSelection(allEntities);
            }
            else
            {
                MyEditorGizmo.AddEntitiesToSelection(m_activePrefabContainer.GetPrefabs());
            }
        }

        /// <summary>
        /// Create selection rectangle
        /// </summary>
        /// <param name="input"></param>
        private void CreateSelectionRectangle(MyGuiInput input)
        {
            // Do not allow to create selection rectangle while using transformation gizmo for object transformation
            if (MyEditorGizmo.TransformationActive)
                return;

            Vector2 screenMouseCursorCoord = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition);
            if (input.WasEditorControlReleased(MyEditorControlEnums.PRIMARY_ACTION_KEY))
            {
                // Set the selection box starting point
                m_selectionRectangle = new MinerWarsMath.Rectangle((int)screenMouseCursorCoord.X, (int)screenMouseCursorCoord.Y, 0, 0);

                // We want the selection box to be relative to the viewport we're using
                m_selectionRectangle.Y -= MyCamera.Viewport.Y;
                m_selectionRectangle.X -= MyCamera.Viewport.X;
            }
            else
            {
                if (!MyConfig.EditorUseCameraCrosshair)
                {
                    // Calculate new width,height
                    m_selectionRectangle.Width = ((int)screenMouseCursorCoord.X) - m_selectionRectangle.X;
                    m_selectionRectangle.Height = ((int)screenMouseCursorCoord.Y) - m_selectionRectangle.Y;

                    // We want the selection box to be relative to the viewport we're using
                    m_selectionRectangle.Width -= MyCamera.Viewport.X;
                    m_selectionRectangle.Height -= MyCamera.Viewport.Y;

                    MyEditorGizmo.Enabled = false;
                }
                else
                {
                    if (m_startCaptureCrosshairPosition)
                    {
                        m_startLineCrosshairSelection = MyUtils.ConvertMouseToLine();
                        m_startCaptureCrosshairPosition = false;
                    }
                    m_endLineCrosshairSelection = MyUtils.ConvertMouseToLine();

                    Vector3 sp = SharpDXHelper.ToXNA(MyCamera.ForwardViewport.Project(SharpDXHelper.ToSharpDX(m_startLineCrosshairSelection.From + m_startLineCrosshairSelection.Direction),
                        SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)));

                    Vector3 ep = SharpDXHelper.ToXNA(MyCamera.ForwardViewport.Project(SharpDXHelper.ToSharpDX(m_endLineCrosshairSelection.From + m_endLineCrosshairSelection.Direction),
                        SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)));

                    m_selectionRectangle.X = (int)sp.X;
                    m_selectionRectangle.Y = (int)sp.Y;
                    m_selectionRectangle.Width = (int)(ep.X - sp.X);
                    m_selectionRectangle.Height = (int)(ep.Y - sp.Y);
                }
            }
        }

        /// <summary>
        /// Get bounding sphere containing all selected entities
        /// </summary>
        void GetSelectedEntitiesBoundingSphere(out BoundingSphere sphere)
        {
            Vector3 relativeToPosition = Vector3.Zero;
            Vector3 center = MyEditorGizmo.GetSelectedObjectsCenter();
            sphere.Center = center;

            /** When attaching multiple objects, we create one bounding sphere having center where
             *  all objects center is and radius that reaches object most far from center of the selection.
             *  This is used when calculating distance of objects from camera, when attaching them to camera
             */
            float radius = MyEditorGizmo.GetFirstSelected().WorldVolume.Radius;

            foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
            {
                float distance = Vector3.Distance(entity.GetPosition(), center) + entity.WorldVolume.Radius;
                if (distance > radius) radius = distance;
            }

            sphere.Radius = radius;
        }

        /// <summary>
        /// This method will set working objects - attached to the camera(so you can fly around and take objects with you anywhere you want)
        /// </summary>
        public void AttachSelectedObjectsToCamera()
        {
            ClearAttachedEntities();

            //Makes no sense to attach objects, when no object is selected
            if (MyEditorGizmo.IsAnyEntitySelected())
            {
                GetSelectedEntitiesBoundingSphere(out m_attachedEntitiesBoundingSphere);

                foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
                {
                    m_attachedEntities.Add(entity, entity.GetPosition() - m_attachedEntitiesBoundingSphere.Center);
                }

                MyEditorGizmo.StartTransformationData();
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectAttach);

            }
        }

        /// <summary>
        /// This method is used for detaching objects from camera and leaving them on new position(if it is valid)
        /// </summary>
        public void DetachAndConfirm()
        {
            //by clearing working phys objects, we also detach them from camera and leave them on place under cursor
            ClearAttachedEntities();

            MyEditorGizmo.TransformationActive = false;
            MyEditorGizmo.EndTransformationData();

            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectDetach);
        }

        /// <summary>
        /// Checks if entity is coliding and not selected by editor or not (by searching up in collision list) and changes its highlighting
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="areTheyColiding"></param>
        public void ChangeHighlightingForEntity(MyEntity entity, bool areTheyColiding)
        {
            if (areTheyColiding)
            {
                entity.SetCollisionHighlighting(ref MyEditorConstants.COLLIDING_OBJECT_DIFFUSE_COLOR_ADDITION);
            }
            else
            {
                Vector3 zero = new Vector3(0, 0, 0);
                entity.SetCollisionHighlighting(ref zero);
            }
        }

        private static void SetAllObjectsPhysicsState(bool physState, bool resetState)
        {
            foreach (MyEntity enti in (MyEntities.GetEntities()))
            {
                if (enti.Physics != null)
                {
                    if (enti.Physics.Enabled != physState)
                        enti.Physics.Enabled = physState;
                    /*
                    if(resetState)
                    {
                        for( MyRBElement rb in (ent.Physics.Type as MyRigidBody).GetRBElementList() )
                            rb.
                    }
                    */
                }
            }
        }

        public static void DisablePhysicsAndResetStatesOnAllObjectsInSector()
        {
            SetAllObjectsPhysicsState(false, true);
        }

        public static void EnablePhysicsOnAllActiveObjectsInSector()
        {
            SetAllObjectsPhysicsState(true, false);
        }

        public void ClearCollidingElements()
        {
            m_collidingElements.Clear();
        }

        /// <summary>
        /// Tests if entity is in any collision with world, sets theirs higlighting and adds them into list of colliding entities
        /// </summary>
        /// <param name="entity"></param>
        public void CheckAllCollidingObjectsForEntity(MyEntity entity)
        {
            if (entity.Physics == null)
                return;

            /*if (entity is MyLargeShipGunBase)
                return;*/            

            if (entity is MyLargeShipGunBase || entity is MyLargeShipGunBase || entity is MyPrefabLight || !entity.VisibleInGame)
            {
                MyEntities.CollisionsElements.Clear();
                return;
            }            

            MyPhysicsBody rBody = entity.Physics;
            foreach (MyRBElement element in rBody.GetRBElementList())
            {
                MyEntities.CollisionsElements.Clear();
                MyEntities.GetCollisionListForElement(element);
                bool colliding = MyEntities.CollisionsElements.Count > 0;
                MyEntities.CollisionsElements.Add(element);
                foreach (MyRBElement colidingElement in MyEntities.CollisionsElements)
                {
                    MyEntity tmp = (colidingElement.GetRigidBody().m_UserData as MyPhysicsBody).Entity;
                    if (tmp is MyPrefabLargeWeapon || tmp is MyLargeShipGunBase || tmp is MyPrefabLight)
                        break;

                    if (colliding && !m_collidingElements.Contains(colidingElement))
                        m_collidingElements.Add(colidingElement);
                    ChangeHighlightingForEntity(tmp, colliding);
                }
            }
        }

        /// <summary>
        /// Rechecks colliding entities and clears non coliding
        /// </summary>
        public void RecheckAllColidingEntitesAndClearNonColiding()
        {
            for (int c = m_collidingElements.Count; c-- != 0; )
            {
                MyEntities.CollisionsElements.Clear();
                MyEntities.GetCollisionListForElement(m_collidingElements[c]);
                if (MyEntities.CollisionsElements.Count == 0)
                {
                    ChangeHighlightingForEntity((m_collidingElements[c].GetRigidBody().m_UserData as MyPhysicsBody).Entity, false);
                    m_collidingElements.Remove(m_collidingElements[c]);
                }
            }
        }

        //public void DeleteAllFromCollidingList()
        //{
        //    m_collidingElements.Clear();
        //}
        /// <summary>
        /// Removes entity from list of colliding entities
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntityFromCollidingList(MyEntity entity)
        {
            foreach (MyEntity subchild in entity.Children)
                DeleteEntityFromCollidingList(subchild);
            if (entity.Physics != null)
            {
                foreach (MyRBElement element in (entity.Physics as MyPhysicsBody).GetRBElementList())
                {
                    if (m_collidingElements.Contains(element))
                    {
                        m_collidingElements.Remove(element);
                        ChangeHighlightingForEntity(entity, false);
                    }
                }
            }
        }

        /// <summary>
        /// Tests if is editor selected entities in any collision with world and sets theirs higlighting and adds them into list of colliding entities
        /// </summary>
        public void CheckAllCollidingObjectsInSelection()
        {
            foreach (MyEntity enityinselect in MyEditorGizmo.SelectedEntities)
            {
                CheckAllCollidingObjectsForEntity(enityinselect);
            }
        }

        /// <summary>
        /// Tests if any entity is in collision state
        /// </summary>
        public void CheckAllCollidingObjects()
        {
            MyEditor.Static.ClearCollidingElements();
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                if (entity.Physics != null && entity.Physics.Enabled)
                {
                    CheckAllCollidingObjectsForEntity(entity);
                }
            }
        }

        /// <summary>
        /// Check entity child collisions
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CheckForEntityChildsCollisions(MyEntity entity)
        {
            if (entity.Physics == null)
                return false;
            foreach (MyRBElement element in (entity.Physics as MyPhysicsBody).GetRBElementList())
            {
                if (m_collidingElements.Contains(element))
                    return true;
            }

            IEnumerable<MyEntity> allSubentites = entity.EnumChildren(EnumerationFlags.Hierarchically);
            foreach (MyEntity childEntity in allSubentites)
            {
                return CheckForEntityChildsCollisions(childEntity);
            }
            return false;
        }

        /// <summary>
        /// Highlights all objects that collides with supplied entities
        /// </summary>
        /// <param name="selectedEntities"></param>
        public void HighlightCollisions(List<MyEntity> selectedEntities)
        {
            MyEntities.ClearCollisionHighlights();

            foreach (MyEntity entity in selectedEntities)
            {
                CheckAllCollidingObjectsForEntity(entity);
            }
        }

        /// <summary>
        /// Perform actions required when switching to editor during gameplay
        /// </summary>
        void SwitchToGameplay()
        {
            MyEditorGizmo.SelectedSnapPoint = null;
            MyEditorGizmo.ClearSelection();
            MyEntities.ClearHighlights();
            MyEntities.ClearCollisionHighlights();
            MyEntities.UnhideAllTypes();
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            //ClearWholeSector();
            //SaveSector();
            /*  If there is background work thread running in editor, make sure to call update and draw only on NECESSARY components
             *  because, during it, player sees only progress box and can't do anything
             */
            if (IsBackgroundWorkThreadNotAlive()) StopBackgroundWorkThread();
                 /*
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyPrefabContainer container = entity as MyPrefabContainer;

                if (container != null)
                {
                    foreach (MyPrefabBase prefab in container.GetPrefabs())
                    {
                        MyPrefabHangar hangar = prefab as MyPrefabHangar;
                        if (hangar != null)
                        {
                            MinerWars.AppCode.Game.World.Global.MyFactionRelationEnum rel = MinerWars.AppCode.Game.World.Global.MyFactions.GetFactionsRelation(hangar.Faction, MySession.PlayerShip.Faction);
                            if (rel != World.Global.MyFactionRelationEnum.Neutral)
                            {
                                container.Faction = MyMwcObjectBuilder_FactionEnum.TTLtd;
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
             */
                /*  
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MySpawnPoint sp = entity as MySpawnPoint;
                if (sp != null)
                {
                    foreach (var tm in sp.GetBotTemplates())
                    {
                        string name = tm.m_name;
                    }

                    foreach (var bot in sp.GetBots())
                    {
                        var st =  bot.Builder.ShipType;
                        if (st == MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR)
                        {
                            var fc = sp.Faction;
                        }

                        if (sp.Faction != MyMwcObjectBuilder_FactionEnum.Euroamerican)
                        {
                            if ((st != MyMwcObjectBuilder_SmallShip_TypesEnum.YG) &&
                                (st != MyMwcObjectBuilder_SmallShip_TypesEnum.ORG) &&
                                (st != MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV) &&
                                (st != MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD))
                            {
                            }
                        }

                    }
                }
            }        */
                 /*
            foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
            {
            }
               */

            // Do update editor only if sure that background thread is stopped completely
            if (BackgroundWorkThread == null)
            {
                if (MyGuiManager.IsDebugScreenEnabled() == false) MyGuiManager.SwitchDebugScreensEnabled();

                MyEditorGrid.Update();

                if (MyGuiScreenEditorProgress.CurrentScreen != null)
                {
                    MyGuiScreenEditorProgress.CurrentScreen.CloseScreen();
                }


                //make flashing effect of selected entites and its children (depends if entities are in collision or not)

                bool playCollisionCue = false;

                if (!IsBackgroundWorkThreadAlive())
                {
                    foreach (MyRBElement colidingRBElementObject in m_collidingElements)
                    {
                        MyEntity colEntity = (colidingRBElementObject.GetRigidBody().m_UserData as MyPhysicsBody).Entity;
                        if (EditorFlashingCanHighlightEntity())
                        { //half sec interval
                            colEntity.SetCollisionHighlighting(ref MyEditorConstants.COLLIDING_OBJECT_DIFFUSE_COLOR_ADDITION);
                            playCollisionCue = true;
                        }
                        else
                        {
                            Vector3 zero = new Vector3(0, 0, 0);
                            colEntity.SetCollisionHighlighting(ref zero);
                            playCollisionCue = false;
                        }
                        //if (MyEditorGizmo.IsEntityOrItsParentSelected(colEntity))
                        //{
                        //    if (EditorFlashingCanHighlightEntity())
                        //    { //half sec interval
                        //        colEntity.SetCollisionHighlighting(ref MyEditorConstants.COLLIDING_OBJECT_DIFFUSE_COLOR_ADDITION);
                        //        playCollisionCue = true;
                        //    }
                        //    else
                        //    {
                        //        Vector3 zero = new Vector3(0, 0, 0);
                        //        colEntity.SetCollisionHighlighting(ref zero);
                        //        playCollisionCue = false;
                        //    }
                        //}
                        //else
                        //{
                        //    //does not work properly if i set highliting to one of childs it doesnt render highlighted
                        //    colEntity.SetCollisionHighlighting(ref MyEditorConstants.COLLIDING_OBJECT_DIFFUSE_COLOR_ADDITION);
                        //}
                    }
                }



                if (playCollisionCue)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                }

                //show message box, that max limit of voxel maps in sector has been reached
                if (m_newAsteroidAllowed == false)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxVoxelMapsLimitReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    m_newAsteroidAllowed = true;
                }

                if (m_waitingForAtachment)
                {
                    AttachSelectedObjectsToCamera();
                    m_waitingForAtachment = false;
                }

                // When objects are attached to camera, move them with camera
                if (IsAnyEntityAttachedToCamera())
                {
                    // During attached mode, gizmo is not usefull, thus disable it for this moment
                    MyEditorGizmo.Enabled = false;

                    // For getting optimal position of attached objects in front of camera, use radius of bounding sphere calculated for all attached objects.
                    // Player can also modify distance from camera using combination of ctrl + mousewheel buttons
                    float radius = IsAnyEntityAttachedToCamera() ? m_attachedEntitiesBoundingSphere.Radius : m_attachedEntitiesBoundingSphere.Radius * 2;
                    Vector3 direction = MyCamera.ForwardVector * m_scrollWheelDistance * radius;


                    List<MyEntity> transformedEntities = new List<MyEntity>();

                    foreach (KeyValuePair<MyEntity, Vector3> kvp in m_attachedEntities)
                    {
                        // skip already transformed linked entities
                        if (transformedEntities.Contains(kvp.Key))
                        {
                            continue;
                        }

                        MyEntity attachedEntity = kvp.Key;
                        Vector3 relativePositionToSphereCenter = kvp.Value;
                        Matrix orientation = attachedEntity.GetWorldRotation();
                        m_attachedEntitiesBoundingSphere.Center = MySpectator.Position + direction;

                        if (attachedEntity is MyPrefabBase && MyEditorGizmo.SnapEnabled)
                        {
                            //when moving with RMB, move over the grid based on currently selected grid step size
                            m_attachedEntitiesBoundingSphere.Center.X = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(m_attachedEntitiesBoundingSphere.Center.X / MyEditorGrid.GetGridStepInMeters());
                            m_attachedEntitiesBoundingSphere.Center.Y = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(m_attachedEntitiesBoundingSphere.Center.Y / MyEditorGrid.GetGridStepInMeters());
                            m_attachedEntitiesBoundingSphere.Center.Z = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(m_attachedEntitiesBoundingSphere.Center.Z / MyEditorGrid.GetGridStepInMeters());
                        }

                        // Take care of moving objects with camera
                        Vector3 position = m_attachedEntitiesBoundingSphere.Center + relativePositionToSphereCenter;
                        MyEditorGizmo.MoveAndRotateObject(position, orientation, attachedEntity);
                        MyEditor.Static.FixLinkedEntities(attachedEntity, transformedEntities, null);
                    }
                }
                else
                {
                    // When nothing attached, gizmo can be enabled again, but only in case something is selected
                    if (MyEditorGizmo.IsAnyEntitySelected())
                    {
                        MyEditorGizmo.Enabled = true;
                    }
                    else
                    {
                        MyEditorGizmo.Enabled = MyEditorGizmo.SelectedSnapPoint != null;
                    }
                }


                // Play sound when sector border reached 
                if (((Math.Abs(MySpectator.Position.X) == MyMwcSectorConstants.SECTOR_SIZE_HALF) ||
                    (Math.Abs(MySpectator.Position.Y) == MyMwcSectorConstants.SECTOR_SIZE_HALF) ||
                    (Math.Abs(MySpectator.Position.Z) == MyMwcSectorConstants.SECTOR_SIZE_HALF)) && MyMinerGame.TotalGamePlayTimeInMilliseconds % MyEditorConstants.SECTOR_BORDER_REACHED_WARNING_DELAY == 0)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorFlyOutsideBorder);
                }

                MyEditorGizmo.Update();
                MyEditorVoxelHand.Update();
            }
            else
            {
                if (MyGuiManager.IsDebugScreenEnabled() == true) MyGuiManager.SwitchDebugScreensEnabled();
            }

            if (m_issueCheckAllCollidingObjectsCounter == 0)
            {
                MyEditor.Static.CheckAllCollidingObjects();
            }
            --m_issueCheckAllCollidingObjectsCounter;
        }

        private bool EditorFlashingCanHighlightEntity()
        {
            return MyMinerGame.TotalTimeInMilliseconds % MyEditorConstants.DELAY_FOR_COLLISION_TIME_FLASHING_IN_MILLIS > MyEditorConstants.DELAY_FOR_COLLISION_TIME_FLASHING_IN_MILLIS / 2;
        }

        #endregion

        #region Draw Methods
        /// <summary>
        /// Load entity models during draw
        /// </summary>
        public void LoadInDraw()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor::LoadInDraw");

            if (ReloadModelsInDrawRequired)
            {
                MyEntities.ReloadModelsInDraw();
                ReloadModelsInDrawRequired = false;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="bEditorActive"></param>
        public void Draw(bool bEditorActive)
        {
            MyEditorGrid.Draw();
            //@ 844
            if (bEditorActive && !IsBackgroundWorkThreadAlive())
                MyPrefabContainerManager.GetInstance().DrawContainersBoundingArea();


            if (!IsBackgroundWorkThreadAlive())
            {
                //draw all colliding objects
                foreach (MyRBElement elementToDraw in m_collidingElements)
                {
                    Color color = new Color(new Vector4(0.9f, 0.1f, 0.1f, 0.7f));

                    if (EditorFlashingCanHighlightEntity())
                    {
                        color = new Color(new Vector4(0.7f, 0.1f, 0.1f, 0.4f));                        
                    }
                    Vector4 colorVec = color.ToVector4();

                    if (elementToDraw is MyRBBoxElement || elementToDraw is MyRBTriangleMeshElement || elementToDraw is MyRBVoxelElement)
                    {
                        MyEntity entity = (elementToDraw.GetRigidBody().m_UserData as MyPhysicsBody).Entity;
                        DrawEntityAxes(entity, colorVec);

                        BoundingBox localAABB = entity.WorldAABB;  

                        Vector3 size = localAABB.Max - localAABB.Min;
                        Vector3 center = size / 2f;
                        center = center + localAABB.Min;

                        Matrix tmpInv = Matrix.Identity;
                        tmpInv.Translation = -center;
                        BoundingBox localbox = localAABB.Transform(tmpInv);    //to local

                        Matrix mat = Matrix.CreateTranslation(entity.LocalVolumeOffset) * entity.WorldMatrix;
                        //MySimpleObjectDraw.DrawTransparentBox(ref mat, ref localbox, ref colorVec, false, 1);

                    }

                    if (elementToDraw is MyRBSphereElement)
                    {
                        MyEntity entity = (elementToDraw.GetRigidBody().m_UserData as MyPhysicsBody).Entity;
                        float radi = (elementToDraw as MyRBSphereElement).Radius;

                        Matrix mat = Matrix.CreateTranslation(entity.LocalVolumeOffset) * entity.WorldMatrix;
                        //MySimpleObjectDraw.DrawTransparentSphere(ref mat, radi, ref colorVec, true, 24);
                    }
                }
            }

            // Draw prefab snap points
            if (ShowSnapPoints && bEditorActive)
            {
                DrawSnapPoints();
            }
        }

        private void DrawEntityAxes(MyEntity entity, Vector4 color) 
        {
            float sectorHalfSize = -MyMwcSectorConstants.SECTOR_SIZE / 2f;
            Vector3 entityPos = entity.GetPosition();
            Vector3 xAxisStart = new Vector3(sectorHalfSize, entityPos.Y, entityPos.Z);
            Vector3 yAxisStart = new Vector3(entityPos.X, sectorHalfSize, entityPos.Z);
            Vector3 zAxisStart = new Vector3(entityPos.X, entityPos.Y, sectorHalfSize);

            // x axis
            MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, color, xAxisStart, Vector3.Right, MyMwcSectorConstants.SECTOR_SIZE, 5f);
            // y axis
            MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, color, yAxisStart, Vector3.Up, MyMwcSectorConstants.SECTOR_SIZE, 5f);
            // z axis
            MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, color, zAxisStart, Vector3.Backward, MyMwcSectorConstants.SECTOR_SIZE, 5f);
        }

        private void DrawSnapPoints()
        {
            var inverseViewMatrix = Matrix.Invert(MyCamera.ViewMatrix);
            var snapSize = MyPrefabSnapPoint.GetFixedSnapSize();
            var screenScale = 2.0f * snapSize * (float)Math.Tan(MyCamera.Zoom.GetFOV() / 2) / MyMinerGame.ScreenSize.Y;
            var container = GetEditedPrefabContainer();
            if (container != null)
            {
                foreach (var child in container.Children)
                {
                    var prefab = child as MyPrefabBase;
                    if (prefab != null)
                    {
                        foreach (var snapPoint in prefab.SnapPoints)
                        {
                            if (!snapPoint.Visible)
                            {
                                continue;
                            }

                            if (MyEditorGizmo.SelectedSnapPoint != null &&
                                snapPoint != MyEditorGizmo.SelectedSnapPoint &&             // Draw selected snap point
                                !MyEditorGizmo.SelectedSnapPoint.CanAttachTo(snapPoint))
                            {
                                continue;
                            }

                            var link = GetSnapPointLink(snapPoint);

                            Vector4 color = Vector4.One;
                            int priority = 2;
                            if (snapPoint == MyEditorGizmo.SelectedSnapPoint)
	                        {
                                color = MyGuiConstants.SNAP_POINT_SELECTED_COLOR;
                                priority = 4;
	                        }
                            else if (link != null)    // Linked snap point
	                        {
                                color = link.Count > 2 ? MyGuiConstants.SNAP_POINT_MULTI_LINKED_COLOR : MyGuiConstants.SNAP_POINT_LINKED_COLOR;
                                priority = 2;
	                        }

                            Vector3 snapPosition = Vector3.Transform(snapPoint.Matrix.Translation, prefab.WorldMatrix);

                            float billboardSize;
                            if (FixedSizeSnapPoints)
                            {
                                float cameraDistance = Math.Abs(Vector3.Dot(inverseViewMatrix.Forward, snapPosition - MyCamera.Position));
                                billboardSize = screenScale * cameraDistance;
                            }
                            else
                            {
                                billboardSize = MyPrefabSnapPoint.GetRealSnapSize();
                            }

                            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.snap_point, color, snapPosition,
                                inverseViewMatrix.Left, inverseViewMatrix.Up, billboardSize, priority);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// This method is responsible for drawing or invoking draw of visual controls or info, like helper texts, selection rectangle and objects bounding
        /// </summary>
        public void DrawVisualControls()
        {
            // Not need to draw this while background thread is running
            if (IsBackgroundWorkThreadAlive() == false)
            {
                MyEditorGizmo.Draw();
                //MyEditorVoxelHand.Draw();
                //m_mouseMoveInputTool.get
                MyEntity moEntity = m_mouseMoveInputTool.GetMouseOverEntity();
                if (GetEditedPrefabContainer() != null)
                {
                    if (moEntity is MyPrefabBase)
                    {
                        if (moEntity.Parent == GetEditedPrefabContainer())
                        {
                            m_mouseMoveInputTool.DrawMouseOver();
                        }
                    }
                }
                else
                    m_mouseMoveInputTool.DrawMouseOver();
            }
        }

        /// <summary>
        /// This method takes care of drawing rectangle, that is drawn by dragging mouse to select multiple objects inside rectangle
        /// </summary>
        private void DrawSelectionRectangle()
        {
            if (IsSelectingByRectangle())
            {
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Top), new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Top), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Top), new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Bottom), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Bottom), new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Bottom), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Bottom), new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Top), Color.Green, Color.Green);
            }
        }


        /// <summary>
        /// DrawSelectedBounding
        /// </summary>
        private void DrawSelectedBounding()
        {
            //0001662: BUG C - Editor - unselected bounding box of object get lost after copying object
            if (false && MyConfig.EditorDisplayUnselectedBounding)
            {
                Vector4 color = new Vector4(0.6f, 0.6f, 0.6f, 0.2f);
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    entity.DebugDrawBox(color, false);
                }

                foreach (MyVoxelMap voxelMap in MyVoxelMaps.GetVoxelMaps())
                {
                    MyDebugDraw.DrawHiresBoxWireframe(Matrix.CreateScale(voxelMap.LocalAABB.Max - voxelMap.LocalAABB.Min) * voxelMap.WorldMatrix, new Vector3(0, 0.4f, 0), 0.3f);
                }
            }
                
            /* We dont want sel.box for prefab container atm
            if (IsEditingPrefabContainer())
            {
                m_activePrefabContainer.DrawSelectionBox(new Vector4(1, 1, 0, 0.4f));
            } 

            foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
            {
                if (entity is MyPrefabContainer)
                {
                    MyPrefabContainer container = (MyPrefabContainer)entity;
                    container.DrawSelectionBox(new Vector4(0, 1, 0, 0.4f));
                }
            } */
        }

        #endregion

        #region Background Thread Methods

        /// <summary>
        /// Start background thread
        /// </summary>
        /// <param name="thread"></param>
        public void StartBackgroundThread(Thread thread)
        {
            System.Diagnostics.Debug.Assert(BackgroundWorkThread == null || !BackgroundWorkThread.IsAlive);

            MyRender.Enabled = false;
            BackgroundWorkThread = thread;
            BackgroundWorkThread.IsBackground = true;
            BackgroundWorkThread.Start();
        }

        /// <summary>
        /// When background thread has finished its work, call this method to stop it
        /// </summary>
        public void StopBackgroundWorkThread()
        {
            BackgroundWorkThread.Abort();
            BackgroundWorkThread.Join();
            BackgroundWorkThread = null;
            MyRender.Enabled = true;
        }

        /// <summary>
        /// Check if background thread is alive
        /// </summary>
        /// <returns></returns>
        public bool IsBackgroundWorkThreadAlive()
        {
            return BackgroundWorkThread != null && BackgroundWorkThread.IsAlive == true;
        }

        /// <summary>
        /// Check if background thread is not alive anymore
        /// </summary>
        /// <returns></returns>
        public bool IsBackgroundWorkThreadNotAlive()
        {
            return BackgroundWorkThread != null && BackgroundWorkThread.IsAlive == false;
        }

        #endregion

        #region Selection Methods

        public static void SaveSelectedVoxelMap(bool saveVoxelMaterials)
        {
            var firstVoxel = MyEditorGizmo.SelectedEntities.OfType<MyVoxelMap>().FirstOrDefault();
            if (firstVoxel != null)
            {
                firstVoxel.SaveVoxelContents(Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "voxel_export.vox"), saveVoxelMaterials);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.Ok, MyTextsWrapperEnum.Ok, MyTextsWrapperEnum.Ok, null));
            }
        }

        /// <summary>
        /// IMPORTANT!! Invoke this method to copy objects in editor
        /// </summary>
        /// <param name="waitForAttachment"></param>
        public bool CopySelected(bool waitForAttachment)
        {
            bool valid = true;
            m_bInsertFromGuiFlag = false;

            if (MyFakes.ENABLE_OBJECT_COUNTS_LIMITS)
            {
                //here test if we can even add entity
                if (!IsEditingPrefabContainer() && MyEntities.GetObjectsCount() + MyEditorGizmo.SelectedEntities.Count > MyEditorConstants.MAX_EDITOR_ENTITIES_LIMIT)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    return false;
                }

                //test max count of containers in scene
                if (MyEditorGizmo.SelectedEntities.Find(a => a is MyPrefabContainer) != null &&
                    MyPrefabContainerManager.GetInstance().GetContainerCount() + MyEditorGizmo.SelectedEntities.FindAll(a => a is MyPrefabContainer).Count >= MyEditorConstants.MAX_CONTAINER_NUMBER)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    return false;
                }
            }

            
            //test if we are in ingame and have prebuilded exact prefabs
            if (MyGuiScreenGamePlay.Static.IsIngameEditorActive())
            {
                //List<MyInventoryItem> prefabInventoryItems = MyEditor.Static.FoundationFactory.Inventory.GetInventoryItems(objectToBuild.ObjectBuilder);
                //Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, int> dict = new Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, int>();
                int[][] dict = new int[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];
                Dictionary<int, MyMwcObjectBuilderTypeEnum> dict2 = new Dictionary<int,MyMwcObjectBuilderTypeEnum>();                

                foreach(MyEntity ent in MyEditorGizmo.SelectedEntities)
                {
                    if (!(ent.GetObjectBuilder(false) is MyMwcObjectBuilder_PrefabBase))
                        continue;
                    MyMwcObjectBuilder_PrefabBase s = ent.GetObjectBuilder(false) as MyMwcObjectBuilder_PrefabBase;

                    if (dict[(int)s.GetObjectBuilderType()] == null)
                    {
                        dict[(int)s.GetObjectBuilderType()] = new int[MyMwcObjectBuilder_Base.GetObjectBuilderIDs(s.GetObjectBuilderType()).Max() + 1];
                        dict2.Add(s.GetObjectBuilderId().Value, s.GetObjectBuilderType());
                    }

                    //if (!dict.ContainsKey(s.PrefabType))
                    //{
                    //    dict.Add(s.PrefabType, 0);
                    //    dict2.Add(s.GetObjectBuilderId().Value, s.GetObjectBuilderType());
                    //}
                    //dict[s.PrefabType] += 1;
                    dict[(int) s.GetObjectBuilderType()][s.GetObjectBuilderId().Value] += 1;
                }

                for (int prefabType = 0; prefabType < dict.Length; prefabType++)
                {
                    if (dict[prefabType] != null)
                    {
                        for (int prefabId = 0; prefabId < dict[prefabType].Length; prefabId++)
                        {
                            if (dict[prefabType][prefabId] > 0)
                            {                                
                                int prefabInventoryItemsCount = MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItemsCount((MyMwcObjectBuilderTypeEnum)prefabType, prefabId);
                                if (prefabInventoryItemsCount < dict[prefabType][prefabId])
                                {
                                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CannotCopyBecauseYouDontHaveEnoughObjectsStored, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                                    return false;
                                }
                            }
                        }
                    }
                }

                for (int prefabType = 0; prefabType < dict.Length; prefabType++)
                {
                    if (dict[prefabType] != null)
                    {
                        for (int prefabId = 0; prefabId < dict[prefabType].Length; prefabId++)
                        {
                            if (dict[prefabType][prefabId] > 0)
                            {
                                MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.RemoveInventoryItemAmount((MyMwcObjectBuilderTypeEnum)prefabType, prefabId, dict[prefabType][prefabId]);
                            }
                        }
                    }
                }

                //foreach (MyMwcObjectBuilder_Prefab_TypesEnum e in dict.Keys)
                //    {
                //        List<MyInventoryItem> prefabInventoryItems = MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.GetInventoryItems(dict2[(int)e], (int)e);
                //        if (prefabInventoryItems.Count < dict[e])
                //        {
                //            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyTextsWrapperEnum.CannotCopyBecauseYouDontHaveEnoughObjectsStored, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                //            return false;
                //        }
                //    }
                //foreach (MyMwcObjectBuilder_Prefab_TypesEnum e in dict.Keys)
                //{
                //    MyEditor.Static.FoundationFactory.PrefabContainer.Inventory.RemoveInventoryItemAmount(dict2[(int)e], (int)e, dict[e]);
                //}
            }
            

            //test voxel hand shapes 
            int voxelShapes = 0;
            foreach (MyEntity ent in MyEditorGizmo.SelectedEntities)
            {
                if (ent is MyVoxelMap)
                    voxelShapes += (ent as MyVoxelMap).GetVoxelHandShapes().Count;
            }
            if (voxelShapes + MyVoxelMaps.GetVoxelShapesCount() > MyVoxelConstants.MAX_VOXEL_HAND_SHAPES_COUNT)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CannotCopyBecauseMaxVoxelShapesReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return false;
            }


            // first remove all weapon prefab childs & objects:
            for (int i = 0; i < MyEditorGizmo.SelectedEntities.Count; ++i)
            {
                if (MyEditorGizmo.SelectedEntities[i] is MyLargeShipGunBase)
                {
                    MyEditorGizmo.RemoveEntityFromSelection(MyEditorGizmo.SelectedEntities[i]);
                    //MyEditorGizmo.SelectedEntities.RemoveAt(i);
                    if (i > 0) --i;
                }

                if (MyEditorGizmo.SelectedEntities.Count == 0)
                {
                    return false;
                }

                if (MyEditorGizmo.SelectedEntities[i] == MySession.PlayerShip)
                {
                    if (MyEditorGizmo.SelectedEntities.Count > 1)
                    {
                        //MyEditorGizmo.SelectedEntities.RemoveAt(i);
                        MyEditorGizmo.RemoveEntityFromSelection(MyEditorGizmo.SelectedEntities[i]);
                        if (i > 1) --i;
                        MySession.PlayerShip.ClearHighlightning();
                    }
                    else
                    {
                        valid = false;
                    }
                }

                var dummyPoint = MyEditorGizmo.SelectedEntities[i] as MyDummyPoint;
                if (dummyPoint != null && (dummyPoint.DummyFlags & MyDummyPointFlags.PLAYER_START) > 0)
                {
                    if (MyEditorGizmo.SelectedEntities.Count > 1)
                    {
                        //MyEditorGizmo.SelectedEntities.RemoveAt(i);
                        MyEditorGizmo.RemoveEntityFromSelection(MyEditorGizmo.SelectedEntities[i]);
                        if (i > 1) --i;
                        dummyPoint.ClearHighlightning();
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }

            if (MyEditorGizmo.SelectedEntities.Count == 0)
            {
                return false;
            }

            foreach (MyEntity physObject in MyEditorGizmo.SelectedEntities)
            {
                //if (IsValidForCreation(physObject.GetObjectBuilder(false), physObject.WorldMatrix) == false)
                //{
                //    valid = false;
                //    break;
                //}
                if(!IsValidForCreation(MyEditorGizmo.SelectedEntities))
                {
                    valid = false;
                    break;
                }
            }

            //are we in container? 
            if (MyFakes.ENABLE_OBJECT_COUNTS_LIMITS)
            {
                if (IsEditingPrefabContainer())
                {
                    MyPrefabContainer container = GetEditedPrefabContainer();
                    if (container.Children.Count + MyEditorGizmo.SelectedEntities.Count > MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER)
                    {
                        valid = false;
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                }
            }

            if (valid)
            {
                MyEditorActionEntityCopy copyAction = new MyEditorActionEntityCopy(MyEditorGizmo.SelectedEntities);
                copyAction.RegisterAndDoAction();
                m_waitingForAtachment = waitForAttachment;
            }
            return valid;
        }

        /// <summary>
        /// Open edit entity screen based on the type of entity selected
        /// </summary>
        public void EditSelected()
        {
            if (MyEditorGizmo.IsOnlyOneEntitySelected())
            {
                MyEntity selectedEntity = MyEditorGizmo.GetFirstSelected();
                ////disabled 0001713: BUG B - Editor - buttons OK and CANCEL missing in asteroid 'edit select' dialogbox
                /*
                if (selectedEntity is MyVoxelMap || selectedEntity is MyStaticAsteroid)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorAsteroid(selectedEntity as MyVoxelMap));
                }
                ////disabled by bug 	0001712: BUG A - Editor - crash after 'edit select' of largeship (also smallship, debris)
                
                else if (selectedEntity is MyLargeShip)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorLargeShip(selectedEntity as MyLargeShip));
                }
                else if (selectedEntity is MySmallShip)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSmallShip(selectedEntity as MySmallShip));
                }
                else if (selectedEntity is MySmallDebris)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSmallDebris(selectedEntity as MySmallDebris));
                }
                if (selectedEntity is MyWayPoint)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorWayPoint(selectedEntity as MyWayPoint));
                }
                */
                if (selectedEntity is MySmallShipBot)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSmallShip((selectedEntity as MySmallShipBot)));
                }
                else if (selectedEntity is MySpawnPoint)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSpawnPoint(selectedEntity as MySpawnPoint));
                }
                if (selectedEntity is MyPrefabLight)
                {
                    List<MyPrefabLight> lights = new List<MyPrefabLight>{selectedEntity as MyPrefabLight};
                    MyGuiScreenGamePlay.Static.EditorControls.EditLights(lights);
                }
                else if (selectedEntity is MyPrefabSecurityControlHUB)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorSecurityControlHUB(selectedEntity as MyPrefabSecurityControlHUB));
                }
                else if (selectedEntity is MyPrefabScanner)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorScanner(selectedEntity as MyPrefabScanner));
                }
                else if (selectedEntity is MyPrefabLargeWeapon) 
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorLargeWeapon(new List<MyPrefabLargeWeapon>{selectedEntity as MyPrefabLargeWeapon}));
                }
                else if (selectedEntity is MyPrefabBase)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorPrefab(selectedEntity as MyPrefabBase));
                }
                else if (selectedEntity is MyInfluenceSphere)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorInfluenceSphere(selectedEntity as MyInfluenceSphere));
                }
                else if (selectedEntity is MyDummyPoint)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorDummyPoint(selectedEntity as MyDummyPoint));
                }
                else if (selectedEntity is MyVoxelMap)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorAsteroid(selectedEntity as MyVoxelMap));
                }
                else if (selectedEntity is MyStaticAsteroid)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorAsteroid(selectedEntity as MyStaticAsteroid));
                }
                else if (selectedEntity is MyCargoBox)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorCargoBox(selectedEntity as MyCargoBox));
                }
                else if (selectedEntity is MyPrefabContainer)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorPrefabContainer(selectedEntity as MyPrefabContainer));
                }
                else if (selectedEntity is MyWayPoint)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorWaypoint(selectedEntity as MyWayPoint));
                }
            }
            else if (MyEditorGizmo.IsOnlyOneEntityTypeSelected())
            {
                MyEntity selectedEntity = MyEditorGizmo.GetFirstSelected();
                if (selectedEntity is MyPrefabLight)
                {
                    List<MyPrefabLight> lights = new List<MyPrefabLight>();
                    foreach (MyEntity entity in MyEditorGizmo.SelectedEntities) 
                    {
                        lights.Add((MyPrefabLight)entity);
                    }
                    MyGuiScreenGamePlay.Static.EditorControls.EditLights(lights);
                }
                if (selectedEntity is MyPrefabLargeWeapon) 
                {
                    List<MyPrefabLargeWeapon> largeWeapons = new List<MyPrefabLargeWeapon>();
                    foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
                    {
                        largeWeapons.Add((MyPrefabLargeWeapon)entity);
                    }
                    MyGuiManager.AddScreen(new MyGuiScreenEditorLargeWeapon(largeWeapons));
                }
                if (selectedEntity is MyWayPoint)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenEditorWaypoint(MyEditorGizmo.SelectedEntities.ConvertAll<MyWayPoint>(a => a as MyWayPoint)));
                }
            }
        }

        /// <summary>
        /// True if edit dialog is available
        /// </summary>
        public bool CanEditSelected()
        {
            if (MyEditorGizmo.IsOnlyOneEntitySelected())
            {
                MyEntity selectedEntity = MyEditorGizmo.GetFirstSelected();
                return
                    /*
                    //disabled 0001713: BUG B - Editor - buttons OK and CANCEL missing in asteroid 'edit select' dialogbox
                    selectedEntity is MyStaticAsteroid ||
                    //disabled by bug 	0001712: BUG A - Editor - crash after 'edit select' of largeship (also smallship, debris)
                    selectedEntity is MyLargeShip ||
                    selectedEntity is MySmallDebris ||
                    */
                    selectedEntity is MySmallShipBot ||
                    selectedEntity is MyVoxelMap ||
                    selectedEntity is MyStaticAsteroid ||
                    selectedEntity is MyPrefabLight ||
                    selectedEntity is MySpawnPoint ||
                    selectedEntity is MyWayPoint ||
                    selectedEntity is MyInfluenceSphere ||
                    selectedEntity is MyDummyPoint ||
                    selectedEntity is MyPrefabSecurityControlHUB ||
                    selectedEntity is MyCargoBox ||
                    selectedEntity is MyPrefabScanner ||                    
                    selectedEntity is MyPrefabBase || 
                    selectedEntity is MyPrefabContainer;
            }
            else if(MyEditorGizmo.IsOnlyOneEntityTypeSelected())
            {
                MyEntity selectedEntity = MyEditorGizmo.GetFirstSelected();
                return selectedEntity is MyPrefabLight || 
                       selectedEntity is MyPrefabLargeWeapon;
            }
            return false;
        }

        /// <summary>
        /// True if selected snap point is linked
        /// </summary>
        public bool CanUnlinkSnapPoints(MyPrefabSnapPoint snapPoint)
        {
            return snapPoint != null && LinkedSnapPoints.Exists(a => a.Exists(b => b.SnapPoint == snapPoint));
        }


        /// <summary>
        /// True if selected snap point is linked
        /// </summary>
        public List<MySnapPointLink> GetSnapPointLink(MyPrefabSnapPoint snapPoint)
        {
            return snapPoint != null ? LinkedSnapPoints.Find(a => a.Exists(b => b.SnapPoint == snapPoint)) : null;
        }

        /// <summary>
        /// True if linked entity is linked with some prefab
        /// </summary>
        public bool IsLinked(MyEntity myEntity)
        {
            return LinkedSnapPoints.Exists(a => a.Exists(b => b.SnapPoint.Prefab == myEntity));
        }

        /// <summary>
        /// Try link two or more snap points, works with selected snap point
        /// </summary>
        public void LinkSnapPoints()
        {
            List<MySnapPointLink> snapPointLinks = new List<MySnapPointLink>();
            if (MyEditorGizmo.SelectedSnapPoint != null)
            {
                UnlinkSnapPointsAtPosition();

                var selectedSnapPointsWorldPosition = (MyEditorGizmo.SelectedSnapPoint.Matrix * MyEditorGizmo.SelectedSnapPoint.Prefab.WorldMatrix).Translation;
                Debug.Assert(GetEditedPrefabContainer() != null);
                foreach (var entity in GetEditedPrefabContainer().Children)
                {
                    var prefab = entity as MyPrefabBase;
                    if (prefab != null)
                    {
                        foreach (var snapPoint in prefab.SnapPoints)
                        {
                            var snapPointWorldPosition = (snapPoint.Matrix * snapPoint.Prefab.WorldMatrix).Translation;

                            // Link two snap points if distance between them is less then 1.0m
                            // Selected snap point is added also here to snapPoints list
                            if ((selectedSnapPointsWorldPosition - snapPointWorldPosition).LengthSquared() < 1.0f)
                            {
                                snapPointLinks.Add(new MySnapPointLink(snapPoint));
                            }
                        }
                    }
                }

                if (snapPointLinks.Count >= 2)
                {
                    if (CanLinkSnapPoints(snapPointLinks.ConvertAll(a => a.SnapPoint)))
                    {
                        AddLinkedSnapPoints(snapPointLinks);

                        // Update highlighting
                        MyEditorGizmo.SelectedSnapPoint = MyEditorGizmo.SelectedSnapPoint;
                    }
                    else
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SnapPointLinkError, MyTextsWrapperEnum.LinkSnapPoints, MyTextsWrapperEnum.Ok, null));
                    }
                }
            }
        }

        /// <summary>
        /// Unlink two or more snap points, works with selected snap point
        /// </summary>
        public void UnlinkSnapPoints()
        {
            if (CanUnlinkSnapPoints(MyEditorGizmo.SelectedSnapPoint))
            {
                var snapPoint = MyEditorGizmo.SelectedSnapPoint;
                // Update highlighting
                MyEditorGizmo.SelectedSnapPoint = null;
                RemoveAllLinkedSnapPoints(b => b.SnapPoint == snapPoint);
                MyEditorGizmo.SelectedSnapPoint = snapPoint;
            }
        }

        /// <summary>
        /// Unlink two or more snap points at position specified by selected snap point
        /// </summary>
        public void UnlinkSnapPointsAtPosition()
        {
            if (MyEditorGizmo.SelectedSnapPoint != null)
            {
                var snapPoint = MyEditorGizmo.SelectedSnapPoint;
                var snapPointWorldPosition = (snapPoint.Matrix * snapPoint.Prefab.WorldMatrix).Translation;

                // Update highlighting
                MyEditorGizmo.SelectedSnapPoint = null;
                RemoveAllLinkedSnapPoints(b =>
                {
                    var bWorldPosition = (b.SnapPoint.Matrix * b.SnapPoint.Prefab.WorldMatrix).Translation;
                    return (snapPointWorldPosition - bWorldPosition).LengthSquared() < 1.0f;
                });
                MyEditorGizmo.SelectedSnapPoint = snapPoint;
            }
        }

        /// <summary>
        /// Adds linked prefabs to entity list
        /// </summary>
        public void AddLinkedEntities(List<MyEntity> entities)
        {
            List<MyEntity> addedEntities = new List<MyEntity>();
            foreach (var entity in entities)
            {
                List<MyEntity> linkedEntities = new List<MyEntity>();
                GetLinkedEntities(entity, linkedEntities, null);

                foreach (var linkedEntity in linkedEntities)
                {
                    if (!entities.Contains(linkedEntity) && !addedEntities.Contains(linkedEntity))
                    {
                        addedEntities.Add(linkedEntity);
                    }
                }
            }

            entities.AddRange(addedEntities);
        }

        public void GetLinkedEntities(MyEntity entity, List<MyEntity> result, MyPrefabSnapPoint ignoreSnapPoint)
        {
            var prefab = entity as MyPrefabBase;
            if (prefab != null)
            {
                foreach (var snapPoint in prefab.SnapPoints)
                {
                    if (snapPoint == ignoreSnapPoint)
                    {
                        continue;
                    }

                    // Get groups of linked snap points in which current snap point is presented
                    var linkGroups = LinkedSnapPoints.FindAll(a => a.Exists(b => b.SnapPoint == snapPoint));

                    // Get all linked entities to this snap point
                    foreach (var links in linkGroups)
                    {
                        foreach (var snapPointLink in links)
                        {
                            if (snapPointLink.SnapPoint.Prefab != entity)
                            {
                                Debug.Assert(!result.Contains(snapPointLink.SnapPoint.Prefab));
                                result.Add(snapPointLink.SnapPoint.Prefab);

                                // Recursively continue addition of entities
                                GetLinkedEntities(snapPointLink.SnapPoint.Prefab, result, snapPointLink.SnapPoint);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for transformation of linked entities
        /// recursive stitching solves precision problems
        /// </summary>
        public void FixLinkedEntities(MyEntity entity, List<MyEntity> result, MyPrefabSnapPoint ignoreSnapPoint)
        {
            var prefab = entity as MyPrefabBase;
            if (prefab != null)
            {
                foreach (var snapPoint in prefab.SnapPoints)
                {
                    if (snapPoint == ignoreSnapPoint)
                    {
                        continue;
                    }

                    List<MySnapPointLink> links;
                    if (SnapPointLinks.TryGetValue(snapPoint, out links))
                    {

                        // Get groups of linked snap points in which current snap point is presented
                        //var linkGroups = GetLinks(snapPoint);

                        // Get all linked entities to this snap point
                        //foreach (var links in linkGroups)
                        {
                            var link = links.Find(a => a.SnapPoint.Prefab == entity);

                            foreach (var snapPointLink in links)
                            {
                                if (snapPointLink.SnapPoint != snapPoint)
                                {
                                    snapPointLink.SnapPoint.Prefab.WorldMatrix =
                                        snapPointLink.LinkTransformation * Matrix.Invert(link.LinkTransformation) * entity.WorldMatrix;

                                    //// Stitch snap points
                                    //AttachSnapPoints(snapPoint, snapPointLink.SnapPoint, false);

                                    // Add stitched prefab to affected prefabs list
                                    Debug.Assert(!result.Contains(snapPointLink.SnapPoint.Prefab));
                                    result.Add(snapPointLink.SnapPoint.Prefab);

                                    // Recursively continue stitching
                                    FixLinkedEntities(snapPointLink.SnapPoint.Prefab, result, snapPointLink.SnapPoint);
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<List<MySnapPointLink>> GetLinks(MyPrefabSnapPoint snapPoint)
        {
            return LinkedSnapPoints.FindAll(a => a.Exists(b => b.SnapPoint == snapPoint));
        }

        private bool CanLinkSnapPoints(MyPrefabSnapPoint firstSnapPoint, MyPrefabSnapPoint secondSnapPoint)
        {
            //it's not possible to link a snapPoint to itself
            if (firstSnapPoint == secondSnapPoint) return false;
            
            List<MyEntity> linkedEntities = new List<MyEntity>();
            GetLinkedEntities(firstSnapPoint.Prefab, linkedEntities, null);

            // return false if connection of these two snap point would mean cycle
            return !linkedEntities.Contains(secondSnapPoint.Prefab);
        }

        private bool CanLinkSnapPoints(List<MyPrefabSnapPoint> snapPoints)
        {
            Debug.Assert(snapPoints.Count >= 2);
            List<MyEntity> linkedEntities = new List<MyEntity>();
            for (int i = 0; i < snapPoints.Count - 1; i++)
			{
                GetLinkedEntities(snapPoints[i].Prefab, linkedEntities, null);
                if (linkedEntities.Contains(snapPoints[i+1].Prefab))
                {
                    // return false if connection of this snap point would mean cycle
                    return false;
                }
			}
            return true;
        }

        /// <summary>
        /// Attach two snap points, both must be not null and connectable
        /// </summary>
        public void AttachSnapPoints(MyPrefabSnapPoint firstSnapPoint, MyPrefabSnapPoint secondSnapPoint, bool affectLinkedEntities)
        {
            // Attach prefabs
            Matrix first = firstSnapPoint.Matrix * firstSnapPoint.Prefab.WorldMatrix;
            Matrix secondReversed = Matrix.CreateWorld(secondSnapPoint.Matrix.Translation, -secondSnapPoint.Matrix.Forward, secondSnapPoint.Matrix.Up);
            Matrix second = secondReversed * secondSnapPoint.Prefab.WorldMatrix;

            Vector3 firstWorld = first.Translation;
            Vector3 secondWorld = second.Translation;

            first.Translation = Vector3.Zero;
            second.Translation = Vector3.Zero;

            Matrix deltaRotation = Matrix.Invert(second) * first;

            secondSnapPoint.Prefab.WorldMatrix =
                secondSnapPoint.Prefab.WorldMatrix *
                Matrix.CreateTranslation(-secondWorld) *
                deltaRotation *
                Matrix.CreateTranslation(firstWorld);

            if (affectLinkedEntities)
            {
                List<MyEntity> linkedEntities = new List<MyEntity>();
                FixLinkedEntities(secondSnapPoint.Prefab, linkedEntities, null);
            }
        }

        // Return waypoints corresponding to snap points (by prefab parent and distance).
        private List<MyWayPoint> GetWayPointsAtSnapPointLinks(List<MySnapPointLink> snapPointLinks)
        {
            var wayPoints = new List<MyWayPoint>();

            foreach (var snapPointLink in snapPointLinks)
            {
                MyPrefabSnapPoint snapPoint = snapPointLink.SnapPoint;
                var position = (snapPoint.Matrix * snapPoint.Prefab.WorldMatrix).Translation;

                MyWayPoint closest = snapPoint.Prefab.GetClosestWayPointTo(position);
                if (closest == null)
                {
                    closest = MyWayPointGraph.GetClosestWaypoint(position);  // if there's nothing in the prefab, try the closest waypoint outside
                }
                if (closest != null) wayPoints.Add(closest);

                wayPoints.AddRange(MyWayPointGraph.GetAllWaypointsInSphere(position, 40.0f));
            }

            return wayPoints;
        }

        public void ConnectWayPointsAtSnapPointLinks(List<MySnapPointLink> snapPointLinks)
        {
            var wayPoints = GetWayPointsAtSnapPointLinks(snapPointLinks);

            // connect the waypoints by a ring - no need to add the complete graph
            MyWayPoint prev = wayPoints.Count > 0 ? wayPoints[wayPoints.Count - 1] : null;
            foreach (var current in wayPoints)
            {
                if (prev != null) MyWayPoint.Connect(prev, current);
                prev = current;
            }
        }

        private void DisconnectWayPointsAtSnapPointLinks(List<MySnapPointLink> snapPointLinks)
        {
            var wayPoints = GetWayPointsAtSnapPointLinks(snapPointLinks);
            MyWayPoint.DisconnectAll(wayPoints);
        }

        public void AddLinkedSnapPoints(List<MySnapPointLink> snapPointLinks)
        {
            ConnectWayPointsAtSnapPointLinks(snapPointLinks);
            LinkedSnapPoints.Add(snapPointLinks);

            foreach (var item in snapPointLinks)
	        {
                List<MySnapPointLink> links;
                if (!SnapPointLinks.TryGetValue(item.SnapPoint, out links))
                {
                    links = snapPointLinks;
                    SnapPointLinks.Add(item.SnapPoint, links);
                }
                else
                {
                    links.AddRange(snapPointLinks);
                }
	        }
        }

        Dictionary<MyPrefabSnapPoint, List<MySnapPointLink>> SnapPointLinks = new Dictionary<MyPrefabSnapPoint,List<MySnapPointLink>>();

        public void RemoveLinkedSnapPoints(MyPrefabBase myPrefab)
        {
            // called only from MyPrefabBase.Close(): we don't need to disconnect waypoints (they will be deleted along with the prefab)

            LinkedSnapPoints.ForEach(a => a.RemoveAll(b => b.SnapPoint.Prefab == myPrefab));
            LinkedSnapPoints.RemoveAll(a => a.Count < 2);

            foreach (var snapPoint in myPrefab.SnapPoints)
            {
                SnapPointLinks.Remove(snapPoint);
            }
        }

        public void RemoveAllLinkedSnapPoints(Predicate<MySnapPointLink> match)
        {
            foreach (var snapPointLinks in LinkedSnapPoints)
                if (snapPointLinks.Exists(match))
                    DisconnectWayPointsAtSnapPointLinks(snapPointLinks);

            LinkedSnapPoints.RemoveAll(a => a.Exists(match));

            for (int i = 0; i < SnapPointLinks.Count; i++)
            {
                bool exists = SnapPointLinks.Values.ElementAt(i).Exists(match);
                if (exists)
                {
                    SnapPointLinks.Remove(SnapPointLinks.Keys.ElementAt(i));
                    i--;
                }
            }
        }


        private void ClearLinkedSnapPoints()
        {
            foreach (var snapPointLinks in LinkedSnapPoints)
                DisconnectWayPointsAtSnapPointLinks(snapPointLinks);
            LinkedSnapPoints.Clear();

            SnapPointLinks.Clear();
        }

        public void CopySnapPointLinks(List<MyEntity> sourceEntities, IMyEntityIdRemapContext remapContext)
        {
            foreach (var group in LinkedSnapPoints.ToList())
            {
                // does this link contain any copied prefab?
                foreach (var link in group)
                    foreach (var sourceEntity in sourceEntities)
                        if (link.SnapPoint.Prefab == sourceEntity)
                            goto containsCopied;
                continue;

            containsCopied:

                // convert to new link and connect
                var snapPointLinks = new List<MySnapPointLink>();

                foreach (var link in group)
                {
                    // get the newly-created prefab
                    var linkPrefab = link.SnapPoint.Prefab;
                    if (!linkPrefab.EntityId.HasValue) continue;
                    uint? newId = remapContext.RemapEntityId(linkPrefab.EntityId.Value.NumericValue);
                    if (newId == null) continue;
                    MyEntity newEntity;
                    if (!MyEntities.TryGetEntityById(new MyEntityIdentifier(newId.Value), out newEntity)) continue;
                    var newPrefab = newEntity as MyPrefabBase;

                    // get the index
                    int snapPointIndex;
                    for (snapPointIndex = 0; snapPointIndex < linkPrefab.SnapPoints.Count; snapPointIndex++)
                        if (linkPrefab.SnapPoints[snapPointIndex] == link.SnapPoint)
                            break;

                    if (snapPointIndex < linkPrefab.SnapPoints.Count)
                        snapPointLinks.Add(new MySnapPointLink(newPrefab.SnapPoints[snapPointIndex]));
                }

                if (snapPointLinks.Count >= 2)
                    AddLinkedSnapPoints(snapPointLinks);
            }
        }

        /// <summary>
        /// Works on selected entities in editor, if theres some invisible snap point, Toggle Snap Points will make all snap points visible
        /// </summary>
        private void ToggleSnapPoints()
        {
            // First try find some invisible snap point
            // If theres some invisible snap point, Toggle Snap Points will make all snap points visible
            var someSnapPointInvisible = false;
            foreach (var entity in MyEditorGizmo.SelectedEntities)
	        {
                var prefab = entity as MyPrefabBase;
                if (prefab != null)
                {
                    foreach (var snapPoint in prefab.SnapPoints)
	                {
                        if (!snapPoint.Visible)
                        {
                            someSnapPointInvisible = true;
                            break;
                        }
	                }

                    if (someSnapPointInvisible) break;
                }
	        }

            foreach (var entity in MyEditorGizmo.SelectedEntities)
	        {
                var prefab = entity as MyPrefabBase;
                if (prefab != null)
                {
                    foreach (var snapPoint in prefab.SnapPoints)
	                {
                        // If some invisible make all visible
                        snapPoint.Visible = someSnapPointInvisible;
	                }
                }
	        }
        }

        /// <summary>
        /// This method is used to select all objects, that are in intersection of boundingFrustum created from selection rectangle(dragging primary action control)
        /// </summary>
        /// <param name="input"></param>
        private void SelectAllEntitiesInRectangle(MyGuiInput input)
        {
            if (IsSelectingByRectangle() && !(MyGuiScreenGamePlay.Static.IsIngameEditorActive() && GetEditedPrefabContainer() == null))
            {
                if (input.IsKeyPress(Keys.LeftControl) == false)
                {
                    MyEditorGizmo.ClearSelection();
                }

                // Retrieve bounding frustum from selection rectangle
                BoundingFrustum boundingFrustum = MyUtils.UnprojectRectangle(m_selectionRectangle, MyCamera.Viewport, MyCamera.ViewMatrix, MyCamera.ProjectionMatrix);

                // In case we perform selection inside container, disallow to select objects outside of container
                List<MyEntity> intersectionObjects = null;
                if (m_activePrefabContainer == null)
                {
                    // first we must if there is no prefab in selected entities
                    if (MyEditorGizmo.SelectedEntities.Count > 0)
                    {
                        foreach (MyEntity entity in MyEditorGizmo.SelectedEntities) 
                        {
                            Debug.Assert(!(entity is MyPrefabBase), "You can't have selected prefab where prefab container is not active");
                        }
                    }

                    intersectionObjects = new List<MyEntity>();
                    MyEntities.GetAllIntersectionWithBoundingFrustum_UNOPTIMIZED(ref boundingFrustum, intersectionObjects, true, false, true);
                }
                else
                {
                    intersectionObjects = m_activePrefabContainer.GetPrefabsInFrustum(ref boundingFrustum);
                }

                // Now that we have all objects in bounding frustum, add or remove from selection, based on if already selected or not
                if (intersectionObjects != null)
                {
                    MyEditorGizmo.AddEntitiesToSelection(intersectionObjects);

                    // add waypoints from selected waypoint path
                    MyWayPointGraph.UpdateSelectedPath();
                }

            }
        }

        #endregion

        #region Create And Entity Loading Methods

        /// <summary>
        /// IMPORTANT!! Invoke this method to add object in editor - newly added object is immediately attached to camera
        /// </summary>
        /// <param name="objectBuilder"></param>
        public void CreateFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder, Matrix matrix, Vector2? screenPosition = null)
        {

            //here test if we can even add entity
            if (MyFakes.ENABLE_OBJECT_COUNTS_LIMITS)
            {
                if (!(objectBuilder is MyMwcObjectBuilder_PrefabBase) && MyEntities.GetObjectsCount() + 1 > MyEditorConstants.MAX_EDITOR_ENTITIES_LIMIT)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    return;
                }


                //here test for container limit
                if (objectBuilder is MyMwcObjectBuilder_PrefabContainer && MyPrefabContainerManager.GetInstance().GetContainerCount() >= MyEditorConstants.MAX_CONTAINER_NUMBER)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    return;
                }

                if (IsEditingPrefabContainer() && objectBuilder is MyMwcObjectBuilder_PrefabBase)
                {
                    MyPrefabContainer container = GetEditedPrefabContainer();
                    if (container.Children.Count + 1 > MyPrefabContainerConstants.MAX_PREFABS_IN_CONTAINER)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxObjectsReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                        return;
                    }
                }
            }

            if (IsValidForCreation(objectBuilder, matrix))
            {
                objectBuilder.IsDestructible = MyGameplayConstants.GetGameplayProperties(objectBuilder, MyMwcObjectBuilder_FactionEnum.Euroamerican).IsDestructible;
                MyEditorActionEntityAdd action = new MyEditorActionEntityAdd(objectBuilder, matrix, screenPosition);
                action.RegisterAndDoAction();
                //m_waitingForAtachment = true;
            }
        }

        /// <summary>
        /// TODO temporary here - will have to remove elsewhere
        /// </summary>
        /// <param name="position"></param>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <param name="shipType"></param>
        /// <param name="botBehaviour"></param>
        /// <param name="shipFaction"></param>
        /// <returns></returns>
        public static MyMwcObjectBuilder_SmallShip_Bot CreateDefaultBotObjectBuilder(Vector3 position, Vector3 forward, Vector3 up, MyMwcObjectBuilder_SmallShip_TypesEnum shipType, MyMwcObjectBuilder_FactionEnum shipFaction)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));

            
            List<MyMwcObjectBuilder_SmallShip_Ammo> ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));

            MyMwcObjectBuilder_Inventory inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000);            
            foreach (MyMwcObjectBuilder_SmallShip_Ammo ammoItem in ammo)
            {
                inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(ammoItem, 1000));
            }
             
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmo = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            assignmentOfAmmo.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            assignmentOfAmmo.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));

            //return new MyMwcObjectBuilder_SmallShip_Bot(shipType,
            //    weapons,
            //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
            //    ammo,
            //    assignmentOfAmmo,
            //    null,
            //    shipFaction);            
            return new MyMwcObjectBuilder_SmallShip_Bot(shipType,
                inventory,
                weapons,
                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
                assignmentOfAmmo,
                null,
                null,
                null,
                MyGameplayConstants.HEALTH_RATIO_MAX,
                100f,
                float.MaxValue,
                float.MaxValue,
                true,
                false,
                shipFaction,
                MyAITemplateEnum.DEFAULT,
                0,
                1000,
                1000,
                MyPatrolMode.CYCLE,
                null,
                BotBehaviorType.IDLE,
                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE,
                0, false, true);
        }

        /// <summary>
        /// TODO load sector command
        /// </summary>
        public void LoadSector()
        {
            MyGuiScreenGamePlay.Static.Restart();
        }

        #endregion

        #region Clear And Removed Methods

        /// <summary>
        /// Removes all objects from sector
        /// </summary>
        public void ClearWholeSector()
        {
            MyEditorActionClearSector action = new MyEditorActionClearSector();
            action.RegisterAndDoAction();
        }

        /// <summary>
        /// IMPORTANT!! Invoke this method to remove objects in editor
        /// </summary>
        /// <param name="physObjects"></param>
        public void RemoveEntities(List<MyEntity> physObjects)
        {
            //dont allow to remove player ship!
            foreach (MyEntity entity in physObjects)
            {
                if (entity == MySession.PlayerShip)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                    return;
                }
            }

            //ask in message box now
            if (MyGuiScreenGamePlay.Static.IsIngameEditorActive())
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.DeleteObjects, MyTextsWrapperEnum.DeleteSelected, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No,
                    mbReturn =>
                    {
                        if (mbReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
                        {
                            MyEditorActionEntityDelete deleteAction = new MyEditorActionEntityDelete(physObjects);
                            deleteAction.RegisterAndDoAction();
                        }
                    }));
            }
            else
            {
                MyEditorActionEntityDelete deleteAction = new MyEditorActionEntityDelete(physObjects);
                deleteAction.RegisterAndDoAction();
            }
        }

        public void RemoveFromObjectGroups(MyEntity entity)
        {
            foreach (var objectGroup in ObjectGroups)
            {
                objectGroup.RemoveObject(entity);
            }
        }

        /// <summary>
        /// Clears attached entities from camera
        /// </summary>
        public void ClearAttachedEntities()
        {
            m_attachedEntities.Clear();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// IMPORTANT!! Validation method, that must be invoked prior to creation of any object
        /// </summary>
        /// <param name="objectBuilder"></param>
        /// <returns></returns>
        bool IsValidForCreation(MyMwcObjectBuilder_Base objectBuilder, Matrix matrix)
        {
            bool valid = true;
            if (objectBuilder is MyMwcObjectBuilder_VoxelMap)
            {
                MyMwcObjectBuilder_VoxelMap voxelMapObjectBuilder = (MyMwcObjectBuilder_VoxelMap)objectBuilder;
                Vector3 voxelPosition = voxelMapObjectBuilder.PositionAndOrientation.Position;
                if (IsObjectOutOfSectorBounds(voxelPosition) || (IsNewAsteroidAllowed(voxelMapObjectBuilder.VoxelFile) == false))
                {
                    valid = false;
                }
            }
            else if (objectBuilder is MyMwcObjectBuilder_Object3dBase)
            {
                MyMwcObjectBuilder_Object3dBase object3dBuilder = (MyMwcObjectBuilder_Object3dBase)objectBuilder;
                if (IsObjectOutOfSectorBounds(matrix.Translation) ||
                    (object3dBuilder is MyMwcObjectBuilder_LargeShip && !IsNewLargeShipAllowed()))
                {
                    valid = false;
                }
            }
            else if (objectBuilder is MyMwcObjectBuilder_PrefabBase)
            {
                MyMwcObjectBuilder_PrefabBase prefabBuilder = (MyMwcObjectBuilder_PrefabBase)objectBuilder;
                Vector3 relativePosition = MyPrefabContainer.GetRelativePositionInAbsoluteCoords(prefabBuilder.PositionInContainer);
                if (MyPrefabContainer.IsPrefabOutOfContainerBounds(relativePosition) == true)
                {
                    valid = false;
                }
            }
            return valid;
        }

        bool IsValidForCreation(List<MyEntity> entities)
        {
            int totalVoxelCells = MyVoxelMaps.GetTotalDataCellsCount();
            int largeShipsCount = 0;
            foreach (MyEntity entity in entities)
            {
                MyMwcObjectBuilder_Base objectBuilder = entity.GetObjectBuilder(false);
                Matrix matrix = entity.WorldMatrix;
                if (objectBuilder is MyMwcObjectBuilder_VoxelMap)
                {
                    MyMwcObjectBuilder_VoxelMap voxelMapObjectBuilder = (MyMwcObjectBuilder_VoxelMap)objectBuilder;
                    Vector3 voxelPosition = voxelMapObjectBuilder.PositionAndOrientation.Position;
                    if (IsObjectOutOfSectorBounds(voxelPosition))
                    {
                        return false;
                    }
                    else if (!MyVoxelMap.IsNewAllowed(voxelMapObjectBuilder.VoxelFile, ref totalVoxelCells))
                    {
                        MyEditor.Static.m_newAsteroidAllowed = false;
                        return false;
                    }                    
                }
                else if (objectBuilder is MyMwcObjectBuilder_Object3dBase)
                {
                    MyMwcObjectBuilder_Object3dBase object3dBuilder = (MyMwcObjectBuilder_Object3dBase)objectBuilder;
                    if (IsObjectOutOfSectorBounds(matrix.Translation))
                    {
                        return false;
                    }
                    else if(object3dBuilder is MyMwcObjectBuilder_LargeShip)
                    {
                        largeShipsCount++;                        
                    }
                }
                else if (objectBuilder is MyMwcObjectBuilder_PrefabBase)
                {
                    MyMwcObjectBuilder_PrefabBase prefabBuilder = (MyMwcObjectBuilder_PrefabBase)objectBuilder;
                    Vector3 relativePosition = MyPrefabContainer.GetRelativePositionInAbsoluteCoords(prefabBuilder.PositionInContainer);
                    if (MyPrefabContainer.IsPrefabOutOfContainerBounds(relativePosition) == true)
                    {
                        return false;
                    }
                }                
            }

            // validate large ships limit            
            if(!IsNewLargeShipsAllowed(largeShipsCount))
            {
                return false;
            }

            MyEditor.Static.m_newAsteroidAllowed = true;
            return true;
        }

        /// <summary>
        /// There is limited number of large ships allowed
        /// </summary>
        /// <returns></returns>
        public static bool IsNewLargeShipAllowed()
        {
            return IsNewLargeShipsAllowed(1);
        }

        public static bool IsNewLargeShipsAllowed(int count)
        {
            if (MyEntities.GetLargeShipsCount() + count > MyLargeShipConstants.MAX_LARGE_SHIPS_COUNT_IN_SECTOR)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MaxLargeShipsCountReached, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Whole sector has limited size, and phys objects can be placed only inside at such positions, that they do not
        /// reach over the sector border
        /// </summary>
        /// <param name="objectPosition"></param>
        /// <returns></returns>
        public static bool IsObjectOutOfSectorBounds(Vector3 objectPosition)
        {
            if (MyMwcSectorConstants.SECTOR_SIZE_FOR_PHYS_OBJECTS_BOUNDING_BOX.Contains(objectPosition) != ContainmentType.Contains)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ObjectOutOfBounds, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return true;
            }
            return false;
        }

        /// <summary>
        /// There is limited number of voxels allowed in sector, validate it in this method
        /// </summary>
        /// <param name="voxelFile"></param>
        /// <returns></returns>
        static bool IsNewAsteroidAllowed(MyMwcVoxelFilesEnum voxelFile)
        {
            if (MyVoxelMap.IsNewAllowed(voxelFile))
            {
                MyEditor.Static.m_newAsteroidAllowed = true;
            }
            else
            {
                MyEditor.Static.m_newAsteroidAllowed = false;
            }
            return MyEditor.Static.m_newAsteroidAllowed;
        }        

        /// <summary>
        /// Check if any entity is attached to camera
        /// </summary>
        /// <returns></returns>
        public bool IsAnyEntityAttachedToCamera()
        {
            return m_attachedEntities.Count != 0;
        }

        /// <summary>
        /// Check if selection by rectangle is happening
        /// </summary>
        /// <returns></returns>
        public bool IsSelectingByRectangle()
        {
            return m_drawingSelectionRectangle && m_selectionRectangle.Width != 0 && m_selectionRectangle.Height != 0;
        }

        /// <summary>
        /// Check if editing prefab container now
        /// </summary>
        /// <returns></returns>
        public bool IsEditingPrefabContainer()
        {
            return m_activePrefabContainer != null;
        }

        /// <summary>
        /// Resets currently active prefab container
        /// </summary>
        public void ResetActivePrefabContainer()
        {
            m_activePrefabContainer = null;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// This method retrieves object builders of all objects in sector and sends them to server
        /// </summary>
        public MyGuiScreenEditorSaveProgress SaveSector()
        {
            if (m_collidingElements.Count == 0)
            {
                Debug.Assert(MySession.Static != null, "Session cannot be null");
                return MySession.Static.Save(true, true, false);
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CantSaveSectorContainerCollides, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                return null;
            }
        }

        /// <summary>
        /// This method returns enum value, that can be used to indicate, if we have something selected in editor
        /// and if so, what it is, or if something is attached to spectator.
        /// </summary>
        /// <returns></returns>
        public static MyEditorStateEnum GetCurrentState()
        {
            MyEditorStateEnum currentState = MyEditorStateEnum.NOTHING_SELECTED;
            if (MyEditor.Static.IsAnyEntityAttachedToCamera())
            {
                currentState = MyEditorStateEnum.ATTACHED;
            }
            else if (MyEditorVoxelHand.IsEnabled())
            {
                currentState = MyEditorStateEnum.VOXEL_HAND_ENABLED;
            }
            else if (MyEditorGizmo.IsOnlyOneEntitySelected())
            {
                MyEntity physObject = MyEditorGizmo.GetFirstSelected();
                if (physObject is MyVoxelMap)
                {
                    currentState = MyEditorStateEnum.SELECTED_ASTEROID;
                }
                else if (physObject is MyWayPoint)
                {
                    currentState = MyEditorStateEnum.SELECTED_WAYPOINT;
                }
                else if (physObject is MySpawnPoint)
                {
                    currentState = MyEditorStateEnum.SELECTED_SPAWNPOINT;
                }
                else if (physObject is MyPrefabLight)
                {
                    currentState = MyEditorStateEnum.SELECTED_LIGHT;
                }
                else if (physObject is MyPrefabSecurityControlHUB)
                {
                    currentState = MyEditorStateEnum.SELECTED_SECURITY_CONTROL_HUB;
                }
                else if (physObject is MyPrefabScanner)
                {
                    currentState = MyEditorStateEnum.SELECTED_SCANNER;
                }
                else if (physObject is MyPrefabContainer)
                {
                    currentState = MyEditorStateEnum.SELECTED_PREFAB_CONTAINER;
                }
                else if (physObject is MyPrefabBase)
                {
                    currentState = MyEditorStateEnum.SELECTED_PREFAB;
                }
/*                else if (physObject is MyLargeShip)
                {
                    currentState = MyEditorStateEnum.SELECTED_LARGE_SHIP;
                }
  */              else if (physObject is MySmallShip)
                {
                    currentState = MyEditorStateEnum.SELECTED_SMALL_SHIP;
                }
                else if (physObject is MySmallDebris)
                {
                    currentState = MyEditorStateEnum.SELECTED_DEBRIS;
                }
                else if (physObject is MyLargeDebrisField)
                {
                    currentState = MyEditorStateEnum.SELECTED_DEBRIS;
                }
                else if (physObject is MyInfluenceSphere)
                {
                    currentState = MyEditorStateEnum.SELECTED_INFLUENCE_SPHERE;
                }
                else if (physObject is MyStaticAsteroid)
                {
                    currentState = MyEditorStateEnum.SELECTED_STATIC_ASTEROID;
                }
                else if (physObject is MyDummyPoint)
                {
                    currentState = MyEditorStateEnum.SELECTED_DUMMYPOINT;
                }
                else if (physObject is MyCargoBox) 
                {
                    currentState = MyEditorStateEnum.SELECTED_CARGO_BOX;
                }
            }
            else if (MyEditor.Static.IsEditingPrefabContainer())
            {
                if (MyEditorGizmo.SelectedSnapPoint != null)
                {
                    currentState = MyEditorStateEnum.SELECTED_SNAPPOINT;
                }
                else if (MyEditorGizmo.IsMoreThanOneEntitySelected())
                {
                    currentState = MyEditorStateEnum.SELECTED_PREFABS;
                }
                else
                {
                    currentState = MyEditorStateEnum.EDITING_PREFAB_CONTAINER;
                }
            }
            else if (MyEditorGizmo.IsMoreThanOneEntitySelected())
            {
                currentState = MyEditorStateEnum.SELECTED_MIXED;
            }

            return currentState;
        }

        //  Lazy-loading and then returning reference to model
        //  Doesn't load vertex/index shader and doesn't touch GPU. Use it when you need model data - vertex, triangles, octre...
        public static List<MyPrefabSnapPoint> GetSnapPoints(MyModelsEnum modelEnum)
        {
            //MyMwcLog.WriteLine("MyModel.GetSnapPoints -> START");
            //MyMwcLog.IncreaseIndent();

            // Parse snap points from Dummies
            List<MyPrefabSnapPoint> snapPoints = new List<MyPrefabSnapPoint>();

            MyModel model = MyModels.GetModelOnlyDummies(modelEnum);

            foreach (var dummy in model.Dummies)
            {
                if (dummy.Key.StartsWith("SNAPPOINT", StringComparison.InvariantCultureIgnoreCase))
                {
                    var customData = dummy.Value.CustomData;
                    var snapPoint = new MyPrefabSnapPoint(null);

                    // Get rid of scale in rotation part
                    snapPoint.Matrix = Matrix.CreateWorld(dummy.Value.Matrix.Translation, dummy.Value.Matrix.Forward, dummy.Value.Matrix.Up);
                    snapPoint.SnapType = new MyPrefabSnapPoint.MyPrefabSnapPointType("OBJECT_", "", dummy.Value.CustomData);
                    snapPoint.Name = dummy.Key;
                    snapPoints.Add(snapPoint);

                    string targetPostfix = "TARGET_BUILD_TYPE";
                    foreach (var target in customData)
                    {
                        if (target.Key.StartsWith(targetPostfix))
                        {
                            string postfix = target.Key.Substring(targetPostfix.Length);
                            snapPoint.SnapTargets.Add(new MyPrefabSnapPoint.MyPrefabSnapPointType(
                                "TARGET_",
                                postfix,
                                dummy.Value.CustomData));
                        }
                    }
                }
            }

            //MyMwcLog.DecreaseIndent();
            //MyMwcLog.WriteLine("MyModel.GetSnapPoints -> END");

            return snapPoints;
        }


        /// <summary>
        /// Get all snap points for all prefab types
        /// </summary>
        public static void GetPrefabSnapPoints()
        {
            if (m_prefabSnapPoints != null)
            {
                return;
            }

            SysUtils.Utils.MyMwcLog.WriteLine("MyEditor.GetPrefabSnapPoints -> START");
            SysUtils.Utils.MyMwcLog.IncreaseIndent();

            //Stopwatch sw = Stopwatch.StartNew();
            //PrefabSnapPoints = new Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, List<MyPrefabSnapPoint>>();
            m_prefabSnapPoints = new List<MyPrefabSnapPoint>[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];
               /*
            foreach (MyMwcObjectBuilderTypeEnum prefabType in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabType))
                {
                    MyPrefabConfiguration prefabConfig = MyPrefabConstants.GetPrefabConfiguration(prefabType, prefabId);

                    //var model = MyModels.GetModelOnlyData(prefabConfig.ModelLod0Enum);
                    //var snapPoints = MyPrefab.GetSnapPoints(model, null); // ~ 7 seconds, loads all models: 164MB

                    // was 1.3 sec, garbage 70MB, 41s without fetch
                    //var snapPoints = GetSnapPoints(prefabConfig.ModelLod0Enum);    // ~ 0.16 seconds, garbage 3MB, 18s without fetch

                    //PrefabSnapPoints.Add(prefabType, snapPoints);   
                  //  AddPrefabSnapPoints(prefabType, prefabId, snapPoints);
                }
            }    */
            //var elapsed = sw.Elapsed;

            SysUtils.Utils.MyMwcLog.DecreaseIndent();
            SysUtils.Utils.MyMwcLog.WriteLine("MyEditor.GetPrefabSnapPoints -> END");

        }

        public static List<MyPrefabSnapPoint> GetPrefabSnapPoints(MyMwcObjectBuilderTypeEnum prefabType, int prefabId)
        {
            if (m_prefabSnapPoints == null)
                MyEditor.GetPrefabSnapPoints();
            List<MyPrefabSnapPoint> prefabSnapPoints = null;
            try
            {
                //lazy load to spare loading time
                if (m_prefabSnapPoints[(int)prefabType] == null || m_prefabSnapPoints[(int)prefabType][(int)prefabId] == null)
                {
                    MyPrefabConfiguration prefabConfig = MyPrefabConstants.GetPrefabConfiguration(prefabType, prefabId);

                    var snapPoints = GetSnapPoints(prefabConfig.ModelLod0Enum);    
                    AddPrefabSnapPoints(prefabType, prefabId, snapPoints);
                }

                prefabSnapPoints = m_prefabSnapPoints[(int)prefabType][prefabId];
            }
            catch
            {
            }
            return prefabSnapPoints;
        }

        public static void AddPrefabSnapPoints(MyMwcObjectBuilderTypeEnum prefabType, int prefabId, List<MyPrefabSnapPoint> snapPoints)
        {
            //SysUtils.Utils.MyMwcLog.WriteLine("MyEditor.AddPrefabSnapPoints -> START");
            //SysUtils.Utils.MyMwcLog.IncreaseIndent();

            if (m_prefabSnapPoints[(int)prefabType] == null)
            {
                m_prefabSnapPoints[(int)prefabType] = new List<MyPrefabSnapPoint>[MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabType).Max() + 1];
            }
            m_prefabSnapPoints[(int)prefabType][prefabId] = snapPoints;

            //SysUtils.Utils.MyMwcLog.DecreaseIndent();
            //SysUtils.Utils.MyMwcLog.WriteLine("MyEditor.AddPrefabSnapPoints -> END");
        }

        /// <summary>
        /// Init values from check point
        /// </summary>
        public void Init(MyMwcObjectBuilder_Checkpoint m_checkpoint)
        {
            Debug.Assert(m_checkpoint != null);

            ObjectGroups.Clear();
            ClearLinkedSnapPoints();

            //Debug.Assert(m_checkpoint.SectorObjectBuilder != null);
            if (m_checkpoint.SectorObjectBuilder == null || 
                m_checkpoint.SectorObjectBuilder.ObjectGroups == null || 
                m_checkpoint.SectorObjectBuilder.SnapPointLinks == null) 
                    return;

            // Object Groups
            foreach (var item in m_checkpoint.SectorObjectBuilder.ObjectGroups)
            {
                MyObjectGroup objectGroup = new MyObjectGroup(item);
                ObjectGroups.Add(objectGroup);
            }

            // Snap Points
            LinkedSnapPointsBuilders = m_checkpoint.SectorObjectBuilder.SnapPointLinks;

            MySession.Static.LinkEntities += OnLinkEntities;
        }

        /// <summary>
        /// Link entities
        /// </summary>
        private void OnLinkEntities()
        {
            foreach (var objectGroup in ObjectGroups)
            {
                objectGroup.LinkEntities();
            }

            // Load snap point links (LinkedSnapPoints cleared and LinkedSnapPointsBuilders set in Init from checkpoint)
            foreach (var item in LinkedSnapPointsBuilders)
            {
                List<MySnapPointLink> snapPointGroup = new List<MySnapPointLink>();
                foreach (var link in item.Links)
                {
                    MyEntity entity = null;
                    bool success = MyEntities.TryGetEntityById(new MyEntityIdentifier(link.EntityId), out entity);
                    //Debug.Assert(success, "Snap points: unknown link target entityID");
                    MyPrefabBase prefab = entity as MyPrefabBase;
                    //Debug.Assert(prefab != null, "Snap points: link target is not a prefab");

                    if (string.IsNullOrEmpty(link.SnapPointName))
                    {
                        if (prefab != null &&
                            link.Index < prefab.SnapPoints.Count)   // in case of removed snap point
                        {
                            var snapPoint = prefab.SnapPoints[link.Index];
                            snapPointGroup.Add(new MySnapPointLink(snapPoint));
                        }
                    }
                    else if (prefab != null)
                    {
                        foreach (var snapPoint in prefab.SnapPoints)
                        {
                            if (snapPoint.Name == link.SnapPointName)
                            {
                                snapPointGroup.Add(new MySnapPointLink(snapPoint));
                            }
                        }
                    }
                }

                // Only 2 or more is valid link
                if (snapPointGroup.Count > 1)
                {
                    // Accept link only if all snap points are in valid distance (1m snap)
                    bool samePosition = true;
                    Vector3? prevPosition = null;
                    for (int i = 0; i < snapPointGroup.Count; i++)
                    {
                        var link = snapPointGroup[i];
                        var position = (link.SnapPoint.Matrix * link.SnapPoint.Prefab.WorldMatrix).Translation;
                        if (prevPosition.HasValue && Vector3.DistanceSquared(position, prevPosition.Value) > 1.0f)
                        {
                            samePosition = false;
                            Debug.Assert(samePosition, string.Format("Linked snap points {3}:{5} {0} and {4}:{6} {1} are too far ({2})",
                                position, prevPosition.Value, Vector3.Distance(position, prevPosition.Value),
                                (link.SnapPoint.Prefab as MyPrefab) == null ? link.SnapPoint.Prefab.PrefabId.ToString() : (link.SnapPoint.Prefab as MyPrefab).PrefabType.ToString(),
                                (snapPointGroup[i - 1].SnapPoint.Prefab as MyPrefab) == null ? snapPointGroup[i - 1].SnapPoint.Prefab.PrefabId.ToString() : (snapPointGroup[i - 1].SnapPoint.Prefab as MyPrefab).PrefabType.ToString(),
                                link.SnapPoint.Name,
                                snapPointGroup[i - 1].SnapPoint.Name
                            ));
                            break;
                        }
                        prevPosition = position;
                    }

                    if (samePosition)
    	            {
                        AddLinkedSnapPoints(snapPointGroup);
	                }
                }
            }
            LinkedSnapPointsBuilders = null;

            MySession.Static.LinkEntities -= OnLinkEntities;
        }
        
        public List<MyMwcObjectBuilder_SnapPointLink> GetSnapPointLinkBuilders()
        {
            List<MyMwcObjectBuilder_SnapPointLink> linkBuilders = new List<MyMwcObjectBuilder_SnapPointLink>();
            foreach (var snapPointGroup in LinkedSnapPoints)
            {
                MyMwcObjectBuilder_SnapPointLink builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SnapPointLink, null) as MyMwcObjectBuilder_SnapPointLink;
                builder.Links = new List<MyMwcObjectBuilder_SnapPointLink.LinkElement>();

                foreach (var snapPoint in snapPointGroup)
                {
                    uint entityId = snapPoint.SnapPoint.Prefab.EntityId.Value.NumericValue;
                    short index = (short)snapPoint.SnapPoint.Prefab.SnapPoints.IndexOf(snapPoint.SnapPoint);

                    builder.Links.Add(new MyMwcObjectBuilder_SnapPointLink.LinkElement(entityId, index, snapPoint.SnapPoint.Name));
                }

                linkBuilders.Add(builder);
            }

            return linkBuilders;
        }
        
        /// <summary>
        /// When objects are added, collision check fails, this is quick workaround
        /// </summary>
        public void IssueCheckAllCollidingObjects()
        {
            m_issueCheckAllCollidingObjectsCounter = 2;           
        }

        private MyMwcObjectBuilder_PrefabContainer m_extractPrefabsBuilder = null;
        public void ExtractPrefabsToNewContainer()
        {
            foreach (var entity in MyEditorGizmo.SelectedEntities)
            {
                if (entity is MyPrefabBase == false)
                {
                    return;
                }
            }

            List<MyMwcObjectBuilder_PrefabBase> prefabBuilders = new List<MyMwcObjectBuilder_PrefabBase>();
            foreach (var entity in MyEditorGizmo.SelectedEntities)
            {
                prefabBuilders.Add(entity.GetObjectBuilder(true) as MyMwcObjectBuilder_PrefabBase);
                entity.MarkForClose();
            }

            m_extractPrefabsBuilder = new MyMwcObjectBuilder_PrefabContainer(
                null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabBuilders, MyClientServer.LoggedPlayer.GetUserId(),
                MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000));
        }
        #endregion

        /// <summary>
        /// Sets other selection settings based on currently selected things
        /// </summary>
        internal void RefreshSelectionSettings()
        {
            // if waypoints became unselectable, also deselect waypoint paths
            if (!MyEntities.IsTypeSelectable(typeof(MyWayPoint)))
                MyWayPointGraph.SelectedPath = null;

            // if prefabs became unselectable, exit prefab container
            if (!MyEntities.IsTypeSelectable(typeof(MyPrefabBase)))
                MyEditor.Static.ExitActivePrefabContainer();

            // if voxels became hidden, exit voxel hand
            if (MyEntities.IsTypeHidden(typeof(MyVoxelMap)))
                MyEditorVoxelHand.SetEnabled(false);
        }

    }
}
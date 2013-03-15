//#define HOMEKEY_RESET_ENVMAP

#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using KeenSoftwareHouse.Library.Extensions;

using MinerWars.AppCode.App;
using MinerWars.AppCode.ExternalEditor;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Debugging;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo.UniversalLauncher;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Radar;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Render.EnvironmentMap;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Trailer;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

using ParallelTasks;
using SysUtils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.CargoBox;

using MinerWarsMath;
//using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;


using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


#endregion

namespace MinerWars.AppCode.Game.GUI
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MinerWars.CommonLIB.AppCode.Import;

    enum MyCameraAttachedToEnum : byte
    {
        PlayerMinerShip,
        BotMinerShip,
        PlayerMinerShip_ThirdPersonStatic,
        PlayerMinerShip_ThirdPersonFollowing,
        Spectator,
        PlayerMinerShip_ThirdPersonDynamic,
        Drone,
        Camera,
        LargeWeapon,
    }

    public enum MyGuiScreenGamePlayType : int
    {
        MAIN_MENU = 0,                  //  Main menu with fly through in the background - started at application start or if user "exits back to main menu"
        GAME_STORY,                     //  Playing story
        GAME_MMO,                       //  Playing MMO
        PURE_FLY_THROUGH,               //  Only fly through
        CREDITS,                        //  Fly through with credits on top
        EDITOR_STORY,                   //  Editor was opened from main menu
        EDITOR_MMO,                     //  Editor was opened from main menu
        EDITOR_SANDBOX,                 //  Editor was opened from main menu
        INGAME_EDITOR,                  //  Editor, that becomes active during game(switching from cockpit to builder mode and back)
        GAME_SANDBOX,                   //  Playing sandbox

        // number of all sreens - keep it as the last one in this enum:
        ALL_SCREEN_COUNT
    }

    delegate void CameraAttachedChangedHandler(MyCameraAttachedToEnum previousAttachedType, MyCameraAttachedToEnum newAttachedType);
    delegate void ControlledEntityHandler(MyEntity e);

    public struct MyChatMessage
    {
        public int SenderId;
        public StringBuilder SenderName;
        public MyFactionRelationEnum SenderRelation;
        public StringBuilder Message;
        public float TimeStamp;
    }


    partial class MyGuiScreenGamePlay : MyGuiScreenBase
    {
        private const int TIME_UNTIL_GPS_REMINDER = 30000; // in ms
        const int DRONE_CONTROL_INTERVAL = 250; // ms
        public static int FrameCounter = 0;

        public static MyGuiScreenGamePlay Static;
        readonly int FIRST_FADE_IN_DELAY = 10;
        readonly int FIRST_GAME_SAFELY_LOADED_DELAY = 13;
        readonly int FIRST_TOTAL_DELAY = 50;
        readonly int FIRST_DELAY_SAFE_IN_GAME = -1000;

        //events for external editors/scripts
        internal static event EventHandler OnGameLoaded;
        internal event ScreenHandler OnGameReady;
        internal event ScreenHandler OnGameSafelyLoaded;

        public event ControlledEntityHandler ReleasedControlOfEntity;
        public event Action RollLeftPressed;
        public event Action RollRightPressed;

        private bool m_detachingForbidden;

        private float m_fadeAlpha;
        private float m_fadeSpeed;
        public bool m_fadingIn;
        public bool m_fadingOut;

        public event Action FadedOut;
        public event Action FadedIn;

        public bool DetachingForbidden
        {
            get { return m_detachingForbidden; }
            set
            {
                m_detachingForbidden = value;
                if (value)
                {
                    if (m_detachNotification != null) m_detachNotification.Disappear();
                }
                else
                {
                    if (m_detachNotification != null)
                    {
                        object[] args = { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE), "" };

                        m_detachNotification = new MyHudNotification.MyNotification(
                              MyTextsWrapperEnum.NotificationExitControlled,
                              MyHudNotification.GetCurrentScreen(),
                              1f,
                              MyHudConstants.NEUTRAL_FONT,
                              MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                              MyHudNotification.DONT_DISAPEAR,
                              null,
                              false,
                              args);

                        if (!DetachingForbidden)
                        {
                            MyHudNotification.AddNotification(m_detachNotification);
                        }
                    }
                }
            }
        }

        public MySmallShip ShipForSimpleTesting;       //  Reference to ship that is controlled by a player too, for some simple testing

        private MyCameraAttachedToEnum m_cameraAttachedTo;

        private MySoundCue? m_idleVehicleCue = null;

        public MyCameraAttachedToEnum CameraAttachedTo
        {
            get
            {
                return m_cameraAttachedTo;
            }
            set
            {
                var old = m_cameraAttachedTo;

                if (old == MyCameraAttachedToEnum.PlayerMinerShip || value == MyCameraAttachedToEnum.PlayerMinerShip) // if player miner ship lost or gained control
                {
                    if (MySession.PlayerShip != null)
                        MySession.PlayerShip.StopSounds(SharpDX.XACT3.StopFlags.Immediate); // we need to stop all sounds, so they will start again as 3d and not 2d
                }
                else if (old == MyCameraAttachedToEnum.LargeWeapon)
                {
                    if (ControlledLargeWeapon != null)
                        ControlledLargeWeapon.StopAimingSound();
                }

                if (old != value)
                {
                    m_cameraAttachedTo = value;
                    if (old == MyCameraAttachedToEnum.LargeWeapon ||
                        old == MyCameraAttachedToEnum.Camera ||
                        old == MyCameraAttachedToEnum.Drone)
                    {
                        ReturnControlToPlayerShip();
                    }

                    if (MyCamera.Zoom != null)
                        MyCamera.Zoom.ResetZoom();

                    var handler = CameraAttachedChanged;
                    if (handler != null)
                    {
                        handler(old, value);
                    }
                }
            }
        }

        public event CameraAttachedChangedHandler CameraAttachedChanged;
        public event ControlledEntityHandler CameraContrlolledObjectChanged;

        public bool StartTimeoutClosing = false;
        //public MySoundCue? MusicCue;
        public Vector3 ThirdPersonCameraDelta = new Vector3(-10, 10, 10);

        //public MyGuiEditorControls EditorControls;
        //public MyGuiFoundationFactoryControls FoundationFactoryControls;
        public MyGuiEditorControlsBase EditorControls;

        MyMwcObjectBuilder_Checkpoint m_checkpoint;

        static MyMwcObjectBuilder_Checkpoint m_lastLoadedCheckpoint;

        public MyMwcObjectBuilder_Checkpoint Checkpoint
        {
            get { return m_checkpoint; }
        }

        protected MyGuiScreenGamePlayType m_type;

        MyGuiScreenGamePlayType? m_previousType = null;
        protected MyMwcStartSessionRequestTypeEnum? m_sessionType;

        //  If game-screen is displaying fly-through animation
        //public bool FlyThroughActive;
        protected bool m_firstUpdateCall;
        protected int m_firstDrawCall;
        MyGuiScreenLoading m_loadingScreen;
        Thread m_drawLoadingThread;
        volatile bool m_backgroundWorkerRunning;
        volatile bool m_backgroundWorkerCanRun;

        private bool m_invokeGameEditorSwitch;
        private bool m_hudSectorBorderInEditor = MyConfig.EditorEnableGrid;

        private MyHudNotification.MyNotification m_detachNotification = null;

        protected int? m_startTimeInMilliseconds;

        private int? m_gpsLocationExistsTime;
        private float m_updateGPSReminderTimer = 0;
        private readonly MyHudNotification.MyNotification m_showGPSNotification;
        private MyHudNotification.MyNotification m_needShowHelpNotification;
        private MyHudNotification.MyNotification m_oreInRangeNotification;
        private MyHudNotification.MyNotification m_drillingInRangeNotification;
        private MyHudNotification.MyNotification m_holdFireToDrillNotification;
        private MyHudNotification.MyNotification m_pressMToDeactivateDrillNotification;

        //int? m_lastHandleInputInMilliseconds;
        private bool m_handleInputMouseKeysReleased;

        static int m_multipleLoadsCount = 0;
        bool m_loadMultiple = MyFakes.TEST_MULTIPLE_LOAD_UNLOAD;

        bool PHYSICS_SIMULATION_SLOWDOWN = false;
        long deltaphys = 0;

        static int m_missionGameplayKillsRemaining = 0;
        int m_missionGameplayTestDelay = MyFakes.TEST_MISSION_GAMEPLAY_DURATION;
        static MyMissionID m_lastMissionID = 0;
        static MyObjective m_lastObjective = null;
        struct MyMissionGameplayStats
        {
            public int FPS;
            public float FrameTimeAvg;
            public int FrameTimeMin;
            public int FrameTimeMax;

            public long GC;
            public long WorkingSet;

            public float VideoMemAllocated;
            public float VideoMemAvailable;
        }
        static Dictionary<MyMissionID, Dictionary<MyMissionID, MyMissionGameplayStats>> m_missionGameplayStats = new Dictionary<MyMissionID, Dictionary<MyMissionID, MyMissionGameplayStats>>();

        public MyGuiScreenSecurityControlHUB ActiveSecurityHubScreen;

        protected MyMwcSectorIdentifier m_sectorIdentifier;         //  Current sector identifier
        protected MyMwcSectorIdentifier m_travelSectorIdentifier;         //  Travel to sector
        protected MyMwcTravelTypeEnum m_travelReason;
        protected MyMissionID? m_missionToStart = null; //What mission start after screen load

        private static Vector4 SectorDustColor;                                  //  Dust color of actual sector
        private List<Vector4> PartialDustColor;
        private Vector4 ResultDustColor;

        public bool CanTravel
        {
            get
            {
                return MyMissions.ActiveMission == null && (!MyMultiplayerGameplay.IsRunning || MyMultiplayerGameplay.Static.IsHost);
            }
        }

        float m_distanceToSectorBoundaries = float.MaxValue;
        bool m_transferToNeighbouringSectorStarted = false;

        bool m_assertWasCalledUnloadContentBeforeLoadContent = true;
        //bool m_controlsChange = false;

        //Background resolve
        Texture m_textureForSectorLoadingScreen;
        bool m_isPreparedTextureForSectorLoadingScreen;
        bool m_prepareTextureForSectorLoadingScreen;

        //  Quick zoom was used.
        bool m_quickZoomWasUsed = false;
        bool m_quickZoomOut = false;

        protected MyGuiControlSelectAmmo m_selectAmmoMenu;
        protected MyGuiControlWheelControl m_wheelControlMenu;

        // Background thread was running last update
        bool m_backgroudThreadWasWorking = false;

        private int m_firstTimeLoaded = 0;

        private List<MyChatMessage> m_chatMessages = new List<MyChatMessage>(MyGuiConstants.CHAT_WINDOW_MAX_MESSAGES_COUNT);


        private StringBuilder m_demoEndText = new StringBuilder();
        public bool DrawDemoEnd { get; set; }

        public bool DrawCampaignEnd { get; set; }

        private int m_lastTimeSwitchedDroneControl;

        private readonly MyRender.MyRenderSetup m_secondarySetup;
        MyHudNotification.MyNotification m_notificationYouHaveToBeNearMothership;
        MyHudNotification.MyNotification m_notificationUnableToLeaveSectorMission;

        private float m_madelynRefillTimer;

        public MyPrefabCamera ControlledCamera
        {
            get { return ControlledEntity as MyPrefabCamera; }
            private set { ControlledEntity = value; }
        }

        public MySmallShip ControlledShip
        {
            get { return ControlledEntity as MySmallShip; }
            set { ControlledEntity = value; }
        }

        public MyDrone ControlledDrone
        {
            get { return ControlledEntity as MyDrone; }
            private set { ControlledEntity = value; }
        }

        public MyPrefabLargeWeapon ControlledLargeWeapon
        {
            get { return ControlledEntity as MyPrefabLargeWeapon; }
            private set { ControlledEntity = value; }
        }

        private MyEntity m_entity;
        public MyEntity ControlledEntity
        {
            get { return m_entity; }
            private set
            {
                System.Diagnostics.Debug.Assert(value != null);

                if (m_entity != value)
                {
                    m_entity = value;
                    MyEnemyTargeting.SwitchOwner(value);
                    if (CameraContrlolledObjectChanged != null)
                    {
                        CameraContrlolledObjectChanged(value);
                    }

                }
            }
        }

        public bool IsControlledPlayerShip
        {
            get { return ControlledEntity == MySession.PlayerShip; }
        }

        public bool IsControlledDrone
        {
            get { return ControlledDrone != null; }
        }

        public bool FoundationFactoryDropEnabled { get; set; }

        public static bool IsRenderingInsideEntity(MyEntity entity)
        {
            if (entity == MySession.PlayerShip)
            {
                var isMainRenderInsidePlayerShip = Static.CameraAttachedTo ==
                                                   MyCameraAttachedToEnum.PlayerMinerShip &&
                                                   !MySecondaryCamera.Instance.IsCurrentlyRendering;

                var isSecondaryRenderInsidePlayerShip = MySecondaryCamera.Instance.IsInsidePlayerShip &&
                                                        MySecondaryCamera.Instance.IsCurrentlyRendering;

                return isMainRenderInsidePlayerShip || isSecondaryRenderInsidePlayerShip;
            }

            var squaredDistanceToCamera = Vector3.DistanceSquared(entity.GetPosition(), MyCamera.Position);

            var closeEnoughToCamera = squaredDistanceToCamera < 0.0001f;

            return closeEnoughToCamera;
        }

        public MyGuiScreenGamePlay(MyGuiScreenGamePlayType type, MyGuiScreenGamePlayType? previousType, MyMwcSectorIdentifier sectorIdentifier, int sectorVersion, MyMwcStartSessionRequestTypeEnum? sessionType)
            : base(Vector2.Zero, null, null)
        {
            MySystemTimer.SetByType(type);

            m_sessionType = sessionType;
            m_sectorIdentifier = sectorIdentifier;
            SectorVersion = sectorVersion;
            m_directionToSunNormalized = -MyMwcUtils.Normalize(GetPositionInMillionsOfKm());
            DrawMouseCursor = false;
            m_closeOnEsc = false;
            m_type = type;
            m_previousType = previousType;
            m_firstUpdateCall = true;
            m_firstDrawCall = FIRST_TOTAL_DELAY;
            m_drawEvenWithoutFocus = true;
            m_enableBackgroundFade = true;
            m_canShareInput = false;
            m_screenCanHide = false;

            m_isPreparedTextureForSectorLoadingScreen = false;
            m_prepareTextureForSectorLoadingScreen = false;

            MinerWars.AppCode.Networking.SectorService.MySectorServerCallback.ClearEvents();
            MinerWars.AppCode.Networking.SectorService.MySectorServerCallback.ShutdownNotification += new MinerWars.AppCode.Networking.SectorService.MySectorServerCallback.ShutdownHandler(MySectorServerCallback_ShutdownNotification);

            m_secondarySetup = new MyRender.MyRenderSetup();
            m_secondarySetup.EnabledModules = new HashSet<MyRenderModuleEnum>();
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Cockpit);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.CockpitGlass);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SunGlareAndLensFlare);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.UpdateOcclusions);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticlesPrepare);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometry);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.ParticlesDustField);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.VoxelHand);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.DistantImpostors);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Decals);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.CockpitWeapons);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SunGlow);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SectorBorder);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.DrawSectorBBox);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.DrawCoordSystem);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Explosions);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.BackgroundCube);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.GPS);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.TestField);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticles);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Lights);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometryForward);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Projectiles);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.DebrisField);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.ThirdPerson);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.Editor);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SolarObjects);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SolarMapGrid);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.PrunningStructure);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.SunWind);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.IceStormWind);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.PrefabContainerManager);
            m_secondarySetup.EnabledModules.Add(MyRenderModuleEnum.PhysicsPrunningStructure);

            m_showGPSNotification = new MyHudNotification.MyNotification(MyTextsWrapperEnum.GPSReminder, MyHudConstants.MISSION_FONT, MyHudNotification.DONT_DISAPEAR, null, null);
            m_showGPSNotification.SetTextFormatArguments(new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.GPS) });
        }

        void MyEntities_OnEntityRemove(MyEntity entity)
        {
            if (CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon && ControlledLargeWeapon == entity)
            {
                CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
            }
        }

        void MySectorServerCallback_ShutdownNotification(TimeSpan shutdownOn, TimeSpan shutdownLength, string shutdownMessage)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, new StringBuilder(String.Format(MyTextsWrapper.Get(MyTextsWrapperEnum.ServerShutdownNotification).ToString(),
                shutdownLength.TotalMinutes, shutdownOn.TotalMinutes)), MyTextsWrapper.Get(MyTextsWrapperEnum.ServerShutdownNotificationCaption), MyTextsWrapperEnum.Ok, null));
        }

        public bool CanTravelThroughSectorBoundaries()
        {
            return (m_type != MyGuiScreenGamePlayType.GAME_SANDBOX && CanTravel && (m_type != MyGuiScreenGamePlayType.GAME_STORY || MyClientServer.LoggedPlayer.GetCanSave()))
                && (!MyMultiplayerGameplay.IsRunning || (MyMultiplayerGameplay.Static.IsHost && m_type == MyGuiScreenGamePlayType.GAME_STORY));
        }

        public bool IsPureFlyThroughActive()
        {
            return (m_type == MyGuiScreenGamePlayType.PURE_FLY_THROUGH);
        }

        public bool IsCreditsActive()
        {
            return (m_type == MyGuiScreenGamePlayType.CREDITS);
        }

        public bool IsFlyThroughActive()
        {
            return (m_type == MyGuiScreenGamePlayType.PURE_FLY_THROUGH) || (m_type == MyGuiScreenGamePlayType.CREDITS);
        }

        public bool IsGameActive()
        {
            return ((m_type == MyGuiScreenGamePlayType.GAME_MMO) || (m_type == MyGuiScreenGamePlayType.GAME_STORY) || (m_type == MyGuiScreenGamePlayType.GAME_SANDBOX));
        }

        public bool IsMainMenuActive()
        {
            return (m_type == MyGuiScreenGamePlayType.MAIN_MENU);
        }

        public bool IsEditorActive()
        {
            return (m_type == MyGuiScreenGamePlayType.EDITOR_STORY) || (m_type == MyGuiScreenGamePlayType.EDITOR_MMO) || (m_type == MyGuiScreenGamePlayType.EDITOR_SANDBOX)
                || IsIngameEditorActive();
        }

        public bool IsEditorStoryActive()
        {
            return (m_type == MyGuiScreenGamePlayType.EDITOR_STORY);
        }

        public bool IsGameStoryActive()
        {
            return (m_type == MyGuiScreenGamePlayType.GAME_STORY);
        }

        public bool IsEditorMmoActive()
        {
            return (m_type == MyGuiScreenGamePlayType.EDITOR_MMO);
        }

        public bool IsEditorSandboxActive()
        {
            return (m_type == MyGuiScreenGamePlayType.EDITOR_SANDBOX);
        }

        public bool IsIngameEditorActive()
        {
            return m_type == MyGuiScreenGamePlayType.INGAME_EDITOR;
        }

        public bool CheatsEnabled()
        {
            if (MyClientServer.LoggedPlayer == null)
                return false;
            return (m_type != MyGuiScreenGamePlayType.GAME_MMO)
                && MyClientServer.LoggedPlayer.GetUseCheats()
                && (MyFakes.MULTIPLAYER_CHEATS_ENABLED || (!MyMultiplayerGameplay.IsRunning || m_type != MyGuiScreenGamePlayType.GAME_SANDBOX));
        }

        public bool IsCheatEnabled(MyGameplayCheatsEnum cheat)
        {
            return CheatsEnabled() && MyGameplayCheats.IsCheatEnabled(cheat);
        }

        public MyGuiScreenGamePlayType GetGameType()
        {
            return m_type;
        }

        public MyGuiScreenGamePlayType? GetPreviousGameType()
        {
            return m_previousType;
        }

        public MyMwcStartSessionRequestTypeEnum? GetSessionType()
        {
            return m_sessionType;
        }

        public string GetSessionTypeFriendlyName()
        {
            if (m_sessionType == null)
            {
                return string.Empty;
            }
            else
            {
                return MyEnumsToStrings.SessionType[(int)m_sessionType.Value];
            }
        }

        //  Only certain game types are pause-able
        public bool IsPausable()
        {
            return m_type != MyGuiScreenGamePlayType.MAIN_MENU && !MyMultiplayerGameplay.OtherPlayersConnected;
        }

        public MyMwcSectorIdentifier GetSectorIdentifier()
        {
            return m_sectorIdentifier;
        }

        public void SetSectorName(string name)
        {
            m_sectorIdentifier.SectorName = name;
        }

        public int SectorVersion { get; private set; }

        public bool IsFirstPersonView
        {
            get
            {
                return CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip ||
                       CameraAttachedTo == MyCameraAttachedToEnum.BotMinerShip ||
                       CameraAttachedTo == MyCameraAttachedToEnum.Camera ||
                       CameraAttachedTo == MyCameraAttachedToEnum.Drone ||
                       CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon;
            }
        }

        //public void SetControlsChange(bool Value)
        //{
        //    m_controlsChange = Value;
        //}

        //public bool GetControlsChange()
        //{
        //    return m_controlsChange;
        //}

        public void AddEnterSectorResponse(MyMwcObjectBuilder_Checkpoint checkpoint, MyMissionID? missionToStart)
        {
            m_checkpoint = checkpoint;
            if (missionToStart != null)
            {
                m_missionToStart = missionToStart;
            }
        }

        /// <summary>
        /// Stores last loaded checkpoint to check missing objects
        /// </summary>
        public static void StoreLastLoadedCheckpoint(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            m_lastLoadedCheckpoint = checkpoint;
        }

        public static List<MyMwcObjectBuilder_Base> FindMissingObjectBuilders(MyMwcObjectBuilder_Checkpoint checkpointToSave)
        {
            var result = new List<MyMwcObjectBuilder_Base>();
            // Fill dictionary with loaded objects
            var loaded = new Dictionary<uint, MyMwcObjectBuilder_Base>();
            GetObjects(m_lastLoadedCheckpoint, loaded);

            // Fill dictionary with saved objects
            var saved = new Dictionary<uint, MyMwcObjectBuilder_Base>();
            GetObjects(checkpointToSave, saved);

            // Removed saved objects from loaded dict
            foreach (var savedObject in saved)
            {
                loaded.Remove(savedObject.Key);
            }

            // Loaded now contains objects which were not saved
            result.AddRange(loaded.Values);
            return result;
        }

        static void GetObjects(MyMwcObjectBuilder_Base builder, Dictionary<uint, MyMwcObjectBuilder_Base> addToDictionary)
        {
            if (builder == null)
                return;

            if (builder.EntityId.HasValue)
            {
                addToDictionary.Add(builder.EntityId.Value, builder);
            }

            if (builder is MyMwcObjectBuilder_Checkpoint)
            {
                GetObjects(((MyMwcObjectBuilder_Checkpoint)builder).SectorObjectBuilder, addToDictionary);
            }

            var sectorBuilder = builder as MyMwcObjectBuilder_Sector;
            if (sectorBuilder != null && sectorBuilder.SectorObjects != null)
            {
                foreach (var ob in sectorBuilder.SectorObjects)
                {
                    GetObjects(ob, addToDictionary);
                }
            }

            var containerBuilder = builder as MyMwcObjectBuilder_PrefabContainer;
            if (containerBuilder != null && containerBuilder.Prefabs != null)
            {
                foreach (var ob in containerBuilder.Prefabs)
                {
                    GetObjects(ob, addToDictionary);
                }
            }
        }

        //  IMPORTANT: This method will be called in background thread so don't mess with main thread objects!!!
        public override void RunBackgroundThread()
        {
            int runBackgroundBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenGameplay.RunBackgroundThread", ref runBackgroundBlock);

            MyMwcLog.WriteLine("MyGuiScreenGamePlay.RunBackgroundThread - START");
            MyMwcLog.IncreaseIndent();

            MyPerformanceTimer.GuiScreenGamePlay_RunBackgroundThread.Start();

            int startBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Start", ref startBlock);

            base.RunBackgroundThread();

            m_selectAmmoMenu = new MyGuiControlSelectAmmo(this, Vector2.Zero, Vector2.Zero, MyGuiConstants.SELECT_AMMO_BACKGROUND_COLOR);
            m_wheelControlMenu = new MyGuiControlWheelControl(this);

            //  Reset "pause" flag to not-paused
            if (MyMinerGame.IsPaused())
                MyMinerGame.SwitchPause();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(startBlock);

            //if (IsEditorActive() || IsGameActive())
            //{
            //    EditorControls = new MyGuiEditorControls(this);
            //    FoundationFactoryControls = new MyGuiFoundationFactoryControls(this);
            //}

            if (IsTypeEditorGodMode(m_previousType) || IsTypeEditorGodMode(m_type))
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiGodEditorControls");
                EditorControls = new MyGuiGodEditorControls(this);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiIngameEditorControls");
                EditorControls = new MyGuiIngameEditorControls(this);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor.Static.SetActive");
            //@ set proper active state to editor
            MyEditor.Static.SetActive(IsEditorActive());
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //  This delay is here because when user starts new game through main menu, we play click sound and this sound must
            //  be finished before we start with heavy work, even if on separate thread
            //  I call it after MySounds.UnloadContent(), because then turning-off game current sounds is instant.
            //  It is also good when turning off the game - so click sound has time to finish.
            //  Perhaps the delay was made by GC.Collect....
            //Thread.Sleep(250);    //  Not needed anymore because we load on main thread and ther are no sound interferences


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Unload");
            if (Static != null)
            {
                //  We must unload instance that is currently set to Static, therefore active gameplay
                //  My first mistake was unloading without specified "Static" so we in fact unloaded this new object
                Static.UnloadContent();
                Static.UnloadData();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            // Fill actual notifications:
            MyHudNotification.SetCurrentScreen(m_type);
            MySystemTimer.SetByType(m_type);

            if (MyFakes.UNLOAD_OPTIMIZATION_KEEP_USED_MODELS)
            {
                UnloadUnused(); // Unloads unused models, textures and voxel textures
            }
            else
            {
                MyModels.UnloadContent();
                MyModels.UnloadData();
            }

            LoadData();//load initialize all data strucutres etc

            LoadContent();// load all disposable or dynamic graphics content

            // This creates objects from object builders
            LoadObjects(); // needs graphics and data content together

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Editor.Misc things");

            if (IsEditorActive() == false)
            {
                if (MySession.PlayerShip != null)
                {
                    MySpectator.Position = MySession.PlayerShip.GetPosition() + ThirdPersonCameraDelta;
                    MySpectator.Target = MySession.PlayerShip.GetPosition();
                }
                else
                {
                    MySpectator.Position = Vector3.Zero;
                    MySpectator.Target = Vector3.Forward;
                }
            }
            else
            {
                if (!MyFakes.MWBUILDER)
                {
                    MySpectator.Position = MySession.PlayerShip.GetPosition() + (50 * MySession.PlayerShip.WorldMatrix.Forward);
                    //MySpectator.Target = MySpectator.Position + (new Vector3(0, 180, 180));
                    MySpectator.Target = MySession.PlayerShip.GetPosition();

                    if (MyHudSectorBorder.Enabled != MyConfig.EditorEnableGrid) MyHudSectorBorder.SwitchSectorBorderVisibility();
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Final GC and finalize");
            MyPerformanceTimer.GuiScreenGamePlay_RunBackgroundThread.End();

            GC.Collect(2);
            GC.Collect(1);
            GC.Collect(0);

            GC.WaitForPendingFinalizers();

            GC.Collect();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.RunBackgroundThread - END");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(runBackgroundBlock);
        }

        private void UnloadUnused()
        {
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadUnused - START");

            if (m_checkpoint == null)
            {
                MyMwcLog.WriteLine("m_checkpoint == null");

                MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadUnused - END");
            }

            if (m_checkpoint.SectorObjectBuilder == null)
            {
                MyMwcLog.WriteLine("m_checkpoint.SectorObjectBuilder == null");

                MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadUnused - END");
            }

            // Here clear unused models
            HashSet<int> usedModels = new HashSet<int>();

            if (m_checkpoint.SectorObjectBuilder != null && m_checkpoint.SectorObjectBuilder.SectorObjects != null)
            {
                foreach (var container in m_checkpoint.SectorObjectBuilder.SectorObjects.OfType<MyMwcObjectBuilder_PrefabContainer>())
                {
                    foreach (var prefab in container.Prefabs)
                    {
                        MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(prefab.GetObjectBuilderType(), prefab.GetObjectBuilderId().Value);
                        usedModels.Add((int)config.ModelLod0Enum);
                        if (config.ModelLod1Enum.HasValue)
                            usedModels.Add((int)config.ModelLod1Enum.Value);
                        if (config.ModelCollisionEnum.HasValue)
                            usedModels.Add((int)config.ModelCollisionEnum.Value);

                        if (config is MyPrefabConfigurationKinematic)
                        {
                            var kinematicConfig = (MyPrefabConfigurationKinematic)config;
                            foreach (var part in kinematicConfig.KinematicParts)
                            {
                                usedModels.Add((int)part.ModelLod0Enum);

                                if (part.m_modelMovingEnum.HasValue)
                                    usedModels.Add((int)part.m_modelMovingEnum.Value);
                                if (part.ModelCollisionEnum.HasValue)
                                    usedModels.Add((int)part.ModelCollisionEnum.Value);
                                if (part.ModelLod1Enum.HasValue)
                                    usedModels.Add((int)part.ModelLod1Enum.Value);
                            }
                        }
                    }
                }
            }

            // TODO: unload optimization of other stuff can be enabled when we're not low on memory
            //foreach (var spawnPoint in m_checkpoint.SectorObjectBuilder.SectorObjects.OfType<MyMwcObjectBuilder_SpawnPoint>())
            //{
            //    foreach (var ship in spawnPoint.ShipTemplates)
            //    {
            //        var shipProps = MyShipTypeConstants.GetShipTypeProperties(ship.ShipType);
            //        usedModels.Add((int)shipProps.Visual.ModelLod0Enum);
            //        if (shipProps.Visual.ModelLod1Enum.HasValue)
            //            usedModels.Add((int)shipProps.Visual.ModelLod1Enum.Value);
            //    }
            //}

            //// Bots
            //foreach (var ship in m_checkpoint.SectorObjectBuilder.SectorObjects.OfType<MyMwcObjectBuilder_SmallShip_Bot>())
            //{
            //    if (ship.ShipType != 0)
            //    {
            //        var shipProps = MyShipTypeConstants.GetShipTypeProperties(ship.ShipType);
            //        usedModels.Add((int)shipProps.Visual.ModelLod0Enum);
            //        if (shipProps.Visual.ModelLod1Enum.HasValue)
            //            usedModels.Add((int)shipProps.Visual.ModelLod1Enum.Value);
            //    }
            //}

            //foreach (var cargo in m_checkpoint.SectorObjectBuilder.SectorObjects.OfType<MyMwcObjectBuilder_CargoBox>())
            //{
            //    var model = MinerWars.AppCode.Game.Entities.CargoBox.MyCargoBox.GetModelLod0EnumFromType(cargo.CargoBoxType);
            //    usedModels.Add((int)model);
            //}

            //// Player ship
            //if (m_checkpoint.PlayerObjectBuilder != null)
            //{
            //    var playShipProps = MyShipTypeConstants.GetShipTypeProperties(m_checkpoint.PlayerObjectBuilder.ShipObjectBuilder.ShipType);
            //    usedModels.Add((int)playShipProps.Visual.ModelLod0Enum);
            //    usedModels.Add((int)playShipProps.Visual.CockpitGlassModel);
            //    usedModels.Add((int)playShipProps.Visual.CockpitInteriorModel);
            //    if (playShipProps.Visual.ModelLod1Enum.HasValue)
            //        usedModels.Add((int)playShipProps.Visual.ModelLod1Enum.Value);
            //}

            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadUnused - MIDDLE");

            MyModels.UnloadExcept(usedModels);
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadUnused - END");
        }


        public void UpdateScreenSize()
        {
            //  If gameplay screen doesn't run yet
            if (Static == null) return;

            if (Static.EditorControls != null) EditorControls.UpdateScreenSize();
        }

        public override void LoadData()
        {
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.LoadData - START");
            MyMwcLog.IncreaseIndent();

            m_multipleLoadsCount++;

            int loadDataBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenGamePlay.LoadData", ref loadDataBlock);

            MyPerformanceTimer.GuiScreenGamePlay_LoadData.Start();

            Static = this;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySunGlare.UpdateSectorInfo");
            MySunGlare.UpdateSectorInfo();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiScreenGamePlay.InitSounds");

            InitSounds();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiScreenGamePlay.MyHudSectorBorder block");
            MyHudSectorBorder.LoadData();
            MyDistantImpostors.LoadData();
            MyLights.LoadData();
            MyExplosions.LoadData();
            MyProjectiles.LoadData();
            MyCockpitGlassDecals.LoadData();
            MyModels.LoadData();

            if (IsEditorActive())
            {
                MyGuiManager.LoadPrefabPreviews();
            }
            MyTransparentGeometry.LoadData();
            MyParticlesDustField.LoadData();
            MyHud.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiScreenGamePlay.MyVoxelMaterials block");
            MyVoxelMaterials.LoadData();

            MyVoxelGenerator.LoadData();
            MyDebrisField.LoadData();

            MyVoxelContentCellContents.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiScreenGamePlay.MyVoxelCacheData block");
            MyVoxelCacheData.LoadData();
            MyVoxelCacheCellRenderHelper.LoadData();
            MyVoxelCacheRender.LoadData();
            MyVoxelPrecalc.LoadData();
            MyVoxelMaps.LoadData();

            MyWayPointGraph.LoadData();

            MyPhysics physics = new MyPhysics();
            physics.InitializePhysics();
            MyConstants.InitializeCollisionLayers();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiScreenGamePlay.MyAmmoConstants block");
            MyAmmoConstants.LoadData();

            MyEntities.LoadData();

            MyMissiles.LoadData();
            MyHologramShips.LoadData();
            MyCannonShots.LoadData();
            MyUniversalLauncherShells.LoadData(true);
            MyExplosionDebrisVoxel.LoadData();
            MyExplosionDebrisModel.LoadData();

            MyEditor.Static.LoadData();
            MyFriendlyFire.Load();

            if (MyFakes.ENABLE_SHOUT)
            {
                MyShouts.LoadData();
            }

            // load editor controls also when game active, because we can enter editor during gameplay
            if (IsEditorActive() || IsGameActive())
            {
                EditorControls.LoadData();
                //FoundationFactoryControls.LoadData();
            }

            m_invokeGameEditorSwitch = false;
            m_lastTimeSwitchedDroneControl = MyConstants.FAREST_TIME_IN_PAST;

            MyPerformanceTimer.GuiScreenGamePlay_LoadData.End();

            PartialDustColor = new List<Vector4>();

            m_notificationYouHaveToBeNearMothership = new MyHudNotification.MyNotification(MyTextsWrapperEnum.NotificationYouHaveToBeNearMothership, MyGuiManager.GetFontMinerWarsRed(), (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
            m_notificationUnableToLeaveSectorMission = new MyHudNotification.MyNotification(MyTextsWrapperEnum.UnableToLeaveSectorMission, MyGuiManager.GetFontMinerWarsRed(), (int)TimeSpan.FromSeconds(5).TotalMilliseconds);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(loadDataBlock);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.LoadData - END");
        }


        //  IMPORTANT: This method will be called in background thread so don't mess with main thread objects!!!
        public override void LoadContent()
        {
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            Static = this;
            MyMinerGame.IsGameReady = false;

#if DETECT_LEAKS
            MyRender.LoadContent();              
#endif

            MySimpleObjectDraw.LoadContent();

            MyEntities.OnEntityRemove += new Action<MyEntity>(MyEntities_OnEntityRemove);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadContent");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Part1");

            MyCommonDebugUtils.AssertRelease(m_assertWasCalledUnloadContentBeforeLoadContent == true);

            MyPerformanceTimer.GuiScreenGamePlay_LoadContent.Start();

            if (IsEditorActive()) CameraAttachedTo = MyCameraAttachedToEnum.Spectator;

            MyMwcObjectBuilder_InventoryItem.ReloadDisabledItems(MySession.Is25DSector);

            MyHudSectorBorder.LoadContent();
            MyBackgroundCube.LoadContent(GetSectorIdentifier());
            MyTransparentGeometry.LoadContent();

            MyHud.LoadContent(this);
            //MyHudRadar.LoadContent();
            MyHudGPS.LoadContent();
            MyDecals.LoadContent();
            MyCockpitGlassDecals.LoadContent();
            MyCockpitGlass.LoadContent();
            MyCockpitWeapons.LoadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Part2");

            // TODO: petrzilka, refactor
            List<MyMwcObjectBuilder_Base> objects = null;
            if (m_checkpoint != null && m_checkpoint.SectorObjectBuilder != null)
            {
                objects = m_checkpoint.SectorObjectBuilder.SectorObjects;
            }

            MyDistantImpostors.LoadContent(objects, false /*IsGameActive() || IsEditorActive()*/);//ok

            MyVoxelCacheData.LoadContent();
            MyVoxelCacheRender.LoadContent();

            MyExplosions.LoadContent();
            MyModels.LoadContent();
            MyModelSubObjects.LoadContent();
            MySunWind.LoadContent();
            MyIceStorm.LoadContent();
            MyHudNotification.LoadContent();
            MyVoxelMaterials.LoadContent();
            MyVoxelMaps.LoadContent();
            MyDebrisField.LoadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Part3");

            MyEditor.Static.LoadContent();
            MyCamera.LoadContent();

            // load editor controls also when game active, because we can enter editor during gameplay
            if (IsEditorActive() || IsGameActive())
            {
                EditorControls.LoadContent();
                if (IsEditorActive())
                {
                    EditorControls.AddEditorControlsToList(Controls);
                }
                //FoundationFactoryControls.LoadContent();
            }

            MyEditor.Static.LoadContent();
            MyCamera.LoadContent();
            MyPerformanceTimer.GuiScreenGamePlay_LoadContent.End();

            MyEnvironmentMap.Reset();

            m_assertWasCalledUnloadContentBeforeLoadContent = false;

            //  Base load content must be called after child's load content
            base.LoadContent();

            //  Do GC collect as last step. Reason is that after we loaded new level, a lot of garbage is created and we want to clear it now and not wait until GC decides so.
            //  Sleep is there because we want GC to finish before this background thread ends. We don't want tearing during main menu or game is displaying.
            GC.Collect();
            //Thread.Sleep(100);    //  Not needed anymore because we load on main thread and ther are no sound interferences

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.LoadContent - END");
        }

        public override void UnloadData()
        {
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadData - START");
            MyMwcLog.IncreaseIndent();

            // duplicated call (called in UnloadContent)
            //if (EditorControls != null)
            //{
            //    EditorControls.UnloadContent();
            //}
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Cleanup missions");
            // Session.Init is not called so cleanup on sector enter
            MyMissions.Unload();
            //m_missionToStart = null; //we need it to load
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRadar.UnloadData();
            MyHud.UnloadData();
            MyEditor.Static.UnloadData();

            MyVoxelMaps.AutoRecalculateVoxelMaps = false;

            MyEntities.UnloadData();

            MyVoxelMaps.AutoRecalculateVoxelMaps = true;

            MyExplosionDebrisVoxel.UnloadData();
            MyExplosionDebrisModel.UnloadData();
            MyUniversalLauncherShells.UnloadData();
            MyCannonShots.UnloadData();
            MyMissiles.UnloadData();
            MyHologramShips.UnloadData();
            MyHudNotification.ClearAllNotifications();
            MyHudWarnings.UnloadData();
            MyDebrisField.UnloadData();
            MyNuclearExplosion.Unload();
            MyEnemyTargeting.Unload();
            MyPathfindingHelper.Unload();
            MySmallShipBot.TotalAliveBots = 0;

            // HOTFIX:
            MyExplosions.UnloadData();
            MyProjectiles.UnloadData();

            //Lights needs to be after explosion, because explosion can keep them
            MyLights.UnloadData();


            if ((IsFlyThroughActive() == false) && (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE == true))
            {
                MyTrailerSave.Save();
            }

            if (MyPhysics.physicsSystem != null)
            {
                MyPhysics.physicsSystem.DestroyPhysics();
            }

            MyWayPointGraph.UnloadData();
            MyVoxelMaps.UnloadData();
            MyVoxelPrecalc.UnloadData();
            MyVoxelCacheRender.UnloadData();
            MyVoxelCacheCellRenderHelper.UnloadData();
            MyVoxelCacheData.UnloadData();
            MyVoxelContentCellContents.UnloadData();
            MyVoxelGenerator.UnloadData();
            //MyVoxelMaterials.UnloadData();

            MyDistantImpostors.UnloadData();

            MyPrefabContainerManager.GetInstance().UnloadData();
            MyDistantImpostors.UnloadData();
            //MyModels.UnloadData();

            //MyRender.UnloadData();
            MyDialogues.UnloadData();
            MyFriendlyFire.Unload();
            MyShouts.UnloadData();

            Parallel.Clean();

            Static = null;
            MyMwcLog.DecreaseIndent();
            GC.Collect(2);
            GC.Collect(1);
            GC.Collect(0);


            int modelMeshes2 = MyPerformanceCounter.PerAppLifetime.MyModelsMeshesCount;
            int modelVertices2 = MyPerformanceCounter.PerAppLifetime.MyModelsVertexesCount;
            int modelTriangles2 = MyPerformanceCounter.PerAppLifetime.MyModelsTrianglesCount;
            MyPerformanceCounter.MyPerAppLifetime app = MyPerformanceCounter.PerAppLifetime;

            // After data unload, render structures should be definitelly empty
            //MyRender.AssertStructuresEmpty();

            m_entity = null;
            m_checkpoint = null;

            //MySession.PlayerShip = null;

            //var o = SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();

            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadData - END");
        }

        //  IMPORTANT: This method will be called in background thread so don't mess with main thread objects!!!
        //  UPDATE: called always when GDevice is disposed
        public override void UnloadContent()
        {
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            base.UnloadContent();

            m_assertWasCalledUnloadContentBeforeLoadContent = true;

            MyBackgroundCube.UnloadContent();
            MyHudSectorBorder.UnloadContent();
            MyVoxelCacheData.UnloadContent();
            MyVoxelCacheRender.UnloadContent();
            MyVoxelMaterials.UnloadContent();
            MyWayPoint.CleanBlockedEdges();

            MyTransparentGeometry.UnloadContent();
            MyExplosions.UnloadContent();
            //MyModels.UnloadContent();
            MyDecals.UnloadContent();
            MyHud.UnloadContent();
            MyHudGPS.UnloadContent();
            MyCockpitGlassDecals.UnloadContent();
            MyCockpitGlass.UnloadContent();
            MyCockpitWeapons.UnloadContent();
            MyModelSubObjects.UnloadContent();
            MySunWind.UnloadContent();
            MyMeteorWind.UnloadContent();
            MyIceStorm.UnloadContent();

            MyPrefabContainerManager.GetInstance().UnloadContent();
            MyDistantImpostors.UnloadContent();

            MyEditor.Static.UnloadContent();

            if (EditorControls != null)
            {
                EditorControls.RemoveEditorControlsFromList(Controls);
                EditorControls.UnloadContent();
            }

            MyEntities.OnEntityRemove -= new Action<MyEntity>(MyEntities_OnEntityRemove);

            //MyRender.UnloadContent();
            MyRender.Clear();

            MySimpleObjectDraw.UnloadContent();

#if DETECT_LEAKS            
            MyTextureManager.UnloadContent();
            MyModels.UnloadContent();
            MyRender.UnloadContent();              
#endif

            MyAudio.UnloadContent();
            //if (FoundationFactoryControls != null) FoundationFactoryControls.UnloadContent();

            //  Do GC collect as last step. Reason is that after we loaded new level, a lot of garbage is created and we want to clear it now and not wait until GC decides so.
            GC.Collect();

            //here, nothing can be allocated in VM except device and core stuff
#if DETECT_LEAKS   
            //MyTexture2D tex = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Loading");

            var o = SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
#endif

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenGamePlay.UnloadContent - END");
        }

        private bool IsTypeEditorGodMode(MyGuiScreenGamePlayType? type)
        {
            if (type == null)
                return false;
            switch (type.Value)
            {
                case MyGuiScreenGamePlayType.EDITOR_STORY:
                case MyGuiScreenGamePlayType.EDITOR_MMO:
                case MyGuiScreenGamePlayType.EDITOR_SANDBOX:
                    return true;
                    break;
            }
            return false;
        }

        public void TrySwitchBetweenGameAndEditor()
        {
            //from game -> to god editor
            if (IsGameActive() && IsTypeEditorGodMode(m_previousType))
            {
                MySpectator.Position = MySession.PlayerShip.GetPosition() + (50 * MySession.PlayerShip.WorldMatrix.Up);
                MySpectator.Target = MySession.PlayerShip.GetPosition();
                MyDialogues.Stop();
                MyAudio.Stop();//stop all 3d sounds
                CameraAttachedTo = MyCameraAttachedToEnum.Spectator;
                MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
                m_type = m_previousType.Value;
                m_previousType = null;
                MyEditor.Static.SetActive(true);
                EditorControls.AddEditorControlsToList(Controls);

                // Reset spawnpoints, delete their bots from scene
                foreach (var spawnpoint in MyEntities.GetEntities().OfType<MySpawnPoint>().ToArray())
                {
                    spawnpoint.Reset();
                }

                //MyEditor.DisablePhysicsAndResetStatesOnAllObjectsInSector();// also ensure we stop all updates and all objects newly inserted into sector are with phyisics defaultly off
                if (MyHudSectorBorder.Enabled != m_hudSectorBorderInEditor)
                    MyHudSectorBorder.SwitchSectorBorderVisibility();

                m_wheelControlMenu.HideScreenIfPossible();
                MyCamera.Zoom.ResetZoom();
            }
            else if (IsTypeEditorGodMode(m_type) && m_previousType == null)
            {
                //from god-editor to -> gameplay
                //we need to enable physics
                //be sure we dont entry in game with any objects in collision                
                //because when you have selected entities, then they are not colliding, so we must clear selection and handle colliding entities now
                List<MyEntity> tempSelectedEntities = MyEditorGizmo.SelectedEntities.ToList();
                MyEditorGizmo.ClearSelection();
                if (MyEditor.Static.CollidingElements.Count == 0)
                {
                    m_previousType = m_type;
                    m_type = MyGuiScreenGamePlayType.GAME_SANDBOX;
                    CameraAttachedTo = MySession.PlayerShip != null ? MySession.PlayerShip.Config.ViewMode.GetCameraMode() : MyCameraAttachedToEnum.PlayerMinerShip;
                    MyEditor.Static.SetActive(false);
                    EditorControls.RemoveEditorControlsFromList(Controls);

                    //MyEditor.EnablePhysicsOnAllActiveObjectsInSector();
                    // sector borders disabled by default in gameplay
                    m_hudSectorBorderInEditor = MyHudSectorBorder.Enabled;
                    if (MyHudSectorBorder.Enabled == true)
                        MyHudSectorBorder.SwitchSectorBorderVisibility();

                    m_wheelControlMenu.HideScreenIfPossible();

                    if (MyVideoModeManager.IsHardwareCursorUsed())
                        MyGuiInput.SetMouseToScreenCenter();

                    MyCamera.Zoom.ResetZoom();

                    // Reset spawnpoints, delete their bots from scene
                    foreach (var spawnpoint in MyEntities.GetEntities().OfType<MySpawnPoint>().ToArray())
                    {
                        spawnpoint.ResetBotsSpawnTime();
                    }
                }
                else
                {
                    MyEditorGizmo.AddEntitiesToSelection(tempSelectedEntities);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.FixCollisions, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                }
            }
            else if (m_type != MyGuiScreenGamePlayType.INGAME_EDITOR)
            {
                // 0006155: Disable builder mode (press 0 when you close to mother ship)
                if (!MyFakes.ENABLE_BUILDER_MODE)
                {
                    return;
                }

                // from normal game to ingame editor
                MySmallShip playersSmallShip = MySession.PlayerShip as MySmallShip;
                if (playersSmallShip == null)
                {
                    return;
                }
                MyPrefabFoundationFactory foundationFactory = null;
                MyEntity detectedPrefabContainer = playersSmallShip.BuildDetector.GetNearestEntity();
                if (detectedPrefabContainer != null)
                {
                    MyPrefabContainer prefabContainer = detectedPrefabContainer as MyPrefabContainer;
                    if (prefabContainer.ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory))
                    {
                        foundationFactory = prefabContainer.GetPrefabs(CategoryTypesEnum.FOUNDATION_FACTORY)[0] as MyPrefabFoundationFactory;
                    }
                }
                if (foundationFactory == null)
                {
                    if (FoundationFactoryDropEnabled)
                    {
                        m_invokeGameEditorSwitch = false;
                        return;
                    }

                    bool addFoundationFactoryResult;
                    foundationFactory = MyPrefabFoundationFactory.TryCreateFoundationFactory(MySession.Static.Player, out addFoundationFactoryResult);
                    if (!addFoundationFactoryResult)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.NotificationYouHaveNoFoundationFactory, MyTextsWrapperEnum.TitleYouCantBuild, MyTextsWrapperEnum.Ok, null));
                        m_invokeGameEditorSwitch = false;
                        return;
                    }
                }

                MySmallShip playerShip = MySession.PlayerShip;

                MySpectator.Position = playerShip.GetPosition() + (50 * playerShip.WorldMatrix.Up);
                MySpectator.Target = playerShip.GetPosition();
                CameraAttachedTo = MyCameraAttachedToEnum.Spectator;
                MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
                MyAudio.Stop();//stop all 3d sounds
                m_previousType = m_type;
                m_type = MyGuiScreenGamePlayType.INGAME_EDITOR;
                MyEditor.Static.SetActive(true);
                //EditorControls.AddEditorControlsToList(ref Controls);
                //FoundationFactoryControls.AddEditorControlsToList(ref Controls);
                EditorControls.AddEditorControlsToList(Controls);

                MyEditor.Static.EnterInGameEditMode(foundationFactory);// this will enter my own container and lock it                 

                // sector borders enabled by default in editor
                if (MyHudSectorBorder.Enabled == false)
                    MyHudSectorBorder.SwitchSectorBorderVisibility();

                m_wheelControlMenu.HideScreenIfPossible();
                MyCamera.Zoom.ResetZoom();
            }
            else
            {
                //from ingame editor back to the game
                //beause when you have selected entities, then they are not colliding, so we must clear selection and handle colliding entities now
                List<MyEntity> tempSelectedEntities = MyEditorGizmo.SelectedEntities.ToList();
                MyEditorGizmo.ClearSelection();
                if (MyEditor.Static.CollidingElements.Count == 0)
                {

                    m_type = m_previousType.Value;
                    m_previousType = null;
                    CameraAttachedTo = MySession.PlayerShip != null ? MySession.PlayerShip.Config.ViewMode.GetCameraMode() : MyCameraAttachedToEnum.PlayerMinerShip;
                    MyEditor.Static.SetActive(false);
                    //EditorControls.RemoveEditorControlsFromList(ref Controls);
                    //FoundationFactoryControls.RemoveEditorControlsFromList(ref Controls);
                    EditorControls.RemoveEditorControlsFromList(Controls);
                    MyEditor.Static.ExitInGameEditMode();

                    // sector borders disabled by default in gameplay
                    if (MyHudSectorBorder.Enabled == true)
                        MyHudSectorBorder.SwitchSectorBorderVisibility();

                    m_wheelControlMenu.HideScreenIfPossible();
                    MyCamera.Zoom.ResetZoom();

                    if (MyVideoModeManager.IsHardwareCursorUsed())
                        MyGuiInput.SetMouseToScreenCenter();
                }
                else
                {
                    MyEditorGizmo.AddEntitiesToSelection(tempSelectedEntities);
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.FixCollisions, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                }
            }

            m_invokeGameEditorSwitch = false;
        }

        string nextTrailerName = "000";

        Matrix latestTrailerStart;

        private void RestartTrailer(bool saveCurrentRecording, bool resetPos)
        {
            string text = saveCurrentRecording ? "Trailer saved & restarted, recording..." : "Trailer restarted, recording...";

            MyHudNotification.AddNotification(new MyHudNotification.MyNotification(text, 3000));
            if (saveCurrentRecording)
            {
                MyTrailerSave.Save();
                MyTrailerSave.RemoveTrackedObjects();
            }
            string freeTrailerName;
            Matrix? lastPos;
            MyTrailerLoad.LoadFromUserFolder(out freeTrailerName, out lastPos);
            if (resetPos && lastPos.HasValue)
            {
                var m = lastPos.Value;
                m.Translation += m.Backward * 15;
                MySession.PlayerShip.WorldMatrix = m;
            }
            nextTrailerName = freeTrailerName;
            StartTrailerRecording();
        }

        private void StartTrailerRecording()
        {
            //MyHudNotification.AddNotification(new MyHudNotification.MyNotification("Recording started", 3000));
            MyTrailerSave.RemoveTrackedObjects();
            MyTrailerSave.ResetTicks();
            MyTrailerSave.AttachPhysObject(nextTrailerName, MySession.PlayerShip);
        }

        public bool DisableBackCamera { get; set; }
        //  This method is called every update (but only if application has focus)
        public override void HandleUnhandledInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            //  This is protection against situation when user clicks on CONTINUE in menu, but don't release the button quickly, so in gameplay screen
            //  it continues with shoting. So in this case he must release buttons and then press again if he want to shot.
            if (m_handleInputMouseKeysReleased == false)
            {
                if ((input.IsLeftMouseReleased()) && (input.IsRightMouseReleased()))
                {
                    m_handleInputMouseKeysReleased = true;
                }
            }

            //Set if ammo select menu or config menu is enable
            m_selectAmmoMenu.IsEnabled = !m_wheelControlMenu.Visible && IsControlledPlayerShip;
            m_wheelControlMenu.IsEnable = !m_selectAmmoMenu.Visible && IsControlledPlayerShip;

            if (IsControlledPlayerShip && !m_selectAmmoMenu.IsEnabled && !m_wheelControlMenu.IsEnable)
            {
                m_selectAmmoMenu.IsEnabled = true;
            }

            //  Launch main menu
            if (!m_wheelControlMenu.Visible && !IsSelectAmmoVisible() && input.IsNewKeyPress(Keys.Escape) && !(IsEditorActive() && (EditorControls.TryExitSelected() || EditorControls.TryExitSelectionMode())))
            {
                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);

                //Allow changing video options from game in DX version
                MyGuiScreenMainMenu.AddMainMenu(true);
                //MyGuiScreenMainMenu.AddMainMenu(false);

                DrawHud = false;

                MySystemTimer.SetMinimalResolution();
            }


            MyHudSectorBorder.SwitchToDraw();

            // Switch from player ship to ingame editor and back
            if (IsGameActive() || IsEditorActive())
            {
                if (!input.IsAnyAltPress())
                {
                    if ((input.IsNewKeyPress(Keys.NumPad0) || input.IsNewKeyPress(Keys.D0)) && (GetSessionType() == MyMwcStartSessionRequestTypeEnum.EDITOR_STORY || GetSessionType() == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX || GetSessionType() == MyMwcStartSessionRequestTypeEnum.EDITOR_MMO))
                    {
                        ReturnControlToPlayerShip();
                        m_invokeGameEditorSwitch = true;
                    }
                }
            }

            if (CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing)
            {
                if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                {
                    ThirdPersonCameraDelta /= 1.1f;
                }
                else if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                {
                    ThirdPersonCameraDelta *= 1.1f;
                }
            }

            if (CameraAttachedTo == MyCameraAttachedToEnum.Spectator)
            {
                if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                {
                    MySpectator.SpeedMode = Math.Min(MySpectator.SpeedMode * 1.5f, MyEditorConstants.MAX_EDITOR_CAMERA_MOVE_MULTIPLIER);
                }
                else if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                {
                    MySpectator.SpeedMode = Math.Max(MySpectator.SpeedMode / 1.5f, MyEditorConstants.MIN_EDITOR_CAMERA_MOVE_MULTIPLIER);
                }
            }

            if (CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic &&
                !m_wheelControlMenu.Visible && !IsSelectAmmoVisible())  // don't zoom if wheel or ammo menu is visible
            {
                float distance = MyThirdPersonSpectator.LookAt.Length();
                float currentDistance = (MyThirdPersonSpectator.Target - MyThirdPersonSpectator.DesiredPosition).Length();
                float newDistance = 0;

                if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                {
                    newDistance = currentDistance / 1.2f;
                }
                else if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                {
                    newDistance = distance * 1.2f;
                }

                if (newDistance > 0)
                {
                    // Limit distance from ship
                    newDistance = MathHelper.Clamp(newDistance, MyThirdPersonSpectator.GetMinDistance(), MyThirdPersonSpectator.MAX_DISTANCE);
                    MyThirdPersonSpectator.LookAt *= newDistance / distance;
                }

                //MyThirdPersonSpectator.LookAt = new Vector3(0, 130, 65);
            }

            #region Universal developer keys

            if (MyGuiInput.ENABLE_DEVELOPER_KEYS)
            {
                if (input.IsNewKeyPress(Keys.Multiply))
                {
                    //MyHudNotification.AddNotification(new MyHudNotification.MyNotification("Searching for most complex region..." , 3000));
                    //int[] res = MyEntities.getMostComplexCameraView(128);
                    //MyHudNotification.AddNotification(new MyHudNotification.MyNotification("X " + res[0] + " Y" + res[1] + " Z" + res[2], 3000));
                    //MyEntities.getMostComplexCameraView(128);
                }
                //if (MyFakes.ENABLE_MULTIPLAYER && input.IsAnyShiftKeyPressed() && input.IsNewKeyPress(Keys.PageUp))
                //{
                //    MyMultiplayer.HostGame(null);
                //    MyMultiplayer.Static.OnNotification = new Action<MyTextsWrapperEnum, object[]>(Static_OnNotification);
                //}

                if (MyMwcFinalBuildConstants.IS_DEVELOP && input.IsNewKeyPress(Keys.F5) && input.IsAnyCtrlKeyPressed())
                {
                    MySession.Static.SaveLastCheckpoint(false);
                }

                //  Show sector border
                if (MyMwcFinalBuildConstants.IS_DEVELOP && input.IsNewKeyPress(Keys.U))
                {
                    if (MyFakes.MWBUILDER)
                    {
                        if (input.IsAnyShiftKeyPressed())
                        {
                            MyEditorGrid.SwitchGridOrientation();
                        }
                        else
                        {
                            MyEditorGrid.IsGridActive = !MyEditorGrid.IsGridActive;
                        }
                    }
                    else
                    {
                        MyHudSectorBorder.SwitchSectorBorderVisibility();
                    }
                }

                if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE && (MyGuiScreenGamePlay.Static.IsGameActive() || MyGuiScreenGamePlay.Static.IsEditorActive())) // Trailer controls
                {
                    //if (input.IsKeyPress(Keys.NumPad5) && input.IsAnyShiftKeyPressed()) // SHIFT + Num5 - Set trailer start position
                    //{
                    //    MyHudNotification.AddNotification(new MyHudNotification.MyNotification("Trailer start set", 5));
                    //    trailerStartPosition = MySession.PlayerShip.WorldMatrix;
                    //}

                    if (input.IsNewKeyPress(Keys.NumPad5) && !input.IsAnyShiftKeyPressed()) // Num5 - Restart & record...
                    {
                        RestartTrailer(false, false);
                    }

                    if (input.IsNewKeyPress(Keys.NumPad7) && !input.IsAnyShiftKeyPressed()) // Num7 - Restart & teleport & record...
                    {
                        RestartTrailer(false, true);
                        //var translation = Matrix.CreateTranslation(MySession.PlayerShip.WorldMatrix.Translation + MyMwcUtils.Normalize(MySession.PlayerShip.WorldMatrix.Backward) * 10);
                        //MySession.PlayerShip.WorldMatrix *= translation;
                    }

                    if (input.IsNewKeyPress(Keys.NumPad9) && !input.IsAnyShiftKeyPressed()) // Num9 - Restart save & record...
                    {
                        RestartTrailer(true, true);
                    }
                }

                if (input.IsNewKeyPress(Keys.S) && input.IsAnyAltKeyPressed() && input.IsAnyCtrlKeyPressed())
                {
                    MyEditor.SaveSelectedVoxelMap(input.IsAnyShiftKeyPressed());
                }

                if (input.IsNewKeyPress(Keys.Delete) && input.IsAnyCtrlKeyPressed())
                {
                    if (MyMissions.ActiveMission != null)
                    {
                        MyObjective.SkipSubmission = true;
                    }
                }

                if (input.IsNewKeyPress(Keys.End) && input.IsAnyCtrlKeyPressed())
                {
                    if (MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives.Count > 0)
                    {
                        var entityId = MyMissions.ActiveMission.ActiveObjectives[0].Location.LocationEntityIdentifier.LocationEntityId;
                        MyEntity entity;
                        if (entityId.HasValue && MyEntities.TryGetEntityById(entityId.ToEntityId().Value, out entity))
                        {
                            MySession.PlayerShip.SetPosition(entity.GetPosition());
                        }
                    }
                    else
                    {
                        MyEntity entity;
                        //if (MyEntities.TryGetEntityByName(MyMissionBase.MyMissionLocation.MADELYN_HANGAR, out entity))
                        if (MyEntities.TryGetEntityByName("Madelyn", out entity))
                        {
                            MySession.PlayerShip.SetPosition(entity.GetPosition());
                        }
                    }
                }

                if (input.IsNewKeyPress(Keys.OemTilde) && input.IsAnyCtrlKeyPressed())
                {
                    var ship = MySession.Static.Player.Ship as MySmallShip;
                    ship.DoDamage(1000, 1000, 0, MyDamageType.Explosion, MyAmmoType.Unknown, null);
                    MySession.Static.Player.AddHealth(-MySession.Static.Player.Health, null);
                }

                if ((input.IsNewKeyPress(Keys.OemBackslash) || input.IsNewKeyPress(Keys.OemPipe)) && input.IsAnyShiftKeyPressed())
                {
                    MyLine line = new MyLine(MyCamera.Position, MyCamera.Position + MyCamera.ForwardVector * 25000, false);
                    var intersection = MyEntities.GetIntersectionWithLine(ref line, MySession.Static.Player.Ship, null, true);
                    if (intersection.HasValue)
                    {
                        var target = intersection.Value.IntersectionPointInWorldSpace;
                        target -= MyCamera.ForwardVector * MySession.Static.Player.Ship.LocalVolume.Radius * 20;
                        MySession.Static.Player.Ship.SetPosition(target);
                    }
                }

                if (input.IsNewKeyPress(Keys.NumLock))
                {
                    var player = MySession.Static.Player;

                    player.RestoreHealth();

                    var ship = MySession.Static.Player.Ship as MySmallShip;
                    if (ship != null)
                    {
                        ship.AddHealth(ship.MaxHealth);
                        ship.ArmorHealth = ship.MaxArmorHealth;

                        ship.Oxygen = ship.MaxOxygen;
                        ship.Fuel = ship.MaxFuel;
                        ship.AfterburnerStatus = 1;

                        foreach (Inventory.MyInventoryItem item in ship.Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
                        {
                            item.Amount = item.MaxAmount;
                        }
                    }
                }

                if (input.IsAnyControlPress() && input.IsAnyShiftPress() && input.IsNewKeyPress(Keys.F8))
                {
                    MyGuiPreviewRenderer renderer = MyGuiManager.GetPreviewRenderer();
                    renderer.CreatePreviewsToFiles(@"C:\Temp", MyHudConstants.PREFAB_PREVIEW_SIZE);
                }

                if (input.IsNewKeyPress(Keys.Space) && input.IsAnyCtrlKeyPressed())
                {
                    if (CameraAttachedTo == MyCameraAttachedToEnum.Spectator)
                    {
                        // we try to move ship to the spot, then we check if it colides if yes we move it back.
                        //
                        Vector3 oldPos = MySession.PlayerShip.GetPosition();
                        MySession.PlayerShip.SetPosition(MySpectator.Position);

                        //Move player ship to camera position
                        MyEntities.CollisionsElements.Clear();
                        MinerWars.AppCode.Game.Physics.MyPhysicsBody rBody = MySession.PlayerShip.Physics;
                        if (rBody.GetRBElementList().Count > 0)
                        {
                            // we expect only one rigid body (box)
                            MyEntities.GetCollisionListForElement(rBody.GetRBElementList().First());
                            BoundingSphere bsphere = MySession.PlayerShip.WorldVolume;

                            if (MyEntities.CollisionsElements.Count > 0 || MyVoxelMaps.GetOverlappingWithSphere(ref bsphere) != null)
                            {

                                MySession.PlayerShip.SetPosition(oldPos);
                                //maybe warning here or sound is enough? 
                                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                            }

                        }
                        else
                            Debug.Assert(false);// no physical box in player ship
                    }
                }

                if (input.IsNewKeyPress(Keys.Space) && input.IsAnyShiftKeyPressed())
                {
                    if (CameraAttachedTo == MyCameraAttachedToEnum.Spectator)
                    {
                        MySmallShip playerShip = MySession.PlayerShip;

                        //Move Spectator to player ship position                        
                        MySpectator.Position = playerShip.GetPosition() +
                            ThirdPersonCameraDelta * playerShip.GetWorldRotation().Up +
                            ThirdPersonCameraDelta * -playerShip.GetWorldRotation().Forward;
                        MySpectator.Target = playerShip.GetPosition();
                    }
                }

                //  Save check point - allowed only for story editors
                // DISABLED, now done automatically
                //if (IsGameActive() && input.IsKeyPress(Keys.RightShift) && input.IsNewKeyPress(Keys.F2) &&
                //    MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetCanAccessEditorForStory())
                //{
                //    MyGuiManager.AddScreen(new MyGuiScreenSaveCheckpoint());
                //}

                if (!MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY && input.IsNewKeyPress(Keys.F10) && input.IsAnyShiftKeyPressed())
                {
                    MyModels.LogUsageInformation();
                }
                if (!MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY && input.IsNewKeyPress(Keys.F10) && input.IsAnyAltKeyPressed())
                {
                    MyModels.CheckAllModels();
                    //MyModels.MergeLogFiles();
                }

                /*if (input.IsNewKeyPress(Keys.F10) && input.IsAnyCtrlKeyPressed() && !MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY)
                {
                    MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY = true;
                    if (MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM.Length == 0)
                    {
                        int i = 0;
                        MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM = new Missions.MyMissionID[Missions.MyMissions.Missions.Count];
                        foreach (var mission in MyMissions.Missions.Values.OfType<MyMission>().OrderBy(x => x.Name.ToString()))
                        {
                            if (i >= 12)
                                MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM[i] = mission.ID;
                            i++;
                        }
                    }
                    GUI.MyGuiScreenMainMenu.StartMission(MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM[0]);
                }*/

                // Voxel import
                if (MyFakes.VOXEL_IMPORT && input.IsNewKeyPress(Keys.PageUp))
                {
                    if (!input.IsAnyShiftKeyPressed())
                    {
                        MyVoxelMap voxelMap = new MyVoxelMap();
                        //{
                        //    // TODO: merge
                        //    //VoxelMaterial = MyMwcVoxelMaterialsEnum.Stone_01,
                        //    Size = MyFakes.VOXEL_IMPORT_SIZE
                        //};
                        voxelMap.Init(Vector3.Zero, MyFakes.VOXEL_IMPORT_SIZE, MyMwcVoxelMaterialsEnum.Stone_01);

                        MyModel model = new MyModel(MyFakes.VOXEL_IMPORT_MODEL, MyMeshDrawTechnique.MESH, MyModelsEnum.ExplosionDebrisVoxel);
                        model.LoadData();

                        MyVoxelImport.Run(voxelMap, model, MyVoxelImportOptions.KeepAspectRatio);
                        voxelMap.SaveVoxelContents(Path.Combine(MyMinerGame.Static.RootDirectory, "VoxelMaps", MyVoxelFiles.ExportFile + ".vox"));

                        // Reload sector
                        MyGuiScreenGamePlay.Static.Restart();
                    }
                    else
                    {
                        int[] sizes = new int[] { 64, 128, 256, 512 };

                        List<MyMwcVector3Int> voxelSizes = new List<MyMwcVector3Int>();

                        for (int x = 0; x < sizes.Length; x++)
                        {
                            int lo = Math.Max(x - 1, 0);
                            int hi = Math.Min(x + 2, sizes.Length);

                            for (int y = lo; y < hi; y++)
                            {
                                for (int z = lo; z < hi; z++)
                                {
                                    voxelSizes.Add(new MyMwcVector3Int(sizes[x], sizes[y], sizes[z]));
                                }
                            }
                        }

                        foreach (var size in voxelSizes)
                        {
                            MyVoxelMap voxelMap = new MyVoxelMap();
                            voxelMap.Init(Vector3.Zero, size, MyMwcVoxelMaterialsEnum.Stone_01);

                            MyVoxelImport.Fill(voxelMap);
                            voxelMap.SaveVoxelContents(Path.Combine(MyMinerGame.Static.RootDirectory, "VoxelMaps", String.Format("Cube_{0}x{1}x{2}.vox", size.X, size.Y, size.Z)));
                            MyEntities.Remove(voxelMap);
                            voxelMap.MarkForClose();
                            GC.Collect();
                        }

                        // Reload sector
                        MyGuiScreenGamePlay.Static.Restart();
                    }
                }


                if (input.IsNewKeyPress(Keys.L) && input.IsAnyCtrlKeyPressed() && input.IsAnyShiftKeyPressed())
                {
                    if (MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives.Count > 0 && MyMissions.ActiveMission.ActiveObjectives[0].Location != null && MyMissions.ActiveMission.ActiveObjectives[0].Location.Entity != null)
                    {
                        MySpectator.Position = MyMissions.ActiveMission.ActiveObjectives[0].Location.Entity.GetPosition();

                    }
                }

                //  Start sun wind
                if (input.IsNewKeyPress(Keys.F3))
                {
                    //Multiplayer_EnterGame(new MyEventEnterGame() { PlayerInfo = new MyPlayerInfo() { DisplayName = "Test" } });
                    //MyDialogues.Play(Audio.Dialogues.MyDialogueEnum.TEST);
                    MyGlobalEvents.StartGlobalEvent(MyGlobalEventEnum.SunWind);
                    //MyGuiManager.AddScreen(new MyGuiScreenMission(MyMissions.ActiveMission));
                    //MySession.PlayerShip.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                    //MyInventory.GenerateDebugInventoryItemsInfo();
                    //MyAmmoConstants.GenerateDebugAmmoTypeInfo();
                    if (MyFakes.ENABLE_DEBUG_INFLUENCE_SPHERES_SOUNDS)
                    {
                        if (input.IsKeyPress(Keys.LeftAlt))
                        {
                            MyInfluenceSphere.UpdateMaxVolume(false);
                        }
                        else if (input.IsKeyPress(Keys.RightAlt))
                        {
                            MyInfluenceSphere.UpdateMaxVolume(true);
                        }
                        else
                        {
                            MyInfluenceSphere.SwitchToNextSound(!input.IsAnyShiftKeyPressed());
                        }
                    }
                }

                //Set camera to player
                if (input.IsNewKeyPress(Keys.F6))
                {
                    TryChangeCameraAttachedTo(MyCameraAttachedToEnum.PlayerMinerShip);
                }

                //Set camera to following third person
                if (input.IsNewKeyPress(Keys.F7))
                {
                    TryChangeCameraAttachedTo(MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing);
                }

                //Set camera to spectator
                if (input.IsNewKeyPress(Keys.F8))//&& TryChangeCameraAttachedTo(MyCameraAttachedToEnum.Spectator))
                {
                    CameraAttachedTo = MyCameraAttachedToEnum.Spectator;
                    if (input.IsAnyCtrlKeyPressed())
                    {
                        MySpectator.Position = MySession.PlayerShip.GetPosition() + ThirdPersonCameraDelta;
                        MySpectator.Target = MySession.PlayerShip.GetPosition();
                    }
                }

                //Set camera to static third person
                if (input.IsNewKeyPress(Keys.F9) && TryChangeCameraAttachedTo(MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic))
                {
                    if (input.IsAnyCtrlKeyPressed())
                    {
                        MySpectator.Position = MySession.PlayerShip.GetPosition() + ThirdPersonCameraDelta;
                        MySpectator.Target = MySession.PlayerShip.GetPosition();
                    }
                }

                if (input.IsNewKeyPress(Keys.F10) /*&& input.IsAnyCtrlKeyPressed()*/ && TryChangeCameraAttachedTo(MyCameraAttachedToEnum.BotMinerShip))
                {
                    //Find nearest bot and attach him
                    BoundingFrustum boundingFrustum = MyCamera.GetBoundingFrustum();
                    List<MyEntity> entities = new List<MyEntity>();
                    MyEntities.GetAllIntersectionWithBoundingFrustum_UNOPTIMIZED(ref boundingFrustum, entities);
                    MySmallShipBot closestBot = null;
                    foreach (MyEntity entity in entities)
                    {
                        if (entity is MySmallShipBot)
                        {
                            if (closestBot == null)
                            {
                                closestBot = entity as MySmallShipBot;
                                continue;
                            }

                            if (Vector3.Distance(MyCamera.Position, entity.GetPosition()) < Vector3.Distance(MyCamera.Position, closestBot.GetPosition()))
                            {
                                closestBot = entity as MySmallShipBot;
                            }
                        }
                    }
                    MyGuiScreenGamePlay.Static.ShipForSimpleTesting = closestBot;
                }
            }

            #endregion

            #region Game/editor keys

            if ((IsGameActive() || IsEditorActive()) && !MyEditorBase.IsEditorActive)
            {
                //  Turn reflector on/off
                if (input.IsNewGameControlPressed(MyGameControlEnums.HEADLIGHTS))
                {
                    if (IsControlledPlayerShip)
                    {
                        MySession.PlayerShip.Config.ReflectorLight.ChangeValueUp();
                    }

                    if (IsControlledDrone)
                    {
                        ControlledDrone.Config.ReflectorLight.ChangeValueUp();
                    }
                }

                //  Move and rotate player or camera
                MoveAndRotatePlayerOrCamera(input);
            }

            #endregion

            #region Game keys that work only when player is alive

            if (IsGameActive() && IsFlyThroughActive() == false && MySession.Static != null && MySession.Static.Player != null && !MySession.Static.Player.IsDead())
            {
                //  Open inventory
                if (input.IsNewGameControlPressed(MyGameControlEnums.INVENTORY))
                {
                    if (IsControlledPlayerShip && !MySession.PlayerShip.IsDead() && !MySession.PlayerShip.IsPilotDead())
                    {
                        MyGuiScreenInventoryType inventoryType;
                        if (IsTypeEditorGodMode(m_previousType) &&
                            MySession.PlayerShip.TradeDetector.GetNearestEntity() == null ||
                            Static.IsCheatEnabled(MyGameplayCheatsEnum.UNLIMITED_TRADING))
                        {
                            inventoryType = MyGuiScreenInventoryType.GodEditor;
                        }
                        else
                        {
                            inventoryType = MyGuiScreenInventoryType.Game;
                        }
                        if (!MyMultiplayerGameplay.IsRunning)
                        {
                            var inventoryScreen = MyGuiScreenInventoryManagerForGame.OpenInventory(inventoryType);
                            MyGuiManager.AddScreen(inventoryScreen);
                        }
                        else
                        {
                            OpenInventoryMultiplayer(inventoryType);
                        }
                    }
                }

                // Target locking
                if (input.IsNewGameControlPressed(MyGameControlEnums.PREV_TARGET))
                    MyEnemyTargeting.SwitchNextTarget(false);
                if (input.IsNewGameControlPressed(MyGameControlEnums.NEXT_TARGET))
                    MyEnemyTargeting.SwitchNextTarget(true);

                // Bot follow / hold position
                if (input.IsNewGameControlPressed(MyGameControlEnums.CHANGE_DRONE_MODE))
                {
                    if (IsControlledDrone)
                        ControlledDrone.HoldPosition = !ControlledDrone.HoldPosition;
                }

                if (input.IsNewGameControlPressed(MyGameControlEnums.QUICK_ZOOM)
                    && !MyCamera.Zoom.IsZooming()
                    && MyCamera.Zoom.GetZoomLevel() < 1.0f && !m_wheelControlMenu.Visible)
                {
                    m_quickZoomOut = true;
                    MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.ZoomOut);
                }

                if (!m_quickZoomOut)
                {
                    if (input.IsGameControlPressed(MyGameControlEnums.QUICK_ZOOM))
                    {
                        MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.ZoomIn);
                        m_quickZoomWasUsed = true;
                    }
                    else if (m_quickZoomWasUsed)
                    {
                        MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.ZoomOut);
                    }
                }

                if (input.IsNewGameControlPressed(MyGameControlEnums.ZOOM_IN) ||
                    input.IsNewGameControlPressed(MyGameControlEnums.ZOOM_OUT))
                {
                    m_quickZoomOut = false;
                }

                if (!input.IsGameControlPressed(MyGameControlEnums.ZOOM_IN))
                {
                    if (input.IsGameControlPressed(MyGameControlEnums.ZOOM_OUT))
                    {
                        MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.ZoomOut);
                        m_quickZoomWasUsed = false;
                    }
                    else if (!m_quickZoomWasUsed && !m_quickZoomOut)
                    {
                        MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.NoZoom);
                    }
                }
                else if (input.IsGameControlPressed(MyGameControlEnums.ZOOM_IN))
                {
                    MyCamera.Zoom.SetZoom(MyCameraZoomOperationType.ZoomIn);
                    m_quickZoomWasUsed = false;
                }

                // Drone manipulation
                if (input.IsGameControlPressed(MyGameControlEnums.DRONE_DEPLOY))
                    DroneDeployKeyPressed();
                if (input.IsGameControlPressed(MyGameControlEnums.DRONE_CONTROL))
                    DroneControlKeyPressed();

                // Roll callbacks (movement is handled in MoveAndRotatePlayerOrCamera)
                if (input.IsNewGameControlPressed(MyGameControlEnums.ROLL_LEFT))
                {
                    OnRollLeftPressed();
                    MyScriptWrapper.RollLeftPressed();
                }
                if (input.IsNewGameControlPressed(MyGameControlEnums.ROLL_RIGHT))
                {
                    OnRollRightPressed();
                    MyScriptWrapper.RollRightPressed();
                }

                // Turn deceleration on/off
                if (input.IsNewGameControlPressed(MyGameControlEnums.MOVEMENT_SLOWDOWN))
                {
                    if (IsControlledPlayerShip && MySession.PlayerShip.Config.Engine.On)
                        MySession.PlayerShip.Config.MovementSlowdown.ChangeValueUp();
                }

                // Change headlight distance
                if (input.IsNewGameControlPressed(MyGameControlEnums.HEADLIGTHS_DISTANCE))
                {
                    if (IsControlledPlayerShip)
                        MySession.PlayerShip.Config.ReflectorLongRange.ChangeValueUp();
                }

                // Turn auto-leveling on/off
                if (input.IsNewGameControlPressed(MyGameControlEnums.AUTO_LEVEL))
                {
                    if (IsControlledPlayerShip && MySession.PlayerShip.Config.Engine.On)
                        MySession.PlayerShip.Config.AutoLeveling.ChangeValueUp();
                }

                // Turn secondary camera on/off
                if (MyFakes.ENABLE_BACK_CAMERA && !DisableBackCamera)
                {
                    if (input.IsNewGameControlPressed(MyGameControlEnums.REAR_CAM))
                    {
                        if (IsControlledPlayerShip)
                            ControlledShip.Config.BackCamera.ChangeValueUp();
                        if (IsControlledDrone)
                            ControlledDrone.Config.BackCamera.ChangeValueUp();
                    }
                }
                else
                {
                    MySession.PlayerShip.Config.BackCamera.SetOff();

                    if (IsControlledDrone)
                        ControlledDrone.Config.BackCamera.SetOff();
                }

                // Switch view - cockpit on/off, third person
                if (input.IsNewGameControlPressed(MyGameControlEnums.VIEW_MODE))
                {
                    if (IsControlledPlayerShip)
                        MySession.PlayerShip.Config.ViewMode.ChangeValueUp();
                }

                // Secondary camera and remote camera controls
                if (input.IsNewGameControlPressed(MyGameControlEnums.PREVIOUS_CAMERA))
                {
                    if (IsControlledPlayerShip)
                        MySession.PlayerShip.SelectPreviousSecondaryCamera();
                }

                if (input.IsNewGameControlPressed(MyGameControlEnums.NEXT_CAMERA))
                {
                    if (IsControlledPlayerShip)
                        MySession.PlayerShip.SelectNextSecondaryCamera();
                }

                HandleRemoteCameraInput(input);

                // Show GPS
                if (input.IsNewGameControlPressed(MyGameControlEnums.GPS) || (MyFakes.GPS_ALWAYS_ON && Math.Abs(MyHud.LastGPS - MyMinerGame.TotalGamePlayTimeInMilliseconds) > 500))
                {
                    MyHud.ShowGPSPathToNextObjective(true);
                    if (!m_showGPSNotification.IsDisappeared())
                        m_showGPSNotification.Disappear();
                }

                // Harvesting and drilling
                if (input.IsNewGameControlPressed(MyGameControlEnums.HARVEST))
                {
                    if (IsControlledPlayerShip && MySession.PlayerShip.IsEngineWorking() && MySession.PlayerShip.Fuel > 0)
                        MySession.PlayerShip.Weapons.FireHarvester();
                }

                if (input.IsGameControlPressed(MyGameControlEnums.DRILL))
                {
                    if (IsControlledPlayerShip && MySession.PlayerShip.IsEngineWorking() && MySession.PlayerShip.Fuel > 0)
                        MySession.PlayerShip.Weapons.FireDrill();
                }
            }

            #endregion

            #region Game keys that work only when player is alive and action menu is closed (e.g. keys that open a new menu)

            if (IsGameActive() && !m_wheelControlMenu.Visible && IsFlyThroughActive() == false && MySession.Static != null && MySession.Static.Player != null && !MySession.Static.Player.IsDead())
            {
                // Travel
                if (input.IsNewGameControlPressed(MyGameControlEnums.TRAVEL))
                {
                    bool canTravel = true;

                    if (m_type == MyGuiScreenGamePlayType.GAME_SANDBOX)
                    {
                        if (MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_STORY)
                            canTravel = false;
                    }
                    else if (m_type == MyGuiScreenGamePlayType.GAME_STORY || m_type == MyGuiScreenGamePlayType.GAME_MMO)
                    {
                        if (MySession.PlayerShip.GetNearMotherShipContainer() == null)
                            canTravel = false;
                        else
                            canTravel = CanTravel;
                    }

                    bool canSave = MyClientServer.LoggedPlayer.GetCanSave();
                    canTravel |= MyFakes.ENABLE_SOLAR_MAP;
                    canTravel &= canSave;

                    if (canTravel)
                    {
                        MyMwcSectorIdentifier sector = m_sectorIdentifier;
                        this.HideScreen();
                        MyGuiManager.AddScreen(new MyGuiScreenSolarSystemMap(this, sector));
                    }
                    else if (m_type == MyGuiScreenGamePlayType.GAME_STORY || m_type == MyGuiScreenGamePlayType.GAME_MMO)
                    {
                        if (!canSave)
                            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.DemoUserCannotTravel, MyTextsWrapperEnum.SolarSystemMap, MyTextsWrapperEnum.Ok, null));
                        else if (CanTravel)
                            MyHudNotification.AddNotification(m_notificationYouHaveToBeNearMothership);
                        else
                            MyHudNotification.AddNotification(m_notificationUnableToLeaveSectorMission);
                    }
                }

                // Use / Hack / Take
                if (input.IsNewGameControlPressed(MyGameControlEnums.USE))
                {
                    bool isHandled = false;
                    // first handled are notifications
                    if (MyHudNotification.HasNotification(MyHudNotification.GetCurrentScreen()))
                        isHandled = MyHudNotification.HandleInput(input);

                    // second handled is take all event
                    if (!isHandled)
                        isHandled = TakeAllItems();

                    // third handled is hacking etc
                    if (!isHandled)
                    {
                        MySmallShip hacker = ControlledDrone ?? MySession.PlayerShip;
                        IMyUseableEntity detectedEntityToUse = hacker.UseableEntityDetector.GetNearestEntity() as IMyUseableEntity;

                        if (ControlledLargeWeapon != null && TryChangeCameraAttachedTo(MyCameraAttachedToEnum.PlayerMinerShip))
                            ReleaseControlOfLargeWeapon();

                        if (ControlledCamera != null)
                            ReleaseControlOfCamera();
                        else if (detectedEntityToUse != null)
                        {
                            MySmallShipInteractionActionEnum detectedAction = (MySmallShipInteractionActionEnum)hacker.UseableEntityDetector.GetNearestEntityCriterias();
                            if (detectedAction == MySmallShipInteractionActionEnum.Use)
                                detectedEntityToUse.Use(hacker);
                            else if (detectedAction == MySmallShipInteractionActionEnum.Hack)
                                hacker.HackingTool.Hack(detectedEntityToUse);
                        }
                        else
                            MyScriptWrapper.UseKeyPressed();
                    }
                }
                       /*  Journal removed
                // Journal
                if (input.IsNewGameControlPressed(MyGameControlEnums.MISSION_DIALOG) && (GetGameType() == MyGuiScreenGamePlayType.GAME_STORY || GetGameType() == MyGuiScreenGamePlayType.GAME_MMO))
                {
                    if (!MyMissions.RequestMissionDialog())
                        MyGuiManager.AddScreen(new MyGuiScreenJournal(MySession.Static.EventLog));
                }        */

                // Chat, multiplayer statistics
                // TODO: prevent dev keys universally, not based on the default mapping
#if !RENDER_PROFILING
                if (MyGuiManager.IsScreenTopMostNonDebug(this))
                {
                    if (input.IsNewGameControlPressed(MyGameControlEnums.CHAT)/* && MyMultiplayerGameplay.IsRunning && (MyMultiplayerGameplay.IsSandBox() || (MyMultiplayerGameplay.IsStory() && MyMultiplayerGameplay.OtherPlayersConnected))*/) //only in deathmatch (all the time) or coop (at least one other player)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenGameChat(input.IsAnyShiftKeyPressed()));
                    }
                    if (input.IsNewGameControlPressed(MyGameControlEnums.SCORE) && MyMultiplayerGameplay.IsRunning && (MyMultiplayerGameplay.IsSandBox() || (MyMultiplayerGameplay.IsStory() && MyMultiplayerGameplay.OtherPlayersConnected))) //only in deathmatch (all the time) or coop (at least one other player)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenGameScore());
                    }
                }
#endif
                // Shooting
                // we can be dead in this moment

                if (MyFakes.CUBE_EDITOR && MySession.PlayerShip != null && input.IsNewGameControlPressed(MyGameControlEnums.QUICK_ZOOM))
                    MySession.PlayerShip.CubeBuilder.BuilderActive = !MySession.PlayerShip.CubeBuilder.BuilderActive;

                if (MySession.PlayerShip != null && MySession.PlayerShip.CubeBuilder.BuilderActive)
                {
                    if (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_PRIMARY))
                        MySession.PlayerShip.CubeBuilder.Add();
                    if (input.IsNewGameControlPressed(MyGameControlEnums.FIRE_SECONDARY))
                        MySession.PlayerShip.CubeBuilder.Remove();
                }
                else if (m_handleInputMouseKeysReleased &&
                    (!IsSelectAmmoVisible()) &&
                    (!m_wheelControlMenu.Visible) &&
                    MySession.PlayerShip != null &&
                    !MySession.PlayerShip.IsDead() &&
                    IsFireKeyEnabled())
                {
                    switch (CameraAttachedTo)
                    {
                        case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic:
                        case MyCameraAttachedToEnum.Spectator:
                        case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic:
                        case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing:
                        case MyCameraAttachedToEnum.PlayerMinerShip:
                            if (!input.IsAnyCtrlKeyPressed() || CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip)
                            {
                                MySmallShip playerShip = MySession.PlayerShip;

                                if (playerShip.IsEngineWorking())
                                {
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.Primary);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY))
                                    {
                                        // TODO: prevent dev keys universally, not based on the default mapping
#if !GPU_PROFILING
                                        playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.Secondary);
#endif
                                    }
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.Third);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.Fourth);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.Fifth);
                                    if (input.IsNewGameControlPressed(MyGameControlEnums.WEAPON_SPECIAL)) playerShip.InvokeAmmoSpecialFunction();
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_HOLOGRAM_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.HologramFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_HOLOGRAM_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.HologramBack);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_BASIC_MINE_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.BasicMineFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_BASIC_MINE_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.BasicMineBack);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SMART_MINE_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.SmartMineFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SMART_MINE_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.SmartMineBack);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FLASH_BOMB_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.FlashBombFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FLASH_BOMB_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.FlashBombBack);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_DECOY_FLARE_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.DecoyFlareFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_DECOY_FLARE_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.DecoyFlareBack);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SMOKE_BOMB_FRONT)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.SmokeBombFront);
                                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SMOKE_BOMB_BACK)) playerShip.Weapons.Fire(MyMwcObjectBuilder_FireKeyEnum.SmokeBombBack);
                                }
                            }
                            break;
                        case MyCameraAttachedToEnum.Drone:
                            if (input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH))
                            {
                                ControlledDrone.Fire();
                                DisableFireKey(new TimeSpan(0, 0, 1));
                            }
                            break;
                        case MyCameraAttachedToEnum.LargeWeapon:
                            if (input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH) ||
                                input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH))
                            {
                                ControlledLargeWeapon.Fire();
                            }
                            else
                            {
                                ControlledLargeWeapon.StopFire();
                            }
                            break;
                    }
                }
            }

            #endregion

            #region Game keys that work all the time (even when player is dead)

            if (IsGameActive())
            {
                // Open cheat screen
                if (input.IsNewKeyPress(Keys.F1) && input.IsAnyCtrlKeyPressed())
                {
                    MyGuiManager.AddScreen(new MyGuiScreenCheats());
                }

                // Open help screen
                if (input.IsNewKeyPress(Keys.F1) && !input.IsAnyCtrlKeyPressed())
                {
                    MyGuiManager.AddScreen(new MyGuiScreenHelp());
                }

                if (IsControlledPlayerShip)
                {
                    m_selectAmmoMenu.HandleInput(input, true, true, receivedFocusInThisUpdate);
                    m_wheelControlMenu.HandleInput(input, true, true, receivedFocusInThisUpdate);
                }

                // DrawMouseCursor = m_wheelControlMenu.Visible;

                /*  //duplicated in if IsGameReady
           if (IsGameActive()) // enable cursor drawing if we have ammo selection opened
           {
               DrawMouseCursor = m_selectAmmoMenu.IsEnabled && m_selectAmmoMenu.Visible;
           } */

                MyDebugConsole.GetInstance().Update(MyDebugSystem.Game, input);
            }

            #endregion

            #region Editor keys

            if (IsEditorActive())
            {
                // Open ship customization screen
                if (input.IsNewGameControlPressed(MyGameControlEnums.INVENTORY) && !input.IsAnyCtrlKeyPressed())
                {
                    if (MyFakes.SHOW_NEW_INVENTORY_SCREEN)
                    {
                        MyGuiScreenInventoryType inventoryType = !IsIngameEditorActive() || Static.IsCheatEnabled(MyGameplayCheatsEnum.UNLIMITED_TRADING)
                                                                        ? MyGuiScreenInventoryType.GodEditor
                                                                        : MyGuiScreenInventoryType.InGameEditor;
                        MyGuiManager.AddScreen(MyGuiScreenInventoryManagerForGame.OpenInventory(inventoryType));
                    }
                }

                MyEditor.Static.HandleInput(input, !AnyControlContainsMouse());
                MyDebugConsole.GetInstance().Update(MyDebugSystem.Editor, input);
            }

            #endregion
        }

        void OnRollLeftPressed()
        {
            var handler = RollLeftPressed;
            if (handler != null)
            {
                handler();
            }
        }

        void OnRollRightPressed()
        {
            var handler = RollRightPressed;
            if (handler != null)
            {
                handler();
            }
        }

        private void OpenInventoryMultiplayer(MyGuiScreenInventoryType inventoryType)
        {
            var tradeEntity = MySession.PlayerShip.TradeDetector.GetNearestEntity();
            if (tradeEntity == null)
            {
                var inventoryScreen = MyGuiScreenInventoryManagerForGame.OpenInventory(inventoryType);
                MyGuiManager.AddScreen(inventoryScreen);
            }
            else
            {
                var tradeEntityCriterias = (MySmallShipInteractionActionEnum)MySession.PlayerShip.TradeDetector.GetNearestEntityCriterias();

                Debug.Assert(tradeEntity.EntityId != null, "tradeEntity.EntityId != null");
                var entityID = tradeEntity.EntityId.Value;

                MyMultiplayerGameplay.Static.LockReponse = (e, success) =>
                    {
                        MyMultiplayerGameplay.Static.LockReponse = null;
                        if (tradeEntity != e)
                        {
                            Debug.Fail("Something went wrong, locked different entity");
                            MyMultiplayerGameplay.Static.Lock(e, false);
                            return;
                        }

                        if (success)
                        {
                            var inventoryScreen = MyGuiScreenInventoryManagerForGame.OpenInventory(inventoryType, tradeEntity, tradeEntityCriterias);
                            //inventoryScreen.ScreenTimeout = TimeSpan.FromSeconds(25);
                            inventoryScreen.Closed += (s) => MyMultiplayerGameplay.Static.Lock(entityID, false);
                            MyGuiManager.AddScreen(inventoryScreen);
                        }
                    };
                MyMultiplayerGameplay.Static.Lock(tradeEntity, true);
            }
        }

        public void ReturnControlToPlayerShip()
        {
            ReleaseControlOfDrone();
            ReleaseControlOfCamera();
            ReleaseControlOfLargeWeapon();
        }

        bool IsFireKeyEnabled()
        {
            return m_fireDisableTimeout <= 0;
        }

        /// <summary>
        /// Disables the fire keys for the specified duration.
        /// </summary>
        void DisableFireKey(TimeSpan duration)
        {
            m_fireDisableTimeout = (float)duration.TotalMilliseconds;
        }

        private void DroneDeployKeyPressed()
        {
            var deployed = MySession.PlayerShip.TryDeployDrone();
            if (deployed && IsControlledPlayerShip)
            {
                MySession.PlayerShip.SelectLastDroneCamera();
                MySession.PlayerShip.Config.BackCamera.SetOn();
            }
        }

        private void DroneControlKeyPressed()
        {
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeSwitchedDroneControl) > DRONE_CONTROL_INTERVAL)
            {
                m_lastTimeSwitchedDroneControl = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                if (IsControlledDrone)
                {
                    ReleaseControlOfDrone();
                }
                else
                {
                    bool tookControlOfDrone = TryTakeControlOfDrone();
                }
            }
        }

        private bool TryTakeControlOfDrone()
        {
            var selectedDrone = MySession.PlayerShip.GetSelectedDrone();

            if (selectedDrone != null)
            {
                TakeControlOfDrone(selectedDrone);
                return true;
            }

            return false;
        }

        private void TakeControlOfDrone(MyDrone drone)
        {
            CameraAttachedTo = MyCameraAttachedToEnum.Drone;
            drone.OnClosing += OnControlledDroneClose;
            ControlledDrone = drone;
            ControlledDrone.ActiveAI = false;
            ControlledDrone.Config.AutoLeveling.SetOff();
            MySession.PlayerShip.StopSounds();
            MySession.PlayerShip.SelectShipCamera();
            MyGuiManager.GetRemoteViewDroneTextures();
            MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireDroneOn);
            if (m_idleVehicleCue.HasValue && m_idleVehicleCue.Value.IsPlaying)
            {
                m_idleVehicleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
            m_idleVehicleCue = MyAudio.AddCue2D(MySoundCuesEnum.VehLoopDrone);
        }

        void OnControlledDroneClose(MyEntity drone)
        {
            if (drone == ControlledEntity)
            {
                drone.OnClosing -= OnControlledDroneClose;
                ReleaseControlOfDrone();
            }
        }

        public void TakeControlOfCamera(MyPrefabCamera camera)
        {
            camera.Visible = false;
            CameraAttachedTo = MyCameraAttachedToEnum.Camera;
            ControlledCamera = camera;
            MyGuiManager.GetRemoteViewCameraTextures();
            MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireCameraOn);
            MySession.PlayerShip.StopSounds();
            if (m_idleVehicleCue.HasValue && m_idleVehicleCue.Value.IsPlaying)
            {
                m_idleVehicleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
            m_idleVehicleCue = MyAudio.AddCue2D(MySoundCuesEnum.VehLoopCamera);

            AddSwitchingNotification();

            AddCameraDetachingNotification();
        }

        void AddCameraDetachingNotification()
        {
            if (m_detachNotification != null &&
                !m_detachNotification.IsDisappeared())
            {
                return;
            }

            object[] args = { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE), "" };

            m_detachNotification = new MyHudNotification.MyNotification(
                MyTextsWrapperEnum.NotificationExitControlled,
                MyHudNotification.GetCurrentScreen(),
                1f,
                MyHudConstants.NEUTRAL_FONT,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                MyHudNotification.DONT_DISAPEAR,
                null,
                false,
                args);

            if (!DetachingForbidden)
            {
                MyHudNotification.AddNotification(m_detachNotification);
            }
        }

        public void SwitchControlOfCamera(MyPrefabCamera prefabCamera)
        {
            if (ControlledEntity != null)
            {
                ControlledEntity.Visible = true;
            }

            //if (m_detachNotification != null)
            //{
            //    m_detachNotification.Disappear();
            //}

            TakeControlOfCamera(prefabCamera);
        }

        public void SwitchControlOfLargeWeapon(MyPrefabLargeWeapon prefabLargeWeapon)
        {
            if (ControlledEntity != null)
            {
                ControlledEntity.Visible = true;
                if (ControlledEntity == ControlledLargeWeapon)
                {
                    ControlledLargeWeapon.StopAimingSound();
                }
            }

            //if (m_detachNotification != null)
            //{
            //    m_detachNotification.Disappear();
            //}

            TakeControlOfLargeWeapon(prefabLargeWeapon);
        }

        public void ReleaseControlOfCamera()
        {
            if (ControlledCamera != null && CameraAttachedTo != MyCameraAttachedToEnum.PlayerMinerShip)
            {
                if (m_switchControlNotification != null)
                {
                    m_switchControlNotification.Disappear();
                    m_switchControlNotification = null;
                }

                m_detachNotification.Disappear();
                m_detachNotification = null;
                ControlledCamera.Visible = true;

                OnControlReleased(ControlledCamera);

                CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
                MySecondaryCamera.Instance.SetEntityCamera(ControlledCamera);
                ControlledShip = MySession.PlayerShip;
                MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireCameraOff);
                MyGuiManager.RemoveRemoteViewCameraTextures();
                if (m_idleVehicleCue.HasValue && m_idleVehicleCue.Value.IsPlaying)
                {
                    m_idleVehicleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
            }
        }

        public void ReleaseControlOfDrone()
        {
            if (IsControlledDrone && CameraAttachedTo != MyCameraAttachedToEnum.PlayerMinerShip)
            {
                CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
                MySecondaryCamera.Instance.SetEntityCamera(ControlledDrone);
                ControlledDrone.ActiveAI = !ControlledDrone.HoldPosition;
                ControlledDrone.Config.AutoLeveling.SetOn();

                OnControlReleased(ControlledDrone);

                ControlledShip = MySession.PlayerShip;
                MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Drone;
                MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireDroneOff);
                MyGuiManager.RemoveRemoteViewDroneTextures();
                if (m_idleVehicleCue.HasValue && m_idleVehicleCue.Value.IsPlaying)
                {
                    m_idleVehicleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
            }
        }

        public void TakeControlOfLargeWeapon(MyPrefabLargeWeapon largeWeapon)
        {
            CameraAttachedTo = MyCameraAttachedToEnum.LargeWeapon;
            ControlledLargeWeapon = largeWeapon;
            MyGuiManager.GetRemoteViewWeaponTextures();
            MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireWeaponOn);
            MySession.PlayerShip.StopSounds();
            largeWeapon.StopAimingSound();

            AddSwitchingNotification();

            AddLargeWeaponDetachingNotification();
        }

        void AddSwitchingNotification()
        {
            if (ActiveSecurityHubScreen == null || ActiveSecurityHubScreen.GetNumberOfControllableEntities() < 2)
            {
                return;
            }

            if (m_switchControlNotification != null)
            {
                if (m_switchControlNotification.IsDisappeared())
                {
                    m_switchControlNotification.Appear();
                }
            }
            else
            {
                object[] args = {
                                    MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_LEFT),
                                    MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_RIGHT)
                                };

                m_switchControlNotification = new MyHudNotification.MyNotification(
                    (ControlledEntity == null || ControlledEntity is MyPrefabCamera) ? MyTextsWrapperEnum.SwitchInHUBCameras : MyTextsWrapperEnum.SwitchInHUBTurrets,
                    MyHudNotification.GetCurrentScreen(),
                    1f,
                    MyHudConstants.FRIEND_FONT,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                    MyHudNotification.DONT_DISAPEAR,
                    null,
                    false,
                    args);

                MyHudNotification.AddNotification(m_switchControlNotification);
            }
        }

        void AddLargeWeaponDetachingNotification()
        {
            if (m_detachNotification != null &&
                !m_detachNotification.IsDisappeared())
            {
                return;
            }

            object[] args = { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE), "" };

            m_detachNotification = new MyHudNotification.MyNotification(
                MyTextsWrapperEnum.NotificationExitControlled,
                MyHudNotification.GetCurrentScreen(),
                1f,
                MyHudConstants.NEUTRAL_FONT,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                MyHudNotification.DONT_DISAPEAR,
                null,
                false,
                args);

            if (!DetachingForbidden)
            {
                MyHudNotification.AddNotification(m_detachNotification);
            }
        }

        /// <summary>
        /// Called automaticaly from CameraAttachedTo setter
        /// </summary>
        public void ReleaseControlOfLargeWeapon()
        {
            if (ControlledLargeWeapon != null)
            {
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.Lock(ControlledLargeWeapon, false);
                }

                if (m_switchControlNotification != null)
                {
                    m_switchControlNotification.Disappear();
                    m_switchControlNotification = null;
                }

                if (m_detachNotification != null)
                {
                    m_detachNotification.Disappear();
                    m_detachNotification = null;
                }

                OnControlReleased(ControlledLargeWeapon);

                CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
                ControlledShip = MySession.PlayerShip;
                MyAudio.AddCue2D(MySoundCuesEnum.SfxAcquireWeaponOff);
                MyGuiManager.RemoveRemoteViewWeaponTextures();
            }
        }

        void OnControlReleased(MyEntity releasedEntity)
        {
            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.Lock(releasedEntity, false);
            }

            var handler = ReleasedControlOfEntity;
            if (handler != null)
            {
                handler(releasedEntity);
            }
        }

        public void DisplaySecurityHubScreen(MyGuiScreenSecurityControlHUB screenSecurityControlHUB)
        {
            ActiveSecurityHubScreen = screenSecurityControlHUB;
            MyGuiManager.AddScreen(screenSecurityControlHUB);
        }

        private static void HandleRemoteCameraInput(MyGuiInput input)
        {
            if (input.IsGameControlPressed(MyGameControlEnums.CONTROL_SECONDARY_CAMERA))
            {
                var rotationIndicator = new Vector3(input.GetMouseYForGamePlay() - MyMinerGame.ScreenSizeHalf.Y, input.GetMouseXForGamePlay() - MyMinerGame.ScreenSizeHalf.X, 0) * MyGuiConstants.MOUSE_ROTATION_INDICATOR_MULTIPLIER;

                rotationIndicator.X -= input.GetGameControlAnalogState(MyGameControlEnums.ROTATION_UP) * MyRemoteCameraConstants.ROTATION_SENSITIVITY_NON_MOUSE;
                rotationIndicator.X += input.GetGameControlAnalogState(MyGameControlEnums.ROTATION_DOWN) * MyRemoteCameraConstants.ROTATION_SENSITIVITY_NON_MOUSE;
                rotationIndicator.Y -= input.GetGameControlAnalogState(MyGameControlEnums.ROTATION_LEFT) * MyRemoteCameraConstants.ROTATION_SENSITIVITY_NON_MOUSE;
                rotationIndicator.Y += input.GetGameControlAnalogState(MyGameControlEnums.ROTATION_RIGHT) * MyRemoteCameraConstants.ROTATION_SENSITIVITY_NON_MOUSE;

                rotationIndicator *= MyConstants.PHYSICS_STEPS_PER_SECOND * MyGuiConstants.ROTATION_INDICATOR_MULTIPLIER;

                if (rotationIndicator != Vector3.Zero)
                {
                    var remoteCamera = MySession.PlayerShip.GetSelectedRemoteCamera();
                    if (remoteCamera != null)
                        remoteCamera.Rotate(-rotationIndicator);
                }
            }
        }

        //Game and editor shares this method
        public void MoveAndRotatePlayerOrCamera(MyGuiInput input)
        {
            // Don't move camera on screenshot
            if (MyGuiManager.GetScreenshot() != null)
                return;

            bool afterburner = false;

            float rollIndicator = input.GetRoll();
            Vector2 rotationIndicator = input.GetRotation();
            Vector3 moveIndicator = input.GetPositionDelta();

            if (MyVideoModeManager.IsHardwareCursorUsed() && MyMinerGame.Static.IsMouseVisible)
                rotationIndicator = Vector2.Zero;


            if (IsGameActive()) if (input.IsGameControlPressed(MyGameControlEnums.AFTERBURNER)) afterburner = true;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Decide who is moving
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (CameraAttachedTo == MyCameraAttachedToEnum.Spectator)
            {
                if (IsEditorActive())
                {
                    //DrawMouseCursor = false; 
                    if (MyConfig.EditorUseCameraCrosshair == false)
                    {
                        if (input.IsRightMousePressed() == false)
                        {
                            rotationIndicator = Vector2.Zero;
                            //Summary	0000835: Editor - camera / spectator - different rotation equations -- rotate even when button is not pressed
                            //rollIndicator = 0f;
                            DrawMouseCursor = true;
                        }
                        else
                        {
                            if (input.IsNewRightMousePressed() && MyVideoModeManager.IsHardwareCursorUsed())
                                rotationIndicator = Vector2.Zero;
                            DrawMouseCursor = false;
                            if (MyVideoModeManager.IsHardwareCursorUsed())
                                MyGuiInput.SetMouseToScreenCenter();
                        }
                    }
                    else
                    {
                        if (input.IsNewMiddleMousePressed() && MyVideoModeManager.IsHardwareCursorUsed())
                            rotationIndicator = Vector2.Zero;

                        if (MyEditorGizmo.IsRotationActive())
                        {
                            rotationIndicator = Vector2.Zero;
                            rollIndicator = 0f;
                            DrawMouseCursor = true;
                        }
                        else
                        {
                            DrawMouseCursor = false;
                            MyGuiInput.SetMouseToScreenCenter();
                        }
                    }
                }

                MySpectator.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, (afterburner ? 1.0f : 0.35f) * (input.IsAnyCtrlKeyPressed() ? 0.3f : 1));
            }

            if (CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic)
            {
                MyThirdPersonSpectator.SetState(moveIndicator, rotationIndicator, rollIndicator);
                MyThirdPersonSpectator.QuickZoom = input.IsAnyAltKeyPressed();
            }

            // disable mouse rotation if we are in weapon select mode(cursor on)
            if ((m_selectAmmoMenu.IsEnabled && IsSelectAmmoVisible() || GetDrawMouseCursor()))
                rotationIndicator = Vector2.Zero;

            if (MySession.Static != null && MySession.Static.Player != null && !MySession.Static.Player.IsDead())
            {
                switch (CameraAttachedTo)
                {
                    case MyCameraAttachedToEnum.BotMinerShip:
                        if (ShipForSimpleTesting != null)
                            ShipForSimpleTesting.MoveAndRotate(moveIndicator, rotationIndicator, 0f, afterburner);
                        break;

                    case MyCameraAttachedToEnum.Drone:
                        ControlledDrone.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, afterburner);
                        break;
                    case MyCameraAttachedToEnum.Camera:
                        {
                            ControlledCamera.HandleInput(rotationIndicator);
                        }
                        break;

                    case MyCameraAttachedToEnum.LargeWeapon:
                        {
                            ControlledLargeWeapon.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, afterburner);
                        }
                        break;

                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic:
                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic:
                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing:
                    case MyCameraAttachedToEnum.PlayerMinerShip:
                        if (MySession.PlayerShip != null)
                            MySession.PlayerShip.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, afterburner);
                        break;
                }
            }
            if (MyVideoModeManager.IsHardwareCursorUsed() && GetDrawMouseCursor() == false && !MyMinerGame.Static.IsMouseVisible)
                MyGuiInput.SetMouseToScreenCenter();
        }

        static long lastEnvWorkingSet = 0;
        static long lastGc = 0;
        static long lastVid = 0;
        private MySoundCue? m_inventoryPlaySound;

        //  This method is called every update - even if application has not focus
        //  Update world, do physics integration...
        public override bool Update(bool hasFocus)
        {
            int profBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GuiScreenGamePlay::Update", ref profBlock);

            if (MyMultiplayerGameplay.IsRunning)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Multiplayer update");
                MyMultiplayerGameplay.Static.Update();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if (MyFakes.GRAVITATION.HasValue)
                MyPhysics.physicsSystem.Gravitation = MyFakes.GRAVITATION.Value;

            if (MySession.Is25DSector)
            {
                MyPhysics.physicsSystem.Gravitation = new Vector3(0, -4000, 0);
            }

            if (MyFakes.MWBUILDER && !MyEditor.Static.IsActive())
            {
                //MyPhysics.physicsSystem.Gravitation = new Vector3(0, -2000, 0);
                MyPhysics.physicsSystem.Gravitation = new Vector3(0, -0, 0);
                /*
    MyVoxelMap voxelMap = MyVoxelMaps.GetVoxelMaps()[0];
    MyPhysics.physicsSystem.GravitationPoints.Clear();
    MyPhysics.physicsSystem.GravitationPoints.Add(new Tuple<BoundingSphere, float>(new BoundingSphere(voxelMap.WorldAABB.GetCenter(), voxelMap.WorldAABB.Size().Length()), 2000));
              */

                MyVoxelMap voxelMap = MyVoxelMaps.GetVoxelMaps()[0];
                float distance = Vector3.Distance(voxelMap.WorldAABB.GetCenter(), MyCamera.Position);
                float alpha = MathHelper.Clamp(distance / voxelMap.WorldAABB.Size().Length(), 0, 1);

                MySector.SunProperties.BackgroundColor = Vector3.Lerp(new Vector3(5, 5, 16), Vector3.One, alpha);

            }

            //MyPhysics.physicsSystem.Gravitation = new Vector3(0,0, 0);

            CheckChatboxMessagesTTL();

            if (m_playInventoryTransferSound && MyMinerGame.TotalGamePlayTimeInMilliseconds > m_inventoryTransferSoundPlayTime)
            {
                m_playInventoryTransferSound = false;
                if (m_inventoryPlaySound == null || !m_inventoryPlaySound.Value.IsPlaying)
                    m_inventoryPlaySound = MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryTransfer);
                else
                    m_inventoryPlaySound = null;
            }


            if (m_invokeGameEditorSwitch)
                TrySwitchBetweenGameAndEditor();

#if RENDER_PROFILING
            MyRender.RenderObjectUpdatesCounter = 0;
#endif

            //  We do not want to do any update when switching between sectors
            if (m_state == MyGuiScreenState.CLOSING)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(profBlock);
                return false;
            }

            if (base.Update(hasFocus) == false)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(profBlock);
                return false;
            }

            if (m_startTimeInMilliseconds.HasValue == false)
            {
                m_startTimeInMilliseconds = MyMinerGame.TotalTimeInMilliseconds;
                //MyGuiSounds.PlayClick();
            }

            if (m_updateGPSReminderTimer > 0.5f)
            {
                m_updateGPSReminderTimer = 0;
                UpdateGPSReminder();
            }
            else
            {
                m_updateGPSReminderTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }

            ShowOrHideNotification(MyConfig.NeedShowHelpScreen, ref m_needShowHelpNotification, MyTextsWrapperEnum.NotificationNeedShowHelpScreen, null);
            ShowOrHideNotification(!MyEditor.Static.IsActive() && !MyFakes.MWBUILDER && MyHud.GetClosestOreDistanceSquared() < 100 * 100 && (MySession.PlayerShip.Weapons.GetMountedDrill() == null || MySession.PlayerShip.Weapons.GetMountedDrill().CurrentState == MyDrillStateEnum.InsideShip), ref m_oreInRangeNotification, MyTextsWrapperEnum.OreNotification, MyGameControlEnums.HARVEST);
            ShowOrHideNotification(!MyEditor.Static.IsActive() && !MyFakes.MWBUILDER && MyHud.GetClosestOreDistanceSquared() < 100 * 100 && (MySession.PlayerShip != null && MySession.PlayerShip.Weapons.GetMountedDrill() != null && MySession.PlayerShip.Weapons.GetMountedDrill().CurrentState == MyDrillStateEnum.InsideShip), ref m_drillingInRangeNotification, MyTextsWrapperEnum.DrillNotification, MyGameControlEnums.DRILL);
            ShowOrHideNotification(!MyEditor.Static.IsActive() && !MyFakes.MWBUILDER && MySession.PlayerShip != null && MySession.PlayerShip.Weapons.GetMountedDrill() != null && MySession.PlayerShip.Weapons.GetMountedDrill().CurrentState != MyDrillStateEnum.InsideShip, ref m_pressMToDeactivateDrillNotification, MyTextsWrapperEnum.PressMToDeactivateDrill, MyGameControlEnums.DRILL);
            ShowOrHideNotification(!MyEditor.Static.IsActive() && !MyFakes.MWBUILDER && MySession.PlayerShip != null && MySession.PlayerShip.Weapons.GetMountedDrill() != null && MySession.PlayerShip.Weapons.GetMountedDrill().CurrentState == MyDrillStateEnum.Activated, ref m_holdFireToDrillNotification, MyTextsWrapperEnum.HoldFireToDrillNotification, MyGameControlEnums.FIRE_PRIMARY);


            if (m_fireDisableTimeout > 0)
            {
                m_fireDisableTimeout = m_fireDisableTimeout - MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            }

            if (hasFocus == false)
            {
                //  We must reset this
                m_handleInputMouseKeysReleased = false;
            }

            if (m_firstUpdateCall)
            {
                if (MyGuiScreenLoading.Static != null)
                {
                    MyGuiScreenLoading.Static.UnloadContent();
                }

                m_loadingScreen = new MyGuiScreenLoading(null, null, MyGuiScreenLoading.LastBackgroundTexture);
                m_loadingScreen.LoadContent();
                m_firstUpdateCall = false;
                m_firstTimeLoaded = MyMinerGame.TotalTimeInMilliseconds;

                if (IsMainMenuActive())
                {
                    //Allow changing video options from game in DX version
                    MyGuiScreenMainMenu.AddMainMenu(true);
                    //MyGuiScreenMainMenu.AddMainMenu(false);
                }

                switch (m_type)
                {
                    case MyGuiScreenGamePlayType.GAME_STORY:
                        break;
                    case MyGuiScreenGamePlayType.EDITOR_SANDBOX:
                    case MyGuiScreenGamePlayType.EDITOR_MMO:
                    case MyGuiScreenGamePlayType.EDITOR_STORY:
                    case MyGuiScreenGamePlayType.INGAME_EDITOR:
                        FillEditorNotifications();
                        break;
                    default:
                        break;
                }


                if (OnGameLoaded != null)
                    OnGameLoaded(this, null);

                //  we may create GPS waypoints only after the first simulation pass
                if (MyFakes.ENABLE_GENERATED_WAYPOINTS_IN_EDITOR)
                {
                    MyWayPointGraph.CreateWaypointsAroundLargeStaticObjects();
                }
                else
                {
                    switch (m_type)
                    {
                        case MyGuiScreenGamePlayType.EDITOR_STORY:  // don't create them in the editor (use Shift+M)
                        case MyGuiScreenGamePlayType.EDITOR_MMO:
                        case MyGuiScreenGamePlayType.EDITOR_SANDBOX:
                            MyWayPointGraph.RemoveWaypointsAroundLargeStaticObjects();
                            MyWayPointGraph.SetVisibilityOfAllWaypoints(true);
                            break;
                        default:
                            MyWayPointGraph.CreateWaypointsAroundLargeStaticObjects();
                            MyWayPointGraph.SetVisibilityOfAllWaypoints(MyHud.ShowDebugWaypoints);
                            break;
                    }
                }
            }


            if (MyMinerGame.IsGameReady)
            {
                if (MyLoadingPerformance.Instance.IsTiming)
                {
                    MyLoadingPerformance.Instance.LoadingName = MyMissions.ActiveMission == null ? "No mission" : MyMissions.ActiveMission.Name.ToString();
                    MyLoadingPerformance.Instance.FinishTiming();
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Audio");

                if (MyAudio.GetMusicState() == MyMusicState.Stopped &&
                    MyAudio.GetMusicCue() == null &&
                    !MyAudio.HasAnyTransition() &&
                    MyMinerGame.TotalTimeInMilliseconds - m_firstTimeLoaded >= 1000)
                {
                    if (IsMainMenuActive())
                    {
                        MyAudio.ApplyTransition(MyMusicTransitionEnum.MainMenu);
                        MyAudio.AddCue2D(MySoundCuesEnum.MenuWelcome);
                    }
                    else if (IsCreditsActive())
                    {
                        MyAudio.ApplyTransition(MyMusicTransitionEnum.MainMenu);
                    }
                    else if (IsGameActive() || IsPureFlyThroughActive())
                    {
                        MyAudio.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);
                    }
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Editor");

                if (IsEditorActive() || IsIngameEditorActive())
                {
                    MyEditor.Static.Update();
                    EditorControls.Update();
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRadar.Update();
                MyFriendlyFire.Update();

                if (m_fadingOut)
                {
                    m_fadeAlpha += m_fadeSpeed;
                    if (m_fadeAlpha >= 1.0f)
                    {
                        m_fadeAlpha = 1.0f;
                        if (FadedOut != null)
                            FadedOut();
                        m_fadingOut = false;
                    }
                }
                else if (m_fadingIn)
                {
                    m_fadeAlpha -= m_fadeSpeed;
                    if (m_fadeAlpha <= 0.0f)
                    {
                        m_fadeAlpha = 0.0f;
                        if (FadedIn != null)
                            FadedIn();
                        m_fadingIn = false;
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("NonBackgroundThread");
            if (!MyEditor.Static.IsBackgroundWorkThreadAlive())
            {
                if (m_backgroudThreadWasWorking)
                {
                    MyRender.Enabled = true;
                }
                m_backgroudThreadWasWorking = false;

                if (MySession.PlayerShip != null)
                {
                    ProcessSectorBoundaries();
                }

                //  If we detect logged off player during active game (story or MMO, but not main-menu-fly-through), we close actual game and practicaly restart it all
                if (StartTimeoutClosing)
                {
                    //  We must shut-down any game that may be played right now - becase loggouted player is a weird state and we need to reset it all
                    MyGuiManager.BackToMainMenu();
                    MyGuiManager.CloseAllScreensExceptThisOneAndAllTopMost(this);
                    StartTimeoutClosing = false;
                }

                if (MyMinerGame.IsPaused())
                {
                    MyCamera.Zoom.PauseZoomCue();
                }
                else
                {

                    if (MyMinerGame.IsGameReady)
                    {
                        if (IsGameActive())
                        {
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Global events");
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySunWind");
                            MySunWind.IsVisible = MyGuiScreenSolarSystemMap.Static == null;
                            MySunWind.Update();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMeteorWind");
                            MyMeteorWind.Update();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyIceStorm");
                            MyIceStorm.Update();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAttackFormations");
                            MyAttackFormations.Instance.Update();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            //TODO
                            /*
                           if (MyFakes.ENABLE_SHOUT)
                           {
                               MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyShouts");
                               MyShouts.Update();
                               MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                           }  */
                        }

                        MyRoutefindingHelper.AdvanceRoutefinding();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEntityDetectorsManager.Update");
                        MyEntityDetectorsManager.Update();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEntities.UpdateBeforeIntegration");
                        UpdateBeforeSimulation();
                        MyEntities.UpdateBeforeSimulation();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Dust+MyExplosions+MyDistantImpostors");
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateAndCalculateDustColors");
                        UpdateAndCalculateDustColors();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Explosions update");
                        MyExplosions.Update();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Impostors update");
                        MyDistantImpostors.Update();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


                        if (MyFakes.MWBUILDER)
                        {                          /*
                        MyVoxelMap voxelMap = MyVoxelMaps.GetVoxelMaps()[0];

                        voxelMap.InvalidateCache(new MyMwcVector3Int(0, 0, 0), new MyMwcVector3Int(1024, 1024, 1024));
                        MyVoxelMaps.RecalcVoxelMaps();     */
                        }



                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyPhysics-Simulate");
                        if (deltaphys <= 0)
                        {
                            if (PHYSICS_SIMULATION_SLOWDOWN)
                                deltaphys = 30;
                            else
                                deltaphys = 0;
                            //  Physics integration
                            // dont allow physics for editor - god mode 
                            switch (m_type)
                            {
                                case MyGuiScreenGamePlayType.EDITOR_STORY:
                                case MyGuiScreenGamePlayType.EDITOR_MMO:
                                case MyGuiScreenGamePlayType.EDITOR_SANDBOX:
                                    //MyPhysics.physicsSystem.Simulate(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                                    MyPhysics.physicsSystem.Simulate(0.0f);
                                    break;
                                default:
                                    MyPhysics.physicsSystem.Simulate(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                                    break;
                            }
                        }
                        else
                            deltaphys--;
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyTrailerLoad, MyEntities-UAI, MySunWind, WpGen, MySession");
                        //  If this is trailer in load phase, we update attached object's positions to values from files
                        //  We must do it before 'after integration', otherwise velocity/speed won't be calculated with correct positions
                        MyTrailerLoad.Update();

                        if (IsGameActive() || IsIngameEditorActive())
                        {
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySession.Update");
                            System.Diagnostics.Debug.Assert(MySession.Static != null, "Session cannot be null in the game");
                            MySession.Static.Update();
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        }

                        MyEnemyTargeting.Update();

                        //Do it after MySession.Static.Update(); because they move objects there, and we shoot here
                        MyEntities.UpdateAfterSimulation();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyProjectiles");
                        //Be careful, projectiles must be updated AFTER physics to get correct velocity results!
                        //Also must be after UpdateAfterIntegration to have fired bullets
                        MyProjectiles.Update();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


                        //Update sun
                        MyRender.Sun.Direction = -MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
                        MyRender.Sun.Color = MySunWind.IsActive ? MySunWind.GetSunColor() : new Vector4(MySector.SunProperties.SunDiffuse, 1.0f);
                        MyRender.Sun.BackColor = MySector.SunProperties.BackSunDiffuse;
                        MyRender.Sun.BackIntensity = MySector.SunProperties.BackSunIntensity;
                        MyRender.Sun.SpecularColor = MySector.SunProperties.SunSpecular;
                        MyRender.Sun.Intensity = MySector.SunProperties.SunIntensity * (MySession.Is25DSector ? 2.5f : 1);
                        MyRender.AmbientColor = MySector.SunProperties.AmbientColor;
                        MyRender.AmbientMultiplier = MySector.SunProperties.AmbientMultiplier;
                        MyRender.EnvAmbientIntensity = MySector.SunProperties.EnvironmentAmbientIntensity;


                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //  From where will be camera looking? (needs to be called after physics integration - because for view matrix we need to know current position of player's ship)
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Camera");
                MyCamera.Update();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update spectator");

                if ((CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip) && (MySession.PlayerShip == null))
                {
                    CameraAttachedTo = MyCameraAttachedToEnum.Spectator;
                }

                switch (CameraAttachedTo)
                {
                    case MyCameraAttachedToEnum.Spectator:
                        MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
                        break;

                    case MyCameraAttachedToEnum.PlayerMinerShip:
                        MyCamera.SetViewMatrix(MySession.PlayerShip.GetViewMatrix());
                        break;

                    case MyCameraAttachedToEnum.BotMinerShip:
                        if (Static.ShipForSimpleTesting != null)
                        {
                            MyCamera.SetViewMatrix(MyGuiScreenGamePlay.Static.ShipForSimpleTesting.GetViewMatrix());
                        }
                        break;

                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing:
                        MySpectator.Position = MySession.PlayerShip.GetPosition() + ThirdPersonCameraDelta;
                        MySpectator.Target = MySession.PlayerShip.GetPosition();
                        MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
                        break;

                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic:
                        {
                            MySmallShip playerShip = MySession.PlayerShip;

                            if (MySession.Is25DSector)
                            {

                                Vector3 forward = playerShip.GetWorldRotation().Forward;
                                forward.Y = 0;
                                forward.Normalize();
                                //Vector3 right = Vector3.Normalize(playerShip.GetWorldRotation().Right.Project(Vector3.Right));
                                Vector3 right = Vector3.Right;
                                Vector3 up = Vector3.Up;
                                right = Vector3.Cross(up, forward);

                                MyThirdPersonSpectator.TargetOrientation = Matrix.CreateWorld(Vector3.Zero, forward, up);

                                //MyThirdPersonSpectator.TargetOrientation = Matrix.Identity;
                            }
                            else
                                MyThirdPersonSpectator.TargetOrientation = playerShip.GetWorldRotation();

                            MyThirdPersonSpectator.Target = playerShip.GetPosition();

                            MyThirdPersonSpectator.Update();
                            if (!MySession.Is25DSector)
                            {
                                MyThirdPersonSpectator.HandleIntersection(
                                    playerShip, true, playerShip.GetHeadPosition(), playerShip.GetHeadDirection());
                            }

                            MyCamera.SetViewMatrix(MyThirdPersonSpectator.GetViewMatrix(
                                MyCamera.FieldOfView,
                                MyCamera.Zoom.GetZoomLevel(),
                                true, playerShip.GetHeadPosition(), playerShip.GetHeadDirection())
                                );
                        }
                        break;

                    case MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic:
                        MyCamera.SetViewMatrix(MySpectator.GetViewMatrix());
                        break;

                    case MyCameraAttachedToEnum.Drone:
                        Debug.Assert(IsControlledDrone);
                        MyCamera.SetViewMatrix(ControlledDrone.GetViewMatrix());
                        break;
                    case MyCameraAttachedToEnum.Camera:
                        Debug.Assert(ControlledCamera != null);
                        MyCamera.SetViewMatrix(ControlledCamera.GetViewMatrix());
                        //MyCamera.ProjectionMatrix = (ControlledCamera.GetProjectionMatrix());
                        break;

                    case MyCameraAttachedToEnum.LargeWeapon:
                        Debug.Assert(ControlledLargeWeapon != null);
                        MyCamera.SetViewMatrix(ControlledLargeWeapon.GetViewMatrix());
                        break;
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.PrepareEntitiesForDrawStart();

                if (MyMinerGame.IsGameReady)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update playership");

                    // Update spectator reflector properties
                    if (MySession.PlayerShip != null)
                    {
                        MyRender.DrawSpectatorReflector = CameraAttachedTo == MyCameraAttachedToEnum.Spectator;

                        // Update player light properties
                        MyRender.PlayerLight = MySession.PlayerShip.Light;
                        MyRender.SpectatorReflector = MySession.PlayerShip.Light;
                        MyRender.DrawPlayerLightShadow = CameraAttachedTo != MyCameraAttachedToEnum.PlayerMinerShip;


#if !RENDER_PROFILING
                        //We dont want to be bothered by gameplay stuff in editor or profiling
                        if ((m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_STORY) ||
                            (m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX) ||
                            (m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_MMO))
#endif
                        {
                            MySession.PlayerShip.Fuel = MySession.PlayerShip.MaxFuel;
                            MySession.PlayerShip.Oxygen = MySession.PlayerShip.MaxOxygen;
                            MySession.PlayerShip.AddHealth(MySession.PlayerShip.MaxHealth);
                            MySession.PlayerShip.ArmorHealth = MySession.PlayerShip.MaxArmorHealth;
                            MySession.PlayerShip.Weight = MySession.PlayerShip.ShipTypeProperties.Physics.Mass;
                            MySession.Static.Player.RestoreHealth();

                            foreach (Inventory.MyInventoryItem item in MySession.PlayerShip.Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
                            {
                                item.Amount = item.MaxAmount;
                            }
                        }

                        if (IsCheatEnabled(MyGameplayCheatsEnum.INFINITE_AMMO))
                        {
                            foreach (Inventory.MyInventoryItem item in MySession.PlayerShip.Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
                            {
                                item.Amount = item.MaxAmount;
                            }
                        }
                        if (IsCheatEnabled(MyGameplayCheatsEnum.INFINITE_FUEL))
                        {
                            MySession.PlayerShip.Fuel = MySession.PlayerShip.MaxFuel;
                        }
                        if (IsCheatEnabled(MyGameplayCheatsEnum.INFINITE_OXYGEN))
                        {
                            MySession.PlayerShip.Oxygen = MySession.PlayerShip.MaxOxygen;
                        }

                        if ((m_quickZoomOut || m_quickZoomWasUsed) && MyCamera.Zoom.GetZoomLevel() == 1.0f)
                        {
                            m_quickZoomOut = false;
                            m_quickZoomWasUsed = false;
                        }
                    }
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                //ProcessSectorBoundaries();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //  Update camera forward and sound after view matrix is updated, because you need actual forward and up vectors
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                MyCamera.EnableForward();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.ReverbControl");

                //  Update reverb coeficient and whole sound engine. This needs to be called!!!
                //MyFpsManager.AddToFrameDebugText("MySounds.ReverbControl: " + MyUtils.GetFormatedFloat(MySounds.ReverbControl, 5));
                MyAudio.ReverbControl = MyVoxelMaps.GetReverb(MyCamera.Position);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLights.Update");
                MyLights.Update();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyDebrisField.Update();

            }
            else
            {
                m_backgroudThreadWasWorking = true;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            if (MyMinerGame.IsGameReady)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Trailer+UpdateMenu+Hud");

                if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE == true/* && GetGameType() != MyGuiScreenGamePlayType.GAME_SANDBOX*/)
                {
                    MyTrailerSave.UpdatePositionsAndOrientations();
                    MyTrailerSave.IncreaseActiveTick();
                }

                if (IsGameActive())
                {
                    m_selectAmmoMenu.Update();
                    m_wheelControlMenu.Update();
                }

                MyHudNotification.Update();

                if (IsGameActive()) // enable cursor drawing if we have ammo selection opened
                {
                    DrawMouseCursor = (m_selectAmmoMenu.IsEnabled && IsSelectAmmoVisible());
                }


                if (MyFakes.TEST_MULTIPLE_LOAD_UNLOAD && m_loadMultiple)
                {
                    m_loadMultiple = false;
                    int count = m_multipleLoadsCount;

                    /*
                    string username = MyConfig.Username;
                    string password = MyConfig.Password;

                    int modelMeshes = MyPerformanceCounter.PerAppLifetime.MyModelsMeshesCount;
                    int modelVertices = MyPerformanceCounter.PerAppLifetime.MyModelsVertexesCount;
                    int modelTriangles = MyPerformanceCounter.PerAppLifetime.MyModelsTrianglesCount;

                    MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                                                  new MyGuiScreenStartQuickLaunch(
                                                                      MyMwcQuickLaunchType.NEW_STORY, MyTextsWrapperEnum.StartGameInProgressPleaseWait), null));
                    */

                    MyScriptWrapper.TravelToMission(MyFakes.TEST_MULTIPLE_LOAD_UNLOAD_MISSION);


                    return false;
                }


                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(profBlock);




            if (MyFakes.TEST_MISSION_GAMEPLAY && MyMinerGame.IsGameReady)
            {
                m_missionGameplayTestDelay -= MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_missionGameplayTestDelay <= 0)
                {
                    m_missionGameplayTestDelay = MyFakes.TEST_MISSION_GAMEPLAY_DURATION;

                    if (m_missionGameplayKillsRemaining > 0)
                    {
                        m_missionGameplayKillsRemaining--;
                        MySession.PlayerShip.DoDamage(0, 10000, 0, MyDamageType.Explosion, MyAmmoType.Explosive, null);
                        MyGuiScreenGamePlay.Static.Restart();
                        return false;
                    }

                    if (MyMissions.ActiveMission == null)
                    {
                        MyMissions.RefreshAvailableMissions();
                        var availableMissions = MyMissions.GetAvailableMissions();
                        if (availableMissions.Count > 0)
                        {
                            MyMission mission = null;
                            foreach (MyMission m in availableMissions)
                            {
                                if (m.RequiredMissions.Contains(m_lastMissionID))
                                {
                                    mission = m;
                                    break;
                                }
                            }

                            if (mission != null)
                            {
                                MyScriptWrapper.TravelToMission(mission.ID);
                            }
                        }
                        else
                        { //no next missions => we have reached the end of singleplayer game
                        }
                    }
                    else
                    {
                        m_lastMissionID = MyMissions.ActiveMission.ID;
                        MyObjective activeObjective = MyMissions.ActiveMission.ActiveObjectives[0];
                        m_lastObjective = activeObjective;

                        if (m_missionGameplayKillsRemaining == 0 && (m_lastObjective == null || m_lastObjective.SaveOnSuccess))
                        {
                            m_missionGameplayKillsRemaining = MyFakes.TEST_MISSION_GAMEPLAY_AUTO_KILLS;
                        }

                        MyMissionGameplayStats statistics = new MyMissionGameplayStats
                        {
                            FPS = MyFpsManager.GetFps(),
                            FrameTimeAvg = (float)MyFpsManager.FrameTimeAvg,
                            FrameTimeMax = (int)MyFpsManager.FrameTimeMax,
                            FrameTimeMin = (int)MyFpsManager.FrameTimeMin,

                            GC = GC.GetTotalMemory(false),
                            WorkingSet = Environment.WorkingSet,

                            // TODO: Videomem
                            VideoMemAllocated = 1.0f, //MyProgram.GetResourcesSizeInMB(),
                            VideoMemAvailable = MyMinerGame.Static.GraphicsDevice.AvailableTextureMemory / (1024.0f * 1024.0f)
                        };

                        Dictionary<MyMissionID, MyMissionGameplayStats> missionStats;
                        m_missionGameplayStats.TryGetValue(MyMissions.ActiveMission.ID, out missionStats);
                        if (missionStats == null)
                        {
                            missionStats = new Dictionary<MyMissionID, MyMissionGameplayStats>();
                            m_missionGameplayStats.Add(MyMissions.ActiveMission.ID, missionStats);
                        }

                        if (missionStats.ContainsKey(activeObjective.ID))
                        {
                            missionStats.Remove(activeObjective.ID);
                        }

                        missionStats.Add(activeObjective.ID, statistics);


                        //Store them each time because of possible crash
                        StoreMissionStats();

                        if (activeObjective.Location != null && activeObjective.Location.Entity != null)
                        {
                            Vector3 objectivePosition = activeObjective.Location.Entity.GetPosition();
                            MySession.PlayerShip.SetPosition(objectivePosition + -20 * Vector3.Forward);
                        }
                        else
                        {
                            if (activeObjective.MissionEntityIDs.Count > 0)
                            {
                                MyEntity ent = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(activeObjective.MissionEntityIDs[0]));
                                if (ent != null)
                                {
                                    Vector3 objectivePosition = ent.GetPosition();
                                    MySession.PlayerShip.SetPosition(objectivePosition + -20 * Vector3.Forward);
                                }
                            }
                        }

                        MyObjective.SkipSubmission = true;
                    }
                }
                return false;
            }

            // detect if controlled large weapon or camera are in working state (Enabled and IsElectrified)
            if (ControlledLargeWeapon != null && !ControlledLargeWeapon.IsWorking() && !DetachingForbidden)
            {
                CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip;
                ReleaseControlOfLargeWeapon();
            }
            if (ControlledCamera != null && !ControlledCamera.IsWorking())
            {
                ReleaseControlOfCamera();
            }

            /*
            m_backgroundWorkerCanRun = false;
            while (m_backgroundWorkerRunning)
            {
            }
              */
            return true;
        }

        private void StoreMissionStats()
        {
            var filePath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "_MissionStatistics.txt");
            using (var output = new StreamWriter(File.Open(filePath, FileMode.Create)))
            {
                StringBuilder line = new StringBuilder();

                line.Clear();
                line.Append("Mission,Objective,FPS,FrameTimeAvg,FrameTimeMin,FrameTimeMax,GC,WorkingSet,VMAllocated,VMAvailable");
                output.WriteLine(line);

                foreach (var missionStats in m_missionGameplayStats)
                {
                    foreach (var stats in missionStats.Value)
                    {
                        line.Clear();
                        line.Append(MyTextsWrapper.Get(MyMissions.GetMissionByID(missionStats.Key).Name).ToString() + ",");
                        line.Append(MyMissions.GetMissionByID(stats.Key).NameTemp.ToString() + ",");
                        line.Append(stats.Value.FPS.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.FrameTimeAvg.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.FrameTimeMin.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.FrameTimeMax.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.GC.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.WorkingSet.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.VideoMemAllocated.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        line.Append(stats.Value.VideoMemAvailable.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",");
                        output.WriteLine(line);
                    }
                }

                output.Flush();
                output.Close();
            }
        }

        private void ShowOrHideNotification(bool show, ref MyHudNotification.MyNotification notification, MyTextsWrapperEnum text, MyGameControlEnums? gameControl)
        {
            if (show)
            {
                if (notification == null)
                {
                    notification = new MyHudNotification.MyNotification(text, MyHudConstants.MISSION_FONT, MyHudNotification.DONT_DISAPEAR, null, null);
                    if (gameControl.HasValue)
                    {
                        notification.SetTextFormatArguments(new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(gameControl.Value) });
                    }
                    notification.Disappear();
                }

                if (notification.IsDisappeared())
                {
                    notification.Appear();
                    MyHudNotification.AddNotification(notification);
                }
            }
            else
            {
                if (notification != null && !notification.IsDisappeared())
                    notification.Disappear();
            }
        }

        private void UpdateGPSReminder()
        {
            var gpsTarget = /*MyMissions.ActiveMission == null ? null : */MyHud.GetGPSGoalEntity(MySession.PlayerShip.GetPosition());

            if (m_gpsLocationExistsTime.HasValue)
            {
                var x = MyMinerGame.TotalGamePlayTimeInMilliseconds - MathHelper.Max(m_gpsLocationExistsTime.Value, MyHud.LastGPS);
                bool timeCheck = MyMinerGame.TotalGamePlayTimeInMilliseconds - MathHelper.Max(m_gpsLocationExistsTime.Value, MyHud.LastGPS) > TIME_UNTIL_GPS_REMINDER;

                if (timeCheck)
                {
                    if (!MyHud.IsGPSWorking() && m_showGPSNotification.IsDisappeared() && gpsTarget != null && MySession.PlayerShip == ControlledEntity)
                    {
                        m_showGPSNotification.SetTextFormatArguments(new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.GPS) });
                        m_showGPSNotification.Appear();
                        MyHudNotification.AddNotification(m_showGPSNotification);
                    }
                }
                else
                {
                    // Hide notification when ie. mission initiated new GPS call
                    m_showGPSNotification.Disappear();
                }

                if (gpsTarget == null)
                {
                    m_showGPSNotification.Disappear();
                    m_gpsLocationExistsTime = null;
                }
            }
            else
            {
                if (gpsTarget != null)
                {
                    m_gpsLocationExistsTime = MyMinerGame.TotalTimeInMilliseconds;
                }
            }
        }

        void BackgroundWorkerThread()
        {
            while (m_backgroundWorkerCanRun)
            {
                //GameTime gameTime = GetGameTime(ref lastTime);

                DrawLoadAnimation();

                //UpdateNetworkSession();
            }

            m_backgroundWorkerRunning = false;
        }

        public void DrawLoading()
        {
            if (m_loadingScreen != null)
            {
                MyGuiManager.BeginSpriteBatch();

                m_loadingScreen.DrawLoading(0.0f);

                MyGuiManager.EndSpriteBatch();
            }
        }

        public void DrawLoadAnimation()
        {
            if ((MyMinerGame.Static.GraphicsDevice == null) || MyMinerGame.Static.GraphicsDevice.IsDisposed)
                return;

            try
            {
                DrawLoading();

                //   MyMinerGameDX.Static.GraphicsDevice.Present();

                //  Thread.Sleep(MyGuiConstants.LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS);
            }
            catch
            {
                // If anything went wrong (for instance the graphics device was lost
                // or reset) we don't have any good way to recover while running on a
                // background thread. Setting the device to null will stop us from
                // rendering, so the main game can deal with the problem later on.
                //MyMinerGameDX.Static.GraphicsDevice = null;
            }
        }

        private void UpdateBeforeSimulation()
        {
            //MySession.PlayerShip.Faction = MyMwcObjectBuilder_FactionEnum.China;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenGameplay.UpdateBeforeIntegration");

            m_madelynRefillTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            if (m_madelynRefillTimer > MyMissionConstants.MADELYN_REFILL_TIME)
            {
                RefillMadelyn();
                m_madelynRefillTimer -= MyMissionConstants.MADELYN_REFILL_TIME;
            }

            MyBotCoordinator.Update();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void UpdateAndCalculateDustColors()
        {
            ResultDustColor = Vector4.Zero; //not sure where to zeroize this
            // Calculate weight for each color and then calculate result color using weights
            Vector4 sectorDustColor = MyGuiScreenGamePlay.SectorDustColor;
            Vector4 colorSum = Vector4.Zero;
            PartialDustColor.Add(sectorDustColor);
            foreach (Vector4 color in PartialDustColor)
            {
                colorSum += color;
            }

            Vector4 resultColor = Vector4.Zero;
            foreach (Vector4 color in PartialDustColor)
            {
                Vector4 weight = color / colorSum;
                resultColor += color * weight;
            }

            if (resultColor == sectorDustColor)
                resultColor = Vector4.Zero;
            ResultDustColor = resultColor;
            PartialDustColor.Clear();
        }

        public static void FillEditorNotifications()
        {
            if (MyFakes.EDITOR_ENABLE_HOWTO_NOTIFICATION)
            {
                // Notification about association of the selection & rotation controls:
                object[] args = new object[2];
                args[0] = MyGuiManager.GetInput().GetControl(MyMouseButtonsEnum.Left, MyGuiGameControlType.EDITOR).GetControlButtonName(MyGuiInputDeviceEnum.Mouse);
                args[1] = MyGuiManager.GetInput().GetControl(MyMouseButtonsEnum.Right, MyGuiGameControlType.EDITOR).GetControlButtonName(MyGuiInputDeviceEnum.Mouse);
                MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.NotificationEditorSelectAndRotate, null, true, args));
            }
        }

        void InitSounds()
        {
            if (m_type == MyGuiScreenGamePlayType.CREDITS || m_type == MyGuiScreenGamePlayType.MAIN_MENU)
            {
                MyAudio.GameSoundsOn = false;
            }
            else
            {
                MyAudio.GameSoundsOn = true;
            }

            MyAudio.MusicOn = true;

            if (m_type == MyGuiScreenGamePlayType.PURE_FLY_THROUGH)
            {
                MyAudio.GameSoundsOn = true;
                MyAudio.MusicOn = true;
            }
        }

        public static MyGuiScreenLoading ReloadGameplayScreen(MyMwcObjectBuilder_Checkpoint checkpoint, MyMwcStartSessionRequestTypeEnum? sessionType = null, MyGuiScreenGamePlayType? gameplayType = null, MyMissionID? startMission = null, MyMwcTravelTypeEnum? travelType = null)
        {
            if (MyMultiplayerGameplay.IsRunning)
                MyMultiplayerGameplay.Static.Suspend();

            if (checkpoint.SectorObjectBuilder != null)
            {
                MySession.Static.Is2DSector = MyMwcSectorIdentifier.Is25DSector(checkpoint.SectorObjectBuilder.Name);
            }

            Debug.Assert((gameplayType != null && sessionType != null) || MyGuiScreenGamePlay.Static != null, "Set gameplay type and session type, there's no previous gameplay screen");
            MyGuiScreenGamePlayType newGameplayType = gameplayType.HasValue ? gameplayType.Value : MyGuiScreenGamePlay.Static.GetGameType();
            MyMwcStartSessionRequestTypeEnum? newSessionType = sessionType.HasValue ? sessionType.Value : MyGuiScreenGamePlay.Static.GetSessionType();
            MyGuiScreenGamePlayType? previousGameplaytype = MyGuiScreenGamePlay.Static != null ? MyGuiScreenGamePlay.Static.GetPreviousGameType() : (MyGuiScreenGamePlayType?)null;
            MyMissionID? previousMissionToStart = MyGuiScreenGamePlay.Static != null ? MyGuiScreenGamePlay.Static.m_missionToStart : (MyMissionID?)null;

            var newGameplayScreen = new MyGuiScreenGamePlay(newGameplayType, previousGameplaytype, checkpoint.CurrentSector, checkpoint.SectorObjectBuilder.Version, newSessionType);
            newGameplayScreen.m_missionToStart = previousMissionToStart;
            if (travelType.HasValue)
                newGameplayScreen.m_travelReason = travelType.Value;


            var loadScreen = new MyGuiScreenLoading(newGameplayScreen, MyGuiScreenGamePlay.Static);
            loadScreen.AddEnterSectorResponse(checkpoint, startMission);

            /*
            if (MyConfig.NeedShowPerfWarning)
            {
                MyGuiScreenPerformanceWarning perfWarningScreen = new MyGuiScreenPerformanceWarning(loadScreen);

                perfWarningScreen.Closed += delegate
                {
                    MyGuiManager.AddScreen(loadScreen);
                };

                MyGuiManager.AddScreen(perfWarningScreen);
            }
            else*/
            {
                MyGuiManager.AddScreen(loadScreen);
            }

            return loadScreen;
        }

        public void TravelToSector(MyMwcSectorIdentifier sectorIdentifier, MyMwcTravelTypeEnum travelType, Vector3 currentShipPosition, MyMissionID? missionToStart = null)
        {
            if (MySession.Static != null)
            {
                if (m_type == MyGuiScreenGamePlayType.GAME_STORY && !MyClientServer.LoggedPlayer.GetCanSave()) // Game travel is not allowed when user cannot save
                {
                    return;
                }
                if (!CanTravel && !MyFakes.ENABLE_SOLAR_MAP && !MyFakes.TEST_MULTIPLE_LOAD_UNLOAD)
                {
                    return;
                }

                m_transferToNeighbouringSectorStarted = true;

                m_travelSectorIdentifier = sectorIdentifier;
                m_travelReason = travelType;
                m_missionToStart = missionToStart;

                if (m_type != MyGuiScreenGamePlayType.EDITOR_STORY && m_previousType != MyGuiScreenGamePlayType.EDITOR_STORY && !MyFakes.DISABLE_AUTO_SAVE)
                {
                    MySession.Static.Player.TravelLeave(m_travelReason);

                    if (!MyMinerGame.IsPaused())
                        MyMinerGame.SwitchPause();

                    MyServerAction serverAction = MySession.Static.SaveLastCheckpoint(); // Saving before travel, save sector
                    travelSaveSuccess();
                }
                else
                {
                    travelSaveSuccess();
                }
            }
        }

        void travelSaveFailed()
        {
            ClampPlayerToBorderSafeArea();

            m_transferToNeighbouringSectorStarted = false;

            // Unpack
            MySession.Static.Player.TravelEnter();

            if (MyMinerGame.IsPaused())
                MyMinerGame.SwitchPause();
        }

        void travelSaveSuccess()
        {
            Debug.Assert(m_travelSectorIdentifier.SectorType == MyMwcSectorTypeEnum.STORY, "Travel is only possible in story game and story editor");
            MySession.Static.Travel(m_travelReason, m_travelSectorIdentifier);
        }

        public void ClampPlayerToBorderSafeArea()
        {
            // Move player out of travel zone
            float safeDistance = 50;
            float maxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF - safeDistance;
            Vector3 border = new Vector3(maxDistance, maxDistance, maxDistance);
            MySession.PlayerShip.SetPosition(Vector3.Clamp(MySession.PlayerShip.GetPosition(), -border, border));
        }

        //  Check if we are still in sector and if not load neighbouring sector. Also display info to user that we are approaching the border.
        void ProcessSectorBoundaries()
        {
            if ((IsGameActive() == false) || (m_transferToNeighbouringSectorStarted)) return;

            Debug.Assert(MySession.PlayerShip != null);

            m_distanceToSectorBoundaries = MyUtils.GetClosestDistanceFromPointToAxisAlignedBoundingBox(
                MySession.PlayerShip.GetPosition(), MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX);

            if (!CanTravelThroughSectorBoundaries())
            {
                Vector3 playerPosition = MySession.PlayerShip.GetPosition();

                Vector3 newPlayerPosition = Vector3.Clamp(playerPosition, new Vector3(-MyMwcSectorConstants.SECTOR_SIZE_HALF), new Vector3(MyMwcSectorConstants.SECTOR_SIZE_HALF));

                if (!MyMwcUtils.IsZero(newPlayerPosition - playerPosition))
                {
                    MySession.PlayerShip.SetPosition(newPlayerPosition);
                }

                return;
            }

            if (MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX.Contains(MySession.PlayerShip.GetPosition()) == MinerWarsMath.ContainmentType.Disjoint)
            {
                //  Choose neighbouring sector in the correct direction
                MyMwcVector3Int neighSectorRelCoord = GetSectorIdentifierCoordinatesFromRealCoordinates(MySession.PlayerShip.GetPosition());
                MyMwcSectorIdentifier sectorIdentifier = new MyMwcSectorIdentifier(m_sectorIdentifier.SectorType, m_sectorIdentifier.UserId, new MyMwcVector3Int(
                    m_sectorIdentifier.Position.X + neighSectorRelCoord.X,
                    m_sectorIdentifier.Position.Y + neighSectorRelCoord.Y,
                    m_sectorIdentifier.Position.Z + neighSectorRelCoord.Z),
                    null);

                if (CanTravelToSector(sectorIdentifier.Position))
                {
                    m_prepareTextureForSectorLoadingScreen = true;
                    if (m_isPreparedTextureForSectorLoadingScreen)
                    {
                        TravelToSector(sectorIdentifier, MyMwcTravelTypeEnum.NEIGHBOUR, MySession.PlayerShip.GetPosition());
                    }
                }
                else
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ForbiddenSolarMapAreaWarning, MyTextsWrapperEnum.Warning, MyTextsWrapperEnum.Ok, null));

                    Vector3 playerPosition = MySession.PlayerShip.GetPosition();
                    playerPosition = Vector3.Clamp(playerPosition, new Vector3(-MyMwcSectorConstants.SECTOR_SIZE_HALF), new Vector3(MyMwcSectorConstants.SECTOR_SIZE_HALF));
                    MySession.PlayerShip.SetPosition(playerPosition);
                }
            }
        }


        //  Returns sector coordinates e.g. [0, -1, 0] for specified "point"
        // This method was modified to allow travel only to 6 neighbour sectors (up, down, left, right, front, back), no diagonals!
        public static MyMwcVector3Int GetSectorIdentifierCoordinatesFromRealCoordinates(Vector3 point)
        {
            MyMwcVector3Int offset = new MyMwcVector3Int(
                (int)(point.X / MyMwcSectorConstants.SECTOR_SIZE_HALF),
                (int)(point.Y / MyMwcSectorConstants.SECTOR_SIZE_HALF),
                (int)(point.Z / MyMwcSectorConstants.SECTOR_SIZE_HALF));

            if (Math.Abs(point.X) < Math.Max(Math.Abs(point.Y), Math.Abs(point.Z)))
            {
                offset.X = 0;
            }
            if (Math.Abs(point.Y) < Math.Max(Math.Abs(point.X), Math.Abs(point.Z)))
            {
                offset.Y = 0;
            }
            if (Math.Abs(point.Z) < Math.Max(Math.Abs(point.X), Math.Abs(point.Y)))
            {
                offset.Z = 0;
            }
            return offset;
        }

        public float GetDistanceToSectorBoundaries()
        {
            return m_distanceToSectorBoundaries;
        }

        //  Fading in/out at the beginning and at the end of trailer.
        //  Or fading-in at the beginning of normal game.
        void DrawBackgroundFade(float alpha)
        {
            if (alpha <= 0) return;

            MyGuiManager.BeginSpriteBatch();
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), new Color(new Vector4(0, 0, 0, alpha)));
            MyGuiManager.EndSpriteBatch();
        }

        private Vector2 NormalizedSize()
        {
            return new Vector2((float)MyMinerGame.ScreenSize.X / 1920f, (float)MyMinerGame.ScreenSize.Y / 1200f);
        }


        private Vector2 NormalizedSize(MyTexture2D tex)
        {
            return new Vector2(tex.Width / 1920f * ((float)MyMinerGame.ScreenSize.Y * 1920f) / ((float)MyMinerGame.ScreenSize.X * 1200f), (tex.Height / 1200f));
        }

        private Rectangle PosToRect(Vector2 pos, Vector2 tex, MyGuiDrawAlignEnum align)
        {
            int Xoff = 0, Yoff = 0;
            switch (align)
            {
                case MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X / 2);
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X / 2);
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y / 2);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X / 2);
                    Yoff = 0;
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM:
                    Xoff = 0;
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER:
                    Xoff = 0;
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y / 2);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP:
                    Xoff = 0;
                    Yoff = 0;
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X);
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X);
                    Yoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y / 2);
                    break;
                case MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP:
                    Xoff = -(int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X);
                    Yoff = 0;
                    break;
            }
            return new Rectangle((int)(Xoff + pos.X * (float)MyMinerGame.ScreenSize.X), (int)(Yoff + pos.Y * (float)MyMinerGame.ScreenSize.Y), (int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.X), (int)((float)MyMinerGame.ScreenSize.Y / 1200 * tex.Y));
        }

        private void DrawBloodAndOxygen()
        {
            if (MySession.PlayerShip != null && MySession.PlayerShip.PlayerBloodTextureOpacity > 0)
            {
                MyTexture2D bloodTexture = MyGuiManager.GetBloodTexture();
                if (bloodTexture != null)
                {
                    MyGuiManager.BeginSpriteBatch();
                    MyGuiManager.DrawSpriteBatch(bloodTexture, new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), new Color(1, 1, 1, MySession.PlayerShip.PlayerBloodTextureOpacity));
                    MyGuiManager.EndSpriteBatch();
                }
            }
            else
            {
                MyGuiManager.RemoveBloodTexture();
            }

            if (MySession.PlayerShip != null && MySession.PlayerShip.Oxygen <= 0)
            {
                MyTexture2D oxygenTexture = MyGuiManager.GetLowOxygenTexture();
                if (oxygenTexture != null)
                {
                    MyGuiManager.BeginSpriteBatch();
                    float opacity = MySession.PlayerShip.NoOxygenTextureOpacity;
                    MyGuiManager.DrawSpriteBatch(oxygenTexture, new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), new Color(opacity, opacity, opacity, opacity));
                    MyGuiManager.EndSpriteBatch();
                }
            }
            else
            {
                MyGuiManager.RemoveNoOxygenTexture();
            }
        }

        private float lastBeepTime = 0;
        private bool beepStatus = true;

        private void DrawRemoteControlScreen()
        {
            if (CameraAttachedTo == MyCameraAttachedToEnum.Camera)
            {
                MyGuiManager.BeginSpriteBatch();
                {
                    Dictionary<string, MyTexture2D> textures = MyGuiManager.GetRemoteViewCameraTextures();
                    Dictionary<string, Vector2> sizes = MyGuiManager.GetRemoteViewCameraSizes();
                    Dictionary<string, Vector2> positions = MyGuiManager.GetRemoteViewCameraPositions();
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_RASTR], new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM], sizes[MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), Color.White);
                    if (lastBeepTime + 1000 < MyMinerGame.TotalGamePlayTimeInMilliseconds)
                    {
                        beepStatus = !beepStatus;
                        lastBeepTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    }
                    if (beepStatus)
                    {
                        MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_REC], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_CAMERA_REC], sizes[MyGuiConstants.REMOTE_VIEW_CAMERA_REC], MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM), Color.White);
                    }
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS], sizes[MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER), Color.White);
                }
                MyGuiManager.EndSpriteBatch();
            }
            else if (CameraAttachedTo == MyCameraAttachedToEnum.Drone)
            {
                MyGuiManager.BeginSpriteBatch();
                {
                    Dictionary<string, MyTexture2D> textures = MyGuiManager.GetRemoteViewDroneTextures();
                    Dictionary<string, Vector2> sizes = MyGuiManager.GetRemoteViewDroneSizes();
                    Dictionary<string, Vector2> positions = MyGuiManager.GetRemoteViewDronePositions();
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_DRONE_RASTR], new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM], sizes[MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_DRONE_CROSS], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_DRONE_CROSS], sizes[MyGuiConstants.REMOTE_VIEW_DRONE_CROSS], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER), Color.White);
                }
                MyGuiManager.EndSpriteBatch();
            }
            else if (CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon)
            {
                MyGuiManager.BeginSpriteBatch();
                {
                    Dictionary<string, MyTexture2D> textures = MyGuiManager.GetRemoteViewWeaponTextures();
                    Dictionary<string, Vector2> sizes = MyGuiManager.GetRemoteViewWeaponSizes();
                    Dictionary<string, Vector2> positions = MyGuiManager.GetRemoteViewWeaponPositions();
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RASTR], new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO], MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS], MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE], MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM), Color.White);
                    MyGuiManager.DrawSpriteBatch(textures[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE], PosToRect(positions[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE], sizes[MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE], MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER), Color.White);
                }
                MyGuiManager.EndSpriteBatch();
            }
        }

        float GetBackgroundFadeAlpha()
        {
            int delta = MyMinerGame.TotalTimeInMilliseconds - m_startTimeInMilliseconds.Value;
            if (delta > MyGuiConstants.GAME_PLAY_SCREEN_FADEIN_IN_MILLISECONDS) return 0;
            return 1 - ((float)delta / (float)MyGuiConstants.GAME_PLAY_SCREEN_FADEIN_IN_MILLISECONDS);
        }

        [Conditional("AUTOBUILD")]
        public static void AutobuildTest()
        {
            if (FrameCounter <= 10)
            {
                FrameCounter++;
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public bool DrawHud = true;

        public override bool Draw(float backgroundFadeAlpha)
        {


            int drawBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenGamePlay::Draw", ref drawBlock);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Autobuild test");

            AutobuildTest();


            //  We start/restart all batched objects in our sprite batch because in game-play screen we do a lot of stuff that's not compatible with it
            MyGuiManager.EndSpriteBatch();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Editor load");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Draw Editor model
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (IsEditorActive())
            {
                MyEditor.Static.LoadInDraw();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Camera reset");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Draw forward
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            MyPerformanceCounter.PerCameraDraw.Reset();

#if HOMEKEY_RESET_ENVMAP
            // Home key to manually refresh environment map
            if (Keyboard.GetState().IsKeyDown(Keys.Home))
            {
                MyEnvironmentMap.Reset();
            }
#endif

            //While getting cell, there must not be any other thread running because this voxel can be loading at same time
            System.Diagnostics.Debug.Assert(!(MyEditor.Static != null && MyEditor.Static.IsBackgroundWorkThreadAlive() && MyRender.Enabled));

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_firstDrawCall == FIRST_TOTAL_DELAY");

            if (m_firstDrawCall == FIRST_TOTAL_DELAY)
            {
                MyMwcLog.WriteLine("First draw call - before render");

                MyRender.GetRenderProfiler().StartProfilingBlock("Load meteor wind");
                MyMeteorWind.LoadInDraw();
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("Preload entities");
                //                MyTextureManager.DbgUpdateStats();
                MyTextureManager.OverrideLoadingMode = LoadingMode.Immediate;
                MyRender.PreloadEntitiesInRadius(MyRenderConstants.RenderQualityProfile.LodTransitionDistanceFar, MyGuiScreenGamePlay.Static.DrawLoadAnimation);
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("Preload ship");
                MySession.PlayerShip.GetShipCockpit(); //Reload
                MySession.PlayerShip.GetShipCockpitGlass(); //Reload
                MyRender.GetRenderProfiler().EndProfilingBlock();

                if (IsEditorActive())
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("Prefab helper icons");
                    MyGuiObjectBuilderHelpers.UpdatePrefabHelperIcons();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                bool foundCache = false;
                MyRender.GetRenderProfiler().StartProfilingBlock("MyLocalVoxelTrianglesCache.LoadAllVoxels");
                if (MyFakes.ENABLE_VOXEL_TRIANGLE_CACHING)
                {
                    foundCache = MyLocalVoxelTrianglesCache.LoadAllVoxels();
                }
                else
                {
                    foundCache = true;
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaps.RecalcVoxelMaps");
                MyVoxelMaps.RecalcVoxelMaps();
                MyRender.GetRenderProfiler().EndProfilingBlock();

                if (MyFakes.ENABLE_VOXEL_TRIANGLE_CACHING)
                {
                    if (!foundCache)
                    {
                        MyRender.GetRenderProfiler().StartProfilingBlock("MyLocalVoxelTrianglesCache.SaveAllVoxels");
                        MyLocalVoxelTrianglesCache.SaveAllVoxels();
                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyRender.PreloadTextures");
                MyVoxelMaterials.MarkAllAsUnused(); // Mark all as unused
                MyRender.PreloadTexturesInRadius(MyMwcSectorConstants.SECTOR_SIZE); // Used voxel textures will be marked as used
                MyVoxelMaterials.UnloadUnused(); // Unload all unused (static asteroid voxel textures won't be unused, because they are referenced)
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyEnvironmentMap");

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyEnvironmentMap.Update");
                MyEnvironmentMap.Update();
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("secondary camera");

            bool drawSecondaryCameraIndicators =
                MySession.PlayerShip != null &&
                IsGameActive() &&
                DrawHud &&
                !MyRenderConstants.RenderQualityProfile.ForwardRender &&
                (IsControlledPlayerShip || IsControlledDrone);

            bool shipSecondaryCamera = IsControlledPlayerShip && ControlledShip.Config.BackCamera.On;
            bool droneSecondaryCamera = IsControlledDrone && ControlledDrone.Config.BackCamera.On;

            bool drawSecondaryCamera =
                drawSecondaryCameraIndicators &&
                (shipSecondaryCamera || droneSecondaryCamera);

            if (drawSecondaryCamera)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MySecondaryCamera.Render");
                MySecondaryCamera.Instance.Render();
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }


            MyCamera.EnableForward();

            MyRender.EnableLightsRuntime = ((IsEditorActive() && MyEditor.EnableLightsInEditor) || !IsEditorActive()) && MyGuiScreenDebugRenderLights.EnableRenderLights;

            // for drone, use smaller near plane distance
            var useSecondaryRenderSetup = IsControlledDrone;
            if (useSecondaryRenderSetup)
            {
                m_secondarySetup.CallerID = MyRenderCallerEnum.Main;
                m_secondarySetup.Fov = MyCamera.FovWithZoom;
                m_secondarySetup.AspectRatio = MyCamera.AspectRatio;
                m_secondarySetup.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    m_secondarySetup.Fov.Value,
                    m_secondarySetup.AspectRatio.Value,
                    MyDroneConstants.NEAR_PLANE_DISTANCE,
                    MyCamera.FAR_PLANE_DISTANCE);
                MyRender.PushRenderSetup(m_secondarySetup);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Main render");

            MyRender.Draw();

            if (useSecondaryRenderSetup)
            {
                MyRender.PopRenderSetup();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("FillDebugScreen");

            FillDebugScreen();

            DrawLoadsCount();


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Draw secondary camera and its gui texts
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("DrawSecondaryCamera");
            if (drawSecondaryCamera)
            {
                DrawSecondaryCamera();
            }

            if (drawSecondaryCameraIndicators)
            {
                DrawSecondaryCameraDescriptions(drawSecondaryCamera);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Bloody remote");

            DrawBloodAndOxygen();

            DrawRemoteControlScreen();

            MyRender.GetRenderProfiler().StartProfilingBlock("DrawHUDAndEditor");
            DrawHUDAndEditor(DrawHud, drawSecondaryCamera);
            MyRender.GetRenderProfiler().EndProfilingBlock();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Please consider that drawing sprites changed render state, so it must be restored after all 2D rendering is done
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Hud");


            if (IsGameActive())
            {
                //  Enable forward look again (for drawing GUI, texts...)
                MyCamera.EnableForward();
            }

            if (IsEditorActive())
            {
                if (MyConfig.EditorUseCameraCrosshair && MyEditorGizmo.IsRotationActive() == false)
                {
                    MyHud.DrawCameraCrosshair();
                }
            }
            //  Draw fade-in or fade-out (alpha 1 means that black rectangle will be totaly opaque and we won't see nothing from the game)
            float backgroundFadeAlphaFlyThrough = (!MyGuiScreenGamePlay.Static.IsGameActive() && !IsEditorActive() && MyTrailerLoad.IsEnabled()) ? MyTrailerLoad.GetBackgroundFadeAlpha() : GetBackgroundFadeAlpha();
            DrawBackgroundFade(backgroundFadeAlphaFlyThrough);

            //  We start/restart all batched objects in our sprite batch because in game-play screen we do a lot of stuff that's not compatible with it
            MyGuiManager.BeginSpriteBatch();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("EditorControls");

            if ((IsEditorActive() || IsIngameEditorActive()) && !MyMinerGame.IsPaused())
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("EditorControls.Draw");
                EditorControls.Draw();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            MyGuiManager.EndSpriteBatch();

            if (m_prepareTextureForSectorLoadingScreen && !m_isPreparedTextureForSectorLoadingScreen)
            {
                m_textureForSectorLoadingScreen = MyRender.GetBackBufferAsTexture();
                m_isPreparedTextureForSectorLoadingScreen = true;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiManager.DrawSpriteBatch");
                //  ResolveBackBuffer will remove what we rendered from backbuffer so we need to render it again otherwise there will be blink on screen
                MyGuiManager.BeginSpriteBatch();
                MyGuiManager.DrawSpriteBatch(m_textureForSectorLoadingScreen, Vector2.Zero, Color.White);
                MyGuiManager.EndSpriteBatch();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCamera.EnableForward");
            MyCamera.EnableForward();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            MyGuiManager.BeginSpriteBatch();

            if (MyMinerGame.IsGameReady && m_loadingScreen != null)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_loadingScreen.UnloadContent");
                m_loadingScreen.UnloadContent();
                m_loadingScreen = null;
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("--m_firstDrawCall == FIRST_FADE_IN_DELAY");

            if (--m_firstDrawCall == FIRST_FADE_IN_DELAY)
            {
                MyMwcLog.WriteLine("First draw call - after render");
                MyTextureManager.DbgUpdateStats();
                MyTextureManager.OverrideLoadingMode = null;
                GC.Collect();
                MyMinerGame.IsGameReady = true;

                m_backgroundFadeColor = new Vector4(0, 0, 0, 1.0f);
                m_fadeAlpha = 1.0f;
                FadeIn(1 / ((float)FIRST_FADE_IN_DELAY * MyConstants.PHYSICS_STEPS_PER_SECOND));
                GameReady();
            }

            if (m_firstDrawCall == FIRST_GAME_SAFELY_LOADED_DELAY)
            {
                MyMwcLog.WriteLine("Draw call - game safely loaded");

                GameSafelyLoaded();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_firstDrawCall > FIRST_FADE_IN_DELAY");

            if (m_firstDrawCall > FIRST_GAME_SAFELY_LOADED_DELAY)
            {
                DrawLoading();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return true;
            }
            else
                if (m_firstDrawCall > FIRST_FADE_IN_DELAY)
                {
                    MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.All, new ColorBGRA(0), 1, 0);
                }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_firstDrawCall == FIRST_DELAY_SAFE_IN_GAME");

            if (m_firstDrawCall == FIRST_DELAY_SAFE_IN_GAME)
            {
                if (MyFakes.TEST_MULTIPLE_SAVE_LOAD && this.GetGameType() == MyGuiScreenGamePlayType.EDITOR_STORY)
                {
                    MultipleSaveLoadTest();
                }
            }

            //MyMinerGameDX.GraphicsDeviceManager.DbgDumpLoadedResources(true);
            //MyTextureManager.DbgDumpLoadedTexturesBetter(true);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyFlashes.Draw");


            MyFlashes.Draw();


            if (m_firstDrawCall <= 0)
            {
                MyMinerGame.IsGameReady = true;
            }
            if (base.Draw(m_fadeAlpha) == false)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return false;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyFlashes.Draw");

            MyDebugDraw.Draw();
            MyEntities.DebugDrawStatistics();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Demo texts");


            if (DrawDemoEnd && IsGameActive() && MyGuiManager.GetScreensCount() == 1)
            {
                m_demoEndText.Clear();
                m_demoEndText.AppendLine("Mission Succesful!");
                m_demoEndText.AppendLine("Official Demo ends here.");
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_demoEndText, new Vector2(0.5f, 0.5f), 3f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            if (DrawCampaignEnd && IsGameActive() && MyGuiManager.GetScreensCount() == 1)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsGreen(), MyTextsWrapper.Get(MyTextsWrapperEnum.CampaignIsCompleted), new Vector2(0.5f, 0.5f), 3f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            // Clear stencil to avoid overflow of following GUI controls
            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Stencil, new ColorBGRA(0), 1, 0);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(drawBlock);

            return true;
        }

        private static HashSet<uint> m_multipleReloadEntityIds;

        private void MultipleSaveLoadTest()
        {
            Debug.Assert(this == MyGuiScreenGamePlay.Static, "Something went terribly wrong");

            if (m_multipleReloadEntityIds != null)
            {
                HashSet<uint> m_currentEntities = new HashSet<uint>();
                GetAllEntityIds(MyEntities.GetEntities(), m_currentEntities);
                bool isOk = true;

                foreach (var id in m_multipleReloadEntityIds)
                {
                    if (!m_currentEntities.Remove(id))
                    {
                        // Current entities did not contain required entity id!
                        //Debug.Fail("Missing entity ID: " + id.ToEntityId());
                        isOk = false;
                        MyMwcLog.WriteLine("SAVE/LOAD TEST, missing entity id " + id.ToEntityId());
                    }
                }

                if (isOk)
                {
                    MyMwcLog.WriteLine("SAVE/LOAD TEST reload OK, num reloads: " + m_multipleLoadsCount);
                }
                saveSector(null);
            }
            else
            {
                MyMwcLog.WriteLine("Starting SAVE/LOAD TEST sector name: " + m_sectorIdentifier.SectorName + ", sector Id: " + m_sectorIdentifier);

                m_multipleReloadEntityIds = new HashSet<uint>();
                GetAllEntityIds(MyEntities.GetEntities(), m_multipleReloadEntityIds);

                // Little hack to save it to another sector (not to overwrite default)
                var userId = MyClientServer.LoggedPlayer.GetUserId();
                m_sectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, null, new MyMwcVector3Int(245450000 + userId, 0, -345457126), "ReloadTest_" + userId.ToString());

                // Lets tell server we are editing another sector (and drop his response)
                var switchSectorScreen = new MyGuiScreenLoadCheckpointProgress(m_sectorIdentifier.SectorType, MyMwcSessionStateEnum.EDITOR, m_sectorIdentifier.UserId, m_sectorIdentifier.Position, null, DummyEnterAction);
                MyGuiManager.AddScreen(switchSectorScreen);
                switchSectorScreen.Closed += new ScreenHandler(saveSector);
            }
        }

        void saveSector(MyGuiScreenBase source)
        {
            var progressScreen = MyEditor.Static.SaveSector();
            progressScreen.Closed += new ScreenHandler(progressScreen_Closed);
        }

        void DummyEnterAction(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
        }

        void GetAllEntityIds(IEnumerable<MyEntity> entities, HashSet<uint> addToList)
        {
            foreach (var e in entities)
            {
                if (e == MySession.PlayerShip)
                    continue;

                if (e is MySmallDebris)
                    continue;

                if (e.EntityId.HasValue)
                {
                    addToList.Add(e.EntityId.Value.NumericValue);
                }
                GetAllEntityIds(e.Children, addToList);
            }
        }

        void progressScreen_Closed(MyGuiScreenBase source)
        {
            MyGuiManager.CloseAllScreensNowExcept(MyGuiScreenGamePlay.Static);

            MyEntityIdentifier.Reset();

            MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(MyMwcStartSessionRequestTypeEnum.EDITOR_STORY, MyTextsWrapperEnum.LoadingPleaseWait,
                    m_sectorIdentifier,
                    CommonLIB.AppCode.ObjectBuilders.MyGameplayDifficultyEnum.EASY,
                    null,
                    null));
        }

        private void InitSecretRooms()
        {
            if (MySteam.IsActive && GetGameType() == MyGuiScreenGamePlayType.GAME_STORY)
            {
                foreach (var dummy in MyEntities.GetEntities().OfType<MyDummyPoint>())
                {
                    if (dummy.SecretRoomName != null)
                    {
                        var detector = dummy.GetDetector();
                        detector.OnEntityEnter += new OnEntityEnter(MyGuiScreenGamePlay_OnEntityEnter);
                        detector.On();
                    }
                }
            }
        }

        void MyGuiScreenGamePlay_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                var dummy = (MyDummyPoint)sender.Parent;
                MySteamStats.SetStat(dummy.SecretRoomName, 1);

                int secretsFound = 0;

                foreach (var room in MySecretRooms.SecretRooms)
                {
                    if (MySteamStats.GetStatInt(room.Value) > 0)
                        secretsFound++;
                }

                var oldVal = MySteamStats.GetStatInt(MySteamStatNames.FoundSecrets);
                if (oldVal != secretsFound)
                {
                    MySteamStats.SetStat(MySteamStatNames.FoundSecrets, secretsFound);
                    MySteamStats.IndicateAchievementProgress(MySteamAchievementNames.SecretRooms, (uint)secretsFound, (uint)MySecretRooms.SecretRooms.Count);
                    MySteamStats.StoreState();
                }
            }
        }

        private void GameReady()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GameReady()");

            InitSecretRooms();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("RebuildCullingStructure");

            // Rebuild culling structure in first frame
            MinerWars.AppCode.Game.Render.MyRender.RebuildCullingStructure();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("OnGameReady");

            if (OnGameReady != null)
            {
                OnGameReady(this);
                OnGameReady = null;
            }

            if (MyMultiplayerGameplay.IsEditor())
            {
                MyEntityIdentifier.CurrentPlayerId = 0;
            }

            if (IsControlledPlayerShip && MySession.Is25DSector)
                MySession.PlayerShip.Config.ViewMode.SetValue(MyViewModeTypesEnum.ThirdPerson);


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyMultiplayerGameplay");

            // No multiplayer in editor
            if (GetGameType() != MyGuiScreenGamePlayType.EDITOR_SANDBOX
                    && GetGameType() != MyGuiScreenGamePlayType.EDITOR_MMO
                    && GetGameType() != MyGuiScreenGamePlayType.EDITOR_STORY)
            {
                // Multiplayre was not started yet
                if (!MyMultiplayerGameplay.IsRunning)
                {
                    if (MyMultiplayerGameplay.Static.IsHost) // If i am host i will inform the server that i try to host and after that start multiplayer
                    {
                        m_hostTryCount = 0;
                        HostGame();
                    }
                    else // If I'm not host that means I'm joining something thus start the multiplayer
                    {
                        MyMultiplayerGameplay.Static.OnShutdown = new Action(Multiplayer_Shutdown);
                        MyMultiplayerGameplay.Static.Start();
                    }
                }
                else // Multiplayer is running already, traveling / reload
                {
                    MyMultiplayerGameplay.Static.OnShutdown = new Action(Multiplayer_Shutdown);
                    MyMultiplayerGameplay.Static.Resume(); // I will resume recieving packets

                    if (MyMultiplayerGameplay.Static.IsHost) // If i am host i will also send new checkpoint to everyone
                    {
                        MyMultiplayerGameplay.Static.ReloadCheckpoint();
                    }
                }

                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.OnNotification = new Action<MyNotificationType, MyTextsWrapperEnum, object[]>(Multiplayer_OnNotification);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void GameSafelyLoaded()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GameLoaded()");

            if (OnGameSafelyLoaded != null)
            {
                OnGameSafelyLoaded(this);
                OnGameSafelyLoaded = null;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        void Multiplayer_Shutdown()
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MP_HostDisconnected, MyTextsWrapperEnum.MP_HostDisconnectedCaption, MyTextsWrapperEnum.Ok, Multiplayer_ShutdownOk));
        }

        void Multiplayer_ShutdownOk(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            MyGuiManager.BackToMainMenu();
        }

        private List<MyJoinGameRequest> m_joinRequests = new List<MyJoinGameRequest>();

        public struct MyJoinGameRequest
        {
            public MyEventEnterGame Message;
            public MyHudNotification.MyNotification Notification;
        }

        private void joinGameNotificationConfirmed(MyJoinGameRequest request)
        {
            m_joinRequests.Remove(request);
            MyGuiManager.AddModalScreen(new MyGuiScreenMultiplayerEnterGameRequest(request), null);
        }

        public string GetGameName(MyGameTypes gameType, MyGameplayDifficultyEnum difficulty)
        {
            if (gameType == MyGameTypes.Story)
            {
                if (MyMissions.ActiveMission != null)
                {
                    return MyTextsWrapper.Get(MyMissions.ActiveMission.Name).ToString();
                }
                else
                {
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.FreeRoaming).ToString();
                }
            }
            else
            {
                return String.IsNullOrWhiteSpace(m_sectorIdentifier.SectorName) ? Checkpoint.SectorObjectBuilder.Name : m_sectorIdentifier.SectorName;
            }
        }

        void HostGame()
        {
            if (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.IsDemoUser()) return;

            MyGameTypes gameType = MyMultiplayerGameplay.GameType;

            MyJoinMode joinMode;
            if (gameType == MyGameTypes.Story)
                joinMode = MyJoinMode.Closed;
            else
                joinMode = MyJoinMode.Open;

            MyGameplayDifficultyEnum difficulty;
            if (gameType == MyGameTypes.Deathmatch)
                difficulty = MyGameplayDifficultyEnum.NORMAL;
            else
                difficulty = MyGameplayConstants.GetGameplayDifficulty();

            string gameName = GetGameName(gameType, difficulty);

            try
            {
                MyMultiplayerLobby.Static.HostGame(gameName ?? "Empty sector", String.Empty, gameType, (r) => HostGameResponse(r), joinMode, difficulty);
            }
            catch (Exception)
            {
                MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.MP_CannotHost, MyGuiManager.GetFontMinerWarsRed(), 10000));
            }
        }

        private int m_hostTryCount = 0;

        void HostGameResponse(MyResultCodeEnum resultCode)
        {
            if (resultCode == MyResultCodeEnum.OK) // Hosting game was succesful
            {
                MyMultiplayerGameplay.Static.Start(); // We can start multiplayer on host now
                MyMultiplayerGameplay.Static.OnShutdown = new Action(Multiplayer_Shutdown);
                MyMultiplayerGameplay.Static.OnNotification = new Action<MyNotificationType, MyTextsWrapperEnum, object[]>(Multiplayer_OnNotification);
            }
            else
            {
                if (m_hostTryCount >= 1)
                {
                    Multiplayer_OnNotification(MyNotificationType.Text, MyTextsWrapperEnum.MP_GameHostFailed_X, new object[] { });
                }
                else
                {
                    m_hostTryCount++;
                    HostGame();
                }
            }
        }

        static void Multiplayer_OnNotification(MyNotificationType notificationType, MyTextsWrapperEnum arg1, object[] arg2)
        {
            if (notificationType == MyNotificationType.Text)
            {
                MyHudNotification.MyNotification notification = new MyHudNotification.MyNotification(arg1, 3000, null, arg2);
                MyHudNotification.AddNotification(notification);
            }
            else if (notificationType == MyNotificationType.WaitStart)
            {
                MyGuiManager.CloseIngameScreens();

                MyGuiManager.AddModalScreen(new MyGuiScreenWaitingOnHost(arg1), null);
                MyMultiplayerGameplay.IsWaiting = true;
                if (!MyMinerGame.IsPaused())
                    MyMinerGame.SwitchPause();
            }
            else if (notificationType == MyNotificationType.WaitEnd)
            {
                MyMultiplayerGameplay.IsWaiting = false;
                if (!MyMinerGame.IsPaused())
                    MyMinerGame.SwitchPause();
                CloseWaitingScreen();
            }
        }

        static void CloseWaitingScreen()
        {
            var dlg = MyGuiManager.GetScreenWithFocus() as MyGuiScreenWaiting;
            if (dlg != null)
            {
                dlg.CloseScreen();
            }
        }

        private static void DrawSecondaryCamera()
        {
            MyGuiManager.BeginSpriteBatch();
            DrawCameraScreen();
            MyGuiManager.EndSpriteBatch();

            MyRender.TakeScreenshot("BackCamera", MyRender.GetRenderTarget(MyRenderTargets.SecondaryCamera), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
        }

        static void DrawSecondaryCameraDescriptions(bool drawSecondaryCamera)
        {
            MyGuiManager.BeginSpriteBatch();

            var positionScreenSpace =
                MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(
                    new Vector2(MyCamera.BackwardViewport.X, 0));

            //var heightWithPadding = MyHudConstants.BACK_CAMERA_HEIGHT * 1.222f;

            // adjustments for spacing
            positionScreenSpace.X += 0.0037f;
            positionScreenSpace.Y += 0.011f;

            if (drawSecondaryCamera)
            {
                MyGuiManager.DrawString(
                    MyGuiManager.GetFontMinerWarsBlue(),
                    MySession.PlayerShip.GetCameraDescription(),
                    positionScreenSpace,
                    MySecondaryCameraConstants.SECONDARY_CAMERA_DESCRIPTION_SCALE,
                    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            positionScreenSpace.Y += 0.011f;

            if (drawSecondaryCamera)
            {
                positionScreenSpace.Y += MyHudConstants.BACK_CAMERA_HEIGHT;
                positionScreenSpace.Y += 0.010f;
            }

            if (drawSecondaryCamera || MySession.PlayerShip.Drones.Count > 0)
            {
                FormatCameraHelp();

                MyGuiManager.DrawString(
                MyGuiManager.GetFontMinerWarsBlue(),
                m_cameraHelpBuilder,
                positionScreenSpace,
                MySecondaryCameraConstants.SECONDARY_CAMERA_DESCRIPTION_SCALE,
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM);
            }

            MyGuiManager.EndSpriteBatch();
        }

        private void DrawHUDAndEditor(bool mainDrawHud, bool backCamera)
        {
            MyGuiManager.BeginSpriteBatch();

            bool drawHud = (IsGameActive() && mainDrawHud && MySession.PlayerShip != null && MyHud.Visible) || MyHud.ShowDebugHud || MyFakes.MWBUILDER;
            if (drawHud)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyHudDrawPassEnum.FIRST");
                MyHud.Draw(MyHudDrawPassEnum.FIRST, CameraAttachedTo, backCamera);
                MyRender.GetRenderProfiler().EndProfilingBlock();
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("DrawChatScreen");
                    DrawChatScreen();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }

            //  Enable forward look again (for drawing GUI, texts...)
            MyCamera.EnableForward();

            MyRender.GetRenderProfiler().StartProfilingBlock("MyEditor.Static.Draw");
            MyEditor.Static.Draw(IsIngameEditorActive() || IsEditorActive());
            MyRender.GetRenderProfiler().EndProfilingBlock();

            if (drawHud)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyHudNotification.Draw");
                if (MyHudNotification.HasNotification(MyHudNotification.GetCurrentScreen()))
                {
                    MyHudNotification.Draw();
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("MyHudDrawPassEnum.SECOND");
                MyHud.Draw(MyHudDrawPassEnum.SECOND, CameraAttachedTo, backCamera);
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("DrawChatScreen2");
                if (MyMultiplayerGameplay.IsRunning)
                {
                    DrawChatScreen();
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("m_selectAmmoMenu.Draw");
                if (MyHud.CanDrawElement(CameraAttachedTo, MyHudDrawElementEnum.AMMO))
                {
                    m_selectAmmoMenu.Draw();
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("m_wheelControlMenu.Draw");
                if (MyHud.CanDrawElement(CameraAttachedTo, MyHudDrawElementEnum.WHEEL_CONTROL))
                {
                    m_wheelControlMenu.Draw();
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else if ((IsGameActive() || IsIngameEditorActive()) && mainDrawHud)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("DrawOnlyMissionObjectives");
                MyHud.DrawOnlyMissionObjectives();
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("DrawChatScreen3");
                if (MyMultiplayerGameplay.IsRunning)
                {
                    DrawChatScreen();
                }
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if ((CameraAttachedTo == MyCameraAttachedToEnum.Drone || CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon) && backCamera)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyHud.DrawOnlyBackCameraBorders");
                MyHud.DrawOnlyBackCameraBorders();
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            if (CameraAttachedTo == MyCameraAttachedToEnum.Camera)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyHud.DrawAsControlledEntity");
                MyHud.DrawAsControlledEntity();
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            if (IsEditorStoryActive() && !m_sectorIdentifier.CanBeSaved())
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("Cant be saved");
                var pos = (MyGuiManager.GetScreenTextLeftBottomPosition() + MyGuiManager.GetScreenTextRightBottomPosition()) / 2;
                var scale = MyGuiConstants.LABEL_TEXT_SCALE * 1.5f;
                pos.Y -= 0.05f;
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), MyTextsWrapper.Get(MyTextsWrapperEnum.StorySectorPositionYMustBeZero),
                       pos, scale, Color.Red, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MyGuiManager.EndSpriteBatch();
        }

        private StringBuilder m_stringBuilderForText;

        internal List<MyChatMessage> GetChatMessages(int max)
        {
            if (m_chatMessages == null)
            {
                return null;
            }
            if (max < m_chatMessages.Count)
            {
                List<MyChatMessage> retval = new List<MyChatMessage>(max);
                for (int i = m_chatMessages.Count - max; i < m_chatMessages.Count; i++)
                {
                    retval.Add(m_chatMessages[i]);
                }
                return retval;
            }
            else
            {
                return m_chatMessages;
            }
        }

        internal List<MyChatMessage> GetChatMessages()
        {
            return GetChatMessages(MyGuiConstants.CHAT_WINDOW_MAX_MESSAGES_COUNT);
        }

        internal void DrawChatScreen()
        {
            if (m_stringBuilderForText == null)
            {
                m_stringBuilderForText = new StringBuilder(MyGuiConstants.CHAT_WINDOW_MAX_MESSAGE_LENGTH);
            }
            else { m_stringBuilderForText.Clear(); }

            int visibleCount = m_chatMessages.Count;

            if (visibleCount == 0) return;

            Vector2 positionOffset = new Vector2(MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0)).X - 920 / 1920f / 2 - 0.24f, -0.2f);

            Vector2 backgroundSize = Vector2.Zero;

            float rowSize = 0;

            for (int i = 0; i < visibleCount; i++)
            {
                // background size
                m_stringBuilderForText.Append(m_chatMessages[i].SenderName.ToString());
                m_stringBuilderForText.Append(": ");
                m_stringBuilderForText.Append(m_chatMessages[i].Message);
                Vector2 textSize = MyGuiManager.GetNormalizedSize(GetSenderFont(m_chatMessages[i].SenderRelation), m_stringBuilderForText, MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE);

                if (backgroundSize.X < textSize.X)
                {
                    backgroundSize.X = textSize.X;
                }
                backgroundSize.Y += textSize.Y;
                rowSize = textSize.Y;
                m_stringBuilderForText.Clear();
            }

            MyGuiManager.BeginSpriteBatch();
            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

            // Draw background
            Vector2 notificationPosition = MyGuiConstants.CHAT_WINDOW_POSITION + positionOffset;
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), notificationPosition + offset,
                backgroundSize + new Vector2(rowSize, rowSize),
                MyGuiConstants.CHAT_WINDOW_BACKGROUND_COLOR, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM);

            // Draw texts
            notificationPosition.Y -= backgroundSize.Y;
            notificationPosition.X += rowSize / 2f;
            for (int i = 0; i < visibleCount; i++)
            {
                m_stringBuilderForText.Append(m_chatMessages[i].SenderName.ToString());
                m_stringBuilderForText.Append(": ");
                MyRectangle2D size = MyGuiManager.DrawString(GetSenderFont(m_chatMessages[i].SenderRelation), m_stringBuilderForText, notificationPosition + offset,
                    MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE, MyGuiConstants.CHAT_WINDOW_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                notificationPosition.X += size.Size.X;
                MyGuiManager.DrawString(GetSenderFont(m_chatMessages[i].SenderRelation), m_chatMessages[i].Message, notificationPosition + offset,
                    MyGuiConstants.CHAT_WINDOW_MESSAGE_SCALE, MyGuiConstants.CHAT_WINDOW_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                notificationPosition.Y += rowSize;
                notificationPosition.X -= size.Size.X;
                m_stringBuilderForText.Clear();
            }
            MyGuiManager.EndSpriteBatch();
        }

        private StringBuilder GetSenderName(int senderId)
        {
            if (senderId == MyClientServer.LoggedPlayer.GetUserId())
            {
                return new StringBuilder(MyClientServer.LoggedPlayer.GetDisplayName().ToString());
            }
            else if (MyMultiplayerGameplay.IsRunning)
            {
                MyPlayerRemote player;
                if (MyMultiplayerPeers.Static.TryGetPlayer(senderId, out player))
                {
                    return new StringBuilder(player.GetDisplayName().ToString());
                }
            }
            return new StringBuilder("UNKNOWN");
        }

        internal MyGuiFont GetSenderFont(MyFactionRelationEnum senderRelation)
        {
            switch (senderRelation)
            {
                case MyFactionRelationEnum.Enemy:
                    return MyGuiManager.GetFontMinerWarsRed();
                case MyFactionRelationEnum.Friend:
                    return MyGuiManager.GetFontMinerWarsGreen();
                case MyFactionRelationEnum.Neutral:
                    return MyGuiManager.GetFontMinerWarsWhite();
                default:
                    return MyGuiManager.GetFontMinerWarsBlue();
            }
        }

        private MyFactionRelationEnum GetSenderRelation(int senderId)
        {
            if (senderId == MyClientServer.LoggedPlayer.GetUserId())
            {
                return MyFactionRelationEnum.Friend;
            }
            else if (MyMultiplayerGameplay.IsRunning)
            {
                MyPlayerRemote player;
                if (MyMultiplayerPeers.Static.TryGetPlayer(senderId, out player))
                {
                    return MyFactions.GetFactionsRelation(player.Faction, MySession.Static.Player.Faction);
                }
                Debug.Assert(false, "Game shouldn't get here!");
                return MyFactionRelationEnum.Neutral;
            }
            else
            {
                Debug.Assert(false, "Game shouldn't get here!");
                return MyFactionRelationEnum.Neutral;
            }
        }

        public bool AddChatMessage(int userId, string message)
        {
            if (m_chatMessages == null) return false;

            if (m_chatMessages.Count == MyGuiConstants.CHAT_WINDOW_MAX_MESSAGES_COUNT)
            {
                m_chatMessages.RemoveAt(0);
            }
            m_chatMessages.Add(new MyChatMessage()
            {
                Message = new StringBuilder(message),
                SenderId = userId,
                TimeStamp = MyMinerGame.TotalTimeInMilliseconds,
                SenderName = GetSenderName(userId),
                SenderRelation = GetSenderRelation(userId)
            });

            return true;
        }

        public void CheckChatboxMessagesTTL()
        {
            while (m_chatMessages.Count > 0 && m_chatMessages[0].TimeStamp + MyGuiConstants.CHAT_WINDOW_MESSAGE_TTL < MyMinerGame.TotalTimeInMilliseconds)
            {
                m_chatMessages.RemoveAt(0);
            }
        }

        private void FillDebugScreen()
        {
            //  We fill debug screen here only in test build, never in production!
            MyGuiScreenDebugStatistics debugScreen = MyGuiManager.GetScreenDebugNormal();

            if (debugScreen != null)
            {
                if (MyMwcFinalBuildConstants.IS_PUBLIC)
                {
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedVector3(
                       "Camera Position: ", MyCamera.Position));
                }
                else
                {
                    //debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedFloat("Reverb: ", MyAudio.ReverbControl));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetStrings("Camera Attached To: ",
                                                    MyEnumsToStrings.CameraAttachedTo[(int)Static.CameraAttachedTo]));

                    StringBuilder renderCellsSB = MyGuiScreenDebugStatistics.StringBuilderCache;
                    renderCellsSB.Append("Render Cells In Cache: ");
                    renderCellsSB.Concat(MyVoxelCacheRender.GetCachedCellsCount());
                    renderCellsSB.Append(" of ");
                    renderCellsSB.Concat(MyVoxelCacheRender.GetCapacity());
                    debugScreen.AddToFrameDebugText(renderCellsSB);

                    StringBuilder dataCellsSB = MyGuiScreenDebugStatistics.StringBuilderCache;
                    dataCellsSB.Append("Data Cells in Cache: ");
                    dataCellsSB.Concat(MyVoxelCacheData.GetCachedCellsCount());
                    dataCellsSB.Append(" of ");
                    dataCellsSB.Concat(MyVoxelCacheData.GetCapacity());
                    debugScreen.AddToFrameDebugText(dataCellsSB);

                    StringBuilder cellContentsSB = MyGuiScreenDebugStatistics.StringBuilderCache;
                    cellContentsSB.Append("Cell Contents: ");
                    cellContentsSB.Concat(MyVoxelContentCellContents.GetCount());
                    cellContentsSB.Append(" of ");
                    cellContentsSB.Concat(MyVoxelContentCellContents.GetCapacity());
                    debugScreen.AddToFrameDebugText(cellContentsSB);

                    if (MySession.PlayerShip != null)
                    {
                        debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedFloat(
                            "Player Speed: ", MySession.PlayerShip.Physics.Speed, " m/s"));
                        /*
                        debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedVector3(
                        "Player Ship Orientation Forward: ", MySession.PlayerShip.WorldMatrix.Forward));
                        debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedVector3(
                            "Player Ship Orientation Up: ", MySession.PlayerShip.WorldMatrix.Up));*/
                    }

                    //This does allocations
                    //debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetStrings("Sector Identifier: ", m_sectorIdentifier.ToString()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Lights: ", MyLights.GetActiveLights()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Active rigids: ", MyPhysics.physicsSystem.GetRigidBodyModule().GetActiveRigids().Count));

                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Missiles + Cannonshots Active: ", MyMissiles.GetActiveCount() + MyCannonShots.GetActiveCount()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Hologram Ships Active: ", MyHologramShips.GetActiveCount()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("ProjectilesActive: ", MyProjectiles.GetActiveCount()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Bots active: ", MySmallShipBot.TotalAliveBots));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Attacking bots: ", MyBotCoordinator.CurrentBotsAttackingPlayer.Count));

                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Render cells (vertexes count): ", MyVoxelCacheRender.GetVertexesCount()));
                    debugScreen.AddToFrameDebugText(MyGuiScreenDebugStatistics.StringBuilderCache.GetFormatedInt("Render cells (indices count): ", MyVoxelCacheRender.GetIndicesCount()));
                    if (MyVoxelContentCellContents.IsAlmostFull() == true)
                    {
                        debugScreen.AddToFrameDebugText(
                            "Voxel cell contents buffer is almost full. Game needs to be restarted or buffer will overflow!" + MyStringUtils.C_CRLF);
                    }
                    debugScreen.AddToFrameDebugText("");

                    MyPerformanceCounter.PerCameraDraw.AddToFrameDebugText(debugScreen);
                }
            }
        }

        private static void DrawCameraScreen()
        {
            var renderedTexture = MySecondaryCamera.Instance.GetRenderedTexture();

            // draw flipped texture (mirror image) if it's rear mirror
            var spriteEffect = MySecondaryCamera.Instance.MirrorImage
                                   ? SpriteEffects.FlipHorizontally
                                   : SpriteEffects.None;

            var position = new Vector2(MyCamera.BackwardViewport.X, MyCamera.BackwardViewport.Y);
            MyGuiManager.DrawSpriteBatch(renderedTexture, position, null, Color.White, 0, Vector2.Zero, 1, spriteEffect, 0);

            MyRender.TakeScreenshot("BackCamera", renderedTexture, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
        }

        private static readonly object[] m_cameraHelpArgs = new object[3];
        private static readonly StringBuilder m_cameraHelpBuilder = new StringBuilder();

        private static void FormatCameraHelp()
        {
            string cameraHelp;

            var controlledDrone = Static.ControlledDrone;
            if (controlledDrone != null)
            {
                m_cameraHelpArgs[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.CHANGE_DRONE_MODE);
                MyTextsWrapperEnum droneText = controlledDrone.HoldPosition ? MyTextsWrapperEnum.DroneHoldPosition : MyTextsWrapperEnum.DroneFollowPlayer;
                cameraHelp = MyTextsWrapper.GetFormatString(droneText, m_cameraHelpArgs);
            }
            else
            {
                m_cameraHelpArgs[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.REAR_CAM);
                m_cameraHelpArgs[1] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.PREVIOUS_CAMERA);
                m_cameraHelpArgs[2] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.NEXT_CAMERA);
                cameraHelp = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.TurnOffCamera, m_cameraHelpArgs);
            }

            m_cameraHelpBuilder.Clear();
            m_cameraHelpBuilder.Append(cameraHelp);
        }

        public static void DrawLoadsCount()
        {
            if (MyFakes.TEST_MULTIPLE_LOAD_UNLOAD || MyFakes.TEST_MULTIPLE_SAVE_LOAD)
            {
                MyDebugDraw.DrawText(new Vector2(MyGuiManager.GetSafeFullscreenRectangle().Width / 2, MyGuiManager.GetSafeFullscreenRectangle().Height / 2),
                    new StringBuilder(m_multipleLoadsCount.ToString()), Color.Red, 10.0f);
            }

            if (MyFakes.TEST_MISSION_GAMEPLAY)
            {
                string missionName = MyMissions.ActiveMission == null ? "" : MyTextsWrapper.Get(MyMissions.ActiveMission.Name).ToString();
                string objectiveName = MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives.Count > 0 ? MyMissions.ActiveMission.ActiveObjectives[0].NameTemp.ToString() : "";
                MyDebugDraw.DrawText(new Vector2(100, MyGuiManager.GetSafeFullscreenRectangle().Height / 2),
                    new StringBuilder("Test whole gameplay: " + missionName + " - " + objectiveName + "(Killed " + (MyFakes.TEST_MISSION_GAMEPLAY_AUTO_KILLS - m_missionGameplayKillsRemaining).ToString() + "/" + MyFakes.TEST_MISSION_GAMEPLAY_AUTO_KILLS + ")"), Color.Red, 1.0f);

                if (Static != null)
                {
                    MyDebugDraw.DrawText(new Vector2(100, MyGuiManager.GetSafeFullscreenRectangle().Height / 2 + 20),
                        new StringBuilder("Time: " + (Static.m_missionGameplayTestDelay / 1000.0f).ToString("#,###0.000") + " s"), Color.Red, 1.0f);
                }
            }
        }

        public override bool CloseScreen()
        {
            if (MyEditor.Static.GetEditedPrefabContainer() != null)
                MyEditor.Static.ExitActivePrefabContainer();

            return base.CloseScreen();
        }

        public override void CloseScreenNow()
        {
            if (MyEditor.Static.GetEditedPrefabContainer() != null)
                MyEditor.Static.ExitActivePrefabContainer();

            base.CloseScreenNow();
        }

        // Restarts gameplay (loads last checkpoint or reloads sandbox)
        // Be careful, this is used for Gameover screen too!
        public void Restart()
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

            if (MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.GAME_STORY)
            {
                MySession.StartLastCheckpoint();
            }
            else if (MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.GAME_SANDBOX)
            {
                var sectorIdentifier = MyGuiScreenGamePlay.Static.GetSectorIdentifier();
                MySession.StartSandbox(sectorIdentifier.Position, sectorIdentifier.UserId);
            }
            else if (MyGuiScreenGamePlay.Static.IsEditorActive())
            {
                // The "old way" in future should be on MySession
                CloseScreenNow();

                Action<MyMwcObjectBuilder_Checkpoint> enterSuccessAction = new Action<MyMwcObjectBuilder_Checkpoint>((checkpoint) =>
                {
                    var loadScreen = new MyGuiScreenLoading(new MyGuiScreenGamePlay(m_type, m_previousType, checkpoint.CurrentSector, checkpoint.SectorObjectBuilder.Version, m_sessionType.Value), MyGuiScreenGamePlay.Static);
                    loadScreen.AnnounceLeaveToServer = true;
                    loadScreen.LeaveSectorReason = MyMwcLeaveSectorReasonEnum.RELOAD;

                    loadScreen.AddEnterSectorResponse(checkpoint, null);
                    MyGuiManager.AddScreen(loadScreen);
                });
                MyMissions.Unload();
                MyGuiManager.AddScreen(new MyGuiScreenLoadCheckpointProgress(m_sectorIdentifier.SectorType, MyMwcSessionStateEnum.EDITOR, m_sectorIdentifier.UserId, m_sectorIdentifier.Position, null, enterSuccessAction));
            }
        }

        //public void StopMusic()
        //{
        //    if (MusicCue.HasValue) MyAudio.ApplyTransition(MusicCue.Value, MyMusicTransitionEnum.STOP_IMMEDIATE);
        //}

        public bool IsSelectAmmoVisible()
        {
            return m_selectAmmoMenu.Visible/* && !MyFakes.MW25D*/;
        }

        public void HideSelectAmmo()
        {
            m_selectAmmoMenu.Visible = false;
        }

        Vector3 m_directionToSunNormalized;

        public Vector3 GetDirectionToSunNormalized()
        {
            //return -MyMwcUtils.Normalize(GetPositionInMillionsOfKm());
            if (MySession.Is25DSector)
            {
                return new Vector3(0.05f, 0.8f, 0.15f);
            }
            if (MyFakes.MWBUILDER)
            {
                return new Vector3(0.05f, 0.5f, 0.65f);
            }
            return m_directionToSunNormalized;
        }

        public Vector3 GetPositionInMillionsOfKm()
        {
            Vector3 pos = new Vector3(m_sectorIdentifier.Position.X, m_sectorIdentifier.Position.Y, m_sectorIdentifier.Position.Z);
            if (pos.Length() < 3)
                return Vector3.Forward * 10; //We are in the sun

            return pos;
        }

        public bool IsCurrentSector(MyMwcVector3Int sectorPosition)
        {
            return m_sectorIdentifier.Position.Equals(sectorPosition);
        }


        public static bool IsMissionSector(MyMwcVector3Int sector)
        {
            bool missionHere = false;
            foreach (var mission in MyMissions.Missions.Values)
            {
                if (mission.Location != null)
                {
                    if (sector == mission.Location.Sector)
                    {
                        missionHere = true;
                        break;
                    }
                }
            }
            return missionHere;
        }

        public static bool CanTravelToSector(MyMwcVector3Int sector)
        {
            bool missionHere = IsMissionSector(sector);
            bool anyMissionAvaliable = false;

            if (missionHere)
            {
                foreach (var mission in MyMissions.Missions.Values)
                {
                    if (mission.IsAvailable() || mission.IsCompleted())
                    {
                        if (mission.Location != null)
                        {
                            if (sector == mission.Location.Sector)
                            {
                                anyMissionAvaliable = true;
                                break;
                            }
                        }
                    }
                }
            }

            return (Static != null) && (Static.IsGameActive()) && (!missionHere || anyMissionAvaliable)
                && (!MySectorGenerator.IsSectorInForbiddenArea(sector) || (Static.GetGameType() == MyGuiScreenGamePlayType.EDITOR_STORY) || (Static.GetPreviousGameType() == MyGuiScreenGamePlayType.EDITOR_STORY));
        }

        public void AddAllBlueprints()
        {
            if (MySession.Static != null && MySession.Static.Inventory != null)
            {
                var inventory = MySession.Static.Inventory;
                var bluePrintIds = MyMwcObjectBuilder_Base.GetObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.Blueprint);
                foreach (var bluePrintId in bluePrintIds)
                {
                    if (!inventory.Contains(MyMwcObjectBuilderTypeEnum.Blueprint, bluePrintId))
                    {
                        inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.Blueprint, bluePrintId, 1.0f, true, true);
                    }
                }
            }
        }

        public void RemoveAllBlueprints()
        {
            if (MySession.Static != null && MySession.Static.Inventory != null)
            {
                var inventory = MySession.Static.Inventory;
                inventory.RemoveInventoryItems(MyMwcObjectBuilderTypeEnum.Blueprint, null, true);
            }
        }

        public bool IsControlledByPlayer(MyPrefabLargeWeapon largeWeapon)
        {
            return MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon && ControlledLargeWeapon == largeWeapon;
        }

        public bool TryChangeCameraAttachedTo(MyCameraAttachedToEnum cameraAttachedTo)
        {
            if (DetachingForbidden)
            {
                return false;
            }

            CameraAttachedTo = cameraAttachedTo;
            return true;
        }

        private bool m_playInventoryTransferSound = false;
        private float m_inventoryTransferSoundPlayTime = 0;

        /// <summary>
        /// In milliseconds.
        /// </summary>
        float m_fireDisableTimeout;

        MyHudNotification.MyNotification m_switchControlNotification;

        private bool TakeAllItems()
        {
            bool looted = false;

            MyEntity detectedEntity = null;
            var m_entityDetector = MySession.PlayerShip.TradeDetector;

            if (m_entityDetector != null)
            {
                detectedEntity = m_entityDetector.GetNearestEntity();
            }

            MySmallShipInteractionActionEnum? detectedAction = (MySmallShipInteractionActionEnum?)m_entityDetector.GetNearestEntityCriterias();
            if (detectedEntity != null && detectedAction == MySmallShipInteractionActionEnum.Examine)
            {
                if (!MyMultiplayerGameplay.IsRunning)
                {
                    looted = Loot(detectedEntity);
                }
                else
                {
                    LootMultiplayer(detectedEntity);
                }
            }

            return looted;
        }

        private void LootMultiplayer(MyEntity entity)
        {
            MyMultiplayerGameplay.Static.LockReponse = (e, success) =>
            {
                if (e != entity)
                {
                    Debug.Fail("Something went wrong, locked different entity");
                    MyMultiplayerGameplay.Static.Lock(e, false);
                    return;
                }

                if (success)
                {
                    Loot(e);
                    MyMultiplayerGameplay.Static.Lock(e, false);
                }
            };

            MyMultiplayerGameplay.Static.Lock(entity, true);
        }

        private bool Loot(MyEntity entity)
        {
            IMyInventory m_detectedEntity = entity as IMyInventory;
            if (m_detectedEntity == null) return false;

            bool allItemsLooted = true;
            if (m_detectedEntity.Inventory.GetInventoryItems().Count > 0)
            {
                MySoundCuesEnum cueToPlay;
                if (m_detectedEntity is MyCargoBox)
                {
                    Entities.CargoBox.MyCargoBox cargo = m_detectedEntity as MyCargoBox;
                    cueToPlay = cargo.GetTakeAllSound();
                }
                else
                {
                    cueToPlay = MySoundCuesEnum.SfxTakeAllUniversal;
                }

                MyAudio.AddCue2D(cueToPlay);
                m_playInventoryTransferSound = true;

                m_inventoryTransferSoundPlayTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + 250;
            }
            for (int i = m_detectedEntity.Inventory.GetInventoryItems().Count - 1; i >= 0; i--)
            {
                if (MySession.PlayerShip.Inventory.GetInventoryItems().Count < MySession.PlayerShip.Inventory.MaxItems)
                {
                    var inventoryItem = m_detectedEntity.Inventory.GetInventoryItems()[i];
                    m_detectedEntity.Inventory.RemoveInventoryItem(inventoryItem);
                    MySession.PlayerShip.Inventory.AddInventoryItem(inventoryItem);
                }
                else
                {
                    allItemsLooted = false;
                    break;
                }
            }
            return allItemsLooted;
        }

        public void FadeOut(float speed)
        {
            m_fadeSpeed = speed;
            m_fadingOut = true;
        }

        public void FadeIn(float speed)
        {
            m_fadeSpeed = speed;
            m_fadingIn = true;
        }

        /// <summary>
        /// Returns the squared distance of the given position from nearest player's point of interest.
        /// This may include player's currently controlled entities (drone, camera, large weapon)
        /// or a remote camera he is looking through, but not directly controlling.
        /// </summary>
        public float GetDistanceSquaredFromNearestPointOfInterest(Vector3 position)
        {
            float distanceFromPlayerShip = float.MaxValue;
            float distanceFromRemoteCamera = float.MaxValue;

            if (MyMultiplayerGameplay.IsRunning)
            {
                foreach (var player in MyMultiplayerGameplay.Peers.Players)
                {
                    if (player.Ship != null)
                    {
                        distanceFromPlayerShip = Math.Min(Vector3.DistanceSquared(player.Ship.GetPosition(), position), distanceFromPlayerShip);
                    }
                }
            }
            if (MySession.PlayerShip != null)
            {
                distanceFromPlayerShip = Math.Min(Vector3.DistanceSquared(MySession.PlayerShip.GetPosition(), position), distanceFromPlayerShip);

                var selectedRemoteCamera = MySession.PlayerShip.GetSelectedRemoteCamera();
                if (selectedRemoteCamera != null)
                {
                    distanceFromRemoteCamera = Vector3.DistanceSquared(selectedRemoteCamera.GetPosition(), position);
                }
            }

            float distanceFromControlledEntity = float.MaxValue;

            if (ControlledEntity != null)
            {
                distanceFromControlledEntity = Vector3.DistanceSquared(ControlledEntity.GetPosition(), position);
            }

            // return the minimum of the three
            return Math.Min(distanceFromRemoteCamera, MathHelper.Min(distanceFromControlledEntity, distanceFromPlayerShip));
        }

        public void RefillMadelyn()
        {
            List<int> madelynKits = new List<int>()
            {
                (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT,
                (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT,
                (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT,
                (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT,
                //(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ELECTRICITY_KIT,
            };

            List<int> ammoItems = new List<int>()
            {
                (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic,
                (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic,
            };

            MyEntity madelyn = null;
            if (MyEntities.TryGetEntityByName("Madelyn", out madelyn))
            {
                var madelynContainer = madelyn as MyPrefabContainer;
                if (madelynContainer != null)
                {
                    foreach (var item in madelynContainer.Inventory.GetInventoryItems())
                    {
                        if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Tool && item.ObjectBuilderId.HasValue)
                        {
                            madelynKits.Remove(item.ObjectBuilderId.Value);
                        }

                        if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Ammo && item.ObjectBuilderId.HasValue)
                        {
                            ammoItems.Remove(item.ObjectBuilderId.Value);
                        }
                    }

                    foreach (var item in madelynKits)
                    {
                        madelynContainer.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, item, 1, true, true);
                    }

                    foreach (var item in ammoItems)
                    {
                        var inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(new MyMwcObjectBuilder_SmallShip_Ammo((MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)item));
                        inventoryItem.Amount = inventoryItem.MaxAmount;
                        madelynContainer.Inventory.AddInventoryItem(inventoryItem);
                    }
                }
            }
        }

        public override void RecreateControls(bool contructor)
        {
            m_selectAmmoMenu.ReloadControlText();
            base.RecreateControls(contructor);
        }
    }
}

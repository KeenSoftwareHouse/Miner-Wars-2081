using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Tools;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Radar;
using MinerWars.AppCode.Game.Render.SecondaryCamera;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Physics.Collisions;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Entities.Tools.ToolKits;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using MinerWars.AppCode.Game.Managers;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Entities
{
    /// <summary>
    /// Small ship controlled by player.
    /// </summary>
    class MySmallShip : MyShip
    {
        #region Structs
        private class MyModelShipSubOject
        {
            public MyModelSubObject ModelSubObject;
            public MyLight Light;
        }

        private class MyDamageOverTimeItem
        {
            public float IntervalDamage { get; set; }
            public float Damage { get; set; }
        }
        #endregion

        #region Constants

        public static float SENSITIVITY_MODIFIER_FOR_MAX_ZOOM = 0.2f;

        public static bool PlayerSimulation = true;

        private const float MIN_GLASS_DIRT_LEVEL = 0.3f;
        private const float MAX_GLASS_DIRT_LEVEL = 1f;

        private const float SOUND_VARIABLE_SHIP_A_SPEED_MAX_VALUE = 250;
        private const float SOUND_VARIABLE_SHIP_IDLE_SPEED_MAX_VALUE = 250;

        private const int MAX_AVAILABLE_SLOTS = 32;
        private const int MAX_AVAILABLE_DEVICES = 32;
        private const int MAX_SHIP_SUBOBJECTS = 128;
        private const int MAX_SAME_SUBOBJECTS = 8;
        private const float ROLL_BALANCING_FACTOR = 0.02f;//step angle in wich we approach the upper vector in strafe-roll movement .. 0 means no roll during strafing at all (up-vector stays unchanged)
        private const float UPVECTOR_UPDATE_MULTIPLICATOR = 0.99f;//constant by wich we multiply upvector (for strafing allign) every update to reduce its influence 
        private const float MAX_SHIP_WEIGHT = 1000000.0f;

        // empirically calculated value for conversion of max speed into deceleration multiplier
        public const int MAGIC_CONSTANT_FOR_MAX_SPEED = 503;
        public static float SHIP_ROTATION_SENSITIVITY_MODIFIER = 1.075f;

        #endregion

        public enum ReflectorTypeEnum
        {
            Single,
            Dual,
        }

        #region Fields

        public float PilotHealth = PilotMaxHealth;
        public const float PilotMaxHealth = 100f;

        public Matrix PlayerHeadForCockpitInteriorWorldMatrix = Matrix.Identity;
        public Matrix PlayerHeadForGunsWorldMatrix = Matrix.Identity;
        public static bool DebugDrawEngineThrusts = false;
        public static bool DebugDrawEngineWeapons = false;
        public bool DebugDrawSecondaryCameraBoundingFrustum = false;
        //public bool IsDummy;

        private bool m_engineAfterburnerOn;
        private bool m_engineAfterburnerCanRefill;
        public bool AfterburnerEnabled { get; set; }
        private int m_engineIdleCueDelayMillis;
        MySoundCue? m_engineIdleCue;
        MySoundCue? m_engineOnOffSound;
        MySoundCue? m_engineMovingCue;
        MySoundCue? m_engineAfterburnerCue;
        float m_engineVolume;
        private MyLight m_light;                 //  Dynamic light used for reflector or muzzle flash
        public MyEntity OwnerEntity;
        public uint? OwnerId;

        private float m_reflectorShadowDistance = MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE;
        public float ReflectorShadowDistance
        {
            get
            {
                return m_reflectorShadowDistance;
            }
            set
            {
                m_reflectorShadowDistance = value;
                if (m_light != null)
                {
                    m_light.ShadowDistanceSquared = m_reflectorShadowDistance * m_reflectorShadowDistance;
                }
            }
        }

        //For tweaking light
        public static float LightRangeMultiplier = 1.0f;
        public static float LightIntensityMultiplier = 1.0f;
        public static float ReflectorIntensityMultiplier = 2.0f;

        public static float LightOffsetX = 0;
        public static float LightOffsetY = 0;
        public static float LightOffsetZ = 0;

        private ReflectorTypeEnum m_reflectorType = ReflectorTypeEnum.Dual;
        private MyGroupMask m_groupMask; // This is only mask allocated to entity, not all masks
        MySoundCue? m_playerBreathing;
        protected Matrix m_levelingMatrix;
        bool m_hasCollided = false;
        float m_collisionFriction = 0.0f;

        protected bool m_includeCargoWeight = false;
        //Matrix m_zedAxisRotationState = 0.0f;// rotation about Z axis of ship... to evoke roll status// will be later upgraded to statematrix

        // group mask for collision blocker

        //  Weapons sounds are unified per miner ship (two or more weapons of same type and shooting simultanously will produce only one sound)
        MySoundCue?[] m_unifiedWeaponCues;

        MyCockpit m_cockpit;
        MyCockpitGlassEntity m_glassIdealPhysObject;

        //  m_initialSunWindPosition of player head inside the cockpit moved in response to acceleration and deceleration.
        //  This coordinates are local, therefor in this object space
        Vector3 m_playerHeadLocal;
        Vector3 m_playerHeadExtraShake;
        Vector3 m_playerViewAngleLocal;
        Vector3 m_playerViewAngleExtraShake;

        MyCameraHeadShake m_shaker;
        MyCameraSpring m_cameraSpring;

        public MyCameraHeadShake GetShake() { return m_shaker; }
        public MyCameraSpring GetCameraSpring() { return m_cameraSpring; }
        public Vector3 GetHeadPosition() { return m_playerHeadLocal; }
        public Vector3 GetHeadDirection() { return m_playerViewAngleLocal; }
        public float GetFuelPercentage() { return Fuel / MaxFuel; }

        private MyMissile m_lastMissileFired;

        public MyMissile LastMissileFired
        {
            get { return m_lastMissileFired; }
            set
            {
                if (m_lastMissileFired != null)
                {
                    m_lastMissileFired.OnClose -= LastMissileFiredOnClose;
                }

                value.OnClose += LastMissileFiredOnClose;

                m_lastMissileFired = value;

                if (this == MyGuiScreenGamePlay.Static.ControlledEntity)
                {
                    MySecondaryCamera.Instance.SetEntityCamera(LastMissileFired);
                    MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Missile;
                }
            }
        }

        void LastMissileFiredOnClose(MyEntity missile)
        {
            m_lastMissileFired.OnClose -= LastMissileFiredOnClose;
            m_lastMissileFired = null;
        }

        bool m_engineSoundOn;
        MyPrefabHangar m_nearMotherShipContainer;

        /// <summary>
        /// Time in seconds to PumpFuel()
        /// </summary>
        float m_timeToPumpFuel;

        //  Parameters for controling the ship

        MyObjectsPool<MyModelShipSubOject> m_modelSubObjectsPool = new MyObjectsPool<MyModelShipSubOject>(MAX_SHIP_SUBOBJECTS);
        List<MyModelShipSubOject> m_subObjectEngineThrustBackwardSmallLeftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackwardSmallRightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackwardLeftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackwardRightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackward2Leftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackward2Rightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustBackward2Middle = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustForwardLeftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustForwardRightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustStrafeLeft = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustStrafeRight = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustUpLeftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustUpRightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustDownLeftside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectEngineThrustDownRightside = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectReflectorLeft = new List<MyModelShipSubOject>();
        List<MyModelShipSubOject> m_subObjectReflectorRight = new List<MyModelShipSubOject>();
        MyModelShipSubOject m_subObjectLight;
        MyModelShipSubOject m_subObjectPlayerHeadForCockpitInteriorTranslation;

        // engine thrust particle effects
        private MyParticleEffect m_damageEffect;
        private Matrix? m_damageEffectLocalMatrix;

        List<MyModelShipSubOject> m_subObjects = new List<MyModelShipSubOject>();

        float m_lockTargetCheckTime;
        protected MyReflectorConfig m_reflectorProperies;

        protected Vector3 m_engineForce = Vector3.Zero;
        protected Vector3 m_engineTorque = Vector3.Zero;

        protected MyShipTypeProperties m_shipTypeProperties;
        protected MySmallShipEngineTypeProperties m_engineTypeProperties;
        protected MySmallShipArmorTypeProperties? m_armorProperties;

        private int m_waypointTicks;

        private bool m_hasRadarJammer = false;
        private MyNanoRepairToolEntity m_nanoRepairTool;

        // Gameplay fields
        public float Weight { get; set; }
        public float MaxFuel { get; set; }
        public float MaxOxygen { get; set; }

        public float MaxArmorHealth { get; set; }

        // time from last detecting another ship to trade
        private float m_lastShipDetecting = 0;
        private MyHudNotification.MyNotification m_tradeNotification = null;
        private MyHudNotification.MyNotification m_buildNotification = null;
        private MyHudNotification.MyNotification m_travelNotification = null;
        private MyHudNotification.MyNotification m_securityControlHUBNotification = null;

        // time from last checking for mission proximity
        private float m_lastMissionProximityCheck = 0.1f * MySmallShipConstants.DETECT_INTERVAL;

        /// <summary>
        /// Active radar devices
        /// </summary>
        private List<MyMwcObjectBuilder_SmallShip_Radar_TypesEnum> RadarCapabilities;

        private int m_enemiesCountDetectedLastUpdate = 0;

        private List<MyInventoryItem> m_helperInventoryItems = new List<MyInventoryItem>();

        private int m_lastAfterburnerFilling;
        private int m_lastAfterburnerEmptying;
        public float AfterburnerStatus = 0.0f;


        private float m_lastEnemyDetection = -60000;

        private float m_angularDampingDefault = 0.0f;
        private float m_angularDampingSlowdown = 0.8f;

        /// <summary>
        /// In milliseconds.
        /// </summary>
        float m_disabledByEMPDuration;

        private Dictionary<uint, MyDamageOverTimeItem> m_damageOverTimeList = new Dictionary<uint, MyDamageOverTimeItem>();
        private static List<uint> m_damageOverTimeRemoveBuffer = new List<uint>(5);
        private float m_friendlyFireDamageTimer;

        //MWBuilder fields
        int m_stepStartMS;
        int m_stepDurationMS = 1000;


        #endregion

        #region Remoting Properties
        /// <summary>
        /// Dirt level of cockpit glass, in interval 0..1
        /// </summary>
        public float GlassDirtLevel { get; private set; }

        /// <summary>
        /// Mounted Engine
        /// </summary>
        MyMwcObjectBuilder_SmallShip_Engine m_engine;

        public MyCubeBuilder CubeBuilder { get; private set; }

        /// <summary>
        /// Mounted Engine
        /// </summary>
        public MyMwcObjectBuilder_SmallShip_Engine Engine
        {
            get
            {
                return m_engine;
            }
            set
            {
                if (m_engine != value)
                {
                    m_engineChanged = true;
                }
                m_engine = value;
                m_engineTypeProperties = value != null ? MySmallShipEngineTypeConstants.GetProperties(value.EngineType) : MySmallShipEngineTypeConstants.ShipWithoutEngineProperties;
            }
        }

        private bool m_engineChanged = false;

        /// <summary>
        /// Mounted Armor
        /// </summary>
        MyMwcObjectBuilder_SmallShip_Armor m_armor;

        /// <summary>
        /// Mounted Armor
        /// </summary>
        public MyMwcObjectBuilder_SmallShip_Armor Armor
        {
            get
            {
                return m_armor;
            }
            set
            {
                m_armor = value;
                OnArmorChanged();
            }
        }

        public bool HasSunWindArmor
        {
            get
            {
                return Armor != null && Armor.ArmorType == MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind;
            }
        }

        float m_armorHealth;
        public float ArmorHealth
        {
            get
            {
                return m_armorHealth;
            }
            set
            {
                m_armorHealth = value;
            }
        }

        float m_oxygen;

        public float Oxygen
        {
            get
            {
                return m_oxygen;
            }
            set
            {
                m_oxygen = value;
            }
        }

        public float PlayerBloodTextureOpacity
        {
            get
            {
                if (MySession.Static != null && MySession.Static.Player != null)
                {
                    float playerHealth = MathHelper.Clamp(MySession.Static.Player.Health / MySession.Static.Player.MaxHealth * 100, 0, 100);
                    if (playerHealth > 40)
                    {
                        return 0;
                    }
                    else if (playerHealth > 0)
                    {
                        return (40f - playerHealth) / 40f;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public float NoOxygenTextureOpacity
        {
            get
            {
                return MathHelper.Clamp(MySession.Static.Player.TimeWithoutOxygen / 30f, 0f, 1f);
            }
        }

        public float Fuel { get; set; }


        MyMwcObjectBuilder_SmallShip_TypesEnum m_shipType;

        public MyMwcObjectBuilder_SmallShip_TypesEnum ShipType
        {
            get
            {
                return m_shipType;
            }
            set
            {
                m_shipType = value;
                m_shipTypeProperties = MyShipTypeConstants.GetShipTypeProperties(value);
            }
        }

        private MySmallShipWeapons m_smallShipWeapons;

        /// <summary>
        /// Gets the weapons.
        /// </summary>
        /// TODO: This also should be entity.        
        public MySmallShipWeapons Weapons
        {
            get
            {
                return m_smallShipWeapons;
            }
            protected set
            {
                m_smallShipWeapons = value;
            }
        }
        #endregion

        #region Properties

        public MyPlayerBase Player;

        /// <summary>
        /// Gets the config.
        /// </summary>
        public MySmallShipConfig Config { get; private set; }

        /// <summary>
        /// Gets the group mask.
        /// </summary>
        public MyGroupMask GroupMask
        {
            get
            {
                return m_groupMask;
            }
        }

        /// <summary>
        /// Gets the target entity.
        /// </summary>
        public MyEntity TargetEntity { get; /*private */set; }
        public List<MyEntity> SideTargets = new List<MyEntity>();

        /// <summary>
        /// List of acquired targets
        /// </summary>
        private List<MyEntity> m_targetEntityHistory;

        /// <summary>
        /// Gets the cockpit glass model enum.
        /// </summary>
        public MyModelsEnum CockpitGlassModelEnum { get; private set; }

        /// <summary>
        /// Gets the cockpit interior model enum.
        /// </summary>
        public MyModelsEnum CockpitInteriorModelEnum { get; private set; }

        /// <summary>
        /// Gets the light.
        /// </summary>
        public MyLight Light
        {
            get { return m_light; }
        }

        /// <summary>
        /// fire weapons 
        /// </summary>
        /// <param name="en"></param>
        public void InitGroupMaskIfNeeded()
        {
            // prevent collision with ship
            if (m_groupMask.Equals(MyGroupMask.Empty))
            {
                bool retVal = MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().GetGroupMask(ref m_groupMask);
                MyCommonDebugUtils.AssertDebug(retVal);
                this.Physics.GroupMask |= this.GroupMask;
            }
        }

        /// <summary>
        /// Gets the ship type properties.
        /// </summary>
        public MyShipTypeProperties ShipTypeProperties
        {
            get { return m_shipTypeProperties; }
        }

        /// <summary>
        /// Memorizing players flight
        /// </summary>
        public MyPositionMemory RouteMemory { get; protected set; }

        ///// <summary>
        ///// Another ships detector
        ///// </summary>
        //public MyShipDetector ShipDetector { get; private set; }

        public MyEntityDetector TradeDetector { get; private set; }

        public MyEntityDetector BuildDetector { get; private set; }

        public MyEntityDetector MotherShipDetector { get; private set; }

        public MyEntityDetector UseableEntityDetector { get; protected set; }

        public MyHackingTool HackingTool { get; protected set; }

        /// <summary>
        /// Ship's radar
        /// </summary>
        private List<IMyObjectToDetect> m_detectedObjects;

        /// <summary>
        /// Tool kits
        /// </summary>
        private MyToolKits ToolKits { get; set; }

        public float DeadTime { get; set; }

        public override bool NearFlag
        {
            get
            {
                return false;
            }
            set
            {
                //base.NearFlag = value;
            }
        }

        public MyEntity Leader = null;

        public List<MySmallShipBot> Followers = new List<MySmallShipBot>();
        public static List<MySmallShipBot> FollowersBuffer = new List<MySmallShipBot>(10);

        float m_pilotDeathTime;

        bool m_isLooted;

        float m_lastLootTime;

        public float RadioactivityAmount;

        #endregion

        public event Action<MySmallShip> ConfigChanged;

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MySmallShip"/> class.
        /// </summary>
        public MySmallShip()
        {
            FalseFactions = MyMwcObjectBuilder_FactionEnum.None;
            this.m_includeCargoWeight = MySmallShipConstants.INCLUDE_CARGO_WEIGHT;
            this.Config = new MySmallShipConfig(this);
            this.Config.ConfigChanged += new Action<MySmallShipConfig>(Config_ConfigChanged);
            m_targetEntityHistory = new List<MyEntity>();

            RadarCapabilities = new List<MyMwcObjectBuilder_SmallShip_Radar_TypesEnum>();

            ShowOnHudOnlyWhenDetected = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySmallShip"/> class.
        /// </summary>
        /// <param name="health">The health.</param>
        /// <param name="displayName">The name.</param>
        public MySmallShip(float health, string displayName)
            : this()
        {
            this.DisplayName = displayName;
        }

        void Config_ConfigChanged(MySmallShipConfig obj)
        {
            RaiseConfigChanged();
        }

        void RaiseConfigChanged()
        {
            var handler = ConfigChanged;
            if (handler != null)
            {
                handler(this);
            }
        }

        void UpdateSubObjectsState(bool cameraInCockpit)
        {
            Debug.Assert(this == MySession.PlayerShip);

            EnableThrustLights(!cameraInCockpit);
            UpdateWeaponsNear(cameraInCockpit);
        }

        private void UpdateWeaponsNear(bool inCockpit)
        {
            foreach (var weapon in Weapons.GetMountedWeaponsWithHarvesterAndDrill())
            {
                weapon.NearFlag = inCockpit;
            }
        }

        private void EnableThrustLights(bool enable)
        {
            EnableThrustLights(m_subObjectEngineThrustBackwardSmallLeftside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackwardSmallRightside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackwardLeftside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackwardRightside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackward2Leftside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackward2Rightside, enable);
            EnableThrustLights(m_subObjectEngineThrustBackward2Middle, enable);
            EnableThrustLights(m_subObjectEngineThrustForwardLeftside, enable);
            EnableThrustLights(m_subObjectEngineThrustForwardRightside, enable);
            EnableThrustLights(m_subObjectEngineThrustStrafeLeft, enable);
            EnableThrustLights(m_subObjectEngineThrustStrafeRight, enable);
            EnableThrustLights(m_subObjectEngineThrustUpLeftside, enable);
            EnableThrustLights(m_subObjectEngineThrustUpRightside, enable);
            EnableThrustLights(m_subObjectEngineThrustDownLeftside, enable);
            EnableThrustLights(m_subObjectEngineThrustDownRightside, enable);
        }

        private void EnableThrustLights(List<MyModelShipSubOject> subobjectGroup, bool enable)
        {
            foreach (MyModelShipSubOject subObject in subobjectGroup)
            {
                if (subObject.Light != null)
                {
                    subObject.Light.LightOn = enable;
                }
            }
        }

        public override void Init(string displayName, MyMwcObjectBuilder_Ship shipObjectBuilder)
        {
            MyMwcObjectBuilder_SmallShip smallShipObjectBuilder = shipObjectBuilder as MyMwcObjectBuilder_SmallShip;
            Debug.Assert(smallShipObjectBuilder != null);

            m_unifiedWeaponCues = new MySoundCue?[MyAudio.GetNumberOfSounds()];

            MaxArmorHealth = 100.0f;

            bool isPlayerShip = smallShipObjectBuilder is MyMwcObjectBuilder_SmallShip_Player;

            if (isPlayerShip)
            {
                Save = false; // Do not save player ships
                m_waypointTicks = 0;
            }

            OwnerId = smallShipObjectBuilder.OwnerId;

            RouteMemory = new MyPositionMemory(MySmallShipConstants.POSITION_MEMORY_SIZE, 1);

            Name = shipObjectBuilder.Name;

            //set leveling matrix default 
            m_levelingMatrix = Matrix.Identity;

            m_engineSoundOn = true;

            StringBuilder hudLabel = null;
            if (displayName != null)
            {
                hudLabel = new StringBuilder(displayName);
            }
            InitShip(hudLabel, smallShipObjectBuilder, null);

            MyModelDummy dummy;
            if (GetModelLod0().Dummies.TryGetValue("destruction", out dummy))
            {
                m_damageEffectLocalMatrix = dummy.Matrix;
            }
            if (shipObjectBuilder.Inventory == null)
            {
                shipObjectBuilder.Inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), m_shipTypeProperties.GamePlay.CargoCapacity);
            }
            shipObjectBuilder.Inventory.MaxItems = m_shipTypeProperties.GamePlay.CargoCapacity;
            if (isPlayerShip)
            {
                if (MyGameplayCheats.IsCheatEnabled(MyGameplayCheatsEnum.INCREASE_CARGO_CAPACITY))
                {
                    shipObjectBuilder.Inventory.MaxItems = MyGamePlayCheatsConstants.CHEAT_INCREASE_CARGO_CAPACITY_MAX_ITEMS;
                }
                //shipObjectBuilder.Inventory.UnlimitedCapacity = MyGameplayCheats.IsCheatEnabled(MyGameplayCheatsEnum.UNLIMITED_CARGO_CAPACITY);
            }
            base.Init(displayName, shipObjectBuilder);

            if (isPlayerShip)
            {
                Inventory.InventorySynchronizer = new MyInventorySynchronizer(Inventory, MyPlayerShipInventorySynchronizerHelper.MustBePlayerShipInventorySynchronized);
            }

            System.Diagnostics.Debug.Assert(Faction != 0);
            m_targetEntityHistory.Clear();

            m_pilotDeathTime = 0;
            m_lastLootTime = 0;
            m_isExploded = false;
            m_lastTimeDeployedDrone = MyConstants.FAREST_TIME_IN_PAST;

            RenderObjects[0].ShadowCastUpdateInterval = 600;

            MyGuiScreenInventoryManagerForGame.OpeningInventoryScreen += OpeningInventoryScreen;
            MyGuiScreenInventoryManagerForGame.InventoryScreenClosed += InventoryScreenClosed;
            AfterburnerStatus = 1.0f;
            AfterburnerEnabled = true;
            IsDummy = smallShipObjectBuilder.IsDummy;
            m_friendlyFireDamageTimer = 0;
            AIPriority = smallShipObjectBuilder.AIPriority;

            UpdateMaxFuel();
            UpdateMaxOxygen();
        }

        public override void Link()
        {
            base.Link();

            if (OwnerId.HasValue)
            {
                if (OwnerId == uint.MaxValue)
                {
                    this.OwnerEntity = MySession.PlayerShip;
                }
                else
                {
                    this.OwnerEntity = MyEntities.GetEntityById(new MyEntityIdentifier(OwnerId.Value));
                }
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base baseBuilder = base.GetObjectBuilderInternal(getExactCopy);
            var builder = baseBuilder as MyMwcObjectBuilder_SmallShip;
            if (builder != null)
            {
                builder.ShipType = m_shipType;
                //builder.Weapons = Weapons != null ? Weapons.GetWeaponsObjectBuilders(getExactCopy) : null;
                builder.Weapons = Weapons.GetWeaponsObjectBuilders(getExactCopy);
                builder.Engine = Engine;
                builder.AssignmentOfAmmo = Weapons.AmmoAssignments.GetObjectBuilder();
                builder.Armor = Armor;
                builder.Radar = new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1);

                //if (MaxHealth == m_gameplayProperties.MaxHealth)
                //    builder.ShipMaxHealth = MyGameplayConstants.MAX_HEALTH_MAX;
                //else
                //    builder.ShipMaxHealth = MaxHealth;

                //if (Health == m_gameplayProperties.MaxHealth)
                //    builder.ShipHealthRatio = MyGameplayConstants.HEALTH_MAX;
                //else
                //    builder.ShipHealthRatio = Health;
                builder.ShipHealthRatio = HealthRatio;
                builder.ShipMaxHealth = GetMaxHealth();

                builder.ArmorHealth = ArmorHealth;
                builder.Oxygen = Oxygen;
                builder.Fuel = Fuel;
                builder.DisplayName = DisplayName;
                builder.ReflectorLight = Config.ReflectorLight.On;
                builder.ReflectorLongRange = Config.ReflectorLongRange.On;
                builder.ReflectorShadowDistance = ReflectorShadowDistance;
                builder.AIPriority = AIPriority;

                if (OwnerEntity != null)
                {
                    if (OwnerEntity == MySession.PlayerShip)
                    {
                        builder.OwnerId = uint.MaxValue;
                    }
                    else
                    {
                        builder.OwnerId = OwnerEntity.EntityId.Value.NumericValue;
                    }
                }


                return builder;
            }
            return baseBuilder;
        }

        public static MyMwcObjectBuilder_SmallShip CreateDefaultSmallShipObjectBuilder(Matrix matrix)
        {
            MyMwcObjectBuilder_SmallShip objectBuilder = new MyMwcObjectBuilder_SmallShip(
                MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                null,
                new List<MyMwcObjectBuilder_SmallShip_Weapon>(),
                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                new List<MyMwcObjectBuilder_AssignmentOfAmmo>(),
                new MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic),
                new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                MyGameplayConstants.MAXHEALTH_SMALLSHIP, MyGameplayConstants.HEALTH_RATIO_MAX, float.MaxValue, float.MaxValue, float.MaxValue, true, false, MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0);
            objectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
            return objectBuilder;
        }

        /// <summary>
        /// Damage is first taken from armor, at full rate. The amount taken from armor is then multiplied by armor
        /// ratio to get how much damage armor absorbed. The rest is taken from ship health.
        /// </summary>
        /// <param name="healthDamage">Damage which should be taken from player health</param>
        /// <param name="damage">Damage to be taken from ship/entity</param>
        /// <param name="damageType"></param>
        /// <param name="ammoType"></param>
        /// <param name="damageSource"></param>
        protected override void DoDamageInternal(float healthDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            if (HasSunWindArmor && damageType == MyDamageType.Sunwind)
            {
                return;
            }

            HandleAttack(damageSource);

            // Mantis 5877: There will be "friendly fire" voice notification, but nobody will get hurt.
            // It will work only on small ships.

            if (damageSource is MySmallShip && damageSource != this)
            {
                var factionRelation = MyFactions.GetFactionsRelation(this, damageSource);
                if (factionRelation == MyFactionRelationEnum.Friend || factionRelation == MyFactionRelationEnum.Neutral)
                {
                    // Report friendly fire on friendly units, when too much damage over time is done, friends will aggro
                    if (damageSource == MySession.PlayerShip && (this.PersistentFlags & MyPersistentEntityFlags.Parked) == 0)
                    {
                        MySession.PlayerShip.AddFriendlyFireDamage(this, damage);
                    }

                    healthDamage = damage = empDamage = 0;
                }
            }
            
            // When ship damages itself and it's player ship, don't do damage
            if (damageSource == this && damageSource == MySession.PlayerShip)
            {
                healthDamage = damage = empDamage = 0;
            }

            bool isPlayerShip = this == MySession.PlayerShip;

#if RENDER_PROFILING
            if (isPlayerShip)
                return;
#endif
            // Invincibility for player in editor or when profiling
            if (isPlayerShip && MyGuiScreenGamePlay.Static.IsEditorActive())
            {
                return;
            }


            if (isPlayerShip && (MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.PLAYER_SHIP_INDESTRUCTIBLE)))
            {
                damage = empDamage = 0;
            }

            if (MyFakes.TEST_MISSION_GAMEPLAY && (damage != 10000 || damageType != MyDamageType.Explosion))
            {
                damage = empDamage = healthDamage = 0;
            }

            if (isPlayerShip)
            {
                if (damageType != MyDamageType.Unknown)
                {
                    damage *= MyGameplayConstants.GameplayDifficultyProfile.DamageToPlayerMultiplicator;
                }
            }

            //how much armor absorbs damage (0.0 no absorbtion, 1.0 full absorbtion)
            float armorReduction = m_armorProperties.HasValue ? m_armorProperties.Value.Resistance : 0;

            if (ammoType == MyAmmoType.Piercing)
            {
                armorReduction *= MyAmmoConstants.ArmorEffectivityVsPiercingAmmo; // Armor effectivity is lower against piercing ammo
            }

            //actual damage armor absorbed
            float armorDamage = Math.Min(ArmorHealth, damage);

            //update armor health value
            ArmorHealth -= armorDamage;

            //calculate actual damage to the ship (decrease it by the absorbed damage by armor)
            float shipDamage = damage - armorDamage * armorReduction;


            if (this == MySession.PlayerShip)
            {
                if (MySession.Static.Player.Medicines[(int)MyMedicineType.HEALTH_ENHANCING_MEDICINE].IsActive())
                    healthDamage *= MyMedicineConstants.HEALTH_ENHANCING_MEDICINE_DAMAGE_MULTIPLIER;

                if (damageType == MyDamageType.Radioactivity || damageType == MyDamageType.Sunwind)
                {
                    // activate if not active
                    if (!MySession.Static.Player.Medicines[(int)MyMedicineType.ANTIRADIATION_MEDICINE].IsActive())
                    {
                        MySession.Static.Player.Medicines[(int)MyMedicineType.ANTIRADIATION_MEDICINE].ActivateIfInInventory(Inventory);
                    }
                    // if active, modify health damage
                    if (MySession.Static.Player.Medicines[(int)MyMedicineType.ANTIRADIATION_MEDICINE].IsActive())
                    {
                        healthDamage *= MyMedicineConstants.ANTIRADIATION_MEDICINE_RADIATION_DAMAGE_MULTIPLIER;
                    }
                }

                if (damageSource == null || (damageSource != this && MyFactions.GetFactionsRelation(damageSource, this) == MyFactionRelationEnum.Enemy))
                {
                    healthDamage *= MyGameplayConstants.GameplayDifficultyProfile.DamageToPlayerFromEnemyMultiplicator;
                    shipDamage *= MyGameplayConstants.GameplayDifficultyProfile.DamageToPlayerFromEnemyMultiplicator;
                    empDamage *= MyGameplayConstants.GameplayDifficultyProfile.DamageToPlayerFromEnemyMultiplicator;
                }

                if (MySession.Static != null)
                {
                    if (!MySession.Static.Player.IsDead())
                    {
                        MySession.Static.Player.AddHealth(-healthDamage, damageSource);
                    }
                }

                if (damageType == MyDamageType.Radioactivity || damageType == MyDamageType.Sunwind)
                {
                    m_radiationLastDamage = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    //PlayGeigerBeeping();
                    if (healthDamage >= MySmallShipConstants.WARNING_RADIATION_DAMAGE_PER_SECOND * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS)
                    {
                        m_radiationCriticalLastDamage = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        //PlayRadiationWarning();
                    }
                }
            }

            DisableByEMP(TimeSpan.FromSeconds(MySmallShipConstants.EMP_SMALLSHIP_DISABLE_DURATION_MULTIPLIER * empDamage));

            if (isPlayerShip)
            {
                if (damageSource != null)
                {
                    MyHud.SetDamageIndicator(damageSource, damageSource.GetPosition(), shipDamage);
                }
            }

            base.DoDamageInternal(healthDamage, shipDamage, empDamage, damageType, ammoType, damageSource, justDeactivate);

            UpdateDamageEffect();
        }

        public override void AddHealth(float deltaHealth)
        {
            float oldHealth = Health;

            base.AddHealth(deltaHealth);

            if (oldHealth != Health)
            {
                UpdateDamageEffect();
            }
        }

        private void UpdateDamageEffect()
        {
            var damageRatio = GetDamageRatio();
            bool threshold = damageRatio > MyShipConstants.DAMAGED_HEALTH;

            if (m_damageEffect == null && threshold)
            {
                m_damageEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_Smoke);
                m_damageEffect.AutoDelete = false;
                m_damageEffect.UserBirthMultiplier = 0;
                m_damageEffect.WorldMatrix = m_damageEffectLocalMatrix != null ? m_damageEffectLocalMatrix.Value * WorldMatrix : WorldMatrix;
            }
        }

        //  We don't need to optimize this value in a member because it's retrieved only once per update or draw and only for player's miner ship
        //  It we also shake the camera/hear. As this method is called only for player ship (not every mining ship), it doesn't have to be super optimized.
        //  Moreover, this method is called from Update, not from Draw.
        public Matrix GetViewMatrix()
        {
            Matrix matrixRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, m_playerViewAngleLocal.Z) *
                Matrix.CreateFromAxisAngle(Vector3.Right, m_playerViewAngleLocal.X);
            Matrix rboRot = WorldMatrix;
            Matrix worldMatrix = WorldMatrix;
            rboRot.Translation = Vector3.Zero;
            return Matrix.Invert(rboRot * matrixRotation * Matrix.CreateTranslation(MyUtils.GetTransform(m_playerHeadLocal, ref worldMatrix)));
        }

        public override float GetHUDDamageRatio()
        {
            return 1f - (HealthRatio);
        }

        /// <summary>
        /// Only for multiplayer
        /// </summary>
        public void SetAfterburner(bool enabled)
        {
            if (!IsDummy)
            {
                return;
            }
            m_engineAfterburnerOn = enabled;
        }

        //  Prepares engine forces but don't apply them to physic object. That is done in UpdateController()
        //  Reason is that JLX clears all forces before Integration Step and then calls UpdateController() for every enabled controller.
        //  So that's only place where we can really alter physic objects.
        //  This method aceepts float rollIndicator, which is used to control the rolling rotation
        public virtual void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator, bool afterburner)
        {
            MyUtils.AssertIsValid(moveIndicator);
            MyUtils.AssertIsValid(rotationIndicator);
            MyUtils.AssertIsValid(rollIndicator);

            if (MySession.Is25DSector)
            {
                if (!(this is MySmallShipBot))
                {
                    rotationIndicator.X = 0;
                    moveIndicator.Y = 0;
                    rollIndicator = 0;
                }
            }

            if (MyFakes.MWBUILDER)
            {         /*
                if (!(this is MySmallShipBot))
                {
                    //rotationIndicator.X = 0;
                    rollIndicator = 0;
                }   

                if (moveIndicator.LengthSquared() > 0.1f && (MyMinerGame.TotalTimeInMilliseconds - m_stepStartMS > m_stepDurationMS))
                {
                    m_stepStartMS = MyMinerGame.TotalTimeInMilliseconds;
                }   */
            }

            rotationIndicator *= SHIP_ROTATION_SENSITIVITY_MODIFIER;

            var zoomSensitivityModifier = (1 - SENSITIVITY_MODIFIER_FOR_MAX_ZOOM) * MyCamera.Zoom.GetZoomLevel() +
                                          SENSITIVITY_MODIFIER_FOR_MAX_ZOOM;
            rotationIndicator *= zoomSensitivityModifier;

            var activeEngineProps = GetEngineProperties();
            //// Ship without engine, fuel, electricity can't move
            //if (Fuel <= 0 || Electricity <= 0 || Engine != null)
            //{
            //    moveIndicator *= 0.0f;
            //    rotationIndicator *= 0.0f;
            //    rollIndicator *= 0.0f;
            //}

            //  If drill is currently active then ship should move and rotate slowly
            if (m_smallShipWeapons != null)
            {
                var drill = m_smallShipWeapons.GetMountedDrill();
                if (drill != null)
                {
                    if (!MySession.Is25DSector && drill.CurrentState != MyDrillStateEnum.InsideShip)
                    {
                        moveIndicator *= 0.25f;
                        rotationIndicator *= 0.25f;
                        rollIndicator *= 0.25f;
                    }
                    else if (drill.CurrentState == MyDrillStateEnum.Drilling)
                    {
                        moveIndicator *= 0.25f;
                        rotationIndicator *= 0.25f;
                        rollIndicator *= 0.25f;
                    }
                }
            }

            var newAfterburnerValue = afterburner && AfterburnerStatus > 0 && moveIndicator.Z < 0 && Fuel > 0 && IsEngineWorking() && Engine != null && AfterburnerEnabled;
            if (MyMultiplayerGameplay.IsRunning && newAfterburnerValue != m_engineAfterburnerOn && this == MySession.PlayerShip)
            {
                MyMultiplayerGameplay.Static.Afterburner(newAfterburnerValue);
            }
            m_engineAfterburnerOn = newAfterburnerValue;

            // prevent afterburner usage after depletion (you have to release Shift to be able to use it again)
            // engine off: always refill
            m_engineAfterburnerCanRefill = !afterburner || moveIndicator.Z >= 0 || !Config.Engine.On;

            var physicsProperties = GetPhysicsProperties();

            if (m_engineAfterburnerOn)
            {
                rotationIndicator *= 0.25f;
                rollIndicator *= 0.25f;
                moveIndicator.X *= 0.25f;
                moveIndicator.Y *= 0.25f;
                moveIndicator.Z *= activeEngineProps.AfterburnerSpeedMultiplier;
            }
            else
            {
                moveIndicator *= 1.2f;
            }

            // no movement: no change
            if (moveIndicator == Vector3.Zero && rotationIndicator == Vector2.Zero && rollIndicator == 0.0f)
                return;

            Matrix tempInvertedWorldMatrix = GetWorldMatrixInverted();
            tempInvertedWorldMatrix.Translation = Vector3.Zero;

            Vector3 localWorldAxisY = Vector3.Zero;
            Vector3 localWorldAxisX = Vector3.Zero;
            if (Config.AutoLeveling.On)
            {
                Vector3 vec = new Vector3(WorldMatrix.Right.X, 0, WorldMatrix.Right.Z);
                if (MyMwcUtils.HasValidLength(vec))
                {
                    localWorldAxisY = MyUtils.GetTransformNormal(m_levelingMatrix.Up, ref tempInvertedWorldMatrix);
                    localWorldAxisX = MyUtils.GetTransformNormal(MyMwcUtils.Normalize(vec), ref tempInvertedWorldMatrix);
                }
                else
                {
                    localWorldAxisY = MyUtils.GetTransformNormal(WorldMatrix.Up, ref tempInvertedWorldMatrix);
                    localWorldAxisX = MyUtils.GetTransformNormal(WorldMatrix.Right, ref tempInvertedWorldMatrix);
                }

            }
            else //  Without autolevel, dont need to transform local x and y axis based on up right orientation
            {
                if (rollIndicator != 0 && IsEngineWorking()) // this is the roll around the forward vector(z)
                {
                    m_engineTorque.Z -= rollIndicator * physicsProperties.MultiplierRoll * Physics.Mass * physicsProperties.MultiplierHorizontalAngleStabilization;
                }

                localWorldAxisY = MyUtils.GetTransformNormal(WorldMatrix.Up, ref tempInvertedWorldMatrix);
                localWorldAxisX = MyUtils.GetTransformNormal(WorldMatrix.Right, ref tempInvertedWorldMatrix);
            }

            if (IsEngineWorking())
            {
                //  Forward/backward
                m_engineForce.Z += moveIndicator.Z * activeEngineProps.Force * physicsProperties.MultiplierForwardBackward * physicsProperties.MultiplierMovement;

                //  Strafe left/right
                m_engineForce += (localWorldAxisX * moveIndicator.X) * activeEngineProps.Force * physicsProperties.MultiplierStrafe * physicsProperties.MultiplierMovement;
                if (Config.AutoLeveling.On) // without auto level, this torque thing would make strafe too difficult
                {
                    m_engineTorque.Z -= moveIndicator.X * Physics.Mass * physicsProperties.MultiplierStrafeRotation * (MySession.Is25DSector && this == MySession.PlayerShip ? 10 : 1);
                    //m_engineTorque.Z -= moveIndicator.X * Physics.Mass * physicsProperties.MultiplierStrafeRotation * (MyFakes.MW25D && this == MySession.PlayerShip ? 1 : 1);

                    if (MySession.Static.Is2DSector && Math.Abs(WorldMatrix.Right.Y) < 0.01f)
                    {
                        //TODO: Sometimes destroy world matrix?
                        m_engineTorque.X += moveIndicator.Z * Physics.Mass * physicsProperties.MultiplierStrafeRotation * (MySession.Is25DSector && this == MySession.PlayerShip ? 40 : 0);
                    }
                }
              
                //  Up/down
                m_engineForce += moveIndicator.Y * localWorldAxisY * activeEngineProps.Force * physicsProperties.MultiplierUpDown * physicsProperties.MultiplierMovement;

                //  Rotate up/down
                m_engineTorque.X -= rotationIndicator.X * Physics.Mass * physicsProperties.MultiplierRotation;

                //  This rotation is just for effect (when rotating left/right, we rotate around front axis - it looks more natural, but it's not really needed for navigation).
                m_engineTorque.Z -= rotationIndicator.Y * Physics.Mass * physicsProperties.MultiplierRotationEffect * (MySession.Is25DSector && this == MySession.PlayerShip ? 5 : 1);
                //m_engineTorque.Z -= rotationIndicator.Y * Physics.Mass * physicsProperties.MultiplierRotationEffect * (MyFakes.MW25D && this == MySession.PlayerShip ? 1 : 1);

                //  Rotate around up vector (left/right)            
                m_engineTorque -= localWorldAxisY * rotationIndicator.Y * Physics.Mass * physicsProperties.MultiplierRotation;
            }

            MyUtils.AssertIsValid(m_engineTorque);
            MyUtils.AssertIsValid(m_engineForce);
        }

        /// <summary>
        /// Returns the rolling angle of the ship
        /// </summary>
        /// <param name="ship"></param>
        /// <returns></returns>
        public float RollingAngle(MySmallShip ship)
        {
            Vector3 plane;
            Vector3 plane2;

            //plane .. two vectors
            var v1 = ship.WorldMatrix.Right;
            var v2 = ship.WorldMatrix.Up;

            //second plane
            var p1 = ship.WorldMatrix.Forward;
            var p2 = Vector3.Up;

            //plane normal vector
            Vector3.Cross(ref v1, ref v2, out plane);
            Vector3.Cross(ref p1, ref p2, out plane2);

            Vector3 intersection;

            //help variables for next calculations
            float zF = (plane.Y * plane2.Z - plane2.Y * plane.Z);
            float yF = plane.Y;

            //if these variables are zero, we cant calculate intersection, so make it same as up vector
            if (zF != 0 && yF != 0)
            {
                //calculate intersection of two planes
                intersection.X = 1;
                intersection.Z = ((-plane.Y * plane2.X + plane2.Y * plane.X) / zF);
                intersection.Y = (-plane.X - plane.Z * intersection.Z) / yF;
                intersection = MyMwcUtils.Normalize(intersection);
            }
            else intersection = Vector3.Up;

            //Make sure that intersection will be in the same direction all the time
            if (MyUtils.GetAngleBetweenVectors(Vector3.Up, intersection) > 1.57f)
                intersection *= -1;

            var d = ship.WorldMatrix.Forward;

            //Debug draw of all used vectors for planes and intersection vector
            //MyDebugDraw.DrawLine3D(GetPosition(), GetPosition()+10*p2, Color.Gold, Color.Gold);
            //MyDebugDraw.DrawLine3D(GetPosition(), GetPosition() + 10 * p1, Color.Gold, Color.Gold);
            //MyDebugDraw.DrawLine3D(this.GetPosition(), GetPosition() + 10*v2, Color.Aquamarine, Color.Aquamarine);
            //MyDebugDraw.DrawLine3D(this.GetPosition(), GetPosition()+10*v1, Color.Aquamarine, Color.Aquamarine);
            //MyDebugDraw.DrawLine3D(this.GetPosition(), GetPosition() + 10 * intersection, Color.Fuchsia, Color.Fuchsia);

            var angleUp = MathHelper.ToDegrees(MyUtils.GetAngleBetweenVectors(intersection, v2));

            //direction of plane for differencing angles with same absolute value
            Vector3.Cross(ref v2, ref d, out d);

            //value of point in view of plane 
            float val = d.X * intersection.X + d.Y * intersection.Y + d.Z * intersection.Z;

            //rolling side
            if (Math.Sign(val) != 0)
                angleUp *= Math.Sign(val);

            return angleUp;
        }

        /// <summary>
        /// Adds new waypoint and deletes previous one if it is redundant
        /// </summary>
        private void AddRoutePoint()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship::AddRoutePoint");
            RouteMemory.Add(GetPosition());
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //public void CheckMemoryListBackwards(int dist)
        //{
        //    var count = RouteMemory.Count;
        //    if ((dist < 0) && (dist > -count))
        //    {
        //        if (null == CanSee(RouteMemory[count + dist]))
        //        {
        //            RouteMemory.RemoveRange(count + dist, -dist);
        //        }
        //    }
        //}

        /// <summary>
        /// Chceck if ship can see to target position.
        /// </summary>
        /// <param name="position">Target ship.</param>
        /// <returns>True, if bot can see ship.</returns>
        protected MyEntity CanSee(Vector3 position)
        {
            return MyEnemyTargeting.CanSee(this, position, null);
        }

        public MyEntity CanSee(MyEntity target)
        {
            return MyEnemyTargeting.CanSee(this, target.GetPosition(), target);
        }

        //  Slown down and stabilization of the ship

        public void UpdatePlayerHeadAndShakingNew()
        {
            m_cameraSpring.Update(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, GetWorldMatrixInverted(), ref m_playerHeadLocal);

            //  Extra camera shaking after shooting or explosions
            m_shaker.UpdateShake(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, ref m_playerHeadLocal, ref m_playerViewAngleLocal);

            if (m_shaker.ShakeActive())
            {
                this.Physics.WakeUp();
            }
        }

        //  Ammount is on interval <1....infinity>
        public void IncreaseHeadShake(float amount)
        {
            // Only shake head if this is player, because for other it doesn't make sence, no one can see that shake
            // Dont shake if player is in main menu
            if (this != MySession.PlayerShip ||
                (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.MAIN_MENU)) return;

            m_shaker.AddShake(amount);

            //  Shake the head after shooting
            m_playerHeadExtraShake.Y += amount * MyMwcUtils.GetRandomFloat(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.5f, MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 2);
            m_playerHeadExtraShake.Z += amount * MyMwcUtils.GetRandomFloat(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.5f, MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 2);
            //m_playerHeadExtraShake += MyMwcUtils.GetRandomVector3Normalized() * MyMwcUtils.GetRandomFloat(0.002f, 0.003f);

            //  Shake-rotate the view after shooting
            m_playerViewAngleExtraShake.Z += amount * MyMwcUtils.GetRandomSign() * MyMwcUtils.GetRandomFloat(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.5f, MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.6f);
            m_playerViewAngleExtraShake.X += amount * MyMwcUtils.GetRandomSign() * MyMwcUtils.GetRandomFloat(MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.5f, MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.6f);
        }

        //  Update light (position, color - reflector or muzzle flash)

        public Vector3 GetPlayerHeadForCockpitInterior()
        {
            Matrix worldMatrix = this.WorldMatrix;
            if (m_subObjectPlayerHeadForCockpitInteriorTranslation != null)
            {
                return MyUtils.GetTransform(m_subObjectPlayerHeadForCockpitInteriorTranslation.ModelSubObject.Position * -1, ref worldMatrix);
            }
            else
            {
                return worldMatrix.Translation;
            }
        }

        public void StopSounds(SharpDX.XACT3.StopFlags stopOptions = SharpDX.XACT3.StopFlags.Release)
        {

            if (m_engineIdleCue != null && m_engineIdleCue.Value.IsPlaying)
            {
                m_engineIdleCue.Value.Stop(stopOptions);
                m_engineIdleCue = null;
            }
            if (m_engineMovingCue != null && m_engineMovingCue.Value.IsPlaying)
            {
                m_engineMovingCue.Value.Stop(stopOptions);
                m_engineMovingCue = null;
            }
            if (m_playerBreathing != null && m_playerBreathing.Value.IsPlaying)
            {
                m_playerBreathing.Value.Stop(stopOptions);
                m_playerBreathing = null;
            }

        }

        protected override void SetHudMarker()
        {
            StringBuilder hudLabel = new StringBuilder();

            if (DisplayName != "Ship" && DisplayName != MyTextsWrapper.Get(MyTextsWrapperEnum.Ship).ToString())
            {
                string displayName = GetCorrectDisplayName();

                hudLabel.Append(displayName);
            }

            MyHud.ChangeText(this, hudLabel, null, MySmallShipConstants.MAX_HUD_DISTANCE, MyHudIndicatorFlagsEnum.SHOW_ALL);
        }

        public void SetEngineSound(bool isEngineOn)
        {
            m_engineOnOffSound = MyAudio.AddCue3D(isEngineOn ? GetEngineProperties().OnCue : GetEngineProperties().OffCue,
                                   GetPlayerHeadForCockpitInterior(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);

            m_engineSoundOn = isEngineOn;
            if (isEngineOn)
            {
                if ((m_playerBreathing != null) && (m_playerBreathing.Value.IsPlaying == true))
                {
                    m_playerBreathing.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }

                //engine idle sound can start only a moment after engine on sound plays, not immediately
                m_engineIdleCueDelayMillis = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyMinerShipConstants.MINER_SHIP_ENGINE_IDLE_CUE_DELAY_IN_MILLIS;
            }
            else
            {
                if (m_engineIdleCue != null && m_engineIdleCue.Value.IsPlaying)
                {
                    m_engineIdleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
                if (m_engineMovingCue != null && m_engineMovingCue.Value.IsPlaying)
                {
                    m_engineMovingCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
                m_playerBreathing = null;
            }
        }

        //  Weapons sounds are unified per miner ship (two or more weapons of same type and shooting simultanously will produce only one sound)
        public void UnifiedWeaponCueSet(MySoundCuesEnum cueEnum, MySoundCue? value)
        {
            m_unifiedWeaponCues[(int)cueEnum] = value;
        }

        public MySoundCue? UnifiedWeaponCueGet(MySoundCuesEnum cueEnum)
        {
            return m_unifiedWeaponCues[(int)cueEnum];
        }

        public MyReflectorConfig GetReflectorProperties()
        {
            return m_reflectorProperies;
        }

        public void TrailerUpdate(float reflectorLevel, float engineLevel)
        {
            Config.ReflectorLight.Level = reflectorLevel;
            Config.Engine.Level = engineLevel;
        }

        /// <summary>
        /// This is the real initialization of the class. This separation lets us control when the heavy-lifting is
        /// done in regards to initialization.
        /// </summary>
        /// <param name="helperName">This objects name. For debugging only. Bot default: "Bot".</param>
        /// <param name="scale">Scale for bot model and JLX (only) collions. Not currently used for line-triangleVertexes intersections in octree. Bot default: null.</param>
        /// <param name="doInitInsertionTest">Test for intital intersection</param>
        void InitShip(StringBuilder displayName, /*Matrix matrix,*/MyMwcObjectBuilder_SmallShip objectBuilder, float? scale)
        {
            ShipType = objectBuilder.ShipType;

            m_subObjects.Clear();

            GetModelShipSubObject(ref m_subObjectPlayerHeadForCockpitInteriorTranslation, m_shipTypeProperties.Visual.ModelLod0Enum, "PLAYER_HEAD", false);

            //Will be removed
            if (m_subObjectPlayerHeadForCockpitInteriorTranslation == null)
            {
                GetModelShipSubObject(ref m_subObjectPlayerHeadForCockpitInteriorTranslation, m_shipTypeProperties.Visual.ModelLod0Enum, "COCKPIT", false);
            }

            System.Diagnostics.Debug.Assert(m_subObjectPlayerHeadForCockpitInteriorTranslation != null);

            GetModelShipSubObject(m_subObjectEngineThrustBackwardLeftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustBackwardRightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustStrafeLeft, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_STRAFE_LEFT", true);
            GetModelShipSubObject(m_subObjectEngineThrustStrafeRight, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_STRAFE_RIGHT", true);
            GetModelShipSubObject(m_subObjectEngineThrustDownLeftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_DOWN_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustDownRightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_DOWN_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustForwardLeftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_FORWARD_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustForwardRightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_FORWARD_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustUpLeftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_UP_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustUpRightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_UP_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectReflectorLeft, m_shipTypeProperties.Visual.ModelLod0Enum, "REFLECTOR_LEFT", false);
            GetModelShipSubObject(m_subObjectReflectorRight, m_shipTypeProperties.Visual.ModelLod0Enum, "REFLECTOR_RIGHT", false);
            GetModelShipSubObject(ref m_subObjectLight, m_shipTypeProperties.Visual.ModelLod0Enum, "LIGHT_01", false);
            GetModelShipSubObject(m_subObjectEngineThrustBackwardSmallLeftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD_SMALL_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustBackwardSmallRightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD_SMALL_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustBackward2Leftside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD2_LEFTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustBackward2Rightside, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD2_RIGHTSIDE", true);
            GetModelShipSubObject(m_subObjectEngineThrustBackward2Middle, m_shipTypeProperties.Visual.ModelLod0Enum, "ENGINE_THRUST_BACKWARD2_MIDDLE", true);

            base.Init(displayName, m_shipTypeProperties.Visual.ModelLod0Enum, m_shipTypeProperties.Visual.ModelLod1Enum, null, scale, objectBuilder);

            this.GlassDirtLevel = MIN_GLASS_DIRT_LEVEL;

            m_playerHeadLocal = Vector3.Zero;
            m_playerHeadExtraShake = Vector3.Zero;
            m_playerViewAngleLocal = Vector3.Zero;
            m_playerViewAngleExtraShake = Vector3.Zero;

            m_shaker = new MyCameraHeadShake();

            CockpitGlassModelEnum = m_shipTypeProperties.Visual.CockpitGlassModel;
            CockpitInteriorModelEnum = m_shipTypeProperties.Visual.CockpitInteriorModel;
            m_glassIdealPhysObject = null;

            // it can happen that too many weapons are in this object builder (old server data, etc.)
            // so we have to remove weapons that don't fit into this ship's slots
            RemoveRedundantWeapons(objectBuilder, m_shipTypeProperties.GamePlay.MaxWeapons);

            Weapons = new MySmallShipWeapons(this, m_shipTypeProperties, MAX_AVAILABLE_SLOTS);
            //Weapons = new MySmallShipWeapons(this, m_shipTypeProperties, m_shipTypeProperties.GamePlay.MaxWeapons);
            Weapons.Init(objectBuilder.Weapons, objectBuilder.AssignmentOfAmmo);

            CubeBuilder = new MyCubeBuilder();
            CubeBuilder.Init(null, null, null, this, null, null);

            ////This solves bad saved values in DB
            //if (objectBuilder.ShipMaxHealth == 0 || objectBuilder.ShipMaxHealth == MyGameplayConstants.MAX_HEALTH_MAX)
            //{
            //    objectBuilder.ShipMaxHealth = m_gameplayProperties.MaxHealth;
            //}

            ////This solves bad saved values in DB
            //if (objectBuilder.ShipHealthRatio == 0 || objectBuilder.ShipHealthRatio == MyGameplayConstants.HEALTH_MAX)
            //{
            //    objectBuilder.ShipHealthRatio = m_gameplayProperties.MaxHealth;
            //}

            //MaxHealth = objectBuilder.ShipMaxHealth;
            //Health = objectBuilder.ShipHealthRatio;

            // TODO: MartinBauer - Marcus is loaded from checkpoint and should be Indestructible
            //Debug.Assert(IsDestructible);
            //IsDestructible = true;
            SetMaxHealth(objectBuilder.ShipMaxHealth);
            HealthRatio = objectBuilder.ShipHealthRatio;
            ArmorHealth = objectBuilder.ArmorHealth;
            Oxygen = objectBuilder.Oxygen;
            Fuel = objectBuilder.Fuel;
            Engine = objectBuilder.Engine;
            Armor = objectBuilder.Armor;
            Config.ReflectorLight.SetValue(objectBuilder.ReflectorLight);
            Config.ReflectorLongRange.SetValue(objectBuilder.ReflectorLongRange);
            ReflectorShadowDistance = objectBuilder.ReflectorShadowDistance;

            // Clamp ArmorHealth values
            ArmorHealth = MathHelper.Clamp(ArmorHealth, 0, MaxArmorHealth);

            m_lockTargetCheckTime = 0;
            m_reflectorProperies = new MyReflectorConfig(this);

            SetWorldMatrix(objectBuilder.PositionAndOrientation.GetMatrix());

            //  Collision SPHERE - works best if miner ship is controled by human player (so we ignore high impulses after collision, good sliding along walls, etc)
            MyModel model = MyModels.GetModelOnlyData(m_shipTypeProperties.Visual.ModelLod0Enum);
            //InitSpherePhysics(objectBuilder is MyMwcObjectBuilder_SmallShip_Player ? MyMaterialType.PLAYERSHIP : m_shipTypeProperties.Visual.MaterialType, model, m_shipTypeProperties.Physics.Mass, 0, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_DEFAULT);

            Weight = ShipTypeProperties.Physics.Mass;

            InitPhysics(1 / MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE, objectBuilder is MyMwcObjectBuilder_SmallShip_Player
                    ? MyMaterialType.PLAYERSHIP
                    : m_shipTypeProperties.Visual.MaterialType);

            m_cameraSpring = new MyCameraSpring(this);

            UpdateParked();
            UpdateLight();

            UpdatePlayerHeadForCockpitInteriorWorldMatrix();

            // if this is player's ship, i will register fuel, oxygen, electricity and damage warnings
            if (objectBuilder is MyMwcObjectBuilder_SmallShip_Player)
            {
                MySession.Static.Inventory.OnInventoryContentChange += CheckPointInventory_OnInventoryContentChange;
                LoadHackingTool();
                ToolKits = new MyToolKits(this, MySession.Static.Player);
                InitShipDetector();
                m_detectedObjects = new List<IMyObjectToDetect>();
                OnMarkForClose += new Action<MyEntity>(PlayerShip_OnMarkForClose);

                // sector boundaries
                m_sectorBoundariesWarning = new MyHudTextWarning(MyTextsWrapperEnum.Blank);
                MyHudWarnings.Add(
                    this,
                    new MyHudWarningGroup(
                        new List<MyHudWarning>()
                            {
                                new MyHudWarning(
                                    IsCloseToSectorBoundaries,
                                    null,
                                    m_sectorBoundariesWarning,
                                    0)
                            }, canBeTurnedOff: false
                        ));

                // fuel
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(HasNoFuelLevel, new MyHudSoundWarning(MySoundCuesEnum.HudFuelNoWarning, MySmallShipConstants.WARNING_FUEL_NO_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationFuelNo), 0),
                        new MyHudWarning(HasCriticalFuelLevel, new MyHudSoundWarning(MySoundCuesEnum.HudFuelCriticalWarning, MySmallShipConstants.WARNING_FUEL_CRITICAL_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationFuelCritical), 1),
                        new MyHudWarning(HasLowFuelLevel, new MyHudSoundWarning(MySoundCuesEnum.HudFuelLowWarning, MySmallShipConstants.WARNING_FUEL_LOW_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationFuelLow), 2) 
                    }, canBeTurnedOff: true
                ));
                //// death breath
                //MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                //    {
                //        new MyHudWarning(HasNoOxygen, new MyHudSoundWarning(MySoundCuesEnum.SfxPlayerDeathBreath, 0), null, 0)
                //    }, canBeTurnedOff:false
                //));
                // oxygen
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                     {
                        // TODO simon when cue is prepared
                        new MyHudWarning(HasNoOxygen, new MyHudSoundWarning(MySoundCuesEnum.HudOxygenNoWarning, MySmallShipConstants.WARNING_NO_OXYGEN_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationNoOxygen), 0),
                        new MyHudWarning(HasCriticalOxygenLevel, new MyHudSoundWarning(MySoundCuesEnum.HudOxygenCriticalWarning, MySmallShipConstants.WARNING_OXYGEN_CRITICAL_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationOxygenCritical), 1),
                        new MyHudWarning(HasLowOxygenLevel, new MyHudSoundWarning(MySoundCuesEnum.HudOxygenLowWarning, MySmallShipConstants.WARNING_OXYGEN_LOW_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationOxygenLow), 2) 
                     }, canBeTurnedOff: true
                ));
                // oxygen leaking
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    {
                        new MyHudWarning(IsOxygenLeaking, new MyHudSoundWarning(MySoundCuesEnum.HudOxygenLeakingWarning, MySmallShipConstants.WARNING_OXYGEN_LEAKING_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationOxygenLeaking), 0)
                    }, canBeTurnedOff: true
                ));
                // damage
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsCriticalDamaged, new MyHudSoundWarning(MySoundCuesEnum.HudDamageCriticalWarning, MySmallShipConstants.WARNING_DAMAGE_CRITICAL_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationDamageCritical), 0),
                        new MyHudWarning(IsDamagedForWarnignAlert, new MyHudSoundWarning(MySoundCuesEnum.HudDamageAlertWarning, MySmallShipConstants.WARNING_DAMAGE_ALERT_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationDamageAlert), 1) 
                    }, canBeTurnedOff: true
                ));
                // ammo
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(HasNoAmmoLevel, new MyHudSoundWarning(MySoundCuesEnum.HudAmmoNoWarning, MySmallShipConstants.WARNING_AMMO_NO_INVERVAL), new MyHudTextWarning(MyTextsWrapperEnum.NotificationAmmoNo), 0),
                        new MyHudWarning(HasCriticalAmmoLevel, new MyHudSoundWarning(MySoundCuesEnum.HudAmmoCriticalWarning), null, 1),
                        new MyHudWarning(HasLowAmmoLevel, new MyHudSoundWarning(MySoundCuesEnum.HudAmmoLowWarning), null, 2) 
                    }, canBeTurnedOff: true
                ));
                // armor
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(HasNoArmorLevel, new MyHudSoundWarning(MySoundCuesEnum.HudArmorNoWarning, MySmallShipConstants.WARNING_ARMOR_NO_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationArmorNo), 0),
                        new MyHudWarning(HasCriticalArmorLevel, new MyHudSoundWarning(MySoundCuesEnum.HudArmorCriticalWarning, MySmallShipConstants.WARNING_ARMOR_CRITICAL_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationArmorCritical), 1),
                        new MyHudWarning(HasLowArmorLevel, new MyHudSoundWarning(MySoundCuesEnum.HudArmorLowWarning, MySmallShipConstants.WARNING_ARMOR_LOW_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationArmorLow), 2) 
                    }, canBeTurnedOff: true
                ));
                // radar jammed
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsRadarJammed, new MyHudSoundWarning(MySoundCuesEnum.HudRadarJammedWarning, MySmallShipConstants.WARNING_RADAR_JAMMED_INVERVAL), new MyHudTextWarning(MyTextsWrapperEnum.RadarJammed), 0)
                    }, canBeTurnedOff: true
                ));
                // health
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(HasNoHealthLevel, null, new MyHudTextWarning(MyTextsWrapperEnum.SmallShipPilotDead), 0),
                        new MyHudWarning(HasCriticalHealthLevel, new MyHudSoundWarning(MySoundCuesEnum.HudHealthCriticalWarning, MySmallShipConstants.WARNING_HEALTH_CRITICAL_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationHealthCritical), 1),
                        new MyHudWarning(HasLowHealthLevel, new MyHudSoundWarning(MySoundCuesEnum.HudHealthLowWarning, MySmallShipConstants.WARNING_HEALTH_LOW_INVERVAL), 
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationHealthLow), 2) 
                    }, canBeTurnedOff: true
                ));
                // radiation geiger
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    {
                        new MyHudWarning(IsRadiation, new MyHudSoundWarning(MySoundCuesEnum.SfxGeigerCounterHeavyLoop, MySmallShipConstants.GEIGER_BEEP_INTERVAL), null, 0)                            
                    }, canBeTurnedOff: false
                ));
                // radiation warning
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    {
                        new MyHudWarning(IsRadiationCritical, new MyHudSoundWarning(MySoundCuesEnum.HudRadiationWarning, MySmallShipConstants.WARNING_RADIATION_INTERVAL),                             
                            new MyHudTextWarning(MyTextsWrapperEnum.NotificationRadiationCritical), 0)                            
                    }, canBeTurnedOff: true
                ));
                // engine turn off
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsEngineTurnedOff, null, new MyHudTextWarning(MyTextsWrapperEnum.NotificationEngineOff, MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.WHEEL_CONTROL) ), 0),
                    }, canBeTurnedOff: true
                ));
                // movement slowdown
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsMovementSlowdownDisabled, null, new MyHudTextWarning(MyTextsWrapperEnum.NotificationMovementSlowdownEnabled, MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.MOVEMENT_SLOWDOWN)), 0),
                    }, canBeTurnedOff: true
                ));

                //// enemy alert
                //MyHudWarnings.Add(this, new MyHudWarningGroup()
                //    { 
                //        new MyHudWarning(IsEnemyDetected, new MyHudSoundWarning(MySoundCuesEnum.SfxEnemyAlertWarning, MySmallShipConstants.WARNING_ENEMY_ALERT_INVERVAL), null, 0)
                //    }, canBeTurnedOff:true
                //);
                // missile detected
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsMissileDetected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmIncoming, MySmallShipConstants.WARNING_MISSILE_ALERT_INTERVAL), new MyHudTextWarning(MyTextsWrapperEnum.IncommingMissileAlarm), 1)
                    }, canBeTurnedOff: false
                ));
                // health low
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    { 
                        new MyHudWarning(IsShipDamage1Detected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmDamageA, MySmallShipConstants.WARNING_HEALTH_CONSTANT1), null, 5),
                        new MyHudWarning(IsShipDamage2Detected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmDamageB, MySmallShipConstants.WARNING_HEALTH_CONSTANT2), null, 4),
                        new MyHudWarning(IsShipDamage3Detected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmDamageC, MySmallShipConstants.WARNING_HEALTH_CONSTANT3), null, 3),
                        new MyHudWarning(IsShipDamage4Detected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmDamageD, MySmallShipConstants.WARNING_HEALTH_CONSTANT4), null, 2),
                        new MyHudWarning(IsShipDamage5Detected, new MyHudSoundWarning(MySoundCuesEnum.SfxHudAlarmDamageE, MySmallShipConstants.WARNING_HEALTH_CONSTANT5), null, 1),
                    }, canBeTurnedOff: false
                ));

                m_lowHealthAlertSounds.Add(MySoundCuesEnum.SfxHudAlarmDamageA);
                m_lowHealthAlertSounds.Add(MySoundCuesEnum.SfxHudAlarmDamageB);
                m_lowHealthAlertSounds.Add(MySoundCuesEnum.SfxHudAlarmDamageC);
                m_lowHealthAlertSounds.Add(MySoundCuesEnum.SfxHudAlarmDamageD);
                m_lowHealthAlertSounds.Add(MySoundCuesEnum.SfxHudAlarmDamageE);

                // Solar wind warning
                MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                    {
                        new MyHudWarning(MySunWind.IsActiveForHudWarning, new MyHudSoundWarning(MySoundCuesEnum.HudSolarFlareWarning, MySmallShipConstants.WARNING_SOLAR_WIND_INTERVAL), new MyHudTextWarning(MyTextsWrapperEnum.NotificationSolarWindWarning, MyGuiManager.GetFontMinerWarsRed()), 0),
                    }, canBeTurnedOff: false
                ));

                // Explanation for players that stay in critical state for 30 seconds
                if (MyFakes.ENABLE_WARNING_EXPLANATION)
                {
                    MyHudWarnings.Add(this, new MyHudWarningGroup(new List<MyHudWarning>()
                        {
                            new MyHudWarning(DoesPlayerNeedExplanation, new MyHudSoundWarning(/*TODO:placeholder*/MySoundCuesEnum.SfxAlertVoc, MySmallShipConstants.WARNING_EXPLANATION_INTERVAL, MySmallShipConstants.WARNING_EXPLANATION_INITIAL_DELAY, false), null, 0),
                        }, canBeTurnedOff: true
                    ));
                }
            }

            //OnInventoryContentChange(Inventory);
        }

        private static bool IsCloseToSectorBoundaries()
        {
            return MyGuiScreenGamePlay.Static.GetDistanceToSectorBoundaries() <= MyHudConstants.DISTANCE_FOR_SECTOR_BORDER_WARNING;
        }

        private void CheckPointInventory_OnInventoryContentChange(MyInventory sender)
        {
            LoadHackingTool();
        }

        private void LoadHackingTool()
        {
            int maxHackingLevel = 0;
            foreach (MyInventoryItem inventoryItem in MySession.Static.Inventory.GetInventoryItems())
            {
                if (inventoryItem.ObjectBuilderType != MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool)
                {
                    continue;
                }
                maxHackingLevel = Math.Max(maxHackingLevel, (int)((MyMwcObjectBuilder_SmallShip_HackingTool)inventoryItem.GetInventoryItemObjectBuilder(false)).HackingToolType);
            }
            if (HackingTool == null)
            {
                HackingTool = new MyHackingTool(this, maxHackingLevel);
            }
            else
            {
                HackingTool.HackingLevel = maxHackingLevel;
            }
        }

        public void InitPhysics(float sizeMultiplier, MyMaterialType materialType)
        {
            //  Collision SPHERE - works best if miner ship is controled by human player (so we ignore high impulses after collision, good sliding along walls, etc)
            MyModel model = MyModels.GetModelOnlyData(m_shipTypeProperties.Visual.ModelLod0Enum);
            //InitSpherePhysics(objectBuilder is MyMwcObjectBuilder_SmallShip_Player ? MyMaterialType.PLAYERSHIP : m_shipTypeProperties.Visual.MaterialType, model, m_shipTypeProperties.Physics.Mass, 0, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_DEFAULT);
            InitBoxPhysics(materialType,
                model.BoundingBox.GetCenter(),
                sizeMultiplier * model.BoundingBoxSize,
                GetPhysicsProperties().Mass,
                m_angularDampingDefault,
                MyConstants.COLLISION_LAYER_DEFAULT,
                RigidBodyFlag.RBF_DEFAULT);

            // Lets disable this because we have too many of strange bugs (shivering, inside collisions), maybe it is caused by this below
            if (sizeMultiplier != 1 && this != MySession.PlayerShip)
            {
                Physics.RigidBody.InertiaTensor = MyPhysicsUtils.GetSphereInertiaTensor(model.BoundingBox.Size().Length() / 2, Physics.Mass);
            } 

            //this.Physics.GroupMask = this.GroupMask;
            this.Physics.Type = MyConstants.RIGIDBODY_TYPE_SHIP;
            this.Physics.PlayCollisionCueEnabled = true;
            //this.Physics.MaxLinearVelocity = GetEngineProperties().MaxSpeed * GetEngineProperties().AfterburnerSpeedMultiplier;
            this.Physics.MaxAngularVelocity = GetPhysicsProperties().MaxAngularVelocity;
            this.Physics.Enabled = true;
            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_SMALL_SHIP;
        }

        static void RemoveRedundantWeapons(MyMwcObjectBuilder_SmallShip shipBuilder, int maxWeapons)
        {
            int harvestersAndDrillCount = 0;
            foreach (var weaponBuilder in shipBuilder.Weapons)
            {
                if (weaponBuilder.IsHarvester || weaponBuilder.IsDrill)
                {
                    harvestersAndDrillCount++;
                }
            }

            int maxWeaponsWithHarvestersAndDrills = maxWeapons + harvestersAndDrillCount;

            int indexToRemove = shipBuilder.Weapons.Count - 1;
            while (shipBuilder.Weapons.Count > maxWeaponsWithHarvestersAndDrills)
            {
                var weapon = shipBuilder.Weapons[indexToRemove];
                if (weapon.IsNormalWeapon)
                {
                    shipBuilder.Weapons.Remove(weapon);
                }

                indexToRemove--;
            }
        }

        public bool IsShipDamage1Detected()
        {
            return HealthRatio <= 0.5f && HealthRatio > 0.4f;
        }

        public bool IsShipDamage2Detected()
        {
            return HealthRatio <= 0.4f && HealthRatio > 0.3f;
        }

        public bool IsShipDamage3Detected()
        {
            return HealthRatio <= 0.3f && HealthRatio > 0.2f;
        }

        public bool IsShipDamage4Detected()
        {
            return HealthRatio <= 0.2f && HealthRatio > 0.1f;
        }

        public bool IsShipDamage5Detected()
        {
            return HealthRatio <= 0.1f && HealthRatio > 0;
        }

        public bool DoesPlayerNeedExplanation()
        {
            return IsCriticalDamaged() || HasNoAmmoLevel() || HasCriticalFuelLevel() || HasCriticalOxygenLevel() || HasCriticalHealthLevel();
        }

        private void InitShipDetector()
        {
            //MyDetectingCriteria detectingCriteria = new MyDetectingCriteria();

            //detectingCriteria.DetectingCriteria.Add(
            //new MyDetectingCriterium(
            //    MyDetectedEntityAction.TradeForFree,
            //    new MyDetectingCondition<MyEntity>(
            //    //x => (MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Friend) && (x is MySmallShip || x is MyPrefabHangar || x is MyFoundationFactory) && ((IMyInventory)x).Inventory != null,
            //        x => (MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Friend) && (x is MySmallShip || x is MyPrefabHangar) && ((IMyInventory)x).Inventory != null,
            //        MySmallShipConstants.DETECT_SHIP_RADIUS)));
            //detectingCriteria.DetectingCriteria.Add(
            //    new MyDetectingCriterium(
            //        MyDetectedEntityAction.TradeForMoney,
            //        new MyDetectingCondition<MyEntity>(
            //            x => (MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Neutral) && (x is MySmallShip || x is MyPrefabHangar) && !x.IsCripple() && ((IMyInventory)x).Inventory != null,
            //            MySmallShipConstants.DETECT_SHIP_RADIUS)));
            //detectingCriteria.DetectingCriteria.Add(
            //    new MyDetectingCriterium(
            //        MyDetectedEntityAction.Steal,
            //        new MyDetectingCondition<MyEntity>(
            //            x => (MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Enemy || MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Neutral) && x.IsCripple() && x is MySmallShip,
            //            MySmallShipConstants.DETECT_SHIP_RADIUS)));
            //detectingCriteria.DetectingCriteria.Add(
            //    new MyDetectingCriterium(
            //        MyDetectedEntityAction.Build,
            //    //new MyDetectingCondition<MyFoundationFactory>(
            //    //    x => true,
            //        new MyDetectingCondition<MyPrefabContainer>(
            //            x => (MyFactions.GetFactionsRelation(this.Faction, x.Faction) == MyFactionRelationEnum.Friend) && x.ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory),
            //            MySmallShipConstants.DETECT_FOUNDATION_FACTORY_RADIUS)));

            //ShipDetector = new MyShipDetector(this, detectingCriteria);

            List<IMyEntityDetectorCriterium> tradeCriterias = new List<IMyEntityDetectorCriterium>()
                {
                    new MyEntityDetectorCriterium<MyEntity>((int)MySmallShipInteractionActionEnum.TradeForFree, 
                        MySmallShipInteraction.CanTradeForFree, true, this),                    
                    new MyEntityDetectorCriterium<MyEntity>((int)MySmallShipInteractionActionEnum.TradeForMoney, 
                        MySmallShipInteraction.CanTrade, true, this),
                    new MyEntityDetectorCriterium<MySmallShip>((int)MySmallShipInteractionActionEnum.Loot, 
                        MySmallShipInteraction.CanLootShip, true, this),
                    new MyEntityDetectorCriterium<MyCargoBox>((int)MySmallShipInteractionActionEnum.Examine,
                        MySmallShipInteraction.CanExamineCargoBox, true, this),
                    new MyEntityDetectorCriterium<MyCargoBox>((int)MySmallShipInteractionActionEnum.ExamineEmpty,
                        MySmallShipInteraction.CanExamineEmptyCargoBox, true, this),
                    new MyEntityDetectorCriterium<MyEntity>((int)MySmallShipInteractionActionEnum.Blocked, 
                        MySmallShipInteraction.IsBlocked, true, this),
                };
            List<IMyEntityDetectorCriterium> buildCriterias = new List<IMyEntityDetectorCriterium>()
                {
                    new MyEntityDetectorCriterium<MyPrefabContainer>((int) MySmallShipInteractionActionEnum.Build,
                        MySmallShipInteraction.CanBuild, this)
                };
            List<IMyEntityDetectorCriterium> motherShipCriterias = new List<IMyEntityDetectorCriterium>()
                {
                    new MyEntityDetectorCriterium<MyPrefabHangar>(1, MySmallShipInteraction.IsNearMothership, true, this)
                };
            List<IMyEntityDetectorCriterium> useableEntityCriterias = new List<IMyEntityDetectorCriterium>()
                {
                    new MyEntityDetectorCriterium<MyEntity>((int)MySmallShipInteractionActionEnum.Use, 
                        MySmallShipInteraction.CanUse, true, this),
                    new MyEntityDetectorCriterium<MyEntity>((int)MySmallShipInteractionActionEnum.Hack, 
                        MySmallShipInteraction.CanHack, true, this),
                };

            TradeDetector = new MyEntityDetector();
            TradeDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(MySmallShipConstants.DETECT_SHIP_RADIUS * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, tradeCriterias);
            TradeDetector.OnNearestEntityChange += OnNearestDetectedEntityChanged;
            TradeDetector.OnNearestEntityCriteriasChange += OnNearestDetectedEntityCriteriasChanged;

            BuildDetector = new MyEntityDetector();
            BuildDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(MySmallShipConstants.DETECT_FOUNDATION_FACTORY_RADIUS * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, buildCriterias);
            BuildDetector.OnNearestEntityChange += OnNearestDetectedEntityChanged;
            BuildDetector.OnNearestEntityCriteriasChange += OnNearestDetectedEntityCriteriasChanged;

            MotherShipDetector = new MyEntityDetector();
            MotherShipDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(MySmallShipConstants.DETECT_SHIP_RADIUS * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, motherShipCriterias);
            MotherShipDetector.OnNearestEntityChange += OnNearestDetectedEntityChanged;
            MotherShipDetector.OnNearestEntityCriteriasChange += OnNearestDetectedEntityCriteriasChanged;

            UseableEntityDetector = new MyEntityDetector();
            UseableEntityDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(MySmallShipConstants.DETECT_SHIP_RADIUS * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, useableEntityCriterias);
            UseableEntityDetector.OnNearestEntityChange += OnNearestDetectedEntityChanged;
            UseableEntityDetector.OnNearestEntityCriteriasChange += OnNearestDetectedEntityCriteriasChanged;
        }

        private void StopShipDetectors()
        {
            TradeDetector.TrySetStatus(false);
            BuildDetector.TrySetStatus(false);
            UseableEntityDetector.TrySetStatus(false);
            MotherShipDetector.TrySetStatus(false);
        }

        protected void OnNearestDetectedEntityChanged(MyEntityDetector sender, MyEntity oldEntity, MyEntity newEntity)
        {
            UpdateNotification(sender, newEntity);
        }

        protected void OnNearestDetectedEntityCriteriasChanged(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            UpdateNotification(sender, entity);
        }

        private void UpdateNotification(MyEntityDetector entityDetector, MyEntity entity)
        {
            if (entityDetector == TradeDetector)
            {
                UpdateNotification(entityDetector, ref m_tradeNotification, entity);
            }
            else if (entityDetector == BuildDetector)
            {
                UpdateNotification(entityDetector, ref m_buildNotification, entity);
            }
            else if (entityDetector == MotherShipDetector)
            {
                m_nearMotherShipContainer = entity as MyPrefabHangar;
            }
            else if (entityDetector == UseableEntityDetector)
            {
                UpdateNotification(entityDetector, ref m_securityControlHUBNotification, entity);
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        private void UpdateNotification(MyEntityDetector entityDetector, ref MyHudNotification.MyNotification notification, MyEntity newEntity)
        {
            if (notification != null)
            {
                notification.Disappear();
                notification = null;
            }
            if (newEntity != null)
            {
                MySmallShipInteractionActionEnum action = (MySmallShipInteractionActionEnum)entityDetector.GetNearestEntityCriterias();
                notification = DisplayDetectedEntityActionNotification(newEntity, action);
            }
        }

        MyModelShipSubOject AddSubObject(List<MyModelShipSubOject> subObjects, MyModelSubObject modelSubObject, bool createLight)
        {
            MyModelShipSubOject subObject = m_modelSubObjectsPool.Allocate();
            subObject.ModelSubObject = modelSubObject;

            if (subObjects != null)
                subObjects.Add(subObject);

            if (createLight)
            {
                subObject.Light = MyLights.AddLight();
                if (subObject.Light != null)
                {
                    subObject.Light.Start(MyLight.LightTypeEnum.PointLight, Color.White.ToVector4(), 0.8f, 1);
                    subObject.Light.LightOn = false;
                    subObject.Light.LightOwner = MyLight.LightOwnerEnum.SmallShip;
                }
            }

            m_subObjects.Add(subObject);

            return subObject;
        }

        void GetModelShipSubObject(ref MyModelShipSubOject subObject, MyModelsEnum modelEnum, string subObjectName, bool createLight)
        {
            MyModelSubObject modelSubObject = MyModelSubObjects.GetModelSubObject(modelEnum, subObjectName);
            if (modelSubObject != null)
            {
                subObject = AddSubObject(null, modelSubObject, createLight);
            }
        }

        void GetModelShipSubObject(List<MyModelShipSubOject> subObjects, MyModelsEnum modelEnum, string subObjectName, bool createLight)
        {
            MyModelSubObject modelSubObject = MyModelSubObjects.GetModelSubObject(modelEnum, subObjectName);
            if (modelSubObject != null)
            {
                AddSubObject(subObjects, modelSubObject, createLight);
            }
            else
            {
                for (int i = 0; i < MAX_SAME_SUBOBJECTS; i++)
                {
                    modelSubObject = MyModelSubObjects.GetModelSubObject(modelEnum, subObjectName + "_" + (i + 1).ToString("##00"));
                    if (modelSubObject == null)
                        continue;

                    AddSubObject(subObjects, modelSubObject, createLight);
                }
            }
        }

        void UpdateLight()
        {
            float distanceSq = Vector3.DistanceSquared(MyCamera.Position, WorldMatrix.Translation);
            //float maxTurnOnDistance = 15000;
            //float maxKeepedDistance = 100;

            float lightActivationDistanceSq = 4000 * 4000;

            if (distanceSq > lightActivationDistanceSq)
            {
                return;
            }

            MyPerformanceCounter.PerCameraDraw.Increment("Small ships with updated lights");

            if (/*distance < maxTurnOnDistance && */m_light == null)
            {
                m_light = MyLights.AddLight();
                if (m_light != null)
                {
                    m_light.Start(MyLight.LightTypeEnum.PointLight | MyLight.LightTypeEnum.Spotlight, 1.5f);
                    m_light.ShadowDistanceSquared = m_reflectorShadowDistance * m_reflectorShadowDistance;
                    m_light.ReflectorFalloff = MyMinerShipConstants.MINER_SHIP_NEAR_REFLECTOR_FALLOFF;
                    m_light.LightOwner = MyLight.LightOwnerEnum.SmallShip;
                    m_light.UseInForwardRender = true;

                    if (MyFakes.SMALL_SHIPS_GLARE)
                    {
                        m_light.GlareOn = true;
                        m_light.Glare.Type = MyLightGlare.GlareTypeEnum.Normal;
                        m_light.Glare.QuerySize = 2.5f;
                    }

                    switch (m_reflectorType)
                    {
                        case ReflectorTypeEnum.Single:
                            m_light.ReflectorTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Lights\\reflector");
                            break;

                        case ReflectorTypeEnum.Dual:
                            m_light.ReflectorTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Lights\\dual_reflector");
                            break;

                        default:
                            {
                                throw new MyMwcExceptionApplicationShouldNotGetHere();
                            }
                    }
                }
            }      /*
            if (distance >= maxTurnOnDistance && m_light != null)
            {
                MyLights.RemoveLight(m_light);
                m_light = null;
            }        */

            if (m_light == null) return;

            if (Fuel <= 0 || !Config.Engine.On)
            {
                m_light.ReflectorOn = false;
                m_light.LightOn = false;
                return;
            }

            var isCameraInsideMinerShip = IsCameraInsideMinerShip();

            var positionLightTransformed = GetLightPosition(isCameraInsideMinerShip);

            //  Update light position
            m_light.SetPosition(positionLightTransformed);


            if ((MyGuiScreenGamePlay.Static.ControlledEntity != this) && (MyFactions.GetFactionsRelation(this.Faction, MySession.PlayerShip.Faction) == MyFactionRelationEnum.Friend))
            {
                m_light.ShadowDistance = MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE;
            }
            else
            {
                if (MyRender.DrawPlayerLightShadow || MySession.Is25DSector)
                {
                    m_light.ShadowDistance = MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE;
                }
                else
                    m_light.ShadowDistance = 0;
            }


            //  Get interpolator between reflector influence and muzzle flash influence. It is because
            //  this one light display reflector or muzzle, so we need to interpolate.
            int muzzleFlashDeltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - Weapons.MuzzleFlashLastTime;
            float interpolator = 1.0f - (float)Math.Pow(MathHelper.Clamp(((float)muzzleFlashDeltaTime /
                                                                          (float)MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN), 0, 1), 3);

            var range = MathHelper.Lerp(MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_RANGE, MyMinerShipConstants.MINER_SHIP_MUZZLE_FLASH_LIGHT_RANGE, interpolator);
            range = MathHelper.Clamp(range * MyMinerShipConstants.MINER_SHIP_PLAYER_NEAR_LIGHT_RANGE_MULTIPLIER * LightRangeMultiplier, 0.1f, MyLightsConstants.MAX_POINTLIGHT_RADIUS);
            if (!isCameraInsideMinerShip)
            {
                range *= MyMinerShipConstants.MINER_SHIP_LIGHT_RADIUS_OUTSIDE_MODIFIER;
            }

            m_light.Range = range;
            
            //  Reflector color and range. See that color depends on reflector's level.
            //Vector4 colorReflector = MyMinerShipConstants.MINER_SHIP_REFLECTOR_LIGHT_COLOR * m_reflectorLevel;
            Vector4 colorReflector = GetReflectorProperties().CurrentReflectorLightColor * Config.ReflectorLight.Level;

            //  Color and range will be interpolated by current muzzle flash value and by state of reflector
            m_light.Color = Vector4.Lerp(colorReflector, MyMinerShipConstants.MINER_SHIP_MUZZLE_FLASH_LIGHT_COLOR, interpolator);
            m_light.SpecularColor = new Vector3(colorReflector.X, colorReflector.Y, colorReflector.Z);
            m_light.Falloff = MyMinerShipConstants.MINER_SHIP_LIGHT_FALLOFF;

            //more distant the ship, smaller light is.
            //float smallerSpeed = 0.1f; //10% on 100m
            /*float ratio = 1.0f;

            if (distance > maxKeepedDistance)
                ratio = 0.7f / (((distance - maxKeepedDistance) / 100.0f) + 1);
            m_light.Range *= ratio;
              */
            m_light.Intensity = MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_INTENSITY * System.Math.Max(Config.ReflectorLight.Level, interpolator) * LightIntensityMultiplier;
            /*if (this != MySession.PlayerShip)
            {
                m_light.Intensity *= 1.0f;
            }
              */

            m_light.ReflectorIntensity = MyMinerShipConstants.MINER_SHIP_NEAR_REFLECTOR_INTENSITY * ReflectorIntensityMultiplier;

            // Reflector properties
            m_light.ReflectorOn = Config.ReflectorLight.Level > 0.0f;
            if (m_light.ReflectorOn)
            {
                m_light.ReflectorUp = this.WorldMatrix.Up;
                m_light.ReflectorDirection = this.WorldMatrix.Forward;
                m_light.ReflectorColor = Config.ReflectorLight.Level * GetReflectorProperties().CurrentReflectorLightColor;
                m_light.UpdateReflectorRangeAndAngle(GetReflectorProperties().CurrentReflectorConeAngleForward, GetReflectorProperties().CurrentReflectorRangeForward);
            }

            m_light.LightOn = !IsDisabledByEMP();
        }

        private Vector3 GetLightPosition(bool isCameraInsideThisShip)
        {
            if (m_subObjectLight == null)
            {
                // fallback to some value at least
                return this.GetPosition();
            }

            Matrix worldMatrix = this.WorldMatrix;
            if (isCameraInsideThisShip)
            {
                return MyUtils.GetTransform(m_subObjectPlayerHeadForCockpitInteriorTranslation.ModelSubObject.Position, ref worldMatrix);
            }
            else
            {
                LightOffsetZ = -5;
                LightOffsetY = -3;
                Vector3 offset = new Vector3(LightOffsetX, LightOffsetY, LightOffsetZ); // For debug testing
                return MyUtils.GetTransform(m_subObjectLight.ModelSubObject.Position + offset, ref worldMatrix);
            }
        }

        public bool IsCameraThirdPerson()
        {
            return ((MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic) && (this.Equals(MySession.PlayerShip)));
        }

        public bool IsCameraInsideMinerShip()
        {
            return ((MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip) && (this.Equals(MySession.PlayerShip)));
        }

        //  Draw all engine thrusts as billboards and polylines.
        //  IMPORTANT: In the method I use a lot of 'ref' for performance reasons, so please be careful.
        void DrawEngineThrusts()
        {
            //  Calculate velocity, speed and local velocity (in model's coordinate space) based on position change from previous Update
            if (Physics == null)
                return;
            float actualSpeed = Physics.Speed;
            Matrix worldInv = this.GetWorldMatrixInverted();
            Vector3 actualVelocityLocal = MyUtils.GetVector3Scaled(MyUtils.GetTransformNormal(Physics.LinearVelocity, ref worldInv), actualSpeed);
            Vector3 actualAcceleration = MyUtils.GetTransformNormal(Physics.LinearAcceleration, ref worldInv);

            //  Calculate strengths for drawing engine thrusts
            float thrustStrengthBackward = CalculateEngineThrustStrength(actualAcceleration.Z, actualVelocityLocal.Z);
            float thrustStrengthForward = CalculateEngineThrustStrength(-actualAcceleration.Z, -actualVelocityLocal.Z);
            float thrustStrengthStrafeLeft = CalculateEngineThrustStrength(-actualAcceleration.X, -actualVelocityLocal.X);
            float thrustStrengthStrafeRight = CalculateEngineThrustStrength(actualAcceleration.X, actualVelocityLocal.X);
            float thrustStrengthUp = CalculateEngineThrustStrength(actualAcceleration.Y, actualVelocityLocal.Y);
            float thrustStrengthDown = CalculateEngineThrustStrength(-actualAcceleration.Y, -actualVelocityLocal.Y);

            DrawEngineThrusts(m_subObjectEngineThrustBackwardSmallLeftside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackwardSmallRightside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackwardLeftside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackwardRightside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackward2Leftside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackward2Rightside, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustBackward2Middle, thrustStrengthBackward);
            DrawEngineThrusts(m_subObjectEngineThrustForwardLeftside, thrustStrengthForward);
            DrawEngineThrusts(m_subObjectEngineThrustForwardRightside, thrustStrengthForward);
            DrawEngineThrusts(m_subObjectEngineThrustStrafeLeft, thrustStrengthStrafeLeft);
            DrawEngineThrusts(m_subObjectEngineThrustStrafeRight, thrustStrengthStrafeRight);
            DrawEngineThrusts(m_subObjectEngineThrustUpLeftside, thrustStrengthUp);
            DrawEngineThrusts(m_subObjectEngineThrustUpRightside, thrustStrengthUp);
            DrawEngineThrusts(m_subObjectEngineThrustDownLeftside, thrustStrengthDown);
            DrawEngineThrusts(m_subObjectEngineThrustDownRightside, thrustStrengthDown);

        }

        //  Calculate strength of engine thrust
        float CalculateEngineThrustStrength(float actualAccelerationLocal, float actualVelocityLocal)
        {
            const float MIN_VEL = 0.1f;
            const float MAX_VEL = 10.0f;

            float result = 0;

            //  Only if there is acceleration
            if (actualAccelerationLocal < -0.001f)
            {
                //  If velocity in direction we are interseted is not zero
                if (actualVelocityLocal <= 0)
                {
                    result = -actualVelocityLocal / (MAX_VEL - MIN_VEL);
                }
                else
                {
                    //  This will be thrust that stops the ship from oposite movement. We decrease it because it can't be too long.
                    result = actualVelocityLocal / (MAX_VEL - MIN_VEL) * 0.25f;
                }
            }

            return MathHelper.Clamp(result, 0, 1);
        }

        //  Draws particle in place of engine thrust. Size/length is determined by 'strength', where 0 means no thrust is drawn, 1 means full size/length thrust particles are drawn.
        //  Input position must be in world space.
        void DrawEngineThrusts(List<MyModelShipSubOject> subObjects, float strength)
        {
            var activeEngineProps = GetEngineProperties();
            Vector4 color = activeEngineProps.ThrustsColor;
            color.W = 0.75f;

            foreach (MyModelShipSubOject subObject in subObjects)
            {
                if (subObject.ModelSubObject == null) continue;
                if (subObject.ModelSubObject.Enabled == false) continue;

                Matrix worldMatrix = this.WorldMatrix;
                Vector3 position = MyUtils.GetTransform(subObject.ModelSubObject.Position, ref worldMatrix);
                Vector3 forward = MyUtils.GetTransformNormalNormalized(subObject.ModelSubObject.ForwardVector.Value, ref worldMatrix);

                float thrustRadius = 1.0f;
                if (subObject.ModelSubObject.Scale != null)
                {
                    thrustRadius = Config.Engine.Level * MyMwcUtils.GetRandomFloat(subObject.ModelSubObject.Scale.Value * 0.9f, subObject.ModelSubObject.Scale.Value);
                }

                float thrustLength = 0;

                if (strength > 0 && thrustRadius > 0)
                {
                    float angle = 1 - Math.Abs(Vector3.Dot(MyMwcUtils.Normalize(MyCamera.Position - position), forward));
                    float alphaCone = (1 - (float)Math.Pow(1 - angle, 30)) * 0.5f;

                    thrustLength = strength * 5 * MyMwcUtils.GetRandomFloat(0.6f, 1.0f);
                    float thrustThickness = MyMwcUtils.GetRandomFloat(thrustRadius * 0.90f, thrustRadius);

                    //  We move polyline particle backward, because we are stretching ball texture and it doesn't look good if stretched. This will hide it.
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.EngineThrustMiddle, color * alphaCone, position - forward * thrustLength * 0.25f,
                                                 forward, thrustLength, thrustThickness);
                }

                if (thrustRadius > 0)
                    MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.EngineThrustMiddle, color, position, thrustRadius, 0);

                if (subObject.Light != null)
                {
                    if (strength > 0 && thrustRadius > 0)
                    {
                        subObject.Light.LightOn = true;
                        subObject.Light.Intensity = 1.3f + thrustLength * 2;
                        subObject.Light.Color = color;
                        subObject.Light.Range = thrustRadius * 12 + thrustLength / 10;
                        subObject.Light.SetPosition(subObject.ModelSubObject.ForwardVector.Value + position);
                        subObject.Light.LightType = MinerWars.AppCode.Game.Lights.MyLight.LightTypeEnum.PointLight;
                    }
                    else
                    {
                        subObject.Light.LightOn = false;
                        subObject.Light.Intensity = 0;
                    }
                }
            }
        }

        void DrawReflectors()
        {
            DrawReflector(m_subObjectReflectorLeft);
            DrawReflector(m_subObjectReflectorRight);
        }

        void DrawReflector(List<MyModelShipSubOject> subObjects)
        {
            foreach (MyModelShipSubOject subObject in subObjects)
            {

                Matrix worldMatrix = this.WorldMatrix;
                Vector3 position = MyUtils.GetTransform(subObject.ModelSubObject.Position, ref worldMatrix);
                Vector3 forwardVector = MyUtils.GetTransformNormal(subObject.ModelSubObject.ForwardVector.Value, ref worldMatrix);
                Vector3 leftVector = MyUtils.GetTransformNormal(subObject.ModelSubObject.LeftVector.Value, ref worldMatrix);
                Vector3 upVector = MyUtils.GetTransformNormal(subObject.ModelSubObject.UpVector.Value, ref worldMatrix);

                float reflectorLength = m_reflectorProperies.CurrentBillboardLength / 2.0f;//40;
                float reflectorThickness = m_reflectorProperies.CurrentBillboardThickness / 2.0f;//6.0f;
                float reflectorRadiusForAdditive = 1;//0.65f;
                reflectorRadiusForAdditive = subObject.ModelSubObject.Scale.Value;

                Vector3 color = Vector3.One;

                var dot = Vector3.Dot(MyMwcUtils.Normalize(MyCamera.Position - position), forwardVector);
                float angle = 1 - Math.Abs(dot);
                float alphaGlareAlphaBlended = (float)Math.Pow(1 - angle, 2);
                float alphaGlareAdditive = (float)Math.Pow(1 - angle, 2);
                float alphaCone = (1 - (float)Math.Pow(1 - angle, 30)) * 0.25f;

                float reflectorRadiusForAlphaBlended = MathHelper.Lerp(0.5f, 2.5f, alphaGlareAlphaBlended); //3.5f;

                //  Multiply alpha by reflector level (and not the color), because if we multiply the color and let alpha unchanged, reflector cune will be drawn as very dark cone, but still visible
                var reflectorLevel = Config.ReflectorLight.Level;

                if (IsDisabledByEMP())
                {
                    reflectorLevel = 0;
                }

                alphaCone *= reflectorLevel;
                alphaGlareAlphaBlended *= reflectorLevel * 0.3f;
                alphaGlareAdditive *= reflectorLevel * 0.8f;

                if (reflectorLength > 0 && reflectorThickness > 0)
                {
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ReflectorCone, new Vector4(color, 1.0f) * alphaCone, position - forwardVector * 1.5f,
                                                 forwardVector, reflectorLength, reflectorThickness);
                }
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, new Vector4(color, 1.0f) * alphaGlareAlphaBlended,
                                              position + forwardVector * 0.7f, reflectorRadiusForAlphaBlended, 0);
                if (dot > 0)
                {
                    MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.ReflectorGlareAdditive, new Vector4(color, 1f) * alphaGlareAdditive,
                                                     position - forwardVector * 0.38f, leftVector, upVector, reflectorRadiusForAdditive);
                }

                //MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, new Vector4(color, 1.0f) * alphaGlareAlphaBlended,
                //                              position + forwardVector * 0.7f, reflectorRadiusForAlphaBlended, 0);
                //MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.ReflectorGlareAdditive, new Vector4(color, 0.65f) * alphaGlareAdditive,
                //                                 position - forwardVector * 0.4f, leftVector, upVector, reflectorRadiusForAdditive);

            }
        }

        public MyCockpit GetShipCockpit()
        {
            if (m_cockpit == null)
            {
                m_cockpit = new MyCockpit();
                m_cockpit.Init(null, CockpitInteriorModelEnum, null, this, null, null);
            }
            return m_cockpit;
        }

        //  Calculates intersection of line with any triangleVertexes in this model instance. Closest intersection and intersected triangleVertexes will be returned.

        //  Lazy-load for cockpit glass, because it's needed only for ship controlled by a player
        public MyCockpitGlassEntity GetShipCockpitGlass()
        {
            if (m_glassIdealPhysObject == null)
            {
                m_glassIdealPhysObject = new MyCockpitGlassEntity();
                m_glassIdealPhysObject.Init(null, CockpitGlassModelEnum, null, this, null, null);
            }
            return m_glassIdealPhysObject;
        }

        internal void AcquireTarget()
        {
            //  Release dead targets
            if (TargetEntity != null)
            {
                if (TargetEntity.IsDead())
                {
                    TargetEntity = null;
                }
            }

            MyLine line;
            MyEntity shootingEntity = this;
            MyIntersectionResultLineBoundingSphere? result;

            if (this == MySession.PlayerShip && MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic)
            {
                // Handle targeting for players in third person camera mode
                Vector3 toVector = MyThirdPersonSpectator.GetCrosshair() - MyThirdPersonSpectator.Position;
                if (toVector == Vector3.Zero)
                {
                    return;
                }
                toVector.Normalize();
                line = new MyLine(MyThirdPersonSpectator.Position, MyThirdPersonSpectator.Position + toVector * 10000, true);
            }
            else if (MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon)
            {
                line = MyGuiScreenGamePlay.Static.ControlledLargeWeapon.GetLine();
                shootingEntity = MyGuiScreenGamePlay.Static.ControlledLargeWeapon;
            }
            else
            {
                line = new MyLine(this.GetPosition(), this.GetPosition() + WorldMatrix.Forward * 10000, true);
            }
            result = MyEntities.GetIntersectionWithLineAndBoundingSphere(ref line, MyGuiScreenGamePlay.Static.ControlledLargeWeapon, null, MySmallShipConstants.LOCK_TARGET_OVERLAP, MyMissile.CanBeTargetedPredicate, true);

            if (result.HasValue && !result.Value.PhysObject.IsDead())
            {
                MyEntity target = result.Value.PhysObject;

                bool canSeeTest = false;
                if ((shootingEntity.GetPosition() - target.GetPosition()).LengthSquared() > MyMwcMathConstants.EPSILON * 100.0f)
                {
                    MyLine canSeeLine = new MyLine(shootingEntity.GetPosition(), target.GetPosition(), true);
                    canSeeTest = !MyEntities.GetIntersectionWithLine(ref canSeeLine, shootingEntity, target, ignoreChilds: true).HasValue;
                }

                if (canSeeTest && target != TargetEntity && this != target && MyFactions.GetFactionsRelation(this, target) == MyFactionRelationEnum.Enemy)
                {
                    int lockTime = MySmallShipConstants.LOCK_TARGET_CHECK_TIME;     // add one check period (curently acquired target)
                    foreach (var historyTarget in m_targetEntityHistory)
                    {
                        lockTime += MySmallShipConstants.LOCK_TARGET_CHECK_TIME;
                        if (historyTarget != target)
                        {
                            m_targetEntityHistory.Clear();
                            m_targetEntityHistory.Add(target);  // start locking new target
                            return;
                        }

                        // succesfull target lock
                        if (lockTime >= MySmallShipConstants.LOCK_TARGET_TIME)
                        {
                            TargetEntity = target;

                            if (this == MySession.PlayerShip && MyGuiScreenGamePlay.Static.IsGameActive())
                                MyAudio.AddCue2D(MySoundCuesEnum.WepMissileLock);
                            return;
                        }
                    }

                    // continue locking same target
                    m_targetEntityHistory.Add(target);
                }
            }
            else
            {
                m_targetEntityHistory.Clear();
            }
        }

        private void OnArmorChanged()
        {
            m_armorProperties = m_armor != null ? MySmallShipArmorTypeConstants.GetProperties(m_armor.ArmorType) : (MySmallShipArmorTypeProperties?)null;
            var armorColor = m_armorProperties.HasValue ? m_armorProperties.Value.DiffuseColor : Vector3.One;
            if (m_modelLod0 != null)
            {
                foreach (var mesh in m_modelLod0.GetMeshList())
                {
                    //We will make armor visualisation better later
                    //mesh.GetMaterial().DiffuseColor = armorColor;
                }
            }
        }

        protected override void OnInventoryContentChange(MyInventory sender)
        {
            Inventory.OnInventoryContentChange -= OnInventoryContentChange;
            base.OnInventoryContentChange(sender);

            // we must play new sound for new engine type, so we must stop old sound cues
            if (m_engineChanged)
            {
                StopEngineSounds(SharpDX.XACT3.StopFlags.Release);
                if (MySession.PlayerShip == this)
                {
                    SetEngineSound(Config.Engine.On);
                }
                m_engineChanged = false;
            }

            //UpdateWeight();
            UpdateMaxFuel();
            UpdateMaxOxygen();
            UpdateRadarCapabilities();

            if (Fuel > MaxFuel)
            {
                Fuel = MaxFuel;
            }

            if (Oxygen > MaxOxygen)
            {
                Oxygen = MaxOxygen;
            }

            m_hasRadarJammer = Inventory.Contains(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER);
            if (Inventory.Contains(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL))
            {
                if (m_nanoRepairTool == null)
                {
                    m_nanoRepairTool = new MyNanoRepairToolEntity(this);
                }
            }
            else
            {
                m_nanoRepairTool = null;
            }

            if (MySession.PlayerShip == this)
            {
                // False ID's
                FalseFactions = MyMwcObjectBuilder_FactionEnum.None;
                m_helperInventoryItems.Clear();
                Inventory.GetInventoryItems(ref m_helperInventoryItems, MyMwcObjectBuilderTypeEnum.FalseId, null);
                foreach (MyInventoryItem falsefactionsInventoryItem in m_helperInventoryItems)
                {
                    FalseFactions |= ((MyMwcObjectBuilder_FalseId)falsefactionsInventoryItem.GetInventoryItemObjectBuilder(false)).Faction;
                }

                // if any blueprints was added to player ship's inventory, then we move them to checkpoint's inventory
                // if any hackingtool was added to player ship's inventory, then we move them to checkpoint's inventory
                m_helperInventoryItems.Clear();
                Inventory.GetInventoryItems(ref m_helperInventoryItems, MyMwcObjectBuilderTypeEnum.Blueprint, null);
                Inventory.GetInventoryItems(ref m_helperInventoryItems, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, null);

                foreach (MyInventoryItem itemToCheckpointInventory in m_helperInventoryItems)
                {
                    if (!MySession.Static.Inventory.Contains(itemToCheckpointInventory.GetInventoryItemObjectBuilder(false)))
                    {
                        MySession.Static.Inventory.AddInventoryItem(itemToCheckpointInventory);
                    }
                }
                Inventory.RemoveInventoryItems(m_helperInventoryItems);
            }
            Inventory.OnInventoryContentChange += OnInventoryContentChange;
        }

        private void UpdateRadarCapabilities()
        {
            RadarCapabilities.Clear();
            foreach (var item in Inventory.GetInventoryItems())
            {
                if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Radar)
                {
                    RadarCapabilities.Add((MyMwcObjectBuilder_SmallShip_Radar_TypesEnum)item.ObjectBuilderId);
                }
            }
        }

        //private void UpdateWeight()
        //{
        //    var inventoryItems = Inventory.GetInventoryItems();
        //    Weight = GetPhysicsProperties().Mass;

        //    if (m_includeCargoWeight)
        //    {
        //        foreach (var inventoryItem in inventoryItems)
        //        {
        //            Weight += inventoryItem.Weight;
        //        }

        //        if (Engine != null)
        //        {
        //            var props = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)Engine.EngineType, Faction);
        //            if (props != null)
        //            {
        //                Weight += props.WeightPerUnit;
        //            }
        //        }

        //        if (Armor != null)
        //        {
        //            var props = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)Armor.ArmorType, Faction);
        //            if (props != null)
        //            {
        //                Weight += props.WeightPerUnit;
        //            }
        //        }

        //        if (Weapons != null)
        //        {
        //            foreach (var slot in Weapons.WeaponSlots)
        //            {
        //                if (slot.IsMounted())
        //                {
        //                    var weapon = slot.MountedWeapon;
        //                    var props = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)weapon.WeaponType, Faction);
        //                    if (props != null)
        //                    {
        //                        Weight += props.WeightPerUnit;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Physics is null on Sector Server
        //    if (Physics != null)
        //    {
        //        Physics.Mass = Weight;
        //    }
        //}

        public void UpdateMaxFuel()
        {
            MaxFuel = m_shipTypeProperties.GamePlay.FuelCapacity + m_shipTypeProperties.GamePlay.ExtraFuelCapacity *
                Inventory.GetInventoryItemsCount(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_FUEL_CONTAINER_DISABLED);
        }

        public void UpdateMaxOxygen()
        {
            MaxOxygen = m_shipTypeProperties.GamePlay.OxygenCapacity + m_shipTypeProperties.GamePlay.ExtraOxygenCapacity *
                Inventory.GetInventoryItemsCount(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_OXYGEN_CONTAINER_DISABLED);
        }


        public MyPrefabHangar GetNearMotherShipContainer()
        {
            return m_nearMotherShipContainer;
        }

        ///// <summary>
        ///// Try detect another entity in detecting radius, if founded then display notification about it
        ///// </summary>
        //private void DetectEntityToAction()
        //{
        //    //MyDetectedEntity oldNearestDetectedShip = ShipDetector.GetDetectedEntities().Find(x => x.Entity is MySmallShip);
        //    MyDetectedEntity oldNearestDetectedEntity = ShipDetector.GetNearestDetectedEntity();

        //    ShipDetector.Detect();
        //    // if foundation factory detected
        //    MyDetectedEntity detectedFoundationFactory = ShipDetector.GetDetectedEntities().Find(x => x.DetectedAction == MyDetectedEntityAction.Build);
        //    if (detectedFoundationFactory != null)
        //    {
        //        if (m_buildNotification == null)
        //        {
        //            m_buildNotification = DisplayDetectedEntityActionNotification(detectedFoundationFactory.Entity, MyDetectedEntityAction.Build);
        //        }
        //    }
        //    else
        //    {
        //        if (m_buildNotification != null)
        //        {
        //            m_buildNotification.Disapear();
        //            m_buildNotification = null;
        //        }
        //    }

        //    if (MyFakes.USE_LARGE_SHIP_HANGAR_DETECTION)
        //    {
        //        MyPrefabContainer nearMotherShipPrefabContainer = null;
        //        MyDetectedEntity detectedHangar = ShipDetector.GetDetectedEntities().Find(x => x.Entity is MyPrefabHangar);
        //        if (detectedHangar != null)
        //        {
        //            MyPrefabContainer prefabContainer = (detectedHangar.Entity as MyPrefabHangar).GetOwner();
        //            if (prefabContainer.ContainsPrefab(PrefabTypesFlagEnum.LargeShip))
        //            {
        //                nearMotherShipPrefabContainer = prefabContainer;
        //            }
        //        }
        //        if (nearMotherShipPrefabContainer != null)
        //        {
        //            if (MySession.PlayerShip != null && nearMotherShipPrefabContainer.Faction == MySession.PlayerShip.Faction)
        //                m_nearMotherShipContainer = nearMotherShipPrefabContainer;
        //            else
        //                m_nearMotherShipContainer = null;
        //        }
        //        else
        //            m_nearMotherShipContainer = null;

        //        if (m_nearMotherShipContainer != null && MyGuiScreenGamePlay.Static.CanTravel)
        //        {
        //            if (m_travelNotification == null)
        //                m_travelNotification = DisplayDetectedEntityActionNotification(detectedHangar.Entity, MyDetectedEntityAction.Travel);
        //        }
        //        else
        //        {
        //            if (m_travelNotification != null)
        //            {
        //                m_travelNotification.Disapear();
        //                m_travelNotification = null;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MyDetectedEntity detectedLargeShip = ShipDetector.GetDetectedEntities().Find(x => x.Entity is MyPrefabBase);
        //        if (detectedLargeShip != null)
        //        {
        //            var nearMotherShip = detectedLargeShip.Entity as MyPrefabLargeShip;
        //            if (nearMotherShip != null && MySession.PlayerShip != null && nearMotherShip.Faction == MySession.PlayerShip.Faction)
        //                m_nearMotherShipContainer = nearMotherShip.GetOwner();
        //            else
        //                m_nearMotherShipContainer = null;
        //        }
        //        else
        //            m_nearMotherShipContainer = null;

        //        if (m_nearMotherShipContainer != null && MyGuiScreenGamePlay.Static.CanTravel)
        //        {
        //            if (m_travelNotification == null)
        //                m_travelNotification = DisplayDetectedEntityActionNotification(detectedLargeShip.Entity, MyDetectedEntityAction.Travel);
        //        }
        //        else
        //        {
        //            if (m_travelNotification != null)
        //            {
        //                m_travelNotification.Disapear();
        //                m_travelNotification = null;
        //            }
        //        }
        //    }

        //    // if new entity to trade/free trade/loot detected
        //    MyDetectedEntity newNearestDetectedEntity = ShipDetector.GetNearestDetectedEntity();
        //    bool displayNotification = false;
        //    bool disapearNotification = false;
        //    if (newNearestDetectedEntity != null)
        //{
        //        if (oldNearestDetectedEntity != null && oldNearestDetectedEntity.Entity != newNearestDetectedEntity.Entity)
        //    {
        //            disapearNotification = true;
        //            displayNotification = true;
        //    }
        //        else if (oldNearestDetectedEntity == null)
        //        {
        //            displayNotification = true;
        //}
        //    }
        //    else if (oldNearestDetectedEntity != null)
        //{
        //        disapearNotification = true;
        //    }

        //    if (m_tradeNotification != null && disapearNotification)
        //    {
        //        m_tradeNotification.Disapear();
        //        m_tradeNotification = null;
        //    }
        //    if (displayNotification)
        //    {
        //        if (newNearestDetectedEntity.Entity is MyPrefabContainer)
        //        {
        //            if (((MyPrefabContainer)newNearestDetectedEntity.Entity).ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory) &&
        //            !((MyPrefabContainer)newNearestDetectedEntity.Entity).ContainsPrefab(PrefabTypesFlagEnum.Hangar))
        //            {
        //                m_tradeNotification = DisplayDetectedEntityActionNotification(newNearestDetectedEntity.Entity, MyDetectedEntityAction.TradeForFree);
        //    }
        //}
        //        else
        //        {
        //            m_tradeNotification = DisplayDetectedEntityActionNotification(newNearestDetectedEntity.Entity, newNearestDetectedEntity.DetectedAction);
        //        }
        //    }


        //    //// if new ship to trade/free trade/loot detected
        //    //MyDetectedEntity newNearestDetectedShip = ShipDetector.GetDetectedEntities().Find(x => x.Entity is MySmallShip);            
        //    //if (oldNearestDetectedShip == null)
        //    //{
        //    //    if (newNearestDetectedShip != null)
        //    //    {
        //    //        m_tradeNotification = DisplayDetectedEntityActionNotification(newNearestDetectedShip);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (newNearestDetectedShip == null || newNearestDetectedShip.Entity != oldNearestDetectedShip.Entity)
        //    //    {
        //    //        m_tradeNotification.Disapear();
        //    //    }
        //    //    if (newNearestDetectedShip != null && newNearestDetectedShip.Entity != oldNearestDetectedShip.Entity)
        //    //    {
        //    //        m_tradeNotification = DisplayDetectedEntityActionNotification(newNearestDetectedShip);
        //    //    }
        //    //}
        //}

        public void ClearNotifications()
        {
            TradeDetector.Reset();
            BuildDetector.Reset();
            MotherShipDetector.Reset();
            UseableEntityDetector.Reset();
            //SecurityControlHUBDetector.Reset();

            //if (m_tradeNotification != null) m_tradeNotification.Disapear();
            //if (m_buildNotification != null) m_buildNotification.Disapear();
            //if (m_travelNotification != null) m_travelNotification.Disapear();

            //m_tradeNotification = null;
            //m_buildNotification = null;
            //m_travelNotification = null;
        }

        /// <summary>
        /// Displays notification
        /// </summary>
        /// <param name="entity">Entity</param>        
        /// <param name="action">Action</param>
        private MyHudNotification.MyNotification DisplayDetectedEntityActionNotification(MyEntity entity, MySmallShipInteractionActionEnum action)
        {
            string entityName = entity.DisplayName;
            //// if we trade throught hangar, we want display prefab container's name, not hangar's name
            //if (entity is MyPrefabHangar)
            //{
            //    entityName = (entity as MyPrefabHangar).GetOwner().DisplayName;
            //}

            //else
            //{
            //    entityName = entity.DisplayName;
            //}


            //get localized entity name
            entityName = entity.GetCorrectDisplayName();

            if (String.IsNullOrWhiteSpace(entityName))
            {
                entityName = MyTextsWrapper.Get(MyTextsWrapperEnum.ThisObject).ToString();
            }

            bool isMissionNotification = MyMissions.IsMissionEntityNotification(entity, action);
            object[] args = new object[3];
            MyTextsWrapperEnum notificationText;
            MyGuiFont notificationFont;
            switch (action)
            {
                case MySmallShipInteractionActionEnum.TradeForFree:
                    notificationFont = MyHudConstants.FRIEND_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanTradeWith;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.INVENTORY);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.TradeForMoney:
                    notificationFont = MyHudConstants.NEUTRAL_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanTradeWith;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.INVENTORY);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Loot:
                    notificationFont = MyHudConstants.ENEMY_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanLoot;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.INVENTORY);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Examine:
                    notificationFont = MyHudConstants.NEUTRAL_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanExamine;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.INVENTORY);
                    args[1] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE);
                    args[2] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.ExamineEmpty:
                    notificationFont = MyHudConstants.NEUTRAL_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanExamineEmpty;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.INVENTORY);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Blocked:
                    notificationFont = MyHudConstants.ENEMY_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCantExamine;
                    args[0] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Build:
                    notificationFont = MyHudConstants.NEUTRAL_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanBuild;
                    break;
                case MySmallShipInteractionActionEnum.Travel:
                    notificationFont = MyHudConstants.FRIEND_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanTravel;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.TRAVEL);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Use:
                    IMyUseableEntity useableEntity = entity as IMyUseableEntity;
                    Debug.Assert(useableEntity != null);
                    notificationFont = MyHudConstants.FRIEND_FONT;
                    if (useableEntity.UseProperties.UseText != null)
                    {
                        notificationText = useableEntity.UseProperties.UseText.Value;
                    }
                    else
                    {
                        notificationText = MyTextsWrapperEnum.NotificationYouCanUse;
                    }
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE);
                    args[1] = entityName;
                    break;
                case MySmallShipInteractionActionEnum.Hack:
                    notificationFont = MyHudConstants.NEUTRAL_FONT;
                    notificationText = MyTextsWrapperEnum.NotificationYouCanHack;
                    args[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE);
                    args[1] = entityName;
                    break;
                default:
                    throw new Exception();
            }

            MyHudNotification.MyNotification notification = new MyHudNotification.MyNotification(
                notificationText,
                MyHudNotification.GetCurrentScreen(),
                1f,
                isMissionNotification ? MyHudConstants.MISSION_FONT : notificationFont,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                MyHudNotification.DONT_DISAPEAR,
                null,
                false,
                args);
            MyHudNotification.AddNotification(notification);

            return notification;
        }

        #region Methods for HUD warnings
        int m_radiationLastDamage = 0;
        int m_radiationCriticalLastDamage = 0;

        private bool IsRadiation()
        {
            return MyMinerGame.TotalGamePlayTimeInMilliseconds - m_radiationLastDamage <= MySmallShipConstants.RADIATION_DAMAGE_MAX_TIME;
        }

        private bool IsRadiationCritical()
        {
            return MyMinerGame.TotalGamePlayTimeInMilliseconds - m_radiationCriticalLastDamage <= MySmallShipConstants.RADIATION_DAMAGE_MAX_TIME;
        }


        public virtual bool IsPilotDead()
        {
            if (MySession.Static != null && MySession.Static.Player != null && MySession.PlayerShip == this)
            {
                return MySession.Static.Player.IsDead();
            }
            else
            {
                return PilotHealth <= 0;
            }
        }

        public bool IsDamagedForWarnignAlert()
        {
            return HealthRatio <= MySmallShipConstants.WARNING_DAMAGE_ALERT_LEVEL;
        }

        public bool IsCriticalDamaged()
        {
            return HealthRatio <= MySmallShipConstants.WARNING_DAMAGE_CRITICAL_LEVEL;
        }

        public bool HasLowOxygenLevel()
        {
            return Oxygen <= MaxOxygen * MySmallShipConstants.WARNING_OXYGEN_LOW_LEVEL;
        }

        public bool HasCriticalOxygenLevel()
        {
            return Oxygen <= MaxOxygen * MySmallShipConstants.WARNING_OXYGEN_CRITICAL_LEVEL;
        }

        public bool IsOxygenLeaking()
        {
            return HealthRatio <= MySmallShipConstants.SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MIN;
        }

        private float GetOxygenLeakingAmount()
        {
            float damageRatioForOxygenLeaking = (MySmallShipConstants.SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MIN - HealthRatio) / (MySmallShipConstants.SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MIN - MySmallShipConstants.SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MAX);
            float oxygenLeft = MySmallShipConstants.OXYGEN_LEFT_AT_MIN_DAMAGE_LEVEL - (MySmallShipConstants.OXYGEN_LEFT_AT_MIN_DAMAGE_LEVEL - MySmallShipConstants.OXYGEN_LEFT_AT_MAX_DAMAGE_LEVEL) * damageRatioForOxygenLeaking;
            float oxygenLeaking = 1f / oxygenLeft;
            return oxygenLeaking * MaxOxygen;
        }

        public bool HasLowFuelLevel()
        {
            return Fuel <= MaxFuel * MySmallShipConstants.WARNING_FUEL_LOW_LEVEL;
        }

        public bool HasCriticalFuelLevel()
        {
            return Fuel <= MaxFuel * MySmallShipConstants.WARNING_FUEL_CRITICAL_LEVEL;
        }

        public bool HasNoFuelLevel()
        {
            return Fuel <= 0f;
        }

        private bool HasMountedAnyWeapon()
        {
            return Weapons.GetNormalWeaponCount(null) > 0;
        }

        public bool HasLowAmmoLevel()
        {
            if (HasMountedAnyWeapon())
            {
                return GetTheLowestLevelOfPrimaryAndSecondaryAmmo() <= MySmallShipConstants.WARNING_AMMO_LOW_LEVEL;
            }
            return false;
        }

        public bool HasCriticalAmmoLevel()
        {
            if (HasMountedAnyWeapon())
            {
                return GetTheLowestLevelOfPrimaryAndSecondaryAmmo() <= MySmallShipConstants.WARNING_AMMO_CRITICAL_LEVEL;
            }
            return false;
        }

        public bool HasNoAmmoLevel()
        {
            if (HasMountedAnyWeapon())
            {
                return GetTheLowestLevelOfPrimaryAndSecondaryAmmo() <= 0f;
            }
            return false;
        }

        private float GetTheLowestLevelOfPrimaryAndSecondaryAmmo()
        {
            return Math.Min(GetLevelOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary), GetLevelOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary));
        }

        private float GetLevelOfAmmo(MyMwcObjectBuilder_FireKeyEnum fireKey)
        {
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = Weapons.AmmoAssignments.GetAmmoType(fireKey);
            float maxAmount = 1;
            if (ammoType != 0)
            {
                maxAmount = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)ammoType, Faction).MaxAmount;
            }

            int amount = Weapons.GetAmountOfAmmo(fireKey);
            return amount / maxAmount;
        }

        public bool HasLowArmorLevel()
        {
            return ArmorHealth <= MaxArmorHealth * MySmallShipConstants.WARNING_ARMOR_LOW_LEVEL;
        }

        public bool HasCriticalArmorLevel()
        {
            return ArmorHealth <= MaxArmorHealth * MySmallShipConstants.WARNING_ARMOR_CRITICAL_LEVEL;
        }

        public bool HasNoArmorLevel()
        {
            return ArmorHealth <= 0f;
        }

        public bool HasLowHealthLevel()
        {
            return MySession.Static.Player.Health <= MySession.Static.Player.MaxHealth * MySmallShipConstants.WARNING_HEALTH_LOW_LEVEL;
        }

        public bool HasNoHealthLevel()
        {
            return MySession.Static.Player.Health <= 0;
        }

        public bool HasCriticalHealthLevel()
        {
            return MySession.Static.Player.Health <= MySession.Static.Player.MaxHealth * MySmallShipConstants.WARNING_HEALTH_CRITICAL_LEVEL;
        }

        public bool IsRadarJammed()
        {
            if (this == MyGuiScreenGamePlay.Static.ControlledEntity)
            {
                return MyRadar.IsNearRadarJammer();
            }
            return false;
        }

        public bool IsEnemyDetected()
        {
            if (this == MyGuiScreenGamePlay.Static.ControlledEntity)
            {
                MyRadar.GetDetectedObjects(ref m_detectedObjects);
                foreach (IMyObjectToDetect detectedObject in m_detectedObjects)
                {
                    if (detectedObject is MySmallShipBot &&
                       MyFactions.GetFactionsRelation(((MySmallShipBot)detectedObject), this) == MyFactionRelationEnum.Enemy)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsMissileDetected()
        {
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds < m_lastIdestructibleAsteroidWarningShowTime + 50)
            {
                return m_lastMissileDetectionStatus;
            }

            m_lastIdestructibleAsteroidWarningShowTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            MyRadar.GetDetectedObjects(ref m_detectedObjects);

            BoundingSphere playerSphere = new BoundingSphere(this.GetPosition(), 100);

            foreach (IMyObjectToDetect detectedObject in m_detectedObjects)
            {
                MyMissile missile = detectedObject as MyMissile;
                if (missile != null)
                {
                    Ray missileRay = new Ray(missile.GetPosition(), missile.GetOrientation().Forward);

                    if (!missileRay.Intersects(playerSphere).HasValue) continue;

                    if (missile.OwnerEntity == null)
                    {
                        if (MyFactions.GetFactionsRelation(missile.Faction, this.Faction) == MyFactionRelationEnum.Enemy)
                        {
                            m_lastMissileDetectionStatus = true;
                            return m_lastMissileDetectionStatus;
                        }
                    }
                    else if (missile.OwnerEntity != MySession.PlayerShip)
                    {
                        if (MyFactions.GetFactionsRelation(missile.Faction, this.Faction) == MyFactionRelationEnum.Enemy)
                        {
                            m_lastMissileDetectionStatus = true;
                            return m_lastMissileDetectionStatus;
                        }
                    }

                }
            }
            m_lastMissileDetectionStatus = false;
            return false;
        }

        private float m_lastIdestructibleAsteroidWarningShowTime = 0;

        private bool m_lastMissileDetectionStatus = false;

        private bool IsShipDamageDetected(int level)
        {
            switch (level)
            {
                case 0:
                    return HealthRatio <= 0.5f && HealthRatio > 0.4f;
                case 1:
                    return HealthRatio <= 0.4f && HealthRatio > 0.3f;
                case 2:
                    return HealthRatio <= 0.3f && HealthRatio > 0.2f;
                case 3:
                    return HealthRatio <= 0.2f && HealthRatio > 0.1f;
                case 4:
                    return HealthRatio <= 0.1f && HealthRatio > 0;
                default:
                    return false;
            }
        }

        private void PlayLowHealthAlertSound()
        {
            if (HealthRatio <= 0 && m_actualAlertCue.HasValue)
            {
                m_actualAlertCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            for (int i = m_lowHealthAlertSounds.Count - 1; i >= 0; i--)
            {
                if (IsShipDamageDetected(i))
                {
                    if (m_actualAlertCue == null)
                    {
                        m_actualAlertCue = MyAudio.AddCue2D(m_lowHealthAlertSounds[i]);
                        if (!m_actualAlertCue.HasValue) continue;
                    }
                    if (m_actualAlertCue.Value.CueEnum != m_lowHealthAlertSounds[i])
                    {
                        m_actualAlertCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                        m_actualAlertCue = MyAudio.AddCue2D(m_lowHealthAlertSounds[i]);
                    }
                    break;
                }
            }
        }

        private MySoundCue? m_actualAlertCue;

        private List<MySoundCuesEnum> m_lowHealthAlertSounds = new List<MySoundCuesEnum>(5);

        #endregion

        bool IsDisabledByEMP()
        {
            return m_disabledByEMPDuration > 0;
        }

        /// <summary>
        /// Disables the fire keys for the specified duration.
        /// </summary>
        void DisableByEMP(TimeSpan duration)
        {
            m_disabledByEMPDuration += (float)duration.TotalMilliseconds;
        }

        public bool IsEngineWorking()
        {
            return IsEngineTurnedOn() && !IsDisabledByEMP();
        }

        public bool IsEngineOn()
        {
            return m_engineForce != Vector3.Zero || m_engineTorque != Vector3.Zero;
        }

        public bool IsEngineTurnedOn()
        {
            return Config.Engine.On;
        }

        public bool IsEngineTurnedOff()
        {
            return !IsEngineTurnedOn();
        }

        public bool IsMovementSlowdownDisabled()
        {
            return !Config.MovementSlowdown.On;
        }

        public bool HasRadarJammerActive()
        {
            return m_hasRadarJammer && Config.RadarJammer.On;
        }

        public bool HasNoOxygen()
        {
            return Oxygen <= MyMwcMathConstants.EPSILON;
        }

        public bool HasNoFuel()
        {
            return Fuel <= MyMwcMathConstants.EPSILON;
        }

        /// <summary>
        /// Tries fill up fuel tank from inventory
        /// </summary>
        public void PumpFuel()
        {
            if (m_engineTypeProperties.FuelType.HasValue)
            {
                var fuelType = m_engineTypeProperties.FuelType;
                float maxAmountToPump = Inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.Ore, (int)fuelType);

                const float fuelEffeciency = 250.0f; // 1.0 means that 1 unit of material adds 1 fuel.

                float fuelToPump = Math.Min((MaxFuel - Fuel) / fuelEffeciency, maxAmountToPump);
                Fuel += fuelToPump * fuelEffeciency;
                Inventory.RemoveInventoryItemAmount(MyMwcObjectBuilderTypeEnum.Ore, (int)fuelType, fuelToPump);
            }
        }

        public MySmallShipEngineTypeProperties GetEngineProperties()
        {
            return (Fuel <= 0) ? MySmallShipEngineTypeConstants.ShipWithoutEngineProperties : m_engineTypeProperties;
        }

        /// <summary>
        /// Use only for debug, not for actual engine properties. For those, use GetEngineProperties() method.
        /// </summary>
        public MySmallShipEngineTypeProperties EnginePropertiesForDebug
        {
            get { return m_engineTypeProperties; }
        }

        public Vector3 GetFormationPosition(int index)
        {
            float lag_distance = WorldVolume.Radius * 2;

            int sideIndex = index / 2 + 1;
            int sideSign = index % 2 * 2 - 1;   // just -1 is left and +1 right

            return GetPosition() + WorldMatrix.Left * sideIndex * sideSign * MyAIConstants.FORMATION_SPACING - WorldMatrix.Forward * sideIndex * lag_distance;
        }

        private void SortFormation()
        {
            FollowersBuffer.Clear();
            FollowersBuffer.AddRange(Followers);

            Followers.Clear();
            while (FollowersBuffer.Count != Followers.Count)
            {
                float closestDistance = float.MaxValue;
                MySmallShipBot closestBot = null;

                Vector3 position = GetFormationPosition(Followers.Count);

                for (int i = 0; i < FollowersBuffer.Count; i++)
                {
                    var bot = FollowersBuffer[i];
                    Vector3 delta = bot.GetPosition() - position;
                    Vector3 projected = delta - Vector3.Dot(delta, WorldMatrix.Forward) * WorldMatrix.Forward;  // Project on plane defined by leader forward vector - better results

                    float distance = projected.Length() - (i == Followers.Count ? 10 : 0);  // Stabilization - if some bot is already heading to some position, make it harder to change target
                    if (distance < closestDistance && !Followers.Contains(bot))
                    {
                        closestDistance = distance;
                        closestBot = bot;
                    }
                }

                Debug.Assert(closestBot != this);
                Followers.Add(closestBot);
            }
        }

        public override Vector3 GetFormationPosition(MySmallShipBot bot)
        {
            int followerIndex = Followers.IndexOf(bot);
            if (followerIndex < 0)
            {
                System.Diagnostics.Debug.Fail("GetFormationPosition - Follower not found!");
                return bot.GetPosition();
            }

            return GetFormationPosition(followerIndex);
        }

        public virtual void AtackedByEnemy(MyEntity damageSource)
        {

        }

        protected bool IsFriendlyFire(MyEntity damageSource)
        {
            var smallShipLeader = Leader as MySmallShip;

            if (damageSource == null)
            {
                return false;
            }

            if (damageSource == this)
            {
                return true;
            }

            // Group leader atacked by his follower?
            foreach (var follower in Followers)
            {
                if (follower == damageSource)
                {
                    return true;
                }
            }

            if (Leader != null)
            {
                // Atacked by leader?
                if (damageSource == Leader)
                {
                    return true;
                }
                else if (smallShipLeader != null)
                {
                    // Atacked by another follower from same group?
                    foreach (var follower in smallShipLeader.Followers)
                    {
                        if (follower == damageSource)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected void HandleAttack(MyEntity damageSource)
        {
            bool friendlyfire = IsFriendlyFire(damageSource);
            bool isSmallShip = damageSource is MySmallShip;
            if (damageSource != null && !friendlyfire)
            {
                MyFactionRelationEnum factionRelation = MyFactions.GetFactionsRelation(this, damageSource);
                if (factionRelation == MyFactionRelationEnum.Friend)
                {
                    return;
                }

                if (factionRelation == MyFactionRelationEnum.Neutral)
                {
                    return;
                }

                var smallShipLeader = Leader as MySmallShip;
                if (smallShipLeader != null)
                {
                    // follower - notify whole group about attack
                    smallShipLeader.AtackedByEnemy(damageSource);
                    foreach (var follower in smallShipLeader.Followers)
                    {
                        follower.AtackedByEnemy(damageSource);
                    }
                }
                else
                {
                    AtackedByEnemy(damageSource);

                    // if this bot is leader, notify also followers about attack
                    foreach (var follower in Followers)
                    {
                        follower.AtackedByEnemy(damageSource);
                    }
                }
            }
        }

        public bool IsAfterburnerOn()
        {
            return m_engineAfterburnerOn;
        }

        void OpeningInventoryScreen(MyEntity entity, MySmallShipInteractionActionEnum interactionAction)
        {
            if (entity == this && interactionAction == MySmallShipInteractionActionEnum.Loot)
            {
                m_isLooted = true;
            }
        }

        void InventoryScreenClosed(MyGuiScreenBase screen)
        {
            if (m_isLooted)
            {
                m_isLooted = false;
                m_lastLootTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        public void HideFromBots()
        {
            Config.Engine.SetOff();
            Config.ReflectorLight.SetOff();
        }

        public bool IsHiddenFromBots()
        {
            return !Config.Engine.On && !Config.ReflectorLight.On;
        }
        #endregion

        #region Overrides of MyEntity

        public override void RepairToMax()
        {
            base.RepairToMax();
            ArmorHealth = MaxArmorHealth;
            Fuel = MaxFuel;
            Oxygen = MaxOxygen;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShip.UpdateBeforeSimulation");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip part 1");

            //Physics.MaxAngularVelocity
            bool isPlayerShip = this == MySession.PlayerShip;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SortFormation");
            if (Followers.Count > 1)
            {
                SortFormation();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddRoutePoint");

            m_lod1ForcedDistance = MySmallShipConstants.MAX_UPDATE_DISTANCE;

            AddRoutePoint();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("isPlayerShip collision");
            if (isPlayerShip)
            {
                if (m_hasCollided)
                {
                    Vector3 newVel = Physics.LinearVelocity * (1 - m_collisionFriction * MySmallShipConstants.COLLISION_FRICTION_MULTIPLIER);
                    System.Diagnostics.Debug.Assert((newVel.Length() <= Physics.LinearVelocity.Length()));
                    Physics.LinearVelocity = newVel;
                    m_collisionFriction = 0.0f;
                }

                RenderObjects[0].ShadowCastUpdateInterval = 60;

                if (MySession.Is25DSector)
                    Config.AutoLeveling.SetOn();
                  /*
                if (MyFakes.MWBUILDER)
                    Config.AutoLeveling.SetOn();
                */
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            m_hasCollided = false;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip base UpdateBeforeSimulation");
            base.UpdateBeforeSimulation();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip engine, gameplay");
            var activeEngineProps = GetEngineProperties();
            if (isPlayerShip || !MyFakes.OPTIMIZATION_FOR_300_SMALLSHIPS)
            {
                UpdateGamePlayProperties(isPlayerShip, activeEngineProps);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip stats update");

            // Pump up fuel from inventory
            if (isPlayerShip || !MyFakes.OPTIMIZATION_FOR_300_SMALLSHIPS)
            {
                m_timeToPumpFuel -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                if (m_timeToPumpFuel <= 0)
                {
                    PumpFuel();
                    m_timeToPumpFuel = 1.0f;
                }
            }

            // shorten remaining EMP disable
            if (m_disabledByEMPDuration >= 0)
            {
                m_disabledByEMPDuration = m_disabledByEMPDuration - MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            }

            // Increase hud indicator distance based on current player needs
            if (isPlayerShip)
            {
                const float needMultipler = 4f;

                if (HasLowFuelLevel())
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxFuel] = needMultipler;
                else
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxFuel] = 1;

                if (HasLowOxygenLevel())
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxOxygen] = needMultipler;
                else
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxOxygen] = 1;

                if (IsCriticalDamaged() || HasLowArmorLevel())
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxRepair] = needMultipler;
                else
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxRepair] = 1;

                if (HasLowAmmoLevel())
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxAmmo] = needMultipler;
                else
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxAmmo] = 1;

                if (HasLowHealthLevel())
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxMedkit] = needMultipler;
                else
                    MyHud.HudMaxDistanceMultiplers[(int)MyHudMaxDistanceMultiplerTypes.CargoBoxMedkit] = 1;
            }

            if (isPlayerShip /* && IsCameraInsideMinerShip()*/)
            {
                UpdatePlayerHeadAndShakingNew();
            }

            m_engineVolume = Config.Engine.Level;
            // i hope that this is enought to remove unnecessary memory allocation
            if (MySession.PlayerShip == this)
            //if (this.GetObjectBuilderInternal(false) is MyMwcObjectBuilder_SmallShip_Player)
            {
                m_engineVolume -= MyAudioConstants.PLAYER_SHIP_ENGINE_VOLUME_DECREASE_AMOUNT;
            }

            m_lockTargetCheckTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            if (m_lockTargetCheckTime > MySmallShipConstants.LOCK_TARGET_CHECK_TIME && this == MySession.PlayerShip)
            {
                m_lockTargetCheckTime = 0;
                AcquireTarget();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip engine + forces");

            Config.Update();

            Matrix tempInvertedWorldMatrix = GetWorldMatrixInverted();
            tempInvertedWorldMatrix.Translation = Vector3.Zero;
            Vector3 localVelocity = MyUtils.GetTransform(Physics.LinearVelocity, ref tempInvertedWorldMatrix);
            Vector3 localAngularVelocity = MyUtils.GetTransform(Physics.AngularVelocity, ref tempInvertedWorldMatrix);

            //  Slowdown movement and rotation
            var physicsProperties = GetPhysicsProperties();
            var maxSpeedMultiplierDecelerate = MAGIC_CONSTANT_FOR_MAX_SPEED / activeEngineProps.MaxSpeed;
            Vector3 slowdownForce = Vector3.Zero;
            if (Config.MovementSlowdown.On)
            {
                slowdownForce -= localVelocity * activeEngineProps.Force * maxSpeedMultiplierDecelerate * physicsProperties.MultiplierMovement;
            }

            // Slowdown movement over a fixed velocity (e.g. 120 m/s) even if engine slowdown is off
            // let's pretend it dissipates as heat or something
            if ((!Config.MovementSlowdown.On || !Config.Engine.On) && localVelocity.Length() > activeEngineProps.MaxSpeed)
            {
                slowdownForce -= localVelocity * activeEngineProps.Force * maxSpeedMultiplierDecelerate * physicsProperties.MultiplierMovement;
            }

            m_engineTorque -= localAngularVelocity * Physics.Mass * physicsProperties.MultiplierRotationDecelerate;

            //  Stabilization of angle around axis Z
            if (Config.AutoLeveling.On)
            {
                Vector3 cameraRight = WorldMatrix.Right;
                Vector3 horizontal = m_levelingMatrix.Up;
                float angle = (MyUtils.GetAngleBetweenVectors(cameraRight, horizontal) - MathHelper.PiOver2) * -1;

                m_engineTorque.Z -= angle * Physics.Mass * physicsProperties.MultiplierHorizontalAngleStabilization * (MySession.Is25DSector && this == MySession.PlayerShip ? 5 : 1);
            }
            
            MyUtils.AssertIsValid(m_engineTorque);
            MyUtils.AssertIsValid(m_engineForce);

            //  Prepare force/torque for update controller
            if (IsEngineWorking())
            {
                if (MySession.Is25DSector)
                {
                    m_engineForce.Y = 0;
                    slowdownForce.Y = 0;
                }

                if (MyFakes.MWBUILDER)
                {
                    //m_engineForce.Y = -500000;
                    this.Physics.PlayCollisionCueEnabled = false;
                }
              

                this.Physics.AddForce(MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE, m_engineForce, null, m_engineTorque);
                this.Physics.AddForce(MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE, slowdownForce, null, null);
            }
            else
                this.Physics.AddForce(MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE, null, null, null);  // don't add any forces with engine off (not even slowdown)

            //  We can reset this temporary forces. They are used in MoveAndRotate too, that is the place where they are filled first time in each Update()
            m_engineForce = Vector3.Zero;
            m_engineTorque = Vector3.Zero;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip dead tests");

            if (IsDead() || IsPilotDead())
            {
                if (isPlayerShip && m_pilotDeathTime == 0)
                {
                    if (MyMultiplayerGameplay.IsStory())
                    {
                        if (MyMultiplayerGameplay.IsRunning)
                        {
                            if (MyMultiplayerGameplay.Static.IsHost)
                            {
                                bool anyAlive = false;
                                foreach (var player in MyMultiplayerGameplay.Peers.Players)
                                {
                                    if (player.Ship != null && !player.Ship.IsDead())
                                    {
                                        anyAlive = true;
                                    }
                                }

                                if (!anyAlive)
                                {
                                    Gameover();
                                }
                            }
                        }
                        else
                        {
                            Gameover();
                        }
                    }

                    MyDialogues.Stop();

                    MyGuiScreenGamePlay.Static.CameraAttachedTo = MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic;
                    MyHud.EnableDrawingMissionObjectives = false;
                    StopShipDetectors();
                }

                // Ship is destroyed with alive pilot or pilot is dead for specified amount of time (dont explode if ship is looted and right after looting)
                if (!IsExploded() && (!IsPilotDead() || base.IsDead() ||
                    (m_pilotDeathTime > MyGameplayConstants.SHIP_WITHOUT_PILOT_DESTRUCTION_TIME &&
                     !WasRecentlyLooted())))
                {
                    StopSounds();

                    m_isExploded = true;
                    //  small ship explosion!
                    MyExplosion explosion = MyExplosions.AddExplosion();
                    Vector3 explosionPosition = GetPosition();
                    if (explosion != null)
                    {
                        explosion.Start(0, WorldVolume.Radius, 0,
                                        MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION,
                                        new BoundingSphere(explosionPosition,
                                                           MyMwcUtils.GetRandomFloat(
                                                               MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MIN,
                                                               MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MAX)),
                                        MyExplosionsConstants.EXPLOSION_LIFESPAN, 0, this);
                    }
                    if (!isPlayerShip)
                    {
                        MarkForClose();
                    }
                    else
                    {
                        Activate(false, false);
                    }
                }
            }
            else
            {
                MyHud.EnableDrawingMissionObjectives = true;
            }

            if (IsPilotDead())
            {
                m_pilotDeathTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip playership upd");

            if (isPlayerShip)
            {                
                Update_PlayerShip();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip 2.5D + tools");

            if (MySession.Is25DSector)
            {                       
                var worldMatrix = this.WorldMatrix;
                var position = worldMatrix.Translation;
                position.Y = 0;
                worldMatrix.Translation = position;
                worldMatrix.Up = Vector3.Up;
                worldMatrix.Forward = Vector3.Cross(worldMatrix.Left, worldMatrix.Up);

                //worldMatrix = Matrix.Identity;


                worldMatrix.Translation = position;

                this.WorldMatrix = worldMatrix;
            }                         

            if (m_nanoRepairTool != null)
            {
                m_nanoRepairTool.Use();
            }

            if (HackingTool != null)
            {
                HackingTool.Update();
            }

            Debug.Assert(RadioactivityAmount >= 0, "Radioactivity influence cannot be negative!");
            if (RadioactivityAmount > MyMwcMathConstants.EPSILON)
            {
                this.DoDamage(RadioactivityAmount * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, 0, 0, MyDamageType.Radioactivity, MyAmmoType.None, null);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void Gameover()
        {
            if (MyMissions.ActiveMission != null)
            {
                MyMissions.ActiveMission.Fail();
            }
            else
            {
                MySession.Static.GameOver();
            }
        }

        public virtual MyShipTypePhysicsProperties GetPhysicsProperties()
        {
            return m_shipTypeProperties.Physics;
        }

        private MySmallShipEngineTypeProperties UpdateGamePlayProperties(bool isPlayerShip, MySmallShipEngineTypeProperties activeEngineProps)
        {
            // Modify oxygen consuption according to game difficulty
            float oxygenConsumption = isPlayerShip ?
                MyGameplayConstants.GameplayDifficultyProfile.PlayerOxygenConsumptionMultiplicator : 1;

            if (isPlayerShip && MySession.Static.Player.Medicines[(int)MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE].IsActive())
                oxygenConsumption *= MyMedicineConstants.PERFORMANCE_ENHANCING_MEDICINE_OXYGEN_CONSUMPTION_MULTIPLIER;

            Oxygen = Math.Max(0, m_oxygen - oxygenConsumption * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            if (IsOxygenLeaking())
            {
                Oxygen = Math.Max(0, m_oxygen - GetOxygenLeakingAmount() * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            }

            if (MySession.Static != null)
            {
                var player = MySession.Static.Player;
                if (isPlayerShip)
                {
                    if (Oxygen <= 0)
                    {
                        player.TimeWithoutOxygen += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    }
                    else
                    {
                        player.TimeWithoutOxygen = 0;
                    }

                    // Reduce player health after 30 seconds without oxygen
                    if (player.TimeWithoutOxygen > 30)
                    {
                        if (!player.IsDead())
                        {
                            player.AddHealth(-MyMinerShipConstants.MINER_SHIP_PLAYER_NO_OXYGEN_HEALTH_LOSS * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, null);

                            if (MyMultiplayerGameplay.IsRunning && IsPilotDead())
                            {
                                MyMultiplayerGameplay.Static.PilotDie(this, null);
                            }
                        }
                    }
                }
            }


            // Update amount of Fuel
            float fuelConsumption = 0;
            if (IsEngineWorking())
            {
                if (IsEngineOn())
                {
                    fuelConsumption = (m_engineAfterburnerOn ? activeEngineProps.AfterburnerConsumptionMultiplier : 1.0f) * activeEngineProps.FuelConsumption;

                    var drill = Weapons.GetMountedDrill();
                    if (drill != null && drill.CurrentState == MyDrillStateEnum.Drilling)
                    {
                        fuelConsumption += MySmallShipConstants.DRILL_FUEL_CONSUMPTION;
                    }

                    var harvester = Weapons.GetMountedHarvestingDevice();
                    if (harvester != null && harvester.IsHarvesterActive)
                    {
                        fuelConsumption += MySmallShipConstants.HARVESTER_FUEL_CONSUMPTION;
                    }
                }
                else
                {
                    // 5% fuel consuption if engine is not active and is on
                    fuelConsumption = activeEngineProps.FuelConsumption * 0.05f;
                }
            }

            if (isPlayerShip)
            {
                fuelConsumption *= MyGameplayConstants.GameplayDifficultyProfile.PlayerFuelConsumptionMultiplicator;
            }

            Fuel = Math.Max(0, Fuel - fuelConsumption * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            return activeEngineProps;
        }

        public void WeaponShot(MySmallShipGunBase gun, MyAmmoAssignment ammoAssignment)
        {
            //Electricity -= MySmallShipConstants.WEAPON_ELECTRICITY_CONSUMPTION;
        }

        private void Update_PlayerShip()
        {
            if (IsDead() || IsPilotDead())
            {
                return;
            }

            UpdateSecondaryCamera();

            UpdateFriendlyFireDamage();

            int updatePlBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip_Update_PlayerShip", ref updatePlBlock);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip_Update_PlayerShip trade test");

            UpdateSubObjectsState(IsCameraInsideMinerShip());

            bool cameraAttachedToPlayer = MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip ||
               MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic ||
               MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing ||
               MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic;
            bool canBeDetectorsActive = MyGuiScreenGamePlay.Static.IsGameActive() &&
                (MyFakes.ENABLE_DETECTORS_IN_EDITOR_GAME || (
                MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_MMO &&
                MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_STORY &&
                MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_SANDBOX));



            // Trade detector
            if (cameraAttachedToPlayer)
            {
                if (TradeDetector.TrySetStatus(true))
                    TradeDetector.ActivateSensor();
            }
            else
            {
                TradeDetector.TrySetStatus(false);
            }

            // other detectors
            if (cameraAttachedToPlayer && canBeDetectorsActive)
            {
                if (MyFakes.ENABLE_BUILDER_MODE)
                {
                    if (BuildDetector.TrySetStatus(true))
                        BuildDetector.ActivateSensor();
                }
                if (MotherShipDetector.TrySetStatus(true))
                    MotherShipDetector.ActivateSensor();
                if (UseableEntityDetector.TrySetStatus(true))
                    UseableEntityDetector.ActivateSensor();
            }
            else
            {
                BuildDetector.TrySetStatus(false);
                MotherShipDetector.TrySetStatus(false);
                UseableEntityDetector.TrySetStatus(false);
            }

            //// Detecting nearest friend or neutral ship to trade
            //if (MyGuiScreenGamePlay.Static.IsGameActive() &&
            //    (MyFakes.ENABLE_DETECTORS_IN_EDITOR_GAME || (
            //    MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_MMO &&
            //    MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_STORY &&
            //    MyGuiScreenGamePlay.Static.GetPreviousGameType() != MyGuiScreenGamePlayType.EDITOR_SANDBOX)) &&
            //    (MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip ||
            //     MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic ||
            //     MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing ||
            //     MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic))
            //{
            //    if (TradeDetector.TrySetStatus(true))
            //        TradeDetector.ActivateSensor();
            //    if (BuildDetector.TrySetStatus(true))
            //        BuildDetector.ActivateSensor();
            //    if (MotherShipDetector.TrySetStatus(true))
            //        MotherShipDetector.ActivateSensor();
            //    if (UseableEntityDetector.TrySetStatus(true))
            //        UseableEntityDetector.ActivateSensor();
            //}
            //else
            //{

            //}

            if (m_travelNotification == null && m_nearMotherShipContainer != null && MyGuiScreenGamePlay.Static.CanTravel)
            {
                m_travelNotification = DisplayDetectedEntityActionNotification(m_nearMotherShipContainer, MySmallShipInteractionActionEnum.Travel);
            }
            else if (m_travelNotification != null && (m_nearMotherShipContainer == null || !MyGuiScreenGamePlay.Static.CanTravel))
            {
                m_travelNotification.Disappear();
                m_travelNotification = null;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SmallShip_Update_PlayerShip radar");



            //if (MyHudConstants.RADAR_DATA_SHARING_WITH_FRIENDLY_RADARS || this == MySession.PlayerShip) 
            //{
            //    ShipRadar.UdpdateTime(MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);
            //}

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SmallShip_Update_PlayerShip mission or neutral");

            if (ToolKits != null)
            {
                ToolKits.Update();
            }

            // Detecting nearest mission or neutral ship to trade
            if (MyMissions.ActiveMission == null)
            {
                m_lastMissionProximityCheck += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_lastMissionProximityCheck >= MySmallShipConstants.DETECT_INTERVAL)
                {
                    MyMissions.CheckMissionProximity();
                    m_lastMissionProximityCheck = 0f;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SmallShip_Update_PlayerShip regen");

            // Regen gameplay properties if near friendly Mother Ship
            if (MyFakes.ENABLE_REFILL_PLAYER_IN_MOTHERSHIP &&
                GetNearMotherShipContainer() != null &&
                MyFactions.GetFactionsRelation(GetNearMotherShipContainer(), this) == MyFactionRelationEnum.Friend)
            {
                float regenRate = 1.0f / 5.0f * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                Health += MaxHealth * regenRate;
                ArmorHealth += MaxArmorHealth * regenRate;
                Oxygen += MaxOxygen * regenRate;
                Fuel += MaxFuel * regenRate;

                Health = MathHelper.Clamp(Health, 0, MaxHealth);
                ArmorHealth = MathHelper.Clamp(ArmorHealth, 0, MaxArmorHealth);
                Oxygen = MathHelper.Clamp(Oxygen, 0, MaxOxygen);
                Fuel = MathHelper.Clamp(Fuel, 0, MaxFuel);

                MySession.Static.Player.AddHealth(MySession.Static.Player.MaxHealth * regenRate, null);
            }

            // handle medikit
            {
                var medikit = MySession.Static.Player.Medicines[(int)MyMedicineType.MEDIKIT];

                // medikit has an effect over time: handle it here
                if (medikit.WasActiveSinceLastTriggered())
                {
                    MySession.Static.Player.AddHealth(medikit.ActiveTimeSinceLastTriggered() * 0.001f * MyMedicineConstants.MEDIKIT_HEALTH_RESTORED_PER_SECOND, null);
                    medikit.Trigger();
                }

                // player low on health: started medikit automatically
                if (!medikit.IsActive() && MySession.Static.Player.Health <= MyMedicineConstants.MEDIKIT_HEALTH_TO_ACTIVATE)
                {
                    medikit.ActivateIfInInventory(Inventory);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SmallShip_Update_PlayerShip enemy sounds");

            int enemiesDetectedCount = MyHud.GetHudEnemiesCount(MySmallShipConstants.WARNING_ENEMY_ALERT_MAX_DISTANCE_SQR);

            if (m_enemiesCountDetectedLastUpdate == 0 && enemiesDetectedCount > 0)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.HudEnemyAlertWarning);
            }
            m_enemiesCountDetectedLastUpdate = enemiesDetectedCount;

            //if (Fuel <= 0 || !Config.Engine.On)
            //{
            //    Config.ReflectorLight.SetValue(false);
            //}

            UpdateAfterburner();

            if (enemiesDetectedCount > 0)
            {
                m_lastEnemyDetection = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastEnemyDetection < 5000)
            {
                MyAudio.ApplyTransition(MyMusicTransitionEnum.LightFight, 2);
            }
            else
            {
                MyAudio.StopTransition(2);
            }

            PlayLowHealthAlertSound();

            GlassDirtLevel = MAX_GLASS_DIRT_LEVEL - (MaxHealth > 0f ? ((MAX_GLASS_DIRT_LEVEL - MIN_GLASS_DIRT_LEVEL) * HealthRatio) : 0f);

            UpdateSectorBoundariesWarning();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(updatePlBlock);
        }

        /// <summary>
        /// When player is too close to sector safe boundaries, we display this notification text
        /// </summary>
        void UpdateSectorBoundariesWarning()
        {
            float dist = MyGuiScreenGamePlay.Static.GetDistanceToSectorBoundaries();

            if (dist <= MyHudConstants.DISTANCE_FOR_SECTOR_BORDER_WARNING)
            {
                if (m_sectorBoundariesWarning != null)
                {
                    var text = m_sectorBoundariesWarning.Text;
                    text.Clear();
                    if (MyGuiScreenGamePlay.Static.CanTravelThroughSectorBoundaries())
                    {
                        //  Construct message: Sector border in XXX meters. Neighbouring sector will load when zero.
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SectorBorderBegin));
                        text.AppendInt32((int)dist);
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.SectorBorderEnd));
                    }
                    else if (!MyClientServer.LoggedPlayer.GetCanSave())
                    {
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.DemoUserCannotTravel));
                    }
                    else if (MyMultiplayerGameplay.IsRunning && !MyMultiplayerGameplay.Static.IsHost && MyGuiScreenGamePlay.Static.IsGameStoryActive())
                    {
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.MP_OnlyHostCanTravel));
                    }
                    else if (MyGuiScreenGamePlay.Static.IsGameStoryActive())
                    {
                        //  Construct message: Finish the mission before leaving the sector
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.UnableToLeaveSectorMission));
                    }
                    else
                    {
                        //  Construct message: Unable to leave the sector
                        text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.UnableToLeaveSector));
                    }
                }
            }
        }

        private void UpdateAfterburner()
        {
            if (m_engineAfterburnerOn)
            {
                AfterburnerStatus -= (m_lastAfterburnerEmptying != 0) ? ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastAfterburnerEmptying) / m_shipTypeProperties.GamePlay.AfterBurnerDurationTime) * 0.001f : 0;
                if (AfterburnerStatus < 0) AfterburnerStatus = 0.0f;
                m_lastAfterburnerEmptying = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_lastAfterburnerFilling = 0;
            }
            else if (m_engineAfterburnerCanRefill)
            {
                AfterburnerStatus += (m_lastAfterburnerFilling != 0) ? ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastAfterburnerFilling) / m_shipTypeProperties.GamePlay.AfterBurnerRefillTime) * 0.001f : 0;
                if (AfterburnerStatus > 1) AfterburnerStatus = 1.0f;
                m_lastAfterburnerFilling = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_lastAfterburnerEmptying = 0;
            }
        }



        public override void UpdateAfterSimulation()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration base");
            base.UpdateAfterSimulation();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration");


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Distance test");

            float distance = Vector3.Distance(MyCamera.Position, WorldMatrix.Translation);
            float maxDistanceForUpdate = MySession.Is25DSector ? 500 : 100;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            //  Update guns
            if (Weapons != null)
            {
                Weapons.UpdateAfterSimulation();
            }

            if (distance < maxDistanceForUpdate)
            {
              
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration cockpit");

                if (this == MySession.PlayerShip)
                {
                    GetShipCockpit().UpdateBeforeSimulation();
                    GetShipCockpitGlass().UpdateBeforeSimulation();
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration physics");

                if (this.Physics != null)
                {
                    if (MyGuiScreenGamePlay.Static.IsGameActive() || MyGuiScreenGamePlay.Static.IsFlyThroughActive())
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration sounds");

                        if (m_engineSoundOn)
                        {
                            //  Engine moving sound
                            if (m_engineMovingCue == null || !m_engineMovingCue.Value.IsPlaying)
                            {
                                if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_engineIdleCueDelayMillis)
                                {
                                    m_engineMovingCue = MyAudio.AddCue2dOr3d(this, GetEngineProperties().HighCue3d,
                                                                             GetPosition(), WorldMatrix.Forward, WorldMatrix.Up,
                                                                             this.Physics.LinearVelocity, m_engineVolume);
                                }
                            }
                            else
                            {
                                MyAudio.UpdateCueVolume(m_engineMovingCue, m_engineVolume);
                                m_engineMovingCue.Value.SetVariable(MyCueVariableEnum.ShipASpeed,
                                                                             MathHelper.Clamp(Physics.Speed, 0,
                                                                                              SOUND_VARIABLE_SHIP_A_SPEED_MAX_VALUE));
                                MyAudio.UpdateCuePosition(m_engineMovingCue, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up,
                                                          Physics.LinearVelocity);
                            }

                            //  Engine idle sound
                            if (m_engineIdleCue == null || !m_engineIdleCue.Value.IsPlaying)
                            {
                                if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_engineIdleCueDelayMillis)
                                {
                                    m_engineIdleCue = MyAudio.AddCue2dOr3d(this, GetEngineProperties().IdleCue3d,
                                                                           GetPosition(),
                                                                           WorldMatrix.Forward, WorldMatrix.Up,
                                                                           Physics.LinearVelocity, m_engineVolume);
                                }
                            }
                            else
                            {
                                m_engineIdleCue.Value.SetVariable(MyCueVariableEnum.ShipAIdle,
                                                                           MathHelper.Clamp(Physics.Speed, 0,
                                                                                            SOUND_VARIABLE_SHIP_IDLE_SPEED_MAX_VALUE));
                                MyAudio.UpdateCueVolume(m_engineIdleCue, m_engineVolume);
                                MyAudio.UpdateCuePosition(m_engineIdleCue, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up,
                                                          Physics.LinearVelocity);
                            }

                            //  Engine afterburner sound
                            if (m_engineAfterburnerOn)
                            {
                                if (m_engineAfterburnerCue == null)
                                {
                                    if (Config.Engine.On)
                                    {
                                        m_engineAfterburnerCue = MyAudio.AddCue2dOr3d(this, GetEngineProperties().Thrust3d,
                                            GetPosition(),
                                                                                      WorldMatrix.Forward, WorldMatrix.Up,
                                                                                      Physics.LinearVelocity);
                                    }
                                }
                                if (m_engineAfterburnerCue != null)
                                {
                                    MyAudio.UpdateCuePosition(m_engineAfterburnerCue,
                                        GetPosition(), WorldMatrix.Forward,
                                                              WorldMatrix.Up, Physics.LinearVelocity);
                                }
                            }
                            else
                            {
                                if (m_engineAfterburnerCue != null)
                                {
                                    if (m_engineAfterburnerCue.Value.IsPlaying)
                                    {
                                        m_engineAfterburnerCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                                    }
                                    m_engineAfterburnerCue = null;
                                }
                            }
                        }
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration sounds2");

                        if (m_engineOnOffSound != null && !m_engineOnOffSound.Value.IsPlaying)
                        {
                            if (!IsEngineWorking())
                            {
                                if (m_playerBreathing == null)
                                {
                                    if ((m_engineIdleCue != null) && (m_engineIdleCue.Value.IsPlaying == true))
                                    {
                                        m_engineIdleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                                    }

                                    if ((m_engineMovingCue != null) && (m_engineMovingCue.Value.IsPlaying == true))
                                    {
                                        m_engineMovingCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                                    }

                                    // duck all sounds outside ship when respirator on
                                    //MyAudio.GetDefaultCategory().SetVolume(0.02f);
                                    m_playerBreathing = MyAudio.AddCue2D(MySoundCuesEnum.SfxPlayerBreath);
                                }
                                else
                                {
                                    // we cant allow breathing in non-player minerships
                                    if (IsCameraInsideMinerShip())
                                    {
                                        MyAudio.GetCockpitCategory().SetVolume(MyAudio.VolumeGame);
                                    }
                                    else
                                    {
                                        MyAudio.GetCockpitCategory().SetVolume(0.0f);
                                    }
                                }
                            }
                            else
                            {
                                //if (m_playerBreathing != null && m_playerBreathing.Value.IsStopping)
                                //{
                                //    MySounds.GetDefaultCategory().SetVolume(1.0f);
                                //}                        
                            }
                        }

                        if (m_playerBreathing != null && m_playerBreathing.Value.IsValid && m_playerBreathing.Value.IsStopping)
                        {
                            MyAudio.GetDefaultCategory().SetVolume(MyAudio.VolumeGame);
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


                    }
                    else
                    {
                        //turn off sounds of engine.. if possible
                        StopEngineSounds(SharpDX.XACT3.StopFlags.Immediate);
                        //StopSounds();
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Stop engine sounds");
                StopEngineSounds(SharpDX.XACT3.StopFlags.Immediate);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                //StopSounds();
            }


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration particles");

            if (m_damageEffect != null)
            {
                if (IsCameraInsideMinerShip())
                    m_damageEffect.UserBirthMultiplier = 0;
                else
                {
                    m_damageEffect.UserBirthMultiplier = GetDamageRatio() > MyShipConstants.DAMAGED_HEALTH
                                                          ? MathHelper.Clamp(0.1f * Physics.LinearVelocity.Length(), 0.0f, 2f)
                                                          : 0;
                    m_damageEffect.UserRadiusMultiplier = GetDamageRatio() > MyShipConstants.DAMAGED_HEALTH
                                                       ? MathHelper.Clamp(0.01f * Physics.LinearVelocity.Length(), 0.75f, 1.5f)
                                                       : 0;
                }

                m_damageEffect.WorldMatrix = m_damageEffectLocalMatrix != null ? m_damageEffectLocalMatrix.Value * WorldMatrix : WorldMatrix;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShip.UpdateAfterIntegration reflector");

            // TODO Only temporary until update / update anim / on worldpos solved
            //  Reflector short-long range
            if (m_reflectorProperies != null)
                m_reflectorProperies.Update();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update light");

            UpdateLight();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMultiplayerGameplay");
            UpdateMultiplayerPosition();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        protected virtual void UpdateMultiplayerPosition()
        {
            if (MyMultiplayerGameplay.IsRunning && !IsDummy)
            {
                MyMultiplayerGameplay.Static.UpdatePosition(this);
            }
        }

        /// <summary>
        /// Draws this object's model and return true. If object isn't in frustum or for whatever reason we won't draw it, return false.
        /// </summary>
        /// <returns></returns>
        public override bool Draw(MyRenderObject renderObject = null)
        {
            if (!MyFakes.DRAW_PLAYER_MINER_SHIP)
                return false;

            if (this == MySession.PlayerShip && /*IsRenderingInsidePlayerShip()*/
                MyGuiScreenGamePlay.IsRenderingInsideEntity(MySession.PlayerShip))
            {
                return false;
            }

            float distance = Vector3.Distance(this.GetPosition(), MyCamera.Position);

            //  If camera sits in the player ship, we won't draw the ship, reflector and guns.
            if (Render.MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0)
            {
                // TODO: Go ahead and do it in customization screen then!
                //ModelLod0.LoadInDraw(); //if we dont call it here, there will be no GPU data if ship is added through customization screen

                base.Draw(renderObject);

                if (distance < MySmallShipConstants.MAX_UPDATE_DISTANCE)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship thrusts");
                    DrawEngineThrusts();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    //  We need to draw reflectors always, because they aren't contained in ship's model bounding sphere
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship reflectors LOD0");
                    DrawReflectors();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }
            else if (Render.MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD1 && (ModelLod1 != null))
            {
                // TODO: Go ahead and do it in customization screen then!
                // ModelLod1.LoadInDraw(); //if we dont call it here, there will be no GPU data if ship is added through customization screen

                base.Draw();

                //  We need to draw reflectors always, because they aren't contained in ship's model bounding sphere
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship reflectors LOD1");
                DrawReflectors();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else if (Render.MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0 || MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD_NEAR)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship thrusts 2");
                DrawEngineThrusts();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if (Render.MyRender.GetCurrentLodDrawPass() != MyLodTypeEnum.LOD1)
            {
                bool drawGuns = true;
                if (this == MySession.PlayerShip)
                {
                    if (!MyGuiScreenGamePlay.Static.IsGameActive() || MyGuiScreenGamePlay.Static.IsEditorActive())
                    {
                        drawGuns = false;
                    }
                }

                if (MyRender.CurrentRenderSetup.CallerID.Value != MyRenderCallerEnum.Main)
                    drawGuns = false;

                drawGuns &= distance < MySmallShipConstants.MAX_UPDATE_DISTANCE;

                if (drawGuns)
                {
                    float maxDistanceForGuns = MyLodConstants.MAX_DISTANCE_FOR_DRAWING_MINER_SHIP_GUNS * (MySession.Is25DSector ?  2 : 1);

                    if (Vector3.Distance(MyCamera.Position, GetPosition()) <= maxDistanceForGuns && Children.Count > 0)
                    {
                        //  Do not draw guns if this is player. Reason is that in large positions guns aren't at precise positions and if viewed from the ship, user will note shakiness.
                        //  But for guns on other players it is not visible.
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Smallship weapons");
                        Weapons.Draw();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }

            //  We return true, but in this case it means nothing. Caller don't need it.
            return true;
        }

        /// <summary>
        /// If model of this phys object uses alpha for rendering, this method will calculate that alpha
        /// Of course childs of this class must implement it's own alpha calculation, because this one will throw an exception.
        /// Children shouldn't call this base method
        /// Only for drawing this object, because some objects need to use special world matrix
        /// </summary>
        public override void DebugDrawNormalVectors()
        {
            base.DebugDrawNormalVectors();

            Weapons.DrawNormalVectors();
        }

        /// <summary>
        /// Draw debug.
        /// </summary>
        /// <returns></returns>
        public override bool DebugDraw()
        {

            //MyDebugDraw.DrawAxis(WorldMatrix, 5, 1);

            /*
            if (this.IsCameraInsideMinerShip())
            {
                return true;
            }   */

            if (!base.DebugDraw())
                return false;

            if (DebugDrawEngineThrusts)
            {
                Vector3 color = Color.Blue.ToVector3();
                DebugDrawSubobject(m_subObjectEngineThrustBackwardSmallLeftside, color);
                DebugDrawSubobject(m_subObjectEngineThrustBackwardSmallRightside, color);
                DebugDrawSubobject(m_subObjectEngineThrustBackwardLeftside, color);
                DebugDrawSubobject(m_subObjectEngineThrustBackwardRightside, color);

                DebugDrawSubobject(m_subObjectEngineThrustBackward2Leftside, color);
                DebugDrawSubobject(m_subObjectEngineThrustBackward2Rightside, color);
                DebugDrawSubobject(m_subObjectEngineThrustForwardLeftside, color);

                DebugDrawSubobject(m_subObjectEngineThrustForwardRightside, color);
                DebugDrawSubobject(m_subObjectEngineThrustStrafeLeft, color);
                DebugDrawSubobject(m_subObjectEngineThrustStrafeRight, color);
                DebugDrawSubobject(m_subObjectEngineThrustUpLeftside, color);

                DebugDrawSubobject(m_subObjectEngineThrustUpRightside, color);
                DebugDrawSubobject(m_subObjectEngineThrustDownLeftside, color);
                DebugDrawSubobject(m_subObjectEngineThrustDownRightside, color);
            }

            if (DebugDrawEngineWeapons)
            {
                DebugDrawWeapons();


                Matrix lightMatrix = Matrix.CreateWorld(m_subObjectLight.ModelSubObject.Position, m_subObjectLight.ModelSubObject.ForwardVector.Value, m_subObjectLight.ModelSubObject.UpVector.Value);
                lightMatrix *= WorldMatrix;
                MyDebugDraw.DrawSphereWireframe(lightMatrix.Translation, m_light.Range, new Vector3(m_light.Color.X, m_light.Color.Y, m_light.Color.Z), 1);
                MyDebugDraw.DrawAxis(lightMatrix, 5.0f, 1.0f);
            }

            //MyDebugDraw.DrawSphereWireframe(m_positionLightTransformed, 1, new Vector3(m_light.Color.X, m_light.Color.Y, m_light.Color.Z), 1);


            //MyDebugDraw.DrawSphereWireframe(WorldMatrix.Translation, ModelLod0.BoundingSphere.Radius, Vector3.Up, 1.0f);
            //MyDebugDraw.DrawSphereWireframe(Vector3.Transform(ModelLod0.BoundingSphere.Center, WorldMatrix), ModelLod0.BoundingSphere.Radius, Vector3.Right, 1.0f);

            if (MyGuiManager.GetScreenDebugBot() != null)
            {
                for (int i = 1; i < RouteMemory.GetCount(); i++)
                {
                    MyDebugDraw.DrawLine3D(RouteMemory.GetItem(i - 1), RouteMemory.GetItem(i), Color.White, Color.Red);
                }


                if (Leader == MySession.PlayerShip)
                {
                    for (int i = 1; i < RouteMemory.GetCount(); i++)
                    {
                        MyDebugDraw.DrawLine3D(((MySmallShip)Leader).RouteMemory.GetItem(i - 1), ((MySmallShip)Leader).RouteMemory.GetItem(i), Color.White, Color.Green);
                    }
                }
            }

            if (this == MySession.PlayerShip && DebugDrawSecondaryCameraBoundingFrustum)
            {
                MySecondaryCamera.Instance.DebugDraw();
            }
            return true;
        }

        void DebugDrawSubobject(StringBuilder text, Matrix matrix, Vector3 color)
        {
            float radius = 0.1f;
            Matrix worldMatrix = this.WorldMatrix;

            matrix *= worldMatrix;
            MyDebugDraw.DrawSphereWireframe(matrix.Translation, radius, color, 1);
            MyDebugDraw.DrawAxis(matrix, 5.0f, 1.0f);

            MyDebugDraw.DrawText(matrix.Translation, text, Color.White, 0.5f);
        }

        void DebugDrawSubobject(MyModelSubObject subObject, Vector3 color)
        {
            if (subObject != null)
            {
                Vector3 forward = Vector3.Forward;
                if (subObject.ForwardVector.HasValue)
                    forward = subObject.ForwardVector.Value;
                Vector3 up = Vector3.Up;
                if (subObject.UpVector.HasValue)
                    up = subObject.UpVector.Value;

                Matrix thrustMatrix = Matrix.CreateWorld(subObject.Position, forward, up);

                DebugDrawSubobject(new StringBuilder(subObject.Name), thrustMatrix, color);
            }
        }

        void DebugDrawSubobject(List<MyModelShipSubOject> subObjects, Vector3 color)
        {
            foreach (MyModelShipSubOject subObject in subObjects)
            {
                DebugDrawSubobject(subObject.ModelSubObject, color);
            }
        }

        void DebugDrawWeapons()
        {
            foreach (MyWeaponSlot weaponSlot in Weapons.WeaponSlots)
            {
                DebugDrawSubobject(weaponSlot.WeaponSubObject, weaponSlot.IsMounted() ? Color.Green.ToVector3() : Color.Yellow.ToVector3());
            }

            if (Weapons.GetMountedDrill() != null)
            {
                DebugDrawSubobject(new StringBuilder("Drill"), Weapons.GetMountedDrill().LocalMatrix, Vector3.One);
            }
            if (Weapons.GetMountedHarvestingDevice() != null)
            {
                DebugDrawSubobject(new StringBuilder("Harvester"), Weapons.GetMountedHarvestingDevice().LocalMatrix, Vector3.One);
            }
        }

        /// <summary>
        /// Gets the intersection with line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public override bool GetIntersectionWithLine(ref MyLine line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            t = null;
            //  First check bounding sphere of whole physic object. We must trust that all his childs are inside of default model bounding sphere.
            Matrix worldMatrix = this.WorldMatrix;
            BoundingSphere boundingSphere = new BoundingSphere(MyUtils.GetTransform(ModelLod0.BoundingSphere.Center, ref worldMatrix), ModelLod0.BoundingSphere.Radius);
            if (MyUtils.IsLineIntersectingBoundingSphere(ref line, ref boundingSphere) == false)
                return false;

            //  Test against default object model
            MyIntersectionResultLineTriangleEx? intersectionWithBase;
            base.GetIntersectionWithLine(ref line, out intersectionWithBase);

            //  Test against childs of this phys object (in this case guns)
            MyIntersectionResultLineTriangleEx? intersectionWithWeapons = Weapons != null ? Weapons.GetIntersectionWithLine(ref line) : null;

            //  Find closer intersection of these two
            MyIntersectionResultLineTriangleEx? result = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref intersectionWithBase, ref intersectionWithWeapons);

            //  Only if this is "player's ship" - thus someone is siting in inside (this is an optimization - enemy ships don't render glass decals, they don't need this code)
            if (this == MySession.PlayerShip)
            {
                //  Intersection with ideal glass
                if (GetShipCockpitGlass() != null && GetShipCockpitGlass().ModelLod0 != null && GetShipCockpitGlass().ModelLod0.GetTrianglePruningStructure() != null)
                {
                    var intersectionGlass =
                        GetShipCockpitGlass().ModelLod0.GetTrianglePruningStructure().GetIntersectionWithLine(
                            GetShipCockpitGlass(), ref line, IntersectionFlags.FLIPPED_TRIANGLES);

                    const float GLASS_DISTANCE_TOLERANCE = 3 * MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE;
                    if (MyIntersectionResultLineTriangleEx.IsDistanceLessThanTolerance(ref result, ref intersectionGlass, GLASS_DISTANCE_TOLERANCE))
                    {
                        result = intersectionGlass;
                    }
                }
            }
            t = result;

            return result != null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            Debug.Assert(!Closed, "Closing already closed entity");

            MySession.Static.Inventory.OnInventoryContentChange -= CheckPointInventory_OnInventoryContentChange;

            if (ToolKits != null)
            {
                ToolKits.Close();
                ToolKits = null;
            }

            TradeDetector = null;
            BuildDetector = null;
            MotherShipDetector = null;
            UseableEntityDetector = null;

            if (!m_groupMask.Equals(MyGroupMask.Empty))
                MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().PushBackGroupMask(GroupMask);

            RemoveLight();

            m_subObjectEngineThrustBackwardSmallLeftside.Clear();
            m_subObjectEngineThrustBackwardSmallRightside.Clear();
            m_subObjectEngineThrustBackwardLeftside.Clear();
            m_subObjectEngineThrustBackwardRightside.Clear();
            m_subObjectEngineThrustBackward2Leftside.Clear();
            m_subObjectEngineThrustBackward2Rightside.Clear();
            m_subObjectEngineThrustBackward2Middle.Clear();
            m_subObjectEngineThrustForwardLeftside.Clear();
            m_subObjectEngineThrustForwardRightside.Clear();
            m_subObjectEngineThrustStrafeLeft.Clear();
            m_subObjectEngineThrustStrafeRight.Clear();
            m_subObjectEngineThrustUpLeftside.Clear();
            m_subObjectEngineThrustUpRightside.Clear();
            m_subObjectEngineThrustDownLeftside.Clear();
            m_subObjectEngineThrustDownRightside.Clear();
            m_subObjectReflectorLeft.Clear();
            m_subObjectReflectorRight.Clear();

            foreach (MyModelShipSubOject subObject in m_subObjects)
            {
                if (subObject.Light != null)
                {
                    MyLights.RemoveLight(subObject.Light);
                    subObject.Light = null;
                }

                m_modelSubObjectsPool.Deallocate(subObject);
            }

            StopSounds();

            Weapons.Close();

            if (m_damageEffect != null)
            {
                m_damageEffect.Stop();
                m_damageEffect = null;
            }

            if (m_tradeNotification != null)
            {
                m_tradeNotification.Disappear();
            }
            if (m_buildNotification != null)
            {
                m_buildNotification.Disappear();
            }
            if (m_travelNotification != null)
            {
                m_travelNotification.Disappear();
            }
            if (m_securityControlHUBNotification != null)
            {
                m_securityControlHUBNotification.Disappear();
            }

            MyHudWarnings.Remove(this);
            MyGuiScreenInventoryManagerForGame.OpeningInventoryScreen -= OpeningInventoryScreen;
            MyGuiScreenInventoryManagerForGame.InventoryScreenClosed -= InventoryScreenClosed;

            base.Close();

            m_unifiedWeaponCues = null;
            m_smallShipWeapons = null;
        }

        public void StopSounds()
        {
            StopEngineSounds(SharpDX.XACT3.StopFlags.Immediate);
            if (m_playerBreathing != null) m_playerBreathing.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);

            if (m_unifiedWeaponCues != null) // can be null for e.g. drone (it has no weapons)
            {
                foreach (MySoundCue? soundCue in m_unifiedWeaponCues)
                {
                    if (soundCue != null)
                    {
                        soundCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (MyGuiScreenGamePlay.Static == null)
            {
                return;
            }

            if (this == MySession.PlayerShip)
            {
                UpdatePlayerHeadForCockpitInteriorWorldMatrix();
            }

            if (MySession.Is25DSector)
            {
                // if (!(this is MySmallShipBot))
                {
                    var worldMatrix = this.WorldMatrix;
                    var position = WorldMatrix.Translation;
                    position.Y = 0;
                    worldMatrix.Translation = position;

                    if (this == MySession.PlayerShip)
                    {
                        //worldMatrix.Up = Vector3.Up;
                        //worldMatrix.Forward = Vector3.Cross(worldMatrix.Up, worldMatrix.Right);
                    }


                    this.WorldMatrix = worldMatrix;
                    if (this.Physics != null)
                    {
                        this.Physics.LinearVelocity = new Vector3(this.Physics.LinearVelocity.X, 0, this.Physics.LinearVelocity.Z);
                    }
                }
            }

        }

        private void StopEngineSounds(SharpDX.XACT3.StopFlags stopOptions)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("StopEngineSounds");

            if (m_engineIdleCue != null) m_engineIdleCue.Value.Stop(stopOptions);
            if (m_engineOnOffSound != null) m_engineOnOffSound.Value.Stop(stopOptions);
            if (m_engineMovingCue != null) m_engineMovingCue.Value.Stop(stopOptions);
            if (m_engineAfterburnerCue != null) m_engineAfterburnerCue.Value.Stop(stopOptions);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void UpdatePlayerHeadForCockpitInteriorWorldMatrix()
        {
            Matrix worldMatrixAtZero = Matrix.CreateWorld(Vector3.Zero, WorldMatrix.Forward, WorldMatrix.Up);
            PlayerHeadForCockpitInteriorWorldMatrix = Matrix.CreateTranslation(-m_playerHeadLocal +
                                                                               m_subObjectPlayerHeadForCockpitInteriorTranslation.ModelSubObject.Position) *
                                                      worldMatrixAtZero;

            PlayerHeadForGunsWorldMatrix = Matrix.CreateTranslation(-m_playerHeadLocal +
                -m_subObjectPlayerHeadForCockpitInteriorTranslation.ModelSubObject.Position)
                * worldMatrixAtZero;
        }

        /// <summary>
        /// Called when [contact] with entity.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns></returns>
        protected override bool OnContact(ref MyRBSolverConstraint constraint)
        {
            var entityA = (constraint.m_SolverBody1.m_RigidBody.m_UserData as MyPhysicsBody).Entity;
            var entityB = (constraint.m_SolverBody2.m_RigidBody.m_UserData as MyPhysicsBody).Entity;

            Debug.Assert(entityA == this || entityB == this, "Contact is not with this smallship");

            var otherEntity = entityA == this ? entityB : entityA;

            if (!PlayerSimulation)
            {
                if (otherEntity is MyEntityDetector)
                    return true;

                return false;
            }

            // For ammo base, don't use "lazy" collision reaction
            if (otherEntity is MyAmmoBase)
                return true;

            var otherSmallShip = otherEntity as MySmallShip;
            if (otherSmallShip != null)
            {
                //// disable collisions of player friends with player
                //if (otherSmallShip.Leader == MySession.PlayerShip && Leader == MySession.PlayerShip)
                //{
                //    return false;
                //}

                if (otherSmallShip.Leader == this && this == MySession.PlayerShip)
                {
                    return false;
                }

                // disable collisions of player and other peers
                if (MyMultiplayerPeers.Static.IsStarted)
                {
                    bool a = false;
                    bool b = false;
                    foreach (var player in MyMultiplayerPeers.Static.Players)
	                {
                        a |= player.Ship == this;
                        b |= player.Ship == this;

                        if (a && b)
	                    {
                            return false;
	                    }
	                }
                }

                if (this == MySession.PlayerShip && MySession.PlayerFriends.Contains(otherSmallShip))
                {
                    return false;
                }
            }
            
            if ((otherEntity is MySmallShip || otherEntity is MyCargoBox) && this == MySession.PlayerShip)
            {
                //There is 0 because otherwise collision with marcus is too bouncy
                this.Physics.AngularDamping = m_angularDampingSlowdown;
                //return true;
            }

            //this.Physics.AngularDamping = 0.0f;

            m_hasCollided = true;
            MyUtils.AssertIsValid(Physics.LinearVelocity);
            MyUtils.AssertIsValid(constraint.m_Normal);
            if (MyMwcUtils.IsZero(Physics.LinearVelocity))// can be because ship switches between static during harvest
                return true;
            m_collisionFriction = Math.Abs(Vector3.Dot(Vector3.Normalize(Physics.LinearVelocity), constraint.m_Normal));

            if (constraint.m_SolverBody1.m_RigidBody.m_UserData == this.Physics)
            {
                //  Marek Rosa: I have disabled Ales' trick with rotating in one direction after contact, because it seems to be not needed after I switched small ship from sphere to a box
                constraint.m_Body1LocalPointCrossedNormal = Vector3.Zero;
                //constraint.m_Body1LocalPointCrossedNormal *= 0.02f;
            }
            else if (constraint.m_SolverBody2.m_RigidBody.m_UserData == this.Physics)
            {
                //  Marek Rosa: I have disabled Ales' trick with rotating in one direction after contact, because it seems to be not needed after I switched small ship from sphere to a box
                constraint.m_Body2LocalPointCrossedNormal = Vector3.Zero;
                //constraint.m_Body2LocalPointCrossedNormal *= 0.02f;
            }

            return true;
        }

        protected override void OnContactEnd(MyContactEventInfo contactInfo)
        {
            base.OnContactEnd(contactInfo);
            this.Physics.AngularDamping = m_angularDampingDefault;
        }

        /// <summary>
        /// Called when [contact touches] with this entity.
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        protected override void OnContactTouch(MyContactEventInfo contactInfo)
        {
            /*var physicObject0 = (MyRigidBody) contactInfo.m_RigidBody1.m_UserData;
            var physicObject1 = (MyRigidBody) contactInfo.m_RigidBody2.m_UserData;

            float minSpeedForScrapeSound = 5;

            if (physicObject0.Speed > 5 || physicObject1.Speed > minSpeedForScrapeSound)
            {
                base.OnContactTouch(contactInfo);
            }*/
        }

        public override string GetFriendlyName()
        {
            return "MySmallShip";
        }

        #endregion

        #region Secondary camera

        // determines which camera is active: 0 for rear-mirror, 1 for missile, 2+ for remote cameras.
        private int m_selectedRemoteCameraIndex = 0;
        private int m_selectedDroneIndex = 0;

        private readonly List<MyRemoteCamera> m_remoteCameras = new List<MyRemoteCamera>();

        private readonly StringBuilder m_remoteCameraStringBuilder = new StringBuilder();

        public void AddRemoteCamera(MyRemoteCamera remoteCamera)
        {
            m_remoteCameras.Add(remoteCamera);
        }

        public void RemoveRemoteCamera(MyRemoteCamera remoteCamera)
        {
            if (IsSelectedRemoteCamera())
            {
                MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RearMirror;
                UpdateSecondaryCamera();
            }
            m_remoteCameras.Remove(remoteCamera);
        }

        public bool IsSelectedRemoteCamera()
        {
            return MySecondaryCamera.Instance.SecondaryCameraAttachedTo == MySecondaryCameraAttachedTo.RemoteCamera;
        }

        public MyRemoteCamera GetSelectedRemoteCamera()
        {
            if (IsSelectedRemoteCamera())
                return m_remoteCameras[m_selectedRemoteCameraIndex];

            return null;
        }

        public bool IsSelectedDrone()
        {
            return MySecondaryCamera.Instance.SecondaryCameraAttachedTo == MySecondaryCameraAttachedTo.Drone;
        }

        public MyDrone GetSelectedDrone()
        {
            if (IsSelectedDrone())
                return m_drones[m_selectedDroneIndex];

            return null;
        }

        public void SelectPreviousSecondaryCamera()
        {
            switch (MySecondaryCamera.Instance.SecondaryCameraAttachedTo)
            {
                case MySecondaryCameraAttachedTo.RearMirror:
                    MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RemoteCamera;
                    m_selectedRemoteCameraIndex = m_remoteCameras.Count - 1;
                    break;

                case MySecondaryCameraAttachedTo.Missile:
                    MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RearMirror;
                    break;

                case MySecondaryCameraAttachedTo.Drone:
                    m_selectedDroneIndex--;
                    if (m_selectedDroneIndex < 0)
                    {
                        m_selectedDroneIndex = 0;
                        MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Missile;
                    }
                    break;

                case MySecondaryCameraAttachedTo.RemoteCamera:
                    m_selectedRemoteCameraIndex--;
                    if (m_selectedRemoteCameraIndex < 0)
                    {
                        m_selectedRemoteCameraIndex = 0;
                        MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Drone;
                        m_selectedDroneIndex = m_drones.Count - 1;
                    }
                    break;

                case MySecondaryCameraAttachedTo.PlayerShip:
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateSecondaryCamera(wasDecrement: true);
        }

        public void SelectNextSecondaryCamera()
        {
            switch (MySecondaryCamera.Instance.SecondaryCameraAttachedTo)
            {
                case MySecondaryCameraAttachedTo.RearMirror:
                    MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Missile;
                    break;

                case MySecondaryCameraAttachedTo.Missile:
                    MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Drone;
                    m_selectedDroneIndex = 0;
                    break;

                case MySecondaryCameraAttachedTo.Drone:
                    m_selectedDroneIndex++;
                    if (m_selectedDroneIndex > m_drones.Count - 1)
                    {
                        m_selectedDroneIndex = m_drones.Count - 1;
                        MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RemoteCamera;
                        m_selectedRemoteCameraIndex = 0;
                    }
                    break;

                case MySecondaryCameraAttachedTo.RemoteCamera:
                    m_selectedRemoteCameraIndex++;
                    if (m_selectedRemoteCameraIndex > m_remoteCameras.Count - 1)
                    {
                        m_selectedRemoteCameraIndex = m_remoteCameras.Count - 1;
                        MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RearMirror;
                    }
                    break;

                case MySecondaryCameraAttachedTo.PlayerShip:
                    {
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateSecondaryCamera(wasDecrement: false);
        }

        public void SelectLastRemoteCamera()
        {
            MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.RemoteCamera;
            m_selectedRemoteCameraIndex = m_remoteCameras.Count - 1;
            UpdateSecondaryCamera();
        }

        public void SelectLastDroneCamera()
        {
            MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.Drone;
            m_selectedDroneIndex = m_drones.Count - 1;
            UpdateSecondaryCamera();
        }

        public void SelectShipCamera()
        {
            MySecondaryCamera.Instance.SecondaryCameraAttachedTo = MySecondaryCameraAttachedTo.PlayerShip;
            MySecondaryCamera.Instance.SetPlayerShip();
            UpdateSecondaryCamera();
        }

        public void UpdateSecondaryCamera(bool wasDecrement = false)
        {
            bool isCameraAvailable = false;

            switch (MySecondaryCamera.Instance.SecondaryCameraAttachedTo)
            {
                case MySecondaryCameraAttachedTo.RearMirror:
                    MySecondaryCamera.Instance.SetRearMirror();
                    isCameraAvailable = true;
                    break;

                case MySecondaryCameraAttachedTo.Missile:
                    if (LastMissileFired != null && !LastMissileFired.IsExploded())
                    {
                        MySecondaryCamera.Instance.SetEntityCamera(LastMissileFired);
                        isCameraAvailable = true;
                    }
                    break;

                case MySecondaryCameraAttachedTo.Drone:
                    if (m_drones.Count > 0 && m_selectedDroneIndex < m_drones.Count)
                    {
                        MySecondaryCamera.Instance.SetEntityCamera(GetSelectedDrone());
                        isCameraAvailable = true;
                    }
                    break;

                case MySecondaryCameraAttachedTo.RemoteCamera:
                    if (m_remoteCameras.Count > 0 && m_selectedRemoteCameraIndex < m_remoteCameras.Count)
                    {
                        MySecondaryCamera.Instance.SetEntityCamera(GetSelectedRemoteCamera());
                        isCameraAvailable = true;
                    }
                    break;

                // This can only be true in special like, such as player is controlling drone
                case MySecondaryCameraAttachedTo.PlayerShip:
                    Debug.Assert(MySession.PlayerShip != null);
                    isCameraAvailable = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!isCameraAvailable)
            {
                if (wasDecrement)
                {
                    SelectPreviousSecondaryCamera();
                }
                else
                {
                    SelectNextSecondaryCamera();
                }
            }
        }

        public StringBuilder GetCameraDescription()
        {
            switch (MySecondaryCamera.Instance.SecondaryCameraAttachedTo)
            {
                case MySecondaryCameraAttachedTo.RearMirror:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.RearViewMirror);

                case MySecondaryCameraAttachedTo.Drone:
                    m_remoteCameraStringBuilder.Clear();
                    m_remoteCameraStringBuilder.AppendFormat(
                        MyTextsWrapper.Get(MyTextsWrapperEnum.DroneNo).ToString(),
                        (m_selectedDroneIndex + 1),
                        MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.DRONE_CONTROL)
                    );
                    return m_remoteCameraStringBuilder;

                case MySecondaryCameraAttachedTo.Missile:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.MissileCamera);

                case MySecondaryCameraAttachedTo.RemoteCamera:
                    MyRemoteCamera remoteCamera = null;
                    if (MySecondaryCamera.Instance != null)
                        remoteCamera = MySecondaryCamera.Instance.GetCameraEntity() as MyRemoteCamera;

                    m_remoteCameraStringBuilder.Clear();
                    m_remoteCameraStringBuilder.AppendFormat(
                        MyTextsWrapper.Get(remoteCamera != null && remoteCamera.CanBeRotated() ? MyTextsWrapperEnum.RemoteCameraNoRotate : MyTextsWrapperEnum.RemoteCameraNo).ToString(),
                        (m_selectedRemoteCameraIndex + 1),
                        MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.CONTROL_SECONDARY_CAMERA));
                    return m_remoteCameraStringBuilder;

                case MySecondaryCameraAttachedTo.PlayerShip:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Ship);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Remote bombs

        readonly List<MyRemoteBomb> m_firedRemoteBombs =
            new List<MyRemoteBomb>(MyUniversalLauncherConstants.MAX_REMOTEBOMBS_COUNT);

        public void AddRemoteBomb(MyRemoteBomb bomb)
        {
            m_firedRemoteBombs.Add(bomb);
        }

        public void RemoveRemoteBomb(MyRemoteBomb bomb)
        {
            m_firedRemoteBombs.Remove(bomb);
        }

        public int RemoteBombCount
        {
            get { return m_firedRemoteBombs.Count; }
        }

        #endregion

        #region Ammo special function

        /// <summary>
        /// Invokes ammo special if special ammo exists in assignment ammo.
        /// </summary>
        /// <remarks>
        /// This is hardcoded now, it would be better if it was more robust.
        /// </remarks>
        public void InvokeAmmoSpecialFunction()
        {
            MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.SfxHudRadarMode,
                    GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);

            if (HasFiredRemoteBombs())
            {
                foreach (var remoteBomb in m_firedRemoteBombs)
                {
                    remoteBomb.Explode();
                }
                m_firedRemoteBombs.Clear();
            }
            else
            {
                Config.TimeBombTimer.ChangeValueUp();
            }
        }

        public bool HasFiredRemoteBombs()
        {
            return m_firedRemoteBombs.Count > 0;
        }

        #endregion

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                if (this == MySession.PlayerShip && MySession.Static != null)
                    return MySession.Static.Player.Faction;
                return m_faction;
            }
            set
            {
                System.Diagnostics.Debug.Assert(value > 0, "Cannot assign faction 'None' to ship");
                base.Faction = value;
                if (MySession.PlayerShip != null)
                {

                    if (MyFactions.GetFactionsRelation(MySession.PlayerShip.Faction, value) == MyFactionRelationEnum.Enemy)
                    {
                        if ((ShipType != MyMwcObjectBuilder_SmallShip_TypesEnum.YG) &&
                                   (ShipType != MyMwcObjectBuilder_SmallShip_TypesEnum.ORG) &&
                                   (ShipType != MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV) &&
                                   (ShipType != MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD))
                        {
                        }
                    }
                }
            }
        }

        internal void RemoveLight()
        {
            if (m_light != null)
            {
                MyLights.RemoveLight(m_light);
                m_light = null;
            }
        }

        internal void RemoveSubObjectLights()
        {
            foreach (MyModelShipSubOject subObject in m_subObjects)
            {
                if (subObject.Light != null)
                {
                    MyLights.RemoveLight(subObject.Light);
                    subObject.Light = null;
                }
            }
        }

        public MyMwcObjectBuilder_FactionEnum FalseFactions
        {
            get;
            private set;
        }

        public bool WasRecentlyLooted()
        {
            return m_isLooted || MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastLootTime <= MyGameplayConstants.SHIP_DESTRUCTION_TIME_AFTER_LOOT * 1000;
        }

        #region Drones

        private readonly List<MyDrone> m_drones = new List<MyDrone>();

        public List<MyDrone> Drones
        {
            get { return m_drones; }
        }

        private int m_lastTimeDeployedDrone;
        MyHudTextWarning m_sectorBoundariesWarning;


        /// <summary>
        /// Tries to deploy a drone from inventory. Fails if there are no drone items in inventory
        /// or if there is already a drone deployed.
        /// </summary>
        /// <returns>True if drone was deployed successfully.</returns>
        public bool TryDeployDrone()
        {
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeDeployedDrone) > MySmallShipConstants.DRONE_RELEASE_INTERVAL)
            {
                var droneItem = Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.Drone, null);
                // because in missions you will have drones in session's inventory
                if (droneItem == null)
                {
                    droneItem = MySession.Static.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.Drone, null);
                }
                if (droneItem != null)
                {
                    var droneInitialPosition = WorldMatrix.Translation + WorldMatrix.Forward * WorldAABB.Size().Z;
                    var lineToTestDroneDeploy = new MyLine(this.GetPosition(), droneInitialPosition + 30 * WorldMatrix.Forward);
                    var intersection = MyEntities.GetIntersectionWithLine(ref lineToTestDroneDeploy, this, null);
                    if (!intersection.HasValue)
                    {
                        MyInventory droneInventory = droneItem.Owner as MyInventory;
                        Debug.Assert(droneItem != null);
                        droneInventory.RemoveInventoryItemAmount(ref droneItem, 1);

                        var droneBuilder = (MyMwcObjectBuilder_Drone)droneItem.GetInventoryItemObjectBuilder(false);
                        if (droneBuilder.Faction == 0)
                        {
                            Debug.Fail("Wrong drone object builder faction. Should not happen.");
                            droneBuilder.Faction = MyMwcObjectBuilder_FactionEnum.None;
                        }

                        DeployDrone((MyMwcObjectBuilder_Drone)droneBuilder.Clone(), droneInitialPosition);
                        m_lastTimeDeployedDrone = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        return true;
                    }
                }
            }

            return false;
        }

        private void DeployDrone(MyMwcObjectBuilder_Drone objectBuilder, Vector3 droneInitialPosition)
        {
            objectBuilder.Faction = this.Faction;
            objectBuilder.OwnerId = this.EntityId.Value.NumericValue;
            objectBuilder.EntityId = MyEntityIdentifier.AllocateId().NumericValue;
            var drone = (MyDrone)MyEntities.CreateFromObjectBuilderAndAdd(
                null, objectBuilder,
                Matrix.CreateWorld(droneInitialPosition, WorldMatrix.Forward, WorldMatrix.Up));
            drone.OwnerShip = this;
            m_drones.Add(drone);
        }

        public void RemoveDrone(MyDrone drone)
        {
            Debug.Assert(m_drones.Contains(drone));
            m_drones.Remove(drone);
            UpdateSecondaryCamera(true);
        }

        public void AddDrone(MyDrone drone)
        {
            Debug.Assert(!m_drones.Contains(drone), "Duplicit drone add.");
            m_drones.Add(drone);
        }

        #endregion

        /// <summary>
        /// Called on player ship after CloseAll (e.g. when leaving the sector, in editor, ...).
        /// Clears all references to other entities (now non-existing).
        /// </summary>
        public void CleanupAfterCloseAll()
        {
        }

        /// <summary>
        /// Because, playership has fixed entity ID, so we must remove it, because when we want init new playership, than we need this ID
        /// </summary>        
        private void PlayerShip_OnMarkForClose(MyEntity obj)
        {
            OnMarkForClose -= PlayerShip_OnMarkForClose;
        }

        public void AddFriendlyFireDamage(MyEntity entity, float damage)
        {
            uint entityId = entity.EntityId.Value.NumericValue;
            MyDamageOverTimeItem item;
            if (!m_damageOverTimeList.TryGetValue(entityId, out item))
            {
                item = new MyDamageOverTimeItem();
                m_damageOverTimeList.Add(entityId, item);
            }
            item.IntervalDamage += damage;
        }


        public void UpdateFriendlyFireDamage()
        {
            if (m_friendlyFireDamageTimer > MySmallShipConstants.DAMAGE_OVER_TIME_INTERVAL && MyGuiScreenGamePlay.Static.IsGameStoryActive())
            {
                m_friendlyFireDamageTimer = 0;
                m_damageOverTimeRemoveBuffer.Clear();

                foreach (var item in m_damageOverTimeList)
                {
                    if (item.Value.IntervalDamage <= 0)
                    {
                        m_damageOverTimeRemoveBuffer.Add(item.Key);
                        continue;
                    }

                    item.Value.Damage += item.Value.IntervalDamage;
                    item.Value.IntervalDamage = 0;

                    // Too much friendly damage, turn friends into enemy
                    if (item.Value.Damage > MySmallShipConstants.MAX_FRIENDLY_DAMAGE)
                    {
                        MyFriendlyFire.MakeEnemy(Faction);
                        MyFriendlyFire.StartGameoverTimer();

                        if (MyMultiplayerGameplay.IsRunning)
                        {
                            MyMultiplayerGameplay.Static.FriendlyFire(MyFriendlyFireEnum.AGGRO);
                        }

                        m_damageOverTimeRemoveBuffer.Add(item.Key);
                    }
                }

                // Remove obsolete items
                foreach (var entityID in m_damageOverTimeRemoveBuffer)
                {
                    m_damageOverTimeList.Remove(entityID);
                }
            }
            else
            {
                m_friendlyFireDamageTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }
        }

        public void Refill()
        {
            this.Fuel = this.MaxFuel;
            this.Oxygen = this.MaxOxygen;
            this.AddHealth(this.MaxHealth);
            this.ArmorHealth = this.MaxArmorHealth;
        }

        public void SetParked(bool staticPhysics)
        {
            PersistentFlags |= MyPersistentEntityFlags.Parked;
            if (staticPhysics)
            {
                PersistentFlags |= MyPersistentEntityFlags.StaticPhysics;
            }
            UpdateParked();
        }

        private void UpdateParked()
        {
            if ((PersistentFlags & MyPersistentEntityFlags.Parked) != 0)
            {
                AIPriority = -1;    // stop attacks from other bots
                HideFromBots();
                Config.Engine.SetOff();
                Physics.Static = (PersistentFlags & MyPersistentEntityFlags.StaticPhysics) != 0;
                if (!Physics.Static)
                {
                    Physics.LinearDamping = 0.5f;
                    Physics.AngularDamping = 0.85f;
                    Physics.MaxLinearVelocity = 350;
                    Physics.MaxAngularVelocity = 0.5f;
                }
                Enabled = false;
                MyHud.RemoveText(this);

                if (MySession.PlayerFriends.Contains(this))
                    MySession.PlayerFriends.Remove(this);

                MySmallShipBot bot = this as MySmallShipBot;
                if (bot != null)
                {
                    bot.ActiveAI = false;
                }
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;

                if (!value)
                {
                    if (Weapons != null)
                    {
                        Weapons.StopAllSounds();
                    }
                }
            }
        }

        public override void MarkForClose()
        {
            if (MySession.PlayerFriends.Contains(this))
                MySession.PlayerFriends.Remove(this);

            base.MarkForClose();
        }

        //MWBuilder method
        public float GetStepRatio()
        {
            int diff = MyMinerGame.TotalTimeInMilliseconds - m_stepStartMS;
            diff = (int)MathHelper.Clamp(diff, 0, m_stepDurationMS);

            return (float)diff / m_stepDurationMS;
        }

        public override string GetCorrectDisplayName()
        {
            if (DisplayName == "Ship")
            {
                return MyTextsWrapper.Get(MyTextsWrapperEnum.Ship).ToString();
            }

            return base.GetCorrectDisplayName();
        }
    }

    #region Position memory helper class
    class MyPositionMemory
    {
        public FastResourceLock RouteMemoryLock = new FastResourceLock();

        int m_startIndex;
        int m_count;

        float m_closeRadiusSquared;

        Vector3[] m_positionList;

        public MyPositionMemory(int capacity, float closeRadius)
        {
            m_closeRadiusSquared = closeRadius * closeRadius;
            m_positionList = new Vector3[capacity];

            m_startIndex = 0;
            m_count = 0;
        }

        private int GetArrayIndex(int index)
        {
            return (m_startIndex + index) % m_positionList.Length;
        }

        private bool IsFull()
        {
            return m_count == m_positionList.Length;
        }

        public void Add(Vector3 position)
        {
            // Don't add position if last and this position are considered same
            if (m_count > 0 && Vector3.DistanceSquared(GetItem(m_count - 1), position) <= m_closeRadiusSquared)
            {
                return;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("IsFull");
            if (IsFull())
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Remove");
                using (RouteMemoryLock.AcquireExclusiveUsing())
                {
                    RemoveFirst();
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetArrayIndex");
            
            // Add item, increase item count
            m_positionList[GetArrayIndex(m_count++)] = position;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public void RemoveFirst()
        {
            if (m_count > 0)
            {
                m_startIndex = GetArrayIndex(1);
                --m_count;
            }
        }

        public int GetCount()
        {
            return m_count;
        }

        public Vector3 GetItem(int index)
        {
            //Debug.Assert(index >= 0 && index < m_positionList.Length);
            return m_positionList[GetArrayIndex(index)];
        }

        public void Copy(MyPositionMemory route)
        {
            Debug.Assert(m_positionList.Length == route.m_positionList.Length);
            if (m_positionList.Length != route.m_positionList.Length)
            {
                return;
            }

            m_startIndex = route.m_startIndex;
            m_count = route.m_count;

            for (int i = 0; i < m_positionList.Length; i++)
            {
                m_positionList[i] = route.m_positionList[i];
            }
        }
    }
    #endregion

}


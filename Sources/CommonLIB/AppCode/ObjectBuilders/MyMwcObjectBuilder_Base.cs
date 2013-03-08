using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using System.IO.Compression;
using SysUtils;

//  Object builder is object that defines how to create instance of particular MyPhysObject**
//  Every object builder class must implement parameter-less constructor (needed when loading objects)

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    // Do not change numbers, these are saved in DB
    [Flags]
    public enum MyPersistentEntityFlags
    {
        None = 0,
        //CULL_OBJECT = 1 << 0, obsolete
        Enabled = 1 << 1,
        CastShadows = 1 << 2,
        Secret = 1 << 3,
        Deactivated = 1 << 4,   //need to be in negative form because of default value
        Destructible = 1 << 5,
        NotTradeable = 1 << 6,  //need to be in negative form because of default value
        Parked = 1 << 7, // For ships
        StaticPhysics = 1 << 8, // For anything, currently used only for ships
        DisplayOnHud = 1 << 9,
        ActivatedOnDifficultyEasy = 1 << 10,
        ActivatedOnDifficultyNormal = 1 << 11,
        ActivatedOnDifficultyHard = 1 << 12,

        KinematicPhysics = 1 << 13, //Determines object which uses kinematic physics instead of default

        ActivatedOnAllDifficulties = ActivatedOnDifficultyEasy | ActivatedOnDifficultyNormal | ActivatedOnDifficultyHard,
    }

    //  You can use this enum for knowing object type or you can get it from class type. Both ways should be equivalent.
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    //  IMPORTANT: Each new enum item add also to m_objectBuilderToType through AddObjectBuilderToType (see static constructor below)
    public enum MyMwcObjectBuilderTypeEnum : ushort
    {
        SmallDebris = 0,
        LargeDebrisField = 1,
        LargeShip = 12,
        SmallShip_Engine = 15,
        SmallShip_Player = 16,
        //SmallShip_Ally = 17,
        SmallShip_Bot = 18,
        Sector = 19,
        VoxelMap = 20,
        VoxelMap_MergeMaterial = 21,
        VoxelMap_MergeContent = 22,
        VoxelMap_Neighbour = 23,
        VoxelHand_Sphere = 24,
        VoxelHand_Box = 25,
        VoxelHand_Cuboid = 26,
        SmallShip_Ammo = 36,
        //LargeShip_Weapon = 37,
        SmallShip_Weapon = 38,
        StaticAsteroid = 39,
        SmallShip_Tool = 40,
        SmallShip = 41,
        SmallShip_AssignmentOfAmmo = 42,

        //SmallShip_RearCamera = 50,
        //SmallShip_LaserPointer = 51,
        //SmallShip_AutoTargeting = 52,
        //SmallShip_NightVision = 53,
        //SmallShip_NanoRepairTool = 54,
        //SmallShip_Medikit = 55,
        //SmallShip_XRay = 56,
        //SmallShip_AntiradiationMedicine = 57,
        //SmallShip_RadarJammer = 58,
        //SmallShip_PerformanceEnhancingMedicine = 59,
        //SmallShip_HealthEnhancingMedicine = 60,
        //SmallShip_ExtraFuelContainer = 61,
        //SmallShip_ExtraElectricityContainer = 62,
        //SmallShip_ExtraOxygenContainer = 63,
        //SmallShip_OxygenConverter = 64,
        //SmallShip_FuelConverter = 65,
        //SmallShip_SolarPanel = 66,
        //SmallShip_BoobyTrap = 67,
        //SmallShip_Sensor = 68,
        //SmallShip_RemoteCamera = 69,
        //SmallShip_RemoteCameraOnDrone = 70,
        //SmallShip_AlienObjectDetector = 71,
        LargeShip_Ammo = 72,
        Prefab = 73,
        PrefabContainer = 74,
        //InfluenceSphereDust = 75,
        //InfluenceSphereSound = 76,
        PrefabLight = 77,
        SpawnPoint = 78,
        SmallShip_Armor = 79,
        SmallShip_Radar = 80,
        Ore = 81,
        Blueprint = 82,
        FoundationFactory = 83,
        Inventory = 84,
        InventoryItem = 85,
        Checkpoint = 86,
        Player = 87,
        PlayerStatistics = 88,
        PrefabSound = 89,
        PrefabParticles = 90,
        PrefabKinematic = 91,
        Session = 92,
        Event = 93,
        //WayPoint = 94,
        //WayPointGroup = 95,
        ObjectToBuild = 96,
        ObjectGroup = 97,
        PrefabLargeShip = 98,
        PrefabLargeWeapon = 99,
        PrefabHangar = 100,
        DummyPoint = 101,
        SnapPointLink = 102,
        PrefabKinematicPart = 103,
        EntityDetector = 104,
        ShipConfig = 105,
        PrefabFoundationFactory = 106,
        PrefabSecurityControlHUB = 107,
        FalseId = 108,
        Drone = 109,
        SmallShip_HackingTool = 110,
        PrefabBankNode = 111,
        EntityUseProperties = 112,
        //WayPointPath = 113,
        PrefabGenerator = 114,
        CargoBox = 115,
        SectorObjectGroups = 116,
        WaypointNew = 117,
        MysteriousCube = 118,
        PrefabScanner = 119,
        PrefabAlarm = 120,
        PrefabCamera = 121,
        //PrefabKinematicRotating = 122,
        PrefabKinematicRotatingPart = 123,
        Meteor = 124,
        //InfluenceSphereRadioactivity = 125,
        InfluenceSphere = 126,
        SmallShipTemplate = 127,
        SmallShipTemplates = 128,
        FactionRelationChange = 129,

        RemotePlayer = 130,

        VoxelHand_Cylinder = 131,
        GlobalData = 132,
    }

    // Do not change numbers, these are saved in DB
    public enum MyMwcObjectBuilder_FactionEnum
    {
        None = 1 << 0,  //major factions
        Euroamerican = 1 << 1,
        China = 1 << 2,
        FourthReich = 1 << 3,
        Omnicorp = 1 << 4,

        Russian = 1 << 5,  //minor factions
        Japan = 1 << 6,
        India = 1 << 7,
        Saudi = 1 << 8,
        Church = 1 << 9,
        FSRE = 1 << 10,
        FreeAsia = 1 << 11,

        Pirates = 1 << 12, //groups
        Miners = 1 << 13,
        Freelancers = 1 << 14,
        Ravens = 1 << 15,
        Traders = 1 << 16,
        Syndicate = 1 << 17,
        Templars = 1 << 18,
        Rangers = 1 << 19,
        TTLtd = 1 << 20,
        SMLtd = 1 << 21,
        CSR = 1 << 22,
        Russian_KGB = 1 << 23,
        Slavers = 1 << 24,
        WhiteWolves = 1 << 25,
        Rainiers = 1 << 26,

        // LIMIT IS 1 << 31!!!
    }

    public class MyMwcObjectBuilderDefinition
    {
        public Type Type { get; set; }
        public int[] Ids { get; set; }

        public MyMwcObjectBuilderDefinition(Type type, int[] ids)
        {
            Type = type;
            Ids = ids;
        }
    }

    public class MyMwcFactionsByIndex
    {
        private static Dictionary<int, MyMwcObjectBuilder_FactionEnum> m_indexesToFactionEnums;
        private static Dictionary<MyMwcObjectBuilder_FactionEnum, int> m_factionEnumsToIndexes;
        private static int[] m_factionsIndexes;

        static MyMwcFactionsByIndex()
        {
            Array factionValues = Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum));
            m_indexesToFactionEnums = new Dictionary<int, MyMwcObjectBuilder_FactionEnum>();
            m_factionEnumsToIndexes = new Dictionary<MyMwcObjectBuilder_FactionEnum, int>();
            m_factionsIndexes = new int[factionValues.Length];
            for (int i = 0; i < factionValues.Length; i++)
            {
                m_indexesToFactionEnums.Add(i, (MyMwcObjectBuilder_FactionEnum)factionValues.GetValue(i));
                m_factionEnumsToIndexes.Add((MyMwcObjectBuilder_FactionEnum)factionValues.GetValue(i), i);
                m_factionsIndexes[i] = i;
            }
        }

        public static MyMwcObjectBuilder_FactionEnum GetFaction(int index)
        {
            return m_indexesToFactionEnums[index];
        }

        public static int GetFactionIndex(MyMwcObjectBuilder_FactionEnum faction)
        {
            // Check if faction is single value
            int factionValue = (int)faction;
            MyCommonDebugUtils.AssertDebug(((factionValue > 0) && ((factionValue & (factionValue - 1)) == 0)));

            return m_factionEnumsToIndexes[faction];
        }

        public static int[] GetFactionsIndexes()
        {
            return m_factionsIndexes;
        }
    }

    public abstract class MyMwcObjectBuilder_Base
    {
        /// <summary>
        /// First version which is compatible with all future versions
        /// </summary>
        public const int FIRST_COMPATIBILITY_VERSION = 01084000;

        static EndPoint fakeEp = new IPEndPoint(0x7F000001, 0); // 127.0.0.1

        // To prevent allocations when cloning object builders (make it large enought to prevent reallocations in LOH)
        static MemoryStream m_cloneStream = new MemoryStream(128 * 1024); // 128 KB should be enough
        static BinaryWriter m_cloneWriter;
        static BinaryReader m_cloneReader;

        static string m_networkError;
        public bool Generated = false;

        //static Type[] m_objectBuilderToType = new Type[Enum.GetValues(typeof(MyMwcObjectBuilderTypeEnum)).Length];
        //static readonly Type[] m_objectBuilderToType = new Type[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1];
        //static readonly int[][] m_objectBuilderIDs = new int[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];
        static readonly MyMwcObjectBuilderDefinition[] m_objectBuilderDefinitions = new MyMwcObjectBuilderDefinition[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1];

        internal MyMwcObjectBuilder_Base()
        {
            LoadPersistantFlags();
        }

        protected virtual void LoadPersistantFlags()
        {
            PersistentFlags |= MyPersistentEntityFlags.Destructible;
            PersistentFlags |= GetDefaultPersistantFlags();
        }

        public static MyPersistentEntityFlags GetDefaultPersistantFlags()
        {
            MyPersistentEntityFlags defaultFlags = MyPersistentEntityFlags.None;

            // by default all entities are activated on all difficulty levels
            defaultFlags |= MyPersistentEntityFlags.ActivatedOnAllDifficulties | MyPersistentEntityFlags.CastShadows;

            return defaultFlags;
        }

        static MyMwcObjectBuilder_Base()
        {
            m_cloneWriter = new BinaryWriter(m_cloneStream);
            m_cloneReader = new BinaryReader(m_cloneStream);

            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallDebris, typeof(MyMwcObjectBuilder_SmallDebris), typeof(MyMwcObjectBuilder_SmallDebris_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.LargeDebrisField, typeof(MyMwcObjectBuilder_LargeDebrisField), typeof(MyMwcObjectBuilder_LargeDebrisField_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.LargeShip, typeof(MyMwcObjectBuilder_LargeShip), typeof(MyMwcObjectBuilder_LargeShip_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, typeof(MyMwcObjectBuilder_SmallShip_Engine), typeof(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Player, typeof(MyMwcObjectBuilder_SmallShip_Player), typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Ally, typeof(MyMwcObjectBuilder_SmallShip_Ally), null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, typeof(MyMwcObjectBuilder_SmallShip_Bot), typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelMap, typeof(MyMwcObjectBuilder_VoxelMap), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial, typeof(MyMwcObjectBuilder_VoxelMap_MergeMaterial), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent, typeof(MyMwcObjectBuilder_VoxelMap_MergeContent), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Sector, typeof(MyMwcObjectBuilder_Sector), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour, typeof(MyMwcObjectBuilder_VoxelMap_Neighbour), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere, typeof(MyMwcObjectBuilder_VoxelHand_Sphere), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelHand_Box, typeof(MyMwcObjectBuilder_VoxelHand_Box), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid, typeof(MyMwcObjectBuilder_VoxelHand_Cuboid), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, typeof(MyMwcObjectBuilder_SmallShip_Ammo), typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum));
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, typeof(MyMwcObjectBuilder_PrefabLargeWeapon), typeof(MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, typeof(MyMwcObjectBuilder_SmallShip_Weapon), typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.StaticAsteroid, typeof(MyMwcObjectBuilder_StaticAsteroid), typeof(MyMwcObjectBuilder_StaticAsteroid_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, typeof(MyMwcObjectBuilder_SmallShip_Tool), typeof(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip, typeof(MyMwcObjectBuilder_SmallShip), typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo, typeof(MyMwcObjectBuilder_AssignmentOfAmmo), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, typeof(MyMwcObjectBuilder_LargeShip_Ammo), typeof(MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Prefab, typeof(MyMwcObjectBuilder_Prefab), typeof(MyMwcObjectBuilder_Prefab_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabContainer, typeof(MyMwcObjectBuilder_PrefabContainer), typeof(MyMwcObjectBuilder_PrefabContainer_TypesEnum));
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.InfluenceSphereDust, typeof(MyMwcObjectBuilder_InfluenceSphereDust), (Type)null);
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.InfluenceSphereSound, typeof(MyMwcObjectBuilder_InfluenceSphereSound), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabLight, typeof(MyMwcObjectBuilder_PrefabLight), typeof(MyMwcObjectBuilder_PrefabLight_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SpawnPoint, typeof(MyMwcObjectBuilder_SpawnPoint), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, typeof(MyMwcObjectBuilder_SmallShip_Armor), typeof(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, typeof(MyMwcObjectBuilder_SmallShip_Radar), typeof(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Ore, typeof(MyMwcObjectBuilder_Ore), typeof(MyMwcObjectBuilder_Ore_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Blueprint, typeof(MyMwcObjectBuilder_Blueprint), typeof(MyMwcObjectBuilder_Blueprint_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.FoundationFactory, typeof(MyMwcObjectBuilder_FoundationFactory), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Inventory, typeof(MyMwcObjectBuilder_Inventory), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.InventoryItem, typeof(MyMwcObjectBuilder_InventoryItem), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Checkpoint, typeof(MyMwcObjectBuilder_Checkpoint), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Player, typeof(MyMwcObjectBuilder_Player), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PlayerStatistics, typeof(MyMwcObjectBuilder_PlayerStatistics), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabSound, typeof(MyMwcObjectBuilder_PrefabSound), typeof(MyMwcObjectBuilder_PrefabSound_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabParticles, typeof(MyMwcObjectBuilder_PrefabParticles), typeof(MyMwcObjectBuilder_PrefabParticles_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabKinematic, typeof(MyMwcObjectBuilder_PrefabKinematic), typeof(MyMwcObjectBuilder_PrefabKinematic_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Session, typeof(MyMwcObjectBuilder_Session), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Event, typeof(MyMwcObjectBuilder_Event), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.ObjectToBuild, typeof(MyMwcObjectBuilder_ObjectToBuild), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.ObjectGroup, typeof(MyMwcObjectBuilder_ObjectGroup), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, typeof(MyMwcObjectBuilder_PrefabLargeShip), typeof(MyMwcObjectBuilder_PrefabLargeShip_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, typeof(MyMwcObjectBuilder_PrefabLargeWeapon), typeof(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabHangar, typeof(MyMwcObjectBuilder_PrefabHangar), typeof(MyMwcObjectBuilder_PrefabHangar_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.DummyPoint, typeof(MyMwcObjectBuilder_DummyPoint), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SnapPointLink, typeof(MyMwcObjectBuilder_SnapPointLink), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, typeof(MyMwcObjectBuilder_PrefabKinematicPart), typeof(MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.EntityDetector, typeof(MyMwcObjectBuilder_EntityDetector), typeof(MyMwcObjectBuilder_EntityDetector_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.ShipConfig, typeof(MyMwcObjectBuilder_ShipConfig), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, typeof(MyMwcObjectBuilder_PrefabFoundationFactory), typeof(MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB, typeof(MyMwcObjectBuilder_PrefabSecurityControlHUB), typeof(MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.FalseId, typeof(MyMwcObjectBuilder_FalseId), MyMwcFactionsByIndex.GetFactionsIndexes());
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Drone, typeof(MyMwcObjectBuilder_Drone), typeof(MyMwcObjectBuilder_Drone_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, typeof(MyMwcObjectBuilder_SmallShip_HackingTool), typeof(MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabBankNode, typeof(MyMwcObjectBuilder_PrefabBankNode), typeof(MyMwcObjectBuilder_PrefabBankNode_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.EntityUseProperties, typeof(MyMwcObjectBuilder_EntityUseProperties), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabGenerator, typeof(MyMwcObjectBuilder_PrefabGenerator), typeof(MyMwcObjectBuilder_PrefabGenerator_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.CargoBox, typeof(MyMwcObjectBuilder_CargoBox), typeof(MyMwcObjectBuilder_CargoBox_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SectorObjectGroups, typeof(MyMwcObjectBuilder_SectorObjectGroups), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.WaypointNew, typeof(MyMwcObjectBuilder_WaypointNew), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.MysteriousCube, typeof(MyMwcObjectBuilder_MysteriousCube), typeof(MyMwcObjectBuilder_MysteriousCube_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabScanner, typeof(MyMwcObjectBuilder_PrefabScanner), typeof(MyMwcObjectBuilder_PrefabScanner_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabAlarm, typeof(MyMwcObjectBuilder_PrefabAlarm), typeof(MyMwcObjectBuilder_PrefabAlarm_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabCamera, typeof(MyMwcObjectBuilder_PrefabCamera), typeof(MyMwcObjectBuilder_PrefabCamera_TypesEnum));
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabKinematicRotating, typeof(MyMwcObjectBuilder_PrefabKinematicRotating), typeof(MyMwcObjectBuilder_PrefabKinematicRotating_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart, typeof(MyMwcObjectBuilder_PrefabKinematicRotatingPart), typeof(MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum));
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.Meteor, typeof(MyMwcObjectBuilder_Meteor), typeof(MyMwcObjectBuilder_Meteor_TypesEnum));
            //AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.InfluenceSphereRadioactivity, typeof(MyMwcObjectBuilder_InfluenceSphereRadioactivity), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.InfluenceSphere, typeof(MyMwcObjectBuilder_InfluenceSphere), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShipTemplate, typeof(MyMwcObjectBuilder_SmallShipTemplate), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.SmallShipTemplates, typeof(MyMwcObjectBuilder_SmallShipTemplates), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.FactionRelationChange, typeof(MyMwcObjectBuilder_FactionRelationChange), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.RemotePlayer, typeof(MyMwcObjectBuilder_RemotePlayer), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.VoxelHand_Cylinder, typeof(MyMwcObjectBuilder_VoxelHand_Cylinder), (Type)null);
            AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum.GlobalData, typeof(MyMwcObjectBuilder_GlobalData), (Type)null);

            #region old
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallDebris, typeof(MyMwcObjectBuilder_SmallDebris));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.LargeDebrisField, typeof(MyMwcObjectBuilder_LargeDebrisField));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.LargeShip, typeof(MyMwcObjectBuilder_LargeShip));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, typeof(MyMwcObjectBuilder_SmallShip_Engine));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Player, typeof(MyMwcObjectBuilder_SmallShip_Player));
            ////AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Ally, typeof(MyMwcObjectBuilder_SmallShip_Ally));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, typeof(MyMwcObjectBuilder_SmallShip_Bot));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelMap, typeof(MyMwcObjectBuilder_VoxelMap));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial, typeof(MyMwcObjectBuilder_VoxelMap_MergeMaterial));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent, typeof(MyMwcObjectBuilder_VoxelMap_MergeContent));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Sector, typeof(MyMwcObjectBuilder_Sector));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour, typeof(MyMwcObjectBuilder_VoxelMap_Neighbour));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere, typeof(MyMwcObjectBuilder_VoxelHand_Sphere));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelHand_Box, typeof(MyMwcObjectBuilder_VoxelHand_Box));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid, typeof(MyMwcObjectBuilder_VoxelHand_Cuboid));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, typeof(MyMwcObjectBuilder_SmallShip_Ammo));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, typeof(MyMwcObjectBuilder_PrefabLargeWeapon));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, typeof(MyMwcObjectBuilder_SmallShip_Weapon));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.StaticAsteroid, typeof(MyMwcObjectBuilder_StaticAsteroid));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, typeof(MyMwcObjectBuilder_SmallShip_Tool));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip, typeof(MyMwcObjectBuilder_SmallShip));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo, typeof(MyMwcObjectBuilder_AssignmentOfAmmo));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, typeof(MyMwcObjectBuilder_LargeShip_Ammo));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Prefab, typeof(MyMwcObjectBuilder_Prefab));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabContainer, typeof(MyMwcObjectBuilder_PrefabContainer));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.InfluenceSphereDust, typeof(MyMwcObjectBuilder_InfluenceSphereDust));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.InfluenceSphereSound, typeof(MyMwcObjectBuilder_InfluenceSphereSound));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabLight, typeof(MyMwcObjectBuilder_PrefabLight));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SpawnPoint, typeof(MyMwcObjectBuilder_SpawnPoint));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, typeof(MyMwcObjectBuilder_SmallShip_Armor));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, typeof(MyMwcObjectBuilder_SmallShip_Radar));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Ore, typeof(MyMwcObjectBuilder_Ore));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Blueprint, typeof(MyMwcObjectBuilder_Blueprint));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.FoundationFactory, typeof(MyMwcObjectBuilder_FoundationFactory));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Inventory, typeof(MyMwcObjectBuilder_Inventory));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.InventoryItem, typeof(MyMwcObjectBuilder_InventoryItem));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Checkpoint, typeof(MyMwcObjectBuilder_Checkpoint));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Player, typeof(MyMwcObjectBuilder_Player));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PlayerStatistics, typeof(MyMwcObjectBuilder_PlayerStatistics));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabSound, typeof(MyMwcObjectBuilder_PrefabSound));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabParticles, typeof(MyMwcObjectBuilder_PrefabParticles));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabKinematic, typeof(MyMwcObjectBuilder_PrefabKinematic));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Session, typeof(MyMwcObjectBuilder_Session));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.Event, typeof(MyMwcObjectBuilder_Event));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.WayPoint, typeof(MyMwcObjectBuilder_WayPoint));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.WayPointGroup, typeof(MyMwcObjectBuilder_WayPointGroup));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.ObjectToBuild, typeof(MyMwcObjectBuilder_ObjectToBuild));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.ObjectGroup, typeof(MyMwcObjectBuilder_ObjectGroup));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, typeof(MyMwcObjectBuilder_PrefabLargeShip));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, typeof(MyMwcObjectBuilder_PrefabLargeWeapon));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.PrefabHangar, typeof(MyMwcObjectBuilder_PrefabHangar));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.DummyPoint, typeof(MyMwcObjectBuilder_DummyPoint));
            //AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum.SnapPointLink, typeof(MyMwcObjectBuilder_SnapPointLink));            

            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallDebris, typeof(MyMwcObjectBuilder_SmallDebris_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.LargeDebrisField, typeof(MyMwcObjectBuilder_LargeDebrisField_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.LargeShip, typeof(MyMwcObjectBuilder_LargeShip_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, typeof(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Player, typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            ////AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Ally, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelMap, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent,null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Sector, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelHand_Box, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, typeof(MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.StaticAsteroid, typeof(MyMwcObjectBuilder_StaticAsteroid_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, typeof(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip, typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, typeof(MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Prefab, typeof(MyMwcObjectBuilder_Prefab_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.PrefabContainer, typeof(MyMwcObjectBuilder_PrefabContainer_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.InfluenceSphereDust, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.InfluenceSphereSound, null);
            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabLight, 
            //                    new int[]{
            //                        (int)MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_LIGHT_0, 
            //                        (int)MyMwcObjectBuilder_Prefab_TypesEnum.P521_A01_LIGHT1, 
            //                        (int)MyMwcObjectBuilder_Prefab_TypesEnum.P521_A02_LIGHT2, 
            //                        (int)MyMwcObjectBuilder_Prefab_TypesEnum.P521_A03_LIGHT3, 
            //                        (int)MyMwcObjectBuilder_Prefab_TypesEnum.P521_A04_LIGHT4
            //                    });
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SpawnPoint, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, typeof(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, typeof(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Ore, typeof(MyMwcObjectBuilder_Ore_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Blueprint, typeof(MyMwcObjectBuilder_Blueprint_TypesEnum));
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.FoundationFactory, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Inventory, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.InventoryItem, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Checkpoint, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Player, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.PlayerStatistics, null);
            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabSound, 
            //                    new int[]{
            //                            (int)MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_SOUND_PREFAB_0, 
            //                            (int)MyMwcObjectBuilder_Prefab_TypesEnum.P561_A01_SOUND, 
            //                            (int)MyMwcObjectBuilder_Prefab_TypesEnum.P561_B01_SOUND, 
            //                            (int)MyMwcObjectBuilder_Prefab_TypesEnum.P561_C01_SOUND, 
            //                            (int)MyMwcObjectBuilder_Prefab_TypesEnum.P561_D01_SOUND
            //                    });
            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabParticles,
            //                    new int[]{
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_PARTICLE_PREFAB_0,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P551_A01_PARTICLES,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P551_B01_PARTICLES,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P551_C01_PARTICLES,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P551_D01_PARTICLES
            //                    });
            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabKinematic,
            //                     new int[]{
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_A01_DOORCASE,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE,
            //                    });


            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Session, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.Event, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.WayPoint, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.WayPointGroup, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.ObjectToBuild, null);
            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.ObjectGroup, null);

            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
            //     new int[]{
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_ARDANT,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_KAI,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_SAYA,
            //                    });
            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
            //     new int[]{
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_CIWS,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9,
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P351_A01_WEAPON_MOUNT,
            //                    });

            //AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum.PrefabHangar,
            //     new int[]{
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P361_A02_HANGAR_PANEL,                                        
            //                            (int) MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_SMALL_HANGAR,                                        
            //                    });

            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.DummyPoint, null);

            //AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum.SnapPointLink, null);
            #endregion

            //  This is just a check (assertion) if we didn't forget define something
            foreach (ushort num in Enum.GetValues(typeof(MyMwcObjectBuilderTypeEnum)))
            {
                MyCommonDebugUtils.AssertDebug(m_objectBuilderDefinitions[num] != null);
                MyCommonDebugUtils.AssertDebug(m_objectBuilderDefinitions[num].Ids != null);
                MyCommonDebugUtils.AssertDebug(CreateNewObject((MyMwcObjectBuilderTypeEnum)num).GetObjectBuilderType() == (MyMwcObjectBuilderTypeEnum)num);

                //MyCommonDebugUtils.AssertDebug(m_objectBuilderToType[num] != null);
                //MyCommonDebugUtils.AssertDebug(m_objectBuilderIDs[num] != null && m_objectBuilderIDs[num].Length != 0);
            }
        }

        // Used when loading objects from different sector
        // EntityIds must be remapped to prevent id collision with existing entities
        // Parent must call this on children! (e.g. prefab container on prefabs)
        public virtual void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            EntityId = remapContext.RemapEntityId(EntityId);
        }

        public MyMwcObjectBuilder_Base Clone()
        {
            // Write
            m_cloneWriter.Seek(0, SeekOrigin.Begin);
            Write(m_cloneWriter);

            // Read
            m_cloneStream.Seek(0, SeekOrigin.Begin);

            var result = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(m_cloneReader, fakeEp);
            if (result == null || !result.Read(m_cloneReader, fakeEp, MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION))
            {
                throw new InvalidOperationException("Clone object error, (de)serialization is broken!");
            }
            return result;
        }

        // this method is there because we need load and check asserts when the game started
        public static void Check()
        {
            MyMwcLog.WriteLine("MyMwcObjectBuilder_Base.Check()");
        }

        /// <summary>
        /// Clears entity id for this object builder and for all childs
        /// </summary>
        public virtual void ClearEntityId()
        {
            this.EntityId = null;
        }

        //static void AddObjectBuilderToType(MyMwcObjectBuilderTypeEnum objectBuilderType, Type type)
        //{
        //    m_objectBuilderToType[(int)objectBuilderType] = type;
        //}

        protected bool NetworkError(string text = null)
        {
            // Only store first error
            if (String.IsNullOrEmpty(m_networkError))
            {
                m_networkError = "Object builder (de)serialization error";
                if (!String.IsNullOrEmpty(text))
                {
                    m_networkError += ": " + text;
                }

                if (MyMwcFinalBuildConstants.IS_DEVELOP)
                {
                    m_networkError += Environment.NewLine + "Stack trace: " + Environment.NewLine + Environment.StackTrace;
                }
                else
                {
                    m_networkError += Environment.NewLine + "Error in type: " + this.GetObjectBuilderType().ToString();
                }
                m_networkError += Environment.NewLine;
            }
            return false;
        }

        static void AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum objectBuilderType, Type type, int[] ids)
        {
            m_objectBuilderDefinitions[(int)objectBuilderType] = new MyMwcObjectBuilderDefinition(type, ids);
        }

        static void AddObjectBuilderDefinition(MyMwcObjectBuilderTypeEnum objectBuilderType, Type type, Type enumType)
        {
            if (enumType == null)
            {
                AddObjectBuilderDefinition(objectBuilderType, type, new int[] { 0 });
            }
            else
            {
                MyCommonDebugUtils.AssertDebug(enumType.IsEnum);

                AddObjectBuilderDefinition(objectBuilderType, type, GetIntArrayFromEnum(enumType));
            }
        }

        public static Type GetObjectBuilderToType(MyMwcObjectBuilderTypeEnum objectBuilderType)
        {
            //return m_objectBuilderToType[(int)objectBuilderType];
            return m_objectBuilderDefinitions[(int)objectBuilderType].Type;
        }

        //static void AddObjectBuilderIDs(MyMwcObjectBuilderTypeEnum objectBuilderType, int[] ids)
        //{
        //    m_objectBuilderIDs[(int) objectBuilderType] = ids;
        //}

        //static void AddObjectBuilderSubTypeEnums(MyMwcObjectBuilderTypeEnum objectBuilderType, Type enumType)
        //{
        //    if(enumType == null)
        //    {
        //        AddObjectBuilderIDs(objectBuilderType, new int[]{0});
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.Assert(enumType.IsEnum);

        //        AddObjectBuilderIDs(objectBuilderType, GetIntArrayFromEnum(enumType));
        //    }
        //}

        static int[] GetIntArrayFromEnum(Type enumType)
        {
            List<int> intValues = new List<int>();

            Array values = Enum.GetValues(enumType);
            Type underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType == typeof(System.Byte))
            {
                foreach (byte value in values)
                {
                    intValues.Add(value);
                }
            }
            else if (underlyingType == typeof(System.Int16))
            {
                foreach (short value in values)
                {
                    intValues.Add(value);
                }
            }
            else if (underlyingType == typeof(System.UInt16))
            {
                foreach (ushort value in values)
                {
                    intValues.Add(value);
                }
            }
            else if (underlyingType == typeof(System.Int32))
            {
                foreach (int value in values)
                {
                    intValues.Add(value);
                }
            }

            return intValues.ToArray();
        }

        public static int[] GetObjectBuilderIDs(MyMwcObjectBuilderTypeEnum objectBuilderType)
        {
            //return m_objectBuilderIDs[(int) objectBuilderType];
            return m_objectBuilderDefinitions[(ushort)objectBuilderType].Ids;
        }

        //[System.Obsolete("WCF needs some internal members for (de)serialization")]
        //public static MyMwcObjectBuilder_Base ReadAndCreateNewObjectWrapper(BinaryReader binaryReader, EndPoint senderEndPoint)
        //{
        //    return ReadAndCreateNewObject(binaryReader, senderEndPoint);
        //}

        //  Reads object type and then creates it
        //  If error occures, null is returned.
        internal static MyMwcObjectBuilder_Base ReadAndCreateNewObject(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            MyMwcObjectBuilderTypeEnum? objectBuilderType = MyMwcMessageIn.ReadObjectBuilderTypeEnumEx(binaryReader, senderEndPoint);
            if (objectBuilderType == null) return null;
            return CreateNewObject(objectBuilderType.Value);
        }

        //  Allocate new object builder according to specified type and returns it
        static MyMwcObjectBuilder_Base CreateNewObject(MyMwcObjectBuilderTypeEnum objectBuilderType)
        {
            return (MyMwcObjectBuilder_Base)Activator.CreateInstance(GetObjectBuilderToType(objectBuilderType), true);
        }

        public static MyMwcObjectBuilder_Base CreateNewObject(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId)
        {
            MyMwcObjectBuilder_Base objectBuilder = (MyMwcObjectBuilder_Base)Activator.CreateInstance(GetObjectBuilderToType(objectBuilderType), true);
            objectBuilder.SetDefaultProperties(); // Health for prefabs etc
            objectBuilder.SetObjectBuilderId(objectBuilderId);
            return objectBuilder;
        }

        public MyPersistentEntityFlags PersistentFlags { get; set; }

        public virtual bool Enabled
        {
            get
            {
                return (PersistentFlags & MyPersistentEntityFlags.Enabled) != 0;
            }
            set
            {
                if (value)
                {
                    PersistentFlags |= MyPersistentEntityFlags.Enabled;
                }
                else
                {
                    PersistentFlags &= (~MyPersistentEntityFlags.Enabled);
                }
            }
        }

        public virtual bool IsDestructible
        {
            get
            {
                return (PersistentFlags & MyPersistentEntityFlags.Destructible) != 0;
            }
            set
            {
                if (value)
                {
                    PersistentFlags |= MyPersistentEntityFlags.Destructible;
                }
                else
                {
                    PersistentFlags &= (~MyPersistentEntityFlags.Destructible);
                }
            }
        }

        private uint? eid;
        public uint? EntityId
        {
            get { return eid; }
            set { eid = value; }
        }

        public string Name;

        public abstract MyMwcObjectBuilderTypeEnum GetObjectBuilderType();

        // Most object builders contain enum values, that are definition of concrete objects of given object builder type. For the DB purpose,
        // int is used, but for networking, most of these enums have different data type(ushort, byte, etc.). Some object builders does not have this
        // enum(in its category, there wont be any more types of objects), thus it can be null.
        public virtual int? GetObjectBuilderId()
        {
            return null;
        }

        // Each object builder will be able to uniquely set its definition of conrete object this way
        internal virtual void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
        }

        // Set default properties, for example health for prefabs
        public virtual void SetDefaultProperties()
        {
        }

        public void SetObjectBuilderId(int? objectBuilderId)
        {
            // we need validate id first
            int id = objectBuilderId == null ? 0 : objectBuilderId.Value;
            MyCommonDebugUtils.AssertDebug(IsObjectBuilderIdValid(id));

            SetObjectBuilderIdInternal(objectBuilderId);
        }

        //public virtual bool IsObjectBuilderIdValid(int objectBuilderId)
        //{
        //    return true;
        //}

        public bool IsObjectBuilderIdValid(int objectBuilderId)
        {
            return IsObjectBuilderIdValid(GetObjectBuilderType(), objectBuilderId);
        }

        public static bool IsObjectBuilderIdValid(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId)
        {
            foreach (int definedId in m_objectBuilderDefinitions[(int)objectBuilderType].Ids)
            {
                if (definedId == objectBuilderId)
                {
                    return true;
                }
            }
            return false;
        }

        //  Write this object into message-out
        internal virtual void Write(BinaryWriter binaryWriter)
        {
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.GetObjectBuilderType: " + GetObjectBuilderType());
            MyMwcMessageOut.WriteObjectBuilderTypeEnum(GetObjectBuilderType(), binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.HasValue: " + this.EntityId.HasValue);
            MyMwcMessageOut.WriteBool(this.EntityId.HasValue, binaryWriter);
            if (this.EntityId.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.Value: " + this.EntityId.Value);
                MyMwcMessageOut.WriteUInt32(this.EntityId.Value, binaryWriter);
            }

            if (Name != null)
            {
                MyMwcMessageOut.WriteBool(true, binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + this.Name);
                MyMwcMessageOut.WriteString(Name, binaryWriter);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + "null");
                MyMwcMessageOut.WriteBool(false, binaryWriter);
            }

            // Flags
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Flags: " + this.PersistentFlags);
            MyMwcMessageOut.WriteInt32((int)this.PersistentFlags, binaryWriter);
        }

        //  Read this object from message-in
        //  Return true if reading was successful. False if reading one of the parameters failed - therefore if object is incomplete.
        internal virtual bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (gameVersion < FIRST_COMPATIBILITY_VERSION)
                return false;

            if (gameVersion > 01085000)
            {
                return Read_Current(binaryReader, senderEndPoint, gameVersion);
            }
            else
            {
                return Read_01085000(binaryReader, senderEndPoint, gameVersion);
            }
        }

        private bool Read_Current(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            //  Read method in this base class always returns true, because it actually doesn't read nothing
            // It does, it reads EntityId

            bool? hasId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasId.HasValue) return NetworkError(); // Cannot read bool - whether entity id is null or not
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.HasValue: " + hasId.Value);

            // Testing whether entity id is null
            if (hasId.Value)
            {
                // entity id has value - read the value
                uint? entityId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                if (!entityId.HasValue) return NetworkError(); // Cannot read entityId

                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.Value: " + entityId.Value);
                this.EntityId = entityId.Value;
            }
            else
            {
                this.EntityId = null;
            }

            bool? isNamed = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!isNamed.HasValue) return NetworkError();
            if (isNamed.Value)
            {
                string name = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (name == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + name);
                Name = name;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + "null");
                Name = null;
            }

            // Flags
            int? flagsResult = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!flagsResult.HasValue) return NetworkError();

            this.PersistentFlags = (MyPersistentEntityFlags)flagsResult.Value;
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Flags: " + this.PersistentFlags);

            return true;
        }

        private bool Read_01085000(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            //  Read method in this base class always returns true, because it actually doesn't read nothing
            // It does, it reads EntityId

            bool? hasId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasId.HasValue) return NetworkError(); // Cannot read bool - whether entity id is null or not
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.HasValue: " + hasId.Value);

            // Testing whether entity id is null
            if (hasId.Value)
            {
                // entity id has value - read the value
                uint? entityId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                if (!entityId.HasValue) return NetworkError(); // Cannot read entityId

                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.EntityId.Value: " + entityId.Value);
                this.EntityId = entityId.Value;
            }
            else
            {
                this.EntityId = null;
            }

            bool? isNamed = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!isNamed.HasValue) return NetworkError();
            if (isNamed.Value)
            {
                string name = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (name == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + name);
                Name = name;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Name: " + "null");
                Name = null;
            }

            // Flags
            int? flagsResult = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!flagsResult.HasValue) return NetworkError();

            this.PersistentFlags = (MyPersistentEntityFlags)flagsResult.Value;
            this.PersistentFlags |= MyPersistentEntityFlags.ActivatedOnDifficultyEasy | MyPersistentEntityFlags.ActivatedOnDifficultyNormal | MyPersistentEntityFlags.ActivatedOnDifficultyHard;
            if (this is MyMwcObjectBuilder_CargoBox || this is MyMwcObjectBuilder_SmallShip) 
            {
                this.PersistentFlags |= MyPersistentEntityFlags.DisplayOnHud;
            }
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Base.Flags: " + this.PersistentFlags);

            return true;
        }

        public static T FromBytes<T>(byte[] bytes, int gameVersion = MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION)
            where T : MyMwcObjectBuilder_Base
        {
            var fakeEndpoint = new System.Net.IPEndPoint(0, 0);

            using (MemoryStream ms = new MemoryStream(bytes))
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
            using (BinaryReader reader = new BinaryReader(gzip))
            {
                T result = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(reader, fakeEndpoint) as T;
                if (result == null || result.Read(reader, fakeEndpoint, gameVersion) == false)
                {
                    MyMwcLog.WriteLine("Networking deserialization error: " + Environment.NewLine + GetAndClearLastError());
                    return null;
                }
                return result;
            }
        }

        public byte[] ToBytes()
        {
            // NOTE: To achieve better compression (30% ratio compared to 90%), 
            // it necessary to write data to uncompressed stream and then copy uncompressed stream to compressed stream
            // It's because GZipStream compress each data written to stream separatelly

            byte[] buffer = new byte[1024 * 1024];
            long remaining = 0;

            using (MemoryStream dataStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(dataStream))
            {
                this.Write(writer); // First write data to uncompressed stream

                using (MemoryStream compressed = new MemoryStream())
                using (GZipStream gzip = new GZipStream(compressed, CompressionMode.Compress, true))
                {
                    remaining = dataStream.Length;
                    dataStream.Position = 0;

                    while (remaining > 0)
                    {
                        //var dataBytes = dataStream.ToArray();
                        long read = dataStream.Read(buffer, 0, (int)Math.Min(remaining, buffer.Length));
                        
                        //gzip.Write(dataBytes, 0, dataBytes.Length); // Write uncompressed stream to compressed stream
                        gzip.Write(buffer, 0, (int)read); // Write uncompressed stream to compressed stream

                        remaining -= read;
                    }
                    gzip.Close();
                    return compressed.ToArray();
                }
            }
            /*
            using (MemoryStream compressed = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(compressed, CompressionMode.Compress, true))
                {
                    using (BinaryWriter writer = new BinaryWriter(gzip))
                    {
                        this.Write(writer); // First write data to uncompressed stream
                    }
                }

                return compressed.ToArray();
            }*/

        }

        public static string GetAndClearLastError()
        {
            var result = m_networkError;
            m_networkError = null;
            return result;
        }
    }
}

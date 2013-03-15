using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.BackgroundCube;

namespace MinerWars.AppCode.Game.SolarSystem
{                  
    partial class MySolarSystemConstants
    {
        static MySectorObjectCounts SlaversBase2ObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 30,
            StaticAsteroidMedium = 2000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] SlaversBase2ImpostorsProperties = new MyImpostorProperties[]
        {
                   new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_A,
                      ImpostorsCount = 1000,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 5,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 10,
                      MinRadius = 1000,
                      MaxRadius = 2000,
                      AnimationSpeed = new Vector4(0.00009f,0,0,0),
                      Color = Vector3.One
                  },

                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_C,
                      ImpostorsCount = 1000,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 10.1f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 11,
                      MinRadius = 1000,
                      MaxRadius = 2000,
                      AnimationSpeed = new Vector4(-0.0001f,0,0,0),
                      Color = Vector3.One
                  },

                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Nebula,
                      Material = null,
                      ImpostorsCount = 0,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.0f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.3f,
                      MinRadius = 0.93f,
                      MaxRadius = 0.93f,
                      AnimationSpeed = new Vector4(0.003f, 0.001f,0.003f, 0),
                      Color = new Vector3(65 / 255.0f, 225 / 255.0f, 208 / 255.0f),
                      Contrast = 1.537f,
                      Intensity = 0.44f
                  },
        };


        public static readonly MySunProperties SlaversBase2SunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(176 / 255.0f, 238 / 255.0f, 238 / 255.0f),
            SunIntensity = 2.106f,
            SunSpecular = new Vector3(32 / 255.0f, 56 / 255.0f, 42 / 255.0f),
            BackSunDiffuse = new Vector3(27 / 255.0f, 153 / 255.0f, 225 / 255.0f),
            BackSunIntensity = 2.206f,
            AmbientColor = new Vector3(36 / 255.0f, 36 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.765f,
            EnvironmentAmbientIntensity = 0.261f,
        };


        public static readonly MyFogProperties SlaversBase2FogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(0 / 255.0f, 0 / 255.0f, 205 / 255.0f),
            FogNear = 4073,
            FogFar = 4726,
            FogMultiplier = 0.161f,
            FogBacklightMultiplier = 1.177f,
        };

        public static readonly MyParticleDustProperties SlaversBase2DustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 108.650f,
            DustFieldCountInDirectionHalf = 5.835f,
            DistanceBetween = 128.44f,
            Color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 1f),
        };

                      //Not worth of limiting, it is used in multiplayer maps where is everything..
        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> SlaversBase2AsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {     /*   
            {  MyMwcVoxelMaterialsEnum.Stone_05, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_06, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_10, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_01, 1},   //because of Junkyard mines in multiplayer
           */
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> SlaversBase2SecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {     /*
            {  MyMwcVoxelMaterialsEnum.Helium3_01, 1},
            {  MyMwcVoxelMaterialsEnum.Ice_01, 1}, //because of Ice cave in multiplayer
            {  MyMwcVoxelMaterialsEnum.Uranite_01, 1}, //because of Plutonium mines in multiplayer
            {  MyMwcVoxelMaterialsEnum.Indestructible_01, 1},   //because of Junkyard mines in multiplayer
            {  MyMwcVoxelMaterialsEnum.Cobalt_01, 1},   //because of Junkyard mines in multiplayer
            {  MyMwcVoxelMaterialsEnum.Lava_01, 1},   //because of Junkyard mines in multiplayer
            {  MyMwcVoxelMaterialsEnum.Treasure_01, 1},   //because of Junkyard mines in multiplayer
               */ 
        };             
                      


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateSlaversBase2Area()
        {
            Areas[MySolarSystemAreaEnum.SlaversBase2] = new MySolarSystemAreaOrbit()
            {
                Name = "Slavers area 2",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.SLAVERS2_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.SLAVERSBASE2_RADIUS * MyBgrCubeConsts.MILLION_KM * 200,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.SLAVERSBASE2_RADIUS * MyBgrCubeConsts.MILLION_KM * 1400,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.8f, 0.35f, 0.16f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 5, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = SlaversBase2SunProperties,
                    FogProperties = SlaversBase2FogProperties,
                    ImpostorProperties = SlaversBase2ImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = SlaversBase2ObjectsProperties,
                    PrimaryAsteroidMaterials = SlaversBase2AsteroidMaterials,
                    SecondaryAsteroidMaterials = SlaversBase2SecondaryMaterials,

                    ParticleDustProperties = SlaversBase2DustProperties,
                },
            };
        }
    }
}

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
        static MySectorObjectCounts UranObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 150,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] UranImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 1.9f,
                      MaxRadius = 1.9f,
                      AnimationSpeed = new Vector4(0.005f, 0.001f,0.003f, 0),
                      Color = new Vector3(17 / 255.0f, 95 / 255.0f, 110 / 255.0f),
                      Contrast = 19.170f,
                      Intensity = 0.656f
                  },
        };


        public static readonly MySunProperties UranSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(179 / 255.0f, 160 / 255.0f, 127 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.URANUS_POSITION) * 5,
            SunSpecular = new Vector3(57 / 255.0f, 136 / 255.0f, 192 / 255.0f),
            BackSunDiffuse = new Vector3(254 / 255.0f, 254 / 255.0f, 254 / 255.0f),
            BackSunIntensity = 1.05f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.45f,
        };


        public static readonly MyFogProperties UranFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(52 / 255.0f, 99 / 255.0f, 73 / 129.0f),
            FogNear = 3000,
            FogFar = 13000,
            FogMultiplier = 0.65f,
        };

        public static readonly MyParticleDustProperties UranDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 125.96f,
            DustFieldCountInDirectionHalf = 5.177f,
            DistanceBetween = 176.900f,
            Color = new Color(0 / 255.0f, 90 / 255.0f, 90 / 255.0f, 1f),
        };
        
        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> UranAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Stone_03, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, 1.0f },
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> UranSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Stone_03, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Snow_01, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Helium4_01, 1.0f },
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateUranArea()
        {
            Areas[MySolarSystemAreaEnum.Uranus] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Uranus",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.URANUS_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.URANUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.URANUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = UranSunProperties,
                    FogProperties = UranFogProperties,
                    ImpostorProperties = UranImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = UranObjectsProperties,
                    PrimaryAsteroidMaterials = UranAsteroidMaterials,
                    SecondaryAsteroidMaterials = UranSecondaryMaterials,

                    ParticleDustProperties = UranDustProperties,
                },
            };
        }
    }
}

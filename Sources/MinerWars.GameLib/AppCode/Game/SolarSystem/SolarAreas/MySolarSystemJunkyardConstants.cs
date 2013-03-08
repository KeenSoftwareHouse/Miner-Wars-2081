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
        static MySectorObjectCounts JunkyardObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] JunkyardImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 1.328f,
                      MaxRadius = 1.328f,
                      AnimationSpeed = new Vector4(0.005f, 0.001f,0.003f, 0),
                      Color = new Vector3(255 / 255.0f, 251 / 255.0f, 208 / 255.0f),
                      Contrast = 2.664f,
                      Intensity = 0.604f
                  },
        };


        public static readonly MySunProperties JunkyardSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(0 / 255.0f, 0 / 255.0f, 0 / 255.0f),
            SunIntensity = 0,
            SunSpecular = new Vector3(187 / 255.0f, 82 / 255.0f, 50 / 255.0f),
            BackSunDiffuse = new Vector3(255 / 255.0f, 246 / 255.0f, 239 / 255.0f),
            BackSunIntensity = 0.477f,
            AmbientColor = new Vector3(36 / 255.0f, 36 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.441f,
        };


        public static readonly MyFogProperties JunkyardFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(139 / 255.0f, 134 / 255.0f, 130 / 255.0f),
            FogNear = 3000,
            FogFar = 7338,
            FogMultiplier = 0.226f,
            FogBacklightMultiplier = 0.851f,
        };

        public static readonly MyParticleDustProperties JunkyardDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 103.054f,
            DustFieldCountInDirectionHalf = 8.072f,
            DistanceBetween = 127.778f,
            Color = new Color(205 / 255.0f, 197 / 255.0f, 170 / 255.0f),
        };


        public static readonly MyDebrisProperties JunkyardDebrisProperties = new MyDebrisProperties()
        {
            Enabled = true,
            MaxDistance = 200,
            CountInDirectionHalf = 4,
            FullScaleDistance = 70,
            DistanceBetween = 30.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> JunkyardAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {        
            {  MyMwcVoxelMaterialsEnum.Stone_05, 1},
            {  MyMwcVoxelMaterialsEnum.Ice_01, 1},
             {  MyMwcVoxelMaterialsEnum.Sandstone_01, 1},
              {  MyMwcVoxelMaterialsEnum.Gold_01, 1},
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> JunkyardSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Lava_01, 1},
            {  MyMwcVoxelMaterialsEnum.Uranite_01, 1},
            {  MyMwcVoxelMaterialsEnum.Cobalt_01, 1},
            {  MyMwcVoxelMaterialsEnum.Helium3_01, 1},
        };



        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateJunkyardArea()
        {
            Areas[MySolarSystemAreaEnum.Junkyard] = new MySolarSystemAreaOrbit()
            {
                Name = "Junkyard",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MySolarSystemUtils.SectorsToKm(MyBgrCubeConsts.JUNKYARD_SECTOR),
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = 2.1f * MyBgrCubeConsts.MILLION_KM,
                    MaxDistanceFromOrbitHigh = 8.4f * MyBgrCubeConsts.MILLION_KM,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.2f, 0.21f, 0.18f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = JunkyardSunProperties,
                    FogProperties = JunkyardFogProperties,
                    ImpostorProperties = JunkyardImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = JunkyardObjectsProperties,
                    PrimaryAsteroidMaterials = JunkyardAsteroidMaterials,
                    SecondaryAsteroidMaterials = JunkyardSecondaryMaterials,

                    ParticleDustProperties = JunkyardDustProperties,
                },
            };
        }
    }
}

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
        static MySectorObjectCounts ChineseRafinaryObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 3000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] ChineseRafinaryImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.930f,
                      MaxRadius = 0.930f,
                      AnimationSpeed = new Vector4(0.003f, 0.001f,0.003f, 0),
                      Color = MyMath.VectorFromColor(255, 13, 0),
                      Contrast = 1.537f,
                      Intensity = 0.320f
                  },
        };


        public static readonly MySunProperties ChineseRafinarySunProperties = new MySunProperties()
        {
            SunDiffuse = MyMath.VectorFromColor(255, 166, 72),
            SunIntensity = 1.242f,
            SunSpecular = MyMath.VectorFromColor(187, 89, 50),
            BackSunDiffuse = MyMath.VectorFromColor(163, 144, 95),
            BackSunIntensity = 3.395f,
            AmbientColor = MyMath.VectorFromColor(36, 36, 36),
            AmbientMultiplier = 0.765f,
            EnvironmentAmbientIntensity = 0.261f,
        };


        public static readonly MyFogProperties ChineseRafinaryFogProperties = new MyFogProperties()
        {
            FogColor = MyMath.VectorFromColor(255, 0, 0),
            FogNear = 4073.413f,
            FogFar = 4867.492f,
            FogMultiplier = 0.151f,
            FogBacklightMultiplier = 1.643f,
        };

        public static readonly MyParticleDustProperties ChineseRafinaryDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 121.709f,
            DustFieldCountInDirectionHalf = 6.610f,
            DistanceBetween = 128.358f,
            Color = new Color(0, 0, 0),
        };


        public static readonly MyDebrisProperties ChineseRafinaryDebrisProperties = new MyDebrisProperties()
        {
            Enabled = false,
            MaxDistance = 200,
            CountInDirectionHalf = 4,
            FullScaleDistance = 70,
            DistanceBetween = 30.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ChineseRafinaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {        
            {  MyMwcVoxelMaterialsEnum.Stone_05, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_06, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_07, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, 1},
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ChineseRafinarySecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Nickel_01, 1},
            {  MyMwcVoxelMaterialsEnum.Platinum_01, 1},
        };


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateChineseRafinaryArea()
        {
            Areas[MySolarSystemAreaEnum.ChineseRafinary] = new MySolarSystemAreaOrbit()
            {
                Name = "Chinese Rafinary",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MySolarSystemUtils.SectorsToKm(MyBgrCubeConsts.CHINESERAFINARY_SECTOR),
                    LongSpread = 0.15f,
                    MaxDistanceFromOrbitLow = 4.1f * MyBgrCubeConsts.MILLION_KM,
                    MaxDistanceFromOrbitHigh = 9.4f * MyBgrCubeConsts.MILLION_KM,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.32f, 0.71f, 0.18f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = ChineseRafinarySunProperties,
                    FogProperties = ChineseRafinaryFogProperties,
                    ImpostorProperties = ChineseRafinaryImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = ChineseRafinaryObjectsProperties,
                    PrimaryAsteroidMaterials = ChineseRafinaryAsteroidMaterials,
                    SecondaryAsteroidMaterials = ChineseRafinarySecondaryMaterials,

                    ParticleDustProperties = ChineseRafinaryDustProperties,
                },
            };
        }
    }
}

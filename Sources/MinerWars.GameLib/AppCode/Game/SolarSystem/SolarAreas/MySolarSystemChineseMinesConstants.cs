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
        static MySectorObjectCounts ChineseMinesObjectsProperties = new MySectorObjectCounts()
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

        public readonly static MyImpostorProperties[] ChineseMinesImpostorsProperties = new MyImpostorProperties[]
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
                      AnimationSpeed = new Vector4(0.005f, 0.001f, 0.003f, 0),
                      Color = MyMath.VectorFromColor(170, 255, 35),
                      Contrast = 2.664f,
                      Intensity = 0.604f
                  },
        };


        public static readonly MySunProperties ChineseMinesSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(175 / 255.0f, 255 / 255.0f, 13 / 255.0f),
            SunIntensity = 0f,
            SunSpecular = new Vector3(187 / 255.0f, 82 / 255.0f, 50 / 255.0f),
            BackSunDiffuse = new Vector3(203 / 255.0f, 255 / 255.0f, 0 / 255.0f),
            BackSunIntensity = 1.197f,
            AmbientColor = new Vector3(36 / 255.0f, 36 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.441f,
        };


        public static readonly MyFogProperties ChineseMinesFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(179 / 255.0f, 161 / 255.0f, 0 / 255.0f),
            FogNear = 3000,
            FogFar = 7338,
            FogMultiplier = 0.226f,
            FogBacklightMultiplier = 0.851f,
        };

        public static readonly MyParticleDustProperties ChineseMinesDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 103.054f,
            DustFieldCountInDirectionHalf = 8.072f,
            DistanceBetween = 123.123f,
            Color = new Color(151 / 255.0f, 201 / 255.0f, 49 / 255.0f),
        };


        public static readonly MyDebrisProperties ChineseMinesDebrisProperties = new MyDebrisProperties()
        {
            Enabled = false,
            MaxDistance = 200,
            CountInDirectionHalf = 4,
            FullScaleDistance = 70,
            DistanceBetween = 30.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };


        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ChineseMinesAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {        
            {  MyMwcVoxelMaterialsEnum.Stone_07, 1},
            {  MyMwcVoxelMaterialsEnum.Stone_03, 1},
            {  MyMwcVoxelMaterialsEnum.Indestructible_02, 1},
            {  MyMwcVoxelMaterialsEnum.Indestructible_03, 1},

        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ChineseMinesSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Iron_01, 1},
            {  MyMwcVoxelMaterialsEnum.Iron_02, 1},
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateChineseMinesArea()
        {
            Areas[MySolarSystemAreaEnum.ChineseMines] = new MySolarSystemAreaOrbit()
            {
                Name = "Chinese Mines",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MySolarSystemUtils.SectorsToKm(MyBgrCubeConsts.CHINESEMINES_SECTOR),
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = 2.1f * MyBgrCubeConsts.MILLION_KM,
                    MaxDistanceFromOrbitHigh = 8.4f * MyBgrCubeConsts.MILLION_KM,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.78f, 0.23f, 0.78f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = ChineseMinesSunProperties,
                    FogProperties = ChineseMinesFogProperties,
                    ImpostorProperties = ChineseMinesImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = ChineseMinesObjectsProperties,
                    PrimaryAsteroidMaterials = ChineseMinesAsteroidMaterials,
                    SecondaryAsteroidMaterials = ChineseMinesSecondaryMaterials,

                    ParticleDustProperties = ChineseMinesDustProperties,
                },
            };
        }
    }
}

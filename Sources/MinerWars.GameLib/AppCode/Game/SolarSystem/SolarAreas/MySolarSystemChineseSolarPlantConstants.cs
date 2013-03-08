using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.BackgroundCube;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
        static MySectorObjectCounts ChinesePowerPlantObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 1000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] ChinesePowerPlantImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.9f,
                      MaxRadius = 0.9f,
                      AnimationSpeed = new Vector4(-0.005f, -0.008f, -0.003f, 0),
                      Color = new Vector3(255 / 255.0f, 255 / 255.0f, 146 / 255.0f),
                      Contrast = 2.6f,
                      Intensity = 0.8f
                  },
        };


        public static readonly MySunProperties ChinesePowerPlantSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(150 / 255.0f, 205 / 255.0f, 208 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.JUPITER_POSITION) * 2,
            SunSpecular = new Vector3(233 / 255.0f, 219 / 255.0f, 29 / 255.0f),
            BackSunDiffuse = new Vector3(178 / 255.0f, 96 / 255.0f, 40 / 255.0f),
            BackSunIntensity = 0.351f,
            AmbientColor = new Vector3(36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.82f,
        };


        public static readonly MyFogProperties ChinesePowerPlantFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(255 / 255.0f, 255 / 255.0f, 225 / 255.0f),
            FogNear = 1200,
            FogFar = 70000,
            FogMultiplier = 0f,
        };

        public static readonly MyParticleDustProperties ChinesePowerPlantDustProperties = new MyParticleDustProperties()
        {
            Enabled = false,
            DustBillboardRadius = 11.417f,
            DustFieldCountInDirectionHalf = 6.680f,
            DistanceBetween = 44.400f,
            Color = new Color(5 / 255.0f, 10 / 255.0f, 30 / 255.0f, 1f),
        };
        
        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateChinesePowerPlantArea()
        {
            Areas[MySolarSystemAreaEnum.ChineseArea] = new MySolarSystemAreaOrbit()
            {
                Name = "Chinese area",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.CHINESE_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 100,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 100,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(255 / 255.0f, 193 / 255.0f, 193 / 255.0f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 5, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = ChinesePowerPlantSunProperties,
                    FogProperties = ChinesePowerPlantFogProperties,
                    ImpostorProperties = ChinesePowerPlantImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = ChinesePowerPlantObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = ChinesePowerPlantDustProperties,
                   },
            };
        }
    }
}

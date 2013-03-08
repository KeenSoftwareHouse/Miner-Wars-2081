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
        static MySectorObjectCounts JupiterObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 0,
            Voxels128 = 0,
            Voxels64 = 0,
            StaticAsteroidLarge = 300,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] JupiterImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 3.5f,
                      MaxRadius = 3.5f,
                      AnimationSpeed = new Vector4(0.004f, 0.001f,0.003f, 0),
                      Color = new Vector3(95 / 255.0f, 63 / 255.0f, 42 / 255.0f),
                      Contrast = 6.170f,
                      Intensity = 0.431f
                  },
        };


        public static readonly MySunProperties JupiterSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 166 / 255.0f, 72 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.JUPITER_POSITION) * 2,
            SunSpecular = new Vector3(187 / 255.0f, 82 / 255.0f, 50 / 255.0f),
            BackSunDiffuse = new Vector3(16 / 255.0f, 144 / 255.0f, 95 / 255.0f),
            BackSunIntensity = 1.39f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.450f,
        };


        public static readonly MyFogProperties JupiterFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(105 / 255.0f, 82 / 255.0f, 38 / 255.0f),
            FogNear = 3000,
            FogFar = 20000,
            FogMultiplier = 0.65f,
        };

        public static readonly MyParticleDustProperties JupiterDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 102.96f,
            DustFieldCountInDirectionHalf = 4.477f,
            DistanceBetween = 112.900f,
            Color = new Color(200 / 255.0f, 110 / 255.0f, 0 / 40.0f, 1f),
        };





        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateJupiterArea()
        {
            Areas[MySolarSystemAreaEnum.Jupiter] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Jupiter",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.JUPITER_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = JupiterSunProperties,
                    FogProperties = JupiterFogProperties,
                    ImpostorProperties = JupiterImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = JupiterObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = JupiterDustProperties,
                },
            };
        }
    }
}

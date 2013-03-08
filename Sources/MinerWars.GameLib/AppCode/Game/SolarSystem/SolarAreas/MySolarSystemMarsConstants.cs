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
        static MySectorObjectCounts MarsObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 300,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 6000,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] MarsImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 2.3f,
                      MaxRadius = 2.3f,
                      AnimationSpeed = new Vector4(0.006f, 0.005f,0.007f, 0),
                      Color = new Vector3(29 / 255.0f, 13 / 255.0f, 0 / 255.0f),
                      Contrast = 2.490f,
                      Intensity = 0.644f
                  },
        };


        public static readonly MySunProperties MarsSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 71 / 255.0f, 17 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.MARS_POSITION) * 2,
            SunSpecular = new Vector3(233 / 255.0f, 203 / 255.0f, 103 / 255.0f),
            BackSunDiffuse = new Vector3(15 / 255.0f, 117 / 255.0f, 95 / 255.0f),
            BackSunIntensity = 0.95f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.620f,
        };


        public static readonly MyFogProperties MarsFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(87 / 255.0f, 15 / 255.0f, 3 / 255.0f),
            FogNear = 3000,
            FogFar = 38000,
            FogMultiplier = 0.6f,
        };

        public static readonly MyParticleDustProperties MarsDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 55.96f,
            DustFieldCountInDirectionHalf = 7.199f,
            DistanceBetween = 76.900f,
            Color = new Color(60 / 255.0f, 20 / 255.0f, 0 / 255.0f, 1f),
        };





        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateMarsArea()
        {
            Areas[MySolarSystemAreaEnum.Mars] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Mars",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.MARS_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.MARS_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.MARS_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = MarsSunProperties,
                    FogProperties = MarsFogProperties,
                    ImpostorProperties = MarsImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = MarsObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = MarsDustProperties,
                },
            };
        }
    }
}

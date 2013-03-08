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
        static MySectorObjectCounts NeptunObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 0,
            Voxels128 = 0,
            Voxels64 = 0,
            StaticAsteroidLarge = 250,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 3386,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] NeptunImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 3.1f,
                      MaxRadius = 3.5f,
                      AnimationSpeed = new Vector4(0.005f, 0.001f,0.003f, 0),
                      Color = new Vector3(0 / 255.0f, 28 / 255.0f, 84 / 255.0f),
                      Contrast = 2.70f,
                      Intensity = 0.646f
                  },
        };


        public static readonly MySunProperties NeptunSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(49 / 255.0f, 196 / 255.0f, 214 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.NEPTUNE_POSITION) * 8,
            SunSpecular = new Vector3(78 / 255.0f, 228 / 255.0f, 217 / 255.0f),
            BackSunDiffuse = new Vector3(17 / 255.0f, 204 / 255.0f, 197 / 255.0f),
            BackSunIntensity = 0.69f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.455f,
        };


        public static readonly MyFogProperties NeptunFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(0 / 255.0f, 120 / 255.0f, 155 / 255.0f),
            FogNear = 3000,
            FogFar = 13000,
            FogMultiplier = 0.39f,
        };

        public static readonly MyParticleDustProperties NeptunDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 82.96f,
            DustFieldCountInDirectionHalf = 4.277f,
            DistanceBetween = 128.900f,
            Color = new Color(0 / 255.0f, 120 / 255.0f, 140 / 255.0f, 1f),
        };





        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateNeptunArea()
        {
            Areas[MySolarSystemAreaEnum.Neptune] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Neptun",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.NEPTUNE_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.NEPTUNE_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.NEPTUNE_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = NeptunSunProperties,
                    FogProperties = NeptunFogProperties,
                    ImpostorProperties = NeptunImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = NeptunObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = NeptunDustProperties,
                },
            };
        }
    }
}

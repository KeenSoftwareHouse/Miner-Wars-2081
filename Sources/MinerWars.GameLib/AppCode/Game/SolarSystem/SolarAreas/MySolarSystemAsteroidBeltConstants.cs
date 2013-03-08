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

        static MySectorObjectCounts AsteroidBeltObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 20,
            Voxels128 = 20,
            Voxels64 = 20,
            StaticAsteroidLarge = 41,
            StaticAsteroidMedium = 4000,
            StaticAsteroidSmall = 5000,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] AsteroidBeltImpostorsProperties = new MyImpostorProperties[]
        {
                   new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_A,
                      ImpostorsCount = 10000,
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
                      MinRadius = 3.543f,
                      MaxRadius = 3.643f,
                      AnimationSpeed = new Vector4(0.004f, 0.001f,0.003f, 0),
                      Color = MyMath.VectorFromColor(165, 196, 225),
                      Contrast = 1.560f,
                      Intensity = 0.350f
                  },
        };

        public static readonly MySunProperties AsteroidBeltSunProperties = new MySunProperties()
        {
            SunDiffuse = MyMath.VectorFromColor(220, 220, 165),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.ASTEROID_BELT_POSITION)*3,
            SunSpecular = MyMath.VectorFromColor(152, 96, 23),
            BackSunDiffuse = MyMath.VectorFromColor(23, 43, 69),
            BackSunIntensity = 0.75f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.95f,
            EnvironmentAmbientIntensity = 0.95f,
        };

        public static readonly MyFogProperties AsteroidBeltFogProperties = new MyFogProperties()
        {
            FogColor = MyMath.VectorFromColor(23, 70, 90),
            FogNear = 3000,
            FogFar = 7000,
            FogMultiplier = 0.35f,
            FogBacklightMultiplier = 1.0f,
        };

        public static readonly MyParticleDustProperties AsteroidBeltDustProperties = new MyParticleDustProperties()
        {
            Enabled = false,
            DustBillboardRadius = 30f,
            DustFieldCountInDirectionHalf = 5f,
            DistanceBetween = 28f,
            Color = new Color(94, 149, 170, 64),
        };


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateAsteroidBeltArea()
        {
            Areas[MySolarSystemAreaEnum.AsteroidBelt] = new MySolarSystemAreaOrbit()
            {
                Name = "Asteroid belt",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.ASTEROID_BELT_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 1,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 10000,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 10000,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.3f, 0.3f, 0.3f),
                    DustColorVariability = new Vector4(0.07f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.7f, Count = 60, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SectorObjectsCounts = AsteroidBeltObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,
                    SunProperties = AsteroidBeltSunProperties,
                    FogProperties = AsteroidBeltFogProperties,

                    ImpostorProperties = AsteroidBeltImpostorsProperties,
                    ParticleDustProperties = AsteroidBeltDustProperties
                },

            };
        }
    }
}

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
        static MySectorObjectCounts Jupiter2ObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 30,
            StaticAsteroidMedium = 1000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] Jupiter2ImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 2f,
                      MaxRadius = 2f,
                      AnimationSpeed = new Vector4(0.002f, 0.005f,0.003f, 0),
                      Color = new Vector3(53 / 255.0f, 45 / 255.0f, 33 / 255.0f),
                      Contrast = 6.670f,
                      Intensity = 2.931f,
                      Enabled = false
                  },
        };


        public static readonly MySunProperties Jupiter2SunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 166 / 255.0f, 72 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.JUPITER2_POSITION) * 2,
            SunSpecular = new Vector3(187 / 255.0f, 82 / 255.0f, 50 / 255.0f),
            BackSunDiffuse = new Vector3(56 / 255.0f, 46 / 255.0f, 36 / 255.0f),
            BackSunIntensity = 1.39f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.450f,
        };


        public static readonly MyFogProperties Jupiter2FogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(56 / 255.0f, 23 / 255.0f, 6 / 255.0f),
            FogNear = 3000,
            FogFar = 20000,
            FogMultiplier = 0.73f,
            FogBacklightMultiplier = 2.9f,
        };

        public static readonly MyParticleDustProperties Jupiter2DustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 117.96f,
            DustFieldCountInDirectionHalf = 6.477f,
            DistanceBetween = 91.900f,
            Color = new Color(7 / 255.0f, 11 / 255.0f, 15 / 255.0f, 1f),
        };





        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateJupiter2Area()
        {
            Areas[MySolarSystemAreaEnum.Jupiter2] = new MySolarSystemAreaOrbit()
            {
                Name = "Jupiter area 2",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.JUPITER2_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER2_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER2_RADIUS * MyBgrCubeConsts.MILLION_KM * 800,
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
                    SunProperties = Jupiter2SunProperties,
                    FogProperties = Jupiter2FogProperties,
                    ImpostorProperties = Jupiter2ImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = Jupiter2ObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = Jupiter2DustProperties,
                },
            };
        }
    }
}

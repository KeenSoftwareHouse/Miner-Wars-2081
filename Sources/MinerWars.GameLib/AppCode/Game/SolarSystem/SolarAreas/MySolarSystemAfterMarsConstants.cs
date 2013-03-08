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
        static MySectorObjectCounts AfterMarsObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 5,
            Voxels64 = 10,
            StaticAsteroidLarge = 10,
            StaticAsteroidMedium = 300,
            StaticAsteroidSmall = 1000,
            Motherships = 0,
            StaticDebrisFields = 0
        };
        
        public readonly static MyImpostorProperties[] AfterMarsImpostorsProperties = new MyImpostorProperties[]
        {
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
                      Color = new Vector3(146 / 255.0f, 239 / 255.0f, 255 / 255.0f),
                      Contrast = 2.490f,
                      Intensity = 0.36f
                  },
        };


        public static readonly MySunProperties AfterMarsSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(156 / 255.0f, 232 / 255.0f, 208 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.MARS_POSITION) * 2,
            SunSpecular = new Vector3(141 / 255.0f, 176 / 255.0f, 154 / 255.0f),
            BackSunDiffuse = new Vector3(95 / 255.0f, 132 / 255.0f, 149 / 255.0f),
            BackSunIntensity = 0.59f,
            AmbientColor = new Vector3(36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.820f,
        };


        public static readonly MyFogProperties AfterMarsFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(102 / 255.0f, 168 / 255.0f, 255 / 255.0f),
            FogNear = 36000,
            FogFar = 60000,
            FogMultiplier = 0.114f,
            FogBacklightMultiplier = 0.188f,
        };

        public static readonly MyParticleDustProperties AfterMarsDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 55.96f,
            DustFieldCountInDirectionHalf = 7.199f,
            DistanceBetween = 76.900f,
            Color = new Color(60 / 255.0f, 20 / 255.0f, 0 / 255.0f, 1f),
            Enabled = false,
        };
        



        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateMars2Area()
        {
            Areas[MySolarSystemAreaEnum.Mars2] = new MySolarSystemAreaOrbit()
            {
                Name = "Mars area",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.MARS2_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.2f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.MARS_RADIUS * MyBgrCubeConsts.MILLION_KM * 200,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.MARS_RADIUS * MyBgrCubeConsts.MILLION_KM * 400,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(122 / 255.0f, 139 / 255.0f, 139 / 255.0f),
                    DustColorVariability = new Vector4(0.05f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 0.2f, Count = 3, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.3f, Count = 8, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = AfterMarsSunProperties,
                    FogProperties = AfterMarsFogProperties,
                    ImpostorProperties = AfterMarsImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = AfterMarsObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = AfterMarsDustProperties,
                },
            };
        }
    }
                         
}

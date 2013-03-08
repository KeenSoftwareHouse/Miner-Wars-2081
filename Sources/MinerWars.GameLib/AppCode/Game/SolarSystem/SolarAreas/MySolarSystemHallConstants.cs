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
        static MySectorObjectCounts HallObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 50,
            StaticAsteroidMedium = 2500,
            StaticAsteroidSmall = 4000,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] HallImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.93f,
                      MaxRadius = 0.93f,
                      AnimationSpeed = new Vector4(0.003f, 0.001f,0.003f, 0),
                      Color = new Vector3(222 / 255.0f, 195 / 255.0f, 225 / 255.0f),
                      Contrast = 2.644f,
                      Intensity = 0.65f
                  },
        };


        public static readonly MySunProperties HallSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(150 / 255.0f, 205 / 255.0f, 208 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.EARTH_POSITION),
            SunSpecular = new Vector3(219 / 255.0f, 220 / 255.0f, 165 / 255.0f),
            BackSunDiffuse = new Vector3(155 / 255.0f, 96 / 255.0f, 40 / 255.0f),
            BackSunIntensity = 1.159f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.942f,
            EnvironmentAmbientIntensity = 0.942f,
            BackgroundColor = Vector3.One
        };


        public static readonly MyFogProperties HallFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(23 / 255.0f, 70 / 255.0f, 89 / 255.0f),
            FogNear = 5600,
            FogFar = 12400,
            FogMultiplier = 0.189f,
            FogBacklightMultiplier = 0.825f,
        };

        public static readonly MyParticleDustProperties HallDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 40.0f,
            DustFieldCountInDirectionHalf = 6.0f,
            DistanceBetween = 42.3f,
            Color = new Color(146 / 255.0f, 184 / 255.0f, 198 / 255.0f, 1f),
            Texture = MyTransparentMaterialEnum.Dust,
        };


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateHallArea()
        {
            Areas[MySolarSystemAreaEnum.Hall] = new MySolarSystemAreaOrbit()
            {
                Name = "Hall area",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = new Vector3(11, 0, -511) * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 400,
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
                    SunProperties = HallSunProperties,
                    FogProperties = HallFogProperties,
                    ImpostorProperties = HallImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = HallObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = HallDustProperties,
                },
            };
        }
    }
}

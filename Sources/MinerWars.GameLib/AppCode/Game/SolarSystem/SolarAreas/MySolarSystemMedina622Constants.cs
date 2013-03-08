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
        static MySectorObjectCounts Medina622ObjectsProperties = new MySectorObjectCounts()
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

        public readonly static MyImpostorProperties[] Medina622ImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.7f,
                      MaxRadius = 0.7f,
                      AnimationSpeed = new Vector4(0.004f, 0.002f, 0.003f, 0),
                      Color = new Vector3(235 / 255.0f, 199 / 255.0f, 157 / 255.0f),
                      Contrast = 11.2f,
                      Intensity = 1.7f
                  },
        };


        public static readonly MySunProperties Medina622SunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 167 / 255.0f, 0 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.JUPITER_POSITION) * 2,
            SunSpecular = new Vector3(255 / 255.0f, 167 / 255.0f, 0 / 255.0f),
            BackSunDiffuse = new Vector3(205 / 255.0f, 127 / 255.0f, 50 / 255.0f),
            BackSunIntensity = 0.6f,
            AmbientColor = new Vector3(36 / 255.0f),
            AmbientMultiplier = 0f,
            EnvironmentAmbientIntensity = 1f,
        };


        public static readonly MyFogProperties Medina622FogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(255 / 255.0f, 179 / 255.0f, 16 / 255.0f),
            FogNear = 1200,
            FogFar = 4600,
            FogMultiplier = 0.25f,
        };

        public static readonly MyParticleDustProperties Medina622DustProperties = new MyParticleDustProperties()
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
        public static void CreateMedina622Area()
        {
            Areas[MySolarSystemAreaEnum.Medina622] = new MySolarSystemAreaOrbit()
            {
                Name = "Arab border",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.MEDINA622_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.05f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 100,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
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
                    SunProperties = Medina622SunProperties,
                    FogProperties = Medina622FogProperties,
                    ImpostorProperties = Medina622ImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = Medina622ObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = Medina622DustProperties,
                   },
            };
        }
    }    
}

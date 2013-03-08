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
        static MySectorObjectCounts VenusObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 190,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] VenusImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.76f,
                      MaxRadius = 0.76f,
                      AnimationSpeed = new Vector4(0.003f, 0.002f,0.000f, 0),
                      Color = new Vector3(255 / 255.0f, 141 / 255.0f, 0 / 255.0f),
                      Contrast = 0.954f,
                      Intensity = 0.560f
                  },
        };


        public static readonly MySunProperties VenusSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(252 / 255.0f, 178 / 255.0f, 51 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.VENUS_POSITION),
            SunSpecular = new Vector3(198 / 255.0f, 114 / 255.0f, 5 / 255.0f),
            BackSunDiffuse = new Vector3(92 / 255.0f, 51 / 255.0f, 24 / 255.0f),
            BackSunIntensity = 0.69f,
            AmbientColor = new Vector3(87 / 255.0f, 51 / 255.0f, 24 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 1.029f,
        };


        public static readonly MyFogProperties VenusFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(210 / 255.0f, 51 / 255.0f, 22 / 255.0f),
            FogNear = 9347.831f,
            FogFar = 13000,
            FogMultiplier = 0.402f,
            FogBacklightMultiplier = 0.860f,
        };

        public static readonly MyParticleDustProperties VenusDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 72.76f,
            DustFieldCountInDirectionHalf = 5.2f,
            DistanceBetween = 104.3f,
            Color = new Color(107 / 255.0f, 54 / 255.0f, 4 / 255.0f, 1f),
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> VenusPrimaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_03, 1.0f },
            { MyMwcVoxelMaterialsEnum.Stone_04, 1.0f },
        };


        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> VenusSecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Magnesium_01, 1.0f },
            { MyMwcVoxelMaterialsEnum.Indestructible_01, 0.0f }
        };

        
        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateVenusArea()
        {
            Areas[MySolarSystemAreaEnum.Venus] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Venus",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.VENUS_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.VENUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.VENUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = VenusSunProperties,
                    FogProperties = VenusFogProperties,
                    ImpostorProperties = VenusImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = VenusObjectsProperties,
                    PrimaryAsteroidMaterials = VenusPrimaryAsteroidMaterials,
                    SecondaryAsteroidMaterials = VenusSecondaryAsteroidMaterials,

                    ParticleDustProperties = VenusDustProperties,
                },
            };
        }
    }
}

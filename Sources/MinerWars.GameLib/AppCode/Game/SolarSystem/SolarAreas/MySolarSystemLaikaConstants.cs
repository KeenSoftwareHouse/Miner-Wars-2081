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
        static MySectorObjectCounts LaikaObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 3,
            StaticAsteroidMedium = 1700,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] LaikaImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.472f,
                      MaxRadius = 0.472f,
                      AnimationSpeed = new Vector4(0.000f, 0.001f,0.001f, 0),
                      Color = new Vector3(241 / 255.0f, 66 / 255.0f, 0 / 255.0f),
                      Contrast = 6.170f,
                      Intensity = 0.460f
                  },
        };


        public static readonly MySunProperties LaikaSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 112 / 255.0f, 29 / 255.0f),
            SunIntensity = 2.0f, //MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.LAIKA_POSITION),
            SunSpecular = new Vector3(255 / 255.0f, 206 / 255.0f, 138 / 255.0f),
            BackSunDiffuse = new Vector3(29 / 255.0f, 29 / 255.0f, 29 / 255.0f),
            BackSunIntensity = 0.923f,
            AmbientColor = new Vector3(41 / 255.0f, 29 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.992f,
            EnvironmentAmbientIntensity = 0.85f,
            BackgroundColor = new Vector3(142 / 255.0f, 35 / 255.0f, 35 / 255.0f),
        };


        public static readonly MyFogProperties LaikaFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(255 / 255.0f, 0 / 255.0f, 0 / 255.0f),
            FogNear = 2631,
            FogFar = 6606,
            FogMultiplier = 0.120f,
            FogBacklightMultiplier = 1.179f, 
        };

        public static readonly MyParticleDustProperties LaikaDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 111.142f,
            DustFieldCountInDirectionHalf = 3.43f,
            DistanceBetween = 72.4f,
            Color = new Color(20 / 255.0f, 0 / 255.0f, 0 / 255.0f, 30 / 255f),
        };


        public static readonly MyDebrisProperties LaikaDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 2,
            FullScaleDistance = 70,
            DistanceBetween = 80.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> LaikaAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateLaikaArea()
        {
            Areas[MySolarSystemAreaEnum.Laika] = new MySolarSystemAreaOrbit()
            {
                Name = "Mars belt end",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.LAIKA_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 500,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 800,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.9f, 0.60f, 0.37f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 5, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    BackgroundTexture = "BackgroundCube",
                    SunProperties = LaikaSunProperties,
                    FogProperties = LaikaFogProperties,
                    ImpostorProperties = LaikaImpostorsProperties,
                    DebrisProperties = LaikaDebrisProperties,

                    SectorObjectsCounts = LaikaObjectsProperties,
                    PrimaryAsteroidMaterials = LaikaAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = LaikaDustProperties,
                },
            };
        }
    }
}

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
        static MySectorObjectCounts MercuryObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 5,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 71,
            StaticAsteroidMedium = 4000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] MercuryImpostorsProperties = new MyImpostorProperties[]
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
                        /*
                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Nebula,
                      Material = null,
                      ImpostorsCount = 0,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.0f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.3f,
                      MinRadius = 9.143f,
                      MaxRadius = 9.143f,
                      AnimationSpeed = new Vector4(0.017f, 0.001f,0.003f, 0),
                      Color = new Vector3(0.8f, 0.1f, 0.0f),
                      Contrast = 0.644f,
                      Intensity = 0.644f
                  },  */
        };


        public static readonly MySunProperties MercurySunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(1.0f, 0.5f, 0.1f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.MERCURY_POSITION),
            SunSpecular = new Vector3(0.9137255f, 0.5078432f, 0.1078431f),
            //BackSunDiffuse = new Vector3(234 / 255.0f, 130 / 255.0f, 28 / 255.0f),
            BackSunDiffuse = new Vector3(229/ 255.0f, 131/ 255.0f, 27/ 255.0f),
            BackSunIntensity = 0.837f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.45f,
        };

        
        public static readonly MyFogProperties MercuryFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(99 / 255.0f, 36 / 255.0f, 3 / 255.0f),
            FogNear = 3000,
            FogFar = 13633,
            FogMultiplier = 0.383f,
        };

        public static readonly MyParticleDustProperties MercuryDustProperties = new MyParticleDustProperties()
        {
            Enabled = false,
            DustBillboardRadius = 105.76f,
            DustFieldCountInDirectionHalf = 7.2f,
            DistanceBetween = 128.4f,
            Color = new Color(99 / 255.0f, 36 / 255.0f, 3 / 255.0f, 1 / 255f),
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> MercuryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},            
            { MyMwcVoxelMaterialsEnum.Stone_04, 1},            
            { MyMwcVoxelMaterialsEnum.Stone_10, 1},            
            { MyMwcVoxelMaterialsEnum.Silver_01, 1},            
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> MercurySecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
          { MyMwcVoxelMaterialsEnum.Helium4_01, 0.5f},
          { MyMwcVoxelMaterialsEnum.Magnesium_01, 0.5f},
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateMercuryArea()
        {
            Areas[MySolarSystemAreaEnum.Mercury] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Mercury",
                AreaType = MySolarSystemArea.AreaEnum.Sun,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.MERCURY_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.9f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.7f, 0.50f, 0.37f),
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
                    SunProperties = MercurySunProperties,
                    FogProperties = MercuryFogProperties,
                    ImpostorProperties = MercuryImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = MercuryObjectsProperties,
                    PrimaryAsteroidMaterials = MercuryAsteroidMaterials,
                    SecondaryAsteroidMaterials = MercurySecondaryAsteroidMaterials,

                    ParticleDustProperties = MercuryDustProperties,                   
                },
            };
        }
    }
}

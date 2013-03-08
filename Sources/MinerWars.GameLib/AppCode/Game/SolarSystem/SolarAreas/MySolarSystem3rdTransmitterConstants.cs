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
        static MySectorObjectCounts ThirdTransmitterObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 71,
            StaticAsteroidMedium = 4000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] ThirdTransmitterImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.9f,
                      MaxRadius = 0.9f,
                      AnimationSpeed = new Vector4(-0.003f, 0.02f, 0.014f, 0),
                      Color = new Vector3(146 / 255.0f, 0 / 255.0f, 211 / 255.0f),
                      Contrast = 1.052f,
                      Intensity = 0.337f
                  },  
        };


        public static readonly MySunProperties ThirdTransmitterSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(215 / 255.0f, 191 / 255.0f, 216 / 255.0f),
            SunIntensity = 2.250f,
            SunSpecular = new Vector3(144 / 255.0f, 101 / 255.0f, 104 / 255.0f),
            BackSunDiffuse = new Vector3(148 / 255.0f, 192 / 255.0f, 211 / 255.0f),
            BackSunIntensity = 0.04f,
            AmbientColor = new Vector3(37 / 255.0f, 37 / 255.0f, 35 / 255.0f),
            AmbientMultiplier = 0.765f,
            EnvironmentAmbientIntensity = 0.261f,
        };


        public static readonly MyFogProperties ThirdTransmitterFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(137 / 255.0f, 44 / 255.0f, 225 / 255.0f),
            FogNear = 4073,
            FogFar = 4726,
            FogMultiplier = 0.133f,
            FogBacklightMultiplier = 1.037f,  
        };

        public static readonly MyParticleDustProperties ThirdTransmitterDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 123.574f,
            DustFieldCountInDirectionHalf = 6.021f,
            DistanceBetween = 123.123f,
            Color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f),
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ThirdTransmitterAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},            
            { MyMwcVoxelMaterialsEnum.Stone_04, 1},            
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ThirdTransmitterSecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
          { MyMwcVoxelMaterialsEnum.Helium4_01, 0.5f},
          { MyMwcVoxelMaterialsEnum.Magnesium_01, 0.5f},
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void Create3rdTransmitterArea()
        {
            Areas[MySolarSystemAreaEnum.ThirdTransmitter] = new MySolarSystemAreaOrbit()
            {
                Name = "Around 3rd transmitter",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MySolarMapRenderer.SectorToKm(new MyMwcVector3Int(3818505, 0, -4273800)),
                    LongSpread = 0.05f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 100,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
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
                    SunProperties = ThirdTransmitterSunProperties,
                    FogProperties = ThirdTransmitterFogProperties,
                    ImpostorProperties = ThirdTransmitterImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = ThirdTransmitterObjectsProperties,
                    PrimaryAsteroidMaterials = ThirdTransmitterAsteroidMaterials,
                    SecondaryAsteroidMaterials = ThirdTransmitterSecondaryAsteroidMaterials,

                    ParticleDustProperties = ThirdTransmitterDustProperties,                   
                },
            };
        }
    }
}

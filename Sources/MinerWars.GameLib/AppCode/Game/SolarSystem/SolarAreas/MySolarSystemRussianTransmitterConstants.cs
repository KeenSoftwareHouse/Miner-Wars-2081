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
        static MySectorObjectCounts RussianTransmitterObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 20,
            StaticAsteroidMedium =6500,
            StaticAsteroidSmall = 5500,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] RussianTransmitterImpostorsProperties = new MyImpostorProperties[]
        {
                   new MyImpostorProperties()
                  {
                      Enabled = false,
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
                      Enabled = false,
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
                      MinRadius = 0f,
                      MaxRadius = 0f,
                      AnimationSpeed = new Vector4(0.014f, 0.010f,0.014f, 0),
                      Color = new Vector3(255 / 255.0f, 0 / 255.0f, 219 / 255.0f),
                      Contrast = 0f,
                      Intensity = 0f
                  },
        };


        public static readonly MySunProperties RussianTransmitterSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 130 / 255.0f, 0 / 255.0f),
            SunIntensity = 2.322f,
            SunSpecular = new Vector3(0 / 255.0f, 0 / 255.0f, 0 / 255.0f),
            BackSunDiffuse = new Vector3(170 / 255.0f, 175 / 255.0f, 75 / 255.0f),
            BackSunIntensity = 0.909f,
            AmbientColor = new Vector3(36 / 255.0f, 36 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.620f,
        };


        public static readonly MyFogProperties RussianTransmitterFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(217 / 255.0f, 184 / 255.0f, 251 / 255.0f),
            FogNear = 1,
            FogFar = 11255.76f,
            FogMultiplier = 0f,
        };

        public static readonly MyParticleDustProperties RussianTransmitterDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 130f,
            DustFieldCountInDirectionHalf = 6,
            DistanceBetween = 218f,
            Color = new Color(94 / 255.0f, 113 / 255.0f, 70 / 207.0f, 1f),
        };


        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> RussianTransmitterAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_10, 1},
            { MyMwcVoxelMaterialsEnum.Stone_04, 1},
            { MyMwcVoxelMaterialsEnum.Stone_06, 1},
            { MyMwcVoxelMaterialsEnum.Stone_07, 1},
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> RussianTransmitterSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, 1},
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},
            { MyMwcVoxelMaterialsEnum.Uranite_01, 1},
            { MyMwcVoxelMaterialsEnum.Gold_01, 1},
        };

      

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateRussianTransmitterArea()
        {
            Areas[MySolarSystemAreaEnum.RussianTransmitter] = new MySolarSystemAreaOrbit()
            {
                Name = "Russian area",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.RUSSIAN_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.RUSSIANTRANSMITTER_RADIUS * MyBgrCubeConsts.MILLION_KM * 200,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.RUSSIANTRANSMITTER_RADIUS * MyBgrCubeConsts.MILLION_KM * 400,
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
                    SunProperties = RussianTransmitterSunProperties,
                    FogProperties = RussianTransmitterFogProperties,
                    ImpostorProperties = RussianTransmitterImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = RussianTransmitterObjectsProperties,
                    PrimaryAsteroidMaterials = RussianTransmitterAsteroidMaterials,
                    SecondaryAsteroidMaterials = RussianTransmitterSecondaryMaterials,

                    ParticleDustProperties = RussianTransmitterDustProperties,
                },
            };
        }
    }
}

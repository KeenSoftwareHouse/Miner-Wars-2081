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
        static MySectorObjectCounts NebulaObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 60,
            StaticAsteroidMedium = 2000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] NebulaImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.395f,
                      MaxRadius = 0.395f,
                      AnimationSpeed = new Vector4(0.001f, -0.004f, -0.006f,0),
                      Color = new Vector3(25 / 255.0f, 187 / 255.0f, 255 / 255.0f),
                      Contrast = 2.469f,
                      Intensity = 1.037f
                  },
        };


        public static readonly MySunProperties NebulaSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 255 / 255.0f, 255 / 255.0f),
            SunIntensity = 1.458f,
            SunSpecular = new Vector3(46 / 255.0f, 46 / 255.0f, 46 / 255.0f),
            BackSunDiffuse = new Vector3(25 / 255.0f, 187 / 255.0f, 255 / 255.0f),
            BackSunIntensity = 0.673f,
            AmbientColor = new Vector3 (0 / 255.0f, 0 / 255.0f, 0 / 255.0f),
            AmbientMultiplier = 0f,
            EnvironmentAmbientIntensity = 0.200f,
        };


        public static readonly MyFogProperties NebulaFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(58 / 255.0f, 134 / 255.0f, 255 / 255.0f),
            FogNear = 6032.194f,
            FogFar = 9296.925f,
            FogMultiplier = 0.186f,
            FogBacklightMultiplier = 0.380f,
        };

        public static readonly MyParticleDustProperties NebulaDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 200.3f,
            DustFieldCountInDirectionHalf = 6.953f,
            DistanceBetween = 206.906f,
            Color = new Color(25 / 255.0f, 111 / 255.0f, 192 / 255.0f, 1f),
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> NebulaAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_10, 1},
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> NebulaSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Helium3_01, 1},
        };




        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateNebulaArea()
        {
            Areas[MySolarSystemAreaEnum.Nebula] = new MySolarSystemAreaOrbit()
            {
                Name = "Nebula",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.NEBULA_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.05f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.NEBULA_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.NEBULA_RADIUS * MyBgrCubeConsts.MILLION_KM * 700,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(28 / 255.0f, 113 / 255.0f, 179 / 255.0f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 5, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = NebulaSunProperties,
                    FogProperties = NebulaFogProperties,
                    ImpostorProperties = NebulaImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = NebulaObjectsProperties,
                    PrimaryAsteroidMaterials = NebulaAsteroidMaterials,
                    SecondaryAsteroidMaterials = NebulaSecondaryMaterials,

                    ParticleDustProperties = NebulaDustProperties,
                },
            };
        }
    }
}

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
        static MySectorObjectCounts EarthObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 3,
            StaticAsteroidMedium = 1000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] EarthImpostorsProperties = new MyImpostorProperties[]
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
                      Color = new Vector3(15 / 255.0f, 48 / 255.0f, 83 / 255.0f),
                      Contrast = 2.644f,
                      Intensity = 0.344f
                  },
        };


        public static readonly MySunProperties EarthSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(150 / 255.0f, 205 / 255.0f, 208 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.EARTH_POSITION),
            SunSpecular = new Vector3(233 / 255.0f, 130 / 255.0f, 29 / 255.0f),
            BackSunDiffuse = new Vector3(155 / 255.0f, 96 / 255.0f, 40 / 255.0f),
            BackSunIntensity = 0.2f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.820f,
            BackgroundColor = Vector3.One
        };


        public static readonly MyFogProperties EarthFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(12 / 255.0f, 38 / 255.0f, 29 / 255.0f),
            FogNear = 3000,
            FogFar = 6000,
            FogMultiplier = 0.58f,
        };

        public static readonly MyParticleDustProperties EarthDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 30.0f,
            DustFieldCountInDirectionHalf = 4.0f,
            DistanceBetween = 30.3f,
            Color = new Color(5 / 255.0f, 10 / 255.0f, 30 / 255.0f, 1f),
            Texture = MyTransparentMaterialEnum.Dust,
        };



        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> EarthAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_01, 1},            
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},            
            { MyMwcVoxelMaterialsEnum.Stone_04, 1},    
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> EarthSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Magnesium_01, 1},            
        };




        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateEarthArea()
        {
            Areas[MySolarSystemAreaEnum.Earth] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Earth",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.EARTH_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.EARTH_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = EarthSunProperties,
                    FogProperties = EarthFogProperties,
                    ImpostorProperties = EarthImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = EarthObjectsProperties,
                    PrimaryAsteroidMaterials = EarthAsteroidMaterials,
                    SecondaryAsteroidMaterials = EarthSecondaryMaterials,

                    ParticleDustProperties = EarthDustProperties,
                },
            };
        }
    }
}

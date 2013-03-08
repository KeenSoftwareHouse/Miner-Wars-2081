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
        static MySectorObjectCounts SaturnObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 150,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] SaturnImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.376f,
                      MaxRadius = 0.376f,
                      AnimationSpeed = new Vector4(0.005f, 0.001f,0.003f, 0),
                      Color = new Vector3(190 / 255.0f, 222 / 255.0f, 255 / 255.0f),
                      Contrast = 20.000f,
                      Intensity = 1.327f
                  },
        };


        public static readonly MySunProperties SaturnSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(112 / 255.0f, 154 / 255.0f, 203 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.SATURN_POSITION) * 5,
            SunSpecular = new Vector3(255 / 255.0f, 255 / 255.0f, 255 / 255.0f),
            BackSunDiffuse = new Vector3(0 / 255.0f, 127 / 255.0f, 254 / 255.0f),
            BackSunIntensity = 1.576f,
            AmbientColor = new Vector3(0 / 255.0f, 0 / 255.0f, 156 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.221f,
        };


        public static readonly MyFogProperties SaturnFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(0 / 255.0f, 127 / 255.0f, 255 / 255.0f),
            FogNear = 3000,
            FogFar = 12928,
            FogMultiplier = 0.277f,
            FogBacklightMultiplier = 0.000f,
        };

        public static readonly MyParticleDustProperties SaturnDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 36.542f,
            DustFieldCountInDirectionHalf = 6.123f,
            DistanceBetween = 30.703f,
            Color = new Color(82 / 255.0f, 176 / 255.0f, 255 / 255.0f, 1f),
        };


        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> SaturnAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Stone_03, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Snow_01, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Ice_01, 1.0f }
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> SaturnSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Silver_01, 1.0f },
            {  MyMwcVoxelMaterialsEnum.Silicon_01, 1.0f }
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateSaturnArea()
        {
            Areas[MySolarSystemAreaEnum.Saturn] = new MySolarSystemAreaOrbit()
            {
                Name = "Post Saturn",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.SATURN_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.3f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.SATURN_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.SATURN_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
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
                    SunProperties = SaturnSunProperties,
                    FogProperties = SaturnFogProperties,
                    ImpostorProperties = SaturnImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = SaturnObjectsProperties,
                    PrimaryAsteroidMaterials = SaturnAsteroidMaterials,
                    SecondaryAsteroidMaterials = SaturnSecondaryMaterials,

                    ParticleDustProperties = SaturnDustProperties,
                },
            };
        }
    }
}

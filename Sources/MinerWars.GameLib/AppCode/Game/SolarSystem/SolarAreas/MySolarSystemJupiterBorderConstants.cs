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
        static MySectorObjectCounts JupiterBorderObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] JupiterBorderImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 1.515f,
                      MaxRadius = 1.515f,
                      AnimationSpeed = new Vector4(0.001f, 0.002f, 0.003f, 0),
                      Color = new Vector3(255 / 255.0f, 251 / 255.0f, 255 / 255.0f),
                      Contrast = 1.037f,
                      Intensity = 0.437f
                  },
        };


        public static readonly MySunProperties JupiterBorderSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(155 / 255.0f, 205 / 255.0f, 255 / 255.0f),
            SunIntensity = 2.337f,
            SunSpecular = new Vector3(21 / 255.0f, 11 / 255.0f, 3 / 255.0f),
            BackSunDiffuse = new Vector3(18 / 255.0f, 165 / 255.0f, 255 / 255.0f),
            BackSunIntensity = 0.945f,
            AmbientColor = new Vector3(3 / 255.0f, 3 / 255.0f, 3 / 255.0f),
            AmbientMultiplier = 1.446f,
            EnvironmentAmbientIntensity = 0.820f,
        };


        public static readonly MyFogProperties JupiterBorderFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(13 / 255.0f, 132 / 255.0f, 255 / 255.0f),
            FogNear = 1928,
            FogFar = 9004,
            FogMultiplier = 0.177f,
            FogBacklightMultiplier = 0.942f,
        };

        public static readonly MyParticleDustProperties JupiterBorderDustProperties = new MyParticleDustProperties()
        {
            Enabled = true,
            DustBillboardRadius = 95.592f,
            DustFieldCountInDirectionHalf = 1.033f,
            DistanceBetween = 109.159f,
            Color = new Color(51 / 255.0f, 134 / 255.0f, 235 / 255.0f, 1f),
        };
        
        public static readonly MyGodRaysProperties JupiterBorderGodRaysProperties = new MyGodRaysProperties()
        {
            Density = 0.21f,
            Weight = 1.161f,
            Decay = 0.937f,
            Exposition = 0.077f,
            Enabled = true,
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> JupiterBorderAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_10, 1 },
            { MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, 1 },
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> JupiterBorderSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Silicon_01, 1 },
            { MyMwcVoxelMaterialsEnum.Gold_01, 1 },
        };


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateJupiterBorderArea()
        {
            Areas[MySolarSystemAreaEnum.PostJupiterBorder] = new MySolarSystemAreaOrbit()
            {
                Name = "Jupiter border",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.JUPITERBORDER_POSITION * MyBgrCubeConsts.MILLION_KM * 1.13f,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 30,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.JUPITER_RADIUS * MyBgrCubeConsts.MILLION_KM * 180,
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
                    SunProperties = JupiterBorderSunProperties,
                    FogProperties = JupiterBorderFogProperties,
                    ImpostorProperties = JupiterBorderImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = JupiterBorderObjectsProperties,
                    PrimaryAsteroidMaterials = JupiterBorderAsteroidMaterials,
                    SecondaryAsteroidMaterials = JupiterBorderSecondaryMaterials,

                   ParticleDustProperties = JupiterBorderDustProperties,
                   GodRaysProperties = JupiterBorderGodRaysProperties,
                },
            };
        }
    }
      
}

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
        static MySectorObjectCounts RimeObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 0,
            Voxels128 = 0,
            Voxels64 = 0,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 1000.090f,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] RimeImpostorsProperties = new MyImpostorProperties[]
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

                  /*new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Nebula,
                      Material = null,
                      ImpostorsCount = 0,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.0f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.3f,
                      MinRadius = 0.302f,
                      MaxRadius = 0.302f,
                      AnimationSpeed = new Vector4(0.014f, -0.019f, -0.010f, 0),
                      Color = MyMath.VectorFromColor(255, 255, 255),
                      Contrast = 2.664f,
                      Intensity = 0.05f
                  },*/
        };


        public static readonly MySunProperties RimeSunProperties = new MySunProperties()
        {
            SunDiffuse = MyMath.VectorFromColor(236, 222, 125),
            SunIntensity = 1.674f,
            SunSpecular = MyMath.VectorFromColor(255, 255, 255),
            BackSunDiffuse = MyMath.VectorFromColor(236, 222, 125),
            BackSunIntensity = 0.189f,
            AmbientColor = MyMath.VectorFromColor(146, 187, 142),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.820f,
        };

        
        public static readonly MyFogProperties RimeFogProperties = new MyFogProperties()
        {
            FogColor = MyMath.VectorFromColor(101, 0, 28),
            FogNear = 1000,
            FogFar = 13867,
            FogMultiplier = 0.068f,
            FogBacklightMultiplier = 0.939f,
        };

        public static readonly MyParticleDustProperties RimeDustProperties = new MyParticleDustProperties()
        {
            Enabled = false,
            DustBillboardRadius = 0.010f,
            DustFieldCountInDirectionHalf = 2.105f,
            DistanceBetween = 244.141f,
            Color = new Color(255, 255, 255),
        };

        public static readonly MyGodRaysProperties RimeGodRaysProperties = new MyGodRaysProperties()
        {
            Density = 0.116f,
            Weight = 1.665f,
            Decay = 0.937f,
            Exposition = 0.077f,
            Enabled = true,
        };

        public static readonly MyDebrisProperties RimeDebrisProperties = new MyDebrisProperties()
        {
            Enabled = false,
            MaxDistance = 0,
            CountInDirectionHalf = 0,
            FullScaleDistance = 70,
            DistanceBetween = 30.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };



        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateRimeArea()
        {
            Areas[MySolarSystemAreaEnum.Rime] = new MySolarSystemAreaOrbit()
            {
                Name = "Rime",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MySolarSystemUtils.SectorsToKm(MyBgrCubeConsts.RIME_SECTOR),
                    LongSpread = 0.05f,
                    MaxDistanceFromOrbitLow = 2.1f * MyBgrCubeConsts.MILLION_KM,
                    MaxDistanceFromOrbitHigh = 8.4f * MyBgrCubeConsts.MILLION_KM,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.8f, 0.11f, 0.08f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 0, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = RimeSunProperties,
                    FogProperties = RimeFogProperties,
                    ImpostorProperties = RimeImpostorsProperties,
                    GodRaysProperties =RimeGodRaysProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = RimeObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultAsteroidMaterials,

                    ParticleDustProperties = RimeDustProperties,
                },
            };
        }
    }
}

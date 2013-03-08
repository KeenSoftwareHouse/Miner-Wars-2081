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
        static MySectorObjectCounts SlaverBaseObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 55,
            StaticAsteroidMedium = 1110,
            StaticAsteroidSmall = 900,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] SlaverBaseImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.475f,
                      MaxRadius = 0.475f,
                      AnimationSpeed = new Vector4(0.001f, 0.001f,0.001f, 0),
                      Color = new Vector3(241 / 255.0f, 66 / 255.0f, 0 / 255.0f),
                      Contrast = 0.604f,
                      Intensity = 0.630f
                  },*/
        };


        public static readonly MySunProperties SlaverBaseSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(235 / 255.0f, 129 / 255.0f, 63 / 255.0f),
            SunIntensity = 2.058f,
            SunSpecular = new Vector3(251 / 255.0f, 123 / 255.0f, 28 / 255.0f),
            BackSunDiffuse = new Vector3(212 / 255.0f, 108 / 255.0f, 8 / 255.0f),
            BackSunIntensity = 0.169f,
            AmbientColor = new Vector3(40 / 255.0f, 29 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.921f,
            EnvironmentAmbientIntensity = 0.46f,
        };

        /*
        public static readonly MyFogProperties SlaverBaseFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(172 / 255.0f, 13 / 255.0f, 10 / 255.0f),
            FogNear = 569.399f,
            FogFar = 10259,
            FogMultiplier = 0.235f,
            FogBacklightMultiplier = 0.968f,
        };*/
        /*
        public static readonly MyParticleDustProperties SlaverBaseDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 123.574f,
            DustFieldCountInDirectionHalf = 6.394f,
            DistanceBetween = 155.705f,
            Color = new Color(166 / 255.0f, 37 / 255.0f, 0 / 207.0f, 1f),
        };*/





        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateSlaverBaseArea()
        {
            Areas[MySolarSystemAreaEnum.SlaverBase] = new MySolarSystemAreaOrbit()
            {
                Name = "Slavers area",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.SLAVERS_POSITION * MyBgrCubeConsts.MILLION_KM * 1.001f,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.SLAVERBASE_RADIUS * MyBgrCubeConsts.MILLION_KM * 200,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.SLAVERBASE_RADIUS * MyBgrCubeConsts.MILLION_KM * 500,
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
                    SunProperties = SlaverBaseSunProperties,
                    FogProperties = DefaultFogProperties,
                    ImpostorProperties = SlaverBaseImpostorsProperties,
                    DebrisProperties = DefaultDebrisProperties,

                    SectorObjectsCounts = SlaverBaseObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = DefaultParticleDustProperties,
                },
            };
        }
    }  
}

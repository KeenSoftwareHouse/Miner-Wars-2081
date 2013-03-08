using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.TransparentGeometry;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
        static MySectorObjectCounts SkyObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 0,
            Voxels128 = 0,
            Voxels64 = 0,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 0,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] SkyImpostorsProperties = new MyImpostorProperties[]
        {               /*
                   new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.smoke_field,
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
                  },  */
        };


        public static readonly MySunProperties SkySunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(150 / 255.0f, 205 / 255.0f, 208 / 255.0f),
            SunIntensity = 5,
            SunSpecular = new Vector3(233 / 255.0f, 130 / 255.0f, 29 / 255.0f),
            BackSunDiffuse = new Vector3(155 / 255.0f, 96 / 255.0f, 40 / 255.0f),
            BackSunIntensity = 0.1f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.1f,
            EnvironmentAmbientIntensity = 0.6f,
            BackgroundColor = new Vector3(5, 5, 16),
        };


        public static readonly MyFogProperties SkyFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(180 / 255.0f, 180 / 255.0f, 180 / 255.0f),
            FogNear = 126,
            FogFar = 521,
            FogMultiplier = 0.5f,            
        };

        public static readonly MyParticleDustProperties SkyDustProperties = new MyParticleDustProperties()
        {
            Enabled = false,
            DustBillboardRadius = 30.0f,
            DustFieldCountInDirectionHalf = 4.0f,
            DistanceBetween = 30.3f,
            Color = new Color(120 / 255.0f, 120 / 255.0f, 30 / 255.0f, 1f),
            Texture = MyTransparentMaterialEnum.Dust,
            AnimSpeed = 0,
        };


        public static readonly MyDebrisProperties SkyDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 0,
            FullScaleDistance = 70,
            DistanceBetween = 50.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };




        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateSkyArea()
        {
            Areas[MySolarSystemAreaEnum.Sky] = new MySolarSystemAreaOrbit()
            {
                Name = "Sky",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = Vector3.Zero,
                    LongSpread = 0,
                    MaxDistanceFromOrbitLow = 0,
                    MaxDistanceFromOrbitHigh = 0,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.7f, 0.7f, 0.16f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    SunProperties = SkySunProperties,
                    FogProperties = SkyFogProperties,
                    ImpostorProperties = SkyImpostorsProperties,
                    DebrisProperties = SkyDebrisProperties,

                    SectorObjectsCounts = SkyObjectsProperties,
                    PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                    SecondaryAsteroidMaterials = DefaultSecondaryMaterials,

                    ParticleDustProperties = SkyDustProperties,
                },
            };
        }
    }
}

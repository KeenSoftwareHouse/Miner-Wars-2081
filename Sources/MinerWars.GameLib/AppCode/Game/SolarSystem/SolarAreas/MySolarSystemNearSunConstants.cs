using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
        static MySectorObjectCounts NearSunObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 10,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 0,
            StaticAsteroidMedium = 1500,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] NearSunImpostorsProperties = new MyImpostorProperties[]
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
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_A,
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
                      MinRadius = 0.266f,
                      MaxRadius = 0.266f,
                      AnimationSpeed = new Vector4(0.009f, 0.000f,0.000f, 0),
                      Color = new Vector3(255 / 255.0f, 255 / 255.0f, 145 / 255.0f),
                      Contrast = 4.000f,
                      Intensity = 0.924f
                  },
        };


        public static readonly MySunProperties NearSunSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 226 / 255.0f, 87 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.EARTH_POSITION) * 10, // max?
            SunSpecular = new Vector3(255 / 255.0f, 203 / 255.0f, 103 / 255.0f),
            BackSunDiffuse = new Vector3(228 / 255.0f, 96 / 255.0f, 40 / 255.0f),
            BackSunIntensity = 0.544f,
            AmbientColor = new Vector3(59 / 255.0f, 33 / 255.0f, 29 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.820f,
            SunSizeMultiplier = 20,
            SunRadiationDamagePerSecond = 5,
        };


        public static readonly MyFogProperties NearSunFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(224 / 255.0f, 87 / 255.0f, 0 / 255.0f),
            FogNear = 1000,
            FogFar = 2500,
            FogMultiplier = 0.375f,
            FogBacklightMultiplier = 0.703f,
        };


        public static readonly MyDebrisProperties NearSunDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 2,
            FullScaleDistance = 70,
            DistanceBetween = 50.0f,
            DebrisEnumValues = new MyMwcObjectBuilder_SmallDebris_TypesEnum[] { 
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4, 
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30,
                MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot },

            DebrisVoxelMaterials = new MyMwcVoxelMaterialsEnum[] { MyMwcVoxelMaterialsEnum.Lava_01, MyMwcVoxelMaterialsEnum.Stone_03 },
        };

        public static readonly MyParticleDustProperties NearSunDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 90.00f,  
            DustFieldCountInDirectionHalf = 4.6f, 
            DistanceBetween = 166.3f, 
            Color = new Color(255 / 255.0f, 79 / 255.0f, 12 / 255.0f, 1f),
        };

        public static readonly MyGodRaysProperties NearSunGodRaysProperties = new MyGodRaysProperties()
        {
            Density = 0.097f,
            Weight = 0.522f,
            Decay = 0.992f,
            Exposition = 0.343f,
            Enabled = true,
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> NearSunAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Stone_03, 1 },
            {  MyMwcVoxelMaterialsEnum.Indestructible_01, 1 },            
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> NearSunSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            {  MyMwcVoxelMaterialsEnum.Uranite_01, 1 },
            {  MyMwcVoxelMaterialsEnum.Lava_01, 1 },
        };



        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateNearSunArea()
        {
            Areas[MySolarSystemAreaEnum.NearSun] = new MySolarSystemAreaOrbit()
            {
                Name = "Near Sun area",
                AreaType = MySolarSystemArea.AreaEnum.Sun,
                SecondaryStaticAsteroidMaterial = MyMwcVoxelMaterialsEnum.Lava_01,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = new Vector3(0,0,3) * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 1,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.SUN_RADIUS * MyBgrCubeConsts.MILLION_KM * 13000,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.SUN_RADIUS * MyBgrCubeConsts.MILLION_KM * 2000,
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
                    SunProperties = NearSunSunProperties,
                    FogProperties = NearSunFogProperties,
                    ImpostorProperties = NearSunImpostorsProperties,
                    DebrisProperties = NearSunDebrisProperties,

                    SectorObjectsCounts = NearSunObjectsProperties,
                    PrimaryAsteroidMaterials = NearSunAsteroidMaterials,
                    SecondaryAsteroidMaterials = NearSunSecondaryMaterials,

                    ParticleDustProperties = NearSunDustProperties,
                    GodRaysProperties = NearSunGodRaysProperties,
                },
            };
        }
    }
}

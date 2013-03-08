using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Render.SolarMap;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.BackgroundCube;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
        static MySectorObjectCounts ValiantObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 0,
            Voxels128 = 0,
            Voxels64 = 0,
            StaticAsteroidLarge = 300,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 0
        };

        public readonly static MyImpostorProperties[] ValiantImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 0.472f,
                      MaxRadius = 0.472f,
                      AnimationSpeed = new Vector4(0.004f, 0.001f,0.003f, 0),
                      Color = new Vector3(255 / 255.0f, 255 / 255.0f, 255 / 255.0f),
                      Contrast = 6.170f,
                      Intensity = 1.135f
                  },
        };


        public static readonly MySunProperties ValiantSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(255 / 255.0f, 200 / 255.0f, 127 / 255.0f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.VALIANT_POSITION) * 1.5f,
            SunSpecular = new Vector3(232 / 255.0f, 132 / 255.0f, 29 / 255.0f),
            BackSunDiffuse = new Vector3(70 / 255.0f, 129 / 255.0f, 181 / 255.0f),
            BackSunIntensity = 1.39f,
            AmbientColor = new Vector3(36 / 255.0f, 36 / 255.0f, 36 / 255.0f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 0.516f,
        };


        public static readonly MyFogProperties ValiantFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(68 / 255.0f, 205 / 255.0f, 255 / 255.0f),
            FogNear = 5318,
            FogFar = 10691,
            FogMultiplier = 0.383f,
        };

        public static readonly MyParticleDustProperties ValiantDustProperties = new MyParticleDustProperties()
        {
            DustBillboardRadius = 102.96f,
            DustFieldCountInDirectionHalf = 4.477f,
            DistanceBetween = 112.900f,
            Color = new Color(117 / 255.0f, 166 / 255.0f, 198 / 40.0f, 1f),
        };

        public static readonly MyDebrisProperties ValiantDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 2,
            FullScaleDistance = 70,
            DistanceBetween = 50.0f,
            //DebrisEnumValues = MyMwcObjectBuilder_SmallDebris_TypesEnum.
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> ValiantAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},
        };


        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateValiantArea()
        {
            Areas[MySolarSystemAreaEnum.FortValiant] = new MySolarSystemAreaOrbit()
            {
                Name = "Fort Valiant Belt",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.VALIANT_POSITION * MyBgrCubeConsts.MILLION_KM,
                    LongSpread = 0.1f,
                    MaxDistanceFromOrbitLow = 0.5f * MyBgrCubeConsts.MILLION_KM * 200,
                    MaxDistanceFromOrbitHigh = 0.6f * MyBgrCubeConsts.MILLION_KM * 200,
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
                    SunProperties = ValiantSunProperties,
                    FogProperties = ValiantFogProperties,
                    ImpostorProperties = ValiantImpostorsProperties,
                    DebrisProperties = ValiantDebrisProperties,

                    SectorObjectsCounts = ValiantObjectsProperties,
                    PrimaryAsteroidMaterials = ValiantAsteroidMaterials,
                    SecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
                    {
                        { MyMwcVoxelMaterialsEnum.Uranite_01, 0.5f},
                        { MyMwcVoxelMaterialsEnum.Gold_01, 0.5f},
                        { MyMwcVoxelMaterialsEnum.Ice_01, 0.5f},
                    },

                    ParticleDustProperties = ValiantDustProperties,
                },
            };
        }
    }
}

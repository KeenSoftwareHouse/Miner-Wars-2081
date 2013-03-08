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
        public static Dictionary<MySolarSystemAreaEnum, MySolarSystemArea> Areas { get; private set; }

        public readonly static MySectorObjectCounts MinimumObjectsProperties = new MySectorObjectCounts()
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

        public readonly static MySectorObjectCounts DefaultObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 5,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 4,
            StaticAsteroidMedium = 5000,
            StaticAsteroidSmall = 0,
            Motherships = 0,
            StaticDebrisFields = 500
        };

        public readonly static MySectorObjectCounts ManyObjectsProperties = new MySectorObjectCounts()
        {
            Voxels512 = 0,
            Voxels256 = 5,
            Voxels128 = 10,
            Voxels64 = 10,
            StaticAsteroidLarge = 40,
            StaticAsteroidMedium = 7000,
            StaticAsteroidSmall = 0,
            Motherships = 10,
            StaticDebrisFields = 500
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> DefaultAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {            
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> DefaultSecondaryMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
        };

        public static List<MyMwcVoxelMaterialsEnum> CreateAllowedMaterials(Dictionary<MyMwcVoxelMaterialsEnum, float> primary, Dictionary<MyMwcVoxelMaterialsEnum, float> secondary)
        {
            List<MyMwcVoxelMaterialsEnum> list = new List<MyMwcVoxelMaterialsEnum>();
            if (primary != null)
            {
                foreach (var pair in primary)
                {
                    if (pair.Value > 0)
                        list.Add(pair.Key);
                }
            }
            if (secondary != null)
            {
                foreach (var pair in secondary)
                {
                    if (pair.Value > 0)
                        list.Add(pair.Key);
                }
            }
            return list;
        }


        public readonly static MyImpostorProperties[] ManyImpostorsProperties = new MyImpostorProperties[]
        {
                   new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_A,
                      ImpostorsCount = 5000,
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
                      ImpostorsCount = 5000,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 10.1f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 11,
                      MinRadius = 1000,
                      MaxRadius = 2000,
                      AnimationSpeed = new Vector4(-0.0001f,0,0,0),
                      Color = Vector3.One
                  },

                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid50m_D,
                      ImpostorsCount = 1000,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 10.1f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 11,
                      MinRadius = 5000,
                      MaxRadius = 8000,
                      AnimationSpeed = Vector4.Zero,
                      Color = Vector3.One
                  },

                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Billboards,
                      Material = MyTransparentMaterialEnum.Impostor_StaticAsteroid50m_E,
                      ImpostorsCount = 1000,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.9f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 2.0f,
                      MinRadius = 500,
                      MaxRadius = 800,
                      AnimationSpeed = Vector4.Zero,
                      Color = Vector3.One
                  },

                  new MyImpostorProperties()
                  {
                      ImpostorType = MyVoxelMapImpostors.MyImpostorType.Nebula,
                      Material = null,
                      ImpostorsCount = 0,
                      MinDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.0f,
                      MaxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF * 1.3f,
                      MinRadius = 1.0f,
                      MaxRadius = 1.0f,
                      AnimationSpeed = new Vector4(0.001f, 0.001f,0.003f, 0),
                      Color = new Vector3(0.4f, 0.9f, 0.4f)
                  },
        };

        public readonly static MyImpostorProperties[] DefaultImpostorsProperties = new MyImpostorProperties[]
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
                      MinRadius = 1.0f,
                      MaxRadius = 1.0f,
                      AnimationSpeed = new Vector4(0.001f, 0.001f,0.003f, 0),
                      Color = new Vector3(0.4f, 0.9f, 0.4f)
                  },
        };

        public static readonly MySunProperties DefaultSunProperties = new MySunProperties()
        {
            SunDiffuse = new Vector3(1.0f, 1.0f, 0.8f),
            SunIntensity = MySolarSystemConstants.SunIntensityFromPosition(MyBgrCubeConsts.EARTH_POSITION),
            SunSpecular = new Vector3(0.9137255f, 0.6078432f, 0.2078431f),
            BackSunDiffuse = new Vector3(0.0627451f, 0.04313726f, 0.03529412f),
            BackSunIntensity = 1.357f,
            AmbientColor = new Vector3(0.14f),
            AmbientMultiplier = 0.2f,
            EnvironmentAmbientIntensity = 1.6f,
            SunSizeMultiplier = 1.0f,
        };
        
        public static readonly MyFogProperties DefaultFogProperties = new MyFogProperties()
        {
            FogColor = new Vector3(30 / 255.0f, 36 / 255.0f, 51 / 255.0f),
            FogNear = 3000,
            FogFar = 3633,
            FogMultiplier = 0.0f,
        };

        public static readonly MyDebrisProperties DefaultDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 2,
            FullScaleDistance = 70,
            DistanceBetween = 50.0f,
        };

        public static readonly MyDebrisProperties ManyDebrisProperties = new MyDebrisProperties()
        {
            MaxDistance = 200,
            CountInDirectionHalf = 5,
            FullScaleDistance = 70,
            DistanceBetween = 50.0f,
        };

        public static readonly MyParticleDustProperties DefaultParticleDustProperties = new MyParticleDustProperties()
        {
        };

        public static readonly MyGodRaysProperties DefaultGodRaysProperties = new MyGodRaysProperties()
        {
        };

        static MySolarSystemConstants()
        {
            CreateAreas();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position in million km</param>
        /// <returns></returns>
        public static float SunIntensityFromPosition(Vector3 positionInMillKm)
        {
            float baseIntensity = MyBgrCubeConsts.EARTH_POSITION.Length();
            float length = positionInMillKm.Length();
            return MathHelper.Clamp(baseIntensity / (length), 0.3f, 5f);
        }

        public static MySolarSystemArea GetDefaultArea()
        {
            CreateAreas();
            return Areas[MySolarSystemAreaEnum.Earth];
        }

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateAreas()
        {
            Areas = new Dictionary<MySolarSystemAreaEnum, MySolarSystemArea>(KeenSoftwareHouse.Library.Collections.Comparers.EnumComparer<MySolarSystemAreaEnum>.Instance);

            CreateMercuryArea();
            CreateVenusArea();
            CreateEarthArea();
            CreateMarsArea();
            CreateJupiterArea();
            CreateSaturnArea();
            CreateUranArea();
            CreateNeptunArea();


            CreateAsteroidBeltArea();
            CreateAsteroidBelt2Area();
            CreateNearSunArea();
            CreateLaikaArea();
            CreateJupiterBorderArea();
            
            CreateMars2Area();

            CreateSlaverBaseArea();
             
            CreateNebulaArea();
            
            CreateMars3Area();
            CreateJupiter2Area();
            CreateMars3Area();
            CreateChinesePowerPlantArea();
            CreateMedina622Area();
            CreateSmallPirateBase2Area();
            CreateSlaversBase2Area();
            CreateRussianTransmitterArea();    

            CreateJunkyardArea();

            CreateRimeArea();
            CreateChineseRafinaryArea();
            CreateChineseMinesArea();

            CreateValiantArea();

            Create3rdTransmitterArea();
            CreateChineseTransmitterArea();

            CreateHallArea();
            CreateHell25DArea();
            CreateSkyArea();

            foreach (var area in Areas)
            {
                area.Value.SectorData.AllowedAsteroidMaterials = CreateAllowedMaterials(area.Value.SectorData.PrimaryAsteroidMaterials, area.Value.SectorData.SecondaryAsteroidMaterials);
            }

            Areas[MySolarSystemAreaEnum.Earth].SectorData.AllowedAsteroidMaterials.Clear();
        }
    }
}

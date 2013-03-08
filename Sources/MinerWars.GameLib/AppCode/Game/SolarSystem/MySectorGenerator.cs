#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

#endregion

namespace MinerWars.AppCode.Game.SolarSystem
{

    enum MySectorObjectType
    {
        Voxel512, Voxel256, Voxel128, Voxel64, StaticAsteroidLarge, StaticAsteroidMedium, StaticAsteroidSmall, Motherships, DebrisFields
    }

    struct MyObjectInfo
    {
        public MySolarSystemEntityEnum EntityType { get; set; }
        public float SizeInMeters { get; set; } //Diameter
        public float ObjectCount { get; set; }
    }

    class MySectorObjectCounts
    {
        public const float LargeAsteroidSize = 10000;
        public const float MediumAsteroidSize = 1000;
        public const float SmallAsteroidSize = 100;

        public MyStaticAsteroidTypeSetEnum StaticAsteroidTypeset = MyStaticAsteroidTypeSetEnum.A | MyStaticAsteroidTypeSetEnum.B;

        public Dictionary<int, float> Values { get; private set; }

        public float this[MySectorObjectType objectType]
        {
            get
            {
                int key = (int)objectType;
                float count;
                if (!Values.TryGetValue(key, out count))
                {
                    count = 0; // Default count is zero
                }
                return count;
            }
            set
            {
                Values[(int)objectType] = value;
            }
        }

        /// <summary>
        /// Key is voxel size in voxels, value is asteroid count
        /// </summary>
        public IEnumerable<KeyValuePair<int, float>> VoxelsAsteroids
        {
            get
            {
                yield return new KeyValuePair<int, float>(512, this[MySectorObjectType.Voxel512]);
                yield return new KeyValuePair<int, float>(256, this[MySectorObjectType.Voxel256]);
                yield return new KeyValuePair<int, float>(128, this[MySectorObjectType.Voxel128]);
                yield return new KeyValuePair<int, float>(64, this[MySectorObjectType.Voxel64]);
            }
        }

        /// <summary>
        /// Key is asteroid size in meters, value is asteroid count
        /// </summary>
        public IEnumerable<KeyValuePair<float, float>> StaticAsteroids
        {
            get
            {
                yield return new KeyValuePair<float, float>(LargeAsteroidSize, this[MySectorObjectType.StaticAsteroidLarge]);
                yield return new KeyValuePair<float, float>(MediumAsteroidSize, this[MySectorObjectType.StaticAsteroidMedium]);
                yield return new KeyValuePair<float, float>(SmallAsteroidSize, this[MySectorObjectType.StaticAsteroidSmall]);
            }
        }

        /// <summary>
        /// All asteroids, key is size in meters, value is asteroid count
        /// </summary>
        public IEnumerable<MyObjectInfo> AllObjects
        {
            get
            {
                var voxelInfo = VoxelsAsteroids.Select(s => new MyObjectInfo() { SizeInMeters = s.Key * MyVoxelConstants.VOXEL_SIZE_IN_METRES, ObjectCount = s.Value, EntityType = MySolarSystemEntityEnum.VoxelAsteroid });
                var asteroidInfo = StaticAsteroids.Select(s => new MyObjectInfo() { SizeInMeters = s.Key, EntityType = MySolarSystemEntityEnum.StaticAsteroid, ObjectCount = s.Value });
                var otherInfo = new MyObjectInfo[]
                {
                    new MyObjectInfo() { ObjectCount = Motherships, EntityType = MySolarSystemEntityEnum.LargeShip, SizeInMeters = 500},
                    new MyObjectInfo() { ObjectCount = StaticDebrisFields, EntityType = MySolarSystemEntityEnum.DebrisField, SizeInMeters = 50},
                };
                return voxelInfo.Union(asteroidInfo).Union(otherInfo).OrderByDescending(s => s.SizeInMeters);
            }
        }

        public float Voxels512 { get { return this[MySectorObjectType.Voxel512]; } set { this[MySectorObjectType.Voxel512] = value; } }
        public float Voxels256 { get { return this[MySectorObjectType.Voxel256]; } set { this[MySectorObjectType.Voxel256] = value; } }
        public float Voxels128 { get { return this[MySectorObjectType.Voxel128]; } set { this[MySectorObjectType.Voxel128] = value; } }
        public float Voxels64 { get { return this[MySectorObjectType.Voxel64]; } set { this[MySectorObjectType.Voxel64] = value; } }

        public float StaticAsteroidLarge { get { return this[MySectorObjectType.StaticAsteroidLarge]; } set { this[MySectorObjectType.StaticAsteroidLarge] = value; } }
        public float StaticAsteroidMedium { get { return this[MySectorObjectType.StaticAsteroidMedium]; } set { this[MySectorObjectType.StaticAsteroidMedium] = value; } }
        public float StaticAsteroidSmall { get { return this[MySectorObjectType.StaticAsteroidSmall]; } set { this[MySectorObjectType.StaticAsteroidSmall] = value; } }

        public float Motherships { get { return this[MySectorObjectType.Motherships]; } set { this[MySectorObjectType.Motherships] = value; } }
        public float StaticDebrisFields { get { return this[MySectorObjectType.DebrisFields]; } set { this[MySectorObjectType.DebrisFields] = value; } }

        public MySectorObjectCounts()
        {
            Values = new Dictionary<int, float>(Enum.GetValues(typeof(MySectorObjectType)).Length);
        }

        /// <param name="interpolator">1 means use other object</param>
        public MySectorObjectCounts InterpolateWith(MySectorObjectCounts otherObject, float interpolator)
        {
            var result = new MySectorObjectCounts();
            // This function assumes that both collection contains all types of objects
            foreach (var kv in Values)
            {
                float currentValue = kv.Value;
                float otherValue = otherObject.Values[kv.Key];
                result.Values[kv.Key] = MathHelper.Lerp(currentValue, otherValue, interpolator);
            }

            StaticAsteroidTypeset = interpolator > 0.5f ? StaticAsteroidTypeset : otherObject.StaticAsteroidTypeset;

            return result;
        }
    }

    class MySectorGenerator
    {
        int m_seed;

        /// <summary>
        /// Deviation of vein in every step.
        /// Higher value means vein will be more curved.
        /// </summary>
        const float VeinAngleDeviation = MathHelper.Pi / 8; // 22.5 degrees

        /// <summary>
        /// Maximum level of vein subdivision
        /// </summary>
        const int MaxLevel = 1;

        /// <summary>
        /// Base thickness of secondary material vein.
        /// </summary>
        const float BaseSecondaryMaterialThickness = 40;

        /// <summary>
        /// Defines how many times we try find not colliding position for entity
        /// </summary>
        const int MaxCollisionsTestsForEntity = 100;

        /// <summary>
        /// Safe areas, where no generated object will collide (in 1000km units)!!!!
        /// </summary>
        private List<BoundingSphere> m_safeAreas;

        Dictionary<MyMwcVector3Int, MySolarSystemAreaEnum> m_customSectors = new Dictionary<MyMwcVector3Int, MySolarSystemAreaEnum>();


        public MySectorGenerator(int seed)
            : this(seed, new List<BoundingSphere>())
        {
        }

        public MySectorGenerator(int seed, List<BoundingSphere> safeAreas)
        {
            m_seed = seed;
            m_safeAreas = safeAreas;

            //Fragile
            m_customSectors.Add(new MyMwcVector3Int(88, 0, -58), MySolarSystemAreaEnum.Laika);
            //Arabian Border
            m_customSectors.Add(new MyMwcVector3Int(18, 0, 96), MySolarSystemAreaEnum.Junkyard);
            //Military Outpost
            m_customSectors.Add(new MyMwcVector3Int(-66, 0, -44), MySolarSystemAreaEnum.Mercury);
            //CKD Mothership Facility            
            m_customSectors.Add(new MyMwcVector3Int(93, 0, 73), MySolarSystemAreaEnum.RussianTransmitter);
            //Nearby Stations
            m_customSectors.Add(new MyMwcVector3Int(19, 0, -92), MySolarSystemAreaEnum.Junkyard);
            //Research Vessel
            m_customSectors.Add(new MyMwcVector3Int(-100, 0, 20), MySolarSystemAreaEnum.Nebula);
            //Uranite Mines
            m_customSectors.Add(new MyMwcVector3Int(64, 0, -20), MySolarSystemAreaEnum.RussianTransmitter);
            //Warehouse Deathmatch
            m_customSectors.Add(new MyMwcVector3Int(-30, 0, -61), MySolarSystemAreaEnum.SlaversBase2);
            //Hall of fame
            m_customSectors.Add(new MyMwcVector3Int(53, 0, 84), MySolarSystemAreaEnum.Hall);
            //Rift Station Deathmatch
            m_customSectors.Add(new MyMwcVector3Int(-93, 0, 46), MySolarSystemAreaEnum.NearSun);
            //New NYC Deathmatch
            m_customSectors.Add(new MyMwcVector3Int(-78, 0, 35), MySolarSystemAreaEnum.Uranus);    

            //Military Area (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(-79, 0, 70), MySolarSystemAreaEnum.Junkyard);            
            //Miner Outpost (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(-8, 0, -5), MySolarSystemAreaEnum.Uranus);
            //City fight (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(93, 0, 14), MySolarSystemAreaEnum.RussianTransmitter);
            //Plain (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(-43, 0, 86), MySolarSystemAreaEnum.Venus);
            //gates of Hell (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(-38, 0, 71), MySolarSystemAreaEnum.Hell25D);
            //Junkyard (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(-15, 0, -42), MySolarSystemAreaEnum.Junkyard);
            //Asteroid Field (2.5D)
            m_customSectors.Add(new MyMwcVector3Int(17, 0, 67), MySolarSystemAreaEnum.Neptune);

            //Sky (Builder)
            m_customSectors.Add(new MyMwcVector3Int(16, 16, 16), MySolarSystemAreaEnum.Sky);
            m_customSectors.Add(new MyMwcVector3Int(17, 17, 17), MySolarSystemAreaEnum.Sky);
            m_customSectors.Add(new MyMwcVector3Int(18, 18, 18), MySolarSystemAreaEnum.Sky);  
        }

        public static bool IsSectorInForbiddenArea(MyMwcVector3Int sector)
        {
            return ((sector.X <= MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.X) && (sector.Y <= MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.Y) && (sector.Z <= MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.Z)
             && (sector.X >= -MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.X) && (sector.Y >= -MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.Y) && (sector.Z >= -MyGuiConstants.SOLAR_SYSTEM_FORBIDDEN_AREA.Z));
        }

        private bool IsSectorCustom(MyMwcVector3Int sector)
        {
            return MyGuiScreenGamePlay.Static.Checkpoint.CurrentSector.SectorType == MyMwcSectorTypeEnum.SANDBOX && m_customSectors.ContainsKey(sector);
        }

        private MySolarSystemAreaEnum GetCustomSectorArea(MyMwcVector3Int sector)
        {
            return m_customSectors[sector];
        }

        /// <summary>
        /// Generates sector with entities from SolarMap and SectorId
        /// </summary>
        /// <param name="solarData">SolarMap data.</param>
        /// <param name="sector">Sector to generate.</param>
        /// <param name="minimalEntitySize">Minimal entity size. When far from sector, no need to display all entities.</param>
        /// <returns>MySolarSystemMapSectorData - collection of sector entities.</returns>
        public MySolarSystemMapSectorData GenerateSectorEntities(MySolarSystemMapData solarData, MyMwcVector3Int sector, float minimalEntitySize, int maxEntityCount, bool onlyStaticAsteroids)
        {
            MySolarSystemAreaEnum? customArea = null;
            if (IsSectorCustom(sector))
            {
                customArea = GetCustomSectorArea(sector);
            }
            else
            if (IsSectorInForbiddenArea(sector))
            {
                sector = MySolarSystemUtils.MillionKmToSectors(MyBgrCubeConsts.EARTH_POSITION);
            }

            MySolarSystemMapSectorData data;
            if (!solarData.SectorData.TryGetValue(sector, out data) || data.MinimalEntitySize > minimalEntitySize)
            {
                data = new MySolarSystemMapSectorData(sector, minimalEntitySize);
                solarData.SectorData[sector] = data;
            }

            // Apply all areas to sector, area tests whether it influents sector or not
            if (!customArea.HasValue)
            {
                foreach (var areaEnum in solarData.Areas)
                {
                    var area = MySolarSystemConstants.Areas[areaEnum];

                    var interpolator = area.GetSectorInterpolator(data.SectorPosition);

                    if (interpolator > 0 && interpolator > data.AreaInfluenceMultiplier)
                    {
                        data.Area = areaEnum;
                        data.AreaInfluenceMultiplier = interpolator;
                    }
                }
            }
            else
            {
                data.Area = customArea.Value;
                data.AreaInfluenceMultiplier = 1;
            }

            AddSectorEntities(data, minimalEntitySize, maxEntityCount, onlyStaticAsteroids);

            return data;
        }


        public MyMwcObjectBuilder_Sector GenerateObjectBuilders(MyMwcVector3Int sectorPosition, MySectorObjectCounts sectorObjectCounts, bool onlyStaticAsteroids)
        {
            Random rnd = new Random(m_seed);
            List<MySolarSystemMapEntity> entities = new List<MySolarSystemMapEntity>();
            AddSectorEntities(sectorObjectCounts, sectorPosition, rnd, 0, int.MaxValue, entities, onlyStaticAsteroids);

            List<MyMwcObjectBuilder_Base> sectorObjects = new List<MyMwcObjectBuilder_Base>();

            MyWeightDictionary<MyMwcVoxelMaterialsEnum> primaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(MySolarSystemConstants.DefaultAsteroidMaterials);
            MyWeightDictionary<MyMwcVoxelMaterialsEnum> secondaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(MySolarSystemConstants.DefaultSecondaryMaterials);

            GenerateSectorObjectBuildersFromSolarEntities(entities, sectorObjects, rnd, primaryMaterials, secondaryMaterials, sectorObjectCounts.StaticAsteroidTypeset);

            return new MyMwcObjectBuilder_Sector()
            {
                SectorObjects = sectorObjects,
            };
        }

        public void GenerateSectorObjectBuildersFromSolarEntities(List<MySolarSystemMapEntity> entities, List<MyMwcObjectBuilder_Base> addToList, Random rnd, MyWeightDictionary<MyMwcVoxelMaterialsEnum> primaryMaterials, MyWeightDictionary<MyMwcVoxelMaterialsEnum> secondaryMaterials, MyStaticAsteroidTypeSetEnum staticAsteroidTypesets, MyMwcVoxelMaterialsEnum? fieldMaterial = null, MySolarSystemArea.AreaEnum? areaType = null)
        {
            List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroids = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>(5);
            List<MyMwcVoxelFilesEnum> voxelAsteroids = new List<MyMwcVoxelFilesEnum>(10);

            int count = addToList.Count;

            foreach (var e in entities)
            {
                if (e.EntityType == MySolarSystemEntityEnum.VoxelAsteroid)
                {
                    int voxelAsteroidSize = FindAsteroidSize(e.Radius, MyVoxelMap.AsteroidSizes);

                    int rndIndex = rnd.Next(0, voxelAsteroids.Count);

                    MyVoxelMap.GetAsteroidsBySizeInMeters(voxelAsteroidSize, voxelAsteroids, false);

                    MyMwcObjectBuilder_VoxelMap builder = GenerateVoxelMap(voxelAsteroidSize, e.PositionInSector, rnd, voxelAsteroids, primaryMaterials, secondaryMaterials);

                    addToList.Add(builder);
                }
                else if (e.EntityType == MySolarSystemEntityEnum.StaticAsteroid)
                {
                    float radius = 100;
                    if (e.Radius == 10000)
                        radius = rnd.Next(2000, 11000);
                    if (e.Radius == 1000)
                        radius = rnd.Next(100, 1100);
                    if (e.Radius == 100)
                        radius = rnd.Next(10, 100);


                    MyMwcVoxelMaterialsEnum asteroidMaterial = MyMwcVoxelMaterialsEnum.Stone_01;
                    if (primaryMaterials.Count > 0)
                        primaryMaterials.GetRandomItem(rnd);

                    MyStaticAsteroidTypeSetEnum asteroidType = MyStaticAsteroidTypeSetEnum.A;

                    //for (int i = 0; i < 40000000; i++)
                    {
                        asteroidType = (MyStaticAsteroidTypeSetEnum)rnd.Item(Enum.GetValues(typeof(MyStaticAsteroidTypeSetEnum)));
                    }

                    if ((staticAsteroidTypesets & MyStaticAsteroidTypeSetEnum.A) == MyStaticAsteroidTypeSetEnum.A)
                        asteroidType = MyStaticAsteroidTypeSetEnum.A;
                    if ((staticAsteroidTypesets & MyStaticAsteroidTypeSetEnum.B) == MyStaticAsteroidTypeSetEnum.B)
                        asteroidType = MyStaticAsteroidTypeSetEnum.B;
                    if ((staticAsteroidTypesets & MyStaticAsteroidTypeSetEnum.All) == MyStaticAsteroidTypeSetEnum.All)
                        asteroidType = rnd.Float(0, 1) > 0.5f ? MyStaticAsteroidTypeSetEnum.A : MyStaticAsteroidTypeSetEnum.B;

                    var builder = GenerateStaticAsteroid(radius, asteroidType, asteroidMaterial, e.PositionInSector, rnd, asteroids);

  
                    builder.AsteroidMaterial1 = fieldMaterial;
                    if (areaType == MySolarSystemArea.AreaEnum.Sun)
                    {
                        builder.FieldDir = MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
                    }

                    builder.Generated = true;
                    addToList.Add(builder);

                 
                    //MyEntity ent = MyEntities.CreateFromObjectBuilderAndAdd(null, new MyMwcObjectBuilder_StaticAsteroid(asteroids[rndIndex], mat),
                    //    Matrix.CreateWorld(e.PositionInSector, rnd.Vector(1), rnd.Vector(1)));
                }
                else if (e.EntityType == MySolarSystemEntityEnum.LargeShip)
                {
                    var shipType = rnd.Enum<MyMwcObjectBuilder_PrefabLargeShip_TypesEnum>();
                    MyMwcObjectBuilder_Prefab_AppearanceEnum appearance = rnd.Enum<MyMwcObjectBuilder_Prefab_AppearanceEnum>();

                    var ship = new MyMwcObjectBuilder_PrefabLargeShip(shipType, appearance, new MyMwcVector3Short(0, 0, 0), rnd.Vector(1), null, rnd.FloatNormal(), "Abandoned large ship", 0, false, 0);                    
                    var gamePlayProperties = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, (int)shipType, MyMwcObjectBuilder_FactionEnum.Euroamerican);
                    ship.PrefabHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
                    ship.PrefabMaxHealth = gamePlayProperties.MaxHealth;
                    var prefabs = new List<MyMwcObjectBuilder_PrefabBase>();
                    prefabs.Add(ship);
                    var container = new MyMwcObjectBuilder_PrefabContainer(0, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabs, 0, rnd.Enum<MyMwcObjectBuilder_FactionEnum>(), null);
                    container.PositionAndOrientation = new MyMwcPositionAndOrientation(e.PositionInSector, Vector3.Forward, Vector3.Up);
                    addToList.Add(container);
                }
                else if (e.EntityType == MySolarSystemEntityEnum.DebrisField)
                {
                    MyMwcObjectBuilder_LargeDebrisField objectBuilder = new MyMwcObjectBuilder_LargeDebrisField(MyMwcObjectBuilder_LargeDebrisField_TypesEnum.Debris84);
                    objectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(e.PositionInSector, rnd.Vector(1), rnd.Vector(1));
                    addToList.Add(objectBuilder);
                }
            }
        }

        public MySolarSystemMapSectorData GenerateSectorObjectBuilders(MyMwcVector3Int sector, MySolarSystemMapData solarData, List<MyMwcObjectBuilder_Base> addToList, bool onlyStaticAsteroids)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateSectorEntities");
            var sectorData = GenerateSectorEntities(solarData, sector, 0, int.MaxValue, onlyStaticAsteroids);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyDynamicAABBTree prunningStructure = new MyDynamicAABBTree(Vector3.Zero);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Misc");

            MyWeightDictionary<MyMwcVoxelMaterialsEnum> primaryMaterials;
            MyWeightDictionary<MyMwcVoxelMaterialsEnum> secondaryMaterials;

            MyMwcObjectBuilder_Sector sectorBuilder = addToList.FirstOrDefault(s => s.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.Sector) as MyMwcObjectBuilder_Sector;

            if (sectorBuilder == null)
            {
                sectorBuilder = GetSectorBuilder(sectorData);
                addToList.Add(sectorBuilder);
            }
            //if (sectorBuilder != null)
            //{
            //    Vector3 kms = MySolarSystemUtils.SectorsToKm(sectorData.SectorPosition);
            //    sectorBuilder.SunDistance = kms.Length();
            //}

            MySector.Area = sectorData.Area;

            MinerWars.AppCode.Game.SolarSystem.MySolarSystemArea.AreaEnum? areaType = null;
            MyMwcVoxelMaterialsEnum? secondaryAsteroidMaterial = null;
            if (sectorData.Area.HasValue && sectorBuilder != null)
            {
                var area = MySolarSystemConstants.Areas[sectorData.Area.Value];
                areaType = area.AreaType;
                secondaryAsteroidMaterial = area.SecondaryStaticAsteroidMaterial;
                primaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(area.SectorData.PrimaryAsteroidMaterials);
                secondaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(area.SectorData.SecondaryAsteroidMaterials);
            }
            else
            {
                primaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(MySolarSystemConstants.DefaultAsteroidMaterials);
                secondaryMaterials = new MyWeightDictionary<MyMwcVoxelMaterialsEnum>(MySolarSystemConstants.DefaultSecondaryMaterials);
            }

            Random rnd = new Random(m_seed);

            MyStaticAsteroidTypeSetEnum staticAsteroidsTypeset = MyStaticAsteroidTypeSetEnum.A | MyStaticAsteroidTypeSetEnum.B;

            var realSizes = sectorData.Entities.GroupBy(s => s.Radius).Select(s => new { Key = s.Key, Count = s.Count() }).ToArray();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateSectorObjectBuildersFromSolarEntities");
            GenerateSectorObjectBuildersFromSolarEntities(sectorData.Entities, addToList, rnd, primaryMaterials, secondaryMaterials, staticAsteroidsTypeset, secondaryAsteroidMaterial, areaType);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (!onlyStaticAsteroids)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateStations");
                GenerateStations(sector, solarData, sectorData, addToList, rnd, prunningStructure);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateBots");
                GenerateBots(sectorData, addToList, rnd, prunningStructure);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            return sectorData;
        }


        static List<MyElement> m_elements = new List<MyElement>(256);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingEntities"></param>
        /// <param name="rnd"></param>
        /// <param name="radius"></param>
        /// <param name="positionMultiplier">How far from offset shoud be position generated</param>
        /// <param name="spacing"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Vector3? FindEntityPosition(MyDynamicAABBTree existingEntities, Random rnd, float radius, float positionMultiplier = 0.8f, float spacing = 1.0f, Vector3 offset = new Vector3())
        {
            bool collide = true;
            Vector3 pos = new Vector3();
            int testCount = 0;

            //collide = false;
            Vector3 halfSize = MyMwcSectorConstants.SECTOR_SIZE / 2.0f * new Vector3(positionMultiplier);
            halfSize -= new Vector3(radius);

            while (collide && testCount < MaxCollisionsTestsForEntity)
            {
                pos = offset + rnd.Vector(halfSize);
                testCount++;

                collide = false;
                // try detect collisions with safe areas
              //  collide = IsEntityCollideWithSafeAreas(pos, radius);
                //if (collide)
                  //  continue;

                /*
                foreach (var e in existingEntities)
                {
                    if ((e.PositionInSector - pos).Length() < (e.Radius + radius) * spacing)
                    {
                        collide = true;
                        break;
                    }
                } */

                BoundingBox bb = new BoundingBox(pos - new Vector3(radius) * spacing, pos + new Vector3(radius) * spacing);
                existingEntities.OverlapAllBoundingBox(ref bb, m_elements);
                if (m_elements.Count > 0)
                {
                    collide = true;
                    continue;
                }
            }
            return !collide ? (Vector3?)pos : null;
        }

        public static MyMwcObjectBuilder_SectorObjectGroups LoadSectorGroups(MyMwcVector3Int sectorPosition)
        {
            var builder = new MyMwcObjectBuilder_SectorObjectGroups();
            var sector = MinerWars.AppCode.Networking.MyLocalCache.LoadSector(new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, null, sectorPosition, String.Empty));
            builder.Groups = sector.ObjectGroups;
            builder.Entities = sector.SectorObjects;
            return builder;
        }

        private MyMwcObjectBuilder_SectorObjectGroups LoadObjectGroups(MyMwcVector3Int sectorPosition)
        {
            try
            {
                return LoadSectorGroups(sectorPosition);               
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Cannot LoadObjectGroups, sector generator won't insert stations into scene");
                MyMwcLog.WriteLine(e);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
                MyGuiManager.BackToMainMenu();
                return null;
            }
        }

        private void GenerateStations(MyMwcVector3Int sector, MySolarSystemMapData solarData, MySolarSystemMapSectorData sectorData, List<MyMwcObjectBuilder_Base> addToList, Random rnd, MyDynamicAABBTree prunningStructure)
        {
            Dictionary<MyMwcVector3Int, MyMwcObjectBuilder_SectorObjectGroups> groupCache = new Dictionary<MyMwcVector3Int, MyMwcObjectBuilder_SectorObjectGroups>();

            //List<MyImportantSolarObject>
            var objects = solarData.ImportantObjects.Where(o => o.NavigationMark.Sector.Equals(sector));

            foreach(var obj in objects)
            {
                var size = MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER / 1000;
                Vector3? pos = FindEntityPosition(prunningStructure, rnd, size);
                if (pos.HasValue)
                {
                    var templateSector = MyTemplateGroups.GetGroupSector(obj.TemplateGroup);
                    MyMwcObjectBuilder_SectorObjectGroups groups;
                    if (!groupCache.TryGetValue(templateSector, out groups))
                    {
                        groups = LoadObjectGroups(templateSector);
                        if (groups == null)
                        {
                            return;
                        }
                        groupCache.Add(templateSector, groups);
                    }

                    sectorData.Entities.Add(new MySolarSystemMapEntity(sector, pos.Value, size, "", MySolarSystemEntityEnum.OutpostIcon));

                    var group = rnd.Item(groups.Groups);
                    IEnumerable<MyMwcObjectBuilder_PrefabBase> prefabs = group.GetPrefabBuilders(groups.Entities);
                    IEnumerable<MyMwcObjectBuilder_Base> rootObjects = group.GetRootBuilders(groups.Entities);
                    var objects3d = rootObjects.OfType<MyMwcObjectBuilder_Object3dBase>();

                    var faction = MyFactions.GetFactionBySector(sector);

                    var objectPos = pos.Value;
                    if (objects3d.Any())
                    {
                        var firstPos = objects3d.First().PositionAndOrientation.Position;
                        var offset = objectPos - firstPos;

                        foreach (var o in objects3d)
                        {
                            // Clone
                            var clone = o.Clone() as MyMwcObjectBuilder_Object3dBase;
                            clone.PositionAndOrientation.Position += offset;
                            clone.ClearEntityId();
                            if (clone is MyMwcObjectBuilder_PrefabContainer)
                            {
                                ((MyMwcObjectBuilder_PrefabContainer)clone).Faction = faction;
                            }
                            if (clone is MyMwcObjectBuilder_SpawnPoint)
                            {
                                ((MyMwcObjectBuilder_SpawnPoint)clone).Faction = faction;
                            }
                            addToList.Add(clone);
                        }
                    }
                    else if(prefabs.Any())
                    {
                        MyMwcObjectBuilder_PrefabContainer container = new MyMwcObjectBuilder_PrefabContainer(null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE,
                            prefabs.ToList(), 0, faction, null);

                        var clone = container.Clone() as MyMwcObjectBuilder_PrefabContainer; // To clone children easily
                        clone.ClearEntityId(); // Clear childs ids
                        clone.PositionAndOrientation = new MyMwcPositionAndOrientation(objectPos, Vector3.Forward, Vector3.Up);
                        addToList.Add(clone);
                    }
                } //end of station generation

                if (pos.HasValue && rnd.Float(0, 1) < 0.5f)
                { //Create mysterious cube at 1% of stations
                    var sizeMyst = size * 1.5f;
                    Vector3? posMyst = FindEntityPosition(prunningStructure, rnd, sizeMyst, 1.0f, 1.0f, pos.Value);
                    if (posMyst.HasValue)
                    {
                        CreateMysteriousCubes(posMyst.Value, addToList, rnd);
                    }

                    //Create some more
                    int count = rnd.Next(5);
                    for (int i = 0; i < count; i++)
                    {
                        var size2 = MyMwcSectorConstants.SECTOR_SIZE / 2;
                        Vector3? pos2 = FindEntityPosition(prunningStructure, rnd, size2);
                        if (pos2.HasValue)
                        {
                            CreateMysteriousCubes(pos2.Value, addToList, rnd);
                        }
                    }
                }
            }
        }


        private void CreateMysteriousCubes(Vector3 posMyst, List<MyMwcObjectBuilder_Base> addToList, Random rnd)
        {
            int maxCubesInGroup = rnd.Next(5);
            float offset = 0;

            while (maxCubesInGroup > 0)
            {
                maxCubesInGroup--;

                CreateMysteriousCube(posMyst + Vector3.Right * offset, addToList, rnd);

                offset += 5;
            }
        }

        private void CreateMysteriousCube(Vector3 posMyst, List<MyMwcObjectBuilder_Base> addToList, Random rnd)
        {
            MyMwcObjectBuilder_MysteriousCube mysteriousCube = new MyMwcObjectBuilder_MysteriousCube(
            new MyMwcPositionAndOrientation(posMyst, Vector3.Forward, Vector3.Up)
            );

            MyMwcObjectBuilder_MysteriousCube_TypesEnum cubeType = (MyMwcObjectBuilder_MysteriousCube_TypesEnum)rnd.Item(Enum.GetValues(typeof(MyMwcObjectBuilder_MysteriousCube_TypesEnum)));
            mysteriousCube.MysteriousCubeType = cubeType;
            addToList.Add(mysteriousCube);
        }


        private void GenerateBots(MySolarSystemMapSectorData sectorData, List<MyMwcObjectBuilder_Base> addToList, Random rnd, MyDynamicAABBTree prunningStructure)
        {
            int spawnPt = (int)(rnd.Float(0, 50) /** sectorData.AreaInfluenceMultiplier*/  );
            float radius = 100;
            for (int i = 0; i < spawnPt; i++)
            {
                Vector3? pos = FindEntityPosition(prunningStructure, rnd, radius, 0.8f, 0.8f);

                if (pos.HasValue)
                {
                    //MyMwcObjectBuilder_FactionEnum faction = MyMwcObjectBuilder_FactionEnum.None;
                    var faction = MyFactions.GetFactionBySector(sectorData.SectorPosition);

                    //var builder = new MyMwcObjectBuilder_;
                    MyMwcObjectBuilder_SpawnPoint spobj = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SpawnPoint, null) as MyMwcObjectBuilder_SpawnPoint;
                    spobj.BoundingRadius = radius;
                    spobj.RespawnTimer = 1;
                    spobj.SpawnCount = 5;
                    spobj.PositionAndOrientation.Position = pos.Value;
                    spobj.PositionAndOrientation.Forward = Vector3.Forward;
                    spobj.PositionAndOrientation.Up = Vector3.Up;
                    spobj.Faction = faction;

                    List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
                    List<MyMwcObjectBuilder_SmallShip_Ammo> ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
                    List<MyMwcObjectBuilder_AssignmentOfAmmo> assignments = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();

                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));
                    assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
                    assignments.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));

                    List<MyMwcObjectBuilder_InventoryItem> inventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 1000f));
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic), 25));                

                    //spobj.m_shipTemplates.Add(
                    int numships = rnd.Next(1, 10);
                    for (int x = 0; x < numships; x++)
                    {
                        spobj.ShipTemplates.Add(
                            //new MyMwcObjectBuilder_SmallShip_Bot(
                            //    MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                            //    weapons,
                            //    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
                            //    ammo,
                            //    assignments,
                            //    null,
                            //    faction
                            //    ) as MyMwcObjectBuilder_SmallShip_Bot
                            new MyMwcObjectBuilder_SmallShip_Bot(
                                MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                                new MyMwcObjectBuilder_Inventory(inventoryItems, 32),
                                weapons,
                                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1),
                                assignments,
                                null,
                                null,
                                null,
                                MyGameplayConstants.HEALTH_RATIO_MAX,
                                100f,
                                float.MaxValue,
                                float.MaxValue,
                                true,
                                false,
                                faction,
                                MyAITemplateEnum.DEFAULT,
                                0,
                                1000,
                                1000,
                                MyPatrolMode.CYCLE,
                                null,
                                BotBehaviorType.IDLE,
                                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE,
                                0, false, true) as MyMwcObjectBuilder_SmallShip_Bot
                            );
                    }

                    addToList.Add(spobj);

                    /*
                    MySolarSystemMapEntity entity = new MySolarSystemMapEntity(sectorData.SectorPosition, pos, size, "Asteroid", MySolarSystemEntityEnum.Asteroid);
                    sectorData.Entities.Add(entity);
                    */
                }
            }
        }

        public static MyMwcObjectBuilder_Sector GetSectorBuilder(MySolarSystemMapSectorData sectorData)
        {
            MyMwcObjectBuilder_Sector sectorBuilder;
            sectorBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Sector, null) as MyMwcObjectBuilder_Sector;
            sectorBuilder.AreaTemplate = sectorData.Area;
            sectorBuilder.AreaMultiplier = sectorData.AreaInfluenceMultiplier;
            return sectorBuilder;
        }

        private bool IsEntityCollideWithSafeAreas(Vector3 entityPosition, float radius)
        {
            foreach (BoundingSphere boundingSphere in m_safeAreas)
            {
                if ((boundingSphere.Center - entityPosition).Length() <= boundingSphere.Radius + radius)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddSpawnPoint(MySolarSystemMapSectorData sectorData, float entityMinimalSize)
        {
            Random random = new Random(sectorData.SectorPosition.X ^ sectorData.SectorPosition.Y ^ sectorData.SectorPosition.Z ^ m_seed);
        }

        private MySectorObjectCounts ApplyAreaInfluence(MySectorObjectCounts objectCounts, float areaInfluence)
        {
            MySectorObjectCounts result = new MySectorObjectCounts();
            foreach (var item in objectCounts.Values)
            {
                result.Values[item.Key] = item.Value * areaInfluence;
            }
            return result;
        }

        private void EnsureMinimalAsteroidCounts(MySectorObjectCounts asteroidCounts)
        {
            foreach (var pair in MySolarSystemConstants.MinimumObjectsProperties.Values)
            {
                float value;
                if (!asteroidCounts.Values.TryGetValue(pair.Key, out value) || value < pair.Value)
                {
                    asteroidCounts.Values[pair.Key] = pair.Value;
                }
            }
        }

        private static int GetSeed(MyMwcVector3Int sectorPosition, int globalSeed)
        {
            return sectorPosition.X ^ sectorPosition.Y ^ sectorPosition.Z ^ globalSeed;
        }

        public static bool IsOutsideSector(Vector3 position, float radius)
        {
            BoundingSphere bs = new BoundingSphere(position, radius);
            if (MyMwcSectorConstants.SECTOR_SIZE_BOUNDING_BOX.Contains(bs) == ContainmentType.Disjoint)
            {
                return true;
            }

            return false;
        }

        private void AddSectorEntities(MySectorObjectCounts asteroidCounts, MyMwcVector3Int sectorPosition, Random random, float entityMinimalSize, int maxEntityCount,  List<MySolarSystemMapEntity> entities, bool onlyStaticAsteroids)
        {
                      
            // Space around asteroid should be at least 1.2x - 2x it's size
            float asteroidSpacingCoeficient = 0.7f;

            // Asteroid count mean is 40%
            float asteroidCountMean = 0.4f;

            Dictionary<int, int> entityCounts = new Dictionary<int, int>();
            foreach (MySolarSystemEntityEnum t in Enum.GetValues(typeof(MySolarSystemEntityEnum)))
            {
                entityCounts.Add((int)t, 0);
            }

            MyDynamicAABBTree prunningStructure = new MyDynamicAABBTree(Vector3.Zero);

            foreach (BoundingSphere boundingSphere in m_safeAreas)
            {
                BoundingBox bb = BoundingBox.CreateFromSphere(boundingSphere);
                prunningStructure.AddProxy(ref bb, new Render.MyRenderObject(null, null), 0);
            }


            // Generate asteroids, check collisions (order asteroids by size)
            //var asteroids = GetAsteroids(asteroidCounts, entityMinimalSize);
            var asteroids = GetAsteroids(asteroidCounts, entityMinimalSize);

            foreach (var info in asteroids)
            {
                if (info.EntityType != MySolarSystemEntityEnum.StaticAsteroid && onlyStaticAsteroids)
                    continue;

                float radius = info.SizeInMeters / 2;
                float count = info.ObjectCount;
                float positionOffset = 1.3f;
                count = (float)Math.Round(count * random.Float(1 - asteroidCountMean, 1 + asteroidCountMean));

                if (info.EntityType == MySolarSystemEntityEnum.VoxelAsteroid)
                {
                    positionOffset = 0.6f; //generate voxels more in center
                }

                while (entityCounts[(int)info.EntityType] < count && entityCounts[(int)info.EntityType] < maxEntityCount)
                {
                    Vector3? pos = FindEntityPosition(prunningStructure, random, radius, positionOffset, asteroidSpacingCoeficient);

                    if (pos.HasValue)
                    {
                        MySolarSystemMapEntity entity = new MySolarSystemMapEntity(sectorPosition, pos.Value, info.SizeInMeters, info.EntityType.ToString(), info.EntityType);
                        entities.Add(entity);

                        if (!MySectorGenerator.IsOutsideSector(pos.Value, radius))
                        {
                            entityCounts[(int)info.EntityType]++;
                        }
                        
                        BoundingBox bb = new BoundingBox(pos.Value - new Vector3(radius), pos.Value + new Vector3(radius));
                        prunningStructure.AddProxy(ref bb, new Render.MyRenderObject(null, null), 0);
                    }
                    else
                        entityCounts[(int)info.EntityType]++;
                }
            }
        }

        private static IEnumerable<MyObjectInfo> GetAsteroids(MySectorObjectCounts asteroidCounts, float entityMinimalSize)
        {
            var asteroids = asteroidCounts.AllObjects.Where(s => s.SizeInMeters > entityMinimalSize);
            return asteroids;
        }

        private void AddSectorEntities(MySolarSystemMapSectorData sectorData, float entityMinimalSize, int maxEntityCount, bool onlyStaticAsteroids)
        {
            Random random = new Random(GetSeed(sectorData.SectorPosition, m_seed));

            MySectorObjectCounts asteroidCounts;
            if (sectorData.Area.HasValue)
            {
                var area = MySolarSystemConstants.Areas[sectorData.Area.Value];

                asteroidCounts = ApplyAreaInfluence(area.SectorData.SectorObjectsCounts, sectorData.AreaInfluenceMultiplier);
            }
            else
            {
                asteroidCounts = MySolarSystemConstants.DefaultObjectsProperties;
            }

            EnsureMinimalAsteroidCounts(asteroidCounts);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddSectorEntities");
            AddSectorEntities(asteroidCounts, sectorData.SectorPosition, random,  entityMinimalSize, maxEntityCount, sectorData.Entities, onlyStaticAsteroids);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static MyMwcObjectBuilder_StaticAsteroid GenerateStaticAsteroid(float sizeInMeters, MyStaticAsteroidTypeSetEnum typeSet, MyMwcVoxelMaterialsEnum material, Vector3 position, Random rnd, List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroids)
        {
            int size = FindAsteroidSize(sizeInMeters, MyMwcObjectBuilder_StaticAsteroid.AsteroidSizes);
            asteroids.Clear();
            MyMwcObjectBuilder_StaticAsteroid.GetAsteroids(size, typeSet, asteroids);
            int rndIndex = rnd.Next(0, asteroids.Count);
            
            var builder = new MyMwcObjectBuilder_StaticAsteroid(asteroids[rndIndex], material);
            builder.PositionAndOrientation.Position = position;
            builder.PositionAndOrientation.Forward = rnd.Vector(1);
            builder.PositionAndOrientation.Up = rnd.Vector(1);
            builder.IsDestructible = false;

            return builder;
        }

        private static MyMwcObjectBuilder_StaticAsteroid CloneStaticAsteroid(MyMwcObjectBuilder_StaticAsteroid source)
        {
            MyMwcObjectBuilder_StaticAsteroid clone = new MyMwcObjectBuilder_StaticAsteroid(source.AsteroidType, source.AsteroidMaterial, source.AsteroidMaterial1);
            clone.PositionAndOrientation = source.PositionAndOrientation;

            return clone;
        }

        private MyMwcObjectBuilder_VoxelMap GenerateVoxelMap(int sizeInVoxels, Vector3 positionInSector, Random rnd, List<MyMwcVoxelFilesEnum> voxelAsteroids, MyWeightDictionary<MyMwcVoxelMaterialsEnum> primaryMaterials, MyWeightDictionary<MyMwcVoxelMaterialsEnum> secondaryMaterials)
        {
            int sizeInMeters = sizeInVoxels;// (int)(sizeInVoxels * MyVoxelConstants.VOXEL_SIZE_IN_METRES);
            voxelAsteroids.Clear();
            MyVoxelMap.GetAsteroidsBySizeInMeters(sizeInMeters, voxelAsteroids, false);
            int rndIndex = rnd.Next(0, voxelAsteroids.Count);

            MyMwcVoxelMaterialsEnum mainMat = MyMwcVoxelMaterialsEnum.Stone_01;
            if (primaryMaterials.Count > 0)
            {
                primaryMaterials.GetRandomItem(rnd);
            }

            MyMwcObjectBuilder_VoxelMap builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.VoxelMap, null) as MyMwcObjectBuilder_VoxelMap;
            builder.VoxelFile = rnd.Item(voxelAsteroids);
            builder.VoxelMaterial = mainMat;
            builder.PositionAndOrientation = new MyMwcPositionAndOrientation(positionInSector, Vector3.Forward, Vector3.Up);

            AddMergeContent(rnd, voxelAsteroids, sizeInMeters, builder);
            AddVeins(secondaryMaterials, rnd, positionInSector, 2, sizeInMeters, builder, VeinAngleDeviation, MaxLevel, BaseSecondaryMaterialThickness);

            return builder;
        }

        public static int FindAsteroidSize(float radius, List<int> sizes)
        {
            for (int i = 0; i < sizes.Count; i++)
            {
                if (radius < sizes[i])
                {
                    int prev = i - 1;
                    if (prev >= 0)
                    {
                        if (sizes[i] - radius > radius - sizes[prev])
                        {
                            return sizes[prev];
                        }
                        else
                        {
                            return sizes[i];
                        }
                    }
                    else
                    {
                        return sizes[i];
                    }
                }
            }
            return sizes[sizes.Count - 1];
        }

        public static void AddMergeContent(Random rnd, List<MyMwcVoxelFilesEnum> voxelAsteroids, int voxelAsteroidSize, MyMwcObjectBuilder_VoxelMap builder)
        {
            if (voxelAsteroidSize > (int)(64 * MyVoxelConstants.VOXEL_SIZE_IN_METRES))
            {
                int mergeSize = voxelAsteroidSize / 2;
                int numMerges = rnd.Next(0, 3);
                //numMerges = 1;
                for (int i = 0; i < numMerges; i++)
                {
                    voxelAsteroids.Clear();

                    MyMwcVoxelMapMergeTypeEnum mergeType;
                    int maxPos = (int)(mergeSize / MyVoxelConstants.VOXEL_SIZE_IN_METRES) / 2;
                    int mergeTypeIndex = rnd.Next(0, 2);
                    //mergeTypeIndex = 0;
                    if (mergeTypeIndex == 0)
                    {
                        mergeType = MyMwcVoxelMapMergeTypeEnum.ADD;
                        MyVoxelMap.GetAsteroidsBySizeInMeters(mergeSize, voxelAsteroids, true);
                        maxPos /= 2;
                    }
                    else
                    {
                        // Subtract merge works with bigger asteroids
                        mergeType = MyMwcVoxelMapMergeTypeEnum.INVERSE_AND_SUBTRACT;
                        MyVoxelMap.GetAsteroidsBySizeInMeters(mergeSize, voxelAsteroids, false);
                        MyVoxelMap.GetAsteroidsBySizeInMeters(mergeSize * 2, voxelAsteroids, false);
                    }

                    int mergeIndex = rnd.Next(0, voxelAsteroids.Count);

                    MyMwcVector3Short offset = new MyMwcVector3Short((short)rnd.Next(0, maxPos), (short)rnd.Next(0, maxPos), (short)rnd.Next(0, maxPos));

                    builder.MergeContents.Add(new MyMwcObjectBuilder_VoxelMap_MergeContent(offset, voxelAsteroids[mergeIndex], mergeType));
                }
            }
        }

        public static void AddVeins(MyWeightDictionary<MyMwcVoxelMaterialsEnum> secondaryMaterials, Random rnd, Vector3 positionInSector, int veinCount, int voxelAsteroidSize, MyMwcObjectBuilder_VoxelMap builder, float veinAngleDev, int maxLevel, float baseThickness)
        {
            MyMwcVoxelMaterialsEnum material = MyMwcVoxelMaterialsEnum.Magnesium_01;

            if (secondaryMaterials.Count > 0)
            {
                material = secondaryMaterials.GetRandomItem(rnd);
            }

            for (int i = 0; i < veinCount; i++)
            {
                Vector3 position = positionInSector + new Vector3(voxelAsteroidSize / 2);
                position += rnd.Vector(voxelAsteroidSize / 3);
                AddVein(builder, baseThickness, position, rnd, material, veinAngleDev, maxLevel);
            }
        }

        public static void AddVein(MyMwcObjectBuilder_VoxelMap builder, float thickness, Vector3 position, Random rnd, MyMwcVoxelMaterialsEnum material, float veinAngleDev, int maxLevel, int level = 0)
        {
            int numSplits = 2;// rnd.Next(2, 4);

            List<Vector3> targets = new List<Vector3>(numSplits);

            float handDist = thickness * 1.5f;

            for (int iSplit = 0; iSplit < numSplits; iSplit++)
            {
                // Hands per line
                int numHands = rnd.Next(8, 12);

                Vector3 direction = rnd.Direction();
                Vector3 target = position;
                for (int i = 0; i < numHands; i++)
                {
                    direction = rnd.Direction(direction, veinAngleDev);
                    target += direction * handDist;

                    // TODO: uncomment this to see material better (shown as tunnels, not only as voxel material)
                    //var voxelHand = new MyMwcObjectBuilder_VoxelHand_Sphere(target, thickness - 1, MyMwcVoxelHandModeTypeEnum.SUBTRACT);
                    //voxelHand.VoxelHandMaterial = material;
                    //builder.VoxelHandShapes.Add(voxelHand);

                    var voxelHand2 = new MyMwcObjectBuilder_VoxelHand_Sphere(new MyMwcPositionAndOrientation(target, Vector3.Forward, Vector3.Up), thickness, MyMwcVoxelHandModeTypeEnum.SET_MATERIAL);
                    voxelHand2.VoxelHandMaterial = material;
                    builder.VoxelHandShapes.Add(voxelHand2);
                }
                targets.Add(target);
            }

            if (level < maxLevel)
            {
                foreach (var t in targets)
                {
                    AddVein(builder, thickness * 0.7f, t, rnd, material, veinAngleDev, maxLevel, level + 1);
                }
            }
        }


    }
}

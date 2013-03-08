using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using Microsoft.Xna.Framework;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.SolarSystem
{
    /*
   



    // Generates sector data based on inserted values, ignores solar areas
    class MyStaticSectorGenerator
    {
        /// <summary>
        /// Deviation of vein in every step.
        /// Higher value means vein will be more curved.
        /// </summary>
        public const float VeinAngleDeviation = MathHelper.Pi / 8; // 22.5 degrees

        /// <summary>
        /// Maximum level of vein subdivision
        /// </summary>
        public const int MaxLevel = 1;

        /// <summary>
        /// Base thickness of secondary material vein.
        /// </summary>
        public const float BaseSecondaryMaterialThickness = 40;

        /// <summary>
        /// Defines how many times we try find not colliding position for entity
        /// </summary>
        public const int MaxCollisionsTestsForEntity = 20;

        Random m_rnd;
        
        List<Tuple<float, Vector3>> m_entities = new List<Tuple<float, Vector3>>();

        static List<int> m_sizes;
        

        List<MyMwcObjectBuilder_Base> m_sectorObjects = new List<MyMwcObjectBuilder_Base>();

        

        static MyStaticSectorGenerator()
        {
            m_sizes = new List<int>(15);
            //m_asteroids = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>(5);
            MyMwcObjectBuilder_StaticAsteroid.GetAsteroidSizes(m_sizes);
        }

        public MyStaticSectorGenerator(int seed)
        {
            m_rnd = new Random(seed);
        }
                        
        public MyMwcObjectBuilder_Sector GenerateObjectBuilders(MySectorObjectCounts sectorObjects)
        {
            m_sectorObjects.Clear();
            m_entities.Clear();

            // Large objects first
            for (int i = 0; i < sectorObjects.Voxels512; i++) AddVoxelAsteroid(512);
            for (int i = 0; i < sectorObjects.StaticAsteroidLarge; i++) AddStaticAsteroid(m_rnd.Next(2000, 11000));
            for (int i = 0; i < sectorObjects.Voxels256; i++) AddVoxelAsteroid(256);
            for (int i = 0; i < sectorObjects.StaticAsteroidMedium; i++) AddStaticAsteroid(m_rnd.Next(100, 1100));
            for (int i = 0; i < sectorObjects.Voxels128; i++) AddVoxelAsteroid(128);
            for (int i = 0; i < sectorObjects.Voxels64; i++) AddVoxelAsteroid(64);
            for (int i = 0; i < sectorObjects.StaticAsteroidSmall; i++) AddStaticAsteroid(m_rnd.Next(10, 60));

            for (int i = 0; i < sectorObjects.Motherships; i++) AddMothership();
            for (int i = 0; i < sectorObjects.StaticDebrisFields; i++) AddDebrisField();

            return new MyMwcObjectBuilder_Sector()
            {
                SectorObjects = m_sectorObjects,
            };
        }

                          
        public MyMwcObjectBuilder_Sector GenerateEntities(MySectorObjectCounts sectorObjects, List<MySolarSystemMapEntity> entities)
        {
            m_sectorObjects.Clear();
            m_entities.Clear();

            // Large objects first
            for (int i = 0; i < sectorObjects.Voxels512; i++) AddVoxelAsteroid(512);
            for (int i = 0; i < sectorObjects.StaticAsteroidLarge; i++) AddStaticAsteroid(m_rnd.Next(2000, 11000));
            for (int i = 0; i < sectorObjects.Voxels256; i++) AddVoxelAsteroid(256);
            for (int i = 0; i < sectorObjects.StaticAsteroidMedium; i++) AddStaticAsteroid(m_rnd.Next(100, 1100));
            for (int i = 0; i < sectorObjects.Voxels128; i++) AddVoxelAsteroid(128);
            for (int i = 0; i < sectorObjects.Voxels64; i++) AddVoxelAsteroid(64);
            for (int i = 0; i < sectorObjects.StaticAsteroidSmall; i++) AddStaticAsteroid(m_rnd.Next(10, 60));

            for (int i = 0; i < sectorObjects.Motherships; i++) AddMothership();
            for (int i = 0; i < sectorObjects.StaticDebrisFields; i++) AddDebrisField();

            return new MyMwcObjectBuilder_Sector()
            {
                SectorObjects = m_sectorObjects,
            };
        }         

        public Vector3? FindObjectPosition(int sizeInMeters)
        {
            Vector3 halfSize = new Vector3(MyMwcSectorConstants.SECTOR_SIZE / 2.0f - sizeInMeters);
            bool collide = true;
            int tryCount = 0;
            Vector3 positionInSector = new Vector3();
            while (collide && tryCount < MaxCollisionsTestsForEntity)
            {
                positionInSector = m_rnd.Vector(halfSize);
                collide = Collide(positionInSector, sizeInMeters);
                tryCount++;
            }
            return collide ? null : (Vector3?)positionInSector;
        }

        private void AddMothership()
        {
            const int size = 500;
            var pos = FindObjectPosition(size);
            if (pos.HasValue)
            {
                var shipType = m_rnd.Enum<MyMwcObjectBuilder_PrefabLargeShip_TypesEnum>();
                MyMwcObjectBuilder_Prefab_AppearanceEnum appearance = m_rnd.Enum<MyMwcObjectBuilder_Prefab_AppearanceEnum>();

                var ship = new MyMwcObjectBuilder_PrefabLargeShip(shipType, appearance, new MyMwcVector3Short(0, 0, 0), m_rnd.Vector(1), MyGameplayConstants.MAX_HEALTH_MAX, m_rnd.FloatNormal() * MyGameplayConstants.MAX_HEALTH_MAX, "Abandoned large ship", 0);
                var prefabs = new List<MyMwcObjectBuilder_PrefabBase>();
                prefabs.Add(ship);
                var container = new MyMwcObjectBuilder_PrefabContainer(0, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, prefabs, 0, m_rnd.Enum<MyMwcObjectBuilder_FactionEnum>(), null);
                container.PositionAndOrientation = new MyMwcPositionAndOrientation(pos.Value, Vector3.Forward, Vector3.Up);
                m_sectorObjects.Add(container);
                m_entities.Add(new Tuple<float, Vector3>(size, pos.Value));
            }


            //MyMwcObjectBuilder_PrefabLargeShip builder = new MyMwcObjectBuilder_PrefabLargeShip(MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP, MyMwcObjectBuilder_Prefab_AppearanceEnum.
            //MyMwcObjectBuilder_PrefabContainer container = new MyMwcObjectBuilder_PrefabContainer(0, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, 

            //MyMwcObjectBuilder_LargeShip objectBuilder = new MyMwcObjectBuilder_LargeShip();
            //objectBuilder.ShipType = GetRandomLargeShipType();
        }

        private void AddDebrisField()
        {
            const int size = 50;
            var pos = FindObjectPosition(size);
            if (pos.HasValue)
            {
                MyMwcObjectBuilder_LargeDebrisField objectBuilder = new MyMwcObjectBuilder_LargeDebrisField(MyMwcObjectBuilder_LargeDebrisField_TypesEnum.Debris84);
                objectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(pos.Value, m_rnd.Vector(1), m_rnd.Vector(1));

                m_sectorObjects.Add(objectBuilder);
                m_entities.Add(new Tuple<float, Vector3>(size, pos.Value));
            }
        }

        public bool Collide( Vector3 position, float sizeInMeters, float spacing = 1.0f)
        {
            foreach (var e in m_entities)
            {
                if ((e.Item2 - position).Length() < (e.Item1 + sizeInMeters) * spacing)
                {
                    return true;
                }
            }
            return false;
        }

   
    }
*/
}

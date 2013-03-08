#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Gameplay;
using System.Threading.Tasks;
using System.Threading;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.Weapons;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.SubObjects;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using MinerWars.AppCode.Game.TransparentGeometry;

#endregion

namespace MinerWars.AppCode.Game.Radar
{

    static class MyRadar
    {
        public struct MySharedHashSet : IDisposable
        {
            FastResourceLock m_lockObject;
            HashSet<IMyObjectToDetect> m_hashSet;

            public MySharedHashSet(FastResourceLock lockObject, HashSet<IMyObjectToDetect> hashSet)
            {
                m_lockObject = lockObject;
                m_hashSet = hashSet;
                m_lockObject.AcquireShared();
            }

            public void Dispose()
            {
                m_lockObject.ReleaseShared();
            }

            public HashSet<IMyObjectToDetect> Collection
            {
                get { return m_hashSet; }
            }
        }

        private static bool m_isInRadarJammerRange;
        private static bool m_isInSunWindRange;
        private static bool m_isNearSunWindRange;

        private static bool m_clearDetectedObjects;

        private static List<IMyObjectToDetect> m_objectsToDetect;
        private static List<IMyObjectToDetect> m_detectedObjectsHelperCollection;

        private static HashSet<IMyObjectToDetect> m_detectedObjects;
        private static HashSet<IMyObjectToDetect> m_detectedBotsAndLargeWeapons;

        private static List<MyRBElement> m_objectsInDetectingArea;

        public static bool On
        {
            get
            {
                return MyGuiScreenGamePlay.Static != null &&
                       MyGuiScreenGamePlay.Static.IsGameActive() &&
                       MyGuiScreenGamePlay.Static.ControlledEntity != null &&
                       !MyGuiScreenGamePlay.Static.ControlledEntity.IsDead() &&
                       MyGuiScreenGamePlay.Static.ControlledEntity.Activated;
            }
        }

        private static float DefaultRadarRange = MySmallShipConstants.MAX_HUD_DISTANCE;

        private static readonly AutoResetEvent m_updateRadarEvent;
        private static volatile bool m_updateCompleted = true;

        private static FastResourceLock m_detectedObjectsLock = new FastResourceLock();

        private static bool Multithreaded = true;

        static MyRadar()
        {
            m_objectsToDetect = new List<IMyObjectToDetect>();

            m_detectedObjectsHelperCollection = new List<IMyObjectToDetect>();
            m_detectedObjects = new HashSet<IMyObjectToDetect>();
            m_detectedBotsAndLargeWeapons = new HashSet<IMyObjectToDetect>();
            m_objectsInDetectingArea = new List<MyRBElement>();

            MyEntities.OnEntityRemove += new Action<MyEntity>(MyEntities_OnEntityRemove);
            MyVoxelMaps.OnRemoveOreCell += new Action<MyVoxelMapOreDepositCell>(MyVoxelMaps_OnRemoveOreCell);

#if RENDER_PROFILING
            Multithreaded = false;
#endif

            if (Multithreaded)
            {
                m_updateRadarEvent = new AutoResetEvent(false);
                Task.Factory.StartNew(UpdateInBackground, TaskCreationOptions.PreferFairness);
            }
        }

        static void MyVoxelMaps_OnRemoveOreCell(MyVoxelMapOreDepositCell cell)
        {
            RemoveClosedObject(cell);
        }

        public static void Update()
        {
            if (!Multithreaded)
            {
                ClearCollections();
                if (!On)
                {
                    return;
                }

                Detect();
            }
        }


        static void MyEntities_OnEntityRemove(MyEntity obj)
        {
            RemoveClosedObject(obj);
        }

        private static void RemoveClosedObject(IMyObjectToDetect obj)
        {
            using (m_detectedObjectsLock.AcquireExclusiveUsing())
            {
                m_detectedObjects.Remove(obj);
                m_detectedBotsAndLargeWeapons.Remove(obj);
                m_detectedObjectsHelperCollection.Remove(obj);
                m_objectsToDetect.Remove(obj);
            }
        }

        private static void UpdateInBackground()
        {
            while (true)
            {
                Debug.Assert(m_updateCompleted);
                ClearCollections();

                //m_updateRadarEvent.WaitOne();
                m_updateRadarEvent.WaitOne(300);

                if (!On)
                {
                    continue;
                }

                Detect();
            }
        }

        private static void ClearCollections()
        {
            m_objectsInDetectingArea.Clear();

            if (m_clearDetectedObjects)
            {
                using (m_detectedObjectsLock.AcquireExclusiveUsing())
                {
                    m_detectedObjects.Clear();
                    m_detectedBotsAndLargeWeapons.Clear();
                }
                m_clearDetectedObjects = false;
            }
        }

        public static bool IsNearRadarJammerOrSunWind()
        {
            return IsNearSunWind() || IsNearRadarJammer();
        }

        public static bool IsNearSunWind()
        {
            return m_isNearSunWindRange;
        }

        public static bool IsNearRadarJammer()
        {
            return m_isInRadarJammerRange;
        }

        public static bool IsInSunWind()
        {
            return m_isInSunWindRange;
        }

        static MyEntity GetOwner()
        {
            return MyGuiScreenGamePlay.Static.ControlledEntity;
        }

        static Vector3 GetRadarPosition()
        {
            if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.ControlledEntity != null)
            {
                return MyGuiScreenGamePlay.Static.ControlledEntity.WorldVolume.Center;
            }
            return Vector3.Zero;
        }

        static bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            if (objectToDetect is MyVoxelMapOreDepositCell)
            {
                MyVoxelMapOreDepositCell oreDeposit = objectToDetect as MyVoxelMapOreDepositCell;
                return oreDeposit.GetTotalRareOreContent() > 0;
            }
            //return objectToDetect is MyEntity || objectToDetect is MyMwcVoxelMaterialsEnum;
            return objectToDetect is MyShip ||
                   objectToDetect is MyPrefabFoundationFactory ||
                   objectToDetect is MyPrefabHangar ||
                   objectToDetect is MyPrefabLargeWeapon ||
                   objectToDetect is MyAmmoBase /*||
                   objectToDetect is MyVoxelMapOreDepositCell*/;
        }

        private static void Detect()
        {
            m_updateCompleted = false;
            if (MySunWind.IsActive)
            {
                float distanceFromSunWind = Vector3.Distance(MySunWind.Position, MyGuiScreenGamePlay.Static.ControlledEntity.WorldVolume.Center);
                m_isInSunWindRange = distanceFromSunWind <= MyHudConstants.RADAR_JAM_FROM_SUN_WIND_RADIUS;
                m_isNearSunWindRange = distanceFromSunWind >= MyHudConstants.RADAR_JAM_FROM_SUN_WIND_RADIUS &&
                                       distanceFromSunWind <= MyHudConstants.RADAR_JAM_FROM_SUN_WIND_RADIUS +
                                                              MyHudConstants.RADAR_BLINKING_RANGE;
            }
            else
            {
                m_isInSunWindRange = false;
                m_isNearSunWindRange = false;
            }

            // get objects to detect and detect if we are in radar jammer's range                        
            GetObjectsToDetect();

            // check criterias for objects in detected range
            GetDetectedObjects();

            // fill detected objects collections from helper collection
            UpdateDetectedObjects();
            m_updateCompleted = true;
        }

        private static void UpdateDetectedObjects()
        {
            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {
                using (m_detectedObjectsLock.AcquireExclusiveUsing())
                {
                    m_detectedObjects.Clear();
                    m_detectedBotsAndLargeWeapons.Clear();

                    foreach (IMyObjectToDetect detectedObject in m_detectedObjectsHelperCollection)
                    {
                        m_detectedObjects.Add(detectedObject);
                        if (detectedObject is MySmallShipBot || detectedObject is MyPrefabLargeWeapon)
                        {
                            m_detectedBotsAndLargeWeapons.Add(detectedObject);
                        }
                    }

                    m_detectedObjectsHelperCollection.Clear();
                }
            }
        }

        private static void GetDetectedObjects()
        {
            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {
                using (m_detectedObjectsLock.AcquireSharedUsing())
                {
                    Debug.Assert(m_detectedObjectsHelperCollection.Count == 0);
                    foreach (IMyObjectToDetect obj in m_objectsToDetect)
                    {

                        // if we are in radar jammer's range or in sun wind jam range, then we detect only friends
                        if (IsNearRadarJammerOrSunWind() && obj is MyEntity && (MyFactions.GetFactionsRelation(GetOwner().Faction, ((MyEntity)obj).Faction) == MyFactionRelationEnum.Enemy))
                        {
                            continue;
                        }

                        //activeDetector.IsDetected(GetRadarPosition(), objectToDetect)
                        if (!CanBeDetected(obj))
                        {
                            continue;
                        }

                        BoundingSphere detectingSphere = new BoundingSphere(GetOwner().GetPosition(), DefaultRadarRange);
                        if (!detectingSphere.Intersects(obj.WorldAABB))
                        {
                            continue;
                        }

                        m_detectedObjectsHelperCollection.Add(obj);
                    }
                    m_objectsToDetect.Clear();
                }
            }
        }

        private static void GetObjectsToDetect()
        {
            m_isInRadarJammerRange = false;
            Debug.Assert(m_objectsToDetect.Count == 0);

            if (MyGuiScreenGamePlay.Static == null)
                return;
            if (MyGuiScreenGamePlay.Static.ControlledEntity == null)
                return;

            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {
                // find all objects in detecting area                
                using (MyEntities.UnloadDataLock.AcquireSharedUsing())
                {
                    if (MyEntities.IsLoaded)
                    {
                        FindObjectsInDetectingArea();
                    }
                }

                GetObjectsToDetectInDetectingArea();
            }
        }

        private static void GetObjectsToDetectInDetectingArea()
        {
            foreach (MyRBElement rbElement in m_objectsInDetectingArea)
            {
                if (rbElement.GetRigidBody() == null)
                    continue;

                if (rbElement.GetRigidBody().m_UserData == null)
                    continue;

                MyEntity physObject = ((MyPhysicsBody)rbElement.GetRigidBody().m_UserData).Entity;
                if (physObject == null)
                    continue;

                // try detect if we are in radar jammer's range
                MySmallShip smallShip = physObject as MySmallShip;
                if (smallShip != null)
                {
                    if (smallShip.IsEngineTurnedOff())
                        continue;

                    if (smallShip.HasRadarJammerActive())
                    {
                        float distanceFromRadarJammer = Vector3.Distance(GetRadarPosition(), smallShip.WorldVolume.Center);
                        if (distanceFromRadarJammer <= MyHudConstants.RADAR_JAMMER_RANGE)
                        {
                            m_isInRadarJammerRange = true;
                        }
                    }
                }

                if (MyGuiScreenGamePlay.Static != null &&
                    MyGuiScreenGamePlay.Static.ControlledEntity == physObject)
                {
                    continue;
                }

                // because ore deposit is not a entity, so we must use this
                if (physObject is MyVoxelMap)
                {
                    MyVoxelMap voxelMap = physObject as MyVoxelMap;
                    using (voxelMap.OreDepositsLock.AcquireSharedUsing())
                    {
                        foreach (MyVoxelMapOreDepositCell oreDeposit in voxelMap.OreDepositCellsContainsOre)
                        {
                            Debug.Assert(oreDeposit != null);
                            m_objectsToDetect.Add(oreDeposit);
                        }
                    }
                }
                else
                {
                    Debug.Assert(physObject != null);
                    m_objectsToDetect.Add(physObject);
                }
            }
        }

        private static void FindObjectsInDetectingArea()
        {
            // find max detector's range
            float range = Math.Max(DefaultRadarRange, MyHudConstants.RADAR_JAMMER_RANGE);
            Debug.Assert(m_objectsInDetectingArea.Count == 0);
            BoundingSphere sphereToDetecting = new BoundingSphere(GetRadarPosition(), range);
            BoundingBox radarBoundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphereToDetecting, ref radarBoundingBox);
            MyEntities.GetElementsInBox(ref radarBoundingBox, m_objectsInDetectingArea);
        }


        private static bool CanSeeAll()
        {
            return
                MyGuiScreenGamePlay.Static != null &&
                MyGuiScreenGamePlay.Static.IsGameActive() &&
                MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.SEE_ALL);
        }

        internal static void UnloadData()
        {
            m_clearDetectedObjects = true;
        }

        public static void GetDetectedObjects(ref List<IMyObjectToDetect> detectedObjects)
        {
            detectedObjects.Clear();
            using (m_detectedObjectsLock.AcquireSharedUsing())
            {
                foreach (var detectedObject in m_detectedObjects)
                    detectedObjects.Add(detectedObject);
            }
        }

        public static void GetDetectedObjects(ref HashSet<IMyObjectToDetect> detectedObjects)
        {
            detectedObjects.Clear();
            using (m_detectedObjectsLock.AcquireSharedUsing())
            {
                foreach (var detectedObject in m_detectedObjects)
                    detectedObjects.Add(detectedObject);
            }
        }

        public static void GetDetectedBotsAndLargeWeapons(ref List<IMyObjectToDetect> detectedObjects)
        {
            detectedObjects.Clear();
            using (m_detectedObjectsLock.AcquireSharedUsing())
            {
                foreach (var detectedObject in m_detectedBotsAndLargeWeapons)
                    detectedObjects.Add(detectedObject);
            }
        }

        public static MySharedHashSet DetectedObjects
        {
            get { return new MySharedHashSet(m_detectedObjectsLock, m_detectedObjects); }
        }
    }
}

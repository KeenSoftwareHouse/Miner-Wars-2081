using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Physics;



using System.Linq;

//  Here we store JLX phys objects. Not normal missiles and not projectiles.
//
//  This class is just holder of the list, so it doesn't do any real logic
//  E.g. when you add or remove object from here, don't expect this object will
//  call Start or Close on it.s You must do it!!!

namespace MinerWars.AppCode.Game.Entities
{
    using App;
    using InfluenceSpheres;
    using VoxelHandShapes;
    using World;
    using Prefabs;
    using System.Diagnostics;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWars.AppCode.Game.Prefabs;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
    using MinerWars.AppCode.Game.Entities.SubObjects;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Localization;
    using MinerWars.AppCode.Game.TransparentGeometry.Particles;
    using MinerWars.AppCode.Game.Entities.CargoBox;
    using MinerWars.AppCode.Game.Explosions;
    using KeenSoftwareHouse.Library.Collections;
    using MinerWars.AppCode.Game.Missions;
    using KeenSoftwareHouse.Library.Parallelization.Threading;
    using KeenSoftwareHouse.Library.Trace;
    using MinerWars.AppCode.Game.HUD;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Physics.Collisions;
    using MinerWars.AppCode.Networking;
    using MinerWars.AppCode.Game.Managers;

    internal enum MyExplosionForceDirection
    {
        EXPLOSION = 1,      //  Throw objects from the epicentrum
        IMPLOSION = 2       //  Throw objects into the epicentrum
    }

    static class MyEntities
    {
        // List of physic objects in the world
        static HashSet<MyEntity> m_entities = new HashSet<MyEntity>();
        public static List<MyEntity> m_entitiesForUpdate = new List<MyEntity>();

        // Count of objects editable in editor
        static int m_editorObjectsCount = 0;
        readonly static int MAX_ENTITIES_CLOSE_PER_UPDATE = 10;

        // Event called when entity is removed from scene
        public static event Action<MyEntity> OnEntityRemove;
        public static event Action OnCloseAll;
        public static event Action<MyEntity, string, string> OnEntityNameSet;

        [ThreadStatic]
        static List<MyRBElement> m_overlapRBElementList;
        static List<List<MyRBElement>> m_overlapRBElementListCollection = new List<List<MyRBElement>>();

        static List<MyRBElement> OverlapRBElementList
        {
            get
            {
                if (m_overlapRBElementList == null)
                {
                    m_overlapRBElementList = new List<MyRBElement>(256);
                    lock (m_overlapRBElementListCollection)
                    {
                        m_overlapRBElementListCollection.Add(m_overlapRBElementList);
                    }
                }
                return m_overlapRBElementList;
            }
        }

        static List<MyRBElement> GetElementsInBox(MyDynamicAABBTree pruningStructure, ref BoundingBox boundingBox)
        {
            MyCommonDebugUtils.AssertDebug(OverlapRBElementList.Count == 0, "The result of GetElementsInBox() wasn't cleared after last use!");
            pruningStructure.OverlapAllBoundingBox(ref boundingBox, OverlapRBElementList, (uint)MyElementFlag.EF_RB_ELEMENT);
            return OverlapRBElementList;
        }

        /// <summary>
        /// Get all rigid body elements touching a bounding box.
        /// Clear() the result list after you're done with it!
        /// </summary>
        /// <returns>The list of results.</returns>
        public static List<MyRBElement> GetElementsInBox(ref BoundingBox boundingBox)
        {
            return GetElementsInBox(m_pruningStructure, ref boundingBox);
        }

        public static void GetElementsInBox(ref BoundingBox boundingBox, List<MyRBElement> foundElements)
        {
            m_pruningStructure.OverlapAllBoundingBox(ref boundingBox, foundElements, (uint)MyElementFlag.EF_RB_ELEMENT);
        }

        // Helper list for storing results of various operations, mostly used in intersections
        [ThreadStatic]
        private static HashSet<MyEntity> m_entityResultSet;
        private static List<HashSet<MyEntity>> m_entityResultSetCollection = new List<HashSet<MyEntity>>();
        static HashSet<MyEntity> EntityResultSet
        {
            get
            {
                if (m_entityResultSet == null)
                {
                    m_entityResultSet = new HashSet<MyEntity>();
                    lock (m_entityResultSetCollection)
                    {
                        m_entityResultSetCollection.Add(m_entityResultSet);
                    }
                }
                return m_entityResultSet;
            }
        }

        // Helper list for storing temporary entities
        [ThreadStatic]
        private static List<MyEntity> m_entityInputList;
        private static List<List<MyEntity>> m_entityInputListCollection = new List<List<MyEntity>>();
        static List<MyEntity> EntityInputList
        {
            get
            {
                if (m_entityInputList == null)
                {
                    m_entityInputList = new List<MyEntity>(32);
                    lock (m_entityInputListCollection)
                    {
                        m_entityInputListCollection.Add(m_entityInputList);
                    }
                }
                return m_entityInputList;
            }
        }

        [ThreadStatic]
        static List<MyLineSegmentOverlapResult<MyRBElement>> m_lineOverlapRBElementList;
        static List<List<MyLineSegmentOverlapResult<MyRBElement>>> m_lineOverlapRBElementListCollection = new List<List<MyLineSegmentOverlapResult<MyRBElement>>>();
        static List<MyLineSegmentOverlapResult<MyRBElement>> LineOverlapRBElementList
        {
            get
            {
                if (m_lineOverlapRBElementList == null)
                {
                    m_lineOverlapRBElementList = new List<MyLineSegmentOverlapResult<MyRBElement>>(256);
                    lock (m_lineOverlapRBElementListCollection)
                    {
                        m_lineOverlapRBElementListCollection.Add(m_lineOverlapRBElementList);
                    }
                }
                return m_lineOverlapRBElementList;
            }
        }

        //  For quick space-traversal
        static MyDynamicAABBTree m_pruningStructure;

        //  Sometimes we need to remove objects from "m_entities" while we iterate that list, so this is the helper
        static List<MyEntity> m_safeIterationHelper = new List<MyEntity>(MyConstants.MAX_ENTITIES);

        // Helper collection, entities are added with MarkForClose(entity), real remove is done with CloseRememberedEntities() which is last Update call
        static HashSet<MyEntity> m_entitiesToClose = new HashSet<MyEntity>();

        static HashSet<MyEntity> m_activateList = new HashSet<MyEntity>();
        static HashSet<MyEntity> m_deactivateList = new HashSet<MyEntity>();

        //preallocated list of entities fileld up/cleared when testing collisions with element
        static List<MyRBElement> m_CollisionsForElementsHelper = new List<MyRBElement>();

        static public List<MyRBElement> CollisionsElements
        {
            get { return m_CollisionsForElementsHelper; }
        }

        // Dictionary of entities where entity name is key
        static public Dictionary<string, MyEntity> m_entityNameDictionary = new Dictionary<string, MyEntity>();

        static bool m_isLoaded = false;
        public static bool IsLoaded
        {
            get { return m_isLoaded; }
        }

        //  Common functionality is provided by this class for phys objects, however, some of them are not JLX and we need to reuse that functionality
        //  and keep it at one place, for that reason, influence spheres are managed by this class.
        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEntities.LoadData");

            m_entities.Clear();
            m_safeIterationHelper.Clear();
            m_entitiesToClose.Clear();
            m_pruningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();
            MyCommonDebugUtils.AssertRelease(m_pruningStructure != null);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            m_isLoaded = true;
        }

        public static void UnloadData()
        {
            using (UnloadDataLock.AcquireExclusiveUsing())
            {
                MyPrefabContainerManager.GetInstance().Clear();
                CloseAll(true);

                System.Diagnostics.Debug.Assert(m_entities.Count == 0);
                System.Diagnostics.Debug.Assert(m_entitiesToClose.Count == 0);

                m_safeIterationHelper.Clear();
                m_pruningStructure = null;
                m_lineOverlapRBElementList = null;
                m_overlapRBElementList = null;
                m_entityResultSet = null;
                m_isLoaded = false;

                lock (m_lineOverlapRBElementListCollection)
                {
                    foreach (var item in m_lineOverlapRBElementListCollection)
                    {
                        item.Clear();
                    }
                }
                lock (m_entityInputListCollection)
                {
                    foreach (var item in m_entityInputListCollection)
                    {
                        item.Clear();
                    }
                }
                lock (m_overlapRBElementListCollection)
                {
                    foreach (var item in m_overlapRBElementListCollection)
                    {
                        item.Clear();
                    }
                }
                lock (m_entityResultSetCollection)
                {
                    foreach (var item in m_entityResultSetCollection)
                    {
                        item.Clear();
                    }
                }
                lock (m_allIgnoredEntitiesCollection)
                {
                    foreach (var item in m_allIgnoredEntitiesCollection)
                    {
                        item.Clear();
                    }
                }
            }
        }

        //  IMPORTANT: Only adds object to the list. Caller must call Start() or Init() on the object.
        public static void Add(MyEntity entity)
        {
            if (Exist(entity) == false)
            {
                if (entity is MyVoxelMap)
                {
                    MyVoxelMaps.Add((MyVoxelMap)entity);
                }

                m_entities.Add(entity);

                RegisterForUpdate(entity);

                if (entity.Flags.HasFlag(MyEntity.EntityFlags.EditableInEditor))
                    m_editorObjectsCount++;
            }

            entity.NotifyAddedToScene(null);
        }

        public static void SetEntityName(MyEntity myEntity, bool possibleRename = true)
        {
            string oldName = null;
            string newName = myEntity.Name;
            if (possibleRename)
            {
                foreach (var item in m_entityNameDictionary)
                {
                    if (item.Value == myEntity)
                    {
                        m_entityNameDictionary.Remove(item.Key);
                        oldName = item.Key;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(myEntity.Name))
            {
                Debug.Assert(!m_entityNameDictionary.ContainsKey(myEntity.Name));
                if (!m_entityNameDictionary.ContainsKey(myEntity.Name))
                {
                    m_entityNameDictionary.Add(myEntity.Name, myEntity);
                }
            }

            if (OnEntityNameSet != null)
            {
                OnEntityNameSet(myEntity, oldName, newName);
            }
        }

        public static bool IsNameExists(MyEntity entity, string name)
        {
            foreach (var item in m_entityNameDictionary)
            {
                if (item.Key == name && item.Value != entity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the specified entity from scene
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="skipIfNotExist">if set to <c>true</c> [skip if not exist].</param>
        public static void Remove(MyEntity entity)
        {
            System.Diagnostics.Debug.Assert(entity != null);

            if (entity is MyVoxelMap)
            {
                MyVoxelMaps.RemoveVoxelMap((MyVoxelMap)entity);
            }

            if (m_entities.Contains(entity))
            {
                m_entities.Remove(entity);
                if (entity.NeedsUpdate)
                    UnregisterForUpdate(entity);
                if (entity.Flags.HasFlag(MyEntity.EntityFlags.EditableInEditor))
                    m_editorObjectsCount--;
            }

            if (m_activateList.Contains(entity))
                m_activateList.Remove(entity);

            RaiseEntityRemove(entity);
            entity.NotifyRemovedFromScene(null);
        }


        public static FastResourceLock EntityCloseLock = new FastResourceLock();
        public static FastResourceLock EntityMarkForCloseLock = new FastResourceLock();
        public static FastResourceLock UnloadDataLock = new FastResourceLock();
        //public static object EntityCloseLock = new object();

        private static void CloseRememberedEntities(int countToClose = int.MaxValue)
        {
            CloseAllowed = true;

            while (m_entitiesToClose.Count > 0 && countToClose > 0)
            {
                EntityCloseLock.AcquireExclusive();
                m_entitiesToClose.FirstElement().Close();
                EntityCloseLock.ReleaseExclusive();
                countToClose--;
            }

            CloseAllowed = false;
        }

        public static void RemoveFromClosedEntities(MyEntity entity)
        {
            if (m_entitiesToClose.Count > 0)
            {
                m_entitiesToClose.Remove(entity);
            }

            m_activateList.Remove(entity);
            m_deactivateList.Remove(entity);
        }

        /// <summary>
        /// Remove name of entity from used names
        /// </summary>
        public static void RemoveName(MyEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.Name))
            {
                m_entityNameDictionary.Remove(entity.Name);
            }
        }

        /// <summary>
        /// Checks if entity exists in scene already
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Exist(MyEntity entity)
        {

            if (entity is MyVoxelMap)
            {
                return MyVoxelMaps.Exist((MyVoxelMap)entity);
            }

            if (entity is MyPrefabBase)
            {
                if ((entity as MyPrefabBase).GetOwner() != null)
                    return m_entities.Contains((entity as MyPrefabBase).GetOwner());
            }

            if (m_entities == null)
                return false;

            return m_entities.Contains(entity);
        }

        public static void MarkForClose(MyEntity entity)
        {
            Debug.Assert(!entity.Closed, "MarkForClose() is being called on close entity, use OnClose to prevent this call");
            //Debug.Assert(MyMinerGame.IsMainThread(), "MarkForClose() can be called only from main thread");

            if (CloseAllowed)
            {
                entity.Close();
                return;
            }

            if (!m_entitiesToClose.Contains(entity))
            {
                EntityMarkForCloseLock.AcquireExclusive();
                m_entitiesToClose.Add(entity);
                EntityMarkForCloseLock.ReleaseExclusive();
            }
            //else
            //{
            //    Debug.Fail("Entity marked more than once for close!");
            //}
        }

        //  Closes all objects - they are removed from within close method
        public static void CloseAll(bool deletePlayerShip)
        {
            if (OnCloseAll != null)
                OnCloseAll();

            CloseAllowed = true;

            foreach (MyEntity entity in m_entities.ToArray())
            {
                if (!deletePlayerShip && MySession.PlayerShip == entity)
                {
                    continue;
                }

                entity.Close();

                //This is already called from Entity.Close
                //MyEntities.Remove(entity, false);
            }

            foreach (MyEntity entity in m_entitiesToClose.ToArray())
            {
                entity.Close();
            }

            CloseAllowed = false;
            m_entitiesToClose.Clear();

            // Clear all
            MyEntityIdentifier.Clear();

            if (!deletePlayerShip)
            {
                MySession.PlayerShip.RemoveFromGamePruningStructure();
            }
            MyGamePruningStructure.Clear();
            if (!deletePlayerShip)
            {
                MySession.PlayerShip.AddToGamePruningStructure();  // put player ship back into pruning structure
            }


            if (deletePlayerShip)
            {
                MyParticlesManager.CloseAll();
                MySession.PlayerShip = null;
            }
            else
            {
                MySession.PlayerShip.CleanupAfterCloseAll();
            }
        }

        public static void ClearCollisionHighlights()
        {
            Vector3 zero = Vector3.Zero;
            foreach (MyVoxelMap voxelMap in MyVoxelMaps.GetVoxelMaps())
            {
                voxelMap.SetCollisionHighlighting(ref zero);
            }
            if (m_entities != null)
            {
                TraverseEntities(ClearCollisionHighlight);
            }
        }

        private static void ClearCollisionHighlight(MyEntity entity)
        {
            Vector3 zero = Vector3.Zero;
            entity.SetCollisionHighlighting(ref zero);
        }

        public static void ClearHighlights()
        {
            foreach (MyVoxelMap voxelMap in MyVoxelMaps.GetVoxelMaps())
            {
                voxelMap.ClearHighlightning();
            }
            if (m_entities != null)
            {
                foreach (MyEntity entity in m_entities)
                {
                    entity.ClearHighlightning();
                }
            }
        }

        //  Prepare list of phys objects, so then we can do remove while iterating it
        static void PrepareSafeIterationHelperForUpdate()
        {
            m_safeIterationHelper.Clear();

            foreach (MyEntity entity in m_entitiesForUpdate)
            {
                //if(!m_registreForStopAnimations.Contains(entity))
                m_safeIterationHelper.Add(entity);
            }

            MyCommonDebugUtils.AssertDebug(m_safeIterationHelper.Count <= MyConstants.MAX_ENTITIES);
        }

        static void PrepareSafeIterationHelperForAll()
        {
            m_safeIterationHelper.Clear();

            foreach (MyEntity entity in m_entities)
            {
                if (!MinerWars.AppCode.Game.SolarSystem.MySectorGenerator.IsOutsideSector(entity.GetPosition(), entity.LocalVolume.Radius))
                {
                    m_safeIterationHelper.Add(entity);
                }
            }

            MyCommonDebugUtils.AssertDebug(m_safeIterationHelper.Count <= MyConstants.MAX_ENTITIES);
        }

        public static List<MyEntity> GetSafeIterationHelperForAll()
        {
            PrepareSafeIterationHelperForAll();
            return m_safeIterationHelper;
        }

        static void GetChildren(List<MyEntity> collect)
        {
            collect.Clear();
            foreach (MyEntity entity in m_entities)
            {
                GetChildrenRecursive(entity.Children, ref collect);
            }
        }

        static void GetChildrenRecursive(ObservableCollection<MyEntity> entities, ref List<MyEntity> collect)
        {
            foreach (MyEntity entity in entities)
            {
                collect.Add(entity);

                GetChildrenRecursive(entity.Children, ref collect);
            }
        }

        public static void Link()
        {
            PrepareSafeIterationHelperForAll();
            foreach (var entity in m_safeIterationHelper)
                entity.Link();
        }

        public static void RegisterForUpdate(MyEntity entity)
        {
            //Add children first, to be updated first (and removed before parent)
            foreach (MyEntity child in entity.Children)
            {
                RegisterForUpdate(child);
            }

            if (entity.NeedsUpdate)
            {
                if (m_entitiesForUpdate != null && !m_entitiesForUpdate.Contains(entity) && m_entities.Contains(entity.GetTopMostParent()))
                    m_entitiesForUpdate.Add(entity);
            }
        }

        public static void UnregisterForUpdate(MyEntity entity)
        {
            if (m_entitiesForUpdate != null && m_entitiesForUpdate.Contains(entity))
                m_entitiesForUpdate.Remove(entity);
        }

        /*private static HashSet<MyEntity> m_registreForStopAnimations = new HashSet<MyEntity>();

        public static void RegisterForStopAnimations(MyEntity entity)
        {
            //Add children first, to be updated first (and removed before parent)
            foreach (MyEntity child in entity.Children)
            {
                RegisterForStopAnimations(child);
            }

            if (m_registreForStopAnimations != null && !m_registreForStopAnimations.Contains(entity) && m_entities.Contains(entity.GetTopMostParent()))
                m_registreForStopAnimations.Add(entity);
        }

        public static void UnregisterForStopAnimations(MyEntity entity)
        {
            foreach (MyEntity child in entity.Children)
            {
                UnregisterForStopAnimations(child);
            }
            if (m_registreForStopAnimations != null && m_registreForStopAnimations.Contains(entity))
                m_registreForStopAnimations.Remove(entity);
        }*/

        public static bool UpdateInProgress = false;
        public static bool CloseAllowed = false;

        //  Update all physics objects - BEFORE physics simulation
        public static void UpdateBeforeSimulation()
        {
            System.Diagnostics.Debug.Assert(UpdateInProgress == false);
            UpdateInProgress = true;

            //           MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PrepareSafeIterationHelperForUpdate");
            PrepareSafeIterationHelperForUpdate();
            //           MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Activate from buffer");
            int counter = 20;
            while (m_activateList.Count > 0 && counter > 0)
            {
                MyEntity entity = m_activateList.First();
                entity.Activate(true, false);
                m_activateList.Remove(entity);
                counter--;
            }
            //            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Deactivate from buffer");
            counter = 20;
            while (m_deactivateList.Count > 0 && counter > 0)
            {
                MyEntity entity = m_deactivateList.First();
                entity.Activate(false, false);
                m_deactivateList.Remove(entity);
                counter--;
            }
            //          MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            foreach (MyEntity entity in m_safeIterationHelper)
            {
                /*
                if (entity is MySmallShip)
                    continue;
                   
                if (entity is MyPrefabBase)
                    continue;
                         
                if (entity is MySpawnPoint)
                    continue;
                           
                if (entity is MyDummyPoint)
                    continue;
                                              
                if (entity is MyPrefabContainer)
                    continue; 

                if (entity is MinerWars.AppCode.Game.Entities.EntityDetector.MyEntityDetector)
                    continue;

                if (entity is MySmallShip)
                    continue;

                if (entity is MyPrefabLight)
                    continue;

                if (entity is MyPrefab)
                    continue;

                if (entity is MyPrefabKinematicPartBase)
                    continue;

                if (entity is MyPrefabSecurityControlHUB)
                    continue;

                if (entity is MyPrefabLargeWeapon)
                    continue;

                if (entity is MyInfluenceSphere)
                    continue;

                if (entity is MyPrefabAlarm)
                    continue;

                if (entity is MyLargeShipBarrelBase)
                    continue;

                if (entity is MyLargeShipGunBase)
                    continue;

                if (entity is MyPrefabLargeShip)
                    continue;

                if (entity is MyPrefabKinematic)
                    continue;

                if (entity is MyPrefabKinematicPart)
                    continue;

                if (entity is MyPrefabGenerator)
                {
                    continue;
                }   */

                entity.UpdateBeforeSimulation();
            }

            UpdateInProgress = false;

            CloseRememberedEntities(MAX_ENTITIES_CLOSE_PER_UPDATE);
        }

        //  Update all physics objects - AFTER physics simulation
        public static void UpdateAfterSimulation()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateAfterSimulation");

            System.Diagnostics.Debug.Assert(UpdateInProgress == false);
            UpdateInProgress = true;

            PrepareSafeIterationHelperForUpdate();

            //  Iterate over all phys objects. We can remove objects from "m_entities" while iterating this list. It is safe.
            foreach (MyEntity entity in m_safeIterationHelper)
            {
                entity.UpdateAfterSimulation();
            }

            UpdateInProgress = false;

            CloseRememberedEntities(MAX_ENTITIES_CLOSE_PER_UPDATE);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        public static void ActivateBuffered(MyEntity entity)
        {
            m_activateList.Add(entity);
        }

        public static void DeactivateBuffered(MyEntity entity)
        {
            m_deactivateList.Add(entity);
        }


        static List<MyRBElement> m_overlapRBElementListLocal = new List<MyRBElement>(256);
        static Queue<MyExplosionInfo> m_explosionInfo = new Queue<MyExplosionInfo>(10); // For explosions which creates explosions
        static bool processingExplosions = false;

        [ThreadStatic]
        private static List<MyEntity> m_allIgnoredEntities;
        private static List<List<MyEntity>> m_allIgnoredEntitiesCollection = new List<List<MyEntity>>();
        private static List<MyEntity> AllIgnoredEntities
        {
            get
            {
                if (m_allIgnoredEntities == null)
                {
                    m_allIgnoredEntities = new List<MyEntity>();
                    m_allIgnoredEntitiesCollection.Add(m_allIgnoredEntities);
                }
                return m_allIgnoredEntities;
            }
        }

        //  Applies external force to all physical objects near the explosion epicentrum. Result is something like throwing them away from the epicentrum, with slight rotation impulse.
        public static void ApplyExplosionForceAndDamage(ref MyExplosionInfo explosionInfo)
        {
            if (processingExplosions)
            {
                m_explosionInfo.Enqueue(explosionInfo);
                return;
            }
            processingExplosions = true;

            try
            {
                // Large weapons have got 3 parts, this is to apply damage only once
                AllIgnoredEntities.Clear();

                //  Get collision elements near the explosion (use sweep-and-prune, so we iterate only close objects)
                BoundingSphere boundingSphere = new BoundingSphere(explosionInfo.HitEntity is MyVoxelMap ? explosionInfo.VoxelExplosionCenter : explosionInfo.ExplosionSphere.Center, explosionInfo.ExplosionSphere.Radius);
                BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
                BoundingBoxHelper.AddSphere(ref boundingSphere, ref boundingBox);

                // we need local list because this method can be called from inside of the loop

                m_pruningStructure.OverlapAllBoundingBox(ref boundingBox, m_overlapRBElementListLocal, (uint)MyElementFlag.EF_RB_ELEMENT);

                foreach (MyRBElement element in m_overlapRBElementListLocal)
                {
                    MyEntity entity = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;

                    /*if (entity is MyStaticAsteroid && explosionInfo.OwnerEntity == Managers.Session.MySession.PlayerShip)
                    {
                        HUD.MyHud.ShowIndestructableAsteroidNotification();
                    }*/

                    if (entity.IsExploded())
                        continue;

                    if ((entity is Weapons.MyAmmoBase) && !(entity as Weapons.MyAmmoBase).CanBeAffectedByExplosionForce())
                        continue;

                    //We dont need to calculate damage and force here
                    if (entity is MyVoxelMap)
                        continue;

                    if (AllIgnoredEntities.Contains(entity.GetBaseEntity()))
                        continue;


                    const float explosionStrengthExponent = 2;

                    bool doDamage = true;
                    float normalizedStrength = 0;


                    //  Ignore objects not in the radius
                    float centerDistance = Vector3.Distance(entity.GetPosition(), boundingSphere.Center);
                    BoundingSphere entitySphere = entity.WorldVolume;
                    float distance = Math.Min(MyUtils.GetSmallestDistanceToSphereAlwaysPositive(ref boundingSphere.Center, ref entitySphere), centerDistance);

                    if (MyMwcUtils.IsZero(distance * 0.01f))
                        distance = 1;// continue; //we need it more sensitive

                    MyEntity damageEntity = entity.GetBaseEntity();

                    Vector3 awayDirection = entity.GetPosition() - boundingSphere.Center;
                    bool isSamePosition = false;
                    if (awayDirection.LengthSquared() > 0.1f)
                    {
                        awayDirection = MyMwcUtils.Normalize(awayDirection);
                    }
                    else
                    {
                        awayDirection = Vector3.Forward;
                        isSamePosition = true;
                    }


                    // Hit entity always get full hit
                    if (explosionInfo.HitEntity == entity)
                    {
                        normalizedStrength = 1;
                    }
                    else
                    {
                        float radius = explosionInfo.ExplosionSphere.Radius;

                        //  If objects is in the radius, calculate how much strength will explosion send to him and apply it to the object
                        float normalizedDistance = MathHelper.Clamp(distance / radius, 0, 1);
                        normalizedStrength = 1 - (float)Math.Pow(normalizedDistance, explosionStrengthExponent); // Reversed exponencial, at 50% distance from explosion is dealt 75% of max damage (when power = 2)

                        if (normalizedStrength <= 0)
                            continue; // No hit

                        //normalizedStrength *= normalizedStrength; //lets do it exponential

                        if (explosionInfo.CheckIntersections && !isSamePosition)
                        {
                            MyRender.GetRenderProfiler().StartProfilingBlock("CheckIntersections");

                            // do damage linearly proportional to the distance (the closer, the more damage)
                            var line = new MyLine(boundingSphere.Center, entity.GetPosition(), true);

                            if (damageEntity.CheckExplosionObstacles)
                            {
                                var intersection = GetAnyIntersectionWithLine(ref line, entity, explosionInfo.HitEntity, false, false, false, true);

                                // check intersection with scene from epicentre to entity position
                                doDamage = !intersection.HasValue;
                            }

                            MyRender.GetRenderProfiler().EndProfilingBlock();
                        }
                    }

                    // if nothing is in the way (no wall, no asteroid), apply damage
                    if (doDamage)
                    {
                        MyRender.GetRenderProfiler().StartProfilingBlock("doDamage");
                        AllIgnoredEntities.Add(damageEntity);
                        damageEntity.DoDamage(normalizedStrength * explosionInfo.PlayerDamage, normalizedStrength * explosionInfo.Damage, normalizedStrength * explosionInfo.EmpDamage, MyDamageType.Explosion, MyAmmoType.Unknown, explosionInfo.OwnerEntity, explosionInfo.CustomEffect != null);
                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }


                    if (explosionInfo.ExplosionForceDirection == MyExplosionForceDirection.IMPLOSION)
                    {
                        awayDirection *= -1;
                    }

                    float angStrength = explosionInfo.StrengthAngularImpulse;
                    var forcePosition = entity.WorldAABB.Min; // Apply force to entity corner - to make it rotate

                    //  If this object is miner ship, then shake his head little bit
                    if (entity == MySession.PlayerShip)
                    {
                        MyRender.GetRenderProfiler().StartProfilingBlock("IncreaseHeadShake");
                        MySmallShip minerShip = (MySmallShip)entity;
                        minerShip.IncreaseHeadShake(normalizedStrength * MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_AFTER_EXPLOSION);
                        angStrength *= MyExplosionsConstants.EXPLOSION_STRENGTH_ANGULAR_IMPULSE_PLAYER_MULTIPLICATOR;
                        forcePosition = entity.GetPosition(); // We don't want to rotate player by force
                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }

                    // When MyAmmoBase entity is close to explosion it will explode
                    var ammo = entity as MyAmmoBase;
                    if (ammo != null)
                    {
                        MyRender.GetRenderProfiler().StartProfilingBlock("ammo.ExplodeCascade");
                        ammo.ExplodeCascade(explosionInfo.CascadeLevel + 1);
                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }

                    MyRender.GetRenderProfiler().StartProfilingBlock("entity.Physics.AddForce");
                    if (!entity.IsDead())
                    {
                        entity.Physics.AddForce(
                            MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                            awayDirection * normalizedStrength * explosionInfo.StrengthImpulse * MyExplosionsConstants.EXPLOSION_FORCE_RADIUS_MULTIPLIER,
                            forcePosition,
                            normalizedStrength * MyMwcUtils.GetRandomVector3Normalized() * angStrength);
                    }
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                m_overlapRBElementListLocal.Clear();
            }
            finally
            {
                processingExplosions = false;
            }
            while (m_explosionInfo.Count > 0)
            {
                var info = m_explosionInfo.Dequeue();
                ApplyExplosionForceAndDamage(ref info);
            }
        }

        //  Throws objects in direction of sun wind, but only if objects are inside the sun wind "storm" and are visible from sun.
        public static void ApplySunWindForce(List<MyEntity> entities, ref MyPlane planeFront, ref MyPlane planeBack, Type[] doNotIgnoreTheseTypes, ref Vector3 sunWindDirectionNormalized)
        {
            MyUtilRandomVector3ByDeviatingVector a = new MyUtilRandomVector3ByDeviatingVector(sunWindDirectionNormalized);

            foreach (var entity in entities)
            {
                MyPerformanceCounter.PerCameraDraw.Increment("SunWind all entities");

                //  Forces are random
                if (MyMwcUtils.GetRandomBool(5) != true) continue;

                //  Apply force only if object is in between the sun wind planes
                float distanceToFront = MyUtils.GetDistanceFromPointToPlane(entity.GetPosition(), ref planeFront);
                float distanceToBack = MyUtils.GetDistanceFromPointToPlane(entity.GetPosition(), ref planeBack);
                if ((distanceToFront <= 0) && (distanceToBack >= 0))
                {
                    MyPerformanceCounter.PerCameraDraw.Increment("SunWind entities between planes");

                    //  Apply impulse only if this object is visible from sun, or of sun wind can move with it
                    if (entity.IsVisibleFromSun)
                    {
                        MyPerformanceCounter.PerCameraDraw.Increment("SunWind entities influented");

                        Vector3 randomVector = a.GetNext(MySunWindConstants.FORCE_ANGLE_RANDOM_VARIATION_IN_RADIANS);
                        if (entity.Physics != null)
                        {
                            entity.Physics.AddForce(
                                MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                                randomVector * MyMwcUtils.GetRandomFloat(0.0f, MySunWindConstants.FORCE_IMPULSE_RANDOM_MAX),
                                entity.GetPosition() - randomVector * MySunWindConstants.FORCE_IMPULSE_POSITION_DISTANCE,
                                null);
                        }

                        // Apply damage to affected entities (small ships etc.)
                        entity.DoDamage(
                            MySunWindConstants.HEALTH_DAMAGE * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS,
                            MySunWindConstants.SHIP_DAMAGE * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS,
                            0,
                            MyDamageType.Sunwind, MyAmmoType.Unknown, null);
                    }
                }
            }
        }

        //  Calculates intersection of line with any bounding sphere in the world (every model instance). Closest intersection and intersected triangleVertexes will be returned.
        //  Params:
        //      line - line we want to test for intersection
        //      ignoreModelInstance0 and 1 - we may specify two phys objects we don't want to test for intersections. Usually this is model instance of who is shoting, or missile, etc.
        //      outIntersection - intersection data calculated by this method
        //      scale - the amount to scale the bounding sphere by, so we can pick up objects near the line
        public static MyIntersectionResultLineBoundingSphere? GetIntersectionWithLineAndBoundingSphere(ref MyLine line, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, float scale, Predicate<MyEntity> condition, bool ignoreChilds = false)
        {
            EntityResultSet.Clear();
            if (ignoreChilds)
            {
                if (ignorePhysObject0 != null)
                {
                    ignorePhysObject0 = ignorePhysObject0.GetBaseEntity();
                    ignorePhysObject0.GetChildrenRecursive(EntityResultSet);
                }

                if (ignorePhysObject1 != null)
                {
                    ignorePhysObject1 = ignorePhysObject1.GetBaseEntity();
                    ignorePhysObject1.GetChildrenRecursive(EntityResultSet);
                }
            }

            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = new BoundingBox();
            boundingBox = boundingBox.CreateInvalid();
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);

            MyIntersectionResultLineBoundingSphere? ret = null;

            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity tempEntity = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                MyEntity topMostParentPrefab = tempEntity.GetTopMostParent(typeof(MyPrefabBase));
                MyEntity physicObject = topMostParentPrefab != null ? topMostParentPrefab : tempEntity;

                if (condition != null && !condition(physicObject)) continue;

                //  Objects to ignore
                if ((physicObject == ignorePhysObject0) || (physicObject == ignorePhysObject1) || (ignoreChilds && EntityResultSet.Contains(physicObject))) continue;

                BoundingSphere sphere = physicObject.WorldVolumeHr.Radius == 0 ?
                    new BoundingSphere(physicObject.WorldVolume.Center, physicObject.WorldVolume.Radius) :
                    new BoundingSphere(physicObject.WorldVolumeHr.Center, physicObject.WorldVolumeHr.Radius);

                Ray ray = new Ray(line.From, line.Direction);
                float? intersectionDistance = sphere.Intersects(ray);
                if (intersectionDistance.HasValue && intersectionDistance <= line.Length)
                {
                    MyIntersectionResultLineBoundingSphere? testResult = new MyIntersectionResultLineBoundingSphere(intersectionDistance.Value, physicObject);

                    //  If intersection occured and distance to intersection is closer to origin than any previous intersection)
                    ret = MyIntersectionResultLineBoundingSphere.GetCloserIntersection(ref ret, ref testResult);
                }
            }
            elements.Clear();

            return ret;
        }


        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void GetIntersectedElements(ref MyLine line, List<MyRBElement> outElementList)
        {
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);
            m_pruningStructure.OverlapAllBoundingBox(ref boundingBox, outElementList, (uint)MyElementFlag.EF_RB_ELEMENT);
        }

        //  Calculate an intersection between a line and any of the models in the world. Use collision models.
        //  The returned intersection doesn't need to be the closest one.
        //  Params:
        //      ignoreEntity0 and 1: entities we don't want to test for intersections: usually someone who is shoting, missile, etc.
        public static Vector3? GetAnyIntersectionWithLine(ref MyLine line, MyEntity ignoreEntity1, MyEntity ignoreEntity2, bool skipAmmo, bool skipShips, bool skipOpenableDoors, bool ignoreWithChildren)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetAnyIntersectionWithLine.GetChildren");
            EntityResultSet.Clear();
            if (ignoreWithChildren)
            {
                if (ignoreEntity1 != null)
                {
                    ignoreEntity1 = ignoreEntity1.GetBaseEntity();
                    ignoreEntity1.GetChildrenRecursive(EntityResultSet);
                }

                if (ignoreEntity2 != null)
                {
                    ignoreEntity2 = ignoreEntity2.GetBaseEntity();
                    ignoreEntity2.GetChildrenRecursive(EntityResultSet);
                }
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            ++MyPerformanceCounter.PerCameraDraw.RayCastCount;

            //  Get collision skins near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            m_pruningStructure.OverlapAllLineSegment(ref line, LineOverlapRBElementList, (uint)MyElementFlag.EF_RB_ELEMENT);

            foreach (var result in LineOverlapRBElementList)
            {
                ++MyPerformanceCounter.PerCameraDraw.RayCastModelsProcessed;
                MyEntity entity = ((MyPhysicsBody)(result.Element).GetRigidBody().m_UserData).Entity;

                if (entity.Physics.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;

                //  Objects to ignore
                if (entity == ignoreEntity1 ||
                    entity == ignoreEntity2 ||
                    entity is MyExplosionDebrisBase ||
                    entity is MyDummyPoint ||
                    entity is MySpawnPoint ||
                    entity is MyInfluenceSphere ||
                    (skipAmmo && entity is MyAmmoBase) ||
                    (skipShips && entity is MySmallShip) ||
                    (skipOpenableDoors && entity is MyPrefabKinematicPartBase && !(entity is MyPrefabKinematicRotatingPart)) ||
                    (ignoreWithChildren && EntityResultSet.Contains(entity))
                )
                    continue;

                Vector3? ret;
                entity.GetIntersectionWithLine(ref line, out ret, true);
                if (ret != null) return ret;
            }
            return null;
        }

        //  Calculate whether there's an intersection between a line and any of the models in the world. Use only AABBs for speed.
        //  Params:
        //      ignoreEntity0 and 1: entities we don't want to test for intersections: usually someone who is shoting, missile, etc.
        public static bool IsAnyIntersectionWithLineAABBOnly(ref MyLine line, MyEntity ignoreEntity1, MyEntity ignoreEntity2)
        {
            ++MyPerformanceCounter.PerCameraDraw.RayCastCount;

            //  Get collision skins near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            m_pruningStructure.OverlapAllLineSegment(ref line, LineOverlapRBElementList);

            foreach (var result in LineOverlapRBElementList)
            {
                MyEntity entity = ((MyPhysicsBody)(result.Element).GetRigidBody().m_UserData).Entity;

                if (entity.Physics.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;

                //  Objects to ignore
                if (entity == ignoreEntity1 ||
                    entity == ignoreEntity2 ||
                    entity is MyExplosionDebrisBase ||
                    entity is MyDummyPoint ||
                    entity is MySpawnPoint ||
                    entity is MyInfluenceSphere ||
                    entity is MyAmmoBase ||
                    entity is MySmallShip ||
                    (entity is MyPrefabKinematicPartBase && !(entity is MyPrefabKinematicRotatingPart))
                )
                    continue;

                return true;
            }
            return false;
        }

        //  Calculates intersection of line with any triangleVertexes in the world (every model instance). Closest intersection and intersected triangleVertexes will be returned.
        //  Params:
        //      line - line we want to test for intersection
        //      ignoreModelInstance0 and 1 - we may specify two phys objects we don't want to test for intersections. Usually this is model instance of who is shoting, or missile, etc.
        //      outIntersection - intersection data calculated by this method
        public static MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(ref MyLine line, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool skipExplosionDebris = false, bool skipAmmo = false, bool skipKinematics = false, bool ignoreChilds = false, bool ignorePrefabLights = false, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES, bool ignoreSmallShips = false)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIntersectionWithLine.GetChildren");
            EntityResultSet.Clear();
            if (ignoreChilds)
            {
                if (ignorePhysObject0 != null)
                {
                    ignorePhysObject0 = ignorePhysObject0.GetBaseEntity();
                    ignorePhysObject0.GetChildrenRecursive(EntityResultSet);
                }

                if (ignorePhysObject1 != null)
                {
                    ignorePhysObject1 = ignorePhysObject1.GetBaseEntity();
                    ignorePhysObject1.GetChildrenRecursive(EntityResultSet);
                }
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            ++MyPerformanceCounter.PerCameraDraw.RayCastCount;

            //  Get collision skins near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIntersectionWithLine.OverlapRBAllLineSegment");
            m_pruningStructure.OverlapAllLineSegment(ref line, LineOverlapRBElementList, (uint)MyElementFlag.EF_RB_ELEMENT);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            LineOverlapRBElementList.Sort(MyLineSegmentOverlapResult<MyRBElement>.DistanceComparer);

            MyIntersectionResultLineTriangleEx? ret = null;
            foreach (var result in LineOverlapRBElementList)
            {
                ++MyPerformanceCounter.PerCameraDraw.RayCastModelsProcessed;
                if (ret.HasValue)
                {
                    float distToIntersectionSq = Vector3.DistanceSquared(line.From, ret.Value.IntersectionPointInWorldSpace);
                    float distToAabbSq = result.Distance * result.Distance;
                    if (distToIntersectionSq < distToAabbSq)
                    {
                        break;
                    }
                }

                MyRBElement element = result.Element;

                //Physics on this entity could be disabled in another thread
                if (element.GetRigidBody() == null)
                    continue;
                if (element.GetRigidBody().m_UserData == null)
                    continue;

                MyEntity entity = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;

                //  Objects to ignore
                if (entity == ignorePhysObject0 || entity == ignorePhysObject1 || (ignoreChilds && EntityResultSet.Contains(entity))) continue;

                if (skipExplosionDebris && entity is Entities.MyExplosionDebrisBase)
                {
                    //PM: Do we ever need intersection with this? It causes problems with aiming correction
                    continue;
                }

                if (skipAmmo && entity is MyAmmoBase)
                {
                    continue;
                }

                if (skipKinematics && (entity is MyPrefabKinematicPart || entity is MyPrefabKinematic))
                {
                    continue;
                }

                if (ignorePrefabLights && (entity is MyPrefabLight))
                    continue;

                if (entity.Physics.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;

                if (ignoreSmallShips && (entity is MySmallShip))
                    continue;


                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIntersectionWithLine.GetIntersectionWithLine");
                MyIntersectionResultLineTriangleEx? testResultEx;
                entity.GetIntersectionWithLine(ref line, out testResultEx, flags);

                // Large weapons problem - PrefabLargeWeapon in fact tests GunBase
                if (testResultEx.HasValue && testResultEx.Value.Entity != ignorePhysObject0 && testResultEx.Value.Entity != ignorePhysObject1 && (!ignoreChilds || !EntityResultSet.Contains(testResultEx.Value.Entity)))
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIntersectionWithLine.GetCloserIntersection");
                    //  If intersection occured and distance to intersection is closer to origin than any previous intersection
                    ret = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref ret, ref testResultEx);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            return ret;
        }

        private static MyEntity GetCloserEntity(MyEntity a, MyEntity b)
        {
            if (((a == null) && (b != null)) ||
                ((a != null) && (b != null) && (b.GetDistanceBetweenCameraAndPosition() < a.GetDistanceBetweenCameraAndPosition())))
            {
                //  If only "b" contains valid intersection, or when it's closer than "a"
                return b;
            }
            else
            {
                //  This will be returned also when ((a == null) && (b == null))
                return a;
            }
        }

        // preinsert interection testing:
        // if these pairs collide, we take it as collision. Any other combination is ignored.
        // sphere vs sphere, box, voxel, tringle mesh
        // box vs shpere, box, voxel, tringle mesh
        // tringle mesh vs box, shpere
        // voxel vs voxel (check only boundig box)
        // the input rigid body can be already in scene

        // assumption rbInputToTest should have in userdata variable pointer to parent entity (for collision test with itself exclusion) 
        //      otherwise its new object not added in phys scene yet

        public static void GetCollisionListForElement(MyRBElement rbInputElement)
        {
            MyDynamicAABBTree prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();
            rbInputElement.UpdateAABB();

            BoundingBox rbInputElementGetWorldSpaceAABB = rbInputElement.GetWorldSpaceAABB();//wrong investigate fix

            var elements = GetElementsInBox(prunningStructure, ref rbInputElementGetWorldSpaceAABB);
            foreach (MyRBElement worldCollidingElement in elements)
            {
                MyRBElement el1 = rbInputElement;
                MyRBElement el2 = worldCollidingElement;
                MyPhysicsBody gameRB = (MyPhysicsBody)worldCollidingElement.GetRigidBody().m_UserData;
                if (!gameRB.Enabled)
                    continue;
                MyEntity worldCollidingEntity = gameRB.Entity;


                if (!MyPhysics.physicsSystem.GetRigidBodyModule().IsEnabledCollisionInLayers(el1.CollisionLayer, el2.CollisionLayer))
                    continue;

                //test if object is alredy in scene //? how we have only rigid body now so far
                if (rbInputElement.GetRigidBody().m_UserData != null && ((MyPhysicsBody)rbInputElement.GetRigidBody().m_UserData).Entity == worldCollidingEntity)
                    continue;
                MyEntity inputEntity = ((MyPhysicsBody)rbInputElement.GetRigidBody().m_UserData).Entity;

                if (!worldCollidingEntity.VisibleInGame)
                    continue;

                // we ignore collision between foundation factory and her prefab container
                if (worldCollidingEntity is MyPrefabFoundationFactory)
                {
                    MyPrefabFoundationFactory foundationFactory = worldCollidingEntity as MyPrefabFoundationFactory;
                    if (foundationFactory.PrefabContainer == inputEntity)
                        continue;
                }

                // we ignore collision between largeship and her hangar
                if (worldCollidingEntity is MyPrefabLargeShip && inputEntity is MyPrefabHangar ||
                    worldCollidingEntity is MyPrefabHangar && inputEntity is MyPrefabLargeShip)
                {
                    continue;
                }

                bool collision = false;
                if (el1 is MyRBSphereElement) //sphere vs sphere
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);
                if (el1 is MyRBSphereElement && el2 is MyRBVoxelElement) //sphere vs voxel
                {
                    MyRBSphereElement el1Retyped = el1 as MyRBSphereElement;
                    MyVoxelMap el2Retyped = worldCollidingEntity as MyVoxelMap;
                    collision = (el2Retyped != null && el2Retyped.DoOverlapSphereTest(el1Retyped.Radius, el1Retyped.GetGlobalTransformation().Translation));
                }
                /*
                if (el1 is MyRBSphereElement && el1 is MyRBSphereElement) //sphere vs sphere
                {
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);
                }
                if (el1 is MyRBSphereElement && el1 is MyRBSphereElement) //sphere vs sphere
                {
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);
                }
                if (el1 is MyRBSphereElement && el1 is MyRBSphereElement) //sphere vs sphere
                {
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);
                }
                */
                if (el1 is MyRBBoxElement)
                {
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);
                }
                if (el1 is MyRBTriangleMeshElement && (el2 is MyRBSphereElement || el2 is MyRBBoxElement))
                    collision = MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(el1, el2);

                /*
                if (el1 is MyRBVoxelElement && el2 is MyRBVoxelElement)
                {
                    MyVoxelMap e1 = inputEntity as MyVoxelMap;
                    MyVoxelMap e2 = worldCollidingEntity as MyVoxelMap;

                    BoundingBox woxMapBB = e2.BoundingBox;
                    if (e1.IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref woxMapBB))
                        collision = true;
                }
                */

                if (collision)
                    m_CollisionsForElementsHelper.Add(el2);
            }
            elements.Clear();
        }


        //  Return JLX that is intersecting sphere, see comments for method used below
        public static MyEntity GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            return GetIntersectionWithSphere(ref sphere, null, null, false, false);
        }

        //  Return objects that is intersecting sphere, see comments for method used below
        public static MyEntity GetIntersectionWithSphere(ref BoundingSphere sphere, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1)
        {
            return GetIntersectionWithSphere(ref sphere, ignorePhysObject0, ignorePhysObject1, false, true);
        }


        //  Return reference to object that intersects specific sphere. If not intersection, null is returned.
        //  We don't look for closest intersection - so we stop on first intersection found.
        //  Params:
        //      sphere - sphere we want to test for intersection
        //      ignoreModelInstance0 and 1 - we may specify two phys objects we don't want to test for intersections. Usually this is model instance of who is shoting, or missile, etc.
        //      ignoreVoxelMaps - in some cases, we want to test intersection only with non-voxelmap phys objects
        public static void GetIntersectionWithSphere(ref BoundingSphere sphere, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool ignoreVoxelMaps, bool volumetricTest, ref List<MyEntity> result)
        {
            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphere, ref boundingBox);

            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;

                // Voxelmap to ignore
                if (ignoreVoxelMaps && physicObject is MyVoxelMap) continue;
                //  Objects to ignore
                if ((physicObject == ignorePhysObject0) || (physicObject == ignorePhysObject1)) continue;

                if (physicObject.Physics.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;


                if (physicObject is Entities.MyExplosionDebrisBase)
                {
                    //PM: Do we ever need intersection with this? It causes problems with aiming correction
                    continue;
                }

                if (physicObject.GetIntersectionWithSphere(ref sphere))
                {
                    //  If intersection found, return that object. We don't need to look for more objects.
                    result.Add(physicObject);
                }

                if (volumetricTest && (physicObject is MyVoxelMap) && (physicObject as MyVoxelMap).DoOverlapSphereTest(sphere.Radius, sphere.Center))
                {
                    //  If intersection found, return that object. We don't need to look for more objects.
                    result.Add(physicObject);
                }
            }
            elements.Clear();
        }

        //  Return reference to object that intersects specific sphere. If not intersection, null is returned.
        //  We don't look for closest intersection - so we stop on first intersection found.
        //  Params:
        //      sphere - sphere we want to test for intersection
        //      ignoreModelInstance0 and 1 - we may specify two phys objects we don't want to test for intersections. Usually this is model instance of who is shoting, or missile, etc.
        //      ignoreVoxelMaps - in some cases, we want to test intersection only with non-voxelmap phys objects
        public static MyEntity GetIntersectionWithSphere(ref BoundingSphere sphere, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool ignoreVoxelMaps, bool volumetricTest)
        {
            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref sphere, ref boundingBox);

            MyEntity result = null;

            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;

                // Voxelmap to ignore
                if (ignoreVoxelMaps && physicObject is MyVoxelMap) continue;
                //  Objects to ignore
                if ((physicObject == ignorePhysObject0) || (physicObject == ignorePhysObject1)) continue;

                if (physicObject is MyExplosionDebrisBase)
                {
                    //PM: Do we ever need intersection with this? It causes problems with aiming correction
                    continue;
                }

                if (physicObject.Physics.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;

                if (physicObject.GetIntersectionWithSphere(ref sphere))
                {
                    //  If intersection found, return that object. We don't need to look for more objects.
                    result = physicObject;
                    break;
                }

                if (volumetricTest && (physicObject is MyVoxelMap) && (physicObject as MyVoxelMap).DoOverlapSphereTest(sphere.Radius, sphere.Center))
                {
                    //  If intersection found, return that object. We don't need to look for more objects.
                    result = physicObject;
                    break;
                }
            }
            elements.Clear();
            return result;
        }

        public static void GetIntersectionsWithAABB(BoundingBox boundingBox, List<MyEntity> intersected)
        {
            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                intersected.Add(physicObject);
            }
            elements.Clear();
        }

        public static void GetIntersectionsWithAABBOfType<T>(BoundingBox boundingBox, List<T> intersected) where T : MyEntity
        {
            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                if (physicObject is T)
                {
                    intersected.Add((T)physicObject);
                }
            }
            elements.Clear();
        }

        //the method finds the new direction from weapon position to the nearest small ship hit point, which is hit in given direction. Can be used for better-aiming system.
        public static Vector3? GetDirectionFromStartPointToHitPointOfNearestObject(MyEntity ignoreEntity, Vector3 weaponPosition, float maxTrajectoryLeng)
        {
            Vector3 directionStartPosition = MyCamera.Position;
            Vector3 directionToSearchTheHit;

            MyCameraAttachedToEnum cameraAttachment = MyGuiScreenGamePlay.Static.CameraAttachedTo;

            bool cameraAttachmentAllowedMode = ((cameraAttachment == MyCameraAttachedToEnum.PlayerMinerShip) || (cameraAttachment == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic) || cameraAttachment == MyCameraAttachedToEnum.LargeWeapon);
            if (!cameraAttachmentAllowedMode)
                return null;

            if (cameraAttachment == MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic)
            {
                if (MySession.Is25DSector)
                {
                    directionStartPosition = ignoreEntity.GetPosition();
                    Vector3 crosshair = directionStartPosition + ignoreEntity.WorldMatrix.Forward;
                    crosshair.Y = 0;

                    directionToSearchTheHit = crosshair - directionStartPosition;
                    directionToSearchTheHit.Y = 0;
                }
                else
                {
                    directionToSearchTheHit = MyThirdPersonSpectator.GetCrosshair() - directionStartPosition;
                }

                directionToSearchTheHit.Normalize();
            }
            else
            {
                directionToSearchTheHit = MyCamera.ForwardVector;
            }

            Vector3 lineEndPosition = directionStartPosition + (directionToSearchTheHit * maxTrajectoryLeng);


            MyLine line = new MyLine(directionStartPosition, lineEndPosition, true);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEntities.GetIntersectionWithLine");
            //Intersection ignores children of "ignoreEntity", thus we must not hit our own barrels
            MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine(ref line, ignoreEntity, null, true, false, false, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            if (intersection == null)
                return null;

            Vector3 result = intersection.Value.IntersectionPointInWorldSpace - weaponPosition;
            if (MySession.Is25DSector)
                result.Y = 0;
            result.Normalize();

            return result;
        }

        //  This method calculated the nearest intersection between line and phys object exactly as GetIntersectionWithLine().
        //  You can define one ignorePhysObject entity
        //public static MyIntersectionResultLineTriangleEx? GetNearestIntersectionWithSphere(MyEntity ignorePhysObject, ref MyLine line)
        //{
        //    //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
        //    //BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
        //    //BoundingBoxHelper.AddLine(ref line, ref boundingBox);
        //    //m_pruningStructure.OverlapRBAllBoundingBox(ref boundingBox, m_overlapRBElementList);
        //    m_pruningStructure.OverlapRBAllLineSegment(ref line, m_overlapRBElementList);

        //    MyIntersectionResultLineTriangleEx? ret = null;

        //    foreach (MyRBElement element in m_overlapRBElementList)
        //    {
        //        MyEntity physicObject = ((MyGameRigidBody)element.GetRigidBody().m_UserData).Entity;
        //        if (physicObject == ignorePhysObject)
        //            continue;
        //        if (physicObject is AppCode.Game.Managers.EntityManager.Entities.MyExplosionDebrisBase)
        //        { //PM: Do we ever need intersection with this? It causes problems with aiming correction
        //            continue;
        //        }

        //        MyIntersectionResultLineTriangleEx? testResultEx = null;
        //        //physicObject.GetIntersectionWithLine(ref line, out testResultEx);
        //        if (testResultEx == null)
        //            continue;

        //        ret = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref ret, ref testResultEx);
        //    }

        //    return ret;
        //}

        //  This method calculated intersection between line and phys object exactly as GetIntersectionWithLine(), but ignores object other than specified types/classes.
        //  It's usefull if you want to check intersection with specified types, e.g. voxels, large ships, etc, but want to ignore other ones.
        public static MyIntersectionResultLineTriangleEx? GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref MyLine line, Type[] doNotIgnoreTheseTypes, bool checkDerivedTypes = false)
        {
            ++MyPerformanceCounter.PerCameraDraw.RayCastCount;

            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);

            MyIntersectionResultLineTriangleEx? ret = null;
            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                ++MyPerformanceCounter.PerCameraDraw.RayCastModelsProcessed;

                //  Find if this object is of type we MUST NOT ignore and if is, we don't ignore. Otherwise we ignore.
                bool ignore = true;
                foreach (Type doNotIgnore in doNotIgnoreTheseTypes)
                {
                    if (physicObject.GetType() == doNotIgnore ||
                        checkDerivedTypes && doNotIgnore.IsAssignableFrom(physicObject.GetType()))
                    {
                        ignore = false;
                        break;
                    }
                }
                if (ignore == true) continue;

                MyIntersectionResultLineTriangleEx? testResultEx;
                physicObject.GetIntersectionWithLine(ref line, out testResultEx);

                //  If intersection occured and distance to intersection is closer to origin than any previous intersection
                ret = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref ret, ref testResultEx);
            }
            elements.Clear();

            return ret;
        }

        //  This method calculated intersection between line and phys object exactly as GetIntersectionWithLine(), but ignores object other than specified types/classes.
        //  It's usefull if you want to check intersection with specified types, e.g. voxels, large ships, etc, but want to ignore other ones.
        public static bool GetAnyIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref MyLine line, Type[] doNotIgnoreTheseTypes, bool checkDerivedTypes = false)
        {
            ++MyPerformanceCounter.PerCameraDraw.RayCastCount;

            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);

            bool result = false;
            var elements = GetElementsInBox(ref boundingBox);
            foreach (MyRBElement element in elements)
            {
                MyEntity physicObject = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                ++MyPerformanceCounter.PerCameraDraw.RayCastModelsProcessed;

                //  Find if this object is of type we MUST NOT ignore and if is, we don't ignore. Otherwise we ignore.
                bool ignore = true;
                foreach (Type doNotIgnore in doNotIgnoreTheseTypes)
                {
                    if (physicObject.GetType() == doNotIgnore ||
                        checkDerivedTypes && doNotIgnore.IsAssignableFrom(physicObject.GetType()))
                    {
                        ignore = false;
                        break;
                    }
                }
                if (ignore == true) continue;

                MyIntersectionResultLineTriangleEx? testResultEx;
                if (physicObject.GetIntersectionWithLine(ref line, out testResultEx))
                {
                    result = true;
                    break;
                }
            }
            elements.Clear();
            return result;
        }

        //  Usefull especially for rectangular selection of objects - we get all objects intersecting bounding frustum
        public static void GetAllIntersectionWithBoundingFrustum_UNOPTIMIZED(ref BoundingFrustum boundingFrustum, List<MyEntity> resultList, bool onlyVisible = true, bool onlySelectable = false, bool ignorePrefabs = false)
        {
            EntityInputList.Clear();

            if (onlySelectable)
            {
                GetChildren(EntityInputList);
            }
            else
            {
                EntityInputList.Clear();
                foreach (var entity in m_entities)
                {
                    EntityInputList.Add(entity);
                }
            }

            GetIntersectionWithBoundingFrustum(ref boundingFrustum, EntityInputList, resultList, onlyVisible, onlySelectable, ignorePrefabs);
        }


        //  Usefull especially for rectangular selection of objects - we get all objects intersecting bounding frustum
        public static void GetAllIntersectionWithBoundingFrustum(ref BoundingFrustum boundingFrustum, List<MyEntity> resultList, bool onlyVisible = true, bool onlySelectable = false, bool ignorePrefabs = false)
        {
            EntityInputList.Clear();

            m_pruningStructure.OverlapAllFrustum(ref boundingFrustum, OverlapRBElementList, (uint)MyElementFlag.EF_RB_ELEMENT);

            foreach (MyRBElement element in OverlapRBElementList)
            {
                MyPhysicsBody gameRB = (MyPhysicsBody)element.GetRigidBody().m_UserData;
                if (gameRB.CollisionLayer == MyConstants.COLLISION_LAYER_UNCOLLIDABLE)
                    continue;

                resultList.Add(gameRB.Entity);
            }

            OverlapRBElementList.Clear();
        }

        // internal method for bounding frustum intersection
        static void GetIntersectionWithBoundingFrustum(ref BoundingFrustum boundingFrustum, List<MyEntity> testObjects, List<MyEntity> addToList, bool onlyVisible = true, bool onlySelectable = false, bool ignorePrefabs = false)
        {
            System.Diagnostics.Debug.Assert(addToList != null);

            foreach (MyEntity physObject in testObjects)
            {
                if (physObject.GetIntersectionWithBoundingFrustum(ref boundingFrustum) == true)
                {
                    if (!onlyVisible || (onlyVisible && physObject.IsVisible()))
                        if (!onlySelectable || (onlySelectable && MyEntities.IsTypeSelectable(physObject.GetType())))
                            if (!ignorePrefabs || (ignorePrefabs && !(physObject is MyPrefabBase)))
                                addToList.Add(physObject);
                }
            }
        }

        //  Returns total count of large ships in sectors
        public static int GetLargeShipsCount()
        {
            int largeShipCounter = 0;
            foreach (MyEntity entity in m_entities)
            {
                if (entity is MyPrefabLargeShip)
                {
                    largeShipCounter++;
                }
            }
            return largeShipCounter;
        }

        //  Returns count of all non-voxel map objects
        //  //check for all childs of prefab container
        public static int GetObjectsCount()
        {
            return m_entities.Count;
        }

        //  Returns count of all non-voxel map objects
        //  //check for all childs of prefab container
        public static int GetEditorObjectsCount()
        {
            return m_editorObjectsCount;
        }

        public static List<MyMwcObjectBuilder_Base> Save()
        {            /*
            if (MyFakes.MWBUILDER)
            {
                MyVoxelMaps.SaveVoxelContents();
            }
                       */
            return GetObjectBuilders(m_entities, true, true);
        }

        public static bool IsMarkedForClose(MyEntity entity)
        {
            return m_entitiesToClose.Contains(entity);
        }

        // This method returns object builders of all objects in MyEntities, not only JLX objects.
        // Saves the whole hierarchy, but every type needs to resolve parent links on its own (probably in Link)
        private static List<MyMwcObjectBuilder_Base> GetObjectBuilders(HashSet<MyEntity> entities, bool getExactCopy, bool entitiesToSave)
        {
            /*
            foreach (MyEntity entity in m_deactivateList)
            {
                entity.Activate(false, false);
            }
            m_deactivateList.Clear();

            foreach (MyEntity entity in m_activateList)
            {
                entity.Activate(true, false);
            }
            m_activateList.Clear();
              */

            List<MyMwcObjectBuilder_Base> physObjectBuilders = null;
            if (entities != null)
            {
                physObjectBuilders = new List<MyMwcObjectBuilder_Base>();

                foreach (var entity in entities)
                {
                    if (entity.Save && !m_entitiesToClose.Contains(entity))
                    {
                        entity.BeforeSave();
                        MyMwcObjectBuilder_Base objBuilder = entity.GetObjectBuilder(getExactCopy);
                        Debug.Assert(objBuilder != null, "Save flag specified returns nullable objectbuilder");
                        
             if (m_activateList.Contains(entity))
                 objBuilder.PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;

             if (m_deactivateList.Contains(entity))
                 objBuilder.PersistentFlags |= MyPersistentEntityFlags.Deactivated;
                          
                        physObjectBuilders.Add(objBuilder);
                    }

                    // recurse
                    var childrenObjectBuilders = GetObjectBuilders(entity.Children, getExactCopy, entitiesToSave);
                    if (childrenObjectBuilders != null) physObjectBuilders.AddRange(childrenObjectBuilders);
                }
            }

            return physObjectBuilders;
        }


        // This method returns object builders of all objects in MyEntities, not only JLX objects.
        // Saves the whole hierarchy, but every type needs to resolve parent links on its own (probably in Link)
        private static List<MyMwcObjectBuilder_Base> GetObjectBuilders(ObservableCollection<MyEntity> entities, bool getExactCopy, bool entitiesToSave)
        {
            List<MyMwcObjectBuilder_Base> physObjectBuilders = null;
            if (entities != null)
            {
                physObjectBuilders = new List<MyMwcObjectBuilder_Base>();

                foreach (var entity in entities)
                {
                    if (entity.Save)
                    {
                        entity.BeforeSave();
                        MyMwcObjectBuilder_Base objBuilder = entity.GetObjectBuilder(getExactCopy);
                        Debug.Assert(objBuilder != null, "Save flag specified returns nullable objectbuilder");
                        physObjectBuilders.Add(objBuilder);
                    }

                    // recurse
                    var childrenObjectBuilders = GetObjectBuilders(entity.Children, getExactCopy, entitiesToSave);
                    if (childrenObjectBuilders != null) physObjectBuilders.AddRange(childrenObjectBuilders);
                }
            }

            return physObjectBuilders;
        }


        private static bool TestRigidBody(MyRBElement element, bool assert = true)
        {
            if (element == null)
                return false;
            MyEntities.CollisionsElements.Clear();
            MyEntities.GetCollisionListForElement(element);
            if (MyEntities.CollisionsElements.Count != 0)
            {
                //TODO: This does not works well too, log is full of false asserts!
                /*
                MyMwcLog.WriteLine("Collision when inserting object");
                MyMwcLog.IncreaseIndent();
                MyMwcLog.WriteLine(element.ToString() + " at " + element.GetGlobalTransformation().Translation.ToString());
                MyEntity entity = ((MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                MyMwcLog.WriteLine(entity.ToString());
                MyMwcLog.DecreaseIndent();
                MyMwcLog.WriteLine("with");
                MyMwcLog.IncreaseIndent();
                foreach (MyRBElement dbg_element in MyEntities.CollisionsElements)
                {
                    MyEntity dbg_entity = ((MyPhysicsBody)dbg_element.GetRigidBody().m_UserData).Entity;
                    MyMwcLog.WriteLine(dbg_entity.ToString() + " at " + dbg_entity.GetPosition().ToString());
                    //add as many more lines as needed
                }
                MyMwcLog.DecreaseIndent();
                  */
                //TODO: This does not works well. Put large ship weapon on some prefab and you get false asserts!
                //Debug.Fail("Attempt to add Element failed due to the initial collision interaction. See log for details.");

                return true;
            }

            return false;
        }

        public static bool TestEntityAfterInsertionForCollision(MyEntity thisEntity, bool assert = true)
        {
            bool res = false;
            if (thisEntity.Physics != null)
            {
                foreach (MyRBElement element in (thisEntity.Physics as MyPhysicsBody).GetRBElementList())
                    res |= TestRigidBody(element, assert);
            }
            foreach (MyEntity entity in thisEntity.Children)
            {
                res |= TestEntityAfterInsertionForCollision(entity, assert);
            }

            return res;
        }

        static void ReleaseMaskOnClose(MyEntity obj, MyGroupMask mask)
        {
            obj.OnClose += new Action<MyEntity>((e) => MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().PushBackGroupMask(mask));
        }

        public static MyEntity CreateFromObjectBuilder(string hudLabelText, MyMwcObjectBuilder_Base objectBuilder, Matrix matrix)
        {
            switch (objectBuilder.GetObjectBuilderType())
            {
                case MyMwcObjectBuilderTypeEnum.SmallShip_Player:
                    {
                        MyMwcObjectBuilder_SmallShip_Player objectBuilderEx = (MyMwcObjectBuilder_SmallShip_Player)objectBuilder;
                        objectBuilderEx.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
                        objectBuilderEx.EntityId = MyEntityIdentifier.AllocatePlayershipId().NumericValue; // Get unused entity id
                        MySmallShip newObject = new MySmallShip();
                        MySession.PlayerShip = newObject;
                        MyGuiScreenGamePlay.Static.ControlledShip = MySession.PlayerShip;
                        newObject.Init(objectBuilderEx.DisplayName, objectBuilderEx);
                        newObject.OnDie += new DieHandler(onPlayerDie);

                        if (MySession.Static != null && MySession.Static.Player != null)
                        {
                            MySession.Static.Player.Ship = newObject;

                            if (newObject.ShipType == MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV)
                            {
                                MySteamStats.SetAchievement(MySteamAchievementNames.ShipStanislav);
                            }
                        }

                        if (MySession.PlayerFriends != null)
                        {
                            MyGroupMask mask = new MyGroupMask();
                            var result = MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().GetGroupMask(ref mask);
                            Debug.Assert(result, "Out of masks");
                            //newObject.Physics.GroupMask |= mask;
                            //ReleaseMaskOnClose(newObject, mask);
                            //MySession.PlayerFriends.FriendMask = mask;

                            foreach (var friend in MySession.PlayerFriends.GetDebug())
                            {
                                MySmallShipBot bot = friend as MySmallShipBot;
                                if (bot != null && bot.Leader != null)
                                {   //Suppose that player lead changed
                                    bot.Follow(MySession.Static.Player.Ship);

                                }
                            }
                        }
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.SmallShip:
                    {
                        MyMwcObjectBuilder_SmallShip objectBuilderEx = (MyMwcObjectBuilder_SmallShip)objectBuilder;
                        objectBuilderEx.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
                        MySmallShip newObject = new MySmallShip();
                        //newObject.Init(hudLabelText, objectBuilderEx, matrix, 1.0f);
                        newObject.Init(objectBuilderEx.DisplayName, objectBuilderEx);
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.SmallShip_Bot:
                    {
                        MyMwcObjectBuilder_SmallShip_Bot objectBuilderEx = (MyMwcObjectBuilder_SmallShip_Bot)objectBuilder;
                        objectBuilderEx.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
                        objectBuilderEx.ShipHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
                        MySmallShipBot newObject = new MySmallShipBot();
                        // !!! we must detect faction status with player by another method
                        //if (string.IsNullOrEmpty(objectBuilderEx.DisplayName))
                        //{
                        //    if (string.IsNullOrEmpty(hudLabelText))
                        //    {
                        //        if (MySession.PlayerShip != null)
                        //        {
                        //            var relationToPlayer = MyFactions.GetFactionsRelation(MySession.PlayerShip.Faction, objectBuilderEx.Faction);
                        //            switch (relationToPlayer)
                        //            {
                        //            case MyFactionRelationEnum.Neutral:
                        //                objectBuilderEx.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Neutral).ToString();
                        //                break;
                        //            case MyFactionRelationEnum.Friend:
                        //                objectBuilderEx.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Friend).ToString();
                        //                break;
                        //            case MyFactionRelationEnum.Enemy:
                        //                objectBuilderEx.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Enemy).ToString();
                        //                break;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        objectBuilderEx.DisplayName = hudLabelText;
                        //    }                            
                        //}
                        if (string.IsNullOrEmpty(objectBuilderEx.DisplayName))
                        {
                            if (!string.IsNullOrEmpty(hudLabelText))
                            {
                                objectBuilderEx.DisplayName = hudLabelText;
                            }
                        }

                        //newObject.Init(hudLabelText, objectBuilderEx, matrix, 1.0f);
                        newObject.Init(objectBuilderEx.DisplayName, objectBuilderEx);

                        // disable because 0004748: Disable all HUD names for generic Friend, Enemy, Neutral
                        //if (string.IsNullOrEmpty(objectBuilderEx.DisplayName)) 
                        //{
                        //    if (MySession.PlayerShip != null)
                        //    {
                        //        var relationToPlayer = MyFactions.GetFactionsRelation(MySession.PlayerShip, newObject);
                        //        switch (relationToPlayer)
                        //        {
                        //            case MyFactionRelationEnum.Neutral:
                        //                newObject.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Neutral).ToString();
                        //                break;
                        //            case MyFactionRelationEnum.Friend:
                        //                newObject.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Friend).ToString();
                        //                break;
                        //            case MyFactionRelationEnum.Enemy:
                        //                newObject.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Enemy).ToString();
                        //                break;
                        //        }
                        //    }
                        //}
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.Drone:
                    {
                        var droneObjectBuilder = (MyMwcObjectBuilder_Drone)objectBuilder;
                        droneObjectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
                        //droneObjectBuilder.ShipHealth = MyGameplayConstants.MAXHEALTH_DRONE;
                        var drone = new MyDrone();

                        StringBuilder displayName;
                        if (!string.IsNullOrEmpty(droneObjectBuilder.DisplayName))
                        {
                            displayName = new StringBuilder(droneObjectBuilder.DisplayName);
                        }
                        else
                        {
                            var defaultName = MyTextsWrapper.Get(MyTextsWrapperEnum.Drone);
                            displayName = string.IsNullOrEmpty(hudLabelText) ? defaultName : new StringBuilder(hudLabelText);
                        }

                        drone.Init(displayName, droneObjectBuilder);
                        drone.Health = MyGameplayConstants.MAXHEALTH_DRONE;
                        return drone;
                    }
                case MyMwcObjectBuilderTypeEnum.LargeShip:
                    {
                        MyMwcObjectBuilder_LargeShip objectBuilderEx = (MyMwcObjectBuilder_LargeShip)objectBuilder;
                        objectBuilderEx.PositionAndOrientation = new MyMwcPositionAndOrientation(matrix);
                        MyPrefabContainer container = new MyPrefabContainer();
                        MyMwcObjectBuilder_PrefabContainer prefabContOB = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabContainer, (int?)MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE) as MyMwcObjectBuilder_PrefabContainer;
                        prefabContOB.Faction = MyMwcObjectBuilder_FactionEnum.None;
                        prefabContOB.Prefabs = new List<MyMwcObjectBuilder_PrefabBase>();
                        container.Init(null, prefabContOB, matrix);
                        // MyEntities.Add(container);

                        MyPrefabLargeShip newObject = new MyPrefabLargeShip(container);
                        MyMwcObjectBuilder_PrefabLargeShip newObjectBuilder = new MyMwcObjectBuilder_PrefabLargeShip(MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_KAI, MyMwcObjectBuilder_Prefab_AppearanceEnum.None, new MyMwcVector3Short(0, 0, 0), Vector3.Zero, null, MyGameplayConstants.HEALTH_RATIO_MAX, null, 0, false, 0);
                        newObject.Init("KAI Mothership", Vector3.Zero, Matrix.Identity, newObjectBuilder,
                            MyPrefabConstants.GetPrefabConfiguration(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_KAI));
                        container.AddPrefab(newObject);
                        container.Faction = MyMwcObjectBuilder_FactionEnum.Euroamerican;
                        container.WorldMatrix = Matrix.CreateWorld(objectBuilderEx.PositionAndOrientation.Position, objectBuilderEx.PositionAndOrientation.Forward, objectBuilderEx.PositionAndOrientation.Up);
                        // newObject.WorldMatrix = container.WorldMatrix;

                        return container;
                    }

                case MyMwcObjectBuilderTypeEnum.SmallDebris:
                    {
                        MyMwcObjectBuilder_SmallDebris objectBuilderEx = (MyMwcObjectBuilder_SmallDebris)objectBuilder;
                        MySmallDebris newObject = new MySmallDebris();
                        newObject.Init(hudLabelText, objectBuilderEx, matrix);
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.LargeDebrisField:
                    {
                        MyMwcObjectBuilder_LargeDebrisField objectBuilderEx = (MyMwcObjectBuilder_LargeDebrisField)objectBuilder;
                        MyLargeDebrisField newObject = new MyLargeDebrisField();
                        newObject.Init(hudLabelText, objectBuilderEx, matrix);
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.Meteor:
                    {
                        MyMwcObjectBuilder_Meteor objectBuilderEx = (MyMwcObjectBuilder_Meteor)objectBuilder;
                        MyMeteor newObject = new MyMeteor();
                        newObject.Init(hudLabelText, objectBuilderEx, matrix);

                        if (objectBuilderEx.Direction != Vector3.Zero)
                        {
                            newObject.Start(objectBuilderEx.Direction, objectBuilderEx.EffectID);
                        }
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.StaticAsteroid:
                    {
                        MyMwcObjectBuilder_StaticAsteroid objectBuilderEx = (MyMwcObjectBuilder_StaticAsteroid)objectBuilder;
                        MyStaticAsteroid newObject = new MyStaticAsteroid();
                        newObject.Init(hudLabelText, objectBuilderEx, matrix);
                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.VoxelMap:
                    {
                        MyPerformanceTimer.VoxelLoad.Start();

                        MyMwcObjectBuilder_VoxelMap objectBuilderEx = (MyMwcObjectBuilder_VoxelMap)objectBuilder;
                        Vector3 voxelPosition = objectBuilderEx.PositionAndOrientation.Position;

                        MyVoxelMap newObject = new MyVoxelMap();

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel map init");
                        newObject.Init(hudLabelText, voxelPosition, objectBuilderEx);
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        MyPerformanceTimer.VoxelContentMerge.Start();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel content merge");
                        for (int i = 0; i < objectBuilderEx.MergeContents.Count; i++)
                        {
                            MyMwcObjectBuilder_VoxelMap_MergeContent voxelMapMergeContentObjectBuilder = objectBuilderEx.MergeContents[i];
                            if (voxelMapMergeContentObjectBuilder != null)
                            {
                                newObject.MergeVoxelContents(
                                    voxelMapMergeContentObjectBuilder.VoxelFile,
                                    voxelMapMergeContentObjectBuilder.PositionInVoxelMapInVoxelCoords,
                                    voxelMapMergeContentObjectBuilder.MergeType);
                            }
                        }
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MyPerformanceTimer.VoxelContentMerge.End();

                        MyPerformanceTimer.VoxelMaterialMerge.Start();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel material merge");
                        for (int i = 0; i < objectBuilderEx.MergeMaterials.Count; i++)
                        {
                            MyMwcObjectBuilder_VoxelMap_MergeMaterial voxelMapMergeMaterialObjectBuilder = objectBuilderEx.MergeMaterials[i];
                            newObject.MergeVoxelMaterials(
                                voxelMapMergeMaterialObjectBuilder.VoxelFile,
                                voxelMapMergeMaterialObjectBuilder.PositionInVoxelMapInVoxelCoords,
                                voxelMapMergeMaterialObjectBuilder.VoxelMaterial);
                        }
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MyPerformanceTimer.VoxelMaterialMerge.End();

                        MyPerformanceTimer.VoxelHandLoad.Start();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel hand load");
                        if (objectBuilderEx.EntityId.HasValue)
                        {
                            MyLoadingPerformance.Instance.AddVoxelHandCount(objectBuilderEx.VoxelHandShapes.Count, objectBuilderEx.EntityId.Value, MyVoxelFiles.DefaultVoxelFiles[(int)objectBuilderEx.VoxelFile].VoxelName);


                        }
                        for (int i = 0; i < objectBuilderEx.VoxelHandShapes.Count; i++)
                        {
                            MyMwcObjectBuilder_VoxelHand_Shape voxelHandShapeBuilder = objectBuilderEx.VoxelHandShapes[i];
                            switch (voxelHandShapeBuilder.GetObjectBuilderType())
                            {
                                case MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere:
                                    {
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel hand sphere");
                                        MyMwcObjectBuilder_VoxelHand_Sphere voxelHandSphereBuilder = (MyMwcObjectBuilder_VoxelHand_Sphere)voxelHandShapeBuilder;
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("new MyVoxelHandSphere()");
                                        MyVoxelHandSphere voxelHandSphere = new MyVoxelHandSphere();
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("voxelHandSphere.Init");
                                        voxelHandSphere.Init(voxelHandSphereBuilder, newObject);
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("newObject.AddVoxelHandShape");
                                        newObject.AddVoxelHandShape(voxelHandSphere, false);
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                        break;
                                    }
                                case MyMwcObjectBuilderTypeEnum.VoxelHand_Box:
                                    {
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel hand box");
                                        MyMwcObjectBuilder_VoxelHand_Box voxelHandboxBuilder = (MyMwcObjectBuilder_VoxelHand_Box)voxelHandShapeBuilder;
                                        MyVoxelHandBox voxelHandBox = new MyVoxelHandBox();
                                        voxelHandBox.Init(voxelHandboxBuilder, newObject);
                                        newObject.AddVoxelHandShape(voxelHandBox, false);
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                        break;
                                    }
                                case MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid:
                                    {
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Voxel hand cuboid");
                                        MyMwcObjectBuilder_VoxelHand_Cuboid voxelHandCuboidBuilder = (MyMwcObjectBuilder_VoxelHand_Cuboid)voxelHandShapeBuilder;
                                        MyVoxelHandCuboid voxelHandCuboid = new MyVoxelHandCuboid();
                                        voxelHandCuboid.Init(voxelHandCuboidBuilder, newObject);
                                        newObject.AddVoxelHandShape(voxelHandCuboid, false);
                                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                        break;
                                    }
                                default:
                                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                            }
                        }
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        MyPerformanceTimer.VoxelHandLoad.End();
                        MyPerformanceTimer.VoxelLoad.End();

                        // Dont use now
                        //newObject.OptimizeSize();

                        newObject.WriteDebugInfo();

                        return newObject;
                    }
                case MyMwcObjectBuilderTypeEnum.PrefabContainer:
                    {
                        MyMwcObjectBuilder_PrefabContainer objectBuilderEx = (MyMwcObjectBuilder_PrefabContainer)objectBuilder;
                        MyPrefabContainer container = new MyPrefabContainer();
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Prefab container - Init");
                        container.Init(null, objectBuilderEx, matrix);
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        return container;
                    }
                case MyMwcObjectBuilderTypeEnum.InfluenceSphere:
                    {
                        var builder = (MyMwcObjectBuilder_InfluenceSphere)objectBuilder;

                        var influenceSphere = new MyInfluenceSphere();
                        influenceSphere.Init(null, builder, matrix);
                        return influenceSphere;
                    }
                case MyMwcObjectBuilderTypeEnum.SpawnPoint:
                    {
                        MyMwcObjectBuilder_SpawnPoint objectBuilderEx = (MyMwcObjectBuilder_SpawnPoint)objectBuilder;
                        MySpawnPoint spawnPoint = new MySpawnPoint();
                        spawnPoint.Init(null, objectBuilderEx, matrix);
                        return spawnPoint;
                    }
                case MyMwcObjectBuilderTypeEnum.FoundationFactory:
                    {
                        MyMwcObjectBuilder_FoundationFactory objectBuilderEx = (MyMwcObjectBuilder_FoundationFactory)objectBuilder;
                        MyMwcObjectBuilder_PrefabFoundationFactory ffBuilder =
                            MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory,
                                                                    (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT) as MyMwcObjectBuilder_PrefabFoundationFactory;
                        ffBuilder.BuildingObject = objectBuilderEx.BuildingObject;
                        ffBuilder.BuildingQueue = objectBuilderEx.BuildingQueue;
                        ffBuilder.BuildingTimeFromStart = objectBuilderEx.BuildingTimeFromStart;
                        ffBuilder.IsBuilding = objectBuilderEx.IsBuilding;
                        ffBuilder.PersistentFlags = objectBuilderEx.PersistentFlags;
                        ffBuilder.PrefabHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
                        ffBuilder.PrefabMaxHealth = null;

                        MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = objectBuilderEx.PrefabContainer;
                        if (prefabContainerBuilder.Prefabs == null)
                        {
                            prefabContainerBuilder.Prefabs = new List<MyMwcObjectBuilder_PrefabBase>();
                        }
                        prefabContainerBuilder.Prefabs.Add(ffBuilder);

                        MyPrefabContainer prefabContainer = new MyPrefabContainer();

                        prefabContainer.Init(null, objectBuilderEx.PrefabContainer, matrix);

                        return prefabContainer;
                    }
                case MyMwcObjectBuilderTypeEnum.WaypointNew:
                    {
                        MyMwcObjectBuilder_WaypointNew waypointOB = (MyMwcObjectBuilder_WaypointNew)objectBuilder;
                        var waypoint = new MyWayPoint();
                        waypoint.Init(hudLabelText, waypointOB, matrix);
                        return waypoint;
                    }
                case MyMwcObjectBuilderTypeEnum.DummyPoint:
                    {
                        MyMwcObjectBuilder_DummyPoint dummyOB = (MyMwcObjectBuilder_DummyPoint)objectBuilder;
                        MyDummyPoint dummy = new MyDummyPoint();
                        dummy.Init(hudLabelText, dummyOB, matrix);
                        return dummy;
                    }
                case MyMwcObjectBuilderTypeEnum.CargoBox:
                    {
                        MyMwcObjectBuilder_CargoBox cargoBoxOB = (MyMwcObjectBuilder_CargoBox)objectBuilder;
                        MyCargoBox cargoBox = new MyCargoBox();
                        cargoBox.Init(string.IsNullOrEmpty(cargoBoxOB.DisplayName) ? hudLabelText : cargoBoxOB.DisplayName, cargoBoxOB, matrix);
                        return cargoBox;
                    }
                case MyMwcObjectBuilderTypeEnum.MysteriousCube:
                    {
                        MyMwcObjectBuilder_MysteriousCube mysteriousBoxOB = (MyMwcObjectBuilder_MysteriousCube)objectBuilder;
                        MyMysteriousCube mysteriousBox = new MyMysteriousCube();
                        mysteriousBox.Init(hudLabelText, mysteriousBoxOB, matrix);
                        return mysteriousBox;
                    }
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        static void onPlayerDie(MyEntity entity, MyEntity killer)
        {
            MinerWars.AppCode.Game.HUD.MyHudNotification.ClearOwnerNotifications(entity);
            MyHudWarnings.Remove(entity);
        }

        //  Create instance and add it into list of phys objects, if it is JLX phys object, otherwise add it to different lists
        //  based on the type of created phys object
        public static MyEntity CreateFromObjectBuilderAndAdd(string hudLabelText, MyMwcObjectBuilder_Base objectBuilder, Matrix matrix)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock(objectBuilder.GetType().Name);

            MyEntity retVal = CreateFromObjectBuilder(hudLabelText, objectBuilder, matrix);

            if (MyFakes.TEST_STORY_MISSION_OBJECTS_AT_SECTOR_BORDER_FOR_LARGE_POSITION_TEST)
            {
                Vector3 position = retVal.GetPosition();
                retVal.MoveAndRotate(MyFakes.TEST_STORY_MISSION_OBJECTS_LARGE_POSITION_OFFSET + position, retVal.WorldMatrix);
            }

            Add(retVal);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return retVal;
        }

        //  Allows you to iterate through all phys. objects
        public static HashSet<MyEntity> GetEntities()
        {
            return m_entities;
        }

        public static MyEntity GetEntityById(MyEntityIdentifier entityId)
        {
            return MyEntityIdentifier.GetEntityById(entityId);
        }

        public static bool ExistsById(MyEntityIdentifier entityId)
        {
            return MyEntityIdentifier.ExistsById(entityId);
        }

        public static MyEntity GetEntityByIdOrNull(MyEntityIdentifier entityId)
        {
            return MyEntityIdentifier.GetEntityByIdOrNull(entityId);
        }

        public static bool TryGetEntityById(MyEntityIdentifier entityId, out MyEntity entity)
        {
            return MyEntityIdentifier.TryGetEntity(entityId, out entity);
        }

        public static bool TryGetEntityById<T>(MyEntityIdentifier entityId, out T entity)
            where T : MyEntity
        {
            MyEntity baseEntity;
            var result = MyEntityIdentifier.TryGetEntity(entityId, out baseEntity) && baseEntity is T;
            entity = baseEntity as T;
            return result;
        }

        public static MyEntity GetEntityByName(string name)
        {
            return m_entityNameDictionary[name];
        }

        public static bool TryGetEntityByName(string name, out MyEntity entity)
        {
            return m_entityNameDictionary.TryGetValue(name, out entity);
        }

        public static MyEntity GetEntityByMissionLocationIdentifier(MyMissionBase.MyMissionLocationEntityIdentifier missionLocationIdentifier)
        {
            if (missionLocationIdentifier.LocationEntityId != null)
            {
                return GetEntityByIdOrNull(new MyEntityIdentifier(missionLocationIdentifier.LocationEntityId.Value));
            }
            else
            {
                MyEntity entity;
                TryGetEntityByName(missionLocationIdentifier.LocationEntityName, out entity);
                return entity;
            }
        }

        public static bool EntityExists(string name)
        {
            return m_entityNameDictionary.ContainsKey(name);
        }

        /// <summary>
        /// Gets the entities by predicate
        /// </summary>
        /// <param name="predicate">The predicate filter.</param>
        /// <param name="result">The result list which will be populated.</param>
        /// <returns>true if some entity was found.</returns>
        public static bool FindEntities(Predicate<MyEntity> predicate, IList<MyEntity> result)
        {
            bool added = false;

            foreach (var myEntity in m_entities)
            {
                if (!predicate(myEntity))
                {
                    continue;
                }

                result.Add(myEntity);
                added = true;
            }

            return added;
        }

        /// <summary>
        /// Gets the entities by predicate (recursively traverse childrens if current entity is not matched)
        /// atm. constrained to prefab container
        /// </summary>
        /// <param name="predicate">The predicate filter.</param>
        /// <param name="result">The result list which will be populated.</param>
        /// <returns>true if some entity was found.</returns>
        public static bool FindEntitiesRecursive(Predicate<MyEntity> predicate, IList<MyEntity> result)
        {
            int oldCount = result.Count;
            foreach (var myEntity in m_entities)
            {
                if (!predicate(myEntity))
                {
                    if (myEntity is MyPrefabContainer)
                    {
                        FindEntitiesRecursive(myEntity.Children, predicate, result);
                    }
                    continue;
                }

                result.Add(myEntity);
            }

            return result.Count > oldCount;
        }

        /// <summary>
        /// Gets the entities by predicate from entity collection
        /// </summary>
        private static void FindEntitiesRecursive(ICollection<MyEntity> entities, Predicate<MyEntity> predicate, IList<MyEntity> result)
        {
            foreach (var myEntity in entities)
            {
                if (!predicate(myEntity))
                {
                    FindEntitiesRecursive(myEntity.Children, predicate, result);
                    continue;
                }

                result.Add(myEntity);
            }
        }

        /// <summary>
        /// Traverse whole hierarchy of entities and invokes action on each one of them
        /// </summary>
        /// <param name="action"></param>
        public static void TraverseEntities(Action<MyEntity> action)
        {
            //foreach (var item in m_entities)
            PrepareSafeIterationHelperForAll();
            foreach (var item in m_safeIterationHelper)
            {
                TraverseEntitiesRecursive(item, action);
            }
        }

        private static void TraverseEntitiesRecursive(MyEntity entity, Action<MyEntity> action)
        {
            action(entity);
            foreach (var item in entity.Children)
            {
                TraverseEntitiesRecursive(item, action);
            }
        }



        /// <summary>
        /// This method unprojects mouse cursor to 3D space and checks intersection with line that is created from nearpoint and farpoint.
        /// Method used for intersection returns triangle, that is nearest from all.
        /// </summary>
        /// <returns></returns>
        public static MyEntity GetEntityUnderMouseCursor()
        {
            MyLine mouseSelectionLine = MyUtils.ConvertMouseToLine();
            //return GetAllIntersectionWithLine(ref mouseSelectionLine, null, null, true);
            return Render.MyRender.GetClosestIntersectionWithLine(ref mouseSelectionLine, null, null);
        }

        // IMPORTANT!! - carefull with this method, it can be called only from within Draw method!!!
        // This method can be usefull in case, that we need to reload during gameplay(because by default, LoadInDraw is
        // called from MyGuiScreenLoading screen on gameplay screen)
        public static void ReloadModelsInDraw()
        {
            foreach (var entity in m_entities)
            {
                entity.ModelLod0.LoadInDraw();
                if (entity.ModelLod1 != null)
                {
                    entity.ModelLod1.LoadInDraw();
                }
            }
        }

        //  Helper method to check if object is voxel map
        public static bool IsObjectVoxelMap(MyEntity physObject)
        {
            if (physObject is MyVoxelMap)
            {
                return true;
            }
            return false;
        }

        //  Helper method to retrieve center position of multiple phys objects
        public static Vector3 GetEntitiesCenter(List<MyEntity> physObjects)
        {
            Vector3 center = Vector3.Zero;
            int i = 0;
            foreach (MyEntity physObject in physObjects)
            {
                center = center + physObject.GetPosition();
                i++;
            }

            return Vector3.Divide(center, i);
        }

        //  Try if there is physObject having coordinate of provided position and if yes, return it
        public static MyEntity TryGetPhysObjectAtPosition(Vector3 position)
        {
            foreach (var entity in m_entities)
            {
                if (entity.Physics == null)
                {
                    continue;
                }

                if (entity.GetPosition() != position)
                {
                    continue;
                }

                return entity;
            }
            return null;
        }

        //  Method returns all collision skins intersecting provided bounding box - see method used inside
        public static void GetCollisionsInBoundingBox(ref BoundingBox boundingBox, List<MyRBElement> list)
        {
            m_pruningStructure.OverlapAllBoundingBox(ref boundingBox, list, (uint)MyElementFlag.EF_RB_ELEMENT);
        }

        public static void RaiseEntityRemove(MyEntity entity)
        {
            if (OnEntityRemove != null)
            {
                OnEntityRemove(entity);
            }
        }

        #region Global visibility and selectability by entity types/groups

        /// <summary>
        /// Types in this set and their subtypes won't be able to be selected.
        /// </summary>
        private static HashSet<Type> m_unselectableTypes = new HashSet<Type>();

        public static void SetTypeSelectable(Type type, bool selectable)
        {
            if (selectable)
                m_unselectableTypes.Remove(type);
            else
                m_unselectableTypes.Add(type);
        }

        public static bool IsTypeSelectable(Type type)
        {
            foreach (var unselectableType in m_unselectableTypes)
                if (unselectableType.IsAssignableFrom(type))
                    return false;
            return !IsTypeHidden(type);  // hidden entities aren't selectable
        }

        public static bool IsSelectable(MyEntity entity)
        {
            // Didn't you want to override MyEntity.IsSelectable instead? Thought so.
            return IsTypeSelectable(entity.GetType()) && (entity.Flags & MyEntity.EntityFlags.EditableInEditor) > 0;
        }


        /// <summary>
        /// Types in this set and their subtypes will be temporarily invisible.
        /// </summary>
        private static HashSet<Type> m_hiddenTypes = new HashSet<Type>();

        public static void SetTypeHidden(Type type, bool hidden)
        {
            if (hidden == m_hiddenTypes.Contains(type)) return;  // no change

            if (hidden)
                m_hiddenTypes.Add(type);
            else
                m_hiddenTypes.Remove(type);
        }

        public static bool IsTypeHidden(Type type)
        {
            foreach (var hiddenType in m_hiddenTypes)
                if (hiddenType.IsAssignableFrom(type))
                    return true;
            return false;
        }

        public static bool IsVisible(MyEntity entity)
        {
            return !IsTypeHidden(entity.GetType());
        }

        public static void UnhideAllTypes()
        {
            foreach (var type in m_hiddenTypes.ToList())
                SetTypeHidden(type, false);
        }

        public static bool SafeAreasHidden, SafeAreasSelectable;
        public static bool DetectorsHidden, DetectorsSelectable;
        public static bool ParticleEffectsHidden, ParticleEffectsSelectable;

        public static bool ShowDebugDrawStatistics = false;
        static Dictionary<string, int> m_typesStats = new Dictionary<string, int>();

        #endregion

        static public void DebugDrawStatistics()
        {/*
            foreach (MyEntity entity in m_entities)
            {
                if (entity is MyPrefabContainer)
                {
                    entity.DebugDraw();
                }
            }
*/
            /*
           foreach (MyEntity e in GetEntities())
           {
               MyDummyPoint dp = e as MyDummyPoint;
               if (dp != null && ((int)(dp.DummyFlags & MyDummyPointFlags.PARTICLE) > 0))
               {
                   MyDebugDraw.DrawSphereSmooth(dp.GetPosition(), dp.Radius, Vector3.One, 0.4f);
               }
           }
              */


            m_typesStats.Clear();

            if (!ShowDebugDrawStatistics)
                return;

            foreach (MyEntity entity in m_entitiesForUpdate)
            {
                string ts = entity.GetType().Name.ToString();
                if (!m_typesStats.ContainsKey(ts))
                    m_typesStats.Add(ts, 0);
                m_typesStats[ts]++;
            }

            Vector2 offset = new Vector2(100, 0);
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Detailed entity statistics"), Color.Yellow, 2);

            float scale = 0.7f;
            offset.Y += 50;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Entities for update:"), Color.Yellow, scale);
            offset.Y += 30;
            foreach (KeyValuePair<string, int> pair in Render.MyRender.SortByValue(m_typesStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.ToString() + "x"), Color.Yellow, scale);
                offset.Y += 20;
            }


            m_typesStats.Clear();

            foreach (MyEntity entity in m_entities)
            {
                string ts = entity.GetType().Name.ToString();
                if (!m_typesStats.ContainsKey(ts))
                    m_typesStats.Add(ts, 0);
                m_typesStats[ts]++;
            }

            offset = new Vector2(500, 0);
            scale = 0.7f;
            offset.Y += 50;
            MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("All entities:"), Color.Yellow, scale);
            offset.Y += 30;
            foreach (KeyValuePair<string, int> pair in Render.MyRender.SortByValue(m_typesStats))
            {
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(pair.Key + ": " + pair.Value.ToString() + "x"), Color.Yellow, scale);
                offset.Y += 20;
            }
        }

        [Conditional("DEBUG")]
        public static void DumpTurretIds()
        {
            List<MyEntityIdentifier?> ids = new List<MyEntityIdentifier?>(100);

            foreach (var container in GetEntities().OfType<MyPrefabContainer>())
            {
                foreach (var weapon in container.Children.OfType<MyPrefabLargeWeapon>())
                {
                    ids.Add(weapon.EntityId);
                }
            }

            ids.Sort(new Comparison<MyEntityIdentifier?>((a, b) => Comparer<uint?>.Default.Compare(a.ToNullableUInt(), b.ToNullableUInt())));
            Console.WriteLine("Dumping turret IDS");
            foreach (var id in ids)
            {
                Console.WriteLine(id);
            }
        }

        public static void DumpWrongEntities()
        {
            HashSet<string> alreadyTestedModels = new HashSet<string>();

            foreach (MyEntity entity in GetEntities())
            {
                if (entity.ModelLod0 != null)
                {
                    if (alreadyTestedModels.Add(entity.ModelLod0.AssetName))
                    {
                        entity.DumpErrorModels();
                    }
                }
            }
        }
    }
}

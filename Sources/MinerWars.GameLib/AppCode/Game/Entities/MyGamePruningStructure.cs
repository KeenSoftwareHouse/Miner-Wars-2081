using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    // For space queries on all entities (including children, invisible objects and objects without physics)
    static class MyGamePruningStructure
    {
        [Flags]
        internal enum QueryFlags: uint
        {
            Waypoints = 1u << 0,
            Others = 1u << 31,
            All = ~0u,
        }

        // A tree for each query type.
        // If you query for a specific type, consider adding a new QueryFlag and AABBTree (so that you don't have to filter the result afterwards).
        static MyDynamicAABBTree m_waypoints;
        static MyDynamicAABBTree m_others;

        static MyGamePruningStructure()
        {
            Init();
        }

        static void Init()
        {
            m_waypoints = new MyDynamicAABBTree(MyConstants.GAME_PRUNING_STRUCTURE_AABB_EXTENSION);
            m_others = new MyDynamicAABBTree(MyConstants.GAME_PRUNING_STRUCTURE_AABB_EXTENSION);
        }

        public static void Add(MyEntity entity)
        {
            if (entity.GamePruningProxyId != MyConstants.GAME_PRUNING_STRUCTURE_PROXY_ID_NOT_INSERTED) return;  // already inserted

            BoundingBox bbox = entity.WorldAABB;
            if (bbox.Size() == Vector3.Zero) return;  // don't add entities with zero bounding boxes

            if (entity is MyWayPoint)
                entity.GamePruningProxyId = m_waypoints.AddProxy(ref bbox, entity, 0);
            else
                entity.GamePruningProxyId = m_others.AddProxy(ref bbox, entity, 0);
        }

        public static void Remove(MyEntity entity)
        {
            if (entity.GamePruningProxyId == MyConstants.GAME_PRUNING_STRUCTURE_PROXY_ID_NOT_INSERTED) return;  // not inserted

            if (entity is MyWayPoint)
                m_waypoints.RemoveProxy(entity.GamePruningProxyId);
            else
                m_others.RemoveProxy(entity.GamePruningProxyId);

            entity.GamePruningProxyId = MyConstants.GAME_PRUNING_STRUCTURE_PROXY_ID_NOT_INSERTED;
        }

        public static void Clear()
        {
            Init();
            m_waypoints.Clear();
            m_others.Clear();
        }

        public static void Move(MyEntity entity)
        {
            if (entity.GamePruningProxyId == MyConstants.GAME_PRUNING_STRUCTURE_PROXY_ID_NOT_INSERTED) return;

            BoundingBox bbox = entity.WorldAABB;
            if (bbox.Size() == Vector3.Zero)  // remove entities with zero bounding boxes
            {
                Remove(entity);
                return;
            }

            if (entity is MyWayPoint)
                m_waypoints.MoveProxy(entity.GamePruningProxyId, ref bbox, Vector3.Zero);
            else
                m_others.MoveProxy(entity.GamePruningProxyId, ref bbox, Vector3.Zero);
        }

        public static List<MyEntity> GetAllEntitiesInBox(ref BoundingBox box, QueryFlags flags)
        {
            var result = new List<MyEntity>();

            if ((flags & QueryFlags.Waypoints) != 0)
                m_waypoints.OverlapAllBoundingBox(ref box, result, 0, false);
            if ((flags & QueryFlags.Others) != 0)
                m_others.OverlapAllBoundingBox(ref box, result, 0, false);
            
            return result;
        }
    }
}


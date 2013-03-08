using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelTasks;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.WayPoints;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using MinerWars.Resources;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.HUD
{
    class MyComputeGPSJob : IWork
    {
        private Vector3 m_startPos;
        private Vector3 m_goalPos;
        private MyEntity m_goalEntity;

        public List<Vector3> Path;
        public StringBuilder Message;

        public void Start(Vector3 startPos, Vector3 goalPos, MyEntity goalEntity)
        {
            m_startPos = startPos;
            m_goalPos = goalPos;
            m_goalEntity = goalEntity;
            Path = new List<Vector3>();
            Message = MyTextsWrapper.Get(MyTextsWrapperEnum.GPSNoPath);

            goalEntity.OnClose += goalEntity_OnClose;
        }

        void goalEntity_OnClose(MyEntity obj)
        {
            m_goalEntity = null;
        }

        public void DoWork()
        {
            try 
            {
                MyEntities.EntityCloseLock.AcquireShared();

                if (m_goalEntity == null)
                    return;
                // try the direct path
                {
                    var directLine = new MyLine(m_startPos, m_goalPos, true);
                    if (MyEntities.GetAnyIntersectionWithLine(ref directLine, m_goalEntity, null, true, true, true, false) == null)
                    {
                        Path.Add(m_startPos);
                        Path.Add(m_goalPos);
                        Message = new StringBuilder().AppendFormat(MyTextsWrapper.Get(MyTextsWrapperEnum.GPSDistance).ToString(), Vector3.Distance(m_startPos, m_goalPos));
                        return;
                    }
                }

                // get the closest waypoint to the goal (ignore visibility)
                MyWayPoint goal = MyWayPointGraph.GetClosestNonGeneratedWaypoint(m_goalPos);
                if (goal == null) return;

                // remember which waypoints were visible/invisible from startPos
                // remember blocked/unblocked edges
                var visibleFromStartPosCache = new Dictionary<MyWayPoint, bool>();
                //var blockedEdges = new HashSet<Tuple<MyWayPoint, MyWayPoint>>();
                HashSet<Tuple<MyWayPoint, MyWayPoint>> blockedEdges = null;
                
                using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
                {
                    blockedEdges = new HashSet<Tuple<MyWayPoint,MyWayPoint>>(MyWayPoint.BlockedEdgesForPlayer);
                }

                var unblockedEdges = new HashSet<Tuple<MyWayPoint, MyWayPoint>>();

                // get 7 closest visible waypoints to startPos and compute shortest paths from them
                
                // first try 7 closest
                var closestVisibleWaypoints = MyWayPointGraph.GetClosestVisibleWaypoints(m_startPos, m_goalEntity, 7, 7, visibleFromStartPosCache);

                if (closestVisibleWaypoints.Count == 0 || !FindPathBetweenWaypoints(closestVisibleWaypoints, goal, visibleFromStartPosCache, blockedEdges, unblockedEdges))
                {
                    // failure: try 50 closest
                    closestVisibleWaypoints = MyWayPointGraph.GetClosestVisibleWaypoints(m_startPos, m_goalEntity, 12, 50, visibleFromStartPosCache);

                    if (closestVisibleWaypoints.Count == 0 || !FindPathBetweenWaypoints(closestVisibleWaypoints, goal, visibleFromStartPosCache, blockedEdges, unblockedEdges))
                    {
                        return;  // no use
                    }
                }
            }
            finally
            {
                if (m_goalEntity != null)
                {
                    m_goalEntity.OnClose -= goalEntity_OnClose;
                }

                MyEntities.EntityCloseLock.ReleaseShared();
            }
        }

        private bool FindPathBetweenWaypoints(
            List<MyWayPoint> closestVisibleWaypoints,
            MyWayPoint goal,
            Dictionary<MyWayPoint, bool> visibleFromStartPosCache,
            HashSet<Tuple<MyWayPoint, MyWayPoint>> blockedEdges,
            HashSet<Tuple<MyWayPoint, MyWayPoint>> unblockedEdges
        )
        {
            // find the best path candidate
            float shortestPathLength = float.MaxValue;
            int retryCount = 0;

            for (int j = 0; j < closestVisibleWaypoints.Count && retryCount < 30; j++)  // max retry count is 30
            {
                var path = closestVisibleWaypoints[j].GetShortestPathTo(goal, blockedEdges, true, false);
                if (path.Count == 0) continue;

                // optimize the path: try to delete waypoints from the start of the path (if the next waypoints are visible)
                int farthestVisibleWaypointInPath = 0;

                for (int i = Math.Min(path.Count - 1, 20); i > 0; i--)  // path optimization: max 20 raycasts per path candidate, reuse previous results
                    if (path[i].IsVisibleFrom(m_startPos, m_goalEntity, visibleFromStartPosCache))
                    {
                        farthestVisibleWaypointInPath = i;
                        break;
                    }

                // compute the length of the path candidate
                float length = Vector3.Distance(m_startPos, path[farthestVisibleWaypointInPath].WorldMatrix.Translation) + Vector3.Distance(path[path.Count - 1].WorldMatrix.Translation, m_goalPos);
                for (int i = farthestVisibleWaypointInPath; i < path.Count - 1; i++)
                    length += Vector3.Distance(path[i].WorldMatrix.Translation, path[i + 1].WorldMatrix.Translation);

                // if it's the shortest path candidate yet, make it the new GPS path
                if (length < shortestPathLength)
                {
                    // but first check that the edges are free of any obstructions (and remember it)
                    bool pathBlocked = false;
                    for (int i = farthestVisibleWaypointInPath; i < path.Count - 2; i++)
                    {
                        // assume that edges between non-generated waypoints are always ok
                        if (path[i].Save && path[i + 1].Save)
                            continue;

                        var tuple = Tuple.Create(path[i], path[i + 1]);
                        if (unblockedEdges.Contains(tuple))  // already tested and it was ok
                        {
                        }
                        else if (path[i + 1].IsVisibleFrom(path[i].Position, m_goalEntity))
                        {
                            unblockedEdges.Add(tuple);
                            unblockedEdges.Add(Tuple.Create(path[i + 1], path[i]));
                        }
                        else
                        {
                            blockedEdges.Add(tuple);
                            blockedEdges.Add(Tuple.Create(path[i + 1], path[i]));
                            pathBlocked = true;
                            break;
                        }
                    }
                    if (pathBlocked)
                    {
                        j--;  // retry current waypoint with updated cache of blocked edges
                        retryCount++;
                        continue;
                    }

                    // seems legit
                    shortestPathLength = length;
                    Path = new List<Vector3>();
                    Path.Add(m_startPos);
                    for (int i = farthestVisibleWaypointInPath; i < path.Count; i++) Path.Add(path[i].WorldMatrix.Translation);
                    Path.Add(m_goalPos);
                }
            }

            if (Path.Count != 0)
            {
                Message = new StringBuilder().AppendFormat(MyTextsWrapper.Get(MyTextsWrapperEnum.GPSDistance).ToString(), shortestPathLength);
                return true;
            }
            else
            {
                return false;
            }
        }

        public WorkOptions Options
        {
            get { return new WorkOptions() { MaximumThreads = 1 }; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using MinerWarsMath;
using System.Collections.Concurrent;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Utils;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using ParallelTasks;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal delegate void PathFoundHandler(List<Vector3> path, object userData);

    static class MyPathfindingHelper
    {
        static Task m_findPathTask;
        static BackgroundTaskWork m_pathfindingHelper = new BackgroundTaskWork();

        class BackgroundTaskWork : IWork
        {
            public void DoWork()
            {
                MyPathfindingHelper.BackgroundTask();
            }

            public WorkOptions Options
            {
                get { return Parallel.DefaultOptions; }
            }
        }

        /// <summary>
        /// Finds a path using waypoints. Use whenever the direct path is blocked.
        /// </summary>
        /// <param name="from">First endpoint of the path. Will be duplicated in the returned path.</param>
        /// <param name="to">First endpoint of the path. Will be duplicated in the returned path.</param>
        /// <param name="pathFoundHandler">To be called after the pathfinder is finished. Will receive the found path, or null when there's none.</param>
        /// <param name="userData">An object that will be passed to pathFoundHandler. Can be used to identify the query.</param>
        /// <param name="useRaycasts">Whether the bot should use raycasts.</param>
        public static void FindPathInBackground(Vector3 from, Vector3 to, PathFoundHandler pathFoundHandler, object userData, bool useRaycasts)
        {
            MyCommonDebugUtils.AssertDebug(from != null && to != null && pathFoundHandler != null);
            m_queue.Enqueue(new PathToBeFound(from, to, pathFoundHandler, userData, useRaycasts));
            //m_event.Set();
            m_findPathTask = Parallel.Start(m_pathfindingHelper);
        }

        #region Implementation

        static MyPathfindingHelper()
        {
            m_queue = new ConcurrentQueue<PathToBeFound>();
            //m_event = new AutoResetEvent(false);
            //Task.Factory.StartNew(BackgroundTask, TaskCreationOptions.PreferFairness);
        }

        private struct PathToBeFound
        {
            public Vector3 From;
            public Vector3 To;
            public PathFoundHandler PathFoundHandler;
            public object UserData;
            public bool UseRaycasts;

            public PathToBeFound(Vector3 from, Vector3 to, PathFoundHandler pathFoundHandler, object userData, bool useRaycasts)
            {
                From = from;
                To = to;
                PathFoundHandler = pathFoundHandler;
                UserData = userData;
                UseRaycasts = useRaycasts;
            }
        }

        private static readonly ConcurrentQueue<PathToBeFound> m_queue;
        //private static readonly AutoResetEvent m_event;
        private static void BackgroundTask()
        {
            PathToBeFound next;
            if (m_queue.TryDequeue(out next))
                next.PathFoundHandler(FindPath(next.From, next.To, next.UseRaycasts), next.UserData);
        }

        private static List<Vector3> FindPath(Vector3 start, Vector3 end, bool useRaycasts)
        {
            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {
                var closestToStart = MyWayPointGraph.GetClosestWaypointReachableByShip(start, null, MyAIConstants.PATHFINDING_MAX_START_RAYCASTS, MyAIConstants.PATHFINDING_SHIP_RADIUS);
                if (closestToStart == null)
                    return null;

                var closestToEnd = MyWayPointGraph.GetClosestWaypointReachableByShip(end, null, MyAIConstants.PATHFINDING_MAX_END_RAYCASTS, MyAIConstants.PATHFINDING_SHIP_RADIUS);
                if (closestToEnd == null)
                    return null;

                if (useRaycasts)
                {
                    HashSet<Tuple<MyWayPoint, MyWayPoint>> blockedEdges = null;
                    HashSet<Tuple<MyWayPoint, MyWayPoint>> unblockedEdges = null;

                    using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
                    {
                        blockedEdges = new HashSet<Tuple<MyWayPoint, MyWayPoint>>(MyWayPoint.BlockedEdgesForBots);  // TODO: do we need to copy this every time?
                        unblockedEdges = new HashSet<Tuple<MyWayPoint, MyWayPoint>>();
                    }

                    for (int retryCount = 0; retryCount < 30; retryCount++)
                    {
                        var pathWaypoints = closestToStart.GetShortestPathTo(closestToEnd, blockedEdges, true, true);
                        if (pathWaypoints == null || pathWaypoints.Count == 0)
                            return null;  // no path (we can exit right away since we only *add* blocked edges)

                        bool blocked = false;
                        for (int i = 0; i < pathWaypoints.Count - 2; i++)
                        {
                            var tuple = Tuple.Create(pathWaypoints[i], pathWaypoints[i+1]);
                            if (unblockedEdges.Contains(tuple))  // already tested and it was ok
                            {
                            }
                            else if (pathWaypoints[i+1].IsVisibleFrom(pathWaypoints[i].Position))
                            {
                                unblockedEdges.Add(tuple);
                                unblockedEdges.Add(Tuple.Create(pathWaypoints[i+1], pathWaypoints[i]));
                            }
                            else
                            {
                                blockedEdges.Add(tuple);
                                blockedEdges.Add(Tuple.Create(pathWaypoints[i+1], pathWaypoints[i]));
                                blocked = true;
                                break;
                            }
                        }
                        if (blocked) continue;

                        var path = new List<Vector3>();
                        path.Add(start);
                        foreach (var waypoint in pathWaypoints) path.Add(waypoint.Position);
                        path.Add(end);
                        return path;
                    }
                    return null;  // no path found after many tries
                }
                else
                {
                    List<MyWayPoint> pathWaypoints = null;
                    
                    using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
                    {
                        pathWaypoints = closestToStart.GetShortestPathTo(closestToEnd, MyWayPoint.BlockedEdgesForBots, true, true);
                    }

                    if (pathWaypoints == null || pathWaypoints.Count == 0)
                        return null;  // no path

                    var path = new List<Vector3>();
                    path.Add(start);
                    foreach (var waypoint in pathWaypoints) path.Add(waypoint.Position);
                    path.Add(end);
                    return path;
                }

            }
        }

        public static void Unload()
        {
            PathToBeFound next;
            while (m_queue.TryDequeue(out next))
            {
            }
        }

        #endregion
    }
}

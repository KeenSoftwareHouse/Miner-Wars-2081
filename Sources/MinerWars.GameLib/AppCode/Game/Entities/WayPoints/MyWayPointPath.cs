using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.Entities.WayPoints
{
    // This is practically just a named list of waypoints.
    class MyWayPointPath
    {
        public List<MyWayPoint> WayPoints;
        public string Name { get; private set; }
        private List<MyWayPoint> m_cachedCompletePath;

        public MyWayPointPath(string name)
        {
            WayPoints = new List<MyWayPoint>();
            Name = name;
            m_cachedCompletePath = null;
            MyWayPointGraph.StoredPaths.Add(this);
            MyWayPointGraph.PathFromName.Add(name, this);
        }

        public void DeleteNullVertices()
        {
            if (WayPoints == null) return;

            var newWaypoints = new List<MyWayPoint>();
            for (int i = 0; i < WayPoints.Count; i++)
                if (WayPoints[i] != null)
                    newWaypoints.Add(WayPoints[i]);
            if (newWaypoints.Count != WayPoints.Count)
                WayPoints = newWaypoints;
        }

        public void InvalidateStoredPathCache()
        {
            m_cachedCompletePath = null;
        }

        // Return the complete shortest path between vertices in a list.
        public List<MyWayPoint> CompletePath(HashSet<Tuple<MyWayPoint, MyWayPoint>> blockedEdges, MyWayPoint currentWayPoint, bool useGeneratedWaypoints = true, bool cycle = false, bool cachedIsOk = true)
        {
            if (!cachedIsOk || m_cachedCompletePath == null)
            {
                using (MyEntities.EntityCloseLock.AcquireSharedUsing())
                {
                    m_cachedCompletePath = new List<MyWayPoint>();
                    if (WayPoints == null || WayPoints.Count == 0) return m_cachedCompletePath;

                    int currentIndex = currentWayPoint != null ? WayPoints.IndexOf(currentWayPoint) : 0;
                    m_cachedCompletePath.Add(WayPoints[0]);
                    //for (int i = 0; i < WayPoints.Count - 1; i++)
                    //{
                    //    var subpath = WayPoints[i].GetShortestPathTo(WayPoints[i + 1], blockedEdges, useGeneratedWaypoints);
                    //    if (subpath.Count == 0)
                    //        m_cachedCompletePath.Add(WayPoints[i + 1]);  // no path: add straight line to next waypoint
                    //    else
                    //        m_cachedCompletePath.AddRange(subpath.GetRange(1, subpath.Count - 1));  // the first point was the last point of the previous segment
                    //}
                    //if (cycle)
                    //{
                    //    var subpath = WayPoints[WayPoints.Count - 1].GetShortestPathTo(WayPoints[0], blockedEdges, useGeneratedWaypoints);
                    //    if (subpath.Count > 2)
                    //        m_cachedCompletePath.AddRange(subpath.GetRange(1, subpath.Count - 2));  // the first point was the last point of the previous segment, the last point was point 0
                    //}
                    for (int i = 0; i < WayPoints.Count - 1; i++)
                    {
                        var subpath = WayPoints[i].GetShortestPathTo(WayPoints[i + 1], blockedEdges, useGeneratedWaypoints);
                        if (subpath.Count == 0)
                        {
                            if (blockedEdges != null && blockedEdges.Contains(Tuple.Create(WayPoints[i], WayPoints[i + 1])))
                            {
                                // this part of the path is unreachable
                                if (currentIndex > i)
                                {
                                    m_cachedCompletePath.Clear();
                                    m_cachedCompletePath.Add(WayPoints[i + 1]);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                m_cachedCompletePath.Add(WayPoints[i + 1]);  // no path: add straight line to next waypoint
                            }
                        }
                        else
                        {
                            m_cachedCompletePath.AddRange(subpath.GetRange(1, subpath.Count - 1));  // the first point was the last point of the previous segment
                        }
                    }

                    if (cycle)
                    {
                        var subpath = WayPoints[WayPoints.Count - 1].GetShortestPathTo(WayPoints[0], blockedEdges, useGeneratedWaypoints);
                        if (subpath.Count > 2)
                            m_cachedCompletePath.AddRange(subpath.GetRange(1, subpath.Count - 2));  // the first point was the last point of the previous segment, the last point was point 0
                    }
                }
            }
            return m_cachedCompletePath;
        }

        public bool ContainsEdge(MyWayPoint v, MyWayPoint w)
        {
            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {
                for (int i = 0; i < WayPoints.Count - 1; i++)
                {
                    var subpath = WayPoints[i].GetShortestPathTo(WayPoints[i + 1]);
                    for (int j = 0; j < subpath.Count - 1; j++)
                        if ((subpath[j] == v && subpath[j + 1] == w) || (subpath[j] == w && subpath[j + 1] == v))
                            return true;
                }
            }
            return false;
        }

        // returns true if everything's ok, false if zero name or if there already is a path with this name
        public bool ChangeName(string newName)
        {
            if (newName == null || newName.Length == 0) return false;  // zero name
            if (newName == Name) return true;  // already the same
            if (MyWayPointGraph.PathFromName.ContainsKey(newName)) return false;  // duplicate

            MyWayPointGraph.PathFromName.Remove(Name);
            Name = newName;
            MyWayPointGraph.PathFromName.Add(newName, this);
            return true;
        }
    }
}

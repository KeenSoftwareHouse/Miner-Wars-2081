using System;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Editor;
using KeenSoftwareHouse.Library.Memory;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Prefabs;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Text;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Linq;
using MinerWars.AppCode.Game.Entities.Prefabs;
using SysUtils.Utils;
using BulletXNA;
using KeenSoftwareHouse.Library.Parallelization.Threading;


namespace MinerWars.AppCode.Game.Entities.WayPoints
{
    static class MyWayPointGraph
    {
        #region Vertices

        private static HashSet<MyWayPoint> m_vertices;
        private static FastResourceLock VerticesLock = new FastResourceLock();

        /// <summary>
        /// Make a copy of all waypoints.
        /// Please don't use this in time-critical code.
        /// </summary>
        public static List<MyWayPoint> GetCopyOfAllWaypoints()
        {
            var list = new List<MyWayPoint>();
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                    list.Add(v);
            }
            return list;
        }

        // Add a new vertex to the graph. Return true if the vertex was successfully added.
        // If you also want to add it into the world (MyEntities), use CreateWaypoint instead.
        public static bool AddVertex(MyWayPoint v)
        {
            MakeConnectedComponentsDirty();
            using (VerticesLock.AcquireExclusiveUsing())
            {
                System.Diagnostics.Debug.Assert(readingVertices == false);
                return m_vertices.Add(v);
            }
        }

        // Remove a vertex and all of its edges. Return true if the vertex was successfully removed from the graph.
        // Note that this function doesn't remove the vertex from the world (MyEntities): use MyWayPoint.Close for that.
        public static bool RemoveVertex(MyWayPoint v)
        {
            MakeConnectedComponentsDirty();
            v.DisconnectFromAllNeighbors();

            // delete vertex from all paths
            foreach (var path in StoredPaths)
                if (path.WayPoints.Contains(v))
                    path.WayPoints.Remove(v);
            RemovePathsWithZeroVertices();

            using (VerticesLock.AcquireExclusiveUsing())
            {
                System.Diagnostics.Debug.Assert(readingVertices == false);
                return m_vertices.Remove(v);
            }
        }

        public static void RemovePath(MyWayPointPath path)
        {
            if (path == SelectedPath) SelectedPath = null;
            StoredPaths.Remove(path);
            PathFromName.Remove(path.Name);
        }

        public static void RemovePathsWithZeroVertices()
        {
            for (int i = StoredPaths.Count - 1; i >= 0; i--)
                if (StoredPaths[i].WayPoints.Count == 0)
                    RemovePath(StoredPaths[i]);
        }

        // Reset all vertex search ids. Used when path searching overflows.
        public static void ResetAllVisitedSearchIds()
        {
            foreach (var v in m_vertices)
                v.ResetVisitedSearchId();
        }

        public static int CountPathsWithWaypointsWithParents()
        {
            int count = 0;
            foreach (var p in StoredPaths)
                foreach (var v in p.WayPoints)
                    if (v.Parent != null) { count++; break; }
            return count;
        }

        public static int CountWaypointsWithParents()
        {
            int count = 0;
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                    if (v.Parent != null) count++;
            }
            return count;
        }

        /// <summary>
        /// Remove and recreate all vertices with parents.
        /// Needs to be called from the main thread (because it uses MyEntities.GetSafeIterationHelperForAll).
        /// </summary>
        public static void ReloadAllWaypointsWithParents()
        {
            var waypointlessPrefabs = new HashSet<string>();  // collect waypointless prefabs with snappoints

            // remove
            foreach (var v in GetVertexListForModification())
                if (v.Parent != null)
                    v.Close();

            // recreate
            var recreatedPrefabs = new List<MyPrefabBase>();
            foreach (var entity in MyEntities.GetSafeIterationHelperForAll())
            {
                var waypoints = new List<MyWayPoint>();
                if (entity is MyPrefabBase)
                {
                    waypoints = (entity as MyPrefabBase).InitWaypoints();
                    if (waypoints.Count > 0)
                        recreatedPrefabs.Add(entity as MyPrefabBase);
                }
                else if (entity is MyPrefabContainer)
                {
                    foreach (var prefab in (entity as MyPrefabContainer).GetPrefabs())
                    {
                        var prefabWaypoints = (prefab as MyPrefabBase).InitWaypoints();
                        waypoints.AddRange(prefabWaypoints);
                        if (prefabWaypoints.Count > 0)
                            recreatedPrefabs.Add(prefab as MyPrefabBase);
                        /*
                        if (prefabWaypoints.Count == 0 && (prefab as MyPrefabBase).SnapPoints.Count > 0)
                        {
                            var p = (prefab as MyPrefabBase);
                            if (prefab is MyPrefab)
                                waypointlessPrefabs.Add((prefab as MyPrefab).PrefabType.ToString());  // add name
                            else
                                waypointlessPrefabs.Add((prefab as MyPrefabBase).PrefabId.ToString());  // just add the id number
                        }
                        */
                    }
                }
                // delete duplicate waypoints if any have remained
                foreach (var waypoint in waypoints)
                {
                    var old = GetClosestWaypointWithoutEdges(waypoint.Position);
                    if (old != null && old != waypoint && Vector3.DistanceSquared(old.Position, waypoint.Position) < 1e-5)
                        old.Close();
                }
            }

            // connect along snappoints
            foreach (var prefab in recreatedPrefabs)
            {
                // for each snap point, 
                // O(N^2), but done rarely... probably ok
                foreach (var snappoint in prefab.SnapPoints)
                {
                    List<MySnapPointLink> snaps = new List<MySnapPointLink>();
                    snaps.Add(new MySnapPointLink(snappoint));

                    var snappointWorldPosition = (snappoint.Matrix * snappoint.Prefab.WorldMatrix).Translation;

                    foreach (var prefab2 in recreatedPrefabs)
                        if (prefab2 != prefab)
                            foreach (var snappoint2 in prefab2.SnapPoints)
                                // if the snap points are close (<20m), link their waypoints
                                if (Vector3.DistanceSquared(snappointWorldPosition, (snappoint2.Matrix * snappoint2.Prefab.WorldMatrix).Translation) < 400.0f)
                                    snaps.Add(new MySnapPointLink(snappoint2));

                    //                    if (snaps.Count >= 2)
                    MyEditor.Static.ConnectWayPointsAtSnapPointLinks(snaps);
                }
            }

            // connect waypoints along snap point links
            foreach (var snaps in MyEditor.Static.LinkedSnapPoints)
                MyEditor.Static.ConnectWayPointsAtSnapPointLinks(snaps);

            /*
            // for collecting prefabs without waypoints
            var sb = new StringBuilder();
            foreach (var name in waypointlessPrefabs)
                sb.Append(name).Append(" ");
            string s = sb.ToString();
            */
        }

        /*
        public static MyWayPoint GetWaypointAtGeneratedPosition(Vector3 idealPosition, MyPrefabBase parent)
        {
            MyWayPoint closestWaypoint = null;
            float closestDistance = float.MaxValue;
            foreach (var wp in GetAllWaypointsInSphere(idealPosition, 1)) if (wp.Parent == parent)
            {
                var distance = Vector3.DistanceSquared(wp.Position, idealPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypoint = wp;
                }
            }
            return closestWaypoint;
        }
        
        // Recreate waypoints that didn't need to be saved.
        public static void RecreateImplicitWaypoints(List<MyPrefabBase> prefabs)
        {
            var added = new List<MyWayPoint>();
            var addedIndex = new List<int>();

            // pass 1: recreate waypoints
            foreach (var prefab in prefabs)
            {
                var positions = new List<Vector3>();
                var names = new List<string>();
                var parentNames = new List<string>();
                prefab.GetDefaultWaypointData(positions, names, parentNames);

                for (int j = 0; j < positions.Count; j++)
                {
                    MyWayPoint closestToGeneratedPosition = GetWaypointAtGeneratedPosition(positions[j], prefab);
                    if (closestToGeneratedPosition == null)
                    {
                        added.Add(CreateWaypoint(positions[j], prefab));
                        addedIndex.Add(j);
                    }
                }
            }

            // pass 2: recreate their edges
            for (int i = 0; i < added.Count; i++)
            {
                var prefab = added[i].Parent as MyPrefabBase;

                var positions = new List<Vector3>();
                var names = new List<string>();
                var parentNames = new List<string>();
                prefab.GetDefaultWaypointData(positions, names, parentNames);

                int k = addedIndex[i];
                for (int j = 0; j < positions.Count; j++)
                {
                    if (names[j].Equals(parentNames[k]) || names[k].Equals(parentNames[j]))
                    {
                        MyWayPoint closestToGeneratedPosition = GetWaypointAtGeneratedPosition(positions[j], prefab);
                        MyWayPoint.Connect(added[i], closestToGeneratedPosition);
                    }
                }
            }
        }

        // Delete waypoints that don't need to be saved.
        public static void DeleteImplicitWaypoints()
        {
            PrepareVertexListForModification();
            foreach (var v in m_vertexListForModification)
            {
                if (!v.NeedsToBeSaved())
                {
                    v.Close();
                }
            }
        }
        */

        // Return the closest waypoint.
        public static MyWayPoint GetClosestWaypoint(Vector3 position)
        {
            MyWayPoint best = null;
            float bestDistanceSquared = float.PositiveInfinity;
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                {
                    float distanceSquared = Vector3.DistanceSquared(position, v.Position);
                    if (distanceSquared < bestDistanceSquared)
                    {
                        bestDistanceSquared = distanceSquared;
                        best = v;
                    }
                }
            }
            return best;
        }

        // Return the closest non-generated waypoint.
        public static MyWayPoint GetClosestNonGeneratedWaypoint(Vector3 position)
        {
            MyWayPoint best = null;
            float bestDistanceSquared = float.PositiveInfinity;

            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                {
                    if (!v.Save) continue;
                    float distanceSquared = Vector3.DistanceSquared(position, v.Position);
                    if (distanceSquared < bestDistanceSquared)
                    {
                        bestDistanceSquared = distanceSquared;
                        best = v;
                    }
                }
            }
            return best;
        }

        static bool readingVertices = false;

        // Return K closest waypoints sorted by distance (smallest first). Worst case O(#waypoints * K).
        public static List<MyWayPoint> GetClosestWaypoints(Vector3 position, int k)
        {
            var sorted = new SortedList<float, MyWayPoint>(k);
            using (VerticesLock.AcquireSharedUsing())
            {
                readingVertices = true;
                foreach (var v in m_vertices)
                {
                    var distanceSquared = Vector3.DistanceSquared(position, v.Position);
                    if (sorted.Count < k)
                    {
                        while (sorted.ContainsKey(distanceSquared))  // can't add two equal keys, wtf
                        {
                            distanceSquared = MathUtil.NextAfter(distanceSquared, float.MaxValue);
                        }
                        sorted.Add(distanceSquared, v);  // add it
                    }
                    else if (distanceSquared < sorted.Keys[k - 1])  // smaller than maximum: remove the maximum and add it
                    {
                        sorted.RemoveAt(k - 1);

                        while (sorted.ContainsKey(distanceSquared))  // can't add two equal keys, wtf
                        {
                            distanceSquared = MathUtil.NextAfter(distanceSquared, float.MaxValue);
                        }
                        sorted.Add(distanceSquared, v);
                    }
                }
                readingVertices = false;
            }            

            var result = new List<MyWayPoint>();
            foreach (var pair in sorted)
            {
                result.Add(pair.Value);
            }

            return result;
        }

        // Return N closest waypoints visible from a position.
        public static List<MyWayPoint> GetClosestVisibleWaypoints(Vector3 position, MyEntity ignore, int n, int maxRaycasts, Dictionary<MyWayPoint, bool> visibilityCache = null)
        {
            var result = new List<MyWayPoint>();
            foreach (var v in GetClosestWaypoints(position, maxRaycasts))
            {
                if (result.Count >= n) break;
                if (v.Position == position || v.IsVisibleFrom(position, ignore, visibilityCache))
                    result.Add(v);
            }
            return result;
        }

        // Return the closest waypoint reachable by a ship.
        public static MyWayPoint GetClosestWaypointReachableByShip(Vector3 position, MyEntity ignore, int maxRaycasts, float shipRadius)
        {
            // One "fat" raycast needs 4 regular raycasts (or fewer when we discover that the path is blocked).
            // This means that success is possible only when maxRaycasts >= 4.
            maxRaycasts = Math.Max(maxRaycasts, 4);

            MyLine line;
            int raycastsDone = 0;

            foreach (var v in GetClosestWaypoints(position, maxRaycasts))
            {
                Vector3 vertexPosition = v.Position;
                Vector3 diff = vertexPosition - position;
                if (MyMwcUtils.IsZero(diff)) return v;  // we're right on it, no need for raycasts

                if (maxRaycasts - raycastsDone < 4) break;  // can't be successful when there's fewer than 4 raycasts remaining

                Matrix m = MinerWars.AppCode.Game.Utils.MyMath.MatrixFromDir(Vector3.Normalize(diff)) * shipRadius;

                const float sin120 = 0.8660254f;
                const float cos120 = 0.5f;

                // center
                line = new MyLine(position, vertexPosition, true);
                if (MyEntities.GetAnyIntersectionWithLine(ref line, ignore, null, true, true, true, false) != null) continue;
                raycastsDone++;

                // triangle
                line = new MyLine(position + m.Forward - m.Up, vertexPosition + m.Forward - m.Up, true);
                if (MyEntities.GetAnyIntersectionWithLine(ref line, ignore, null, true, true, true, false) != null) continue;
                raycastsDone++;

                line = new MyLine(position + m.Forward + m.Up * cos120 + m.Left * sin120, vertexPosition + m.Forward + m.Up * cos120 + m.Left * sin120, true);
                if (MyEntities.GetAnyIntersectionWithLine(ref line, ignore, null, true, true, true, false) != null) continue;
                raycastsDone++;

                line = new MyLine(position + m.Forward + m.Up * cos120 - m.Left * sin120, vertexPosition + m.Forward + m.Up * cos120 - m.Left * sin120, true);
                if (MyEntities.GetAnyIntersectionWithLine(ref line, ignore, null, true, true, true, false) != null) continue;
                raycastsDone++;

                return v;
            }
            return null;
        }

        // Return the closest waypoint. Waypoints along the given direction are considered to be closer.
        // directionBonus = 0: no bonus (= return the closest waypoint)
        // directionBonus = 1: every point on line has distance 0 (= return the waypoint with the smallest angle from direction)
        public static MyWayPoint GetClosestWaypointWithDirectionBias(Vector3 position, Vector3 direction, float directionBonus)
        {
            MyWayPoint best = null;
            float bestScore = float.MaxValue;
            direction.Normalize();

            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                {
                    float distanceAlongDirection = Vector3.Dot(v.Position - position, direction);
                    float totalDistance = Vector3.Distance(position, v.Position);

                    float score = totalDistance - distanceAlongDirection * directionBonus;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = v;
                    }
                }
            }

            return best;
        }

        /*
        // Return the closest visible waypoint. Waypoints along the given direction are considered to be closer.
        // directionBonus = 0: no bonus (= return the closest waypoint)
        // directionBonus = 1: every point on line has distance 0 (= return the waypoint with the smallest angle from direction)
        public static MyWayPoint GetClosestVisibleWaypointWithDirectionBias(Vector3 position, Vector3 direction, float directionBonus, int maxTests)
        {
            direction.Normalize();

            PrepareVertexListForModification();
            var ordered = m_vertexListForModification.OrderBy(
                delegate(MyWayPoint v)
                {
                    float distanceAlongDirection = Vector3.Dot(v.Position - position, direction);
                    float totalDistance = Vector3.Distance(position, v.Position);
                    return totalDistance - distanceAlongDirection * directionBonus;
                }
            ).Take(maxTests);
            foreach (var v in ordered)
                if (v.IsVisibleFrom(position, null))
                    return v;
            return null;
        }
        */

        // Return the closest waypoint without any edges.
        public static MyWayPoint GetClosestWaypointWithoutEdges(Vector3 position)
        {
            MyWayPoint best = null;
            float bestDistance = float.MaxValue;

            using (VerticesLock.AcquireSharedUsing())
            {                
                foreach (var v in m_vertices) if (v.Neighbors.Count == 0)
                    {
                        float distance = Vector3.DistanceSquared(position, v.Position);
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            best = v;
                        }
                    }
            }
            return best;
        }

        // Return all waypoints inside a sphere.
        public static List<MyWayPoint> GetAllWaypointsInSphere(Vector3 center, float radius)
        {
            float radiusSquared = radius * radius;
            var list = new List<MyWayPoint>();

            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                    if (Vector3.DistanceSquared(center, v.Position) <= radiusSquared)
                        list.Add(v);
            }

            return list;
        }

        // Return all waypoint edges touching a sphere.
        public static List<Tuple<MyWayPoint, MyWayPoint>> GetAllEdgesInSphere(Vector3 center, float radius)
        {
            float radiusSquared = radius * radius;
            var list = new List<Tuple<MyWayPoint, MyWayPoint>>();
            using (VerticesLock.AcquireSharedUsing())
            {
                using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                {
                    foreach (var v in m_vertices)
                        foreach (var n in v.Neighbors)
                            if (MyMath.DistanceSquaredFromLineSegment(v.Position, n.Position, center) <= radiusSquared)
                                list.Add(Tuple.Create(v, n));
                }
            }
            return list;
        }

        // Return all waypoint edges touching a box.
        public static List<Tuple<MyWayPoint, MyWayPoint>> GetAllEdgesInBox(ref BoundingBox box)
        {
            var list = new List<Tuple<MyWayPoint, MyWayPoint>>();
            if (box.Max.X < box.Min.X)
                return list;
            using (VerticesLock.AcquireSharedUsing())
            {
                using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                {
                    foreach (var v in m_vertices) foreach (var n in v.Neighbors)
                        {
                            var d = n.Position - v.Position;
                            var l = d.Length();
                            d.Normalize();
                            float? result = box.Intersects(new Ray(v.Position, d));
                            if (result != null && result.Value <= l)
                                list.Add(Tuple.Create(v, n));
                        }
                }
            }
            return list;
        }

        // Remove all generated visibility-obstructed waypoint edges around an entity.
        public static void RemoveAllObstructedGeneratedEdgesAround(MyEntity entity)
        {
            using(MyEntities.EntityCloseLock.AcquireSharedUsing())
            {                
                entity.UpdateAABBHr();
                BoundingBox box = entity.WorldAABBHr;
                if (box.Max - box.Min == Vector3.Zero)
                {
                    box = entity.WorldAABB;
                    if (box.Max - box.Min == Vector3.Zero)
                        return;
                }

                foreach (var edge in GetAllEdgesInBox(ref box))
                {
                    if (edge.Item1.Save && edge.Item2.Save) continue;
                    if (!edge.Item1.IsVisibleFrom(edge.Item2.Position))
                        MyWayPoint.Disconnect(edge.Item1, edge.Item2);
                }
            }
        }

        public static void CountWaypointTypes(out int total, out int notsave, out int save, out int gen)
        {
            total = save = notsave = gen = 0;
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                {
                    total++;
                    if (!v.Save) { gen++; continue; }
                    if (v.NeedsToBeSaved()) save++; else notsave++;
                }
            }
        }

        #endregion

        #region Connected components

        static bool m_componentsDirty = true;

        static public void MakeConnectedComponentsDirty()
        {
            m_componentsDirty = true;
        }

        static Dictionary<MyWayPoint, int> m_componentIds;
        static List<List<MyWayPoint>> m_waypointsFromComponentId;

        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        static void RecomputeConnectedComponents()
        {
            if (!m_componentsDirty) return;

            int componentId = 0;
            m_componentIds = new Dictionary<MyWayPoint, int>();
            m_waypointsFromComponentId = new List<List<MyWayPoint>>();
            using (VerticesLock.AcquireSharedUsing())
            {
                readingVertices = true;
                foreach (var v in m_vertices)
                {
                    if (!m_componentIds.ContainsKey(v))
                    {
                        var componentWaypoints = new List<MyWayPoint>();
                        var todo = new Stack<MyWayPoint>();
                        todo.Push(v);
                        while (todo.Count != 0)
                        {
                            var w = todo.Pop();
                            componentWaypoints.Add(w);
                            if (!m_componentIds.ContainsKey(w))
                            {
                                m_componentIds[w] = componentId;
                                using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                                {
                                    foreach (var n in w.Neighbors)
                                        todo.Push(n);
                                }
                            }
                        }
                        m_waypointsFromComponentId.Add(componentWaypoints);
                        componentId++;
                    }
                }
                readingVertices = false;
            }
            
            m_componentsDirty = false;
        }

        // Return the component id, or -1 if the given waypoint doesn't lie in the waypoint graph.
        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        public static int GetConnectedComponentId(MyWayPoint wp)
        {
            RecomputeConnectedComponents();
            int value;
            if (m_componentIds.TryGetValue(wp, out value))
                return value;
            return -1;
        }

        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        public static List<MyWayPoint> GetWaypointsWithConnectedComponentId(int id)
        {
            RecomputeConnectedComponents();
            if (id >= 0 && id < m_waypointsFromComponentId.Count)
                return m_waypointsFromComponentId[id];
            else
                return new List<MyWayPoint>();
        }

        #endregion

        #region ObjectBuilders

        public static void Init()
        {
            m_vertices = new HashSet<MyWayPoint>();
            InitPaths();
            m_GPSWaypointsInited = false;
        }

        public static MyWayPoint CreateWaypoint(Vector3 position, MyEntity parent)
        {
            var builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.WaypointNew, null) as MyMwcObjectBuilder_WaypointNew;
            builder.ParentEntityId = (parent == null || parent.EntityId == null) ? null : (int?)parent.EntityId.Value.NumericValue;
            var result = MyEntities.CreateFromObjectBuilderAndAdd(null, builder, Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up)) as MyWayPoint;
            result.ResolveLinks();
            return result;
        }

        #endregion

        #region StoredPaths and editor

        public static List<MyWayPointPath> StoredPaths;
        public static Dictionary<string, MyWayPointPath> PathFromName;
        public static MyWayPointPath SelectedPath;

        private static void InitPaths()
        {
            StoredPaths = new List<MyWayPointPath>();
            PathFromName = new Dictionary<string, MyWayPointPath>();
            SelectedPath = null;
        }

        // Update visibility, path and vertex selection. Used when toggling waypoint visibility.
        public static void UpdateVisibilityAndSelection(bool visible)
        {
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var waypoint in m_vertices)
                {
                    waypoint.Visible = visible;
                }
            }

            // deselect waypoints
            if (!visible)
            {
                SelectedPath = null;
                foreach (var waypoint in MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities))
                    MyEditorGizmo.RemoveEntityFromSelection(waypoint);
            }
        }

        // Update or delete the selected waypoint path.
        public static void UpdateSelectedPath()
        {
            if (SelectedPath != null)
            {
                var selectedWaypoints = MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities);
                if (selectedWaypoints.Count == 0)
                    RemovePath(SelectedPath);
                else
                    SelectedPath.WayPoints = selectedWaypoints;
            }
        }

        // Find a waypoint path by name.
        public static MyWayPointPath GetPath(string name)
        {
            MyWayPointPath path;
            if (PathFromName.TryGetValue(name, out path))
                return path;
            return null;
        }

        // Purge null vertices. Called after sector load.
        public static void DeleteNullVerticesFromPaths()
        {
            foreach (var path in StoredPaths)
                path.DeleteNullVertices();
            RemovePathsWithZeroVertices();
        }

        public static void Close()
        {
            foreach (var v in GetVertexListForModification())
                v.Close();
            InitPaths();
            m_GPSWaypointsInited = false;
            RecomputeConnectedComponents();  // releases m_waypointsFromComponentId and m_componentIds
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyWayPointGraph.LoadData");
            m_vertices = new HashSet<MyWayPoint>();
            InitPaths();
            m_GPSWaypointsInited = false;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            Close();
        }

        public static void InvalidateStoredPathCache()
        {
            foreach (var s in StoredPaths)
                s.InvalidateStoredPathCache();
        }

        public static void SetSecrecyForPath(string pathName, bool newSecrecy)
        {
            var path = GetPath(pathName);
            if (path == null) return;
            foreach (var v in path.WayPoints)
                v.IsSecret = newSecrecy;
        }

        public static void SetSecrecyForWaypoints(List<MyWayPoint> waypoints, bool newSecrecy)
        {
            foreach (var v in waypoints)
                v.IsSecret = newSecrecy;
        }

        #endregion

        #region Sector filling

        private static bool m_GPSWaypointsInited = false;

        public static void RemoveWaypointsAroundLargeStaticObjects()
        {
            if (!m_GPSWaypointsInited) return;

            foreach (var v in GetVertexListForModification())
                if (!v.Save)
                    v.MarkForClose();

            m_GPSWaypointsInited = false;
            InvalidateStoredPathCache();
        }


        public static bool WaypointsIgnoreDepth = true;

        private static bool m_visibilityOfAllWaypoints = true;

        public static bool GetVisibilityOfAllWaypoints()
        {
            return m_visibilityOfAllWaypoints;
        }

        public static void SetVisibilityOfAllWaypoints(bool visibility)
        {
            if (m_visibilityOfAllWaypoints == visibility) return;

            m_visibilityOfAllWaypoints = visibility;

            using (VerticesLock.AcquireSharedUsing())
            {                
                foreach (var v in m_vertices)
                {
                    v.Visible = visibility;
                    v.RenderObjects[0].SkipIfTooSmall = false;
                }
            }
        }

        /// <summary>
        /// Create and connect waypoints to enable navigation outside prefab containers.
        /// Needs to be called from the main thread (because it uses MyEntities.GetSafeIterationHelperForAll).
        /// </summary>
        public static void CreateWaypointsAroundLargeStaticObjects()
        {
            if (m_GPSWaypointsInited) return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateWaypointsAroundLargeStaticObjects");

            // memory benchmark
            //GC.Collect(2);
            //MyMwcLog.WriteLine("#CreateWaypointsAroundLargeStaticObjects# before: working set " + MyValueFormatter.GetFormatedLong(Environment.WorkingSet) + ", gc " + MyValueFormatter.GetFormatedLong(GC.GetTotalMemory(false)));

            // counters for debugging
            int largeObjects = 0;
            int totalWaypointsOutside = 0;
            int bigSubdivisions = 0;
            int freeEnvelopes = 0;
            int edgesWithoutRaycasts = 0;
            int edgesWithRaycastsAdded = 0;
            int edgesWithRaycastsNotAdded = 0;
            int closed = 0;

            var envelopes = new List<MyWayPoint[, ,]>();
            var envelopeEntity = new List<MyEntity>();
            var envelopeBvh = new MyDynamicAABBTree(MyConstants.GAME_PRUNING_STRUCTURE_AABB_EXTENSION);

            var nonFree = new HashSet<MyWayPoint>();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("create envelopes");

            var interestingBoxInflation = new Vector3(MyWaypointConstants.MAXIMUM_BOX_DISTANCE_TO_INTERESTING_STUFF_TO_GENERATE_WAYPOINTS);
            var interestingBoxes = new List<BoundingBox>();

            // find interesting stuff (only prefab containers for now)
            foreach (var entity in MyEntities.GetEntities())
            {
                if (!(entity is MyPrefabContainer)) continue;

                entity.UpdateAABBHr();
                BoundingBox box = entity.WorldAABBHr;
                if (box.Max - box.Min == Vector3.Zero)
                {
                    box = entity.WorldAABB;
                    if (box.Max - box.Min == Vector3.Zero) continue;  // no bounding box
                }

                BoundingBox extrudedBox = new BoundingBox(box.Min - interestingBoxInflation, box.Max + interestingBoxInflation);
                interestingBoxes.Add(extrudedBox);
            }

            int ss = 0;
            int madelynsBox = -1;
            // create envelopes
            foreach (var entity in MyEntities.GetSafeIterationHelperForAll())
            {
                if (!(entity is MyVoxelMap || entity is MyPrefabContainer || entity is MyStaticAsteroid)) continue;

                entity.UpdateAABBHr();
                BoundingBox box = entity.WorldAABBHr;
                if (box.Max - box.Min == Vector3.Zero)
                    box = entity.WorldAABB;
                if (entity is MyStaticAsteroid &&
                    (box.Max - box.Min).LengthSquared() < MyWaypointConstants.MINIMUM_ASTEROID_DIAGONAL_LENGTH_TO_GENERATE_WAYPOINTS * MyWaypointConstants.MINIMUM_ASTEROID_DIAGONAL_LENGTH_TO_GENERATE_WAYPOINTS)
                {
                    continue;  // small static asteroids: ignore
                }
                if (entity is MyStaticAsteroid)
                {
                    ss++;
                    bool inInteresting = false;
                    foreach (var iBox in interestingBoxes)
                    {
                        if (iBox.Contains(box) != ContainmentType.Disjoint)
                        {
                            inInteresting = true;
                            break;
                        }
                    }
                    if (!inInteresting) continue;  // static asteroids far from interesting stuff: ignore
                }
                // enlarge by 1% and 15 meters on each side
                BoundingBox extrudedBox = new BoundingBox(box.Min - (box.Max - box.Min) * 0.01f - new Vector3(15, 15, 15), box.Max + (box.Max - box.Min) * 0.01f + new Vector3(15, 15, 15));

                //diagonals.Add((float)Math.Sqrt((box.Max - box.Min).LengthSquared()));

                var waypointsOutside = new HashSet<MyWayPoint>();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("find crossing");
                // add all edges that cross the non-extruded box from inside to outside (remember out-vertices)
                foreach (var waypoint in MyGamePruningStructure.GetAllEntitiesInBox(ref extrudedBox, MyGamePruningStructure.QueryFlags.Waypoints))
                {
                    var v = waypoint as MyWayPoint;
                    if (!v.Save) continue;
                    nonFree.Add(v);
                    using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                    {
                        foreach (var n in v.Neighbors) if (n.Save && extrudedBox.Contains(n.Position) != ContainmentType.Contains)
                            {
                                if (waypointsOutside.Add(n)) totalWaypointsOutside++;
                                nonFree.Add(n);
                            }
                    }
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                // create envelope
                int s = 1;
                if (waypointsOutside.Count > 0 || entity as MyStaticAsteroid == null)  // voxel maps and prefabs are automatically subdivided more
                {
                    s = 2;
                    bigSubdivisions++;
                }
                MyWayPoint[, ,] envelope = new MyWayPoint[s + 1, s + 1, s + 1];

                for (int i = 0; i <= s; i++) for (int j = 0; j <= s; j++) for (int k = 0; k <= s; k++)
                        {
                            if (s == 2 && i == 1 && j == 1 && k == 1) continue;
                            envelope[i, j, k] = CreateWaypoint(new Vector3(
                                extrudedBox.Min.X + i * (extrudedBox.Max.X - extrudedBox.Min.X) / s,
                                extrudedBox.Min.Y + j * (extrudedBox.Max.Y - extrudedBox.Min.Y) / s,
                                extrudedBox.Min.Z + k * (extrudedBox.Max.Z - extrudedBox.Min.Z) / s
                            ), null);
                            envelope[i, j, k].Save = false;  // don't save generated waypoints
                            nonFree.Add(envelope[i, j, k]);

                            // connect with neighbors
                            // use ConnectIfNoAABBBlockers only for non-static asteroids
                            // don't connect to the non-existing middle vertex
                            if (entity is MyStaticAsteroid)
                            {
                                if (i != 0) if (!(s == 2 && i - 1 == 1 && j == 1 && k == 1)) { MyWayPoint.Connect(envelope[i, j, k], envelope[i - 1, j, k]); edgesWithoutRaycasts++; }
                                if (j != 0) if (!(s == 2 && j - 1 == 1 && i == 1 && k == 1)) { MyWayPoint.Connect(envelope[i, j, k], envelope[i, j - 1, k]); edgesWithoutRaycasts++; }
                                if (k != 0) if (!(s == 2 && k - 1 == 1 && j == 1 && i == 1)) { MyWayPoint.Connect(envelope[i, j, k], envelope[i, j, k - 1]); edgesWithoutRaycasts++; }
                            }
                            else
                            {
                                if (i != 0) if (!(s == 2 && i - 1 == 1 && j == 1 && k == 1)) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i - 1, j, k])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                                if (j != 0) if (!(s == 2 && j - 1 == 1 && i == 1 && k == 1)) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i, j - 1, k])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                                if (k != 0) if (!(s == 2 && k - 1 == 1 && j == 1 && i == 1)) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i, j, k - 1])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                            }

                            // if it's a part of a face that faces an out-vertex, connect it
                            foreach (var v in waypointsOutside)
                                if ((i == 0 && v.Position.X <= envelope[i, j, k].Position.X) ||
                                    (i == s && v.Position.X >= envelope[i, j, k].Position.X) ||
                                    (j == 0 && v.Position.Y <= envelope[i, j, k].Position.Y) ||
                                    (j == s && v.Position.Y >= envelope[i, j, k].Position.Y) ||
                                    (k == 0 && v.Position.Z <= envelope[i, j, k].Position.Z) ||
                                    (k == s && v.Position.Z >= envelope[i, j, k].Position.Z)
                                )
                                {
                                    if (MyWayPoint.ConnectIfNoAABBBlockers(v, envelope[i, j, k]))
                                        edgesWithRaycastsAdded++;
                                    else
                                        edgesWithRaycastsNotAdded++;
                                }
                        }

                envelopes.Add(envelope);
                envelopeEntity.Add(entity);
                envelopeBvh.AddProxy(ref extrudedBox, envelopes.Count - 1, 0);
                largeObjects++;

                if (entity.Name == "Madelyn")
                {
                    madelynsBox = envelopes.Count - 1;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            var componentsDone = new HashSet<int>();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("connect free wps");

            // free waypoint: check whether it's connected to an envelope
            foreach (var v in GetVertexListForModification()) if (!nonFree.Contains(v))
                {
                    int id = GetConnectedComponentId(v);
                    if (componentsDone.Contains(id)) continue;

                    componentsDone.Add(id);

                    var connectedWaypoints = GetWaypointsWithConnectedComponentId(id);

                    // is this component already connected to an envelope?
                    var box = new BoundingBox(v.Position, v.Position);
                    foreach (var w in connectedWaypoints) if (w.Save)
                        {
                            var pos = w.Position;
                            box = box.Include(ref pos);
                            if (nonFree.Contains(w))
                                goto alreadyConnected;
                        }

                    BoundingBox extrudedBox = new BoundingBox(box.Min - (box.Max - box.Min) * 0.01f - new Vector3(5, 5, 5), box.Max + (box.Max - box.Min) * 0.01f + new Vector3(5, 5, 5));

                    // no - create a new one
                    int s = 1;
                    MyWayPoint[, ,] envelope = new MyWayPoint[s + 1, s + 1, s + 1];
                    for (int i = 0; i <= s; i++) for (int j = 0; j <= s; j++) for (int k = 0; k <= s; k++)
                            {
                                envelope[i, j, k] = CreateWaypoint(new Vector3(
                                    extrudedBox.Min.X + i * (extrudedBox.Max.X - extrudedBox.Min.X) / s,
                                    extrudedBox.Min.Y + j * (extrudedBox.Max.Y - extrudedBox.Min.Y) / s,
                                    extrudedBox.Min.Z + k * (extrudedBox.Max.Z - extrudedBox.Min.Z) / s
                                ), null);
                                envelope[i, j, k].Save = false;  // don't save generated waypoints
                                nonFree.Add(envelope[i, j, k]);

                                // connect with neighbors
                                // should use ConnectIfVisible, but it's slow and we can resolve it while computing the GPS
                                if (i != 0) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i - 1, j, k])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                                if (j != 0) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i, j - 1, k])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                                if (k != 0) { if (MyWayPoint.ConnectIfNoAABBBlockers(envelope[i, j, k], envelope[i, j, k - 1])) edgesWithRaycastsAdded++; else edgesWithRaycastsNotAdded++; }
                            }

                    // connect all waypoints to the closest corner of the new envelope
                    foreach (var w in connectedWaypoints)
                    {
                        var pos = w.Position;
                        if (MyWayPoint.ConnectIfNoAABBBlockers(w, envelope[pos.X < extrudedBox.GetCenter().X ? 0 : 1, pos.Y < extrudedBox.GetCenter().Y ? 0 : 1, pos.Z < extrudedBox.GetCenter().Z ? 0 : 1]))
                            edgesWithRaycastsAdded++;
                        else
                            edgesWithRaycastsNotAdded++;
                    }
                    envelopes.Add(envelope);
                    envelopeEntity.Add(null);
                    envelopeBvh.AddProxy(ref extrudedBox, envelopes.Count - 1, 0);
                    freeEnvelopes++;

                alreadyConnected: { }
                }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("connect envelopes");

            // connect envelopes together
            for (int eIndex = 0; eIndex < envelopes.Count; eIndex++)
            {
                var e = envelopes[eIndex];
                int es = e.GetLength(0) - 1;
                var eCenter = 0.5f * (e[0, 0, 0].Position + e[es, es, es].Position);

                // get K closest indices
                var closestEnvelopeIndices = new List<int>();
                for (int i = 200; i <= 6400; i *= 2)  // try 200, 400, 800, 1600, 3200, 6400 m
                {
                    var halfExtent = new Vector3(i);
                    var bbox = new BoundingBox(eCenter - halfExtent, eCenter + halfExtent);
                    envelopeBvh.OverlapAllBoundingBox(ref bbox, closestEnvelopeIndices);
                    if (closestEnvelopeIndices.Count >= 16) break;
                }

                // connect them together
                int k = 0;
                foreach (var qIndex in closestEnvelopeIndices)
                {
                    if (++k == 16) break;  // take only 16 envelopes
                    if (qIndex == eIndex) continue;

                    var q = envelopes[qIndex];

                    int qs = q.GetLength(0) - 1;
                    var qCenter = 0.5f * (q[0, 0, 0].Position + q[qs, qs, qs].Position);

                    // connect the closest opposite vertices
                    int qx, qy, qz, ex, ey, ez;
                    if (qCenter.X < eCenter.X) { qx = qs; ex = 0; } else { qx = 0; ex = es; }
                    if (qCenter.Y < eCenter.Y) { qy = qs; ey = 0; } else { qy = 0; ey = es; }
                    if (qCenter.Z < eCenter.Z) { qz = qs; ez = 0; } else { qz = 0; ez = es; }

                    if (es > 1 || qs > 1)
                    {
                        if (MyWayPoint.ConnectIfNoAABBBlockers(e[ex, ey, ez], q[qx, qy, qz], envelopeEntity[eIndex], envelopeEntity[qIndex]))
                            edgesWithRaycastsAdded++;
                        else
                            edgesWithRaycastsNotAdded++;
                    }
                    else
                    {
                        // don't make a raycast if one of the envelopes isn't important
                        MyWayPoint.Connect(e[ex, ey, ez], q[qx, qy, qz]);
                        edgesWithoutRaycasts++;
                    }

                    // connect Madelyn's waypoint to envelopes
                    if (eIndex == madelynsBox)
                    {
                        MyEntity madelyn = envelopeEntity[madelynsBox];
                        foreach (var child in madelyn.Children)
                        {
                            var w = child as MyWayPoint;
                            if (w == null)
                                continue;
                            MyWayPoint.ConnectIfNoAABBBlockers(w, q[qx, qy, qz]);  // make a raycast
                        }
                    }
                }

                // connect Madelyn's waypoint to envelopes
                if (eIndex == madelynsBox)
                {
                    MyEntity madelyn = envelopeEntity[madelynsBox];

                    madelyn.UpdateAABBHr();
                    BoundingBox extrudedAABB = madelyn.WorldAABBHr;
                    extrudedAABB.Min -= new Vector3(500);
                    extrudedAABB.Max += new Vector3(500);

                    List<MyWayPoint> nearMadelynWaypoints = new List<MyWayPoint>();

                    foreach (var waypoint in MyGamePruningStructure.GetAllEntitiesInBox(ref extrudedAABB, MyGamePruningStructure.QueryFlags.Waypoints))
                    {
                        MyWayPoint v = waypoint as MyWayPoint;
                        if (v != null)
                        {
                            if (!v.Save)
                                continue;
                            nearMadelynWaypoints.Add(v);
                        }
                    }

                    foreach (var child in madelyn.Children)
                    {
                        var w = child as MyWayPoint;
                        if (w == null)
                            continue;

                        foreach (MyWayPoint v in nearMadelynWaypoints)
                        {
                            MyWayPoint.ConnectIfVisible(w, v);  // make a raycast
                        }
                    }
                }
            }

            // delete generated waypoints without edges
            foreach (var v in GetVertexListForModification())
            {
                if (!v.Save && v.Neighbors.Count == 0)
                {
                    v.MarkForClose();
                    closed++;
                }
            }

            m_GPSWaypointsInited = true;
            InvalidateStoredPathCache();

            // memory benchmark
            //GC.Collect(2);
            //MyMwcLog.WriteLine("#CreateWaypointsAroundLargeStaticObjects# after: working set " + MyValueFormatter.GetFormatedLong(Environment.WorkingSet) + ", gc " + MyValueFormatter.GetFormatedLong(GC.GetTotalMemory(false)));

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        /// <summary>
        /// Create and connect waypoints to enable navigation outside prefab containers.
        /// Needs to be called from the main thread (because it uses MyEntities.GetSafeIterationHelperForAll).
        /// </summary>
        public static void RecreateWaypointsAroundMadelyn()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("RecreateWaypointsAroundMadelyn");

            // counters for debugging
            int largeObjects = 0;
            int totalWaypointsOutside = 0;
            int bigSubdivisions = 0;
            int closed = 0;

            var envelopes = new List<MyWayPoint[, ,]>();
            var envelopeEntity = new List<MyEntity>();
            var envelopeBvh = new MyDynamicAABBTree(MyConstants.GAME_PRUNING_STRUCTURE_AABB_EXTENSION);

            var nonFree = new HashSet<MyWayPoint>();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("create envelope");

            int madelynsBox = -1;
            // create envelopes
            {
                var entity = MyEntities.GetEntityByName("Madelyn");

                entity.UpdateAABBHr();
                BoundingBox box = entity.WorldAABBHr;
                if (box.Max - box.Min == Vector3.Zero)
                    box = entity.WorldAABB;

                // enlarge by 1% and 15 meters on each side
                BoundingBox extrudedBox = new BoundingBox(box.Min - (box.Max - box.Min) * 0.01f - new Vector3(15, 15, 15), box.Max + (box.Max - box.Min) * 0.01f + new Vector3(15, 15, 15));

                var waypointsOutside = new HashSet<MyWayPoint>();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("find crossing");
                // add all edges that cross the non-extruded box from inside to outside (remember out-vertices)
                foreach (var waypoint in MyGamePruningStructure.GetAllEntitiesInBox(ref extrudedBox, MyGamePruningStructure.QueryFlags.Waypoints))
                {
                    var v = waypoint as MyWayPoint;
                    if (!v.Save) continue;
                    nonFree.Add(v);
                    using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                    {
                        foreach (var n in v.Neighbors)
                            if (n.Save && extrudedBox.Contains(n.Position) != ContainmentType.Contains)
                            {
                                if (waypointsOutside.Add(n)) totalWaypointsOutside++;
                                nonFree.Add(n);
                            }
                    }
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                // create envelope
                int s = 1;
                if (waypointsOutside.Count > 0 || entity as MyStaticAsteroid == null)  // voxel maps and prefabs are automatically subdivided more
                {
                    s = 2;
                    bigSubdivisions++;
                }
                MyWayPoint[, ,] envelope = new MyWayPoint[s + 1, s + 1, s + 1];

                for (int i = 0; i <= s; i++) for (int j = 0; j <= s; j++) for (int k = 0; k <= s; k++)
                {
                    if (s == 2 && i == 1 && j == 1 && k == 1) continue;
                    envelope[i, j, k] = CreateWaypoint(new Vector3(
                        extrudedBox.Min.X + i * (extrudedBox.Max.X - extrudedBox.Min.X) / s,
                        extrudedBox.Min.Y + j * (extrudedBox.Max.Y - extrudedBox.Min.Y) / s,
                        extrudedBox.Min.Z + k * (extrudedBox.Max.Z - extrudedBox.Min.Z) / s
                    ), null);
                    envelope[i, j, k].Save = false;  // don't save generated waypoints
                    nonFree.Add(envelope[i, j, k]);

                    // assume Madelyn's envelope has no blockers
                    if (i != 0) if (!(s == 2 && i - 1 == 1 && j == 1 && k == 1)) MyWayPoint.Connect(envelope[i, j, k], envelope[i - 1, j, k]);
                    if (j != 0) if (!(s == 2 && j - 1 == 1 && i == 1 && k == 1)) MyWayPoint.Connect(envelope[i, j, k], envelope[i, j - 1, k]);
                    if (k != 0) if (!(s == 2 && k - 1 == 1 && j == 1 && i == 1)) MyWayPoint.Connect(envelope[i, j, k], envelope[i, j, k - 1]);

                    // if it's a part of a face that faces an out-vertex, connect it
                    foreach (var v in waypointsOutside)
                        if ((i == 0 && v.Position.X <= envelope[i, j, k].Position.X) ||
                            (i == s && v.Position.X >= envelope[i, j, k].Position.X) ||
                            (j == 0 && v.Position.Y <= envelope[i, j, k].Position.Y) ||
                            (j == s && v.Position.Y >= envelope[i, j, k].Position.Y) ||
                            (k == 0 && v.Position.Z <= envelope[i, j, k].Position.Z) ||
                            (k == s && v.Position.Z >= envelope[i, j, k].Position.Z)
                        )
                        {
                            MyWayPoint.Connect(v, envelope[i, j, k]);
                        }
                }

                envelopes.Add(envelope);
                envelopeEntity.Add(entity);
                envelopeBvh.AddProxy(ref extrudedBox, envelopes.Count - 1, 0);
                largeObjects++;

                madelynsBox = envelopes.Count - 1;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("disconnect from old waypoints");
            var madelynWaypoints = new HashSet<MyWayPoint>();
            {
                var entity = MyEntities.GetEntityByName("Madelyn");

                foreach (var child in entity.Children)
                {
                    var w = child as MyWayPoint;
                    if (w == null) continue;
                    madelynWaypoints.Add(w);
                }
                foreach (var v in madelynWaypoints)
                {
                    var outsiders = new List<MyWayPoint>();
                    foreach (var n in v.Neighbors)
                        if (!madelynWaypoints.Contains(n)) outsiders.Add(n);
                    foreach (var n in outsiders)
                        MyWayPoint.Disconnect(v, n);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("connect to new surrounding waypoints");

            for (int eIndex = 0; eIndex < envelopes.Count; eIndex++)
            {
                var e = envelopes[eIndex];
                int es = e.GetLength(0) - 1;
                var eCenter = 0.5f * (e[0, 0, 0].Position + e[es, es, es].Position);

                // connect Madelyn's inner waypoints to waypoints around her
                if (eIndex == madelynsBox)
                {
                    MyEntity madelyn = envelopeEntity[madelynsBox];

                    madelyn.UpdateAABBHr();
                    BoundingBox extrudedAABB = madelyn.WorldAABBHr;
                    extrudedAABB.Min -= new Vector3(500);
                    extrudedAABB.Max += new Vector3(500);

                    List<MyWayPoint> nearMadelynWaypoints = new List<MyWayPoint>();

                    foreach (var waypoint in MyGamePruningStructure.GetAllEntitiesInBox(ref extrudedAABB, MyGamePruningStructure.QueryFlags.Waypoints))
                    {
                        MyWayPoint v = waypoint as MyWayPoint;
                        if (v != null)
                        {
                            if (!v.Save) continue;
                            if (madelyn.Children.Contains(v)) continue;
                            nearMadelynWaypoints.Add(v);
                        }
                    }
                    int connected = 0, raycasts = 0;
                    foreach (MyWayPoint v in nearMadelynWaypoints)
                    {
                        foreach (var child in madelyn.Children)
                        {
                            var w = child as MyWayPoint;
                            if (w == null) continue;

                            if (v.Neighbors.Contains(w)) continue;
                            raycasts++;
                            if (MyWayPoint.ConnectIfVisible(w, v))  // make a raycast
                            {
                                connected++;
                                break;
                            }
                        }
                    }
                }
            }

            // delete generated waypoints without edges
            foreach (var v in GetVertexListForModification())
            {
                if (!v.Save && v.Neighbors.Count == 0)
                {
                    v.MarkForClose();
                    closed++;
                }
            }

            InvalidateStoredPathCache();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }
        
        #endregion


        #region Anti-allocation measures

        private static List<MyWayPoint> m_vertexListForModification = new List<MyWayPoint>(MyWaypointConstants.MAX_WAYPOINTS);
        private static void PrepareVertexListForModification()
        {
            m_vertexListForModification.Clear();
            using (VerticesLock.AcquireSharedUsing())
            {
                foreach (var v in m_vertices)
                    m_vertexListForModification.Add(v);
            }

            MyCommonDebugUtils.AssertDebug(m_vertexListForModification.Count <= MyWaypointConstants.MAX_WAYPOINTS);
        }

        private static List<MyWayPoint> GetVertexListForModification()
        {
            PrepareVertexListForModification();
            return m_vertexListForModification;
        }

        #endregion

        public static int WaypointCount()
        {
            if (m_vertices == null) return 0;
            return m_vertices.Count;
        }
    }
}

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
using KeenSoftwareHouse.Library.Memory;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Editor;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Text;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Prefabs;
using System.Diagnostics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Physics.Collisions;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Entities.WayPoints
{
    class MyWayPoint : MyEntity
    {
        #region 3D position and distance

        public Vector3 Position  // world position; orientation doesn't matter
        {
            get { return GetPosition(); }
            set { SetPosition(value); }
        }

        // Get the distance between two vertices, no matter their parents.
        public static float Distance(MyWayPoint v1, MyWayPoint v2)
        {
            return Vector3.Distance(v1.Position, v2.Position);
        }

        #endregion

        #region Edges

        private HashSet<MyWayPoint> m_neighbors;
        public static FastResourceLock NeighborsLock = new FastResourceLock();

        public HashSet<MyWayPoint> Neighbors
        {
            get { return m_neighbors; }
        }

        // Add a symmetric edge between two vertices. O(1)
        public static void Connect(MyWayPoint v1, MyWayPoint v2)
        {
            if (v1 == v2) return;
            using (NeighborsLock.AcquireExclusiveUsing())
            {
                MyWayPointGraph.MakeConnectedComponentsDirty();
                v1.m_neighbors.Add(v2);
                v2.m_neighbors.Add(v1);
            }
        }

        // Add a symmetric edge between two vertices if there's nothing blocking the way.
        // Returns whether the connection exists after the raycast.
        public static bool ConnectIfVisible(MyWayPoint v1, MyWayPoint v2)
        {
            if (v1 == v2) return true;
            if (v1.Neighbors.Contains(v2)) return true;
            if (v1.Position == v2.Position)
            {
                Connect(v1, v2);
                return true;
            }
            var line = new MyLine(v1.Position, v2.Position, true);
            if (MyEntities.GetAnyIntersectionWithLine(ref line, null, null, true, true, true, false) == null)
            {
                Connect(v1, v2);
                return true;
            }
            return false;
        }

        // Add a symmetric edge between two vertices if there are no AABBs blocking the way.
        // Returns whether the connection exists after the raycast.
        public static bool ConnectIfNoAABBBlockers(MyWayPoint v1, MyWayPoint v2, MyEntity ignore1 = null, MyEntity ignore2 = null)
        {
            if (v1 == v2) return true;
            if (v1.Neighbors.Contains(v2)) return true;
            if (v1.Position == v2.Position)
            {
                Connect(v1, v2);
                return true;
            }
            var line = new MyLine(v1.Position, v2.Position, true);
            if (!MyEntities.IsAnyIntersectionWithLineAABBOnly(ref line, ignore1, ignore2))
            {
                Connect(v1, v2);
                return true;
            }
            return false;
        }

        // Remove an edge if it exists. O(1)
        public static void Disconnect(MyWayPoint v1, MyWayPoint v2)
        {
            if (v1 == v2) return;
            using (NeighborsLock.AcquireExclusiveUsing())
            {
                MyWayPointGraph.MakeConnectedComponentsDirty();
                v1.m_neighbors.Remove(v2);
                v2.m_neighbors.Remove(v1);
            }
        }

        // Add a symmetric edge between this and all vertices from a list. O(n)
        public void ConnectTo(IEnumerable<MyWayPoint> newNeighbors)
        {
            foreach (var v in newNeighbors)
                Connect(this, v);
        }

        // Remove all edges to vertices from a list. O(others.Count)
        public void DisconnectFrom(IEnumerable<MyWayPoint> others)
        {
            foreach (var v in others)
                Disconnect(this, v);
        }

        // Remove all edges from this. O(deg(this))
        public void DisconnectFromAllNeighbors()
        {
            using (NeighborsLock.AcquireExclusiveUsing())
            {
                MyWayPointGraph.MakeConnectedComponentsDirty();
                foreach (var v in m_neighbors)
                    v.m_neighbors.Remove(this);
                m_neighbors.Clear();
            }
        }

        // Find out whether there are any edges between any vertices from a list. O(n^2)
        public static bool AnyEdgesBetween(IEnumerable<MyWayPoint> vertices)
        {
            foreach (var v in vertices)
                foreach (var w in vertices)
                    if (v.m_neighbors.Contains(w))
                        return true;
            return false;
        }

        // Add symmetric edges between all vertices from a list. O(n^2)
        public static void ConnectAll(IEnumerable<MyWayPoint> vertices)
        {
            foreach (var v in vertices)
                v.ConnectTo(vertices);
        }

        // Remove all edges between all vertices from a list. O(n^2)
        public static void DisconnectAll(IEnumerable<MyWayPoint> vertices)
        {
            foreach (var v in vertices)
                v.DisconnectFrom(vertices);
        }

        // Return true if the given waypoints are connected by a path. O(n) (or O(1) if connected components were unchanged)
        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        public static bool Connected(IEnumerable<MyWayPoint> vertices)
        {
            int component = -1;
            bool first = true;
            foreach (var v in vertices)
            {
                if (first)
                {
                    component = MyWayPointGraph.GetConnectedComponentId(v);
                    first = false;
                }
                else
                {
                    int c = MyWayPointGraph.GetConnectedComponentId(v);
                    if (c != component) return false;
                    else if (c == -1) return false;  // two vertices that don't lie in the waypoint graph must be unconnected
                }
            }
            return true;  // 0 or 1 vertices are always connected
        }

        // Return true if the given waypoints are connected by a path. O(n) (or O(1) if connected components were unchanged)
        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        public static bool Connected(MyWayPoint v1, MyWayPoint v2)
        {
            int c1 = MyWayPointGraph.GetConnectedComponentId(v1);
            int c2 = MyWayPointGraph.GetConnectedComponentId(v2);
            return c1 == c2 && c1 != -1;
        }

        #endregion

        #region A* path searching

        static private int m_searchId = 0;     // Path search id. Incremented on every search.
        private MyWayPoint m_cameFrom;         // Previous vertex in the current path. If it's the same as the vertex, it's the start of the path.
        private float m_fScore;                // Path length from the start + estimated path length to the goal.
        private float m_gScore;                // Path length from the start.
        private int m_visitedSearchId = 0;     // Path search id in which this vertex was last visited. If visitedSearchId==searchId, the vertex was already visited in this search.

        public int SearchId
        {
            get { return m_visitedSearchId; }
            set { m_visitedSearchId = value; }
        }

        // Reset search ids to 0 (to be used on search id overflow).
        public void ResetVisitedSearchId()
        {
            m_visitedSearchId = 0;
        }

        // Sort vertices in the open set by lowest f; break ties by highest g (makes the last iteration faster).
        private class MyWayPointVertexAStarComparer : IComparer<MyWayPoint>
        {
            public int Compare(MyWayPoint u, MyWayPoint v)
            {
                int fComparison = u.m_fScore.CompareTo(v.m_fScore);
                if (fComparison != 0) return fComparison;
                return v.m_gScore.CompareTo(u.m_gScore);
            }
        }

        private static MyWayPointVertexAStarComparer m_aStarComparer;
        private static object m_shortestPathLock = new object();

        private static HashSet<Tuple<MyWayPoint, MyWayPoint>> m_blockedEdgesForBots = new HashSet<Tuple<MyWayPoint, MyWayPoint>>();
        private static int m_blockedEdgesChangeId = 0;

        private static HashSet<Tuple<MyWayPoint, MyWayPoint>> m_blockedEdgesForPlayer = new HashSet<Tuple<MyWayPoint, MyWayPoint>>();

        public static FastResourceLock BlockedEdgesLock = new FastResourceLock();

        public static void AddBlockedEdgesForBots(Tuple<MyWayPoint, MyWayPoint> blockedEdges) 
        {
            if (!BlockedEdgesForBots.Contains(blockedEdges)) 
            {
                using (BlockedEdgesLock.AcquireExclusiveUsing())
                {
                    BlockedEdgesForBots.Add(blockedEdges);
                    m_blockedEdgesChangeId++;
                }
            }
        }

        public static void RemoveBlockedEdgesForBots(Tuple<MyWayPoint, MyWayPoint> blockedEdges)
        {
            using (BlockedEdgesLock.AcquireExclusiveUsing())
            {
                if (BlockedEdgesForBots.Remove(blockedEdges))
                {
                    m_blockedEdgesChangeId++;
                }
            }
        }

        public static HashSet<Tuple<MyWayPoint, MyWayPoint>> BlockedEdgesForBots
        {
            get { return m_blockedEdgesForBots; }
        }

        public static HashSet<Tuple<MyWayPoint, MyWayPoint>> BlockedEdgesForPlayer
        {
            get { return m_blockedEdgesForPlayer; }
        }

        public static int BlockedEdgesChangeId 
        {
            get { return m_blockedEdgesChangeId; }
        }


        public static void AddBlockedEdgesForPlayer(Tuple<MyWayPoint, MyWayPoint> blockedEdges)
        {
            using (BlockedEdgesLock.AcquireExclusiveUsing())
            {
                if (!BlockedEdgesForPlayer.Contains(blockedEdges))
                {
                    BlockedEdgesForPlayer.Add(blockedEdges);
                }
            }
        }

        public static void RemoveBlockedEdgesForPlayer(Tuple<MyWayPoint, MyWayPoint> blockedEdges)
        {
            using (BlockedEdgesLock.AcquireExclusiveUsing())
            {
                if (BlockedEdgesForPlayer.Remove(blockedEdges))
                {
                }
            }
        }


        public static void CleanBlockedEdges()
        {
            MyMwcLog.WriteLine("MyWayPoint.CleanBlockedEdges - START");
            MyMwcLog.IncreaseIndent();

            using (BlockedEdgesLock.AcquireExclusiveUsing())
            {
                BlockedEdgesForBots.Clear();
                BlockedEdgesForPlayer.Clear();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyWayPoint.CleanBlockedEdges - END");
        }

        private static SortedSet<MyWayPoint> m_openSet;

        // Get the shortest path to the goal vertex using the A* algorithm. Return an empty list if the goal is unreachable.
        /// <summary>This function needs to be called with EntityCloseLock already acquired, or not an issue.</summary>
        public List<MyWayPoint> GetShortestPathTo(MyWayPoint goal, HashSet<Tuple<MyWayPoint, MyWayPoint>> blockedEdges = null, bool useGeneratedWaypoints = true, bool useSecretWaypoints = true)
        {
            lock (m_shortestPathLock)
            {
                if (Connected(this, goal) && (useGeneratedWaypoints || (Save && goal.Save)) && (useSecretWaypoints || (!IsSecret && !goal.IsSecret)))
                {
                    // use next search id, handle overflow
                    m_searchId++;
                    if (m_searchId == int.MaxValue)
                    {
                        MyWayPointGraph.ResetAllVisitedSearchIds();
                        m_searchId = 1;
                    }

                    // seen but yet unvisited vertices; add start vertex
                    m_openSet.Clear();
                    m_fScore = Distance(this, goal);  // estimate distance to goal as Euclidean (= consistent)
                    m_gScore = 0;
                    m_cameFrom = this;
                    m_openSet.Add(this);

                    int num = 0;

                    while (m_openSet.Count != 0)
                    {
                        if (num++ > MyWayPointGraph.WaypointCount() + 10)
                        {
                            int maxSearchId = 0;
                            foreach (var v in MyWayPointGraph.GetCopyOfAllWaypoints())
                            {
                                maxSearchId = Math.Max(maxSearchId, v.m_visitedSearchId);
                            }
                            Debug.Fail(string.Format(
                                "Infinite path search... contact JanK! \nDebug info: set_count={0} start_end_connected={1} current_search_id={2} max_search_id={3}",
                                m_openSet.Count,
                                Connected(this, goal),
                                m_searchId,
                                maxSearchId
                            ));
                            break;
                        }

                        var current = m_openSet.Min;  // get vertex with the smallest f (ties: highest g)
                        m_openSet.Remove(current);
                        if (current == goal) return ReconstructPath(goal);  // found the shortest path, return it
                        current.m_visitedSearchId = m_searchId;  // mark current as visited

                        using (NeighborsLock.AcquireSharedUsing())
                        {
                            // look at all neighbors
                            foreach (var neighbor in current.m_neighbors)
                            {
                                if (!useGeneratedWaypoints && !neighbor.Save) continue;  // generated
                                if (!useSecretWaypoints && neighbor.IsSecret) continue;  // secret
                                if (neighbor.m_visitedSearchId == m_searchId) continue;  // already visited

                                if (blockedEdges != null && blockedEdges.Contains(Tuple.Create(current, neighbor))) continue;  // blocked

                                var gScoreThroughCurrent = current.m_gScore + Distance(current, neighbor);

                                if (m_openSet.Contains(neighbor))  // already seen
                                {
                                    if (gScoreThroughCurrent < neighbor.m_gScore) m_openSet.Remove(neighbor);  // path through current is better: remove neighbor and put it back with updated score
                                    else continue;
                                }

                                neighbor.m_fScore = gScoreThroughCurrent + Distance(neighbor, goal);
                                neighbor.m_gScore = gScoreThroughCurrent;
                                neighbor.m_cameFrom = current;
                                m_openSet.Add(neighbor);
                            }
                        }
                    }
                }
            }
            return new List<MyWayPoint>();  // unreachable
        }

        // Reconstruct the path that leads to the given vertex using cameFrom links.
        private static List<MyWayPoint> ReconstructPath(MyWayPoint v)
        {
            var result = new List<MyWayPoint>();
            result.Add(v);
            while (v.m_cameFrom != v)
            {
                v = v.m_cameFrom;
                result.Add(v);
            }
            result.Reverse();
            return result;
        }

        #endregion

        #region Construction and destruction, Entity stuff

        public MyWayPoint()
            : base(true)
        {
            m_neighbors = new HashSet<MyWayPoint>();
        }

        static MyWayPoint()
        {
            m_aStarComparer = new MyWayPointVertexAStarComparer();
            m_openSet = new SortedSet<MyWayPoint>(m_aStarComparer);
        }

        const float MAX_RADIUS_WAYPOINT = 10.0f;  // for selecting

        public MyMwcObjectBuilder_WaypointNew objectBuilder;

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_WaypointNew ob, Matrix matrix)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);
            base.Init(hudLabelTextSb, ob);

            objectBuilder = ob;

            SetWorldMatrix(matrix);

            MyWayPointGraph.AddVertex(this);

            this.LocalVolume = new BoundingSphere(Vector3.Zero, MAX_RADIUS_WAYPOINT);
            //VisibleInGame = false;
            Visible = true;
            Flags |= EntityFlags.EditableInEditor;
            CastShadows = false;
            UpdateRenderObject(Visible);
        }

        // Resolve EntityId links.
        public void ResolveLinks()
        {
            // parent entities (prefabs)
            if (objectBuilder.ParentEntityId != null && MyEntities.GetEntities().Contains(this))
            {
                var parent = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier((uint)objectBuilder.ParentEntityId.Value));
                if (parent != null)
                {
                    var worldMatrix = WorldMatrix;
                    MyEntities.Remove(this);
                    //parent.Children.Add(this);  // remove it and add back through the parent
                    parent.AddChild(this);
                    this.Activate(true, false);
                    SetWorldMatrix(worldMatrix);
                }
            }

            // neighbors
            foreach (var neighborId in objectBuilder.NeighborEntityIds)
            {
                var neighbor = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier((uint)neighborId));
                if (neighbor != null && neighbor is MyWayPoint)
                    Connect(this, (MyWayPoint)neighbor);
            }

            // path placing
            for (int i = 0; i < objectBuilder.GroupPlacings.Count; i++)
            {
                var name = objectBuilder.GroupNames[i];
                var placing = objectBuilder.GroupPlacings[i];
                
                // resolve path by name: create it if it didn't exist
                MyWayPointPath path = MyWayPointGraph.GetPath(name);
                if (path == null)
                    path = new MyWayPointPath(name);

                // make enough empty waypoints to make it fit
                while (placing >= path.WayPoints.Count)
                    path.WayPoints.Add(null);

                path.WayPoints[placing] = this;
            }

            // can't delete object builder
            // This function is reentrant and sometimes we need both runs.
            //objectBuilder = null;

            // but we can delete neighbor and path info
            if (MyGuiScreenGamePlay.Static.IsGameActive())
            {
                objectBuilder.GroupNames.Clear();
                objectBuilder.GroupPlacings.Clear();
                objectBuilder.NeighborEntityIds.Clear();
            }
        }

        public override void Link()
        {
            base.Link();
            ResolveLinks();
        }

        public override void Close()
        {
            MyWayPointGraph.RemoveVertex(this);
            base.Close();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_WaypointNew ob = (MyMwcObjectBuilder_WaypointNew)base.GetObjectBuilderInternal(getExactCopy);

            ob.GroupNames = new List<string>();
            ob.GroupPlacings = new List<int>();
            foreach (var path in Paths())
            {
                System.Diagnostics.Debug.Assert(path.Name.Length <= MyWaypointConstants.MAXIMUM_WAYPOINT_PATH_NAME_LENGTH);
                ob.GroupNames.Add(path.Name);
                ob.GroupPlacings.Add(path.WayPoints.IndexOf(this));
            }

            ob.NeighborEntityIds = new List<int>();
            using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
            {
                foreach (var neighbor in Neighbors)
                {
                    var id = neighbor.EntityId;
                    System.Diagnostics.Debug.Assert(id != null);
                    ob.NeighborEntityIds.Add((int)id.Value.NumericValue);
                }
            }

            ob.ParentEntityId = (Parent == null || Parent.EntityId == null) ? null : (int?)Parent.EntityId.Value.NumericValue;

            return ob;
        }

        #endregion

        #region Editor

        void DrawWaypointVertex(Vector3 position, Vector3 color)
        {
            if (MyWayPointGraph.WaypointsIgnoreDepth)
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.snap_point, new Vector4(color * 0.5f, 0.5f), position, WorldVolume.Radius, 0.0f, 2);
            else
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.snap_point_depth, new Vector4(color * 0.5f, 0.5f), position, WorldVolume.Radius, 0.0f, 2);
        }

        void DrawWaypointEdge(Vector3 position1, Vector3 position2, Color color1, Color color2)
        {
            if (MyFakes.MWBUILDER)
            {
                MyDebugDraw.DrawText((position1 + position2) / 2, new StringBuilder(Vector3.Distance(position1, position2).ToString("#,###0.000")), Color.White, 1); 
            }

            if (position1 == position2) return;
            Vector3 direction = position2 - position1;
            float lineLength = direction.Length();
            direction.Normalize();
            MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, color1.ToVector4(), position1, direction, lineLength, 0.25f);

            if (MyWayPointGraph.WaypointsIgnoreDepth)
            {
                MyDebugDraw.DrawLine3D(position1, position2, color1, color2);
            }
        }

        private static Vector3 HsvToRgb(float h, float s, float v)
        {
            float[] hues = { 0, 15, 21, 25, 36, 49, 55 };
            h *= hues[hues.Length - 1];

            for (int i = 0; i < hues.Length - 1; i++)
                if (h <= hues[i + 1])
                {
                    h = i + (h - hues[i]) / (hues[i + 1] - hues[i]);
                    break;
                }

            float f = h % 1.0f;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);
            switch ((int)h)
            {
                case 0: return new Vector3(v, t, p);
                case 1: return new Vector3(q, v, p);
                case 2: return new Vector3(p, v, t);
                case 3: return new Vector3(p, q, v);
                case 4: return new Vector3(t, p, v);
                default:return new Vector3(v, p, q);
            }
        }

        public override bool DebugDraw()
        {            /*
            int i = MyWayPointGraph.GetConnectedComponentId(this);
            var vertexColor = HsvToRgb((0.36f + i * 0.618034f) % 1.0f, 0.8f, 0.75f);

            DrawWaypointVertex(WorldMatrix.Translation, vertexColor);  // draw only edges for generated waypoints


            // draw edges
            foreach (var neighbor in Neighbors)
            {
                //DrawWaypointEdge(wp.WorldMatrix.Translation, neighbor.WorldMatrix.Translation, Color.Red, Color.Green);  // selected path: red-green edges

                if (neighbor.WorldMatrix.Translation != WorldMatrix.Translation)
                {
                    Vector3 direction = neighbor.WorldMatrix.Translation - WorldMatrix.Translation;
                    float lineLength = direction.Length();
                    direction.Normalize();
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, Color.Yellow.ToVector4(), WorldMatrix.Translation, direction, lineLength, 0.25f);
                }

            }
                       */
            if (((MyHud.ShowDebugWaypoints) || (MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())) && (MyFakes.ENABLE_GENERATED_WAYPOINTS_IN_EDITOR || MyHud.ShowDebugGeneratedWaypoints || Save))
            {
                // color by connected components
                int i = MyWayPointGraph.GetConnectedComponentId(this);
                var vertexColor = HsvToRgb((0.36f + i * 0.618034f) % 1.0f, 0.8f, 0.75f);

                if (MyWayPointGraph.SelectedPath != null && MyWayPointGraph.SelectedPath.WayPoints.Contains(this))
                {
                    vertexColor = Color.Orange.ToVector3();  // selected path: orange vertices
                }

                if (IsSecret) { vertexColor *= 0.25f; }

                // draw vertices
                if (MyEditorGizmo.SelectedEntities.Contains(this))
                {
                    DrawWaypointVertex(WorldMatrix.Translation, vertexColor + (IsSecret ? 1 : 3) * GetHighlightColor());
                    var name = new StringBuilder();
                    if (MyWayPointGraph.SelectedPath != null && MyWayPointGraph.SelectedPath.WayPoints.Contains(this))
                    {
                        name.Append(MyWayPointGraph.SelectedPath.Name).Append(": ").Append(MyWayPointGraph.SelectedPath.WayPoints.IndexOf(this) + 1);
                    }
                    else
                    {
                        name.Append(MyWayPoint.FilterWayPoints(MyEditorGizmo.SelectedEntities).IndexOf(this) + 1);
                    }
                    MyDebugDraw.DrawText(WorldMatrix.Translation, name, Color.White, 1);
                }
                else
                {
                    if (Save)
                        DrawWaypointVertex(WorldMatrix.Translation, vertexColor);  // for generated waypoints, draw only edges
                }

                // draw edges
                if (Save || MyHud.ShowDebugGeneratedWaypoints)
                {
                    using (MyWayPoint.NeighborsLock.AcquireSharedUsing())
                    {
                        foreach (var neighbor in Neighbors)
                        {
                            if (MyWayPointGraph.SelectedPath != null && MyWayPointGraph.SelectedPath.ContainsEdge(this, neighbor))
                            {
                                DrawWaypointEdge(WorldMatrix.Translation, neighbor.WorldMatrix.Translation, Color.Yellow, Color.White);  // on selected path: yellow-white
                                continue;
                            }

                            if (neighbor.Save || MyHud.ShowDebugGeneratedWaypoints)
                            {
                                using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
                                {

                                    // blocked for player (by a locked indestructible door: white-gray)
                                    if (BlockedEdgesForPlayer.Contains(Tuple.Create(this, neighbor)) || BlockedEdgesForPlayer.Contains(Tuple.Create(neighbor, this)))
                                    {
                                        DrawWaypointEdge(WorldMatrix.Translation, neighbor.WorldMatrix.Translation, Color.White, Color.Gray);
                                        continue;
                                    }

                                    // blocked for bots by a locked door: black-gray
                                    if (BlockedEdgesForBots.Contains(Tuple.Create(this, neighbor)) || BlockedEdgesForBots.Contains(Tuple.Create(neighbor, this)))
                                    {
                                        DrawWaypointEdge(WorldMatrix.Translation, neighbor.WorldMatrix.Translation, Color.Black, Color.Gray);
                                        continue;
                                    }
                                }

                                // obstructed: violet-white
                                if (MyHud.ShowDebugWaypointsCollisions && Position != neighbor.Position)
                                {
                                    var line = new MyLine(Position, neighbor.Position, true);
                                    if (MyEntities.GetAnyIntersectionWithLine(ref line, null, null, true, true, true, false) != null)
                                    {
                                        DrawWaypointEdge(WorldMatrix.Translation, neighbor.WorldMatrix.Translation, Color.Violet, Color.White);
                                        continue;
                                    }
                                }

                                // normal-normal: red-green
                                // generated-normal: orange-green (normally invisible)
                                // generated-generated: yellow-green (normally invisible)
                                bool generated = !(Save && neighbor.Save);
                                bool fullyGenerated = !Save && !neighbor.Save;
                                DrawWaypointEdge(WorldMatrix.Translation, neighbor.WorldMatrix.Translation, generated ? fullyGenerated ? Color.Yellow : Color.Orange : Color.Red, Color.Green);
                                continue;
                            }
                        }
                    }
                }
            }
            return base.DebugDraw();
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            Ray ray = new Ray(line.From, line.Direction);
            float? ds = ray.Intersects(this.WorldAABB);
            if (ds == null)
                return false;
            v = line.From + line.Direction * ds;
            return true;
        }


        public override bool IsSelectableAsChild()
        {
            return true;
        }

        #endregion

        #region Visibility
        
        // Return whether a waypoint is visible from the current position. Ignores debris, ammo and ships.
        // You can provide a cache that contains previous query results for this position. Results of new queries will be added to the cache.
        public bool IsVisibleFrom(Vector3 position, MyEntity ignore = null, Dictionary<MyWayPoint, bool> visibilityCache = null)
        {
            bool visible;
            if (visibilityCache == null || !visibilityCache.TryGetValue(this, out visible))
            {
                if (Vector3.DistanceSquared(position, Position) < 0.01f)
                {
                    visible = true;
                }
                else
                {
                    var line = new MyLine(position, Position, true);
                    visible = (MyEntities.GetAnyIntersectionWithLine(ref line, ignore, null, true, true, true, false) == null);  // skip explosion debris, ammo, ships, closed doors that can open, dummies
                }

                if (visibilityCache != null) 
                    visibilityCache[this] = visible;
            }
            return visible;
        }
        
        #endregion

        // Get all vertices from a list of entities. O(n)
        public static List<MyWayPoint> FilterWayPoints(IEnumerable<MyEntity> entities)
        {
            var vertexList = new List<MyWayPoint>();
            foreach (var e in entities)
                if (e is MyWayPoint)
                    vertexList.Add(e as MyWayPoint);
            return vertexList;
        }

        // Return true if a list contains waypoints.
        public static bool ContainsWayPoint(IEnumerable<MyEntity> entities)
        {
            foreach (var e in entities)
                if (e is MyWayPoint)
                    return true;
            return false;
        }


        public List<MyWayPointPath> Paths()
        {
            var result = new List<MyWayPointPath>();
            foreach (var path in MyWayPointGraph.StoredPaths)
                if (path.WayPoints.Contains(this))
                    result.Add(path);
            return result;
        }

        #region Needs to be saved?

        public bool NeedsToBeSaved()
        {
            if (Save == false) return false;
            if (Parent == null) return true;

            // it has a prefab as a parent; check whether it corresponds to the default ones
            var prefab = Parent as MyPrefabBase;
            if (prefab == null) return true;  // wtf, waypoints should have only prefabs as parents

            var positions = new List<Vector3>();
            var names = new List<string>();
            var parentNames = new List<string>();
            prefab.GetDefaultWaypointData(positions, names, parentNames);

            for (int j = 0; j < positions.Count; j++)
            {
                var pos = positions[j];
                if (Vector3.DistanceSquared(pos, Position) < 1)
                {
                    // TODO: test whether all edges have originated from the parent prefab or snap points; also that no edges are missing
                    foreach (var n in Neighbors)
                    {
                        bool neighborOk = false;
                        if (n.Parent == Parent)
                        {
                            for (int k = 0; k < names.Count; k++)
                            {
                                // should be connected? (the neighbor doesn't have to be in its original position, but then I wouldn't know if it's the correct one)
                                if (Vector3.DistanceSquared(positions[k], n.Position) < 1 && (names[j].Equals(parentNames[k]) || names[k].Equals(parentNames[j])))
                                {
                                    neighborOk = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            /*
                            // if both are in the reach of a snappoint, they may be connected
                            foreach (var snappoint in prefab.SnapPoints)
                            {
                                var snappointWorldPosition = (snappoint.Matrix * snappoint.Prefab.WorldMatrix).Translation;

                                var wps = MyWayPointGraph.GetAllWaypointsInSphere(snappointWorldPosition, 20);
                                if (wps.Count == 2 && wps.Contains(this) && wps.Contains(n))
                                {
                                    neighborOk = true;
                                }
                            }
                            */
                        }
                        if (!neighborOk) return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Voxels
{
    class MyMeshSimplifier
    {
        private static MyMeshSimplifier m_instance;

        /// <summary>
        /// Lazy-loaded singleton.
        /// </summary>
        public static MyMeshSimplifier Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MyMeshSimplifier();
                }

                return m_instance;
            }
        }

        private const int TRIANGLE_EDGE_COUNT = 3;

        readonly List<List<int>> m_adjacentTriangleIndices = new List<List<int>>();

        /// <summary>
        /// A flag array indicating which triangles have been removed from the mesh.
        /// </summary>
        readonly HashSet<int> m_removedTriangles = new HashSet<int>();

        readonly List<bool> m_usedVertices = new List<bool>();

        readonly HashSet<short> m_helperHashSet = new HashSet<short>();

        private List<MyVoxelTriangle> m_triangles;
        private List<MyVoxelVertex> m_vertices;

        static float m_minEdgeLength = 15f;
        public static int VoxelRecalcTime;

        public static float MinEdgeLength
        {
            get { return m_minEdgeLength; }
            set
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                m_minEdgeLength = value;
                var voxelMap = MyVoxelMaps.GetLargestVoxelMap();
                if (voxelMap != null)
                {
                    voxelMap.InvalidateCache(new MyMwcVector3Int(0, 0, 0), voxelMap.Size);
                    voxelMap.PrepareRenderCellCache();
                }

                stopwatch.Stop();
                VoxelRecalcTime = (int) stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        public void SimplifyMesh(List<MyVoxelVertex> vertices, List<MyVoxelTriangle> triangles)
        {
            m_triangles = triangles;
            m_vertices = vertices;

            Debug.Assert(m_vertices.Count < short.MaxValue, "voxel map vertex has index > 16bit = 32767");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Initialize");
            Initialize();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("ComputeAdjacencies");
            ComputeAdjacencies();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("RemoveShortEdges");
            RemoveShortEdges(m_minEdgeLength);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("SkipDeletedVertices");
            SkipDeletedVertices();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddNonRemovedTriangles");
            AddNonRemovedTriangles(triangles);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void AddNonRemovedTriangles(List<MyVoxelTriangle> triangles)
        {
            var trianglesCount = m_triangles.Count - m_removedTriangles.Count;
            int trianglesAdded = 0;
            for (int i = 0; i < m_triangles.Count; i++)
            {
                if (m_removedTriangles.Contains(i))
                    continue;

                triangles[trianglesAdded++] = m_triangles[i];
            }

            //for (int i = 0; i < m_triangles.Count + m_removedTriangles.Count; i++)
            //{
            //    m_removedTriangles.Remove(i);
            //}

            triangles.RemoveRange(trianglesCount, m_removedTriangles.Count);
        }

        private void Initialize()
        {
            m_removedTriangles.Clear();
        }

        /// <summary>
        /// Shuffles the m_vertices array so that it does not contain 'holes' - indices of removed vertices.
        /// </summary>
        private void SkipDeletedVertices()
        {
            for (int i = 0; i < m_usedVertices.Count; i++)
            {
                m_usedVertices[i] = false;
            }

            for (int i = m_usedVertices.Count; i < m_vertices.Count; i++)
            {
                m_usedVertices.Add(false);
            }

            for (int triangleIndex = 0; triangleIndex < m_triangles.Count; triangleIndex++)
            {
                if (!m_removedTriangles.Contains(triangleIndex))
                {
                    var triangle = m_triangles[triangleIndex];
                    for (int i = 0; i < TRIANGLE_EDGE_COUNT; i++)
                    {
                        m_usedVertices[triangle[i]] = true;
                    }
                }
            }

            for (short i = 0; i < m_vertices.Count; i++)
            {
                if (!m_usedVertices[i])
                {
                    RemoveVertex(i);
                }
                m_usedVertices[i] = false;
            }
        }

        private void RemoveVertex(short index)
        {
            Debug.Assert(index >= 0 && index < m_vertices.Count);

            short replacementIndex = (short) m_vertices.Count;
            replacementIndex--;

            if (m_usedVertices[replacementIndex])
            {
                foreach (var triangleIndex in m_adjacentTriangleIndices[replacementIndex])
                {
                    ReplaceVertexIndex(triangleIndex, replacementIndex, index);
                }
            }

            m_vertices[index] = m_vertices[replacementIndex];
            m_vertices.RemoveAt(m_vertices.Count - 1);
        }

        /// <summary>
        /// Updates a triangle's reference to a vertex to a new one (reference = index).
        /// </summary>
        private void ReplaceVertexIndex(int triangleIndex, short oldIndex, short newIndex)
        {
            var triangle = m_triangles[triangleIndex];

            if (triangle.VertexIndex0 == oldIndex)
                triangle.VertexIndex0 = newIndex;

            if (triangle.VertexIndex1 == oldIndex)
                triangle.VertexIndex1 = newIndex;

            if (triangle.VertexIndex2 == oldIndex)
                triangle.VertexIndex2 = newIndex;

            m_triangles[triangleIndex] = triangle;
        }

        /// <summary>
        /// Locks a vertex, so that in the mesh decimation phase, it cannot be moved or removed.
        /// </summary>
        //private void LockVertex(MyEdgeVertex edgeVertex)
        //{
        //    m_verticesLocked[edgeVertex.VertexIndex] = true;

        //    // for visualization, give it blue color if it's locked
        //    //m_vertices[edgeVertex.VertexIndex].Material = MyMwcVoxelMaterialsEnum.Treasure_01;
        //}

        /// <summary>
        /// Decimates the mesh by removing edges whose length is shorter than <c>minEdgeLength</c>.
        /// </summary>
        private void RemoveShortEdges(float minEdgeLength)
        {
            float minEdgeLengthSquared = minEdgeLength * minEdgeLength;
            for (int triangleIndex = 0; triangleIndex < m_triangles.Count; triangleIndex++)
            {
                if (m_removedTriangles.Contains(triangleIndex))
                    continue;

                var triangle = m_triangles[triangleIndex];

                // iterate through triangle edges
                for (int i = 0; i < TRIANGLE_EDGE_COUNT; i++)
                {
                    var firstVertexIndex = triangle[i];
                    var secondVertexIndex = triangle[(i + 1) % TRIANGLE_EDGE_COUNT];
                    if (!IsVertexLocked(firstVertexIndex))
                    {
                        bool collapsed = CollapseIfCloseEnough(minEdgeLengthSquared, secondVertexIndex, firstVertexIndex);
                        if (collapsed)
                            break;
                    }
                    else
                    {
                        if (!IsVertexLocked(secondVertexIndex))
                        {
                            bool collapsed = CollapseIfCloseEnough(minEdgeLengthSquared, firstVertexIndex, secondVertexIndex);
                            if (collapsed)
                                break;
                        }
                    }
                }
            }
        }

        bool IsVertexLocked(short firstVertexIndex)
        {
            //return m_vertices[firstVertexIndex].OnRenderCellEdge;
            return false;
        }

        /// <summary>
        /// Collapses the edge defined by the two vertices if it is shorter than minEdgeLength.
        /// </summary>
        private bool CollapseIfCloseEnough(float minEdgeLengthSquared, short keptVertexIndex, short removedVertexIndex)
        {
            var keptPosition = m_vertices[keptVertexIndex].Position;
            var removedPosition = m_vertices[removedVertexIndex].Position;
            float distanceSquared = (keptPosition - removedPosition).LengthSquared();
            if (distanceSquared < minEdgeLengthSquared)
            {
                bool isRemovedEdgeintest = IsShortestInAdjacentPolygon(keptVertexIndex, removedVertexIndex, distanceSquared);
                if (isRemovedEdgeintest)
                {
                    CollapseEdge(keptVertexIndex, removedVertexIndex);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// TODO rename this function - it not only checks, but also collapses the shortest if necessary
        /// </summary>
        private bool IsShortestInAdjacentPolygon(short keptVertexIndex, short removedVertexIndex, float edgeLengthSquared)
        {
            m_helperHashSet.Clear();
            foreach (var triangleIndex in m_adjacentTriangleIndices[removedVertexIndex])
            {
                if (m_removedTriangles.Contains(triangleIndex))
                {
                    Debug.Fail("Did you forget to remove triangle?");
                }

                for (int i = 0; i < TRIANGLE_EDGE_COUNT; i++)
                {
                    var neighborIndex = m_triangles[triangleIndex][i];
                    if (neighborIndex != removedVertexIndex && neighborIndex != keptVertexIndex)
                    {
                        m_helperHashSet.Add(neighborIndex);
                    }
                }
            }

            float minDistanceSquared = edgeLengthSquared;
            Edge minEdge;
            minEdge.VertexIndex0 = keptVertexIndex;
            minEdge.VertexIndex1 = removedVertexIndex;
            foreach (var neighborVertexIndex in m_helperHashSet)
            {
                var neighbor = m_vertices[neighborVertexIndex];
                float distanceSquared = Vector3.DistanceSquared(neighbor.Position, m_vertices[removedVertexIndex].Position);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    minEdge.VertexIndex0 = neighborVertexIndex;
                }
            }

            if (minDistanceSquared < edgeLengthSquared)
            {
                CollapseEdge(minEdge.VertexIndex0, minEdge.VertexIndex1);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the coord is at the boundary of the current data cell.
        /// </summary>
        //private bool AtDataCellBoundary(MyMwcVector3Int coord)
        //{
        //    return coord.X <= 0 || coord.X >= m_polygCubesX - 2 ||
        //           coord.Y <= 0 || coord.Y >= m_polygCubesY - 2 ||
        //           coord.Z <= 0 || coord.Z >= m_polygCubesZ - 2;
        //}

        struct Edge
        {
            public short VertexIndex0;
            public short VertexIndex1;
        }

        /// <summary>
        /// Collapses an edge defined by its two endpoint vertices.
        /// </summary>
        /// <param name="keptVertexIndex">The index of the vertex that will NOT be removed in the process.
        /// However, it will be moved if it's not locked.</param>
        /// <param name="removedVertexIndex">The index of the vertex that WILL be removed in the process.</param>
        private void CollapseEdge(short keptVertexIndex, short removedVertexIndex)
        {
            Debug.Assert(!IsVertexLocked(removedVertexIndex));

            // if I can move the kept vertex, make its new position the average of the previous positions
            bool interpolate = !IsVertexLocked(keptVertexIndex);

            if (interpolate)
            {
                var keptVertex = m_vertices[keptVertexIndex];

                keptVertex.Position = .5f *
                    (keptVertex.Position + m_vertices[removedVertexIndex].Position);

                keptVertex.Normal = MyMwcUtils.Normalize(
                    (keptVertex.Normal + m_vertices[removedVertexIndex].Normal));

                m_vertices[keptVertexIndex] = keptVertex;
            }

            var removedVertexTriangles = m_adjacentTriangleIndices[removedVertexIndex];
            for (int i = removedVertexTriangles.Count - 1; i >= 0; i--)
            {
                var triangleIndex = removedVertexTriangles[i];
                var triangle = m_triangles[triangleIndex];

                if (triangle.VertexIndex0 == removedVertexIndex)
                {
                    triangle.VertexIndex0 = keptVertexIndex;
                    Debug.Assert(keptVertexIndex < m_vertices.Count);
                }
                if (triangle.VertexIndex1 == removedVertexIndex)
                {
                    triangle.VertexIndex1 = keptVertexIndex;
                    Debug.Assert(keptVertexIndex < m_vertices.Count);
                }
                if (triangle.VertexIndex2 == removedVertexIndex)
                {
                    triangle.VertexIndex2 = keptVertexIndex;
                    Debug.Assert(keptVertexIndex < m_vertices.Count);
                }

                // todo find out why test for zero-surface does not work
                if (IsDegenerated(triangle) /*|| HasZeroSurface(triangle)*/)
                {
                    // delete triangle
                    RemoveTriangle(triangleIndex);
                }
                else
                {
                    // update triangle
                    m_triangles[triangleIndex] = triangle;
                    if (!m_adjacentTriangleIndices[keptVertexIndex].Contains(triangleIndex))
                        m_adjacentTriangleIndices[keptVertexIndex].Add(triangleIndex);
                }
            }
        }

        /// <summary>
        /// Returns true if the given triangle is defined by less than three distincs vertex indices.
        /// </summary>
        private bool IsDegenerated(MyVoxelTriangle triangle)
        {
            return triangle.VertexIndex0 == triangle.VertexIndex1 ||
                   triangle.VertexIndex0 == triangle.VertexIndex2 ||
                   triangle.VertexIndex1 == triangle.VertexIndex2;
        }

        /// <summary>
        /// Returns true if triangle has very close to zero surface area.
        /// </summary>
        private bool HasZeroSurface(MyVoxelTriangle triangle)
        {
            var vertex0 = m_vertices[triangle.VertexIndex0];
            var vertex1 = m_vertices[triangle.VertexIndex1];
            var vertex2 = m_vertices[triangle.VertexIndex2];

            var vector1 = vertex1.Position - vertex0.Position;
            vector1.Normalize();
            var vector2 = vertex2.Position - vertex0.Position;
            vector2.Normalize();
            var dot = Vector3.Dot(vector1, vector2);
            if (Math.Abs(dot) > 0.9999)
                return true;

            return false;
        }

        /// <summary>
        /// Remove a triangle by removing its adjacency information as well as 
        /// flagging it as deleted in the m_removedTriangles array.
        /// </summary>
        private void RemoveTriangle(int triangleIndex)
        {
            Debug.Assert(!m_removedTriangles.Contains(triangleIndex));

            var triangle = m_triangles[triangleIndex];
            RemoveAdjacency(triangle.VertexIndex0, triangleIndex);
            RemoveAdjacency(triangle.VertexIndex1, triangleIndex);
            RemoveAdjacency(triangle.VertexIndex2, triangleIndex);

            m_removedTriangles.Add(triangleIndex);
        }

        /// <summary>
        /// Removes the stored adjacency for the given vertex and triangle.
        /// </summary>
        private void RemoveAdjacency(int vertexIndex, int triangleIndex)
        {
            m_adjacentTriangleIndices[vertexIndex].Remove(triangleIndex);
        }

        /// <summary>
        /// Fills the m_adjacentTriangleIndices array, which associates with each vertex a list
        /// of triangles (by index) that are adjacent to the vertex.
        /// </summary>
        void ComputeAdjacencies()
        {
            for (int i = 0; i < m_adjacentTriangleIndices.Count; i++)
            {
                m_adjacentTriangleIndices[i].Clear();
            }

            for (int i = m_adjacentTriangleIndices.Count; i < m_vertices.Count; i++)
            {
                m_adjacentTriangleIndices.Add(new List<int>());
            }

            for (int faceIndex = 0; faceIndex < m_triangles.Count; faceIndex++)
            {
                var triangle = m_triangles[faceIndex];

                m_adjacentTriangleIndices[triangle.VertexIndex0].Add(faceIndex);

                m_adjacentTriangleIndices[triangle.VertexIndex1].Add(faceIndex);

                m_adjacentTriangleIndices[triangle.VertexIndex2].Add(faceIndex);
            }
        }
    }
}
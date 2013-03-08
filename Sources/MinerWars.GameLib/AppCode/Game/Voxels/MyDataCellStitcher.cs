using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Voxels
{
    static class MyDataCellStitcher
    {
        public static List<MyVoxelTriangle> Triangles = new List<MyVoxelTriangle>();
        public static List<MyVoxelVertex> Vertices = new List<MyVoxelVertex>();

        private static readonly Dictionary<MyVoxelVertex, short> m_verticesMap = new Dictionary<MyVoxelVertex, short>();

        private static readonly List<short> m_remapHelper = new List<short>();

        public static void StitchDataCells(List<MyVoxelCacheCellData> dataCells)
        {
            Vertices.Clear();
            Triangles.Clear();
            m_verticesMap.Clear();

            //int vertexCount = 0;
            //foreach (MyVoxelCacheCellData dataCell in dataCells)
            //{
            //    m_remapHelper.Clear();

            //    for (int vertexIndex = 0; vertexIndex < dataCell.VoxelVerticesCount; vertexIndex++)
            //    {
            //        var vertex = dataCell.VoxelVertexes[vertexIndex];
            //        Vertices.Add(vertex);
            //    }

            //    for (int triangleIndex = 0; triangleIndex < dataCell.VoxelTrianglesCount; triangleIndex++)
            //    {
            //        var triangle = dataCell.VoxelTriangles[triangleIndex];
            //        MyVoxelTriangle32 newTriangle = new MyVoxelTriangle32();
            //        for (int i = 0; i < 3; i++)
            //        {
            //            var index = triangle[i];
            //            newTriangle[i] = index + vertexCount;
            //        }
            //        Triangles.Add(newTriangle);
            //    }

            //    vertexCount = Vertices.Count;
            //}

            for (int dataCellIndex = 0; dataCellIndex < dataCells.Count; dataCellIndex++)
            {
                m_remapHelper.Clear();
                var dataCell = dataCells[dataCellIndex];

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Stitch Data Cells - first cycle");

                for (int vertexIndex = 0; vertexIndex < dataCell.VoxelVerticesCount; vertexIndex++)
                {
                    var vertex = dataCell.VoxelVertices[vertexIndex];
                    short index;
                    if (m_verticesMap.TryGetValue(vertex, out index))
                    {
                        m_remapHelper.Add(index);
                    }
                    else
                    {
                        Debug.Assert(Vertices.Count < short.MaxValue);
                        m_verticesMap.Add(vertex, (short) Vertices.Count);
                        m_remapHelper.Add((short) Vertices.Count);
                        Vertices.Add(vertex);
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Stitch Data Cells - second cycle");
                for (int triangleIndex = 0; triangleIndex < dataCell.VoxelTrianglesCount; triangleIndex++)
                {
                    var triangle = dataCell.VoxelTriangles[triangleIndex];
                    MyVoxelTriangle newTriangle = new MyVoxelTriangle();
                    for (int i = 0; i < 3; i++)
                    {
                        var index = triangle[i];
                        newTriangle[i] = m_remapHelper[index];
                    }
                    Triangles.Add(newTriangle);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }
    }
}
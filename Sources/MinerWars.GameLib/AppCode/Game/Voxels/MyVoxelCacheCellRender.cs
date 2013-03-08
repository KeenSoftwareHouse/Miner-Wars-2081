using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

//  Caching voxel cell vertex buffers.
//
//  Voxel cell cache implemented as preallocated array of cached cells - that can be accessed be key (calculated from voxel map Id and cell coordinates).
//  All methods are O(1). Class can hold cached cells for any voxel map.
//
//  At the beginning, cell caches are preallocated (they contain vertex buffer, etc). Later when user wants to display particular cell, he looks
//  into the cache if that cell isn't already calculated. If yes, he gets reference and can draw its vertex buffer. If not, he asks for one preallocated
//  cell - and he is given cell with lowest priority.
//  After that, he updates the cell - that means, he tells he needed this cell in cache so it should be stored for future (cell is given higher priority and
//  there is less chance it's removed in near future).
//  If he decides to remove cell from cache (e.g. after explosion), cell is freed and marked as available for new allocation - its priority is lowered, so
//  it's very probable it will be allocated when next time new cell will be needed.
//
//  Using this approach, frequently drawn cells are almost always in the cache. As user moves through the level, older cells are removed and new are added.
//  If cell is changed, we need to invalidate it in cache and use it's place for another cell (or same).
//  Remember: size of cache is constant, so this is sort of a better circular buffer (just with priorities, etc).
//
//  Added 8.6.2008: Now I can decide if one render-cell-element stored LOD0 cell data or LOD1 version. So one render cell can be stored in render cache
//  two times - once for detail version LOD0 and once for LOD1 version. But they are separate, so if we don't need LOD1 version, it may be freed and not be
//  calculated until we need it again. E.g. if player is near voxel map to which he is making a tunnel, we are updating LOD0 cached version. All players
//  that are in distance of this voxel map update just their LOD1 version, which is fast.


namespace MinerWars.AppCode.Game.Voxels
{
    using Byte4 = MinerWarsMath.Graphics.PackedVector.Byte4;
    using HalfVector2 = MinerWarsMath.Graphics.PackedVector.HalfVector2;
    using HalfVector4 = MinerWarsMath.Graphics.PackedVector.HalfVector4;
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    //  IMPORTANT: Order of this enum values is important. It has impact on sorting batches.
    enum MyVoxelCacheCellRenderBatchType : byte
    {
        MULTI_MATERIAL_CLEAR = 0,
        SINGLE_MATERIAL = 1,
        MULTI_MATERIAL = 2
    }

    enum MyVoxelCellQueryMode
    {
        LoadImmediately,
        StartLoadBackground,
        DoNotLoad,
    }

    //  One cell stored in a cache
    class MyVoxelCacheCellRender
    {
        public MyVoxelMap VoxelMap;
        public Vector3 Center;          //  We need render cell center for sorting by distance
        public MyMwcVector3Int CellCoord;
        public List<MyVoxelCacheCellRenderBatch> Batches;
        public MyLodTypeEnum CellHashType;        //  This will tell us if this render-cell-item stores normal data or LOD1
        public bool Contains;       //  True if this render cell cache actualy stores some render cell. False if not and is waiting for its time (because it was just reseted)
        public static readonly int VoxelMaterialCount = MyVoxelMaterials.GetMaterialsCount();

        /// <summary>
        /// Result of the occlusion query for this entity.
        /// </summary>
        private BoundingBox m_boundingBox;
        public BoundingBox CellBoundingBox
        {
            get
            {
                return m_boundingBox;
            }

            set
            {
                m_boundingBox = value;
            }
        }

        public MyVoxelCacheCellRender()
        {
            Batches = new List<MyVoxelCacheCellRenderBatch>(MyVoxelConstants.PREALLOCATED_RENDER_CELL_BATCHES);
            Contains = false;
        }

        //  This methods needs to be called after every cell cache released!! It releases vertex buffer. It's important, 
        //  because when this cell cache will be associated to a new cell, not same material vertex buffer will be used so unused needs to be disposed!!
        public void Reset()
        {
            if (Batches != null)
            {
                for (int i = 0; i < Batches.Count; i++)
                {
                    MyVoxelCacheCellRenderBatch batch = Batches[i];

                    if (batch != null)
                    {
                        if (batch.VertexBuffer != null)
                        {
                            //  Dispose and set to null, so GC can take it
                            batch.VertexBuffer.Dispose();
                            batch.VertexBuffer = null;
                            MyPerformanceCounter.PerAppLifetime.VoxelVertexBuffersSize -= batch.VertexBufferSize;
                            batch.VertexBufferSize = 0;
                        }

                        if (batch.IndexBuffer != null)
                        {
                            //  Dispose and set to null, so GC can take it
                            batch.IndexBuffer.Dispose();
                            batch.IndexBuffer = null;
                            MyPerformanceCounter.PerAppLifetime.VoxelIndexBuffersSize -= batch.IndexBufferSize;
                            batch.IndexBufferSize = 0;
                        }
                    }
                }

                Contains = false;
                Batches.Clear();
            }
        }

        public void Begin(MyVoxelMap voxelMap, ref MyMwcVector3Int cellCoord)
        {
            VoxelMap = voxelMap;
            CellCoord = cellCoord;
            Center = voxelMap.GetRenderCellCenterPositionAbsolute(ref cellCoord);
            Contains = true;
            MyVoxelCacheCellRenderHelper.Begin();
        }

        public void End()
        {
            foreach (MySingleMaterialHelper materialHelper in MyVoxelCacheCellRenderHelper.GetSingleMaterialHelpers())
            {
                if (materialHelper != null && materialHelper.IndexCount > 0) 
                    EndSingleMaterial(materialHelper);
            }

            foreach (var pair in MyVoxelCacheCellRenderHelper.GetMultiMaterialHelpers())
            {
                if (pair.Value.VertexCount > 0)
                    EndMultiMaterial(pair.Value);
            }

            //if (materialHelper.IndexCount > 0) EndMultiMaterial(materialHelper.Material);

            //  SortForSAP batches by type and material
            Batches.Sort();
        }

        void EndSingleMaterial(MySingleMaterialHelper materialHelper)
        {
            if (materialHelper.IndexCount > 0 && materialHelper.VertexCount > 0)
            {
                //  This will just preload textures used by this material - so they are ready in memory when first time drawn
                MyVoxelMaterials.Get(materialHelper.Material).GetTextures();

                MyVoxelCacheCellRenderBatch newBatch = new MyVoxelCacheCellRenderBatch();
                //  Vertex buffer
                newBatch.VertexBufferCount = materialHelper.VertexCount;
                newBatch.VertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, MyVertexFormatVoxelSingleMaterial.Stride * newBatch.VertexBufferCount, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                newBatch.VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(materialHelper.Vertices, 0, newBatch.VertexBufferCount);
                newBatch.VertexBuffer.Unlock();
                newBatch.VertexBuffer.Tag = newBatch;
                newBatch.VertexBuffer.DebugName = "VoxelBatchSingle";
                newBatch.VertexBufferSize = materialHelper.VertexCount * MyVertexFormatVoxelSingleMaterial.Stride;
                MyPerformanceCounter.PerAppLifetime.VoxelVertexBuffersSize += newBatch.VertexBufferSize;

                //  Index buffer
                newBatch.IndexBufferCount = materialHelper.IndexCount;
                newBatch.IndexBuffer = new IndexBuffer(MyMinerGame.Static.GraphicsDevice, newBatch.IndexBufferCount * sizeof(short), Usage.WriteOnly, Pool.Default, true);
                newBatch.IndexBuffer.Lock(0, 0, LockFlags.None).WriteRange(materialHelper.Indices, 0, newBatch.IndexBufferCount);
                newBatch.IndexBuffer.Unlock();
                newBatch.IndexBuffer.DebugName = "VoxelBatchSingle";
                newBatch.IndexBufferSize = materialHelper.IndexCount * sizeof(short);
                MyPerformanceCounter.PerAppLifetime.VoxelIndexBuffersSize += newBatch.IndexBufferSize;

                newBatch.Type = MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL;
                newBatch.Material0 = materialHelper.Material;
                newBatch.Material1 = null;
                newBatch.Material2 = null;
                newBatch.UpdateSortOrder();

                Batches.Add(newBatch);
            }
            //  Reset helper arrays, so we can start adding triangles to them again
            materialHelper.IndexCount = 0;
            materialHelper.VertexCount = 0;
            MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookupCount[(int)materialHelper.Material]++;
        }

        void EndMultiMaterial(MyMultiMaterialHelper helper)
        {
            if (helper.VertexCount > 0)
            {
                //  This will just preload textures used by this material - so they are ready in memory when first time drawn
                MyVoxelMaterials.Get(helper.Material0).GetTextures();
                MyVoxelMaterials.Get(helper.Material1).GetTextures();
                MyVoxelMaterials.Get(helper.Material2).GetTextures();

                MyVoxelCacheCellRenderBatch newBatch = new MyVoxelCacheCellRenderBatch();

                //  Vertex buffer
                newBatch.VertexBufferCount = helper.VertexCount;
                newBatch.VertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, MyVertexFormatVoxelSingleMaterial.Stride * newBatch.VertexBufferCount, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                newBatch.VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(helper.Vertices, 0, newBatch.VertexBufferCount);
                newBatch.VertexBuffer.Unlock();
                newBatch.VertexBuffer.Tag = this;
                newBatch.VertexBuffer.DebugName = "VoxelBatchMulti";
                newBatch.VertexBufferSize = helper.VertexCount * MyVertexFormatVoxelSingleMaterial.Stride;
                MyPerformanceCounter.PerAppLifetime.VoxelVertexBuffersSize += newBatch.VertexBufferSize;

                // Index buffer (because everything must have IB)
                newBatch.IndexBufferCount = helper.VertexCount;
                newBatch.IndexBuffer = new IndexBuffer(MyMinerGame.Static.GraphicsDevice, newBatch.IndexBufferCount * sizeof(short), Usage.WriteOnly, Pool.Default, true);

                short[] indices = new short[helper.VertexCount];
                for (short i = 0; i < indices.Length; i++)
                {
                    indices[i] = i;
                }
                newBatch.IndexBuffer.Lock(0,0, LockFlags.None).WriteRange(indices);
                newBatch.IndexBuffer.Unlock();
                newBatch.IndexBuffer.DebugName = "VoxelBatchMulti";
                newBatch.IndexBuffer.Tag = this;
                newBatch.IndexBufferSize = helper.VertexCount * sizeof(short);
                MyPerformanceCounter.PerAppLifetime.VoxelIndexBuffersSize += newBatch.IndexBufferSize;

                newBatch.Type = MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL;
                newBatch.Material0 = helper.Material0;
                newBatch.Material1 = helper.Material1;
                newBatch.Material2 = helper.Material2;
                newBatch.UpdateSortOrder();

                Batches.Add(newBatch);
            }

            //  Reset helper arrays, so we can start adding triangles to them again
            helper.VertexCount = 0;
        }

        public static int GetMultimaterialId(MyMwcVoxelMaterialsEnum mat0, MyMwcVoxelMaterialsEnum mat1, MyMwcVoxelMaterialsEnum mat2)
        {
            int i0 = (int)mat0;
            int i1 = (int)mat1;
            int i2 = (int)mat2;

            if (i0 > i1)
            {
                MyUtils.Swap(ref i0, ref i1);
            }
            if (i1 > i2)
            {
                MyUtils.Swap(ref i1, ref i2);
            }
            if (i0 > i1)
            {
                MyUtils.Swap(ref i0, ref i1);
            }
            return i0 + i1 * VoxelMaterialCount + i2 * VoxelMaterialCount * VoxelMaterialCount;
        }

        private static void GetMultimaterialsFromId(int id, out MyMwcVoxelMaterialsEnum mat0, out MyMwcVoxelMaterialsEnum mat1, out MyMwcVoxelMaterialsEnum mat2)
        {
            int div = id;
            mat0 = (MyMwcVoxelMaterialsEnum)(div % VoxelMaterialCount);
            div /= VoxelMaterialCount;
            mat1 = (MyMwcVoxelMaterialsEnum)(div % VoxelMaterialCount);
            div /= VoxelMaterialCount;
            mat2 = (MyMwcVoxelMaterialsEnum)(div % VoxelMaterialCount);
        }
   
        public void AddTriangles(List<MyVoxelVertex> vertices, List<MyVoxelTriangle> triangles)
        {
            //Removed because it was duplicate of AddTriangles below
        }

        //  This method adds triangles from one data cell into this render cell. Single-texture triangles are added using indices (so we use m_notCompressedIndex buffer).
        //  For this we need to find indices. We use lookup array for it.
        //  Now we support only 16-bit indices, so vertex buffer can't have more then short.MaxValue vertices.
        public void AddTriangles(List<MyVoxelCacheCellData> cacheDataArray)
        {


            //     MyPerformanceTimer.VoxelGpuBuffersBuild.Start();


            //MyMwcVoxelMaterialsEnum? CurrentSingleMaterial = null;
            //            bool triangleAdded = true;

            // while (triangleAdded)
            //  {
            //   triangleAdded = false;
            //CurrentSingleMaterial = null;


            foreach (var cacheData in cacheDataArray)
            {
                //  Increase lookup count, so we will think that all vertexes in helper arrays are new
                for (int i = 0; i < MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookupCount.Length; i++)
                {
                    MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookupCount[i]++;
                }

                for (int i = 0; i < cacheData.VoxelTrianglesCount; i++)
                {
                    MyVoxelTriangle triangle = cacheData.VoxelTriangles[i];
                    MyVoxelVertex vertex0 = cacheData.VoxelVertices[triangle.VertexIndex0];
                    MyVoxelVertex vertex1 = cacheData.VoxelVertices[triangle.VertexIndex1];
                    MyVoxelVertex vertex2 = cacheData.VoxelVertices[triangle.VertexIndex2];

                    if ((vertex0.Material == vertex1.Material) && (vertex0.Material == vertex2.Material))
                    {
                        int matIndex = (int)vertex0.Material;

                        //  This is single-texture triangleVertexes, so we can choose material from any edge
                        MySingleMaterialHelper materialHelper = MyVoxelCacheCellRenderHelper.GetForMaterial(vertex0.Material);

                        //  Add vertex0 to vertex buffer
                        AddVertexToBuffer(materialHelper, ref vertex0, matIndex, triangle.VertexIndex0);

                        //  Add vertex1 to vertex buffer
                        AddVertexToBuffer(materialHelper, ref vertex1, matIndex, triangle.VertexIndex1);

                        //  Add vertex2 to vertex buffer
                        AddVertexToBuffer(materialHelper, ref vertex2, matIndex, triangle.VertexIndex2);

                        //triangleAdded = true;

                        //  Add indices
                        int nextTriangleIndex = materialHelper.IndexCount;
                        materialHelper.Indices[nextTriangleIndex + 0] = MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][triangle.VertexIndex0].VertexIndex;
                        materialHelper.Indices[nextTriangleIndex + 1] = MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][triangle.VertexIndex1].VertexIndex;
                        materialHelper.Indices[nextTriangleIndex + 2] = MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][triangle.VertexIndex2].VertexIndex;
                        materialHelper.IndexCount += 3;

                        if ((materialHelper.VertexCount >= MyVoxelCacheCellRenderHelper.MAX_VERTICES_COUNT_STOP) ||
                            (materialHelper.IndexCount >= MyVoxelCacheCellRenderHelper.MAX_INDICES_COUNT_STOP))
                        {
                            //  If this batch is almost full (or is full), we end it and start with new one
                            EndSingleMaterial(materialHelper);
                        }
                    }
                    else
                    {
                        int id = GetMultimaterialId(vertex0.Material, vertex1.Material, vertex2.Material);
                        // Assign current material
                        MyMultiMaterialHelper multiMaterialHelper = MyVoxelCacheCellRenderHelper.GetForMultimaterial(vertex0.Material, vertex1.Material, vertex2.Material);
  
                        //triangleAdded = true;

#if PACKED_VERTEX_FORMAT
                        // Copy packed normals
                        multiMaterialHelper.Vertices[multiMaterialHelper.VertexCount + 0].PackedNormal = vertex0.PackedNormal;
                        multiMaterialHelper.Vertices[multiMaterialHelper.VertexCount + 1].PackedNormal = vertex0.PackedNormal;
                        multiMaterialHelper.Vertices[multiMaterialHelper.VertexCount + 2].PackedNormal = vertex0.PackedNormal;
#endif

                        multiMaterialHelper.AddVertex(ref vertex0);
                        multiMaterialHelper.AddVertex(ref vertex1);
                        multiMaterialHelper.AddVertex(ref vertex2);

                        if (multiMaterialHelper.VertexCount >= MyVoxelCacheCellRenderHelper.MAX_VERTICES_COUNT_STOP)
                        {
                            EndMultiMaterial(multiMaterialHelper);
                        }
                    }
                }
                     /*
                if (multiMaterialHelper != null)
                {
                    int id = GetMultimaterialId(multiMaterialHelper.Material0, multiMaterialHelper.Material1, multiMaterialHelper.Material2);
                    MyVoxelCacheCellRenderHelper.FinishedMultiMaterials[id] = true;
                    EndMultimaterial(multiMaterialHelper);
                }

                if (singleMaterialHelper != null)
                {
                    MyVoxelCacheCellRenderHelper.FinishedSingleMaterials[(int)singleMaterialHelper.Material] = true;
                    EndSingleMaterial(singleMaterialHelper);
                }      */
            }


            //    }
            //MyPerformanceTimer.VoxelGpuBuffersBuild.End();
        }

        private static void AddVertexToBuffer(MySingleMaterialHelper materialHelper, ref MyVoxelVertex vertex0,
            int matIndex, short vertexIndex0)
        {
            if (MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][vertexIndex0].CalcCounter !=
                MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookupCount[matIndex])
            {
                int nextVertexIndex = materialHelper.VertexCount;

                //  Short overflow check
                System.Diagnostics.Debug.Assert(nextVertexIndex <= short.MaxValue);

                // copy position and ambient
                materialHelper.Vertices[nextVertexIndex].m_positionAndAmbient = vertex0.m_positionAndAmbient;
                materialHelper.Vertices[nextVertexIndex].Ambient = vertex0.Ambient;

                // Copy normal
#if PACKED_VERTEX_FORMAT
                materialHelper.Vertices[nextVertexIndex].m_normal = vertex0.m_normal;
#else
                materialHelper.Vertices[nextVertexIndex].Normal = vertex0.Normal;
#endif

                MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][vertexIndex0].CalcCounter =
                    MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookupCount[matIndex];
                MyVoxelCacheCellRenderHelper.SingleMaterialIndicesLookup[matIndex][vertexIndex0].VertexIndex =
                    (short) nextVertexIndex;

                materialHelper.VertexCount++;
            }
        }
    }

    //  Container of all cells stored in voxel cache
    static class MyVoxelCacheRender
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //  Access cached cell by key
        static Dictionary<Int64, LinkedListNode<MyVoxelCacheCellRender>> m_cellsByCoordinate = null;

        //  Here we preallocate cell caches
        static MyVoxelCacheCellRender[] m_cellsPreallocated = null;

        //  Linked list used to allocate, remove and update voxel cell in O(1) time.
        static LinkedList<MyVoxelCacheCellRender> m_priority;
        static LinkedListNode<MyVoxelCacheCellRender>[] m_priorityArray;

        static object m_priorityLocker = new object();

        //  Capacity of this cell cache
        static int m_capacity;

        //  Used for creating LOD1 version only
        static MyVoxelCacheCellData m_helperLodCachedDataCell;

        static List<MyVoxelCacheCellData> m_dataCellsQueue;

        static readonly ConcurrentQueue<RenderCellLoadJob> m_queue;
        //static AutoResetEvent m_event;
        private static HashSet<long> m_cellsBeingLoaded;

        static MyVoxelCacheRender()
        {
            m_cellsBeingLoaded = new HashSet<long>();

            m_queue = new ConcurrentQueue<RenderCellLoadJob>();
          //  m_event = new AutoResetEvent(false);
          //  Task.Factory.StartNew(BackgroundTask, TaskCreationOptions.PreferFairness);
        }
               /*
        static void BackgroundTask()
        {
            while (true)
            {
                RenderCellLoadJob next;
                if (m_queue.TryDequeue(out next))
                {
                    long key = MyVoxelMaps.GetCellHashCode(next.VoxelMap.VoxelMapId, ref next.RenderCellCoord, next.CellHashType);
                    LoadCell(next.VoxelMap, ref next.RenderCellCoord, next.CellHashType);
                    m_cellsBeingLoaded.Remove(key);
                }
                else
                {
                    m_event.WaitOne();
                }
            }
        }        */

        private class RenderCellLoadJob
        {
            public MyVoxelMap VoxelMap;
            public MyMwcVector3Int RenderCellCoord;
            public MyLodTypeEnum CellHashType;

            public RenderCellLoadJob(MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyLodTypeEnum cellHashType)
            {
                VoxelMap = voxelMap;
                RenderCellCoord = renderCellCoord;
                CellHashType = cellHashType;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCacheRender.LoadData");

            MyMwcLog.WriteLine("MyVoxelCacheRender.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            m_capacity = MyVoxelConstants.VOXEL_RENDER_CELL_CACHE_SIZE;
            m_cellsByCoordinate = new Dictionary<Int64, LinkedListNode<MyVoxelCacheCellRender>>(m_capacity);
            m_priority = new LinkedList<MyVoxelCacheCellRender>();
            m_priorityArray = new LinkedListNode<MyVoxelCacheCellRender>[m_capacity];
            m_cellsPreallocated = new MyVoxelCacheCellRender[m_capacity];
            m_helperLodCachedDataCell = new MyVoxelCacheCellData();
            m_dataCellsQueue = new List<MyVoxelCacheCellData>(MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_DATA_CELLS_TOTAL);
            for (int i = 0; i < m_capacity; i++)
            {
                m_cellsPreallocated[i] = new MyVoxelCacheCellRender();
                m_priorityArray[i] = new LinkedListNode<MyVoxelCacheCellRender>(m_cellsPreallocated[i]);
                m_priority.AddLast(m_priorityArray[i]);
            }
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheRender.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_cellsPreallocated = null;
            m_priority = null;
            m_priorityArray = null;
            m_dataCellsQueue.Clear();
            m_helperLodCachedDataCell = null;
        }

        //  Initialized new voxel cell cache. Capacity specified count of voxel cells in the cache.
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyVoxelCacheRender.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCacheRender::LoadContent");



            foreach (KeyValuePair<Int64, LinkedListNode<MyVoxelCacheCellRender>> kvp in m_cellsByCoordinate)
            {
                //kvp.Value.Value.Reset();
                m_priority.Remove(kvp.Value.Value);
                m_priority.AddFirst(kvp.Value.Value);
            }
            m_cellsByCoordinate.Clear();//forces to recreate all rendering cells on next call



            //this is here becouse it worked and is probably right way how to invalidate this stuff (really dont know why)
            foreach (MyVoxelMap map in MyVoxelMaps.GetVoxelMaps())
            {
                map.InvalidateCache(new MyMwcVector3Int(-1000, -1000, -1000), new MyMwcVector3Int(1000, 1000, 1000));
            }


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheRender.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyVoxelCacheRender.UnloadContent - START");
            MyMwcLog.IncreaseIndent();
            if (m_cellsPreallocated != null)
            {
                for (int i = 0; i < m_cellsPreallocated.Length; i++)
                {
                    if (m_cellsPreallocated[i] != null)
                    {
                        //  Dispose vertex buffers
                        m_cellsPreallocated[i].Reset();
                    }
                }
            }
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheRender.UnloadContent - END");
        }

        public static MyVoxelCacheCellRender GetCell(
            MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyLodTypeEnum cellHashType, MyVoxelCellQueryMode loadingMode = MyVoxelCellQueryMode.LoadImmediately)
        {
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetCellFromCache");
            MyVoxelCacheCellRender ret = GetCellFromCache(voxelMap.VoxelMapId, ref renderCellCoord, cellHashType);
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (ret == null)
            {
                switch (loadingMode)
                {
                    case MyVoxelCellQueryMode.LoadImmediately:
                        ret = LoadCell(voxelMap, ref renderCellCoord, cellHashType);
                        break;
                    case MyVoxelCellQueryMode.StartLoadBackground:
                        TryLoadCellInBackground(voxelMap, ref renderCellCoord, cellHashType);
                        break;
                    case MyVoxelCellQueryMode.DoNotLoad:
                        // do nothing
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("loadingMode");
                }
            }

            if (ret != null)
            {
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCacheRender.UpdateCell");
                UpdateCell(voxelMap.VoxelMapId, ref renderCellCoord, cellHashType);

                         /*
                BoundingBox box;
                voxelMap.GetRenderCellBoundingBox(ref renderCellCoord, out box);


            //MyDebugDraw.DrawAABBLowRes(ref box, ref v, 1);

                 MyMwcVector3Int cellCoord;
                 MyMwcVector3Int cellCoordMin = voxelMap.GetDataCellCoordinateFromMeters(ref box.Min);
                 MyMwcVector3Int cellCoordMax = voxelMap.GetDataCellCoordinateFromMeters(ref box.Max);

            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        if (MyVoxelCacheData.GetCellFromCache(voxelMap.VoxelMapId, ref cellCoord) != null)
                        {
                            MyVoxelCacheData.UpdateCell(voxelMap.VoxelMapId, ref cellCoord);
                        }
                    }
                }
            }          */



                       /*
                MyMwcVector3Int cellCoord;
                for (cellCoord.X = 0; cellCoord.X < voxelMap.DataCellsCount.X; cellCoord.X++)
                {
                    for (cellCoord.Y = 0; cellCoord.Y < voxelMap.DataCellsCount.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = 0; cellCoord.Z < voxelMap.DataCellsCount.Z; cellCoord.Z++)
                        {
                            if (MyVoxelCacheData.GetCellFromCache(voxelMap.VoxelMapId, ref cellCoord) != null)
                            {
                                MyVoxelCacheData.UpdateCell(voxelMap.VoxelMapId, ref cellCoord);
                            }
                        }
                    }
                }        */
                      /*
                // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                MyMwcVector3Int voxelCoord = new MyMwcVector3Int(renderCellCoord.X * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, renderCellCoord.Y * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, renderCellCoord.Z * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS);
                var datacellCoord = voxelMap.GetDataCellCoordinate(ref voxelCoord);
                if (MyVoxelCacheData.GetCellFromCache(voxelMap.VoxelMapId, ref datacellCoord) != null)
                {
                    MyVoxelCacheData.UpdateCell(voxelMap.VoxelMapId, ref datacellCoord);
                }   */
            }

            return ret;
        }

        private static void TryLoadCellInBackground(MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyLodTypeEnum cellHashType)
        {
            long key = MyVoxelMaps.GetCellHashCode(voxelMap.VoxelMapId, ref renderCellCoord, cellHashType);

            bool loadingCell = m_cellsBeingLoaded.Contains(key);
            if (loadingCell) return; // already loading the cell
            m_cellsBeingLoaded.Add(key);

            LoadCellInBackground(voxelMap, ref renderCellCoord, cellHashType);
        }

        private static void LoadCellInBackground(
            MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyLodTypeEnum cellHashType)
        {
            System.Diagnostics.Debug.Assert(false, "Not implemented");
            MyCommonDebugUtils.AssertDebug(voxelMap != null);
            //m_queue.Enqueue(new RenderCellLoadJob(voxelMap, ref renderCellCoord, cellHashType));
            //m_event.Set();
        }

        private static MyVoxelCacheCellRender LoadCell(
            MyVoxelMap voxelMap, ref MyMwcVector3Int renderCellCoord, MyLodTypeEnum cellHashType)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddCell");

            MyVoxelCacheCellRender ret = AddCell(voxelMap.VoxelMapId, ref renderCellCoord, cellHashType);
            ret.Begin(voxelMap, ref renderCellCoord);
            ret.CellHashType = cellHashType;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (cellHashType == MyLodTypeEnum.LOD0)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("cellHashType LOD0");
                m_dataCellsQueue.Clear();

                //  Create normal (LOD0) version
                for (int dataX = 0; dataX < MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE; dataX++)
                {
                    for (int dataY = 0; dataY < MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE; dataY++)
                    {
                        for (int dataZ = 0; dataZ < MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE; dataZ++)
                        {
                            //  Don't precalculate this cells now. Store it in queue and calculate all cells at once by MyVoxelPrecalc.PrecalcQueue()
                            MyMwcVector3Int dataCellCoord =
                                new MyMwcVector3Int(
                                    renderCellCoord.X * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + dataX,
                                    renderCellCoord.Y * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + dataY,
                                    renderCellCoord.Z * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + dataZ);
                            MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCellLater(voxelMap, ref dataCellCoord);
                            if (cachedDataCell != null)
                            {
                                m_dataCellsQueue.Add(cachedDataCell);
                            }
                        }
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("PrecalcQueue LOD0");

                //  Precalculate all queued data cells in parallel threads - using multiple cores if possible.
                MyVoxelPrecalc.PrecalcQueue();

                if (MyFakes.SIMPLIFY_VOXEL_MESH)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Stitch Data Cells LOD0");
                    MyDataCellStitcher.StitchDataCells(m_dataCellsQueue);

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Decimate mesh LOD0");
                    MyMeshSimplifier.Instance.SimplifyMesh(MyDataCellStitcher.Vertices, MyDataCellStitcher.Triangles);

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddTriangles LOD0");
                    ret.AddTriangles(MyDataCellStitcher.Vertices, MyDataCellStitcher.Triangles);
                }
                else
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddTriangles LOD0");
                    ret.AddTriangles(m_dataCellsQueue);
                }

                //  Iterate all data cells and copy their triangles to this render cell
                //for (int i = 0; i < m_dataCellsQueue.Count; i++)
                //{
                //    ret.AddTriangles(m_dataCellsQueue[i]);
                //}
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else if (cellHashType == MyLodTypeEnum.LOD1)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("cellHashType LOD1");

                m_helperLodCachedDataCell.Reset();

                //  Create LOD1 render cell
                MyVoxelPrecalc.PrecalcImmediatelly(
                    new MyVoxelPrecalcTaskItem(
                        MyLodTypeEnum.LOD1,
                        voxelMap,
                        m_helperLodCachedDataCell,
                        new MyMwcVector3Int(
                            renderCellCoord.X * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE,
                            renderCellCoord.Y * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE,
                            renderCellCoord.Z * MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE)));


                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_dataCellsQueue LOD1");
                m_dataCellsQueue.Clear();
                m_dataCellsQueue.Add(m_helperLodCachedDataCell);


                if (MyFakes.SIMPLIFY_VOXEL_MESH)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Stitch data cells LOD1");
                    MyDataCellStitcher.StitchDataCells(m_dataCellsQueue);

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Decimate mesh LOD1");
                    MyMeshSimplifier.Instance.SimplifyMesh(MyDataCellStitcher.Vertices, MyDataCellStitcher.Triangles);

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddTriangles LOD1");
                    ret.AddTriangles(MyDataCellStitcher.Vertices, MyDataCellStitcher.Triangles);
                }
                else
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("AddTriangles LOD1");
                    ret.AddTriangles(m_dataCellsQueue);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            ret.End();
            return ret;
        }

        //  Gets cell from the cache. If cell isn't in the cache, null is returned.
        //  This is only lookup into hashtable. No precalc is made here.
        static MyVoxelCacheCellRender GetCellFromCache(int voxelMapId, ref MyMwcVector3Int cellCoord, MyLodTypeEnum cellHashType)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, cellHashType);

            LinkedListNode<MyVoxelCacheCellRender> ret;
            if (m_cellsByCoordinate.TryGetValue(key, out ret) == true)
            {
                return ret.Value;
            }

            return null;
        }

        //  Add cell into cache and returns reference to it. Cache item with lowest priority is choosen.
        //  Call this method when you want allocate new item in the cache.
        static MyVoxelCacheCellRender AddCell(int voxelMapId, ref MyMwcVector3Int cellCoord, MyLodTypeEnum cellHashType)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, cellHashType);

            //  Cache item with lowest priority is choosen.
            LinkedListNode<MyVoxelCacheCellRender> first = m_priority.First;
            lock (m_priorityLocker)
            {
                m_priority.RemoveFirst();
                m_priority.AddLast(first);
            }

            //  If this object already contained some vertex buffers (and of course some render cell), we need to dispose its vertex buffers and 
            //  remove from hash table, so that render cell will no longer be in the render cell cache
            if (first.Value.Contains == true)
            {
                System.Diagnostics.Debug.Assert(false, "Cache is full - increase it atm");

                Int64 keyForRemoving = MyVoxelMaps.GetCellHashCode(first.Value.VoxelMap.VoxelMapId, ref first.Value.CellCoord, first.Value.CellHashType);
                m_cellsByCoordinate.Remove(keyForRemoving);
                first.Value.Reset();
            }

            //  Remember where is render cell cache for this render cell
            m_cellsByCoordinate.Add(key, first);

            //  You have reached the capacity of RENDER cells cache. Consider increasing it.
            MyCommonDebugUtils.AssertDebug(m_cellsByCoordinate.Count <= m_capacity);

            return first.Value;
        }

        //  Remove cell - after voxels were changed, etc.
        public static void RemoveCell(int voxelMapId, ref MyMwcVector3Int cellCoord, MyLodTypeEnum cellHashType)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, cellHashType);

            //  If cell is in cache, we remove it from dictionary and move it to the beginning of priority linked list
            LinkedListNode<MyVoxelCacheCellRender> ret;
            if (m_cellsByCoordinate.TryGetValue(key, out ret) == true)
            {
                m_cellsByCoordinate.Remove(key);

                //  Dispose vertex buffers
                ret.Value.Reset();

                //  Move it to the beginning of priority linked list
                lock (m_priorityLocker)
                {
                    m_priority.Remove(ret);
                    m_priority.AddFirst(ret);
                }
            }
        }

        //  Update cell - immediately after it was last time used. It will get higher priority and won't be flushed when AddCell() called next time.
        public static void UpdateCell(int voxelMapId, ref MyMwcVector3Int cellCoord, MyLodTypeEnum cellHashType)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, cellHashType);
            LinkedListNode<MyVoxelCacheCellRender> ret = m_cellsByCoordinate[key];

            //  Move it to the end of priority linked list
            lock (m_priorityLocker)
            {
                m_priority.Remove(ret);
                m_priority.AddLast(ret);
            }
        }

        //  Remove cell for voxels specified be min/max corner. Used after explosion when we want to remove a lot of voxels/cell from cache.
        //  This is efficient method, because it doesn't invalidate cache after every voxel change.
        //  Method knows that adjacent cells need to be removed too (because of MCA), so it invalidates them too.
        public static void RemoveCellForVoxels(MyVoxelMap voxelMap, MyMwcVector3Int minVoxel, MyMwcVector3Int maxVoxel)
        {
            //  Calculate voxel for boundary things...
            MyMwcVector3Int maxCornerExt = new MyMwcVector3Int(maxVoxel.X + 1, maxVoxel.Y + 1, maxVoxel.Z + 1);
            if (maxCornerExt.X > voxelMap.SizeMinusOne.X) maxCornerExt.X = voxelMap.SizeMinusOne.X;
            if (maxCornerExt.Y > voxelMap.SizeMinusOne.Y) maxCornerExt.Y = voxelMap.SizeMinusOne.Y;
            if (maxCornerExt.Z > voxelMap.SizeMinusOne.Z) maxCornerExt.Z = voxelMap.SizeMinusOne.Z;

            //  Min/max cell
            MyMwcVector3Int minCell = voxelMap.GetVoxelRenderCellCoordinate(ref minVoxel);
            MyMwcVector3Int maxCell = voxelMap.GetVoxelRenderCellCoordinate(ref maxCornerExt);

            //  Invalidate cells
            MyMwcVector3Int tempCellCoord;
            for (tempCellCoord.X = minCell.X; tempCellCoord.X <= maxCell.X; tempCellCoord.X++)
            {
                for (tempCellCoord.Y = minCell.Y; tempCellCoord.Y <= maxCell.Y; tempCellCoord.Y++)
                {
                    for (tempCellCoord.Z = minCell.Z; tempCellCoord.Z <= maxCell.Z; tempCellCoord.Z++)
                    {
                        RemoveCell(voxelMap.VoxelMapId, ref tempCellCoord, MyLodTypeEnum.LOD0);
                        RemoveCell(voxelMap.VoxelMapId, ref tempCellCoord, MyLodTypeEnum.LOD1);
                    }
                }
            }
        }

        //  Return count of cells currently in the cache
        public static int GetCachedCellsCount()
        {
            return m_cellsByCoordinate.Count;
        }

        //  Return capacity of this whole cache (max number of cacheable cells)
        public static int GetCapacity()
        {
            return m_capacity;
        }

        //  Number of vertexes in ALL vertex buffers in ALL render cells
        public static int GetVertexesCount()
        {
            int ret = 0;
            foreach (KeyValuePair<Int64, LinkedListNode<MyVoxelCacheCellRender>> kvp in m_cellsByCoordinate)
            {
                foreach (MyVoxelCacheCellRenderBatch batch in kvp.Value.Value.Batches)
                {
                    ret += batch.VertexBufferCount;
                }
            }

            return ret;
        }

        //  Number of indices in ALL index buffers in ALL render cells
        public static int GetIndicesCount()
        {
            int ret = 0;
            foreach (KeyValuePair<Int64, LinkedListNode<MyVoxelCacheCellRender>> kvp in m_cellsByCoordinate)
            {
                foreach (MyVoxelCacheCellRenderBatch batch in kvp.Value.Value.Batches)
                {
                    if (batch.IndexBufferCount > 0)
                    {
                        ret += batch.IndexBufferCount;
                    }
                }
            }

            return ret;
        }

        ////  Remove cell for voxel
        //public static void RemoveCellForVoxel(MyVoxelMap voxelMap, int voxelX, int voxelY, int voxelZ)
        //{
        //    MyMwcVector3Int cell = voxelMap.GetVoxelCellCoordinate(voxelX, voxelY, voxelZ);
        //    RemoveCell(voxelMap.GetVoxelMapId(), cell.X, cell.Y, cell.Z);
        //}
    }
}

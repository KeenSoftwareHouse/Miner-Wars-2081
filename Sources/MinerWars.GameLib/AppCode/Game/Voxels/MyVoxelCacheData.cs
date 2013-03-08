using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MinerWarsMath.Graphics.PackedVector;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
//using System.Threading;
using MinerWars.AppCode.Physics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using BulletXNA.BulletCollision;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Parallelization.Threading;

//  Caching voxel cell triangles.
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


namespace MinerWars.AppCode.Game.Voxels
{
    //  One cell stored in a cache
    class MyVoxelCacheCellData
    {
        public static readonly Int64 INVALID_KEY = Int64.MaxValue;

        public Int64 Key;
        public int VoxelTrianglesCount;
        public int VoxelVerticesCount;
        public MyVoxelTriangle[] VoxelTriangles;
        public MyVoxelVertex[] VoxelVertices;
        private MyOctree m_octree;

        private object m_syncRoot = new object();

        public MyVoxelCacheCellData()
        {
            Key = INVALID_KEY;
        }

        public MyOctree Octree
        {
            get
            {
                lock (m_syncRoot)
                {
                    if (m_octree == null && VoxelTrianglesCount > 0)
                    {
                        m_octree = new MyOctree();
                        m_octree.Init(ref VoxelVertices, ref VoxelVerticesCount, ref VoxelTriangles, ref VoxelTrianglesCount, out VoxelTriangles);
                    }
                    return m_octree;
                }
            }
        }

        public void PrepareCache(MyVoxelVertex[] vertices, int vertexCount, MyVoxelTriangle[] triangles, int triangleCount)
        {
            lock (m_syncRoot)
            {
                if (vertexCount == 0)
                {
                    VoxelVerticesCount = 0;
                    VoxelTrianglesCount = 0;
                    m_octree = null;
                    VoxelVertices = null;
                    return;
                }
                MyCommonDebugUtils.AssertDebug(vertexCount <= Int16.MaxValue);

                MyRender.GetRenderProfiler().StartProfilingBlock("build octree");
                if (m_octree == null)
                    m_octree = new MyOctree();
                m_octree.Init(ref vertices, ref vertexCount, ref triangles, ref triangleCount, out VoxelTriangles);
                MyRender.GetRenderProfiler().EndProfilingBlock();

                // copy voxel vertices
                VoxelVertices = new MyVoxelVertex[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                    VoxelVertices[i] = vertices[i];

                // set size only after the arrays are fully allocated
                VoxelVerticesCount = vertexCount;
                VoxelTrianglesCount = triangleCount;
            }
        }

        //  This methods needs to be called after every cell cache released!! It releases vertex buffer. It's important, 
        //  because when this cell cache will be associated to a new cell, not same material vertex buffer will be used so unused needs to be disposed!!
        public void Reset()
        {
            lock (m_syncRoot)
            {
                VoxelTrianglesCount = 0;
                VoxelTriangles = null;
                m_octree = null;

                VoxelVerticesCount = 0;
                VoxelVertices = null;
                Key = INVALID_KEY;
            }
        }

        #region Serialization for cache

        public void Write(BinaryWriter binaryWriter)
        {
            // triangles
            MyMwcMessageOut.WriteInt32(VoxelTrianglesCount, binaryWriter);
            for (int i = 0; i < VoxelTrianglesCount; i++)
            {
                MyDataCellReadWriteHelper.WriteVoxelTriangle(VoxelTriangles[i], binaryWriter);
            }

            // vertices
            MyMwcMessageOut.WriteInt32(VoxelVerticesCount, binaryWriter);
            for (int i = 0; i < VoxelVerticesCount; i++)
            {
                MyDataCellReadWriteHelper.WriteVoxelVertex(VoxelVertices[i], binaryWriter);
            }
        }

        public bool Read(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  triangles
            int? voxelTrianglesCount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (voxelTrianglesCount == null)
                return false;
            VoxelTrianglesCount = voxelTrianglesCount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("voxelTrianglesCount: " + VoxelTrianglesCount);
            VoxelTriangles = new MyVoxelTriangle[VoxelTrianglesCount];
            for (int i = 0; i < VoxelTrianglesCount; i++)
            {
                var voxelTriangle = MyDataCellReadWriteHelper.ReadVoxelTriangleEx(binaryReader, senderEndPoint);
                if (voxelTriangle == null)
                    return false;
                VoxelTriangles[i] = voxelTriangle.Value;
            }

            // vertices
            int? voxelVerticesCount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (voxelVerticesCount == null)
                return false;
            VoxelVerticesCount = voxelVerticesCount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("voxelVerticesCount: " + VoxelVerticesCount);
            VoxelVertices = new MyVoxelVertex[VoxelVerticesCount];
            for (int i = 0; i < VoxelVerticesCount; i++)
            {
                var voxelVertex = MyDataCellReadWriteHelper.ReadVoxelVertexEx(binaryReader, senderEndPoint);
                if (voxelVertex == null)
                    return false;
                VoxelVertices[i] = voxelVertex.Value;
            }

            return true;
        }

        #endregion
    }

    //  Container of all cells stored in voxel cache
    static class MyVoxelCacheData
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //  Access cached cell by key
        static Dictionary<Int64, LinkedListNode<MyVoxelCacheCellData>> m_cellsByCoordinate = null;

        //  Here we preallocate cell caches
        static MyVoxelCacheCellData[] m_cellsPreallocated = null;

        //  Linked list used to allocate, remove and update voxel cell in O(1) time.
        static LinkedList<MyVoxelCacheCellData> m_priority;
        static LinkedListNode<MyVoxelCacheCellData>[] m_priorityArray;

        public static FastResourceLock Locker = new FastResourceLock();

        //private static SpinLock m_spinLock;

        //  Capacity of this cell cache
        static int m_capacity;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static MyVoxelCacheData()
        {
            m_capacity = MyVoxelConstants.VOXEL_DATA_CELL_CACHE_SIZE;
            m_cellsByCoordinate = new Dictionary<Int64, LinkedListNode<MyVoxelCacheCellData>>(m_capacity);
            m_priority = new LinkedList<MyVoxelCacheCellData>();
            m_priorityArray = new LinkedListNode<MyVoxelCacheCellData>[m_capacity];
            m_cellsPreallocated = new MyVoxelCacheCellData[m_capacity];
            for (int i = 0; i < m_capacity; i++)
            {
                m_cellsPreallocated[i] = new MyVoxelCacheCellData();
                m_priorityArray[i] = new LinkedListNode<MyVoxelCacheCellData>(m_cellsPreallocated[i]);
                m_priority.AddLast(m_priorityArray[i]);
            }
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCacheData.LoadData");

            m_cellsByCoordinate.Clear();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_cellsByCoordinate.Clear();
            
            for (int i = 0; i < m_capacity; i++)
            {
                m_cellsPreallocated[i].Reset();
            }
        }

        //  Initialized new voxel cell cache. Capacity specified count of voxel cells in the cache.
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyVoxelCacheData.LoadContent() - START");
            MyMwcLog.IncreaseIndent();



            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheData.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyVoxelCacheData.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            for (int i = 0; i < m_cellsPreallocated.Length; i++)
            {
                if (m_cellsPreallocated[i] != null)
                {
                    //resets vertex buffers
                    m_cellsPreallocated[i].Reset();
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheData.UnloadContent - END");
        }

        //  Get cell from cache. If cell isn't in the cache, we try to precalc it. But first we check if cell can contain any triangleVertexes.
        //  If no, null is returned. Otherwise we run precalc on it and store it in the cache.
        //  Recap: 
        //      - we return null if cell doesn't contain triangles and because of this it isn't in the cache
        //      - otherwise we return correct cached cell (with triangles)
        public static MyVoxelCacheCellData GetCell(MyVoxelMap voxelMap, ref MyMwcVector3Int cellCoord, bool createTriangles = true)
        {
            MyVoxelCacheCellData cachedDataCell;

            lock (Locker)
            {
                cachedDataCell = GetCellFromCache(voxelMap.VoxelMapId, ref cellCoord);

                //  If cell isn't in the cache yet
                if (cachedDataCell == null && createTriangles)
                {
                    //  If cell and its neighborhood is completely full or completely empty, result of precalc will be zero-triangles, so we can skip precalc.
                    //  It can speedup precalc because then we don't have to check every voxel
                    if (voxelMap.IsDataCellCompletelyFullOrCompletelyEmpty(ref cellCoord) == false)
                    {

                        //  Cell may have triangles, so add it to cache and run precalc
                        cachedDataCell = AddCell(voxelMap.VoxelMapId, ref cellCoord);

                        MyVoxelPrecalc.PrecalcImmediatelly(
                            new MyVoxelPrecalcTaskItem(
                                MyLodTypeEnum.LOD0,
                                voxelMap,
                                cachedDataCell,
                                new MyMwcVector3Int(
                                    cellCoord.X * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS,
                                    cellCoord.Y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS,
                                    cellCoord.Z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS)));
                    }
                }

                //  I commented out this condition "(cachedDataCell != null)" because now I think we want to move-up in priority list
                //  all data cells, independently of whether they contain or don't contain triangles
                if (cachedDataCell != null)
                {
                    UpdateCell(voxelMap.VoxelMapId, ref cellCoord);
                }
            }

            return cachedDataCell;
        }

        //  This method does same thing as GetCell(), but doesn't do it now or immediately. 
        //  Instead it adds data cells that are needed to be precalculated to the queue so later they can be calculated on multiple cores.
        public static MyVoxelCacheCellData GetCellLater(MyVoxelMap voxelMap, ref MyMwcVector3Int cellCoord)
        {
            lock (Locker)
            {
                MyVoxelCacheCellData cachedDataCell = GetCellFromCache(voxelMap.VoxelMapId, ref cellCoord);

                //  If cell isn't in the cache yet
                if (cachedDataCell == null)
                {
                    //  If cell and its neighborhood is completely full or completely empty, result of precalc will be zero-triangles, so we can skip precalc.
                    //  It can speedup precalc because then we don't have to check every voxel
                    if (voxelMap.IsDataCellCompletelyFullOrCompletelyEmpty(ref cellCoord) == false)
                    {
                        //  Cell may have triangles, so add it to cache and run precalc
                        cachedDataCell = AddCell(voxelMap.VoxelMapId, ref cellCoord);

                        MyVoxelPrecalc.AddToQueue(MyLodTypeEnum.LOD0, voxelMap, cachedDataCell, cellCoord.X * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS, cellCoord.Y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS, cellCoord.Z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);
                    }
                }

                //  I commented out this condition "(cachedDataCell != null)" because now I think we want to move-up in priority list
                //  all data cells, independently of whether they contain or don't contain triangles
                if (cachedDataCell != null)
                {
                    UpdateCell(voxelMap.VoxelMapId, ref cellCoord);
                }

                return cachedDataCell;
            }
        }

        //  Gets cell from the cache. If cell isn't in the cache, null is returned.
        //  This is only lookup into hashtable. No precalc is made here.
        public static MyVoxelCacheCellData GetCellFromCache(int voxelMapId, ref MyMwcVector3Int cellCoord)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, MyLodTypeEnum.LOD0);

            LinkedListNode<MyVoxelCacheCellData> ret;
            if (m_cellsByCoordinate.TryGetValue(key, out ret) == true)
            {
                return ret.Value;
            }

            return null;
        }

        //  Add cell into cache and returns reference to it. Cache item with lowest priority is choosen.
        //  Call this method when you want allocate new item in the cache.
        public static MyVoxelCacheCellData AddCell(int voxelMapId, ref MyMwcVector3Int cellCoord)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, MyLodTypeEnum.LOD0);

            //  Cache item with lowest priority is choosen.
            LinkedListNode<MyVoxelCacheCellData> first = m_priority.First;
            m_priority.RemoveFirst();
            if (first.Value.Key != MyVoxelCacheCellData.INVALID_KEY)
                m_cellsByCoordinate.Remove(first.Value.Key);

            m_priority.AddLast(first);
            m_cellsByCoordinate.Add(key, first);
            first.Value.Key = key;

            //  You have exceeded the capacity of DATA cells cache. Thats serious bug!
            MyCommonDebugUtils.AssertDebug(m_cellsByCoordinate.Count <= m_capacity);

            return first.Value;
        }

        //  Remove cell - after voxels were changed, etc.
        public static void RemoveCell(int voxelMapId, ref MyMwcVector3Int cellCoord)
        {
            using (Locker.AcquireExclusiveUsing()) 
            {
                lock (Locker)
                {
                    Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, MyLodTypeEnum.LOD0);

                    //  If cell is in cache, we remove it from dictionary and move it to the beginning of priority linked list
                    LinkedListNode<MyVoxelCacheCellData> ret;
                    if (m_cellsByCoordinate.TryGetValue(key, out ret) == true)
                    {
                        m_cellsByCoordinate.Remove(key);

                        //  Dispose vertex buffers
                        ret.Value.Reset();

                        //  Move it to the beginning of priority linked list
                        m_priority.Remove(ret);
                        m_priority.AddFirst(ret);
                    }
                }
            }
        }

        //  Update cell - immediately after it was last time used. It will get higher priority and won't be flushed when AddCell() called next time.
        public static void UpdateCell(int voxelMapId, ref MyMwcVector3Int cellCoord)
        {
            //bool lockTaken = false;
            //try
            //{
            //    m_spinLock.Enter(ref lockTaken);

            lock (Locker)
            {
                Int64 key = MyVoxelMaps.GetCellHashCode(voxelMapId, ref cellCoord, MyLodTypeEnum.LOD0);
                LinkedListNode<MyVoxelCacheCellData> cell = m_cellsByCoordinate[key];

                //  Move it to the end of priority linked list
                m_priority.Remove(cell);
                m_priority.AddLast(cell);
            }
            //}
            //finally
            //{
            //    if (lockTaken) m_spinLock.Exit();
            //}
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
            MyMwcVector3Int minCell = voxelMap.GetDataCellCoordinate(ref minVoxel);
            MyMwcVector3Int maxCell = voxelMap.GetDataCellCoordinate(ref maxCornerExt);

            //  Invalidate cells
            MyMwcVector3Int tempCellCoord;
            for (tempCellCoord.X = minCell.X; tempCellCoord.X <= maxCell.X; tempCellCoord.X++)
            {
                for (tempCellCoord.Y = minCell.Y; tempCellCoord.Y <= maxCell.Y; tempCellCoord.Y++)
                {
                    for (tempCellCoord.Z = minCell.Z; tempCellCoord.Z <= maxCell.Z; tempCellCoord.Z++)
                    {
                        RemoveCell(voxelMap.VoxelMapId, ref tempCellCoord);
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

        ////  Remove cell for voxel
        //public static void RemoveCellForVoxel(MyVoxelMap voxelMap, int voxelX, int voxelY, int voxelZ)
        //{
        //    MyMwcVector3Int cell = voxelMap.GetVoxelCellCoordinate(voxelX, voxelY, voxelZ);
        //    RemoveCell(voxelMap.GetVoxelMapId(), cell.X, cell.Y, cell.Z);
        //}
    }

    static class MyDataCellReadWriteHelper
    {
        public static void WriteVoxelTriangle(MyVoxelTriangle voxelTriangle, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(voxelTriangle.VertexIndex0);
            binaryWriter.Write(voxelTriangle.VertexIndex1);
            binaryWriter.Write(voxelTriangle.VertexIndex2);
        }

        public static void WriteVoxelVertex(MyVoxelVertex voxelVertex, BinaryWriter binaryWriter)
        {
            MyMwcMessageOut.WriteVoxelMaterialsEnum(voxelVertex.Material, binaryWriter);
            MyMwcMessageOut.WriteByte4(voxelVertex.m_normal, binaryWriter);
            binaryWriter.Write(voxelVertex.m_positionAndAmbient.packed_value);
        }

        public static MyVoxelTriangle? ReadVoxelTriangleEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                MyVoxelTriangle result;
                result.VertexIndex0 = MyMwcMessageIn.ReadInt16(binaryReader);
                result.VertexIndex1 = MyMwcMessageIn.ReadInt16(binaryReader);
                result.VertexIndex2 = MyMwcMessageIn.ReadInt16(binaryReader);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static MyVoxelVertex? ReadVoxelVertexEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            MyMwcVoxelMaterialsEnum? voxelMaterial = MyMwcMessageIn.ReadVoxelMaterialsEnumEx(binaryReader, senderEndPoint);
            Byte4? normal = MyMwcMessageIn.ReadByte4Ex(binaryReader, senderEndPoint);
            UInt64? positionAndAmbient = MyMwcMessageIn.ReadUInt64Ex(binaryReader, senderEndPoint);

            if (voxelMaterial.HasValue && normal.HasValue && positionAndAmbient.HasValue)
            {
                MyVoxelVertex result = new MyVoxelVertex
                    {
                        Material = voxelMaterial.Value,
                        m_normal = normal.Value,
                        m_positionAndAmbient = { packed_value = positionAndAmbient.Value }
                    };
                return result;
            }

            return null;
        }
    }
}

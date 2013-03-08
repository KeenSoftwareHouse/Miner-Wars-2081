using System;
using MinerWars.CommonLIB.AppCode.Networking;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Voxels
{
    //  One vertexbuffer/indexbuffer stored in one cell cache. Each render cell can have more of this objects.
    //  Each one represents one draw call. Max size of VB/IB is 32678 indices (divide it by 3 and you get triangles count).
    class MyVoxelCacheCellRenderBatch : IComparable
    {
        public MyVoxelCacheCellRenderBatchType Type;
        public MyMwcVoxelMaterialsEnum Material0;
        public MyMwcVoxelMaterialsEnum? Material1;
        public MyMwcVoxelMaterialsEnum? Material2;

        //  For sorting batches by type and material
        public int SortOrder;

        // Unique id of (multi)material
        public int MaterialId;

        //  Index buffer (may be null if type is MULTI_MATERIAL_CLEAR)
        public int IndexBufferCount;
        public int IndexBufferSize;
        public IndexBuffer IndexBuffer;

        //  Vertex buffer
        public int VertexBufferCount;
        public int VertexBufferSize;
        public VertexBuffer VertexBuffer;

        public void UpdateSortOrder()
        {
            int maxMat = MyVoxelMaterials.GetMaterialsCount() + 1;
            int matCount = MyVoxelMaterials.GetMaterialsCount() + 2;

            int mats0 = (int)Material0;
            int mats1 = (int)(Material1.HasValue ? (int)Material1.Value : maxMat);
            int mats2 = (int)(Material2.HasValue ? (int)Material2.Value : maxMat);

            //  Important is type and material/texture. Order of type is defined by enum values
            SortOrder = ((int)Type * matCount * matCount * matCount) + mats2 * matCount * matCount + mats1 * matCount + mats0;
            MaterialId = mats2 * matCount * matCount + mats1 * matCount + mats0;
        }

        //  For sorting batches by type and material
        //  We want first multi-clear, then single-material and multi-material as last
        public int CompareTo(object compareToObject)
        {
            MyVoxelCacheCellRenderBatch compareToBatch = (MyVoxelCacheCellRenderBatch)compareToObject;
            return this.SortOrder.CompareTo(compareToBatch.SortOrder);
        }
    }
}

using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;

//  This STATIC class is buffer of preallocated voxel contents.
//  It is used only if we are switching cell from type FULL to EMPTY or MIXED.
//  We never release cell content, even if it becomes EMPTY (so basicaly it isn't needed more).

namespace MinerWars.AppCode.Game.Voxels
{
    static class MyVoxelContentCellContents
    {
        //static int m_capacity = 0;

        //  Preallocated cell contents. This is buffer from which we get new cell content if needed (when changing from FULL to MIXED or EMPTY)
        //static MyVoxelCellContent[] m_preallocatedContents = null;
        static MyObjectsPool<MyVoxelContentCellContent> m_preallocatedContents;

        //  Index of next content from buffer we will give when asked
        //static int m_nextForAllocation;


        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelContentCellContents.LoadData");
            if (m_preallocatedContents == null) // Never reallocate
            {
                m_preallocatedContents = new MyObjectsPool<MyVoxelContentCellContent>(MyVoxelConstants.PREALLOCATED_CELL_CONTENTS_COUNT);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_preallocatedContents.DeallocateAll();
            //m_preallocatedContents = null;
        }

        //  Get new preallocated content from the buffer.
        //  We don't check for size, because it must be done by higher game logic (reinicializing too destroyed level).
        public static LinkedListNode<MyVoxelContentCellContent> Allocate()
        {
            return m_preallocatedContents.AllocateEx();

            ////  Get content from preallocated buffer and increase m_notCompressedIndex for following allocation
            //MyVoxelCellContent ambient = m_preallocatedContents[m_nextForAllocation];
            //m_nextForAllocation++;
            //return ambient;
        }

        public static void Deallocate(LinkedListNode<MyVoxelContentCellContent> item)
        {
            m_preallocatedContents.MarkForDeallocate(item);
            m_preallocatedContents.DeallocateAllMarked();            
        }

        //  This method tells us if this buffer is almost consumed, so few more allocations and it's done.
        //  In that case, level needs to be reinicialized by server.
        public static bool IsAlmostFull()
        {
            //return m_nextForAllocation >= (m_capacity * 0.9);
            return m_preallocatedContents.GetActiveCount() > (m_preallocatedContents.GetCapacity() * 0.9);
        }

        public static int GetCount()
        {
            //return m_nextForAllocation;
            return m_preallocatedContents.GetActiveCount();
        }

        public static int GetCapacity()
        {
            //return m_capacity;
            return m_preallocatedContents.GetCapacity();
        }
    }
}

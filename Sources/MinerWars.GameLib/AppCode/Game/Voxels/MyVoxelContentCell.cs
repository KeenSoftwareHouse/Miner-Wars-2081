using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  This class holds data about voxel cell, but doesn't hold content of each voxel. That's in MyVoxelCellContent.

namespace MinerWars.AppCode.Game.Voxels
{
    //  This enum tells us if cell is 100% empty, 100% full or mixed (some voxels are full, some empty, some are something between)
    enum MyVoxelCellType : byte
    {
        EMPTY,
        FULL,
        MIXED
    }

    class MyVoxelContentCell
    {
        //  Cell type. Default is FULL.
        public MyVoxelCellType CellType { get; private set; }
        
        //  Reference to cell's content (array of voxel values). Only if cell type is MIXED.
        LinkedListNode<MyVoxelContentCellContent> m_cellContent = null;

        //  Sums all voxel values. Default is summ of all full voxel in cell, so by subtracting we can switch cell from MIXED to EMPTY.
        int m_voxelContentSum;


        public MyVoxelContentCell()
        {
            //  Default cell is FULL
            CellType = MyVoxelCellType.FULL;         

            //  Sums all voxel values. Default is summ of all full voxel in cell, so be subtracting we can switch cell from MIXED to EMPTY.
            m_voxelContentSum = MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL;
        }

        //  Voxel at specified coordinate 'x, y, z' sets to value 'content'. Coordinates are relative to voxel cell
        //  IMPORTANT: Do not call this method directly! Always call it through MyVoxelMap.SetVoxelContent()
        public void SetVoxelContent(byte content, ref MyMwcVector3Int voxelCoordInCell)
        {
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("QuantizedValue");
            content = MyVoxelContentCellContent.QuantizedValue(content);
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (MyFakes.MWCURIOSITY)
            {
                if (content > MyVoxelConstants.VOXEL_ISO_LEVEL)
                {
                    content = MyVoxelConstants.VOXEL_CONTENT_FULL;
                }
                else
                {
                    content = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                }
            }


            if (CellType == MyVoxelCellType.FULL)
            {
                if (content == MyVoxelConstants.VOXEL_CONTENT_FULL)
                {
                    //  Nothing is changing
                    return;
                }
                else
                {
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FULL:CheckCellType");
                    m_voxelContentSum -= (MyVoxelConstants.VOXEL_CONTENT_FULL - content);
                    CheckCellType();
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    //  If this cell is mixed, we change voxel's value in the cell content array, but first allocate the array
                    if (CellType == MyVoxelCellType.MIXED)
                    {
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FULL:MyVoxelCellType.MIXED");
                        
                        m_cellContent = MyVoxelContentCellContents.Allocate();
                        if (m_cellContent != null)
                        {
                            m_cellContent.Value.Reset(MyVoxelConstants.VOXEL_CONTENT_FULL);
                            m_cellContent.Value.SetVoxelContent(content, ref voxelCoordInCell);
                        }
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }
            else if (CellType == MyVoxelCellType.EMPTY)
            {
                if (content == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                {
                    //  Nothing is changing
                    return;
                }
                else
                {
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("EMPTY:CheckCellType");
                    m_voxelContentSum += content;
                    CheckCellType();
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    //  If this cell is mixed, we change voxel's value in the cell content array, but first allocate the array
                    if (CellType == MyVoxelCellType.MIXED)
                    {
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("EMPTY:MyVoxelCellType.MIXED");
                        m_cellContent = MyVoxelContentCellContents.Allocate();
                        if (m_cellContent != null)
                        {
                            m_cellContent.Value.Reset(MyVoxelConstants.VOXEL_CONTENT_EMPTY);
                            m_cellContent.Value.SetVoxelContent(content, ref voxelCoordInCell);
                        }
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }
            else if (CellType == MyVoxelCellType.MIXED)
            {
                if (m_cellContent == null)
                {
                    return;
                }
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCellType.MIXED");
                //  Check for previous content value not only for optimisation, but because we need to know how much it changed
                //  for calculating whole cell content summary.
                byte previousContent = m_cellContent.Value.GetVoxelContent(ref voxelCoordInCell);
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                if (previousContent == content)
                {
                    //  New value is same as current, so nothing needs to be changed
                    return;
                }

                m_voxelContentSum -= previousContent - content;
                CheckCellType();

                //  If this cell is still mixed, we change voxel's value in the cell content array
                if (CellType == MyVoxelCellType.MIXED)
                {
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MIXED:MyVoxelCellType.MIXED");
                    m_cellContent.Value.SetVoxelContent(content, ref voxelCoordInCell);
                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        // Set voxel content for the whole cell.
        public void SetAllVoxelContents(byte[] buffer)
        {
            // quantize the buffer and compute sum
            m_voxelContentSum = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = MyVoxelContentCellContent.QuantizedValue(buffer[i]);
                m_voxelContentSum += buffer[i];
            }

            // mixed-->empty/full: deallocate
            // empty/full-->mixed: allocate
            // mixed: fill with values from buffer
            if (m_voxelContentSum == 0)
            {
                if (CellType == MyVoxelCellType.MIXED) Deallocate();
                CellType = MyVoxelCellType.EMPTY;
            }
            else if (m_voxelContentSum == MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL)
            {
                if (CellType == MyVoxelCellType.MIXED) Deallocate();
                CellType = MyVoxelCellType.FULL;
            }
            else
            {
                if (CellType == MyVoxelCellType.FULL || CellType == MyVoxelCellType.EMPTY) m_cellContent = MyVoxelContentCellContents.Allocate();
                if (m_cellContent != null)
                {
                    m_cellContent.Value.SetAddVoxelContents(buffer);
                }
                CellType = MyVoxelCellType.MIXED;
            }
        }

        
        //  Coordinates are relative to voxel cell
        //  IMPORTANT: Input variable 'voxelCoordInCell' is 'ref' only for optimization. Never change its value in the method!!!
        public byte GetVoxelContent(ref MyMwcVector3Int voxelCoordInCell)
        {
            if (CellType == MyVoxelCellType.EMPTY)
            {
                //  Cell is empty, therefore voxel must be empty too.
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;
            }
            else if (CellType == MyVoxelCellType.FULL)
            {
                //  Cell is full, therefore voxel must be full too.
                return MyVoxelConstants.VOXEL_CONTENT_FULL;
            }
            else
            {
                //  If cell is mixed, get voxel's content from the cell's content.
                //  Content was allocated before, we don't need to do it now (or even check it).
                if (m_cellContent != null)
                {
                    return m_cellContent.Value.GetVoxelContent(ref voxelCoordInCell);
                }

                return 0;
            }
        }

        //  This method helps us to maintain correct cell type even after removing or adding voxels from cell
        //  If all voxels were removed from this cell, we change its type to from MIXED to EMPTY.
        //  If voxels were added, we change its type to from EMPTY to MIXED.
        //  If voxels were added to full, we change its type to FULL.
        void CheckCellType()
        {
            //  Voxel cell content sum isn't in allowed range. Probably increased or descreased too much.
            System.Diagnostics.Debug.Assert((m_voxelContentSum >= 0) && (m_voxelContentSum <= MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL));

            if (m_voxelContentSum == 0)
            {
                CellType = MyVoxelCellType.EMPTY;
            }
            else if (m_voxelContentSum == MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL)
            {
                CellType = MyVoxelCellType.FULL;
            }
            else
            {
                CellType = MyVoxelCellType.MIXED;
            }

            //  If cell changed from MIXED to EMPTY or FULL, we will release it's cell content because it's not needed any more
            if ((CellType == MyVoxelCellType.EMPTY) || (CellType == MyVoxelCellType.FULL))
            {
                Deallocate();
            }
        }

        public byte GetAverageContent()
        {
            return (byte)(m_voxelContentSum / MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_TOTAL);
        }

        public int GetVoxelContentSum()
        {
            return m_voxelContentSum;
        }

        public void SetToEmpty()
        {
            CellType = MyVoxelCellType.EMPTY;
            m_voxelContentSum = 0;

            CheckCellType();
        }

        public void SetToFull()
        {
            CellType = MyVoxelCellType.FULL;
            m_voxelContentSum = MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL;

            CheckCellType();
        }
        
        public void Deallocate()
        {
            if (m_cellContent != null)
            {
                //m_cellContent.Value.NeedReset = true;
                MyVoxelContentCellContents.Deallocate(m_cellContent);
                m_cellContent = null;
            }
        }
    }
}

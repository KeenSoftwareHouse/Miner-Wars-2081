using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  This class represents materials for cell of voxels (e.g. 8x8x8). Mapping is always one-to-one.
//  But detail 3D array is allocated and used only if all materials in this cell aren't same - so we save a lot of memory in areas where are same materials.
//  This cell is also used as a source for 'average data cell material'
//  Size of this cell is same as voxel data cell size.
//  This class doesn't support deallocating/disposing of 3D array if it's not anymore needed. Reason is that situation should happen or if, then it's very rare.
//  IndestructibleContents - it's content/scalar value of a voxel that tell us its minimum possible value, we can't set smaller content value. Used for indestructible materials only.

namespace MinerWars.AppCode.Game.Voxels
{
    class MyVoxelMaterialCell
    {
        static readonly int voxelsInCell = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        static readonly int xStep = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        static readonly int yStep = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        static readonly int zStep = 1;

        //  If whole cell contains only one material, it will be written in this member and 3D arrays won't be used.
        bool m_singleMaterialForWholeCell;
        MyMwcVoxelMaterialsEnum m_singleMaterial;
        byte m_singleIndestructibleContent;

        //  Used only if individual materials aren't same - are mixed.
        MyMwcVoxelMaterialsEnum[] m_materials;
        byte[] m_indestructibleContent;

        MyMwcVoxelMaterialsEnum? m_averageCellMaterial;

        static int[] m_cellMaterialCounts = new int[MyVoxelMaterials.GetMaterialsCount()];


        public MyVoxelMaterialCell(MyMwcVoxelMaterialsEnum defaultMaterial, byte defaultIndestructibleContents)
        {
            //  By default cell contains only one single material
            Reset(defaultMaterial, defaultIndestructibleContents);
        }

        //  Use when you want to change whole cell to one single material
        public void Reset(MyMwcVoxelMaterialsEnum defaultMaterial, byte defaultIndestructibleContents)
        {
            m_singleMaterialForWholeCell = true;
            m_singleMaterial = defaultMaterial;
            m_singleIndestructibleContent = defaultIndestructibleContents;
            m_averageCellMaterial = m_singleMaterial;
            m_materials = null;
            m_indestructibleContent = null;
        }

        //  Change material for specified voxel
        //  If this material is single material for whole cell, we do nothing. Otherwise we allocate 3D arrays and start using them.
        public void SetMaterialAndIndestructibleContent(MyMwcVoxelMaterialsEnum material, byte indestructibleContent, ref MyMwcVector3Int voxelCoordInCell)
        {
            CheckInitArrays(material);

            if (m_singleMaterialForWholeCell == false)
            {
                int xyz = voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep;
                m_materials[xyz] = material;
                m_indestructibleContent[xyz] = indestructibleContent;
            }
        }

        //  Return material for specified voxel. If whole cell contain one single material, this one is returned. Otherwise material from 3D array is returned.
        public MyMwcVoxelMaterialsEnum GetMaterial(ref MyMwcVector3Int voxelCoordInCell)
        {
            if (MyFakes.SINGLE_VOXEL_MATERIAL != null)
                return MyFakes.SINGLE_VOXEL_MATERIAL.Value;

            if (m_singleMaterialForWholeCell == true)
            {
                return m_singleMaterial;
            }
            else
            {
                return m_materials[voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep];
            }
        }

        //  Same as GetMaterial() - but this is for indestructible content
        public byte GetIndestructibleContent(ref MyMwcVector3Int voxelCoordInCell)
        {
            if (m_singleMaterialForWholeCell == true)
            {
                return m_singleIndestructibleContent;
            }
            else
            {
                return m_indestructibleContent[voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep];
            }
        }

        //  Calculate and then remember average material in this cell. It isn't single material, but average.
        public void CalcAverageCellMaterial()
        {
            if (m_singleMaterialForWholeCell == true)
            {
                //  For single material it's easy
                m_averageCellMaterial = m_singleMaterial;
            }
            else
            {
                //  If materials are stored in 3D array, we need to really calculate average material
                //  Iterate materials in this data cell
                for (int xyz = 0; xyz < voxelsInCell; xyz++)
                {
                    MyMwcVoxelMaterialsEnum material = m_materials[xyz];
                    m_cellMaterialCounts[(int)material]++;
                }

                int maxNum = 0;
                for (int i = 0; i < m_cellMaterialCounts.Length; i++)
                {
                    if (m_cellMaterialCounts[i] > maxNum)
                    {
                        maxNum = m_cellMaterialCounts[i];
                        m_averageCellMaterial = (MyMwcVoxelMaterialsEnum)i;
                    }
                    m_cellMaterialCounts[i] = 0; // Erase for next operation
                }
            }

            MyCommonDebugUtils.AssertRelease(m_averageCellMaterial.HasValue);
        }

        public MyMwcVoxelMaterialsEnum GetAverageCellMaterial()
        {
            if (MyFakes.SINGLE_VOXEL_MATERIAL != null)
                return MyFakes.SINGLE_VOXEL_MATERIAL.Value;

            if (m_averageCellMaterial == null)
                CalcAverageCellMaterial();

            return m_averageCellMaterial.Value;
        }

        public bool IsSingleMaterialForWholeCell()
        {
            return m_singleMaterialForWholeCell;
        }

        //  Check if we new material differs from one main material and if yes, we need to start using 3D arrays
        void CheckInitArrays(MyMwcVoxelMaterialsEnum material)
        {
            if ((m_singleMaterialForWholeCell == true) && (m_singleMaterial != material))
            {
                m_materials = new MyMwcVoxelMaterialsEnum[voxelsInCell];
                m_indestructibleContent = new byte[voxelsInCell];
                //  Fill with present cell values
                for (int xyz = 0; xyz < voxelsInCell; xyz++)
                {
                    m_materials[xyz] = m_singleMaterial;
                    m_indestructibleContent[xyz] = m_singleIndestructibleContent;
                }

                //  From now, this cell contains more than one material
                m_singleMaterialForWholeCell = false;
            }
        }

        public void OptimizeSize()
        {
            if (!m_singleMaterialForWholeCell && TestIsSingleMaterial())
            {
                m_singleMaterialForWholeCell = true;
                m_singleMaterial = m_materials[0];
                m_singleIndestructibleContent = m_indestructibleContent[0];
            }

            if (m_singleMaterialForWholeCell)
            {
                m_materials = null;
                m_indestructibleContent = null;
            }
        }

        bool TestIsSingleMaterial()
        {
            var first = m_materials[0];
            foreach (var v in m_materials)
            {
                if (v != first)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

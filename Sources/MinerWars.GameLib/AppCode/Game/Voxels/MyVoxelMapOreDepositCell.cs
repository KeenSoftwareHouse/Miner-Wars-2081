using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Radar;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.Voxels
{     
    delegate void OnVoxelMapOreDepositCellContainsOreChanged(MyVoxelMapOreDepositCell sender, bool isEmpty);

    class MyVoxelMapOreDepositCell : IMyObjectToDetect
    {
        private MyVoxelMap m_voxelMap;
        private MyMwcVector3Int m_coord;

        private Dictionary<int, int> m_allMaterialsContent;
        private int m_totalSumOfOreContent;
        private List<MyMwcVoxelMaterialsEnum> m_oreWithContent;
        private Dictionary<int, Vector3?> m_allMaterialsPositions;
        private bool m_positionIsDirty;

        private Dictionary<int, byte> m_helpersMaxContentForMaterial;

        private BoundingBox m_worldAABB;
        public BoundingBox WorldAABB 
        {
            get { return m_worldAABB; }
            set { m_positionIsDirty = true; m_worldAABB = value; }
        }
        public event Action<MyVoxelMapOreDepositCell> OnClose;

        public MyMwcVector3Int Coord 
        {
            get { return m_coord; }
        }

        public MyVoxelMap VoxelMap
        {
            get { return m_voxelMap; }
        }

        public MyVoxelMapOreDepositCell(MyVoxelMap voxelMap, MyMwcVector3Int coord)
        {
            m_voxelMap = voxelMap;
            m_coord = coord;
            m_positionIsDirty = true;
            m_totalSumOfOreContent = 0;
            
            m_oreWithContent = new List<MyMwcVoxelMaterialsEnum>(MyVoxelMapOreMaterials.RareOreCount());

            /*
            int allMaterialsCount = MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1;
            m_allMaterialsContent = new int[allMaterialsCount];
            m_allMaterialsPositions = new Vector3?[allMaterialsCount];
            m_helpersMaxContentForMaterial = new byte[allMaterialsCount];
              */

            m_allMaterialsContent = new Dictionary<int, int>();
            m_allMaterialsPositions = new Dictionary<int, Vector3?>();
            m_helpersMaxContentForMaterial = new Dictionary<int, byte>();
        }

        public event OnVoxelMapOreDepositCellContainsOreChanged OnVoxelMapOreDepositCellContainsOreChanged;

        public void SetOreContent(MyMwcVoxelMaterialsEnum ore, int content)
        {
            if (content == 0 || !MyVoxelMapOreMaterials.IsRareOre(ore))
            {
                return;
            }

            int previousTotalOreContentSum = m_totalSumOfOreContent;


            int existingContent = 0;
            m_allMaterialsContent.TryGetValue((int)ore, out existingContent);

            int contentToAdd = content;
            if (content < 0) 
            {
                contentToAdd = Math.Max(content, -existingContent);
            }

            if (!m_allMaterialsContent.ContainsKey((int)ore))
                m_allMaterialsContent.Add((int)ore, 0);
            m_allMaterialsContent[(int)ore] += contentToAdd;
            m_totalSumOfOreContent += contentToAdd;

            // this ore hasn't any content before, so we add it to oreWithContent collection
            if (contentToAdd > 0 && existingContent == 0)
            {
                m_oreWithContent.Add(ore);
            }
            // this ore has no content now, so we remove it from oreWithContent collection
            else if (contentToAdd < 0 && m_allMaterialsContent[(int)ore] == 0)
            {
                m_oreWithContent.Remove(ore);
            }
            
            bool containsOreChanged = false;
            // detect if cell was changed from empty to not empty
            if (m_totalSumOfOreContent > 0 && previousTotalOreContentSum == 0)
            {                
                containsOreChanged = true;
            }
            // detect if cell was changed from not empty to empty
            else if (previousTotalOreContentSum > 0 && m_totalSumOfOreContent <= 0)
            {                
                containsOreChanged = true;
            }
            
            if (containsOreChanged && OnVoxelMapOreDepositCellContainsOreChanged != null)
            {
                OnVoxelMapOreDepositCellContainsOreChanged(this, m_totalSumOfOreContent == 0);
            }
            m_positionIsDirty = true;
        }

        public void Close(bool needLock) 
        {
            if (needLock)
            {
                MyEntities.EntityCloseLock.AcquireExclusive();
            }

            MyVoxelMaps.RemoveVoxelMapOreDepositCell(this);

            m_voxelMap = null;
            if (m_oreWithContent != null)
            {
                m_oreWithContent.Clear();
                m_oreWithContent = null;
            }
            m_allMaterialsContent = null;
            m_allMaterialsPositions = null;
            m_helpersMaxContentForMaterial = null;
            m_totalSumOfOreContent = 0;
            if (OnClose != null)
            {
                OnClose(this);
                OnClose = null;
            }

            if (needLock)
            {
                MyEntities.EntityCloseLock.ReleaseExclusive();
            }
        }

        public void Clear() 
        {
            m_allMaterialsContent.Clear();
            m_oreWithContent.Clear();
            m_totalSumOfOreContent = 0;
            m_positionIsDirty = true;
        }

        public int GetOreContent(MyMwcVoxelMaterialsEnum ore)
        {
            return m_allMaterialsContent[(int)ore];
        }

        public List<MyMwcVoxelMaterialsEnum> GetOreWithContent()
        {
            return m_oreWithContent;
        }

        public int GetTotalRareOreContent() 
        {
            return m_totalSumOfOreContent;
        }

        public void SortByContent() 
        {
            m_oreWithContent.Sort(OreComparer);            
        }

        public Vector3 GetPosition()
        {
            return WorldAABB.GetCenter();
        }

        public Vector3? GetPosition(MyMwcVoxelMaterialsEnum material) 
        {
            if (m_positionIsDirty)
            {
                RecalculatePositions();
                m_positionIsDirty = false;
            }

            Vector3? pos;
            m_allMaterialsPositions.TryGetValue((int)material, out pos);
            return pos;
        }

        private void ClearPositionsAndMaxContent() 
        {
            m_allMaterialsPositions.Clear();
            m_helpersMaxContentForMaterial.Clear();
        }

        private void RecalculatePositions()
        {
            ClearPositionsAndMaxContent();
            int sizeInVoxels = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS;

            MyMwcVector3Int voxelStartCoord = new MyMwcVector3Int(sizeInVoxels * m_coord.X, sizeInVoxels * m_coord.Y, sizeInVoxels * m_coord.Z);
            MyMwcVector3Int voxelEndCoord = new MyMwcVector3Int(voxelStartCoord.X + sizeInVoxels - 1, voxelStartCoord.Y + sizeInVoxels - 1, voxelStartCoord.Z + sizeInVoxels - 1);

            MyMwcVector3Int voxelCoord;
            for (voxelCoord.X = voxelStartCoord.X; voxelCoord.X <= voxelEndCoord.X; voxelCoord.X++)
            {
                for (voxelCoord.Y = voxelStartCoord.Y; voxelCoord.Y <= voxelEndCoord.Y; voxelCoord.Y++)
                {
                    for (voxelCoord.Z = voxelStartCoord.Z; voxelCoord.Z <= voxelEndCoord.Z; voxelCoord.Z++)
                    {
                        byte content = m_voxelMap.GetVoxelContent(ref voxelCoord);
                        if (content >= MyVoxelConstants.VOXEL_ISO_LEVEL)
                        {
                            MyMwcVoxelMaterialsEnum material = m_voxelMap.GetVoxelMaterial(ref voxelCoord);
                            byte maxContent;
                            m_helpersMaxContentForMaterial.TryGetValue((int)material, out maxContent);

                            if (!m_allMaterialsPositions.ContainsKey((int)material) || content > maxContent)
                            {
                                if (!m_allMaterialsPositions.ContainsKey((int)material))
                                    m_allMaterialsPositions.Add((int)material,m_voxelMap.GetVoxelCenterPositionAbsolute(ref voxelCoord));
                                else
                                    m_allMaterialsPositions[(int)material] = m_voxelMap.GetVoxelCenterPositionAbsolute(ref voxelCoord);

                                if (!m_helpersMaxContentForMaterial.ContainsKey((int)material))
                                    m_helpersMaxContentForMaterial.Add((int)material, content);
                                else
                                    m_helpersMaxContentForMaterial[(int)material] = content;
                            }
                        }
                    }
                }
            }

            //if (m_oreWithContent.Count > 0)
            //{
            //    SortByContent();
            //    m_position = m_allMaterialsPositions[(int)m_oreWithContent[0]].Value;
            //}
            //else 
            //{
            //    m_position = WorldAABB.GetCenter();
            //}
        }

        private int OreComparer(MyMwcVoxelMaterialsEnum ore1, MyMwcVoxelMaterialsEnum ore2) 
        {
            int content1 = m_allMaterialsContent[(int)ore1];
            int content2 = m_allMaterialsContent[(int)ore2];

            if (content1 == content2)
            {
                return 0;
            }
            else if (content1 < content2)
            {
                return 1;
            }
            else 
            {
                return -1;
            }
        }
    }
}

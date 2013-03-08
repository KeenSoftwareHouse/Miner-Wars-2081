using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System;

//  This class holds array of voxels in a cell. It is allocated only when voxel changes status from empty or full to mixed.
//  This class is just array, nothing else.

namespace MinerWars.AppCode.Game.Voxels
{
    class MyVoxelContentCellContent
    {
        const int xStep = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        const int yStep = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        const int zStep = 1;
        const int TOTAL_VOXEL_COUNT = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;

        const int QUANTIZATION_BITS = 3;                   // number of bits kept
        
        const int THROWAWAY_BITS = 8 - QUANTIZATION_BITS;  // number of bits thrown away

        // Values quantized to (8 - QUANTIZATION_BITS) with correct smearing of significant bits.
        // Example: 3 significant bits
        //   000xxxxx -> 0000000   001xxxxx -> 0010010   010xxxxx -> 0100100   011xxxxx -> 0110110
        //   100xxxxx -> 1001001   101xxxxx -> 1011011   110xxxxx -> 1101101   111xxxxx -> 1111111
        // It's important to return 255 for max value and 0 for min value.
        static byte[] m_smearBits;

        static MyVoxelContentCellContent()
        {
            m_smearBits = new byte[1 << QUANTIZATION_BITS];
            for (uint i = 0; i < 1 << QUANTIZATION_BITS; i++)
            {
                uint value = i << THROWAWAY_BITS;

                // smear bits
                value = value + (value >> QUANTIZATION_BITS);
                if (QUANTIZATION_BITS < 4)
                {
                    value = value + (value >> QUANTIZATION_BITS * 2);
                    if (QUANTIZATION_BITS < 2)
                        value = value + (value >> QUANTIZATION_BITS * 4);
                }

                m_smearBits[i] = (byte)value;
            }
        }

        public static byte QuantizedValue(byte content)
        {
            unchecked
            {
                return m_smearBits[content >> THROWAWAY_BITS];
            }
        }

        
        byte[] m_packed;

        public MyVoxelContentCellContent()
        {
            // round number of bytes up, add 1 for quantizations with bits split into different bytes
            m_packed = new byte[(TOTAL_VOXEL_COUNT * QUANTIZATION_BITS + 7) / 8 + 1];
            Reset(MyVoxelConstants.VOXEL_CONTENT_FULL);
        }

        //  Reset all voxels in this content to specified value. Original version was reseting to full only, but now we need reseting to empty too.
        //  Old: By default all voxels are full
        //      This method must be called in constructor and then everytime we allocate this content after it was deallocated before.
        //      So, when this content is used first time, it's freshly reseted by constructor. If later we deallocate it and then
        //      more later allocate again, we have to reset it so it contains only full voxels again.
        public void Reset(byte resetToContent)
        {
            if (resetToContent == MyVoxelConstants.VOXEL_CONTENT_FULL)
                for (int i = 0; i < m_packed.Length; i++)
                    m_packed[i] = 255;
            else if (resetToContent == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                Array.Clear(m_packed, 0, m_packed.Length);
            else
            {
                MyMwcVector3Int position;
                for (position.X = 0; position.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; position.X++)
                    for (position.Y = 0; position.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; position.Y++)
                        for (position.Z = 0; position.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; position.Z++)
                            SetVoxelContent(resetToContent, ref position);
            }
        }

        static uint[] m_bitmask = {
            ~((255u >> THROWAWAY_BITS) << 0), ~((255u >> THROWAWAY_BITS) << 1), ~((255u >> THROWAWAY_BITS) << 2), ~((255u >> THROWAWAY_BITS) << 3),
            ~((255u >> THROWAWAY_BITS) << 4), ~((255u >> THROWAWAY_BITS) << 5), ~((255u >> THROWAWAY_BITS) << 6), ~((255u >> THROWAWAY_BITS) << 7),
        };


        public void SetAddVoxelContents(byte[] contents)
        {
            unchecked
            {
                // for QUANTIZATION_BITS == 8 we can just do System.Buffer.BlockCopy

                Array.Clear(m_packed, 0, m_packed.Length); 

                for (int bitadr = 0, adr = 0; bitadr < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_TOTAL * QUANTIZATION_BITS; bitadr += QUANTIZATION_BITS, adr++)
                {
                    int byteadr = bitadr >> 3;
                    uint c = ((uint)contents[adr] >> THROWAWAY_BITS) << (bitadr & 7);
                    m_packed[byteadr] |= (byte)c;
                    m_packed[byteadr + 1] |= (byte)(c >> 8);  // this needs to be done only for QUANTIZATION_BITS == 1,2,4,8
                }
            }
        }


        //  Voxel at specified coordinate 'x, y, z' sets to value 'content'
        //  Coordinates are relative to voxel cell
        public void SetVoxelContent(byte content, ref MyMwcVector3Int voxelCoordInCell)
        {
            //if (!CheckVoxelCoord(ref voxelCoordInCell)) return;

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVxlContent::SetVoxelContent");

            unchecked
            {
                // for QUANTIZATION_BITS == 8: m_packed[voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep] = content;
                int bitadr = (voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep) * QUANTIZATION_BITS;
                int bit = bitadr & 7;
                int byteadr = bitadr >> 3;
                uint c = ((uint)content >> THROWAWAY_BITS) << bit;
                m_packed[byteadr] = (byte)(m_packed[byteadr] & m_bitmask[bit] | c);
                m_packed[byteadr + 1] = (byte)(m_packed[byteadr + 1] & m_bitmask[bit] >> 8 | c >> 8);   // this needs to be done only for QUANTIZATION_BITS == 1,2,4,8
            }

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Coordinates are relative to voxel cell
        //  IMPORTANT: Input variable 'voxelCoordInCell' is 'ref' only for optimization. Never change its value in the method!!!
        public byte GetVoxelContent(ref MyMwcVector3Int voxelCoordInCell)
        {
            if (!CheckVoxelCoord(ref voxelCoordInCell)) return 0;

            unchecked
            {
                // for QUANTIZATION_BITS == 8: return m_packed[voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep];
                int bitadr = (voxelCoordInCell.X * xStep + voxelCoordInCell.Y * yStep + voxelCoordInCell.Z * zStep) * QUANTIZATION_BITS;
                int byteadr = bitadr >> 3;
                uint value = m_packed[byteadr] + ((uint)m_packed[byteadr + 1] << 8);  // QUANTIZATION_BITS == 1,2,4,8: value = (uint)m_packed[bitadr >> 3];
                return m_smearBits[(value >> (bitadr & 7)) & (255 >> THROWAWAY_BITS)];
            }
        }

        private bool CheckVoxelCoord(ref MyMwcVector3Int cellCoord)
        {
            return (uint)(cellCoord.X | cellCoord.Y | cellCoord.Z) < (uint)MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;  // VOXEL_DATA_CELL_SIZE_IN_VOXELS must be a power of 2
            /*
            return (cellCoord.X >= 0 && cellCoord.Y >= 0 && cellCoord.Z >= 0) &&
                (cellCoord.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS && cellCoord.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS && cellCoord.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);
            */
        }
    }
}

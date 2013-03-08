using System;
using System.IO;

namespace MinerWars.AppCode.Game.Utils
{
    public class MyCompressionFileLoad
    {
        byte[] m_decompressed;
        int m_readIndex;
        string m_fileName;


        public MyCompressionFileLoad(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    //  Read whole file to byte array and then decompress
                    m_decompressed = MyCompression.Decompress(br.ReadBytes((int)fs.Length));

                    //  Reset reading index
                    m_readIndex = 0;
                }
                fs.Close();
            }
        }

        public MyCompressionFileLoad(byte[] compressedVoxelContents)
        {
            using (MemoryStream fs = new MemoryStream(compressedVoxelContents))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    //  Read whole file to byte array and then decompress
                    m_decompressed = MyCompression.Decompress(br.ReadBytes((int)fs.Length));

                    //  Reset reading index
                    m_readIndex = 0;
                }
                fs.Close();
            }
        }

        //  Reads value (int, float, ...) from decompressed buffer
        public int GetInt32()
        {
            int ret = BitConverter.ToInt32(m_decompressed, m_readIndex);
            m_readIndex += sizeof(Int32);
            return ret;
        }

        //  Reads value (int, float, ...) from decompressed buffer
        public byte GetByte()
        {
            byte ret = m_decompressed[m_readIndex];
            m_readIndex += sizeof(byte);
            return ret;
        }

        //  Copy raw bytes
        public void GetBytes(int bytes, byte[] output)
        {
            System.Buffer.BlockCopy(m_decompressed, m_readIndex, output, 0, bytes);
            m_readIndex += bytes;
        }

        //  Signalizes if we haven't reached the end of un-compressed file by series of Get***() calls.
        public bool EndOfFile()
        {
            return m_readIndex >= m_decompressed.Length;
        }

        //  Finally save the file as compressed value
        public void Save(string fileName)
        {
            using (FileStream fs = File.Create(fileName))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(m_decompressed);
                }
            } 
        }
    }
}

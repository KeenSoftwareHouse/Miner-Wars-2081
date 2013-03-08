using System;
using System.Collections.Generic;
using System.IO;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyCompressionConstants
    {
        //  This array is used only during loading, so GC will free it after not needed. It's not used during the game-play phase.
        public const int HELPER_BYTE_ARRAY_SIZE_FOR_COMPRESSION = 128 * 1024 * 1024; //  100 MB
    }

    public class MyCompressionFileSave
    {
        Stream m_notCompressed;
        string m_tempFileName;

        public MyCompressionFileSave()
        {
            //m_notCompressed = new MemoryStream();
            m_tempFileName = System.IO.Path.GetTempFileName();
            m_notCompressed = new System.IO.FileStream(m_tempFileName, FileMode.Create);
        }

        //  Add value to byte array (float, int, string, etc)
        public void Add(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                m_notCompressed.WriteByte(value[i]);
            }
        }

        //  Add value to byte array (float, int, string, etc)
        public void Add(float value)
        {
            Add(BitConverter.GetBytes(value));
        }

        //  Add value to byte array (float, int, string, etc)
        public void Add(int value)
        {
            Add(BitConverter.GetBytes(value));
        }

        //  Add value to byte array (float, int, string, etc)
        public void Add(byte value)
        {
            m_notCompressed.WriteByte(value);
        }

        //  Finally save the file as compressed value
        public void Save(string fileName)
        {
            if (m_notCompressed != null)
            {
                m_notCompressed.Flush();
                m_notCompressed.Close();
                m_notCompressed = null;
            }

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            System.IO.File.Move(m_tempFileName, fileName);
            MyCompression.CompressFile(fileName);
        }

        //  Finally save the file as compressed value
        public void Save(out byte[] compressedBytes)
        {
            if (m_notCompressed != null)
            {
                m_notCompressed.Flush();
                m_notCompressed.Close();
                m_notCompressed = null;
            }

            MyCompression.CompressFile(m_tempFileName);

            using (FileStream fs = File.OpenRead(m_tempFileName))
            {
                long length = fs.Length;
                compressedBytes = new byte[length];

                using (BinaryReader br = new BinaryReader(fs))
                {
                    br.Read(compressedBytes, 0, (int)length);
                }
            }
        }

        //  Finally save the file as compressed value
        public void Save(string fileName, byte[] notCompressed)
        {
            //  Compress byte array
            byte[] compressed = MyCompression.Compress(notCompressed);

            using (FileStream fs = File.Create(fileName))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(compressed);
                }
            }
        }

        //  Finally save the file as compressed value
        public static void SaveFile(string fileName)
        {
            //  Compress byte array
            MyCompression.CompressFile(fileName);
        }
    }
}

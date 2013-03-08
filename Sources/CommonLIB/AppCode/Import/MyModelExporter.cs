using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics.PackedVector;
using System.IO;
using BulletXNA.BulletCollision;
using MinerWars;

namespace MinerWars.CommonLIB.AppCode.Import
{
    public class MyModelExporter
    {
        private BinaryWriter m_writer = null;

        private Dictionary<string, object> m_retTagData = new Dictionary<string, object>();

        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="filePath"></param>
        public MyModelExporter(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            m_writer = new BinaryWriter(fileStream);
        }


        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="filePath"></param>
        public MyModelExporter()
        {
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            if (m_writer != null)
                m_writer.Close();
        }


        /// <summary>
        /// WriteTag
        /// </summary>
        /// <param name="tagName"></param>
        private void WriteTag(string tagName)
        {
            m_writer.Write(tagName);
        }

        /// <summary>
        /// WriteVector3
        /// </summary>
        /// <param name="vct"></param>
        private void WriteVector3(ref Vector3 vct)
        {
            m_writer.Write(vct.X);
            m_writer.Write(vct.Y);
            m_writer.Write(vct.Z);
        }


        /// <summary>
        /// WriteVector2
        /// </summary>
        /// <param name="vct"></param>
        private void WriteVector2(ref Vector2 vct)
        {
            m_writer.Write(vct.X);
            m_writer.Write(vct.Y);
        }


        /// <summary>
        /// WriteMatrix
        /// </summary>
        /// <param name="matrix"></param>
        private void WriteMatrix(ref Matrix matrix)
        {
            m_writer.Write(matrix.M11);
            m_writer.Write(matrix.M12);
            m_writer.Write(matrix.M13);
            m_writer.Write(matrix.M14);

            m_writer.Write(matrix.M21);
            m_writer.Write(matrix.M22);
            m_writer.Write(matrix.M23);
            m_writer.Write(matrix.M24);

            m_writer.Write(matrix.M31);
            m_writer.Write(matrix.M32);
            m_writer.Write(matrix.M33);
            m_writer.Write(matrix.M34);

            m_writer.Write(matrix.M41);
            m_writer.Write(matrix.M42);
            m_writer.Write(matrix.M43);
            m_writer.Write(matrix.M44);
        }

        /// <summary>
        /// Write HalfVector4
        /// </summary>
        private void WriteHalfVector4(ref HalfVector4 val)
        {
            m_writer.Write(val.PackedValue);
        }

        /// <summary>
        /// Write HalfVector2
        /// </summary>
        private void WriteHalfVector2(ref HalfVector2 val)
        {
            m_writer.Write(val.PackedValue);
        }

        /// <summary>
        /// Write Byte4
        /// </summary>
        private void WriteByte4(ref Byte4 val)
        {
            m_writer.Write(val.PackedValue);
        }

        public bool ExportDataPackedAsHV4(string tagName, Vector3[] vctArr)
        {
            WriteTag(tagName);

            if (vctArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(vctArr.Length);
            foreach (Vector3 vctVal in vctArr)
            {
                Vector3 v = vctVal;
                HalfVector4 vct = VF_Packer.PackPosition(ref v);
                WriteHalfVector4(ref vct);
            }

            return true;
        }

        public bool ExportDataPackedAsB4(string tagName, Vector3[] vctArr)
        {
            WriteTag(tagName);

            if (vctArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(vctArr.Length);
            foreach (Vector3 vctVal in vctArr)
            {
                Vector3 v = vctVal;
                Byte4 vct = new Byte4();
                vct.PackedValue = VF_Packer.PackNormal(ref v);
                WriteByte4(ref vct);
            }

            return true;
        }

        public bool ExportDataPackedAsHV2(string tagName, Vector2[] vctArr)
        {
            WriteTag(tagName);

            if (vctArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(vctArr.Length);
            foreach (Vector2 vctVal in vctArr)
            {
                HalfVector2 vct = new HalfVector2(vctVal);
                WriteHalfVector2(ref vct);
            }

            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="vctArray"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Vector3[] vctArr)
        {
            if (vctArr == null)
                return true;

            WriteTag(tagName);
            m_writer.Write(vctArr.Length);
            foreach (Vector3 vctVal in vctArr)
            {
                Vector3 vct = vctVal;
                WriteVector3(ref vct);
            }

            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="vctArray"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Matrix[] matArr)
        {
            if (matArr == null)
                return true;

            WriteTag(tagName);
            m_writer.Write(matArr.Length);
            foreach (Matrix matVal in matArr)
            {
                Matrix mat = matVal;
                WriteMatrix(ref mat);
            }

            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="vctArray"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Vector2[] vctArr)
        {
            WriteTag(tagName);

            if (vctArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(vctArr.Length);
            foreach (Vector2 vctVal in vctArr)
            {
                Vector2 vct = vctVal;
                WriteVector2(ref vct);
            }

            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="strArr"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, string[] strArr)
        {
            WriteTag(tagName);

            if (strArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(strArr.Length);
            foreach (string sVal in strArr)
                m_writer.Write(sVal);

            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="strArr"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, int[] intArr)
        {
            WriteTag(tagName);

            if (intArr == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(intArr.Length);
            foreach(int iVal in intArr)
                m_writer.Write(iVal);

            return true;
        }


        public bool ExportData(string tagName, byte[] byteArray)
        {
            WriteTag(tagName);

            if (byteArray == null)
            {
                m_writer.Write(0);
                return true;
            }

            m_writer.Write(byteArray.Length);
            m_writer.Write(byteArray);
            return true;
        }

        public bool ExportData(string tagName, MyModelInfo modelInfo)
        {
            WriteTag(tagName);

            m_writer.Write(modelInfo.TrianglesCount);
            m_writer.Write(modelInfo.VerticesCount);
            WriteVector3(ref modelInfo.BoundingBoxSize);
            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="strArr"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, ref BoundingBox boundingBox)
        {
            WriteTag(tagName);
            WriteVector3(ref boundingBox.Min);
            WriteVector3(ref boundingBox.Max);
            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="strArr"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, ref BoundingSphere boundingSphere)
        {
            WriteTag(tagName);
            WriteVector3(ref boundingSphere.Center);
            m_writer.Write(boundingSphere.Radius);
            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Dictionary<string, Matrix> dict)
        {
            WriteTag(tagName);
            m_writer.Write(dict.Count);
            foreach (KeyValuePair<string, Matrix> pair in dict)
            {
                m_writer.Write(pair.Key);
                Matrix mat = pair.Value;
                WriteMatrix(ref mat);
            }
            return true;
        }


        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Dictionary<int, MyMeshPartInfo> dict)
        {
            WriteTag(tagName);
            m_writer.Write(dict.Count);
            foreach (KeyValuePair<int, MyMeshPartInfo> pair in dict)
            {
                MyMeshPartInfo meshInfo = pair.Value;
                meshInfo.Export(m_writer);
            }


            return true;
        }

        /// <summary>
        /// ExportData
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public bool ExportData(string tagName, Dictionary<string, MyModelDummy> dict)
        {
            WriteTag(tagName);
            m_writer.Write(dict.Count);
            foreach (KeyValuePair<string, MyModelDummy> pair in dict)
            {
                m_writer.Write(pair.Key);
                Matrix mat = pair.Value.Matrix;
                WriteMatrix(ref mat);

                m_writer.Write(pair.Value.CustomData.Count);
                foreach (KeyValuePair<string, object> customDataPair in pair.Value.CustomData)
                {
                    m_writer.Write(customDataPair.Key);
                    m_writer.Write(customDataPair.Value.ToString());
                }
            }
            return true;
        }


        /// <summary>
        /// ExportFloat
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ExportFloat(string tagName, float value)
        {
            WriteTag(tagName);
            m_writer.Write(value);
            return true;
        }


        /// <summary>
        /// ExportFloat
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ExportBool(string tagName, bool value)
        {
            WriteTag(tagName);
            m_writer.Write(value);
            return true;
        }

        /// <summary>
        /// Read HalfVector4
        /// </summary>
        HalfVector4 ReadHalfVector4(BinaryReader reader)
        {
            HalfVector4 vct = new HalfVector4();
            vct.PackedValue = reader.ReadUInt64();
            return vct;
        }

        /// <summary>
        /// Read HalfVector2
        /// </summary>
        HalfVector2 ReadHalfVector2(BinaryReader reader)
        {
            HalfVector2 vct = new HalfVector2();
            vct.PackedValue = reader.ReadUInt32();
            return vct;
        }

        /// <summary>
        /// Read Byte4
        /// </summary>
        Byte4 ReadByte4(BinaryReader reader)
        {
            Byte4 vct = new Byte4();
            vct.PackedValue = reader.ReadUInt32();
            return vct;
        }

        /// <summary>
        /// ImportVector3
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Vector3 ImportVector3(BinaryReader reader)
        {
            Vector3 vct;
            vct.X = reader.ReadSingle();
            vct.Y = reader.ReadSingle();
            vct.Z = reader.ReadSingle();
            return vct;
        }


        /// <summary>
        /// ImportVector2
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Vector2 ImportVector2(BinaryReader reader)
        {
            Vector2 vct;
            vct.X = reader.ReadSingle();
            vct.Y = reader.ReadSingle();
            return vct;
        }

        /// <summary>
        /// Read array of HalfVector4
        /// </summary>
        private HalfVector4[] ReadArrayOfHalfVector4(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            HalfVector4[] vctArr = new HalfVector4[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                vctArr[i] = this.ReadHalfVector4(reader);
            }

            return vctArr;
        }

        /// <summary>
        /// Read array of Byte4
        /// </summary>
        private Byte4[] ReadArrayOfByte4(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            Byte4[] vctArr = new Byte4[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                vctArr[i] = this.ReadByte4(reader);
            }

            return vctArr;
        }

        /// <summary>
        /// Read array of HalfVector2
        /// </summary>
        private HalfVector2[] ReadArrayOfHalfVector2(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            HalfVector2[] vctArr = new HalfVector2[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                vctArr[i] = this.ReadHalfVector2(reader);
            }

            return vctArr;
        }



        /// <summary>
        /// ReadArrayOfVector3
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private Vector3[] ReadArrayOfVector3(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            Vector3[] vctArr = new Vector3[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                vctArr[i] = this.ImportVector3(reader);
            }

            return vctArr;
        }


        /// <summary>
        /// ReadArrayOfVector2
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private Vector2[] ReadArrayOfVector2(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            Vector2[] vctArr = new Vector2[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                vctArr[i] = this.ImportVector2(reader);
            }

            return vctArr;
        }

        /// <summary>
        /// ReadArrayOfString
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string[] ReadArrayOfString(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            string[] strArr = new string[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                strArr[i] = reader.ReadString();
            }

            return strArr;
        }


        /// <summary>
        /// ReadBoundingBox
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private BoundingBox ReadBoundingBox(BinaryReader reader)
        {
            BoundingBox bbox;
            bbox.Min = this.ImportVector3(reader);
            bbox.Max = this.ImportVector3(reader);
            return bbox;

        }


        /// <summary>
        /// ReadBoundingSphere
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private BoundingSphere ReadBoundingSphere(BinaryReader reader)
        {
            BoundingSphere bsphere;
            bsphere.Center = this.ImportVector3(reader);
            bsphere.Radius = reader.ReadSingle();
            return bsphere;
        }


        /// <summary>
        /// ReadMatrix
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Matrix ReadMatrix(BinaryReader reader)
        {
            Matrix mat;
            mat.M11 = reader.ReadSingle();
            mat.M12 = reader.ReadSingle();
            mat.M13 = reader.ReadSingle();
            mat.M14 = reader.ReadSingle();

            mat.M21 = reader.ReadSingle();
            mat.M22 = reader.ReadSingle();
            mat.M23 = reader.ReadSingle();
            mat.M24 = reader.ReadSingle();

            mat.M31 = reader.ReadSingle();
            mat.M32 = reader.ReadSingle();
            mat.M33 = reader.ReadSingle();
            mat.M34 = reader.ReadSingle();

            mat.M41 = reader.ReadSingle();
            mat.M42 = reader.ReadSingle();
            mat.M43 = reader.ReadSingle();
            mat.M44 = reader.ReadSingle();

            return mat;
        }


        /// <summary>
        /// ReadMeshParts
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<MyMeshPartInfo> ReadMeshParts(BinaryReader reader)
        {
            List<MyMeshPartInfo> list = new List<MyMeshPartInfo>();
            int nCount = reader.ReadInt32();
            for (int i = 0; i < nCount; ++i)
            {
                MyMeshPartInfo meshPart = new MyMeshPartInfo();
                meshPart.Import(reader);
                list.Add(meshPart);
            }

            return list;
        }


        /// <summary>
        /// ReadDummies
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Dictionary<string, MyModelDummy> ReadDummies(BinaryReader reader)
        {
            Dictionary<string, MyModelDummy> dummies = new Dictionary<string, MyModelDummy>();

            int nCount = reader.ReadInt32();
            for (int i = 0; i < nCount; ++i)
            {
                string str = reader.ReadString();
                Matrix mat = this.ReadMatrix(reader);

                MyModelDummy dummy = new MyModelDummy();
                dummy.Matrix = mat;
                dummy.CustomData = new Dictionary<string, object>();

                int customDataCount = reader.ReadInt32();
                for (int j = 0; j < customDataCount; ++j)
                {
                    string name = reader.ReadString();
                    string value = reader.ReadString();
                    dummy.CustomData.Add(name, value);
                }

                dummies.Add(str, dummy);
            }

            return dummies;
        }


        /// <summary>
        /// ReadArrayOfInt
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private int[] ReadArrayOfInt(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            int[] intArr = new int[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                intArr[i] = reader.ReadInt32();
            }

            return intArr;
        }


        private byte[] ReadArrayOfBytes(BinaryReader reader)
        {
            int nCount = reader.ReadInt32();
            byte[] data = reader.ReadBytes(nCount);
            return data;
        }


        /// <summary>
        /// ImportData
        /// </summary>
        /// <param name="assetFileName"></param>
        /// <returns></returns>
        public void ImportData(string assetFileName)
        {
            m_retTagData.Clear();

            using (FileStream fs = new FileStream(assetFileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    LoadTagData(reader);
                }
            }
        }

        /// <summary>
        /// ImportCustomData
        /// </summary>
        /// <param name="assetFileName"></param>
        /// <returns></returns>
        public void ImportCustomData(string assetFileName, SortedSet<string> tags)
        {
            using (FileStream fs = new FileStream(assetFileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    LoadCustomTagData(reader, tags);
                }
            }
        }

        /// <summary>
        /// LoadTagData
        /// </summary>
        /// <returns></returns>
        private bool LoadTagData(BinaryReader reader)
        {
            //m_retTagData
            string tagName;
            tagName = reader.ReadString();
            string[] strArr = ReadArrayOfString(reader);
            m_retTagData.Add(tagName, strArr);

            //@ TAG_DUMMIES
            tagName = reader.ReadString();
            Dictionary<string, MyModelDummy> dummies = this.ReadDummies(reader);
            m_retTagData.Add(tagName, dummies);

            //@ verticies
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            Vector3[] VctArr = ReadArrayOfVector3(reader);
#else
            HalfVector4[] VctArr = ReadArrayOfHalfVector4(reader);
#endif
            m_retTagData.Add(tagName, VctArr);
            //@ normals
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            m_retTagData.Add(tagName, VctArr);
#else
            Byte4[] VctArr2 = ReadArrayOfByte4(reader);
            m_retTagData.Add(tagName, VctArr2);
#endif

            //@ indicies
            // removed TAG_INDICES from loading
//          tagName = reader.ReadString();
//          int[] intArr = ReadArrayOfInt(reader);
//          m_retTagData.Add(tagName, intArr);

            //@ texCoords0
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            Vector2[] vct2Arr = ReadArrayOfVector2(reader);
#else
            HalfVector2[] vct2Arr = ReadArrayOfHalfVector2(reader);
#endif
            m_retTagData.Add(tagName, vct2Arr);
            //@ binormals 
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            m_retTagData.Add(tagName, VctArr);
#else
            VctArr2 = ReadArrayOfByte4(reader);
            m_retTagData.Add(tagName, VctArr2);
#endif
            //@ tangents
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            m_retTagData.Add(tagName, VctArr);
#else
            VctArr2 = ReadArrayOfByte4(reader);
            m_retTagData.Add(tagName, VctArr2);
#endif
            //@ texcoords1
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            vct2Arr = ReadArrayOfVector2(reader);
#else
            vct2Arr = ReadArrayOfHalfVector2(reader);
#endif
            m_retTagData.Add(tagName, vct2Arr);

            //////////////////////////////////////////////////////////////////////////

            //@ TAG_RESCALE_TO_LENGTH_IN_METERS
            tagName = reader.ReadString();
            bool bVal = reader.ReadBoolean();
            m_retTagData.Add(tagName, bVal);
            //@ TAG_LENGTH_IN_METERS
            tagName = reader.ReadString();
            float fVal = reader.ReadSingle();
            m_retTagData.Add(tagName, fVal);
            //@ TAG_RESCALE_FACTOR
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            m_retTagData.Add(tagName, fVal);
            //@ TAG_CENTERED
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            m_retTagData.Add(tagName, bVal);
            //@ TAG_USE_MASK
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            m_retTagData.Add(tagName, bVal);
            //@ TAG_SPECULAR_SHININESS
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            m_retTagData.Add(tagName, fVal);
            //@ TAG_SPECULAR_POWER
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            m_retTagData.Add(tagName, fVal);
            //@ TAG_BOUNDING_BOX
            tagName = reader.ReadString();
            BoundingBox bbox = this.ReadBoundingBox(reader);
            m_retTagData.Add(tagName, bbox);
            //@ TAG_BOUNDING_SPHERE
            tagName = reader.ReadString();
            BoundingSphere bSphere = this.ReadBoundingSphere(reader);
            m_retTagData.Add(tagName, bSphere);
            //@ TAG_SWAP_WINDING_ORDER
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            m_retTagData.Add(tagName, bVal);

            

            //@ TAG_MESH_PARTS
            tagName = reader.ReadString();
            List<MyMeshPartInfo> meshes = this.ReadMeshParts(reader);
            m_retTagData.Add(tagName, meshes);

            //@ TAG_MODEL_BVH
            tagName = reader.ReadString();
            GImpactQuantizedBvh bvh = new GImpactQuantizedBvh();
            bvh.Load(this.ReadArrayOfBytes(reader));
            m_retTagData.Add(tagName, bvh);


            //@ TAG_MODEL_INFO
            tagName = reader.ReadString();
            int tri, vert;
            Vector3 bb;
            tri = reader.ReadInt32();
            vert = reader.ReadInt32();
            bb = ImportVector3(reader);
            m_retTagData.Add(tagName, new MyModelInfo(tri, vert, bb));

            return true;
        }

        /// <summary>
        /// LoadCustomTagData
        /// </summary>
        /// <returns></returns>
        private bool LoadCustomTagData(BinaryReader reader, SortedSet<string> tags)
        {
            int tagsToRead = tags.Count;

            //m_retTagData
            string tagName;
            tagName = reader.ReadString();
            string[] strArr = ReadArrayOfString(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, strArr);
                if (--tagsToRead <= 0) return true;
            }

            //@ TAG_DUMMIES
            tagName = reader.ReadString();
            Dictionary<string, MyModelDummy> dummies = this.ReadDummies(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, dummies);
                if (--tagsToRead <= 0) return true;
            }

            //@ verticies
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            Vector3[] VctArr = ReadArrayOfVector3(reader);
#else
            HalfVector4[] VctArr = ReadArrayOfHalfVector4(reader);
#endif
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr);
                if (--tagsToRead <= 0) return true;
            }
            //@ normals
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr);
                if (--tagsToRead <= 0) return true;
            }
#else
            Byte4[] VctArr2 = ReadArrayOfByte4(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr2);
                if (--tagsToRead <= 0) return true;
            }
#endif
            //@ indicies
            // removed TAG_INDICES from loading
            //          tagName = reader.ReadString();
            //          int[] intArr = ReadArrayOfInt(reader);
            //          m_retTagData.Add(tagName, intArr);

            //@ texCoords0
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            Vector2[] vct2Arr = ReadArrayOfVector2(reader);
#else
            HalfVector2[] vct2Arr = ReadArrayOfHalfVector2(reader);
#endif
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, vct2Arr);
                if (--tagsToRead <= 0) return true;
            }
            //@ binormals 
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr);
                if (--tagsToRead <= 0) return true;
            }
#else
            VctArr2 = ReadArrayOfByte4(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr2);
                if (--tagsToRead <= 0) return true;
            }
#endif
            //@ tangents
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            VctArr = ReadArrayOfVector3(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr);
                if (--tagsToRead <= 0) return true;
            }
#else
            VctArr2 = ReadArrayOfByte4(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, VctArr2);
                if (--tagsToRead <= 0) return true;
            }
#endif
            //@ texcoords1
            tagName = reader.ReadString();
#if !PACKED_VERTEX_FORMAT
            vct2Arr = ReadArrayOfVector2(reader);
#else
            vct2Arr = ReadArrayOfHalfVector2(reader);
#endif
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, vct2Arr);
                if (--tagsToRead <= 0) return true;
            }
            //////////////////////////////////////////////////////////////////////////

            //@ TAG_RESCALE_TO_LENGTH_IN_METERS
            tagName = reader.ReadString();
            bool bVal = reader.ReadBoolean();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_LENGTH_IN_METERS
            tagName = reader.ReadString();
            float fVal = reader.ReadSingle();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, fVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_RESCALE_FACTOR
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, fVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_CENTERED
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_USE_MASK
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_SPECULAR_SHININESS
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, fVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_SPECULAR_POWER
            tagName = reader.ReadString();
            fVal = reader.ReadSingle();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, fVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_BOUNDING_BOX
            tagName = reader.ReadString();
            BoundingBox bbox = this.ReadBoundingBox(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bbox);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_BOUNDING_SPHERE
            tagName = reader.ReadString();
            BoundingSphere bSphere = this.ReadBoundingSphere(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bSphere);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_SWAP_WINDING_ORDER
            tagName = reader.ReadString();
            bVal = reader.ReadBoolean();
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, bVal);
                if (--tagsToRead <= 0) return true;
            }
            //@ TAG_MESH_PARTS
            tagName = reader.ReadString();
            List<MyMeshPartInfo> meshes = this.ReadMeshParts(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, meshes);
                if (--tagsToRead <= 0) return true;
            }

            //@ TAG_MODEL_BVH
            tagName = reader.ReadString();
            var bvhBytes = this.ReadArrayOfBytes(reader);
            if (tags.Contains(tagName))
            {
                GImpactQuantizedBvh bvh = new GImpactQuantizedBvh();
                bvh.Load(bvhBytes);
                m_retTagData.Add(tagName, bvh);
                if (--tagsToRead <= 0) return true;
            }

            //@ TAG_MODEL_INFO
            tagName = reader.ReadString();
            int tri, vert;
            Vector3 bb;
            tri = reader.ReadInt32();
            vert = reader.ReadInt32();
            bb = ImportVector3(reader);
            if (tags.Contains(tagName))
            {
                m_retTagData.Add(tagName, new MyModelInfo(tri, vert, bb));
                if (--tagsToRead <= 0) return true;
            }

            return false;
        }

        /// <summary>
        /// GetTagData
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetTagData() { return m_retTagData; }
    }
}

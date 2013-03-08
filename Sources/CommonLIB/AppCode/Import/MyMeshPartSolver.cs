using System;
using System.Collections.Generic;
using System.IO;
using MinerWarsMath;
using MinerWarsMath.Graphics.PackedVector;

namespace MinerWars.CommonLIB.AppCode.Import
{
    //  IMPORTANT: If you add/delete technique in this enum, don't forget to change code
    //  in these methods: 
    //      MyModel.CreateVertexBuffer
    public enum MyMeshDrawTechnique : byte
    {
        MESH,                      //  Renders using diffuse, normal map and specular textures
        VOXELS_DEBRIS,              //  For explosion debris objects, with scaling and texture is calculated by tri-planar mapping (same as with voxel maps)
        VOXELS_STATIC_ASTEROID,     //  For static asteroids, therefore texture is calculated by tri-planar mapping (same as with voxel maps)
        VOXEL_MAP,                  //  Destroyable voxel asteroid
        ALPHA_MASKED,               //  Alpha masked object

        //Leave decal type last because it is alpha blended, meshes are sorted by this enum
        DECAL,                      //  Alpha blended object, it has alpha in diffuse.a texture channel and emissivity in normal.a texture channel
        HOLO,                       //  Advanced type of blended object, it has some special features decal doesnt have (cull none, no physics, sorting..)
    }

    public static class PositionPacker
    {
        static public HalfVector4 PackPosition(ref Vector3 position)
        {
            float max_value = System.Math.Max(System.Math.Abs(position.X), System.Math.Abs(position.Y));
            max_value = System.Math.Max(max_value, System.Math.Abs(position.Z));
            float multiplier = System.Math.Min((float)System.Math.Floor(max_value), 2048.0f);
            float invMultiplier = 0;
            if (multiplier > 0)
                invMultiplier = 1.0f / multiplier;
            else
                multiplier = invMultiplier = 1.0f;

            return new HalfVector4(invMultiplier * position.X, invMultiplier * position.Y, invMultiplier * position.Z, multiplier);
        }

        static public Vector3 UnpackPosition(ref HalfVector4 position)
        {
            Vector4 unpacked = position.ToVector4();
            return unpacked.W * new Vector3(unpacked.X, unpacked.Y, unpacked.Z);
        }
    }


    public class MyModelDummy
    {
        public Dictionary<string,object> CustomData;
        public Matrix Matrix;
    }

    public class MyModelInfo
    {
        public int TrianglesCount;
        public int VerticesCount;
        public Vector3 BoundingBoxSize;

        public MyModelInfo(int triCnt, int VertCnt, Vector3 BBsize)
        {
            this.TrianglesCount = triCnt;
            this.VerticesCount = VertCnt;
            this.BoundingBoxSize = BBsize;
        }
    }

    public class MyMeshPartInfo
    {
        public int m_MaterialHash;
		public MyMaterialDescriptor m_MaterialDesc = null;
        public List<int> m_Indicies = new List<int>();
        public MyMeshDrawTechnique m_MeshRenderTechnique = MyMeshDrawTechnique.MESH;

        public static bool IsPhysical(MyMeshDrawTechnique technique)
        {
            //return true;
            return technique != MyMeshDrawTechnique.HOLO && technique != MyMeshDrawTechnique.DECAL && technique != MyMeshDrawTechnique.ALPHA_MASKED;
        }

        public bool Export(BinaryWriter writer)
        {
            writer.Write(m_MaterialHash);
			writer.Write((int)m_MeshRenderTechnique);
            writer.Write(m_Indicies.Count);
            foreach (int indice in m_Indicies)
                writer.Write(indice);

            bool bRes = true;
            if (m_MaterialDesc != null)
            {
                writer.Write(true);
                bRes = m_MaterialDesc.Write(writer);
            }
            else
            {
                writer.Write(false);
            }
			
			return bRes;
        }

        public bool Import(BinaryReader reader)
        {
            m_MaterialHash = reader.ReadInt32();
            m_MeshRenderTechnique = (MyMeshDrawTechnique)reader.ReadInt32();
            int nCount = reader.ReadInt32();
            for (int i = 0; i < nCount; ++i)
            {
                m_Indicies.Add(reader.ReadInt32());
            }

            bool bMatDesc = reader.ReadBoolean();
            bool bRes = true;
            if (bMatDesc)
            {
                m_MaterialDesc = new MyMaterialDescriptor();
                bRes = m_MaterialDesc.Read(reader);
            }
            else
            {
                m_MaterialDesc = null;
            }
            
            return bRes;
        }
    }

    public class MyMeshPartSolver
    {
        const string C_TEXTURE_IDENTIFICATOR_STR = "Texture";
        static int C_TEXTURE_IDENTIFICATOR = C_TEXTURE_IDENTIFICATOR_STR.GetHashCode();
        static string C_DECAL_PREFIX = "decal_";
        static string C_HOLO_PREFIX = "holo_";
        static string C_MASK_PREFIX = "mask_";

        private Dictionary<int, MyMeshPartInfo> m_PartContainer = new Dictionary<int, MyMeshPartInfo>();    //hash of relativeFile
        public Dictionary<int, MyMeshPartInfo> GetMeshPartContainer() { return m_PartContainer; }

        //  IMPORTANT: If you change these constants, don't forget to change them also in MyMwcMathConstants
        const float EPSILON = 0.00001f;
        const float EPSILON_SQUARED = EPSILON * EPSILON;
                      /*
        public void SetMaterial(MaterialContent material)
        {
            if (material.Textures.Count == 0)
                return;

            int matHash = material.Name.GetHashCode();
            if (!m_PartContainer.ContainsKey(matHash))
                return;

            MyMeshPartInfo mpInfo = m_PartContainer[matHash];

            ExternalReference<TextureContent> tex;
            if (material.Textures.TryGetValue("Texture", out tex) == false) throw new Exception("Material doesn't have 'Texture' property specified. Please fix the FBX file. Stack trace: " + Environment.StackTrace);

			MyMaterialDescriptor matDesc = new MyMaterialDescriptor(material.Name);
			matDesc.m_DiffuseTextureName = tex.Filename;

			object specColor = material.OpaqueData.GetValue("SpecularColor", Vector3.Zero);
            if (specColor != null && specColor is Vector3)
			{
                matDesc.m_SpecularColor = ((Vector3)(specColor));
			}

            object glossiness = material.OpaqueData.GetValue("SpecularPower", 0f);
            if (glossiness != null && glossiness is float)
            {
                matDesc.m_Glossiness = (float)glossiness;
            }
		
            object diffuseColor = material.OpaqueData.GetValue("DiffuseColor", Vector3.One);
            if (diffuseColor != null && diffuseColor is Vector3)
            {
                //Disabled because there is a lot of garbage in diffuse color in exported objects
                //and it is not used at all however
                //matDesc.m_DiffuseColor = (Vector3)diffuseColor;
                matDesc.m_DiffuseColor = Vector3.One;
            }

			mpInfo.m_MaterialDesc = matDesc;
        }

        public void GenerateSpecialRenderTechnique()
        {
            foreach (KeyValuePair<int, MyMeshPartInfo> pair in m_PartContainer)
            {
				MyMaterialDescriptor matDesc = pair.Value.m_MaterialDesc;
				if (matDesc == null)
					continue;

				string fileName = Path.GetFileNameWithoutExtension(matDesc.m_DiffuseTextureName);
                if (fileName == null)
                    continue;

                fileName = fileName.ToLower();

                if ((fileName.Length < C_DECAL_PREFIX.Length) || (fileName.Length < C_MASK_PREFIX.Length))
                    continue;

                if (fileName.Contains(C_DECAL_PREFIX))
                {
                    pair.Value.m_MeshRenderTechnique = MyMeshDrawTechnique.DECAL;
                    continue;
                }

                if (fileName.Contains(C_HOLO_PREFIX))
                {
                    pair.Value.m_MeshRenderTechnique = MyMeshDrawTechnique.HOLO;
                    continue;
                }

                if (fileName.Contains(C_MASK_PREFIX))
                {
                    pair.Value.m_MeshRenderTechnique = MyMeshDrawTechnique.ALPHA_MASKED;
                    continue;
                }
            }
        }

        public void SetIndicies(NodeContent contentNode, ContentProcessorContext context, int matHash, int verticiesOffset, IndexCollection indicies, List<Vector3> vertices)
        {
            MyMeshPartInfo mpInfo;

            if (m_PartContainer.TryGetValue(matHash, out mpInfo) == false)
            {
                mpInfo = new MyMeshPartInfo();
                mpInfo.m_MaterialHash = matHash;
                m_PartContainer.Add(matHash, mpInfo);
            }

            int numberOfWrongTriangles = 0;
            for (int i = 0; i < indicies.Count; i += 3)
            {
                int index0 = indicies[i + 0] + verticiesOffset;
                int index1 = indicies[i + 1] + verticiesOffset;
                int index2 = indicies[i + 2] + verticiesOffset;

                Vector3 vertex0 = vertices[index0];
                Vector3 vertex1 = vertices[index1];
                Vector3 vertex2 = vertices[index2];

                HalfVector4 packedVertex0 = PositionPacker.PackPosition(ref vertex0);
                HalfVector4 packedVertex1 = PositionPacker.PackPosition(ref vertex1);
                HalfVector4 packedVertex2 = PositionPacker.PackPosition(ref vertex2);

                Vector3 unpackedVertex0 = PositionPacker.UnpackPosition(ref packedVertex0);
                Vector3 unpackedVertex1 = PositionPacker.UnpackPosition(ref packedVertex1);
                Vector3 unpackedVertex2 = PositionPacker.UnpackPosition(ref packedVertex2);

                if (IsWrongTriangle(unpackedVertex0, unpackedVertex1, unpackedVertex2) == true)
                {
                    numberOfWrongTriangles++;
                }
                else
                {
                    mpInfo.m_Indicies.Add(index0);
                    mpInfo.m_Indicies.Add(index1);
                    mpInfo.m_Indicies.Add(index2);
                }
            }

            if (numberOfWrongTriangles > 0)
            {
                context.Logger.LogWarning("", contentNode.Identity, "WARNING! Mesh has " + numberOfWrongTriangles + " wrong triangles. They are going to be ignored, but artist should fix it!");
            }
        }
               */
        public void Clear()
        {
            m_PartContainer.Clear();
        }

        //  We want to skip all wrong triangles, those that have two vertex at almost the same location, etc.
        //  We do it simply, by calculating triangle normal and then checking if this normal has length large enough
        bool IsWrongTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            //  Distance between two vertexes is the fastest test
            Vector3 triangleEdgeVector1 = vertex2 - vertex0;
            if (triangleEdgeVector1.LengthSquared() <= EPSILON_SQUARED) return true;

            //  Distance between two vertexes is the fastest test
            Vector3 triangleEdgeVector2 = vertex1 - vertex0;
            if (triangleEdgeVector2.LengthSquared() <= EPSILON_SQUARED) return true;

            //  Distance between two vertexes is the fastest test
            Vector3 triangleEdgeVector3 = vertex1 - vertex2;
            if (triangleEdgeVector3.LengthSquared() <= EPSILON_SQUARED) return true;

            //  But we also need to do a cross product, because distance tests are not sufficient in case when all vertexes lie on a line
            Vector3 norm;
            Vector3.Cross(ref triangleEdgeVector1, ref triangleEdgeVector2, out norm);
            if (norm.Length() < EPSILON) return true;

            return false;
        }
    }
}
using System;
using System.IO;

using MinerWars.AppCode.App;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers;

//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Models
{
    internal class MyMesh
    {
        private const string C_CONTENT_ID = "Content\\";
        private const string C_POSTFIX_DIFFUSE = "_d";
        internal const string C_POSTFIX_DIFFUSE_EMISSIVE = "_de";
        private const string C_POSTFIX_DONT_HAVE_NORMAL = "_dn";
        internal const string C_POSTFIX_NORMAL_SPECULAR = "_ns";
        private const string DEFAULT_DIRECTORY = "\\v01\\";

        private IndexBuffer m_IndexBuffer;
        private readonly string m_assetName;
        public readonly MyMeshMaterial[] Materials = null;

        public int IndexStart;
        public int TriCount;

        /// <summary>
        /// c-tor - generic way for collecting resources
        /// </summary>
        /// <param name="meshInfo"></param>
        /// assetName - just for debug output
        public MyMesh(MyMeshPartInfo meshInfo, string assetName)
        {
            string textureName = null;
            MyMaterialDescriptor matDesc = meshInfo.m_MaterialDesc;
            if (matDesc != null)
            {
                bool hasNormalTexture = true;

                string texName = matDesc.m_DiffuseTextureName;
                if (String.IsNullOrEmpty(texName) == false)
                {
                    int i = texName.LastIndexOf(C_CONTENT_ID);

                    texName = texName.Substring(i + C_CONTENT_ID.Length, texName.Length - i - C_CONTENT_ID.Length);
                    //@ cut extension
                    int lastIndex = texName.LastIndexOf(".");
                    textureName = texName.Substring(0, texName.Length - (texName.Length - lastIndex));

                    if (textureName.LastIndexOf(C_POSTFIX_DONT_HAVE_NORMAL) == (textureName.Length - C_POSTFIX_DONT_HAVE_NORMAL.Length))
                    {
                        hasNormalTexture = false;
                        textureName = textureName.Substring(0, textureName.Length - C_POSTFIX_DONT_HAVE_NORMAL.Length);
                    }

                    //@ if postfix for diffuse is _d -> trim it
                    if (textureName.LastIndexOf(C_POSTFIX_DIFFUSE) == (textureName.Length - 2))
                    {
                        textureName = textureName.Substring(0, textureName.Length - 2);
                    }

                    if (textureName.LastIndexOf(C_POSTFIX_DIFFUSE_EMISSIVE) == (textureName.Length - 3))
                    {
                        textureName = textureName.Substring(0, textureName.Length - 3);
                    }
                }

                var defaultMaterial = new MyMeshMaterial(matDesc.MaterialName,
                                                         textureName + C_POSTFIX_DIFFUSE_EMISSIVE,
                                                         textureName + C_POSTFIX_NORMAL_SPECULAR, matDesc.m_Glossiness,
                                                         hasNormalTexture, ref matDesc.m_DiffuseColor,
                                                         ref matDesc.m_SpecularColor);

                // check for alternative textures and create corresponding materials.
                if (!textureName.Contains(DEFAULT_DIRECTORY))
                {
                    Materials = new MyMeshMaterial[8];
                    for (int j = 1; j < Materials.Length; j++)
                    {
                        Materials[j] = defaultMaterial;
                    }
                }
                else
                {
                    int materialCount = FindMaterialCount(textureName);

                    Materials = new MyMeshMaterial[materialCount];

                    // here check if corresponding textures exist in the "v02" or "v03" ...
                    // folder. If not, fall back to default "v01" folder
                    for (int j = 1; j < Materials.Length; j++)
                    {
                        string newFolder = "\\v" + String.Format("{0:00}", j + 1) + "\\";
                        string newNameDiffuse = textureName.Replace(DEFAULT_DIRECTORY, newFolder) + C_POSTFIX_DIFFUSE_EMISSIVE;
                        string newNameNormal = textureName.Replace(DEFAULT_DIRECTORY, newFolder) + C_POSTFIX_NORMAL_SPECULAR;

                        string diffusepath = Path.Combine(MyMinerGame.Static.RootDirectory, newNameDiffuse) + ".dds";
                        if (!File.Exists(diffusepath))
                            newNameDiffuse = textureName + C_POSTFIX_DIFFUSE_EMISSIVE;

                        if (!File.Exists(Path.Combine(MyMinerGame.Static.RootDirectory, newNameNormal) + ".dds"))
                            newNameNormal = textureName + C_POSTFIX_NORMAL_SPECULAR;

                        Materials[j] = new MyMeshMaterial(          matDesc.MaterialName,
                                                                    newNameDiffuse, newNameNormal,
                                                                      matDesc.m_Glossiness,
                                                                      hasNormalTexture, ref matDesc.m_DiffuseColor,

                                                                      ref matDesc.m_SpecularColor);
                    }
                }

                Materials[0] = defaultMaterial;
            }
            else
            {
                //It is OK because ie. collision meshes dont have materials
                //MyCommonDebugUtils.AssertRelease(false, String.Format("Model {0} has bad material for mesh.", assetName));
                Trace.TraceWarning("Model with null material: " + assetName);

                //We define at least debug material
                MinerWarsMath.Vector3 color = MinerWarsMath.Color.Pink.ToVector3();
                Materials = new MyMeshMaterial[8];
                Materials[0] = new MyMeshMaterial("", "Textures2\\Models\\Prefabs\\v01\\v01_cargo_box_de", "Textures2\\Models\\Prefabs\\v01\\v01_cargo_box_ns", 0, true, ref color, ref color);
                for (int j = 1; j < Materials.Length; j++)
                {
                    Materials[j] = Materials[0];
                }
            }

            for (int i = 0; i < Materials.Length; i++)
            {
                Materials[i].DrawTechnique = meshInfo.m_MeshRenderTechnique;
            }

            m_assetName = assetName;
        }

        /// <summary>
        /// Gets the number of existing folders with names "v01", "v02" ...
        /// </summary>
        /// <param name="textureName">Path to a texture that contains "v01".</param>
        /// <returns>Gets the number of available materials (textures) for the given textureName.</returns>
        private static int FindMaterialCount(string textureName)
        {
            int materialCount = 0;
            string directory = Path.Combine(MyMinerGame.Static.RootDirectory, textureName);
            directory = directory.Remove(1 + directory.LastIndexOf('\\'));
            while (Directory.Exists(directory))
            {
                materialCount++;
                directory = directory.Replace("\\v" + String.Format("{0:00}", materialCount) + "\\",
                                              "\\v" + String.Format("{0:00}", materialCount + 1) + "\\");
            }
            return materialCount;
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="grDevice"></param>
        /// <param name="vertexCount"></param>
        /// <param name="triCount"></param>
        public void Render(Device grDevice, int vertexCount)
        {
            grDevice.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, vertexCount, IndexStart, TriCount);
            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
        }
    }
}

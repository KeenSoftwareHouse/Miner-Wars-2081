#region

using System;
using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Physics;
using SysUtils.Utils;

using MinerWars.AppCode.Game.Render;
using SysUtils;
using System.Diagnostics;
using System.IO;
using MinerWars.AppCode.Game.Textures;
using BulletXNA.BulletCollision;
using MinerWars.AppCode.Physics.Collisions;
using MinerWarsMath.Graphics.PackedVector;
using System.Runtime.InteropServices;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

//  Coordinate system transformation:
//  3DS MAX displays different coordinate system than XNA, plus when converting to FBX, it looks like it switches sign of Z values (Z in XNA way).
//  So when thinking about converting coordinate system, this is it:
//  XNA X = 3DSMAX X
//  XNA Y = 3DSMAX Z
//  XNA Z = NEGATIVE OF 3DSMAX Y

namespace MinerWars.AppCode.Game.Models
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyModel : IDisposable, IPrimitiveManagerBase
    {
        private const string C_POSTFIX_MASK = "_m";

        static MyModelExporter m_exporter = new MyModelExporter();

        public readonly MyModelsEnum ModelEnum;

        public bool KeepInMemory { get; private set; }


        MyMeshDrawTechnique m_drawTechnique;
        VertexBuffer m_vertexBuffer = null;
        IndexBuffer m_indexBuffer = null;
        int m_verticesCount;
        int m_trianglesCount;
        int m_vertexBufferSize;
        int m_indexBufferSize;
        int m_vertexStride;
        VertexDeclaration m_vertexDeclaration;

        public int GetVBSize
        {
            get { return m_vertexBufferSize; }
        }
        public int GetIBSize
        {
            get { return m_indexBufferSize; }
        }

        //  Vertices and indices used for collision detection
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct MyCompressedVertexNormal
        {  //8 + 4 bytes
            public HalfVector4 Position;
            public Byte4 Normal;
        }

        //  Vertices and indices used for collision detection
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct MyVertexNormal
        {  //12 + 12 bytes
            public Vector3 Position;
            public Vector3 Normal;
        }

#if PACKED_VERTEX_FORMAT
        private MyCompressedVertexNormal[] m_vertices;
#else
        private MyVertexNormal[] m_vertices;
#endif

        private int[] m_Indicies = null;
        private ushort[] m_Indicies_16bit = null;

        public MyTriangleVertexIndices[] Triangles;       //  Triangles specified by three indicies to "Vertex" list      //TODO: Could be made readonly from the outside, and alterable only from the inside of this class
        public Dictionary<string, MyModelDummy> Dummies;
        public MyModelInfo ModelInfo;
        public MyTexture2D MaskTexture;

        /// <summary>
        /// State of loading.
        /// </summary>
        private volatile LoadState m_loadState;

        /// <summary>
        /// Gets or sets the state of the load.
        /// </summary>
        /// <value>
        /// The state of the load.
        /// </value>
        public LoadState LoadState
        {
            get
            {
                return this.m_loadState;
            }
            internal set
            {
                this.m_loadState = value;
            }
        }

        public int GetVertexStride()
        {
            return m_vertexStride;
        }

        public VertexDeclaration GetVertexDeclaration()
        {
            return m_vertexDeclaration;
        }

        public Vector3 GetVertexInt(int vertexIndex)
        {
#if PACKED_VERTEX_FORMAT
            return VF_Packer.UnpackPosition(ref m_vertices[vertexIndex].Position);
#else
            return m_vertices[vertexIndex].Position;
#endif
        }

        public Vector3 GetVertex(int vertexIndex)
        {
            return GetVertexInt(vertexIndex);
        }

        public void GetVertex(int vertexIndex1, int vertexIndex2, int vertexIndex3, out Vector3 v1, out Vector3 v2, out Vector3 v3)
        {   
            v1 = GetVertex(vertexIndex1);
            v2 = GetVertex(vertexIndex2);
            v3 = GetVertex(vertexIndex3);
        }

        public Vector3 GetVertexNormal(int vertexIndex)
        {
#if PACKED_VERTEX_FORMAT
           return  VF_Packer.UnpackNormal(ref m_vertices[vertexIndex].Normal);
#else
           return m_vertices[vertexIndex].Normal;
#endif
        }

        public Vector3 GetVertexBinormal(int vertexIndex)
        {
#if PACKED_VERTEX_FORMAT
            return VF_Packer.UnpackNormal(ref m_forLoadingBinormals[vertexIndex]);
#else
            return m_forLoadingBinormals[vertexIndex];
#endif
        }

        public Vector3 GetVertexTangent(int vertexIndex)
        {
#if PACKED_VERTEX_FORMAT
            return VF_Packer.UnpackNormal(ref m_forLoadingTangents[vertexIndex]);
#else
            return m_forLoadingTangents[vertexIndex];
#endif
        }

        //  Used only for loading and then disposed. It lives between LoadData and LoadInDraw... but that's OK because it's only during sector loading
        //  and I have to make it so because we can't load vertex/index buffers from other place than Draw call
#if PACKED_VERTEX_FORMAT
        HalfVector2[] m_forLoadingTexCoords0;
        HalfVector2[] m_forLoadingTexCoords1;
        Byte4[] m_forLoadingBinormals;
        Byte4[] m_forLoadingTangents;
#else
        Vector2[] m_forLoadingTexCoords0;
        Vector2[] m_forLoadingTexCoords1;
        Vector3[] m_forLoadingBinormals;
        Vector3[] m_forLoadingTangents;
#endif

        float m_specularShininess;
        float m_specularPower;
        float m_rescaleFactor;
        bool m_useChannelTextures;

        //  Bounding volumes
        public BoundingSphere BoundingSphere;   //TODO: Could be made readonly from the outside, and alterable only from the inside of this class
        public BoundingBox BoundingBox;         //TODO: Could be made readonly from the outside, and alterable only from the inside of this class

        //  Size of the bounding box
        public Vector3 BoundingBoxSize;         //TODO: Could be made readonly from the outside, and alterable only from the inside of this class
        public Vector3 BoundingBoxSizeHalf;     //TODO: Could be made readonly from the outside, and alterable only from the inside of this class

        //  Octree
        IMyTriangePruningStructure m_bvh;

        readonly string m_assetName;
        bool m_loadedData;
        bool m_loadedContent;


        public bool LoadedData
        {
            get { return m_loadedData; }
        }

        public bool LoadedContent
        {
            get { return m_loadedContent; }
        }

        List<MyMesh> m_meshContainer = new List<MyMesh>();

        public bool UseChannels
        {
            get
            {
                return m_useChannelTextures && MyRenderConstants.RenderQualityProfile.UseChannels;
            }
        }

        //  Create instance of a model, but doesn't really load the model from file to memory. Only remembers its definition.
        //  Data are loaded later using lazy-load mechanism - in LoadData or LoadInDraw
        //  Parameters of this constructor that are nullable aren't mandatory - that's why they are nullable.
        //  But they might be needed at some point of model's life, so think!
        //  E.g. if texture isn't specified, then it's not assigned to shader during rendering. Same for "shininess" and "specularPower"
        //  But models that use it should have null in all other texture parameters, because those textures won't be used.
        //  IMPORTANT: ASSERTS IN THIS CONSTRUCTOR SHOULD CHECK IF ALL REQUIRED PARAMETERS AND THEIR COMBINATIONS ARE FINE!
        //  BUT THE REALITY IS THAT I DON'T HAVE TIME TO ASSERT ALL POSSIBLE COMBINATIONS...                          

        public MyModel(string assetName, MyMeshDrawTechnique drawTechnique, MyModelsEnum modelEnum)
            :this(assetName, drawTechnique, modelEnum, false)
        {
        }

        /// <summary>
        /// c-tor - this constructor should be used just for max models - not voxels!
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="unloadableModel"></param>
        /// <param name="drawTechnique"></param>
        /// <param name="modelEnum"></param>
        public MyModel(string assetName, MyMeshDrawTechnique drawTechnique, MyModelsEnum modelEnum, bool keepInMemory)
        {
            m_assetName = assetName;
            m_loadedData = false;
            m_loadedContent = false;
            ModelEnum = modelEnum;
            m_drawTechnique = drawTechnique;
            KeepInMemory = keepInMemory;

            string absFilePath = Directory.GetCurrentDirectory() + "\\Content\\" + m_assetName + ".mwm";
            System.Diagnostics.Debug.Assert(File.Exists(absFilePath), "Model data for " + m_assetName + " does not exists!");

            LoadState = Managers.LoadState.Unloaded;
        }

        /// <summary>
        /// Use brain w
        /// </summary>
        /// <returns></returns>
        public List<MyMesh> GetMeshList()
        {
            return m_meshContainer;
        }


        //  Sort of lazy-load, where constructor just saves information about what this model should be, but real load is done here - and only one time.
        //  This loads only vertex data, doesn't touch GPU
        //  Can be called from main and background thread
        public void LoadData()
        {
            if (m_loadedData) return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModel::LoadData");


            MyMwcLog.WriteLine("MyModel.LoadData -> START", LoggingOptions.LOADING_MODELS);
            MyMwcLog.IncreaseIndent(LoggingOptions.LOADING_MODELS);

            MyMwcLog.WriteLine("m_assetName: " + m_assetName, LoggingOptions.LOADING_MODELS);

            //  Read data from model TAG parameter. There are stored vertex positions, triangle indices, vectors, ... everything we need.
            string absFilePath = Directory.GetCurrentDirectory() + "\\Content\\" + m_assetName + ".mwm";

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - import data");

            

            MyMwcLog.WriteLine(String.Format("Importing asset {0}, path: {1}", m_assetName, absFilePath), LoggingOptions.LOADING_MODELS);
            try
            {
                m_exporter.ImportData(absFilePath);
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine(String.Format("Importing asset failed {0}, message: {1}, stack:{2}", m_assetName, e.Message, e.StackTrace));
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - load tag data");
            Dictionary<string, object> tagData = m_exporter.GetTagData();
            if (tagData.Count == 0)
            {
                throw new Exception(String.Format("Uncompleted tagData for asset: {0}, path: {1}", m_assetName, absFilePath));
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - vertex, normals, texture coords");


#if PACKED_VERTEX_FORMAT
            HalfVector4[] vertices = (HalfVector4[])tagData[MyImporterConstants.TAG_VERTICES];

            System.Diagnostics.Debug.Assert(vertices.Length > 0);

            Byte4[] normals = (Byte4[])tagData[MyImporterConstants.TAG_NORMALS];
            m_vertices = new MyCompressedVertexNormal[vertices.Length];
            if (normals.Length > 0)
            {
                for (int v = 0; v < vertices.Length; v++)
                {
                    m_vertices[v] = new MyCompressedVertexNormal()
                    {
                        Position = vertices[v],// VF_Packer.PackPosition(ref vertices[v]),
                        Normal = normals[v]//VF_Packer.PackNormalB4(ref normals[v])
                    };
                }
            }
            else
            {
                for (int v = 0; v < vertices.Length; v++)
                {
                    m_vertices[v] = new MyCompressedVertexNormal()
                    {
                        Position = vertices[v],// VF_Packer.PackPosition(ref vertices[v]),
                    };
                }
            }
#else            
            Vector3[] vertices = (Vector3[])tagData[MyImporterConstants.TAG_VERTICES];
            Vector3[] normals = (Vector3[])tagData[MyImporterConstants.TAG_NORMALS];
            m_vertices = new MyVertexNormal[vertices.Length];
            for (int v = 0; v < vertices.Length; v++)
            {
                m_vertices[v] = new MyVertexNormal()
                {
                    Position = vertices[v],
                    Normal = normals[v]
                };
            }
#endif

            m_verticesCount = vertices.Length;

#if PACKED_VERTEX_FORMAT
            HalfVector2[] forLoadingTexCoords0 = (HalfVector2[])tagData[MyImporterConstants.TAG_TEXCOORDS0];
            m_forLoadingTexCoords0 = new HalfVector2[forLoadingTexCoords0.Length];
            for (int t = 0; t < forLoadingTexCoords0.Length; t++)
            {
                m_forLoadingTexCoords0[t] = forLoadingTexCoords0[t];// new HalfVector2(forLoadingTexCoords0[t]);
            }
#else
            Vector2[] forLoadingTexCoords0 = (Vector2[])tagData[MyImporterConstants.TAG_TEXCOORDS0];
            m_forLoadingTexCoords0 = new Vector2[forLoadingTexCoords0.Length];
            for (int t = 0; t < forLoadingTexCoords0.Length; t++)
            {
                m_forLoadingTexCoords0[t] = forLoadingTexCoords0[t];
            }
#endif

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - mesh");
            m_meshContainer.Clear();
            
            if (tagData.ContainsKey(MyImporterConstants.TAG_MESH_PARTS))
            {
                List<int> indices = new List<int>(GetVerticesCount()); // Default capacity estimation
                int maxIndex = 0;

                List<MyMeshPartInfo> meshes = tagData[MyImporterConstants.TAG_MESH_PARTS] as List<MyMeshPartInfo>;
                foreach (MyMeshPartInfo meshPart in meshes)
                {
                    MyMesh mesh = new MyMesh(meshPart, m_assetName);

                    mesh.IndexStart = indices.Count;
                    mesh.TriCount = meshPart.m_Indicies.Count / 3;

                    System.Diagnostics.Debug.Assert(mesh.TriCount > 0);

                    foreach(var i in meshPart.m_Indicies)
                    {
                        indices.Add(i);
                        if(i > maxIndex)
                        {
                            maxIndex = i;
                        }
                    }

                    m_meshContainer.Add(mesh);
                }

                if (maxIndex <= ushort.MaxValue)
                {
                    // create 16 bit indices
                    m_Indicies_16bit = new ushort[indices.Count];
                    for (int i = 0; i < indices.Count; i++)
                    {
                        m_Indicies_16bit[i] = (ushort)indices[i];
                    }
                }
                else
                {
                    // use 32bit indices
                    m_Indicies = indices.ToArray();
                }
            }

            if (tagData.ContainsKey(MyImporterConstants.TAG_MODEL_BVH))
            {
                m_bvh = new MyQuantizedBvhAdapter(tagData[MyImporterConstants.TAG_MODEL_BVH] as GImpactQuantizedBvh, this);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - other data");
            if (MyRenderConstants.RenderQualityProfile.UseNormals)
            {

#if PACKED_VERTEX_FORMAT
                Byte4[] forLoadingBinormals = (Byte4[])tagData[MyImporterConstants.TAG_BINORMALS];
                Byte4[] forLoadingTangents = (Byte4[])tagData[MyImporterConstants.TAG_TANGENTS];
                m_forLoadingBinormals = new Byte4[forLoadingBinormals.Length];
                m_forLoadingTangents = new Byte4[forLoadingBinormals.Length];

                for (int v = 0; v < forLoadingBinormals.Length; v++)
                {
                    m_forLoadingBinormals[v] = forLoadingBinormals[v];// VF_Packer.PackNormalB4(ref forLoadingBinormals[v]);
                    m_forLoadingTangents[v] = forLoadingTangents[v];//VF_Packer.PackNormalB4(ref forLoadingTangents[v]);
                }
#else
                Vector3[] forLoadingBinormals = (Vector3[])tagData[MyImporterConstants.TAG_BINORMALS];
                Vector3[] forLoadingTangents = (Vector3[])tagData[MyImporterConstants.TAG_TANGENTS];
                m_forLoadingBinormals = new Vector3[forLoadingBinormals.Length];
                m_forLoadingTangents = new Vector3[forLoadingBinormals.Length];

                for (int v = 0; v < forLoadingBinormals.Length; v++)
                {
                    m_forLoadingBinormals[v] = forLoadingBinormals[v];
                    m_forLoadingTangents[v] = forLoadingTangents[v];
                }
#endif
            }

            m_specularShininess = (float)tagData[MyImporterConstants.TAG_SPECULAR_SHININESS];
            m_specularPower = (float)tagData[MyImporterConstants.TAG_SPECULAR_POWER];
            m_rescaleFactor = (float)tagData[MyImporterConstants.TAG_RESCALE_FACTOR];

            m_useChannelTextures = (bool)tagData[MyImporterConstants.TAG_USE_CHANNEL_TEXTURES];
            if (m_useChannelTextures)
            {

#if PACKED_VERTEX_FORMAT
                HalfVector2[] forLoadingTexCoords1 = (HalfVector2[])tagData[MyImporterConstants.TAG_TEXCOORDS1];
                System.Diagnostics.Debug.Assert(forLoadingTexCoords1.Length > 0);
                m_forLoadingTexCoords1 = new HalfVector2[forLoadingTexCoords1.Length];
                for (int t = 0; t < forLoadingTexCoords1.Length; t++)
                {
                    m_forLoadingTexCoords1[t] = forLoadingTexCoords1[t];// new HalfVector2(forLoadingTexCoords1[t]);
                }
#else
                Vector2[] forLoadingTexCoords1 = (Vector2[])tagData[MyImporterConstants.TAG_TEXCOORDS1];
                System.Diagnostics.Debug.Assert(forLoadingTexCoords1.Length > 0);
                m_forLoadingTexCoords1 = new Vector2[forLoadingTexCoords1.Length];
                for (int t = 0; t < forLoadingTexCoords1.Length; t++)
                {
                    m_forLoadingTexCoords1[t] = forLoadingTexCoords1[t];
                }
#endif
            }

            BoundingBox = (BoundingBox)tagData[MyImporterConstants.TAG_BOUNDING_BOX];
            BoundingSphere = (BoundingSphere)tagData[MyImporterConstants.TAG_BOUNDING_SPHERE];
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
            BoundingBoxSizeHalf = BoundingBoxSize / 2.0f;
            Dummies = tagData[MyImporterConstants.TAG_DUMMIES] as Dictionary<string, MyModelDummy>;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            foreach (MyMesh mesh in m_meshContainer)
            {
                for (int i = 0; i < mesh.Materials.Length; i++)
                {
                    MyMeshMaterial material = mesh.Materials[i];

                    if (Dummies.ContainsKey(material.MaterialName))
                    {
                        MyModelDummy dummy = Dummies[material.MaterialName];

                        material.DiffuseUVAnim = new Vector2(
                            MyUtils.ReadSingleSafe((string)dummy.CustomData["DiffuseU"]),
                            MyUtils.ReadSingleSafe((string)dummy.CustomData["DiffuseV"])
                            );

                        material.EmissiveUVAnim = new Vector2(
                            MyUtils.ReadSingleSafe((string)dummy.CustomData["EmissiveU"]),
                            MyUtils.ReadSingleSafe((string)dummy.CustomData["EmissiveV"])
                            );
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - copy triangle indices");
            //  Prepare data
            CopyTriangleIndices();
            m_trianglesCount = Triangles.Count();

            //m_effect = MyModels.GetEffectForDrawTechnique(m_drawTechnique);

            //  Remember this numbers as list may be cleared at the end of this method
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.WriteLine("Triangles.Length: " + Triangles.Length, LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("Vertexes.Length: " + GetVerticesCount(), LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("Centered: " + (bool)tagData[MyImporterConstants.TAG_CENTERED], LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("UseChannelTextures: " + (bool)tagData[MyImporterConstants.TAG_USE_CHANNEL_TEXTURES], LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("Length in meters: " + (float)tagData[MyImporterConstants.TAG_LENGTH_IN_METERS], LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("Rescale to length in meters?: " + (bool)tagData[MyImporterConstants.TAG_RESCALE_TO_LENGTH_IN_METERS], LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("SpecularShininess: " + m_specularShininess, LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("SpecularPower: " + m_specularPower, LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("RescaleFactor: " + m_rescaleFactor, LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("BoundingBox: " + BoundingBox, LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("BoundingSphere: " + BoundingSphere, LoggingOptions.LOADING_MODELS);

            MyPerformanceCounter.PerAppLifetime.MyModelsCount++;
            MyPerformanceCounter.PerAppLifetime.MyModelsMeshesCount += m_meshContainer.Count;
            MyPerformanceCounter.PerAppLifetime.MyModelsVertexesCount += GetVerticesCount();
            MyPerformanceCounter.PerAppLifetime.MyModelsTrianglesCount += Triangles.Length;

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Model - load data - octree");
            ////  Create octree for fast line intersections. 
            ////  IMPORTANT: This can be calculated only after we know bounding box and vertex/triangleVertexes arrays are filled
            //m_octree = new MyModelOctree(this);
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            // Preload octree & bvh
            //var before = System.GC.GetTotalMemory(true);
            //DateTime beforeTime = DateTime.Now;
            //GetOctree();
            //MyPerformanceCounter.PerAppLifetime.MyModelsOctreeLoadTime += (DateTime.Now - beforeTime);
            //var after = System.GC.GetTotalMemory(true);
            //MyPerformanceCounter.PerAppLifetime.MyModelsOctreeSizeB += (after - before);

            //var before2 = System.GC.GetTotalMemory(true);
            //DateTime beforeTime2 = DateTime.Now;
            //GetBvh();
            //MyPerformanceCounter.PerAppLifetime.MyModelsBvhLoadTime += (DateTime.Now - beforeTime2);
            //var after2 = System.GC.GetTotalMemory(true);
            //MyPerformanceCounter.PerAppLifetime.MyModelsBvhSizeB += (after2 - before2);

            //tempContentManagerForModelOnly.Unload();

            ModelInfo = new MyModelInfo(GetTrianglesCount(), GetVerticesCount(), BoundingBoxSize);

            m_loadedData = true;

            MyMwcLog.DecreaseIndent(LoggingOptions.LOADING_MODELS);
            MyMwcLog.WriteLine("MyModel.LoadData -> END", LoggingOptions.LOADING_MODELS);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        //
        //
        public void LoadContent()
        {
            //If model was loaded previously, we need it reload because it has some temporary data discarded
            //otherwise model wont load after device reset
            // We must also reload when binormals or tangents are missing.
            /*
            if (m_loadedData && (m_forLoadingTexCoords0 == null || m_forLoadingBinormals == null || m_forLoadingTangents == null || m_meshContainer.Count == 0))
            {
                m_loadedData = false;
                LoadData();
            }  */
            
            LoadInDraw();


        }



        private void CreateRenderDataForMesh()
        {
            // Creating vertex and index buffer
            //  Write to GPU
            CreateVertexBuffer();
            CreateIndexBuffer();

            //  We don't need this anymore - we need it for merging geometry
            //  Cleaning up lists          
            /*
            m_forLoadingTexCoords0 = null;
            m_forLoadingTexCoords1 = null;
            m_forLoadingBinormals = null;
            m_forLoadingTangents = null; 
            */

            // Cleaning normals if we do not need vertex normal debug drawing
           // if (!MyMwcFinalBuildConstants.ENABLE_VERTEX_NORMALS_DEBUG_DRAW)
             //   m_VertexNormals = null;
        }

        //  Loads vertex/index buffers and textures, access GPU
        //  Should be called only from Draw method on main thread
        public void LoadInDraw(LoadingMode loadingMode = LoadingMode.Immediate)
        {
            //  If already loaded into GPU
            if (m_loadedContent) return;

            //  If this model wasn't loaded through lazy-load then it means we don't need it in this game/sector, and we
            //  don't need to load him into GPU
            if (m_loadedData == false) return;

            if (LoadState == Managers.LoadState.Loading)
                return;

            if (loadingMode == LoadingMode.Background)
            {
                MyModels.LoadModelInDrawInBackground(this);
                return;
            }
            

            Debug.Assert(m_forLoadingTexCoords0 != null && m_meshContainer.Count != 0, "Somebody forget to call LoadData on model before rendering");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModel::LoadInDraw");

            if (m_useChannelTextures)
            {
                MaskTexture = MyTextureManager.GetTexture<MyTexture2D>(m_assetName + C_POSTFIX_MASK, null, LoadingMode.LazyBackground);
            }



            // Creating
            CreateRenderDataForMesh();


            m_loadedContent = true;

            LoadState = Managers.LoadState.Loaded;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void CreateVertexBuffer()
        {
            //  Create vertex buffer - vertex format type depends on draw technique

            switch (m_drawTechnique)
            {
                case MyMeshDrawTechnique.MESH:
                case MyMeshDrawTechnique.DECAL:
                case MyMeshDrawTechnique.HOLO:
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        if (m_forLoadingTexCoords0 == null) throw new Exception("Model '" + m_assetName + "' doesn't have texture channel 0 specified, but this shader requires it");

                        if (m_forLoadingTexCoords0.Length == 0)
                        {
                            MyVertexFormatPosition[] vertexArray = new MyVertexFormatPosition[GetVerticesCount()];
                            for (int i = 0; i < GetVerticesCount(); i++)
                            {
                                Vector4 pos = m_vertices[i].Position.ToVector4();
                                vertexArray[i].Position = new Vector3(pos.X, pos.Y, pos.Z);
                            }

                            m_vertexDeclaration = MyVertexFormatPosition.VertexDeclaration;
                            m_vertexStride = MyVertexFormatPosition.Stride;
                            m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                            m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                            m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                            m_vertexBuffer.Unlock();
                            m_vertexBuffer.Tag = this;
                        }
                        else
                        if (MyRenderConstants.RenderQualityProfile.UseNormals)
                        {
                            if (m_forLoadingBinormals == null) 
                                throw new Exception("Model '" + m_assetName + "' doesn't have binormals calculated, but this shader requires them");
                            if (m_forLoadingTangents == null) 
                                throw new Exception("Model '" + m_assetName + "' doesn't have tangent vectors calculated, but this shader requires them");

                            if (UseChannels)
                            {
                                if (m_forLoadingTexCoords1 == null || m_forLoadingTexCoords1.Length == 0)
                                {
                                    throw new Exception("Model '" + m_assetName + "' doesn't have UVcoords1 calculated, but this shader requires them");
                                }

                                MyVertexFormatPositionNormalTextureTangentBinormalMask[] vertexArray = new MyVertexFormatPositionNormalTextureTangentBinormalMask[GetVerticesCount()];
                                for (int i = 0; i < GetVerticesCount(); i++)
                                {
#if PACKED_VERTEX_FORMAT
                                    vertexArray[i].PositionPacked = m_vertices[i].Position;
                                    vertexArray[i].NormalPacked = m_vertices[i].Normal;
                                    vertexArray[i].TexCoordPacked = m_forLoadingTexCoords0[i];
                                    vertexArray[i].BinormalPacked = m_forLoadingBinormals[i];
                                    vertexArray[i].TangentPacked = m_forLoadingTangents[i];
                                    vertexArray[i].MaskCoordPacked = m_forLoadingTexCoords1[i];
#else
                                    vertexArray[i].Position = m_vertices[i].Position;
                                    vertexArray[i].Normal = m_vertices[i].Normal;
                                    vertexArray[i].TexCoord = m_forLoadingTexCoords0[i];
                                    vertexArray[i].Binormal = m_forLoadingBinormals[i];
                                    vertexArray[i].Tangent = m_forLoadingTangents[i];
                                    vertexArray[i].MaskCoord = m_forLoadingTexCoords1[i];

#endif
                                }

                                m_vertexDeclaration = MyVertexFormatPositionNormalTextureTangentBinormalMask.VertexDeclaration;
                                m_vertexStride = MyVertexFormatPositionNormalTextureTangentBinormalMask.Stride;
                                m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                                m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                                m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                                m_vertexBuffer.Unlock();
                                m_vertexBuffer.Tag = this;
                            }
                            else
                            {
                                MyVertexFormatPositionNormalTextureTangentBinormal[] vertexArray = new MyVertexFormatPositionNormalTextureTangentBinormal[GetVerticesCount()];
                                for (int i = 0; i < GetVerticesCount(); i++)
                                {
#if PACKED_VERTEX_FORMAT
                                    vertexArray[i].PositionPacked = m_vertices[i].Position;
                                    vertexArray[i].NormalPacked = m_vertices[i].Normal;
                                    vertexArray[i].TexCoordPacked = m_forLoadingTexCoords0[i];
                                    vertexArray[i].BinormalPacked = m_forLoadingBinormals[i];
                                    vertexArray[i].TangentPacked = m_forLoadingTangents[i];
#else
                                    vertexArray[i].Position = m_vertices[i].Position;
                                    vertexArray[i].Normal = m_vertices[i].Normal;
                                    vertexArray[i].TexCoord = m_forLoadingTexCoords0[i];
                                    vertexArray[i].Binormal = m_forLoadingBinormals[i];
                                    vertexArray[i].Tangent = m_forLoadingTangents[i];
#endif
                                }

                                m_vertexDeclaration = MyVertexFormatPositionNormalTextureTangentBinormal.VertexDeclaration;
                                m_vertexStride = MyVertexFormatPositionNormalTextureTangentBinormal.Stride;
                                m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                                m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                                m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                                m_vertexBuffer.Unlock();
                                m_vertexBuffer.Tag = this;
                            }
                        }
                        else
                        {
                            if (UseChannels)
                            {
                                MyVertexFormatPositionNormalTextureMask[] vertexArray = new MyVertexFormatPositionNormalTextureMask[GetVerticesCount()];
                                for (int i = 0; i < GetVerticesCount(); i++)
                                {
#if PACKED_VERTEX_FORMAT
                                    vertexArray[i].PositionPacked = m_vertices[i].Position;
                                    vertexArray[i].NormalPacked = m_vertices[i].Normal;
                                    vertexArray[i].TexCoordPacked = m_forLoadingTexCoords0[i];
                                    vertexArray[i].MaskCoordPacked = m_forLoadingTexCoords1[i];
#else
                                    vertexArray[i].Position = m_vertices[i].Position;
                                    vertexArray[i].Normal = m_vertices[i].Normal;
                                    vertexArray[i].TexCoord = m_forLoadingTexCoords0[i];
                                    vertexArray[i].MaskCoord = m_forLoadingTexCoords1[i];
#endif
                                }

                                m_vertexDeclaration = MyVertexFormatPositionNormalTextureMask.VertexDeclaration;
                                m_vertexStride = MyVertexFormatPositionNormalTextureMask.Stride;
                                m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                                m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                                m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                                m_vertexBuffer.Unlock();
                                m_vertexBuffer.Tag = this;
                            }
                            else
                            {
                                MyVertexFormatPositionNormalTexture[] vertexArray = new MyVertexFormatPositionNormalTexture[GetVerticesCount()];
                                for (int i = 0; i < GetVerticesCount(); i++)
                                {
                                    vertexArray[i].Position = GetVertexInt(i);
                                    vertexArray[i].Normal = GetVertexNormal(i);
#if PACKED_VERTEX_FORMAT
                                    vertexArray[i].TexCoord = m_forLoadingTexCoords0[i].ToVector2();
#else
                                    vertexArray[i].TexCoord = m_forLoadingTexCoords0[i];
#endif
                                }

                                m_vertexDeclaration = MyVertexFormatPositionNormalTexture.VertexDeclaration;
                                m_vertexStride = MyVertexFormatPositionNormalTexture.Stride;
                                m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                                m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly , VertexFormat.None, Pool.Default);
                                m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                                m_vertexBuffer.Unlock();
                                m_vertexBuffer.Tag = this;
                            }
                        }
                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        MyVertexFormatPositionNormal[] vertexArray = new MyVertexFormatPositionNormal[GetVerticesCount()];
                        for (int i = 0; i < GetVerticesCount(); i++)
                        {
                            vertexArray[i].Position = GetVertexInt(i);
                            vertexArray[i].Normal = GetVertexNormal(i);
                        }

                        m_vertexDeclaration = MyVertexFormatPositionNormal.VertexDeclaration;
                        m_vertexStride = MyVertexFormatPositionNormal.Stride;
                        m_vertexBufferSize = vertexArray.Length * m_vertexStride;
                        m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, m_vertexBufferSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                        m_vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(vertexArray);
                        m_vertexBuffer.Unlock();
                        m_vertexBuffer.Tag = this;
                    }
                    break;
                default:
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
            }

            MyPerformanceCounter.PerAppLifetime.ModelVertexBuffersSize += m_vertexBufferSize;

            SignResource(m_vertexBuffer);
        }

        void CreateIndexBuffer()
        {
            MyCommonDebugUtils.AssertDebug(m_indexBuffer == null);

            if (m_Indicies != null)
            {
                m_indexBuffer = new IndexBuffer(MyMinerGame.Static.GraphicsDevice, m_Indicies.Length * sizeof(int), Usage.WriteOnly, Pool.Default, false);
                m_indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(m_Indicies);
                m_indexBuffer.Unlock();
                m_indexBuffer.Tag = this;
                m_indexBufferSize = m_Indicies.Length * sizeof(int);
            }
            else if (m_Indicies_16bit != null)
            {
                m_indexBuffer = new IndexBuffer(MyMinerGame.Static.GraphicsDevice, m_Indicies_16bit.Length * sizeof(short), Usage.WriteOnly, Pool.Default, true);
                m_indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(m_Indicies_16bit);
                m_indexBuffer.Unlock();
                m_indexBuffer.Tag = this;
                m_indexBufferSize = m_Indicies_16bit.Length * sizeof(short);
            }

            MyPerformanceCounter.PerAppLifetime.ModelIndexBuffersSize += m_indexBufferSize;

            SignResource(m_indexBuffer);
        }

        /// <summary>
        /// Signs the resource.
        /// </summary>
        /// <param name="indexBuffer">The index buffer.</param>
        [Conditional("DEBUG")]
        private void SignResource(IndexBuffer indexBuffer)
        {
            indexBuffer.DebugName = m_assetName + "_ib";
        }

        bool IsMeshGoingToBeAddedIntoTrianglesArray(MyMesh mesh)
        {
            return MyMeshPartInfo.IsPhysical(mesh.Materials[0].DrawTechnique);
        }

        int GetNumberOfTrianglesForColDet()
        {
            int trianglesCount = 0;
            foreach (MyMesh mesh in m_meshContainer)
            {
                if (IsMeshGoingToBeAddedIntoTrianglesArray(mesh))
                {
                    trianglesCount += mesh.TriCount;
                }
            }
            return trianglesCount;
        }

        void CopyTriangleIndices()
        {
            Triangles = new MyTriangleVertexIndices[GetNumberOfTrianglesForColDet()];
            int triangleIndex = 0;

            foreach (MyMesh mesh in m_meshContainer)
            {
                if (IsMeshGoingToBeAddedIntoTrianglesArray(mesh))
                {
                    if (m_Indicies != null)
                    {
                        for (int i = 0; i < mesh.TriCount; i++)
                        {
                            //  Notice we swap indices. It's because XNA's clock-wise rule probably differs from FBX's, and JigLib needs it in this order.
                            //  But because of this, I did similar swaping in my col/det functions
                            Triangles[triangleIndex] = new MyTriangleVertexIndices(m_Indicies[mesh.IndexStart + i * 3 + 0], m_Indicies[mesh.IndexStart + i * 3 + 2], m_Indicies[mesh.IndexStart + i * 3 + 1]);
                            triangleIndex++;
                        }
                    }
                    else if (m_Indicies_16bit != null)
                    {
                        for (int i = 0; i < mesh.TriCount; i++)
                        {
                            //  Notice we swap indices. It's because XNA's clock-wise rule probably differs from FBX's, and JigLib needs it in this order.
                            //  But because of this, I did similar swaping in my col/det functions
                            Triangles[triangleIndex] = new MyTriangleVertexIndices(m_Indicies_16bit[mesh.IndexStart + i * 3 + 0], m_Indicies_16bit[mesh.IndexStart + i * 3 + 2], m_Indicies_16bit[mesh.IndexStart + i * 3 + 1]);
                            triangleIndex++;
                        }
                    }
                    else throw new MyMwcExceptionApplicationShouldNotGetHere(); // Neither 32bit or 16bit indices are set, probably already called mesh.DisposeIndices()
                }
            }

            // No need to store indices anymore
            //m_Indicies = null;
            //m_Indicies_16bit = null;

            //  Validate this new array, if size is correct and if all indices are OK
            MyCommonDebugUtils.AssertDebug(triangleIndex == Triangles.Length);
            foreach (MyTriangleVertexIndices triangle in Triangles)
            {
                MyCommonDebugUtils.AssertDebug(triangle.I0 != triangle.I1);
                MyCommonDebugUtils.AssertDebug(triangle.I1 != triangle.I2);
                MyCommonDebugUtils.AssertDebug(triangle.I2 != triangle.I0);
                MyCommonDebugUtils.AssertDebug((triangle.I0 >= 0) && (triangle.I0 < GetVerticesCount()));
                MyCommonDebugUtils.AssertDebug((triangle.I1 >= 0) && (triangle.I1 < GetVerticesCount()));
                MyCommonDebugUtils.AssertDebug((triangle.I2 >= 0) && (triangle.I2 < GetVerticesCount()));
            }
        }


        private void UnloadTemporaryData()
        {
            m_forLoadingTexCoords0 = null;
            m_forLoadingTexCoords1 = null;
            m_forLoadingBinormals = null;
            m_forLoadingTangents = null;
        }

        public bool UnloadData()
        {
            if (m_loadedContent)
                UnloadContent();

            UnloadTemporaryData();

            bool res = m_loadedData;
            m_loadedData = false;
            if (m_bvh != null)
            {
                m_bvh.Close();
                m_bvh = null;
            }
            LoadState = Managers.LoadState.Unloaded;

            MyPerformanceCounter.PerAppLifetime.MyModelsMeshesCount -= m_meshContainer.Count;
            if (m_vertices != null)
                MyPerformanceCounter.PerAppLifetime.MyModelsVertexesCount -= GetVerticesCount();
            if (Triangles != null)
                MyPerformanceCounter.PerAppLifetime.MyModelsTrianglesCount -= Triangles.Length;
            if (res)
                MyPerformanceCounter.PerAppLifetime.MyModelsCount--;

            m_vertices = null;
            Triangles = null;
            m_meshContainer.Clear();
            m_Indicies_16bit = null;
            m_Indicies = null;

            return res;
        }

        public void UnloadContent()
        {
            //MyPerformanceCounter.PerAppLifetime.MyModelsCount--;

            if (m_vertexBuffer != null)
            {
                m_vertexBuffer.Dispose();
                m_vertexBuffer = null;
                MyPerformanceCounter.PerAppLifetime.ModelVertexBuffersSize -= m_vertexBufferSize;
                m_vertexBufferSize = 0;
            }

            if (m_indexBuffer != null)
            {
                m_indexBuffer.Dispose();
                m_indexBuffer = null;
                MyPerformanceCounter.PerAppLifetime.ModelIndexBuffersSize -= m_indexBufferSize;
                m_indexBufferSize = 0;
            }

            LoadState = Managers.LoadState.Unloaded;

            //foreach (MyMesh mesh in m_meshContainer)
            //{
            //    mesh.DisposeBuffers();  //index
            //}

            //m_meshContainer.Clear();// added for LoadContent purpose this will cause whole mesh to reload

            m_loadedContent = false;
        }

        public MyModelsEnum GetModelEnum()
        {
            return ModelEnum;
        }

        public IMyTriangePruningStructure GetTrianglePruningStructure()
        {
            Debug.Assert(m_bvh != null, "BVH should be loaded from content processor");
            return m_bvh;
        }

        public void GetTriangleBoundingBox(int triangleIndex, ref BoundingBox boundingBox)
        {
            boundingBox = BoundingBoxHelper.InitialBox;
            Vector3 v1, v2, v3;
            GetVertex(Triangles[triangleIndex].I0, Triangles[triangleIndex].I1, Triangles[triangleIndex].I2, out v1, out v2, out v3);

            BoundingBoxHelper.AddTriangle(ref boundingBox, 
                v1, 
                v2, 
                v3);
        }

        public int GetTrianglesCount()
        {
            return m_trianglesCount;
        }

        public int GetVerticesCount()
        {
            return m_verticesCount;
        }

        public int GetBVHSize()
        {
            return m_bvh != null ? m_bvh.Size : 0;
        }

        public MyMeshDrawTechnique GetDrawTechnique()
        {
            return m_drawTechnique;
        }

        public void SetDrawTechnique(MyMeshDrawTechnique drawTechnique)
        {
            m_drawTechnique = drawTechnique;
        }

        public float GetSpecularShininess()
        {
            return m_specularShininess;
        }

        public float GetSpecularPower()
        {
            return m_specularPower;
        }

        public float GetRescaleFactor()
        {
            return m_rescaleFactor;
        }
              

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="effect"></param>
        public void Render()
        {
            Device device = MyMinerGame.Static.GraphicsDevice;
            device.SetStreamSource(0, m_vertexBuffer, 0, m_vertexStride);
            device.Indices = m_indexBuffer;
            device.VertexDeclaration = GetVertexDeclaration();

            foreach (MyMesh mesh in m_meshContainer)
            {
                mesh.Render(device, m_verticesCount);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_meshContainer.Clear();
            m_vertexBuffer.Dispose();
            m_vertexBuffer = null;
            MyPerformanceCounter.PerAppLifetime.ModelVertexBuffersSize -= m_vertexBufferSize;
            m_vertexBufferSize = 0;

            m_indexBuffer.Dispose();
            m_indexBuffer = null;
            MyPerformanceCounter.PerAppLifetime.ModelIndexBuffersSize -= m_indexBufferSize;
            m_indexBufferSize = 0;
        }

        /// <summary>
        /// File path of the model
        /// </summary>
        internal string AssetName
        {
            get { return m_assetName; }
        }

        internal VertexBuffer VertexBuffer
        {
            get { return m_vertexBuffer; }
        }

        internal IndexBuffer IndexBuffer
        {
            get { return m_indexBuffer; }
        }

        /// <summary>
        /// Signs the resource.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        [Conditional("DEBUG")]
        private void SignResource(VertexBuffer vertexBuffer)
        {
            vertexBuffer.DebugName = m_assetName + "_vb";
        }

        //  Load only snappoints.
        public void LoadOnlyDummies()
        {
            if (!m_loadedData)
            {
                MyMwcLog.WriteLine("MyModel.LoadSnapPoints -> START", LoggingOptions.LOADING_MODELS);
                MyMwcLog.IncreaseIndent(LoggingOptions.LOADING_MODELS);

                MyMwcLog.WriteLine("m_assetName: " + m_assetName, LoggingOptions.LOADING_MODELS);

                //  Read data from model TAG parameter. There are stored vertex positions, triangle indices, vectors, ... everything we need.
                string absFilePath = Directory.GetCurrentDirectory() + "\\Content\\" + m_assetName + ".mwm";

                MyModelExporter exporter = new MyModelExporter();

                MyMwcLog.WriteLine(String.Format("Importing asset {0}, path: {1}", m_assetName, absFilePath), LoggingOptions.LOADING_MODELS);

                // Read only TAG_DUMMIES data
                SortedSet<string> tags = new SortedSet<string>();
                tags.Add(MyImporterConstants.TAG_DUMMIES);

                try
                {
                    exporter.ImportCustomData(absFilePath, tags);
                }
                catch (Exception e)
                {
                    MyMwcLog.WriteLine(String.Format("Importing asset failed {0}, message: {1}, stack:{2}", m_assetName, e.Message, e.StackTrace));
                }

                Dictionary<string, object> tagData = exporter.GetTagData();
                //if (tagData.Count == 0)
                //{
                //    throw new Exception(String.Format("Uncompleted tagData for asset: {0}, path: {1}", m_assetName, absFilePath));
                //}

                //Dummies = tagData[MyImporterConstants.TAG_DUMMIES] as Dictionary<string, MyModelDummy>;
                if (tagData.Count > 0)
                {
                    Dummies = tagData[MyImporterConstants.TAG_DUMMIES] as Dictionary<string, MyModelDummy>;
                }
                else
                {
                    Dummies = new Dictionary<string, MyModelDummy>();
                }
            }
        }

        //  Load only snappoints.
        public void LoadOnlyModelInfo()
        {
            if (!m_loadedData)
            {
                MyMwcLog.WriteLine("MyModel.LoadModelData -> START", LoggingOptions.LOADING_MODELS);
                MyMwcLog.IncreaseIndent(LoggingOptions.LOADING_MODELS);

                MyMwcLog.WriteLine("m_assetName: " + m_assetName, LoggingOptions.LOADING_MODELS);

                //  Read data from model TAG parameter. There are stored vertex positions, triangle indices, vectors, ... everything we need.
                string absFilePath = Directory.GetCurrentDirectory() + "\\Content\\" + m_assetName + ".mwm";

                MyModelExporter exporter = new MyModelExporter();

                MyMwcLog.WriteLine(String.Format("Importing asset {0}, path: {1}", m_assetName, absFilePath), LoggingOptions.LOADING_MODELS);

                // Read only TAG_DUMMIES data
                SortedSet<string> tags = new SortedSet<string>();
                tags.Add(MyImporterConstants.TAG_MODEL_INFO);

                try
                {
                    exporter.ImportCustomData(absFilePath, tags);
                }
                catch (Exception e)
                {
                    MyMwcLog.WriteLine(String.Format("Importing asset failed {0}, message: {1}, stack:{2}", m_assetName, e.Message, e.StackTrace));
                }

                Dictionary<string, object> tagData = exporter.GetTagData();

                if (tagData.Count > 0)
                {
                    ModelInfo = tagData[MyImporterConstants.TAG_MODEL_INFO] as MyModelInfo;
                }
                else
                {
                    ModelInfo = new MyModelInfo(0, 0, Vector3.Zero);
                }
            }
        }

        void IPrimitiveManagerBase.Cleanup()
        {
            //throw new NotImplementedException();
        }

        bool IPrimitiveManagerBase.IsTrimesh()
        {
            return true;
            //throw new NotImplementedException();
        }

        int IPrimitiveManagerBase.GetPrimitiveCount()
        {
            return this.m_trianglesCount;
            //throw new NotImplementedException();
        }

        void IPrimitiveManagerBase.GetPrimitiveBox(int prim_index, out AABB primbox)
        {
            BoundingBox bbox = BoundingBoxHelper.InitialBox;
            Vector3 v1 = GetVertex(Triangles[prim_index].I0);
            Vector3 v2 = GetVertex(Triangles[prim_index].I1);
            Vector3 v3 = GetVertex(Triangles[prim_index].I2);
            BoundingBoxHelper.AddTriangle(ref bbox,
                ref v1,
                ref v2,
                ref v3);

            primbox = new AABB() { m_min = bbox.Min, m_max = bbox.Max };
        }

        void IPrimitiveManagerBase.GetPrimitiveTriangle(int prim_index, PrimitiveTriangle triangle)
        {
            triangle.m_vertices[0] = GetVertex(Triangles[prim_index].I0);
            triangle.m_vertices[1] = GetVertex(Triangles[prim_index].I1);
            triangle.m_vertices[2] = GetVertex(Triangles[prim_index].I2);
        }

        public void PreloadTextures(LoadingMode loadingMode, int materialIndex = -1)
        {
            foreach (MyMesh mesh in GetMeshList())
            {
                if (materialIndex == -1)
                    mesh.Materials[0].PreloadTexture(loadingMode);
                else
                    mesh.Materials[materialIndex].PreloadTexture(loadingMode);
            }
        }        
    }
}

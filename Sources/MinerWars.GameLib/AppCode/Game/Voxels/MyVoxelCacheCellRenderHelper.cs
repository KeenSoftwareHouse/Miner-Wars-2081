using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWarsMath;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using System;

namespace MinerWars.AppCode.Game.Voxels
{
    class MySingleMaterialHelper
    {
        //  Here we store calculated vertexes (before we send them to vertex buffer) - for single-material triangles
        public MyVertexFormatVoxelSingleMaterial[] Vertices;
        public int VertexCount;
        public short[] Indices;
        public int IndexCount;

        public MyMwcVoxelMaterialsEnum Material;

        //  This pre-initializes this object when preallocating, not when allocating object for specific purpose
        public void LoadData()
        {
            Vertices = new MyVertexFormatVoxelSingleMaterial[MyVoxelCacheCellRenderHelper.MAX_VERTICES_COUNT];
            Indices = new short[MyVoxelCacheCellRenderHelper.MAX_INDICES_COUNT];
        }

        public void UnloadData()
        {
            Vertices = null;
            Indices = null;
        }

        //  This really starts/initializes this object
        public void SetMaterial(MyMwcVoxelMaterialsEnum material)
        {
            Material = material;
        }
    }

    class MyMultiMaterialHelper
    {
        public MyVertexFormatVoxelSingleMaterial[] Vertices;
        public int VertexCount;

        public MyMwcVoxelMaterialsEnum Material0;
        public MyMwcVoxelMaterialsEnum Material1;
        public MyMwcVoxelMaterialsEnum Material2;

        public void LoadData()
        {
            Vertices = new MyVertexFormatVoxelSingleMaterial[MyVoxelCacheCellRenderHelper.MAX_VERTICES_COUNT];
        }

        public void UnloadData()
        {
            Vertices = null;
        }

        public void AddVertex(ref MyVoxelVertex vertex)//Vector3 pos, Vector3 normal, MyMwcVoxelMaterialsEnum material, float ambient)
        {
            var material = vertex.Material;
            byte alphaIndex;
            if (Material0 == material)
                alphaIndex = 0;
            else if (Material1 == material)
                alphaIndex = 1;
            else if (Material2 == material)
                alphaIndex = 2;
            else
                throw new System.InvalidOperationException("Should not be there, invalid material");

            Vertices[VertexCount].m_positionAndAmbient = vertex.m_positionAndAmbient;
            Vertices[VertexCount].Ambient = vertex.Ambient;

#if PACKED_VERTEX_FORMAT
            Vertices[VertexCount].m_normal = vertex.m_normal;
#else
            Vertices[VertexCount].Normal = vertex.Normal;
#endif

            Vertices[VertexCount].MaterialAlphaIndex = alphaIndex;
            VertexCount++;
        }

        private bool HasMaterial(MyMwcVoxelMaterialsEnum material)
        {
            if(material == Material0 || material == Material1 || material == Material2)
            {
                return true;
            }
            return false;
        }

        public bool MatchMaterials(MyMwcVoxelMaterialsEnum material0, MyMwcVoxelMaterialsEnum material1, MyMwcVoxelMaterialsEnum material2)
        {
            return HasMaterial(material0) && HasMaterial(material1) && HasMaterial(material2);
        }

        public void SetMaterials(MyMwcVoxelMaterialsEnum mat0, MyMwcVoxelMaterialsEnum mat1, MyMwcVoxelMaterialsEnum mat2)
        {
            Material0 = mat0;
            Material1 = mat1;
            Material2 = mat2;
        }
    }

    static class MyVoxelCacheCellRenderHelper
    {
        public struct MySingleMaterialIndexLookup
        {
            public short VertexIndex;             //  If this vertex is in the list, this is its m_notCompressedIndex 
            public int CalcCounter;               //  For knowing if vertex was calculated in this Begin/End or one of previous (or in this batch!!!)
        }

        public const int MAX_VERTICES_COUNT = short.MaxValue;           //  Max number of vertexes we can hold in vertex buffer (because we support only 16-bit m_notCompressedIndex buffer)
        public const int MAX_INDICES_COUNT = 100000;                    //  Max number of indices we can hold in m_notCompressedIndex buffer (because we don't want to have too huge helper arrays). This number doesn't relate to 16-bit indices.
        public const int MAX_VERTICES_COUNT_STOP = MAX_VERTICES_COUNT - 3;
        public const int MAX_INDICES_COUNT_STOP = MAX_INDICES_COUNT - 3;

        //public static bool[] FinishedSingleMaterials = new bool[MyVoxelMaterials.GetMaterialsCount()];
        //public static Dictionary<int, bool> FinishedMultiMaterials = new Dictionary<int, bool>(MyVoxelConstants.DEFAULT_MULTIMATERIAL_CACHE_SIZE);

        //static MySingleMaterialHelper m_singleMaterialHelper;
        //static MyMultiMaterialHelper m_multiMaterialHelper;

        //  For creating corect indices
        public static MySingleMaterialIndexLookup[][] SingleMaterialIndicesLookup = new MySingleMaterialIndexLookup[MyVoxelMaterials.GetMaterialsCount()][];
        public static int[] SingleMaterialIndicesLookupCount = new int[MyVoxelMaterials.GetMaterialsCount()];

        private static MySingleMaterialHelper[] m_preallocatedSingleMaterialHelpers;
        private static Dictionary<int, MyMultiMaterialHelper> m_preallocatedMultiMaterialHelpers;

        static MyVoxelCacheCellRenderHelper()
        {
            for (int i = 0; i < SingleMaterialIndicesLookup.Length; i++)
            {
                SingleMaterialIndicesLookup[i] = new MySingleMaterialIndexLookup[MyVoxelCacheCellRenderHelper.MAX_VERTICES_COUNT];
            }
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelCacheCellRenderHelper.LoadData");

            MyMwcLog.WriteLine("MyVoxelCacheCellRenderHelper.LoadData - START");
            MyMwcLog.IncreaseIndent();

            /*
            if (m_singleMaterialHelper == null)
            {
                m_singleMaterialHelper = new MySingleMaterialHelper();
                m_singleMaterialHelper.LoadData();

                m_multiMaterialHelper = new MyMultiMaterialHelper();
                m_multiMaterialHelper.LoadData();
            } */

            if (m_preallocatedSingleMaterialHelpers == null)
            {
                m_preallocatedSingleMaterialHelpers = new MySingleMaterialHelper[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];
                m_preallocatedMultiMaterialHelpers = new Dictionary<int, MyMultiMaterialHelper>();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelCacheCellRenderHelper.LoadData - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            foreach (var pair in m_preallocatedMultiMaterialHelpers)
            {
                pair.Value.UnloadData();
            }
            m_preallocatedMultiMaterialHelpers.Clear();

            for (int i = 0; i < m_preallocatedSingleMaterialHelpers.Length; i++)
            {
                if (m_preallocatedSingleMaterialHelpers[i] != null)
                {
                    m_preallocatedSingleMaterialHelpers[i].UnloadData();
                    m_preallocatedSingleMaterialHelpers[i] = null;
                }
            }
        }

        public static MySingleMaterialHelper GetForMaterial(MyMwcVoxelMaterialsEnum material)
        {
            if (m_preallocatedSingleMaterialHelpers[(int)material] == null)
            {
                m_preallocatedSingleMaterialHelpers[(int)material] = new MySingleMaterialHelper();
                m_preallocatedSingleMaterialHelpers[(int)material].LoadData();
                m_preallocatedSingleMaterialHelpers[(int)material].SetMaterial(material);
            }

            return m_preallocatedSingleMaterialHelpers[(int)material];
            //m_singleMaterialHelper.SetMaterial(material);
            //return m_singleMaterialHelper;
        }

        public static MyMultiMaterialHelper GetForMultimaterial(MyMwcVoxelMaterialsEnum material0, MyMwcVoxelMaterialsEnum material1, MyMwcVoxelMaterialsEnum material2)
        {
            int id = MyVoxelCacheCellRender.GetMultimaterialId(material0, material1, material2);
            MyMultiMaterialHelper helper = null;
            m_preallocatedMultiMaterialHelpers.TryGetValue(id, out helper);
            if (helper == null)
            {
                helper = new MyMultiMaterialHelper();
                helper.LoadData();
                helper.SetMaterials(material0, material1, material2);
                m_preallocatedMultiMaterialHelpers.Add(id, helper);
            }
            return helper;

            //m_multiMaterialHelper.SetMaterials(material0, material1, material2);
            //return m_multiMaterialHelper;
        }

        public static void Begin()
        {
            foreach (MySingleMaterialHelper helper in m_preallocatedSingleMaterialHelpers)
            {
                if (helper != null)
                {
                    helper.IndexCount = 0;
                    helper.VertexCount = 0;
                }
            }

            foreach (var pair in m_preallocatedMultiMaterialHelpers)
            {
                pair.Value.VertexCount = 0;
            }
        }

        public static MySingleMaterialHelper[] GetSingleMaterialHelpers()
        {
            return m_preallocatedSingleMaterialHelpers;
        }

        public static Dictionary<int, MyMultiMaterialHelper> GetMultiMaterialHelpers()
        {
            return m_preallocatedMultiMaterialHelpers;
        }
    }
}

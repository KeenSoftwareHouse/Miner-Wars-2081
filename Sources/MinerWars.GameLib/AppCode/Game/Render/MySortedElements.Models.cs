using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Render
{
    partial class MySortedElements
    {
        public class ModelSet
        {
            public const int AproxUsedMeshMaterials = 100;

            public int RenderElementCount = 0;

            // We can save 0.2ms (in scene with 2800 models) when using global mesh material table and access materials only by index (dictionary replaced by array)
            public Dictionary<MyMeshMaterial, ModelMaterialSet> Models = new Dictionary<MyMeshMaterial, ModelMaterialSet>(AproxUsedMeshMaterials , m_meshMaterialComparer);
        }

        public class ModelMaterialSet
        {
            public const int AproxSameVertexBuffersPerMaterial = 10;

            public int RenderElementCount = 0;

            // Sorting by VertexBuffer is necessary, because there is often large number of same models
            public Dictionary<VertexBuffer, List<MyRender.MyRenderElement>> Models = new Dictionary<VertexBuffer, List<MyRender.MyRenderElement>>(AproxSameVertexBuffersPerMaterial, m_vertexBufferComparer);
        }
    }
}

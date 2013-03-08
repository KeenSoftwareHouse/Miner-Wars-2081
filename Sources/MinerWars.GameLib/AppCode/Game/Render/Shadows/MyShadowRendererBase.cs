using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.App;
using MinerWarsMath.Graphics;
using System.Diagnostics;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Managers;

namespace MinerWars.AppCode.Game.Render
{
    

    class MyShadowRendererBase
    {
        static protected List<MyElement> m_castingRenderObjects = new List<MyElement>();
        static protected HashSet<MyRenderObject> m_castingRenderObjectsUnique = new HashSet<MyRenderObject>();
        static protected List<MyElement> m_castingCullObjects = new List<MyElement>();
        static protected List<MyRender.MyRenderElement> m_renderElementsForShadows = new List<MyRender.MyRenderElement>();
        static protected List<MyRender.MyRenderElement> m_transparentRenderElementsForShadows = new List<MyRender.MyRenderElement>();

        //  Used to sort render elements by their properties to spare switching render states
        public class MyShadowRenderElementsComparer : IComparer<MyRender.MyRenderElement>
        {
            public int Compare(MyRender.MyRenderElement x, MyRender.MyRenderElement y)
            {
                return x.VertexBuffer.GetHashCode().CompareTo(y.VertexBuffer.GetHashCode());
            }
        }


        public static void LoadContent()
        {
        }

        public static void UnloadContent()
        {
            m_castingRenderObjects.Clear();
            m_castingRenderObjectsUnique.Clear();
            m_castingCullObjects.Clear();
            m_renderElementsForShadows.Clear();
            m_transparentRenderElementsForShadows.Clear();
        }

        protected static MyShadowRenderElementsComparer m_shadowElementsComparer = new MyShadowRenderElementsComparer();

        protected static void DrawElements(List<MyRender.MyRenderElement> elements, MyEffectShadowMap effect, bool relativeCamera, int perfCounterIndex)
        {
            
            // Draw shadows.
            effect.SetTechnique(MyEffectShadowMap.ShadowTechnique.GenerateShadow);
            DrawShadowsForElements(effect, relativeCamera, elements, false, perfCounterIndex);

            effect.SetTechnique(MyEffectShadowMap.ShadowTechnique.GenerateShadowForVoxels);
            DrawShadowsForElements(effect, relativeCamera, elements, true, perfCounterIndex);
        }

        private static void DrawShadowsForElements(MyEffectShadowMap effect, bool relativeCamera, List<MyRender.MyRenderElement> elements, bool voxelShadows, int perfCounterIndex)
        {
            effect.Begin();

            long lastVertexBuffer = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                MyRender.MyRenderElement renderElement = elements[i];

                if ((renderElement.Entity is MyVoxelMap) != voxelShadows ||
                    renderElement.VertexBuffer.IsDisposed ||
                    renderElement.IndexBuffer.IsDisposed)
                    continue;

                long currentVertexBuffer = renderElement.VertexBuffer.GetHashCode();
                if (lastVertexBuffer != currentVertexBuffer)
                {
                    lastVertexBuffer = currentVertexBuffer;

                    MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, renderElement.VertexBuffer, 0, renderElement.VertexStride);
                    MyMinerGame.Static.GraphicsDevice.VertexDeclaration = renderElement.VertexDeclaration;
                    MyMinerGame.Static.GraphicsDevice.Indices = renderElement.IndexBuffer;

                    System.Diagnostics.Debug.Assert(renderElement.IndexBuffer != null);
                }

                effect.SetWorldMatrix(relativeCamera ? renderElement.WorldMatrixForDraw : renderElement.WorldMatrix);

                MyPerformanceCounter.PerCameraDraw.ShadowDrawCalls[perfCounterIndex]++;

                effect.D3DEffect.CommitChanges();
                MyMinerGame.Static.GraphicsDevice.DrawIndexedPrimitive(SharpDX.Direct3D9.PrimitiveType.TriangleList, 0, 0, renderElement.VertexCount, renderElement.IndexStart, renderElement.TriCount);

                MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
            }

            effect.End();
        }
    }
}

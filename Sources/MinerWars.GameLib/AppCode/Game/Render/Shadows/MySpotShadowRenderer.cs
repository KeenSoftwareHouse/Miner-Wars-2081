using System.Collections.Generic;

using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Managers;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace MinerWars.AppCode.Game.Render.Shadows
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

    class MySpotShadowRenderer : MyShadowRendererBase
    {
        public const int SpotShadowMapSize = 64;

        List<MyRender.MyRenderElement> m_culledElements = new List<MyRender.MyRenderElement>();
        private BoundingFrustum m_spotFrustum = new BoundingFrustum(Matrix.Identity);

        public void RenderForLight(Matrix lightViewProjection, ref BoundingBox lightBoundingBox, Texture shadowRenderTarget, Texture shadowDepth, int spotIndex)
        {
            m_renderElementsForShadows.Clear();
            m_castingRenderObjectsUnique.Clear();
            
            m_spotFrustum.Matrix = lightViewProjection;
            
            //MyRender.GetEntitiesFromPrunningStructure(ref lightBoundingBox, m_castingRenderObjects);
            MyRender.GetEntitiesFromShadowStructure(ref lightBoundingBox, m_castingRenderObjects);

            foreach (MyElement element in m_castingRenderObjects)
            {
                MyRenderObject renderObject = (MyRenderObject)element;
                MyEntity entity = ((MyRenderObject)element).Entity;

                if (entity != null)
                {
                    if (entity is MyVoxelMap)
                    {
                        // Changed m_castersBox to lightBoundingBox, should work
                        //(entity as MyVoxelMap).GetRenderElementsForShadowmap(m_renderElementsForShadows, ref lightBoundingBox, m_spotFrustum, MyLodTypeEnum.LOD0, false);
                        (entity as MyVoxelMap).GetRenderElementsForShadowmap(m_renderElementsForShadows, renderObject.RenderCellCoord.Value, MyLodTypeEnum.LOD0, false);
                    }
                    else
                    {
                        if (entity.ModelLod0 != null)
                        {
                            MyRender.CollectRenderElementsForShadowmap(m_renderElementsForShadows, m_transparentRenderElementsForShadows, entity, entity.ModelLod0);
                        }
                    }
                }
            }

            // Set our targets
            MyMinerGame.SetRenderTarget(shadowRenderTarget, shadowDepth);
            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.All, new ColorBGRA(1.0f), 1.0f, 0);

            DepthStencilState.Default.Apply();
            RasterizerState.CullNone.Apply();
            BlendState.Opaque.Apply();

            RenderShadowMap(lightViewProjection);

            MyRender.TakeScreenshot("ShadowMapSpot", shadowRenderTarget, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
        }

        protected void RenderShadowMap(Matrix lightViewProjection)
        {
            // Set up the effect
            MyEffectShadowMap shadowMapEffect = MyRender.GetEffect(MyEffects.ShadowMap) as MinerWars.AppCode.Game.Effects.MyEffectShadowMap;
            shadowMapEffect.SetViewProjMatrix(lightViewProjection);
                      /*
            m_culledElements.Clear();
            foreach (MyRender.MyRenderElement element in m_renderElementsForShadows)
            {
                m_culledElements.Add(element);
            }
                        */
            // Draw the models
            DrawElements(m_renderElementsForShadows, shadowMapEffect, false, MyPerformanceCounter.NoSplit);
        }

        public Matrix CreateViewProjectionMatrix(Matrix lightView, float halfSize, float nearClip, float farClip)
        {
            float a = halfSize;

            Matrix lightShadowProjection = Matrix.CreatePerspectiveOffCenter(-a, a, -a, a, nearClip, farClip);

            return lightView * lightShadowProjection;
        }

        public void SetupSpotShadowBaseEffect(MyEffectSpotShadowBase effect, Matrix lightViewProjectionShadow, Texture shadowRenderTarget)
        {
            // Set shadow properties
            var shadowMap = shadowRenderTarget;
            effect.SetShadowMap(shadowMap);
            effect.SetShadowMapSize(new Vector2(shadowMap.GetLevelDescription(0).Width, shadowMap.GetLevelDescription(0).Height));
            effect.SetShadowBias(0.005f);
            effect.SetSlopeBias(0.02f);
            effect.SetLightViewProjectionShadow(lightViewProjectionShadow);
        }
    }
}

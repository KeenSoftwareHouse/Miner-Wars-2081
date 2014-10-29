#region Using

using System;
using System.Collections.Generic;

using SysUtils.Utils;

using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Render.EnvironmentMap;
using MinerWars.AppCode.Game.Render.Shadows;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using System.Linq;

//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    using Vector3 = MinerWarsMath.Vector3;
    using BoundingBox = MinerWarsMath.BoundingBox;

    static partial class MyRender
    {
        #region Content load
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyRender.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyRender::LoadContent");
            UnloadContent(false); //Because XNA does not do this automatically

            m_device = MyMinerGame.Static.GraphicsDevice;

            CreateRenderTargets();
            CreateEnvironmentMapsRT(MyRenderConstants.ENVIRONMENT_MAP_SIZE);

            MyMwcLog.WriteLine("CreateRandomTexture");

            m_randomTexture = CreateRandomTexture();

            MyShadowRendererBase.LoadContent();

            MyOcclusionQueries.LoadContent(m_device);

            LoadEffects();

            MyMwcLog.WriteLine("MyShadowRenderer");

            if (m_shadowRenderer == null)
            {
#if RENDER_PROFILING
                //m_shadowRenderer = new MyShadowRenderer(GetShadowCascadeSize(), MyRenderTargets.ShadowMap, false);
                m_shadowRenderer = new MyShadowRenderer(GetShadowCascadeSize(), MyRenderTargets.ShadowMap, MyRenderTargets.ShadowMapZBuffer, true);
#else
                m_shadowRenderer = new MyShadowRenderer(GetShadowCascadeSize(), MyRenderTargets.ShadowMap, MyRenderTargets.ShadowMapZBuffer, true);
#endif
            }

            MyMwcLog.WriteLine("SpotShadowRenderer");

            if (m_spotShadowRenderer == null)
                m_spotShadowRenderer = new MySpotShadowRenderer();

            MyMwcLog.WriteLine("InitQueries");

            List<MyElement> list = new List<MyElement>();
            BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            m_cullingStructure.OverlapAllBoundingBox(ref aabb, list);
            foreach (MyCullableRenderObject element in list)
            {
                element.InitQueries();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyRender.LoadContent - END");
        }

        private static int GetShadowCascadeSize()
        {
            return System.Math.Min(MyMinerGame.GraphicsDeviceManager.MaxTextureSize / MyShadowRenderer.NumSplits, MyRenderConstants.RenderQualityProfile.ShadowMapCascadeSize);
        }

        public static void UnloadContent(bool removeObjects = true)
        {
            MyMwcLog.WriteLine("MyRender.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyShadowRendererBase.UnloadContent();

            for (int i = 0; i < m_renderTargets.GetLength(0); i++)
            {
                DisposeRenderTarget((MyRenderTargets)i);
            }

            DisposeSpotShadowRT();

            if (m_randomTexture != null)
            {
                m_randomTexture.Dispose();
                m_randomTexture = null;
            }

            MyOcclusionQueries.UnloadContent();

            UnloadEffects();

            Clear();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyRender.UnloadContent - END");
        }

        public static void UnloadData()
        {
            AssertStructuresEmpty();

            List<MyElement> list = new List<MyElement>();
            BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            m_cullingStructure.OverlapAllBoundingBox(ref aabb, list);
            foreach (MyCullableRenderObject element in list)
            {
                element.DestroyQueries();
            }

            m_prunningStructure.Clear();
            m_cullingStructure.Clear();
            m_shadowPrunningStructure.Clear();
        }

        public static void Clear()
        {
            m_renderOcclusionQueries.Clear();


            m_spotLightRenderElements.Clear();
            m_spotLightsPool.DeallocateAll();

            m_renderObjectsToDraw.Clear();
            m_entitiesToDebugDraw.Clear();

            foreach (MyRenderElement renderElement in m_renderElementsPool)
            {
                renderElement.Entity = null;
                //renderElement.MaskTexture = null;
                renderElement.Material = null;
                renderElement.IndexBuffer = null;
                renderElement.VertexBuffer = null;
            }

            m_renderElementIndex = 0;
            m_renderElements.Clear();
            m_transparentRenderElements.Clear();
            m_cullObjectListForDraw.Clear();
            m_cullObjectListForDrawMain.Clear();
            m_renderObjectListForDraw.Clear();
            m_renderObjectListForDrawMain.Clear();
            m_renderObjectListForIntersections.Clear();
        }

        [Conditional("DEBUG")]
        public static void AssertStructuresEmpty()
        {
            BoundingBox testAABB = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            m_prunningStructure.OverlapAllBoundingBox(ref testAABB, m_renderObjectListForDraw);
            Debug.Assert(m_renderObjectListForDraw.Count == 0, "There are some objects in render prunning structure which are not removed on unload!");

            m_shadowPrunningStructure.OverlapAllBoundingBox(ref testAABB, m_renderObjectListForDraw);
            Debug.Assert(m_renderObjectListForDraw.Count == 0, "There are some objects in shadow prunning structure which are not removed on unload!");

            m_cullingStructure.OverlapAllBoundingBox(ref testAABB, m_cullObjectListForDraw);
            int count = 0;
            foreach (var obj in m_cullObjectListForDraw)
            {
                count += ((MyCullableRenderObject)obj).EntitiesContained;
                Debug.Assert(((MyCullableRenderObject)obj).EntitiesContained == 0, "There are some objects in culling structure which are not removed on unload!");

               // ((MyCullableRenderObject)obj).CulledObjects.OverlapAllBoundingBox(ref testAABB, m_renderObjectListForDraw);

            }
        }

        #endregion

        #region Render targets

        public static void CreateRenderTargets()
        {
            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                return;

            MyMwcLog.WriteLine("MyRender.CreateRenderTargets - START");

            int forwardRTWidth = (int)(MyCamera.ForwardViewport.Width);
            int forwardRTHeight = (int)(MyCamera.ForwardViewport.Height);
            int forwardRTHalfWidth = (int)(MyCamera.ForwardViewport.Width / 2);
            int forwardRTHalfHeight = (int)(MyCamera.ForwardViewport.Height / 2);
            int forwardRT4Width = (int)(MyCamera.ForwardViewport.Width / 4);
            int forwardRT4Height = (int)(MyCamera.ForwardViewport.Height / 4);
            int forwardRT8Width = (int)(MyCamera.ForwardViewport.Width / 8);
            int forwardRT8Height = (int)(MyCamera.ForwardViewport.Height / 8);

            int secondaryShadowMapSize = MyRenderConstants.RenderQualityProfile.SecondaryShadowMapCascadeSize;

            //Largest RT
#if COLOR_SHADOW_MAP_FORMAT
            CreateRenderTarget(MyRenderTargets.ShadowMap, MyShadowRenderer.NumSplits * GetShadowCascadeSize(), GetShadowCascadeSize(), SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            CreateRenderTarget(MyRenderTargets.SecondaryShadowMap, MyShadowRenderer.NumSplits * secondaryShadowMapSize, secondaryShadowMapSize, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
#else
            CreateRenderTarget(MyRenderTargets.ShadowMap, MyShadowRenderer.NumSplits * GetShadowCascadeSize(), GetShadowCascadeSize(), Format.R32F);
            CreateRenderTarget(MyRenderTargets.ShadowMapZBuffer, MyShadowRenderer.NumSplits * GetShadowCascadeSize(), GetShadowCascadeSize(), Format.D24S8, Usage.DepthStencil);

            CreateRenderTarget(MyRenderTargets.SecondaryShadowMap, MyShadowRenderer.NumSplits * secondaryShadowMapSize, secondaryShadowMapSize, Format.R32F);
            CreateRenderTarget(MyRenderTargets.SecondaryShadowMapZBuffer, MyShadowRenderer.NumSplits * secondaryShadowMapSize, secondaryShadowMapSize, Format.D24S8, Usage.DepthStencil);
#endif

            //Full viewport RTs
            CreateRenderTarget(MyRenderTargets.Auxiliary0, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.Auxiliary1, forwardRTWidth, forwardRTHeight, Format.A16B16G16R16F);
            CreateRenderTarget(MyRenderTargets.Auxiliary2, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8);

            CreateRenderTarget(MyRenderTargets.Normals, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8, Usage.RenderTarget | Usage.AutoGenerateMipMap);
            CreateRenderTarget(MyRenderTargets.Diffuse, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8, Usage.RenderTarget | Usage.AutoGenerateMipMap);
            CreateRenderTarget(MyRenderTargets.Depth, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8, Usage.RenderTarget | Usage.AutoGenerateMipMap);
            CreateRenderTarget(MyRenderTargets.ZBuffer, forwardRTWidth, forwardRTHeight, Format.D24S8, Usage.DepthStencil);

            CreateRenderTarget(MyRenderTargets.EnvironmentMap, forwardRTWidth, forwardRTHeight, Format.A8R8G8B8);

            //Half viewport RTs
            CreateRenderTarget(MyRenderTargets.AuxiliaryHalf0, forwardRTHalfWidth, forwardRTHalfHeight, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.AuxiliaryHalf1010102, forwardRTHalfWidth, forwardRTHalfHeight, Format.A2R10G10B10);

            CreateRenderTarget(MyRenderTargets.DepthHalf, forwardRTHalfWidth, forwardRTHalfHeight, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.SSAO, forwardRTHalfWidth, forwardRTHalfHeight, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.SSAOBlur, forwardRTHalfWidth, forwardRTHalfHeight, Format.A8R8G8B8);

            //Quarter viewport RTs
            CreateRenderTarget(MyRenderTargets.AuxiliaryQuarter0, forwardRT4Width, forwardRT4Height, Format.A8R8G8B8);

            if (MyPostProcessHDR.RenderHDR())
            {
                CreateRenderTarget(MyRenderTargets.HDR4, forwardRT4Width, forwardRT4Height, Format.A2R10G10B10);
                CreateRenderTarget(MyRenderTargets.HDR4Threshold, forwardRT4Width, forwardRT4Height, Format.A2R10G10B10);
            }

            //Low size RTs
            CreateRenderTarget(MyRenderTargets.SecondaryCamera, MyCamera.BackwardViewport.Width, MyCamera.BackwardViewport.Height, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.SecondaryCameraZBuffer, MyCamera.BackwardViewport.Width, MyCamera.BackwardViewport.Height, Format.D24S8, Usage.DepthStencil);
            CreateSpotShadowRT();

            SetEnvironmentRenderTargets();


            m_GBufferDefaultBinding = new Texture[] { (Texture)MyRender.GetRenderTarget(MyRenderTargets.Normals), (Texture)MyRender.GetRenderTarget(MyRenderTargets.Diffuse), (Texture)MyRender.GetRenderTarget(MyRenderTargets.Depth) };
            m_aux0Binding = new Texture[] { (Texture)MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0) };

            MyMwcLog.WriteLine("MyRender.CreateRenderTargets - END");
        }

        public static void CreateSpotShadowRT()
        {
            MyMwcLog.WriteLine("MyRender.CreateSpotShadowRT - START");

            DisposeSpotShadowRT();

            for (int i = 0; i < MyRenderConstants.SPOT_SHADOW_RENDER_TARGET_COUNT; i++)
            {
                if (MySpotShadowRenderer.SpotShadowMapSize <= 0 || MySpotShadowRenderer.SpotShadowMapSize <= 0) // may happen when creatting in load content
                    return;

                m_spotShadowRenderTargets[i] = new Texture(m_device, MySpotShadowRenderer.SpotShadowMapSize, MySpotShadowRenderer.SpotShadowMapSize, 0, Usage.RenderTarget, Format.R32F, Pool.Default);
                m_spotShadowRenderTargets[i].DebugName = "SpotShadowRT" + i;
                m_spotShadowRenderTargetsZBuffers[i] = new Texture(m_device, MySpotShadowRenderer.SpotShadowMapSize, MySpotShadowRenderer.SpotShadowMapSize, 0, Usage.DepthStencil, Format.D24S8, Pool.Default);
                m_spotShadowRenderTargetsZBuffers[i].DebugName = "SpotShadowDepthRT" + i;
            }

            MyMwcLog.WriteLine("MyRender.CreateSpotShadowRT - END");
        }

        // Create environment map render targets for both cube textures
        public static void CreateEnvironmentMapsRT(int environmentMapSize)
        {
            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                return;


            MyMwcLog.WriteLine("MyRender.CreateEnvironmentMapsRT - START");

            CreateRenderTargetCube(MyRenderTargets.EnvironmentCube, environmentMapSize, Format.A8R8G8B8);
            CreateRenderTargetCube(MyRenderTargets.EnvironmentCubeAux, environmentMapSize, Format.A8R8G8B8);

            CreateRenderTargetCube(MyRenderTargets.AmbientCube, environmentMapSize, Format.A8R8G8B8);
            CreateRenderTargetCube(MyRenderTargets.AmbientCubeAux, environmentMapSize, Format.A8R8G8B8);

            CreateRenderTarget(MyRenderTargets.EnvironmentFaceAux, environmentMapSize, environmentMapSize, Format.A8R8G8B8);
            CreateRenderTarget(MyRenderTargets.EnvironmentFaceAux2, environmentMapSize, environmentMapSize, Format.A8R8G8B8);

            SetEnvironmentRenderTargets();

            MyMwcLog.WriteLine("MyRender.CreateEnvironmentMapsRT - END");
        }

        /// <summary>
        /// Sets the environment render targets.
        /// </summary>
        private static void SetEnvironmentRenderTargets()
        {
            var rt1 = MyRender.GetRenderTargetCube(MyRenderTargets.EnvironmentCube);
            var rt2 = MyRender.GetRenderTargetCube(MyRenderTargets.EnvironmentCubeAux);
            var rt3 = MyRender.GetRenderTargetCube(MyRenderTargets.AmbientCube);
            var rt4 = MyRender.GetRenderTargetCube(MyRenderTargets.AmbientCubeAux);
            var rt5 = MyRender.GetRenderTarget(MyRenderTargets.EnvironmentMap);

            MyEnvironmentMap.SetRenderTargets((CubeTexture)rt1, (CubeTexture)rt2, (CubeTexture)rt3, (CubeTexture)rt4, (Texture)rt5);
        }

        static void CreateRenderTargetCube(MyRenderTargets renderTarget, int size, Format surfaceFormat)
        {
            MyMwcLog.WriteLine("MyRender.CreateRenderTargetCube - START");

            DisposeRenderTarget(renderTarget);
            if (size <= 0)
            {
                return;
            }

            m_renderTargets[(int)renderTarget] = new CubeTexture(m_device, size, 0, Usage.RenderTarget, surfaceFormat, Pool.Default);
            m_renderTargets[(int)renderTarget].DebugName = renderTarget.ToString();
            m_renderTargets[(int)renderTarget].Tag = new Vector2(size, size);

            MyMwcLog.WriteLine("MyRender.CreateRenderTargetCube - END");
        }

        static void CreateRenderTarget(MyRenderTargets renderTarget, int width, int height, Format preferredFormat, Usage usage = Usage.RenderTarget /*| Usage.AutoGenerateMipMap*/)
        {
            //  Dispose render target - this happens e.g. after video resolution change
            DisposeRenderTarget(renderTarget);
            if (width <= 0 || height <= 0) // may happen when creatting in load content
                return;

            //  Create new render target, no anti-aliasing
            m_renderTargets[(int)renderTarget] = new Texture(m_device, width, height, 0, usage, preferredFormat, Pool.Default);
            m_renderTargets[(int)renderTarget].DebugName = renderTarget.ToString();
            m_renderTargets[(int)renderTarget].Tag = new Vector2(width, height);
        }

        static void DisposeRenderTarget(MyRenderTargets renderTarget)
        {
            if (m_renderTargets[(int)renderTarget] != null)
            {
                m_renderTargets[(int)renderTarget].Dispose();
                m_renderTargets[(int)renderTarget] = null;
            }
        }

        static void DisposeSpotShadowRT()
        {
            for (int i = 0; i < MyRenderConstants.SPOT_SHADOW_RENDER_TARGET_COUNT; i++)
            {
                if (m_spotShadowRenderTargets[i] != null)
                {
                    m_spotShadowRenderTargets[i].Dispose();
                    m_spotShadowRenderTargets[i] = null;
                }

                if (m_spotShadowRenderTargetsZBuffers[i] != null)
                {
                    m_spotShadowRenderTargetsZBuffers[i].Dispose();
                    m_spotShadowRenderTargetsZBuffers[i] = null;
                }
            }
        }

        internal static Texture GetRenderTarget(MyRenderTargets renderTarget)
        {
            return (Texture)m_renderTargets[(int)renderTarget];
        }

        internal static CubeTexture GetRenderTargetCube(MyRenderTargets renderTarget)
        {
            return (CubeTexture)m_renderTargets[(int)renderTarget];
        }

        #endregion

        #region Effects

        public static void LoadEffects()
        {
            MyMwcLog.WriteLine("MyRender.LoadEffects - START");

            //Post process effects
            //m_effects[(int)MyEffects.LodTransition2] = new MyEffectLodTransition2();
            m_effects[(int)MyEffects.ClearGBuffer] = new MyEffectClearGbuffer();
            m_effects[(int)MyEffects.ShadowMap] = new MyEffectShadowMap();
            //m_effects[(int)MyEffects.ShadowOcclusion] = new MyEffectShadowOcclusion();
            m_effects[(int)MyEffects.TransparentGeometry] = new MyEffectTransparentGeometry();
            //m_effects[(int)MyEffects.HudSectorBorder] = new MyEffectHudSectorBorder();
            m_effects[(int)MyEffects.Decals] = new MyEffectDecals();
            m_effects[(int)MyEffects.PointLight] = new MyEffectPointLight();
            m_effects[(int)MyEffects.DirectionalLight] = new MyEffectDirectionalLight();
            m_effects[(int)MyEffects.BlendLights] = new MyEffectBlendLights();
            m_effects[(int)MyEffects.VolumetricSSAO] = new MyEffectVolumetricSSAO2();
            m_effects[(int)MyEffects.SSAOBlur] = new MyEffectSSAOBlur2();
            m_effects[(int)MyEffects.GaussianBlur] = new MyEffectGaussianBlur();
            m_effects[(int)MyEffects.DownsampleForSSAO] = new MyEffectDownsampleDepthForSSAO();
            m_effects[(int)MyEffects.AntiAlias] = new MyEffectAntiAlias();
            m_effects[(int)MyEffects.Screenshot] = new MyEffectScreenshot();
            m_effects[(int)MyEffects.Scale] = new MyEffectScale();
            m_effects[(int)MyEffects.Threshold] = new MyEffectThreshold();
            m_effects[(int)MyEffects.HDR] = new MyEffectHDR();
            m_effects[(int)MyEffects.Luminance] = new MyEffectLuminance();
            m_effects[(int)MyEffects.Contrast] = new MyEffectContrast();
            m_effects[(int)MyEffects.VolumetricFog] = new MyEffectVolumetricFog();
            m_effects[(int)MyEffects.GodRays] = new MyEffectGodRays();

            //Model effects
            m_effects[(int)MyEffects.Gizmo] = new MyEffectRenderGizmo();
            m_effects[(int)MyEffects.ModelDNS] = new MyEffectModelsDNS();
            m_effects[(int)MyEffects.ModelDiffuse] = new MyEffectModelsDiffuse();
            m_effects[(int)MyEffects.VoxelDebrisMRT] = new MyEffectVoxelsDebris();
            m_effects[(int)MyEffects.VoxelsMRT] = new MyEffectVoxels();
            m_effects[(int)MyEffects.VoxelStaticAsteroidMRT] = new MyEffectVoxelsStaticAsteroid();
            m_effects[(int)MyEffects.OcclusionQueryDrawMRT] = new MyEffectOcclusionQueryDraw();
            m_effects[(int)MyEffects.VideoSpriteEffects] = new MyEffectSpriteBatchShader();//prejmenovat enum..
            m_effects[(int)MyEffects.AmbientMapPrecalculation] = new MyEffectAmbientPrecalculation();

            //Background
            m_effects[(int)MyEffects.DistantImpostors] = new MyEffectDistantImpostors();
            m_effects[(int)MyEffects.BackgroundCube] = new MyEffectBackgroundCube();

            //HUD
            m_effects[(int)MyEffects.HudRadar] = new MyEffectHudRadar();
            m_effects[(int)MyEffects.Hud] = new MyEffectHud();
            m_effects[(int)MyEffects.CockpitGlass] = new MyEffectCockpitGlass();

            //SolarSystemMap
            m_effects[(int)MyEffects.SolarMapGrid] = new MyEffectSolarMapGrid();   


            Debug.Assert(m_effects.All(effect => effect != null));

            MyMwcLog.WriteLine("MyRender.LoadEffects - END");
        }

        static void UnloadEffects()
        {
            MyMwcLog.WriteLine("MyRender.UnloadEffects - START");

            for (int i = 0; i < Enum.GetValues(typeof(MyEffects)).GetLength(0); i++)
            {
                MyEffectBase effect = m_effects[i];
                if (effect != null)
                {
                    effect.Dispose();
                    m_effects[i] = null;
                }
            }

            MyMwcLog.WriteLine("MyRender.UnloadEffects - END");
        }

        static Texture CreateRandomTexture()
        {
            Random r = new Random();
            int size = 256;
            float[] rnd = new float[size];
            for (int i = 0; i < size; i++)
            {
                rnd[i] = MyMwcUtils.GetRandomFloat(-1, 1);
            }

            var result = new Texture(MyMinerGame.GraphicsDeviceManager.GraphicsDevice, size, 1, 0, Usage.None, Format.R32F, Pool.Managed);
            DataStream ds;
            result.LockRectangle(0, LockFlags.None, out ds);
            ds.WriteRange(rnd);
            result.UnlockRectangle(0);

            return result;
        }

        public static Texture GetRandomTexture()
        {
            return m_randomTexture;
        }

        internal static MyEffectBase GetEffect(MyEffects effect)
        {
            return m_effects[(int)effect];
        }

        internal static MyPostProcessBase GetPostProcess(MyPostProcessEnum name)
        {
            return m_postProcesses.Find(x => x.Name == name);
        }

        internal static IEnumerable<MyPostProcessBase> GetPostProcesses()
        {
            return m_postProcesses;
        }

        #endregion
    }
}
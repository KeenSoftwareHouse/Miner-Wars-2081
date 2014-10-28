#region Using

using System.Collections.Generic;

using SysUtils;
using SysUtils.Utils;


using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using KeenSoftwareHouse.Library.Extensions;
using System.Diagnostics;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using System.Linq;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.Render
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

    static partial class MyRender
    {
        #region Draw

        internal static void DrawScene()
        {
            m_renderProfiler.StartProfilingBlock("Draw scene Part1");

            m_transparentRenderElements.Clear();

            // Renders into aux0
            DrawBackground(m_aux0Binding);

                /*
            MyMinerGameDX.SetRenderTarget(null);
            BlendState.Opaque.Apply();
            Blit(GetRenderTarget(MyRenderTargets.Auxiliary0), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
                  */

            //return;

            m_sortedElements.Clear();

            // Prepare entities for draw
            PrepareRenderObjectsForDraw();

            //  Draw LOD0 scene, objects
            DrawScene_OneLodLevel(MyLodTypeEnum.LOD0);

            //  Draw LOD1 scene, objects
            DrawScene_OneLodLevel(MyLodTypeEnum.LOD1);

            m_renderProfiler.StartProfilingBlock("AllGeometryRendered");
            DrawRenderModules(MyRenderStage.AllGeometryRendered);
            m_renderProfiler.EndProfilingBlock();

            // Draw transparent models (non LODed, to be fully lit, not sorted)
            DrawScene_Transparent();

            // Issue occlusion queries
            IssueOcclusionQueries();
              
            // Render post processes, there's no source and target in this stage
            RenderPostProcesses(PostProcessStage.LODBlend, null, null, GetRenderTarget(MyRenderTargets.Auxiliary1), false);

            // Take screenshots if required
            //TakeLODScreenshots();

            m_renderProfiler.EndProfilingBlock();   //Draw scene Part1

            m_renderProfiler.StartProfilingBlock("Draw scene Part2");

            if (EnableLights && EnableLightsRuntime && MyRender.CurrentRenderSetup.EnableLights.Value)
            {
                //Render shadows to Lod0Diffuse
                if (EnableSun && EnableShadows && CurrentRenderSetup.EnableSun.Value)
                {
                    m_renderProfiler.StartProfilingBlock("Render shadows");

                    GetShadowRenderer().Render();

                    m_renderProfiler.EndProfilingBlock();
                }
                else
                {
                    // Set our targets
                    MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.ShadowMap), MyRender.GetRenderTarget(MyRenderTargets.ShadowMapZBuffer));
                    m_device.Clear(ClearFlags.ZBuffer, new ColorBGRA(1.0f), 1.0f, 0);

                    MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.SecondaryShadowMap), MyRender.GetRenderTarget(MyRenderTargets.SecondaryShadowMapZBuffer));
                    m_device.Clear(ClearFlags.ZBuffer, new ColorBGRA(1.0f), 1.0f, 0);
                }

                //Render all lights to Lod0Depth (LDR or HDR-part1) and Lod0Diffuse (HDR-part2)
                RenderLights();
            }
            else
            { 
                MyMinerGame.SetRenderTarget(GetRenderTarget(MyRenderTargets.Auxiliary1), null);
                BlendState.Opaque.Apply();
                RasterizerState.CullNone.Apply();
                DepthStencilState.None.Apply();
                Blit(GetRenderTarget(MyRenderTargets.Diffuse), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
            }

                       /*    
          MyMinerGameDX.SetRenderTarget(null, null, SetDepthTargetEnum.RestoreDefault);
          BlendState.Opaque.Apply();
          RasterizerState.CullNone.Apply();
          DepthStencilState.None.Apply();
          Blit(GetRenderTarget(MyRenderTargets.Auxiliary0), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
          Blit(GetRenderTarget(MyRenderTargets.Diffuse), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
          return;
                         */   

            RenderPostProcesses(PostProcessStage.PostLighting, null, null, GetRenderTarget(MyRenderTargets.EnvironmentMap), false);

            m_renderProfiler.StartProfilingBlock("AlphaBlendPreHDR");
            DrawRenderModules(MyRenderStage.AlphaBlendPreHDR);
            m_renderProfiler.EndProfilingBlock();

            TakeScreenshot("Blended_lights", GetRenderTarget(MyRenderTargets.Auxiliary1), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);

            m_renderProfiler.EndProfilingBlock();   //Draw scene part 2

            m_renderProfiler.StartProfilingBlock("Draw scene Part3 ");

            //MyMinerGameDX.SetRenderTarget(null, null);

            // Render post processes
            RenderPostProcesses(PostProcessStage.HDR, GetRenderTarget(MyRenderTargets.Auxiliary1), m_aux0Binding, GetRenderTarget(MyRenderTargets.Auxiliary2), true, true);
           /* 
            MyMinerGameDX.SetRenderTarget(GetRenderTarget(MyRenderTargets.Auxiliary0), null, SetDepthTargetEnum.RestoreDefault);
            SetCorrectViewportSize();
            BlendState.Opaque.Apply();
            RasterizerState.CullNone.Apply();
            DepthStencilState.None.Apply();
            Blit(GetRenderTarget(MyRenderTargets.Auxiliary1), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
                   */
            TakeScreenshot("HDR_down4_blurred", MyRender.GetRenderTarget(MyRenderTargets.HDR4Threshold), MyEffectScreenshot.ScreenshotTechniqueEnum.HDR);

            m_renderProfiler.EndProfilingBlock();   //Draw scene Part2

            m_renderProfiler.StartProfilingBlock("Draw scene Part4");
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Only one render target - where we draw final LOD scene
            MyMinerGame.SetRenderTargets(m_aux0Binding, null);
            SetCorrectViewportSize();


            if (ShowBlendedScreens && CurrentRenderSetup.EnableDebugHelpers.Value)
            {
                DrawDebugBlendedRenderTargets();
            }

            m_renderProfiler.StartProfilingBlock("Alphablend");
            DrawRenderModules(MyRenderStage.AlphaBlend);
            m_renderProfiler.EndProfilingBlock();



            // Render post processes
            RenderPostProcesses(PostProcessStage.AlphaBlended, GetRenderTarget(MyRenderTargets.Auxiliary0), CurrentRenderSetup.RenderTargets, GetRenderTarget(MyRenderTargets.Auxiliary1), true, true);

            /*  
            MyMinerGame.SetRenderTargets(CurrentRenderSetup.RenderTargets, CurrentRenderSetup.DepthTarget);
            BlendState.Opaque.Apply();
            RasterizerState.CullNone.Apply();
            DepthStencilState.None.Apply();
            Blit(GetRenderTarget(MyRenderTargets.Auxiliary0), false, MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
              */

            MyMinerGame.SetRenderTargets(CurrentRenderSetup.RenderTargets, CurrentRenderSetup.DepthTarget);
            SetCorrectViewportSize();

            if (m_currentSetup.DepthToAlpha)
            {
                Blit(GetRenderTarget(MyRenderTargets.Depth), true, MyEffectScreenshot.ScreenshotTechniqueEnum.DepthToAlpha);
            }
              /*
            if (ShowLODScreens && CurrentRenderSetup.EnableDebugHelpers.Value)
            {
                DrawDebugLODRenderTargets();
            }   */


            if (ShowEnvironmentScreens && CurrentRenderSetup.EnableDebugHelpers.Value)
            {
                DrawDebugEnvironmentRenderTargets();
            }

            m_renderProfiler.EndProfilingBlock();   //Draw scene Part3
        }

        internal static void DrawSceneForward()
        {
            bool HWOcclusionQueries = EnableHWOcclusionQueries;
            bool HWOcclusionQueriesForShadows = EnableHWOcclusionQueriesForShadows;

            //EnableHWOcclusionQueries = false;
            EnableHWOcclusionQueriesForShadows = false;


            m_transparentRenderElements.Clear();

            m_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new ColorBGRA(0), 1, 0);

            // Forward render into screen
            DrawBackground(CurrentRenderSetup.RenderTargets);

            m_sortedElements.Clear();

            // Prepare entities for draw
            PrepareRenderObjectsForDraw();

            UpdateForwardLights();

            DrawScene_OneLodLevel_Forward(MyLodTypeEnum.LOD0);

            DrawScene_OneLodLevel_Forward(MyLodTypeEnum.LOD1);

            DrawScene_Transparent();

            if (MyGuiManager.GetScreenshot() == null)
            {
                IssueOcclusionQueries();
            }

            m_renderProfiler.StartProfilingBlock("AlphaBlendPreHDR");
            DrawRenderModules(MyRenderStage.AlphaBlendPreHDR);
            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.StartProfilingBlock("AlphaBlend");
            DrawRenderModules(MyRenderStage.AlphaBlend);
            m_renderProfiler.EndProfilingBlock();

            EnableHWOcclusionQueries = HWOcclusionQueries;
            EnableHWOcclusionQueriesForShadows = HWOcclusionQueriesForShadows;
        }


        #region Screenshot


        internal static void TakeLODScreenshots()
        {
            if (MyGuiManager.GetScreenshot() != null)
            {
                MyRender.TakeScreenshot("Blended_Diffuse", MyRender.GetRenderTarget(MyRenderTargets.Diffuse), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
                MyRender.TakeScreenshot("Blended_SpecularIntensity", MyRender.GetRenderTarget(MyRenderTargets.Diffuse), MyEffectScreenshot.ScreenshotTechniqueEnum.Alpha);

                MyRender.TakeScreenshot("Blended_Normals", MyRender.GetRenderTarget(MyRenderTargets.Normals), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
                MyRender.TakeScreenshot("Blended_SpecularPower", MyRender.GetRenderTarget(MyRenderTargets.Normals), MyEffectScreenshot.ScreenshotTechniqueEnum.Alpha);

                MyRender.TakeScreenshot("Blended_Depth", MyRender.GetRenderTarget(MyRenderTargets.Depth), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);
                MyRender.TakeScreenshot("Blended_Emissive", MyRender.GetRenderTarget(MyRenderTargets.Depth), MyEffectScreenshot.ScreenshotTechniqueEnum.Alpha);
            }
        }


        public static void TakeScreenshot(Texture rt, string name = "last")
        {
            MyMinerGame.SetRenderTarget(null, null);
       //     MyScreenshot.SaveScreenshot(rt, name + ".png");
        }

        public static void TakeScreenshot(MyRenderTargets rt)
        {
            TakeScreenshot(GetRenderTarget(rt), rt.ToString());
        }
       
        internal static void TakeScreenshot(string name, BaseTexture target, MyEffectScreenshot.ScreenshotTechniqueEnum technique)
        {
            if (ScreenshotOnlyFinal && name != "FinalScreen")
                return;

            //  Screenshot object survives only one DRAW after created. We delete it immediatelly. So if 'm_screenshot'
            //  is not null we know we have to take screenshot and set it to null.
            if (MyGuiManager.GetScreenshot() != null)
            {
                if (target is Texture)
                {
                    Texture renderTarget = target as Texture;
                    Texture rt = new Texture(m_device, renderTarget.GetLevelDescription(0).Width, renderTarget.GetLevelDescription(0).Height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                    MyMinerGame.SetRenderTarget(rt, null);
                    BlendState.NonPremultiplied.Apply();

                    MyEffectScreenshot ssEffect = GetEffect(MyEffects.Screenshot) as MyEffectScreenshot;
                    ssEffect.SetSourceTexture(renderTarget);
                    ssEffect.SetTechnique(technique);
                    MyGuiManager.GetFullscreenQuad().Draw(ssEffect);

                    MyMinerGame.SetRenderTarget(null, null);
                    MyGuiManager.GetScreenshot().SaveTexture2D(rt, name);
                    rt.Dispose();
                }
                else if (target is CubeTexture)
                {
                    string filename = MyGuiManager.GetScreenshot().GetFilename(name + ".dds");
                    CubeTexture.ToFile(target, filename, ImageFileFormat.Dds);
                    //MyDDSFile.DDSToFile(filename, true, target, false);
                }
            }  
        }

        #endregion

        #region Draw


        public static MyLodTypeEnum GetCurrentLodDrawPass()
        {
            return m_currentLodDrawPass;
        }

        private static void DrawModelsLod(MyLodTypeEnum lodTypeEnum, bool drawUsingStencil, bool collectTransparentElements)
        {
            m_renderElements.Clear();

            // Call draw for all entities, they push wanted render models to the list
            m_renderObjectsToDraw.Clear();

            m_renderProfiler.StartProfilingBlock("LODDrawStart");
            DrawRenderModules(MyRenderStage.LODDrawStart);
            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.StartProfilingBlock("entity.Draw");


            if (lodTypeEnum == MyLodTypeEnum.LOD_NEAR)
            {
                foreach (MyRenderObject renderObject in m_nearObjects)
                {
                    MyEntity entity = renderObject.Entity;
                    entity.Draw(renderObject);
                }
            }
            else
            {        
                //  Iterate over all objects that intersect the frustum (objects and voxel maps)
                foreach (MyRenderObject renderObject in m_renderObjectListForDraw)
                {
                    MyEntity entity = renderObject.Entity;
                    entity.Draw(renderObject);
                }      
            }


            m_renderProfiler.EndProfilingBlock();

            switch (lodTypeEnum)
            {
                case MyLodTypeEnum.LOD_NEAR:
                    m_renderProfiler.StartProfilingBlock("CollectElements LOD_NEAR");
                    break;
                case MyLodTypeEnum.LOD0:
                    m_renderProfiler.StartProfilingBlock("CollectElements LOD0");
                    break;
                case MyLodTypeEnum.LOD1:
                    m_renderProfiler.StartProfilingBlock("CollectElements LOD1");
                    break;
            }

            CollectElements(lodTypeEnum, collectTransparentElements);

            m_renderProfiler.EndProfilingBlock();

            switch (lodTypeEnum)
            {
                case MyLodTypeEnum.LOD_NEAR:
                    m_renderProfiler.StartProfilingBlock("m_renderElements.Sort LOD_NEAR");
                    break;
                case MyLodTypeEnum.LOD0:
                    m_renderProfiler.StartProfilingBlock("m_renderElements.Sort LOD0");
                    break;
                case MyLodTypeEnum.LOD1:
                    m_renderProfiler.StartProfilingBlock("m_renderElements.Sort LOD1");
                    break;
            }

            //Sort render elements by their model and drawtechnique. We spare a lot of device state changes      
            //MyPerformanceCounter.PerCameraDraw.RestartTimer(lodTypeEnum == MyLodTypeEnum.LOD0 ? "SortLOD0" : lodTypeEnum == MyLodTypeEnum.LOD1 ? "SortLOD1" : "SortLODNEAR");
            if (AlternativeSort)
            {
                m_sortedElements.Add(lodTypeEnum, m_renderElements);
            }
            else
            {
                m_renderElements.Sort(m_renderElementsComparer);
            }
            //MyPerformanceCounter.PerCameraDraw.SaveTimer(lodTypeEnum == MyLodTypeEnum.LOD0 ? "SortLOD0" : lodTypeEnum == MyLodTypeEnum.LOD1 ? "SortLOD1" : "SortLODNEAR");
            m_renderProfiler.EndProfilingBlock();

            //Draw sorted render elements
            switch (lodTypeEnum)
            {
                case MyLodTypeEnum.LOD_NEAR:
                    m_renderProfiler.StartProfilingBlock("DrawRenderElements LOD_NEAR");
                    break;
                case MyLodTypeEnum.LOD0:
                    m_renderProfiler.StartProfilingBlock("DrawRenderElements LOD0");
                    break;
                case MyLodTypeEnum.LOD1:
                    m_renderProfiler.StartProfilingBlock("DrawRenderElements LOD1");
                    break;
            }
            int ibChangesStats;

            //MyPerformanceCounter.PerCameraDraw.RestartTimer(lodTypeEnum == MyLodTypeEnum.LOD0 ? "DrawLOD0" : lodTypeEnum == MyLodTypeEnum.LOD1 ? "DrawLOD1" : "DrawLODNEAR");
            if (AlternativeSort)
            {
                DrawRenderElementsAlternative(lodTypeEnum, drawUsingStencil, out ibChangesStats);
            }
            else
            {
                DrawRenderElements(m_renderElements, drawUsingStencil, out ibChangesStats);
            }
            //MyPerformanceCounter.PerCameraDraw.SaveTimer(lodTypeEnum == MyLodTypeEnum.LOD0 ? "DrawLOD0" : lodTypeEnum == MyLodTypeEnum.LOD1 ? "DrawLOD1" : "DrawLODNEAR");

            MyPerformanceCounter.PerCameraDraw.RenderElementsIBChanges += ibChangesStats;

            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.StartProfilingBlock("DrawRenderElements Near");

            if (lodTypeEnum == MyLodTypeEnum.LOD_NEAR)  //We cannot render near transparent elements later
            {
                if (!drawUsingStencil)
                {   //Only near objects while LOD0 rendering
                    int ibStateChanges;
                    DrawRenderElements(m_transparentRenderElements, drawUsingStencil, out ibStateChanges);
                    m_transparentRenderElements.Clear();
                }
            }

            MyPerformanceCounter.PerCameraDraw.RenderElementsInFrustum += m_renderElements.Count;

            m_renderProfiler.EndProfilingBlock();
        }

        public static void AddRenderObjectToDraw(MyRenderObject renderObject)
        {
            m_renderObjectsToDraw.Add(renderObject);
            m_entitiesToDebugDraw.Add(renderObject.Entity);
        }

        private static void CollectElements(MyLodTypeEnum lodTypeEnum, bool collectTransparentElements)
        {
            if (m_renderObjectsToDraw.Count <= 0)
                return;

            float farLodDistance = MyRenderConstants.RenderQualityProfile.ForwardRender ? (MyRender.CurrentRenderSetup.LodTransitionFar.Value + MyRender.CurrentRenderSetup.LodTransitionNear.Value) / 2 : MyRender.CurrentRenderSetup.LodTransitionFar.Value;
            float nearLodDistance = MyRenderConstants.RenderQualityProfile.ForwardRender ? farLodDistance : MyRender.CurrentRenderSetup.LodTransitionNear.Value;
            float backEndLodDistance = MyRenderConstants.RenderQualityProfile.ForwardRender ? farLodDistance : MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value;

            // Gather all render elements from entities, sort them later
            for (int i = 0; i < m_renderObjectsToDraw.Count; i++)
            {
                MyRenderObject renderObject = m_renderObjectsToDraw[i];
                MyEntity entity = renderObject.Entity;

                MyVoxelMap voxelMap = entity as MyVoxelMap;
                if (voxelMap != null)
                {
                    voxelMap.GetRenderElements(m_renderElements, renderObject.RenderCellCoord.Value, lodTypeEnum);
                    continue;
                }

                MyModel currentModel = null;

                if (lodTypeEnum == MyLodTypeEnum.LOD_NEAR)
                {
                    if (entity.NearFlag)
                    {
                        currentModel = entity.ModelLod0;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (lodTypeEnum == MyLodTypeEnum.LOD0)
                {
                    //m_renderProfiler.StartProfilingBlock("GetSmallestDistanceBetweenCameraAndBoundingSphere");

                    float distanceForLod0 = MyRenderConstants.RenderQualityProfile.ForwardRender ? Vector3.Distance(MyCamera.Position, entity.GetPosition()) / 2 : entity.GetSmallestDistanceBetweenCameraAndBoundingSphere();
                    distanceForLod0 = MyCamera.GetDistanceWithFOV(distanceForLod0);

                    //m_renderProfiler.EndProfilingBlock();

                    if (distanceForLod0 <= farLodDistance)
                    {
                        if (entity.Lod1ForcedDistance.HasValue && entity.Lod1ForcedDistance.Value <= distanceForLod0)
                        {
                            currentModel = entity.ModelLod1;
                        }

                        if (currentModel == null)
                            currentModel = entity.ModelLod0;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (lodTypeEnum == MyLodTypeEnum.LOD1)
                {
                    //m_renderProfiler.StartProfilingBlock("GetLargestDistanceBetweenCameraAndBoundingSphere");

                    float distanceForLod1 = MyRenderConstants.RenderQualityProfile.ForwardRender ? Vector3.Distance(MyCamera.Position, entity.GetPosition()) / 2 : entity.GetLargestDistanceBetweenCameraAndBoundingSphere();
                    float distanceForBackground = MyRenderConstants.RenderQualityProfile.ForwardRender ? Vector3.Distance(MyCamera.Position, entity.GetPosition()) / 2 : entity.GetDistanceBetweenCameraAndBoundingSphere();

                    //m_renderProfiler.EndProfilingBlock();

                    if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                    {
                        if (distanceForBackground > MyCamera.GetLodTransitionDistanceBackgroundEnd() / 2)
                        {
                            continue;
                        }
                        //Vector3.Distance(MyCamera.Position, entity.GetPosition())
                    }

                    if ((distanceForLod1 >= nearLodDistance
                        &&
                        (distanceForBackground <= MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value)
                        )
                        && (entity.ModelLod1 != null))
                    {
                        currentModel = entity.ModelLod1;

                        if ((distanceForLod1 > (farLodDistance + backEndLodDistance) * 0.5f) && (entity.ModelLod2 != null))// && entity.WorldVolume.Radius < 500)
                        //if (entity.ModelLod2 != null)
                        {
                            currentModel = entity.ModelLod2;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }

                if (currentModel == null)
                    continue;

                //////////////////////////////////////////////////////////////////////////
                //@ Collect Statistics
                if (lodTypeEnum == MyLodTypeEnum.LOD0)
                {
                    if (ShowEnhancedRenderStatsEnabled)
                    {
                        m_debugRenderObjectListForDrawLOD0.Add(entity.RenderObjects[0]);
                    }

                    MyPerformanceCounter.PerCameraDraw.ModelTrianglesInFrustum_LOD0 += currentModel.GetTrianglesCount();
                }
                else
                    if (lodTypeEnum == MyLodTypeEnum.LOD1)
                    {
                        if (ShowEnhancedRenderStatsEnabled)
                        {
                            m_debugRenderObjectListForDrawLOD1.Add(entity.RenderObjects[0]);
                        }

                        MyPerformanceCounter.PerCameraDraw.ModelTrianglesInFrustum_LOD1 += currentModel.GetTrianglesCount();
                    }

                MyPerformanceCounter.PerCameraDraw.EntitiesRendered++;

                //   m_renderProfiler.StartNextBlock("Collect render elements");
                CollectRenderElements(m_renderElements, m_transparentRenderElements, entity, currentModel, collectTransparentElements);
                //   m_renderProfiler.EndProfilingBlock();
            }
        }


        public static void AllocateRenderElement(out MyRenderElement renderElement)
        {
            lock (m_renderElementsPool)
            {
                renderElement = m_renderElementsPool[m_renderElementIndex++];
            }
            IsRenderOverloaded = m_renderElementIndex >= (MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT - 4096);
        }

        public static void CollectRenderElements(List<MyRenderElement> renderElements, List<MyRenderElement> transparentRenderElements, MyEntity entity, MyModel model, bool collectTransparentElements)
        {
            // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadInDraw");
            if (model.LoadState == LoadState.Unloaded)
            {
                //model.LoadInDraw(LoadingMode.Background);
                model.LoadInDraw(LoadingMode.Immediate);
                return;
            }
            if (model.LoadState == LoadState.Loading)
                return;

            // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            var drawMatrix = entity.GetWorldMatrixForDraw();


            int meshCount = model.GetMeshList().Count;
            for (int i = 0; i < meshCount; i++)
            {
                MyMesh mesh = model.GetMeshList()[i];
                // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Add render element");

                //if (mesh.Materials[entity.MaterialIndex].DrawTechnique == MyMeshDrawTechnique.DECAL ||
                //    mesh.Materials[entity.MaterialIndex].DrawTechnique == MyMeshDrawTechnique.HOLO)
                //    continue;

                MyMeshMaterial material = entity.GetMaterial(mesh);

                bool createElement = true;

                if (createElement)
                {
                    MyRenderElement renderElement;
                    AllocateRenderElement(out renderElement);

                    //renderElement.DebugName = entity.Name;
                    renderElement.Entity = entity;

                    renderElement.VertexBuffer = model.VertexBuffer;
                    renderElement.IndexBuffer = model.IndexBuffer;
                    renderElement.VertexCount = model.GetVerticesCount();
                    renderElement.VertexDeclaration = model.GetVertexDeclaration();
                    renderElement.VertexStride = model.GetVertexStride();

                    renderElement.IndexStart = mesh.IndexStart;
                    renderElement.TriCount = mesh.TriCount;

                    //renderElement.UseChannels = model.UseChannels;
                    //renderElement.MaskTexture = model.MaskTexture;

                    renderElement.WorldMatrixForDraw = drawMatrix;

                    renderElement.WorldMatrix = entity.WorldMatrix;

                    renderElement.Material = material;
                    renderElement.DrawTechnique = material.DrawTechnique;
                    renderElement.VoxelBatch = null;

                    Debug.Assert(renderElement.VertexBuffer != null, "Vertex buffer cannot be null!");
                    Debug.Assert(renderElement.IndexBuffer != null, "Index buffer cannot be null!");

                    if (material.DrawTechnique == MyMeshDrawTechnique.HOLO)
                    {
                        if (collectTransparentElements)
                            transparentRenderElements.Add(renderElement);
                    }
                    else
                    {

                        renderElements.Add(renderElement);
                    }
                }

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public static void CollectRenderElementsForShadowmap(List<MyRenderElement> renderElements, List<MyRenderElement> transparentRenderElements, MyEntity entity, MyModel model)
        {
            if (!EnableAsteroidShadows && (entity is MyStaticAsteroid && ((entity as MyStaticAsteroid).IsGenerated)))
                return;

            if (model.LoadState != LoadState.Loaded)
                return;

            model.LoadInDraw();

            MyRenderElement renderElement;
            CreateRenderElementShadow(renderElements, entity, model, out renderElement);
        }

        private static void CreateRenderElementShadow(List<MyRenderElement> renderElements, MyEntity entity, MyModel model, out MyRenderElement renderElement)
        {
            //AddRenderElement(renderElements, out renderElement);
            AllocateRenderElement(out renderElement);

            if (!IsRenderOverloaded)
            {
                renderElement.Entity = entity;

                renderElement.VertexBuffer = model.VertexBuffer;
                renderElement.IndexBuffer = model.IndexBuffer;
                renderElement.VertexCount = model.GetVerticesCount();
                renderElement.VertexDeclaration = model.GetVertexDeclaration();
                renderElement.VertexStride = model.GetVertexStride();


                renderElement.IndexStart = 0;
                if (renderElement.IndexBuffer != null)
                {
                    renderElement.TriCount = model.GetTrianglesCount();
                }       

                //renderElement.DebugName = entity.Name;
                renderElement.WorldMatrix = entity.WorldMatrix;

                renderElements.Add(renderElement);
            }
        }

        struct ShaderContext
        {
            public bool ApplyStencil;

            public MyEntity CurrentEntity;
            public VertexBuffer CurrentVertexBuffer;

            public MyVoxelCacheCellRenderBatch CurrentVoxelBatch;
            public MyMeshMaterial CurrentMaterial;

            public MyEffectBase CurrentShader;
            public byte CurrentDrawTechnique;

            public int IBChangesStats;
        }


        private static MyMeshMaterial m_emptyMaterial = new MyMeshMaterial("", "", null, null);

        private static void DrawRenderElements(List<MyRenderElement> renderElements, bool applyStencil, out int ibChangesStats)
        {
            ShaderContext shaderContext;
            shaderContext.ApplyStencil = applyStencil;
            shaderContext.CurrentEntity = null;
            shaderContext.CurrentShader = null;
            shaderContext.CurrentDrawTechnique = 255;
            shaderContext.CurrentVoxelBatch = null;
            shaderContext.CurrentVertexBuffer = null;
            shaderContext.CurrentMaterial = m_emptyMaterial;
            shaderContext.IBChangesStats = 0;

            BlendState.Opaque.Apply(); //set by default, blend elements are at the end. 

            //Compare last and current model or entity to change device states as little as possible
            for (int i = 0; i < renderElements.Count; i++)
            {
                MyRenderElement renderElement = renderElements[i];

                m_renderProfiler.StartProfilingBlock("PrepareDeviceForRender");
                PrepareDeviceForRender(ref shaderContext, ref renderElement);
                m_renderProfiler.EndProfilingBlock();

                m_renderProfiler.StartProfilingBlock("DrawIndexedPrimitives");
                m_device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, renderElement.VertexCount, renderElement.IndexStart, renderElement.TriCount);
                MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
                m_renderProfiler.EndProfilingBlock();
            }


            m_renderProfiler.StartProfilingBlock("shaderContext.CurrentShader.End");
            if (shaderContext.CurrentShader != null)
                shaderContext.CurrentShader.End();
            m_renderProfiler.EndProfilingBlock();


            ibChangesStats = shaderContext.IBChangesStats;
        }

        private static void PrepareDeviceForRender(ref ShaderContext shaderContext, ref MyRenderElement renderElement)
        {
            //    m_renderProfiler.StartProfilingBlock("PrepareDeviceForRender");

            int lod = (int)m_currentLodDrawPass;

            System.Diagnostics.Debug.Assert(renderElement.Entity != null);

            bool needBegin = false;

            //Material change
            bool voxelMaterialChanged = shaderContext.CurrentVoxelBatch == null && renderElement.VoxelBatch != null;
            bool voxelTechniqueChanged = false;
            
            if (renderElement.VoxelBatch != null && shaderContext.CurrentVoxelBatch != null) 
            {
                voxelMaterialChanged = renderElement.VoxelBatch.SortOrder != shaderContext.CurrentVoxelBatch.SortOrder;
                voxelTechniqueChanged = renderElement.VoxelBatch.Type != shaderContext.CurrentVoxelBatch.Type;
            }
            if ((shaderContext.CurrentMaterial.GetHashCode() != renderElement.Material.GetHashCode())
                || voxelMaterialChanged || voxelTechniqueChanged)
            {
                MyPerformanceCounter.PerCameraDraw.MaterialChanges[lod]++;
                shaderContext.CurrentMaterial = renderElement.Material;
                shaderContext.CurrentVoxelBatch = renderElement.VoxelBatch;

                MyEffectBase oldShader = shaderContext.CurrentShader;
                shaderContext.CurrentShader = MyRender.SetupShaderForMaterial(renderElement.Material, renderElement.VoxelBatch);

                if (shaderContext.CurrentDrawTechnique != (byte)renderElement.DrawTechnique || voxelTechniqueChanged)
                {
                    if (oldShader != null)
                        oldShader.End();
                    
                    needBegin = true;
                    
                    MyPerformanceCounter.PerCameraDraw.TechniqueChanges[lod]++;

                    shaderContext.CurrentDrawTechnique = (byte)renderElement.DrawTechnique;

                    MyRender.SetupShaderPerDraw(shaderContext.CurrentShader, renderElement.DrawTechnique);
                }
            }

            //VB change
            if (!object.ReferenceEquals(shaderContext.CurrentVertexBuffer, renderElement.VertexBuffer))
            {
                MyPerformanceCounter.PerCameraDraw.VertexBufferChanges[lod]++;

                //Now we have IB+VB 1:1 everywhere, subsets are done through offsets
                shaderContext.CurrentVertexBuffer = renderElement.VertexBuffer;
                m_device.Indices = renderElement.IndexBuffer;

                m_device.SetStreamSource(0, renderElement.VertexBuffer, 0, renderElement.VertexStride);
                m_device.VertexDeclaration = renderElement.VertexDeclaration;

                shaderContext.IBChangesStats++;
            }

            //Entity changed
            if (shaderContext.CurrentEntity != renderElement.Entity)
            {
                MyPerformanceCounter.PerCameraDraw.EntityChanges[lod]++;

                shaderContext.CurrentEntity = renderElement.Entity;

                SetupShaderForEntity(shaderContext.CurrentShader, renderElement);
            }

            if (needBegin)
            {
                BeginShader(shaderContext.CurrentShader, ref renderElement);
                shaderContext.CurrentShader.Begin();
            }
            else
                shaderContext.CurrentShader.D3DEffect.CommitChanges();

            //    m_renderProfiler.EndProfilingBlock();
        }

        private static void GetShaderParameters(MyEntity entity, out Vector3 diffuseColor, out float emisivity, out Vector3 highlightColor)
        {
            diffuseColor = Vector3.One;
            highlightColor = Vector3.Zero;

            emisivity = 0;

            if (entity != null)
            {
                if (entity.GetDiffuseColor().HasValue)
                    diffuseColor = entity.GetDiffuseColor().Value;

                highlightColor = entity.GetHighlightColor();
            }
        }

        private static void SetupShaderPerDraw(MyEffectBase shader, MyMeshDrawTechnique technique)
        {
            //  m_renderProfiler.StartProfilingBlock("SetupShaderPerDraw");

            MyCamera.SetupBaseEffect(shader, m_currentSetup.FogMultiplierMult);

            if (m_currentLodDrawPass == MyLodTypeEnum.LOD_NEAR)
                shader.SetProjectionMatrix(ref MyCamera.ProjectionMatrixForNearObjects);
            else
                shader.SetProjectionMatrix(ref MyCamera.ProjectionMatrix);

            shader.SetViewMatrix(ref MyCamera.ViewMatrixAtZero);

            switch (technique)
            {
                case MyMeshDrawTechnique.DECAL:
                    {
                        MyStateObjects.Static_Decals_BlendState.Apply();
                        MyStateObjects.BiasedRasterizer_StaticDecals.Apply();
                        MyStateObjects.DepthStencil_TestFarObject_DepthReadOnly.Apply();
                    }
                    break;
                case MyMeshDrawTechnique.HOLO:
                    {
                        RasterizerState.CullNone.Apply();
                        MyStateObjects.Holo_BlendState.Apply();

                        MyEffectModelsDNS dnsShader = shader as MyEffectModelsDNS;

                        dnsShader.SetHalfPixel(MyCamera.ForwardViewport.Width, MyCamera.ForwardViewport.Height);
                        dnsShader.SetScale(GetScaleForViewport(GetRenderTarget(MyRenderTargets.Depth)));

                        if (m_currentLodDrawPass != MyLodTypeEnum.LOD_NEAR && !MyRenderConstants.RenderQualityProfile.ForwardRender)
                        {
                            //m_device.DepthStencilState = DepthStencilState.DepthRead;
                            
                            MyStateObjects.DepthStencil_TestFarObject_DepthReadOnly.Apply();
                            MyStateObjects.HoloRasterizerState.Apply();
                            /*dnsShader.SetDepthTextureNear(GetRenderTarget(MyRenderTargets.Lod0Depth));
                            dnsShader.SetDepthTextureFar(GetRenderTarget(MyRenderTargets.Lod1Depth));
                             */

                        }
                        else
                        {
                            DepthStencilState.DepthRead.Apply();
                        }
                    }
                    break;
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        if (!Wireframe)
                            RasterizerState.CullCounterClockwise.Apply();
                        else
                            MyStateObjects.WireframeRasterizerState.Apply();


                        BlendState.Opaque.Apply();
                    }
                    break;
                case MyMeshDrawTechnique.VOXEL_MAP:
                    {
                        if (!Wireframe)
                            RasterizerState.CullCounterClockwise.Apply();
                        else
                            MyStateObjects.WireframeRasterizerState.Apply();

                        break;
                    }
                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        if (!Wireframe)
                            RasterizerState.CullNone.Apply();
                        else
                            MyStateObjects.WireframeRasterizerState.Apply();

                        break;
                    }
                default:
                    {
                        if (!Wireframe)
                            RasterizerState.CullCounterClockwise.Apply();
                        else
                            MyStateObjects.WireframeRasterizerState.Apply();

                    }
                    break;
            }

            // m_renderProfiler.EndProfilingBlock();
        }

        private static void BeginShader(MyEffectBase shader, ref MyRenderElement renderElement)
        {
            switch (renderElement.DrawTechnique)
            {
                case MyMeshDrawTechnique.DECAL:
                    {
                        if (shader is MyEffectModelsDNS)
                        {
                            (shader as MyEffectModelsDNS).BeginBlended();
                        }
                    }
                    break;
                case MyMeshDrawTechnique.HOLO:
                    {
                        if (m_currentLodDrawPass != MyLodTypeEnum.LOD_NEAR && !MyRenderConstants.RenderQualityProfile.ForwardRender)
                        {
                            (shader as MyEffectModelsDNS).ApplyHolo(false);
                        }
                        else
                        {
                            (shader as MyEffectModelsDNS).ApplyHolo(true);
                        }
                    }
                    break;
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        (shader as MyEffectModelsDNS).ApplyMasked();
                    }
                    break;
                case MyMeshDrawTechnique.VOXEL_MAP:
                    {
                        MyEffectVoxels effectVoxels = shader as MyEffectVoxels;
                        if (renderElement.VoxelBatch.Type == MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL)
                        {
                            effectVoxels.Apply();
                        }
                        else if (renderElement.VoxelBatch.Type == MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL)
                        {
                            effectVoxels.ApplyMultimaterial();
                        }
                        break;
                    }
                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        ((MyEffectVoxelsStaticAsteroid)shader).Apply();
                    }
                    break;
                case MyMeshDrawTechnique.MESH:
                    {
                        ((MyEffectModelsDNS)shader).SetTechnique(MyRenderConstants.RenderQualityProfile.ModelsRenderTechnique);
                    }
                    break;
                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    {
                        ((MyEffectVoxelsDebris)shader).SetTechnique(MyRenderConstants.RenderQualityProfile.VoxelsRenderTechnique);
                    }
                    break;
                default:
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                    break;
            }
        }

        private static void SetupShaderForEntity(MyEffectBase shader, MyRenderElement renderElement)
        {
            // m_renderProfiler.StartProfilingBlock("SetupShaderForEntity");

            MyEffectBase currRenderEffect = null;

            Vector3 diffuseColor = Vector3.One;
            Vector3 highlightColor = Vector3.Zero;
            float emisivity = 0;

            MyEntity entity = renderElement.Entity;

            if (entity.GetDiffuseColor().HasValue)
                diffuseColor = entity.GetDiffuseColor().Value;

            highlightColor = entity.GetHighlightColor();

            switch (renderElement.DrawTechnique)
            {
                case MyMeshDrawTechnique.MESH:
                case MyMeshDrawTechnique.DECAL:
                case MyMeshDrawTechnique.HOLO:
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        MyEffectModelsDNS effectDNS = shader as MyEffectModelsDNS;

                        effectDNS.SetWorldMatrix(renderElement.WorldMatrixForDraw);
                        effectDNS.SetDiffuseColor(diffuseColor);
                        effectDNS.SetEmissivity(emisivity);
                        effectDNS.SetHighlightColor(highlightColor);
                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    {
                        //m_renderProfiler.StartProfilingBlock("VOXELD");

                        MyEffectVoxelsDebris effectVoxelsDebris = shader as MyEffectVoxelsDebris;

                        MyExplosionDebrisVoxel explosionDebrisVoxel = entity as MyExplosionDebrisVoxel;

                        //  Random texture coord scale and per-object random texture coord offset
                        effectVoxelsDebris.SetTextureCoordRandomPositionOffset(explosionDebrisVoxel.GetRandomizedTextureCoordRandomPositionOffset());
                        effectVoxelsDebris.SetTextureCoordScale(explosionDebrisVoxel.GetRandomizedTextureCoordScale());
                        effectVoxelsDebris.SetDiffuseTextureColorMultiplier(explosionDebrisVoxel.GetRandomizedDiffuseTextureColorMultiplier());
                        effectVoxelsDebris.SetViewWorldScaleMatrix(renderElement.WorldMatrixForDraw * MyCamera.ViewMatrixAtZero);
                        effectVoxelsDebris.SetWorldMatrix(ref renderElement.WorldMatrixForDraw);
                        effectVoxelsDebris.SetDiffuseColor(diffuseColor);
                        effectVoxelsDebris.SetHighlightColor(highlightColor);
                        effectVoxelsDebris.SetEmissivity(0);

                        effectVoxelsDebris.UpdateVoxelTextures(explosionDebrisVoxel.VoxelMaterial);

                        currRenderEffect = effectVoxelsDebris;

                        //m_renderProfiler.EndProfilingBlock();
                    }
                    break;

                case MyMeshDrawTechnique.VOXEL_MAP:
                    {
                        //m_renderProfiler.StartProfilingBlock("VOXMAP");

                        MyVoxelMap voxelMap = entity as MyVoxelMap;

                        MyEffectVoxels effectVoxels = shader as MyEffectVoxels;

                        effectVoxels.SetVoxelMapPosition(voxelMap.PositionLeftBottomCorner - MyCamera.Position);

                        effectVoxels.SetDiffuseColor(diffuseColor);
                        effectVoxels.SetHighlightColor(highlightColor);
                        effectVoxels.EnablePerVertexAmbient(EnablePerVertexVoxelAmbient);

                        //m_renderProfiler.EndProfilingBlock();
                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        // m_renderProfiler.StartProfilingBlock("STATA");

                        MyEffectVoxelsStaticAsteroid effectVoxelsStaticAsteroid = shader as MyEffectVoxelsStaticAsteroid;
                        var asteroid = ((MyStaticAsteroid)entity);
                        //effectVoxelsStaticAsteroid.UpdateVoxelTextures(((MyStaticAsteroid)entity).VoxelMaterial);
                        //effectVoxelsStaticAsteroid.UpdateVoxelMultiTextures(((MyStaticAsteroid)entity).VoxelMaterial, ((MyStaticAsteroid)entity).VoxelMaterial1, null);

                        if (asteroid.VoxelMaterial1 == null || MyRenderConstants.RenderQualityProfile.ForwardRender)
                        {
                            effectVoxelsStaticAsteroid.UpdateVoxelTextures(asteroid.VoxelMaterial);
                        }
                        else
                        {
                            Debug.Assert(asteroid.FieldDir != null);

                            effectVoxelsStaticAsteroid.SetEmissivityPower1(3f);
                            effectVoxelsStaticAsteroid.SetTime(MyMinerGame.TotalGamePlayTimeInMilliseconds);
                            effectVoxelsStaticAsteroid.SetFieldDir(asteroid.FieldDir.Value);
                            effectVoxelsStaticAsteroid.UpdateVoxelMultiTextures(asteroid.VoxelMaterial, asteroid.VoxelMaterial1, null);
                        }

                        effectVoxelsStaticAsteroid.SetWorldMatrix(ref renderElement.WorldMatrixForDraw);
                        effectVoxelsStaticAsteroid.SetDiffuseColor(diffuseColor);
                        effectVoxelsStaticAsteroid.SetHighlightColor(highlightColor);
                        effectVoxelsStaticAsteroid.SetEmissivity(0);

                        //m_renderProfiler.EndProfilingBlock();
                    }
                    break;

                default:
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
            }

            //   m_renderProfiler.EndProfilingBlock();
        }

        internal static MyTexture2D GetDebugTexture()
        {
            LazyLoadDebugTextures();
            return m_debugTexture;
        }

        internal static MyTexture2D GetDebugNormalTexture()
        {
            LazyLoadDebugTextures();
            return m_debugNormalTexture;
        }

        internal static MyTexture2D GetDebugNormalTextureBump()
        {
            LazyLoadDebugTextures();
            return m_debugNormalTextureBump;
        }

        /// <summary>
        /// SetupShader
        /// </summary>
        /// <param name="shader"></param>
        public static MyEffectBase SetupShaderForMaterial(MyMeshMaterial material, MyVoxelCacheCellRenderBatch voxelBatch)
        {
            switch (material.DrawTechnique)
            {
                case MyMeshDrawTechnique.MESH:
                case MyMeshDrawTechnique.DECAL:
                case MyMeshDrawTechnique.HOLO:
                case MyMeshDrawTechnique.ALPHA_MASKED:
                    {
                        MyEffectModelsDNS shader = GetEffect(MyEffects.ModelDNS) as MyEffectModelsDNS;

                        if (material != null)
                        {
                            shader.SetTextureDiffuse(material.DiffuseTexture);
                            shader.SetTextureNormal(material.NormalTexture);

                            //Do we need this? Graphicians dont use this
                            //shader.SetDiffuseColor(material.DiffuseColor);

                            shader.SetSpecularIntensity(material.SpecularIntensity);
                            shader.SetSpecularPower(material.SpecularPower);

                            shader.SetDiffuseUVAnim(material.DiffuseUVAnim);
                            shader.SetEmissivityUVAnim(material.EmissiveUVAnim);

                            shader.SetEmissivityOffset(material.EmissivityOffset);

                            if (material.DrawTechnique == MyMeshDrawTechnique.HOLO)
                            {
                                shader.SetEmissivity(material.HoloEmissivity);
                            }

                            // Commented due 856 - graphicians have to reexport white diffuse colors from MAX
                            //shader.SetDiffuseColor(material.DiffuseColor);
                        }
                        else
                        {
                            shader.SetTextureDiffuse(null);
                            shader.SetTextureNormal(null);

                            shader.SetSpecularPower(1);
                            shader.SetSpecularIntensity(1);

                            //this value is set from object if not from material
                            //shader.SetDiffuseColor(material.DiffuseColor);
                        }

                        if (CheckDiffuseTextures)
                        {
                            if (!shader.IsTextureDiffuseSet())
                            {
                                LazyLoadDebugTextures();

                                shader.SetTextureDiffuse(m_debugTexture);
                                shader.SetDiffuseColor(Vector3.One);
                                shader.SetEmissivity(1);
                            }
                            else
                            {
                                if (material.DrawTechnique != MyMeshDrawTechnique.HOLO)
                                {
                                    shader.SetEmissivity(0);
                                }
                            }
                        }
                        if (CheckNormalTextures)
                        {
                            if (!shader.IsTextureNormalSet())
                            {
                                LazyLoadDebugTextures();

                                shader.SetTextureDiffuse(m_debugTexture);
                                shader.SetEmissivity(1);
                            }
                            else
                            {
                                shader.SetTextureDiffuse(material.NormalTexture);
                                //shader.SetTextureDiffuse(m_debugNormalTexture);
                                shader.SetEmissivity(0);
                            }
                        }

                        if (!shader.IsTextureNormalSet())
                        {
                            LazyLoadDebugTextures();
                            shader.SetTextureNormal(m_debugTexture);
                        }


                        return shader;
                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_DEBRIS:
                    {
                        MyEffectVoxelsDebris effectVoxelsDebris = GetEffect(MyEffects.VoxelDebrisMRT) as MyEffectVoxelsDebris;
                        return effectVoxelsDebris;
                    }
                    break;

                case MyMeshDrawTechnique.VOXEL_MAP:
                    {
                        MyEffectVoxels effectVoxels = MyRender.GetEffect(MyEffects.VoxelsMRT) as MyEffectVoxels;

                        if (voxelBatch.Type == MyVoxelCacheCellRenderBatchType.SINGLE_MATERIAL)
                        {
                            effectVoxels.UpdateVoxelTextures(OverrideVoxelMaterial ?? voxelBatch.Material0);
                        }
                        else if (voxelBatch.Type == MyVoxelCacheCellRenderBatchType.MULTI_MATERIAL)
                        {
                            effectVoxels.UpdateVoxelMultiTextures(OverrideVoxelMaterial ?? voxelBatch.Material0, OverrideVoxelMaterial ?? voxelBatch.Material1, OverrideVoxelMaterial ?? voxelBatch.Material2);
                        }

                        return effectVoxels;
                    }
                    break;

                case MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID:
                    {
                        MyEffectVoxelsStaticAsteroid effectVoxelsStaticAsteroid = GetEffect(MyEffects.VoxelStaticAsteroidMRT) as MyEffectVoxelsStaticAsteroid;
                        return effectVoxelsStaticAsteroid;
                    }
                    break;

                default:
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
            }

            return null;
        }

        static void LazyLoadDebugTextures()
        {
            if (m_debugTexture == null)
            {
                m_debugTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures2\\Models\\Debug\\debug_d");
            }
            if (m_debugNormalTexture == null)
            {
                m_debugNormalTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures2\\Models\\fake_ns");
            }
            if (m_debugNormalTextureBump == null)
            {
                m_debugNormalTextureBump = MyTextureManager.GetTexture<MyTexture2D>("Textures2\\Models\\Debug\\debug_n");
            }
        }

        internal static void Draw(bool applyBackupStack = true)
        {
            if (!Enabled)
            {
                return;
            }
             /*
            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                System.Diagnostics.Debug.Assert(GetShadowRenderer().m_prepareForDrawCompleted == true);
            }  */

            ApplySetupStack(m_backupSetup);

            m_renderProfiler.StartProfilingBlock("Draw total");

            if (m_currentSetup.CallerID.Value == MyRenderCallerEnum.Main)
                m_renderCounter++;

            m_renderElementIndex = 0;
            GetShadowRenderer().UpdateFrustumCorners();
            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                if (EnableLights && EnableLightsRuntime && MyRender.CurrentRenderSetup.EnableLights.Value && EnableSun && EnableShadows && CurrentRenderSetup.EnableSun.Value)
                    GetShadowRenderer().PrepareFrame();
            }
            
            //Updates dependent on draw
            m_renderProfiler.StartProfilingBlock("PrepareForDraw");
            DrawRenderModules(MyRenderStage.PrepareForDraw);
            m_renderProfiler.EndProfilingBlock();
               
            //Scene rendering

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DrawSceneForward();
            else
                DrawScene();

            if (MyMwcFinalBuildConstants.EnableDebugDraw && MyRender.CurrentRenderSetup.EnableDebugHelpers.Value)
            {
                //Debug draw
                DrawDebug();

                m_device.Clear(ClearFlags.ZBuffer, new ColorBGRA(0), 1, 0);

                m_renderProfiler.StartProfilingBlock("DebugDraw");
                DrawRenderModules(MyRenderStage.DebugDraw);
                m_renderProfiler.EndProfilingBlock();
            }

            if (applyBackupStack)
            {
                ApplySetup(m_backupSetup);
            }

            if (m_currentSetup.CallerID.Value == MyRenderCallerEnum.Main)
            {
                // Debug draw
                if (MyFakes.DEBUG_DRAW_COLLIDING_ENTITIES)
                {
                    MyDebugDrawCachedLines.DrawLines();
                    MyDebugDrawCachedLines.Clear();
                }

                MyDebugDraw.TextBatch.Draw();
            }


            //if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
              //  System.Diagnostics.Debug.Assert(GetShadowRenderer().m_prepareForDrawCompleted == true);

            //Profiling data
            m_renderProfiler.EndProfilingBlock();
        }

        /// <summary>
        /// Renders the source from parameter to the current device's render target.
        /// It just copies one RT into another (but by a rendering pass - so it's probably redundant).
        /// </summary>
        /// <param name="source"></param>
        public static void Blit(Texture source, bool scaleToTarget, MyEffectScreenshot.ScreenshotTechniqueEnum technique = MyEffectScreenshot.ScreenshotTechniqueEnum.Default)
        {
            var screenEffect = m_effects[(int)MyEffects.Screenshot] as MyEffectScreenshot;
            screenEffect.SetSourceTexture(source);

            //For case that source is bigger then camera viewport (back camera etc.)
            Vector2 scale = Vector2.One;

            if (scaleToTarget)
            {
                scale = GetScaleForViewport(source);
            }


            screenEffect.SetScale(scale);
            screenEffect.SetTechnique(technique);


            BlendState bs = BlendState.Current;
            if (technique == MyEffectScreenshot.ScreenshotTechniqueEnum.DepthToAlpha)
            {
                MyStateObjects.AlphaChannels_BlendState.Apply();
            }

            MyGuiManager.GetFullscreenQuad().Draw(screenEffect);

            if (technique == MyEffectScreenshot.ScreenshotTechniqueEnum.DepthToAlpha)
            {
                bs.Apply();
            }
        }

        internal static Vector2 GetScaleForViewport(Texture source)
        {
            return m_scaleToViewport;
            //return new Vector2(((float)MyCamera.ForwardViewport.Width / source.Width), ((float)MyCamera.ForwardViewport.Height / source.Height));
            //Vector2 scale = new Vector2(((float)source.Width / MyCamera.ForwardViewport.Width), ((float)source.Height / MyCamera.ForwardViewport.Height));
        }

        internal static void RenderPostProcesses(PostProcessStage postProcessStage, Texture source, Texture[] target, Texture availableRT, bool copyToTarget = true, bool scaleToTarget = false)
        {
            Texture lastSurface = source;

            m_renderProfiler.StartProfilingBlock("Render Post process: " + postProcessStage.ToString());

            {
                (MyRender.GetEffect(MyEffects.BlendLights) as MyEffectBlendLights).DefaultTechnique = MyEffectBlendLights.Technique.LightsEnabled;
                (MyRender.GetEffect(MyEffects.BlendLights) as MyEffectBlendLights).CopyEmissivityTechnique = MyEffectBlendLights.Technique.CopyEmissivity;

                MyEffectDirectionalLight directionalLight = MyRender.GetEffect(MyEffects.DirectionalLight) as MyEffectDirectionalLight;
                directionalLight.DefaultTechnique = MyEffectDirectionalLight.Technique.Default;
                directionalLight.DefaultWithoutShadowsTechnique = MyEffectDirectionalLight.Technique.WithoutShadows;
                directionalLight.DefaultNoLightingTechnique = MyEffectDirectionalLight.Technique.NoLighting;

                MyEffectPointLight pointLight = MyRender.GetEffect(MyEffects.PointLight) as MyEffectPointLight;
                pointLight.DefaultTechnique = MyEffectPointLight.MyEffectPointLightTechnique.Default;
                pointLight.DefaultPointTechnique = MyEffectPointLight.MyEffectPointLightTechnique.Default;
                pointLight.DefaultHemisphereTechnique = MyEffectPointLight.MyEffectPointLightTechnique.Default;
                pointLight.DefaultReflectorTechnique = MyEffectPointLight.MyEffectPointLightTechnique.Reflector;
                pointLight.DefaultSpotTechnique = MyEffectPointLight.MyEffectPointLightTechnique.Spot;
                pointLight.DefaultSpotShadowTechnique = MyEffectPointLight.MyEffectPointLightTechnique.SpotShadows;
            }


            foreach (MyPostProcessBase postProcess in m_postProcesses)
            {
                if (postProcess.Enabled && (MyRender.CurrentRenderSetup.EnabledPostprocesses == null || MyRender.CurrentRenderSetup.EnabledPostprocesses.Contains(postProcess.Name)))
                {
                    var currSurface = postProcess.Render(postProcessStage, lastSurface, availableRT);

                    // Effect used availableRT as target, so lastSurface is available now
                    if (currSurface != lastSurface && lastSurface != null)
                    {
                        availableRT = lastSurface;
                    }
                    lastSurface = currSurface;
                }
            }

            m_renderProfiler.EndProfilingBlock();

            if (lastSurface != null && copyToTarget)
            {
                MyMinerGame.SetRenderTargets(target, null);

                if (scaleToTarget)
                    SetCorrectViewportSize();

                //m_device.BlendState = BlendState.Opaque;
                BlendState.Opaque.Apply();

                Blit(lastSurface, scaleToTarget);

                //SpriteBlit(lastSurface);
            }
        }

        public static void SetCorrectViewportSize()
        {
            if (((Texture)MyRender.GetRenderTarget(MyRenderTargets.Depth)).GetLevelDescription(0).Width != MyCamera.ForwardViewport.Width)
            {   //missile camera, remote camera, back camera etc
                MyMinerGame.Static.GraphicsDevice.Viewport = MyCamera.ForwardViewport;
            }
        }

        
        /// <summary>
        /// Draw background of the scene
        /// </summary>
        internal static void DrawBackground(Texture[] targets)
        {
            m_renderProfiler.StartProfilingBlock("Draw background");

            if (targets != null)
            {
                var rt = targets[0];
                var targetWidth = rt.GetLevelDescription(0).Width;
                var targetHeight = rt.GetLevelDescription(0).Height;
                m_scaleToViewport = new Vector2(((float)MyCamera.ForwardViewport.Width / targetWidth), ((float)MyCamera.ForwardViewport.Height / targetHeight));
            }
            else
            {
                m_scaleToViewport = Vector2.One;
            }

            //Render background
            MyMinerGame.SetRenderTargets(targets, null);
            MyMinerGame.Static.SetDeviceViewport(MyCamera.Viewport);

            RasterizerState.CullNone.Apply();
            DepthStencilState.None.Apply();
            BlendState.Opaque.Apply();


            if (ShowGreenBackground)
            {
                m_device.Clear(ClearFlags.Target, new ColorBGRA(1.0f), 1, 0);
            }
            else
            {
                m_renderProfiler.StartProfilingBlock("Background");
                DrawRenderModules(MyRenderStage.Background);
                m_renderProfiler.EndProfilingBlock();
            }

            m_renderProfiler.EndProfilingBlock();
        }



        /// <summary>
        /// Draw one LOD level of the scene
        /// </summary>
        /// <param name="currentLodDrawPass"></param>
        /// <param name="drawCockpitInterior"></param>
        internal static void DrawScene_OneLodLevel(MyLodTypeEnum currentLodDrawPass)
        {
            m_renderProfiler.StartProfilingBlock(currentLodDrawPass.ToString());

            m_currentLodDrawPass = currentLodDrawPass;

            switch (currentLodDrawPass)
            {
                case MyLodTypeEnum.LOD0:
                    MyMinerGame.SetRenderTargets(m_GBufferDefaultBinding, null);
                    break;
            }

            MyMinerGame.Static.SetDeviceViewport(MyCamera.Viewport);

            //  We don't need depth buffer for clearing Gbuffer
            DepthStencilState.None.Apply();
            BlendState.Opaque.Apply();
            RasterizerState.CullCounterClockwise.Apply();


            if (currentLodDrawPass == MyLodTypeEnum.LOD0)
            {
                //  Clear depth buffer + stencil because that should be faster than just clearing depth buffer (HW thing...)
                m_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new ColorBGRA(1.0f,0,0,1), 1, 0);
                //  Clear Gbuffer
                MyGuiManager.GetFullscreenQuad().Draw(MyRender.GetEffect(MyEffects.ClearGBuffer));
            }

            //  This compare function "less" is better when drawing normal and LOD1 cells, then z-fighting isn't so visible.
            DepthStencilState.Default.Apply();

            if (!Wireframe)
                RasterizerState.CullCounterClockwise.Apply();
            else
                MyStateObjects.WireframeRasterizerState.Apply();

            m_renderProfiler.StartProfilingBlock("MyRenderStage.LODDrawStart");
            // Render registered modules for this stage
            m_renderProfiler.EndProfilingBlock();

            bool drawNear = !SkipLOD_NEAR && m_currentSetup.EnableNear.HasValue && m_currentSetup.EnableNear.Value;

            if (currentLodDrawPass == MyLodTypeEnum.LOD0) // LOD0
            {
                m_renderProfiler.StartProfilingBlock("DrawNearObjects");
                if (drawNear/* && MyRender.EnableLODBlending*/)
                {
                    DrawScene_OneLodLevel_DrawNearObjects(ShowStencilOptimization, true);
                }
                m_renderProfiler.EndProfilingBlock();

                m_renderProfiler.StartProfilingBlock("Draw(false);");
                m_currentLodDrawPass = MyLodTypeEnum.LOD0;
                if (!SkipLOD_0)
                {
                    DrawScene_OneLodLevel_Draw(false, true);
                }
                m_renderProfiler.EndProfilingBlock();
            }
            else // LOD1
            {
                /*
                if (EnableStencilOptimizationLOD1 && drawNear)
                {
                    DrawScene_OneLodLevel_DrawNearObjects(true, false);
                }
                 */
                         /*
                if (drawNear && !MyRender.EnableLODBlending)
                {
                    DrawScene_OneLodLevel_DrawNearObjects(ShowStencilOptimization, true);
                }          */

                //if (!MyRender.EnableLODBlending)
                {
                    //m_device.DepthStencilState = MyStateObjects.DepthStencil_TestFarObject;
                    MyStateObjects.DepthStencil_TestFarObject.Apply();
                }

                m_currentLodDrawPass = MyLodTypeEnum.LOD1;
                if (!SkipLOD_1)
                {
                    DrawScene_OneLodLevel_Draw(false, true);
                }

                m_renderProfiler.StartProfilingBlock("DrawNearObjects 2");
                /*if (drawNear && !MyRender.EnableLODBlending)
                {
                    DrawScene_OneLodLevel_DrawNearObjects(ShowStencilOptimization, true);
                } */
                m_renderProfiler.EndProfilingBlock();
            }

            //m_device.RasterizerState = RasterizerState.CullCounterClockwise;
            RasterizerState.CullCounterClockwise.Apply();

            // Render registered modules for this stage
            m_renderProfiler.StartProfilingBlock("MyRenderStage.LODDrawEnd");
            DrawRenderModules(MyRenderStage.LODDrawEnd);
            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.EndProfilingBlock();
        }

        internal static void DrawScene_OneLodLevel_Forward(MyLodTypeEnum currentLodDrawPass)
        {
            m_renderProfiler.StartProfilingBlock(currentLodDrawPass.ToString());

            m_currentLodDrawPass = currentLodDrawPass;

            MyMinerGame.Static.SetDeviceViewport(MyCamera.Viewport);

            BlendState.Opaque.Apply();
            DepthStencilState.Default.Apply();

            if (currentLodDrawPass == MyLodTypeEnum.LOD0)
            {
                //                 //  Clear depth buffer + stencil because that should be faster than just clearing depth buffer (HW thing...)
                //                 m_device.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, new Color(0, 0, 0, 0), 1, 0);
                m_renderElementIndex = 0;
            }

            if (!Wireframe)
                RasterizerState.CullCounterClockwise.Apply();
            else
                MyStateObjects.WireframeRasterizerState.Apply();

            // Render registered modules for this stage
            m_renderProfiler.StartProfilingBlock("MyRenderStage.LODDrawStart");
            DrawRenderModules(MyRenderStage.LODDrawStart);
            m_renderProfiler.EndProfilingBlock();

            if (currentLodDrawPass == MyLodTypeEnum.LOD0) // LOD0
            {
                m_renderProfiler.StartProfilingBlock("DrawNearObjects");
                if (!SkipLOD_NEAR && m_currentSetup.EnableNear.HasValue && m_currentSetup.EnableNear.Value)
                {
                    DrawScene_OneLodLevel_DrawNearObjects(ShowStencilOptimization, true);
                }
                m_renderProfiler.EndProfilingBlock();

                m_renderProfiler.StartProfilingBlock("Draw(false);");
                m_currentLodDrawPass = MyLodTypeEnum.LOD0;
                if (!SkipLOD_0)
                {
                    DrawScene_OneLodLevel_Draw(false, true);
                }
                m_renderProfiler.EndProfilingBlock();
            }
            else // LOD1
            {
                m_currentLodDrawPass = MyLodTypeEnum.LOD1;
                if (!SkipLOD_1)
                {
                    DrawScene_OneLodLevel_Draw(false, true);
                }
            }

            RasterizerState.CullCounterClockwise.Apply();

            // Render registered modules for this stage
            m_renderProfiler.StartProfilingBlock("MyRenderStage.LODDrawEnd");
            DrawRenderModules(MyRenderStage.LODDrawEnd);
            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.EndProfilingBlock();
        }

        private static void DrawScene_OneLodLevel_DrawNearObjects(bool drawStencilTechnique, bool collectTransparentElements)
        {
            if (EnableStencilOptimization && !MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                MyStateObjects.DepthStencil_WriteNearObject.Apply();
            }
            /*else if (!MyRender.EnableLODBlending)
            {
                MyMinerGame.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            }   */
            else if (ShowStencilOptimization)
            {
                DepthStencilState.None.Apply();
            }

            m_currentLodDrawPass = MyLodTypeEnum.LOD_NEAR;

            //if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            //    MyCamera.SetNearObjectsClipPlanes(true);

            DrawScene_OneLodLevel_Draw(drawStencilTechnique, collectTransparentElements);
            
            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                //MyCamera.ResetClipPlanes(true);

                // Need to clear only depth
                //m_device.Clear(ClearFlags.ZBuffer, new ColorBGRA(0), 1, 0);
            }
            else
            {
                BlendState.Opaque.Apply();
            }
            DepthStencilState.Default.Apply();
        }

        private static void DrawScene_OneLodLevel_Draw(bool drawStencilTechnique, bool collectTransparentElements)
        {
            if (CurrentRenderSetup.RenderElementsToDraw != null)
            {
                //Draw render elements
                m_renderProfiler.StartProfilingBlock("CurrentRenderSetup.RenderElementsToDraw");
                int ibChangesStats;
                DrawRenderElements(CurrentRenderSetup.RenderElementsToDraw, false, out ibChangesStats);
                m_renderProfiler.EndProfilingBlock();
            }
            else
            {
                // Render are models listed for draw
                m_renderProfiler.StartProfilingBlock("DrawModels()");
                DrawModelsLod(GetCurrentLodDrawPass(), drawStencilTechnique, collectTransparentElements);
                m_renderProfiler.EndProfilingBlock();
            }
        }

        internal static void DrawScene_Transparent()
        {
            int ibChangesStats;
            //Draw sorted render elements
            if (CurrentRenderSetup.TransparentRenderElementsToDraw != null)
                DrawRenderElements(CurrentRenderSetup.TransparentRenderElementsToDraw, false, out ibChangesStats);
            else
                DrawRenderElements(m_transparentRenderElements, false, out ibChangesStats);
        }

        #endregion

        #region Modules

        /// <summary>
        /// Register renderer event handler to make specific behaviour in several render stage
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="handler"></param>
        /// <param name="renderStage"></param>
        /// <param name="priority"></param>
        public static void RegisterRenderModule(MyRenderModuleEnum module, string displayName, DrawEventHandler handler, MyRenderStage renderStage)
        {
            RegisterRenderModule(module, displayName, handler, renderStage, MyRenderConstants.DEFAULT_RENDER_MODULE_PRIORITY, true);
        }

        public static void RegisterRenderModule(MyRenderModuleEnum module, string displayName, DrawEventHandler handler, MyRenderStage renderStage, bool enabled)
        {
            RegisterRenderModule(module, displayName, handler, renderStage, MyRenderConstants.DEFAULT_RENDER_MODULE_PRIORITY, enabled);
        }

        /// <summary>
        /// Register renderer event handler to make specific behaviour in several render stage
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="handler"></param>
        /// <param name="renderStage"></param>
        /// <param name="priority">0 - first item, higher number means lower priority</param>
        public static void RegisterRenderModule(MyRenderModuleEnum module, string displayName, DrawEventHandler handler, MyRenderStage renderStage, int priority, bool enabled)
        {
            Debug.Assert(!m_renderModules[(int)renderStage].Any(x => x.Name == module));

            m_renderModules[(int)renderStage].Add(new MyRenderModuleItem { Name = module, DisplayName = displayName, Priority = priority, Handler = handler, Enabled = enabled });
            m_renderModules[(int)renderStage].Sort((p1, p2) => p1.Priority.CompareTo(p2.Priority));
        }

        /// <summary>
        /// Removes render module from the list
        /// </summary>
        /// <param name="name"></param>
        public static void UnregisterRenderModule(MyRenderModuleEnum name)
        {
            for (int i = 0; i < m_renderModules.Length; i++)
            {
                List<MyRenderModuleItem> modules = m_renderModules[i];
                foreach (MyRenderModuleItem module in modules)
                {
                    if (module.Name == name)
                    {
                        modules.Remove(module);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns enumerator for render modules of current render stage
        /// </summary>
        /// <param name="renderStage"></param>
        /// <returns></returns>
        public static List<MyRenderModuleItem> GetRenderModules(MyRenderStage renderStage)
        {
            return m_renderModules[(int)renderStage];
        }


        public static bool IsModuleEnabled(MyRenderStage stage, MyRenderModuleEnum module)
        {
            if (!(CurrentRenderSetup.EnabledRenderStages == null || CurrentRenderSetup.EnabledRenderStages.Contains(stage)))
                return false;

            List<MyRenderModuleItem> renderModules = m_renderModules[(int)stage];
            if (!(CurrentRenderSetup.EnabledModules == null || CurrentRenderSetup.EnabledModules.Contains(module)))
                return false;

            foreach (var moduleItem in renderModules)
            {
                if (moduleItem.Name == module)
                {
                    return moduleItem.Enabled;
                }
            }
            return false;
        }

        private static void DrawRenderModules(MyRenderStage renderStage)
        {
            if (CurrentRenderSetup.EnabledRenderStages == null || CurrentRenderSetup.EnabledRenderStages.Contains(renderStage))
            {
                List<MyRenderModuleItem> renderModules = m_renderModules[(int)renderStage];
                foreach (MyRenderModuleItem moduleItem in renderModules)
                {
                    if (moduleItem.Enabled && (CurrentRenderSetup.EnabledModules == null || CurrentRenderSetup.EnabledModules.Contains(moduleItem.Name)))
                    {
                        m_renderProfiler.StartProfilingBlock(moduleItem.DisplayName);
                        moduleItem.Handler();
                        m_renderProfiler.EndProfilingBlock();
                    }
                }
            }
        }


        #endregion

        #region Prepare for draw

        static int OCCLUSION_INTERVAL = 4;

        internal static void PrepareRenderObjectsForDraw()
        {
            if (CurrentRenderSetup.RenderElementsToDraw != null)
                return;

            m_entitiesToDebugDraw.Clear();
            m_debugRenderObjectListForDrawLOD0.Clear();
            m_debugRenderObjectListForDrawLOD1.Clear();

            //TODO: Temporary solution, will be removed when rendering will have its own subdivision.
            //For now it is needed because of smallship reflectors (billboards)
            //Comment by Marek: I think we need to increase bbox of ship (due to reflectors) and not bbox of camera.
            //m_cameraFrustum = MyCamera.BoundingBox;
            //m_cameraFrustum.Max += Vector3.One * MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_LENGTH;
            //m_cameraFrustum.Min -= Vector3.One * MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_LENGTH;


            Matrix optProjection = Matrix.CreatePerspectiveFieldOfView(MyCamera.FovWithZoom, MyCamera.AspectRatio, MyCamera.NEAR_PLANE_DISTANCE, CurrentRenderSetup.LodTransitionBackgroundEnd.Value);
            m_cameraFrustum.Matrix = MyCamera.ViewMatrix * optProjection;
            m_cameraFrustumBox = new BoundingBox(new Vector3(float.PositiveInfinity), new Vector3(float.NegativeInfinity));
            BoundingBoxHelper.AddFrustum(ref m_cameraFrustum, ref m_cameraFrustumBox);
            m_cameraPosition = MyCamera.Position;
            m_cameraZoomDivider = MyCamera.ZoomDivider;

            m_cameraFrustumBox.Max += Vector3.One * MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_LENGTH;
            m_cameraFrustumBox.Min -= Vector3.One * MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_LENGTH;

            if (EnableEntitiesPrepareInBackground && m_currentSetup.CallerID.Value == MyRenderCallerEnum.Main)
            {
                WaitUntilEntitiesPrepared();

                m_renderObjectListForDraw.Clear();
                m_renderObjectListForDraw.AddRange(m_renderObjectListForDrawMain);
                
                m_cullObjectListForDraw.Clear();
                m_cullObjectListForDraw.AddRange(m_cullObjectListForDrawMain);
                
                m_renderOcclusionQueries.Clear();
                m_renderOcclusionQueries.AddRange(m_renderOcclusionQueriesMain);
            }
            else
            {
                MyPerformanceCounter.PerCameraDraw.EntitiesOccluded = 0;
                PrepareEntitiesForDraw(ref m_cameraFrustum, m_cameraPosition, m_cameraZoomDivider, MyOcclusionQueryID.MAIN_RENDER, m_renderObjectListForDraw, m_cullObjectListForDraw, m_renderOcclusionQueries, ref MyPerformanceCounter.PerCameraDraw.EntitiesOccluded);
            }
        }


        internal static void PrepareEntitiesForDraw(ref BoundingBox box, MyOcclusionQueryID queryID, List<MyElement> renderObjectListForDraw, List<MyOcclusionQueryIssue> renderOcclusionQueries, ref int occludedItemsStats)
        {
            m_renderProfiler.StartProfilingBlock("PrepareEntitiesForDraw()");

            //Process only big cull object for queries
            renderOcclusionQueries.Clear();

            m_cullingStructure.OverlapAllBoundingBox(ref box, m_cullObjectListForDraw);

            PrepareObjectQueries(queryID, m_cullObjectListForDraw, renderOcclusionQueries, ref occludedItemsStats);

            renderObjectListForDraw.Clear();

            m_renderProfiler.StartProfilingBlock("m_prunningStructure.OverlapAllBoundingBox");
            m_prunningStructure.OverlapAllBoundingBox(ref box, renderObjectListForDraw);

            foreach (MyCullableRenderObject cullableObject in m_cullObjectListForDraw)
            {
                cullableObject.CulledObjects.OverlapAllBoundingBox(ref box, renderObjectListForDraw, 0, false);
            }

            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.EndProfilingBlock();
        }


        internal static void PrepareEntitiesForDraw(ref BoundingFrustum frustum, Vector3 cameraPosition, float cameraZoomDivider, MyOcclusionQueryID queryID, List<MyElement> renderObjectListForDraw, List<MyElement> cullObjectListForDraw, List<MyOcclusionQueryIssue> renderOcclusionQueries, ref int occludedItemsStats)
        {
            m_renderProfiler.StartProfilingBlock("PrepareEntitiesForDrawFr()");
                 
            if (queryID != MyOcclusionQueryID.MAIN_RENDER)
            {
                m_shadowPrunningStructure.OverlapAllFrustum(ref frustum, renderObjectListForDraw);
                m_renderProfiler.EndProfilingBlock();
                return;
            }      

            m_renderProfiler.StartProfilingBlock("m_cullingStructure.OverlapAllFrustum");
            m_cullingStructure.OverlapAllFrustum(ref frustum, cullObjectListForDraw);
            m_renderProfiler.EndProfilingBlock();

            if (renderOcclusionQueries != null)
            {
                //Process only big cull object for queries
                renderOcclusionQueries.Clear();

                m_renderProfiler.StartProfilingBlock("PrepareObjectQueries");
                PrepareObjectQueries(queryID, cullObjectListForDraw, renderOcclusionQueries, ref occludedItemsStats);
                m_renderProfiler.EndProfilingBlock();
            }

            renderObjectListForDraw.Clear();

            m_renderProfiler.StartProfilingBlock("m_prunningStructure.OverlapAllFrustum");
            m_prunningStructure.OverlapAllFrustum(ref frustum, renderObjectListForDraw);
            //AssertRenderObjects(renderObjectListForDraw);
            m_renderProfiler.EndProfilingBlock();


            m_renderProfiler.StartProfilingBlock("Get from cullobjects - part 1");

            //int i = 1;

            if (queryID == MyOcclusionQueryID.MAIN_RENDER)
            {
                foreach (MyCullableRenderObject cullableObject in cullObjectListForDraw)
                {
                    if (frustum.Contains(cullableObject.GetWorldSpaceAABB()) == MinerWarsMath.ContainmentType.Contains)
                    {
                        cullableObject.CulledObjects.GetAll(renderObjectListForDraw, false);
                    }
                    else
                    {
                        cullableObject.CulledObjects.OverlapAllFrustum(ref frustum, renderObjectListForDraw, false);
                      //  i++;
                    } 




                    //AssertRenderObjects(renderObjectListForDraw);
                    //BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
                    //cullableObject.CulledObjects.OverlapAllBoundingBox(ref aabb, renderObjectListForDraw, false);
                    //cullableObject.CulledObjects.GetAll(renderObjectListForDraw, false);                
                }
            }
            else
            {
                foreach (MyCullableRenderObject cullableObject in cullObjectListForDraw)
                {
                    if (frustum.Contains(cullableObject.GetWorldSpaceAABB()) == MinerWarsMath.ContainmentType.Contains)
                    {
                        cullableObject.CulledObjects.GetAll(renderObjectListForDraw, false);
                    }
                    else
                    {
                        cullableObject.CulledObjects.OverlapAllFrustum(ref frustum, renderObjectListForDraw, false);
                    }

                    //cullableObject.CulledObjects.OverlapAllFrustum(ref frustum, renderObjectListForDraw, false);
                    //cullableObject.CulledObjects.GetAll(renderObjectListForDraw, false);                
                }
            }

            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.StartProfilingBlock("Get from cullobjects - part 2");
                
            int c = 0;

            if (queryID == MyOcclusionQueryID.MAIN_RENDER)
            {
                //int ii = 0;
                while (c < renderObjectListForDraw.Count)
                {
                    MyRenderObject ro = renderObjectListForDraw[c] as MyRenderObject;
                    if (!ro.SkipIfTooSmall)
                    {
                        c++;
                        continue;
                    }
                          
                    Vector3 entityPosition = ro.Entity.GetPosition();
                    
                    Vector3.Distance(ref cameraPosition, ref entityPosition, out ro.Distance);
                    ro.Distance = MyCamera.GetDistanceWithFOV(ro.Distance);

                    float cullRatio = ro.Entity is MyStaticAsteroid ? 75 : MyRenderConstants.DISTANCE_CULL_RATIO;

                    if (ro.Entity is MinerWars.AppCode.Game.Entities.SubObjects.MyPrefabLargeWeapon ||
                        ro.Entity is MinerWars.AppCode.Game.Entities.Weapons.MyLargeShipBarrelBase ||
                        ro.Entity is MinerWars.AppCode.Game.Entities.Weapons.MyLargeShipGunBase)
                    {
                        cullRatio = 250;
                    }

                    if (ro.Entity.WorldVolume.Radius < ro.Distance / cullRatio)
                    {
                        renderObjectListForDraw.RemoveAtFast(c);
                        continue;
                    } 


                                      
                    //float f = ro.Distance / (2 * (float)Math.Tan(Math.PI * MyCamera.FieldOfView));

                    //if (f > ro.Entity.LocalVolume.Radius * 100)
                    //{
                    //    renderObjectListForDraw.RemoveAtFast(c);
                    //    continue;
                    //}

                    c++;
                }
            }

            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.EndProfilingBlock();
        }

        [Conditional("DEBUG")]
        static void AssertRenderObjects(List<MyElement> elements)
        {
            foreach (var ro in elements)
            {
                Debug.Assert(!(ro as MyRenderObject).Entity.NearFlag);
            }
        }

        static void PrepareObjectQueries(MyOcclusionQueryID queryID, List<MyElement> cullObjectListForDraw, List<MyOcclusionQueryIssue> renderOcclusionQueries, ref int occludedItemsStats)
        {
            if (queryID != MyOcclusionQueryID.MAIN_RENDER)
                return;

            if (!EnableHWOcclusionQueries)
                return;

            if (!m_currentSetup.EnableOcclusionQueries)
                return;

            int c = 0;
            while (c < cullObjectListForDraw.Count)
            {
                MyCullableRenderObject cullableRenderObject = (MyCullableRenderObject)cullObjectListForDraw[c];

                bool isVisibleFromQuery = false;
                MyOcclusionQueryIssue query = cullableRenderObject.GetQuery(queryID);
                if (query.OcclusionQueryIssued)
                {
                    isVisibleFromQuery = query.OcclusionQueryVisible;

                    bool isComplete = query.OcclusionQuery.IsComplete;
                    
                    if (isComplete)
                    {
                        query.OcclusionQueryIssued = false;

                        isVisibleFromQuery = query.OcclusionQuery.PixelCount > 0;

                        //Holy ATI shit
                        if (query.OcclusionQuery.PixelCount < 0)
                        {
                            isVisibleFromQuery = true;
                        }

                        query.OcclusionQueryVisible = isVisibleFromQuery;

                        
                        //if (m_renderCounter % OCCLUSION_INTERVAL == cullableRenderObject.RenderCounter)
                        if (!query.OcclusionQueryVisible)
                        {
                            renderOcclusionQueries.Add(query);
                        } 
                    }

                    if (!isVisibleFromQuery)
                    {
                        occludedItemsStats += cullableRenderObject.EntitiesContained;
                        cullObjectListForDraw.RemoveAtFast(c);
                        continue;
                    }
                }
                else
                {
                    if (query.OcclusionQueryVisible && m_renderCounter % OCCLUSION_INTERVAL == cullableRenderObject.RenderCounter)
                    {
                        renderOcclusionQueries.Add(cullableRenderObject.GetQuery(queryID));
                    }

                    if (!query.OcclusionQueryVisible)
                    {
                        renderOcclusionQueries.Add(cullableRenderObject.GetQuery(queryID));

                        occludedItemsStats += cullableRenderObject.EntitiesContained;
                        cullObjectListForDraw.RemoveAtFast(c);
                        continue;
                    }
                }

                c++;
            }
        }

        #endregion

        #endregion
    }
}
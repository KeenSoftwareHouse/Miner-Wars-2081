#region Using

using System;
using System.Collections.Generic;
using System.Text;

using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using System.Diagnostics;
using ParallelTasks;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Managers.Session;

using SharpDX;
using SharpDX.Direct3D9;
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
    using MathHelper = MinerWarsMath.MathHelper;


    class MyShadowRenderer : MyShadowRendererBase, IWork
    {
        public static bool RespectCastShadowsFlags = true;

        #region Members

        public const int NumSplits = 4;
        public static readonly float SHADOW_MAX_OFFSET = 10000.0f; //how far we are going to render objects behind cascade box
        private int m_shadowMapCascadeSize;
        private Vector2 m_shadowMapCascadeSizeInv;

        public int ShadowMapCascadeSize
        {
            get
            {
                return m_shadowMapCascadeSize;
            }
            private set
            {
                m_shadowMapCascadeSize = value;
                m_shadowMapCascadeSizeInv.X = 1.0f / ((float)NumSplits * value);
                m_shadowMapCascadeSizeInv.Y = 1.0f / value;
            }
        }

        

        Vector3[] m_frustumCornersVS = new Vector3[8];
        Vector3[] m_frustumCornersWS = new Vector3[8];
        Vector3[] m_frustumCornersLS = new Vector3[8];
        Vector3[] m_farFrustumCornersVS = new Vector3[4];
        Vector3[] m_splitFrustumCornersVS = new Vector3[8];
        MyOrthographicCamera[] m_lightCameras = new MyOrthographicCamera[NumSplits];
        Matrix[] m_lightViewProjectionMatrices = new Matrix[NumSplits];
        Vector2[] m_lightClipPlanes = new Vector2[NumSplits];
        List<MyOcclusionQueryIssue>[] m_occlusionQueriesLists = new List<MyOcclusionQueryIssue>[NumSplits];
        float[] m_splitDepths = new float[NumSplits + 1];

        Vector3 m_sunLightDirection;
        Vector3 m_sunPosition;
        MyPerspectiveCamera m_camera = new MyPerspectiveCamera();

        public MyOcclusionQueryIssue[] m_cascadeQueries = new MyOcclusionQueryIssue[NumSplits];

        int m_frameIndex;
        bool[] m_skip = new bool[NumSplits];
        bool[] m_interleave = new bool[NumSplits];
        bool[] m_visibility = new bool[NumSplits];

        MyRenderTargets m_shadowRenderTarget;
        MyRenderTargets m_shadowDepthTarget;

        /// <summary>
        /// Event that occures when we want prepare shadows for draw
        /// </summary>
        public bool MultiThreaded = false;
        Task m_prepareForDrawTask;


        #endregion

        #region Properties

        public Vector3 SunLightDirection
        {
            get { return m_sunLightDirection; }
            set { m_sunLightDirection = value; }
        }

        public Vector3 SunPosition
        {
            get { return m_sunPosition; }
            set { m_sunPosition = value; }
        }

        #endregion

        /// <summary>
        /// Creates the renderer
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for rendering</param>
        /// <param name="contentManager">The MyCustomContentManager to use for loading content</param>
        public MyShadowRenderer(int shadowMapSize, MyRenderTargets renderTarget, MyRenderTargets depthTarget, bool multiThreaded)
        {
            ShadowMapCascadeSize = shadowMapSize;
            m_shadowRenderTarget = renderTarget;
            m_shadowDepthTarget = depthTarget;

            for (int i = 0; i < NumSplits; i++)
            {
                m_lightCameras[i] = new MyOrthographicCamera(1, 1, 1, 10);
                // Occ queries for shadows are disabled, so save memory by commenting this
                //m_occlusionQueriesLists[i] = new List<MyOcclusionQueryIssue>(1024);
                m_cascadeQueries[i] = new MyOcclusionQueryIssue(null);
                m_visibility[i] = true;
            }

            MultiThreaded = multiThreaded;

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                MultiThreaded = false;
        }

        public void ChangeSize(int newSize)
        {
            ShadowMapCascadeSize = newSize;
        }

        public void UpdateFrustumCorners()
        {
            //Set camera data
            m_camera.FarClip = 1000.0f;
            m_camera.NearClip = 1.0f;
            m_camera.AspectRatio = MyCamera.AspectRatio;
            m_camera.FieldOfView = MyCamera.FovWithZoom;

            //camera.WorldMatrix = Matrix.CreateWorld(MyCamera.Position, MyCamera.ForwardVector, MyCamera.UpVector);
            m_camera.ViewMatrix = MyCamera.ViewMatrix;
            m_camera.ProjectionMatrix = MyCamera.ProjectionMatrix;

            // Get corners of the main camera's bounding frustum
            Matrix cameraTransform, viewMatrix;
            m_camera.GetWorldMatrix(out cameraTransform);
            m_camera.GetViewMatrix(out viewMatrix);
            m_camera.BoundingFrustum.GetCorners(m_frustumCornersWS);
            Vector3.Transform(m_frustumCornersWS, ref viewMatrix, m_frustumCornersVS);
            for (int i = 0; i < 4; i++)
                m_farFrustumCornersVS[i] = m_frustumCornersVS[i + 4];
        }

        public void PrepareFrame()
        {
            m_frameIndex++;
            m_frameIndex %= NumSplits;

            // Need both interleave and skip (interleave is required for occ queries
            m_skip[0] = m_interleave[0] = MyRender.FreezeCascade[0];
            m_skip[1] = m_interleave[1] = MyRender.FreezeCascade[1] || (MyRender.ShadowInterleaving && m_frameIndex % 2 != 0); // on frames 0, 2
            m_skip[2] = m_interleave[2] = MyRender.FreezeCascade[2] || (MyRender.ShadowInterleaving && m_frameIndex % 2 == 0); // on frames 1, 3
            m_skip[3] = m_interleave[3] = MyRender.FreezeCascade[3] || (MyRender.ShadowInterleaving && m_frameIndex % 2 == 0); // on frames 1, 3

            PrepareForDraw();
        }

        void PrepareForDraw()
        {
            if (MultiThreaded)
            {
                if (m_prepareForDrawTask.IsComplete)
                {
                    m_prepareForDrawTask = ParallelTasks.Parallel.Start(this);
                }
            }
        }
 
        public void DoWork()
        {
            PrepareCascadesForDraw();
        }

        public WorkOptions Options
        {
            get { return new WorkOptions() { MaximumThreads = 1 }; }
        }

        public void WaitUntilPrepareForDrawCompleted()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("WaitUntilPrepareForDrawCompleted");

            m_prepareForDrawTask.Wait();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void PrepareCascadesForDraw()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateFrustums");

            SunLightDirection = -MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            SunPosition = 100000 * -SunLightDirection;
            UpdateFrustums();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            // Set casting shadows geometry

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("update entities");

            int frustumIndex = 0;

            foreach (MyOrthographicCamera lightCamera in m_lightCameras)
            {
                if (m_skip[frustumIndex])
                {
                    frustumIndex++;
                    continue;
                }

                m_renderElementsForShadows.Clear();
                m_transparentRenderElementsForShadows.Clear();
                m_castingRenderObjectsUnique.Clear();

                MyRender.GetRenderProfiler().StartProfilingBlock("OverlapAllBoundingBox");
         

                BoundingBox castersBox = lightCamera.BoundingBox; //Cannot use unscaled - incorrect result because of different cascade viewport size
                BoundingFrustum castersFrustum = lightCamera.BoundingFrustum;//Cannot use unscaled - incorrect result because of different cascade viewport size
                //MyRender.PrepareEntitiesForDraw(ref castersBox, (MyOcclusionQueryID)(frustumIndex + 1), m_castingRenderObjects, m_occlusionQueriesLists[frustumIndex], ref MyPerformanceCounter.PerCameraDraw.ShadowEntitiesOccluded[frustumIndex]);
                MyRender.PrepareEntitiesForDraw(ref castersFrustum, lightCamera.Position, 1, (MyOcclusionQueryID)(frustumIndex + 1), m_castingRenderObjects, m_castingCullObjects, null, ref MyPerformanceCounter.PerCameraDraw.ShadowEntitiesOccluded[frustumIndex]);

                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("m_castingRenderObjects");

                int c = 0;
                int skipped = 0;

                while (c < m_castingRenderObjects.Count)
                {
                    MyRenderObject renderObject = (MyRenderObject)m_castingRenderObjects[c];
                    MyEntity entity = renderObject.Entity;

                    //TODO: Appears in Chinese Escape when reloaded several times
                    //System.Diagnostics.Debug.Assert(!entity.NearFlag);

                    if (RespectCastShadowsFlags)
                    {
                        System.Diagnostics.Debug.Assert(!(entity is MyDummyPoint) && !(entity is MinerWars.AppCode.Game.Entities.WayPoints.MyWayPoint));

                        if ((renderObject.ShadowCastUpdateInterval > 0) && ((MyRender.RenderCounter % renderObject.ShadowCastUpdateInterval) == 0))
                        {
                            renderObject.NeedsResolveCastShadow = true;
                            //We have to leave last value, because true when not casting shadow make radiation to ship
                           // renderObject.CastShadow = true;
                        }

                        if (renderObject.NeedsResolveCastShadow)
                        { //Resolve raycast to sun
                            if (renderObject.CastShadowJob == null)
                            {
                                renderObject.CastShadowJob = new MyCastShadowJob(entity);
                                renderObject.CastShadowTask = ParallelTasks.Parallel.Start(renderObject.CastShadowJob);
                            }
                            else
                                if (renderObject.CastShadowTask.IsComplete)
                                {
                                    renderObject.CastShadow = renderObject.CastShadowJob.VisibleFromSun;
                                    renderObject.CastShadowTask = new ParallelTasks.Task();
                                    renderObject.CastShadowJob = null;
                                    renderObject.NeedsResolveCastShadow = false;
                                }
                        }

                        if (!renderObject.NeedsResolveCastShadow && !renderObject.CastShadow)
                        {
                            m_castingRenderObjects.RemoveAtFast(c);
                            skipped++;
                            continue;
                        }
                    }
                    else
                    {
                        renderObject.NeedsResolveCastShadow = true;
                    }
                     
                    /*
                    //Skip object depending on their size and cascade
                    if (entity.WorldVolume.Radius < (frustumIndex + 1) * 5)
                    {
                        m_castingRenderObjects.RemoveAtFast(c);
                        continue;
                    }  
                                        */
                
                    if (entity != null)
                    {
                        if (!m_castingRenderObjectsUnique.Contains(renderObject))
                        {
                            m_castingRenderObjectsUnique.Add(renderObject);

                            if (frustumIndex < MyRenderConstants.RenderQualityProfile.ShadowCascadeLODTreshold)
                            {
                                if (entity is MyVoxelMap)
                                {
                                    //(entity as MyVoxelMap).GetRenderElementsForShadowmap(m_renderElementsForShadows, ref castersBox, castersFrustum, MyLodTypeEnum.LOD0, true);
                                    (entity as MyVoxelMap).GetRenderElementsForShadowmap(m_renderElementsForShadows, renderObject.RenderCellCoord.Value, MyLodTypeEnum.LOD0, true);
                                }

                                else
                                    if (entity.ModelLod0 != null)
                                        MyRender.CollectRenderElementsForShadowmap(m_renderElementsForShadows, m_transparentRenderElementsForShadows,
                                            entity, entity.ModelLod0);

                            }
                            else
                            {

                                if (entity is MyVoxelMap)
                                {
                                    (entity as MyVoxelMap).GetRenderElementsForShadowmap(m_renderElementsForShadows, renderObject.RenderCellCoord.Value, MyLodTypeEnum.LOD1, true);
                                }
                                else
                                    if (entity.ModelLod1 != null)
                                        MyRender.CollectRenderElementsForShadowmap(m_renderElementsForShadows, m_transparentRenderElementsForShadows,
                                            entity, entity.ModelLod1);
                            }
                        }
                    }

                    c++;
                }

                MyRender.GetRenderProfiler().EndProfilingBlock();

                //Sorting VBs to minimize VB switches
                m_renderElementsForShadows.Sort(m_shadowElementsComparer);

                lightCamera.CastingRenderElements = m_renderElementsForShadows;

                MyPerformanceCounter.PerCameraDraw.RenderElementsInShadows += m_renderElementsForShadows.Count;

                frustumIndex++;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void UpdateFrustums()
        {
            //Calculate cascade splits
            m_splitDepths[0] = 1;
            m_splitDepths[1] = 200;
            m_splitDepths[2] = 500;
            m_splitDepths[3] = 2000;
            m_splitDepths[4] = 10000;

            // Calculate data to each split of the cascade
            for (int i = 0; i < NumSplits; i++)
            {
                if (m_skip[i])
                {
                    if (!m_interleave[i])
                    {
                        MyPerformanceCounter.PerCameraDraw.ShadowEntitiesOccluded[i] += m_lightCameras[i].CastingRenderElements.Count;
                    }
                    continue;
                }

                float minZ = m_splitDepths[i];
                float maxZ = m_splitDepths[i + 1];

                if (CalculateFrustum(m_lightCameras[i], m_camera, minZ, maxZ) == null)
                {
                    //Shadow map caching
                    //m_skip[i] = true;                   
                }
            }

            // We'll use these clip planes to determine which split a pixel belongs to
            for (int i = 0; i < NumSplits; i++)
            {            
                m_lightClipPlanes[i].X = -m_splitDepths[i];
                m_lightClipPlanes[i].Y = -m_splitDepths[i + 1];

                m_lightCameras[i].GetViewProjMatrix(out m_lightViewProjectionMatrices[i]);
                m_lightViewProjectionMatrices[i] = Matrix.CreateWorld(MyCamera.Position, MyCamera.ForwardVector, MyCamera.UpVector) * m_lightViewProjectionMatrices[i];
            }
        }

        /// <summary>
        /// Renders a list of models to the shadow map, and returns a surface 
        /// containing the shadow occlusion factor
        /// </summary>
        public void Render()
        {
            int shadowBlock = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyShadowRenderer::Render", ref shadowBlock);

            if (MultiThreaded)
            {
                WaitUntilPrepareForDrawCompleted();
            }
            else
            {
                //PrepareFrame();
                PrepareCascadesForDraw();
            }

            IssueQueriesForCascades();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Set & Clear RT");

            // Set our targets
            MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(m_shadowRenderTarget), MyRender.GetRenderTarget(m_shadowDepthTarget));
            //MyMinerGameDX.Static.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.ZBuffer, new ColorBGRA(1.0f), 1.0f, 0);

            DepthStencilState.Default.Apply();
            RasterizerState.CullCounterClockwise.Apply();
            BlendState.Opaque.Apply();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Render 4 ShadowMaps");

            // Render our scene geometry to each split of the cascade
            for (int i = 0; i < NumSplits; i++)
            {
                if (m_skip[i]) continue;
                if (!m_visibility[i]) continue;

                RenderShadowMap(i);
                //IssueQueriesForShadowMap(i);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //   MyGuiManager.TakeScreenshot();
            MyRender.TakeScreenshot("ShadowMap", MyRender.GetRenderTarget(m_shadowRenderTarget), MyEffectScreenshot.ScreenshotTechniqueEnum.Color);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(shadowBlock);
        }

        /// <summary>
        /// Determines the size of the frustum needed to cover the viewable area,
        /// then creates an appropriate orthographic projection.
        /// </summary>
        /// <param name="light">The directional light to use</param>
        /// <param name="mainCamera">The camera viewing the scene</param>
        protected MyOrthographicCamera CalculateFrustum(MyOrthographicCamera lightCamera, MyPerspectiveCamera mainCamera, float minZ, float maxZ)
        {
            // Shorten the view frustum according to the shadow view distance
            Matrix cameraMatrix;
            mainCamera.GetWorldMatrix(out cameraMatrix);

            Matrix.CreatePerspectiveFieldOfView(mainCamera.FieldOfView, mainCamera.AspectRatio, minZ, maxZ, out lightCamera.CameraSubfrustum);
            Matrix wma;
            mainCamera.GetViewMatrix(out wma);
            lightCamera.CameraSubfrustum = MyCamera.ViewMatrix * lightCamera.CameraSubfrustum;

            for (int i = 0; i < 4; i++)
                m_splitFrustumCornersVS[i] = m_frustumCornersVS[i + 4] * (minZ / mainCamera.FarClip);

            for (int i = 4; i < 8; i++)
                m_splitFrustumCornersVS[i] = m_frustumCornersVS[i] * (maxZ / mainCamera.FarClip);

            Vector3.Transform(m_splitFrustumCornersVS, ref cameraMatrix, m_frustumCornersWS);

            // Position the shadow-caster camera so that it's looking at the centroid,
            // and backed up in the direction of the sunlight

            //Toto se nemeni per frame!
            Matrix viewMatrix = Matrix.CreateLookAt(Vector3.Zero - (m_sunLightDirection * mainCamera.FarClip), Vector3.Zero, new Vector3(0, 1, 0));

            // Determine the position of the frustum corners in light space
            Vector3.Transform(m_frustumCornersWS, ref viewMatrix, m_frustumCornersLS);

            // Calculate an orthographic projection by sizing a bounding box
            // to the frustum coordinates in light space
            Vector3 mins = m_frustumCornersLS[0];
            Vector3 maxes = m_frustumCornersLS[0];
            for (int i = 0; i < 8; i++)
            {
                if (m_frustumCornersLS[i].X > maxes.X)
                    maxes.X = m_frustumCornersLS[i].X;
                else if (m_frustumCornersLS[i].X < mins.X)
                    mins.X = m_frustumCornersLS[i].X;
                if (m_frustumCornersLS[i].Y > maxes.Y)
                    maxes.Y = m_frustumCornersLS[i].Y;
                else if (m_frustumCornersLS[i].Y < mins.Y)
                    mins.Y = m_frustumCornersLS[i].Y;
                if (m_frustumCornersLS[i].Z > maxes.Z)
                    maxes.Z = m_frustumCornersLS[i].Z;
                else if (m_frustumCornersLS[i].Z < mins.Z)
                    mins.Z = m_frustumCornersLS[i].Z;
            }

            // Update an orthographic camera for collision detection
            lightCamera.UpdateUnscaled(mins.X, maxes.X, mins.Y, maxes.Y, -maxes.Z - SHADOW_MAX_OFFSET, -mins.Z);
            lightCamera.SetViewMatrixUnscaled(ref viewMatrix);

            // We snap the camera to 1 pixel increments so that moving the camera does not cause the shadows to jitter.
            // This is a matter of integer dividing by the world space size of a texel
            float diagonalLength = (m_frustumCornersWS[0] - m_frustumCornersWS[6]).Length();

            //Make bigger box - ensure rotation and movement stabilization
            diagonalLength = MyMath.GetNearestBiggerPowerOfTwo(diagonalLength);

            float worldsUnitsPerTexel = diagonalLength / (float)ShadowMapCascadeSize;

            Vector3 vBorderOffset = (new Vector3(diagonalLength, diagonalLength, diagonalLength) - (maxes - mins)) * 0.5f;
            maxes += vBorderOffset;
            mins -= vBorderOffset;

            mins /= worldsUnitsPerTexel;
            mins.X = (float)Math.Floor(mins.X);
            mins.Y = (float)Math.Floor(mins.Y);
            mins.Z = (float)Math.Floor(mins.Z);
            mins *= worldsUnitsPerTexel;

            maxes /= worldsUnitsPerTexel;
            maxes.X = (float)Math.Floor(maxes.X);
            maxes.Y = (float)Math.Floor(maxes.Y);
            maxes.Z = (float)Math.Floor(maxes.Z);
            maxes *= worldsUnitsPerTexel;


            /*
            Matrix proj;
            Matrix.CreateOrthographicOffCenter(mins.X, maxes.X, mins.Y, maxes.Y, -maxes.Z - SHADOW_MAX_OFFSET, -mins.Z, out proj);
            
            if (MyUtils.IsEqual(lightCamera.ProjectionMatrix, proj))
            {   //cache
                return null;
            } */


            // Update an orthographic camera for use as a shadow caster
            lightCamera.Update(mins.X, maxes.X, mins.Y, maxes.Y, -maxes.Z - SHADOW_MAX_OFFSET, -mins.Z);


            lightCamera.SetViewMatrix(ref viewMatrix);

            return lightCamera;
        }


        void PrepareViewportForCascade(int splitIndex)
        {
            // Set the viewport for the current split   
            Viewport splitViewport = new Viewport();
            splitViewport.MinDepth = 0;
            splitViewport.MaxDepth = 1;
            splitViewport.Width = ShadowMapCascadeSize;
            splitViewport.Height = ShadowMapCascadeSize;
            splitViewport.X = splitIndex * ShadowMapCascadeSize;
            splitViewport.Y = 0;
            //Must be here because otherwise it crasher after resolution change
            MyMinerGame.Static.SetDeviceViewport(splitViewport);
        }


        /// <summary>
        /// Renders the shadow map using the orthographic camera created in
        /// CalculateFrustum.
        /// </summary>
        /// <param name="modelList">The list of models to be rendered</param>        
        protected void RenderShadowMap(int splitIndex)
        {
            PrepareViewportForCascade(splitIndex);

            // Set up the effect
            MyEffectShadowMap shadowMapEffect = MyRender.GetEffect(MyEffects.ShadowMap) as MinerWars.AppCode.Game.Effects.MyEffectShadowMap;

            // Clear shadow map
            shadowMapEffect.SetTechnique(MyEffectShadowMap.ShadowTechnique.Clear);
            MyGuiManager.GetFullscreenQuad().Draw(shadowMapEffect);

            shadowMapEffect.SetViewProjMatrix(m_lightCameras[splitIndex].ViewProjectionMatrix);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("draw elements");
            // Draw the models
            DrawElements(m_lightCameras[splitIndex].CastingRenderElements, shadowMapEffect, false, splitIndex);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        Vector3[] frustum = new Vector3[8];
        BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);

        public void IssueQueriesForCascades()
        {             /*
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyShadowRenderer::IssueQueriesForCascades");

            bool useOccQueries = MyRender.EnableHWOcclusionQueriesForShadows && MyRender.CurrentRenderSetup.EnableOcclusionQueries;

            if (!useOccQueries)
            {
                for (int i = 0; i < NumSplits; i++)
                {
                    m_visibility[i] = true;
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return;
            }

            Device device = MyMinerGame.Static.GraphicsDevice;
            BlendState oldBlendState = BlendState.Current;
            MyStateObjects.DisabledColorChannels_BlendState.Apply();

            //generate and draw bounding box of our renderCell in occlusion query 
            //device.BlendState = MyStateObjects.DisabledColorChannels_BlendState;
            MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0), null);

            Vector3 campos = MyCamera.Position;

            RasterizerState.CullNone.Apply();

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DepthStencilState.DepthRead.Apply();
            else
                DepthStencilState.None.Apply();

            for (int i = 1; i < NumSplits; i++)
            {
                if (m_interleave[i]) continue;

                MyPerformanceCounter.PerCameraDraw.QueriesCount++;

                var queryIssue = m_cascadeQueries[i];

                if (queryIssue.OcclusionQueryIssued)
                {
                    if (queryIssue.OcclusionQuery.IsComplete)
                    {
                        m_visibility[i] = queryIssue.OcclusionQuery.PixelCount > 0;
                        queryIssue.OcclusionQueryIssued = false;
                    }
                    continue;
                }

                queryIssue.OcclusionQueryIssued = true;

                if (queryIssue.OcclusionQuery == null) 
                    queryIssue.OcclusionQuery = new MyOcclusionQuery(device);

                cameraFrustum.Matrix = m_lightCameras[i].CameraSubfrustum;

                cameraFrustum.GetCorners(frustum);

                var tmp = frustum[3];
                frustum[3] = frustum[2];
                frustum[2] = tmp;

                queryIssue.OcclusionQuery.Begin();
                MySimpleObjectDraw.OcclusionPlaneDraw(frustum);
                queryIssue.OcclusionQuery.End();
            }

            oldBlendState.Apply();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();*/
        }


        public void SetupShadowBaseEffect(MyEffectShadowBase effect)
        {
            //Matrix cameraTransform = Matrix.Invert(MyCamera.ViewMatrix);
            //effect.SetInvViewMatrix(cameraTransform);

            effect.SetLightViewProjMatrices(m_lightViewProjectionMatrices);
            effect.SetClipPlanes(m_lightClipPlanes);

            effect.SetShadowMapSize(new Vector4(ShadowMapCascadeSize * NumSplits, ShadowMapCascadeSize, m_shadowMapCascadeSizeInv.X, m_shadowMapCascadeSizeInv.Y));
            effect.SetShadowMap(MyRender.GetRenderTarget(m_shadowRenderTarget));
        }

        public Vector3[] GetFrustumCorners()
        {
            return m_farFrustumCornersVS;
        }

        void DebugDrawFrustum(Matrix camera, Color color)
        {
            BoundingFrustum frustum = new BoundingFrustum(camera);
            MyDebugDraw.DrawBoundingFrustum(frustum, color);
        }

        static readonly Color[] frustumColors = new Color[]
        {
            new Color(1.0f,0.0f,0.0f),
            new Color(0.0f,1.0f,0.0f),
            new Color(0.0f,0.0f,1.0f),
            new Color(1.0f,1.0f,0.0f),
        };

        Matrix[] frustumMatrices = new Matrix[4];
        Matrix mainCamera;

        public void DebugDraw()
        {        
            return;
            MyStateObjects.WireframeRasterizerState.Apply();
            for (int i = 0; i < NumSplits; i++)
            {
                cameraFrustum.Matrix = m_lightCameras[i].CameraSubfrustum;
                cameraFrustum.GetCorners(frustum);

                var tmp = frustum[3];
                frustum[3] = frustum[2];
                frustum[2] = tmp;

                //MyDebugDraw.DrawBoundingFrustum(cameraFrustum, frustumColors[i]);
                MySimpleObjectDraw.OcclusionPlaneDraw(frustum);
                //MyDebugDraw.DrawTriangle(frustum[0], frustum[1], frustum[2], frustumColors[i], frustumColors[i], frustumColors[i]);
                //MyDebugDraw.DrawTriangle(frustum[1], frustum[2], frustum[3], frustumColors[i], frustumColors[i], frustumColors[i]);
            }

            return;
                   
            bool update = false;

            if (MyRender.CurrentRenderSetup.CallerID.Value == MyRenderCallerEnum.Main)
            {
                if (update)
                {
                    mainCamera = MyCamera.GetBoundingFrustum().Matrix;
                }

                for (int i = 0; i < NumSplits; i++)
                {
                    if (update)
                    {
                        Vector4 c = frustumColors[i].ToVector4();

                        //MyDebugDraw.DrawAABBLowRes(ref box, ref c, 1);
                        //BoundingFrustum bf = new BoundingFrustum();

                        //frustumMatrices[i] = m_lightCameras[i].CameraSubfrustum;
                        frustumMatrices[i] = m_lightCameras[i].BoundingFrustum.Matrix;
                    }

                    DebugDrawFrustum(frustumMatrices[i], frustumColors[i]);


                    Vector4 cc = frustumColors[i].ToVector4();

                    BoundingFrustum frma = new BoundingFrustum(frustumMatrices[i]);
                    MyRender.PrepareEntitiesForDraw(ref frma, Vector3.Zero, 0, (MyOcclusionQueryID)(i + 1), m_castingRenderObjects, m_castingCullObjects, m_occlusionQueriesLists[i], ref MyPerformanceCounter.PerCameraDraw.ShadowEntitiesOccluded[i]);
                    BoundingBox aabbFr = new BoundingBox();
                    aabbFr = aabbFr.CreateInvalid();
                    foreach (MyRenderObject ro in m_castingRenderObjects)
                    {
                        BoundingBox vv = ro.GetWorldSpaceAABB();
                        //MyDebugDraw.DrawAABBLowRes(ref vv, ref cc, 1);
                        aabbFr = aabbFr.Include(ref vv);
                    }


                    //MyDebugDraw.DrawAABBLowRes(ref aabbFr, ref cc, 1);
                }

                // DebugDrawFrustum(mainCamera, new Color(1.0f, 1.0f, 1.0f));
            }

        }
    }
}

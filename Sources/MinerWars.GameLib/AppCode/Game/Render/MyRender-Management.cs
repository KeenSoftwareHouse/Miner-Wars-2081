#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Threading;

using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using ParallelTasks;

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

    static partial class MyRender
    {

        #region Render object management

        public static void PushRenderSetupAndApply(MyRenderSetup setup, ref MyRenderSetup storePreviousSetup)
        {
            PushRenderSetup(setup);
            ApplySetupStack(storePreviousSetup);
        }

        public static void PopRenderSetupAndRevert(MyRenderSetup previousSetup)
        {
            PopRenderSetup();
            ApplySetup(previousSetup);
        }

        public static void PushRenderSetup(MyRenderSetup setup)
        {
            m_renderSetupStack.Add(setup);
        }

        public static void PopRenderSetup()
        {
            m_renderSetupStack.RemoveAt(m_renderSetupStack.Count - 1);
        }

        public static void ApplyBackupSetup()
        {
            ApplySetup(m_backupSetup);
        }

        private static void ApplySetup(MyRenderSetup setup)
        {
            MyCamera.EnableZoom = setup.EnableZoom;
            if (setup.CameraPosition.HasValue)
            {
                MyCamera.SetPosition(setup.CameraPosition.Value);
            }
            if (setup.AspectRatio.HasValue)
            {
                MyCamera.ForwardAspectRatio = setup.AspectRatio.Value;
            }
            if (setup.Fov.HasValue && MyCamera.FieldOfView != setup.Fov.Value)
            {
                MyCamera.FieldOfView = setup.Fov.Value;
            }
            if (setup.ViewMatrix.HasValue && setup.ViewMatrix != MyUtils.ZeroMatrix)
            {
                MyCamera.SetViewMatrix(setup.ViewMatrix.Value);
            }
            if (setup.Viewport.HasValue)
            {
                MyCamera.ForwardViewport = setup.Viewport.Value;
            }
            if (setup.Fov.HasValue && MyCamera.FieldOfView != setup.Fov.Value)
            {
                // When custom FOV set, zoom will be disabled
                MyCamera.ChangeFov(setup.Fov.Value);
            }
            if (setup.ProjectionMatrix.HasValue)
            {
                MyCamera.SetCustomProjection(setup.ProjectionMatrix.Value);
            }

            if (setup.RenderTargets != null && setup.RenderTargets.Length > 0)
            {
                Texture rt = setup.RenderTargets[0];
                if (rt != null)
                {
                    MyMinerGame.SetRenderTarget(rt, setup.DepthTarget);
                }
                 else
                    MyMinerGame.SetRenderTarget(null, null);
            }
            else
                MyMinerGame.SetRenderTarget(null, null);

            m_currentSetup.RenderTargets = setup.RenderTargets;

            MyCamera.EnableForward();
        }

        private static void ApplySetupStack(MyRenderSetup storeBackup)
        {
            if (storeBackup != null)
            {
                if (MyCamera.ViewMatrix.Left != Vector3.Zero)
                {
                    storeBackup.CameraPosition = MyCamera.Position;
                    storeBackup.AspectRatio = MyCamera.ForwardAspectRatio;
                    storeBackup.Fov = MyCamera.FieldOfView;
                    storeBackup.ViewMatrix = MyCamera.ViewMatrix;
                    storeBackup.ProjectionMatrix = MyCamera.ProjectionMatrix;
                    storeBackup.Viewport = MyCamera.ForwardViewport;
                    storeBackup.EnableZoom = MyCamera.EnableZoom;
                    storeBackup.RenderTargets = m_currentSetup.RenderTargets;
                }
            }

            if (MyCamera.ViewMatrix.Left != Vector3.Zero)
            {
                m_currentSetup.ViewMatrix = MyCamera.ViewMatrix;

            }

            // Set default values
            m_currentSetup.CallerID = MyRenderCallerEnum.Main;

            m_currentSetup.RenderTargets = null;

            m_currentSetup.CameraPosition = MyCamera.Position;
            m_currentSetup.AspectRatio = MyCamera.ForwardAspectRatio;
            m_currentSetup.Fov = null;
            m_currentSetup.Viewport = MyCamera.ForwardViewport;
            m_currentSetup.ProjectionMatrix = null;
            m_currentSetup.EnableZoom = true;
            m_currentSetup.FogMultiplierMult = 1;
            m_currentSetup.DepthToAlpha = false;

            m_currentSetup.LodTransitionNear = MyCamera.GetLodTransitionDistanceNear();
            m_currentSetup.LodTransitionFar = MyCamera.GetLodTransitionDistanceFar();
            m_currentSetup.LodTransitionBackgroundStart = MyCamera.GetLodTransitionDistanceBackgroundStart();
            m_currentSetup.LodTransitionBackgroundEnd = MyCamera.GetLodTransitionDistanceBackgroundEnd();

            m_currentSetup.EnableHDR = true;
            m_currentSetup.EnableLights = true;
            m_currentSetup.EnableSun = true;
            m_currentSetup.ShadowRenderer = m_shadowRenderer; // Default shadow render
            m_currentSetup.EnableShadowInterleaving = ShadowInterleaving;
            m_currentSetup.EnableSmallLights = true;
            m_currentSetup.EnableSmallLightShadows = true;
            m_currentSetup.EnableDebugHelpers = true;
            m_currentSetup.EnableEnvironmentMapping = true;
            m_currentSetup.EnableNear = true;
            m_currentSetup.EnableOcclusionQueries = true;

            m_currentSetup.BackgroundColor = null;

            m_currentSetup.EnabledModules = null;
            m_currentSetup.EnabledPostprocesses = null;
            m_currentSetup.EnabledRenderStages = null;

            m_currentSetup.LightsToUse = null;
            m_currentSetup.RenderElementsToDraw = null;
            m_currentSetup.TransparentRenderElementsToDraw = null;

            foreach (var setup in m_renderSetupStack)
            {
                AggregateSetup(setup);
            }

            ApplySetup(m_currentSetup);
        }

        private static void AggregateSetup(MyRenderSetup setup)
        {
            if (setup.CallerID != null)
            {
                m_currentSetup.CallerID = setup.CallerID;
            }
            else
            {
                Debug.Assert(false, "CallerID has to be set in render setup.");
            }

            if (setup.RenderTargets != null)
            {
                m_currentSetup.RenderTargets = setup.RenderTargets;
            }

            if (setup.CameraPosition.HasValue)
            {
                m_currentSetup.CameraPosition = setup.CameraPosition;
            }

            if (setup.ViewMatrix.HasValue)
            {
                m_currentSetup.ViewMatrix = setup.ViewMatrix;
            }

            if (setup.ProjectionMatrix.HasValue)
            {
                m_currentSetup.ProjectionMatrix = setup.ProjectionMatrix;
            }

            if (setup.Fov.HasValue)
            {
                m_currentSetup.Fov = setup.Fov;
            }

            if (setup.AspectRatio.HasValue)
            {
                m_currentSetup.AspectRatio = setup.AspectRatio;
            }

            if (setup.Viewport.HasValue)
            {
                m_currentSetup.Viewport = setup.Viewport;
            }

            if (setup.LodTransitionNear.HasValue)
            {
                m_currentSetup.LodTransitionNear = setup.LodTransitionNear;
            }

            if (setup.LodTransitionFar.HasValue)
            {
                m_currentSetup.LodTransitionFar = setup.LodTransitionFar;
            }

            if (setup.LodTransitionBackgroundStart.HasValue)
            {
                m_currentSetup.LodTransitionBackgroundStart = setup.LodTransitionBackgroundStart;
            }

            if (setup.LodTransitionBackgroundEnd.HasValue)
            {
                m_currentSetup.LodTransitionBackgroundEnd = setup.LodTransitionBackgroundEnd;
            }

            if (setup.EnableHDR.HasValue)
            {
                m_currentSetup.EnableHDR = setup.EnableHDR;
            }

            if (setup.EnableLights.HasValue)
            {
                m_currentSetup.EnableLights = setup.EnableLights;
            }

            if (setup.EnableSun.HasValue)
            {
                m_currentSetup.EnableSun = setup.EnableSun;
            }

            // Special case...when no shadow render specified, no shadows are rendered
            m_currentSetup.ShadowRenderer = setup.ShadowRenderer;
            m_currentSetup.FogMultiplierMult = setup.FogMultiplierMult;
            m_currentSetup.DepthToAlpha = setup.DepthToAlpha;

            if (setup.EnableShadowInterleaving.HasValue)
            {
                m_currentSetup.EnableShadowInterleaving = setup.EnableShadowInterleaving;
            }

            if (setup.EnableSmallLights.HasValue)
            {
                m_currentSetup.EnableSmallLights = setup.EnableSmallLights;
            }

            if (setup.EnableSmallLightShadows.HasValue)
            {
                m_currentSetup.EnableSmallLightShadows = setup.EnableSmallLightShadows;
            }

            if (setup.EnableDebugHelpers.HasValue)
            {
                m_currentSetup.EnableDebugHelpers = setup.EnableDebugHelpers;
            }

            if (setup.EnableEnvironmentMapping.HasValue)
            {
                m_currentSetup.EnableEnvironmentMapping = setup.EnableEnvironmentMapping;
            }

            if (setup.EnableNear.HasValue)
            {
                m_currentSetup.EnableNear = setup.EnableNear;
            }

            if (setup.BackgroundColor.HasValue)
            {
                m_currentSetup.BackgroundColor = setup.BackgroundColor;
            }

            if (setup.RenderElementsToDraw != null)
            {
                m_currentSetup.RenderElementsToDraw = setup.RenderElementsToDraw;
            }

            if (setup.TransparentRenderElementsToDraw != null)
            {
                m_currentSetup.TransparentRenderElementsToDraw = setup.TransparentRenderElementsToDraw;
            }

            if (setup.LightsToUse != null)
            {
                m_currentSetup.LightsToUse = setup.LightsToUse;
            }

            m_currentSetup.EnableOcclusionQueries = setup.EnableOcclusionQueries;
            m_currentSetup.EnableZoom = setup.EnableZoom;

            if (setup.EnabledModules != null)
            {
                if (m_currentSetup.EnabledModules == null)
                {
                    m_currentSetup.EnabledModules = setup.EnabledModules;
                }
                else
                {
                    m_currentSetup.EnabledModules.IntersectWith(setup.EnabledModules);
                }
            }

            if (setup.EnabledPostprocesses != null)
            {
                if (m_currentSetup.EnabledPostprocesses == null)
                {
                    m_currentSetup.EnabledPostprocesses = setup.EnabledPostprocesses;
                }
                else
                {
                    m_currentSetup.EnabledPostprocesses.IntersectWith(setup.EnabledPostprocesses);
                }
            }

            if (setup.EnabledRenderStages != null)
            {
                if (m_currentSetup.EnabledRenderStages == null)
                {
                    m_currentSetup.EnabledRenderStages = setup.EnabledRenderStages;
                }
                else
                {
                    m_currentSetup.EnabledRenderStages.IntersectWith(setup.EnabledRenderStages);
                }
            }

            //m_currentSetup.RenderTargets = setup.RenderTargets;
        }

        #endregion

        #region Prunning structure

        static int m_renderObjectIncrementalCounter = 0;

        public static void AddRenderObject(MyRenderObject renderObject, bool rebalance = true)
        {
            if (renderObject.Entity is MyVoxelMap)
            {
            }

            if (renderObject.Entity != null && renderObject.Entity.NearFlag && !m_nearObjects.Contains(renderObject))
            {
                m_nearObjects.Add(renderObject);
            }
            else if (renderObject.ProxyData == MyElement.PROXY_UNASSIGNED)
            {
                BoundingBox aabb = renderObject.GetWorldSpaceAABB();
                renderObject.SetDirty();

                if (renderObject is MyCullableRenderObject)
                {
                    MyCullableRenderObject cullableObject = renderObject as MyCullableRenderObject;

                    renderObject.ProxyData = m_cullingStructure.AddProxy(ref aabb, renderObject, 0);

                    //Move all existing included proxies to cull objects
                    m_prunningStructure.OverlapAllBoundingBox(ref aabb, m_renderObjectListForDraw);

                    foreach (MyRenderObject ro in m_renderObjectListForDraw)
                    {
                        System.Diagnostics.Debug.Assert(!(ro is MyCullableRenderObject));
                        Debug.Assert(!ro.Entity.NearFlag);

                        BoundingBox roAABB = ro.GetWorldSpaceAABB();

                        if (ro.CullObject == null && aabb.Contains(roAABB) == MinerWarsMath.ContainmentType.Contains)
                        {
                            RemoveRenderObject(ro, false);
                            ro.ProxyData = cullableObject.CulledObjects.AddProxy(ref roAABB, ro, 0);
                            cullableObject.EntitiesContained++;
                            ro.CullObject = cullableObject;
                        }
                    }

                    System.Diagnostics.Debug.Assert(cullableObject.GetQuery(MyOcclusionQueryID.MAIN_RENDER).OcclusionQuery == null);
                    cullableObject.InitQueries();

                    cullableObject.RenderCounter = m_renderObjectIncrementalCounter % OCCLUSION_INTERVAL;
                    m_renderObjectIncrementalCounter++;
                }
                else
                {
                    m_renderProfiler.StartProfilingBlock("Overlap");
                    //find potential cull objects and move render object to it if it is fully included
                    m_cullingStructure.OverlapAllBoundingBox(ref aabb, m_cullObjectListForDraw);
                    m_renderProfiler.EndProfilingBlock();
                    bool contained = false;
                    MyCullableRenderObject mostSuitableCO = null;
                    float minVolume = float.MaxValue;
                    foreach (MyCullableRenderObject co in m_cullObjectListForDraw)
                    {
                        if (co.GetWorldSpaceAABB().Contains(aabb) == MinerWarsMath.ContainmentType.Contains)
                        {
                            float volume = co.GetWorldSpaceAABB().Volume();
                            if (volume < minVolume)
                            {
                                minVolume = volume;
                                mostSuitableCO = co;
                            }
                        }
                    }

                    if (mostSuitableCO != null)
                    {
                        m_renderProfiler.StartProfilingBlock("AddProxy");
                        renderObject.ProxyData = mostSuitableCO.CulledObjects.AddProxy(ref aabb, renderObject, 0, rebalance);
                        m_renderProfiler.EndProfilingBlock();
                        mostSuitableCO.EntitiesContained++;
                        renderObject.CullObject = mostSuitableCO;
                        contained = true;
                    }

                    if (!contained)
                    {
                        renderObject.ProxyData = m_prunningStructure.AddProxy(ref aabb, renderObject, 0, rebalance);
                        renderObject.CullObject = null;
                    }

                    if (renderObject.Entity.CastShadows)
                        AddShadowRenderObject(renderObject, rebalance);
                }
            }
        }

        public static void AddShadowRenderObject(MyRenderObject renderObject, bool rebalance = true)
        {
            if (renderObject.ShadowProxyData != MyElement.PROXY_UNASSIGNED)
                RemoveShadowRenderObject(renderObject);

            if (renderObject.ShadowProxyData == MyElement.PROXY_UNASSIGNED && renderObject.Entity != null && renderObject.Entity.CastShadows)
            {
                BoundingBox aabb = renderObject.GetWorldSpaceAABB();
                renderObject.SetDirty();

                renderObject.ShadowProxyData = m_shadowPrunningStructure.AddProxy(ref aabb, renderObject, 0, rebalance);
            }
        }

        public static void RemoveRenderObject(MyRenderObject renderObject, bool includeShadowObject = true)
        {
            if (m_nearObjects.Contains(renderObject))
            {
                m_nearObjects.Remove(renderObject);
            }
            else if (renderObject.ProxyData != MyElement.PROXY_UNASSIGNED)
            {
                if (renderObject is MyCullableRenderObject)
                {
                    MyCullableRenderObject cullableObject = renderObject as MyCullableRenderObject;

                    //Move all existing included objects to render prunning structure
                    BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
                    cullableObject.CulledObjects.OverlapAllBoundingBox(ref aabb, m_renderObjectListForDraw);
                    foreach (MyRenderObject ro in m_renderObjectListForDraw)
                    {
                        Debug.Assert(!ro.Entity.NearFlag);
                        cullableObject.CulledObjects.RemoveProxy(ro.ProxyData);
                        BoundingBox roAABB = ro.GetWorldSpaceAABB();
                        ro.ProxyData = m_prunningStructure.AddProxy(ref roAABB, ro, 0);
                        ro.CullObject = null;
                    }

                    //destroy cull object
                    m_cullingStructure.RemoveProxy(cullableObject.ProxyData);
                    cullableObject.ProxyData = MyElement.PROXY_UNASSIGNED;

                    //return query to pool
                    cullableObject.DestroyQueries();
                }
                else
                {
                    if (renderObject.CullObject != null)
                    {
                        renderObject.CullObject.CulledObjects.RemoveProxy(renderObject.ProxyData);
                        renderObject.CullObject.EntitiesContained--;
                        renderObject.ProxyData = MyElement.PROXY_UNASSIGNED;
                        renderObject.CullObject = null;
                    }
                    else
                    {
                        m_prunningStructure.RemoveProxy(renderObject.ProxyData);
                        renderObject.ProxyData = MyElement.PROXY_UNASSIGNED;
                    }
                }
            }

            if (includeShadowObject)
                RemoveShadowRenderObject(renderObject);
        }

        public static void RemoveShadowRenderObject(MyRenderObject renderObject)
        {
            if (renderObject.ShadowProxyData != MyElement.PROXY_UNASSIGNED)
            {
                m_shadowPrunningStructure.RemoveProxy(renderObject.ShadowProxyData);
                renderObject.ShadowProxyData = MyElement.PROXY_UNASSIGNED;
            }
        }

        public static void MoveRenderObject(MyRenderObject renderObject)
        {
            /*
            if (renderObject.ProxyData != MyElement.PROXY_UNASSIGNED)
            {
                BoundingBox aabb = renderObject.GetWorldSpaceAABB();
                m_prunningStructure.MoveProxy(renderObject.ProxyData, ref aabb, Vector3.Zero);
            } */

            System.Diagnostics.Debug.Assert(renderObject.ProxyData != MyElement.PROXY_UNASSIGNED);

            BoundingBox aabb = renderObject.GetWorldSpaceAABB();

            if (renderObject is MyCullableRenderObject)
            {
                m_cullingStructure.MoveProxy(renderObject.ProxyData, ref aabb, Vector3.Zero);
            }
            else
            {

                if (renderObject.CullObject != null)
                {
                    //Cannot use move because cullobject aabb then does not fit
                    //renderObject.CullObject.CulledObjects.MoveProxy(renderObject.ProxyData, ref aabb, Vector3.Zero);
                    RemoveRenderObject(renderObject, false);

                    renderObject.SetDirty();
                    renderObject.ProxyData = m_prunningStructure.AddProxy(ref aabb, renderObject, 0, true);
                    renderObject.CullObject = null;
                }
                else
                {
                    m_prunningStructure.MoveProxy(renderObject.ProxyData, ref aabb, Vector3.Zero);
                }

                if (renderObject.ShadowProxyData != MyElement.PROXY_UNASSIGNED)
                {
                    m_shadowPrunningStructure.MoveProxy(renderObject.ShadowProxyData, ref aabb, Vector3.Zero);
                }
            }
        }

#if RENDER_PROFILING
        public static int RenderObjectUpdatesCounter = 0;
#endif 

        public static void UpdateRenderObject(MyRenderObject renderObject, bool sortIntoCullobjects = false)
        {
            /*
            if (!renderObject.Entity.EntityId.HasValue)
                return;

            string ts = renderObject.Entity.EntityId.Value.NumericValue.ToString() + " " + renderObject.Entity.GetType().Name.ToString();
            if (!m_typesStats.ContainsKey(ts))
                m_typesStats.Add(ts, 0);
            m_typesStats[ts]++;

            if (renderObject.Entity is MyPrefab)
            {
                MyPrefab prefab = renderObject.Entity as MyPrefab;

                string pt = prefab.PrefabType.ToString();
                if (!m_prefabStats.ContainsKey(pt))
                    m_prefabStats.Add(pt, 0);
                m_prefabStats[pt]++;
            }
                */

            m_renderProfiler.StartProfilingBlock("UpdateRenderObject");
            if (renderObject.ProxyData != MyElement.PROXY_UNASSIGNED)
            {
#if RENDER_PROFILING
                RenderObjectUpdatesCounter++;
#endif


                if (sortIntoCullobjects)
                {
                    m_renderProfiler.StartProfilingBlock("RemoveRenderObject");
                    RemoveRenderObject(renderObject);
                    m_renderProfiler.EndProfilingBlock();
                    m_renderProfiler.StartProfilingBlock("AddRenderObject");
                    AddRenderObject(renderObject, false);
                    m_renderProfiler.EndProfilingBlock();
                }
                else
                {
                    m_renderProfiler.StartProfilingBlock("MoveRenderObject");
                    MoveRenderObject(renderObject);
                    m_renderProfiler.EndProfilingBlock();
                }
                             
#if RENDER_PROFILING
                m_renderProfiler.ProfileCustomValue("Updated objects count", RenderObjectUpdatesCounter);
#endif
            }

            m_renderProfiler.EndProfilingBlock();
        }        

        private static void DebugDrawPrunning()
        {
/*
            return;
            BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            List<MyElement> list = new List<MyElement>();
            ((MyDynamicAABBTree)MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure()).Rebalance(256);
            MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure().OverlapAllBoundingBox(ref aabb, list, true);
            
            foreach (Physics.MyElement element in list)
            {
                BoundingBox elementAABB = element.GetWorldSpaceAABB();

                Vector4 color = Vector4.One;
                MyDebugDraw.DrawAABBLowRes(ref elementAABB, ref color, 1.0f);
            }
*/
            
            List<MyElement> list = new List<MyElement>();
            List<MyElement> list2 = new List<MyElement>();
            BoundingBox aabb = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));            
            m_prunningStructure.OverlapAllBoundingBox(ref aabb, list);
                  
            if (true)
            {
                foreach (MyElement element in list)
                {
                    BoundingBox elementAABB;                    
                    m_prunningStructure.GetFatAABB(element.ProxyData, out elementAABB);
                    //BoundingBox elementAABB = element.GetWorldSpaceAABB();

                    Vector4 color = Vector4.One;
                    MyDebugDraw.DrawAABBLine(ref elementAABB, ref color, 1.0f);
                    //MyDebugDraw.DrawText(elementAABB.GetCenter(), new System.Text.StringBuilder(((MyRenderObject)element).Entity.DisplayName), Color.White, 0.7f);
                }
            }
                
            return;

            m_cullingStructure.OverlapAllBoundingBox(ref aabb, list);

            float i = 0;
            foreach (MyElement element in list)
            {
                BoundingBox elementAABB = element.GetWorldSpaceAABB();
                i++;
                //if (i % 16 != 0) continue;

                float r = (i * 0.6234890156176f) % 1.0f * 0.5f, g = (i * 0.7234890156176f) % 1.0f * 0.5f;
                Vector4 randColor = new Vector4(r, g, 0.5f - 0.5f*(r+g), 0.5f);
                Vector4 color = randColor * 2;

                //if (Vector3.Distance(MyCamera.Position, elementAABB.GetCenter()) < 3000)
                {
                    m_cullingStructure.GetFatAABB(element.ProxyData, out elementAABB);
                   // MyDebugDraw.DrawAABBLine(ref elementAABB, ref color, 1.0f);
                    //m_prunningStructure.GetFatAABB(element.ProxyData, out elementAABB);

                    //MyDebugDraw.DrawAABBLine(ref elementAABB, ref color, 1.0f);
                    //MyDebugDraw.DrawText(elementAABB.GetCenter(), new System.Text.StringBuilder(((MyRenderObject)element).Entity.DisplayName), Color.White, 0.7f);

                    MyCullableRenderObject cullObject = (MyCullableRenderObject)element;
                    cullObject.CulledObjects.OverlapAllBoundingBox(ref aabb, list2);

                    

                    if (true)
                    {
                        foreach (MyElement element2 in list2)
                        {
                            //elementAABB = element2.GetWorldSpaceAABB();
                            
                            

                            //if (Vector3.Distance(MyCamera.Position, elementAABB.GetCenter()) < 3000)
                            if (((MyRenderObject)element2).Entity is MyVoxelMap)
                            {
                                MyDebugDraw.DrawAABBLine(ref elementAABB, ref color, 1.0f);
                                MyDebugDraw.DrawText(elementAABB.GetCenter(), new System.Text.StringBuilder(list2.Count.ToString()), new Color(randColor * 2), 0.7f);

                                cullObject.CulledObjects.GetFatAABB(element2.ProxyData, out elementAABB);
                                if (!cullObject.GetQuery(MyOcclusionQueryID.MAIN_RENDER).OcclusionQueryVisible)
                                    color = Vector4.One;
                                else
                                    color = randColor * 1.5f;
                                MyDebugDraw.DrawAABBLine(ref elementAABB, ref color, 1.0f);
                            }
                        }
                    }
                }
            }
             
        }


        static MyEntity GetEntityFromPrunningStructure(MyDynamicAABBTree tree, ref MyLine line, ref BoundingBox boundingBox, MyEntity currentEntity, ref Vector3? ret, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool ignoreSelectable, List<MyElement> elementList)
        {
            tree.OverlapAllBoundingBox(ref boundingBox, elementList);

            foreach (MyElement element in elementList)
            {
                MyEntity entity = ((MyRenderObject)element).Entity;

                Debug.Assert(!entity.NearFlag);
                Debug.Assert(entity.Visible);

                //  Objects to ignore
                if ((entity == ignorePhysObject0) || (entity == ignorePhysObject1))
                    continue;

                if (entity is MinerWars.AppCode.Game.Entities.VoxelHandShapes.MyVoxelHandShape)
                    continue;

                if (!ignoreSelectable && !entity.IsSelectable())
                    continue;
                 
                Vector3? testResultEx;
                entity.GetIntersectionWithLine(ref line, out testResultEx);

                if (testResultEx != null)
                {
                    Vector3 dir = line.Direction;

                    if (Vector3.Dot((testResultEx.Value - line.From), dir) > 0)
                    {
                        if (ret == null)
                        {
                            ret = testResultEx;
                            currentEntity = entity;
                        }

                        if ((testResultEx.Value - line.From).Length() < (ret.Value - line.From).Length())
                        {
                            ret = testResultEx;
                            currentEntity = entity;
                        }
                    }
                }
            }

            return currentEntity;
        }


        public static void GetEntitiesFromPrunningStructure(ref BoundingBox boundingBox, List<MyElement> list)
        {
            list.Clear();

            GetEntitiesFromPrunningStructure(m_prunningStructure, ref boundingBox, list);

            m_cullingStructure.OverlapAllBoundingBox(ref boundingBox, m_cullObjectListForDraw, 0, false);

            foreach (MyElement element in m_cullObjectListForDraw)
            {
                MyCullableRenderObject cullObject = (MyCullableRenderObject)element;

                GetEntitiesFromPrunningStructure(cullObject.CulledObjects, ref boundingBox, list);
            }
        }

        public static void GetEntitiesFromShadowStructure(ref BoundingBox boundingBox, List<MyElement> list)
        {
            list.Clear();

            GetEntitiesFromPrunningStructure(m_shadowPrunningStructure, ref boundingBox, list);
        }

        static void GetEntitiesFromPrunningStructure(MyDynamicAABBTree tree, ref BoundingBox boundingBox, List<MyElement> list)
        {
            tree.OverlapAllBoundingBox(ref boundingBox, list, 0, false);
        }

        static void AddCullingObjects(List<BoundingBox> bbs)
        {
            // create culling objects (bbox with smallest surface area first)
            foreach (var bb in bbs.OrderBy(a => a.SurfaceArea()))
            {
                AddRenderObject(new MyCullableRenderObject(bb));
            }
        }

        public static float CullingStructureWorstAllowedBalance = 0.05f;
        public static float CullingStructureCutBadness = 20;  // Don't cut boxes.
        public static float CullingStructureImbalanceBadness = 0.15f;  // Be close to the median.
        public static float CullingStructureOffsetBadness = 0.1f;  // Be close to the geometric center (more so for initial splits).

        public static void RebuildCullingStructure()
        {
            var list = new List<MyElement>();
            var everything = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            var resultDivision = new List<BoundingBox>();

            // Clear old culling nodes
            m_cullingStructure.OverlapAllBoundingBox(ref everything, list);
            foreach (MyRenderObject ro in list)
            {
                RemoveRenderObject(ro);
            }

            // Split by type
            var roList = new List<MyRenderObject>();
            var prefabRoList = new List<MyRenderObject>();
            var voxelRoList = new List<MyRenderObject>();

            m_prunningStructure.OverlapAllBoundingBox(ref everything, list);
            foreach (MyRenderObject o in list)
            {
                if (o is MyCullableRenderObject) continue;

                if (MyFakes.CULL_EVERY_RENDER_CELL)
                {
                    if (o.Entity is MinerWars.AppCode.Game.Prefabs.MyPrefabBase)
                    {
                        roList.Add(o);
                        prefabRoList.Add(o);
                    }
                    else if (o.Entity is MyVoxelMap)
                    {
                        resultDivision.Add(o.GetWorldSpaceAABB());
                    }
                    else
                        roList.Add(o);
                }
                else
                {
                    if (o.Entity is MinerWars.AppCode.Game.Prefabs.MyPrefabBase)
                    {
                        roList.Add(o);
                        prefabRoList.Add(o);
                    }
                    else if (o.Entity is MyVoxelMap)
                    {
                        roList.Add(o);
                        voxelRoList.Add(o);
                    }
                    else
                        roList.Add(o);
                }
            }

            // Divide
            AddDivisionForCullingStructure(roList, Math.Max(MyRenderConstants.MIN_OBJECTS_IN_CULLING_STRUCTURE, (int)(roList.Count / MyRenderConstants.MAX_CULLING_OBJECTS * 1.5f)), resultDivision);

            AddDivisionForCullingStructure(
                prefabRoList,
                Math.Max(MyRenderConstants.MIN_PREFAB_OBJECTS_IN_CULLING_STRUCTURE, (int)(prefabRoList.Count / (MyRenderConstants.MAX_CULLING_PREFAB_OBJECTS * MyRenderConstants.m_maxCullingPrefabObjectMultiplier) * 1.5f)),
                resultDivision
            );

            AddDivisionForCullingStructure(
                voxelRoList,
                Math.Max(MyRenderConstants.MIN_VOXEL_RENDER_CELLS_IN_CULLING_STRUCTURE, (int)(voxelRoList.Count / (MyRenderConstants.MAX_CULLING_VOXEL_RENDER_CELLS) * 1.5f)),
                resultDivision
            );

            AddCullingObjects(resultDivision);
        }


        public static void RebuildCullingStructureCullEveryPrefab()
        {
            var list = new List<MyElement>();
            var everything = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
            var resultDivision = new List<BoundingBox>();

            // Clear old culling nodes
            m_cullingStructure.OverlapAllBoundingBox(ref everything, list);
            foreach (MyRenderObject ro in list)
            {
                RemoveRenderObject(ro);
            }

            // Split by type
            var roList = new List<MyRenderObject>();

            m_prunningStructure.OverlapAllBoundingBox(ref everything, list);
            foreach (MyRenderObject o in list)
            {
                if (o is MyCullableRenderObject) continue;

                roList.Add(o);
                if (o.Entity is MinerWars.AppCode.Game.Prefabs.MyPrefabBase)  // every prefab will be culled on its own
                    resultDivision.Add(o.GetWorldSpaceAABB());
            }

            // Divide
            AddDivisionForCullingStructure(roList, Math.Max(MyRenderConstants.MIN_OBJECTS_IN_CULLING_STRUCTURE, (int)(roList.Count / MyRenderConstants.MAX_CULLING_OBJECTS * 1.5f)), resultDivision);

            AddCullingObjects(resultDivision);
        }


        // Compute a division and add it to resultDivision.
        public static void AddDivisionForCullingStructure(List<MyRenderObject> roList, int objectCountLimit, List<BoundingBox> resultDivision)
        {
            List<List<MyRenderObject>> resultList = new List<List<MyRenderObject>>();

            // Have a stack of boxes to split; the initial box contains the whole sector
            Stack<List<MyRenderObject>> stackToDivide = new Stack<List<MyRenderObject>>();
            stackToDivide.Push(roList);
            int maxDivides = MyRenderConstants.MAX_CULLING_OBJECTS * 1000;  // sanity check

            while (stackToDivide.Count > 0 && maxDivides-- > 0)
            {
                // take the next box
                List<MyRenderObject> llist = stackToDivide.Pop();

                // if the object count is small, add it to the result list
                if (llist.Count <= objectCountLimit)
                {
                    resultList.Add(llist);
                    continue;
                }

                // get the tightest bounding box containing all objects
                BoundingBox caabb = MyMath.CreateInvalidAABB();
                foreach (MyRenderObject lro in llist)
                {
                    caabb = lro.GetWorldSpaceAABB().Include(ref caabb);
                }

                // we'll optimize split badness
                float bestPlanePos = 0;
                int bestAxis = 0;
                float bestBadness = float.MaxValue;

                // find the longest axis
                // nice to have (not needed): forbid an axis if it didn't work in the last split
                float longestAxisSpan = float.MinValue;
                for (int axis = 0; axis <= 2; axis++)
                {
                    float axisSpan = caabb.Max.GetDim(axis) - caabb.Min.GetDim(axis);
                    if (axisSpan > longestAxisSpan)
                    {
                        longestAxisSpan = axisSpan;
                        bestAxis = axis;
                        bestPlanePos = 0.5f * (caabb.Max.GetDim(axis) + caabb.Min.GetDim(axis));  // sanity check: if nothing works, split in the middle
                    }
                }

                // find the best split perpendicular to the longest axis (nicest results)
                // nice to have (not needed): try all three axes
                for (int axis = bestAxis; axis <= bestAxis; axis++)
                {
                    float axisSpan = caabb.Max.GetDim(axis) - caabb.Min.GetDim(axis);
                    float axisCenter = 0.5f * (caabb.Max.GetDim(axis) + caabb.Min.GetDim(axis));

                    // lo = bounding box mins, hi = bounding box maxes; add a sentinel at the end
                    var lo = new List<float>(); lo.Add(float.MaxValue);
                    var hi = new List<float>(); hi.Add(float.MaxValue);
                    foreach (var ro in llist)
                    {
                        lo.Add(ro.GetWorldSpaceAABB().Min.GetDim(axis));
                        hi.Add(ro.GetWorldSpaceAABB().Max.GetDim(axis));
                    }
                    lo.Sort();
                    hi.Sort();

                    // find the dividing plane that minimizes split badness
                    int leftCount = 0, cutCount = 0, rightCount = llist.Count;

                    for (int l = 0, h = 0; h < hi.Count - 1; )  // don't put everything on one side, that would be silly
                    {
                        // find split interval
                        float thisEventPos;
                        if (lo[l] < hi[h])
                        {
                            thisEventPos = lo[l];
                            rightCount--; cutCount++; l++;
                        }
                        else
                        {
                            thisEventPos = hi[h];
                            cutCount--; leftCount++; h++;
                        }
                        float nextEventPos = Math.Min(lo[l], hi[h]);  // nice to know

                        // if the split isn't too imbalanced
                        if (leftCount + cutCount >= CullingStructureWorstAllowedBalance * llist.Count &&
                            rightCount + cutCount >= CullingStructureWorstAllowedBalance * llist.Count)
                        {
                            // the split could be anywhere in (thisEventPos, nextEventPos); find the closest point in this interval to the geometric center
                            float closestSplitToCenter = axisCenter < thisEventPos ? thisEventPos : axisCenter > nextEventPos ? nextEventPos : axisCenter;

                            // compute badness
                            float badness =
                                cutCount * CullingStructureCutBadness  // Don't cut boxes.
                                + Math.Abs(leftCount - rightCount) * CullingStructureImbalanceBadness  // Be close to the median.
                                + Math.Abs(axisCenter - closestSplitToCenter) * CullingStructureOffsetBadness;  // Be close to the geometric center (more so for initial splits).

                            // found the best split?
                            if (badness < bestBadness)
                            {
                                bestBadness = badness;
                                bestAxis = axis;
                                bestPlanePos = 0.5f * (thisEventPos + nextEventPos);  // put the split plane between this and the next event
                            }
                        }
                    }
                }

                // split objects between left, right and cut
                var left = new List<MyRenderObject>();
                var right = new List<MyRenderObject>();
                var cut = new List<MyRenderObject>();

                foreach (MyRenderObject ro in llist)
                {
                    if (ro.GetWorldSpaceAABB().Max.GetDim(bestAxis) <= bestPlanePos)
                        left.Add(ro);
                    else if (ro.GetWorldSpaceAABB().Min.GetDim(bestAxis) >= bestPlanePos)
                        right.Add(ro);
                    else
                        cut.Add(ro);
                }

                // add cut boxes to the side with fewer boxes
                (left.Count < right.Count ? left : right).AddRange(cut);

                if (left.Count == 0)
                {
                    resultList.Add(right);  // can't be cut better
                    continue;
                }
                else if (right.Count == 0)
                {
                    resultList.Add(left);  // can't be cut better
                    continue;
                }
                else
                {
                    stackToDivide.Push(left);
                    stackToDivide.Push(right);
                }
            }

            // add bounding boxes to the resulting division
            foreach (var xList in resultList)
            {
                BoundingBox caabb = MyMath.CreateInvalidAABB();
                foreach (MyRenderObject ro in xList)
                    caabb = ro.GetWorldSpaceAABB().Include(ref caabb);
                resultDivision.Add(caabb);
            }
        }


        // Detects intersection for all entities
        public static MyEntity GetClosestIntersectionWithLine(ref MyLine line, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool ignoreSelectable = false)
        {
            //  Get collision skins near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);

            MyEntity retEntity = null;
            Vector3? ret = null;
            retEntity = GetEntityFromPrunningStructure(m_prunningStructure, ref line, ref boundingBox, retEntity, ref ret, ignorePhysObject0, ignorePhysObject1, ignoreSelectable, m_renderObjectListForIntersections);

            m_cullingStructure.OverlapAllBoundingBox(ref boundingBox, m_cullObjectListForIntersections);

            foreach (MyElement element in m_cullObjectListForIntersections)
            {
                MyCullableRenderObject cullObject = (MyCullableRenderObject)element;

                retEntity = GetEntityFromPrunningStructure(cullObject.CulledObjects, ref line, ref boundingBox, retEntity, ref ret, ignorePhysObject0, ignorePhysObject1, ignoreSelectable, m_renderObjectListForIntersections);
            }

            // retEntity = GetEntityFromPrunningStructure(m_cullingStructure, ref line, ref boundingBox, retEntity, ignorePhysObject0, ignorePhysObject1);

            return retEntity;
        }


        public static MyIntersectionResultLineTriangleEx? GetAnyIntersectionWithLine(ref MyLine line, MyEntity ignorePhysObject0, MyEntity ignorePhysObject1, bool ignoreSelectable)
        {
            //  Get collision skins near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref line, ref boundingBox);

            MyEntity retEntity = null;
            Vector3? ret = null;
            MyIntersectionResultLineTriangleEx? result = null;
            retEntity = GetEntityFromPrunningStructure(m_prunningStructure, ref line, ref boundingBox, retEntity, ref ret, ignorePhysObject0, ignorePhysObject1, ignoreSelectable, m_renderObjectListForIntersections);
            if (retEntity != null)
            {
                retEntity.GetIntersectionWithLine(ref line, out result);
                if (result.HasValue)
                    return result;
            }

            m_cullingStructure.OverlapAllBoundingBox(ref boundingBox, m_cullObjectListForIntersections);

            foreach (MyElement element in m_cullObjectListForIntersections)
            {
                MyCullableRenderObject cullObject = (MyCullableRenderObject)element;

                retEntity = GetEntityFromPrunningStructure(cullObject.CulledObjects, ref line, ref boundingBox, retEntity, ref ret, ignorePhysObject0, ignorePhysObject1, ignoreSelectable, m_renderObjectListForIntersections);

                if (retEntity != null)
                {
                    retEntity.GetIntersectionWithLine(ref line, out result);
                    if (result.HasValue)
                        return result;
                }
            }

            return result;
        }
        #endregion

        #region Occlusion queries

        static void IssueOcclusionQueries()
        {
            if (!m_currentSetup.EnableOcclusionQueries || !EnableHWOcclusionQueries)
                return;

            //     return;
            m_renderProfiler.StartProfilingBlock("IssueOcclusionQueries");

            m_renderProfiler.StartProfilingBlock("BlendState");

            bool showQueries = false;// ShowHWOcclusionQueries;

            BlendState oldBlendState = BlendState.Current;

            //generate and draw bounding box of our renderCell in occlusion query 
            if (showQueries)
                BlendState.Opaque.Apply();
            else
            {
                MyStateObjects.DisabledColorChannels_BlendState.Apply();
                MyMinerGame.SetRenderTarget(null, null);
            }

            Vector3 campos = MyCamera.Position;

            m_renderProfiler.EndProfilingBlock();

            RasterizerState oldRasterizeState = RasterizerState.Current;

            RasterizerState.CullNone.Apply();

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DepthStencilState.DepthRead.Apply();
            else
                DepthStencilState.None.Apply();

            MySimpleObjectDraw.PrepareFastOcclusionBoundingBoxDraw();

            MyPerformanceCounter.PerCameraDraw.QueriesCount += m_renderOcclusionQueries.Count;


            foreach (MyOcclusionQueryIssue queryIssue in m_renderOcclusionQueries)
            {
                //System.Diagnostics.Debug.Assert(!queryIssue.OcclusionQueryIssued);

                BoundingBox aabbExtended = new BoundingBox(queryIssue.CullObject.GetWorldSpaceAABB().Min - new Vector3(20.0f), queryIssue.CullObject.GetWorldSpaceAABB().Max + new Vector3(20.0f));

                if (!EnableHWOcclusionQueries || MyMath.Intersects(aabbExtended, ref campos))
                {
                    queryIssue.OcclusionQueryIssued = false;
                    queryIssue.OcclusionQueryVisible = true;
                    continue;
                }

                //m_renderProfiler.StartProfilingBlock("OcclusionQuery.Begin");

                queryIssue.OcclusionQueryIssued = !showQueries;
                //renderObject.OcclusionQueryVisible = true;
                if (queryIssue.OcclusionQuery == null)
                {
                    m_renderProfiler.EndProfilingBlock();
                    return;
                }

                bool isCompleted = queryIssue.OcclusionQuery.IsComplete;

                BoundingBox aabbExtendedForOC = new BoundingBox(queryIssue.CullObject.GetWorldSpaceAABB().Min - new Vector3(10.0f), queryIssue.CullObject.GetWorldSpaceAABB().Max + new Vector3(10.0f));

                if (!showQueries)
                {
                    //    renderObject.OcclusionQuery = new OcclusionQuery(m_device);
                    queryIssue.OcclusionQuery.Begin();
                }
                //m_renderProfiler.EndProfilingBlock();

                //m_renderProfiler.StartProfilingBlock("DrawOcclusionBoundingBox");
                MySimpleObjectDraw.FastOcclusionBoundingBoxDraw(aabbExtendedForOC, 1.0f);
                //m_renderProfiler.EndProfilingBlock();


                //m_renderProfiler.StartProfilingBlock("OcclusionQuery.End");

                if (!showQueries)
                    queryIssue.OcclusionQuery.End();
                //m_renderProfiler.EndProfilingBlock();
            }

            m_renderProfiler.StartProfilingBlock("oldBlendState");

            oldBlendState.Apply();

            m_renderProfiler.EndProfilingBlock();

            m_renderProfiler.EndProfilingBlock();
        }

        #endregion

        #region Preload


        public static void PreloadTexturesInRadius(float radius)
        {
            BoundingBox box = BoundingBox.CreateFromSphere(new BoundingSphere(MyCamera.Position, radius * 2));

            m_cullingStructure.OverlapAllBoundingBox(ref box, m_cullObjectListForDraw);
            m_prunningStructure.OverlapAllBoundingBox(ref box, m_renderObjectListForDraw);

            foreach (MyCullableRenderObject cullableObject in m_cullObjectListForDraw)
            {
                cullableObject.CulledObjects.OverlapAllBoundingBox(ref box, m_renderObjectListForDraw, 0, false);
            }

            foreach (MyRenderObject ro in m_renderObjectListForDraw)
            {
                ro.Entity.PreloadTextures();
            }

        }

        private static void PreloadEntityForDraw(MyEntity entity, Action BusyAction)
        {
            Stopwatch stopwatch = new Stopwatch();
            double msElapsed = 0;

            entity.PreloadForDraw();

            foreach (MyEntity child in entity.Children)
            {
                stopwatch.Start();

                PreloadEntityForDraw(child, BusyAction);


                stopwatch.Stop();

                msElapsed += stopwatch.Elapsed.TotalMilliseconds;

                stopwatch.Reset();

                if (msElapsed >= MyGuiConstants.LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS)
                {
                    msElapsed = 0;
                    BusyAction();
                }
            }
        }

        public static void PreloadEntitiesInRadius(float radius, Action BusyAction)
        {
            //Cannot use prunning structure because of deactivated entities which are not there
            /*
            BoundingBox box = BoundingBox.CreateFromSphere(new BoundingSphere(MyCamera.Position, radius * 2));

            m_cullingStructure.OverlapAllBoundingBox(ref box, m_cullObjectListForDraw);
            m_prunningStructure.OverlapAllBoundingBox(ref box, m_renderObjectListForDraw);

            foreach (MyCullableRenderObject cullableObject in m_cullObjectListForDraw)
            {
                cullableObject.CulledObjects.OverlapAllBoundingBox(ref box, m_renderObjectListForDraw, false);
            }

            Stopwatch stopwatch = new Stopwatch();
            double msElapsed = 0;

            foreach (MyRenderObject ro in m_renderObjectListForDraw)
            {
                stopwatch.Start();

                ro.Entity.PreloadForDraw();

                stopwatch.Stop();

                msElapsed += stopwatch.Elapsed.TotalMilliseconds;

                stopwatch.Reset();

                if (msElapsed >= MyGuiConstants.LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS)
                {
                    msElapsed = 0;
                    MyGuiScreenGamePlay.Static.DrawLoadAnimation();
                }
            }
           */

            

            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                PreloadEntityForDraw(entity, BusyAction);
            }
        }

        #endregion

        #region Entities prepare background worker

        //private static readonly AutoResetEvent m_prepareEntitiesEvent;
        //private static volatile bool m_prepareEntitiesCompleted = true;
        static Task m_prepareEntitiesTask;

        internal static void PrepareEntitiesForDrawStart()
        {
            if (!EnableEntitiesPrepareInBackground)
                return;

            WaitUntilEntitiesPrepared();

            if (MyCamera.GetLodTransitionDistanceBackgroundEnd() < 1)
            {
                return;
            }      

            Matrix optProjection = Matrix.CreatePerspectiveFieldOfView(MyCamera.FovWithZoom, MyCamera.AspectRatio, MyCamera.NEAR_PLANE_DISTANCE, MyCamera.GetLodTransitionDistanceBackgroundEnd());
            m_cameraFrustumMain.Matrix = MyCamera.ViewMatrix * optProjection;
            m_cameraPositionMain = MyCamera.Position;
            m_cameraZoomDivider = MyCamera.ZoomDivider;
            m_cameraZoomDividerMain = MyCamera.ZoomDivider;

            m_prepareEntitiesTask = Parallel.Start(PrepareEntitiesForDrawBackground);
        }

        private static void WaitUntilEntitiesPrepared()
        {
            m_prepareEntitiesTask.Wait();
        }

        private static void PrepareEntitiesForDrawBackground()
        {
            m_renderObjectListForDrawMain.Clear();
            m_cullObjectListForDrawMain.Clear();
            m_renderOcclusionQueriesMain.Clear();
            MyPerformanceCounter.PerCameraDraw.EntitiesOccluded = 0;

            PrepareEntitiesForDraw(ref m_cameraFrustumMain, m_cameraPositionMain, m_cameraZoomDividerMain, MyOcclusionQueryID.MAIN_RENDER, m_renderObjectListForDrawMain, m_cullObjectListForDrawMain, m_renderOcclusionQueriesMain, ref MyPerformanceCounter.PerCameraDraw.EntitiesOccluded);
        }

        #endregion

    }
}
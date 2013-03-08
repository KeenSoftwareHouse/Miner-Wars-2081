#region Using

using System.Collections.Generic;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
using ParallelTasks;
using MinerWarsMath.Graphics.PackedVector;
using System.Runtime.InteropServices;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using System;
using MinerWars.AppCode.Game.Textures;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;



#endregion

//  Use this STATIC class to create new particle and draw all living particles.
//  Particle is drawn as billboard or poly-line facing the camera. All particles lie on same texture atlas.
//  We use only pre-multiplied alpha particles. Here is the principle:
//      texture defines:
//          RGB - how much color adds/contributes the object to the scene
//          A   - how much it obscures whatever is behind it (0 = opaque, 1 = transparent ... but RGB is also important because it's additive)
//
//  Pre-multiplied alpha ----> blend(source, dest)  =  source.rgb + (dest.rgb * (1 - source.a))
//
//  In this world, RGB and alpha are linked. To make an object transparent you must reduce both its RGB (to contribute less color) and also its 
//  alpha (to obscure less of whatever is behind it). Fully transparent objects no longer have any RGB color, so there is only one value that 
//  represents 100% transparency (RGB and alpha all zero).
//
//  Billboards can be affected (lighted, attenuated) by other lights (player reflector, dynamic lights, etc) if 'CanBeAffectedByOtherLights = true'.
//  This is usefull for example when you want to have dust particle lighted by reflector, but don't want it for explosions or ship engine thrusts.
//  Drawing is always in back-to-front order.
//  If you set particle color higher than 1.0, it will shine more. It's cheap HDR effect.

namespace MinerWars.AppCode.Game.TransparentGeometry
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


    static class MyTransparentGeometry
    {
        #region Fields

        static MyObjectsPool<MyAnimatedParticle> m_animatedParticles = new MyObjectsPool<MyAnimatedParticle>(MyTransparentGeometryConstants.MAX_NEW_PARTICLES_COUNT);

        static MyObjectsPoolSimple<MyBillboard> m_preallocatedBillboards = new MyObjectsPoolSimple<MyBillboard>(MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT / 2);   //  Billboards that survive only 1 draw call (dust, reflector, ...)
        static MyObjectsPoolSimple<MyBillboard> m_preallocatedParticleBillboards = new MyObjectsPoolSimple<MyBillboard>(MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT / 2);   //  Billboards that survive only 1 draw call (dust, reflector, ...)

        static List<MyBillboard> m_sortedTransparentGeometry = new List<MyBillboard>(MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT); //  Used for drawing sorted particles
        static List<MyBillboard> m_preparedTransparentGeometry = new List<MyBillboard>(MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT);
        static List<MyBillboard> m_lowresTransparentGeometry = new List<MyBillboard>(MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT);

        const int RENDER_BUFFER_SIZE = 4096;

        //  For drawing particle billboards using vertex buffer
        static MyTexture2D[] m_textures = new MyTexture2D[Enum.GetValues(typeof(MyTransparentGeometryTexturesEnum)).Length];
        static MyVertexFormatTransparentGeometry[] m_vertices = new MyVertexFormatTransparentGeometry[MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT * MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY];

        // This can be freed, but it would create holes in LOH, so let is allocated
        static int[] m_indices = new int[MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_INDICES];

        static MyTexture2D m_atlasTexture;

        static VertexBuffer m_vertexBuffer;
        static IndexBuffer m_indexBuffer;
        static int m_startOffsetInVertexBuffer;
        static int m_endOffsetInVertexBuffer;
        
        //Bounding sphere used to calculate nearby lights
        static BoundingSphere m_boundingSphereForLights = new BoundingSphere();

        //interpolated property used to calculate color for particles overdraw layer
        static TransparentGeometry.Particles.MyAnimatedPropertyVector4 m_overDrawColorsAnim;
        const int PARTICLES_OVERDRAW_MAX = 100;
        static Viewport m_halfViewport = new Viewport();

        public static Color ColorizeColor { get; set; }
        public static Vector3 ColorizePlaneNormal { get; set; }
        public static float ColorizePlaneDistance { get; set; }
        public static bool EnableColorize { get; set; }
        public static bool VisualiseOverdraw = false;  //must be public field because of obfuscation problems

        static bool IsEnabled
        {
            get
            {
                return MyRender.IsModuleEnabled(MyRenderStage.AlphaBlend, MyRenderModuleEnum.TransparentGeometry) 
                    || MyRender.IsModuleEnabled(MyRenderStage.AlphaBlendPreHDR, MyRenderModuleEnum.TransparentGeometry)
                    || MyRender.IsModuleEnabled(MyRenderStage.AlphaBlend, MyRenderModuleEnum.TransparentGeometryForward);
            }
        }

        #endregion

        #region Constructor

        static MyTransparentGeometry()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.TransparentGeometry, "Transparent geometry", Draw, MyRenderStage.AlphaBlendPreHDR, 150, true);
            
            //MyRender.RegisterRenderModule(MyRenderModuleEnum.TransparentGeometryForward, "Transparent geometry forward", DrawForward, MyRenderStage.AlphaBlend, 150, true);
            
            MyParticlesLibrary.Init();

            int v = 0;
            for (int i = 0; i < m_indices.Length; i += 6)
            {
                m_indices[i + 0] = v + 0;
                m_indices[i + 1] = v + 1;
                m_indices[i + 2] = v + 2;

                m_indices[i + 3] = v + 0;
                m_indices[i + 4] = v + 2;
                m_indices[i + 5] = v + 3;

                v += MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY;
            }   
        }

        #endregion

        #region Load/unload content

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyTransparentGeometry.LoadData");

            MyMwcLog.WriteLine(string.Format("MyTransparentGeometry.LoadData - START"));

            m_animatedParticles.DeallocateAll();
            m_preallocatedBillboards.ClearAllAllocated();
            m_preallocatedParticleBillboards.ClearAllAllocated();

            m_sortedTransparentGeometry.Clear();

            m_preparedTransparentGeometry.Clear();
            m_lowresTransparentGeometry.Clear();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public static void LoadContent()
        {                 
            MyMwcLog.WriteLine("TransparentGeometry.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("TransparentGeometry.LoadContent");

            //  Max count of all particles should be less or equal than max count of billboards
            MyCommonDebugUtils.AssertRelease(MyTransparentGeometryConstants.MAX_PARTICLES_COUNT <= MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT);
            MyCommonDebugUtils.AssertRelease(MyTransparentGeometryConstants.MAX_COCKPIT_PARTICLES_COUNT <= MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT);

            //Prepare data for atlas
            List<string> atlasTextures = new List<string>();
            Dictionary<MyTransparentMaterialEnum, int> atlasMaterials = new Dictionary<MyTransparentMaterialEnum, int>();
            foreach (MyTransparentMaterialEnum materialEnum in Enum.GetValues(typeof(MyTransparentMaterialEnum)))
            {
                MyTransparentMaterialProperties materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(materialEnum);
                if (materialProperties.UseAtlas)
                {
                    int i = (int)materialProperties.Texture;
                    string texturePath = "Textures\\" + MyEnumsToStrings.Particles[i];
                    if (!atlasTextures.Contains(texturePath))
                        atlasTextures.Add(texturePath);
                    atlasMaterials.Add(materialEnum, atlasTextures.IndexOf(texturePath));
                }
            }
            string[] atlasTexturesArray = atlasTextures.ToArray();
            MyAtlasTextureCoordinate[] m_textureCoords;
            //Load atlas
            MyUtils.LoadTextureAtlas(atlasTexturesArray, "Textures\\Particles\\", MyMinerGame.Static.RootDirectory + "\\Textures\\Particles\\ParticlesAtlas.tai", out m_atlasTexture, out m_textureCoords);
            
            //Assign atlas coordinates to materials UV
            foreach(KeyValuePair<MyTransparentMaterialEnum, int> pair in atlasMaterials)
            {
                MyTransparentMaterialEnum materialEnum = pair.Key;
                MyTransparentMaterialProperties materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(materialEnum);

                materialProperties.UVOffset = m_textureCoords[pair.Value].Offset;
                materialProperties.UVSize = m_textureCoords[pair.Value].Size;
            }

            m_vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice, MyVertexFormatTransparentGeometry.Stride * MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_COUNT * MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
            m_startOffsetInVertexBuffer = 0;
            m_endOffsetInVertexBuffer = 0;
            m_vertexBuffer.DebugName = "TransparentGeometry";

            m_indexBuffer = new IndexBuffer(MyMinerGame.Static.GraphicsDevice, MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_INDICES * sizeof(int), Usage.WriteOnly, Pool.Default, false);
            m_indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(m_indices);
            m_indexBuffer.Unlock();

            for (int i = 0; i < m_textures.GetLength(0); i++)
            {
                string toRemoveFromAtlas = "Textures\\" + MyEnumsToStrings.Particles[i];
                if (!atlasTextures.Contains(toRemoveFromAtlas))
                {
                    string contentPath = "Textures\\Particles\\" + MyEnumsToStrings.Particles[i].Remove(MyEnumsToStrings.Particles[i].Length - 4);
                    m_textures[i] = MyTextureManager.GetTexture<MyTexture2D>(contentPath);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("TransparentGeometry.LoadContent() - END");  
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("TransparentGeometry.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            
            if (m_indexBuffer != null)
            {
                m_indexBuffer.Dispose();
                m_indexBuffer = null;
            }

            if (m_vertexBuffer != null)
            {
                m_vertexBuffer.Dispose();
                m_vertexBuffer = null;
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("TransparentGeometry.UnloadContent - END");

        }


        #endregion

        #region Adding billboards/particles

        public static MyAnimatedParticle AddAnimatedParticle()
        {
            return m_animatedParticles.Allocate(true);
        }

        public static void DeallocateAnimatedParticle(MyAnimatedParticle particle)
        {
            m_animatedParticles.Deallocate(particle);
        }

        //  Add billboard for one frame only. This billboard isn't particle (it doesn't survive this frame, doesn't have update/draw methods, etc).
        //  It's used by other classes when they want to draw some billboard (e.g. rocket thrusts, reflector glare).
        public static void AddLineBillboard(MyTransparentMaterialEnum materialEnum,
            Vector4 color, Vector3 origin, Vector3 directionNormalized, float length, float thickness, int priority = 0, bool near = false)
        {
            if (!IsEnabled) return;

            MyUtils.AssertIsValid(origin);
            MyUtils.AssertIsValid(length);
            MyCommonDebugUtils.AssertDebug(length > 0);
            MyCommonDebugUtils.AssertDebug(thickness > 0);

            MyBillboard billboard = m_preallocatedBillboards.Allocate();
            if (billboard == null)
                return;

            billboard.Priority = priority;

            MyPolyLine polyLine;
            polyLine.LineDirectionNormalized = directionNormalized;
            polyLine.Point0 = origin;
            polyLine.Point1 = origin + directionNormalized * length;
            polyLine.Thickness = thickness;

            MyQuad quad;
            MyUtils.GetPolyLineQuad(out quad, ref polyLine);

            billboard.Start(ref quad, materialEnum, ref color, ref origin, false, near);
        }

        public static void AddLineBillboard2(MyTransparentMaterialEnum materialEnum,
          Vector4 color, Vector3 start, Vector3 end, float thickness, int priority = 0, bool near = false)
        {
            if (!IsEnabled) return;

            Vector3 dir = end - start;
            float length = dir.Length();
            dir.Normalize();

            AddLineBillboard(materialEnum, color,
                start, dir, length, thickness, priority, near);
        }

        //  Add billboard for one frame only. This billboard isn't particle (it doesn't survive this frame, doesn't have update/draw methods, etc).
        //  It's used by other classes when they want to draw some billboard (e.g. rocket thrusts, reflector glare).
        public static void AddPointBillboard(MyTransparentMaterialEnum materialEnum,
            Vector4 color, Vector3 origin, float radius, float angle, int priority = 0, bool colorize = false, bool near = false, bool lowres = false)
        {
            if (!IsEnabled) return;

            MyUtils.AssertIsValid(origin);
            MyUtils.AssertIsValid(angle);

            MyQuad quad;
            if (MyUtils.GetBillboardQuadAdvancedRotated(out quad, origin, radius, angle, MyCamera.Position) != false)
            {
                MyBillboard billboard = m_preallocatedBillboards.Allocate();
                if (billboard == null)
                    return;

                billboard.Priority = priority;
                billboard.Start(ref quad, materialEnum, ref color, ref origin, colorize, near, lowres);
            }
        }

        //  Add billboard for one frame only. This billboard isn't particle (it doesn't survive this frame, doesn't have update/draw methods, etc).
        //  This billboard isn't facing the camera. It's always oriented in specified direction. May be used as thrusts, or inner light of reflector.
        //  It's used by other classes when they want to draw some billboard (e.g. rocket thrusts, reflector glare).
        public static void AddBillboardOriented(MyTransparentMaterialEnum materialEnum,
            Vector4 color, Vector3 origin, Vector3 leftVector, Vector3 upVector, float radius, int priority = 0, bool colorize = false)
        {
            if (!IsEnabled) return;

            MyUtils.AssertIsValid(origin);
            MyUtils.AssertIsValid(leftVector);
            MyUtils.AssertIsValid(upVector);
            MyUtils.AssertIsValid(radius);
            MyCommonDebugUtils.AssertDebug(radius > 0);


            MyBillboard billboard = m_preallocatedBillboards.Allocate();
            if (billboard == null)
                return;

            billboard.Priority = priority;

            MyQuad quad;
            MyUtils.GetBillboardQuadOriented(out quad, ref origin, radius, ref leftVector, ref upVector);

            billboard.Start(ref quad, materialEnum, ref color, ref origin, colorize);
        }

        //  Add billboard for one frame only. This billboard isn't particle (it doesn't survive this frame, doesn't have update/draw methods, etc).
        //  This billboard isn't facing the camera. It's always oriented in specified direction. May be used as thrusts, or inner light of reflector.
        //  It's used by other classes when they want to draw some billboard (e.g. rocket thrusts, reflector glare).
        public static void AddBillboardOriented(MyTransparentMaterialEnum materialEnum,
            Vector4 color, Vector3 origin, Vector3 leftVector, Vector3 upVector, float width, float height, int priority = 0, bool colorize = false)
        {
            AddBillboardOriented(materialEnum, color, origin, leftVector, upVector, width, height, Vector2.Zero, priority, colorize);
        }

        public static void AddBillboardOriented(MyTransparentMaterialEnum materialEnum,
            Vector4 color, Vector3 origin, Vector3 leftVector, Vector3 upVector, float width, float height, Vector2 uvOffset, int priority = 0, bool colorize = false)
        {
            if (!IsEnabled) return;

            MyBillboard billboard = m_preallocatedBillboards.Allocate();
            if (billboard == null)
                return;

            billboard.Priority = priority;

            MyQuad quad;
            MyUtils.GetBillboardQuadOriented(out quad, ref origin, width, height, ref leftVector, ref upVector);

            billboard.Start(ref quad, materialEnum, ref color, ref origin, uvOffset, colorize);
        }

        public static bool AddQuad(MyTransparentMaterialEnum materialEnum, ref MyQuad quad, ref Vector4 color, ref Vector3 vctPos, int priority = 0)
        {
            if (!IsEnabled) return false;

            MyUtils.AssertIsValid(quad.Point0);
            MyUtils.AssertIsValid(quad.Point1);
            MyUtils.AssertIsValid(quad.Point2);
            MyUtils.AssertIsValid(quad.Point3);

            MyBillboard billboard = m_preallocatedBillboards.Allocate();
            if (billboard == null)
                return false;

            billboard.Priority = priority;

            billboard.Start(ref quad, materialEnum, ref color, ref vctPos);
            return true;
        }

        //  Call this at the start of every Draw() call. It will not delete one-time billboards.
        public static void ClearBillboards()
        {
            m_preallocatedBillboards.ClearAllAllocated();
            m_preallocatedParticleBillboards.ClearAllAllocated();
        }


        public static MyBillboard AddBillboardParticle(MyAnimatedParticle particle, MyBillboard effectBillboard, bool sort)
        {
            MyBillboard billboard = m_preallocatedParticleBillboards.Allocate();
            if (billboard != null)
            {
                MyTransparentGeometry.StartParticleProfilingBlock("item.Value.Draw");
                if (particle.Draw(billboard) == true)
                {
                    if (sort)
                    {
                        //  m_sortedTransparentGeometry.Add(billboard);
                    }
                    else
                        effectBillboard.ContainedBillboards.Add(billboard);

                    MyPerformanceCounter.PerCameraDraw.NewParticlesCount++;
                }

                MyTransparentGeometry.EndParticleProfilingBlock();
            }

            return billboard;
        }

        public static void AddBillboardToSortingList(MyBillboard billboard)
        {
            if (!IsEnabled) return;

            m_sortedTransparentGeometry.Add(billboard);
        }

        public static MyBillboard AddBillboardEffect(MyParticleEffect effect)
        {
            MyBillboard billboard = m_preallocatedParticleBillboards.Allocate();
            if (billboard != null)
            {
                billboard.ContainedBillboards.Clear();

                MyTransparentGeometry.StartParticleProfilingBlock("AddBillboardEffect");

                billboard.DistanceSquared = Vector3.DistanceSquared(MyCamera.Position, effect.WorldMatrix.Translation);

                billboard.Lowres = effect.LowRes || MyRenderConstants.RenderQualityProfile.LowResParticles;

                MyTransparentGeometry.EndParticleProfilingBlock();
            }
            return billboard;
        }

        
        #endregion

        #region Background worker

        static void PrepareVertexBuffer()
        {
            //  Billboards by distance to camera (back-to-front)
            m_sortedTransparentGeometry.Clear();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CopyBillboardsToSortingList sorted");
            CopyBillboardsToSortingList(m_preallocatedBillboards, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CopyAnimatedParticlesToSortingList");
            CopyAnimatedParticlesToSortingList(m_animatedParticles);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Sort");
            m_sortedTransparentGeometry.Sort();

            MyPerformanceCounter.PerCameraDraw.BillboardsSorted = m_sortedTransparentGeometry.Count;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CopyBillboardsToSortingList unsorted");
            CopyBillboardsToSortingList(m_preallocatedBillboards, false);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ProcessSortedBillboards");
            ProcessSortedBillboards();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        #endregion 

        #region Draw

        public static void DrawForward()
        {
            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
                return;

            Draw();
        }

        public static void Draw()
        {   
            MyStateObjects.AlphaBlend_NoAlphaWrite_BlendState.Apply();

            MyTransparentGeometry.Draw(Render.MyRender.GetRenderTarget(Render.MyRenderTargets.Depth));

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Particles count", MyPerformanceCounter.PerCameraDraw.NewParticlesCount);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().ProfileCustomValue("Billboard drawcalls", MyPerformanceCounter.PerCameraDraw.BillboardsDrawCalls);
        }


        //  Draws and updates active particles. If particle dies/timeouts, remove it from the list.
        //  This method is in fact update+draw.
        //  If drawNormalParticles = true, then normal particles are drawn. If drawNormalParticles=false, then in-cockpit particles are drawn.
        static void Draw(Texture depthForParticlesRT)
        {        
            PrepareVertexBuffer();

            bool setClipPlanes = !MyRenderConstants.RenderQualityProfile.ForwardRender && MyRender.CurrentRenderSetup.CallerID.Value == MyRenderCallerEnum.Main;
            if (setClipPlanes)
                MyCamera.SetParticleClipPlanes(true);
                 
            if (!VisualiseOverdraw && (!MyRenderConstants.RenderQualityProfile.ForwardRender))
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CopyToVertexBuffer Lowres");
                CopyToVertexBuffer(m_lowresTransparentGeometry);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("DrawVertexBuffer Lowres");

                MyRenderTargets lowresTarget = MyRenderTargets.AuxiliaryHalf0;
                MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(lowresTarget), null);
                MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.All, new ColorBGRA(0), 1, 0);

                m_halfViewport.Width = MyRender.GetRenderTarget(lowresTarget).GetLevelDescription(0).Width;
                m_halfViewport.Height = MyRender.GetRenderTarget(lowresTarget).GetLevelDescription(0).Height;
                MyMinerGame.Static.SetDeviceViewport(m_halfViewport);

                //  Pre-multiplied alpha
                BlendState.AlphaBlend.Apply();
                DrawVertexBuffer(MyRender.GetRenderTarget(MyRenderTargets.Depth), m_lowresTransparentGeometry);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.TakeScreenshot("LowresParticles", MyRender.GetRenderTarget(lowresTarget), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);

                MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.Auxiliary1), null);
                MyMinerGame.Static.SetDeviceViewport(MyCamera.Viewport);

                MyStateObjects.AlphaBlend_NoAlphaWrite_BlendState.Apply();
                MyRender.Blit(MyRender.GetRenderTarget(lowresTarget), false, MyEffectScreenshot.ScreenshotTechniqueEnum.LinearScale);

                
            }   
                

            //  Render
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CopyToVertexBuffer");
            CopyToVertexBuffer(m_preparedTransparentGeometry);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("DrawVertexBuffer");
            //  Pre-multiplied alpha with disabled alpha write
            MyStateObjects.AlphaBlend_NoAlphaWrite_BlendState.Apply();
            DrawVertexBuffer(depthForParticlesRT, m_preparedTransparentGeometry);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                        

            BlendState.Opaque.Apply();
            if (setClipPlanes)
                MyCamera.ResetClipPlanes(true);

            //Now we dont need billboards anymore and we can clear them
            ClearBillboards();  
        }


        static void ProcessSortedBillboards()
        {
            //replace contained billboards, move lowres billboards
            int c = 0;
            m_lowresTransparentGeometry.Clear();
            m_preparedTransparentGeometry.Clear();
            while (c < m_sortedTransparentGeometry.Count)
            {
                MyBillboard billboard = m_sortedTransparentGeometry[c];

                if (billboard.Lowres)
                {
                    if (billboard.ContainedBillboards.Count > 0)
                    {
                        m_lowresTransparentGeometry.AddRange(billboard.ContainedBillboards);
                    }
                    else
                    {
                        m_lowresTransparentGeometry.Add(billboard);
                    }
                }
                else
                {
                    if (billboard.ContainedBillboards.Count > 0)
                    {
                        m_preparedTransparentGeometry.AddRange(billboard.ContainedBillboards);
                    }
                    else
                    {
                        m_preparedTransparentGeometry.Add(billboard);
                    }
                }
                
                c++;
            }
        }

        static void CopyBillboardsToSortingList(MyObjectsPoolSimple<MyBillboard> billboards, bool sortedMaterials)
        {
            for (int i = 0; i < billboards.GetAllocatedCount(); i++)
            {
                MyBillboard billboard = billboards.GetAllocatedItem(i);
                MyTransparentMaterialProperties materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(billboard.MaterialEnum);
                if ((materialProperties.NeedSort && sortedMaterials)
                    ||
                    (!materialProperties.NeedSort && !sortedMaterials))
                    m_sortedTransparentGeometry.Add(billboard);
            }
        }

       
        static void CopyAnimatedParticlesToSortingList(MyObjectsPool<MyAnimatedParticle> particles)
        {
            if (!MyParticlesManager.Enabled)
            {
                m_preallocatedBillboards.ClearAllAllocated();
                return;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyParticlesManager.Draw()");

            //MyParticlesManager.PrepareForDraw();
            MyParticlesManager.Draw();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        /// <summary>
        /// Copies to vertex buffer.
        /// </summary>
        static void CopyToVertexBuffer(List<MyBillboard> billboards)
        {
            // Loop over in parallel tasks
            //Parallel.For(0, m_sortedTransparentGeometry.Count, m_copyGeometryToVertexBuffer);

            int verticesCount = billboards.Count * MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY;

            LockFlags lockFlags = LockFlags.NoOverwrite;

            if (verticesCount + m_endOffsetInVertexBuffer > MyTransparentGeometryConstants.MAX_TRANSPARENT_GEOMETRY_VERTICES)
            {
                m_startOffsetInVertexBuffer = 0;
                lockFlags = LockFlags.Discard;
            }
            else
                m_startOffsetInVertexBuffer = m_endOffsetInVertexBuffer;

            for (int i = 0; i < billboards.Count; i++)
            {
                CopyBillboardToVertices(i, billboards[i]);
            }

            if (billboards.Count > 0)
            {
                m_vertexBuffer.Lock(m_startOffsetInVertexBuffer * MyVertexFormatTransparentGeometry.Stride, verticesCount * MyVertexFormatTransparentGeometry.Stride, lockFlags).WriteRange(m_vertices, 0, verticesCount);
                m_vertexBuffer.Unlock();
            }
            m_endOffsetInVertexBuffer = m_startOffsetInVertexBuffer + verticesCount;
            //m_startOffsetInVertexBuffer = 0;
        }

        /// <summary>
        /// Copies the billboard to vertex buffer.
        /// </summary>
        /// <param name="billboarIdx">The billboar idx.</param>
        static void CopyBillboardToVertices(int billboarIdx, MyBillboard billboard)
        {
            int startIndex = (billboarIdx) * MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY;
            HalfVector4 colorHalf = new HalfVector4(billboard.Color);
            MyTransparentMaterialProperties materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(billboard.MaterialEnum);

            MyUtils.AssertIsValid(billboard.Position0);
            MyUtils.AssertIsValid(billboard.Position1);
            MyUtils.AssertIsValid(billboard.Position2);
            MyUtils.AssertIsValid(billboard.Position3);

            m_vertices[startIndex + 0].Position = billboard.Position0;
            m_vertices[startIndex + 0].Color = colorHalf;
            m_vertices[startIndex + 0].TexCoord = new HalfVector4(materialProperties.UVOffset.X + billboard.UVOffset.X, materialProperties.UVOffset.Y + billboard.UVOffset.Y, billboard.BlendTextureRatio, materialProperties.Emissivity);
            
            m_vertices[startIndex + 1].Position = billboard.Position1;
            m_vertices[startIndex + 1].Color = colorHalf;
            m_vertices[startIndex + 1].TexCoord = new HalfVector4(materialProperties.UVOffset.X + materialProperties.UVSize.X + billboard.UVOffset.X, materialProperties.UVOffset.Y + billboard.UVOffset.Y, billboard.BlendTextureRatio, materialProperties.Emissivity);
            
            m_vertices[startIndex + 2].Position = billboard.Position2;
            m_vertices[startIndex + 2].Color = colorHalf;
            m_vertices[startIndex + 2].TexCoord = new HalfVector4(materialProperties.UVOffset.X + materialProperties.UVSize.X + billboard.UVOffset.X, materialProperties.UVOffset.Y + materialProperties.UVSize.Y + billboard.UVOffset.Y, billboard.BlendTextureRatio, materialProperties.Emissivity);
            
            m_vertices[startIndex + 3].Position = billboard.Position3;
            m_vertices[startIndex + 3].Color = colorHalf;
            m_vertices[startIndex + 3].TexCoord = new HalfVector4(materialProperties.UVOffset.X + billboard.UVOffset.X, materialProperties.UVOffset.Y + materialProperties.UVSize.Y + billboard.UVOffset.Y, billboard.BlendTextureRatio, materialProperties.Emissivity);
            
            if (billboard.BlendTextureRatio > 0)
            {
                MyTransparentMaterialProperties blendMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(billboard.BlendMaterial);
                m_vertices[startIndex + 0].TexCoord2 = new HalfVector2(blendMaterialProperties.UVOffset.X + billboard.UVOffset.X, blendMaterialProperties.UVOffset.Y + billboard.UVOffset.Y);
                m_vertices[startIndex + 1].TexCoord2 = new HalfVector2(blendMaterialProperties.UVOffset.X + blendMaterialProperties.UVSize.X + billboard.UVOffset.X, blendMaterialProperties.UVOffset.Y + billboard.UVOffset.Y);
                m_vertices[startIndex + 2].TexCoord2 = new HalfVector2(blendMaterialProperties.UVOffset.X + blendMaterialProperties.UVSize.X + billboard.UVOffset.X , blendMaterialProperties.UVOffset.Y + blendMaterialProperties.UVSize.Y + billboard.UVOffset.Y);
                m_vertices[startIndex + 3].TexCoord2 = new HalfVector2(blendMaterialProperties.UVOffset.X + billboard.UVOffset.X, blendMaterialProperties.UVOffset.Y + blendMaterialProperties.UVSize.Y + billboard.UVOffset.Y);
            }

            /*
            m_vertices[startIndex + 3].Position = billboard.Position0;
            m_vertices[startIndex + 3].Color = colorHalf;
            m_vertices[startIndex + 3].TexCoord = new HalfVector4(0, 0, billboard.BlendTextureRatio, 0);

            m_vertices[startIndex + 4].Position = billboard.Position2;
            m_vertices[startIndex + 4].Color = colorHalf;
            m_vertices[startIndex + 4].TexCoord = new HalfVector4(1, 1, billboard.BlendTextureRatio, 0);

            m_vertices[startIndex + 5].Position = billboard.Position3;
            m_vertices[startIndex + 5].Color = colorHalf;
            m_vertices[startIndex + 5].TexCoord = new HalfVector4(0, 1, billboard.BlendTextureRatio, 0);
             * */
        }


        static void DrawBuffer(int firstIndex, int primitivesCount)
        {
            //baseVertexIndex - start of part VB which ATI copy to internal buffer
            //minVertexIndex - relative value each index is decremented do get correct index to internal VB buffer part
            MyMinerGame.Static.GraphicsDevice.DrawIndexedPrimitive(PrimitiveType.TriangleList, m_startOffsetInVertexBuffer, firstIndex / 3 * 2, primitivesCount * MyTransparentGeometryConstants.VERTICES_PER_TRIANGLE, firstIndex, primitivesCount);
            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
        }



        static void DrawVertexBuffer(Texture depthForParticlesRT, List<MyBillboard> billboards)
        {    
            //  This is important for optimalization (although I don't know when it can happen to have zero billboards), but
            //  also that loop below needs it - as it assumes we are rendering at least one billboard.
            if (billboards.Count == 0)
                return;

            Device device = MyMinerGame.Static.GraphicsDevice;
            Surface oldTargets = null;

            if (VisualiseOverdraw)
            {
                oldTargets = device.GetRenderTarget(0);

                //We borrow lod0normals to render stencil
                MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0), null);
                device.Clear(ClearFlags.Target | ClearFlags.Stencil, new ColorBGRA(0), 1.0f, 0);

                MyStateObjects.StencilMask_AlwaysIncrement_DepthStencilState.Apply();
            }
            else
            {
                if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                    DepthStencilState.DepthRead.Apply();
                else
                    DepthStencilState.None.Apply();
            }

            //  Draw particles without culling. It's because how we calculate left/up vector, we can have problems in back camera. (yes, that can be solved, but why bother...)
            //  Also I guess that drawing without culling may be faster - as GPU doesn't have to check it
            RasterizerState.CullNone.Apply();

            MyEffectTransparentGeometry effect = MyRender.GetEffect(MyEffects.TransparentGeometry) as MyEffectTransparentGeometry;

            effect.SetWorldMatrix(Matrix.Identity);

            effect.SetViewMatrix(ref MyCamera.ViewMatrix);

            effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrix);

            //MyMinerGame.Static.GraphicsDevice.Viewport = MyCamera.ForwardViewport;

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                effect.SetDepthsRT(depthForParticlesRT);
                effect.SetHalfPixel(depthForParticlesRT.GetLevelDescription(0).Width, depthForParticlesRT.GetLevelDescription(0).Height);
                effect.SetScale(MyRender.GetScaleForViewport(depthForParticlesRT));
            }


            //For struct size checks
            //int stride = MyVertexFormatTransparentGeometry.VertexDeclaration.VertexStride;
            //int s = Marshal.SizeOf(new MyVertexFormatTransparentGeometry());


            //  We iterate over all sorted billboards, and seach for when texture/shader has changed.
            //  We try to draw as many billboards as possible (using the same texture), but because we are rendering billboards
            //  sorted by depth, we still need to switch sometimes. Btw: I have observed, that most time consuming when drawing particles
            //  is device.DrawUserPrimitives(), even if I call it for the whole list of billboards (without this optimization). I think, it's
            //  because particles are pixel-bound (I do a lot of light calculation + there is blending, which is always slow).
            MyTransparentMaterialEnum? lastMaterialEnum = billboards[0].MaterialEnum;

            MyTransparentMaterialProperties materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastMaterialEnum.Value);
            MyBillboard lastBillboard = billboards[0];
            MyTransparentMaterialEnum lastBlendMaterial = lastBillboard.BlendMaterial;

            // 0.05% of billboard is blended
            const float softColorizeSize = 0.05f;

            device.VertexDeclaration = MyVertexFormatTransparentGeometry.VertexDeclaration;
            device.SetStreamSource(0, m_vertexBuffer, 0, MyVertexFormatTransparentGeometry.Stride);
            device.Indices = m_indexBuffer;

            MyRender.GetShadowRenderer().SetupShadowBaseEffect(effect);

            MyEffectTransparentGeometry effect2 = MyRender.GetEffect(MyEffects.TransparentGeometry) as MyEffectTransparentGeometry;
            effect2.SetShadowBias(0.001f);
            MyLights.UpdateEffectReflector(effect2.Reflector, false);
            m_boundingSphereForLights.Center = MyCamera.Position;
            m_boundingSphereForLights.Radius = 200;
            MyLights.UpdateEffect(effect2, ref m_boundingSphereForLights, false);


            int geomCount = billboards.Count;
            int it = 0;
            int cnt = 0;
            
            while (geomCount > 0)
            {
                if (geomCount > RENDER_BUFFER_SIZE)
                {
                    geomCount -= RENDER_BUFFER_SIZE;
                    cnt = RENDER_BUFFER_SIZE;
                }
                else
                {
                    cnt = geomCount;
                    geomCount = 0;
                }

                int indexFrom = it * RENDER_BUFFER_SIZE + 1;
                cnt = cnt + indexFrom - 1;
                for (int i = indexFrom; i <= cnt; i++)
                {
                    //  We need texture from billboard that's before the current billboard (because we always render "what was")
                    MyBillboard billboard = billboards[i - 1];
                    MyTransparentMaterialProperties blendMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(billboard.BlendMaterial);
                    MyTransparentMaterialProperties lastBlendMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastBlendMaterial);

                    bool colorizeChanged = EnableColorize && lastBillboard.EnableColorize != billboard.EnableColorize;
                    bool nearChanged = lastBillboard.Near != billboard.Near;
                    bool sizeChanged = EnableColorize && billboard.EnableColorize && lastBillboard.Size != billboard.Size;
                    bool blendTextureChanged = false;

                    if (lastBlendMaterial != billboard.BlendMaterial && billboard.BlendTextureRatio > 0)
                    {
                        if ((lastBlendMaterialProperties.UseAtlas) && (blendMaterialProperties.UseAtlas))
                            blendTextureChanged = false;
                        else
                            blendTextureChanged = true;
                    }

                    //bool blendTextureChanged = lastBlendTexture != billboard.BlendTexture;
                    bool billboardChanged = colorizeChanged || sizeChanged || blendTextureChanged || nearChanged;

                    MyTransparentMaterialProperties actMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(billboard.MaterialEnum);
                    MyTransparentMaterialProperties lastMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastMaterialEnum.Value);

                    billboardChanged |= (actMaterialProperties.CanBeAffectedByOtherLights != lastMaterialProperties.CanBeAffectedByOtherLights)
                                    || (actMaterialProperties.IgnoreDepth != lastMaterialProperties.IgnoreDepth);


                    if (!billboardChanged)
                    {
                        if (billboard.MaterialEnum != lastMaterialEnum)
                        {
                            if (actMaterialProperties.UseAtlas && lastMaterialProperties.UseAtlas)
                                billboardChanged = false;
                            else
                                billboardChanged = true;
                        }
                    }

                    //  If texture is different than the last one, or if we reached end of billboards
                    if ((i == cnt) || billboardChanged)
                    {
                        //  We don't need to do this when we reach end of billboards - it's needed only if we do next iteration of possible billboards
                        if ((i != cnt) || billboardChanged)
                        {
                            if ((i - indexFrom) > 0)
                            {
                                int firstIndex = (indexFrom - 1) * MyTransparentGeometryConstants.INDICES_PER_TRANSPARENT_GEOMETRY; //MyTransparentGeometryConstants.VERTICES_PER_TRANSPARENT_GEOMETRY;
                              
                                SetupEffect(ref materialProperties, lastBlendMaterialProperties, MyCamera.Position, EnableColorize && lastBillboard.EnableColorize, lastBillboard.Size * softColorizeSize, lastBillboard.Near);
                                   
                                if (lastBillboard.Near)
                                    effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrixForNearObjects);
                                else
                                    effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrix);

                                effect.Begin();
                                DrawBuffer(firstIndex, (i - indexFrom) * MyTransparentGeometryConstants.TRIANGLES_PER_TRANSPARENT_GEOMETRY);
                                effect.End();
                                       
                                MyPerformanceCounter.PerCameraDraw.BillboardsDrawCalls++;
                            }

                            lastMaterialEnum = billboard.MaterialEnum;
                            lastBillboard = billboard;
                            lastBlendMaterial = billboard.BlendMaterial;

                            materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastMaterialEnum.Value);
                            indexFrom = i;
                        }


                        if ((i == cnt) && (i - indexFrom + 1 != 0))
                        {
                            materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastBillboard.MaterialEnum);
                            blendMaterialProperties = MyTransparentMaterialConstants.GetMaterialProperties(lastBillboard.BlendMaterial);
                            int firstIndex = (indexFrom - 1) * MyTransparentGeometryConstants.INDICES_PER_TRANSPARENT_GEOMETRY;

                            SetupEffect(ref materialProperties, blendMaterialProperties, MyCamera.Position, EnableColorize && billboard.EnableColorize, lastBillboard.Size * softColorizeSize, lastBillboard.Near);

                            effect.Begin();
                            DrawBuffer(firstIndex, (i - indexFrom + 1) * MyTransparentGeometryConstants.TRIANGLES_PER_TRANSPARENT_GEOMETRY);
                            effect.End();

                            MyPerformanceCounter.PerCameraDraw.BillboardsDrawCalls++;
                        }
                    }
                }

                it++;
            }




            device.SetStreamSource(0, null, 0, 0);

            MyPerformanceCounter.PerCameraDraw.BillboardsInFrustum += billboards.Count;

            // Visualize overdraw of particles. More overdraws = bigger performance issue.
            if (VisualiseOverdraw)
            {
                if (m_overDrawColorsAnim == null)
                {
                    m_overDrawColorsAnim = new MyAnimatedPropertyVector4();
                    m_overDrawColorsAnim.AddKey(0.0f, new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
                    m_overDrawColorsAnim.AddKey(0.25f, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                    m_overDrawColorsAnim.AddKey(0.75f, new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
                    m_overDrawColorsAnim.AddKey(1.0f, new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                }

                //Space without particles is black
                device.Clear(ClearFlags.Target, new ColorBGRA(0) , 1.0f, 0);

                for (int referenceStencil = 1; referenceStencil < PARTICLES_OVERDRAW_MAX; referenceStencil++)
                {
                    DepthStencilState ds = new DepthStencilState()
                    {
                        StencilEnable = true,
                        ReferenceStencil = referenceStencil,
                        StencilFunction =  Compare.LessEqual,
                        DepthBufferEnable = false,
                    };

                    ds.Apply();

                    float diff = (float)(referenceStencil - 1) / (PARTICLES_OVERDRAW_MAX - 1);
                    Vector4 referenceColorV4;
                    m_overDrawColorsAnim.GetInterpolatedValue<Vector4>(diff, out referenceColorV4);
                    Color referenceColor = new Color(referenceColorV4);

                    Game.GUI.Core.MyGuiManager.DrawSpriteFast(Game.GUI.Core.MyGuiManager.GetBlankTexture(), 0, 0, MyCamera.Viewport.Width, MyCamera.Viewport.Height, referenceColor);
                }

                DepthStencilState.None.Apply();

                int leftStart = MyCamera.Viewport.Width / 4;
                int topStart = (int)(MyCamera.Viewport.Height * 0.75f);

                int size = MyCamera.Viewport.Width - 2 * leftStart;
                int sizeY = (int)(MyCamera.Viewport.Width / 32.0f);
                int sizeStep = size / PARTICLES_OVERDRAW_MAX;

                for (int i = 0; i < PARTICLES_OVERDRAW_MAX; i++)
                {
                    float diff = (float)(i - 1) / (PARTICLES_OVERDRAW_MAX - 1);
                    Vector4 referenceColorV4;
                    m_overDrawColorsAnim.GetInterpolatedValue<Vector4>(diff, out referenceColorV4);
                    Color referenceColor = new Color(referenceColorV4);

                    Game.GUI.Core.MyGuiManager.DrawSpriteFast(Game.GUI.Core.MyGuiManager.GetBlankTexture(), leftStart + i * sizeStep, topStart, sizeStep, sizeY, referenceColor);
                }

                MyDebugDraw.DrawText(new Vector2((float)leftStart, (float)(topStart + sizeY)), new System.Text.StringBuilder("1"), Color.White, 1.0f);
                MyDebugDraw.DrawText(new Vector2((float)leftStart + size, (float)(topStart + sizeY)), new System.Text.StringBuilder(">" + PARTICLES_OVERDRAW_MAX.ToString()), Color.White, 1.0f);

                device.SetRenderTarget(0, oldTargets);
                oldTargets.Dispose();

                MyRender.Blit(MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0), false);
            }

            //  Restore to 'opaque', because that's the usual blend state
            BlendState.Opaque.Apply();     
        }

        public static MyTexture2D GetTexture(MyTransparentMaterialEnum material)
        {
            var materialProperties = MyTransparentMaterialConstants.GetMaterialProperties(material);
            return GetTexture(ref materialProperties);
        }

        public static MyTexture2D GetTexture(ref MyTransparentMaterialProperties materialProperties)
        {
            MyTexture2D texture = materialProperties.UseAtlas ? m_atlasTexture : m_textures[(int)materialProperties.Texture];
            System.Diagnostics.Debug.Assert(texture != null, "Null particle texture - probably already loaded from atlas, check it");
            return texture;
        }

        private static void SetupEffect(ref MyTransparentMaterialProperties materialProperties, MyTransparentMaterialProperties blendMaterialProperties, Vector3 position, bool colorize, float colorizeSoftDist, bool near)
        {      
            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DepthStencilState.DepthRead.Apply();

            MyEffectTransparentGeometry effect = MyRender.GetEffect(MyEffects.TransparentGeometry) as MyEffectTransparentGeometry;

            effect.SetBillboardTexture(GetTexture(ref materialProperties));
            effect.SetBillboardBlendTexture(GetTexture(ref blendMaterialProperties));

            effect.SetSoftParticleDistanceScale(materialProperties.SoftParticleDistanceScale);

            effect.SetAlphaMultiplierAndSaturation(1, materialProperties.AlphaSaturation);

            if (near)
                effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrixForNearObjects);
            else
                effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrix);

            if (VisualiseOverdraw)
            {
                effect.SetTechnique(MyEffectTransparentGeometry.Technique.VisualizeOverdraw);
            }
            else
            {
                if (colorize)
                {
                    effect.SetColorizeSoftDistance(colorizeSoftDist);
                    effect.SetColorizeColor(ColorizeColor);
                    effect.SetColorizePlane(ColorizePlaneNormal, ColorizePlaneDistance);
                    effect.SetTechnique(MyEffectTransparentGeometry.Technique.ColorizeHeight);
                }
                else if (materialProperties.IgnoreDepth)
                {
                    if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                        DepthStencilState.None.Apply();

                    effect.SetTechnique(MyEffectTransparentGeometry.Technique.IgnoreDepth);
                }
                else if (materialProperties.CanBeAffectedByOtherLights)
                {     
                    if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                        effect.SetTechnique(MyEffectTransparentGeometry.Technique.UnlitForward);
                    else
                    {
                        effect.SetTechnique(MyEffectTransparentGeometry.Technique.Lit);
                    }   
                }
                else
                {
                    if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                        effect.SetTechnique(MyEffectTransparentGeometry.Technique.UnlitForward);
                    else
                    {
                        effect.SetTechnique(MyEffectTransparentGeometry.Technique.Unlit);
                    }
                }
            }     
        }

        [Conditional("PARTICLE_PROFILING")]
        public static void StartParticleProfilingBlock(string name)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock(name);
        }

        [Conditional("PARTICLE_PROFILING")]
        public static void EndParticleProfilingBlock()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        #endregion      
    }
}
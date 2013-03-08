#region Using

using System;
using System.Collections.Generic;

using MinerWars.CommonLIB.AppCode.Generics;

using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;

using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Render.EnvironmentMap;
using MinerWars.AppCode.Game.Render.Shadows;
using MinerWars.AppCode.Game.Voxels;
//using System.Threading;
//using System.Threading.Tasks;

using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.CommonLIB.AppCode.Networking;

using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MinerWars.CommonLIB.AppCode.Import;

    #region Enums
    public enum MyRenderTargets
    {
        Normals,
        Diffuse,
        Depth,
        DepthHalf,
        ZBuffer,

        SSAO,
        SSAOBlur,

        Auxiliary0,
        Auxiliary1,
        Auxiliary2,
        AuxiliaryHalf0,
        AuxiliaryQuarter0,
        AuxiliaryHalf1010102,

        HDR4,
        HDR4Threshold,

        ShadowMap,
        ShadowMapZBuffer,
        SecondaryShadowMap,
        SecondaryShadowMapZBuffer,

        // Environment map 2D texture - for rendering one face of cube texture
        EnvironmentMap,

        // Environment map cube texture - rendered cube texture (small size 6x128x128 or similar)
        EnvironmentCube,
        EnvironmentCubeAux,

        // Ambient map cube texture - precalculated ambient from cube (small size 6x128x128 or similar)
        AmbientCube,
        AmbientCubeAux,

        // Environment map aux texture (size of one face - to apply effects)
        EnvironmentFaceAux,
        EnvironmentFaceAux2,

        SecondaryCamera,
        SecondaryCameraZBuffer,
    }

    public enum MyEffects
    {
        //Rendering
        PointLight,
        DirectionalLight,
        BlendLights,
        //LodTransition2,
        ClearGBuffer,
        ShadowMap,
        //ShadowOcclusion,
        TransparentGeometry,
        //HudSectorBorder,
        Decals,

        //Post process effects
        SSAOBlur,
        VolumetricSSAO,
        GaussianBlur,
        DownsampleForSSAO,
        AntiAlias,
        Screenshot,
        Scale,
        Threshold,
        HDR,
        Luminance,
        Contrast,
        VolumetricFog,
        GodRays,

        //Model effects
        ModelDNS,
        ModelDiffuse,
        VoxelDebrisMRT,
        VoxelsMRT,
        VoxelStaticAsteroidMRT,
        Gizmo,

        //Occlusion queries
        OcclusionQueryDrawMRT,

        //Sprite Effects
        VideoSpriteEffects,

        //Ambient map
        AmbientMapPrecalculation,

        //Background
        DistantImpostors,
        BackgroundCube,
        //DebugDrawBillboards,

        //HUD
        HudRadar,
        Hud,
        CockpitGlass,

        //SolarSystemMap
        SolarMapGrid
    }

    public enum MyRenderStage
    {
        PrepareForDraw,
        Background,
        LODDrawStart,
        LODDrawEnd,
        AllGeometryRendered,
        AlphaBlendPreHDR,
        AlphaBlend,
        DebugDraw
    }

    /// <summary>
    /// This enum should contain an identificator for anything that uses the MyRender pipeline.
    /// </summary>
    public enum MyRenderCallerEnum
    {
        Main,
        EnvironmentMap,
        SolarMap,
        SecondaryCamera,
        GUIPreview,
    }
    public enum MyPostProcessEnum
    {
        VolumetricSSAO2,
        HDR,
        Contrast,
        VolumetricFog,
        FXAA,
        GodRays

    }
    public enum MyRenderModuleEnum
    {
        Cockpit,
        CockpitGlass,
        SunGlareAndLensFlare,
        UpdateOcclusions,
        AnimatedParticlesPrepare,
        TransparentGeometry,
        ParticlesDustField,
        VoxelHand,
        DistantImpostors,
        Decals,
        CockpitWeapons,
        SunGlow,
        SectorBorder,
        DrawSectorBBox, // SectorBorderRendering
        DrawCoordSystem,
        Explosions,
        BackgroundCube,
        GPS,
        TestField,
        AnimatedParticles,
        Lights,
        TransparentGeometryForward,
        Projectiles,
        DebrisField,
        ThirdPerson,
        Editor,
        SolarObjects,
        SolarMapGrid,
        PrunningStructure,
        SunWind,
        IceStormWind,
        PrefabContainerManager,
        InfluenceSpheres,
        SolarAreaBorders,
        PhysicsPrunningStructure,
        NuclearExplosion,
        AttackingBots,
    }
    #endregion

    static partial class MyRender
    {
        

        #region Delegates

        public delegate void DrawEventHandler();

        #endregion

        #region Nested classes


        public class MyRenderSetup
        {
            /// <summary>
            /// Holds information about who is calling the MyRender.Draw() method.
            /// This information is mandatory.
            /// </summary>
            public MyRenderCallerEnum? CallerID;

            public Texture[] RenderTargets;
            public Texture DepthTarget;

            public Vector3? CameraPosition;

            Matrix? m_viewMatrix;

            public Matrix? ViewMatrix
            {
                get { return m_viewMatrix; }

                set
                {
                    m_viewMatrix = value;
                    if (m_viewMatrix != null)
                    {
                        MyUtils.AssertIsValid(m_viewMatrix.Value);
                    }
                }
            }
            public Matrix? ProjectionMatrix;
            public float? AspectRatio;
            public float? Fov;
            public Viewport? Viewport;

            public float? LodTransitionNear; // Used
            public float? LodTransitionFar; // Used
            public float? LodTransitionBackgroundStart; // Used
            public float? LodTransitionBackgroundEnd; // Used

            public bool? EnableHDR; // Used
            public bool? EnableLights; // Used
            public bool? EnableSun; // Used
            public MyShadowRenderer ShadowRenderer; // Used - null for no shadows
            public bool? EnableShadowInterleaving;
            public bool? EnableSmallLights; // Used
            public bool? EnableSmallLightShadows; // Used
            public bool? EnableDebugHelpers; // Used
            public bool? EnableEnvironmentMapping;
            public bool? EnableNear;
            public bool EnableOcclusionQueries; //Used
            public bool EnableZoom;
            public float FogMultiplierMult; //Used
            public bool DepthToAlpha;

            // If background color is set, background cube is replaced by color
            public Color? BackgroundColor;

            public HashSet<MyRenderModuleEnum> EnabledModules; // Used
            public HashSet<MyPostProcessEnum> EnabledPostprocesses; // Used
            public HashSet<MyRenderStage> EnabledRenderStages; // Used

            public List<MyLight> LightsToUse; // if null, MyLights.GetLights() will be used
            public List<MyRenderElement> RenderElementsToDraw; // if null, MyEntities.Draw() will be used
            public List<MyRenderElement> TransparentRenderElementsToDraw; // if null, MyEntities.Draw() will be used

            public void Clear()
            {
                CallerID = null;

                RenderTargets = null;
                CameraPosition = null;
                ViewMatrix = null;
                ProjectionMatrix = null;
                AspectRatio = null;
                Fov = null;
                Viewport = null;

                LodTransitionNear = null;
                LodTransitionFar = null;
                LodTransitionBackgroundStart = null;
                LodTransitionBackgroundEnd = null;

                EnableHDR = null;
                EnableLights = null;
                EnableSun = null;
                ShadowRenderer = null;
                EnableShadowInterleaving = null;
                EnableSmallLights = null;
                EnableSmallLightShadows = null;
                EnableDebugHelpers = null;
                EnableEnvironmentMapping = null;
                EnableNear = null;

                BackgroundColor = null;

                EnableOcclusionQueries = true;
                EnableZoom = true;
                FogMultiplierMult = 1.0f;
                DepthToAlpha = false;

                EnabledModules = null;
                EnabledPostprocesses = null;
                EnabledRenderStages = null;
            }
        }

        public class MyRenderModuleItem
        {
            public string DisplayName;
            public MyRenderModuleEnum Name;
            public int Priority; // Lower is higher priority
            public DrawEventHandler Handler;
            public bool Enabled;


            public override string ToString()
            {
                return DisplayName;
            }
        }

        internal class MyRenderElement
        {
            public MyMeshDrawTechnique DrawTechnique;
            public MyMeshMaterial Material;

            //Debug
            //public string DebugName;
            public MyEntity Entity;

            //Element members
            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;
            public int IndexStart;
            public int TriCount;
            public int VertexCount;
            public VertexDeclaration VertexDeclaration;
            public int VertexStride;

            public Matrix WorldMatrix;
            public Matrix WorldMatrixForDraw;

            //public bool UseChannels;
            //public MyTexture2D MaskTexture;


            public MyVoxelCacheCellRenderBatch VoxelBatch;

            public override string ToString()
            {
                return "DrawTechnique: " + DrawTechnique.ToString() + " IB:" + IndexBuffer.GetHashCode().ToString();
            }
        }

        internal class MyLightRenderElement
        {
            public class MySpotComparer : IComparer<MyLightRenderElement>
            {
                public int Compare(MyLightRenderElement x, MyLightRenderElement y)
                {
                    var result = x.RenderShadows.CompareTo(y.RenderShadows);
                    if (result == 0)
                    {
                        var xHash = x.Light.ReflectorTexture != null ? x.Light.ReflectorTexture.GetHashCode() : 0;
                        var yHash = y.Light.ReflectorTexture != null ? y.Light.ReflectorTexture.GetHashCode() : 0;
                        result = xHash.CompareTo(yHash);
                    }
                    return result;
                }
            }

            public static MySpotComparer SpotComparer = new MySpotComparer();

            public MyLight Light;
            public Matrix World;
            public Matrix View;
            public Matrix Projection;
            public Matrix ShadowLightViewProjection;
            public Texture ShadowMap;
            public BoundingBox BoundingBox;
            public bool RenderShadows;
            public bool UseReflectorTexture; // If we should use reflector texture for spot light
        }

        //  Used to sort render elements by their properties to spare switching render states
        class MyRenderElementsComparer : IComparer<MyRenderElement>
        {
            public int Compare(MyRenderElement x, MyRenderElement y)
            {
                MyMeshDrawTechnique xDrawTechnique = x.DrawTechnique;
                MyMeshDrawTechnique yDrawTechnique = y.DrawTechnique;

                if (xDrawTechnique == yDrawTechnique)
                {
                    if (x.VoxelBatch != null && y.VoxelBatch != null)
                    {
                        return ((short)x.VoxelBatch.SortOrder).CompareTo((short)y.VoxelBatch.SortOrder);
                    }

                    int xMat = x.Material.GetHashCode();
                    int yMat = y.Material.GetHashCode();

                    if (xMat == yMat)
                    {
                        // This is right and slightly faster, static get hash code returns instance identifier
                        return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(x.VertexBuffer).CompareTo(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(y.VertexBuffer));
                    }
                    else
                    {
                        return xMat.CompareTo(yMat);
                    }
                }

                return ((int)xDrawTechnique).CompareTo((int)yDrawTechnique);
            }
        }

        #endregion

        #region Public Members

        public static bool AlternativeSort = true;

        public static bool ScreenshotOnlyFinal = true;

        public static bool ShowHWOcclusionQueries = false;
        public static bool EnableHWOcclusionQueries = true;
        public static bool EnableHWOcclusionQueriesForShadows = false;

        public static uint RenderCounter { get { return m_renderCounter; } }
        public static readonly Vector3 PrunningExtension = new Vector3(10, 10, 10);

        public static bool SkipLOD_NEAR = false;
        public static bool SkipLOD_0 = false;
        public static bool SkipLOD_1 = false;

        public static bool SkipVoxels = false;

        public static bool DebugDiffuseTexture = false;
        public static bool DebugNormalTexture = false;


        public static Vector3 AmbientColor;
        public static float AmbientMultiplier;
        public static float EnvAmbientIntensity;

        //Debug properties
        //public static bool ShowLODScreens = false;
        public static bool ShowEnvironmentScreens = false;
        public static bool ShowBlendedScreens = false;
        //public static bool EnableLODBlending = true;
        public static bool ShowGreenBackground = false;
        public static bool ShowLod1WithRedOverlay = false;
        
        //TODO
        public static bool EnableLights = true;
        public static bool EnableLightsRuntime = true;
        public static bool ShowEnhancedRenderStatsEnabled = false;
        public static bool ShowResourcesStatsEnabled = false;
        public static bool ShowTexturesStatsEnabled = false;

        public static bool EnableSun = true;
        public static bool EnableShadows = true;
        public static bool EnableAsteroidShadows = false;
        public static bool EnableFog = true;

        public static bool EnableEnvironmentMapAmbient
        {
            get { return m_enableEnvironmentMapAmbient; }
            set { m_enableEnvironmentMapAmbient = value; if (value) MyEnvironmentMap.Reset(); }
        }

        public static bool EnableEnvironmentMapReflection
        {
            get { return m_enableEnvironmentMapReflection; }
            set { m_enableEnvironmentMapReflection = value; if (value) MyEnvironmentMap.Reset(); }
        }

        public static bool EnablePerVertexVoxelAmbient = true;
        public static bool ShowCascadeSplits = false;

        //blinkg moving asteroids
        public static bool ShadowInterleaving = true;

        public static bool[] FreezeCascade = new bool[4];
        public static bool FreezeCascade0 { get { return FreezeCascade[0]; } set { FreezeCascade[0] = value; } }
        public static bool FreezeCascade1 { get { return FreezeCascade[1]; } set { FreezeCascade[1] = value; } }
        public static bool FreezeCascade2 { get { return FreezeCascade[2]; } set { FreezeCascade[2] = value; } }
        public static bool FreezeCascade3 { get { return FreezeCascade[3]; } set { FreezeCascade[3] = value; } }

        public static bool Wireframe = false;
        public static bool EnableStencilOptimization = true;
        public static bool EnableStencilOptimizationLOD1 = true;
        public static bool ShowStencilOptimization = false;

        public static bool CheckDiffuseTextures = true;
        public static bool CheckNormalTextures = false;

        public static bool ShowSpecularIntensity = false;
        public static bool ShowSpecularPower = false;
        public static bool ShowEmissivity = false;
        public static bool ShowReflectivity = false;

        public static bool EnableSpotShadows = true;

        public static bool EnableSpectatorReflector = true;
        public static bool DrawSpectatorReflector = false;
        public static MyLight SpectatorReflector;

        public static bool DrawPlayerLightShadow = false;
        public static MyLight PlayerLight;

        public static MyMwcVoxelMaterialsEnum? OverrideVoxelMaterial = null;


#if RENDER_PROFILING
        public static bool EnableEntitiesPrepareInBackground = true;
#else
        public static bool EnableEntitiesPrepareInBackground = true;
#endif
       

        // When no render setup on stack, we're rendering main fullscreen scene (no mirror, env.map etc)
        public static bool MainRendering
        {
            get
            {
                return m_renderSetupStack.Count == 0;
            }
        }

        /// <summary>
        /// Current render setup, it's safe to get value from any field, all nullable fields has value.
        /// Do not write to this, use Push/Pop functions
        /// </summary>
        public static MyRenderSetup CurrentRenderSetup
        {
            get
            {
                return m_currentSetup;
            }
        }

        //Temporary debug for occ queris
        public static bool RenderOcclusionsImmediatelly = false;

        //Too much render elements
        public static bool IsRenderOverloaded = false;


        #endregion

        #region Members


        private static MyRenderSetup m_currentSetup = new MyRenderSetup();
        private static List<MyRenderSetup> m_renderSetupStack = new List<MyRenderSetup>(10);
        private static MyRenderSetup m_backupSetup = new MyRenderSetup();

        private static List<MyTexture2D> m_textures = new List<MyTexture2D>(5);
        private static List<MyLight> m_lightsToRender = new List<MyLight>(20);
        private static List<MyLight> m_pointLights = new List<MyLight>(50); // just references
        private static List<MyLight> m_hemiLights = new List<MyLight>(50); // just references


        private static bool m_enableEnvironmentMapAmbient = true;
        private static bool m_enableEnvironmentMapReflection = true;

        static readonly BaseTexture[] m_renderTargets = new BaseTexture[Enum.GetValues(typeof(MyRenderTargets)).GetLength(0)];
        static readonly BaseTexture[] m_spotShadowRenderTargets = new BaseTexture[MyRenderConstants.SPOT_SHADOW_RENDER_TARGET_COUNT];
        static readonly BaseTexture[] m_spotShadowRenderTargetsZBuffers = new BaseTexture[MyRenderConstants.SPOT_SHADOW_RENDER_TARGET_COUNT];

        static readonly List<MyLightRenderElement> m_spotLightRenderElements = new List<MyLightRenderElement>(MyRenderConstants.SPOT_SHADOW_RENDER_TARGET_COUNT);
        static MyObjectsPool<MyLightRenderElement> m_spotLightsPool = new MyObjectsPool<MyLightRenderElement>(400); // Maximum number of spotlights allowed

        static readonly MyEffectBase[] m_effects = new MyEffectBase[Enum.GetValues(typeof(MyEffects)).GetLength(0)];
        static readonly List<MyRenderModuleItem>[] m_renderModules = new List<MyRenderModuleItem>[Enum.GetValues(typeof(MyRenderStage)).GetLength(0)];

        static MyLodTypeEnum m_currentLodDrawPass;
        static List<MyRenderObject> m_renderObjectsToDraw = new List<MyRenderObject>(MyModelsConstants.MAX_ENTITIES_TO_DRAW);
        static HashSet<MyEntity> m_entitiesToDebugDraw = new HashSet<MyEntity>();
        static List<MyPostProcessBase> m_postProcesses = new List<MyPostProcessBase>();

        //static MyObjectsPool<MyRenderElement> m_renderElementsPool = new MyObjectsPool<MyRenderElement>(MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT);
        static MyRenderElement[] m_renderElementsPool = new MyRenderElement[MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT];
        static int m_renderElementIndex = 0;
        static MyRenderElementsComparer m_renderElementsComparer = new MyRenderElementsComparer();
        static List<MyRenderElement> m_renderElements = new List<MyRenderElement>(MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT);
        static List<MyRenderElement> m_transparentRenderElements = new List<MyRenderElement>(MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT);
        static BoundingSphere m_lightBoundigSphere = new BoundingSphere();
        static BoundingBox m_lightBoundingBox = new BoundingBox();
        static Vector2 m_scaleToViewport = Vector2.One;

        static MySortedElements m_sortedElements = new MySortedElements();

        static Texture[] m_GBufferDefaultBinding;
        //static RenderTargetBinding[] m_GBufferLOD0Binding;
        //static RenderTargetBinding[] m_GBufferLOD1Binding;
        //static RenderTargetBinding[] m_GBufferLOD0ExBinding;
        //static RenderTargetBinding[] m_GBufferAux1Lod1DiffBinding;
        static Texture[] m_aux0Binding;

        //Texture for debug rendering
        static MyTexture2D m_debugTexture;
        static MyTexture2D m_debugNormalTexture;
        static MyTexture2D m_debugNormalTextureBump;
        //static RenderTarget2D m_screenshot;

        // Struct for getting statistics of render object
        static MyRenderStatistics m_renderStatistics;

        // Profiling of render
        static MyRenderProfiler m_renderProfiler = new MyRenderProfiler();

        //Shadows rendering
        static MyShadowRenderer m_shadowRenderer;

        // Spol light shadows 
        static MySpotShadowRenderer m_spotShadowRenderer;

        static Texture m_randomTexture = null;

        //Enabled renderer
        private static bool m_enabled = true;
        private static uint m_renderCounter = 0;
        

        static HashSet<MyRenderObject> m_nearObjects = new HashSet<MyRenderObject>();

        
        static MyDynamicAABBTree m_prunningStructure = new MyDynamicAABBTree(PrunningExtension);
        static MyDynamicAABBTree m_cullingStructure = new MyDynamicAABBTree(PrunningExtension);
        static MyDynamicAABBTree m_shadowPrunningStructure = new MyDynamicAABBTree(PrunningExtension);
        static List<MyElement> m_renderObjectListForDraw = new List<MyElement>(16384); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static List<MyElement> m_renderObjectListForDrawMain = new List<MyElement>(16384); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static List<MyElement> m_cullObjectListForDraw = new List<MyElement>(16384); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static List<MyElement> m_cullObjectListForDrawMain = new List<MyElement>(16384); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static List<MyElement> m_renderObjectListForIntersections = new List<MyElement>(128); 
        static List<MyElement> m_cullObjectListForIntersections = new List<MyElement>(128);
        static List<MyOcclusionQueryIssue> m_renderOcclusionQueries = new List<MyOcclusionQueryIssue>(256); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static List<MyOcclusionQueryIssue> m_renderOcclusionQueriesMain = new List<MyOcclusionQueryIssue>(256); // for draw method, using separate list, so that it cannot be impacted by concurrent modification
        static MyPhysObjectVoxelMapByDistanceComparer m_sortedVoxelMapsByDistanceComparer = new MyPhysObjectVoxelMapByDistanceComparer();
        static BoundingBox m_cameraFrustumBox;
        static BoundingFrustum m_cameraFrustum = new BoundingFrustum(Matrix.Identity);
        static BoundingFrustum m_cameraFrustumMain = new BoundingFrustum(Matrix.Identity);
        static Vector3 m_cameraPosition;
        static Vector3 m_cameraPositionMain;
        static float m_cameraZoomDivider;
        static float m_cameraZoomDividerMain;

        static List<MyRenderObject> m_debugRenderObjectListForDrawLOD0 = new List<MyRenderObject>();
        static List<MyRenderObject> m_debugRenderObjectListForDrawLOD1 = new List<MyRenderObject>();

        static Device m_device;
        //Sun
        static Lights.MyDirectionalLight m_sun = new MyDirectionalLight();
        static Vector3[] frustumCorners = new Vector3[8];

      
        #endregion

        #region Init

        static MyRender()
        {
            //Initialize post processes
            m_postProcesses.Add(new MyPostProcessHDR());
            m_postProcesses.Add(new MyPostProcessAntiAlias());
            m_postProcesses.Add(new MyPostProcessVolumetricSSAO2());
            //m_postProcesses.Add(new MyPostProcessContrast());
            m_postProcesses.Add(new MyPostProcessVolumetricFog());

            m_postProcesses.Add(new MyPostProcessGodRays());

            //Initialize for event registration
            for (int i = 0; i < Enum.GetValues(typeof(MyRenderStage)).GetLength(0); i++)
            {
                m_renderModules[i] = new List<MyRenderModuleItem>();
            }

            for (int i = 0; i < MyRenderConstants.MAX_RENDER_ELEMENTS_COUNT; i++)
            {
                m_renderElementsPool[i] = new MyRenderElement();
            }

            m_sun.Start();

            //m_prepareEntitiesEvent = new AutoResetEvent(false);
            //Task.Factory.StartNew(PrepareEntitiesForDrawBackground, TaskCreationOptions.PreferFairness);

            MyRender.RegisterRenderModule(MyRenderModuleEnum.PrunningStructure, "Prunning structure", DebugDrawPrunning, MyRenderStage.DebugDraw, 250, false);
            MyRender.RegisterRenderModule(MyRenderModuleEnum.PhysicsPrunningStructure, "Physics prunning structure", MyPhysics.DebugDrawPhysicsPrunning, MyRenderStage.DebugDraw, 250, false);

            MyRenderConstants.OnRenderQualityChange += new EventHandler(MyRenderConstants_OnRenderQualityChange);
        }

        static void MyRenderConstants_OnRenderQualityChange(object sender, EventArgs e)
        {
            //Recreate render targets with size depending on render quality
            if (m_shadowRenderer != null)
            {   //Test if content was already loaded
                m_shadowRenderer.ChangeSize(GetShadowCascadeSize());
                //CreateRenderTargets();
            }
        }

        /// <summary>
        /// Resets the render states.
        /// </summary>
        public static void ResetStates()
        {
            /*
            //m_device.SetSamplerState(0, SharpDX.Direct3D9.SamplerState.MagFilter, TextureFilter.Point);
            m_device.VertexSamplerStates[0] = SamplerState.PointClamp;
            m_device.VertexSamplerStates[1] = SamplerState.PointClamp;
            m_device.VertexSamplerStates[2] = SamplerState.PointClamp;
            m_device.VertexSamplerStates[3] = SamplerState.PointClamp;

            m_device.SamplerStates[0] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[1] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[2] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[3] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[4] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[5] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[6] = MyStateObjects.PointTextureFilter;
            m_device.SamplerStates[7] = MyStateObjects.PointTextureFilter;
             */
        }

        #endregion

        #region Properties


        internal static MyShadowRenderer GetShadowRenderer()
        {
            return m_currentSetup.ShadowRenderer != null ? m_currentSetup.ShadowRenderer : m_shadowRenderer;
        }

        public static MyDirectionalLight Sun
        {
            get { return m_sun; }
        }

        internal static MyRenderProfiler GetRenderProfiler()
        {
            return m_renderProfiler;
        }

        internal static bool Enabled
        {
            get { return m_enabled; }
            set
            {
                m_enabled = value;
            }
        }

        //  Resolve back buffer into texture. This method doesn't belong to GUI manager but I don't know about better place. It doesn't belong
        //  to utils too, because it's too XNA specific.
        
        public static Texture GetBackBufferAsTexture()
        {
            //TODO
            //return (new MyResolveBackBuffer(m_device)).RenderTarget;
            return null;
        } 

        #endregion
    }
}
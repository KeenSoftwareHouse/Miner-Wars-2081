using System;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Effects;

namespace MinerWars.AppCode.Game.Render
{
    enum MyRenderQualityEnum
    {
        NORMAL = 0,
        HIGH = 1,
        EXTREME = 2,
        LOW = 3
    }

    class MyRenderQualityProfile
    {
        public MyRenderQualityEnum RenderQuality;

        //LODs
        public float LodTransitionDistanceNear;
        public float LodTransitionDistanceFar;
        public float LodTransitionDistanceBackgroundStart;
        public float LodTransitionDistanceBackgroundEnd;

        // LODs for Environment maps
        public float EnvironmentLodTransitionDistance;
        public float EnvironmentLodTransitionDistanceBackground;
            
        //Textures
        public TextureQuality TextureQuality;

        //Voxels
        public MyEffectVoxels.MyEffectVoxelsTechniqueEnum VoxelsRenderTechnique;

        //Models
        public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsRenderTechnique;
        public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsBlendedRenderTechnique;
        public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsMaskedRenderTechnique;
        public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsHoloRenderTechnique;
        //public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsChannelsTechnique;
        public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsStencilTechnique;
        //public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsStencilTechniqueInstanced;
        //public MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum ModelsInstancedTechnique;

        //Shadows
        /// <summary>
        /// Determines last index of cascade, which will use LOD0 objects. Lower cascade index means
        /// closer cascade to camera. Ie. 0 means all cascaded will use LOD1 models (worst shadow quality),
        /// 5 means best quality, because all cascades will use LOD0 objects (we have 4 cascades currently);
        /// </summary>
        public int ShadowCascadeLODTreshold;
        
        /// <summary>
        /// Size in pixels for one shadow map cascade (of 4 total). Be carefull, we are limited by 8192 in texture size on PC.
        /// </summary>
        public int ShadowMapCascadeSize;
        public int SecondaryShadowMapCascadeSize; // For back camera

        public int ShadowBiasMultiplier;
        public bool EnableCascadeBlending;

        //HDR
        public bool EnableHDR;

        //SSAO
        public bool EnableSSAO;

        //FXAA
        public bool EnableFXAA;
                        
        //Environmentals
        public bool EnableEnvironmentals;

        //GodRays
        public bool EnableGodRays;

        //Geometry quality
        public bool UseNormals;
        public bool NeedReloadContent;

        // Use additional channel textures on models (dirt, rust, etc...)
        public bool UseChannels; 

        // Spot shadow max distance multiplier 
        public float SpotShadowsMaxDistanceMultiplier;

        // Forward render
        public bool ForwardRender;

        // Low resolution particles
        public bool LowResParticles;

        // Distant impostors
        public bool EnableDistantImpostors;

        // Flying debris
        public bool EnableFlyingDebris;

        // Decals
        public bool EnableDecals;

        //Explosion voxel debris
        public float ExplosionDebrisCountMultiplier;
    }

    static class MyRenderConstants
    {
        /// <summary>
        /// Maximum distance for which a light uses occlusion an occlusion query.
        /// </summary>
        public const float MAX_GPU_OCCLUSION_QUERY_DISTANCE = 150;

        public static readonly int MAX_RENDER_ELEMENTS_COUNT = 32768;
        public static readonly int DEFAULT_RENDER_MODULE_PRIORITY = 100;
        public static readonly int SPOT_SHADOW_RENDER_TARGET_COUNT = 4;
        public static readonly int ENVIRONMENT_MAP_SIZE = 128;
                
        public static readonly int MIN_OBJECTS_IN_CULLING_STRUCTURE = 128;
        public static readonly int MAX_CULLING_OBJECTS = 64;
        
        public static readonly int MIN_PREFAB_OBJECTS_IN_CULLING_STRUCTURE = 32;
        public static readonly int MAX_CULLING_PREFAB_OBJECTS = 256;

        public static readonly int MIN_VOXEL_RENDER_CELLS_IN_CULLING_STRUCTURE = 2;
        public static readonly int MAX_CULLING_VOXEL_RENDER_CELLS = 128;

        public static float m_maxCullingPrefabObjectMultiplier = 1.0f;

        public static readonly float DISTANCE_CULL_RATIO = 100; //in meters, how far must be 1m radius object to be culled by distance
        public static readonly float DISTANCE_LIGHT_CULL_RATIO = 40;

        static readonly MyRenderQualityProfile[] m_renderQualityProfiles = new MyRenderQualityProfile[Enum.GetValues(typeof(MyRenderQualityEnum)).Length];

        public static MyRenderQualityProfile RenderQualityProfile { get; private set; }
        public static event EventHandler OnRenderQualityChange = null;

        static MyRenderConstants()
        {
            m_renderQualityProfiles[(int)MyRenderQualityEnum.NORMAL] = new MyRenderQualityProfile()
            {
                RenderQuality = MyRenderQualityEnum.NORMAL,

                //LODs
                LodTransitionDistanceNear = 1500,
                LodTransitionDistanceFar = 2000,
                LodTransitionDistanceBackgroundStart = 10000,
                LodTransitionDistanceBackgroundEnd = 11000,

                // No need to set, env maps enabled only on high and extreme
                EnvironmentLodTransitionDistance = 200,
                EnvironmentLodTransitionDistanceBackground = 300,

                //Textures
                TextureQuality = TextureQuality.Half,
            
                //Voxels
                VoxelsRenderTechnique = MyEffectVoxelsBase.MyEffectVoxelsTechniqueEnum.Normal,

                //Models
                ModelsRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Normal,
                ModelsBlendedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.NormalBlended,
                ModelsMaskedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.NormalMasked,
                ModelsHoloRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Holo,
                //ModelsChannelsTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Normal, // Do not use channels on normal
                ModelsStencilTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Stencil,
                //ModelsStencilTechniqueInstanced = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.StencilInstanced,
                //ModelsInstancedTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Normalnstanced,

                //Shadows
                ShadowCascadeLODTreshold = 2,
                ShadowMapCascadeSize = 1024,
                SecondaryShadowMapCascadeSize = 64,
                ShadowBiasMultiplier = 5,
                EnableCascadeBlending = false,
                
                //HDR
                EnableHDR = false,

                //SSAO
                EnableSSAO = false,

                //FXAA
                EnableFXAA = false,

                //Environmentals
                EnableEnvironmentals = false,

                //GodRays
                EnableGodRays = false,


                //Geometry quality
                UseNormals = true,
                NeedReloadContent = true, //because normal->high and vertex channels
                UseChannels = false,

                // Spot shadow max distance multiplier 
                SpotShadowsMaxDistanceMultiplier = 1.0f,

                // Forward render
                ForwardRender = false,

                // Low res particles
                LowResParticles = true,

                // Distant impostors
                EnableDistantImpostors = false,

                // Flying debris
                EnableFlyingDebris = false,

                // Decals
                EnableDecals = false,

                //Explosion voxel debris
                ExplosionDebrisCountMultiplier = 0.5f,
            };

            m_renderQualityProfiles[(int)MyRenderQualityEnum.LOW] = new MyRenderQualityProfile()
            {
                RenderQuality = MyRenderQualityEnum.LOW,

                //LODs
                LodTransitionDistanceNear = 600,
                LodTransitionDistanceFar = 800,
                LodTransitionDistanceBackgroundStart = 3000,
                LodTransitionDistanceBackgroundEnd = 3500,

                // No need to set, env maps enabled only on high and extreme
                EnvironmentLodTransitionDistance = 100,
                EnvironmentLodTransitionDistanceBackground = 200,

                //Textures
                TextureQuality = TextureQuality.OneFourth,

                //Voxels
                VoxelsRenderTechnique = MyEffectVoxelsBase.MyEffectVoxelsTechniqueEnum.Low,

                //Models
                ModelsRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Low,
                ModelsBlendedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.LowBlended,
                ModelsMaskedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.LowMasked,
                ModelsHoloRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.HoloForward,
                //ModelsChannelsTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Low, // Do not use channels on low
                ModelsStencilTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.StencilLow,
                //ModelsStencilTechniqueInstanced = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.StencilLowInstanced,
                //ModelsInstancedTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.LowInstanced,

                //Shadows
                ShadowCascadeLODTreshold = 2,
                ShadowMapCascadeSize = 512,
                SecondaryShadowMapCascadeSize = 32,
                ShadowBiasMultiplier = 10,
                EnableCascadeBlending = false,

                //HDR
                EnableHDR = false,

                //SSAO
                EnableSSAO = false,

                //FXAA
                EnableFXAA = false,   
    
                //Environmentals
                EnableEnvironmentals = false,

                //GodRays
                EnableGodRays = false,

                //Geometry quality
                UseNormals = false,
                NeedReloadContent = true,
                UseChannels = false,

                // Spot shadow max distance multiplier 
                SpotShadowsMaxDistanceMultiplier = 0.0f,

                // Forward render
                ForwardRender = true,

                // Low res particles
                LowResParticles = false,

                // Distant impostors
                EnableDistantImpostors = false,

                // Flying debris
                EnableFlyingDebris = false,

                // Decals
                EnableDecals = false,

                //Explosion voxel debris
                ExplosionDebrisCountMultiplier = 0,
            };

            m_renderQualityProfiles[(int)MyRenderQualityEnum.HIGH] = new MyRenderQualityProfile()
            {
                RenderQuality = MyRenderQualityEnum.HIGH,

                //LODs
                LodTransitionDistanceNear = 2000,
                LodTransitionDistanceFar = 2500,
                LodTransitionDistanceBackgroundStart = 18000,
                LodTransitionDistanceBackgroundEnd = 20000,

                EnvironmentLodTransitionDistance = 400,
                EnvironmentLodTransitionDistanceBackground = 800,

                //Textures
                TextureQuality = TextureQuality.Full,

                //Voxels
                VoxelsRenderTechnique = MyEffectVoxelsBase.MyEffectVoxelsTechniqueEnum.High,

                //Models
                ModelsRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.High,
                ModelsBlendedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.HighBlended,
                ModelsMaskedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.HighMasked,
                ModelsHoloRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Holo,
                //ModelsChannelsTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.HighChannels,
                ModelsStencilTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Stencil,
                //ModelsStencilTechniqueInstanced = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.StencilInstanced,
                //ModelsInstancedTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.HighInstanced,

                //Shadows
                ShadowCascadeLODTreshold = 2,
                ShadowMapCascadeSize = 1024,
                SecondaryShadowMapCascadeSize = 64,
                ShadowBiasMultiplier = 2,
                EnableCascadeBlending = true,

                //HDR
                EnableHDR = true,

                //SSAO
                EnableSSAO = true,

                //FXAA
                EnableFXAA = true,

                //Environmentals
                EnableEnvironmentals = true,

                //GodRays
                EnableGodRays = true,

                //Geometry quality
                UseNormals = true,
                NeedReloadContent = true,
                UseChannels = true,

                // Spot shadow max distance multiplier 
                SpotShadowsMaxDistanceMultiplier = 2.5f,

                // Forward render
                ForwardRender = false,

                // Low res particles
                LowResParticles = false,

                // Distant impostors
                EnableDistantImpostors = true,

                // Flying debris
                EnableFlyingDebris = true,

                // Decals
                EnableDecals = true,

                //Explosion voxel debris
                ExplosionDebrisCountMultiplier = 0.8f,
            };

            m_renderQualityProfiles[(int)MyRenderQualityEnum.EXTREME] = new MyRenderQualityProfile()
            {
                RenderQuality = MyRenderQualityEnum.EXTREME,

                //LODs
                LodTransitionDistanceNear = 10000,
                LodTransitionDistanceFar = 11000,
                LodTransitionDistanceBackgroundStart = 42000,
                LodTransitionDistanceBackgroundEnd = 50000,

                EnvironmentLodTransitionDistance = 500,
                EnvironmentLodTransitionDistanceBackground = 1000,

                //Textures
                TextureQuality = TextureQuality.Full,

                //Voxels
                VoxelsRenderTechnique = MyEffectVoxelsBase.MyEffectVoxelsTechniqueEnum.Extreme,

                //Models
                ModelsRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Extreme,
                ModelsBlendedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.ExtremeBlended,
                ModelsMaskedRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.ExtremeMasked,
                ModelsHoloRenderTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Holo,
                //ModelsChannelsTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.ExtremeChannels,
                ModelsStencilTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.Stencil,
                //ModelsStencilTechniqueInstanced = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.StencilInstanced,
                //ModelsInstancedTechnique = MyEffectModelsDNS.MyEffectModelsDNSTechniqueEnum.ExtremeInstanced,

                //Shadows
                ShadowCascadeLODTreshold = 4,
                ShadowMapCascadeSize = 2048,
                SecondaryShadowMapCascadeSize = 128,
                ShadowBiasMultiplier = 2,
                EnableCascadeBlending = true,

                //HDR
                EnableHDR = true,

                //SSAO
                EnableSSAO = true,

                //FXAA
                EnableFXAA = true,

                //Environmentals
                EnableEnvironmentals = true,

                //GodRays
                EnableGodRays = true,

                //Geometry quality
                UseNormals = true,
                NeedReloadContent = true,
                UseChannels = true,

                // Spot shadow max distance multiplier 
                SpotShadowsMaxDistanceMultiplier = 3.0f,

                // Forward render
                ForwardRender = false,

                // Low res particles
                LowResParticles = false,

                // Distant impostors
                EnableDistantImpostors = true,

                // Flying debris
                EnableFlyingDebris = true,

                // Decals
                EnableDecals = true,

                //Explosion voxel debris
                ExplosionDebrisCountMultiplier = 3.0f,
            };

            //Default value
            RenderQualityProfile = m_renderQualityProfiles[(int)MyRenderQualityEnum.NORMAL];
        }

        public static void SwitchRenderQuality(MyRenderQualityEnum renderQuality)
        {
            RenderQualityProfile = m_renderQualityProfiles[(int)renderQuality];

            if (OnRenderQualityChange != null)
                OnRenderQualityChange(renderQuality, null);
        }
    }
}

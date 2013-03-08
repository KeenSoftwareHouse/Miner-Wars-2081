using System;
using System.Collections.Generic;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Effects;

using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;



namespace MinerWars.AppCode.Game.Render.EnvironmentMap
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    public class MyEnvironmentMapRenderer 
    {
        const float NEAR_CLIP_FOR_INSTANT = 120;

        CubeTexture m_environmentRT;
        CubeTexture m_ambientRT;
        Texture m_fullSizeRT;

        MyRender.MyRenderSetup m_setup;
        MyRender.MyRenderSetup m_backup = new MyRender.MyRenderSetup();

        BaseTexture[] m_bindings = new BaseTexture[1];

        float NearClip = MyCamera.NEAR_PLANE_DISTANCE;

        public float NearDistance 
        {
            get
            {
                return m_setup.LodTransitionNear.Value; 
            }
            set 
            { 
                m_setup.LodTransitionNear = value;
                m_setup.LodTransitionFar = value;
            }
        }

        public float FarDistance 
        {
            get 
            { 
                return m_setup.LodTransitionBackgroundStart.Value; 
            } 
            set 
            { 
                m_setup.LodTransitionBackgroundStart = value;
                m_setup.LodTransitionBackgroundEnd = value;
            }
        }

        public MyEnvironmentMapRenderer()
        {
            SetRenderSetup();
            MyRenderConstants.OnRenderQualityChange += new EventHandler(MyRenderConstants_OnRenderQualityChange);
        }

        void MyRenderConstants_OnRenderQualityChange(object sender, EventArgs e)
        {
            SetRenderSetup();
        }

        public void SetRenderTarget(CubeTexture environmentRT, CubeTexture ambientRT, Texture fullSizeRT)
        {
            m_setup.RenderTargets[0] = fullSizeRT;

            m_environmentRT = environmentRT;
            m_ambientRT = ambientRT;
            m_fullSizeRT = fullSizeRT;
        }

        Matrix CreateViewMatrix(CubeMapFace cubeMapFace, Vector3 position)
        {
            Matrix viewMatrix = Matrix.Identity;
            Vector3 pos = position;
            switch (cubeMapFace)
            {
                // Face index 0
                case CubeMapFace.PositiveX:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Left, -Vector3.Up);
                    break;

                // Face index 1
                case CubeMapFace.NegativeX:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Right, -Vector3.Up);
                    break;

                // Face index 2
                case CubeMapFace.PositiveY:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Down, Vector3.Backward);
                    break;

                // Face index 3
                case CubeMapFace.NegativeY:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Up, Vector3.Forward);
                    break;

                // Face index 4
                case CubeMapFace.PositiveZ:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Forward, -Vector3.Up);
                    break;

                // Face index 5
                case CubeMapFace.NegativeZ:
                    viewMatrix = Matrix.CreateLookAt(pos, pos + Vector3.Backward, -Vector3.Up);
                    break;
            }
            return viewMatrix;
        }

        public CubeTexture Environment
        {
            get
            {
                return m_environmentRT;
            }
        }

        public CubeTexture Ambient
        {
            get
            {
                return m_ambientRT;
            }
        }

        int currentIndex = -1;
        Vector3 position;

        // If render now is true, all face are rendered instantly
        public void StartUpdate(Vector3 position, bool renderNow = false)
        {
            this.position = position;
            currentIndex = 0;

            if (renderNow)
            {
                // When rendering all in one frame, make sure no close objects bother us
                var old = NearClip;
                NearClip = NEAR_CLIP_FOR_INSTANT;

                for (int i = 0; i < 12; i++)
                {
                    ContinueUpdate();
                }

                NearClip = old;
            }
        }

        public void ContinueUpdate()
        {
            if (currentIndex >= 0 && currentIndex <= 5)
            {
                // We use rendered scene in cube map for both environment and ambient;
                if (MyRender.EnableEnvironmentMapReflection || MyRender.EnableEnvironmentMapAmbient)
                {
                    UpdateFace(position, currentIndex);
                }
                currentIndex++;
                //currentIndex = 6; // Only render one side
            }
            else if (currentIndex >= 6 && currentIndex <= 11)
            {
                // Precalculate ambient to be used as lookup texture (cumulation)
                if (MyRender.EnableEnvironmentMapAmbient)
                {
                    UpdateAmbient(currentIndex - 6);
                }
                currentIndex++;
                //currentIndex = 12; // Only blur one side
            }
        }

        public bool IsDone()
        {
            return currentIndex == 12;
        }

        private void SetRenderSetup()
        {
            m_setup = new MyRender.MyRenderSetup();
            m_setup.CallerID = MyRenderCallerEnum.EnvironmentMap;

            m_setup.RenderTargets = new Texture[1];
           
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.Cockpit);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.CockpitGlass);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.SunGlareAndLensFlare);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.UpdateOcclusions);
                                   
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.AnimatedParticlesPrepare);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.TransparentGeometry);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.ParticlesDustField);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.VoxelHand);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.DistantImpostors);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.Decals);
            //m_setup.EnabledModules.Remove(MyRenderModuleEnum.CockpitWeapons);

            m_setup.EnabledModules = new HashSet<MyRenderModuleEnum>();
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SunGlow);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SectorBorder);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DrawSectorBBox);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DrawCoordSystem);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Explosions);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.BackgroundCube);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.GPS);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TestField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Lights);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometryForward);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Projectiles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DebrisField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.ThirdPerson);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Editor);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarObjects);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarMapGrid);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PrunningStructure);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SunWind);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.IceStormWind);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PrefabContainerManager);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PhysicsPrunningStructure);

            m_setup.EnabledRenderStages = new HashSet<MyRenderStage>();
            m_setup.EnabledRenderStages.Add(MyRenderStage.PrepareForDraw);
            m_setup.EnabledRenderStages.Add(MyRenderStage.Background);
            m_setup.EnabledRenderStages.Add(MyRenderStage.LODDrawStart);
            m_setup.EnabledRenderStages.Add(MyRenderStage.LODDrawEnd);

            m_setup.EnabledPostprocesses = new HashSet<MyPostProcessEnum>();
//            m_setup.EnabledPostprocesses.Add(MyPostProcessEnum.VolumetricFog);
            m_setup.FogMultiplierMult = 1.0f; //increases fog to imitate missing particle dust

            m_setup.EnableHDR = false;
            m_setup.EnableSun = false;
            m_setup.EnableSmallLights = false;
            m_setup.EnableDebugHelpers = false;
            m_setup.EnableEnvironmentMapping = false;
            m_setup.EnableOcclusionQueries = false;
            m_setup.EnableNear = false;

            m_setup.LodTransitionNear = MyRenderConstants.RenderQualityProfile.EnvironmentLodTransitionDistance;
            m_setup.LodTransitionFar = MyRenderConstants.RenderQualityProfile.EnvironmentLodTransitionDistance;
            m_setup.LodTransitionBackgroundStart = MyRenderConstants.RenderQualityProfile.EnvironmentLodTransitionDistanceBackground;
            m_setup.LodTransitionBackgroundEnd = MyRenderConstants.RenderQualityProfile.EnvironmentLodTransitionDistanceBackground;
        }

        public void UpdateFace(Vector3 position, int faceIndex)
        {
           // SetRenderSetup();

            CubeMapFace face = (CubeMapFace)faceIndex;

            // New setup
            m_setup.CameraPosition = position;
            m_setup.AspectRatio = 1.0f;
            m_setup.Viewport = new Viewport(0, 0, (int)m_environmentRT.GetLevelDescription(0).Width, (int)m_environmentRT.GetLevelDescription(0).Width);
            m_setup.ViewMatrix = CreateViewMatrix(face, position);
            m_setup.Fov = MathHelper.PiOver2;
            m_setup.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_setup.Fov.Value, m_setup.AspectRatio.Value, NearClip, m_setup.LodTransitionBackgroundEnd.Value);
            m_setup.DepthToAlpha = true;

            MyRender.GetRenderProfiler().StartProfilingBlock("Draw environmental maps");

            MyRender.PushRenderSetupAndApply(m_setup, ref m_backup);
            MyRender.Draw(false);

            MyRender.GetRenderProfiler().EndProfilingBlock();
                                        
            Surface cubeSurface = m_environmentRT.GetCubeMapSurface(face, 0);                 
            MyMinerGame.Static.GraphicsDevice.SetRenderTarget(0, cubeSurface);

            var screenEffect = MyRender.GetEffect(MyEffects.Screenshot) as MyEffectScreenshot;
            screenEffect.SetTechnique(MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
            screenEffect.SetSourceTexture(m_fullSizeRT);
            screenEffect.SetScale(new Vector2(m_environmentRT.GetLevelDescription(0).Width / (float)m_fullSizeRT.GetLevelDescription(0).Width * 0.968f, 0.982f * m_environmentRT.GetLevelDescription(0).Width / (float)m_fullSizeRT.GetLevelDescription(0).Height));
            MyGuiManager.GetFullscreenQuad().Draw(screenEffect);
            screenEffect.SetScale(new Vector2(1, 1));

            cubeSurface.Dispose();    

            MyRender.PopRenderSetupAndRevert(m_backup);
        }

        public void UpdateAmbient(int index)
        {                             
            CubeMapFace face = (CubeMapFace)index;
            Surface cubeSurface = m_ambientRT.GetCubeMapSurface(face, 0);
            MyMinerGame.Static.GraphicsDevice.SetRenderTarget(0, cubeSurface);
            BlendState.Opaque.Apply();

            MyEffectAmbientPrecalculation precalc = MyRender.GetEffect(MyEffects.AmbientMapPrecalculation) as MyEffectAmbientPrecalculation;
            precalc.SetEnvironmentMap(this.m_environmentRT);
            precalc.SetFaceMatrix(CreateViewMatrix(face, Vector3.Zero));
            precalc.SetRandomTexture(MyRender.GetRandomTexture());
            precalc.SetIterationCount(14);
            precalc.SetMainVectorWeight(1.0f);
            precalc.SetBacklightColorAndIntensity(new Vector3(MyRender.Sun.BackColor.X, MyRender.Sun.BackColor.Y, MyRender.Sun.BackColor.Z), MyRender.Sun.BackIntensity);
            MyGuiManager.GetFullscreenQuad().Draw(precalc);

            MyMinerGame.SetRenderTarget(null, null);
            cubeSurface.Dispose();  
        }
    }
}

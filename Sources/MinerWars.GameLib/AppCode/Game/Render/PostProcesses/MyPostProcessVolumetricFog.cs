#region Using

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;

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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Volumetric SSAO 2
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class MyPostProcessVolumetricFog : MyPostProcessBase
    {
        /// <summary>
        /// Name of the post process
        /// </summary>
        public override MyPostProcessEnum Name { get { return MyPostProcessEnum.VolumetricFog; } }
        public override string DisplayName { get { return "Volumetric Fog"; } }

        /// <summary>
        /// Enable state of post process
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Render method is called directly by renderer. Depending on stage, post process can do various things 
        /// </summary>
        /// <param name="postProcessStage">Stage indicating in which part renderer currently is.</param>public override void RenderAfterBlendLights()
        public override Texture Render(PostProcessStage postProcessStage, Texture source, Texture availableRenderTarget)
        {             
            switch (postProcessStage)
            {
                case PostProcessStage.PostLighting:
                    {
                        if (MySector.FogProperties.FogMultiplier <= 0.0f)
                            return source;

                        //MyMinerGame.Static.GraphicsDevice.SetRenderTarget(availableRenderTarget);

                        //MyMinerGame.Static.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                        MyStateObjects.VolumetricFogBlend.Apply();

                    //    MyMinerGame.Static.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                        MyEffectVolumetricFog volumetricFog = MyRender.GetEffect(MyEffects.VolumetricFog) as MyEffectVolumetricFog;

                        int width = MyRender.GetRenderTarget(MyRenderTargets.Normals).GetLevelDescription(0).Width;
                        int height = MyRender.GetRenderTarget(MyRenderTargets.Normals).GetLevelDescription(0).Height;

                        volumetricFog.SetSourceRT(source);
                        volumetricFog.SetDepthsRT(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                        volumetricFog.SetNormalsTexture(MyRender.GetRenderTarget(MyRenderTargets.Normals));
                        volumetricFog.SetHalfPixel(width, height);
                        volumetricFog.SetViewProjectionMatrix(MyCamera.ViewProjectionMatrix);
                        volumetricFog.SetCameraPosition(MyCamera.Position);
                        volumetricFog.SetCameraMatrix(Matrix.Invert(MyCamera.ViewMatrix));
                        volumetricFog.SetFrustumCorners(MyRender.GetShadowRenderer().GetFrustumCorners());
                        MyCamera.SetupBaseEffect(volumetricFog);

                        //volumetricFog.SetWorldMatrix(Matrix.CreateScale(1000) * Matrix.CreateTranslation(MyCamera.Position));
                        if (MyFakes.MWBUILDER)
                            volumetricFog.SetTechnique(MyEffectVolumetricFog.TechniqueEnum.SkipBackground);
                        else
                            volumetricFog.SetTechnique(MyEffectVolumetricFog.TechniqueEnum.Default);

                        MyGuiManager.GetFullscreenQuad().Draw(volumetricFog);

                      //  MyMinerGame.Static.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                      //  MyMinerGame.Static.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    }
                    break;
            }        
            return source;
        }
    }
}

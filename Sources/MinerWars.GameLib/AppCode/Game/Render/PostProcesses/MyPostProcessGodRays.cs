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
    using Color = MinerWarsMath.Color;

    class MyPostProcessGodRays : MyPostProcessBase
    {
        public MyPostProcessGodRays()
        {
            ApplyBlur = false;
        }

        /// <summary>
        /// Name of the post process
        /// </summary>
        public override MyPostProcessEnum Name { get { return MyPostProcessEnum.GodRays; } }
        public override string DisplayName { get { return "GodRays"; } }

        public float Density = 0.34f;  //0.097
        public float Weight = 1.27f;   //0.522
        public float Decay = 0.97f;    //0.992
        public float Exposition = 0.077f; //0.343

        public bool ApplyBlur;

        /// <summary>
        /// Enable state of post process
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled && (MyRenderConstants.RenderQualityProfile.EnableGodRays && MySector.GodRaysProperties.Enabled);
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
                case PostProcessStage.AlphaBlended:
                    {
                        Density = MySector.GodRaysProperties.Density;
                        Weight = MySector.GodRaysProperties.Weight;
                        Decay = MySector.GodRaysProperties.Decay;
                        Exposition = MySector.GodRaysProperties.Exposition;

                                    
                        var halfRT = MyRender.GetRenderTarget(MyRenderTargets.AuxiliaryHalf0);

                        MyMinerGame.SetRenderTarget(halfRT, null);
                        BlendState.Opaque.Apply();
                        RasterizerState.CullNone.Apply();
                        DepthStencilState.None.Apply();
                        
                        MyEffectGodRays effectGodRays = MyRender.GetEffect(MyEffects.GodRays) as MyEffectGodRays;

                        effectGodRays.SetDiffuseTexture(source);
                        effectGodRays.SetDepthTexture(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                        effectGodRays.SetFrustumCorners(MyRender.GetShadowRenderer().GetFrustumCorners());
                        effectGodRays.SetView(MyCamera.ViewMatrix);
                        effectGodRays.SetWorldViewProjection(MyCamera.ViewProjectionMatrix);
                        effectGodRays.SetDensity(Density);
                        effectGodRays.SetDecay(Decay);
                        effectGodRays.SetWeight(Weight * (1 - MySector.FogProperties.FogMultiplier));
                        effectGodRays.SetExposition(Exposition);
                        //effectGodRays.LightPosition.SetValue(1500f * -MySunGlare.GetSunDirection() * MySunConstants.RENDER_SUN_DISTANCE);
                        effectGodRays.SetLightPosition(1500f * -MyRender.Sun.Direction * MySunConstants.RENDER_SUN_DISTANCE);
                        effectGodRays.SetLightDirection(MyRender.Sun.Direction);
                        effectGodRays.SetCameraPos(MyCamera.Position);
                        
                        MyGuiManager.GetFullscreenQuad().Draw(effectGodRays);
                        
                        if (ApplyBlur)
                        {
                            var auxTarget = MyRender.GetRenderTarget(MyRenderTargets.AuxiliaryHalf1010102);

                            var blurEffect = MyRender.GetEffect(MyEffects.GaussianBlur) as MyEffectGaussianBlur;
                            blurEffect.SetHalfPixel(halfRT.GetLevelDescription(0).Width, halfRT.GetLevelDescription(0).Height);

                            // Apply vertical gaussian blur
                            MyMinerGame.SetRenderTarget(auxTarget, null);
                            blurEffect.BlurAmount = 1;
                            blurEffect.SetSourceTexture(halfRT);
                            blurEffect.SetHeightForVerticalPass(halfRT.GetLevelDescription(0).Height);
                            MyGuiManager.GetFullscreenQuad().Draw(blurEffect);

                            // Apply horizontal gaussian blur
                            MyMinerGame.SetRenderTarget(halfRT, null);
                            blurEffect.BlurAmount = 1;
                            blurEffect.SetSourceTexture(auxTarget);
                            blurEffect.SetWidthForHorisontalPass(auxTarget.GetLevelDescription(0).Width);
                            MyGuiManager.GetFullscreenQuad().Draw(blurEffect);
                        }
                                
                        // Additive
                        MyMinerGame.SetRenderTarget(availableRenderTarget, null);
                        //MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.All, new SharpDX.ColorBGRA(0), 1, 0);
                        BlendState.Opaque.Apply();
                        MyRender.Blit(source, true);

                        var upscaleEffect = MyRender.GetEffect(MyEffects.Scale) as MyEffectScale;
                        upscaleEffect.SetScale(new Vector2(2));
                        upscaleEffect.SetTechnique(MyEffectScale.Technique.HWScale);
                        MyStateObjects.Additive_NoAlphaWrite_BlendState.Apply();

                        upscaleEffect.SetSourceTextureMod(halfRT);
                        MyGuiManager.GetFullscreenQuad().Draw(upscaleEffect);
                                     
                                        /*
                        MyMinerGame.SetRenderTarget(availableRenderTarget, null);
                        var upscaleEffect = MyRender.GetEffect(MyEffects.Scale) as MyEffectScale;
                        upscaleEffect.SetScale(new Vector2(2));
                        upscaleEffect.SetTechnique(MyEffectScale.Technique.HWScale);
                        //MyStateObjects.Additive_NoAlphaWrite_BlendState.Apply();
                        BlendState.Opaque.Apply();

                        upscaleEffect.SetSourceTextureMod(halfRT);
                        MyGuiManager.GetFullscreenQuad().Draw(upscaleEffect);
                                          */
                        return availableRenderTarget;
                    }
                    break;
            }

            return source;
        }
    }
}

#region Using

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;

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
    class MyPostProcessVolumetricSSAO2 : MyPostProcessBase
    {
        public float MinRadius = 1.554f; 
        public float MaxRadius = 400; 
        public float RadiusGrowZScale = 10.0f;
        public float CameraZFar = 70294;   //71500  //70294

        public float Bias = 0.35f; 
        public float Falloff = 0.12f; 
        public float NormValue = 1.0f;   //1
        public float Contrast = 4f;  //4

        // SSAOParams.x = minRadius
// SSAOParams.y = maxRadius
// SSAOParams.z = radiusGrowZscale
// SSAOParams.w = camera zfar

// SSAOParams2.x = bias
// SSAOParams2.y = fallof
// SSAOParams2.z = occlusion samples normalization value * color scale
//uniform float4	SSAOParams2;


        /// <summary>
        /// Name of the post process
        /// </summary>
        public override MyPostProcessEnum Name { get { return MyPostProcessEnum.VolumetricSSAO2; } }
        public override string DisplayName { get { return "Volumetric SSAO 2"; } } 

        /// <summary>
        /// Enable state of post process
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled && MyRenderConstants.RenderQualityProfile.EnableSSAO;
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
                case PostProcessStage.LODBlend:
                    {
                       

                        MyEffectVolumetricSSAO2 volumetricSsao = MyRender.GetEffect(MyEffects.VolumetricSSAO) as MyEffectVolumetricSSAO2;

                        int width = MyRender.GetRenderTarget(MyRenderTargets.Normals).GetLevelDescription(0).Width;
                        int height = MyRender.GetRenderTarget(MyRenderTargets.Normals).GetLevelDescription(0).Height;
                        int halfWidth = width / 2;
                        int halfHeight = height / 2;

                        //Render SSAO
                        MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.SSAO), null);

                        MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, new SharpDX.ColorBGRA(0), 1, 0);
                        DepthStencilState.None.Apply();
                        BlendState.Opaque.Apply();

                        Vector4 ssaoParams = new Vector4(MinRadius, MaxRadius, RadiusGrowZScale, CameraZFar);
                        Vector4 ssaoParams2 = new Vector4(Bias, Falloff, NormValue, 0);

                        volumetricSsao.SetDepthsRT(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                        volumetricSsao.SetNormalsTexture(MyRender.GetRenderTarget(MyRenderTargets.Normals));
                        volumetricSsao.SetHalfPixel(width, height);

                        volumetricSsao.SetFrustumCorners(MyRender.GetShadowRenderer().GetFrustumCorners());

                        volumetricSsao.SetViewMatrix(MyCamera.ViewMatrixAtZero);

                        volumetricSsao.SetParams1(ssaoParams);
                        volumetricSsao.SetParams2(ssaoParams2);

                        volumetricSsao.SetProjectionMatrix(MyCamera.ProjectionMatrix);

                        volumetricSsao.SetContrast(Contrast);


                        MyGuiManager.GetFullscreenQuad().Draw(volumetricSsao);
                                  
                        if (volumetricSsao.UseBlur)
                        {
                            //SSAO Blur
                            MyMinerGame.SetRenderTarget(availableRenderTarget, null);
                            MyEffectSSAOBlur2 effectSsaoBlur = MyRender.GetEffect(MyEffects.SSAOBlur) as MyEffectSSAOBlur2;
                            effectSsaoBlur.SetDepthsRT(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                            //effectSsaoBlur.SetNormalsRT(MyRender.GetRenderTarget(MyRenderTargets.Normals));
                            effectSsaoBlur.SetHalfPixel(width, height);
                            effectSsaoBlur.SetSSAOHalfPixel(halfWidth, halfHeight);
                            effectSsaoBlur.SetSsaoRT(MyRender.GetRenderTarget(MyRenderTargets.SSAO));
                            effectSsaoBlur.SetBlurDirection(new Vector2(0, 1f / (float)halfHeight));
                            //effectSsaoBlur.SetBlurDirection(new Vector2(1 / (float)halfWidth, 1f / (float)halfHeight));

                            MyGuiManager.GetFullscreenQuad().Draw(effectSsaoBlur);

                            MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.SSAOBlur), null);
                            effectSsaoBlur.SetSsaoRT(availableRenderTarget);
                            effectSsaoBlur.SetBlurDirection(new Vector2(1f / (float)halfWidth, 0));
                            MyGuiManager.GetFullscreenQuad().Draw(effectSsaoBlur);
                        }

                        //Bake it into diffuse
                        /*
                        MyEffectScreenshot ssEffect = MyRender.GetEffect(MyEffects.Screenshot) as MyEffectScreenshot;     
                        MyMinerGame.SetRenderTarget(availableRenderTarget, null);
                        ssEffect.SetSourceTexture(MyRender.GetRenderTarget(MyRenderTargets.Diffuse));
                        ssEffect.SetScale(Vector2.One);
                        ssEffect.SetTechnique(MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
                        
                        MyGuiManager.GetFullscreenQuad().Draw(ssEffect);
                                          
                                         */ 
                        MyMinerGame.SetRenderTarget(MyRender.GetRenderTarget(MyRenderTargets.Diffuse), null);
                        /*
                        ssEffect.SetSourceTexture(availableRenderTarget);
                        ssEffect.SetTechnique(MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
                        ssEffect.SetScale(Vector2.One);
                        MyGuiManager.GetFullscreenQuad().Draw(ssEffect);
                                          */
                        MyEffectVolumetricSSAO2 effectVolumetricSsao = MyRender.GetEffect(MyEffects.VolumetricSSAO) as MyEffectVolumetricSSAO2;

                        //Blend with SSAO together
                        DepthStencilState.None.Apply();
                        
                        if (!effectVolumetricSsao.ShowOnlySSAO)
                        {
                            MyStateObjects.SSAO_BlendState.Apply();
                        }
                        else
                        {
                            MyRender.CurrentRenderSetup.EnableLights = false;
                            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, new SharpDX.ColorBGRA(1.0f), 1, 0);
                            MyStateObjects.SSAO_BlendState.Apply();
                        }


                        if (effectVolumetricSsao.UseBlur)
                            MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.SSAOBlur), 0, 0, MyCamera.Viewport.Width, MyCamera.Viewport.Height, Color.White);
                        else
                            MyGuiManager.DrawSpriteFast(MyRender.GetRenderTarget(MyRenderTargets.SSAO), 0, 0, MyCamera.Viewport.Width, MyCamera.Viewport.Height, Color.White);
 
                    }
                    break;
            }       
            return source;
        }
    }
}

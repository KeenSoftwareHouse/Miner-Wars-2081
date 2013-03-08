#region Using
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using SysUtils;

using SharpDX.Toolkit.Graphics;
using SharpDX.Direct3D9;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Antialiasing
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class MyPostProcessAntiAlias : MyPostProcessBase
    {

        /// <summary>
        /// Name of the post process
        /// </summary>
        public override MyPostProcessEnum Name { get { return MyPostProcessEnum.FXAA; } }
        public override string DisplayName { get { return "FXAA"; } }

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
                        BlendState.Opaque.Apply();
                        DepthStencilState.None.Apply();
                        RasterizerState.CullCounterClockwise.Apply();

                        MyMinerGame.SetRenderTarget(availableRenderTarget, null);

                        MyEffectAntiAlias effectAntiAlias = MyRender.GetEffect(MyEffects.AntiAlias) as MyEffectAntiAlias;
                        effectAntiAlias.SetDiffuseTexture(source);
                        effectAntiAlias.SetHalfPixel(source.GetLevelDescription(0).Width, source.GetLevelDescription(0).Height);

                        if (MyMwcFinalBuildConstants.EnableFxaa && MyRenderConstants.RenderQualityProfile.EnableFXAA)
                            effectAntiAlias.ApplyFxaa();
                        else
                            return source; // Nothing to do, return source

                        MyGuiManager.GetFullscreenQuad().Draw(effectAntiAlias);
                        return availableRenderTarget;
                    }
                    break;
            }
            return source;
        }
    }
}

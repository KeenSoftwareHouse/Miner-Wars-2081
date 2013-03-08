#region Using

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;

using SharpDX.Direct3D9;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    class MyPostProcessContrast : MyPostProcessBase
    {

        /// <summary>
        /// Name of the post process
        /// </summary>
        public override MyPostProcessEnum Name { get { return MyPostProcessEnum.Contrast; } }
        public override string DisplayName { get { return "Contrast"; } }

        public float Contrast = 0.0f;
        public float Hue = 0.0f;
        public float Saturation = 0.0f;

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
                case PostProcessStage.HDR:
                    {
                        MyMinerGame.SetRenderTarget(availableRenderTarget, null);
                        MyEffectContrast effectContrast = MyRender.GetEffect(MyEffects.Contrast) as MyEffectContrast;


                        effectContrast.SetDiffuseTexture(source);
                        effectContrast.SetHalfPixel(MyUtils.GetHalfPixel(source.GetLevelDescription(0).Width, source.GetLevelDescription(0).Height));
                        effectContrast.SetContrast(Contrast);
                        effectContrast.SetHue(Hue);
                        effectContrast.SetSaturation(Saturation);

                        MyGuiManager.GetFullscreenQuad().Draw(effectContrast);

                        return availableRenderTarget;
                    }
                    break;
            }

            return source;
        }
    }
}

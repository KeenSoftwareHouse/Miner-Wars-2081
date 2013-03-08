using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectScreenshot: MyEffectBase
    {
        public enum ScreenshotTechniqueEnum
        {
            Default,
            Color,
            HDR,
            Alpha,
            DepthToAlpha,
            LinearScale,
        }

        readonly EffectHandle m_source;
        readonly EffectHandle m_halfPixel;
        readonly EffectHandle m_scale;

        readonly EffectHandle m_defaultTechnique;
        readonly EffectHandle m_colorTechnique;
        readonly EffectHandle m_alphaTechnique;
        readonly EffectHandle m_hdrTechnique;
        readonly EffectHandle m_depthToAlpha;
        readonly EffectHandle m_linearTechnique;

        public MyEffectScreenshot()
            : base("Effects2\\Fullscreen\\MyEffectScreenshot")
        {
            m_source = m_D3DEffect.GetParameter(null, "SourceTexture");
            m_halfPixel = m_D3DEffect.GetParameter(null, "HalfPixel");
            m_scale = m_D3DEffect.GetParameter(null, "Scale");

            m_defaultTechnique = m_D3DEffect.GetTechnique("BasicTechnique");
            m_colorTechnique = m_D3DEffect.GetTechnique("ColorTechnique");
            m_alphaTechnique = m_D3DEffect.GetTechnique("AlphaTechnique");
            m_hdrTechnique = m_D3DEffect.GetTechnique("HDRTechnique");
            m_depthToAlpha = m_D3DEffect.GetTechnique("DepthToAlphaTechnique");
            m_linearTechnique = m_D3DEffect.GetTechnique("LinearTechnique");
        }

        public void SetSourceTexture(Texture renderTarget2D)
        {
            m_D3DEffect.SetTexture(m_source, renderTarget2D);
            m_D3DEffect.SetValue(m_halfPixel, MyUtils.GetHalfPixel(renderTarget2D.GetLevelDescription(0).Width, renderTarget2D.GetLevelDescription(0).Height));
        }

        public void SetScale(Vector2 scale)
        {
            m_D3DEffect.SetValue(m_scale, scale);
        }

        public void SetTechnique(ScreenshotTechniqueEnum technique)
        {
            switch (technique)
            {
                case ScreenshotTechniqueEnum.Default:
                    m_D3DEffect.Technique = m_defaultTechnique;
                    break;

                case ScreenshotTechniqueEnum.Color:
                    m_D3DEffect.Technique = m_colorTechnique;
                    break;

                case ScreenshotTechniqueEnum.HDR:
                    m_D3DEffect.Technique = m_hdrTechnique;
                    break;

                case ScreenshotTechniqueEnum.Alpha:
                    m_D3DEffect.Technique = m_alphaTechnique;
                    break;

                case ScreenshotTechniqueEnum.DepthToAlpha:
                    m_D3DEffect.Technique = m_depthToAlpha;
                    break;

                case ScreenshotTechniqueEnum.LinearScale:
                    m_D3DEffect.Technique = m_linearTechnique;
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }
        
    }
}

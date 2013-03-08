using MinerWars.AppCode.Game.Utils;

using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectDownsampleDepthForSSAO : MyEffectBase
    {
        readonly EffectHandle m_sourceDepthsRT;
        readonly EffectHandle m_halfPixel;

        public MyEffectDownsampleDepthForSSAO()
            : base("Effects2\\SSAO\\MyEffectDownsampleDepthForSSAO")
        {
            m_sourceDepthsRT = m_D3DEffect.GetParameter(null, "SourceDepthsRT");
            m_halfPixel = m_D3DEffect.GetParameter(null, "HalfPixel");
        }

        public void SetSourceDepthsRT(Texture renderTarget2D)
        {
            m_D3DEffect.SetTexture(m_sourceDepthsRT, renderTarget2D);
        }

        //  Set half-pixel and calculates 'quarter pixel' immediatelly
        public void SetHalfPixel(int screenSizeX, int screenSizeY)
        {
            Vector2 halfPixel = MyUtils.GetHalfPixel(screenSizeX, screenSizeY);
            m_D3DEffect.SetValue(m_halfPixel, halfPixel);
        }    
    }
}

using SysUtils.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectHudRadar : MyEffectBase
    {
        readonly EffectHandle m_billboardTexture;
        readonly EffectHandle m_viewProjectionMatrix;

        public MyEffectHudRadar()
            : base("Effects2\\HUD\\MyHudRadarEffect")
        {
            m_billboardTexture = m_D3DEffect.GetParameter(null, "Texture");
           m_viewProjectionMatrix = m_D3DEffect.GetParameter(null, "ViewProjectionMatrix");
        }

        public void SetBillboardTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_billboardTexture, texture);
        }

        public void SetViewProjectionMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_viewProjectionMatrix, matrix);
        }
    }
}

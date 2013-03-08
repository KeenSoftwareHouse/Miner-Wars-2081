using SysUtils.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectSolarMapGrid : MyEffectBase
    {
        readonly EffectHandle m_gridTexture;
        readonly EffectHandle m_colorA, m_alphaA;
        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_viewProjectionMatrix;


        public MyEffectSolarMapGrid()
            : base("Effects2\\SolarSystemMap\\MySolarSystemMapGrid")
        {
            m_gridTexture = m_D3DEffect.GetParameter(null, "GridTexture");
            m_colorA = m_D3DEffect.GetParameter(null, "ColorA");
            m_alphaA = m_D3DEffect.GetParameter(null, "AlphaA");
            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_viewProjectionMatrix = m_D3DEffect.GetParameter(null, "ViewProjectionMatrix");
        }

        public void SetGridTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_gridTexture, texture);
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_worldMatrix, matrix);
        }

        public void SetViewProjectionMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_viewProjectionMatrix, matrix);
        }

        public void SetColorA(Vector3 colorA)
        {
            m_D3DEffect.SetValue(m_colorA, colorA);
        }

        public void SetAlpha(float alpha)
        {
            m_D3DEffect.SetValue(m_alphaA, alpha);
        }
    }
}

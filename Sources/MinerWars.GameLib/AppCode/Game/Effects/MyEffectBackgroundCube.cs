using SysUtils.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectBackgroundCube : MyEffectBase
    {
        readonly EffectHandle m_backgroundTexture;
        readonly EffectHandle m_backgroundColor;
        readonly EffectHandle m_viewProjectionMatrix;

        public MyEffectBackgroundCube()
            : base("Effects2\\BackgroundCube\\MyBackgroundCube")
        {
            m_backgroundTexture = m_D3DEffect.GetParameter(null, "BackgroundTexture");
            m_backgroundColor = m_D3DEffect.GetParameter(null, "BackgroundColor");
            m_viewProjectionMatrix = m_D3DEffect.GetParameter(null, "ViewProjectionMatrix");
        }

        public void SetBackgroundTexture(CubeTexture texture)
        {
            m_D3DEffect.SetTexture(m_backgroundTexture, texture);
        }

        public void SetBackgroundColor(Vector3 color)
        {
            m_D3DEffect.SetValue(m_backgroundColor, color);
        }

        public void SetViewProjectionMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_viewProjectionMatrix, matrix);
        }
    }
}

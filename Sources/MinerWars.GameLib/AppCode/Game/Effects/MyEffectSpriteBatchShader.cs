using MinerWars.AppCode.Game.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectSpriteBatchShader : MyEffectBase
    {

        public enum Technique
        {
            LightsEnabled,
            LightsDisabled,
            OnlyLights
        }


        readonly EffectHandle m_Texture1;
        readonly EffectHandle m_Texture2;
        readonly EffectHandle m_TextureTiling;
        

        public MyEffectSpriteBatchShader()
            : base("Effects2\\Sprites\\MyEffectSpriteBatchShader")
        {
            m_Texture1 = m_D3DEffect.GetParameter(null, "Texture1");
            m_Texture2 = m_D3DEffect.GetParameter(null, "Texture2");
            m_TextureTiling = m_D3DEffect.GetParameter(null, "Texture2Tiling");
        }

        public void SetDiffuseTexture1(Texture textureToSet)
        {
             m_D3DEffect.SetTexture(m_Texture1, textureToSet);
        }

        public void SetDiffuseTexture2(Texture textureToSet)
        {
            m_D3DEffect.SetTexture(m_Texture2, textureToSet);
        }

        public void SetTexture2Tiling(Vector2 tiling)
        {
            m_D3DEffect.SetValue(m_TextureTiling, tiling);
        }

        public Effect GetEffect() { return m_D3DEffect; }
    }

}

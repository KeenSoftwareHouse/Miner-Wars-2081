using SysUtils.Utils;

using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectDecals : MyEffectBase
    {
        readonly EffectHandle m_voxelMapPosition;
        readonly EffectHandle m_techniqueVoxelDecals;
        readonly EffectHandle m_techniqueModelDecals;
        readonly EffectHandle m_techniqueVoxelDecalsForward;
        readonly EffectHandle m_techniqueModelDecalsForward;

        readonly EffectHandle m_decalDiffuseTexture;
        readonly EffectHandle m_decalNormalMapTexture;
        readonly EffectHandle m_fadeoutDistance;
        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_viewProjectionMatrix;
        readonly EffectHandle m_emissivityColor;

        public MyEffectDynamicLightingBase DynamicLights { get; private set; }
        public MyEffectReflectorBase Reflector { get; private set; }


        public enum Technique
        {
            Voxels,
            Model,
            VoxelsForward,
            ModelForward
        }

        public MyEffectDecals()
            : base("Effects2\\Decals\\MyDecalEffect")
        {
            m_voxelMapPosition = m_D3DEffect.GetParameter(null, "VoxelMapPosition");
            m_techniqueVoxelDecals = m_D3DEffect.GetTechnique("TechniqueVoxelDecals");
            m_techniqueModelDecals = m_D3DEffect.GetTechnique("TechniqueModelDecals");
            m_techniqueVoxelDecalsForward = m_D3DEffect.GetTechnique("TechniqueVoxelDecals_Forward");
            m_techniqueModelDecalsForward = m_D3DEffect.GetTechnique("TechniqueModelDecals_Forward");

            m_decalDiffuseTexture = m_D3DEffect.GetParameter(null, "DecalDiffuseTexture");
            m_decalNormalMapTexture = m_D3DEffect.GetParameter(null, "DecalNormalMapTexture");
            m_fadeoutDistance = m_D3DEffect.GetParameter(null, "FadeoutDistance");
            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_viewProjectionMatrix = m_D3DEffect.GetParameter(null, "ViewProjectionMatrix");
            m_emissivityColor = m_D3DEffect.GetParameter(null, "EmissiveColor");

            DynamicLights = new MyEffectDynamicLightingBase(m_D3DEffect);
            Reflector = new MyEffectReflectorBase(m_D3DEffect);
        }

        public void SetDecalDiffuseTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_decalDiffuseTexture, texture);
        }

        public void SetDecalNormalMapTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_decalNormalMapTexture, texture);
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_worldMatrix, matrix);
        }

        public void SetViewProjectionMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_viewProjectionMatrix, matrix);
        }

        public void SetFadeoutDistance(float distance)
        {
            m_D3DEffect.SetValue(m_fadeoutDistance, distance);
        }

        public void SetVoxelMapPosition(Vector3 pos)
        {
            m_D3DEffect.SetValue(m_voxelMapPosition, pos);
        }

        public void SetEmissivityColor(Vector4 color)
        {
            m_D3DEffect.SetValue(m_emissivityColor, color);
        }

        public void SetTechnique(Technique technique)
        {
            switch (technique)
            {
                case Technique.Voxels:
                     m_D3DEffect.Technique = m_techniqueVoxelDecals;
                    break;
                case Technique.Model:
                     m_D3DEffect.Technique = m_techniqueModelDecals;
                    break;

                case Technique.VoxelsForward:
                    m_D3DEffect.Technique = m_techniqueVoxelDecalsForward;
                    break;
                case Technique.ModelForward:
                    m_D3DEffect.Technique = m_techniqueModelDecalsForward;
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }
        }

        public override void Dispose()
        {
            DynamicLights.Dispose();
            Reflector.Dispose();
            base.Dispose();
        }

    }
}

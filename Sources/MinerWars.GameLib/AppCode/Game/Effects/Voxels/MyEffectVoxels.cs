using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectVoxels : MyEffectVoxelsBase
    {
        readonly EffectHandle m_voxelMapPosition;
        readonly EffectHandle m_viewMatrix;
        readonly EffectHandle m_diffuseColor;
        readonly EffectHandle m_highlightColor;
        
        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_enablePerVertexAmbient;

        public MyEffectVoxels()
            : base("Effects2\\Voxels\\MyEffectVoxels")
        {
            m_viewMatrix = m_D3DEffect.GetParameter(null, "ViewMatrix");
            m_voxelMapPosition = m_D3DEffect.GetParameter(null, "VoxelMapPosition");
            m_diffuseColor = m_D3DEffect.GetParameter(null, "DiffuseColor");
            m_highlightColor = m_D3DEffect.GetParameter(null, "Highlight");

            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_enablePerVertexAmbient = m_D3DEffect.GetParameter(null, "EnablePerVertexAmbient");
        }

        public void EnablePerVertexAmbient(bool bEnable)
        {
            m_D3DEffect.SetValue(m_enablePerVertexAmbient, bEnable ? 1 : 0);
        }

        public override void SetViewMatrix(ref Matrix matrix)
        {
            m_D3DEffect.SetValue(m_viewMatrix, matrix);
        }

        public void SetVoxelMapPosition(Vector3 pos)
        {
            m_D3DEffect.SetValue(m_voxelMapPosition, pos);
        }

        public override void SetDiffuseColor(Vector3 diffuseColorAdd)
        {
            m_D3DEffect.SetValue(m_diffuseColor, diffuseColorAdd);
        }

        public override void SetHighlightColor(Vector3 highlight)
        {
            m_D3DEffect.SetValue(m_highlightColor, highlight);
        }

        public void SetWorldMatrix(ref Matrix matrix)
        {
            m_D3DEffect.SetValue(m_worldMatrix, matrix);
        }
    }
}

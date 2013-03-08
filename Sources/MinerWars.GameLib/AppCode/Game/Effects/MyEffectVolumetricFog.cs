using MinerWars.AppCode.Game.Utils;
using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;

    class MyEffectVolumetricFog : MyEffectBase
    {
        public enum TechniqueEnum
        {
            Default,
            SkipBackground
        }


        readonly EffectHandle m_sourceRT;
        readonly EffectHandle m_depthsRT;
        readonly EffectHandle m_halfPixel;
        readonly EffectHandle m_normalsTexture;
        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_viewProjectionMatrix;
        readonly EffectHandle m_cameraPosition;
        readonly EffectHandle m_cameraMatrix;
        readonly EffectHandle m_frustumCorners;

        readonly EffectHandle m_defaultTechnique;
        readonly EffectHandle m_skipBackgroundTechnique;

        public MyEffectPerlinNoiseBase PerlinNoise { get; private set; }

        public MyEffectVolumetricFog()
            : base("Effects2\\Fullscreen\\MyEffectVolumetricFog")
        {
            m_sourceRT = m_D3DEffect.GetParameter(null, "SourceRT");
            m_depthsRT = m_D3DEffect.GetParameter(null, "DepthsRT");
            m_normalsTexture = m_D3DEffect.GetParameter(null, "NormalsTexture");
            m_halfPixel = m_D3DEffect.GetParameter(null, "HalfPixel");
            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_viewProjectionMatrix = m_D3DEffect.GetParameter(null, "ViewProjectionMatrix");
            m_cameraPosition = m_D3DEffect.GetParameter(null, "CameraPos");
            m_cameraMatrix = m_D3DEffect.GetParameter(null, "CameraMatrix");
            m_frustumCorners = m_D3DEffect.GetParameter(null, "FrustumCorners");

            m_defaultTechnique = m_D3DEffect.GetTechnique("Technique1");
            m_skipBackgroundTechnique = m_D3DEffect.GetTechnique("SkipBackgroundTechnique");

            PerlinNoise = new MyEffectPerlinNoiseBase(m_D3DEffect);
        }

        public void SetSourceRT(Texture renderTarget2D)
        {
            m_D3DEffect.SetTexture(m_sourceRT, renderTarget2D);
        }

        public void SetDepthsRT(Texture renderTarget2D)
        {
            m_D3DEffect.SetTexture(m_depthsRT, renderTarget2D);
        }

        public void SetHalfPixel(int screenSizeX, int screenSizeY)
        {
            m_D3DEffect.SetValue(m_halfPixel, MyUtils.GetHalfPixel(screenSizeX, screenSizeY));
        }

        public void SetNormalsTexture(Texture normalsTexture)
        {
            m_D3DEffect.SetTexture(m_normalsTexture, normalsTexture);
        }

        public void SetViewProjectionMatrix(Matrix viewProjectionMatrix)
        {
            m_D3DEffect.SetValue(m_viewProjectionMatrix, viewProjectionMatrix);
        }

        public void SetWorldMatrix(Matrix worldMatrix)
        {
            m_D3DEffect.SetValue(m_worldMatrix, worldMatrix);
        }

        public void SetCameraPosition(Vector3 pos)
        {
            m_D3DEffect.SetValue(m_cameraPosition, pos);
        }

        public void SetCameraMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_cameraMatrix, matrix);
        }

        public void SetFrustumCorners(Vector3[] frustumCornersVS)
        {
            m_D3DEffect.SetValue(m_frustumCorners, frustumCornersVS);
        }

        public void SetTechnique(TechniqueEnum technique)
        {
            switch(technique)
            {
                case TechniqueEnum.Default:
                    m_D3DEffect.Technique = m_defaultTechnique;
                    break;
                case TechniqueEnum.SkipBackground:
                    m_D3DEffect.Technique = m_skipBackgroundTechnique;
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        public override void Dispose()
        {
            PerlinNoise.Dispose();
            base.Dispose();
        }
    }
}

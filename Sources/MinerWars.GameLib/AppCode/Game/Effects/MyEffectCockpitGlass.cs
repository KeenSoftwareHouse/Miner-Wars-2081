using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Matrix = MinerWarsMath.Matrix;
    using MinerWars.AppCode.Game.Render;

    class MyEffectCockpitGlass : MyEffectDynamicLightingBase
    {
        public enum MyEffectCockpitGlassTechniqueEnum
        {
            Default,
            Forward
        }

        readonly EffectHandle m_cockpitGlassTexture;
        readonly EffectHandle m_cockpitInteriorLight;
        readonly EffectHandle m_glassDirtLevelAlpha;
        readonly EffectHandle m_depthTexture;
        readonly EffectHandle m_halfPixel;
        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_viewMatrix;
        readonly EffectHandle m_worldViewProjectionMatrix;
        readonly EffectHandle m_reflectorPosition;
        readonly EffectHandle m_nearLightRange;
        readonly EffectHandle m_nearLightColor;

        readonly EffectHandle m_defaultTechnique;
        readonly EffectHandle m_forwardTechnique;

        public MyEffectCockpitGlass()
            : base("Effects2\\HUD\\MyEffectCockpitGlass")
        {
            m_cockpitGlassTexture = m_D3DEffect.GetParameter(null, "CockpitGlassTexture");
            m_cockpitInteriorLight = m_D3DEffect.GetParameter(null, "CockpitInteriorLight");
            m_glassDirtLevelAlpha = m_D3DEffect.GetParameter(null, "GlassDirtLevelAlpha");
            m_depthTexture = m_D3DEffect.GetParameter(null, "DepthsRT");
            m_halfPixel = m_D3DEffect.GetParameter(null, "HalfPixel");
            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_viewMatrix = m_D3DEffect.GetParameter(null, "ViewMatrix");
            m_worldViewProjectionMatrix = m_D3DEffect.GetParameter(null, "WorldViewProjectionMatrix");
            m_reflectorPosition = m_D3DEffect.GetParameter(null, "ReflectorPosition");
            m_nearLightRange = m_D3DEffect.GetParameter(null, "NearLightRange");
            m_nearLightColor = m_D3DEffect.GetParameter(null, "NearLightColor");

            m_defaultTechnique = m_D3DEffect.GetTechnique("GlassDefault");
            m_forwardTechnique = m_D3DEffect.GetTechnique("GlassForward");
        }

        public void SetCockpitGlassTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_cockpitGlassTexture, texture);
        }

        public void SetGlassDirtLevelAlpha(Vector4 alpha)
        {
            m_D3DEffect.SetValue(m_glassDirtLevelAlpha, alpha);
        }

        public void SetDepthTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_depthTexture, texture);
        }

        public void SetHalfPixel(Vector2 halfPixel)
        {
            m_D3DEffect.SetValue(m_halfPixel, halfPixel);
        }

        public void SetWorldMatrix(Matrix worldMatrix)
        {
            m_D3DEffect.SetValue(m_worldMatrix, worldMatrix);
        }

        public void SetViewMatrix(Matrix viewMatrix)
        {
            m_D3DEffect.SetValue(m_viewMatrix, viewMatrix);
        }

        public void SetWorldViewProjectionMatrix(Matrix matrix)
        {
            m_D3DEffect.SetValue(m_worldViewProjectionMatrix, matrix);
        }


        public void SetReflectorPosition(Vector3 pos)
        {
            m_D3DEffect.SetValue(m_reflectorPosition, pos);
        }

        public void SetNearLightRange(float range)
        {
            m_D3DEffect.SetValue(m_nearLightRange, range);
        }

        public void SetNearLightColor(Vector4 color)
        {
            m_D3DEffect.SetValue(m_nearLightColor, color);
        }

        void SetTechnique(MyEffectCockpitGlassTechniqueEnum technique)
        {
            switch (technique)
            {
                case MyEffectCockpitGlassTechniqueEnum.Default:
                    m_D3DEffect.Technique = m_defaultTechnique;
                    break;

                case MyEffectCockpitGlassTechniqueEnum.Forward:
                    m_D3DEffect.Technique = m_forwardTechnique;
                    break;
    }
}

        public override void Begin(int pass = 0, FX fx = FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState)
        {
            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                SetTechnique(MyEffectCockpitGlassTechniqueEnum.Forward);
            else
                SetTechnique(MyEffectCockpitGlassTechniqueEnum.Default);

            base.Begin(pass);
        }
    }
}

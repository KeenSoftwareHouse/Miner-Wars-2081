using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

using SharpDX.Direct3D9;

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;

    class MyEffectTransparentGeometry : MyEffectDynamicLightingBase
    {
        public enum Technique
        {
            Lit,
            Unlit,
            UnlitForward,
            IgnoreDepth,
            ColorizeHeight,
            VisualizeOverdraw
        }

        readonly EffectHandle m_worldMatrix;
        readonly EffectHandle m_viewMatrix;
        readonly EffectHandle m_projectionMatrix;

        readonly EffectHandle m_worldViewMatrix;
        readonly EffectHandle m_worldViewProjectionMatrix;

        readonly EffectHandle m_billboardTexture;
        readonly EffectHandle m_billboardBlendTexture;
        readonly EffectHandle m_billboardBlendRatio;
        readonly EffectHandle m_depthsRT;
        readonly EffectHandle m_halfPixel;
        readonly EffectHandle m_scale;
        readonly EffectHandle m_softParticleDistanceScale;

        readonly EffectHandle m_colorizeColor, m_colorizePlaneNormal, m_colorizePlaneDistance, m_colorizeSoftDistance;

        readonly EffectHandle m_alphaMultiplier;

        readonly EffectHandle m_litBasicTechnique;
        readonly EffectHandle m_unlitBasicTechnique;
        readonly EffectHandle m_unlitBasicForwardTechnique;
        readonly EffectHandle m_ignoreDepthBasicTechnique;
        readonly EffectHandle m_colorizeTechnique;

        readonly EffectHandle m_visualizeOverdrawTechnique;

        Matrix m_world, m_view, m_projection;
   
        public MyEffectReflectorBase Reflector { get; private set; }
   
        public MyEffectTransparentGeometry()
            : base("Effects2\\TransparentGeometry\\MyEffectTransparentGeometry")
        {
            m_worldMatrix = m_D3DEffect.GetParameter(null, "WorldMatrix");
            m_projectionMatrix = m_D3DEffect.GetParameter(null, "ProjectionMatrix");
            m_viewMatrix = m_D3DEffect.GetParameter(null, "ViewMatrix");

            m_worldViewMatrix = m_D3DEffect.GetParameter(null, "WorldViewMatrix");
            m_worldViewProjectionMatrix = m_D3DEffect.GetParameter(null, "WorldViewProjectionMatrix");

            m_billboardTexture = m_D3DEffect.GetParameter(null, "BillboardTexture");
            m_billboardBlendTexture = m_D3DEffect.GetParameter(null, "BillboardBlendTexture");
            m_billboardBlendRatio = m_D3DEffect.GetParameter(null, "BillboardBlendRatio");
            m_depthsRT = m_D3DEffect.GetParameter(null, "DepthsRT");
            m_halfPixel = m_D3DEffect.GetParameter(null, "HalfPixel");
            m_scale = m_D3DEffect.GetParameter(null, "Scale");
            m_softParticleDistanceScale = m_D3DEffect.GetParameter(null, "SoftParticleDistanceScale");

            m_colorizeColor = m_D3DEffect.GetParameter(null, "ColorizeColor");
            m_colorizePlaneNormal = m_D3DEffect.GetParameter(null, "ColorizePlaneNormal");
            m_colorizePlaneDistance = m_D3DEffect.GetParameter(null, "ColorizePlaneDistance");
            m_colorizeSoftDistance = m_D3DEffect.GetParameter(null, "ColorizeSoftDistance");

            m_alphaMultiplier = m_D3DEffect.GetParameter(null, "AlphaMultiplierSaturation");

            m_litBasicTechnique = m_D3DEffect.GetTechnique("Technique_LitBasic");
            m_unlitBasicTechnique = m_D3DEffect.GetTechnique("Technique_UnlitBasic");
            m_unlitBasicForwardTechnique = m_D3DEffect.GetTechnique("Technique_UnlitBasic_Forward");
            m_ignoreDepthBasicTechnique = m_D3DEffect.GetTechnique("Technique_IgnoreDepthBasic");
            m_colorizeTechnique = m_D3DEffect.GetTechnique("Technique_ColorizeHeight");

            m_visualizeOverdrawTechnique = m_D3DEffect.GetTechnique("Technique_VisualizeOverdraw");

            Reflector = new MyEffectReflectorBase(m_D3DEffect);
        }

        public void SetWorldMatrix(Matrix worldMatrix)
        {
            m_world = worldMatrix;
            m_D3DEffect.SetValue(m_worldMatrix, worldMatrix);
        }

        public override void SetViewMatrix(ref Matrix viewMatrix)
        {
            m_view = viewMatrix;
            m_D3DEffect.SetValue(m_viewMatrix, viewMatrix);
        }

        public override void SetProjectionMatrix(ref Matrix projectionMatrix)
        {
            m_projection = projectionMatrix;
            m_D3DEffect.SetValue(m_projectionMatrix, projectionMatrix);
        }

        public void SetBillboardTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_billboardTexture, texture);
        }

        public void SetBillboardBlendTexture(Texture texture)
        {
            m_D3DEffect.SetTexture(m_billboardBlendTexture, texture);
        }

        public void SetDepthsRT(Texture renderTarget2D)
        {
            m_D3DEffect.SetTexture(m_depthsRT, renderTarget2D);
        }

        public void SetHalfPixel(int screenSizeX, int screenSizeY)
        {
            m_D3DEffect.SetValue(m_halfPixel, MyUtils.GetHalfPixel(screenSizeX, screenSizeY));
        }

        public void SetScale(Vector2 scale)
        {
            m_D3DEffect.SetValue(m_scale, scale);
        }

        public void SetSoftParticleDistanceScale(float softParticleDistanceScale)
        {
            m_D3DEffect.SetValue(m_softParticleDistanceScale, softParticleDistanceScale);
        }

        public void SetColorizeColor(Color color)
        {
            m_D3DEffect.SetValue(m_colorizeColor, color.ToVector4());
        }

        public void SetColorizePlane(Vector3 planeNormal, float planeDistance)
        {
            m_D3DEffect.SetValue(m_colorizePlaneNormal, planeNormal);
            m_D3DEffect.SetValue(m_colorizePlaneDistance, planeDistance);
        }

        public void SetColorizeSoftDistance(float distance)
        {
            m_D3DEffect.SetValue(m_colorizeSoftDistance, distance);
        }

        public override void Begin(int pass = 0, FX fx = FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState)
        {
            m_D3DEffect.SetValue(m_worldViewMatrix, m_world * m_view);
            m_D3DEffect.SetValue(m_worldViewProjectionMatrix, m_world * m_view * m_projection);
            base.Begin(pass);
        }

        public void SetTechnique(Technique technique)
        {
            switch (technique)
            {
                case Technique.Lit:
                    m_D3DEffect.Technique = m_litBasicTechnique;
                    break;
                case Technique.Unlit:
                    m_D3DEffect.Technique = m_unlitBasicTechnique;
                    break;
                case Technique.UnlitForward:
                    m_D3DEffect.Technique = m_unlitBasicForwardTechnique;
                    break;
                case Technique.IgnoreDepth:
                    m_D3DEffect.Technique = m_ignoreDepthBasicTechnique;
                    break;
                case Technique.ColorizeHeight:
                    m_D3DEffect.Technique = m_colorizeTechnique;
                    break;

                case Technique.VisualizeOverdraw:
                    m_D3DEffect.Technique = m_visualizeOverdrawTechnique;
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;

            }
        }

        public void SetAlphaMultiplierAndSaturation(float alphaMultiplier, float alphaSaturation)
        {
            m_D3DEffect.SetValue(m_alphaMultiplier, new Vector2(alphaMultiplier, alphaSaturation));
        }

        public override void Dispose()
        {
            Reflector.Dispose();
            base.Dispose();
        }
    }
}

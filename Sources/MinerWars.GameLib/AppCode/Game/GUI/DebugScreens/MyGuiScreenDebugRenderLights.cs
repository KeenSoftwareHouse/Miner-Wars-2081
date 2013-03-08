using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Entities.SubObjects;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugRenderLights : MyGuiScreenDebugBase
    {
        public static bool EnableRenderLights = true;

        public MyGuiScreenDebugRenderLights()
            : base(0.35f * Color.Yellow.ToVector4(), false)
        {
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            m_isTopMostScreen = false;
            m_canHaveFocus = false;

            RecreateControls(true);
        }

        public override void RecreateControls(bool contructor)
        {
            Controls.Clear();

            m_scale = 0.7f;

            AddCaption(new System.Text.StringBuilder("Render Lights debug"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Lights"), Color.Yellow.ToVector4(), 1.2f);

            AddCheckBox(new StringBuilder("Enable lights"), null, MemberHelper.GetMember(() => EnableRenderLights));
            AddCheckBox(new StringBuilder("Enable light glares"), null, MemberHelper.GetMember(() => MyLightGlare.EnableLightGlares));
            AddCheckBox(new StringBuilder("Enable spot shadows"), null, MemberHelper.GetMember(() => MyRender.EnableSpotShadows));
            AddCheckBox(new StringBuilder("Enable spectator light"), null, MemberHelper.GetMember(() => MyRender.EnableSpectatorReflector));
            AddCheckBox(new StringBuilder("Only specular intensity"), null, MemberHelper.GetMember(() => MyRender.ShowSpecularIntensity));
            AddCheckBox(new StringBuilder("Only specular power"), null, MemberHelper.GetMember(() => MyRender.ShowSpecularPower));
            AddCheckBox(new StringBuilder("Only emissivity"), null, MemberHelper.GetMember(() => MyRender.ShowEmissivity));
            AddCheckBox(new StringBuilder("Only reflectivity"), null, MemberHelper.GetMember(() => MyRender.ShowReflectivity));

            MyEffectShadowMap shadowMapEffect = MyRender.GetEffect(MyEffects.ShadowMap) as MyEffectShadowMap;

            if (shadowMapEffect != null)
            {
                m_currentPosition.Y += 0.01f;
                AddLabel(new StringBuilder("Sun"), Color.Yellow.ToVector4(), 1.2f);
                AddCheckBox(new StringBuilder("Enable sun"), null, MemberHelper.GetMember(() => MyRender.EnableSun));
                AddCheckBox(new StringBuilder("Enable shadows"), null, MemberHelper.GetMember(() => MyRender.EnableShadows));
                AddCheckBox(new StringBuilder("Enable asteroid shadows"), null, MemberHelper.GetMember(() => MyRender.EnableAsteroidShadows));
                AddCheckBox(new StringBuilder("Enable ambient map"), MyRender.EnableEnvironmentMapAmbient, OnAmbientMapChecked);
                AddCheckBox(new StringBuilder("Enable reflection map"), MyRender.EnableEnvironmentMapReflection, OnReflectionMapChecked);
                AddCheckBox(new StringBuilder("Enable voxel ambient"), null, MemberHelper.GetMember(() => MyRender.EnablePerVertexVoxelAmbient));
                AddSlider(new StringBuilder("Intensity"), 0, 10.0f, MySector.SunProperties, MemberHelper.GetMember(() => MySector.SunProperties.SunIntensity));
                AddCheckBox(new StringBuilder("Show cascade splits"), null, MemberHelper.GetMember(() => MyRender.ShowCascadeSplits));
                //nefunguje po obfuskaci
               // AddSlider(new StringBuilder("ShadowBias"), 0, 0.01f, shadowMapEffect, MemberHelper.GetMember(() => shadowMapEffect.ShadowBias));

                AddCheckBox(new StringBuilder("Enable shadow interleaving"), null, MemberHelper.GetMember(() => MyRender.ShadowInterleaving));
                AddCheckBox(new StringBuilder("Freeze cascade 0"), null, MemberHelper.GetMember(() => MyRender.FreezeCascade0));
                AddCheckBox(new StringBuilder("Freeze cascade 1"), null, MemberHelper.GetMember(() => MyRender.FreezeCascade1));
                AddCheckBox(new StringBuilder("Freeze cascade 2"), null, MemberHelper.GetMember(() => MyRender.FreezeCascade2));
                AddCheckBox(new StringBuilder("Freeze cascade 3"), null, MemberHelper.GetMember(() => MyRender.FreezeCascade3));
            }               
        }

        void OnAmbientMapChecked(MyGuiControlCheckbox checkbox)
        {
            MyRender.EnableEnvironmentMapAmbient = checkbox.Checked;
        }

        void OnReflectionMapChecked(MyGuiControlCheckbox checkbox)
        {
            MyRender.EnableEnvironmentMapReflection = checkbox.Checked;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderLights";
        }

    }
}

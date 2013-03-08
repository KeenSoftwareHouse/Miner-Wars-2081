using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugVolumetricSSAO : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugVolumetricSSAO()
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

            AddCaption(new StringBuilder("Volumetric SSAO Debug"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                                   MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            MyPostProcessVolumetricSSAO2 volumetricSsaoPP = MyRender.GetPostProcess(MyPostProcessEnum.VolumetricSSAO2) as MyPostProcessVolumetricSSAO2;
            if (volumetricSsaoPP == null)
            {
                label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.1f), null, new System.Text.StringBuilder("Sorry SSAO post process is not available"), Color.Red.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                                       MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                Controls.Add(label);
                return;
            }


            MyEffectVolumetricSSAO2 volumetricSsaoEffect = MyRender.GetEffect(MyEffects.VolumetricSSAO) as MyEffectVolumetricSSAO2;
                                         
            AddCheckBox(new StringBuilder("Use SSAO"), volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.Enabled));
            AddCheckBox(new StringBuilder("Use blur"), volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.UseBlur));
            AddCheckBox(new StringBuilder("Show only SSAO"), volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.ShowOnlySSAO));

            m_currentPosition.Y += 0.01f;

//            AddSlider(new StringBuilder("Offset"), 0, 10, volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.Offset));
  //          AddSlider(new StringBuilder("SiluetteDist"), 0, 10, volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.SiluetteDist));
    //        AddSlider(new StringBuilder("R"), 0, 10, volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.R));
                                 

            AddSlider(new StringBuilder("MinRadius"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.MinRadius));
            AddSlider(new StringBuilder("MaxRadius"), 0, 1000, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.MaxRadius));
            AddSlider(new StringBuilder("RadiusGrowZScale"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.RadiusGrowZScale));
            AddSlider(new StringBuilder("CameraZFar"), 0, 100000, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.CameraZFar));

            AddSlider(new StringBuilder("Bias"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.Bias));
            AddSlider(new StringBuilder("Falloff"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.Falloff));
            AddSlider(new StringBuilder("NormValue"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.NormValue));
            //AddSlider(new StringBuilder("ColorScale"), 0, 10, volumetricSsaoEffect, MemberHelper.GetMember(() => volumetricSsaoEffect.ColorScale));
            AddSlider(new StringBuilder("Contrast"), 0, 10, volumetricSsaoPP, MemberHelper.GetMember(() => volumetricSsaoPP.Contrast));

            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugSSAO";
        }
    }
}

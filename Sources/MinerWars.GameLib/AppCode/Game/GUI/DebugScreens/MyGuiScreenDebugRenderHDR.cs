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
    //Prepared to be render debug screen

    class MyGuiScreenDebugRenderHDR : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugRenderHDR()
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

            AddCaption(new System.Text.StringBuilder("Render HDR"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f * m_scale;

            MyPostProcessHDR hdr = MyRender.GetPostProcess(MyPostProcessEnum.HDR) as MyPostProcessHDR;
            if (hdr != null)
            {
                AddLabel(new StringBuilder("HDR"), Color.Yellow.ToVector4(), 1.2f);

                AddCheckBox(new StringBuilder("Enable HDR and bloom"), null, MemberHelper.GetMember(() => MyPostProcessHDR.DebugHDRChecked));

                m_currentPosition.Y += 0.01f * m_scale;

                AddSlider(new StringBuilder("Exposure"), 0, 6.0f, hdr, MemberHelper.GetMember(() => hdr.Exposure));
                AddSlider(new StringBuilder("Bloom Threshold"), 0, 4.0f, hdr, MemberHelper.GetMember(() => hdr.Threshold));
                AddSlider(new StringBuilder("Bloom Intensity"), 0, 4.0f, hdr, MemberHelper.GetMember(() => hdr.BloomIntensity));
                AddSlider(new StringBuilder("Bloom Intensity for Background"), 0, 1.5f, hdr, MemberHelper.GetMember(() => hdr.BloomIntensityBackground));
                AddSlider(new StringBuilder("Vertical Blur Amount"), 1.0f, 8.0f, hdr, MemberHelper.GetMember(() => hdr.VerticalBlurAmount));
                AddSlider(new StringBuilder("Horizontal Blur Amount"), 1.0f, 8.0f, hdr, MemberHelper.GetMember(() => hdr.HorizontalBlurAmount));
                AddSlider(new StringBuilder("Number of blur passes (integer)"), 1.0f, 8.0f, hdr, MemberHelper.GetMember(() => hdr.NumberOfBlurPasses));

                m_currentPosition.Y += 0.01f * m_scale;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugRenderHDR";
        }

    }
}

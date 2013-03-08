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

    class MyGuiScreenRenderModules : MyGuiScreenDebugBase
    {
        public MyGuiScreenRenderModules()
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

            AddCaption(new System.Text.StringBuilder("Render modules"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_scale = 0.7f;

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;
               
            AddLabel(new StringBuilder("Prepare for draw"), Color.Yellow.ToVector4(), 1.2f);
            foreach (MyRender.MyRenderModuleItem renderModule in MyRender.GetRenderModules(MyRenderStage.PrepareForDraw))
            {
                AddCheckBox(new StringBuilder(renderModule.DisplayName), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));
            }

            AddLabel(new StringBuilder("Background"), Color.Yellow.ToVector4(), 1.2f);
            foreach (MyRender.MyRenderModuleItem renderModule in MyRender.GetRenderModules(MyRenderStage.Background))
            {
                AddCheckBox(new StringBuilder(renderModule.DisplayName), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));
            }

            AddLabel(new StringBuilder("Pre-HDR Alpha blend"), Color.Yellow.ToVector4(), 1.2f);
            foreach (MyRender.MyRenderModuleItem renderModule in MyRender.GetRenderModules(MyRenderStage.AlphaBlendPreHDR))
            {
                AddCheckBox(new StringBuilder(renderModule.DisplayName), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));
            }

            AddLabel(new StringBuilder("Alpha blend"), Color.Yellow.ToVector4(), 1.2f);
            foreach (MyRender.MyRenderModuleItem renderModule in MyRender.GetRenderModules(MyRenderStage.AlphaBlend))
            {
                AddCheckBox(new StringBuilder(renderModule.DisplayName), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));
            }

            AddLabel(new StringBuilder("Debug draw"), Color.Yellow.ToVector4(), 1.2f);
            foreach (MyRender.MyRenderModuleItem renderModule in MyRender.GetRenderModules(MyRenderStage.DebugDraw))
            {
                AddCheckBox(new StringBuilder(renderModule.DisplayName), renderModule, MemberHelper.GetMember(() => renderModule.Enabled));
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenRenderModules";
        }

    }
}

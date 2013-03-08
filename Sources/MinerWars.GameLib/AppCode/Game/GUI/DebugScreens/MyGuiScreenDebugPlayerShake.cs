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
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.GUI.DebugScreens
{
    class MyGuiScreenDebugPlayerShake : MyGuiScreenDebugBase
    {
        public MyGuiScreenDebugPlayerShake()
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

            AddCaption(new StringBuilder("Player Head Shake"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                                   MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);

            m_currentPosition.Y += 0.01f;

            MySmallShip ship = MySession.PlayerShip;

            if (ship != null)
            {
                AddSlider(new StringBuilder("MaxShake"), 0, 30, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_MaxShake));
                AddSlider(new StringBuilder("MaxShakePos"), 0, 1, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_MaxShakePos));
                AddSlider(new StringBuilder("MaxShakeDir"), 0, 1, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_MaxShakeDir));
                AddSlider(new StringBuilder("Reduction"), 0, 1, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_Reduction));
                AddSlider(new StringBuilder("Damping"), 0, 1, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_Damping));
                AddSlider(new StringBuilder("OffConstant"), 0, 1, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_OffConstant));
                AddSlider(new StringBuilder("DirReduction"), 0, 30, ship.GetShake(), MemberHelper.GetMember(() => ship.GetShake().m_DirReduction));
            }

            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugPlayerShake";
        }
    }

}

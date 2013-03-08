using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenMultiplayerEnterGameRequest : MyGuiScreenBase
    {
        private MyGuiScreenGamePlay.MyJoinGameRequest m_request;
        private MyGuiControlCheckbox m_chkRememberSetting;

        public MyGuiScreenMultiplayerEnterGameRequest(MyGuiScreenGamePlay.MyJoinGameRequest request)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 600 / 1200f), false, MyGuiManager.GetSandboxBackgoround())
        {
            m_enableBackgroundFade = true;
            m_backgroundFadeColor = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);

            m_request = request;
            AddCaption(MyTextsWrapperEnum.JoinRequestTitle);

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.146f);
            MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            Controls.Add(new MyGuiControlLabel(this, menuPositionOrigin + 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, null, new StringBuilder(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.JoinRequest, request.Message.PlayerInfo.DisplayName)), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiConstants.DEFAULT_CONTROL_FONT));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.AllowEnter, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAllowEnter, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.DenyEnter, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnDenyEnter, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            MyGuiControlLabel lblRememberSetting = new MyGuiControlLabel(this, menuPositionOrigin + 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA - new Vector2(0.03f, 0), null, MyTextsWrapperEnum.Remember, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            Controls.Add(lblRememberSetting);

            m_chkRememberSetting = new MyGuiControlCheckbox(this, menuPositionOrigin + 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(0.05f, 0), false, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR, lblRememberSetting);
            Controls.Add(m_chkRememberSetting);
        }

        public void OnAllowEnter(MyGuiControlButton sender)
        {
            MyMultiplayerGameplay.Static.AllowEnter(ref m_request.Message);
            if (m_chkRememberSetting.Checked)
            {
                MyMultiplayerGameplay.Static.JoinMode = MyJoinMode.Open;
                MyMultiplayerGameplay.Static.UpdateGameInfo();
            }
            CloseScreen();
        }

        public void OnDenyEnter(MyGuiControlButton sender)
        {
            MyMultiplayerGameplay.Static.DenyEnter(ref m_request.Message);
            if (m_chkRememberSetting.Checked)
            {
                MyMultiplayerGameplay.Static.JoinMode = MyJoinMode.Closed;
                MyMultiplayerGameplay.Static.UpdateGameInfo();
            }

            CloseScreen();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenMultiplayerEnterGameRequest";
        }
    }
}

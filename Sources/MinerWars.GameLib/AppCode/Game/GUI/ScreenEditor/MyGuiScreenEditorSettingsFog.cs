using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using System;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    [Obsolete("Isn't used anymore")]
    class MyGuiScreenEditorSettingsSun : MyGuiScreenBase
    {
        public MyGuiScreenEditorSettingsSun()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;

            m_size = new Vector2(0.45f, 0.4f);

            //TODO
            //AddCaption(MyTextsWrapperEnum.AudioOptions);

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.03f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.175f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            
            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSettingsSun";
        }

        private void Save()
        {
            //TODO
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            //  Save values
            Save();
            CloseScreen();
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {           
            CloseScreen();
        }
    }
}

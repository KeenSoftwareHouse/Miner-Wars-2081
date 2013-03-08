using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using SysUtils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenOptions : MyGuiScreenBase
    {
        bool m_showVideoOption;

        public MyGuiScreenOptions(bool showVideoOption)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 695 / 1200f), false, MyGuiManager.GetSandboxBackgoround())
        {
            m_enableBackgroundFade = true;
            m_showVideoOption = showVideoOption;

            RecreateControls(true);
        }

        public override void RecreateControls(bool contructor)
        {
            Controls.Clear();

            AddCaption(MyTextsWrapperEnum.Options, new Vector2(0, 0.005f));

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.146f);
            MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            int index = 0;

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Game, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnGameClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Video, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnVideoClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, m_showVideoOption ? (MyTextsWrapperEnum?)null : MyTextsWrapperEnum.FeatureAccessibleOnlyFromMainMenu));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Audio, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAudioClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Controls, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnControlsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            var backButton = new MyGuiControlButton(this, new Vector2(0, 0.178f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(backButton);

        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenOptions";
        }

        public void OnGameClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenOptionsGame());
        }

        public void OnVideoClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("MyGuiScreenOptions.OnVideoClick START");

            MyGuiManager.AddScreen(new MyGuiScreenOptionsVideo());

            MyMwcLog.WriteLine("MyGuiScreenOptions.OnVideoClick END");
        }

        public void OnAudioClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenOptionsAudio());
        }

        public void OnControlsClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenOptionsControls());
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            //  Just close the screen
            CloseScreen();
        }
    }
}

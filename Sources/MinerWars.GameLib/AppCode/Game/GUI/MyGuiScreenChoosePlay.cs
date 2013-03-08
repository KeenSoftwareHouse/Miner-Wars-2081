using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenChoosePlay : MyGuiScreenBase
    {
        MyGuiScreenBase m_closeAfterSuccessfulEnter;

        public MyGuiScreenChoosePlay(MyGuiScreenBase closeAfterSuccessfulEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.25f, 0.35f))
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_enableBackgroundFade = true;
            AddCaption(MyTextsWrapperEnum.ChooseGameType, new Vector2(0, 0.005f));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.125f);
            Vector2 buttonDelta = new Vector2(0.1f, 0);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            int index = 0;
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.PlayStory, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnPlayStoryClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.PLAY_STORY_BUTTON_IMPLEMENTED));
            
            // MMO is now disabled
            //Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
            //    MyTextsWrapperEnum.PlayMMO, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnPlayMMOClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.PLAY_MMO_BUTTON_IMPLEMENTED));
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + index++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.PlaySandbox, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnPlaySandboxClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.PLAY_SANDBOX_BUTTON_IMPLEMENTED));

            Controls.Add(new MyGuiControlButton(this, new Vector2(0.00f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnBackClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenChoosePlay";
        }

        public void OnPlayStoryClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenSelectStory(m_closeAfterSuccessfulEnter));
        }

        public void OnPlayMMOClick(MyGuiControlButton sender)
        {
            //  TODO: See implementation of OnPlayStoryClick()
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapper.Get(MyTextsWrapperEnum.MessageMmoWillBeAddedLater), new StringBuilder(), MyTextsWrapperEnum.Ok, null));
        }

        public void OnPlaySandboxClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMultiplayer(m_closeAfterSuccessfulEnter));
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}

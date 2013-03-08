using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Sessions;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenChooseDifficulty : MyGuiScreenBase
    {
        MyGameplayDifficultyEnum m_difficulty = MyGameplayDifficultyEnum.EASY;
        string m_checkpointName;
        MyGuiScreenBase m_closeAfterSuccessfulEnter;

        public MyGuiScreenChooseDifficulty(MyGuiScreenBase closeAfterSuccessfulEnter, string checkpointName)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR,  new Vector2(574f/1600f, 640/1200f), false, MyGuiManager.GetSelectEditorBackground())
        
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_checkpointName = checkpointName;

            m_enableBackgroundFade = true;
            m_canHaveFocus = true;
            m_closeOnEsc = true;
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;

            AddCaption(MyTextsWrapperEnum.DifficultyCaption, new Vector2(0, 0.005f));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.145f);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;


            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.DifficultyEasy, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnEasyClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.DifficultyNormal, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnNormalClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.DifficultyHard, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnHardClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            var exitButton = new MyGuiControlButton(this, new Vector2(0, 0.1460f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(exitButton);


            SetControlIndex(2);
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenChooseDifficulty";
        }

        public void OnEasyClick(MyGuiControlButton sender)
        {
            m_difficulty = MyGameplayDifficultyEnum.EASY;
            Run();
        }

        public void OnNormalClick(MyGuiControlButton sender)
        {
            m_difficulty = MyGameplayDifficultyEnum.NORMAL;
            Run();
        }

        public void OnHardClick(MyGuiControlButton sender)
        {
            m_difficulty = MyGameplayDifficultyEnum.HARD;
            Run();
        }

        public void Run()
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
            try
            {
                StartNewGame();
            }
            catch (MyDataCorruptedException e)
            {
                MyMwcLog.WriteLine(e);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.LocalDataCorrupted, MyTextsWrapperEnum.Error, OnErrorClose));
            }
        }

        void StartNewGame()
        {
            MySession.StartNewGame(m_difficulty);
        }

        static void OnErrorClose(MyGuiScreenMessageBoxCallbackEnum result)
        {
            MyGuiManager.BackToMainMenu();
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }        
    }
}
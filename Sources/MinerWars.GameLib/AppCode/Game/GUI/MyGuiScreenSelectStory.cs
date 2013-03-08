using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Sessions;
using System.Net;
using System.Linq;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.App;
using System.Threading;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSelectStory : MyGuiScreenBase
    {
        MyGuiControlButton m_continueLastGame;
        MyGuiControlButton m_loadChapter;

        MyGuiScreenBase m_closeAfterSuccessfulEnter;
        bool loaded = false;

        bool m_hasCheckpoint = false;
        bool m_hasChapters = false;

        public MyGuiScreenSelectStory(MyGuiScreenBase closeAfterSuccessfulEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 695 / 1200f), false, MyGuiManager.GetSandboxBackgoround())
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_enableBackgroundFade = true;

            AddControls();
        }

        protected override void OnShow()
        {
            if (!loaded)
            {
                AddControls();
                loaded = true;
            }
            base.OnShow();
        }

        private void AddControls()
        {
            Controls.Clear();

            AddCaption(MyTextsWrapperEnum.PlayStory, new Vector2(0, 0.005f));

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.147f);
            Vector2 buttonDelta = new Vector2(0.15f, 0);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            MyTextsWrapperEnum? otherButtonsForbidden = null;
            MyTextsWrapperEnum? newGameForbidden = null;
            //MyTextsWrapperEnum newGameText = MyTextsWrapperEnum.StartDemo;
            int buttonPositionCounter = 0;

            if (MyClientServer.LoggedPlayer != null)
            {
                if ((MyClientServer.LoggedPlayer.GetCanAccessDemo() == false) && (MyClientServer.LoggedPlayer.GetCanSave() == false))
                {
                    //Uncomment when other buttons functionality implemented
                    newGameForbidden = MyTextsWrapperEnum.NotAvailableInDemoMode;
                    //otherButtonsForbidden = newGameForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                }
                else if (MyClientServer.LoggedPlayer.IsDemoUser())
                {
                    //Uncomment when other buttons functionality implemented
                    newGameForbidden = null;
                    otherButtonsForbidden = MyTextsWrapperEnum.NotAvailableInDemoMode;
                }
                else if (MyClientServer.LoggedPlayer.GetCanSave() == true)
                {
                    //newGameText = MyTextsWrapperEnum.NewGame;
                    newGameForbidden = null;
                }

                ParallelTasks.Parallel.Start(CheckCheckpointAndChapter);
            }

            //  New Game / Start Demo
            var newGameButton = new MyGuiControlButton(this, menuPositionOrigin + buttonPositionCounter++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.NewGame, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnNewGameClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, newGameForbidden);

            //  Continue last game
            m_continueLastGame = new MyGuiControlButton(this, menuPositionOrigin + buttonPositionCounter++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.ContinueLastGame, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnLoadLastCheckpointClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                MyFakes.LOAD_LAST_CHECKPOINT_ENABLED, otherButtonsForbidden);
            m_continueLastGame.DrawRedTextureWhenDisabled = false;

            // Show load checkpoint first (it's unknown whether checkpoint exists
            var tmp = newGameButton.GetPosition();
            newGameButton.SetPosition(m_continueLastGame.GetPosition());
            m_continueLastGame.SetPosition(tmp);

            Controls.Add(m_continueLastGame);
            Controls.Add(newGameButton);

            m_continueLastGame.Enabled = false;

            //  Load Chapter
            m_loadChapter = new MyGuiControlButton(this, menuPositionOrigin + buttonPositionCounter++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.LoadChapter, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnLoadChapterClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                //MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetCanAccessEditorForStory()
                true
                /* && !otherButtonsForbidden.HasValue*/, otherButtonsForbidden);
            m_loadChapter.DrawRedTextureWhenDisabled = false;
            Controls.Add(m_loadChapter);

            m_loadChapter.Enabled = false;

            //  Load Checkpoint
            //var loadCheckpoint = new MyGuiControlButton(this, menuPositionOrigin + buttonPositionCounter++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
            //    MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
            //    MyTextsWrapperEnum.LoadCheckpoint, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
            //    OnLoadCheckpointClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
            //    MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetCanAccessEditorForStory()/* && !otherButtonsForbidden.HasValue*/, otherButtonsForbidden);
            //loadCheckpoint.DrawRedTextureWhenDisabled = false;
            //Controls.Add(loadCheckpoint);

            //  Join friend’s game - Coop mode
            var join = new MyGuiControlButton(this, menuPositionOrigin + buttonPositionCounter++ * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.JoinFriendGame, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnJoinFriendGameClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, otherButtonsForbidden);
            join.DrawRedTextureWhenDisabled = false;
            Controls.Add(join);

            var backButton = new MyGuiControlButton(this, new Vector2(0, 0.178f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(backButton);
        }

        void CheckCheckpointAndChapter()
        {
            var startTime = MyMinerGame.TotalTimeInMilliseconds;
            bool checkpoint = MinerWars.AppCode.Networking.MyLocalCache.HasCurrentSave();
            bool chapter = MyLocalCache.HasChapters();
            var endTime = MyMinerGame.TotalTimeInMilliseconds;
            var length = endTime - startTime;
            var minLength = 250;
            if (length < minLength)
            {
                // To prevent blink
                Thread.Sleep(minLength - length);
            }
            m_hasCheckpoint = checkpoint;
            m_hasChapters = chapter;
        }

        public override bool Update(bool hasFocus)
        {
            m_continueLastGame.Enabled = m_hasCheckpoint;
            m_loadChapter.Enabled = m_hasChapters;

            return base.Update(hasFocus);
        }


        public override string GetFriendlyName()
        {
            return "MyGuiScreenSelectStory";
        }

        public void OnNewGameClick(MyGuiControlButton sender)
        {
            if (MyClientServer.LoggedPlayer.HasAnyCheckpoints)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessageAreYouSureYouWantStartNewStory,
                    MyTextsWrapperEnum.TitleStartNewStory, MyTextsWrapperEnum.Ok, MyTextsWrapperEnum.Cancel, StartNewStoryMessageBoxCallback));
            }
            else
            {
                StartNewStory();
            }
        }

        private void StartNewStoryMessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                StartNewStory();
            }
        }

        private void StartNewStory()
        {
            MyMwcLog.WriteLine("OnNewGameClick - Start");
            MyGuiManager.AddScreen(new MyGuiScreenChooseDifficulty(m_closeAfterSuccessfulEnter, null));
            //Run(MyMwcStartSessionRequestTypeEnum.NEW_STORY);
            MyMwcLog.WriteLine("OnNewGameClick - End");
        }

        public void OnLoadLastCheckpointClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("OnLoadLastCheckpointClick - Start");
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
            MySession.StartLastCheckpoint();
            MyMwcLog.WriteLine("OnLoadLastCheckpointClick - End");
        }

        public void OnLoadChapterClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenLoadChapter());
        }


        public void OnJoinFriendGameClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenJoinGame(m_closeAfterSuccessfulEnter, MyGameTypes.Story));
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}
using System;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using KeenSoftwareHouse.Library.Trace;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.GUI
{
    //this screen is invoked after succesfull login to server during quicklaunch
    //
    //

    class MyGuiScreenStartQuickLaunch : MyGuiScreenBase
    {
        MyMwcQuickLaunchType m_quickLaunchType;
        bool m_childScreenLaunched = false;
        
        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenStartQuickLaunch CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.


        public MyGuiScreenStartQuickLaunch(MyMwcQuickLaunchType quickLaunchType, MyTextsWrapperEnum progressText) :
            base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_quickLaunchType = quickLaunchType;
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            CurrentScreen = this;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenStartQuickLaunch";
        }

        public override bool Update(bool hasFocus)
        {
            if (!hasFocus)
                return base.Update(hasFocus);

            if (m_childScreenLaunched && hasFocus)
                CloseScreenNow();

            if (m_childScreenLaunched)
                return base.Update(hasFocus);

            switch (m_quickLaunchType)
            {
                case MyMwcQuickLaunchType.EDITOR_SANDBOX:
                    {
                        //normally when entering editor, you can choose which sector you would like, this is for programming purposes, choose whatever you want directly
                        //MyMwcSectorIdentifier quickSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorGroupEnum.MIDDLE_WEST, MyMwcSectorTypeEnum.SANDBOX, null, new MyMwcVector3Int(0, 0, 0));

                        MyMwcSectorIdentifier quickSectorIdentifier;

                        if (MyFakes.MWBUILDER)
                        {
                            quickSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, MyClientServer.LoggedPlayer.GetUserId(),
                                                                                MyFakes.MWBUILDER_SECTOR, null);
                        }
                        else
                        {
                            quickSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, MyClientServer.LoggedPlayer.GetUserId(),
                                                                                MyConfig.LastSandboxSector, null);
                        }

                        bool bAssert = (quickSectorIdentifier.SectorType == MyMwcSectorTypeEnum.SANDBOX &&
                                        quickSectorIdentifier.UserId == null);
                        Debug.Assert(!bAssert);

                        if (MyFakes.MWBUILDER)
                        {
                            quickSectorIdentifier.UserId = null;
                            MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(
                                                        MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX,
                                                        MyTextsWrapperEnum.StartEditorInProgressPleaseWait,
                                                         quickSectorIdentifier,
                                                         MyGameplayDifficultyEnum.EASY, null, null));
                        }
                        else
                        {
                            MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(
                                                        MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX,
                                                        MyTextsWrapperEnum.StartEditorInProgressPleaseWait,
                                                         quickSectorIdentifier,
                                                         MyGameplayDifficultyEnum.EASY, null, null));
                        }
                        m_childScreenLaunched = true;
                    }
                    break;
                case MyMwcQuickLaunchType.LAST_SANDBOX:
                    {
                        //normally when entering editor, you can choose which sector you would like, this is for programming purposes, choose whatever you want directly
                        //MyMwcSectorIdentifier quickSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorGroupEnum.MIDDLE_WEST, MyMwcSectorTypeEnum.SANDBOX, null, new MyMwcVector3Int(0, 0, 0));

                        MyMwcSectorIdentifier quickSectorIdentifier;

                        int userId = MyClientServer.LoggedPlayer.GetUserId();
                        MyMwcVector3Int sector = MyConfig.LastSandboxSector;

                        if (MyConfig.LastFriendSectorUserId.HasValue)
                        {
                            userId = MyConfig.LastFriendSectorUserId.Value;
                            sector = MyConfig.LastFriendSectorPosition;
                        }

                        quickSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, userId,
                                                                           sector, null);

                        bool bAssert = (quickSectorIdentifier.SectorType == MyMwcSectorTypeEnum.SANDBOX &&
                                        quickSectorIdentifier.UserId == null);
                        Debug.Assert(!bAssert);

                        MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(
                                                    MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS,
                                                    MyTextsWrapperEnum.StartGameInProgressPleaseWait,
                                                     quickSectorIdentifier,
                                                     MyGameplayDifficultyEnum.EASY, null, null));
                        m_childScreenLaunched = true;
                    }
                    break;
                case MyMwcQuickLaunchType.NEW_STORY:
                    {
                        MySession.StartNewGame(MyGameplayDifficultyEnum.EASY);
                        m_childScreenLaunched = true;
                    }
                    break;
                case MyMwcQuickLaunchType.LOAD_CHECKPOINT:
                    {
                        MySession.StartLastCheckpoint();
                        m_childScreenLaunched = true;
                    }
                    break;
                case MyMwcQuickLaunchType.SANDBOX_RANDOM:
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(
                                                        MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM,
                                                        MyTextsWrapperEnum.StartGameInProgressPleaseWait,
                                                        new MyMwcSectorIdentifier(
                                                            MyMwcSectorTypeEnum.SANDBOX, MyClientServer.LoggedPlayer.GetUserId(),
                                                            new MyMwcVector3Int(0, 0, 0), null),
                                                            MyGameplayDifficultyEnum.EASY,
                                                            null,
                                                        null));
                        m_childScreenLaunched = true;
                    }
                    break;
                default:
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }
            }
           
            return base.Update(hasFocus);
        }

    }
}

using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Sessions;
using System;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Networking;
using SysUtils;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenHostGame : MyGuiScreenBase
    {
        readonly MyGuiScreenBase m_closeAfterSuccessfulEnter;

        public MyGuiScreenHostGame(MyGuiScreenBase closeAfterSuccessfulEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 695 / 1200f), false, MyGuiManager.GetSandboxBackgoround())
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_enableBackgroundFade = true;
            AddCaption(MyTextsWrapperEnum.HostGame, new Vector2(0, 0.005f));

            Debug.Assert(m_size != null, "m_size != null");
            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.146f);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            MyTextsWrapperEnum? officialSectorsForbidden = null;
            MyTextsWrapperEnum? buttonsForbidden = null;
            MyTextsWrapperEnum? friendsSectorsForbidden = null;
            if (MyClientServer.LoggedPlayer != null)
            {
                if ((MyClientServer.LoggedPlayer.GetCanAccessDemo() == false) && (MyClientServer.LoggedPlayer.GetCanSave() == false))
                {
                    officialSectorsForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                    buttonsForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                    friendsSectorsForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                }
                else if (MyClientServer.LoggedPlayer.IsDemoUser())
                {
                    friendsSectorsForbidden = MyTextsWrapperEnum.NotAvailableInDemoMode;
                }
            }

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.SandboxSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnSandboxSectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, officialSectorsForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.YourSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnMySectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, buttonsForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.FriendsSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnFriendsSectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, friendsSectorsForbidden));

            var backButton = new MyGuiControlButton(this, new Vector2(0, 0.178f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(backButton);

        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenHostGame";
        }

        void ChooseSandbox(MyMwcStartSessionRequestTypeEnum sessionRequestType, bool global)
        {
            MyMwcSectorTypeEnum sectorType = MyMwcClientServer.GetSectorTypeFromSessionType(sessionRequestType);

            var selectSectorScreen = new MyGuiScreenEnterSectorMap(m_closeAfterSuccessfulEnter, sessionRequestType, MyTextsWrapperEnum.StartGameInProgressPleaseWait, MyConfig.LastSandboxSector);
            selectSectorScreen.CustomLoadAction = StartSandbox;

            bool isOfficialSandbox = sessionRequestType == MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS && global;

            if (isOfficialSandbox)
            {
                selectSectorScreen.SetSectorSourceAction(MyLocalCache.GetOfficialMultiplayerSectorIdentifiers);
                MyGuiManager.AddScreen(selectSectorScreen);
            }
            else
            {
                MyGuiScreenMainMenu.AddLoginScreen(new MyGuiScreenLoadSectorIdentifiersProgress(sectorType, global, selectSectorScreen));
            }
        }

        void StartSandbox(MyMwcSectorIdentifier sector)
        {
            MySession.StartSandbox(sector.Position, sector.UserId);
        }

        void OnSandboxSectorsClick(MyGuiControlButton sender)
        {
            ChooseSandbox(MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS, true);
        }

        void OnMySectorsClick(MyGuiControlButton sender)
        {
            ChooseSandbox(MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN, false);
        }

        void OnFriendsSectorsClick(MyGuiControlButton sender)
        {
            var enterFriendSectorMapScreen = new MyGuiScreenEnterFriendSectorMap(MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS, MyTextsWrapperEnum.StartGameInProgressPleaseWait, m_closeAfterSuccessfulEnter);
            enterFriendSectorMapScreen.CustomLoadAction = sector => StartSandbox(sector);

            string lastname = MyConfig.LastFriendName;
            MyMwcSelectSectorRequestTypeEnum startRequest = String.IsNullOrEmpty(lastname) ? MyMwcSelectSectorRequestTypeEnum.RANDOM_FRIENDS : MyMwcSelectSectorRequestTypeEnum.FIND_BY_PLAYER_NAME_FULLTEXT;

            MyGuiScreenMainMenu.AddLoginScreen(new MyGuiScreenSelectSandboxProgress(startRequest, MyTextsWrapperEnum.SelectSandboxInProgressPleaseWait,
                enterFriendSectorMapScreen, lastname, enterFriendSectorMapScreen.AddFriendSectorsResponse));
        }

        void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}
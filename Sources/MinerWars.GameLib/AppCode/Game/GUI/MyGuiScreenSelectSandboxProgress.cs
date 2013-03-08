using System;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Collections.Generic;
using MinerWars.AppCode.Networking.SectorService;

//  IMPORTANT: THIS SCREEN CAN'T BE CANCELED BY ESC OR CANCEL BUTTON BECAUSE THAT WOULD INTERFERE THE PROCESS ON SERVER

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSelectSandboxProgress : MyGuiScreenSectorServiceCallProgress
    {
        public delegate void AddResponseDelegate(List<MyMwcSectorIdentifier> sectorIdentifiers, List<MyMwcUserDetail> userDetails);

        MyMwcSelectSectorRequestTypeEnum m_selectSandboxType;
        MyGuiScreenBase m_openAfterSuccessfulEnter;
        AddResponseDelegate m_responseHandler;

        string m_findPlayerName;
        
        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenSelectSandboxProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.


        public MyGuiScreenSelectSandboxProgress(MyMwcSelectSectorRequestTypeEnum selectSandboxType, MyTextsWrapperEnum progressText, MyGuiScreenBase openAfterSuccessfulEnter, string findPlayerName, AddResponseDelegate responseHandler) : 
            base(progressText, false)
        {
            m_selectSandboxType = selectSandboxType;
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            m_openAfterSuccessfulEnter = openAfterSuccessfulEnter;
            m_findPlayerName = findPlayerName;
            CurrentScreen = this;
            m_responseHandler = responseHandler;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSelectSandboxProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            AddAction(client.BeginSelectSandbox(m_selectSandboxType, m_findPlayerName, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            List<MyMwcUserDetail> users;
            var sectorIdentifiers = client.EndSelectSandbox(out users, asyncResult);

            m_responseHandler(sectorIdentifiers, users);
            if (m_openAfterSuccessfulEnter.GetState() != MyGuiScreenState.HIDDEN && m_openAfterSuccessfulEnter.GetState() != MyGuiScreenState.HIDING)
            {
                if (m_openAfterSuccessfulEnter.GetState() != MyGuiScreenState.OPENED)
                {
                    MyGuiManager.AddScreen(m_openAfterSuccessfulEnter);
                }
            }
            else
            {
                //this screen should be unhide automaticaly
                //m_openAfterSuccessfulEnter.UnhideScreen();
            }

            CloseScreen();
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true)
            {
                CurrentScreen = null;
            }

            return ret;
        }
    }
}

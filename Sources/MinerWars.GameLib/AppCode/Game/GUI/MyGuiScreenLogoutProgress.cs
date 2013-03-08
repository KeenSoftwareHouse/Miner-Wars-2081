using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.AppCode.Networking.SectorService;
using System;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLogoutProgress : MyGuiScreenSectorServiceCallProgress
    {
        public delegate void OnLogoutProgressClosedCallback();
        OnLogoutProgressClosedCallback m_onLogoutProgressClosed;

        //  Using this static public property client-server tells us about logout response
        public static MyGuiScreenLogoutProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.

        public MyGuiScreenLogoutProgress(OnLogoutProgressClosedCallback onLogoutProgressClosed)
            : base(MyTextsWrapperEnum.LogoutInProgressPleaseWait, true)
        {
            CurrentScreen = this;
            m_onLogoutProgressClosed = onLogoutProgressClosed;
            m_screenCanHide = true;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenLogoutProgress";
        }

        public override void LoadContent()
        {
            if (MySectorServiceClient.IsInstanceValid)
            {
                base.LoadContent();
            }
            else
            {
                HandleLogout();
            }
        }

        protected override void OnError(Exception exception, MySectorServiceClient asyncState)
        {
            // It's same like when logout succeded
            HandleLogout();
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            if (MySectorServiceClient.HasCredentials && MySectorServiceClient.HasUrl)
            {
                AddAction(client.BeginLogout(null, client), OnLogoutCompleted);
            }
            else
            {
                CloseScreen();
            }
        }

        void OnLogoutCompleted(IAsyncResult result, MySectorServiceClient client)
        {
            try
            {
                client.EndLogout(result);
            }
            catch (Exception e) // We don't care about exceptions here, just log exception
            {
                MyMwcLog.WriteLine(e);
            }

            HandleLogout();
        }

        private void HandleLogout()
        {
            MyClientServer.Logout();
            MySectorServiceClient.ClearAndClose();
            m_onLogoutProgressClosed();
            CloseScreen();
        }
    }
}

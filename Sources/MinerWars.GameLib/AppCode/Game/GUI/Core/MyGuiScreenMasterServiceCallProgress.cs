using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Networking.MasterService;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.World;

namespace MinerWars.AppCode.Game.GUI.Core
{
    abstract class MyGuiScreenMasterServiceCallProgress : MyGuiScreenProgressBaseAsync<MyMasterServiceClient>
    {
        protected MyGuiScreenMasterServiceCallProgress(MyTextsWrapperEnum progressText, bool enableCancel)
            : base(progressText, enableCancel)
        {
        }

        protected abstract void ServiceProgressStart(MyMasterServiceClient client);

        public void SetPublicCredentials()
        {
            MyMasterServiceClient.SetCredentials(MyMwcNetworkingConstants.WCF_MS_PUBLIC_USERNAME, MyMwcNetworkingConstants.WCF_MS_PUBLIC_PASSWORD);
        }

        public void SetUserCredentials()
        {
            if (MyClientServer.LoggedPlayer != null)
            {
                MyMasterServiceClient.SetCredentials(MyClientServer.LoggedPlayer.UserName.ToString(), MyClientServer.LoggedPlayer.PasswordHash.ToString());
            }
        }

        // This method is sealed, never called, childs implements ServiceProgressStart
        protected sealed override void ProgressStart()
        {
            MyMasterServiceClient client = null;

            try
            {
                client = MyMasterServiceClient.GetCheckedInstance();
                ServiceProgressStart(client);
            }
            catch (Exception e)
            {
                OnError(e, client);
            }
        }

        // Default errror handling is changed
        protected override void OnError(Exception exception, MyMasterServiceClient asyncState)
        {
            HandleSectorServiceError(exception, asyncState);
        }

        protected void HandleSectorServiceError(Exception exception, MyMasterServiceClient client)
        {
            // abort connection (reconnect will restore connection after few seconds
            client.Abort();
            MyMwcLog.WriteLine(exception); // log exception

            // Show error dialog and go to main menu
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
            CloseScreen();
            MyGuiManager.BackToMainMenu();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Networking.SectorService;
using System.Diagnostics;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using System.ServiceModel;

namespace MinerWars.AppCode.Game.GUI.Core
{
    abstract class MyGuiScreenSectorServiceCallProgress : MyGuiScreenProgressBaseAsync<MySectorServiceClient>
    {
        TimeSpan m_operationTimeout;

        protected MyGuiScreenSectorServiceCallProgress(MyTextsWrapperEnum progressText, bool enableCancel, TimeSpan operationTimeout)
            : base(progressText, enableCancel)
        {
            m_operationTimeout = operationTimeout;
        }

        protected MyGuiScreenSectorServiceCallProgress(MyTextsWrapperEnum progressText, bool enableCancel)
            : base(progressText, enableCancel)
        {
            m_operationTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_SEND; // Use default value
        }

        protected abstract void ServiceProgressStart(MySectorServiceClient client);

        // This method is sealed, never called, childs implements ServiceProgressStart
        protected sealed override void ProgressStart()
        {
            MySectorServiceClient client = null;

            try
            {
                client = MySectorServiceClient.GetCheckedInstance();
                var channelContext = client.GetChannelContext();
                if(channelContext != null)
                {
                    channelContext.OperationTimeout = m_operationTimeout;
                }
                ServiceProgressStart(client);
            }
            catch (Exception e)
            {
                OnError(e, client);
            }
        }

        // Default errror handling is changed
        protected override void OnError(Exception exception, MySectorServiceClient asyncState)
        {
            HandleSectorServiceError(exception, asyncState);
        }

        protected void HandleSectorServiceError(Exception exception, MySectorServiceClient client)
        {
            // abort connection (reconnect will restore connection after few seconds
            // client can be null when cannot be initialized
            MySectorServiceClient.SafeClose();

            MyMwcLog.WriteLine(exception); // log exception

            // Show error dialog and go to main menu
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
            CloseScreen();
            MyGuiManager.BackToMainMenu();
        }

        protected override void OnClosed()
        {
            MySectorServiceClient.SafeClose();

            base.OnClosed();
        }
    }
}

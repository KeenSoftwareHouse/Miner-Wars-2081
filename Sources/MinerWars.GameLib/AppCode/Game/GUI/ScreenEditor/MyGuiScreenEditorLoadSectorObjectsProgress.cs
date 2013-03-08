using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.ServiceModel;
using MinerWars.CommonLIB.AppCode.Networking.Services;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorLoadSectorObjectsProgress : MyGuiScreenSectorServiceCallProgress
    {
        string m_playerName;
        MyMwcSectorIdentifier m_sectorIdentifier;

        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenBase CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.
        public MyGuiScreenBase m_parentScreen;

        public MyGuiScreenEditorLoadSectorObjectsProgress(MyGuiScreenBase parentScreen, MyTextsWrapperEnum loadingText, string playerName, MyMwcSectorIdentifier sectorIdentifier)
            : base(loadingText, false)
        {
            CurrentScreen = this;
            m_parentScreen = parentScreen;
            m_playerName = playerName;
            m_sectorIdentifier = sectorIdentifier;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorLoadSectorObjectsProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            //  Send save request and wait for callback
            AddAction(client.BeginLoadSectorObjects(m_playerName, m_sectorIdentifier.SectorName, m_sectorIdentifier.Position, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            try
            {
                byte[] result = client.EndLoadSectorObjects(asyncResult);
                var sectorBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(result);

                m_parentScreen.CloseScreen();
                MyGuiManager.AddScreen(new MyGuiScreenEditorCopyTool(sectorBuilder));
            }
            catch (FaultException<MyCustomFault> faultException)
            {
                if (faultException.Detail.ErrorCode == MyCustomFaultCode.UserNotFound)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PlayerNotFound, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                }
                else if (faultException.Detail.ErrorCode == MyCustomFaultCode.SectorNotFound)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SectorNotFound, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                }
                else
                {
                    throw faultException;
                }
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

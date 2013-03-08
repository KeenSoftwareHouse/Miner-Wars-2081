using System.Collections.Generic;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Networking.SectorService;
using System;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLoadSectorIdentifiersProgress : MyGuiScreenSectorServiceCallProgress
    {
        MyMwcSectorTypeEnum m_sectorType;
        Action<List<MyMwcSectorIdentifier>> m_successAction;
        bool m_loadGlobal;

        public MyGuiScreenLoadSectorIdentifiersProgress(MyMwcSectorTypeEnum sectorType, bool global, Action<List<MyMwcSectorIdentifier>> successAction)
            : base(MyTextsWrapperEnum.LoadSectorIdentifiersPleaseWait, false)
        {
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            m_successAction = successAction;
            m_sectorType = sectorType;
            m_loadGlobal = global;
        }

        public MyGuiScreenLoadSectorIdentifiersProgress(MyMwcSectorTypeEnum sectorType, bool global, MyGuiScreenEnterSectorMap openAfterSuccessfulEnter)
            :this(sectorType, global, new Action<List<MyMwcSectorIdentifier>>((sectors) => OpenScreen(openAfterSuccessfulEnter, sectors)))
        {
        }

        private static void OpenScreen(MyGuiScreenEnterSectorMap openAfterSuccessfulEnter, List<MyMwcSectorIdentifier> sectors)
        {
            if (openAfterSuccessfulEnter != null)
            {
                openAfterSuccessfulEnter.AddLoadSectorIdentifiersResponse(sectors);
                MyGuiManager.AddScreen(openAfterSuccessfulEnter);
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenLoadSectorIdentifiersProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            if (m_loadGlobal)
            {
                AddAction(client.BeginLoadSectorIdentifiers(m_sectorType, null, client));
            }
            else
            {
                AddAction(client.BeginLoadUserSectorIdentifiers(null, client));
            }
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            if (m_loadGlobal)
            {
                m_successAction(client.EndLoadSectorIdentifiers(asyncResult));
            }
            else
            {
                m_successAction(client.EndLoadUserSectorIdentifiers(asyncResult));
            }
            this.CloseScreen();
        }
    }
}

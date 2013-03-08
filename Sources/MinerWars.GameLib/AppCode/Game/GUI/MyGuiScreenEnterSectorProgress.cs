using System.Collections.Generic;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.IO;
using SysUtils.Utils;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.Utils;
using System;
using MinerWarsMath;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Networking;
using System.Diagnostics;

//  IMPORTANT: THIS SCREEN CAN'T BE CANCELED BY ESC OR CANCEL BUTTON BECAUSE THAT WOULD INTERFERE THE PROCESS ON SERVER

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenEnterSectorProgress : MyGuiScreenSectorServiceCallProgress
    {
        System.Action<MyMwcObjectBuilder_Sector, Vector3> m_enterSectorSuccessfulAction;

        MyMwcTravelTypeEnum m_travelType;
        MyMwcVector3Int m_targetSectorPosition;
        Vector3 m_currentShipPosition;

        MyMwcObjectBuilder_Sector sector;

        public MyGuiScreenEnterSectorProgress(MyMwcTravelTypeEnum travelType, MyMwcVector3Int targetSectorPosition, Vector3 currentShipPosition, System.Action<MyMwcObjectBuilder_Sector, Vector3> enterSectorSuccessfulAction)
            : base(MyTextsWrapperEnum.EnterSectorInProgressPleaseWait, false)
        {
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            m_travelType = travelType;
            m_targetSectorPosition = targetSectorPosition;
            m_currentShipPosition = currentShipPosition;
            m_enterSectorSuccessfulAction = enterSectorSuccessfulAction;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEnterSectorProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            int version = 0;
            AddAction(client.BeginTravelToSector(m_travelType, m_targetSectorPosition, m_currentShipPosition, version, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            Vector3 newShipPosition;
            byte[] result = client.EndTravelToSector(out newShipPosition, asyncResult);

            var sectorBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(result);
            m_enterSectorSuccessfulAction(sectorBuilder, newShipPosition);

            CloseScreen();
        }
    }
}

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
using System.ServiceModel;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using KeenSoftwareHouse.Library.Trace;
using MinerWarsMath;

//  IMPORTANT: THIS SCREEN CAN'T BE CANCELED BY ESC OR CANCEL BUTTON BECAUSE THAT WOULD INTERFERE THE PROCESS ON SERVER

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLoadCheckpointProgress : MyGuiScreenSectorServiceCallProgress
    {
        System.Action<MyMwcObjectBuilder_Checkpoint> m_loadCheckpointSuccessfulAction;

        MyMwcSectorTypeEnum m_sessionType;
        MyMwcSessionStateEnum m_sessionStartType;
        int? m_userId;
        MyMwcVector3Int? m_sectorPosition;
        string m_checkpointName;

        MyMwcObjectBuilder_Checkpoint m_checkpoint;

        public MyGuiScreenLoadCheckpointProgress(MyMwcSectorTypeEnum sessionType, MyMwcSessionStateEnum sessionStartType, int? userId, MyMwcVector3Int? position, string checkpointName, System.Action<MyMwcObjectBuilder_Checkpoint> enterSectorSuccessfulAction)
            : base(MyTextsWrapperEnum.EnterSectorInProgressPleaseWait, false, TimeSpan.FromSeconds(360)) // Some missions loads very long (Roch's junkyard)
        {
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            m_sessionType = sessionType;
            m_sessionStartType = sessionStartType;
            m_userId = userId;
            m_sectorPosition = position;
            m_checkpointName = checkpointName;
            m_loadCheckpointSuccessfulAction = enterSectorSuccessfulAction;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEnterSectorProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            AddAction(client.BeginLoadCheckpoint(m_sessionType, m_sessionStartType, m_userId, m_sectorPosition, m_checkpointName, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            try
            {
                byte[] result = client.EndLoadCheckpoint(asyncResult);
                m_checkpoint = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Checkpoint>(result);
                //if (m_checkpoint.SectorObjectBuilder == null) // Server told us to use cache
                //{
                //    LoadSector();
                //    return;
                //}

                MyGuiScreenGamePlay.StoreLastLoadedCheckpoint(m_checkpoint);

                m_loadCheckpointSuccessfulAction(m_checkpoint);
                CloseScreen();
            }
            catch (FaultException<MyCustomFault> faultException)
            {
                if (faultException.Detail.ErrorCode == MyCustomFaultCode.CheckpointTemplateNotExists)
                {
                    HandleNonexistentCheckpoint();
                }
                else
                {
                    // Handled by base class
                    throw faultException;
                }
            }
        }

        //private void LoadSector()
        //{
        //    // Handles also cache
        //    var travelScreen = new MyGuiScreenEnterSectorProgress(MyMwcTravelTypeEnum.SOLAR, m_checkpoint.CurrentSector.Position, new MinerWarsMath.Vector3(), OnTravelSuccess, m_checkpoint.CurrentSector);
        //    MyGuiManager.AddScreen(travelScreen);
        //}

        private void OnTravelSuccess(MyMwcObjectBuilder_Sector sector, Vector3 newPosition)
        {
            m_checkpoint.SectorObjectBuilder = sector;
            m_loadCheckpointSuccessfulAction(m_checkpoint);
            CloseScreen();
        }

        private void HandleNonexistentCheckpoint()
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.TemplateCheckpointDeleted, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
            CloseScreen();
            MyGuiManager.BackToMainMenu();
        }
    }
}

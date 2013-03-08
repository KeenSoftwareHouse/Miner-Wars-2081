using System;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Gameplay;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;

//  IMPORTANT: THIS SCREEN CAN'T BE CANCELED BY ESC OR CANCEL BUTTON BECAUSE THAT WOULD INTERFERE THE PROCESS ON SERVER

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenStartSessionProgress : MyGuiScreenProgressBase
    {
        MyMwcSectorIdentifier? m_sectorIdentifier;
        MyMwcStartSessionRequestTypeEnum m_sessionType;
        MyGuiScreenBase m_closeAfter;
        MyGameplayDifficultyEnum m_difficulty;
        string m_checkpointName;

        public Action<MyGuiScreenGamePlayType, MyMwcStartSessionRequestTypeEnum, MyMwcObjectBuilder_Checkpoint> OnSuccessEnter { get; set; }
        
        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenStartSessionProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.


        public MyGuiScreenStartSessionProgress(MyMwcStartSessionRequestTypeEnum sessionType,  MyTextsWrapperEnum progressText, MyMwcSectorIdentifier? sectorIdentifier, MyGameplayDifficultyEnum difficulty, string checkpointName, MyGuiScreenBase closeAfter) : 
            base(progressText, false)
        {
            // TODO: Not ready yet
            //Debug.Assert(sessionType != MyMwcStartSessionRequestTypeEnum.NEW_STORY && sessionType != MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT, "Invalid operation, call OndrejP");

            m_sectorIdentifier = sectorIdentifier;
            m_sessionType = sessionType;
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN;
            m_closeAfter = closeAfter;
            m_difficulty = difficulty;
            m_checkpointName = checkpointName;
            CurrentScreen = this;

            OnSuccessEnter = new Action<MyGuiScreenGamePlayType, MyMwcStartSessionRequestTypeEnum, MyMwcObjectBuilder_Checkpoint>((screenType, sessType, checkpoint) =>
            {
                var newGameplayScreen = new MyGuiScreenGamePlay(screenType, null, checkpoint.CurrentSector, checkpoint.SectorObjectBuilder.Version, sessType);
                var loadScreen = new MyGuiScreenLoading(newGameplayScreen, MyGuiScreenGamePlay.Static);

                if (sessType == MyMwcStartSessionRequestTypeEnum.NEW_STORY)
                    loadScreen.AddEnterSectorResponse(checkpoint, MyMissionID.EAC_SURVEY_SITE);
                else
                    loadScreen.AddEnterSectorResponse(checkpoint, null);

                MyGuiManager.AddScreen(loadScreen);
            });
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenStartSessionProgress";
        }

        protected override void ProgressStart()
        {
            //MyClientServer.SendMessageStartSessionRequest(m_sessionType, m_sectorIdentifier, m_difficulty);

            MyGameplayConstants.SetGameplayDifficulty(m_difficulty);
        }

        public void AddResponse(MyMwcStartSessionResponseTypeEnum startSessionResponse, MyMwcSectorIdentifier? sectorIdentifier, MyGameplayDifficultyEnum difficulty)
        {
            MyGuiScreenGamePlayType screenType;

            if (startSessionResponse == MyMwcStartSessionResponseTypeEnum.OK)
            {
                MyMwcSectorTypeEnum sessionType;
                MyMwcSessionStateEnum sessionStartType;
                int? userId = null;
                MyMwcVector3Int? position = null;

                if ((m_sessionType == MyMwcStartSessionRequestTypeEnum.NEW_STORY) || (m_sessionType == MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT))
                {
                    MySession.Static = new MySinglePlayerSession(difficulty);
                    MySession.Static.Init();

                    screenType = MyGuiScreenGamePlayType.GAME_STORY;

                    sessionType = MyMwcSectorTypeEnum.STORY;
                    sessionStartType = m_sessionType == MyMwcStartSessionRequestTypeEnum.NEW_STORY ? MyMwcSessionStateEnum.NEW_GAME : MyMwcSessionStateEnum.LOAD_CHECKPOINT;
                    userId = MyClientServer.LoggedPlayer.GetUserId();
                }
                else if (m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX)
                {
                    MySession.Static = new MySandboxSession();
                    MySession.Static.Init();

                    screenType = MyGuiScreenGamePlayType.EDITOR_SANDBOX;

                    Debug.Assert(m_sectorIdentifier.HasValue, "Sector identifier must be set for editor");
                    Debug.Assert(m_sectorIdentifier.Value.UserId == null || m_sectorIdentifier.Value.UserId == MyClientServer.LoggedPlayer.GetUserId(), "Sandbox sector user identifier must be null or same as current user");
                    sessionType = MyMwcSectorTypeEnum.SANDBOX;
                    sessionStartType = MyMwcSessionStateEnum.EDITOR;
                    userId = m_sectorIdentifier.Value.UserId;
                    position = m_sectorIdentifier.Value.Position;
                }
                else if (m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_STORY)
                {
                    MySession.Static = new MySinglePlayerSession(difficulty);
                    MySession.Static.Init();

                    screenType = MyGuiScreenGamePlayType.EDITOR_STORY;

                    Debug.Assert(m_sectorIdentifier.HasValue, "Sector identifier must be set for editor");
                    sessionType = MyMwcSectorTypeEnum.STORY;
                    sessionStartType = MyMwcSessionStateEnum.EDITOR;
                    userId = null;
                    position = m_sectorIdentifier.Value.Position;
                }
                else if (m_sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_MMO)
                {
                    MySession.Static = new MySinglePlayerSession(difficulty);
                    MySession.Static.Init();

                    screenType = MyGuiScreenGamePlayType.EDITOR_MMO;

                    Debug.Assert(m_sectorIdentifier.HasValue, "Sector identifier must be set for editor");
                    sessionType = MyMwcSectorTypeEnum.MMO;
                    sessionStartType = MyMwcSessionStateEnum.EDITOR;
                    userId = null;
                    position = m_sectorIdentifier.Value.Position;
                }
                else if (
                    (m_sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN) || 
                    (m_sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS) ||
                    (m_sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM))
                {
                    MySession.Static = new MySandboxSession();
                    MySession.Static.Init();

                    screenType = MyGuiScreenGamePlayType.GAME_SANDBOX;

                    Debug.Assert(m_sectorIdentifier.HasValue, "Sector identifier must be set for sandbox");
                    sessionType = MyMwcSectorTypeEnum.SANDBOX;
                    sessionStartType = MyMwcSessionStateEnum.LOAD_CHECKPOINT;
                    userId = m_sectorIdentifier.Value.UserId;
                    position = m_sectorIdentifier.Value.Position;
                    //if (m_sessionType != MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM)
                    //{
                    //    position = m_sectorIdentifier.Value.Position;
                    //}
                }
                else
                {
                    throw new NotImplementedException();
                }
                
                Action<MyMwcObjectBuilder_Checkpoint> enterSuccessAction = new Action<MyMwcObjectBuilder_Checkpoint>((checkpoint) =>
                {
                    OnSuccessEnter(screenType, m_sessionType, checkpoint);
                });

                MyGuiManager.AddScreen(new MyGuiScreenLoadCheckpointProgress(sessionType, sessionStartType, userId, position, m_checkpointName, enterSuccessAction));

                if (m_closeAfter != null) m_closeAfter.CloseScreenNow();
            }
            else
            {
                MyMwcLog.WriteLine("Error starting the session: " + startSessionResponse.ToString());
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.StartGameFailed,
                    MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
            }

            CloseScreen();
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //  Only continue if this screen is really open (not closing or closed)
            if (GetState() != MyGuiScreenState.OPENED) return false;

            AddResponse(MyMwcStartSessionResponseTypeEnum.OK, m_sectorIdentifier, m_difficulty);

            return true;
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

        public override int GetTransitionOpeningTime()
        {
            return 0;
        }

        public override int GetTransitionClosingTime()
        {
            return 0;
        }
    }
}

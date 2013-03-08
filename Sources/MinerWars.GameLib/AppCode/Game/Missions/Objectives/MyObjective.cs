#region Using

using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Audio.Dialogues;
using SysUtils.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Sessions;

#endregion

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjective : MyMissionBase
    {
        public bool CanBeSkipped { get; set; }
        public bool FailIfEntityDestroyed { get; set; }
        public bool MakeEntityIndestructible { get; set; }
        public bool ShowNotificationOnSuccess = true;
        public bool MarkMissionEntities = false;

        public event MissionHandler OnSuccessDialogueStarted;

        [Obsolete]
        public MyObjective( StringBuilder Name, 
                            MyMissionID ID, 
                            StringBuilder Description, 
                            MyTexture2D Icon, 
                            MyMission ParentMission, 
                            MyMissionID[] RequiredMissions, 
                            MyMissionLocation Location, 
                            List<uint> MissionEntityIDs = null, 
                            MyDialogueEnum? successDialogId = null, 
                            MyDialogueEnum? startDialogId = null,
                            MyActorEnum[] requiredActors = null,
                            float? radiusOverride = null)
        {
            this.NameTemp = Name;
            this.ID = ID;
            this.DescriptionTemp = Description;
            this.Icon = Icon;
            this.ParentMission = ParentMission;
            this.RequiredMissions = RequiredMissions;
            this.Location = Location;
            this.MissionEntityIDs = new List<uint>();
            this.SuccessDialogId = successDialogId;
            this.StartDialogId = startDialogId;
            this.m_radiusOverride = radiusOverride;
            if (MissionEntityIDs != null)
            {
                this.MissionEntityIDs.AddRange(MissionEntityIDs);
            }
            if (requiredActors != null)
            {
                this.RequiredActors = requiredActors;
            }
            RequiresDrone = false;
            CanBeSkipped = true;
            FailIfEntityDestroyed = true;
            MakeEntityIndestructible = true;
        }

        public MyObjective( MyTextsWrapperEnum name,
                            MyMissionID missionId,
                            MyTextsWrapperEnum description,
                            MyTexture2D icon,
                            MyMission parentMission,
                            MyMissionID[] requiredMissions,
                            MyMissionLocation location,
                            List<uint> missionEntityIDs = null,
                            MyDialogueEnum? successDialogId = null,
                            MyDialogueEnum? startDialogId = null,
                            MyActorEnum[] requiredActors = null,
                            float? radiusOverride = null)
            : this(MyTextsWrapper.Get(name), missionId, MyTextsWrapper.Get(description), icon, parentMission, requiredMissions, location, missionEntityIDs, successDialogId,
            startDialogId, requiredActors, radiusOverride)

        {
        }

        public MyMission ParentMission { get; private set; }

        public static bool SkipSubmission = false;
        public bool SaveOnSuccess = false;
        public StringBuilder AdditionalHudInformation { get; protected set; }
        bool m_showAsOptional = false;
        int m_dialogDelayToSkipForTesting = 1000; //ms

        protected bool SkipDialogEnabled;
        public bool ShowAsOptional
        {
            get
            {
                return m_showAsOptional;
            }
            set
            {
                m_showAsOptional = value;
                GuiTargetMode = value ? MyGuitargetMode.ObjectiveOptional : MyGuitargetMode.Objective;
            }
        }


        private bool m_succesDialogStarted = false;
        private bool m_successDialogFinished = false;
        private int m_objectiveStartTime = 0;
        bool m_destinationReached = false;
        private MyHudNotification.MyNotification m_learnToUseDrone  ;

        public bool RequiresDrone { get; set; }

        public override void Load()
        {
            m_succesDialogStarted = false;
            m_successDialogFinished = false;
            if(RequiresDrone)
            {
                MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged += Static_CameraContrlolledObjectChanged;
                if(MyGuiScreenGamePlay.Static.CameraAttachedTo != MyCameraAttachedToEnum.Drone)
                {
                    AddDroneNotification();
                }
            }
            m_objectiveStartTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            MySession.Static.EventLog.AddSubmissionAvailable(ID);

            MyMwcLog.WriteLine("Submission " + NameTemp + " loaded.");
            base.Load();

            // Clear player notifications (so that ie. Travel notification can change color immediately)
            MySession.PlayerShip.ClearNotifications();

            if (SuccessDialogId != null)
            {
                MyDialogues.OnDialogueFinished += OnDialogueFinished;
            }

            MyHud.ShowGPSPathToNextObjective(false);


            if (MarkMissionEntities)
            {
                foreach (var item in MissionEntityIDs)
                {
                    SetLocationVisibility(true, MyScriptWrapper.GetEntity(item), MyGuitargetMode.Objective);
                }
            }
        }

        private void Static_CameraContrlolledObjectChanged(MyEntity e)
        {
            if ((e is MyDrone)) RemoveDroneNotification();
            else AddDroneNotification();
        }

        private void AddDroneNotification()
        {
            m_learnToUseDrone = MyScriptWrapper.CreateNotification(
                MyTextsWrapperEnum.HowToControlDrone,
                MyHudConstants.MISSION_FONT, 0,
                new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.DRONE_DEPLOY), MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.DRONE_CONTROL) }
                );
            MyScriptWrapper.AddNotification(m_learnToUseDrone);
        }
        private void RemoveDroneNotification()
        {
            if(m_learnToUseDrone!=null)m_learnToUseDrone.Disappear();
        }

        void OnDialogueFinished(Audio.Dialogues.MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == SuccessDialogId)
            {
                m_successDialogFinished = true;
                Success();
            }
        }

        public override void Unload()
        {
            MyMwcLog.WriteLine("Submission " + NameTemp + " unload.");
            base.Unload();
            m_objectiveStartTime = 0;

            MyDialogues.OnDialogueFinished -= OnDialogueFinished;
            if(m_learnToUseDrone!=null) m_learnToUseDrone.Disappear();
            MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged -= Static_CameraContrlolledObjectChanged;

            if (MarkMissionEntities)
            {
                foreach (var item in MissionEntityIDs)
                {
                    SetLocationVisibility(false, MyScriptWrapper.GetEntity(item), MyGuitargetMode.Objective);
                }
            }
        }

        public int GetObjectiveStartTime()
        {
            return m_objectiveStartTime;
        }

        public override void Update()
        {
            base.Update();

            if (!m_destinationReached && IsPlayerNearLocation())
            {
                DestinationReached();
            }

            if ((IsSuccess() || (SkipSubmission && (CanBeSkipped || MyFakes.TEST_MISSION_GAMEPLAY)))
                // complete submission only if player is in game play screen (In some missions there's possibility that mission will complete and player is in MainMenu,
                // progress dialog which follows will break gameplay)
                && MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static == MyGuiManager.GetScreenWithFocus())
            {
                if (SkipSubmission) MyDialogues.Stop();
                SkipSubmission = false;
                Success();
                
            }
        }

        public virtual void DestinationReached()
        {
            m_destinationReached = true;
            MyAudio.AddCue2D(MySoundCuesEnum.HudDestinationReached);
        }

        public override void Success()
        {
            if (this.ParentMission.MarkedForUnload)
                return;

            if (MyMultiplayerGameplay.IsHosting && !MySession.Static.EventLog.IsMissionFinished(ID))
            {
                MyMultiplayerGameplay.Static.SendMissionProgress(this, CommonLIB.AppCode.Networking.Multiplayer.MyMissionProgressType.Success);
            }
            
            MySession.Static.EventLog.SubmissionFinished(ID);
            
            SetLocationVisibility(false);
            if (Location != null)
            {
                Location.Entity = null;
            }

            bool canPlaySuccessDialogue = (!MyFakes.ENABLE_AUTOSKIPPING_ENDMISSION_DIALOGUE || SkipDialogEnabled);

            if (MyFakes.TEST_MISSION_GAMEPLAY && MyFakes.TEST_MISSION_GAMEPLAY_AUTO_KILLS == 0)
            {
                m_dialogDelayToSkipForTesting -= MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (m_dialogDelayToSkipForTesting <= 0)
                {
                    //Stop success dialogue after one second. This delay should be enough to spawn bots etc.
                    canPlaySuccessDialogue = false;
                }
            }
            else if (MyFakes.TEST_MISSION_GAMEPLAY)
                canPlaySuccessDialogue = false;
                

            if (canPlaySuccessDialogue)
            {
                if (m_succesDialogStarted && !m_successDialogFinished) // We are waiting to finish dialog
                    return;
                if (!m_successDialogFinished && SuccessDialogId != null) // We succeded but still did not played succes dialog
                {
                    m_succesDialogStarted = true;
                    
                    if (OnSuccessDialogueStarted != null)
                    {
                        OnSuccessDialogueStarted(this);
                    }

                    MyScriptWrapper.PlayDialogue(SuccessDialogId.Value);
                    return;
                }
            }
            
            MyMwcLog.WriteLine("Submission " + NameTemp + " success.");


            base.Success();
                        
            MyAudio.AddCue2D(MySoundCuesEnum.HudObjectiveComplete);
            ParentMission.ObjectiveCompleted(this);

            MyMissions.SaveName = this.ParentMission.NameTemp.ToString() + ": " + (NameTemp != null ? NameTemp.ToString() : "(null)") + (SaveOnSuccess ? "" : " *");
            MyMissions.NeedsSave |= SaveOnSuccess;
        }

        public override void Fail(MyTextsWrapperEnum? customMessage = null)
        {
            base.Fail(customMessage);

            MyMwcLog.WriteLine("Submission " + NameTemp + " fail. " + (customMessage.HasValue ? " (" + MyTextsWrapper.Get(customMessage.Value).ToString() + ")" : ""));

            ParentMission.Fail(customMessage);
        }

        public override bool IsAvailable()
        {
            return (MyMissions.ActiveMission == ParentMission) && base.IsAvailable();
        }

        public virtual bool IsSuccess()
        {
            return IsPlayerNearLocation();
        }

        public override bool IsMissionEntityNotification(MyEntity entity, MySmallShipInteractionActionEnum action)
        {
            //return base.IsMissionEntityNotification(entity, action);
            return base.IsMissionEntity(entity);
         
        }

        public override void SetLocationVisibility(bool visible)
        {
            base.SetLocationVisibility(visible);
        }

        public bool IsPlayerNearLocation()
        {
            if (HasLocationEntity())
            {

                if (MyGuiScreenGamePlay.Static.ControlledDrone != null)
                {
                    var boundingSphere = MyGuiScreenGamePlay.Static.ControlledDrone.WorldVolume;
                    if (m_radiusOverride.HasValue)
                    {
                        boundingSphere.Radius = m_radiusOverride.Value;
                    }
                    bool isNear = Location.Entity.GetIntersectionWithSphere(ref boundingSphere);
                    if (isNear)
                        return true;
                }
                {
                    var boundingSphere = MySession.PlayerShip.WorldVolume;
                    if (m_radiusOverride.HasValue)
                    {
                        boundingSphere.Radius = m_radiusOverride.Value;
                    }
                    return Location.Entity.GetIntersectionWithSphere(ref boundingSphere);
                }
            }
            return false;
        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.Missions
{
    public enum MyUseObjectiveType
    {
        Hacking,
        Building,
        Repairing,
        Activating,
        Taking,
        Putting,
    }

    class MyUseObjective : MyObjective
    {
        private MyTextsWrapperEnum m_notificationText;
        private MyHudNotification.MyNotification m_notification;
        private bool m_isNearLocation;

        private int m_requiredTime;
        private int m_elapsedTime;

        private bool m_inUse;
        private uint? m_realMissionEntityId;
        private MyEntity m_realMissionEntity;

        private MyGuiScreenUseProgressBar m_useProgress;
        private MyTextsWrapperEnum m_useCaption;
        private MyTextsWrapperEnum m_useText;
        private EventHandler m_startingGeneratorProgressScreen_OnCanceledHandler;
        private EventHandler m_startingGeneratorProgressScreen_OnSuccessHandler;

        private MySoundCuesEnum m_progressCue, m_cancelCue;

        public MyUseObjective(
            MyTextsWrapperEnum name,
            MyMissionID id,
            MyTextsWrapperEnum description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyMissionLocation location,
            MyTextsWrapperEnum notificationText,
            MyTextsWrapperEnum useCaption,
            MyTextsWrapperEnum useText,
            int requiredTime,
            MyUseObjectiveType objectiveType = MyUseObjectiveType.Hacking,
            MyDialogueEnum? successDialogId = null,
            MyDialogueEnum? startDialogId = null,
            float? radiusOverride = null,
            List<uint> fakeMissionIds = null,
            uint? realMissionId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, location, fakeMissionIds, successDialogId, startDialogId, radiusOverride: radiusOverride)
        {
            m_notificationText = notificationText;
            m_useText = useText;
            m_useCaption = useCaption;
            m_requiredTime = requiredTime;
            m_realMissionEntityId = realMissionId;
            InitSounds(objectiveType);

            if (location != null)
            {
                if (location.LocationEntityIdentifier.LocationEntityId != null)
                {
                    RequiredEntityIDs.Add(location.LocationEntityIdentifier.LocationEntityId.Value);
                }
            }
        }

        [Obsolete]
        public MyUseObjective(
            StringBuilder name,
            MyMissionID id,
            StringBuilder description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyMissionLocation location,
            MyTextsWrapperEnum notificationText,
            MyTextsWrapperEnum useCaption,
            MyTextsWrapperEnum useText,
            int requiredTime,
            MyUseObjectiveType objectiveType = MyUseObjectiveType.Hacking,
            MyDialogueEnum? successDialogId = null, 
            MyDialogueEnum? startDialogId = null,
            float? radiusOverride = null,
            List<uint> fakeMissionIds = null,
            uint? realMissionId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, location, fakeMissionIds, successDialogId, startDialogId, radiusOverride: radiusOverride)
        {
            m_notificationText = notificationText;
            m_useText = useText;
            m_useCaption = useCaption;
            m_requiredTime = requiredTime;
             m_realMissionEntityId = realMissionId;
            InitSounds(objectiveType);

            if (location != null)
            {
                if (location.LocationEntityIdentifier.LocationEntityId != null)
                {
                    RequiredEntityIDs.Add(location.LocationEntityIdentifier.LocationEntityId.Value);
                }
            }
        }


        private void InitSounds(MyUseObjectiveType objectiveType)
        {
            switch (objectiveType)
            {
                case MyUseObjectiveType.Activating:
                    m_progressCue = MySoundCuesEnum.SfxProgressActivation;
                    m_cancelCue = MySoundCuesEnum.SfxCancelActivation;
                    break;
                case MyUseObjectiveType.Building: 
                    m_progressCue = MySoundCuesEnum.SfxProgressBuild;
                    m_cancelCue = MySoundCuesEnum.SfxCancelBuild;
                    break;
                case MyUseObjectiveType.Hacking:
                    m_progressCue = MySoundCuesEnum.SfxProgressHack;
                    m_cancelCue = MySoundCuesEnum.SfxCancelHack;
                    break;
                case MyUseObjectiveType.Repairing:
                    m_progressCue = MySoundCuesEnum.SfxProgressRepair;
                    m_cancelCue = MySoundCuesEnum.SfxCancelRepair;
                    break;
                case MyUseObjectiveType.Taking:
                    m_progressCue = MySoundCuesEnum.SfxProgressTake;
                    m_cancelCue = MySoundCuesEnum.SfxCancelTake;
                    break;
                case MyUseObjectiveType.Putting:
                    m_progressCue = MySoundCuesEnum.SfxProgressPut;
                    m_cancelCue = MySoundCuesEnum.SfxCancelPut;
                    break;
            }
        }


        public override void Unload()
        {
            base.Unload();
            m_useProgress.OnCanceled -= m_startingGeneratorProgressScreen_OnCanceledHandler;
            m_useProgress.OnSuccess -= m_startingGeneratorProgressScreen_OnSuccessHandler;
        }

        public override void Load()
        {
            base.Load();
            if (m_realMissionEntityId.HasValue)
                m_realMissionEntity = MyScriptWrapper.TryGetEntity(m_realMissionEntityId.Value);

            m_notification = new MyHudNotification.MyNotification(m_notificationText, MyHudConstants.MISSION_FONT, MyHudNotification.DONT_DISAPEAR, null, new object[] { "" });
            m_startingGeneratorProgressScreen_OnCanceledHandler = new EventHandler(OnCanceledHandler);
            m_startingGeneratorProgressScreen_OnSuccessHandler = new EventHandler(OnSuccessHandler);

            m_useProgress = new MyGuiScreenUseProgressBar(m_useCaption, m_useText, 0f, m_progressCue, m_cancelCue, MyGameControlEnums.USE, 0, m_requiredTime, 0);
            m_useProgress.OnCanceled += m_startingGeneratorProgressScreen_OnCanceledHandler;
            m_useProgress.OnSuccess += m_startingGeneratorProgressScreen_OnSuccessHandler;
        }

        public override bool IsSuccess()
        {
            m_isNearLocation = base.IsSuccess() || IsNearRealLocation();
            if (m_isNearLocation)
            {
                m_notification.SetTextFormatArguments(new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE) });
                m_notification.Appear();
                MyHudNotification.AddNotification(m_notification);
            }
            else
            {
                m_notification.Disappear();
            }

            return false;
        }


        public override void Success()
        {
            m_notification.Disappear();
            base.Success();
        }

        private bool IsNearRealLocation()
        {
            if (m_realMissionEntity == null) return false;

            var boundingSphere = MySession.PlayerShip.WorldVolume;

            if (MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.Drone)
            {
                boundingSphere = MyGuiScreenGamePlay.Static.ControlledDrone.WorldVolume;
            }

            if (m_radiusOverride.HasValue)
            {
                boundingSphere.Radius = m_radiusOverride.Value;
            }

            return  m_realMissionEntity.GetIntersectionWithSphere(ref boundingSphere);
            
        }

        public override void Update()
        {
            base.Update();

            if (MyGuiManager.GetInput().GetGameControl(MyGameControlEnums.USE).IsPressed() && (base.IsSuccess() || IsNearRealLocation()))
            {
                if (!m_inUse)
                {
                    StartUse();
                }
                //m_elapsedTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                //m_useProgress.UpdateValue((float)(m_elapsedTime) / (float)m_requiredTime);

                //if (m_elapsedTime >= m_requiredTime)
                //{
                //    FinishUse();
                //}
            }
        }



        private void OnCanceledHandler(object sender, EventArgs e)
        {
            FailUse();
        }

        private void OnSuccessHandler(object sender, EventArgs e)
        {
            FinishUse();
        }

        private void StartUse()
        {
            m_inUse = true;
            m_useProgress.Reset();
            MyGuiManager.AddScreen(m_useProgress);
        }

        private void FinishUse()
        {
            m_inUse = false;
            Success();
        }

        public void FailUse()
        {
            m_inUse = false;
            m_elapsedTime = 0;
            //m_useProgress.CloseScreenNow();
        }
    }
}

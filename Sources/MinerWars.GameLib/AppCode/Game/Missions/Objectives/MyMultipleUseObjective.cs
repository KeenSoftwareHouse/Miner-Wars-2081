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
    delegate void ObjectUsedSucces(uint entityID);



    class MyMultipleUseObjective : MyMultipleObjectives
    {
        private MyTextsWrapperEnum m_notificationText;
        private MyHudNotification.MyNotification m_notification;
        private bool m_isNearLocation;

        private int m_requiredTime;
        private int m_elapsedTime;

        private bool m_inUse;


        private MyGuiScreenUseProgressBar m_useProgress;
        private MyTextsWrapperEnum m_useCaption;
        private MyTextsWrapperEnum m_useText;
        private EventHandler m_startingGeneratorProgressScreen_OnCanceledHandler;
        private EventHandler m_startingGeneratorProgressScreen_OnSuccessHandler;

        private MySoundCuesEnum m_progressCue, m_cancelCue;
        private int m_totalCount;
        private uint m_usingEntityId ;
        private MyUseObjectiveType m_objectiveType;
        public event ObjectUsedSucces OnObjectUsedSucces;

        public float? RadiusOverride;

        private List<uint> m_entitiesToUse = new List<uint>();

        public MyMultipleUseObjective(
            MyTextsWrapperEnum name,
            MyMissionID id,
            MyTextsWrapperEnum description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyTextsWrapperEnum notificationText,
            MyTextsWrapperEnum useCaption,
            MyTextsWrapperEnum useText,
            int requiredTime,
            List<uint> MissionEntityIDs,
            MyUseObjectiveType objectiveType = MyUseObjectiveType.Hacking,


            MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, MissionEntityIDs, startDialogId: startDialogId)
        {
            m_notificationText = notificationText;
            m_useText = useText;
            m_useCaption = useCaption;
            m_requiredTime = requiredTime;

            m_objectiveType = objectiveType;
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
            m_entitiesToUse = new List<uint>();
            m_entitiesToUse.AddRange(MissionEntityIDs);
            InitSounds(m_objectiveType);
            m_totalCount = MissionEntityIDs.Count;
            m_notification = new MyHudNotification.MyNotification(m_notificationText, MyHudConstants.MISSION_FONT, MyHudNotification.DONT_DISAPEAR,null, new object[] { "" });
            m_startingGeneratorProgressScreen_OnCanceledHandler = new EventHandler(OnCanceledHandler);
            m_startingGeneratorProgressScreen_OnSuccessHandler = new EventHandler(OnSuccessHandler);

            m_useProgress = new MyGuiScreenUseProgressBar(m_useCaption, m_useText, 0f, m_progressCue, m_cancelCue, MyGameControlEnums.USE, 0, m_requiredTime, 0);
            m_useProgress.OnCanceled += m_startingGeneratorProgressScreen_OnCanceledHandler;
            m_useProgress.OnSuccess += m_startingGeneratorProgressScreen_OnSuccessHandler;


            foreach(var id in MissionEntityIDs)
            {
               SetLocationVisibility(true,MyScriptWrapper.GetEntity(id),MyGuitargetMode.Objective);
            }

            ReloadAdditionalHubInfo();
        }


       

        public override bool IsSuccess()
        {
            uint eId = 0;
            m_isNearLocation = IsNearSomeLocation(ref eId);

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



        private bool IsNearSomeLocation(ref uint entityId)
        {
            foreach (var entityID in m_entitiesToUse)
            {
                MyEntity entity;
                MyEntities.TryGetEntityById(new MyEntityIdentifier(entityID), out entity);
                if (entity!=null)
                {
                    var boundingSphere = MySession.PlayerShip.WorldVolume;
                    if (RadiusOverride.HasValue)
                    {
                        boundingSphere.Radius = RadiusOverride.Value;
                    }
                    
                    if(entity.GetIntersectionWithSphere(ref boundingSphere))
                    {
                        if (entity.EntityId != null) entityId = entity.EntityId.Value.NumericValue;
                        return true;
                    }
                }
            }

            return false;
        }

        /*
        public override void Success()
        {
            m_notification.Disappear();
            base.Success();
        }
        */

        private void UseSuccess(uint entityID)
        {
            if (entityID != 0 && m_entitiesToUse.Remove(entityID))
            {
                SetLocationVisibility(false, MyScriptWrapper.GetEntity(entityID), MyGuitargetMode.Objective);
                m_notification.Disappear();
                
                SetLocationVisibility(false, MyScriptWrapper.GetEntity(entityID), MyGuitargetMode.Objective);
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(entityID));
                if (OnObjectUsedSucces != null)
                {
                    OnObjectUsedSucces(entityID);
                }
                if (m_entitiesToUse.Count == 0) Success();
            }
        }



        public override void Update()
        {
            base.Update();

            uint entityId = 0;
            if (MyGuiManager.GetInput().GetGameControl(MyGameControlEnums.USE).IsPressed() && IsNearSomeLocation(ref entityId))
            {
                if (!m_inUse)
                {
                    StartUse();
                    m_usingEntityId = entityId;
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
            if (m_requiredTime == 0)
            {
                FinishUse();
                return;
            }
            else
            {
                m_useProgress.Reset();
                MyGuiManager.AddScreen(m_useProgress);
            }
        }

        private void FinishUse()
        {
            m_inUse = false;
            
            UseSuccess(m_usingEntityId);
        }

        public void FailUse()
        {
            m_inUse = false;
            m_elapsedTime = 0;
            //m_useProgress.CloseScreenNow();
        }


        protected override int GetObjectivesTotalCount()
        {
            return m_totalCount;
        }

        protected override int GetObjectivesCompletedCount()
        {
            return (m_totalCount - m_entitiesToUse.Count);
        }
    }
}

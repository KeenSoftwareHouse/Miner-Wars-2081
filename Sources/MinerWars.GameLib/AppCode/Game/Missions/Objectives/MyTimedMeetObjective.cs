using System;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Missions.Objectives
{   

    class MyTimedMeetObjective : MyMeetObjective
    {
        private TimeSpan m_submissionDuration;
        private TimeSpan m_remainingTime;
        private MyHudNotification.MyNotification m_countdownNotification;


        public MyTimedMeetObjective(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? detectorId, uint botId, float distanceToTalk, float slowdown, TimeSpan submissionDuration, MyDialogueEnum? successDialogueId = null, MyDialogueEnum? startDialogueId = null, bool stopFollow = true) 
            : base(Name, ID, Description, ParentMission, RequiredMissions, detectorId, botId, distanceToTalk, slowdown, successDialogueId, startDialogueId, stopFollow)
        {
            m_submissionDuration = submissionDuration;
            m_remainingTime = submissionDuration;
        }
        public override void Load()
        {
            base.Load();
            m_remainingTime = m_submissionDuration;
            m_countdownNotification = new MyHudNotification.MyNotification(MyTextsWrapperEnum.Countdown, (int)m_remainingTime.TotalMilliseconds, null);
            MyHudNotification.AddNotification(m_countdownNotification,MyGuiScreenGamePlayType.GAME_STORY);
        }

        public override void Update()
        {
            base.Update();
            m_remainingTime = m_remainingTime - new TimeSpan(0, 0, 0, 0, MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);
            if (m_remainingTime.TotalMilliseconds < 0 || m_detectorReached)
            {
                m_countdownNotification.Disappear();
            }

            m_countdownNotification.SetTextFormatArguments(new object[] { String.Format("{0:00}", m_remainingTime.Minutes) + ":" + String.Format("{0:00}", m_remainingTime.Seconds) });

            //if not speaking
            if (m_remainingTime.TotalSeconds<=0 && !m_detectorReached)
            {
                Fail(MyTextsWrapperEnum.Fail_TimeIsUp);        
            }
        }

        
        public float Progress
        {
            get
            {
                return (float)(1 - (m_remainingTime.TotalMilliseconds / m_submissionDuration.TotalMilliseconds));
            }
        }
        

        public override void Unload()
        {
            base.Unload();
            if (m_countdownNotification!=null) m_countdownNotification.Disappear();
        }


    }
}
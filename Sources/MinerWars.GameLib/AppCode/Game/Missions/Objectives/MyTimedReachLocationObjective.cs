using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Missions
{
    class MyTimedReachLocationObjective : MyObjective
    {
        private TimeSpan m_submissionDuration;
        private TimeSpan m_remainingTime;
        private MyHudNotification.MyNotification m_countdownNotification;
        public event Action OnMissionFailed;

        public MyTimedReachLocationObjective(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, MyMissionLocation location)
            : base(name, id, description, icon, parentMission, requiredMissions, location)
        {
            m_submissionDuration = submissionDuration;
            m_remainingTime = submissionDuration;
        }

        public MyTimedReachLocationObjective(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, MyMissionLocation location)
            : base(name, id, description, icon, parentMission, requiredMissions, location)
        {
            m_submissionDuration = submissionDuration;
            m_remainingTime = submissionDuration;
        }

        /// <summary>
        /// Return a real number between 0 and 1 indicating how much time has passed from the beginning of the mission.
        /// </summary>
        public float Progress
        {
            get
            {
                return (float)(1 - (m_remainingTime.TotalMilliseconds / m_submissionDuration.TotalMilliseconds));
            }
        }

        public override void Load()
        {
            base.Load();
            m_remainingTime = m_submissionDuration;
            m_countdownNotification = new MyHudNotification.MyNotification(MyTextsWrapperEnum.Countdown, (int)m_remainingTime.TotalMilliseconds, null);
            MyHudNotification.AddNotification(m_countdownNotification);
        }

        public override void Update()
        {
            base.Update();


            if (m_remainingTime.TotalMilliseconds <= 0)
            {
                // submission event?
                if (OnMissionFailed != null) OnMissionFailed();
                else 
                    Fail(MyTextsWrapperEnum.Fail_TimeIsUp);
            }
            else
            {
                m_remainingTime -= new TimeSpan(0, 0, 0, 0, MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);
                m_countdownNotification.SetTextFormatArguments(new object[] { String.Format("{0:00}", m_remainingTime.Minutes) + ":" + String.Format("{0:00}", m_remainingTime.Seconds) });
            }
        }

        public override bool IsSuccess()
        {
            bool success = base.IsSuccess();
            if (success && m_countdownNotification != null)
            {
                m_countdownNotification.Disappear();
            }
            return success;
        }
    }
}

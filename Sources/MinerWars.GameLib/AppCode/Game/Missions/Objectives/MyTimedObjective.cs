using System;
using System.Diagnostics;
using System.Text;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.Missions
{
    class MyTimedObjective : MyObjective
    {
        private MyHudNotification.MyNotification m_countdownNotification;
        private bool m_isOn = true;
        private bool m_defaultState = true;

        public MyTextsWrapperEnum NotificationText = MyTextsWrapperEnum.Countdown;
        public bool DisplayCounter { get; set; }

        public MyTimedObjective(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, MyDialogueEnum? successDialogId = null, MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, null, successDialogId, startDialogId)
        {
            SubmissionDuration = submissionDuration;
            DisplayCounter = true;
        }

        public MyTimedObjective(MyTextsWrapperEnum name, MyMissionID id, MyTextsWrapperEnum description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, bool isOn)
            : this(name, id, description, icon, parentMission, requiredMissions, submissionDuration)
        {
            m_isOn = isOn;
            m_defaultState = m_isOn;
            DisplayCounter = true;
        }

        [Obsolete]
        public MyTimedObjective(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, MyDialogueEnum? successDialogId = null, MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, null, null, successDialogId, startDialogId)
        {
            SubmissionDuration = submissionDuration;
            DisplayCounter = true;
        }

        [Obsolete]
        public MyTimedObjective(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, TimeSpan submissionDuration, bool isOn)
            : this(name, id, description, icon, parentMission, requiredMissions, submissionDuration)
        {
            m_isOn = isOn;
            m_defaultState = m_isOn;
            DisplayCounter = true;
        }

        /// <summary>
        /// Return a real number between 0 and 1 indicating how much time has passed from the beginning of the mission.
        /// </summary>
        public float Progress
        {
            get
            {
                return (float)(1 - (RemainingTime.TotalMilliseconds / SubmissionDuration.TotalMilliseconds));
            }
        }

        public TimeSpan RemainingTime
        {
            get { return SubmissionDuration - MissionTimer.GetElapsedTime(); }
        }

        public TimeSpan SubmissionDuration { get; set; }

        public override void Load()
        {
            base.Load();
            if (m_isOn)
            {
                m_countdownNotification = new MyHudNotification.MyNotification(NotificationText, MyGuiManager.GetFontMinerWarsBlue(), (int)RemainingTime.TotalMilliseconds, null);
                m_countdownNotification.IsImportant = true;
                if (DisplayCounter) 
                    MyHudNotification.AddNotification(m_countdownNotification);
            }
        }

        public override void Unload()
        {
            base.Unload();
            m_isOn = m_defaultState;
        }

        public override void Update()
        {
            base.Update();

            if (m_countdownNotification == null)  //loaded checkpoint
            {
                m_isOn = false;
            }

            if (m_isOn)
            {
                Debug.Assert(m_countdownNotification != null, "m_countdownNotification != null");
                m_countdownNotification.SetTextFormatArguments(new object[] { String.Format("{0:00}", RemainingTime.Minutes) + ":" + String.Format("{0:00}", RemainingTime.Seconds) });
                SendTimer(false, DisplayCounter);
            }
        }

        public void SkipTimer()
        {
            MissionTimer.SetElapsedTime(SubmissionDuration);
            //SubmissionDuration = MissionTimer.GetElapsedTime();
            m_countdownNotification.Disappear();
            SendTimer(true, false);
        }

        public override bool IsSuccess()
        {
            return RemainingTime.TotalMilliseconds <= 0;
        }

        public void Suspend(bool isOn)
        {
            if (isOn && !m_isOn)
            {
                m_countdownNotification = new MyHudNotification.MyNotification(NotificationText, (int)RemainingTime.TotalMilliseconds);
                m_countdownNotification.IsImportant = true;
                if (DisplayCounter) MyHudNotification.AddNotification(m_countdownNotification);
            }
            m_isOn = isOn;

        }

        public void HideNotification()
        {
            if (m_countdownNotification != null)
            {
                m_countdownNotification.Disappear();
            }
            SendTimer(true, false);
        }

        void SendTimer(bool force, bool displayCounter)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                var timespan = displayCounter ? RemainingTime : TimeSpan.FromTicks(0);
                MyMultiplayerGameplay.Static.SendCountdown(timespan, force);
            }
        }
    }
}

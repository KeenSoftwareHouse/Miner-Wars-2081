using System;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Missions.Utils
{
    /// <summary>
    /// Class for controlling countdown timers in missions.
    /// </summary>
    class MyCountdownTimer
    {
        readonly MyMissionTimer m_timer = new MyMissionTimer();

        TimeSpan m_countdownDuration;
        MyTimerActionDelegate m_actionAfterCountdown;

        public void Start(TimeSpan countdownDuration, MyTimerActionDelegate actionAfterCountdown)
        {
            Debug.Assert(!m_timer.Started);

            m_timer.Reset();
            m_timer.Start();

            m_countdownDuration = countdownDuration;
            m_actionAfterCountdown = actionAfterCountdown;

            m_timer.RegisterTimerAction((int)countdownDuration.TotalMilliseconds, CountdownEnded);
        }

        public TimeSpan GetRemainingTime()
        {
            return m_countdownDuration - m_timer.GetElapsedTime();
        }

        private void CountdownEnded()
        {
            m_actionAfterCountdown.Invoke();
            m_timer.End();
        }

        public void Update()
        {
            m_timer.Update();
        }
    }
}
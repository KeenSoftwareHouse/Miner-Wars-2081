using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Missions
{
    delegate void MyTimerActionDelegate();

    // Times are in milliseconds.
    class MyMissionTimer
    {
        /// <summary>
        /// Inner class Timer action
        /// </summary>
        protected class MyTimerAction
        {
            public MyTimerActionDelegate Action { get; set; }
            public bool Launched { get; set; }
            public int StartTime { get; set; }
            public string Notification { get; set; }
            public bool ShowTimeFromStart { get; set; }

            private MyHudNotification.MyNotification m_hudNotification;
            private object[] m_hudNotificationArgs;

            public MyTimerAction(int startTime, MyTimerActionDelegate action, string notification, bool showTimeFromStart)
            {
                StartTime = startTime;
                Launched = false;
                Action = action;
                Notification = notification;
                ShowTimeFromStart = showTimeFromStart;

                if (Notification != null)
                {
                    m_hudNotification = new MyHudNotification.MyNotification(Notification + "{0}", MyGuiManager.GetFontMinerWarsBlue(), MyHudNotification.DONT_DISAPEAR, null);
                    m_hudNotificationArgs = new object[1];
                }
            }

            public void Launch()
            {
                Debug.Assert(!Launched);
                Launched = true;
                Action();
                if (m_hudNotification != null)
                {
                    m_hudNotification.Disappear();
                }
            }

            public void Stop()
            {
                if (m_hudNotification != null)
                {
                    m_hudNotification.Disappear();
                }
            }

            public void Start()
            {
                if (m_hudNotification != null)
                {
                    if (m_hudNotification.IsDisappeared())
                    {
                        m_hudNotification.Appear();
                    }
                    MyHudNotification.AddNotification(m_hudNotification);
                }
            }

            public void Update(int elapsedTime)
            {
                if (m_hudNotification != null)
                {
                    TimeSpan remainingTime;
                    if (ShowTimeFromStart)
                    {
                        remainingTime = TimeSpan.FromMilliseconds(elapsedTime);
                    }
                    else
                    {
                        remainingTime = TimeSpan.FromMilliseconds(StartTime - elapsedTime);
                    }
                    m_hudNotificationArgs[0] = String.Format("{0:00}", remainingTime.Minutes) + ":" + String.Format("{0:00}", remainingTime.Seconds);
                    m_hudNotification.SetTextFormatArguments(m_hudNotificationArgs);
                }
            }
        }

        private List<MyTimerAction> m_registeredActions;
        private int m_elapsedTime;

        public bool Started { get; private set; }

        public int ElapsedTime
        {
            get { return m_elapsedTime; }
        }

        public MyMissionTimer()
        {
            m_registeredActions = new List<MyTimerAction>();
        }

        public TimeSpan GetElapsedTime()
        {
            return new TimeSpan(0, 0, 0, 0, ElapsedTime);
        }

        public void Start()
        {
            Started = true;
            foreach (MyTimerAction timerAction in m_registeredActions)
            {
                timerAction.Start();
            }
        }

        public void Reset()
        {
            m_elapsedTime = 0;
        }

        public void End()
        {
            Started = false;
            foreach (MyTimerAction timerAction in m_registeredActions)
            {
                timerAction.Stop();
            }
        }

        public void RegisterTimerAction(int startTime, MyTimerActionDelegate timerActionDelegate, bool fromMissionStart = true, string displayNotification = null, bool showTimeFromStart = false)
        {
            int timerActionStartTime = startTime + (fromMissionStart ? 0 : ElapsedTime);
            MyTimerAction timerAction = new MyTimerAction(timerActionStartTime, timerActionDelegate, displayNotification, showTimeFromStart);
            if (Started)
            {
                timerAction.Start();
            }
            m_registeredActions.Add(timerAction);
        }

        public void RegisterTimerAction(TimeSpan startTime, MyTimerActionDelegate timerActionDelegate, bool fromMissionStart = true, string displayNotification = null, bool displayElapsedTime = false)
        {
            RegisterTimerAction((int)startTime.TotalMilliseconds, timerActionDelegate, fromMissionStart, displayNotification, displayElapsedTime);
        }

        public void Update()
        {
            if (Started)
            {
                m_elapsedTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                int index = 0;
                while (index < m_registeredActions.Count)
                {
                    MyTimerAction timerAction = m_registeredActions[index];
                    if (ElapsedTime >= timerAction.StartTime)
                    {
                        Debug.Assert(!timerAction.Launched);
                        timerAction.Launch();
                        if (m_registeredActions.Count > 0)
                        {
                            m_registeredActions.RemoveAt(index);
                        }
                    }
                    else
                    {
                        timerAction.Update(ElapsedTime);
                        index++;
                    }
                }
            }
        }

        public void ClearActions()
        {
            // hide notifications
            foreach (var item in m_registeredActions)
            {
                item.Stop();
            }

            m_registeredActions.Clear();
        }

        internal void SetElapsedTime(TimeSpan timeSpan)
        {
            m_elapsedTime = (int)timeSpan.TotalMilliseconds;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Audio
{
    /// <summary>
    /// Handles audio queue for hud, plays always only one HUD cue at a time
    /// </summary>
    static class MyHudAudio
    {
        struct MyHudCue
        {
            public MySoundCuesEnum CueEnum;
            public float Volume;
        }

        struct MyActiveCueInfo
        {
            public MySoundCuesEnum CueEnum;
            public MySoundCue Cue;
        }

        //  Hud cues
        static Queue<MyHudCue> m_hudCuesQueue;
        static MyActiveCueInfo? m_activeCue;

        const int m_hudCueDelay = 1000;  // in ms
        static int m_hudTimeFromLastCuePlayed = 0;

        public static int QueueLength { get { return m_hudCuesQueue != null ? m_hudCuesQueue.Count : 0; } }

        public static void LoadData()
        {
            m_hudCuesQueue = new Queue<MyHudCue>();
        }

        public static void UnloadData()
        {
            m_hudCuesQueue = null;
            if (m_activeCue.HasValue)
            {
                m_activeCue.Value.Cue.Stop(StopFlags.Immediate);
            }
        }

        /// <summary>
        /// Cleans hud audio queue and stops any sounds
        /// </summary>
        public static void Reset()
        {
            if (m_hudCuesQueue != null)
            {
                m_hudCuesQueue.Clear();
            }

            if (m_activeCue.HasValue)
            {
                m_activeCue.Value.Cue.Stop(StopFlags.Release);
                m_activeCue = null;
            }
        }

        public static bool AddHudCue(MySoundCuesEnum cueEnum, float volume)
        {
            // Playing cue right now?
            if (m_activeCue.HasValue && m_activeCue.Value.CueEnum == cueEnum)
                return false;

            foreach (var hudCue in m_hudCuesQueue)
            {
                if (hudCue.CueEnum == cueEnum)
                {
                    return false;
                }
            }

            m_hudCuesQueue.Enqueue(new MyHudCue() { CueEnum = cueEnum, Volume = volume });
            Debug.Assert(m_hudCuesQueue.Count < 100, "Too many hud sounds were enqueued, are you checking if sound was already enqueud?");
            return true;
        }

        public static void Update()
        {
            m_hudTimeFromLastCuePlayed += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

            // if actual hud cue still playing
            if (m_activeCue.HasValue && m_activeCue.Value.Cue.IsPlaying)
            {
                return;
            }

            // we dequeue next hud cue a play it
            if (m_hudCuesQueue.Count > 0)
            {
                // we want have small delay between hud cues
                if (m_hudTimeFromLastCuePlayed >= m_hudCueDelay || m_activeCue == null)
                {
                    MyHudCue nextHudCue = m_hudCuesQueue.Dequeue();
                    var cue = MyAudio.PlayCueNow2D(nextHudCue.CueEnum, nextHudCue.Volume);

                    m_activeCue = new MyActiveCueInfo() { Cue = cue, CueEnum = nextHudCue.CueEnum };
                    m_hudTimeFromLastCuePlayed = 0;
                }
            }
            else
            {
                m_activeCue = null;
            }
        }
    }
}

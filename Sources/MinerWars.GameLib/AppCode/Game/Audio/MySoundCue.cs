using System;
using System.Diagnostics;
using SharpDX.X3DAudio;
using SharpDX.XACT3;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Audio
{
    /// <summary>
    /// Cue which has started playing.
    /// </summary>
    struct MySoundCue
    {
        /// <summary>
        /// Never null
        /// </summary>
        private readonly MyCueProxy m_cue;
        private readonly MySoundCuesEnum m_cueEnum;
        private readonly bool m_apply3d;
        private readonly uint m_version;

        public bool IsSame(MySoundCue cue)
        {
            return m_cue == cue.m_cue && m_version == cue.m_version;
        }

        public bool IsValid { get { return m_cue.Version == m_version; } }

        public bool IsAmbientSound { get { return MyEnumsToStrings.Sounds[(int)CueEnum].StartsWith("Amb"); } }
        
        public MySoundCue(MyCueProxy cue, MySoundCuesEnum cueEnum, bool apply3d)
        {
            m_cue = cue;
            m_version = cue.Version;
            m_cueEnum = cueEnum;
            m_apply3d = apply3d;
        }

        [Obsolete("This will be removed")]
        public MySoundCuesEnum CueEnum
        {
            get
            {
                return m_cueEnum;
            }
        }

        [Obsolete("This will be removed")]
        public bool Is3D
        {
            get
            {
                return m_apply3d;
            }
        }

        // In future, this will be changed (won't test valid)
        public bool IsPlaying { get { return IsValid && State == CueState.Playing; } }

        // In future, this will be changed (won't test valid)
        public bool IsStopped { get { return IsValid && State == CueState.Stopped; } }

        public bool IsStopping { get { return CheckState(CueState.Stopping); } }        
        public bool IsPaused { get { return CheckState(CueState.Paused); } }

        public void Apply3D(Listener listener, Emitter emiter)
        {
            if (IsValid)
            {
                m_cue.Apply3D(listener, emiter);
            }
        }

        public void SetVariable(MyCueVariableEnum variable, float value)
        {
            if (IsValid)
            {
                m_cue.SetVariable(variable, value);
            }
        }

        public void Stop(StopFlags mode)
        {
            if (IsValid)
            {
                this.m_cue.Stop(mode);
            }
        }

        public void Pause()
        {
            if (IsValid)
            {
                this.m_cue.Pause(true);
            }
        }

        public void Resume()
        {
            if (IsValid)
            {
                this.m_cue.Pause(false);
            }
        }

        private CueState State
        {
            get
            {
                return m_cue.State;
            }
        }

        bool CheckState(CueState state)
        {
            Debug.Assert(IsValid, "Before checking state, test IsValid!");
            return IsValid && ((int)(m_cue.State & state) == (int)state);
        }
    }
}

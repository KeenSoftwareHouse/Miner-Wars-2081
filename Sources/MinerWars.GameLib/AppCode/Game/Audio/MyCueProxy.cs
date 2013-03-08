using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;
using SharpDX.X3DAudio;

namespace MinerWars.AppCode.Game.Audio
{
    class MyCueProxy
    {
        /// <summary>
        /// Private interface for MyCue (used by pool)
        /// </summary>
        public class Private
        {
            public readonly MyCueProxy Cue = new MyCueProxy();
            public void OnInit(Cue cue, MySoundCuesEnum cueEnum)
            {
                Cue.m_cue = cue;
                Cue.CueEnum = cueEnum;
            }

            public void OnRelease()
            {
                unchecked
                {
                    Cue.m_version++;
                }
            }

            public void Destroy()
            {
                Cue.m_cue.Destroy();
            }
        }

        Cue m_cue;
        uint m_version;

        public MySoundCuesEnum CueEnum { get; private set; }
        public uint Version { get { return m_version; } }

        public CueState State
        {
            get
            {
                return m_cue.State;
            }
        }

        public void Apply3D(Listener listener, Emitter emiter)
        {
            m_cue.Apply3D(listener, emiter);
        }

        public void Stop(StopFlags mode)
        {
            m_cue.Stop(mode);
        }

        public void Pause(bool pause)
        {
            m_cue.Pause(pause);
        }

        public void SetVariable(MyCueVariableEnum variable, float value)
        {
            m_cue.SetVariable(variable, value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;

namespace MinerWars.AppCode.Game.Audio
{
    public struct AudioCategory
    {
        private AudioEngine m_audioEngine;
        private short m_index;
        private string m_name;

        public short CategoryIndex { get { return m_index; } }
        public string CategoryName { get { return m_name; } }

        public AudioCategory(AudioEngine audioEngine, string categoryName)
        {
            this.m_index = audioEngine.GetCategory(categoryName);
            this.m_audioEngine = audioEngine;
            this.m_name = categoryName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AudioCategory))
            {
                return false;
            }
            else
            {
                return ((AudioCategory)obj) == this;
            }
        }

        public override int GetHashCode()
        {
            return m_index;
        }

        public static bool operator==(AudioCategory a, AudioCategory b)
        {
            return a.m_index == b.m_index;
        }

        public static bool operator !=(AudioCategory a, AudioCategory b)
        {
            return !(a == b);
        }

        public void SetVolume(float volume)
        {
            m_audioEngine.SetVolume(CategoryIndex, volume);
        }

        public void Pause()
        {
            m_audioEngine.Pause(CategoryIndex, true);
        }

        public void Resume()
        {
            m_audioEngine.Pause(CategoryIndex, false);
        }

        public void Stop(StopFlags stopFlags)
        {
            m_audioEngine.Stop(m_index, stopFlags);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;

namespace MinerWars.AppCode.Game.Audio
{
    struct MyCueInfo
    {
        public SoundBank SoundBank;
        public short CueIndex;

        public Cue Prepare()
        {
            return SoundBank.Prepare(CueIndex);
        }
    }
}

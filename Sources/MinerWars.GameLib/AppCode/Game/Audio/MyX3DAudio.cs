using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.X3DAudio;
using SharpDX.XACT3;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq.Expressions;

namespace MinerWars.AppCode.Game.Audio
{
    class MyX3DAudio
    {
        X3DAudio m_x3dAudio;
        DspSettings m_dsp;

        public MyX3DAudio(AudioEngine engine)
        {
            m_x3dAudio = new X3DAudio(engine.FinalMixFormat.ChannelMask);
            m_dsp = new DspSettings(1, engine.FinalMixFormat.Channels);
        }

        public void Apply3D(Cue cue, Listener listener, Emitter emitter)
        {
            m_x3dAudio.Calculate(listener, emitter, CalculateFlags.Matrix | CalculateFlags.Doppler | CalculateFlags.EmitterAngle, m_dsp);

            cue.SetMatrixCoefficients(m_dsp.SourceChannelCount, m_dsp.DestinationChannelCount, m_dsp.MatrixCoefficients);
            cue.SetVariable(MyCueVariableEnum.Distance, m_dsp.EmitterToListenerDistance);
            cue.SetVariable(MyCueVariableEnum.DopplerPitchScalar, m_dsp.DopplerFactor);
            cue.SetVariable(MyCueVariableEnum.OrientationAngleDegrees, m_dsp.EmitterToListenerAngle * 57.29578f); // From radians to degrees
        }
    }
}

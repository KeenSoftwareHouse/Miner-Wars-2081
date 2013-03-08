using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;
using System.Diagnostics;
using SharpDX.X3DAudio;
using MinerWarsMath;
using SharpDX.Toolkit;

namespace MinerWars.AppCode.Game.Audio
{
    public static class AudioEngineExtensions
    {
        public static AudioCategory GetCategoryInstance(this AudioEngine engine, string categoryName)
        {
            return new AudioCategory(engine, categoryName);
        }

        private static bool HasState(this Cue cue, CueState state)
        {
            return (cue.State & state) != 0;
        }

        public static bool IsPlaying(this Cue cue)
        {
            return cue.HasState(CueState.Playing);
        }

        public static bool IsPrepared(this Cue cue)
        {
            return cue.HasState(CueState.Prepared);
        }

        public static bool IsPaused(this Cue cue)
        {
            return cue.HasState(CueState.Paused);
        }

        public static bool IsStopping(this Cue cue)
        {
            return cue.HasState(CueState.Stopping);
        }

        public static bool IsStopped(this Cue cue)
        {
            return cue.HasState(CueState.Stopped);
        }

        public static bool IsValid(this Cue cue)
        {
            return cue.NativePointer != IntPtr.Zero;
        }

        public static void SetVariable(this Cue cue, MyCueVariableEnum variableEnum, float value)
        {
            MyXactVariables.SetVariable(cue, variableEnum, value);
        }

        public static void SetGlobalVariable(this AudioEngine engine, MyGlobalVariableEnum variableEnum, float value)
        {
            MyXactVariables.SetVariable(engine, variableEnum, value);
        }

        public static bool IsPrepared(this WaveBank waveBank)
        {
            const int StatePrepared = 4;

            return (waveBank.State & StatePrepared) != 0;
        }

        public static void Apply3D(this Cue cue, Listener listener, Emitter emitter)
        {
            MyAudio.X3DAudio.Apply3D(cue, listener, emitter);
        }

        /// <summary>
        /// Sets default values for emitter, makes it valid
        /// </summary>
        public static void SetDefaultValues(this Emitter emitter)
        {
            emitter.Position = SharpDX.Vector3.Zero;
            emitter.Velocity = SharpDX.Vector3.Zero;
            emitter.OrientFront = SharpDX.Vector3.UnitZ;
            emitter.OrientTop = SharpDX.Vector3.UnitY;
            emitter.ChannelCount = 1;
            emitter.CurveDistanceScaler = float.MinValue;
        }

        /// <summary>
        /// Sets default values for listener, makes it valid
        /// </summary>
        public static void SetDefaultValues(this Listener listener)
        {
            listener.Position = SharpDX.Vector3.Zero;
            listener.Velocity = SharpDX.Vector3.Zero;
            listener.OrientFront = SharpDX.Vector3.UnitZ;
            listener.OrientTop = SharpDX.Vector3.UnitY;
        }
        
        /// <summary>
        /// Updates emitter position, forward, up and velocity
        /// </summary>
        public static void UpdateValues(this Emitter emitter, ref Vector3 position, ref Vector3 forward, ref Vector3 up, ref Vector3 velocity)
        {
            emitter.Position = SharpDXHelper.ToSharpDX(position);
            emitter.OrientFront = SharpDXHelper.ToSharpDX(forward);
            emitter.OrientTop = SharpDXHelper.ToSharpDX(up);
            emitter.Velocity = SharpDXHelper.ToSharpDX(velocity);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using SharpDX.XACT3;

namespace MinerWars.AppCode.Game.Audio
{
    public enum MyCueVariableEnum
    {
        Volume,
        AmbVolume,
        Pitch,
        Occluder,
        MusicProg1Progression,
        RotatingSpeed,
        ShipASpeed,
        ShipAIdle,

        // 3D variables (Apply3D sets it)
        Distance,
        DopplerPitchScalar,
        OrientationAngleDegrees,
    }

    public enum MyGlobalVariableEnum
    {
        ReverbControl,
    }

    /// <summary>
    /// Caching class for cue variable indices.
    /// Variable index is same for all cues in audio engine
    /// </summary>
    static class MyXactVariables
    {
        public static bool CacheEnabled = true;

        struct MyVar
        {
            public short VariableIndex;
            public string VariableName;
        }

        static MyVar[] m_cueVariables;
        static MyVar[] m_glovalVariables;

        public static void LoadData()
        {
            m_cueVariables = new MyVar[MyMwcUtils.GetMaxValueFromEnum<MyCueVariableEnum>() + 1];
            m_glovalVariables = new MyVar[MyMwcUtils.GetMaxValueFromEnum<MyCueVariableEnum>() + 1];

            m_cueVariables[(int)MyCueVariableEnum.AmbVolume] = new MyVar() { VariableName = "AmbVolume", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.MusicProg1Progression] = new MyVar() { VariableName = "MusicProg1Progression", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.Occluder] = new MyVar() { VariableName = "Occluder", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.Pitch] = new MyVar() { VariableName = "Pitch", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.RotatingSpeed] = new MyVar() { VariableName = "RotatingSpeed", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.Volume] = new MyVar() { VariableName = "Volume", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.ShipASpeed] = new MyVar() { VariableName = "Ship A Speed", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.ShipAIdle] = new MyVar() { VariableName = "Ship A Idle", VariableIndex = -1 };

            m_cueVariables[(int)MyCueVariableEnum.Distance] = new MyVar() { VariableName = "Distance", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.DopplerPitchScalar] = new MyVar() { VariableName = "DopplerPitchScalar", VariableIndex = -1 };
            m_cueVariables[(int)MyCueVariableEnum.OrientationAngleDegrees] = new MyVar() { VariableName = "OrientationAngle", VariableIndex = -1 };

            m_glovalVariables[(int)MyGlobalVariableEnum.ReverbControl] = new MyVar() { VariableName = "ReverbControl", VariableIndex = -1 };
        }

        public static void UnloadData()
        {
            m_cueVariables = null;
            m_glovalVariables = null;
        }

        public static void SetVariable(Cue cue, MyCueVariableEnum variableEnum, float value)
        {
            var varInfo = m_cueVariables[(int)variableEnum];

            if (varInfo.VariableIndex == -1 || CacheEnabled == false)
            {
                varInfo.VariableIndex = cue.GetVariableIndex(varInfo.VariableName);
                m_cueVariables[(int)variableEnum].VariableIndex = varInfo.VariableIndex;
            }
            cue.SetVariable(varInfo.VariableIndex, value);
        }

        public static void SetVariable(AudioEngine engine, MyGlobalVariableEnum variableEnum, float value)
        {
            var varInfo = m_glovalVariables[(int)variableEnum];

            if (varInfo.VariableIndex == -1 || CacheEnabled == false)
            {
                varInfo.VariableIndex = engine.GetGlobalVariableIndex(varInfo.VariableName);
                m_glovalVariables[(int)variableEnum].VariableIndex = varInfo.VariableIndex;
            }
            engine.SetGlobalVariable(varInfo.VariableIndex, value);
        }

        public static short GetVariableIndex(MyGlobalVariableEnum variableEnum)
        {
            return m_glovalVariables[(int)variableEnum].VariableIndex;
        }

        public static short GetVariableIndex(MyCueVariableEnum variableEnum)
        {
            return m_cueVariables[(int)variableEnum].VariableIndex;
        }

        public static string GetVariableName(MyGlobalVariableEnum variableEnum)
        {
            return m_glovalVariables[(int)variableEnum].VariableName;
        }

        public static string GetVariableName(MyCueVariableEnum variableEnum)
        {
            return m_cueVariables[(int)variableEnum].VariableName;
        }

        public static void SetVariableIndex(MyGlobalVariableEnum variableEnum, short index)
        {
            m_glovalVariables[(int)variableEnum].VariableIndex = index;
        }

        public static void SetVariableIndex(MyCueVariableEnum variableEnum, short index)
        {
            m_cueVariables[(int)variableEnum].VariableIndex = index;
        }
    }
}

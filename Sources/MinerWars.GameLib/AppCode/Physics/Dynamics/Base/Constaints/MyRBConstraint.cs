#region Using Statements

using System;
using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    public enum ConstraintFlag
    {
        CF_INSERTED = 1 << 0,
    }

    /// <summary>
    /// Base constraint for rbelements
    /// </summary>
    abstract class MyRBConstraint
    {
        public MyRBConstraint()
        {
            m_Magnitude = 0.0f;
            m_Flags = 0;
        }

        public float m_Magnitude;

        public ConstraintFlag m_Flags;

	};

}
#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// description class for sensor
    /// </summary>
    class MySensorDesc
    {
        #region members

        public Matrix m_Matrix;
        public MySensorElement m_Element;
        public IMySensorEventHandler m_SensorEventHandler;

        #endregion

        public MySensorDesc()
        {
            m_Matrix = Matrix.Identity;
            m_SensorEventHandler = null;
            m_Element = null;
        }

        /// <summary>
        /// default settings
        /// </summary>
        public void SetToDefault()
        {
            m_Matrix = Matrix.Identity;
            m_SensorEventHandler = null;
        }

        /// <summary>
        /// validity check for the descriptor
        /// </summary>
        public bool IsValid()
        {
            if (m_Element == null)
            {
                return false;
            }

            // without event handler it does not make sense to create the sensor
            if (m_SensorEventHandler == null)
            {
                return false;
            }

            return true;
        }
    }
}

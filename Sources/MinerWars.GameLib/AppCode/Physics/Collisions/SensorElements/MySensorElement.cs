#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Sensor element used for sensors
    /// </summary>
    abstract class MySensorElement: MyElement
    {
        public MySensorElement()
        {
            m_Sensor = null;
            Flags = MyElementFlag.EF_SENSOR_ELEMENT | MyElementFlag.EF_AABB_DIRTY;
            DetectRigidBodyTypes = null;
        }

        public virtual MySensorElementType GetElementType() { return MySensorElementType.ET_UNKNOWN; }

        public override Matrix GetGlobalTransformation() { return m_Matrix * m_Sensor.Matrix; }

        public MySensor Sensor { set { m_Sensor = value; } get { return m_Sensor; } }

        private MySensor m_Sensor;

        public uint? DetectRigidBodyTypes { get; set; }

        public abstract void SetSize(Vector3 size);

        public void Close() 
        {
            m_Sensor = null;
        }
    }
}

#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Sphere sensor element implement AABB stuff
    /// </summary>
    class MySphereSensorElement: MySensorElement
    {
        #region interface

        public MySphereSensorElement()
        {

        }

        public MySphereSensorElement(float radius)
        {
            SetRadius(radius);
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            base.LoadFromDesc(desc);

            MySphereSensorElementDesc se = (MySphereSensorElementDesc) desc;

            m_Matrix = se.m_Matrix;

            SetRadius(se.m_Radius);

            return true;
        }

        /// <summary>
        /// Angle in radians, which must has RB with forward vector of sensor element
        /// </summary>
        public float? SpecialDetectingAngle { get; set; }

        public float Radius { get { return this.m_Radius; } set { this.SetRadius(value); } }

        public override MySensorElementType GetElementType() { return MySensorElementType.ET_SPHERE; }

        #endregion

        public override void UpdateAABB()
        {            
            Vector3 origin = GetGlobalTransformation().Translation;
            m_AABB.Min = origin - m_Extent;
            m_AABB.Max = origin + m_Extent;
            base.UpdateAABB();
        }

        #region members
        private float   m_Radius;
        private Vector3 m_Extent;
        #endregion

#region implementation
        public override void SetSize(Vector3 size)
        {
            SetRadius(size.Length() / 2f);
        }

        void SetRadius(float radius)
        {
            m_Radius = radius;
            m_Extent.X = m_Radius;
            m_Extent.Y = m_Radius;
            m_Extent.Z = m_Radius;

            Flags |= MyElementFlag.EF_AABB_DIRTY;

            if (Sensor != null)
            {
                MyPhysics.physicsSystem.GetSensorModule().MoveSensor(Sensor, true);
            }
        }

#endregion
    }
}

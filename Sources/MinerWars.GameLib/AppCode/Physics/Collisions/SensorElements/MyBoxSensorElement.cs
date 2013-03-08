using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Physics
{    
    /// <summary>
    /// Box sensor element implement AABB stuff
    /// </summary>
    class MyBoxSensorElement : MySensorElement
    {
        #region interface

        public MyBoxSensorElement()
        {
            m_points = new Vector3[8];
        }

        public MyBoxSensorElement(Vector3 size)
            : this()
        {
            SetSize(size);
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            base.LoadFromDesc(desc);

            MyBoxSensorElementDesc se = (MyBoxSensorElementDesc)desc;

            m_Matrix = se.m_Matrix;

            SetSize(se.Size);

            return true;
        }

        public Vector3 Size { get { return this.m_size; } set { this.SetSize(value); } }
        public Vector3 Extent { get { return m_extent; } }

        public override MySensorElementType GetElementType() { return MySensorElementType.ET_BOX; }

        #endregion

        public override void UpdateAABB()
        {
            Matrix matrix = GetGlobalTransformation();
            m_AABB = m_AABB.CreateInvalid();

            BoundingBox box = new BoundingBox(-m_extent, m_extent);
            box.GetCorners(m_points);
            Vector3 point2;
            Vector3 point1;

            foreach (Vector3 point in m_points)
            {
                point1 = point;
                Vector3.Transform(ref point1, ref matrix, out point2);
                MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, point2);
            }
            base.UpdateAABB();
        }

        #region members        
        private Vector3 m_size;
        private Vector3 m_extent;
        private Vector3[] m_points;
        #endregion

        #region implementation
        public override void SetSize(Vector3 size)
        {
            m_size = size;
            m_extent = size / 2f;

            Flags |= MyElementFlag.EF_AABB_DIRTY;

            if (Sensor != null)
            {
                MyPhysics.physicsSystem.GetSensorModule().MoveSensor(Sensor, true);
            }
        }

        #endregion
    }
}

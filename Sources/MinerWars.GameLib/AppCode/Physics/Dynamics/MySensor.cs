using System;
using System.Collections.Generic;
using MinerWarsMath;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Sensor class used for rigid body enter/leave certain area
    /// </summary>
    class MySensor
    {
        #region members
        private Matrix m_Matrix;
        private MySensorElement m_Element;
        private ushort m_Guid;
        private bool m_Inserted;
        public object m_UserData;
        private IMySensorEventHandler m_SensorEventHandler;
        private bool m_active;
        private bool m_isMarkedForClose;

        private static ushort GUID_COUNTER = 0;

        public Dictionary<int, MySensorInteraction> m_Interactions;
        #endregion

        public MySensor()
        {
            m_UserData = null;

            m_Element = null;

            m_SensorEventHandler = null;

            m_Matrix = new Matrix();

            m_Inserted = false;

            m_isMarkedForClose = false;

            m_Interactions = new Dictionary<int, MySensorInteraction>(32);

            m_Guid = GUID_COUNTER;
            if (GUID_COUNTER == ushort.MaxValue)
            {
                GUID_COUNTER = 0;
            }
            GUID_COUNTER++;
        }

        /// <summary>
        /// Loads descriptor of the sensor and initialize the sensor
        /// </summary>
        public bool LoadFromDesc(MySensorDesc desc)
        {
            if (m_Inserted)
            {
                return false;
            }

            if(!desc.IsValid())
                return false;
            
            m_Matrix = desc.m_Matrix;
            m_Element = desc.m_Element;

            m_Element.Sensor = this;

            m_SensorEventHandler = desc.m_SensorEventHandler;
            MyCommonDebugUtils.AssertDebug(m_Interactions.Count == 0);
            m_Interactions.Clear();

            m_Guid = GUID_COUNTER;
            if (GUID_COUNTER > ushort.MaxValue)
            {
                GUID_COUNTER = 0;
            }

            GUID_COUNTER++;

            return true;
        }

        public void MarkForClose() 
        {
            m_isMarkedForClose = true;
        }

        public bool IsMarkedForClose() 
        {
            return m_isMarkedForClose;
        }

        public void Close() 
        {
            MyCommonDebugUtils.AssertDebug(m_isMarkedForClose);
            MyCommonDebugUtils.AssertDebug(m_Interactions.Count == 0);                        

            if (m_Element != null) 
            {
                m_Element.Close();
                m_Element = null;
            }

            m_SensorEventHandler = null;

            m_Interactions.Clear();
            m_Interactions = null;

            m_UserData = null;
        }

        public Matrix Matrix { get { return this.m_Matrix; } set { SetMatrix(value); } }

        /// <summary>
        /// checks if sensor is inserted - simulated
        /// </summary>
        public bool Inserted { get { return m_Inserted; } set { m_Inserted = value; } }

        public bool Active 
        { 
            get 
            { 
                return m_active; 
            } 
            set 
            {
                bool changed = m_active != value;
                m_active = value;
                if (changed) 
                {
                    if (m_active)
                    {
                        MyPhysics.physicsSystem.GetSensorModule().ActiveSensors.Add(this);
                    }
                    else 
                    {
                        MyPhysics.physicsSystem.GetSensorModule().ActiveSensors.Remove(this);
                    }
                }
            } 
        }

        /// <summary>
        /// returns element of the sensor which specifies the shape of the sensor
        /// </summary>
        public MySensorElement GetElement() { return m_Element; }

        /// <summary>
        /// notification handler for the enter/leave events
        /// </summary>
        public IMySensorEventHandler GetHandler() { return m_SensorEventHandler; }

        public int GUID 
        {
            get { return m_Guid; }
        }

        private void SetMatrix(Matrix matr)
        {
            bool activateSensor = matr.Translation != m_Matrix.Translation;
            m_Matrix = matr;
            MyPhysics.physicsSystem.GetSensorModule().MoveSensor(this, activateSensor);
        }

        public void PrepareSensorInteractions() 
        {
            foreach (var dv in m_Interactions)
            {
                MySensorInteraction si = dv.Value;
                if (Active ||
                    //si.m_RBElement.GetRigidBody() == null || 
                    si.m_RBElement.GetRigidBody() != null &&
                    (si.m_RBElement.GetRigidBody().ReadFlag(RigidBodyFlag.RBF_ACTIVE) || !si.m_RBElement.GetRigidBody().ReadFlag(RigidBodyFlag.RBF_INSERTED)))
                {
                    si.m_IsInside = false;
                }                
            }
        }
    }
}

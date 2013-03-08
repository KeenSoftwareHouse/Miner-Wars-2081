using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Parallelization;

namespace MinerWars.AppCode.Physics
{
    public enum MySensorInteractionEnum
    {
        /// <summary>
        /// Unknown
        /// </summary>
        SI_UNKNOWN,
        /// <summary>
        /// Sphere vs sphere for sphere sensor
        /// </summary>
        SI_SPHERE_SPHERE,
        /// <summary>
        /// Sphere vs box for sphere sensor
        /// </summary>
        SI_SPHERE_BOX,
        /// <summary>
        /// Sphere vs box for box sensor
        /// </summary>
        SI_BOX_SPHERE,
        /// <summary>
        /// Box vs box for box sensor
        /// </summary>
        SI_BOX_BOX,
        /// <summary>
        /// Sphere vs other element type for sphere sensor
        /// </summary>
        SI_SPHERE_OTHER,
        /// <summary>
        /// Box vs other elemnt type for box sensor
        /// </summary>
        SI_BOX_OTHER,
    }

    /// <summary>
    /// Sensor interaction
    /// </summary>
    abstract class MySensorInteraction: ParallelTasks.IWork
    {
        private MyNoArgsDelegate m_onRigidBodyDeactivatedEventHandler;

        public MySensorInteraction()
        {
            m_SensorElement = null;
            m_RBElement = null;
            m_Guid = 0;
            m_IsInside = false;
            m_onRigidBodyDeactivatedEventHandler = new MyNoArgsDelegate(OnRigidBodyDeactivated);
        }

        public virtual void DoWork()
        {
        }

        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }

        public virtual MySensorInteractionEnum GetInteractionType() { return MySensorInteractionEnum.SI_UNKNOWN; }

        public void Init(MySensorElement sensorElement, MyRBElement rbElement)
        {
            Debug.Assert(rbElement.GetRigidBody() != null);
            Debug.Assert(sensorElement.Sensor != null);
            m_SensorElement = sensorElement;
            m_RBElement = rbElement;            

            int guid1 = sensorElement.GUID;
            int guid2 = rbElement.GUID;

            if (guid1 > guid2)
            {
                int tm = guid2;
                guid2 = guid1;
                guid1 = tm;
            }

            m_Guid = guid1 + (guid2 << 16);

            m_IsInside = false;
            m_IsInUse = true;
            m_RBElement.GetRigidBody().OnDeactivated.Event += m_onRigidBodyDeactivatedEventHandler;
        }

        private void OnRigidBodyDeactivated()
        {
            Debug.Assert(m_SensorElement != null);
            Debug.Assert(m_SensorElement.Sensor != null);
            if (m_SensorElement.Sensor != null)
            {
                m_SensorElement.Sensor.GetHandler().OnLeave(m_SensorElement.Sensor, m_RBElement.GetRigidBody(), m_RBElement);
                m_SensorElement.Sensor.m_Interactions.Remove(m_Guid);
            }
            MyPhysics.physicsSystem.GetSensorInteractionModule().RemoveSensorInteraction(this);
        }

        public void Close() 
        {
            m_RBElement.GetRigidBody().OnDeactivated.Event -= m_onRigidBodyDeactivatedEventHandler;

            m_SensorElement = null;
            m_RBElement = null;
            m_Guid = 0;
            m_IsInUse = false;
        }
        
        public MySensorElement m_SensorElement;
        public MyRBElement m_RBElement;
        public int m_Guid;
        public bool m_IsInside;
        public bool m_IsInUse;        
        //public List<MyAddRemoveFrom> m_AddedRemoveFrom = new List<MyAddRemoveFrom>();        
    }

    //struct MyAddRemoveFrom 
    //{        
    //    public long CheckCount;
    //    public MyAddRemoveFromEnum AddRemoveFrom;
    //    public uint Guid;
    //    public uint CurrentGuid;

    //    public MyAddRemoveFrom(long checkCount, MyAddRemoveFromEnum addRemoveFrom, uint guid, uint currentGuid) 
    //    {            
    //        CheckCount = checkCount;
    //        AddRemoveFrom = addRemoveFrom;
    //        Guid = guid;
    //        CurrentGuid = currentGuid;
    //    }
    //}

    //enum MyAddRemoveFromEnum
    //{
    //    Add,
    //    RemoveWhenRemoveSensor,
    //    RemoveFromCurrent,
    //    RemoveWhenNotInside,
    //}
}

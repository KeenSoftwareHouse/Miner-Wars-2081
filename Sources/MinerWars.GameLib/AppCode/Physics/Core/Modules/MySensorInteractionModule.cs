#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Stores sensor interactions and updates them
    /// </summary>
    class MySensorInteractionModule
    {
        static MySensorInteractionModule()
        {
            while (m_FreeSSSi.Count < m_InitialSphereSphereSensorInteractionStack)
                m_FreeSSSi.Push(new MySphereSphereSensorInteraction());

            while (m_FreeSBSi.Count < m_InitialSphereBoxSensorInteractionStack)
                m_FreeSBSi.Push(new MySphereBoxSensorInteraction());

            while (m_FreeBSSi.Count < m_InitialBoxSphereSensorInteractionStack)
                m_FreeBSSi.Push(new MyBoxSphereSensorInteraction());

            while (m_FreeBBSi.Count < m_InitialBoxBoxSensorInteractionStack)
                m_FreeBBSi.Push(new MyBoxBoxSensorInteraction());

            while (m_FreeBOSi.Count < m_InitialBoxOtherSensorInteractionStack)
                m_FreeBOSi.Push(new MyBoxOtherSensorInteraction());

            while (m_FreeSOSi.Count < m_InitialSphereOtherSensorInteractionStack)
                m_FreeSOSi.Push(new MySphereOtherSensorInteraction());
        }

        public MySensorInteractionModule()
        {
            while (m_FreeSSSi.Count < m_InitialSphereSphereSensorInteractionStack)
                m_FreeSSSi.Push(new MySphereSphereSensorInteraction());

            while (m_FreeSBSi.Count < m_InitialSphereBoxSensorInteractionStack)
                m_FreeSBSi.Push(new MySphereBoxSensorInteraction());

            while (m_FreeBSSi.Count < m_InitialBoxSphereSensorInteractionStack)
                m_FreeBSSi.Push(new MyBoxSphereSensorInteraction());

            while (m_FreeBBSi.Count < m_InitialBoxBoxSensorInteractionStack)
                m_FreeBBSi.Push(new MyBoxBoxSensorInteraction());

            while (m_FreeBOSi.Count < m_InitialBoxOtherSensorInteractionStack)
                m_FreeBOSi.Push(new MyBoxOtherSensorInteraction());

            while (m_FreeSOSi.Count < m_InitialSphereOtherSensorInteractionStack)
                m_FreeSOSi.Push(new MySphereOtherSensorInteraction());
            
            m_CurrentInteractions.Clear();
        }

        /// <summary>
        /// adds new sensor interaction between sensor and rigid
        /// </summary>
        public void AddSensorInteraction(MySensorElement sensorElement, MyRBElement rbElement)
        {
            if (sensorElement.DetectRigidBodyTypes != null && rbElement.GetRigidBody().Type != sensorElement.DetectRigidBodyTypes.Value) 
            {
                return;
            }            
            
            MySensorInteraction si = null;

            int guid1 = sensorElement.GUID;
            int guid2 = rbElement.GUID;

            if (guid1 > guid2)
            {
                int tm = guid2;
                guid2 = guid1;
                guid1 = tm;
            }

            int guid = guid1 + (guid2 << 16);

            // if this interaction is in current interactions
            if (m_CurrentInteractions.ContainsKey(guid)) 
            {
                return;
            }            

            //if (sensorElement.Sensor.m_Interactions.TryGetValue(guid, out si))
            //{
            //    Debug.Assert(guid == si.m_Guid);
            //    m_CurrentInteractions.Add(guid, si);
            //    return;
            //}

            if (m_InteractionsInUse.TryGetValue(guid, out si))
            {
                Debug.Assert(guid == si.m_Guid);
                m_CurrentInteractions.Add(guid, si);
                return;
            }            

            switch (sensorElement.GetElementType())
            {
                case MySensorElementType.ET_SPHERE:
                    {
                        switch (rbElement.GetElementType())
                        {
                            case MyRBElementType.ET_SPHERE:
                                {
                                    if (m_FreeSSSi.Count == 0)
                                    {
                                        m_FreeSSSi.Push(new MySphereSphereSensorInteraction());
                                        m_newAllocatedInteractions++;
                                    }
                                    si = m_FreeSSSi.Pop();
                                }
                                break;
                            case MyRBElementType.ET_BOX:
                                {
                                    if (m_FreeSBSi.Count == 0)
                                    {
                                        m_FreeSBSi.Push(new MySphereBoxSensorInteraction());
                                        m_newAllocatedInteractions++;
                                    }
                                    si = m_FreeSBSi.Pop();
                                }
                                break;
                            default:
                                {
                                    if (m_FreeSOSi.Count == 0) 
                                    {
                                        m_FreeSOSi.Push(new MySphereOtherSensorInteraction());
                                        m_newAllocatedInteractions++;
                                    }
                                    si = m_FreeSOSi.Pop();
                                }
                                break;
                        }
                    }
                    break;
                case MySensorElementType.ET_BOX:
                    switch (rbElement.GetElementType())
                    {
                        case MyRBElementType.ET_SPHERE:
                            {
                                if (m_FreeBSSi.Count == 0)
                                {
                                    m_FreeBSSi.Push(new MyBoxSphereSensorInteraction());
                                    m_newAllocatedInteractions++;
                                }
                                si = m_FreeBSSi.Pop();
                            }
                            break;
                        case MyRBElementType.ET_BOX:
                            {
                                if (m_FreeBBSi.Count == 0)
                                {
                                    m_FreeBBSi.Push(new MyBoxBoxSensorInteraction());
                                    m_newAllocatedInteractions++;
                                }
                                si = m_FreeBBSi.Pop();
                            }
                            break;
                        default:
                            {
                                if (m_FreeBOSi.Count == 0)
                                {
                                    m_FreeBOSi.Push(new MyBoxOtherSensorInteraction());
                                    m_newAllocatedInteractions++;
                                }
                                si = m_FreeBOSi.Pop();
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }

            if (si == null)
            {
                return;
            }

            Debug.Assert(!si.m_IsInUse);             
            si.Init(sensorElement, rbElement);            
            Debug.Assert(guid == si.m_Guid);
            m_CurrentInteractions.Add(guid, si);
            m_InteractionsInUse.Add(guid, si);
            m_interactionsInUse++;
            if (m_interactionsInUse > m_interactionsInUseMax) 
            {
                m_interactionsInUseMax = m_interactionsInUse;
            }
        }

        public void RemoveSensorInteraction(MySensorInteraction si)
        {            
            Debug.Assert(si.m_IsInUse);            
            m_CurrentInteractions.Remove(si.m_Guid);
            m_InteractionsInUse.Remove(si.m_Guid);
            si.Close();
            switch (si.GetInteractionType())
            {
                case MySensorInteractionEnum.SI_SPHERE_SPHERE:
                    {
                        m_FreeSSSi.Push((MySphereSphereSensorInteraction)si);
                    }
                    break;
                case MySensorInteractionEnum.SI_SPHERE_BOX:
                    {
                        m_FreeSBSi.Push((MySphereBoxSensorInteraction)si);
                    }
                    break;
                case MySensorInteractionEnum.SI_BOX_SPHERE:
                    {
                        m_FreeBSSi.Push((MyBoxSphereSensorInteraction)si);
                    }
                    break;
                case MySensorInteractionEnum.SI_BOX_BOX:
                    {
                        m_FreeBBSi.Push((MyBoxBoxSensorInteraction)si);
                    }
                    break;
                case MySensorInteractionEnum.SI_BOX_OTHER:
                    {
                        m_FreeBOSi.Push((MyBoxOtherSensorInteraction)si);
                    }
                    break;
                case MySensorInteractionEnum.SI_SPHERE_OTHER:
                    { 
                        m_FreeSOSi.Push((MySphereOtherSensorInteraction)si);
                    }
                    break;
                default:
                    break;
            }
            m_interactionsInUse--;
        }

        private void PrepareSafetySensorInteractionIterator() 
        {
            m_SafetySensorInteractionIterator.Clear();
            foreach (var siKvp in m_CurrentInteractions) 
            {
                m_SafetySensorInteractionIterator.Add(siKvp.Value);
            }
        }

        /// <summary>
        /// Computes the interactions to check if there are in penetration or not
        /// </summary>
        public void UpdateInteractions(float dt)
        {
            PrepareSafetySensorInteractionIterator();
            // lets compute the work synchronouse, those interaction now are very fast to compute, can make multithreaded later
            foreach (var si in m_SafetySensorInteractionIterator) 
            {
                si.DoWork();
            }
        }

        /// <summary>
        /// Checks for new interactions to raise enter event
        /// </summary>
        public void CheckInteractions(float dt)
        {
            m_checkInteractionsActive = true;
            PrepareSafetySensorInteractionIterator();
            foreach (var si in m_SafetySensorInteractionIterator)
            {                
                MySensor sensor = si.m_SensorElement.Sensor;                

                if (!sensor.m_Interactions.ContainsKey(si.m_Guid))
                {
                    if (si.m_IsInside)
                    {
                        // new enter event
                        sensor.m_Interactions.Add(si.m_Guid, si);
                        sensor.GetHandler().OnEnter(si.m_SensorElement.Sensor, si.m_RBElement.GetRigidBody(), si.m_RBElement);
                    }
                    else
                    {
                        RemoveSensorInteraction(si);
                    }
                }
            }
            m_checkInteractionsActive = false;
        }

        public void PutCurrentInteractionsBack() 
        {
            PrepareSafetySensorInteractionIterator();
            foreach (var si in m_SafetySensorInteractionIterator)
            {
                // we want remove interactions which are not added to their sensor's interactions
                if (!si.m_SensorElement.Sensor.m_Interactions.ContainsKey(si.m_Guid)) 
                {
                    RemoveSensorInteraction(si);
                }                
            }
        }

        public void Flush()
        {
            m_CurrentInteractions.Clear();
        }

        public bool IsCheckInteractionsActive() 
        {
            return m_checkInteractionsActive;
        }

        public const int m_InitialSphereSphereSensorInteractionStack = 512;
        public const int m_InitialSphereBoxSensorInteractionStack = 512;
        public const int m_InitialBoxSphereSensorInteractionStack = 512;
        public const int m_InitialBoxBoxSensorInteractionStack = 512;
        public const int m_InitialBoxOtherSensorInteractionStack = 512;
        public const int m_InitialSphereOtherSensorInteractionStack = 512;
        private static Stack<MySphereSphereSensorInteraction> m_FreeSSSi = new Stack<MySphereSphereSensorInteraction>(m_InitialSphereSphereSensorInteractionStack);
        private static Stack<MySphereBoxSensorInteraction> m_FreeSBSi = new Stack<MySphereBoxSensorInteraction>(m_InitialSphereBoxSensorInteractionStack);
        private static Stack<MyBoxSphereSensorInteraction> m_FreeBSSi = new Stack<MyBoxSphereSensorInteraction>(m_InitialBoxSphereSensorInteractionStack);
        private static Stack<MyBoxBoxSensorInteraction> m_FreeBBSi = new Stack<MyBoxBoxSensorInteraction>(m_InitialSphereBoxSensorInteractionStack);
        private static Stack<MyBoxOtherSensorInteraction> m_FreeBOSi = new Stack<MyBoxOtherSensorInteraction>(m_InitialBoxOtherSensorInteractionStack);
        private static Stack<MySphereOtherSensorInteraction> m_FreeSOSi = new Stack<MySphereOtherSensorInteraction>(m_InitialSphereOtherSensorInteractionStack);
        private static Dictionary<int, MySensorInteraction> m_CurrentInteractions = new Dictionary<int, MySensorInteraction>(128);
        private static Dictionary<int, MySensorInteraction> m_InteractionsInUse = new Dictionary<int, MySensorInteraction>(512);
        private static List<MySensorInteraction> m_SafetySensorInteractionIterator = new List<MySensorInteraction>(128);
        private int m_interactionsInUse = 0;
        private int m_newAllocatedInteractions = 0;
        private int m_interactionsInUseMax = 0;
        private bool m_checkInteractionsActive = false;

        public int GetNewAllocatedInteractionsCount() 
        {
            return m_newAllocatedInteractions;
        }

        public int GetInteractionsInUseCount() 
        {
            return m_interactionsInUse;
        }

        public int GetInteractionsInUseCountMax() 
        {
            return m_interactionsInUseMax;
        }
    }
}

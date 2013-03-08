#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Sensor module for sensor operations. If you want to simulate your sensor it has to be inserted into this module in order to be simulated.
    /// </summary>
    class MySensorModule
    {
        public MySensorModule()
        {
            m_Sensors.Clear();
            m_SensorsToRemove.Clear();
            m_SensorsToAdd.Clear();
            m_RemoveList.Clear();
            m_ActiveSensors.Clear();
        }

        public void Destroy() 
        {
            foreach (MySensor sensor in m_Sensors) 
            {
                sensor.MarkForClose();
                ClearSensor(sensor);
            }
            m_Sensors.Clear();
            //m_Sensors = null;

            foreach (var sensorKVP in m_SensorsToAdd) 
            {
                sensorKVP.Value.MarkForClose();
                ClearSensor(sensorKVP.Value);
            }
            m_SensorsToAdd.Clear();
            //m_SensorsToAdd = null;

            //foreach (var sensorKVP in m_SensorsToRemove) 
            //{
            //    sensorKVP.Value.MarkForClose();
            //    ClearSensor(sensorKVP.Value);
            //}
            m_SensorsToRemove.Clear();
            //m_SensorsToRemove = null;

            m_ActiveSensors.Clear();
            //m_ActiveSensors = null;

            m_RemoveList.Clear();
            //m_RemoveList = null;
        }

        public bool AddSensor(MySensor sensor)
        {
            //if (sensor.Inserted)
            //{
            //    return false;
            //}

            //m_SensorsToAdd.Add(sensor);
            //return true;
            bool result = false;
            if (m_SensorsToRemove.ContainsKey(sensor.GUID))
            {
                m_SensorsToRemove.Remove(sensor.GUID);
                sensor.Active = true;
                sensor.PrepareSensorInteractions();
                result = true;
            }
            else
            {
                if (!m_SensorsToAdd.ContainsKey(sensor.GUID))
                {
                    m_SensorsToAdd.Add(sensor.GUID, sensor);
                    result = true;
                }
            }
            return result;
        }        

        public bool RemoveSensor(MySensor sensor)
        {
            //if (sensor.Inserted == false)
            //{
            //    return false;
            //}
                        
            //m_SensorsToRemove.Add(sensor);
            //return true;
            bool result = false;
            if (m_SensorsToAdd.ContainsKey(sensor.GUID))
            {                
                m_SensorsToAdd.Remove(sensor.GUID);
                result = true;
            }
            else
            {
                if (!m_SensorsToRemove.ContainsKey(sensor.GUID))
                {
                    m_SensorsToRemove.Add(sensor.GUID, sensor);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// When a sensor moves its needs to update its volume in broadphase
        /// </summary>
        public void MoveSensor(MySensor sensor, bool activateSensor)
        {
            if (sensor.Inserted)
            {
                MySensorElement el = sensor.GetElement();
                MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().MoveVolume(el);
                sensor.Active = true;                
            }
        }

        /// <summary>
        /// updates the sensor interactions (only non active sensors)
        /// </summary>
        public void PrepareSensorInteractions()
        {
            foreach(var sensor in m_Sensors)
            {
                if (!sensor.Active)
                {
                    sensor.PrepareSensorInteractions();
                }
            }
        }

        /// <summary>
        /// Checks if some interactions are not used anymore and removes them
        /// </summary>
        public void ParseSensorInteractions()
        {
            foreach (var sensor in m_Sensors)
            {
                m_RemoveList.Clear();
                foreach (var dv in sensor.m_Interactions)
                {
                    MySensorInteraction si = dv.Value;
                    if (si.m_IsInside == false)
                    {
                        // leave event 
                        sensor.GetHandler().OnLeave(si.m_SensorElement.Sensor, si.m_RBElement.GetRigidBody(), si.m_RBElement);
                        m_RemoveList.Add(si.m_Guid);                        
                        MyPhysics.physicsSystem.GetSensorInteractionModule().RemoveSensorInteraction(si);                        
                    }
                }

                foreach(var guid in m_RemoveList)
                {
                    sensor.m_Interactions.Remove(guid);
                }
            }                        
        }

        public void HandleSensorChanges() 
        {
            foreach (var sensorToRemoveKVP in m_SensorsToRemove)
            {
                MySensor sensorToRemove = sensorToRemoveKVP.Value;

                ClearSensor(sensorToRemove);

                m_Sensors.Remove(sensorToRemove);
            }
            m_SensorsToRemove.Clear();

            foreach(var sensorToAddKVP in m_SensorsToAdd)
            {
                MySensor sensorToAdd = sensorToAddKVP.Value;

                // insert to sort into bp
                MySensorElement elem = sensorToAdd.GetElement();
                elem.UpdateAABB();
                MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().CreateVolume(elem);
                sensorToAdd.Inserted = true;

                m_Sensors.Add(sensorToAdd);                
            }
            m_SensorsToAdd.Clear();            
        }

        private void ClearSensor(MySensor sensor) 
        {
            MySensorElement elem = sensor.GetElement();
            elem.UpdateAABB();
            MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().DestroyVolume(elem);
            foreach (var siKvp in sensor.m_Interactions)
            {
                MyPhysics.physicsSystem.GetSensorInteractionModule().RemoveSensorInteraction(siKvp.Value);
            }
            sensor.m_Interactions.Clear();
            sensor.Active = false;
            sensor.Inserted = false;

            if (sensor.IsMarkedForClose())
            {
                sensor.Close();
            }
        }

        public int SensorsCount() 
        {
            return m_Sensors.Count;
        }

        public List<MySensor> ActiveSensors { get { return m_ActiveSensors; } }

        private static List<MySensor> m_Sensors = new List<MySensor>(128);
        private static List<int> m_RemoveList = new List<int>(32);
        private static Dictionary<int, MySensor> m_SensorsToAdd = new Dictionary<int, MySensor>(32);
        private static Dictionary<int, MySensor> m_SensorsToRemove = new Dictionary<int, MySensor>(32);
        private static List<MySensor> m_ActiveSensors = new List<MySensor>(128);
    }
}

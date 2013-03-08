using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.App;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Entities.EntityDetector
{
    static class MyEntityDetectorsManager
    {
        private static List<MyEntityDetector> m_slowDetectors;
        private static int m_currentDetectorIndex;
        private static int m_entitiesPerFrameToUpdate;

        public static int GlobalSlowDetectorCounter = 0;

        static MyEntityDetectorsManager()
        {
            m_slowDetectors = new List<MyEntityDetector>();
            m_currentDetectorIndex = 0;

            GlobalSlowDetectorCounter = 0;
        }

        public static void RegisterSlowDetectorForUpdate(MyEntityDetector detector)
        {
            m_slowDetectors.Add(detector);
            detector.UpdateCounter = GlobalSlowDetectorCounter++;

            UpdateEntitiesPerFrame();
        }

        public static void UnregisterSlowDetectorForUpdate(MyEntityDetector detector)
        {
            m_slowDetectors.Remove(detector);
            m_currentDetectorIndex = 0;

            UpdateEntitiesPerFrame();
        }

        private static void UpdateEntitiesPerFrame()
        {
            //Update all entities in 30 frames
            m_entitiesPerFrameToUpdate = (int)Math.Ceiling(m_slowDetectors.Count / 30f);
        }

        public static void Update()
        {
            if (m_slowDetectors.Count > 0)
            {
                m_currentDetectorIndex += m_entitiesPerFrameToUpdate;
                if (m_currentDetectorIndex >= m_slowDetectors.Count)
                {
                    m_currentDetectorIndex = 0;
                }
            }
        }

        public static bool CanBeSlowDetectorUpdated(MyEntityDetector detector)
        {
            Debug.Assert(m_slowDetectors.Count > m_currentDetectorIndex);
            int index = detector.UpdateCounter % m_slowDetectors.Count;
            if (index >= m_currentDetectorIndex && index < m_currentDetectorIndex + m_entitiesPerFrameToUpdate)
                return true;

            return false;
        }
    }
}

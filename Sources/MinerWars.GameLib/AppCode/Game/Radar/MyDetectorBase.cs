using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Radar
{
    enum MyDetectorType
    {
        DefaultDetector = 0,
        EngineDetector = 1,
        MovementDetector = 2,
        OreDetector = 3,
        RadarDetector = 4,
        PulseDetector = 5,
        AllKnowingRadar = 6
    }    

    // I must remove abstract things because, detector is used in objects pool
    abstract class MyDetectorBase
    {        
        private int m_timeFromLastDetect;
        private bool m_isActive;
        private int m_detectInterval;
        private MyDetectorType m_detectorType;

        protected MySmallShip m_ship;
        
        public float Range { get; private set; }        
        public bool CanBeDisabledByRadarJammer { get; private set; }

        protected MyDetectorBase()
        {
            
        }

        public virtual void Start(MySmallShip ship, MyDetectorConfiguration configuration) 
        {
            m_ship = ship;
            m_detectInterval = configuration.DetectInterval;
            m_timeFromLastDetect = 0;
            m_isActive = false;
            m_detectorType = configuration.DetectorType;

            Range = configuration.Range;
            CanBeDisabledByRadarJammer = configuration.CanBeDisabledByRadarJammer;
        }

        public void UpdateTime(int deltaTime)
        {
            m_timeFromLastDetect += deltaTime;
            if (m_timeFromLastDetect >= m_detectInterval)
            {
                m_timeFromLastDetect = 0;
                m_isActive = true;
            }
            else
            {
                m_isActive = false;
            }
        }

        public bool IsActive()
        {
            return m_isActive;
        }

        public bool IsDetected(Vector3 radarPosition, IMyObjectToDetect objectToDetect)
        {
            //if (objectToDetect is MyFoundationFactory)
            //{
            //    return false;
            //}

            //// we don't want detect prefab containers with hagnars
            //if (objectToDetect is MyPrefabContainer && ((MyPrefabContainer)objectToDetect).ContainsPrefab(PrefabTypesFlagEnum.Hangar))
            //{
            //    return false;
            //}

            if (!CanBeDetected(objectToDetect))
            {
                return false;
            }

            BoundingSphere detectingSphere = new BoundingSphere(radarPosition, Range);
            if (!detectingSphere.Intersects(objectToDetect.WorldAABB))
            {
                return false;
            }

            return IsObjectMeetDetectCriterium(objectToDetect);
        }

        public MyDetectorType GetDetectorType() 
        {
            return m_detectorType;
        }

        protected abstract bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect);        
        protected abstract bool CanBeDetected(IMyObjectToDetect objectToDetect);
    }
}

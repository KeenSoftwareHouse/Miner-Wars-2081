using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Radar
{
    class MyRadarDetector : MyDetectorBase
    {
        public MyRadarDetector()
            : base()
        {
        }        

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {
            if (objectToDetect is MySmallShip)
            {
                MySmallShip smallShip = objectToDetect as MySmallShip;
                return smallShip.Radar != null;
            }            
            if(objectToDetect is MyPrefabBase)
            {
                MyPrefabBase prefab = objectToDetect as MyPrefabBase;
                return prefab.GetOwner().ContainsPrefab(PrefabTypesFlagEnum.LargeShip);
            }
            return false;
        }

        protected override bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            return objectToDetect is MySmallShip || 
                   objectToDetect is MyPrefabFoundationFactory || 
                   objectToDetect is MyPrefabHangar;
        }
    }
}

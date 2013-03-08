using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Radar
{
    class MyMovementDetector : MyDetectorBase
    {
        public MyMovementDetector()
            : base()
        {
        }        

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {            
            MyEntity entity = objectToDetect as MyEntity;            
            if(entity.Physics == null)
            {
                return false;
            }
            return entity.Physics.Speed > MyHudConstants.RADAR_MOVEMENT_DETECTOR_MIN_SPEED_TO_DETECT;
        }

        protected override bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            //return objectToDetect is MyEntity &&
            //    !(objectToDetect is MySmallDebris || objectToDetect is MyExplosionDebrisBase || objectToDetect is MyLargeDebrisField) &&
            //    !(objectToDetect is MyPrefabBase);
            return objectToDetect is MyShip || objectToDetect is MyAmmoBase;
        }
    }
}

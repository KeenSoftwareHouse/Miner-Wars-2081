using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Radar
{
    class MyPulseDetector : MyDetectorBase
    {
        public MyPulseDetector()
            : base()
        {
        }        

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {            
            // entity movement, prefabcontainer or ship detect
            if(objectToDetect is MyEntity)
            {
                MyEntity entity = objectToDetect as MyEntity;

                if (entity.Physics != null && entity.Physics.Speed > MyHudConstants.RADAR_MOVEMENT_DETECTOR_MIN_SPEED_TO_DETECT)
                {
                    return true;
                }
                else
                {
                    if(entity is MyShip || entity is MyPrefabBase)
                    {
                        return true;                    
                    }                    
                }                
            }
            // ore detect
            else if (objectToDetect is MyVoxelMapOreDepositCell)
            {
                return true;
            }
            return false;
        }

        protected override bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            //return objectToDetect is MyEntity || objectToDetect is MyMwcVoxelMaterialsEnum;
            return objectToDetect is MyShip ||
                   objectToDetect is MyPrefabFoundationFactory ||
                   objectToDetect is MyPrefabHangar || 
                   objectToDetect is MyAmmoBase ||
                   objectToDetect is MyVoxelMapOreDepositCell;
        }
    }
}

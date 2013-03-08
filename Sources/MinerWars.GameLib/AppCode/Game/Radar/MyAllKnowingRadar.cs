using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Radar
{
    class MyAllKnowingRadar : MyDetectorBase
    {
        public MyAllKnowingRadar()
            : base()
        {
        }

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {            
            return true;
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

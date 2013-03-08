using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Radar
{
    class MyOreDetector : MyDetectorBase
    {
        private MyMwcVoxelMaterialsEnum m_oreMaterial;

        public MyOreDetector()
            : base()
        {
            
        }

        public override void Start(MySmallShip ship, MyDetectorConfiguration configuration) 
        {
            base.Start(ship, configuration);
            m_oreMaterial = (MyMwcVoxelMaterialsEnum)configuration.Parameters[0];
        }        

        public MyMwcVoxelMaterialsEnum OreMaterial
        {
            get
            {
                return m_oreMaterial;
            }
        }

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {
            MyVoxelMapOreDepositCell oreDeposit = (MyVoxelMapOreDepositCell)objectToDetect;
            return oreDeposit.GetOreContent(m_oreMaterial) > 0;
        }

        protected override bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            return objectToDetect is MyVoxelMapOreDepositCell;
        }        
    }
}

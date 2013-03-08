using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;

namespace MinerWars.AppCode.Game.Radar
{
    class MyDefaultDetector : MyDetectorBase
    {
        public MyDefaultDetector() 
            : base()
        {
        }        

        protected override bool IsObjectMeetDetectCriterium(IMyObjectToDetect objectToDetect)
        {            
            MyEntity entity = objectToDetect as MyEntity;            
            // TODO: temporary disable displaying mission entities on radar
            /*// we detect all mission entities
            if(MyMissions.IsMissionEntity(entity))
            {
                return true;
            }
            // we detect all friendly ship, foundation factories and hangars
            else */if(entity is MyShip || entity is MyPrefabFoundationFactory || entity is MyPrefabHangar)
            {
                return MyFactions.GetFactionsRelation(m_ship, entity) == MyFactionRelationEnum.Friend;
            }
            return false;
        }

        protected override bool CanBeDetected(IMyObjectToDetect objectToDetect)
        {
            //return objectToDetect is MyShip || objectToDetect is MyPrefabContainer;
            return objectToDetect is MyEntity;
        }
    }
}

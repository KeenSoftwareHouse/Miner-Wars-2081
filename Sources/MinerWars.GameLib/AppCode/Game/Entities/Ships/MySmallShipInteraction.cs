using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Entities.Ships;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.AppCode.Game.Entities
{
    [Flags]
    internal enum MySmallShipInteractionActionEnum
    {
        TradeForFree = 1 << 0,
        TradeForMoney = 1 << 1,
        Loot = 1 << 2,
        Build = 1 << 3,
        Travel = 1 << 4,
        Use = 1 << 5,
        Hack = 1 << 6,
        Examine = 1 << 7,
        ExamineEmpty = 1 << 8,
        Blocked = 1 << 9,
    }

    static class MySmallShipInteraction
    {
        public static bool MUST_LOOK_AT_TARGET = true;

        private static bool IsLookAtTarget(MySmallShip smallShip, MyEntity otherEntity) 
        {
            if (!MUST_LOOK_AT_TARGET || MySession.Is25DSector) 
            {
                return true;
            }
            float length = Vector3.Distance(smallShip.WorldVolume.Center, otherEntity.WorldVolume.Center);
            MyLine line = new MyLine(smallShip.WorldVolume.Center, smallShip.WorldVolume.Center + smallShip.WorldMatrix.Forward * length * 1.5f, true);
            MyIntersectionResultLineTriangleEx? result = MyEntities.GetIntersectionWithLine(ref line, smallShip, null, ignoreChilds: true);
            return result != null && result.Value.Entity.GetBaseEntity() == otherEntity;                       
        }

        private static bool IsCustomLookAtTarget(MySmallShip smallShip, MyEntity otherEntity, float radius)
        {
            if (!MUST_LOOK_AT_TARGET)
            {
                return true;
            }

            BoundingSphere sphere = new BoundingSphere(otherEntity.WorldVolume.Center, radius);
            Ray ray = new Ray(smallShip.GetPosition(), smallShip.WorldMatrix.Forward);
            float? rayIntersection = ray.Intersects(sphere);
            if (rayIntersection.HasValue)
            {
                MyLine line = new MyLine(smallShip.WorldVolume.Center, otherEntity.WorldVolume.Center, true);
                MyIntersectionResultLineTriangleEx? result = MyEntities.GetIntersectionWithLine(ref line, smallShip, null, ignoreChilds: true);
                return !result.HasValue || !result.HasValue || result.Value.Entity == otherEntity;
            }
            return false;
        }

        private static bool IsTargetVisible(MySmallShip smallShip, MyEntity otherEntity)
        {
            MyLine line = new MyLine(smallShip.WorldVolume.Center, otherEntity.GetPosition(), true);
            MyIntersectionResultLineTriangleEx? result = MyEntities.GetIntersectionWithLine(ref line, smallShip, null, ignoreChilds: true);
            return result != null && result.Value.Entity.GetBaseEntity() == otherEntity;
        }

        private static bool ControlIsInPlayerShip()
        {
            return MyGuiScreenGamePlay.Static.IsControlledPlayerShip;
        }

        public static bool CanTradeForFree(MyEntity entityToTrade, params object[] args)
        {
            if (!ControlIsInPlayerShip() || IsBlocked(entityToTrade, args))
            {
                return false;
            }                        
            
            // temporary disabled trading for free with smallships
            bool hasRightType = /*(entityToTrade is MySmallShip) || */(entityToTrade is MyPrefabHangar || entityToTrade is MyDrone);
            if (!hasRightType) 
            {
                return false;
            }

            MySmallShip smallShip = GetSmallShipFromArguments(args);
            MyFactionRelationEnum factionRelation = MyFactions.GetFactionsRelation(entityToTrade, smallShip);            
            if (entityToTrade is MyPrefabHangar)
            {
                MyPrefabHangar prefabHangar = entityToTrade as MyPrefabHangar;
                return prefabHangar.PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR &&
                       factionRelation == MyFactionRelationEnum.Friend &&
                       prefabHangar.IsWorking() && IsTargetVisible(smallShip, prefabHangar);
            }
            else if(entityToTrade is MyDrone)
            {
                return factionRelation == MyFactionRelationEnum.Friend &&
                    !IsShipLootable(entityToTrade as MySmallShip) &&
                    IsCustomLookAtTarget(smallShip, entityToTrade, entityToTrade.WorldVolume.Radius * 1.0f);
            }
            else 
            {
                return factionRelation == MyFactionRelationEnum.Friend &&
                    !IsShipLootable(entityToTrade as MySmallShip) && 
                    IsLookAtTarget(smallShip, entityToTrade);
            }
        }        

        public static bool CanTrade(MyEntity entityToTrade, params object[] args)
        {
            if (!ControlIsInPlayerShip() || IsBlocked(entityToTrade, args))
            {
                return false;
            }

            bool hasRightType = /*(entityToTrade is MySmallShip) || */(entityToTrade is MyPrefabHangar);
            if (!hasRightType)
            {
                return false;
            }
            
            MySmallShip smallShip = GetSmallShipFromArguments(args);
            MyFactionRelationEnum factionRelation = MyFactions.GetFactionsRelation(entityToTrade, smallShip);            

            if (entityToTrade is MyPrefabHangar)
            {
                MyPrefabHangar prefabHangar = entityToTrade as MyPrefabHangar;
                return (factionRelation == MyFactionRelationEnum.Neutral || factionRelation == MyFactionRelationEnum.Friend || prefabHangar.UseProperties.IsHacked) &&
                       prefabHangar.IsWorking() && prefabHangar.PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR && IsTargetVisible(smallShip, prefabHangar);
            }
            else
            {
                Debug.Fail("This shouldn't happen! You can't trade with bot");
                return factionRelation == MyFactionRelationEnum.Neutral &&
                    !IsShipLootable(entityToTrade as MySmallShip) &&
                    IsLookAtTarget(smallShip, entityToTrade);
            }
        }        

        private static bool IsShipLootable(MySmallShip smallShip) 
        {
            bool isParked = (smallShip is MySmallShipBot && ((MySmallShipBot)smallShip).IsParked());

            return smallShip.IsPilotDead() || isParked && smallShip.Enabled;
        }

        public static bool CanLootShip(MySmallShip shipToLoot, params object[] args)
        {
            if (!ControlIsInPlayerShip() || IsBlocked(shipToLoot, args))
            {
                return false;
            }            

            MySmallShip smallShip = GetSmallShipFromArguments(args);
            //bool hasRightFactionRelation = MyFactions.GetFactionsRelation(smallShip, shipToLoot) == MyFactionRelationEnum.Enemy;            
            //return hasRightFactionRelation && IsLookAtTarget(smallShip, shipToLoot);            
            return IsShipLootable(shipToLoot) && IsLookAtTarget(smallShip, shipToLoot);            
        }

        public static bool CanExamineCargoBox(MyCargoBox cargoBox, params object[] args)
        {
            if (IsBlocked(cargoBox, args))
            {
                return false;
            }

            MySmallShip smallShip = GetSmallShipFromArguments(args);
            return cargoBox.Inventory.GetInventoryItems().Count > 0 && IsLookAtTarget(smallShip, cargoBox) && ControlIsInPlayerShip();
        }

        public static bool CanExamineEmptyCargoBox(MyCargoBox cargoBox, params object[] args)
        {
            if (IsBlocked(cargoBox, args))
            {
                return false;
            }

            MySmallShip smallShip = GetSmallShipFromArguments(args);
            return cargoBox.Inventory.GetInventoryItems().Count == 0 && IsLookAtTarget(smallShip, cargoBox) && ControlIsInPlayerShip();
        }

        public static bool IsBlocked(MyEntity entity, params object[] args)
        {
            return MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsLockedByOtherPlayer(entity);
        }

        public static bool CanBuild(MyPrefabContainer prefabContainer, params object[] args)
        {
            MySmallShip smallShip = GetSmallShipFromArguments(args);

            return (MyFactions.GetFactionsRelation(smallShip, prefabContainer) == MyFactionRelationEnum.Friend) &&
                   prefabContainer.ContainsPrefab(PrefabTypesFlagEnum.FoundationFactory) &&
                    ControlIsInPlayerShip();
        }

        public static bool IsNearMothership(MyPrefabHangar prefabHangar, params object[] args)
        {
            MySmallShip smallShip = GetSmallShipFromArguments(args);

            return (prefabHangar.PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR && 
                   MyFactions.GetFactionsRelation(smallShip, prefabHangar) == MyFactionRelationEnum.Friend) &&
                   IsTargetVisible(smallShip, prefabHangar) && 
                   ControlIsInPlayerShip();
        }

        public static bool CanUse(MyEntity entity, params object[] args) 
        {
            IMyUseableEntity useableEntity = entity as IMyUseableEntity;

            MySmallShip smallShip = GetSmallShipFromArguments(args);

            if (useableEntity == null) 
            {
                return false;
            }
            if((useableEntity.UseProperties.UseType & MyUseType.Solo) == 0)
            {
                return false;
            }
            if (entity is MyDrone ? !IsCustomLookAtTarget(smallShip, entity, entity.WorldVolume.Radius) : !IsLookAtTarget(smallShip, entity)) 
            {
                return false;
            }
            if (smallShip != MyGuiScreenGamePlay.Static.ControlledEntity)
            {
                return false;
            }

            return useableEntity.CanBeUsed(smallShip);
        }

        public static bool CanHack(MyEntity entity, params object[] args) 
        {
            IMyUseableEntity useableEntity = entity as IMyUseableEntity;

            MySmallShip smallShip = GetSmallShipFromArguments(args);
            
            if (useableEntity == null)
            {
                return false;
            }
            if (useableEntity.UseProperties.IsHacked)
            {
                return false;
            }
            if ((useableEntity.UseProperties.HackType & MyUseType.Solo) == 0)
            {
                return false;
            }
            if (!IsLookAtTarget(smallShip, entity))
            {
                return false;
            }
            if (smallShip != MyGuiScreenGamePlay.Static.ControlledEntity)
            {
                return false;
            }

            return useableEntity.CanBeHacked(smallShip);
        }

        private static MySmallShip GetSmallShipFromArguments(object[] args) 
        {
            MySmallShip smallShip = args[0] as MySmallShip;
            Debug.Assert(smallShip != null);
            return smallShip;
        }
    }
}

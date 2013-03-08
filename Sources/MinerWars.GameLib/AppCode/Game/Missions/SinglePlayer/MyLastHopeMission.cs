using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.World.Global;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyLastHopeMission : MyMission
    {
        private const int StopSlaversObjectiveTime = 150000;
        private int m_repairedCounter;

        private readonly MyMultipleUseObjective m_repairPipes;
        private readonly MyObjective m_stopSlaverTransport;
        private readonly MyObjective m_findTunnel;
        private readonly MyObjective m_catchSlaverRiders;
        private readonly MyDestroyWavesObjective m_killSaboteurSquad;
        private readonly MyMultipleUseObjective m_deactivateBombs;

        private MyEntity m_repairPipesDummyShoot;

        private readonly MyMovingEntity m_movingShip1;
        private readonly MyMovingEntity m_movingShip2;
        private readonly MyMovingEntity m_movingShip1Particle;
        private readonly MyMovingEntity m_movingShip2Particle;
        
        

        #region EntityIDs
        private readonly Dictionary<uint, Tuple<List<uint>, uint>> m_gasPipes = new Dictionary<uint, Tuple<List<uint>, uint>>()
                                                                           {
                                                                               {16779440,new Tuple<List<uint>, uint>(new List<uint>(){16778002,16778003},16778629 )},
                                                                               {16779438,new Tuple<List<uint>, uint>(new List<uint>(){16777997,16777998},16778628 )},
                                                                               {16779442,new Tuple<List<uint>, uint>(new List<uint>(){16777995,16778000,16777994,16777999,16777996,16778001},16778630 )},                   
                                                                           };

        private readonly Dictionary<uint, Tuple<List<uint>, uint>> m_nuclearCores = new Dictionary<uint, Tuple<List<uint>, uint>>()
                                                                           {
                                                                               {16779444,new Tuple<List<uint>, uint>(new List<uint>(){16777712,16777708},16778034 )},
                                                                               {16779448,new Tuple<List<uint>, uint>(new List<uint>(){16777710,16777711},16778035 )},
                                                                               {16779446,new Tuple<List<uint>, uint>(new List<uint>(){16777709,16777713},16778033 )},
                                                                           };
        
        private readonly List<uint> m_cargoBoxes = new List<uint>() { 16781486, 16781485 };
        private readonly List<uint> m_cargoBoxesHide = new List<uint>() { 16781480, 16781463, 16781481 };

        private enum EntityId
        {
            //Objective 01
            ReachColonyDummy = 2468,
            ReachColonyDialogDetector = 16782668,
            ReachColonySpawn1 = 16781269,
            //Objective 02
            
            DestroySlaverRidersShip1 = 22640,
            DestroySlaverRidersShip2 = 16780300,

            DestroySlaverRidersGenerator1 = 16779905,
            DestroySlaverRidersGenerator2 = 16780305,
            DestroySlaverRidersDetector1 = 16781327,

            DestroySlaverRidersDetector3 = 16781329,

            DestroySlaverRidersSpawn11 = 16781268,
            DestroySlaverRidersSpawn21 = 16779838,
            DestroySlaverRidersSpawn31 = 16781274,
            DestroySlaverRidersSpawn41 = 16781273,
            DestroySlaverRidersSpawn12 = 16779836,
            DestroySlaverRidersSpawn22 = 533,
            DestroySlaverRidersSpawn32 = 16781272,
            DestroySlaverRidersSpawn42 = 16781271,
            //Objective 03
            CatchSlaverRidersDummy = 2469,
            CatchSlaverRidersDetector1 = 16781321,
            CatchSlaverRidersDetector2 = 16781323,
            CatchSlaverRidersSpawn1 = 16781276,
            CatchSlaverRidersSpawn21 = 16781277,
            CatchSlaverRidersSpawn22 = 16781278,
            //Objective 04
            StopSlaverTransportShip1 = 16780927,
            StopSlaverTransportShip1Target = 16781266,
            StopSlaverTransportShip2 = 16781064,
            StopSlaverTransportShip2Target = 16781201,
            StopSlaverTransportShip1Particle = 16781260,
            StopSlaverTransportShip1TargetParticle = 16781265,
            StopSlaverTransportShip2Particle = 16781256,
            StopSlaverTransportShip2TargetParticle = 16781264,
            StopSlaverTransportGenerator1 = 16781008,
            StopSlaverTransportGenerator2 = 16781145,
            StopSlaverTransportSpawn11 = 16781280,
            StopSlaverTransportSpawn12 = 16781281,
            StopSlaverTransportSpawn21 = 16781283,
            StopSlaverTransportSpawn22 = 16781282,
            //Objective 05
            ReachCavesLocation = 25345,
            //Objective 06
            FindTunnelDummy = 25347,
            FindTunnelSpawn1 = 16781284,
            FindTunnelSpawn2 = 16781285,
            FindTunnelSpawn3 = 16781286,
            //Objective 07
            KillSaboteursSpawn = 16781287,
            //Objective 08
            DeactivateBombPrefab1 = 16777992,
            DeactivateBombPrefab2 = 16777993,
            //Objective 09
            StabilizeGasPipesShootDetector = 16788549,
            StabilizeGasPipesShootparticle = 16788553,
            //Objective 10
            //Spawn1 = 25341,
            //Spawn2 = 25343,

            LaveUndergroundDummy = 51,
            KillSabotersDetector = 25351,
            GasCloudsDetector = 45,
            StabilizeCoreDialog1Dummy = 47,
            StabilizeCoreDialog2Dummy = 49,

        }


        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            if (!IsMainSector) return;
            foreach (var value in Enum.GetValues(typeof(EntityId)))
            {
                MyScriptWrapper.GetEntity((uint)(((EntityId?)value).Value));
            }

            var list = new List<uint>();
            list.AddRange(m_cargoBoxes);
            list.AddRange(m_cargoBoxesHide);

            foreach (KeyValuePair<uint, Tuple<List<uint>, uint>> keyValuePair in m_gasPipes)
            {
                list.Add(keyValuePair.Key);
                list.AddRange(keyValuePair.Value.Item1);
                list.Add(keyValuePair.Value.Item2);
            }

            foreach (KeyValuePair<uint, Tuple<List<uint>, uint>> keyValuePair in m_nuclearCores)
            {
                list.Add(keyValuePair.Key);
                list.AddRange(keyValuePair.Value.Item1);
                list.Add(keyValuePair.Value.Item2);
            }

            foreach (var value in list)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }
        #endregion

        public MyLastHopeMission()
        {
            ID = MyMissionID.LAST_HOPE;
            Name = MyTextsWrapperEnum.LAST_HOPE;
            Description = MyTextsWrapperEnum.LAST_HOPE_Description;
            DebugName = new StringBuilder("07-Last Hope");
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-190694, 0, -18204363);
            Location = new MyMissionLocation(baseSector, (uint)EntityId.ReachColonyDummy);

            RequiredMissions = new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.LAST_HOPE_LEAVE_UNDERGROUND };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            #region Objectives

            m_objectives = new List<MyObjective>();

            var reachColony = new MyObjective(
                MyTextsWrapperEnum.LAST_HOPE_REACH_COLONY,
                MyMissionID.LAST_HOPE_REACH_COLONY,
                MyTextsWrapperEnum.LAST_HOPE_REACH_COLONY_DESCRIPTION,
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityId.ReachColonyDummy),
                startDialogId: MyDialogueEnum.LAST_HOPE_0100
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudLastHope };
            reachColony.Components.Add(new MySpawnpointWaves((uint)EntityId.ReachColonyDialogDetector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.ReachColonySpawn1}
                                                                                                        }));
            var dialogComponent = new MyDetectorDialogue((uint)EntityId.ReachColonyDialogDetector, MyDialogueEnum.LAST_HOPE_0200);
            dialogComponent.OnDialogStarted += ReachColonyDialogComonentOnOnDialogStarted;
            dialogComponent.OnDialogFinished += ReachColonyDialogComonentOnOnDialogFinished;
            reachColony.Components.Add(dialogComponent);
            m_objectives.Add(reachColony);




            var destroySlaverRiders = new MyObjectiveDestroy(
              MyTextsWrapperEnum.LAST_HOPE_REACH_DESTROY_RIDERS,
              MyMissionID.LAST_HOPE_DESTROY_SLAVER_RIDERS,
              MyTextsWrapperEnum.LAST_HOPE_REACH_DESTROY_RIDERS_DESC,
              null,
              this,
              new MyMissionID[] { MyMissionID.LAST_HOPE_REACH_COLONY },
              new List<uint> { (uint)EntityId.DestroySlaverRidersGenerator1, (uint)EntityId.DestroySlaverRidersGenerator2 }
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.LAST_HOPE_0300, HudName = MyTextsWrapperEnum.Nothing };
            destroySlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.DestroySlaverRidersDetector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn11},
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn12}
                                                                                                        }));
            destroySlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.DestroySlaverRidersDetector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn21},
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn22}
                                                                                                        }));
            destroySlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.DestroySlaverRidersDetector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn31},
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn32}
                                                                                                        }));
            destroySlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.DestroySlaverRidersDetector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn41},
                                                                                                             new uint[]{(uint)EntityId.DestroySlaverRidersSpawn42}
                                                                                                        }));
            destroySlaverRiders.OnMissionLoaded += DestroySlaverRiders_Loaded;
            m_objectives.Add(destroySlaverRiders);


            m_catchSlaverRiders = new MyObjective(
                MyTextsWrapperEnum.LAST_HOPE_REACH_CATCH_SHIPS,
                MyMissionID.LAST_HOPE_CATCH_SLAVER_RIDERS,
                MyTextsWrapperEnum.LAST_HOPE_REACH_CATCH_SHIPS_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_DESTROY_SLAVER_RIDERS, },
                new MyMissionLocation(baseSector, (uint)EntityId.CatchSlaverRidersDummy)
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.LAST_HOPE_0400 };
            m_catchSlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.CatchSlaverRidersDetector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.CatchSlaverRidersSpawn1},
                                                                                                        }));
            m_catchSlaverRiders.Components.Add(new MySpawnpointWaves((uint)EntityId.CatchSlaverRidersDetector2, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.CatchSlaverRidersSpawn21},
                                                                                                             new uint[]{(uint)EntityId.CatchSlaverRidersSpawn22}
                                                                                                        }));
            m_catchSlaverRiders.OnMissionLoaded += CatchSlaverRiders_Loaded;
            m_objectives.Add(m_catchSlaverRiders);



            m_stopSlaverTransport = new MyObjectiveDestroy(
                MyTextsWrapperEnum.LAST_HOPE_REACH_STOP_SHIPS,
                MyMissionID.LAST_HOPE_STOP_SLAVER_RIDERS,
                MyTextsWrapperEnum.LAST_HOPE_REACH_STOP_SHIPS_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_CATCH_SLAVER_RIDERS },
                new List<uint>() { (uint)EntityId.StopSlaverTransportGenerator1, (uint)EntityId.StopSlaverTransportGenerator2 }
            ) { SaveOnSuccess = false, StartDialogId = MyDialogueEnum.LAST_HOPE_0500 };
            m_movingShip1 = new MyMovingEntity((uint)EntityId.StopSlaverTransportShip1,
                                   (uint)EntityId.StopSlaverTransportShip1Target, StopSlaversObjectiveTime, false);

            m_movingShip2 = new MyMovingEntity((uint)EntityId.StopSlaverTransportShip2,
                                   (uint)EntityId.StopSlaverTransportShip2Target, StopSlaversObjectiveTime, false);

            m_movingShip1Particle = new MyMovingEntity((uint)EntityId.StopSlaverTransportShip1Particle,
                                   (uint)EntityId.StopSlaverTransportShip1TargetParticle, StopSlaversObjectiveTime, false);

            m_movingShip2Particle = new MyMovingEntity((uint)EntityId.StopSlaverTransportShip2Particle,
                                   (uint)EntityId.StopSlaverTransportShip2TargetParticle, StopSlaversObjectiveTime, false);

            m_stopSlaverTransport.Components.Add(m_movingShip1);
            m_stopSlaverTransport.Components.Add(m_movingShip2);
            m_stopSlaverTransport.Components.Add(m_movingShip1Particle);
            m_stopSlaverTransport.Components.Add(m_movingShip2Particle);
            m_stopSlaverTransport.OnMissionLoaded += StopSlaverTransport_Loaded;

            m_stopSlaverTransport.Components.Add(new MySpawnpointWaves((uint)EntityId.CatchSlaverRidersDummy, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.StopSlaverTransportSpawn11,(uint)EntityId.StopSlaverTransportSpawn21,},
                                                                                                             new uint[]{(uint)EntityId.StopSlaverTransportSpawn12,(uint)EntityId.StopSlaverTransportSpawn22,},
                                                                                                        }));
            m_objectives.Add(m_stopSlaverTransport);


            var speakWithFathetTobias = new MyObjectiveDialog
                (
                MyTextsWrapperEnum.LAST_HOPE_SPEAK_WITH_FATHER,
                MyMissionID.LAST_HOPE_SPEAK_WITH_FATHER,
                MyTextsWrapperEnum.LAST_HOPE_SPEAK_WITH_FATHER_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_STOP_SLAVER_RIDERS },
                MyDialogueEnum.LAST_HOPE_0600
                ) { SaveOnSuccess = true };
            m_objectives.Add(speakWithFathetTobias);


            var reachCaves = new MyObjective(
                MyTextsWrapperEnum.LAST_HOPE_REACH_REACH_CAVE,
                MyMissionID.LAST_HOPE_REACH_UNDEGROUND_CAVES,
                MyTextsWrapperEnum.LAST_HOPE_REACH_REACH_CAVE_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_SPEAK_WITH_FATHER },
                new MyMissionLocation(baseSector, (uint)EntityId.ReachCavesLocation)

            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.LAST_HOPE_0700, HudName = MyTextsWrapperEnum.HudCave };
            m_objectives.Add(reachCaves);


            m_findTunnel = new MyObjective(
                    MyTextsWrapperEnum.LAST_HOPE_REACH_TUNNEL,
                    MyMissionID.LAST_HOPE_FIND_MAINTANCE_TUNELL,
                    MyTextsWrapperEnum.LAST_HOPE_REACH_TUNNEL_DESC,
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.LAST_HOPE_REACH_UNDEGROUND_CAVES },
                    new MyMissionLocation(baseSector, (uint)EntityId.FindTunnelDummy)
                    ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.LAST_HOPE_0800, HudName = MyTextsWrapperEnum.HudTunnel };
            m_findTunnel.Components.Add(new MySpawnpointWaves((uint)EntityId.ReachCavesLocation, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityId.FindTunnelSpawn1},
                                                                                                             new uint[]{(uint)EntityId.FindTunnelSpawn2},
                                                                                                             new uint[]{(uint)EntityId.FindTunnelSpawn3},
                                                                                                        }));
            m_objectives.Add(m_findTunnel);


            var killSaboteurSquad = new MyDestroyWavesObjective
                (
                MyTextsWrapperEnum.LAST_HOPE_KILL_SABOTERS,
                MyMissionID.LAST_HOPE_KILL_SQUAD,
                MyTextsWrapperEnum.LAST_HOPE_KILL_SABOTERS,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_FIND_MAINTANCE_TUNELL }
                ) { SaveOnSuccess = true };
            killSaboteurSquad.AddWave(new List<uint>() { (uint)EntityId.KillSaboteursSpawn });
            killSaboteurSquad.Components.Add(new MyDetectorDialogue((uint)EntityId.KillSabotersDetector, MyDialogueEnum.LAST_HOPE_1100));
            m_objectives.Add(killSaboteurSquad);



            var deactivateBombs = new MyMultipleUseObjective
                (MyTextsWrapperEnum.LAST_HOPE_DEACTIVATE_BOMBS,
                 MyMissionID.LAST_HOPE_DEACTIVATE_BOMB,
                 MyTextsWrapperEnum.LAST_HOPE_DEACTIVATE_BOMBS_DESC,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.LAST_HOPE_KILL_SQUAD },
                 MyTextsWrapperEnum.PressToDeactivateNuclearHead,
                 MyTextsWrapperEnum.NuclearHead,
                 MyTextsWrapperEnum.DeactivatingInProgress,
                 5000,
                 new List<uint>() { (uint)EntityId.DeactivateBombPrefab1, (uint)EntityId.DeactivateBombPrefab2 },
                 MyUseObjectiveType.Taking

                ) { RadiusOverride = 30, SaveOnSuccess = true, StartDialogId = MyDialogueEnum.LAST_HOPE_1200 };
            deactivateBombs.OnMissionLoaded += DeactivateBombs_Loaded;
            deactivateBombs.OnObjectUsedSucces += DeactivateBombs_ObjectUsedSuccess;
            deactivateBombs.Components.Add(new MyDetectorDialogue((uint)EntityId.GasCloudsDetector, MyDialogueEnum.LAST_HOPE_1300));
            m_objectives.Add(deactivateBombs);



            m_repairPipes = new MyMultipleUseObjective
                (MyTextsWrapperEnum.LAST_HOPE_STABILIZE_GAS,
                 MyMissionID.LAST_HOPE_REPAIR_PIPES,
                  MyTextsWrapperEnum.LAST_HOPE_STABILIZE_GAS_DESC,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.LAST_HOPE_DEACTIVATE_BOMB },
                 MyTextsWrapperEnum.PressToRepairGasPipe,
                 MyTextsWrapperEnum.GasPipe,
                 MyTextsWrapperEnum.ProgressRepairing,
                 5000,
                 new List<uint>() { },
                 MyUseObjectiveType.Repairing
                ) { SaveOnSuccess = true, RadiusOverride = 30, StartDialogId = MyDialogueEnum.LAST_HOPE_1400, SuccessDialogId = MyDialogueEnum.LAST_HOPE_1600 };
            foreach (var gasPipe in m_gasPipes)
            {
                m_repairPipes.MissionEntityIDs.Add(gasPipe.Key);
            }
            m_repairPipes.OnObjectUsedSucces += RepairPipes_ObjectUsedSuccess;
            m_objectives.Add(m_repairPipes);



            var stabilizeNuclearCore = new MyMultipleUseObjective
                (
                MyTextsWrapperEnum.LAST_HOPE_STABILIZE_NUCLEAR_CORE,
                MyMissionID.LAST_HOPE_STABILIZE_NUCLEAR_CORE,
                MyTextsWrapperEnum.LAST_HOPE_STABILIZE_GAS_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_REPAIR_PIPES },
                MyTextsWrapperEnum.PressToRepairRadiation,
                MyTextsWrapperEnum.RadiationLeak,
                MyTextsWrapperEnum.Fixing,
                5000,
                new List<uint>() { },
                MyUseObjectiveType.Repairing
                ) { RadiusOverride = 40, SaveOnSuccess = true };
            stabilizeNuclearCore.Components.Add(new MyDetectorDialogue((uint)EntityId.StabilizeCoreDialog1Dummy, MyDialogueEnum.LAST_HOPE_1700));
            stabilizeNuclearCore.Components.Add(new MyDetectorDialogue((uint)EntityId.StabilizeCoreDialog2Dummy, MyDialogueEnum.LAST_HOPE_1800));
            foreach (var core in m_nuclearCores)
            {
                stabilizeNuclearCore.MissionEntityIDs.Add(core.Key);
            }
            stabilizeNuclearCore.OnObjectUsedSucces += StabilizeNuclearCore_ObjectUsedSuccess;
            m_objectives.Add(stabilizeNuclearCore);


            var leaveUnderground = new MyObjective
                (
                MyTextsWrapperEnum.LAST_HOPE_LEAVE_SHAFTS,
                MyMissionID.LAST_HOPE_LEAVE_UNDERGROUND,
                MyTextsWrapperEnum.LAST_HOPE_LEAVE_SHAFTS_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAST_HOPE_STABILIZE_NUCLEAR_CORE },
                new MyMissionLocation(baseSector, (uint)EntityId.LaveUndergroundDummy)) { StartDialogId = MyDialogueEnum.LAST_HOPE_1900, HudName = MyTextsWrapperEnum.Nothing };
            leaveUnderground.OnMissionSuccess += LeaveUnderground_Success;
            m_objectives.Add(leaveUnderground);

            #endregion
        }

        void LeaveUnderground_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAST_HOPE_2000);
        }

        private void DestroySlaverRiders_Loaded(MyMissionBase sender)
        {
            
        }


        public override void Success()
        {
            base.Success();

        }

        private void StabilizeNuclearCore_ObjectUsedSuccess(uint entityId)
        {
            MyScriptWrapper.TryUnhideEntities(m_nuclearCores[entityId].Item1);
            MyScriptWrapper.SetEntityEnabled(m_nuclearCores[entityId].Item2, false);
        }


        private void RepairPipes_ObjectUsedSuccess(uint entityId)
        {
            MyScriptWrapper.TryUnhideEntities(m_gasPipes[entityId].Item1);
            MyScriptWrapper.SetEntityEnabled(m_gasPipes[entityId].Item2, false);

            m_repairedCounter++;

            if (m_repairedCounter == 2)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAST_HOPE_1500);
            }
        }

        private void DeactivateBombs_ObjectUsedSuccess(uint entityId)
        {
            MyScriptWrapper.Highlight(entityId, false, this);
            MyScriptWrapper.TryHide(entityId);
        }

        private void DeactivateBombs_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityId.DeactivateBombPrefab1, true, this);
            MyScriptWrapper.Highlight((uint)EntityId.DeactivateBombPrefab2, true, this);
        }


        private void StopSlaverTransport_Loaded(MyMissionBase sender)
        {
            sender.MissionTimer.RegisterTimerAction(StopSlaversObjectiveTime, () => Fail(MyTextsWrapperEnum.Fail_TimeIsUp), true, "");
        }

        private void ReachColonyDialogComonentOnOnDialogStarted()
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, category:"KA07", priority:100);
        }

        private void ReachColonyDialogComonentOnOnDialogFinished()
        {
            //MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Omnicorp, MyFactions.RELATION_WORST);
        }

        private void CatchSlaverRiders_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(100);
        }

        void Script_EntityDeath(MyEntity entityDeath, MyEntity killedBy)
        {
            if (entityDeath.EntityId == null)
                return;

            if (entityDeath.EntityId.Value.NumericValue == (uint)EntityId.StopSlaverTransportGenerator1)
            {
                m_movingShip1.StopShip();
                m_movingShip1Particle.StopShip();
                /*
                m_stopSlaverTransport.RemoveComponent(m_movingShip1);
                m_stopSlaverTransport.RemoveComponent(m_movingShip1Particle);
                 */
            }

            if (entityDeath.EntityId.Value.NumericValue == (uint)EntityId.StopSlaverTransportGenerator2)
            {
                m_movingShip2.StopShip();
                m_movingShip2Particle.StopShip();
                /*
                m_stopSlaverTransport.RemoveComponent(m_movingShip2);
                m_stopSlaverTransport.RemoveComponent(m_movingShip2Particle);
                 */
            }
        }




        public override void Accept()
        {
            base.Accept();

            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, 5000f);
        }

        public override void Load()
        {
            base.Load();
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityId.DestroySlaverRidersShip1), -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityId.DestroySlaverRidersShip2), -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityId.StopSlaverTransportShip1), -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityId.StopSlaverTransportShip2), -1);

            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((int)EntityId.StopSlaverTransportShip1), false);
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((int)EntityId.StopSlaverTransportShip2), false);

                
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Slavers, MyFactions.RELATION_WORST);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Pirates, MyFactions.RELATION_BEST);


            MyScriptWrapper.EntityDeath += Script_EntityDeath;
            MyScriptWrapper.OnEntityAtacked += Script_EntityAtacked;
            MyScriptWrapper.OnSpawnpointBotSpawned += Script_SpawnpointBotSpawned;
            MyScriptWrapper.OnDialogueFinished += Script_DialogueFinished;


            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);

            m_repairPipesDummyShoot = MyScriptWrapper.GetEntity((uint)EntityId.StabilizeGasPipesShootDetector);
            m_repairedCounter = 0;
        }

        private void Script_DialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.LAST_HOPE_0800 && m_findTunnel.IsAvailable())
            {
                m_findTunnel.Success();
            }
        }


        private void Script_EntityAtacked(MyEntity attacker, MyEntity entity2)
        {
            var boundingSphere = MySession.PlayerShip.WorldVolume;

            if (MyScriptWrapper.IsPlayerShip(attacker) && m_repairPipes.IsAvailable() && m_repairPipesDummyShoot.GetIntersectionWithSphere(ref boundingSphere))
            {
                MyScriptWrapper.SetEntityEnabled((uint)EntityId.StabilizeGasPipesShootparticle, true);
                MyScriptWrapper.DestroyPlayerShip();
            }
        }

        private void Script_SpawnpointBotSpawned(MyEntity entity1, MyEntity entity2)
        {
            var bot = entity2 as MySmallShipBot;

            if (entity1.EntityId != null && bot != null)
            {
                bot.SleepDistance = 40000;
                bot.SeeDistance = 2000;
                //bot.Attack(MySession.PlayerShip);
            }
        }


        public override void Unload()
        {
            MyScriptWrapper.EntityDeath -= Script_EntityDeath;
            MyScriptWrapper.OnEntityAtacked -= Script_EntityAtacked;
            MyScriptWrapper.OnSpawnpointBotSpawned -= Script_SpawnpointBotSpawned;
            MyScriptWrapper.OnDialogueFinished -= Script_DialogueFinished;

            m_repairPipesDummyShoot = null;

            base.Unload();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Journal;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyResearchVesselMission : MyMission
    {
        #region EntityIDs
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 1871,
            DummyReachShip = 30608,
            checkpoint_1 = 30187,
            DummyNearHub1 = 30188,
            checkpoint_3 = 30189, // LAB CARGO
            checkpoint_4 = 30190,
            checkpoint_5 = 30191,
            checkpoint_6 = 30192, //HANGAR CARGO
            checkpoint_7 = 30397,
            checkpoint_8 = 30398,
            checkpoint_9 = 30399,
            checkpoint_10 = 30400, // GENERATOR CARGO
            checkpoint_11 = 30401, //CARGO
            DummyNearHub2 = 35067, // new HANGAR
            DummyNearHub3 = 35072, // new GENERATOR 
            checkpoint_14 = 35071, // new LAB 
            DummyReturn = 21583,
            BridgeDetector = 31047,
            BridgeSpawnpoint = 31048,
            GeneratorDetector1 = 31053,
            GeneratorDetector2 = 31055,
            GeneratorDetector3 = 31057,
            GeneratorSpawnpoint = 31059,
            Cargo1 = 31550, // checkpoint 11 - CARGO
            Cargo2 = 31554, // checkpoint 10 - GENERATOR
            Cargo3 = 31549, //checkpoint 3 - LAB
            Cargo4 = 31551, //checkpoint 6 - HANGAR
            Hub1 = 2408,
            Hub2 = 2407,
            Hub3 = 2434,
            Spawn1 = 36779,
            spawn2 = 36787,
            detect1 = 36785,
            detect2 = 36788,

            Door1 = 277,
            Door2 = 278,
            Door3 = 279,

            Particle1 = 16777494,
            Particle2 = 16777495,
            Particle3 = 16777496,
            Particle4 = 16777497,
            Particle5 = 16777498,
            Particle6 = 16777499,
            Particle7 = 16777500,
            Particle8 = 16777501,
            Particle9 = 16777502,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)(((EntityID?) value).Value));
            }
        }
        #endregion

        
        private MyEntityDetector m_generatorDetector1;
        private MyEntityDetector m_generatorDetector2;
        private MyEntityDetector m_generatorDetector3;


        public MyResearchVesselMission()
        {
            ID = MyMissionID.RESEARCH_VESSEL;
            DebugName = new StringBuilder("12-Eurydice/Osaka");
            Name = MyTextsWrapperEnum.RESEARCH_VESSEL;
            Description = MyTextsWrapperEnum.RESEARCH_VESSEL_Description;
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(4189723, 0, -2201402);
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.RIME_CONVINCE };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_TAKE_FOURTH };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };


            #region Objectives
            m_objectives = new List<MyObjective>(); 

            var introDialogue = new MyObjectiveDialog(
                MyMissionID.RESEARCH_VESSEL_INTRO,
                null,
                this,
                new MyMissionID[] { },
                MyDialogueEnum.RESEARCH_VESSEL_0100_INTRO
            ) { SaveOnSuccess = true };
            m_objectives.Add(introDialogue);


            var reachShip = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_REACH_SHIP,
                MyMissionID.RESEARCH_VESSEL_REACH_SHIP,
                MyTextsWrapperEnum.RESEARCH_VESSEL_REACH_SHIP_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_INTRO },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyReachShip)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudEurydice };
            reachShip.OnMissionSuccess += new MissionHandler(ReachShip_Success);
            m_objectives.Add(reachShip);

    
            var checkCargo = new MyObjective( 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_CARGO,
                MyMissionID.RESEARCH_VESSEL_CHECK_CARGO,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_CARGO_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_REACH_SHIP },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_1)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudContainer };
            m_objectives.Add(checkCargo);


            var checkCommandRoom = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_COMMAND_ROOM,
                MyMissionID.RESEARCH_VESSEL_CHECK_COMMAND_ROOM,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_COMMAND_ROOM_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_CARGO },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyNearHub1)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudCommandRoom };
            checkCommandRoom.OnMissionLoaded += new MissionHandler(checkCommandRoom_OnMissionLoaded);
            m_objectives.Add(checkCommandRoom);


            var unlockFirstCargo = new MyObjectiveEnablePrefabs(
                MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_1,
                MyMissionID.RESEARCH_VESSEL_USE_HUB_1,
                MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_1_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_COMMAND_ROOM },
                null,
                new List<uint> { (uint)EntityID.Hub1 },
                new List<uint> { (uint)EntityID.Door1 }
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(unlockFirstCargo);


            var checkLaboratory = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_LABORATORY,
                MyMissionID.RESEARCH_VESSEL_CHECK_LABORATORY,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_LABORATORY_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_USE_HUB_1 },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_14)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudLab };
            m_objectives.Add(checkLaboratory);


            var takeFirstParts = new MyUseObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_FIRST,
                MyMissionID.RESEARCH_VESSEL_TAKE_FIRST,
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_FIRST_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_LABORATORY },
                new MyMissionLocation(baseSector, (uint)EntityID.Cargo3),
                MyTextsWrapperEnum.PressToTakeComponent,
                MyTextsWrapperEnum.Component,
                MyTextsWrapperEnum.TakingInProgress,
                1000,
                MyUseObjectiveType.Activating,
                radiusOverride: 50
            ) { SaveOnSuccess = true };
            takeFirstParts.OnMissionSuccess += TakeFirstParts_Success;
            m_objectives.Add(takeFirstParts);


            var checkWarehouse = new MyObjective( 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_WAREHOUSE,
                MyMissionID.RESEARCH_VESSEL_CHECK_WAREHOUSE,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_WAREHOUSE_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_TAKE_FIRST }, 
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_4)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudWarehouse };
            m_objectives.Add(checkWarehouse); 


            var checkDrillMachineRoom = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_DRILL_ROOM,
                MyMissionID.RESEARCH_VESSEL_CHECK_DRILL_ROOM, 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_DRILL_ROOM_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_WAREHOUSE }, 
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_5)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudDrillRoom };
            checkDrillMachineRoom.OnMissionSuccess += new MissionHandler(checkDrillMachineRoom_OnMissionSuccess);
            m_objectives.Add(checkDrillMachineRoom); 


            var checkFirstHangar = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_FIRST_HANGAR,
                MyMissionID.RESEARCH_VESSEL_CHECK_FIRST_HANGAR,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_FIRST_HANGAR_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_DRILL_ROOM },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyNearHub2)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHangar };
            m_objectives.Add(checkFirstHangar);


            var takeSecondParts = new MyUseObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_SECOND,
                MyMissionID.RESEARCH_VESSEL_TAKE_SECOND,
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_SECOND_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_FIRST_HANGAR },
                new MyMissionLocation(baseSector, (uint)EntityID.Cargo4),
                MyTextsWrapperEnum.PressToTakeComponent,
                MyTextsWrapperEnum.Component,
                MyTextsWrapperEnum.TakingInProgress,
                1000,
                MyUseObjectiveType.Activating,
                radiusOverride: 50
            ) { SaveOnSuccess = true };
            takeSecondParts.OnMissionSuccess += TakeSecondParts_Success;
            m_objectives.Add(takeSecondParts);


            var useHub2 = new MyObjectiveEnablePrefabs(
               MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_2,
               MyMissionID.RESEARCH_VESSEL_USE_HUB_2,
               MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_2_Description,
               null,
               this,
               new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_TAKE_SECOND },
                null,
                new List<uint> { (uint)EntityID.Hub2 },
                new List<uint> { (uint)EntityID.Door2 }
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(useHub2);


            var checkSecondHangar = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_SECOND_HANGAR,
                MyMissionID.RESEARCH_VESSEL_CHECK_SECOND_HANGAR, 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_SECOND_HANGAR_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_USE_HUB_2 },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_7)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHangar };
            m_objectives.Add(checkSecondHangar);


            var checkThirdHangar = new MyObjective( 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_THIRD_HANGAR,
                MyMissionID.RESEARCH_VESSEL_CHECK_THIRD_HANGAR,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_THIRD_HANGAR_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_SECOND_HANGAR },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_8)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHangar };
            m_objectives.Add(checkThirdHangar);
          

            var checkSecondWarehouse = new MyObjective( 
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_SECOND_WAREHOUSE,
                MyMissionID.RESEARCH_VESSEL_CHECK_SECOND_WAREHOUSE,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_SECOND_WAREHOUSE_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_THIRD_HANGAR },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_9)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudWarehouse };
            m_objectives.Add(checkSecondWarehouse);
            

            var checkGenerator = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_GENERATOR,
                MyMissionID.RESEARCH_VESSEL_CHECK_GENERATOR,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_GENERATOR_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_SECOND_WAREHOUSE },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyNearHub3)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudGeneratorRoom };
            m_objectives.Add(checkGenerator);


            var useHub3 = new MyObjectiveEnablePrefabs(
                MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_3,
                MyMissionID.RESEARCH_VESSEL_USE_HUB_3,
                MyTextsWrapperEnum.RESEARCH_VESSEL_USE_HUB_3_Description,
               null,
               this,
               new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_GENERATOR },
                null,
                new List<uint> { (uint)EntityID.Hub3 },
                new List<uint> { (uint)EntityID.Door3 }
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            useHub3.OnMissionSuccess += new MissionHandler(UseHub3_Success);
            m_objectives.Add(useHub3);

            var takeThridProbe = new MyUseObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_THIRD,
                MyMissionID.RESEARCH_VESSEL_TAKE_THIRD,
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_THIRD_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_USE_HUB_3 },
                new MyMissionLocation(baseSector, (uint)EntityID.Cargo2),
                MyTextsWrapperEnum.PressToTakeComponent,
                MyTextsWrapperEnum.Component,
                MyTextsWrapperEnum.TakingInProgress,
                1000,
                MyUseObjectiveType.Activating,
                radiusOverride: 50
                ) { SaveOnSuccess = true };
            takeThridProbe.OnMissionSuccess += TakeThirdParts_Success;
            m_objectives.Add(takeThridProbe);

            var takeThridProbeDialogue = new MyObjectiveDialog(
                MyTextsWrapperEnum.Null,
                MyMissionID.RESEARCH_VESSEL_TAKE_THIRD_DIALOGUE,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_TAKE_THIRD },
                MyDialogueEnum.RESEARCH_VESSEL_0700_THIRDPARTS);
            m_objectives.Add(takeThridProbeDialogue);

            var checkCargoAgain = new MyObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_CARGO_AGAIN,
                MyMissionID.RESEARCH_VESSEL_CHECK_CARGO_AGAIN,
                MyTextsWrapperEnum.RESEARCH_VESSEL_CHECK_CARGO_AGAIN_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_TAKE_THIRD_DIALOGUE },
                new MyMissionLocation(baseSector, (uint)EntityID.checkpoint_1)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudContainer };
            checkCargoAgain.OnMissionSuccess += new MissionHandler(CheckCargoAgain_Success);
            m_objectives.Add(checkCargoAgain);
        
            var takeFourthParts = new MyUseObjective(
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_FOURTH,
                MyMissionID.RESEARCH_VESSEL_TAKE_FOURTH,
                MyTextsWrapperEnum.RESEARCH_VESSEL_TAKE_FOURTH_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RESEARCH_VESSEL_CHECK_CARGO_AGAIN },
                new MyMissionLocation(baseSector, (uint)EntityID.Cargo1),
                MyTextsWrapperEnum.PressToTakeComponent,
                MyTextsWrapperEnum.Component,
                MyTextsWrapperEnum.TakingInProgress,
                 1000,
                 MyUseObjectiveType.Activating, 
                 radiusOverride: 50
                ) { SaveOnSuccess = false };
            takeFourthParts.OnMissionSuccess += TakeFourthParts_Success;
            m_objectives.Add(takeFourthParts);


            #endregion
        }

        void CheckCargoAgain_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0800_FORCEFIELDDOWN);
        }

        void ReachShip_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0200_INCOMINGSHIPS);
        }

        void checkDrillMachineRoom_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0400_LOOKATTHIS);
        }

        void checkCommandRoom_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0300_FORTIFIED);
        }


        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();

            // Ensure correct game state
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_NEUTRAL);
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);
            
            if (MySession.Static.EventLog.IsMissionFinished(MyMissionID.RESEARCH_VESSEL_INTRO))
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_WORST);    
            }

            var bridgeDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.BridgeDetector));
            bridgeDetector.OnEntityEnter += BridgeDetectorAction;
            bridgeDetector.On();

            m_generatorDetector1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.GeneratorDetector1));
            m_generatorDetector1.OnEntityEnter += GeneratorDetector_Enter;
            m_generatorDetector1.On();

            m_generatorDetector2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.GeneratorDetector1));
            m_generatorDetector2.OnEntityEnter += GeneratorDetector_Enter;
            m_generatorDetector2.On();

            m_generatorDetector3 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.GeneratorDetector1));
            m_generatorDetector3.OnEntityEnter += GeneratorDetector_Enter;
            m_generatorDetector3.On();

            var detectorNearHub1 = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearHub1);
            detectorNearHub1.OnEntityEnter += new OnEntityEnter(DetectorNearHub1_Enter);
            detectorNearHub1.On();

            var detectorNearHub2 = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearHub2);
            detectorNearHub2.OnEntityEnter += new OnEntityEnter(DetectorNearHub2_Enter);
            detectorNearHub2.On();

            var detectorNearHub3 = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearHub3);
            detectorNearHub3.OnEntityEnter += new OnEntityEnter(DetectorNearHub3_Enter);
            detectorNearHub3.On();

            var detect1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.detect1));
            detect1.OnEntityEnter += Detector1_Enter;
            detect1.On();


            var detect2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.detect2));
            detect2.OnEntityEnter += Detector2_Enter;
            detect2.On();


            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 3); // Sets music group to be played in the sector - no matter if the mission is running or not

            MyScriptWrapper.Highlight((uint)EntityID.Cargo1, true, this); //highlighted items
            MyScriptWrapper.Highlight((uint)EntityID.Cargo2, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.Cargo3, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.Cargo4, true, this);

            MyScriptWrapper.OnDialogueFinished += new MyScriptWrapper.DialogueHandler(MyScriptWrapper_OnDialogueFinished);
        }

        public override void Unload()
        {
            base.Unload();

            m_generatorDetector1 = null;
            m_generatorDetector2 = null;
            m_generatorDetector3 = null;
        
            MyScriptWrapper.OnDialogueFinished -= new MyScriptWrapper.DialogueHandler(MyScriptWrapper_OnDialogueFinished);
        }

        void MyScriptWrapper_OnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.RESEARCH_VESSEL_0200_INCOMINGSHIPS)
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0250_YOUASKEDFORIT);
            }
        }

        void DetectorNearHub1_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_1000_FIRSTHUB);
                sender.Off();
            }
        }

        void DetectorNearHub2_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_1100_SECONDHUB);
                sender.Off();
            }
        }

        void DetectorNearHub3_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_1200_THIRDHUB);
                sender.Off();
            }
        }

        void UseHub3_Success(MyMissionBase sender)
        {
            MyScriptWrapper.TryHide((uint)EntityID.Particle1);
            MyScriptWrapper.TryHide((uint)EntityID.Particle2);
            MyScriptWrapper.TryHide((uint)EntityID.Particle3);
            MyScriptWrapper.TryHide((uint)EntityID.Particle4);
            MyScriptWrapper.TryHide((uint)EntityID.Particle5);
            MyScriptWrapper.TryHide((uint)EntityID.Particle6);
            MyScriptWrapper.TryHide((uint)EntityID.Particle7);
            MyScriptWrapper.TryHide((uint)EntityID.Particle8);
            MyScriptWrapper.TryHide((uint)EntityID.Particle9);
        }


        void TakeFirstParts_Success(MyMissionBase sender) 
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0500_FIRSTPARTS);
            MyScriptWrapper.GetEntity((uint)EntityID.Cargo3).MarkForClose();
        }


        void TakeSecondParts_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0600_SECONDPARTS);
            MyScriptWrapper.GetEntity((uint)EntityID.Cargo4).MarkForClose();
        }


        void TakeThirdParts_Success(MyMissionBase sender) 
        {
            MyScriptWrapper.GetEntity((uint)EntityID.Cargo2).MarkForClose();
        }


        void TakeFourthParts_Success(MyMissionBase sender) 
        {
            MyScriptWrapper.GetEntity((uint)EntityID.Cargo1).MarkForClose();
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RESEARCH_VESSEL_0900_FOURTHPARTS);
        }

        public void BridgeDetectorAction(MyEntityDetector sender, MyEntity entity, int meetCriterias) 
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.BridgeSpawnpoint);
                sender.Off();
            }
        }


        public void GeneratorDetector_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.GeneratorSpawnpoint);
                m_generatorDetector1.Off();
                m_generatorDetector2.Off();
                m_generatorDetector3.Off();
            }
        }


        public void Detector1_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn1);
                sender.Off();
            }
        }


        public void Detector2_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.spawn2);
                sender.Off();
            }
        }
    }
}

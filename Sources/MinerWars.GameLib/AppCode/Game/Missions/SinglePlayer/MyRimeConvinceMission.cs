#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
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
using MinerWars.Resources;

#endregion


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyRimeConvinceMission : MyMission
    {
        #region Enums

        private enum EntityID // list of IDs used in script
        {
            StartLocation = 40233,
            //FrancisReached = 40234,
            Madelyn = 43179,
            FrancisReef = 47152,
            Barkeeper = 47167,
            FlyToDummy = 47180,
            //LookAtDummy = 47181,
            FactoryFound = 48143,
            FlyToVessel = 62,
            DetectorVessel = 135,
            SpawnContact = 47165,
            WaypointStart = 47183,
            WaypointFollow1 = 16777586,
            WaypointFollow2 = 16777642,
            WaypointFollow3 = 16777704,
            SpawnpointClient1 = 1,
            SpawnpointClient2 = 2,
            SpawnpointBouncer = 48,
            SpawnpointFactory = 61,
            SpawnpointGuardL = 63,
            SpawnpointGuardR = 64,
            WaypointGuardL = 133,
            WaypointGuardR = 121
        }

        private List<uint> m_factorySpawns = new List<uint>()
                                                   {
                                                       39164,
                                                       39163,
                                                       39162,
                                                       39161,
                                                       39166,
                                                       39165,
                                                       16777724,
                                                       16777723,
                                                       16777726,
                                                       16777727,
                                                       16777728,
                                                   };


        #endregion

        private List<uint> m_boxes = new List<uint>() { 70, 72, 74, 76, 78, 103, 104, 105 };
        private List<uint> m_plantBoxes = new List<uint>() { 107, 108 };
        private List<uint> m_doors = new List<uint>() { 10064, 10063 };
        private MyMeetObjective m_01MeetReef;
        private MyObjective m_waitForTheMoment;
        private MyObjectiveDialog m_02TalkToReef;
        private MySmallShipBot m_ravenguyBot;
        private MySmallShipBot m_ravengirlBot;
        private MySmallShipBot m_marcus;
        private MyMeetObjective m_03MeetBarkeeper;
        private MyObjectiveDialog m_04TalkToBarKeeper;
        private MyObjective m_05FlyToDuplex;
        private MyObjective m_06watchSuspects;
        private MyObjectiveFollowBot m_07followContact;
        private MySmallShipBot m_reef;
        private MySmallShipBot m_barkeeper;
        private MyEntityDetector m_vesselDetector;
        private bool m_guardLReached;
        private bool m_guardRReached;
        private MyMultipleUseObjective m_08CollectIllegalCargo;
        private MyMultipleUseObjective m_09plantcargo;
        private MyMeetObjective m_10MeetReef;
        private MyObjectiveDialog m_11TalkToReef;
        private MyObjective m_12getBackmadelyn;
        

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }

            foreach (var value in m_factorySpawns)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        public MyRimeConvinceMission()
        {
            ID = MyMissionID.RIME_CONVINCE; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("10-Rime"); // Name of mission
            Name = MyTextsWrapperEnum.RIME_CONVINCE;
            Description = MyTextsWrapperEnum.RIME_CONVINCE_Description;
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1922856, 0, -2867519); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.FORT_VALIANT_C }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RIME_CONVINCE_TALK_TO_FRANCIS };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            var introduction = new MyObjectiveDialog(
                MyMissionID.RIME_CONVINCE_INTRODUCTION,
                null,
                this,
                new MyMissionID[] { },
                dialogId: MyDialogueEnum.RIME_0100_INTRODUCTION
                ) { SaveOnSuccess = true };
            m_objectives.Add(introduction);   

            m_01MeetReef = new MyMeetObjective(
                (MyTextsWrapperEnum.RIME_CONVINCE_GET_FRANCIS_REEF_Name),
                MyMissionID.RIME_CONVINCE_GET_FRANCIS_REEF,
                (MyTextsWrapperEnum.RIME_CONVINCE_GET_FRANCIS_REEF_Description),
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_INTRODUCTION},
                null,
                (uint)EntityID.FrancisReef,
                distanceToTalk: 300,
                slowdown: 0.25f,
                startDialogueId: MyDialogueEnum.RIME_0150_HEAD_TO_REEF,
                successDialogueId: MyDialogueEnum.RIME_0200_REEF_REACHED
                ) { SaveOnSuccess = false, FollowMe = false };
            m_objectives.Add(m_01MeetReef);
            m_01MeetReef.OnMissionLoaded += M01MeetReefOnOnMissionLoaded;

            m_02TalkToReef = new MyObjectiveDialog(
                MyTextsWrapperEnum.Null,
                MyMissionID.RIME_CONVINCE_TALK_FRANCIS_REEF,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_GET_FRANCIS_REEF },
                dialogId: MyDialogueEnum.RIME_0300_TALK_TO_REEF
                ) { SaveOnSuccess = true };
            m_objectives.Add(m_02TalkToReef);
            m_02TalkToReef.OnMissionLoaded += M02TalkToReefOnOnMissionLoaded;
            m_02TalkToReef.OnMissionSuccess += M02TalkToReefOnOnMissionSuccess;

            m_05FlyToDuplex = new MyObjective(
                (MyTextsWrapperEnum.RIME_CONVINCE_GO_TO_DUPLEX_Name),
                MyMissionID.RIME_CONVINCE_GO_TO_DUPLEX,
                (MyTextsWrapperEnum.RIME_CONVINCE_GO_TO_DUPLEX_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_TALK_FRANCIS_REEF },
                new MyMissionLocation(baseSector, (uint)EntityID.FlyToDummy),
                //startDialogId: MyDialogueEnum.RIME_0400_ON_THE_WAY,
                successDialogId: MyDialogueEnum.RIME_0500_LISTEN_TO_SUSPICIOUS
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudDuplex };
            m_objectives.Add(m_05FlyToDuplex);

            MyObjectiveDialog clientsTalk = new MyObjectiveDialog(
            MyMissionID.RIME_CONVINCE_CLIENTS_TALK,
            null,
            this,
            new MyMissionID[] { MyMissionID.RIME_CONVINCE_GO_TO_DUPLEX },
            dialogId: MyDialogueEnum.RIME_0600_CLIENTS_TALK);
            clientsTalk.OnMissionLoaded += delegate
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointClient1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointClient2);
            };
            m_objectives.Add(clientsTalk);

            MyObjectiveDialog bouncerTalk = new MyObjectiveDialog(
            MyMissionID.RIME_CONVINCE_DUPLEX_BOUNCER,
            null,
            this,
            new MyMissionID[] { MyMissionID.RIME_CONVINCE_CLIENTS_TALK },
            dialogId: MyDialogueEnum.RIME_0700_DUPLEX_BOUNCER);
            bouncerTalk.OnMissionLoaded += delegate
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBouncer);
            };
            bouncerTalk.OnMissionSuccess += delegate
            {
                MySmallShipBot bouncer = MyScriptWrapper.TryGetEntity("Bouncer") as MySmallShipBot;
                if (bouncer != null)
                {
                    bouncer.LookTarget = null;
                    m_barkeeper.LookTarget = null;
                    bouncer.SetWaypointPath("Client1");
                    bouncer.PatrolMode = MyPatrolMode.ONE_WAY;
                    bouncer.Patrol();
                }
            };
            m_objectives.Add(bouncerTalk);

            var returnBack = new MyObjective(
                (MyTextsWrapperEnum.RIME_CONVINCE_RETURN_TO_POSITION_Name),
                MyMissionID.RIME_CONVINCE_RETURN_TO_POSITION,
                (MyTextsWrapperEnum.RIME_CONVINCE_RETURN_TO_POSITION_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_DUPLEX_BOUNCER },
                new MyMissionLocation(baseSector, (uint)EntityID.FlyToDummy)) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            returnBack.OnMissionSuccess += ReturnBackOnMissionSuccess;
            m_objectives.Add(returnBack);

            MyObjectiveDialog contactAppears = new MyObjectiveDialog(
            MyTextsWrapperEnum.RIME_CONVINCE_CONTACT_APPEARS_Name,
            MyMissionID.RIME_CONVINCE_CONTACT_APPEARS,
            MyTextsWrapperEnum.RIME_CONVINCE_CONTACT_APPEARS_Description,
            null,
            this,
            new MyMissionID[] { MyMissionID.RIME_CONVINCE_RETURN_TO_POSITION },
            dialogId: MyDialogueEnum.RIME_0800_CONTACT_APPEARS);
            contactAppears.OnMissionSuccess += delegate
            {
                MySmallShipBot mitchel = (MySmallShipBot)MyScriptWrapper.GetEntity("Mitchel");
                mitchel.SetWaypointPath("Follow1");
                mitchel.PatrolMode = MyPatrolMode.ONE_WAY;
                mitchel.Patrol();
                m_07followContact.TargetId = mitchel.EntityId.Value.NumericValue;
            };
            m_objectives.Add(contactAppears);

            m_07followContact = new MyObjectiveFollowBot(
                (MyTextsWrapperEnum.RIME_CONVINCE_FOLLOW_CONTACT_Name),
                MyMissionID.RIME_CONVINCE_FOLLOW_CONTACT,
                (MyTextsWrapperEnum.RIME_CONVINCE_FOLLOW_CONTACT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_CONTACT_APPEARS },
                null,
                (uint)EntityID.FactoryFound,
                MyDialogueEnum.RIME_0900_FOLLOW_INSTRUCTION)
                                    {
                                        FarDialog = MyDialogueEnum.RIME_2100_HE_IS_GONE,
                                        ShortDialog = MyDialogueEnum.RIME_2200_HE_SPOTTED_US,
                                        SaveOnSuccess = true,
                                        HudName = MyTextsWrapperEnum.HudFollow
                                    };
            m_objectives.Add(m_07followContact);

            var factoryFoundDialogue = new MyObjectiveDialog(
                (MyTextsWrapperEnum.RIME_CONVINCE_FACTORY_FOUND_DIALOGUE_Name),
                MyMissionID.RIME_CONVINCE_FACTORY_FOUND_DIALOGUE,
                (MyTextsWrapperEnum.RIME_CONVINCE_FACTORY_FOUND_DIALOGUE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_FOLLOW_CONTACT },
                dialogId: MyDialogueEnum.RIME_1000_FACTORY_FOUND
                ) { SaveOnSuccess = true };
            m_objectives.Add(factoryFoundDialogue);
      
            var destroyFactoryBots = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.RIME_CONVINCE_DESTROY_FACTORY_BOTS_Name),
                MyMissionID.RIME_CONVINCE_DESTROY_FACTORY_BOTS,
                (MyTextsWrapperEnum.RIME_CONVINCE_DESTROY_FACTORY_BOTS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_FACTORY_FOUND_DIALOGUE },
                null,
                m_factorySpawns
                //startDialogID: MyDialogueEnum.RIME_1100_WE_GOT_YOUR_BACK
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            destroyFactoryBots.OnMissionLoaded += DestroyFactoryBotsLoaded;
            m_objectives.Add(destroyFactoryBots);

            m_08CollectIllegalCargo = new MyMultipleUseObjective
               ((MyTextsWrapperEnum.RIME_CONVINCE_COLLECT_CARGO_Name),
                 MyMissionID.RIME_CONVINCE_COLLECT_CARGO,
                 (MyTextsWrapperEnum.RIME_CONVINCE_COLLECT_CARGO_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.RIME_CONVINCE_DESTROY_FACTORY_BOTS },
                 MyTextsWrapperEnum.PressToCollectCargo,
                 MyTextsWrapperEnum.IllegalCargo,
                 MyTextsWrapperEnum.TakingInProgress,
                 2000,
                 m_boxes,
                 MyUseObjectiveType.Taking
                ) { RadiusOverride = 70f, SaveOnSuccess = true, StartDialogId = MyDialogueEnum.RIME_1100_GRAB_THE_ALCOHOL };
            m_objectives.Add(m_08CollectIllegalCargo);
            m_08CollectIllegalCargo.OnMissionLoaded += M08CollectIllegalCargoOnOnMissionLoaded;
            m_08CollectIllegalCargo.OnObjectUsedSucces += M08CollectIllegalCargoOnOnObjectUsedSucces;
            m_08CollectIllegalCargo.OnMissionSuccess += M08CollectIllegalCargoOnOnMissionSuccess;

            var cargoCollectedDialogue = new MyObjectiveDialog(
                (MyTextsWrapperEnum.RIME_CONVINCE_CARGO_COLLECTED_DIALOG_Name),
                MyMissionID.RIME_CONVINCE_CARGO_COLLECTED_DIALOG,
                (MyTextsWrapperEnum.RIME_CONVINCE_CARGO_COLLECTED_DIALOG_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_COLLECT_CARGO },
                dialogId: MyDialogueEnum.RIME_1200_GET_TO_THE_VESSEL
                ) {SaveOnSuccess = true};
            m_objectives.Add(cargoCollectedDialogue);

            var flyToVessel = new MyObjective(
                 (MyTextsWrapperEnum.RIME_CONVINCE_FLY_TO_VESSEL_Name),
                MyMissionID.RIME_CONVINCE_FLY_TO_VESSEL,
                (MyTextsWrapperEnum.RIME_CONVINCE_FLY_TO_VESSEL_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_CARGO_COLLECTED_DIALOG },
                new MyMissionLocation(baseSector, (uint)EntityID.FlyToVessel),
                startDialogId: MyDialogueEnum.RIME_1300_ON_THE_WAY_TO_VESSEL
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudRaynoldsVessel };
            m_objectives.Add(flyToVessel);
            flyToVessel.OnMissionLoaded += FlyToVesselLoaded;

            m_waitForTheMoment = new MyObjective(
                 (MyTextsWrapperEnum.RIME_CONVINCE_WAIT_FOR_THE_MOMENT_Name),
                MyMissionID.RIME_CONVINCE_WAIT_FOR_THE_MOMENT,
                (MyTextsWrapperEnum.RIME_CONVINCE_WAIT_FOR_THE_MOMENT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_FLY_TO_VESSEL },
                null,
                startDialogId: MyDialogueEnum.RIME_1500_WAIT_FOR_THE_SIGNAL,
                successDialogId: MyDialogueEnum.RIME_1600_THIS_IS_OUR_CHANCE)
                { SaveOnSuccess = true };
            m_objectives.Add(m_waitForTheMoment);
            m_waitForTheMoment.OnMissionLoaded += WaitForTheMomentLoaded;    
            m_waitForTheMoment.OnMissionUpdate += WaitForTheMomentUpdate;
            m_waitForTheMoment.OnMissionCleanUp += new MissionHandler(m_waitForTheMoment_OnMissionCleanUp);

            m_09plantcargo = new MyMultipleUseObjective
                ((MyTextsWrapperEnum.RIME_CONVINCE_PLANT_CARGO_Name),
                 MyMissionID.RIME_CONVINCE_PLANT_CARGO,
                (MyTextsWrapperEnum.RIME_CONVINCE_PLANT_CARGO_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.RIME_CONVINCE_WAIT_FOR_THE_MOMENT },
                 MyTextsWrapperEnum.HoldToPlantCargo,
                 MyTextsWrapperEnum.IllegalCargo,
                 MyTextsWrapperEnum.PlantingInProgress,
                 2000,
                 m_plantBoxes,
                 MyUseObjectiveType.Taking,
                 startDialogId: MyDialogueEnum.RIME_1650_PLACE
                 ) { RadiusOverride = 60f, SaveOnSuccess = true};
            m_objectives.Add(m_09plantcargo);
            m_09plantcargo.OnMissionLoaded += M09PlantcargoOnOnMissionLoaded;
            m_09plantcargo.OnMissionSuccess += M09PlantcargoOnOnMissionSuccess;
            m_09plantcargo.OnObjectUsedSucces += M09PlantcargoOnOnObjectUsedSucces;

            var getOutOfTheVessel = new MyObjective(
                  (MyTextsWrapperEnum.RIME_CONVINCE_GET_OUT_OF_THE_VESSEL_Name),
                MyMissionID.RIME_CONVINCE_GET_OUT_OF_THE_VESSEL,
                (MyTextsWrapperEnum.RIME_CONVINCE_GET_OUT_OF_THE_VESSEL_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_PLANT_CARGO },
                new MyMissionLocation(baseSector, (uint)EntityID.FlyToVessel)
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(getOutOfTheVessel);
            getOutOfTheVessel.OnMissionLoaded += GetOutOfTheVesselLoaded;
            getOutOfTheVessel.OnMissionSuccess += GetOutOfTheVesselSuccess;
                          /*
            var cargoPlantedObjectiveDialogue = new MyObjectiveDialog(
                new StringBuilder("Listen to the conversation"),
                MyMissionID.RIME_CONVINCE_CARGO_PLANTED_OBJECTIVE_DIALOGUE,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_GET_OUT_OF_THE_VESSEL },
                dialogId: MyDialogueEnum.RIME_1800_CARGO_PLANTED
                ) { SaveOnSuccess = true };
            m_objectives.Add(cargoPlantedObjectiveDialogue);
                           */
            m_10MeetReef = new MyMeetObjective(
                (MyTextsWrapperEnum.RIME_CONVINCE_GO_BACK_TO_FRANCIS_Name),
                MyMissionID.RIME_CONVINCE_GO_BACK_TO_FRANCIS,
                (MyTextsWrapperEnum.RIME_CONVINCE_GO_BACK_TO_FRANCIS_Description),
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_GET_OUT_OF_THE_VESSEL },
                null,
                (uint)EntityID.FrancisReef,
                100,
                0.25f,
                null,
                startDialogueId: MyDialogueEnum.RIME_1800_CARGO_PLANTED
                ) { SaveOnSuccess = false, FollowMe = false };
            m_objectives.Add(m_10MeetReef);
            //m_01MeetReef.OnMissionLoaded += M01MeetReefOnOnMissionLoaded;

            m_11TalkToReef = new MyObjectiveDialog(
                MyMissionID.RIME_CONVINCE_TALK_TO_FRANCIS,
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_CONVINCE_GO_BACK_TO_FRANCIS },
                dialogId: MyDialogueEnum.RIME_2000_REEF_TALK
                ) { SaveOnSuccess = false };
            m_objectives.Add(m_11TalkToReef);
            m_11TalkToReef.OnMissionLoaded += M11TalkToReefOnOnMissionLoaded;
            m_11TalkToReef.OnMissionSuccess += M11TalkToReefOnOnMissionSuccess;
        }

        void m_waitForTheMoment_OnMissionCleanUp(MyMissionBase sender)
        {
            m_vesselDetector.OnEntityEnter -= VesselDetectorEntered;
            m_vesselDetector.Off();
        }

        private void M11TalkToReefOnOnMissionSuccess(MyMissionBase sender)
        {
            m_barkeeper.LookTarget = null;
        }

        private void M02TalkToReefOnOnMissionSuccess(MyMissionBase sender)
        {
            m_reef.LookTarget = null;
        }


        private void M02TalkToReefOnOnMissionLoaded(MyMissionBase sender)
        {
            m_reef.LookTarget = MySession.PlayerShip;
        }

        private void DestroyFactoryBotsLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoints(m_factorySpawns);

            MySmallShipBot mitchel = (MySmallShipBot)MyScriptWrapper.GetEntity("Mitchel");
            MyScriptWrapper.ChangeFaction(mitchel, MyMwcObjectBuilder_FactionEnum.Slavers);
            MyScriptWrapper.SetEntityDestructible(mitchel, true);

            //Support for objective skip
            MySmallShipBot smuggler2 = (MySmallShipBot)MyScriptWrapper.TryGetEntity("Smuggler");
            if (smuggler2 == null)
            {
                Follow1Reached();
                MyEntity spawnPoint = MyScriptWrapper.GetEntity(((uint)EntityID.SpawnpointFactory));
                (spawnPoint as MySpawnPoint).SpawnShip(0);
            }

            MySmallShipBot smuggler = (MySmallShipBot)MyScriptWrapper.GetEntity("Smuggler");
            MyScriptWrapper.ChangeFaction(smuggler, MyMwcObjectBuilder_FactionEnum.Slavers);
            MyScriptWrapper.SetEntityDestructible(smuggler, true);

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointGuardL);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointGuardR);
        }

        private void FlyToVesselLoaded(MyMissionBase sender)
        {        
        }

        private void VesselDetectorEntered(MyEntity sender, MyEntity bot, int meetCriterias)
        {
            if (bot == MySession.PlayerShip)
            {
                GuardsCatchedPlayer();
            }
        }

        private void WaitForTheMomentLoaded(MyMissionBase sender)
        {
            m_guardLReached = false;
            m_guardRReached = false;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StealthAction, 3, "MM01");

            MySmallShipBot vesselGuardL = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardL");
            MySmallShipBot vesselGuardR = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardR");
            vesselGuardL.SetWaypointPath("GUARDLL");
            vesselGuardR.SetWaypointPath("GUARDRL");
            vesselGuardL.PatrolMode = MyPatrolMode.ONE_WAY;
            vesselGuardR.PatrolMode = MyPatrolMode.ONE_WAY;
            vesselGuardR.Patrol();
            vesselGuardL.Patrol();
            MyScriptWrapper.SetEntityDestructible(vesselGuardR, false);
            MyScriptWrapper.SetEntityDestructible(vesselGuardL, false);

            m_vesselDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorVessel));
            m_vesselDetector.On();
            m_vesselDetector.OnEntityEnter += VesselDetectorEntered;

        }

        private void WaitForTheMomentUpdate(MyMissionBase sender)
        {
            if (m_guardLReached && m_guardRReached)
            {
                m_waitForTheMoment.Success();
            }
        }

        private void ReturnBackOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_doors, true);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnContact);
        }

        private void GetOutOfTheVesselLoaded(MyMissionBase sender)
        {
            sender.MissionTimer.RegisterTimerAction(20000, GuardsCatchedPlayer, false, "", false);
        }

        private void GetOutOfTheVesselSuccess(MyMissionBase sender)
        {
            sender.MissionTimer.ClearActions();
        }

        private void MissedWindow()
        {
            MySmallShipBot vesselGuardL = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardL");
            MySmallShipBot vesselGuardR = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardR");
            MyScriptWrapper.ChangeFaction(vesselGuardL, MyMwcObjectBuilder_FactionEnum.Slavers);
            MyScriptWrapper.ChangeFaction(vesselGuardR, MyMwcObjectBuilder_FactionEnum.Slavers);
            Fail(MyTextsWrapperEnum.Fail_MissedTimeWindow);
        }

        private void GuardsCatchedPlayer()
        {
            MySmallShipBot vesselGuardL = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardL");
            MySmallShipBot vesselGuardR = (MySmallShipBot)MyScriptWrapper.GetEntity("GuardR");
            MyScriptWrapper.ChangeFaction(vesselGuardL, MyMwcObjectBuilder_FactionEnum.Slavers);
            MyScriptWrapper.ChangeFaction(vesselGuardR, MyMwcObjectBuilder_FactionEnum.Slavers);
            Fail(MyTextsWrapperEnum.Fail_CaughtByGuards);
        }

        private void M11TalkToReefOnOnMissionLoaded(MyMissionBase sender)
        {
            m_marcus.Follow(MySession.PlayerShip);
            m_ravenguyBot.Follow(MySession.PlayerShip);
            m_ravengirlBot.Follow(MySession.PlayerShip);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3, "KA02");
            m_reef.LookTarget = MySession.PlayerShip;
        }

        private void M09PlantcargoOnOnObjectUsedSucces(uint entityId)
        {
            MyScriptWrapper.Highlight(entityId, false, this);
        }

        private void M09PlantcargoOnOnMissionSuccess(MyMissionBase sender)
        {
            sender.MissionTimer.ClearActions();
        }

        private void M09PlantcargoOnOnMissionLoaded(MyMissionBase sender)
        {
            foreach (var box in m_plantBoxes)
            {
                MyScriptWrapper.Highlight(box, true, this);
            }
            sender.MissionTimer.RegisterTimerAction(25000, MissedWindow, false, "", false);
        }

        void HurryUp()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RIME_1700_HURRY_UP);
        }

        private void M08CollectIllegalCargoOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3, "KA02");
        }
        private void M08CollectIllegalCargoOnOnObjectUsedSucces(uint entityId)
        {
            MyScriptWrapper.Highlight(entityId, false, this);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(entityId));
        }

        private void M08CollectIllegalCargoOnOnMissionLoaded(MyMissionBase sender)
        {
            foreach (var box in m_boxes)
            {
                MyScriptWrapper.Highlight(box, true, this);
            }

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 3, "KA08");
        }


        private void MyScriptWrapperOnOnSpawnpointBotSpawned(MyEntity spawnPoint, MyEntity bot)
        {
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnContact)
            {
                //  Debug.Assert(!m_07followContact.IsAvailable(), "Bot spawned in bad mission"); //Not needed anymore
                MySmallShipBot mitchel = bot as MySmallShipBot;
                mitchel.SpeedModifier = 0.7f;
                mitchel.SetName("Mitchel");
                mitchel.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_MITCHEL)).ToString();
            }
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointClient1 || spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointClient2)
            {
                bot.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_CLIENT1)).ToString();
                MySmallShipBot botship = bot as MySmallShipBot;
                botship.SpeedModifier = 0.5f;
            }
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointBouncer)
            {
                MySmallShipBot bouncer = bot as MySmallShipBot;
                bot.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_BOUNCER)).ToString();
                bot.SetName("Bouncer");
                MyScriptWrapper.SetEntityDestructible(bot, false);
                bouncer.LookTarget = m_barkeeper;
                m_barkeeper.LookTarget = bouncer;

            }
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointFactory)
            {
                bot.SetName("Smuggler");
                bot.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_SMUGGLER)).ToString();
                MyScriptWrapper.SetEntityDestructible(bot, false);
                ((MySmallShipBot)bot).LookTarget = MyScriptWrapper.GetEntity("Mitchel");
            }
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointGuardL)
            {
                bot.SetName("GuardL");
                bot.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_GUARD)).ToString();
                MyScriptWrapper.SetEntityDestructible(bot, false);
                ((MySmallShipBot)bot).SleepDistance = 10000;
            }
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointGuardR)
            {
                bot.SetName("GuardR");
                bot.DisplayName = MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.RIME_GUARD)).ToString();
                MyScriptWrapper.SetEntityDestructible(bot, false);
                ((MySmallShipBot)bot).SleepDistance = 10000;
            }
        }

        private void MyScriptWrapperOnBotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (m_07followContact.IsAvailable() && waypoint.EntityId.Value.NumericValue == (uint)EntityID.WaypointFollow1 && bot == MyScriptWrapper.GetEntity("Mitchel"))
            {
                MissionTimer.RegisterTimerAction(5000, Follow1Reached, false);
            }

            MySmallShipBot vesselGuardL = (MySmallShipBot)MyScriptWrapper.TryGetEntity("GuardL");
            MySmallShipBot vesselGuardR = (MySmallShipBot)MyScriptWrapper.TryGetEntity("GuardR");

            if (m_waitForTheMoment.IsAvailable() && bot == vesselGuardL && waypoint.EntityId.Value.NumericValue == (uint)EntityID.WaypointGuardL)
            {
                m_guardLReached = true;             
            }
            if (m_waitForTheMoment.IsAvailable() && bot == vesselGuardR && waypoint.EntityId.Value.NumericValue == (uint)EntityID.WaypointGuardR)
            {
                m_guardRReached = true;
            }
        }

        private void Follow1Reached()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointFactory);
            MySmallShipBot mitchel = (MySmallShipBot)MyScriptWrapper.GetEntity("Mitchel");
            mitchel.SetWaypointPath("Follow2");
            mitchel.PatrolMode = MyPatrolMode.ONE_WAY;
            mitchel.Patrol();

            MyScriptWrapper.SetEntityEnabled((uint)EntityID.FactoryFound, true);
        }

        private void M01MeetReefOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 0, "KA01");
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();

            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnBotReachedWaypoint += MyScriptWrapperOnBotReachedWaypoint;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress); // Sets music group to be played in the sector - no matter if the mission is running or not

            m_ravenguyBot = MyScriptWrapper.GetEntity("RavenGuy") as MySmallShipBot;
            m_ravengirlBot = MyScriptWrapper.GetEntity("RavenGirl") as MySmallShipBot;
            m_marcus = MyScriptWrapper.GetEntity("Marcus") as MySmallShipBot;
            m_reef = MyScriptWrapper.GetEntity((uint)EntityID.FrancisReef) as MySmallShipBot;
            m_barkeeper = MyScriptWrapper.GetEntity((uint)EntityID.Barkeeper) as MySmallShipBot;

            MyScriptWrapper.SetEntityEnabled((uint)EntityID.FactoryFound, false);
        }


        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnBotReachedWaypoint -= MyScriptWrapperOnBotReachedWaypoint;

            m_ravenguyBot = null;
            m_ravengirlBot = null;
            m_marcus = null;
            m_reef = null;
            m_barkeeper = null;
        }


        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }

    }
}

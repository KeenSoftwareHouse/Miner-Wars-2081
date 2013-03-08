using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyFortValiantMissionC : MyFortValiantMissionBase
    {

        #region EntityIDs
        private readonly List<uint> m_scanners1IDs = new List<uint>() { 137461, 137467, 123500, 123504, 137451, 137491, 137485, 137479, 137473, 137497, 137504, 137510, 137516, 137522 };
        private readonly List<uint> m_minesDummies = new List<uint>() { 164, 169, 170, 171, 172, 184, 185, 186, 187, 188, 202, 207, 206, 203, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249 };

        public override void ValidateIds()
        {
            base.ValidateIds();

            foreach (var entityIds in new List<List<uint>> { m_scanners1IDs, m_minesDummies })
            {
                foreach (var entityId in entityIds)
                {
                    MyScriptWrapper.GetEntity(entityId);    
                }
            }
        }
        #endregion

        private bool m_shootWarningSent = false;

        private MyObjective m_visitVendor;

        private MyEntity m_madelynDummy;
        private MyEntity m_madelyn;

        private MySmallShipBot m_royalBot;
        private MySmallShipBot m_templarBot;

        private MyUseObjective m_findArtifact;
        private MyObjectiveDisablePrefabs m_disableScanner2;
        private MyDeadlyScanners m_deadlyScanners;
        private List<MyInventoryItem> m_myInventory = new List<MyInventoryItem>();

        private MySmallShipBot m_ravenguyBot;
        private MySmallShipBot m_ravengirlBot;
        private MySmallShipBot m_marcus;
        

        public override void Load()
        {
            //if (!IsMainSector) return;
            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath += OnEntityDeath;
            MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged += Static_CameraContrlolledObjectChanged;



            m_templarBot = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar);
            m_royalBot = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BotRoyal);

            m_templarBot.SetWaypointPath("Templar");
            m_templarBot.PatrolMode = MyPatrolMode.CYCLE;
            m_templarBot.SpeedModifier = 0.25f;
            m_templarBot.Patrol();

            m_royalBot.SetWaypointPath("Royal");
            m_royalBot.PatrolMode = MyPatrolMode.CYCLE;
            m_royalBot.SpeedModifier = 0.25f;
            m_royalBot.Patrol();

            m_ravenguyBot = MyScriptWrapper.GetEntity("RavenGuy") as MySmallShipBot;
            m_ravengirlBot = MyScriptWrapper.GetEntity("RavenGirl") as MySmallShipBot;
            m_marcus = MyScriptWrapper.GetEntity("Marcus") as MySmallShipBot;



            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar));


            m_deadlyScanners = new MyDeadlyScanners(m_scanners1IDs, new List<int>() { 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000 });
            if (!MyScriptWrapper.IsMissionFinished(m_disableScanner2.ID)) 
                Components.Add(m_deadlyScanners);
            DisableEntities(new List<uint>() { (uint)EntityID.Scanner1, (uint)EntityID.Scanner2 });
            
            foreach (var mine in m_minesDummies)
            {
                MyScriptWrapper.GenerateMinesField<MyMineBasic>(MyScriptWrapper.GetEntity(mine),
                                                                MyMwcObjectBuilder_FactionEnum.Russian_KGB, 1, MyTexts.Mine,
                                                                MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS |
                                                                MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE |
                                                                MyHudIndicatorFlagsEnum.SHOW_TEXT |
                                                                MyHudIndicatorFlagsEnum.SHOW_DISTANCE |
                                                                MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER |
                                                                MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR
            );
            }

            if (MySession.Static.EventLog.IsMissionFinished(MyMissionID.FORT_VALIANT_C_FOLLOW_FIND_VENTILATION) && !MySession.Static.EventLog.IsMissionFinished(MyMissionID.FORT_VALIANT_C_CATACOMBS))
            {
                SetFriendsPatrol();
            }

            if (!MyMissions.GetMissionByID(MyMissionID.FORT_VALIANT_C_CAPTAIN).IsCompleted())
            {
                var startPosition = MyScriptWrapper.GetEntity((uint)EntityID.StartLocationC).GetPosition();
                MyScriptWrapper.Move(MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN)), startPosition);
                MyScriptWrapper.MovePlayerAndFriendsToHangar(this.RequiredActors);
            }
            

            base.Load();
        }

        private void Static_CameraContrlolledObjectChanged(MyEntity e)
        {
            if(e.EntityId.Value.NumericValue == (uint)EntityID.CameraLookThroughID)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.FORT_VALIANT_C_2000);
            }
        }


        private void MyScriptWrapperOnOnSpawnpointBotSpawned(MyEntity spawnPoint, MyEntity entity2)
        {
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnAlarm1 || spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnAlarm2)
            {
                var bot = (MySmallShipBot)entity2;
                bot.SpeedModifier = 0.5f;
            }
        }



        public MyFortValiantMissionC()
            : base()
        {
            ID = MyMissionID.FORT_VALIANT_C; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("09e-Fort Valiant C"); // Name of mission
            Name = MyTextsWrapperEnum.FORT_VALIANT_C;
            Description = MyTextsWrapperEnum.FORT_VALIANT_C_Description;
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocationC); //StartLocationC Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredMissions = new MyMissionID[] { MyMissionID.SLAVER_BASE_2 };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.FORT_VALIANT_C_PICK_UP_EQUIP };
            AchievementName = MySteamAchievementNames.Mission17_FortValiantDungeons;

            #region Objectives
            m_objectives = new List<MyObjective>();

            var speakCaptain = new MyMeetObjective(
                 (MyTextsWrapperEnum.FORT_VALIANT_C_CAPTAIN_Name),
                 MyMissionID.FORT_VALIANT_C_CAPTAIN,
                 (MyTextsWrapperEnum.FORT_VALIANT_C_CAPTAIN_Description),
                 this,
                 new MyMissionID[] { },
                 (uint)EntityID.CaptainDummy,
                 (uint)EntityID.Captain,
                 50,
                 0.25f,
                 MyDialogueEnum.FORT_VALIANT_C_0100,
                 null,
                 false
            ) { SaveOnSuccess = true, FollowMe = false, StartDialogId = MyDialogueEnum.FORT_VALIANT_C_0100_BEGIN, HudName = MyTextsWrapperEnum.HudCaptainCedric };
            speakCaptain.OnMissionSuccess += SpeakCaptainOnOnMissionSuccess;
            m_objectives.Add(speakCaptain);


            var reachUpperFloor = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_UPPER_FLOOR_Name),
                MyMissionID.FORT_VALIANT_C_UPPER_FLOOR,
                (MyTextsWrapperEnum.FORT_VALIANT_C_UPPER_FLOOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_CAPTAIN },
                new MyMissionLocation(baseSector, (uint)EntityID.UpperFloorDummy)
                ) { HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(reachUpperFloor);



            var meetRoyal = new MyMeetObjective(
                 (MyTextsWrapperEnum.FORT_VALIANT_C_EQUIP_TALK_Name),
                 MyMissionID.FORT_VALIANT_C_EQUIP_TALK,
                 (MyTextsWrapperEnum.FORT_VALIANT_C_EQUIP_TALK_Description),
                 this,
                 new MyMissionID[] { MyMissionID.FORT_VALIANT_C_UPPER_FLOOR },
                 (uint)EntityID.RoyalDummy,
                 (uint)EntityID.BotRoyal,
                 50,
                 0.25f,
                 MyDialogueEnum.FORT_VALIANT_C_0200,
                 null,
                 false
            ) { SaveOnSuccess = true, FollowMe = false };
            m_objectives.Add(meetRoyal);


            var giveoutEquipment = new MyUseObjective(
                   (MyTextsWrapperEnum.FORT_VALIANT_C_EQUIP_Name),
                   MyMissionID.FORT_VALIANT_C_EQUIP,
                   (MyTextsWrapperEnum.FORT_VALIANT_C_EQUIP_Description),
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.FORT_VALIANT_C_EQUIP_TALK },
                   new MyMissionLocation(baseSector, (uint)EntityID.RoyalCargoBoxDetector),
                   MyTextsWrapperEnum.PressToGiveEquipment,
                   MyTextsWrapperEnum.CargoBox,
                   MyTextsWrapperEnum.TransferInProgress,
                   3000,
                   MyUseObjectiveType.Activating
            ) { SaveOnSuccess = true };
            giveoutEquipment.OnMissionSuccess += GiveoutEquipmentOnOnMissionSuccess;
            m_objectives.Add(giveoutEquipment);



            var meetOfficers = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_MEET_OFFICIALS_Name),
                MyMissionID.FORT_VALIANT_C_MEET_OFFICIALS,
                (MyTextsWrapperEnum.FORT_VALIANT_C_MEET_OFFICIALS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_EQUIP },
                new MyMissionLocation(baseSector, (uint)EntityID.OfficalsDetector),
                null
                ) { StartDialogId = MyDialogueEnum.FORT_VALIANT_C_0300, SuccessDialogId = MyDialogueEnum.FORT_VALIANT_C_0400, HudName = MyTextsWrapperEnum.HudChamber };
            m_objectives.Add(meetOfficers);


            var leaveOfficers = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_LEAVE_OFFICIALS_Name),
                MyMissionID.FORT_VALIANT_C_LEAVE_OFFICIALS,
                (MyTextsWrapperEnum.FORT_VALIANT_C_LEAVE_OFFICIALS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_MEET_OFFICIALS },
                new MyMissionLocation(baseSector, (uint)EntityID.OfficalsleaveDetector),
                null
                ) { SuccessDialogId = MyDialogueEnum.FORT_VALIANT_C_0500, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(leaveOfficers);

            var followPath = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_LEAVE_FOLLOW_Name),
                MyMissionID.FORT_VALIANT_C_LEAVE_FOLLOW,
                (MyTextsWrapperEnum.FORT_VALIANT_C_LEAVE_FOLLOW_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_LEAVE_OFFICIALS },
                new MyMissionLocation(baseSector, (uint)EntityID.FollowPathDetector),
                null,
                null) { HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(followPath);

            var speakWithSir = new MyMeetObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_SPEAK_SIR_Name),
                MyMissionID.FORT_VALIANT_C_SPEAK_SIR,
                 (MyTextsWrapperEnum.FORT_VALIANT_C_SPEAK_SIR_Description),
                 this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_LEAVE_FOLLOW },
                 (uint)EntityID.SirBendivereDummy,
                 (uint)EntityID.BotTemplar,
                 50,
                 0.25f,
                 MyDialogueEnum.FORT_VALIANT_C_0600,
                 null,
                 false
            ) { SaveOnSuccess = true, FollowMe = false, HudName = MyTextsWrapperEnum.HudSirGeraint };
            m_objectives.Add(speakWithSir);


            var getItemsFromCargoBoxes = new MyObjectiveGetItems(
                (MyTextsWrapperEnum.FORT_VALIANT_C_GET_EQUP_CARGO_Name),
                MyMissionID.FORT_VALIANT_C_GET_EQUP_CARGO,
                (MyTextsWrapperEnum.FORT_VALIANT_C_GET_EQUP_CARGO_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_SPEAK_SIR },
                new List<MyItemToGetDefinition>() {
                            new MyItemToGetDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_1)},
                new List<uint>() { (uint)EntityID.CargoBox }
            ) { SaveOnSuccess = true };
            m_objectives.Add(getItemsFromCargoBoxes);



            var findEntrance = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_FOLLOW_FIND_VENTILATION_Name),
                MyMissionID.FORT_VALIANT_C_FOLLOW_FIND_VENTILATION,
                (MyTextsWrapperEnum.FORT_VALIANT_C_FOLLOW_FIND_VENTILATION_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_GET_EQUP_CARGO },
                new MyMissionLocation(baseSector, (uint)EntityID.Ventilation),
                null,
                null) { StartDialogId = MyDialogueEnum.FORT_VALIANT_C_0700, HudName = MyTextsWrapperEnum.HudEntrance };
            findEntrance.OnMissionLoaded += FindEntranceOnOnMissionLoaded;
            m_objectives.Add(findEntrance);
            

            var disableScanner = new MyObjectiveDisablePrefabs(
                  (MyTextsWrapperEnum.FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE_Name),
                  MyMissionID.FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE,
                  (MyTextsWrapperEnum.FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.FORT_VALIANT_C_FOLLOW_FIND_VENTILATION },
                    new List<uint> { (uint)EntityID.DisableScanner },
                    new List<uint> { (uint)EntityID.DisableScannerHub },
                    false,
                    false
                ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.FORT_VALIANT_C_0800, HudName = MyTextsWrapperEnum.HudSecurityHub };
            disableScanner.OnMissionLoaded += DisableScannerOnOnMissionLoaded;
            m_objectives.Add(disableScanner);

            var scanners1 = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS1_Name),
                MyMissionID.FORT_VALIANT_C_SCANNERS1,
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE },
                new MyMissionLocation(baseSector, (uint)EntityID.ScannersLocation1)) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.FORT_VALIANT_C_1000, HudName = MyTextsWrapperEnum.Nothing };
            //scanners1.Components.Add(new MyDetectorDialogue((uint)EntityID.DetectorGuardsDialogue, MyDialogueEnum.FORT_VALIANT_C_1100));
            scanners1.Components.Add(new MyDetectorDialogue((uint)EntityID.DetectorSensorsDialogue, MyDialogueEnum.FORT_VALIANT_C_1200));
            m_objectives.Add(scanners1);


            var scanners2 = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS2_Name),
                MyMissionID.FORT_VALIANT_C_SCANNERS2,
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_SCANNERS1 },
                new MyMissionLocation(baseSector, (uint)EntityID.ScannersLocation2)) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            scanners2.OnMissionSuccess += Scanners2OnOnMissionSuccess;
            m_objectives.Add(scanners2);

            
            var scanners23 = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS23_Name),
                MyMissionID.FORT_VALIANT_C_SCANNERS23,
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS23_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_SCANNERS2 },
                new MyMissionLocation(baseSector, (uint)EntityID.ScannersLocation23)) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(scanners23);
            
            m_findArtifact = new MyUseObjective(
               (MyTextsWrapperEnum.FORT_VALIANT_C_TAKE_ARTEFACT_Name),
               MyMissionID.FORT_VALIANT_C_TAKE_ARTEFACT,
               (MyTextsWrapperEnum.FORT_VALIANT_C_TAKE_ARTEFACT_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.FORT_VALIANT_C_SCANNERS23 },
               null,
               MyTextsWrapperEnum.NotificationTakeArtifact,
               MyTextsWrapperEnum.Artifact,
               MyTextsWrapperEnum.TakingInProgress,
               3000,
               MyUseObjectiveType.Taking,
               null,
               null,
               null,
               new List<uint>() { (uint)EntityID.Box1Marker, (uint)EntityID.Box2Marker, (uint)EntityID.Box3Marker, (uint)EntityID.Box4Marker },
               (uint)EntityID.ArtifactDummy
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.FORT_VALIANT_C_1400 };
            m_findArtifact.OnMissionLoaded += FindArtifactOnOnMissionLoaded;
            m_findArtifact.OnMissionSuccess += FindArtifactOnOnMissionSuccess;
            m_findArtifact.Components.Add(new MyTimedDialogue(new TimeSpan(0,0,10),MyDialogueEnum.FORT_VALIANT_C_1500 ));
            m_objectives.Add(m_findArtifact);


            var vault = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_VALUT_Name),
                MyMissionID.FORT_VALIANT_C_VALUT,
                (MyTextsWrapperEnum.FORT_VALIANT_C_VALUT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_TAKE_ARTEFACT },
                new MyMissionLocation(baseSector, (uint)EntityID.ScannersLocation2)) { HudName = MyTextsWrapperEnum.Nothing };
            vault.Components.Add(new MyDetectorDialogue((uint)EntityID.DetectorSecurityFields, MyDialogueEnum.FORT_VALIANT_C_1700));
            vault.Components.Add(new MyDetectorDialogue((uint)EntityID.DetectorOpenDoors, MyDialogueEnum.FORT_VALIANT_C_1800));
            vault.Components.Add(new MyDetectorDialogue((uint)EntityID.DetectorComputer, MyDialogueEnum.FORT_VALIANT_C_1900));
            m_objectives.Add(vault);

            m_disableScanner2 = new MyObjectiveDisablePrefabs(
                  (MyTextsWrapperEnum.FORT_VALIANT_C_TURN_OFF_SCANNER_Name),
                  MyMissionID.FORT_VALIANT_C_TURN_OFF_SCANNER,
                  (MyTextsWrapperEnum.FORT_VALIANT_C_TURN_OFF_SCANNER_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.FORT_VALIANT_C_VALUT },
                    new List<uint> { (uint)EntityID.DisableSnanners2 },
                    new List<uint> { (uint)EntityID.DisableScanner2Hub },
                    false

                ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.FORT_VALIANT_C_2100, HudName = MyTextsWrapperEnum.HudSecurityHub };
            m_disableScanner2.OnMissionSuccess += DisableScanner2_Success;
            m_objectives.Add(m_disableScanner2);

            var scanners4 = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS4_Name),
                MyMissionID.FORT_VALIANT_C_SCANNERS4,
                (MyTextsWrapperEnum.FORT_VALIANT_C_SCANNERS4_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_TURN_OFF_SCANNER },
                new MyMissionLocation(baseSector, (uint)EntityID.HigherFloors)) { HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(scanners4);


            var elevator = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_TOP_ELEVATOR_Name),
                MyMissionID.FORT_VALIANT_C_TOP_ELEVATOR,
                (MyTextsWrapperEnum.FORT_VALIANT_C_TOP_ELEVATOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_SCANNERS4 },
                new MyMissionLocation(baseSector, (uint)EntityID.ElevatorDummy)) { HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(elevator);


            var escape = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_VENT_SYSTEM_Name),
                MyMissionID.FORT_VALIANT_C_VENT_SYSTEM,
                (MyTextsWrapperEnum.FORT_VALIANT_C_VENT_SYSTEM_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_TOP_ELEVATOR },
                new MyMissionLocation(baseSector, (uint)EntityID.EscapeVentSystemDummy)) { SuccessDialogId = MyDialogueEnum.FORT_VALIANT_C_2300, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(escape);


            var useCatacombs = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_CATACOMBS_Name),
                MyMissionID.FORT_VALIANT_C_CATACOMBS,
                (MyTextsWrapperEnum.FORT_VALIANT_C_CATACOMBS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_C_VENT_SYSTEM },
                new MyMissionLocation(baseSector, (uint)EntityID.EscapeCatacombsDummy)
                //startDialogId: dialog9
                ) { HudName = MyTextsWrapperEnum.Nothing };
            useCatacombs.OnMissionSuccess += UseCatacombsOnOnMissionSuccess;
            m_objectives.Add(useCatacombs);

            var getItemsFromCargoBoxes2 = new MyUseObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_C_PICK_UP_EQUIP_Name),
                MyMissionID.FORT_VALIANT_C_PICK_UP_EQUIP,
                (MyTextsWrapperEnum.FORT_VALIANT_C_PICK_UP_EQUIP_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.FORT_VALIANT_C_CATACOMBS },
               new MyMissionLocation(baseSector, (uint)EntityID.RoyalCargoBoxDetector),
               MyTextsWrapperEnum.PressToTakeCargo,
               MyTextsWrapperEnum.CargoBox,
               MyTextsWrapperEnum.TakingInProgress,
               3000,
               MyUseObjectiveType.Taking
               //startDialogId: dialog10
        ) { SaveOnSuccess = true,StartDialogId = MyDialogueEnum.FORT_VALIANT_C_2400};
            getItemsFromCargoBoxes2.Components.Add(new MyDetectorDialogue((uint)EntityID.GetItemsWayBackDialogue, MyDialogueEnum.FORT_VALIANT_C_2500));
            getItemsFromCargoBoxes2.OnMissionSuccess += GetItemsFromCargoBoxesOnOnMissionSuccess;
            m_objectives.Add(getItemsFromCargoBoxes2);
            #endregion
        }

        private void UseCatacombsOnOnMissionSuccess(MyMissionBase sender)
        {
            var sir = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar);
            sir.SetWaypointPath("ChurchPatrol");
            sir.PatrolMode = MyPatrolMode.ONE_WAY;
            sir.Patrol();

            m_marcus.Follow(MySession.PlayerShip);
            m_ravengirlBot.Follow(MySession.PlayerShip);
            m_ravenguyBot.Follow(MySession.PlayerShip);
        }

        private void GetItemsFromCargoBoxesOnOnMissionSuccess(MyMissionBase sender)
        {
            m_myInventory.Clear();
            
            var cargobox = (MyCargoBox)MyScriptWrapper.GetEntity((uint)EntityID.FakeCargo);
            m_myInventory.AddRange(cargobox.Inventory.GetInventoryItems());
            MyScriptWrapper.AddInventoryItems(MyScriptWrapper.GetPlayerInventory(), m_myInventory, ((MyPrefabHangar)MyScriptWrapper.GetMothershipHangar(MyScriptWrapper.GetEntity("Madelyn"))).Inventory);
            cargobox.Inventory.ClearInventoryItems(false);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.FORT_VALIANT_C_2600);
        }

        private void DisableScannerOnOnMissionLoaded(MyMissionBase sender)
        {
            var sir = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar);
            sir.SetWaypointPath("ChurchPatrol");
            sir.PatrolMode = MyPatrolMode.ONE_WAY;
            sir.Patrol();

            SetFriendsPatrol();
        }

        private void SetFriendsPatrol()
        {
            m_marcus.SetWaypointPath("ChurchPatrol");
            m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();

            m_ravengirlBot.SetWaypointPath("ChurchPatrol");
            m_ravengirlBot.PatrolMode = MyPatrolMode.ONE_WAY;
            m_ravengirlBot.Patrol();

            m_ravenguyBot.SetWaypointPath("ChurchPatrol");
            m_ravenguyBot.PatrolMode = MyPatrolMode.ONE_WAY;
            m_ravenguyBot.Patrol();
        }

        private void Scanners2OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled((MyScriptWrapper.GetEntity((uint)EntityID.DisableScanner)), true);
        }

        private void DisableScanner2_Success(MyMissionBase sender)
        {
            Components.Remove(m_deadlyScanners);
            MyScriptWrapper.SetEntitiesEnabled(m_scanners1IDs, false);

            var bots = MyScriptWrapper.GetSpawnPointBots((uint)EntityID.SpawnAlarm2);
            foreach (var bot in bots)
            {
                var smallShipBot = bot.Ship;
                smallShipBot.SetWaypointPath("group3");
                smallShipBot.PatrolMode = MyPatrolMode.ONE_WAY;
                smallShipBot.Patrol();
            }
        }

        private void FindArtifactOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.PrefabArtifact, false, this);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PrefabArtifact));

            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box1Marker));
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box2Marker));
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box3Marker));
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box4Marker));

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.FORT_VALIANT_C_1600);
        }

        private void FindArtifactOnOnMissionLoaded(MyMissionBase sender)
        {
            sender.SetLocationVisibility(true, MyScriptWrapper.GetEntity((uint)EntityID.Box1Marker), MyGuitargetMode.Objective);
            sender.SetLocationVisibility(true, MyScriptWrapper.GetEntity((uint)EntityID.Box2Marker), MyGuitargetMode.Objective);
            sender.SetLocationVisibility(true, MyScriptWrapper.GetEntity((uint)EntityID.Box3Marker), MyGuitargetMode.Objective);
            sender.SetLocationVisibility(true, MyScriptWrapper.GetEntity((uint)EntityID.Box4Marker), MyGuitargetMode.Objective);

            InitDetector((uint)EntityID.Box1Marker, Box1Entered);
            InitDetector((uint)EntityID.Box2Marker, Box2Entered);
            InitDetector((uint)EntityID.Box3Marker, Box3Entered);
            InitDetector((uint)EntityID.Box4Marker, Box4Entered);


            MyScriptWrapper.Highlight((uint)EntityID.PrefabArtifact, true, this);
        }

        private void Box4Entered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            sender.Off();
            if (m_findArtifact.IsAvailable()) m_findArtifact.MissionEntityIDs.Remove((uint)EntityID.Box4Marker);
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box4Marker));
        }

        private void Box3Entered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            sender.Off();
            if (m_findArtifact.IsAvailable()) m_findArtifact.MissionEntityIDs.Remove((uint)EntityID.Box3Marker);
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box3Marker));
        }

        private void Box2Entered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            sender.Off();
            if (m_findArtifact.IsAvailable()) m_findArtifact.MissionEntityIDs.Remove((uint)EntityID.Box2Marker);
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box2Marker));
        }

        private void Box1Entered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            sender.Off();
            if (m_findArtifact.IsAvailable()) m_findArtifact.MissionEntityIDs.Remove((uint)EntityID.Box1Marker);
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.Box1Marker));
        }

        private void FindEntranceOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnAlarm1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnAlarm2);

            var sir = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar);

            sir.SetWaypointPath("WayChurch");
            sir.PatrolMode = MyPatrolMode.ONE_WAY;
            sir.Patrol();


            m_marcus.SetWaypointPath("WayChurch");
            m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();

            m_ravengirlBot.SetWaypointPath("WayChurch");
            m_ravengirlBot.PatrolMode = MyPatrolMode.ONE_WAY;
            m_ravengirlBot.Patrol();

            m_ravenguyBot.SetWaypointPath("WayChurch");
            m_ravenguyBot.PatrolMode = MyPatrolMode.ONE_WAY;
            m_ravenguyBot.Patrol();

        }

        private void GiveoutEquipmentOnOnMissionSuccess(MyMissionBase sender)
        {
            m_myInventory.Clear();
            m_myInventory.AddRange(MyScriptWrapper.GetPlayerInventory().GetInventoryItems());
            var cargobox = (MyCargoBox)MyScriptWrapper.GetEntity((uint)EntityID.FakeCargo);

            var builders = MySession.PlayerShip.Weapons.GetWeaponsObjectBuilders(true);
            foreach (var myMwcObjectBuilderSmallShipWeapon in builders)
            {
                var item = MyInventory.CreateInventoryItemFromObjectBuilder(myMwcObjectBuilderSmallShipWeapon);
                m_myInventory.Add(item);
            }
            cargobox.Inventory.AddInventoryItems(m_myInventory);

            MySession.PlayerShip.Weapons.RemoveAllWeapons();
            var inventory = MyScriptWrapper.GetPlayerInventory();
            inventory.ClearInventoryItems(false);
        }

        private void SpeakCaptainOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Generator1), false);
        }

        public override void Unload()
        {
            //if (!IsMainSector) return;
            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
            MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged -= Static_CameraContrlolledObjectChanged;
            Components.Clear();

            m_ravenguyBot = null;
            m_ravengirlBot = null;
            m_marcus = null;
            m_templarBot = null;
            m_royalBot = null;

            base.Unload();
        }
        
        void OnEntityDeath(MyEntity entity, MyEntity killedBy)
        {
            if (MyScriptWrapper.IsPlayerShip(killedBy) && !m_shootWarningSent)
            {
                m_shootWarningSent = true;
                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.DontShoot, MyHudConstants.ENEMY_FONT, 10000));
                return;
            }

            if (MyScriptWrapper.IsPlayerShip(killedBy) && !MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_ASSAULT))
            {
                Fail(MyTextsWrapperEnum.DontShoot);
            }
        }
    }

}

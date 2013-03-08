using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyRussianTransmitterMission : MyMission
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 2835,
            OutpostSideEntrance = 2832,
            EnterTheBase = 7789,
            SabotageTurrets_1 = 2836,
            ReachWarehouse = 93853,
            StealMilitarySupply = 93862,
            FightRussianCommando = 93864,
            BackToTransmitter = 93865,
            SabotageTurrets_3 = 7081,
            FindMainRoomEntrance = 94327,
            PlaceDeviceOnTransmitter = 2837,
            EscapeTheOutpost_1 = 7082,
            EscapeTheOutpost_2 = 6169,
            VolodiaCommandoSpawnPoint = 94842,
            VolodiaCommandoSpawnPoint2 = 124834,
            CommandoSpawnPoint = 94839,
            Mothership = 95432,
            Hub1 = 6499,
            Hub2 = 7078,
            VolodiaDummy = 97258,
            CargoBox1 = 93859,
            MothershipEntity = 95433,
            Transmitter1 = 2500,
            Transmitter2 = 2532,
            Cargospawn = 123223,
            Transmitter1spawn = 123224,

            ShipVolodia = 107656,

            Particle1 = 16779409,
            Particle2 = 16779410,
            Particle3 = 16779411,
            Particle4 = 16779438,

            Hub3 = 7085,
            Door1 = 272,
            Door2 = 273,
            Door3 = 274,
            Door4 = 275,

            DummyInsideBase = 15,
            DummyNearFrequencyHub = 57,
            DummyNearVolodia = 61,
            DummyNearCargo = 59,
            DummyOutsideTransmitter = 60,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)(((EntityID?) value).Value));
            }
        }

        private MyObjective m_fightRussianCommando;
        private MyObjective m_tradeWithVolodia;
        private MyObjectiveDialog m_strangerContact;

        public MyRussianTransmitterMission()
        {
            ID = MyMissionID.RUSSIAN_TRANSMITTER; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("17-RUF transmitter");
            Name = MyTextsWrapperEnum.RUSSIAN_TRANSMITTER;
            Description = MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_Description;
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-4988032, 0, -865747); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_BACK_TO_MADELYN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            #region Objectives

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            
            var introDialogue = new MyObjectiveDialog(
                MyMissionID.RUSSIAN_TRANSMITTER_INTRO_DIALOGUE,
                null,
                this,
                new MyMissionID[] { },
                MyDialogueEnum.RUSSIAN_TRANSMITTER_0100_INTRO);
            m_objectives.Add(introDialogue);

            // START OF REACH OBJECTIVE SUBMISSION DEFINITION
            var reachSideEntrance = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_REACH_SIDE_ENTRANCE, // Name of the submission
                MyMissionID.RUSSIAN_TRANSMITTER_REACH_SIDE_ENTRANCE, // ID of the submission - must be added to MyMissions.cs
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_REACH_SIDE_ENTRANCE_Description, // Description of the submission
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_INTRO_DIALOGUE }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.OutpostSideEntrance) // ID of dummy point of checkpoint
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudEntrance }; // False means do not save game in that checkpoint
            m_objectives.Add(reachSideEntrance); // Adding this submission to the list of submissions of current mission
            // END OF REACH OBJECTIVE SUBMISSION DEFINITION


            var enterTheBase = new MyObjective(
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_ENTER_THE_BASE,
                  MyMissionID.RUSSIAN_TRANSMITTER_ENTER_THE_BASE,
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_ENTER_THE_BASE_Description,
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_REACH_SIDE_ENTRANCE },
                  new MyMissionLocation(baseSector, (uint)EntityID.EnterTheBase)
              ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudEntrance };
            enterTheBase.OnMissionLoaded += new MissionHandler(enterTheBase_OnMissionLoaded);
            enterTheBase.OnMissionSuccess += new MissionHandler(enterTheBase_OnMissionSuccess);
            m_objectives.Add(enterTheBase);


            var findFrequency = new MyUseObjective(
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIND_FREQUENCY,
                  MyMissionID.RUSSIAN_TRANSMITTER_FIND_FREQUENCY,
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIND_FREQUENCY_Description,
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_ENTER_THE_BASE },
                  new MyMissionLocation(baseSector, (uint)EntityID.SabotageTurrets_1),
                  MyTextsWrapperEnum.PressToDownloadData,
                  MyTextsWrapperEnum.SecurityControlHUB,
                  MyTextsWrapperEnum.DownloadingData,
                  10000,
                  MyUseObjectiveType.Hacking
              ) { SaveOnSuccess = false };
            findFrequency.OnMissionLoaded += new MissionHandler(findFrequency_OnMissionLoaded);
            findFrequency.OnMissionSuccess += FindFrequency_Success;
            m_objectives.Add(findFrequency);

            m_strangerContact = new MyObjectiveDialog(
                MyTextsWrapperEnum.Null,
                MyMissionID.RUSSIAN_TRANSMITTER_STRANGER_CONTACT,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_FIND_FREQUENCY },
                MyDialogueEnum.RUSSIAN_TRANSMITTER_0500_0600_HACKPROBLEM_STRANGERCALLS
            );
            m_objectives.Add(m_strangerContact);

            var meetStranger = new MyObjective(
                 MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_MEET_STRANGER,
                 MyMissionID.RUSSIAN_TRANSMITTER_MEET_STRANGER,
                 MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_MEET_STRANGER_Description,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_STRANGER_CONTACT },
                 new MyMissionLocation(baseSector, (uint)EntityID.ShipVolodia),
                 radiusOverride: 90f
             ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudStranger };
            meetStranger.OnMissionLoaded += MeetStranger_Loaded;
            meetStranger.OnMissionSuccess += MeetStranger_Success;
            m_objectives.Add(meetStranger);



            var reachWarehouse = new MyObjective(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_REACH_WAREHOUSE,
               MyMissionID.RUSSIAN_TRANSMITTER_REACH_WAREHOUSE,
               MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_REACH_WAREHOUSE_Description,
               null,
               this,
               new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_MEET_STRANGER },
               new MyMissionLocation(baseSector, (uint)EntityID.ReachWarehouse)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudWarehouse };
            reachWarehouse.OnMissionLoaded += new MissionHandler(ReachWarehouse_Loaded);
            m_objectives.Add(reachWarehouse);


            var stealMilitarySupply = new MyUseObjective(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_STEAL_MILITARY_SUPPLY,
                MyMissionID.RUSSIAN_TRANSMITTER_STEAL_MILITARY_SUPPLY,
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_STEAL_MILITARY_SUPPLY_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_REACH_WAREHOUSE },
                new MyMissionLocation(baseSector, (uint)EntityID.StealMilitarySupply),
                MyTextsWrapperEnum.PressToTakeCargo,
                MyTextsWrapperEnum.CargoBox,
                MyTextsWrapperEnum.TakingInProgress,
                1000,
                MyUseObjectiveType.Activating
            ) { SaveOnSuccess = false };
            stealMilitarySupply.OnMissionSuccess += new MissionHandler(StealMilitarySupply_Success);
            m_objectives.Add(stealMilitarySupply);


            m_tradeWithVolodia = new MyObjective(
               MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_TRADE_WITH_VOLODIA,
               MyMissionID.RUSSIAN_TRANSMITTER_TRADE_WITH_VOLODIA,
               MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_TRADE_WITH_VOLODIA_Description,
               null,
               this,
               new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_STEAL_MILITARY_SUPPLY },
               new MyMissionLocation(baseSector, (uint)EntityID.ShipVolodia),
               radiusOverride: 90
            ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.RUSSIAN_TRANSMITTER_1000_VOLODIA_FOUND, HudName = MyTextsWrapperEnum.Volodia };
            m_tradeWithVolodia.OnMissionSuccess += TradeWithVolodia_Success;
            m_objectives.Add(m_tradeWithVolodia);


            m_fightRussianCommando = new MyObjectiveDestroy(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIGHT_RUSSIAN_COMMANDO,
               MyMissionID.RUSSIAN_TRANSMITTER_FIGHT_RUSSIAN_COMMANDO,
               MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIGHT_RUSSIAN_COMMANDO_Description,
               null,
               this,
               new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_TRADE_WITH_VOLODIA },
               //new List<uint> { (uint)EntityID.MothershipEntity },
               new List<uint> { },
               new List<uint> { (uint)EntityID.CommandoSpawnPoint },
               false
            ) { SaveOnSuccess = false, SuccessDialogId = MyDialogueEnum.RUSSIAN_TRANSMITTER_1400_RETREAT, HudName = MyTextsWrapperEnum.Nothing };
            m_fightRussianCommando.OnMissionLoaded += RussianCommandoLoaded;
            m_fightRussianCommando.OnMissionSuccess += new MissionHandler(FightRussianCommando_Success);
            m_objectives.Add(m_fightRussianCommando);


            var backToTransmitter = new MyObjective(
                    MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_BACK_TO_TRANSMITTER,
                   MyMissionID.RUSSIAN_TRANSMITTER_BACK_TO_TRANSMITTER,
                   MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_BACK_TO_TRANSMITTER_Description,
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_FIGHT_RUSSIAN_COMMANDO }, // prerekvizita - OPRAVIT na survive!!!
                   new MyMissionLocation(baseSector, (uint)EntityID.BackToTransmitter)
               ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudTransmitter };
            backToTransmitter.OnMissionLoaded += new MissionHandler(BackToTransmitter_Loaded);
            m_objectives.Add(backToTransmitter);


            var decryptFrequency = new MyUseObjective(
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_DECRYPT_FREQUENCY,
                  MyMissionID.RUSSIAN_TRANSMITTER_DECRYPT_FREQUENCY,
                  MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_DECRYPT_FREQUENCY_Description,
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_BACK_TO_TRANSMITTER },
                  new MyMissionLocation(baseSector, (uint)EntityID.SabotageTurrets_1),
                  MyTextsWrapperEnum.PressToHack,
                  MyTextsWrapperEnum.SecurityControlHUB,
                  MyTextsWrapperEnum.HackingProgress,
                  5000,
                  MyUseObjectiveType.Hacking
              ) { SaveOnSuccess = false, SuccessDialogId = MyDialogueEnum.RUSSIAN_TRANSMITTER_1600_ITSWORKING };
            m_objectives.Add(decryptFrequency);


            var uploadData = new MyUseObjective(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_UPLOAD_DATA,
                 MyMissionID.RUSSIAN_TRANSMITTER_UPLOAD_DATA,
                 MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_UPLOAD_DATA_Description,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_DECRYPT_FREQUENCY },
                 new MyMissionLocation(baseSector, (uint)EntityID.SabotageTurrets_3),
                 MyTextsWrapperEnum.PressToHack,
                 MyTextsWrapperEnum.SecurityControlHUB,
                 MyTextsWrapperEnum.HackingProgress,
                 5000
             ) { SaveOnSuccess = false, SuccessDialogId = MyDialogueEnum.RUSSIAN_TRANSMITTER_1700_UPLOADINGSIGNAL };
            m_objectives.Add(uploadData);


            var findMainRoomEntrance = new MyObjective(
                 MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIND_MAIN_ROOM_ENTRANCE,
                 MyMissionID.RUSSIAN_TRANSMITTER_FIND_MAIN_ROOM_ENTRANCE,
                 MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_FIND_MAIN_ROOM_ENTRANCE_Description,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_UPLOAD_DATA },
                 new MyMissionLocation(baseSector, (uint)EntityID.FindMainRoomEntrance)
             ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudEntrance };
            findMainRoomEntrance.OnMissionLoaded += new MissionHandler(findMainRoomEntrance_Loaded);
            m_objectives.Add(findMainRoomEntrance);


            var placeDeviceOnTransmitter = new MyUseObjective(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_PLACE_DEVICE_ON_TRANSMITTER,
                MyMissionID.RUSSIAN_TRANSMITTER_PLACE_DEVICE_ON_TRANSMITTER,
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_PLACE_DEVICE_ON_TRANSMITTER_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_FIND_MAIN_ROOM_ENTRANCE },
                new MyMissionLocation(baseSector, (uint)EntityID.PlaceDeviceOnTransmitter),
                MyTextsWrapperEnum.PressToPlaceDevice,
                MyTextsWrapperEnum.Transmitter,
                MyTextsWrapperEnum.PlacementInProgress,
                10000,
                MyUseObjectiveType.Building
              ) { SaveOnSuccess = false };
            placeDeviceOnTransmitter.OnMissionSuccess += new MissionHandler(PlaceDeviceOnTransmitter_Success);
            m_objectives.Add(placeDeviceOnTransmitter);


            var openDoors = new MyObjectiveEnablePrefabs(
              MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_OPEN_DOORS,
              MyMissionID.RUSSIAN_TRANSMITTER_OPEN_DOORS,
              MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_OPEN_DOORS_Description,
              null,
              this,
              new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_PLACE_DEVICE_ON_TRANSMITTER },
              null,
              new List<uint> { (uint)EntityID.Hub3 },
              new List<uint> { (uint)EntityID.Door1, (uint)EntityID.Door2, (uint)EntityID.Door3, (uint)EntityID.Door4, }
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            openDoors.OnMissionSuccess += new MissionHandler(OpenDoors_Success);
            m_objectives.Add(openDoors);


            var backToMadelyn = new MyObjective(
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_BACK_TO_MADELYN,
                MyMissionID.RUSSIAN_TRANSMITTER_BACK_TO_MADELYN,
                MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_BACK_TO_MADELYN_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER_OPEN_DOORS },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            backToMadelyn.OnMissionSuccess += new MissionHandler(BackToMadelyn_Success);
            m_objectives.Add(backToMadelyn);

            #endregion

        }

        void enterTheBase_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0300_FINDHUB);
        }


        void FindMainRoomEntrance_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_1900_PLACEDEVICE);
            var detectorOutside = MyScriptWrapper.GetDetector((uint)EntityID.DummyOutsideTransmitter);
            detectorOutside.Off();

            sender.Off();
        }

        void detectorOutsideTransmitter_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_1800_BADROUTE);
            sender.Off();
        }

        void findMainRoomEntrance_Loaded(MyMissionBase sender)
        {
            var detectorFindMainRoomEntrance = MyScriptWrapper.GetDetector((uint)EntityID.FindMainRoomEntrance);
            detectorFindMainRoomEntrance.On();

            var detectorOutside = MyScriptWrapper.GetDetector((uint)EntityID.DummyOutsideTransmitter);
            detectorOutside.On();

        }

        void ReachWarehouse_Loaded(MyMissionBase sender)
        {
            var detector = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearCargo);
            detector.On();  
        }

        void DummyNearCargo_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0800_NEARCARGO);
                sender.Off();
            }
        }

        void MeetStranger_Loaded(MyMissionBase sender)
        {
            var detector = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearVolodia);
            detector.OnEntityEnter += new OnEntityEnter(DummyNearVolodia_Enter);
            detector.On();     
        }

        void DummyNearVolodia_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0700_STRANGERPROPOSAL);
                sender.Off();
            }
        }

        void findFrequency_OnMissionLoaded(MyMissionBase sender)
        {
            var detector = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearFrequencyHub);
            detector.OnEntityEnter += new OnEntityEnter(DummyNearFrequencyHub_Enter);
            detector.On(); 
        }

        void DummyNearFrequencyHub_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                if (m_tradeWithVolodia.IsCompleted())
                {
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_1500_IFITDOESNOTWORK);
                    sender.Off();
                }
                else
                {
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0400_HUBFOUND);
                    sender.Off();
                }
            }
        }

        void enterTheBase_OnMissionLoaded(MyMissionBase sender)
        {
            var detector = MyScriptWrapper.GetDetector((uint)EntityID.DummyInsideBase);
            detector.On();
        }

        public override void Accept()
        {
            base.Accept();
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Mothership));
        }



        void BackToMadelyn_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_2200_THOMASCHAT);
            MissionTimer.ClearActions();
        }


        void BackToTransmitter_Loaded(MyMissionBase sender)
        {
            MissionTimer.RegisterTimerAction(TimeSpan.FromMinutes(5), () => Fail(MyTextsWrapperEnum.Fail_TimeIsUp), false);
        }

        void FightRussianCommando_Success(MyMissionBase sender)
        {
            MyScriptWrapper.TryHide((uint)EntityID.ShipVolodia);
        }

        void OpenDoors_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_2100_WEMADEIT);
            MyScriptWrapper.TryHide((uint)EntityID.Particle1);
            MyScriptWrapper.TryHide((uint)EntityID.Particle2);
            MyScriptWrapper.TryHide((uint)EntityID.Particle3);
            MyScriptWrapper.TryHide((uint)EntityID.Particle4);
        }

        void MeetStranger_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0700_VOLODIAINTRO);
            MyScriptWrapper.SetEntityDisplayName((uint)EntityID.ShipVolodia, MyTextsWrapper.Get(MyTextsWrapperEnum.Volodia).ToString());
        }

        void FindFrequency_Success(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Transmitter1spawn);
        }



        void StealMilitarySupply_Success(MyMissionBase sender)
        {
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.CargoBox1));

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0900_VOLODIA_RANT);

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Cargospawn);
            MyScriptWrapper.Move(MyScriptWrapper.GetEntity((uint)EntityID.ShipVolodia), MyScriptWrapper.GetEntity((uint)EntityID.VolodiaDummy).GetPosition());
        }

        void PlaceDeviceOnTransmitter_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_2000_DEVICEWORKING);
            MyScriptWrapper.Highlight((uint)EntityID.Transmitter1, false, this);
            MyScriptWrapper.Highlight((uint)EntityID.Transmitter2, false, this);
            MyScriptWrapper.Highlight((uint)EntityID.PlaceDeviceOnTransmitter, false, this);
            MyScriptWrapper.EnablePhysics((uint)EntityID.PlaceDeviceOnTransmitter, true);
        }

        void TradeWithVolodia_Success(MyMissionBase sender)
        {
            MyScriptWrapper.GetEntity((uint)EntityID.Hub1).Enabled = true;
            MyScriptWrapper.GetEntity((uint)EntityID.Hub2).Enabled = true;

            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Mothership));
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CommandoSpawnPoint); //aktivace spawnpointu pri startu mise   
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.VolodiaCommandoSpawnPoint); //aktivace spawnpointu pri startu mise
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.VolodiaCommandoSpawnPoint2); //aktivace spawnpointu pri startu mise    

            var detector = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearFrequencyHub);
            detector.On();
        }


        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();

            // Ensure correct game state
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian, MyFactions.RELATION_WORST);

            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 3); // Sets music group to be played in the sector - no matter if the mission is running or not

            MyScriptWrapper.Highlight((uint)EntityID.CargoBox1, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.Transmitter1, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.Transmitter2, true, this);

            var detectorNearFrequencyHub = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearFrequencyHub);
            detectorNearFrequencyHub.OnEntityEnter += new OnEntityEnter(DummyNearFrequencyHub_Enter);

            var detectorNearCargo = MyScriptWrapper.GetDetector((uint)EntityID.DummyNearCargo);
            detectorNearCargo.OnEntityEnter += new OnEntityEnter(DummyNearCargo_Enter);

            var detectorInsideBase = MyScriptWrapper.GetDetector((uint)EntityID.DummyInsideBase);
            detectorInsideBase.OnEntityEnter += new OnEntityEnter(MyRussianTransmitterMission_OnEntityEnter);

            var detectorOutsideTransmitter = MyScriptWrapper.GetDetector((uint)EntityID.DummyOutsideTransmitter);
            detectorOutsideTransmitter.OnEntityEnter += new OnEntityEnter(detectorOutsideTransmitter_OnEntityEnter);

            var detectorFindMainRoomEntrance = MyScriptWrapper.GetDetector((uint)EntityID.FindMainRoomEntrance);
            detectorFindMainRoomEntrance.OnEntityEnter += new OnEntityEnter(FindMainRoomEntrance_OnEntityEnter);
        }


        void MyRussianTransmitterMission_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_0200_BACKDOOR);
                sender.Off();
            }
        }


        void RussianCommandoLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_1100_MADELYNSCARED);
            MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(5), () => MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_TRANSMITTER_1300_APOLLOSCARED), false);
            MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(65), () => m_fightRussianCommando.Success(), false);
        }

    }

}


using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio;
using System.Diagnostics;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.Resources;
using System;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Audio.Dialogues;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyEACTransmitterMission : MyMission
    {
        
        #region Enums
        private enum EntityID
        {
            StartLocation = 41632,
            DummyCargoBomb = 41372,
            DoorCargoBomb = 43436,
            DummyControlRoom = 41376,
            HubA = 53255,
            DoorHubA = 34864,
            HubB = 53256,
            DoorHubB1 = 33840,
            DoorHubB2 = 33702,
            DoorHubB3 = 33789,
            DoorHubB4 = 33705,
            HubC = 53257,
            DoorHubC1 = 33922,
            DoorHubC2 = 33895,
            DoorHubC3 = 33912,
            DoorHubC4 = 33896,

            DummySatelliteA = 41375,

            AlarmAutodestruct1 = 33656,
            AlarmAutodestruct2 = 41012,
            PanelAutodestruct = 43395,
            TurretAutodescrut1 = 53630,
            TurretAutodescrut2 = 53633,
            TurretAutodescrut3 = 53636,
            TurretAutodescrut4 = 53639,

            DummyAutodestructHack = 54437,

            PrefabAutodestruct1 = 53271,
            PrefabAutodestruct2 = 53274,
            PrefabAutodestruct3 = 53259,
            PrefabAutodestruct4 = 53268,

            SpawnAutodestruct = 53276,
            DummyNearPrefabAutodestruct3 = 54837,
            DummyNearPrefabAutodestruct4 = 54836,

            DummySatelliteB = 41374,
            DummySatelliteC = 41373,
            DummySatelliteD = 42456,

            HubSolarArm = 53278,
            DoorSolarArm = 34714,

            CargoRepair = 55,
            DummyCargoRepair = 54036,
            DummyGeneratorFix = 53281,
            GeneratorFix = 53279,

            DummySolarCircuit = 41377,

            DummyReturn = 52594,

            WaypointAutodestruct1 = 42084,
            WaypointAutodestruct2 = 42079,

            WaypointControlRoom1 = 42388,
            WaypointControlRoom2 = 42304,

            //detectors and spawnpoints porn
            DetectorBeforeEnteringCargoWithRavens = 57425,
            DetectorBeforeEnteringControlRoomFromOutside = 57428,
            DetectorBeforeEnteringSatelitteA = 57431,
            DetectorWhenEscapingSatAThruDestroyedPanel = 57433,
            DetectorBeforeAgregat1 = 57438,
            DetectorBeforeAgregat2 = 57441,
            DetectorBeforeAgregat3 = 57444,
            DetectorBeforeHackingSatelitteBandC = 57447,
            DetectorBeforeStartingTransmission = 57449,
            DetectorBeforeOpeningDoorToArm = 57453,
            DetectorBeforeEnteringCargoThruArm = 57456,
            DetectorBeforeFixingGenerator = 57458,
            DetectorBeforeTurnBackOnSolars = 57461,

            SpawnpointAtInnerCargo = 57427,
            SpawnpointAtOuterControlRoom1 = 57430,
            SpawnpointAtControlRoom1 = 42707,
            SpawnpointAtControlRoom2 = 42706,
            SpawnpointAtSatelitteA = 42978,
            SpawnpointAtOuterSatelitteA = 57435,
            SpawnpointAtControlRoom3 = 57436,
            SpawnpointAtControlRoom4 = 57437,
            SpawnpointAtAgregat1 = 57440,
            SpawnpointAtAgregat2 = 57443,
            SpawnpointAtAgregat3 = 57446,
            SpawnpointAtSatelitteB = 42977,
            SpawnpointAtSatelitteC = 42976,
            SpawnpointAtControlRoom5 = 57451,
            SpawnpointAtControlRoom6 = 57452,
            SpawnpointAtArmEntrance = 57455,
            SpawnpointAtCargoInnerEntrance = 42979,
            SpawnpointAtGenerator = 57460,
            SpawnpointAtSolarStarting = 42980,
            SpawnpointAtControlRoom7 = 57463,
            SpawnpointAtControlRoom8 = 57464,
            SpawnpointAtOuterControlRoom2 = 57465,

            ParticleBombExplosion = 58057,
            PrefabBomb = 58658,

            InnerCargoDoor1 = 34991,
            InnerCargoDoor2 = 34996,

        }

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        #endregion

        public MyEACTransmitterMission()
        {
            ID = MyMissionID.EAC_TRANSMITTER;
            DebugName = new StringBuilder("21-EAC transmitter");
            Name = MyTextsWrapperEnum.EAC_TRANSMITTER;
            Description = MyTextsWrapperEnum.EAC_TRANSMITTER_Description; // "Set all 3 satellites to your needs.\n"
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(3818505, 0, -4273800);
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);



            RequiredMissions = new MyMissionID[] { MyMissionID.EAC_PRISON };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_MEETMS };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };
            
            m_objectives = new List<MyObjective>();

            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringCargoWithRavens, 0, (uint)EntityID.SpawnpointAtInnerCargo));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringCargoWithRavens, 0, (uint)EntityID.SpawnpointAtOuterControlRoom1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringControlRoomFromOutside, 0, (uint)EntityID.SpawnpointAtControlRoom1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringControlRoomFromOutside, 0, (uint)EntityID.SpawnpointAtControlRoom2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringSatelitteA, 0, (uint)EntityID.SpawnpointAtSatelitteA));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorWhenEscapingSatAThruDestroyedPanel, 0, (uint)EntityID.SpawnpointAtOuterSatelitteA));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorWhenEscapingSatAThruDestroyedPanel, 0, (uint)EntityID.SpawnpointAtControlRoom3));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorWhenEscapingSatAThruDestroyedPanel, 0, (uint)EntityID.SpawnpointAtControlRoom4));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeAgregat1, 0, (uint)EntityID.SpawnpointAtAgregat1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeAgregat2, 0, (uint)EntityID.SpawnpointAtAgregat2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeAgregat3, 0, (uint)EntityID.SpawnpointAtAgregat3));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeHackingSatelitteBandC, 0, (uint)EntityID.SpawnpointAtSatelitteB));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeHackingSatelitteBandC, 0, (uint)EntityID.SpawnpointAtSatelitteC));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeStartingTransmission, 0, (uint)EntityID.SpawnpointAtControlRoom5));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeStartingTransmission, 0, (uint)EntityID.SpawnpointAtControlRoom6));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeOpeningDoorToArm, 0, (uint)EntityID.SpawnpointAtArmEntrance));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringCargoThruArm, 0, (uint)EntityID.SpawnpointAtCargoInnerEntrance));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeFixingGenerator, 0, (uint)EntityID.SpawnpointAtGenerator));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeTurnBackOnSolars, 0, (uint)EntityID.SpawnpointAtSolarStarting));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeTurnBackOnSolars, 0, (uint)EntityID.SpawnpointAtControlRoom7));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeTurnBackOnSolars, 0, (uint)EntityID.SpawnpointAtControlRoom8));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeTurnBackOnSolars, 0, (uint)EntityID.SpawnpointAtOuterControlRoom2));

            var openCargoDoor = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_OPEN_CARGO_DOOR),
                MyMissionID.EAC_TRANSMITTER_OPEN_CARGO,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_OPEN_CARGO_DOOR_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyCargoBomb),
                MyTextsWrapperEnum.PressToPlaceBomb,
                MyTextsWrapperEnum.Blank,
                MyTextsWrapperEnum.PlacingBomb,
                1000,
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_0100
            ) { SaveOnSuccess = true };
            m_objectives.Add(openCargoDoor);
            openCargoDoor.OnMissionLoaded += new MissionHandler(openCargoDoor_OnMissionLoaded);
            openCargoDoor.OnMissionSuccess += new MissionHandler(openCargoDoor_OnMissionSuccess);


            var enterControlRoom = new MyObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_CENTRAL_ROOM),
                MyMissionID.EAC_TRANSMITTER_CENTRAL_ROOM,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_CENTRAL_ROOM_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_OPEN_CARGO },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyControlRoom),
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_0200
            ) { HudName = MyTextsWrapperEnum.HudControlRoom,
               SaveOnSuccess = true };
            enterControlRoom.OnMissionLoaded += new MissionHandler(enterControlRoom_OnMissionLoaded);
            enterControlRoom.OnMissionSuccess += new MissionHandler(enterControlRoom_OnMissionSuccess);
            m_objectives.Add(enterControlRoom);


            var unlockWayToSatelliteA = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES),
                MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_A,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES_Descrption),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_CENTRAL_ROOM },
                null,
                new List<uint> { (int)EntityID.HubA },
                new List<uint> { (int)EntityID.DoorHubA }
            ) { HudName = MyTextsWrapperEnum.HudHub,
                SaveOnSuccess = true };
            m_objectives.Add(unlockWayToSatelliteA);

            var unlockWayToSatelliteB = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES),
                MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_B,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES_Descrption),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_A },
                null,
                new List<uint> { (int)EntityID.HubB },
                new List<uint> { (int)EntityID.DoorHubB1, (int)EntityID.DoorHubB2, (int)EntityID.DoorHubB3, (int)EntityID.DoorHubB4}
            ) { HudName = MyTextsWrapperEnum.HudHub,
                SaveOnSuccess = true };
            m_objectives.Add(unlockWayToSatelliteB);

            var unlockWayToSatelliteC = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES),
                MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_C,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITES_Descrption),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_B },
                null,
                new List<uint> { (int)EntityID.HubC },
                new List<uint> { (int)EntityID.DoorHubC1, (int)EntityID.DoorHubC2, (int)EntityID.DoorHubC3, (int)EntityID.DoorHubC4}
            ) { HudName = MyTextsWrapperEnum.HudHub,
                SaveOnSuccess = true };
            m_objectives.Add(unlockWayToSatelliteC);


            var hackSatelliteA = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_A),
                MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_A,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_A_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_C },
                new MyMissionLocation(baseSector, (uint)EntityID.DummySatelliteA),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.HackingProgress,
                MyTextsWrapperEnum.Hacking,
                2000,
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_0300
            );
            m_objectives.Add(hackSatelliteA);


            var autodestructEscape = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_ESCAPE),
                MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_ESCAPE,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_ESCAPE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_A },
                new List<uint> { (uint)EntityID.PanelAutodestruct },
                startDialogID: MyDialogueEnum.EAC_TRANSMITTER_0400
            ) { HudName = MyTextsWrapperEnum.HudPanel};
            autodestructEscape.OnMissionLoaded += new MissionHandler(autodestructEscape_OnMissionLoaded);
            m_objectives.Add(autodestructEscape);


            var autodestructHack = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HACK),
                MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HACK,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HACK_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_ESCAPE },
                new MyMissionLocation(baseSector, (uint)EntityID.DummyAutodestructHack),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.Hacking,
                MyTextsWrapperEnum.HackingProgress,
                2000
            );
            m_objectives.Add(autodestructHack);

            //Escape by destroying panel upon your head
            var autodestructDestroy = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_DESTROY),
                MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_DESTROY,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_DESTROY_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HACK },
                new List<uint> { (uint)EntityID.PrefabAutodestruct1, (uint)EntityID.PrefabAutodestruct2 },
                startDialogID: MyDialogueEnum.EAC_TRANSMITTER_0500
            ) { SaveOnSuccess = true };
            autodestructDestroy.OnMissionLoaded += new MissionHandler(autodestructDestroy_OnMissionLoaded);
            autodestructDestroy.OnMissionSuccess += new MissionHandler(autodestructDestroy_OnMissionSuccess);
            m_objectives.Add(autodestructDestroy);

            var autodestructHelp = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HELP),
                MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HELP,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HELP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_DESTROY },
                null,
                new List<uint> { (uint)EntityID.SpawnAutodestruct },
                true,
                true,
                startDialogID: MyDialogueEnum.EAC_TRANSMITTER_0600,
                successDialogID: MyDialogueEnum.EAC_TRANSMITTER_0700
            ) { SaveOnSuccess = true };
            autodestructHelp.OnMissionSuccess += new MissionHandler(autodestructHelp_OnMissionSuccess);
            m_objectives.Add(autodestructHelp);


            var hackSatelliteB = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_B),
                MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_B,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_B_Descrption),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HELP },
                new MyMissionLocation(baseSector, (uint)EntityID.DummySatelliteB),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.HackingProgress,
                MyTextsWrapperEnum.Hacking,
                2000
            ) { SaveOnSuccess = true };
            m_objectives.Add(hackSatelliteB);


            var hackSatelliteC = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_C),
                MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_C,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_HACK_SATELLITE_C_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_B },
                new MyMissionLocation(baseSector, (uint)EntityID.DummySatelliteC),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.HackingProgress,
                MyTextsWrapperEnum.Hacking,
                2000,
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_0800
            ) { SaveOnSuccess = true };
            m_objectives.Add(hackSatelliteC);

            var startTransmittion = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_START_TRANSMISSION),
                MyMissionID.EAC_TRANSMITTER_START_TRANSMISSION,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_START_TRANSMISSION_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_HACK_SATELLITE_C },
                new MyMissionLocation(baseSector, (uint)EntityID.DummySatelliteD),
                MyTextsWrapperEnum.PressToStartTransmission,
                MyTextsWrapperEnum.StartingTransmission,
                MyTextsWrapperEnum.Transmission,
                1000,
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_0900
            );
            m_objectives.Add(startTransmittion);

            var openSolarArm = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_TRANSMITTER_OPEN_SOLAR_ARM),
                MyMissionID.EAC_TRANSMITTER_OPEN_SOLAR_ARM,
                (MyTextsWrapperEnum.EAC_TRANSMITTER_OPEN_SOLAR_ARM_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_START_TRANSMISSION },
                null,
                new List<uint> { (int)EntityID.HubSolarArm },
                new List<uint> { (int)EntityID.DoorSolarArm },
                startDialogId: MyDialogueEnum.EAC_TRANSMITTER_1000
            ) { HudName = MyTextsWrapperEnum.HudHub,
                SaveOnSuccess = true };
            openSolarArm.OnMissionLoaded += new MissionHandler(openSolarArm_OnMissionLoaded);
            m_objectives.Add(openSolarArm);



            var findRepairKit = new MyUseObjective(
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_FIND_REPAIR),
                  MyMissionID.EAC_TRANSMITTER_FIND_REPAIR,
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_FIND_REPAIR_Description),
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_OPEN_SOLAR_ARM },
                  new MyMissionLocation(baseSector, (uint)EntityID.CargoRepair),
                  MyTextsWrapperEnum.PressToTakeCargo,
                  MyTextsWrapperEnum.TakeAll,
                  MyTextsWrapperEnum.TakingInProgress,
                  2000,
                  radiusOverride: 50
              ) { HudName = MyTextsWrapperEnum.HudGeneratorRepairKit,
                  SaveOnSuccess = true };
            findRepairKit.OnMissionLoaded += new MissionHandler(findRepairKit_OnMissionLoaded);
            findRepairKit.OnMissionSuccess += new MissionHandler(findRepairKit_OnMissionSuccess);
            m_objectives.Add(findRepairKit);


            var fixGenerator = new MyUseObjective(
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_FIX_GENERATOR),
                  MyMissionID.EAC_TRANSMITTER_FIX_GENERATOR,
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_FIX_GENERATOR_Descrpition),
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_FIND_REPAIR },
                  new MyMissionLocation(baseSector, (uint)EntityID.DummyGeneratorFix),
                  MyTextsWrapperEnum.PressToStartGenerator,
                  MyTextsWrapperEnum.StartingProgress,
                  MyTextsWrapperEnum.StartingProgress,
                  2000,
                  startDialogId: MyDialogueEnum.EAC_TRANSMITTER_1100
              ) { SaveOnSuccess = true };
            fixGenerator.OnMissionSuccess += new MissionHandler(fixGenerator_OnMissionSuccess);
            m_objectives.Add(fixGenerator);

            var activateSolarpanels = new MyUseObjective(
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_ACTIVATE_SOLARPANELS),
                  MyMissionID.EAC_TRANSMITTER_ACTIVATE_SOLARPANELS,
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_ACTIVATE_SOLARPANELS_Description),
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_FIX_GENERATOR },
                  new MyMissionLocation(baseSector, (uint)EntityID.DummySolarCircuit),
                  MyTextsWrapperEnum.PressToRedirectEnergy,
                  MyTextsWrapperEnum.RedirectEnergy,
                  MyTextsWrapperEnum.RedirectionInProgress,
                  2000,
                  startDialogId: MyDialogueEnum.EAC_TRANSMITTER_1200
              );
            m_objectives.Add(activateSolarpanels);

            var restartTransmition = new MyUseObjective(
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_RESTART_TRANSMISSION),
                  MyMissionID.EAC_TRANSMITTER_RESTART_TRANSMISSION,
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_RESTART_TRANSMISSION_Description),
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_ACTIVATE_SOLARPANELS },
                  new MyMissionLocation(baseSector, (uint)EntityID.DummySatelliteD),
                  MyTextsWrapperEnum.PressToStartTransmission,
                  MyTextsWrapperEnum.StartingTransmission,
                  MyTextsWrapperEnum.Transmission,
                  2000,
                  startDialogId: MyDialogueEnum.EAC_TRANSMITTER_1300
              ) { SaveOnSuccess = true };
            m_objectives.Add(restartTransmition);


            var meetms = new MyObjective(
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_MEETMS),
                  MyMissionID.EAC_TRANSMITTER_MEETMS,
                  (MyTextsWrapperEnum.EAC_TRANSMITTER_MEETMS_Description),
                  null,
                  this,
                  new MyMissionID[] { MyMissionID.EAC_TRANSMITTER_RESTART_TRANSMISSION },
                  new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                  startDialogId: MyDialogueEnum.EAC_TRANSMITTER_1400,
                  radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
              ) { HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            m_objectives.Add(meetms);


  
        }

        void openSolarArm_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((uint)EntityID.InnerCargoDoor1), true);
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((uint)EntityID.InnerCargoDoor2), true);
        }

        void enterControlRoom_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.WaypointControlRoom1, (uint)EntityID.WaypointControlRoom2 }, false);
        }

        void enterControlRoom_OnMissionLoaded(MyMissionBase sender)
        {
            this.MissionTimer.RegisterTimerAction(6000, openCargoDoor_boom, false, MyTexts.ExplosionIn, false);
            MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.WaypointControlRoom1, (uint)EntityID.WaypointControlRoom2 }, true);
        }

        void autodestructDestroy_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetAlarmMode((uint)EntityID.AlarmAutodestruct1, false);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.AlarmAutodestruct2, false);

            MissionTimer.ClearActions();
        }

        void autodestructEscape_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DoorHubA), false);

            MyScriptWrapper.SetAlarmMode((uint)EntityID.AlarmAutodestruct1, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.AlarmAutodestruct2, true);

            MyScriptWrapper.TryUnhide((uint)EntityID.TurretAutodescrut1);
            MyScriptWrapper.TryUnhide((uint)EntityID.TurretAutodescrut2);
            MyScriptWrapper.TryUnhide((uint)EntityID.TurretAutodescrut3);
            MyScriptWrapper.TryUnhide((uint)EntityID.TurretAutodescrut4);

            MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.WaypointAutodestruct1, (uint)EntityID.WaypointAutodestruct2 }, true);

            MissionTimer.RegisterTimerAction(TimeSpan.FromMinutes(1), Autodestruct, false, MyTexts.AutodestructIn, false);
        }

        void Autodestruct()
        {
            Fail(MyTextsWrapperEnum.Fail_Autodestruction);
        }

        void fixGenerator_OnMissionSuccess(MyMissionBase sender)
        {
            MyEntity generator = MyScriptWrapper.GetEntity((uint)EntityID.GeneratorFix);
            MyScriptWrapper.SetEntityEnabled(generator, true);
        }

        void findRepairKit_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.TryHide((uint)EntityID.CargoRepair);
        }

        void findRepairKit_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.CargoRepair, true,this);
            MyScriptWrapper.TryUnhide((uint)EntityID.CargoRepair);
            MyScriptWrapper.EnablePhysics((uint)EntityID.CargoRepair, false);
        }

        void autodestructHelp_OnMissionSuccess(MyMissionBase sender)
        {
            MySmallShipBot marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            MyScriptWrapper.Follow(MySession.PlayerShip, marcus);
        }

        void MyScriptWrapper_EntityDeath(MyEntity entity1, MyEntity entity2)
        {
            if (entity1 == null || !entity1.EntityId.HasValue)
                return;

            if (entity1.EntityId.Value.NumericValue == (uint)EntityID.PrefabAutodestruct1)
            {
                Aggregator1Or2Destroyed();
            }
            else if (entity1.EntityId.Value.NumericValue == (uint)EntityID.PrefabAutodestruct2)
            {
                Aggregator1Or2Destroyed();
            }
            else if (entity1.EntityId.Value.NumericValue == (uint)EntityID.PrefabAutodestruct3)
            {
                Aggregator3Destroyed();
            }
            else if (entity1.EntityId.Value.NumericValue == (uint)EntityID.PrefabAutodestruct4)
            {
                Aggregator4Destroyed();
            }
        }

        void autodestructDestroy_OnMissionLoaded(MyMissionBase sender)
        {
            MySmallShipBot marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            MyEntity destroy3 = MyScriptWrapper.TryGetEntity((uint)EntityID.PrefabAutodestruct3);
            MyScriptWrapper.StopFollow(marcus);

            if (destroy3 != null)
            {
                marcus.Attack(destroy3);
            }
        }

        void Aggregator1Or2Destroyed()
        {
            MyEntity destroy1 = MyScriptWrapper.TryGetEntity((uint)EntityID.PrefabAutodestruct1);
            MyEntity destroy2 = MyScriptWrapper.TryGetEntity((uint)EntityID.PrefabAutodestruct2);

            int deadCount = 0;
            if (destroy1 == null || destroy1.IsDead())
            {
                deadCount++;
            }
            if (destroy2 == null || destroy2.IsDead())
            {
                deadCount++;
            }

            if (deadCount == 1)
            {
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.PrefabAutodestruct3 });
            }
            else if (deadCount == 2)
            {
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.PrefabAutodestruct4 });
            }
        }

        void Aggregator3Destroyed()
        {
            MySmallShipBot marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            MyEntity destroy4 = MyScriptWrapper.GetEntity((uint)EntityID.PrefabAutodestruct4);
            marcus.Attack(destroy4);
        }

        void Aggregator4Destroyed()
        {
            MySmallShipBot marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            MyScriptWrapper.Move(marcus, MyScriptWrapper.GetEntity((uint)EntityID.DummyNearPrefabAutodestruct4).GetPosition());
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnAutodestruct);
        }



        void openCargoDoor_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.EnablePhysics((uint)EntityID.PrefabBomb, false);
            MyScriptWrapper.Highlight((uint)EntityID.PrefabBomb, true, this);
        }

        void openCargoDoor_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.PrefabBomb, false, this);
            MyScriptWrapper.EnablePhysics((uint)EntityID.PrefabBomb, true);
        }

        private void openCargoDoor_boom()
        {
            //door.Health = 0.1f;
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.PrefabBomb).GetPosition(), MyExplosionTypeEnum.BOMB_EXPLOSION, 80, 25);

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.ParticleBombExplosion), true);

            MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.PrefabBomb));
            MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.DoorCargoBomb));

            var ravenGirl = (MySmallShipBot)MyScriptWrapper.GetEntity("RavenGirl");
            var ravenGuy = (MySmallShipBot)MyScriptWrapper.GetEntity("RavenGuy");

            MyScriptWrapper.StopFollow(ravenGirl);
            MyScriptWrapper.StopFollow(ravenGuy);
        
            //ravenGirl.Follow
        }


        public override void Load()
        {
            base.Load();

            // Add Fourth Reich FalseId to inventory if player already haven't got one
            //MyScriptWrapper.AddFalseIdToPlayersInventory(MyMwcObjectBuilder_FactionEnum.FourthReich);

            // Set musicmood right from script start
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress);

            // Change player faction to Rainiers and set relations between them
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_WORST);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            MyScriptWrapper.FixBotNames();
            
            MyScriptWrapper.TryHide((uint)EntityID.TurretAutodescrut1);
            MyScriptWrapper.TryHide((uint)EntityID.TurretAutodescrut2);
            MyScriptWrapper.TryHide((uint)EntityID.TurretAutodescrut3);
            MyScriptWrapper.TryHide((uint)EntityID.TurretAutodescrut4);

            MyScriptWrapper.TryHide((uint)EntityID.CargoRepair);

            // Add hacking tool to inventory if player already haven't got one
            MyScriptWrapper.AddHackingToolToPlayersInventory(2);


            MyScriptWrapper.EntityDeath += MyScriptWrapper_EntityDeath;
        }


        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.EntityDeath -= MyScriptWrapper_EntityDeath;

        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.Resources;
using MinerWars.AppCode.Game.Missions.Components;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyChineseTransportMission : MyMission
    {
        #region Entities

        MyEntity m_stealMS;
        MyEntity m_ms1;
        MyEntity m_ms2;
        MyEntity m_ms3;
        MyEntity m_ms4;
        MyEntity m_ms5;
        MyEntity m_ms6;
        MySmallShipBot m_marcus;
        MySmallShipBot m_tarja;

        #endregion

        private List<List<uint>> m_validateIDlists;

        #region Objectives

        MyObjective m_mission01_getFirstKey;
        MyObjective m_mission02_reachTunnel_1;
        MyObjective m_mission03_reachTransmitter;
        MyObjectiveDestroy m_mission04_killGuards;
        MyMultipleUseObjective m_mission05HackTransmitters;
        MyUseObjective m_mission06_placeBomb;
        MyObjective m_mission07_runExplosion;
        MyObjective m_mission08_lookOnExplosion;
        MyObjective m_mission09_reachTunnel2;
        MyObjective m_mission10_pastTunnel2;
        MyObjective m_mission11_reachHangarHack;
        MyObjective m_mission12_hackHangarServicePC;
        MyDestroyWavesObjective m_mission13_defendMarcus;
        MyObjectiveDestroy m_mission14_killBoss;
        MyObjective m_mission15_landIn;

        #endregion

        #region Detector

        MyEntityDetector m_detector_tunnelExplosion;
        MyEntityDetector m_detector_ambushExplosion;
        MyEntityDetector m_detector_headshake1;
        MyEntityDetector m_detector_headshake2;
        MyEntityDetector m_detector_headshake3;
        MyEntityDetector m_detector_reachHangar;
        MyEntityDetector m_detector_bots1;
        MyEntityDetector m_detector_bots2;
        MyEntityDetector m_detector_bots3;
        MyEntityDetector m_detector_bots4;
        MyEntityDetector m_detector_bots5;
        MyEntityDetector m_detector_bots6;
        MyEntityDetector m_detector_bots7;
        MyEntityDetector m_detector_bots8;
        MyEntityDetector m_detector_bots9;
        MyEntityDetector m_detector_bots10;
        MyEntityDetector m_detector_marcusLeave1;
        MyEntityDetector m_detector_marcusLeave2;

        #endregion

        #region EntityIDs

        enum EntityID // list of IDs used in script
        {
            StartLocation = 187309,
            ReachTunnel1 = 187267,
            ReachTransmitter = 187268,
            PlaceBomb = 187314,
            RunExplosion = 187363,
            ReachTunnel2 = 187270,
            PastTunnel2 = 187272,
            ReachHacking = 187364,
            DefendMS = 10363,
            Land = 180700,
            MS1 = 16780285,
            MS2 = 16780373,
            MS3 = 16780417,
            MS4 = 16780329,
            MS5 = 16780461,
            MS6 = 182340,
            MSSteal = 179850,
            HUBGetKey = 193646,
            HUBHackTrans1 = 192364,
            HUBHackTrans2 = 192367,
            HUBHackHangar = 196818,
            DetectorTunnelExplosion = 187324,
            DetectorAmbushExplosion = 187322,
            DetectorHeadshake1 = 188137,
            DetectorHeadshake2 = 188135,
            DetectorHeadshake3 = 188129,
            DetectorReachHangar = 189273,
            DestructionTunnel = 180,
            DestructionTransmitter = 182838,
            DestructionBuilding = 182760,
            DetectorBots1 = 196871,
            DetectorBots2 = 196873,
            DetectorBots3 = 196875,
            DetectorBots4 = 196878,
            DetectorBots5 = 196880,
            DetectorBots7 = 197310,
            DetectorBots8 = 197312,
            DetectorBots9 = 197323,
            DetectorBots10 = 197327,
            DetectorMarcusLeave1 = 198507,
            DetectorMarcusLeave2 = 198509,
            SpawnBots1_1 = 187231,
            SpawnBots2_1 = 187232,
            SpawnBots2_2 = 4839,
            SpawnBots3_1 = 187233,
            SpawnBots3_2 = 196877,
            SpawnBots4_1 = 187234,
            SpawnBots4_2 = 187235,
            SpawnBots5_1 = 187236,
            SpawnBots5_2 = 196882,
            SpawnBots5_3 = 187237,
            SpawnBots6_1 = 186917,
            SpawnBots7_1 = 18140,
            SpawnBots8_1 = 187320,
            SpawnBots8_2 = 27485,
            SpawnBots9_1 = 18143,
            SpawnBots9_2 = 197325,
            SpawnBots10_1 = 197326,
            SpawnBots10_2 = 197329,
            SpawnBotsLast = 18147,
            SpawnTunnelDestruction = 187238,
            SpawnAmbush = 27486,
            SpawnBoss = 187284,
            SpawnMarcus = 187751,
            SpawnTarja = 1,
            SpawnBossCompanion1 = 187286,
            SpawnBossCompanion2 = 187285,
            WaypointReachedMarcus = 187754,
            ExplosionAmbush = 187315,
            ExplosionTunnel = 187313,
            Bomb = 191603,
            UnlockDoors = 191982,
            MarcusWaypointReached = 199916,
            MarcusLeavePosition = 199918,
            ExplosionPrefab = 16778085,
            ExplosionTransmitter = 16778086,
            DoorToHub = 167944,
        }

        List<uint> m_explosion1 = new List<uint>
            {
                187327,
                187330,
                187335,
                187339,
                187344
            };

        List<uint> m_explosion2 = new List<uint>
            {
                187328,
                187331,
                187336,
                187340,
                187345
            };

        List<uint> m_explosion3 = new List<uint>
            {
                187329,
                187332,
                187337,
                187341
            };

        List<uint> m_explosion4 = new List<uint>
            {
                187338,
                187342,
                187343
            };

        List<uint> m_explosionFinal = new List<uint>
            {
                187333,
                187334,
                187346,
                187347,
                187348
            };

        List<uint> m_transmitterPrefabs1 = new List<uint>
            {
                184588,
                184614,
                184617,
                184641,
                184640,
                184639,
                184623,
                184624,
                184625,
                184633,
                184632,
                182768
            };

        List<uint> m_transmitterPrefabs2 = new List<uint>
            {
                184586,
                184592,
                184615,
                184616,
                184638,
                184622,
                184635,
                184634,
                182767,
                186431,
                186433,
                186409,
                186407,
                186408,
                186406,
                186410,
                185566,
                185565,
                185562,
                185564,
                185563,
                185557,
                185561,
                185560,
                185556,
                185559,
                186411,
                186412,
                186416,
                186413,
                186414,
                186415,
                184655,
                185098,
                192370,
                192371,
                192369,
                192372,
                192368,
                192373
            };

        List<uint> m_spawnsStart = new List<uint>
            {
                186919,
                186920,
                4430
            };

        List<uint> m_spawnsCenterAsteroid = new List<uint>
            {
                187239,
                187257,
                187258
            };

        List<uint> m_spawnsTransmitter = new List<uint>
            {
                187249,
                187255,
                187251,
                187252,
                187253,
                187254,
                187244,
                187250
            };

        List<uint> m_spawnsCenterAsteroidOut = new List<uint>
            {
                187259,
                4066,
                2288
            };

        List<uint> m_spawnsWave1 = new List<uint> { 187274, 187273 };

        List<uint> m_spawnsWave2 = new List<uint> { 187279, 187275 };

        List<uint> m_spawnsWave3 = new List<uint> { 187299, 187280 };

        List<uint> m_spawnsWave4 = new List<uint> { 187276, 187283 };

        List<uint> m_spawnsWave5 = new List<uint> { 187282, 187278, 187281 };
        
        uint[] m_spawns = new uint[] { 187274, 187273, 187279, 187275, 187299, 187280, 187276, 187283, 187282, 187278, 187281 };

        List<uint> m_debrisAfterExplosion = new List<uint>
            {
                193177,
                193173,
                193180,
                193176,
                193207,
                193209,
                193213,
                193212,
                193208
            };

        bool m_doorDied;

        #endregion

        public MyChineseTransportMission()
        {
            #region Validate IDs

            m_validateIDlists = new List<List<uint>>();
            m_validateIDlists.Add(m_explosion1);
            m_validateIDlists.Add(m_explosion2);
            m_validateIDlists.Add(m_explosion3);
            m_validateIDlists.Add(m_explosion4);
            m_validateIDlists.Add(m_explosionFinal);
            m_validateIDlists.Add(m_transmitterPrefabs1);
            m_validateIDlists.Add(m_transmitterPrefabs2);
            m_validateIDlists.Add(m_spawnsStart);
            m_validateIDlists.Add(m_spawnsCenterAsteroid);
            m_validateIDlists.Add(m_spawnsTransmitter);
            m_validateIDlists.Add(m_spawnsCenterAsteroidOut);
            m_validateIDlists.Add(m_spawnsWave1);
            m_validateIDlists.Add(m_spawnsWave2);
            m_validateIDlists.Add(m_spawnsWave3);
            m_validateIDlists.Add(m_spawnsWave4);
            m_validateIDlists.Add(m_spawnsWave5);

            #endregion

            ID = MyMissionID.CHINESE_TRANSPORT; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.CHINESE_TRANSPORT;
            Description = MyTextsWrapperEnum.CHINESE_TRANSPORT_Description; // "Destroy the transmitter and steal the chinese transporter\n"
            DebugName = new StringBuilder("08b-Chinese mines of Changde");
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-4274372, 0, 4874227); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_LAND_IN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


            m_mission01_getFirstKey = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_GET_SECURITY_KEY_Name), // Name of the submission
                MyMissionID.CHINESE_TRANSPORT_GET_SECURITY_KEY, // ID of the submission - must be added to MyMissions.cs
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_GET_SECURITY_KEY_Description), // Description of the submission
                null,
                this,
                new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                null,
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0100_INTRODUCE
            )
                {
                    SaveOnSuccess = false,
                    HudName = MyTextsWrapperEnum.HudHub
                };
            m_mission01_getFirstKey.MissionEntityIDs.Add((uint)EntityID.HUBGetKey);
            m_objectives.Add(m_mission01_getFirstKey);

            m_mission02_reachTunnel_1 = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TUNNEL_1_Name),
                MyMissionID.CHINESE_TRANSPORT_REACH_TUNNEL_1,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TUNNEL_1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_GET_SECURITY_KEY },
                new MyMissionLocation(baseSector, (uint) EntityID.ReachTunnel1),
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0200_FIRST_DEVICE_HACKED
                ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudTunnel };
            m_objectives.Add(m_mission02_reachTunnel_1);

            m_mission03_reachTransmitter = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TRANSMITTER_Name),
                MyMissionID.CHINESE_TRANSPORT_REACH_TRANSMITTER,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TRANSMITTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_REACH_TUNNEL_1 },
                 new MyMissionLocation(baseSector, (uint)EntityID.ReachTransmitter)
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudTransmitter };
            m_objectives.Add(m_mission03_reachTransmitter);

            m_mission04_killGuards = new MyObjectiveDestroy( // Var is used to call functions on that member
                 (MyTextsWrapperEnum.CHINESE_TRANSPORT_KILL_GUARDS_Name),
                 MyMissionID.CHINESE_TRANSPORT_KILL_GUARDS,
                 (MyTextsWrapperEnum.CHINESE_TRANSPORT_KILL_GUARDS_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_REACH_TRANSMITTER },
                 null,
                 m_spawnsTransmitter,
                 true,
                 startDialogID: MyDialogueEnum.CHINESE_TRANSPORT_0500_MARCUS_IS_HERE
             ) { SaveOnSuccess = true };
            m_objectives.Add(m_mission04_killGuards);

            m_mission05HackTransmitters = new MyMultipleUseObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_HACK_TRANSMITTER_Name),
                MyMissionID.CHINESE_TRANSPORT_HACK_TRANSMITTER,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_HACK_TRANSMITTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_KILL_GUARDS },
                MyTextsWrapperEnum.HoldToUseSecurityKey,
                MyTextsWrapperEnum.Using,
                MyTextsWrapperEnum.DeactivatingInProgress,
                2000,
                new List<uint> { (uint) EntityID.HUBHackTrans1, (uint) EntityID.HUBHackTrans2 }
                )
                {
                    StartDialogId = MyDialogueEnum.CHINESE_TRANSPORT_0600_DESTROY_THE_TRANSMITTER, 
                    SaveOnSuccess = false,
                    RadiusOverride = 30,
                    MakeEntityIndestructible = false,
                    FailIfEntityDestroyed = true
                };
            m_objectives.Add(m_mission05HackTransmitters);

            m_mission06_placeBomb = new MyUseObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_PLACE_BOMB_Name),
                MyMissionID.CHINESE_TRANSPORT_PLACE_BOMB,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_PLACE_BOMB_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_HACK_TRANSMITTER },
                new MyMissionLocation(baseSector, (uint)EntityID.PlaceBomb),
                MyTextsWrapperEnum.PressToPlaceBomb,
                MyTextsWrapperEnum.Bomb,
                MyTextsWrapperEnum.PlacementInProgress,
                5000,
                MyUseObjectiveType.Activating, 
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0670_disabling_the_terminals
            ) { SaveOnSuccess = false };
            m_mission06_placeBomb.MissionEntityIDs.Add((uint) EntityID.Bomb);
            m_objectives.Add(m_mission06_placeBomb);

            m_mission07_runExplosion = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_RUN_EXPLOSION_Name),
                MyMissionID.CHINESE_TRANSPORT_RUN_EXPLOSION,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_RUN_EXPLOSION_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_PLACE_BOMB },
                new MyMissionLocation(baseSector, (uint)EntityID.RunExplosion),
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0700_RUN
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudSafeArea };
            m_objectives.Add(m_mission07_runExplosion);

            m_mission08_lookOnExplosion = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_TRANSPORT_LOOK_ON_EXPLOSION_Name),
              MyMissionID.CHINESE_TRANSPORT_LOOK_ON_EXPLOSION,
              (MyTextsWrapperEnum.CHINESE_TRANSPORT_LOOK_ON_EXPLOSION_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_RUN_EXPLOSION },
              new MyMissionLocation(baseSector, (uint)EntityID.DestructionTransmitter)
          ) { SaveOnSuccess = true };
            m_objectives.Add(m_mission08_lookOnExplosion);

            m_mission09_reachTunnel2 = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TUNNEL_2_Name),
                MyMissionID.CHINESE_TRANSPORT_REACH_TUNNEL_2,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_TUNNEL_2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_LOOK_ON_EXPLOSION },
                new MyMissionLocation(baseSector, (uint)EntityID.ReachTunnel2),
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0800_GO_TO_SECOND_BASE
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudTunnel };
            m_objectives.Add(m_mission09_reachTunnel2);

            m_mission10_pastTunnel2 = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_PAST_TUNNEL_2_Name),
                MyMissionID.CHINESE_TRANSPORT_PAST_TUNNEL_2,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_PAST_TUNNEL_2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_REACH_TUNNEL_2 },
                new MyMissionLocation(baseSector, (uint)EntityID.PastTunnel2),
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0850_INSIDE_TUNNEL,
                successDialogId: MyDialogueEnum.CHINESE_TRANSPORT_0900_SURRENDER
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudEnd };
            m_objectives.Add(m_mission10_pastTunnel2);

            m_mission11_reachHangarHack = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_HANGAR_HACK_Name),
                MyMissionID.CHINESE_TRANSPORT_REACH_HANGAR_HACK,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_REACH_HANGAR_HACK_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_PAST_TUNNEL_2 },
                new MyMissionLocation(baseSector, (uint)EntityID.ReachHacking),
                successDialogId: MyDialogueEnum.CHINESE_TRANSPORT_1000_DOOR_BLOCKED
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudServiceRoom };
            m_mission11_reachHangarHack.OnMissionLoaded += Mission11ReachHangarHackOnOnMissionLoaded;
            m_objectives.Add(m_mission11_reachHangarHack);

            m_mission12_hackHangarServicePC = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_HACK_HANGAR_SERVICE_PC_Name),
                MyMissionID.CHINESE_TRANSPORT_HACK_HANGAR_SERVICE_PC,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_HACK_HANGAR_SERVICE_PC_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_REACH_HANGAR_HACK },
                null
                //startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_1400_DOORS_UNLOCKED
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHub };
            m_mission12_hackHangarServicePC.OnMissionLoaded += new MissionHandler(m_mission12_hackHangarServicePC_OnMissionLoaded);
            m_objectives.Add(m_mission12_hackHangarServicePC);
            m_mission12_hackHangarServicePC.MissionEntityIDs.Add((uint)EntityID.HUBHackHangar);

            m_mission13_defendMarcus = new MyDestroyWavesObjective(
               (MyTextsWrapperEnum.CHINESE_TRANSPORT_DEFEND_MARCUS_Name),
               MyMissionID.CHINESE_TRANSPORT_DEFEND_MARCUS,
               (MyTextsWrapperEnum.CHINESE_TRANSPORT_DEFEND_MARCUS_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_HACK_HANGAR_SERVICE_PC },
               null,
               null,
               null,
               null,
               4
               )
                {
                    SaveOnSuccess = false,
                    StartDialogId = MyDialogueEnum.CHINESE_TRANSPORT_1100_HELP_MARCUS
                };
            m_mission13_defendMarcus.AddWave(m_spawnsWave1);
            m_mission13_defendMarcus.AddWave(m_spawnsWave2);
            m_mission13_defendMarcus.AddWave(m_spawnsWave3);
            m_mission13_defendMarcus.AddWave(m_spawnsWave4);
            m_mission13_defendMarcus.AddWave(m_spawnsWave5);
            m_mission13_defendMarcus.Components.Add(new MySpawnpointLimiter(m_spawns, 10));
            m_objectives.Add(m_mission13_defendMarcus);

            m_mission14_killBoss = new MyObjectiveDestroy( // Var is used to call functions on that member
                 (MyTextsWrapperEnum.CHINESE_TRANSPORT_KILL_BOSS_Name),
                 MyMissionID.CHINESE_TRANSPORT_KILL_BOSS,
                 (MyTextsWrapperEnum.CHINESE_TRANSPORT_KILL_BOSS_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_DEFEND_MARCUS },
                 null,
                 new List<uint> { (uint)EntityID.SpawnBossCompanion1, (uint)EntityID.SpawnBossCompanion2, (uint)EntityID.SpawnBoss },
                 true,
                 startDialogID: MyDialogueEnum.CHINESE_TRANSPORT_1200_GENERAL_ARRIVAL
             ) { SaveOnSuccess = true };
            m_objectives.Add(m_mission14_killBoss);

            m_mission15_landIn = new MyObjective(
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_LAND_IN_Name),
                MyMissionID.CHINESE_TRANSPORT_LAND_IN,
                (MyTextsWrapperEnum.CHINESE_TRANSPORT_LAND_IN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT_KILL_BOSS },
                new MyMissionLocation(baseSector, (uint)EntityID.Land),
                startDialogId: MyDialogueEnum.CHINESE_TRANSPORT_1300_LAND_IN
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudShip };
            m_objectives.Add(m_mission15_landIn);

            m_mission01_getFirstKey.OnMissionLoaded += M01GetFirstKeyLoaded;
            m_mission02_reachTunnel_1.OnMissionLoaded += M02ReachTunnel1Loaded;
            m_mission03_reachTransmitter.OnMissionLoaded += M03ReachTransmitterLoaded;
            m_mission04_killGuards.OnMissionLoaded += M04KillGuardsLoaded;
            m_mission05HackTransmitters.OnMissionLoaded += M05HackTransmitterLoaded;
            m_mission06_placeBomb.OnMissionLoaded += M06PlaceBombLoaded;
            m_mission07_runExplosion.OnMissionLoaded += M07RunExplosionLoaded;
            m_mission08_lookOnExplosion.OnMissionLoaded += M08LookOnExplosionLoaded;
            m_mission09_reachTunnel2.OnMissionLoaded += M09ReachTunnel2Loaded;
            m_mission10_pastTunnel2.OnMissionLoaded += M10PastTunnel2Loaded;
            m_mission11_reachHangarHack.OnMissionLoaded += M11ReachHangarHackLoaded;
            m_mission12_hackHangarServicePC.OnMissionLoaded += M12HackHangarServicePCLoaded;
            m_mission13_defendMarcus.OnMissionLoaded += M13DefendMarcusLoaded;
            m_mission13_defendMarcus.OnMissionCleanUp += Mission13DefendMarcusOnOnMissionCleanUp;
            m_mission14_killBoss.OnMissionLoaded += M14KillBossLoaded;
            m_mission14_killBoss.OnMissionCleanUp += M14KillBossUnloaded;
            m_mission15_landIn.OnMissionLoaded += M15LandInLoaded;

            m_mission06_placeBomb.OnMissionSuccess += M06PlaceBombSuccess;
            m_mission05HackTransmitters.OnObjectUsedSucces += M05TransmittersSuccess;
        }

        void m_mission12_hackHangarServicePC_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.EntityHacked += MyScriptWrapper_EntityHackedAfterExplosion;
        }

        void Mission11ReachHangarHackOnOnMissionLoaded(MyMissionBase sender)
        {
            var door = MyScriptWrapper.GetEntity((uint) EntityID.DoorToHub) as MyPrefabKinematic;
            if (door != null)
            {
                foreach (var part in door.Parts)
                {
                    if (part != null)
                        part.OnDie += DoorDied;
                }
            }

            m_doorDied = false;
        }

        void DoorDied(MyEntity entity, MyEntity killer)
        {
            entity.OnDie -= DoorDied;

            if (!m_doorDied)
            {
                m_doorDied = true;
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_TRANSPORT_1400_DOORS_UNLOCKED);
            }
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (List<uint> list in m_validateIDlists) 
            {
                foreach (var item in list)
                {
                    MyScriptWrapper.GetEntity(item);
                }
            }
          
        }

        private void Mission13DefendMarcusOnOnMissionCleanUp(MyMissionBase sender)
        {
            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapperOnOnSpawnpointBotSpawnedDefendMarcus;
            MyScriptWrapper.RemoveEntityMark(m_stealMS);
        }

        public override void Update()
        {
            if (!IsMainSector)
            {
                return;
            }

            MoveMotherShipForward(m_ms1, 120.0f);
            MoveMotherShipForward(m_ms2, 80.0f);
            MoveMotherShipForward(m_ms3, 100.0f);
            MoveMotherShipForward(m_ms4, 140.0f);
            MoveMotherShipForward(m_ms5, 140.0f);
            MoveMotherShipForwardDest(m_ms6, 70.0f, new Vector3(-2634f, 1490f, -8516f));

            if (m_stealMS.IsDead())
            {
                Fail(MyTextsWrapperEnum.Fail_MothershipDestroyed);
            }
            base.Update();
        }

        public override void Load()
        {
            m_ms1 = MyScriptWrapper.GetEntity((uint)EntityID.MS1);
            m_ms2 = MyScriptWrapper.GetEntity((uint)EntityID.MS2);
            m_ms3 = MyScriptWrapper.GetEntity((uint)EntityID.MS3);
            m_ms4 = MyScriptWrapper.GetEntity((uint)EntityID.MS4);
            m_ms5 = MyScriptWrapper.GetEntity((uint)EntityID.MS5);
            m_ms6 = MyScriptWrapper.GetEntity((uint)EntityID.MS6);
            m_stealMS = MyScriptWrapper.GetEntity((uint)EntityID.MSSteal);
            
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);

            m_marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            m_tarja = (MySmallShipBot)MyScriptWrapper.GetEntity("RavenGirl");

            MyScriptWrapper.EnablePhysics((uint)EntityID.MS1, false);
            MyScriptWrapper.EnablePhysics((uint)EntityID.MS2, false);
            MyScriptWrapper.EnablePhysics((uint)EntityID.MS3, false);
            MyScriptWrapper.EnablePhysics((uint)EntityID.MS4, false);
            MyScriptWrapper.EnablePhysics((uint)EntityID.MS5, false);
            MyScriptWrapper.EnablePhysics((uint)EntityID.MS6, false);


            m_detector_reachHangar = MyScriptWrapper.GetDetector((uint)EntityID.DetectorReachHangar);
            m_detector_tunnelExplosion = MyScriptWrapper.GetDetector((uint)EntityID.DetectorTunnelExplosion);
            m_detector_ambushExplosion = MyScriptWrapper.GetDetector((uint)EntityID.DetectorAmbushExplosion);
            m_detector_headshake1 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorHeadshake1);
            m_detector_headshake2 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorHeadshake2);
            m_detector_headshake3 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorHeadshake3);
            m_detector_bots1 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots1);
            m_detector_bots2 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots2);
            m_detector_bots3 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots3);
            m_detector_bots4 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots4);
            m_detector_bots5 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots5);
            m_detector_bots7 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots7);
            m_detector_bots8 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots8);
            m_detector_bots9 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots9);
            m_detector_bots10 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorBots10);
            m_detector_marcusLeave1 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorMarcusLeave1);
            m_detector_marcusLeave2 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorMarcusLeave2);

            MyScriptWrapper.OnBotReachedWaypoint += ReachedWaypoint;
            base.Load();
        }


        public override void Unload()
        {
            MyScriptWrapper.EntityHacked -= MyScriptWrapper_EntityHackedAfterExplosion;
            MyScriptWrapper.EntityHacked -= MyScriptWrapperEntityHackedFirstKey;
            MyScriptWrapper.OnBotReachedWaypoint -= ReachedWaypoint;
            m_detector_bots1.Off();
            m_detector_bots1.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots2.Off();
            m_detector_bots2.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots3.Off();
            m_detector_bots3.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots4.Off();
            m_detector_bots4.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots5.Off();
            m_detector_bots5.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots7.Off();
            m_detector_bots7.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots8.Off();
            m_detector_bots8.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots9.Off();
            m_detector_bots9.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots10.Off();
            m_detector_bots10.OnEntityEnter -= SpawnBotsDetectorEntered;
            base.Unload();

            m_ms1 = null;
            m_ms2 = null;
            m_ms3 = null;
            m_ms4 = null;
            m_ms5 = null;
            m_ms6 = null;
            m_stealMS = null;

            m_marcus = null;
            m_tarja = null;

            m_detector_reachHangar = null;
            m_detector_tunnelExplosion = null;
            m_detector_ambushExplosion = null;
            m_detector_headshake1 = null;
            m_detector_headshake2 = null;
            m_detector_headshake3 = null;
            m_detector_bots1 = null;
            m_detector_bots2 = null;
            m_detector_bots3 = null;
            m_detector_bots4 = null;
            m_detector_bots5 = null;
            m_detector_bots7 = null;
            m_detector_bots8 = null;
            m_detector_bots9 = null;
            m_detector_bots10 = null;
            m_detector_marcusLeave1 = null;
            m_detector_marcusLeave2 = null;

            if (MyScriptWrapper.IsMissionFinished(this.ID))
            {
                MyScriptWrapper.TravelToMission(MyMissionID.CHINESE_REFINERY); //Chinese refinery
            }
        }

        public override void Accept()
        {
            base.Accept();
        }


        private void ReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (waypoint == MyScriptWrapper.GetEntity((uint)EntityID.WaypointReachedMarcus) && bot == m_marcus)
            {
                MyScriptWrapper.HideEntity(m_marcus);
                MyScriptWrapper.HideEntity(m_tarja);
                MyScriptWrapper.OnBotReachedWaypoint -= ReachedWaypoint;
            }
        }

        private void DetectorEntered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (sender == m_detector_tunnelExplosion && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.CHINESE_TRANSPORT_0400_THEY_FOUND_ME);
                m_detector_tunnelExplosion.OnEntityEnter -= DetectorEntered;
                m_detector_tunnelExplosion.Off();
                MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionTunnel), Explosions.MyExplosionTypeEnum.MEDIUM_PREFAB_EXPLOSION, 20000);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnTunnelDestruction);
                MyScriptWrapper.IncreaseHeadShake(8);
            }
            if (sender == m_detector_headshake1 && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.CHINESE_TRANSPORT_0300_SHOOTING_ON_ME);
                MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                MyScriptWrapper.IncreaseHeadShake(12);
                m_detector_headshake1.OnEntityEnter -= DetectorEntered;
                m_detector_headshake1.Off();

            }
            if (sender == m_detector_headshake2 && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                MyScriptWrapper.IncreaseHeadShake(18);
                m_detector_headshake2.OnEntityEnter -= DetectorEntered;
                m_detector_headshake2.Off();
            }
            if (sender == m_detector_headshake3 && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                MyScriptWrapper.IncreaseHeadShake(15);
                m_detector_headshake3.OnEntityEnter -= DetectorEntered;
                m_detector_headshake3.Off();
            }
            if (sender == m_detector_ambushExplosion && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionAmbush), true);
                MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                MyScriptWrapper.IncreaseHeadShake(9);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnAmbush);
                m_detector_ambushExplosion.OnEntityEnter -= DetectorEntered;
                m_detector_ambushExplosion.Off();
            }

            if (sender == m_detector_reachHangar && MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_TRANSPORT_1050_REACHED_COMPUTER);
                m_detector_reachHangar.OnEntityEnter -= DetectorEntered;
                m_detector_reachHangar.Off();
            }
        }

        void MarcusLeaveAction(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            MyScriptWrapper.OnBotReachedWaypoint += MarcusReached;
            m_marcus.SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.MarcusLeavePosition).WorldMatrix);
            m_marcus.SetWaypointPath("MarcusL");
            m_marcus.PatrolMode = CommonLIB.AppCode.ObjectBuilders.Object3D.MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.MarucsIsLeavingForTransporter, MyGuiManager.GetFontMinerWarsGreen(), 5000));
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_TRANSPORT_0650_MARCUS_IS_LEAVING);
            m_detector_marcusLeave1.OnEntityEnter -= MarcusLeaveAction;
            m_detector_marcusLeave1.Off();
            m_detector_marcusLeave2.OnEntityEnter -= MarcusLeaveAction;
            m_detector_marcusLeave2.Off();
        }

        void MarcusReached(MyEntity bot, MyEntity waypoint)
        {
            if (bot == m_marcus && waypoint == MyScriptWrapper.GetEntity((uint)EntityID.MarcusWaypointReached))
            {
                MyScriptWrapper.HideEntity(m_marcus);
                MyScriptWrapper.OnBotReachedWaypoint -= MarcusReached;
            }
        }

        void SpawnBotsDetectorEntered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            if (sender == m_detector_bots1 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots1.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots1.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots1_1);               
            }
            if (sender == m_detector_bots2 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots2.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots2.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots2_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots2_2);
            }
            if (sender == m_detector_bots3 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots3.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots3.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots3_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots3_2);
            }
            if (sender == m_detector_bots4 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots4.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots4.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots4_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots4_2);
            }
            if (sender == m_detector_bots5 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots5.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots5.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots5_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots5_2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots5_3);
            }
            if (sender == m_detector_bots7 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots7.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots7.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots7_1);
            }
            if (sender == m_detector_bots8 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots8.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots8.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots8_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots8_2);
            }
            if (sender == m_detector_bots9 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots9.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots9.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots9_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots9_2);
            }
            if (sender == m_detector_bots10 && MySession.IsPlayerShip(entity))
            {
                m_detector_bots10.OnEntityEnter -= SpawnBotsDetectorEntered;
                m_detector_bots10.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots10_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots10_2);
            }
        }
        
        void MyScriptWrapperEntityHackedFirstKey(MyEntity entity)
        {
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.HUBGetKey))
            {
                m_mission01_getFirstKey.Success();
                MyScriptWrapper.RemoveEntityMark(entity);
            }
        }

        void MyScriptWrapper_EntityHackedAfterExplosion(MyEntity entity)
        {
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.HUBHackHangar))
            {
                m_mission12_hackHangarServicePC.Success();
            }
        }

        void BossSpawned(MyEntity spawnpoint, MyEntity botmain)
        {
            foreach (var item2 in new List<List<uint>> { m_spawnsWave1, m_spawnsWave2, m_spawnsWave3, m_spawnsWave4, m_spawnsWave5 })
            foreach (var item in item2)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(item))
                {
                    MyScriptWrapper.SetSleepDistance(botmain, 3000);
                }
            }
            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.SpawnBoss))
            {
                MySmallShipBot bossbot = botmain as MySmallShipBot;
                bossbot.Health = bossbot.MaxHealth = 1500;
                botmain.DisplayName = MyTexts.GeneralChenLin;
            }
        }

        void M01GetFirstKeyLoaded(MyMissionBase sender)
        {
            m_detector_bots1.On();
            m_detector_bots1.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots2.On();
            m_detector_bots2.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots3.On();
            m_detector_bots3.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots4.On();
            m_detector_bots4.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots5.On();
            m_detector_bots5.OnEntityEnter += SpawnBotsDetectorEntered;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 1, "MM01");
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.MarucsIsLeavingForKey, MyGuiManager.GetFontMinerWarsGreen(), 20000));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Bomb));
            MyScriptWrapper.EntityHacked += MyScriptWrapperEntityHackedFirstKey;
            m_detector_bots1.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots1.On();
            foreach (var spawnpoint in m_spawnsStart)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnpoint);
            }
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.HUBGetKey), MyTexts.GetFirstSecurityKey, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);


            MyScriptWrapper.StopFollow(m_marcus);
            m_marcus.SpeedModifier = 2.0f;
            MyScriptWrapper.SetSleepDistance(m_marcus, 10000f);

            m_tarja.Follow(m_marcus);
            m_tarja.SpeedModifier = 2.0f;
            MyScriptWrapper.SetSleepDistance(m_tarja, 10000f);

            m_marcus.SetWaypointPath("MarcusW");
            m_marcus.PatrolMode = CommonLIB.AppCode.ObjectBuilders.Object3D.MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();
        }

        void M02ReachTunnel1Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.EntityHacked -= MyScriptWrapperEntityHackedFirstKey;
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.HUBGetKey));
        }

        void M03ReachTransmitterLoaded(MyMissionBase sender)
        { 
            foreach (var spawn in m_spawnsTransmitter)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawn);
            }
            foreach (var spawn in m_spawnsCenterAsteroid)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawn);
            }
            m_detector_tunnelExplosion.OnEntityEnter += DetectorEntered;
            m_detector_tunnelExplosion.On();
            m_detector_headshake1.OnEntityEnter += DetectorEntered;
            m_detector_headshake1.On();
            m_detector_headshake2.OnEntityEnter += DetectorEntered;
            m_detector_headshake2.On();
            m_detector_headshake3.OnEntityEnter += DetectorEntered;
            m_detector_headshake3.On();
        }

        void M04KillGuardsLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.UnhideEntity(m_marcus);
            MyScriptWrapper.UnhideEntity(m_tarja);
            m_marcus.SetPosition(MyScriptWrapper.GetEntity((uint)EntityID.SpawnMarcus).GetPosition());
            m_tarja.SetPosition(MyScriptWrapper.GetEntity((uint)EntityID.SpawnTarja).GetPosition());

            m_marcus.SpeedModifier = 1.0f;
            m_tarja.SpeedModifier = 1.0f;

            m_tarja.Follow(MySession.PlayerShip);
        }

        void M05HackTransmitterLoaded(MyMissionBase sender)
        {
            m_marcus.Idle();
            m_detector_marcusLeave1.OnEntityEnter += MarcusLeaveAction;
            m_detector_marcusLeave2.OnEntityEnter += MarcusLeaveAction;
            m_detector_marcusLeave1.On();
            m_detector_marcusLeave2.On();
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Bomb));
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans1), MyTexts.UseFirstSecurityKey, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans2), MyTexts.UseSecondSecurityKey, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
        }

        void M05TransmittersSuccess(uint entityID)
        {
            switch (entityID)
            {
                case (uint)EntityID.HUBHackTrans1:
                    MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans1));
                    break;
                case (uint)EntityID.HUBHackTrans2:
                    MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans2));
                    break;
            }
        }
            
        void M06PlaceBombLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.Bomb, true, this);
            MyScriptWrapper.EnablePhysics((uint)EntityID.Bomb, false);
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Bomb), false);
        }

        void M06PlaceBombSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.Bomb, false,this);
            MyScriptWrapper.EnablePhysics((uint)EntityID.Bomb, true);
        }

        void M07RunExplosionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 3, "KA02");
            m_mission07_runExplosion.MissionTimer.RegisterTimerAction(20000, CloseToExplosion, false, MyTexts.ExplosionIn);
        }

        void M08LookOnExplosionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3, "KA02");
            foreach (var spawnpoint in m_spawnsCenterAsteroidOut)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnpoint);
            }
            MissionTimer.RegisterTimerAction(2000, TransmitterExplosion1, false);
        }

        void M09ReachTunnel2Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 1, "KA02");
            MyScriptWrapper.HideEntity(m_marcus);
        }

        void M10PastTunnel2Loaded(MyMissionBase sender)
        {
            m_detector_reachHangar.OnEntityEnter += DetectorEntered;
            m_detector_reachHangar.On(); 
            m_detector_ambushExplosion.OnEntityEnter += DetectorEntered;
            m_detector_ambushExplosion.On();
        }

        void M11ReachHangarHackLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBots6_1);
            m_detector_bots1.Off();
            m_detector_bots1.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots2.Off();
            m_detector_bots2.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots3.Off();
            m_detector_bots3.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots4.Off();
            m_detector_bots4.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots5.Off();
            m_detector_bots5.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots7.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots7.On();
            m_detector_bots8.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots8.On();
            m_detector_bots9.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots9.On();
            m_detector_bots10.OnEntityEnter += SpawnBotsDetectorEntered;
            m_detector_bots10.On();
        }

        void M12HackHangarServicePCLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackHangar), MyTexts.HackHangarDatabase, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
        }

        void M13DefendMarcusLoaded(MyMissionBase sender)
        {
            m_detector_bots7.Off();
            m_detector_bots7.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots8.Off();
            m_detector_bots8.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots9.Off();
            m_detector_bots9.OnEntityEnter -= SpawnBotsDetectorEntered;
            m_detector_bots10.Off();
            m_detector_bots10.OnEntityEnter -= SpawnBotsDetectorEntered;
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.UnlockDoors), true);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3, "KA03");
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.HUBHackHangar));
            MyScriptWrapper.HideEntity(m_marcus);
            sender.MissionEntityIDs.Add(m_stealMS.EntityId.Value.NumericValue);
            MyScriptWrapper.MarkEntity(m_stealMS,sender.NameTemp.ToString(),MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS);
            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapperOnOnSpawnpointBotSpawnedDefendMarcus;

        }

        private void MyScriptWrapperOnOnSpawnpointBotSpawnedDefendMarcus(MyEntity spawn, MyEntity bot)
        {
            var ship = bot as MySmallShipBot;
            ship.SleepDistance = 5000;
        }

        void M14KillBossLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.OnSpawnpointBotSpawned += BossSpawned;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 3, "KA05");
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBoss);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBossCompanion1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnBossCompanion2);
            MyScriptWrapper.HideEntity(m_marcus);
        }

        void M14KillBossUnloaded(MyMissionBase sender)
        {
            MyScriptWrapper.OnSpawnpointBotSpawned -= BossSpawned;
        }

        void M15LandInLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3, "KA02");
        }

        void TransmitterExplosion1()
        {
            MissionTimer.RegisterTimerAction(3000, TransmitterExplosion2, false);
            
           /* foreach (var particle in m_explosion1)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            } */

            //MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter), MySoundCuesEnum.SfxShipLargeExplosion);

            MyScriptWrapper.AddExplosions(m_explosion1, Explosions.MyExplosionTypeEnum.MISSILE_EXPLOSION, 1000);
        }

        void TransmitterExplosion2()
        {
            MissionTimer.RegisterTimerAction(2000, TransmitterExplosion3, false);
          /*  foreach (var particle in m_explosion2)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }*/

            MyScriptWrapper.AddExplosions(m_explosion2, Explosions.MyExplosionTypeEnum.MISSILE_EXPLOSION, 500);
            /*
            foreach (var entity in m_transmitterPrefabs1)
            {
                var item = MyScriptWrapper.TryGetEntity(entity);
                if (item != null)
                {
                    MyScriptWrapper.GetEntity(entity).MarkForClose();
                }
            }
            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter), MySoundCuesEnum.SfxShipLargeExplosion);
             * */
        }

        void TransmitterExplosion3()
        {
            MissionTimer.RegisterTimerAction(1000, TransmitterExplosion4, false);
            /*
            foreach (var particle in m_explosion3)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }
            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter), MySoundCuesEnum.SfxShipLargeExplosion);
             * */

            MyScriptWrapper.AddExplosions(m_explosion3, Explosions.MyExplosionTypeEnum.MISSILE_EXPLOSION, 1000);
        }

        void TransmitterExplosion4()
        {
            MissionTimer.RegisterTimerAction(2000, TransmitterExplosionFinal, false);
            /*foreach (var particle in m_explosion4)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }       */
            //MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter), MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.AddExplosions(m_explosion4, Explosions.MyExplosionTypeEnum.MISSILE_EXPLOSION, 1000);
        }

        void TransmitterExplosionFinal()
        {
            MyScriptWrapper.GetEntity((uint)EntityID.Bomb).MarkForClose();
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPrefab), Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 40000);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionTransmitter), Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 40000);
            //MyScriptWrapper.AddExplosions(m_explosionFinal, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 20000);
            /*
            foreach (var particle in m_explosionFinal)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }
            foreach (var entity in m_transmitterPrefabs2)
            {
                var item = MyScriptWrapper.TryGetEntity(entity);
                if (item != null)
                {
                    item.MarkForClose();
                }
            }*/
            MissionTimer.RegisterTimerAction(1000, UnhideDebris, false);
            m_mission08_lookOnExplosion.Success();
            
            /*
            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter), MySoundCuesEnum.SfxShipLargeExplosion);
            
            MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans1).MarkForClose();
            MyScriptWrapper.GetEntity((uint)EntityID.HUBHackTrans2).MarkForClose();
            MyScriptWrapper.GetEntity((uint)EntityID.DestructionBuilding).MarkForClose();
            MyScriptWrapper.GetEntity((uint)EntityID.DestructionTransmitter).MarkForClose();
             * */
        }

        void UnhideDebris()
        {
            foreach (var item in m_debrisAfterExplosion)
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(item));
            }
        }

        void CloseToExplosion()
        {
            if (Vector3.DistanceSquared(MySession.PlayerShip.GetPosition(), MyScriptWrapper.GetEntity((uint)EntityID.DestructionBuilding).GetPosition()) < 500 * 500)

            {
                //MissionTimer.ClearActions();
                MyScriptWrapper.DestroyPlayerShip();
                
            }
            else
            {
                m_mission07_runExplosion.Success();
            }
        }

        bool MoveMotherShipForwardDest(MyEntity entity, float speed, Vector3 destination)
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            if (Vector3.DistanceSquared(destination, entity.GetPosition()) > 10 * 10)
            {
                MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                return false;
            }
            return true;
        }

        void MoveMotherShipForward(MyEntity entity, float speed)
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
        }


        
    }
       
}

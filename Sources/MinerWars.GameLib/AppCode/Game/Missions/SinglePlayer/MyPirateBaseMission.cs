using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyPirateBaseMission : MyMission
    {
        enum EntityID
        {
            FIND_PIRATE_BASE_LOCATION = 548180,

            DETECTOR_ID_1 = 247687,
            DETECTOR_ID_2 = 247688,
            DETECTOR_ID_3 = 247714,
            DETECTOR_ID_4 = 247716,
            DETECTOR_ID_5 = 247717,
            DETECTOR_ID_6 = 247718,
            DETECTOR_ID_7 = 247720,
            DETECTOR_ID_8 = 247719,
            DETECTOR_ID_9 = 247715,
            DETECTOR_ID_10 = 247712,
            DETECTOR_ID_11 = 247723,

            TURRET_ID_1 = 247685,
            TURRET_ID_2 = 247692,
            TURRET_ID_3 = 247694,
            TURRET_ID_4 = 247696,
            TURRET_ID_5 = 247698,
            TURRET_ID_6 = 247702,
            TURRET_ID_7 = 247704,
            TURRET_ID_8 = 247706,
            TURRET_ID_9 = 247710,
            TURRET_ID_10 = 247708,
            TURRET_ID_11 = 247721,

            LIGHT_ID_1 = 247713,
            LIGHT_ID_2 = 259371,
            LIGHT_ID_3 = 259373,
            LIGHT_ID_4 = 259376,
            LIGHT_ID_5 = 259380,
            LIGHT_ID_6 = 259359,
            LIGHT_ID_7 = 259363,
            LIGHT_ID_8 = 259366,
            LIGHT_ID_9 = 259367,
            LIGHT_ID_10 = 259377,
            LIGHT_ID_11 = 259361,

            AMBUSH_1_DETECTOR = 547685,
            AMBUSH_1_SPAWNPOINT_A = 547191,
            AMBUSH_1_SPAWNPOINT_B = 553601,

            AMBUSH_2_DETECTOR = 547686,
            AMBUSH_2_SPAWNPOINT_A = 547192,
            AMBUSH_2_SPAWNPOINT_B = 553600,

            AMBUSH_3_DETECTOR = 547687,
            AMBUSH_3_SPAWNPOINT_A = 548182,
            AMBUSH_3_SPAWNPOINT_B = 553599,

            AMBUSH_4_DETECTOR = 548183,
            AMBUSH_4_SPAWNPOINT_A = 547193,
            AMBUSH_4_SPAWNPOINT_B = 553598,

            AMBUSH_5_DETECTOR = 560662,
            AMBUSH_5_SPAWNPOINT_A = 552954,
            AMBUSH_5_SPAWNPOINT_B = 553602,

            AMBUSH_6_DETECTOR = 547688,
            AMBUSH_6_SPAWNPOINT_A = 547190,
            AMBUSH_6_SPAWNPOINT_B = 553603,

            PIRATE_LEADER = 247736,
            PIRATE_SPAWNPOINT1 = 247652,
            PIRATE_SPAWNPOINT2 = 247653,
            PIRATE_SPAWNPOINT3 = 247654,
            PREPARE_DETECTOR = 558789,
            PREPARE_SPAWNPOINT0 = 259383,
            PREPARE_SPAWNPOINT1 = 259384,
            PREPARE_SPAWNPOINT2 = 259385,
            PREPARE_FOR_DEFENSE_LOCATION = 557978,
            TURRET_PREFABS_DUMMY = 228781,
            TURRET_PREFAB0 = 603561,
            TURRET_PREFAB1 = 603565,
            TURRET_PREFAB2 = 603555,
            TURRET_PREFAB3 = 603573,
            TURRET_PREFAB4 = 603569,
            TURRET_PREFAB5 = 603576,
            RUSSIAN_MOTHERSHIP0 = 132833,
            RUSSIAN_MOTHERSHIP1 = 247728,
            RUSSIAN_MOTHERSHIP2 = 247727,
            RUSSIAN_MOTHERSHIP_TARGET0 = 247726,
            RUSSIAN_MOTHERSHIP_TARGET1 = 247733,
            RUSSIAN_MOTHERSHIP_TARGET2 = 247735,
            RUSSIAN_MOTHERSHIP_FLEE_TARGET0 = 556291,
            RUSSIAN_MOTHERSHIP_FLEE_TARGET1 = 556290,
            RUSSIAN_MOTHERSHIP_FLEE_TARGET2 = 556292,
            PIRATE_LEADER_BASE_WAYPOINT = 557985,
            PIRATES_SPLIT_WAYPOINT = 553676,
            BASE_ATACKERS_DETECTOR = 557157,
            BASE_GENERATOR = 557158,
            PIRATE_MOTHERSHIP_CONTAINER = 555325,
            RUSSIAN_MOTHERSHIP_CONTAINER = 132832,
            PIRATE_MOTHERSHIP_DESTINATION = 555487,
            MADELYN_DUMMY = 560586,

            ASTEROID1 = 247667,
            ASTEROID2 = 247666,
            ASTEROID3 = 247682,
        }

        uint[] m_defendSpawnpoints = { 228814, 553607, 553605, 228815, 553604, 228813, 553628, 553625, 553626, 553627 };
        uint[] m_allySpawnpoints = { 553752, 553747, 553746, 553750, 553751, 553748 };
        string[] m_piratePatrolRoutes = { "Wing1", "Wing2", "center" };

        private MyEntityDetector m_baseAtackersDetector { get; set; }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }

            foreach (var id in m_defendSpawnpoints)
            {
                MyScriptWrapper.GetEntity(id);
            }
        }

        MyEntity GetEntity(EntityID entityId)
        {
            return MyScriptWrapper.GetEntity((uint)entityId);
        }

        enum PirateMissionStage
        {
            FirstWaveAttacking,
            SecondWaveAttacking,
            PirateLeaderGoingToPlayer,
            PirateLeaderEscortingPlayer,
            SettingUpDefenses,
            Defending,
            GetBackToMotherShip,
        }

        private const int NUMBER_OF_TURRETS_TO_BUILD = 6;
        private const MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum TURRET_PREFAB_TYPE =
            MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON;

        private const float PIRATE_SHIP_HEALTH_HANDICAP = 0.5f;

        private readonly MyPrefabLargeShip[] m_russianMotherships = new MyPrefabLargeShip[3];
        private readonly MyLine[] m_russianMothershipTrajectories = new MyLine[3];
        private readonly MyLine[] m_russianMothershipTrajectories2 = new MyLine[3];
        private MyLine m_pirateNothershipTrajectory;

        private readonly Dictionary<uint, uint> m_detectorTurretMapping = new Dictionary<uint, uint>();
        private readonly Dictionary<uint, uint> m_detectorLightMapping = new Dictionary<uint, uint>();
        private readonly List<uint> m_dummiesForTurrets = new List<uint>();
        private int m_turretsBuilt;

        private readonly MyTimedObjective m_defendObjective;
        private readonly MyTimedObjective m_buildDefensesObjective;
        private readonly MyObjective m_getTurrets;
        private readonly MyObjective m_allyArrivedObjective;

        private MyEntityDetector m_currentDetector;
        private MyHudNotification.MyNotification m_canBuildNotification;
        private MyHudNotification.MyNotification m_remainingTurretsNotification;
        //private MyHudNotification.MyNotification m_canSkipNotification;
        private MyHudNotification.MyNotification m_generatorUnderAttackNotification;
        private object[] m_actionKeyString = new object[1];

        private MyEntityDetector m_wave1Detector;
        private MyEntityDetector m_wave6Detector;

        public MyPirateBaseMission()
        {
            ID = MyMissionID.PIRATE_BASE; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("04-Pirate base");
            Name = MyTextsWrapperEnum.PIRATE_BASE;
            Description = MyTextsWrapperEnum.PIRATE_BASE_Description;
            Flags = MyMissionFlags.Story;
            
            MyMwcVector3Int baseSector = new MyMwcVector3Int(190921, 0, 2152692);

            /* sector where the mission is located */
            Location = new MyMissionLocation(baseSector, 228782);

            RequiredMissions = new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.PIRATE_BASE_ALLY_ARRIVED };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>();

            // == Objective 01 ==
            var travel = new MyObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_TRAVEL_TO_BASE_Name),
                MyMissionID.PIRATE_BASE_TRAVEL_TO_BASE,
                (MyTextsWrapperEnum.PIRATE_BASE_TRAVEL_TO_BASE_Description),
                null,
                this,
                new MyMissionID[] {  },
                new MyMissionLocation(baseSector, (uint)EntityID.FIND_PIRATE_BASE_LOCATION)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_1_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_1_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_1_SPAWNPOINT_B } }));

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_2_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_2_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_2_SPAWNPOINT_B } }));

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_3_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_3_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_3_SPAWNPOINT_B } }));

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_4_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_4_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_4_SPAWNPOINT_B } }));

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_5_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_5_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_5_SPAWNPOINT_B } }));

            travel.Components.Add(new MySpawnpointWaves((uint)EntityID.AMBUSH_6_DETECTOR, 0,
                new List<uint[]> { new uint[] { (uint)EntityID.AMBUSH_6_SPAWNPOINT_A }, new uint[] { (uint)EntityID.AMBUSH_6_SPAWNPOINT_B } }));

            //travel.OnMissionSuccess += ArrivedToBase;
            travel.OnMissionLoaded += Travel_OnMissionLoaded;
            travel.OnMissionCleanUp += Travel_OnMissionUnload;
            m_objectives.Add(travel);

            // == Objective 02 ==
            var speakWithPirates = new MyMeetObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_SPEAK_WITH_PIRATES_Name),
                MyMissionID.PIRATE_BASE_SPEAK_WITH_PIRATES,
                (MyTextsWrapperEnum.PIRATE_BASE_SPEAK_WITH_PIRATES_Description),
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_TRAVEL_TO_BASE },
                null, (uint)EntityID.PIRATE_LEADER, 300) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudPirateCaptain };
            speakWithPirates.OnMissionLoaded += SpeakWithPirates_OnMissionLoaded;
            m_objectives.Add(speakWithPirates);

            // == Objective 03 ==
            var listenToCaptain = new MyObjectiveDialog(
                MyMissionID.PIRATE_BASE_LISTEN_TO_CAPTAIN,
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_SPEAK_WITH_PIRATES },
                MyDialogueEnum.PIRATE_BASE_0600);
            listenToCaptain.OnMissionLoaded += ListenToCaptain_OnMissionLoaded;
            listenToCaptain.OnMissionCleanUp += ListenToCaptain_OnMissionUnload;
            m_objectives.Add(listenToCaptain);

            // == Objective 04 ==
            var prepareForDefense = new MyObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_PREPARE_FOR_DEFENSE_Name),
                MyMissionID.PIRATE_BASE_PREPARE_FOR_DEFENSE,
                (MyTextsWrapperEnum.PIRATE_BASE_PREPARE_FOR_DEFENSE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_LISTEN_TO_CAPTAIN },
                new MyMissionLocation(baseSector, (uint)EntityID.PREPARE_FOR_DEFENSE_LOCATION),
                startDialogId: MyDialogueEnum.PIRATE_BASE_0800) { SaveOnSuccess = true };
            prepareForDefense.Components.Add(new MySpawnpointWaves((uint)EntityID.PREPARE_DETECTOR, 0, new List<uint[]> { new uint[] { (uint)EntityID.PREPARE_SPAWNPOINT0, (uint)EntityID.PREPARE_SPAWNPOINT1, (uint)EntityID.PREPARE_SPAWNPOINT2 } }));
            prepareForDefense.OnMissionLoaded += PrepareForDefense_OnMissionLoaded;
            m_objectives.Add(prepareForDefense);

            // == Objective 05 ==
            m_getTurrets = new MyUseObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_GET_TURRETS_Name),
                MyMissionID.PIRATE_BASE_GET_TURRETS,
                (MyTextsWrapperEnum.PIRATE_BASE_GET_TURRETS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_PREPARE_FOR_DEFENSE },
                new MyMissionLocation(baseSector, (uint)EntityID.TURRET_PREFABS_DUMMY),
                MyTextsWrapperEnum.PressToTakeTurrets,
                MyTextsWrapperEnum.Turrets,
                MyTextsWrapperEnum.TransferInProgress,
                3000,
                objectiveType: MyUseObjectiveType.Taking) { SaveOnSuccess = true };
            m_getTurrets.OnMissionLoaded += GetTurrets_OnMissionLoaded;
            m_getTurrets.OnMissionSuccess += GetTurrets_OnMissionSuccess;
            m_objectives.Add(m_getTurrets);

            // == Objective 06 ==
            m_buildDefensesObjective = new MyTimedObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_DEFENSE_SETUP_Name),
                MyMissionID.PIRATE_BASE_DEFENSE_SETUP,
                (MyTextsWrapperEnum.PIRATE_BASE_DEFENSE_SETUP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_GET_TURRETS },
                new TimeSpan(0, 0, minutes: 5, seconds: 0)
                ) { SaveOnSuccess = true };
            
            m_buildDefensesObjective.OnMissionLoaded += BuildDefenses_OnMissionLoaded;
            m_buildDefensesObjective.OnMissionUpdate += BuildDefenses_OnMissionUpdate;
            m_buildDefensesObjective.OnMissionCleanUp += BuildDefenses_OnMissionUnloaded;
            m_buildDefensesObjective.OnMissionSuccess += BuildDefenses_OnMissionSuccess;
            m_objectives.Add(m_buildDefensesObjective);

            // == Objective 07 ==
            m_defendObjective = new MyTimedObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_DEFEND_Name),
                MyMissionID.PIRATE_BASE_DEFEND,
                (MyTextsWrapperEnum.PIRATE_BASE_DEFEND_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_DEFENSE_SETUP },
                new TimeSpan(0, 0, minutes: 5, seconds: 0),
                startDialogId: MyDialogueEnum.PIRATE_BASE_0900
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudGenerator };
            
            m_defendObjective.OnMissionLoaded += Defend_OnMissionLoaded;
            m_defendObjective.OnMissionCleanUp += Defend_OnMissionUnloaded;
            m_defendObjective.OnMissionUpdate += Defend_OnMissionUpdate;
            m_defendObjective.OnMissionSuccess += Defend_OnMissionSuccess;

            m_defendObjective.Components.Add(new MyHeadshake((3*60)*1000, MyHeadshake.DefaultShaking));
            m_defendObjective.Components.Add(new MyHeadshake((6*60)*1000, MyHeadshake.DefaultShaking));
            m_defendObjective.Components.Add(new MyHeadshake((9*60)*1000, MyHeadshake.DefaultShaking));

            m_objectives.Add(m_defendObjective);

            // == Objective 08 ==
            m_allyArrivedObjective = new MyObjective(
                (MyTextsWrapperEnum.PIRATE_BASE_ALLY_ARRIVED_Name),
                MyMissionID.PIRATE_BASE_ALLY_ARRIVED,
                (MyTextsWrapperEnum.PIRATE_BASE_ALLY_ARRIVED_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_DEFEND },
                null,
                startDialogId: MyDialogueEnum.PIRATE_BASE_1100) { SaveOnSuccess = true };
            m_allyArrivedObjective.OnMissionLoaded += AllyArrived_OnMissionLoaded;
            m_allyArrivedObjective.OnMissionCleanUp += AllyArrived_OnMissionUnload;
            m_allyArrivedObjective.OnMissionUpdate += AllyArrived_OnMissionUpdate;
            m_allyArrivedObjective.OnMissionSuccess += AllyArrived_OnMissionSuccess;
            m_objectives.Add(m_allyArrivedObjective);

            /*
            var returnBackToMotherShip = new MyObjective(
                new StringBuilder("Return to your mothership"),
                MyMissionID.PIRATE_BASE_RETURN_BACK_TO_MOTHERSHIP,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_ALLY_ARRIVED },
                new MyMissionLocation(baseSector, (uint)EntityID.MADELYN_DUMMY)
            ) { SaveOnSuccess = true };
            m_objectives.Add(returnBackToMotherShip);
             */
        }


        private MyGuiScreenUseProgressBar m_useProgress;
        private bool m_inUse;

        public override void Load()
        {
            
            MyScriptWrapper.OnBotReachedWaypoint += BotReachedWaypoint;
            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath += OnEntityDeath;

            m_actionKeyString[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE);
            
            m_canBuildNotification = MyScriptWrapper.CreateNotification(
                Localization.MyTextsWrapperEnum.PressToBuildTurret,
                MyGuiManager.GetFontMinerWarsBlue());
            m_canBuildNotification.SetTextFormatArguments(m_actionKeyString);

            m_remainingTurretsNotification = MyScriptWrapper.CreateNotification(
                Localization.MyTextsWrapperEnum.RemainingTurrets,
                MyGuiManager.GetFontMinerWarsBlue());

            //m_canSkipNotification = MyScriptWrapper.CreateNotification(
            //    Localization.MyTextsWrapperEnum.PressToSkipTimer,
            //    MyGuiManager.GetFontMinerWarsBlue());
            //m_canSkipNotification.SetTextFormatArguments(m_actionKeyString);
            //m_canSkipNotification.Disappear();

            m_generatorUnderAttackNotification = MyScriptWrapper.CreateNotification(
                Localization.MyTextsWrapperEnum.GeneratorUnderAttack,
                MyGuiManager.GetFontMinerWarsRed(), 3000);
            m_generatorUnderAttackNotification.Disappear();


            m_useProgress = new MyGuiScreenUseProgressBar(MyTextsWrapperEnum.Turrets, MyTextsWrapperEnum.BuildingInProgress,0f, MySoundCuesEnum.SfxProgressBuild, MySoundCuesEnum.SfxCancelBuild, MyGameControlEnums.USE, 0, 3000, 0);
            m_useProgress.OnCanceled += OnCanceledHandler;
            m_useProgress.OnSuccess += OnSuccessHandler;

            SetupMotherships();
            SetUpTurrets();

            base.Load();
        }

        private void OnSuccessHandler(object sender, EventArgs e)
        {
            TryBuildTurret();
            m_inUse = false;
        }

        private void OnCanceledHandler(object sender, EventArgs e)
        {
            m_inUse = false;
        }


        private void StartUse()
        {
            m_inUse = true;
            m_useProgress.Reset();
            MyGuiManager.AddScreen(m_useProgress);
        }



        public override void Unload()
        {
            base.Unload();

            m_russianMotherships[0] = null;
            m_russianMotherships[1] = null;
            m_russianMotherships[2] = null;

            m_detectorTurretMapping.Clear();
            m_detectorLightMapping.Clear();
            m_dummiesForTurrets.Clear();

            m_currentDetector = null;
            m_wave1Detector = null;
            m_wave6Detector = null;

            MyScriptWrapper.OnBotReachedWaypoint -= BotReachedWaypoint;
            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
        }

        private void BotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            MySmallShipBot smallShipBot = bot as MySmallShipBot;
            Debug.Assert(smallShipBot != null);
            if (smallShipBot == null)
            {
                return;
            }

            // Set pirate leader to patrol around base
            if (MyScriptWrapper.GetEntityId(smallShipBot) == (uint)EntityID.PIRATE_LEADER &&
                MyScriptWrapper.GetEntityId(waypoint) == (uint)EntityID.PIRATE_LEADER_BASE_WAYPOINT)
            {
                smallShipBot.PatrolMode = MyPatrolMode.CYCLE;
                smallShipBot.SetWaypointPath("CapainPat");
            }

            // Split pirates
            if (MyScriptWrapper.GetEntityId(waypoint) == (uint)EntityID.PIRATES_SPLIT_WAYPOINT &&
                smallShipBot.WaypointPath != null && smallShipBot.WaypointPath.Name == "AttPir")
            {
                List<MySmallShipBot> ships = new List<MySmallShipBot>();
                ships.Add(smallShipBot);
                ships.AddRange(smallShipBot.Followers);
                Debug.Assert(ships.Count == 9);

                if (ships.Count == 9)   // dont split
                {
                    foreach (var ship in ships)
                    {
                        MyScriptWrapper.StopFollow(ship);
                    }

                    MySmallShipBot leader = null;
                    for (int i = 0; i < ships.Count; i++)
                    {
                        if (i % 3 == 0 && i <= 6)
                        {
                            leader = ships[i];
                            leader.PatrolMode = MyPatrolMode.CYCLE;
                            leader.SetWaypointPath(m_piratePatrolRoutes[i / 3]);     // PirSup1, PirSup2, PirSup3
                            leader.Patrol();
                        }
                        else
                        {
                            ships[i].Follow(leader);
                        }
                    }
                }
            }
        }

        private void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            var pirateLeader = GetEntity(EntityID.PIRATE_LEADER) as MySmallShipBot;

            var spawnpointId = MyScriptWrapper.GetEntityId(spawnpoint);
            switch (spawnpointId)
            {
                case (uint)EntityID.PIRATE_SPAWNPOINT1:
                case (uint)EntityID.PIRATE_SPAWNPOINT2:
                case (uint)EntityID.PIRATE_SPAWNPOINT3:
                    MyScriptWrapper.SetHealth(bot, PIRATE_SHIP_HEALTH_HANDICAP);
                    MyScriptWrapper.StopFollow(bot);
                    if (pirateLeader != null)
                    {
                        MyScriptWrapper.Follow(pirateLeader, bot);
                    }
                    break;
            }

            foreach (var defendSpawnpointId in m_defendSpawnpoints)
            {
                if (spawnpointId == defendSpawnpointId)
                {
                    MyScriptWrapper.SetSleepDistance(bot, 5000);
                    break;
                }
            }
        }

        private void OnEntityDeath(MyEntity victim, MyEntity killer)
        {
            uint id = MyScriptWrapper.GetEntityId(victim);
            if (id == (uint)EntityID.BASE_GENERATOR)
            {
                Fail(MyTextsWrapperEnum.PirateBaseGeneratorFailMessage);
            }

            foreach (var item in m_detectorTurretMapping)
            {
                if (id == item.Value)
                {
                    var turretLight = MyScriptWrapper.TryGetEntity(m_detectorLightMapping[item.Key]);
                    if (turretLight != null)
                    {
                        MyScriptWrapper.SetLight(turretLight, false);
                    }
                    break;
                }
            }
        }

        void Travel_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Pirates, MyFactions.RELATION_NEUTRAL);

            MyScriptWrapper.HideEntity(GetEntity(EntityID.PIRATE_MOTHERSHIP_CONTAINER));
            MyScriptWrapper.HideEntity(GetEntity(EntityID.RUSSIAN_MOTHERSHIP_CONTAINER));
            
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_0100);
            MyScriptWrapper.OnDialogueFinished += Travel_OnDialogueFinished;
            MyScriptWrapper.SpawnpointBotsKilled += Travel_SpawnpointBotsKilled;
            m_wave1Detector = InitDetector((uint)EntityID.AMBUSH_1_DETECTOR, Travel_Wave1Activated);
            m_wave6Detector = InitDetector((uint)EntityID.AMBUSH_6_DETECTOR, Travel_Wave6Activated);
        }

        void Travel_OnMissionUnload(MyMissionBase sender)
        {
            MyScriptWrapper.OnDialogueFinished -= Travel_OnDialogueFinished;
            MyScriptWrapper.SpawnpointBotsKilled -= Travel_SpawnpointBotsKilled;
            CleanUpDetector(m_wave1Detector, Travel_Wave6Activated);
            CleanUpDetector(m_wave6Detector, Travel_Wave6Activated);
        }

        void Travel_OnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.PIRATE_BASE_0200)
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Pirates, MyFactions.RELATION_WORST);
            }
        }

        void Travel_SpawnpointBotsKilled(MySpawnPoint spawnPoint)
        {
            if (spawnPoint == MyScriptWrapper.TryGetEntity((uint)EntityID.AMBUSH_1_SPAWNPOINT_B))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_0300);
            }

            if (spawnPoint == MyScriptWrapper.TryGetEntity((uint)EntityID.AMBUSH_6_SPAWNPOINT_B))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_0500);
            }
        }

        void Travel_Wave1Activated(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_0200);
                CleanUpDetector(m_wave1Detector, Travel_Wave1Activated);
            }
        }

        void Travel_Wave6Activated(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_0400);
                CleanUpDetector(m_wave6Detector, Travel_Wave6Activated);
            }
        }

        void SpeakWithPirates_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.PIRATE_SPAWNPOINT1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.PIRATE_SPAWNPOINT2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.PIRATE_SPAWNPOINT3);

            var pirateLeader = GetEntity(EntityID.PIRATE_LEADER) as MySmallShipBot;
            MyScriptWrapper.SetSleepDistance(pirateLeader, 3000);

            HighlightTurrets();
        }

        void ListenToCaptain_OnMissionLoaded(MyMissionBase sender)
        {
            var pirateLeader = GetEntity(EntityID.PIRATE_LEADER) as MySmallShipBot;
            if (pirateLeader != null)
            {
                pirateLeader.LookTarget = MySession.PlayerShip;
            }

            HighlightTurrets();
        }

        void ListenToCaptain_OnMissionUnload(MyMissionBase sender)
        {
            var pirateLeader = GetEntity(EntityID.PIRATE_LEADER) as MySmallShipBot;
            if (pirateLeader != null)
            {
                pirateLeader.LookTarget = null;
            }
        }

        void PrepareForDefense_OnMissionLoaded(MyMissionBase sender)
        {
            var pirateLeader = GetEntity(EntityID.PIRATE_LEADER) as MySmallShipBot;

            if (pirateLeader != null)
            {
                var x = MyFactions.GetFactionsRelation(MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_FactionEnum.Rainiers, CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_FactionEnum.Pirates); 
                
                pirateLeader.LookTarget = null;
                pirateLeader.SetWaypointPath("CaptWay");
                pirateLeader.PatrolMode = MyPatrolMode.ONE_WAY;
                pirateLeader.Patrol();

                if (pirateLeader.Followers.Count > 0)
	            {
                    var newLeader = pirateLeader.Followers[0];
                    MyScriptWrapper.StopFollow(newLeader);

                    var followers = pirateLeader.Followers.ToArray();
                    for (int i = 0; i < followers.Length; i++)
			        {
                        MyScriptWrapper.StopFollow(followers[i]);
                        MyScriptWrapper.Follow(newLeader, followers[i]);
			        }

                    newLeader.SetWaypointPath("AttPir");
                    newLeader.PatrolMode = MyPatrolMode.ONE_WAY;
                    newLeader.Patrol();
	            }
            }
        }

        void GetTurrets_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB0, true, m_getTurrets);
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB1, true, m_getTurrets);
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB2, true, m_getTurrets);
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB3, true, m_getTurrets);
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB4, true, m_getTurrets);
            MyScriptWrapper.Highlight((uint)EntityID.TURRET_PREFAB5, true, m_getTurrets);

            HighlightTurrets();
        }

        void GetTurrets_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB0));
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB1));
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB2));
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB3));
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB4));
            MyScriptWrapper.CloseEntity(GetEntity(EntityID.TURRET_PREFAB5));
            
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                (int)TURRET_PREFAB_TYPE,
                NUMBER_OF_TURRETS_TO_BUILD, true);    // TODO: will be changed to central inventory
        }

        void BuildDefenses_OnMissionLoaded(MyMissionBase sender)
        {
            HighlightTurrets();

            m_turretsBuilt = 0; 
            
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_1);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_2);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_3);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_4);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_5);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_6);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_7);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_8);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_9);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_10);
            m_buildDefensesObjective.MissionEntityIDs.Add((uint)EntityID.TURRET_ID_11);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush);
            MyScriptWrapper.UnhideEntity(GetEntity(EntityID.RUSSIAN_MOTHERSHIP_CONTAINER));

            foreach (var dummyId in m_dummiesForTurrets)
            {
                var detector = MyScriptWrapper.GetDetector(dummyId);
                Debug.Assert(detector != null);
                if (detector != null)
                {
                    // register for player ship enter
                    detector.OnEntityEnter += TurretDummyEnter;
                    detector.OnEntityLeave += TurretDummyLeave;
                    detector.On();
                }
            }
            
            UpdateRemainingTurretsCount();
            MyScriptWrapper.AddNotification(m_remainingTurretsNotification);

            MyScriptWrapper.OnUseKeyPress += BuildDefenses_OnUseKeyPress;
        }

        void BuildDefenses_OnMissionUpdate(MyMissionBase sender)
        {
            UpdateRemainingTurretsCount();
        }

        void BuildDefenses_OnMissionUnloaded(MyMissionBase sender)
        {
            m_buildDefensesObjective.MissionEntityIDs.Clear();

            foreach (var dummyTurretPair in m_dummiesForTurrets)
            {
                var detector = MyScriptWrapper.GetDetector(dummyTurretPair);
                Debug.Assert(detector != null);
                if (detector != null)
                {
                    // register for player ship enter
                    detector.OnEntityEnter -= TurretDummyEnter;
                    if (detector.IsOn())
                    {
                        detector.Off();
                    }
                }
            }

            MyScriptWrapper.OnUseKeyPress -= BuildDefenses_OnUseKeyPress;

            MyScriptWrapper.GetCentralInventory().RemoveInventoryItems(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                (int)TURRET_PREFAB_TYPE, true);
        }

        private void BuildDefenses_OnMissionSuccess(MyMissionBase sender)
        {
            foreach (var detectorTurretPair in m_detectorTurretMapping)
            {
                var turret = MyScriptWrapper.GetEntity(detectorTurretPair.Value) as MyPrefabLargeWeapon;
                if (turret != null && !turret.Visible)
                {
                    turret.MarkForClose();
                }
            }

            foreach (var detectorLight in m_detectorLightMapping)
            {
                var detector = (MyEntityDetector)MyScriptWrapper.GetEntity(detectorLight.Key);
                var light = (MyPrefabLight)MyScriptWrapper.GetEntity(detectorLight.Value);
                var turret = MyScriptWrapper.GetEntity(m_detectorTurretMapping[detectorLight.Key]) as MyPrefabLargeWeapon;

                if (detector.IsOn())
                {
                    MyScriptWrapper.SetLight(light, false);
                    MyScriptWrapper.CloseEntity(turret);
                    //light.GetLight().LightOn = false;
                    //light.SetAllColors(new Color(m_turretHighlightColor.X,m_turretHighlightColor.Y,m_turretHighlightColor.Z));
                }
            }
            
            //m_canSkipNotification.Disappear();
            m_remainingTurretsNotification.Disappear();            
        }

        void BuildDefenses_OnUseKeyPress()
        {
            if (/*m_canSkipNotification.IsDisappeared() && */!m_inUse)
            {
                if (m_currentDetector != null)
                {
                    StartUse();
                }
            }
            //else
            //{
            //    m_buildDefensesObjective.SkipTimer();
            //    m_canSkipNotification.Disappear();
            //}
        }

        private void TurretDummyEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                m_currentDetector = sender;

                m_canBuildNotification.Appear();
                MyScriptWrapper.AddNotification(m_canBuildNotification);
            }
        }

        private void TurretDummyLeave(MyEntityDetector sender, MyEntity entity)
        {
            if (entity == MySession.PlayerShip)
            {
                m_currentDetector = null;
                m_canBuildNotification.Disappear();
            }
        }

        private void TryBuildTurret()
        {
            if (m_currentDetector != null)
            {
                var inventory = MyScriptWrapper.GetCentralInventory();
                bool canBuild = inventory.RemoveInventoryItemAmount(
                    MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                    (int)TURRET_PREFAB_TYPE,
                    1);

                if (canBuild)
                {
                    m_buildDefensesObjective.MissionEntityIDs.Remove(
                        m_detectorTurretMapping[MyScriptWrapper.GetEntityId(m_currentDetector)]);
                    ++m_turretsBuilt;

                    BuildTurret();
                    //m_currentDetector.Parent.Close();
                    m_currentDetector.Off();
                    m_currentDetector = null;

                    int remainingTurretCount = inventory.GetInventoryItemsCount(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)TURRET_PREFAB_TYPE);
                    if (remainingTurretCount <= 0)
                    {
                        m_buildDefensesObjective.MissionEntityIDs.Clear();
                    }
                }
            }
        }

        private void BuildTurret()
        {
            var turret =
                MyScriptWrapper.GetEntity(m_detectorTurretMapping[m_currentDetector.EntityId.Value.NumericValue])
                as MyPrefabLargeWeapon;

            Debug.Assert(turret != null);
              if (turret != null)
            {
                MyScriptWrapper.Highlight(m_detectorTurretMapping[m_currentDetector.EntityId.Value.NumericValue], false,this);
                MyScriptWrapper.EnablePhysics(m_detectorTurretMapping[m_currentDetector.EntityId.Value.NumericValue], true);
                turret.Enabled = true;
                turret.IsDestructible = true;
            }

            var light =
                MyScriptWrapper.GetEntity(m_detectorLightMapping[m_currentDetector.EntityId.Value.NumericValue])
                as MyPrefabLight;

            if (light != null)
            {
                light.SetAllColors(new Color(0f, 1f, 0f));
                light.Effect = MyLightEffectTypeEnum.DISTANT_GLARE;
            }

            UpdateRemainingTurretsCount();
            m_canBuildNotification.Disappear();
        }

        private void UpdateRemainingTurretsCount()
        {
            var inventory = MyScriptWrapper.GetCentralInventory();
            var remainingTurretsCount = Convert.ToInt32(inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)TURRET_PREFAB_TYPE));
            if (remainingTurretsCount <= 0)
            {
                m_remainingTurretsNotification.Disappear();
                if (remainingTurretsCount != int.MinValue)
                {
                    remainingTurretsCount = int.MinValue;  // <- debug&continue
                    m_buildDefensesObjective.MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(5), DefensesBuilt, false);
                }
                
                //m_canSkipNotification.Appear();
                //MyScriptWrapper.AddNotification(m_canSkipNotification);
            }
            else
            {
                m_remainingTurretsNotification.SetTextFormatArguments(new object[] { m_turretsBuilt, remainingTurretsCount });
            }
        }

        private void DefensesBuilt()
        {
            m_buildDefensesObjective.SkipTimer();
        }

        private void Defend_OnMissionLoaded(MyMissionBase sender)
        {
            sender.MissionTimer.RegisterTimerAction(new TimeSpan(0, 3, 00), FirstAsteroidExplosion);
            sender.MissionTimer.RegisterTimerAction(new TimeSpan(0, 6, 00), SecondAsteroidExplosion);
            sender.MissionTimer.RegisterTimerAction(new TimeSpan(0, 9, 00), ThirdAsteroidExplosion);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA06");
            m_turretsBuilt = 0;

            var generator = MyScriptWrapper.TryGetEntity((uint)EntityID.BASE_GENERATOR);
            if (generator != null)
            {
                MyScriptWrapper.MarkEntity(generator, MyTexts.BaseGenerator, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS);
            }

            foreach (var spawnpointId in m_defendSpawnpoints)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnpointId);
            }

            m_baseAtackersDetector = InitDetector((uint)EntityID.BASE_ATACKERS_DETECTOR, OnAttackerInBase);

            MyScriptWrapper.OnEntityAtacked += Defend_EntityAttacked;
        }

        private void Defend_OnMissionUnloaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(100);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);
            CleanUpDetector(m_baseAtackersDetector, OnAttackerInBase);
            var generator = MyScriptWrapper.TryGetEntity((uint)EntityID.BASE_GENERATOR);
            if (generator != null)
            {
                generator.UpdateHudMarker(true);
            }

            m_baseAtackersDetector = null;
            MyScriptWrapper.OnEntityAtacked -= Defend_EntityAttacked;
        }

        private void Defend_OnMissionUpdate(MyMissionBase sender)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 newPosition = Vector3.Lerp(
                m_russianMothershipTrajectories[i].From,
                m_russianMothershipTrajectories[i].To,
                m_defendObjective.Progress);

                MyScriptWrapper.Move(m_russianMotherships[i], newPosition);
            }
        }

        private void Defend_OnMissionSuccess(MyMissionBase sender)
        {
            foreach (var spawnpointId in m_defendSpawnpoints)
            {
                MyScriptWrapper.DeactivateSpawnPoint(spawnpointId);
            }
        }

        void Defend_EntityAttacked(MyEntity attacker, MyEntity target)
        {
            var generator = MyScriptWrapper.TryGetEntity((uint)EntityID.BASE_GENERATOR);
            if (generator != null && target == generator)
            {
                m_generatorUnderAttackNotification.ResetAliveTime();
                m_generatorUnderAttackNotification.Appear();
                MyScriptWrapper.AddNotification(m_generatorUnderAttackNotification);
            }
        }

        void OnAttackerInBase(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MySmallShipBot bot = entity as MySmallShipBot;
            if (bot != null && MyFactions.GetFactionsRelation(MySession.PlayerShip.Faction, bot.Faction) == MyFactionRelationEnum.Enemy)
            {
                MyScriptWrapper.SetBotTarget(entity, GetEntity(EntityID.BASE_GENERATOR));
            }
        }

        private void FirstAsteroidExplosion()
        {
            var asteroid = GetEntity(EntityID.ASTEROID1) as MyStaticAsteroid;
            Debug.Assert(asteroid != null);
            if (asteroid != null)
            {
                MyScriptWrapper.MakeNuclearExplosion(asteroid);
                MyScriptWrapper.CloseEntity(asteroid);
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_1000);
            }
        }

        private void SecondAsteroidExplosion()
        {
            var asteroid = GetEntity(EntityID.ASTEROID2) as MyStaticAsteroid;
            Debug.Assert(asteroid != null);
            if (asteroid != null)
            {
                MyScriptWrapper.MakeNuclearExplosion(asteroid);
                MyScriptWrapper.CloseEntity(asteroid);
            }
        }

        private void ThirdAsteroidExplosion()
        {
            var asteroid = GetEntity(EntityID.ASTEROID3) as MyStaticAsteroid;
            Debug.Assert(asteroid != null);
            if (asteroid != null)
            {
                MyScriptWrapper.MakeNuclearExplosion(asteroid);
                MyScriptWrapper.CloseEntity(asteroid);
            }
        }

        private void AllyArrived_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.UnhideEntity(GetEntity(EntityID.PIRATE_MOTHERSHIP_CONTAINER), true); 
            
            m_russianMothershipTrajectories2[0] = new MyLine(
                m_russianMotherships[0].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_FLEE_TARGET0).GetPosition()
            );

            m_russianMothershipTrajectories2[1] = new MyLine(
                m_russianMotherships[1].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_FLEE_TARGET1).GetPosition()
            );

            m_russianMothershipTrajectories2[2] = new MyLine(
                m_russianMotherships[2].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_FLEE_TARGET2).GetPosition()
            );

            m_pirateNothershipTrajectory = new MyLine(
                    GetEntity(EntityID.PIRATE_MOTHERSHIP_CONTAINER).GetPosition(),
                    GetEntity(EntityID.PIRATE_MOTHERSHIP_DESTINATION).GetPosition()
                );

            MyScriptWrapper.OnDialogueFinished += AllyArrived_OnDialogueFinished;
        }

        private void AllyArrived_OnMissionUnload(MyMissionBase sender)
        {
            MyScriptWrapper.OnDialogueFinished -= AllyArrived_OnDialogueFinished;
        }

        private void AllyArrived_OnMissionUpdate(MyMissionBase sender)
        {
            float missionTime = (float)sender.MissionTimer.GetElapsedTime().TotalSeconds;

            float flyProgress = MathHelper.Clamp(missionTime / 120, 0, 1);

            for (int i = 0; i < 3; i++)
            {
                MyScriptWrapper.Move(
                    m_russianMotherships[i],
                    Vector3.Lerp(m_russianMothershipTrajectories2[i].From, m_russianMothershipTrajectories2[i].To, flyProgress));
            }

            float pirateflyProgress = 0;
            if (missionTime <= 115)
            {
                pirateflyProgress = flyProgress;
            }
            else
            {
                float slowdownTime = 5;
                float distance = (m_pirateNothershipTrajectory.From - m_pirateNothershipTrajectory.To).Length();
                float slowDownDistance = distance * slowdownTime / 120.0f;
                float slowDownSpeed = (distance - slowDownDistance) / (120.0f - slowdownTime);
                float deceleration = 2.0f * slowDownSpeed / (slowdownTime * slowdownTime);
                float decelerationTime = missionTime - (120.0f - slowdownTime);
                if (missionTime > 120)
                {
                    pirateflyProgress = 1;
                }
                else
                {
                    pirateflyProgress = distance * (120.0f - slowdownTime) / 120 + (slowDownSpeed * decelerationTime - 0.5f * deceleration * decelerationTime * decelerationTime);
                    pirateflyProgress = MathHelper.Clamp(pirateflyProgress / distance, 0, 1);
                }
            }

            MyScriptWrapper.Move(
                GetEntity(EntityID.PIRATE_MOTHERSHIP_CONTAINER),
                Vector3.Lerp(m_pirateNothershipTrajectory.From, m_pirateNothershipTrajectory.To, pirateflyProgress));
        }

        private void AllyArrived_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.HideEntity(GetEntity(EntityID.RUSSIAN_MOTHERSHIP_CONTAINER));
            MyScriptWrapper.Move(MyScriptWrapper.GetEntity("Madelyn"), GetEntity(EntityID.MADELYN_DUMMY).GetPosition());
        }

        void AllyArrived_OnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.PIRATE_BASE_1100)
            {
                m_allyArrivedObjective.MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(5), AllyArrived_ContinueDialogue, false);
            }

            if (dialogue == MyDialogueEnum.PIRATE_BASE_1200)
            {
                m_allyArrivedObjective.MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(1), AllyArrived_ContinueDialogue2, false);
            }

            if (dialogue == MyDialogueEnum.PIRATE_BASE_1400)
            {
                m_allyArrivedObjective.Success();
            }
        }

        void AllyArrived_ContinueDialogue()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_1200);
        }

        void AllyArrived_ContinueDialogue2()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.PIRATE_BASE_1400);
        }

        public override void Update()
        {
            base.Update();
        }

        private void SetupMotherships()
        {
            m_russianMotherships[0] = MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP0) as MyPrefabLargeShip;
            m_russianMotherships[1] = MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP1) as MyPrefabLargeShip;
            m_russianMotherships[2] = MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP2) as MyPrefabLargeShip;

            m_russianMothershipTrajectories[0] = new MyLine(
                m_russianMotherships[0].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_TARGET0).GetPosition()
            );

            m_russianMothershipTrajectories[1] = new MyLine(
                m_russianMotherships[1].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_TARGET1).GetPosition()
            );

            m_russianMothershipTrajectories[2] = new MyLine(
                m_russianMotherships[2].GetPosition(),
                MyScriptWrapper.GetEntity((uint)EntityID.RUSSIAN_MOTHERSHIP_TARGET2).GetPosition()
            );
        }

        private void SetUpTurrets()
        {
            m_detectorTurretMapping.Clear();
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_1).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_1);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_2).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_2);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_3).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_3);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_4).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_4);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_5).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_5);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_6).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_6);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_7).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_7);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_8).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_8);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_9).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_9);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_10).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_10);
            m_detectorTurretMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_11).EntityId.Value.NumericValue, (uint)EntityID.TURRET_ID_11);

            m_detectorLightMapping.Clear();
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_1).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_1);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_2).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_2);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_3).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_3);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_4).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_4);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_5).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_5);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_6).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_6);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_7).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_7);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_8).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_8);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_9).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_9);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_10).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_10);
            m_detectorLightMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_11).EntityId.Value.NumericValue, (uint)EntityID.LIGHT_ID_11);

            m_dummiesForTurrets.Clear();
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_1);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_2);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_3);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_4);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_5);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_6);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_7);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_8);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_9);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_10);
            m_dummiesForTurrets.Add((uint)EntityID.DETECTOR_ID_11);
        }

        private void HighlightTurrets()
        {
            foreach (var dummyTurretPair in m_detectorTurretMapping)
            {
                var turret = MyScriptWrapper.GetEntity(dummyTurretPair.Value) as MyPrefabLargeWeapon;
                MyScriptWrapper.Highlight(dummyTurretPair.Value, true, this);
                MyScriptWrapper.EnablePhysics(dummyTurretPair.Value, false);

                Debug.Assert(turret != null);
                if (turret != null)
                {
                    turret.Enabled = false;
                }
            }
        }

        private MyEntityDetector InitDetector(uint detectorID, OnEntityEnter handler)
        {
            MyEntityDetector detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(detectorID));
            detector.OnEntityEnter += handler;
            detector.On();
            return detector;
        }

        private void CleanUpDetector(MyEntityDetector detector, OnEntityEnter handler)
        {
            if (detector != null)
            {
                detector.OnEntityEnter -= handler;
                detector.Off();
            }
        }

    }
}
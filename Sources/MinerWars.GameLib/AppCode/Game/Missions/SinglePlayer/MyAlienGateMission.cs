#region Using 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Missions.Objectives;
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
using MinerWars.AppCode.Game.World.Global;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Game.GUI;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyAlienGateMission : MyMission
    {
        #region Members

        private const int FollowCoordinatesMadelynTime = 15000;
        private const int ContinueSearchingMadelynTime = 18000;
        private const int CoughtInTrapMoveTime = 75000;
        private const int RunForYourLifeObjectiveTime = 20000;
        private const int RunForYourLifeShipMoveTime = 14000;
        private const int RunForYourLifeDialogStartTime = 9000;
        private const int RunForYourLifeParticleStartTime = 11000;
        private const int RegroupWithMadelynMissionTime = 50000;
        private const int m_10ObjectiveTime = 40000;
        private const int m_18ObjectiveTime = 40000;
        private const int m_20ObjectiveTime = 8000;
        private const int m_21ObjectiveMovingTime = 9000;

        private readonly MyDestroyWavesObjective m_followCords;
        private MyEntityDetector m_followCoordinatesDetector;
        private MyEntity m_madelyn;

        #endregion

        #region EntityIDs
        private enum EntityID // list of IDs used in script
        {
            Voxel1 = 26431,
            Voxel2 = 16782567,
            Voxel3 = 26406,
            //Objective 01
            FollowCoordinatesDummy = 21305,
            FollowCoordinatesSpawnPoint = 21306,
            //Objective 02
            ContinueSearchingDummy = 3401,
            //Objective 03
            FollowDirectionDummy = 4756,
            FollowDirectionDetector = 21309,
            //Objective04
            CoughtInTrapDummy = 21311,
            CoughtInTrapSpawn1 = 21702,
            CoughtInTrapSpawn2 = 21700,
            CoughtInTrapShip3 = 15108,
            CoughtInTrapShip1 = 15616,
            CoughtInTrapShip1Target = 18725,
            CoughtInTrapShip2 = 17730,
            CoughtInTrapShip2Target = 18726,
            CoughtInTrapShip3Target = 95,
            //objective 05
            RunForYourLifeDummy = 21312,
            RunForYourLifeParticleEffect = 18723,
            RunForYourLifeMovingMadelyn = 4063,
            RunForYourLifeShipTarget = 14252,
            RunForYourLifeVoxel = 69,
            //Objective 06
            RegroupWithMadelynShip2 = 4297,
            RegroupWithMadelynShip1Target = 9471,
            RegroupWithMadelynShip2Target = 16609,
            RegroupWithMadelynShip3Target = 18724,
            RegroupWithMadelynShip4Target = 15276,
            RegroupWithMadelynDetector = 21694,
            RegroupWithMadelynSpawn1 = 21648,
            RegroupWithMadelynSpawn2 = 21650,
            RegroupWithMadelynDetector2 = 21696,
            RegroupWithMadelynSpawn3 = 21651,
            RegroupWithMadelynSpawn4 = 21652,
            //Objective 07
            BoardmotherShipDummy = 21785,
            BoardmotherShipDetector1 = 21786,
            BoardmotherShipDetector2 = 21789,
            BoardmotherShipDetector3 = 21792,
            BoardmotherShipSpawn1 = 21788,
            BoardmotherShipSpawn2 = 21791,
            BoardmotherShipSpawn3 = 21794,
            //Objective 08
            HackGeneratordestroy = 15618,
            HackGeneratorDummy = 22459,
            HackGeneratorDetector1 = 27235,
            HackGeneratorDetector2 = 27237,
            HackGeneratorDetector3 = 27241,
            HackGeneratorDetector4 = 27244,
            HackGeneratorDetector5 = 27248,
            HackGeneratorSpawn11 = 25533,
            HackGeneratorSpawn12 = 25534,
            HackGeneratorSpawn21 = 25552,
            HackGeneratorSpawn22 = 25530,
            HackGeneratorSpawn31 = 25550,
            HackGeneratorSpawn32 = 27243,
            HackGeneratorSpawn33 = 25551,
            HackGeneratorSpawn41 = 25529,
            HackGeneratorSpawn42 = 25535,
            HackGeneratorSpawn5 = 16785284,
            Ship1Interier = 15727,
            //Objective 09
            HackEngineDummy = 22467,
            HackEngineDetector3 = 27257,
            HackEngineDetector4 = 16787021,
            HackEngineDetector5 = 27261,
            HackEngineDetector6 = 27263,
            HackEngineSpawn2 = 25548,
            HackEngineSpawn3 = 27259,
            HackEngineSpawn4 = 25547,
            HackEngineSpawn51 = 25528,
            HackEngineSpawn52 = 25549,
            HackEngineSpawn6 = 25526,
            //Objective 10
            _10WayPoint = 21878,
            _10Spawn2 = 28070,
            _10DisableDummy = 22484,
            _10Dummy = 22487,
            _10DisableTurret = 15623,
            _10PrefabCont = 7357,
            //_10Explosiondummy = 16787928,
            BoardSecondShipSpawn1 = 22499,
            BoardSecondShipSpawn2 = 22500,

            //Objective 11
            BoardSecondShipbDetector = 22509,
            BoardSecondShipbDummy = 22522,
            BoardSecondShipbSpawn0 = 22572,
            BoardSecondShipbSpawn1 = 22576,
            BoardSecondShipbSpawn2 = 22596,
            //Objective 12
            HackGenerator2Doors = 17731,
            HackGenerator2Dummy = 23249,
            HackGenerator2Detector1 = 25582,
            HackGenerator2Detector2 = 25584,
            HackGenerator2Spawn11 = 25520,
            HackGenerator2Spawn12 = 25544,
            HackGenerator2Spawn2 = 25538,
            MotherShip2Interior = 17841,
            //Objective 13
            _13Dummy = 23251,
            _13Detector1 = 25602,
            _13Detector2 = 25586,
            _13Detector3 = 25588,
            _13Detector4 = 25590,
            _13Detector5 = 25592,
            _13Spawn11 = 25604,
            _13Spawn12 = 25542,
            _13Spawn13 = 25543,
            _13Spawn2 = 25536,
            _13Spawn3 = 25537,
            _13Spawn41 = 25532,
            _13Spawn42 = 25531,
            _13Spawn51 = 25545,
            _13Spawn52 = 25539,
            //Objective 14
            EnableDoorsHUB = 17857,
            EnableDoorsDoors = 22602,
            EnableDoorsDetector = 25594,
            EnableDoorsSpawnPoint = 25546,

            //Objective 15
            EnterLaboratoryDummy = 23253,
            EnterLaboratoryDetector = 25596,
            EnterLaboratorySpawn = 25525,
            //Objective 16
            _16Dummy = 23255,
            //Objective 17
            _17Dummy = 23257,
            _17Detector = 25598,
            _17Spawn1 = 25524,
            //_17Spawn2 = 25525,
            //Objective 18
            _18Dummy = 23263,
            //_18Detector1 = 25592 ,
            _18Spawn1 = 25605,
            _18DisableDummy = 23261,
            _18DisableTurret = 17737,
            MotherShip2Destroyed = 9791,
            //Objective 19
            _19Dummy = 23279,
            _19Spawn1 = 25513,
            _19Spawn2 = 25512,
            _19Ship = 14940,
            //Objective 20
            _20Prefab = 15074,
            _20ShipTarget = 14051,
            //objective 21
            _21Detector1 = 28077,
            _21SpawnPoint11 = 28074,
            _21SpawnPoint12 = 28076,
            _21VoxelAsteroidHide = 16782569,
            _21Prefab1UnHide = 13705,
            _21Ship2 = 13912,
            _21Ship2Target = 14062,
            _21Particle1 = 14049,
            _21Particle1Target = 14061,
            _21Particle2 = 14048,
            _21Particle2Target = 14060,
            _21ShipUnhide = 14395,
            _21ToDesroy = 14018,
            //Objective 22
            _22Dummy = 24017,
            _22Ship1 = 4808,
            _22Ship2 = 11081,
            _23Dummy = 24018,
            _23Shiptarget = 24620,
            RussianGeneralSpeak = 58,
            DoorsDialogue = 60,
            MadelynSRightWingPosition = 59,
        }

        private readonly List<uint> m_hackGeneratorCargos = new List<uint>() { 30069, 30057, 30061, 30071, 30058, 30062, 30059, 30063, 30070, 30060, 30064, 30072, 30948 };
        private readonly List<uint> m_hackGeneratorparticles = new List<uint>() { 28959, 28963, 29011, 28966, 28968, 29009, 29005, 29065 };
        private readonly List<uint> m_hackEngineparticles = new List<uint>() { 22454, 22457, 22458 };
        private readonly List<uint> m_10particlesEnable = new List<uint>() { 8640, 8639 };
        private readonly List<uint> m_10particlesDisable = new List<uint>() { 22454, 22457, 22458, 22483, 22486, 22485, 28959, 28963, 29011, 28966, 28968, 29009, 29005, 29065 };
        private readonly List<uint> m_10explosionDummies = new List<uint>() { 27, 26, 25, 24, 23, 19, 18, 17, 16, 15 };
        private readonly List<uint> m_boardSecondShipparticles = new List<uint>() { 8641, 8648, 9141, 8644, 9142, 8646, 9135, 9137, 9139, 9136, 9134, 9138 };
        private readonly List<uint> m_boardSecondShipbparticlesDisable = new List<uint>() { 8641, 8648, 9141, 8644, 9142, 8646, 9135, 9137, 9139, 9136, 9134, 9138 };
        private readonly List<uint> m_boardSecondShipbparticlesEnable = new List<uint>() { 22514, 22515, 22520 };
        private readonly List<uint> m_hackGenerator2Particles = new List<uint>() { 29069, 29072, 29074, 29109, 29077, 29165, 29170, 29168 };
        private readonly List<uint> m_hackGenerator2Cargos = new List<uint>() { 30065, 30066, 30049, 30053, 30050, 30054, 30067, 30068, 30052, 30056, 30055, 30051 };
        private readonly List<uint> m_13Particles = new List<uint>() { 28072, 28071, 28073 };

        private readonly List<uint> m_18EnableParticles = new List<uint>() { 23262, 23260, 23259 };

        private readonly List<uint> m_18EnableParticlesBoom = new List<uint>() { 10040, 10039 };
        private readonly List<uint> m_18DisableParticlesBoom = new List<uint>() { 23259, 23260, 23262, 28072, 28071, 28073, 29069, 29072, 29074, 29109, 29077, 29165, 29170, 29168 };
        private readonly List<uint> m_18explosionDummies = new List<uint>() { 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 };


        private readonly List<uint> m_19EnableParticles = new List<uint>() { 10042, 10043, 10059, 10046, 10038, 10041, 10045, 10057, 10037, 10044, 10061, 10062, };
        private readonly List<uint> m_19DisableParticles = new List<uint>() { 22514, 9134, 22515, 9141, 8648 };
        private readonly List<uint> m_21Enableparticles = new List<uint>() { 13703, 14048, 13736, 14049, 13737 };
        private readonly List<uint> m_21EnableparticlesDestroy = new List<uint>() { 14253, 14567, 14541, 14532, 14565 };
        private readonly List<uint> m_21DisableParticlesDestroy = new List<uint>() { 14049, 14048 };



        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)(((EntityID?)value).Value));
            }


            var list = new List<uint>();
            list.AddRange(m_hackGeneratorCargos);
            list.AddRange(m_hackGeneratorparticles);
            list.AddRange(m_hackEngineparticles);
            list.AddRange(m_10particlesEnable);
            list.AddRange(m_10particlesDisable);
            list.AddRange(m_boardSecondShipparticles);
            list.AddRange(m_boardSecondShipbparticlesDisable);
            list.AddRange(m_boardSecondShipbparticlesEnable);
            list.AddRange(m_hackGenerator2Cargos);
            list.AddRange(m_hackGenerator2Particles);
            list.AddRange(m_13Particles);
            list.AddRange(m_18EnableParticles);
            list.AddRange(m_19EnableParticles);
            list.AddRange(m_19DisableParticles);
            list.AddRange(m_21Enableparticles);
            list.AddRange(m_21EnableparticlesDestroy);
            list.AddRange(m_13Particles);
            list.AddRange(m_21DisableParticlesDestroy);

            foreach (var u in list)
            {
                MyScriptWrapper.GetEntity(u);
            }
        }
        #endregion

        public MyAlienGateMission()
        {
            ID = MyMissionID.ALIEN_GATE;
            DebugName = new StringBuilder("22-Alien artifact");
            Name = MyTextsWrapperEnum.ALIEN_GATE;
            Description = MyTextsWrapperEnum.ALIEN_GATE_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission31_AlienGate;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1202900, 0, -112652); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.FollowCoordinatesDummy); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.EAC_TRANSMITTER }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.ALIEN_GATE_23 };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            #region Objectives
            m_objectives = new List<MyObjective>();


            var keepFormation = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_RIGHT_WING_Name),
                MyMissionID.ALIEN_GATE_RIGHT_WING,
                (MyTextsWrapperEnum.ALIEN_GATE_RIGHT_WING_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.MadelynSRightWingPosition)) { HudName = MyTextsWrapperEnum.HudLeftWing };
            m_objectives.Add(keepFormation);


            m_followCords = new MyDestroyWavesObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_FOLLOW_COORDINATES_Name),
                MyMissionID.ALIEN_GATE_FOLLOW_COORDINATES,
                (MyTextsWrapperEnum.ALIEN_GATE_FOLLOW_COORDINATES_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_RIGHT_WING, },
                new List<uint>() { },
                new MyMissionLocation(baseSector, (uint)EntityID.FollowCoordinatesDummy),
                radiusOverride: 150) { StartDialogId = MyDialogueEnum.ALIEN_GATE_0100, SuccessDialogId = MyDialogueEnum.ALIEN_GATE_0300 };
            m_followCords.AddWave(new List<uint>() { (uint)EntityID.FollowCoordinatesSpawnPoint });
            m_followCords.OnMissionLoaded += M01FollowCordsOnOnMissionLoaded;
            m_followCords.OnMissionSuccess += M01FollowCordsOnOnMissionSuccess;
            m_followCords.Components.Add(new MyMovingEntity("Madelyn", (uint)EntityID.FollowCoordinatesDummy, FollowCoordinatesMadelynTime));
            m_objectives.Add(m_followCords);


            var continueSearching = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_CONTINUE_SEARCHING_Name),
                MyMissionID.ALIEN_GATE_CONTINUE_SEARCHING,
                (MyTextsWrapperEnum.ALIEN_GATE_CONTINUE_SEARCHING_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_FOLLOW_COORDINATES },
                new MyMissionLocation(baseSector, (uint)EntityID.ContinueSearchingDummy)
                ) { SaveOnSuccess = false, SuccessDialogId = MyDialogueEnum.ALIEN_GATE_0400, HudName = MyTextsWrapperEnum.HudSearch };
            continueSearching.Components.Add(new MyMovingEntity("Madelyn", (uint)EntityID.ContinueSearchingDummy, ContinueSearchingMadelynTime));
            m_objectives.Add(continueSearching);


            var followDirection = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_FOLLOW_DIRECTION_Name),
                MyMissionID.ALIEN_GATE_FOLLOW_DIRECTION,
                (MyTextsWrapperEnum.ALIEN_GATE_FOLLOW_DIRECTION_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_CONTINUE_SEARCHING },
                new MyMissionLocation(baseSector, (uint)EntityID.FollowDirectionDummy)) { HudName = MyTextsWrapperEnum.HudFollow };
            followDirection.OnMissionLoaded += M03FollowDirectionOnOnMissionLoaded;
            followDirection.OnMissionCleanUp += M03FollowDirectionOnOnMissionCleanUp;
            m_objectives.Add(followDirection);


            m_coughtInTrap = new MyObjectiveDialog
                (MyTextsWrapperEnum.ALIEN_GATE_COUGHT_IN_TRAP_Name,
                 MyMissionID.ALIEN_GATE_COUGHT_IN_TRAP,
                 MyTextsWrapperEnum.ALIEN_GATE_COUGHT_IN_TRAP_Description,
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_FOLLOW_DIRECTION },
                 MyDialogueEnum.ALIEN_GATE_0500
                 ) { Location = new MyMissionLocation(baseSector, (uint)EntityID.CoughtInTrapDummy), SaveOnSuccess = true };
            m_coughtInTrap.OnMissionLoaded += M04CoughtInTrapOnOnMissionLoaded;
            m_coughtInTrap.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip1, (uint)EntityID.CoughtInTrapShip1Target, CoughtInTrapMoveTime));
            m_coughtInTrap.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip2, (uint)EntityID.CoughtInTrapShip2Target, CoughtInTrapMoveTime));
            m_coughtInTrap.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip3, (uint)EntityID.CoughtInTrapShip3Target, CoughtInTrapMoveTime));
            m_objectives.Add(m_coughtInTrap);
            



            m_runForYourLife = new MyTimedObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_RUN_FOR_LIFE_Name),
                 MyMissionID.ALIEN_GATE_RUN_FOR_LIFE,
                 (MyTextsWrapperEnum.ALIEN_GATE_RUN_FOR_LIFE_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_COUGHT_IN_TRAP },
                 TimeSpan.FromMilliseconds(RunForYourLifeObjectiveTime),
                 startDialogId: MyDialogueEnum.ALIEN_GATE_0600
                ) { DisplayCounter = false, Location = new MyMissionLocation(baseSector, (uint)EntityID.RunForYourLifeDummy), HudName = MyTextsWrapperEnum.HudRun };
            m_runForYourLife.OnMissionLoaded += M05RunForYourLifeOnOnMissionLoaded;
            m_runForYourLife.Components.Add(new MyMovingEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN), (uint)EntityID.RunForYourLifeShipTarget, RunForYourLifeShipMoveTime));
            m_objectives.Add(m_runForYourLife);
            
            m_regroupWithMadelyn = new MyTimedObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_REGROUP_WITH_MADELYN_Name),
                MyMissionID.ALIEN_GATE_REGROUP_WITH_MADELYN,
                (MyTextsWrapperEnum.ALIEN_GATE_REGROUP_WITH_MADELYN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_RUN_FOR_LIFE, },
                TimeSpan.FromMilliseconds(RegroupWithMadelynMissionTime)
            ) { DisplayCounter = false, Location = new MyMissionLocation(baseSector, (uint)EntityID.RegroupWithMadelynShip1Target), HudName = MyTextsWrapperEnum.HudRegroup, SaveOnSuccess = true };
            m_regroupWithMadelyn.OnMissionLoaded += M06RegroupWithMadelynOnOnMissionLoaded;
            m_regroupWithMadelyn.Components.Add(new MySpawnpointWaves((uint)EntityID.RegroupWithMadelynDetector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.RegroupWithMadelynSpawn1},
                                                                                                             new uint[]{(uint)EntityID.RegroupWithMadelynSpawn2}
                                                                                                         }));
            m_regroupWithMadelyn.Components.Add(new MySpawnpointWaves((uint)EntityID.RegroupWithMadelynDetector2, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.RegroupWithMadelynSpawn3, (uint)EntityID.RegroupWithMadelynSpawn4}
                                                                                                         }));

            m_regroupWithMadelyn.Components.Add(new MyMovingEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN), (uint)EntityID.RegroupWithMadelynShip1Target, RegroupWithMadelynMissionTime));
            m_regroupWithMadelyn.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip1, (uint)EntityID.RegroupWithMadelynShip2Target, RegroupWithMadelynMissionTime));
            m_regroupWithMadelyn.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip2, (uint)EntityID.RegroupWithMadelynShip3Target, RegroupWithMadelynMissionTime));
            m_regroupWithMadelyn.Components.Add(new MyMovingEntity((uint)EntityID.CoughtInTrapShip3, (uint)EntityID.RegroupWithMadelynShip4Target, RegroupWithMadelynMissionTime));
            m_regroupWithMadelyn.Components.Add(new MyTimedDialogue(new TimeSpan(0, 0, 50), MyDialogueEnum.ALIEN_GATE_0900));
            m_regroupWithMadelyn.Components.Add(new MyDetectorDialogue((uint)EntityID.RegroupWithMadelynShip1Target, MyDialogueEnum.ALIEN_GATE_0800B));
            m_objectives.Add(m_regroupWithMadelyn);
            


            m_boardMotherShip = new MyObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_BOARD_MOTHER_SHIP_Name),
                 MyMissionID.ALIEN_GATE_BOARD_MOTHER_SHIP,
                 (MyTextsWrapperEnum.ALIEN_GATE_BOARD_MOTHER_SHIP_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_REGROUP_WITH_MADELYN, },
                 new MyMissionLocation(baseSector, (uint)EntityID.BoardmotherShipDummy)
                ) { SuccessDialogId = MyDialogueEnum.ALIEN_GATE_1100, HudName = MyTextsWrapperEnum.HudMothership };
            m_boardMotherShip.Components.Add(new MySpawnpointWaves((uint)EntityID.BoardmotherShipDetector1, 1, (uint)EntityID.BoardmotherShipSpawn1));
            m_boardMotherShip.Components.Add(new MySpawnpointWaves((uint)EntityID.BoardmotherShipDetector2, 1, (uint)EntityID.BoardmotherShipSpawn2));
            m_boardMotherShip.Components.Add(new MySpawnpointWaves((uint)EntityID.BoardmotherShipDetector3, 1, (uint)EntityID.BoardmotherShipSpawn3));
            m_boardMotherShip.Components.Add(new MyDetectorDialogue((uint)EntityID.RussianGeneralSpeak, MyDialogueEnum.ALIEN_GATE_1000));
            m_boardMotherShip.OnMissionLoaded += BoardMotherShipOnOnMissionLoaded;
            m_objectives.Add(m_boardMotherShip);

            m_hackGenerator = new MyUseObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_HACK_GENERATOR_Name),
                 MyMissionID.ALIEN_GATE_HACK_GENERATOR,
                 (MyTextsWrapperEnum.ALIEN_GATE_HACK_GENERATOR_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_BOARD_MOTHER_SHIP },
                 new MyMissionLocation(baseSector, (uint)EntityID.HackGeneratorDummy),
                 MyTextsWrapperEnum.PressToHack,
                 MyTextsWrapperEnum.Generator,
                 MyTextsWrapperEnum.StartingProgress,
                 5000
                );
            m_hackGenerator.SaveOnSuccess = true;
            m_hackGenerator.OnMissionLoaded += M08HackGeneratorOnOnMissionLoaded;
            m_hackGenerator.Components.Add(new MyTimedDialogue(new TimeSpan(0, 0, 2), MyDialogueEnum.ALIEN_GATE_1200));
            m_hackGenerator.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn11},
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn12}
                                                                                                         }));
            m_hackGenerator.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector2, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn21},
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn22}
                                                                                                         }));
            m_hackGenerator.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn31,(uint)EntityID.HackGeneratorSpawn32,(uint)EntityID.HackGeneratorSpawn33},
                                                                                                         }));
            m_hackGenerator.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector4, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn41,(uint)EntityID.HackGeneratorSpawn42},
                                                                                                         }));
            m_hackGenerator.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector5, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn5},
                                                                                                         }));
            m_objectives.Add(m_hackGenerator);


            m_hackEngine = new MyUseObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_HACK_ENGINE_Name),
                 MyMissionID.ALIEN_GATE_HACK_ENGINE,
                 (MyTextsWrapperEnum.ALIEN_GATE_HACK_ENGINE_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_HACK_GENERATOR },
                 new MyMissionLocation(baseSector, (uint)EntityID.HackEngineDummy),
                 MyTextsWrapperEnum.PressToHack,
                 MyTextsWrapperEnum.Generator,
                 MyTextsWrapperEnum.StartingProgress,
                 5000,
                 MyUseObjectiveType.Hacking,
                 startDialogId: MyDialogueEnum.ALIEN_GATE_1300
                ) { SuccessDialogId = MyDialogueEnum.ALIEN_GATE_1500, SaveOnSuccess = true };
            m_hackEngine.OnMissionLoaded += M09HackEngineOnOnMissionLoaded;
            m_hackEngine.Components.Add(new MyDetectorDialogue((uint)EntityID.HackEngineDummy, MyDialogueEnum.ALIEN_GATE_1400));
            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector5, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGeneratorSpawn5},
                                                                                                        }));

            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector4, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn2},
                                                                                                         }));
            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackEngineDetector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn3},
                                                                                                         }));
            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackEngineDetector4, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn4},
                                                                                                         }));
            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackEngineDetector5, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn51},
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn52},
                                                                                                         }));
            m_hackEngine.Components.Add(new MySpawnpointWaves((uint)EntityID.HackEngineDetector6, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn6},
                                                                                                         }));
            m_objectives.Add(m_hackEngine);


            m_leaveShip1 = new MyTimedReachLocationObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_LEAVE_SHIP_Name),
                 MyMissionID.ALIEN_GATE_LEAVE_SHIP,
                 (MyTextsWrapperEnum.ALIEN_GATE_LEAVE_SHIP_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_HACK_ENGINE, },
                 TimeSpan.FromMilliseconds((double)m_10ObjectiveTime),
                 new MyMissionLocation(baseSector, (uint)EntityID._10Dummy)
                ) { HudName = MyTextsWrapperEnum.Nothing };

            m_leaveShip1.OnMissionLoaded += M10LeaveShip1OnOnMissionLoaded;
            m_leaveShip1.OnMissionFailed += M10LeaveShip1OnOnMissionFailed;
            m_leaveShip1.Components.Add(new MySpawnpointWaves((uint)EntityID.HackEngineDetector4, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackEngineSpawn4},
                                                                                                        }));

            m_leaveShip1.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGeneratorDetector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._10Spawn2},
                                                                                                         }));
            m_objectives.Add(m_leaveShip1);


            m_regroupWithMadelyn2 = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_REGROPUP_WITH_MADELYN_Name),
                MyMissionID.ALIEN_GATE_REGROPUP_WITH_MADELYN,
                (MyTextsWrapperEnum.ALIEN_GATE_REGROPUP_WITH_MADELYN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_LEAVE_SHIP, },
                new MyMissionLocation(baseSector, (uint)EntityID.RegroupWithMadelynShip1Target),
                startDialogId: MyDialogueEnum.ALIEN_GATE_1700
            ) { HudName = MyTextsWrapperEnum.HudRegroup, SaveOnSuccess = true };
            m_regroupWithMadelyn2.OnMissionLoaded += M11RegroupWithMadelyn2OnOnMissionLoaded;
            m_objectives.Add(m_regroupWithMadelyn2);

            m_shipbBoard2 = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_BOARD_SECOND_Name),
                MyMissionID.ALIEN_GATE_BOARD_SECOND,
                (MyTextsWrapperEnum.ALIEN_GATE_BOARD_SECOND_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_REGROPUP_WITH_MADELYN, },
                new MyMissionLocation(baseSector, (uint)EntityID.BoardSecondShipbDummy)
                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_1900, HudName = MyTextsWrapperEnum.HudMothership };
            m_shipbBoard2.Components.Add(new MySpawnpointWaves((uint)EntityID.RegroupWithMadelynDetector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.BoardSecondShipbSpawn1, (uint)EntityID.BoardSecondShipbSpawn2},
                                                                                                        }));
            m_shipbBoard2.OnMissionLoaded += M11BBoard2OnOnMissionLoaded;
            m_objectives.Add(m_shipbBoard2);


            m_hackGenerator2 = new MyUseObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_HACK_GENERATOR2_Name),
                 MyMissionID.ALIEN_GATE_HACK_GENERATOR2,
                 (MyTextsWrapperEnum.ALIEN_GATE_HACK_GENERATOR2_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_BOARD_SECOND },
                 new MyMissionLocation(baseSector, (uint)EntityID.HackGenerator2Dummy),
                 MyTextsWrapperEnum.PressToHack,
                 MyTextsWrapperEnum.Generator,
                 MyTextsWrapperEnum.StartingProgress,
                 5000,
                 MyUseObjectiveType.Hacking
                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2000, SaveOnSuccess = true };
            m_hackGenerator2.OnMissionLoaded += M12HackGenerator2OnOnMissionLoaded;

            m_hackGenerator2.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGenerator2Detector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGenerator2Spawn11, (uint)EntityID.HackGenerator2Spawn12},
                                                                                                        }));
            m_hackGenerator2.Components.Add(new MySpawnpointWaves((uint)EntityID.HackGenerator2Detector2, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.HackGenerator2Spawn2},
                                                                                                        }));
            m_objectives.Add(m_hackGenerator2);


            m_hackEngine2 = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_HACK_ENGINE_2_Name),
                MyMissionID.ALIEN_GATE_HACK_ENGINE_2,
                (MyTextsWrapperEnum.ALIEN_GATE_HACK_ENGINE_2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_HACK_GENERATOR2 },
                new MyMissionLocation(baseSector, (uint)EntityID._13Dummy)/*,
                startDialogId: MyDialogueEnum.ALIEN_GATE_2100*/
                );
            m_hackEngine2.OnMissionLoaded += M13HackEngine2OnOnMissionLoaded;
            m_hackEngine2.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._13Spawn11, (uint)EntityID._13Spawn12,(uint)EntityID._13Spawn13},
                                                                                                        }));
            m_hackEngine2.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector2, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._13Spawn2}
                                                                                                        }));
            m_hackEngine2.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector3, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._13Spawn3}
                                                                                                        }));
            m_hackEngine2.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector4, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._13Spawn41},
                                                                                                            new uint[]{(uint)EntityID._13Spawn42}
                                                                                                        }));
            m_hackEngine2.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector5, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._13Spawn51},
                                                                                                            new uint[]{(uint)EntityID._13Spawn52}
                                                                                                        }));
            m_objectives.Add(m_hackEngine2);


            m_enableDoors = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.ALIEN_GATE_ENABLE_DOORS_Name),
                MyMissionID.ALIEN_GATE_ENABLE_DOORS,
                (MyTextsWrapperEnum.ALIEN_GATE_ENABLE_DOORS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_HACK_ENGINE_2 },
                null,
                new List<uint> { (uint)EntityID.EnableDoorsHUB },
                new List<uint> { (uint)EntityID.EnableDoorsDoors, }


                ) { HudName = MyTextsWrapperEnum.HudHub };
            m_enableDoors.Components.Add(new MySpawnpointWaves((uint)EntityID.EnableDoorsDetector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.EnableDoorsSpawnPoint}
                                                                                                        }));

            m_enableDoors.Components.Add(new MyDetectorDialogue((uint)EntityID.DoorsDialogue, MyDialogueEnum.ALIEN_GATE_2200));
            m_objectives.Add(m_enableDoors);

            m_enterlaboratory = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_ENTER_LAB_Name),
                MyMissionID.ALIEN_GATE_ENTER_LAB,
                (MyTextsWrapperEnum.ALIEN_GATE_ENTER_LAB_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_ENABLE_DOORS },
                new MyMissionLocation(baseSector, (uint)EntityID.EnterLaboratoryDummy)
                // startDialogId: m_dialog12
                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2300, HudName = MyTextsWrapperEnum.HudLab };
            m_enterlaboratory.Components.Add(new MySpawnpointWaves((uint)EntityID.EnterLaboratoryDetector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID.EnterLaboratorySpawn}
                                                                                                        }));
            m_objectives.Add(m_enterlaboratory);

            m_downloadData = new MyUseObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_16_Name),
                 MyMissionID.ALIEN_GATE_16,
                 (MyTextsWrapperEnum.ALIEN_GATE_16_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_ENTER_LAB },
                 new MyMissionLocation(baseSector, (uint)EntityID._16Dummy),
                 MyTextsWrapperEnum.PressToDownloadData,
                 MyTextsWrapperEnum.DataTransfer,
                 MyTextsWrapperEnum.DownloadingData,
                 3000,
                 MyUseObjectiveType.Taking
                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2400, SaveOnSuccess = true };
            m_objectives.Add(m_downloadData);


            m_hackGenerator3 = new MyUseObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_17_Name),
                 MyMissionID.ALIEN_GATE_17,
                 (MyTextsWrapperEnum.ALIEN_GATE_17_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_16 },
                 new MyMissionLocation(baseSector, (uint)EntityID._17Dummy),
                 MyTextsWrapperEnum.PressToHack,
                 MyTextsWrapperEnum.Generator,
                 MyTextsWrapperEnum.StartingProgress,
                 5000,
                 MyUseObjectiveType.Hacking,
                 startDialogId: MyDialogueEnum.ALIEN_GATE_2500
                );
            m_objectives.Add(m_hackGenerator3);
            m_hackGenerator3.OnMissionLoaded += M17HackGenerator3OnOnMissionLoaded;

            m_hackGenerator3.Components.Add(new MySpawnpointWaves((uint)EntityID._17Detector, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._17Spawn1,(uint)EntityID.EnterLaboratorySpawn}
                                                                                                        }));

            m_leaveShip = new MyTimedReachLocationObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_18_Name),
                 MyMissionID.ALIEN_GATE_18,
                 (MyTextsWrapperEnum.ALIEN_GATE_18_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_17, },
                 TimeSpan.FromMilliseconds((double)m_18ObjectiveTime),
                 new MyMissionLocation(baseSector, (uint)EntityID._18Dummy)

                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2600, HudName = MyTextsWrapperEnum.Nothing, SaveOnSuccess = true };
            m_leaveShip.OnMissionLoaded += M18LeaveShipOnOnMissionLoaded;
            m_leaveShip.OnMissionFailed += M18LeaveShipOnOnMissionFailed;
            m_leaveShip.Components.Add(new MySpawnpointWaves((uint)EntityID._13Detector5, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._18Spawn1}
                                                                                                        }));
            m_objectives.Add(m_leaveShip);

            m_killReef = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_19_Name),
                MyMissionID.ALIEN_GATE_19,
                (MyTextsWrapperEnum.ALIEN_GATE_19_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_18 },
                new MyMissionLocation(baseSector, (uint)EntityID._19Dummy)

                ) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2800 };
            m_killReef.OnMissionLoaded += M19KilReefOnOnMissionLoaded;
            m_objectives.Add(m_killReef);


            m_20ReefEscape = new MyTimedObjective
                ((MyTextsWrapperEnum.ALIEN_GATE_20_Name),
                 MyMissionID.ALIEN_GATE_20,
                 (MyTextsWrapperEnum.ALIEN_GATE_20_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.ALIEN_GATE_19, },
                 TimeSpan.FromMilliseconds(m_20ObjectiveTime)
                ) { HudName = MyTextsWrapperEnum.HudFrancisReef };
            m_20ReefEscape.Location = new MyMissionLocation(baseSector, (uint)EntityID._20Prefab);
            m_20ReefEscape.Components.Add(new MyMovingEntity((uint)EntityID._19Ship, (uint)EntityID._20ShipTarget, m_20ObjectiveTime));
            m_20ReefEscape.OnMissionLoaded += M20ReefEscapeOnOnMissionLoaded;
            m_20ReefEscape.OnMissionSuccess += ReefEscapeOnOnMissionSuccess;
            m_objectives.Add(m_20ReefEscape);

            m_21DestroyReef = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.ALIEN_GATE_21_Name),
                MyMissionID.ALIEN_GATE_21,
                (MyTextsWrapperEnum.ALIEN_GATE_21_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_20 },
                new List<uint>() { (uint)EntityID._21ToDesroy }) { StartDialogId = MyDialogueEnum.ALIEN_GATE_2900, HudName = MyTextsWrapperEnum.HudFrancisReef, SaveOnSuccess = true };
            m_21DestroyReef.OnMissionLoaded += M21DestroyReefOnOnMissionLoaded;
            m_21DestroyReef.OnMissionSuccess += M21DestroyReefOnOnMissionSuccess;
            m_21DestroyReef.Components.Add(new MySpawnpointWaves((uint)EntityID._21Detector1, 1, new List<uint[]>()
                                                                                                         {
                                                                                                             new uint[]{(uint)EntityID._21SpawnPoint11},
                                                                                                             new uint[]{(uint)EntityID._21SpawnPoint12}
                                                                                                        }));
            m_21DestroyReef.Components.Add(new MyMovingEntity((uint)EntityID._21Ship2, (uint)EntityID._21Ship2Target, m_21ObjectiveMovingTime));
            m_21DestroyReef.Components.Add(new MyMovingEntity((uint)EntityID._21Particle1, (uint)EntityID._21Particle1Target, m_21ObjectiveMovingTime, false));
            m_21DestroyReef.Components.Add(new MyMovingEntity((uint)EntityID._21Particle2, (uint)EntityID._21Particle2Target, m_21ObjectiveMovingTime, false));
            //m_21DestroyReef.Components.Add(new MyTimedDialogue(TimeSpan.FromMilliseconds(m_21ObjectiveMovingTime), MyDialogueEnum.ALIEN_GATE_3100));
            m_21DestroyReef.Components.Add(new MyTimedDialogue(TimeSpan.FromMilliseconds(5000), MyDialogueEnum.ALIEN_GATE_3000));
            m_objectives.Add(m_21DestroyReef);


            m_22FlyBackTomadelyn = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_22_Name),
                MyMissionID.ALIEN_GATE_22,
                (MyTextsWrapperEnum.ALIEN_GATE_22_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_21 },
                new MyMissionLocation(baseSector, (uint)EntityID._22Dummy),
                startDialogId: MyDialogueEnum.ALIEN_GATE_3200
                ) { HudName = MyTextsWrapperEnum.HudMeetingPoint };
            m_22FlyBackTomadelyn.OnMissionLoaded += M22FlyBackTomadelynOnOnMissionLoaded;
            m_objectives.Add(m_22FlyBackTomadelyn);

            m_23ExploreAliengate = new MyObjective(
                (MyTextsWrapperEnum.ALIEN_GATE_23_Name),
                MyMissionID.ALIEN_GATE_23,
                (MyTextsWrapperEnum.ALIEN_GATE_23_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.ALIEN_GATE_22 },
                new MyMissionLocation(baseSector, (uint)EntityID._23Dummy),
                startDialogId: MyDialogueEnum.ALIEN_GATE_3300
                ) { HudName = MyTextsWrapperEnum.Nothing };
            m_23ExploreAliengate.Components.Add(new MyMovingEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN), (uint)EntityID._23Shiptarget, 50000));
            m_23ExploreAliengate.OnMissionLoaded += M23ExploreAliengateOnOnMissionLoaded;
            m_objectives.Add(m_23ExploreAliengate);
            #endregion
        }

        private void BoardMotherShipOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.TryUnhide((uint)EntityID.Ship1Interier, true);
            MyScriptWrapper.TryUnhide((uint)EntityID.MotherShip2Interior, true);
        }

        private void ReefEscapeOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.TryUnhide((uint)EntityID._21Prefab1UnHide, true);
            MyScriptWrapper.TryUnhide((uint)EntityID._21Ship2, true);
        }

        private void M23ExploreAliengateOnOnMissionLoaded(MyMissionBase sender)
        {
            //MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTexts.CampaignIsCompleted, MyGuiManager.GetFontMinerWarsGreen(), 100000));
            MyGuiScreenGamePlay.Static.DrawCampaignEnd = true;
            sender.MissionTimer.RegisterTimerAction(15000, HideDrawCampaignEnd, false);
        }

        void HideDrawCampaignEnd()
        {
            MyGuiScreenGamePlay.Static.DrawCampaignEnd = false;
        }

        private void M22FlyBackTomadelynOnOnMissionLoaded(MyMissionBase sender)
        {
            var madelynLocation = MyScriptWrapper.GetEntity((uint) EntityID._22Ship1);
            MyScriptWrapper.Move(m_madelyn, madelynLocation.GetPosition(), madelynLocation.GetForward(), madelynLocation.GetUp());
            MyScriptWrapper.TryUnhide((uint)EntityID._22Ship2, true);
        }

        private void M21DestroyReefOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_21EnableparticlesDestroy, true);
            MyScriptWrapper.SetEntitiesEnabled(m_21DisableParticlesDestroy, false);
            MyScriptWrapper.TryHide((uint)EntityID._21Ship2, true);
            MyScriptWrapper.TryUnhide((uint)EntityID._21ShipUnhide, true);
        }

        private void M21DestroyReefOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_21Enableparticles, true);
            MyScriptWrapper.TryHide((uint)EntityID._19Ship, true);
            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_21Enableparticles[0]), MySoundCuesEnum.SfxShipLargeExplosion);

            MyScriptWrapper.TryUnhide((uint)EntityID.Voxel2, true);
            MyScriptWrapper.TryHide((uint)EntityID._21VoxelAsteroidHide, true);
        }

        private void M20ReefEscapeOnOnMissionLoaded(MyMissionBase sender)
        {
            //MyScriptWrapper.TryHide((uint)EntityID.RegroupWithMadelynShip2);
        }

        private void M19KilReefOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_19DisableParticles, false);
            MyScriptWrapper.ActivateSpawnPoints(new List<uint>() { (uint)EntityID._19Spawn1, (uint)EntityID._19Spawn2 });
            MyScriptWrapper.TryHide((uint)EntityID.CoughtInTrapShip3, true);
            MyScriptWrapper.TryUnhide((uint)EntityID._19Ship, true);
            sender.MissionTimer.RegisterTimerAction(5000, KillReefParticles);
        }

        private void KillReefParticles()
        {
            MyScriptWrapper.SetEntitiesEnabled(m_19EnableParticles, true);
        }

        private void M18LeaveShipOnOnMissionFailed()
        {
            MyScriptWrapper.DestroyPlayerShip();
        }

        private void M18LeaveShipOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled((uint)EntityID._18DisableDummy, false);
            MyScriptWrapper.SetEntitiesEnabled(m_18EnableParticles, true);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID._18DisableTurret, false);
            MissionTimer.RegisterTimerAction(m_18ObjectiveTime, ExplosionMission18, false);

            for (int i = 0; i < m_18ObjectiveTime / 5000; i++)
            {
                Components.Add(new MyHeadshake(MissionTimer.ElapsedTime + i * 5000, MyHeadshake.DefaultShaking, 12, 5, 10));
            }

        }

        private void ExplosionMission18()
        {
            Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 17, 10, 10));

            var time = 0;


            MissionTimer.RegisterTimerAction(time += 750, FirstExplosion, false);


            MissionTimer.RegisterTimerAction(time += 500, Explosion18Final, false);
        }

        private void FirstExplosion()
        {
            foreach (uint explosionDummy in m_10explosionDummies)
            {
                var entity = MyScriptWrapper.GetEntity(explosionDummy);
                MyScriptWrapper.AddExplosion(entity, Explosions.MyExplosionTypeEnum.MEDIUM_PREFAB_EXPLOSION, 20000);
            }
        }

        private void Explosion18Final()
        {
            MyScriptWrapper.SetEntitiesEnabled(m_18DisableParticlesBoom, false);
            MyScriptWrapper.SetEntitiesEnabled(m_18EnableParticlesBoom, true);

            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.CoughtInTrapShip2), MySoundCuesEnum.SfxShipLargeExplosion);


            MissionTimer.RegisterTimerAction(250, () => MyScriptWrapper.TryHide((uint)EntityID.MotherShip2Interior), false);
            MissionTimer.RegisterTimerAction(250, () => MyScriptWrapper.TryHide((uint)EntityID.CoughtInTrapShip2, true), false);
            MissionTimer.RegisterTimerAction(250, () => MyScriptWrapper.TryUnhide((uint)EntityID.MotherShip2Destroyed, false));


            MyScriptWrapper.TryHideEntities(m_hackGenerator2Cargos, true);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.ALIEN_GATE_2700);
        }



        private void Explosion10Final()
        {
            MyScriptWrapper.SetEntitiesEnabled(m_10particlesDisable, false);
            MyScriptWrapper.SetEntitiesEnabled(m_10particlesEnable, true);
            MyScriptWrapper.TryHideEntities(m_hackGeneratorCargos, true);

            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.CoughtInTrapShip1), MySoundCuesEnum.SfxShipLargeExplosion);

            MissionTimer.RegisterTimerAction(500, () => MyScriptWrapper.TryHide((uint)EntityID.CoughtInTrapShip1), false);
            MissionTimer.RegisterTimerAction(500, () => MyScriptWrapper.TryHide((uint)EntityID.Ship1Interier, true), false);
            MissionTimer.RegisterTimerAction(500, () => MyScriptWrapper.TryUnhide((uint)EntityID._10PrefabCont, false));

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.ALIEN_GATE_1600);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA09", false);
        }

        private void M17HackGenerator3OnOnMissionLoaded(MyMissionBase sender)
        {
            sender.Components.Add(new MyHeadshake(m_leaveShip1.MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));
        }

        private void M13HackEngine2OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_13Particles, true);
        }

        private void M12HackGenerator2OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.MarkEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.HackGenerator2Doors), "Destroy", MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, MyGuitargetMode.Enemy);
            MyScriptWrapper.TryUnhideEntities(m_hackGenerator2Cargos, true);
            sender.Components.Add(new MyHeadshake(0, MyHeadshake.DefaultShaking));

            MyScriptWrapper.SetEntitiesEnabled(m_hackGenerator2Particles, true);
        }





        private void M11BBoard2OnOnMissionLoaded(MyMissionBase sender)
        {
            var boardSecondShipBDetector = MyScriptWrapper.GetDetector((uint)EntityID.BoardSecondShipbDetector);
            boardSecondShipBDetector.On();
            boardSecondShipBDetector.OnEntityEnter += M11BDetectorOnOnEntityEnter;
            MyScriptWrapper.TryUnhide((uint)EntityID.MotherShip2Interior, true);
        }

        private void M11BDetectorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_boardSecondShipbparticlesDisable, false);
            MyScriptWrapper.SetEntitiesEnabled(m_boardSecondShipbparticlesEnable, true);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.BoardSecondShipbSpawn0);
        }

        private void M11RegroupWithMadelyn2OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.BoardSecondShipSpawn1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.BoardSecondShipSpawn2);
            sender.MissionTimer.RegisterTimerAction(5000, M11Action);
        }

        private void M11Action()
        {
            MyScriptWrapper.SetEntitiesEnabled(m_boardSecondShipparticles, true);
        }

        private void M10LeaveShip1OnOnMissionFailed()
        {
            MyScriptWrapper.DestroyPlayerShip();
        }

        private void M10LeaveShip1OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetWaypointListSecrecy(new List<uint>() { (uint)EntityID._10WayPoint }, true);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID._10DisableDummy, false);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID._10DisableTurret, false);
            MissionTimer.RegisterTimerAction(m_10ObjectiveTime, ExplosionMission10, false);


            for (int i = 0; i < m_10ObjectiveTime / 5000; i++)
            {
                Components.Add(new MyHeadshake(MissionTimer.ElapsedTime + i * 5000, MyHeadshake.DefaultShaking, 12, 5, 10));
            }
        }

        private void ExplosionMission10()
        {
            Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 17, 10, 10));

            var time = 0;
            foreach (uint explosionDummy in m_18explosionDummies)
            {
                var entity = MyScriptWrapper.GetEntity(explosionDummy);
                MissionTimer.RegisterTimerAction(time += 750, () => MyScriptWrapper.AddExplosion(entity, Explosions.MyExplosionTypeEnum.MEDIUM_PREFAB_EXPLOSION, 25000), false);
            }

            MissionTimer.RegisterTimerAction(time += 500, Explosion10Final, false);
        }


        private void M09HackEngineOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntitiesEnabled(m_hackEngineparticles, true);
        }

        private void M08HackGeneratorOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.HackGeneratordestroy), MyTexts.Destroy, MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_DISTANCE, MyGuitargetMode.Enemy);
            MyScriptWrapper.TryUnhideEntities(m_hackGeneratorCargos, true);

            MyScriptWrapper.SetEntitiesEnabled(m_hackGeneratorparticles, true);
        }

        private void M06RegroupWithMadelynOnOnMissionLoaded(MyMissionBase sender)
        {
            var madelynLocation = MyScriptWrapper.GetEntity((uint) EntityID.RegroupWithMadelynShip2);
            MyScriptWrapper.Move(m_madelyn, madelynLocation.GetPosition(), madelynLocation.GetForward(), madelynLocation.GetUp());
        }



        private void M05RunForYourLifeOnOnMissionLoaded(MyMissionBase sender)
        {
            sender.MissionTimer.RegisterTimerAction(RunForYourLifeShipMoveTime, M05StartDialogue);
            sender.MissionTimer.RegisterTimerAction(RunForYourLifeParticleStartTime, M05Startparticle);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
        }




        private void M05Startparticle()
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.RunForYourLifeParticleEffect), true);

            MyScriptWrapper.TryHideEntities(new List<uint>() { (uint)EntityID.RunForYourLifeVoxel }, true);
            MyScriptWrapper.TryUnhideEntities(new List<uint>() { (uint)EntityID.Voxel1, (uint)EntityID.Voxel3 }, true);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.ALIEN_GATE_0700);
            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.RunForYourLifeParticleEffect), MySoundCuesEnum.SfxShipLargeExplosion);
        }



        private void M05StartDialogue()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.ALIEN_GATE_0800);
        }



        private void M04CoughtInTrapOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.CoughtInTrapShip1), 0, true);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.CoughtInTrapShip2), 0, true);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.CoughtInTrapShip3), 0, true);
            MyScriptWrapper.ActivateSpawnPoints(new List<uint>() { (uint)EntityID.CoughtInTrapSpawn1, (uint)EntityID.CoughtInTrapSpawn2 });
            MyScriptWrapper.TryUnhideEntities(new List<uint>() { (uint)EntityID.CoughtInTrapShip3, (uint)EntityID.CoughtInTrapShip1, (uint)EntityID.CoughtInTrapShip2 }, true);

        }

        private MyEntityDetector m_followDirectionDetector;
        private MyObjectiveDialog m_coughtInTrap;

        private MyObjective m_runForYourLife;
        private MyObjective m_regroupWithMadelyn;
        private MyObjective m_boardMotherShip;
        private MyUseObjective m_hackGenerator;
        private MyUseObjective m_hackEngine;
        private MyTimedReachLocationObjective m_leaveShip1;
        private MyObjective m_regroupWithMadelyn2;
        private MyObjective m_shipbBoard2;
        private MyUseObjective m_hackGenerator2;
        private MyObjective m_hackEngine2;
        private MyObjectiveEnablePrefabs m_enableDoors;
        private MyObjective m_enterlaboratory;
        private MyUseObjective m_downloadData;
        private MyUseObjective m_hackGenerator3;
        private MyTimedReachLocationObjective m_leaveShip;
        private MyObjective m_killReef;
        private MyTimedObjective m_20ReefEscape;
        private MyObjectiveDestroy m_21DestroyReef;
        private MyObjective m_22FlyBackTomadelyn;
        private MyObjective m_23ExploreAliengate;


        private void M03FollowDirectionOnOnMissionCleanUp(MyMissionBase sender)
        {
            m_followDirectionDetector.Off();
            m_followDirectionDetector.OnEntityEnter -= M03DetectorOnOnEntityEnter;
            //MyScriptWrapper.TryHide(m_madelyn.EntityId.Value.NumericValue);
        }


        private void M03DetectorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                var madelynLocation = MyScriptWrapper.GetEntity((uint)EntityID.RunForYourLifeMovingMadelyn);
                MyScriptWrapper.Move(m_madelyn, madelynLocation.GetPosition(), madelynLocation.GetForward(), madelynLocation.GetUp());
                sender.Off();
            }
        }

        private void M03FollowDirectionOnOnMissionLoaded(MyMissionBase sender)
        {
            m_followDirectionDetector = MyScriptWrapper.GetDetector((uint)EntityID.FollowDirectionDetector);
            m_followDirectionDetector.On();
            m_followDirectionDetector.OnEntityEnter += M03DetectorOnOnEntityEnter;
        }


        private void M01FollowCordsOnOnMissionSuccess(MyMissionBase sender)
        {
            m_followCoordinatesDetector.Off();
            m_followCoordinatesDetector.OnEntityEnter -= M01DetectorOnOnEntityEnter;
        }

        private void M01DetectorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            //if(entity==MySession.PlayerShip)
            //  MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FollowCoordinatesSpawnPoint);
        }

        private void M01FollowCordsOnOnMissionLoaded(MyMissionBase sender)
        {
            m_followCoordinatesDetector.On();
            m_followCoordinatesDetector.OnEntityEnter += M01DetectorOnOnEntityEnter;

        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            m_followCoordinatesDetector = MyScriptWrapper.GetDetector((uint)EntityID.FollowCoordinatesDummy);
            m_madelyn = MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN));  

            base.Load();

            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnSentenceStarted += MyScriptWrapper_OnSentenceStarted;

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.ALIEN_GATE_COUGHT_IN_TRAP))
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);
            }
            else
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Horror, 100, "KA02");
            }

            MyScriptWrapper.TryHide((uint)EntityID.Voxel1);
            MyScriptWrapper.TryHide((uint)EntityID.Voxel2);
            MyScriptWrapper.TryHide((uint)EntityID.Voxel3);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyFactions.RELATION_NEUTRAL);

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.ALIEN_GATE_FOLLOW_DIRECTION)) 
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
        }

        void MyScriptWrapper_OnSentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (dialogue == MyDialogueEnum.ALIEN_GATE_0500 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_0502)
            {
                //MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Horror, 100, "KA02");
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA15");
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_0600 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_0600)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA15");
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_0800 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_0802)
            {
                MyScriptWrapper.StopTransition(100);
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_1500 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_1500)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 100, "KA02");
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_2600 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_2600)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA19");
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_2900 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_2901)
            {
                MyScriptWrapper.StopTransition(0); //ensure that nothing will play after KA10
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA10", false);
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_3200 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_3202)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 100, "KA05");
            }
            if (dialogue == MyDialogueEnum.ALIEN_GATE_3300 && sentence == MyDialoguesWrapperEnum.Dlg_AlienGate_3300)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA11");
            }
        }

        public override void Unload()
        {
            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnSentenceStarted -= MyScriptWrapper_OnSentenceStarted;

            m_madelyn = null;
            m_followCoordinatesDetector = null;
            HideDrawCampaignEnd();

            base.Unload();
        }

        private void MyScriptWrapperOnOnSpawnpointBotSpawned(MyEntity spawnPoint, MyEntity entity2)
        {
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.BoardSecondShipSpawn1
                || spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.BoardSecondShipSpawn2
                || spawnPoint.EntityId.Value.NumericValue == (uint)EntityID._19Spawn1
                || spawnPoint.EntityId.Value.NumericValue == (uint)EntityID._19Spawn2)
            {
                entity2.Health = 0.75f * entity2.MaxHealth;
            }

            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.FollowCoordinatesSpawnPoint)
            {
                if (m_followCords.IsAvailable())
                {
                    m_followCords.Components.Add(new MyTimedDialogue(m_followCords.MissionTimer.GetElapsedTime() + new TimeSpan(0, 0, 3), MyDialogueEnum.ALIEN_GATE_0200));
                }
            }
        }
    }
}

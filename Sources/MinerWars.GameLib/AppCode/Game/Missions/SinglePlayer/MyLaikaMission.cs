#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Managers.EntityManager;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.Resources;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyLaikaMission : MyMission
    {
        #region Enums

        private enum EntityID
        {
            StartLocation = 4988,
            Objective01_Hangar = 3891,
            Objective01_Generator = 14177,
            Objective02_CommandHub = 39164,
            Objective03_Transmitter1Hub = 35601,
            Objective04_Transmitter2Hub = 35595,
            Objective05_Warhead = 29774,

            MothershipToHide = 1736,
            MothershipToShow = 1759,
            
            BaseMain = 278,
            BaseStation1 = 1664,
            BaseStation2 = 1590,
            BaseStore = 14152,
            BaseCivilianParts = 7456,

            Spawnpoint_01 = 1969,
            Spawnpoint_03 = 4102,
            Spawnpoint_04 = 4108,
            
            Spawnpoint_HangarAmbush1 = 1970,
            Spawnpoint_HangarAmbush2 = 1971,
            Spawnpoint_Hangar_Back = 1938,
            Spawnpoint_ResearchPatrol = 9638,
            Spawnpoint_CivilianArea = 4777,
            Spawnpoint_ToLeftHub = 29246,
            Spawnpoint_ToRightHub = 29247,
            Spawnpoint_Ambush = 9627,
            SpawnpointHub2 = 29494,
            Spawnpoint_Hounds_Asteroid = 34666,

            Spawnpoint_Hounds_Left_Up = 36881,
            Spawnpoint_Hounds_Left_Down = 36880,

            Spawnpoint_Hounds_Right_Up = 36878,
            Spawnpoint_Hounds_Right_Down = 36879,

            Spawnpoint_Hounds_Base_Up = 34667,
            Spawnpoint_Hounds_Base_Down = 36877,

            Detector_Warhead = 29492,

            Detector_Hangar = 3890,
            Door_ToHangar01 = 996,
            Door_ToHangar02 = 383,

            Detector_Pipe = 9626,
            PipeToDestroy = 419,
            
            Detector_PlayerInPipes = 29248,
            GrilleToDestroy = 4800,

            Detector_RightEntered = 37497,
            Detector_LeftEntered = 37499,

            Waypoint_Right_Wait = 1541,
            Waypoint_Right_Voxel = 1173,
            Waypoint_Right_Lattice1 = 9615,
            Waypoint_Right_Lattice2 = 1469,
            Waypoint_Left_Wait = 37189,
            Waypoint_Left_Lattice1 = 35569,
            Waypoint_Left_Lattice2 = 1429,
            Waypoint_Right_Door = 29495,

            Waypoint_Unsecret_Escape = 28470,

            Dummy_DestroyVoxel01 = 35591,
            Dummy_DestroyVoxel02 = 35592,
            Dummy_DestroyVoxel03 = 35593,

            RightLatticeToDestroy0 = 55544,
            RightLatticeToDestroy1 = 8661,
            RightLatticeToDestroy2 = 1601,
            LeftLatticeToDestroy1 = 9061,
            LeftLatticeToDestroy2 = 1607,

            Detector_Right_Down = 36562,
            Detector_Right_Up = 36873,

            Detector_Left_Down = 36558,
            Detector_Left_Up = 36875,

            Detector_Base_Down = 36563,
            Detector_Base_Up = 36550,

            RussianMothership = 38882,

            DoorLock_Right1 = 1597,
            DoorLock_Right2 = 1608,
            DoorLock_Left1 = 1679,
            DoorLock_Left2 = 1693,

            PipeToDestroy_waytowarhead = 826,

            Detector_StopRussianMothershipMove = 47214,
            Spawn_MothershipHounds = 47218,

            Detector_StopMadelynMove = 47216,

            Particle_Ambush_mist = 57378,
            Particle_Ambush_explosion = 57379,
            Particle_Ambush_smoke = 57380,

            Prefab_Ambush_1 = 58293,
            Prefab_Ambush_2 = 58294,

            Spawnpoint_LastFight_1 = 59208,
            Spawnpoint_LastFight_2 = 59223,
            
            Dummy_Madelyn_Moved = 61142,

            Influence_Sphere_Fallout = 62119,
            Stuff_to_remove_prefab_container1 = 45316,
            Stuff_to_remove_prefab_container4 = 8566,

            Waypoint_ToAmbush = 1240,

            Dummy_Remove_stuff_after_explosion = 16778189,

            CommandCenterDetector = 16779246,
        }

        private List<uint> m_MothershipToHideThrusters = new List<uint>
        {
            25402, 25403, 25404, 25405, 25406, 25407
        };

        private List<uint> m_MothershipToShowThrusters = new List<uint>
        {
            25756, 25757, 26106, 26107, 26108, 26109,
        };

        private List<uint> m_asteroidDetectors = new List<uint>
        {
            36196, 36198, 36200, 36202, 36204, 36206, 36208, 36210, 36212
        };

        private List<uint> m_houndSpawnpoints = new List<uint>
        {
            (uint)EntityID.Spawnpoint_Hounds_Asteroid, (uint)EntityID.Spawnpoint_Hounds_Base_Down, (uint)EntityID.Spawnpoint_Hounds_Base_Up, (uint)EntityID.Spawnpoint_Hounds_Left_Down, (uint)EntityID.Spawnpoint_Hounds_Left_Up, (uint)EntityID.Spawnpoint_Hounds_Right_Down, (uint)EntityID.Spawnpoint_Hounds_Right_Up, (uint)EntityID.Spawn_MothershipHounds
        };

        private List<uint> m_smallShips = new List<uint>
        {
            10, 23
        };

        private List<uint> m_asteroidSecretWaypoints = new List<uint>
        {
            28470, 28153, 28098, 36246, 28171, 28167, 28165, 28163, 28199, 36232, 36230, 31391, 28473, 28130
        };

        private List<uint> m_leavePipeWaypoints = new List<uint>
        {
            35567,
        };

        private List<uint> m_stuffWithRussianMothership = new List<uint>
        {
            (uint)EntityID.Detector_StopRussianMothershipMove, (uint)EntityID.Spawn_MothershipHounds
        };

        private List<uint> m_stuffWithMadelyn = new List<uint>
        {
            (uint)EntityID.Detector_StopMadelynMove
        };

        private List<uint> m_staticAsteroidRubble = new List<uint>
        {
            31, 18
        };

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (var value in m_MothershipToHideThrusters)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_MothershipToShowThrusters)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_asteroidDetectors)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_smallShips)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_asteroidSecretWaypoints)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_staticAsteroidRubble)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        #endregion

        private float m_MadelynSpeed;
        private float m_RussianSpeed;

        MyEntity m_hangar;
        bool m_laikaBaseCalled = false;

        #region Submissions
        private MyObjective m_generatorSubmission;
        private MyObjective m_toCommandCentreSubmission;
        private MyObjective m_warheadSubmission;
        private MyObjective m_communication01Submission;
        private MyObjective m_communication02Submission;
        private MyObjective m_escapeSubmission;
        
        public MyLaikaMission()
        {
            ID = MyMissionID.LAIKA; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.LAIKA;
            Description = MyTextsWrapperEnum.LAIKA_Description;
            DebugName = new StringBuilder("02-Laika");
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-3851812, 0, -2054500);

            /* sector where the mission is located */
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.LAIKA_LASTSTAND };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>();

            var toHangarSubmission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_GOTO_10_Name),
                MyMissionID.LAIKA_GOTO_10,
                (MyTextsWrapperEnum.LAIKA_GOTO_10_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective01_Hangar)
            ) { HudName = MyTextsWrapperEnum.HudHangar };
            //{ SaveOnSuccess = true };
            toHangarSubmission.OnMissionUpdate += new MissionHandler(toHangarSubmission_OnMissionUpdate);

            //toHangarSubmission.OnMissionSuccess += ToHangarSubmissionSuccess;
            toHangarSubmission.OnMissionLoaded += ToHangarSubmissionLoaded;
            m_objectives.Add(toHangarSubmission);

            var inHangarSubmission = new MyTimedObjective(
                MyTextsWrapperEnum.Null,
                MyMissionID.LAIKA_GOTO_11,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_10 },
                TimeSpan.FromSeconds(0.2f),
                successDialogId: MyDialogueEnum.LAIKA_0275_INSIDEHANGAR
            ) { DisplayCounter = false };

            inHangarSubmission.OnMissionSuccess += InHangarSubmissionSuccess;
            m_objectives.Add(inHangarSubmission);

            var inHangarSubmission2 = new MyTimedObjective(
                MyTextsWrapperEnum.Null,
                MyMissionID.LAIKA_GOTO_12,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_11 },
                TimeSpan.FromSeconds(5)
            ) { DisplayCounter = false };
            m_objectives.Add(inHangarSubmission2);

            m_generatorSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.LAIKA_GOTO_GENERATOR_Name),
                MyMissionID.LAIKA_GOTO_GENERATOR,
                (MyTextsWrapperEnum.LAIKA_GOTO_GENERATOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_12, },
                new List<uint> { (uint)EntityID.Objective01_Generator },
                successDialogID: MyDialogueEnum.LAIKA_0300_AFTERHANGAR,
                startDialogID: MyDialogueEnum.LAIKA_0277_DESTROYGENERATOR
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudGenerator };
            m_generatorSubmission.OnMissionSuccess += GeneratorSubmissionSuccess;
            m_generatorSubmission.OnMissionLoaded += GeneratorSubmissionLoaded;
            m_objectives.Add(m_generatorSubmission);

            m_toCommandCentreSubmission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMAND_Name),
                MyMissionID.LAIKA_GOTO_COMMAND,
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMAND_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_GENERATOR },
                null,
                new List<uint>() { (uint)EntityID.Objective02_CommandHub },
                successDialogId: MyDialogueEnum.LAIKA_0500_COMMAND,
                startDialogId: MyDialogueEnum.LAIKA_0350_GPS
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCommandCenter };
            m_toCommandCentreSubmission.OnMissionSuccess += ToCommandCentreSubmissionSuccess;
            m_toCommandCentreSubmission.OnMissionLoaded += ToCommandCentreSubmissionLoaded;
            m_objectives.Add(m_toCommandCentreSubmission);


            m_communication01Submission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMUNICATION_01_Name),
                MyMissionID.LAIKA_GOTO_COMMUNICATION_01,
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMUNICATION_01_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_COMMAND },
                null,
                new List<uint>() { (uint)EntityID.Objective03_Transmitter1Hub },
                successDialogId: MyDialogueEnum.LAIKA_0700_LEFTHUB
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCommunicationHub };
            m_communication01Submission.OnMissionSuccess += Communication01SubmissionSuccess;
            m_communication01Submission.OnMissionLoaded += Communication01SubmissionLoaded;
            m_objectives.Add(m_communication01Submission);


            m_communication02Submission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMUNICATION_02_Name),
                MyMissionID.LAIKA_GOTO_COMMUNICATION_02,
                (MyTextsWrapperEnum.LAIKA_GOTO_COMMUNICATION_02_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_COMMUNICATION_01 },
                null,
                new List<uint>() { (uint)EntityID.Objective04_Transmitter2Hub },
                successDialogId: MyDialogueEnum.LAIKA_0800_RIGHTHUB,
                startDialogId: MyDialogueEnum.LAIKA_0700_LEFTHUB2
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCommunicationHub };
            m_communication02Submission.OnMissionSuccess += Communication02SubmissionSuccess;
            m_communication02Submission.OnMissionLoaded += Communication02SubmissionLoaded;
            m_objectives.Add(m_communication02Submission);


            m_warheadSubmission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_WARHEAD_Name),
                MyMissionID.LAIKA_WARHEAD,
                (MyTextsWrapperEnum.LAIKA_WARHEAD_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_GOTO_COMMUNICATION_02 },
                null,
                new List<uint>() { (uint)EntityID.Objective05_Warhead },
                successDialogId: MyDialogueEnum.LAIKA_1800_WARHEAD_DONE
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudWarhead };
            m_warheadSubmission.OnMissionSuccess += WarheadSubmissionSuccess;
            m_warheadSubmission.OnMissionLoaded += WarheadSubmissionLoaded;
            m_objectives.Add(m_warheadSubmission);


            m_escapeSubmission = new MyObjective(
                (MyTextsWrapperEnum.LAIKA_RETURN_Name),
                MyMissionID.LAIKA_RETURN,
                (MyTextsWrapperEnum.LAIKA_RETURN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_WARHEAD },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: 1000
            ) { HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            m_escapeSubmission.OnMissionLoaded += EscapeSubmissionLoaded;
            m_escapeSubmission.OnMissionSuccess += EscapeSubmissionSuccess;
            m_escapeSubmission.OnMissionUpdate += EscapeSubmissionUpdate;
            m_escapeSubmission.OnMissionCleanUp += EscapeSubmissionUnload;
            m_objectives.Add(m_escapeSubmission);
                                
            var hideSubmission = new MyTimedObjective(
                (MyTextsWrapperEnum.LAIKA_LASTSTAND_Name),
                MyMissionID.LAIKA_LASTSTAND,
                (MyTextsWrapperEnum.LAIKA_LASTSTAND_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.LAIKA_RETURN },
                TimeSpan.FromSeconds(15), true
            ) { SaveOnSuccess = false,
                DisplayCounter = false,
                HudName = MyTextsWrapperEnum.Nothing
            };
            m_objectives.Add(hideSubmission);
            hideSubmission.OnMissionLoaded += new MissionHandler(hideSubmission_OnMissionLoaded);
        }

        #endregion

        private MyTimerActionDelegate m_giveHoundsTarget;
        private MySmallShipBot m_Marcus;
        private bool m_tunnelToRight = false;
        private bool m_tunnelToLeft = false;

        private Vector3 m_mothershipPosition_Russian_Final = new Vector3(-3046, -1489, -6821);
        private Vector3 m_mothershipPosition_Madelyn_Final = new Vector3(-215.0004f, -1853.138f, -4312.602f);

        private bool m_flyRussian = false;
        private bool m_flyMadelyn = false;

        MyEntity m_Madelyn;
        MyEntity m_RussianMS;

        public override void Accept()
        {
            base.Accept();
            MyScriptWrapper.AddHackingToolToPlayersInventory(3);

            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective02_CommandHub)).UseProperties.HackType = MyUseType.None;
            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective03_Transmitter1Hub)).UseProperties.HackType = MyUseType.None;
            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective04_Transmitter2Hub)).UseProperties.HackType = MyUseType.None;
            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective05_Warhead)).UseProperties.HackType = MyUseType.None;
        }

        public override void Load()
        {
            if (!IsMainSector)
                return;

            foreach (var item in m_staticAsteroidRubble)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(item));
            }
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Influence_Sphere_Fallout));

            m_Marcus = MyScriptWrapper.GetEntity("Marcus") as MySmallShipBot;

            m_hangar = MyScriptWrapper.GetEntity((uint)EntityID.Objective01_Hangar);

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityHacked += OnEntityHacked;
            MyScriptWrapper.OnBotReachedWaypoint += OnBotReachedWaypoint;
            MyScriptWrapper.EntityDeath += OnEntityDeath;

            m_MadelynSpeed = 200f;
            m_RussianSpeed = 100f;
            m_laikaBaseCalled = false;

            var pipeDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Pipe);
            pipeDetector.OnEntityEnter += PipeEntered;

            var rightEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_RightEntered);
            rightEnteredDetector.OnEntityEnter += RightEntered;

            var leftEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_LeftEntered);
            leftEnteredDetector.OnEntityEnter += LeftEntered;

            m_Madelyn = MyScriptWrapper.GetEntity("Madelyn");
            m_RussianMS = MyScriptWrapper.GetEntity((uint)EntityID.RussianMothership);
            //MyScriptWrapper.PrepareMotherShipForMove(m_RussianMS);

            foreach (var ship in m_smallShips)
            {
                MyScriptWrapper.DisableShip(MyScriptWrapper.TryGetEntity(ship));
            }

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_10))
            {
                ConvertToKGB();
            }
            else
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Door_ToHangar01), false);
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Door_ToHangar02), false);
            }

            if (!MyScriptWrapper.AreMissionFinished(new MyMissionID[] { MyMissionID.LAIKA_GOTO_COMMUNICATION_01, MyMissionID.LAIKA_GOTO_COMMUNICATION_02 }))
            {
                HideMothershipToShow();
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.RussianMothership));
            }

            BoundingBox box = MyScriptWrapper.GetEntity((uint)EntityID.Dummy_Remove_stuff_after_explosion).WorldAABB;
            //MyEntities.GetIntersectionWithSphere(ref sphere, null, null, true, false, ref entitiesAfterExplosion);
            List<MyEntity> entitiesAfterExplosion = MyGamePruningStructure.GetAllEntitiesInBox(ref box, MyGamePruningStructure.QueryFlags.Others);
            m_entitiesAfterExplosion.Clear();
            foreach (MyEntity entity in entitiesAfterExplosion)
            {
                if (entity.EntityId.HasValue)
                {
                    if ((entity is MySmallShipBot) ||
                        (entity is MyCargoBox) ||
                        (entity is MyVoxelMap) ||
                        (entity is MyStaticAsteroid) ||
                        (entity is MyDummyPoint) ||
                        (entity is MyPrefabLight) ||
                        (entity is MyPrefabHangar))
                    {
                        m_entitiesAfterExplosion.Add(entity.EntityId.Value.NumericValue);
                    }
                }
            }

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMAND))
            {
                HideMothershipToHide();
                AsteroidDetectorsActivate();

                if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01) || MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
                {
                    MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.GrilleToDestroy });
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToLeftHub);
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToRightHub);
                    if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01) == MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
                    {
                        SetHoundDetectorsBase();
                    }
                    else
                    {
                        if (!MyScriptWrapper.IsEntityDead((uint)EntityID.PipeToDestroy))
                        {
                            pipeDetector.On();
                        }
                        /*
                        if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01))
                        {
                            MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0720_AFTERLEFTHUB);
                        } */
                    }
                }
                else
                {
                    MyEntityDetector playerInPipesDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_PlayerInPipes);
                    playerInPipesDetector.OnEntityEnter += PlayerInPipes;
                    playerInPipesDetector.On();
                }

                if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01) && !MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
                {
                    SetHoundDetectorsLeft();
                }
                else if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02) && !MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01))
                {
                    SetHoundDetectorsRight();
                }

                if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
                {
                    RestartRussian();
                    RestartMadelyn();
                }
                else
                {
                    StopRussian();
                    //StopMadelyn();
                }
            }

            base.Load();
        }


        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityHacked -= OnEntityHacked;
            MyScriptWrapper.OnBotReachedWaypoint -= OnBotReachedWaypoint;
            MyScriptWrapper.EntityDeath -= OnEntityDeath;

            m_Marcus = null;
            m_Madelyn = null;
            m_hangar = null;
            m_RussianMS = null;
        }

        //bool explode = false;

        public override void Update()
        {
            if (!IsMainSector)
                return;

            base.Update();

            //RestartMadelyn();
            //StopMadelyn();

             /*
            if (MySession.PlayerShip.Physics.LinearVelocity.Length() > 10)
            {
                explode = true;
            } 
              
            if (explode)
            {
                MyNuclearExplosion.MakeExplosion(MyScriptWrapper.GetEntity(m_warheadSubmission.MissionEntityIDs[0]).GetPosition());
                explode = false;
                return;
            }  */

            if (m_flyRussian)
            {
                // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FlyMS");
                //     Too bad performance
                /*if (MoveMotherShipForward(m_RussianMS, m_RussianSpeed, m_mothershipPosition_Russian_Final, m_stuffWithRussianMothership))
                {
                    if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_RETURN))
                    {
                        m_RussianSpeed = 250;
                    }
                    else
                    {
                        StopRussian();
                        var russianStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopRussianMothershipMove);
                        russianStopDetector.Off();
                    }
                } 
              */
                MyScriptWrapper.Move(m_RussianMS, m_mothershipPosition_Russian_Final);
                var russianStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopRussianMothershipMove);
                russianStopDetector.Off();

                m_flyRussian = false;
            }
           
            if (m_flyMadelyn)
            {
                if (MoveMotherShipForward(m_Madelyn, m_MadelynSpeed, m_mothershipPosition_Madelyn_Final, m_stuffWithMadelyn))
                {
                    if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_WARHEAD))
                    {
                        StopMadelyn();
                        var madelynStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopMadelynMove);
                        madelynStopDetector.Off();
                        MyScriptWrapper.RegenerateWaypointGraph();
                    }
                }
            }
        }

        #region Event Handlers
        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (m_houndSpawnpoints.Contains(MyScriptWrapper.GetEntityId(spawnpoint)))
            {
                ((MySmallShipBot)MyScriptWrapper.GetSpawnPointLeader(spawnpoint)).Attack(MySession.PlayerShip);
            }
            else if ((MyScriptWrapper.GetEntityId(spawnpoint) == (uint)EntityID.Spawnpoint_LastFight_2) && (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_RETURN)))
            {
                ((MySmallShipBot)MyScriptWrapper.GetSpawnPointLeader(spawnpoint)).Attack(MySession.PlayerShip);
            }
        }

        void OnEntityHacked(MyEntity entity)
        {
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.Objective02_CommandHub))
            {
                m_toCommandCentreSubmission.Success();                
            } 
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.Objective03_Transmitter1Hub))
            {
                m_communication01Submission.Success();
            }
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.Objective04_Transmitter2Hub))
            {
                m_communication02Submission.Success();
            }
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.Objective05_Warhead))
            {
                m_warheadSubmission.Success();
            }
        }

        void OnBotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (bot == MyScriptWrapper.GetSpawnPointLeader((uint)EntityID.Spawnpoint_ToRightHub))
            {
                switch (MyScriptWrapper.GetEntityId(waypoint))
                {
                    case (uint)EntityID.Waypoint_Right_Wait:
                        if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
                        {
                            ((MySmallShipBot)bot).SuspendPatrol = true;
                        }
                        else
                        {
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.RightLatticeToDestroy0 }); 
                        }
                        break;
                    case (uint)EntityID.Waypoint_Right_Voxel:
                        if (!m_tunnelToRight)
                        {
                        MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Dummy_DestroyVoxel01).GetPosition(), Explosions.MyExplosionTypeEnum.BOMB_EXPLOSION, 50.0f, 10.0f);
                        MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Dummy_DestroyVoxel02).GetPosition(), Explosions.MyExplosionTypeEnum.BOMB_EXPLOSION, 50.0f, 10.0f);
                        MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Dummy_DestroyVoxel03).GetPosition(), Explosions.MyExplosionTypeEnum.BOMB_EXPLOSION, 50.0f, 10.0f);
                        }
                        break;
                    case (uint)EntityID.Waypoint_Right_Lattice1:
                        if (!m_tunnelToRight)
                        {
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.RightLatticeToDestroy1 });
                        }
                        break;
                    case (uint)EntityID.Waypoint_Right_Lattice2:
                        if (!m_tunnelToRight)
                        {
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.RightLatticeToDestroy2 });
                            m_tunnelToRight = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (bot == MyScriptWrapper.GetSpawnPointLeader((uint)EntityID.Spawnpoint_ToLeftHub))
            {
                switch (MyScriptWrapper.GetEntityId(waypoint))
                {
                    case (uint)EntityID.Waypoint_Left_Wait:
                        if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01))
                        {
                            ((MySmallShipBot)bot).SuspendPatrol = true;
                        }
                        break;
                    case (uint)EntityID.Waypoint_Left_Lattice1:
                        if (!m_tunnelToLeft)
                        {
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.LeftLatticeToDestroy1 });
                        }
                        break;
                    case (uint)EntityID.Waypoint_Left_Lattice2:
                        if (!m_tunnelToLeft)
                        {
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.LeftLatticeToDestroy2 });
                            m_tunnelToLeft = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void OnEntityDeath(MyEntity entity, MyEntity killedBy)
        {
            if (entity == MyScriptWrapper.TryGetEntity((uint)EntityID.PipeToDestroy))
            {
                var pipeDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Pipe);
                pipeDetector.Off();
            }
        }

        #endregion


        void toHangarSubmission_OnMissionUpdate(MyMissionBase sender)
        {
            if (!m_laikaBaseCalled && Vector3.Distance(MySession.PlayerShip.GetPosition(), m_hangar.GetPosition()) < 3000)
                CallLaikaBase();
        }


        private void ToHangarSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 100, "MM_b");

            MyEntityDetector hangarDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Hangar);
            hangarDetector.On();
            MissionTimer.RegisterTimerAction(1000, Arrival, false);
            m_laikaBaseCalled = false;
        }

        private void Arrival()
        {
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.LAIKA_0100_ARRIVAL);
        }

        private void CallLaikaBase()
        {
            m_laikaBaseCalled = true;
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.LAIKA_0250_WAYTOHANGAR);
        }

        private void InHangarSubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hangar_Back);

            // Activate ambush
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_HangarAmbush1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_HangarAmbush2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_CivilianArea);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ResearchPatrol);

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Door_ToHangar01), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Door_ToHangar02), true);

            ConvertToKGB();
        }

        private void GeneratorSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 100);

            //((MyObjective)sender).MissionTimer.RegisterTimerAction(3000, DestroyGeneratorDialog, false);
        }

        /*
        private void DestroyGeneratorDialog()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0277_DESTROYGENERATOR);
        } */

        private void GeneratorSubmissionSuccess(MyMissionBase sender)
        {
            //MissionTimer.RegisterTimerAction(8000, AfterHangarChatter, false);
        }
        
        private void ToCommandCentreSubmissionLoaded(MyMissionBase sender)
        {
            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective02_CommandHub)).UseProperties.HackType = MyUseType.Solo;

            MyEntityDetector commandCenterDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_PlayerInPipes);
            commandCenterDetector.OnEntityEnter += PlayerInCommandCenter;
            commandCenterDetector.On();
        }


        void PlayerInCommandCenter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyEntityDetector commandCenterDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_PlayerInPipes);
                commandCenterDetector.OnEntityEnter -= PlayerInCommandCenter;
                commandCenterDetector.Off();

                MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0480_COMMANDARRIVE);
            }
        }


        private void ToCommandCentreSubmissionSuccess(MyMissionBase sender)
        {
            HideMothershipToHide();

            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective03_Transmitter1Hub)).UseProperties.HackType = MyUseType.Solo;

            MyEntityDetector playerInPipesDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_PlayerInPipes);
            playerInPipesDetector.OnEntityEnter += PlayerInPipes;
            playerInPipesDetector.On();

            var rightEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_RightEntered);
            rightEnteredDetector.On();
            var leftEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_LeftEntered);
            leftEnteredDetector.On();

            AsteroidDetectorsActivate();

            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_GENERATOR))
            {
                m_generatorSubmission.Success();
            }
        }


        private void PlayerInPipes(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.GrilleToDestroy });
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToLeftHub);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToRightHub);
                SetHoundDetectorsBase();
                var rightEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_RightEntered);
                rightEnteredDetector.Off();
                var leftEnteredDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_LeftEntered);
                leftEnteredDetector.Off();
                MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.Waypoint_ToAmbush }, true);
                sender.Off();
            }
        }

        private void PipeEntered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Ambush);
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.MothershipToShow));
                
                foreach (var value in m_MothershipToShowThrusters)
                {
                    MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity(value));
                }
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.PipeToDestroy });
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.Particle_Ambush_mist), true);
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.Particle_Ambush_smoke), true);
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.Particle_Ambush_explosion), true);
                
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Prefab_Ambush_1));
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Prefab_Ambush_2));
            }
        }

        private void RightEntered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.GrilleToDestroy });
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToLeftHub);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToRightHub);
                sender.Off();
            }
        }

        private void LeftEntered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.GrilleToDestroy });
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToLeftHub);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_ToRightHub);
                sender.Off();
            }
        }

        private void WarheadDiscovered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0900_WARHEAD);
                MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.Waypoint_Right_Door }, false);
                sender.OnEntityEnter -= WarheadDiscovered;
                sender.Off();
            }
        }

        private void WarheadDetectorEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0950_WARHEAD_HACK);
                sender.OnEntityEnter -= WarheadDetectorEnter;
                sender.Off();
            }
        }

        private void Communication01SubmissionLoaded(MyMissionBase sender)
        {
            MissionEntityIDs.Add((uint)EntityID.Objective03_Transmitter1Hub);
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_0600_TOPIPE);
            }
        }

        private void Communication01SubmissionSuccess(MyMissionBase sender)
        {
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_02))
            {
                var pipeDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Pipe);
                pipeDetector.On();
            }
            
            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective04_Transmitter2Hub)).UseProperties.HackType = MyUseType.Solo;

            SetHoundDetectorsLeft();
            MySmallShipBot leader = MyScriptWrapper.GetSpawnPointLeader(MyScriptWrapper.GetEntity((uint)EntityID.Spawnpoint_ToLeftHub)) as MySmallShipBot;
            if (leader != null)
            {
                leader.SuspendPatrol = false;
            }
        }

        private void Communication02SubmissionLoaded(MyMissionBase sender)
        {
            var warheadDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Warhead);
            warheadDetector.OnEntityEnter += WarheadDiscovered;
            warheadDetector.On();

            MissionEntityIDs.Add((uint)EntityID.Objective04_Transmitter2Hub);
        }

        private void Communication02SubmissionSuccess(MyMissionBase sender)
        {
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_GOTO_COMMUNICATION_01))
            {
                var pipeDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Pipe);
                pipeDetector.On();
            }

            ((IMyUseableEntity)MyScriptWrapper.GetEntity((uint)EntityID.Objective05_Warhead)).UseProperties.HackType = MyUseType.Solo;

            SetHoundDetectorsRight();
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointHub2);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorLock_Right1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorLock_Right2), true);
            MySmallShipBot leader = MyScriptWrapper.GetSpawnPointLeader(MyScriptWrapper.GetEntity((uint)EntityID.Spawnpoint_ToRightHub)) as MySmallShipBot;
            if (leader != null)
            {
                leader.SuspendPatrol = false;
            }
            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.RightLatticeToDestroy0 });
        }

        private void WarheadSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.RussianMothership));
            MyScriptWrapper.EnablePhysics((uint)EntityID.RussianMothership, false, true);
            MyScriptWrapper.EnablePhysics(MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue, false, true);
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.PipeToDestroy_waytowarhead });

            //MyScriptWrapper.SetWaypointListSecrecy(m_leavePipeWaypoints, false);

            RestartMadelyn();
            var madelynStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopMadelynMove);
            madelynStopDetector.OnEntityEnter += StopMadelyn;
            madelynStopDetector.OnEntityLeave += RestartMadelyn;
            madelynStopDetector.On();

            RestartRussian();
            var russianStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopRussianMothershipMove);
            russianStopDetector.OnEntityEnter += StopRussian;
            russianStopDetector.OnEntityLeave += RestartRussian;
            russianStopDetector.On();

            var warheadDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Warhead);
            warheadDetector.OnEntityEnter += WarheadDetectorEnter;
            warheadDetector.On();
        }

        private void FindWarheadSubmissionSuccess(MyMissionBase sender)
        {
           // MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Tunnel_explosion).GetPosition(), Explosions.MyExplosionTypeEnum.BOMB_EXPLOSION, 40.0f, 10.0f);
        }

        private void WarheadSubmissionSuccess(MyMissionBase sender)
        {
            //MyScriptWrapper.EnablePhysics((uint)EntityID.RussianMothership, true);
            //MyScriptWrapper.EnablePhysics(MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue, true);
            MyScriptWrapper.SetWaypointListSecrecy(new List<uint> { (uint)EntityID.Waypoint_Unsecret_Escape }, false);

            //m_Madelyn.SetPosition(MyScriptWrapper.GetEntity((uint)EntityID.Dummy_Madelyn_Moved).GetPosition());
            MoveMotherShip(m_Madelyn, MyScriptWrapper.GetEntity((uint)EntityID.Dummy_Madelyn_Moved).GetPosition(), m_stuffWithMadelyn);
        }

        private void EscapeSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Horror, 100, "KA02");

            RestartMadelyn();
            //m_mothershipPosition_Madelyn_Final = new Vector3(0f);
            
            var madelynStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopMadelynMove);
            madelynStopDetector.On();
            m_MadelynSpeed = 80f;

            var warheadDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Warhead);
            warheadDetector.Off();

            MyScriptWrapper.HideEntity(m_RussianMS, true);

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_1800_WARHEAD_DONE2);
        }

        private void EscapeSubmissionSuccess(MyMissionBase sender)
        {
            StopMadelyn();

            var madelynStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopMadelynMove);
            madelynStopDetector.Off();
            MyScriptWrapper.EnablePhysics(MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue, true, true);

            MyScriptWrapper.MakeNuclearExplosion(MyScriptWrapper.GetEntity(m_warheadSubmission.MissionEntityIDs[0]));
        }



        private void EscapeSubmissionUpdate(MyMissionBase sender)
        {
            var dummy = MyScriptWrapper.TryGetEntity((uint)EntityID.Dummy_Remove_stuff_after_explosion);
            if (dummy != null && (dummy.GetPosition() - MySession.PlayerShip.GetPosition()).Length() > 3641.1f/2)
            {
                MyScriptWrapper.StopMusic();
            }
        }

        private void EscapeSubmissionUnload(MyMissionBase sender)
        {
        }

        List<uint> m_entitiesAfterExplosion = new List<uint>(1024);

        private void RussianExplodesBase()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.LAIKA_1900_ESCAPED);

            foreach (uint entityID in m_entitiesAfterExplosion)
            {
                MyEntity entity = MyScriptWrapper.TryGetEntity(entityID);
                if (entity != null)
                {
                    MyScriptWrapper.HideEntity(entity, true);
                }
            }

            foreach (var item in m_staticAsteroidRubble)
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(item), true);
            }
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Influence_Sphere_Fallout), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Stuff_to_remove_prefab_container1), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BaseCivilianParts), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BaseMain), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Stuff_to_remove_prefab_container4), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.MothershipToShow), true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BaseStore), true);
        }

        void hideSubmission_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA05");

            m_RussianSpeed = 250;
            m_mothershipPosition_Russian_Final = new Vector3(0f);
            RestartRussian();
            var russianStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopRussianMothershipMove);
            russianStopDetector.Off();
            MissionTimer.RegisterTimerAction(4000, RussianExplodesBase, false);
        }


        private void ConvertToKGB()
        {
            // change factions relations
            MyScriptWrapper.ChangeFaction((uint)EntityID.BaseStation1, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
            MyScriptWrapper.ChangeFaction((uint)EntityID.BaseStation2, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
            MyScriptWrapper.ChangeFaction((uint)EntityID.BaseStore, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
            MyScriptWrapper.ChangeFaction((uint)EntityID.BaseMain, MyMwcObjectBuilder_FactionEnum.Russian_KGB);

            foreach (var spawn in new List<uint> { (uint)EntityID.Spawnpoint_01, (uint)EntityID.Spawnpoint_Hangar_Back, (uint)EntityID.Spawnpoint_03, (uint)EntityID.Spawnpoint_04 })
            {
                MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    if (bot.Ship != null)
                    {
                        MyScriptWrapper.ChangeFaction(bot.Ship.EntityId.Value.NumericValue, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
                    }
                }
            }
        }

        private void HideMothershipToHide()
        {
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.MothershipToHide));
            foreach (var value in m_MothershipToHideThrusters)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity(value));
            }
        }

        private void HideMothershipToShow()
        {
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.MothershipToShow)); // hide ambush assault vessel
            foreach (var value in m_MothershipToShowThrusters)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity(value));
            }
        }

        private void AsteroidDetectorsActivate()
        {
            foreach (var detector in m_asteroidDetectors)
            {
                MyEntityDetector Detector = MyScriptWrapper.GetDetector(detector);
                Detector.OnEntityEnter += AsteroidTriggered;
                Detector.On();
            }
        }

        private void AsteroidTriggered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Asteroid);
                foreach (var detector in m_asteroidDetectors)
                {
                    MyEntityDetector Detector = MyScriptWrapper.GetDetector(detector);
                    Detector.Off();
                    Detector.OnEntityEnter -= AsteroidTriggered;
                }
                MyScriptWrapper.SetWaypointListSecrecy(m_asteroidSecretWaypoints, false);
            }
        }

        private void SetHoundDetectorsRight()
        {
            MyEntityDetector RightUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Right_Up);
            RightUpDetector.OnEntityEnter += RightUp;
            RightUpDetector.On();
            MyEntityDetector RightDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Right_Down);
            RightDownDetector.OnEntityEnter += RightDown;
            RightDownDetector.On();
        }

        private void DeactivateHoundDetectorsRight()
        {
            MyEntityDetector RightUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Right_Up);
            RightUpDetector.OnEntityEnter -= RightUp;
            RightUpDetector.Off();
            MyEntityDetector RightDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Right_Down);
            RightDownDetector.OnEntityEnter -= RightDown;
            RightDownDetector.Off();
        }

        private void SetHoundDetectorsLeft()
        {
            MyEntityDetector LeftUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Left_Up);
            LeftUpDetector.OnEntityEnter += LeftUp;
            LeftUpDetector.On();
            MyEntityDetector LeftDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Left_Down);
            LeftDownDetector.OnEntityEnter += LeftDown;
            LeftDownDetector.On();
        }

        private void DeactivateHoundDetectorsLeft()
        {
            MyEntityDetector LeftUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Left_Up);
            LeftUpDetector.OnEntityEnter -= LeftUp;
            LeftUpDetector.Off();
            MyEntityDetector LeftDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Left_Down);
            LeftDownDetector.OnEntityEnter -= LeftDown;
            LeftDownDetector.Off();
        }

        private void SetHoundDetectorsBase()
        {
            MyEntityDetector BaseUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Base_Up);
            BaseUpDetector.OnEntityEnter += BaseUp;
            BaseUpDetector.On();
            MyEntityDetector BaseDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Base_Down);
            BaseDownDetector.OnEntityEnter += BaseDown;
            BaseDownDetector.On();
        }

        private void DeactivateHoundDetectorsBase()
        {
            MyEntityDetector BaseUpDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Base_Up);
            BaseUpDetector.OnEntityEnter -= BaseUp;
            BaseUpDetector.Off();
            MyEntityDetector BaseDownDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Base_Down);
            BaseDownDetector.OnEntityEnter -= BaseDown;
            BaseDownDetector.Off();
        }

        private void RightUp(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Right_Up);
                DeactivateHoundDetectorsRight();
            }
        }

        private void RightDown(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Right_Down);
                DeactivateHoundDetectorsRight();
            }
        }

        private void LeftUp(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Left_Up);
                DeactivateHoundDetectorsLeft();
            }
        }

        private void LeftDown(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Left_Down);
                DeactivateHoundDetectorsLeft();
            }
        }

        private void BaseUp(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Base_Up);
                DeactivateHoundDetectorsBase();
            }
        }

        private void BaseDown(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Hounds_Base_Down);
                DeactivateHoundDetectorsBase();
            }
        }

        private void StopMadelyn(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                StopMadelyn();
            }
        }

        private void StopMadelyn()
        {
            m_flyMadelyn = false;
            //MyScriptWrapper.EnablePhysics(MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue, true, true);
            //MyScriptWrapper.ReturnMotherShipFromMove(MyScriptWrapper.GetEntity("Madelyn"));

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_WARHEAD) && !MyScriptWrapper.IsMissionFinished(MyMissionID.LAIKA_RETURN))
            {
                var madelynStopDetector = MyScriptWrapper.GetDetector((uint)EntityID.Detector_StopMadelynMove);
                madelynStopDetector.Off();
                m_escapeSubmission.Success();
            }
        }

        private void RestartMadelyn(MyEntityDetector sender, MyEntity entity)
        {
            if (MySession.IsPlayerShip(entity))
            {
                RestartMadelyn();
            }
        }

        private void RestartMadelyn()
        {
            m_flyMadelyn = true;
            //MyScriptWrapper.EnablePhysics(MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue, false, true);
            //MyScriptWrapper.PrepareMotherShipForMove(MyScriptWrapper.GetEntity("Madelyn"));
        }

        private void StopRussian(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                StopRussian();
            }
        }

        private void StopRussian()
        {
            if (m_flyRussian == true)
            {
                m_flyRussian = false;
                //MyScriptWrapper.EnablePhysics((uint)EntityID.RussianMothership, true, true);
                //MyScriptWrapper.ReturnMotherShipFromMove(MyScriptWrapper.GetEntity((uint)EntityID.RussianMothership));
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_MothershipHounds);
            }
        }

        private void RestartRussian(MyEntityDetector sender, MyEntity entity)
        {
            if (MySession.IsPlayerShip(entity))
            {
                RestartRussian();
            }
        }

        private void RestartRussian()
        {
            m_flyRussian = true;
            //MyScriptWrapper.EnablePhysics((uint)EntityID.RussianMothership, false, true);
            //MyScriptWrapper.PrepareMotherShipForMove(MyScriptWrapper.GetEntity((uint)EntityID.RussianMothership));
            //MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawn_MothershipHounds);
        }


        bool MoveMotherShipForward(MyEntity entity, float speed, Vector3 destination, List<uint> AdditionalStuffToMove) //helps to move even non prefab entities with the ship, like thruster particles and spawnpoints
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            if (Vector3.DistanceSquared(destination, entity.GetPosition()) > 10 * 10)
            {
                //Render.MyRender.ClearEnhancedStats();
                MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS); // recalculate position
                foreach (var item in AdditionalStuffToMove)
                {
                    MyEntity Item = MyScriptWrapper.GetEntity(item);
                    if (Item != null)
                    {
                        MyScriptWrapper.Move(Item, Item.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                    }
                }
                //MyScriptWrapper.GetEntity((uint)EntityID.CrashingMothershipSmoke).SetPosition(MyScriptWrapper.GetEntity((uint)EntityID.CrashingMothershipSmoke).GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                //Render.MyRender.ClearEnhancedStats();
                return false;
            }
            return true;
        }

        void MoveMotherShip(MyEntity entity, Vector3 destination, List<uint> AdditionalStuffToMove) //helps to move even non prefab entities with the ship, like thruster particles and spawnpoints
        {
            MyScriptWrapper.Move(entity, destination);

            Vector3 delta = destination - entity.GetPosition();

            foreach (var item in AdditionalStuffToMove)
            {
                MyEntity Item = MyScriptWrapper.GetEntity(item);
                if (Item != null)
                {
                    MyScriptWrapper.Move(Item, Item.GetPosition() + delta);
                }
            }
        }
    }
}

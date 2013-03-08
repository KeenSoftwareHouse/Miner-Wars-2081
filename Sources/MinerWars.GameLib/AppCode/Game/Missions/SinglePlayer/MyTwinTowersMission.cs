using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWarsMath;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    //Doppelburg?
    class MyTwinTowersMission : MyMission
    {
        #region EntitiyIDs and entities

        enum EntityID
        {
            StartLocation = 2315,
            ReichMothership = 11747,

            Objective_randevouz = 3791,

            Objective_Railgun1 = 3792,
            Objective_Railgun1_Controls_01 = 34549,
            Objective_Railgun1_Controls_02 = 34550,

            Objective_Railgun2 = 7160,
            Objective_Railgun2_Controls_01 = 38701,
            Objective_Railgun2_Controls_02 = 38702,
            Objective_Railgun2_Controls_03 = 38703,
            Objective_Railgun2_Controls_04 = 38704,
            Objective_RightBase = 9580,

            Objective_Generator = 9581,
            Objective_Generator_Pipe1 = 1264,
            Objective_Generator_Pipe2 = 1293,
            Prefab_Generator_ToDisable = 13697,
            Prefab_Light_ToDisable_01 = 39800,
            Prefab_Light_ToDisable_02 = 39801,
            Prefab_Light_ToEnable_01 = 39796,
            Prefab_Light_ToEnable_02 = 39795,

            Objective_Command = 9582,
            Objective_Command_HUB = 16188,

            Sabotage1 = 3459,
            Sabotage2 = 3460,
            Sabotage3 = 3461,
            Sabotage4 = 3462,
            Explosives1 = 23463,
            Explosives2 = 23462,
            Explosives3 = 23464,
            Explosives4 = 23467,
            Prefab_Rubble_01 = 41862,
            Prefab_Sabotage1_ToDestroy1 = 8811,
            Dummy_Sabotage1 = 41869,
            Prefab_Rubble_02 = 39842,
            Prefab_Sabotage2_ToDestroy1 = 12680,
            Prefab_Sabotage2_ToDestroy2 = 12681,
            Prefab_Sabotage2_ToDestroy3 = 12682,
            Prefab_Rubble_03 = 41870,
            Prefab_Sabotage3_ToDestroy1 = 1401,
            Prefab_Sabotage3_ToDestroy2 = 1161,
            Prefab_Sabotage3_ToDestroy3 = 24231,
            Prefab_Sabotage3_ToDestroy4 = 24232,
            Dummy_Sabotage3 = 42881,
            Prefab_Rubble_04 = 43168,
            Prefab_Sabotage4_ToDestroy1 = 43156,
            Prefab_Sabotage4_ToDestroy2 = 43155,
            Prefab_Sabotage4_ToDestroy3 = 43165,
            Prefab_Sabotage4_ToDestroy4 = 23468,
            Dummy_Sabotage4 = 43178,

            Bot_Friend_Hacker = 7315,
            //Bot_Friend_01 = 31403,
            //Bot_Friend_02 = 31416,
            //Bot_Friend_03 = 31429,

            Spawnpoint_Command = 7309,
            Spawnpoint_Jammer = 7835,
            Spawnpoint_Jammer_Guards = 7834,

            Detector_Command = 7328,
            Detector_Hacker = 11986,
            Detector_Friends = 30428,

            Spawnpoint_Left_Assault_01 = 7310,
            Bot_Left_01_01 = 22002,
            Bot_Left_01_02 = 22003,
            Spawnpoint_Left_Assault_02 = 7311,
            Bot_Left_02_01 = 21989,
            Bot_Left_02_02 = 21976,
            Bot_Left_02_03 = 21951,
            Spawnpoint_Left_Assault_03 = 7312,
            //Bot_Left_03_01 = 21887,
            //Bot_Left_03_02 = 21886,
            Spawnpoint_Left_Assault_04 = 7313,
            Bot_Left_04_01 = 21836,
            Bot_Left_04_02 = 21861,
            Spawnpoint_Left_Assault_05 = 7314,
            Bot_Left_05_01 = 21938,
            Bot_Left_05_02 = 21912,
            Bot_Left_05_03 = 21913,

            Spawnpoint_LastWave = 25254,

            Spawnpoint_Right_Assault_01 = 9391,
            //Bot_Right_01_01 = 24323,
            //Bot_Right_01_02 = 24322,
            Spawnpoint_Right_Assault_02 = 9392,
            //Bot_Right_02_01 = 24245,
            //Bot_Right_02_02 = 24258,
            Spawnpoint_Right_Assault_03 = 9393,
            Bot_Right_03_01 = 24350,
            Bot_Right_03_02 = 24348,
            Bot_Right_03_03 = 24349,
            Spawnpoint_Right_Assault_04 = 9394, //disabable
            Bot_Right_04_01 = 24297,
            Bot_Right_04_02 = 24271,
            Bot_Right_04_03 = 24272,

            Spawnpoint_Railgun_Room_01 = 8626,
            Spawnpoint_Railgun_Room_02 = 8627,

            Spawnpoint_Right_Command = 9397,
            Spawnpoint_Right_Lower = 9396,
            Spawnpoint_Right_Upper = 9395,

            Spawnpoint_Zeppelin_FoR_01 = 28037,
            Spawnpoint_Zeppelin_FoR_02 = 28038,

            Spawnpoint_Zeppelin_WW1_01 = 28473,
            Spawnpoint_Zeppelin_WW1_02 = 28474,
            Spawnpoint_Zeppelin_WW1_03_Hounds = 28475,
            Detector_Zeppelin_WW1_Hounds = 28476,

            Zeppelin_WW_1_Static = 20365,
            Zeppelin_WW_2 = 20837,
            Zeppelin_FoR_1 = 19893,
            Zeppelin_WW_1_Moving = 69,

            Objective_Zeppelin1_Battery1 = 20749,
            Objective_Zeppelin1_Battery2 = 20467,
            Objective_Zeppelin1_GeneratorWithoutBatteries = 20638,
            Objective_Zeppelin1_Generator = 20666,

            Objective_Zeppelin2_Command = 27123,

            Prefab_Base_Left_01 = 2232,
            Prefab_Base_Left_02 = 1969,
            Prefab_Base_Left_03 = 483,

            Prefab_Base_Right_01 = 1160,
            Prefab_Base_Right_02 = 11215,
            Prefab_Base_Right_03 = 2152,
            Prefab_Base_Bridge = 267,
            Prefab_Base_Construction = 1565,

            Prefab_Door_LeftCommand1 = 820,
            Prefab_Door_LeftCommand2 = 859,

            Spawn_point_left_middle = 45275,
            Spawn_point_left_down = 45274,
            Spawn_point_left_up = 45276,
            Spawn_point_right_up = 45277,
            Spawn_point_right_down = 45278,

            Spawn_point_End_friends1 = 16778202,
            Spawn_point_End_friends2 = 16778203,

            Waypoint_hacker_final = 11983,
            Hacker_hub = 2233,
        }

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof (EntityID)))
            {
                MyScriptWrapper.GetEntity((uint) ((value as EntityID?).Value));
            }
            foreach (var value in m_turrets1)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        private uint[] m_WWZeppelinSpawns = new uint[] {
            (uint)EntityID.Spawnpoint_Zeppelin_WW1_01,
            (uint)EntityID.Spawnpoint_Zeppelin_WW1_02,
            (uint)EntityID.Spawnpoint_Zeppelin_WW1_03_Hounds
        };

        List<uint> m_botsInLeftHangars = new List<uint>
            {
                (uint) EntityID.Bot_Left_01_01,
                (uint) EntityID.Bot_Left_01_02,
                (uint) EntityID.Bot_Left_02_01,
                (uint) EntityID.Bot_Left_02_02,
                (uint) EntityID.Bot_Left_02_03,
                /*(uint)EntityID.Bot_Left_03_01, (uint)EntityID.Bot_Left_03_02,*/ (uint) EntityID.Bot_Left_04_01,
                (uint) EntityID.Bot_Left_04_02,
                (uint) EntityID.Bot_Left_05_01,
                (uint) EntityID.Bot_Left_05_02,
                (uint) EntityID.Bot_Left_05_03,
            };

        List<uint> m_botsInRightHangars = new List<uint>
            {
                /*(uint)EntityID.Bot_Right_01_01, (uint)EntityID.Bot_Right_01_02, (uint)EntityID.Bot_Right_02_01, (uint)EntityID.Bot_Right_02_02,*/
                (uint) EntityID.Bot_Right_03_01,
                (uint) EntityID.Bot_Right_03_02,
                (uint) EntityID.Bot_Right_03_03,
                (uint) EntityID.Bot_Right_04_01,
                (uint) EntityID.Bot_Right_04_02,
                (uint) EntityID.Bot_Right_04_03
            };

        List<uint> m_turrets1 = new List<uint>
            {
                7869,
                7867,
                7872,
                7875,
                7887,
                7881,
                7879
            };

        /*
        private List<uint> m_list_friends = new List<uint>
        {
            (uint)EntityID.Bot_Friend_01, (uint)EntityID.Bot_Friend_02, (uint)EntityID.Bot_Friend_03
        };
        */

        Int32 m_explosives;

        MySmallShipBot m_hacker;
        MyEntity m_jammer;

        MyEntityDetector m_commandDetector;

        MyEntity m_explosives01;
        MyEntity m_explosives02;
        MyEntity m_explosives03;
        MyEntity m_explosives04;

        #endregion

        private int? m_shootWarningTime;
        private bool m_flyWWMS1;
        private bool m_flyWWMS2;
        private bool m_flyFoRMS;

        private bool m_WWMS1_passedmark;
        private bool m_WWMS2_passedmark;
        private bool m_FoRMS_passedmark;

        private Vector3 m_WW1mothershipPosition = new Vector3(-7004.5f, -168f, -8494f);
        private Vector3 m_WW2mothershipPosition = new Vector3(-6208, -3509, -7949);
        private Vector3 m_FoRmothershipPosition = new Vector3(-4920.5f, 447, -3742.5f);

        private Vector3 m_WW1mothershipPositionHalf = new Vector3(-7362f, -168f, -9111f);
        //private Vector3 m_WW2mothershipPosition = new Vector3(-6208, -3509, -7949);
        private Vector3 m_FoRmothershipPositionHalf = new Vector3(-4253f, 447f, -3922f);

        private MyEntity m_WWMS1_Moving;
        private MyEntity m_WWMS1_Static;
        private MyEntity m_WWMS2;
        private MyEntity m_FoRMS;

        #region Submissions

        private MyObjective m_introTalk;
        private MyObjective m_assaultSubmission;
        private MyTimedObjective m_hackerSubmission;
        private MyTimedObjective m_hackerSubmission2;
        private MyObjectiveDestroy m_jammerSubmission;
        private MyObjective m_rightBaseCommandSubmission;
        private MyObjectiveDestroy m_mothership1Submission;
        private MyObjectiveDestroy m_mothership1Submission2;
        private MyObjective m_waitSubmission;

        public MyTwinTowersMission()
        {
            ID = MyMissionID.TWIN_TOWERS; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.TWIN_TOWERS;
            Description = MyTextsWrapperEnum.TWIN_TOWERS_Description;
            DebugName = new StringBuilder("19-Doppelburg");
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2809328, 0, -4609055);

            /* sector where the mission is located */
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.REICHSTAG_C };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.TWIN_TOWERS_MOTHERSHIP1_V2 };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN  };
            MovePlayerToMadelynHangar = false;

            m_objectives = new List<MyObjective>();

            m_introTalk = new MyObjective(
                MyTextsWrapperEnum.Blank,
                MyMissionID.TWIN_TOWERS_INTRO,
                MyTextsWrapperEnum.Blank,
                null,
                this,
                new MyMissionID[] { },
                null
                ) { StartDialogId = MyDialogueEnum.TWIN_TOWERS_0100_INTRO };
            m_objectives.Add(m_introTalk);

            var multipleSabotage = new MyMultipleUseObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_SABOTAGE_Name),
                MyMissionID.TWIN_TOWERS_SABOTAGE,
                (MyTextsWrapperEnum.TWIN_TOWERS_SABOTAGE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_INTRO },
                MyTextsWrapperEnum.PressToPlaceExplosive,
                MyTextsWrapperEnum.Blank,
                MyTextsWrapperEnum.PlacingExplosives,
                1000,
                new List<uint> { (uint)EntityID.Explosives2, (uint)EntityID.Explosives3, (uint)EntityID.Explosives1 },
                MyUseObjectiveType.Putting
                ) { StartDialogId = MyDialogueEnum.TWIN_TOWERS_0200_PLACE_EXPLOSIVES };
            multipleSabotage.OnObjectUsedSucces += MultipleSabotageOnOnObjectUsedSucces;
            multipleSabotage.RequiredUncompletedMissions = new MyMissionID[] { MyMissionID.TWIN_TOWERS_RANDEVOUZ };
            multipleSabotage.Components.Add(
                new MyDetectorDialogue(
                    (uint) EntityID.Sabotage1,
                    MyDialogueEnum.TWIN_TOWERS_0300_player_reaching_the_main_electricity_supply));
            multipleSabotage.Components.Add(
                new MyDetectorDialogue(
                    (uint) EntityID.Sabotage2,
                    MyDialogueEnum.TWIN_TOWERS_0400_reaching_a_hangar_with_unmanned_enemy_small_ships));
            multipleSabotage.Components.Add(
                new MyDetectorDialogue(
                    (uint) EntityID.Sabotage3, MyDialogueEnum.TWIN_TOWERS_0500_reaching_electricity_distribution_HUB));
            m_objectives.Add(multipleSabotage);

            /*
            var sabotageSubmission04 = new MySabotageSubmission(
                new StringBuilder("Industrial section control room"),
                MyMissionID.TWIN_TOWERS_SABOTAGE04,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.Sabotage4)
            );
            sabotageSubmission04.RequiredUncompletedMissions = new MyMissionID[] { MyMissionID.TWIN_TOWERS_RANDEVOUZ };
            sabotageSubmission04.OnMissionSuccess += Sabotage04SubmissionSuccess;
            m_objectives.Add(sabotageSubmission04);
            */
            var randevouzSubmission = new MyObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_RANDEVOUZ_Name),
                MyMissionID.TWIN_TOWERS_RANDEVOUZ,
                (MyTextsWrapperEnum.TWIN_TOWERS_RANDEVOUZ_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_SABOTAGE },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective_randevouz)
            )
                {
                    SaveOnSuccess = true,
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_0600_after_all_the_sabotages_are_done,
                    HudName = MyTextsWrapperEnum.HudMeetingPoint
                };
            randevouzSubmission.OnMissionLoaded += RandevouzSubmissionLoaded;
            randevouzSubmission.OnMissionSuccess += RandevouzSubmissionSuccess;
            m_objectives.Add(randevouzSubmission);

            m_assaultSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_ASSAULT_Name),
                MyMissionID.TWIN_TOWERS_ASSAULT,
                (MyTextsWrapperEnum.TWIN_TOWERS_ASSAULT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_RANDEVOUZ },
                null,
                new List<uint> { (uint) EntityID.Spawnpoint_Command }
                )
                {
                    SaveOnSuccess = true,
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_0700_Meeting_point,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_0800_command_center_cleared
                };
            m_assaultSubmission.OnMissionSuccess += AssaultSubmissionSuccess;
            m_assaultSubmission.OnMissionLoaded += AssaultSubmissionLoaded;
            m_objectives.Add(m_assaultSubmission);

            m_hackerSubmission = new MyTimedObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_HACKING_Name),
                MyMissionID.TWIN_TOWERS_HACKING,
                (MyTextsWrapperEnum.TWIN_TOWERS_HACKING_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_ASSAULT },
                new TimeSpan(0, 3, 0),
                false
                )
                {
                    SaveOnSuccess = true,
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_0900_hacker_reaches_computer,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1100_hacking_gets_jammed,
                    HudName = MyTextsWrapperEnum.HudHacker
                };
            m_hackerSubmission.OnMissionSuccess += HackerSubmissionSuccess;
            m_hackerSubmission.OnMissionLoaded += HackerSubmissionLoaded;
            m_hackerSubmission.NotificationText = MyTextsWrapperEnum.CountdownHacker;
            m_hackerSubmission.Components.Add(new MyTimedDialogue(TimeSpan.FromSeconds(90), MyDialogueEnum.TWIN_TOWERS_1000_through_the_fight));
            m_objectives.Add(m_hackerSubmission);

            m_jammerSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_JAMMER_Name),
                MyMissionID.TWIN_TOWERS_JAMMER,
                (MyTextsWrapperEnum.TWIN_TOWERS_JAMMER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_HACKING },
                null,
                new List<uint> { (uint) EntityID.Spawnpoint_Jammer }
                )
                {
                    SaveOnSuccess = true,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1200_killing_jammer,
                    HudName = MyTextsWrapperEnum.HudJammer
                };
            m_jammerSubmission.OnMissionSuccess += JammerSubmissionSuccess;
            m_jammerSubmission.OnMissionLoaded += JammerSubmissionLoaded;
            m_objectives.Add(m_jammerSubmission);

            m_hackerSubmission2 = new MyTimedObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_HACKING_CONTINUE_Name),
                MyMissionID.TWIN_TOWERS_HACKING_CONTINUE,
                (MyTextsWrapperEnum.TWIN_TOWERS_HACKING_CONTINUE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_JAMMER },
                new TimeSpan(0, 1, 7)
                )
                {
                    SaveOnSuccess = true,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1300_hacking_done,
                    HudName = MyTextsWrapperEnum.HudHacker
                };
            m_hackerSubmission2.OnMissionSuccess += Hacker2SubmissionSuccess;
            m_hackerSubmission2.OnMissionLoaded += Hacker2SubmissionLoaded;
            m_hackerSubmission2.NotificationText = MyTextsWrapperEnum.CountdownHacker;
            m_objectives.Add(m_hackerSubmission2);

            var railgun01Submission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_RAILGUN1_Name),
                MyMissionID.TWIN_TOWERS_RAILGUN1,
                (MyTextsWrapperEnum.TWIN_TOWERS_RAILGUN1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_HACKING_CONTINUE },
                new List<uint>
                    { (uint) EntityID.Objective_Railgun1_Controls_01, (uint) EntityID.Objective_Railgun1_Controls_02 }
                )
                {
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1400_clearing_first_control_room
                };
            railgun01Submission.OnMissionSuccess += Railgun01SubmissionSuccess;
            m_objectives.Add(railgun01Submission);

            var railgun02Submission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_RAILGUN2_Name),
                MyMissionID.TWIN_TOWERS_RAILGUN2,
                (MyTextsWrapperEnum.TWIN_TOWERS_RAILGUN2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_RAILGUN1 },
                new List<uint>
                    {
                        (uint) EntityID.Objective_Railgun2_Controls_01,
                        (uint) EntityID.Objective_Railgun2_Controls_02,
                        (uint) EntityID.Objective_Railgun2_Controls_03,
                        (uint) EntityID.Objective_Railgun2_Controls_04
                    }
                )
                {
                    SaveOnSuccess = true
                };
            railgun02Submission.OnMissionLoaded += Railgun02SubmissionLoaded;
            railgun02Submission.OnMissionSuccess += Railgun02SubmissionSuccess;
            m_objectives.Add(railgun02Submission);

            var rightBaseGoToSubmission = new MyObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_GOTO_RIGHT_Name),
                MyMissionID.TWIN_TOWERS_GOTO_RIGHT,
                (MyTextsWrapperEnum.TWIN_TOWERS_GOTO_RIGHT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_RAILGUN2 },
                new MyMissionLocation(baseSector, (uint) EntityID.Objective_RightBase)
                )
                {
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_1500_clearing_second_control_room,
                    HudName = MyTextsWrapperEnum.HudTowerB
                };
            rightBaseGoToSubmission.OnMissionSuccess += RightBaseGoToSubmissionSuccess;
            m_objectives.Add(rightBaseGoToSubmission);

            var rightBaseGeneratorSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_GENERATOR_Name),
                MyMissionID.TWIN_TOWERS_GENERATOR,
                (MyTextsWrapperEnum.TWIN_TOWERS_GENERATOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_GOTO_RIGHT },
                new List<uint> { (uint) EntityID.Objective_Generator_Pipe1, (uint) EntityID.Objective_Generator_Pipe2 }
                )
                {
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_1600_in_tower_B,
                    HudName = MyTextsWrapperEnum.HudGenerator
                };
            rightBaseGeneratorSubmission.SaveOnSuccess = true;
            rightBaseGeneratorSubmission.OnMissionLoaded += RightBaseGeneratorSubmissionLoaded;
            rightBaseGeneratorSubmission.OnMissionSuccess += RightBaseGeneratorSubmissionSuccess;
            m_objectives.Add(rightBaseGeneratorSubmission);

            m_rightBaseCommandSubmission = new MyObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_COMMAND_Name),
                MyMissionID.TWIN_TOWERS_COMMAND,
                (MyTextsWrapperEnum.TWIN_TOWERS_COMMAND_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_GENERATOR },
                null,
                new List<uint> { (uint) EntityID.Objective_Command_HUB }
                )
                {
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_1700_reactor_shut_down,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1800_computer_hacked,
                    HudName = MyTextsWrapperEnum.HudCommandCenter
                };
            m_rightBaseCommandSubmission.OnMissionLoaded += RightBaseCommandSubmissionLoaded;
            m_rightBaseCommandSubmission.OnMissionSuccess += RightBaseCommandSubmissionSuccess;
            m_objectives.Add(m_rightBaseCommandSubmission);

            m_waitSubmission = new MyObjective(
                (MyTextsWrapperEnum.TWIN_TOWERS_WAIT_Name),
                MyMissionID.TWIN_TOWERS_WAIT,
                (MyTextsWrapperEnum.TWIN_TOWERS_WAIT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_COMMAND },
                null
                )
                {
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_1900_motherships_arrived
                };
            m_waitSubmission.SaveOnSuccess = true;
            m_waitSubmission.OnMissionLoaded += WaitSubmissionLoaded;
            m_waitSubmission.OnMissionSuccess += WaitSubmissionSuccess;
            m_objectives.Add(m_waitSubmission);

            m_mothership1Submission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_MOTHERSHIP1_Name),
                MyMissionID.TWIN_TOWERS_MOTHERSHIP1,
                (MyTextsWrapperEnum.TWIN_TOWERS_MOTHERSHIP1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_WAIT },
                new List<uint> { (uint)EntityID.Objective_Zeppelin1_Generator }
                )
                {
                    HudName = MyTextsWrapperEnum.HudGenerator
                };
            m_mothership1Submission.SaveOnSuccess = true;
            m_mothership1Submission.Components.Add(new MySpawnpointWaves((uint)EntityID.Detector_Zeppelin_WW1_Hounds, 0, (uint)EntityID.Spawnpoint_Zeppelin_WW1_03_Hounds));
            m_mothership1Submission.OnMissionLoaded += Mothership1SubmissionLoaded;
            m_mothership1Submission.OnMissionSuccess += Mothership1SubmissionSuccess;
            m_mothership1Submission.Components.Add(new MySpawnpointLimiter(m_WWZeppelinSpawns, 10));
            m_objectives.Add(m_mothership1Submission);

            m_mothership1Submission2 = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.TWIN_TOWERS_MOTHERSHIP1_V2_Name),
                MyMissionID.TWIN_TOWERS_MOTHERSHIP1_V2,
                (MyTextsWrapperEnum.TWIN_TOWERS_MOTHERSHIP1_V2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_MOTHERSHIP1 },
                new List<uint>
                    { (uint) EntityID.Objective_Zeppelin1_Battery1, (uint) EntityID.Objective_Zeppelin1_Battery2 }
                )
                {
                    StartDialogId = MyDialogueEnum.TWIN_TOWERS_2000_destroying_the_generator,
                    SuccessDialogId = MyDialogueEnum.TWIN_TOWERS_2100_destroying_batteries,
                    HudName = MyTextsWrapperEnum.HudBatteries
                };
            m_mothership1Submission2.Components.Add(new MySpawnpointWaves((uint)EntityID.Detector_Zeppelin_WW1_Hounds, 0, (uint)EntityID.Spawnpoint_Zeppelin_WW1_03_Hounds));
            m_mothership1Submission2.OnMissionLoaded += Mothership1Submission2Loaded;
            m_mothership1Submission2.OnMissionSuccess += Mothership1Submission2Success;
            m_mothership1Submission2.Components.Add(new MySpawnpointLimiter(m_WWZeppelinSpawns, 10));
            m_objectives.Add(m_mothership1Submission2);
            /*
            var mothership2Submission = new MyObjective(
                new StringBuilder("Destroy the second mothership"),
                MyMissionID.TWIN_TOWERS_MOTHERSHIP2,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_MOTHERSHIP1_V2 },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective_Zeppelin2_Command)
            );
            mothership2Submission.OnMissionLoaded += Mothership2SubmissionLoaded;
            mothership2Submission.OnMissionSuccess += Mothership2SubmissionSuccess;
            m_objectives.Add(mothership2Submission);
            */  /*
            var returnSubmission = new MyObjective(
                new StringBuilder("Return to Madelyn"),
                MyMissionID.TWIN_TOWERS_GOTO_MADELYN,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.TWIN_TOWERS_MOTHERSHIP1_V2 },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR)
            );
            m_objectives.Add(returnSubmission);*/
        }

        #endregion

        #region Load/Unload
        public override void Accept()
        {
            base.Accept();

            MyScriptWrapper.AddHackingToolToPlayersInventory(5);
        }

        public override void Load()
        {
            m_shootWarningTime = null;

            /*
            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_HACKING_CONTINUE))
            {
                for (int i = 1; i < m_friends.Count; i++)
                {
                    m_friends[i].Follow(m_friends[0]);
                }
                m_friends[0].SetWaypointPath("friendPatrol");
                m_friends[0].PatrolMode = MyPatrolMode.CYCLE;
                m_friends[0].Patrol();
            }
            */

            // audio
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Mystery);

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            MyScriptWrapper.OnEntityAtacked += OnEntityAtacked;
            MyScriptWrapper.OnBotReachedWaypoint += OnBotReachedWaypoint;
            MyScriptWrapper.EntityDeath += OnEntityDeath;
            MyScriptWrapper.EntityHacked += OnEntityHacked;

            MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnDialogueFinished;

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);

            // deactivate bots in hangar
            foreach (var botID in m_botsInLeftHangars)
            {
                MyEntity bot = MyScriptWrapper.TryGetEntity(botID);
                if (bot != null)
                {
                    MyScriptWrapper.DisableShip(MyScriptWrapper.TryGetEntity(botID));
                }
            }

            foreach (var botID in m_botsInRightHangars)
            {
                MyEntity bot = MyScriptWrapper.TryGetEntity(botID);
                if (bot != null)
                {
                    MyScriptWrapper.DisableShip(MyScriptWrapper.TryGetEntity(botID));
                }
            }

            // hide motherships
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_MOTHERSHIP2))
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity("Madelyn"));
            }

            // get hacker
            m_hacker = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Friend_Hacker) as MySmallShipBot;
            MyScriptWrapper.SetEntityDestructible(m_hacker, false);

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_RANDEVOUZ))
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.ReichMothership));
            }
            else
            {
                m_explosives = 3;
                m_explosives01 = MyScriptWrapper.TryGetEntity((uint)EntityID.Explosives1);
                HideExplosives(m_explosives01);

                m_explosives02 = MyScriptWrapper.TryGetEntity((uint)EntityID.Explosives2);
                HideExplosives(m_explosives02);

                m_explosives03 = MyScriptWrapper.TryGetEntity((uint)EntityID.Explosives3);
                HideExplosives(m_explosives03);

                m_explosives04 = MyScriptWrapper.TryGetEntity((uint)EntityID.Explosives4);
                HideExplosives(m_explosives04);
            }

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_ASSAULT))
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Rainiers, -100);
                if (!MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_HACKING_CONTINUE))
                {
                    foreach (var value in m_turrets1)
                    {
                        MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(value), false);
                    }
                }
                else
                {
                    MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Door_LeftCommand1), true);
                    MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Door_LeftCommand2), true);
                }
            }
            else
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Rainiers, 100);
            }

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_HACKING_CONTINUE))
            {
                ChangeFaction_Phase01();
            }

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_GENERATOR))
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Objective_Generator), false);
            }

            m_FoRMS = MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_FoR_1);
            m_WWMS1_Moving = MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_WW_1_Moving);
            m_WWMS1_Static = MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_WW_1_Static);
            m_WWMS2 = MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_WW_2);
            
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_COMMAND))
            {
                MyScriptWrapper.HideEntity(m_WWMS1_Moving, true);
                MyScriptWrapper.HideEntity(m_WWMS1_Static, true);
                MyScriptWrapper.HideEntity(m_WWMS2, true);
                MyScriptWrapper.HideEntity(m_FoRMS, true);
            }
            else
            {
                ChangeFaction_Phase02();
            }

            m_flyWWMS1 = false;
            m_flyWWMS2 = false;
            m_flyFoRMS = false;
            m_WWMS1_passedmark = false;
            m_WWMS2_passedmark = false;
            m_FoRMS_passedmark = false;

            base.Load();

            // To be removed later
            // inventory items
            /*
            MyMwcObjectBuilder_SmallShip_Player originalBuilder = MySession.PlayerShip.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player;
            originalBuilder.ShipType = MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER;
            MySession.PlayerShip.Close();
            MyEntities.CreateFromObjectBuilderAndAdd(null, originalBuilder, originalBuilder.PositionAndOrientation.GetMatrix());
             */

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToDisable_01),true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToDisable_02),true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToEnable_01),false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToEnable_02),false);
        }

        void MyScriptWrapperOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            switch (dialogue)
            {
                case MyDialogueEnum.TWIN_TOWERS_0100_INTRO:
                    m_introTalk.Success();
                    break;
            }
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnDialogueFinished;

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.OnEntityAtacked -= OnEntityAtacked;
            MyScriptWrapper.OnBotReachedWaypoint -= OnBotReachedWaypoint;
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
            MyScriptWrapper.EntityHacked -= OnEntityHacked;

            m_hacker = null;
            m_explosives01 = null;
            m_explosives02 = null;
            m_explosives03 = null;
            m_explosives04 = null;
            m_FoRMS = null;
            m_WWMS1_Moving = null;
            m_WWMS1_Static = null;
            m_WWMS2 = null;
        }

        public override void Update()
        {
            if (!IsMainSector) return;

            base.Update();

            if (m_flyWWMS1)
            {
                if (m_WWMS1_passedmark)
                {
                    if (MoveMotherShipForward(m_WWMS1_Moving, 300f, m_WW1mothershipPosition))
                    {
                        m_flyWWMS1 = false;
                        m_waitSubmission.Success();
                        MyScriptWrapper.EnablePhysics((uint)EntityID.Zeppelin_WW_1_Static, true);
                        MyScriptWrapper.UnhideEntity(m_WWMS1_Static);
                        MyScriptWrapper.HideEntity(m_WWMS1_Moving);
                    }
                }
                else
                {
                    if (MoveMotherShipForward(m_WWMS1_Moving, 1000f, m_WW1mothershipPositionHalf))
                    {
                        m_WWMS1_passedmark = true;
                    }
                }
            }
            if (m_flyWWMS2)
            {
                if (MoveMotherShipForward(m_WWMS2, 1000f, m_WW2mothershipPosition))
                {
                    m_flyWWMS2 = false;
                    MyScriptWrapper.ReturnMotherShipFromMove(m_WWMS2);
                }
            }
            if (m_flyFoRMS)
            {
                if (m_FoRMS_passedmark)
                {
                    if (MoveMotherShipForward(m_FoRMS, 300f, m_FoRmothershipPosition))
                    {
                        m_flyFoRMS = false;
                        //MyScriptWrapper.EnablePhysics((uint)EntityID.Zeppelin_FoR_1, true);
                        MyScriptWrapper.ReturnMotherShipFromMove(m_FoRMS);
                        m_flyWWMS1 = true;
                        //m_flyWWMS2 = true;
                    }
                }
                else
                {
                    if (MoveMotherShipForward(m_FoRMS, 1000f, m_FoRmothershipPositionHalf))
                    {
                        m_FoRMS_passedmark = true;
                    }
                }
                
            }
        }

        #endregion

        #region Event handlers
        void OnBotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (bot == m_hacker && waypoint.EntityId != null && waypoint.EntityId.Value.NumericValue == (uint)EntityID.Waypoint_hacker_final)
            {
                m_hacker.LookTarget = MyScriptWrapper.TryGetEntity((uint)EntityID.Hacker_hub);
            }
        }

        bool KillableWithoutAlarm(MyEntity entity)
        {
            var bot = entity as MySmallShipBot;
            foreach (var spBot in MyScriptWrapper.GetSpawnPointBots((uint)EntityID.Spawnpoint_Command))  // bots in command center are killable without alarm
            {
                if (spBot.Ship == bot) return true;
            }
            return false;
        }

        void DisplayWarningOrStartAlarm()
        {
            if (m_shootWarningTime == null)
            {
                m_shootWarningTime = MyScriptWrapper.GetGameTime();
                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.DontShoot, MyHudConstants.ENEMY_FONT, 10000));
                return;
            }
            if (MyScriptWrapper.GetGameTime() - m_shootWarningTime.Value < 2000)  // two-second grace period
            {
                return;
            }
            ChangeToEnemy();
            SpawnBotsOnAlarm();
        }

        void OnEntityDeath(MyEntity entity, MyEntity killedBy)
        {
            if ((entity == m_hacker) && (!MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_HACKING) || !MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_HACKING_CONTINUE)))
            {
                Fail(MyTextsWrapperEnum.Fail_HackerKilled);
            }

            if (MySession.IsPlayerShip(killedBy) && !MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_ASSAULT) && !KillableWithoutAlarm(entity))
            {
                DisplayWarningOrStartAlarm();
            }
        }


        private void SpawnBotsOnAlarm()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_left_down);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_left_middle);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_left_up);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_right_down);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_right_up);
        }

        void OnEntityAtacked(MyEntity attacker, MyEntity target)
        {
            if (MySession.IsPlayerShip(attacker) && !MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_ASSAULT) && !KillableWithoutAlarm(target))
            {
                if (target is MySmallShipBot && target.Faction == MyMwcObjectBuilder_FactionEnum.WhiteWolves)
                {
                    DisplayWarningOrStartAlarm();
                }
            }
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            switch (MyScriptWrapper.GetEntityId(spawnpoint))
            {
                case (uint)EntityID.Spawnpoint_Zeppelin_WW1_03_Hounds:
                    ((MySmallShipBot)bot).Attack(MySession.PlayerShip);
                    break;
                case (uint)EntityID.Spawnpoint_Jammer:
                    m_jammer = bot;
                    MyScriptWrapper.MarkEntity(bot, MyTexts.Jammer);
                    // stop hacker mission and play jammer mission
                    m_hackerSubmission.Success();
                    break;
                case (uint)EntityID.Spawnpoint_Left_Assault_01:
                    ((MySmallShipBot)bot).Attack(m_hacker);
                    break;
            }
        }



        void OnEntityHacked(MyEntity entity)
        {
            if (entity == MyScriptWrapper.GetEntity((uint)EntityID.Objective_Command_HUB))
            {
                m_rightBaseCommandSubmission.Success();
            }
        }
        #endregion

        #region MissionSuccess

        void MultipleSabotageOnOnObjectUsedSucces(uint entityID)
        {
            switch (entityID)
            {
                case (uint)EntityID.Explosives1:
                    PlaceExplosives(m_explosives01);
                    break;
                case (uint)EntityID.Explosives2:
                    PlaceExplosives(m_explosives02);
                    break;
                case (uint)EntityID.Explosives3:
                    PlaceExplosives(m_explosives03);
                    break;
            }
        }

        private void RandevouzSubmissionLoaded(MyMissionBase sender)
        {
            
        }

        private void RandevouzSubmissionSuccess(MyMissionBase sender)
        {
            //m_hacker.Follow(MySession.PlayerShip);
        }

        private void AssaultSubmissionLoaded(MyMissionBase sender)
        {
            m_commandDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Command));
            m_commandDetector.OnEntityEnter += CommandReached;
            m_commandDetector.On();

            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.ReichMothership));
        }

        private void AssaultSubmissionSuccess(MyMissionBase sender)
        {
            ChangeToEnemy();
        }

        private void HackerSubmissionLoaded(MyMissionBase sender)
        {
            m_hacker.PatrolMode = MyPatrolMode.ONE_WAY;
            m_hacker.SetWaypointPath("HackerRoute1");
            MyScriptWrapper.SetEntityDestructible(m_hacker, true);
            m_hacker.Patrol();
            m_hacker.MaxHealth = 500;
            m_hacker.Health = 500;

            MyScriptWrapper.SetEntityPriority(m_hacker, MySession.PlayerShip.AIPriority *6);

            MyEntityDetector securityDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Hacker));
            securityDetector.OnEntityEnter += SecurityDetectorReached;
            securityDetector.On();
            
            m_hackerSubmission.MissionEntityIDs.Add(MyScriptWrapper.GetEntityId(m_hacker));

            MissionTimer.RegisterTimerAction(3000, AssaultSubmissionWave01, false);
            MissionTimer.RegisterTimerAction(55000, AssaultSubmissionWave02, false);
            MissionTimer.RegisterTimerAction(113000, AssaultSubmissionWave03, false);

            foreach (var value in m_turrets1)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(value), false);
            }

            ExplodeExplosives();
        }

        private void HackerSubmissionSuccess(MyMissionBase sender)
        {
            
        }

        private void JammerSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_04);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_03);
        }

        private void JammerSubmissionSuccess(MyMissionBase sender)
        {
           // m_hackerSubmission.Suspend(true);
        }

        private void Hacker2SubmissionLoaded(MyMissionBase sender)
        {
            m_hackerSubmission2.SubmissionDuration = m_hackerSubmission.RemainingTime;
            m_hackerSubmission.HideNotification();

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_LastWave);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_04);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_03);

            m_hackerSubmission2.MissionEntityIDs.Add(MyScriptWrapper.GetEntityId(m_hacker));
        }

        private void Hacker2SubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_01);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_02);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_03);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_04);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_05);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_LastWave);

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Railgun_Room_01);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Railgun_Room_02);
            ChangeFaction_Phase01();

            foreach (var value in m_turrets1)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(value), true);
            }

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Door_LeftCommand1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Door_LeftCommand2), true);
        }

        private void Railgun01SubmissionSuccess(MyMissionBase sender)
        {
            
        }

        private void Railgun02SubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_01);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_02); // dead
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_03);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_04); // dead
            /*
            MyEntityDetector friendsDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Friends));
            friendsDetector.OnEntityEnter += FriendsToHangar;
            friendsDetector.On();
            */
        }

        private void Railgun02SubmissionSuccess(MyMissionBase sender)
        {
            /*
            MyEntityDetector friendsDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Friends));
            friendsDetector.OnEntityEnter -= FriendsToHangar;
            friendsDetector.Off();
            */
        }

        private void RightBaseGoToSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_01);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Assault_02);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Upper);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Lower);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Right_Command);
        }

        private void RightBaseGoToSubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Objective_Generator), false);
        }

        private void RightBaseGeneratorSubmissionLoaded(MyMissionBase sender)
        {

        }

        private void RightBaseGeneratorSubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Generator_ToDisable),false);

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToDisable_01),false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToDisable_02),false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToEnable_01),true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Light_ToEnable_02),true);


        }

        private void RightBaseCommandSubmissionLoaded(MyMissionBase sender)
        {
            
        }

        private void RightBaseCommandSubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.UnhideEntity(m_WWMS1_Moving, false);
            MyScriptWrapper.UnhideEntity(m_WWMS2, false);
            MyScriptWrapper.UnhideEntity(m_FoRMS, false);
            ChangeFaction_Phase02();
        }

        private void WaitSubmissionLoaded(MyMissionBase sender)
        {
            m_flyFoRMS = true;
            //m_flyWWMS1 = true;
            //m_flyWWMS2 = true;
            MyScriptWrapper.PrepareMotherShipForMove(MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_WW_1_Moving));
            MyScriptWrapper.PrepareMotherShipForMove(MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_WW_2));
            MyScriptWrapper.PrepareMotherShipForMove(MyScriptWrapper.GetEntity((uint)EntityID.Zeppelin_FoR_1));
        }

        private void WaitSubmissionSuccess(MyMissionBase sender)
        {
            
        }

        private void Mothership1SubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_FoR_01);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_FoR_02);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_WW1_01);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_WW1_02);
        }

        private void Mothership1SubmissionSuccess(MyMissionBase sender)
        {

        }

        private void Mothership1Submission2Loaded(MyMissionBase sender)
        {
            
        }

        private void Mothership1Submission2Success(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Objective_Zeppelin1_GeneratorWithoutBatteries), false);
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity("Madelyn"));
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_WW1_01);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Spawnpoint_Zeppelin_WW1_02);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_End_friends1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_point_End_friends2);
        }

        private void Mothership2SubmissionLoaded(MyMissionBase sender)
        {

        }

        private void Mothership2SubmissionSuccess(MyMissionBase sender)
        {
            
        }


        #endregion


        private void CommandReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if ((entity == m_hacker) || (MySession.IsPlayerShip(entity)))
            {
                ChangeToEnemy();
            }
        }

        private void ChangeToEnemy()
        {
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyFactions.RELATION_WORST);

            MyScriptWrapper.ChangeFaction(m_hacker, MyMwcObjectBuilder_FactionEnum.FourthReich);

            if (MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_RANDEVOUZ))
            {
                m_commandDetector.Off();
            }

            //prefab_base_Left, prefab_base_Right, prefab_base_Bridge, prefab_base_Construction

            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Left_01,true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Left_02, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Left_03, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Right_01, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Right_02, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Right_03, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Bridge, true);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.Prefab_Base_Construction, true);
        }

        void SecurityDetectorReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == m_hacker)
            {
                m_hackerSubmission.Suspend(true);
                sender.Off();
            }
        }
        
        private void AssaultSubmissionWave01()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_01);
            var bot1 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_01_01); if (bot1 != null) bot1.MarkForClose();
            var bot2 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_01_02); if (bot2 != null) bot2.MarkForClose();
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_02);
            var bot3 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_02_01); if (bot3 != null) bot3.MarkForClose();
            var bot4 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_02_02); if (bot4 != null) bot4.MarkForClose();
            var bot5 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_02_03); if (bot5 != null) bot5.MarkForClose();
        }

        private void AssaultSubmissionWave02()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_04);
            var bot1 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_04_01); if (bot1 != null) bot1.MarkForClose();
            var bot2 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_04_02); if (bot2 != null) bot2.MarkForClose();
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_05);
            var bot3 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_05_01); if (bot3 != null) bot3.MarkForClose();
            var bot4 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_05_02); if (bot4 != null) bot4.MarkForClose();
            var bot5 = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Left_05_03); if (bot5 != null) bot5.MarkForClose();
        }

        private void AssaultSubmissionWave03()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Jammer);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Jammer_Guards);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint_Left_Assault_03);
            //MyScriptWrapper.GetEntity((uint)EntityID.Bot_Left_03_01).Close();
            //MyScriptWrapper.GetEntity((uint)EntityID.Bot_Left_03_02).Close();
        }


        private void ChangeFaction_Phase01()
        {
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Left_01,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Left_02,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Left_03,  MyMwcObjectBuilder_FactionEnum.FourthReich);
        }

        private void ChangeFaction_Phase02()
        {
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Right_01,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Right_02,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Right_03,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Construction,  MyMwcObjectBuilder_FactionEnum.FourthReich);
            MyScriptWrapper.ChangeFaction((uint)EntityID.Prefab_Base_Bridge,  MyMwcObjectBuilder_FactionEnum.FourthReich);
        }

        private void HideExplosives(MyEntity explosivesContainer)
        {
            //MyScriptWrapper.HideEntity(explosivesContainer);
            MyScriptWrapper.RemoveEntityMark(explosivesContainer);
            if (explosivesContainer.EntityId != null)
            {
                MyScriptWrapper.Highlight(explosivesContainer.EntityId.Value.NumericValue,true,this);
            }
        }

        private bool PlaceExplosives(MyEntity explosivesContainer)
        {
            if (m_explosives > 0)
            {
                MyScriptWrapper.UnhideEntity(explosivesContainer);
                if (explosivesContainer.EntityId != null)
                {
                    MyScriptWrapper.Highlight(explosivesContainer.EntityId.Value.NumericValue, false,this);
                }
                explosivesContainer.DisplayName = MyTexts.Explosives;
                --m_explosives;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExplodeExplosives()
        {
            MyScriptWrapper.DestroyEntities(new List<uint>()
            {
                (uint)EntityID.Explosives1,
                (uint)EntityID.Explosives2,
                (uint)EntityID.Explosives3,
                (uint)EntityID.Explosives4,
                (uint)EntityID.Prefab_Sabotage1_ToDestroy1,
                (uint)EntityID.Prefab_Sabotage2_ToDestroy1,
                (uint)EntityID.Prefab_Sabotage2_ToDestroy2,
                (uint)EntityID.Prefab_Sabotage2_ToDestroy3,
                (uint)EntityID.Prefab_Sabotage3_ToDestroy1,
                (uint)EntityID.Prefab_Sabotage3_ToDestroy2,
                (uint)EntityID.Prefab_Sabotage3_ToDestroy3,
                (uint)EntityID.Prefab_Sabotage3_ToDestroy4,
                (uint)EntityID.Prefab_Sabotage4_ToDestroy1,
                (uint)EntityID.Prefab_Sabotage4_ToDestroy2,
                (uint)EntityID.Prefab_Sabotage4_ToDestroy3,
                (uint)EntityID.Prefab_Sabotage4_ToDestroy4,
            });

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Dummy_Sabotage1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Dummy_Sabotage3), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Dummy_Sabotage4), true);

            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Rubble_01));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Rubble_02));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Rubble_03));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Prefab_Rubble_04));
        }

        bool MoveMotherShipForward(MyEntity entity, float speed, Vector3 destination)
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            if (Vector3.DistanceSquared(destination, entity.GetPosition()) > 10 * 10)
            {
                entity.SetPosition(entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS); // recalculate position
                return false;
            }
            return true;
        }
    }
}

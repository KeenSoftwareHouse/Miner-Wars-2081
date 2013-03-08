using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MySlaverBaseMission : MyMission
    {
        #region EntityIDs

        List<List<uint>> m_ValidateIDlists;

        enum EntityID
        {
            StartLocation = 4837,
            Navigation_Dummy = 23559,
            Navigation_Dummy_2 = 24104,
            Prison_Dummy = 4838,
            Bars_1 = 849,
            Bars_2 = 560,
            Bars_3 = 1319,
            Bars_4 = 434,
            Bars_5 = 705,
            Pit_Dummy = 6501,
            Escape_Dummy = 20321,
            Generator_Spawn_1 = 4966,
            Generator_Spawn_2 = 4946,
            Prison_Spawn_1 = 4947,
            Prison_Spawn_2 = 23026,
            Prison_Spawm_3 = 23027,

            Batteries_Spawn = 26257,
            Mothership_Spawn_1 = 55202,
            Mothership_Spawn_2 = 55741,
            Mothership_Spawn_3 = 55740,

            Battery_1 = 25723,
            Battery_2 = 25724,
            Transmitter = 641,
            //Light_1 = 7883,
            //Light_2 = 7885,
            //Light_3 = 8345,
            PC_1 = 28173,
            //  PC_2 = 28174,
            PC_3 = 28175,
            PC_4 = 28176,

            PC_6 = 28178,
            PC_7 = 28140,
            PC_8 = 28141,
            // PC_9 = 28142,
            PC_10 = 28143,
            PC_11 = 28138,

            PC_13 = 28160,
            PC_14 = 28161,
            //     PC_15 = 28162,
            PC_16 = 28159,
            //   PC_17 = 28158,
            PC_18 = 28157,
            PC_19 = 31380,
            //   PC_20 = 31381,
            //   PC_21 = 31382,
            PC_22 = 31383,
            //  PC_23 = 31384,
            PC_24 = 31385,
            //  PC_25 = 31386,
            PC_26 = 31387,
            //  PC_27 = 31390,
            PC_28 = 31391,
            //  PC_29 = 31388,
            PC_30 = 31389,
            PC_31 = 31392,
            PC_32 = 31393,
            // PC_33 = 31362,
            PC_34 = 31363,
            // PC_35 = 31364,
            PC_36 = 31365,
            // PC_37 = 31366,
            PC_38 = 31370,
            // PC_39 = 31371,
            PC_40 = 31372,
            // PC_41 = 31367,
            PC_42 = 31368,
            // PC_43 = 31369,
            PC_44 = 28122,
            PC_45 = 28123,
            PC_46 = 31414,
            PC_47 = 28124,
            PC_48 = 31415,
            PC_49 = 32585,
            PC_50 = 32587,
            PC_51 = 32588,
            PC_52 = 32586,
            PC_53 = 52099,
            Hack_PC = 32591,
            Hide_Doors_1 = 35740,
            Hide_Doors_2 = 5709,
            Hide_Doors_3 = 5707,
            Hide_Doors_4 = 5708,
            Hidden_Doors_1 = 35741,
            Hidden_Doors_2 = 35742,
            Hidden_Doors_3 = 35743,
            HangarDoorsHub = 50917,
            Fake_escape = 38219,
            CIC_Spawn = 38231,
            Navigation = 40615,
            Generator_1 = 11489,
            Generator_2 = 12017,
            Mothership_Bars_1 = 42386,
            Mothership_Bars_2 = 42389,
            Mothership_Bars_3 = 42387,
            Mothership_Bars_4 = 42388,
            Bar_pit = 3604,
            Mothership_container = 264,
            Cargo_Bay_Doors = 292,
            Hangar_Doors = 298,
            Endless_spawn = 54560,
        }

        List<uint> m_closeddoors = new List<uint>
            {
                456,
                787,
                921,
                373,
                506,
                1654,
                779,
                1337,
                328,
                1024,
                1025,
                1590,
                623,
                1778,
                1063,
                1473,
                458,
                1412,
                1125,
                624,
                1029
            };

        List<uint> m_finalparticle = new List<uint>
            {
                48578,
                48597,
                48610,
                48613,
                48615,
                48617,
                48620,
                48622,
                48624,
                48626
            };

        List<uint> m_standbyparticle = new List<uint>
            {
                15716,
                15717,
                15427,
                15718,
                15719,
                15153,
                15151,
                15150,
                15149,
                14712
            };

        List<uint> m_openedddoors = new List<uint>
            {
                48648,
                48649,
                48652,
                48651,
                48650,
                48657,
                48653,
                48646,
                48645,
                48642,
                48641,
                48638,
                48640,
                48643,
                48644,
                48647,
                48639,
                16777819,
                16777817,
                16777816
            };

        List<uint> m_pilotsSpawn = new List<uint>
            {
                12785,
                12796,
                12797,
                12794,
                /* 12798*/ 12795
            };

        List<uint> m_closeentiites = new List<uint>
            {
                28090,
                28089,
                28088,
                28092,
                28100,
                28096,
                28094,
                28103,
                37481,
                28105,
                38173,
                38174,
                28114,
                28116,
                31402,
                31404,
                43556,
                43555,
                28172,
                28137,
                28156,
                31993,
                31994,
                32001,
                31374,
                31996,
                32002,
                32003,
                31998,
                31997,
                32004,
                32005,
                32000,
                32585,
                32586,
                32588,
                32587,
                32008,
                38172,
                32009,
                38171,
                //28120,
                28163,
                31341,
                31373,
                31995,
                31340,
                31339,
                31375,
                31376,
                31335,
                31334,
                31377,
                31378,
                31333,
                28183,
                28184,
                31343,
                31355,
                31356,
                31342,
                31357,
                31345,
                31346,
                31358,
                31347,
                31360,
                //31348,
                //31359,
                38176,
                31416,
                31417,
                31419,
                31417,
                31419,
                28083,
                28082,
                28075,
                28073,
                28070,
                28069,
                28068,
                28063,
                28071,
                28057,
                28053,
                28056,
                28052,
                28046,
                28178,
                40020,
                38180,
                38179,
                28031,
                28043,
                49242,
                38225,
                38224,
                18251,
                6182,
                6183,
                6184,
                6185,
                6186,
                6188,
                6187,
                6190,
                6189,
                6192,
                6191,
                6152,
                6151,
                6153,
                6154,
                6155,
                6156,
                6157,
                6158,
                6159,
                6160,
                6162,
                6161,
                6167,
                6168,
                6165,
                6166,
                6163,
                6164,
                5804,
                5803,
                5806,
                5805,
                5808,
                5807,
                5801,
                5802,
                5799,
                5800,
                5797,
                5798,
                5795,
                5796,
                5794,
                5793,
                5791,
                5792,
                6067,
                6068,
                6070,
                6071,
                6072,
                6066,
                6065,
                6064,
                6063,
                6062,
                6061,
                42389,
                42386,
                42388,
                42387,
                7712,
                41211,
                7709
            };

        List<uint> m_doors = new List<uint>
            {
                49801,
                49799,
                49800
            };

        List<uint> m_turrets = new List<uint>
            {
                1917,
                1916,
                1914,
                1915,
                1913
            };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof (EntityID)))
            {
                MyScriptWrapper.GetEntity((uint) ((value as EntityID?).Value));
            }
            foreach (
                List<uint> list in
                    new List<List<uint>>
                        {
                            m_closeddoors,
                            m_finalparticle,
                            m_standbyparticle,
                            m_openedddoors,
                            m_pilotsSpawn,
                            m_closeentiites,
                            m_doors
                        })
            {
                foreach (var item in list)
                {
                    MyScriptWrapper.GetEntity(item);
                }
            }

        }

        List<uint> m_PC = new List<uint>
            {
                (uint) EntityID.PC_1,
                (uint) EntityID.PC_3,
                (uint) EntityID.PC_4,
                (uint) EntityID.PC_6,
                (uint) EntityID.PC_7,
                (uint) EntityID.PC_8,
                (uint) EntityID.PC_10,
                (uint) EntityID.PC_11,
                (uint) EntityID.PC_13,
                (uint) EntityID.PC_14,
                (uint) EntityID.PC_18,
                (uint) EntityID.PC_19,
                (uint) EntityID.PC_22,
                (uint) EntityID.PC_24,
                (uint) EntityID.PC_26,
                (uint) EntityID.PC_28,
                (uint) EntityID.PC_30,
                (uint) EntityID.PC_31,
                (uint) EntityID.PC_32,
                (uint) EntityID.PC_34,
                (uint) EntityID.PC_36,
                (uint) EntityID.PC_38,
                (uint) EntityID.PC_40,
                (uint) EntityID.PC_42,
                (uint) EntityID.PC_49,
                (uint) EntityID.PC_50,
                (uint) EntityID.PC_44,
                (uint) EntityID.PC_45,
                (uint) EntityID.PC_46,
                (uint) EntityID.PC_51,
                (uint) EntityID.PC_52,
                (uint) EntityID.PC_47,
                (uint) EntityID.PC_48,
                (uint) EntityID.PC_53,
                (uint) EntityID.PC_16
            };

        List<uint> m_Bars = new List<uint>
            {
                (uint) EntityID.Bars_1,
                (uint) EntityID.Bars_2,
                (uint) EntityID.Bars_3,
                (uint) EntityID.Bars_4,
                (uint) EntityID.Bars_5
            };

        List<uint> TurnOff = new List<uint>
            {
                (uint) EntityID.Transmitter,
                //(uint) EntityID.Light_1,
                //(uint) EntityID.Light_2,
                //(uint) EntityID.Light_3
            };

        List<uint> Batteries = new List<uint>
            {
                (uint) EntityID.Battery_1,
                (uint) EntityID.Battery_2,
            };

        List<uint> Generators = new List<uint>
            {
                (uint) EntityID.Generator_1,
                (uint) EntityID.Generator_2,
            };

        List<uint> Mothership_Bars = new List<uint>
            {
                (uint) EntityID.Mothership_Bars_1,
                (uint) EntityID.Mothership_Bars_2,
                (uint) EntityID.Mothership_Bars_3,
                (uint) EntityID.Mothership_Bars_4
            };

        List<uint> bar_pit = new List<uint>
            {
                (uint) EntityID.Bar_pit
            };

        #endregion

        #region Objectives

        MyObjectiveDestroy m_freeSlaves;
        MyObjectiveDestroy m_freeSlavesMS;
        MyObjective m_hackTransmitter;
        MyObjective m_hackHangar;
        MyObjective m_openHangarDoors;
        //private MyObjective m_returnToMothership;

        #endregion

        #region Fields

        MyEntity m_fourthReichMS;
        Vector3 m_fourthReichMS_UpDestination = new Vector3(315.974f, 11599.000f, 7698.000f);
        Vector3 m_fourthReichMS_BackDestination = new Vector3(315.974f, 11599.000f, 9780.000f);
        Vector3 m_fourthReichMS_Up2Destination = new Vector3(315.974f, 13500.000f, 9780.000f);
        bool m_moveMS_Up;
        bool m_moveMS_Back;
        bool m_moveMS_Up2;
        bool m_moveMS_Out;
        float m_moveMS_Speed;
        int m_moveMS_AccelerateCounter;
        //private MySubmisisonDefend m_defendGenerator;

        #endregion

        public MySlaverBaseMission()
        {
            m_ValidateIDlists = new List<List<uint>>();
            m_ValidateIDlists.Add(m_closeddoors);
            m_ValidateIDlists.Add(m_finalparticle);
            m_ValidateIDlists.Add(m_openedddoors);
            m_ValidateIDlists.Add(m_standbyparticle);
            m_ValidateIDlists.Add(m_pilotsSpawn);
            m_ValidateIDlists.Add(m_closeentiites);
            ID = MyMissionID.SLAVER_BASE_1; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.SLAVER_BASE_GRAND_BURROW;
            Description = MyTextsWrapperEnum.SLAVER_BASE_GRAND_BURROW_Description;
            DebugName = new StringBuilder("09b-Slaver base Grand Burrow"); // Nazev mise

            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2052452, 0, -10533886); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { MyMissionID.FORT_VALIANT }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_10 };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


            // -------------- START DEFINICE SUBMISE ---------------------

            var talkWith = new MyObjectiveDialog(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_1_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_1,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_1_Description),
                null,
                this,
                new MyMissionID[] { },
                dialogId: MyDialogueEnum.SLAVERBASE_0100_INTRODUCE
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith);


            var DestroyGenerators = new MyObjectiveDestroy(
                // Var is used to call functions on that member
                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_GENERATOR_Name),
                MyMissionID.SLAVER_BASE_1_DESTROY_GENERATOR,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_GENERATOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_1 },
                Generators,
                false
                ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudGenerator };
            m_objectives.Add(DestroyGenerators); // pridani do seznamu submisi
            DestroyGenerators.OnMissionSuccess += DestroyGeneratorMSSuccess;


            var talkWith2 = new MyObjectiveDialog(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_2_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_2,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DESTROY_GENERATOR },
                dialogId: MyDialogueEnum.SLAVERBASE_0200_GENERATORS_DESTROYED
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith2);


            var DestroyBatteries = new MyObjectiveDestroy(
                // Var is used to call functions on that member

                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_BATTERIES_Name),
                MyMissionID.SLAVER_BASE_1_DESTROY_BATTERIES,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_BATTERIES_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_2 },
                Batteries,
                false
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudBatteries };
            m_objectives.Add(DestroyBatteries); // pridani do seznamu submisi
            DestroyBatteries.OnMissionSuccess += DestroyBatteriesMSSuccess;


            var FindPrison = new MyObjective( // One member of that list - je to MyObjective takze cilem je doletet do checkpointu
                (MyTextsWrapperEnum.SLAVER_BASE_1_FIND_PRISON_Name), // nazev submise
                MyMissionID.SLAVER_BASE_1_FIND_PRISON, // id submise
                (MyTextsWrapperEnum.SLAVER_BASE_1_FIND_PRISON_Description), // popis submise
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DESTROY_BATTERIES }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                new MyMissionLocation(baseSector, (uint)EntityID.Prison_Dummy),
                startDialogId: MyDialogueEnum.SLAVERBASE_0300_BATTERIES_DESTROYED// ID of dummy point of checkpoint
                ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudPrison }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(FindPrison); // pridani do seznamu submisi
            FindPrison.OnMissionSuccess += FindPrisonMSSuccess;


            m_freeSlaves = new MyObjectiveDestroy(
                // Var is used to call functions on that member
                (MyTextsWrapperEnum.SLAVER_BASE_1_FREE_SLAVES_Name),
                MyMissionID.SLAVER_BASE_1_FREE_SLAVES,
                (MyTextsWrapperEnum.SLAVER_BASE_1_FREE_SLAVES_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_FIND_PRISON },
                m_Bars,
                false
                ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudCage };
            m_objectives.Add(m_freeSlaves); // pridani do seznamu submisi


            var talkWith4 = new MyObjectiveDialog(
                MyMissionID.SLAVER_BASE_1_DIALOG_4,
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_FREE_SLAVES },
                dialogId: MyDialogueEnum.SLAVERBASE_0400_SLAVES_SAVED
                ) { SaveOnSuccess = true };
            m_objectives.Add(talkWith4);


            m_hackHangar = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.SLAVER_BASE_1_HACK_HANGAR_Name),
                MyMissionID.SLAVER_BASE_1_HACK_HANGAR,
                (MyTextsWrapperEnum.SLAVER_BASE_1_HACK_HANGAR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_4 },
                null,
                new List<uint> { (uint)EntityID.Hack_PC },
                new List<uint> { (uint)EntityID.Hangar_Doors }
                ) { HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(m_hackHangar);
            m_hackHangar.OnMissionSuccess += HangarOpen;

            m_freeSlavesMS = new MyObjectiveDestroy( // Var is used to call functions on that member
               (MyTextsWrapperEnum.SLAVER_BASE_1_FREE_SLAVES_2_Name),
               MyMissionID.SLAVER_BASE_1_FREE_SLAVES_2,
               (MyTextsWrapperEnum.SLAVER_BASE_1_FREE_SLAVES_2_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.SLAVER_BASE_1_HACK_HANGAR },
               Mothership_Bars,
               false
           ) { SaveOnSuccess = false, ToDestroyCount = 1, HudName = MyTextsWrapperEnum.HudCage };
            m_objectives.Add(m_freeSlavesMS); // pridani do seznamu submisi


            var talkWith6 = new MyObjectiveDialog(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_6_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_6,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_6_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_FREE_SLAVES_2, },
                dialogId: MyDialogueEnum.SLAVERBASE_0600_MOTHERSHIP_EMPTY
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith6);


            var findSlaves = new MyObjectiveDestroy(
                // Var is used to call functions on that member
                (MyTextsWrapperEnum.SLAVER_BASE_1_FIND_SLAVES_Name),
                MyMissionID.SLAVER_BASE_1_FIND_SLAVES,
                (MyTextsWrapperEnum.SLAVER_BASE_1_FIND_SLAVES_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_6 },
                bar_pit,
                // ID of dummy point of checkpoint
                false
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCage };
            findSlaves.MissionEntityIDs.Add((uint)EntityID.Bar_pit);
            m_objectives.Add(findSlaves); // pridani do seznamu submisi


            var talkWith7 = new MyObjectiveDialog(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_7_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_7,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_7_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_FIND_SLAVES, },
                dialogId: MyDialogueEnum.SLAVERBASE_0700_PIT_EMPTY
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith7);
            talkWith7.OnMissionLoaded += FreePilotsMSSuccess;


            var destroyEnemies = new MyDestroyWavesObjective(
                // Var is used to call functions on that member
                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_ENEMIES_Name),
                MyMissionID.SLAVER_BASE_1_DESTROY_ENEMIES,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DESTROY_ENEMIES_Description),
                null,
                this,
                new[] { MyMissionID.SLAVER_BASE_1_DIALOG_7 }
                //new List<uint>
                //    {
                //        (uint) EntityID.Mothership_Spawn_1,
                //        (uint) EntityID.Mothership_Spawn_2,
                //        (uint) EntityID.Mothership_Spawn_3
                //    },
                ) { SaveOnSuccess = false };
            destroyEnemies.AddWave(new List<uint> { (uint)EntityID.Mothership_Spawn_1 });
            destroyEnemies.AddWave(new List<uint> { (uint)EntityID.Mothership_Spawn_2 });
            destroyEnemies.AddWave(new List<uint> { (uint)EntityID.Mothership_Spawn_3 });
            destroyEnemies.OnMissionSuccess += EnemiesDestroyed;
            m_objectives.Add(destroyEnemies); // pridani do seznamu submisi

            var talkWith9 = new MyObjectiveDialog(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_9_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_9,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_9_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DESTROY_ENEMIES },
                dialogId: MyDialogueEnum.SLAVERBASE_0800_ENEMIES_DESTROYED
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith9);


            m_openHangarDoors = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.SLAVER_BASE_1_HACK_NUKE_Name),
                MyMissionID.SLAVER_BASE_1_HACK_NUKE,
                (MyTextsWrapperEnum.SLAVER_BASE_1_HACK_NUKE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_DIALOG_9 },
                null,
                new List<uint> { (uint)EntityID.HangarDoorsHub },
                new List<uint> { (uint)EntityID.Cargo_Bay_Doors }
                ) { HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(m_openHangarDoors);
            m_openHangarDoors.OnMissionSuccess += MSEscape;
            m_openHangarDoors.OnMissionSuccess += MoveFourthReichMSStart;
          


            var talkWith10 = new MyTimedObjective(
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_10_Name),
                MyMissionID.SLAVER_BASE_1_DIALOG_10,
                (MyTextsWrapperEnum.SLAVER_BASE_1_DIALOG_10_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_1_HACK_NUKE },
                TimeSpan.FromSeconds(50)
                )
                {
                    SaveOnSuccess = false,
                    SuccessDialogId = MyDialogueEnum.SLAVERBASE_1100_FAKE_ESCAPED,
                    DisplayCounter = false,
                };
            talkWith10.MissionEntityIDs.Add((uint) EntityID.Escape_Dummy);
            m_objectives.Add(talkWith10);
        }

        void HideMothership()
        {
            MyScriptWrapper.HideEntity(m_fourthReichMS);

            foreach (var item in m_standbyparticle)
            {
                MyScriptWrapper.SetEntityEnabled(item, false);
            }
            foreach (var item in m_finalparticle)
            {
                MyScriptWrapper.SetEntityEnabled(item, false);
            }
        }

        void DestroyGeneratorMSSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Generator_Spawn_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Generator_Spawn_2);
        }

        void DestroyBatteriesMSSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Batteries_Spawn);
            foreach (var Light in TurnOff)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(Light), false);
            }
        }

        void FindPrisonMSSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Prison_Spawn_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Prison_Spawn_2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CIC_Spawn);
        }

        void MSEscape(MyMissionBase sender)
        {
            foreach (var Door in m_openedddoors)
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(Door));
            }
            foreach (var Door in m_closeddoors)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(Door));
            }
            foreach (var spawn in m_pilotsSpawn)
            {
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    if (bot.Ship != null) // If bots are already spawned
                    {
                        if (!bot.Ship.IsDead())
                        {
                            bot.Ship.MarkForClose();
                        }
                    }
                }
            }

            foreach (var Entity in m_closeentiites)
            {
                var item = MyScriptWrapper.TryGetEntity(Entity);
                if (item != null)
                {
                    item.MarkForClose();
                }
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3);

            }
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.Mothership_container), -1);
            foreach (var Entity in m_PC)
            {
                var item = MyScriptWrapper.TryGetEntity(Entity);
                if (item != null)
                {
                    item.MarkForClose();
                }
               
            }

        
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_1));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_2));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_3));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_4));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_1));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_2));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_3));
            foreach (var Doors in m_doors)
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(Doors));
            }
        }
        void EnemiesDestroyed(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
        }

        void HangarOpen(MyMissionBase sender)
        {
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_1));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_2));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_3));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_1));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_2));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_3));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hide_Doors_4));
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Prison_Spawm_3);
        }

     /*   void FreeMothershipMSSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_3);
            sender.OnMissionSuccess -= FreeMothershipMSSuccess;
        
        }*/

        void FreePilotsMSSuccess(MyMissionBase sender)
        {
            foreach (var spawn in m_pilotsSpawn)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawn);
            }
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.Endless_spawn);
            //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_1);
            //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_2);
            //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Mothership_Spawn_3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3);

            //sender.OnMissionSuccess -= FreePilotsMSSuccess;
        }

        /*
        void Hide(MyMissionBase sender)
        {
            MyScriptWrapper.HideEntity(m_fourthReichMS);
            sender.OnMissionSuccess -= Hide;
        }
          */
     
        /* void DestroyMSSuccess2(MyMissionBase sender)
         {
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4723), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4719), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4709), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4705), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4687), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4691), false);
             MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity(4677), false);
            
             sender.OnMissionSuccess -= DestroyMSSuccess2;
         }*/
        
        
        public override void Load() // vykona se jednou na zacatku
        {
            // MyScriptWrapper.SetEntityPriority(m_fourthReichMS, -1);
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);

            //MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity("Marcus"));
            foreach (var turret in m_turrets)
            {
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(turret));
            }
            foreach (var doors in m_doors)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(doors));
            }
            m_fourthReichMS = MyScriptWrapper.GetEntity((uint)EntityID.Mothership_container);
            m_moveMS_Up = false;
            m_moveMS_Back = false;
            m_moveMS_Up2 = false;
            m_moveMS_Out = false;
            m_moveMS_Speed = 70f;
            m_moveMS_AccelerateCounter = 0;
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_1));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_2));
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Hidden_Doors_3));
            //m_returnToMothership.OnMissionSuccess += Hide;
            foreach (var Door in m_openedddoors)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(Door));
            }
            foreach (var PC in m_PC)
            {
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(PC));
            }
           
            base.Load();  
        }

        public override void Unload()
        {
            MyScriptWrapper.StopTransition(3);
            m_fourthReichMS = null;
            base.Unload();
        }



        public override void Update()//vola se v kazdem snimku
        {
            base.Update();
            if (m_moveMS_Up)
            {
                foreach (var item in m_standbyparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MyScriptWrapper.SetEntityEnabled(particle, true);
                    MoveMotherShipForward(particle, new Vector3(0, m_moveMS_Speed, 0));
                }
                foreach (var item in m_finalparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MoveMotherShipForward(particle, new Vector3(0, m_moveMS_Speed, 0));
                }
                m_moveMS_Back = MoveMotherShipForwardDest(m_fourthReichMS, new Vector3(0, m_moveMS_Speed, 0), m_fourthReichMS_UpDestination);
                if (m_moveMS_Back) 
                    m_moveMS_Up = false;
            }
            if (m_moveMS_Back)
            {
                foreach (var item in m_standbyparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MoveMotherShipForward(particle, new Vector3(0, 0, m_moveMS_Speed));
                }
                foreach (var item in m_finalparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MoveMotherShipForward(particle, new Vector3(0, 0, m_moveMS_Speed));
                }
                m_moveMS_Up2 = MoveMotherShipForwardDest(m_fourthReichMS, new Vector3(0, 0, m_moveMS_Speed), m_fourthReichMS_BackDestination);
                if (m_moveMS_Up2) m_moveMS_Back = false;
            }
            if (m_moveMS_Up2)
            {
                foreach (var item in m_standbyparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MyScriptWrapper.SetEntityEnabled(particle, false);
                    MoveMotherShipForward(particle, new Vector3(0, m_moveMS_Speed, 0));
                }
                foreach (var item in m_finalparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MyScriptWrapper.SetEntityEnabled(particle, true);
                    MoveMotherShipForward(particle, new Vector3(0, m_moveMS_Speed, 0));
                }
                m_moveMS_Out = MoveMotherShipForwardDest(m_fourthReichMS, new Vector3(0, m_moveMS_Speed, 0), m_fourthReichMS_Up2Destination);
                if (m_moveMS_Out)
                {
                    MissionTimer.RegisterTimerAction(300, Accelerate, false);
                    m_moveMS_Up2 = false;
                }
            }
            if (m_moveMS_Out)
            {
                foreach (var item in m_standbyparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MoveMotherShipForward(particle, new Vector3(0, 0, -m_moveMS_Speed));
                }
                foreach (var item in m_finalparticle)
                {
                    MyEntity particle = MyScriptWrapper.GetEntity(item);
                    MoveMotherShipForward(particle, new Vector3(0, 0, -m_moveMS_Speed));
                }
                MoveMotherShipForward(m_fourthReichMS, new Vector3(0, 0, -m_moveMS_Speed));
            }
        }

        void Accelerate()
        {
            if (m_moveMS_AccelerateCounter < 150)
            {
                m_moveMS_Speed += 28f;
                MissionTimer.RegisterTimerAction(250, Accelerate, false);
            }
        }

        bool MoveMotherShipForwardDest(MyEntity entity, Vector3 velocity, Vector3 destination)
        {
            if (Vector3.DistanceSquared(destination, entity.GetPosition()) > 10 * 10)
            {
                MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);

                return false;
            }
            return true;
        }

        void MoveMotherShipForward(MyEntity entity, Vector3 velocity)
        {
            MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
        }

        void MoveFourthReichMSStart(MyMissionBase sender)
        {
            m_moveMS_Up = true;
        }

        //public Missions.OnMissionSuccess DestroyMSSuccess { get; set; }
    }


}
using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyJunkyardConvinceMission: MyMission
    {
        #region EntityIDs

        enum EntityID // list of IDs used in script
        {
            StartLocation = 131399,
            SmugglerShip = 131460,
            FindSmuggler = 215058,
            FlyToEnemy = 218159,
            GetCloserToMomo = 219214,
            SP1_1 = 218102,
            SP1_2 = 218103,
            SP1_3 = 215497,
            SP1_4 = 215498,
            SP2_1 = 218118,
            SP2_2 = 218119,
            SP3_1 = 218121,
            SP3_2 = 218120,
            SP3_3 = 218122,
            SP3_4 = 218123,
            SP4_1 = 219223,
            SP4_2 = 218124,
            MomoZappaSP = 222239,
            MomoGangMember1 = 1626,
            MomoGangMember2 = 1599,
            Smuggler = 156176,
            SP_Find_1 = 218868,
            SP_Find_2 = 218161,
            SP_Find_3 = 218162,
            Waypoint = 215041,
            WaypointLast = 218520,
            Waypointprelast = 167047,
            SmugglerRunWaypoint = 167041,
            BombDealer = 240252,
            BombDealerDetector = 240251,
            MarcusBRPosition = 240324,
            TarjaInitialPosition = 20,
            TarjaBRPosition = 240325,
            GetToMarcus = 240326,
            ValentinBRPosition = 264212,
            BR_SP_Boss = 240342,
            BR_SP_1 = 240328,
            BR_SP_2 = 240339,
            BR_SP_3 = 240340,
            BR_SP_4 = 240341,
            FlyToMS = 131401,
            SmugglerStart = 264571,
            ManjeetFinalPosition = 531508,
            ManjeetFindDetector = 532570,

            // cargo box IDs:
            Ammo1 = 900,
            Ammo2 = 903,
            Repair1 = 901,
            Repair2 = 902,
        }

        List<uint> m_mothershipsSetlowpriority = new List<uint>
            {
                215353,
                215351,
                130855,
                131199,
                131091,
                5668,
                130774,
                131052,
                130828,
                5664
            };

        List<uint> m_killWavesSecond1 = new List<uint> { 545718, 545736 };
        List<uint> m_killWavesSecond2 = new List<uint> { 16786676, 16786677, 545717, 545714 };
        List<uint> m_killWavesSecond3 = new List<uint> { 16786734, 16786735, 545716, 545715 };
        List<uint> m_killWavesSecond4 = new List<uint> { 16786733, 16786732, 16786737, 16786736, 16786738, 16786739 };

        List<uint> m_spawns = new List<uint>
            {
                (uint) EntityID.BR_SP_1,
                (uint) EntityID.BR_SP_2,
                (uint) EntityID.BR_SP_3,
                (uint) EntityID.BR_SP_4,
                (uint) EntityID.BR_SP_Boss
            };

        private List<uint> m_firstWave = new List<uint>
        {
            (uint)EntityID.SP1_1, (uint)EntityID.SP1_2, (uint)EntityID.SP1_3, (uint)EntityID.SP1_4
        };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof (EntityID)))
            {
                MyScriptWrapper.GetEntity((uint) ((value as EntityID?).Value));
            }
            foreach (var value in m_mothershipsSetlowpriority)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        #endregion

        #region Entities

        MyEntity m_waypoint;
        MyEntity m_waypointprelast;
        MyEntity m_waypointlast;

        MyObjective m_objective01FindInformator;
        MyObjectiveDialog m_objective01BFindInformator;
        MyObjective m_objective02FindSmuggler;
        MyObjectiveDestroy m_objective03FightCompanions;
        MyObjective m_objective04FollowSmuggler;
        MyObjectiveDialog m_objective04DFollowSmuggler;
        MyObjective m_objective05GetCloserToEnemy;
        MyObjectiveDialog m_objective05MetZappasFirstGangman;
        MyDestroyWavesObjective m_objective06KillWaves;
        MyMeetObjective m_objective07SpeakWithMomo;
        MyObjective m_objective07DSpeakWithMomo;
        MyObjectiveDestroy m_objective08FightMomo;
        MyObjective m_objective09ReturnToSmuggler;
        MyObjectiveDialog m_objective09DReturnToSmuggler;
        MyObjective m_objective10FindBombDealer;
        MyObjectiveDialog m_objective10DFindBombDealer;
        MyObjective m_objective11GetToMarcus;
        MyDestroyWavesObjective m_objective12BrFight;
        MyObjective m_objective13FlyToMS;

        MySmallShipBot m_manjeet;
        MySmallShipBot m_blackRavenBoss;
        MySmallShipBot m_momoBoss;
        MySmallShipBot m_marcus;
        MySmallShipBot m_valentin;
        MySmallShipBot m_tarja;

        MyEntityDetector m_manjeetFindDetector;

        #endregion

        public MyJunkyardConvinceMission()
        {
            ID = MyMissionID.JUNKYARD_CONVINCE; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.JUNKYARD_CONVINCE;
            Description = MyTextsWrapperEnum.JUNKYARD_CONVINCE_Description; //"Convince the Smuggler to join you in your mission \n"
            DebugName = new StringBuilder("08a-Junkyard"); // Name of mission
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission08_JunkyardDuncan;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2567538,0,-172727); // Story sector of the script - i.e. (-2465,0,6541)
            
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.LAST_HOPE }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_RETURN_TO_MS };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, };

            #region Objectives
            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            // ----- objectives -----

            m_objective01FindInformator = new MyMeetObjective(
                // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIND_INFORMATOR_Name),
                // Name of the submission
                MyMissionID.JUNKYARD_CONVINCE_FIND_INFORMATOR,
                // ID of the submission - must be added to MyMissions.cs
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIND_INFORMATOR_Description),
                // Description of the submission
                this,
                new MyMissionID[] { },
                // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                null,
                "RavenGirl",
                250,
                1.00f,
                startDialogueId:MyDialogueEnum.JUNKYARD_CONVINCE_0100_INTRODUCE
                )
                {
                    // False means do not save game in that checkpoint
                    SaveOnSuccess = false,
                    FollowMe = false
                };
            m_objective01FindInformator.OnMissionLoaded += O01FindInformatorLoaded;
            m_objectives.Add(m_objective01FindInformator);


            m_objective01BFindInformator = new MyObjectiveDialog(
                MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_FIND_INFORMATOR_Name,
                MyMissionID.JUNKYARD_CONVINCE_D_FIND_INFORMATOR,
                MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_FIND_INFORMATOR_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_FIND_INFORMATOR },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_0200_INFORMATOR
                )
                {
                    SaveOnSuccess = false,
                };
            m_objective01BFindInformator.OnMissionLoaded += O01BFindInformatorLoaded;
            m_objective01BFindInformator.OnMissionSuccess += O01BFindInformatorSuccess;
            m_objectives.Add(m_objective01BFindInformator);

            m_objective02FindSmuggler = new MyObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIND_SMUGGLER_Name),
                MyMissionID.JUNKYARD_CONVINCE_FIND_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIND_SMUGGLER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_FIND_INFORMATOR },
                new MyMissionLocation(baseSector, (uint) EntityID.FindSmuggler) // ID of dummy point of checkpoint
                )
                {
                    SaveOnSuccess = false,
                    SuccessDialogId = MyDialogueEnum.JUNKYARD_CONVINCE_0300_RUN,
                    HudName = MyTextsWrapperEnum.HudManjeet
                };
            m_objective02FindSmuggler.OnMissionLoaded += O02FindSmugglerLoaded;
            m_objective02FindSmuggler.OnMissionSuccess += O2FindSmugglerSuccess;
            m_objectives.Add(m_objective02FindSmuggler);

            m_objective04FollowSmuggler = new MyMeetObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FOLLOW_SMUGGLER_Name),
                MyMissionID.JUNKYARD_CONVINCE_FOLLOW_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FOLLOW_SMUGGLER_Description),
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_FIND_SMUGGLER },
                null,
                (uint) EntityID.Smuggler,
                300,
                1.00f
                )
                {
                    SaveOnSuccess = false,
                    FollowMe = false
                };
            m_objective04FollowSmuggler.OnMissionLoaded += O04FollowSmugglerLoaded;
            m_objective04FollowSmuggler.OnMissionSuccess += O04FollowSmugglerSuccess;
            m_objectives.Add(m_objective04FollowSmuggler);

            m_objective04DFollowSmuggler = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_FOLLOW_SMUGGLER_Name),
                MyMissionID.JUNKYARD_CONVINCE_D_FOLLOW_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_FOLLOW_SMUGGLER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_FOLLOW_SMUGGLER },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_0500_CATCHED
                ) { SaveOnSuccess = false, };
            m_objectives.Add(m_objective04DFollowSmuggler);
            m_objective04DFollowSmuggler.OnMissionLoaded += O04DFollowSmugglerLoaded;
            m_objective04DFollowSmuggler.OnMissionSuccess += O04DFollowSmugglerSuccess;

            m_objective05GetCloserToEnemy = new MyObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FLY_TO_ENEMY_Name),
                MyMissionID.JUNKYARD_CONVINCE_FLY_TO_ENEMY,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FLY_TO_ENEMY_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_FOLLOW_SMUGGLER },
                new MyMissionLocation(baseSector, (uint) EntityID.FlyToEnemy),
                radiusOverride: 500f
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            m_objective05GetCloserToEnemy.OnMissionLoaded += O05GetCloserToEnemyLoaded;
            m_objectives.Add(m_objective05GetCloserToEnemy);

            m_objective05MetZappasFirstGangman = new MyObjectiveDialog(
                MyMissionID.JUNKYARD_CONVINCE_MEET_GANGMAN,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_FLY_TO_ENEMY },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_0650_MET_ZAPPA_GUARD
            );
            m_objectives.Add(m_objective05MetZappasFirstGangman);
            m_objective05MetZappasFirstGangman.OnMissionLoaded += O05MetZappasFirstGangmanLoaded;
            m_objective05MetZappasFirstGangman.OnMissionSuccess += O05MetZappasFirstGangmanSuccess;

            m_objective06KillWaves = new MyDestroyWavesObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_KILL_WAVES_Name),
                MyMissionID.JUNKYARD_CONVINCE_KILL_WAVES,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_KILL_WAVES_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_MEET_GANGMAN },
                null,
                null,
                null,
                null,
                4
                ) { SaveOnSuccess = false };
            m_objective06KillWaves.AddWave(new List<uint> {(uint)EntityID.SP1_1, (uint)EntityID.SP1_2, (uint)EntityID.SP1_3, (uint)EntityID.SP1_4});
            m_objective06KillWaves.AddWave(new List<uint> {(uint)EntityID.SP2_1, (uint)EntityID.SP2_2});
            m_objective06KillWaves.AddWave(new List<uint> { (uint)EntityID.SP3_1, (uint)EntityID.SP3_2, (uint)EntityID.SP3_3 });
            m_objective06KillWaves.OnMissionLoaded += O06KillWavesLoaded;
            m_objective06KillWaves.OnMissionSuccess += O06KillWavesSuccess;
            m_objectives.Add(m_objective06KillWaves);

            m_objective07SpeakWithMomo = new MyMeetObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_SPEAK_WITH_MOMO_Name),
                MyMissionID.JUNKYARD_CONVINCE_SPEAK_WITH_MOMO,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_SPEAK_WITH_MOMO_Description),
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_KILL_WAVES },
                null,
                (uint?) null,
                500,
                1.00f
                )
                {
                    SaveOnSuccess = false,
                    FollowMe = false
                };
            m_objectives.Add(m_objective07SpeakWithMomo);

            m_objective07DSpeakWithMomo = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_SPEAK_WITH_MOMO_Name),
                MyMissionID.JUNKYARD_CONVINCE_D_SPEAK_WITH_MOMO,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_SPEAK_WITH_MOMO_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_SPEAK_WITH_MOMO },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_0900_THE_MOMO
                ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective07DSpeakWithMomo);
            m_objective07DSpeakWithMomo.OnMissionLoaded += O07DSpeakWithMomoLoaded;
            m_objective07DSpeakWithMomo.OnMissionSuccess += O07DSpeakWithMomoSuccess;

            m_objective08FightMomo = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIGHT_MOMO_Name),
                MyMissionID.JUNKYARD_CONVINCE_FIGHT_MOMO,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_FIGHT_MOMO_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_SPEAK_WITH_MOMO },
                null,
                new List<uint> { (uint) EntityID.SP4_1, (uint) EntityID.SP4_2, (uint) EntityID.MomoZappaSP },
                true
                ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing };
            m_objective08FightMomo.OnMissionLoaded += O08FightMomoLoaded;
            m_objectives.Add(m_objective08FightMomo);

            m_objective09ReturnToSmuggler = new MyMeetObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_RETURN_TO_SMUGGLER_Name),
                MyMissionID.JUNKYARD_CONVINCE_RETURN_TO_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_RETURN_TO_SMUGGLER_Description),
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_FIGHT_MOMO },
                null,
                (uint) EntityID.Smuggler,
                300,
                1.00f
                //startDialogueId: MyDialogueEnum.JUNKYARD_CONVINCE_1000_LAST_OF_THEM
                ) { SaveOnSuccess = true, FollowMe = false };
            m_objective09ReturnToSmuggler.OnMissionLoaded += O09ReturnToSmugglerLoaded;
            m_objectives.Add(m_objective09ReturnToSmuggler);

            m_objective09DReturnToSmuggler = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_RETURN_TO_SMUGGLER_Name),
                MyMissionID.JUNKYARD_CONVINCE_D_RETURN_TO_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_RETURN_TO_SMUGGLER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_RETURN_TO_SMUGGLER },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_1100_FIGHT_ENDS
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective09DReturnToSmuggler);
            m_objective09DReturnToSmuggler.OnMissionLoaded += O09DReturnToSmugglerLoaded;
            m_objective09DReturnToSmuggler.OnMissionSuccess += O09DReturnToSmugglerSuccess;

            m_objective10FindBombDealer = new MyMeetObjective(
                // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_GO_TO_BOMB_DEALER_Name),
                // Name of the submission
                MyMissionID.JUNKYARD_CONVINCE_GO_TO_BOMB_DEALER,
                // ID of the submission - must be added to MyMissions.cs
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_GO_TO_BOMB_DEALER_Description),
                // Description of the submission
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_RETURN_TO_SMUGGLER },
                // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                null,
                (uint) EntityID.BombDealer,
                300,
                1.00f
                )
                {
                    SaveOnSuccess = false,
                    RequiredActors = new MyActorEnum[] { MyActorEnum.TARJA, MyActorEnum.VALENTIN },
                    FollowMe = false
                };
            m_objective10FindBombDealer.OnMissionLoaded += O10FindBombDealerLoaded;
            m_objectives.Add(m_objective10FindBombDealer);

            m_objective10DFindBombDealer = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_GO_TO_BOMB_DEALER_Name),
                MyMissionID.JUNKYARD_CONVINCE_D_GO_TO_BOMB_DEALER,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_GO_TO_BOMB_DEALER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_GO_TO_BOMB_DEALER},
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_1300_BOMB_DEALER
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective10DFindBombDealer);
            m_objective10DFindBombDealer.OnMissionLoaded += O10DFindBombDealerLoaded;
            m_objective10DFindBombDealer.OnMissionSuccess += O10DFindBombDealerSuccess;

            m_objective11GetToMarcus = new MyMeetObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_GO_TO_MARCUS_Name),
                MyMissionID.JUNKYARD_CONVINCE_GO_TO_MARCUS,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_GO_TO_MARCUS_Description),
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_GO_TO_BOMB_DEALER },
                null,
                "Marcus",
                300,
                1.00f
                )
                {
                    SaveOnSuccess = true,
                    FollowMe = false,
                };
            m_objective11GetToMarcus.OnMissionLoaded += O11GetToMarcusLoaded;
            m_objective11GetToMarcus.OnMissionSuccess += O11GetToMarcusSuccess;
            m_objectives.Add(m_objective11GetToMarcus);

            var objective11DGetToMarcus = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_GO_TO_MARCUS_Name),
                MyMissionID.JUNKYARD_CONVINCE_D_GO_TO_MARCUS,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_D_GO_TO_MARCUS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_GO_TO_MARCUS },
                dialogId: MyDialogueEnum.JUNKYARD_CONVINCE_1400_ARRIVED_AT_MARCUS
                ) { SaveOnSuccess = false };
            m_objectives.Add(objective11DGetToMarcus);

            m_objective12BrFight = new MyDestroyWavesObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_BR_FIGHT_Name),
                MyMissionID.JUNKYARD_CONVINCE_BR_FIGHT,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_BR_FIGHT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_D_GO_TO_MARCUS },
                new List<uint>
                    {
                        (uint) EntityID.BR_SP_1,
                        (uint) EntityID.BR_SP_2,
                        (uint) EntityID.BR_SP_3,
                        (uint) EntityID.BR_SP_4,
                        (uint) EntityID.BR_SP_Boss
                    }
                )
                {
                    SaveOnSuccess = false,
                    StartDialogId = MyDialogueEnum.JUNKYARD_CONVINCE_1500_GANGSTER_FIGHT_STARTED,
                    SuccessDialogId = MyDialogueEnum.JUNKYARD_CONVINCE_1600_GO_TO_MS
                };
            m_objective12BrFight.OnMissionLoaded += O12BRFightLoaded;
            m_objective12BrFight.OnMissionSuccess += O12BRFightSuccess;
            m_objective12BrFight.AddWave(m_killWavesSecond1);
            m_objective12BrFight.AddWave(m_killWavesSecond2);
            m_objective12BrFight.AddWave(m_killWavesSecond3);
            m_objective12BrFight.AddWave(m_killWavesSecond4);
            m_objectives.Add(m_objective12BrFight);

            m_objective13FlyToMS = new MyObjective(
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_RETURN_TO_MS_Name),
                MyMissionID.JUNKYARD_CONVINCE_RETURN_TO_MS,
                (MyTextsWrapperEnum.JUNKYARD_CONVINCE_RETURN_TO_MS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_CONVINCE_BR_FIGHT },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: 300f
                )
                {
                    SaveOnSuccess = true,
                    SuccessDialogId = MyDialogueEnum.JUNKYARD_CONVINCE_1700_FINALE,
                    HudName = MyTextsWrapperEnum.HudMadelynsSapho
                };
            m_objectives.Add(m_objective13FlyToMS);
            #endregion      

            m_spawns.AddRange(m_killWavesSecond1);
            m_spawns.AddRange(m_killWavesSecond2);
            m_spawns.AddRange(m_killWavesSecond3);
            m_spawns.AddRange(m_killWavesSecond4);
        }


        // Code in that block will be called on the load of the sector
        public override void Load()
        {
            foreach (var item in m_mothershipsSetlowpriority)
            {
                MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity(item),-1);
            }
            
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BombDealer));

            m_manjeet = (MySmallShipBot) MyScriptWrapper.GetEntity((uint) EntityID.Smuggler);
            m_marcus = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            m_tarja = MyScriptWrapper.InsertFriend(MyActorEnum.TARJA);
            MyScriptWrapper.SetEntityDisplayName(m_tarja, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_SmugglerInformator).ToString());

            m_waypoint = MyScriptWrapper.GetEntity((uint)EntityID.Waypoint);
            m_waypointlast = MyScriptWrapper.GetEntity((uint)EntityID.WaypointLast);
            m_waypointprelast = MyScriptWrapper.GetEntity((uint)EntityID.Waypointprelast);

            m_manjeetFindDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.ManjeetFindDetector));

            m_momoBoss = MyScriptWrapper.TryGetEntity("Momo") as MySmallShipBot;

            base.Load();
        }

        public override void Unload()
        {
            base.Unload();

            m_manjeet = null;
            m_marcus = null;
            m_tarja = null;
            
            m_waypoint = null;
            m_waypointlast = null;
            m_waypointprelast = null;

            m_manjeetFindDetector = null;

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
        }

        #region OnMissionLoaded

        void O01FindInformatorLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopFollow(m_tarja);
            MyScriptWrapper.Move(m_tarja, MyScriptWrapper.GetEntity((uint)EntityID.TarjaInitialPosition).GetPosition());
            MyScriptWrapper.Move(m_manjeet, MyScriptWrapper.GetEntity((uint)EntityID.SmugglerStart).GetPosition());
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 1, "KA02");
            MyScriptWrapper.HideEntity(m_manjeet);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP_Find_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP_Find_2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP_Find_3);
        }

        void O01BFindInformatorLoaded(MyMissionBase sender)
        {
            m_tarja.LookTarget = MySession.PlayerShip;
            MyScriptWrapper.SetEntityDisplayName(m_tarja, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_Tarja).ToString());
        }

        void O02FindSmugglerLoaded(MyMissionBase sender)
        {
        }

        void O04FollowSmugglerLoaded(MyMissionBase sender)
        {
            m_manjeetFindDetector.OnEntityEnter -= ManjeetFound;
            m_manjeetFindDetector.Off();
            m_manjeet.SetWaypointPath("SmugglerRun");
            m_manjeet.PatrolMode = MyPatrolMode.ONE_WAY;
            m_manjeet.Patrol();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 3, "MM01");
        }

        void O04DFollowSmugglerLoaded(MyMissionBase sender)
        {
            m_manjeet.LookTarget = MySession.PlayerShip;
        }

        void O05GetCloserToEnemyLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 1, "KA01");
            //smuggler.SetWaypointPath("SmugglerRun2");
            m_manjeet.PatrolMode = MyPatrolMode.ONE_WAY;
            m_manjeet.Patrol();
            m_manjeet.LookTarget = null;
            MissionTimer.RegisterTimerAction(10000, StartPathDialogue, false);
        }

        void O05MetZappasFirstGangmanLoaded(MyMissionBase sender)
        {
            foreach (var spawn in new List<uint> { (uint)EntityID.MomoGangMember1, (uint)EntityID.MomoGangMember2 })
            {
                var entity = MyScriptWrapper.GetEntity(spawn) as MySmallShipBot;
                if (entity != null) entity.LookTarget = MySession.PlayerShip;
            }
        }

        void O06KillWavesLoaded(MyMissionBase sender)
        {
            //MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_CONVINCE_0700_ALMOST_THERE);
            m_manjeet.Idle();
            m_manjeet.SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.ManjeetFinalPosition).WorldMatrix);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 3, "MM01");
            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
        }

        void O07DSpeakWithMomoLoaded(MyMissionBase sender)
        {
            m_momoBoss.LookTarget = MySession.PlayerShip;
            foreach (var spawn in new List<uint> { (uint)EntityID.SP4_1, (uint)EntityID.SP4_2, (uint)EntityID.MomoZappaSP })
            {
                MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.Slavers);
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    MyScriptWrapper.SetEntityDestructible(bot.Ship, true);
                    MyScriptWrapper.ChangeFaction(bot.Ship, MyMwcObjectBuilder_FactionEnum.Slavers);
                }
            }
        }

        void O08FightMomoLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 3, "KA07");
        }

        void O09ReturnToSmugglerLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 1, "KA02");
        }

        void O09DReturnToSmugglerLoaded(MyMissionBase sender)
        {
            m_manjeet.LookTarget = MySession.PlayerShip;
        }

        void O10FindBombDealerLoaded(MyMissionBase sender)
        {
            m_manjeet.LookTarget = null;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 1, "MM01");
            MyScriptWrapper.SetSleepDistance(m_marcus, 5000);
            MyScriptWrapper.StopFollow(m_marcus);
            m_marcus.SetWaypointPath("MarcusOut");
            m_marcus.SpeedModifier = 2.0f;
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.MarcusIsLeavingForTarja, MyGuiManager.GetFontMinerWarsGreen(), 5000));
            m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();

            m_valentin = (MySmallShipBot) MyScriptWrapper.GetEntity("RavenGuy");
            m_valentin.LeaderLostEnabled = true;
            MyScriptWrapper.MarkEntity(m_valentin, MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.VALENTIN)).ToString(), HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Friend);
            MyScriptWrapper.StopFollow(m_valentin);
            MyEntity valentinPosition = MyScriptWrapper.GetEntity((uint)EntityID.ValentinBRPosition);
            m_valentin.SetWorldMatrix(valentinPosition.WorldMatrix);

            m_tarja.LeaderLostEnabled = true;
            MyScriptWrapper.StopFollow(m_tarja);
            MyScriptWrapper.MarkEntity(m_tarja, MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.TARJA)).ToString(), HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Friend);
            MyEntity tarjaPosition = MyScriptWrapper.GetEntity((uint)EntityID.TarjaBRPosition);
            m_tarja.SetWorldMatrix(tarjaPosition.WorldMatrix);
            MyScriptWrapper.SetEntityDisplayName(m_tarja, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_Tarja).ToString());
            
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BombDealer));
        }

        void O10DFindBombDealerLoaded(MyMissionBase sender)
        {
            MySmallShipBot bombDealer = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BombDealer);
            bombDealer.LookTarget = MySession.PlayerShip;
        }

        void O11GetToMarcusLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.BR_SP_Boss);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 1, "MM01");
        }

        void O12BRFightLoaded(MyMissionBase sender)
        {
            m_valentin = (MySmallShipBot) MyScriptWrapper.GetEntity("RavenGuy");
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3, "KA03");
            MyScriptWrapper.Follow(MySession.PlayerShip, m_marcus);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_tarja);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_valentin);
            m_marcus.SpeedModifier = 1.0f;

            MyScriptWrapper.ChangeFaction((uint)EntityID.BR_SP_Boss, MyMwcObjectBuilder_FactionEnum.Slavers);
            foreach (var bot in MyScriptWrapper.GetSpawnPointBots((uint)EntityID.BR_SP_Boss))
            {
                if (bot.Ship != null)
                {
                    MyScriptWrapper.SetEntityDestructible(bot.Ship, true);
                    MyScriptWrapper.ChangeFaction(bot.Ship, MyMwcObjectBuilder_FactionEnum.Slavers);
                }
            }

            foreach (var spawn in m_spawns)
            {
                MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.Slavers);
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    if (bot.Ship != null)
                    {
                        MyScriptWrapper.SetEntityDestructible(bot.Ship, true);
                        MyScriptWrapper.ChangeFaction(bot.Ship, MyMwcObjectBuilder_FactionEnum.Slavers);
                    }
                }
            }
        }

        #endregion

        #region OnMissionSuccess

        void O01BFindInformatorSuccess(MyMissionBase sender)
        {
            m_tarja.LookTarget = null;
            m_manjeetFindDetector.OnEntityEnter += ManjeetFound;
            m_manjeetFindDetector.On();
            MyScriptWrapper.UnhideEntity(m_manjeet);
            MyScriptWrapper.SetEntityPriority(m_manjeet, -1);
            m_manjeet.SpeedModifier = 1.25f;
        }

        void O2FindSmugglerSuccess(MyMissionBase sender)
        {
            foreach (var spawn in new List<uint> { (uint)EntityID.SP_Find_1, (uint)EntityID.SP_Find_2, (uint)EntityID.SP_Find_3 })
            {
                MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.Slavers);
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    if (bot.Ship != null)
                    {
                        MyScriptWrapper.SetEntityDestructible(bot.Ship, true);
                        MyScriptWrapper.ChangeFaction(bot.Ship, MyMwcObjectBuilder_FactionEnum.Slavers);
                    }
                }
            }
        }

        void O04FollowSmugglerSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 1, "KA03");

            foreach (var spawn in new List<uint> { (uint)EntityID.SP_Find_1, (uint)EntityID.SP_Find_2, (uint)EntityID.SP_Find_3 })
            {
                MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.Slavers);
                foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                {
                    if (bot.Ship != null)
                    {
                        MyScriptWrapper.ChangeFaction(bot.Ship, MyMwcObjectBuilder_FactionEnum.Traders);
                    }
                }
            }

            m_manjeet.Idle();

            foreach (var spawn in new List<uint> { (uint)EntityID.MomoGangMember1, (uint)EntityID.MomoGangMember2 })
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(spawn));
            }
        }

        void O04DFollowSmugglerSuccess(MyMissionBase sender)
        {
            m_manjeet.LookTarget = null;
        }

        void O05MetZappasFirstGangmanSuccess(MyMissionBase sender)
        {
            foreach (var spawn in new List<uint> { (uint)EntityID.MomoGangMember1, (uint)EntityID.MomoGangMember2 })
            {
                var entity = MyScriptWrapper.GetEntity(spawn) as MySmallShipBot;
                if (entity != null) entity.LookTarget = null;
                MyScriptWrapper.ChangeFaction(entity, MyMwcObjectBuilder_FactionEnum.Slavers);
            }
        }

        void O06KillWavesSuccess(MyMissionBase sender)
        {
            //MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_CONVINCE_0800_MOMO_ARRIVE);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.MomoZappaSP);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 3, "KA02");
        }

        void O07DSpeakWithMomoSuccess(MyMissionBase sender)
        {
            m_momoBoss.LookTarget = null;
        }

        void O09DReturnToSmugglerSuccess(MyMissionBase sender)
        {
            m_manjeet.LookTarget = null;
        }

        void O10DFindBombDealerSuccess(MyMissionBase sender)
        {
            MySmallShipBot bombDealer = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.BombDealer);
            bombDealer.LookTarget = null;
        }

        void O11GetToMarcusSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.MarkEntity(m_valentin, MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.VALENTIN)).ToString(), HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS, HUD.MyGuitargetMode.Friend);
            MyScriptWrapper.MarkEntity(m_tarja, MyTextsWrapper.Get(MyActorConstants.GetActorDisplayName(MyActorEnum.TARJA)).ToString(), HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS, HUD.MyGuitargetMode.Friend);
        }

        static void O12BRFightSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 1, "KA01");
        }

        #endregion

        #region General Events

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity spawnedBot)
        {
            foreach (var sp in m_firstWave)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(sp))
                {
                    MyScriptWrapper.SetSleepDistance(spawnedBot, 3000);
                }
            }

            foreach (var item in new List<uint> { (uint)EntityID.SP4_1, (uint)EntityID.SP4_2 })
            {
                if (spawnpoint.EntityId.Value.NumericValue == item)
                {
                    MyScriptWrapper.StopFollow(spawnedBot);
                    if (m_momoBoss != null)
                    {
                        MyScriptWrapper.SetEntityDestructible(spawnedBot, false);
                        MyScriptWrapper.Follow(m_momoBoss, spawnedBot);
                        MyScriptWrapper.SetSleepDistance(spawnedBot, 3000);
                    }
                    else
                    {
                        Debug.Assert(true, "Boss entity is null. Cannot identify leader");
                    }
                }
            }

            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.MomoZappaSP))
            {
                spawnedBot.SetName("Momo");
                spawnedBot.DisplayName = MyTexts.MomoZappa;
                m_momoBoss = (MySmallShipBot) spawnedBot;
                m_objective07SpeakWithMomo.BotToTalkId = MyScriptWrapper.GetEntityId(m_momoBoss);
                MyScriptWrapper.SetEntityDestructible(m_momoBoss, false);
                m_momoBoss.LookTarget = MySession.PlayerShip;
                m_momoBoss.Health = m_momoBoss.MaxHealth = 1000;
                MyScriptWrapper.SetSleepDistance(spawnedBot, 3000);
                MyScriptWrapper.ActivateSpawnPoint((uint) EntityID.SP4_1);
                MyScriptWrapper.ActivateSpawnPoint((uint) EntityID.SP4_2);
            }

            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.BR_SP_Boss))
            {
                spawnedBot.DisplayName = MyTexts.GangsterBoss;
                m_blackRavenBoss = (MySmallShipBot)spawnedBot;
                MyScriptWrapper.SetEntityDestructible(m_blackRavenBoss, false);
                MyScriptWrapper.ChangeFaction(m_blackRavenBoss, MyMwcObjectBuilder_FactionEnum.Traders);
                m_blackRavenBoss.LookTarget = MySession.PlayerShip;
                m_blackRavenBoss.Health = m_blackRavenBoss.MaxHealth = 500;
                MyScriptWrapper.SetSleepDistance(spawnedBot, 5000);

                m_blackRavenBoss.OnDie += OnGangsterBossDied;

                foreach (var item in new List<uint>
                    {
                        (uint) EntityID.BR_SP_1,
                        (uint) EntityID.BR_SP_2,
                        (uint) EntityID.BR_SP_3,
                        (uint) EntityID.BR_SP_4,
                        (uint) EntityID.BR_SP_Boss
                    })
                {
                    MyScriptWrapper.ActivateSpawnPoint(item);
                }
            }

            foreach (var item in m_spawns)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(item) && spawnedBot != m_blackRavenBoss)
                {
                    MyScriptWrapper.SetSleepDistance(spawnedBot, 5000);
                    if (m_blackRavenBoss != null && !m_blackRavenBoss.Closed)
                    {
                        if (ActiveObjectives.Contains(m_objective12BrFight))
                        {
                            MyScriptWrapper.SetEntityDestructible(spawnedBot, true);
                            MyScriptWrapper.ChangeFaction(spawnedBot, MyMwcObjectBuilder_FactionEnum.Slavers);
                        }
                        else
                        {
                            MyScriptWrapper.SetEntityDestructible(spawnedBot, false);
                            MyScriptWrapper.ChangeFaction(spawnedBot, MyMwcObjectBuilder_FactionEnum.Traders);
                        }
                        MyScriptWrapper.Follow(m_blackRavenBoss, spawnedBot);
                    }
                    else
                    {
                        MyScriptWrapper.SetEntityDestructible(spawnedBot, true);
                        MyScriptWrapper.ChangeFaction(spawnedBot, MyMwcObjectBuilder_FactionEnum.Slavers);
                        ((MySmallShipBot)spawnedBot).Attack(MySession.PlayerShip);
                    }
                }
            }
        }

        void OnGangsterBossDied(MyEntity entity, MyEntity killer)
        {
            foreach (var bot in m_blackRavenBoss.Followers)
            {
                bot.Attack(MySession.PlayerShip);
            }
        }

        void ManjeetFound(MyEntityDetector sender, MyEntity bot, int meetcriterias)
        {
            if (sender == m_manjeetFindDetector && bot == MySession.PlayerShip && m_objective01BFindInformator.IsAvailable())
            {
                m_objective01BFindInformator.Success();
                m_objective02FindSmuggler.Success();
            }
        }

        void StartPathDialogue()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_CONVINCE_0600_BEFORE_FIGHT);
        }

        #endregion
    }
}

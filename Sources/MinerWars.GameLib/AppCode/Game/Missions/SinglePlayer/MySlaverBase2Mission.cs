using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MySlaverBase2Mission : MyMission
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 4954,
            turret_1 = 966,
            turret_2 = 991,
            turret_3 = 864,
            turret_4 = 2081,
            turret_5 = 1566,
            turret_6 = 1605,
            generator_1 = 2681,
            slave_1 = 5910,
            slave_2 = 5909,
            slave_3 = 5911,
            slave_4 = 5912,
            slave_5 = 5913,
            FinalFreedom = 5914,
            Return = 14377,
            hub1 = 18080,
            hub2 = 18081,
            generatorspawn = 18359,
            unlockspawn = 19303,
            freeslaves2spawn = 20146,
            freeslaves4spawn = 19714,
            ShipTransport1 = 5928,
            ShipTransport2 = 5915,
            GeneratorDetectorDummy = 10,
        }

        List<uint> m_chainEffects = new List<uint>()
            {
                25492, 25493, 25494, 25495, 25496, 25497, 25498, 25499, 25500, 25501, 
            };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }

            foreach (var value in m_chainEffects)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        bool m_slaverCaptainTalked;
        int m_slavesFreed;

        public MySlaverBase2Mission()
        {
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };


            ID = MyMissionID.SLAVER_BASE_2; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("09d-Slaver base Delta Earnings"); // Name of mission
            Name = MyTextsWrapperEnum.SLAVER_BASE_DELTA_EARNINGS;
            Description = MyTextsWrapperEnum.SLAVER_BASE_DELTA_EARNINGS_Description;
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(4169480, 0, -8216683); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.FORT_VALIANT_B }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[]
                {
                    MyMissionID.SLAVER_BASE_2_FREE_SLAVES,
                };
            m_objectives = new List<MyObjective>(); // Creating of list of submissions


            // ----------- START OF SUBMISSIONS DEFINITION --------------
            var introTalk = new MyObjectiveDialog(
                MyMissionID.SLAVER_BASE_2_INTRO,
                null,
                this,
                new MyMissionID[] { },
                MyDialogueEnum.SLAVERBASE2_0100_INTRO
                ) { SaveOnSuccess = false };
            m_objectives.Add(introTalk);


            var paralyzeDefense = new MyObjectiveDestroy(
                // MySubmissionDestroy means mission with objective to destroy something - here it is class member so you can call methods on it
                MyTextsWrapperEnum.SLAVER_BASE_2_PARALYZE_DEFENSE,
                //Name of the submission
                MyMissionID.SLAVER_BASE_2_PARALYZE_DEFENSE,
                // ID of the submission
                MyTextsWrapperEnum.SLAVER_BASE_2_PARALYZE_DEFENSE_Description,
                // Description of the submission
                null,
                this,
                new MyMissionID[]
                    {
                        MyMissionID.SLAVER_BASE_2_INTRO
                    },
                // ID of submissions required to make this submission available - these declares the sequence of submissions
                new List<uint>
                    {
                        (uint) EntityID.turret_1,
                        (uint) EntityID.turret_2,
                        (uint) EntityID.turret_3,
                        (uint) EntityID.turret_4,
                        (uint) EntityID.turret_5,
                        (uint) EntityID.turret_6
                    },
                // ID of objects to be destroyed as a mission objective
                null,
                true,
                startDialogID: MyDialogueEnum.SLAVERBASE2_0200_DESTROY_TURRETS
                ) { SaveOnSuccess = true };
            m_objectives.Add(paralyzeDefense);

            paralyzeDefense.OnMissionLoaded += ParalyzeDefenseOnMissionLoaded;
            paralyzeDefense.OnMissionCleanUp += ParalyzeDefenseOnMissionUnloaded;


            var unlockprison1 = new MyUseObjective(
                MyTextsWrapperEnum.UNLOCK_PRISON_1,
                MyMissionID.UNLOCK_PRISON_1,
                MyTextsWrapperEnum.Blank,
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_2_PARALYZE_DEFENSE },
                new MyMissionLocation(baseSector, (uint) EntityID.hub1),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.SecurityControlHUB,
                MyTextsWrapperEnum.HackingProgress,
                2000,
                startDialogId: MyDialogueEnum.SLAVERBASE2_0300_TURRETS_DESTROYED
                ) { SaveOnSuccess = false };
            unlockprison1.OnMissionSuccess += m_Unlockprison1_OnMissionSuccess;
            m_objectives.Add(unlockprison1);


            var unlockprison2 = new MyUseObjective(
                MyTextsWrapperEnum.UNLOCK_PRISON_2,
                MyMissionID.UNLOCK_PRISON_2,
                MyTextsWrapperEnum.Blank,
                null,
                this,
                new MyMissionID[] { MyMissionID.UNLOCK_PRISON_1 },
                new MyMissionLocation(baseSector, (uint) EntityID.hub2),
                MyTextsWrapperEnum.PressToHack,
                MyTextsWrapperEnum.SecurityControlHUB,
                MyTextsWrapperEnum.HackingProgress,
                2000,
                startDialogId: MyDialogueEnum.SLAVERBASE2_0400_FIRST_HUB_DESTROYED
                ) { SaveOnSuccess = false };
            m_objectives.Add(unlockprison2);


            var talkAboutGenerator = new MyObjectiveDialog(
                MyMissionID.SLAVER_BASE_2_TALK_ABOUT_GENERATOR,
                null,
                this,
                new MyMissionID[] { MyMissionID.UNLOCK_PRISON_2 },
                MyDialogueEnum.SLAVERBASE2_0500_BOTH_HUBS_DESTROYED
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkAboutGenerator);


            var breakTheChains = new MyObjectiveDestroy(
                MyTextsWrapperEnum.SLAVER_BASE_2_BREAK_THE_CHAINS,
                MyMissionID.SLAVER_BASE_2_BREAK_THE_CHAINS,
                MyTextsWrapperEnum.Blank,
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_2_TALK_ABOUT_GENERATOR },
                new List<uint> { (uint) EntityID.generator_1 },
                null,
                true
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudGenerator };
            breakTheChains.OnMissionSuccess += BreakTheChains_OnMissionSuccess;
            breakTheChains.Components.Add(new MyDetectorDialogue((uint)EntityID.GeneratorDetectorDummy, MyDialogueEnum.SLAVERBASE2_0600_GENERATOR_REACHED));
            m_objectives.Add(breakTheChains);


            var talkAboutPrisoners = new MyObjectiveDialog(
                MyMissionID.SLAVER_BASE_2_TALK_ABOUT_PRISONERS,
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_2_BREAK_THE_CHAINS },
                MyDialogueEnum.SLAVERBASE2_0700_GENERATOR_DESTROYED
                ) { SaveOnSuccess = false };
            m_objectives.Add(talkAboutPrisoners);


            var freeSlaves = new MyMultipleUseObjective(
                MyTextsWrapperEnum.SLAVER_BASE_2_FREE_SLAVES,
                MyMissionID.SLAVER_BASE_2_FREE_SLAVES,
                MyTextsWrapperEnum.Blank,
                null,
                this,
                new MyMissionID[] { MyMissionID.SLAVER_BASE_2_TALK_ABOUT_PRISONERS },
                MyTextsWrapperEnum.UnlockCell,
                MyTextsWrapperEnum.PrisonCell,
                MyTextsWrapperEnum.UnlockingInProgress,
                2000,
                new List<uint>
                    {
                        (uint) EntityID.slave_1,
                        (uint) EntityID.slave_2,
                        (uint) EntityID.slave_3,
                        (uint) EntityID.slave_4,
                        (uint) EntityID.slave_5
                    },
                MyUseObjectiveType.Activating
                )
                {
                    SaveOnSuccess = true,
                    SuccessDialogId = MyDialogueEnum.SLAVERBASE2_1200_MISSION_COMPLETE
                };
            freeSlaves.OnObjectUsedSucces += OnFreeSlavesObjectiveSuccess;
            m_objectives.Add(freeSlaves);

            //var finalFreedom = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
            //    new StringBuilder("Return to Valiant."), // Name of the submission
            //    MyMissionID.SLAVER_BASE_2_FINAL_FREEDOM, // ID of the submission - must be added to MyMissions.cs
            //    new StringBuilder("Return to Valiant.\n"), // Description of the submission
            //    null,
            //    this,
            //    new MyMissionID[] { MyMissionID.SLAVER_BASE_2_FREE_SLAVES, MyMissionID.SLAVER_BASE_1_FREE_SLAVES_2, MyMissionID.SLAVER_BASE_1_FREE_SLAVES_3, MyMissionID.SLAVER_BASE_1_FREE_SLAVES_4, MyMissionID.SLAVER_BASE_1_FREE_SLAVES_5 }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
            //    new MyMissionLocation(baseSector, (uint)EntityID.FinalFreedom), // ID of dummy point of checkpoint
            //    startDialogId: MyDialogueEnum.SLAVERBASE2_1200_MISSION_COMPLETE
            //  ) { SaveOnSuccess = false }; // False means do not save game in that checkpointa
            //m_objectives.Add(finalFreedom); // Adding this submission to the list of submissions of current mission
        }

        void OnFreeSlavesObjectiveSuccess(uint entityID)
        {
            m_slavesFreed++;

            switch (m_slavesFreed)
            {
                case 1:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_0800_FIRST_CELL_UNLOCKED);
                    break;
                case 2:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_0900_SECOND_CELL_UNLOCKED);
                    break;
                case 3:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_1000_THIRD_CELL_UNLOCKED);
                    break;
                case 4:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_1100_FOURTH_CELL_UNLOCKED);
                    break;
                case 5:
                    //MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_1200_MISSION_COMPLETE);
                    break;
                default:
                    Debug.Fail("should not get here! slaver base 2, too many prisoners?");
                    break;
            }
        }

        void ParalyzeDefenseOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.EntityDeath += OnMyParalyzeDefenseEntityDeath;
        }

        void ParalyzeDefenseOnMissionUnloaded(MyMissionBase sender)
        {
            MyScriptWrapper.EntityDeath -= OnMyParalyzeDefenseEntityDeath;
        }

        void OnMyParalyzeDefenseEntityDeath(MyEntity entity1, MyEntity entity2)
        {
            if (m_slaverCaptainTalked)
            {
                return;
            }

            if (entity1.EntityId == null)
                return;

            if (entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_1 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_2 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_3 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_4 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_5 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_6 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_5 ||
                entity1.EntityId.Value.NumericValue == (uint)EntityID.turret_6)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.SLAVERBASE2_0201_SLAVER_TALK);
                m_slaverCaptainTalked = true;
            }
        }

        void m_FreeSlaves_4_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.freeslaves4spawn);
        }

        void m_FreeSlaves_2_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.freeslaves2spawn);
        }

        void m_Unlockprison1_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.unlockspawn);
        }

        void BreakTheChains_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.generatorspawn);

            foreach (var chainEffect in m_chainEffects)
            {
                MyScriptWrapper.SetEntityEnabled(chainEffect, false);
            }
        }

        void Return_OnMissionLoaded(MyMissionBase sender)
        {
            MySmallShipBot transport1 = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.ShipTransport1);
            transport1.SetWaypointPath("transport");
            transport1.Patrol();
            MySmallShipBot transport2 = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.ShipTransport2);
            transport2.SetWaypointPath("transport");
            transport2.Patrol();
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            m_slaverCaptainTalked = false;
            m_slavesFreed = 0;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.EntityDeath += MyScriptWrapperOnEntityDeath;
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);
        }

        private void MyScriptWrapperOnEntityDeath(MyEntity entity1, MyEntity entity2)
        {
            if (entity1.EntityId == null)
                return;

            if (entity1.EntityId.Value.NumericValue == (uint)EntityID.ShipTransport1 || entity1.EntityId.Value.NumericValue == (uint)EntityID.ShipTransport2)
            {
                Fail(MyTextsWrapperEnum.Fail_TransportDestroyed);
            }
        }


        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.EntityDeath -= MyScriptWrapperOnEntityDeath;
        }

    }

}

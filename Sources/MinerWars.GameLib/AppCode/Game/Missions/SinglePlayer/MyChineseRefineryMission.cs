#region Using

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
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.Resources;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyChineseRefineryMission : MyMission
    {
        #region Members

        private MyObjective m_objective01_GetCloserToAsteroid;
        private MyObjective m_objective02_GetInside;
        private MyObjective m_objective03_FindSecretRoom;
        private MyObjectiveDialog m_objective03_D_FindSecterRoom;
        private MyObjective m_objective04_SetVirus;
        private MyUseObjective m_objective05_DeactivateBomb;
        private MyObjective m_objective06_GetToFirstTunnel;
        private MyObjective m_objective07_PastFirstTunnel;
        private MyObjective m_objective08_SetBugInComputer;
        private MyObjective m_objective09_GetToSecondTunnel;
        private MyObjective m_objective10_PastSecondTunnel;
        private MyObjective m_objective11_SneakInsideTheStation;
        private MyObjective m_objective12_GetToOldPath;
        private MyObjective m_objective13_HackRefineryComputer;
        private MyObjective m_objective14_GetOutOfTheStation;
        private MyObjective m_objective15_LandInsideTheTransporter;

        private MyEntityDetector m_detector_spawns1;
        private MyEntityDetector m_detector_spawns2;
        private MyEntityDetector m_detector_spawns3;
        private MyEntityDetector m_detector_spawns4;
        private MyEntityDetector m_detector_spawns5;
        private MyEntityDetector m_detector_spawns6;
        private MyEntityDetector m_detector_spawns7;
        private MyEntityDetector m_detector_spawns8;
        private MyEntityDetector m_detector_spawns9;
        private MyEntityDetector m_detector_spawns10;
        private MyEntityDetector m_detector_spawns11;
        private MyEntityDetector m_detector_spawns12;
        private MyEntityDetector m_detector_spawns13;
        private MyEntityDetector m_detector_spawns14;
        private MyEntityDetector m_detector_spawns15;
        private MyEntityDetector m_detector_spawns16;
        private MyEntityDetector m_detector_spawns17;
        private MyEntityDetector m_detector_spawns18;
        private MyEntityDetector m_detector_spawns19;
        private MyEntityDetector m_detector_spawns20;
        private MyEntityDetector m_detector_spawns21;
        private MyEntityDetector m_detector_spawns22;
        private MyEntityDetector m_detector_spawns23;
        private MyEntityDetector m_detector_spawns24;
        private MyEntityDetector m_detector_spawns25;
        
        private MyEntity m_transporter;
        private MyEntity m_HUBRefineryComputer;
        private MyEntity m_HUBSecretRoomComputer;
        private MyEntity m_HUBSetBugComputer;
        private MyEntity m_particleTransporter1;
        private MyEntity m_particleTransporter2;
        private MyEntity m_particleTransporter3;

        private bool m_transporterstop;

        #endregion

        #region Enum

        private enum EntityID // list of IDs used in script
        {
            StartLocation = 1432,
            O01GetCloserToAsterid = 886,
            O02GetInside = 7775,
            O03FindSecretRoom = 9645,
            O05Bomb = 7566,
            O06GetToFirstTunnel = 7746,
            O07PastFirstTunnel = 7747,
            O09GetToSecondTunnel = 7748,
            O10PastSecondTunnel = 885,
            O11SneakToStation = 883,
            O12GetOldPath = 882,
            O14GetOutOfTheStation = 2842,
            O15LandIn = 8900,
            Transporter = 2845,
            MoveTransporterDummy = 878,
            NuclearHeadDummy = 11784,
            HUBSetBugComputer = 16694,
            HUBRefineryComputer = 19441,
            HUBSecretRoomComputer = 7570,
            SPStart = 8125,
            SP1 = 8134,
            SP2 = 8135,
            SP3 = 8136,
            SP4 = 8137,
            SP5_1 = 8138,
            SP5_2 = 19368,
            SP6_1 = 8139,
            SP6_2 = 8140,
            SP7 = 8142,
            SPReachTunnel1 = 8149,
            SP8 = 8148,
            SP9 = 8146,
            SP10 = 8144,
            SPPastTunnel1_1 = 8151,
            SPPastTunnel1_2 = 8152,
            SPPastTunnel1_3 = 8153,
            SP11_1 = 8154,
            SP11_2 = 8155,
            SP11_3 = 8156,
            SP12_1 = 8157,
            SP12_2 = 8158,
            SP13 = 8160,
            SPReachTunnel2 = 8161,
            SP14 = 8163,
            SP15 = 8164,
            SP16 = 8165,
            SP17 = 4399,
            SP18_1 = 19423,
            SP18_2 = 19438,
            SP18_3 = 4400,
            SP19 = 4401,
            SP20 = 19431,
            SP21_1 = 19435,
            SP21_2 = 19440,
            SPBackdoor = 8904,
            SP22_1 = 19445,
            SP22_2 = 4398,
            SP23 = 19448,
            SP24 = 19451,
            SP25 = 19420,
            DetectorSP1 = 19283,
            DetectorSP2 = 19360,
            DetectorSP3 = 19362,
            DetectorSP4 = 19364,
            DetectorSP5 = 19366,
            DetectorSP6 = 19369,
            DetectorSP7 = 19393,
            DetectorSP8 = 19395,
            DetectorSP9 = 19397,
            DetectorSP10 = 19399,
            DetectorSP11 = 19405,
            DetectorSP12 = 19407,
            DetectorSP13 = 19409,
            DetectorSP14 = 19411,
            DetectorSP15 = 19413,
            DetectorSP16 = 19415,
            DetectorSP17 = 19421,
            DetectorSP18 = 19424,
            DetectorSP19 = 19426,
            DetectorSP20 = 19428,
            DetectorSP21 = 19436, 
            DetectorSP22 = 19446,
            DetectorSP23 = 19449,
            DetectorSP24 = 19452,     
            DetectorSP25 = 20750,
            TransporterParticle1 = 21397,
            TransporterParticle2 = 21396,
            TransporterParticle3 = 5147,
            Container1 = 6787,
        }

        private List<uint> m_SPPastTunnel2 = new List<uint>
        {
            4393, 4390, 4388, 19419, 4395, 4397
        };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            if (!IsMainSector)
            {
                return;
            }

            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (var item in m_SPPastTunnel2)
            {
                MyScriptWrapper.GetEntity(item);
            }
        }

        #endregion

        public MyChineseRefineryMission()
        {
            ID = MyMissionID.CHINESE_REFINERY; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.CHINESE_REFINERY;
            Description = MyTextsWrapperEnum.CHINESE_REFINERY_Description; // "Sneak throught the Chinese Refinery station and hack their system\n"
            DebugName = new StringBuilder("08c-Chinese refinery of Jingzhou"); // Name of mission
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2716080, 0, 4951053); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.CHINESE_TRANSPORT }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.CHINESE_REFINERY_15_LAND_INSIDE_THE_TRANSPORTER };
            RequiredActors = new MyActorEnum[] { MyActorEnum.TARJA };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            m_objective01_GetCloserToAsteroid = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_01_GET_CLOSER_Name),
              MyMissionID.CHINESE_REFINERY_01_GET_CLOSER,
              (MyTextsWrapperEnum.CHINESE_REFINERY_01_GET_CLOSER_Description),
              null,
              this,
              new MyMissionID[] { },
              new MyMissionLocation(baseSector, (uint)EntityID.O01GetCloserToAsterid),
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0100_GO_CLOSER
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudBackDoor };
            m_objectives.Add(m_objective01_GetCloserToAsteroid);

            m_objective02_GetInside = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_02_GET_IN_Name),
              MyMissionID.CHINESE_REFINERY_02_GET_IN,
              (MyTextsWrapperEnum.CHINESE_REFINERY_02_GET_IN_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_01_GET_CLOSER },
              new MyMissionLocation(baseSector, (uint)EntityID.O02GetInside),
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0150_GET_INSIDE
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(m_objective02_GetInside);

            m_objective03_FindSecretRoom = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_03_FIND_SECRET_ROOM_Name),
              MyMissionID.CHINESE_REFINERY_03_FIND_SECRET_ROOM,
              (MyTextsWrapperEnum.CHINESE_REFINERY_03_FIND_SECRET_ROOM_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_02_GET_IN },
              new MyMissionLocation(baseSector, (uint)EntityID.O03FindSecretRoom)
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudResearchRoom };
            m_objectives.Add(m_objective03_FindSecretRoom);

            m_objective03_D_FindSecterRoom = new MyObjectiveDialog(
                (MyTextsWrapperEnum.CHINESE_REFINERY_03_D_FIND_SECRET_ROOM_Name),
                MyMissionID.CHINESE_REFINERY_03_D_FIND_SECRET_ROOM,
                (MyTextsWrapperEnum.CHINESE_REFINERY_03_D_FIND_SECRET_ROOM_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_REFINERY_03_FIND_SECRET_ROOM },
                dialogId: MyDialogueEnum.CHINESE_REFINERY_0200_LABORATORY
                ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective03_D_FindSecterRoom);

            m_objective04_SetVirus = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_04_SET_VIRUS_Name),
              MyMissionID.CHINESE_REFINERY_04_SET_VIRUS,
              (MyTextsWrapperEnum.CHINESE_REFINERY_04_SET_VIRUS_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_03_D_FIND_SECRET_ROOM },
              null,
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0300_DEACTIVATE_BOMB
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            m_objective04_SetVirus.MissionEntityIDs.Add((uint)EntityID.HUBSecretRoomComputer);
            m_objectives.Add(m_objective04_SetVirus); 
            
            m_objective05_DeactivateBomb = new MyUseObjective(
                (MyTextsWrapperEnum.CHINESE_REFINERY_05_DEACTIVATE_BOMB_Name),
                MyMissionID.CHINESE_REFINERY_05_DEACTIVATE_BOMB,
                (MyTextsWrapperEnum.CHINESE_REFINERY_05_DEACTIVATE_BOMB_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_REFINERY_04_SET_VIRUS },
                new MyMissionLocation(baseSector, (uint)EntityID.NuclearHeadDummy),
                MyTextsWrapperEnum.PressToDeactivateNuclearHead,
                MyTextsWrapperEnum.NuclearHead,
                MyTextsWrapperEnum.DeactivatingInProgress,
                5000,
                MyUseObjectiveType.Repairing
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective05_DeactivateBomb);

            m_objective06_GetToFirstTunnel = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_06_GET_TO_FIRST_TUNNEL_Name),
              MyMissionID.CHINESE_REFINERY_06_GET_TO_FIRST_TUNNEL,
              (MyTextsWrapperEnum.CHINESE_REFINERY_06_GET_TO_FIRST_TUNNEL_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_05_DEACTIVATE_BOMB },
              new MyMissionLocation(baseSector, (uint)EntityID.O06GetToFirstTunnel),
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0400_GO_TO_SECOND_ASTEROID
          ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudTunnel };
            m_objectives.Add(m_objective06_GetToFirstTunnel);

            m_objective07_PastFirstTunnel = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_07_PAST_FIRST_TUNNEL_Name),
              MyMissionID.CHINESE_REFINERY_07_PAST_FIRST_TUNNEL,
              (MyTextsWrapperEnum.CHINESE_REFINERY_07_PAST_FIRST_TUNNEL_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_06_GET_TO_FIRST_TUNNEL },
              new MyMissionLocation(baseSector, (uint)EntityID.O07PastFirstTunnel)
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudEnd };
            m_objectives.Add(m_objective07_PastFirstTunnel);

            m_objective08_SetBugInComputer = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_08_SET_BUG_IN_COMPUTER_Name),
              MyMissionID.CHINESE_REFINERY_08_SET_BUG_IN_COMPUTER,
              (MyTextsWrapperEnum.CHINESE_REFINERY_08_SET_BUG_IN_COMPUTER_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_07_PAST_FIRST_TUNNEL },
              null,
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0500_FIND_THE_COMPUTER
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudHub };
            m_objective08_SetBugInComputer.MissionEntityIDs.Add((uint)EntityID.HUBSetBugComputer);
            m_objectives.Add(m_objective08_SetBugInComputer);

            m_objective09_GetToSecondTunnel = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_09_GET_TO_SECOND_TUNNEL_Name),
              MyMissionID.CHINESE_REFINERY_09_GET_TO_SECOND_TUNNEL,
              (MyTextsWrapperEnum.CHINESE_REFINERY_09_GET_TO_SECOND_TUNNEL_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_08_SET_BUG_IN_COMPUTER },
              new MyMissionLocation(baseSector, (uint)EntityID.O09GetToSecondTunnel),
              startDialogId: MyDialogueEnum.CHINESE_REFINERY_0600_GO_TO_THIRD_ASTEROID
          ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudTunnel };
            m_objectives.Add(m_objective09_GetToSecondTunnel);

            m_objective10_PastSecondTunnel = new MyObjective(
              (MyTextsWrapperEnum.CHINESE_REFINERY_10_PAST_SECOND_TUNNEL_Name),
              MyMissionID.CHINESE_REFINERY_10_PAST_SECOND_TUNNEL,
              (MyTextsWrapperEnum.CHINESE_REFINERY_10_PAST_SECOND_TUNNEL_Description),
              null,
              this,
              new MyMissionID[] { MyMissionID.CHINESE_REFINERY_09_GET_TO_SECOND_TUNNEL },
              new MyMissionLocation(baseSector, (uint)EntityID.O10PastSecondTunnel)
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudEnd };
            m_objectives.Add(m_objective10_PastSecondTunnel); 

            m_objective11_SneakInsideTheStation = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                (MyTextsWrapperEnum.CHINESE_REFINERY_11_SNEAK_INSIDE_THE_STATION_Name), // Name of the submission
                MyMissionID.CHINESE_REFINERY_11_SNEAK_INSIDE_THE_STATION, // ID of the submission - must be added to MyMissions.cs
                (MyTextsWrapperEnum.CHINESE_REFINERY_11_SNEAK_INSIDE_THE_STATION_Description), // Description of the submission
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_REFINERY_10_PAST_SECOND_TUNNEL }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.O11SneakToStation),
                startDialogId: MyDialogueEnum.CHINESE_REFINERY_0700_SNEAK_INSIDE_THE_STATION
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudBackDoor }; // False means do not save game in that checkpoint
            m_objectives.Add(m_objective11_SneakInsideTheStation); // Adding this submission to the list of submissions of current mission

             m_objective12_GetToOldPath = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
             (MyTextsWrapperEnum.CHINESE_REFINERY_12_GET_TO_OLD_PATH_Name), // Name of the submission
             MyMissionID.CHINESE_REFINERY_12_GET_TO_OLD_PATH, // ID of the submission - must be added to MyMissions.cs
             (MyTextsWrapperEnum.CHINESE_REFINERY_12_GET_TO_OLD_PATH_Description), // Description of the submission
             null,
             this,
             new MyMissionID[] { MyMissionID.CHINESE_REFINERY_11_SNEAK_INSIDE_THE_STATION }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
             new MyMissionLocation(baseSector, (uint)EntityID.O12GetOldPath),
             startDialogId: MyDialogueEnum.CHINESE_REFINERY_0800_FIND_THE_OLD_PATH
         ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing }; // False means do not save game in that checkpoint
            m_objectives.Add(m_objective12_GetToOldPath); // Adding this submission to the list of submissions of current mission

            m_objective13_HackRefineryComputer = new MyObjective( // Var is used to call functions on that member
                (MyTextsWrapperEnum.CHINESE_REFINERY_13_HACK_REFINARY_COMPUTER_Name),
                MyMissionID.CHINESE_REFINERY_13_HACK_REFINARY_COMPUTER,
                (MyTextsWrapperEnum.CHINESE_REFINERY_13_HACK_REFINARY_COMPUTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_REFINERY_12_GET_TO_OLD_PATH },
                null,
                startDialogId:MyDialogueEnum.CHINESE_REFINERY_0900_HACK_THE_COMPUTER
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHub };
            m_objective13_HackRefineryComputer.MissionEntityIDs.Add((uint)EntityID.HUBRefineryComputer);
            m_objectives.Add(m_objective13_HackRefineryComputer);

            m_objective14_GetOutOfTheStation = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
            (MyTextsWrapperEnum.CHINESE_REFINERY_14_GET_OUT_OF_THE_STATION_Name), // Name of the submission
            MyMissionID.CHINESE_REFINERY_14_GET_OUT_OF_THE_STATION, // ID of the submission - must be added to MyMissions.cs
            (MyTextsWrapperEnum.CHINESE_REFINERY_14_GET_OUT_OF_THE_STATION_Description), // Description of the submission
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_REFINERY_13_HACK_REFINARY_COMPUTER }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
            new MyMissionLocation(baseSector, (uint)EntityID.O14GetOutOfTheStation), 
            startDialogId: MyDialogueEnum.CHINESE_REFINERY_1000_GET_OUT
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing }; // False means do not save game in that checkpoint
            m_objectives.Add(m_objective14_GetOutOfTheStation); // Adding this submission to the list of submissions of current mission

            m_objective15_LandInsideTheTransporter = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
         (MyTextsWrapperEnum.CHINESE_REFINERY_15_LAND_INSIDE_THE_TRANSPORTER_Name), // Name of the submission
         MyMissionID.CHINESE_REFINERY_15_LAND_INSIDE_THE_TRANSPORTER, // ID of the submission - must be added to MyMissions.cs
         (MyTextsWrapperEnum.CHINESE_REFINERY_15_LAND_INSIDE_THE_TRANSPORTER_Description), // Description of the submission
         null,
         this,
         new MyMissionID[] { MyMissionID.CHINESE_REFINERY_14_GET_OUT_OF_THE_STATION }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
         new MyMissionLocation(baseSector, (uint)EntityID.O15LandIn) // ID of dummy point of checkpoint
         ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudTransport }; // False means do not save game in that checkpoint
            m_objectives.Add(m_objective15_LandInsideTheTransporter); // Adding this submission to the list of submissions of current mission



            m_objective05_DeactivateBomb.OnMissionSuccess += O05DeactivateBombSuccess;
            m_objective04_SetVirus.OnMissionSuccess += O04SetVirusSuccess;
            m_objective08_SetBugInComputer.OnMissionSuccess += O08SetBugInComputerSuccess;
            m_objective13_HackRefineryComputer.OnMissionSuccess += O13HackRefinerySuccess;

            m_objective01_GetCloserToAsteroid.OnMissionLoaded += M01GetCloserToAsteroidLoaded;
            m_objective03_FindSecretRoom.OnMissionLoaded += M03FindSecretRoomLoaded;
            m_objective04_SetVirus.OnMissionLoaded += M04SetVirusLoaded;
            m_objective05_DeactivateBomb.OnMissionLoaded += M05DeactivateBombLoaded;
            m_objective06_GetToFirstTunnel.OnMissionLoaded += M06GetToFirstTunnelLoaded;
            m_objective07_PastFirstTunnel.OnMissionLoaded += M07PastFirstTunnelLoaded;
            m_objective08_SetBugInComputer.OnMissionLoaded += M08SetBugInComputerLoaded;
            m_objective10_PastSecondTunnel.OnMissionLoaded += M10PastSecondTunnelLoaded;
            m_objective11_SneakInsideTheStation.OnMissionLoaded += M11SneakInsideTheStationLoaded;
            m_objective13_HackRefineryComputer.OnMissionLoaded += M13HackRefineryComputerLoaded;
            m_objective14_GetOutOfTheStation.OnMissionLoaded += M14GetOutOfTheStationLoaded;
        }

        public override void Accept()
        {
            base.Accept();

            MySession.PlayerShip.RepairToMax();
        }
        
        public override void Load() // Code in that block will be called on the load of the sector
        {
            if (!IsMainSector)
            {
                return;
            }

            m_detector_spawns1 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP1);
            m_detector_spawns2 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP2);
            m_detector_spawns3 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP3);
            m_detector_spawns4 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP4);
            m_detector_spawns5 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP5);
            m_detector_spawns6 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP6);
            m_detector_spawns7 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP7);
            m_detector_spawns8 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP8);
            m_detector_spawns9 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP9);
            m_detector_spawns10 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP10);
            m_detector_spawns11 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP11);
            m_detector_spawns12 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP12);
            m_detector_spawns13 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP13);
            m_detector_spawns14 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP14);
            m_detector_spawns15 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP15);
            m_detector_spawns16 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP16);
            m_detector_spawns17 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP17);
            m_detector_spawns18 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP18);
            m_detector_spawns19 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP19);
            m_detector_spawns20 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP20);
            m_detector_spawns21 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP21);
            m_detector_spawns22 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP22);
            m_detector_spawns23 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP23);
            m_detector_spawns24 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP24);
            m_detector_spawns25 = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSP24);

            m_HUBRefineryComputer = MyScriptWrapper.GetEntity((uint)EntityID.HUBRefineryComputer);
            m_HUBSecretRoomComputer = MyScriptWrapper.GetEntity((uint)EntityID.HUBSecretRoomComputer);
            m_HUBSetBugComputer = MyScriptWrapper.GetEntity((uint)EntityID.HUBSetBugComputer);

            MyScriptWrapper.EnablePhysics((uint)EntityID.Transporter, false);
            
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);



            m_transporter = MyScriptWrapper.GetEntity((uint)EntityID.Transporter);
            m_particleTransporter1 = MyScriptWrapper.GetEntity((uint)EntityID.TransporterParticle1);
            m_particleTransporter2 = MyScriptWrapper.GetEntity((uint)EntityID.TransporterParticle2);
            m_particleTransporter3 = MyScriptWrapper.GetEntity((uint)EntityID.TransporterParticle3);

            m_transporterstop = false;
           
            //We have travelled here with her
            MyEntity madelyn = MyScriptWrapper.TryGetEntity("Madelyn");
            if (madelyn != null)
            {
                MyScriptWrapper.HideEntity(madelyn);
            }

            MyScriptWrapper.EntityHacked += MyScriptWrapper_EntityHacked;
            base.Load();
        }

        public override void  Unload()
        {
            if (!IsMainSector)
            {
                return;
            }
            MyScriptWrapper.EntityHacked -= MyScriptWrapper_EntityHacked;
            m_detector_spawns1.Off();
            m_detector_spawns1.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns2.Off();
            m_detector_spawns2.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns3.Off();
            m_detector_spawns3.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns4.Off();
            m_detector_spawns4.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns5.Off();
            m_detector_spawns5.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns6.Off();
            m_detector_spawns6.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns7.Off();
            m_detector_spawns7.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns8.Off();
            m_detector_spawns8.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns9.Off();
            m_detector_spawns9.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns10.Off();
            m_detector_spawns10.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns11.Off();
            m_detector_spawns11.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns12.Off();
            m_detector_spawns12.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns13.Off();
            m_detector_spawns13.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns14.Off();
            m_detector_spawns14.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns15.Off();
            m_detector_spawns15.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns16.Off();
            m_detector_spawns16.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns17.Off();
            m_detector_spawns17.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns18.Off();
            m_detector_spawns18.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns19.Off();
            m_detector_spawns19.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns20.Off();
            m_detector_spawns20.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns21.Off();
            m_detector_spawns21.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns22.Off();
            m_detector_spawns22.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns23.Off();
            m_detector_spawns23.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns24.Off();
            m_detector_spawns24.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns25.Off();
            m_detector_spawns25.OnEntityEnter -= DetectorSPEntered;

            m_HUBRefineryComputer = null;
            m_HUBSecretRoomComputer = null;
            m_HUBSetBugComputer = null;

            m_transporter = null;
            m_particleTransporter1 = null;
            m_particleTransporter2 = null;
            m_particleTransporter3 = null;

            base.Unload();

            if (MyScriptWrapper.IsMissionFinished(this.ID))
            {
                MyScriptWrapper.TravelToMission(MyMissionID.CHINESE_ESCAPE);
            }
        }

        public override void Update() //Code in that block will be called in each frame
        {
            if (!IsMainSector)
            {
                return;
            }

            base.Update();

            if (!m_transporterstop)
            {
                m_transporterstop = MoveMotherShipForwardDest(m_transporter, 120f, new Vector3(-121f, 2407f, -2061f));
                MoveMotherShipForward(m_particleTransporter1, new Vector3(0, 0, -120f));
                MoveMotherShipForward(m_particleTransporter2, new Vector3(0, 0, -120f));
                MoveMotherShipForward(m_particleTransporter3, new Vector3(0, 0, -120f));
            }
            else
            {
                MyScriptWrapper.EnablePhysics((uint)EntityID.Transporter, true);
                MyScriptWrapper.SetParticleEffect(m_particleTransporter1, false);
                MyScriptWrapper.SetParticleEffect(m_particleTransporter2, false);
                MyScriptWrapper.SetParticleEffect(m_particleTransporter3, false);
            }
        }

        #region Mission Success
        void O04SetVirusSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.RemoveEntityMark(m_HUBSecretRoomComputer);
        }
           
        void O05DeactivateBombSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.O05Bomb, false,this);
        }

        void O08SetBugInComputerSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory);
            MyScriptWrapper.RemoveEntityMark(m_HUBSetBugComputer);
        }

        void O13HackRefinerySuccess(MyMissionBase sender)
        {
            MyScriptWrapper.RemoveEntityMark(m_HUBRefineryComputer);
        }
        #endregion

        #region Mission Loaded
        void M01GetCloserToAsteroidLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPStart);
            m_detector_spawns1.On();
            m_detector_spawns1.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns2.On();
            m_detector_spawns2.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns3.On();
            m_detector_spawns3.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns4.On();
            m_detector_spawns4.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns5.On();
            m_detector_spawns5.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns6.On();
            m_detector_spawns6.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns7.On();
            m_detector_spawns7.OnEntityEnter += DetectorSPEntered;
        }

        void M03FindSecretRoomLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetAlarmMode(MyScriptWrapper.GetEntity((uint)EntityID.Container1), true);
        }

        void M04SetVirusLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Mystery, 0, "MM01");
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.HUBSecretRoomComputer), true);
            MyScriptWrapper.MarkEntity(m_HUBSecretRoomComputer, MyTexts.SetVirusInComputer, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
        }

        void M05DeactivateBombLoaded(MyMissionBase sender)
        {
            //MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_REFINERY_0300_DEACTIVATE_BOMB);
            MyScriptWrapper.Highlight((uint)EntityID.O05Bomb, true,this);
        }

        void M06GetToFirstTunnelLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress);
        }

        void M07PastFirstTunnelLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetAlarmMode(MyScriptWrapper.GetEntity((uint)EntityID.Container1), true);
            m_detector_spawns1.Off();
            m_detector_spawns1.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns2.Off();
            m_detector_spawns2.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns3.Off();
            m_detector_spawns3.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns4.Off();
            m_detector_spawns4.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns5.Off();
            m_detector_spawns5.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns6.Off();
            m_detector_spawns6.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns7.Off();
            m_detector_spawns7.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns8.On();
            m_detector_spawns8.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns9.On();
            m_detector_spawns9.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns10.On();
            m_detector_spawns10.OnEntityEnter += DetectorSPEntered;
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPReachTunnel1);
            m_transporterstop = true;
        }

        void M08SetBugInComputerLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetAlarmMode(MyScriptWrapper.GetEntity((uint)EntityID.Container1), false);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPPastTunnel1_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPPastTunnel1_2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPPastTunnel1_3);
            m_detector_spawns8.Off();
            m_detector_spawns8.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns9.Off();
            m_detector_spawns9.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns10.Off();
            m_detector_spawns10.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns11.On();
            m_detector_spawns11.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns12.On();
            m_detector_spawns12.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns13.On();
            m_detector_spawns13.OnEntityEnter += DetectorSPEntered;
            MyScriptWrapper.MarkEntity(m_HUBSetBugComputer, MyTexts.SetBugInComputer, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
        }

        void M10PastSecondTunnelLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPReachTunnel2);
            m_detector_spawns11.Off();
            m_detector_spawns11.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns12.Off();
            m_detector_spawns12.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns13.Off();
            m_detector_spawns13.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns14.On();
            m_detector_spawns14.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns15.On();
            m_detector_spawns15.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns16.On();
            m_detector_spawns16.OnEntityEnter += DetectorSPEntered;
            m_transporterstop = true;
        }

        void M11SneakInsideTheStationLoaded(MyMissionBase sender)
        {
            foreach (var item in m_SPPastTunnel2)
            {
                MyScriptWrapper.ActivateSpawnPoint(item);
            }
            m_detector_spawns14.Off();
            m_detector_spawns14.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns15.Off();
            m_detector_spawns15.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns16.Off();
            m_detector_spawns16.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns17.On();
            m_detector_spawns17.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns18.On();
            m_detector_spawns18.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns19.On();
            m_detector_spawns19.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns20.On();
            m_detector_spawns20.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns21.On();
            m_detector_spawns21.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns25.On();
            m_detector_spawns25.OnEntityEnter += DetectorSPEntered;
        }

        void M13HackRefineryComputerLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SPBackdoor);
            m_detector_spawns17.Off();
            m_detector_spawns17.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns18.Off();
            m_detector_spawns18.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns19.Off();
            m_detector_spawns19.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns20.Off();
            m_detector_spawns20.OnEntityEnter -= DetectorSPEntered;
            m_detector_spawns21.Off();
            m_detector_spawns21.OnEntityEnter -= DetectorSPEntered;
            MyScriptWrapper.MarkEntity(m_HUBRefineryComputer, MyTexts.HackSystem, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, HUD.MyGuitargetMode.Objective);
        }

        void M14GetOutOfTheStationLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3);
            m_detector_spawns22.On();
            m_detector_spawns22.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns23.On();
            m_detector_spawns23.OnEntityEnter += DetectorSPEntered;
            m_detector_spawns24.On();
            m_detector_spawns24.OnEntityEnter += DetectorSPEntered;
            m_transporterstop = true;
            m_transporter.SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.MoveTransporterDummy).WorldMatrix);
        }
        #endregion       

        #region Spawnpoint Detectors
        void DetectorSPEntered(MyEntityDetector sender, MyEntity smallship, int meetcriterias)
        {
            if (sender == m_detector_spawns1 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP1);
                m_detector_spawns1.Off();
                m_detector_spawns1.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns2 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP2);
                m_detector_spawns2.Off();
                m_detector_spawns2.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns3 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP3);
                m_detector_spawns3.Off();
                m_detector_spawns3.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns4 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP4);
                m_detector_spawns4.Off();
                m_detector_spawns4.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns5 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP5_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP5_2);
                m_detector_spawns5.Off();
                m_detector_spawns5.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns6 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP6_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP6_2);
                m_detector_spawns6.Off();
                m_detector_spawns6.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns7 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP7);
                m_detector_spawns7.Off();
                m_detector_spawns7.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns8 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP8);
                m_detector_spawns8.Off();
                m_detector_spawns8.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns9 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP9);
                m_detector_spawns9.Off();
                m_detector_spawns9.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns10 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP10);
                m_detector_spawns10.Off();
                m_detector_spawns10.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns11 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP11_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP11_2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP11_3);
                m_detector_spawns11.Off();
                m_detector_spawns11.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns12 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP12_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP12_2);
                m_detector_spawns12.Off();
                m_detector_spawns12.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns13 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP13);
                m_detector_spawns13.Off();
                m_detector_spawns13.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns14 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP14);
                m_detector_spawns14.Off();
                m_detector_spawns14.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns15 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP15);
                m_detector_spawns15.Off();
                m_detector_spawns15.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns16 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP16);
                m_detector_spawns16.Off();
                m_detector_spawns16.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns17 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP17);
                m_detector_spawns17.Off();
                m_detector_spawns17.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns18 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP18_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP18_2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP18_3);
                m_detector_spawns18.Off();
                m_detector_spawns18.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns19 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP19);
                m_detector_spawns19.Off();
                m_detector_spawns19.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns20 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP20);
                m_detector_spawns20.Off();
                m_detector_spawns20.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns21 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP21_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP21_2);
                m_detector_spawns21.Off();
                m_detector_spawns21.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns22 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP22_1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP22_2);
                m_detector_spawns22.Off();
                m_detector_spawns22.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns23 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP23);
                m_detector_spawns23.Off();
                m_detector_spawns23.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns24 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP24);
                m_detector_spawns24.Off();
                m_detector_spawns24.OnEntityEnter -= DetectorSPEntered;
                return;
            }
            if (sender == m_detector_spawns25 && smallship == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SP24);
                m_detector_spawns25.Off();
                m_detector_spawns25.OnEntityEnter -= DetectorSPEntered;
                return;
            }
        }
#endregion

        #region General Methods

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

        void MoveMotherShipForward(MyEntity entity, Vector3 velocity)
        {
            MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
        }

        void MyScriptWrapper_EntityHacked(MyEntity entity)
        {
            if (entity == m_HUBRefineryComputer)
            {
                m_objective13_HackRefineryComputer.Success();
            }
            if (entity == m_HUBSecretRoomComputer)
            {
                m_objective04_SetVirus.Success();
            }
            if (entity == m_HUBSetBugComputer)
            {
                m_objective08_SetBugInComputer.Success();
            }
        }
        #endregion
    }

}

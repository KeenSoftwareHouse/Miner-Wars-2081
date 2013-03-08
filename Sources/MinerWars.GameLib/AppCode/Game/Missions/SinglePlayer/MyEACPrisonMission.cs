#region Using

using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
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
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyEACPrisonMission : MyMission
    {
        #region Enums

        public enum EntityID
        {
            DetectorBeforeStorage = 77502,
            DetectorBeforeIndustry = 77504,
            DetectorBeforeParking = 77506,
            DetectorBeforeMines = 77508,
            DetectorBeforeMinesEntrance = 77510,
            DetectorBeforeMarcusCell = 77073,
            DetectorBeforeSecurityCheck7 = 77512,
            DetectorBeforeSecurityCheck6 = 77514,
            DetectorBeforeSecurityCheck8 = 77516,
            DetectorBeforeSecurityCheck4 = 77518,
            DetectorBeforeCellAfterSecurityCheck4 = 77520,
            DetectorBeforeSecurityCheck5 = 77522,
            DetectorBeforeCell1AfterSecurityCheck5 = 77524,
            DetectorBeforeCell2AfterSecurityCheck5 = 77526,
            DetectorBeforeCell3AfterSecurityCheck5 = 77528,
            DetectorBeforeSecurityCheck3 = 77530,
            DetectorBeforeCell1AfterSecurityCheck3 = 77532,
            DetectorBeforeCell2AfterSecurityCheck3 = 77534,

            SpawnpointAtStorage =12596,
            SpawnpointAtIndustry1 = 76622,
            SpawnpointAtIndustry2 = 76621,
            SpawnpointAtParking = 12597,
            SpawnpointAtMines = 76623,
            SpawnpointAtMinesEntrance = 76624,
            SpawnpointAtMarcusCell = 76625,
            SpawnpointAtSecurityCheck7 = 12598,
            SpawnpointAtSecurityCheck6 = 12600,
            SpawnpointAtSecurityCheck8 = 12599,
            SpawnpointAtSecurityCheck4 = 12601,
            SpawnpointAtCellAfterSecurityCheck4 = 76626,
            SpawnpointAtSecurityCheck5 = 12602,
            SpawnpointAtCell1AfterSecurityCheck5 = 77056,
            SpawnpointAtCell2AfterSecurityCheck5 = 77055,
            SpawnpointAtCell3AfterSecurityCheck5 = 77054,
            SpawnpointAtSecurityCheck3 = 12603,
            SpawnpointAtCell1AfterSecurityCheck3 = 77072,
            SpawnpointAtCell2AfterSecurityCheck3 = 77071,
            SpawnpointAtWeHaveCompany = 185,

            SecurityHub = 75002,

            CargoIDCard = 16782189,

            CargoExitDoor = 10763,

           // OfflineMarcusShip = 75040,

            MotherShiBattleLocationDummy = 72035,
            BreakInsideStationLocationDummy = 28346,
            _MarcusIntelLocation = 31233,
            HubHack = 74991,
            HubHackGenerator = 74993,
            MarcusCellLocation = 31236,
            MarcusShipDetector = 31237,
            CargoForMarcus = 31238,
            StationDummy = 31239,
            _16MeetingPoint = 70028,
            StartLocation = 16536,

            StartSpawn = 12569,
            SecuritySpawn1 = 12604,
            SecuritySpawn2 = 12590,
            DestroyPanelsEnable1 = 11131,
            DestroyPanelsEnable2 = 11147,
            PanelsPrefabContainer = 42938,
            StartDetector = 40036,

            SecurityDetector = 51658,
            SecurityDetector2 = 54568,
            MarcusPlaceholder = 75040,
            MarcusFakeShip1 = 75026,

            MarcusFakeShip2 = 75053,
            MarcusFakeShip3 = 75066,
            Doors1 = 10638,
            Doors3 = 11072,
            Doors4 = 10692,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }

            var list = new List<uint>();
            list.AddRange(m_02toKill);
            list.AddRange(m_04toKill);
            list.AddRange(m_09toEnablePrefabs);
            list.AddRange(m_15toKillSpawnpoints);

            foreach (var entityId in list)
            {
                MyScriptWrapper.GetEntity(entityId);
            }
        }

        #endregion

        private MyObjective m_destroySolarPanelsFirst;
        private MyObjective m_destroySolarPanelsSecond;
        private MyObjective m_findCircoutPart;
        private int m_panelsLeft;

        private MyHudNotification.MyNotification m_hudSolarPanelsCounter;

        private const int m_totalPanelCount = 117;
        private static float DestroySolarPanelPercentsOne = 0.5f;
        private static float DestroySolarPanelPercentsTwo = 0.75f;
        private MyEntity m_botToBeLooted;
        private MySmallShipBot m_marcus;
        private List<MyInventoryItem> m_marcusInventory = new List<MyInventoryItem>();
        private MyEntity m_marcusPlacHolder ;
        private bool m_playerShipReverted;

        private MySmallShipBot m_ravenguyBot;
        private MySmallShipBot m_ravengirlBot;
        private List<uint> m_02toKill = new List<uint> { 37078, 37079, 37096, 37097, 37086, 37087, 37120, 37121, 37058, 37054, 37104, 37105, 37112, 37113 };
        private List<uint> m_04toKill = new List<uint> { 72040, 72042, 72044, 21 };
        private List<uint> m_09toEnablePrefabs = new List<uint> { 10756, 10927};
        private List<uint> m_15toKillSpawnpoints = new List<uint> { 74227, 75093 };
        List<uint> m_battleSpawnpoints = new List<uint>()
                                                 {
                                                     73060,
                                                     73059,
                                                     73610,
                                                     73611,
                                                     80377,
                                                     80378,

                                                 };

        private List<uint> m_EACdetectorSpawns = new List<uint>
                                                     {
                                                         74227,
                                                         75093,
                                                         74226,
                                                         85075,
                                                     };

        private List<uint> m_spawnsSecurityDetector= new List<uint>
                                                         {
                                                             12586,
                                                             12587,
                                                             12588,
                                                             12589,
                                                             12594,
                                                             12595,
                                                             12591,
                                                             12592,
                                                             12593,
                                                             79075,
                                                             79076,
                                                             79077,
                                                             79078,
                                                         };

        private uint[] m_spawns = new uint[] { 12586, 12587, 12588, 12589, 12594, 12595, 12591, 12592, 12593, 79075, 79076, 79077, 79078,
            /*12596, 76622,76621, 12597, 76623, 76624, 76625, 12598, 12600, 12599, 12601, 76626, 12602, 77056, 77055, 77054, 12603, 77072, 77071, 185*/
        };


        public MyEACPrisonMission()
        {
            ID = MyMissionID.EAC_PRISON; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("20-EAC prison");
            Name = MyTextsWrapperEnum.EAC_PRISON;
            Description = MyTextsWrapperEnum.EAC_PRISON_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission29_EacPrison;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(5480055, 0, -5077310);

            /* sector where the mission is located */
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); //posledne cislo - ID dummy pointu kde prijimam misiu

            RequiredMissions = new MyMissionID[] { MyMissionID.TWIN_TOWERS };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.EAC_PRISON_MEETINGPOINT };
            RequiredActors = new MyActorEnum[] { MyActorEnum.TARJA, MyActorEnum.VALENTIN, MyActorEnum.MADELYN, MyActorEnum.MARCUS };
            
            m_objectives = new List<MyObjective>();

            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeStorage, 0, (uint)EntityID.SpawnpointAtStorage));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeIndustry, 0, (uint)EntityID.SpawnpointAtIndustry1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeIndustry, 0, (uint)EntityID.SpawnpointAtIndustry2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeParking, 0, (uint)EntityID.SpawnpointAtParking));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeMines, 0, (uint)EntityID.SpawnpointAtMines));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeMinesEntrance, 0, (uint)EntityID.SpawnpointAtMinesEntrance));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeMarcusCell, 0, (uint)EntityID.SpawnpointAtMarcusCell));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck7, 0, (uint)EntityID.SpawnpointAtSecurityCheck7));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck6, 0, (uint)EntityID.SpawnpointAtSecurityCheck6));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck8, 0, (uint)EntityID.SpawnpointAtSecurityCheck8));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck4, 0, (uint)EntityID.SpawnpointAtSecurityCheck4));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCellAfterSecurityCheck4, 0, (uint)EntityID.SpawnpointAtCellAfterSecurityCheck4));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck5, 0, (uint)EntityID.SpawnpointAtSecurityCheck5));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCell1AfterSecurityCheck5, 0, (uint)EntityID.SpawnpointAtCell1AfterSecurityCheck5));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCell2AfterSecurityCheck5, 0, (uint)EntityID.SpawnpointAtCell2AfterSecurityCheck5));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCell3AfterSecurityCheck5, 0, (uint)EntityID.SpawnpointAtCell3AfterSecurityCheck5));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeSecurityCheck3, 0, (uint)EntityID.SpawnpointAtSecurityCheck3));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCell1AfterSecurityCheck3, 0, (uint)EntityID.SpawnpointAtCell1AfterSecurityCheck3));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeCell2AfterSecurityCheck3, 0, (uint)EntityID.SpawnpointAtCell2AfterSecurityCheck3));

            //Components.Add(new MySpawnpointWaves(detectorId, 3, new List<uint[]> { 
            //    new uint[] { spawnpointId1, spawnpointId2, spawnpointId3 },
            //    new uint[] { spawnpointId4, spawnpointId5 }}));
                        
            //01
            var mothershipBattle = new MyObjective(
                (MyTextsWrapperEnum.EAC_PRISON_THRUSWARM_Name),
                MyMissionID.EAC_PRISON_THRUSWARM,
                (MyTextsWrapperEnum.EAC_PRISON_THRUSWARM_Description),
                null,
                this,
                new MyMissionID[] {},
                new MyMissionLocation(baseSector, (uint)EntityID.MotherShiBattleLocationDummy)
                ) {SaveOnSuccess = true,StartDialogId = MyDialogueEnum.EAC_PRISON_0100, SuccessDialogId = MyDialogueEnum.EAC_PRISON_0200};
            m_objectives.Add(mothershipBattle);
            //mothershipBattle.Components.Add(new MySpawnpointWaves(77073, 0, 76625));
            mothershipBattle.OnMissionLoaded += mothershipBattle_OnMissionLoaded;

            //02
            
            var destroySolarDefence = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_PRISON_SOLARDEF_Name),
                MyMissionID.EAC_PRISON_SOLARDEF,
                (MyTextsWrapperEnum.EAC_PRISON_SOLARDEF_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_THRUSWARM },
                m_02toKill
            ) { SaveOnSuccess = true,StartDialogId = MyDialogueEnum.EAC_PRISON_0300};
            destroySolarDefence.OnMissionSuccess += DestroySolarDefenceOnOnMissionSuccess;
            m_objectives.Add(destroySolarDefence);
            destroySolarDefence.OnMissionLoaded += destroySolarDefence_OnMissionLoaded;

            //03
            m_destroySolarPanelsFirst = new MyObjective(
                (MyTextsWrapperEnum.EAC_PRISON_SOLAROFF1_Name),
                MyMissionID.EAC_PRISON_SOLAROFF1,
                (MyTextsWrapperEnum.EAC_PRISON_SOLAROFF1_Description),
                null,
                this,
                new MyMissionID[] {MyMissionID.EAC_PRISON_SOLARDEF},
                null
                ) {SaveOnSuccess = false};
            m_destroySolarPanelsFirst.OnMissionLoaded += DestroySolarPanelsFirstSubmissionLoaded;
            m_destroySolarPanelsFirst.OnMissionSuccess += DestroySolarPanelsFirstSubmissionSuccess;
            m_objectives.Add(m_destroySolarPanelsFirst);
            m_destroySolarPanelsFirst.SaveOnSuccess = true;

            //04
            var motherShipHelp = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_PRISON_MOTHERSHIPHELP_Name),
                MyMissionID.EAC_PRISON_MOTHERSHIPHELP,
                (MyTextsWrapperEnum.EAC_PRISON_MOTHERSHIPHELP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_SOLAROFF1 },
                m_04toKill
            ) { SaveOnSuccess = false, StartDialogId = MyDialogueEnum.EAC_PRISON_0400, SuccessDialogId = MyDialogueEnum.EAC_PRISON_0500};
            m_objectives.Add(motherShipHelp);
            motherShipHelp.OnMissionLoaded += new MissionHandler(motherShipHelp_OnMissionLoaded);

                        //05
            m_destroySolarPanelsSecond = new MyObjective(
                (MyTextsWrapperEnum.EAC_PRISON_SOLAROFF2_Name),
                MyMissionID.EAC_PRISON_SOLAROFF2,
                (MyTextsWrapperEnum.EAC_PRISON_SOLAROFF2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_MOTHERSHIPHELP },
                null
            ) { SaveOnSuccess = true,};
            m_destroySolarPanelsSecond.OnMissionLoaded += DestroySolarPanelsSecondSubmissionLoaded;
            m_destroySolarPanelsSecond.OnMissionSuccess += DestroySolarPanelsSecondSubmissionSuccess;
            m_objectives.Add(m_destroySolarPanelsSecond);
            
            //06
            var breakInsideStation = new MyTimedReachLocationObjective(
               (MyTextsWrapperEnum.EAC_PRISON_BREAKIN_Name),
               MyMissionID.EAC_PRISON_BREAKIN,
               (MyTextsWrapperEnum.EAC_PRISON_BREAKIN_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_PRISON_SOLAROFF2 },
               new System.TimeSpan(0, 1, 0),
               new MyMissionLocation(baseSector,(uint) EntityID.BreakInsideStationLocationDummy)
           ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_PRISON_0600, HudName = MyTextsWrapperEnum.HudCargoBay };
            breakInsideStation.OnMissionSuccess += BreakInsideStationSubmissionSuccess;
            breakInsideStation.OnMissionLoaded += BreakInsideStationOnOnMissionLoaded;
            m_objectives.Add(breakInsideStation);
            
            //07
            var marcusLocationIntel = new MyUseObjective(
                   (MyTextsWrapperEnum.EAC_PRISON_LOCINTEL_Name),
                   MyMissionID.EAC_PRISON_LOCINTEL,
                   (MyTextsWrapperEnum.EAC_PRISON_LOCINTEL_Description),
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.EAC_PRISON_BREAKIN },
                   new MyMissionLocation(baseSector, (uint)EntityID._MarcusIntelLocation),
                   MyTextsWrapperEnum.PressToDownloadData,
                   MyTextsWrapperEnum.Console,
                   MyTextsWrapperEnum.DownloadingData,
                   5000,
                   MyUseObjectiveType.Hacking
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudSecurityHub };

            m_objectives.Add(marcusLocationIntel);
            marcusLocationIntel.OnMissionLoaded += new MissionHandler(marcusLocationIntel_OnMissionLoaded);
            marcusLocationIntel.OnMissionSuccess += MarcusLocationIntelOnOnMissionSuccess;
            marcusLocationIntel.Components.Add(new MySpawnpointLimiter(m_spawns, 6));
            
            //08
            m_findCircoutPart = new MyUseObjective(
                    (MyTextsWrapperEnum.EAC_PRISON_ACQUIREIDCARD_Name),
                    MyMissionID.EAC_PRISON_ACQUIREIDCARD,
                    (MyTextsWrapperEnum.EAC_PRISON_ACQUIREIDCARD_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.EAC_PRISON_LOCINTEL},
                    new MyMissionLocation(baseSector, (uint)EntityID.CargoIDCard),
                    MyTextsWrapperEnum.PressToTakeCargo,
                    MyTextsWrapperEnum.TakeAll,
                    MyTextsWrapperEnum.TakeAll,
                    2000,
                    MyUseObjectiveType.Taking,
                    radiusOverride: 50
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_PRISON_0700, HudName = MyTextsWrapperEnum.HudIdCard };

            m_findCircoutPart.OnMissionLoaded += OnFindCircuitLoaded;
            m_findCircoutPart.OnMissionSuccess += new MissionHandler(m_findCircoutPart_OnMissionSuccess);
            m_objectives.Add(m_findCircoutPart);


            var marcusLocationIntel2 = new MyUseObjective(
               (MyTextsWrapperEnum.EAC_PRISON_LOCINTEL2_Name),
               MyMissionID.EAC_PRISON_LOCINTEL2,
               (MyTextsWrapperEnum.EAC_PRISON_LOCINTEL2_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_PRISON_ACQUIREIDCARD },
               new MyMissionLocation(baseSector, (uint)EntityID._MarcusIntelLocation),
               MyTextsWrapperEnum.PressToDownloadData,
               MyTextsWrapperEnum.Console,
               MyTextsWrapperEnum.DownloadingData,
               5000,
               MyUseObjectiveType.Hacking
            ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.EAC_PRISON_0900, StartDialogId = MyDialogueEnum.EAC_PRISON_0800, HudName = MyTextsWrapperEnum.HudSecurityHub };

            m_objectives.Add(marcusLocationIntel2);
            
            //09
            var openDoors = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_PRISON_OPENACCESS_Name),
                MyMissionID.EAC_PRISON_OPENACCESS,
                (MyTextsWrapperEnum.EAC_PRISON_OPENACCESS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_LOCINTEL2 },
                null,
                new List<uint> {(int)EntityID.SecurityHub},
                m_09toEnablePrefabs
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudSecurityHub };
            m_objectives.Add(openDoors);

            openDoors.OnMissionLoaded += OnOpenDoorsLoaded;
            
            //10
            var takeOffSecurityCircuit = new MyObjectiveDisablePrefabs(
              (MyTextsWrapperEnum.EAC_PRISON_SECURITYOFF_Name),
              MyMissionID.EAC_PRISON_SECURITYOFF,
              (MyTextsWrapperEnum.EAC_PRISON_SECURITYOFF_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_OPENACCESS },
                new List<uint> { (uint)EntityID.HubHack },
                new List<uint> { (uint)EntityID.HubHackGenerator },
                false,
                false
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_PRISON_1000, HudName = MyTextsWrapperEnum.HudHub };

            takeOffSecurityCircuit.OnMissionLoaded += OntakeOffSecuritypartLoaded;//zapne svetlo
            m_objectives.Add(takeOffSecurityCircuit);
            
            //11
            var approachMarcusCell = new MyUseObjective(
                (MyTextsWrapperEnum.EAC_PRISON_MARCUSCELL_Name),
               MyMissionID.EAC_PRISON_MARCUSCELL,
               (MyTextsWrapperEnum.EAC_PRISON_MARCUSCELL_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_PRISON_SECURITYOFF },
               new MyMissionLocation(baseSector, (uint)EntityID.MarcusCellLocation),
               MyTextsWrapperEnum.HoldToMoveMarcus,
               MyTextsWrapperEnum.Actor_Marcus,
               MyTextsWrapperEnum.Moving,
               4000,
               MyUseObjectiveType.Taking
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMarcus };
            approachMarcusCell.OnMissionLoaded += OnapproachMarcusLoaded;
            m_objectives.Add(approachMarcusCell);

            var marcusLoaded = new MyObjectiveDialog(
                MyMissionID.EAC_PRISON_MARCUSDIALOG,
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_MARCUSCELL },
                MyDialogueEnum.EAC_PRISON_1200
            ) { SaveOnSuccess = true };
            marcusLoaded.OnMissionLoaded += OnMarcusLoadedLoaded;
            m_objectives.Add(marcusLoaded);

            var weHaveCompany = new MyObjectiveDialog(
                MyMissionID.EAC_PRISON_WE_HAVE_COMPANY,
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_MARCUSDIALOG },
                MyDialogueEnum.EAC_PRISON_1250
            ) { SaveOnSuccess = false };
            weHaveCompany.OnMissionLoaded += OnWeHaveCompanyLoaded;
            m_objectives.Add(weHaveCompany);

            //12
            var findMarcusSmallShip = new MyUseObjective(
             (MyTextsWrapperEnum.EAC_PRISON_COVERMARCUS_Name),
             MyMissionID.EAC_PRISON_COVERMARCUS,
             (MyTextsWrapperEnum.EAC_PRISON_COVERMARCUS_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_PRISON_WE_HAVE_COMPANY },
               new MyMissionLocation(baseSector, (uint)EntityID.MarcusShipDetector),
               MyTextsWrapperEnum.HoldToMoveMarcus,
               MyTextsWrapperEnum.Actor_Marcus,
               MyTextsWrapperEnum.Moving,
               4000,
               MyUseObjectiveType.Putting
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudShip };

            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_objectives.Add(findMarcusSmallShip);
            findMarcusSmallShip.OnMissionSuccess += OnSuccesfindMarcusSmallShip;

            //13
            var equipMarcus = new MyUseObjective(
             (MyTextsWrapperEnum.EAC_PRISON_GETARMS_Name),
             MyMissionID.EAC_PRISON_GETARMS,
             (MyTextsWrapperEnum.EAC_PRISON_GETARMS_Description),
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.EAC_PRISON_COVERMARCUS },
                   new MyMissionLocation(baseSector, (uint)EntityID.CargoForMarcus),
                   MyTextsWrapperEnum.HoldToMoveWeapons,
                   MyTextsWrapperEnum.Actor_Marcus,
                   MyTextsWrapperEnum.Moving,
                   4000
             ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudEquipment };

            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_objectives.Add(equipMarcus);
            equipMarcus.OnMissionSuccess += OnEquipMarcussSucces;
            equipMarcus.OnMissionLoaded += EquipMarcusOnOnMissionLoaded;

            //14
            var fightOutStation = new MyObjective(
                    (MyTextsWrapperEnum.EAC_PRISON_FIGHTOUT_Name),
                MyMissionID.EAC_PRISON_FIGHTOUT,
                (MyTextsWrapperEnum.EAC_PRISON_FIGHTOUT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_GETARMS },
                new MyMissionLocation(baseSector, (uint)EntityID.StationDummy)
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_PRISON_1300, SuccessDialogId = MyDialogueEnum.EAC_PRISON_1400, HudName = MyTextsWrapperEnum.Nothing };
            fightOutStation.OnMissionLoaded += fightOutStation_OnMissionLoaded;
            fightOutStation.OnMissionSuccess += fightOutStation_OnMissionSuccess;
            m_objectives.Add(fightOutStation);

            //15
            var CrushRemainingShips = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_PRISON_CRUSHREINFORCEMENTS_Name),
                MyMissionID.EAC_PRISON_CRUSHREINFORCEMENTS,
                (MyTextsWrapperEnum.EAC_PRISON_CRUSHREINFORCEMENTS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_FIGHTOUT },
                new List<uint> { },         // entities needed to kill
                m_15toKillSpawnpoints,  // spawnpoint from which bots must be killed
                false                       // don't show marks on entities (not from spawnpoint)
            ) { SaveOnSuccess = true, };
            m_objectives.Add(CrushRemainingShips);
            
            //16
            var GetToMeetingPoint = new MyObjective(
                (MyTextsWrapperEnum.EAC_PRISON_MEETINGPOINT_Name),
                MyMissionID.EAC_PRISON_MEETINGPOINT,
                (MyTextsWrapperEnum.EAC_PRISON_MEETINGPOINT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_PRISON_CRUSHREINFORCEMENTS },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
              ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_PRISON_1500, SuccessDialogId = MyDialogueEnum.EAC_PRISON_1600, HudName = MyTextsWrapperEnum.HudMeetingPoint };
            m_objectives.Add(GetToMeetingPoint);
            
        }

        private void EquipMarcusOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 100, "KA06");
        }

        private void BreakInsideStationOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StealthAction, 100, "KA03");
        }

        private void MarcusLocationIntelOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTexts.DownloadFailed, MyGuiManager.GetFontMinerWarsRed(),5000));
        }

        void fightOutStation_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((uint)EntityID.CargoExitDoor), true);
        }

        void fightOutStation_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA17");
        }

        void m_findCircoutPart_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.TryHide((uint)EntityID.CargoIDCard);
        }

        void marcusLocationIntel_OnMissionLoaded(MyMissionBase sender)
        {
            //throw new NotImplementedException();
            MyScriptWrapper.DeactivateSpawnPoint(74982);
            MyScriptWrapper.DeactivateSpawnPoint(74981);
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.HeavyFight);
        }

        void motherShipHelp_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint(74982);
            MyScriptWrapper.ActivateSpawnPoint(74981);
        }

        void destroySolarDefence_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.DeactivateSpawnPoints(m_battleSpawnpoints);
        }

        void mothershipBattle_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoints(m_battleSpawnpoints);
            MyAudio.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA19");
        }

        private void OnEquipMarcussSucces(MyMissionBase sender)
        {
            if (m_marcus != null)
            {
                m_marcus.Inventory.AddInventoryItems(m_marcusInventory);
                m_marcusInventory.Clear();
            }
        }

           private void OnSuccesfindMarcusSmallShip(MyMissionBase sender)
           {

            if (m_marcus != null)
            {
                MyScriptWrapper.HideEntity(m_marcusPlacHolder);

                m_marcus.Enabled = true;
                m_marcus.Visible = true;
                m_marcus.ActiveAI = true;
                m_marcus.Follow(MySession.PlayerShip);

                var position = m_marcusPlacHolder.GetPosition();

                m_marcus.WorldMatrix = m_marcusPlacHolder.WorldMatrix;
            }

            MyScriptWrapper.Highlight((uint)EntityID.MarcusPlaceholder, false, this);
        }

        private void OnapproachMarcusLoaded(MyMissionBase sender)
        {
            var light = MyScriptWrapper.GetEntity(75005) as MyPrefabLight;

            if (light != null)
            {
                light.SetAllColors(new Color(1f, 0f, 0f));
                light.Effect = MyLightEffectTypeEnum.CONSTANT_FLASHING;
            }

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((int)EntityID.HubHackGenerator), false);
            var hub = MyScriptWrapper.GetEntity((int) EntityID.HubHackGenerator) as MyPrefabSecurityControlHUB;
            hub.UseProperties.HackType = MyUseType.None;
            hub.UseProperties.UseType = MyUseType.None;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA28");

            MyScriptWrapper.Highlight((uint)EntityID.MarcusPlaceholder, true, this);
        }

        private void OnMarcusLoadedLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA16");
        }

        private void OnWeHaveCompanyLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointAtWeHaveCompany);
        }
        
        private void OnOpenDoorsLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((int)EntityID.SecurityHub), true);
        }


        private void OntakeOffSecuritypartLoaded(MyMissionBase sender)
        {
            var light = MyScriptWrapper.TryGetEntity(75006) as MyPrefabLight;

            if (light != null)
            {
                light.SetAllColors(new Color(0f, 1f, 0f));
                light.Effect = MyLightEffectTypeEnum.DISTANT_GLARE;
            }
        }
        
        private void OnFindCircuitLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA27");
            MyScriptWrapper.Highlight((uint)EntityID.CargoIDCard, true, this);
            MyScriptWrapper.ActivateSpawnPoint(72524);
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity(75066));
            MyScriptWrapper.SetEntityEnabled((uint)EntityID.CargoIDCard,false);
        }
        
        public void DestroySolarDefenceOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA06");
        }

        public override void Load()
        {
            if (!IsMainSector)
                return;

            //add biochem ammo
            //MyScriptWrapper.GetPlayerInventory().AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem, 100f, true);
            //MyScriptWrapper.GetPlayerInventory().AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem, 100, true);
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem, 100);
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2, 1f, true);

            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnpointAtWeHaveCompany);

            /*
            MyMwcObjectBuilder_SmallShip_Player originalBuilder = MySession.PlayerShip.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player;
            originalBuilder.ShipType = MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG;
            MySession.PlayerShip.Close();
            MyEntities.CreateFromObjectBuilderAndAdd(null, originalBuilder, originalBuilder.PositionAndOrientation.GetMatrix());
              */
            m_panelsLeft = GetPanelsLeft((uint)EntityID.PanelsPrefabContainer);
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.HeavyFight);

            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.FourthReich, MyFactions.RELATION_BEST);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_WORST);
            MyScriptWrapper.FixBotNames();

            MyEntityDetector startDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint) EntityID.StartDetector));
            startDetector.OnEntityEnter += StartDummyDetector;
            startDetector.On();

            MyEntityDetector Security01Detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.SecurityDetector));
            Security01Detector.OnEntityEnter += StartSecurity01Detector;
            Security01Detector.On();

            MyEntityDetector Security02Detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.SecurityDetector2));
            Security02Detector.OnEntityEnter += StartSecurity02Detector;
            Security02Detector.On();

            MyScriptWrapper.EntityClosing += EntityClosing;

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.CargoExitDoor), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Doors1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable2), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Doors3), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Doors4), false);

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointSpawned;

            MyScriptWrapper.OnSentenceStarted += MyScriptWrapper_OnSentenceStarted;



            m_marcus = MyScriptWrapper.GetEntity("Marcus") as MySmallShipBot;

            //disable marcus 
            if (m_marcus != null)
            {
                //MyScriptWrapper.DisableShip(m_ma);
                m_marcus.Enabled = false;
                m_marcus.Visible = false;
                m_marcus.ActiveAI = false;
                MyScriptWrapper.StopFollow(m_marcus);
                m_marcusInventory.Clear();
                m_marcusInventory.AddRange(m_marcus.Inventory.GetInventoryItems());
                m_marcus.Inventory.ClearInventoryItems();
            }

            //disable ships for marcus
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.MarcusFakeShip1));
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.MarcusFakeShip2));
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.MarcusPlaceholder));
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.MarcusFakeShip3));


            m_marcusPlacHolder = MyScriptWrapper.GetEntity((uint)EntityID.MarcusPlaceholder);

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((int)EntityID.SecurityHub), false);

            InitDetector((uint)75094, StartEACDetector);

            //add Raven Girl and Raven Guy
            //We need to replace old vitolino+tanja, because they have deutch smallships from doppelburg

            m_ravenguyBot = MyScriptWrapper.GetEntity("RavenGuy") as MySmallShipBot;
            m_ravenguyBot.SetName("Disabled_vitolino");
            m_ravenguyBot.MarkForClose();

            m_ravenguyBot = MyScriptWrapper.InsertFriend(MyActorEnum.VALENTIN);
            m_ravenguyBot.LeaderLostEnabled = true;
            m_ravenguyBot.Follow(MySession.PlayerShip);
            Matrix ravenguyPosition = Matrix.CreateWorld(MySession.PlayerShip.GetPosition() - Vector3.Right * 15, MySession.PlayerShip.WorldMatrix.Forward, MySession.PlayerShip.WorldMatrix.Up);
            m_ravenguyBot.SetWorldMatrix(ravenguyPosition);

            m_ravengirlBot = MyScriptWrapper.GetEntity("RavenGirl") as MySmallShipBot;
            m_ravengirlBot.SetName("Disabled_tanja");
            m_ravengirlBot.MarkForClose();

            m_ravengirlBot = MyScriptWrapper.InsertFriend(MyActorEnum.TARJA);
            m_ravengirlBot.LeaderLostEnabled = true;
            m_ravenguyBot.Follow(MySession.PlayerShip);
            Matrix ravengirlPosition = Matrix.CreateWorld(MySession.PlayerShip.GetPosition() + Vector3.Right * 15, MySession.PlayerShip.WorldMatrix.Forward, MySession.PlayerShip.WorldMatrix.Up);
            m_ravengirlBot.SetWorldMatrix(ravengirlPosition);
            
            base.Load();
        }

        void MyScriptWrapper_OnSentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (dialogue == MyDialogueEnum.EAC_PRISON_1200 && sentence == MyDialoguesWrapperEnum.Dlg_EACPrison_1200)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA13");
            }

            if (dialogue == MyDialogueEnum.EAC_PRISON_1250 && sentence == MyDialoguesWrapperEnum.Dlg_EACPrison_1210)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA16");
            }
        }

        private void StartEACDetector(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            MyScriptWrapper.ActivateSpawnPoints(m_EACdetectorSpawns);
            
        }

        private MyEntityDetector InitDetector(uint detectorID, OnEntityEnter handler)
        {
            MyEntityDetector detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(detectorID));
            detector.OnEntityEnter += handler;
            detector.On();
            return detector;
        }

        private void OnSpawnpointSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (m_battleSpawnpoints.Contains(spawnpoint.EntityId.Value.NumericValue))
            {
                MyScriptWrapper.SetSleepDistance(bot, 10000);
            }
        }

        public override void Unload()
        {
            base.Unload();

            m_marcus = null;
            m_marcusPlacHolder = null;
            m_ravenguyBot = null;
            m_ravengirlBot = null;

            MyScriptWrapper.OnSentenceStarted -= MyScriptWrapper_OnSentenceStarted;
            MyScriptWrapper.EntityClosing -= EntityClosing;
            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointSpawned;
        }


        public override void Update()
        {
            if (!IsMainSector)
                return;

            base.Update();

            if (!m_playerShipReverted)
            {
                MyScriptWrapper.RevertShipFromInventory();
                m_playerShipReverted = true;
            }

            if (m_destroySolarPanelsFirst.IsAvailable() && m_panelsLeft <= m_totalPanelCount * (1 - DestroySolarPanelPercentsOne))
            {
                m_destroySolarPanelsFirst.Success();
            }

            if (m_destroySolarPanelsSecond.IsAvailable() && m_panelsLeft <= m_totalPanelCount * (1 - DestroySolarPanelPercentsTwo))
            {
                m_destroySolarPanelsSecond.Success();
            }
        }

        void AddPanelPercentNotification()
        {
            if (m_hudSolarPanelsCounter != null) m_hudSolarPanelsCounter.Disappear();
            m_hudSolarPanelsCounter = MyScriptWrapper.CreateNotification(
                    MyTextsWrapperEnum.SolarPanelsLeft,
                    MyHudConstants.MISSION_FONT,
                    0,
                    new object[] { (int)((1 - (float)m_panelsLeft / m_totalPanelCount) * 100) });
            MyScriptWrapper.AddNotification(m_hudSolarPanelsCounter);
        }

        void RemovePanelPercentNotification()
        {
            if (m_hudSolarPanelsCounter != null) m_hudSolarPanelsCounter.Disappear();
        }

        void EntityClosing(MyEntity entity)
        {
            MyPrefabBase prefab = entity as MyPrefabBase;
            if ((m_destroySolarPanelsFirst.IsAvailable() || m_destroySolarPanelsSecond.IsAvailable()) &&
                (prefab != null && entity is MyPrefabLight == false && prefab.Parent != null && prefab.Parent.EntityId.HasValue && prefab.Parent.EntityId.Value.NumericValue == 42938)
               )
            {
                --m_panelsLeft;
                AddPanelPercentNotification();
            }
        }

        private void StartDummyDetector(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.StartSpawn);
                sender.Off();
            }
        }

        private void StartSecurity01Detector(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SecuritySpawn1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SecuritySpawn2);
                sender.Off();
            }
        }

        private void StartSecurity02Detector(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoints(m_spawnsSecurityDetector);
                sender.Off();
            }
        }
        
        private void DestroySolarPanelsFirstSubmissionLoaded(MyMissionBase sender)
        {
            m_panelsLeft = GetPanelsLeft((uint)EntityID.PanelsPrefabContainer);
            AddPanelPercentNotification();
        }

        private void DestroySolarPanelsFirstSubmissionSuccess(MyMissionBase sender)
        {
            RemovePanelPercentNotification();
        }

        private void DestroySolarPanelsSecondSubmissionLoaded(MyMissionBase sender)
        {
            m_panelsLeft = GetPanelsLeft((uint)EntityID.PanelsPrefabContainer);
            AddPanelPercentNotification();
        }

        private void DestroySolarPanelsSecondSubmissionSuccess(MyMissionBase sender)
        {
            RemovePanelPercentNotification();
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable2), true);
        }



        void BreakInsideStationSubmissionSuccess(MyMissionBase sender)
        {
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyPanelsEnable2), false);
        }

        private int GetPanelsLeft(uint containerEntityId)
        {
            int panelsLeft = -2;  // apparently there are 13*9 panels and two non-panels :)
            MyPrefabContainer container = MyScriptWrapper.GetEntity(containerEntityId) as MyPrefabContainer;
            Debug.Assert(container != null);
            if (container != null)
            {
                foreach (var prefab in container.GetPrefabs())
                {
                    if (prefab is MyPrefabLight == false)
                    {
                        ++panelsLeft;
                    }
                }
            }
            return panelsLeft;
        }

        public override void Accept()
        {
            base.Accept();

            //Revert ship only once
            m_playerShipReverted = false;
        }
    }
}

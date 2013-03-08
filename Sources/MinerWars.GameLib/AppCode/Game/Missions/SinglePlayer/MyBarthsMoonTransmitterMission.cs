using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyBarthsMoonTransmitterMission : MyBarthsMoonMissionBase
    {
        private MyObjective m_lookBothHubs;
        private MyObjective m_lookBothHubs2;
        private MyObjective m_escape;
        private MyObjective m_learnToUseDrones;
        private MyObjective m_findWayToMoon;
        private MyObjective m_talkWithThomasBarth;
        private MyMultipleUseObjective m_needPart3;
        private MyObjectiveDestroy m_destroyComputer;
        private MyUseObjective m_enableGeneratorWithDrone1Mission;
        private MyUseObjective m_enableGeneratorWithDrone2Mission;

        private bool m_computerDownDialoguePlayed;
        private bool m_computerDestroydDialoguePlayed;
        private bool m_lookedThroughLeftCameras;
        private bool m_lookedThroughRightCameras;
        private MyEntity m_controlledDrone;
        private bool m_enableGeneratorWithDrone1;
        private int m_releaseDroneTime;
        private bool m_spawnpoint5Or6BotsKilled;
        private MyBlinkingObjects m_blinkingObjects;
        private MySmallShipBot m_thomasBarth;
        private MyHudNotification.MyNotification m_learnToUseDrone;
 
        #region EntityIDs
        private readonly List<uint> m_leftHubCameras = new List<uint>() { 2203571, 2203570 };
        private readonly List<uint> m_rightHubCameras = new List<uint>() { 2203572, 2203573, 2203574 };

        private readonly List<uint> m_turrets1Waves = new List<uint>() { 2203839, 2203844 };
        private readonly List<uint> m_turrets2Waves = new List<uint>() { 2203843, 2203840 };
        private readonly List<uint> m_turrets3Waves = new List<uint>() { 2203842, 2203841 };


        private readonly List<uint> m_turretsLights1 = new List<uint>() { 2502272, 2502269 };
        private readonly List<uint> m_turretsLights2 = new List<uint>() { 2502271, 2502267 };
        private readonly List<uint> m_turretsLights3 = new List<uint>() { 2502270, 2502268 };
                 
        private readonly List<uint> m_toDestroyAfterComputerDestroy = new List<uint> { 2502326, 2502323, 2502321, 2502319, 2502324, 2502328, 2502325, 2502322, 2502321, 2489803, 2489801, 2489810, 2489799, 2489804, 2489797, 2489798, 2489811, 2489800, 2489802, (uint)EntityID.ActivateGenerator };
        private readonly List<uint> m_deadShips = new List<uint> { 2577221, 2577234, 2577247, 2577260, 2577285, 2577286, 2577287, 2577288, 2577289, 2577302, 2582974 };
        private readonly List<uint> m_entitiesToHide = new List<uint> { 2708065, 2708068, 2708069, 2708066, 2708067, 2708074, 2708073, 2708070, 2708075, 2708076 };
        private readonly List<uint> m_whiteMarks = new List<uint>() { 2203568, 2584525, 2203582, 1976987, 2273178, 1976989, 1993795, 2580811, 2273178, 2322458 };
                 
        private readonly List<uint> m_turretsToActivate = new List<uint> { 1801016 };
        private readonly List<uint> m_hidePrefabs = new List<uint>() { 1793182, 1800664, 1946366, 1921567, 1934141, 1949974, 2584326, 1946253, 1921569, 1934140, 1946250, 2584325, 1921568, 1946252, 1921566 };
        private readonly List<uint> m_disableDummies = new List<uint>() { 1822189, 1918251, 1825161, 1918000, 1918001, 1917998, 1917997, 1917999, 1917661, 1816248, 1875664, 1822190, 1905545, 1902558, 1918252 };
        private readonly List<uint> m_fans = new List<uint>() { 2322386, 2322387, 2322384, 2322419, 2322410 };

        private readonly List<uint> m_turrets1 = new List<uint>() { };
        private readonly List<uint> m_turrets2 = new List<uint>() { };
        private readonly List<uint> m_turrets3 = new List<uint>() { };


        public override void ValidateIds()
        {
            base.ValidateIds();
            var list = new List<uint>();

            list.AddRange(m_leftHubCameras);
            list.AddRange(m_rightHubCameras);
            list.AddRange(m_turrets1);
            list.AddRange(m_turrets2);
            list.AddRange(m_turrets3);
            list.AddRange(m_turretsLights1);
            list.AddRange(m_turretsLights2);
            list.AddRange(m_turretsLights3);
            list.AddRange(m_toDestroyAfterComputerDestroy);
            list.AddRange(m_deadShips);
            list.AddRange(m_entitiesToHide);
            list.AddRange(m_whiteMarks);
            list.AddRange(m_turretsToActivate);
            list.AddRange(m_disableDummies);
            list.AddRange(m_hidePrefabs);
            list.AddRange(m_fans);

            foreach (var u in list)
            {
                MyScriptWrapper.GetEntity(u);
            }

        }
        #endregion

        public MyBarthsMoonTransmitterMission()
            : base(
            MyMissionID.BARTHS_MOON_TRANSMITTER,
            new StringBuilder("06-Barth's moon transmitter"),
            MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER,
            MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_Description,
            new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE },
            new EntityID[] { },
            EntityID.PlayerStartLocationTransmitter)
        {
            m_objectives = new List<MyObjective>();
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_RETURN_BACK_TO_MADELYN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };
            RequiredMissions = new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE };

            Components.Add(new MySpawnpointWaves((uint)EntityID.EnterMainLabDetector, 0,
                new List<uint[]> { new List<uint> { (uint)EntityID.SpawnPoint3 }.ToArray(), new List<uint> { (uint)EntityID.SpawnPoint4 }.ToArray() }));

            #region Objectives
            var meetThomasBarth = new MyObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_MEET_BARTH_Name),
                    MyMissionID.BARTHS_MOON_TRANSMITTER_MEET_BARTH,
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_MEET_BARTH_Description),
                    null,
                    this,
                    new MyMissionID[] { },
                    new MyMissionLocation(baseSector, (uint)EntityID.ThomasBartId),
                    radiusOverride: 50
                    ) { SaveOnSuccess = false, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0100_INTRO, HudName = MyTextsWrapperEnum.HudBarth };
            meetThomasBarth.OnMissionLoaded += MeetThomasBarthOnLoaded;
            m_objectives.Add(meetThomasBarth);


            m_talkWithThomasBarth = new MyBarthsMoonSubmissionTalkWithThomasBarth(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH_Description),
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_MEET_BARTH },
                null,
                true
                ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0300 };
            m_objectives.Add(m_talkWithThomasBarth);


            //02
            m_findWayToMoon = new MyObjective(
                 (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_WAY_TO_MOON_Name),
                 MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_WAY_TO_MOON,
                 (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_WAY_TO_MOON_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH },
                 new MyMissionLocation(baseSector, (uint)EntityID.BrokenMoonDummy),
                 null,
                 null,
                 MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0400
                 ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudBrokenMoon };
            m_objectives.Add(m_findWayToMoon);

            //03
            var destroyLaboratories = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DESTROY_LAB_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_DESTROY_LAB,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DESTROY_LAB_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_WAY_TO_MOON },
                new List<uint> { },         // entities needed to kill
                new List<uint> { },  // spawnpoint from which bots must be killed
                true,
                false // dont count 
                ) { SaveOnSuccess = true };
            destroyLaboratories.OnMissionLoaded += destroyLaboratories_OnMissionLoaded;
            destroyLaboratories.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog5DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0500));
            destroyLaboratories.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog6DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0600));
            m_objectives.Add(destroyLaboratories);

            //04


            var enableGenerator = new MyUseObjective(
                       (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_Name),
                   MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR,
                   (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_Description),
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DESTROY_LAB },
                   new MyMissionLocation(baseSector, (uint)EntityID.SecurityHubDummy),
                   MyTextsWrapperEnum.PressToStartGenerator,
                   MyTextsWrapperEnum.Generator,
                   MyTextsWrapperEnum.StartingProgress,
                   5000,
                   MyUseObjectiveType.Activating
                   ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudGenerator, ShowNavigationMark = true };
            enableGenerator.OnMissionSuccess += EnableGeneratorOnOnMissionSuccess;
            m_objectives.Add(enableGenerator);
            


            //05
            m_lookBothHubs = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_LOOK_HUBS_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_LOOK_HUBS,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_LOOK_HUBS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR },
                null,
                new List<uint> { (uint)EntityID.SecurityHubLook1 },
                null
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHub, MarkMissionEntities = true };
            m_objectives.Add(m_lookBothHubs);


            //06
            var getItemsFromCargoBoxes = new MyObjectiveGetItems(
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_GET_ITEMS_Name),
                    MyMissionID.BARTHS_MOON_TRANSMITTER_GET_ITEMS,
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_GET_ITEMS_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_LOOK_HUBS },
                    new List<MyItemToGetDefinition>() { new MyItemToGetDefinition(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS),
                    new MyItemToGetDefinition(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_3)},
                    new List<uint>() { (uint)EntityID.Cargobox1, (uint)EntityID.Cargobox2 }
                    ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0700, MarkMissionEntities = true };
            m_objectives.Add(getItemsFromCargoBoxes);




            var findFans = new MyObjective(
                 (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FAN_Name),
                 MyMissionID.BARTHS_MOON_TRANSMITTER_FAN,
                 (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FAN_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_GET_ITEMS },
                 new MyMissionLocation(baseSector, (uint)EntityID.FanDummy)
                 ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0800, HudName = MyTextsWrapperEnum.HudEntrance };
            m_objectives.Add(findFans);


            m_learnToUseDrones = new MyObjective(
                     (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DRONES_Name),
                     MyMissionID.BARTHS_MOON_TRANSMITTER_DRONES,
                     (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DRONES_Description),
                     null,
                     this,
                     new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_FAN },
                     null
                     ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0900 };
            m_objectives.Add(m_learnToUseDrones);

            m_learnToUseDrones.OnMissionLoaded += LearnDronesOnOnMissionLoaded;
            m_learnToUseDrones.OnMissionSuccess += LearnDronesOnOnMissionSuccess;



            //07
            m_enableGeneratorWithDrone1Mission = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE1_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE1,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DRONES },
                new MyMissionLocation(baseSector, (uint)EntityID.DroneGeneratroHub1),
                MyTextsWrapperEnum.PressToStartGenerator,
                MyTextsWrapperEnum.Generator,
                MyTextsWrapperEnum.StartingProgress,
                5000,
                radiusOverride: 50
                ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1100, HudName = MyTextsWrapperEnum.HudHub };

            m_enableGeneratorWithDrone1Mission.RequiresDrone = true;
            m_enableGeneratorWithDrone1Mission.OnMissionSuccess += EnableGeneratorWithDrone1OnOnMissionSuccess;
            m_enableGeneratorWithDrone1Mission.OnMissionLoaded += EnsureDronesInShip;
            m_enableGeneratorWithDrone1Mission.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog10DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1000));
            m_objectives.Add(m_enableGeneratorWithDrone1Mission);

            m_enableGeneratorWithDrone2Mission = new MyUseObjective(
                       (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE2_Name),
                       MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE2,
                       (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE2_Description),
                       null,
                       this,
                       new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE1 },
                       new MyMissionLocation(baseSector, (uint)EntityID.DroneGeneratroHub2),
                       MyTextsWrapperEnum.PressToStartGenerator,
                       MyTextsWrapperEnum.Generator,
                       MyTextsWrapperEnum.StartingProgress,
                       5000,
                       radiusOverride: 50
                       ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1200, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1400, RequiresDrone = true, HudName = MyTextsWrapperEnum.HudHub };
            m_enableGeneratorWithDrone2Mission.OnMissionSuccess += EnableGeneratorWithDrone2OnOnMissionSuccess;
            m_enableGeneratorWithDrone2Mission.OnMissionLoaded += EnsureDronesInShip;
            m_enableGeneratorWithDrone2Mission.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog13DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1300));
            m_objectives.Add(m_enableGeneratorWithDrone2Mission);

            //05
            m_lookBothHubs2 = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_LOOK_HUBS2_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_LOOK_HUBS2,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_LOOK_HUBS2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE2 },
                null,
                new List<uint> { (uint)EntityID.SecurityHubLook2 }
                ) { SaveOnSuccess = true, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1500, HudName = MyTextsWrapperEnum.HudHub, ShowNavigationMark = true };
            m_objectives.Add(m_lookBothHubs2);

            //08


            var downloadData1 = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA1_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA1,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_LOOK_HUBS2 },
                new MyMissionLocation(baseSector, (uint)EntityID.Hub1Dummy),
                MyTextsWrapperEnum.PressToDownloadData,
                MyTextsWrapperEnum.Console,
                MyTextsWrapperEnum.DownloadingData,
                3000,
                MyUseObjectiveType.Activating,
                null) { HudName = MyTextsWrapperEnum.HudHub };
            downloadData1.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog17DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1700));
            m_objectives.Add(downloadData1);

            var downloadData2 = new MyUseObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA2_Name),
                    MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA2,
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA2_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA1 },
                    new MyMissionLocation(baseSector, (uint)EntityID.Hub2Dummy),
                    MyTextsWrapperEnum.PressToDownloadData,
                    MyTextsWrapperEnum.Console,
                    MyTextsWrapperEnum.DownloadingData,
                    3000,
                    MyUseObjectiveType.Hacking,
                    null) { SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1800, HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(downloadData2);

            var downloadData3 = new MyUseObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA3_Name),
                    MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA3,
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA3_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA2 },
                    new MyMissionLocation(baseSector, (uint)EntityID.Hub3Dummy),
                    MyTextsWrapperEnum.PressToDownloadData,
                    MyTextsWrapperEnum.Console,
                    MyTextsWrapperEnum.DownloadingData,
                    3000,
                    MyUseObjectiveType.Hacking,
                    null) { SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2000, HudName = MyTextsWrapperEnum.HudHub };
            downloadData3.RequiresDrone = true;
            downloadData3.OnMissionLoaded += DownloadData3OnOnMissionLoaded;
            downloadData3.OnMissionSuccess += DownloadData3OnOnMissionSuccess;
            downloadData3.Components.Add(new MyDetectorDialogue((uint)EntityID.Dialog19DetectorId, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1900));
            m_objectives.Add(downloadData3);

            var downloadData4 = new MyUseObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA4_Name),
                    MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA4,
                    (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA4_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA3 },
                    new MyMissionLocation(baseSector, (uint)EntityID.Hub4Dummy),
                    MyTextsWrapperEnum.PressToDownloadData,
                    MyTextsWrapperEnum.Console,
                    MyTextsWrapperEnum.DownloadingData,
                    3000) { HudName = MyTextsWrapperEnum.HudHub };
            downloadData4.OnMissionLoaded += EnsureDronesInShip;
            m_objectives.Add(downloadData4);

            var entermainLab = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENTER_MAINLAB_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_ENTER_MAINLAB,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_ENTER_MAINLAB_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA4 },
                new MyMissionLocation(baseSector, (uint)EntityID.EnterMainLabDummy)
                ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2100, HudName = MyTextsWrapperEnum.HudLab };
            entermainLab.OnMissionSuccess += EntermainLabOnOnMissionSuccess;
            m_objectives.Add(entermainLab);


            //10
            m_destroyComputer = new MyObjectiveDestroy(
               (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DESTROY_COMPUTER_Name),
               MyMissionID.BARTHS_MOON_TRANSMITTER_DESTROY_COMPUTER,
               (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_DESTROY_COMPUTER_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_ENTER_MAINLAB },
               new List<uint> { (uint)EntityID.DestroyComputer },         // entities needed to kill
               new List<uint> { },  // spawnpoint from which bots must be killed
               true,
               false // dont count 
               ) { SaveOnSuccess = false, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2200/*, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2400*/, HudName = MyTextsWrapperEnum.HudBlondi };
            m_destroyComputer.OnMissionSuccess += DestroyComputerOnOnMissionSuccess;
            m_destroyComputer.OnMissionLoaded += DestroyComputerOnOnMissionLoaded;
            m_destroyComputer.Components.Add(new MySpawnpointWaves((uint)EntityID.EnterMainLabDummy, 0,
                new List<uint[]>
                    {
                        new List<uint> { (uint)EntityID.SpawnPoint5, (uint)EntityID.SpawnPoint6 }.ToArray(), 
                        new List<uint> { (uint)EntityID.SpawnPoint7, (uint)EntityID.SpawnPoint8 }.ToArray()
                    }));
            m_objectives.Add(m_destroyComputer);


            m_needPart3 = new MyMultipleUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_PART3_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_PART3,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_PART3_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_DESTROY_COMPUTER },
                MyTextsWrapperEnum.PressToTakeComponent,
                MyTextsWrapperEnum.Component,
                MyTextsWrapperEnum.TakingInProgress,
                1000,
                new List<uint>() { (uint)EntityID.CollectPart1Dummy, (uint)EntityID.CollectPart2Dummy, (uint)EntityID.CollectPart3Dummy },
                MyUseObjectiveType.Taking) { SaveOnSuccess =  true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2500, HudName = MyTextsWrapperEnum.HudPart };
            m_needPart3.OnMissionSuccess += NeedPart3OnOnMissionSuccess;
            m_needPart3.OnObjectUsedSucces += OnObjectUsedSucces;
            m_needPart3.OnMissionLoaded += NeedPart1OnOnMissionLoaded;
            m_objectives.Add(m_needPart3);


            m_escape = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_WAY_OUT_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_WAY_OUT,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_FIND_WAY_OUT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_PART3 },
                new MyMissionLocation(baseSector, (uint)EntityID.EscapeDummy)
                ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2600, SuccessDialogId = MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3100, HudName = MyTextsWrapperEnum.Nothing };
            m_escape.OnMissionSuccess += Escape_Success;
            m_escape.Components.Add(new MyDetectorDialogue((uint)EntityID.EscapeEventDummy, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2700));
            m_escape.Components.Add(new MyDetectorDialogue((uint)EntityID.EscapeDummy, MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2900));
            m_escape.OnMissionLoaded += EscapeOnOnMissionLoaded;
            m_escape.OnMissionCleanUp += EscapeOnOnMissionCleanUp;
            m_objectives.Add(m_escape);
            

            var build = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_FIND_WAY_OUT },
                new MyMissionLocation(baseSector, (uint)EntityID.BuildDetector4),
                MyTextsWrapperEnum.PressToBuildTransmitter,
                MyTextsWrapperEnum.Transmitter,
                MyTextsWrapperEnum.BuildingInProgress,
                3000,
                MyUseObjectiveType.Building);
            build.OnMissionSuccess += Build_Success;
            build.OnMissionLoaded += Build_Loaded;
            build.Components.Add(new MySpawnpointWaves((uint)EntityID.EscapeDummy, 0,
                                                       new List<uint[]>
                                                           {
                                                               new List<uint> {(uint) EntityID.SpawnPoint9}.ToArray(),
                                                               new List<uint>
                                                                   {
                                                                       (uint) EntityID.SpawnPoint10,
                                                                       (uint) EntityID.SpawnPoint11,
                                                                       (uint) EntityID.SpawnPoint12
                                                                   }.ToArray()
                                                           }));
            build.Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorWayBack2, 0,
                                                       new List<uint[]>
                                                           {
                                                               new List<uint> {(uint) EntityID.SpawnPoint13}.ToArray(),
                                                               new List<uint> {(uint) EntityID.SpawnPoint14}.ToArray()
                                                           }));
            m_objectives.Add(build);

            var backToMadelyn = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_RETURN_BACK_TO_MADELYN_Name),
                MyMissionID.BARTHS_MOON_TRANSMITTER_RETURN_BACK_TO_MADELYN,
                (MyTextsWrapperEnum.BARTHS_MOON_TRANSMITTER_RETURN_BACK_TO_MADELYN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                startDialogId: MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3200,
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            m_objectives.Add(backToMadelyn);

            //var talkWithThomasBarthEnd = new MyMeetObjective(
            //    new StringBuilder("Talk with Thomas Barth"),
            //    MyMissionID.BARTHS_MOON_TRANSMITTER_TALK_WITH_THOMAS_BARTH_END,
            //    new StringBuilder(""),
            //    this,
            //    new MyMissionID[] { MyMissionID.BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER },
            //    null,
            //    (uint)EntityID.ThomasBartId,
            //    50,
            //    0.25f
            //) { SaveOnSuccess = true, FollowMe = false };
            //talkWithThomasBarthEnd.OnMissionSuccess += TalkWithThomasBarthEnd_Success;
            //m_objectives.Add(talkWithThomasBarthEnd);


            #endregion
        }

        private void Build_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetWaypointListSecrecy(new List<uint>() { (uint)EntityID.WayPoint }, true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.WayPoint), false);

            SetDummyVisibleStatus(EntityID.BuildDetector4, true);
            SetDummyVisibleStatus(EntityID.BuildPlatform4, true);
            SetContainerVisibleStatus(EntityID.BuildContainer4, true);


            m_thomasBarth.SetWaypointPath("BarthWayo");
            m_thomasBarth.PatrolMode = MyPatrolMode.ONE_WAY;
            m_thomasBarth.Patrol();
        }

        private void EscapeOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SpawnpointBotsKilled += Script_SpawnpointBotsKilled;
        }

        private void EscapeOnOnMissionCleanUp(MyMissionBase sender)
        {
            MyScriptWrapper.SpawnpointBotsKilled += Script_SpawnpointBotsKilled;
        }

        private void Script_SpawnpointBotsKilled(MySpawnPoint spawnPoint)
        {
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnPointEscape)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2800);
            }

            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnPoint4)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3000);
            }
        }

        void TalkWithThomasBarthEnd_Success(MyMissionBase sender)
        {
            m_thomasBarth.LookTarget = null;
            m_thomasBarth.SetWaypointPath("BarthIn");
            m_thomasBarth.PatrolMode = MyPatrolMode.ONE_WAY;
            m_thomasBarth.SpeedModifier = 0.5f;
            m_thomasBarth.Patrol();
        }

        private void OnObjectUsedSucces(uint entity)
        {
            if (entity == (uint)EntityID.CollectPart3Dummy)
            {
                MyScriptWrapper.Highlight((uint)EntityID.CollectPart3, false, this);
                MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.CollectPart3));
            }
            else if (entity == (uint)EntityID.CollectPart2Dummy)
            {
                MyScriptWrapper.Highlight((uint)EntityID.CollectPart2, false, this);
                MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.CollectPart2));
            }
            else if (entity == (uint)EntityID.CollectPart1Dummy)
            {
                MyScriptWrapper.Highlight((uint)EntityID.CollectPart1, false, this);
                MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.CollectPart1));
            }
        }

        private void DownloadData3OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.ExitDrone, MyGuiManager.GetFontMinerWarsBlue(), 5000));

        }

        private void DownloadData3OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.EnterDrone, MyGuiManager.GetFontMinerWarsBlue(), 5000));
        }

        private void NeedPart1OnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.CollectPart1, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.CollectPart2, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.CollectPart3, true, this);
        }

        private void Build_Success(MyMissionBase sender)
        {
            var prefabCont = MyScriptWrapper.GetEntity((uint)EntityID.BuildContainer4);

            foreach (var child in prefabCont.Children)
            {
                MyScriptWrapper.Highlight(child.EntityId.Value.NumericValue, false, this);
                MyScriptWrapper.EnablePhysics(child.EntityId.Value.NumericValue, true);
            }

            //m_thomasBarth.Follow(MySession.PlayerShip);
            //m_thomasBarth.LookTarget = MySession.PlayerShip;
            //m_blinkingObjects.Enabled = false;
        }

        private void LearnDronesOnOnMissionSuccess(MyMissionBase sender)
        {
            if (m_learnToUseDrone != null)
            {
                m_learnToUseDrone.Disappear();
            }
        }

        private void LearnDronesOnOnMissionLoaded(MyMissionBase sender)
        {
            m_learnToUseDrone = MyScriptWrapper.CreateNotification(
                MyTextsWrapperEnum.HowToControlDrone,
                MyHudConstants.MISSION_FONT, 0,
                new object[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.DRONE_DEPLOY), MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.DRONE_CONTROL) }
            );
            MyScriptWrapper.AddNotification(m_learnToUseDrone);
        }

        private void Escape_Success(MyMissionBase sender)
        {
            ShowContainer(EntityID.HidePrefabCont);
            MyScriptWrapper.SetAlarmMode((uint)EntityID.AlarmContId, false);
        }

        private void DestroyComputerOnOnMissionLoaded(MyMissionBase sender)
        {
            ActivateTurretsObjects(m_turrets1);
            ActivateObjects(m_turretsLights1);
        }

        private void DestroyComputerOnOnMissionSuccess(MyMissionBase sender)
        {

            MyScriptWrapper.DestroyEntities(m_toDestroyAfterComputerDestroy);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.InvisibleDummy4), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DestroyCompParticleDummy), true);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPoint5);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPoint6);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPoint7);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPoint8);

            this.Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));

        }


        private void NeedPart3OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.EscapeDummy), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.DetectorWayBack2), true);
            //var x = MyScriptWrapper.TryGetEntity((uint) EntityID.Doors);
            var doors = MyScriptWrapper.TryGetEntity((uint)EntityID.Doors) as MyPrefabKinematic;
            if (doors != null)
            {
                doors.OrderToOpen();
                doors.Enabled = true;
            }
        }

        private void NeedPart2OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.CollectPart2));
        }

        private void NeedPart1OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.CollectPart1));
        }



        private void EntermainLabOnOnMissionSuccess(MyMissionBase sender)
        {
            var doors = (MyPrefabKinematic)MyScriptWrapper.GetEntity((uint)EntityID.Doors);
            doors.OrderToClose();
            doors.Enabled = false;
        }


        private void EnableGeneratorWithDrone1OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.Dronegenerator1 });

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Dronegenerator1particles), true);
            m_enableGeneratorWithDrone1 = true;
            m_releaseDroneTime = MissionTimer.ElapsedTime + 1000;

        }

        private void EnableGeneratorWithDrone2OnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.ExitDrone, MyGuiManager.GetFontMinerWarsGreen(), 4000));
            //deactivate inf. spheres
            ActivateObjects(new List<uint> { (uint)EntityID.InfluenceSphere1, (uint)EntityID.InfluenceSphere2, (uint)EntityID.InfluenceSphere3 }, false);
            ActivateObjects(new List<uint> { (uint)EntityID.ActivateDummyInfl, (uint)EntityID.DroneGenerator2, (uint)EntityID.ActivateGenerator, (uint)EntityID.ActiveDummy1, (uint)EntityID.ActiveDummy2, (uint)EntityID.ActiveDummy3, (uint)EntityID.ActiveDummy4 });
        }



        private void EnableGeneratorOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.EnableGenerator), true);
        }


        void destroyLaboratories_OnMissionLoaded(MyMissionBase sender)
        {
            var mission = sender as MyObjectiveDestroy;
            var doors = (MyPrefabKinematic)MyScriptWrapper.GetEntity((uint)EntityID.Destroyprefab);

            foreach (var doorParts in doors.Parts)
            {
                if (doorParts != null && doorParts.EntityId != null)
                    if (mission != null) mission.AddToKill(doorParts.EntityId.Value.NumericValue);
            }
        }



        private void MeetThomasBarthOnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.ShipFlameEffect), false);
            MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.GeneratorFlameEffect), false);
        }


        public override void Update()
        {
            base.Update();

            if (m_enableGeneratorWithDrone1 && MissionTimer.ElapsedTime > m_releaseDroneTime)
            {
                MyGuiScreenGamePlay.Static.ReleaseControlOfDrone();
                if (m_controlledDrone != null && m_controlledDrone.EntityId != null) MyScriptWrapper.DestroyEntities(new List<uint> { m_controlledDrone.EntityId.Value.NumericValue });
                m_enableGeneratorWithDrone1 = false;
            }
        }

        public override void Load()
        {
            EnableCorrectBarths((uint)EntityID.ThomasBartId, (uint)EntityID._01SmallShipBarth);

            MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged += Static_CameraContrlolledObjectChanged;
            MyScriptWrapper.EntityClosing += MyScriptWrapperOnEntityClosing;
            MyScriptWrapper.AlarmLaunched += MyScriptWrapperOnAlarmLaunched;
            MyScriptWrapper.SpawnpointBotsKilled += MyScriptWrapperOnSpawnpointBotsKilled;
            MyScriptWrapper.OnBotReachedWaypoint += MyScriptWrapperOnOnBotReachedWaypoint;
            MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnOnDialogueFinished;
            MyScriptWrapper.OnEntityAtacked += MyScriptWrapperOnEntityAttacked;

            if (!MyMissions.GetMissionByID(MyMissionID.BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH).IsCompleted())
            {
                var startPosition = MyScriptWrapper.GetEntity((uint)EntityID.PlayerStartLocationTransmitter).GetPosition();
                MyScriptWrapper.Move(MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN)), startPosition);
                MyScriptWrapper.MovePlayerAndFriendsToHangar(this.RequiredActors);
            }

            HideContainer(EntityID.InvisibleContainer1);
            HideContainer(EntityID.InvisibleContainer2);
            HideContainer(EntityID.InvisibleContainer3);
            //HideContainer(EntityID.HidePrefabCont);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID.BarthsMoon2DisableInfluenceSphere, false);
            MyScriptWrapper.TryHideEntities(m_hidePrefabs);
            MyScriptWrapper.SetEntitiesEnabled(m_disableDummies, false);
            //InitDetector((uint)EntityID.EnterMainLabDetector, StartMainLabDetector);
            MyScriptWrapper.OnSentenceStarted += MyScriptWrapper_OnSentenceStarted;

            MyEntityDetector detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.EscapeEventDummy));
            detector.OnEntityEnter += (OnEntityEnter)EscapeEvent;
            detector.On();


            foreach (uint deadShip in m_deadShips)
            {
                MyEntity shipEntity = MyScriptWrapper.TryGetEntity(deadShip);
                if (shipEntity != null)
                {
                    MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity(deadShip), false);
                    MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(deadShip));
                }
            }


            m_thomasBarth = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.ThomasBartId);
            m_thomasBarth.SpeedModifier = 0.5f;

            ActivateTurretsObjects(m_turrets1, false);
            ActivateTurretsObjects(m_turrets2, false);
            ActivateTurretsObjects(m_turrets3, false);


            foreach (var entity in m_whiteMarks)
            {
                var en = MyScriptWrapper.TryGetEntity(entity);

                if (en != null)
                {
                    var name = "";
                    if (en.Name != null) name = en.Name;
                    if (en.DisplayName != null) name = en.DisplayName;
                    //TODO: can't we just remove SHOW_TEXT?
                    MyScriptWrapper.MarkEntity(en, name, HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE, HUD.MyGuitargetMode.Neutral);
                }
            }


            m_computerDownDialoguePlayed = false;

            var prefabCont = MyScriptWrapper.GetEntity((uint)EntityID.BuildContainer4);

            foreach (var child in prefabCont.Children)
            {
                if (child.Physics != null) child.Physics.Enabled = false;
                MyScriptWrapper.Highlight(child.EntityId.Value.NumericValue, true, this);
            }

            m_thomasBarth.SetWaypointPath("interior2");
            m_thomasBarth.PatrolMode = MyPatrolMode.CYCLE;
            m_thomasBarth.Patrol();

            MyScriptWrapper.TryHideEntities(m_entitiesToHide);
            //MyScriptWrapper.TryHideEntities(new List<uint>(){(uint)EntityID._01SmallShipBarth});

            m_turrets1.Clear();
            m_turrets1.AddRange(m_turrets1Waves);
            m_turrets2.Clear();
            m_turrets2.AddRange(m_turrets2Waves);
            m_turrets3.Clear();
            m_turrets3.AddRange(m_turrets3Waves);

            //MyScriptWrapper.GetEntity("Madelyn").SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.BarthsMoon2MadelynLocation).WorldMatrix);
            //MySession.PlayerShip.SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.BarthsMoon2StartLocation).WorldMatrix);

            LoadFanPhysics();

            m_lookedThroughLeftCameras = false;
            m_lookedThroughRightCameras = false;
            base.Load();
        }

        void MyScriptWrapper_OnSentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (dialogue == MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3200 && sentence == MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3210)
            {
                MyScriptWrapper.ApplyTransition(Audio.MyMusicTransitionEnum.Special, 100, "KA08", false);
            }
        }


        private void LoadFanPhysics()
        {
            foreach (var fan in m_fans)
            {
                EnableTriangleLod0Physics(fan);
            }
        }

        private void EnableTriangleLod0Physics(uint fan)
        {
            var fanPrefab = MyScriptWrapper.TryGetEntity(fan);
            if (fanPrefab != null && fanPrefab.ModelLod0 != null)
            {
                MyScriptWrapper.EnablePhysics(fan, false);
                fanPrefab.InitTrianglePhysics(MyMaterialType.METAL, 1000, fanPrefab.ModelLod0, fanPrefab.ModelLod0);
            }
        }

        private void MyScriptWrapperOnEntityAttacked(MyEntity attacker, MyEntity target)
        {
            if (target.EntityId != null && (target.EntityId.Value.NumericValue == (uint)EntityID.DestroyComputer && m_destroyComputer.IsAvailable()))
            {
                if (!m_computerDownDialoguePlayed && target.HealthRatio <= 0.4f)
                {
                    m_computerDownDialoguePlayed = true;
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2300);
                }
                else if (!m_computerDestroydDialoguePlayed && target.HealthRatio <= 0.1f)
                {
                    m_computerDestroydDialoguePlayed = true;
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2400);

                }
            }
        }

        private void MyScriptWrapperOnOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0400 && m_findWayToMoon.IsAvailable())
            {
                m_thomasBarth.LookTarget = null;
                m_thomasBarth.SetWaypointPath("interior2");
                m_thomasBarth.Patrol();
            }
            if (dialogue == MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2400)
            {
                MyScriptWrapper.DestroyEntity((uint)EntityID.DestroyComputer);
            }
            if (dialogue == MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0100_INTRO)
            {
                MissionTimer.RegisterTimerAction(2000, () => MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0200), false);
            }

        }

        private void MyScriptWrapperOnOnBotReachedWaypoint(MyEntity entity1, MyEntity entity2)
        {
            if (entity2.EntityId != null && (entity1 == m_thomasBarth && entity2.EntityId.Value.NumericValue == (uint)EntityID.WayForbarthLastWayPoint))
            {
                m_thomasBarth.PatrolMode = MyPatrolMode.CYCLE;
                m_thomasBarth.SetWaypointPath("BarthTrans");
                m_thomasBarth.Patrol();
            }
        }

        private void EscapeEvent(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            if (MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives != null && MyMissions.ActiveMission.ActiveObjectives.Contains(m_escape))
            {
                MyScriptWrapper.AddExplosions(new List<uint>() { (uint)EntityID.EscapeEventDummyToActive1, (uint)EntityID.EscapeEventDummyToActive2 }, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 100);

                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.ParticleDummy1), false);
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.ParticleDummy2), false);
                ShowContainer(EntityID.InvisibleContainer3);
                MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2203550, 100, MyMwcVoxelMaterialsEnum.Stone_03, MyMwcVoxelHandModeTypeEnum.SUBTRACT);
                MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2203550, 61, MyMwcVoxelMaterialsEnum.Stone_03, MyMwcVoxelHandModeTypeEnum.SUBTRACT);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointEscape);

                this.Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));
            }

        }



        private void MyScriptWrapperOnSpawnpointBotsKilled(MySpawnPoint spawnPoint)
        {
            if (spawnPoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnPoint1)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPoint2);
            }
        }



        private void MyScriptWrapperOnAlarmLaunched(MyEntity entity1, MyEntity entity2)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPoint1);

            ActivateObjects(m_turretsToActivate);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1600);

        }

        private void MyScriptWrapperOnEntityClosing(MyEntity entity)
        {
            if (entity.EntityId == null)
                return;

            if (entity.EntityId.Value.NumericValue == (uint)EntityID.TurretId)
            {
                BoomAfterTurretDestroy();
            }

            if (entity is MyPrefabLargeWeapon)
            {
                if (m_turrets1.Contains(entity.EntityId.Value.NumericValue))
                {
                    m_turrets1.Remove(entity.EntityId.Value.NumericValue);
                }

                if (m_turrets2.Contains(entity.EntityId.Value.NumericValue))
                {
                    m_turrets2.Remove(entity.EntityId.Value.NumericValue);
                }

                if (m_turrets1.Count == 0) { ActivateTurretsObjects(m_turrets2); ActivateObjects(m_turretsLights2); }
                if (m_turrets2.Count == 0) { ActivateTurretsObjects(m_turrets3); ActivateObjects(m_turretsLights3); }
            }
        }



        private void BoomAfterTurretDestroy()
        {
            //MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.turretDestroy1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.turretDestroy2), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.turretDestroy3), true);

            ShowContainer(EntityID.InvisibleContainer2);
            //MyScriptWrapper.DestroyEntities(new List<uint>{(uint)EntityID.TurretDestroyprefab});

            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103326, 100, MyMwcVoxelMaterialsEnum.Stone_03);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103327, 100, MyMwcVoxelMaterialsEnum.Stone_03);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103328, 100, MyMwcVoxelMaterialsEnum.Stone_03);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103329, 100, MyMwcVoxelMaterialsEnum.Stone_03);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103330, 61, MyMwcVoxelMaterialsEnum.Stone_03);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, 2103331, 61, MyMwcVoxelMaterialsEnum.Stone_03);

            this.Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));
        }

        void Static_CameraContrlolledObjectChanged(MyEntity e)
        {
            if (MyMissions.ActiveMission.ActiveObjectives.Contains(m_lookBothHubs))
            {
                if (m_leftHubCameras.Contains(e.EntityId.Value.NumericValue)) m_lookedThroughLeftCameras = true;

                if (m_lookedThroughLeftCameras)
                {
                    m_lookBothHubs.Success();
                }
            }

            if (MyMissions.ActiveMission.ActiveObjectives.Contains(m_lookBothHubs2))
            {
                if (m_rightHubCameras.Contains(e.EntityId.Value.NumericValue)) m_lookedThroughRightCameras = true;

                if (m_lookedThroughRightCameras)
                {
                    m_lookBothHubs2.Success();
                }
            }


            if (e is MyDrone)
            {
                m_controlledDrone = e;
                if (m_learnToUseDrones.IsAvailable())
                {
                    m_learnToUseDrones.Success();
                }
            }

        }

        public override void Unload()
        {
            base.Unload();

            m_thomasBarth = null;
            m_controlledDrone = null;

            MyScriptWrapper.OnSentenceStarted -= MyScriptWrapper_OnSentenceStarted;
            MyGuiScreenGamePlay.Static.CameraContrlolledObjectChanged -= Static_CameraContrlolledObjectChanged;
            MyScriptWrapper.EntityClosing -= MyScriptWrapperOnEntityClosing;
            MyScriptWrapper.AlarmLaunched -= MyScriptWrapperOnAlarmLaunched;
            MyScriptWrapper.SpawnpointBotsKilled -= MyScriptWrapperOnSpawnpointBotsKilled;
            MyScriptWrapper.OnBotReachedWaypoint -= MyScriptWrapperOnOnBotReachedWaypoint;
            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnOnDialogueFinished;
            MyScriptWrapper.OnEntityAtacked -= MyScriptWrapperOnEntityAttacked;
        }


        private void ActivateObjects(List<uint> objects, bool activate = true)
        {
            foreach (var u in objects)
            {
                var entity = MyScriptWrapper.TryGetEntity(u);
                if (entity != null)
                {
                    MyScriptWrapper.SetEntityEnabled(entity, activate);
                }
            }
        }


        private void ActivateTurretsObjects(List<uint> objects, bool activate = true)
        {
            foreach (var u in objects)
            {
                var entity = MyScriptWrapper.TryGetEntity(u);
                if (entity != null)
                {
                    MyScriptWrapper.SetEntityEnabled(entity, activate);
                    entity.Visible = activate;
                    if (!activate)
                    {
                        MyScriptWrapper.RemoveEntityMark(entity);
                    }
                    else
                    {
                        MyScriptWrapper.MarkEntity(entity, "", guiTargetMode: MyGuitargetMode.Enemy);
                    }
                }
            }
        }


        private void EnsureDronesInShip(MyMissionBase sender)
        {
            // there is no drone in player's or session's inventory, then add 10 drones to session's inventory, because player neeeds them for completing mission
            MyScriptWrapper.EnsureInventoryItem(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS, 10f);
        }


    }
}

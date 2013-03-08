using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio;
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
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.World.Global;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyBarthsMoonPlantMission : MyBarthsMoonMissionBase
    {

        #region EntityIDs
        private readonly List<uint> m_barthsMoonM1 = new List<uint>() { 1800664, 1793182 };
        private readonly List<uint> m_barthsMoonM2 = new List<uint>() { 1958806, 2203565, 2580779, 2203552, 2103332, 2577321 };
        private readonly List<uint> m_particleEffectsToEnable = new List<uint>() { 2700930, 2700931, 2700933, 2700932, 2700934, 2700935, 2700936, 2700942, 2700992, 2700991, 2700990, 2701048, 2701088, 2701280 };
        private readonly List<uint> m_particleEffectsToDisable = new List<uint>() { 1677229, 1734884, 1697589, 1687418 };
        private readonly List<uint> m_02SpawnPoints = new List<uint>() { 2696600, 2696597, 2696599 };
        private readonly List<uint> m_05SpawnPoints = new List<uint>() { 2695664, 2695663, 2695660, 2695659, 2695656, 2695658, 2695665, 2695666, 2695662, 2695661 };
        private readonly List<uint> m_06SpawnPoints = new List<uint>() { 2701423, 2701422, 2701391, 2701392, 2701426 };
        private readonly List<uint> m_06ParticleDummies = new List<uint>() { 2701689, 2702880, 2702874 };

        private readonly List<uint> m_06BombsContainersIds = new List<uint>() { 2694990, 2694915, 2686986, 2694965, 2694940 };
        private readonly List<uint> m_06BombsTargetDummy = new List<uint>() { 2695015, 2695016, 2695017, 2695018, 2695019 };
        private readonly List<uint> m_06BombsParticleDummy = new List<uint>() { 2701444, 2701445, 2701446, 2701447, 2701448 };
        private readonly List<uint> m_06BombsPrefabsIDs = new List<uint>() { 2694993, 2694918, 2686989, 2694968, 2694943 };
        private readonly List<uint> m_06BombsFlyTimes = new List<uint>() { 40 * 1000, 40 * 1000, 40 * 1000, 40 * 1000, 40 * 1000 };
        private readonly List<uint> m_06BombsTimeOffsets = new List<uint>() { 5000, 35 * 1000, 45 * 1000, 75 * 1000, 90 * 1000 };

        private readonly List<uint> m_08Generator1Particles = new List<uint>() { 2703126, 2703127, 2703128, 2703125, 2703132 };
        private readonly List<uint> m_08Generator2Particles = new List<uint>() { 2702950, 2702949, 2702951, 2702947, 2702954, 2701439 };
        private readonly List<uint> m_08SpawnPoints1 = new List<uint>() { 2695653, 2695654, 2695651, 2695652 };
        private readonly List<uint> m_08SpawnPoints2 = new List<uint>() { 2697733 };
        private readonly List<uint> m_hidePrefabContainers = new List<uint>() { 2577321, 2203565, 1958806, 2103332, 2203552, 2580779, 1921382, 1800664, 1793182 };
        private readonly List<uint> m_spawns = new List<uint>() { 2695664, 2695663, 2695660, 2695659, 2695656, 2695658, 2695665, 2695666, 2695662, 2695661, 2701423, 2701422, 2701391, 2701392, 2701426, 2695653, 2695654, 2695651, 2695652, 2697733 };

        private readonly List<uint> m_setVisible = new List<uint>() { 2708067, 2708074, 2708070, 2708068, 2708065, 2708076, 2708066, 2723725, 2708069, 2708073 };
        private readonly List<uint> m_setInVisible = new List<uint>() { 2203565, 1958806, 2328595, 2577321, 1793182, 1800664, 1946366, 1921567, 1934141, 1949974, 2584326, 1946253, 1921569, 1934140, 1946250, 2584325, 1921568, 1946252, 1921566 };
        private readonly List<uint> m_disableDummy = new List<uint>() { 1822189, 1918251, 1825161, 1918000, 1918001, 1917998, 1917997, 1917999, 1917661, 1816248, 1875664, 1822190, 1905545, 1902558, 1918252 };



        public override void ValidateIds()
        {
            if (!IsMainSector) return;

            base.ValidateIds();

            var list = new List<uint>();
            list.AddRange(m_barthsMoonM1);
            list.AddRange(m_barthsMoonM2);
            list.AddRange(m_particleEffectsToDisable);
            list.AddRange(m_particleEffectsToEnable);
            list.AddRange(m_02SpawnPoints);
            list.AddRange(m_05SpawnPoints);
            list.AddRange(m_06SpawnPoints);
            list.AddRange(m_06ParticleDummies);

            list.AddRange(m_06BombsContainersIds);
            list.AddRange(m_06BombsTargetDummy);
            list.AddRange(m_06BombsParticleDummy);
            list.AddRange(m_06BombsPrefabsIDs);
            list.AddRange(m_08Generator1Particles);
            list.AddRange(m_08Generator2Particles);
            list.AddRange(m_08SpawnPoints1);
            list.AddRange(m_08SpawnPoints2);
            list.AddRange(m_hidePrefabContainers);
            list.AddRange(m_spawns);
            list.AddRange(m_setVisible);
            list.AddRange(m_setInVisible);
            list.AddRange(m_disableDummy);

            foreach (var u in list)
            {
                MyScriptWrapper.GetEntity(u);
            }
        }
        #endregion

        private MyEntity m_05PirateShip;
        private MyEntity m_05PirateBigShip;
        private MyEntityDetector m_attackBarthDetector;

        private MySmallShipBot m_barth;

        private readonly Dictionary<uint, uint> m_turretContainerToDetectorMapping = new Dictionary<uint, uint>();
        private readonly Dictionary<uint, uint> m_detectorToTurretContainerMapping = new Dictionary<uint, uint>();
        private readonly Dictionary<uint, uint> m_detectorToParticleMapping = new Dictionary<uint, uint>();

        private MyTimedObjective m_protectBarth;
        private MyTimedObjective m_protectMadelyn;
        private MyTimedObjective m_buildDefenses;
        private MyTimedObjective m_lastStand;
        private MyObjectiveDestroy m_destroyGenerators;
        private MyMeetObjective m_barthEndDialogue;
        private MyUseObjective m_10startGenerator;
        private MyUseObjective m_11placeBomb;
        private MyMeetObjective m_12talkWithThomasBarthEnd;

        private object[] m_actionKeyString = new object[1];

        private MyHudNotification.MyNotification m_canBuildNotification;
        private MyHudNotification.MyNotification m_remainingTurretsNotification;
        //private MyHudNotification.MyNotification m_canSkipNotification;
        private MyHudNotification.MyNotification m_canRepairNotification;
        
        private MyGuiScreenUseProgressBar m_buildProgress;
        private MyGuiScreenUseProgressBar m_repairProgress;

        private const int NUMBER_OF_TURRETS_TO_BUILD = 4;

        private int m_turretsBuilt = 0;
        private bool m_destroySuicideDialoguePlayed;

        private bool m_building;
        private bool m_repairing;

        private MyEntityDetector m_currentDetector;
        private MyEntityDetector m_06AttackDetector;

        private List<MyLine> m_06BombsTrajectories;
        private List<MyEntity> m_06BombsContainers;
        private List<bool> m_06BombsDeath;
        private List<float> m_06BombsProgresses;
        private List<bool> m_06BombsMarked;

        public MyBarthsMoonPlantMission()
            : base(MyMissionID.BARTHS_MOON_PLANT,
                   new StringBuilder("15-Barth's moon plant"),
                   MyTextsWrapperEnum.BARTHS_MOON_PLANT,
                   MyTextsWrapperEnum.BARTHS_MOON_PLANT_Description,
                   new MyMissionID[] { MyMissionID.RIFT },
                   new EntityID[] { EntityID.BuildContainer4 },
                   EntityID.PlayerStartLocationPlant)
        {
            m_objectives = new List<MyObjective>();
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_TALK_BARTH };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.VALENTIN, MyActorEnum.TARJA };
            RequiredMissions = new MyMissionID[] { MyMissionID.RIFT };

            #region Objectives
            var saveThomas = new MyTimedMeetObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_SAVE_BARTH_Name),
                MyMissionID.BARTHS_MOON_PLANT_SAVE_BARTH,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_SAVE_BARTH_Description),
                this,
                new MyMissionID[] { },
                (uint)EntityID._01Detector,
                (uint)EntityID._01SmallShipBarth,
                100,
                0.25f,
                new TimeSpan(0, 1, 30),
                startDialogueId: MyDialogueEnum.BARTHS_MOON_PLANT_0100,
                successDialogueId: MyDialogueEnum.BARTHS_MOON_PLANT_0200,
                stopFollow: false
                ) { SaveOnSuccess = true, PathName = "BarthEs" };
            saveThomas.Components.Add(new MyMovingEntity((uint)EntityID._01PirateShip, (uint)EntityID._01PirateShiptarget, (int)new TimeSpan(0, 1, 30).TotalMilliseconds));
            saveThomas.OnMissionLoaded += SaveThomas_Loaded;
            m_objectives.Add(saveThomas);

            var killAttackers = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_KILL_ATTACKERS_Name),
                MyMissionID.BARTHS_MOON_PLANT_KILL_ATTACKERS,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_KILL_ATTACKERS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_SAVE_BARTH },
                null,
                m_02SpawnPoints,
                false
                ) { SaveOnSuccess = true, };
            killAttackers.OnMissionLoaded += KillAttackers_Loaded;
            m_objectives.Add(killAttackers);


            var getTurrets = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_DEFENCE_Name),
                MyMissionID.BARTHS_MOON_PLANT_DEFENCE,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_DEFENCE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_KILL_ATTACKERS },
                new MyMissionLocation(baseSector, (uint)EntityID._03Dummy),
                MyTextsWrapperEnum.PressToTakeTurrets,
                MyTextsWrapperEnum.Turrets,
                MyTextsWrapperEnum.TransferInProgress,
                3000,
                startDialogId: MyDialogueEnum.BARTHS_MOON_PLANT_0300) { SaveOnSuccess = true };
            getTurrets.OnMissionLoaded += GetTurrets_Loaded;
            getTurrets.OnMissionSuccess += GetTurrets_Success;
            m_objectives.Add(getTurrets);



            m_buildDefenses = new MyTimedObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_BUILD_DEFENCE_LINE_Name),
                MyMissionID.BARTHS_MOON_PLANT_BUILD_DEFENCE_LINE,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_BUILD_DEFENCE_LINE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_DEFENCE },
                new TimeSpan(0, 0, minutes: 1, seconds: 30)

                ) { DisplayCounter = true, SaveOnSuccess = true };
            m_buildDefenses.OnMissionLoaded += BuildDefenses_Loaded;
            m_buildDefenses.OnMissionUpdate += BuildDefenses_Update;
            m_buildDefenses.OnMissionCleanUp += BuildDefenses_Unloaded;
            m_objectives.Add(m_buildDefenses);



            m_protectBarth = new MyTimedObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_PROTECT_BARTH_Name),
                MyMissionID.BARTHS_MOON_PLANT_PROTECT_BARTH,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_PROTECT_BARTH_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_BUILD_DEFENCE_LINE },
                new TimeSpan(0, minutes: 2, seconds: 0),
                true
                ) { DisplayCounter = false, SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_PLANT_0400, MakeEntityIndestructible = false };
            m_protectBarth.MissionEntityIDs.Add((uint)EntityID._01SmallShipBarth);
            m_protectBarth.Components.Add(new MyMovingEntity((uint)EntityID._05PirateShip, (uint)EntityID._01PirateShiptarget, (int)m_protectBarth.SubmissionDuration.TotalMilliseconds));
            m_protectBarth.Components.Add(new MyMovingEntity((uint)EntityID._05PirateBigShip, (uint)EntityID._05PirateBigShipTarget, (int)m_protectBarth.SubmissionDuration.TotalMilliseconds));
            m_protectBarth.OnMissionLoaded += ProtectBarth_Loaded;
            m_protectBarth.OnMissionCleanUp += ProtectBarth_Unloaded;
            m_protectBarth.OnMissionSuccess += ProtectBarth_Success;
            m_objectives.Add(m_protectBarth);


            m_protectMadelyn = new MyTimedObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_PROTECT_MADELYN_Name),
                MyMissionID.BARTHS_MOON_PLANT_PROTECT_MADELYN,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_PROTECT_MADELYN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_PROTECT_BARTH },
                new TimeSpan(0, minutes: 3, seconds: 0),
                true) { SaveOnSuccess = true, DisplayCounter = false, HudName = MyTextsWrapperEnum.HudCommandCenter };
            m_protectMadelyn.OnMissionLoaded += ProtectMadelyn_Loaded;
            m_protectMadelyn.OnMissionSuccess += ProtectMadelyn_Success;
            m_protectMadelyn.OnMissionCleanUp += ProtectMadelyn_CleanUp;
            m_protectMadelyn.OnMissionUpdate += ProtectMadelyn_Update;
            m_protectMadelyn.Components.Add(new MyTimedDialogue(new TimeSpan(0, 0, 5), MyDialogueEnum.BARTHS_MOON_PLANT_0600));
            m_objectives.Add(m_protectMadelyn);


            m_lastStand = new MyTimedObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_ENEMY_Name),
                MyMissionID.BARTHS_MOON_PLANT_ENEMY,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_ENEMY_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_PROTECT_MADELYN },
                new TimeSpan(0, 0, 50),
                true
            ) { /*DisplayCounter = false, */ SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_PLANT_0800 };
            m_lastStand.Components.Add(new MyMovingEntity((uint)EntityID._05PirateShip, (uint)EntityID._07PirateShipTarget, (int)m_lastStand.SubmissionDuration.TotalMilliseconds));
            m_lastStand.Components.Add(new MyMovingEntity((uint)EntityID._05PirateBigShip, (uint)EntityID._07PirateBigShipTarget, (int)m_lastStand.SubmissionDuration.TotalMilliseconds));
            m_lastStand.OnMissionLoaded += LastStand_Loaded;
            m_lastStand.OnMissionCleanUp += LastStand_CleanUp;
            m_objectives.Add(m_lastStand);


            m_destroyGenerators = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_DESTROY_GENERATORS_Name),
                MyMissionID.BARTHS_MOON_PLANT_DESTROY_GENERATORS,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_DESTROY_GENERATORS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_ENEMY },
                new List<uint> { (uint)EntityID._08generator1, (uint)EntityID._08generator2 },         // entities needed to kill
                new List<uint> { },  // spawnpoint from which bots must be killed
                true,
                false // dont count 
            ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.BARTHS_MOON_PLANT_0900, HudName = MyTextsWrapperEnum.HudGenerator };
            m_destroyGenerators.OnMissionSuccess += DestroyGenerators_Success;
            m_destroyGenerators.OnMissionLoaded += DestroyGenerators_Loaded;
            m_objectives.Add(m_destroyGenerators);



            var getComponents = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_GET_NEEDED_COMPONENTS_Name),
                MyMissionID.BARTHS_MOON_PLANT_GET_NEEDED_COMPONENTS,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_GET_NEEDED_COMPONENTS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_DESTROY_GENERATORS },
                new MyMissionLocation(baseSector, (uint)EntityID.GetNeededComponentsDummy),
                MyTextsWrapperEnum.PressToPickItems,
                MyTextsWrapperEnum.Items,
                MyTextsWrapperEnum.PickingInProgress,
                4000,
                MyUseObjectiveType.Taking,
                startDialogId: MyDialogueEnum.BARTHS_MOON_PLANT_1000) { SaveOnSuccess = true };
            getComponents.OnMissionLoaded += GetComponents_Loaded;
            getComponents.OnMissionSuccess += GetComponents_Success;
            m_objectives.Add(getComponents);

            var buildPlant = new MyUseObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_PLANT_BUILD_PLANT_Name),
                    MyMissionID.BARTHS_MOON_PLANT_BUILD_PLANT,
                    (MyTextsWrapperEnum.BARTHS_MOON_PLANT_BUILD_PLANT_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_GET_NEEDED_COMPONENTS },
                    new MyMissionLocation(baseSector, (uint)EntityID.ConstructDetector),
                    MyTextsWrapperEnum.PressToBuildTransmitter,
                    MyTextsWrapperEnum.Items,
                    MyTextsWrapperEnum.BuildingInProgress,
                    4000,
                    MyUseObjectiveType.Activating,
                    startDialogId: MyDialogueEnum.BARTHS_MOON_PLANT_1100) { SaveOnSuccess = true };
            buildPlant.OnMissionLoaded += new MissionHandler(BuildPlant_OnMissionLoaded);
            buildPlant.OnMissionSuccess += new MissionHandler(BuildPlant_OnMissionSuccess);
            m_objectives.Add(buildPlant);

            var constructDetectors = new MyUseObjective(
                    (MyTextsWrapperEnum.BARTHS_MOON_PLANT_CONSTRUCT_DETECTORS_Name),
                    MyMissionID.BARTHS_MOON_PLANT_CONSTRUCT_DETECTORS,
                    (MyTextsWrapperEnum.BARTHS_MOON_PLANT_CONSTRUCT_DETECTORS_Description),
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_BUILD_PLANT },
                    new MyMissionLocation(baseSector, (uint)EntityID.ConstructDetector),
                    MyTextsWrapperEnum.PressToBuildTransmitter,
                    MyTextsWrapperEnum.Items,
                    MyTextsWrapperEnum.BuildingInProgress,
                    4000,
                    MyUseObjectiveType.Activating) { SaveOnSuccess = true };
            constructDetectors.OnMissionLoaded +=new MissionHandler(ConstructDetectors_Loaded);
            constructDetectors.OnMissionSuccess += new MissionHandler(ConstructDetectors_Success);
            m_objectives.Add(constructDetectors);



            m_barthEndDialogue = new MyMeetObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_TALK_BARTH_Name),
                MyMissionID.BARTHS_MOON_PLANT_TALK_BARTH,
                (MyTextsWrapperEnum.BARTHS_MOON_PLANT_TALK_BARTH_Description),
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT_CONSTRUCT_DETECTORS },
                (uint)EntityID._01Detector,
                (uint)EntityID._01SmallShipBarth,
                50,
                0.25f,
                successDialogueId: MyDialogueEnum.BARTHS_MOON_PLANT_1200
            ) { SaveOnSuccess = true, FollowMe = false, PathName = "Group23", MakeEntityIndestructible = false };
            m_barthEndDialogue.OnMissionLoaded += BarthEndDialogue_Loaded;
            m_objectives.Add(m_barthEndDialogue);
            #endregion
        }


        private void GetComponents_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.TryUnhide((uint)EntityID.GetNeededComponentsContainer);
            MyScriptWrapper.Highlight((uint)EntityID.GetNeededComponentsContainer, true, this);
        }

        private void GetComponents_Success(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.GetNeededComponentsContainer, false, this);
        }

        void BuildPlant_OnMissionLoaded(MyMissionBase sender)
        {
            // show plant
            MyScriptWrapper.TryUnhide((uint)EntityID.BuildContainer1);
            MyScriptWrapper.Highlight((uint)EntityID.BuildContainer1, true, sender);
        }

        void BuildPlant_OnMissionSuccess(MyMissionBase sender)
        {
            // unhighlight plant
            MyScriptWrapper.Highlight((uint)EntityID.BuildContainer1, false, sender);
        }

        private void ConstructDetectors_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.TryUnhide((uint)EntityID.UnhidePrefab);
            MyScriptWrapper.Highlight((uint)EntityID.UnhidePrefab, true, this);
        }

        private void ConstructDetectors_Success(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.UnhidePrefab, false, this);
        }

        private void BarthEndDialogue_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled((uint)EntityID.Light1, true);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID.Light2, true);
            MyScriptWrapper.SetEntityEnabled((uint)EntityID.Light3, true);
            
        }


        private void DestroyGenerators_Loaded(MyMissionBase sender)
        {
            ActivateSpawnpoints(m_08SpawnPoints1);
            ActivateSpawnpoints(m_08SpawnPoints2);
        }

        private void DestroyGenerators_Success(MyMissionBase sender)
        {
            foreach (KeyValuePair<uint, uint> keyValuePair in m_detectorToTurretContainerMapping)
            {
                var container = MyScriptWrapper.TryGetEntity(keyValuePair.Value);
                if (container != null)
                {
                    RepairTurret(container.EntityId.Value.NumericValue);

                    var detectorId = m_turretContainerToDetectorMapping[container.EntityId.Value.NumericValue];

                    var detector = (MyEntityDetector)MyScriptWrapper.GetEntity(detectorId);
                    detector.Off();
                    detector.OnEntityLeave -= RepairDetector_Leave;
                    detector.OnEntityEnter -= RepairDetector_Enter;
                }
            }

            MyScriptWrapper.KillAllEnemy();

            MyScriptWrapper.DeactivateSpawnPoints(m_02SpawnPoints);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID._01SpawnPoint);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID._01SpawnPoint2);
        }

        private void LastStand_CleanUp(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity("MadelynsBridge"), false);
        }


        private void LastStand_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTexts.RepairDamagedTurrets, MyGuiManager.GetFontMinerWarsBlue(), 10000));
        }

        private void ProtectBarth_Loaded(MyMissionBase sender)
        {
            m_canRepairNotification = MyScriptWrapper.CreateNotification(
                Localization.MyTextsWrapperEnum.PressToRepairTurret,
                MyGuiManager.GetFontMinerWarsBlue());
            m_canRepairNotification.SetTextFormatArguments(m_actionKeyString);


            m_repairing = false;


            ShowContainer(EntityID._05PirateBigShip);
            ShowContainer(EntityID._05PirateShip);

            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity("MadelynsBridge"), true);

            MyScriptWrapper.PrepareMotherShipForMove(m_05PirateShip);
            MyScriptWrapper.PrepareMotherShipForMove(m_05PirateBigShip);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("FrontTurretM"), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("BackTurretM"), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("BottomTurretM"), true);
            ActivateSpawnpoints(m_05SpawnPoints);
        }

        private void KillAttackers_Loaded(MyMissionBase sender)
        {
            m_barth.SetWaypointPath("BarthEs");
            m_barth.StopFollow();
            m_barth.PatrolMode = MyPatrolMode.CYCLE;
            m_barth.Patrol();

            ActivateSpawnpoints(m_02SpawnPoints);

            HideContainer(EntityID._01PirateShip);

            var madelyn = MyScriptWrapper.GetEntity("Madelyn");
            MyScriptWrapper.Move(madelyn, GetEntity(EntityID._02MadelyneDefenceShip).GetPosition(), GetEntity(EntityID._02MadelyneDefenceShip).GetForward(), GetEntity(EntityID._02MadelyneDefenceShip).GetUp());
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("FrontTurretM"), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("BackTurretM"), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity("BottomTurretM"), false);
        }


        private void SaveThomas_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID._01SpawnPoint);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID._01SpawnPoint2);
        }

        private void GetTurrets_Loaded(MyMissionBase sender)
        {
            foreach (var child in MyScriptWrapper.GetEntity((uint)EntityID._03PortableTurrets).Children)
            {
                MyScriptWrapper.Highlight(child.EntityId.Value.NumericValue, true, this);
            }
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID._03Dummy), MyTexts.PortableTurrets, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE, MyGuitargetMode.Objective);
        }

        private void GetTurrets_Success(MyMissionBase sender)
        {
            foreach (var child in MyScriptWrapper.GetEntity((uint)EntityID._03PortableTurrets).Children)
            {
                MyScriptWrapper.Highlight(child.EntityId.Value.NumericValue, false, this);
            }
            HideContainer(EntityID._03PortableTurrets);

            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON,
                NUMBER_OF_TURRETS_TO_BUILD, true);

        }

        void BuildDefenses_Loaded(MyMissionBase sender)
        {
            m_buildingStopped = false;

            m_turretsBuilt = 0;

            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_1).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_2).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_3).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_4).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_5).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_6).EntityId.Value.NumericValue);
            m_buildDefenses.MissionEntityIDs.Add(GetTurretFromContainer((uint)EntityID.TURRET_CONTAINER_ID_7).EntityId.Value.NumericValue);

            foreach (var detectorTurretContainerPair in m_detectorToTurretContainerMapping)
            {
                MyScriptWrapper.Highlight(detectorTurretContainerPair.Value, true, this);
                MyScriptWrapper.EnablePhysics(detectorTurretContainerPair.Value, false);
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(detectorTurretContainerPair.Value));

                ShowContainer(MyScriptWrapper.GetEntity(detectorTurretContainerPair.Value) as MyPrefabContainer);

                var turret = GetTurretFromContainer(detectorTurretContainerPair.Value);
                Debug.Assert(turret != null);
                if (turret != null)
                {
                    turret.Enabled = false;
                    MyScriptWrapper.MarkEntity(turret, MyTexts.BuildTurret, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER, MyGuitargetMode.Objective);
                }
            }


            foreach (var dummyId in m_detectorToTurretContainerMapping)
            {
                var detector = MyScriptWrapper.GetEntity(dummyId.Key) as MyEntityDetector;
                //MyScriptWrapper.GetEntity(dummyId.Key);
                Debug.Assert(detector != null);
                if (detector != null)
                {
                    // register for player ship enter
                    detector.OnEntityEnter += TurretDummyEnter;
                    detector.OnEntityLeave += TurretDummyLeave;
                    detector.On();
                }
            }

            m_canBuildNotification = MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.PressToBuildTurret, MyGuiManager.GetFontMinerWarsBlue());
            m_canBuildNotification.SetTextFormatArguments(m_actionKeyString);

            m_remainingTurretsNotification = MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.RemainingTurrets, MyGuiManager.GetFontMinerWarsBlue());
            m_remainingTurretsNotification.IsImportant = true;

            //m_canSkipNotification = MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.PressToSkipTimer, MyGuiManager.GetFontMinerWarsBlue());
            //m_canSkipNotification.SetTextFormatArguments(m_actionKeyString);
            //m_canSkipNotification.Disappear();


            MyScriptWrapper.AddNotification(m_remainingTurretsNotification);

            MyScriptWrapper.OnUseKeyPress += Build_UseKeyPress;
        }

        void BuildDefenses_Update(MyMissionBase sender)
        {
            UpdateRemainingTurretsCount();
        }


        private void TryBuildTurret()
        {
            if (m_currentDetector != null)
            {
                var inventory = MyScriptWrapper.GetCentralInventory();
                bool canBuild = inventory.RemoveInventoryItemAmount(
                    MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                    (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON,
                    1);

                if (canBuild)
                {
                    m_buildDefenses.MissionEntityIDs.Remove(GetTurretFromContainer(m_detectorToTurretContainerMapping[MyScriptWrapper.GetEntityId(m_currentDetector)]).EntityId.Value.NumericValue);

                    BuildTurret();
                    //m_currentDetector.Parent.Close();
                    m_currentDetector.Off();
                    m_currentDetector = null;
                }
            }
        }

        private void BuildTurret()
        {
            var container = MyScriptWrapper.GetEntity(m_detectorToTurretContainerMapping[m_currentDetector.EntityId.Value.NumericValue]);

            Debug.Assert(container != null);
            if (container != null)
            {
                MyScriptWrapper.Highlight(m_detectorToTurretContainerMapping[m_currentDetector.EntityId.Value.NumericValue], false, this);
                MyScriptWrapper.EnablePhysics(m_detectorToTurretContainerMapping[m_currentDetector.EntityId.Value.NumericValue], true);

                var turret = GetTurretFromContainer(container.EntityId.Value.NumericValue);
                MyScriptWrapper.RemoveEntityMark(turret);
                turret.Enabled = true;

                foreach (var child in turret.Children)
                {
                    child.Enabled = true;
                }
            }

            m_turretsBuilt++;
            UpdateRemainingTurretsCount();
            m_canBuildNotification.Disappear();
        }

        private void UpdateRemainingTurretsCount()
        {
            var inventory = MyScriptWrapper.GetCentralInventory();
            var remainingTurretsCount = Convert.ToInt32(inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON));
            if (remainingTurretsCount <= 0)
            {
                StopBuilding();
                
                //m_canSkipNotification.Appear();
                //MyScriptWrapper.AddNotification(m_canSkipNotification);
            }
            else
            {
                m_remainingTurretsNotification.Appear();
                m_remainingTurretsNotification.SetTextFormatArguments(new object[] { m_turretsBuilt, remainingTurretsCount });
            }
        }
        

        void BuildDefenses_Unloaded(MyMissionBase sender)
        {
            StopBuilding();

            //m_canSkipNotification.Disappear();
            //MyScriptWrapper.OnUseKeyPress -= SkipToAttack_OnUseKeyPress;
            m_currentDetector = null;
        }

        private bool m_buildingStopped = false;
        private void StopBuilding()
        {
            if (m_buildingStopped)
                return;
            m_buildingStopped = true;

            m_remainingTurretsNotification.Disappear();

            var missionIds = m_buildDefenses.MissionEntityIDs.ToList();
            foreach (var containerID in missionIds)
            {
                m_buildDefenses.MissionEntityIDs.Remove(containerID);

                var turret = MyScriptWrapper.TryGetEntity(containerID);
                if (turret != null)
                {
                    var container = turret.Parent as MyPrefabContainer;
                    if (container != null)
                    {
                        MyScriptWrapper.Highlight(container.EntityId.Value.NumericValue, false, this);
                        MyScriptWrapper.HideEntity(container);
                        HideContainer(container);
                        MyScriptWrapper.RemoveEntityMark(turret);
                        container.MarkForClose();
                    }
                }
            }

            foreach (var dummyTurretContainerPair in m_detectorToTurretContainerMapping)
            {
                var detector = MyScriptWrapper.GetEntity(dummyTurretContainerPair.Key) as MyEntityDetector;
                Debug.Assert(detector != null);
                if (detector != null)
                {
                    // unregister for player ship enter
                    detector.OnEntityEnter -= TurretDummyEnter;
                    detector.OnEntityLeave -= TurretDummyLeave;
                    if (detector.IsOn())
                    {
                        detector.Off();
                    }
                }
            }


            MyScriptWrapper.OnUseKeyPress -= Build_UseKeyPress;

            m_buildDefenses.SkipTimer();

            //MyScriptWrapper.OnUseKeyPress += SkipToAttack_OnUseKeyPress;

            MyScriptWrapper.GetCentralInventory().RemoveInventoryItems(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON, true);
        }

        private void TurretDummyEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip && this.m_turretsBuilt < NUMBER_OF_TURRETS_TO_BUILD)
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

        private void SetUpTurrets()
        {
            m_detectorToTurretContainerMapping.Clear();
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_1).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_1);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_2).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_2);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_3).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_3);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_4).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_4);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_5).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_5);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_6).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_6);
            m_detectorToTurretContainerMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_7).EntityId.Value.NumericValue, (uint)EntityID.TURRET_CONTAINER_ID_7);

            m_turretContainerToDetectorMapping.Clear();
            foreach (var item in m_detectorToTurretContainerMapping)
            {
                m_turretContainerToDetectorMapping.Add(item.Value, item.Key);
            }

            m_detectorToParticleMapping.Clear();
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_1).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_1);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_2).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_2);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_3).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_3);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_4).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_4);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_5).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_5);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_6).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_6);
            m_detectorToParticleMapping.Add(MyScriptWrapper.GetDetector((uint)EntityID.DETECTOR_ID_7).EntityId.Value.NumericValue, (uint)EntityID.PARTICLE_ID_7);
        }

        private static MyPrefabLargeWeapon GetTurretFromContainer(uint id)
        {
            MyPrefabLargeWeapon turret = null;
            var container = MyScriptWrapper.TryGetEntity(id);
            if (container == null)
                return null;
            if (container is MyPrefabLargeWeapon)
                return (MyPrefabLargeWeapon)container;
            foreach (var child in container.Children)
            {
                turret = child as MyPrefabLargeWeapon;
                if (turret != null) break;
            }
            return turret;
        }


        private void Repair_UseKeyPress()
        {
            if (m_buildDefenses.IsCompleted() && !m_destroyGenerators.IsCompleted())
            {
                if (!m_repairing && m_currentDetector != null)
                {
                    m_repairProgress.Reset();
                    MyGuiManager.AddScreen(m_repairProgress);
                }
            }
        }

                                        /*
        void SkipToAttack_OnUseKeyPress()
        {
            m_buildDefenses.SkipTimer();
        }
                                          */
        void Build_UseKeyPress()
        {
            if (m_buildDefenses.IsAvailable())
            {
                if (!m_building && m_currentDetector != null)
                {
                    m_building = true;
                    m_buildProgress.Reset();
                    MyGuiManager.AddScreen(m_buildProgress);
                }
            }
        }



        private void RepairDetector_Leave(MyEntityDetector sender, MyEntity entity)
        {
            if (entity == MySession.PlayerShip)
            {
                m_canRepairNotification.Disappear();
                m_currentDetector = null;
            }
        }

        private void RepairDetector_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddNotification(m_canRepairNotification);
                m_currentDetector = sender;
            }
        }


        private void Build_Success(object sender, EventArgs e)
        {
            TryBuildTurret();
            m_building = false;
        }

        private void Build_Canceled(object sender, EventArgs e)
        {
            m_building = false;
        }

        private void Repair_Success(object sender, EventArgs e)
        {
            if (m_currentDetector != null)
            {
                var containerId = m_detectorToTurretContainerMapping[MyScriptWrapper.GetEntityId(m_currentDetector)];
                RepairTurret(containerId);
            }
            
            m_repairing = false;
        }

        private void Repair_Canceled(object sender, EventArgs e)
        {
            m_repairing = false;
        }

        private void RepairTurret(uint containerId)
        {
            var turret = GetTurretFromContainer(containerId);
            MyScriptWrapper.SetEntityDestructible(turret, true);
            turret.Health = turret.MaxHealth;
            turret.Enabled = true;
            turret.AIPriority = 0;

            var detectorId = m_turretContainerToDetectorMapping[containerId];
            MyScriptWrapper.SetEntityEnabled(m_detectorToParticleMapping[detectorId], false);

            MyEntityDetector detector = (MyEntityDetector)MyScriptWrapper.GetEntity(detectorId);
            detector.Off();

            if (m_lastStand.IsAvailable())
            {
                m_lastStand.MissionEntityIDs.Remove(turret.EntityId.Value.NumericValue);
                MyScriptWrapper.RemoveEntityMark(turret);
            }

            MyScriptWrapper.RemoveEntityMark(turret);
        }

        private void HookDamagedTurrets()
        {
            foreach (var turretContainerId in m_turretContainerToDetectorMapping.Keys)
            {
                var turret = GetTurretFromContainer(turretContainerId);
                if (turret == null) continue;
                if (!MarkForRepair(turret))
                {
                    MyScriptWrapper.SetEntityDestructible(turret, true);
                    turret.Enabled = true;
                    turret.AIPriority = 0;

                    var detectorId = m_turretContainerToDetectorMapping[turretContainerId];
                    MyScriptWrapper.SetEntityEnabled(m_detectorToParticleMapping[detectorId], false);

                    MyEntityDetector detector = (MyEntityDetector)MyScriptWrapper.GetEntity(detectorId);
                    detector.Off();
                }
            }
        }

        private void Script_EntityAtacked(MyEntity entity1, MyEntity entity2)
        {
            var turret = entity2 as MyPrefabLargeWeapon;
            MarkForRepair(turret);
        }

        private bool MarkForRepair(MyPrefabLargeWeapon turret)
        {
            if (turret != null)
            {
                if (turret.Health <= 250 && turret.IsDestructible && m_turretContainerToDetectorMapping.ContainsKey(turret.Parent.EntityId.Value.NumericValue))
                {
                    MyScriptWrapper.SetEntityDestructible(turret, false);
                    MyScriptWrapper.SetEntityEnabled(turret, false);
                    turret.AIPriority = -1;

                    uint detectorId = m_turretContainerToDetectorMapping[turret.Parent.EntityId.Value.NumericValue];

                    var repairDetector = (MyEntityDetector)MyScriptWrapper.GetEntity(detectorId);
                    repairDetector.On();
                    repairDetector.OnEntityEnter += RepairDetector_Enter;
                    repairDetector.OnEntityLeave += RepairDetector_Leave;

                    MyScriptWrapper.SetEntityEnabled(m_detectorToParticleMapping[detectorId], true);

                    MyScriptWrapper.MarkEntity(turret, MyTexts.Repair, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS);
                    return true;
                }
            }
            return false;
        }

        private void ProtectBarth_Success(MyMissionBase sender)
        {
            DeactivateSpawnpoints(m_05SpawnPoints);
        }


        private void ProtectBarth_Unloaded(MyMissionBase sender)
        {
            MyScriptWrapper.ReturnMotherShipFromMove(m_05PirateShip);
            MyScriptWrapper.ReturnMotherShipFromMove(m_05PirateBigShip);
        }


        private void ProtectMadelyn_Loaded(MyMissionBase sender)
        {
            ActivateSpawnpoints(m_06SpawnPoints);

            m_06AttackDetector = MyScriptWrapper.GetDetector((uint)EntityID._06AttackmadelynDummy);
            m_06AttackDetector.On();
            m_06AttackDetector.OnEntityEnter += M06AttackDetectorOnOnEntityEnter;

            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTexts.ProtectSaphosCommandCenter, MyGuiManager.GetFontMinerWarsBlue(), 10000));

            ShowContainers(m_06BombsContainersIds);

            var index = 0;
            m_06BombsTrajectories = new List<MyLine>();
            m_06BombsContainers = new List<MyEntity>();
            m_06BombsDeath = new List<bool>();
            m_06BombsProgresses = new List<float>();
            m_06BombsMarked = new List<bool>();
            
            foreach (uint prefabId in m_06BombsPrefabsIDs)
            {
                m_06BombsTrajectories.Add(new MyLine(MyScriptWrapper.GetEntity(prefabId).GetPosition(), MyScriptWrapper.GetEntity(m_06BombsTargetDummy[index]).GetPosition()));
                m_06BombsContainers.Add(MyScriptWrapper.GetEntity(m_06BombsContainersIds[index]));
                m_06BombsProgresses.Add(-1);
                m_06BombsDeath.Add(false);
                m_06BombsMarked.Add(false);
                index++;
            }

            var madelyn = MyScriptWrapper.GetEntity("MadelynsBridge");
            MyScriptWrapper.MarkEntity(madelyn, sender.HudNameTemp.ToString(), MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_TEXT);
        }

        private void M06AttackDetectorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            var bot = entity as MySmallShipBot;
            if (bot != null)
            {
                var madelynsBridge = MyScriptWrapper.TryGetEntity("MadelynsBridge");

                if (MyFactions.GetFactionsRelation(bot.Faction, madelynsBridge.Faction) == MyFactionRelationEnum.Enemy)
                {
                    bot.Attack(madelynsBridge);
                }
            }
        }



        private void ProtectMadelyn_Update(MyMissionBase sender)
        {
            var index = 0;
            foreach (var prefab in m_06BombsContainers)
            {
                //m_06bobmsTrajectories.Add(new MyLine(MyScriptWrapper.GetEntity(container).GetPosition(), MyScriptWrapper.GetEntity(m_06BombsTargetDummy[index]).GetPosition()));
                var progressUp = sender.MissionTimer.ElapsedTime - m_06BombsTimeOffsets[index];
                var progressDown = (float)m_06BombsFlyTimes[index];
                var progress = progressUp / progressDown;

                if (m_06BombsTimeOffsets[index] < sender.MissionTimer.ElapsedTime && !m_06BombsMarked[index])
                {
                    MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity(m_06BombsPrefabsIDs[index]), MyTexts.Destroy, MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS, MyGuitargetMode.Enemy);
                    m_06BombsMarked[index] = true;
                }
                m_06BombsProgresses[index] = progress;


                if (progress > 1 && !m_06BombsDeath[index])
                {
                    m_destroySuicideDialoguePlayed = true;
                    MyScriptWrapper.DestroyEntity(MyScriptWrapper.GetEntityId("MadelynsBridge"));
                    MyScriptWrapper.DestroyEntities(m_06BombsPrefabsIDs);
                    BlowMadelyn();
                }

                if (progress > 0 && progress < 1 && !m_06BombsDeath[index])
                {
                    MyScriptWrapper.Move(prefab, Vector3.Lerp(m_06BombsTrajectories[index].From, m_06BombsTrajectories[index].To, progress));
                }
                index++;
            }
        }

        private void ProtectMadelyn_CleanUp(MyMissionBase sender)
        {
            m_06AttackDetector.Off();

            var madelyn = MyScriptWrapper.GetEntity("MadelynsBridge");
            MyScriptWrapper.RemoveEntityMark(madelyn);
        }

        private void ProtectMadelyn_Success(MyMissionBase sender)
        {
            DeactivateSpawnpoints(m_06SpawnPoints);
        }

        private void Script_EntityDeath(MyEntity entity1, MyEntity entity2)
        {
            if (MyMissions.ActiveMission != this)
                return;

            //if (m_protectMadelyn.IsAvailable() && entity1.Name == "MadelynsBridge")
            //{
            //    BlowMadelyn();
            //}


            if (m_protectMadelyn.IsAvailable() && entity1.EntityId.HasValue && m_06BombsPrefabsIDs.Contains(entity1.EntityId.Value.NumericValue))
            {
                int index = m_06BombsPrefabsIDs.IndexOf(entity1.EntityId.Value.NumericValue);
                var particle = MyScriptWrapper.GetEntity(m_06BombsParticleDummy[index]);
                particle.SetWorldMatrix(entity1.WorldMatrix);
                MyScriptWrapper.SetEntityEnabled(particle, true);
                m_06BombsDeath[index] = true;
                HideContainers(new List<uint>() { m_06BombsContainersIds[index] });
                if (!m_destroySuicideDialoguePlayed)
                {
                    m_destroySuicideDialoguePlayed = true;
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_PLANT_0700);
                }
            }


            if (m_destroyGenerators.IsAvailable() && entity1.EntityId.HasValue && entity1.EntityId.Value.NumericValue == (uint)EntityID._08generator1)
            {
                MyScriptWrapper.SetEntityEnabled((uint)EntityID._08generator1particle, true);
                HideContainer(EntityID._05PirateBigShip);
                ShowContainer(EntityID._08generator1VisibleCOnt);
                this.Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));
                EnableEntities(m_08Generator1Particles);
                DeactivateSpawnpoints(m_08SpawnPoints1);
            }

            if (m_destroyGenerators.IsAvailable() && entity1.EntityId.HasValue && entity1.EntityId.Value.NumericValue == (uint)EntityID._08generator2)
            {
                HideContainer(EntityID._05PirateShip);
                ShowContainer(EntityID._08generator2VisibleCont);
                EnableEntities(m_08Generator2Particles);
                this.Components.Add(new MyHeadshake(MissionTimer.ElapsedTime, MyHeadshake.DefaultShaking, 12, 5, 10));
                DeactivateSpawnpoints(m_08SpawnPoints2);
            }
        }

        private void BlowMadelyn()
        {
            MyScriptWrapper.SetEntityEnabled((uint)EntityID._06ParticleDummy, true);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity("Madelyn"));
            ShowContainer(EntityID._06PrefabContainer1);
            EnableEntities(m_06ParticleDummies);
            MyScriptWrapper.IncreaseHeadShake(8);

            Fail(MyTextsWrapperEnum.MadelynWasDestroyed);
        }


        public override void Load()
        {
            if (!IsMainSector) return;
            

            m_buildProgress = new MyGuiScreenUseProgressBar(MyTextsWrapperEnum.Turrets, MyTextsWrapperEnum.BuildingInProgress, 0f, MySoundCuesEnum.SfxProgressBuild, MySoundCuesEnum.SfxCancelBuild, MyGameControlEnums.USE, 0, 3000, 0);
            m_buildProgress.OnCanceled += Build_Canceled;
            m_buildProgress.OnSuccess += Build_Success;

            m_repairProgress = new MyGuiScreenUseProgressBar(MyTextsWrapperEnum.Turret, MyTextsWrapperEnum.ProgressRepairing, 0f, MySoundCuesEnum.SfxProgressRepair, MySoundCuesEnum.SfxCancelRepair, MyGameControlEnums.USE, 0, 2000, 0);
            m_repairProgress.OnCanceled += Repair_Canceled;
            m_repairProgress.OnSuccess += Repair_Success;

            m_protectMadelyn.MissionEntityIDs.Add(MyScriptWrapper.GetEntity("MadelynsBridge").EntityId.Value.NumericValue);

            MyScriptWrapper.OnEntityAtacked += Script_EntityAtacked;
            MyScriptWrapper.EntityDeath += Script_EntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned += Script_BotSpawned;
            MyScriptWrapper.OnUseKeyPress += Repair_UseKeyPress;

            ShowContainer(EntityID._01PirateShip);
            ShowContainer(EntityID._03PortableTurrets);
            ShowContainer(EntityID.BarthsTurrets1);
            ShowContainer(EntityID.BarthsDestroyedBase);
            HideContainer(EntityID.BarthNormalBase);

            HideContainers(m_barthsMoonM1);
            HideContainers(m_barthsMoonM2);
            DisableEntities(m_particleEffectsToDisable);
            EnableEntities(m_particleEffectsToEnable);
            
            EnableCorrectBarths((uint)EntityID._01SmallShipBarth, (uint)EntityID.ThomasBartId);

            MyScriptWrapper.SetEntitiesEnabled(m_disableDummy, false);
            MyScriptWrapper.TryHideEntities(m_setInVisible);
            MyScriptWrapper.TryUnhideEntities(m_setVisible);

            m_05PirateShip = MyScriptWrapper.GetEntity((uint)EntityID._05PirateShip);

            m_05PirateBigShip = MyScriptWrapper.GetEntity((uint)EntityID._05PirateBigShip);

            m_barth = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID._01SmallShipBarth);

            m_actionKeyString[0] = MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE);

            if (!MyMissions.GetMissionByID(MyMissionID.BARTHS_MOON_PLANT_SAVE_BARTH).IsCompleted())
            {
                var startPosition = MyScriptWrapper.GetEntity((uint)EntityID.PlayerStartLocationPlant).GetPosition();
                MyScriptWrapper.Move(MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN)), startPosition);
                MyScriptWrapper.MovePlayerAndFriendsToHangar(this.RequiredActors);
            }

            MyScriptWrapper.HideEntities(m_hidePrefabContainers);

            SetUpTurrets();

            m_attackBarthDetector = MyScriptWrapper.GetDetector((uint)EntityID.BarthAttackDetector);
            m_attackBarthDetector.On();
            m_attackBarthDetector.OnEntityEnter += AttackBarthDetector_Enter;
   
                                      /*
            MySector.FogProperties.FogNear = 1;
            MySector.FogProperties.FogFar = 53182.205f;
            MySector.FogProperties.FogMultiplier = 0.728f;
            MySector.FogProperties.FogBacklightMultiplier = 0;
            MySector.FogProperties.FogColor = MyMath.VectorFromColor(0, 112, 186);
            MySector.ParticleDustProperties.DustBillboardRadius = 168.707f;
            MySector.ParticleDustProperties.DustFieldCountInDirectionHalf = 8.817f;
            MySector.ParticleDustProperties.DistanceBetween = 125.083f;
            MySector.ImpostorProperties[2].Intensity = 1.519f;
            MySector.ImpostorProperties[2].Radius = 2.007f;
            MySector.ImpostorProperties[2].Anim1 = -0.021f;
                                        */
            m_currentDetector = null;

            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity("FrontTurretM"), -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity("BackTurretM"), -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity("BottomTurretM"), -1);

            // Show notifications
            HookDamagedTurrets();

            base.Load();

            if (MySession.Static.EventLog.IsMissionFinished(MyMissionID.BARTHS_MOON_PLANT_BUILD_PLANT))
            {
                ShowContainer(EntityID.BuildContainer1);
            }
        }


        private void AttackBarthDetector_Enter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == m_barth)
                return;

            var relation = MyFactions.GetFactionsRelation(entity.Faction, m_barth.Faction);
            var bot = entity as MySmallShipBot;
            if (bot != null && relation == MyFactionRelationEnum.Enemy)
            {
                bot.Attack(m_barth);
            }

            sender.Off();
        }

        private void Script_BotSpawned(MyEntity spawn, MyEntity entity2)
        {
            if (m_spawns.Contains(spawn.EntityId.Value.NumericValue))
            {
                ((MySmallShipBot) entity2).SleepDistance = 5000;
            }
        }

        public override void Unload()
        {
            if (!IsMainSector) return;

            base.Unload();

            m_protectMadelyn.MissionEntityIDs.Clear();

            m_05PirateShip = null;
            m_05PirateBigShip = null;
            m_barth = null;
            m_detectorToTurretContainerMapping.Clear();
            m_turretContainerToDetectorMapping.Clear();
            m_detectorToParticleMapping.Clear();

            m_attackBarthDetector = null;
             
            MyScriptWrapper.OnUseKeyPress -= Repair_UseKeyPress;
            MyScriptWrapper.OnEntityAtacked -= Script_EntityAtacked;
            MyScriptWrapper.EntityDeath -= Script_EntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned -= Script_BotSpawned;
        }
    }
}

using System.Text;
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
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyRussianWarehouseMission : MyMission
    {
        private enum EntityID
        {
            StartLocation = 54915,
            SneakInsideSubmissionLocation = 65922,
            TurnOffCamera0 = 436725,
            TurnOffCamera1 = 436726,
            TurnOffCamera2 = 76920,
            TurnOffHUB = 76921,
            GetOutsideSubmissionLocation = 69549,
            BreakThroughSubmissionLocation = 65925,
            LocateWarehouseSubmissionLocation = 65926,
            TransmitterCargoBox = 125618,
            CargoTurret0 = 65883,
            CargoTurret1 = 65885,
            CargoTurret2 = 65882,
            CargoTurret3 = 65886,
            CargoTurret4 = 65884,
            CargoTurret5 = 65794,
            CargoTurret6 = 65790,
            CargoTurret7 = 65788,
            CargoTurret8 = 65796,
            CargoTurret9 = 65792,
            CargoTurretHUB0 = 179534,
            CargoTurretHUB1 = 179535,
            GetOutsideThruCargoSubmissionLocation = 65929,
            CrushRussiansSpawnpoint = 171854,
            MeetingPointSubmission = 441040,
            UpperOldEntranceDoor1 = 10899,
            UpperOldEntranceDoor2 = 10796,
            LeftCargoDoors = 10658,
            OldEntranceDoor1 = 10799,
            OldEntranceDoor2 = 10822,
            WarehouseDoors = 10873,
            GetOutsideDoors = 84173,
            RightCargoDoors = 10665,
            MainbaseWarehouseDoors = 18611,
            WarehouseMainbaseDoors = 18604,
            ToWarehouseDoors = 18617,
            MainChamberDoors = 10583,
            AbandonedPartDoors = 10858,
            Spawnpoint1 = 69555,
            Spawnpoint2 = 69596,
            Spawnpoint3 = 69597,
            Spawnpoint4 = 133329,
            Spawnpoint5 = 443908,
            SpawnpointSlaversRight = 426612,
            SpawnpointSlaversLeft = 443422,
            SpawnpointSlaversMiddle = 443429,
            SpawnpointOnRoadToOldEntr1 = 443902,
            SpawnpointOnRoadToOldEntr2 = 443903,
            SpawnpointBonus1 = 448384,
            SpawnpointBonus2 = 448635,
            SpawnpointBonus3 = 448651,
            SpawnpointBonus4 = 448658,
            SpawnpointBonus5 = 449174,
            SpawnpointSeekAndDestroyA = 450259,
            SpawnpointSeekAndDestroyB = 450260,
            Detector02 = 95056,
            Detector03 = 141011,
            Detector04 = 156385,
            Detector05 = 164164,
            Detector06 = 164165,
            Detector07 = 164166,
            Detector08 = 164171,
            Detector09 = 164172,
            Detector10 = 171853,
            Detector11 = 443904,
            Detector12 = 443906,
            DetectorBonus1 = 448631,
            DetectorBonus2 = 448633,
            DetectorBonus3 = 448649,
            DetectorBonus4 = 448656,
            DetectorBonus5 = 449172,
            DetectorForUpperTurretHub = 450246,
            DetectorForLowerTurretHub = 450248,
            DetectorForDownData1 = 450251,
            DetectorForDownData2 = 450253,
            DetectorForExitArea1 = 450255,
            DetectorForExitArea2 = 450257,
            //DetectorForSeekAndDestroy1 = 164164,
            //DetectorForSeekAndDestroy2 = 164165,
            //DetectorForSeekAndDestroy3 = 164166,
            //DetectorForSeekAndDestroy4 = 164171,
            SlaversTarget0 = 248960,
            OpenDoorsSpawnpoint = 443910,
            LateWarehouseSpawnpoint = 443439,
            OpenDoorHUB = 87800,
            OpenDoorsDummy = 443909,
            SecurityTowersHUB = 80548,
            SecurityTower1 = 454252,
            SecurityTower2 = 454255,
            SecurityTower3 = 453696,
            SecurityTower4 = 76886,
            SecurityTower5 = 76913,
            //SecurityTower6 = 73263,
            DownloadDataDummy = 443913,
            EntranceDoors = 65845,
            PirateMothershipContainer = 69550,
            PirateMothershipThrust0 = 441843,
            PirateMothershipThrust1 = 441844,
            PirateMothershipThrust2 = 441845,
            PirateMothershipTrigger = 449176,
            SpawnpointCtrlTurret = 449702,
            LastHatch = 443436,

            MovingMothershipContainer = 54898,
            MovingMothershipContainerParticles1 = 16778602,
            MovingMothershipContainerParticles2 = 16778572,
            MovingMothershipTarget = 16777852,
            MovingMothershipSpawnpoint1 = 16777873, 
            MovingMothershipSpawnpoint2 = 16777874,

            SomeSpawnpoint01 = 156439,
            SomeSpawnpoint02 = 110268,
            SomeSpawnpoint03 = 451403,
            SomeSpawnpoint04 = 141012,
            SomeSpawnpoint05 = 164173,
            SomeSpawnpoint06 = 426633,
            SomeSpawnpoint07 = 164168,
            SomeSpawnpoint08 = 451951,
            SomeSpawnpoint09 = 164170,
            SomeSpawnpoint10 = 164169,

            RaidSpawnpoint = 102,
            RaidDetector = 103,
        }

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (var value in m_waypoints)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        private List<uint> m_waypoints = new List<uint>
        {
            187215, 43820, 10664, 10657, 10787, 10803, 10786, 10811
        };

        private MyEntity GetEntity(EntityID entityId)
        {
            return MyScriptWrapper.GetEntity((uint)entityId);
        }

        private MyEntityDetector Detector01 { get; set; }
        private MyEntityDetector Detector02 { get; set; }
        private MyEntityDetector Detector03 { get; set; }
        private MyEntityDetector Detector04 { get; set; }
        private MyEntityDetector Detector05 { get; set; }
        private MyEntityDetector Detector06 { get; set; }
        private MyEntityDetector Detector07 { get; set; }
        private MyEntityDetector Detector08 { get; set; }
        private MyEntityDetector Detector09 { get; set; }
        private MyEntityDetector Detector10 { get; set; }
        private MyEntityDetector Detector11 { get; set; }
        private MyEntityDetector Detector12 { get; set; }
        private MyEntityDetector DetectorBonus1 { get; set; }
        private MyEntityDetector DetectorBonus2 { get; set; }
        private MyEntityDetector DetectorBonus3 { get; set; }
        private MyEntityDetector DetectorBonus4 { get; set; }
        private MyEntityDetector DetectorBonus5 { get; set; }
        private MyEntityDetector DetectorForUpperTurretHub { get; set; }
        private MyEntityDetector DetectorForLowerTurretHub { get; set; }
        private MyEntityDetector DetectorForDownData1 { get; set; }
        private MyEntityDetector DetectorForDownData2 { get; set; }
        private MyEntityDetector DetectorForExitArea1 { get; set; }
        private MyEntityDetector DetectorForExitArea2 { get; set; }
        
        private MyEntityDetector PirateMothershipTrigger { get; set; }

        private MyObjective m_returnToMeetingPointSubmission;
        private MyObjective m_sneakInsideMainBaseSubmission;

        private bool m_movePirateMothership;
        private float m_pirateMothershipSpeed = 100;
        private Vector3 m_pirateMothershipTargetPosition = new Vector3(7151, 1314, -4923);

        public MyRussianWarehouseMission()
        {
            MyMwcVector3Int baseSector = new MyMwcVector3Int(-7420630, 0, 388170);

            ID = MyMissionID.RUSSIAN_WAREHOUSE;
            DebugName = new StringBuilder("05-Russian warehouse");
            Name = MyTextsWrapperEnum.RUSSIAN_WAREHOUSE;
            Description = MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_Description;
            Flags = MyMissionFlags.Story;
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.PIRATE_BASE };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_MEETINGPOINT };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>();
            m_sneakInsideMainBaseSubmission = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_SNEAKINMAINBASE_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_SNEAKINMAINBASE,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_SNEAKINMAINBASE_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.SneakInsideSubmissionLocation),
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0100
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudEntrance };
            m_sneakInsideMainBaseSubmission.OnMissionLoaded += SneakInsideMainBaseSubmission_OnMissionLoaded;
            m_objectives.Add(m_sneakInsideMainBaseSubmission);
            
            var TurnOffSecurityCams = new MyObjectiveDisablePrefabs(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFCAMS_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFCAMS,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFCAMS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_SNEAKINMAINBASE },
                new List<uint>() { (uint)EntityID.TurnOffCamera0, (uint)EntityID.TurnOffCamera1, (uint)EntityID.TurnOffCamera2 },
                new List<uint>() { (uint)EntityID.TurnOffHUB },
                markObjectsToDisable: false,
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0200
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudSecurityHub };
            m_objectives.Add(TurnOffSecurityCams);
            TurnOffSecurityCams.OnMissionLoaded += TurnOffSecurityCams_OnMissionLoaded;

            var ControlTurret = new MyObjectiveDisablePrefabs(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_CTRLTURRET_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_CTRLTURRET,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_CTRLTURRET_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFCAMS },
                new List<uint>() { (uint)EntityID.SecurityTower1, (uint)EntityID.SecurityTower2, (uint)EntityID.SecurityTower3, (uint)EntityID.SecurityTower4, (uint)EntityID.SecurityTower5 },
                new List<uint>() { (uint)EntityID.SecurityTowersHUB },
                markObjectsToDisable: false
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudSecurityHub };
            ControlTurret.OnMissionLoaded += ControlTurretOnLoaded;
            ControlTurret.OnMissionSuccess += ControlTurretOnSuccess;
            m_objectives.Add(ControlTurret);

            var OpenDoor = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_OPEN_DOORS_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_OPEN_DOORS,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_OPEN_DOORS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_CTRLTURRET },
                null,
                new List<uint> { (uint)EntityID.OpenDoorHUB},
                new List<uint> { (uint)EntityID.GetOutsideDoors },
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0300
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHub };
            m_objectives.Add(OpenDoor);

            var GetOutsideMainBase = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_GETOUTMAINBASE_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_GETOUTMAINBASE,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_GETOUTMAINBASE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_OPEN_DOORS },
                new MyMissionLocation(baseSector, (uint)EntityID.GetOutsideSubmissionLocation),
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0400
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            GetOutsideMainBase.OnMissionLoaded += new MissionHandler(GetOutsideMainBase_OnMissionLoaded);
            m_objectives.Add(GetOutsideMainBase);

            var BreakThruOldEntrance = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_BREAKOLDENTRANCE_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_BREAKOLDENTRANCE,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_BREAKOLDENTRANCE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_GETOUTMAINBASE },
                new MyMissionLocation(baseSector, (uint)EntityID.BreakThroughSubmissionLocation),
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_1100
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            BreakThruOldEntrance.OnMissionLoaded += BreakThruOldEntrance_OnMissionLoaded;
            BreakThruOldEntrance.OnMissionUpdate += UpdateRussianMothershipMovement;
            m_objectives.Add(BreakThruOldEntrance);

            var LocateWarehouse = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_LOCATEWAREHOUSE_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_LOCATEWAREHOUSE,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_LOCATEWAREHOUSE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_BREAKOLDENTRANCE },
                new MyMissionLocation(baseSector, (uint)EntityID.LocateWarehouseSubmissionLocation),
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0500
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            LocateWarehouse.OnMissionUpdate += UpdateRussianMothershipMovement;
            LocateWarehouse.OnMissionLoaded += LocateWarehouse_OnMissionLoaded;
            LocateWarehouse.OnMissionCleanUp += LocateWarehouse_OnMissionUnload;
            m_objectives.Add(LocateWarehouse);

            var FindTransmitter = new MyUseObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_FINDTRANSMITTER_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_FINDTRANSMITTER,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_FINDTRANSMITTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_LOCATEWAREHOUSE },
                new MyMissionLocation(baseSector, (uint)EntityID.TransmitterCargoBox),
                Localization.MyTextsWrapperEnum.PressToTakeCargo,
                Localization.MyTextsWrapperEnum.TakeAll,
                Localization.MyTextsWrapperEnum.TakeAll,
                1000,
                radiusOverride: 50,
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0700
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudTransmitter };
            FindTransmitter.OnMissionUpdate += UpdateRussianMothershipMovement;
            FindTransmitter.OnMissionLoaded += new MissionHandler(FindTransmitter_OnMissionLoaded);
            FindTransmitter.OnMissionSuccess += new MissionHandler(FindTransmitter_OnMissionSuccess);
            m_objectives.Add(FindTransmitter);

            var downDataDlgSubmission = new MyObjectiveDialog(
                    MyMissionID.RUSSIAN_WAREHOUSE_DOWNDATADIALOGUE,
                    null,
                    this,
                    new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_FINDTRANSMITTER },
                    MyDialogueEnum.RUSSIAN_WAREHOUSE_0800
                );
            downDataDlgSubmission.OnMissionUpdate += UpdateRussianMothershipMovement;
            m_objectives.Add(downDataDlgSubmission);

            var DownData = new MyUseObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_DOWNLOAD_DATA_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_DOWNLOAD_DATA,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_DOWNLOAD_DATA_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_DOWNDATADIALOGUE },
                new MyMissionLocation(baseSector, (uint)EntityID.DownloadDataDummy),
                Localization.MyTextsWrapperEnum.PressToDownloadData,
                Localization.MyTextsWrapperEnum.Console,
                Localization.MyTextsWrapperEnum.DownloadingData,
                3000) { SaveOnSuccess = true };
            DownData.OnMissionUpdate += UpdateRussianMothershipMovement;
            DownData.OnMissionLoaded += DownDataOnLoaded;
            m_objectives.Add(DownData);

            var TurnOffMainDefense1 = new MyObjectiveDisablePrefabs(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_DOWNLOAD_DATA },
                new List<uint>() { (uint)EntityID.CargoTurret0, (uint)EntityID.CargoTurret1, (uint)EntityID.CargoTurret2, (uint)EntityID.CargoTurret3, (uint)EntityID.CargoTurret4 },
                new List<uint>() { (uint)EntityID.CargoTurretHUB0 },
                markObjectsToDisable: false,
                startDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_0900
            ) { HudName = MyTextsWrapperEnum.HudHubA };
            TurnOffMainDefense1.OnMissionUpdate += UpdateRussianMothershipMovement;
            m_objectives.Add(TurnOffMainDefense1);

            var TurnOffMainDefense2 = new MyObjectiveDisablePrefabs(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART2_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART2,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART2_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1 },
                new List<uint>() { (uint)EntityID.CargoTurret5, (uint)EntityID.CargoTurret6, (uint)EntityID.CargoTurret7, (uint)EntityID.CargoTurret8, (uint)EntityID.CargoTurret9 },
                new List<uint>() { (uint)EntityID.CargoTurretHUB1 },
                markObjectsToDisable: false
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHubB };
            TurnOffMainDefense2.OnMissionUpdate += UpdateRussianMothershipMovement;
            m_objectives.Add(TurnOffMainDefense2);

            var GetOutsideWarehouse = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_GETOUTWAREHOUSE_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_GETOUTWAREHOUSE,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_GETOUTWAREHOUSE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1, MyMissionID.RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART2 },
                new MyMissionLocation(baseSector, (uint)EntityID.GetOutsideThruCargoSubmissionLocation)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            GetOutsideWarehouse.OnMissionUpdate += UpdateRussianMothershipMovement;
            GetOutsideWarehouse.OnMissionLoaded += GetOutsideWarehouseOnLoad;
            GetOutsideWarehouse.OnMissionCleanUp += GetOutsideWarehouseOnCleanUp;
            m_objectives.Add(GetOutsideWarehouse);

            var CrushRemainingShips = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_CRUSHREMAINGSHIPS_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_CRUSHREMAINGSHIPS,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_CRUSHREMAINGSHIPS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_GETOUTWAREHOUSE },
                new List<uint> { },
                new List<uint> { (uint)EntityID.CrushRussiansSpawnpoint },
                false
            ) { SaveOnSuccess = true };
            CrushRemainingShips.OnMissionUpdate += UpdateRussianMothershipMovement;
            CrushRemainingShips.OnMissionLoaded += CrushRemainingShipsOnLoad;
            m_objectives.Add(CrushRemainingShips);

            m_returnToMeetingPointSubmission = new MyObjective(
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_MEETINGPOINT_Name),
                MyMissionID.RUSSIAN_WAREHOUSE_MEETINGPOINT,
                (MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_MEETINGPOINT_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RUSSIAN_WAREHOUSE_CRUSHREMAINGSHIPS },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                successDialogId: MyDialogueEnum.RUSSIAN_WAREHOUSE_1000,
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            m_returnToMeetingPointSubmission.OnMissionUpdate += UpdateRussianMothershipMovement;
            m_returnToMeetingPointSubmission.OnMissionUpdate += new MissionHandler(ReturnToMeetingPointSubmissionOnUpdate);
            m_objectives.Add(m_returnToMeetingPointSubmission);

            Components.Add(new MySpawnpointWaves((uint)EntityID.RaidDetector, 0, (uint)EntityID.RaidSpawnpoint));
        }

        void FindTransmitter_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.TryHide((uint)EntityID.TransmitterCargoBox);            
        }

        void FindTransmitter_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.TransmitterCargoBox, true, this);
        }

        void GetOutsideMainBase_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((uint)EntityID.GetOutsideDoors), true);
        }

        void BreakThruOldEntrance_OnMissionLoaded(MyMissionBase sender)
        {
            var mothership = GetEntity(EntityID.MovingMothershipContainer);
            var particle1 = GetEntity(EntityID.MovingMothershipContainerParticles1);
            var particle2 = GetEntity(EntityID.MovingMothershipContainerParticles2);

            MyScriptWrapper.UnhideEntity(mothership, true);
            MyScriptWrapper.UnhideEntity(particle1);
            MyScriptWrapper.UnhideEntity(particle2);

            MyDummyPoint motherShipStart = MyScriptWrapper.TryGetEntity("motherShipStart") as MyDummyPoint;
            MyDummyPoint particle1Start = MyScriptWrapper.TryGetEntity("particle1Start") as MyDummyPoint;
            MyDummyPoint particle2Start = MyScriptWrapper.TryGetEntity("particle2Start") as MyDummyPoint;

            if (motherShipStart == null)
            {
                motherShipStart = MyEntities.CreateFromObjectBuilderAndAdd(null, MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null), mothership.WorldMatrix) as MyDummyPoint;
                motherShipStart.SetName("motherShipStart");
            }
            else
                motherShipStart.WorldMatrix = mothership.WorldMatrix;

            if (particle1Start == null)
            {
                particle1Start = MyEntities.CreateFromObjectBuilderAndAdd(null, MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null), particle1.WorldMatrix) as MyDummyPoint;
                particle1Start.SetName("particle1Start");
            }
            else
                particle1Start.WorldMatrix = mothership.WorldMatrix;

            if (particle2Start == null)
            {
                particle2Start = MyEntities.CreateFromObjectBuilderAndAdd(null, MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null), particle2.WorldMatrix) as MyDummyPoint;
                particle2Start.SetName("particle2Start");
            }
            else
                particle2Start.WorldMatrix = mothership.WorldMatrix;

            MyScriptWrapper.MarkEntity(mothership, MyTexts.RussianReinforcements, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE, MyGuitargetMode.Enemy);
        }

        void UpdateRussianMothershipMovement(MyMissionBase sender)
        {
            MoveRussianMothership();
        }

        void SneakInsideMainBaseSubmission_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetWaypointListSecrecy(m_waypoints, true);

            MyScriptWrapper.HideEntity(GetEntity(EntityID.MovingMothershipContainer), true);
            MyScriptWrapper.HideEntity(GetEntity(EntityID.MovingMothershipContainerParticles1));
            MyScriptWrapper.HideEntity(GetEntity(EntityID.MovingMothershipContainerParticles2));
        }

        void TurnOffSecurityCams_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetWaypointListSecrecy(m_waypoints, false);
        }

        void LocateWarehouse_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.EntityDeath += LocateWarehouse_EntityDeath;
        }

        void LocateWarehouse_OnMissionUnload(MyMissionBase sender)
        {
            MyScriptWrapper.EntityDeath -= LocateWarehouse_EntityDeath;
        }

        void LocateWarehouse_EntityDeath(MyEntity entity, MyEntity damageSource)
        {
            if (entity == MyScriptWrapper.TryGetEntity((uint)EntityID.LastHatch))
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RUSSIAN_WAREHOUSE_0600);
            }
        }

        void DownDataOnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.LateWarehouseSpawnpoint);
        }

        void ControlTurretOnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.EntranceDoors), false);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointCtrlTurret);
        }

        void ControlTurretOnSuccess(MyMissionBase sender)
        {
            var inventory = MyScriptWrapper.GetPlayerInventory();
            bool removed = inventory.RemoveInventoryItemAmount(
                MyMwcObjectBuilderTypeEnum.FalseId,
                MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Russian),
                1);
            MyScriptWrapper.FixBotNames();
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.OpenDoorsSpawnpoint);
        }

        void GetOutsideWarehouseOnLoad(MyMissionBase sender)
        {
            PirateMothershipTrigger = InitDetector((uint)EntityID.PirateMothershipTrigger, OnPirateMothershipTrigger);

            MyScriptWrapper.SetEntityDestructible(MyScriptWrapper.GetEntity((uint)EntityID.LeftCargoDoors), true);
        }

        void GetOutsideWarehouseOnCleanUp(MyMissionBase sender)
        {
            CleanUpDetector(PirateMothershipTrigger, OnPirateMothershipTrigger);
        }

        void CrushRemainingShipsOnLoad(MyMissionBase sender)
        {
            m_movePirateMothership = true;
        }

        void ReturnToMeetingPointSubmissionOnUpdate(MyMissionBase sender)
        {
            m_movePirateMothership = true;
        }

        public override void Load()
        {
            base.Load();

            // Add hacking tool to inventory if player already haven't got one
            MyScriptWrapper.AddHackingToolToPlayersInventory(2);

            // Set musicmood right from script start
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight);

            // Change player faction to Rainiers and set relations between them
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Pirates, MyFactions.RELATION_BEST);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian, MyFactions.RELATION_WORST);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Russian, MyFactions.RELATION_WORST);
            MyScriptWrapper.FixBotNames();

            // Permaclosed doors
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.UpperOldEntranceDoor1), false);   // Upper old entrance tunnel 1st door
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.UpperOldEntranceDoor2), false);   // Upper old entrance tunnel 2nd door
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.LeftCargoDoors), false);          // Left cargo bay doors which leads to nowhere

            // Closed doors on mission start
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.OldEntranceDoor1), false);        // Should be opened from HUB at main base - it's 1st door at old entrance
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.OldEntranceDoor2), false);        // Should be opened from HUB at main base - it's 2nd door at old entrance
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.WarehouseDoors), false);          // Should be opened from HUB at old entrance - leads to warehouse
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.GetOutsideDoors), false);         // Should open for GetOutsideMainBase submission
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.RightCargoDoors), false);         // Right cargo bay doors which leads to outside of Warehouse Station
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.MainbaseWarehouseDoors), false);  // Doors at connection tunnel between main base and warehouse base (from side of main base)
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.WarehouseMainbaseDoors), false);  // Doors at connection tunnel between main base and warehouse base (from side of warehouse base)
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.ToWarehouseDoors), false);        // Doors at connection tunnel between main base and warehouse base which leads to warehouse itself
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.MainChamberDoors), false);        // Doors at main chamber after cargobay which leads to warehouse itself
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.AbandonedPartDoors), false);      // Doors to abandoned part of warehouse

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint3);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint4);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSlaversRight);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSlaversLeft);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSlaversMiddle);

            // Detectors Dummy points
            Detector02 = InitDetector((uint)EntityID.Detector02, StartDetector02);
            Detector03 = InitDetector((uint)EntityID.Detector03, StartDetector03);
            Detector04 = InitDetector((uint)EntityID.Detector04, StartDetector04);
            Detector05 = InitDetector((uint)EntityID.Detector05, StartDetector05);
            Detector06 = InitDetector((uint)EntityID.Detector06, StartDetector06);
            Detector07 = InitDetector((uint)EntityID.Detector07, StartDetector07);
            Detector08 = InitDetector((uint)EntityID.Detector08, StartDetector08);
            Detector09 = InitDetector((uint)EntityID.Detector09, StartDetector09);
            Detector10 = InitDetector((uint)EntityID.Detector10, StartDetector10);
            Detector11 = InitDetector((uint)EntityID.Detector11, StartDetector11);
            Detector12 = InitDetector((uint)EntityID.Detector12, StartDetector12);

            DetectorBonus1 = InitDetector((uint)EntityID.DetectorBonus1,StartDetectorBonus1);
            DetectorBonus2 = InitDetector((uint)EntityID.DetectorBonus2,StartDetectorBonus2);
            DetectorBonus3 = InitDetector((uint)EntityID.DetectorBonus3,StartDetectorBonus3);
            DetectorBonus4 = InitDetector((uint)EntityID.DetectorBonus4,StartDetectorBonus4);
            DetectorBonus5 = InitDetector((uint)EntityID.DetectorBonus5,StartDetectorBonus5);

            DetectorForUpperTurretHub = InitDetector((uint)EntityID.DetectorForUpperTurretHub, StartDetectorForUpperTurretHub);
            DetectorForLowerTurretHub = InitDetector((uint)EntityID.DetectorForLowerTurretHub, StartDetectorForLowerTurretHub);
            DetectorForDownData1 = InitDetector((uint)EntityID.DetectorForDownData1, StartDetectorForDownData1);
            DetectorForDownData2 = InitDetector((uint)EntityID.DetectorForDownData2, StartDetectorForDownData2);
            DetectorForExitArea1 = InitDetector((uint)EntityID.DetectorForExitArea1, StartDetectorForExitArea1);
            DetectorForExitArea2 = InitDetector((uint)EntityID.DetectorForExitArea2, StartDetectorForExitArea2);

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            m_movePirateMothership = false;
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (spawnpoint.EntityId.Value.NumericValue == (uint)EntityID.SpawnpointSlaversRight)
            {
                MyScriptWrapper.SetBotTarget(bot, MyScriptWrapper.GetEntity((uint)EntityID.SlaversTarget0));
            }
        }

        public override void Update()
        {
            base.Update();

            if (m_movePirateMothership)
            {
                MovePirateShip();
            }
        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;

            CleanUpDetector(Detector01, StartDetector01);
            CleanUpDetector(Detector02, StartDetector02);
            CleanUpDetector(Detector03, StartDetector03);
            CleanUpDetector(Detector04, StartDetector04);
            CleanUpDetector(Detector05, StartDetector05);
            CleanUpDetector(Detector06, StartDetector06);
            CleanUpDetector(Detector07, StartDetector07);
            CleanUpDetector(Detector08, StartDetector08);
            CleanUpDetector(Detector09, StartDetector09);
            CleanUpDetector(Detector10, StartDetector10);
            CleanUpDetector(Detector11, StartDetector11);
            CleanUpDetector(Detector12, StartDetector12);
            
            CleanUpDetector(DetectorBonus1, StartDetectorBonus1);
            CleanUpDetector(DetectorBonus2, StartDetectorBonus2);
            CleanUpDetector(DetectorBonus3, StartDetectorBonus3);
            CleanUpDetector(DetectorBonus4, StartDetectorBonus4);
            CleanUpDetector(DetectorBonus5, StartDetectorBonus5);

            CleanUpDetector(DetectorForUpperTurretHub, StartDetectorForUpperTurretHub);
            CleanUpDetector(DetectorForLowerTurretHub, StartDetectorForLowerTurretHub);
            CleanUpDetector(DetectorForDownData1, StartDetectorForDownData1);
            CleanUpDetector(DetectorForDownData2, StartDetectorForDownData2);
            CleanUpDetector(DetectorForExitArea1, StartDetectorForExitArea1);
            CleanUpDetector(DetectorForExitArea2, StartDetectorForExitArea2);
        }

        private void StartDetector01(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint3);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint4);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSlaversRight);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSlaversLeft);
                sender.Off();
            }
        }

        private void StartDetector02(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint02);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint03);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawnpoint5);
                sender.Off();
            }
        }

        private void StartDetector03(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint04);
                sender.Off();
            }
        }

        private void StartDetector04(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyScriptWrapper.ActivateSpawnPoint(156386);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint01);
                sender.Off();
            }
        }

        private void StartDetector05(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint07);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                //MyScriptWrapper.ActivateSpawnPoint(164174);
                //MyScriptWrapper.ActivateSpawnPoint(156386);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint01);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyA);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyB);
                sender.Off();
            }
        }

        private void StartDetector06(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint07);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                //MyScriptWrapper.ActivateSpawnPoint(164174);
                //MyScriptWrapper.ActivateSpawnPoint(156386);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint01);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyA);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyB);
                sender.Off();
            }
        }

        private void StartDetector07(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint07);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                //MyScriptWrapper.ActivateSpawnPoint(164174);
                //MyScriptWrapper.ActivateSpawnPoint(156386);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint01);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyA);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyB);
                sender.Off();
            }
        }

        private void StartDetector08(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint07);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                //MyScriptWrapper.ActivateSpawnPoint(164174);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyA);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointSeekAndDestroyB);
                sender.Off();
            }
        }

        private void StartDetector09(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint05);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint06);
                sender.Off();
            }
        }

        private void StartDetector10(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CrushRussiansSpawnpoint);
                sender.Off();
            }
        }

        private void StartDetector11(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointOnRoadToOldEntr1);
                sender.Off();
            }
        }

        private void StartDetector12(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointOnRoadToOldEntr2);
                sender.Off();
            }
        }

        private void OnPirateMothershipTrigger(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            m_movePirateMothership = true;
        }

        private void StartDetectorBonus1(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBonus1);
                sender.Off();
            }
        }

        private void StartDetectorBonus2(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint02);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBonus2);
                sender.Off();
            }
        }

        private void StartDetectorBonus3(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBonus3);
                sender.Off();
            }
        }

        private void StartDetectorBonus4(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBonus4);
                sender.Off();
            }
        }

        private void StartDetectorBonus5(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointBonus5);
                sender.Off();
            }
        }

        private void StartDetectorForUpperTurretHub(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint07);
                sender.Off();
            }
        }

        private void StartDetectorForLowerTurretHub(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint08);
                sender.Off();
            }
        }

        private void StartDetectorForDownData1(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                sender.Off();
            }
        }

        private void StartDetectorForDownData2(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint09);
                sender.Off();
            }
        }

        private void StartDetectorForExitArea1(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                sender.Off();
            }
        }

        private void StartDetectorForExitArea2(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SomeSpawnpoint10);
                sender.Off();
            }
        }

        private void MovePirateShip()
        {
            var pirateMotherShipContainer = GetEntity(EntityID.PirateMothershipContainer);
            if (pirateMotherShipContainer != null && Vector3.DistanceSquared(m_pirateMothershipTargetPosition, pirateMotherShipContainer.GetPosition()) > 10 * 10)
            {
                Vector3 direction = MyMwcUtils.Normalize(m_pirateMothershipTargetPosition - pirateMotherShipContainer.GetPosition());
                Vector3 deltaPosition = direction * m_pirateMothershipSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                MyScriptWrapper.Move(pirateMotherShipContainer, pirateMotherShipContainer.GetPosition() + deltaPosition);

                var thrust0 = GetEntity(EntityID.PirateMothershipThrust0);
                var thrust1 = GetEntity(EntityID.PirateMothershipThrust1);
                var thrust2 = GetEntity(EntityID.PirateMothershipThrust2);

                MyScriptWrapper.Move(thrust0, thrust0.GetPosition() + deltaPosition);
                MyScriptWrapper.Move(thrust1, thrust1.GetPosition() + deltaPosition);
                MyScriptWrapper.Move(thrust2, thrust2.GetPosition() + deltaPosition);
            }
        }

        private void MoveRussianMothership()
        {
            var motherShipStart = MyScriptWrapper.TryGetEntity("motherShipStart");
            if (motherShipStart != null)
            {
                var particle1Start = MyScriptWrapper.GetEntity("particle1Start");
                var particle2Start = MyScriptWrapper.GetEntity("particle2Start");

                var mothership = GetEntity(EntityID.MovingMothershipContainer);
                var particle1 = GetEntity(EntityID.MovingMothershipContainerParticles1);
                var particle2 = GetEntity(EntityID.MovingMothershipContainerParticles2);

                var mothershipTarget = GetEntity(EntityID.MovingMothershipTarget);

                var distance = (mothershipTarget.GetPosition() - motherShipStart.GetPosition()).Length();
                var traveledDistance = (motherShipStart.GetPosition() - mothership.GetPosition()).Length();
                var phase =  traveledDistance / distance;

                phase += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS / (6 * 60);   // 6 minutes travel time

                Vector3 position = Vector3.Lerp(motherShipStart.GetPosition(), mothershipTarget.GetPosition(), phase);

                MyScriptWrapper.Move(mothership, position);
                MyScriptWrapper.Move(particle1, particle1Start.GetPosition() - motherShipStart.GetPosition() + position);
                MyScriptWrapper.Move(particle2, particle2Start.GetPosition() - motherShipStart.GetPosition() + position);

                if (distance - traveledDistance < 10f)
                {
                    motherShipStart.MarkForClose();
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.MovingMothershipSpawnpoint1);
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.MovingMothershipSpawnpoint2);

                    MyScriptWrapper.RemoveEntityMark(mothership);
                }
            }
        }

        private MyEntityDetector InitDetector(uint detectorID, OnEntityEnter handler)
        {
            MyEntityDetector detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(detectorID));
            detector.OnEntityEnter += handler;
            detector.On();
            return detector;
        }

        private void CleanUpDetector(MyEntityDetector detector, OnEntityEnter handler)
        {
            if (detector != null)
            {
                detector.OnEntityEnter -= handler;
                detector.Off();
            }
        }
    }
}

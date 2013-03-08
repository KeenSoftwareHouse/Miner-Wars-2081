using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    internal class MyDeadlyScanners : MyMissionComponent
    {
        private List<uint> m_scannersIds;
        private List<MyPrefabScanner> m_scanners;

        private List<int> m_scannersStartTimes;
        private List<int> m_scannersUpdateTimes;

        private bool m_isFirstUpdate;
        public MyDeadlyScanners(
                            List<uint> scanners,
                            List<int> scannersUpdateTimes)
        {
            m_scannersIds = new List<uint>();
            m_scannersIds.AddRange(scanners);
            m_scannersUpdateTimes = scannersUpdateTimes;
            Debug.Assert(scanners.Count == scannersUpdateTimes.Count, "Bad parameters of constructor");
        }

        public override void Load(MyMissionBase sender)
        {
           
            m_scanners = new List<MyPrefabScanner>();
            m_isFirstUpdate = true;
            m_scannersStartTimes = new List<int>();
            foreach (var mScannersId in m_scannersIds)
            {
                var scanner = MyScriptWrapper.GetEntity(mScannersId) as MyPrefabScanner;
                scanner.OnEntityScanned += ScannerOnOnEntityScanned;
                m_scanners.Add(scanner);
                m_scannersStartTimes.Add(0);
            }

            
        }

        private void ScannerOnOnEntityScanned(MyPrefabScanner sender, MyEntity scannedEntity)
        {
            if (scannedEntity == MySession.PlayerShip && m_scanners.Contains(sender))
            {
                scannedEntity.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                ((MyPrefabContainer) sender.Parent).AlarmOn = true;
            }
        }

        public override void Update(MyMissionBase sender)
        {
            if (m_isFirstUpdate)
            {
                for (int i = 0; i < m_scannersStartTimes.Count; i++)
                {
                    m_scannersStartTimes[i] = sender.MissionTimer.ElapsedTime + MyMwcUtils.GetRandomInt(0, 3)*1000;
                }
                m_isFirstUpdate = false;
            }
            else
            {
                var index =0;
                foreach (MyEntity scanner in m_scanners)
                {
                    if (m_scannersStartTimes[index] < sender.MissionTimer.ElapsedTime)
                    {
                        scanner.Enabled = !scanner.Enabled;
                        m_scannersStartTimes[index] = sender.MissionTimer.ElapsedTime + m_scannersUpdateTimes[index];
                    }
                    index++;
                }
            }

        }

        public override void Unload(MyMissionBase sender)
        {
            foreach (var scanner in m_scanners)
            {
                scanner.OnEntityScanned -= ScannerOnOnEntityScanned;
            }

            m_scanners.Clear();
            //m_scannersStartTimes.Clear();
        }
    }

    internal abstract class MyFortValiantMissionBase : MyMission
    {
        protected MySmallShipBot m_captain;
        protected MySmallShipBot m_gateKeeper;
        private List<uint> SpawnPoints1 = new List<uint>(){17157, 17155, 94434, 17146, 17150, 17132 };
        private List<uint> SpawnPoints2 = new List<uint>() { 17150, 17132, 17149, 17147, 17152, 23874 };
        private List<uint> SpawnPoints3 = new List<uint>() { 90075, 15948 };
        private List<uint> SpawnPoints4 = new List<uint>() { 17153, 17154 };
        private List<uint> SpawnPoints5 = new List<uint>() { 93699, 17148 };

        protected enum EntityID // list of IDs used in script
        {
            StartLocation = 80570,
            VendorSpeakDetector1 = 164699,
            DetecorGateKeeper = 143240, 
            DetectorCaptain = 143242,
            Vendor = 174831, 
            VendorSpeakDetector = 80571 ,
            //MeetTemplarRepresentatives = 87795,
            //ReturnBackToMadelyn = 97026,
            //FindVentEntrance = 87868,
            //GetToElevatorShaft = 87881,
            //GetToCaveEntrance = 88117,
            Box1Marker = 88105,
            Box2Marker = 88103,
            Box3Marker = 88106,
            Box4Marker = 88108,
            ArtifactDummy = 88121,
            PrefabArtifact = 170398,           
            //SpawnPointToJoin = 17157,           
            MadelynLocation = 97959,
            //ActivateSlaveMission = 100640,

            Captain = 143227,
            GateKeeper  = 143214,
            //Alarm = 153297,
            //Alarm2 = 148633,
            //Alarm3 = 153310,
            Detector1 = 153685,
            Detector2 = 153673,
            Detector3 = 153669,
            Detector4 = 153671,
            Detector5 = 16777667,
            MadelynDummy = 143755,
            PrefabContainer = 143744,
            Scanner1 = 152953,
            Scanner2 = 165559,
            VendorB = 174833,
            MadelyDummyB = 143759,

            StartLocationC = 16777666,
            BotRoyal = 144286 ,
            BotTemplar = 87796,
            Generator1 = 152943,
            CaptainDummy = 144265,
            UpperFloorDummy = 144267,
            RoyalDummy = 144268,
            RoyalCargoBoxDetector = 144270,
            OfficalsDetector = 87795,
            OfficalsleaveDetector = 144308,
            FollowPathDetector = 144309,
            SirBendivereDummy = 144334 ,
            CargoBox = 87444,
            Ventilation = 87868,
            SpawnAlarm1 = 142111, 
            SpawnAlarm2 = 142109,
            DisableScanner = 172860,
            DisableScanner2Hub = 170056,
            ScannersLocation1 = 153677,
            //ScannersTalkDummy = 153678,
            //ScannersEndGameDummy = 153322,
            ScannersLocation2 = 88082,
            //DisableScanner2Detector = 153324,
            DisableScannerHub = 170058,
            ElevatorDummy = 144338,
            EscapeVentSystemDummy = 144340,
            EscapeCatacombsDummy = 144341,
            //CargoBoxWayBack = 144270,
            DisableSnanners2 = 172862,
            DialogDummy2 = 144336 ,
            ScannersLocation23 = 88117,
            HigherFloors = 172502,
            FakeCargo = 173576,


            DontuseHarvestor1 =  173927,
            //DontuseHarvestor2 = 153669,

            DetectorGuardsDialogue = 1,
            DetectorSensorsDialogue = 3,
            DetectorSecurityFields = 11,
            DetectorOpenDoors = 13,
            DetectorComputer = 15,
            CameraLookThroughID = 142450,
            GetItemsWayBackDialogue = 18,
            
        }

        protected MyMwcVector3Int baseSector;
        private List<MyEntity> m_entities = new List<MyEntity>();
        private bool m_harvesterWarningSend;

        public MyFortValiantMissionBase()
        {
            Flags = MyMissionFlags.Story;
            baseSector = new MyMwcVector3Int(-588410, 0, -3425542); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.JUNKYARD_RETURN }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.FORT_VALIANT_RETURN_BACK_TO_MADELYN_2 };
        }


        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            MyScriptWrapper.AlarmLaunched += MyScriptWrapperOnAlarmLaunched;

            MyScriptWrapper.OnHarvesterUse += MyScriptWrapperOnOnHarvesterUse;
            
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.BotTemplar));
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.SunWind, false);
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.MeteorWind, false);
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.IceStorm, false);


            m_captain = MyScriptWrapper.GetEntity((uint) EntityID.Captain) as MySmallShipBot;
            m_gateKeeper = MyScriptWrapper.GetEntity((uint) EntityID.GateKeeper) as MySmallShipBot;
            m_captain.SpeedModifier = 0.25f;
            m_gateKeeper.SpeedModifier = 0.25f;

            m_captain.SetWaypointPath("Captain");
            m_captain.PatrolMode = MyPatrolMode.CYCLE;
            m_captain.SpeedModifier = 0.25f;
            m_captain.Patrol();

            m_gateKeeper.SetWaypointPath("GateKeep");
            m_gateKeeper.PatrolMode = MyPatrolMode.CYCLE;
            m_gateKeeper.SpeedModifier = 0.25f;
            m_gateKeeper.Patrol();

            InitDetector((uint)EntityID.Detector1, OnDetecor1Active);
            InitDetector((uint)EntityID.Detector2, OnDetecor2Active);
            InitDetector((uint)EntityID.Detector3, OnDetecor3Active);
            InitDetector((uint)EntityID.Detector4, OnDetecor4Active);
            InitDetector((uint)EntityID.Detector5, OnDetecor5Active);


            base.Load();
        }

        public override void Unload()
        {
            MyScriptWrapper.OnHarvesterUse -= MyScriptWrapperOnOnHarvesterUse;
            MyScriptWrapper.AlarmLaunched -= MyScriptWrapperOnAlarmLaunched;

            base.Unload();
        }

        private void OnDetecor5Active(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            ActivateSpawnPoints(SpawnPoints5);
        }


        private void OnDetecor4Active(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            ActivateSpawnPoints(SpawnPoints4);
        }

        private void OnDetecor3Active(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            ActivateSpawnPoints(SpawnPoints3);
        }

        private void OnDetecor2Active(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            ActivateSpawnPoints(SpawnPoints2);
        }

        private void OnDetecor1Active(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            ActivateSpawnPoints(SpawnPoints1);
        }

        private void ActivateSpawnPoints(List<uint> spawnPoints)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnPoint);
            }
        }

        protected MyEntityDetector InitDetector(uint detectorID, OnEntityEnter handler)
        {
            MyEntityDetector detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(detectorID));
            detector.OnEntityEnter += handler;
            detector.On();
            return detector;
        }

        protected virtual void MyScriptWrapperOnAlarmLaunched(MyEntity prefabContainer, MyEntity launchedBy)
        {
            Fail(Localization.MyTextsWrapperEnum.YouWereDetected);
        }







        #region HelpMethods

        protected static void EnableEntities(List<uint> toEnable)
        {
            foreach (var u in toEnable)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(u), true);
            }
        }

        protected static void DisableEntities(List<uint> toEnable)
        {
            foreach (var u in toEnable)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(u), false);
            }
        }
        #endregion

        protected void MyScriptWrapperOnOnHarvesterUse()
        {
            bool harvestingIn = false;
            MyScriptWrapper.GetEntitiesInDummyPoint((uint) EntityID.DontuseHarvestor1, m_entities);

            if (m_entities.Contains(MySession.PlayerShip)) harvestingIn = true;

            MyScriptWrapper.GetEntitiesInDummyPoint((uint)EntityID.Detector3, m_entities);

            if (m_entities.Contains(MySession.PlayerShip)) harvestingIn = true;

            if(harvestingIn)
            {   
                if (m_harvesterWarningSend)
                {
                    Fail(Localization.MyTextsWrapperEnum.DontHarvest);
                }
                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.DontHarvest,MyGuiManager.GetFontMinerWarsRed(),5000));
                m_harvesterWarningSend = true;
            }
 

        }
    }
}
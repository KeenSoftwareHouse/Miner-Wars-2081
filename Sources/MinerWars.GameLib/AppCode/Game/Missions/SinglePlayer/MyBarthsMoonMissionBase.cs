using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWarsMath;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyBarthsMoonMissionBase : MyMission
    {
        protected class MyBarthsMoonSubmissionTalkWithThomasBarth : MyObjective
        {
            private uint m_botDetectorId;
            private uint m_botToTalkId;            
            private int m_distanceToTalk;
            private bool m_restartPatrol;
            private string m_pathName;

            private MySmallShipBot m_botToTalk;
            private MyEntityDetector m_botDetector;

            public MyBarthsMoonSubmissionTalkWithThomasBarth(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyMission ParentMission, MyMissionID[] RequiredMissions, MyDialogueEnum? dialogue, bool restartPatrol = true, MyDialogueEnum? startDialogue = null, string pathName = "interior2")
                : base(Name, ID, Description, null, ParentMission, RequiredMissions, null, null, dialogue) 
            {                
                m_botToTalkId = (uint)EntityID.ThomasBartId;

                m_botDetectorId = (uint)EntityID.ThomasBartDetectorId;
                m_distanceToTalk = 100;
                m_restartPatrol = restartPatrol;
                m_pathName = pathName;
                MissionEntityIDs.Add(m_botToTalkId);
            }

            public override void Load()
            {
                base.Load();
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not
                m_botDetector = MyScriptWrapper.GetDetector(m_botDetectorId);
                m_botDetector.On();
                m_botDetector.OnEntityEnter += m_botDetector_OnEntityEnter;
                m_botToTalk = MyScriptWrapper.GetEntity(m_botToTalkId) as MySmallShipBot;
                m_botToTalk.SpeedModifier = 0.25f;
                SetPatrolMode();
                
                Debug.Assert(m_botToTalk != null);                
            }

            private void SetPatrolMode() 
            {
                m_botToTalk.LookTarget = null;
                m_botToTalk.SetWaypointPath(m_pathName);
                m_botToTalk.Patrol();
            }

            void m_botDetector_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
            {
                MyScriptWrapper.Follow(MySession.PlayerShip, m_botToTalk);
                sender.Off();
            }            

            public override bool IsSuccess()
            {
                if (m_botToTalk != null && Vector3.DistanceSquared(m_botToTalk.GetPosition(), MySession.PlayerShip.GetPosition()) <= m_distanceToTalk * m_distanceToTalk)
                {
                    StopFollow();
                    return true;
                }
                return false;
            }

            public override void Success()
            {
                base.Success();
            }

            public override void Unload()
            {
                if (m_restartPatrol)
                {
                    SetPatrolMode();
                }

                base.Unload();
                if (m_botDetector != null) 
                {
                    m_botDetector.Off();
                    m_botDetector.OnEntityEnter -= m_botDetector_OnEntityEnter;
                }
            }

            private void StopFollow() 
            {
                m_botToTalk.LookTarget = MySession.PlayerShip;
                MyScriptWrapper.StopFollow(m_botToTalk);                
            }
        }

        protected enum EntityID : uint
        {

            BarthsMoon2StartLocation = 2725127,
            BarthsMoon2MadelynLocation = 2725121,
            BarthsMoon2DisableInfluenceSphere = 16787347,

            BuildDetector0 = 1089490,
            BuildPlatform0 = 1663315,
            BuildContainer0 = 1663266,

            BuildDetector1 = 1089492,
            BuildPlatform1 = 1663313,
            BuildContainer1 = 1663267,

            BuildDetector2 = 1089496,
            BuildPlatform2 = 1663319,
            BuildContainer2 = 1663268,

            BuildDetector3 = 1089494,
            BuildPlatform3 = 1663317,
            BuildContainer3 = 1663269,

            BuildDetector4 = 1089476,
            BuildPlatform4 = 1663321,
            BuildContainer4 = 1663270,

            BuildDetector5 = 1089488,
            BuildPlatform5 = 1663323,
            BuildContainer5 = 1663271,

            ThomasBartId = 1785814,
            ThomasBartDetectorId = 1785846,
            MadelynDummyId = 16785949,

            FlyToEnemyBaseSubmissionLocation = 1934316,
            ShipGenerator = 1800811,
            BaseGenerator = 1896481,
            MainSmallShip = 1946366,
            EngineSpawnpoint = 1800916,
            EngineSpawnpoint2 = 1949674,
            CaveSpawnpoint1 = 1800915,
            CaveSpawnpoint2 = 1800914,
            CaveSpawnpoint3 = 1800913,
            CaveSpawnpoint4 = 1800919,
            TakeTransmitterDummy = 1801017,
            ShipFlameEffect = 1918251,
            GeneratorFlameEffect = 1918252,
            FlyBackDetector = 1800804,
            FlyBackSpawnpoint0 = 1918248,
            FlyBackSpawnpoint1 = 1954776,
            FlyBackSpawnpoint2 = 1954777,
            AmbushDetectorDummy = 1949972,
            AmbushSpawnpoint0 = 1800917,
            AmbushSpawnpoint1 = 1949676,
            AmbushSpawnpoint2 = 1949675,

            InvisibleContainer1 = 2058250,
            InvisibleContainer2 = 2103332,
            InvisibleContainer3 = 2203552,


            BrokenMoonDummy = 2016697,
            FanDummy = 2671110,
            Destroyprefab = 2058259,
            ToDestroyPrefab1 = 2058257,
            ToDestroyPrefab2 = 2058256,
            ToDestroyParticle1 = 2103319,
            ToDestroyParticle2 = 2068275,
            ToDestroyParticle3 = 2020682,
            VoxelDummy1 = 2058244,
            VoxelDummy2 = 2058245,
            VoxelDummy3 = 2058246,
            VoxelDummy4 = 2058247,
            VoxelDummy5 = 2058249,
            VoxelDummy6 = 2058248,
            VoxelMap = 2582973,

            SecurityHubDummy = 1993580,

            EnableGenerator = 2203566,
            SecurityHubLook1 = 2203568,
            SecurityHubLook2 = 2584525,

            Cargobox1 = 2016743,
            Cargobox2 = 1990020,

            DroneGeneratroHub1 = 2322458,
            Dronegenerator1 = 1976987,
            Dronegenerator1particles = 2203576,
            DroneGeneratroHub2 = 2273178,
            DroneGenerator2 = 1976989,

            Hub1Dummy = 2489570,
            Hub2Dummy = 2489571,
            Hub3Dummy = 2489572,
            Hub4Dummy = 2489573,

            /*
            SecurityScanner1 = 1972344,
            SecurityScanner2 = 1981496,
            */

            InfluenceSphere1 = 2583203,
            InfluenceSphere2 = 2583205,
            InfluenceSphere3 = 2583207,
            ActivateDummyInfl = 2503268,
            ActivateGenerator = 2203795,
            TurretId = 1976967,
            TurretDestroyprefab = 2103322,
            ActiveDummy1 = 2503264,
            ActiveDummy2 = 2503263,
            ActiveDummy3 = 2503262,
            ActiveDummy4 = 2503265,
            //TurretKillShow = 2103332,

            SpawnPoint1 = 1981606,
            SpawnPoint2 = 1981608,

            EnterMainLabDummy = 1981501,
            EnterMainLabDetector = 2203585,
            SpawnPoint3 = 1981601,
            SpawnPoint4 = 1981607,

            DestroyComputer = 2671717,
            DestroyCompParticleDummy = 2671716,
            //DestroyComputerDummyActivateTurrets = 1981501,
            //DestroyComputerDummyActivateSpawn = 1981501 ;
            SpawnPoint5 = 1981605,
            SpawnPoint6 = 1981603,
            SpawnPoint7 = 1981604,
            SpawnPoint8 = 1981602,


            CollectPart1 = 2671316,
            CollectPart2 = 2671315,
            CollectPart3 = 2671312,


            CollectPart1Dummy = 2210396,
            CollectPart2Dummy = 2210395,
            CollectPart3Dummy = 2210397,


            EscapeDummy = 2020668,
            EscapeEventDummy = 2203587,
            EscapeEventDummyToActive1 = 2203547,
            EscapeEventDummyToActive2 = 2203543,
            InvisibleDummy4 = 2579910,

            SpawnPoint9 = 2203778,
            SpawnPoint10 = 2203779,
            SpawnPoint11 = 2203781,
            SpawnPoint12 = 2203780,
            DetectorWayBack2 = 2203784,
            SpawnPoint13 = 2203783,
            SpawnPoint14 = 2203782,

            WayForbarthLastWayPoint = 2203799,
            SpawnPointEscape = 2579909,
            HidePrefabCont = 2580779,

            ParticleDummy1 = 2580916,
            ParticleDummy2 = 2580890,
            AlarmContId = 1958806,

            PlayerStartLocationPlant = 2701202,
            PlayerStartLocationConvince = 303745,
            PlayerStartLocationTransmitter = 71,
            MadelynMotShip = 2672892,
            BarthsTurrets1 = 2700945,
            BarthsDestroyedBase = 2699223,
            BarthNormalBase = 2700604,
            WayPoint = 2701207,

            //turretDestroy1 =2103324,
            turretDestroy2 = 2103320,
            turretDestroy3 = 2103325,
            Doors = 2016400,
            GeneratorDestroyed = 2580811,


            _01SmallShipBarth = 2701051,
            _01Detector = 2703136,
            _01PirateShip = 2697503,
            _01PirateShiptarget = 2697564,
            _01SpawnPoint = 2696598,
            _01SpawnPoint2 = 2720983,

            _02MadelyneDefenceShip = 2695020,
            //_02MadelynsHangar = 2708104 ,

            _03Dummy = 2703138,
            _03PortableTurrets = 2691923,


            TURRET_CONTAINER_ID_1 = 2692004,
            TURRET_CONTAINER_ID_2 = 2701090,
            TURRET_CONTAINER_ID_3 = 2691968,
            TURRET_CONTAINER_ID_4 = 2691950,
            TURRET_CONTAINER_ID_5 = 2701184,
            TURRET_CONTAINER_ID_6 = 2701108,
            TURRET_CONTAINER_ID_7 = 2701144,


            DETECTOR_ID_1 = 2703140,
            DETECTOR_ID_2 = 2703142,
            DETECTOR_ID_3 = 2703144,
            DETECTOR_ID_4 = 2703146,
            DETECTOR_ID_5 = 2703148,
            DETECTOR_ID_6 = 2703150,
            DETECTOR_ID_7 = 2704352,

            DETECTOR_ID_8 = 2708082,

            PARTICLE_ID_1 = 2704348,
            PARTICLE_ID_2 = 2704346,
            PARTICLE_ID_3 = 2704344,
            PARTICLE_ID_4 = 2704342,
            PARTICLE_ID_5 = 2704340,
            PARTICLE_ID_6 = 2704350,
            PARTICLE_ID_7 = 2703152,

            PARTICLE_ID_8 = 2708083,

            _05PirateShip = 2697625,
            _05PirateBigShip = 2672627,
            _05PirateShipTarget = 2696581,
            _05PirateBigShipTarget = 2701428,


            _06ParticleDummy = 2701441,
            _06PrefabContainer1 = 2701450,
            _06AttackmadelynDummy = 2705609,


            _07PirateShipTarget = 2697725,
            _07PirateBigShipTarget = 2695495,


            _08generator1 = 2672774,
            _08generator1particle = 2701440,
            _08generator1VisibleCOnt = 2702969,

            _08generator2 = 2697665,
            _08generator2VisibleCont = 2702883,


            _10generatorDummy = 2706812,
            _10generatorparticleDummyEnable = 2706833,

            _11placeBombDummy = 2706834,

            _12MadelynDummy = 2708046,

            MadelynLocation = 2720990,

            BarthAttackDetector = 2720981,

            //MadelynTurret1 = 2722358,
            //MadelynTurret2 = 2722348,
            //MadelynTurret3 = 2722353,


            MadelynTurret1Detector = 2722376,
            MadelynTurret2Detector = 2722366,
            MadelynTurret3Detector = 2722370,


            MadelynTurret1Particle = 2722377,
            MadelynTurret2Particle = 2722367,
            MadelynTurret3Particle = 2722371,


            Explosion = 2701439,



            Dialog5DetectorId = 16788772,
            Dialog6DetectorId = 16788776,
            Dialog10DetectorId = 16788778,
            Dialog13DetectorId = 16788818,
            Dialog17DetectorId = 16788820,

            Dialog19DetectorId = 16788822,
            //Dialog27DetectorId = 2203587,
            Dialog31DetectorId = 16788826,

            ConvinceDialog2DetectorId = 2989,
            GetNeededComponentsDummy = 1989,
            GetNeededComponentsContainer = 1979,
            ConstructDetector = 2027,
            UnhidePrefab = 259,
            Light1 = 1662751,
            Light2 = 1662734,
            Light3 = 1662745,
            PirateReward = 10,
            RaidersMothership = 1793184,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (EntityID value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)value);
            }
        }

        protected MyEntity GetEntity(EntityID entityId)
        {
            return MyScriptWrapper.GetEntity((uint)entityId);
        }

        protected MyBarthsMoonMissionBase(MyMissionID id, StringBuilder debugName, MyTextsWrapperEnum name, MyTextsWrapperEnum decription, MyMissionID[] requiredMissions, EntityID[] buildContainers, EntityID locationEntity)
        {            
            ID = id;
            DebugName = debugName;
            Name = name;
            Description = decription;
            RequiredMissions = requiredMissions;            
            Location = new MyMissionLocation(baseSector, (uint)locationEntity);
            m_buildedContainers = buildContainers;
            m_objectives = new List<MyObjective>();
            Flags = MyMissionFlags.Story;
        }

        protected static MyMwcVector3Int baseSector = new MyMwcVector3Int(3360719, 0, -27890037);
        
        private EntityID[] m_buildedContainers;        

        public override void Load()
        {            
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector0, EntityID.BuildPlatform0, EntityID.BuildContainer0);
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector1, EntityID.BuildPlatform1, EntityID.BuildContainer1);
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector2, EntityID.BuildPlatform2, EntityID.BuildContainer2);
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector3, EntityID.BuildPlatform3, EntityID.BuildContainer3);
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector4, EntityID.BuildPlatform4, EntityID.BuildContainer4);
            HideDetectorAndPlatformAndContainer(EntityID.BuildDetector5, EntityID.BuildPlatform5, EntityID.BuildContainer5);

            base.Load();
        }        

        protected void HideDetectorAndPlatformAndContainer(EntityID detectorId, EntityID platformId, EntityID containerId) 
        {
            HideDummy(detectorId);
            HideDummy(platformId);
            if (m_buildedContainers == null || !m_buildedContainers.Contains(containerId))
            {                
                HideContainer(containerId);
            }
        }        

        protected static void HideDummy(EntityID platformId) 
        {
            SetDummyVisibleStatus(platformId, false);
        }

        protected static void HideDummy(MyDummyPoint platform) 
        {
            SetDummyVisibleStatus(platform, false);
        }

        protected static void ShowDummy(EntityID platformId) 
        {
            SetDummyVisibleStatus(platformId, true);
        }

        protected static void ShowDummy(MyDummyPoint platform) 
        {
            SetDummyVisibleStatus(platform, true);
        }

        protected static void SetDummyVisibleStatus(EntityID platformId, bool visible) 
        {
            SetDummyVisibleStatus((MyDummyPoint)MyScriptWrapper.GetEntity((uint)platformId), visible);            
        }

        private static void SetDummyVisibleStatus(MyDummyPoint platform, bool visible) 
        {
            if (visible)
            {
                MyScriptWrapper.UnhideEntity(platform);
            }
            else
            {
                MyScriptWrapper.HideEntity(platform);
            }
        }

        protected static void ShowContainer(EntityID containerId) 
        {
            SetContainerVisibleStatus(containerId, true);
        }

        protected static void ShowContainer(MyPrefabContainer container) 
        {
            SetContainerVisibleStatus(container, true);
        }

        protected static void HideContainer(EntityID containerId) 
        {
            SetContainerVisibleStatus(containerId, false);
        }

        protected static void HideContainer(MyPrefabContainer container) 
        {
            SetContainerVisibleStatus(container, false);
        }

        protected static void SetContainerVisibleStatus(EntityID containerId, bool visible) 
        {
            SetContainerVisibleStatus((MyPrefabContainer)MyScriptWrapper.GetEntity((uint)containerId), visible);            
        }

        protected static void HideContainers(List<uint> toHide)
        {
            foreach (var u in toHide)
            {
                SetContainerVisibleStatus((MyPrefabContainer)MyScriptWrapper.GetEntity(u), false);         
            }
        }

        protected static void ShowContainers(List<uint> toHide)
        {
            foreach (var u in toHide)
            {
                SetContainerVisibleStatus((MyPrefabContainer)MyScriptWrapper.GetEntity(u), true);
            }
        }

        protected static void EnableCorrectBarths(uint bartIdToEnable, uint bartIdToDisable) 
        {
            MyScriptWrapper.SetEntityEnabled(bartIdToDisable, false);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(bartIdToDisable));

            EnableEntity(bartIdToEnable);
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(bartIdToEnable));
        }

        protected static void EnableEntities(List<uint> toEnable)
        {
            foreach (var u in toEnable)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(u),true);
            }
        }

        protected static void DisableEntities(List<uint> toEnable)
        {
            foreach (var u in toEnable)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(u), true);
            }
        }

        protected static void EnableEntity(uint id)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(id), true);
        }


        protected static void DisableEntity(uint id)
        {
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(id), false);
        }

        private static void SetContainerVisibleStatus(MyPrefabContainer container, bool visible) 
        {
            if (visible)
            {
                MyScriptWrapper.UnhideEntity(container);
            }
            else 
            {
                MyScriptWrapper.HideEntity(container);
            }
        }

        protected static void ActivateSpawnpoints(List<uint> spawnPoints)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnPoint);
            }
        }


        protected static void DeactivateSpawnpoints(List<uint> spawnPoints)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                MyScriptWrapper.DeactivateSpawnPoint(spawnPoint);
            }
        }
    }
}

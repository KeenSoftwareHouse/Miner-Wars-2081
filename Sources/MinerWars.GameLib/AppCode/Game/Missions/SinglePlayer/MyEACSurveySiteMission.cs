#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.Resources;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyEACSurveySiteMission : MyMission
    {
        #region Enums
        private enum EntityID
        {
            StartLocation = 1275,
            RedHubSubmissionLocation = 1277,
            ToTheBaseSubmissionLocation = 70907,
            CommandCentreSubmissionLocation = 70908,
            GeneratorSubmissionLocation = 72629,
            GaneratorHUB = 215890,            
            HangarSubmissionLocation = 72910,
            Barricade1 = 35074,
            Barricade2 = 35075,
            Barricade3 = 35076,
            Barricade4 = 35077,
            Barricade5 = 34042,
            Barricade6 = 34043,
            Barricade7 = 34038,
            Barricade8 = 34044,
            PreBarricade1 = 101879,
            PreBarricade2 = 101875,
            PreBarricade3 = 101876,
            PreBarricade4 = 101880,
            PreBarricade5 = 101882,
            PreBarricade6 = 101883,
            PreBarricade7 = 101884,
            PreBarricade8 = 101885,
            OptionalSaveMinersAcceptLocation = 72909,
            OptionalSaveMinersLocation = 100685,
            HangerEscapeLocation = 83479,
            Light1 = 99889,
            Light2 = 99887,
            Light3 = 99890,
            Light4 = 99892,
            Light5 = 99894,
            Light6 = 99893,
            MothershipParticleEffect = 77392,
            SpawnPointStart1 = 66031,
            SpawnPointStart2 = 66032,
            SpawnPointStart3 = 78710,
            SpawnPointBlue1 = 66033,
            SpawnPointBlue2 = 66035,
            SpawnPointBlue3 = 79545,
            SpawnPointGreen1 = 73160,
            SpawnPointGreen2 = 73161,
            SpawnPointGreen3 = 73163,
            SpawnPointGreen4 = 80506,
            SpawnPointCrazyRussian = 77633,
            SpawnPointPipe1 = 76156,
            SpawnPointPipe2 = 76157,
            SpawnPointPipe3 = 76158,
            SpawnPointPipe4 = 76159,
            SpawnPointInside = 71772,
            SpawnPointEnemyStream01 = 107942,
            SpawnPointEnemyStream02 = 107943,
            SpawnPointEnemyStream03 = 107955,
            SpawnPointRouteLiving1 = 72411,
            SpawnPointRouteLiving2 = 71824,
            SpawnPointRouteLiving3 = 72412,
            SpawnPointCommandCentre1 = 76664,
            SpawnPointCommandCentre2 = 76665,
            SpawnPointWorkshop1 = 76650,
            SpawnPointWorkshop2 = 76663,
            SpawnPointHangarRoute = 76649,
            SpawnPointStorageRoute = 93239,
            SpawnPointPhase1 = 76647,
            SpawnPointPhase2 = 72911,
            SpawnPointPhase3 = 76648,
            SpawnPointSurvived = 76651,
            DetectorBlue = 1276,
            DetectorGreen = 71793,
            DetectorRed = 73358,
            DetectorBarricade = 35433,
            DetectorPipe = 70906,
            DetectorRouteLiving = 76177,
            DetectorCommandDoor = 76412,
            ExplosionBlue = 80024,
            Explosion1 = 80264,
            Explosion2 = 80265,
            Explosion3 = 80266,
            Explosion4a = 80267,
            Explosion4b = 80263,
            ExplosionBarricade1 = 80747,
            ExplosionBarricade2 = 91472,
            ExplosionCutscene1 = 77394,
            ExplosionCutscene2a = 77393,
            CrashingMothershipFuel = 100146,
            ExplosionCutscene3 = 100138,
            CrashingMothershipComm = 100143,
            CrashingMothershipThruster1 = 100142,
            CrashingMothershipThruster2 = 100140,
            CrashingMothershipBatery = 100147,
            CrashingMothershipSmoke = 206377,
            ExplosionCutscene5a = 103258,
            ExplosionCutscene5b = 103259,
            ExplosionPhase1 = 80748,
            ExplosionPhase2 = 80750,
            ExplosionPhase3 = 80749,
            ExplosionHangarBay1 = 83480,
            ExplosionHangarBay2 = 83481,
            ExplosionOptionalPhase = 102405,
            ExplosionCenterHangarBay1 = 83482,
            ExplosionCenterHangarBay2 = 83483,
            ExplosionCenterHangarBay3 = 83484,
            ExplosionFuelTank1a = 20400,
            ExplosionFuelTank1b = 82749,
            ExplosionFuelTank1c = 83491,
            ExplosionFuelTank2 = 85296,
            VoxelMap = 16791862,
            VoxelSphere1 = 91210,
            VoxelSphere2 = 91209,
            VoxelSphere3 = 91208,
            VoxelSphere4 = 91207,
            DoorNearControl1 = 67281,
            DoorNearControl2 = 67285,
            DoorNearControl3 = 67276,
            DoorNearControl4 = 67219,
            DoorNearControl5 = 67535,
            DoorNearControl6 = 67537,
            DoorNearControl7 = 69688,
            DoorNearControl8 = 67456,
            DoorNearControl9 = 67458,
            DoorProcessingResearch1 = 71820,
            DoorProcessingResearch2 = 71817,
            DoorProcessingResearch3 = 72904,
            DoorProcessingResearch4 = 72903,
            DoorProcessingResearch5 = 67472,
            DoorProcessingResearch6 = 67470,
            DoorProcessingResearch7 = 67465,
            DoorProcessingResearch8 = 67463,
            DoorLocked1 = 67186,
            DoorLocked2 = 76911,
            DoorGenerator1 = 67201,
            DoorGenerator2 = 67192,
            DoorHangar1 = 69677,
            DoorHangar2 = 66784,
            DoorHangar3 = 69668,
            DoorHangar4 = 67130,
            DoorHangar5 = 67132,
            DoorHangar6 = 67134,
            DoorHangar7 = 66772,
            DoorHangar8 = 66773,
            DoorHangar9 = 66776,
            DoorHangar10 = 67325,
            DoorHangar11 = 67329,
            DoorHangar12 = 67142,
            DetectorCommandCentreEntrance = 93240,
            DetectorHangar = 93241,
            Generator1 = 99638,
            Generator2 = 99643,
            Generator3 = 72406,
            Generator4 = 99644,
            LoadingBayDummy = 93719,
            SavedMiner1 = 100699,
            SavedMiner2 = 100686,
            SavedMiner3 = 100712,
            HangarBay1a = 70210,
            HangarBay1b = 70219,
            HangarBay2a = 70215,
            HangarBay2b = 70220,
            HangarBay3 = 70208,
            Mothership = 100135,
            //MadelynMothership = 83485,
            BeamageBefore01 = 8776,
            BeamageBefore02 = 8779,
            BeamageBefore03 = 8780,
            BeamageBefore04 = 8781,
            BeamageBefore05 = 1229,
            BeamageBefore06 = 1129,
            BeamageBefore07 = 1176,
            BeamageBefore08 = 1160,
            BeamageBefore09 = 20402,
            BeamageAfter01 = 106023,
            BeamageAfter02 = 106028,
            BeamageAfter03 = 131471,
            BeamageAfter04 = 106026,
            BeamageAfter05 = 106027,
            BeamageAfter06 = 104113,
            RussianMothership1 = 131472,
            RussianMothership2 = 75843,
            RussianMothership3 = 109335,
            FollowDetector = 117136,
            MarcusRoute01 = 42763,
            MarcusRoute02 = 20537,
            MarcusRoute03 = 20530,
            MarcusRoute04 = 20527,
            //Marcus = 118137,
            MinersSpawnpoint01 = 121176,
            MinersSpawnpoint02 = 121177,
            MinersSpawnpoint03 = 122214,
            MinersSpawnpoint04 = 139970,
            MinersSpawnpoint05 = 144446,
            Left_PlatformDummy = 135785,
            Left_Control = 125839,
            Left_Generator = 125837, 
            Left_LeftTurretLight = 127200,
            Left_RightTurret = 70085,
            Left_RightTurretLight = 127199,
            Left_Light1 = 127204,
            Left_Light2 = 125874,
            Right_PlatformDummy = 135786,
            Right_Control = 125871,
            Right_Generator = 125870,
            Right_LeftTurret = 70082,
            Right_LeftTurretLight = 127202,
            Right_RightTurretLight = 127201,
            Right_Light1 = 125873,
            Right_Light2 = 127203,
            Asteroid_Static_01 = 136845,
            Asteroid_Static_02 = 136847,
            Asteroid_Static_03 = 136846,
            Asteroid_Static_04 = 136848,
            Asteroid_Static_05 = 136850,
            Asteroid_Static_06 = 136851,
            Asteroid_Static_07 = 136853,
            Asteroid_Static_08 = 136852,
            Asteroid_Static_09 = 136849,
            Particles_01 = 82582,
            Particles_03 = 112775,
            Particles_04 = 115123,
            CargoToMove01_Dummy = 172108,
            CargoToMove01_1 = 131446, // box
            CargoToMove01_2 = 131447,
            CargoToMove01_3 = 101117, // box
            CargoToMove01_4 = 70889,
            CargoToMove02_Dummy = 172109,
            CargoToMove02_1 = 131451, // box
            CargoToMove02_2 = 131450,
            CargoToMove02_3 = 131452, // box
            CargoToMove02_4 = 131453,
            CargoToMove03_Dummy = 172110,
            CargoToMove03_1 = 131462, // box
            CargoToMove03_2 = 131463,
            BarricadeVoxels1 = 174192, //80
            BarricadeVoxels2 = 174842, //100
            BarricadeVoxels3 = 174843, //100
            TurretFriend01 = 72418,
            TurretFriend02 = 72419,
            TurretFriend03 = 93477,
            TurretFriend04 = 93479,
            TurretFriend05 = 93481,
            TurretFriend06 = 93483,
            //InfluenceSphere01 = 194376,
            //InfluenceSphere02 = 194378,
            //InfluenceSphere03 = 194380,
            //InfluenceSphere04 = 194382,
            SharpshooterSpawn = 193371,
            Dust01 = 198371,
            Dust02 = 198372,
            Dust03 = 198373,
            Dust04 = 198377,
            Dust05 = 198378,
            Dust06 = 198379,
            Dust07 = 198380,
            Dust08 = 198381,
            OtherParticle01 = 199349,
            OtherParticle02 = 200786,
            OtherParticle03 = 210172,
            OtherParticle04 = 210067,
            OtherParticle05 = 209951,
            OtherParticle06 = 210030,
            OtherParticle07 = 209872,
            OtherParticle08 = 209767,
            OtherParticle09 = 209582,
            OtherParticle10 = 207306,
            OtherParticle11 = 209584,
            OtherParticle12 = 209583,
            OtherParticle13 = 207643,
            OtherParticle14 = 207473,
            OtherParticle15 = 207507,
            OtherParticle16 = 207541,
            GeneratorLight = 75352,

            MothershipCrash01 = 204612,
            MothershipCrash02 = 204611,
            MothershipCrash03 = 204610,
            MothershipCrash04 = 204609,
            //MothershipCrashTurret = 100145,
            MothershipCrashVoxel = 204613,
            MothershipCrashEffect1 = 204614,
            MothershipCrashEffect2 = 205333,
            MothershipCrashEffect3 = 212087,
            MothershipCrashEffect4 = 212088,

            VendorHangar = 195395,

            MSCrashExplosion1 = 16780710,
            MSCrashExplosion2 = 16780709,
            MSCrashExplosion3 = 16780711,
            MSCrashExplosion4 = 16780707,
            MSCrashExplosion5 = 16780708,
            MSCrashExplosion6 = 16782441,


        }

        private List<uint> m_LightsAllOfThem = new List<uint>
        {                                                              
            73557, 73556, 73555, 73554, 70900, 70899, 70901, 70881, 70446, 70883, 70885, 70884, 70882, 70881, 70441, 75382, 73755, 73756, 72901, 68793, 68796, 68795, 68794, 68786, 68779, 68783, 68784, 68785, 72889, 68787, 68789, 72900, 72902, 72888, 173168, 173166, 127205, 127206, 93004, 127209, 127208, 127207, 91975, 91974, 91977, 91976, 69416, 69415, 69413, 69414, 73973, 73974, 73975, 73976
        };

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (var item in m_LightsAllOfThem)
            {
                MyScriptWrapper.GetEntity(item);
            }
        }

        /*private List<uint> m_influenceAlarm = new List<uint>
        {
            (uint)EntityID.InfluenceSphere01, (uint)EntityID.InfluenceSphere02, (uint)EntityID.InfluenceSphere03, (uint)EntityID.InfluenceSphere04
        };
        */

        private List<uint> m_turrets = new List<uint>
        {
            (uint)EntityID.TurretFriend01, (uint)EntityID.TurretFriend02, (uint)EntityID.TurretFriend03, (uint)EntityID.TurretFriend04, (uint)EntityID.TurretFriend05, (uint)EntityID.TurretFriend06
        };
        private List<uint> m_particles = new List<uint>
        {
            (uint)EntityID.Particles_01, (uint)EntityID.ExplosionFuelTank1b, (uint)EntityID.Particles_03, (uint)EntityID.Particles_04, (uint)EntityID.Dust01, (uint)EntityID.Dust02, (uint)EntityID.Dust03, (uint)EntityID.Dust04, (uint)EntityID.Dust05, (uint)EntityID.Dust06, (uint)EntityID.Dust07, (uint)EntityID.Dust08, (uint)EntityID.OtherParticle01, (uint)EntityID.OtherParticle02, (uint)EntityID.OtherParticle03, (uint)EntityID.OtherParticle04, (uint)EntityID.OtherParticle05, (uint)EntityID.OtherParticle06, (uint)EntityID.OtherParticle07, (uint)EntityID.OtherParticle08, (uint)EntityID.OtherParticle09, (uint)EntityID.OtherParticle10, (uint)EntityID.OtherParticle11, (uint)EntityID.OtherParticle12, (uint)EntityID.OtherParticle13, (uint)EntityID.OtherParticle14, (uint)EntityID.OtherParticle15, (uint)EntityID.OtherParticle16
        };
        private List<uint> m_beamsBefore = new List<uint>
        {
            (uint)EntityID.BeamageBefore01, (uint)EntityID.BeamageBefore02, (uint)EntityID.BeamageBefore03, (uint)EntityID.BeamageBefore04, (uint)EntityID.BeamageBefore05, (uint)EntityID.BeamageBefore06, (uint)EntityID.BeamageBefore07, (uint)EntityID.BeamageBefore08, (uint)EntityID.BeamageBefore09
        };
        private List<uint> m_beamsAfter = new List<uint>
        {
            (uint)EntityID.BeamageAfter01, (uint)EntityID.BeamageAfter02, (uint)EntityID.BeamageAfter03, (uint)EntityID.BeamageAfter04, (uint)EntityID.BeamageAfter05, (uint)EntityID.BeamageAfter06
        };
        private List<uint> m_mothershipCrashStuff = new List<uint>
        {
            (uint)EntityID.MothershipCrash01, (uint)EntityID.MothershipCrash02, (uint)EntityID.MothershipCrash03, (uint)EntityID.MothershipCrash04
        };

        private List<uint> m_barricade = new List<uint>
        {
            (uint)EntityID.Barricade1, (uint)EntityID.Barricade2, (uint)EntityID.Barricade3, (uint)EntityID.Barricade4, (uint)EntityID.Barricade5, (uint)EntityID.Barricade6, (uint)EntityID.Barricade7, (uint)EntityID.Barricade8
        };
        private List<uint> m_preBarricade = new List<uint>
        { 
            (uint)EntityID.PreBarricade1, (uint)EntityID.PreBarricade2, (uint)EntityID.PreBarricade3, (uint)EntityID.PreBarricade4, (uint)EntityID.PreBarricade5, (uint)EntityID.PreBarricade6, (uint)EntityID.PreBarricade7, (uint)EntityID.PreBarricade8
        };

        private List<uint> m_Cargo1 = new List<uint>
        {
            (uint)EntityID.CargoToMove01_Dummy, (uint)EntityID.CargoToMove01_1, (uint)EntityID.CargoToMove01_2, (uint)EntityID.CargoToMove01_3, (uint)EntityID.CargoToMove01_4
        };
        private List<uint> m_Cargo2 = new List<uint>
        {
            (uint)EntityID.CargoToMove02_Dummy, (uint)EntityID.CargoToMove02_1, (uint)EntityID.CargoToMove02_2, (uint)EntityID.CargoToMove02_3, (uint)EntityID.CargoToMove02_4
        };
        private List<uint> m_Cargo3 = new List<uint>
        {
            (uint)EntityID.CargoToMove03_Dummy, (uint)EntityID.CargoToMove03_1, (uint)EntityID.CargoToMove03_2
        };

        #endregion

        #region Members

        private static readonly int MAX_ANNOYING_BOTS = 2;

        private Vector3 m_mothershipFinalPosition;
        private Vector3 m_mothershipPosition01 = new Vector3(530, -2316, -10171); // explode
        private Vector3 m_mothershipPosition02 = new Vector3(476, -2155, -10294); // beam
        private Vector3 m_mothershipPosition03 = new Vector3(414, -1966, -10439); // final, crash and explode

        private MySmallShipBot m_sharpshooter;
        //private MyObjective m_followSubmission;
        private MyObjective m_hangarLastStand;
        private MyObjective m_toTheBaseSubmission;
        private MyObjective m_barricadeSubmission;
        private MyObjective m_generatorSubmission;
        private MyObjective m_optionalSaveMinersAccept;
        private MyObjective m_optionalSaveMiners;

        private MySmallShipBot m_MarcusBot;
        //private bool m_followMarcusShown;
        private bool m_flyMS;

        /*
        private bool m_moveCargo1 = false;
        private bool m_moveCargo2 = false;
        private bool m_moveCargo3 = false;
          */

        private Vector3 m_Cargo1FinalPosition = new Vector3(16, -1437, -10562);
        private Vector3 m_Cargo2FinalPosition = new Vector3(30, -2265, -10598);
        private Vector3 m_Cargo3FinalPosition = new Vector3(-1258, -4441, -11050);

        private MyEntity m_MSmove;
        private bool m_minesShake = true;
        //private bool m_marcusInShaft = false;
        //private bool m_playerInShaft = false;
        //private bool m_beginningSkipped = false;
        //private bool m_skipIntroUsed = false;

        //private MyEntityDetector m_followDetector;

        private MyTimerActionDelegate m_subShakeAction;
        private MyTimerActionDelegate m_farExplosionAction;
        //private MyMissionLocation m_Shaft7;

        private MinerWars.AppCode.Game.HUD.MyHudNotification.MyNotification m_skipIntroductionNotification;
        //private bool m_skippingIntroduction;
        //private float m_skippingIntroductionTime;

        MySpawnpointSmartWaves m_spawnPointSmartWaves;

        #endregion

        public MyEACSurveySiteMission()
        {
            m_subShakeAction = new MyTimerActionDelegate(SubShake);
            m_farExplosionAction = new MyTimerActionDelegate(FarExplosion);

            ID = MyMissionID.EAC_SURVEY_SITE; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("01-EAC survey site");
            Name = MyTextsWrapperEnum.EAC_SURVEY_SITE;
            Description = MyTextsWrapperEnum.EAC_SURVEY_SITE_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission01_EacSS;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-913818, 0, -790076);

            /* sector where the mission is located */
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);
            //m_Shaft7 = new MyMissionLocation(baseSector, (uint)EntityID.FollowDetector);
            RequiredMissions = new MyMissionID[] { };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_90 };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            this.OnMissionSuccess += new MissionHandler(MyEACSurveySiteMission_OnMissionSuccess);
            
            m_objectives = new List<MyObjective>();

     
            /*
            m_followSubmission = new MyObjective(
                new StringBuilder("Follow Marcus to the mines"),
                MyMissionID.EAC_SURVEY_SITE_FOLLOWMARCUS_1,
                new StringBuilder("Another boring day...\n"),
                null,
                this,
                new MyMissionID[] { },
                null,
                successDialogId: MyDialogueEnum.EAC_SURVEY_SITE_0200_ACTIONSTARTS
            ) { SaveOnSuccess = true };
            m_followSubmission.OnMissionLoaded += FollowSubmission_OnMissionLoaded;
            m_followSubmission.OnMissionSuccess += FollowSubmissionSuccess;
            m_followSubmission.OnMissionUpdate += FollowSubmission_OnMissionUpdate;

            m_objectives.Add(m_followSubmission);
              */
            var redHubSubmission = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_10_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_10,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_10_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.RedHubSubmissionLocation)
            ) { HudName = MyTextsWrapperEnum.Nothing };
            redHubSubmission.OnMissionLoaded += RedHubLoaded;
            m_objectives.Add(redHubSubmission);

                        
            m_spawnPointSmartWaves = new MySpawnpointSmartWaves(
                new uint[] 
                {
                    (uint)EntityID.SpawnPointPipe1,
                    (uint)EntityID.SpawnPointPipe2,
                    (uint)EntityID.SpawnPointPipe3,
                    (uint)EntityID.SpawnPointPipe4,
                    (uint)EntityID.SpawnPointEnemyStream01,
                    (uint)EntityID.SpawnPointEnemyStream02,
                    (uint)EntityID.SpawnPointEnemyStream03,
                },
               new uint[] { (uint)EntityID.SpawnPointCrazyRussian }
               , MAX_ANNOYING_BOTS);

            Components.Add(m_spawnPointSmartWaves);

            m_barricadeSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_CLEAR_THE_WAY_Name),
                MyMissionID.EAC_SURVEY_SITE_CLEAR_THE_WAY,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_CLEAR_THE_WAY_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_10 },
                m_barricade
            ) { HudName = MyTextsWrapperEnum.Nothing };
            m_barricadeSubmission.OnMissionLoaded += BarricadeLoaded;
            m_barricadeSubmission.OnMissionSuccess += BarricadeSuccess;
            m_objectives.Add(m_barricadeSubmission);

            m_toTheBaseSubmission = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_30_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_30,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_30_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_CLEAR_THE_WAY },
                new MyMissionLocation(baseSector, (uint)EntityID.ToTheBaseSubmissionLocation)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMainBase };
            m_toTheBaseSubmission.OnMissionLoaded += ToTheBaseLoaded;
            m_objectives.Add(m_toTheBaseSubmission);


            var commandCentreSubmission = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_40_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_40,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_40_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_30 },
                new MyMissionLocation(baseSector, (uint)EntityID.CommandCentreSubmissionLocation),
                successDialogId: MyDialogueEnum.EAC_SURVEY_SITE_0550_COMMANDOFFLINE
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCommandCenter };
            commandCentreSubmission.OnMissionLoaded += CommandCenterLoaded;// CommandCentreSubmissionSuccess;
            m_objectives.Add(commandCentreSubmission);

            m_generatorSubmission = new MyObjectiveEnablePrefabs(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GENERATOR_Name),
               MyMissionID.EAC_SURVEY_SITE_GENERATOR,
               (MyTextsWrapperEnum.EAC_SURVEY_SITE_GENERATOR_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_40 },
               new MyMissionLocation(baseSector, (uint)EntityID.GaneratorHUB),
               new List<uint>() { (uint)EntityID.Generator1, (uint)EntityID.Generator2, (uint)EntityID.Generator3, (uint)EntityID.Generator4 },
               new List<uint>() { (uint)EntityID.Generator1, (uint)EntityID.Generator2, (uint)EntityID.Generator3, (uint)EntityID.Generator4 }
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHub };
           // m_generatorSubmission = new MyUseObjective(
           //    new StringBuilder("Start the generator"),
           //    MyMissionID.EAC_SURVEY_SITE_GENERATOR,
           //    new StringBuilder("An auxiliary generator is near the workshop.\n"),
           //    null,
           //    this,
           //    new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_40 },
           //    new MyMissionLocation(baseSector, (uint)EntityID.GeneratorSubmissionLocation),
           //    MyTextsWrapperEnum.PressToStartGenerator,
           //    MyTextsWrapperEnum.Generator,
           //    MyTextsWrapperEnum.StartingProgress,
           //    5000,
           //    MyUseObjectiveType.Activating
           //) { SaveOnSuccess = true };
            m_generatorSubmission.OnMissionLoaded += GeneratorLoaded;// GeneratorSubmissionSuccess;
            m_generatorSubmission.OnMissionSuccess += GeneratorSuccess;
            m_objectives.Add(m_generatorSubmission);


            var commandCentreAgainSubmission = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_60_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_60,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_60_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GENERATOR },
                new MyMissionLocation(baseSector, (uint)EntityID.CommandCentreSubmissionLocation),
                successDialogId: MyDialogueEnum.EAC_SURVEY_SITE_0700_MADELYN
            ) { HudName = MyTextsWrapperEnum.HudCommandCenter };
            commandCentreAgainSubmission.OnMissionLoaded += CommandCenterAgainLoaded;// CommandCentreAgainSubmissionSuccess;
            m_objectives.Add(commandCentreAgainSubmission);

            m_optionalSaveMinersAccept = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_65_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_65,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_65_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_60 },
                new MyMissionLocation(baseSector, (uint)EntityID.OptionalSaveMinersAcceptLocation)
            ) { HudName = MyTextsWrapperEnum.Nothing };
            m_optionalSaveMinersAccept.OnMissionSuccess += OptionalSaveMinersAcceptSubmissionSuccess;
            m_optionalSaveMinersAccept.OnMissionLoaded += OptionalSaveMinersAcceptSubmissionLoaded;
            m_objectives.Add(m_optionalSaveMinersAccept);

            m_optionalSaveMiners = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_SAVEMINERS_Name),
                MyMissionID.EAC_SURVEY_SITE_SAVEMINERS,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_SAVEMINERS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_65 },
                new MyMissionLocation(baseSector, (uint)EntityID.OptionalSaveMinersLocation)
            ) { HudName = MyTextsWrapperEnum.HudMiners };
            m_optionalSaveMiners.OnMissionSuccess += OptionalSaveMinersSubmissionSuccess;
            m_objectives.Add(m_optionalSaveMiners);


            var hangarSubmission = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_70_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_70,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_70_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_SAVEMINERS },
                new MyMissionLocation(baseSector, (uint)EntityID.HangarSubmissionLocation)
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudHangar };
            hangarSubmission.OnMissionLoaded += HangarLoaded; // HangarSubmissionSuccess;
            m_objectives.Add(hangarSubmission);





            m_hangarLastStand = new MyTimedObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_SURVIVE_Name),
                MyMissionID.EAC_SURVEY_SITE_SURVIVE,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_SURVIVE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_70 },
                new TimeSpan(0, 3, 0)
            ) { SaveOnSuccess = true };
            m_hangarLastStand.OnMissionLoaded += HangarLastStandLoaded;// Survived;
            m_objectives.Add(m_hangarLastStand);

                           /*
            var TurretsRightSubmission = new MyObjectiveEnablePrefabs(
               new StringBuilder("Activate the turrets"),
               MyMissionID.EAC_SURVEY_SITE_TURRETS_RIGHT,
               new StringBuilder(""),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_70 },
               new MyMissionLocation(baseSector, (uint)EntityID.Right_Generator),
               null,
               new List<uint>() { (uint)EntityID.Right_Generator }
           ) { ShowAsOptional = true };
                            
            TurretsRightSubmission.OnMissionSuccess += TurretsRightSuccess;// TurretsRightSubmissionSuccess;
            m_objectives.Add(TurretsRightSubmission);

            var TurretsLeftSubmission = new MyObjectiveEnablePrefabs(
               new StringBuilder("Activate the turrets"),
               MyMissionID.EAC_SURVEY_SITE_TURRETS_LEFT,
               new StringBuilder(""),
               null,
               this,
               new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_GOTO_70 },
               new MyMissionLocation(baseSector, (uint)EntityID.Left_Generator),
               null,
               new List<uint>() { (uint)EntityID.Left_Generator }
           ) { ShowAsOptional = true };
            TurretsLeftSubmission.OnMissionSuccess += TurretLeftSuccess;// TurretsLeftSubmissionSuccess;
            m_objectives.Add(TurretsLeftSubmission);
                            */


            var hangarEscape = new MyObjective(
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_90_Name),
                MyMissionID.EAC_SURVEY_SITE_GOTO_90,
                (MyTextsWrapperEnum.EAC_SURVEY_SITE_GOTO_90_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.EAC_SURVEY_SITE_SURVIVE },
                //new MyMissionLocation(baseSector, (uint)EntityID.HangerEscapeLocation)
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            hangarEscape.OnMissionLoaded += HangarEscapeLoaded;
            m_objectives.Add(hangarEscape);

            m_subShakeAction = SubShake;
            m_farExplosionAction = FarExplosion;
        }

        void MyEACSurveySiteMission_OnMissionSuccess(MyMissionBase sender)
        {
            if (MyClientServer.LoggedPlayer.IsDemoUser())
            {
                MyGuiScreenGamePlay.Static.DrawDemoEnd = true;
            }
        }

        public override void Accept()
        {
            base.Accept();
            
            // inventory shuffle
            // MyScriptWrapper.GetPlayerInventory().ClearInventoryItems();
            //MyScriptWrapper.GetPlayerInventory().AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, 1000f, true);
            //MyScriptWrapper.GetPlayerInventory().AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, 48, true);
            
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_Control), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_Generator), false);
            //MyScriptWrapper.SetPrefabMode(MyScriptWrapper.GetEntity((uint)EntityID.Right_RightTurret), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_RightTurretLight), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_LeftTurret), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_LeftTurretLight), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Right_Light2), false);
        
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_Control), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_Generator), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_RightTurret), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_RightTurretLight), false);
            //MyScriptWrapper.SetPrefabMode(MyScriptWrapper.GetEntity((uint)EntityID.Left_LeftTurret), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_LeftTurretLight), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Left_Light2), false);

            HideEnemymotherships();

            if (!MyMinerGame.IsGameReady)
            {
                MyGuiScreenGamePlay.Static.OnGameSafelyLoaded += new MyGuiScreenBase.ScreenHandler(Static_OnGameLoaded);
            }
            else
            {
                Static_OnGameLoaded(null);
            }
        }

        void Static_OnGameLoaded(MyGuiScreenBase source)
        {
            // Run intro
            if (MyFakes.ENABLE_INTRO)
            {
                MyMwcLog.WriteLine("Intro started");
                
                MyGuiScreenGamePlay.Static.HideScreen();

                MyGuiManager.AddScreen(MyGuiScreenIntroVideo.CreateIntroScreen(IntroFinished));

                if (!MyMinerGame.IsPaused())
                {
                    MyMinerGame.SwitchPause();
                }
            }
        }

        private void IntroFinished()
        {
            MyMwcLog.WriteLine("Intro finished");

            MyGuiScreenGamePlay.Static.UnhideScreen();

            if (MyMinerGame.IsPaused())
            {
                MyMinerGame.SwitchPause();
            }
        }

        private void HideEnemymotherships()
        {
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyPrefabContainer motherShip = entity as MyPrefabContainer;
                if (motherShip != null && MyFactions.GetFactionsRelation(motherShip.Faction, MySession.PlayerShip.Faction) == MyFactionRelationEnum.Enemy)
                {
                    MyScriptWrapper.HideEntity(entity, false);
                }
            }
        }

        public override void Load()
        {
            if (!IsMainSector) return;

            //m_followMarcusShown = false;
            m_flyMS = false;

            /*
            m_moveCargo1 = false;
            m_moveCargo2 = false;
            m_moveCargo3 = false;
              */
            m_minesShake = !MyScriptWrapper.IsMissionFinished(MyMissionID.EAC_SURVEY_SITE_GOTO_30);
            //m_marcusInShaft = false;
            //m_playerInShaft = false;
            //m_beginningSkipped = false;
            //m_skipIntroUsed = false;

            m_skipIntroductionNotification = MyScriptWrapper.CreateNotification(MinerWars.AppCode.Game.Localization.MyTextsWrapperEnum.NotificationSkipIntroduction, MyHudConstants.MISSION_FONT);
            m_skipIntroductionNotification.SetTextFormatArguments(new[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.USE) });

            // spawn Marcus
            //marcus = MyScriptWrapper.CreateFriend("Marcus", 100000);
            m_MarcusBot = (MySmallShipBot)MyScriptWrapper.GetEntity("Marcus");
            m_MarcusBot.LeaderLostEnabled = true;

            // When not in base, there was no crash, so hide debris
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.EAC_SURVEY_SITE_GOTO_30))
            {
                foreach (var prefab in m_mothershipCrashStuff)
                {
                    MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity(prefab));
                }
            }

            // crashing mothership
            m_MSmove = MyScriptWrapper.TryGetEntity((uint)EntityID.Mothership);
            MyScriptWrapper.HideEntity(m_MSmove.Parent);
            m_mothershipFinalPosition = m_mothershipPosition01;

            //MyScriptWrapper.OnBotReachedWaypoint += OnBotReachedWaypoint;
            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;

            m_flyMS = false;

       
            // hide beams
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.EAC_SURVEY_SITE_GOTO_10))
            {
                foreach (var prefab in m_beamsAfter)
                {
                    MyScriptWrapper.TryHide(prefab);
                }
                // player priority lower
                //MyScriptWrapper.SetEntityPriority(MySession.PlayerShip, 5);
                //MyScriptWrapper.SetEntityPriority(m_MarcusBot, -50);
            }
                      
            if (!MyScriptWrapper.IsMissionFinished(MyMissionID.EAC_SURVEY_SITE_SURVIVE))
            {
                // hide final mothership
                MyScriptWrapper.TryHide((uint)MyScriptWrapper.GetEntity("Madelyn").EntityId.Value.NumericValue);
            }

            //Hide outside mothership and spawnpoint because of confusion
            //Ticket 8925
            MyScriptWrapper.TryHide((uint)EntityID.RussianMothership3); //Mothership
            MyScriptWrapper.TryHide((uint)EntityID.SpawnPointEnemyStream01); //SP1
            MyScriptWrapper.TryHide((uint)EntityID.SpawnPointEnemyStream02); //SP2

            
            base.Load(); // base.Load() loads submissions, so it must be after mission load
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;

            //m_followDetector = null;
            m_MarcusBot = null;
            m_sharpshooter = null;
            m_MSmove = null;
            MyAudio.MusicAllowed = true;

        }
                        
       
        public override void Update()
        {
            if (!IsMainSector) return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEacSurveySite::Update");

            //MyAudio.ApplyTransition(MyMusicTransitionEnum.LightFight, 0, "KA26");

            base.Update();

            if (m_flyMS)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FlyMS");
                if (MoveMotherShipForward(m_MSmove, 110f, m_mothershipFinalPosition))
                {
                    if (m_mothershipFinalPosition == m_mothershipPosition01)
                    {
                        m_mothershipFinalPosition = m_mothershipPosition02;

                        MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionCutscene1), true);
                        MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionCutscene1), MySoundCuesEnum.SfxShipLargeExplosion);
                        MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.CrashingMothershipFuel });
                        // destroy fuel
                        //((MyPrefabLargeWeapon)MyScriptWrapper.TryGetEntity((uint)EntityID.MothershipTurret)).GetGun().Target = MyScriptWrapper.TryGetEntity((uint)EntityID.MothershipTurretTarget);
                        MissionTimer.RegisterTimerAction(500, CutsceneExplosion01, false);
                    }
                    else
                    {
                        if (m_mothershipFinalPosition == m_mothershipPosition02)
                        {
                            m_mothershipFinalPosition = m_mothershipPosition03;

                            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.MothershipParticleEffect), true);
                            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion1), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50, 0);
                            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion2), MyExplosionTypeEnum.MISSILE_EXPLOSION, 60, 0);
                            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion3), MyExplosionTypeEnum.MISSILE_EXPLOSION, 80, 0);
                            MissionTimer.RegisterTimerAction(1000, MSCrashExplosionNew, false);
                            MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.MothershipParticleEffect), MySoundCuesEnum.SfxShipLargeExplosion);
                            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.CrashingMothershipComm, (uint)EntityID.CrashingMothershipThruster1, (uint)EntityID.CrashingMothershipThruster2/*, (uint)EntityID.MothershipCrashTurret*/ });
                            // destroy beam
                        }
                        else
                        {
                            if (m_mothershipFinalPosition == m_mothershipPosition03)
                            {
                                MyScriptWrapper.IncreaseHeadShake(8);
                                MissionTimer.RegisterTimerAction(350, m_subShakeAction, false);
                                MissionTimer.RegisterTimerAction(550, m_subShakeAction, false);
                                MissionTimer.RegisterTimerAction(800, m_subShakeAction, false);
                                MissionTimer.RegisterTimerAction(1050, m_subShakeAction, false);
                                MissionTimer.RegisterTimerAction(1400, m_subShakeAction, false);
                                MissionTimer.RegisterTimerAction(1900, m_subShakeAction, false);
                                
                                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.MothershipParticleEffect), true);
                                MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity((uint)EntityID.MothershipParticleEffect), MySoundCuesEnum.SfxShipLargeExplosion);

                                m_flyMS = false;
                            }
                        }
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

           
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void MSCrashExplosionNew()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion4), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50, 0);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion5), MyExplosionTypeEnum.MISSILE_EXPLOSION, 60, 0);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.MSCrashExplosion6), MyExplosionTypeEnum.MISSILE_EXPLOSION, 80, 0);
        }

        void CutsceneExplosion01()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CutsceneExplosion01");
            MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.CrashingMothershipBatery  });
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        bool MoveMotherShipForward(MyEntity entity, float speed, Vector3 destination)
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            MyEntity container = entity.Parent;                    // Get prefab container
            if (Vector3.DistanceSquared(destination, container.GetPosition()) > 10 * 10)
            {
                MyScriptWrapper.Move(container, container.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                MyEntity smoke = MyScriptWrapper.GetEntity((uint)EntityID.CrashingMothershipSmoke);
                MyScriptWrapper.Move(smoke, smoke.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                return false;
            }
            return true;
        }
                     

        private void SubShake()
        {
            MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomInt(3, 4));
        }

        private void FarExplosion()
        {
            if (m_minesShake)
            {
                // MainShake
                MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomInt(7, 10));
                MyScriptWrapper.AddAudioImpShipQuake();

                // Register sub shakes
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(200, 400), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(600, 800), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(800, 1200), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(1200, 1400), m_subShakeAction, false);

                // Register next far explosion
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(4000, 12000), m_farExplosionAction, false);
            }
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            switch ((EntityID)MyScriptWrapper.GetEntityId(spawnpoint))
            {
                case EntityID.SpawnPointStart3:
                    MyScriptWrapper.DamageEntity(bot, 0.7f);
                    MyScriptWrapper.SetMaxHealth(bot, 0.5f);
                    MyScriptWrapper.SetEntityPriority(bot, 50);
                    break;
                case EntityID.SpawnPointGreen4:
                    MyScriptWrapper.DamageEntity(bot, 0.4f);
                    MyScriptWrapper.SetMaxHealth(bot, 0.5f);
                    MyScriptWrapper.SetEntityPriority(bot, 20);
                    break;
                case EntityID.SpawnPointBlue3:
                    MyScriptWrapper.DamageEntity(bot, 0.6f);
                    MyScriptWrapper.SetMaxHealth(bot, 0.5f);
                    MyScriptWrapper.SetEntityPriority(bot, -5);
                    break;
                case EntityID.MinersSpawnpoint01:
                    MyScriptWrapper.SetMaxHealth(bot, 0.5f);
                    MyScriptWrapper.SetEntityPriority(bot, -5);
                    break;
                case EntityID.MinersSpawnpoint02:
                    MyScriptWrapper.SetMaxHealth(bot, 0.3f);
                    break;
                case EntityID.MinersSpawnpoint03:
                    MyScriptWrapper.SetMaxHealth(bot, 0.3f);
                    break;
                case EntityID.MinersSpawnpoint04:
                    MyScriptWrapper.SetMaxHealth(bot, 0.3f);
                    break;
                case EntityID.MinersSpawnpoint05:
                    MyScriptWrapper.SetMaxHealth(bot, 0.3f);
                    break;
              
                default:
                    break;
            }
        }

        private void LetGoMarcus()
        {
            m_MarcusBot.SpeedModifier = 1.0f;
            
            //change music mood
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.Mystery);

            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyPrefabContainer motherShip = entity as MyPrefabContainer;
                if (motherShip != null && MyFactions.GetFactionsRelation(motherShip.Faction, MySession.PlayerShip.Faction) == MyFactionRelationEnum.Enemy)
                {
                    if (motherShip.EntityId.Value.NumericValue == (uint)EntityID.RussianMothership3)
                        continue;

                    MyScriptWrapper.UnhideEntity(entity);
                }
            }
        }

        private void RedHubLoaded(MyMissionBase sender)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("RedHubLoaded");


            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3500, 6000), LetGoMarcus, false);

            m_MarcusBot.SetWorldMatrix(Matrix.CreateWorld(MySession.PlayerShip.GetPosition() - MySession.PlayerShip.GetForward() * MySession.PlayerShip.LocalVolume.Radius * 2,
                                                MySession.PlayerShip.WorldMatrix.Forward, MySession.PlayerShip.WorldMatrix.Up));
            m_MarcusBot.Idle();
            m_MarcusBot.Physics.Clear();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("All lights set");

            foreach (var light in m_LightsAllOfThem)
            {
                if (MyScriptWrapper.TryGetEntity(light) is MyPrefabLight)
                {
                    MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(light), false);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Turrets");

            foreach (var turret in m_turrets)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(turret), false);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Alarms");

            // activate alarm
            MyScriptWrapper.SetAlarmMode(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl1).Parent, true);

            // disable vendor
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.VendorHangar), false);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Bots");

            // marcus to follow
            m_MarcusBot.Follow(MySession.PlayerShip);

            // activate first ambush
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointStart1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointStart2);
            // minership escaping
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointStart3);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Detectors");

            // activate further detectors
            MyEntityDetector blueDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorBlue));
            blueDetector.OnEntityEnter += BlueHubReached;
            blueDetector.On();
            MyEntityDetector greenDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorGreen));
            greenDetector.OnEntityEnter += GreenHubReached;
            greenDetector.On();
            MyEntityDetector redDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorRed));
            redDetector.OnEntityEnter += RedHubReached;
            redDetector.On();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Music");

            //change music mood to battle
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);
            MyAudio.MusicAllowed = false; //to forbid action music caused by bot fight
            MissionTimer.RegisterTimerAction(10000, PlayActionMusic, false);

            // set lights off
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light1), false);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light2), false);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light3), false);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light4), false);

            // explosions & dialogue
            MissionTimer.RegisterTimerAction(2000, m_farExplosionAction, true);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3400, 3600), m_subShakeAction, false);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3600, 3800), m_subShakeAction, false);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3800, 4000), m_subShakeAction, false);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Next dialogue");

            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0205_ACTION);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Unhide russians");

            // Unhide russians
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.RussianMothership1));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.RussianMothership2));
           // MyScriptWrapper.UnhideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.RussianMothership3));

            // unhide crashing mothership
            MyScriptWrapper.UnhideEntity(m_MSmove.Parent);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Hide asteroids");

            // hide static asteroids
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_01));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_02));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_03));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_04));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_05));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_06));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_07));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_08));
            MyScriptWrapper.HideEntity(MyScriptWrapper.TryGetEntity((uint)EntityID.Asteroid_Static_09));

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Close miners");

            List<MySpawnPoint.Bot> Miners = MyScriptWrapper.GetSpawnPointBots((uint)EntityID.MinersSpawnpoint02);
            foreach (var bot in Miners)
            {
                if (bot.Ship != null)
                {
                    bot.Ship.MarkForClose();
                }
            }

            Miners = MyScriptWrapper.GetSpawnPointBots((uint)EntityID.MinersSpawnpoint01);
            foreach (var bot in Miners)
            {
                if (bot.Ship != null)
                {
                    bot.Ship.MarkForClose();
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Start particles");

            // start particles
            foreach (var particle in m_particles)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void PlayActionMusic()
        {
            MyAudio.MusicAllowed = true;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA26");
        }

        private void BlueHubReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BlueHubLoaded");

            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointBlue1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointBlue2);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointEnemyStream01);
                //MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointEnemyStream02);
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointStart1);
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointStart2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointBlue3); // miners escaping
                sender.Off();

                // explosion spawn
                MyScriptWrapper.AddExplosion(MyScriptWrapper.TryGetEntity((uint)EntityID.ExplosionBlue), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f);
                MissionTimer.RegisterTimerAction(500, ScriptExplosions01_01, false);
                MissionTimer.RegisterTimerAction(800, ScriptExplosions01_02, false);
                MissionTimer.RegisterTimerAction(900, ScriptExplosions01_03, false);
                MissionTimer.RegisterTimerAction(1100, ScriptExplosions01_04, false);
                
                // player priority
                //MyScriptWrapper.SetEntityPriority(MySession.PlayerShip, -25);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void ScriptExplosions01_01()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ScriptExplosions01_01");
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Explosion1), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void ScriptExplosions01_02()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ScriptExplosions01_02");
            MyScriptWrapper.IncreaseHeadShake(30);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Explosion2), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void ScriptExplosions01_03()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ScriptExplosions01_03");
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Explosion3), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void ScriptExplosions01_04()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ScriptExplosions01_04");
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Explosion4a), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f, true);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.Explosion4b), MyExplosionTypeEnum.MISSILE_EXPLOSION, 50f, 50f, true);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void GreenHubReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GreenHubReached");
            if (MySession.IsPlayerShip(entity))
            {
                // player priority raise
                //MyScriptWrapper.SetEntityPriority(MySession.PlayerShip, 5);

                // set priority for marcus
                //MyScriptWrapper.SetEntityPriority(m_MarcusBot, 3);

                // activate spawns
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointGreen1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointGreen2);
                // MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointGreen3);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointGreen4); // panicking miners
                // explosion
                MissionTimer.RegisterTimerAction(4500, FuelTankExplosion01, false);
                MissionTimer.RegisterTimerAction(5000, FuelTankExplosion02, false);
                sender.Off();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void FuelTankExplosion01() 
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FuelTankExplosion01");
            // add beams exploding
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.ExplosionFuelTank1a });
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1b), false);
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1c), true);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1b), MyExplosionTypeEnum.BOMB_EXPLOSION, 1000);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1c), MyExplosionTypeEnum.BOMB_EXPLOSION, 1000);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void FuelTankExplosion02()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("FuelTankExplosion02");
            // add beams appeareing
            //MyScriptWrapper.DestroyEntities(m_beamsBefore);
            MyScriptWrapper.TryHideEntities(m_beamsBefore, true);
            MyScriptWrapper.TryUnhideEntities(m_beamsAfter, true);

            MyScriptWrapper.IncreaseHeadShake(30);
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank2), true);
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1c), true);

            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank2), MyExplosionTypeEnum.BOMB_EXPLOSION, 1000);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionFuelTank1c), MyExplosionTypeEnum.BOMB_EXPLOSION, 1000);

                              /*
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.VoxelSphere1, 80f, MyMwcVoxelMaterialsEnum.Stone_04);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.VoxelSphere2, 98f, MyMwcVoxelMaterialsEnum.Stone_04);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.VoxelSphere3, 60f, MyMwcVoxelMaterialsEnum.Stone_04);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.VoxelSphere4, 86f, MyMwcVoxelMaterialsEnum.Stone_03);
                                */
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void RedHubReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("RedHubReached");
            if (MySession.IsPlayerShip(entity))
            {
                // stop bots spawning inside mines, some cleanup there would be fine (kill all bots there etc.)
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointEnemyStream03);
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointGreen1);
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointGreen2);
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointGreen3);
                sender.Off();

                //change music mood to calmer
                //MyAudio.ApplyTransition(MyMusicTransitionEnum.SadnessOrDesperation);

                // explosion
                //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade2), true);
                MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade2), MyExplosionTypeEnum.BOMB_EXPLOSION, 10);

                MissionTimer.RegisterTimerAction(1200, BarricadeExplosion01, false);
                MissionTimer.RegisterTimerAction(1400, BarricadeExplosion02, false);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void BarricadeExplosion01()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BarricadeExplosion01");

            MyScriptWrapper.IncreaseHeadShake(20);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade1), MyExplosionTypeEnum.BLASTER_EXPLOSION, 80f, 90f, true);
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade1), true);
            MyScriptWrapper.TryUnhideEntities(m_barricade, true);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void BarricadeExplosion02()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BarricadeExplosion02");

            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade2), false);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionBarricade2), MyExplosionTypeEnum.BOMB_EXPLOSION, 10);

            // barricade
            /*
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.BarricadeVoxels1, 40f, MyMwcVoxelMaterialsEnum.Stone_02);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.BarricadeVoxels2, 60f, MyMwcVoxelMaterialsEnum.Stone_04);
            MyScriptWrapper.AddVoxelHand((uint)EntityID.VoxelMap, (uint)EntityID.BarricadeVoxels3, 30f, MyMwcVoxelMaterialsEnum.Stone_02);
              */
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void BarricadeLoaded(MyMissionBase sender)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BarricadeLoaded");

            // barrier breached without destroying all parts
            MyEntityDetector barricadeDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorBarricade));
            barricadeDetector.OnEntityEnter += BarricadeBReached;
            barricadeDetector.On();

            // play dialogue
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0400_BARRICADE);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void BarricadeBReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BarricadeBreached");

            if (MySession.IsPlayerShip(entity))
            {
                if (!MyScriptWrapper.IsMissionFinished(m_barricadeSubmission.ID))
                {
                    m_barricadeSubmission.Success();
                }
                sender.Off();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void BarricadeSuccess(MyMissionBase sender)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BarricadeSuccess");

            // play dialogue
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0500_TOBASE);

            // spawn crazy russian
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointCrazyRussian);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void ToTheBaseLoaded(MyMissionBase sender)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("ToTheBaseLoaded");

            // activate detector for commentary to cutscene
            MyEntityDetector pipeDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorPipe));
            pipeDetector.OnEntityEnter += PipeReached;
            pipeDetector.On();
            
            //change music mood
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction);

            // disable shakes
            m_minesShake = false;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void PipeReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PipeReached");

            if (MySession.IsPlayerShip(entity))
            {
                //m_moveCargo2 = true;
                
                // comment on cutscene
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0450_MOTHERSHIPS);

                // activate explosions
                m_flyMS = true;
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.CrashingMothershipSmoke), true);

                // start cutscene with russian motherships fighting eac
                /*
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPipe1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPipe2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPipe3);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPipe4);
                  */
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SharpshooterSpawn);

                // cleanup crazy russian
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointCrazyRussian);

                //change music mood to epic
                //MyAudio.ApplyTransition(MyMusicTransitionEnum.SadnessOrDesperation);

                sender.Off();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void CommandCenterLoaded(MyMissionBase sender)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CommandCenterLoaded");

            // stop cutscene
            
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPipe1);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPipe2);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPipe3);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPipe4);
              
            // activate bots inside base
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointInside);
             
            // activate detector for another wave of bots
            MyEntityDetector routeLivingDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorRouteLiving));
            routeLivingDetector.OnEntityEnter += RouteLivingReached;
            routeLivingDetector.On();

            // activate detector for Command center entrace.
            //MyEntityDetector commandDoorDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorCommandDoor));
            //commandDoorDetector.OnEntityEnter += commandDoorReached;
            //commandDoorDetector.On();

            // lock doors around control centre
            //MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl1), false);
            //MyScriptWrapper.SetEntityMode(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl2), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl3), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl4), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl5), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl6), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl7), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl8), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl9), false);

            // lock ore processing and research centers
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch2), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch3), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch4), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch5), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch6), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch7), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch8), false);
            // always locked
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorLocked1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorLocked2), false);
            // generator room always locked
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorGenerator1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorGenerator2), false);

            //change music mood
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.Mystery);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void RouteLivingReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CommandCenterLoaded");

            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving2);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving3);
                
                // some russian bullshit
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1200_RUSSIANCHAT_01);
                 
                sender.Off();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void CommandDoorReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CommandCenterLoaded");

            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl1), true);
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl2), true);
                sender.Off();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        private void GeneratorLoaded(MyMissionBase sender)
        {
            // deactivate spawns in living area
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointInside);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving1);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving2);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointRouteLiving3);

            // activate detectors for ambush and stuff happening inside workshop
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointCommandCentre1);
            MyEntityDetector workshopDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(76666));
            workshopDetector.OnEntityEnter += WorkshopReached;
            workshopDetector.On();

            //change music mood
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction);
        }

        private void GeneratorSuccess(MyMissionBase sender)
        {
            if (!MyScriptWrapper.IsEntityDead((uint)EntityID.GeneratorLight))
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.GeneratorLight), false);
            }

            // disable vendor
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.VendorHangar), true);

            //activate lights
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light1), true);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light2), true);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light3), true);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light4), true);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light5), false);
            MyScriptWrapper.SetLight(MyScriptWrapper.TryGetEntity((uint)EntityID.Light6), false);

            foreach (var light in m_LightsAllOfThem)
            {
                if ((MyScriptWrapper.TryGetEntity(light) is MyPrefabLight))
                {
                    MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(light), true);
                    // MyPrefabLight lightprefab = MyScriptWrapper.GetEntity(light) as MyPrefabLight;
                    // lightprefab.GetLight().Intensity = lightprefab.GetLight().Intensity * 5;
                }
            }

            // animate generators
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Generator1), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Generator2), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Generator3), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Generator4), true);

            // unlock way to command centre
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl8), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl9), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl7), true);

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl3), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl4), true);

        }


        private void WorkshopReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointCommandCentre1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointWorkshop1);
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointWorkshop2);
                sender.Off();

                //change music mood
                //MyAudio.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere);

                // some russian bullshit
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1400_RUSSIANCHAT_03);
            }
        }

        private void CommandCenterAgainLoaded(MyMissionBase sender)
        {
            foreach (var turret in m_turrets)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity(turret), true);
            }

            // activate detector for Command center entrace.
            var storageDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorCommandCentreEntrance));
            storageDetector.OnEntityEnter += StorageRouteReached;
            storageDetector.On();

            // activate
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointCommandCentre2);

            // deactivate spawns from previous session
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointWorkshop1);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointWorkshop2);
            
            // dialogue
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0560_GENERATORUP);
        }

        private void HangarLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA27");
        }

        private void HangarRouteReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                sender.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointHangarRoute);

                // some russian bullshit
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1500_RUSSIANCHAT_04);
            }
        }

        private void StorageRouteReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                sender.Off();
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointStorageRoute);
            }
        }


        private List<MySmallShipBot> saveMinersEnemyShips = new List<MySmallShipBot>();

        private void OptionalSaveMinersAcceptSubmissionLoaded(MyMissionBase sender)
        {
            MyEntityDetector hangarDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorHangar));
            hangarDetector.OnEntityEnter += HangarRouteReached;
            hangarDetector.On();

            // unlock way from command centre
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl5), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl6), true);
            // lock way to command centre
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl8), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl9), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorNearControl7), false);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA28");
        }

        private void OptionalSaveMinersAcceptSubmissionSuccess(MyMissionBase sender)
        {
            // open doors
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch8), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch7), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch6), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch5), true);
            // way down
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch2), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch1), true);
            // way to miners
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch3), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorProcessingResearch4), true);

            // set off detector in storage but spawn bots there
            var storageDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorCommandCentreEntrance));
            storageDetector.Off();
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointStorageRoute);
            
            // Dialogue
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1000_RESEARCHERS_1);
        }

        private void OptionalSaveMinersSubmissionSuccess(MyMissionBase sender)
        {
            // set miners to follow player
            MySmallShipBot miner01 = MyScriptWrapper.TryGetEntity((uint)EntityID.SavedMiner1) as MySmallShipBot;
            miner01.Follow(MySession.PlayerShip);
            MySmallShipBot miner02 = MyScriptWrapper.TryGetEntity((uint)EntityID.SavedMiner2) as MySmallShipBot;
            miner02.Follow(MySession.PlayerShip);
            MySmallShipBot miner03 = MyScriptWrapper.TryGetEntity((uint)EntityID.SavedMiner3) as MySmallShipBot;
            miner03.Follow(MySession.PlayerShip);
                              
            // send there bots from storage
            MyScriptWrapper.GetSmallShipBotsInDummyPoint((uint)EntityID.LoadingBayDummy, saveMinersEnemyShips, MinerWars.AppCode.Game.World.Global.MyFactionRelationEnum.Enemy);
            foreach (MySmallShipBot enemyShip in saveMinersEnemyShips)
            {
                enemyShip.SetWaypointPath("BackHangAssault");
            }   

            // Dialogue
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1100_RESEARCHERS_2);
        }
        
        private void HangarLastStandLoaded(MyMissionBase sender)
        {
            // Player entered final hangar for big battle

            // Lock all doors
            /*
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar1), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar2), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar3), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar4), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar5), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar6), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar7), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar8), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar9), false);
            // Lock door you came from
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar10), false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.DoorHangar11), false);
              */

            /*
            m_spawnPointSmartWaves.SpawnIntervalInMS = 7000;
            m_spawnPointSmartWaves.SpawnInGroups = true;
            m_spawnPointSmartWaves.MaxBotsCount = 5;
              */
            // change music mood
            //MyAudio.ApplyTransition(MyMusicTransitionEnum.LightFight);

            // set player ship priority to be equal to everything else
            //MyScriptWrapper.SetEntityPriority(MySession.PlayerShip, 0);

            // Timer
            MissionTimer.RegisterTimerAction(5000, ActivatePhase1, false);
            MissionTimer.RegisterTimerAction(10000, ActivatePhase1Explosion, false);
            MissionTimer.RegisterTimerAction(60000, ActivatePhase2, false);
            MissionTimer.RegisterTimerAction(68000, ActivatePhase2Explosion, false);
            MissionTimer.RegisterTimerAction(120000, ActivatePhase3, false);
            MissionTimer.RegisterTimerAction(130000, ActivatePhase3Explosion, false);
            MissionTimer.RegisterTimerAction(170000, DangerMark, false);
            MissionTimer.RegisterTimerAction(179000, hangarBayExplosion01, false);
            MissionTimer.RegisterTimerAction(179300, hangarBayExplosion02, false);
            MissionTimer.RegisterTimerAction(179800, hangarBayExplosion03, false);
            MissionTimer.RegisterTimerAction(40000, ActivateOptionalPhase, false);
            MissionTimer.RegisterTimerAction(60000, ActivateOptionalPhaseExplosion, false);

            // deactivate storage spawn
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointStorageRoute);

            // check this
            //m_optionalSaveMinersAccept.Enabled = false;
            //m_optionalSaveMiners.Enabled = false;

            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Right_Control), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Right_Generator), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Right_RightTurretLight), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Right_Light2), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Left_Control), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Left_Generator), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Left_LeftTurretLight), true);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.TryGetEntity((uint)EntityID.Left_Light2), true);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 100, "KA02b");
        }

        // Last stand timed events
        private void ActivatePhase1()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPhase1);
            
            // some russian bullshit
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_1600_RUSSIANCHAT_05);
        }

        private void ActivatePhase1Explosion()
        {
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.DoorHangar1, (uint)EntityID.DoorHangar2, (uint)EntityID.DoorHangar3 });
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase1), true);
            //MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase1), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
        }

        private void ActivatePhase2()
        {
            // set player ship priority higher than turrets
            //MyScriptWrapper.SetEntityPriority(MySession.PlayerShip, 50);

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPhase2);
        }

        private void ActivatePhase2Explosion()
        {
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.DoorHangar4, (uint)EntityID.DoorHangar5, (uint)EntityID.DoorHangar6, (uint)EntityID.DoorHangar12 });
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase2), true);
            //MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase2), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
        }
        
        private void ActivatePhase3()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPhase3);
        }

        private void ActivatePhase3Explosion()
        {
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.DoorHangar7, (uint)EntityID.DoorHangar8, (uint)EntityID.DoorHangar9 });
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase3), true);
            //MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionPhase3), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
        }

        private void DangerMark()
        {
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionHangarBay2), MyTexts.Danger);
            
            // unhide final mothership
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity("Madelyn"));

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_SURVEY_SITE_0750_NEAREND);
        }

        private void hangarBayExplosion01()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionHangarBay1), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionCenterHangarBay1), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.HangarBay1a, (uint)EntityID.HangarBay1b });

            //Update madelyn waypoints
            RegenerateWaypointGraph();
        }

        private void hangarBayExplosion02()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionHangarBay2), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionCenterHangarBay2), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.HangarBay2a, (uint)EntityID.HangarBay2b });
        }

        private void hangarBayExplosion03()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionCenterHangarBay3), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.HangarBay3 });
        }

        private void ActivateOptionalPhase()
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointPhase1);    
        }

        private void ActivateOptionalPhaseExplosion()
        {
            
            //MyScriptWrapper.DestroyEntities(new List<uint> { (uint)EntityID.DoorHangar10, (uint)EntityID.DoorHangar11 });
            //MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionOptionalPhase), true);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionOptionalPhase), MyExplosionTypeEnum.BOMB_EXPLOSION, 15000);
        }

        private void RegenerateWaypointGraph()
        {
            MyScriptWrapper.RegenerateWaypointGraph();
        }

        private void HangarEscapeLoaded(MyMissionBase sender)
        {
            // Big battle ended, mothership is here, hangar doors are opened (exploded)
                                  /*
            m_spawnPointSmartWaves.SpawnIntervalInMS = 0;
            m_spawnPointSmartWaves.SpawnInGroups = false;
            m_spawnPointSmartWaves.MaxBotsCount = 4;
                                    */

            // PetrM - show Madelyn
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity("Madelyn"));

            MissionTimer.RegisterTimerAction(1000, RegenerateWaypointGraph, false);

            

            // In the end some more bots will spawn, towers will go offline and hangar will open with mothership waiting
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnPointSurvived);
            //change music mood
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 100);
            // deactivate spawnpoints inside base
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPhase1);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPhase2);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.SpawnPointPhase3);

            MyEntity marker = MyScriptWrapper.TryGetEntity((uint)EntityID.ExplosionHangarBay2);
            if (marker != null)
            {
                MyScriptWrapper.RemoveEntityMark(marker);
            }

            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.EAC_SURVEY_SITE_0800_PICKUP);

            //sender.Location.Entity = MyScriptWrapper.GetMothershipHangar(MyScriptWrapper.GetEntity("Madelyn"));
        }

    }
}

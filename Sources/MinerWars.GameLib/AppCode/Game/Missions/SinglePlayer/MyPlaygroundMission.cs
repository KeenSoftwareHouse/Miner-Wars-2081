using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
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

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyPlaygroundMission : MyMission
    {
        private MyObjective m_cubeSubmission;
        
        private MyTimerActionDelegate m_subShakeAction;
        private MyTimerActionDelegate m_shakeAction;

        private MyEntityDetector doorDetector1;
        private MyEntityDetector doorDetector2;

        private MySmallShipBot m_Speedy;

        private List<uint> Puzzle = new List<uint> { 448,447,449,442,443,441,444,445,446,430,424,426,429,425,427,431,423,428,418,417,419,412,411,413,409,407,404 };
        private const uint RadarEntity = 1812;
        private Vector3 m_turretHighlightColor = new Vector3(9 / 255f, 96 / 255f, 177 / 255f);

        public MyPlaygroundMission()
        {
            ID = MyMissionID.PLAYGROUND; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Playground");
            Name = MyTextsWrapperEnum.PLAYGROUND;
            Description = MyTextsWrapperEnum.EmptyDescription;
            Flags = MyMissionFlags.HiddenInSolarMap;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-85381496, 0, -85381496);

            Location = new MyMissionLocation(baseSector, 315);

            RequiredMissions = new MyMissionID[] { };

            m_objectives = new List<MyObjective>();


            var placeRadarMission = new MyUseObjective(
                new StringBuilder("Find a place for the alien radar"),
                MyMissionID.PLAYGROUND_SUBMISSION_01,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)1889),
                MyTextsWrapperEnum.PressToTakeTransmitter,
                MyTextsWrapperEnum.Transmitter,
                MyTextsWrapperEnum.TransferInProgress,
                10
            ) { SaveOnSuccess = false };
                    placeRadarMission.OnMissionSuccess += PlaceRadarSucces;

            m_objectives.Add(placeRadarMission);


            /*
            var playgroundSubmission = new MySubmission(
                new StringBuilder("Play around a little"),
                MyMissionID.PLAYGROUND_SUBMISSION_02,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, 831)
            );
            playgroundSubmission.OnMissionSuccess += PlaygroundSubmissionSubmissionSuccess;
            m_submissions.Add(playgroundSubmission);
            */
            /*
            m_cubeSubmission = new MySubmission(
                new StringBuilder("3D cubes"),
                MyMissionID.PLAYGROUND_SUBMISSION_02,
                new StringBuilder("Reach the exit. Enter every cube exactly once."),
                null,
                this,
                new MyMissionID[] { MyMissionID.PLAYGROUND_SUBMISSION_01 },
                null
            );
            m_submissions.Add(m_cubeSubmission);
            */
            m_subShakeAction = new MyTimerActionDelegate(SubShake);
            m_shakeAction = new MyTimerActionDelegate(FarExplosion);
        }

        public override void Load()
        {
            base.Load();

            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3000, 5000), m_shakeAction, true);

            doorDetector1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(367));
            doorDetector1.OnEntityEnter += DoorDetector1Open;
            doorDetector1.On();

            doorDetector2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(366));
            doorDetector2.OnEntityEnter += DoorDetector2Closed;
            doorDetector2.On();
                        
            for (int i = 0; i < mines.GetLength(0); i++)
            {
                MyEntityDetector mineDetector = MyScriptWrapper.GetDetector(mines[i, 1]);
                mineDetector.OnEntityEnter += new OnEntityEnter(mineDetector_OnEntityEnter);
                mineDetector.OnEntityPositionChange += new OnEntityPositionChange(mineDetector_OnEntityPositionChange);
                mineDetector.On();
            }

            MyScriptWrapper.OnBotReachedWaypoint += OnBotReachedWaypoint;
            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;

            //aliendetector
            MyScriptWrapper.Highlight(RadarEntity, true,this);
            MyScriptWrapper.EnablePhysics(RadarEntity, false);

            //aliendetector
            MyScriptWrapper.Highlight(1962, true,this);
            MyScriptWrapper.EnablePhysics(1962, false);

        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.OnBotReachedWaypoint -= OnBotReachedWaypoint;
        }

        uint[,] mines = new uint[,]
        {
            { 626,631 },
            { 628,632 },
            { 629,634 },
            { 627,633 },
            { 637,636 },
            { 694,695 },
        };

        const float mineDamage = 25;
        const float mineExplosionRadius = 25;
        const float mineStartRadius = 10;
        MySoundCue? beepCue = null;
        void mineDetector_OnEntityPositionChange(MyEntityDetector sender, MyEntity entity, Vector3 newPosition)
        {
            if (sender.Closed)
                return;

            if (entity == MySession.PlayerShip)
            {
                if (beepCue == null || !beepCue.Value.IsPlaying)
                {
                    beepCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxHudAlarmDamageA);
                }

                float distance = (entity.GetPosition() - sender.GetPosition()).Length();
                
                if (distance < mineStartRadius)
                {
                    uint mineId = 0;
                    for (int i = 0; i < mines.GetLength(0); i++)
                    {
                        if (mines[i, 1] == sender.Parent.EntityId.Value.NumericValue)
                        {
                            mineId = mines[i, 0]; 
                        }
                    }
                    ExplodeMine(mineId);
                    sender.Off();
                    sender.Parent.MarkForClose();
                }

            }
        }

        public void ExplodeMine(uint entityId)
        {
            MyEntity mine = MyScriptWrapper.GetEntity(entityId);
            MyExplosion newExplosion = MyExplosions.AddExplosion();
            if (newExplosion != null)
            {
                newExplosion.Start(0, mineDamage, 0, MyExplosionTypeEnum.BOMB_EXPLOSION, new BoundingSphere(mine.GetPosition(), mineExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, 1, ownerEntity: mine);
            }
            mine.MarkForClose();
        }

        void mineDetector_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {

        }

        private void PlaygroundSubmissionSubmissionSuccess(MyMissionBase sender)
        {
            var puzzleReset = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(572));
            puzzleReset.OnEntityEnter += new OnEntityEnter(puzzleReset_OnEntityEnter);
            puzzleReset.On();
            var puzzleFinish = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(753));
            puzzleFinish.OnEntityEnter += new OnEntityEnter(puzzleFinish_OnEntityEnter);
            puzzleFinish.On();
            foreach (uint item in Puzzle)
            {
                var puzzleDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(item));
                puzzleDetector.OnEntityEnter += new OnEntityEnter(puzzleDetector_OnEntityEnter);
                puzzleDetector.On();
            }
        }

        void puzzleReset_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            Reset();
        }

        void puzzleFinish_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (CheckWin())
            {
                m_cubeSubmission.Success();
                foreach (uint item in Puzzle)
                {
                    var puzzleDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(item));
                    puzzleDetector.Off();
                }
                var puzzleReset = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(572));
                puzzleReset.Off();
                var puzzleFinish = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(753));
                puzzleFinish.Off();
            }
            else
            {
                Reset();
            }
        }

        void puzzleDetector_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyDummyPoint dummyPoint = (MyDummyPoint)sender.Parent;
            if (dummyPoint.Color == Color.Green.ToVector4())
            {
                Reset();
            }
            else
            {
                dummyPoint.Color = Color.Green.ToVector4();
            }
        }

        private bool CheckWin()
        {
            foreach (uint item in Puzzle)
            {
                var puzzleDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(item));
                MyDummyPoint dummyPoint = (MyDummyPoint)puzzleDetector.Parent;
                if (dummyPoint.Color != Color.Green.ToVector4())
                {
                    Reset();
                    return false;
                }
            }
            return true;
        }

        private void Reset()
        {
            foreach (uint item in Puzzle)
            {
                var puzzleDetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity(item));
                MyDummyPoint dummyPoint = (MyDummyPoint)puzzleDetector.Parent;
                dummyPoint.Color = Color.Yellow.ToVector4();
            }
        }

        private void SubShake()
        {
            MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomInt(3, 4));
        }

        private void FarExplosion()
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
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(3000, 5000), m_shakeAction, false);
            //TODO add particle effect
        }

        private void DoorDetector1Open(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(370), false);
                doorDetector2.On();
                doorDetector1.On();
            }
        }

        private void DoorDetector2Closed(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(370), true);
                doorDetector2.On();
                doorDetector1.On();
            }
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (spawnpoint == MyScriptWrapper.GetEntity(1660))
            {
                m_Speedy = (MySmallShipBot)bot; 
            }   
        }

        void OnBotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (bot == m_Speedy)
            {
                switch (MyScriptWrapper.GetEntityId(waypoint))
                {
                    case 1652:
                        m_Speedy.SpeedModifier = 0.3f;
                        break;
                    case 1646:
                        m_Speedy.SpeedModifier = 2.0f;
                        break;
                    default:
                        break;
                }
            }
        }

        void PlaceRadarSucces(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight(RadarEntity, false,this);
            MyScriptWrapper.EnablePhysics(RadarEntity, true);

           
            var inventory = MyScriptWrapper.GetPlayerInventory();
            var items = new List<MyInventoryItem> ();
            inventory.GetInventoryItems(ref items, MyMwcObjectBuilderTypeEnum.SmallShip_Radar, null);//SmallShip_AlienObjectDetector,

            inventory.RemoveInventoryItems(items,false);

        }


    }
}

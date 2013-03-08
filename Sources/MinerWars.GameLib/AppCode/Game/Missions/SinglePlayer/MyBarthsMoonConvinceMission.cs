using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyBarthsMoonConvinceMission : MyBarthsMoonMissionBase
    {
        private MyEntityDetector m_flyBackDetector;
        private List<uint> m_botSpawnpoints = new List<uint>() { 
            (uint)EntityID.AmbushSpawnpoint0,
            (uint)EntityID.AmbushSpawnpoint1,
            (uint)EntityID.AmbushSpawnpoint2,
            (uint)EntityID.EngineSpawnpoint, 
            (uint)EntityID.EngineSpawnpoint2, 
            (uint)EntityID.CaveSpawnpoint1,
            (uint)EntityID.CaveSpawnpoint2,
            (uint)EntityID.CaveSpawnpoint3,
            (uint)EntityID.CaveSpawnpoint4,
            (uint)EntityID.FlyBackSpawnpoint0,
            (uint)EntityID.FlyBackSpawnpoint1,
            (uint)EntityID.FlyBackSpawnpoint2,
        };
        
        private MyEntityDetector m_ambushDetector { get; set; }
        private List<uint> m_hideEntities = new List<uint>() { 2203565, 1958806, 2328595, 2577321 };
        private MyEntityDetector m_dialogue2Detector;
        private MyEntityDetector m_dialogue5Detector;


        public override void ValidateIds()
        {
            var list = new List<uint>();
            list.AddRange(m_hideEntities);


            foreach (var u in list)
            {
                MyScriptWrapper.GetEntity(u);
            }

            base.ValidateIds();
        }
        
        public MyBarthsMoonConvinceMission()
            : base(
            MyMissionID.BARTHS_MOON_CONVINCE,
            new StringBuilder("03-Barth's moon convince"), 
            MyTextsWrapperEnum.BARTHS_MOON_CONVINCE,
            MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_Description,
            new MyMissionID[] { MyMissionID.LAIKA }, 
            new EntityID[]{},
            EntityID.PlayerStartLocationConvince
            )
        {
            RequiredMissions = new MyMissionID[] { MyMissionID.LAIKA };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_RETURN_BACK_TO_MADELYN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };
            AchievementName = MySteamAchievementNames.Mission03_Barths;
            
            m_objectives = new List<MyObjective>();

            var meetThomasBarth = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_MEET_THOMAS_BART_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_MEET_THOMAS_BART,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_MEET_THOMAS_BART_Description),
                null,
                this,
                new MyMissionID[] {},
                new MyMissionLocation(baseSector, (uint)EntityID.ThomasBartId),
                startDialogId: MyDialogueEnum.BARTHS_MOON_CONVINCE_0100, 
                radiusOverride: 50
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudBarth };
            meetThomasBarth.OnMissionLoaded += MeetThomasBarthOnLoaded;
            meetThomasBarth.OnMissionCleanUp += MeetThomasBarthOnUnload;
            m_objectives.Add(meetThomasBarth);

            var talkWithThomasBarth = new MyBarthsMoonSubmissionTalkWithThomasBarth(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_TALK_WITH_THOMAS_BART_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_TALK_WITH_THOMAS_BART,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_TALK_WITH_THOMAS_BART_Description),
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_MEET_THOMAS_BART },
                MyDialogueEnum.BARTHS_MOON_CONVINCE_0300,
                true
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWithThomasBarth);

            var flyToEnemyBase = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_FLY_TO_ENEMY_BASE_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_FLY_TO_ENEMY_BASE,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_FLY_TO_ENEMY_BASE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_TALK_WITH_THOMAS_BART },
                new MyMissionLocation(baseSector, (uint)EntityID.FlyToEnemyBaseSubmissionLocation),
                null,
                successDialogId: MyDialogueEnum.BARTHS_MOON_CONVINCE_0500,
                startDialogId: MyDialogueEnum.BARTHS_MOON_CONVINCE_0400
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudEnemyBase };
            flyToEnemyBase.OnMissionLoaded += FlyToEnemyBaseOnLoaded;
            flyToEnemyBase.OnMissionCleanUp += FlyToEnemyBaseOnCleanUp;
            m_objectives.Add(flyToEnemyBase);

            var destroyShipSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_DESTROY_SHIP_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_DESTROY_SHIP,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_DESTROY_SHIP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_FLY_TO_ENEMY_BASE },
                new List<uint>() { (uint)EntityID.ShipGenerator },
                new List<uint>() {},
                startDialogID: MyDialogueEnum.BARTHS_MOON_CONVINCE_0600
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudGenerator };
            destroyShipSubmission.OnMissionLoaded += DestroyShipSubmissionOnLoad;
            m_objectives.Add(destroyShipSubmission);

            var destroyGeneratorSubmission = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_DESTROY_GENERATOR_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_DESTROY_GENERATOR,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_DESTROY_GENERATOR_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_DESTROY_SHIP },
                new List<uint>() { /*(uint)EntityID.BaseGenerator,*/ (uint)EntityID.MainSmallShip },
                new List<uint>() { /*(uint)EntityID.EngineSpawnpoint, (uint)EntityID.EngineSpawnpoint2, */(uint)EntityID.CaveSpawnpoint1, (uint)EntityID.CaveSpawnpoint2, (uint)EntityID.CaveSpawnpoint3, (uint)EntityID.CaveSpawnpoint4 },
                showMarks: false
            ) { SaveOnSuccess = true };
            destroyGeneratorSubmission.OnMissionLoaded += DestroyGeneratorSubmissionOnLoad;
            destroyGeneratorSubmission.OnMissionSuccess += new MissionHandler(destroyGeneratorSubmission_OnMissionSuccess);
            m_objectives.Add(destroyGeneratorSubmission);
            
            var findTransmitterSubmission = new MyUseObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_FIND_TRANSMITTER_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_FIND_TRANSMITTER,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_FIND_TRANSMITTER_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_DESTROY_GENERATOR },
                new MyMissionLocation(baseSector, (uint)EntityID.TakeTransmitterDummy),
                MyTextsWrapperEnum.PressToCollectReward,
                MyTextsWrapperEnum.Reward,
                MyTextsWrapperEnum.TransferInProgress,
                3000
            ) { SaveOnSuccess = true };

            findTransmitterSubmission.OnMissionLoaded += FindTransmitterSubmissionOnMissionLoaded;
            findTransmitterSubmission.OnMissionSuccess += FindTransmitterSubmissionOnSuccess;
            m_objectives.Add(findTransmitterSubmission);

            /*
            var flyBackSubmission = new MyBarthsMoonSubmissionTalkWithThomasBarth(
                new StringBuilder("Fly back to Barth"),
                MyMissionID.BARTHS_MOON_CONVINCE_FLY_BACK_TO_BARTH,
                new StringBuilder(""),
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_FIND_TRANSMITTER },
                null,
                false
            ) { SaveOnSuccess = false };
            flyBackSubmission.OnMissionLoaded += FlyBackSubmissionOnLoaded;
            flyBackSubmission.OnMissionCleanUp += FlyBackSubmissionOnCleanUp;
            m_objectives.Add(flyBackSubmission);
            */
            
            var backToMadelyn = new MyObjective(
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_RETURN_BACK_TO_MADELYN_Name),
                MyMissionID.BARTHS_MOON_CONVINCE_RETURN_BACK_TO_MADELYN,
                (MyTextsWrapperEnum.BARTHS_MOON_CONVINCE_RETURN_BACK_TO_MADELYN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.BARTHS_MOON_CONVINCE_FIND_TRANSMITTER },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                startDialogId: MyDialogueEnum.BARTHS_MOON_CONVINCE_0700,
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            m_objectives.Add(backToMadelyn);
        }

        void destroyGeneratorSubmission_OnMissionSuccess(MyMissionBase sender)
        {
            var madelynDummy = MyScriptWrapper.GetEntity((uint)EntityID.MadelynDummyId);
            MyScriptWrapper.Move(MyScriptWrapper.GetEntity("Madelyn"), madelynDummy.WorldMatrix.Translation, madelynDummy.WorldMatrix.Forward, madelynDummy.WorldMatrix.Up);
            MyScriptWrapper.RegenerateWaypointGraph();
        }

        public override void Load()
        {
            base.Load();
            EnableCorrectBarths((uint)EntityID.ThomasBartId, (uint)EntityID._01SmallShipBarth);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity("Marcus"), 10);
            MyScriptWrapper.SetEntityPriority(GetEntity(EntityID.RaidersMothership), -1);

            var mainSmallShip = GetEntity(EntityID.MainSmallShip) as MySmallShipBot;
            if (mainSmallShip != null)
            {
                mainSmallShip.SetWaypointPath("inside");
                mainSmallShip.Patrol();
            }

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            MyScriptWrapper.SpawnpointBotsKilled += OnSpawnpointBotsKilled;
            MyScriptWrapper.EntityDeath += EntityDeath;
            MyScriptWrapper.OnSentenceStarted += MyScriptWrapper_OnSentenceStarted;
            //MyScriptWrapper.OnDialogueFinished += OnDialogueFinished;
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.SpawnpointBotsKilled -= OnSpawnpointBotsKilled;
            MyScriptWrapper.EntityDeath -= EntityDeath;
            MyScriptWrapper.OnSentenceStarted -= MyScriptWrapper_OnSentenceStarted;
            //MyScriptWrapper.OnDialogueFinished -= OnDialogueFinished;
        }


        void MyScriptWrapper_OnSentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (dialogue == MyDialogueEnum.BARTHS_MOON_CONVINCE_0300 && sentence == MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0308)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA12", false);
            }

            if (dialogue == MyDialogueEnum.BARTHS_MOON_CONVINCE_0400 && sentence == MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0405)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA14");
            }
        }

        void FlyToEnemyBaseOnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StealthAction, 100, "KA01");
            m_ambushDetector = InitDetector((uint)EntityID.AmbushDetectorDummy, OnAmbushDetectorEnter);
        }

        void FlyToEnemyBaseOnCleanUp(MyMissionBase sender)
        {
            CleanUpDetector(m_ambushDetector, OnAmbushDetectorEnter);
        }

        void FlyBackSubmissionOnLoaded(MyMissionBase sender)
        {
            m_flyBackDetector = InitDetector((uint)EntityID.FlyBackDetector, OnFlyBackDetectorEnter);
        }

        void OnAmbushDetectorEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.AmbushSpawnpoint0);
            }
        }

        void FlyBackSubmissionOnCleanUp(MyMissionBase sender)
        {
            CleanUpDetector(m_flyBackDetector, OnFlyBackDetectorEnter);
        }

        void OnFlyBackDetectorEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FlyBackSpawnpoint0);
            }
        }

        void MeetThomasBarthOnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Mystery, 100, "KA01");
            MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.ShipFlameEffect), false);
            MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.GeneratorFlameEffect), false);
            m_dialogue2Detector = InitDetector((uint)EntityID.ConvinceDialog2DetectorId, MeetThomasBarthDialogue2Detector);
        }

        void MeetThomasBarthOnUnload(MyMissionBase sender)
        {
            CleanUpDetector(m_dialogue2Detector, MeetThomasBarthDialogue2Detector);
        }

        void MeetThomasBarthDialogue2Detector(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
        	{
                CleanUpDetector(m_dialogue2Detector, MeetThomasBarthDialogue2Detector);
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.BARTHS_MOON_CONVINCE_0200);
	        }
        }

        void DestroyShipSubmissionOnLoad(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 100, "KA06");

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.EngineSpawnpoint);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.EngineSpawnpoint2);
        }
        
        void DestroyGeneratorSubmissionOnLoad(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 100, "KA05");

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CaveSpawnpoint1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CaveSpawnpoint2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CaveSpawnpoint3);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CaveSpawnpoint4);

            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.EngineSpawnpoint);
            MyScriptWrapper.DeactivateSpawnPoint((uint)EntityID.EngineSpawnpoint2);
        }

        void FindTransmitterSubmissionOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.PirateReward, true, sender);
        }

        void FindTransmitterSubmissionOnSuccess(MyMissionBase sender)
         {
            MyScriptWrapper.CloseEntity(MyScriptWrapper.GetEntity((uint)EntityID.PirateReward));
            MyScriptWrapper.AddMoney(50000);
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            // Reduce health of all bots by 25%
            uint id = MyScriptWrapper.GetEntityId(spawnpoint);
            foreach (var spawnpointId in m_botSpawnpoints)
            {
                if (spawnpointId == id)
                {
                    bot.MaxHealth *= 0.25f;
                    break;
                }
            }
        }

        void OnSpawnpointBotsKilled(MySpawnPoint spawnpoint)
        {
            uint id = MyScriptWrapper.GetEntityId(spawnpoint);
            switch (id)
            {
                // Ambush waves
                case (uint)EntityID.AmbushSpawnpoint0:
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.AmbushSpawnpoint1);
                    break;
                case (uint)EntityID.AmbushSpawnpoint1:
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.AmbushSpawnpoint2);
                    break;

                // Fly Back waves
                case (uint)EntityID.FlyBackSpawnpoint0:
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FlyBackSpawnpoint1);
                    break;
                case (uint)EntityID.FlyBackSpawnpoint1:
                    MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FlyBackSpawnpoint2);
                    break;
            }
        }

        void EntityDeath(MyEntity entity, MyEntity killedBy)
        {
            uint id = MyScriptWrapper.GetEntityId(entity);

            if (id == (uint)EntityID.ShipGenerator)
            {
                MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.ShipFlameEffect), true);
            }

            if (id == (uint)EntityID.BaseGenerator)
            {
                MyScriptWrapper.SetParticleEffect(GetEntity(EntityID.GeneratorFlameEffect), true);
            }
        }
                 /*
        void OnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == talk1 || dialogue == talk2)
            {
                var barth = GetEntity(EntityID.ThomasBartId) as MySmallShipBot;
                if (barth != null)
	            {
                    barth.LookTarget = null;
                    barth.SetWaypointPath("interior2");
                    barth.Patrol();
	            }
            }
        }          */
        
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

using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Missions.Objectives;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyJunkyardEACAmbushMission : MyMission
    {

        #region Fields
        private MyObjectiveDialog m_defendMadelyn;
        private MyObjective m_speakWithPolice;
        private MyObjective m_backToMadelyn;
        private MyObjective m_defendMadelyn2;

        private MySmallShipBot m_ravenGuy;
        private MySmallShipBot m_ravenGirl;
        private MySmallShipBot m_marcus;
        private MyEntity m_madelyn;
        private int m_activeTurret = 0;
        private MyPrefabLargeWeapon[] m_madelynTurrets;
        private bool m_speakWithPoliceDialogueFinished;
        private bool m_marcusForCrashDialoguePlayed;
        private float m_gameVolume;

        private readonly List<uint> m_barricadeSpawns = new List<uint>() { 535965, 535921, 535966, 535967, 535968, 535970, 535969};
        private readonly List<uint> m_barricadeLoomers = new List<uint>() { 1650, 1652, 1653 };
        private readonly List<uint> m_barricadeHeavies = new List<uint>() { 1588 };
        private readonly List<uint> m_barricadeElites = new List<uint>() { 1673, 1670 };
        private readonly List<uint> m_particlesMarcus = new List<uint>() { 535475, 535474, 535473 };
        private readonly List<uint> m_particlesApollo = new List<uint>() { 535468, 535469, 535190 };
                 
        private readonly List<uint> m_wavesApolloSpawns = new List<uint>() { 536782 };
        private readonly List<uint> m_wavesMarcusSpawns = new List<uint>() { 536780 };

        private readonly uint[] m_spawns = { 535965, 535921, 535966, 535967, 535968, 535970, 535969, 1650, 1652, 1653, 1588, 1673, 1670, 536782, 536780 };

        private readonly List<uint> m_particlesExplosion1 = new List<uint>() {536879, 536875, 536884};
        private readonly List<uint>	m_particlesExplosion2= new List<uint>() { 536878, 536876, 536881};
        private readonly List<uint>	m_particlesExplosion3= new List<uint>() { 537002, 537003, 537004};
        private readonly List<uint>	m_particlesExplosion4= new List<uint>() { 536880, 536882, 536883, 536877};
        private readonly List<uint>	m_particlesExplosion5= new List<uint>() { 536888, 536887, 536886, 536885, 536889};
        private readonly List<uint> m_particlesExplosion2_2 = new List<uint>() { 16779458, 16779459, 16779460, 16779500 };
        private readonly List<uint> m_particlesExplosion4_2 = new List<uint>() { 16779501, 16779504 };
        private readonly List<uint> m_particlesExplosion4_2_Destroy = new List<uint>() { 16779502, 16779503 };

        private Vector3 m_generatorPosition;

        private bool m_generatorAttacked = false;

        #endregion

        #region EntityIDs
        private enum EntityID // list of IDs used in sScript
        {
            FlyToManjeet = 156176,
            GoBackToMadelyn = 536812,
            DestroyGenerator = 534691,
            ReturnToMadelyn = 536854,
            WaypointFinalVitolinoRetreat = 536861,
            WaypointFinalMarcusRetreat = 536855,
            PlayerStart = 538082,
            MadelynStart = 538055,
            EACMothershipContainer1 = 50,
            EACMothershipContainer2 = 533595,
            WarSound = 1621,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)(((EntityID?) value).Value));
            }
        }
        #endregion

        public MyJunkyardEACAmbushMission()
        {
            ID = MyMissionID.JUNKYARD_EAC_AMBUSH; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("13-EAC ambush"); // Name of mission
            Name = MyTextsWrapperEnum.JUNKYARD_EAC_AMBUSH;
            Description = MyTextsWrapperEnum.JUNKYARD_EAC_AMBUSH_Description;
            Flags = MyMissionFlags.Story;
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };
            MyMwcVector3Int baseSector = new MyMwcVector3Int(2567538, 0, -172727); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.PlayerStart); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.RESEARCH_VESSEL }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN };

            Components.Add(new MySpawnpointLimiter(m_spawns, 7));

            #region Objectives
            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            var flyToManjeet = new MyMeetObjective(
                MyTextsWrapperEnum.EAC_AMBUSH_FLY_TO_MANJEET,
                MyMissionID.JUNKYARD_EAC_AMBUSH_FLY_MANJEET,
                MyTextsWrapperEnum.EAC_AMBUSH_FLY_TO_MANJEET_DESC,
                this,
                new MyMissionID[] { },
                null,
                (uint)EntityID.FlyToManjeet,
                100,
                0.25f,
                null,
                MyDialogueEnum.EAC_AMBUSH_0100_INTRO
                ) { SaveOnSuccess = false, FollowMe = false };
            flyToManjeet.OnMissionLoaded += FlyToManjeet_Loaded;
            m_objectives.Add(flyToManjeet);

            var speakToManjeet = new MyObjectiveDialog(
                MyMissionID.JUNKYARD_EAC_AMBUSH_TALK_RANIJT,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_FLY_MANJEET },
                MyDialogueEnum.EAC_AMBUSH_0200_MANJEET
                ) { SaveOnSuccess = false };
            speakToManjeet.OnMissionLoaded += SpeakToManjeet_Loaded;
            m_objectives.Add(speakToManjeet);

            m_backToMadelyn = new MyObjective(
                MyTextsWrapperEnum.EAC_AMBUSH_GO_BACK_TO_MADELYN,
                MyMissionID.JUNKYARD_EAC_AMBUSH_GO_BACK_TO_MADELYN,
                MyTextsWrapperEnum.EAC_AMBUSH_GO_BACK_TO_MADELYN_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_TALK_RANIJT },
                new MyMissionLocation(baseSector, (uint)EntityID.GoBackToMadelyn),
                startDialogId: MyDialogueEnum.EAC_AMBUSH_0300_GUYS_HURRY_UP
            ) { HudName = MyTextsWrapperEnum.Nothing };
            m_backToMadelyn.OnMissionLoaded += BackToMadelyn_Loaded;
            m_objectives.Add(m_backToMadelyn);

            m_speakWithPolice = new MyObjective(
                MyTextsWrapperEnum.Null,
                MyMissionID.JUNKYARD_EAC_AMBUSH_SPEAK_POLICE,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_GO_BACK_TO_MADELYN },
                null
            );
            m_speakWithPolice.OnMissionLoaded += SpeakWithPolice_Loaded;
            m_objectives.Add(m_speakWithPolice);

            m_defendMadelyn = new MyObjectiveDialog(
                MyTextsWrapperEnum.EAC_AMBUSH_DEFEND_MADELYN1,
                MyMissionID.JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN_1,
                MyTextsWrapperEnum.EAC_AMBUSH_DEFEND_MADELYN1_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_SPEAK_POLICE },
                MyDialogueEnum.EAC_AMBUSH_0500_ONE_LITTLE_ISSUE
                ) { SaveOnSuccess = true };
            m_defendMadelyn.OnMissionLoaded += DefendMadelyn_Loaded;
            m_objectives.Add(m_defendMadelyn);

            var destroyGenerator  = new MyObjectiveDestroy(
                MyTextsWrapperEnum.EAC_AMBUSH_DESTROY_GENERATOR,
                MyMissionID.JUNKYARD_EAC_AMBUSH_DESTROY_GENERATOR,
                MyTextsWrapperEnum.EAC_AMBUSH_DESTROY_GENERATOR_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN_1 },
                new List<uint> {  (uint)EntityID.DestroyGenerator}
                ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_AMBUSH_0700_SPLIT_TO_DESTROY_GENERATORS, HudName = MyTextsWrapperEnum.HudDisruptor };
            destroyGenerator.OnMissionLoaded += DestroyGenerator_Loaded;
            destroyGenerator.OnMissionSuccess += DestroyGenerator_Success;
            m_objectives.Add(destroyGenerator);

            var returnToMadelyn = new MyTimedReachLocationObjective(
                MyTextsWrapperEnum.EAC_AMBUSH_RETURN_TO_MADELYN,
                MyMissionID.JUNKYARD_EAC_AMBUSH_RETUR_TO_MADELYN,
                MyTextsWrapperEnum.EAC_AMBUSH_RETURN_TO_MADELYN_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_DESTROY_GENERATOR },
                new TimeSpan(0, 0, 45),
                new MyMissionLocation(baseSector, (uint)EntityID.ReturnToMadelyn)
                ) { SaveOnSuccess = true, StartDialogId = MyDialogueEnum.EAC_AMBUSH_1000, HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            returnToMadelyn.OnMissionLoaded += ReturnToMadelyn_Loaded;
            m_objectives.Add(returnToMadelyn);

            m_defendMadelyn2 = new MyObjective(
                MyTextsWrapperEnum.EAC_AMBUSH_DEFEND_MADELYN2,
                MyMissionID.JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN,
                MyTextsWrapperEnum.EAC_AMBUSH_DEFEND_MADELYN2_DESC,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH_RETUR_TO_MADELYN },
                null
            ) { StartDialogId = MyDialogueEnum.EAC_AMBUSH_1200_1300 };
            m_defendMadelyn2.OnMissionLoaded += DefendMadelyn2_Loaded;
            m_defendMadelyn2.OnMissionUpdate += DefendMadelyn2_Update;
            m_objectives.Add(m_defendMadelyn2);

            #endregion
        }

        void FlyToManjeet_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.MovePlayerAndFriendsToHangar(RequiredActors);
        }

        private void MyScriptWrapperOnFadedOut()
        {
            MyScriptWrapper.FadedOut -= MyScriptWrapperOnFadedOut;

            MyScriptWrapper.FadeIn();

            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.SwitchInHUBTurrets, MyGuiManager.GetFontMinerWarsGreen(), 60000,
                new object[] {
                                MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_LEFT),
                                MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_RIGHT)
                            }
            ));

            MyScriptWrapper.TakeControlOfLargeWeapon(m_madelynTurrets[m_activeTurret]);
            MyScriptWrapper.ForbideDetaching();

            MyScriptWrapper.SwitchTowerPrevious += MyScriptWrapper_SwitchTowerPrevious;
            MyScriptWrapper.SwitchTowerNext += MyScriptWrapper_SwitchTowerNext;

        }

        private void MyScriptWrapper_SwitchTowerNext()
        {
            m_activeTurret--;
            m_activeTurret = Mod(m_activeTurret, m_madelynTurrets.Length);
            MyScriptWrapper.TakeControlOfLargeWeapon(m_madelynTurrets[m_activeTurret]);
        }


        private void MyScriptWrapper_SwitchTowerPrevious()
        {
            m_activeTurret++;
            m_activeTurret = Mod(m_activeTurret, m_madelynTurrets.Length);
            MyScriptWrapper.TakeControlOfLargeWeapon(m_madelynTurrets[m_activeTurret]);
        }

        private static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }


        private void DefendMadelyn_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(100);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 3, "KA03");

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_WORST);

            var bots = GetBotsFromSpawnpoints(m_barricadeSpawns);
            foreach (var mySmallShipBot in bots)
            {
                if (mySmallShipBot != null)
                {
                    mySmallShipBot.LookTarget = null;
                    MyScriptWrapper.SetEntityDestructible(mySmallShipBot, true);
                }
            }
            var looms = GetBotsFromSpawnpoints(m_barricadeLoomers);
            foreach (var mySmallShipBot in looms)
            {
                if (mySmallShipBot != null)
                {
                    mySmallShipBot.LookTarget = null;
                    MyScriptWrapper.SetEntityDestructible(mySmallShipBot, true);
                }
            }
            MyScriptWrapper.ActivateSpawnPoints(m_barricadeElites);

            ((MyInfluenceSphere)MyScriptWrapper.GetEntity((uint)EntityID.WarSound)).Enabled = true;
        }

        private void SpeakWithPolice_Loaded(MyMissionBase sender)
        {
            if (m_speakWithPoliceDialogueFinished)
            {
                sender.Success();
            }
        }

        private void SpeakToManjeet_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, MyFactions.RELATION_NEUTRAL);  // The police won't attack you at first.
            MyScriptWrapper.TryUnhide((uint)EntityID.EACMothershipContainer2, destroyGeneratedWaypointEdges: true);
            MyScriptWrapper.TryUnhide((uint)EntityID.EACMothershipContainer1, destroyGeneratedWaypointEdges: true);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.EACMothershipContainer1), -1, true);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.EACMothershipContainer2), -1, true);
        }

        public override void Update()
        {
            base.Update();

            if (m_desperationFadeout)
            {
                // fade out sounds
                float level = Math.Max(-1f, -1f * (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_desperationFadeoutStarted) / 10000.0f); // it takes 60s for desperation to sink in
                MyScriptWrapper.SetGameVolumeExceptDialogues((float)Math.Pow(10, level));  // logarithmic fadeout
            }
        }

        private void DefendMadelyn2_Update(MyMissionBase sender)
        {
            if (m_madelynMoving2)
            {
                var speed = Math.Min(100, 15 + (100 - 15) * (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_madelynMovingStarted2) / 20000.0f);  // it takes 20s for Madelyn to reach max speed after Marcus crashes

                Vector3 velocity = speed * m_madelyn.WorldMatrix.Forward; // Speed in direction
                MyScriptWrapper.Move(m_madelyn, m_madelyn.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            }
            else if (m_madelynMoving)
            {
                var speed = Math.Min(15, 15 * (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_madelynMovingStarted) / 10000.0f);  // it takes 10s for Madelyn to reach max speed of 15 (jammed)

                Vector3 velocity = speed * m_madelyn.WorldMatrix.Forward; // Speed in direction
                MyScriptWrapper.Move(m_madelyn, m_madelyn.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            }
        }

        private void DefendMadelyn2_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.FadedOut += MyScriptWrapperOnFadedOut;
            MyScriptWrapper.FadeOut();

            MyScriptWrapper.SetEntityPriority(m_madelyn, -1, true);
            MyScriptWrapper.HideEntity(MySession.PlayerShip);

            MyScriptWrapper.Refill();

            EveryoneAttackMadelyn();
        }

        private void ReturnToMadelyn_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3, "KA19");
            MyScriptWrapper.SetEntitiesEnabled(m_particlesApollo, false);
            MyScriptWrapper.ActivateSpawnPoints(m_barricadeHeavies);

            EveryoneAttackMadelyn();
        }

        private void DestroyGenerator_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoints(m_wavesApolloSpawns);
            MyScriptWrapper.ActivateSpawnPoints(m_wavesMarcusSpawns);

            m_marcus.StopFollow();
            m_ravenGuy.StopFollow();

            m_marcus.SetWaypointPath("MarcusLeaved");
            m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
            m_marcus.SeeDistance = 250f;
            m_marcus.Patrol();

            m_ravenGuy.SetWaypointPath("VitolinoLeaved");
            m_ravenGuy.SeeDistance = 250f;
            m_ravenGuy.PatrolMode = MyPatrolMode.ONE_WAY;
            m_ravenGuy.Patrol();

            m_generatorPosition = MyScriptWrapper.GetEntity((uint)EntityID.DestroyGenerator).GetPosition();
            MyScriptWrapper.ActivateSpawnPoints(m_barricadeLoomers);

            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.MarcusAndValentinoLeavingParty, MyGuiManager.GetFontMinerWarsGreen(), 10000));
            EveryoneAttackMadelyn();
        }

        private void EveryoneAttackMadelyn()
        {
            foreach (var bot in GetBotsFromSpawnpoints(m_spawns)) if (bot != null)
            {
                bot.Attack(MyScriptWrapper.GetEntity("Madelyn"));
            }
        }

        private void DestroyGenerator_Success(MyMissionBase sender)
        {
            MyScriptWrapper.PlaySound3D(m_generatorPosition, MySoundCuesEnum.WepBombExplosion);
        }

        private List<MySmallShipBot> GetBotsFromSpawnpoints(IEnumerable<uint> spawnPoints)
        {
            var result = new List<MySmallShipBot>();
            foreach (var spawnPoint in spawnPoints)
            {
                var bots = MyScriptWrapper.GetSpawnPointBots(spawnPoint);
                foreach (var bot in bots)
                {
                    result.Add(bot.Ship);
                }
            }
            return result;
        }

        private void BackToMadelyn_Loaded(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 3, "KA02");
            MyScriptWrapper.ActivateSpawnPoints(m_barricadeSpawns);

            foreach (var particle in m_particlesMarcus)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }
            foreach (var particle in m_particlesApollo)
            {
                MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity(particle), true);
            }
            MyScriptWrapper.SetEntitiesEnabled(m_particlesMarcus, true);
            MyScriptWrapper.SetEntitiesEnabled(m_particlesApollo, true);     
        }

        public override void Load()
        {
            //Because he has different position from junkyard race
            MyEntity manjeet = MyScriptWrapper.GetEntity((int)EntityID.FlyToManjeet);
            manjeet.SetPosition(new Vector3(-7529.0f, -3368.0f, 5938.0f));

            m_marcusForCrashDialoguePlayed = false;

            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnBotReachedWaypoint += MyScriptWrapperOnOnBotReachedWaypoint;
            MyScriptWrapper.OnDialogueFinished += MyScriptWrapperOnOnDialogueFinished;
            MyScriptWrapper.OnEntityAtacked += MyScriptWrapperOnOnEntityAtacked;

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 0, "KA01");
            MyScriptWrapper.OnSentenceStarted += MyScriptWrapper_OnSentenceStarted;

            m_speakWithPoliceDialogueFinished = false;

            m_ravenGuy = (MySmallShipBot)MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.VALENTIN));
            m_ravenGuy.SleepDistance = 8000;

            m_gameVolume = MyScriptWrapper.GetGameVolume();

            m_ravenGirl = (MySmallShipBot)MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.TARJA));
            m_ravenGirl.SleepDistance = 8000;

            m_marcus = (MySmallShipBot)MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MARCUS));
            m_marcus.SleepDistance = 8000;

            m_madelyn = MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MADELYN));
            MyScriptWrapper.SetEntityDestructible(m_madelyn, false);
            m_madelyn.SetWorldMatrix(MyScriptWrapper.GetEntity((uint)EntityID.MadelynStart).WorldMatrix);

            m_madelynTurrets = new MyPrefabLargeWeapon[3];
            m_madelynTurrets[0] = (MyPrefabLargeWeapon)MyScriptWrapper.GetEntity("BackTurretM");
            m_madelynTurrets[1] = (MyPrefabLargeWeapon)MyScriptWrapper.GetEntity("FrontTurretM");
            m_madelynTurrets[2] = (MyPrefabLargeWeapon)MyScriptWrapper.GetEntity("BottomTurretM");

            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint) EntityID.EACMothershipContainer1), -1, true);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint) EntityID.EACMothershipContainer2), -1, true);

            m_ravenGuyIn = false;
            m_madelynMoving = false;
            m_madelynMoving2 = false;
            m_desperationFadeout = false;

            base.Load();
        }

        void MyScriptWrapper_OnSentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (dialogue == MyDialogueEnum.EAC_AMBUSH_0400_MARCUS_TO_EAC && sentence == MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0407)
            {
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 3, "KA16");
            }
        }

        private void MyScriptWrapperOnOnEntityAtacked(MyEntity attacker, MyEntity target)
        {
            if (target.EntityId != null && (attacker == MySession.PlayerShip && target.EntityId.Value.NumericValue == (uint)EntityID.DestroyGenerator) && !m_generatorAttacked)
            {             
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_0800);
                m_generatorAttacked = true;
            }
        }

        private void MyScriptWrapperOnOnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            switch (dialogue)
            {
                case MyDialogueEnum.EAC_AMBUSH_0300_GUYS_HURRY_UP:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_0400_MARCUS_TO_EAC);
                    break;

                case MyDialogueEnum.EAC_AMBUSH_0400_MARCUS_TO_EAC:
                    if (m_backToMadelyn.IsAvailable())
                    {
                        m_backToMadelyn.Success();
                    }
                    if (m_speakWithPolice.IsAvailable())
                    {
                        m_speakWithPolice.Success();
                    }
                    MyScriptWrapper.ActivateSpawnPoints(m_barricadeSpawns);
                    m_speakWithPoliceDialogueFinished = true;
                    break;

                case MyDialogueEnum.EAC_AMBUSH_1200_1300:
                    MyScriptWrapper.HideEntity(m_ravenGirl);
                    MyScriptWrapper.RemoveEntityMark(m_ravenGirl);

                    MyScriptWrapper.SetEntitiesEnabled(m_particlesMarcus, false);

                    m_marcus.AITemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.PASSIVE);
                    m_marcus.SetWaypointPath("MarcusRetreat");
                    m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
                    m_marcus.Patrol();

                    m_ravenGuy.AITemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.PASSIVE);
                    m_ravenGuy.SetWaypointPath("VitolinoRetreat");
                    m_ravenGuy.PatrolMode = MyPatrolMode.ONE_WAY;
                    m_ravenGuy.Patrol();

                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_1500);
                    MissionTimer.RegisterTimerAction(30000, MarcusForCrashDialogue, false);
                    break;

                case MyDialogueEnum.EAC_AMBUSH_1500:
                    //MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 3, "LostInTheDistance");
                    break;
                case MyDialogueEnum.EAC_AMBUSH_1600:
                    //MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.SadnessOrDesperation, 3, "KA02"); //TODO: change to 
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_1650);
                    MarcusForCrash();
                    break;
                case MyDialogueEnum.EAC_AMBUSH_1650:
                    Boom();
                    break;
            }
        }


        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.FadedOut -= MyScriptWrapperOnFadedOut;
            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapperOnOnSpawnpointBotSpawned;
            MyScriptWrapper.OnBotReachedWaypoint -= MyScriptWrapperOnOnBotReachedWaypoint;
            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapperOnOnDialogueFinished;
            MyScriptWrapper.OnEntityAtacked -= MyScriptWrapperOnOnEntityAtacked;
            MyScriptWrapper.OnSentenceStarted -= MyScriptWrapper_OnSentenceStarted;

            MyScriptWrapper.SetEntitySaveFlagDisabled(MyScriptWrapper.GetEntity((uint)EntityID.EACMothershipContainer1));
            MyScriptWrapper.SetEntitySaveFlagDisabled(MyScriptWrapper.GetEntity((uint)EntityID.EACMothershipContainer2));

            MyScriptWrapper.SetGameVolume(m_gameVolume);

            m_ravenGuy = null;
            m_ravenGirl = null;
            m_marcus = null;
            m_madelyn = null;
            m_madelynTurrets = null;

            MyScriptWrapper.EnableDetaching();
            MyScriptWrapper.SwitchTowerPrevious -= MyScriptWrapper_SwitchTowerPrevious;
            MyScriptWrapper.SwitchTowerNext -= MyScriptWrapper_SwitchTowerNext;

            if (MyScriptWrapper.IsMissionFinished(this.ID))
            {
                MyScriptWrapper.TravelToMission(MyMissionID.RIFT);
            }
        }

        private bool m_ravenGuyIn = false;
        private bool m_marcusIn = false;
        private bool m_madelynMoving = false;
        private int m_madelynMovingStarted = 0;
        private bool m_madelynMoving2 = false;
        private int m_madelynMovingStarted2 = 0;
        private bool m_desperationFadeout = false;
        private int m_desperationFadeoutStarted = 0;


        private void MyScriptWrapperOnOnBotReachedWaypoint(MyEntity bot , MyEntity waypoint)
        {
            if (bot == m_ravenGuy)
            {
                if (waypoint.EntityId.Value.NumericValue == (uint)EntityID.WaypointFinalVitolinoRetreat && m_defendMadelyn2.IsAvailable())
                {
                    MyScriptWrapper.HideEntity(m_ravenGuy);
                    MyScriptWrapper.RemoveEntityMark(m_ravenGuy);
                    m_ravenGuyIn = true;
                    if (m_marcusIn)
                    {
                        MarcusForCrashDialogue();
                    }
                }
            }

            if (bot == m_marcus && waypoint.EntityId.Value.NumericValue == (uint)EntityID.WaypointFinalMarcusRetreat && m_defendMadelyn2.IsAvailable())
            {
                m_marcusIn = true;
                if (m_ravenGuyIn)
                {
                    MarcusForCrashDialogue();
                }
            }
        }

        private void MarcusForCrashDialogue()
        {
            if (!m_marcusForCrashDialoguePlayed)
            {
                m_marcusForCrashDialoguePlayed = true;
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_1600);

                //Here Marek wants to lower nondialogue sounds and start sad music
                m_desperationFadeout = true;
                m_desperationFadeoutStarted = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "LostInTheDistance");

                StartMovingMadelyn1();
            }
        }

        private void StartMovingMadelyn1()
        {
            MyScriptWrapper.EnablePhysics(m_madelyn.EntityId.Value.NumericValue, false);
            m_madelynMoving = true;
            m_madelynMovingStarted = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        private void StartMovingMadelyn2()
        {
            m_madelynMoving2 = true;
            m_madelynMovingStarted2 = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        private void MarcusForCrash()
        {
            MyScriptWrapper.DeactivateSpawnPoints(m_wavesApolloSpawns);

            m_marcus.AITemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.PASSIVE);
            m_marcus.SetWaypointPath("MarcusCrash");
            m_marcus.PatrolMode = MyPatrolMode.ONE_WAY;
            m_marcus.Patrol();

           // MyScriptWrapper.MarkEntity(m_marcus, m_marcus.DisplayName, HUD.MyHudIndicatorFlagsEnum.SHOW_ALL);
        }

  
        private void Boom()
        {
            StartMovingMadelyn2();
            MyScriptWrapper.HideEntity(m_marcus);
            MyScriptWrapper.RemoveEntityMark(m_marcus);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.EAC_AMBUSH_1700);
            MyScriptWrapper.DeactivateSpawnPoints(m_wavesMarcusSpawns);

            MyScriptWrapper.AddExplosions(m_particlesExplosion1, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 100, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);

            MissionTimer.RegisterTimerAction(3000, Boom1, false);
            MissionTimer.RegisterTimerAction(4000, Boom2, false);
            MissionTimer.RegisterTimerAction(4500, Boom2_2, false);
            MissionTimer.RegisterTimerAction(5000, Boom3, false);
            MissionTimer.RegisterTimerAction(5500, Boom4, false);
            MissionTimer.RegisterTimerAction(7000, Boom4_2, false);
            MissionTimer.RegisterTimerAction(30000, EndGame, false);
        }

        private void Boom1()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion2, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 800, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
        }

        private void Boom2()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion3, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 800, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
        }

        private void Boom2_2()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion2_2, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 800, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
        }

        private void Boom3()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion4, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 800, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
        }

        private void Boom4()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion5, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 800, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
        }

        private void Boom4_2()
        {
            MyScriptWrapper.AddExplosions(m_particlesExplosion4_2, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 18000, 1000, particleIDOverride: MyParticleEffectsIDEnum.Explosion_Medium);
            MyScriptWrapper.AddExplosions(m_particlesExplosion4_2_Destroy, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 100000, 0, false, false, MyParticleEffectsIDEnum.Explosion_Huge);
        }

        private void EndGame()
        {   
            m_defendMadelyn2.Success();
        }

        private void MyScriptWrapperOnOnSpawnpointBotSpawned(MyEntity entity1, MyEntity entity2)
        {
            if (m_barricadeSpawns.Contains(entity1.EntityId.Value.NumericValue) && !MyScriptWrapper.IsMissionFinished(MyMissionID.JUNKYARD_EAC_AMBUSH_SPEAK_POLICE))
            {
                var bot = (MySmallShipBot)entity2;
                bot.LookTarget = MySession.PlayerShip;
                MyScriptWrapper.SetEntityDestructible(bot, false);
            }
            if (m_barricadeLoomers.Contains(entity1.EntityId.Value.NumericValue) && !MyScriptWrapper.IsMissionFinished(MyMissionID.JUNKYARD_EAC_AMBUSH_SPEAK_POLICE))
            {
                var bot = (MySmallShipBot)entity2;
                bot.LookTarget = MySession.PlayerShip;
                MyScriptWrapper.SetEntityDestructible(bot, false);
            }

            if (m_barricadeHeavies.Contains(entity1.EntityId.Value.NumericValue))
            {
                var bot = (MySmallShipBot)entity2;
                bot.Attack(MyScriptWrapper.GetEntity("Madelyn"));  // hope they won't consider the player to be a better target :)
            }

            if (m_barricadeElites.Contains(entity1.EntityId.Value.NumericValue))
            {
                var bot = (MySmallShipBot)entity2;
                bot.Attack(MyScriptWrapper.GetEntity("Madelyn"));  // hope they won't consider the player to be a better target :)
            }

            if (m_wavesApolloSpawns.Contains(entity1.EntityId.Value.NumericValue) || m_wavesMarcusSpawns.Contains(entity1.EntityId.Value.NumericValue))
            {
                var bot = (MySmallShipBot)entity2;
                bot.SleepDistance = 5000;
            }
        }

        public override void Success()
        {
            MyScriptWrapper.UnhideEntity(MySession.PlayerShip, false);

            //We dont want him in Rift
            MyScriptWrapper.CloseEntity(m_marcus);

            m_ravenGuy.AITemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.DEFAULT);

            base.Success();
        }
    }

}

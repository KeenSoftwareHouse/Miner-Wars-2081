using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyJunkyardReturnMission: MyMission
    {
        private MyObjective m_objective03_FlyToStart;
        private MyObjectiveRace m_objective04_Race;
        private MyObjectiveDialog m_objective05_Win;
        private MyObjective m_objective06_GoToSmuggler;
        private MyObjectiveDialog m_objective07_SmugglerDialogueReturn;
        //private MyObjective m_objective08_FlyToMS;

        private MyEntityDetector m_detector_ReachStart;
        //private MyEntityDetector m_detector_SmugglerFirst;
        //private MyEntityDetector m_detector_SmugglerReturn;
        private MyEntityDetector m_detector_Explosion1;
        private MyEntityDetector m_detector_Explosion2;
        private MyEntityDetector m_detector_Explosion3;
        private MyEntityDetector m_detector_TunnelExplosion;

        private bool m_playerIsOnStart;
        private bool m_challengerIsOnStart;
        private bool m_unfollowedCompanions;

        private MySmallShipBot m_manjeet;
        private MyEntity m_marcus;
        private MyEntity m_tarja;
        private MyEntity m_vitolino;

        #region EntityIDs
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 131399,
            SmugglerStartPosition = 404558,
            DetectorStartRacePosition = 418116,
            DetectorStartRacePosition2 = 407419,
            DetectorExplosion1 = 419491,
            DetectorExplosion2 = 419496,
            DetectorExplosion3 = 419498,
            DetectorTunnelExplosion = 433532,
            ParticleExplosionInTunnel = 433564,
            Challenger = 375502,
            Manjeet = 156176,
            ChallengerStartPosWaypoint = 404592,
            LookAtDummy = 404602,
            Transporter = 418656,
            Racer2 = 529318,
            Racer3 = 529317,
            ComentatorDeetctor = 869
        }

        private readonly List<uint> m_minesDummies = new List<uint>() { 
            406486, 16785233, 16785234, 16785235, 406485, 16785236, 16785241 ,406483, 16785240, 16785239, 16785238, 406484, 16785237, 406487, 406488
        }; 

        private readonly List<uint> m_companionSpawnpoints = new List<uint>
        {
            404573, 404572, 404569, 404571
        };

        private readonly List<uint> m_raceCheckpoints = new List<uint>
        {
            404103, //0
            404104, //1
            404105, //2
            404106, //3
            404107, //4
            404108, //5
            404109, //6
            404110, //7
            404111, //8
            404112, //9
            404113, //10
            404114, //11
            404115, //12
            404116, //13
            404117, //14
            404118, //15
            404119, //16
            404120, //17
            404121  //18
        };

        private readonly List<uint> m_greenLights = new List<uint>
        {
            407900, 407899, 407886, 407884
        };

        private readonly List<uint> m_yellowLights = new List<uint>
        {
           407898, 407897, 407891, 407892
        };

        private readonly List<uint> m_redLights = new List<uint>
        {
           407894, 407893, 407896, 407895
        };

        private readonly List<uint> m_explosionParticles1 = new List<uint>
        {
            419488, 419489, 419490
        };

        private readonly List<uint> m_explosionParticles2 = new List<uint>
        {
            419495, 419494, 419493
        };

        private readonly List<uint> m_explosionParticles3 = new List<uint>
        {
            419500, 419502, 419503, 419501
        };

        private readonly List<uint> m_raceFansSP1 = new List<uint>
        {
            419482, 419483, 419484, 419485, 419486, 419487
        };

        private readonly List<uint> m_raceFansSP2 = new List<uint>
        {
            419507, 419504, 419506, 419505
        };

        private readonly List<uint> m_raceFansSP3 = new List<uint>
        {
            419510, 419512, 419513, 419511
        };

        private readonly List<uint> m_raceFansSP4 = new List<uint>
        {
            433566, 433565, 433568, 433567
        };

        private readonly List<uint> m_raceFansSP5 = new List<uint>
        {
            433571, 433569, 433570, 433572
        };

        private readonly List<List<uint>> m_raceGates = new List<List<uint>> 
        {
            new List<uint> {290478, 290477}, new List<uint> {306930, 306929}, new List<uint> {365999, 365998}, new List<uint> {366019, 366020}, new List<uint> {306971, 306972},
            new List<uint> {306999, 307000}, new List<uint> {307020, 307021}, new List<uint> {527748, 366105}, new List<uint> {366127, 366126}, new List<uint> {366147, 366148},
            new List<uint> {366211, 366210}, new List<uint> {366231, 366232}, new List<uint> {366253, 366252}, new List<uint> {366273, 366274}, new List<uint> {366298, 366299},
            new List<uint> {375462, 375463}, new List<uint> {375484, 375483}, new List<uint> {366319, 366320}, new List<uint> {366361, 376421}
        };

        private readonly List<List<uint>> m_raceTowers = new List<List<uint>> 
        {
            null, //0
            new List<uint> {516356, 516353}, //1
            null, //2
            null, //3
            new List<uint> {527736, 527739}, //4
            null, //5
            new List<uint> {527742, 527745}, //6
            null, //7
            null, //8
            null, //9
            null, //10
            new List<uint> {527749, 527752}, //11
            null, //12
            null, //13
            null, //14
            new List<uint> {527755, 527758}, //15
            null, //16
            null, //17
            null  //18
        };

        private readonly List<uint> m_influenceSpheresFansBeforeStart = new List<uint>() {895, 897};
        private readonly List<uint> m_influenceSpheresFansAfterStart = new List<uint>() { 905, 907, 909, 911, 920, 921, 922, 924, 925, 926, 941, 942, 943, 946, 947, 948, 950, 952, 955, 957, 958, 960, };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            if (!IsMainSector) return;

            foreach (EntityID value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)value);
            }


            foreach (var item in new List<List<uint>> {
                m_explosionParticles1, m_explosionParticles2, m_explosionParticles3, m_raceFansSP1, m_raceFansSP2, m_raceFansSP3, m_raceFansSP4, m_raceFansSP5,
                m_minesDummies, m_redLights, m_yellowLights, m_companionSpawnpoints, m_influenceSpheresFansBeforeStart})
            {
                foreach (var item2 in item)
                {
                    MyScriptWrapper.GetEntity(item2);
                }
            }
            foreach (var item in m_raceGates)
            {
                foreach (var item2 in item)
                {
                    MyScriptWrapper.GetEntity(item2);
                }
            }
            foreach (var item in m_raceTowers)
            {
                if (item != null)
                {
                    foreach (var item2 in item)
                    {
                        MyScriptWrapper.GetEntity(item2);
                    }
                }
            }
        }
        #endregion

        public MyJunkyardReturnMission()
        {
            ID = MyMissionID.JUNKYARD_RETURN; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.JUNKYARD_RETURN;
            Description = MyTextsWrapperEnum.JUNKYARD_RETURN_Description; //"Return to Ranjit with stolen Cargoship\n"
            DebugName = new StringBuilder("08e-Junkyard racing"); // Name of mission
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission12_JunkyardRacing;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2567538,0,-172727); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.CHINESE_ESCAPE }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_DIALOGUE_RETURN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            #region Objectives
            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            var meetManjeet = new MyObjective(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_MEET_SMUGGLER_Name),
                MyMissionID.JUNKYARD_RETURN_MEET_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_MEET_SMUGGLER_Description),
                null,
                this,
                new MyMissionID[] {},
                new MyMissionLocation(baseSector, (uint) EntityID.Manjeet),
                new List<uint> {(uint) EntityID.Manjeet},
                radiusOverride: 50
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudManjeet };
            m_objectives.Add(meetManjeet);
            meetManjeet.OnMissionLoaded += O01MeetSmugglerLoaded;


            var manjeetDialogue = new MyObjectiveDialog(
                MyMissionID.JUNKYARD_RETURN_SMUGGLER_DIALOGUE,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_MEET_SMUGGLER },
                MyDialogueEnum.JUNKYARD_RETURN_0200
                ) { SaveOnSuccess = true };
            manjeetDialogue.OnMissionSuccess += new MissionHandler(manjeetDialogue_OnMissionSuccess);
            m_objectives.Add(manjeetDialogue);

            var speedsterDialogue = new MyObjective(
                MyTextsWrapperEnum.Null,
                MyMissionID.JUNKYARD_RETURN_SPEEDSTER_DIALOGUE,
                MyTextsWrapperEnum.Null,
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_SMUGGLER_DIALOGUE },
                null
                ) { SaveOnSuccess = true };
            speedsterDialogue.OnMissionLoaded += O02SmugglerDialogueLoaded;
            m_objectives.Add(speedsterDialogue);

            m_objective03_FlyToStart = new MyObjective(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_FLY_TO_START_Name),
                MyMissionID.JUNKYARD_RETURN_FLY_TO_START,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_FLY_TO_START_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_SPEEDSTER_DIALOGUE },
                null,
                startDialogId: MyDialogueEnum.JUNKYARD_RETURN_0300
            ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudStart };
            m_objective03_FlyToStart.MissionEntityIDs.Add((uint)EntityID.DetectorStartRacePosition);
            m_objective03_FlyToStart.OnMissionLoaded += O03FlyToStartLoaded;
            m_objective03_FlyToStart.OnMissionCleanUp += O03FlyToStartCleanUp;
            m_objectives.Add(m_objective03_FlyToStart);


            m_objective04_Race = new MyObjectiveRace(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_RACE_Name),
                (MyTextsWrapperEnum.JUNKYARD_RETURN_RACE_PrepareName),
                (MyTextsWrapperEnum.JUNKYARD_RETURN_RACE_GoBackName),
                MyMissionID.JUNKYARD_RETURN_RACE,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_RACE_Description),
                null,
                this,
                new MyMissionID[]{ MyMissionID.JUNKYARD_RETURN_FLY_TO_START },
                baseSector,
                m_raceCheckpoints,
                (uint)EntityID.DetectorStartRacePosition2
            ) { SaveOnSuccess = true} ;
            m_objective04_Race.RacerNames = new List<string> { "Racer2", "Racer3", "Challenger" };
            m_objective04_Race.BotLoaded += Race_BotLoaded;
            m_objective04_Race.OnMissionLoaded += O04RaceLoaded;
            m_objective04_Race.OnMissionFailed += RaceMissionFailed;
            m_objective04_Race.OnMissionSuccess += Objective04_RaceOnOnMissionSuccess;
            m_objective04_Race.CheckpointReached += RaceMissonOnCheckpointReached;
            m_objective04_Race.OnStartNumberChanged += RaceStartNumberChanged;
            m_objective04_Race.RaceStarted = RaceStarted;
            m_objectives.Add(m_objective04_Race);

            m_objective05_Win = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_WIN_Name),
                MyMissionID.JUNKYARD_RETURN_WIN,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_WIN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_RACE },
                MyDialogueEnum.JUNKYARD_RETURN_1300
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective05_Win);
            m_objective05_Win.OnMissionLoaded += O05WinLoaded;


            m_objective06_GoToSmuggler = new MyMeetObjective(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_GO_TO_SMUGGLER_name),
                MyMissionID.JUNKYARD_RETURN_GO_TO_SMUGGLER,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_GO_TO_SMUGGLER_Description),
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_WIN },
                null,
                (uint)EntityID.Manjeet,
                100,
                0.25f,
                null
                ) { SaveOnSuccess = true, FollowMe = false };
            m_objectives.Add(m_objective06_GoToSmuggler);


            m_objective07_SmugglerDialogueReturn = new MyObjectiveDialog(
                (MyTextsWrapperEnum.JUNKYARD_RETURN_DIALOGUE_RETURN_Name),
                MyMissionID.JUNKYARD_RETURN_DIALOGUE_RETURN,
                (MyTextsWrapperEnum.JUNKYARD_RETURN_DIALOGUE_RETURN_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.JUNKYARD_RETURN_GO_TO_SMUGGLER},
                dialogId: MyDialogueEnum.JUNKYARD_RETURN_1400
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_objective07_SmugglerDialogueReturn);

            m_objective07_SmugglerDialogueReturn.OnMissionLoaded += O07SmugglerDialogueReturnLoaded;

            #endregion
        }

        void manjeetDialogue_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Challenger);
        }


        private void Objective04_RaceOnOnMissionSuccess(MyMissionBase sender)
        {
            this.MissionTimer.RegisterTimerAction(TimeSpan.FromSeconds(5), () => {
                                                                                   MyScriptWrapper.SetEntitiesEnabled(m_influenceSpheresFansAfterStart,false);
                                                                                   MyScriptWrapper.SetEntitiesEnabled(m_influenceSpheresFansBeforeStart,false);
                                                                                 }, false);
        }

        private void RaceStarted()
        {
            MySmallShipBot challenger = (MySmallShipBot)MyScriptWrapper.GetEntity("Challenger");
            challenger.LookTarget = null;

            MySmallShipBot racer3 = (MySmallShipBot)MyScriptWrapper.GetEntity("Racer3");
            racer3.LookTarget = null;

            MySmallShipBot racer2 = (MySmallShipBot)MyScriptWrapper.GetEntity("Racer2");
            racer2.LookTarget = null;
        }

        public override void Load()
        {
            if (!IsMainSector) return;

            m_detector_ReachStart = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorStartRacePosition));
            m_detector_Explosion1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorExplosion1));

            m_detector_Explosion2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorExplosion2));
            m_detector_Explosion3 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorExplosion3));
            m_detector_TunnelExplosion = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.DetectorTunnelExplosion));            

            m_marcus = MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.MARCUS));
            m_tarja = MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.TARJA));
            m_vitolino = MyScriptWrapper.GetEntity(MyActorConstants.GetActorName(MyActorEnum.VALENTIN));
            m_manjeet = (MySmallShipBot)MyScriptWrapper.GetEntity((uint)EntityID.Manjeet);

            MyScriptWrapper.OnDialogueFinished += Script_DialogueFinished;
            MyScriptWrapper.OnSpawnpointBotSpawned += Script_BotSpawned;  

            ShutDownAllLights();

            foreach (var minesDummy in m_minesDummies)
            {
                MyScriptWrapper.GenerateMinesField<MyMineSmart>(MyScriptWrapper.GetEntity(minesDummy),
                                                                MyMwcObjectBuilder_FactionEnum.Russian, 4,
                                                                MyTextsWrapper.Get(MyTextsWrapperEnum.MineBasicHud).ToString(),
                                                                MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS |
                                                                MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE |
                                                                MyHudIndicatorFlagsEnum.SHOW_TEXT |
                                                                MyHudIndicatorFlagsEnum.SHOW_DISTANCE |
                                                                MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER |
                                                                MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR);
            }

            m_playerIsOnStart = false;
            m_challengerIsOnStart = false;
            m_unfollowedCompanions = false;

            foreach (var item in m_raceTowers)
            {
                if (item != null)
                {
                    foreach (var item2 in item)
                    {
                        MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(item2));
                    }
                }
            }
            
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.Transporter));
            
            base.Load();
        }

        void Script_DialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            switch (dialogue)
            {
                case MyDialogueEnum.JUNKYARD_RETURN_0250:
                    MyMissions.GetMissionByID(MyMissionID.JUNKYARD_RETURN_SPEEDSTER_DIALOGUE).Success();
                    break;
            }
        }

        public override void Unload()
        {
            if (!IsMainSector) return;

            m_playerIsOnStart = false;
            m_challengerIsOnStart = false;

            MyScriptWrapper.OnDialogueFinished -= Script_DialogueFinished;
            MyScriptWrapper.OnSpawnpointBotSpawned -= Script_BotSpawned;  


            MyScriptWrapper.SetEntitySaveFlagDisabled(MyScriptWrapper.GetEntity((uint)EntityID.Transporter));
            foreach (var item in m_raceTowers)
            {
                if (item != null)
                {
                    foreach (var item2 in item)
                    {
                        MyEntity tower = MyScriptWrapper.TryGetEntity(item2);
                        if (tower != null)
                        {
                            MyScriptWrapper.SetEntitySaveFlagDisabled(tower);
                        }
                    }
                }
            }

            m_detector_ReachStart = null;
            m_detector_Explosion1 = null;
            m_detector_Explosion2 = null;
            m_detector_Explosion3 = null;
            m_detector_TunnelExplosion = null;
    
            m_manjeet = null;
            m_marcus = null;
            m_tarja = null;
            m_vitolino = null;

            base.Unload();
        }

        public override void Update()
        {
            if (!IsMainSector) return;

            base.Update();

            if (m_playerIsOnStart && m_challengerIsOnStart && m_objective03_FlyToStart.IsAvailable())
            {
                m_objective03_FlyToStart.Success();
                m_objective03_FlyToStart.MissionEntityIDs.Remove((uint) EntityID.DetectorStartRacePosition);
            }

        }

        void O01MeetSmugglerLoaded(MyMissionBase sender)
        {
            //Because it sometimes, no one knows why, get broken when leaving chinese escape
            MyScriptWrapper.UnhideEntity(MySession.PlayerShip);
            MyScriptWrapper.EnableDetaching();
            MySession.PlayerShip.Activate(true, false);



            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 0, "KA01");
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0100);

            var manjeetPosition = MyScriptWrapper.GetEntity((uint) EntityID.SmugglerStartPosition);

            MyScriptWrapper.Move(m_manjeet, manjeetPosition.GetPosition(), manjeetPosition.GetForward(), manjeetPosition.GetUp());            
        }

        void O02SmugglerDialogueLoaded(MyMissionBase sender)
        {
            foreach (var item in m_companionSpawnpoints)
            {
                MyScriptWrapper.ActivateSpawnPoint(item);
            }
        }

        void O03FlyToStartLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.OnBotReachedWaypoint += Script_BotReachedWaypoint;

            m_detector_ReachStart.OnEntityEnter += StartPositionEntered;
            m_detector_ReachStart.OnEntityLeave += StartPositionLeaved;
            m_detector_ReachStart.On();

            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Racer2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Racer3);
            MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.DetectorStartRacePosition), MyTexts.FlyToStartingPoint, HUD.MyHudIndicatorFlagsEnum.SHOW_TEXT | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS);

            m_manjeet.LookTarget = null;
            m_manjeet.SetWaypointPath("SmugglerOut");
            m_manjeet.PatrolMode = CommonLIB.AppCode.ObjectBuilders.Object3D.MyPatrolMode.ONE_WAY;
            m_manjeet.Patrol();
            MyScriptWrapper.SetSleepDistance(m_manjeet, 3000);

            MySmallShipBot challenger = (MySmallShipBot)MyScriptWrapper.GetEntity("Challenger");
            challenger.SetWaypointPath("Gotostart");
            challenger.PatrolMode = CommonLIB.AppCode.ObjectBuilders.Object3D.MyPatrolMode.ONE_WAY;
            challenger.Patrol();

            MyScriptWrapper.SetSleepDistance(challenger, 20000);

            MyScriptWrapper.SetEntitiesEnabled(m_influenceSpheresFansBeforeStart, true);
        }

        void O04RaceLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.DetectorStartRacePosition));

            MySmallShipBot challenger = (MySmallShipBot)MyScriptWrapper.GetEntity("Challenger");
            challenger.SetWaypointPath("Gotostart");
            challenger.PatrolMode = CommonLIB.AppCode.ObjectBuilders.Object3D.MyPatrolMode.ONE_WAY;
            challenger.Patrol();
            MyScriptWrapper.SetSleepDistance(challenger, 20000);

            m_playerIsOnStart = true;
            m_challengerIsOnStart = true;
            m_unfollowedCompanions = true;


            m_detector_Explosion1.OnEntityEnter += RaceExplosionsDetectorEntered;
            m_detector_Explosion1.On();

            m_detector_Explosion2.OnEntityEnter += RaceExplosionsDetectorEntered;
            m_detector_Explosion2.On();

            m_detector_Explosion3.OnEntityEnter += RaceExplosionsDetectorEntered;
            m_detector_Explosion3.On();

            m_detector_TunnelExplosion.OnEntityEnter += RaceExplosionsDetectorEntered;
            m_detector_TunnelExplosion.On();

            var detectorComentator = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.ComentatorDeetctor));
            detectorComentator.OnEntityEnter += DetectorComentatorOnOnEntityEnter;
            detectorComentator.On();


            MyScriptWrapper.SetEntitiesEnabled(m_influenceSpheresFansAfterStart, true);


        }

        private void DetectorComentatorOnOnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            MyScriptWrapper.PlaySound3D(sender, MySoundCuesEnum.VocRace04running);
            sender.Off();
        }

        void O05WinLoaded(MyMissionBase sender)
        {
            MySmallShipBot challenger = (MySmallShipBot)MyScriptWrapper.GetEntity("Challenger");
            MyScriptWrapper.RemoveEntityMark(challenger);

            MySmallShipBot racer2 = (MySmallShipBot)MyScriptWrapper.GetEntity("Racer2");
            MyScriptWrapper.RemoveEntityMark(racer2);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Victory, 3, "KA06");
            m_unfollowedCompanions = false;
            m_playerIsOnStart = true;
            m_challengerIsOnStart = true;
            MyScriptWrapper.Follow(MySession.PlayerShip, m_marcus);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_tarja);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_vitolino);
            
        }

        void O07SmugglerDialogueReturnLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.StopTransition(3);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.DesperateWithStress, 0, "KA04"); 
        }

        void O03FlyToStartCleanUp(MyMissionBase sender)
        {
            MyScriptWrapper.OnBotReachedWaypoint -= Script_BotReachedWaypoint;

            if (m_detector_ReachStart != null)
            {
                m_detector_ReachStart.Off();
                m_detector_ReachStart.OnEntityEnter -= StartPositionEntered;
                m_detector_ReachStart.OnEntityLeave -= StartPositionLeaved;
            }
            
        }


        private void Script_BotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            MySmallShipBot challenger = (MySmallShipBot)MyScriptWrapper.GetEntity("Challenger");
            MySmallShipBot racer2 = (MySmallShipBot)MyScriptWrapper.GetEntity("Racer2");
            MySmallShipBot racer3 = (MySmallShipBot)MyScriptWrapper.GetEntity("Racer3");

            if (waypoint == MyScriptWrapper.GetEntity((uint)EntityID.ChallengerStartPosWaypoint) && bot == challenger)
            {
                m_challengerIsOnStart = true;
                MySmallShipBot botship = (MySmallShipBot)bot;
                botship.LookTarget = MyScriptWrapper.GetEntity((uint)EntityID.LookAtDummy);
                racer3.LookTarget = MyScriptWrapper.GetEntity((uint)EntityID.LookAtDummy);
                racer2.LookTarget = MyScriptWrapper.GetEntity((uint)EntityID.LookAtDummy);
            }
            
        }

        private void StartPositionEntered(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            if (m_unfollowedCompanions == false)
            {
                MyScriptWrapper.StopFollow(m_marcus);
                MyScriptWrapper.StopFollow(m_tarja);
                MyScriptWrapper.StopFollow(m_vitolino);
                m_unfollowedCompanions = true;
            }
            if (sender == m_detector_ReachStart && entity == MySession.PlayerShip)
            {
                m_playerIsOnStart = true;
            }
        }

        private void StartPositionLeaved(MyEntityDetector sender, MyEntity entity)
        {
            if (sender == m_detector_ReachStart && entity == MySession.PlayerShip)
            {
                m_playerIsOnStart = false;
            }
        }


        private void RaceExplosionsDetectorEntered(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (sender == m_detector_Explosion1 && entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddExplosions(m_explosionParticles1, MyExplosionTypeEnum.BOMB_EXPLOSION, 5);
                
                MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_explosionParticles1[0]), MySoundCuesEnum.VocRace09dirtyTricks);
                //MyScriptWrapper.IncreaseHeadShake(12f);
            }
            if (sender == m_detector_Explosion2 && entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddExplosions(m_explosionParticles2, MyExplosionTypeEnum.BOMB_EXPLOSION, 5);
                //MyScriptWrapper.IncreaseHeadShake(10f);
            }
            if (sender == m_detector_Explosion3 && entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddExplosions(m_explosionParticles3, MyExplosionTypeEnum.BOMB_EXPLOSION, 5);
                //MyScriptWrapper.IncreaseHeadShake(11f);
            }
            if (sender == m_detector_TunnelExplosion && entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ParticleExplosionInTunnel), MyExplosionTypeEnum.BOMB_EXPLOSION, 5);
                //MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxAcquireDroneOn);
                //MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0500);
            }
        }

        void Race_BotLoaded(MySmallShipBot bot)
        {
            switch (bot.Name)
            {
                case "Racer2":
                    {
                        var racer2 = new MyRacer(bot, MyTexts.RaceOpponent, "Racing2", 0.20f, 0.8f, 1.1f, 1.5f, 340, 250);
                        m_objective04_Race.AddRacer(racer2);
                    }
                    break;
                case "Racer3":
                    {
                        var racer3 = new MyRacer(bot, MyTexts.RaceOpponent, "Racing3", 0.20f, 0.8f, 1.1f, 1.5f, 340, 250);
                        m_objective04_Race.AddRacer(racer3);
                    }
                    break;
                case "Challenger":
                    {
                        var chalanger = new MyRacer(bot, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_RaceChallenger).ToString(), "Racing", 0.15f, 0.7f, 1.1f, 1.9f, 200, 150)
                                            {
                                                FrontDialogs = new List<MyDialogueEnum>() { MyDialogueEnum.RACING_CHALLENGER_0100_FRONT01, MyDialogueEnum.RACING_CHALLENGER_0200_FRONT02, MyDialogueEnum.RACING_CHALLENGER_0300_FRONT03, MyDialogueEnum.RACING_CHALLENGER_0400_FRONT04, MyDialogueEnum.RACING_CHALLENGER_0500_FRONT05 },
                                                BehindDialogs = new List<MyDialogueEnum>() { MyDialogueEnum.RACING_CHALLENGER_0600_BEHIND01, MyDialogueEnum.RACING_CHALLENGER_0700_BEHIND02, MyDialogueEnum.RACING_CHALLENGER_0800_BEHIND03, MyDialogueEnum.RACING_CHALLENGER_0900_BEHIND04, MyDialogueEnum.RACING_CHALLENGER_1000_BEHIND05 }
                                            };
                        m_objective04_Race.AddRacer(chalanger);
                    }
                    break;
            }
        }

        void Script_BotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            foreach (var item2 in new List<List<uint>> {m_raceFansSP1, m_raceFansSP2, m_raceFansSP3, m_raceFansSP4, m_raceFansSP5})
            {
                foreach (var item in item2)
                {
                    if (spawnpoint == MyScriptWrapper.GetEntity(item))
                    {
                        MySmallShipBot botship = (MySmallShipBot)bot;
                        botship.LookTarget = MySession.PlayerShip;
                        botship.DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.RacingFan).ToString();
                    }
                }
            }
            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.Racer2))
            {
                MySmallShipBot racer2 = (MySmallShipBot)bot;
                racer2.SetName("Racer2");
                MyScriptWrapper.SetSleepDistance(bot, 10000f);
                MyScriptWrapper.SetEntityDisplayName(bot, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_RaceOpponent).ToString());
                MyScriptWrapper.SetEntityDestructible(racer2, false);
            }
            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.Racer3))
            {
                MySmallShipBot racer3 = (MySmallShipBot)bot;
                racer3.SetName("Racer3");
                MyScriptWrapper.SetSleepDistance(bot, 10000f);
                MyScriptWrapper.SetEntityDisplayName(bot, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_RaceOpponent).ToString());
                MyScriptWrapper.SetEntityDestructible(racer3, false);
            }
            if (spawnpoint == MyScriptWrapper.GetEntity((uint)EntityID.Challenger))
            {
                MySmallShipBot challenger = (MySmallShipBot)bot;
                challenger.SetName("Challenger");
                MyScriptWrapper.SetSleepDistance(bot, 10000f);
                MyScriptWrapper.SetEntityDisplayName(bot, MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_RaceChallenger).ToString());
                MyScriptWrapper.SetEntityDestructible(challenger, false);

                MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0250);
            }
            foreach (var item in m_companionSpawnpoints)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(item))
                {
                    MySmallShipBot botship = (MySmallShipBot)bot;
                    botship.LookTarget = MySession.PlayerShip;
                    MyScriptWrapper.SetEntityDestructible(bot, false);
                }
            }
        }

        private void RaceStartNumberChanged(int number)
        {
            switch (number) 
            {
                case -1:
                    ShutDownAllLights();
                    MyScriptWrapper.MarkEntity(MyScriptWrapper.GetEntity((uint)EntityID.DetectorStartRacePosition), MyTexts.FlyToStartingPoint, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS);
                    break;
                case 0:
                    MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.LightFight, 3, "KA04");
                    foreach (var item in m_raceGates[0])
                    {
                        MyScriptWrapper.Highlight(item, true, this);
                    }
                    MyScriptWrapper.SetEntitiesEnabled(m_greenLights, true);
                    MyScriptWrapper.SetEntitiesEnabled(m_yellowLights, false);
                    break;
                case 1:
                    MyScriptWrapper.SetEntitiesEnabled(m_yellowLights, true);
                    MyScriptWrapper.SetEntitiesEnabled(m_redLights, false);
                    break;
                case 2:
                    MyScriptWrapper.SetEntitiesEnabled(m_redLights, true);
                    break;
                case 3:
                    ShutDownAllLights();
                    break;
            }
            if (number > 0) 
            {
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity((uint)EntityID.DetectorStartRacePosition));
            }
        }

        void ShutDownAllLights()
        {
            MyScriptWrapper.SetEntitiesEnabled(m_redLights, false);
            MyScriptWrapper.SetEntitiesEnabled(m_yellowLights, false);
            MyScriptWrapper.SetEntitiesEnabled(m_greenLights, false);
        }

        void RaceMissionFailed(MySmallShipBot bot)
        {
            Fail(MyTextsWrapperEnum.Fail_RaceLost);
        }

        private void RaceMissonOnCheckpointReached(MyEntity checkPoint, int checkPointNumber)
        {
            if (checkPointNumber + 1 < m_raceGates.Count) // highlight and activate towers on next checkpoint except for last checkpoint
            {
                foreach (var item in m_raceGates[checkPointNumber + 1])
                {
                    MyScriptWrapper.Highlight(item, true, this);
                }
                if (m_raceTowers[checkPointNumber + 1] != null) // only if there are some towers
                {
                    MyScriptWrapper.SetEntitiesEnabled(m_raceTowers[checkPointNumber + 1], true);
                }
            }
            foreach (var item in m_raceGates[checkPointNumber]) // stop highlight and deactivate towers on current checkpoint
            {
                    MyScriptWrapper.Highlight(item, false,this);
            }
            if (m_raceTowers[checkPointNumber] != null) // only if there are some towers
            {
                MyScriptWrapper.SetEntitiesEnabled(m_raceTowers[checkPointNumber], false);
            }
            switch (checkPointNumber)
            {
                case 1:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0400);
                    break;
                case 2:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0900);
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]),MySoundCuesEnum.VocRace05minefields);
                    break;
                case 3:
                    MyScriptWrapper.ActivateSpawnPoints(m_raceFansSP1);
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0800);
                    break;
                case 4:
                    MyScriptWrapper.ActivateSpawnPoints(m_raceFansSP2);
                    MyScriptWrapper.ActivateSpawnPoints(m_raceFansSP3);
                    break;
                case 5:
                    MyScriptWrapper.ActivateSpawnPoints(m_raceFansSP4);
                    break;
                case 6:
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]), MySoundCuesEnum.VocRace06speedDuel);
                    break;
                case 9:
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]), MySoundCuesEnum.VocRace07allThose);
                    break;
                case 10:
                    MyScriptWrapper.ActivateSpawnPoints(m_raceFansSP5);
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_1000);
                    break;
                case 7:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0600);
                    break;
                case 11:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_0700);
                    break;
                case 12:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_1100);
                    break;
                case 13:
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]), MySoundCuesEnum.VocRace08moving);
                    break;
                case 15:
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]), MySoundCuesEnum.VocRace10inside);
                    break;
                case 18:
                    MyScriptWrapper.PlayDialogue(MyDialogueEnum.JUNKYARD_RETURN_1200);
                    MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_raceGates[checkPointNumber][0]), MySoundCuesEnum.VocRace11wire);
                    break;
            }
        }


    }
       
}

#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

#endregion


namespace MinerWars.AppCode.Game.Missions
{
    internal class MyRacer
    {
        private MySmallShipBot m_racer;
        private string m_wayPointPath;
        public float SlowSpeed { get; set; }
        public float NormalSpeed { get; set; }
        public float FastSpeed { get; set; }
        public float SlowDownDistance { get; set; }
        public float ShootDistance { get; set; }
        public string Name;
        public List<MyDialogueEnum> FrontDialogs;
        public List<MyDialogueEnum> BehindDialogs;
        public float DialogPorobability { get; set; }

        private int m_nextShoot = 0;
        private float m_distanceToMe;
        private int shootInterval = 1000;
        private const float distanceOffset = 5.00f;
        private bool m_botBehindRacer;

        public float CurrentSppedUp
        {
            get { return m_racer.SpeedModifier; }
            set
            {
                if (m_racer.SpeedModifier != value)
                {
                    if (m_racer.SpeedModifier == NormalSpeed && value == FastSpeed) PlayBehindDialogue();
                    if (m_racer.SpeedModifier == NormalSpeed && value == SlowSpeed) PlayInFrontDialogue();
                    m_racer.SpeedModifier = value;
                }
            }
        }

        private void PlayInFrontDialogue()
        {   var rand = MyMwcUtils.GetRandomFloat(0, 1); 
            if (rand< DialogPorobability)
            {
                if (FrontDialogs != null) MyScriptWrapper.PlayDialogue(FrontDialogs[MyMwcUtils.GetRandomInt(0, FrontDialogs.Count - 1)]);
            }
        }

        private void PlayBehindDialogue()
        {
            if (MyMwcUtils.GetRandomFloat(0, 1) < DialogPorobability)
            {
                if (BehindDialogs != null) MyScriptWrapper.PlayDialogue(BehindDialogs[MyMwcUtils.GetRandomInt(0, BehindDialogs.Count - 1)]);
            }
        }




        public MyRacer(MySmallShipBot bot, string name, string path, float dialogPorbability, float slowSpeed = 0.9f, float normalSpeed = 1.3f,
                       float fastSpeed = 1.6f, float slowDownDistance = 250f, float shootDistance = 100f)
        {
            m_racerId = bot.EntityId.Value.NumericValue;
            m_wayPointPath = path;
            SlowDownDistance = slowDownDistance;
            SlowSpeed = slowSpeed;
            NormalSpeed = normalSpeed;
            FastSpeed = fastSpeed;
            Name = name;
            DialogPorobability = dialogPorbability;
            ShootDistance = shootDistance;

        }

        public MySmallShipBot RacerBot
        {
            get { return m_racer; }
        }

        public string WayPointPath
        {
            get { return m_wayPointPath; }
        }

        public void Load()
        {
            m_racer = MyScriptWrapper.GetEntity(m_racerId) as MySmallShipBot;
        }

        public void StartRace()
        {

            m_racer.SetWaypointPath(WayPointPath);
            m_racer.Patrol();
            m_racer.Enabled = true;
            m_racer.Engine = MySession.PlayerShip.Engine;
            MyHudIndicatorFlagsEnum flags = MyHudIndicatorFlagsEnum.SHOW_TEXT |
                                            MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS |
                                            MyHudIndicatorFlagsEnum.SHOW_DISTANCE;
            MyScriptWrapper.MarkEntity(m_racer, Name, flags, MyGuitargetMode.Objective);
        }


        public void UpdateRacerSpeed(float distance, int botsCheckPoint, int currentCheckPointIndex, int totalLenngth)
        {
            m_distanceToMe = distance;
            m_botBehindRacer = false;
            // Im in front of bot
            if (botsCheckPoint < currentCheckPointIndex)
            {
                m_botBehindRacer = true;
                if (Math.Abs(distance) > (SlowDownDistance + distanceOffset))
                {
                    CurrentSppedUp = FastSpeed;
                    //Debug.WriteLine("fast");
                }
                //Im loosing
            }
            else if (botsCheckPoint > currentCheckPointIndex)
            {
                if (Math.Abs(distance) > (SlowDownDistance + distanceOffset))
                {
                    CurrentSppedUp = SlowSpeed;
                    if (botsCheckPoint >= totalLenngth - 1) m_racer.AfterburnerEnabled = false;
                    // Debug.WriteLine("slow");
                }
                //we are in same race part
            }
            else
            {
                //Im first

                if (distance < -distanceOffset)
                {
                    m_botBehindRacer = true;
                    //Debug.WriteLine("fast");
                }

                if (distance < -(SlowDownDistance + distanceOffset))
                {
                    CurrentSppedUp = FastSpeed;
                    //Debug.WriteLine("fast");
                }
                //Im loosin
                else if (distance > SlowDownDistance + distanceOffset)
                {
                    CurrentSppedUp = SlowSpeed;
                    if (botsCheckPoint >= totalLenngth - 1) m_racer.AfterburnerEnabled = false;
                    //Debug.WriteLine("slow");
                }
                else if (distance < (SlowDownDistance + distanceOffset) || distance > -(SlowDownDistance - distanceOffset))
                {
                    CurrentSppedUp = NormalSpeed;
                    //Debug.WriteLine("normal");
                }
            }

            

            //Debug.WriteLine(m_racer.DisplayName+" "+botsCheckPoint);
            //Debug.WriteLine("-----------------------------------------");

        }

        private bool m_shoot = false;
        private uint m_racerId;


        public void UpdateShooting(int time)
        {
            if (m_nextShoot < time)
            {
                m_nextShoot = time + shootInterval;
                shootInterval = MyMwcUtils.GetRandomInt(0, 2000);
                m_shoot = !m_shoot;
            }

            if (m_shoot && m_botBehindRacer && Math.Abs(m_distanceToMe) > ShootDistance)
            {
               if(m_racer.Weapons!=null) m_racer.Shoot(MyMwcObjectBuilder_FireKeyEnum.Primary);
            }
        }
    }

    internal class MyObjectiveRace : MyMultipleObjectives
    {

        #region Events

        public delegate void ChechpointReachedEvent(MyEntity checkPoint, int CheckpointNumber);

        public delegate void OnRaceMissionFailed(MySmallShipBot bot);

        public delegate void StartingRaceEvent(int toStart);

        public event ChechpointReachedEvent CheckpointReached;
        public event OnRaceMissionFailed OnMissionFailed;
        public event StartingRaceEvent OnStartNumberChanged;

        #endregion

        #region Fields

        private const int startingTimeTick = 1000;
        private const int notifyCount = 4;
        private MyHudNotification.MyNotification[] notifications = new MyHudNotification.MyNotification[notifyCount];

        private List<MyMissionLocation> m_raceLocations;

        private int m_currentCheckPointIndex = 0;
        private List<uint> m_checkpointsIDs;
        private MyMwcVector3Int m_sector;
        private int m_startingTime;
        private bool m_isStartingGame;
        private uint m_detectorID;
        private MyEntityDetector m_detector;
        private bool m_restartRace;
        private List<int> botsFinishedCheckPoints = new List<int>();

        private MyTextsWrapperEnum m_normalName;
        private MyTextsWrapperEnum m_prepareName;
        private MyTextsWrapperEnum m_goBackName;
        private StringBuilder m_emptyString = new StringBuilder("");
        private MySmallShipBot m_winner;
        public List<string> RacerNames;
        public Action<MySmallShipBot> BotLoaded;
        public Action RaceStarted;
        #endregion

        private List<MyRacer> m_racers = new List<MyRacer>();

        public void AddRacer(MyRacer bot)
        {
            if (!m_racers.Contains(bot) && bot != null)
            {
                m_racers.Add(bot);
                botsFinishedCheckPoints.Add(0);
            }
        }

        public MyObjectiveRace(MyTextsWrapperEnum Name, MyTextsWrapperEnum PrepareName, MyTextsWrapperEnum goBackToStart,
                               MyMissionID ID, MyTextsWrapperEnum Description, MyTexture2D Icon, MyMission ParentMission,
                               MyMissionID[] RequiredMissions, MyMwcVector3Int sector, List<uint> Checkpoints,
                               uint detectorID, MyDialogueEnum? successDialogId = null,
                               MyDialogueEnum? startDialogId = null, bool displayObjectivesCount = true)
            :
                base(
                PrepareName, ID, Description, Icon, ParentMission, RequiredMissions, null, null, successDialogId,
                startDialogId, displayObjectivesCount)
        {
            Debug.Assert(Checkpoints != null);
            Debug.Assert(Checkpoints.Count > 0);
            m_sector = sector;
            m_checkpointsIDs = Checkpoints;
            m_detectorID = detectorID;

            m_normalName = Name;
            m_prepareName = PrepareName;
            m_goBackName = goBackToStart;
        }



        void MyScriptWrapper_OnSpawnpointBotSpawned(MyEntity entity1, MyEntity entity2)
        {
            if (BotLoaded != null)
            {
                if (RacerNames.Contains(entity2.Name))
                {
                    BotLoaded((MySmallShipBot)entity2);
                }
            }
        }
        public override void Load()
        {
            m_emptyString = new StringBuilder();
            m_isFailed = false;
            if (BotLoaded != null)
            {
                foreach (var racerName in RacerNames)
                {
                    MySmallShipBot racer = (MySmallShipBot)MyScriptWrapper.GetEntity(racerName);
                    BotLoaded(racer);
                }
            }
            MyScriptWrapper.OnSpawnpointBotSpawned += MyScriptWrapper_OnSpawnpointBotSpawned;
            ResetStart();
            m_raceLocations = new List<MyMissionLocation>();
            foreach (uint u in m_checkpointsIDs)
            {
                MyMissionLocation loc = new MyMissionLocation(m_sector, u);
                MyEntity entity;
                MyEntities.TryGetEntityById(new MyEntityIdentifier(u), out entity);
                loc.Entity = entity;
                m_raceLocations.Add(loc);
            }


            m_isStartingGame = true;
            m_startingTime = 0;


            m_detector = MyScriptWrapper.GetDetector(m_detectorID);
            if (m_detector != null)
            {
                m_detector.OnEntityEnter += OnDetectorEnter;
                m_detector.OnEntityLeave += OnDetectorLeave;
                m_detector.On();
            }

            foreach (var myRacer in m_racers)
            {
                myRacer.Load();
            }

            m_currentCheckPointIndex = 0;
            m_restartRace = false;

            
            base.Load();
        }


        private void OnDetectorLeave(MyEntityDetector sender, MyEntity entity)
        {
            if (m_isStartingGame)
            {
                MyScriptWrapper.RemoveEntityMark(m_detector);
                m_restartRace = true;
                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.GoBackToStartingPosition,
                                                                                   MyHudConstants.ENEMY_FONT, 5000));
                Name = m_goBackName;
                for (int i = 0; i < notifyCount; i++)
                {
                    if (notifications[i] != null)
                    {
                        notifications[i].Disappear();
                    }
                }
                if (OnStartNumberChanged != null) OnStartNumberChanged(-1);
                if(m_startSoundFinished) MyAudio.Stop();
            }
        }

        private void OnDetectorEnter(MyEntityDetector sender, MyEntity entity, int meetcriterias)
        {
            if (m_restartRace)
            {
                MyScriptWrapper.RemoveEntityMark(m_detector);
                ResetStart();
                m_restartRace = false;
                Name = m_prepareName;
            }
            else //only first time
            {

                MyScriptWrapper.PlaySound3D(sender, MySoundCuesEnum.VocRace01Welcome);
                m_startSoundFinished = false;
                MissionTimer.RegisterTimerAction(14000, () => { m_startSoundFinished = true; });
            }
        }


        private void ResetStart()
        {
            startCounter = 3;
            m_isStartingGame = true;
            m_startingTime = 0;

        }


        public override bool IsMissionEntity(MyEntity target)
        {
            if (base.IsMissionEntity(target)) return true;
            foreach (MyMissionLocation locationToVisit in m_raceLocations)
            {
                if (target == locationToVisit.Entity)
                {
                    return true;
                }
            }
            return false;
        }


        public override void Update()
        {
            base.Update();

            //notifications 3,2,1,Start ...
            if (m_isStartingGame && !m_restartRace && m_startSoundFinished)
            {
                UpdateNotifications();
                
            }

            if (m_isStartingGame)
            {
                AdditionalHudInformation = m_emptyString;
            }


            //managing next checkpoints

            UpdateCheckpoints();

            UpdateBots();

            if (m_isFailed)
            {
                OnMissionFailed(m_winner);
            }
        }



        private void UpdateCheckpoints()
        {
            var boundingSphere = MySession.PlayerShip.WorldVolume;
            MyMissionLocation raceLocation = m_raceLocations[m_currentCheckPointIndex];

            if (raceLocation.Entity.GetIntersectionWithSphere(ref boundingSphere))
            {
                if (CheckpointReached != null) 
                    CheckpointReached(raceLocation.Entity, m_currentCheckPointIndex);

                RemovelabelFormCurrentCheckPoint();

                m_currentCheckPointIndex++;
                if (m_currentCheckPointIndex >= m_raceLocations.Count)
                {
                    Success();
                    m_currentCheckPointIndex--;
                }
                else
                {
                    AddLabelToCurrentCheckPoint();
                    MyHud.ShowGPSPathToNextObjective(false);
                }
            }



        }

        private void UpdateBots()
        {
            MyMissionLocation raceLocation = m_raceLocations[m_currentCheckPointIndex];
            //controlling speed of bots 
            int counter = 0;
            foreach (MyRacer racer in m_racers)
            {
                var mySmallShipBot = racer.RacerBot;

                var boundingSphere = mySmallShipBot.WorldVolume;
                var botsCheckPoint = botsFinishedCheckPoints[counter];


                var myChceckPoint = m_raceLocations[m_currentCheckPointIndex].Entity.GetPosition();
                var hisCheckPoint = m_raceLocations[botsCheckPoint].Entity.GetPosition();



                var distance = (myChceckPoint - hisCheckPoint).Length() +
                               (MySession.PlayerShip.GetPosition() - myChceckPoint).Length() -
                               (mySmallShipBot.GetPosition() - hisCheckPoint).Length();

                racer.UpdateRacerSpeed(distance, botsCheckPoint, m_currentCheckPointIndex, m_raceLocations.Count);

                //check if bot went to next chcechpoint
                raceLocation = m_raceLocations[botsFinishedCheckPoints[counter]];
                if (raceLocation.Entity.GetIntersectionWithSphere(ref boundingSphere))
                {
                    botsFinishedCheckPoints[counter]++;
                    if (botsFinishedCheckPoints[counter] == m_raceLocations.Count)
                    {
                        //Bot won
                        if (OnMissionFailed != null && !m_isFailed)
                        {
                            m_winner = mySmallShipBot;
                            m_isFailed = true;
                        }
                        botsFinishedCheckPoints[counter]--;
                    }
                }

                racer.UpdateShooting(MissionTimer.ElapsedTime);
                counter++;
            }
        }


        private int startCounter = 3;
        private bool m_isFailed;
        private bool m_startSoundFinished;

        private void UpdateNotifications()
        {


            if (MissionTimer.ElapsedTime > m_startingTime)
            {   
                if(startCounter==3)
                {
                    MyScriptWrapper.PlaySound3D(m_detector, MySoundCuesEnum.VocRace02countdown);
                }

                if (startCounter == 0)
                {
                    notifications[0] = MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.Start,
                                                                          MyHudConstants.MISSION_FONT, startingTimeTick);
                    MyScriptWrapper.AddNotification(notifications[0]);
                    m_isStartingGame = false;
                    AddLabelToCurrentCheckPoint();
                    if (OnStartNumberChanged != null) OnStartNumberChanged(startCounter);
                    StartRace();
                }
                else
                {
                    notifications[startCounter] = MyScriptWrapper.CreateNotification(startCounter.ToString(),
                                                                                     MyHudConstants.MISSION_FONT,
                                                                                     startingTimeTick);
                    MyScriptWrapper.AddNotification(notifications[startCounter]);
                    m_startingTime = MissionTimer.ElapsedTime + startingTimeTick;
                    if (OnStartNumberChanged != null) OnStartNumberChanged(startCounter);
                    startCounter--;

                }
            }

        }



        private void StartRace()
        {
            Name = m_normalName;
            ReloadAdditionalHubInfo();
            foreach (MyRacer racer in m_racers)
            {
                racer.StartRace();
            }
            MyScriptWrapper.PlaySound3D(m_detector,MySoundCuesEnum.VocRace03cleanStart);
            if (RaceStarted != null)
            {
                RaceStarted();
            }
        }

        public override void Unload()
        {
            base.Unload();

            m_winner = null;

            foreach (MyMissionLocation locationToVisit in m_raceLocations)
            {
                SetLocationVisibility(false, locationToVisit.Entity, MyGuitargetMode.Objective);
            }

            MyScriptWrapper.OnSpawnpointBotSpawned -= MyScriptWrapper_OnSpawnpointBotSpawned;

            botsFinishedCheckPoints.Clear();
            m_racers.Clear();

            if (m_detector != null)
            {
                m_detector.OnEntityEnter -= OnDetectorEnter;
                m_detector.OnEntityLeave -= OnDetectorLeave;
                m_detector.Off();
            }


        }

        public override bool IsSuccess()
        {
            return false;
        }

        protected override int GetObjectivesCompletedCount()
        {
            return m_currentCheckPointIndex + 1;
        }

        protected override int GetObjectivesTotalCount()
        {
            return m_raceLocations.Count;
        }


        private void RemovelabelFormCurrentCheckPoint()
        {
            MyHud.RemoveText(m_raceLocations[m_currentCheckPointIndex].Entity);
        }

        private void AddLabelToCurrentCheckPoint()
        {
            string label = (m_currentCheckPointIndex + 1).ToString(CultureInfo.InvariantCulture) + "/" +
                           m_raceLocations.Count.ToString(CultureInfo.InvariantCulture);
            m_raceLocations[m_currentCheckPointIndex].Entity =
                MyScriptWrapper.GetEntity(m_raceLocations[m_currentCheckPointIndex].LocationEntityIdentifier);
            SetLocationVisibility(true, m_raceLocations[m_currentCheckPointIndex].Entity, MyGuitargetMode.Objective);
            MyHudIndicatorFlagsEnum flags = MyHudIndicatorFlagsEnum.SHOW_TEXT |
                                            MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS |
                                            MyHudIndicatorFlagsEnum.SHOW_DISTANCE;

            MyHud.ChangeText(m_raceLocations[m_currentCheckPointIndex].Entity, new StringBuilder(label),
                             MyGuitargetMode.Objective, 0, flags);
        }


    }
}



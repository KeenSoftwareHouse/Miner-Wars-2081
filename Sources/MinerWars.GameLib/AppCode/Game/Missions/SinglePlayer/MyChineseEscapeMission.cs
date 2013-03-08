#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.Resources;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyChineseEscapeMission : MyMission
    {
        readonly float MOTHERSHIP_FULLSPEED = 70;
        readonly float MOTHERSHIP_SLOWSPEED = 40;

        private MyObjective m_objective01_flyTowardsMadelyn;
        private MyObjective m_objective02_defendMadelyn;
        
        private MyEntity m_madelyn;
        private MyEntity m_transporter;
        private int m_activeTower;
        private int m_towersCount;
        private int m_switchCounter;
        private MyEntity[] m_towers = new MyEntity[3];
        private bool m_madelynDestinationReached;
        private bool m_transporterDestinationReached;
        private bool m_moveMadelynFlag;
        private bool m_reassignBotTargets;
        private MyEntityDetector m_detectorFirst;
        private MyEntityDetector m_detectorSecond;
        private MyEntityDetector m_detectorThird;
        private float m_motherShipSpeed;

        private float m_assignBotTargetsTimer;

        private List<MySmallShipBot> m_attackerBots = new List<MySmallShipBot>();
        private List<MySmallShipBot> m_attackerWaitingForPass = new List<MySmallShipBot>();
        private Vector3 transporterDestination = new Vector3(-1898.427f, 3664.597f, 10353.816f);

        private enum EntityID // list of IDs used in script
        {
            Transporter = 248,
            TransporterShip = 249,
            Tower1 = 2729,
            Tower2 = 2732,
            Tower3 = 9363,
            TowerDown = 2744,
            DetectorFirst = 7979,
            DetectorSecond = 7981,
            DetectorThird = 8116,
            StartLocation = 526,
            O01DefendMadelyn = 533,
            ExplosionRight1_1 = 7983,
            ExplosionRight2_1 = 7985,
            ExplosionRight2_2 = 7984,
            ExplosionLeft1_1 = 7993,
            ExplosionLeft2_1 = 7994,
            ExplosionLeft2_2 = 7995,
            ExplosionParticleFirstRight = 8115,
            ExplosionParticleRight1 = 8055,
            ExplosionParticleRight2 = 8053,
            ExplosionParticleLeft1 = 8054,
            ExplosionIn3_1 = 8405,
            ExplosionIn3_2 = 8404,
            ExplosionIn3_3 = 8403,
            ExplosionIn4_1 = 8415,
            ExplosionIn4_2 = 8414,
            ExplosionIn4_3 = 8416,
            ExplosionLarge1 = 9629,
            ExplosionLarge2 = 9630,
            ExplosionLarge3 = 9631,
            SpawnInR = 7986,
            SpawnInL = 7996,
            SpawnIn3 = 8402,
            SpawnIn4 = 8417,    
            SpawnFrontL = 8003,
            SpawnFrontR = 8004,
            SpawnCenterL = 8431,
            SpawnCenterR = 8428,
            SpawnLastL = 551,
            SpawnLastR = 552,
            SpawnLastAsteroid = 558,
            SpawnLast1 = 10320,
            SpawnLast1_2 = 10610,
            SpawnLast2 = 9825,
            SpawnLast2_2 = 10615,
            SpawnLast3 = 10319,
            SpawnLast3_2 = 10620,
            SpawnLast4 = 10065,

            //Transporter attack points
            AttackPoint01 = 8754,
            AttackPoint02 = 8755,
            AttackPoint03 = 8756,
            AttackPoint04 = 8757,
            AttackPoint05 = 8758,
            AttackPoint06 = 8759,
            AttackPoint07 = 8760,
            AttackPoint08 = 8761,
            AttackPoint09 = 8762,
            AttackPoint10 = 8763,
            AttackPoint11 = 8764,
        }

        #region Enums

        private List<uint> m_spawnCompanions = new List<uint> {
            2747, 2748, 2749, 2752, 2751, 2750, 2753
        };

        private List<EntityID> m_transporterAttackPoints = new List<EntityID> 
        {
            EntityID.AttackPoint01,EntityID.AttackPoint02,EntityID.AttackPoint03,EntityID.AttackPoint04,EntityID.AttackPoint05,
            EntityID.AttackPoint06,EntityID.AttackPoint07,EntityID.AttackPoint08,EntityID.AttackPoint09,EntityID.AttackPoint10,
            EntityID.AttackPoint11
        };

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            if (!IsMainSector) return;

            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            
            foreach (var value in m_spawnCompanions)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        #endregion

        public MyChineseEscapeMission()
        {
            ID = MyMissionID.CHINESE_ESCAPE; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.CHINESE_ESCAPE;
            Description = MyTextsWrapperEnum.CHINESE_ESCAPE_Description; // "Defend Madelyn's mothership and escape with stolen cargo\n"
            DebugName = new StringBuilder("08d-Chinese escape");
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1919599, 0, 5268734); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { MyMissionID.CHINESE_REFINERY };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.CHINESE_ESCAPE_SPEAK_WITH_MADELYN };
            RequiredActors = new MyActorEnum[] {MyActorEnum.MADELYN};

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            m_objective01_flyTowardsMadelyn = new MyObjective( // One member of that list
                (MyTextsWrapperEnum.CHINESE_ESCAPE_GET_CLOSER_Name),
                MyMissionID.CHINESE_ESCAPE_GET_CLOSER,
                (MyTextsWrapperEnum.CHINESE_ESCAPE_GET_CLOSER_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.O01DefendMadelyn),
                startDialogId: MyDialogueEnum.CHINESE_ESCAPE_0100_INTRODUCTION
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho };            
            m_objective01_flyTowardsMadelyn.OnMissionLoaded += O01FlyTowardsMadelynLoaded;
            m_objective01_flyTowardsMadelyn.OnMissionSuccess += O01FlyTowardsMadelynSuccess;

            m_objectives.Add(m_objective01_flyTowardsMadelyn);

                                    
            m_objective02_defendMadelyn = new MyObjectiveDestroy( 
                (MyTextsWrapperEnum.CHINESE_ESCAPE_DEFEND_SHIP_Name),
                MyMissionID.CHINESE_ESCAPE_DEFEND_SHIP,
                (MyTextsWrapperEnum.CHINESE_ESCAPE_DEFEND_SHIP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_ESCAPE_GET_CLOSER },
                null,
                new List<uint> { (uint)EntityID.SpawnLastL, (uint)EntityID.SpawnLastR, (uint)EntityID.SpawnLastAsteroid},
                true,
                true
            ) { SaveOnSuccess = true };            
            m_objectives.Add(m_objective02_defendMadelyn);
            m_objective02_defendMadelyn.OnMissionLoaded += new MissionHandler(m_objective02_defendMadelyn_OnMissionLoaded);
            m_objective02_defendMadelyn.OnMissionCleanUp += new MissionHandler(m_objective02_defendMadelyn_OnMissionCleanUp);
                                    
            
            var objective03_lastDialogue = new MyObjectiveDialog(
                MyTextsWrapperEnum.CHINESE_ESCAPE_SPEAK_WITH_MADELYN_Name,
                MyMissionID.CHINESE_ESCAPE_SPEAK_WITH_MADELYN,
                MyTextsWrapperEnum.CHINESE_ESCAPE_SPEAK_WITH_MADELYN_Description,
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_ESCAPE_DEFEND_SHIP },
                dialogId: MyDialogueEnum.CHINESE_ESCAPE_1000_LAST_OF_THEM
                ) { SaveOnSuccess = false };
            m_objectives.Add(objective03_lastDialogue);
        }

        void m_objective02_defendMadelyn_OnMissionCleanUp(MyMissionBase sender)
        {
            MissionTimer.ClearActions();
        }

        void m_objective02_defendMadelyn_OnMissionLoaded(MyMissionBase sender)
        {
            MissionTimer.RegisterTimerAction(TimeSpan.FromMinutes(2), MadelynDefended, false, MyTexts.DefendMadelyn, false);
        }

        void MadelynDefended()
        {
            m_objective02_defendMadelyn.Success();
        }

        public override void Load()
        {
            
            if (!IsMainSector) return;

            RemoveFriends();

            MyScriptWrapper.DisableAllGlobalEvents();

            m_attackerBots.Clear();

            m_detectorFirst = MyScriptWrapper.GetDetector((uint)EntityID.DetectorFirst);
            m_detectorSecond = MyScriptWrapper.GetDetector((uint)EntityID.DetectorSecond);
            m_detectorThird = MyScriptWrapper.GetDetector((uint)EntityID.DetectorThird);
            m_detectorFirst.SetSensorDetectRigidBodyTypes(null);
            m_detectorSecond.SetSensorDetectRigidBodyTypes(null);
            m_detectorThird.SetSensorDetectRigidBodyTypes(null);

            m_madelyn = MyScriptWrapper.GetEntity("Madelyn");
            //Because she was hidden in previous mission
            MyScriptWrapper.UnhideEntity(m_madelyn);
            m_transporter = MyScriptWrapper.GetEntity((uint)EntityID.Transporter);
            m_transporter.OnContactEvent += new Action<MyEntity>(m_transporter_OnContactEvent);
            m_reassignBotTargets = false;

            m_motherShipSpeed = MOTHERSHIP_FULLSPEED;

            MyScriptWrapper.PrepareMotherShipForMove(m_transporter);

            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(Localization.MyTextsWrapperEnum.SwitchInHUBTurrets, MyGuiManager.GetFontMinerWarsGreen(), 60000, 
                new object[] {
                    MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_LEFT),
                    MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.ROLL_RIGHT)
                }
            ));

            foreach (var item in m_spawnCompanions)
            {
                MyScriptWrapper.ActivateSpawnPoint(item);
            }
           
            var pos = MySession.PlayerShip.WorldMatrix.Translation;
            MySession.PlayerShip.WorldMatrix = m_transporter.WorldMatrix;
            Vector3 playerPos = m_transporter.WorldMatrix.Translation - 400 * m_transporter.WorldMatrix.Forward;
            MyScriptWrapper.Move(MySession.PlayerShip, playerPos);
            //MyScriptWrapper.EnablePhysics(MySession.PlayerShip.EntityId.Value.NumericValue, false);
            MyScriptWrapper.HideEntity(MySession.PlayerShip);

            m_towers[0] = MyScriptWrapper.GetEntity((uint)EntityID.Tower1);
            m_towers[1] = MyScriptWrapper.GetEntity((uint)EntityID.Tower2);
            m_towers[2] = MyScriptWrapper.GetEntity((uint)EntityID.Tower3);

            MyScriptWrapper.SetEntityPriority(m_towers[0], -1);
            MyScriptWrapper.SetEntityPriority(m_towers[1], -1);
            MyScriptWrapper.SetEntityPriority(m_towers[2], -1);
            MyScriptWrapper.SetEntityPriority(MyScriptWrapper.GetEntity((uint)EntityID.TowerDown), -1);

            MyScriptWrapper.TakeControlOfLargeWeapon(m_towers[m_activeTower]);
            MyScriptWrapper.ForbideDetaching();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Mystery);
            
            MyScriptWrapper.SwitchTowerPrevious += MyScriptWrapper_SwitchTowerPrevious;
            MyScriptWrapper.SwitchTowerNext += MyScriptWrapper_SwitchTowerNext;
            MyScriptWrapper.EntityDeath += MyScriptWrapper_OnEntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned += BotSpawned;

            m_objective01_flyTowardsMadelyn.OnMissionLoaded += O01FlyTowardsMadelynLoaded;
            m_objective01_flyTowardsMadelyn.OnMissionSuccess += O01FlyTowardsMadelynSuccess;

            m_madelynDestinationReached = false;
            m_transporterDestinationReached = false;
            m_moveMadelynFlag = false;
           
            m_towersCount = 3;
            m_activeTower = 0;
            m_switchCounter = 0;

            MyScriptWrapper.DrawHealthOfCustomPrefabInLargeWeapon(MyScriptWrapper.GetEntity((uint)EntityID.TransporterShip));
            MyScriptWrapper.DisableShipBackCamera();
            m_detectorFirst.OnEntityEnter += DetectorActionFirst;
            m_detectorFirst.On();

            MyScriptWrapper.OnDialogueFinished += MyScriptWrapper_OnDialogueFinished;
            base.Load();
        }

        void m_transporter_OnContactEvent(MyEntity obj)
        {
            //Damage all bot which touched our mothership (we have to prevent sticking bots to ms because of physics)
            if (obj is MySmallShipBot)
            {
                obj.DoDamage(0, 5, 0, MyDamageType.Unknown, MyAmmoType.Unknown, m_transporter);
            }
        }


        public override void Accept()
        {
            base.Accept();

            MySession.PlayerShip.RepairToMax();

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.China, MyFactions.RELATION_BEST);
        }

        public override void Unload()
        {
            if (!IsMainSector) return;
            MyScriptWrapper.SwitchTowerPrevious -= MyScriptWrapper_SwitchTowerPrevious;
            MyScriptWrapper.SwitchTowerNext -= MyScriptWrapper_SwitchTowerNext;
            MyScriptWrapper.EntityDeath -= MyScriptWrapper_OnEntityDeath;
            MyScriptWrapper.OnSpawnpointBotSpawned -= BotSpawned;
            MyScriptWrapper.OnDialogueFinished -= MyScriptWrapper_OnDialogueFinished;

            m_attackerBots.Clear();
            m_detectorFirst = null;
            m_detectorSecond = null;
            m_detectorThird = null;

            m_madelyn = null;
            m_transporter = null;

            m_towers[0] = null;
            m_towers[1] = null;
            m_towers[2] = null;
            
            m_detectorFirst = null;
            
            base.Unload();

            MyScriptWrapper.UnhideEntity(MySession.PlayerShip);
            MyScriptWrapper.EnableDetaching();
            MySession.PlayerShip.Activate(true, false);

            MyScriptWrapper.EnableAllGlobalEvents();

            if (MyScriptWrapper.IsMissionFinished(this.ID))
            {
                MyScriptWrapper.TravelToMission(MyMissionID.JUNKYARD_RETURN);
            }
        }

         public override void Update()
        {
            if (!IsMainSector) return;

            base.Update();

            if (m_madelyn != null && m_moveMadelynFlag && !m_madelynDestinationReached) 
            {
                m_madelynDestinationReached = MoveEntityForward(m_madelyn, 110f, new Vector3(-1294f, 3616f, 10572f), false);
            }
            if (!m_transporterDestinationReached)
            {
                MoveEntityForward(MySession.PlayerShip, m_motherShipSpeed);
                m_transporterDestinationReached = MoveEntityForward(m_transporter, m_motherShipSpeed, transporterDestination, true);
            }

            m_assignBotTargetsTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            if (m_assignBotTargetsTimer > 0.25f)
            {
                m_assignBotTargetsTimer -= 0.25f;
                AssignBotTargets();
                AllowBotManeuvers();
            }
        }

         void AssignBotTargets()
         {
             MyEntity transporterShip = MyScriptWrapper.TryGetEntity((uint)EntityID.TransporterShip);
             if (transporterShip != null)
             {
                 for (int i = m_attackerBots.Count - 1; i >= 0; i--)
                 {
                     var bot = m_attackerBots[i];

                     MyLine line = new MyLine(bot.GetPosition(), transporterShip.GetPosition(), true);
                     var result = MyEntities.GetIntersectionWithLine(ref line, bot, transporterShip, true, ignoreChilds: true);
                     if (!result.HasValue)
                     {
                         bot.Attack(transporterShip);
                         bot.SpeedModifier = 0;
                         m_attackerWaitingForPass.Add(bot);

                         m_attackerBots.RemoveAt(i);
                     }
                 }
             }
         }

         void AllowBotManeuvers()
         {
             

             MyEntity transporterShip = MyScriptWrapper.TryGetEntity((uint)EntityID.TransporterShip);
             if (transporterShip != null)
             {
                 for (int i = m_attackerWaitingForPass.Count - 1; i >= 0; i--)
                 {
                     var bot = m_attackerWaitingForPass[i];
                     
                     var direction = transporterDestination - transporterShip.GetPosition();
                     if (direction.LengthSquared() > 0)
                     {
                         direction.Normalize();
                         var d = Vector3.Dot(transporterShip.GetPosition() - m_towers[0].GetPosition(), direction);
                         var transporterToBot = bot.GetPosition() - transporterShip.GetPosition();

                         if (Vector3.Dot(direction, transporterToBot) < 180)
                         {
                             bot.SpeedModifier = 1;
                             m_attackerWaitingForPass.RemoveAt(i);
                         }
                     }
                 }
             }
         }

         void O01FlyTowardsMadelynLoaded(MyMissionBase sender)
         {
             //Fly slower on beginning
             m_motherShipSpeed = 40;

             MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction, 3, "KA02");
             MyEntity transporter = MyScriptWrapper.GetEntity((uint)EntityID.Transporter);
             
             /*
             foreach (EntityID attackPointID in m_transporterAttackPoints)
             {
                 MyEntity attackPoint = MyScriptWrapper.GetEntity((uint)attackPointID);
                 MyEntities.Remove(attackPoint);
                 transporter.AddChild(attackPoint, true);
             } */
         }

        void MyScriptWrapper_SwitchTowerPrevious()
        {
            m_switchCounter--;
            m_activeTower = mod(m_switchCounter, m_towersCount);
            MyScriptWrapper.TakeControlOfLargeWeapon(m_towers[m_activeTower]);       
        }

         void MyScriptWrapper_SwitchTowerNext()
        {
            m_switchCounter++;
            m_activeTower = mod(m_switchCounter, m_towersCount);
            MyScriptWrapper.TakeControlOfLargeWeapon(m_towers[m_activeTower]);       
        }

        void MyScriptWrapper_OnEntityDeath(MyEntity entity, MyEntity killedBy) 
        {
            if (!(entity is MySmallShipBot))
            {
                if (entity == m_madelyn || entity == m_transporter || entity is MyPrefabLargeShip)
                {
                    if (entity is MyPrefabLargeShip)
                        m_madelyn.MarkForClose();

                    MyScriptWrapper.EnableDetaching();
                    Fail(MyTextsWrapperEnum.Fail_MothershipDestroyed);
                    m_madelyn = null;
                }
            }
        }

        void BotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            //List<uint> { 
            //    (uint)EntityID.SpawnLastR, 
            //    (uint)EntityID.SpawnLastL, 
            //    (uint)EntityID.SpawnLastAsteroid, 
            //    (uint)EntityID.SpawnCenterL, 
            //    (uint)EntityID.SpawnCenterR }

            var attackerList = new List<uint>
            {
                (uint)EntityID.SpawnInR,
                (uint)EntityID.SpawnInL,
                (uint)EntityID.SpawnIn3,
                (uint)EntityID.SpawnIn4,
                (uint)EntityID.SpawnFrontL,
                (uint)EntityID.SpawnFrontR,
                (uint)EntityID.SpawnCenterL,
                (uint)EntityID.SpawnCenterR,
                (uint)EntityID.SpawnLastL,
                (uint)EntityID.SpawnLastR,
                (uint)EntityID.SpawnLastAsteroid,
                (uint)EntityID.SpawnLast1,
                (uint)EntityID.SpawnLast1_2,
                (uint)EntityID.SpawnLast2,
                (uint)EntityID.SpawnLast2_2,
                (uint)EntityID.SpawnLast3,
                (uint)EntityID.SpawnLast3_2,
                (uint)EntityID.SpawnLast4,
            };
            
            foreach (var item in attackerList)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(item) && bot != null)
                {
                    MyScriptWrapper.SetSleepDistance(bot as MySmallShipBot, 10000);
                    var botShip = bot as MySmallShipBot;
                    m_attackerBots.Add(botShip);
                    //botShip.Attack(MyScriptWrapper.GetEntity((uint)m_transporterAttackPoints[MyMwcUtils.GetRandomInt(m_transporterAttackPoints.Count)]));
                }
            }

            foreach (var item in m_spawnCompanions)
            {
                if (spawnpoint == MyScriptWrapper.GetEntity(item))
                {
                    MySmallShipBot botship = bot as MySmallShipBot;
                    //botship.SlowDown = 0.583f;
                    botship.SpeedModifier = 0.583f * (MOTHERSHIP_SLOWSPEED / MOTHERSHIP_FULLSPEED);

                    botship.IsDestructible = m_reassignBotTargets;
                    if (m_reassignBotTargets)
                        bot.Kill(m_towers[MyMwcUtils.GetRandomInt(m_towers.Length)]);
                }
            }
        }

        private void MyScriptWrapper_OnDialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (dialogue == MyDialogueEnum.CHINESE_ESCAPE_0100_INTRODUCTION)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0200_IT_IS_TRAP);
            }
            else
            if (dialogue == MyDialogueEnum.CHINESE_ESCAPE_0200_IT_IS_TRAP)
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.China, MyFactions.RELATION_WORST);
                MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3, "MM01");

                m_motherShipSpeed = MOTHERSHIP_FULLSPEED;

                foreach (var spawn in m_spawnCompanions)
                {
                    MyScriptWrapper.ChangeFaction(spawn, MyMwcObjectBuilder_FactionEnum.China);
                    foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
                    {
                        if (bot.Ship != null)
                        {
                            MyScriptWrapper.ChangeFaction(bot.Ship.EntityId.Value.NumericValue, MyMwcObjectBuilder_FactionEnum.China);
                            bot.Ship.SpeedModifier = 1f;
                            MyScriptWrapper.SetEntityDestructible(bot.Ship, true);

                            m_attackerBots.Add(bot.Ship);
                        }
                    }
                }


                m_detectorFirst.Off();
                m_detectorFirst.OnEntityEnter -= DetectorActionFirst;
                m_detectorSecond.OnEntityEnter += DetectorActionSecond;
                m_detectorSecond.On();

                m_reassignBotTargets = true;
                //AssignTargetsToBots();

                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(
                                    MyTextsWrapperEnum.WatchMothershipHealth,
                                    MyHudConstants.ENEMY_FONT,
                                    8000,
                                    null));
            }
            else if (dialogue == MyDialogueEnum.CHINESE_ESCAPE_0300_ON_THIRD)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0400_ON_NINE); 
            }
        }

        private void DetectorActionFirst(MyEntityDetector sender, MyEntity entity, int meetCriterias) 
        {
            if (entity == m_transporter)
            {
            }
        }
        
        //private void AssignTargetsToBots()
        //{
        //    foreach (var spawn in m_spawnCompanions)
        //    {
        //        MyScriptWrapper.GetEntity(spawn).Faction = MyMwcObjectBuilder_FactionEnum.China;
        //        foreach (var bot in MyScriptWrapper.GetSpawnPointBots(spawn))
        //        {
        //            if (bot.Ship != null)
        //            {
        //                bot.Ship.Attack(MyScriptWrapper.GetEntity((uint)m_transporterAttackPoints[MyMwcUtils.GetRandomInt(m_transporterAttackPoints.Count)]));
        //            }
        //        }
        //    }

        //    if (m_reassignBotTargets)
        //    {
        //        MissionTimer.RegisterTimerAction(3000, AssignTargetsToBots, false);
        //    }
        //}

         private void DetectorActionSecond(MyEntityDetector sender, MyEntity entity, int meetCriterias)
         {
             if (entity == m_transporter)
             {
                 MissionTimer.RegisterTimerAction(8000, AsteroidInsideExplosionR2, false);
                 MissionTimer.RegisterTimerAction(12000, AsteroidInsideExplosionL1, false);
                 MissionTimer.RegisterTimerAction(20000, WatchFront, false);
                 MissionTimer.RegisterTimerAction(15000, AsteroidInsideExplosion3, false);
                 MissionTimer.RegisterTimerAction(18000, AsteroidInsideExplosion4, false);
                 MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionRight1_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
                 m_detectorSecond.Off();
                 m_detectorSecond.OnEntityEnter -= DetectorActionSecond;
                 m_detectorThird.OnEntityEnter += DetectorActionThird;
                 m_detectorThird.On();
             }
         }  

         private void DetectorActionThird(MyEntityDetector sender, MyEntity entity, int meetCriterias) 
         {

             MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLarge3), true);
             MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
             if (entity == m_transporter)
             {
                 m_moveMadelynFlag = true;
                 MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionParticleFirstRight), true);
                 MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
                 MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnCenterL);
                 MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnCenterR);
                 MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0700_ON_THE_RIGHT);

                 MissionTimer.RegisterTimerAction(20000, Last1, false);
                 m_detectorThird.Off();
                 m_detectorThird.OnEntityEnter -= DetectorActionThird;
             }
         }

         void Last1()
         {
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast1);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast1_2);
             MissionTimer.RegisterTimerAction(15000, Last2, false);
         }

         void Last2()
         {
             MissionTimer.RegisterTimerAction(20000, Last3, false);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast2);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast2_2);
         }

         void Last3()
         {
             MissionTimer.RegisterTimerAction(15000, Last4, false);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast3);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast3_2);
         }

         void Last4()
         {
             MissionTimer.RegisterTimerAction(5000, MadelynInSight, false);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLast4);
         }

         void MadelynInSight()
         {
             MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0800_MADELYN_IN_SIGHT);
         }

        void AsteroidInsideExplosionR2() 
        {
             MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionRight2_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
             MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionRight2_2), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
             MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0300_ON_THIRD); 
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnInR);
        }

        void AsteroidInsideExplosionL1() 
        {
             MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLeft1_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
             MissionTimer.RegisterTimerAction(1000, AsteroidInsideExplosionL2, false);
        }

        void AsteroidInsideExplosionL2()
        {
             MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLeft2_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
             MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLeft2_2), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
             MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnInL);
        }

        void AsteroidInsideExplosion3()
        {
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLarge1), true);
            MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn3_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn3_2), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MissionTimer.RegisterTimerAction(1000, AsteroidInsideExplosion3_2, false);
        }

        void AsteroidInsideExplosion3_2()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn3_3), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnIn3);
        }

        void AsteroidInsideExplosion4()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn4_1), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn4_2), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionLarge2), true);
            MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
            MissionTimer.RegisterTimerAction(1000, AsteroidInsideExplosion4_2, false);
        }

        void AsteroidInsideExplosion4_2()
        {
            MyScriptWrapper.AddExplosion(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionIn4_3), MyExplosionTypeEnum.BOMB_EXPLOSION, 100f, 500f);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnIn4);
        }

        void WatchFront() 
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0600_WATCH_FRONT);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnFrontL);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnFrontR);
        }

        void O01FlyTowardsMadelynSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3, "KA03");
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLastAsteroid);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.CHINESE_ESCAPE_0900_KILL_THOSE_BASTARDS);
            MissionTimer.RegisterTimerAction(15000, ExplosionLastAsteroid1, false);
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionParticleLeft1), true);
            MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLastL);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnLastR);
        }

        void ExplosionLastAsteroid1() 
        {
            MissionTimer.RegisterTimerAction(5000, ExplosionLastAsteroid2, false);
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionParticleRight1), true);
            MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
        }

        void ExplosionLastAsteroid2() 
        {
            MyScriptWrapper.SetParticleEffect(MyScriptWrapper.GetEntity((uint)EntityID.ExplosionParticleRight2), true);
            MyScriptWrapper.PlaySound3D(MySession.PlayerShip, MySoundCuesEnum.SfxShipLargeExplosion);
        }

        bool MoveEntityForward(MyEntity entity, float speed, Vector3 destination, bool updateVelocity)
        {
            float SlowDownRadius = 200;
            float StopRadius = 10;

           // speed *= 10;

            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction

            if (Vector3.DistanceSquared(destination, entity.GetPosition()) < SlowDownRadius * SlowDownRadius)
            {
                velocity = entity.Physics.LinearVelocity * 0.995f;
            }

            entity.Physics.Clear();

            if (Vector3.DistanceSquared(destination, entity.GetPosition()) > StopRadius * StopRadius)
            {
                Vector3 newPos = entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                //entity.SetPosition(newPos); // recalculate position
                if (updateVelocity)
                {
                    MyScriptWrapper.MoveWithVelocity(entity, newPos, velocity);
                    // When at target set velocity to zero
                    if (Vector3.DistanceSquared(destination, entity.GetPosition()) > StopRadius * StopRadius)
                    {
                        MyScriptWrapper.MoveWithVelocity(entity, entity.GetPosition(), Vector3.Zero);
                    }
                }
                else
                {
                    MyScriptWrapper.Move(entity, newPos);
                }
                return false;
            }
            return true;
        }
        
        int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        void MoveEntityForward(MyEntity entity, float speed)
        {
            Vector3 velocity = speed * entity.WorldMatrix.Forward; // Speed in direction
            MyScriptWrapper.Move(entity, entity.GetPosition() + velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
        }

        public override void Success()
        {
            base.Success();

            m_reassignBotTargets = false;
  
        }
    }
}

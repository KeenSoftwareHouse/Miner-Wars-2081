using System;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using KeenSoftwareHouse.Library.Memory;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Helpers;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.HUD;
using System.Text;
using MinerWars.AppCode.Game.Managers.PhysicsManager.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;

namespace MinerWars.AppCode.Game.Managers.EntityManager.Entities
{

    //using MinerWars.AppCode.Game.Trailer;

    

    class MySmallShipBot : MySmallShip
    {

        public const int TICKS_BETWEEN_WAYPOINT_CHECKS = 20;

        //TODO hack for let enemy go after some time
        private int m_guardingCounter;

        #region Constants

        //Each parameter is scaled for these values, there is a function for rescaling these parameters so don't change this
        private const int DEFAULT_MASS = 7000;
        private const int DEFAULT_DDISTANCE = 250;

        //Which aim is good for shooting, adjusted with tuner for each weapon during shooting
        private const float FIRING_CONE_DEGREES = 45f;

        //How far can we check waypoint as visited
        private const float WAYPOINT_DISTANCE_FOR_CHECK = 100f;

        //Speed modificator
        private const float MAX_SPEED_AHEAD = -0.5f;
        private const float MAX_ROTATE_SPEED_COEF = 40.0f;

        //Minimal rotation
        private const float MIN_ROTATION_POWER = 1200;

        // How much should bot predict movements to aim more precisely
        private const float MOVEMENT_PREDICTION_TUNER = 0.18f;

        // How much must bots position change to save new position as waypoint
        private const int MEMORY_MAX_TIME_MILLISECONDS = 1000;
        private const int CHECKS_TO_GET_LOST = 5;
        private const int FREQUENCY_FOR_RAYCAST_TICKS = 5;

        private const int MAX_LEVELING_DISTANCE = 2000;
        public  const int MAX_FOLLOW_DISTANCE = 1000;
        private const int MAX_CHAIN_AGGRO_DISTANCE = 2000;
        private const int MAX_ENEMY_IN_SIGHT = 2000;
        private const int MIN_WAY_POINT_CHECK_TIMER = 2000;//2sec
        private const int MAX_WAY_POINT_CHECK_TIMER = 10000;//10sec

        private float GetMinAttackDistance() { return 100.0f * MathHelper.Lerp(0.5f, 1f, DifficultyAttackReactDistance); }
        private float m_attack_distance = 0;//maybe problem

        /// <summary>
        /// At what distance does bot consider player too close to get a shot and should back off.
        /// </summary>
        private const float TOO_CLOSE_DISTANCE = 750.0f;

        /// <summary>
        /// How long will bot roll to the side he is strafing
        /// </summary>
        private const int MAX_ROLLING_TIME_WHEN_STRAFING = 30;

        #endregion

        #region Enums

        /// <summary>
        /// Contains current bot's burst times for each weapon slot, based on ammo types.
        /// For each weapon slot we have some important values:
        /// 0 - Firing duration tuner    - less for rockets and low amount of ammo weapons, basic value is 10x shot interval for tuner = 1.0
        /// 1 - Firing frequency tuner   - more for rockets and low amount of ammo weapons
        /// 2 - Firing Cone tuner        - bot should launch its bigger weapons when it has more chances for hit
        /// 3 - Firing Distance          - maximal shoot distance for weapon
        /// 4 - Firing distance tuner    - tuner for shooting range
        /// 5 - Shot interval            - interval between two shots from weapon
        /// </summary>
        private enum MyBotWeaponsParametrsEnum
        {
            FiringDurationTuner = 0,
            FiringFrequencyTuner = 1,
            FiringConeTuner = 2,
            FiringDistance = 3,
            FiringDistanceTuner = 4,
            ShotIntervalMilliseconds = 5,
        }

        /// <summary>
        /// All behaviors bot can use in decision logic
        /// </summary>
        public enum MyBotBehaviorsEnum
        {
            None = 0,
            Attack = 1,
            FollowTarget = 2,
            FollowWaypoint = 3,
            FlyAroundTarget = 4,
            RunFromTarget = 5,
            RaidAttack = 6,
        }

        /// <summary>
        /// Supported decision logics, main setup for bots overall behavior 
        /// </summary>
        public enum MyBotDecisionLogicsEnum
        {
            None=0,
            CasualFighter=1,
            GuardingFighter=2,
            CarefulFighter=3,
            Follower=4,
            Guard=5,
            FindYourself=6,
            Harvester=7,
            Repair=8
        }

        /// <summary>
        /// Survival triggers bot can use
        /// </summary>
        public enum MyBotSurvivalAlertsEnum
        {
            None=0,
            LowHealth=1,
            LowOxygen=2,
            LowFuel=3,
            EnemyInsight=4,
            CallForHelp=5,
            Lost=6,
            UnderAttack=7
        }

        /// <summary>
        /// Type of waypoint ridethrough
        /// </summary>
        public enum MyBotGuardTypeEnum
        {
            OneWay=0,
            Cycle=1,
            Patrol=2
        }
        #endregion

        #region Properties

        /// <summary>
        /// Bot decision for normal and combat situations
        /// </summary>
        public MyBotDecisionLogicsEnum NormalDecision { get; set; }

        public MyBotDecisionLogicsEnum AttackDecision { get; set; }


        //always 0 super easy 1 max hard

        // how much will bot strafe during combat  (not its speed)
        public float DifficultyStrafingSpeed { get; set; }

        // ship speed 
        public float DifficultyMovingSpeed { get; set; }

        // affect speed of shooting (delays between all shooting)
        public float DifficultyFireRatio { get; set; }

        // what guns/slots will be used during fight / shoting
        public float DifficultyGunUsageRatio { get; set; }

        // how often will be raid attack executed
        public float DifficultyRaidAttackOccurrence { get; set; }

        // how often will be fly around exectued
        public float DifficultyFlyAroundTargetOccurrence { get; set; }

        //how far will bot react on enemy presence and from what distance he will shoot ( harder - more far )
        public float DifficultyAttackReactDistance { get; set; }

        //use of fire burst 0 - 1
        public float DifficultyUseFireBurst { get; set; }


        #endregion

        #region Variables
        //Determines if bot should fire now
        private bool m_isFiring = false;

        //Switches afterburner on/off
        private bool m_afterBurner = false;

        //Switches decision logic between non-combat decision and combat decision
        private bool m_isInCombat = false;

        //Switches between flight right to the target or some way around
        private bool m_rolling = false;

        //If bot is lost
        private bool m_isLost = false;

        /// <summary>
        /// reference to keep track of all bots following us
        /// </summary>
        private List<MySmallShipBot> m_followers = new List<MySmallShipBot> ();

        /// <summary>
        /// Size of waypoint memory
        /// </summary>
        private int m_waypointMemorySize = 200;

        /// <summary>
        /// Arrays for control times for each behavior
        /// </summary>
        private float[] m_minBehaviorsTimesInTicks = new float[Enum.GetNames(typeof(MyBotBehaviorsEnum)).Length];
        private float[] m_maxBehaviorsTimesInTicks = new float[Enum.GetNames(typeof(MyBotBehaviorsEnum)).Length];

        /// <summary>
        /// Keeps distance from target for multiple use of this parameter each update
        /// </summary>
        private float m_distanceToTarget;

        /// <summary>
        /// Keeps angle to target for multiple use of this parameter each update
        /// </summary>
        private float m_angleToTarget;

        /// <summary>
        /// Direction of current ship strafe.
        /// </summary>
        //private MyBotStrafeDirectionsEnum m_strafe = MyBotStrafeDirectionsEnum.None;
        private float m_strafeHorizontal = 0;
        private float m_strafeVertical = 0;


        /// <summary>
        /// Timers for strafing direction change, making decision etc. 
        /// </summary>
        private float m_strafeTimeCounter;
        private float m_strafeMaxTime;

        private float m_raidTimeCounter;
        private float m_raidMaxTime;


        private float m_decisionTimeCounter;
        private float m_decisionMaxTime = MyConstants.PHYSICS_STEPS_PER_SECOND * 0.5f;

        private float m_behaviourTimeCounter;
        private float m_behaviourMaxTime;

        private int m_nextWaypointTimer;
        private int m_waypointCheckTimer;

        private int m_lostCounter;

        private MyEntity m_target;
        /// <summary>
        /// Target for attack/follow etc.
        /// </summary>
        private MyEntity Target
        {
            get
            {
                return m_target;
            }
            set
            {
                if (m_target != value)
	            {
                    Trace.SendMsgLastCall(string.Format("Bot {0} Target changed: {1}", Name, value != null ? value.ToString() : "null"));
	            }

                m_target = value;
            }
        }

        private MyEntity m_followTarget;
        /// <summary>
        /// Target for attack/follow etc.
        /// </summary>
        private MyEntity FollowTarget
        {
            get
            {
                return m_followTarget;
            }

            set
            {
                if (m_followTarget != value)
                {
                    Trace.SendMsgLastCall(string.Format("Bot {0} Follow target: {1}", Name, value != null ? value.ToString() : "null"));
                }
                m_followTarget = value;
            }
        }

        private MyEntity m_threat;
        /// <summary>
        /// some1 who attacked me or is threat to me
        /// </summary>
        private MyEntity Threat
        {
            get
            {
                return m_threat;
            }
            set
            {
                if (m_threat != value)
                {
                    Trace.SendMsgLastCall(string.Format("Bot {0} Threat changed: {1}", Name, value != null ? value.ToString() : "null"));
                }
                m_threat = value;
            }
        }

        /// <summary>
        /// spot where are we moving
        /// </summary>
        private Vector3? m_flightTarget;

        private MyBotBehaviorsEnum m_behaviour = MyBotBehaviorsEnum.None;
        /// <summary>
        /// Actual behavior of bot
        /// This can be changed only in decision logic
        /// </summary>
        private MyBotBehaviorsEnum Behaviour
        {
            get
            {
                return m_behaviour;
            }

            set
            {
                if (m_behaviour != value)
                {
                    Trace.SendMsgLastCall(string.Format("Bot {0} Behavior changed: {1}", Name, value));
                }
                m_behaviour = value;
            }
        }

        private MyBotDecisionLogicsEnum m_survivalDecision = MyBotDecisionLogicsEnum.None;
        //notEditable | this is changed by survival kit, there you can set your desired decision for each occasion
        private MyBotDecisionLogicsEnum SurvivalDecision
        {
            get
            {
                return m_survivalDecision;
            }
            set
            {
                if (value != m_survivalDecision)
                {
                    Trace.SendMsgLastCall(string.Format("Bot {0} Survival Decision changed: {1}", Name, value));
                }

                m_survivalDecision = value;
            }
        }

        /// <summary>
        /// Waypoint for guarding/traveling and second one for creating bot's own way (so he can return back or find a way for others)
        /// </summary>
        private MyWaypointMemory m_savedWaypoints;

        private MyWaypointMemory m_rememberWaypoints;
        private MyWaypointMemory m_temporaryWaypoints;

        private MyCanSeeMemory m_canSeeCheck;
        private MyCanSeeMemory m_canSeeCheckForMove;
        private MyCanSeeMemory m_canSeeCheckForShoot;

        /// <summary>
        /// Priority survival alerts and decision logic for each one so bot can control some indicators before his usual work
        /// Priority 0 has the highest priority
        /// /// </summary>
        private Dictionary<MyBotSurvivalAlertsEnum, MySurvivalDecision> m_survivalKit;

        /// <summary>
        /// Saves all weapon types the bot can use
        /// </summary>
        private MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum m_weaponTypesMounted;

        /// <summary>
        /// Weapon parameters for all weapon slots
        /// </summary>
        private MyBotParams[] m_botWeaponParamsAllSlots;

        /// <summary>
        /// Friendly fire cue
        /// </summary>
        private MySoundCue? m_friendlyFireCue;
        #endregion

        #region Structs
        /// <summary>
        /// Params of mounted weapons, so bot know when to fire each slot etc.
        /// </summary>
        private struct MyBotParams
        {
            public float[] WeaponsParams;
            public MyMwcObjectBuilder_FireKeyEnum FireKey;
            public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum WeaponType;
            public bool Enabled;
            public bool BurstFiring;
            public MyStopwatch BurstTimer;
            public float LastShotMilliseconds;
        }

        /// <summary>
        /// Survival control and trigger
        /// </summary>
        private struct MySurvivalDecision
        {
            public MyBotDecisionLogicsEnum SurvivalDecision;
            public int SurvivalTrigger;
        }

        /// <summary>
        /// Struct for memorize waypoint way
        /// </summary>
        public struct MyWaypointMemory
        {
            //List of waypoints
            public List<Vector3> m_WaypointList;

            //Pointer to actual waypoint
            public int m_WayIndex;

            //How many waypoints will be checked while looking for shorter way
            public int m_checkDistance;

            /// <summary>
            /// If bot is goin forward or getting back
            /// +1 forward
            /// -1 backward
            /// </summary>
            public int m_IndexDirection;

            /// <summary>
            /// Type of cycling waypoints 
            /// </summary>
            public MyBotGuardTypeEnum m_GuardType;

            public MyWaypointMemory(MySmallShipBot m_bot)
            {
                m_WayIndex = 0;
                m_checkDistance = 1;
                m_IndexDirection = 1;
                m_GuardType = MyBotGuardTypeEnum.OneWay;
                m_WaypointList = new List<Vector3>(m_bot.m_waypointMemorySize){m_bot.GetPosition()};
            }
        }

        private struct MyCanSeeMemory
        {
            public bool m_result;
            public int m_ticksFromLastCheck;

            public MyCanSeeMemory(bool result)
            {
                m_result = result;
                m_ticksFromLastCheck = FREQUENCY_FOR_RAYCAST_TICKS;
            }
        }
        #endregion

        #region BotLogics

        /// <summary>
        /// This method sets various variables for moving, shooting, etc. based on actual behavior and bot conditions
        /// TODO
        /// </summary>
        public void ResolveBehavior()
        {
            switch (Behaviour)
            {
                case MyBotBehaviorsEnum.Attack:
                    {
                        //VOICE: aggressive
                        break;
                    }
                case MyBotBehaviorsEnum.FlyAroundTarget:
                    {
                        break;
                    }
                case MyBotBehaviorsEnum.FollowTarget:
                case MyBotBehaviorsEnum.FollowWaypoint:
                    {
                        if (m_flightTarget == null)
                            break;
                        //VOICE: On my way
                        //If bot is really far from target, he should use afterburner
                        if (DistanceToTarget(m_flightTarget.Value) > 250 ) 
                            m_afterBurner = true;
                        else 
                            m_afterBurner = false;
                        break;
                    }
                case MyBotBehaviorsEnum.RunFromTarget:
                    {
                        //VOICE: flee
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets variables which can be set only once - when changed to some behavior
        /// TODO
        /// </summary>
        /// <param name="behavior"></param>
        private void OnBehaviorChanged(MyBotBehaviorsEnum behavior)
        {
            switch (behavior)
            {
                case MyBotBehaviorsEnum.Attack:
                    {
                        m_isFiring = true;
                        m_rolling = false;
                        m_afterBurner = false;
                        break;
                    }
                case MyBotBehaviorsEnum.RaidAttack:
                    {
                        m_isFiring = true;
                        m_rolling = true;
                        m_afterBurner = true;
                        break;
                    }
                case MyBotBehaviorsEnum.FlyAroundTarget:
                    {
                        m_isFiring = false;
                        m_rolling = true;
                        m_afterBurner = true;
                        break;
                    }
                case MyBotBehaviorsEnum.FollowTarget:
                    {
                        m_isFiring = false;
                        m_rolling = false;
                        m_afterBurner = false;
                        break;
                    }
                case MyBotBehaviorsEnum.FollowWaypoint:
                    {
                        m_isFiring = false;
                        m_rolling = false;
                        m_afterBurner = false;
                        break;
                    }
                case MyBotBehaviorsEnum.RunFromTarget:
                    {
                        m_isFiring = false;
                        m_rolling = false;
                        m_afterBurner = true;
                        break;
                    }
            }
        }

        public void SetWaypoints(MyWaypointMemory mem)
        {
            m_savedWaypoints = mem;
        }

        public void InitPatrolWaypointsSimpleDirections(Vector3 pos, float radi)
        {
            m_savedWaypoints = new MyWaypointMemory(this);

            Matrix randomMatrix = Matrix.CreateLookAt(pos, pos + MyMwcUtils.GetRandomVector3Normalized(), MyMwcUtils.GetRandomVector3Normalized());

            //Set Waypoint route
            SetFirstWaypoint(ref m_savedWaypoints, pos);
            AddWaypoint(ref m_savedWaypoints, pos + randomMatrix.Right * radi);
            AddWaypoint(ref m_savedWaypoints, pos + randomMatrix.Up * radi);
            AddWaypoint(ref m_savedWaypoints, pos + randomMatrix.Down * radi + randomMatrix.Right * radi);

            //WayPointerToStart(ref m_savedWaypoints);
            m_savedWaypoints.m_GuardType = MyBotGuardTypeEnum.Cycle;
        }

        Vector3 HelperGetSidePositionVector(Matrix spot, float dir, float dr, float myradi, List<MyRBElement> rbFounded, ref bool canPlace)
        {
            Vector3 sideDir = spot.Right;
            sideDir.Normalize();
            Vector3 pos = spot.Translation + sideDir * dr * dir /*+ -spot.Forward * myradi * 2*/;
            //bool canPlace = true;
            if (CanSee(pos) != null)
                return Vector3.Zero;
            //now try find spot
            foreach (MyRBElement rb in rbFounded)
            {
                MyEntity ent = ((MyGameRigidBody)rb.GetRigidBody().m_UserData).Entity;
                if (ent == this)
                    continue;
                if (ent is MySmallShip)
                {
                    if (MyMwcUtils.SphereVsSphere(ent.WorldVolume.Center, ent.WorldVolume.Radius, pos, WorldVolume.Radius))
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (ent is MySmallShipBot)
                {
                    MySmallShipBot bot = ent as MySmallShipBot;
                    if (bot.m_flightTarget.HasValue && MyMwcUtils.SphereVsSphere(bot.m_flightTarget.Value, ent.WorldVolume.Radius, pos, WorldVolume.Radius))
                    {
                        canPlace = false;
                        break;
                    }
                }
            }
            return pos;
        }

        //computable expensive
        Vector3 GetNearestFreeSpotToFollowTarget(Matrix spot)
        {
            Vector3 toRet = Vector3.Zero;
            float myradi = WorldVolume.Radius * 2.5f;
            float myboxd = 500;

            //find our free spot
            using (var rbFounded = PoolList<MyRBElement>.Get())
            {

                MyPruningStructure prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();

                BoundingBox rbInputElementGetWorldSpaceAABB = new BoundingBox(
                    spot.Translation - new Vector3(myboxd ,myboxd ,myboxd ), 
                    spot.Translation +new Vector3(myboxd ,myboxd ,myboxd ) );
                prunningStructure.OverlapRBAllBoundingBox(ref rbInputElementGetWorldSpaceAABB, rbFounded);

                for (float dr = 0; dr < 100; dr += myradi)//100m from left to right
                {
                    bool canPlaced1 = true;
                    Vector3 d1 = HelperGetSidePositionVector(spot, -1, dr, myradi, rbFounded, ref canPlaced1);

                    bool canPlaced2 = true;
                    Vector3 d2 = HelperGetSidePositionVector(spot, 1, dr, myradi, rbFounded, ref canPlaced2);

                    if (canPlaced1)
                        toRet = d1;

                    if (canPlaced2 && (d2 - WorldMatrix.Translation).Length() < (d1 - WorldMatrix.Translation).Length() || !canPlaced1)
                        toRet = d2;

                    if (toRet != Vector3.Zero)
                        break;
                }

            }
            return toRet;
        }

        /// <summary>
        /// Changes behaviors according actual decision logic
        /// TODO
        /// </summary>
        private void MakeDecision()
        {
            //Selects bot decision logic, according to priority: Survival -> Combat -> Normal
            MyBotDecisionLogicsEnum superiorBehaviour = NormalDecision;
            if (m_isInCombat) 
                superiorBehaviour = AttackDecision;
            if (SurvivalDecision != MyBotDecisionLogicsEnum.None) 
                superiorBehaviour = SurvivalDecision;

            //Make decision 
            switch (superiorBehaviour)
            {
                case MyBotDecisionLogicsEnum.None:
                    {
                        break;
                    }
                //Follow waypoint route
                case MyBotDecisionLogicsEnum.Guard:
                    {
                        FollowWaypoint(ref m_savedWaypoints);
                        
                        //If we cant go to target, try go this way for a while before getting lost
                        if (CheckCanSeeIgnoreMovingObjects(ref m_canSeeCheckForMove, m_flightTarget.Value))
                        {
                            //we want to return bot to his actual position if something happens

                            SetActualWaypoint(ref m_rememberWaypoints, this.GetPosition());
                            m_lostCounter = 0;
                        }
                        else
                        {
                            m_lostCounter++;
                        }

                        //OK, we are definitely lost now
                        if (m_lostCounter > CHECKS_TO_GET_LOST)
                        {
                            m_lostCounter = 0;
                            m_rememberWaypoints.m_IndexDirection = -1;
                            //CheckMemoryList(ref m_rememberWaypoints, -1, 15);
                            m_isLost = true;
                            break;
                        }
                        break;
                    }
                case MyBotDecisionLogicsEnum.Follower:
                    {
                        if (FollowTarget == null)
                        {
                            FollowTarget = SearchTargetToFollow();
                            if (FollowTarget == null)
                                NormalDecision = MyBotDecisionLogicsEnum.Guard;
                        }

                        float toleranceRadius = WorldVolume.Radius * 7;//experimental value so far yet

                        if (FollowTarget != null && (this.WorldMatrix.Translation - FollowTarget.GetPosition()).Length() > toleranceRadius)
                        {
                            if (true||CheckCanSeeIgnoreMovingObjects(ref m_canSeeCheckForMove, FollowTarget.GetPosition()))
                            {
                                Vector3 toleranceFly = GetNearestFreeSpotToFollowTarget(FollowTarget.WorldMatrix);
                                if ((this.WorldMatrix.Translation - toleranceFly).Length() < toleranceRadius)
                                    m_flightTarget = this.WorldMatrix.Translation;
                                else
                                    m_flightTarget = toleranceFly;
                                Behaviour = MyBotBehaviorsEnum.FollowTarget;
                                OnBehaviorChanged(Behaviour);
                                m_lostCounter = 0;
                            }
                            else
                            {
                                m_lostCounter++;
                            }
                        }else
                            m_flightTarget = this.WorldMatrix.Translation;
                        
                        if(m_lostCounter > CHECKS_TO_GET_LOST)
                        {
                            if (FollowTarget == MySession.PlayerShip) GetTargetsRoute((MySmallShip)FollowTarget);
                            m_lostCounter = 0;
                            m_isLost = true;
                        }
                        
                        break;
                    }
                
                case MyBotDecisionLogicsEnum.CasualFighter:
                    {
                        CasualFighter();
                        break;
                    }
                case MyBotDecisionLogicsEnum.GuardingFighter:
                    {
                        if (m_guardingCounter++ > 30)
                        {
                            m_guardingCounter = 0;
                            m_isInCombat = false;
                            Target = null;
                        }
                        else
                            CasualFighter();
                        break;
                    }
                    
                case MyBotDecisionLogicsEnum.FindYourself:
                    {
                        if(Target==null)
                        {
                            //set next flight target and change behaviour to fly
                            m_flightTarget = SetupNextWaypointIfPosible(ref m_rememberWaypoints);

                            //If waypoint changed, delete previous waypoint from list
                            int previousWaypoint = m_rememberWaypoints.m_WayIndex + 1;
                            if (m_rememberWaypoints.m_WaypointList.Count > previousWaypoint)
                                m_rememberWaypoints.m_WaypointList.RemoveRange(previousWaypoint, 1);

                            MyBotBehaviorsEnum next_behaviour = MyBotBehaviorsEnum.FollowWaypoint;
                            if (next_behaviour != Behaviour)
                                OnBehaviorChanged(next_behaviour);

                            Behaviour = next_behaviour;

                            //if we are at start of memory, we are not lost anymore
                            float d = DistanceToTarget(m_rememberWaypoints.m_WaypointList[0]);
                            if (DistanceToTarget(m_rememberWaypoints.m_WaypointList[0]) < WAYPOINT_DISTANCE_FOR_CHECK)
                            {
                                m_rememberWaypoints.m_IndexDirection = 1;
                                m_isLost = false;
                            }
                            break;
                        }
                        else
                        {
                            if (CheckCanSee(ref m_canSeeCheck, Target) && Target == MySession.PlayerShip)
                            {
                                if (m_temporaryWaypoints.m_WayIndex >= m_temporaryWaypoints.m_WaypointList.Count - 1)
                                {
                                    GetTargetsRoute((MySmallShip)Target);
                                }
                                FollowWaypoint(ref m_temporaryWaypoints);
                            }
                            else
                            {
                                m_isLost = false;
                            }
                            break; 
                        }
                    }
                   
            }
        }

        private void NotifyEnemy(MyEntity enemy)
        {
            bool factionPossibleEnemy =
                MyFactions.GetFactionsRelation(this.Faction, enemy.Faction) == MyFactionRelationEnum.Enemy;
                //||
                //MyFactions.GetFactionsRelation(this.Faction, enemy.Faction) == MyFactionRelationEnum.Neutral;
            if (factionPossibleEnemy)
                Threat = enemy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enemy"></param>
        private void NotifyMyGroupAboutEnemy(MyEntity enemy)
        {
            NotifyEnemy(enemy);

            //notify leader and its followers
            if (FollowTarget != null && FollowTarget is MySmallShipBot)
            {
                (FollowTarget as MySmallShipBot).NotifyEnemy(enemy);
                foreach (MySmallShip fl in (FollowTarget as MySmallShipBot).m_followers)
                    (fl as MySmallShipBot).NotifyEnemy(enemy);
            }

            //notify my childs
            foreach(MySmallShip fl in m_followers)
                (fl as MySmallShipBot).NotifyEnemy(enemy);
            
            //then all "friends" around
            using (var rbFounded = PoolList<MyRBElement>.Get())
            {
                MyPruningStructure prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();
                float myboxd = MAX_CHAIN_AGGRO_DISTANCE / 2.0f;
                BoundingBox rbInputElementGetWorldSpaceAABB = new BoundingBox(
                    GetPosition() - new Vector3(myboxd, myboxd, myboxd),
                    GetPosition() + new Vector3(myboxd, myboxd, myboxd));
                prunningStructure.OverlapRBAllBoundingBox(ref rbInputElementGetWorldSpaceAABB, rbFounded);
                foreach (MyRBElement rb in rbFounded)
                {
                    MyEntity ent = ((MyGameRigidBody)rb.GetRigidBody().m_UserData).Entity;
                    if (ent == this)
                        continue;

                    if (ent is MySmallShipBot)
                    {
                        MySmallShipBot bot = ent as MySmallShipBot;
                        bool factionFriend = MyFactions.GetFactionsRelation(this.Faction, bot.Faction) == MyFactionRelationEnum.Friend;
                        if (!bot.IsDead() && bot.GetObjectWeFollow() == null && factionFriend)
                            bot.NotifyEnemy(enemy);
                    }

                }
            }
            return; 
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private Vector3 GetRaidAttackSpot(Matrix spot, float direct_angle, float deltadist)
        {
            Vector3 disp = this.WorldMatrix.Up * MyMwcUtils.GetRandomFloat(-1,1)  + this.WorldMatrix.Left * MyMwcUtils.GetRandomFloat(-1,1);
            disp.Normalize();

            // rotate disp about its forward
            float radi = (WorldMatrix.Translation - spot.Translation).Length() * (2 + deltadist);
            disp *= radi * (float)Math.Tan(direct_angle);
            return this.WorldMatrix.Translation + this.WorldMatrix.Forward * radi + disp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private Vector3 GetFlyAroundSpot(Matrix spot)
        {
            Vector3 LeftNormal = spot.Left;
            LeftNormal.Normalize();
            float around = (MyMwcUtils.GetRandomFloat(0,1) > 0.5f )?(MyMwcUtils.GetRandomFloat(-0.6f, -0.3f)) : (MyMwcUtils.GetRandomFloat(0.3f, 0.6f));
            return this.WorldMatrix.Translation + LeftNormal * (spot.Translation - this.WorldMatrix.Translation).Length() * around;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private Vector3 GetAttackSpot(Matrix spot, float rightleftcoef, float updowncoef)
        {
            rightleftcoef = MathHelper.Lerp(0, 1, DifficultyStrafingSpeed);
            updowncoef = MathHelper.Lerp(0, 1, DifficultyStrafingSpeed);
            Vector3 LeftNormal = spot.Left;
            LeftNormal.Normalize();
            Vector3 UpNormal = spot.Up;
            UpNormal.Normalize();
            return spot.Translation + Vector3.Normalize(WorldMatrix.Translation - spot.Translation) * m_attack_distance + LeftNormal*rightleftcoef + UpNormal*updowncoef;//ATTACK_DISTANCE;
        }


        /// <summary>
        /// Does domage to entity
        /// </summary>
        /// <param name="deltaHealth">Amount of damage</param>
        public override void DoDamage(float healthDamage, float damage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource)
        {     
            MyFactionRelationEnum? factionRelation = null;
            if (damageSource != null)
            {
                factionRelation = MyFactions.GetFactionsRelation(this.Faction, damageSource.Faction);
            }

            // notify group
            if (damageSource is MySmallShip && factionRelation == MyFactionRelationEnum.Enemy)
            {
                NotifyMyGroupAboutEnemy(damageSource);                
            }
            
            bool wasDead = IsDead();
            
            if (CanDie(damageSource))
            {
                base.DoDamage(healthDamage, damage, damageType, ammoType, damageSource);
            }
            
            // if player's ship fired
            if (damageSource == MySession.PlayerShip)
            {
                // friendly fire warning
                if (factionRelation == MyFactionRelationEnum.Friend)
                {
                    if (m_friendlyFireCue == null || !m_friendlyFireCue.Value.IsPlaying)
                    {
                        m_friendlyFireCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxFriendlyFireWarning);
                    }
                }
                // targed destroyed notification
                if (factionRelation == MyFactionRelationEnum.Enemy && !wasDead && IsDead())
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.SfxTargetDestroyed);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CasualFighter()
        {
            //If target insight, set flight target to it, else try to fly on last known position of enemy
            if (Behaviour == MyBotBehaviorsEnum.Attack)
                m_attack_distance = MyMwcUtils.GetRandomFloat(GetMinAttackDistance(), GetMinAttackDistance());

            
            if(Behaviour == MyBotBehaviorsEnum.FlyAroundTarget || Behaviour == MyBotBehaviorsEnum.RaidAttack)
            {
                if ((m_flightTarget.Value - this.WorldMatrix.Translation).Length() < this.WorldVolume.Radius || m_flightTarget.HasValue && CanSee(m_flightTarget.Value)!=null)
                {
                    Behaviour = MyBotBehaviorsEnum.Attack;
                    OnBehaviorChanged(Behaviour);
                    ResetBehaviourTimer();
                }
            }

            if (Behaviour == MyBotBehaviorsEnum.Attack && Target != null && Target.Health > 0)
            {
                if (CheckCanSee(ref m_canSeeCheckForShoot, Target))
                {
                    if (m_strafeTimeCounter++ > m_strafeMaxTime)
                    {
                        m_strafeHorizontal = MyMwcUtils.GetRandomFloat(-100,100);
                        m_strafeVertical = MyMwcUtils.GetRandomFloat(-100, 100);
                        ResetStrafeTimer();
                    }

                    m_flightTarget = GetAttackSpot(Target.WorldMatrix, m_strafeHorizontal, m_strafeVertical);
                }
                else
                {
                    Behaviour = MyBotBehaviorsEnum.FollowWaypoint;
                    if (Target == MySession.PlayerShip) GetTargetsRoute((MySmallShip)Target);
                    OnBehaviorChanged(Behaviour);
                    ResetBehaviourTimer();
                }
            }

            if (Behaviour == MyBotBehaviorsEnum.FollowWaypoint)
            {
            }

            if (m_behaviourTimeCounter > m_behaviourMaxTime)
            {
                switch (Behaviour)
                {
                    case MyBotBehaviorsEnum.RaidAttack:
                        {
                            break;
                        }
                    //If bot is near target and attacking now, he should try some evasive maneuver
                    case MyBotBehaviorsEnum.Attack:
                        {

                            if (m_distanceToTarget < TOO_CLOSE_DISTANCE * MathHelper.Lerp(1f,0.5f, DifficultyAttackReactDistance))
                            {
                                Behaviour = MyBotBehaviorsEnum.FlyAroundTarget;
                                if (Target != null)
                                {
                                    m_flightTarget = GetFlyAroundSpot(Target.WorldMatrix);
                                }
                            }
                            //here setup raid type attack triger on some switch
                            if (m_distanceToTarget < TOO_CLOSE_DISTANCE * MathHelper.Lerp(1f, 0.5f, DifficultyAttackReactDistance) && m_raidTimeCounter++ > m_raidMaxTime)
                            {
                                Behaviour = MyBotBehaviorsEnum.RaidAttack;

                                if (Target != null && Target.Health > 0)
                                    m_flightTarget = GetRaidAttackSpot(Target.WorldMatrix, (float)Math.PI * (float)MyMwcUtils.GetRandomFloat(10, 15) / 180, MyMwcUtils.GetRandomFloat(0.5f, 1));
                                //NotifyMyGroupAboutRaid( m_target );
                                ResetRaidTimer();
                            }

                            if (Target != null && Target.Health > 0)
                                NotifyMyGroupAboutEnemy( Target );
                            //m_strife_attack_coef = 0; // how many meters will strife +- to avoid shots this value vill pump constantly to add some dynamics during attack
                            //m_attack_distance = MyMwcUtils.GetRandomFloat(MIN_ATTACK_DISTANCE, MAX_ATTACK_DISTANCE);

                            break;
                        }
                    case MyBotBehaviorsEnum.FollowWaypoint:
                        if (CheckCanSee(ref m_canSeeCheckForShoot, Target))
                        {
                            Behaviour = MyBotBehaviorsEnum.Attack;
                        }
                        else
                        {
                            if (m_temporaryWaypoints.m_WayIndex >= m_temporaryWaypoints.m_WaypointList.Count - 1)
                            {
                                GetTargetsRoute((MySmallShip)Target);
                            }

                            if (m_temporaryWaypoints.m_WaypointList != null)
                            {
                                if (m_temporaryWaypoints.m_WaypointList.Count > m_temporaryWaypoints.m_WayIndex)
                                {
                                    int index = m_temporaryWaypoints.m_WayIndex;
                                    while (index < m_temporaryWaypoints.m_WaypointList.Count)
                                    {
                                        Vector3 waypoint = m_temporaryWaypoints.m_WaypointList[index];
                                        if (CanSee(waypoint) == null)
                                        {
                                            m_temporaryWaypoints.m_WayIndex = index++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    m_flightTarget = m_temporaryWaypoints.m_WaypointList[m_temporaryWaypoints.m_WayIndex];
                                }
                            }

                            ResetBehaviourTimer();
                        }
                        break;
                    //If bot is not attacking now, he should attack
                    default:
                        {
                            Behaviour = MyBotBehaviorsEnum.Attack;
                            break;
                        }
                }
                //Restart behaviour timer so bot will change behaviour later
                OnBehaviorChanged(Behaviour);
                ResetBehaviourTimer();
            }
        }

        private void FollowWaypoint(ref MyWaypointMemory memory)
        {
            if (memory.m_WaypointList == null)
                return;

            //Set the right waypoint as target for flight
            m_flightTarget = SetupNextWaypointIfPosible(ref memory);

            MyBotBehaviorsEnum next_behaviour = MyBotBehaviorsEnum.FollowWaypoint;
            if (next_behaviour != Behaviour)
                OnBehaviorChanged(next_behaviour);

            Behaviour = next_behaviour;

        }

        /// <summary>
        /// Cycle all survival alerts by their priority and sets survival decision
        /// </summary>
        private void CheckSurvivalKit()
        {
            SurvivalDecision = MyBotDecisionLogicsEnum.None;
            foreach (var item in m_survivalKit)
            {
                if (CheckSurvivalAlert(item.Key, item.Value))
                {
                    SurvivalDecision = item.Value.SurvivalDecision;
                    return;
                }
            }
        }

        /// <summary>
        /// Check one survival alert and decide if it is triggered
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private bool CheckSurvivalAlert(MyBotSurvivalAlertsEnum alert, MySurvivalDecision check)
        {
            switch (alert)
            {
                case MyBotSurvivalAlertsEnum.LowHealth:
                    {
                        //VOICE: DMG taking / LOW HP 
                        //TODO: merge
                        //if (check.SurvivalTrigger * DEFAULT_HEALTH_MAX < this.Health) return true;
                        return false;
                    }
                case MyBotSurvivalAlertsEnum.LowOxygen:
                    {
                        //VOICE: LOW AIR/ EMPTY AIR
                        return false;
                    }
                case MyBotSurvivalAlertsEnum.LowFuel:
                    {
                        //VOICE: LOW FUEL / EMPTY FUEL
                        return false;
                    }
                case MyBotSurvivalAlertsEnum.UnderAttack:
                    {
                        if (Threat != null)
                        {
                            if (MyFactions.GetFactionsRelation(this.Faction, Threat.Faction) == MyFactionRelationEnum.Enemy
                                ||
                                MyFactions.GetFactionsRelation(this.Faction, Threat.Faction) == MyFactionRelationEnum.Neutral)
                            {
                                Target = Threat;
                                return true;
                            }
                        }

                        return false;
                    }

                case MyBotSurvivalAlertsEnum.EnemyInsight:
                    {
                        //handle this with under attack
                        if (Threat != null && m_survivalKit.ContainsKey(MyBotSurvivalAlertsEnum.UnderAttack))
                            return false;
                        //VOICE: DETECT PLAYER

                        //Search for targets around
                        //find new target if old target is hidden
                        if (Target == null || !CheckCanSee(ref m_canSeeCheck, Target) )
                        {
                            Target = SearchTargetToAttack(check.SurvivalTrigger);

                            if (Target != null)
                            {
                                m_isInCombat = true;
                                m_behaviourTimeCounter = Int16.MaxValue;
                                return true;
                            }
                            else
                            {
                                Target = null;
                                m_isInCombat = false;
                                m_behaviourTimeCounter = Int16.MaxValue;
                            }
                        }
                        else if (Target.IsDead())
                        {
                            Target = null;
                            m_isInCombat = false;
                            m_behaviourTimeCounter = Int16.MaxValue;
                            return true;
                        }
                        else 
                            if (
                                MyFactions.GetFactionsRelation(this.Faction, Target.Faction) == MyFactionRelationEnum.Enemy
                                &&
                                (GetPosition() - Target.GetPosition()).Length() < MAX_ENEMY_IN_SIGHT)
                            return true;

                        return false;
                    }
                case MyBotSurvivalAlertsEnum.CallForHelp: 
                    {
                        return false;
                    }
                case MyBotSurvivalAlertsEnum.Lost:
                    {
                        if (m_isLost)
                        {
                            return true;
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Adds survival alert, what to do when this alert is triggered and trigger for this ocasion
        /// </summary>
        /// <param name="survivalAlert">trigger for survival decision</param>
        /// <param name="decision">decision for handeling survival</param>
        /// <param name="trigger">percentage of max parameter for health, oxygen and fuel issues | distance for detecting enemy</param>
        public void AddSurvivalAlert(MyBotSurvivalAlertsEnum survivalAlert, MyBotDecisionLogicsEnum decision, int trigger)
        {
            var survivalHandler = new MySurvivalDecision { SurvivalDecision = decision, SurvivalTrigger = trigger };
            m_survivalKit.Add(survivalAlert, survivalHandler);
        }

        /// <summary>
        /// Adds one waypoint to the end of waypoint list
        /// </summary>
        /// <param name="memory">Waypoint memory</param>
        /// <param name="waypoint"></param>
        public void AddWaypoint(ref MyWaypointMemory memory, Vector3 waypoint)
        {
            //TODO: check if waypoint is inside sector
            //if(MyMwcSectorConstants.SECTOR_SIZE_FOR_PHYS_OBJECTS_BOUNDING_BOX.Contains(waypoint) != ContainmentType.Contains)

            memory.m_WaypointList.Add(waypoint);
            memory.m_WayIndex = memory.m_WaypointList.Count - 1;
        }

        //Re - Initialize waypoint memory
        public void CleanRoute(ref MyWaypointMemory memory)
        {
            memory.m_WaypointList.Clear();
            memory.m_WaypointList.Add(GetPosition());
        }

        //Changes first waypoint which is normally set to bot starting location
        public void SetFirstWaypoint(ref MyWaypointMemory memory, Vector3 target)
        {
            memory.m_WaypointList[0] = target;
        }

        /// <summary>
        /// Sets position instead actual waypoint
        /// </summary>
        /// <param name="memory">which memory to use</param>
        /// <param name="target">position to set</param>
        public void SetActualWaypoint(ref MyWaypointMemory memory, Vector3 target)
        {
            if (memory.m_WaypointList.Count > 0)
                memory.m_WaypointList[memory.m_WayIndex] = target;

            else
                AddWaypoint(ref memory, target);
        }

        protected void AddFollower(MySmallShipBot follower)
        {
            if (m_followers.Contains(follower))
                return;
            m_followers.Add(follower);
        }

        protected void RemoveFollower(MySmallShipBot follower)
        {
            m_followers.Remove(follower);
        }

        public MyEntity GetObjectWeFollow()
        {
            return FollowTarget;
        }

        /// <summary>
        /// Sets the next waypoint pointer based on actual waypoint, direction which we are taking and guarding type
        /// </summary>
        /// <param name="memory"></param>
        private void NextWayPointer(ref MyWaypointMemory memory)
        {
            int nextIndex = memory.m_WayIndex + memory.m_IndexDirection;

            //If we are not at the very end or start of route, just return next index
            if (nextIndex < memory.m_WaypointList.Count && nextIndex >= 0)
            {
                memory.m_WayIndex = nextIndex;
                return;
            }
            //else handle the end of route
            switch (memory.m_GuardType)
            {
                case MyBotGuardTypeEnum.OneWay:
                    {
                        return;
                    }
                case MyBotGuardTypeEnum.Cycle:
                    {
                        if (memory.m_IndexDirection > 0)
                            memory.m_WayIndex = 0;
                        else
                            memory.m_WayIndex = memory.m_WaypointList.Count - 1;

                        return;
                    }
                case MyBotGuardTypeEnum.Patrol:
                    {
                        memory.m_IndexDirection *= -1;
                        memory.m_WayIndex += memory.m_IndexDirection;
                        return;
                    }
            }
        }

        /// <summary>
        /// Decides if bot will change to next waypoint or keeps previous
        /// </summary>
        /// <param name="memory"></param>
        /// <returns>Position of waypoint</returns>
        private Vector3 SetupNextWaypointIfPosible(ref MyWaypointMemory memory)
        {
            //If bot is far away from target waypoint, he should go to it, else he should continou with next waypoint
            if (DistanceToTarget(memory.m_WaypointList[memory.m_WayIndex]) < WAYPOINT_DISTANCE_FOR_CHECK && MyMinerGame.TotalGamePlayTimeInMilliseconds - m_nextWaypointTimer > m_waypointCheckTimer)
            {
                m_nextWaypointTimer = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_waypointCheckTimer = (int)MyMwcUtils.GetRandomFloat(MIN_WAY_POINT_CHECK_TIMER, MAX_WAY_POINT_CHECK_TIMER);
                NextWayPointer(ref memory);
            }
            return memory.m_WaypointList[memory.m_WayIndex];
        }

        /*
        private void RememberPosition()
        {
            //add waypoint
            AddWaypoint(ref m_rememberWaypoints, GetPosition());

            //check previous slots
            //CheckMemoryList(ref m_rememberWaypoints, -1);

            if (m_waypointTicks >= TICKS_BETWEEN_WAYPOINT_CHECKS)
            {
                CheckMemoryListBackwards(ref m_rememberWaypoints, -TICKS_BETWEEN_WAYPOINT_CHECKS);
                m_waypointTicks = 0;
            }
            m_waypointTicks++;
        }
        */

        /// <summary>
        /// Checks some waypoint and sets WayIndex to position with further visible waypoint from current position
        /// </summary>
        /// <param name="memory">Memory to check</param>
        /// <param name="direction">Direction to check - forward check is the same as direction in memory</param>
        /// <param name="checkDistance">How many waypoint to check</param>
        private void CheckMemoryList(ref MyWaypointMemory memory, int direction, int checkDistance)
        {

            int checkingIndex = memory.m_WayIndex;
            int finalIndex = memory.m_WayIndex;

            for (int i = 0; i < checkDistance; i++)
            {
                checkingIndex += direction;
                if ((checkingIndex < 0) || (checkingIndex >= memory.m_WaypointList.Count))
                    break;

                //if cant see this waypoint, try next one
                if (null != CanSee(memory.m_WaypointList[checkingIndex]))
                    continue;

                finalIndex = checkingIndex;
                i = 0;
            }

            //If we can only see waypoint just before this one, dont make any change
            var count = Math.Abs(finalIndex - memory.m_WayIndex) - 1;
            if (count < 1)
                return;

            //otherwise, delete redundant waypoints
            memory.m_WaypointList.RemoveRange(Math.Min(finalIndex, memory.m_WayIndex) + 1, count);
            memory.m_WayIndex += count * direction;
        }
        #endregion

        #region BotHandlers
        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_SmallShip_Bot objectBuilder/*, Matrix matrix, float health*/)
        {
            System.Diagnostics.Debug.Assert(objectBuilder.Faction != 0);
            if (objectBuilder.Faction == 0)
            {
                Trace.SendMsgLastCall(objectBuilder.EntityId.ToString());
            }
            base.Init(hudLabelText, objectBuilder/*, matrix, health*/);

            Faction = objectBuilder.Faction;

            // Array of params for all weapons on ship 
            m_botWeaponParamsAllSlots = new MyBotParams[Enum.GetNames(typeof(MyMwcObjectBuilder_FireKeyEnum)).Length];

            //Assignment of constants for each weapon
            foreach (var item in objectBuilder.AssignmentOfAmmo)
            {
                //Set params for weapon in selected slot
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo = item.AmmoType;
                MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? weaponTypeNullable = MyGuiSmallShipHelpers.GetWeaponType(item.AmmoType, item.Group);
                MyMwcObjectBuilder_FireKeyEnum fireKey = item.FireKey;

                if (weaponTypeNullable != null)
                {
                    var weaponType = (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum)weaponTypeNullable;
                    MyBotParams botParamsOneSlot = SettingsForWeaponType(weaponType, ammo);
                    botParamsOneSlot.FireKey = fireKey;
                    m_botWeaponParamsAllSlots[(int)fireKey - 1] = botParamsOneSlot;

                    //store all weapon types bot has
                    m_weaponTypesMounted |= weaponType;
                }
            }

            //always 0 super easy 1 max hard
            MyGameplayDifficultyEnum difficultyOfThisBot = GetBotDifficulty();

            DifficultyStrafingSpeed = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotStrafingSpeed;

            DifficultyMovingSpeed = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotMovingSpeed;

            DifficultyFireRatio = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotFireRatio;

            //HACK BECAUSE OF TEST BUILD..
	        DifficultyGunUsageRatio = 0.5f;
            //DifficultyGunUsageRatio = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotGunUsageRatio;

            DifficultyRaidAttackOccurrence = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotRaidAttackOccurrence;

            DifficultyFlyAroundTargetOccurrence = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotFlyAroundTargetOccurrence;

            DifficultyAttackReactDistance = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotAttackReactDistance;

            DifficultyUseFireBurst = 1;

            MyHud.ChangeText(this, new StringBuilder(hudLabelText), null, 10000, MyHudIndicatorFlagsEnum.SHOW_ALL);

            NormalDecision = MyBotDecisionLogicsEnum.None;
            AttackDecision = MyBotDecisionLogicsEnum.None;

            //bot survival waypoints if getting lost
            m_rememberWaypoints = new MyWaypointMemory(this);
            m_temporaryWaypoints = new MyWaypointMemory(this);

            m_canSeeCheck = new MyCanSeeMemory(false);
            m_canSeeCheckForShoot = new MyCanSeeMemory(false);
            m_canSeeCheckForMove = new MyCanSeeMemory(false);

            //Set behavrior duration times
            SettingsForBehaviorTimers();

            //Inicialize timers
            ResetBehaviourTimer();
            ResetStrafeTimer();

            m_decisionTimeCounter = 0;
            m_lostCounter = 0;

            Threat = null;
            Target = null;
            FollowTarget = null;

            //Next inicialization should be done from input so every bot can do something different (shuold it be in object builder for bot?)
            //--------------------------------------------------------
            //Set default decision logic
            NormalDecision = MyBotDecisionLogicsEnum.Guard;
            AttackDecision = MyBotDecisionLogicsEnum.CasualFighter;

            m_survivalKit = new Dictionary<MyBotSurvivalAlertsEnum, MySurvivalDecision>();
            AddSurvivalAlert(MyBotSurvivalAlertsEnum.EnemyInsight, MyBotDecisionLogicsEnum.None, 1000);
            AddSurvivalAlert(MyBotSurvivalAlertsEnum.UnderAttack, MyBotDecisionLogicsEnum.CasualFighter, 1000);
            
            //Initialize waypoint memories
            m_savedWaypoints = new MyWaypointMemory(this);

            Trace.SendMsgLastCall("Init Bot " + base.Name);
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void Update()
        {
            if (MyFakes.BOT_DEBUG_VIEW)
            {
                MyHud.ChangeText(this, new StringBuilder(string.Format("{0} {1} {2} {3} T: {4} Thr: {5}", Name, Behaviour, SurvivalDecision, Health,
                    Target != null ? (Target == MySession.PlayerShip ? "player" : Target.Name): "no target",
                    Threat != null ? (Threat == MySession.PlayerShip ? "player" : Threat.Name) : "no threat")));
            }

            base.Update();
           
            if (IsDead())
            {
                return;
            }

            SettingsForBehaviorTimers();

            // if bots are disabled, do not update
            if (MyGuiScreenGamePlay.Static.IsEditorActive() || MyMwcFinalBuildConstants.DisableEnemyBots)
            {
                return;
            }

            if (Target != null && Target.Health <= 0)
                Target = null;
            if (Threat != null && Threat.Health <= 0)
                Threat = null;
            
            // autoleveling on
            if (Config.Engine.On && !Config.AutoLeveling.On && (Target != null || FollowTarget != null) )
            {
                Config.AutoLeveling.ChangeValueUp();
            }

            // autoleveling off when no target
            if (Config.Engine.On && Config.AutoLeveling.On && (Target != null || FollowTarget != null))
            {
                Config.AutoLeveling.ChangeValueUp();
            }

            //0002539: Bug - enemy bots - why they do not align to my horisontal orientation? friends do it
            //hotfix for release
            if(MySession.PlayerShip != null)
                SetLevelingMatrix(MySession.PlayerShip.WorldMatrix);
            /*
            //allign autolevel matrix priorly to follow then to attack target
            if ((m_target != null && DistanceToTarget(m_target.GetPosition()) < MAX_LEVELING_DISTANCE))
            {
                SetLevelingMatrix(m_target.WorldMatrix);
            } else if ((m_followTarget != null && DistanceToTarget(m_followTarget.GetPosition()) < MAX_LEVELING_DISTANCE))
            {
                SetLevelingMatrix(m_followTarget.WorldMatrix);
            }
            */

            /*
            //Save position if we need it
            if (m_rememberWaypoint)//???
                RememberPosition();
            */

            //  If now is the right time, call survival and decision logic's update
            if ((m_decisionTimeCounter++ > m_decisionMaxTime) || (m_behaviourTimeCounter++ > m_behaviourMaxTime && m_minBehaviorsTimesInTicks[(int)Behaviour] > 0))
            {
                CheckSurvivalKit();

                MakeDecision();
                m_decisionTimeCounter = 0;

                ResetBehaviourTimer();
            }
            //else
                //m_target = null;
            
            //If bot has flight target, get distance and angle to it
            if (Target != null)//why not null at default?
            {
                // Calculate bot to target distance.
                m_distanceToTarget = DistanceToTarget(Target.GetPosition());

                //Calculate bot's anglet o target.
                m_angleToTarget = AngleTo(Target.GetPosition());
            }

            //  Call update method of current behavior, if there is any.
            if ((Behaviour != MyBotBehaviorsEnum.None))
            {
                ResolveBehavior();
                if (m_isFiring) 
                    Shoot();
            }
            ResolveMovement();

            //
            //if (IS_BOT_DEBUG_MODE_ACTIVE)
            //{
            //    MyBotDebugUtils.ThirdPersonCameraDelta(new Vector3(0, 100, 200));
            //}

            RefillAmmoAndResources();
        }

        /// <summary>
        /// Draw bot and associated visuals.
        /// </summary>
        /// <returns>An ununsed true result.</returns>
        public override bool Draw()
        {
            // Draw this bot.
            bool result = base.Draw();

            // If debug mode is active, draw debug helpers.
            if (MyMwcFinalBuildConstants.BOT_DEBUG_MODE)
            {
                // Draw debug geometry.
                MyBotDebugUtils.DrawForward(WorldMatrix);
                MyBotDebugUtils.DrawLocalCoordinateAxes(WorldMatrix);
            }

            // Returns an (unused) true result.
            return result;
        }

        public override bool DebugDraw()
        {
            if (!MyFakes.BOT_DEBUG_VIEW)
                return base.DebugDraw();

            if (m_flightTarget != null && MyMwcFinalBuildConstants.BOT_DEBUG_MODE)
            {
                MyDebugDraw.DrawLine3D(this.GetPosition(), m_flightTarget.Value, Color.BlueViolet, Color.Gold);
                return true;
            }

            //draw my target,
            if (Target != null)
            {
                Matrix mat = Matrix.Identity;
                mat.Translation = Target.WorldMatrix.Translation;
                MyDebugDraw.DrawLine3D(WorldMatrix.Translation, Target.WorldMatrix.Translation, Color.Green, Color.Red);
                MyDebugDraw.DrawSphereWireframe(mat.Translation, WorldVolume.Radius, Color.Red.ToVector3(), 1.0f);
            }
            //draw my go-to,
            //?
            if (m_flightTarget != null)
            {
                Matrix mat = Matrix.Identity;
                mat.Translation = m_flightTarget.Value;
                MyDebugDraw.DrawLine3D(WorldMatrix.Translation, m_flightTarget.Value, Color.Green, Color.Green);
                MyDebugDraw.DrawSphereWireframe(m_flightTarget.Value, WorldVolume.Radius / 2, Color.Green.ToVector3(), 1.0f);
            }

            MyDebugDraw.DrawLine3D(WorldMatrix.Translation, WorldMatrix.Translation + WorldMatrix.Forward * 10000, Color.Green, Color.Red);
            
            //draw my aim target

            return base.DebugDraw();
        }

        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);
        }

        /// <summary>
        /// Movement and shooting
        /// </summary>
        public override void UpdateBeforeIntegration()
        {
            base.UpdateBeforeIntegration();

        }

        /// <summary>
        /// Shooting moved to beforeintegration, only calling base function now
        /// </summary>
        /// <returns></returns>
        public override bool UpdateAfterIntegration()
        {
            return base.UpdateAfterIntegration();
        }

        /// <summary>
        /// Resets strafe timer to count from zero
        /// </summary>
        public void ResetStrafeTimer()
        {
            float MinStrafeTimeInTicks = 4;//Math.Lerp(5f, 10f, DifficultyStrafingSpeed);
            float MaxStrafeTimeInTicks = 5;//Math.Lerp(5f, 10f, DifficultyStrafingSpeed);
            m_strafeMaxTime = MyMwcUtils.GetRandomFloat(MinStrafeTimeInTicks, MaxStrafeTimeInTicks);
            m_strafeTimeCounter = 0;
        }

        public void ResetRaidTimer()
        {
            float MinRaidTimeInTicks = MathHelper.Lerp(4, 8, 1-DifficultyRaidAttackOccurrence);
            float MaxRaidTimeInTicks = MathHelper.Lerp(5, 15, 1-DifficultyRaidAttackOccurrence);
            m_raidMaxTime = MyMwcUtils.GetRandomFloat(MinRaidTimeInTicks, MaxRaidTimeInTicks);
            m_raidTimeCounter = 0;
        }

        /// <summary>
        /// Resets behaviour timer to count from zero
        /// </summary>
        public void ResetBehaviourTimer()
        {
            m_behaviourMaxTime = MyMwcUtils.GetRandomFloat(m_minBehaviorsTimesInTicks[(int)Behaviour], m_maxBehaviorsTimesInTicks[(int)Behaviour]);
            m_behaviourTimeCounter = 0;
        }
        #endregion

        #region SensorMethods

        private bool CheckCanSee(ref MyCanSeeMemory mem, MyEntity target)
        {
            if (mem.m_ticksFromLastCheck >= FREQUENCY_FOR_RAYCAST_TICKS)
            {
                mem.m_result = CanSee(target) == null;
                mem.m_ticksFromLastCheck = 0;
            }
            mem.m_ticksFromLastCheck++;
            return mem.m_result;
        }

        private bool CheckCanSeeIgnoreMovingObjects(ref MyCanSeeMemory mem, Vector3 target)
        {
            if (mem.m_ticksFromLastCheck >= FREQUENCY_FOR_RAYCAST_TICKS||true)
            {
                mem.m_result = CanSeeIgnoreMovingObj(target);
                mem.m_ticksFromLastCheck = 0;
            }
            mem.m_ticksFromLastCheck++;
            return mem.m_result;
        }

        /// <summary>
        /// Calculate distance between bot and target. ... public because of TestMission
        /// </summary>
        /// <param name="target">Target entity.</param>
        /// <returns>Distance between bot and ship.</returns>
        public float DistanceToTarget(Vector3 target)
        {
            return Vector3.Distance(GetPosition(), target);
        }

        /// <summary>
        /// Angle between the bot's forward axis and an imganary line drawn from bot to player. Example 1: If this angle
        /// is zero, player is exactly in front of bot. Example 2: At 45 degrees player is in front of bot but is either
        /// up/down/left/right of bot a pretty good amount (not really lined up for a shot).
        /// </summary>
        /// <param name="target">Target position.</param>
        /// <returns>Angle, in degrees.</returns>
        private float AngleTo(Vector3 target)
        {
            Vector3 targetRelativePosition = target - WorldMatrix.Translation;

            if (targetRelativePosition.Length() < MyMwcMathConstants.EPSILON)
                return 0;

            Vector3 v1 = MyMwcUtils.Normalize(WorldMatrix.Forward);
            Vector3 v2 = MyMwcUtils.Normalize(targetRelativePosition);
            return MathHelper.ToDegrees(MyUtils.GetAngleBetweenVectors(v1, v2));
        }

        /// <summary>
        /// This checks to see if bot is "too close" to player. We have this so that bot doesn't just always ram 
        /// into the player. We want to get a sense of being too close. If so, we can act on that knowledge to 
        /// correct the problem. For example: kill thrusters or even reverse them and back off. Bot consider distance 
        /// between players as "OK" in 10% range for both directions
        /// </summary>
        /// <returns></returns>
        ///
        
        private bool CanSeeIgnoreMovingObj(Vector3 position)
        {
            return CanSeeIgnoreMovingObj(position, null);
        }

        private bool CanSeeIgnoreMovingObj(Vector3 position, MyEntity target)
        {
            var result = CanSee(position, target);
            return (result == null) || IsMovingObject(result);
        }

        private bool IsMovingObject(MyEntity obj)
        {
            return obj != null && obj.Physics != null && obj.Physics.LinearVelocity != Vector3.Zero;
        }

        /// <summary>
        /// Finds some nearby player or bot to follow ( if am i follower ) 
        /// </summary>
        /// <returns></returns>
        private MySmallShip SearchTargetToFollow()
        {
            //  Search for friend to follow (in case of player)
            MySmallShip target = null;
            if (
                DistanceToTarget(MySession.PlayerShip.GetPosition()) < MAX_FOLLOW_DISTANCE
                && CheckCanSee(ref m_canSeeCheck, MySession.PlayerShip)
                && MyFactions.GetFactionsRelation(this.Faction, MySession.PlayerShip.Faction) == MyFactionRelationEnum.Friend
                )
            {
                return target = MySession.PlayerShip;
            }

            // ( find other non follwing bot / leader )
            //
            //find our free bot to follow
            using (var rbFounded = PoolList<MyRBElement>.Get())
            {
                MyPruningStructure prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();
                float myboxd = MAX_FOLLOW_DISTANCE / 2.0f;
                BoundingBox rbInputElementGetWorldSpaceAABB = new BoundingBox(
                    GetPosition() - new Vector3(myboxd, myboxd, myboxd),
                    GetPosition() + new Vector3(myboxd, myboxd, myboxd));
                prunningStructure.OverlapRBAllBoundingBox(ref rbInputElementGetWorldSpaceAABB, rbFounded);
                foreach (MyRBElement rb in rbFounded)
                {
                    MyEntity ent = ((MyGameRigidBody)rb.GetRigidBody().m_UserData).Entity;
                    if (ent == this)
                        continue;

                    if (ent is MySmallShipBot)
                    {
                        MySmallShipBot bot = ent as MySmallShipBot;
                        if (MyFactions.GetFactionsRelation(this.Faction, ent.Faction) == MyFactionRelationEnum.Friend)
                        {
                            if (!bot.IsDead() && bot.GetObjectWeFollow() == null && bot.NormalDecision != MyBotDecisionLogicsEnum.Follower)
                                return target = bot;
                        }
                    }

                }
            }

            return target;
        }

        /// <summary>
        /// Finds nearby enemy ship
        /// </summary>
        /// <returns></returns>
        private MyEntity SearchTargetToAttack(int triggerDistance)
        {
            //  Search for target to attack
            MyEntity target = null;
            float distance = float.PositiveInfinity;

            using (var rbFounded = PoolList<MyRBElement>.Get())
            {

                MyPruningStructure prunningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();

                BoundingBox rbInputElementGetWorldSpaceAABB = new BoundingBox(
                    this.WorldMatrix.Translation - new Vector3(triggerDistance ,triggerDistance ,triggerDistance ), 
                    this.WorldMatrix.Translation +new Vector3(triggerDistance ,triggerDistance ,triggerDistance ) );
                prunningStructure.OverlapRBAllBoundingBox(ref rbInputElementGetWorldSpaceAABB, rbFounded);
                
                //now try find spot
                foreach (MyRBElement rb in rbFounded)
                {
                    MyEntity ent = ((MyGameRigidBody)rb.GetRigidBody().m_UserData).Entity;
                    if (ent == this || ent == null)
                        continue;
                    if(ent is MySmallShip || ent is MyPrefabLargeShip)
                    {
                        if (MyFactions.GetFactionsRelation(this.Faction, ent.Faction) == MyFactionRelationEnum.Enemy)
                        {
                            var d = DistanceToTarget(ent.WorldMatrix.Translation);
                            if (d < distance && d < triggerDistance && CheckCanSee(ref m_canSeeCheck, ent))
                            {
                                distance = d;
                                target = ent;
                            }
                        }
                    }
                }
            }
            return  target;
        }

        private void GetTargetsRoute(MySmallShip target)
        {
            CleanRoute(ref m_temporaryWaypoints);
            // Get first visible index of target route
            int firstVisibleIndex = -1;
            for (int i = target.RouteMemory.GetCount() - 1; i >= 0; i--)
            {
                if (CanSee(target.RouteMemory.GetItem(i)) == null)
                {
                    firstVisibleIndex = i;
                    break;
                }
            }

            if (firstVisibleIndex < 0)
            {
                return;
            }

            for (int i = firstVisibleIndex; i < target.RouteMemory.GetCount(); i++)
            {
                m_temporaryWaypoints.m_WaypointList.Add(target.RouteMemory.GetItem(i));
            }

            m_temporaryWaypoints.m_GuardType = MyBotGuardTypeEnum.OneWay;
            m_temporaryWaypoints.m_WayIndex = 1;                            // First record is actual ship position for some reason
            m_temporaryWaypoints.m_checkDistance = 0;
            m_temporaryWaypoints.m_IndexDirection = 1;

            //for (int i = 0; i < target.RouteMemory.GetCount(); i++)
            //{
            //    m_temporaryWaypoints.m_WaypointList.Add(target.RouteMemory.GetItem(i));
            //}

            //m_temporaryWaypoints.m_GuardType = MyBotGuardTypeEnum.OneWay;
            //m_temporaryWaypoints.m_WayIndex = m_temporaryWaypoints.m_WaypointList.Count-1;
            //m_temporaryWaypoints.m_checkDistance = 0;
            //m_temporaryWaypoints.m_IndexDirection = 1;

            //while (m_temporaryWaypoints.m_WayIndex >= 0)
            //{
            //    if (CanSee(m_temporaryWaypoints.m_WaypointList[m_temporaryWaypoints.m_WayIndex]) == null)
            //        break;

            //    m_temporaryWaypoints.m_WayIndex--;
            //}
        }

        #endregion

        #region ActionMethods
        /// <summary>
        /// Calls MoveAndRotate method with parameters based on m_flightTarget:
        /// - rotates bot 
        /// - moves bot in desired direction
        /// - strafes when enemy around
        /// - keeps minimal distance from target
        /// </summary>
        private void ResolveMovement()
        {
            // Declare movement variables
            Vector3 moveIndicator = Vector3.Zero;
            Vector2 rotationIndicator = Vector2.Zero;
            float rollIndicator = 0f;

            if (Behaviour != MyBotBehaviorsEnum.None)
            {
                
                if (m_rolling) 
                    rollIndicator = MyMwcUtils.GetRandomFloat(-1, 1);
                
                // rotate to my target or rotate to my go to spot or do nothing
                if (Target != null && Behaviour != MyBotBehaviorsEnum.FlyAroundTarget && Behaviour != MyBotBehaviorsEnum.FollowWaypoint)
                {
                    if ((Target.WorldMatrix.Translation - WorldMatrix.Translation).Length() > WorldVolume.Radius)
                        rotationIndicator = RotationTowardsTarget(Target.WorldMatrix.Translation);
                }
                else if (m_flightTarget != null)
                {
                    if ((m_flightTarget.Value - WorldMatrix.Translation).Length() > WorldVolume.Radius)
                        rotationIndicator = RotationTowardsTarget(m_flightTarget.Value);
                }
                else
                    rotationIndicator = Vector2.Zero;
                MyUtils.AssertIsValid(rotationIndicator);

                //Trace.SendMsgLastCall(rotationIndicator.ToString());

                // fly to my target
                if (m_flightTarget != null)
                {
                    //reduce my moveIndicator by our current velocity
                    moveIndicator = MoveTowardsTarget(m_flightTarget.Value - Physics.LinearVelocity * MOVEMENT_PREDICTION_TUNER);
                    if (moveIndicator.Length() > 0)
                    {
                        float dR = moveIndicator.Length();

                        moveIndicator.Z *= -1;
                        moveIndicator.X *= -1;
                        moveIndicator.Y *= -1;
                        moveIndicator.Normalize();
                        moveIndicator *= Math.Min(dR * MathHelper.Lerp(0.3f, 1, DifficultyMovingSpeed), MAX_SPEED_AHEAD * MathHelper.Lerp(0.3f, 1, DifficultyMovingSpeed));
                    }
                    //just unification 
                    if ((m_flightTarget.Value - WorldMatrix.Translation).Length() / WorldVolume.Radius < 2.0f)
                        moveIndicator = Vector3.Zero;
                }
                //Trace.SendMsgLastCall(moveIndicator.ToString());
                MyUtils.AssertIsValid(moveIndicator);

            }
            MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, m_afterBurner);
        }

        private Vector3 MoveTowardsTarget(Vector3 position)
        {
            return Vector3.Transform(position, GetWorldMatrixInverted()); 
        }

        /// <summary>
        /// Transforms position from Vector3 to matrix and balances movements
        /// </summary>
        private Vector2 RotationTowardsTarget(Vector3 position)
        {
            Matrix defMatrix;
            Vector3 forward = Vector3.Forward;
            Vector3 up = Vector3.Up;

            var distancePredictionTuner = m_distanceToTarget / DEFAULT_DDISTANCE;

            //Compute with bots movement (strafing at most)
            position -= MOVEMENT_PREDICTION_TUNER * Physics.LinearVelocity * distancePredictionTuner;

            //Compute with targets movement
            if (Target != null)
                position += MOVEMENT_PREDICTION_TUNER * Target.Physics.LinearVelocity * distancePredictionTuner;

            Matrix.CreateWorld(ref position, ref forward, ref up, out defMatrix);
            return RotationTowardsTarget(defMatrix);
        }

        /// <summary>
        /// Computes rotation parameters towards target coordinates. It is done for move and rotate method.
        /// </summary>
        private Vector2 RotationTowardsTarget(Matrix targetPosition)
        {
            Vector2 dR = Vector2.Zero;
            Vector3 dP = Vector3.Transform(targetPosition.Translation, GetWorldMatrixInverted());
            bool behind = dP.Z > 0;
            if (dP.Length() > 0)
            {

                float l = dP.Length();
                dP.Normalize();
                dP *= -1;
                dR.X = dP.Y * MAX_ROTATE_SPEED_COEF * 0.5f;
                dR.Y = -dP.X * MAX_ROTATE_SPEED_COEF * 0.5f;
            }

            return dR;
        }

        float GetActiveDistanceToTarget(MyEntity e)
        {
            return (e.GetPosition() - GetPosition()).Length();
        }

        /// <summary>
        /// Shoot with all weapons we can
        /// TODO
        /// </summary>
        private void Shoot()
        {
            if (Target == null)
                return;

            if (!CheckCanSee(ref m_canSeeCheckForShoot, Target))
                return;

            foreach (var item in m_botWeaponParamsAllSlots)
            {
                var fire = false;
                if (item.WeaponsParams == null)
                    continue;
                if(item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back && DifficultyGunUsageRatio <= 0.75f)
                    continue;
                if(item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front && DifficultyGunUsageRatio <= 0.75f)
                    continue;
                if(item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher && DifficultyGunUsageRatio <= 0.75f)
                    continue;
                if(item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun && DifficultyGunUsageRatio <= 0.5f)
                    continue;
                if(item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon && DifficultyGunUsageRatio <= 0.75f)
                    continue;

                //m_ammoProperties.ExplosionRadius
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = Weapons.AmmoAssignments.GetAmmoType(item.FireKey);
                MyAmmoProperties m_ammoProperties = MyAmmoConstants.GetAmmoProperties(ammoType);
                float radius = m_ammoProperties.ExplosionRadius;

                //spot explosion fail
                if (item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher && GetActiveDistanceToTarget(Target) * 1.1 <= radius)
                    continue;

                //check if weapon is enabled, if bot is in range for firing this weapon and if bot has sufficient aim for selected weapon
                float distCheck = item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistance] * item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner];
                float angleCheck = FIRING_CONE_DEGREES * item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner];
                if (item.Enabled
                    && distCheck > m_distanceToTarget
                    && m_angleToTarget < angleCheck)
                {
                    int bl = MyMinerGame.TotalGamePlayTimeInMilliseconds - (int)item.LastShotMilliseconds;
                    //shoot in bursts or just single shots
                    if (item.BurstFiring && DifficultyUseFireBurst > 0 )
                    {
                        //fire in burst or wait for pause ends
                        if (!item.BurstTimer.IsTimeUp)
                        {
                            fire = true;
                            m_botWeaponParamsAllSlots[(int)item.FireKey - 1].LastShotMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        }
                        else if (
                            MyMinerGame.TotalGamePlayTimeInMilliseconds - item.LastShotMilliseconds > 
                            MathHelper.Lerp(5,1,DifficultyFireRatio)*item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] * (1 / item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner]))
                        {
                            item.BurstTimer.Reset();
                            item.BurstTimer.Start();
                        }
                    }
                    //If not burst firing, just shoot once and wait
                    else if(
                        MyMinerGame.TotalGamePlayTimeInMilliseconds - item.LastShotMilliseconds > 
                        MathHelper.Lerp(5,1,DifficultyFireRatio)*item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] * (1 / item.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner]))
                    {
                        m_botWeaponParamsAllSlots[(int)item.FireKey - 1].LastShotMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        fire = true;
                    }
                        /*
                    else
                        fire = true;
                        */
                }
                

                //Are we shooting? If yes, just fire this weapon
                if (fire/*||true*/)
                {
                    Weapons.Fire(item.FireKey);
                }
            }
        }

        #endregion

        #region SettingsMethods

        private float RescaleForCurrentMass(float param)
        {
            float modificator = m_shipTypeProperties.Physics.Mass / DEFAULT_MASS;
            return param * modificator;
        }


        /// <summary>
        /// Constants for all mountable weapons
        /// TODO: numeric params should be taken from editor later
        /// </summary>
        private MyBotParams SettingsForWeaponType(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            var settings = new MyBotParams { WeaponsParams = new float[Enum.GetNames(typeof(MyBotWeaponsParametrsEnum)).Length] };

            switch (weaponType)
            {
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 10.0f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.06f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyAutocanonConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.5f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.3f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 1.4f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyShotgunConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.3f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.4f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyARSConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.25f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyCannonConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.6f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.6f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMachineGunConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.25f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.45f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MySniperConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        settings.BurstFiring = false;
                        break;
                    }
                case (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back):
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 1.0f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = false;
                        settings.BurstFiring = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.5f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = false;
                        settings.BurstFiring = false;
                        break;
                    }
            }

            //Keep max trajectory and weapon type
            settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistance] = GetMaxTrajectoryForAmmoType(ammoType);
            settings.WeaponType = weaponType;

            //If weapon is set for burstFiring, start burst timer
            if (settings.BurstFiring)
            {
                settings.BurstTimer = new MyStopwatch((int)(settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] * settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] * 0.9f), (int)(settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] * settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] * 1.1f));
            }

            settings.LastShotMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            return settings;
        }

        /// <summary>
        /// Default settings for behavior timers are set here, TODO: take this from editor
        /// </summary>
        private void SettingsForBehaviorTimers()
        {
            float ticks = MyConstants.PHYSICS_STEPS_PER_SECOND;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.None] = 2f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.None] = 2.1f * ticks;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.Attack] = 5.5f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.Attack] = 8f * ticks;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FlyAroundTarget] = 1.0f * ticks * MathHelper.Lerp(1,4,1-DifficultyFlyAroundTargetOccurrence);
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FlyAroundTarget] = 3.2f * ticks * MathHelper.Lerp(1,4,1-DifficultyFlyAroundTargetOccurrence);

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FollowTarget] = 0.5f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FollowTarget] = 0.5f * ticks;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FollowWaypoint] = 0.5f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.FollowWaypoint] = 0.5f * ticks;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.RunFromTarget] = 15f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.RunFromTarget] = 20f * ticks;

            m_minBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.RaidAttack] = 15f * ticks;
            m_maxBehaviorsTimesInTicks[(int)MyBotBehaviorsEnum.RaidAttack] = 20f * ticks;
        }

        /// <summary>
        /// Returns Max trajectory for given ammo type
        /// TODO: Universal launcher ammo
        /// </summary>
        private float GetMaxTrajectoryForAmmoType(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            float maxTrajectory = MyAmmoConstants.GetAmmoProperties(ammoType).MaxTrajectory;

            return maxTrajectory;
        }
        #endregion

        private void RefillAmmoAndResources()
        {
            Oxygen = MaxOxygen;
            Electricity = MaxElectricity;
            Fuel = MaxFuel;

            foreach (Inventory.MyInventoryItem item in Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
            {
                item.Amount = item.MaxAmount;
            }
        }

        private bool CanDie(MyEntity damageSource)
        {
            if (MyGuiScreenGamePlay.Static != null && MySession.PlayerShip != null && damageSource != null)
            {
                var playerFactionRelation = MyFactions.GetFactionsRelation(this.Faction, damageSource.Faction);

                // Cheat enemy can't die
                if (playerFactionRelation == MyFactionRelationEnum.Enemy &&
                    MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.ENEMY_CANT_DIE))
                {
                    return false;
                }

                // Cheat neutral can't die
                if ((playerFactionRelation == MyFactionRelationEnum.Friend || playerFactionRelation == MyFactionRelationEnum.Neutral) &&
                    MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.FRIEND_NEUTRAL_CANT_DIE))
                {
                    return false;
                }
            }

            return true;
        }

        private MyGameplayDifficultyEnum GetBotDifficulty()
        {
            MyFactionRelationEnum relationToPlayer = MyFactions.GetFactionsRelation(MySession.PlayerShip.Faction, Faction);
            if (relationToPlayer == MyFactionRelationEnum.Friend)
            {
                switch (MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty)
                {
                    case MyGameplayDifficultyEnum.EASY:
                        return MyGameplayDifficultyEnum.HARD;
                    case MyGameplayDifficultyEnum.NORMAL:
                        return MyGameplayDifficultyEnum.NORMAL;
                    case MyGameplayDifficultyEnum.HARD:
                        return MyGameplayDifficultyEnum.EASY;
                }
            }

            return MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty;
        }

    }
}

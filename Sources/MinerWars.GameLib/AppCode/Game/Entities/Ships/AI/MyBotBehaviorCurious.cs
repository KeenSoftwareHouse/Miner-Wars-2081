
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorCurious : MyBotBehaviorBase
    {
        private enum CuriousState
        {
            GOTO,
            EXPLORE,
            EXPLORE_SEARCHING,
            FINISHED
        }

        private static float LOCATION_NEAR_DISTANCE_SQR = 100;
        private static float EXPLORATION_TIME = 10;
        private static float DISCOVER_TIME = 3;
        private static float IDENTIFY_DISTANCE_SQR = 200*200; 
        
        Vector3 m_location;
        CuriousState m_state;
        float m_explorationTime;
        Vector3 m_exploreTarget;
        float m_exploreTargetTimer;
        
        MyEntity m_target;
        bool m_targetVisible;
        float m_visibilityCheckTimer;
        float m_targetVisibleTime;

        MyGotoLocationHelper m_gotoLocationHelper = new MyGotoLocationHelper();

        public MyBotBehaviorCurious()
        {
            
        }

        internal override void Init(MySmallShipBot bot)
        {
            base.Init(bot);

            m_location = SourceDesire.GetLocation();
            m_state = CuriousState.GOTO;
            m_explorationTime = 0;

            m_target = SourceDesire.GetEnemy();

            if (m_target != null)
            {
	            Debug.Assert(!m_target.Closed);
                Debug.Assert(m_target != bot);
                //MyDialogues.Play(MyDialogueEnum.EAC_SURVEY_SITE_1800_RUSSIANSCREAM_02);
            }

            m_gotoLocationHelper.Init(bot, m_location);
        }

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            m_targetVisibleTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            //No need to have this here, because it runs on main thread
            //lock (MyEntities.EntityCloseLock)
            {
                UpdateTargetVisibility(bot);

                if (m_targetVisible && m_target != null)
                {
                    // If target is visible long enough, attack is initiated, this also happens when target is too close or target shoots
                    MySmallShip smallShip = m_target as MySmallShip;
                    if (m_targetVisibleTime > DISCOVER_TIME || 
                        (m_target.GetPosition() - bot.GetPosition()).LengthSquared() < IDENTIFY_DISTANCE_SQR ||
                        (smallShip != null && smallShip.Weapons != null && smallShip.Weapons.IsShooting()))
                    {
                        bot.AddSeenEnemy(m_target);
                        m_state = CuriousState.FINISHED;
                        return;
                    }
                }
            }

            switch (m_state)
            {
                // First stage - goto location
                case CuriousState.GOTO:
                    m_gotoLocationHelper.Update(bot);
                    
                    if (m_gotoLocationHelper.PathNotFound || Vector3.DistanceSquared(bot.GetPosition(), m_location) < LOCATION_NEAR_DISTANCE_SQR)
                    {
                        TrySwitchExploreSearch(bot);
                    }
                    break;
                
                // Second stage - try explore area
                case CuriousState.EXPLORE:
                    m_explorationTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    m_exploreTargetTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                    if (m_explorationTime > EXPLORATION_TIME)
                    {
                        m_state = CuriousState.FINISHED;
                    }
                    else if (m_exploreTargetTimer <= 0)
                    {
                        TrySwitchExploreSearch(bot);
                    }
                    else
                    {
                        float factor = MathHelper.Clamp((m_exploreTarget - bot.GetPosition()).Length() / 20, 0.15f, 0.45f);
                        bot.Move(m_exploreTarget, m_exploreTarget, GetUpPlane(bot), false, 1, 5, factor, slowRotation: true); 
                    }
                    
                    break;

                // Paralel searching for new explore location
                case CuriousState.EXPLORE_SEARCHING:
                    TrySwitchToExplore(bot);
                    
                    if (m_explorationTime > EXPLORATION_TIME)
                    {
                        m_state = CuriousState.FINISHED;
                    }
                    break;

                // Third stage - nothing interesting was found (IsInvalid is true)
                case CuriousState.FINISHED:
                default:
                    break;
            }
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.CURIOUS;
        }

        internal override bool ShouldFallAsleep(MySmallShipBot bot)
        {
            return base.ShouldFallAsleep(bot);
        }

        internal override bool IsInvalid()
        {
            return m_state == CuriousState.FINISHED;
        }

        internal bool TrySwitchExploreSearch(MySmallShipBot bot)
        {
            Vector3 flyTo = bot.GetPosition() + MyMwcUtils.GetRandomVector3Normalized() * MyMwcUtils.GetRandomFloat(0, 100);

            if (bot.TryTestPosition(flyTo, bot.GetPosition()))
            {
                m_state = CuriousState.EXPLORE_SEARCHING;
                return true;
            }
            return false;
        }

        void TrySwitchToExplore(MySmallShipBot bot)
        {
            Vector3? result = null;
            if (bot.TryGetTestPositionResult(ref result))
            {
                if (result.HasValue)
                {
                    m_exploreTarget = result.Value;
                    m_exploreTargetTimer = MyMwcUtils.GetRandomFloat(2, 3);
                    m_state = CuriousState.EXPLORE;
                }
                else
                {
                    bool success = TrySwitchExploreSearch(bot);
                    Debug.Assert(success);
                }
            }
        }

        internal Vector3 GetUpPlane(MySmallShipBot bot)
        {
            if (MySession.PlayerShip != null)
            {
                return MySession.PlayerShip.WorldMatrix.Up;
            }

            return bot.WorldMatrix.Up;
        }

        private void UpdateTargetVisibility(MySmallShipBot bot)
        {
            if (m_target != null && m_visibilityCheckTimer <= 0)
            {
                Debug.Assert(!m_target.Closed);

                MyLine line = new MyLine(bot.GetPosition(), m_target.GetPosition(), true);
                var result = MyEntities.GetIntersectionWithLine(ref line, bot, m_target, true, ignoreChilds: true);
                m_targetVisible = !result.HasValue;
                m_visibilityCheckTimer = 0.25f;
            }
            else
            {
                m_visibilityCheckTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }
        }
    }
}

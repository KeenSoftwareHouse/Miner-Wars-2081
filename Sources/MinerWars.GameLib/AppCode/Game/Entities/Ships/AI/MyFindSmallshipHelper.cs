using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    class MyFindSmallshipHelper
    {
        public bool PathNotFound;

        const float STUCK_DISTANCE = 3;
        const float STUCK_TIME = 0.5f;

        Vector3? m_followPosition = null;
        bool m_lookingForPosition = false;
        Vector3 m_stuckPosition;
        float m_stuckTimer;

        bool m_lookingForPath = false;
        List<Vector3> m_followPath = null;
        int m_currentPathPosition = 0;
        const float PATH_NEAR_DISTANCE_SQR = 100;

        public void Init(MySmallShipBot bot)
        {
            m_stuckPosition = bot.GetPosition();
            m_stuckTimer = 0;
            m_followPosition = null;
            m_followPath = null;
            PathNotFound = false;
        }

        private void CheckIfRoutePositionComputed(MySmallShipBot bot, MyEntity entityTarget)
        {
            // Wait for result (visible route position)
            if (bot.TryGetRoute(out m_followPosition))
            {
                m_lookingForPosition = false;

                if (!m_followPosition.HasValue)
                {
                    //PathNotFound = true;
                    m_lookingForPath = bot.TryFindPath(entityTarget.WorldAABB.GetCenter());  // can't get unstuck; try to find path using waypoints
                }

                ResetStuck(bot);
            }
        }

        private void FollowToRoutePosition(MySmallShipBot bot)
        {
            // Fly to visible position, if too close look for new visible position
            if (Vector3.DistanceSquared(m_followPosition.Value, bot.GetPosition()) < 5 * 5)
            {
                m_followPosition = null;
            }
            else
            {
                bot.Move(m_followPosition.Value, m_followPosition.Value, bot.WorldMatrix.Up, false, 1, 2);

                if (m_stuckTimer > STUCK_TIME)
                {
                    if (Vector3.DistanceSquared(bot.GetPosition(), m_stuckPosition) > STUCK_DISTANCE)
                    {
                        m_followPosition = null;
                    }
                    else
                    {
                        m_followPosition = m_stuckPosition + MyMwcUtils.GetRandomVector3Normalized() * 1000;
                    }
                }
                else if (Vector3.DistanceSquared(bot.GetPosition(), m_stuckPosition) > STUCK_DISTANCE)
                {
                    ResetStuck(bot);
                }
                else
                {
                    m_stuckTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                }
            }
        }

        private void CheckIfPathComputed(MySmallShipBot bot)
        {
            if (bot.TryGetPath(out m_followPath))
            {
                if (m_followPath == null)
                {
                    PathNotFound = true;
                }
                m_lookingForPath = false;
                m_currentPathPosition = 0;
                ResetStuck(bot);
            }
        }

        private void FollowPath(MySmallShipBot bot)
        {
            if (m_currentPathPosition >= m_followPath.Count)
            {
                m_followPath = null;  // reached the goal or got stuck
            }
            else if (Vector3.DistanceSquared(m_followPath[m_currentPathPosition], bot.GetPosition()) < PATH_NEAR_DISTANCE_SQR)
            {
                m_currentPathPosition++;  // next waypoint reached
            }
            else
            {
                bot.Move(m_followPath[m_currentPathPosition], m_followPath[m_currentPathPosition], bot.WorldMatrix.Up, false, 1, 2);

                if (m_stuckTimer > STUCK_TIME)
                {
                    if (Vector3.DistanceSquared(bot.GetPosition(), m_stuckPosition) > STUCK_DISTANCE)
                    {
                        m_currentPathPosition++;  // try skipping ahead to the next waypoint instead of getting stuck right away
                        ResetStuck(bot);
                    }
                    else
                    {
                        m_followPath[m_currentPathPosition] += MyMwcUtils.GetRandomVector3Normalized() * 1000;  // jitter the sub-goal
                    }
                }
                else if (Vector3.DistanceSquared(bot.GetPosition(), m_stuckPosition) > STUCK_DISTANCE)
                {
                    ResetStuck(bot);
                }
                else
                {
                    m_stuckTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                }
            }
        }

        public void Update(MySmallShipBot bot, MyEntity entityTarget)
        {
            var smallShipTarget = entityTarget as MySmallShip;

            if (!m_followPosition.HasValue && !m_lookingForPosition && m_followPath == null && !m_lookingForPath)
            {
                if (smallShipTarget != null)
                {
                    // Start looking for visible route position
                    m_lookingForPosition = bot.TryFindRoute(smallShipTarget);
                }
                else
                {
                    // No remembered position pathfinding
                    m_lookingForPath = bot.TryFindPath(entityTarget.WorldAABB.GetCenter());
                }
            }
            else if (m_lookingForPosition)
            {
                CheckIfRoutePositionComputed(bot, entityTarget);
            }
            else if (!m_lookingForPosition && m_followPosition.HasValue)
            {
                FollowToRoutePosition(bot);
            }
            else if (m_lookingForPath)
            {
                CheckIfPathComputed(bot);
            }
            else if (!m_lookingForPath && m_followPath != null)
            {
                FollowPath(bot);
            }
        }

        public bool GotPosition()
        {
            return m_followPosition != null || m_followPath != null;
        }

        private void ResetStuck(MySmallShip bot)
        {
            m_stuckPosition = bot.GetPosition();
            m_stuckTimer = 0;
        }
    }
}

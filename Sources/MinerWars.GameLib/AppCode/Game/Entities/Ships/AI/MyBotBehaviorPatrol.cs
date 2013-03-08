using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Sessions;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorPatrol : MyBotBehaviorBase
    {
        const float WAYPOINT_NEAR_DISTANCE_SQR = 100;
        //int processedWaypoints;
        bool m_forward = true;
        List<MyWayPoint> m_path = new List<MyWayPoint>();
        int m_lastBlockedEdgesChangeId = -1;
        int m_currentWayPointIndex = 0;

        bool m_targetVisible;
        float m_visibilityCheckTimer;
        bool m_isInvalid;
        MyFindSmallshipHelper findSmallship;
        private MyWayPoint m_lastWayPoint;
        MyWayPointPath m_lastWayPointPath;

        internal override void Init(MySmallShipBot bot)
        {
            base.Init(bot);

            findSmallship = new MyFindSmallshipHelper();
            //processedWaypoints = 0;
            if (bot.WaypointPath != null && bot.WaypointPath.WayPoints.Count > 0)
            {
                bot.CurrentWaypoint = bot.WaypointPath.WayPoints[0];
                m_lastWayPointPath = bot.WaypointPath;
            }
        }

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            if (ShouldFallAsleep(bot))
            {
                bot.IsSleeping = true;
                return;
            }

            bool pathChange = m_lastWayPointPath != bot.WaypointPath;
            if (pathChange) 
            {
                m_lastWayPointPath = bot.WaypointPath;
                m_currentWayPointIndex = 0;
            }

            bool cycle = bot.PatrolMode == MyPatrolMode.CYCLE;

            if (bot.WaypointPath != null && !bot.SuspendPatrol && bot.WaypointPath.WayPoints.Count > 0)
            {
                UpdateVisibility(bot, bot.CurrentWaypoint.GetPosition());

                if (!m_targetVisible)
                {
                    findSmallship.Update(bot, bot.CurrentWaypoint);
                    if (findSmallship.PathNotFound)
                    {
                        bot.IsSleeping = true;
                        m_isInvalid = true;
                    }
                }
                else
                {
                    bool blockedEdgesIdDirty = m_lastBlockedEdgesChangeId != MyWayPoint.BlockedEdgesChangeId;
                    m_path.Clear();
                    using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
                    {
                        m_path.AddRange(bot.WaypointPath.CompletePath(MyWayPoint.BlockedEdgesForBots, bot.CurrentWaypoint, false, cycle, !blockedEdgesIdDirty));
                    }
                    m_lastBlockedEdgesChangeId = MyWayPoint.BlockedEdgesChangeId;

                    if (blockedEdgesIdDirty)
                    {
                        if (bot.CurrentWaypoint == null)
                        {
                            m_currentWayPointIndex = 0;
                        }
                        else
                        {
                            m_currentWayPointIndex = m_path.IndexOf(bot.CurrentWaypoint);
                        }
                    }

                    // no path found
                    if (m_currentWayPointIndex == -1)
                    {
                        return;
                    }

                    bot.CurrentWaypoint = m_path[m_currentWayPointIndex];

                    if (Vector3.DistanceSquared(bot.GetPosition(), bot.CurrentWaypoint.GetPosition()) <= WAYPOINT_NEAR_DISTANCE_SQR)
                    {
                        if (bot.CurrentWaypoint.EntityId != null && m_lastWayPoint != bot.CurrentWaypoint)
                        {
                            m_lastWayPoint = bot.CurrentWaypoint;

                            MyScriptWrapper.BotReachedWaypoint(bot, bot.CurrentWaypoint);
                        }
                        //++processedWaypoints;

                        int count = m_path.Count;
                        switch (bot.PatrolMode)
                        {
                            case MyPatrolMode.CYCLE:
                                //bot.CurrentWaypointIndex = processedWaypoints % count;
                                m_currentWayPointIndex++;
                                if (m_currentWayPointIndex >= count)
                                {
                                    m_currentWayPointIndex = 0;
                                }
                                break;
                            case MyPatrolMode.PING_PONG:
                                if (count > 1)
                                {
                                    //bot.CurrentWaypointIndex = processedWaypoints % (count * 2 - 2);
                                    //if (bot.CurrentWaypointIndex >= count)
                                    //{
                                    //    bot.CurrentWaypointIndex = (count * 2 - 2) - bot.CurrentWaypointIndex;
                                    //}
                                    if (m_forward)
                                    {
                                        if (m_currentWayPointIndex < count - 1)
                                        {
                                            m_currentWayPointIndex++;
                                        }
                                        else
                                        {
                                            m_currentWayPointIndex--;
                                            m_forward = false;
                                        }
                                    }
                                    else
                                    {
                                        if (m_currentWayPointIndex > 0)
                                        {
                                            m_currentWayPointIndex--;
                                        }
                                        else
                                        {
                                            m_currentWayPointIndex++;
                                            m_forward = true;
                                        }
                                    }
                                }
                                else
                                {
                                    m_currentWayPointIndex = 0;
                                }
                                break;
                            case MyPatrolMode.ONE_WAY:
                                if (m_currentWayPointIndex < m_path.Count - 1)
                                {
                                    ++m_currentWayPointIndex;
                                }
                                break;
                        }

                        bot.CurrentWaypoint = m_path[m_currentWayPointIndex];
                    }

                    bot.Move(bot.CurrentWaypoint.GetPosition(), bot.CurrentWaypoint.GetPosition(), GetUpPlane(), false);
                    findSmallship.Init(bot);
                }
            }
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.PATROL;
        }

        internal override bool ShouldFallAsleep(MySmallShipBot bot)
        {
            // check distance from path
            if (bot.WaypointPath == null)
                return base.ShouldFallAsleep(bot);
            if (MySession.PlayerShip == null)
                return false;  // don't fall asleep if there's no player

            var playerPos = MySession.PlayerShip.GetPosition();
            bool cycle = bot.PatrolMode == MyPatrolMode.CYCLE;

            List<MyWayPoint> path = null;

            using (MyWayPoint.BlockedEdgesLock.AcquireSharedUsing())
            {
                path = bot.WaypointPath.CompletePath(MyWayPoint.BlockedEdgesForBots, bot.CurrentWaypoint, false, cycle);
            }

            for (int i = (cycle ? 0 : 1), j = (cycle ? path.Count - 1 : 0); i < path.Count; j = i++)
                if (MyMath.DistanceSquaredFromLineSegment(path[i].Position, path[j].Position, playerPos) < MyAIConstants.SLEEP_DISTANCE_FROM_PATH_SQUARED)
                    return false;

            return base.ShouldFallAsleep(bot);
        }

        internal Vector3 GetUpPlane()
        {
            if (MySession.PlayerShip != null)
            {
                return MySession.PlayerShip.WorldMatrix.Up;
            }

            return Vector3.Up;
        }

        internal override void DebugDraw()
        {
            if (m_path != null && m_path.Count > 0)
            {
                MyDebugDraw.DrawSphereWireframe(m_path[0].Position, 2, new Vector3(0, 1, 1), 1);
                for (int i = 1; i < m_path.Count; i++)
                {
                    MyDebugDraw.DrawSphereWireframe(m_path[i].Position, 2, new Vector3(0, 1, 1), 1);
                    MyDebugDraw.DrawLine3D(m_path[i - 1].Position, m_path[i].Position, Color.Blue, Color.Cyan);

                }
            }
        }

        private void UpdateVisibility(MySmallShipBot bot, Vector3 targetPosition)
        {
            if (m_visibilityCheckTimer <= 0)
            {
                if (bot.GetPosition() == targetPosition)
                {
                    m_targetVisible = true;


                }
                else
                {
                    MyLine line = new MyLine(bot.GetPosition(), targetPosition, true);
                    var result = MyEntities.GetIntersectionWithLine(ref line, bot, null, true, ignoreChilds: true);
                    m_targetVisible = !result.HasValue;
                }

                m_visibilityCheckTimer = 0.5f;
            }
            else
            {
                m_visibilityCheckTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }
        }

        internal override bool IsInvalid()
        {
            return m_isInvalid;
        }
    }
}

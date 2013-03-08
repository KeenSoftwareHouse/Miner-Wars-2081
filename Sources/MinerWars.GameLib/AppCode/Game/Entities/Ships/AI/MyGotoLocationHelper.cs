using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    class MyGotoLocationHelper
    {
        const float STUCK_DISTANCE = 3;
        const float STUCK_TIME = 0.5f;
        const float PATH_NEAR_DISTANCE_SQR = 100;

        public bool PathNotFound;

        Vector3 location;
        bool lookingForPath = false;
        List<Vector3> followPath = null;
        int currentPathPosition = 0;

        Vector3 stuckPosition;
        float stuckTimer;

        bool locationVisible;
        float locationVisibleCheckTimer;

        public void Init(MySmallShipBot bot, Vector3 location)
        {
            locationVisibleCheckTimer = 0;
            stuckPosition = bot.GetPosition();
            stuckTimer = 0;
            PathNotFound = false;
            this.location = location;
            currentPathPosition = 0;

            UpdateTargetVisibility(bot);
        }

        private void FollowPath(MySmallShipBot bot)
        {
            if (currentPathPosition >= followPath.Count)
            {
                followPath = null;  // reached the goal or got stuck
            }
            else if (Vector3.DistanceSquared(followPath[currentPathPosition], bot.GetPosition()) < PATH_NEAR_DISTANCE_SQR)
            {
                currentPathPosition++;  // next waypoint reached
            }
            else
            {
                bot.Move(followPath[currentPathPosition], followPath[currentPathPosition], bot.WorldMatrix.Up, false, 1, 2);

                if (stuckTimer > 3)
                {
                    if (Vector3.DistanceSquared(bot.GetPosition(), stuckPosition) > STUCK_DISTANCE)
                    {
                        currentPathPosition++;  // try skipping ahead to the next waypoint instead of getting stuck right away
                        ResetStuck(bot);
                    }
                    else
                    {
                        //followPath[currentPathPosition] += MyMwcUtils.GetRandomVector3Normalized() * 1000;  // jitter the sub-goal
                    }
                }
                else if (Vector3.DistanceSquared(bot.GetPosition(), stuckPosition) > STUCK_DISTANCE)
                {
                    ResetStuck(bot);
                }
                else
                {
                    stuckTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                }
            }
        }

        private void WaitForPath(MySmallShipBot bot)
        {
            if (bot.TryGetPath(out followPath))
            {
                if (followPath == null)
                {
                    PathNotFound = true;
                }
                lookingForPath = false;
                currentPathPosition = 0;
                ResetStuck(bot);
            }
        }

        private void ResetStuck(MySmallShipBot bot)
        {
            stuckPosition = bot.GetPosition();
            stuckTimer = 0;
        }

        private void FlyToLocation(MySmallShipBot bot)
        {
            bot.Move(location, location, GetUpPlane(bot), false, 1, 2);
        }

        public void Update(MySmallShipBot bot)
        {
            // Location visible, fly to it
            if (!lookingForPath)
            {
                UpdateTargetVisibility(bot);
                
                if (locationVisible)
                {
                    FlyToLocation(bot);
                    return;
                }
            }

            // Wait for path
            if (lookingForPath)
            {
                WaitForPath(bot);
            }
            // Path found, follow path
            else if (!lookingForPath && followPath != null)
            {
                FollowPath(bot);
            }
            // Start looking for path
            else if (!lookingForPath)
            {
                lookingForPath = bot.TryFindPath(location);
            }
        }

        private void UpdateTargetVisibility(MySmallShipBot bot)
        {
            if (locationVisibleCheckTimer <= 0)
            {
                MyLine line = new MyLine(bot.GetPosition(), location, true);
                var result = MyEntities.GetIntersectionWithLine(ref line, bot, null, true, ignoreChilds: true);
                locationVisible = !result.HasValue;
                locationVisibleCheckTimer = 0.25f;
            }
            else
            {
                locationVisibleCheckTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
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
    }
}

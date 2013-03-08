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

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorFollow : MyBotBehaviorBase
    {
        const float LOOK_DISTANCE = 60;
        const float AFTERBURNER_DISTANCE = 100.0f;
        const float CHECK_PERIOD = 1.5f;
        const float MIN_LEADER_DISTANCE = 40;

        float checkTimer = CHECK_PERIOD;
        Vector3 formationPosition;
        Vector3 formationUpVector;

        MyFindSmallshipHelper findSmallship;

        bool leaderVisible;
        float visibilityCheckTimer;

        public MyBotBehaviorFollow()
        {
            findSmallship = new MyFindSmallshipHelper();
        }

        internal override void Init(MySmallShipBot bot)
        {
            base.Init(bot);

            leaderVisible = true;
            visibilityCheckTimer = 0;
        }

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            if (bot.Leader != null)
            {
                if (ShouldFallAsleep(bot))
                {
                    bot.IsSleeping = true;
                    return;
                }

                Vector3 leaderToBot = bot.GetPosition() - bot.Leader.GetPosition();
                Vector3 formationPositionActual = bot.Leader.GetFormationPosition(bot);
                Vector3 botToFormationPosition = formationPositionActual - bot.GetPosition();

                float leaderDistance = leaderToBot.Length();
                float formationPositionDistance = botToFormationPosition.Length();

                Vector3 flyTo;
                if (formationPositionDistance > MyMwcMathConstants.EPSILON_SQUARED && leaderDistance > MyMwcMathConstants.EPSILON)
                {
                    float leaderFactor = MathHelper.Clamp(leaderDistance - 5, 0, 25) / 20;
                    flyTo = (1.0f - leaderFactor) * leaderToBot / leaderDistance + leaderFactor * botToFormationPosition / formationPositionDistance;
                    flyTo = MyMwcUtils.Normalize(flyTo);
                    flyTo = bot.GetPosition() + flyTo * formationPositionDistance;

                    // Update leader visibility
                    if (visibilityCheckTimer <= 0)
                    {
                        MyLine line = new MyLine(bot.GetPosition(), formationPositionActual, true);
                        leaderVisible = !MyEntities.GetIntersectionWithLine(ref line, bot, bot.Leader, true, ignoreSmallShips: true).HasValue;

                        visibilityCheckTimer = 0.5f;
                    }
                    else
                    {
                        visibilityCheckTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    }
                }
                else
                {
                    // Bot is on formation position
                    flyTo = bot.GetPosition() + bot.WorldMatrix.Forward;
                    leaderVisible = true;
                }
                 
                if (leaderVisible)
                {
                    bool afterburner = /*bot.Leader.IsAfterburnerOn() || */formationPositionDistance > AFTERBURNER_DISTANCE;
                    Vector3 lookTarget = formationPositionDistance < LOOK_DISTANCE ? formationPositionActual + bot.Leader.WorldMatrix.Forward * 5000 : formationPositionActual;

                    float factor = MathHelper.Clamp(formationPositionDistance / 200, 0.5f, 1.0f);

                    factor = factor * factor * factor;

                    bot.Move(flyTo, lookTarget, bot.Leader.WorldMatrix.Up, afterburner, 1, 25, factor, slowRotation: true);

                    checkTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                    findSmallship.Init(bot);
                }
                else
                {
                    if (leaderDistance > MIN_LEADER_DISTANCE)
                    {
                        findSmallship.Update(bot, bot.Leader);

                        if (findSmallship.PathNotFound)
                        {
							//We dont want our friends sleeping elsewhere
                          //  bot.IsSleeping = true;
                        }
                    }
                }
            }
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.FOLLOW;
        }

        internal override bool ShouldFallAsleep(MySmallShipBot bot)
        {
            // check whether the followed object is/can be asleep
            if (bot.Leader != null)
            {
                var leaderBot = bot.Leader as MySmallShipBot;
                if (leaderBot == null) return false;  // if I don't follow a bot, I don't fall asleep no matter what
                return leaderBot.IsSleeping;
            }
            return base.ShouldFallAsleep(bot);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorIdle : MyBotBehaviorBase
    {
        const float CHANGE_TIME_MIN = 1.0f;
        const float CHANGE_TIME_MAX = 6.0f;

        float timeToGoalChange = 0;

        Vector3 lookTarget;

        Matrix baseMatrix;

        internal override void Init(MySmallShipBot bot)
        {
            base.Init(bot);

            baseMatrix = bot.WorldMatrix;
            lookTarget = bot.GetPosition() + baseMatrix.Forward * 100;
        }

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            if (ShouldFallAsleep(bot))
            {
                bot.IsSleeping = true;
                return;
            }

            if (timeToGoalChange <= 0)
            {
                UpdateGoals(bot);
                timeToGoalChange = MyMwcUtils.GetRandomFloat(CHANGE_TIME_MIN, CHANGE_TIME_MAX);
            }
            else
            {
                timeToGoalChange -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }

            bot.Move(bot.GetPosition(), lookTarget, baseMatrix.Up, false, slowRotation: true);

        }

        void UpdateGoals(MySmallShipBot bot)
        {
            lookTarget = bot.GetPosition() + baseMatrix.Forward * 50 + baseMatrix.Right * MyMwcUtils.GetRandomFloat(-50, 50) + baseMatrix.Up * MyMwcUtils.GetRandomFloat(-50, 50);
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.IDLE;
        }
    }
}

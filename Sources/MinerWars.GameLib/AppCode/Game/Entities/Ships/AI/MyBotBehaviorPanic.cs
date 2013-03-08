using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorPanic : MyBotBehaviorBase
    {
        const float CHANGE_TIME_MIN = 0.25f;
        const float CHANGE_TIME_MAX = 1.25f;

        float timeToGoalChange = 0;

        Vector3 moveTarget;
        Vector3 lookTarget;
        Vector3 up;
        bool shoot;

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            if (timeToGoalChange <= 0)
            {
                UpdateGoals(bot);
                timeToGoalChange = MyMwcUtils.GetRandomFloat(CHANGE_TIME_MIN, CHANGE_TIME_MAX);
            }
            else
            {
                timeToGoalChange -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }

            bot.Move(moveTarget, lookTarget, up, false);
            if (shoot)
            {
                // just some default distance so that bot shoots (legacy shoot method)
                bot.Shoot(MyMwcObjectBuilder_FireKeyEnum.Primary);
            }
        }

        void UpdateGoals(MySmallShipBot bot)
        {
            moveTarget = bot.GetPosition() + MyMwcUtils.GetRandomVector3Normalized() * 1000;
            lookTarget = bot.GetPosition() + MyMwcUtils.GetRandomVector3Normalized() * 1000;
            up = MyMwcUtils.GetRandomVector3Normalized() * 1000;
            shoot = MyMwcUtils.GetRandomBool(2);
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.PANIC;
        }
    }
}

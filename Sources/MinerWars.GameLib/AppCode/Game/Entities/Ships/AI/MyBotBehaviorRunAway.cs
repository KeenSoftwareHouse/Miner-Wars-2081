using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorRunAway : MyBotBehaviorBase
    {
        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            MyEntity enemy = bot.GetClosestEnemy();
            if (enemy != null)
            {
                Vector3 enemyToBot = bot.GetPosition() - enemy.GetPosition();
                float distance = enemyToBot.Length();
                if (distance <= bot.RunAwayDistance)
                {
                    Vector3 enemyToBotDirection = enemyToBot / distance;
                    Vector3 escapeTarget = bot.GetPosition() + enemyToBotDirection * 1000;
                    bot.Move(escapeTarget, escapeTarget, bot.WorldMatrix.Up, false);
                }
            }
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.RUN_AWAY;
        }
    }
}

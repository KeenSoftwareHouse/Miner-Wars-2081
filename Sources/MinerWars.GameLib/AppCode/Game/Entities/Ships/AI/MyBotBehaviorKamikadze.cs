using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorKamikadze : MyBotBehaviorBase
    {
        const float AFTERBURNER_DISTANCE = 100.0f;

        internal override void Close(MySmallShipBot bot)
        {
            base.Close(bot);

            var drill = bot.Weapons.GetMountedDrill();
            if (drill != null && drill.CurrentState != MyDrillStateEnum.InsideShip)
            {
                drill.Eject();
            }
        }

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            MyEntity enemy = bot.GetEnemy();
            MyDrillBase drill = bot.Weapons.GetMountedDrill();

            if (enemy != null && drill != null)
            {
                Vector3 botToEnemy = enemy.GetPosition() - bot.GetPosition();
                float distance = botToEnemy.Length();
                if (distance <= (bot.DrillDistance + bot.WorldVolume.Radius + enemy.WorldVolume.Radius))
                {
                    if (drill.CurrentState == MyDrillStateEnum.InsideShip)
                    {
                        drill.Eject();
                    }
                    else
                    {
                        drill.Shot(null);
                    }
                }
                else
                {
                    // pull drill back
                    if (drill.CurrentState == MyDrillStateEnum.Activated)
                    {
                        drill.Eject();
                    }
                }

                bot.Move(enemy.GetPosition(), enemy.GetPosition(), enemy.WorldMatrix.Up, distance < 100);
            }
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.KAMIKADZE;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyLowHealthDesire : MyBotDesire
    {
        public MyLowHealthDesire()
            : base(BotDesireType.LOW_HEALTH)
        {

        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return !bot.IsDamagedForWarnignAlert();
        }
    }
}

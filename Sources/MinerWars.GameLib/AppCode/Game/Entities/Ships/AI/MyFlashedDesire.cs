using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World.Global;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyFlashedDesire : MyBotDesire
    {
        int m_flashDurationInMS;

        public MyFlashedDesire(int flashDurationInMS)
            : base(BotDesireType.FLASHED)
        {
            m_flashDurationInMS = flashDurationInMS;
        }

        public override void Update()
        {
            base.Update();

            m_flashDurationInMS -= MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return m_flashDurationInMS <= 0;
        }
    }
}

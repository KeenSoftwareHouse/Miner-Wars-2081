using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyKillDesire : MyBotDesire
    {
        public MyEntity Target;
        private bool m_entityRemoved;
        private bool m_invalidated;

        public MyKillDesire(MyEntity target)
            : base(BotDesireType.KILL)
        {
            Target = target.GetBaseEntity();
        }

        public override void OnEntityRemove(MyEntity entity)
        {
            base.OnEntityRemove(entity);

            if (entity == Target)
            {
                m_entityRemoved = true;
            }
        }

        public override MyEntity GetEnemy()
        {
            return Target;
        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return m_invalidated || m_entityRemoved || Target == null || Target.IsDead() || bot.IsSpoiledHologram(Target) || !Target.Activated;
        }

        public void Invalidate()
        {
            m_invalidated = true;
        }
    }
}

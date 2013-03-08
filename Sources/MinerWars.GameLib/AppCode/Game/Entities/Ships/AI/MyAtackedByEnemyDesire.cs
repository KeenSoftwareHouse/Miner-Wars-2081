using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyAttackedByEnemyDesire : MyBotDesire
    {
        public MyEntity DamageSource { get; set; }
        private bool invalid;

        public MyAttackedByEnemyDesire(MyEntity damageSource)
            : base(BotDesireType.ATACKED_BY_ENEMY)
        {
            DamageSource = damageSource.GetBaseEntity();
        }

        public override MyEntity GetEnemy()
        {
            return DamageSource;
        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return invalid || DamageSource.IsDead() || bot.IsLeaderLost() || bot.IsSpoiledHologram(DamageSource) || MyFactions.GetFactionsRelation(bot, DamageSource) != MyFactionRelationEnum.Enemy || !DamageSource.Activated;
        }

        public override void OnEntityRemove(MyEntity entity)
        {
            base.OnEntityRemove(entity);

            if (entity == DamageSource)
            {
                invalid = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MySeeEnemyDesire : MyBotDesire
    {
        public MyEntity Enemy { get; set; }
        
        private bool invalid;

        public MySeeEnemyDesire(MyEntity enemy)
            : base(BotDesireType.SEE_ENEMY)
        {
            System.Diagnostics.Debug.Assert(!enemy.Closed);
            Enemy = enemy.GetBaseEntity();
            invalid = false;
        }

        public override void OnEntityRemove(MyEntity entity)
        {
            base.OnEntityRemove(entity);

            if (entity == Enemy)
            {
                invalid = true;
            }
        }

        public override MyEntity GetEnemy()
        {
            return Enemy;
        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return base.IsInvalid(bot) || invalid || Enemy.IsDead() || bot.IsLeaderLost() || bot.IsSpoiledHologram(Enemy) || MyFactions.GetFactionsRelation(bot, Enemy) != MyFactionRelationEnum.Enemy || !Enemy.Activated || Enemy.AIPriority == -1;
        }
    }
}

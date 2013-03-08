using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World.Global;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyCuriousDesire : MyBotDesire
    {
        MyEntityIdentifier? EntityID { get; set; }
        public MyEntity Entity
        {
            get
            {
                return EntityID.HasValue ? MyEntities.GetEntityByIdOrNull(EntityID.Value) : null;
            }
        }

        public Vector3 Location { get; set; }

        private bool m_invalid;

        public MyCuriousDesire(MyEntity entity, Vector3 location)
            : base(BotDesireType.CURIOUS)
        {
            if (entity != null)
                EntityID = entity.GetBaseEntity().EntityId;
            Location = location;

            System.Diagnostics.Debug.Assert(entity == null || !entity.Closed);
        }

        public override void OnEntityRemove(MyEntity entity)
        {
            base.OnEntityRemove(entity);

            if (entity == Entity)
            {
                m_invalid = true;
            }
        }

        public override MyEntity GetEnemy()
        {
            return Entity;
        }

        public override Vector3 GetLocation()
        {
            return Location;
        }

        public override bool IsInvalid(MySmallShipBot bot)
        {
            return base.IsInvalid(bot) || Entity == null || bot.IsLeaderLost() || bot.IsSpoiledHologram(Entity);
        }
    }
}

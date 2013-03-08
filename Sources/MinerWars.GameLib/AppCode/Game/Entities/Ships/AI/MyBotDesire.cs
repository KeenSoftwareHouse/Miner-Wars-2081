
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal class MyBotDesire
    {
        public BotDesireType DesireType { get; set; }

        public MyBotDesire(BotDesireType desireType)
        {
            DesireType = desireType;
        }

        public virtual MyEntity GetEnemy()
        {
            return null;
        }

        public virtual Vector3 GetLocation()
        {
            return Vector3.Zero;
        }

        public virtual bool IsInvalid(MySmallShipBot bot)
        {
            return false;
        }

        public virtual void OnEntityRemove(MyEntity entity)
        {
        }

        public virtual void Update()
        {
        }
    }
}

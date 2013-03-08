using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorBase
    {
        internal MyBotDesire SourceDesire;

        internal virtual void Init(MySmallShipBot bot)
        {
        }

        internal virtual void Update(MySmallShipBot bot)
        {

        }

        internal virtual void Close(MySmallShipBot bot)
        {
            
        }

        internal virtual bool IsInvalid()
        {
            return false;
        }

        internal virtual BotBehaviorType GetBehaviorType()
        {
            throw new ApplicationException();
        }

        internal virtual bool ShouldFallAsleep(MySmallShipBot bot)
        {
            return bot.IsFarEnoughToBeAsleep();
        }

        internal virtual void DebugDraw() 
        {

        }

        internal virtual string GetDebugHudString()
        {
            string behaviorName = this.GetType().Name;
            if (behaviorName.StartsWith("MyBotBehavior"))
            {
                behaviorName = behaviorName.Substring(13);
            }

            string desireName = SourceDesire != null ? SourceDesire.GetType().Name : "";
            if (desireName.StartsWith("My") && desireName.EndsWith("Desire"))
            {
                desireName = " " + desireName.Substring(2, desireName.Length - 2 - 6);
            }

            return string.Format("{0}{1}", behaviorName, desireName);

        }
    }
}

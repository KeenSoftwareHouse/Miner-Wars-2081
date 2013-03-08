using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Tools
{
    abstract class MyNanoRepairToolBase
    {                
        public abstract void Use();

        protected void UseForEntity(MyEntity entity)
        {
            if (entity.IsDamaged() && entity.HealthRatio < MyGameplayConstants.NANO_REPAIR_TOOL_REPAIR_TO_HEALTH_MAX)
            {
                float repairHealth =
                    Math.Min(
                        entity.MaxHealth * MyGameplayConstants.NANO_REPAIR_TOOL_REPAIR_HEALTH_RATIO_PER_SEC *
                        MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS,
                        entity.MaxHealth * MyGameplayConstants.NANO_REPAIR_TOOL_REPAIR_TO_HEALTH_MAX - entity.Health);
                entity.AddHealth(repairHealth);
            }            
        }
    }
}

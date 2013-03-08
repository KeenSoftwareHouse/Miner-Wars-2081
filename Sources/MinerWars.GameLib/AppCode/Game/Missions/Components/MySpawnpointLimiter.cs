using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Missions.Components
{
    /// <summary>
    /// The given spawnpoints will spawn bots only if the bot count is below maxBotCount.
    /// </summary>
    class MySpawnpointLimiter : MyMissionComponent
    {
        private uint[] m_spawnpointIDs;
        public int MaxBotCount = 0;

        public MySpawnpointLimiter(uint[] spawnpointIDs, int maxBotCount)
        {
            m_spawnpointIDs = spawnpointIDs;
            MaxBotCount = maxBotCount;
        }

        public int CurrentBotCount
        {
            get {
                int count = 0;
                foreach (uint id in m_spawnpointIDs)
                {
                    MySpawnPoint spawnpoint;
                    if (MyEntities.TryGetEntityById(new MyEntityIdentifier(id), out spawnpoint))
                        count += spawnpoint.GetShipCount();
                }
                return count;
            }
        }

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);

            foreach (uint id in m_spawnpointIDs)
            {
                MySpawnPoint spawnpoint;
                if (MyEntities.TryGetEntityById(new MyEntityIdentifier(id), out spawnpoint))
                    spawnpoint.AddLimiter(this);
            }
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);

            foreach (uint id in m_spawnpointIDs)
            {
                MySpawnPoint spawnpoint;
                if (MyEntities.TryGetEntityById(new MyEntityIdentifier(id), out spawnpoint))
                    spawnpoint.RemoveLimiter(this);
            }
        }
    }
}

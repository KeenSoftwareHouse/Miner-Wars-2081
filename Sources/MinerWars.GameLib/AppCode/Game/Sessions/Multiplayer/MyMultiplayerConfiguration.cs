using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Game.Sessions
{
    class MyMultiplayerConfiguration
    {
        public TimeSpan RespawnTime = TimeSpan.FromSeconds(5);
        
        /// <summary>
        /// Number of position updates per second.
        /// Minimum value - position is not updated less times per second, than this value
        /// </summary>
        public int PositionTickRateMin = 2;

        /// <summary>
        /// Number of position updates per second
        /// Maximum value - position is not updated more times per second, than this value
        /// </summary>
        public int PositionTickRateMax = 20;

        public float RotationTickRate = 20;

        /// <summary>
        /// Number of projectile position updates
        /// </summary>
        public int ProjectilesTickRate = 20;
    }
}

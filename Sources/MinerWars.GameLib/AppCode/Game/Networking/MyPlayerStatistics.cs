using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using System.Reflection;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.World
{
    class MyPlayerStatistics
    {
        public void Init(MyMwcObjectBuilder_PlayerStatistics playerStatisticsObjectBuilder)
        {
            Debug.Assert(playerStatisticsObjectBuilder != null); 

            BulletsShot = playerStatisticsObjectBuilder.BulletsShot;
            FriendlyFire = playerStatisticsObjectBuilder.FriendlyFire;
            GamePlayTime = playerStatisticsObjectBuilder.GamePlayTime;
            HarvestedOre = playerStatisticsObjectBuilder.HarvestedOre;
            OxygenSpent = playerStatisticsObjectBuilder.OxygenSpent;
            PlayersKilled = playerStatisticsObjectBuilder.PlayersKilled;
            RescuedPlayers = playerStatisticsObjectBuilder.RescuedPlayers;
            TraveledDistance = playerStatisticsObjectBuilder.TraveledDistance;
            TunnelsDug = playerStatisticsObjectBuilder.TunnelsDug;
            Deaths = playerStatisticsObjectBuilder.Deaths;
        }

        public MyMwcObjectBuilder_PlayerStatistics GetObjectBuilder()
        {
            return new MyMwcObjectBuilder_PlayerStatistics(PlayersKilled, BulletsShot, HarvestedOre, TunnelsDug, TraveledDistance, RescuedPlayers, GamePlayTime, OxygenSpent, FriendlyFire, Deaths);
        }

        int m_deads;

        public int Deaths
        {
            get { return m_deads; }
            set { m_deads = value; }
        }

        int m_playersKilled;

        public int PlayersKilled
        {
            get
            {
                return m_playersKilled;
            }
            set
            {
                m_playersKilled = value;
            }
        }

        int m_bulletsShot;

        public int BulletsShot
        {
            get
            {
                return m_bulletsShot;
            }
            set
            {
                m_bulletsShot = value;
            }
        }

        int m_harvestedOre;

        public int HarvestedOre
        {
            get
            {
                return m_harvestedOre;
            }
            set
            {
                m_harvestedOre = value;
            }
        }

        int m_tunnelsDug;

        public int TunnelsDug
        {
            get
            {
                return m_tunnelsDug;
            }
            set
            {
                m_tunnelsDug = value;
            }
        }


        int m_traveledDistance;

        public int TraveledDistance
        {
            get
            {
                return m_traveledDistance;
            }
            set
            {
                m_traveledDistance = value;
            }
        }

        int m_rescuedPlayers;

        public int RescuedPlayers
        {
            get
            {
                return m_rescuedPlayers;
            }
            set
            {
                m_rescuedPlayers = value;
            }
        }

        float m_gamePlayTime;

        public float GamePlayTime
        {
            get
            {
                return m_gamePlayTime;
            }
            set
            {
                m_gamePlayTime = value;
            }
        }

        float m_oxygenSpent;

        public float OxygenSpent
        {
            get
            {
                return m_oxygenSpent;
            }
            set
            {
                m_oxygenSpent = value;
            }
        }

        int m_friendlyFire;

        public int FriendlyFire
        {
            get
            {
                return m_friendlyFire;
            }
            set
            {
                m_friendlyFire = value;
            }
        }
    }
}
